using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using Silk.NET.Core.Contexts;
using Silk.NET.Vulkan;
using SkiaSharp;
using VkDevice = Silk.NET.Vulkan.Device;

namespace Doroti.Validation.WindowsVulkanCapability;

internal static unsafe partial class Program
{
    // NG1 is deliberately a separate result: a Vulkan/D3D11 capability PASS does
    // not qualify Graphite, and an offscreen readback does not qualify presentation.
    private static int RunGraphiteProbe(Options options, string[] args)
    {
        var report = new Dictionary<string, object?>
        {
            ["schema"] = "doroti.native-graphite-probe/v1",
            ["utc"] = DateTimeOffset.UtcNow,
            ["status"] = "FAIL",
            ["externalTextureRoundTrip"] = "notVerified",
            ["platformPresent"] = "notVerified",
            ["physicalScanOut"] = "notVerified",
            ["performance"] = "notVerified",
            ["contextBudgetBytes"] = 256L * 1024 * 1024,
            ["recorderBudgetBytes"] = 64L * 1024 * 1024,
        };
        try
        {
            var nativeIndex = Array.IndexOf(args, "--graphite-native");
            if (nativeIndex >= 0)
            {
                if (nativeIndex + 1 >= args.Length) throw new ArgumentException("--graphite-native requires an absolute library path.");
                var nativePath = Path.GetFullPath(args[nativeIndex + 1]);
                // The process owns this loaded module through exit. Both bindings
                // resolve to this ONE library, including the added C ABI exports.
                var module = NativeLibrary.Load(nativePath);
                nint Resolve(string name, Assembly assembly, DllImportSearchPath? search) =>
                    name is "libSkiaSharp" or "libSkiaSharp.dll" ? module : 0;
                NativeLibrary.SetDllImportResolver(typeof(SKGraphiteContext).Assembly, Resolve);
                NativeLibrary.SetDllImportResolver(typeof(Program).Assembly, Resolve);
                report["requestedNativeAsset"] = AssetIdentity(nativePath);
            }
            report["managedAsset"] = AssetIdentity(typeof(SKGraphiteContext).Assembly.Location);
            report["assemblyVersion"] = typeof(SKGraphiteContext).Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            report["backends"] = Enum.GetValues<SKGraphiteBackend>()
                .Where(value => value != SKGraphiteBackend.Unknown)
                .ToDictionary(value => value.ToString(), SKGraphiteContext.IsBackendAvailable);
            var nativeModule = Process.GetCurrentProcess().Modules.Cast<ProcessModule>()
                .Single(module => module.ModuleName.Equals("libSkiaSharp.dll", StringComparison.OrdinalIgnoreCase));
            report["loadedNativeAsset"] = AssetIdentity(nativeModule.FileName);
            report["publicInteropContract"] = new
            {
                backendContext = typeof(SKGraphiteVkBackendContext).GetProperties().Select(p => p.Name).ToArray(),
                backendTexture = typeof(SKGraphiteBackendTexture).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Select(m => m.ToString()).ToArray(),
                insertRecording = typeof(SKGraphiteInsertRecordingInfo).GetProperties().Select(p => p.Name).ToArray(),
                submit = typeof(SKGraphiteSubmitInfo).GetProperties().Select(p => p.Name).ToArray(),
            };
            SaveGraphiteReport(options, report); // Preserve preflight even after a native abort.
            if (!SKGraphiteContext.IsBackendAvailable(SKGraphiteBackend.Vulkan))
                throw new NotSupportedException("The loaded Skia native asset does not include Graphite/Vulkan.");
            ProbeGraphiteDevice(options, report);
            report["status"] = "PARTIAL";
            report["blocker"] = report["externalTextureRoundTrip"] as string == "PASS"
                ? "Diagnostic external texture roundtrip passed; device recreation/loss, platform present, enabled-feature binding and RID packaging remain unqualified."
                : "NG1 external texture state/queue synchronization requires a same-build native bridge; offscreen success does not qualify NG3/NG5/NG6.";
        }
        catch (Exception exception)
        {
            report["error"] = exception.ToString();
        }
        report["validationWarnings"] = _validationWarnings;
        report["validationErrors"] = _validationErrors;
        report["validationMessages"] = ValidationMessages.ToArray();
        if (_validationWarnings != 0 || _validationErrors != 0) report["status"] = "FAIL";
        SaveGraphiteReport(options, report);
        Console.WriteLine(JsonSerializer.Serialize(report, JsonOptions));
        return report["status"] as string == "PARTIAL" ? 2 : 1;
    }

    private static object AssetIdentity(string path) => new
    {
        path = Path.GetFullPath(path),
        bytes = new FileInfo(path).Length,
        sha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant(),
    };

    private static void SaveGraphiteReport(Options options, Dictionary<string, object?> report)
    {
        if (string.IsNullOrWhiteSpace(options.OutputPath)) return;
        var path = Path.GetFullPath(options.OutputPath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, JsonSerializer.Serialize(report, JsonOptions));
    }

    private static void ProbeGraphiteDevice(Options options, Dictionary<string, object?> report)
    {
        var loaderPath = Path.GetFullPath(options.LoaderPath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.System), "vulkan-1.dll"));
        if (!options.AllowNonSystemLoader && !loaderPath.Equals(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.System), "vulkan-1.dll"), StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("A non-system Vulkan loader requires --allow-non-system-loader.");
        report["loader"] = AssetIdentity(loaderPath);
        using var vk = new Vk(new DefaultNativeContext(loaderPath));
        var validation = EnumerateInstanceLayers(vk).Contains("VK_LAYER_KHRONOS_validation") && !options.DisableValidation;
        report["validationEnabled"] = validation;
        report["synchronizationValidationEnabled"] = validation;
        var instance = CreateInstance(vk, validation ? [ExtDebugUtilsExtensionName, "VK_EXT_validation_features"] : [], validation, validation);
        DebugUtilsMessengerEXT messenger = default;
        VkDevice device = default;
        try
        {
            if (validation) messenger = CreateDebugMessenger(vk, instance);
            var selected = SelectDevice(EnumerateDevices(vk, instance, null, default, options), options.DeviceSelector);
            if (selected.DeviceType == PhysicalDeviceType.Cpu && !options.AllowSoftware)
                throw new NotSupportedException("Software Vulkan device rejected.");
            if (selected.ApiVersion < VulkanApiVersion11)
                throw new NotSupportedException("Graphite probe requires Vulkan 1.1.");
            report["device"] = new { selected.Name, selected.VendorId, selected.DeviceId, selected.DriverVersion,
                apiVersion = FormatVersion(selected.ApiVersion), selected.Luid, selected.QueueFamily };
            // Match the pinned C shim: it cannot accept an enabled-feature chain.
            // Enable no optional device features and do not report supported as enabled.
            report["enabledDeviceFeatures"] = Array.Empty<string>();
            report["enabledDeviceExtensions"] = Array.Empty<string>();
            var priority = 1f;
            var queueInfo = new DeviceQueueCreateInfo { SType = StructureType.DeviceQueueCreateInfo,
                QueueFamilyIndex = selected.QueueFamily, QueueCount = 1, PQueuePriorities = &priority };
            var deviceInfo = new DeviceCreateInfo { SType = StructureType.DeviceCreateInfo,
                QueueCreateInfoCount = 1, PQueueCreateInfos = &queueInfo };
            Check(vk.CreateDevice(selected.Handle, &deviceInfo, null, out device), "vkCreateDevice(Graphite)");
            vk.GetDeviceQueue(device, selected.QueueFamily, 0, out var queue);
            using var backend = new SKGraphiteVkBackendContext
            {
                VkInstance = instance.Handle, VkPhysicalDevice = selected.Handle.Handle,
                VkDevice = device.Handle, VkQueue = queue.Handle, GraphicsQueueIndex = selected.QueueFamily,
                MaxApiVersion = VulkanApiVersion11,
                GetProcedureAddress = (name, inst, dev) => dev != 0
                    ? vk.GetDeviceProcAddr(new VkDevice(dev), name) : vk.GetInstanceProcAddr(new Instance(inst), name),
            };
            report["lastOperation"] = "CreateVulkan";
            SaveGraphiteReport(options, report);
            using var context = SKGraphiteContext.CreateVulkan(backend,
                new SKGraphiteContextOptions { GpuBudgetInBytes = 256L * 1024 * 1024 })
                ?? throw new InvalidOperationException("SKGraphiteContext.CreateVulkan returned null.");
            report["contextCreated"] = true;
            report["actualBackend"] = context.Backend.ToString();
            report["lastOperation"] = "draw/readback";
            SaveGraphiteReport(options, report);
            var frames = new List<object>();
            for (var generation = 1; generation <= 3; generation++)
                frames.Add(ProbeGraphiteFrame(context, generation));
            report["offscreenFrames"] = frames;
            report["offscreenReadback"] = "PASS";
            uint bridgeVersion = 0;
            try { bridgeVersion = GraphiteInterop.doroti_graphite_interop_version(); }
            catch (EntryPointNotFoundException) { report["bridgeAbi"] = "notAvailable"; }
            if (bridgeVersion != 0)
            {
                if (bridgeVersion != 1) throw new NotSupportedException($"Unexpected Graphite bridge ABI {bridgeVersion}.");
                report["bridgeAbi"] = bridgeVersion;
                report["lastOperation"] = "external-texture-roundtrip";
                SaveGraphiteReport(options, report);
                var external = new List<object>();
                foreach (var size in new[] { 64, 128, 96 })
                    external.Add(ProbeExternalTexture(vk, selected.Handle, device, queue, selected.QueueFamily, context, size));
                report["externalTextureFrames"] = external;
                report["externalTextureRoundTrip"] = validation ? "PASS" : "notVerified";
            }
            report["lastOperation"] = "dispose-context";
            SaveGraphiteReport(options, report);
        }
        finally
        {
            // Context is disposed first; normal completion was established by each
            // async readback. No per-frame device-idle or Sync=true qualification.
            if (device.Handle != 0) vk.DestroyDevice(device, null);
            if (messenger.Handle != 0) DestroyDebugMessenger(vk, instance, messenger);
            vk.DestroyInstance(instance, null);
        }
        report["normalTeardown"] = "PASS";
    }

    private static object ProbeGraphiteFrame(SKGraphiteContext context, int generation)
    {
        using var recorder = context.CreateRecorder(64L * 1024 * 1024,
            (owner, raster, mipmapped) => raster.ToTextureImage(owner, mipmapped))
            ?? throw new InvalidOperationException("CreateRecorder returned null.");
        var size = 64 * generation;
        var info = new SKImageInfo(size, size, SKColorType.Rgba8888, SKAlphaType.Premul);
        using var surface = SKSurface.Create(recorder, info)
            ?? throw new InvalidOperationException("Graphite SKSurface.Create returned null.");
        surface.Canvas.Clear(SKColors.Red);
        using var paint = new SKPaint { Color = SKColors.Lime };
        surface.Canvas.DrawRect(0, 0, size / 2, size / 2, paint);
        using var gradient = SKShader.CreateLinearGradient(new SKPoint(0, size / 2), new SKPoint(size, size / 2),
            [SKColors.White, SKColors.Black], SKShaderTileMode.Clamp);
        using var gradientPaint = new SKPaint { Shader = gradient };
        surface.Canvas.DrawRect(0, size / 2, size, size / 4, gradientPaint);
        using var font = new SKFont(SKTypeface.Default, 12);
        using var textPaint = new SKPaint { Color = SKColors.Black, IsAntialias = true };
        surface.Canvas.DrawText("GPU", 4, size / 2 - 3, SKTextAlign.Left, font, textPaint);
        using var offscreen = SKSurface.Create(recorder, new SKImageInfo(8, 8, SKColorType.Rgba8888))
            ?? throw new InvalidOperationException("Offscreen surface allocation failed.");
        offscreen.Canvas.Clear(SKColors.Yellow);
        using var offscreenImage = offscreen.Snapshot();
        surface.Canvas.DrawImage(offscreenImage, 0, size - 8, SKSamplingOptions.Default);
        using var rasterBitmap = new SKBitmap(new SKImageInfo(8, 8));
        rasterBitmap.Erase(SKColors.Blue);
        using var raster = SKImage.FromBitmap(rasterBitmap);
        surface.Canvas.DrawImage(raster, size - 8, size - 8, SKSamplingOptions.Default);
        using var recording = recorder.Snap() ?? throw new InvalidOperationException("Snap returned null.");
        if (context.InsertRecording(recording) != SKGraphiteInsertStatus.Success)
            throw new InvalidOperationException("InsertRecording failed.");
        byte[]? pixels = null;
        var stride = 0;
        var complete = false;
        context.RequestReadPixels(surface, info, new SKRectI(0, 0, size, size), SKImageRescaleGamma.Src,
            SKImageRescaleMode.Nearest, result =>
            {
                pixels = result?.ToArray(0);
                stride = result?.GetPlaneRowBytes(0) ?? 0;
                complete = true;
            });
        if (!context.Submit(new SKGraphiteSubmitInfo { Sync = false }))
            throw new InvalidOperationException("Submit failed.");
        var timer = Stopwatch.StartNew();
        while (!complete && timer.Elapsed < TimeSpan.FromSeconds(10))
        {
            context.CheckAsyncWorkCompletion();
            Thread.Sleep(1);
        }
        if (!complete || pixels is null) throw new InvalidOperationException("Async readback failed or exceeded 10 seconds.");
        if (stride < size * 4 || pixels.Length < stride * (size - 1) + size * 4)
            throw new InvalidOperationException("Invalid async readback stride/length.");
        bool Pixel(int x, int y, byte r, byte g, byte b) => pixels[y * stride + x * 4] == r &&
            pixels[y * stride + x * 4 + 1] == g && pixels[y * stride + x * 4 + 2] == b && pixels[y * stride + x * 4 + 3] == 255;
        if (!Pixel(2, 2, 0, 255, 0) || !Pixel(size - 2, size - 2, 0, 0, 255) ||
            !Pixel(size / 2, 2, 255, 0, 0) || !Pixel(2, size - 2, 255, 255, 0))
            throw new InvalidOperationException("Graphite color/image upload pixel mismatch.");
        return new { generation, size, readbackMilliseconds = timer.Elapsed.TotalMilliseconds,
            pixelCheck = "PASS", rgbaSha256 = Convert.ToHexString(SHA256.HashData(pixels)).ToLowerInvariant() };
    }
}
