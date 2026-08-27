// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/binding.dart
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

namespace Doroti.Framework.Gestures;

internal delegate void _HandleSampleTimeChangedCallback__binding();

public class SamplingClock
{
    public virtual DateTime now() => new DateTime();
    public virtual Stopwatch stopwatch() => new Stopwatch();
}
internal class _Resampler__binding
{
    internal virtual DartMap<long, PointerEventResampler> _resamplers { get; private set; } = new DartMap<long, PointerEventResampler>();
    internal virtual bool _frameCallbackScheduled { get; set; } = false;
    internal virtual Duration _frameTime { get; set; } = Duration.zero;
    internal virtual Stopwatch _frameTimeAge { get; set; } = new Stopwatch();
    internal virtual Duration _lastSampleTime { get; set; } = Duration.zero;
    internal virtual Duration _lastEventTime { get; set; } = Duration.zero;
    internal virtual Action<PointerEvent> _handlePointerEvent { get; private set; } = default!;
    internal virtual Action _handleSampleTimeChanged { get; private set; } = default!;
    internal virtual Duration _samplingInterval { get; private set; } = default!;
    internal virtual Timer? _timer { get; set; } = default;

    internal _Resampler__binding(Action<PointerEvent> _handlePointerEvent, Action _handleSampleTimeChanged, Duration _samplingInterval)
    {
        this._handlePointerEvent = _handlePointerEvent;
        this._handleSampleTimeChanged = _handleSampleTimeChanged;
        this._samplingInterval = _samplingInterval;
    }

    public virtual void addOrDispatch(PointerEvent @event)
    {
        if ((object.Equals(((PointerEvent)@event).kind, PointerDeviceKind.touch)))
        {
            _lastEventTime = ((PointerEvent)@event).timeStamp;
            PointerEventResampler resampler = this._resamplers.putIfAbsent(((PointerEvent)@event).device, (() => new PointerEventResampler()));
            resampler.addEvent(@event);
        }
        else
        {
            this._handlePointerEvent(@event);
        }
    }

    public virtual void sample(Duration samplingOffset, SamplingClock clock)
    {
        SchedulerBinding scheduler = SchedulerBinding.instance;
        if ((object.Equals(this._frameTime, Duration.zero)))
        {
            _frameTime = Duration.Create(milliseconds: new DateTimeOffset(clock.now()).ToUnixTimeMilliseconds());
            _frameTimeAge = ((Func<Stopwatch>)(() =>
{
    var __cascade = clock.stopwatch();
    __cascade.Start();
    return __cascade;
}))();
        }
        if ((this._timer?.isActive != true))
        {
            _timer = new Timer(this._samplingInterval, ((_) => _onSampleTimeChanged()));
        }
        long samplingIntervalUs = this._samplingInterval.inMicroseconds;
        long elapsedIntervals = (checked((long)(this._frameTimeAge.ElapsedTicks / (TimeSpan.TicksPerMillisecond / 1000) / samplingIntervalUs)));
        long elapsedUs = (elapsedIntervals * samplingIntervalUs);
        Duration frameTime = (this._frameTime + Duration.Create(microseconds: elapsedUs));
        Duration sampleTime = (frameTime + samplingOffset);
        Duration nextSampleTime = (sampleTime + this._samplingInterval);
        foreach (PointerEventResampler resamplerLocal in this._resamplers.Values)
        {
            resamplerLocal.sample(sampleTime, nextSampleTime, (Action<PointerEvent>)this._handlePointerEvent);
        }
        this._resamplers.removeWhere(((key, resampler) =>
        {
            return (!((PointerEventResampler)resampler).hasPendingEvents && !((PointerEventResampler)resampler).isDown);
            return default;
        }));
        _lastSampleTime = sampleTime;
        if ((checked((long)(this._resamplers.Count)) == 0))
        {
            this._timer!.cancel();
            return;
        }
        if (!this._frameCallbackScheduled)
        {
            _frameCallbackScheduled = true;
            scheduler.addPostFrameCallback(((_) =>
            {
                _frameCallbackScheduled = false;
                _frameTime = scheduler.currentSystemFrameTimeStamp;
                this._frameTimeAge.Reset();
                this._timer?.cancel();
                _timer = new Timer(this._samplingInterval, ((_) => _onSampleTimeChanged()));
                _onSampleTimeChanged();
            }), debugLabel: "Resampler.startTimer");
        }
    }

    public virtual void stop()
    {
        foreach (PointerEventResampler resampler in this._resamplers.Values)
        {
            resampler.stop((Action<PointerEvent>)this._handlePointerEvent);
        }
        this._resamplers.Clear();
        _frameTime = Duration.zero;
        this._timer?.cancel();
    }

    internal virtual void _onSampleTimeChanged()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (global::Doroti.Framework.Gestures.DebugLibrary.debugPrintResamplingMargin)
                {
                    Duration resamplingMargin = (this._lastEventTime - this._lastSampleTime);
                    global::Doroti.Framework.Foundation.PrintLibrary.debugPrint($"{resamplingMargin}");
                }
                return true;
            });
        this._handleSampleTimeChanged();
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

public abstract class GestureBinding : global::Doroti.Framework.Services.ServicesBinding, HitTestable, HitTestDispatcher, HitTestTarget
{
    internal static GestureBinding? _instance = default;
    internal virtual Queue<PointerEvent> _pendingPointerEvents { get; private set; } = new Queue<PointerEvent>();
    public virtual PointerRouter pointerRouter { get; private set; } = new PointerRouter();
    public virtual GestureArenaManager gestureArena { get; private set; } = new GestureArenaManager();
    public virtual PointerSignalResolver pointerSignalResolver { get; private set; } = new PointerSignalResolver();
    internal virtual DartMap<long, HitTestResult> _hitTests { get; private set; } = new DartMap<long, HitTestResult>();
    private bool __late__resampler_initialized;
    private _Resampler__binding __late__resampler = default!;
    internal virtual _Resampler__binding _resampler
    {
        get
        {
            if (!__late__resampler_initialized)
            {
                __late__resampler = new _Resampler__binding(this._handlePointerEventImmediately, this._handleSampleTimeChanged, BindingLibrary._samplingInterval);
                __late__resampler_initialized = true;
            }
            return __late__resampler;
        }
    }
    public virtual bool resamplingEnabled { get; set; } = false;
    public virtual Duration samplingOffset { get; set; } = BindingLibrary._defaultSamplingOffset;

    protected GestureBinding(PlatformDispatcher? platformDispatcher = null)
        : base(platformDispatcher)
    {
    }

    protected override void initInstances()
    {
        base.initInstances();
        _instance = this;
        ((Func<PlatformDispatcher>)(() =>
{
    var __cascade = platformDispatcher;
    __cascade.onPointerDataPacket = (_, packet) => this._handlePointerDataPacket(packet);
    __cascade.onHitTest = this._handleHitTest;
    return __cascade;
}))();
    }

    public static GestureBinding instance => BindingBase.checkInstance(_instance);
    protected override void unlocked()
    {
        base.unlocked();
        _flushPointerEventQueue();
    }

    internal virtual void _handlePointerDataPacket(PointerDataPacket packet)
    {
        try
        {
            this._pendingPointerEvents.AddRange(PointerEventConverter.expand(packet.data, (Func<long, double?>)this._devicePixelRatioForView));
            if (!locked)
            {
                _flushPointerEventQueue();
            }
        }
        catch (Exception error)
        {
            var stackLocal = new System.Diagnostics.StackTrace();
            FlutterError.reportError(new FlutterErrorDetails(exception: error, stack: stackLocal, library: "gestures library", context: new ErrorDescription("while handling a pointer data packet")));
        }
    }

    internal virtual global::Doroti.Ui.HitTestResponse _handleHitTest(HitTestRequest request)
    {
        var result = new HitTestResult();
        hitTestInView(result, request.offset, checked((long)request.view.viewId));
        bool hasPlatformViewLocal = ((HitTestResult)result).path.any(((entry) => (((HitTestEntry<HitTestTarget>)entry).target is NativeHitTestTarget)));
        return new global::Doroti.Ui.HitTestResponse(hasPlatformView: hasPlatformViewLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double? _devicePixelRatioForView(long viewId)
    {
        return platformDispatcher.view(id: viewId)?.devicePixelRatio;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void cancelPointer(long pointer)
    {
        if (((this._pendingPointerEvents.Count == 0) && !locked))
        {
            DartAsyncRuntime.scheduleMicrotask(this._flushPointerEventQueue);
        }
        this._pendingPointerEvents.addFirst(new PointerCancelEvent(pointer: pointer));
    }

    internal virtual void _flushPointerEventQueue()
    {
        DartRuntimePrimitives.Assert(() => !locked);
        while ((this._pendingPointerEvents.Count != 0))
        {
            handlePointerEvent(this._pendingPointerEvents.Dequeue());
        }
    }

    public virtual void handlePointerEvent(PointerEvent @event)
    {
        DartRuntimePrimitives.Assert(() => !locked);
        if (this.resamplingEnabled)
        {
            this._resampler.addOrDispatch(@event);
            this._resampler.sample(this.samplingOffset, this.samplingClock);
            return;
        }
        this._resampler.stop();
        _handlePointerEventImmediately(@event);
    }

    internal virtual void _handlePointerEventImmediately(PointerEvent @event)
    {
        HitTestResult? hitTestResult = default!;
        if (((((@event is PointerDownEvent) || (@event is PointerSignalEvent)) || (@event is PointerHoverEvent)) || (@event is PointerPanZoomStartEvent)))
        {
            DartRuntimePrimitives.Assert(() => !this._hitTests.ContainsKey(((PointerEvent)@event).pointer));
            hitTestResult = new HitTestResult();
            hitTestInView(hitTestResult, ((PointerEvent)@event).position, ((PointerEvent)@event).viewId);
            if (((@event is PointerDownEvent) || (@event is PointerPanZoomStartEvent)))
            {
                this._hitTests[((PointerEvent)@event).pointer] = hitTestResult;
            }
            DartRuntimePrimitives.Assert(() =>
                {
                    if (global::Doroti.Framework.Gestures.DebugLibrary.debugPrintHitTestResults)
                    {
                        global::Doroti.Framework.Foundation.PrintLibrary.debugPrint($"{@event.toDiagnosticsNode().toStringDeep(minLevel: DiagnosticLevel.debug)}: {hitTestResult}");
                    }
                    return true;
                });
        }
        else
        {
            if ((((@event is PointerUpEvent) || (@event is PointerCancelEvent)) || (@event is PointerPanZoomEndEvent)))
            {
                hitTestResult = this._hitTests.remove(((PointerEvent)@event).pointer);
            }
            else
            {
                if ((((PointerEvent)@event).down || (@event is PointerPanZoomUpdateEvent)))
                {
                    hitTestResult = this._hitTests.GetValueOrDefault(((PointerEvent)@event).pointer);
                }
            }
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if ((global::Doroti.Framework.Gestures.DebugLibrary.debugPrintMouseHoverEvents && (@event is PointerHoverEvent)))
                {
                    PointerHoverEvent @event__as17248 = (PointerHoverEvent)@event;
                    global::Doroti.Framework.Foundation.PrintLibrary.debugPrint($"{((PointerHoverEvent)@event__as17248)}");
                }
                return true;
            });
        if ((((hitTestResult is not null) || (@event is PointerAddedEvent)) || (@event is PointerRemovedEvent)))
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
        if ((hitTestResult is null))
        {
            DartRuntimePrimitives.Assert(() => ((@event is PointerAddedEvent) || (@event is PointerRemovedEvent)));
            try
            {
                this.pointerRouter.route(@event);
            }
            catch (Exception exceptionLocal)
            {
                var stackLocal = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new FlutterErrorDetailsForPointerEventDispatcher(exception: exceptionLocal, stack: stackLocal, library: "gesture library", context: new ErrorDescription("while dispatching a non-hit-tested pointer event"), @event: @event, informationCollector: (() => new List<DiagnosticsNode> { new DiagnosticsProperty<PointerEvent>("Event", @event, style: DiagnosticsTreeStyle.errorProperty) })));
            }
            return;
        }
        foreach (HitTestEntry<HitTestTarget> entry in ((HitTestResult)hitTestResult).path)
        {
            try
            {
                ((HitTestEntry<HitTestTarget>)entry).target.handleEvent(@event.transformed(((HitTestEntry<HitTestTarget>)entry).transform), entry);
            }
            catch (Exception exceptionAlternate)
            {
                var stackAlternate = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new FlutterErrorDetailsForPointerEventDispatcher(exception: exceptionAlternate, stack: stackAlternate, library: "gesture library", context: new ErrorDescription("while dispatching a pointer event"), @event: @event, hitTestEntry: entry, informationCollector: (() => new List<DiagnosticsNode> { new DiagnosticsProperty<PointerEvent>("Event", @event, style: DiagnosticsTreeStyle.errorProperty), new DiagnosticsProperty<HitTestTarget>("Target", ((HitTestEntry<HitTestTarget>)entry).target, style: DiagnosticsTreeStyle.errorProperty) })));
            }
        }
    }

    public virtual void handleEvent(PointerEvent @event, HitTestEntry<HitTestTarget> entry)
    {
        this.pointerRouter.route(@event);
        if (((@event is PointerDownEvent) || (@event is PointerPanZoomStartEvent)))
        {
            this.gestureArena.close(((PointerEvent)@event).pointer);
        }
        else
        {
            if (((@event is PointerUpEvent) || (@event is PointerPanZoomEndEvent)))
            {
                this.gestureArena.sweep(((PointerEvent)@event).pointer);
            }
            else
            {
                if ((@event is PointerSignalEvent))
                {
                    PointerSignalEvent @event__as21033 = (PointerSignalEvent)@event;
                    this.pointerSignalResolver.resolve(((PointerSignalEvent)@event__as21033));
                }
            }
        }
    }

    public virtual void resetGestureBinding()
    {
        this._hitTests.Clear();
    }

    internal virtual void _handleSampleTimeChanged()
    {
        if (!locked)
        {
            if (this.resamplingEnabled)
            {
                this._resampler.sample(this.samplingOffset, this.samplingClock);
            }
            else
            {
                this._resampler.stop();
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
                    SamplingClock? debugValue = this.debugSamplingClock;
                    if ((debugValue is not null))
                    {
                        value = debugValue;
                    }
                    return true;
                });
            return value;
            return default!;
        }
    }
}

public class FlutterErrorDetailsForPointerEventDispatcher : FlutterErrorDetails
{
    public virtual PointerEvent? @event { get; private set; }
    public virtual HitTestEntry<HitTestTarget>? hitTestEntry { get; private set; }

    public FlutterErrorDetailsForPointerEventDispatcher(object exception, global::System.Diagnostics.StackTrace? stack = null, string? library = "Flutter framework", DiagnosticsNode? context = null, PointerEvent? @event = null, HitTestEntry<HitTestTarget>? hitTestEntry = null, InformationCollector? informationCollector = null, bool silent = false) : base(exception: exception, stack: stack, library: library, context: context, informationCollector: informationCollector, silent: silent)
    {
        this.@event = @event;
        this.hitTestEntry = hitTestEntry;
    }

}
