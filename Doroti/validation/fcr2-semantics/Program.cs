using System.Numerics;
using Doroti.Framework.Animation;
using Doroti.Runtime;
using Doroti.Ui;
using Timer = Doroti.Runtime.Timer;

var diagnostics = new List<DartFutureDiagnostic>();
var previousSink = DartRuntimePrimitives.FutureDiagnosticSink;
DartRuntimePrimitives.FutureDiagnosticSink = diagnostics.Add;
try
{
    VerifyTypedTweenArithmetic();
    VerifyNullableAndAsyncSemantics(diagnostics);
    VerifyTimerDisposeRace();
    VerifyCollectionAndPatternSemantics();
    VerifyAssertConfiguration();
    Console.WriteLine($"FCR-2 semantic runtime contract: PASS (configuration={ConfigurationName()}, diagnostics={diagnostics.Count})");
}
finally
{
    DartRuntimePrimitives.FutureDiagnosticSink = previousSink;
}

static void VerifyTypedTweenArithmetic()
{
    var offsetTween = new Tween<Offset>(new Offset(1.25, -2.5), new Offset(9.75, 5.5));
    Require(offsetTween.transform(0.0) == new Offset(1.25, -2.5), "Tween endpoint t=0");
    Require(offsetTween.transform(1.0) == new Offset(9.75, 5.5), "Tween endpoint t=1");
    Require(Near(offsetTween.transform(0.25), new Offset(3.375, -0.5)), "Offset generic typed lerp");

    var sizeTween = new SizeTween(new Size(10.0, 20.0), new Size(30.0, 60.0));
    Require(NearSize(sizeTween.transform(0.5), new Size(20.0, 40.0)), "Size lerp");

    var rectTween = new RectTween(new Rect(2.0, 4.0, 10.0, 20.0), new Rect(6.0, 12.0, 18.0, 28.0));
    Require(rectTween.transform(0.5) == new Rect(4.0, 8.0, 14.0, 24.0), "Rect lerp");

    var vectorTween = new Tween<Vector2>(new Vector2(1.25f, -2.5f), new Vector2(9.75f, 5.5f));
    var vector = vectorTween.transform(0.25);
    Require(Math.Abs(vector.X - 3.375f) < 0.00001f && Math.Abs(vector.Y + 0.5f) < 0.00001f, "Vector2 typed float boundary");

    var highPrecision = DartRuntimePrimitives.LerpTweenValue(0.12345678901234567, 9.876543210987654, 0.3333333333333333);
    var expected = 0.12345678901234567 + ((9.876543210987654 - 0.12345678901234567) * 0.3333333333333333);
    Require(highPrecision == expected, "double precision before host conversion");

    var matrix = Matrix4.translationValues(1.0 / 3.0, 2.0 / 7.0, 11.0 / 13.0);
    var hostX = (float)matrix.storage[12];
    Require(hostX == (float)(1.0 / 3.0), "Matrix host conversion is explicit float narrowing");
}

static void VerifyNullableAndAsyncSemantics(List<DartFutureDiagnostic> diagnostics)
{
    Func<Future>? nullableCallback = null;
    Require(DartRuntimePrimitives.AdaptAsyncCallback(nullableCallback) is null, "nullable callback remains null");

    var nullableResult = Future<int?>.value(null).asTask().GetAwaiter().GetResult();
    Require(nullableResult is null, "nullable Future result remains null");

    Future? nullAwareInvocationResult = null;
    DartRuntimePrimitives.Ignore(nullAwareInvocationResult);
    Require(diagnostics.Count == 0, "discarded null-aware Future invocation is a no-op");

    var completer = new Completer<int>();
    completer.complete(Future<int>.value(7));
    Require(completer.future.asTask().GetAwaiter().GetResult() == 7, "Completer observes Future completion");

    DartRuntimePrimitives.Observe(Future<int>.error(new InvalidOperationException("fixture failure")), "FCR-2 failing Future");
    Require(SpinWait.SpinUntil(() => diagnostics.Any(item => item.Operation == "FCR-2 failing Future"), TimeSpan.FromSeconds(5)), "Future error diagnostic");

    var canceledSource = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
    DartRuntimePrimitives.Observe(Future<int>.fromTask(canceledSource.Task), "FCR-2 canceled Future");
    canceledSource.TrySetCanceled();
    Thread.Sleep(50);
    Require(diagnostics.All(item => item.Operation != "FCR-2 canceled Future"), "Future cancellation is not reported as an error");
}

static void VerifyTimerDisposeRace()
{
    var canceledQueue = new DartMicrotaskQueue();
    var calls = 0;
    using (DartAsyncRuntime.enterMicrotaskScheduler(canceledQueue.enqueue))
    {
        using var timer = new Timer(Duration.Create(milliseconds: 1), () => calls++);
        Require(SpinWait.SpinUntil(() => canceledQueue.count > 0, TimeSpan.FromSeconds(5)), "timer callback enqueue");
        timer.cancel();
        canceledQueue.drain();
    }
    Require(calls == 0, "timer dispose suppresses queued callback");
}

static void VerifyCollectionAndPatternSemantics()
{
    var values = new List<int> { 1, 3 };
    foreach (var value in values.ToArray())
    {
        if (value == 1) values.Add(2);
    }
    Require(values.SequenceEqual([1, 3, 2]), "collection mutation uses a stable iteration snapshot");

    object candidate = 5;
    var result = candidate switch
    {
        int number when number > 0 => number + 1,
        _ => 0,
    };
    Require(result == 6, "pattern switch preserves typed value");
}

static void VerifyAssertConfiguration()
{
    var failed = false;
    try
    {
        DartRuntimePrimitives.Assert(() => false);
    }
    catch (AssertionError)
    {
        failed = true;
    }
#if DEBUG
    Require(failed, "Debug assert is active");
#else
    Require(!failed, "Release assert is elided");
#endif
}

static bool Near(Offset? actual, Offset expected) => actual is { } value && Math.Abs(value.dx - expected.dx) < 0.0000000001 && Math.Abs(value.dy - expected.dy) < 0.0000000001;
static bool NearSize(Size? actual, Size expected) => actual is not null && Math.Abs(actual.width - expected.width) < 0.0000000001 && Math.Abs(actual.height - expected.height) < 0.0000000001;
static string ConfigurationName() =>
#if DEBUG
    "Debug";
#else
    "Release";
#endif

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}
