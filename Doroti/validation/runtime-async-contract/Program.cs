using Doroti.Runtime;
using Timer = Doroti.Runtime.Timer;

var canceledQueue = new DartMicrotaskQueue();
var canceledCalls = 0;
using (DartAsyncRuntime.enterMicrotaskScheduler(canceledQueue.enqueue))
{
    using var timer = new Timer(Duration.Create(milliseconds: 1), () => canceledCalls++);
    WaitForQueuedCallback(canceledQueue, "canceled timer callback");
    timer.cancel();
    canceledQueue.drain();
}

Require(canceledCalls == 0, "Cancel must suppress a callback already queued for the host event loop.");

var completedQueue = new DartMicrotaskQueue();
var completedCalls = 0;
using (DartAsyncRuntime.enterMicrotaskScheduler(completedQueue.enqueue))
{
    using var timer = new Timer(Duration.Create(milliseconds: 1), () => completedCalls++);
    WaitForQueuedCallback(completedQueue, "active timer callback");
    Require(timer.isActive, "A one-shot timer must remain active until its queued callback begins.");
    completedQueue.drain();
    Require(!timer.isActive, "A one-shot timer must become inactive when its callback begins.");
}

Require(completedCalls == 1, "An active one-shot timer callback must run exactly once.");
Console.WriteLine("Runtime async contract: PASS");

static void WaitForQueuedCallback(DartMicrotaskQueue queue, string name)
{
    Require(SpinWait.SpinUntil(() => queue.count > 0, TimeSpan.FromSeconds(5)), $"Timed out waiting for {name}.");
}

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}
