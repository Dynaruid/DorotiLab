// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/platform_view.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

public enum PlatformViewHitTestBehavior
{
    opaque,
    translucent,
    transparent,
}

internal enum _PlatformViewState__platform_view
{
    uninitialized,
    resizing,
    ready,
}

public static partial class Platform_viewLibrary
{
    internal static bool _factoryTypesSetEquals<T>(HashSet<Factory<T>>? a, HashSet<Factory<T>>? b)
    {
        if (Equals(a, b))
        {
            return true;
        }
        if ((a is null) || (b is null))
        {
            return false;
        }
        return CollectionsLibrary.setEquals(_factoriesTypeSet(a), _factoriesTypeSet(b));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public static partial class Platform_viewLibrary
{
    internal static HashSet<Type> _factoriesTypeSet<T>(HashSet<Factory<T>> factories)
    {
        return factories.map((factory) => factory.type).toSet();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class RenderAndroidView : PlatformViewRenderBox
{
    internal virtual _PlatformViewState__platform_view _state { get; set; } =
        _PlatformViewState__platform_view.uninitialized;
    internal virtual Size? _currentTextureSize { get; set; } = default;
    internal virtual bool _isDisposed { get; set; } = false;
    internal virtual AndroidViewController _viewController { get; set; } = default!;
    internal virtual Clip _clipBehavior { get; set; } = Clip.hardEdge;
    internal virtual LayerHandle<ClipRectLayer> _clipRectLayer { get; private set; } =
        new LayerHandle<ClipRectLayer>();

    public RenderAndroidView(
        AndroidViewController viewController,
        PlatformViewHitTestBehavior hitTestBehavior,
        HashSet<Factory<OneSequenceGestureRecognizer>> gestureRecognizers,
        Clip clipBehavior = Clip.hardEdge
    )
        : base(
            controller: viewController,
            hitTestBehavior: hitTestBehavior,
            gestureRecognizers: gestureRecognizers
        )
    {
        _viewController = viewController;
        _clipBehavior = clipBehavior;
    }

    public override PlatformViewController controller
    {
        get => _viewController;
        set
        {
            var controller = (AndroidViewController)(object)value;
            DartRuntimePrimitives.Assert(() => !_isDisposed);
            if (Equals(_viewController, controller))
            {
                return;
            }
            _viewController.removeOnPlatformViewCreatedListener(_onPlatformViewCreated);
            base.controller = controller;
            _viewController = controller;
            _viewController.pointTransformer = (offset) => globalToLocal(offset);
            _ = _sizePlatformView();
            if (_viewController.isCreated)
            {
                markNeedsSemanticsUpdate();
            }
            _viewController.addOnPlatformViewCreatedListener(_onPlatformViewCreated);
        }
    }
    public virtual Clip clipBehavior
    {
        get => _clipBehavior;
        set
        {
            var __value = value;
            if (!Equals(__value, _clipBehavior))
            {
                _clipBehavior = __value;
                markNeedsPaint();
                markNeedsSemanticsUpdate();
            }
        }
    }

    internal virtual void _onPlatformViewCreated(long id)
    {
        DartRuntimePrimitives.Assert(() => !_isDisposed);
        markNeedsSemanticsUpdate();
    }

    public override bool sizedByParent => true;
    public override bool alwaysNeedsCompositing => true;
    public override bool isRepaintBoundary => true;

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return constraints.biggest;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void performResize()
    {
        base.performResize();
        _ = _sizePlatformView();
    }

    internal virtual async Future _sizePlatformView()
    {
        if (Equals(_state, _PlatformViewState__platform_view.resizing) || size.isEmpty)
        {
            return;
        }
        _state = _PlatformViewState__platform_view.resizing;
        markNeedsPaint();
        Size targetSize = default!;
        do
        {
            targetSize = size;
            _currentTextureSize = await _viewController.setSize(targetSize);
            if (_isDisposed)
            {
                return;
            }
        } while (!Equals(size, targetSize));
        _state = _PlatformViewState__platform_view.ready;
        markNeedsPaint();
    }

    internal virtual void _setOffset()
    {
        SchedulerBinding.instance.addPostFrameCallback(
            async (_) =>
            {
                if (!_isDisposed)
                {
                    if (attached)
                    {
                        await _viewController.setOffset(localToGlobal(Offset.zero));
                    }
                    _setOffset();
                }
            },
            debugLabel: "RenderAndroidView.setOffset"
        );
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (
            ((_viewController.textureId is null) && !_viewController.requiresViewComposition)
            || (_currentTextureSize is null)
        )
        {
            return;
        }
        bool isTextureLargerThanWidget =
            (
                (
                    _currentTextureSize
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ).width > size.width
            )
            || (
                (
                    _currentTextureSize
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ).height > size.height
            );
        if (isTextureLargerThanWidget && (!Equals(clipBehavior, Clip.none)))
        {
            _clipRectLayer.layer = context.pushClipRect(
                true,
                offset,
                offset & size,
                _paintTexture,
                clipBehavior: clipBehavior,
                oldLayer: _clipRectLayer.layer
            );
            return;
        }
        _clipRectLayer.layer = null;
        _paintTexture(context, offset);
    }

    public override void dispose()
    {
        _isDisposed = true;
        _clipRectLayer.layer = null;
        _viewController.removeOnPlatformViewCreatedListener(_onPlatformViewCreated);
        base.dispose();
    }

    internal virtual void _paintTexture(PaintingContext context, Offset offset)
    {
        if (_currentTextureSize is null)
        {
            return;
        }
        if (_viewController.requiresViewComposition)
        {
            context.addLayer(
                new PlatformViewLayer(
                    rect: offset
                        & (
                            _currentTextureSize
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        ),
                    viewId: _viewController.viewId
                )
            );
            return;
        }
        context.addLayer(
            new TextureLayer(
                rect: offset
                    & (
                        _currentTextureSize
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ),
                textureId: (
                    _viewController.textureId
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
            )
        );
    }

    public override void describeSemanticsConfiguration(SemanticsConfiguration config)
    {
        config.isSemanticBoundary = true;
        if (_viewController.isCreated)
        {
            config.platformViewId = _viewController.viewId;
            config.hitTestBehavior = Dart_uiLibrary.SemanticsHitTestBehavior.transparent;
        }
    }
}

public abstract class RenderDarwinPlatformView<T> : RenderBox
    where T : DarwinPlatformViewController
{
    internal virtual T _viewController { get; set; } = default!;
    public virtual PlatformViewHitTestBehavior hitTestBehavior { get; set; } = default!;
    internal virtual PointerEvent? _lastPointerDownEvent { get; set; } = default;
    internal virtual _UiKitViewGestureRecognizer__platform_view? _gestureRecognizer { get; set; } =
        default;

    protected RenderDarwinPlatformView(
        T viewController,
        PlatformViewHitTestBehavior hitTestBehavior,
        HashSet<Factory<OneSequenceGestureRecognizer>> gestureRecognizers
    )
    {
        this.hitTestBehavior = hitTestBehavior;
        _viewController = viewController;
    }

    public virtual T viewController
    {
        get => _viewController;
        set
        {
            var __value = value;
            if (EqualityComparer<T>.Default.Equals(_viewController, __value))
            {
                return;
            }
            var needsSemanticsUpdate = _viewController.id != __value.id;
            _viewController = __value;
            markNeedsPaint();
            if (needsSemanticsUpdate)
            {
                markNeedsSemanticsUpdate();
            }
        }
    }
    public override bool sizedByParent => true;
    public override bool alwaysNeedsCompositing => true;
    public override bool isRepaintBoundary => true;

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return constraints.biggest;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        context.addLayer(new PlatformViewLayer(rect: offset & size, viewId: _viewController.id));
    }

    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        if (
            Equals(hitTestBehavior, PlatformViewHitTestBehavior.transparent)
            || !size.contains((position))
        )
        {
            return false;
        }
        result.add(new BoxHitTestEntry(this, (position)));
        return Equals(hitTestBehavior, PlatformViewHitTestBehavior.opaque);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool hitTestSelf(Offset position) =>
        !Equals(hitTestBehavior, PlatformViewHitTestBehavior.transparent);

    internal virtual void _handleGlobalPointerEvent(PointerEvent @event)
    {
        if (!hasSize)
        {
            return;
        }
        if (@event is not Gestures.PointerDownEvent)
        {
            return;
        }
        if (!(Offset.zero & size).contains(globalToLocal((@event.position))))
        {
            return;
        }
        if (!Equals(@event.original ?? @event, _lastPointerDownEvent))
        {
            _ = _viewController.rejectGesture();
        }
        _lastPointerDownEvent = null;
    }

    public override void describeSemanticsConfiguration(SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.isSemanticBoundary = true;
        config.platformViewId = _viewController.id;
        config.hitTestBehavior = Dart_uiLibrary.SemanticsHitTestBehavior.transparent;
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        GestureBinding.instance.pointerRouter.addGlobalRoute(_handleGlobalPointerEvent);
    }

    public override void detach()
    {
        GestureBinding.instance.pointerRouter.removeGlobalRoute(_handleGlobalPointerEvent);
        base.detach();
    }

    public abstract void updateGestureRecognizers(
        HashSet<Factory<OneSequenceGestureRecognizer>> gestureRecognizers
    );
}

public class RenderUiKitView : RenderDarwinPlatformView<UiKitViewController>
{
    public RenderUiKitView(
        UiKitViewController viewController,
        PlatformViewHitTestBehavior hitTestBehavior,
        HashSet<Factory<OneSequenceGestureRecognizer>> gestureRecognizers
    )
        : base(
            viewController: viewController,
            hitTestBehavior: hitTestBehavior,
            gestureRecognizers: gestureRecognizers
        ) { }

    public override void updateGestureRecognizers(
        HashSet<Factory<OneSequenceGestureRecognizer>> gestureRecognizers
    )
    {
        DartRuntimePrimitives.Assert(() =>
            checked(Platform_viewLibrary._factoriesTypeSet(gestureRecognizers).Count)
            == checked((long)gestureRecognizers.Count)
        );
        if (
            Platform_viewLibrary._factoryTypesSetEquals(
                gestureRecognizers,
                _gestureRecognizer?.gestureRecognizerFactories
            )
        )
        {
            return;
        }
        _gestureRecognizer?.dispose();
        _gestureRecognizer = new _UiKitViewGestureRecognizer__platform_view(
            viewController,
            gestureRecognizers
        );
    }

    public override void handleEvent(PointerEvent @event, HitTestEntry<HitTestTarget> entry)
    {
        if (@event is not Gestures.PointerDownEvent)
        {
            return;
        }
        _gestureRecognizer!.addPointer((Gestures.PointerDownEvent)(object)@event);
        _lastPointerDownEvent = @event.original ?? @event;
    }

    public override void detach()
    {
        _gestureRecognizer!.reset();
        base.detach();
    }

    public override void dispose()
    {
        _gestureRecognizer?.dispose();
        base.dispose();
    }
}

public class RenderAppKitView : RenderDarwinPlatformView<AppKitViewController>
{
    public RenderAppKitView(
        AppKitViewController viewController,
        PlatformViewHitTestBehavior hitTestBehavior,
        HashSet<Factory<OneSequenceGestureRecognizer>> gestureRecognizers
    )
        : base(
            viewController: viewController,
            hitTestBehavior: hitTestBehavior,
            gestureRecognizers: gestureRecognizers
        ) { }

    public override void updateGestureRecognizers(
        HashSet<Factory<OneSequenceGestureRecognizer>> gestureRecognizers
    ) { }
}

internal class _UiKitViewGestureRecognizer__platform_view : OneSequenceGestureRecognizer
{
    public virtual HashSet<Factory<OneSequenceGestureRecognizer>> gestureRecognizerFactories
    {
        get;
        private set;
    } = default!;
    internal virtual HashSet<OneSequenceGestureRecognizer> _gestureRecognizers { get; set; } =
        default!;
    public virtual UiKitViewController controller { get; private set; } = default!;

    internal _UiKitViewGestureRecognizer__platform_view(
        UiKitViewController controller,
        HashSet<Factory<OneSequenceGestureRecognizer>> gestureRecognizerFactories
    )
    {
        this.controller = controller;
        this.gestureRecognizerFactories = gestureRecognizerFactories;
    }

    public override void addAllowedPointer(Gestures.PointerDownEvent @event)
    {
        base.addAllowedPointer(@event);
        foreach (OneSequenceGestureRecognizer recognizer in _gestureRecognizers)
        {
            recognizer.addPointer((Gestures.PointerDownEvent)(object)@event);
        }
    }

    public override string debugDescription => "UIKit view";

    public override void didStopTrackingLastPointer(long pointer) { }

    public override void handleEvent(PointerEvent @event)
    {
        stopTrackingIfPointerNoLongerDown(@event);
    }

    public override void acceptGesture(long pointer)
    {
        _ = controller.acceptGesture();
    }

    public override void rejectGesture(long pointer)
    {
        _ = controller.rejectGesture();
    }

    public virtual void reset()
    {
        resolve(GestureDisposition.rejected);
    }
}

internal delegate Future _HandlePointerEvent__platform_view(PointerEvent @event);

public class _PlatformViewGestureRecognizer__platform_view : OneSequenceGestureRecognizer
{
    internal virtual Func<PointerEvent, Future> _handlePointerEvent { get; set; } = default!;
    public virtual DartMap<long, List<PointerEvent>> cachedEvents { get; private set; } =
        new DartMap<long, List<PointerEvent>>();
    public virtual HashSet<long> forwardedPointers { get; private set; } = new HashSet<long>();
    public virtual HashSet<Factory<OneSequenceGestureRecognizer>> gestureRecognizerFactories
    {
        get;
        private set;
    } = default!;
    internal virtual HashSet<OneSequenceGestureRecognizer> _gestureRecognizers { get; set; } =
        default!;

    internal _PlatformViewGestureRecognizer__platform_view(
        Func<PointerEvent, Future> handlePointerEvent,
        HashSet<Factory<OneSequenceGestureRecognizer>> gestureRecognizerFactories
    )
    {
        this.gestureRecognizerFactories = gestureRecognizerFactories;
    }

    public override void addAllowedPointer(Gestures.PointerDownEvent @event)
    {
        base.addAllowedPointer(@event);
        foreach (OneSequenceGestureRecognizer recognizer in _gestureRecognizers)
        {
            recognizer.addPointer((Gestures.PointerDownEvent)(object)@event);
        }
    }

    public override string debugDescription => "Platform view";

    public override void didStopTrackingLastPointer(long pointer) { }

    public override void handleEvent(PointerEvent @event)
    {
        if (!forwardedPointers.Contains(@event.pointer))
        {
            _cacheEvent(@event);
        }
        else
        {
            _ = _handlePointerEvent(@event);
        }
        stopTrackingIfPointerNoLongerDown(@event);
    }

    public override void acceptGesture(long pointer)
    {
        _flushPointerCache(pointer);
        forwardedPointers.Add(pointer);
    }

    public override void rejectGesture(long pointer)
    {
        stopTrackingPointer(pointer);
        cachedEvents.remove(pointer);
    }

    internal virtual void _cacheEvent(PointerEvent @event)
    {
        if (!cachedEvents.ContainsKey(@event.pointer))
        {
            cachedEvents[@event.pointer] = new List<PointerEvent>();
        }
        cachedEvents.GetValueOrDefault(@event.pointer)!.Add(@event);
    }

    internal virtual void _flushPointerCache(long pointer)
    {
        cachedEvents
            .remove(pointer)
            ?.forEach(
                (__item) =>
                {
                    _ = _handlePointerEvent(__item);
                }
            );
    }

    public override void stopTrackingPointer(long pointer)
    {
        base.stopTrackingPointer(pointer);
        forwardedPointers.Remove(pointer);
    }

    public virtual void reset()
    {
        forwardedPointers.forEach(base.stopTrackingPointer);
        forwardedPointers.Clear();
        cachedEvents.Keys.forEach(base.stopTrackingPointer);
        cachedEvents.Clear();
        resolve(GestureDisposition.rejected);
    }
}

public class PlatformViewRenderBox : RenderBox, _PlatformViewGestureMixin__platform_view
{
    internal virtual PlatformViewController _controller { get; set; } = default!;
    public virtual PlatformViewHitTestBehavior? _hitTestBehavior { get; set; } = default;
    public virtual Func<PointerEvent, Future>? _handlePointerEvent { get; set; } = default;
    public virtual _PlatformViewGestureRecognizer__platform_view? _gestureRecognizer { get; set; } =
        default;

    public PlatformViewRenderBox(
        PlatformViewController controller,
        PlatformViewHitTestBehavior hitTestBehavior,
        HashSet<Factory<OneSequenceGestureRecognizer>> gestureRecognizers
    )
    {
        _controller = controller;
        System.Diagnostics.Debug.Assert(controller.viewId > -1L);
    }

    public virtual PlatformViewController controller
    {
        get => _controller;
        set
        {
            var controller = value;
            DartRuntimePrimitives.Assert(() => controller.viewId > -1L);
            if (Equals(_controller, controller))
            {
                return;
            }
            var needsSemanticsUpdate = _controller.viewId != controller.viewId;
            _controller = controller;
            markNeedsPaint();
            if (needsSemanticsUpdate)
            {
                markNeedsSemanticsUpdate();
            }
        }
    }

    public virtual void updateGestureRecognizers(
        HashSet<Factory<OneSequenceGestureRecognizer>> gestureRecognizers
    )
    {
        _updateGestureRecognizersWithCallBack(
            gestureRecognizers,
            (__event) =>
                _controller.dispatchPointerEvent(
                    Ui.PointerEvent.FromFrameworkEvent(
                        __event is Gestures.PointerDownEvent ? 1L
                            : __event is Gestures.PointerUpEvent ? 2L
                            : __event is Gestures.PointerCancelEvent ? 3L
                            : __event is Gestures.PointerHoverEvent ? 4L
                            : __event is Gestures.PointerMoveEvent ? 5L
                            : 0L,
                        __event.pointer,
                        __event.embedderId,
                        __event.platformData,
                        __event.timeStamp,
                        __event.position,
                        __event.kind,
                        __event.orientation,
                        __event.pressure,
                        __event.size,
                        __event.radiusMajor,
                        __event.radiusMinor
                    )
                )
        );
    }

    public override bool sizedByParent => true;
    public override bool alwaysNeedsCompositing => true;
    public override bool isRepaintBoundary => true;

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return constraints.biggest;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        context.addLayer(new PlatformViewLayer(rect: offset & size, viewId: _controller.viewId));
    }

    public override void describeSemanticsConfiguration(SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.isSemanticBoundary = true;
        config.platformViewId = _controller.viewId;
        config.hitTestBehavior = Dart_uiLibrary.SemanticsHitTestBehavior.transparent;
    }

    public virtual PlatformViewHitTestBehavior hitTestBehavior
    {
        set
        {
            var __value = value;
            if (!Equals(__value, _hitTestBehavior))
            {
                _hitTestBehavior = __value;
                if (owner is not null)
                {
                    markNeedsPaint();
                }
            }
        }
    }

    public virtual void _updateGestureRecognizersWithCallBack(
        HashSet<Factory<OneSequenceGestureRecognizer>> gestureRecognizers,
        Func<PointerEvent, Future> handlePointerEvent
    )
    {
        DartRuntimePrimitives.Assert(() =>
            checked(Platform_viewLibrary._factoriesTypeSet(gestureRecognizers).Count)
            == checked((long)gestureRecognizers.Count)
        );
        if (
            Platform_viewLibrary._factoryTypesSetEquals(
                gestureRecognizers,
                _gestureRecognizer?.gestureRecognizerFactories
            )
        )
        {
            return;
        }
        _gestureRecognizer?.dispose();
        _gestureRecognizer = new _PlatformViewGestureRecognizer__platform_view(
            handlePointerEvent,
            gestureRecognizers
        );
        _ = _handlePointerEvent = handlePointerEvent;
    }

    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        if (
            Equals(_hitTestBehavior, PlatformViewHitTestBehavior.transparent)
            || !size.contains(position)
        )
        {
            return false;
        }
        result.add(new BoxHitTestEntry(this, position));
        return Equals(_hitTestBehavior, PlatformViewHitTestBehavior.opaque);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool hitTestSelf(Offset position) =>
        !Equals(_hitTestBehavior, PlatformViewHitTestBehavior.transparent);

    public virtual Action<Gestures.PointerEnterEvent>? onEnter => null;
    public virtual Action<Gestures.PointerExitEvent>? onExit => null;
    public virtual MouseCursor cursor =>
        Foundation.ConstantsLibrary.kIsWeb ? MouseCursor.defer : MouseCursor.uncontrolled;
    public virtual bool validForMouseTracker => true;

    public override void handleEvent(PointerEvent @event, HitTestEntry<HitTestTarget> entry)
    {
        if (@event is Gestures.PointerDownEvent)
        {
            _gestureRecognizer!.addPointer((Gestures.PointerDownEvent)(object)@event);
        }
        if (@event is Gestures.PointerHoverEvent)
        {
            _ = _handlePointerEvent?.Invoke(@event);
        }
    }

    public override void detach()
    {
        _gestureRecognizer!.reset();
        base.detach();
    }

    public override void dispose()
    {
        _gestureRecognizer?.dispose();
        base.dispose();
    }
}

public interface _PlatformViewGestureMixin__platform_view
{
    PlatformViewHitTestBehavior? _hitTestBehavior { get; set; }
    Func<PointerEvent, Future>? _handlePointerEvent { get; set; }
    _PlatformViewGestureRecognizer__platform_view? _gestureRecognizer { get; set; }

    public PlatformViewHitTestBehavior hitTestBehavior { set; }
    public void _updateGestureRecognizersWithCallBack(
        HashSet<Factory<OneSequenceGestureRecognizer>> gestureRecognizers,
        Func<PointerEvent, Future> handlePointerEvent
    );
    public bool hitTest(BoxHitTestResult result, Offset position);
    public bool hitTestSelf(Offset position);
    public Action<Gestures.PointerEnterEvent>? onEnter { get; }
    public Action<Gestures.PointerExitEvent>? onExit { get; }
    public MouseCursor cursor { get; }
    public bool validForMouseTracker { get; }
    public void handleEvent(PointerEvent @event, HitTestEntry<HitTestTarget> entry);
    public void detach();
    public void dispose();
}
