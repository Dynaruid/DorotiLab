using System.Reflection;
using Doroti.Framework.Foundation;
using Doroti.Framework.Scheduler;
using Doroti.Framework.Services;
using Doroti.Runtime;
using Doroti.Ui;

if (args.SequenceEqual(["--api-snapshot"]))
{
    var visibility = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
        | BindingFlags.Static | BindingFlags.DeclaredOnly;
    foreach (var type in typeof(DorotiExecutionContext).Assembly.GetTypes()
        .Where(type => type.Namespace == "Doroti.Runtime" && IsVisible(type))
        .OrderBy(type => type.FullName, StringComparer.Ordinal))
    {
        Console.WriteLine($"TYPE {type.FullName}");
        foreach (var member in type.GetMembers(visibility)
            .Where(IsVisibleMember)
            .OrderBy(member => member.ToString(), StringComparer.Ordinal))
        {
            Console.WriteLine($"  {member.MemberType} {member}");
        }
    }
    return;
}

if (args.Length != 0)
{
    throw new ArgumentException($"Unknown argument: {string.Join(' ', args)}");
}

var checks = 0;
void Check(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
    checks++;
}

void ExpectOverflow(Action action, string message)
{
    try
    {
        action();
    }
    catch (OverflowException)
    {
        checks++;
        return;
    }
    throw new InvalidOperationException(message);
}

Check(Duration.zero.inMicroseconds == 0, "Duration zero");
Check(Duration.Create(days: 1, hours: 2, minutes: 3, seconds: 4,
    milliseconds: 5, microseconds: 6).inMicroseconds == 93_784_005_006,
    "Duration named units combine in microseconds");
Check(Duration.Create(microseconds: -1).inMilliseconds == 0,
    "negative sub-millisecond duration truncates toward zero");
Check(((TimeSpan)Duration.Create(microseconds: 1)).Ticks == 10,
    "one microsecond is ten TimeSpan ticks");
Check(((Duration)TimeSpan.FromTicks(-19)).inMicroseconds == -1,
    "TimeSpan conversion truncates negative sub-microsecond ticks");
Check(((Duration)TimeSpan.FromTicks(9)).inMicroseconds == 0,
    "TimeSpan conversion truncates positive sub-microsecond ticks");
var largestTimeSpanDuration = new Duration(TimeSpan.MaxValue.Ticks / 10);
Check(((TimeSpan)largestTimeSpanDuration).Ticks == TimeSpan.MaxValue.Ticks - 7,
    "largest convertible Duration preserves whole microseconds");
ExpectOverflow(() => _ = (TimeSpan)new Duration((TimeSpan.MaxValue.Ticks / 10) + 1),
    "Duration beyond TimeSpan range must overflow");
ExpectOverflow(() => _ = Duration.Create(days: long.MaxValue),
    "Duration named-unit overflow must be reported");

var orderedMap = new DartMap<string?, int>();
orderedMap[null] = 1;
orderedMap["first"] = 2;
orderedMap["second"] = 3;
orderedMap["first"] = 4;
Check(orderedMap.Keys.SequenceEqual([null, "first", "second"]),
    "null keys and updates retain map insertion order");
Check(orderedMap.TryGetValue(null, out var nullKeyValue) && nullKeyValue == 1,
    "null-key lookup preserves its value");
Check(orderedMap.Remove("first") && orderedMap.Count == 2,
    "map removal removes exactly one entry");
orderedMap["first"] = 5;
Check(orderedMap.Keys.SequenceEqual([null, "second", "first"]),
    "a reinserted key goes to the end");

var sharedBytes = new ByteBuffer([0x01, 0x02, 0x03, 0x04]);
var byteView = sharedBytes.asUint8List(1, 2);
byteView[0] = 0xFE;
Check(sharedBytes.asUint8List()[1] == 0xFE, "byte view writes through to its buffer");
var copiedBytes = byteView.ToByteArray();
byteView[1] = 0xAA;
Check(copiedBytes.SequenceEqual(new byte[] { 0xFE, 0x03 }),
    "byte copy is independent of its view");
var byteData = sharedBytes.asByteData(1, 2);
var memoryCopy = byteData.asMemory();
byteView[0] = 0xBB;
Check(memoryCopy.Span.SequenceEqual(new byte[] { 0xFE, 0xAA }),
    "ByteData memory conversion returns an independent copy");
Check(new ByteBuffer([0x01, 0x02, 0x03, 0x04]).asByteData().getUint32(0)
    == 0x0102_0304, "ByteData uint32 uses big-endian order");
Check(new ByteBuffer([0x01, 0x02, 0x03, 0x04]).asUint32List()[0]
    == 0x0403_0201, "ByteBuffer uint32 view uses little-endian order");

var matrix = Matrix4.identity();
Check(matrix.storage.Length == 16, "Matrix4 exposes sixteen double storage entries");
matrix.storage[12] = 3;
Check(matrix.getTranslation().x == 3, "Matrix4 storage remains mutable through its view");
var copiedMatrix = matrix.clone();
matrix.storage[12] = 4;
Check(copiedMatrix.getTranslation().x == 3, "Matrix4 clone owns an independent array");

var standardCodec = new StandardMessageCodec();
var encodedInt32 = standardCodec.encodeMessage(new int[] { 0x0102_0304, -2 })!;
Check(encodedInt32.asMemory().Span[0] == 9
    && standardCodec.decodeMessage(encodedInt32) is int[] decodedInt32
    && decodedInt32.SequenceEqual(new int[] { 0x0102_0304, -2 }),
    "codec int32 array tag and values");
var encodedInt64 = standardCodec.encodeMessage(new long[] { long.MaxValue, -3 })!;
Check(encodedInt64.asMemory().Span[0] == 10
    && standardCodec.decodeMessage(encodedInt64) is long[] decodedInt64
    && decodedInt64.SequenceEqual(new long[] { long.MaxValue, -3 }),
    "codec int64 array tag and values");
var encodedFloat32 = standardCodec.encodeMessage(new float[] { 1.25f, -2f })!;
Check(encodedFloat32.asMemory().Span[0] == 14
    && standardCodec.decodeMessage(encodedFloat32) is float[] decodedFloat32
    && decodedFloat32.SequenceEqual(new float[] { 1.25f, -2f }),
    "codec float32 array tag and values");
var encodedFloat64 = standardCodec.encodeMessage(new double[] { 1.25, -2 })!;
Check(encodedFloat64.asMemory().Span[0] == 11
    && standardCodec.decodeMessage(encodedFloat64) is double[] decodedFloat64
    && decodedFloat64.SequenceEqual(new double[] { 1.25, -2 }),
    "codec float64 array tag and values");
var wire = new WriteBuffer();
wire.putUint8(9);
wire.putUint8(2);
wire.putInt32List(new int[] { 0x0102_0304, -2 }, Endian.little);
Check(wire.done().Span.SequenceEqual(new byte[] {
    9, 2, 0, 0, 4, 3, 2, 1, 0xFE, 0xFF, 0xFF, 0xFF
}), "codec int32 little-endian fixture and four-byte alignment");

var firstClock = new ContractTimeProvider();
var secondClock = new ContractTimeProvider();
Check(ReferenceEquals(DorotiExecutionContext.TimeProvider, TimeProvider.System), "native default time");
using (DorotiExecutionContext.EnterTimeProvider(firstClock))
{
    Check(ReferenceEquals(DorotiExecutionContext.TimeProvider, firstClock), "first time scope");
    using (DorotiExecutionContext.EnterTimeProvider(secondClock))
    {
        Check(ReferenceEquals(DorotiExecutionContext.TimeProvider, secondClock), "nested time scope");
        Check(ReferenceEquals(DartAsyncRuntime.timeProvider, secondClock), "legacy time bridge");
    }
    Check(ReferenceEquals(DorotiExecutionContext.TimeProvider, firstClock), "time scope restore");
}
Check(ReferenceEquals(DorotiExecutionContext.TimeProvider, TimeProvider.System), "time scope exit");
Check(
    DorotiRandom.FromSeed(1L << 40).NextDouble()
        == DorotiRandom.FromSeed(1L << 40).NextDouble(),
    "64-bit seed is reproducible"
);
Check(
    DorotiRandom.FromSeed(0).NextDouble() != DorotiRandom.FromSeed(1L << 40).NextDouble(),
    "upper seed bits affect the sequence"
);

using var dispatcher = new PlatformDispatcher();
var firstHost = new ContractViewHost(1);
using var firstView = dispatcher.RegisterView(
    1,
    new DorotiViewCapabilities("runtime-dotnet")
        .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, firstHost)
        .Register<IFrameHostCapability>(DorotiCapabilityIds.ViewFrameDispatch, firstHost)
);
var secondHost = new ContractViewHost(2);
using var secondView = dispatcher.RegisterView(
    2,
    new DorotiViewCapabilities("runtime-dotnet")
        .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, secondHost)
        .Register<IFrameHostCapability>(DorotiCapabilityIds.ViewFrameDispatch, secondHost)
);

DorotiCallbackDispatcher? firstCapture = null;
var order = new List<string>();
firstView.DispatchPlatformEvent(() =>
{
    firstCapture = DorotiExecutionContext.CaptureDispatcher();
    Check(firstCapture is not null, "view callback has a dispatcher");
    firstCapture!.TryPost(() =>
    {
        order.Add("first");
        firstCapture!.TryPost(() => order.Add("reentrant"));
    });
    DartAsyncRuntime.scheduleMicrotask(() => order.Add("legacy"));
    Check(order.Count == 0, "callbacks remain queued until event exit");
});
Check(order.SequenceEqual(["first", "legacy", "reentrant"]), "FIFO and reentrant drain");
Check(DorotiExecutionContext.CaptureDispatcher() is null, "dispatcher scope restores ambient state");

var delivered = false;
var completion = firstCapture!.PostAsync(() => delivered = true);
Check(!completion.IsCompleted && firstHost.PendingFrames > 0, "later callback requests a frame");
firstHost.Pump();
await completion;
Check(delivered, "captured callback runs in a host frame");

DorotiCallbackDispatcher? secondCapture = null;
secondView.DispatchPlatformEvent(() =>
    secondCapture = DorotiExecutionContext.CaptureDispatcher()
);
Check(!ReferenceEquals(firstCapture, secondCapture), "view callbacks capture separate lifetimes");
firstView.Dispose();
Check(!firstCapture.TryPost(() => throw new Exception("stale view callback")), "closed view rejects work");
try
{
    await firstCapture.PostAsync(() => throw new Exception("stale view callback"));
    throw new InvalidOperationException("Closed view task was not canceled.");
}
catch (TaskCanceledException)
{
    checks++;
}

var faulted = secondCapture!.PostAsync(() => throw new InvalidOperationException("callback failure"));
secondHost.Pump();
try
{
    await faulted;
    throw new InvalidOperationException("Callback failure was not observed.");
}
catch (InvalidOperationException error) when (error.Message == "callback failure")
{
    checks++;
}

var staleDelivered = false;
var pending = secondCapture.PostAsync(() => staleDelivered = true);
Check(!pending.IsCompleted && secondHost.PendingFrames > 0, "post is pending before close");
secondView.Dispose();
try
{
    await pending;
    throw new InvalidOperationException("Pending callback was not canceled.");
}
catch (TaskCanceledException)
{
    checks++;
}
Check(!staleDelivered, "queued callback is suppressed after view close");

using var outerDispatcher = new PlatformDispatcher();
using var innerDispatcher = new PlatformDispatcher();
using (outerDispatcher.EnterScope())
{
    Check(ReferenceEquals(PlatformDispatcher.instance, outerDispatcher), "outer dispatcher scope");
    using (innerDispatcher.EnterScope())
    {
        Check(ReferenceEquals(PlatformDispatcher.instance, innerDispatcher), "nested dispatcher scope");
    }
    Check(ReferenceEquals(PlatformDispatcher.instance, outerDispatcher), "dispatcher scope restore");
}

var timerClock = new ManualTimeProvider();
using (DorotiExecutionContext.EnterTimeProvider(timerClock))
{
    using var timerDispatcher = new PlatformDispatcher();
    var timerHost = new ContractViewHost(3);
    using var timerView = timerDispatcher.RegisterView(
        3,
        new DorotiViewCapabilities("runtime-dotnet")
            .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, timerHost)
            .Register<IFrameHostCapability>(DorotiCapabilityIds.ViewFrameDispatch, timerHost)
    );
    var timerFired = false;
    Doroti.Runtime.Timer? timer = null;
    timerView.DispatchPlatformEvent(() =>
        timer = new Doroti.Runtime.Timer(Duration.Create(milliseconds: 1), () => timerFired = true)
    );
    timerClock.Fire();
    Check(timerHost.PendingFrames > 0, "timer callback queues a host frame");
    timer!.cancel();
    timerHost.Pump();
    Check(!timerFired, "cancellation suppresses an already queued timer callback");

    using var scheduler = new ContractSchedulerBinding(timerDispatcher);
    var taskOrder = new List<int>();
    var idleTask = scheduler.scheduleTask<int>(
        new Func<int>(() => { taskOrder.Add(1); return 1; }), Priority.idle
    );
    var touchTask = scheduler.scheduleTask<int>(
        new Func<int>(() => { taskOrder.Add(3); return 3; }), Priority.touch
    );
    var animationTask = scheduler.scheduleTask<int>(
        new Func<int>(() => { taskOrder.Add(2); return 2; }), Priority.animation
    );
    scheduler.handleEventLoopCallback();
    scheduler.handleEventLoopCallback();
    scheduler.handleEventLoopCallback();
    Check(taskOrder.SequenceEqual([3, 2, 1]), "scheduler dequeues highest priority first");
    Check(await idleTask == 1 && await touchTask == 3 && await animationTask == 2,
        "generic task results survive type-erased queueing");
}

Console.WriteLine($"runtime-dotnet contracts: {checks} assertions passed.");

static bool IsVisible(Type type) =>
    type.IsPublic || type.IsNotPublic || type.IsNestedPublic || type.IsNestedAssembly;

static bool IsVisibleMember(MemberInfo member) => member switch
{
    MethodBase method => method.IsPublic || method.IsAssembly || method.IsFamilyOrAssembly,
    FieldInfo field => field.IsPublic || field.IsAssembly || field.IsFamilyOrAssembly,
    PropertyInfo property => property.GetAccessors(nonPublic: true).Any(method => IsVisibleMember(method)),
    EventInfo eventInfo => eventInfo.GetAddMethod(nonPublic: true) is { } method && IsVisibleMember(method),
    Type nested => IsVisible(nested),
    _ => false,
};

sealed class ContractTimeProvider : TimeProvider;

sealed class ContractSchedulerBinding(PlatformDispatcher dispatcher) : SchedulerBinding(dispatcher);

sealed class ManualTimeProvider : TimeProvider
{
    private ManualTimer? _timer;

    public override ITimer CreateTimer(
        TimerCallback callback,
        object? state,
        TimeSpan dueTime,
        TimeSpan period
    )
    {
        _timer = new ManualTimer(callback, state);
        _timer.Change(dueTime, period);
        return _timer;
    }

    public void Fire() => (_timer ?? throw new InvalidOperationException("No timer was created.")).Fire();

    private sealed class ManualTimer(TimerCallback callback, object? state) : ITimer
    {
        private bool _disposed;
        public bool Change(TimeSpan dueTime, TimeSpan period) => !_disposed;
        public void Fire()
        {
            if (!_disposed)
            {
                callback(state);
            }
        }
        public void Dispose() => _disposed = true;
        public ValueTask DisposeAsync()
        {
            Dispose();
            return ValueTask.CompletedTask;
        }
    }
}

sealed class ContractViewHost(ulong viewId) : IViewHostCapability, IFrameHostCapability
{
    private readonly Queue<Action<TimeSpan>> _frames = new();

    public ViewMetrics Metrics { get; } = new(
        new Size(100, 100), 1,
        ViewPadding.zero, ViewPadding.zero, ViewPadding.zero,
        AppLifecycleState.resumed, 1, 1
    );
    public DorotiViewEpoch ViewEpoch { get; } = new(viewId, 1, 1, 100, 100, 100, 100, 1, 1, 0);
    public int PendingFrames => _frames.Count;

    public event Action<ViewMetrics>? MetricsChanged { add { } remove { } }
    public event Action<AppLifecycleState>? LifecycleChanged { add { } remove { } }
    public event Action? CloseRequested { add { } remove { } }
    public event Action? Closed { add { } remove { } }

    public void ScheduleFrame(Action<TimeSpan> callback) => _frames.Enqueue(callback);
    public void Pump() => _frames.Dequeue()(TimeSpan.Zero);
    public void Show() { }
    public void Resize(Size logicalSize) => throw new NotSupportedException();
    public void Close() => _frames.Clear();
    public void Dispose() => _frames.Clear();
}
