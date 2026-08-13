#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/scheduler/binding.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Scheduler;

public static partial class BindingLibrary
{
    public static double timeDilation
    {
        get => BindingLibrary._timeDilation;
        set
        {
            DartRuntimePrimitives.Assert(() => (value > 0.0));
            if ((BindingLibrary._timeDilation == value))
            {
                return;
            }
            SchedulerBinding._instance?.resetEpoch();
            BindingLibrary._timeDilation = value;
        }
    }
}

public static partial class BindingLibrary
{
    internal static double _timeDilation = 1.0;
}

public delegate void FrameCallback(Duration timeStamp);

public delegate object TaskCallback<T>();

public delegate bool SchedulingStrategy(long priority, SchedulerBinding scheduler);

internal class _TaskEntry<T>
{
    public virtual Func<object> task { get; private set; } = default!;
    public virtual long priority { get; private set; } = default!;
    public virtual string? debugLabel { get; private set; }
    public virtual Flow? flow { get; private set; }
    public virtual StackTrace debugStack { get; set; } = default!;
    public virtual Completer<T> completer { get; private set; } = new Completer<T>();

    internal _TaskEntry(Func<object> task, long priority, string? debugLabel, Flow? flow)
    {
        this.task = task;
        this.priority = priority;
        this.debugLabel = debugLabel;
        this.flow = flow;
    }

    public virtual void run()
    {
        if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            Timeline.timeSync((debugLabel ?? "Scheduled Task"), (() =>
            {
                completer.complete(task());
            }), flow: ((flow is not null) ? Flow.step(flow!.id) : null));
        }
        else
        {
            completer.complete(task());
        }
    }

}

internal class _FrameCallbackEntry
{
    public virtual Action<Duration> callback { get; private set; } = default!;
    public static StackTrace? debugCurrentCallbackStack = default;
    public virtual StackTrace? debugStack { get; set; } = default;

    internal _FrameCallbackEntry(Action<Duration> callback, bool rescheduling = false)
    {
        this.callback = callback;
    }

}

public enum SchedulerPhase
{
    idle,
    transientCallbacks,
    midFrameMicrotasks,
    persistentCallbacks,
    postFrameCallbacks
}

internal delegate void _PerformanceModeCleanupCallback();

public class PerformanceModeRequestHandle
{
    internal virtual Action? _cleanup { get; set; } = default;

    public PerformanceModeRequestHandle(Action _cleanup)
    {
        this._cleanup = _cleanup;
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => (_cleanup is not null));
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        _cleanup!();
        _cleanup = null;
    }

}

public abstract class SchedulerBinding : BindingBase
{
    internal static SchedulerBinding? _instance = default;
    internal virtual List<Action<List<FrameTiming>>> _timingsCallbacks { get; private set; } = new List<Action<List<FrameTiming>>>();
    internal virtual AppLifecycleState? _lifecycleState { get; set; } = default;
    public virtual SchedulingStrategy schedulingStrategy { get; set; } = BindingLibrary.defaultSchedulingStrategy;
    internal virtual PriorityQueue<_TaskEntry<object>> _taskQueue { get; private set; } = new HeapPriorityQueue<_TaskEntry<object>>(_taskSorter);
    internal virtual bool _hasRequestedAnEventLoopCallback { get; set; } = false;
    internal virtual long _nextFrameCallbackId { get; set; } = 0L;
    internal virtual DartMap<long, _FrameCallbackEntry> _transientCallbacks { get; set; } = new DartMap<long, _FrameCallbackEntry>();
    internal virtual HashSet<long> _removedIds { get; private set; } = new HashSet<long>();
    internal virtual List<Action<Duration>> _persistentCallbacks { get; private set; } = new List<Action<Duration>>();
    internal virtual List<Action<Duration>> _postFrameCallbacks { get; private set; } = new List<Action<Duration>>();
    internal virtual Completer<object?>? _nextFrameCompleter { get; set; } = default;
    internal virtual bool _hasScheduledFrame { get; set; } = false;
    internal virtual SchedulerPhase _schedulerPhase { get; set; } = SchedulerPhase.idle;
    internal virtual bool _framesEnabled { get; set; } = true;
    internal virtual bool _warmUpFrame { get; set; } = false;
    internal virtual Duration? _firstRawTimeStampInEpoch { get; set; } = default;
    internal virtual Duration _epochStart { get; set; } = Duration.zero;
    internal virtual Duration _lastRawTimeStamp { get; set; } = Duration.zero;
    internal virtual Duration? _currentFrameTimeStamp { get; set; } = default;
    internal virtual long _debugFrameNumber { get; set; } = 0L;
    internal virtual string? _debugBanner { get; set; } = default;
    internal virtual bool _rescheduleAfterWarmUpFrame { get; set; } = false;
    internal virtual TimelineTask? _frameTimelineTask { get; private set; } = (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode ? null : new TimelineTask());
    internal virtual DartPerformanceMode? _performanceMode { get; set; } = default;
    internal virtual long _numPerformanceModeRequests { get; set; } = 0L;

    protected SchedulerBinding(PlatformDispatcher? platformDispatcher = null)
        : base(platformDispatcher)
    {
    }

    protected override void initInstances()
    {
        base.initInstances();
        _instance = this;
        if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            addTimingsCallback(((timings) =>
            {
                timings.forEach(_profileFramePostEvent);
            }));
        }
    }

    public static SchedulerBinding instance => BindingBase.checkInstance(_instance);
    public virtual void addTimingsCallback(Action<List<FrameTiming>> callback)
    {
        _timingsCallbacks.Add(callback);
        if ((_timingsCallbacks.Count == 1L))
        {
            DartRuntimePrimitives.Assert(() => (platformDispatcher.onReportTimings is null));
            platformDispatcher.onReportTimings = _executeTimingsCallbacks;
        }
        DartRuntimePrimitives.Assert(() => (object.Equals((Action<List<FrameTiming>>?)platformDispatcher.onReportTimings, (Action<List<FrameTiming>>)_executeTimingsCallbacks)));
    }

    public virtual void removeTimingsCallback(Action<List<FrameTiming>> callback)
    {
        DartRuntimePrimitives.Assert(() => _timingsCallbacks.Contains(callback));
        _timingsCallbacks.Remove(callback);
        if ((_timingsCallbacks.Count == 0))
        {
            platformDispatcher.onReportTimings = null;
        }
    }

    internal virtual void _executeTimingsCallbacks(List<FrameTiming> timings)
    {
        var clonedCallbacks = new List<Action<List<FrameTiming>>>(_timingsCallbacks);
        foreach (var callback in clonedCallbacks)
        {
            try
            {
                if (_timingsCallbacks.Contains(callback))
                {
                    callback(timings);
                }
            }
            catch (Exception exception)
            {
                var stack = new System.Diagnostics.StackTrace();
                InformationCollector? collector = default!;
                DartRuntimePrimitives.Assert(() =>
                    {
                        collector = (() => new List<DiagnosticsNode> { new DiagnosticsProperty<Action<List<FrameTiming>>>("The TimingsCallback that gets executed was", callback, style: DiagnosticsTreeStyle.errorProperty) });
                        return true;
                    });
                FlutterError.reportError(new FlutterErrorDetails(exception: exception, stack: stack, context: new ErrorDescription("while executing callbacks for FrameTiming"), informationCollector: collector));
            }
        }
    }

    protected override void initServiceExtensions()
    {
        base.initServiceExtensions();
        if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            registerNumericServiceExtension(name: SchedulerServiceExtensions.timeDilation.ToString(), getter: (() => BindingLibrary.timeDilation), setter: ((value) =>
            {
                BindingLibrary.timeDilation = value;
            }));
        }
    }

    public virtual AppLifecycleState? lifecycleState => _lifecycleState;
    public virtual void resetInternalState()
    {
        _lifecycleState = null;
        _framesEnabled = true;
    }

    public virtual void handleAppLifecycleStateChanged(AppLifecycleState state)
    {
        if ((object.Equals(lifecycleState, state)))
        {
            return;
        }
        _lifecycleState = state;
        switch (state)
        {
            case var __case15339 when object.Equals(__case15339, AppLifecycleState.resumed):
            case var __case15377 when object.Equals(__case15377, AppLifecycleState.inactive):
                {
                    _setFramesEnabledState(true);
                    break;
                }
            case var __case15454 when object.Equals(__case15454, AppLifecycleState.hidden):
            case var __case15491 when object.Equals(__case15491, AppLifecycleState.paused):
            case var __case15528 when object.Equals(__case15528, AppLifecycleState.detached):
                {
                    _setFramesEnabledState(false);
                    break;
                }
        }
    }

    internal static long _taskSorter(_TaskEntry<object> e1, _TaskEntry<object> e2)
    {
        return -e1.priority.CompareTo(e2.priority);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<T> scheduleTask<T>(Func<object> task, Priority priority, string? debugLabel = null, Flow? flow = null)
    {
        bool isFirstTask = (_taskQueue.Count == 0);
        var entry = new _TaskEntry<T>(task, priority.value, debugLabel, flow);
        _taskQueue.Add(entry);
        if ((isFirstTask && !locked))
        {
            _ensureEventLoopCallback();
        }
        return entry.completer.future;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<T> scheduleTask<T>(Func<T> task, Priority priority, string? debugLabel = null, Flow? flow = null) =>
        scheduleTask<T>(() => (object?)task()!, priority, debugLabel, flow);

    protected override void unlocked()
    {
        base.unlocked();
        if ((_taskQueue.Count != 0))
        {
            _ensureEventLoopCallback();
        }
    }

    internal virtual void _ensureEventLoopCallback()
    {
        DartRuntimePrimitives.Assert(() => !locked);
        DartRuntimePrimitives.Assert(() => (_taskQueue.Count != 0));
        if (_hasRequestedAnEventLoopCallback)
        {
            return;
        }
        _hasRequestedAnEventLoopCallback = true;
        global::Doroti.Flutter.Runtime.Timer.run(_runTasks);
    }

    internal virtual void _runTasks()
    {
        _hasRequestedAnEventLoopCallback = false;
        if (handleEventLoopCallback())
        {
            _ensureEventLoopCallback();
        }
    }

    public virtual bool handleEventLoopCallback()
    {
        if (((_taskQueue.Count == 0) || locked))
        {
            return false;
        }
        _TaskEntry<object> entry = _taskQueue.first;
        if (schedulingStrategy(entry.priority, this))
        {
            try
            {
                _taskQueue.removeFirst();
                entry.run();
            }
            catch (Exception exception)
            {
                var exceptionStack = new System.Diagnostics.StackTrace();
                StackTrace? callbackStack = default!;
                DartRuntimePrimitives.Assert(() =>
                    {
                        callbackStack = entry.debugStack;
                        return true;
                    });
                FlutterError.reportError(new FlutterErrorDetails(exception: exception, stack: exceptionStack, library: "scheduler library", context: new ErrorDescription("during a task callback"), informationCollector: (((callbackStack is null)) ? null : (() =>
                {
                    return new List<DiagnosticsNode> { new DiagnosticsStackTrace("\nThis exception was thrown in the context of a scheduler callback. " + "When the scheduler callback was _registered_ (as opposed to when the " + "exception was thrown), this was the stack", callbackStack) };
                }))));
            }
            return (_taskQueue.Count != 0);
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long transientCallbackCount => _transientCallbacks.Count;
    public virtual long scheduleFrameCallback(Action<Duration> callback, bool rescheduling = false, bool scheduleNewFrame = true)
    {
        if (scheduleNewFrame)
        {
            scheduleFrame();
        }
        _nextFrameCallbackId += 1L;
        _transientCallbacks[_nextFrameCallbackId] = new _FrameCallbackEntry(callback, rescheduling: rescheduling);
        return _nextFrameCallbackId;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void cancelFrameCallbackWithId(long id)
    {
        DartRuntimePrimitives.Assert(() => (id > 0L));
        _transientCallbacks.remove(id);
        _removedIds.Add(id);
    }

    public virtual bool debugAssertNoTransientCallbacks(string reason)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((transientCallbackCount > 0L))
                {
                    long count__24307 = transientCallbackCount;
                    var callbacks__24353 = new DartMap<long, _FrameCallbackEntry>(_transientCallbacks);
                    FlutterError.reportError(new FlutterErrorDetails(exception: reason, library: "scheduler library", informationCollector: (() => new List<DiagnosticsNode>())));
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool debugAssertNoPendingPerformanceModeRequests(string reason)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((_performanceMode is not null))
                {
                    throw new FlutterError(reason);
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool debugAssertNoTimeDilation(string reason)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((BindingLibrary.timeDilation != 1.0))
                {
                    throw new FlutterError(reason);
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static void debugPrintTransientCallbackRegistrationStack()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((_FrameCallbackEntry.debugCurrentCallbackStack is not null))
                {
                    global::Doroti.Generated.Framework.Foundation.PrintLibrary.debugPrint("When the current transient callback was registered, this was the stack:");
                    global::Doroti.Generated.Framework.Foundation.PrintLibrary.debugPrint(string.Join("\n", FlutterError.defaultStackFilter(FlutterError.demangleStackTrace(_FrameCallbackEntry.debugCurrentCallbackStack!).ToString().trimRight().split("\n"))));
                }
                else
                {
                    global::Doroti.Generated.Framework.Foundation.PrintLibrary.debugPrint("No transient callback is currently executing.");
                }
                return true;
            });
    }

    public virtual void addPersistentFrameCallback(Action<Duration> callback)
    {
        _persistentCallbacks.Add(callback);
    }

    public virtual void addPostFrameCallback(Action<Duration> callback, string debugLabel = "callback")
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (global::Doroti.Generated.Framework.Scheduler.DebugLibrary.debugTracePostFrameCallbacks)
                {
                    var originalCallback__30444 = callback;
                    callback = ((timeStamp) =>
                    {
                        Timeline.startSync(debugLabel);
                        try
                        {
                            originalCallback__30444(timeStamp);
                        }
                        finally
                        {
                            Timeline.finishSync();
                        }
                    });
                }
                return true;
            });
        _postFrameCallbacks.Add(callback);
    }

    public virtual Future endOfFrame
    {
        get
        {
            if ((_nextFrameCompleter is null))
            {
                if ((object.Equals(schedulerPhase, SchedulerPhase.idle)))
                {
                    scheduleFrame();
                }
                _nextFrameCompleter = new Completer<object?>();
                addPostFrameCallback(((timeStamp) =>
                {
                    _nextFrameCompleter!.complete();
                    _nextFrameCompleter = null;
                }), debugLabel: "SchedulerBinding.completeFrame");
            }
            return _nextFrameCompleter!.future;
        }
    }
    public virtual bool hasScheduledFrame => _hasScheduledFrame;
    public virtual SchedulerPhase schedulerPhase => _schedulerPhase;
    public virtual bool framesEnabled => _framesEnabled;
    internal virtual void _setFramesEnabledState(bool enabled)
    {
        if ((_framesEnabled == enabled))
        {
            return;
        }
        _framesEnabled = enabled;
        if (enabled)
        {
            scheduleFrame();
        }
    }

    public virtual void ensureFrameCallbacksRegistered()
    {
        platformDispatcher.onBeginFrame ??= _handleBeginFrame;
        platformDispatcher.onDrawFrame ??= _handleDrawFrame;
    }

    public virtual void ensureVisualUpdate()
    {
        switch (schedulerPhase)
        {
            case var __case33516 when object.Equals(__case33516, SchedulerPhase.idle):
            case var __case33548 when object.Equals(__case33548, SchedulerPhase.postFrameCallbacks):
                {
                    scheduleFrame();
                    return;
                }
            case var __case33635 when object.Equals(__case33635, SchedulerPhase.transientCallbacks):
            case var __case33681 when object.Equals(__case33681, SchedulerPhase.midFrameMicrotasks):
            case var __case33727 when object.Equals(__case33727, SchedulerPhase.persistentCallbacks):
                {
                    return;
                }
        }
    }

    public virtual void scheduleFrame()
    {
        if ((_hasScheduledFrame || !framesEnabled))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if (global::Doroti.Generated.Framework.Scheduler.DebugLibrary.debugPrintScheduleFrameStacks)
                {
                    global::Doroti.Generated.Framework.Foundation.AssertionsLibrary.debugPrintStack(label: $"scheduleFrame() called. Current phase is {schedulerPhase}.");
                }
                return true;
            });
        ensureFrameCallbacksRegistered();
        platformDispatcher.scheduleFrame();
        _hasScheduledFrame = true;
    }

    public virtual void scheduleForcedFrame()
    {
        if (_hasScheduledFrame)
        {
            return;
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if (global::Doroti.Generated.Framework.Scheduler.DebugLibrary.debugPrintScheduleFrameStacks)
                {
                    global::Doroti.Generated.Framework.Foundation.AssertionsLibrary.debugPrintStack(label: $"scheduleForcedFrame() called. Current phase is {schedulerPhase}.");
                }
                return true;
            });
        ensureFrameCallbacksRegistered();
        platformDispatcher.scheduleFrame();
        _hasScheduledFrame = true;
    }

    public virtual void scheduleWarmUpFrame()
    {
        if ((_warmUpFrame || (!object.Equals(schedulerPhase, SchedulerPhase.idle))))
        {
            return;
        }
        _warmUpFrame = true;
        TimelineTask? debugTimelineTask = default!;
        if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            debugTimelineTask = ((Func<TimelineTask>)(() =>
{
    var __cascade = new TimelineTask();
    __cascade.start("Warm-up frame");
    return __cascade;
}))();
        }
        bool hadScheduledFrame = _hasScheduledFrame;
        PlatformDispatcher.instance.scheduleWarmUpFrame(beginFrame: (() =>
        {
            DartRuntimePrimitives.Assert(() => _warmUpFrame);
            handleBeginFrame(null);
        }), drawFrame: (() =>
        {
            DartRuntimePrimitives.Assert(() => _warmUpFrame);
            handleDrawFrame();
            resetEpoch();
            _warmUpFrame = false;
            if (hadScheduledFrame)
            {
                scheduleFrame();
            }
        }));
        _ = lockEvents((async () =>
        {
            await endOfFrame;
            if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                debugTimelineTask!.finish();
            }
        }));
    }

    public virtual void resetEpoch()
    {
        _epochStart = _adjustForEpoch(_lastRawTimeStamp);
        _firstRawTimeStampInEpoch = null;
    }

    internal virtual Duration _adjustForEpoch(Duration rawTimeStamp)
    {
        Duration rawDurationSinceEpoch = ((_firstRawTimeStampInEpoch is null) ? Duration.zero : (rawTimeStamp - DartRuntimePrimitives.RequireValue(_firstRawTimeStampInEpoch)));
        return new Duration(microseconds: (((rawDurationSinceEpoch.inMicroseconds / BindingLibrary.timeDilation)).round() + _epochStart.inMicroseconds));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Duration currentFrameTimeStamp
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (_currentFrameTimeStamp is not null));
            return DartRuntimePrimitives.RequireValue(_currentFrameTimeStamp);
        }
    }
    public virtual Duration currentSystemFrameTimeStamp
    {
        get
        {
            return _lastRawTimeStamp;
        }
    }
    internal virtual void _handleBeginFrame(Duration rawTimeStamp)
    {
        if (_warmUpFrame)
        {
            DartRuntimePrimitives.Assert(() => !_rescheduleAfterWarmUpFrame);
            _rescheduleAfterWarmUpFrame = true;
            return;
        }
        handleBeginFrame(rawTimeStamp);
    }

    internal virtual void _handleDrawFrame()
    {
        if (_rescheduleAfterWarmUpFrame)
        {
            _rescheduleAfterWarmUpFrame = false;
            addPostFrameCallback(((timeStamp) =>
            {
                _hasScheduledFrame = false;
                scheduleFrame();
            }), debugLabel: "SchedulerBinding.scheduleFrame");
            return;
        }
        handleDrawFrame();
    }

    public virtual void handleBeginFrame(Duration? rawTimeStamp)
    {
        _frameTimelineTask?.start("Frame");
        _firstRawTimeStampInEpoch ??= rawTimeStamp;
        _currentFrameTimeStamp = _adjustForEpoch((rawTimeStamp ?? _lastRawTimeStamp));
        if (rawTimeStamp is Duration rawTimeStamp__value47349)
        {
            _lastRawTimeStamp = DartRuntimePrimitives.RequireValue(rawTimeStamp__value47349);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                _debugFrameNumber += 1L;
                if ((global::Doroti.Generated.Framework.Scheduler.DebugLibrary.debugPrintBeginFrameBanner || global::Doroti.Generated.Framework.Scheduler.DebugLibrary.debugPrintEndFrameBanner))
                {
                    var frameTimeStampDescription__47549 = new StringBuffer();
                    if (rawTimeStamp is Duration rawTimeStamp__value47605)
                    {
                        _debugDescribeTimeStamp(DartRuntimePrimitives.RequireValue(_currentFrameTimeStamp), frameTimeStampDescription__47549);
                    }
                    else
                    {
                        frameTimeStampDescription__47549.write("(warm-up frame)");
                    }
                    _debugBanner = $"▄▄▄▄▄▄▄▄ Frame {_debugFrameNumber.ToString().padRight(7L)}   {frameTimeStampDescription__47549.ToString().padLeft(18L)} ▄▄▄▄▄▄▄▄";
                    if (global::Doroti.Generated.Framework.Scheduler.DebugLibrary.debugPrintBeginFrameBanner)
                    {
                        global::Doroti.Generated.Framework.Foundation.PrintLibrary.debugPrint(_debugBanner);
                    }
                }
                return true;
            });
        DartRuntimePrimitives.Assert(() => (object.Equals(schedulerPhase, SchedulerPhase.idle)));
        _hasScheduledFrame = false;
        try
        {
            _frameTimelineTask?.start("Animate");
            _schedulerPhase = SchedulerPhase.transientCallbacks;
            DartMap<long, _FrameCallbackEntry> callbacks__48364 = _transientCallbacks;
            _transientCallbacks = new DartMap<long, _FrameCallbackEntry>();
            callbacks__48364.forEach(((id, callbackEntry) =>
            {
                if (!_removedIds.Contains(id))
                {
                    _invokeFrameCallback(callbackEntry.callback, DartRuntimePrimitives.RequireValue(_currentFrameTimeStamp), callbackEntry.debugStack);
                }
            }));
            _removedIds.Clear();
        }
        finally
        {
            _schedulerPhase = SchedulerPhase.midFrameMicrotasks;
        }
    }

    public virtual PerformanceModeRequestHandle? requestPerformanceMode(DartPerformanceMode mode)
    {
        if (((_performanceMode is not null) && (!object.Equals(_performanceMode, mode))))
        {
            return null;
        }
        if ((object.Equals(_performanceMode, mode)))
        {
            DartRuntimePrimitives.Assert(() => (_numPerformanceModeRequests > 0L));
            _numPerformanceModeRequests++;
        }
        else
        {
            if ((_performanceMode is null))
            {
                DartRuntimePrimitives.Assert(() => (_numPerformanceModeRequests == 0L));
                _performanceMode = mode;
                _numPerformanceModeRequests = 1L;
            }
        }
        return new PerformanceModeRequestHandle(_disposePerformanceModeRequest);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _disposePerformanceModeRequest()
    {
        _numPerformanceModeRequests--;
        if ((_numPerformanceModeRequests == 0L))
        {
            _performanceMode = null;
            PlatformDispatcher.instance.requestDartPerformanceMode(DartPerformanceMode.balanced);
        }
    }

    public virtual DartPerformanceMode? debugGetRequestedPerformanceMode()
    {
        if (!((global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode || global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kProfileMode)))
        {
            return null;
        }
        else
        {
            return _performanceMode;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void handleDrawFrame()
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(_schedulerPhase, SchedulerPhase.midFrameMicrotasks)));
        _frameTimelineTask?.finish();
        try
        {
            _schedulerPhase = SchedulerPhase.persistentCallbacks;
            foreach (var callback in new List<Action<Duration>>(_persistentCallbacks))
            {
                _invokeFrameCallback(callback, DartRuntimePrimitives.RequireValue(_currentFrameTimeStamp));
            }
            _schedulerPhase = SchedulerPhase.postFrameCallbacks;
            var localPostFrameCallbacks__51803 = new List<Action<Duration>>(_postFrameCallbacks);
            _postFrameCallbacks.Clear();
            if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.startSync("POST_FRAME");
            }
            try
            {
                foreach (var callback in localPostFrameCallbacks__51803)
                {
                    _invokeFrameCallback(callback, DartRuntimePrimitives.RequireValue(_currentFrameTimeStamp));
                }
            }
            finally
            {
                if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
                {
                    FlutterTimeline.finishSync();
                }
            }
        }
        finally
        {
            _schedulerPhase = SchedulerPhase.idle;
            _frameTimelineTask?.finish();
            DartRuntimePrimitives.Assert(() =>
                {
                    if (global::Doroti.Generated.Framework.Scheduler.DebugLibrary.debugPrintEndFrameBanner)
                    {
                        global::Doroti.Generated.Framework.Foundation.PrintLibrary.debugPrint(DartCoreExtensions.repeat("▀", _debugBanner!.Length));
                    }
                    _debugBanner = null;
                    return true;
                });
            _currentFrameTimeStamp = null;
        }
    }

    internal virtual void _profileFramePostEvent(FrameTiming frameTiming)
    {
        postEvent("Flutter.Frame", new DartMap<string, object> { ["number"] = frameTiming.frameNumber, ["startTime"] = frameTiming.timestampInMicroseconds(FramePhase.buildStart), ["elapsed"] = frameTiming.totalSpan.inMicroseconds, ["build"] = frameTiming.buildDuration.inMicroseconds, ["raster"] = frameTiming.rasterDuration.inMicroseconds, ["vsyncOverhead"] = frameTiming.vsyncOverhead.inMicroseconds });
    }

    internal static void _debugDescribeTimeStamp(Duration timeStamp, StringBuffer buffer)
    {
        if ((timeStamp.inDays > 0L))
        {
            buffer.write($"{timeStamp.inDays}d ");
        }
        if ((timeStamp.inHours > 0L))
        {
            buffer.write($"{(timeStamp.inHours - (timeStamp.inDays * Duration.hoursPerDay))}h ");
        }
        if ((timeStamp.inMinutes > 0L))
        {
            buffer.write($"{(timeStamp.inMinutes - (timeStamp.inHours * Duration.minutesPerHour))}m ");
        }
        if ((timeStamp.inSeconds > 0L))
        {
            buffer.write($"{(timeStamp.inSeconds - (timeStamp.inMinutes * Duration.secondsPerMinute))}s ");
        }
        buffer.write($"{(timeStamp.inMilliseconds - (timeStamp.inSeconds * Duration.millisecondsPerSecond))}");
        long microseconds = (timeStamp.inMicroseconds - (timeStamp.inMilliseconds * Duration.microsecondsPerMillisecond));
        if ((microseconds > 0L))
        {
            buffer.write($".{microseconds.ToString().padLeft(3L, "0")}");
        }
        buffer.write("ms");
    }

    internal virtual void _invokeFrameCallback(Action<Duration> callback, Duration timeStamp, StackTrace? callbackStack = null)
    {
        DartRuntimePrimitives.Assert(() => (_FrameCallbackEntry.debugCurrentCallbackStack is null));
        DartRuntimePrimitives.Assert(() =>
            {
                _FrameCallbackEntry.debugCurrentCallbackStack = callbackStack;
                return true;
            });
        try
        {
            callback(timeStamp);
        }
        catch (Exception exception)
        {
            var exceptionStack = new System.Diagnostics.StackTrace();
            FlutterError.reportError(new FlutterErrorDetails(exception: exception, stack: exceptionStack, library: "scheduler library", context: new ErrorDescription("during a scheduler callback"), informationCollector: (((callbackStack is null)) ? null : (() =>
            {
                return new List<DiagnosticsNode> { new DiagnosticsStackTrace("\nThis exception was thrown in the context of a scheduler callback. " + "When the scheduler callback was _registered_ (as opposed to when the " + "exception was thrown), this was the stack", callbackStack) };
            }))));
        }
        DartRuntimePrimitives.Assert(() =>
            {
                _FrameCallbackEntry.debugCurrentCallbackStack = null;
                return true;
            });
    }

}

public static partial class BindingLibrary
{
    public static bool defaultSchedulingStrategy(long priority, SchedulerBinding scheduler)
    {
        if ((scheduler.transientCallbackCount > 0L))
        {
            return (priority >= Priority.animation.value);
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
