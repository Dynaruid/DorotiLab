using System.Diagnostics;
using System.Runtime.InteropServices;
using Silk.NET.Vulkan;
using SkiaSharp;
using VkDevice = Silk.NET.Vulkan.Device;
using VkSemaphore = Silk.NET.Vulkan.Semaphore;

internal static unsafe partial class Program
{
    // A real timeline semaphore delays the queue without a shader spin, GPU reset,
    // fake loss result, or modified driver behavior. Only the host uses this feature.
    // The owner thread alone calls Graphite; a helper performs a legal host signal.
    private static void ProbeShutdown(Vk vk, VkDevice device, Queue queue, SKGraphiteContext context, WaitObserver observer)
    {
        Report["controlledDelay"] = "host-signaled timeline semaphore; not physical device loss";
        Report["shutdownDeadlineSeconds"] = 5;
        Report["physicalDeviceLoss"] = "notVerified";
        // Drain initialization before establishing the controlled wait fixture.
        using var recorder = context.CreateRecorder() ?? throw new InvalidOperationException("Recorder null.");
        using var surface = SKSurface.Create(recorder, new SKImageInfo(32, 32)) ?? throw new InvalidOperationException("Surface null.");
        surface.Canvas.Clear(SKColors.Red);
        using var recording = recorder.Snap() ?? throw new InvalidOperationException("Recording null.");
        var type = new SemaphoreTypeCreateInfo { SType = StructureType.SemaphoreTypeCreateInfo, SemaphoreType = SemaphoreType.Timeline };
        var sci = new SemaphoreCreateInfo { SType = StructureType.SemaphoreCreateInfo, PNext = &type };
        Check(vk.CreateSemaphore(device, &sci, null, out var semaphore), "timeline semaphore");
        var fci = new FenceCreateInfo { SType = StructureType.FenceCreateInfo };
        Check(vk.CreateFence(device, &fci, null, out var fence), "host completion fence");
        Thread? release = null;
        Exception? releaseError = null;
        Result signalResult = Result.NotReady;
        bool waitSubmitted = false;
        using var disposeEntered = new ManualResetEventSlim();
        try
        {
            ulong value = 1;
            var timeline = new TimelineSemaphoreSubmitInfo { SType = StructureType.TimelineSemaphoreSubmitInfo, WaitSemaphoreValueCount = 1, PWaitSemaphoreValues = &value };
            var stage = PipelineStageFlags.AllCommandsBit;
            var wait = new SubmitInfo { SType = StructureType.SubmitInfo, PNext = &timeline, WaitSemaphoreCount = 1,
                PWaitSemaphores = &semaphore, PWaitDstStageMask = &stage };
            Check(vk.QueueSubmit(queue, 1, &wait, default), "host controlled wait submit");
            waitSubmitted = true;
            var savedSemaphore = semaphore;
            // Release is armed immediately so even an unexpected Insert/Submit exception
            // cannot strand the device. The test never destroys pending Vulkan resources.
            release = new Thread(() =>
            {
                try
                {
                    // Defensive 10s wait if the owner fails before disposal; then always release.
                    disposeEntered.Wait(TimeSpan.FromSeconds(10));
                    Thread.Sleep(7000);
                    signalResult = Signal(vk, device, savedSemaphore);
                    Check(signalResult, "host signal timeline");
                }
                catch (Exception e) { releaseError = e; }
            });
            release.Start();
            if (context.InsertRecording(recording) != SKGraphiteInsertStatus.Success) throw new InvalidOperationException("Insert failed.");
            if (!context.Submit(new SKGraphiteSubmitInfo { Sync = false })) throw new InvalidOperationException("Submit failed.");
            var empty = new SubmitInfo { SType = StructureType.SubmitInfo };
            Check(vk.QueueSubmit(queue, 1, &empty, fence), "completion submit");
            Step("shutdown-close-admission-poll-fence");
            var shutdown = Stopwatch.StartNew();
            Result completion;
            do
            {
                completion = vk.GetFenceStatus(device, fence);
                if (completion != Result.NotReady) break;
                context.CheckAsyncWorkCompletion();
                Thread.Sleep(1);
            } while (shutdown.Elapsed.TotalSeconds < 5);
            Report["hostFenceAtDeadline"] = completion.ToString();
            Report["isDeviceLostAtDeadline"] = context.IsDeviceLost;
            if (completion != Result.NotReady || context.IsDeviceLost)
                throw new InvalidOperationException("Controlled delay did not reproduce an ordinary timeout.");
            Step("dispose-with-real-pending-work");
            observer.Armed = true;
            disposeEntered.Set();
            var dispose = Stopwatch.StartNew();
            context.Dispose();
            dispose.Stop();
            Report["disposeMilliseconds"] = dispose.Elapsed.TotalMilliseconds;
            Report["shutdownMilliseconds"] = shutdown.Elapsed.TotalMilliseconds;
            Report["shutdownWithinDeadline"] = shutdown.Elapsed.TotalSeconds <= 5;
            if (shutdown.Elapsed.TotalSeconds > 5)
                Report["blocker"] = "Ordinary incomplete GPU work exceeds the 5-second shutdown deadline; public Dispose waits indefinitely until real completion. No safe bounded abandonment path qualified.";
            Report["nativeWaitCalls"] = observer.Calls.ToArray();
            Report["nativeWaitUsedInfiniteTimeout"] = observer.Calls.Any(c => c.Timeout == ulong.MaxValue);
            Report["hostFenceAfterDispose"] = vk.GetFenceStatus(device, fence).ToString();
            release.Join();
            Report["releaseResult"] = signalResult.ToString();
            if (releaseError != null) throw new InvalidOperationException("Controlled release failed.", releaseError);
            // The terminal post-Graphite host fence is independent of the Skia destructor.
            // Its completion must be verified before destroying it or the semaphore.
            Check(vk.WaitForFences(device, 1, &fence, true, 5_000_000_000), "terminal fence");
        }
        finally
        {
            release?.Join();
            if (waitSubmitted)
            {
                if (release == null) Check(Signal(vk, device, semaphore), "emergency release");
                // One diagnostic teardown drain only, never a presentation-loop substitute.
                Check(vk.DeviceWaitIdle(device), "controlled test drain");
            }
            vk.DestroyFence(device, fence, null);
            vk.DestroySemaphore(device, semaphore, null);
        }
    }

    private static Result Signal(Vk vk, VkDevice device, VkSemaphore semaphore)
    {
        var signal = (delegate* unmanaged<nint, SemaphoreSignalInfo*, Result>)(nint)vk.GetDeviceProcAddr(device, "vkSignalSemaphoreKHR");
        if (signal == null) throw new NotSupportedException("vkSignalSemaphoreKHR unavailable.");
        var info = new SemaphoreSignalInfo { SType = StructureType.SemaphoreSignalInfo, Semaphore = semaphore, Value = 1 };
        return signal(device.Handle, &info);
    }

    private sealed class WaitObserver : IDisposable
    {
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate Result Wait(nint device, uint count, Fence* fences, uint all, ulong timeout);
        private readonly Wait Forward;
        private readonly Wait Callback;
        public nint Address { get; }
        public bool Armed { get; set; }
        public List<WaitCall> Calls { get; } = [];
        public WaitObserver(Vk vk, VkDevice device)
        {
            Forward = Marshal.GetDelegateForFunctionPointer<Wait>(vk.GetDeviceProcAddr(device, "vkWaitForFences"));
            Callback = (d, count, fences, all, timeout) =>
            {
                var start = Stopwatch.GetTimestamp();
                // Exact arguments and real result. Observation cannot shorten or fake this wait.
                var result = Forward(d, count, fences, all, timeout);
                try { if (Armed) Calls.Add(new(timeout, result.ToString(), Stopwatch.GetElapsedTime(start).TotalMilliseconds)); }
                catch { CallbackFault = true; }
                return result;
            };
            Address = Marshal.GetFunctionPointerForDelegate(Callback);
        }
        public void Dispose() { GC.KeepAlive(Callback); GC.KeepAlive(Forward); }
    }
    private sealed record WaitCall(ulong Timeout, string Result, double Milliseconds);
}
