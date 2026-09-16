// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/layer.dart
using Doroti.Runtime;
using Doroti.Ui;

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
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "AnnotationEntry")}(annotation: {annotation}, localPosition: {localPosition})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class AnnotationResult<T>
{
    internal virtual List<AnnotationEntry<T>> _entries { get; private set; } = new List<AnnotationEntry<T>>();

    public virtual void add(AnnotationEntry<T> entry) => _entries.Add(entry);
    public virtual IEnumerable<AnnotationEntry<T>> entries => _entries;
    public virtual IEnumerable<T> annotations
    {
        get
        {
            return _entries.map((entry) => entry.annotation);
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

    public virtual bool subtreeHasCompositionCallbacks => _compositionCallbackCount > 0L;
    internal virtual void _updateSubtreeCompositionObserverCount(long delta)
    {
        DartRuntimePrimitives.Assert(() => delta != 0L);
        _compositionCallbackCount += delta;
        DartRuntimePrimitives.Assert(() => _compositionCallbackCount >= 0L);
        parent?._updateSubtreeCompositionObserverCount(delta);
    }

    internal virtual void _fireCompositionCallbacks(bool includeChildren)
    {
        if (checked((long)_callbacks.Count) == 0)
        {
            return;
        }
        foreach (var callback in new List<Action>(DartRuntimePrimitives.ConvertEnumerable<Action>(_callbacks.Values)))
        {
            callback();
        }
    }

    public virtual bool supportsRasterization()
    {
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Rect? describeClipBounds() => null;
    public virtual Action addCompositionCallback(Action<Layer> callback)
    {
        _updateSubtreeCompositionObserverCount(1L);
        long callbackId = _nextCallbackId += 1L;
        _callbacks[callbackId] = () =>
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
        };
        return () =>
        {
            DartRuntimePrimitives.Assert(() => debugDisposed || _callbacks.ContainsKey(callbackId));
            _callbacks.remove(callbackId);
            _updateSubtreeCompositionObserverCount(-1L);
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool debugDisposed
    {
        get
        {
            bool disposed = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    disposed = _debugDisposed;
                    return true;
                });
            return disposed;
        }
    }
    internal virtual void _unref()
    {
        DartRuntimePrimitives.Assert(() => !_debugMutationsLocked);
        DartRuntimePrimitives.Assert(() => _refCount > 0L);
        _refCount -= 1L;
        if (_refCount == 0L)
        {
            dispose();
        }
    }

    public virtual long debugHandleCount
    {
        get
        {
            long count = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    count = _refCount;
                    return true;
                });
            return count;
        }
    }
    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => !_debugMutationsLocked);
        DartRuntimePrimitives.Assert(() => !_debugDisposed);
        DartRuntimePrimitives.Assert(() =>
            {
                DartRuntimePrimitives.Assert(() => _refCount == 0L);
                _debugDisposed = true;
                return true;
            });
        DartRuntimePrimitives.Assert(() => Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        _engineLayer?.dispose();
        _engineLayer = null;
    }

    public virtual ContainerLayer? parent => _parent;
    public virtual void markNeedsAddToScene()
    {
        DartRuntimePrimitives.Assert(() => !_debugMutationsLocked);
        DartRuntimePrimitives.Assert(() => !alwaysNeedsAddToScene);
        DartRuntimePrimitives.Assert(() => !_debugDisposed);
        if (_needsAddToScene)
        {
            return;
        }
        _needsAddToScene = true;
    }

    public virtual void debugMarkClean()
    {
        DartRuntimePrimitives.Assert(() => !_debugMutationsLocked);
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
            bool? result = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    result = _needsAddToScene;
                    return true;
                });
            return result;
        }
    }
    public virtual EngineLayer? engineLayer
    {
        get => _engineLayer;
        set
        {
            var __value = value is null ? null : value;
            DartRuntimePrimitives.Assert(() => !_debugMutationsLocked);
            DartRuntimePrimitives.Assert(() => !_debugDisposed);
            // Doroti's managed SceneBuilder deliberately returns the same engine-layer
            // handle when a scope can be updated in place. Do not dispose that handle
            // while installing it again; doing so would turn the next retained scene
            // into a reference to an already-disposed resource.
            if (ReferenceEquals(_engineLayer, __value))
            {
                // A child may have updated its immutable retained payload while this
                // managed handle stayed stable. Its parent still needs to rebuild if
                // the child was composed outside the parent's current scene build.
                if (!alwaysNeedsAddToScene && (parent is not null) && !parent!.alwaysNeedsAddToScene)
                {
                    parent!.markNeedsAddToScene();
                }
                return;
            }
            _engineLayer?.dispose();
            _engineLayer = __value;
            if (!alwaysNeedsAddToScene)
            {
                if ((parent is not null) && !parent!.alwaysNeedsAddToScene)
                {
                    parent!.markNeedsAddToScene();
                }
            }
        }
    }
    public virtual void updateSubtreeNeedsAddToScene()
    {
        DartRuntimePrimitives.Assert(() => !_debugMutationsLocked);
        _needsAddToScene = _needsAddToScene || alwaysNeedsAddToScene;
    }

    public virtual object? owner => _owner;
    public virtual bool attached => _owner is not null;
    public virtual void attach(object owner)
    {
        DartRuntimePrimitives.Assert(() => _owner is null);
        _owner = owner;
    }

    public virtual void detach()
    {
        DartRuntimePrimitives.Assert(() => _owner is not null);
        _owner = null;
        DartRuntimePrimitives.Assert(() => (parent is null) || (attached == parent!.attached));
    }

    public virtual long depth => _depth;
    public virtual void redepthChildren()
    {
    }

    public virtual Layer? nextSibling => _nextSibling;
    public virtual Layer? previousSibling => _previousSibling;
    public virtual void remove()
    {
        DartRuntimePrimitives.Assert(() => !_debugMutationsLocked);
        parent?._removeChild(this);
    }

    public virtual bool findAnnotations<S>(AnnotationResult<S> result, Offset localPosition, bool onlyFirst)
    {
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual S? find<S>(Offset localPosition)
    {
        var result = new AnnotationResult<S>();
        findAnnotations(result, localPosition, onlyFirst: true);
        return (result.entries.Count() == 0) ? default(S) : result.entries.First().annotation;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual AnnotationResult<S> findAllAnnotations<S>(Offset localPosition)
    {
        var result = new AnnotationResult<S>();
        findAnnotations(result, localPosition, onlyFirst: false);
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract void addToScene(SceneBuilder builder);
    internal virtual void _addToSceneWithRetainedRendering(SceneBuilder builder)
    {
        DartRuntimePrimitives.Assert(() => !_debugMutationsLocked);
        // This is the same retained-layer decision as Flutter: a clean layer with
        // a completed engine handle contributes one immutable retained node. The
        // host still replays that node into a fresh native back buffer when needed,
        // but it does not re-record the unchanged subtree.
        if (!_needsAddToScene && (_engineLayer is not null))
        {
            builder.addRetained(_engineLayer!);
            return;
        }
        addToScene(builder);
        _needsAddToScene = false;
    }

    public override string toStringShort() => $"{base.toStringShort()}{((owner is null) ? " DETACHED" : "")}";
    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<object>("owner", owner, level: (parent is not null) ? DiagnosticLevel.hidden : DiagnosticLevel.info, defaultValue: null));
        properties.add(new DiagnosticsProperty<object?>("creator", debugCreator, defaultValue: null, level: DiagnosticLevel.debug));
        if (_engineLayer is not null)
        {
            properties.add(new DiagnosticsProperty<string>("engine layer", DiagnosticsLibrary.describeIdentity(_engineLayer)));
        }
        properties.add(new DiagnosticsProperty<long>("handles", debugHandleCount));
    }

}

public class LayerHandle<T> where T : Layer
{
    internal virtual T? _layer { get; set; } = default;

    public LayerHandle(T? _layer = default)
    {
        DartRuntimePrimitives.Assert(() => _layer?.debugDisposed != true);
        this._layer = _layer;
        if (_layer is not null)
        {
            _layer._refCount += 1L;
        }
    }

    public virtual T? layer
    {
        get => _layer;
        set
        {
            var layer = value;
            DartRuntimePrimitives.Assert(() => layer?.debugDisposed != true);
            if (DartRuntimePrimitives.Identical(layer, _layer))
            {
                return;
            }
            _layer?._unref();
            _layer = layer;
            if (_layer is not null)
            {
                _layer!._refCount += 1L;
            }
        }
    }
    public override string ToString() => $"LayerHandle({((_layer is not null) ? _layer.ToString() : "DISPOSED")})";
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

    public virtual Picture? picture
    {
        get => _picture;
        set
        {
            var picture = value is null ? null : value;
            DartRuntimePrimitives.Assert(() => !_debugDisposed);
            markNeedsAddToScene();
            _picture?.dispose();
            _picture = picture;
        }
    }
    public virtual bool isComplexHint
    {
        get => _isComplexHint;
        set
        {
            var __value = value;
            if (__value != _isComplexHint)
            {
                _isComplexHint = __value;
                markNeedsAddToScene();
            }
        }
    }
    public virtual bool willChangeHint
    {
        get => _willChangeHint;
        set
        {
            var __value = value;
            if (__value != _willChangeHint)
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
        DartRuntimePrimitives.Assert(() => picture is not null);
        builder.addPicture(
            Offset.zero,
            picture!,
            canvasBounds,
            isComplexHint: isComplexHint,
            willChangeHint: willChangeHint);
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Rect>("paint bounds", canvasBounds));
        properties.add(new DiagnosticsProperty<string>("picture", DiagnosticsLibrary.describeIdentity(_picture)));
        properties.add(new DiagnosticsProperty<string>("raster cache hints", $"isComplex = {isComplexHint}, willChange = {willChangeHint}"));
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
        builder.addTexture(textureId, offset: rect.topLeft, width: rect.width, height: rect.height, freeze: freeze, filterQuality: filterQuality);
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
        builder.addPlatformView(viewId, offset: rect.topLeft, width: rect.width, height: rect.height);
    }

}

public class PerformanceOverlayLayer : Layer
{
    internal virtual Rect _overlayRect { get; set; } = default!;
    public virtual long optionsMask { get; private set; } = default!;

    public PerformanceOverlayLayer(Rect overlayRect, long optionsMask)
    {
        this.optionsMask = optionsMask;
        _overlayRect = overlayRect;
    }

    public virtual Rect overlayRect
    {
        get => _overlayRect;
        set
        {
            var __value = value;
            if (!Equals(__value, _overlayRect))
            {
                _overlayRect = __value;
                markNeedsAddToScene();
            }
        }
    }
    public override void addToScene(SceneBuilder builder)
    {
        builder.addPerformanceOverlay(optionsMask, overlayRect);
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
        Layer? child = firstChild;
        while (child is not null)
        {
            child._fireCompositionCallbacks(includeChildren: includeChildren);
            child = child.nextSibling;
        }
    }

    public virtual Layer? firstChild => _firstChild;
    public virtual Layer? lastChild => _lastChild;
    public virtual bool hasChildren => _firstChild is not null;
    public override bool supportsRasterization()
    {
        for (Layer? child = lastChild; child is not null; child = child.previousSibling)
        {
            if (!child.supportsRasterization())
            {
                return false;
            }
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Scene buildScene(SceneBuilder builder)
    {
        updateSubtreeNeedsAddToScene();
        addToScene(builder);
        if (subtreeHasCompositionCallbacks)
        {
            _fireCompositionCallbacks(includeChildren: true);
        }
        _needsAddToScene = false;
        Scene scene = builder.build();
        return scene;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _debugUltimatePreviousSiblingOf(Layer child, Layer? equals = null)
    {
        DartRuntimePrimitives.Assert(() => child.attached == attached);
        while (child.previousSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(child.previousSibling, child));
            child = child.previousSibling!;
            DartRuntimePrimitives.Assert(() => child.attached == attached);
        }
        return Equals(child, equals);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _debugUltimateNextSiblingOf(Layer child, Layer? equals = null)
    {
        DartRuntimePrimitives.Assert(() => child.attached == attached);
        while (child._nextSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(child._nextSibling, child));
            child = child._nextSibling!;
            DartRuntimePrimitives.Assert(() => child.attached == attached);
        }
        return Equals(child, equals);
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
        Layer? child = firstChild;
        while (child is not null)
        {
            child.updateSubtreeNeedsAddToScene();
            _needsAddToScene = _needsAddToScene || child._needsAddToScene;
            child = child.nextSibling;
        }
    }

    public override bool findAnnotations<S>(AnnotationResult<S> result, Offset localPosition, bool onlyFirst)
    {
        for (Layer? child = lastChild; child is not null; child = child.previousSibling)
        {
            bool isAbsorbed = child.findAnnotations(result, localPosition, onlyFirst: onlyFirst);
            if (isAbsorbed)
            {
                return true;
            }
            if (onlyFirst && (result.entries.Count() != 0))
            {
                return isAbsorbed;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void attach(object owner)
    {
        DartRuntimePrimitives.Assert(() => !_debugMutationsLocked);
        base.attach(owner);
        Layer? child = firstChild;
        while (child is not null)
        {
            child.attach(owner);
            child = child.nextSibling;
        }
    }

    public override void detach()
    {
        DartRuntimePrimitives.Assert(() => !_debugMutationsLocked);
        base.detach();
        Layer? child = firstChild;
        while (child is not null)
        {
            child.detach();
            child = child.nextSibling;
        }
        _fireCompositionCallbacks(includeChildren: false);
    }

    public virtual void append(Layer child)
    {
        DartRuntimePrimitives.Assert(() => !_debugMutationsLocked);
        DartRuntimePrimitives.Assert(() => !Equals(child, this));
        DartRuntimePrimitives.Assert(() => !Equals(child, firstChild));
        DartRuntimePrimitives.Assert(() => !Equals(child, lastChild));
        DartRuntimePrimitives.Assert(() => child.parent is null);
        DartRuntimePrimitives.Assert(() => !child.attached);
        DartRuntimePrimitives.Assert(() => child.nextSibling is null);
        DartRuntimePrimitives.Assert(() => child.previousSibling is null);
        DartRuntimePrimitives.Assert(() => child._parentHandle.layer is null);
        DartRuntimePrimitives.Assert(() =>
            {
                Layer node = this;
                while (node.parent is not null)
                {
                    node = node.parent!;
                }
                DartRuntimePrimitives.Assert(() => !Equals(node, child));
                return true;
            });
        _adoptChild(child);
        child._previousSibling = lastChild;
        if (lastChild is not null)
        {
            lastChild!._nextSibling = child;
        }
        _lastChild = child;
        _firstChild ??= child;
        child._parentHandle.layer = child;
        DartRuntimePrimitives.Assert(() => child.attached == attached);
    }

    internal virtual void _adoptChild(Layer child)
    {
        DartRuntimePrimitives.Assert(() => !_debugMutationsLocked);
        if (!alwaysNeedsAddToScene)
        {
            markNeedsAddToScene();
        }
        if (child._compositionCallbackCount != 0L)
        {
            _updateSubtreeCompositionObserverCount(child._compositionCallbackCount);
        }
        DartRuntimePrimitives.Assert(() => child._parent is null);
        DartRuntimePrimitives.Assert(() =>
            {
                Layer node = this;
                while (node.parent is not null)
                {
                    node = node.parent!;
                }
                DartRuntimePrimitives.Assert(() => !Equals(node, child));
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
        Layer? child = firstChild;
        while (child is not null)
        {
            redepthChild(child);
            child = child.nextSibling;
        }
    }

    public virtual void redepthChild(Layer child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.owner, owner));
        if (child._depth <= _depth)
        {
            child._depth = _depth + 1L;
            child.redepthChildren();
        }
    }

    internal virtual void _removeChild(Layer child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        DartRuntimePrimitives.Assert(() => child.attached == attached);
        DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(child, equals: firstChild));
        DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(child, equals: lastChild));
        DartRuntimePrimitives.Assert(() => child._parentHandle.layer is not null);
        if (child._previousSibling is null)
        {
            DartRuntimePrimitives.Assert(() => Equals(_firstChild, child));
            _firstChild = child._nextSibling;
        }
        else
        {
            child._previousSibling!._nextSibling = child.nextSibling;
        }
        if (child._nextSibling is null)
        {
            DartRuntimePrimitives.Assert(() => Equals(lastChild, child));
            _lastChild = child.previousSibling;
        }
        else
        {
            child.nextSibling!._previousSibling = child.previousSibling;
        }
        DartRuntimePrimitives.Assert(() => firstChild is null == lastChild is null);
        DartRuntimePrimitives.Assert(() => (firstChild is null) || (firstChild!.attached == attached));
        DartRuntimePrimitives.Assert(() => (lastChild is null) || (lastChild!.attached == attached));
        DartRuntimePrimitives.Assert(() => (firstChild is null) || _debugUltimateNextSiblingOf(firstChild!, equals: lastChild));
        DartRuntimePrimitives.Assert(() => (lastChild is null) || _debugUltimatePreviousSiblingOf(lastChild!, equals: firstChild));
        child._previousSibling = null;
        child._nextSibling = null;
        _dropChild(child);
        child._parentHandle.layer = null;
        DartRuntimePrimitives.Assert(() => !child.attached);
    }

    internal virtual void _dropChild(Layer child)
    {
        DartRuntimePrimitives.Assert(() => !_debugMutationsLocked);
        if (!alwaysNeedsAddToScene)
        {
            markNeedsAddToScene();
        }
        if (child._compositionCallbackCount != 0L)
        {
            _updateSubtreeCompositionObserverCount(-child._compositionCallbackCount);
        }
        DartRuntimePrimitives.Assert(() => Equals(child._parent, this));
        DartRuntimePrimitives.Assert(() => child.attached == attached);
        child._parent = null;
        if (attached)
        {
            child.detach();
        }
    }

    public virtual void removeAllChildren()
    {
        DartRuntimePrimitives.Assert(() => !_debugMutationsLocked);
        Layer? child = firstChild;
        while (child is not null)
        {
            Layer? next = child.nextSibling;
            child._previousSibling = null;
            child._nextSibling = null;
            DartRuntimePrimitives.Assert(() => child.attached == attached);
            _dropChild(child);
            child._parentHandle.layer = null;
            child = next;
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
        Layer? child = firstChild;
        while (child is not null)
        {
            child._addToSceneWithRetainedRendering(builder);
            child = child.nextSibling;
        }
    }

    public virtual void applyTransform(Layer? child, Matrix4 transform)
    {
        DartRuntimePrimitives.Assert(() => child is not null);
    }

    public virtual List<Layer> depthFirstIterateChildren()
    {
        if (firstChild is null)
        {
            return new List<Layer>();
        }
        var children = new List<Layer>();
        Layer? child = firstChild;
        while (child is not null)
        {
            children.Add(child);
            if (child is ContainerLayer)
            {
                ContainerLayer child__50793__as50878 = (ContainerLayer)child;
                children.AddRange(child__50793__as50878.depthFirstIterateChildren());
            }
            child = child.nextSibling;
        }
        return children;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        var children = new List<DiagnosticsNode>();
        if (firstChild is null)
        {
            return children;
        }
        Layer? child = firstChild;
        var count = 1L;
        while (true)
        {
            children.Add(((Diagnosticable)child!).toDiagnosticsNode(name: $"child {count}"));
            if (Equals(child, lastChild))
            {
                break;
            }
            count += 1L;
            child = child.nextSibling;
        }
        return children;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class OffsetLayer : ContainerLayer
{
    internal virtual Offset _offset { get; set; } = default!;

    public OffsetLayer(Offset offset = default)
    {
        _offset = offset;
    }

    public virtual Offset offset
    {
        get => _offset;
        set
        {
            var __value = value;
            if (!Equals(__value, _offset))
            {
                markNeedsAddToScene();
            }
            _offset = __value;
        }
    }
    public override bool findAnnotations<S>(AnnotationResult<S> result, Offset localPosition, bool onlyFirst)
    {
        return base.findAnnotations(result, localPosition - offset, onlyFirst: onlyFirst);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void applyTransform(Layer? child, Matrix4 transform)
    {
        DartRuntimePrimitives.Assert(() => child is not null);
        transform.translateByDouble(offset.dx, offset.dy, 0, 1);
    }

    public override void addToScene(SceneBuilder builder)
    {
        engineLayer = builder.pushOffset(
            offset.dx,
            offset.dy,
            oldLayer: ((OffsetEngineLayer?)_engineLayer)!);
        addChildrenToScene(builder);
        builder.pop();
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Offset>("offset", offset));
    }

    internal virtual Scene _createSceneForImage(Rect bounds, double pixelRatio = 1.0)
    {
        var builder = new SceneBuilder();
        var transform = Matrix4.diagonal3Values(pixelRatio, pixelRatio, 1);
        transform.translateByDouble(-(bounds.left + offset.dx), -(bounds.top + offset.dy), 0, 1);
        builder.pushTransform(transform.storage);
        return buildScene(builder);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async virtual Future<Image> toImage(Rect bounds, double pixelRatio = 1.0)
    {
        Scene scene = _createSceneForImage(bounds, pixelRatio: pixelRatio);
        try
        {
            return await scene.toImage((pixelRatio * bounds.width).ceil(), (pixelRatio * bounds.height).ceil());
        }
        finally
        {
            scene.dispose();
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Image toImageSync(Rect bounds, double pixelRatio = 1.0)
    {
        Scene scene = _createSceneForImage(bounds, pixelRatio: pixelRatio);
        try
        {
            return scene.toImageSync((pixelRatio * bounds.width).ceil(), (pixelRatio * bounds.height).ceil());
        }
        finally
        {
            scene.dispose();
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
        _clipRect = clipRect;
        _clipBehavior = clipBehavior;
        System.Diagnostics.Debug.Assert(!Equals(clipBehavior, Clip.none));
    }

    public virtual Rect? clipRect
    {
        get => _clipRect;
        set
        {
            var __value = value;
            if (!Equals(__value, _clipRect))
            {
                _clipRect = __value;
                markNeedsAddToScene();
            }
        }
    }
    public override Rect? describeClipBounds() => clipRect;
    public virtual Clip clipBehavior
    {
        get => _clipBehavior;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => !Equals(DartRuntimePrimitives.RequireValue(__value), Clip.none));
            if (!Equals(DartRuntimePrimitives.RequireValue(__value), _clipBehavior))
            {
                _clipBehavior = DartRuntimePrimitives.RequireValue(__value);
                markNeedsAddToScene();
            }
        }
    }
    public override bool findAnnotations<S>(AnnotationResult<S> result, Offset localPosition, bool onlyFirst)
    {
        if (!DartRuntimePrimitives.RequireValue(clipRect).contains(localPosition))
        {
            return false;
        }
        return base.findAnnotations(result, localPosition, onlyFirst: onlyFirst);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void addToScene(SceneBuilder builder)
    {
        DartRuntimePrimitives.Assert(() => clipRect is not null);
        var enabled = true;
        DartRuntimePrimitives.Assert(() =>
            {
                enabled = !DebugLibrary.debugDisableClipLayers;
                return true;
            });
        if (enabled)
        {
            engineLayer = builder.pushClipRect(DartRuntimePrimitives.RequireValue(clipRect), clipBehavior: clipBehavior, oldLayer: ((ClipRectEngineLayer?)_engineLayer)!);
        }
        else
        {
            engineLayer = null;
        }
        addChildrenToScene(builder);
        if (enabled)
        {
            builder.pop();
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Rect>("clipRect", clipRect));
        properties.add(new DiagnosticsProperty<Clip>("clipBehavior", clipBehavior));
    }

}

public class ClipRRectLayer : ContainerLayer
{
    internal virtual RRect? _clipRRect { get; set; } = default;
    internal virtual Clip _clipBehavior { get; set; } = default!;

    public ClipRRectLayer(RRect? clipRRect = null, Clip clipBehavior = Clip.antiAlias)
    {
        _clipRRect = clipRRect;
        _clipBehavior = clipBehavior;
        System.Diagnostics.Debug.Assert(!Equals(clipBehavior, Clip.none));
    }

    public virtual RRect? clipRRect
    {
        get => _clipRRect;
        set
        {
            var __value = value is null ? null : value;
            if (!Equals(__value, _clipRRect))
            {
                _clipRRect = __value;
                markNeedsAddToScene();
            }
        }
    }
    public override Rect? describeClipBounds() => clipRRect?.outerRect;
    public virtual Clip clipBehavior
    {
        get => _clipBehavior;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => !Equals(DartRuntimePrimitives.RequireValue(__value), Clip.none));
            if (!Equals(DartRuntimePrimitives.RequireValue(__value), _clipBehavior))
            {
                _clipBehavior = DartRuntimePrimitives.RequireValue(__value);
                markNeedsAddToScene();
            }
        }
    }
    public override bool findAnnotations<S>(AnnotationResult<S> result, Offset localPosition, bool onlyFirst)
    {
        if (!clipRRect!.contains(localPosition))
        {
            return false;
        }
        return base.findAnnotations(result, localPosition, onlyFirst: onlyFirst);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void addToScene(SceneBuilder builder)
    {
        DartRuntimePrimitives.Assert(() => clipRRect is not null);
        var enabled = true;
        DartRuntimePrimitives.Assert(() =>
            {
                enabled = !DebugLibrary.debugDisableClipLayers;
                return true;
            });
        if (enabled)
        {
            engineLayer = builder.pushClipRRect(clipRRect!, clipBehavior: clipBehavior, oldLayer: ((ClipRRectEngineLayer?)_engineLayer)!);
        }
        else
        {
            engineLayer = null;
        }
        addChildrenToScene(builder);
        if (enabled)
        {
            builder.pop();
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<RRect>("clipRRect", clipRRect));
        properties.add(new DiagnosticsProperty<Clip>("clipBehavior", clipBehavior));
    }

}

public class ClipRSuperellipseLayer : ContainerLayer
{
    internal virtual RSuperellipse? _clipRSuperellipse { get; set; } = default;
    internal virtual Clip _clipBehavior { get; set; } = default!;

    public ClipRSuperellipseLayer(RSuperellipse? clipRSuperellipse = null, Clip clipBehavior = Clip.antiAlias)
    {
        _clipRSuperellipse = clipRSuperellipse;
        _clipBehavior = clipBehavior;
        System.Diagnostics.Debug.Assert(!Equals(clipBehavior, Clip.none));
    }

    public virtual RSuperellipse? clipRSuperellipse
    {
        get => _clipRSuperellipse;
        set
        {
            var __value = value is null ? null : value;
            if (!Equals(__value, _clipRSuperellipse))
            {
                _clipRSuperellipse = __value;
                markNeedsAddToScene();
            }
        }
    }
    public override Rect? describeClipBounds() => clipRSuperellipse?.outerRect;
    public virtual Clip clipBehavior
    {
        get => _clipBehavior;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => !Equals(DartRuntimePrimitives.RequireValue(__value), Clip.none));
            if (!Equals(DartRuntimePrimitives.RequireValue(__value), _clipBehavior))
            {
                _clipBehavior = DartRuntimePrimitives.RequireValue(__value);
                markNeedsAddToScene();
            }
        }
    }
    public override bool findAnnotations<S>(AnnotationResult<S> result, Offset localPosition, bool onlyFirst)
    {
        if (!clipRSuperellipse!.outerRect.contains(localPosition))
        {
            return false;
        }
        return base.findAnnotations(result, localPosition, onlyFirst: onlyFirst);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void addToScene(SceneBuilder builder)
    {
        DartRuntimePrimitives.Assert(() => clipRSuperellipse is not null);
        var enabled = true;
        DartRuntimePrimitives.Assert(() =>
            {
                enabled = !DebugLibrary.debugDisableClipLayers;
                return true;
            });
        if (enabled)
        {
            engineLayer = builder.pushClipRSuperellipse(clipRSuperellipse!, clipBehavior: clipBehavior, oldLayer: ((ClipRSuperellipseEngineLayer?)_engineLayer)!);
        }
        else
        {
            engineLayer = null;
        }
        addChildrenToScene(builder);
        if (enabled)
        {
            builder.pop();
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<RSuperellipse>("clipRSuperellipse", clipRSuperellipse));
        properties.add(new DiagnosticsProperty<Clip>("clipBehavior", clipBehavior));
    }

}

public class ClipPathLayer : ContainerLayer
{
    internal virtual Path? _clipPath { get; set; } = default;
    internal virtual Clip _clipBehavior { get; set; } = default!;

    public ClipPathLayer(Path? clipPath = null, Clip clipBehavior = Clip.antiAlias)
    {
        _clipPath = clipPath;
        _clipBehavior = clipBehavior;
        System.Diagnostics.Debug.Assert(!Equals(clipBehavior, Clip.none));
    }

    public virtual Path? clipPath
    {
        get => _clipPath;
        set
        {
            var __value = value is null ? null : value;
            if (!Equals(__value, _clipPath))
            {
                _clipPath = __value;
                markNeedsAddToScene();
            }
        }
    }
    public override Rect? describeClipBounds() => clipPath?.getBounds();
    public virtual Clip clipBehavior
    {
        get => _clipBehavior;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => !Equals(DartRuntimePrimitives.RequireValue(__value), Clip.none));
            if (!Equals(DartRuntimePrimitives.RequireValue(__value), _clipBehavior))
            {
                _clipBehavior = DartRuntimePrimitives.RequireValue(__value);
                markNeedsAddToScene();
            }
        }
    }
    public override bool findAnnotations<S>(AnnotationResult<S> result, Offset localPosition, bool onlyFirst)
    {
        if (!clipPath!.contains(localPosition))
        {
            return false;
        }
        return base.findAnnotations(result, localPosition, onlyFirst: onlyFirst);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void addToScene(SceneBuilder builder)
    {
        DartRuntimePrimitives.Assert(() => clipPath is not null);
        var enabled = true;
        DartRuntimePrimitives.Assert(() =>
            {
                enabled = !DebugLibrary.debugDisableClipLayers;
                return true;
            });
        if (enabled)
        {
            engineLayer = builder.pushClipPath(clipPath!, clipBehavior: clipBehavior, oldLayer: ((ClipPathEngineLayer?)_engineLayer)!);
        }
        else
        {
            engineLayer = null;
        }
        addChildrenToScene(builder);
        if (enabled)
        {
            builder.pop();
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Clip>("clipBehavior", clipBehavior));
    }

}

public class ColorFilterLayer : ContainerLayer
{
    internal virtual ColorFilter? _colorFilter { get; set; } = default;

    public ColorFilterLayer(ColorFilter? colorFilter = null)
    {
        _colorFilter = colorFilter;
    }

    public virtual ColorFilter? colorFilter
    {
        get => _colorFilter;
        set
        {
            var __value = value is null ? null : value;
            DartRuntimePrimitives.Assert(() => __value is not null);
            if (!Equals(__value, _colorFilter))
            {
                _colorFilter = __value;
                markNeedsAddToScene();
            }
        }
    }
    public override void addToScene(SceneBuilder builder)
    {
        DartRuntimePrimitives.Assert(() => colorFilter is not null);
        engineLayer = builder.pushColorFilter(colorFilter!, oldLayer: ((ColorFilterEngineLayer?)_engineLayer)!);
        addChildrenToScene(builder);
        builder.pop();
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<ColorFilter>("colorFilter", colorFilter));
    }

}

public class ImageFilterLayer : OffsetLayer
{
    internal virtual ImageFilter? _imageFilter { get; set; } = default;
    internal virtual Rect? _bounds { get; set; } = default;
    internal virtual long _filterCacheGeneration { get; set; }
    internal virtual bool _filterInputDirty { get; set; } = true;

    public ImageFilterLayer(ImageFilter? imageFilter = null, Offset offset = default) : base(offset: offset)
    {
        _imageFilter = imageFilter;
    }

    public virtual ImageFilter? imageFilter
    {
        get => _imageFilter;
        set
        {
            var __value = value is null ? null : value;
            DartRuntimePrimitives.Assert(() => __value is not null);
            if (!Equals(__value, _imageFilter))
            {
                _imageFilter = __value;
                _filterInputDirty = true;
                markNeedsAddToScene();
            }
        }
    }
    public virtual Rect? bounds
    {
        get => _bounds;
        set
        {
            if (!Equals(value, _bounds))
            {
                _bounds = value;
                _filterInputDirty = true;
                markNeedsAddToScene();
            }
        }
    }
    internal override void _adoptChild(Layer child)
    {
        _filterInputDirty = true;
        base._adoptChild(child);
    }
    internal override void _dropChild(Layer child)
    {
        _filterInputDirty = true;
        base._dropChild(child);
    }
    public override void addToScene(SceneBuilder builder)
    {
        DartRuntimePrimitives.Assert(() => imageFilter is not null);
        var childNeedsUpdate = false;
        for (Layer? child = firstChild; child is not null; child = child.nextSibling)
        {
            if (child._needsAddToScene)
            {
                childNeedsUpdate = true;
                break;
            }
        }
        // The filtered pixels are translation-independent. Moving this repaint
        // boundary during a scroll still rebuilds its scene scope, but it must not
        // discard an otherwise unchanged GPU image-filter result.
        if (_filterInputDirty || childNeedsUpdate)
        {
            _filterCacheGeneration++;
            _filterInputDirty = false;
        }
        engineLayer = builder.pushImageFilter(imageFilter!, offset: offset,
            oldLayer: ((ImageFilterEngineLayer?)_engineLayer)!, bounds: bounds,
            cacheKey: this, cacheGeneration: _filterCacheGeneration);
        addChildrenToScene(builder);
        builder.pop();
    }

    public virtual long debugFilterCacheGeneration => _filterCacheGeneration;

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<ImageFilter>("imageFilter", imageFilter));
        properties.add(new DiagnosticsProperty<Rect?>("bounds", bounds));
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
        _transform = transform;
    }

    public virtual Matrix4? transform
    {
        get => _transform;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value is not null);
            DartRuntimePrimitives.Assert(() => __value!.storage.All((component) => double.IsFinite(component)));
            if (Equals(__value, _transform))
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
        DartRuntimePrimitives.Assert(() => transform is not null);
        _lastEffectiveTransform = transform;
        if (!Equals(offset, Offset.zero))
        {
            _lastEffectiveTransform = ((Func<Matrix4>)(() =>
{
    var __cascade = Matrix4.translationValues(offset.dx, offset.dy, 0.0);
    __cascade.multiply(_lastEffectiveTransform!);
    return __cascade;
}))();
        }
        engineLayer = builder.pushTransform(_lastEffectiveTransform!.storage, oldLayer: ((TransformEngineLayer?)_engineLayer)!);
        addChildrenToScene(builder);
        builder.pop();
    }

    internal virtual Offset? _transformOffset(Offset localPosition)
    {
        if (_inverseDirty)
        {
            _invertedTransform = Matrix4.tryInvert(PointerEvent.removePerspectiveTransform(transform!));
            _inverseDirty = false;
        }
        if (_invertedTransform is null)
        {
            return null;
        }
        return MatrixUtils.transformPoint(_invertedTransform!, localPosition);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool findAnnotations<S>(AnnotationResult<S> result, Offset localPosition, bool onlyFirst)
    {
        Offset? transformedOffset = _transformOffset(localPosition);
        if (transformedOffset is null)
        {
            return false;
        }
        return base.findAnnotations(result, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(transformedOffset)), onlyFirst: onlyFirst);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void applyTransform(Layer? child, Matrix4 transform)
    {
        DartRuntimePrimitives.Assert(() => child is not null);
        DartRuntimePrimitives.Assert(() => (_lastEffectiveTransform is not null) || (this.transform is not null));
        if (_lastEffectiveTransform is null)
        {
            transform.multiply(this.transform!);
        }
        else
        {
            transform.multiply(_lastEffectiveTransform!);
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new TransformProperty("transform", transform));
    }

}

public class OpacityLayer : OffsetLayer
{
    internal virtual long? _alpha { get; set; } = default;

    public OpacityLayer(long? alpha = null, Offset offset = default) : base(offset: offset)
    {
        _alpha = alpha;
    }

    public virtual long? alpha
    {
        get => _alpha;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value is not null);
            if (__value != _alpha)
            {
                if ((__value == 255L) || (_alpha == 255L))
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
        DartRuntimePrimitives.Assert(() => alpha is not null);
        var enabled = firstChild is not null;
        if (!enabled)
        {
            engineLayer = null;
            return;
        }
        DartRuntimePrimitives.Assert(() =>
            {
                enabled = enabled && !DebugLibrary.debugDisableOpacityLayers;
                return true;
            });
        long realizedAlpha = DartRuntimePrimitives.RequireValue(alpha);
        if (enabled && (realizedAlpha < 255L))
        {
            DartRuntimePrimitives.Assert(() => _engineLayer is null or OpacityEngineLayer);
            engineLayer = builder.pushOpacity(realizedAlpha, offset: offset, oldLayer: ((OpacityEngineLayer?)_engineLayer)!);
        }
        else
        {
            DartRuntimePrimitives.Assert(() => _engineLayer is null or OffsetEngineLayer);
            engineLayer = builder.pushOffset(offset.dx, offset.dy, oldLayer: ((OffsetEngineLayer?)_engineLayer)!);
        }
        addChildrenToScene(builder);
        builder.pop();
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new IntProperty("alpha", alpha));
    }

}

public class ShaderMaskLayer : ContainerLayer
{
    internal virtual Shader? _shader { get; set; } = default;
    internal virtual Rect? _maskRect { get; set; } = default;
    internal virtual BlendMode? _blendMode { get; set; } = default;

    public ShaderMaskLayer(Shader? shader = null, Rect? maskRect = null, BlendMode? blendMode = null)
    {
        _shader = shader;
        _maskRect = maskRect;
        _blendMode = blendMode;
    }

    public virtual Shader? shader
    {
        get => _shader;
        set
        {
            var __value = value is null ? null : value;
            if (!Equals(__value, _shader))
            {
                _shader = __value;
                markNeedsAddToScene();
            }
        }
    }
    public virtual Rect? maskRect
    {
        get => _maskRect;
        set
        {
            var __value = value;
            if (!Equals(__value, _maskRect))
            {
                _maskRect = __value;
                markNeedsAddToScene();
            }
        }
    }
    public virtual BlendMode? blendMode
    {
        get => _blendMode;
        set
        {
            var __value = value;
            if (!Equals(__value, _blendMode))
            {
                _blendMode = __value;
                markNeedsAddToScene();
            }
        }
    }
    public override void addToScene(SceneBuilder builder)
    {
        DartRuntimePrimitives.Assert(() => shader is not null);
        DartRuntimePrimitives.Assert(() => maskRect is not null);
        DartRuntimePrimitives.Assert(() => blendMode is not null);
        engineLayer = builder.pushShaderMask(shader!, DartRuntimePrimitives.RequireValue(maskRect), DartRuntimePrimitives.RequireValue(blendMode), oldLayer: ((ShaderMaskEngineLayer?)_engineLayer)!);
        addChildrenToScene(builder);
        builder.pop();
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Shader>("shader", shader));
        properties.add(new DiagnosticsProperty<Rect>("maskRect", maskRect));
        properties.add(new EnumProperty<BlendMode>("blendMode", blendMode));
    }

}

public class BackdropKey
{
    internal static long _nextKey = 0L;
    internal virtual long _key { get; private set; } = default!;

    public BackdropKey()
    {
        _key = _nextKey++;
    }

}

public class BackdropFilterLayer : ContainerLayer
{
    internal virtual ImageFilter? _filter { get; set; } = default;
    internal virtual BlendMode _blendMode { get; set; } = default!;
    internal virtual BackdropKey? _backdropKey { get; set; } = default;

    public BackdropFilterLayer(ImageFilter? filter = null, BlendMode blendMode = BlendMode.srcOver)
    {
        _filter = filter;
        _blendMode = blendMode;
    }

    public virtual ImageFilter? filter
    {
        get => _filter;
        set
        {
            var __value = value is null ? null : value;
            if (!Equals(__value, _filter))
            {
                _filter = __value;
                markNeedsAddToScene();
            }
        }
    }
    public virtual BlendMode blendMode
    {
        get => _blendMode;
        set
        {
            var __value = value;
            if (!Equals(DartRuntimePrimitives.RequireValue(__value), _blendMode))
            {
                _blendMode = DartRuntimePrimitives.RequireValue(__value);
                markNeedsAddToScene();
            }
        }
    }
    public virtual BackdropKey? backdropKey
    {
        get => _backdropKey;
        set
        {
            var __value = value;
            if (!Equals(__value, _backdropKey))
            {
                _backdropKey = __value;
                markNeedsAddToScene();
            }
        }
    }
    public override void addToScene(SceneBuilder builder)
    {
        DartRuntimePrimitives.Assert(() => filter is not null);
        engineLayer = builder.pushBackdropFilter(filter!, blendMode: blendMode, oldLayer: ((BackdropFilterEngineLayer?)_engineLayer)!, backdropId: _backdropKey?._key);
        addChildrenToScene(builder);
        builder.pop();
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<ImageFilter>("filter", filter));
        properties.add(new EnumProperty<BlendMode>("blendMode", blendMode));
        properties.add(new IntProperty("backdropKey", _backdropKey?._key));
    }

}

public class LayerLink
{
    internal virtual LeaderLayer? _leader { get; set; } = default;
    internal virtual HashSet<LeaderLayer>? _debugPreviousLeaders { get; set; } = default;
    internal virtual bool _debugLeaderCheckScheduled { get; set; } = false;
    public virtual Size? leaderSize { get; set; } = default;

    public virtual LeaderLayer? leader => _leader;
    internal virtual void _registerLeader(LeaderLayer leader)
    {
        DartRuntimePrimitives.Assert(() => !Equals(_leader, leader));
        DartRuntimePrimitives.Assert(() =>
            {
                if (_leader is not null)
                {
                    _debugPreviousLeaders ??= new HashSet<LeaderLayer>();
                    _debugScheduleLeadersCleanUpCheck();
                    return _debugPreviousLeaders!.Add(_leader!);
                }
                return true;
            });
        _leader = leader;
    }

    internal virtual void _unregisterLeader(LeaderLayer leader)
    {
        if (Equals(_leader, leader))
        {
            _leader = null;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => _debugPreviousLeaders!.Remove(leader));
        }
    }

    internal virtual void _debugScheduleLeadersCleanUpCheck()
    {
        DartRuntimePrimitives.Assert(() => _debugPreviousLeaders is not null);
        DartRuntimePrimitives.Assert(() =>
            {
                if (_debugLeaderCheckScheduled)
                {
                    return true;
                }
                _debugLeaderCheckScheduled = true;
                SchedulerBinding.instance.addPostFrameCallback((timeStamp) =>
                {
                    _debugLeaderCheckScheduled = false;
                    DartRuntimePrimitives.Assert(() => checked((long)_debugPreviousLeaders!.Count) == 0);
                }, debugLabel: "LayerLink.leadersCleanUpCheck");
                return true;
            });
    }

    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        return $"{DiagnosticsLibrary.describeIdentity(this)}({((_leader is not null) ? "<linked>" : "<dangling>")})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class LeaderLayer : ContainerLayer
{
    internal virtual LayerLink _link { get; set; } = default!;
    internal virtual Offset _offset { get; set; } = default!;

    public LeaderLayer(LayerLink link, Offset offset = default)
    {
        _link = link;
        _offset = offset;
    }

    public virtual LayerLink link
    {
        get => _link;
        set
        {
            var __value = value;
            if (Equals(_link, __value))
            {
                return;
            }
            if (attached)
            {
                _link._unregisterLeader(this);
                __value._registerLeader(this);
            }
            _link = __value;
        }
    }
    public virtual Offset offset
    {
        get => _offset;
        set
        {
            var __value = value;
            if (Equals(__value, _offset))
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
        _link._registerLeader(this);
    }

    public override void detach()
    {
        _link._unregisterLeader(this);
        base.detach();
    }

    public override bool findAnnotations<S>(AnnotationResult<S> result, Offset localPosition, bool onlyFirst)
    {
        return base.findAnnotations(result, localPosition - offset, onlyFirst: onlyFirst);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void addToScene(SceneBuilder builder)
    {
        if (!Equals(offset, Offset.zero))
        {
            engineLayer = builder.pushTransform(Matrix4.translationValues(offset.dx, offset.dy, 0.0).storage, oldLayer: ((TransformEngineLayer?)_engineLayer)!);
        }
        else
        {
            engineLayer = null;
        }
        addChildrenToScene(builder);
        if (!Equals(offset, Offset.zero))
        {
            builder.pop();
        }
    }

    public override void applyTransform(Layer? child, Matrix4 transform)
    {
        if (!Equals(offset, Offset.zero))
        {
            transform.translateByDouble(offset.dx, offset.dy, 0, 1);
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Offset>("offset", offset));
        properties.add(new DiagnosticsProperty<LayerLink>("link", link));
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

    internal virtual Offset? _transformOffset(Offset localPosition)
    {
        if (_inverseDirty)
        {
            _invertedTransform = Matrix4.tryInvert(getLastTransform()!);
            _inverseDirty = false;
        }
        if (_invertedTransform is null)
        {
            return null;
        }
        var vector = new System.Numerics.Vector4(checked((float)localPosition.dx), checked((float)localPosition.dy), checked((float)0.0), checked((float)1.0));
        System.Numerics.Vector4 result = _invertedTransform!.transform(vector);
        return new Offset(result[(int)0L] - DartRuntimePrimitives.RequireValue(linkedOffset).dx, result[(int)1L] - DartRuntimePrimitives.RequireValue(linkedOffset).dy);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool findAnnotations<S>(AnnotationResult<S> result, Offset localPosition, bool onlyFirst)
    {
        if (link.leader is null)
        {
            if (DartRuntimePrimitives.RequireValue(showWhenUnlinked))
            {
                return base.findAnnotations(result, localPosition - DartRuntimePrimitives.RequireValue(unlinkedOffset), onlyFirst: onlyFirst);
            }
            return false;
        }
        Offset? transformedOffset = _transformOffset(localPosition);
        if (transformedOffset is null)
        {
            return false;
        }
        return base.findAnnotations(result, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(transformedOffset)), onlyFirst: onlyFirst);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Matrix4? getLastTransform()
    {
        if (_lastTransform is null)
        {
            return null;
        }
        var result = Matrix4.translationValues(-DartRuntimePrimitives.RequireValue(_lastOffset).dx, -DartRuntimePrimitives.RequireValue(_lastOffset).dy, 0.0);
        result.multiply(_lastTransform!);
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static Matrix4 _collectTransformForLayerChain(List<ContainerLayer?> layers)
    {
        var result = Matrix4.identity();
        for (long index = checked(layers.Count) - 1L; index > 0L; index -= 1L)
        {
            layers[(int)index]?.applyTransform(layers[(int)(index - 1L)], result);
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static Layer? _pathsToCommonAncestor(Layer? a, Layer? b, List<ContainerLayer?> ancestorsA, List<ContainerLayer?> ancestorsB)
    {
        if ((a is null) || (b is null))
        {
            return null;
        }
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if (a.depth < b.depth)
        {
            ancestorsB.Add(b.parent);
            return _pathsToCommonAncestor(a, b.parent, ancestorsA, ancestorsB);
        }
        else
        {
            if (a.depth > b.depth)
            {
                ancestorsA.Add(a.parent);
                return _pathsToCommonAncestor(a.parent, b, ancestorsA, ancestorsB);
            }
        }
        ancestorsA.Add(a.parent);
        ancestorsB.Add(b.parent);
        return _pathsToCommonAncestor(a.parent, b.parent, ancestorsA, ancestorsB);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _debugCheckLeaderBeforeFollower(List<ContainerLayer?> leaderToCommonAncestor, List<ContainerLayer?> followerToCommonAncestor)
    {
        if (checked(followerToCommonAncestor.Count) <= 1L)
        {
            return false;
        }
        if (checked(leaderToCommonAncestor.Count) <= 1L)
        {
            return true;
        }
        ContainerLayer? leaderSubtreeBelowAncestor = leaderToCommonAncestor[(int)(checked(leaderToCommonAncestor.Count) - 2L)];
        ContainerLayer? followerSubtreeBelowAncestor = followerToCommonAncestor[(int)(checked(followerToCommonAncestor.Count) - 2L)];
        Layer? sibling = leaderSubtreeBelowAncestor;
        while (sibling is not null)
        {
            if (Equals(sibling, followerSubtreeBelowAncestor))
            {
                return true;
            }
            sibling = sibling.nextSibling;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _establishTransform()
    {
        _lastTransform = null;
        LeaderLayer? leaderLocal = link.leader;
        if (leaderLocal is null)
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => Equals(leaderLocal.owner, owner));
        var forwardLayers = new List<ContainerLayer?> { leaderLocal };
        var inverseLayers = new List<ContainerLayer?> { this };
        Layer? ancestor = _pathsToCommonAncestor(leaderLocal, this, forwardLayers, inverseLayers);
        DartRuntimePrimitives.Assert(() => ancestor is not null);
        DartRuntimePrimitives.Assert(() => _debugCheckLeaderBeforeFollower(forwardLayers, inverseLayers));
        Matrix4 forwardTransform = _collectTransformForLayerChain(forwardLayers);
        leaderLocal.applyTransform(null, forwardTransform);
        forwardTransform.translateByDouble(DartRuntimePrimitives.RequireValue(linkedOffset).dx, DartRuntimePrimitives.RequireValue(linkedOffset).dy, 0, 1);
        Matrix4 inverseTransform = _collectTransformForLayerChain(inverseLayers);
        if (inverseTransform.invert() == 0.0)
        {
            return;
        }
        inverseTransform.multiply(forwardTransform);
        _lastTransform = inverseTransform;
        _inverseDirty = true;
    }

    public override bool alwaysNeedsAddToScene => true;
    public override void addToScene(SceneBuilder builder)
    {
        DartRuntimePrimitives.Assert(() => showWhenUnlinked is not null);
        if ((link.leader is null) && !DartRuntimePrimitives.RequireValue(showWhenUnlinked))
        {
            _lastTransform = null;
            _lastOffset = null;
            _inverseDirty = true;
            engineLayer = null;
            return;
        }
        _establishTransform();
        if (_lastTransform is not null)
        {
            _lastOffset = unlinkedOffset;
            engineLayer = builder.pushTransform(_lastTransform!.storage, oldLayer: ((TransformEngineLayer?)_engineLayer)!);
            addChildrenToScene(builder);
            builder.pop();
        }
        else
        {
            _lastOffset = null;
            var matrix = Matrix4.translationValues(DartRuntimePrimitives.RequireValue(unlinkedOffset).dx, DartRuntimePrimitives.RequireValue(unlinkedOffset).dy, 0.0);
            engineLayer = builder.pushTransform(matrix.storage, oldLayer: ((TransformEngineLayer?)_engineLayer)!);
            addChildrenToScene(builder);
            builder.pop();
        }
        _inverseDirty = true;
    }

    public override void applyTransform(Layer? child, Matrix4 transform)
    {
        DartRuntimePrimitives.Assert(() => child is not null);
        if (_lastTransform is not null)
        {
            transform.multiply(_lastTransform!);
        }
        else
        {
            transform.multiply(Matrix4.translationValues(DartRuntimePrimitives.RequireValue(unlinkedOffset).dx, DartRuntimePrimitives.RequireValue(unlinkedOffset).dy, 0));
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<LayerLink>("link", link));
        properties.add(new TransformProperty("transform", getLastTransform(), defaultValue: null));
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
        this.offset = offset ?? Offset.zero;
    }

    public override bool findAnnotations<S>(AnnotationResult<S> result, Offset localPosition, bool onlyFirst)
    {
        bool isAbsorbed = base.findAnnotations(result, localPosition, onlyFirst: onlyFirst);
        if ((result.entries.Count() != 0) && onlyFirst)
        {
            return isAbsorbed;
        }
        if ((size is not null) && !(offset & DartRuntimePrimitives.RequireValue(size)).contains(localPosition))
        {
            Size size__value103949 = DartRuntimePrimitives.RequireValue(size);
            return isAbsorbed;
        }
        if (Equals(typeof(T), typeof(S)))
        {
            isAbsorbed = isAbsorbed || opaque;
            object? untypedValue = value;
            var typedValue = ((S?)untypedValue)!;
            result.add(new AnnotationEntry<S>(annotation: typedValue, localPosition: localPosition - offset));
        }
        return isAbsorbed;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<T>("value", value));
        properties.add(new DiagnosticsProperty<Size>("size", size, defaultValue: null));
        properties.add(new DiagnosticsProperty<Offset>("offset", offset, defaultValue: null));
        properties.add(new DiagnosticsProperty<bool>("opaque", opaque, defaultValue: false));
    }

}
