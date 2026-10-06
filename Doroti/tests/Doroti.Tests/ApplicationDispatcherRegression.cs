using Doroti.Ui;

internal static class ApplicationDispatcherRegression
{
    // Isolated command: the ThreadPool limits are process-wide and restored
    // before returning. The application owner and native caller are real threads.
    public static void Run()
    {
        ThreadPool.GetMinThreads(out var oldMin, out var oldMinIo);
        ThreadPool.GetMaxThreads(out var oldMax, out var oldMaxIo);
        if (!ThreadPool.SetMinThreads(1, oldMinIo) || !ThreadPool.SetMaxThreads(1, oldMaxIo))
            throw new InvalidOperationException("Cannot isolate the ThreadPool starvation regression.");
        try
        {
            CheckNativeCompletion(fullQueue: false);
            CheckNativeCompletion(fullQueue: true);
        }
        finally
        {
            ThreadPool.SetMaxThreads(oldMax, oldMaxIo);
            ThreadPool.SetMinThreads(oldMin, oldMinIo);
        }
        CheckLifetime();
    }

    private static void CheckNativeCompletion(bool fullQueue)
    {
        var owner = new DorotiApplicationDispatcher(capacity: 1);
        using var poolEntered = new ManualResetEventSlim();
        using var releasePool = new ManualResetEventSlim();
        using var uiEntered = new ManualResetEventSlim();
        using var releaseUi = new ManualResetEventSlim();
        using var uiFinished = new ManualResetEventSlim();
        using var nativeFinished = new ManualResetEventSlim();
        Exception? error = null;
        var result = 0;
        Task? held = null, queued = null;
        var native = new Thread(() =>
        {
            try
            {
                result = owner.InvokeAsync(() =>
                {
                    if (!owner.HasThreadAccess) throw new Exception("UI callback escaped its owner.");
                    uiEntered.Set();
                    releaseUi.Wait();
                    uiFinished.Set();
                    return 42;
                }).AsTask().GetAwaiter().GetResult();
            }
            catch (Exception failure) { error = failure; }
            finally { nativeFinished.Set(); }
        }) { IsBackground = true, Name = "native completion regression" };
        bool completedWithoutPool = false;
        try
        {
            ThreadPool.QueueUserWorkItem(_ => { poolEntered.Set(); releasePool.Wait(); });
            Require(poolEntered.Wait(TimeSpan.FromSeconds(3)), "ThreadPool worker did not enter.");
            if (fullQueue)
            {
                held = owner.InvokeAsync(() => { uiEntered.Set(); releaseUi.Wait(); }).AsTask();
                Require(uiEntered.Wait(TimeSpan.FromSeconds(3)), "Owner did not enter.");
                queued = owner.InvokeAsync(() => { }).AsTask();
                uiEntered.Reset();
            }
            native.Start();
            if (!fullQueue) Require(uiEntered.Wait(TimeSpan.FromSeconds(3)), "UI callback did not enter.");
            var parked = false;
            for (var attempt = 0; attempt < 30 && !parked; attempt++)
            {
                parked = (native.ThreadState & System.Threading.ThreadState.WaitSleepJoin) != 0;
                if (!parked) Thread.Sleep(5);
            }
            Require(parked, "Native caller did not await its queued operation.");
            releaseUi.Set();
            Require(uiFinished.Wait(TimeSpan.FromSeconds(3)), "UI operation did not complete.");
            completedWithoutPool = nativeFinished.Wait(TimeSpan.FromMilliseconds(500));
        }
        finally
        {
            releaseUi.Set();
            releasePool.Set();
            if (native.IsAlive && !native.Join(TimeSpan.FromSeconds(3)))
                throw new Exception("Native completion did not drain.");
            held?.GetAwaiter().GetResult();
            queued?.GetAwaiter().GetResult();
            owner.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
        if (error is not null) throw error;
        Require(completedWithoutPool && result == 42,
            $"UI work completed, but native completion depended on a busy ThreadPool (fullQueue={fullQueue}).");
        Console.WriteLine($"PASS: UI-to-native completion with a saturated ThreadPool (fullQueue={fullQueue}).");
    }

    private static void CheckLifetime()
    {
        var owner = new DorotiApplicationDispatcher(capacity: 1);
        using var entered = new ManualResetEventSlim();
        using var release = new ManualResetEventSlim();
        using var cancellation = new CancellationTokenSource();
        var invoked = 0;
        var running = owner.InvokeAsync(() =>
        {
            entered.Set();
            release.Wait();
            return 7;
        }, cancellation.Token).AsTask();
        Task? queued = null, rejected = null;
        try
        {
            Require(entered.Wait(TimeSpan.FromSeconds(3)), "Running operation did not start.");
            queued = owner.InvokeAsync(() => invoked++).AsTask();
            using var pendingCancellation = new CancellationTokenSource();
            rejected = owner.InvokeAsync(() => invoked++, pendingCancellation.Token).AsTask();
            pendingCancellation.Cancel();
            try { rejected.GetAwaiter().GetResult(); throw new Exception("Pending admission was not canceled."); }
            catch (OperationCanceledException) { }
            Require(rejected.IsCanceled, "Canceled admission returned a faulted task.");
            cancellation.Cancel();
            Require(!running.IsCompleted, "Cancellation reported an executing callback as drained.");
            release.Set();
            Require(running.GetAwaiter().GetResult() == 7, "Executing callback lost its result after cancellation.");
            queued.GetAwaiter().GetResult();
            Require(invoked == 1, "Canceled admission ran or accepted work was lost.");
            Require(owner.InvokeAsync(() => owner.InvokeAsync(() => 42).GetAwaiter().GetResult())
                .AsTask().GetAwaiter().GetResult() == 42, "Reentrant owner invocation changed.");
            using var alreadyCanceled = new CancellationTokenSource();
            alreadyCanceled.Cancel();
            Require(owner.InvokeAsync(() => 0, alreadyCanceled.Token).AsTask().IsCanceled,
                "Pre-canceled invocation lost its canceled task state.");
            var inlineCanceled = owner.InvokeAsync(() => owner.InvokeAsync<int>(() => throw new OperationCanceledException())
                .AsTask().IsCanceled).AsTask().GetAwaiter().GetResult();
            Require(inlineCanceled, "Inline owner cancellation became a fault.");
        }
        finally
        {
            release.Set();
            owner.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
        Require(owner.InvokeAsync(() => 0).AsTask().IsFaulted, "Closed owner accepted new work.");
        Console.WriteLine("PASS: bounded admission cancellation, execution drain, reentrant ownership and close.");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
