// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/layer.dart
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

public class AnnotationEntry<T>
{
    public virtual T annotation { get; private set; } = default!;
    public virtual Offset localPosition { get; private set; } = default!;

    public AnnotationEntry(T annotation, Offset localPosition)
    {
        this.annotation = annotation;
        this.localPosition = localPosition;
    }

    public override string ToString()
    {
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "AnnotationEntry"))}(annotation: {this.annotation}, localPosition: {this.localPosition})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class AnnotationResult<T>
{
    internal virtual List<AnnotationEntry<T>> _entries { get; private set; } = new List<AnnotationEntry<T>>();

    public virtual void add(AnnotationEntry<T> entry) => this._entries.Add(entry);
    public virtual IEnumerable<AnnotationEntry<T>> entries => this._entries;
    public virtual IEnumerable<T> annotations
    {
        get
        {
            return this._entries.map<AnnotationEntry<T>, T>(((entry) => ((AnnotationEntry<T>)entry).annotation));
            return default!;
        }
    }
}

public abstract class Layer : DiagnosticableTreeMixin
{
    internal virtual DartMap<long, Action> _callbacks { get; private set; } = new DartMap<long, Action>();
    internal static long _nextCallbackId = 0L;
    internal virtual long _compositionCallbackCount { get; set; } = 0L;
    internal virtual bool _debugMutationsLocked { get; set; } = false;
    internal virtual bool _debugDisposed { get; set; } = false;
    internal virtual LayerHandle<Layer> _parentHandle { get; private set; } = new LayerHandle<Layer>();
    internal virtual long _refCount { get; set; } = 0L;
    internal virtual ContainerLayer? _parent { get; set; } = default;
    internal virtual bool _needsAddToScene { get; set; } = true;
    internal virtual EngineLayer? _engineLayer { get; set; } = default;
    internal virtual object? _owner { get; set; } = default;
    internal virtual long _depth { get; set; } = 0L;
    internal virtual Layer? _nextSibling { get; set; } = default;
    internal virtual Layer? _previousSibling { get; set; } = default;
    public virtual object? debugCreator { get; set; } = default;

    protected Layer()
    {
    }

    public virtual bool subtreeHasCompositionCallbacks => (this._compositionCallbackCount > 0L);
    internal virtual void _updateSubtreeCompositionObserverCount(long delta)
    {
        DartRuntimePrimitives.Assert(() => (delta != 0L));
        _compositionCallbackCount += delta;
        DartRuntimePrimitives.Assert(() => (this._compositionCallbackCount >= 0L));
        this.parent?._updateSubtreeCompositionObserverCount(delta);
    }

    internal virtual void _fireCompositionCallbacks(bool includeChildren)
    {
        if ((checked((long)(this._callbacks.Count)) == 0))
        {
            return;
        }
        foreach (var callback__6035 in new List<Action>(DartRuntimePrimitives.ConvertEnumerable<Action>(this._callbacks.Values)))
        {
            callback__6035();
        }
    }

    public virtual bool supportsRasterization()
    {
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Rect? describeClipBounds() => null;
    public virtual Action addCompositionCallback(Action<Layer> callback)
    {
        _updateSubtreeCompositionObserverCount(1L);
        long callbackId__8342 = _nextCallbackId += 1L;
        this._callbacks[callbackId__8342] = (() =>
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    _debugMutationsLocked = true;
                    return true;
                });
            callback(this);
            DartRuntimePrimitives.Assert(() =>
                {
                    _debugMutationsLocked = false;
                    return true;
                });
        });
        return (() =>
        {
            DartRuntimePrimitives.Assert(() => (this.debugDisposed || this._callbacks.ContainsKey(callbackId__8342)));
            this._callbacks.remove(callbackId__8342);
            _updateSubtreeCompositionObserverCount(-1L);
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool debugDisposed
    {
        get
        {
            bool disposed__9034 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    disposed__9034 = this._debugDisposed;
                    return true;
                });
            return disposed__9034;
            return default!;
        }
    }
    internal virtual void _unref()
    {
        DartRuntimePrimitives.Assert(() => !this._debugMutationsLocked);
        DartRuntimePrimitives.Assert(() => (this._refCount > 0L));
        _refCount -= 1L;
        if ((this._refCount == 0L))
        {
            dispose();
        }
    }

    public virtual long debugHandleCount
    {
        get
        {
            long count__10129 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    count__10129 = this._refCount;
                    return true;
                });
            return count__10129;
            return default!;
        }
    }
    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => !this._debugMutationsLocked);
        DartRuntimePrimitives.Assert(() => !this._debugDisposed);
        DartRuntimePrimitives.Assert(() =>
            {
                DartRuntimePrimitives.Assert(() => (this._refCount == 0L));
                _debugDisposed = true;
                return true;
            });
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        this._engineLayer?.dispose();
        _engineLayer = null;
    }

    public virtual ContainerLayer? parent => this._parent;
    public virtual void markNeedsAddToScene()
    {
        DartRuntimePrimitives.Assert(() => !this._debugMutationsLocked);
        DartRuntimePrimitives.Assert(() => !this.alwaysNeedsAddToScene);
        DartRuntimePrimitives.Assert(() => !this._debugDisposed);
        if (this._needsAddToScene)
        {
            return;
        }
        _needsAddToScene = true;
    }

    public virtual void debugMarkClean()
    {
        DartRuntimePrimitives.Assert(() => !this._debugMutationsLocked);
        DartRuntimePrimitives.Assert(() =>
            {
                _needsAddToScene = false;
                return true;
            });
    }

    public virtual bool alwaysNeedsAddToScene => false;
    public virtual bool? debugSubtreeNeedsAddToScene
    {
        get
        {
            bool? result__14895 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    result__14895 = this._needsAddToScene;
                    return true;
                });
            return result__14895;
            return default!;
        }
    }
    public virtual global::Doroti.Ui.EngineLayer? engineLayer
    {
        get => this._engineLayer;
        set
        {
            var __value = value is null ? null : (EngineLayer)(object)value;
            DartRuntimePrimitives.Assert(() => !this._debugMutationsLocked);
            DartRuntimePrimitives.Assert(() => !this._debugDisposed);
            // Doroti's managed SceneBuilder deliberately returns the same engine-layer
            // handle when a scope can be updated in place. Do not dispose that handle
            // while installing it again; doing so would turn the next retained scene
            // into a reference to an already-disposed resource.
            if (ReferenceEquals(this._engineLayer, __value))
            {
                // A child may have updated its immutable retained payload while this
                // managed handle stayed stable. Its parent still needs to rebuild if
                // the child was composed outside the parent's current scene build.
                if ((!this.alwaysNeedsAddToScene && (this.parent is not null) && !this.parent!.alwaysNeedsAddToScene))
                {
                    this.parent!.markNeedsAddToScene();
                }
                return;
            }
            this._engineLayer?.dispose();
            _engineLayer = __value;
            if (!this.alwaysNeedsAddToScene)
            {
                if (((this.parent is not null) && !this.parent!.alwaysNeedsAddToScene))
                {
                    this.parent!.markNeedsAddToScene();
                }
            }
        }
    }
    public virtual void updateSubtreeNeedsAddToScene()
    {
        DartRuntimePrimitives.Assert(() => !this._debugMutationsLocked);
        _needsAddToScene = (this._needsAddToScene || this.alwaysNeedsAddToScene);
    }

    public virtual object? owner => this._owner;
    public virtual bool attached => (this._owner is not null);
    public virtual void attach(object owner)
    {
        DartRuntimePrimitives.Assert(() => (this._owner is null));
        _owner = owner;
    }

    public virtual void detach()
    {
        DartRuntimePrimitives.Assert(() => (this._owner is not null));
        _owner = null;
        DartRuntimePrimitives.Assert(() => ((this.parent is null) || (this.attached == this.parent!.attached)));
    }

    public virtual long depth => this._depth;
    public virtual void redepthChildren()
    {
    }

    public virtual Layer? nextSibling => this._nextSibling;
    public virtual Layer? previousSibling => this._previousSibling;
    public virtual void remove()
    {
        DartRuntimePrimitives.Assert(() => !this._debugMutationsLocked);
        this.parent?._removeChild(this);
    }

    public virtual bool findAnnotations<S>(AnnotationResult<S> result, Offset localPosition, bool onlyFirst)
    {
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual S? find<S>(Offset localPosition)
    {
        var result__24845 = new AnnotationResult<S>();
        findAnnotations<S>(result__24845, localPosition, onlyFirst: true);
        return ((((AnnotationResult<S>)result__24845).entries.Count() == 0) ? default(S) : ((AnnotationResult<S>)result__24845).entries.First().annotation);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual AnnotationResult<S> findAllAnnotations<S>(Offset localPosition)
    {
        var result__25988 = new AnnotationResult<S>();
        findAnnotations<S>(result__25988, localPosition, onlyFirst: false);
        return result__25988;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract void addToScene(SceneBuilder builder);
    internal virtual void _addToSceneWithRetainedRendering(SceneBuilder builder)
    {
        DartRuntimePrimitives.Assert(() => !this._debugMutationsLocked);
        // This is the same retained-layer decision as Flutter: a clean layer with
        // a completed engine handle contributes one immutable retained node. The
        // host still replays that node into a fresh native back buffer when needed,
        // but it does not re-record the unchanged subtree.
        if ((!this._needsAddToScene && (this._engineLayer is not null)))
        {
            builder.addRetained(this._engineLayer!);
            return;
        }
        addToScene(builder);
        _needsAddToScene = false;
    }

    public virtual string toStringShort() => $"{base.toStringShort()}{((this.owner is null) ? " DETACHED" : "")}";
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<object>("owner", this.owner, level: ((this.parent is not null) ? DiagnosticLevel.hidden : DiagnosticLevel.info), defaultValue: null));
        properties.add(new DiagnosticsProperty<object?>("creator", this.debugCreator, defaultValue: null, level: DiagnosticLevel.debug));
        if ((this._engineLayer is not null))
        {
            properties.add(new DiagnosticsProperty<string>("engine layer", global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this._engineLayer)));
        }
        properties.add(new DiagnosticsProperty<long>("handles", this.debugHandleCount));
    }

}

public class LayerHandle<T> where T : Layer
{
    internal virtual T? _layer { get; set; } = default;

    public LayerHandle(T? _layer = default)
    {
        this._layer = _layer;
    }

    public virtual T? layer
    {
        get => this._layer;
        set
        {
            var layer = value;
            DartRuntimePrimitives.Assert(() => (layer?.debugDisposed != true));
            if (DartRuntimePrimitives.Identical(layer, this._layer))
            {
                return;
            }
            this._layer?._unref();
            _layer = layer;
            if ((this._layer is not null))
            {
                this._layer!._refCount = ((dynamic)this._layer!._refCount) + ((dynamic)1L);
            }
        }
    }
    public override string ToString() => $"LayerHandle({((this._layer is not null) ? this._layer.ToString() : "DISPOSED")})";
}

public class PictureLayer : Layer
{
    public virtual Rect canvasBounds { get; private set; } = default!;
    internal virtual Picture? _picture { get; set; } = default;
    internal virtual bool _isComplexHint { get; set; } = false;
    internal virtual bool _willChangeHint { get; set; } = false;

    public PictureLayer(Rect canvasBounds)
    {
        this.canvasBounds = canvasBounds;
    }

    public virtual global::Doroti.Ui.Picture? picture
    {
        get => this._picture;
        set
        {
            var picture = value is null ? null : (Picture)(object)value;
            DartRuntimePrimitives.Assert(() => !_debugDisposed);
            markNeedsAddToScene();
            this._picture?.dispose();
            _picture = picture;
        }
    }
    public virtual bool isComplexHint
    {
        get => this._isComplexHint;
        set
        {
            var __value = value;
            if ((__value != this._isComplexHint))
            {
                _isComplexHint = __value;
                markNeedsAddToScene();
            }
        }
    }
    public virtual bool willChangeHint
    {
        get => this._willChangeHint;
        set
        {
            var __value = value;
            if ((__value != this._willChangeHint))
            {
                _willChangeHint = __value;
                markNeedsAddToScene();
            }
        }
    }
    public override void dispose()
    {
        picture = null;
        base.dispose();
    }

    public override void addToScene(SceneBuilder builder)
    {
        DartRuntimePrimitives.Assert(() => (this.picture is not null));
        builder.addPicture(Offset.zero, this.picture!, isComplexHint: this.isComplexHint, willChangeHint: this.willChangeHint);
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Rect>("paint bounds", this.canvasBounds));
        properties.add(new DiagnosticsProperty<string>("picture", global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this._picture)));
        properties.add(new DiagnosticsProperty<string>("raster cache hints", $"isComplex = {this.isComplexHint}, willChange = {this.willChangeHint}"));
    }

    public override bool findAnnotations<S>(AnnotationResult<S> result, Offset localPosition, bool onlyFirst)
    {
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class TextureLayer : Layer
{
    public virtual Rect rect { get; private set; } = default!;
    public virtual long textureId { get; private set; } = default!;
    public virtual bool freeze { get; private set; } = default!;
    public virtual FilterQuality filterQuality { get; private set; } = default!;

    public TextureLayer(Rect rect, long textureId, bool freeze = false, FilterQuality filterQuality = FilterQuality.low)
    {
        this.rect = rect;
        this.textureId = textureId;
        this.freeze = freeze;
        this.filterQuality = filterQuality;
    }

    public override void addToScene(SceneBuilder builder)
    {
        builder.addTexture(this.textureId, offset: this.rect.topLeft, width: this.rect.width, height: this.rect.height, freeze: this.freeze, filterQuality: this.filterQuality);
    }

    public override bool findAnnotations<S>(AnnotationResult<S> result, Offset localPosition, bool onlyFirst)
    {
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class PlatformViewLayer : Layer
{
    public virtual Rect rect { get; private set; } = default!;
    public virtual long viewId { get; private set; } = default!;

    public PlatformViewLayer(Rect rect, long viewId)
    {
        this.rect = rect;
        this.viewId = viewId;
    }

    public override bool supportsRasterization()
    {
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void addToScene(SceneBuilder builder)
    {
        builder.addPlatformView(this.viewId, offset: this.rect.topLeft, width: this.rect.width, height: this.rect.height);
    }

}

public class PerformanceOverlayLayer : Layer
{
    internal virtual Rect _overlayRect { get; set; } = default!;
    public virtual long optionsMask { get; private set; } = default!;

    public PerformanceOverlayLayer(Rect overlayRect, long optionsMask)
    {
        this.optionsMask = optionsMask;
        this._overlayRect = overlayRect;
    }

    public virtual global::Doroti.Ui.Rect overlayRect
    {
        get => this._overlayRect;
        set
        {
            var __value = value;
            if ((!object.Equals(__value, this._overlayRect)))
            {
                _overlayRect = __value;
                markNeedsAddToScene();
            }
        }
    }
    public override void addToScene(SceneBuilder builder)
    {
        builder.addPerformanceOverlay(this.optionsMask, this.overlayRect);
    }

    public override bool findAnnotations<S>(AnnotationResult<S> result, Offset localPosition, bool onlyFirst)
    {
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public delegate void CompositionCallback(Layer layer);

public class ContainerLayer : Layer
{
    internal virtual Layer? _firstChild { get; set; } = default;
    internal virtual Layer? _lastChild { get; set; } = default;

    internal override void _fireCompositionCallbacks(bool includeChildren)
    {
        base._fireCompositionCallbacks(includeChildren: includeChildren);
        if (!includeChildren)
        {
            return;
        }
        Layer? child__39819 = this.firstChild;
        while ((child__39819 is not null))
        {
            child__39819._fireCompositionCallbacks(includeChildren: includeChildren);
            child__39819 = ((Layer)child__39819).nextSibling;
        }
    }

    public virtual Layer? firstChild => this._firstChild;
    public virtual Layer? lastChild => this._lastChild;
    public virtual bool hasChildren => (this._firstChild is not null);
    public override bool supportsRasterization()
    {
        for (Layer? child__40400 = this.lastChild; (child__40400 is not null); child__40400 = ((Layer)child__40400).previousSibling)
        {
            if (!child__40400.supportsRasterization())
            {
                return false;
            }
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Scene buildScene(SceneBuilder builder)
    {
        updateSubtreeNeedsAddToScene();
        addToScene(builder);
        if (subtreeHasCompositionCallbacks)
        {
            _fireCompositionCallbacks(includeChildren: true);
        }
        _needsAddToScene = false;
        global::Doroti.Ui.Scene scene__41403 = builder.build();
        return scene__41403;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _debugUltimatePreviousSiblingOf(Layer child, Layer? equals = null)
    {
        DartRuntimePrimitives.Assert(() => (((Layer)child).attached == attached));
        while ((((Layer)child).previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(((Layer)child).previousSibling, child)));
            child = ((Layer)child).previousSibling!;
            DartRuntimePrimitives.Assert(() => (((Layer)child).attached == attached));
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _debugUltimateNextSiblingOf(Layer child, Layer? equals = null)
    {
        DartRuntimePrimitives.Assert(() => (((Layer)child).attached == attached));
        while ((((Layer)child)._nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(((Layer)child)._nextSibling, child)));
            child = ((Layer)child)._nextSibling!;
            DartRuntimePrimitives.Assert(() => (((Layer)child).attached == attached));
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        removeAllChildren();
        _callbacks.Clear();
        base.dispose();
    }

    public override void updateSubtreeNeedsAddToScene()
    {
        base.updateSubtreeNeedsAddToScene();
        Layer? child__42289 = this.firstChild;
        while ((child__42289 is not null))
        {
            child__42289.updateSubtreeNeedsAddToScene();
            _needsAddToScene = (_needsAddToScene || ((Layer)child__42289)._needsAddToScene);
            child__42289 = ((Layer)child__42289).nextSibling;
        }
    }

    public override bool findAnnotations<S>(AnnotationResult<S> result, Offset localPosition, bool onlyFirst)
    {
        for (Layer? child__42660 = this.lastChild; (child__42660 is not null); child__42660 = ((Layer)child__42660).previousSibling)
        {
            bool isAbsorbed__42744 = child__42660.findAnnotations<S>(result, localPosition, onlyFirst: onlyFirst);
            if (isAbsorbed__42744)
            {
                return true;
            }
            if ((onlyFirst && (((AnnotationResult<S>)result).entries.Count() != 0)))
            {
                return isAbsorbed__42744;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void attach(object owner)
    {
        DartRuntimePrimitives.Assert(() => !_debugMutationsLocked);
        base.attach(owner);
        Layer? child__43111 = this.firstChild;
        while ((child__43111 is not null))
        {
            child__43111.attach(owner);
            child__43111 = ((Layer)child__43111).nextSibling;
        }
    }

    public override void detach()
    {
        DartRuntimePrimitives.Assert(() => !_debugMutationsLocked);
        base.detach();
        Layer? child__43327 = this.firstChild;
        while ((child__43327 is not null))
        {
            child__43327.detach();
            child__43327 = ((Layer)child__43327).nextSibling;
        }
        _fireCompositionCallbacks(includeChildren: false);
    }

    public virtual void append(Layer child)
    {
        DartRuntimePrimitives.Assert(() => !_debugMutationsLocked);
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this.firstChild)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this.lastChild)));
        DartRuntimePrimitives.Assert(() => (((Layer)child).parent is null));
        DartRuntimePrimitives.Assert(() => !((Layer)child).attached);
        DartRuntimePrimitives.Assert(() => (((Layer)child).nextSibling is null));
        DartRuntimePrimitives.Assert(() => (((Layer)child).previousSibling is null));
        DartRuntimePrimitives.Assert(() => (((Layer)child)._parentHandle.layer is null));
        DartRuntimePrimitives.Assert(() =>
            {
                Layer node__44250 = this;
                while ((((Layer)node__44250).parent is not null))
                {
                    node__44250 = ((Layer)node__44250).parent!;
                }
                DartRuntimePrimitives.Assert(() => (!object.Equals(node__44250, child)));
                return true;
            });
        _adoptChild(child);
        child._previousSibling = this.lastChild;
        if ((this.lastChild is not null))
        {
            this.lastChild!._nextSibling = child;
        }
        _lastChild = child;
        _firstChild ??= child;
        ((Layer)child)._parentHandle.layer = child;
        DartRuntimePrimitives.Assert(() => (((Layer)child).attached == attached));
    }

    internal virtual void _adoptChild(Layer child)
    {
        DartRuntimePrimitives.Assert(() => !_debugMutationsLocked);
        if (!alwaysNeedsAddToScene)
        {
            markNeedsAddToScene();
        }
        if ((((Layer)child)._compositionCallbackCount != 0L))
        {
            _updateSubtreeCompositionObserverCount(((Layer)child)._compositionCallbackCount);
        }
        DartRuntimePrimitives.Assert(() => (((Layer)child)._parent is null));
        DartRuntimePrimitives.Assert(() =>
            {
                Layer node__45046 = this;
                while ((((Layer)node__45046).parent is not null))
                {
                    node__45046 = ((Layer)node__45046).parent!;
                }
                DartRuntimePrimitives.Assert(() => (!object.Equals(node__45046, child)));
                return true;
            });
        child._parent = this;
        if (attached)
        {
            child.attach(_owner!);
        }
        redepthChild(child);
    }

    public override void redepthChildren()
    {
        Layer? child__45395 = this.firstChild;
        while ((child__45395 is not null))
        {
            redepthChild(child__45395);
            child__45395 = ((Layer)child__45395).nextSibling;
        }
    }

    public virtual void redepthChild(Layer child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((Layer)child).owner, owner)));
        if ((((Layer)child)._depth <= _depth))
        {
            child._depth = (_depth + 1L);
            child.redepthChildren();
        }
    }

    internal virtual void _removeChild(Layer child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((Layer)child).parent, this)));
        DartRuntimePrimitives.Assert(() => (((Layer)child).attached == attached));
        DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(child, equals: this.firstChild));
        DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(child, equals: this.lastChild));
        DartRuntimePrimitives.Assert(() => (((Layer)child)._parentHandle.layer is not null));
        if ((((Layer)child)._previousSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._firstChild, child)));
            _firstChild = ((Layer)child)._nextSibling;
        }
        else
        {
            ((Layer)child)._previousSibling!._nextSibling = ((Layer)child).nextSibling;
        }
        if ((((Layer)child)._nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this.lastChild, child)));
            _lastChild = ((Layer)child).previousSibling;
        }
        else
        {
            ((Layer)child).nextSibling!._previousSibling = ((Layer)child).previousSibling;
        }
        DartRuntimePrimitives.Assert(() => (((this.firstChild is null)) == ((this.lastChild is null))));
        DartRuntimePrimitives.Assert(() => ((this.firstChild is null) || (this.firstChild!.attached == attached)));
        DartRuntimePrimitives.Assert(() => ((this.lastChild is null) || (this.lastChild!.attached == attached)));
        DartRuntimePrimitives.Assert(() => ((this.firstChild is null) || _debugUltimateNextSiblingOf(this.firstChild!, equals: this.lastChild)));
        DartRuntimePrimitives.Assert(() => ((this.lastChild is null) || _debugUltimatePreviousSiblingOf(this.lastChild!, equals: this.firstChild)));
        child._previousSibling = null;
        child._nextSibling = null;
        _dropChild(child);
        ((Layer)child)._parentHandle.layer = null;
        DartRuntimePrimitives.Assert(() => !((Layer)child).attached);
    }

    internal virtual void _dropChild(Layer child)
    {
        DartRuntimePrimitives.Assert(() => !_debugMutationsLocked);
        if (!alwaysNeedsAddToScene)
        {
            markNeedsAddToScene();
        }
        if ((((Layer)child)._compositionCallbackCount != 0L))
        {
            _updateSubtreeCompositionObserverCount(-((Layer)child)._compositionCallbackCount);
        }
        DartRuntimePrimitives.Assert(() => (object.Equals(((Layer)child)._parent, this)));
        DartRuntimePrimitives.Assert(() => (((Layer)child).attached == attached));
        child._parent = null;
        if (attached)
        {
            child.detach();
        }
    }

    public virtual void removeAllChildren()
    {
        DartRuntimePrimitives.Assert(() => !_debugMutationsLocked);
        Layer? child__47718 = this.firstChild;
        while ((child__47718 is not null))
        {
            Layer? next__47785 = ((Layer)child__47718).nextSibling;
            child__47718._previousSibling = null;
            child__47718._nextSibling = null;
            DartRuntimePrimitives.Assert(() => (((Layer)child__47718).attached == attached));
            _dropChild(child__47718);
            ((Layer)child__47718)._parentHandle.layer = null;
            child__47718 = next__47785;
        }
        _firstChild = null;
        _lastChild = null;
    }

    public override void addToScene(SceneBuilder builder)
    {
        addChildrenToScene(builder);
    }

    public virtual void addChildrenToScene(SceneBuilder builder)
    {
        Layer? child__48661 = this.firstChild;
        while ((child__48661 is not null))
        {
            child__48661._addToSceneWithRetainedRendering(builder);
            child__48661 = ((Layer)child__48661).nextSibling;
        }
    }

    public virtual void applyTransform(Layer? child, Matrix4 transform)
    {
        DartRuntimePrimitives.Assert(() => (child is not null));
    }

    public virtual List<Layer> depthFirstIterateChildren()
    {
        if ((this.firstChild is null))
        {
            return new List<Layer>();
        }
        var children__50760 = new List<Layer>();
        Layer? child__50793 = this.firstChild;
        while ((child__50793 is not null))
        {
            children__50760.Add(child__50793);
            if ((child__50793 is ContainerLayer))
            {
                ContainerLayer child__50793__as50878 = (ContainerLayer)child__50793;
                children__50760.AddRange(((ContainerLayer)child__50793__as50878).depthFirstIterateChildren());
            }
            child__50793 = ((Layer)child__50793).nextSibling;
        }
        return children__50760;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<DiagnosticsNode> debugDescribeChildren()
    {
        var children__51110 = new List<DiagnosticsNode>();
        if ((this.firstChild is null))
        {
            return children__51110;
        }
        Layer? child__51212 = this.firstChild;
        var count__51240 = 1L;
        while (true)
        {
            children__51110.Add(((Diagnosticable)child__51212!).toDiagnosticsNode(name: $"child {count__51240}"));
            if ((object.Equals(child__51212, this.lastChild)))
            {
                break;
            }
            count__51240 += 1L;
            child__51212 = ((Layer)child__51212).nextSibling;
        }
        return children__51110;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class OffsetLayer : ContainerLayer
{
    internal virtual Offset _offset { get; set; } = default!;

    public OffsetLayer(Offset offset = default)
    {
        this._offset = offset;
    }

    public virtual global::Doroti.Ui.Offset offset
    {
        get => this._offset;
        set
        {
            var __value = value;
            if ((!object.Equals(__value, this._offset)))
            {
                markNeedsAddToScene();
            }
            _offset = __value;
        }
    }
    public override bool findAnnotations<S>(AnnotationResult<S> result, Offset localPosition, bool onlyFirst)
    {
        return base.findAnnotations<S>(result, (localPosition - this.offset), onlyFirst: onlyFirst);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void applyTransform(Layer? child, Matrix4 transform)
    {
        DartRuntimePrimitives.Assert(() => (child is not null));
        transform.translateByDouble(this.offset.dx, this.offset.dy, 0, 1);
    }

    public override void addToScene(SceneBuilder builder)
    {
        engineLayer = builder.pushOffset(this.offset.dx, this.offset.dy, oldLayer: ((global::Doroti.Ui.OffsetEngineLayer?)(object?)_engineLayer)!);
        addChildrenToScene(builder);
        builder.pop();
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("offset", this.offset));
    }

    internal virtual global::Doroti.Ui.Scene _createSceneForImage(Rect bounds, double pixelRatio = 1.0)
    {
        var builder__54038 = new global::Doroti.Ui.SceneBuilder();
        var transform__54077 = Matrix4.diagonal3Values(pixelRatio, pixelRatio, 1);
        transform__54077.translateByDouble(-((bounds.left + this.offset.dx)), -((bounds.top + this.offset.dy)), 0, 1);
        builder__54038.pushTransform(transform__54077.storage);
        return buildScene(builder__54038);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async virtual Future<global::Doroti.Ui.Image> toImage(Rect bounds, double pixelRatio = 1.0)
    {
        global::Doroti.Ui.Scene scene__55344 = _createSceneForImage(bounds, pixelRatio: pixelRatio);
        try
        {
            return await scene__55344.toImage(((pixelRatio * bounds.width)).ceil(), ((pixelRatio * bounds.height)).ceil());
        }
        finally
        {
            scene__55344.dispose();
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Image toImageSync(Rect bounds, double pixelRatio = 1.0)
    {
        global::Doroti.Ui.Scene scene__56760 = _createSceneForImage(bounds, pixelRatio: pixelRatio);
        try
        {
            return scene__56760.toImageSync(((pixelRatio * bounds.width)).ceil(), ((pixelRatio * bounds.height)).ceil());
        }
        finally
        {
            scene__56760.dispose();
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ClipRectLayer : ContainerLayer
{
    internal virtual Rect? _clipRect { get; set; } = default;
    internal virtual Clip _clipBehavior { get; set; } = default!;

    public ClipRectLayer(Rect? clipRect = null, Clip clipBehavior = Clip.hardEdge)
    {
        this._clipRect = clipRect;
        this._clipBehavior = clipBehavior;
        System.Diagnostics.Debug.Assert((!object.Equals(clipBehavior, Clip.none)));
    }

    public virtual global::Doroti.Ui.Rect? clipRect
    {
        get => this._clipRect;
        set
        {
            var __value = value;
            if ((!object.Equals(__value, this._clipRect)))
            {
                _clipRect = __value;
                markNeedsAddToScene();
            }
        }
    }
    public override Rect? describeClipBounds() => this.clipRect;
    public virtual global::Doroti.Ui.Clip clipBehavior
    {
        get => this._clipBehavior;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (!object.Equals(DartRuntimePrimitives.RequireValue(__value), Clip.none)));
            if ((!object.Equals(DartRuntimePrimitives.RequireValue(__value), this._clipBehavior)))
            {
                _clipBehavior = DartRuntimePrimitives.RequireValue(__value);
                markNeedsAddToScene();
            }
        }
    }
    public override bool findAnnotations<S>(AnnotationResult<S> result, Offset localPosition, bool onlyFirst)
    {
        if (!DartRuntimePrimitives.RequireValue(this.clipRect).contains(localPosition))
        {
            return false;
        }
        return base.findAnnotations<S>(result, localPosition, onlyFirst: onlyFirst);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void addToScene(SceneBuilder builder)
    {
        DartRuntimePrimitives.Assert(() => (this.clipRect is not null));
        var enabled__59084 = true;
        DartRuntimePrimitives.Assert(() =>
            {
                enabled__59084 = !global::Doroti.Framework.Rendering.DebugLibrary.debugDisableClipLayers;
                return true;
            });
        if (enabled__59084)
        {
            engineLayer = builder.pushClipRect(DartRuntimePrimitives.RequireValue(this.clipRect), clipBehavior: this.clipBehavior, oldLayer: ((global::Doroti.Ui.ClipRectEngineLayer?)(object?)_engineLayer)!);
        }
        else
        {
            engineLayer = null;
        }
        addChildrenToScene(builder);
        if (enabled__59084)
        {
            builder.pop();
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Rect>("clipRect", this.clipRect));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Clip>("clipBehavior", this.clipBehavior));
    }

}

public class ClipRRectLayer : ContainerLayer
{
    internal virtual RRect? _clipRRect { get; set; } = default;
    internal virtual Clip _clipBehavior { get; set; } = default!;

    public ClipRRectLayer(RRect? clipRRect = null, Clip clipBehavior = Clip.antiAlias)
    {
        this._clipRRect = clipRRect;
        this._clipBehavior = clipBehavior;
        System.Diagnostics.Debug.Assert((!object.Equals(clipBehavior, Clip.none)));
    }

    public virtual global::Doroti.Ui.RRect? clipRRect
    {
        get => this._clipRRect;
        set
        {
            var __value = value is null ? null : (RRect)(object)value;
            if ((!object.Equals(__value, this._clipRRect)))
            {
                _clipRRect = __value;
                markNeedsAddToScene();
            }
        }
    }
    public override Rect? describeClipBounds() => this.clipRRect?.outerRect;
    public virtual global::Doroti.Ui.Clip clipBehavior
    {
        get => this._clipBehavior;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (!object.Equals(DartRuntimePrimitives.RequireValue(__value), Clip.none)));
            if ((!object.Equals(DartRuntimePrimitives.RequireValue(__value), this._clipBehavior)))
            {
                _clipBehavior = DartRuntimePrimitives.RequireValue(__value);
                markNeedsAddToScene();
            }
        }
    }
    public override bool findAnnotations<S>(AnnotationResult<S> result, Offset localPosition, bool onlyFirst)
    {
        if (!this.clipRRect!.contains(localPosition))
        {
            return false;
        }
        return base.findAnnotations<S>(result, localPosition, onlyFirst: onlyFirst);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void addToScene(SceneBuilder builder)
    {
        DartRuntimePrimitives.Assert(() => (this.clipRRect is not null));
        var enabled__61652 = true;
        DartRuntimePrimitives.Assert(() =>
            {
                enabled__61652 = !global::Doroti.Framework.Rendering.DebugLibrary.debugDisableClipLayers;
                return true;
            });
        if (enabled__61652)
        {
            engineLayer = builder.pushClipRRect(this.clipRRect!, clipBehavior: this.clipBehavior, oldLayer: ((global::Doroti.Ui.ClipRRectEngineLayer?)(object?)_engineLayer)!);
        }
        else
        {
            engineLayer = null;
        }
        addChildrenToScene(builder);
        if (enabled__61652)
        {
            builder.pop();
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.RRect>("clipRRect", this.clipRRect));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Clip>("clipBehavior", this.clipBehavior));
    }

}

public class ClipRSuperellipseLayer : ContainerLayer
{
    internal virtual RSuperellipse? _clipRSuperellipse { get; set; } = default;
    internal virtual Clip _clipBehavior { get; set; } = default!;

    public ClipRSuperellipseLayer(RSuperellipse? clipRSuperellipse = null, Clip clipBehavior = Clip.antiAlias)
    {
        this._clipRSuperellipse = clipRSuperellipse;
        this._clipBehavior = clipBehavior;
        System.Diagnostics.Debug.Assert((!object.Equals(clipBehavior, Clip.none)));
    }

    public virtual global::Doroti.Ui.RSuperellipse? clipRSuperellipse
    {
        get => this._clipRSuperellipse;
        set
        {
            var __value = value is null ? null : (RSuperellipse)(object)value;
            if ((!object.Equals(__value, this._clipRSuperellipse)))
            {
                _clipRSuperellipse = __value;
                markNeedsAddToScene();
            }
        }
    }
    public override Rect? describeClipBounds() => this.clipRSuperellipse?.outerRect;
    public virtual global::Doroti.Ui.Clip clipBehavior
    {
        get => this._clipBehavior;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (!object.Equals(DartRuntimePrimitives.RequireValue(__value), Clip.none)));
            if ((!object.Equals(DartRuntimePrimitives.RequireValue(__value), this._clipBehavior)))
            {
                _clipBehavior = DartRuntimePrimitives.RequireValue(__value);
                markNeedsAddToScene();
            }
        }
    }
    public override bool findAnnotations<S>(AnnotationResult<S> result, Offset localPosition, bool onlyFirst)
    {
        if (!this.clipRSuperellipse!.outerRect.contains(localPosition))
        {
            return false;
        }
        return base.findAnnotations<S>(result, localPosition, onlyFirst: onlyFirst);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void addToScene(SceneBuilder builder)
    {
        DartRuntimePrimitives.Assert(() => (this.clipRSuperellipse is not null));
        var enabled__64482 = true;
        DartRuntimePrimitives.Assert(() =>
            {
                enabled__64482 = !global::Doroti.Framework.Rendering.DebugLibrary.debugDisableClipLayers;
                return true;
            });
        if (enabled__64482)
        {
            engineLayer = builder.pushClipRSuperellipse(this.clipRSuperellipse!, clipBehavior: this.clipBehavior, oldLayer: ((global::Doroti.Ui.ClipRSuperellipseEngineLayer?)(object?)_engineLayer)!);
        }
        else
        {
            engineLayer = null;
        }
        addChildrenToScene(builder);
        if (enabled__64482)
        {
            builder.pop();
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.RSuperellipse>("clipRSuperellipse", this.clipRSuperellipse));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Clip>("clipBehavior", this.clipBehavior));
    }

}

public class ClipPathLayer : ContainerLayer
{
    internal virtual Path? _clipPath { get; set; } = default;
    internal virtual Clip _clipBehavior { get; set; } = default!;

    public ClipPathLayer(Path? clipPath = null, Clip clipBehavior = Clip.antiAlias)
    {
        this._clipPath = clipPath;
        this._clipBehavior = clipBehavior;
        System.Diagnostics.Debug.Assert((!object.Equals(clipBehavior, Clip.none)));
    }

    public virtual global::Doroti.Ui.Path? clipPath
    {
        get => this._clipPath;
        set
        {
            var __value = value is null ? null : (Path)(object)value;
            if ((!object.Equals(__value, this._clipPath)))
            {
                _clipPath = __value;
                markNeedsAddToScene();
            }
        }
    }
    public override Rect? describeClipBounds() => this.clipPath?.getBounds();
    public virtual global::Doroti.Ui.Clip clipBehavior
    {
        get => this._clipBehavior;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (!object.Equals(DartRuntimePrimitives.RequireValue(__value), Clip.none)));
            if ((!object.Equals(DartRuntimePrimitives.RequireValue(__value), this._clipBehavior)))
            {
                _clipBehavior = DartRuntimePrimitives.RequireValue(__value);
                markNeedsAddToScene();
            }
        }
    }
    public override bool findAnnotations<S>(AnnotationResult<S> result, Offset localPosition, bool onlyFirst)
    {
        if (!this.clipPath!.contains(localPosition))
        {
            return false;
        }
        return base.findAnnotations<S>(result, localPosition, onlyFirst: onlyFirst);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void addToScene(SceneBuilder builder)
    {
        DartRuntimePrimitives.Assert(() => (this.clipPath is not null));
        var enabled__67057 = true;
        DartRuntimePrimitives.Assert(() =>
            {
                enabled__67057 = !global::Doroti.Framework.Rendering.DebugLibrary.debugDisableClipLayers;
                return true;
            });
        if (enabled__67057)
        {
            engineLayer = builder.pushClipPath(this.clipPath!, clipBehavior: this.clipBehavior, oldLayer: ((global::Doroti.Ui.ClipPathEngineLayer?)(object?)_engineLayer)!);
        }
        else
        {
            engineLayer = null;
        }
        addChildrenToScene(builder);
        if (enabled__67057)
        {
            builder.pop();
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Clip>("clipBehavior", this.clipBehavior));
    }

}

public class ColorFilterLayer : ContainerLayer
{
    internal virtual ColorFilter? _colorFilter { get; set; } = default;

    public ColorFilterLayer(ColorFilter? colorFilter = null)
    {
        this._colorFilter = colorFilter;
    }

    public virtual global::Doroti.Ui.ColorFilter? colorFilter
    {
        get => this._colorFilter;
        set
        {
            var __value = value is null ? null : (ColorFilter)(object)value;
            DartRuntimePrimitives.Assert(() => (__value is not null));
            if ((!object.Equals(__value, this._colorFilter)))
            {
                _colorFilter = __value;
                markNeedsAddToScene();
            }
        }
    }
    public override void addToScene(SceneBuilder builder)
    {
        DartRuntimePrimitives.Assert(() => (this.colorFilter is not null));
        engineLayer = builder.pushColorFilter(this.colorFilter!, oldLayer: ((global::Doroti.Ui.ColorFilterEngineLayer?)(object?)_engineLayer)!);
        addChildrenToScene(builder);
        builder.pop();
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.ColorFilter>("colorFilter", this.colorFilter));
    }

}

public class ImageFilterLayer : OffsetLayer
{
    internal virtual ImageFilter? _imageFilter { get; set; } = default;
    internal virtual Rect? _bounds { get; set; } = default;

    public ImageFilterLayer(ImageFilter? imageFilter = null, Offset offset = default) : base(offset: offset)
    {
        this._imageFilter = imageFilter;
    }

    public virtual global::Doroti.Ui.ImageFilter? imageFilter
    {
        get => this._imageFilter;
        set
        {
            var __value = value is null ? null : (ImageFilter)(object)value;
            DartRuntimePrimitives.Assert(() => (__value is not null));
            if ((!object.Equals(__value, this._imageFilter)))
            {
                _imageFilter = __value;
                markNeedsAddToScene();
            }
        }
    }
    public virtual global::Doroti.Ui.Rect? bounds
    {
        get => this._bounds;
        set
        {
            if (!object.Equals(value, this._bounds))
            {
                _bounds = value;
                markNeedsAddToScene();
            }
        }
    }
    public override void addToScene(SceneBuilder builder)
    {
        DartRuntimePrimitives.Assert(() => (this.imageFilter is not null));
        engineLayer = builder.pushImageFilter(this.imageFilter!, offset: offset,
            oldLayer: ((global::Doroti.Ui.ImageFilterEngineLayer?)(object?)_engineLayer)!, bounds: bounds);
        addChildrenToScene(builder);
        builder.pop();
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.ImageFilter>("imageFilter", this.imageFilter));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Rect?>("bounds", this.bounds));
    }

}

public class TransformLayer : OffsetLayer
{
    internal virtual Matrix4? _transform { get; set; } = default;
    internal virtual Matrix4? _lastEffectiveTransform { get; set; } = default;
    internal virtual Matrix4? _invertedTransform { get; set; } = default;
    internal virtual bool _inverseDirty { get; set; } = true;

    public TransformLayer(Matrix4? transform = null, Offset offset = default) : base(offset: offset)
    {
        this._transform = transform;
    }

    public virtual Matrix4? transform
    {
        get => this._transform;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value is not null));
            DartRuntimePrimitives.Assert(() => __value!.storage.All(((component) => double.IsFinite(component))));
            if ((object.Equals(__value, this._transform)))
            {
                return;
            }
            _transform = __value;
            _inverseDirty = true;
            markNeedsAddToScene();
        }
    }
    public override void addToScene(SceneBuilder builder)
    {
        DartRuntimePrimitives.Assert(() => (this.transform is not null));
        _lastEffectiveTransform = this.transform;
        if ((!object.Equals(offset, Offset.zero)))
        {
            _lastEffectiveTransform = ((Func<Matrix4>)(() =>
{
    var __cascade = Matrix4.translationValues(offset.dx, offset.dy, 0.0);
    __cascade.multiply(this._lastEffectiveTransform!);
    return __cascade;
}))();
        }
        engineLayer = builder.pushTransform(this._lastEffectiveTransform!.storage, oldLayer: ((global::Doroti.Ui.TransformEngineLayer?)(object?)_engineLayer)!);
        addChildrenToScene(builder);
        builder.pop();
    }

    internal virtual global::Doroti.Ui.Offset? _transformOffset(Offset localPosition)
    {
        if (this._inverseDirty)
        {
            _invertedTransform = Matrix4.tryInvert(PointerEvent.removePerspectiveTransform(this.transform!));
            _inverseDirty = false;
        }
        if ((this._invertedTransform is null))
        {
            return null;
        }
        return MatrixUtils.transformPoint(this._invertedTransform!, localPosition);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool findAnnotations<S>(AnnotationResult<S> result, Offset localPosition, bool onlyFirst)
    {
        global::Doroti.Ui.Offset? transformedOffset__72553 = _transformOffset(localPosition);
        if ((transformedOffset__72553 is null))
        {
            return false;
        }
        return base.findAnnotations<S>(result, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(transformedOffset__72553)), onlyFirst: onlyFirst);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void applyTransform(Layer? child, Matrix4 transform)
    {
        DartRuntimePrimitives.Assert(() => (child is not null));
        DartRuntimePrimitives.Assert(() => ((this._lastEffectiveTransform is not null) || (this.transform is not null)));
        if ((this._lastEffectiveTransform is null))
        {
            transform.multiply(this.transform!);
        }
        else
        {
            transform.multiply(this._lastEffectiveTransform!);
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Painting.TransformProperty("transform", this.transform));
    }

}

public class OpacityLayer : OffsetLayer
{
    internal virtual long? _alpha { get; set; } = default;

    public OpacityLayer(long? alpha = null, Offset offset = default) : base(offset: offset)
    {
        this._alpha = alpha;
    }

    public virtual long? alpha
    {
        get => this._alpha;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value is not null));
            if ((__value != this._alpha))
            {
                if (((__value == 255L) || (this._alpha == 255L)))
                {
                    engineLayer = null;
                }
                _alpha = __value;
                markNeedsAddToScene();
            }
        }
    }
    public override void addToScene(SceneBuilder builder)
    {
        DartRuntimePrimitives.Assert(() => (this.alpha is not null));
        var enabled__74627 = (firstChild is not null);
        if (!enabled__74627)
        {
            engineLayer = null;
            return;
        }
        DartRuntimePrimitives.Assert(() =>
            {
                enabled__74627 = (enabled__74627 && !global::Doroti.Framework.Rendering.DebugLibrary.debugDisableOpacityLayers);
                return true;
            });
        long realizedAlpha__74987 = DartRuntimePrimitives.RequireValue(this.alpha);
        if ((enabled__74627 && (realizedAlpha__74987 < 255L)))
        {
            DartRuntimePrimitives.Assert(() => (_engineLayer is null or global::Doroti.Ui.OpacityEngineLayer));
            engineLayer = builder.pushOpacity(realizedAlpha__74987, offset: offset, oldLayer: ((global::Doroti.Ui.OpacityEngineLayer?)(object?)_engineLayer)!);
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (_engineLayer is null or global::Doroti.Ui.OffsetEngineLayer));
            engineLayer = builder.pushOffset(offset.dx, offset.dy, oldLayer: ((global::Doroti.Ui.OffsetEngineLayer?)(object?)_engineLayer)!);
        }
        addChildrenToScene(builder);
        builder.pop();
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new IntProperty("alpha", this.alpha));
    }

}

public class ShaderMaskLayer : ContainerLayer
{
    internal virtual Shader? _shader { get; set; } = default;
    internal virtual Rect? _maskRect { get; set; } = default;
    internal virtual BlendMode? _blendMode { get; set; } = default;

    public ShaderMaskLayer(Shader? shader = null, Rect? maskRect = null, BlendMode? blendMode = null)
    {
        this._shader = shader;
        this._maskRect = maskRect;
        this._blendMode = blendMode;
    }

    public virtual global::Doroti.Ui.Shader? shader
    {
        get => this._shader;
        set
        {
            var __value = value is null ? null : (Shader)(object)value;
            if ((!object.Equals(__value, this._shader)))
            {
                _shader = __value;
                markNeedsAddToScene();
            }
        }
    }
    public virtual global::Doroti.Ui.Rect? maskRect
    {
        get => this._maskRect;
        set
        {
            var __value = value;
            if ((!object.Equals(__value, this._maskRect)))
            {
                _maskRect = __value;
                markNeedsAddToScene();
            }
        }
    }
    public virtual global::Doroti.Ui.BlendMode? blendMode
    {
        get => this._blendMode;
        set
        {
            var __value = value;
            if ((!object.Equals(__value, this._blendMode)))
            {
                _blendMode = __value;
                markNeedsAddToScene();
            }
        }
    }
    public override void addToScene(SceneBuilder builder)
    {
        DartRuntimePrimitives.Assert(() => (this.shader is not null));
        DartRuntimePrimitives.Assert(() => (this.maskRect is not null));
        DartRuntimePrimitives.Assert(() => (this.blendMode is not null));
        engineLayer = builder.pushShaderMask(this.shader!, DartRuntimePrimitives.RequireValue(this.maskRect), DartRuntimePrimitives.RequireValue(this.blendMode), oldLayer: ((global::Doroti.Ui.ShaderMaskEngineLayer?)(object?)_engineLayer)!);
        addChildrenToScene(builder);
        builder.pop();
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Shader>("shader", this.shader));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Rect>("maskRect", this.maskRect));
        properties.add(new EnumProperty<global::Doroti.Ui.BlendMode>("blendMode", this.blendMode));
    }

}

public class BackdropKey
{
    internal static long _nextKey = 0L;
    internal virtual long _key { get; private set; } = default!;

    public BackdropKey()
    {
        this._key = _nextKey++;
    }

}

public class BackdropFilterLayer : ContainerLayer
{
    internal virtual ImageFilter? _filter { get; set; } = default;
    internal virtual BlendMode _blendMode { get; set; } = default!;
    internal virtual BackdropKey? _backdropKey { get; set; } = default;

    public BackdropFilterLayer(ImageFilter? filter = null, BlendMode blendMode = BlendMode.srcOver)
    {
        this._filter = filter;
        this._blendMode = blendMode;
    }

    public virtual global::Doroti.Ui.ImageFilter? filter
    {
        get => this._filter;
        set
        {
            var __value = value is null ? null : (ImageFilter)(object)value;
            if ((!object.Equals(__value, this._filter)))
            {
                _filter = __value;
                markNeedsAddToScene();
            }
        }
    }
    public virtual global::Doroti.Ui.BlendMode blendMode
    {
        get => this._blendMode;
        set
        {
            var __value = value;
            if ((!object.Equals(DartRuntimePrimitives.RequireValue(__value), this._blendMode)))
            {
                _blendMode = DartRuntimePrimitives.RequireValue(__value);
                markNeedsAddToScene();
            }
        }
    }
    public virtual BackdropKey? backdropKey
    {
        get => this._backdropKey;
        set
        {
            var __value = value;
            if ((!object.Equals(__value, this._backdropKey)))
            {
                _backdropKey = __value;
                markNeedsAddToScene();
            }
        }
    }
    public override void addToScene(SceneBuilder builder)
    {
        DartRuntimePrimitives.Assert(() => (this.filter is not null));
        engineLayer = builder.pushBackdropFilter(this.filter!, blendMode: this.blendMode, oldLayer: ((global::Doroti.Ui.BackdropFilterEngineLayer?)(object?)_engineLayer)!, backdropId: this._backdropKey?._key);
        addChildrenToScene(builder);
        builder.pop();
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.ImageFilter>("filter", this.filter));
        properties.add(new EnumProperty<global::Doroti.Ui.BlendMode>("blendMode", this.blendMode));
        properties.add(new IntProperty("backdropKey", this._backdropKey?._key));
    }

}

public class LayerLink
{
    internal virtual LeaderLayer? _leader { get; set; } = default;
    internal virtual HashSet<LeaderLayer>? _debugPreviousLeaders { get; set; } = default;
    internal virtual bool _debugLeaderCheckScheduled { get; set; } = false;
    public virtual Size? leaderSize { get; set; } = default;

    public virtual LeaderLayer? leader => this._leader;
    internal virtual void _registerLeader(LeaderLayer leader)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(this._leader, leader)));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._leader is not null))
                {
                    _debugPreviousLeaders ??= new HashSet<LeaderLayer>();
                    _debugScheduleLeadersCleanUpCheck();
                    return this._debugPreviousLeaders!.Add(this._leader!);
                }
                return true;
            });
        _leader = leader;
    }

    internal virtual void _unregisterLeader(LeaderLayer leader)
    {
        if ((object.Equals(this._leader, leader)))
        {
            _leader = null;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => this._debugPreviousLeaders!.Remove(leader));
        }
    }

    internal virtual void _debugScheduleLeadersCleanUpCheck()
    {
        DartRuntimePrimitives.Assert(() => (this._debugPreviousLeaders is not null));
        DartRuntimePrimitives.Assert(() =>
            {
                if (this._debugLeaderCheckScheduled)
                {
                    return true;
                }
                _debugLeaderCheckScheduled = true;
                SchedulerBinding.instance.addPostFrameCallback(((timeStamp) =>
                {
                    _debugLeaderCheckScheduled = false;
                    DartRuntimePrimitives.Assert(() => (checked((long)(this._debugPreviousLeaders!.Count)) == 0));
                }), debugLabel: "LayerLink.leadersCleanUpCheck");
                return true;
            });
    }

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        return $"{(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}({((this._leader is not null) ? "<linked>" : "<dangling>")})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class LeaderLayer : ContainerLayer
{
    internal virtual LayerLink _link { get; set; } = default!;
    internal virtual Offset _offset { get; set; } = default!;

    public LeaderLayer(LayerLink link, Offset offset = default)
    {
        this._link = link;
        this._offset = offset;
    }

    public virtual LayerLink link
    {
        get => this._link;
        set
        {
            var __value = value;
            if ((object.Equals(this._link, __value)))
            {
                return;
            }
            if (attached)
            {
                this._link._unregisterLeader(this);
                __value._registerLeader(this);
            }
            _link = __value;
        }
    }
    public virtual global::Doroti.Ui.Offset offset
    {
        get => this._offset;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._offset)))
            {
                return;
            }
            _offset = __value;
            if (!alwaysNeedsAddToScene)
            {
                markNeedsAddToScene();
            }
        }
    }
    public override void attach(object owner)
    {
        base.attach(owner);
        this._link._registerLeader(this);
    }

    public override void detach()
    {
        this._link._unregisterLeader(this);
        base.detach();
    }

    public override bool findAnnotations<S>(AnnotationResult<S> result, Offset localPosition, bool onlyFirst)
    {
        return base.findAnnotations<S>(result, (localPosition - this.offset), onlyFirst: onlyFirst);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void addToScene(SceneBuilder builder)
    {
        if ((!object.Equals(this.offset, Offset.zero)))
        {
            engineLayer = builder.pushTransform(Matrix4.translationValues(this.offset.dx, this.offset.dy, 0.0).storage, oldLayer: ((global::Doroti.Ui.TransformEngineLayer?)(object?)_engineLayer)!);
        }
        else
        {
            engineLayer = null;
        }
        addChildrenToScene(builder);
        if ((!object.Equals(this.offset, Offset.zero)))
        {
            builder.pop();
        }
    }

    public override void applyTransform(Layer? child, Matrix4 transform)
    {
        if ((!object.Equals(this.offset, Offset.zero)))
        {
            transform.translateByDouble(this.offset.dx, this.offset.dy, 0, 1);
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("offset", this.offset));
        properties.add(new DiagnosticsProperty<LayerLink>("link", this.link));
    }

}

public class FollowerLayer : ContainerLayer
{
    public virtual LayerLink link { get; set; } = default!;
    public virtual bool? showWhenUnlinked { get; set; } = default;
    public virtual Offset? unlinkedOffset { get; set; } = default;
    public virtual Offset? linkedOffset { get; set; } = default;
    internal virtual Offset? _lastOffset { get; set; } = default;
    internal virtual Matrix4? _lastTransform { get; set; } = default;
    internal virtual Matrix4? _invertedTransform { get; set; } = default;
    internal virtual bool _inverseDirty { get; set; } = true;

    public FollowerLayer(LayerLink link, bool? showWhenUnlinked = true, Offset? unlinkedOffset = default, Offset? linkedOffset = default)
    {
        this.link = link;
        this.showWhenUnlinked = showWhenUnlinked;
        this.unlinkedOffset = unlinkedOffset;
        this.linkedOffset = linkedOffset;
    }

    internal virtual global::Doroti.Ui.Offset? _transformOffset(Offset localPosition)
    {
        if (this._inverseDirty)
        {
            _invertedTransform = Matrix4.tryInvert(getLastTransform()!);
            _inverseDirty = false;
        }
        if ((this._invertedTransform is null))
        {
            return null;
        }
        var vector__91011 = new global::System.Numerics.Vector4(checked((float)localPosition.dx), checked((float)localPosition.dy), checked((float)0.0), checked((float)1.0));
        global::System.Numerics.Vector4 result__91093 = this._invertedTransform!.transform(vector__91011);
        return new global::Doroti.Ui.Offset((result__91093[(int)(0L)] - DartRuntimePrimitives.RequireValue(this.linkedOffset).dx), (result__91093[(int)(1L)] - DartRuntimePrimitives.RequireValue(this.linkedOffset).dy));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool findAnnotations<S>(AnnotationResult<S> result, Offset localPosition, bool onlyFirst)
    {
        if ((((LayerLink)this.link).leader is null))
        {
            if (DartRuntimePrimitives.RequireValue(this.showWhenUnlinked))
            {
                return base.findAnnotations(result, (localPosition - DartRuntimePrimitives.RequireValue(this.unlinkedOffset)), onlyFirst: onlyFirst);
            }
            return false;
        }
        global::Doroti.Ui.Offset? transformedOffset__91590 = _transformOffset(localPosition);
        if ((transformedOffset__91590 is null))
        {
            return false;
        }
        return base.findAnnotations<S>(result, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(transformedOffset__91590)), onlyFirst: onlyFirst);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Matrix4? getLastTransform()
    {
        if ((this._lastTransform is null))
        {
            return null;
        }
        var result__92190 = Matrix4.translationValues(-DartRuntimePrimitives.RequireValue(this._lastOffset).dx, -DartRuntimePrimitives.RequireValue(this._lastOffset).dy, 0.0);
        result__92190.multiply(this._lastTransform!);
        return result__92190;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static Matrix4 _collectTransformForLayerChain(List<ContainerLayer?> layers)
    {
        var result__92770 = Matrix4.identity();
        for (long index__92943 = (checked((long)(layers.Count)) - 1L); (index__92943 > 0L); index__92943 -= 1L)
        {
            layers[(int)(index__92943)]?.applyTransform(layers[(int)((index__92943 - 1L))], result__92770);
        }
        return result__92770;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static Layer? _pathsToCommonAncestor(Layer? a, Layer? b, List<ContainerLayer?> ancestorsA, List<ContainerLayer?> ancestorsB)
    {
        if (((a is null) || (b is null)))
        {
            return null;
        }
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if ((((Layer)a).depth < ((Layer)b).depth))
        {
            ancestorsB.Add(((Layer)b).parent);
            return _pathsToCommonAncestor(a, ((Layer)b).parent, ancestorsA, ancestorsB);
        }
        else
        {
            if ((((Layer)a).depth > ((Layer)b).depth))
            {
                ancestorsA.Add(((Layer)a).parent);
                return _pathsToCommonAncestor(((Layer)a).parent, b, ancestorsA, ancestorsB);
            }
        }
        ancestorsA.Add(((Layer)a).parent);
        ancestorsB.Add(((Layer)b).parent);
        return _pathsToCommonAncestor(((Layer)a).parent, ((Layer)b).parent, ancestorsA, ancestorsB);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _debugCheckLeaderBeforeFollower(List<ContainerLayer> leaderToCommonAncestor, List<ContainerLayer> followerToCommonAncestor)
    {
        if ((checked((long)(followerToCommonAncestor.Count)) <= 1L))
        {
            return false;
        }
        if ((checked((long)(leaderToCommonAncestor.Count)) <= 1L))
        {
            return true;
        }
        ContainerLayer leaderSubtreeBelowAncestor__94734 = leaderToCommonAncestor[(int)((checked((long)(leaderToCommonAncestor.Count)) - 2L))];
        ContainerLayer followerSubtreeBelowAncestor__94855 = followerToCommonAncestor[(int)((checked((long)(followerToCommonAncestor.Count)) - 2L))];
        Layer? sibling__94969 = leaderSubtreeBelowAncestor__94734;
        while ((sibling__94969 is not null))
        {
            if ((object.Equals(sibling__94969, followerSubtreeBelowAncestor__94855)))
            {
                return true;
            }
            sibling__94969 = ((Layer)sibling__94969).nextSibling;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _establishTransform()
    {
        _lastTransform = null;
        LeaderLayer? leader__95401 = ((LayerLink)this.link).leader;
        if ((leader__95401 is null))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => (object.Equals(leader__95401.owner, owner)));
        var forwardLayers__95784 = new List<ContainerLayer> { leader__95401 };
        var inverseLayers__95934 = new List<ContainerLayer> { this };
        Layer? ancestor__95992 = _pathsToCommonAncestor(leader__95401, this, forwardLayers__95784, inverseLayers__95934);
        DartRuntimePrimitives.Assert(() => (ancestor__95992 is not null));
        DartRuntimePrimitives.Assert(() => _debugCheckLeaderBeforeFollower(forwardLayers__95784, inverseLayers__95934));
        Matrix4 forwardTransform__96373 = _collectTransformForLayerChain(forwardLayers__95784);
        leader__95401.applyTransform(null, forwardTransform__96373);
        forwardTransform__96373.translateByDouble(DartRuntimePrimitives.RequireValue(this.linkedOffset).dx, DartRuntimePrimitives.RequireValue(this.linkedOffset).dy, 0, 1);
        Matrix4 inverseTransform__96796 = _collectTransformForLayerChain(inverseLayers__95934);
        if ((inverseTransform__96796.invert() == 0.0))
        {
            return;
        }
        inverseTransform__96796.multiply(forwardTransform__96373);
        _lastTransform = inverseTransform__96796;
        _inverseDirty = true;
    }

    public override bool alwaysNeedsAddToScene => true;
    public override void addToScene(SceneBuilder builder)
    {
        DartRuntimePrimitives.Assert(() => (this.showWhenUnlinked is not null));
        if (((((LayerLink)this.link).leader is null) && !DartRuntimePrimitives.RequireValue(this.showWhenUnlinked)))
        {
            _lastTransform = null;
            _lastOffset = null;
            _inverseDirty = true;
            engineLayer = null;
            return;
        }
        _establishTransform();
        if ((this._lastTransform is not null))
        {
            _lastOffset = this.unlinkedOffset;
            engineLayer = builder.pushTransform(this._lastTransform!.storage, oldLayer: ((global::Doroti.Ui.TransformEngineLayer?)(object?)_engineLayer)!);
            addChildrenToScene(builder);
            builder.pop();
        }
        else
        {
            _lastOffset = null;
            var matrix__98426 = Matrix4.translationValues(DartRuntimePrimitives.RequireValue(this.unlinkedOffset).dx, DartRuntimePrimitives.RequireValue(this.unlinkedOffset).dy, 0.0);
            engineLayer = builder.pushTransform(matrix__98426.storage, oldLayer: ((global::Doroti.Ui.TransformEngineLayer?)(object?)_engineLayer)!);
            addChildrenToScene(builder);
            builder.pop();
        }
        _inverseDirty = true;
    }

    public override void applyTransform(Layer? child, Matrix4 transform)
    {
        DartRuntimePrimitives.Assert(() => (child is not null));
        if ((this._lastTransform is not null))
        {
            transform.multiply(this._lastTransform!);
        }
        else
        {
            transform.multiply(Matrix4.translationValues(DartRuntimePrimitives.RequireValue(this.unlinkedOffset).dx, DartRuntimePrimitives.RequireValue(this.unlinkedOffset).dy, 0));
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<LayerLink>("link", this.link));
        properties.add(new global::Doroti.Framework.Painting.TransformProperty("transform", getLastTransform(), defaultValue: null));
    }

}

public class AnnotatedRegionLayer<T> : ContainerLayer
{
    public virtual T value { get; private set; } = default!;
    public virtual Size? size { get; private set; }
    public virtual Offset offset { get; private set; } = default!;
    public virtual bool opaque { get; private set; } = default!;

    public AnnotatedRegionLayer(T value, Size? size = null, Offset? offset = null, bool opaque = false)
    {
        this.value = value;
        this.size = size;
        this.opaque = opaque;
        this.offset = (offset ?? Offset.zero);
    }

    public override bool findAnnotations<S>(AnnotationResult<S> result, Offset localPosition, bool onlyFirst)
    {
        bool isAbsorbed__103779 = base.findAnnotations(result, localPosition, onlyFirst: onlyFirst);
        if (((((AnnotationResult<S>)result).entries.Count() != 0) && onlyFirst))
        {
            return isAbsorbed__103779;
        }
        if (((this.size is not null) && !((this.offset & DartRuntimePrimitives.RequireValue(this.size))).contains(localPosition)))
        {
            Size size__value103949 = DartRuntimePrimitives.RequireValue(size);
            return isAbsorbed__103779;
        }
        if ((object.Equals(typeof(T), typeof(S))))
        {
            isAbsorbed__103779 = (isAbsorbed__103779 || this.opaque);
            object untypedValue__104119 = this.value;
            var typedValue__104153 = ((S?)(object?)untypedValue__104119)!;
            result.add(new AnnotationEntry<S>(annotation: typedValue__104153, localPosition: (localPosition - this.offset)));
        }
        return isAbsorbed__103779;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<T>("value", this.value));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Size>("size", this.size, defaultValue: null));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("offset", this.offset, defaultValue: null));
        properties.add(new DiagnosticsProperty<bool>("opaque", this.opaque, defaultValue: false));
    }

}
