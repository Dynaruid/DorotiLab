// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/platform_view.dart
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

public enum PlatformViewHitTestBehavior
{
    opaque,
    translucent,
    transparent
}

internal enum _PlatformViewState__platform_view
{
    uninitialized,
    resizing,
    ready
}

public static partial class Platform_viewLibrary
{
    internal static bool _factoryTypesSetEquals<T>(HashSet<Factory<T>>? a, HashSet<Factory<T>>? b)
    {
        if ((object.Equals(a, b)))
        {
            return true;
        }
        if (((a is null) || (b is null)))
        {
            return false;
        }
        return global::Doroti.Framework.Foundation.CollectionsLibrary.setEquals(Platform_viewLibrary._factoriesTypeSet<T>(a), Platform_viewLibrary._factoriesTypeSet<T>(b));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Platform_viewLibrary
{
    internal static HashSet<Type> _factoriesTypeSet<T>(HashSet<Factory<T>> factories)
    {
        return factories.map<Factory<T>, Type>(((factory) => factory.type)).toSet();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class RenderAndroidView : PlatformViewRenderBox
{
    internal virtual _PlatformViewState__platform_view _state { get; set; } = _PlatformViewState__platform_view.uninitialized;
    internal virtual Size? _currentTextureSize { get; set; } = default;
    internal virtual bool _isDisposed { get; set; } = false;
    internal virtual AndroidViewController _viewController { get; set; } = default!;
    internal virtual Clip _clipBehavior { get; set; } = Clip.hardEdge;
    internal virtual LayerHandle<ClipRectLayer> _clipRectLayer { get; private set; } = new LayerHandle<ClipRectLayer>();

    public RenderAndroidView(AndroidViewController viewController, PlatformViewHitTestBehavior hitTestBehavior, HashSet<Factory<OneSequenceGestureRecognizer>> gestureRecognizers, Clip clipBehavior = Clip.hardEdge) : base(controller: viewController, hitTestBehavior: hitTestBehavior, gestureRecognizers: gestureRecognizers)
    {
        this._viewController = viewController;
        this._clipBehavior = clipBehavior;
    }

    public override PlatformViewController controller
    {
        get => this._viewController;
        set
        {
            var controller = (AndroidViewController)(object)value;
            DartRuntimePrimitives.Assert(() => !this._isDisposed);
            if ((object.Equals(this._viewController, controller)))
            {
                return;
            }
            this._viewController.removeOnPlatformViewCreatedListener(this._onPlatformViewCreated);
            base.controller = controller;
            _viewController = controller;
            this._viewController.pointTransformer = ((offset) => globalToLocal(offset));
            _ = _sizePlatformView();
            if (this._viewController.isCreated)
            {
                markNeedsSemanticsUpdate();
            }
            this._viewController.addOnPlatformViewCreatedListener(this._onPlatformViewCreated);
        }
    }
    public virtual global::Doroti.Ui.Clip clipBehavior
    {
        get => this._clipBehavior;
        set
        {
            var __value = value;
            if ((!object.Equals(__value, this._clipBehavior)))
            {
                _clipBehavior = __value;
                markNeedsPaint();
                markNeedsSemanticsUpdate();
            }
        }
    }
    internal virtual void _onPlatformViewCreated(long id)
    {
        DartRuntimePrimitives.Assert(() => !this._isDisposed);
        markNeedsSemanticsUpdate();
    }

    public override bool sizedByParent => true;
    public override bool alwaysNeedsCompositing => true;
    public override bool isRepaintBoundary => true;
    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return ((BoxConstraints)constraints).biggest;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performResize()
    {
        base.performResize();
        _ = _sizePlatformView();
    }

    internal async virtual Future _sizePlatformView()
    {
        if (((object.Equals(this._state, _PlatformViewState__platform_view.resizing)) || size.isEmpty))
        {
            return;
        }
        _state = _PlatformViewState__platform_view.resizing;
        markNeedsPaint();
        global::Doroti.Ui.Size targetSize = default!;
        do
        {
            targetSize = size;
            _currentTextureSize = await this._viewController.setSize(targetSize);
            if (this._isDisposed)
            {
                return;
            }
        }
        while ((!object.Equals(size, targetSize)));
        _state = _PlatformViewState__platform_view.ready;
        markNeedsPaint();
    }

    internal virtual void _setOffset()
    {
        SchedulerBinding.instance.addPostFrameCallback((async (_) =>
        {
            if (!this._isDisposed)
            {
                if (attached)
                {
                    await this._viewController.setOffset(localToGlobal(Offset.zero));
                }
                _setOffset();
            }
        }), debugLabel: "RenderAndroidView.setOffset");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (((((this._viewController.textureId is null) && !this._viewController.requiresViewComposition)) || (this._currentTextureSize is null)))
        {
            return;
        }
        bool isTextureLargerThanWidget = ((DartRuntimePrimitives.RequireValue(this._currentTextureSize).width > size.width) || (DartRuntimePrimitives.RequireValue(this._currentTextureSize).height > size.height));
        if ((isTextureLargerThanWidget && (!object.Equals(this.clipBehavior, Clip.none))))
        {
            this._clipRectLayer.layer = context.pushClipRect(true, offset, (offset & size), (Action<PaintingContext, Offset>)this._paintTexture, clipBehavior: this.clipBehavior, oldLayer: ((LayerHandle<ClipRectLayer>)this._clipRectLayer).layer);
            return;
        }
        this._clipRectLayer.layer = null;
        _paintTexture(context, offset);
    }

    public override void dispose()
    {
        _isDisposed = true;
        this._clipRectLayer.layer = null;
        this._viewController.removeOnPlatformViewCreatedListener(this._onPlatformViewCreated);
        base.dispose();
    }

    internal virtual void _paintTexture(PaintingContext context, Offset offset)
    {
        if ((this._currentTextureSize is null))
        {
            return;
        }
        if (this._viewController.requiresViewComposition)
        {
            context.addLayer(new PlatformViewLayer(rect: (offset & DartRuntimePrimitives.RequireValue(this._currentTextureSize)), viewId: this._viewController.viewId));
            return;
        }
        context.addLayer(new TextureLayer(rect: (offset & DartRuntimePrimitives.RequireValue(this._currentTextureSize)), textureId: DartRuntimePrimitives.RequireValue(this._viewController.textureId)));
    }

    public override void describeSemanticsConfiguration(global::Doroti.Framework.Semantics.SemanticsConfiguration config)
    {
        config.isSemanticBoundary = true;
        if (this._viewController.isCreated)
        {
            config.platformViewId = this._viewController.viewId;
            config.hitTestBehavior = Dart_uiLibrary.SemanticsHitTestBehavior.transparent;
        }
    }

}

public abstract class RenderDarwinPlatformView<T> : RenderBox where T : DarwinPlatformViewController
{
    internal virtual T _viewController { get; set; } = default!;
    public virtual PlatformViewHitTestBehavior hitTestBehavior { get; set; } = default!;
    internal virtual global::Doroti.Framework.Gestures.PointerEvent? _lastPointerDownEvent { get; set; } = default;
    internal virtual _UiKitViewGestureRecognizer__platform_view? _gestureRecognizer { get; set; } = default;

    protected RenderDarwinPlatformView(T viewController, PlatformViewHitTestBehavior hitTestBehavior, HashSet<Factory<OneSequenceGestureRecognizer>> gestureRecognizers)
    {
        this.hitTestBehavior = hitTestBehavior;
        this._viewController = viewController;
    }

    public virtual T viewController
    {
        get => this._viewController;
        set
        {
            var __value = value;
            if (EqualityComparer<T>.Default.Equals(this._viewController, __value))
            {
                return;
            }
            var needsSemanticsUpdate = (this._viewController.id != __value.id);
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
        return ((BoxConstraints)constraints).biggest;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        context.addLayer(new PlatformViewLayer(rect: (offset & size), viewId: this._viewController.id));
    }

    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        if (((object.Equals(this.hitTestBehavior, PlatformViewHitTestBehavior.transparent)) || !size.contains(DartRuntimePrimitives.RequireValue(position))))
        {
            return false;
        }
        result.add(new BoxHitTestEntry(this, DartRuntimePrimitives.RequireValue(position)));
        return (object.Equals(this.hitTestBehavior, PlatformViewHitTestBehavior.opaque));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestSelf(Offset position) => (!object.Equals(this.hitTestBehavior, PlatformViewHitTestBehavior.transparent));
    internal virtual void _handleGlobalPointerEvent(global::Doroti.Framework.Gestures.PointerEvent @event)
    {
        if (!hasSize)
        {
            return;
        }
        if ((@event is not global::Doroti.Framework.Gestures.PointerDownEvent))
        {
            return;
        }
        if (!((Offset.zero & size)).contains(globalToLocal(DartRuntimePrimitives.RequireValue(@event.position))))
        {
            return;
        }
        if ((!object.Equals(((@event.original ?? @event)), this._lastPointerDownEvent)))
        {
            _ = this._viewController.rejectGesture();
        }
        _lastPointerDownEvent = null;
    }

    public override void describeSemanticsConfiguration(global::Doroti.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.isSemanticBoundary = true;
        config.platformViewId = this._viewController.id;
        config.hitTestBehavior = Dart_uiLibrary.SemanticsHitTestBehavior.transparent;
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        GestureBinding.instance.pointerRouter.addGlobalRoute(this._handleGlobalPointerEvent);
    }

    public override void detach()
    {
        GestureBinding.instance.pointerRouter.removeGlobalRoute(this._handleGlobalPointerEvent);
        base.detach();
    }

    public abstract void updateGestureRecognizers(HashSet<Factory<OneSequenceGestureRecognizer>> gestureRecognizers);
}

public class RenderUiKitView : RenderDarwinPlatformView<UiKitViewController>
{

    public RenderUiKitView(UiKitViewController viewController, PlatformViewHitTestBehavior hitTestBehavior, HashSet<Factory<OneSequenceGestureRecognizer>> gestureRecognizers) : base(viewController: viewController, hitTestBehavior: hitTestBehavior, gestureRecognizers: gestureRecognizers)
    {
    }

    public override void updateGestureRecognizers(HashSet<Factory<OneSequenceGestureRecognizer>> gestureRecognizers)
    {
        DartRuntimePrimitives.Assert(() => (checked((long)(Platform_viewLibrary._factoriesTypeSet(gestureRecognizers).Count)) == checked((long)(gestureRecognizers.Count))));
        if (Platform_viewLibrary._factoryTypesSetEquals(gestureRecognizers, _gestureRecognizer?.gestureRecognizerFactories))
        {
            return;
        }
        _gestureRecognizer?.dispose();
        _gestureRecognizer = new _UiKitViewGestureRecognizer__platform_view(viewController, gestureRecognizers);
    }

    public override void handleEvent(global::Doroti.Framework.Gestures.PointerEvent @event, HitTestEntry<HitTestTarget> entry)
    {
        if ((@event is not global::Doroti.Framework.Gestures.PointerDownEvent))
        {
            return;
        }
        _gestureRecognizer!.addPointer((global::Doroti.Framework.Gestures.PointerDownEvent)(object)@event);
        _lastPointerDownEvent = (@event.original ?? @event);
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
    public RenderAppKitView(AppKitViewController viewController, PlatformViewHitTestBehavior hitTestBehavior, HashSet<Factory<OneSequenceGestureRecognizer>> gestureRecognizers) : base(viewController: viewController, hitTestBehavior: hitTestBehavior, gestureRecognizers: gestureRecognizers)
    {
    }

    public override void updateGestureRecognizers(HashSet<Factory<OneSequenceGestureRecognizer>> gestureRecognizers)
    {
    }

}

internal class _UiKitViewGestureRecognizer__platform_view : OneSequenceGestureRecognizer
{
    public virtual HashSet<Factory<OneSequenceGestureRecognizer>> gestureRecognizerFactories { get; private set; } = default!;
    internal virtual HashSet<OneSequenceGestureRecognizer> _gestureRecognizers { get; set; } = default!;
    public virtual UiKitViewController controller { get; private set; } = default!;

    internal _UiKitViewGestureRecognizer__platform_view(UiKitViewController controller, HashSet<Factory<OneSequenceGestureRecognizer>> gestureRecognizerFactories)
    {
        this.controller = controller;
        this.gestureRecognizerFactories = gestureRecognizerFactories;
    }

    public override void addAllowedPointer(global::Doroti.Framework.Gestures.PointerDownEvent @event)
    {
        base.addAllowedPointer(@event);
        foreach (OneSequenceGestureRecognizer recognizer in this._gestureRecognizers)
        {
            recognizer.addPointer((global::Doroti.Framework.Gestures.PointerDownEvent)(object)@event);
        }
    }

    public override string debugDescription => "UIKit view";
    public override void didStopTrackingLastPointer(long pointer)
    {
    }

    public override void handleEvent(global::Doroti.Framework.Gestures.PointerEvent @event)
    {
        stopTrackingIfPointerNoLongerDown(@event);
    }

    public override void acceptGesture(long pointer)
    {
        _ = this.controller.acceptGesture();
    }

    public override void rejectGesture(long pointer)
    {
        _ = this.controller.rejectGesture();
    }

    public virtual void reset()
    {
        resolve(GestureDisposition.rejected);
    }

}

internal delegate Future _HandlePointerEvent__platform_view(global::Doroti.Framework.Gestures.PointerEvent @event);

public class _PlatformViewGestureRecognizer__platform_view : OneSequenceGestureRecognizer
{
    internal virtual Func<global::Doroti.Framework.Gestures.PointerEvent, Future> _handlePointerEvent { get; set; } = default!;
    public virtual DartMap<long, List<global::Doroti.Framework.Gestures.PointerEvent>> cachedEvents { get; private set; } = new DartMap<long, List<global::Doroti.Framework.Gestures.PointerEvent>>();
    public virtual HashSet<long> forwardedPointers { get; private set; } = new HashSet<long>();
    public virtual HashSet<Factory<OneSequenceGestureRecognizer>> gestureRecognizerFactories { get; private set; } = default!;
    internal virtual HashSet<OneSequenceGestureRecognizer> _gestureRecognizers { get; set; } = default!;

    internal _PlatformViewGestureRecognizer__platform_view(Func<global::Doroti.Framework.Gestures.PointerEvent, Future> handlePointerEvent, HashSet<Factory<OneSequenceGestureRecognizer>> gestureRecognizerFactories)
    {
        this.gestureRecognizerFactories = gestureRecognizerFactories;
    }

    public override void addAllowedPointer(global::Doroti.Framework.Gestures.PointerDownEvent @event)
    {
        base.addAllowedPointer(@event);
        foreach (OneSequenceGestureRecognizer recognizer in this._gestureRecognizers)
        {
            recognizer.addPointer((global::Doroti.Framework.Gestures.PointerDownEvent)(object)@event);
        }
    }

    public override string debugDescription => "Platform view";
    public override void didStopTrackingLastPointer(long pointer)
    {
    }

    public override void handleEvent(global::Doroti.Framework.Gestures.PointerEvent @event)
    {
        if (!this.forwardedPointers.Contains(@event.pointer))
        {
            _cacheEvent(@event);
        }
        else
        {
            _ = this._handlePointerEvent(@event);
        }
        stopTrackingIfPointerNoLongerDown(@event);
    }

    public override void acceptGesture(long pointer)
    {
        _flushPointerCache(pointer);
        this.forwardedPointers.Add(pointer);
    }

    public override void rejectGesture(long pointer)
    {
        stopTrackingPointer(pointer);
        this.cachedEvents.remove(pointer);
    }

    internal virtual void _cacheEvent(global::Doroti.Framework.Gestures.PointerEvent @event)
    {
        if (!this.cachedEvents.ContainsKey(@event.pointer))
        {
            this.cachedEvents[@event.pointer] = new List<global::Doroti.Framework.Gestures.PointerEvent>();
        }
        this.cachedEvents.GetValueOrDefault(@event.pointer)!.Add(@event);
    }

    internal virtual void _flushPointerCache(long pointer)
    {
        this.cachedEvents.remove(pointer)?.forEach((__item) => { _ = this._handlePointerEvent(__item); });
    }

    public override void stopTrackingPointer(long pointer)
    {
        base.stopTrackingPointer(pointer);
        this.forwardedPointers.Remove(pointer);
    }

    public virtual void reset()
    {
        this.forwardedPointers.forEach(base.stopTrackingPointer);
        this.forwardedPointers.Clear();
        this.cachedEvents.Keys.forEach(base.stopTrackingPointer);
        this.cachedEvents.Clear();
        resolve(GestureDisposition.rejected);
    }

}

public class PlatformViewRenderBox : RenderBox, _PlatformViewGestureMixin__platform_view
{
    internal virtual PlatformViewController _controller { get; set; } = default!;
    public virtual PlatformViewHitTestBehavior? _hitTestBehavior { get; set; } = default;
    public virtual Func<global::Doroti.Framework.Gestures.PointerEvent, Future>? _handlePointerEvent { get; set; } = default;
    public virtual _PlatformViewGestureRecognizer__platform_view? _gestureRecognizer { get; set; } = default;

    public PlatformViewRenderBox(PlatformViewController controller, PlatformViewHitTestBehavior hitTestBehavior, HashSet<Factory<OneSequenceGestureRecognizer>> gestureRecognizers)
    {
        this._controller = controller;
        System.Diagnostics.Debug.Assert((controller.viewId > -1L));
    }

    public virtual PlatformViewController controller
    {
        get => this._controller;
        set
        {
            var controller = value;
            DartRuntimePrimitives.Assert(() => (controller.viewId > -1L));
            if ((object.Equals(this._controller, controller)))
            {
                return;
            }
            var needsSemanticsUpdate = (this._controller.viewId != controller.viewId);
            _controller = controller;
            markNeedsPaint();
            if (needsSemanticsUpdate)
            {
                markNeedsSemanticsUpdate();
            }
        }
    }
    public virtual void updateGestureRecognizers(HashSet<Factory<OneSequenceGestureRecognizer>> gestureRecognizers)
    {
        _updateGestureRecognizersWithCallBack(gestureRecognizers, (Func<global::Doroti.Framework.Gestures.PointerEvent, Future>)((__event) => _controller.dispatchPointerEvent(global::Doroti.Ui.PointerEvent.FromFrameworkEvent(__event is global::Doroti.Framework.Gestures.PointerDownEvent ? 1L : __event is global::Doroti.Framework.Gestures.PointerUpEvent ? 2L : __event is global::Doroti.Framework.Gestures.PointerCancelEvent ? 3L : __event is global::Doroti.Framework.Gestures.PointerHoverEvent ? 4L : __event is global::Doroti.Framework.Gestures.PointerMoveEvent ? 5L : 0L, __event.pointer, __event.embedderId, __event.platformData, __event.timeStamp, __event.position, __event.kind, __event.orientation, __event.pressure, __event.size, __event.radiusMajor, __event.radiusMinor))));
    }

    public override bool sizedByParent => true;
    public override bool alwaysNeedsCompositing => true;
    public override bool isRepaintBoundary => true;
    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return ((BoxConstraints)constraints).biggest;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        context.addLayer(new PlatformViewLayer(rect: (offset & size), viewId: this._controller.viewId));
    }

    public override void describeSemanticsConfiguration(global::Doroti.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.isSemanticBoundary = true;
        config.platformViewId = this._controller.viewId;
        config.hitTestBehavior = Dart_uiLibrary.SemanticsHitTestBehavior.transparent;
    }

    public virtual PlatformViewHitTestBehavior hitTestBehavior
    {
        set
        {
            var __value = value;
            if ((!object.Equals(__value, this._hitTestBehavior)))
            {
                this._hitTestBehavior = __value;
                if ((owner is not null))
                {
                    markNeedsPaint();
                }
            }
        }
    }
    public virtual void _updateGestureRecognizersWithCallBack(HashSet<Factory<OneSequenceGestureRecognizer>> gestureRecognizers, Func<global::Doroti.Framework.Gestures.PointerEvent, Future> handlePointerEvent)
    {
        DartRuntimePrimitives.Assert(() => (checked((long)(Platform_viewLibrary._factoriesTypeSet(gestureRecognizers).Count)) == checked((long)(gestureRecognizers.Count))));
        if (Platform_viewLibrary._factoryTypesSetEquals(gestureRecognizers, this._gestureRecognizer?.gestureRecognizerFactories))
        {
            return;
        }
        this._gestureRecognizer?.dispose();
        this._gestureRecognizer = new _PlatformViewGestureRecognizer__platform_view(handlePointerEvent, gestureRecognizers);
        _ = this._handlePointerEvent = handlePointerEvent;
    }

    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        if (((object.Equals(this._hitTestBehavior, PlatformViewHitTestBehavior.transparent)) || !size.contains(position)))
        {
            return false;
        }
        result.add(new BoxHitTestEntry(this, position));
        return (object.Equals(this._hitTestBehavior, PlatformViewHitTestBehavior.opaque));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestSelf(Offset position) => (!object.Equals(this._hitTestBehavior, PlatformViewHitTestBehavior.transparent));
    public virtual Action<global::Doroti.Framework.Gestures.PointerEnterEvent>? onEnter => null;
    public virtual Action<global::Doroti.Framework.Gestures.PointerExitEvent>? onExit => null;
    public virtual MouseCursor cursor => (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb ? MouseCursor.defer : MouseCursor.uncontrolled);
    public virtual bool validForMouseTracker => true;
    public override void handleEvent(global::Doroti.Framework.Gestures.PointerEvent @event, HitTestEntry<HitTestTarget> entry)
    {
        if ((@event is global::Doroti.Framework.Gestures.PointerDownEvent))
        {
            this._gestureRecognizer!.addPointer((global::Doroti.Framework.Gestures.PointerDownEvent)(object)@event);
        }
        if ((@event is global::Doroti.Framework.Gestures.PointerHoverEvent))
        {
            _ = this._handlePointerEvent?.Invoke(@event);
        }
    }

    public override void detach()
    {
        this._gestureRecognizer!.reset();
        base.detach();
    }

    public override void dispose()
    {
        this._gestureRecognizer?.dispose();
        base.dispose();
    }

}

public interface _PlatformViewGestureMixin__platform_view
{
    PlatformViewHitTestBehavior? _hitTestBehavior { get; set; }
    Func<global::Doroti.Framework.Gestures.PointerEvent, Future>? _handlePointerEvent { get; set; }
    _PlatformViewGestureRecognizer__platform_view? _gestureRecognizer { get; set; }

    public PlatformViewHitTestBehavior hitTestBehavior { set; }
    public void _updateGestureRecognizersWithCallBack(HashSet<Factory<OneSequenceGestureRecognizer>> gestureRecognizers, Func<global::Doroti.Framework.Gestures.PointerEvent, Future> handlePointerEvent);
    public bool hitTest(BoxHitTestResult result, Offset position);
    public bool hitTestSelf(Offset position);
    public Action<global::Doroti.Framework.Gestures.PointerEnterEvent>? onEnter { get; }
    public Action<global::Doroti.Framework.Gestures.PointerExitEvent>? onExit { get; }
    public MouseCursor cursor { get; }
    public bool validForMouseTracker { get; }
    public void handleEvent(global::Doroti.Framework.Gestures.PointerEvent @event, HitTestEntry<HitTestTarget> entry);
    public void detach();
    public void dispose();
}

