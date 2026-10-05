using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using Doroti.Skia.Rendering;
using Doroti.Skia.RuntimeEffects;
using Doroti.Skia.Vulkan;
using Doroti.Testing;
using Doroti.Ui;
using Silk.NET.Vulkan;
using SkiaSharp;
using VkSemaphore = Silk.NET.Vulkan.Semaphore;
using Path = System.IO.Path;

// Actual Vulkan pixels and two deliberately unfinished recordings. This tests
// producer/copy and consumer-bank ownership, not Qt/DWM scanout or physical input.
internal static class NativeFrameGpuRegression
{
    public static void Run(string output)
    {
        GraphiteNativeLibrary.Configure();
        using var gpu = new VulkanDeviceOwner();
        using var quick = new GraphiteVulkanQuick(gpu.Instance.Handle, gpu.Physical.Handle,
            gpu.Device.Handle, gpu.Queue.Handle, gpu.Family, (1u << 22) | (2u << 12));
        using var tester = new WidgetTester(new Size(96, 80));
        var host = (ISkiaSceneRendererHost)typeof(WidgetTester)
            .GetField("_host", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(tester)!;
        using var renderer = new SkiaSceneRenderer(1, host, null, null, "native-frame-gpu",
            DorotiSkiaRuntimeEffects.NativeGraphiteVulkanBackend, "Vulkan", false);
        long sequence = 0;
        (Task<SkiaGraphiteReadback> Readback, SkiaPaintCompletion Completion) Record(string mode, double sigma, bool green)
        {
            var paths = new List<PathCommand>();
            var canvas = new Doroti.Ui.Canvas(paths);
            canvas.drawRect(new Rect(0, 0, 96, 80), new Doroti.Ui.Paint { color = new Color(green ? 0xff00ff00 : 0xffff0000) });
            for (var x = 0; x < 96; x += 8)
                canvas.drawRect(new Rect(x, 0, x + 2, 80), new Doroti.Ui.Paint { color = new Color(0xff000000) });
            var commands = new List<SceneCommand>
            {
                new("picture", null) { HostPayload = new ScenePicturePayload(++sequence, new(0, 0), paths, null, false, true) },
            };
            if (mode != "off" && sigma > 0)
            {
                var filter = ImageFilter.variableBlur(new(0, 0), new(0, 80), endSigma: sigma,
                    resolutionScale: mode == "full" ? 1 : .25,
                    adaptiveResolution: mode is "adaptive" or "fast",
                    kernel: mode == "fast" ? VariableBlurKernel.fastGaussian : mode == "kawase"
                        ? VariableBlurKernel.dualKawase : VariableBlurKernel.gaussian);
                commands.Add(new("backdropFilter", null) { HostPayload = new SceneBackdropFilterPayload(
                    ImageFilterSnapshot.Capture(filter), BlendMode.srcOver, null) });
                commands.Add(new("pop", null));
            }
            using var scene = new Scene(1, commands);
            using var submission = new DorotiSceneSubmission(scene, new(host.ViewEpoch, sequence, 96, 80));
            renderer.Submit(1, submission,
                DorotiUiInvocation.Managed("native-frame-gpu"));
            var surface = quick.Begin(96, 80);
            var paint = renderer.PaintNewShaderScene(surface, 96, 80, host.ResizeTarget);
            var frame = (SkiaGraphiteSession.Frame)typeof(GraphiteVulkanQuick)
                .GetField("_frame", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(quick)!;
            var read = frame.RequestReadback(surface, new SKImageInfo(96, 80, SKColorType.Rgba8888));
            quick.Complete(asynchronous: true);
            quick.MarkPublished();
            quick.QtConsumerSubmitted();
            return (read, paint.Completion ?? throw new Exception("Missing fresh GPU scene."));
        }
        void Drain()
        {
            var deadline = Stopwatch.GetTimestamp();
            while (!quick.PollGpuWork())
            {
                if (Stopwatch.GetElapsedTime(deadline) > TimeSpan.FromSeconds(10)) throw new TimeoutException("GPU regression drain.");
                Thread.Sleep(1);
            }
        }
        var checks = new List<object>();
        foreach (var mode in new[] { "off", "full", "adaptive", "fast", "fixed", "kawase" })
        foreach (var sigma in mode == "off" ? new[] { 0d } : mode == "kawase" ? new[] { 20d, 32d }
            : new[] { 0d, 1d, 2d, 4d, 8d, 20d, 32d })
        {
            var referenceRed = Record(mode, sigma, false); Drain();
            renderer.CompletePaint(referenceRed.Completion);
            var referenceGreen = Record(mode, sigma, true); Drain();
            renderer.CompletePaint(referenceGreen.Completion);
            gpu.Hold();
            var released = false;
            try
            {
                var cRed = Record(mode, sigma, false);
                var cGreen = Record(mode, sigma, true);
                if (gpu.WatchdogFired) throw new Exception("Recording blocked on the previous GPU workload.");
                if (quick.FramesInFlight != 2) throw new Exception("The GPU blocker did not retain both recordings.");
                var denied = NativeFrameAdmissionPolicy.PrepareAndDecide(() => { },
                    () => SkiaShaderSceneAdmission.eligible, () => quick.FramesInFlight, false, false, false);
                if (denied.Admitted) throw new Exception("A third GPU frame was admitted.");
                gpu.Release(); released = true; Drain();
                var red = cRed.Readback.GetAwaiter().GetResult().Pixels;
                var green = cGreen.Readback.GetAwaiter().GetResult().Pixels;
                if (!red.SequenceEqual(referenceRed.Readback.GetAwaiter().GetResult().Pixels)
                    || !green.SequenceEqual(referenceGreen.Readback.GetAwaiter().GetResult().Pixels))
                    throw new Exception($"C sequential/overlapped pixel mismatch or snapshot contamination: {mode} sigma={sigma}.");
                if (red.SequenceEqual(green)) throw new Exception("Different scenes produced identical pixels.");
                renderer.CompletePaint(cGreen.Completion);
                renderer.CompletePaint(cRed.Completion);
                checks.Add(new { mode, sigma, redSha256 = Convert.ToHexString(SHA256.HashData(red)),
                    greenSha256 = Convert.ToHexString(SHA256.HashData(green)), maximumPending = quick.MaximumFramesInFlight });
                Console.WriteLine($"PASS Vulkan C sequential/overlapped pixels and isolated recordings: {mode} sigma={sigma}");
            }
            finally { if (!released) gpu.Release(); }
        }
        Drain();
        object dynamicBudget;
        using (var large = new GraphiteVulkanQuick(gpu.Instance.Handle, gpu.Physical.Handle,
            gpu.Device.Handle, gpu.Queue.Handle, gpu.Family, (1u << 22) | (2u << 12)))
        {
            void RecordLayers(int width, int height, int count)
            {
                large.Begin(width, height);
                for (var index = 0; index < count; index++) large.Canvas(index).Clear(SKColors.Red);
                large.Complete(asynchronous: true);
                large.MarkPublished();
                large.QtConsumerSubmitted();
            }
            void DrainLarge()
            {
                var started = Stopwatch.GetTimestamp();
                while (!large.PollGpuWork())
                {
                    if (Stopwatch.GetElapsedTime(started) > TimeSpan.FromSeconds(10))
                        throw new TimeoutException("Large texture consumer drain.");
                    Thread.Sleep(1);
                }
            }
            gpu.Hold();
            try
            {
                RecordLayers(1600, 1200, 5);
                RecordLayers(1600, 1200, 5);
                if (gpu.WatchdogFired || large.FramesInFlight != 2 || large.ReservedBytes <= 128UL * 1024 * 1024
                    || large.TextureBudgetBytes < large.ReservedBytes)
                    throw new Exception("Dynamic texture allowance failed two GPU-owned banks above 128 MiB.");
            }
            finally { gpu.Release(); }
            DrainLarge();
            var peak = large.PeakReservedBytes;
            RecordLayers(96, 80, 1); DrainLarge();
            RecordLayers(96, 80, 1); DrainLarge();
            if (large.ReservedBytes >= peak || large.TextureBudgetBytes >= peak)
                throw new Exception("Texture allocations/allowance did not shrink after safe resize retirement.");
            var heaps = large.TextureBudgets;
            var peakBudget = large.PeakTextureBudgetBytes;
            large.Dispose();
            if (large.ReservedBytes != 0 || large.TextureBudgetBytes != 0
                || large.ConsumerSubmissions != large.CompletedConsumers)
                throw new Exception("Texture budget shutdown retained unretired allocations.");
            dynamicBudget = new { peakReservedBytes = peak, peakBudgetBytes = peakBudget, heaps,
                finalReservedBytes = large.ReservedBytes, finalBudgetBytes = large.TextureBudgetBytes,
                consumerSubmissions = large.ConsumerSubmissions, completedConsumers = large.CompletedConsumers };
            Console.WriteLine("PASS Vulkan dynamic texture allowance above 128 MiB, two held GPU banks, resize shrink and final consumer drain.");
        }
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output))!);
        File.WriteAllText(output, JsonSerializer.Serialize(new { gpu = gpu.Name, software = gpu.Software,
            policy = NativeFrameConfiguration.Mode, reference = "C submission with drain between scenes",
            checks, dynamicBudget, quick.MaximumFramesInFlight, quick.ConsumerSubmissions, quick.CompletedConsumers,
            scanout = "notMeasured", hardwareOverlap = "notMeasured", physicalInput = "notVerified" },
            new JsonSerializerOptions { WriteIndented = true }));
    }


}
