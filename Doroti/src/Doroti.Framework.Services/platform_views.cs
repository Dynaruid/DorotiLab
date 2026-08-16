#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/platform_views.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Services;

public delegate Offset PointTransformer(Offset position);

public static partial class Platform_viewsLibrary
{
    public static PlatformViewsRegistry platformViewsRegistry = new PlatformViewsRegistry();
}

public class PlatformViewsRegistry
{
    internal virtual long _nextPlatformViewId { get; set; } = 0L;

    public PlatformViewsRegistry()
    {
    }

    public virtual long getNextPlatformViewId()
    {
        var MAX_INT32 = 2147483647L;
        DartRuntimePrimitives.Assert(() => (_nextPlatformViewId <= MAX_INT32));
        return _nextPlatformViewId++;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public delegate void PlatformViewCreatedCallback(long id);

public class PlatformViewsService
{
    internal static PlatformViewsService _instance = new PlatformViewsService();
    internal virtual DartMap<long, Action> _focusCallbacks { get; private set; } = new DartMap<long, Action>();

    public PlatformViewsService()
    {
    }

    internal virtual Future _onMethodCall(MethodCall call)
    {
        switch (call.method)
        {
            case var __case2931 when object.Equals(__case2931, "viewFocused"):
                {
                    var id__2960 = ((long)call.arguments);
                    if (_focusCallbacks.ContainsKey(id__2960))
                    {
                        _focusCallbacks.GetValueOrDefault(id__2960)!();
                    }
                    break;
                }
            default:
                {
                    throw new NotImplementedException($"{call.method} was invoked but isn't implemented by PlatformViewsService");
                }
        }
        return Future.value();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static AndroidViewController initAndroidView(long id, string viewType, TextDirection layoutDirection, object creationParams = default!, MessageCodec<object>? creationParamsCodec = null, Action? onFocus = null)
    {
        DartRuntimePrimitives.Assert(() => ((creationParams is null) || (creationParamsCodec is not null)));
        var controller = new TextureAndroidViewController(viewId: id, viewType: viewType, layoutDirection: layoutDirection, creationParams: creationParams, creationParamsCodec: creationParamsCodec);
        _instance._focusCallbacks[id] = (onFocus ?? (() =>
        {
        }));
        return controller;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static SurfaceAndroidViewController initSurfaceAndroidView(long id, string viewType, TextDirection layoutDirection, object creationParams = default!, MessageCodec<object>? creationParamsCodec = null, Action? onFocus = null)
    {
        DartRuntimePrimitives.Assert(() => ((creationParams is null) || (creationParamsCodec is not null)));
        var controller = new SurfaceAndroidViewController(viewId: id, viewType: viewType, layoutDirection: layoutDirection, creationParams: creationParams, creationParamsCodec: creationParamsCodec);
        _instance._focusCallbacks[id] = (onFocus ?? (() =>
        {
        }));
        return controller;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ExpensiveAndroidViewController initExpensiveAndroidView(long id, string viewType, TextDirection layoutDirection, object creationParams = default!, MessageCodec<object>? creationParamsCodec = null, Action? onFocus = null)
    {
        var controller = new ExpensiveAndroidViewController(viewId: id, viewType: viewType, layoutDirection: layoutDirection, creationParams: creationParams, creationParamsCodec: creationParamsCodec);
        _instance._focusCallbacks[id] = (onFocus ?? (() =>
        {
        }));
        return controller;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static HybridAndroidViewController initHybridAndroidView(long id, string viewType, TextDirection layoutDirection, object creationParams = default!, MessageCodec<object>? creationParamsCodec = null, Action? onFocus = null)
    {
        var controller = new HybridAndroidViewController(viewId: id, viewType: viewType, layoutDirection: layoutDirection, creationParams: creationParams, creationParamsCodec: creationParamsCodec);
        _instance._focusCallbacks[id] = (onFocus ?? (() =>
        {
        }));
        return controller;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static async Future<UiKitViewController> initUiKitView(long id, string viewType, UiKitViewGestureBlockingPolicy gestureBlockingPolicy = UiKitViewGestureBlockingPolicy.fallbackToPluginDefault, TextDirection layoutDirection = default!, object creationParams = default!, MessageCodec<object>? creationParamsCodec = null, Action? onFocus = null)
    {
        DartRuntimePrimitives.Assert(() => ((creationParams is null) || (creationParamsCodec is not null)));
        string gestureBlockingPolicyValue = (gestureBlockingPolicy switch { var __case10434 when object.Equals(__case10434, UiKitViewGestureBlockingPolicy.eager) => "eager", var __case10489 when object.Equals(__case10489, UiKitViewGestureBlockingPolicy.waitUntilTouchesEnded) => "waitUntilTouchesEnded", var __case10576 when object.Equals(__case10576, UiKitViewGestureBlockingPolicy.fallbackToPluginDefault) => "fallbackToPluginDefault", var __case10667 when object.Equals(__case10667, UiKitViewGestureBlockingPolicy.doNotBlockGesture) => "doNotBlockGesture", _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        var args = new DartMap<string, object> { ["id"] = id, ["viewType"] = viewType, ["gestureBlockingPolicy"] = gestureBlockingPolicyValue };
        if ((creationParams is not null))
        {
            ByteData paramsByteData__11083 = creationParamsCodec!.encodeMessage(creationParams)!;
            args["params"] = new Uint8List(paramsByteData__11083.buffer, 0L, paramsByteData__11083.lengthInBytes);
        }
        await SystemChannels.platform_views.invokeMethod<object?>("create", args);
        if ((onFocus is not null))
        {
            _instance._focusCallbacks[id] = onFocus;
        }
        return new UiKitViewController(id, layoutDirection);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static async Future<AppKitViewController> initAppKitView(long id, string viewType, TextDirection layoutDirection, object creationParams = default!, MessageCodec<object>? creationParamsCodec = null, Action? onFocus = null)
    {
        DartRuntimePrimitives.Assert(() => ((creationParams is null) || (creationParamsCodec is not null)));
        var args = new DartMap<string, object> { ["id"] = id, ["viewType"] = viewType };
        if ((creationParams is not null))
        {
            ByteData paramsByteData__12780 = creationParamsCodec!.encodeMessage(creationParams)!;
            args["params"] = new Uint8List(paramsByteData__12780.buffer, 0L, paramsByteData__12780.lengthInBytes);
        }
        await SystemChannels.platform_views.invokeMethod<object?>("create", args);
        if ((onFocus is not null))
        {
            _instance._focusCallbacks[id] = onFocus;
        }
        return new AppKitViewController(id, layoutDirection);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class AndroidPointerProperties
{
    public virtual long id { get; private set; } = default!;
    public virtual long toolType { get; private set; } = default!;
    public const long kToolTypeUnknown = 0L;
    public const long kToolTypeFinger = 1L;
    public const long kToolTypeStylus = 2L;
    public const long kToolTypeMouse = 3L;
    public const long kToolTypeEraser = 4L;

    public AndroidPointerProperties(long id, long toolType)
    {
        this.id = id;
        this.toolType = toolType;
    }

    internal virtual List<long> _asList() => new List<long> { id, toolType };
    public override string ToString()
    {
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "AndroidPointerProperties"))}(id: {id}, toolType: {toolType})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class AndroidPointerCoords
{
    public virtual double orientation { get; private set; } = default!;
    public virtual double pressure { get; private set; } = default!;
    public virtual double size { get; private set; } = default!;
    public virtual double toolMajor { get; private set; } = default!;
    public virtual double toolMinor { get; private set; } = default!;
    public virtual double touchMajor { get; private set; } = default!;
    public virtual double touchMinor { get; private set; } = default!;
    public virtual double x { get; private set; } = default!;
    public virtual double y { get; private set; } = default!;

    public AndroidPointerCoords(double orientation, double pressure, double size, double toolMajor, double toolMinor, double touchMajor, double touchMinor, double x, double y)
    {
        this.orientation = orientation;
        this.pressure = pressure;
        this.size = size;
        this.toolMajor = toolMajor;
        this.toolMinor = toolMinor;
        this.touchMajor = touchMajor;
        this.touchMinor = touchMinor;
        this.x = x;
        this.y = y;
    }

    internal virtual List<double> _asList()
    {
        return new List<double> { orientation, pressure, size, toolMajor, toolMinor, touchMajor, touchMinor, x, y };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "AndroidPointerCoords"))}(orientation: {orientation}, pressure: {pressure}, size: {size}, toolMajor: {toolMajor}, toolMinor: {toolMinor}, touchMajor: {touchMajor}, touchMinor: {touchMinor}, x: {x}, y: {y})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class AndroidMotionEvent
{
    public virtual long downTime { get; private set; } = default!;
    public virtual long eventTime { get; private set; } = default!;
    public virtual long action { get; private set; } = default!;
    public virtual long pointerCount { get; private set; } = default!;
    public virtual List<AndroidPointerProperties> pointerProperties { get; private set; } = default!;
    public virtual List<AndroidPointerCoords> pointerCoords { get; private set; } = default!;
    public virtual long metaState { get; private set; } = default!;
    public virtual long buttonState { get; private set; } = default!;
    public virtual double xPrecision { get; private set; } = default!;
    public virtual double yPrecision { get; private set; } = default!;
    public virtual long deviceId { get; private set; } = default!;
    public virtual long edgeFlags { get; private set; } = default!;
    public virtual long source { get; private set; } = default!;
    public virtual long flags { get; private set; } = default!;
    public virtual long motionEventId { get; private set; } = default!;

    public AndroidMotionEvent(long downTime, long eventTime, long action, long pointerCount, List<AndroidPointerProperties> pointerProperties, List<AndroidPointerCoords> pointerCoords, long metaState, long buttonState, double xPrecision, double yPrecision, long deviceId, long edgeFlags, long source, long flags, long motionEventId)
    {
        this.downTime = downTime;
        this.eventTime = eventTime;
        this.action = action;
        this.pointerCount = pointerCount;
        this.pointerProperties = pointerProperties;
        this.pointerCoords = pointerCoords;
        this.metaState = metaState;
        this.buttonState = buttonState;
        this.xPrecision = xPrecision;
        this.yPrecision = yPrecision;
        this.deviceId = deviceId;
        this.edgeFlags = edgeFlags;
        this.source = source;
        this.flags = flags;
        this.motionEventId = motionEventId;
        System.Diagnostics.Debug.Assert((pointerProperties.Count == pointerCount));
        System.Diagnostics.Debug.Assert((pointerCoords.Count == pointerCount));
    }

    internal virtual List<object> _asList(long viewId)
    {
        return new List<object> { viewId, downTime, eventTime, action, pointerCount, pointerProperties.map(((p) => p._asList())).ToList(), pointerCoords.map(((p) => p._asList())).ToList(), metaState, buttonState, xPrecision, yPrecision, deviceId, edgeFlags, source, flags, motionEventId };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"AndroidPointerEvent(downTime: {downTime}, eventTime: {eventTime}, action: {action}, pointerCount: {pointerCount}, pointerProperties: {pointerProperties}, pointerCoords: {pointerCoords}, metaState: {metaState}, buttonState: {buttonState}, xPrecision: {xPrecision}, yPrecision: {yPrecision}, deviceId: {deviceId}, edgeFlags: {edgeFlags}, source: {source}, flags: {flags}, motionEventId: {motionEventId})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal enum _AndroidViewState
{
    waitingForSize,
    creating,
    created,
    disposed
}

internal class _AndroidMotionEventConverter
{
    public virtual DartMap<long, AndroidPointerCoords> pointerPositions { get; private set; } = new DartMap<long, AndroidPointerCoords>();
    public virtual DartMap<long, AndroidPointerProperties> pointerProperties { get; private set; } = new DartMap<long, AndroidPointerProperties>();
    public virtual HashSet<long> usedAndroidPointerIds { get; private set; } = new HashSet<long>();
    public virtual Func<Offset, Offset> pointTransformer { get; set; } = default!;
    public virtual long? downTimeMillis { get; set; } = default;

    internal _AndroidMotionEventConverter()
    {
    }

    public virtual void handlePointerDownEvent(PointerDownEvent @event)
    {
        if ((pointerProperties.Count == 0))
        {
            downTimeMillis = @event.timeStamp.inMilliseconds;
        }
        var androidPointerId = 0L;
        while (usedAndroidPointerIds.Contains(androidPointerId))
        {
            androidPointerId++;
        }
        usedAndroidPointerIds.Add(androidPointerId);
        pointerProperties[@event.pointer] = propertiesFor(@event, androidPointerId);
    }

    public virtual void updatePointerPositions(PointerEvent @event)
    {
        global::Doroti.Ui.Offset position = pointTransformer(@event.position);
        pointerPositions[@event.pointer] = new AndroidPointerCoords(orientation: @event.orientation, pressure: @event.pressure, size: @event.size, toolMajor: @event.radiusMajor, toolMinor: @event.radiusMinor, touchMajor: @event.radiusMajor, touchMinor: @event.radiusMinor, x: position.dx, y: position.dy);
    }

    internal virtual void _remove(long pointer)
    {
        pointerPositions.remove(pointer);
        usedAndroidPointerIds.Remove(pointerProperties.GetValueOrDefault(pointer)!.id);
        pointerProperties.remove(pointer);
        if ((pointerProperties.Count == 0))
        {
            downTimeMillis = null;
        }
    }

    public virtual void handlePointerUpEvent(PointerUpEvent @event)
    {
        _remove(@event.pointer);
    }

    public virtual void handlePointerCancelEvent(PointerCancelEvent @event)
    {
        _remove(@event.pointer);
    }

    public virtual AndroidMotionEvent? toAndroidMotionEvent(PointerEvent @event)
    {
        List<long> pointers = pointerPositions.Keys.ToList();
        long pointerIdx = pointers.IndexOf(@event.pointer);
        long numPointers = pointers.Count;
        var kPointerDataFlagBatched = 1L;
        var kPointerDataFlagMultiple = 2L;
        var kPointerDataFlagMask = 255L;
        var kPointerDataMultiplePointerCountShift = 8L;
        long platformDataFlag = (@event.platformData & kPointerDataFlagMask);
        if ((platformDataFlag == kPointerDataFlagBatched))
        {
            return null;
        }
        if ((platformDataFlag == kPointerDataFlagMultiple))
        {
            long originalPointerCount__26526 = (@event.platformData >> (int)(kPointerDataMultiplePointerCountShift));
            if ((pointerIdx != (originalPointerCount__26526 - 1L)))
            {
                return null;
            }
        }
        long? action = (@event switch { PointerDownEvent _ when (numPointers == 1L) => AndroidViewController.kActionDown, PointerUpEvent _ when (numPointers == 1L) => AndroidViewController.kActionUp, PointerDownEvent _ => AndroidViewController.pointerAction(pointerIdx, AndroidViewController.kActionPointerDown), PointerUpEvent _ => AndroidViewController.pointerAction(pointerIdx, AndroidViewController.kActionPointerUp), PointerMoveEvent _ => AndroidViewController.kActionMove, PointerCancelEvent _ => AndroidViewController.kActionCancel, _ => null });
        if ((action is null))
        {
            return null;
        }
        return new AndroidMotionEvent(downTime: DartRuntimePrimitives.RequireValue(downTimeMillis), eventTime: @event.timeStamp.inMilliseconds, action: DartRuntimePrimitives.RequireValue(action), pointerCount: pointerPositions.Count, pointerProperties: pointers.map(((i) => pointerProperties.GetValueOrDefault(i)!)).ToList(), pointerCoords: pointers.map(((i) => pointerPositions.GetValueOrDefault(i)!)).ToList(), metaState: 0L, buttonState: 0L, xPrecision: 1.0, yPrecision: 1.0, deviceId: 0L, edgeFlags: 0L, source: _AndroidMotionEventConverter.sourceFor(@event), flags: 0L, motionEventId: @event.embedderId);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static long sourceFor(PointerEvent @event)
    {
        return (@event.kind switch { var __case28143 when object.Equals(__case28143, PointerDeviceKind.touch) => AndroidViewController.kInputDeviceSourceTouchScreen, var __case28229 when object.Equals(__case28229, PointerDeviceKind.trackpad) => AndroidViewController.kInputDeviceSourceTouchPad, var __case28315 when object.Equals(__case28315, PointerDeviceKind.mouse) => AndroidViewController.kInputDeviceSourceMouse, var __case28395 when object.Equals(__case28395, PointerDeviceKind.stylus) => AndroidViewController.kInputDeviceSourceStylus, var __case28477 when object.Equals(__case28477, PointerDeviceKind.invertedStylus) => AndroidViewController.kInputDeviceSourceStylus, var __case28567 when object.Equals(__case28567, PointerDeviceKind.unknown) => AndroidViewController.kInputDeviceSourceUnknown, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual AndroidPointerProperties propertiesFor(PointerEvent @event, long pointerId)
    {
        return new AndroidPointerProperties(id: pointerId, toolType: (@event.kind switch { var __case28839 when object.Equals(__case28839, PointerDeviceKind.touch) => AndroidPointerProperties.kToolTypeFinger, var __case28916 when object.Equals(__case28916, PointerDeviceKind.trackpad) => AndroidPointerProperties.kToolTypeFinger, var __case28996 when object.Equals(__case28996, PointerDeviceKind.mouse) => AndroidPointerProperties.kToolTypeMouse, var __case29072 when object.Equals(__case29072, PointerDeviceKind.stylus) => AndroidPointerProperties.kToolTypeStylus, var __case29150 when object.Equals(__case29150, PointerDeviceKind.invertedStylus) => AndroidPointerProperties.kToolTypeEraser, var __case29236 when object.Equals(__case29236, PointerDeviceKind.unknown) => AndroidPointerProperties.kToolTypeUnknown, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CreationParams
{
    public virtual object data { get; private set; } = default!;
    public virtual MessageCodec<object> codec { get; private set; } = default!;

    internal _CreationParams(object data, MessageCodec<object> codec)
    {
        this.data = data;
        this.codec = codec;
    }

}

public abstract class AndroidViewController : PlatformViewController
{
    public const long kActionDown = 0L;
    public const long kActionUp = 1L;
    public const long kActionMove = 2L;
    public const long kActionCancel = 3L;
    public const long kActionPointerDown = 5L;
    public const long kActionPointerUp = 6L;
    public const long kAndroidLayoutDirectionLtr = 0L;
    public const long kAndroidLayoutDirectionRtl = 1L;
    public const long kInputDeviceSourceUnknown = 0L;
    public const long kInputDeviceSourceTouchScreen = 4098L;
    public const long kInputDeviceSourceMouse = 8194L;
    public const long kInputDeviceSourceStylus = 16386L;
    public const long kInputDeviceSourceTouchPad = 1048584L;
    public override long viewId { get; } = default!;
    internal virtual string _viewType { get; private set; } = default!;
    internal virtual _AndroidMotionEventConverter _motionEventConverter { get; private set; } = new _AndroidMotionEventConverter();
    internal virtual TextDirection _layoutDirection { get; set; } = default!;
    internal virtual _AndroidViewState _state { get; set; } = _AndroidViewState.waitingForSize;
    internal virtual _CreationParams? _creationParams { get; private set; }
    internal virtual List<Action<long>> _platformViewCreatedCallbacks { get; private set; } = new List<Action<long>>();

    protected AndroidViewController(long viewId, string viewType, TextDirection layoutDirection, object creationParams = default!, MessageCodec<object>? creationParamsCodec = null)
    {
        this.viewId = viewId;
        this._viewType = viewType;
        this._layoutDirection = layoutDirection;
        this._creationParams = ((creationParams is null) ? null : new _CreationParams(creationParams, creationParamsCodec!));
        System.Diagnostics.Debug.Assert(((creationParams is null) || (creationParamsCodec is not null)));
    }

    internal static long _getAndroidDirection(TextDirection direction)
    {
        return (direction switch { var __case33618 when object.Equals(__case33618, TextDirection.ltr) => kAndroidLayoutDirectionLtr, var __case33673 when object.Equals(__case33673, TextDirection.rtl) => kAndroidLayoutDirectionRtl, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static long pointerAction(long pointerId, long action)
    {
        return (((((pointerId << (int)(8L))) & 65280L)) | ((action & 255L)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal abstract Future _sendDisposeMessage();
    internal abstract bool _createRequiresSize { get; }
    internal abstract Future _sendCreateMessage(Size? size, Offset? position = null);
    internal abstract Future<Size> _sendResizeMessage(Size size);
    public override bool awaitingCreation => (object.Equals(_state, _AndroidViewState.waitingForSize));
    public async override Future create(Size? size = null, Offset? position = null)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(_state, _AndroidViewState.disposed)));
        DartRuntimePrimitives.Assert(() => (object.Equals(_state, _AndroidViewState.waitingForSize)));
        if ((_createRequiresSize && (size is null)))
        {
            return;
        }
        _state = _AndroidViewState.creating;
        await _sendCreateMessage(size: size, position: position);
        _state = _AndroidViewState.created;
        foreach (Action<long> callback in _platformViewCreatedCallbacks)
        {
            callback(viewId);
        }
    }

    public async virtual Future<Size> setSize(Size size)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(_state, _AndroidViewState.disposed)));
        if ((object.Equals(_state, _AndroidViewState.waitingForSize)))
        {
            await create(size: size);
            return size;
        }
        else
        {
            return await _sendResizeMessage(size);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract Future setOffset(Offset off);
    public abstract long? textureId { get; }
    public virtual bool requiresViewComposition => false;
    public async virtual Future sendMotionEvent(AndroidMotionEvent @event)
    {
        await SystemChannels.platform_views.invokeMethod<object>("touch", @event._asList(viewId));
    }

    public virtual Func<Offset, Offset> pointTransformer
    {
        get => _motionEventConverter.pointTransformer;
        set
        {
            var transformer = value;
            _motionEventConverter.pointTransformer = transformer;
        }
    }
    public virtual bool isCreated => (object.Equals(_state, _AndroidViewState.created));
    public virtual void addOnPlatformViewCreatedListener(Action<long> listener)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(_state, _AndroidViewState.disposed)));
        _platformViewCreatedCallbacks.Add(listener);
    }

    public virtual void removeOnPlatformViewCreatedListener(Action<long> listener)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(_state, _AndroidViewState.disposed)));
        _platformViewCreatedCallbacks.Remove(listener);
    }

    public virtual List<Action<long>> createdCallbacks => _platformViewCreatedCallbacks;
    public async virtual Future setLayoutDirection(TextDirection layoutDirection)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(_state, _AndroidViewState.disposed)));
        if ((object.Equals(layoutDirection, _layoutDirection)))
        {
            return;
        }
        _layoutDirection = layoutDirection;
        if ((object.Equals(_state, _AndroidViewState.waitingForSize)))
        {
            return;
        }
        await SystemChannels.platform_views.invokeMethod<object?>("setDirection", new DartMap<string, object> { ["id"] = viewId, ["direction"] = _getAndroidDirection(layoutDirection) });
    }

    public async override Future dispatchPointerEvent(PointerEvent @event)
    {
        if (@event is PointerHoverEvent @event__as41207)
        {
            return;
        }
        if (@event is PointerDownEvent @event__as41266)
        {
            _motionEventConverter.handlePointerDownEvent(@event__as41266);
        }
        _motionEventConverter.updatePointerPositions(@event);
        AndroidMotionEvent? androidEvent = _motionEventConverter.toAndroidMotionEvent(@event);
        if (@event is PointerUpEvent @event__as41524)
        {
            _motionEventConverter.handlePointerUpEvent(@event__as41524);
        }
        else
        {
            if (@event is PointerCancelEvent @event__as41623)
            {
                _motionEventConverter.handlePointerCancelEvent(@event__as41623);
            }
        }
        if ((androidEvent is not null))
        {
            await sendMotionEvent(androidEvent);
        }
    }

    public override Future clearFocus()
    {
        if ((!object.Equals(_state, _AndroidViewState.created)))
        {
            return Future.value();
        }
        return SystemChannels.platform_views.invokeMethod<object?>("clearFocus", viewId);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async override Future dispose()
    {
        _AndroidViewState state = _state;
        _state = _AndroidViewState.disposed;
        _platformViewCreatedCallbacks.Clear();
        PlatformViewsService._instance._focusCallbacks.remove(viewId);
        if (((object.Equals(state, _AndroidViewState.creating)) || (object.Equals(state, _AndroidViewState.created))))
        {
            await _sendDisposeMessage();
        }
    }

}

public class SurfaceAndroidViewController : AndroidViewController
{
    internal virtual _AndroidViewControllerInternals _internals { get; set; } = new _TextureAndroidViewControllerInternals();

    public SurfaceAndroidViewController(long viewId, string viewType, TextDirection layoutDirection, object creationParams = default!, MessageCodec<object>? creationParamsCodec = null) : base(viewId, viewType, layoutDirection, creationParams, creationParamsCodec)
    {
    }

    internal override bool _createRequiresSize => true;
    internal async override Future<bool> _sendCreateMessage(Size? size, Offset? position = null)
    {
        var __size = DartRuntimePrimitives.RequireValue(size);
        DartRuntimePrimitives.Assert(() => !__size.isEmpty);
        object response = await _AndroidViewControllerInternals.sendCreateMessage(viewId: viewId, viewType: _viewType, hybrid: false, hybridFallback: true, layoutDirection: _layoutDirection, creationParams: _creationParams, size: __size, position: position);
        if (response is long response__as43898)
        {
            (((_TextureAndroidViewControllerInternals?)_internals)!).textureId = ((long)response__as43898);
        }
        else
        {
            _internals = new _HybridAndroidViewControllerInternals();
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long? textureId
    {
        get
        {
            return _internals.textureId;
        }
    }
    public override bool requiresViewComposition
    {
        get
        {
            return _internals.requiresViewComposition;
        }
    }
    internal override Future _sendDisposeMessage()
    {
        return _internals.sendDisposeMessage(viewId: viewId);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal override Future<Size> _sendResizeMessage(Size size)
    {
        return _internals.setSize(size, viewId: viewId, viewState: _state);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Future setOffset(Offset off)
    {
        return _internals.setOffset(off, viewId: viewId, viewState: _state);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ExpensiveAndroidViewController : AndroidViewController
{
    internal virtual _AndroidViewControllerInternals _internals { get; private set; } = new _HybridAndroidViewControllerInternals();

    public ExpensiveAndroidViewController(long viewId, string viewType, TextDirection layoutDirection, object creationParams = default!, MessageCodec<object>? creationParamsCodec = null) : base(viewId, viewType, layoutDirection, creationParams, creationParamsCodec)
    {
    }

    internal override bool _createRequiresSize => false;
    internal async override Future _sendCreateMessage(Size? size, Offset? position = null)
    {
        await _AndroidViewControllerInternals.sendCreateMessage(viewId: viewId, viewType: _viewType, hybrid: true, layoutDirection: _layoutDirection, creationParams: _creationParams, position: position);
    }

    public override long? textureId
    {
        get
        {
            return _internals.textureId;
        }
    }
    public override bool requiresViewComposition
    {
        get
        {
            return _internals.requiresViewComposition;
        }
    }
    internal override Future _sendDisposeMessage()
    {
        return _internals.sendDisposeMessage(viewId: viewId);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal override Future<Size> _sendResizeMessage(Size size)
    {
        return _internals.setSize(size, viewId: viewId, viewState: _state);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Future setOffset(Offset off)
    {
        return _internals.setOffset(off, viewId: viewId, viewState: _state);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class HybridAndroidViewController : AndroidViewController
{
    internal virtual _AndroidViewControllerInternals _internals { get; private set; } = new _Hybrid2AndroidViewControllerInternals();

    public HybridAndroidViewController(long viewId, string viewType, TextDirection layoutDirection, object creationParams = default!, MessageCodec<object>? creationParamsCodec = null) : base(viewId, viewType, layoutDirection, creationParams, creationParamsCodec)
    {
    }

    public static Future<bool> checkIfSupported() => _Hybrid2AndroidViewControllerInternals.checkIfSurfaceControlEnabled();
    internal override bool _createRequiresSize => false;
    internal async override Future _sendCreateMessage(Size? size, Offset? position = null)
    {
        await _AndroidViewControllerInternals.sendCreateMessage(viewId: viewId, viewType: _viewType, hybrid: true, layoutDirection: _layoutDirection, creationParams: _creationParams, position: position, useNewController: true);
    }

    public override long? textureId
    {
        get
        {
            return _internals.textureId;
        }
    }
    public override bool requiresViewComposition
    {
        get
        {
            return _internals.requiresViewComposition;
        }
    }
    internal override Future _sendDisposeMessage()
    {
        return _internals.sendDisposeMessage(viewId: viewId);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal override Future<Size> _sendResizeMessage(Size size)
    {
        return _internals.setSize(size, viewId: viewId, viewState: _state);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Future setOffset(Offset off)
    {
        return _internals.setOffset(off, viewId: viewId, viewState: _state);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async override Future sendMotionEvent(AndroidMotionEvent @event)
    {
        await SystemChannels.platform_views_2.invokeMethod<object>("touch", @event._asList(viewId));
    }

}

public class TextureAndroidViewController : AndroidViewController
{
    internal virtual _AndroidViewControllerInternals _internals { get; set; } = new _TextureAndroidViewControllerInternals();

    public TextureAndroidViewController(long viewId, string viewType, TextDirection layoutDirection, object creationParams = default!, MessageCodec<object>? creationParamsCodec = null) : base(viewId, viewType, layoutDirection, creationParams, creationParamsCodec)
    {
    }

    internal override bool _createRequiresSize => true;
    internal async override Future _sendCreateMessage(Size? size, Offset? position = null)
    {
        var __size = DartRuntimePrimitives.RequireValue(size);
        DartRuntimePrimitives.Assert(() => !__size.isEmpty);
        object response = await _AndroidViewControllerInternals.sendCreateMessage(viewId: viewId, viewType: _viewType, hybrid: false, layoutDirection: _layoutDirection, creationParams: _creationParams, size: __size, position: position);
        if (response is long response__as49495)
        {
            (((_TextureAndroidViewControllerInternals?)_internals)!).textureId = ((long)response__as49495);
        }
        else
        {
            _internals = new _Hybrid2AndroidViewControllerInternals();
        }
    }

    public override long? textureId
    {
        get
        {
            if (_internals.requiresViewComposition)
            {
                return null;
            }
            return _internals.textureId;
        }
    }
    public override bool requiresViewComposition
    {
        get
        {
            return _internals.requiresViewComposition;
        }
    }
    internal override Future _sendDisposeMessage()
    {
        return _internals.sendDisposeMessage(viewId: viewId);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal override Future<Size> _sendResizeMessage(Size size)
    {
        if (_internals.requiresViewComposition)
        {
            return Future<global::Doroti.Ui.Size>.value(size);
        }
        return _internals.setSize(size, viewId: viewId, viewState: _state);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Future setOffset(Offset off)
    {
        if (_internals.requiresViewComposition)
        {
            return Future.value();
        }
        return _internals.setOffset(off, viewId: viewId, viewState: _state);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal interface _AndroidViewControllerInternals
{
    public static Future<object> sendCreateMessage(long viewId, string viewType, TextDirection layoutDirection, bool hybrid, bool hybridFallback = false, bool useNewController = false, _CreationParams? creationParams = null, Size? size = null, Offset? position = null)
    {
        var args = new DartMap<string, object> { ["id"] = viewId, ["viewType"] = viewType, ["direction"] = AndroidViewController._getAndroidDirection(layoutDirection), ["width"] = size?.width, ["height"] = size?.height, ["left"] = position?.dx, ["top"] = position?.dy };
        if ((creationParams is not null))
        {
            ByteData paramsByteData__51813 = creationParams.codec.encodeMessage(creationParams.data)!;
            args["params"] = new Uint8List(paramsByteData__51813.buffer, 0L, paramsByteData__51813.lengthInBytes);
        }
        if (useNewController)
        {
            return SystemChannels.platform_views_2.invokeMethod<object>("create", args);
        }
        return SystemChannels.platform_views.invokeMethod<object>("create", args);
    }
    public long? textureId { get; }
    public bool requiresViewComposition { get; }
    public Future<Size> setSize(Size size, long viewId, _AndroidViewState viewState);
    public Future setOffset(Offset offset, long viewId, _AndroidViewState viewState);
    public Future sendDisposeMessage(long viewId);
}

internal class _TextureAndroidViewControllerInternals : _AndroidViewControllerInternals
{
    internal virtual Offset _offset { get; set; } = Offset.zero;
    public virtual long? textureId { get; set; } = default;

    internal _TextureAndroidViewControllerInternals()
    {
    }

    public virtual bool requiresViewComposition => false;
    public async virtual Future<Size> setSize(Size size, long viewId, _AndroidViewState viewState)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(viewState, _AndroidViewState.waitingForSize)));
        DartRuntimePrimitives.Assert(() => !size.isEmpty);
        DartMap<object?, object?>? meta = await SystemChannels.platform_views.invokeMapMethod<object?, object?>("resize", new DartMap<string, object> { ["id"] = viewId, ["width"] = size.width, ["height"] = size.height });
        DartRuntimePrimitives.Assert(() => (meta is not null));
        DartRuntimePrimitives.Assert(() => meta!.ContainsKey("width"));
        DartRuntimePrimitives.Assert(() => meta!.ContainsKey("height"));
        return new global::Doroti.Ui.Size(((double)meta!.GetValueOrDefault("width")!), ((double)meta.GetValueOrDefault("height")!));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async virtual Future setOffset(Offset offset, long viewId, _AndroidViewState viewState)
    {
        if ((object.Equals(offset, _offset)))
        {
            return;
        }
        if ((!object.Equals(viewState, _AndroidViewState.created)))
        {
            return;
        }
        _offset = offset;
        await SystemChannels.platform_views.invokeMethod<object?>("offset", new DartMap<string, object> { ["id"] = viewId, ["top"] = offset.dy, ["left"] = offset.dx });
    }

    public virtual Future sendDisposeMessage(long viewId)
    {
        return SystemChannels.platform_views.invokeMethod<object?>("dispose", new DartMap<string, object> { ["id"] = viewId, ["hybrid"] = false });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _HybridAndroidViewControllerInternals : _AndroidViewControllerInternals
{
    public virtual long? textureId
    {
        get
        {
            throw new NotImplementedException("Not supported for hybrid composition.");
        }
    }
    public virtual bool requiresViewComposition => true;
    public virtual Future<Size> setSize(Size size, long viewId, _AndroidViewState viewState)
    {
        throw new NotImplementedException("Not supported for hybrid composition.");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future setOffset(Offset offset, long viewId, _AndroidViewState viewState)
    {
        throw new NotImplementedException("Not supported for hybrid composition.");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future sendDisposeMessage(long viewId)
    {
        return SystemChannels.platform_views.invokeMethod<object?>("dispose", new DartMap<string, object> { ["id"] = viewId, ["hybrid"] = true });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _Hybrid2AndroidViewControllerInternals : _AndroidViewControllerInternals
{
    public static async Future<bool> checkIfSurfaceControlEnabled()
    {
        return DartRuntimePrimitives.RequireValue((await SystemChannels.platform_views_2.invokeMethod<bool>("isSurfaceControlEnabled", new DartMap<string, object?>())));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long? textureId
    {
        get
        {
            throw new NotImplementedException("Not supported for hybrid composition.");
        }
    }
    public virtual bool requiresViewComposition => true;
    public virtual Future<Size> setSize(Size size, long viewId, _AndroidViewState viewState)
    {
        throw new NotImplementedException("Not supported for hybrid composition.");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future setOffset(Offset offset, long viewId, _AndroidViewState viewState)
    {
        throw new NotImplementedException("Not supported for hybrid composition.");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future sendDisposeMessage(long viewId)
    {
        return SystemChannels.platform_views_2.invokeMethod<object?>("dispose", new DartMap<string, object> { ["id"] = viewId, ["hybrid"] = true });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class DarwinPlatformViewController
{
    public virtual long id { get; private set; } = default!;
    internal virtual bool _debugDisposed { get; set; } = false;
    internal virtual TextDirection _layoutDirection { get; set; } = default!;

    protected DarwinPlatformViewController(long id, TextDirection layoutDirection)
    {
        this.id = id;
        this._layoutDirection = layoutDirection;
    }

    public async virtual Future setLayoutDirection(TextDirection layoutDirection)
    {
        DartRuntimePrimitives.Assert(() => !_debugDisposed);
        if ((object.Equals(layoutDirection, _layoutDirection)))
        {
            return;
        }
        _layoutDirection = layoutDirection;
    }

    public virtual Future acceptGesture()
    {
        var args = new DartMap<string, object> { ["id"] = id };
        return SystemChannels.platform_views.invokeMethod<object?>("acceptGesture", args);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future rejectGesture()
    {
        var args = new DartMap<string, object> { ["id"] = id };
        return SystemChannels.platform_views.invokeMethod<object?>("rejectGesture", args);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async virtual Future dispose()
    {
        _debugDisposed = true;
        await SystemChannels.platform_views.invokeMethod<object?>("dispose", id);
        PlatformViewsService._instance._focusCallbacks.remove(id);
    }

}

public enum UiKitViewGestureBlockingPolicy
{
    eager,
    waitUntilTouchesEnded,
    doNotBlockGesture,
    fallbackToPluginDefault
}

public class UiKitViewController : DarwinPlatformViewController
{
    public UiKitViewController(long id, TextDirection layoutDirection) : base(id: id, layoutDirection: layoutDirection)
    {
    }

}

public class AppKitViewController : DarwinPlatformViewController
{
    public AppKitViewController(long id, TextDirection layoutDirection) : base(id: id, layoutDirection: layoutDirection)
    {
    }

}

public abstract class PlatformViewController
{
    public abstract long viewId { get; }
    public virtual bool awaitingCreation => false;
    public abstract Future dispatchPointerEvent(PointerEvent @event);
    public async virtual Future create(Size? size = null, Offset? position = null)
    {
    }

    public abstract Future dispose();
    public abstract Future clearFocus();
}

