using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using Doroti.Skia.Rendering;
using Doroti.Skia.RuntimeEffects;
using Doroti.Testing;
using Doroti.Ui;
using SkiaSharp;
using Path = System.IO.Path;

// Exercises the production Metal session with two submissions retained until
// the same queue's terminal marker completes. It does not measure GPU overlap.
internal static class NativeFrameMetalGpuRegression
{
    public static void Run(string output)
    {
        if (!OperatingSystem.IsMacOS())
            throw new PlatformNotSupportedException("Native frame Metal validation requires macOS.");
        using var gpu = new MetalOwner();
        using var session = SkiaGraphiteSession.CreateMetal(gpu.Device, gpu.Queue, 1, maxFrames: 2);
        using var tester = new WidgetTester(new Size(96, 80));
        var host = (ISkiaSceneRendererHost)typeof(WidgetTester)
            .GetField("_host", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(tester)!;
        using var renderer = new SkiaSceneRenderer(1, host, null, null, "native-frame-metal-gpu",
            DorotiSkiaRuntimeEffects.NativeGraphiteMetalBackend, "Metal", false);
        var pending = new List<(SkiaGraphiteSession.Frame Frame, nint Texture, Task<SkiaGraphiteReadback> Readback)>();
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
                DorotiUiInvocation.Managed("native-frame-metal-gpu"));
            var texture = gpu.CreateTexture();
            var frame = session.BeginMetalFrame(96, 80, texture);
            var surface = frame.Surface;
            var paint = renderer.PaintNewShaderScene(surface, 96, 80, host.ResizeTarget);
            var readback = frame.RequestReadback(new SKImageInfo(96, 80, SKColorType.Rgba8888));
            frame.Submit(); // Same asynchronous submission as the Apple hosts.
            pending.Add((frame, texture, readback));
            return (readback, paint.Completion ?? throw new Exception("Missing fresh Metal scene."));
        }
        void Drain()
        {
            gpu.WaitForTerminal(); // Fixture-only wait, after all requested submissions.
            foreach (var entry in pending.AsEnumerable().Reverse())
            {
                entry.Frame.CompleteGpuWork();
                if (!entry.Readback.IsCompletedSuccessfully) throw new Exception("Metal readback did not retire.");
                Native.Send(entry.Texture, Native.Selector("release"));
            }
            pending.Clear();
            if (session.OutstandingFrames != 0) throw new Exception("Metal session retained completed frames.");
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
            var redFrame = Record(mode, sigma, false);
            var greenFrame = Record(mode, sigma, true);
            if (session.OutstandingFrames != 2 || session.CanBeginFrame)
                throw new Exception("The Metal fixture did not retain two unfinished submissions.");
            var callbacks = 0;
            var denied = NativeFrameAdmissionPolicy.PrepareAndDecide(() => callbacks++,
                () => SkiaShaderSceneAdmission.eligible, () => session.OutstandingFrames, false, false, false);
            if (denied.Admitted || callbacks != 1) throw new Exception("GPU full blocked preparation or admitted a third frame.");
            Drain();
            var red = redFrame.Readback.GetAwaiter().GetResult().Pixels;
            var green = greenFrame.Readback.GetAwaiter().GetResult().Pixels;
            if (!red.SequenceEqual(referenceRed.Readback.GetAwaiter().GetResult().Pixels)
                || !green.SequenceEqual(referenceGreen.Readback.GetAwaiter().GetResult().Pixels)
                || red.SequenceEqual(green))
                throw new Exception($"Metal C sequential/retained pixel mismatch: {mode} sigma={sigma}.");
            renderer.CompletePaint(greenFrame.Completion);
            renderer.CompletePaint(redFrame.Completion);
            checks.Add(new { mode, sigma, redSha256 = Convert.ToHexString(SHA256.HashData(red)),
                greenSha256 = Convert.ToHexString(SHA256.HashData(green)), maximumPending = 2 });
            Console.WriteLine($"PASS Metal C pixels, two retained frames, full-queue preparation, reverse retirement: {mode} sigma={sigma}");
        }
        session.StopAcceptingFrames();
        if (session.CanBeginFrame || session.OutstandingFrames != 0) throw new Exception("Metal shutdown did not drain.");
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output))!);
        File.WriteAllText(output, JsonSerializer.Serialize(new { policy = NativeFrameConfiguration.Mode,
            backend = "Graphite-Metal", runtime = RuntimeInformation.FrameworkDescription,
            reference = "C submission with terminal drain between scenes", checks,
            outstandingFramesAfterDrain = session.OutstandingFrames, scanout = "notMeasured",
            hardwareOverlap = "notMeasured", physicalInput = "notVerified" }, new JsonSerializerOptions { WriteIndented = true }));
    }

    private sealed class MetalOwner : IDisposable
    {
        private readonly nint _pool = Native.Send(Native.Send(Native.objc_getClass("NSAutoreleasePool"),
            Native.Selector("alloc")), Native.Selector("init"));
        internal nint Device { get; } = Native.MTLCreateSystemDefaultDevice();
        internal nint Queue { get; }
        internal MetalOwner()
        {
            if (Device == 0) throw new Exception("Metal device unavailable.");
            Queue = Native.Send(Device, Native.Selector("newCommandQueue"));
            if (Queue == 0) throw new Exception("Metal command queue unavailable.");
        }
        internal nint CreateTexture()
        {
            var descriptor = Native.Descriptor(Native.objc_getClass("MTLTextureDescriptor"),
                Native.Selector("texture2DDescriptorWithPixelFormat:width:height:mipmapped:"), 80, 96, 80, false);
            Native.SetInteger(descriptor, Native.Selector("setUsage:"), 5); // ShaderRead | RenderTarget.
            Native.SetInteger(descriptor, Native.Selector("setStorageMode:"), 2); // Private; GPU readback.
            var texture = Native.SendObject(Device, Native.Selector("newTextureWithDescriptor:"), descriptor);
            return texture != 0 ? texture : throw new Exception("Metal output texture allocation failed.");
        }
        internal void WaitForTerminal()
        {
            var marker = Native.Send(Queue, Native.Selector("commandBuffer"));
            if (marker == 0) throw new Exception("Metal terminal marker unavailable.");
            Native.Send(marker, Native.Selector("commit"));
            Native.Send(marker, Native.Selector("waitUntilCompleted"));
            if (Native.Send(marker, Native.Selector("status")) != 4)
                throw new Exception("Metal terminal did not complete successfully.");
        }
        public void Dispose()
        {
            Native.Send(Queue, Native.Selector("release"));
            Native.Send(Device, Native.Selector("release"));
            Native.Send(_pool, Native.Selector("drain"));
        }
    }

    private static class Native
    {
        internal static nint Selector(string name) => sel_registerName(name);
        [DllImport("/System/Library/Frameworks/Metal.framework/Metal")] internal static extern nint MTLCreateSystemDefaultDevice();
        [DllImport("/usr/lib/libobjc.A.dylib")] internal static extern nint objc_getClass(string name);
        [DllImport("/usr/lib/libobjc.A.dylib")] private static extern nint sel_registerName(string name);
        [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")] internal static extern nint Send(nint obj, nint selector);
        [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")] internal static extern nint SendObject(nint obj, nint selector, nint value);
        [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")] internal static extern void SetInteger(nint obj, nint selector, nuint value);
        [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")] internal static extern nint Descriptor(nint obj, nint selector,
            nuint format, nuint width, nuint height, [MarshalAs(UnmanagedType.I1)] bool mipmapped);
    }
}
