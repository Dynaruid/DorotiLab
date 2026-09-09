using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using Doroti.Skia.Rendering;
using Doroti.Skia.RuntimeEffects;
using Foundation;
using Metal;
using SkiaSharp;

namespace Doroti.Validation.AppKitMetalSpike;

internal static class GraphiteMetalContract
{
    internal static object Asset(string path) => new
    {
        path,
        sha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant(),
        bytes = new FileInfo(path).Length,
    };

    internal static object Identity()
    {
        var assembly = typeof(SKGraphiteContext).Assembly;
        var native = Enumerable.Range(0, checked((int)DyldImageCount()))
            .Select(i => Marshal.PtrToStringUTF8(DyldImageName((uint)i)))
            .Where(p => p is not null && Path.GetFileName(p) == "libSkiaSharp.dylib")
            .Select(p => Asset(p!)).ToArray();
        return new
        {
            managed = Asset(assembly.Location),
            version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion,
            native,
            os = RuntimeInformation.OSDescription,
            rid = RuntimeInformation.RuntimeIdentifier,
            metalAvailable = SKGraphiteContext.IsBackendAvailable(SKGraphiteBackend.Metal),
        };
    }

    [DllImport("/usr/lib/libSystem.B.dylib", EntryPoint = "_dyld_image_count")]
    private static extern uint DyldImageCount();
    [DllImport("/usr/lib/libSystem.B.dylib", EntryPoint = "_dyld_get_image_name")]
    private static extern nint DyldImageName(uint index);

    internal static int Run()
    {
        using var pool = new NSAutoreleasePool();
        var report = new Dictionary<string, object?>
        {
            ["schema"] = "doroti.graphite-metal-contract/v1",
            ["status"] = "FAIL",
            ["physicalScanOut"] = "notVerified",
            ["performance"] = "notVerified",
            ["deviceLoss"] = "notVerified",
            ["platformPresent"] = "notVerified; separate live drawable probe",
        };
        var frames = new List<object>();
        report["frames"] = frames;
        try
        {
            if (!SKGraphiteContext.IsBackendAvailable(SKGraphiteBackend.Metal))
                throw new PlatformNotSupportedException("Loaded native Skia lacks Graphite/Metal.");
            report["identity"] = Identity();
            using var device = MTLDevice.SystemDefault ?? throw new PlatformNotSupportedException("No Metal device.");
            using var queue = device.CreateCommandQueue() ?? throw new InvalidOperationException("No Metal queue.");
            report["device"] = device.Name;
            report["contextBudgetBytes"] = SkiaGraphiteSession.ContextBudgetBytes;
            report["recorderBudgetBytes"] = SkiaGraphiteSession.RecorderBudgetBytes;
            report["maxFrames"] = 3;
            Save(report); // Retain the last checkpoint if a native assertion aborts.
            var generation = 0;
            foreach (var size in new[] { 64, 128, 96 })
            {
                using var session = SkiaGraphiteSession.CreateMetal(device.Handle, queue.Handle, ++generation);
                using var descriptor = MTLTextureDescriptor.CreateTexture2DDescriptor(MTLPixelFormat.BGRA8Unorm,
                    (nuint)size, (nuint)size, false);
                descriptor.Usage = MTLTextureUsage.RenderTarget | MTLTextureUsage.ShaderRead;
                descriptor.StorageMode = MTLStorageMode.Private;
                using var texture = device.CreateTexture(descriptor) ?? throw new InvalidOperationException("No external Metal texture.");
                var cancelled = session.BeginMetalFrame(size, size, texture.Handle);
                cancelled.Surface.Canvas.Clear(SKColors.Black);
                cancelled.CancelRecording();
                Assert(session.OutstandingFrames == 0, "Cancelled recording must return its frame.");
                AssertThrows(() => cancelled.Submit(), "Cancelled frame cannot submit.");
                for (var frameNumber = 0; frameNumber < 12; frameNumber++)
                {
                    report["checkpoint"] = $"generation {generation}, frame {frameNumber}";
                    Save(report);
                    var frame = session.BeginMetalFrame(size, size, texture.Handle);
                    Draw(frame.Surface, size, frameNumber % 2 == 0);
                    var readback = frame.RequestReadback(new(size, size, SKColorType.Rgba8888, SKAlphaType.Premul));
                    frame.Submit();
                    AssertThrows(frame.Submit, "Double submission must fail.");
                    // A marker on the SAME queue follows Graphite's async submit.
                    // Its terminal status releases GPU work; it does not claim presentation.
                    using var marker = queue.CommandBuffer() ?? throw new InvalidOperationException("No completion marker.");
                    marker.Commit();
                    WaitForMarker(marker);
                    frame.CompleteGpuWork();
                    Assert(readback.IsCompletedSuccessfully, "Readback callback must finish at GPU completion.");
                    var result = readback.GetAwaiter().GetResult();
                    Pixel(result, 2, 2, 0, 255, 0);
                    Pixel(result, size - 2, size - 2, 0, 0, 255);
                    Pixel(result, 2, size - 2, 255, 255, 0);
                    Pixel(result, size / 2, size - 4, 255, 0, 255);
                    Pixel(result, size - 2, 2, frameNumber % 2 == 0 ? (byte)255 : (byte)0, 0,
                        frameNumber % 2 == 0 ? (byte)0 : (byte)255);
                    AssertThrows(frame.CompleteGpuWork, "Duplicate GPU completion must fail.");
                    frames.Add(new { generation, size, frameNumber, pixelCheck = "PASS",
                        rgbaSha256 = Convert.ToHexString(SHA256.HashData(result.Pixels)).ToLowerInvariant() });
                }
                Assert(session.OutstandingFrames == 0, "All frame resources must return.");
                session.StopAcceptingFrames();
                Assert(!session.CanBeginFrame, "Stopped session must reject new frames.");
            }
            ProbeOwnership(device, queue);
            report["ownership"] = "PASS: bounded frames, owner thread, independent sessions, pending-work disposal rejection";
            report["normalTeardown"] = "PASS";
            report["status"] = "PASS";
        }
        catch (Exception exception) { report["error"] = exception.ToString(); }
        Save(report);
        Console.WriteLine(JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true }));
        return report["status"] as string == "PASS" ? 0 : 1;
    }

    private static void ProbeOwnership(IMTLDevice device, IMTLCommandQueue queue)
    {
        using var first = SkiaGraphiteSession.CreateMetal(device.Handle, queue.Handle, 10, maxFrames: 1);
        using var second = SkiaGraphiteSession.CreateMetal(device.Handle, queue.Handle, 11, maxFrames: 1);
        using var descriptor = MTLTextureDescriptor.CreateTexture2DDescriptor(MTLPixelFormat.BGRA8Unorm, 32, 32, false);
        descriptor.Usage = MTLTextureUsage.RenderTarget | MTLTextureUsage.ShaderRead;
        using var a = device.CreateTexture(descriptor)!;
        using var b = device.CreateTexture(descriptor)!;
        var fa = first.BeginMetalFrame(32, 32, a.Handle);
        var fb = second.BeginMetalFrame(32, 32, b.Handle);
        Assert(!ReferenceEquals(SkiaGpuSurfaces.RecorderFor(fa.Surface.Canvas), SkiaGpuSurfaces.RecorderFor(fb.Surface.Canvas)),
            "Views must have independent recorders and image providers.");
        var crossThreadRejected = false;
        var thread = new Thread(() =>
        {
            try { _ = first.CanBeginFrame; }
            catch (InvalidOperationException) { crossThreadRejected = true; }
        });
        thread.Start();
        Assert(thread.Join(TimeSpan.FromSeconds(5)) && crossThreadRejected, "Cross-thread session use must fail.");
        fa.Surface.Canvas.Clear(SKColors.Red);
        fb.Surface.Canvas.Clear(SKColors.Blue);
        fa.Submit();
        fb.Submit();
        Assert(!first.CanBeginFrame, "In-flight bound must apply after submission.");
        AssertThrows(first.Dispose, "Pending work must prevent context destruction.");
        using var marker = queue.CommandBuffer()!;
        marker.Commit();
        WaitForMarker(marker);
        fa.CompleteGpuWork();
        fb.CompleteGpuWork();
    }

    private static void WaitForMarker(IMTLCommandBuffer marker)
    {
        var timer = Stopwatch.StartNew();
        while (marker.Status is not (MTLCommandBufferStatus.Completed or MTLCommandBufferStatus.Error) &&
            timer.Elapsed < TimeSpan.FromSeconds(10)) Thread.Sleep(1);
        Assert(marker.Status == MTLCommandBufferStatus.Completed,
            $"Metal marker failed/timed out: {marker.Status}: {marker.Error?.LocalizedDescription}");
    }

    private static void Draw(SKSurface surface, int size, bool red)
    {
        surface.Canvas.Clear(red ? SKColors.Red : SKColors.Blue);
        using var paint = new SKPaint { Color = SKColors.Lime };
        surface.Canvas.DrawRect(0, 0, size / 2, size / 2, paint);
        using var gradient = SKShader.CreateLinearGradient(new(0, size / 2), new(size, size / 2),
            [SKColors.White, SKColors.Black], SKShaderTileMode.Clamp);
        using var gradientPaint = new SKPaint { Shader = gradient };
        surface.Canvas.DrawRect(0, size / 2, size, size / 4, gradientPaint);
        using var font = new SKFont(SKTypeface.Default, 12);
        using var textPaint = new SKPaint { Color = SKColors.Black, IsAntialias = true };
        surface.Canvas.DrawText("GPU", 4, size / 2 - 3, SKTextAlign.Left, font, textPaint);
        using var offscreen = SkiaGpuSurfaces.CreateCompatible(surface.Canvas, new(8, 8, SKColorType.Bgra8888), null);
        offscreen.Canvas.Clear(SKColors.Yellow);
        using var image = offscreen.Snapshot();
        surface.Canvas.DrawImage(image, 0, size - 8, SKSamplingOptions.Default);
        using var bitmap = new SKBitmap(new SKImageInfo(8, 8));
        bitmap.Erase(SKColors.Blue);
        using var raster = SKImage.FromBitmap(bitmap);
        surface.Canvas.DrawImage(raster, size - 8, size - 8, SKSamplingOptions.Default);
        using var effect = SKRuntimeEffect.CreateShader("half4 main(float2 p) { return half4(1,0,1,1); }", out var errors)
            ?? throw new InvalidOperationException(errors);
        using var uniforms = new SKRuntimeEffectUniforms(effect);
        using var children = new SKRuntimeEffectChildren(effect);
        using var shader = effect.ToShader(uniforms, children);
        using var shaderPaint = new SKPaint { Shader = shader };
        surface.Canvas.DrawRect(size / 2 - 4, size - 8, 8, 8, shaderPaint);
        using var blur = SKImageFilter.CreateBlur(2, 2);
        using var blurPaint = new SKPaint { Color = SKColors.Cyan, ImageFilter = blur };
        surface.Canvas.DrawCircle(size - 12, size / 3, 4, blurPaint);
    }

    private static void Pixel(SkiaGraphiteReadback result, int x, int y, byte r, byte g, byte b)
    {
        var i = y * result.RowBytes + x * 4;
        Assert(result.Pixels[i] == r && result.Pixels[i + 1] == g && result.Pixels[i + 2] == b && result.Pixels[i + 3] == 255,
            $"Pixel mismatch at {x},{y}.");
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private static void AssertThrows(Action action, string message)
    {
        try { action(); }
        catch (InvalidOperationException) { return; }
        throw new InvalidOperationException(message);
    }

    private static void Save(object report)
    {
        var path = Environment.GetEnvironmentVariable("DOROTI_APPKIT_SPIKE_EVIDENCE");
        if (string.IsNullOrWhiteSpace(path)) return;
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        File.WriteAllText(path, JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true }));
    }
}
