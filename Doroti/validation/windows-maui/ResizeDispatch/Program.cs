using System.Collections.Concurrent;
using Doroti.Host.Maui;

var dispatched = new ConcurrentQueue<Action>();
using var queue = new WindowsResizeDispatchQueue(dispatched.Enqueue);
var uiThread = Environment.CurrentManagedThreadId;
var calls = 0;
void Commit()
{
    Check(Environment.CurrentManagedThreadId == uiThread, "Commit escaped the UI thread");
    calls++;
}
void Drain()
{
    while (dispatched.TryDequeue(out var action))
    {
        action();
    }
}
void Check(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}

// A blocked UI thread must service only the owned queue, including resource
// attachment needed before the GPU frame can be prepared.
queue.BeginResize(1, false);
var fixedOrigin = Task.Run(() =>
{
    queue.Invoke(Commit);
    queue.Invoke(Commit, 1);
});
Check(queue.WaitForFrame(1, false, TimeSpan.FromSeconds(1)), "Fixed-origin deadlock");
fixedOrigin.GetAwaiter().GetResult();
Drain();
Check(calls == 2, "Queued and inline pumps executed a callback twice");

queue.BeginResize(2, true);
var moving = Task.Run(() => queue.Invoke(Commit, 2));
Check(queue.WaitForFrame(2, true, TimeSpan.FromSeconds(1)), "Moving frame was not prepared");
Drain();
Check(calls == 2 && !moving.IsCompleted, "Moving frame was visible before geometry");
queue.EndResize();
moving.GetAwaiter().GetResult();
Check(calls == 3, "Moving frame was not committed after geometry");

// Cancel/mismatch changes the same latest-generation gate used by the product
// presenter before unblocking a prepared front.
long latestGeneration = 3;
queue.BeginResize(3, true);
var cancelled = Task.Run(() =>
    queue.Invoke(
        () =>
        {
            if (latestGeneration == 3)
            {
                Commit();
            }
        },
        3
    )
);
Check(queue.WaitForFrame(3, true, TimeSpan.FromSeconds(1)), "Cancel setup failed");
latestGeneration = 4;
queue.EndResize();
cancelled.GetAwaiter().GetResult();
Check(calls == 3, "Cancelled proposal mutated the visible front");

queue.BeginResize(4, true);
Check(!queue.WaitForFrame(4, true, TimeSpan.FromMilliseconds(10)), "Missing frame never timed out");
var late = Task.Run(() => queue.Invoke(Commit, 4));
Check(queue.WaitForFrame(4, true, TimeSpan.FromSeconds(1)), "Late frame lost its wake");
queue.EndResize();
late.GetAwaiter().GetResult();

queue.BeginResize(5, false);
var failing = Task.Run(() =>
    queue.Invoke(() => throw new InvalidOperationException("injected"), 5)
);
Check(!queue.WaitForFrame(5, false, TimeSpan.FromSeconds(1)), "Failed commit accepted");
try
{
    failing.GetAwaiter().GetResult();
    throw new InvalidOperationException("Failure not propagated to raster owner");
}
catch (Exception exception)
    when (exception.ToString().Contains("injected", StringComparison.Ordinal)) { }
queue.EndResize();
Drain();
Check(calls == 4, "Unexpected UI mutations");
Console.WriteLine(
    "PASS: fixed/moving resize, UI affinity, exactly-once dispatch, cancellation, timeout, failure"
);
