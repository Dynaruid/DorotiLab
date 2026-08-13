// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/object.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Rendering;

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

public class PaintingContext : global::Doroti.Generated.Framework.Painting.ClipContext
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
        DartRuntimePrimitives.Assert(() => ((RenderObject)child).isRepaintBoundary);
        DartRuntimePrimitives.Assert(() =>
            {
                child.debugRegisterRepaintBoundaryPaint(includedParent: debugAlsoPaintedParent, includedChild: true);
                return true;
            });
        var childLayer__4874 = ((OffsetLayer?)(object?)((RenderObject)child)._layerHandle.layer)!;
        if ((childLayer__4874 is null))
        {
            DartRuntimePrimitives.Assert(() => debugAlsoPaintedParent);
            DartRuntimePrimitives.Assert(() => (((RenderObject)child)._layerHandle.layer is null));
            OffsetLayer layer__5336 = child.updateCompositedLayer(oldLayer: null);
            ((RenderObject)child)._layerHandle.layer = childLayer__4874 = layer__5336;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (debugAlsoPaintedParent || childLayer__4874.attached));
            global::Doroti.Flutter.Ui.Offset? debugOldOffset__5530 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    debugOldOffset__5530 = childLayer__4874!.offset;
                    return true;
                });
            childLayer__4874.removeAllChildren();
            OffsetLayer updatedLayer__5704 = child.updateCompositedLayer(oldLayer: childLayer__4874);
            DartRuntimePrimitives.Assert(() => DartRuntimePrimitives.Identical(updatedLayer__5704, childLayer__4874));
            DartRuntimePrimitives.Assert(() => (object.Equals(debugOldOffset__5530, ((OffsetLayer)updatedLayer__5704).offset)));
        }
        child._needsCompositedLayerUpdate = false;
        DartRuntimePrimitives.Assert(() => DartRuntimePrimitives.Identical(childLayer__4874, ((RenderObject)child)._layerHandle.layer));
        DartRuntimePrimitives.Assert(() => (((RenderObject)child)._layerHandle.layer is OffsetLayer));
        DartRuntimePrimitives.Assert(() =>
            {
                childLayer__4874!.debugCreator = (((object?)((RenderObject)child).debugCreator ?? (object?)DartRuntimePrimitives.RuntimeType(child)));
                return true;
            });
        childContext ??= new PaintingContext(childLayer__4874, ((RenderObject)child).paintBounds);
        child._paintWithContext(childContext, Offset.zero);
        DartRuntimePrimitives.Assert(() => DartRuntimePrimitives.Identical(childLayer__4874, ((RenderObject)child)._layerHandle.layer));
        childContext.stopRecordingIfNeeded();
    }

    public static void updateLayerProperties(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() => (((RenderObject)child).isRepaintBoundary && ((RenderObject)child)._wasRepaintBoundary));
        DartRuntimePrimitives.Assert(() => !((RenderObject)child)._needsPaint);
        DartRuntimePrimitives.Assert(() => (((RenderObject)child)._layerHandle.layer is not null));
        var childLayer__7529 = ((OffsetLayer?)(object?)((RenderObject)child)._layerHandle.layer!)!;
        global::Doroti.Flutter.Ui.Offset? debugOldOffset__7596 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                debugOldOffset__7596 = ((OffsetLayer)childLayer__7529).offset;
                return true;
            });
        OffsetLayer updatedLayer__7721 = child.updateCompositedLayer(oldLayer: childLayer__7529);
        DartRuntimePrimitives.Assert(() => DartRuntimePrimitives.Identical(updatedLayer__7721, childLayer__7529));
        DartRuntimePrimitives.Assert(() => (object.Equals(debugOldOffset__7596, ((OffsetLayer)updatedLayer__7721).offset)));
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
                global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugOnProfilePaint?.Invoke(child);
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
        var childOffsetLayer__10630 = ((OffsetLayer?)(object?)((RenderObject)child)._layerHandle.layer!)!;
        childOffsetLayer__10630.offset = offset;
        appendLayer(childOffsetLayer__10630);
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
            var hasCanvas__11326 = (this._canvas is not null);
            DartRuntimePrimitives.Assert(() =>
                {
                    if (hasCanvas__11326)
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
            return hasCanvas__11326;
            return default!;
        }
    }
    public virtual global::Doroti.Flutter.Ui.PictureRecorder recorder
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
                if (global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugRepaintRainbowEnabled)
                {
                    var paint__14174 = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Flutter.Ui.Paint();
    __cascade.style = PaintingStyle.stroke;
    __cascade.strokeWidth = 6.0;
    __cascade.color = global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugCurrentRepaintColor.toColor();
    return __cascade;
}))();
                    this.canvas.drawRect(this.estimatedBounds.deflate(3.0), paint__14174);
                }
                if (global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugPaintLayerBordersEnabled)
                {
                    var paint__14444 = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Flutter.Ui.Paint();
    __cascade.style = PaintingStyle.stroke;
    __cascade.strokeWidth = 1.0;
    __cascade.color = new global::Doroti.Flutter.Ui.Color(4294940672L);
    return __cascade;
}))();
                    this.canvas.drawRect(this.estimatedBounds, paint__14444);
                }
                return true;
            });
        this._currentLayer!.picture = this._recorder!.endRecording();
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
        PaintingContext childContext__18877 = createChildContext(childLayer, (childPaintBounds ?? this.estimatedBounds));
        painter(childContext__18877, offset);
        childContext__18877.stopRecordingIfNeeded();
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
        global::Doroti.Flutter.Ui.Rect offsetClipRect__21651 = clipRect.shift(offset);
        if (needsCompositing)
        {
            ClipRectLayer layer__21746 = (oldLayer ?? new ClipRectLayer());
            ((Func<ClipRectLayer>)(() =>
{
    var __cascade = layer__21746;
    __cascade.clipRect = offsetClipRect__21651;
    __cascade.clipBehavior = clipBehavior;
    return __cascade;
}))();
            pushLayer(layer__21746, (Action<PaintingContext, Offset>)painter, offset, childPaintBounds: offsetClipRect__21651);
            return layer__21746;
        }
        else
        {
            clipRectAndPaint(offsetClipRect__21651, clipBehavior, offsetClipRect__21651, ((Action)(() => painter(this, offset))));
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
        global::Doroti.Flutter.Ui.Rect offsetBounds__23331 = bounds.shift(offset);
        global::Doroti.Flutter.Ui.RRect offsetClipRRect__23384 = clipRRect.shift(offset);
        if (needsCompositing)
        {
            ClipRRectLayer layer__23482 = (oldLayer ?? new ClipRRectLayer());
            ((Func<ClipRRectLayer>)(() =>
{
    var __cascade = layer__23482;
    __cascade.clipRRect = offsetClipRRect__23384;
    __cascade.clipBehavior = clipBehavior;
    return __cascade;
}))();
            pushLayer(layer__23482, (Action<PaintingContext, Offset>)painter, offset, childPaintBounds: offsetBounds__23331);
            return layer__23482;
        }
        else
        {
            clipRRectAndPaint(offsetClipRRect__23384, clipBehavior, offsetBounds__23331, ((Action)(() => painter(this, offset))));
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
        global::Doroti.Flutter.Ui.Rect offsetBounds__25224 = bounds.shift(offset);
        global::Doroti.Flutter.Ui.RSuperellipse offsetShape__25285 = clipRSuperellipse.shift(offset);
        if (needsCompositing)
        {
            ClipRSuperellipseLayer layer__25395 = (oldLayer ?? new ClipRSuperellipseLayer());
            ((Func<ClipRSuperellipseLayer>)(() =>
{
    var __cascade = layer__25395;
    __cascade.clipRSuperellipse = offsetShape__25285;
    __cascade.clipBehavior = clipBehavior;
    return __cascade;
}))();
            pushLayer(layer__25395, (Action<PaintingContext, Offset>)painter, offset, childPaintBounds: offsetBounds__25224);
            return layer__25395;
        }
        else
        {
            clipRSuperellipseAndPaint(offsetShape__25285, clipBehavior, offsetBounds__25224, ((Action)(() => painter(this, offset))));
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
        global::Doroti.Flutter.Ui.Rect offsetBounds__26993 = bounds.shift(offset);
        global::Doroti.Flutter.Ui.Path offsetClipPath__27045 = clipPath.shift(offset);
        if (needsCompositing)
        {
            ClipPathLayer layer__27140 = (oldLayer ?? new ClipPathLayer());
            ((Func<ClipPathLayer>)(() =>
{
    var __cascade = layer__27140;
    __cascade.clipPath = offsetClipPath__27045;
    __cascade.clipBehavior = clipBehavior;
    return __cascade;
}))();
            pushLayer(layer__27140, (Action<PaintingContext, Offset>)painter, offset, childPaintBounds: offsetBounds__26993);
            return layer__27140;
        }
        else
        {
            clipPathAndPaint(offsetClipPath__27045, clipBehavior, offsetBounds__26993, ((Action)(() => painter(this, offset))));
            return null;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ColorFilterLayer pushColorFilter(Offset offset, ColorFilter colorFilter, Action<PaintingContext, Offset> painter, ColorFilterLayer? oldLayer = null)
    {
        ColorFilterLayer layer__28482 = (oldLayer ?? new ColorFilterLayer());
        layer__28482.colorFilter = colorFilter;
        pushLayer(layer__28482, (Action<PaintingContext, Offset>)painter, offset);
        return layer__28482;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TransformLayer? pushTransform(bool needsCompositing, Offset offset, Matrix4 transform, Action<PaintingContext, Offset> painter, TransformLayer? oldLayer = null)
    {
        var effectiveTransform__29518 = ((Func<Matrix4>)(() =>
{
    var __cascade = Matrix4.translationValues(offset.dx, offset.dy, 0.0);
    __cascade.multiply(transform);
    __cascade.translateByDouble(-offset.dx, -offset.dy, 0, 1);
    return __cascade;
}))();
        if (needsCompositing)
        {
            TransformLayer layer__29732 = (oldLayer ?? new TransformLayer());
            layer__29732.transform = effectiveTransform__29518;
            pushLayer(layer__29732, (Action<PaintingContext, Offset>)painter, offset, childPaintBounds: MatrixUtils.inverseTransformRect(effectiveTransform__29518, this.estimatedBounds));
            return layer__29732;
        }
        else
        {
            ((Func<Canvas>)(() =>
{
    var __cascade = this.canvas;
    __cascade.save();
    __cascade.transform(effectiveTransform__29518.storage);
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
        OpacityLayer layer__31294 = (oldLayer ?? new OpacityLayer());
        ((Func<OpacityLayer>)(() =>
{
    var __cascade = layer__31294;
    __cascade.alpha = alpha;
    __cascade.offset = offset;
    return __cascade;
}))();
        pushLayer(layer__31294, (Action<PaintingContext, Offset>)painter, Offset.zero);
        return layer__31294;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "PaintingContext"))}#{GetHashCode()}(layer: {this._containerLayer}, canvas bounds: {this.estimatedBounds})";
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

internal class _LocalSemanticsHandle__object : global::Doroti.Generated.Framework.Semantics.SemanticsHandle
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
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
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
    internal virtual bool _debugDoingLayout { get; set; } = false;
    internal virtual bool _debugDoingChildLayout { get; set; } = false;
    internal virtual bool _debugAllowMutationsToDirtySubtrees { get; set; } = false;
    internal virtual List<RenderObject> _nodesNeedingCompositingBitsUpdate { get; private set; } = new List<RenderObject>();
    internal virtual List<RenderObject> _nodesNeedingPaint { get; set; } = new List<RenderObject>();
    internal virtual bool _debugDoingPaint { get; set; } = false;
    internal virtual global::Doroti.Generated.Framework.Semantics.SemanticsOwner? _semanticsOwner { get; set; } = default;
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
        if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            DartMap<string, string>? debugTimelineArguments__44535 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    if (global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugEnhanceLayoutTimelineArguments)
                    {
                        debugTimelineArguments__44535 = new DartMap<string, string> { ["dirty count"] = $"{checked((long)(this._nodesNeedingLayout.Count))}", ["dirty list"] = $"{this._nodesNeedingLayout}" };
                    }
                    return true;
                });
            FlutterTimeline.startSync($"LAYOUT{this._debugRootSuffixForTimelineEventNames}", arguments: debugTimelineArguments__44535);
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
                List<RenderObject> dirtyNodes__45202 = this._nodesNeedingLayout;
                _nodesNeedingLayout = new List<RenderObject>();
                dirtyNodes__45202.sort(((a, b) => (((RenderObject)a).depth - ((RenderObject)b).depth)));
                for (var i__45381 = 0L; (i__45381 < checked((long)(dirtyNodes__45202.Count))); i__45381++)
                {
                    if (this._shouldMergeDirtyNodes)
                    {
                        _shouldMergeDirtyNodes = false;
                        if ((checked((long)(this._nodesNeedingLayout.Count)) != 0))
                        {
                            this._nodesNeedingLayout.AddRange(dirtyNodes__45202.Skip(checked((int)i__45381)).ToList());
                            break;
                        }
                    }
                    RenderObject node__45713 = dirtyNodes__45202[(int)(i__45381)];
                    if ((((RenderObject)node__45713)._needsLayout && (object.Equals(((RenderObject)node__45713).owner, this))))
                    {
                        node__45713._layoutWithoutResize();
                    }
                }
                _shouldMergeDirtyNodes = false;
            }
            DartRuntimePrimitives.Assert(() =>
                {
                    _debugDoingChildLayout = true;
                    return true;
                });
            foreach (PipelineOwner child__46135 in this._children)
            {
                child__46135.flushLayout();
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
            if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.finishSync();
            }
        }
    }

    internal virtual void _enableMutationsToDirtySubtrees(Action callback)
    {
        DartRuntimePrimitives.Assert(() => this._debugDoingLayout);
        bool? oldState__47152 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                oldState__47152 = this._debugAllowMutationsToDirtySubtrees;
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
                    _debugAllowMutationsToDirtySubtrees = DartRuntimePrimitives.RequireValue(oldState__47152);
                    return true;
                });
        }
    }

    public virtual void flushCompositingBits()
    {
        if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            FlutterTimeline.startSync($"UPDATING COMPOSITING BITS{this._debugRootSuffixForTimelineEventNames}");
        }
        this._nodesNeedingCompositingBitsUpdate.sort(((a, b) => (((RenderObject)a).depth - ((RenderObject)b).depth)));
        foreach (RenderObject node__48045 in this._nodesNeedingCompositingBitsUpdate)
        {
            if ((((RenderObject)node__48045)._needsCompositingBitsUpdate && (object.Equals(((RenderObject)node__48045).owner, this))))
            {
                node__48045._updateCompositingBits();
            }
        }
        this._nodesNeedingCompositingBitsUpdate.Clear();
        foreach (PipelineOwner child__48289 in this._children)
        {
            child__48289.flushCompositingBits();
        }
        DartRuntimePrimitives.Assert(() => (checked((long)(this._nodesNeedingCompositingBitsUpdate.Count)) == 0));
        if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            FlutterTimeline.finishSync();
        }
    }

    public virtual IEnumerable<RenderObject> nodesNeedingPaint => this._nodesNeedingPaint;
    public virtual bool debugDoingPaint => this._debugDoingPaint;
    public virtual void flushPaint()
    {
        if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            DartMap<string, string>? debugTimelineArguments__49986 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    if (global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugEnhancePaintTimelineArguments)
                    {
                        debugTimelineArguments__49986 = new DartMap<string, string> { ["dirty count"] = $"{checked((long)(this._nodesNeedingPaint.Count))}", ["dirty list"] = $"{this._nodesNeedingPaint}" };
                    }
                    return true;
                });
            FlutterTimeline.startSync($"PAINT{this._debugRootSuffixForTimelineEventNames}", arguments: debugTimelineArguments__49986);
        }
        try
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    _debugDoingPaint = true;
                    return true;
                });
            List<RenderObject> dirtyNodes__50566 = this._nodesNeedingPaint;
            _nodesNeedingPaint = new List<RenderObject>();
            foreach (var node__50726 in ((Func<List<RenderObject>>)(() =>
{
    var __cascade = dirtyNodes__50566;
    __cascade.sort(((a, b) => (((RenderObject)b).depth - ((RenderObject)a).depth)));
    return __cascade;
}))())
            {
                DartRuntimePrimitives.Assert(() => (((RenderObject)node__50726)._layerHandle.layer is not null));
                if ((((((RenderObject)node__50726)._needsPaint || ((RenderObject)node__50726)._needsCompositedLayerUpdate)) && (object.Equals(((RenderObject)node__50726).owner, this))))
                {
                    if (((RenderObject)node__50726)._layerHandle.layer!.attached)
                    {
                        DartRuntimePrimitives.Assert(() => ((RenderObject)node__50726).isRepaintBoundary);
                        if (((RenderObject)node__50726)._needsPaint)
                        {
                            PaintingContext.repaintCompositedChild(node__50726);
                        }
                        else
                        {
                            PaintingContext.updateLayerProperties(node__50726);
                        }
                    }
                    else
                    {
                        node__50726._skippedPaintingOnLayer();
                    }
                }
            }
            foreach (PipelineOwner child__51359 in this._children)
            {
                child__51359.flushPaint();
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
            if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.finishSync();
            }
        }
    }

    public virtual global::Doroti.Generated.Framework.Semantics.SemanticsOwner? semanticsOwner => this._semanticsOwner;
    public virtual long debugOutstandingSemanticsHandles => this._outstandingSemanticsHandles;
    public virtual global::Doroti.Generated.Framework.Semantics.SemanticsHandle ensureSemantics(Action? listener = null)
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
                _semanticsOwner = new global::Doroti.Generated.Framework.Semantics.SemanticsOwner(onSemanticsUpdate: this.onSemanticsUpdate!);
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
        if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
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
            List<RenderObject> nodesToProcess__56481 = ((Func<List<RenderObject>>)(() =>
{
    var __cascade = this._nodesNeedingSemanticsUpdate.where(((@object) => (!((RenderObject)@object)._needsLayout && (object.Equals(((RenderObject)@object).owner, this))))).ToList();
    __cascade.sort(((a, b) => (((RenderObject)a).depth - ((RenderObject)b).depth)));
    return __cascade;
}))();
            this._nodesNeedingSemanticsUpdate.Clear();
            if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.startSync("Semantics.updateChildren");
            }
            RenderObject? rootNode__56896 = this.rootNode;
            foreach (var node__56939 in nodesToProcess__56481)
            {
                if (((RenderObject)node__56939)._semantics.parentDataDirty)
                {
                    continue;
                }
                ((RenderObject)node__56939)._semantics.updateChildren();
            }
            if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.finishSync();
            }
            DartRuntimePrimitives.Assert(() =>
                {
                    DartRuntimePrimitives.Assert(() => ((checked((long)(nodesToProcess__56481.Count)) == 0) || (rootNode__56896 is not null)));
                    if ((rootNode__56896 is not null))
                    {
                        _RenderObjectSemantics__object.debugCheckForParentData(rootNode__56896);
                    }
                    return true;
                });
            if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.startSync("Semantics.ensureGeometry");
            }
            List<RenderObject> nodesToProcessGeometry__58239 = this._nodesNeedingSemanticsGeometryUpdate.where(((@object) => ((!((RenderObject)@object)._needsLayout && (object.Equals(((RenderObject)@object).owner, this))) && !((RenderObject)@object)._semantics.parentDataDirty))).ToList();
            this._nodesNeedingSemanticsGeometryUpdate.Clear();
            foreach (var node__59131 in nodesToProcessGeometry__58239)
            {
                if ((((RenderObject)node__59131)._semantics.shouldFormSemanticsNode && ((RenderObject)node__59131)._semantics.geometryDirty))
                {
                    continue;
                }
                if ((((RenderObject)node__59131)._semantics.shouldFormSemanticsNode && ((((RenderObject)node__59131)._isRelayoutBoundary ?? false))))
                {
                    ((RenderObject)node__59131)._semantics.geometry = null;
                    continue;
                }
                if (!((RenderObject)node__59131)._semantics.contributesToSemanticsTree)
                {
                    foreach (_RenderObjectSemantics__object child__59920 in ((RenderObject)node__59131)._semantics.mergeUp.OfType<_RenderObjectSemantics__object>())
                    {
                        if (((_RenderObjectSemantics__object)child__59920).shouldFormSemanticsNode)
                        {
                            child__59920.geometry = null;
                        }
                        else
                        {
                            foreach (_RenderObjectSemantics__object nodeInSubtree__60390 in ((_RenderObjectSemantics__object)child__59920)._children)
                            {
                                DartRuntimePrimitives.Assert(() => ((_RenderObjectSemantics__object)nodeInSubtree__60390).shouldFormSemanticsNode);
                                nodeInSubtree__60390.geometry = null;
                            }
                        }
                    }
                    continue;
                }
                foreach (_RenderObjectSemantics__object child__60932 in ((RenderObject)node__59131)._semantics._children)
                {
                    child__60932.geometry = null;
                }
            }
            var treeShapeToken__61432 = new object();
            var nodeToEnsureGeometry__61472 = new HashSet<_RenderObjectSemantics__object>();
            foreach (var node__61540 in nodesToProcessGeometry__58239)
            {
                ((RenderObject)node__61540)._semantics.computeAncestorInfo(treeShapeToken__61432);
                if ((((RenderObject)node__61540)._semantics.firstAncestorNodeWithCleanGeometry is not null))
                {
                    nodeToEnsureGeometry__61472.Add(((RenderObject)node__61540)._semantics.firstAncestorNodeWithCleanGeometry!);
                }
            }
            foreach (_RenderObjectSemantics__object node__61922 in ((Func<List<_RenderObjectSemantics__object>>)(() =>
{
    var __cascade = nodeToEnsureGeometry__61472.ToList();
    __cascade.sort(((a, b) => (((_RenderObjectSemantics__object)a).renderObject.depth - ((_RenderObjectSemantics__object)b).renderObject.depth)));
    return __cascade;
}))())
            {
                node__61922.ensureGeometry();
            }
            if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.finishSync();
            }
            if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.startSync("Semantics.ensureSemanticsNode");
            }
            foreach (RenderObject node__62368 in System.Linq.Enumerable.Reverse(nodesToProcess__56481))
            {
                ((RenderObject)node__62368)._semantics.computeAncestorInfo(treeShapeToken__61432);
                var targets__62478 = new List<_RenderObjectSemantics__object>();
                if (((RenderObject)node__62368)._semantics.geometryDirty)
                {
                    if ((((RenderObject)node__62368)._semantics.firstAncestorNodeWithCleanGeometry is not null))
                    {
                        targets__62478.Add(((RenderObject)node__62368)._semantics.firstAncestorNodeWithCleanGeometry!);
                    }
                }
                else
                {
                    if ((!((RenderObject)node__62368)._semantics.geometry!.isVisible && !((RenderObject)node__62368)._semantics.isRoot))
                    {
                        _RenderObjectSemantics__object? parentInSemanticsTree__63157 = ((RenderObject)node__62368)._semantics.parentInSemanticsTree;
                        if ((parentInSemanticsTree__63157 is not null))
                        {
                            if (!((_RenderObjectSemantics__object)parentInSemanticsTree__63157).geometryDirty)
                            {
                                targets__62478.Add(parentInSemanticsTree__63157);
                            }
                            else
                            {
                                _RenderObjectSemantics__object? firstAncestorNodeWithCleanGeometry__63464 = ((_RenderObjectSemantics__object)parentInSemanticsTree__63157).firstAncestorNodeWithCleanGeometry;
                                if ((firstAncestorNodeWithCleanGeometry__63464 is not null))
                                {
                                    targets__62478.Add(firstAncestorNodeWithCleanGeometry__63464);
                                }
                            }
                        }
                    }
                    targets__62478.Add(((RenderObject)node__62368)._semantics);
                }
                foreach (var target__63937 in targets__62478)
                {
                    if (((_RenderObjectSemantics__object)target__63937).parentDataDirty)
                    {
                        continue;
                    }
                    target__63937.ensureSemanticsNode();
                }
            }
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((rootNode__56896 is not null))
                    {
                        _RenderObjectSemantics__object.debugCheckForBuilds(((RenderObject)rootNode__56896)._semantics);
                    }
                    return true;
                });
            if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.finishSync();
            }
            this._semanticsOwner!.sendSemanticsUpdate();
            foreach (PipelineOwner child__64482 in this._children)
            {
                child__64482.flushSemantics();
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
            if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.finishSync();
            }
        }
    }

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
        foreach (PipelineOwner child__66280 in this._children)
        {
            child__66280.attach(manifold);
        }
    }

    public virtual void detach()
    {
        DartRuntimePrimitives.Assert(() => (this._manifold is not null));
        this._manifold!.removeListener(this._updateSemanticsOwner);
        _manifold = null;
        foreach (PipelineOwner child__66964 in this._children)
        {
            child__66964.detach();
        }
    }

    internal virtual bool _debugAllowChildListModifications => ((!this._debugDoingChildLayout && !this._debugDoingPaint) && !this._debugDoingSemantics);
    public virtual void adoptChild(PipelineOwner child)
    {
        DartRuntimePrimitives.Assert(() => (((PipelineOwner)child)._debugParent is null));
        DartRuntimePrimitives.Assert(() => !this._children.Contains(child));
        DartRuntimePrimitives.Assert(() => this._debugAllowChildListModifications);
        this._children.Add(child);
        if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
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
        if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
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
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
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
            bool? disposed__81144 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    disposed__81144 = this._debugDisposed;
                    return true;
                });
            return disposed__81144;
            return default!;
        }
    }
    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => !this._debugDisposed);
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
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
                var node__86054 = this;
                while ((((RenderObject)node__86054).parent is not null))
                {
                    node__86054 = ((RenderObject)node__86054).parent!;
                }
                DartRuntimePrimitives.Assert(() => (!object.Equals(node__86054, child)));
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
        RenderObject? debugPreviousActiveLayout__90009 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                debugPreviousActiveLayout__90009 = _debugActiveLayout;
                _debugActiveLayout = null;
                return true;
            });
        T result__90180 = inner();
        DartRuntimePrimitives.Assert(() =>
            {
                _debugActiveLayout = debugPreviousActiveLayout__90009;
                return true;
            });
        return result__90180;
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
            bool isMutationAllowed__92311 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    if (this._debugDisposed)
                    {
                        throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("A disposed RenderObject was mutated."), new DiagnosticsProperty<RenderObject>("The disposed RenderObject was", this, style: DiagnosticsTreeStyle.errorProperty) });
                    }
                    PipelineOwner? owner__92718 = this.owner;
                    if (((owner__92718 is null) || !((PipelineOwner)owner__92718).debugDoingLayout))
                    {
                        isMutationAllowed__92311 = true;
                        return true;
                    }
                    RenderObject? activeLayoutRoot__93065 = default!;
                    (activeLayoutRoot__93065, isMutationAllowed__92311) = (this._debugClosestMutationRoot ?? (null, false));
                    if (isMutationAllowed__92311)
                    {
                        return true;
                    }
                    RenderObject debugActiveLayout__93388 = RenderObject.debugActiveLayout!;
                    var culpritMethodName__93453 = (((RenderObject)debugActiveLayout__93388).debugDoingThisLayout ? "performLayout" : "performResize");
                    var culpritFullMethodName__93581 = $"{DartRuntimePrimitives.RuntimeType(debugActiveLayout__93388)}.{culpritMethodName__93453}";
                    if ((activeLayoutRoot__93065 is null))
                    {
                        throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"A {this.GetType()} was mutated in {culpritFullMethodName__93581}."), new ErrorDescription("The RenderObject was mutated when none of its ancestors is actively performing layout."), new DiagnosticsProperty<RenderObject>("The RenderObject being mutated was", this, style: DiagnosticsTreeStyle.errorProperty), new DiagnosticsProperty<RenderObject>($"The RenderObject that was mutating the said {this.GetType()} was", debugActiveLayout__93388, style: DiagnosticsTreeStyle.errorProperty) });
                    }
                    if ((object.Equals(activeLayoutRoot__93065, this)))
                    {
                        throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"A {this.GetType()} was mutated in its own {culpritMethodName__93453} implementation."), new ErrorDescription("A RenderObject must not re-dirty itself while still being laid out."), new DiagnosticsProperty<RenderObject>("The RenderObject being mutated was", this, style: DiagnosticsTreeStyle.errorProperty), new ErrorHint("Consider using the LayoutBuilder widget to dynamically change a subtree during layout.") });
                    }
                    var summary__95044 = new ErrorSummary($"A {this.GetType()} was mutated in {culpritFullMethodName__93581}.");
                    var isMutatedByAncestor__95137 = (object.Equals(activeLayoutRoot__93065, debugActiveLayout__93388));
                    var description__95210 = (isMutatedByAncestor__95137 ? $"A RenderObject must not mutate its descendants in its {culpritMethodName__93453} method." : "A RenderObject must not mutate another RenderObject from a different render subtree " + $"in its {culpritMethodName__93453} method.");
                    throw new FlutterError(new List<DiagnosticsNode> { summary__95044, new ErrorDescription(description__95210), new DiagnosticsProperty<RenderObject>("The RenderObject being mutated was", this, style: DiagnosticsTreeStyle.errorProperty), new DiagnosticsProperty<RenderObject>($"The {(isMutatedByAncestor__95137 ? "ancestor " : "")}RenderObject that was mutating the said {this.GetType()} was", debugActiveLayout__93388, style: DiagnosticsTreeStyle.errorProperty), new ErrorHint("Mutating the layout of another RenderObject may cause some RenderObjects in its subtree to be laid out more than once. " + "Consider using the LayoutBuilder widget to dynamically mutate a subtree during layout.") });
                });
            return isMutationAllowed__92311;
            return default!;
        }
    }
    public virtual RenderObject? debugLayoutParent
    {
        get
        {
            RenderObject? layoutParent__97313 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    layoutParent__97313 = this.parent;
                    return true;
                });
            return layoutParent__97313;
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
            if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode)
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
        for (RenderObject? node__104348 = this; ((node__104348 is not null) && (((RenderObject)node__104348)._isRelayoutBoundary is not null)); node__104348 = ((RenderObject)node__104348).parent)
        {
            bool alreadyMarkedNeedsLayout__104467 = (((RenderObject)node__104348)._needsLayout || ((RenderObject)node__104348)._debugDoingThisLayout);
            if (!alreadyMarkedNeedsLayout__104467)
            {
                return false;
            }
            if (DartRuntimePrimitives.RequireValue(((RenderObject)node__104348)._isRelayoutBoundary))
            {
                return true;
            }
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void markNeedsLayout()
    {
        DartRuntimePrimitives.Assert(() => this._debugCanPerformMutations);
        if (this._needsLayout)
        {
            DartRuntimePrimitives.Assert(() => _debugRelayoutBoundaryAlreadyMarkedNeedsLayout());
            return;
        }
        _needsLayout = true;
        if (this.owner is PipelineOwner owner__107012 && (((this._isRelayoutBoundary ?? false))))
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    if (global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugPrintMarkNeedsLayoutStacks)
                    {
                        global::Doroti.Generated.Framework.Foundation.AssertionsLibrary.debugPrintStack(label: $"markNeedsLayout() called for {this}");
                    }
                    return true;
                });
            ((PipelineOwner)owner__107012)._nodesNeedingLayout.Add(this);
            owner__107012.requestVisualUpdate();
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
        RenderObject parent__107989 = this.parent!;
        if (!this._doingThisLayoutWithCallback)
        {
            parent__107989.markNeedsLayout();
        }
        else
        {
            DartRuntimePrimitives.Assert(() => ((RenderObject)parent__107989)._debugDoingThisLayout);
        }
        DartRuntimePrimitives.Assert(() => (object.Equals(parent__107989, this.parent)));
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
        RenderObject? debugPreviousActiveLayout__109468 = default!;
        DartRuntimePrimitives.Assert(() => !this._debugMutationsLocked);
        DartRuntimePrimitives.Assert(() => !this._doingThisLayoutWithCallback);
        DartRuntimePrimitives.Assert(() => (this._debugCanParentUseSize is not null));
        DartRuntimePrimitives.Assert(() =>
            {
                _debugMutationsLocked = true;
                _debugDoingThisLayout = true;
                debugPreviousActiveLayout__109468 = _debugActiveLayout;
                _debugActiveLayout = this;
                if (global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugPrintLayouts)
                {
                    global::Doroti.Generated.Framework.Foundation.PrintLibrary.debugPrint($"Laying out (without resize) {this}");
                }
                return true;
            });
        try
        {
            performLayout();
            markNeedsSemanticsUpdate();
        }
        catch (Exception e__109998)
        {
            var stack__110001 = new System.Diagnostics.StackTrace();
            _reportException("performLayout", e__109998, stack__110001);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                _debugActiveLayout = debugPreviousActiveLayout__109468;
                _debugDoingThisLayout = false;
                _debugMutationsLocked = false;
                return true;
            });
        _needsLayout = false;
        markNeedsPaint();
    }

    public virtual void layout(Constraints constraints, bool parentUsesSize = false)
    {
        DartRuntimePrimitives.Assert(() => !this._debugDisposed);
        if ((!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode && global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugProfileLayoutsEnabled))
        {
            DartMap<string, string>? debugTimelineArguments__111903 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    if (global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugEnhanceLayoutTimelineArguments)
                    {
                        debugTimelineArguments__111903 = toDiagnosticsNode().toTimelineArguments();
                    }
                    return true;
                });
            FlutterTimeline.startSync($"{this.GetType()}", arguments: debugTimelineArguments__111903);
        }
        DartRuntimePrimitives.Assert(() => constraints.debugAssertIsValid(isAppliedConstraint: true, informationCollector: ((InformationCollector)(() =>
        {
            List<string> stack__112356 = new global::System.Diagnostics.StackTrace(true).ToString().split("\n");
            long? targetFrame__112422 = default!;
            Pattern layoutFramePattern__112459 = new RegExp("^#[0-9]+ +Render(?:Object|Box).layout \\(");
            for (var i__112552 = 0L; (i__112552 < checked((long)(stack__112356.Count))); i__112552 += 1L)
            {
                if ((layoutFramePattern__112459.matchAsPrefix(stack__112356[(int)(i__112552)]) is not null))
                {
                    targetFrame__112422 = (i__112552 + 1L);
                }
                else
                {
                    if ((targetFrame__112422 is not null))
                    {
                        long targetFrame__112422__value112715 = DartRuntimePrimitives.RequireValue(targetFrame__112422);
                        break;
                    }
                }
            }
            if (((targetFrame__112422 is not null) && (DartRuntimePrimitives.RequireValue(targetFrame__112422) < checked((long)(stack__112356.Count)))))
            {
                long targetFrame__112422__value112799 = DartRuntimePrimitives.RequireValue(targetFrame__112422);
                Pattern targetFramePattern__112878 = new RegExp("^#[0-9]+ +(.+)$");
                Match? targetFrameMatch__112952 = targetFramePattern__112878.matchAsPrefix(stack__112356[(int)(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(targetFrame__112422__value112799)))]);
                string? problemFunction__113051 = ((((targetFrameMatch__112952 is not null) && (targetFrameMatch__112952.groupCount > 0L))) ? targetFrameMatch__112952.group(1L) : stack__112356[(int)(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(targetFrame__112422__value112799)))].Trim());
                return new List<DiagnosticsNode> { new ErrorDescription($"These invalid constraints were provided to {this.GetType()}'s layout() " + "function by the following function, which probably computed the " + "invalid constraints in question:\n" + $"  {problemFunction__113051}") };
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
            DartRuntimePrimitives.Assert(() =>
                {
                    _debugDoingThisResize = this.sizedByParent;
                    _debugDoingThisLayout = !this.sizedByParent;
                    RenderObject? debugPreviousActiveLayout__114281 = _debugActiveLayout;
                    _debugActiveLayout = this;
                    debugResetSize();
                    _debugActiveLayout = debugPreviousActiveLayout__114281;
                    _debugDoingThisLayout = false;
                    _debugDoingThisResize = false;
                    return true;
                });
            if ((!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode && global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugProfileLayoutsEnabled))
            {
                FlutterTimeline.finishSync();
            }
            return;
        }
        _constraints = constraints;
        DartRuntimePrimitives.Assert(() => !this._debugMutationsLocked);
        DartRuntimePrimitives.Assert(() => !this._doingThisLayoutWithCallback);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugMutationsLocked = true;
                if (global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugPrintLayouts)
                {
                    global::Doroti.Generated.Framework.Foundation.PrintLibrary.debugPrint($"Laying out ({(this.sizedByParent ? "with separate resize" : "with resize allowed")}) {this}");
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
            catch (Exception e__115308)
            {
                var stack__115311 = new System.Diagnostics.StackTrace();
                _reportException("performResize", e__115308, stack__115311);
            }
            DartRuntimePrimitives.Assert(() =>
                {
                    _debugDoingThisResize = false;
                    return true;
                });
        }
        RenderObject? debugPreviousActiveLayout__115495 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                _debugDoingThisLayout = true;
                debugPreviousActiveLayout__115495 = _debugActiveLayout;
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
        catch (Exception e__115863)
        {
            var stack__115866 = new System.Diagnostics.StackTrace();
            _reportException("performLayout", e__115863, stack__115866);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                _debugActiveLayout = debugPreviousActiveLayout__115495;
                _debugDoingThisLayout = false;
                _debugMutationsLocked = false;
                return true;
            });
        _needsLayout = false;
        markNeedsPaint();
        if ((!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode && global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugProfileLayoutsEnabled))
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
            ContainerLayer? result__127483 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    result__127483 = ((LayerHandle<ContainerLayer>)this._layerHandle).layer;
                    return true;
                });
            return result__127483;
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
        RenderObject? parent__128718 = this.parent;
        if ((parent__128718 is not null))
        {
            if (((RenderObject)parent__128718)._needsCompositingBitsUpdate)
            {
                return;
            }
            if ((((!this._wasRepaintBoundary || !this.isRepaintBoundary)) && !((RenderObject)parent__128718).isRepaintBoundary))
            {
                parent__128718.markNeedsCompositingBitsUpdate();
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
        bool oldNeedsCompositing__129786 = this._needsCompositing;
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
            if ((oldNeedsCompositing__129786 != this._needsCompositing))
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
            if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode)
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
            if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode)
            {
                return false;
            }
            return this._needsCompositedLayerUpdate;
            return default!;
        }
    }
    public virtual void markNeedsPaint()
    {
        DartRuntimePrimitives.Assert(() => !this._debugDisposed);
        DartRuntimePrimitives.Assert(() => ((this.owner is null) || !this.owner!.debugDoingPaint));
        if (this._needsPaint)
        {
            return;
        }
        _needsPaint = true;
        if ((this.isRepaintBoundary && this._wasRepaintBoundary))
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    if (global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugPrintMarkNeedsPaintStacks)
                    {
                        global::Doroti.Generated.Framework.Foundation.AssertionsLibrary.debugPrintStack(label: $"markNeedsPaint() called for {this}");
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
            }
            else
            {
                DartRuntimePrimitives.Assert(() =>
                    {
                        if (global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugPrintMarkNeedsPaintStacks)
                        {
                            global::Doroti.Generated.Framework.Foundation.AssertionsLibrary.debugPrintStack(label: $"markNeedsPaint() called for {this} (root of render tree)");
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
        RenderObject? node__137213 = this.parent;
        while ((node__137213 is not null))
        {
            if (((RenderObject)node__137213).isRepaintBoundary)
            {
                if ((((RenderObject)node__137213)._layerHandle.layer is null))
                {
                    break;
                }
                if (((RenderObject)node__137213)._layerHandle.layer!.attached)
                {
                    break;
                }
                node__137213._needsPaint = true;
            }
            node__137213 = ((RenderObject)node__137213).parent;
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
        if ((!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode && global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugProfilePaintsEnabled))
        {
            DartMap<string, string>? debugTimelineArguments__140237 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    if (global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugEnhancePaintTimelineArguments)
                    {
                        debugTimelineArguments__140237 = toDiagnosticsNode().toTimelineArguments();
                    }
                    return true;
                });
            FlutterTimeline.startSync($"{this.GetType()}", arguments: debugTimelineArguments__140237);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if (this._needsCompositingBitsUpdate)
                {
                    RenderObject? parent__140625 = this.parent;
                    if ((parent__140625 is not null))
                    {
                        var visitedByParent__140691 = false;
                        parent__140625.visitChildren(((Action<RenderObject>)((child) =>
                        {
                            if ((object.Equals(child, this)))
                            {
                                visitedByParent__140691 = true;
                            }
                        })));
                        if (!visitedByParent__140691)
                        {
                            throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("A RenderObject was not visited by the parent's visitChildren " + "during paint."), parent__140625.describeForError("The parent was"), describeForError("The child that was not visited was"), new ErrorDescription("A RenderObject with children must implement visitChildren and " + "call the visitor exactly once for each child; it also should not " + "paint children that were removed with dropChild."), new ErrorHint("This usually indicates an error in the Flutter framework itself.") });
                        }
                    }
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Tried to paint a RenderObject before its compositing bits were " + "updated."), describeForError("The following RenderObject was marked as having dirty compositing " + "bits at the time that it was painted"), new ErrorDescription("A RenderObject that still has dirty compositing bits cannot be " + "painted because this indicates that the tree has not yet been " + "properly configured for creating the layer tree."), new ErrorHint("This usually indicates an error in the Flutter framework itself.") });
                }
                return true;
            });
        RenderObject? debugLastActivePaint__142446 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                _debugDoingThisPaint = true;
                debugLastActivePaint__142446 = _debugActivePaint;
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
        catch (Exception e__143033)
        {
            var stack__143036 = new System.Diagnostics.StackTrace();
            _reportException("paint", e__143033, stack__143036);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                debugPaint(context, offset);
                _debugActivePaint = debugLastActivePaint__142446;
                _debugDoingThisPaint = false;
                return true;
            });
        if ((!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode && global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugProfilePaintsEnabled))
        {
            FlutterTimeline.finishSync();
        }
    }

    public abstract global::Doroti.Flutter.Ui.Rect paintBounds { get; }
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
        List<RenderObject>? fromPath__148090 = default!;
        List<RenderObject>? toPath__148226 = default!;
        var from__148243 = this;
        RenderObject to__148273 = (target ?? this.owner!.rootNode!);
        while (!DartRuntimePrimitives.Identical(from__148243, to__148273))
        {
            long fromDepth__148358 = ((RenderObject)from__148243).depth;
            long toDepth__148398 = ((RenderObject)to__148273).depth;
            if ((fromDepth__148358 >= toDepth__148398))
            {
                RenderObject fromParent__148480 = (((RenderObject)from__148243).parent ?? throw new FlutterError($"{target} and {this} are not in the same render tree."));
                (fromPath__148090 ??= new List<RenderObject> { this }).Add(fromParent__148480);
                from__148243 = fromParent__148480;
            }
            if ((fromDepth__148358 <= toDepth__148398))
            {
                RenderObject toParent__148765 = (((RenderObject)to__148273).parent ?? throw new FlutterError($"{target} and {this} are not in the same render tree."));
                DartRuntimePrimitives.Assert(() => (target is not null));
                (toPath__148226 ??= new List<RenderObject> { target! }).Add(toParent__148765);
                to__148273 = toParent__148765;
            }
        }
        Matrix4? fromTransform__149121 = default!;
        if ((fromPath__148090 is not null))
        {
            DartRuntimePrimitives.Assert(() => (checked((long)(fromPath__148090.Count)) > 1L));
            fromTransform__149121 = Matrix4.identity();
            long lastIndex__149257 = ((target is null) ? (checked((long)(fromPath__148090.Count)) - 2L) : (checked((long)(fromPath__148090.Count)) - 1L));
            for (var index__149344 = lastIndex__149257; (index__149344 > 0L); index__149344 -= 1L)
            {
                fromPath__148090[(int)(index__149344)].applyPaintTransform(fromPath__148090[(int)((index__149344 - 1L))], fromTransform__149121);
            }
        }
        if ((toPath__148226 is null))
        {
            return (fromTransform__149121 ?? Matrix4.identity());
        }
        DartRuntimePrimitives.Assert(() => (checked((long)(toPath__148226.Count)) > 1L));
        var toTransform__149607 = Matrix4.identity();
        for (long index__149654 = (checked((long)(toPath__148226.Count)) - 1L); (index__149654 > 0L); index__149654 -= 1L)
        {
            toPath__148226[(int)(index__149654)].applyPaintTransform(toPath__148226[(int)((index__149654 - 1L))], toTransform__149607);
        }
        if ((toTransform__149607.invert() == 0L))
        {
            return Matrix4.zero();
        }
        return ((((Func<Matrix4?>)(() =>
{
    var __cascade = fromTransform__149121;
    __cascade.multiply(toTransform__149607);
    return __cascade;
}))()) ?? toTransform__149607);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Flutter.Ui.Rect? describeApproximatePaintClip(RenderObject child) => null;
    public virtual global::Doroti.Flutter.Ui.Rect? describeSemanticsClip(RenderObject? child) => null;
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

    public virtual void describeSemanticsConfiguration(global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration config)
    {
    }

    public virtual void sendSemanticsEvent(global::Doroti.Generated.Framework.Semantics.SemanticsEvent semanticsEvent)
    {
        if ((this.owner!.semanticsOwner is null))
        {
            return;
        }
        global::Doroti.Generated.Framework.Semantics.SemanticsNode? node__154880 = ((_RenderObjectSemantics__object)this._semantics).cachedSemanticsNode;
        if (((node__154880 is not null) && !((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node__154880).isMergedIntoParent))
        {
            node__154880.sendEvent(semanticsEvent);
        }
        else
        {
            if ((this.parent is not null))
            {
                this.parent!.sendSemanticsEvent(semanticsEvent);
            }
        }
    }

    public abstract global::Doroti.Flutter.Ui.Rect semanticBounds { get; }
    public virtual bool debugNeedsSemanticsUpdate
    {
        get
        {
            if (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                return false;
            }
            return ((_RenderObjectSemantics__object)this._semantics).parentDataDirty;
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Semantics.SemanticsNode? debugSemantics
    {
        get
        {
            if ((!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode && ((_RenderObjectSemantics__object)this._semantics).built))
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

    public virtual void assembleSemanticsNode(global::Doroti.Generated.Framework.Semantics.SemanticsNode node, global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration config, IEnumerable<global::Doroti.Generated.Framework.Semantics.SemanticsNode> children)
    {
        node.updateWith(config: config, childrenInInversePaintOrder: ((List<global::Doroti.Generated.Framework.Semantics.SemanticsNode>?)(object?)children)!);
    }

    public virtual void handleEvent(global::Doroti.Generated.Framework.Gestures.PointerEvent @event, HitTestEntry<HitTestTarget> entry)
    {
    }

    public virtual string toStringShort()
    {
        string header__159804 = global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
        if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            if (this._debugDisposed)
            {
                header__159804 += " DISPOSED";
                return header__159804;
            }
            var count__159963 = 0L;
            for (RenderObject? node__160008 = this; ((node__160008 is not null) && !((((RenderObject)node__160008)._isRelayoutBoundary ?? false))); node__160008 = ((RenderObject)node__160008).parent)
            {
                if ((((RenderObject)node__160008)._isRelayoutBoundary is null))
                {
                    count__159963 = -1L;
                    break;
                }
                count__159963 += 1L;
            }
            if ((count__159963 > 0L))
            {
                header__159804 += $" relayoutBoundary=up{count__159963}";
            }
            if (this._needsLayout)
            {
                header__159804 += " NEEDS-LAYOUT";
            }
            if (this._needsPaint)
            {
                header__159804 += " NEEDS-PAINT";
            }
            if (this._needsCompositingBitsUpdate)
            {
                header__159804 += " NEEDS-COMPOSITING-BITS-UPDATE";
            }
            if (!this.attached)
            {
                header__159804 += " DETACHED";
            }
        }
        return header__159804;
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
        properties.add(new DiagnosticsProperty<global::Doroti.Generated.Framework.Semantics.SemanticsNode>("semantics node", this.debugSemantics, defaultValue: null));
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

public interface RenderObjectWithChildMixin<ChildType> where ChildType : RenderObject
{
    ChildType? _child { get; set; }

    public bool debugValidateChild(RenderObject child);
    public ChildType? child { get; set; }
    public void attach(PipelineOwner owner);
    public void detach();
    public void redepthChildren();
    public void visitChildren(Action<RenderObject> visitor);
    public List<DiagnosticsNode> debugDescribeChildren();
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

public interface ContainerRenderObjectMixin<ChildType, ParentDataType> where ChildType : RenderObject where ParentDataType : ContainerParentDataMixin<ChildType>
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
    global::Doroti.Generated.Framework.Semantics.SemanticsProperties _properties { get; set; }
    bool _container { get; set; }
    bool _explicitChildNodes { get; set; }
    bool _excludeSemantics { get; set; }
    bool _blockUserActions { get; set; }
    Locale? _localeForSubtree { get; set; }
    global::Doroti.Generated.Framework.Semantics.AttributedString? _attributedLabel { get; set; }
    global::Doroti.Generated.Framework.Semantics.AttributedString? _attributedValue { get; set; }
    global::Doroti.Generated.Framework.Semantics.AttributedString? _attributedIncreasedValue { get; set; }
    global::Doroti.Generated.Framework.Semantics.AttributedString? _attributedDecreasedValue { get; set; }
    global::Doroti.Generated.Framework.Semantics.AttributedString? _attributedHint { get; set; }
    TextDirection? _textDirection { get; set; }

    public void initSemanticsAnnotations(global::Doroti.Generated.Framework.Semantics.SemanticsProperties properties, bool container, bool explicitChildNodes, bool excludeSemantics, bool blockUserActions, Locale? localeForSubtree, TextDirection? textDirection);
    public global::Doroti.Generated.Framework.Semantics.SemanticsProperties properties { get; set; }
    public bool container { get; set; }
    public bool explicitChildNodes { get; set; }
    public bool excludeSemantics { get; set; }
    public bool blockUserActions { get; set; }
    public global::Doroti.Flutter.Ui.Locale? localeForSubtree { get; set; }
    public void _updateAttributedFields(global::Doroti.Generated.Framework.Semantics.SemanticsProperties value);
    public global::Doroti.Generated.Framework.Semantics.AttributedString? _effectiveAttributedLabel(global::Doroti.Generated.Framework.Semantics.SemanticsProperties value);
    public global::Doroti.Generated.Framework.Semantics.AttributedString? _effectiveAttributedValue(global::Doroti.Generated.Framework.Semantics.SemanticsProperties value);
    public global::Doroti.Generated.Framework.Semantics.AttributedString? _effectiveAttributedIncreasedValue(global::Doroti.Generated.Framework.Semantics.SemanticsProperties value);
    public global::Doroti.Generated.Framework.Semantics.AttributedString? _effectiveAttributedDecreasedValue(global::Doroti.Generated.Framework.Semantics.SemanticsProperties value);
    public global::Doroti.Generated.Framework.Semantics.AttributedString? _effectiveAttributedHint(global::Doroti.Generated.Framework.Semantics.SemanticsProperties value);
    public global::Doroti.Flutter.Ui.TextDirection? textDirection { get; set; }
    public void visitChildrenForSemantics(Action<RenderObject> visitor);
    public void describeSemanticsConfiguration(global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration config);
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
    public virtual global::Doroti.Generated.Framework.Semantics.AccessibilityFocusBlockType? accessibilityFocusBlockType { get; private set; }
    public virtual bool explicitChildNodes { get; private set; } = default!;
    public virtual HashSet<global::Doroti.Generated.Framework.Semantics.SemanticsTag>? tagsForChildren { get; private set; }
    public virtual Locale? localeForChildren { get; private set; }

    internal _SemanticsParentData__object(bool mergeIntoParent, bool blocksUserActions, bool explicitChildNodes, HashSet<global::Doroti.Generated.Framework.Semantics.SemanticsTag>? tagsForChildren, Locale? localeForChildren, global::Doroti.Generated.Framework.Semantics.AccessibilityFocusBlockType? accessibilityFocusBlockType)
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
        return (((((((__other is _SemanticsParentData__object) && (((_SemanticsParentData__object)((_SemanticsParentData__object)__other)).mergeIntoParent == this.mergeIntoParent)) && (((_SemanticsParentData__object)((_SemanticsParentData__object)__other)).blocksUserActions == this.blocksUserActions)) && (((_SemanticsParentData__object)((_SemanticsParentData__object)__other)).explicitChildNodes == this.explicitChildNodes)) && (object.Equals(((_SemanticsParentData__object)((_SemanticsParentData__object)__other)).localeForChildren, this.localeForChildren))) && (object.Equals(((_SemanticsParentData__object)((_SemanticsParentData__object)__other)).accessibilityFocusBlockType, this.accessibilityFocusBlockType))) && global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.setEquals<global::Doroti.Generated.Framework.Semantics.SemanticsTag>(((_SemanticsParentData__object)((_SemanticsParentData__object)__other)).tagsForChildren, this.tagsForChildren));
    }

    public override int GetHashCode()
    {
        return FoundationRuntimePorts.ObjectHash(this.mergeIntoParent, this.blocksUserActions, this.explicitChildNodes, this.localeForChildren, this.accessibilityFocusBlockType, Dart_coreLibrary.hashAllUnordered((this.tagsForChildren ?? new HashSet<global::Doroti.Generated.Framework.Semantics.SemanticsTag>())));
        return default!;
    }
}

public class _SemanticsConfigurationProvider__object
{
    internal virtual RenderObject _renderObject { get; private set; } = default!;
    internal virtual bool _isEffectiveConfigWritable { get; set; } = false;
    internal virtual global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration? _originalConfiguration { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration? _effectiveConfiguration { get; set; } = default;

    internal _SemanticsConfigurationProvider__object(RenderObject _renderObject)
    {
        this._renderObject = _renderObject;
    }

    public virtual bool wasSemanticsBoundary => (this._originalConfiguration?.isSemanticBoundary ?? false);
    public virtual global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration effective
    {
        get
        {
            return (this._effectiveConfiguration ?? this.original);
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration original
    {
        get
        {
            if ((this._originalConfiguration is null))
            {
                _effectiveConfiguration = _originalConfiguration = new global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration();
                this._renderObject.describeSemanticsConfiguration(this._originalConfiguration!);
                DartRuntimePrimitives.Assert(() => (!this._originalConfiguration!.explicitChildNodes || (this._originalConfiguration!.childConfigurationsDelegate is null)));
            }
            return this._originalConfiguration!;
            return default!;
        }
    }
    public virtual void updateConfig(Action<global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration> callback)
    {
        if (!this._isEffectiveConfigWritable)
        {
            _effectiveConfiguration = this.original.copy();
            _isEffectiveConfigWritable = true;
        }
        callback(this._effectiveConfiguration!);
    }

    public virtual void absorbAll(IEnumerable<global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration> configs)
    {
        updateConfig(((Action<global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration>)((config) =>
        {
            configs.forEach(((global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration)config).absorb);
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

    public abstract global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration? configToMergeUp { get; }
    public abstract _RenderObjectSemantics__object owner { get; }
    public abstract void markSiblingConfigurationConflict(bool conflict);
}

internal class _IncompleteSemanticsFragment__object : _SemanticsFragment__object
{
    private global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration? __field_configToMergeUp = default!;
    public override global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration? configToMergeUp { get => __field_configToMergeUp; }
    private _RenderObjectSemantics__object __field_owner = default!;
    public override _RenderObjectSemantics__object owner { get => __field_owner; }

    internal _IncompleteSemanticsFragment__object(global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration configToMergeUp, _RenderObjectSemantics__object owner)
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
    public virtual global::Doroti.Generated.Framework.Semantics.SemanticsNode? cachedSemanticsNode { get; set; } = default;
    public virtual List<global::Doroti.Generated.Framework.Semantics.SemanticsNode> semanticsNodes { get; private set; } = new List<global::Doroti.Generated.Framework.Semantics.SemanticsNode>();
    public virtual List<_SemanticsFragment__object> mergeUp { get; private set; } = new List<_SemanticsFragment__object>();
    internal virtual List<_RenderObjectSemantics__object> _children { get; private set; } = new List<_RenderObjectSemantics__object>();
    public virtual List<List<_SemanticsFragment__object>> siblingMergeGroups { get; private set; } = new List<List<_SemanticsFragment__object>>();
    internal virtual DartMap<global::Doroti.Generated.Framework.Semantics.SemanticsNode, List<_SemanticsFragment__object>> _producedSiblingNodesAndOwners { get; private set; } = new DartMap<global::Doroti.Generated.Framework.Semantics.SemanticsNode, List<_SemanticsFragment__object>>();
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
        _RenderObjectSemantics__object? next__217998 = default!;
        if (this.shouldFormSemanticsNode)
        {
            if (!this.geometryDirty)
            {
                firstAncestorNodeWithCleanGeometry = this;
            }
            next__217998 = this.parentInSemanticsTree;
        }
        else
        {
            next__217998 = this;
            while ((!next__217998!.parentDataDirty && !((_RenderObjectSemantics__object)next__217998).shouldFormSemanticsNode))
            {
                next__217998 = ((_RenderObjectSemantics__object)next__217998).parent;
                DartRuntimePrimitives.Assert(() => (next__217998 is not null));
            }
        }
        if ((next__217998 is null))
        {
            return;
        }
        if ((this.firstAncestorNodeWithCleanGeometry is null))
        {
            next__217998.computeAncestorInfo(treeShapeToken);
            firstAncestorNodeWithCleanGeometry = ((_RenderObjectSemantics__object)next__217998).firstAncestorNodeWithCleanGeometry;
        }
    }

    public override global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration? configToMergeUp => (this.shouldFormSemanticsNode ? null : ((_SemanticsConfigurationProvider__object)this.configProvider).effective);
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
                _RenderObjectSemantics__object childSemantics__221191 = ((RenderObject)child)._semantics;
                if (((_RenderObjectSemantics__object)childSemantics__221191).isBlockingPreviousSibling)
                {
                    _blocksPreviousSibling = true;
                }
            })));
            return DartRuntimePrimitives.RequireValue(this._blocksPreviousSibling);
            return default!;
        }
    }
    public static bool shouldDrop(global::Doroti.Generated.Framework.Semantics.SemanticsNode node) => ((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).isInvisible;
    public virtual void markNeedsBuild()
    {
        built = false;
        if ((!this.parentDataDirty && !this.shouldFormSemanticsNode))
        {
            return;
        }
        foreach (List<_SemanticsFragment__object> group__221604 in this.siblingMergeGroups)
        {
            foreach (_RenderObjectSemantics__object semantics__221675 in group__221604.OfType<_RenderObjectSemantics__object>())
            {
                if (((_RenderObjectSemantics__object)semantics__221675).parentDataDirty)
                {
                    continue;
                }
                if (!((_RenderObjectSemantics__object)semantics__221675).shouldFormSemanticsNode)
                {
                    semantics__221675.markNeedsBuild();
                }
            }
        }
    }

    public virtual void updateChildren()
    {
        DartRuntimePrimitives.Assert(() => ((this.parentData is not null) || this.isRoot));
        this.configProvider.reset();
        HashSet<global::Doroti.Generated.Framework.Semantics.SemanticsTag>? tagsForChildren__223131 = _getTagsForChildren();
        bool explicitChildNodesForChildren__223187 = ((this.isRoot || ((_SemanticsConfigurationProvider__object)this.configProvider).effective.explicitChildNodes) || ((!this.contributesToSemanticsTree && ((this.parentData?.explicitChildNodes ?? true)))));
        bool blocksUserAction__223532 = (((this.parentData?.blocksUserActions ?? false)) || ((_SemanticsConfigurationProvider__object)this.configProvider).effective.isBlockingUserActions);
        global::Doroti.Generated.Framework.Semantics.AccessibilityFocusBlockType accessibilityFocusBlockType__223684 = default!;
        if ((object.Equals(this.parentData?.accessibilityFocusBlockType, global::Doroti.Generated.Framework.Semantics.AccessibilityFocusBlockType.blockSubtree)))
        {
            accessibilityFocusBlockType__223684 = global::Doroti.Generated.Framework.Semantics.AccessibilityFocusBlockType.blockSubtree;
        }
        else
        {
            accessibilityFocusBlockType__223684 = ((_SemanticsConfigurationProvider__object)this.configProvider).effective.accessibilityFocusBlockType;
        }
        global::Doroti.Flutter.Ui.Locale? localeForChildren__224095 = (((_SemanticsConfigurationProvider__object)this.configProvider).effective.localeForSubtree ?? this.parentData?.localeForChildren);
        this.siblingMergeGroups.Clear();
        this.mergeUp.Clear();
        var childParentData__224263 = new _SemanticsParentData__object(mergeIntoParent: (((this.parentData?.mergeIntoParent ?? false)) || ((_SemanticsConfigurationProvider__object)this.configProvider).effective.isMergingSemanticsOfDescendants), blocksUserActions: blocksUserAction__223532, accessibilityFocusBlockType: accessibilityFocusBlockType__223684, localeForChildren: localeForChildren__224095, explicitChildNodes: explicitChildNodesForChildren__223187, tagsForChildren: tagsForChildren__223131);
        (List<_SemanticsFragment__object>, List<List<_SemanticsFragment__object>>) result__224742 = _collectChildMergeUpAndSiblingGroup(childParentData__224263);
        this.mergeUp.AddRange(result__224742.Item1);
        this.siblingMergeGroups.AddRange(result__224742.Item2);
        HashSet<_RenderObjectSemantics__object> oldChildren__224994 = this._children.toSet();
        this._children.Clear();
        if (!this.contributesToSemanticsTree)
        {
            return;
        }
        _marksConflictsInMergeGroup(this.mergeUp, isMergeUp: true);
        this.siblingMergeGroups.forEach(__fragments => this._marksConflictsInMergeGroup(__fragments));
        IEnumerable<global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration> mergeUpConfigs__225273 = this.mergeUp.map<_SemanticsFragment__object, global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration?>(((fragment) => ((_SemanticsFragment__object)fragment).configToMergeUp)).OfType<global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration>();
        this.configProvider.absorbAll(mergeUpConfigs__225273);
        this.mergeUp.Clear();
        this.mergeUp.Add(this);
        foreach (_RenderObjectSemantics__object childSemantics__225714 in result__224742.Item1.OfType<_RenderObjectSemantics__object>())
        {
            DartRuntimePrimitives.Assert(() => ((_RenderObjectSemantics__object)childSemantics__225714).contributesToSemanticsTree);
            if (((_RenderObjectSemantics__object)childSemantics__225714).shouldFormSemanticsNode)
            {
                foreach (_RenderObjectSemantics__object child__225940 in ((_RenderObjectSemantics__object)childSemantics__225714)._children)
                {
                    child__225940.parentInSemanticsTree = childSemantics__225714;
                }
                if (((_RenderObjectSemantics__object)childSemantics__225714).geometryDirty)
                {
                    ((RenderObject)this.renderObject).owner!._nodesNeedingSemanticsGeometryUpdate.Add(((_RenderObjectSemantics__object)childSemantics__225714).renderObject);
                }
                this._children.Add(childSemantics__225714);
            }
            else
            {
                this._children.AddRange(((_RenderObjectSemantics__object)childSemantics__225714)._children);
                this.siblingMergeGroups.AddRange(((_RenderObjectSemantics__object)childSemantics__225714).siblingMergeGroups);
            }
        }
        if ((this.isRoot || ((_SemanticsConfigurationProvider__object)this.configProvider).effective.isSemanticBoundary))
        {
            foreach (_RenderObjectSemantics__object child__227677 in this._children)
            {
                child__227677.parentInSemanticsTree = this;
            }
        }
        oldChildren__224994.removeAll(this._children);
        foreach (var removedChild__227810 in oldChildren__224994)
        {
            if ((object.Equals(((_RenderObjectSemantics__object)removedChild__227810).parentInSemanticsTree, this)))
            {
                removedChild__227810.parentInSemanticsTree = null;
            }
        }
        HashSet<global::Doroti.Generated.Framework.Semantics.SemanticsTag>? tags__227992 = this.parentData?.tagsForChildren;
        if ((tags__227992 is not null))
        {
            DartRuntimePrimitives.Assert(() => (checked((long)(tags__227992.Count)) != 0));
            this.configProvider.updateConfig(((Action<global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration>)((config) =>
            {
                tags__227992.forEach(((global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration)config).addTagForChildren);
            })));
        }
        if ((!object.Equals(accessibilityFocusBlockType__223684, ((_SemanticsConfigurationProvider__object)this.configProvider).effective.accessibilityFocusBlockType)))
        {
            this.configProvider.updateConfig(((Action<global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration>)((config) =>
            {
                config.accessibilityFocusBlockType = accessibilityFocusBlockType__223684;
            })));
        }
        if ((blocksUserAction__223532 != ((_SemanticsConfigurationProvider__object)this.configProvider).effective.isBlockingUserActions))
        {
            this.configProvider.updateConfig(((Action<global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration>)((config) =>
            {
                config.isBlockingUserActions = blocksUserAction__223532;
            })));
        }
        if ((!object.Equals(localeForChildren__224095, ((_SemanticsConfigurationProvider__object)this.configProvider).effective.locale)))
        {
            this.configProvider.updateConfig(((Action<global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration>)((config) =>
            {
                config.locale = localeForChildren__224095;
            })));
        }
        if ((!object.Equals(accessibilityFocusBlockType__223684, global::Doroti.Generated.Framework.Semantics.AccessibilityFocusBlockType.none)))
        {
            this.configProvider.updateConfig(((Action<global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration>)((config) =>
            {
                config.isFocused = null;
            })));
        }
    }

    internal virtual List<_RenderObjectSemantics__object> _getNonBlockedChildren()
    {
        var result__229144 = new List<_RenderObjectSemantics__object>();
        this.renderObject.visitChildrenForSemantics(((Action<RenderObject>)((renderChild) =>
        {
            if (((RenderObject)renderChild)._semantics.isBlockingPreviousSibling)
            {
                result__229144.Clear();
            }
            result__229144.Add(((RenderObject)renderChild)._semantics);
        })));
        return result__229144;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual HashSet<global::Doroti.Generated.Framework.Semantics.SemanticsTag>? _getTagsForChildren()
    {
        if (this.contributesToSemanticsTree)
        {
            return ((_SemanticsConfigurationProvider__object)this.configProvider).original.tagsForChildren?.toSet();
        }
        HashSet<global::Doroti.Generated.Framework.Semantics.SemanticsTag>? result__229596 = default!;
        if ((((_SemanticsConfigurationProvider__object)this.configProvider).original.tagsForChildren is not null))
        {
            result__229596 = ((_SemanticsConfigurationProvider__object)this.configProvider).original.tagsForChildren!.toSet();
        }
        if ((this.parentData?.tagsForChildren is not null))
        {
            if ((result__229596 is null))
            {
                result__229596 = this.parentData!.tagsForChildren;
            }
            else
            {
                result__229596.UnionWith(this.parentData!.tagsForChildren!);
            }
        }
        return result__229596;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual (List<_SemanticsFragment__object>, List<List<_SemanticsFragment__object>>) _collectChildMergeUpAndSiblingGroup(_SemanticsParentData__object childParentData)
    {
        var mergeUp__230088 = new List<_SemanticsFragment__object>();
        var siblingMergeGroups__230132 = new List<List<_SemanticsFragment__object>>();
        var childConfigurations__230194 = new List<global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration>();
        Func<List<global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration>, global::Doroti.Generated.Framework.Semantics.ChildSemanticsConfigurationsResult>? childConfigurationsDelegate__230292 = ((_SemanticsConfigurationProvider__object)this.configProvider).effective.childConfigurationsDelegate;
        var hasChildConfigurationsDelegate__230394 = (childConfigurationsDelegate__230292 is not null);
        var configToFragment__230474 = new DartMap<global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration, _SemanticsFragment__object>();
        bool needsToMakeIncompleteFragmentAssumption__231345 = (hasChildConfigurationsDelegate__230394 && ((_SemanticsParentData__object)childParentData).explicitChildNodes);
        _SemanticsParentData__object effectiveChildParentData__231497 = default!;
        if (needsToMakeIncompleteFragmentAssumption__231345)
        {
            effectiveChildParentData__231497 = new _SemanticsParentData__object(mergeIntoParent: ((_SemanticsParentData__object)childParentData).mergeIntoParent, blocksUserActions: ((_SemanticsParentData__object)childParentData).blocksUserActions, accessibilityFocusBlockType: ((_SemanticsParentData__object)childParentData).accessibilityFocusBlockType, explicitChildNodes: false, tagsForChildren: ((_SemanticsParentData__object)childParentData).tagsForChildren, localeForChildren: ((_SemanticsParentData__object)childParentData).localeForChildren);
        }
        else
        {
            effectiveChildParentData__231497 = childParentData;
        }
        foreach (_RenderObjectSemantics__object childSemantics__232102 in _getNonBlockedChildren())
        {
            DartRuntimePrimitives.Assert(() => !((_RenderObjectSemantics__object)childSemantics__232102).renderObject._needsLayout);
            childSemantics__232102._didUpdateParentData(effectiveChildParentData__231497);
            foreach (_SemanticsFragment__object fragment__232310 in ((_RenderObjectSemantics__object)childSemantics__232102).mergeUp)
            {
                if ((hasChildConfigurationsDelegate__230394 && (((_SemanticsFragment__object)fragment__232310).configToMergeUp is not null)))
                {
                    childConfigurations__230194.Add(((_SemanticsFragment__object)fragment__232310).configToMergeUp!);
                    configToFragment__230474[((_SemanticsFragment__object)fragment__232310).configToMergeUp!] = fragment__232310;
                }
                else
                {
                    mergeUp__230088.Add(fragment__232310);
                }
            }
            if (!((_RenderObjectSemantics__object)childSemantics__232102).contributesToSemanticsTree)
            {
                siblingMergeGroups__230132.AddRange(((_RenderObjectSemantics__object)childSemantics__232102).siblingMergeGroups);
            }
        }
        _containsIncompleteFragment = false;
        DartRuntimePrimitives.Assert(() => ((childConfigurationsDelegate__230292 is not null) || (checked((long)(configToFragment__230474.Count)) == 0)));
        if (hasChildConfigurationsDelegate__230394)
        {
            global::Doroti.Generated.Framework.Semantics.ChildSemanticsConfigurationsResult result__233226 = childConfigurationsDelegate__230292(childConfigurations__230194);
            mergeUp__230088.AddRange(((global::Doroti.Generated.Framework.Semantics.ChildSemanticsConfigurationsResult)result__233226).mergeUp.map<global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration, _SemanticsFragment__object>(((config) =>
            {
                _SemanticsFragment__object? fragment__233441 = configToFragment__230474.GetValueOrDefault(config);
                if ((fragment__233441 is not null))
                {
                    return fragment__233441;
                }
                _containsIncompleteFragment = true;
                return new _IncompleteSemanticsFragment__object(config, this);
                return default;
            })));
            foreach (IEnumerable<global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration> group__233731 in ((global::Doroti.Generated.Framework.Semantics.ChildSemanticsConfigurationsResult)result__233226).siblingMergeGroups)
            {
                siblingMergeGroups__230132.Add(group__233731.map<global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration, _SemanticsFragment__object>(((config) =>
                {
                    _SemanticsFragment__object? fragment__233913 = configToFragment__230474.GetValueOrDefault(config);
                    if ((fragment__233913 is not null))
                    {
                        return fragment__233913;
                    }
                    _containsIncompleteFragment = true;
                    return new _IncompleteSemanticsFragment__object(config, this);
                    return default;
                })).ToList());
            }
        }
        if ((!this._containsIncompleteFragment && needsToMakeIncompleteFragmentAssumption__231345))
        {
            mergeUp__230088.Clear();
            siblingMergeGroups__230132.Clear();
            foreach (_RenderObjectSemantics__object childSemantics__234435 in _getNonBlockedChildren())
            {
                DartRuntimePrimitives.Assert(() => ((_SemanticsParentData__object)childParentData).explicitChildNodes);
                childSemantics__234435._didUpdateParentData(childParentData);
                mergeUp__230088.AddRange(((_RenderObjectSemantics__object)childSemantics__234435).mergeUp);
                if (!((_RenderObjectSemantics__object)childSemantics__234435).contributesToSemanticsTree)
                {
                    siblingMergeGroups__230132.AddRange(((_RenderObjectSemantics__object)childSemantics__234435).siblingMergeGroups);
                }
            }
        }
        return (mergeUp__230088, siblingMergeGroups__230132);
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
        _SemanticsGeometry__object parentGeometry__236314 = this.geometry!;
        foreach (_RenderObjectSemantics__object child__236380 in this._children)
        {
            if (child__236380.renderObject is RenderBox childBox__236380 && !childBox__236380.hasSize)
            {
                continue;
            }
            if ((onlyDirtyChildren && !((_RenderObjectSemantics__object)child__236380).geometryDirty))
            {
                continue;
            }
            _SemanticsGeometry__object childGeometry__236514 = _SemanticsGeometry__object.computeChildGeometry(parentPaintClipRect: ((_SemanticsGeometry__object)parentGeometry__236314).paintClipRect, parentSemanticsClipRect: ((_SemanticsGeometry__object)parentGeometry__236314).semanticsClipRect, parentTransform: null, parent: this, child: child__236380);
            child__236380._updateGeometry(newGeometry: childGeometry__236514);
        }
        foreach (_RenderObjectSemantics__object explicitSiblingChild__236882 in this.siblingMergeGroups.expand(((group) => group)).OfType<_RenderObjectSemantics__object>().expand(((siblingChild) => (((_RenderObjectSemantics__object)siblingChild).shouldFormSemanticsNode ? new List<_RenderObjectSemantics__object> { siblingChild } : ((_RenderObjectSemantics__object)siblingChild)._children))))
        {
            if (explicitSiblingChild__236882.renderObject is RenderBox siblingBox__236882 && !siblingBox__236882.hasSize)
            {
                continue;
            }
            if ((onlyDirtyChildren && !((_RenderObjectSemantics__object)explicitSiblingChild__236882).geometryDirty))
            {
                continue;
            }
            _SemanticsGeometry__object childGeometry__237425 = _SemanticsGeometry__object.computeChildGeometry(parentPaintClipRect: ((_SemanticsGeometry__object)parentGeometry__236314).paintClipRect, parentSemanticsClipRect: ((_SemanticsGeometry__object)parentGeometry__236314).semanticsClipRect, parentTransform: ((_SemanticsGeometry__object)parentGeometry__236314).transform, parent: this, child: explicitSiblingChild__236882);
            explicitSiblingChild__236882._updateGeometry(newGeometry: childGeometry__237425);
        }
    }

    internal virtual void _updateGeometry(_SemanticsGeometry__object newGeometry)
    {
        _SemanticsGeometry__object? currentGeometry__237908 = this.geometry;
        geometry = newGeometry;
        if ((currentGeometry__237908 is not null))
        {
            bool isSemanticsHidden__238016 = (((_SemanticsConfigurationProvider__object)this.configProvider).original.isHidden || ((!((this.parentData?.mergeIntoParent ?? false)) && ((_SemanticsGeometry__object)newGeometry).hidden)));
            var sizeChanged__238169 = (!object.Equals(((_SemanticsGeometry__object)currentGeometry__237908).rect.size, ((_SemanticsGeometry__object)newGeometry).rect.size));
            var visibilityChanged__238247 = (((_SemanticsConfigurationProvider__object)this.configProvider).effective.isHidden != isSemanticsHidden__238016);
            if ((!sizeChanged__238169 && !visibilityChanged__238247))
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
            foreach (global::Doroti.Generated.Framework.Semantics.SemanticsNode node__240151 in this.semanticsNodes)
            {
                if ((!object.Equals(node__240151, this.cachedSemanticsNode)))
                {
                    node__240151.tags = null;
                }
            }
        }
        if (!this.built)
        {
            _produceSemanticsNode(usedSemanticsIds: usedSemanticsIds);
        }
        DartRuntimePrimitives.Assert(() => this.built);
        global::Doroti.Generated.Framework.Semantics.SemanticsNode producedNode__240560 = this.cachedSemanticsNode!;
        foreach (global::Doroti.Generated.Framework.Semantics.SemanticsNode node__240626 in this.semanticsNodes)
        {
            if ((!object.Equals(node__240626, producedNode__240560)))
            {
                if ((this.parentData?.tagsForChildren is not null))
                {
                    node__240626.tags ??= new HashSet<global::Doroti.Generated.Framework.Semantics.SemanticsTag>();
                    ((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node__240626).tags!.UnionWith(this.parentData!.tagsForChildren!);
                }
                else
                {
                    if (((((long?)(((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node__240626).tags?.Count)) is { } __count240857 ? __count240857 == 0 : (bool?)null) ?? false))
                    {
                        node__240626.tags = null;
                    }
                }
            }
        }
    }

    internal virtual void _buildSemanticsSubtree(HashSet<long> usedSemanticsIds)
    {
        var children__241093 = new List<global::Doroti.Generated.Framework.Semantics.SemanticsNode>();
        foreach (_RenderObjectSemantics__object child__241161 in this._children)
        {
            if (child__241161.geometry is null)
            {
                continue;
            }
            if (((_RenderObjectSemantics__object)child__241161).parentDataDirty)
            {
                continue;
            }
            DartRuntimePrimitives.Assert(() => ((_RenderObjectSemantics__object)child__241161).shouldFormSemanticsNode);
            if (((((_RenderObjectSemantics__object)child__241161).cachedSemanticsNode is not null) && usedSemanticsIds.Contains(((_RenderObjectSemantics__object)child__241161).cachedSemanticsNode!.id)))
            {
                child__241161.markNeedsBuild();
                child__241161.cachedSemanticsNode = null;
            }
            child__241161._buildSemantics(usedSemanticsIds: usedSemanticsIds);
            children__241093.AddRange(((_RenderObjectSemantics__object)child__241161).semanticsNodes);
        }
        global::Doroti.Generated.Framework.Semantics.SemanticsNode node__241828 = this.cachedSemanticsNode!;
        children__241093.removeWhere(shouldDrop);
        bool isSemanticsHidden__241910 = (((_SemanticsConfigurationProvider__object)this.configProvider).original.isHidden || ((!((this.parentData?.mergeIntoParent ?? false)) && this.geometry!.hidden)));
        if ((((_SemanticsConfigurationProvider__object)this.configProvider).effective.isHidden != isSemanticsHidden__241910))
        {
            this.configProvider.updateConfig(((Action<global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration>)((config) =>
            {
                config.isHidden = isSemanticsHidden__241910;
            })));
        }
        if (((_SemanticsConfigurationProvider__object)this.configProvider).effective.isSemanticBoundary)
        {
            if (this._needsMergingSiblingNodesIntoSelf)
            {
                var innerNode__242640 = new global::Doroti.Generated.Framework.Semantics.SemanticsNode(showOnScreen: () => this.renderObject.showOnScreen());
                this.renderObject.assembleSemanticsNode(innerNode__242640, ((_SemanticsConfigurationProvider__object)this.configProvider).effective, children__241093);
                var config__242814 = ((Func<global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration>)(() =>
{
    var __cascade = new global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration();
    __cascade.isSemanticBoundary = true;
    __cascade.isMergingSemanticsOfDescendants = true;
    return __cascade;
}))();
                node__241828.updateWith(config: config__242814, childrenInInversePaintOrder: new List<global::Doroti.Generated.Framework.Semantics.SemanticsNode> { innerNode__242640 });
            }
            else
            {
                this.renderObject.assembleSemanticsNode(node__241828, ((_SemanticsConfigurationProvider__object)this.configProvider).effective, children__241093);
            }
        }
        else
        {
            DartRuntimePrimitives.Assert(() => !((_SemanticsConfigurationProvider__object)this.configProvider).effective.isMergingSemanticsOfDescendants);
            node__241828.updateWith(config: ((_SemanticsConfigurationProvider__object)this.configProvider).effective, childrenInInversePaintOrder: children__241093);
        }
    }

    internal virtual void _produceSemanticsNode(HashSet<long> usedSemanticsIds)
    {
        DartRuntimePrimitives.Assert(() => !this.built);
        this.semanticsNodes.Clear();
        this._producedSiblingNodesAndOwners.Clear();
        global::Doroti.Generated.Framework.Semantics.SemanticsNode node__243633 = cachedSemanticsNode ??= _createSemanticsNode();
        ((Func<global::Doroti.Generated.Framework.Semantics.SemanticsNode>)(() =>
{
    var __cascade = node__243633;
    __cascade.isMergedIntoParent = ((this.parentData?.mergeIntoParent ?? false));
    __cascade.tags = this.parentData?.tagsForChildren;
    return __cascade;
}))();
        _updateSemanticsNodeGeometry();
        _mergeSiblingGroup(usedSemanticsIds);
        _buildSemanticsSubtree(usedSemanticsIds: usedSemanticsIds);
        this.semanticsNodes.Add(node__243633);
        if (!this._needsMergingSiblingNodesIntoSelf)
        {
            this.semanticsNodes.AddRange(this._producedSiblingNodesAndOwners.Keys);
        }
        built = true;
    }

    internal virtual global::Doroti.Generated.Framework.Semantics.SemanticsNode _createSemanticsNode()
    {
        if (this.isRoot)
        {
            return global::Doroti.Generated.Framework.Semantics.SemanticsNode.CreateRoot(showOnScreen: () => ((_RenderObjectSemantics__object)this.owner).renderObject.showOnScreen(), owner: ((_RenderObjectSemantics__object)this.owner).renderObject.owner!.semanticsOwner!);
        }
        return new global::Doroti.Generated.Framework.Semantics.SemanticsNode(showOnScreen: () => ((_RenderObjectSemantics__object)this.owner).renderObject.showOnScreen());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _mergeSiblingGroup(HashSet<long> usedSemanticsIds)
    {
        foreach (List<_SemanticsFragment__object> group__244711 in this.siblingMergeGroups)
        {
            global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration? configuration__244772 = default!;
            global::Doroti.Generated.Framework.Semantics.SemanticsNode? node__244808 = default!;
            var explicitChildren__244826 = new List<_RenderObjectSemantics__object>();
            foreach (var fragment__244890 in group__244711)
            {
                if ((fragment__244890 is _RenderObjectSemantics__object))
                {
                    _RenderObjectSemantics__object fragment__244890__as244923 = (_RenderObjectSemantics__object)fragment__244890;
                    if (((_RenderObjectSemantics__object)((_RenderObjectSemantics__object)fragment__244890__as244923)).shouldFormSemanticsNode)
                    {
                        explicitChildren__244826.Add(((_RenderObjectSemantics__object)fragment__244890__as244923));
                        DartRuntimePrimitives.Assert(() => (((_RenderObjectSemantics__object)((_RenderObjectSemantics__object)fragment__244890__as244923)).configToMergeUp is null));
                        continue;
                    }
                    explicitChildren__244826.AddRange(((_RenderObjectSemantics__object)((_RenderObjectSemantics__object)fragment__244890__as244923))._children);
                }
                if ((((_SemanticsFragment__object)fragment__244890).configToMergeUp is not null))
                {
                    fragment__244890.mergesToSibling = true;
                    node__244808 ??= ((_SemanticsFragment__object)fragment__244890).owner.cachedSemanticsNode;
                    configuration__244772 ??= new global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration();
                    configuration__244772.absorb(((_SemanticsFragment__object)fragment__244890).configToMergeUp!);
                }
            }
            var childrenNodes__245497 = new List<global::Doroti.Generated.Framework.Semantics.SemanticsNode>();
            foreach (var explicitChild__245549 in explicitChildren__244826)
            {
                explicitChild__245549._buildSemantics(usedSemanticsIds: usedSemanticsIds);
                childrenNodes__245497.AddRange(((_RenderObjectSemantics__object)explicitChild__245549).semanticsNodes);
            }
            if ((configuration__244772 is not null))
            {
                if (((node__244808 is null) || usedSemanticsIds.Contains(((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node__244808).id)))
                {
                    node__244808 = new global::Doroti.Generated.Framework.Semantics.SemanticsNode(showOnScreen: () => this.renderObject.showOnScreen());
                }
                usedSemanticsIds.Add(((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node__244808).id);
                foreach (var fragment__246056 in group__244711)
                {
                    if ((((_SemanticsFragment__object)fragment__246056).configToMergeUp is not null))
                    {
                        ((_SemanticsFragment__object)fragment__246056).owner.built = true;
                        ((_SemanticsFragment__object)fragment__246056).owner.cachedSemanticsNode = node__244808;
                    }
                }
                node__244808.updateWith(config: configuration__244772, childrenInInversePaintOrder: childrenNodes__245497);
                this._producedSiblingNodesAndOwners[DartRuntimePrimitives.RequireReference(node__244808)] = group__244711;
                HashSet<global::Doroti.Generated.Framework.Semantics.SemanticsTag> tags__246424 = group__244711.map<_SemanticsFragment__object, HashSet<global::Doroti.Generated.Framework.Semantics.SemanticsTag>?>(((fragment) => ((_SemanticsFragment__object)fragment).owner.parentData!.tagsForChildren)).OfType<HashSet<global::Doroti.Generated.Framework.Semantics.SemanticsTag>>().expand(((tags__246424) => tags__246424)).toSet();
                if ((checked((long)(tags__246424.Count)) != 0))
                {
                    if ((((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node__244808).tags is null))
                    {
                        node__244808.tags = tags__246424;
                    }
                    else
                    {
                        ((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node__244808).tags!.UnionWith(tags__246424);
                    }
                }
                node__244808.isMergedIntoParent = (this.parentData?.mergeIntoParent ?? false);
            }
        }
        _updateSiblingNodesGeometries();
    }

    internal virtual void _updateSemanticsNodeGeometry()
    {
        global::Doroti.Generated.Framework.Semantics.SemanticsNode node__247586 = this.cachedSemanticsNode!;
        _SemanticsGeometry__object nodeGeometry__247644 = this.geometry!;
        ((Func<global::Doroti.Generated.Framework.Semantics.SemanticsNode>)(() =>
{
    var __cascade = node__247586;
    __cascade.rect = ((_SemanticsGeometry__object)nodeGeometry__247644).rect;
    __cascade.transform = ((_SemanticsGeometry__object)nodeGeometry__247644).transform;
    __cascade.parentSemanticsClipRect = ((_SemanticsGeometry__object)nodeGeometry__247644).semanticsClipRect;
    __cascade.parentPaintClipRect = ((_SemanticsGeometry__object)nodeGeometry__247644).paintClipRect;
    return __cascade;
}))();
    }

    internal virtual void _updateSiblingNodesGeometries()
    {
        _SemanticsGeometry__object mainGeometry__247953 = this.geometry!;
        foreach (MapEntry<global::Doroti.Generated.Framework.Semantics.SemanticsNode, List<_SemanticsFragment__object>> entry__248044 in this._producedSiblingNodesAndOwners.entries)
        {
            global::Doroti.Flutter.Ui.Rect? rect__248115 = default!;
            global::Doroti.Flutter.Ui.Rect? semanticsClipRect__248133 = default!;
            global::Doroti.Flutter.Ui.Rect? paintClipRect__248164 = default!;
            foreach (_SemanticsFragment__object fragment__248215 in entry__248044.value)
            {
                if (((_SemanticsFragment__object)fragment__248215).owner.shouldFormSemanticsNode)
                {
                    continue;
                }
                _SemanticsGeometry__object parentGeometry__248359 = _SemanticsGeometry__object.computeChildGeometry(parentTransform: ((_SemanticsGeometry__object)mainGeometry__247953).transform, parentSemanticsClipRect: ((_SemanticsGeometry__object)mainGeometry__247953).semanticsClipRect, parentPaintClipRect: ((_SemanticsGeometry__object)mainGeometry__247953).paintClipRect, parent: this, child: ((_SemanticsFragment__object)fragment__248215).owner);
                global::Doroti.Flutter.Ui.Rect rectInFragmentOwnerCoordinates__248681 = (((_SemanticsGeometry__object)parentGeometry__248359).semanticsClipRect?.intersect(((_SemanticsFragment__object)fragment__248215).owner.renderObject.semanticBounds) ?? ((_SemanticsFragment__object)fragment__248215).owner.renderObject.semanticBounds);
                global::Doroti.Flutter.Ui.Rect rectInParentCoordinates__248921 = MatrixUtils.transformRect(((_SemanticsGeometry__object)parentGeometry__248359).transform, rectInFragmentOwnerCoordinates__248681);
                rect__248115 = (rect__248115?.expandToInclude(rectInParentCoordinates__248921) ?? rectInParentCoordinates__248921);
                if ((((_SemanticsGeometry__object)parentGeometry__248359).semanticsClipRect is not null))
                {
                    global::Doroti.Flutter.Ui.Rect rect__249230 = MatrixUtils.transformRect(((_SemanticsGeometry__object)parentGeometry__248359).transform, DartRuntimePrimitives.RequireValue(((_SemanticsGeometry__object)parentGeometry__248359).semanticsClipRect));
                    semanticsClipRect__248133 = (semanticsClipRect__248133?.intersect(rect__249230) ?? rect__249230);
                }
                if ((((_SemanticsGeometry__object)parentGeometry__248359).paintClipRect is not null))
                {
                    global::Doroti.Flutter.Ui.Rect rect__249519 = MatrixUtils.transformRect(((_SemanticsGeometry__object)parentGeometry__248359).transform, DartRuntimePrimitives.RequireValue(((_SemanticsGeometry__object)parentGeometry__248359).paintClipRect));
                    paintClipRect__248164 = (paintClipRect__248164?.intersect(rect__249519) ?? rect__249519);
                }
            }
            global::Doroti.Generated.Framework.Semantics.SemanticsNode node__249757 = entry__248044.key;
            ((Func<global::Doroti.Generated.Framework.Semantics.SemanticsNode>)(() =>
{
    var __cascade = node__249757;
    __cascade.rect = DartRuntimePrimitives.RequireValue(rect__248115);
    __cascade.transform = null;
    __cascade.parentSemanticsClipRect = semanticsClipRect__248133;
    __cascade.parentPaintClipRect = paintClipRect__248164;
    return __cascade;
}))();
        }
    }

    public virtual void markNeedsUpdate()
    {
        ((RenderObject)this.renderObject).owner!._nodesNeedingSemanticsGeometryUpdate.Add(this.renderObject);
        global::Doroti.Generated.Framework.Semantics.SemanticsNode? producedSemanticsNode__250232 = this.cachedSemanticsNode;
        bool wasSemanticsBoundary__250597 = ((producedSemanticsNode__250232 is not null) && ((_SemanticsConfigurationProvider__object)this.configProvider).wasSemanticsBoundary);
        this.configProvider.clear();
        _containsIncompleteFragment = false;
        var mayProduceSiblingNodes__250777 = (((_SemanticsConfigurationProvider__object)this.configProvider).effective.childConfigurationsDelegate is not null);
        bool isEffectiveSemanticsBoundary__250873 = (((_SemanticsConfigurationProvider__object)this.configProvider).effective.isSemanticBoundary && wasSemanticsBoundary__250597);
        RenderObject node__250998 = this.renderObject;
        while (((((RenderObject)node__250998).parent is not null) && ((mayProduceSiblingNodes__250777 || !isEffectiveSemanticsBoundary__250873))))
        {
            if ((((!object.Equals(node__250998, this.renderObject)) && ((RenderObject)node__250998)._semantics.parentDataDirty) && !mayProduceSiblingNodes__250777))
            {
                break;
            }
            ((RenderObject)node__250998)._semantics.parentData = null;
            ((RenderObject)node__250998)._semantics._blocksPreviousSibling = null;
            if (isEffectiveSemanticsBoundary__250873)
            {
                mayProduceSiblingNodes__250777 = false;
            }
            mayProduceSiblingNodes__250777 |= (((RenderObject)node__250998)._semantics.configProvider.effective.childConfigurationsDelegate is not null);
            node__250998 = ((RenderObject)node__250998).parent!;
            isEffectiveSemanticsBoundary__250873 = (((RenderObject)node__250998)._semantics.configProvider.effective.isSemanticBoundary && ((RenderObject)node__250998)._semantics.built);
        }
        if ((((!object.Equals(node__250998, this.renderObject)) && (producedSemanticsNode__250232 is not null)) && ((RenderObject)node__250998)._semantics.parentDataDirty))
        {
            ((RenderObject)this.renderObject).owner!._nodesNeedingSemanticsUpdate.Remove(this.renderObject);
        }
        if ((!((RenderObject)node__250998)._semantics.parentDataDirty || ((RenderObject)node__250998)._semantics.isRoot))
        {
            if ((((RenderObject)this.renderObject).owner is not null))
            {
                DartRuntimePrimitives.Assert(() => (((RenderObject)node__250998)._semantics.configProvider.effective.isSemanticBoundary || (((RenderObject)node__250998).parent is null)));
                if (((RenderObject)this.renderObject).owner!._nodesNeedingSemanticsUpdate.Add(node__250998))
                {
                    ((RenderObject)this.renderObject).owner!.requestVisualUpdate();
                }
            }
        }
    }

    internal virtual void _marksConflictsInMergeGroup(List<_SemanticsFragment__object> mergeGroup, bool isMergeUp = false)
    {
        var hasSiblingConflict__253975 = new HashSet<_SemanticsFragment__object>();
        for (var i__254033 = 0L; (i__254033 < checked((long)(mergeGroup.Count))); i__254033 += 1L)
        {
            _SemanticsFragment__object fragment__254104 = mergeGroup[(int)(i__254033)];
            fragment__254104.markSiblingConfigurationConflict(false);
            if ((((_SemanticsFragment__object)fragment__254104).configToMergeUp is null))
            {
                continue;
            }
            if ((isMergeUp && !((_SemanticsConfigurationProvider__object)this.configProvider).original.isCompatibleWith(((_SemanticsFragment__object)fragment__254104).configToMergeUp)))
            {
                hasSiblingConflict__253975.Add(fragment__254104);
            }
            var siblingLength__254440 = i__254033;
            for (var j__254474 = 0L; (j__254474 < siblingLength__254440); j__254474 += 1L)
            {
                _SemanticsFragment__object siblingFragment__254543 = mergeGroup[(int)(j__254474)];
                if (!((_SemanticsFragment__object)fragment__254104).configToMergeUp!.isCompatibleWith(((_SemanticsFragment__object)siblingFragment__254543).configToMergeUp))
                {
                    hasSiblingConflict__253975.Add(fragment__254104);
                    hasSiblingConflict__253975.Add(siblingFragment__254543);
                }
            }
        }
        foreach (var fragment__254802 in hasSiblingConflict__253975)
        {
            fragment__254802.markSiblingConfigurationConflict(true);
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
        properties.add(new StringProperty("owner", global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this.renderObject)));
        properties.add(new FlagProperty("noParentData", value: this.parentDataDirty, ifTrue: "NO PARENT DATA"));
        properties.add(new FlagProperty("geometry", value: this.geometryDirty, ifTrue: "NO GEOMETRY"));
        properties.add(new FlagProperty("semanticsBlock", value: ((_SemanticsConfigurationProvider__object)this.configProvider).effective.isBlockingSemanticsOfPreviouslyPaintedNodes, ifTrue: "BLOCK PREVIOUS"));
        if ((!this.parentDataDirty && this.contributesToSemanticsTree))
        {
            string semanticsNodeStatus__256181 = default!;
            if (this.built)
            {
                semanticsNodeStatus__256181 = $"formed {this.cachedSemanticsNode?.id}";
            }
            else
            {
                if (this.shouldFormSemanticsNode)
                {
                    semanticsNodeStatus__256181 = "needs build";
                }
                else
                {
                    semanticsNodeStatus__256181 = "no semantics node";
                }
            }
            properties.add(new StringProperty("formedSemanticsNode", semanticsNodeStatus__256181, quoted: false));
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
            global::Doroti.Generated.Framework.Foundation.PrintLibrary.debugPrint("No render tree root was added to the binding.");
            return;
        }
        global::Doroti.Generated.Framework.Foundation.PrintLibrary.debugPrint(string.Join("\n\n", new List<string>()));
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
        RenderObject childRenderObject__259121 = ((_RenderObjectSemantics__object)child).renderObject;
        RenderObject parentRenderObject__259184 = ((_RenderObjectSemantics__object)parent).renderObject;
        var childToCommonAncestor__259237 = new List<RenderObject> { childRenderObject__259121 };
        while ((((RenderObject)childRenderObject__259121).depth > ((RenderObject)parentRenderObject__259184).depth))
        {
            DartRuntimePrimitives.Assert(() => (((RenderObject)childRenderObject__259121).parent is not null));
            childRenderObject__259121 = ((RenderObject)childRenderObject__259121).parent!;
            childToCommonAncestor__259237.Add(childRenderObject__259121);
        }
        DartRuntimePrimitives.Assert(() => (checked((long)(childToCommonAncestor__259237.Count)) >= 2L));
        DartRuntimePrimitives.Assert(() => DartRuntimePrimitives.Identical(childRenderObject__259121, parentRenderObject__259184));
        global::Doroti.Flutter.Ui.Rect? paintClipRect__259962 = default!;
        global::Doroti.Flutter.Ui.Rect? semanticsClipRect__259987 = default!;
        var transform__260016 = Matrix4.identity();
        for (long i__260121 = (checked((long)(childToCommonAncestor__259237.Count)) - 1L); (i__260121 > 0L); i__260121 -= 1L)
        {
            RenderObject nodeParent__260201 = childToCommonAncestor__259237[(int)(i__260121)];
            RenderObject node__260265 = childToCommonAncestor__259237[(int)((i__260121 - 1L))];
            global::Doroti.Flutter.Ui.Rect? localPaintClipInParent__260321 = _transformRect(nodeParent__260201.describeApproximatePaintClip(node__260265), transform__260016, (Func<Matrix4, Rect, Rect>)global::Doroti.Generated.Framework.Painting.MatrixUtils.transformRect);
            global::Doroti.Flutter.Ui.Rect? localSemanticsClipInParent__260553 = _transformRect(nodeParent__260201.describeSemanticsClip(node__260265), transform__260016, (Func<Matrix4, Rect, Rect>)global::Doroti.Generated.Framework.Painting.MatrixUtils.transformRect);
            paintClipRect__259962 = _intersectRects(paintClipRect__259962, localPaintClipInParent__260321);
            semanticsClipRect__259987 = (localSemanticsClipInParent__260553 ?? semanticsClipRect__259987?.intersect((localPaintClipInParent__260321 ?? DartRuntimePrimitives.RequireValue(semanticsClipRect__259987))));
            nodeParent__260201.applyPaintTransform(node__260265, transform__260016);
        }
        semanticsClipRect__259987 = (semanticsClipRect__259987 ?? _intersectRects(paintClipRect__259962, parentSemanticsClipRect));
        paintClipRect__259962 = _intersectRects(paintClipRect__259962, parentPaintClipRect);
        if (((paintClipRect__259962 is not null) || (semanticsClipRect__259987 is not null)))
        {
            Matrix4 inverted__261373 = transform__260016.clone();
            var hasInverse__261415 = (inverted__261373.invert() != 0.0);
            semanticsClipRect__259987 = (hasInverse__261415 ? _transformRect(semanticsClipRect__259987, inverted__261373, (Func<Matrix4, Rect, Rect>)global::Doroti.Generated.Framework.Painting.MatrixUtils.transformRect) : null);
            paintClipRect__259962 = (hasInverse__261415 ? _transformRect(paintClipRect__259962, inverted__261373, (Func<Matrix4, Rect, Rect>)global::Doroti.Generated.Framework.Painting.MatrixUtils.transformRect) : null);
        }
        if ((parentTransform is not null))
        {
            MatrixUtils.multiplyInPlace(parentTransform, transform__260016);
        }
        global::Doroti.Flutter.Ui.Rect rect__261843 = (semanticsClipRect__259987?.intersect(((_RenderObjectSemantics__object)child).renderObject.semanticBounds) ?? ((_RenderObjectSemantics__object)child).renderObject.semanticBounds);
        var isRectHidden__261976 = false;
        if ((paintClipRect__259962 is not null))
        {
            Rect paintClipRect__259962__value262006 = DartRuntimePrimitives.RequireValue(paintClipRect__259962);
            global::Doroti.Flutter.Ui.Rect paintRect__262048 = DartRuntimePrimitives.RequireValue(paintClipRect__259962__value262006).intersect(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(rect__261843)));
            isRectHidden__261976 = (paintRect__262048.isEmpty && !DartRuntimePrimitives.RequireValue(rect__261843).isEmpty);
            if (!isRectHidden__261976)
            {
                rect__261843 = paintRect__262048;
            }
        }
        return new _SemanticsGeometry__object(transform: transform__260016, paintClipRect: paintClipRect__259962, semanticsClipRect: semanticsClipRect__259987, rect: DartRuntimePrimitives.RequireValue(rect__261843), hidden: isRectHidden__261976);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Flutter.Ui.Rect? _transformRect(Rect? rect, Matrix4 transform, Func<Matrix4, Rect, Rect> apply = default!)
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

    internal static global::Doroti.Flutter.Ui.Rect? _intersectRects(Rect? a, Rect? b)
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
