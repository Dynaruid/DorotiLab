using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using Silk.NET.Core.Contexts;
using Silk.NET.Core.Native;
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
            ["os"] = RuntimeInformation.OSDescription,
            ["rid"] = RuntimeInformation.RuntimeIdentifier,
            ["productQualified"] = false,
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
                    name is "libSkiaSharp" or "libSkiaSharp.dll" or "libSkiaSharp.so" ? module : 0;
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
                .Single(module => module.ModuleName.Equals(OperatingSystem.IsWindows() ? "libSkiaSharp.dll" : "libSkiaSharp.so", StringComparison.OrdinalIgnoreCase));
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
            if (options.SelfTest == "graphite-device-lost" && !args.Contains("--graphite-extended-context"))
                throw new ArgumentException("graphite-device-lost requires --graphite-extended-context.");
            if (!SKGraphiteContext.IsBackendAvailable(SKGraphiteBackend.Vulkan))
                throw new NotSupportedException("The loaded Skia native asset does not include Graphite/Vulkan.");
            var cycleIndex = Array.IndexOf(args, "--graphite-context-cycles");
            var cycles = 1;
            if (cycleIndex >= 0 && (cycleIndex + 1 >= args.Length ||
                !int.TryParse(args[cycleIndex + 1], out cycles) || cycles is < 1 or > 10))
                throw new ArgumentException("--graphite-context-cycles requires a count from 1 to 10.");
            report["contextGenerationsRequested"] = cycles;
            var completedGenerations = new List<object>();
            report["completedContextGenerations"] = completedGenerations;
            for (var generation = 1; generation <= cycles; generation++)
            {
                report["contextGeneration"] = generation;
                report["contextCreated"] = false;
                report["normalTeardown"] = "notVerified";
                report["offscreenReadback"] = "notVerified";
                report["externalTextureRoundTrip"] = "notVerified";
                report.Remove("offscreenFrames");
                report.Remove("externalTextureFrames");
                ProbeGraphiteDevice(options, report, args.Contains("--graphite-extended-context"));
                completedGenerations.Add(new { generation,
                    contextCreated = report["contextCreated"], normalTeardown = report["normalTeardown"],
                    offscreenReadback = report["offscreenReadback"], externalTextureRoundTrip = report["externalTextureRoundTrip"],
                    externalTextureFrames = report.GetValueOrDefault("externalTextureFrames") });
            }
            if (report.ContainsKey("simulatedDeviceLoss")) report["simulatedDeviceLossTeardown"] = "PASS";
            report["status"] = "PARTIAL";
            report["blocker"] = report.ContainsKey("simulatedDeviceLoss")
                ? "Simulated device-loss propagation/teardown passed; physical device loss and product recovery remain unqualified."
                : report["externalTextureRoundTrip"] as string == "PASS"
                    ? "Diagnostic external texture roundtrip passed; physical device loss, platform present, product device-feature combinations and RID packaging remain unqualified."
                    : "NG1 external state/queue synchronization is not qualified by this run; offscreen success does not qualify NG3/NG5/NG6.";
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

    private static object AssetIdentity(string path)
    {
        var file = new FileInfo(Path.GetFullPath(path));
        var resolved = file.ResolveLinkTarget(returnFinalTarget: true) ?? file;
        var bytes = File.ReadAllBytes(resolved.FullName);
        return new { path = file.FullName, resolvedPath = resolved.FullName, bytes = bytes.LongLength,
            sha256 = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant() };
    }

    private static void SaveGraphiteReport(Options options, Dictionary<string, object?> report)
    {
        if (string.IsNullOrWhiteSpace(options.OutputPath)) return;
        var path = Path.GetFullPath(options.OutputPath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, JsonSerializer.Serialize(report, JsonOptions));
    }

    private static void ProbeGraphiteDevice(Options options, Dictionary<string, object?> report, bool extended)
    {
        var systemLoader = OperatingSystem.IsWindows()
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "vulkan-1.dll")
            : "/usr/lib/x86_64-linux-gnu/libvulkan.so.1";
        var loaderPath = Path.GetFullPath(options.LoaderPath ?? systemLoader);
        if (!options.AllowNonSystemLoader && !loaderPath.Equals(systemLoader, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("A non-system Vulkan loader requires --allow-non-system-loader.");
        report["loader"] = AssetIdentity(loaderPath);
        using var vk = new Vk(new DefaultNativeContext(loaderPath));
        var validation = EnumerateInstanceLayers(vk).Contains("VK_LAYER_KHRONOS_validation") && !options.DisableValidation;
        report["validationEnabled"] = validation;
        report["synchronizationValidationEnabled"] = validation;
        string[] instanceExtensions = validation ? [ExtDebugUtilsExtensionName, "VK_EXT_validation_features"] : [];
        var instance = CreateInstance(vk, instanceExtensions, validation, validation);
        DebugUtilsMessengerEXT messenger = default;
        VkDevice device = default;
        try
        {
            if (validation) messenger = CreateDebugMessenger(vk, instance);
            var selected = SelectDevice(EnumerateDevices(vk, instance, null, default, options), options.DeviceSelector);
            report["device"] = new { selected.Name, selected.VendorId, selected.DeviceId, selected.DriverVersion,
                deviceType = selected.DeviceType.ToString(), software = selected.DeviceType == PhysicalDeviceType.Cpu,
                apiVersion = FormatVersion(selected.ApiVersion), selected.Luid, selected.QueueFamily };
            SaveGraphiteReport(options, report);
            if (selected.DeviceType == PhysicalDeviceType.Cpu && !options.AllowSoftware)
                throw new NotSupportedException("Software Vulkan device rejected.");
            if (selected.ApiVersion < VulkanApiVersion11)
                throw new NotSupportedException("Graphite probe requires Vulkan 1.1.");
            // Keep the stock no-feature baseline. ABI 2 receives the exact same
            // feature chain and names used here by vkCreateDevice.
            var storageQuery = new PhysicalDevice16BitStorageFeatures { SType = StructureType.PhysicalDevice16BitStorageFeatures };
            var supported = new PhysicalDeviceFeatures2 { SType = StructureType.PhysicalDeviceFeatures2, PNext = &storageQuery };
            vk.GetPhysicalDeviceFeatures2(selected.Handle, &supported);
            var storageEnabled = new PhysicalDevice16BitStorageFeatures { SType = StructureType.PhysicalDevice16BitStorageFeatures,
                StorageBuffer16BitAccess = extended && storageQuery.StorageBuffer16BitAccess };
            var enabled = new PhysicalDeviceFeatures2 { SType = StructureType.PhysicalDeviceFeatures2, PNext = &storageEnabled,
                Features = new PhysicalDeviceFeatures { RobustBufferAccess = extended && supported.Features.RobustBufferAccess } };
            string[] deviceExtensions = extended ? ["VK_KHR_maintenance1"] : [];
            RequireAll(selected.Extensions, deviceExtensions, "Graphite probe device extension");
            report["extendedContext"] = extended;
            report["enabledInstanceExtensions"] = instanceExtensions;
            report["enabledDeviceFeatures"] = new { robustBufferAccess = (bool)enabled.Features.RobustBufferAccess,
                storageBuffer16BitAccess = (bool)storageEnabled.StorageBuffer16BitAccess, features2Chain = extended };
            report["enabledDeviceExtensions"] = deviceExtensions;
            var priority = 1f;
            var queueInfo = new DeviceQueueCreateInfo { SType = StructureType.DeviceQueueCreateInfo,
                QueueFamilyIndex = selected.QueueFamily, QueueCount = 1, PQueuePriorities = &priority };
            var deviceInfo = new DeviceCreateInfo { SType = StructureType.DeviceCreateInfo,
                QueueCreateInfoCount = 1, PQueueCreateInfos = &queueInfo, PNext = extended ? &enabled : null };
            var names = (byte**)SilkMarshal.StringArrayToPtr(deviceExtensions);
            try
            {
                deviceInfo.EnabledExtensionCount = (uint)deviceExtensions.Length;
                deviceInfo.PpEnabledExtensionNames = names;
                Check(vk.CreateDevice(selected.Handle, &deviceInfo, null, out device), "vkCreateDevice(Graphite)");
            }
            finally { FreeStringArray(names, deviceExtensions.Length); }
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
            using var extendedContext = extended ? new ExtendedGraphiteContext(vk, instance, selected.Handle,
                device, queue, selected.QueueFamily, &enabled, instanceExtensions, deviceExtensions,
                options.SelfTest == "graphite-device-lost") : null;
            using var context = extendedContext?.Context ?? SKGraphiteContext.CreateVulkan(backend,
                new SKGraphiteContextOptions { GpuBudgetInBytes = 256L * 1024 * 1024 })
                ?? throw new InvalidOperationException("SKGraphiteContext.CreateVulkan returned null.");
            report["contextCreated"] = true;
            if (extended) report["bridgeAbi"] = 2;
            // Exercise dispatch after temporary extension-name arrays are freed;
            // the owner retains the callback through native destruction.
            GC.Collect();
            GC.WaitForPendingFinalizers();
            report["actualBackend"] = context.Backend.ToString();
            if (options.SelfTest == "graphite-device-lost")
            {
                // Context creation itself submits initialization work. Finish it
                // before simulating loss; this is an isolated negative test only.
                Check(vk.DeviceWaitIdle(device), "vkDeviceWaitIdle(before simulated loss)");
                context.CheckAsyncWorkCompletion();
                extendedContext!.ArmDeviceLoss();
                report["lastOperation"] = "injected-device-loss";
                SaveGraphiteReport(options, report);
                using var lostRecorder = context.CreateRecorder(64L * 1024 * 1024)
                    ?? throw new InvalidOperationException("Device-loss probe recorder creation failed before submission.");
                using var lostSurface = SKSurface.Create(lostRecorder, new SKImageInfo(64, 64));
                if (lostSurface is null)
                    throw new InvalidOperationException("Device-loss probe surface/recorder creation failed before submission.");
                lostSurface.Canvas.Clear(SKColors.Red);
                using var lostRecording = lostRecorder.Snap();
                if (lostRecording is null || context.InsertRecording(lostRecording) != SKGraphiteInsertStatus.Success)
                    throw new InvalidOperationException("Device-loss probe failed before submission.");
                if (context.Submit(new SKGraphiteSubmitInfo { Sync = false }) || !context.IsDeviceLost)
                    throw new InvalidOperationException("Injected VK_ERROR_DEVICE_LOST was not reported by Graphite.");
                report["simulatedDeviceLoss"] = "PASS";
                report["physicalDeviceLoss"] = "notVerified";
                report["lastOperation"] = "dispose-lost-context";
                return;
            }
            report["lastOperation"] = "draw/readback";
            SaveGraphiteReport(options, report);
            var frames = new List<object>();
            for (var generation = 1; generation <= 3; generation++)
                frames.Add(ProbeGraphiteFrame(vk, device, context, generation));
            report["offscreenFrames"] = frames;
            report["offscreenReadback"] = "PASS";
            uint bridgeVersion = 0;
            try { bridgeVersion = GraphiteInterop.doroti_graphite_interop_version(); }
            catch (EntryPointNotFoundException) { report["bridgeAbi"] = "notAvailable"; }
            if (bridgeVersion != 0)
            {
                if (bridgeVersion is not (1 or 2)) throw new NotSupportedException($"Unexpected Graphite bridge ABI {bridgeVersion}.");
                report["bridgeAbi"] = bridgeVersion;
                report["lastOperation"] = "external-texture-roundtrip";
                SaveGraphiteReport(options, report);
                var external = new List<object>();
                foreach (var size in new[] { 64, 128, 96 })
                    external.Add(ProbeExternalTexture(vk, selected.Handle, device, queue, selected.QueueFamily,
                        context, size, options.SelfTest == "graphite-after-submit"));
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

    private static object ProbeGraphiteFrame(Vk vk, VkDevice device, SKGraphiteContext context, int generation)
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
        var gpuCompleted = false;
        try
        {
            if (context.InsertRecording(recording) != SKGraphiteInsertStatus.Success)
                throw new InvalidOperationException("InsertRecording failed.");
            byte[]? pixels = null;
            var stride = 0;
            var complete = false;
            Exception? readbackFailure = null;
            context.RequestReadPixels(surface, info, new SKRectI(0, 0, size, size), SKImageRescaleGamma.Src,
                SKImageRescaleMode.Nearest, result =>
                {
                    try
                    {
                        pixels = result?.ToArray(0);
                        stride = result?.GetPlaneRowBytes(0) ?? 0;
                    }
                    catch (Exception exception) { readbackFailure = exception; }
                    finally { complete = true; }
                });
            if (!context.Submit(new SKGraphiteSubmitInfo { Sync = false }))
                throw new InvalidOperationException("Submit failed.");
            var timer = Stopwatch.StartNew();
            while (!complete && timer.Elapsed < TimeSpan.FromSeconds(10))
            {
                context.CheckAsyncWorkCompletion();
                Thread.Sleep(1);
            }
            if (readbackFailure is not null) throw new InvalidOperationException("Async readback callback failed.", readbackFailure);
            if (!complete || pixels is null) throw new InvalidOperationException("Async readback failed or exceeded 10 seconds.");
            gpuCompleted = true;
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
        finally
        {
            // Diagnostic failure drain only. Keep the recording, callback targets
            // and surfaces alive before their using scopes release them.
            if (!gpuCompleted)
            {
                _ = vk.DeviceWaitIdle(device);
                context.CheckAsyncWorkCompletion();
            }
        }
    }
}
