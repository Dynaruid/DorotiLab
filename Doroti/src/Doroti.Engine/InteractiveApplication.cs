using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Doroti.Composition;
using Doroti.Core;
using Doroti.Graphics;
using Doroti.Platform;
using Doroti.Rendering;
using Doroti.Widgets;

namespace Doroti.Engine;

public sealed record InteractiveTraceEvent(
    long Sequence,
    string Kind,
    ulong? DeviceId,
    ulong? FrameId,
    string Detail)
{
    public long TimestampTicks { get; init; }

    public int ThreadId { get; init; }

    public string? CorrelationId { get; init; }
}

public sealed record InteractiveTraceDocument(string SchemaVersion, InteractiveTraceEvent[] Events);

public sealed class InteractiveTraceRecorder
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
    };
    private readonly List<InteractiveTraceEvent> _events = [];
    private long _nextCorrelation;
    private string? _nextCorrelationId;

    public IReadOnlyList<InteractiveTraceEvent> Events => _events.ToArray();

    public void SetNextCorrelation(string correlationId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(correlationId);
        _nextCorrelationId = correlationId;
    }

    internal string ConsumeCorrelation() => Interlocked.Exchange(ref _nextCorrelationId, null)
        ?? $"input-{Interlocked.Increment(ref _nextCorrelation):D8}";

    public void Record(
        string kind,
        string detail,
        ulong? deviceId = null,
        ulong? frameId = null,
        string? correlationId = null) =>
        _events.Add(new(_events.Count + 1, kind, deviceId, frameId, detail)
        {
            TimestampTicks = Stopwatch.GetTimestamp(),
            ThreadId = Environment.CurrentManagedThreadId,
            CorrelationId = correlationId,
        });

    public void Write(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        var document = new InteractiveTraceDocument("doroti.interactive-trace/v1", _events.ToArray());
        File.WriteAllText(path, JsonSerializer.Serialize(document, JsonOptions).Replace("\r\n", "\n", StringComparison.Ordinal) + "\n");
    }

    public static InteractiveTraceDocument Replay(string path)
    {
        var document = JsonSerializer.Deserialize<InteractiveTraceDocument>(File.ReadAllText(path), JsonOptions)
            ?? throw new InvalidDataException("Interactive trace is empty.");
        if (document.SchemaVersion != "doroti.interactive-trace/v1")
        {
            throw new InvalidDataException($"Unsupported interactive trace schema {document.SchemaVersion}.");
        }
        if (!document.Events.Select(item => item.Sequence).SequenceEqual(Enumerable.Range(1, document.Events.Length).Select(value => (long)value)))
        {
            throw new InvalidDataException("Interactive trace sequence is not contiguous.");
        }
        return document;
    }
}

public sealed class InputDispatcher
{
    private readonly Func<RenderBox?> _root;
    private readonly FlutterArenaAdapter _arena = new();
    private readonly PointerSignalResolver _pointerSignalResolver = new();
    private readonly Dictionary<ulong, RouteEntry[]> _capturedPaths = [];
    private readonly InteractiveTraceRecorder _trace;
    private readonly IFrameDispatcher? _frameDispatcher;
    private IKeyboardEventTarget? _keyboardTarget;

    public InputDispatcher(Func<RenderBox?> root, InteractiveTraceRecorder trace, IFrameDispatcher? frameDispatcher = null)
    {
        _root = root ?? throw new ArgumentNullException(nameof(root));
        _trace = trace ?? throw new ArgumentNullException(nameof(trace));
        _frameDispatcher = frameDispatcher;
    }

    public int CapturedPathCount => _capturedPaths.Count;

    public string? LastCorrelationId { get; private set; }

    public void Dispatch(RawPointerEvent raw)
    {
        var correlationId = _trace.ConsumeCorrelation();
        LastCorrelationId = correlationId;
        var phase = raw.Phase switch
        {
            PointerPhase.Added => PointerEventPhase.Added,
            PointerPhase.Hover => raw.ScrollDelta == Offset.Zero ? PointerEventPhase.Hover : PointerEventPhase.Wheel,
            PointerPhase.Down => PointerEventPhase.Down,
            PointerPhase.Move => raw.ScrollDelta == Offset.Zero ? PointerEventPhase.Move : PointerEventPhase.Wheel,
            PointerPhase.Up => PointerEventPhase.Up,
            PointerPhase.Removed => PointerEventPhase.Removed,
            _ => PointerEventPhase.Cancelled,
        };
        var pointerDetail = $"{phase}:{raw.Position.X:0.###},{raw.Position.Y:0.###}:buttons={raw.Buttons}:wheel={raw.ScrollDelta.Y:0.###}";
        _trace.Record("raw-pointer", pointerDetail, raw.DeviceId, correlationId: correlationId);
        _trace.Record("normalized-pointer-signal", pointerDetail, raw.DeviceId, correlationId: correlationId);

        RouteEntry[] route;
        if (phase is PointerEventPhase.Down)
        {
            route = HitTest(raw.Position);
            _capturedPaths[raw.DeviceId] = route;
            _keyboardTarget = route.Select(item => item.Target).OfType<IKeyboardEventTarget>().FirstOrDefault();
            route.Select(item => item.Target).OfType<IFocusableKeyboardTarget>().FirstOrDefault()?.RequestFocus();
        }
        else if (_capturedPaths.TryGetValue(raw.DeviceId, out var captured))
        {
            route = captured;
        }
        else
        {
            route = HitTest(raw.Position);
        }

        _trace.Record("route", string.Join(">", route.Select(item => item.Target.GetType().Name)), raw.DeviceId, correlationId: correlationId);
        PointerScrollEvent? scrollEvent = null;
        if (phase is PointerEventPhase.Wheel)
        {
            scrollEvent = new(raw.DeviceId, raw.Position, raw.ScrollDelta, raw.Timestamp);
        }
        foreach (var item in route)
        {
            if (_frameDispatcher is not null && item.Target is IFrameDrivenScrollTarget frameDriven)
            {
                frameDriven.BindScrollFrameDispatcher(_frameDispatcher);
            }
            if (item.Target is IFlutterArenaTarget detector)
            {
                detector.BindFlutterArena(_arena, detail => _trace.Record("gesture", detail, raw.DeviceId, correlationId: correlationId));
            }
            item.Target.HandlePointerEvent(new(
                raw.DeviceId,
                phase,
                raw.Position,
                item.LocalAtDown + (raw.Position - item.GlobalAtDown),
                raw.Buttons,
                raw.ScrollDelta,
                raw.Timestamp,
                raw.DeviceKind));
            if (scrollEvent is not null && item.Target is IPointerSignalTarget signalTarget)
            {
                signalTarget.RegisterPointerSignal(scrollEvent, _pointerSignalResolver);
            }
        }
        if (scrollEvent is not null)
        {
            var handled = _pointerSignalResolver.Resolve(scrollEvent);
            _trace.Record("pointer-signal-resolved", $"handled={handled};dx={raw.ScrollDelta.X:0.###};dy={raw.ScrollDelta.Y:0.###}", raw.DeviceId, correlationId: correlationId);
        }
        if (phase is PointerEventPhase.Down)
        {
            _arena.Close(raw.DeviceId);
            _arena.FlushMicrotasks();
        }
        else if (phase is PointerEventPhase.Cancelled or PointerEventPhase.Removed)
        {
            _arena.FlushMicrotasks();
            _capturedPaths.Remove(raw.DeviceId);
        }
        else if (phase is PointerEventPhase.Up && raw.Buttons == 0)
        {
            _arena.Sweep(raw.DeviceId);
            _arena.FlushMicrotasks();
            _capturedPaths.Remove(raw.DeviceId);
        }
    }

    public void Dispatch(RawKeyEvent raw)
    {
        var phase = raw.Phase switch
        {
            KeyPhase.Down => KeyboardEventPhase.Down,
            KeyPhase.Repeat => KeyboardEventPhase.Repeat,
            _ => KeyboardEventPhase.Up,
        };
        var handled = _keyboardTarget?.HandleKeyboardEvent(new(
            raw.PhysicalKey,
            raw.LogicalKey,
            phase,
            raw.Timestamp,
            (KeyboardModifiers)raw.Modifiers)) is true;
        _trace.Record("raw-key", $"{phase}:physical={raw.PhysicalKey}:logical={raw.LogicalKey}:handled={handled}");
    }

    public void CancelAll(TimeSpan timestamp)
    {
        foreach (var deviceId in _capturedPaths.Keys.ToArray())
        {
            foreach (var item in _capturedPaths[deviceId])
            {
                item.Target.HandlePointerEvent(new(
                    deviceId,
                    PointerEventPhase.Cancelled,
                    item.GlobalAtDown,
                    item.LocalAtDown,
                    0,
                    Offset.Zero,
                    timestamp));
            }
            _arena.FlushMicrotasks();
            _trace.Record("route-cancel", "window-lifecycle", deviceId);
        }
        _capturedPaths.Clear();
    }

    private RouteEntry[] HitTest(Offset position)
    {
        var root = _root();
        if (root is null)
        {
            return [];
        }
        var result = new HitTestResult();
        if (!root.HitTest(result, position))
        {
            return [];
        }
        return result.Path
            .Where(item => item.Target is IPointerEventTarget)
            .Select(item => new RouteEntry((IPointerEventTarget)item.Target, position, item.LocalPosition))
            .ToArray();
    }

    private sealed record RouteEntry(IPointerEventTarget Target, Offset GlobalAtDown, Offset LocalAtDown);
}

public sealed record InteractiveFrameTiming(
    FrameId FrameId,
    string? CorrelationId,
    int PixelWidth,
    int PixelHeight,
    TimeSpan Build,
    TimeSpan Layout,
    TimeSpan CompositingBits,
    TimeSpan Semantics,
    TimeSpan Paint,
    TimeSpan Commit,
    long UiThreadAllocatedBytes);

public sealed class InteractiveApplication : IRawInputSink, IWidgetHost, IDisposable
{
    private readonly IWindow _window;
    private readonly IInteractiveFrameSink _frameSink;
    private readonly BuildOwner _buildOwner;
    private readonly PipelineOwner _pipelineOwner;
    private readonly RenderView _view;
    private readonly InputDispatcher _input;
    private readonly IAccessibilityBridge? _accessibility;
    private readonly QueuedUiDispatcher _uiDispatcher;
    private Widget _root;
    private ulong _nextFrameId = 1;
    private readonly List<InteractiveFrameTiming> _frameTimings = [];
    private string? _pendingCorrelationId;
    private bool _disposed;

    public InteractiveApplication(
        IWindow window,
        Widget root,
        IInteractiveFrameSink frameSink,
        InteractiveTraceRecorder? trace = null,
        QueuedUiDispatcher? uiDispatcher = null)
    {
        _window = window ?? throw new ArgumentNullException(nameof(window));
        _root = root ?? throw new ArgumentNullException(nameof(root));
        _frameSink = frameSink ?? throw new ArgumentNullException(nameof(frameSink));
        _uiDispatcher = uiDispatcher ?? new();
        _uiDispatcher.WorkScheduled += HandleUiWorkScheduled;
        Trace = trace ?? new();
        _buildOwner = new(RequestFrame);
        _pipelineOwner = new(RequestFrame);
        _buildOwner.Mount(_root);
        var metrics = window.Metrics;
        _view = new(new(metrics.LogicalSize, metrics.PixelSize, metrics.ScaleFactor, metrics.SurfaceGeneration), GetRootBox());
        _pipelineOwner.SetRoot(_view);
        _window.TryGetFeature<IFrameDispatcher>(out var frameDispatcher);
        _input = new(() => _view, Trace, frameDispatcher);
        _window.TryGetFeature<IAccessibilityBridge>(out _accessibility);
        window.RawInput.Attach(this);
        NeedsFrame = true;
    }

    public InteractiveTraceRecorder Trace { get; }

    public event Action? FrameRequested;

    public bool NeedsFrame { get; private set; }

    public ulong PresentedFrameCount { get; private set; }

    public IReadOnlyList<InteractiveFrameTiming> FrameTimings => _frameTimings.ToArray();

    public BuildOwner BuildOwner => _buildOwner;

    public PipelineOwner PipelineOwner => _pipelineOwner;

    public QueuedUiDispatcher UiDispatcher => _uiDispatcher;

    public Widget Root
    {
        get => _root;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _root = value;
            _buildOwner.UpdateRoot(value);
            RequestFrame();
        }
    }

    IWidget IWidgetHost.Root
    {
        get => Root;
        set => Root = value as Widget ?? throw new ArgumentException("InteractiveApplication requires a Doroti Widget root.", nameof(value));
    }

    public FrameAckStatus PumpFrame()
    {
        var pending = SubmitFrame();
        var result = pending.Completion.GetAwaiter().GetResult();
        CompleteFrame(pending, result);
        return result.Status;
    }

    /// <summary>
    /// Builds and commits on the UI thread, then returns immediately after mailbox submission.
    /// Terminal ACK bookkeeping is posted back through the engine UI dispatcher.
    /// </summary>
    public Task<FrameAckResult> PumpFrameNonBlocking()
    {
        var pending = SubmitFrame();
        _ = pending.Completion.ContinueWith(
            static (task, state) =>
            {
                var completion = (PendingCompletion)state!;
                completion.Application._uiDispatcher.Post(() =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        completion.Application.CompleteFrame(completion.Frame, task.Result);
                    }
                    else
                    {
                        var failure = task.Exception?.GetBaseException() ?? new TaskCanceledException(task);
                        completion.Application.CompleteFrame(
                            completion.Frame,
                            new(completion.Frame.FrameId, FrameAckStatus.Failed, failure.Message, FrameFaultKind.FatalBackend));
                    }
                });
            },
            new PendingCompletion(this, pending),
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);
        return pending.Completion;
    }

    private PendingFrame SubmitFrame()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        NeedsFrame = false;
        var allocationStart = GC.GetAllocatedBytesForCurrentThread();
        _uiDispatcher.Drain();
        var metrics = _window.Metrics;
        _view.Configuration = new(metrics.LogicalSize, metrics.PixelSize, metrics.ScaleFactor, metrics.SurfaceGeneration);
        var buildStart = Stopwatch.GetTimestamp();
        _buildOwner.BuildScope();
        var build = Stopwatch.GetElapsedTime(buildStart);
        var root = GetRootBox();
        if (!ReferenceEquals(_view.Child, root))
        {
            _view.Child = root;
        }
        var frame = _pipelineOwner.FlushFrame();
        if (_pipelineOwner.SemanticsOwner.Snapshot is { } semantics)
        {
            _accessibility?.Update(semantics, request =>
                _pipelineOwner.SemanticsOwner.PerformAction(request.NodeId, request.Action));
        }
        var frameId = new FrameId(_nextFrameId++);
        var pipelineTiming = _pipelineOwner.LastFrameTiming;
        var correlationId = _pendingCorrelationId;
        var pixelWidth = checked((int)frame.Configuration.PixelSize.Width);
        var pixelHeight = checked((int)frame.Configuration.PixelSize.Height);
        Trace.Record("build", $"microseconds={build.TotalMicroseconds:0.###};dirty={_buildOwner.DirtyElementCount}", frameId: frameId.Value, correlationId: correlationId);
        Trace.Record("layout", $"microseconds={pipelineTiming.Layout.TotalMicroseconds:0.###}", frameId: frameId.Value, correlationId: correlationId);
        Trace.Record("paint", $"microseconds={pipelineTiming.Paint.TotalMicroseconds:0.###}", frameId: frameId.Value, correlationId: correlationId);
        Trace.Record("commit", $"microseconds={pipelineTiming.Commit.TotalMicroseconds:0.###};snapshot={frame.Snapshot.DisplayListBytes}", frameId: frameId.Value, correlationId: correlationId);
        Trace.Record("layout-paint-commit", $"snapshot={frame.Snapshot.DisplayListBytes}", frameId: frameId.Value, correlationId: correlationId);
        Task<FrameAckResult> completion;
        if (_frameSink is IAsyncInteractiveFrameSink asynchronous)
        {
            completion = asynchronous.PresentAsync(frameId, frame).AsTask();
        }
        else
        {
            var status = _frameSink.Present(frameId, frame);
            completion = Task.FromResult(new FrameAckResult(frameId, status));
        }
        var allocated = GC.GetAllocatedBytesForCurrentThread() - allocationStart;
        _frameTimings.Add(new(
            frameId,
            correlationId,
            pixelWidth,
            pixelHeight,
            build,
            pipelineTiming.Layout,
            pipelineTiming.CompositingBits,
            pipelineTiming.Semantics,
            pipelineTiming.Paint,
            pipelineTiming.Commit,
            allocated));
        _pendingCorrelationId = null;
        _buildOwner.FinalizeTree();
        return new(frameId, correlationId, completion);
    }

    private void CompleteFrame(PendingFrame pending, FrameAckResult result)
    {
        if (Interlocked.Exchange(ref pending.Completed, 1) != 0)
        {
            return;
        }
        Trace.Record("ack", result.Status.ToString(), frameId: pending.FrameId.Value, correlationId: pending.CorrelationId);
        if (result.Status is FrameAckStatus.Presented)
        {
            PresentedFrameCount++;
        }
        else if (!_disposed)
        {
            RequestFrame();
        }
    }

    public void OnPointer(RawPointerEvent input)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _input.Dispatch(input);
        _pendingCorrelationId = _input.LastCorrelationId;
    }

    public void OnKey(RawKeyEvent input)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _input.Dispatch(input);
    }

    public void OnFocus(RawFocusEvent input)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        Trace.Record("focus", input.IsFocused ? "gained" : "lost");
        if (!input.IsFocused)
        {
            _input.CancelAll(input.Timestamp);
        }
    }

    public void CancelInput(TimeSpan timestamp) => _input.CancelAll(timestamp);

    public void OnMetricsChanged()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_window.Metrics.IsMinimized)
        {
            _input.CancelAll(TimeSpan.Zero);
        }
        if (_window.Metrics.IsMinimized)
        {
            NeedsFrame = false;
        }
        else
        {
            RequestFrame();
        }
        Trace.Record("metrics", $"{_window.Metrics.LogicalSize.Width:0.###}x{_window.Metrics.LogicalSize.Height:0.###}@{_window.Metrics.ScaleFactor:0.###}");
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        _window.RawInput.Detach(this);
        _uiDispatcher.WorkScheduled -= HandleUiWorkScheduled;
        _input.CancelAll(TimeSpan.Zero);
        _buildOwner.UnmountRoot();
    }

    private RenderBox GetRootBox() => _buildOwner.RootRenderObject as RenderBox
        ?? throw new InvalidOperationException("The interactive Widget tree must produce a RenderBox root.");

    private void HandleUiWorkScheduled() => RequestFrame();

    private void RequestFrame()
    {
        if (NeedsFrame)
        {
            return;
        }
        NeedsFrame = true;
        FrameRequested?.Invoke();
    }

    private sealed class PendingFrame(FrameId frameId, string? correlationId, Task<FrameAckResult> completion)
    {
        internal FrameId FrameId { get; } = frameId;

        internal string? CorrelationId { get; } = correlationId;

        internal Task<FrameAckResult> Completion { get; } = completion;

        internal int Completed;
    }

    private sealed record PendingCompletion(InteractiveApplication Application, PendingFrame Frame);
}
