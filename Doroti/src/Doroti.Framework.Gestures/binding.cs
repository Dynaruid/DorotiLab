// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/binding.dart
using System.Diagnostics;
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Gestures;

internal delegate void _HandleSampleTimeChangedCallback__binding();

public class SamplingClock
{
    public virtual DateTime now() => new DateTime();

    public virtual Stopwatch stopwatch() => new Stopwatch();
}

internal class _Resampler__binding
{
    internal virtual DartMap<long, PointerEventResampler> _resamplers { get; private set; } =
        new DartMap<long, PointerEventResampler>();
    internal virtual bool _frameCallbackScheduled { get; set; } = false;
    internal virtual Duration _frameTime { get; set; } = Duration.zero;
    internal virtual Stopwatch _frameTimeAge { get; set; } = new Stopwatch();
    internal virtual Duration _lastSampleTime { get; set; } = Duration.zero;
    internal virtual Duration _lastEventTime { get; set; } = Duration.zero;
    internal virtual Action<PointerEvent> _handlePointerEvent { get; private set; } = default!;
    internal virtual Action _handleSampleTimeChanged { get; private set; } = default!;
    internal virtual Duration _samplingInterval { get; private set; } = default!;
    internal virtual Timer? _timer { get; set; } = default;

    internal _Resampler__binding(
        Action<PointerEvent> _handlePointerEvent,
        Action _handleSampleTimeChanged,
        Duration _samplingInterval
    )
    {
        this._handlePointerEvent = _handlePointerEvent;
        this._handleSampleTimeChanged = _handleSampleTimeChanged;
        this._samplingInterval = _samplingInterval;
    }

    public virtual void addOrDispatch(PointerEvent @event)
    {
        if (Equals(@event.kind, PointerDeviceKind.touch))
        {
            _lastEventTime = @event.timeStamp;
            PointerEventResampler resampler = _resamplers.putIfAbsent(
                @event.device,
                () => new PointerEventResampler()
            );
            resampler.addEvent(@event);
        }
        else
        {
            _handlePointerEvent(@event);
        }
    }

    public virtual void sample(Duration samplingOffset, SamplingClock clock)
    {
        SchedulerBinding scheduler = SchedulerBinding.instance;
        if (Equals(_frameTime, Duration.zero))
        {
            _frameTime = Duration.Create(
                milliseconds: new DateTimeOffset(clock.now()).ToUnixTimeMilliseconds()
            );
            _frameTimeAge = (
                (Func<Stopwatch>)(
                    () =>
                    {
                        var __cascade = clock.stopwatch();
                        __cascade.Start();
                        return __cascade;
                    }
                )
            )();
        }
        if (_timer?.isActive != true)
        {
            _timer = new Timer(_samplingInterval, (_) => _onSampleTimeChanged());
        }
        long samplingIntervalUs = _samplingInterval.inMicroseconds;
        long elapsedIntervals = checked(
            _frameTimeAge.ElapsedTicks / (TimeSpan.TicksPerMillisecond / 1000) / samplingIntervalUs
        );
        long elapsedUs = elapsedIntervals * samplingIntervalUs;
        Duration frameTime = _frameTime + Duration.Create(microseconds: elapsedUs);
        Duration sampleTime = frameTime + samplingOffset;
        Duration nextSampleTime = sampleTime + _samplingInterval;
        foreach (PointerEventResampler resamplerLocal in _resamplers.Values)
        {
            resamplerLocal.sample(sampleTime, nextSampleTime, _handlePointerEvent);
        }
        _resamplers.removeWhere(
            (key, resampler) =>
            {
                return !resampler.hasPendingEvents && !resampler.isDown;
            }
        );
        _lastSampleTime = sampleTime;
        if (checked((long)_resamplers.Count) == 0)
        {
            _timer!.cancel();
            return;
        }
        if (!_frameCallbackScheduled)
        {
            _frameCallbackScheduled = true;
            scheduler.addPostFrameCallback(
                (_) =>
                {
                    _frameCallbackScheduled = false;
                    _frameTime = scheduler.currentSystemFrameTimeStamp;
                    _frameTimeAge.Reset();
                    _timer?.cancel();
                    _timer = new Timer(_samplingInterval, (_) => _onSampleTimeChanged());
                    _onSampleTimeChanged();
                },
                debugLabel: "Resampler.startTimer"
            );
        }
    }

    public virtual void stop()
    {
        foreach (PointerEventResampler resampler in _resamplers.Values)
        {
            resampler.stop(_handlePointerEvent);
        }
        _resamplers.Clear();
        _frameTime = Duration.zero;
        _timer?.cancel();
    }

    internal virtual void _onSampleTimeChanged()
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (DebugLibrary.debugPrintResamplingMargin)
            {
                Duration resamplingMargin = _lastEventTime - _lastSampleTime;
                PrintLibrary.debugPrint($"{resamplingMargin}");
            }
            return true;
        });
        _handleSampleTimeChanged();
    }
}

public static partial class BindingLibrary
{
    internal static Duration _defaultSamplingOffset = Duration.Create(milliseconds: -38L);
}

public static partial class BindingLibrary
{
    internal static Duration _samplingInterval = Duration.Create(microseconds: 16667L);
}

public abstract class GestureBinding
    : Services.ServicesBinding,
        HitTestable,
        HitTestDispatcher,
        HitTestTarget
{
    internal static GestureBinding? _instance = default;
    internal virtual Queue<PointerEvent> _pendingPointerEvents { get; private set; } =
        new Queue<PointerEvent>();
    public virtual PointerRouter pointerRouter { get; private set; } = new PointerRouter();
    public virtual GestureArenaManager gestureArena { get; private set; } =
        new GestureArenaManager();
    public virtual PointerSignalResolver pointerSignalResolver { get; private set; } =
        new PointerSignalResolver();
    internal virtual DartMap<long, HitTestResult> _hitTests { get; private set; } =
        new DartMap<long, HitTestResult>();
    private bool __late__resampler_initialized;
    private _Resampler__binding __late__resampler = default!;
    internal virtual _Resampler__binding _resampler
    {
        get
        {
            if (!__late__resampler_initialized)
            {
                __late__resampler = new _Resampler__binding(
                    _handlePointerEventImmediately,
                    _handleSampleTimeChanged,
                    BindingLibrary._samplingInterval
                );
                __late__resampler_initialized = true;
            }
            return __late__resampler;
        }
    }
    public virtual bool resamplingEnabled { get; set; } = false;
    public virtual Duration samplingOffset { get; set; } = BindingLibrary._defaultSamplingOffset;

    protected GestureBinding(PlatformDispatcher? platformDispatcher = null)
        : base(platformDispatcher) { }

    protected override void initInstances()
    {
        base.initInstances();
        _instance = this;
        (
            (Func<PlatformDispatcher>)(
                () =>
                {
                    var __cascade = platformDispatcher;
                    __cascade.onPointerDataPacket = (_, packet) => _handlePointerDataPacket(packet);
                    __cascade.onHitTest = _handleHitTest;
                    return __cascade;
                }
            )
        )();
    }

    public static new GestureBinding instance => checkInstance(_instance);

    protected override void unlocked()
    {
        base.unlocked();
        _flushPointerEventQueue();
    }

    internal virtual void _handlePointerDataPacket(PointerDataPacket packet)
    {
        try
        {
            _pendingPointerEvents.AddRange(
                PointerEventConverter.expand(packet.data, _devicePixelRatioForView)
            );
            if (!locked)
            {
                _flushPointerEventQueue();
            }
        }
        catch (Exception error)
        {
            var stackLocal = new StackTrace();
            FlutterError.reportError(
                new FlutterErrorDetails(
                    exception: error,
                    stack: stackLocal,
                    library: "gestures library",
                    context: new ErrorDescription("while handling a pointer data packet")
                )
            );
        }
    }

    internal virtual HitTestResponse _handleHitTest(HitTestRequest request)
    {
        var result = new HitTestResult();
        hitTestInView(result, request.offset, checked((long)request.view.viewId));
        bool hasPlatformViewLocal = result.path.any((entry) => entry.target is NativeHitTestTarget);
        return new HitTestResponse(hasPlatformView: hasPlatformViewLocal);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual double? _devicePixelRatioForView(long viewId)
    {
        return platformDispatcher.view(id: viewId)?.devicePixelRatio;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void cancelPointer(long pointer)
    {
        if ((_pendingPointerEvents.Count == 0) && !locked)
        {
            DartAsyncRuntime.scheduleMicrotask(_flushPointerEventQueue);
        }
        _pendingPointerEvents.addFirst(new PointerCancelEvent(pointer: pointer));
    }

    internal virtual void _flushPointerEventQueue()
    {
        DartRuntimePrimitives.Assert(() => !locked);
        while (_pendingPointerEvents.Count != 0)
        {
            handlePointerEvent(_pendingPointerEvents.Dequeue());
        }
    }

    public virtual void handlePointerEvent(PointerEvent @event)
    {
        DartRuntimePrimitives.Assert(() => !locked);
        if (resamplingEnabled)
        {
            _resampler.addOrDispatch(@event);
            _resampler.sample(samplingOffset, samplingClock);
            return;
        }
        _resampler.stop();
        _handlePointerEventImmediately(@event);
    }

    internal virtual void _handlePointerEventImmediately(PointerEvent @event)
    {
        HitTestResult? hitTestResult = default!;
        if (
            (@event is PointerDownEvent)
            || (@event is PointerSignalEvent)
            || (@event is PointerHoverEvent)
            || (@event is PointerPanZoomStartEvent)
        )
        {
            DartRuntimePrimitives.Assert(() => !_hitTests.ContainsKey(@event.pointer));
            hitTestResult = new HitTestResult();
            hitTestInView(hitTestResult, @event.position, @event.viewId);
            if ((@event is PointerDownEvent) || (@event is PointerPanZoomStartEvent))
            {
                _hitTests[@event.pointer] = hitTestResult;
            }
            DartRuntimePrimitives.Assert(() =>
            {
                if (DebugLibrary.debugPrintHitTestResults)
                {
                    PrintLibrary.debugPrint(
                        $"{@event.toDiagnosticsNode().toStringDeep(minLevel: DiagnosticLevel.debug)}: {hitTestResult}"
                    );
                }
                return true;
            });
        }
        else
        {
            if (
                (@event is PointerUpEvent)
                || (@event is PointerCancelEvent)
                || (@event is PointerPanZoomEndEvent)
            )
            {
                hitTestResult = _hitTests.remove(@event.pointer);
            }
            else
            {
                if (@event.down || (@event is PointerPanZoomUpdateEvent))
                {
                    hitTestResult = _hitTests.GetValueOrDefault(@event.pointer);
                }
            }
        }
        DartRuntimePrimitives.Assert(() =>
        {
            if (DebugLibrary.debugPrintMouseHoverEvents && (@event is PointerHoverEvent))
            {
                PointerHoverEvent @event__as17248 = (PointerHoverEvent)@event;
                PrintLibrary.debugPrint($"{@event__as17248}");
            }
            return true;
        });
        if (
            (hitTestResult is not null)
            || (@event is PointerAddedEvent)
            || (@event is PointerRemovedEvent)
        )
        {
            dispatchEvent(@event, hitTestResult);
        }
    }

    public virtual void hitTestInView(HitTestResult result, Offset position, long viewId)
    {
        result.add(new HitTestEntry<HitTestTarget>(this));
    }

    public virtual void hitTest(HitTestResult result, Offset position)
    {
        hitTestInView(result, position, checked((long)platformDispatcher.implicitView!.viewId));
    }

    public virtual void dispatchEvent(PointerEvent @event, HitTestResult? hitTestResult)
    {
        DartRuntimePrimitives.Assert(() => !locked);
        if (hitTestResult is null)
        {
            DartRuntimePrimitives.Assert(() =>
                (@event is PointerAddedEvent) || (@event is PointerRemovedEvent)
            );
            try
            {
                pointerRouter.route(@event);
            }
            catch (Exception exceptionLocal)
            {
                var stackLocal = new StackTrace();
                FlutterError.reportError(
                    new FlutterErrorDetailsForPointerEventDispatcher(
                        exception: exceptionLocal,
                        stack: stackLocal,
                        library: "gesture library",
                        context: new ErrorDescription(
                            "while dispatching a non-hit-tested pointer event"
                        ),
                        @event: @event,
                        informationCollector: () =>
                            new List<DiagnosticsNode>
                            {
                                new DiagnosticsProperty<PointerEvent>(
                                    "Event",
                                    @event,
                                    style: DiagnosticsTreeStyle.errorProperty
                                ),
                            }
                    )
                );
            }
            return;
        }
        foreach (HitTestEntry<HitTestTarget> entry in hitTestResult.path)
        {
            try
            {
                entry.target.handleEvent(@event.transformed(entry.transform), entry);
            }
            catch (Exception exceptionAlternate)
            {
                var stackAlternate = new StackTrace();
                FlutterError.reportError(
                    new FlutterErrorDetailsForPointerEventDispatcher(
                        exception: exceptionAlternate,
                        stack: stackAlternate,
                        library: "gesture library",
                        context: new ErrorDescription("while dispatching a pointer event"),
                        @event: @event,
                        hitTestEntry: entry,
                        informationCollector: () =>
                            new List<DiagnosticsNode>
                            {
                                new DiagnosticsProperty<PointerEvent>(
                                    "Event",
                                    @event,
                                    style: DiagnosticsTreeStyle.errorProperty
                                ),
                                new DiagnosticsProperty<HitTestTarget>(
                                    "Target",
                                    entry.target,
                                    style: DiagnosticsTreeStyle.errorProperty
                                ),
                            }
                    )
                );
            }
        }
    }

    public virtual void handleEvent(PointerEvent @event, HitTestEntry<HitTestTarget> entry)
    {
        pointerRouter.route(@event);
        if ((@event is PointerDownEvent) || (@event is PointerPanZoomStartEvent))
        {
            gestureArena.close(@event.pointer);
        }
        else
        {
            if ((@event is PointerUpEvent) || (@event is PointerPanZoomEndEvent))
            {
                gestureArena.sweep(@event.pointer);
            }
            else
            {
                if (@event is PointerSignalEvent)
                {
                    PointerSignalEvent @event__as21033 = (PointerSignalEvent)@event;
                    pointerSignalResolver.resolve(@event__as21033);
                }
            }
        }
    }

    public virtual void resetGestureBinding()
    {
        _hitTests.Clear();
    }

    internal virtual void _handleSampleTimeChanged()
    {
        if (!locked)
        {
            if (resamplingEnabled)
            {
                _resampler.sample(samplingOffset, samplingClock);
            }
            else
            {
                _resampler.stop();
            }
        }
    }

    public virtual SamplingClock? debugSamplingClock => null;
    public virtual SamplingClock samplingClock
    {
        get
        {
            var value = new SamplingClock();
            DartRuntimePrimitives.Assert(() =>
            {
                SamplingClock? debugValue = debugSamplingClock;
                if (debugValue is not null)
                {
                    value = debugValue;
                }
                return true;
            });
            return value;
        }
    }
}

public class FlutterErrorDetailsForPointerEventDispatcher : FlutterErrorDetails
{
    public virtual PointerEvent? @event { get; private set; }
    public virtual HitTestEntry<HitTestTarget>? hitTestEntry { get; private set; }

    public FlutterErrorDetailsForPointerEventDispatcher(
        object exception,
        StackTrace? stack = null,
        string? library = "Flutter framework",
        DiagnosticsNode? context = null,
        PointerEvent? @event = null,
        HitTestEntry<HitTestTarget>? hitTestEntry = null,
        InformationCollector? informationCollector = null,
        bool silent = false
    )
        : base(
            exception: exception,
            stack: stack,
            library: library,
            context: context,
            informationCollector: informationCollector,
            silent: silent
        )
    {
        this.@event = @event;
        this.hitTestEntry = hitTestEntry;
    }
}
