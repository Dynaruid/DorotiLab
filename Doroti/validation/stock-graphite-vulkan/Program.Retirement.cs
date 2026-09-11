using System.Diagnostics;
using Silk.NET.Vulkan;
using SkiaSharp;
using VkDevice = Silk.NET.Vulkan.Device;
using VkSemaphore = Silk.NET.Vulkan.Semaphore;

internal static unsafe partial class Program
{
    // One device/context generation per admission slot. A close request stops new
    // frames immediately, but releases the slot only after native teardown.
    // A permanently stalled generation keeps the slot; this is not fault recovery.
    private sealed class RetirementAdmission
    {
        private readonly object Gate = new();
        private int _live, _peak;
        private string _state = "Retired";
        public int Live { get { lock (Gate) return _live; } }
        public int Peak { get { lock (Gate) return _peak; } }
        public string State { get { lock (Gate) return _state; } }
        public bool TryStart()
        {
            lock (Gate)
            {
                if (_live != 0) return false;
                _live = 1; _peak = Math.Max(_peak, _live); _state = "Active";
                return true;
            }
        }
        public bool AdmitFrame() { lock (Gate) return _state == "Active"; }
        public void Close() { lock (Gate) if (_state == "Active") _state = "Draining"; }
        public void Timeout() { lock (Gate) if (_state == "Draining") _state = "FaultedHold"; }
        public void GpuCompleted() { lock (Gate) _state = "GpuComplete"; }
        public void Complete()
        {
            lock (Gate)
            {
                if (_state != "GpuComplete") throw new InvalidOperationException("Releasing an uncompleted generation.");
                _state = "Retired"; _live = 0;
            }
        }
    }

    private static void ProbeRetirement(Vk vk, VkDevice device, Queue queue, SKGraphiteContext context, WaitObserver observer)
    {
        bool delayed = Mode == "retire-delayed", cancel = Mode == "retire-cancel";
        var ownerThread = Environment.CurrentManagedThreadId;
        var info = new SKImageInfo(32, 32, SKColorType.Rgba8888, SKAlphaType.Premul);
        using var recorder = context.CreateRecorder() ?? throw new InvalidOperationException("Recorder null.");
        using var surface = SKSurface.Create(recorder, info) ?? throw new InvalidOperationException("Surface null.");
        surface.Canvas.Clear(SKColors.Red);
        using var recording = recorder.Snap() ?? throw new InvalidOperationException("Snap null.");
        VkSemaphore semaphore = default;
        Fence fence = default;
        bool hostWaitSubmitted = false, readbackComplete = false, gpuComplete = false;
        byte[]? pixels = null;
        Exception? callbackError = null, coordinatorError = null, releaseError = null;
        var events = new List<string> { "Active" };
        using var submitted = new ManualResetEventSlim();
        using var closeRequested = new ManualResetEventSlim();
        using var acknowledged = new ManualResetEventSlim();
        using var releaseRequested = new ManualResetEventSlim();
        Thread? coordinator = null, release = null;
        double closeMilliseconds = -1;
        int rejectedGenerations = 0, rejectedFrames = 0, coordinatorThread = 0;
        Result releaseResult = Result.NotReady;
        long closeAt = 0;
        try
        {
            if (delayed)
            {
                var type = new SemaphoreTypeCreateInfo { SType = StructureType.SemaphoreTypeCreateInfo, SemaphoreType = SemaphoreType.Timeline };
                var sci = new SemaphoreCreateInfo { SType = StructureType.SemaphoreCreateInfo, PNext = &type };
                Check(vk.CreateSemaphore(device, &sci, null, out semaphore), "retirement timeline");
                var savedSemaphore = semaphore;
                release = new Thread(() =>
                {
                    try
                    {
                        // Test producer is independent of Dispose. finally requests release on
                        // any setup failure; successful runs hold for seven seconds after close.
                        releaseRequested.Wait();
                        Thread.Sleep(7000);
                        releaseResult = Signal(vk, device, savedSemaphore);
                        Check(releaseResult, "real delayed producer signal");
                    }
                    catch (Exception e) { releaseError = e; }
                });
                release.Start();
                ulong value = 1;
                var timeline = new TimelineSemaphoreSubmitInfo { SType = StructureType.TimelineSemaphoreSubmitInfo,
                    WaitSemaphoreValueCount = 1, PWaitSemaphoreValues = &value };
                var stage = PipelineStageFlags.AllCommandsBit;
                var wait = new SubmitInfo { SType = StructureType.SubmitInfo, PNext = &timeline, WaitSemaphoreCount = 1,
                    PWaitSemaphores = &semaphore, PWaitDstStageMask = &stage };
                Check(vk.QueueSubmit(queue, 1, &wait, default), "delayed producer wait");
                hostWaitSubmitted = true;
            }
            if (!cancel)
            {
                if (!Retirement.AdmitFrame()) throw new InvalidOperationException("Frame admission closed early.");
                if (context.InsertRecording(recording) != SKGraphiteInsertStatus.Success) throw new InvalidOperationException("Insert failed.");
                context.RequestReadPixels(surface, info, new(0, 0, 32, 32), SKImageRescaleGamma.Src, SKImageRescaleMode.Nearest, result =>
                {
                    try { pixels = result?.ToArray(0); }
                    catch (Exception e) { callbackError = e; }
                    finally { readbackComplete = true; }
                });
                if (!context.Submit(new SKGraphiteSubmitInfo { Sync = false })) throw new InvalidOperationException("Submit failed.");
            }
            // retire-cancel represents an unavailable HOST input: neither InsertRecording
            // nor a Vulkan wait for that input is submitted. This is not WSI acquire cancel.
            var fci = new FenceCreateInfo { SType = StructureType.FenceCreateInfo };
            Check(vk.CreateFence(device, &fci, null, out fence), "retirement terminal fence");
            var empty = new SubmitInfo { SType = StructureType.SubmitInfo };
            Check(vk.QueueSubmit(queue, 1, &empty, fence), "retirement terminal submit");
            submitted.Set();
            coordinator = new Thread(() =>
            {
                try
                {
                    coordinatorThread = Environment.CurrentManagedThreadId;
                    submitted.Wait();
                    closeAt = Stopwatch.GetTimestamp();
                    Retirement.Close();
                    closeRequested.Set();
                    if (!acknowledged.Wait(TimeSpan.FromSeconds(5))) throw new TimeoutException("Close acknowledgement exceeded five seconds.");
                    closeMilliseconds = Stopwatch.GetElapsedTime(closeAt).TotalMilliseconds;
                    for (int attempt = 0; attempt < 1000; attempt++)
                    {
                        if (Retirement.TryStart()) throw new InvalidOperationException("Unretired context replaced.");
                        rejectedGenerations++;
                        if (Retirement.AdmitFrame()) throw new InvalidOperationException("Frame accepted after close.");
                        rejectedFrames++;
                    }
                }
                catch (Exception e) { coordinatorError = e; }
            });
            coordinator.Start();
            if (!closeRequested.Wait(TimeSpan.FromSeconds(5))) throw new TimeoutException("Coordinator did not request close.");
            events.Add("Draining");
            var pendingAtClose = vk.GetFenceStatus(device, fence);
            var callbackPendingAtClose = !cancel && !readbackComplete;
            acknowledged.Set(); // No native destruction or GPU wait on this path.
            coordinator.Join(); // Managed admission checks only, never waits for GPU.
            if (coordinatorError != null) throw new InvalidOperationException("Coordinator failed.", coordinatorError);
            RetirementCloseAt = closeAt;
            releaseRequested.Set();
            var polling = Stopwatch.StartNew();
            bool timedOut = false;
            Result completion;
            Step("retirement-poll-only");
            do
            {
                completion = vk.GetFenceStatus(device, fence);
                if (completion == Result.Success) break;
                if (completion != Result.NotReady) throw new InvalidOperationException("Real fence failure: " + completion);
                context.CheckAsyncWorkCompletion();
                if (!timedOut && polling.Elapsed.TotalSeconds >= 5)
                {
                    timedOut = true; Retirement.Timeout(); events.Add("FaultedHold");
                    if (context.IsDeviceLost) throw new InvalidOperationException("Ordinary delayed producer unexpectedly became device loss.");
                    // Recheck the cap after the deadline, not just during the close callback.
                    if (Retirement.TryStart() || Retirement.AdmitFrame()) throw new InvalidOperationException("Faulted generation admitted replacement work.");
                }
                if (polling.Elapsed.TotalSeconds >= 30) throw new TimeoutException("Controlled producer did not complete within diagnostic deadline.");
                Thread.Sleep(1);
            } while (true);
            gpuComplete = true;
            context.CheckAsyncWorkCompletion();
            if (!cancel && (!readbackComplete || callbackError != null || pixels is null || pixels.Length < 4096 ||
                pixels[0] != 255 || pixels[1] != 0 || pixels[2] != 0 || pixels[3] != 255))
                throw new InvalidOperationException("Pending readback was lost or pixels incorrect.", callbackError);
            if (delayed && (pendingAtClose != Result.NotReady || !callbackPendingAtClose || !timedOut))
                throw new InvalidOperationException("Delayed scenario did not exercise retained GPU/callback ownership.");
            Retirement.GpuCompleted(); events.Add("GpuComplete");
            observer.Armed = true;
            // Dispose Skia children first; the controller slot still retains this VkDevice.
            recording.Dispose(); surface.Dispose(); recorder.Dispose();
            var dispose = Stopwatch.StartNew();
            context.Dispose();
            dispose.Stop();
            events.Add("ContextDisposed");
            release?.Join();
            if (releaseError != null) throw new InvalidOperationException("Producer failed.", releaseError);
            if (observer.Calls.Count != 0) throw new InvalidOperationException("Drained context still issued a blocking fence wait.");
            if (closeMilliseconds >= 5000 || dispose.Elapsed.TotalSeconds >= 5)
                throw new InvalidOperationException("Close or drained disposal exceeded five seconds.");
            Report["retirementResult"] = new
            {
                generation = Report["generation"], mode = Mode, events, ownerThread, coordinatorThread,
                logicalCloseMilliseconds = closeMilliseconds,
                gpuRetirementMilliseconds = Stopwatch.GetElapsedTime(closeAt).TotalMilliseconds,
                disposeMilliseconds = dispose.Elapsed.TotalMilliseconds,
                pendingFenceAtClose = pendingAtClose.ToString(), callbackPendingAtClose,
                callbackCompleted = !cancel && readbackComplete,
                cancelledBeforeInsert = cancel, graphFramesSubmitted = cancel ? 0 : 1,
                hostInputWaitSubmitted = hostWaitSubmitted, timeoutWasNotDeviceLoss = timedOut && !CallbackFault,
                rejectedGenerations, rejectedFrames, liveGenerationCap = 1,
                disposeNativeWaitCalls = observer.Calls.ToArray(),
                producerSignalResult = delayed ? releaseResult.ToString() : "notApplicable",
                externalProcessTermination = false, permanentStallRecovery = "notVerified",
                platformRetirement = "notVerified", outcome = "PASS-scoped-retirement-protocol"
            };
        }
        finally
        {
            closeRequested.Set(); acknowledged.Set(); releaseRequested.Set();
            coordinator?.Join(); release?.Join();
            if (!gpuComplete)
            {
                // Diagnostic exceptional cleanup only, after releasing this test's producer.
                // Never use it in the logical close path or to claim bounded failure recovery.
                Check(vk.DeviceWaitIdle(device), "retirement diagnostic failure drain");
                context.CheckAsyncWorkCompletion();
            }
            if (fence.Handle != 0) vk.DestroyFence(device, fence, null);
            if (semaphore.Handle != 0) vk.DestroySemaphore(device, semaphore, null);
        }
    }
}
