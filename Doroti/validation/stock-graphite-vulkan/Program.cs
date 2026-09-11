using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using Silk.NET.Core.Contexts;
using Silk.NET.Vulkan;
using SkiaSharp;
using VkDevice = Silk.NET.Vulkan.Device;

// Independent stock-asset experiment: no Doroti project, custom ABI, or native build.
internal static unsafe partial class Program
{
    private static readonly Dictionary<string, object?> Report = new()
    {
        ["stage"] = "W0-1", ["status"] = "FAIL", ["evidenceKind"] = "automated-headless",
        ["product"] = "notVerified", ["presentation"] = "notVerified", ["performance"] = "notVerified",
        ["normalTeardown"] = "notVerified", ["externalTimeoutSeconds"] = 1200,
        ["packageVersion"] = "4.154.0-preview.1.26454.9", ["utc"] = DateTimeOffset.UtcNow,
        ["os"] = RuntimeInformation.OSDescription, ["rid"] = RuntimeInformation.RuntimeIdentifier,
    };
    private static readonly List<string> Messages = [];
    private static readonly HashSet<string> Procedures = [];
    private static readonly DebugUtilsMessengerCallbackFunctionEXT Callback = Validation;
    private static string Output = "report.json";
    private static int ValidationCount;
    private static bool CallbackFault;
    private static nint SkiaModule; // Process lifetime, also while native finalizers run.
    private const uint ApiVersion = (1u << 22) | (2u << 12);
    private static bool ShutdownProbe;
    private static string Mode = "";
    private static bool RetirementProbe => Mode.StartsWith("retire-", StringComparison.Ordinal);
    private static bool CopyProbe => Mode is "copy-one" or "copy-two" or "copy-msaa" or "copy-v2";
    private static bool NeedsTimeline => ShutdownProbe || Mode == "retire-delayed";
    private static readonly RetirementAdmission Retirement = new();
    private static long RetirementCloseAt;
    private static double FullNativeRetirementMilliseconds;

    private static int Main(string[] args)
    {
        try
        {
            if (args.Length == 1 && args[0] == "--journal-tests")
            { Console.WriteLine(JsonSerializer.Serialize(SubmissionJournalTests.Run())); return 0; }
            if (args.Length != 4) throw new ArgumentException("Usage: StockGraphite <official-dll> <GPU-name-substring> <sync|async|shutdown|retire-ready|retire-delayed|retire-cancel> <report.json>");
            Output = Path.GetFullPath(args[3]);
            Directory.CreateDirectory(Path.GetDirectoryName(Output)!);
            Mode = args[2];
            if (Mode is not ("sync" or "async" or "shutdown" or "retire-ready" or "retire-delayed" or "retire-cancel" or "copy-one" or "copy-two" or "copy-msaa" or "copy-v2" or "session")) throw new ArgumentException("Unknown submit mode.");
            ShutdownProbe = args[2] == "shutdown";
            if (ShutdownProbe) Report["stage"] = "W0-4-early-feasibility";
            if (RetirementProbe) Report["stage"] = "W0-4-alternative-retirement";
            if (CopyProbe) Report["stage"] = "W0-2-W0-3-observed-copy";
            if (Mode == "session") Report["stage"] = "W0-6-shared-session";
            Report["mode"] = Mode;
            Report["sync"] = args[2] == "sync";
            var path = Path.GetFullPath(args[0]);
            Report["requestedAsset"] = Identity(path);
            SkiaModule = NativeLibrary.Load(path);
            // Reject custom assets, including an accidentally staged product DLL.
            if (NativeLibrary.TryGetExport(SkiaModule, "doroti_graphite_interop_version", out _))
                throw new InvalidOperationException("Custom Graphite asset rejected.");
#if SHARED_SESSION
            if (Mode == "session")
            {
                var asset = new Doroti.Skia.Vulkan.OfficialGraphiteAsset(path, "SkiaSharp.NativeAssets.Win32", "4.154.0-preview.1.26454.9", "win-x64",
                    "07ce51fd59e099b9561b0327223c27b21aa5605b5b8f4484dd297fdb8c8725a1", "7c8cdb451146fcb12899899286e279e6aa615a5fc9b610f01137a741819f210f");
                bool hashRejected = false;
                try { Doroti.Skia.Vulkan.GraphiteNativeLibrary.ConfigureOfficial(asset with { Sha256 = new string('0', 64) }); }
                catch (InvalidDataException) { hashRejected = true; }
                if (!hashRejected) throw new InvalidOperationException("Official loader accepted an incorrect provenance hash.");
                Doroti.Skia.Vulkan.GraphiteNativeLibrary.ConfigureOfficial(asset);
                Doroti.Skia.Vulkan.GraphiteNativeLibrary.ConfigureOfficial(asset); // Same selection is idempotent.
                bool differentAssetRejected = false;
                try { Doroti.Skia.Vulkan.GraphiteNativeLibrary.ConfigureOfficial(asset with { Sha256 = new string('1', 64) }); }
                catch (InvalidOperationException) { differentAssetRejected = true; }
                if (!differentAssetRejected) throw new InvalidOperationException("Official loader allowed a different asset identity.");
                Report["productLoader"] = new { status = "PASS", hashRejected, differentAssetRejected, idempotentSelection = true };
            }
            else
#endif
                NativeLibrary.SetDllImportResolver(typeof(SKGraphiteContext).Assembly,
                    (name, _, _) => name is "libSkiaSharp" or "libSkiaSharp.dll" ? SkiaModule : 0);
            Report["managedAsset"] = Identity(typeof(SKGraphiteContext).Assembly.Location);
            Report["publicContextApi"] = typeof(SKGraphiteContext).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(m => m.ToString()).Order().ToArray();
            Report["vulkanAvailable"] = SKGraphiteContext.IsBackendAvailable(SKGraphiteBackend.Vulkan);
            Report["loadedAsset"] = Identity(Process.GetCurrentProcess().Modules.Cast<ProcessModule>()
                .Single(m => m.ModuleName.Equals("libSkiaSharp.dll", StringComparison.OrdinalIgnoreCase)).FileName);
            if (!SKGraphiteContext.IsBackendAvailable(SKGraphiteBackend.Vulkan)) throw new NotSupportedException("Graphite/Vulkan unavailable.");
            if (RetirementProbe)
            {
                var generations = new List<object>();
                Report["retirementGenerations"] = generations;
                for (var generation = 1; generation <= 3; generation++)
                {
                    if (!Retirement.TryStart()) throw new InvalidOperationException("Previous generation still owns the retirement slot.");
                    Report["generation"] = generation;
                    Run(args[1], false);
                    generations.Add(new { protocol = Report["retirementResult"], fullNativeRetirementMilliseconds = FullNativeRetirementMilliseconds,
                        nativeObjectsReleased = Retirement.Live == 0 });
                }
                Report["peakLiveGenerations"] = Retirement.Peak;
                Report["liveGenerationsAfterRun"] = Retirement.Live;
                Report["originalFiveSecondFullRetirementGate"] = Mode == "retire-delayed" ? "FAIL: controlled GPU completion is later than five seconds" : "PASS: scoped diagnostic";
                Report["windowPresentation"] = "notVerified: close is a managed coordinator acknowledgement only";
            }
            else if (Mode == "session")
            {
                var generations = new List<object>(); Report["sessionGenerations"] = generations;
                for (int generation = 1; generation <= 3; generation++)
                { Report["generation"] = generation; Run(args[1], false); generations.Add(new { session = Report["sessionResult"], observer = Report["observerAfterContext"] }); }
            }
            else if (CopyProbe)
            {
                var generations = new List<object>(); Report["copyGenerations"] = generations;
                for (int generation = 1; generation <= 3; generation++)
                { Report["generation"] = generation; Run(args[1], false); generations.Add(new { copy = Report["copyResult"], observer = Report["observerAfterContext"] }); }
            }
            else Run(args[1], args[2] == "sync");
            if (ValidationCount != 0 || CallbackFault) throw new InvalidOperationException("Vulkan validation or callback failure; candidate rejected.");
            Report["status"] = ShutdownProbe && Report["shutdownWithinDeadline"] is false ? "FAIL" : "PASS";
        }
        catch (Exception e) { Report["error"] = e.ToString(); }
        Save();
        Console.WriteLine(JsonSerializer.Serialize(Report, new JsonSerializerOptions { WriteIndented = true }));
        return Report["status"] as string == "PASS" ? 0 : 1;
    }

    private static object Identity(string path) => new { path, bytes = new FileInfo(path).Length,
        sha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant() };
    private static void Save()
    {
        lock (Messages)
        {
            Report["validationCount"] = ValidationCount;
            Report["validationMessages"] = Messages.ToArray();
            Report["callbackFault"] = CallbackFault;
        }
        Report["requestedProcedures"] = Procedures.Order().ToArray();
        File.WriteAllText(Output, JsonSerializer.Serialize(Report, new JsonSerializerOptions { WriteIndented = true }));
    }
    private static void Step(string operation) { Report["lastOperation"] = operation; Save(); }
    private static void Check(Result result, string name)
    { if (result != Result.Success) throw new InvalidOperationException($"{name}: {result}"); }
    private static uint Validation(DebugUtilsMessageSeverityFlagsEXT severity, DebugUtilsMessageTypeFlagsEXT type,
        DebugUtilsMessengerCallbackDataEXT* data, void* user)
    {
        try
        {
            var message = Marshal.PtrToStringUTF8((nint)data->PMessage) ?? "Missing validation message";
            lock (Messages) { ValidationCount++; if (Messages.Count < 256) Messages.Add($"{severity}: {message}"); }
            Console.Error.WriteLine(message);
        }
        catch { CallbackFault = true; }
        return 0; // Preserve real driver behavior. Never manufacture Vulkan results.
    }

    private static void Run(string selector, bool sync)
    {
        var loader = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "vulkan-1.dll");
        Report["loader"] = Identity(loader);
        using var vk = new Vk(new DefaultNativeContext(loader));
        uint version = 0;
        Check(vk.EnumerateInstanceVersion(&version), "loader version");
        Report["loaderApiVersion"] = version;
        var app = new ApplicationInfo { SType = StructureType.ApplicationInfo, ApiVersion = ApiVersion };
        using var names = new Utf8Names("VK_EXT_debug_utils", "VK_EXT_validation_features", "VK_LAYER_KHRONOS_validation");
        var feature = ValidationFeatureEnableEXT.SynchronizationValidationExt;
        var validation = new ValidationFeaturesEXT { SType = StructureType.ValidationFeaturesExt,
            EnabledValidationFeatureCount = 1, PEnabledValidationFeatures = &feature };
        var debugInfo = new DebugUtilsMessengerCreateInfoEXT { SType = StructureType.DebugUtilsMessengerCreateInfoExt,
            MessageSeverity = DebugUtilsMessageSeverityFlagsEXT.WarningBitExt | DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt,
            MessageType = DebugUtilsMessageTypeFlagsEXT.GeneralBitExt | DebugUtilsMessageTypeFlagsEXT.ValidationBitExt | DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt,
            PfnUserCallback = new PfnDebugUtilsMessengerCallbackEXT(Callback) };
        validation.PNext = &debugInfo;
        var ci = new InstanceCreateInfo { SType = StructureType.InstanceCreateInfo, PApplicationInfo = &app,
            PNext = &validation, EnabledExtensionCount = 2, PpEnabledExtensionNames = names.Pointer,
            EnabledLayerCount = 1, PpEnabledLayerNames = names.Pointer + 2 };
        Report["enabledInstanceExtensions"] = new[] { "VK_EXT_debug_utils", "VK_EXT_validation_features" };
        Report["validationEnabled"] = true;
        Report["synchronizationValidationEnabled"] = true;
        Report["enabledDeviceExtensions"] = Array.Empty<string>();
        Report["enabledOptionalDeviceFeatures"] = Array.Empty<string>();
        Step("vkCreateInstance");
        Check(vk.CreateInstance(&ci, null, out var instance), "instance");
        VkDevice device = default;
        DebugUtilsMessengerEXT messenger = default;
        try
        {
            var createDebug = (delegate* unmanaged<nint, DebugUtilsMessengerCreateInfoEXT*, void*, DebugUtilsMessengerEXT*, Result>)
                (nint)vk.GetInstanceProcAddr(instance, "vkCreateDebugUtilsMessengerEXT");
            if (createDebug == null) throw new NotSupportedException("Debug utils unavailable.");
            Check(createDebug(instance.Handle, &debugInfo, null, &messenger), "debug messenger");
            uint count = 0;
            Check(vk.EnumeratePhysicalDevices(instance, &count, null), "physical count");
            var devices = new PhysicalDevice[count];
            fixed (PhysicalDevice* p = devices) Check(vk.EnumeratePhysicalDevices(instance, &count, p), "physical devices");
            PhysicalDevice selected = default;
            foreach (var candidate in devices)
            {
                vk.GetPhysicalDeviceProperties(candidate, out var props);
                var name = Marshal.PtrToStringUTF8((nint)props.DeviceName)!;
                if (!name.Contains(selector, StringComparison.OrdinalIgnoreCase)) continue;
                if (selected.Handle != 0) throw new ArgumentException("Ambiguous GPU selector.");
                if (props.DeviceType == PhysicalDeviceType.Cpu) throw new NotSupportedException("Software device rejected.");
                if (props.ApiVersion < ApiVersion) throw new NotSupportedException("Official stock profile requires Vulkan 1.2 for core driver properties.");
                selected = candidate;
                Report["device"] = new { name, props.VendorID, props.DeviceID, props.DriverVersion, props.ApiVersion, type = props.DeviceType.ToString() };
            }
            if (selected.Handle == 0) throw new NotSupportedException("GPU not found: " + selector);
            uint familyCount = 0;
            vk.GetPhysicalDeviceQueueFamilyProperties(selected, &familyCount, null);
            var families = new QueueFamilyProperties[familyCount];
            fixed (QueueFamilyProperties* p = families) vk.GetPhysicalDeviceQueueFamilyProperties(selected, &familyCount, p);
            uint family = checked((uint)Array.FindIndex(families, f => f.QueueCount > 0 && (f.QueueFlags & QueueFlags.GraphicsBit) != 0));
            float priority = 1;
            var qci = new DeviceQueueCreateInfo { SType = StructureType.DeviceQueueCreateInfo, QueueFamilyIndex = family, QueueCount = 1, PQueuePriorities = &priority };
            var dci = new DeviceCreateInfo { SType = StructureType.DeviceCreateInfo, QueueCreateInfoCount = 1, PQueueCreateInfos = &qci };
            using var timelineName = new Utf8Names("VK_KHR_timeline_semaphore");
            using var version2Names = new Utf8Names("VK_KHR_synchronization2", "VK_KHR_create_renderpass2");
            var synchronization2 = new PhysicalDeviceSynchronization2Features { SType = StructureType.PhysicalDeviceSynchronization2Features };
            var timelineFeature = new PhysicalDeviceTimelineSemaphoreFeatures { SType = StructureType.PhysicalDeviceTimelineSemaphoreFeatures };
            var features2 = new PhysicalDeviceFeatures2 { SType = StructureType.PhysicalDeviceFeatures2, PNext = &timelineFeature };
            vk.GetPhysicalDeviceFeatures2(selected, &features2);
            if (NeedsTimeline)
            {
                if (!timelineFeature.TimelineSemaphore) throw new NotSupportedException("Timeline host stall probe unavailable.");
                dci.PNext = &timelineFeature;
                dci.EnabledExtensionCount = 1; dci.PpEnabledExtensionNames = timelineName.Pointer;
                Report["enabledDeviceExtensions"] = new[] { "VK_KHR_timeline_semaphore" };
                Report["enabledOptionalDeviceFeatures"] = new[] { "timelineSemaphore (host-only controlled wait fixture)" };
            }
            if (Mode == "copy-v2")
            {
                features2.PNext = &synchronization2; vk.GetPhysicalDeviceFeatures2(selected, &features2);
                if (!synchronization2.Synchronization2) throw new NotSupportedException("Synchronization2 unavailable.");
                dci.PNext = &synchronization2; dci.EnabledExtensionCount = 2; dci.PpEnabledExtensionNames = version2Names.Pointer;
                Report["enabledDeviceExtensions"] = new[] { "VK_KHR_synchronization2", "VK_KHR_create_renderpass2" };
                Report["enabledOptionalDeviceFeatures"] = new[] { "synchronization2 (host adapter test)" };
            }
            Step("vkCreateDevice");
            Check(vk.CreateDevice(selected, &dci, null, out device), "device");
            vk.GetDeviceQueue(device, family, 0, out var queue);
            Report["queueFamily"] = family;
            using var waitObserver = new WaitObserver(vk, device);
            using var imageObserver = CopyProbe || Mode == "session" ? new VulkanObserver(vk, instance, device, queue, family) : null;
            using var backend = new SKGraphiteVkBackendContext { VkInstance = instance.Handle, VkPhysicalDevice = selected.Handle,
                VkDevice = device.Handle, VkQueue = queue.Handle, GraphicsQueueIndex = family, MaxApiVersion = ApiVersion,
                GetProcedureAddress = (name, inst, dev) =>
                {
                    try { Procedures.Add(name); return imageObserver != null ? imageObserver.Resolve(name, inst, dev) : (ShutdownProbe || RetirementProbe) && name == "vkWaitForFences" ? waitObserver.Address : dev != 0 ? vk.GetDeviceProcAddr(new VkDevice(dev), name) : vk.GetInstanceProcAddr(new Instance(inst), name); }
                    catch { CallbackFault = true; return 0; }
                } };
            Step("SKGraphiteContext.CreateVulkan");
            using (var context = SKGraphiteContext.CreateVulkan(backend, new SKGraphiteContextOptions { GpuBudgetInBytes = 256L * 1024 * 1024, InternalMultisampleCount = Mode == "copy-msaa" ? 4 : 0 })
                ?? throw new InvalidOperationException("Public CreateVulkan returned null."))
            {
                Report["contextCreated"] = true;
                Report["backend"] = context.Backend.ToString();
                GC.Collect(); GC.WaitForPendingFinalizers();
                if (Mode == "session")
                {
#if SHARED_SESSION
                    ProbeSharedSession(vk, instance, selected, device, queue, family, imageObserver!);
#else
                    throw new NotSupportedException("Use the stock-graphite-session project for product session validation.");
#endif
                }
                else if (CopyProbe) ProbeCopy(vk, selected, device, queue, family, context, imageObserver!);
                else if (RetirementProbe) ProbeRetirement(vk, device, queue, context, waitObserver);
                else if (ShutdownProbe) ProbeShutdown(vk, device, queue, context, waitObserver);
                else Draw(context, sync);
                Step("dispose-context");
            }
            Report["contextDisposed"] = true;
            if (imageObserver != null) { Report["observerAfterContext"] = imageObserver.Summary(); imageObserver.Check(); }
        }
        finally
        {
            Step("destroy-device-instance");
            if (device.Handle != 0) vk.DestroyDevice(device, null);
            if (messenger.Handle != 0)
            {
                var destroy = (delegate* unmanaged<nint, DebugUtilsMessengerEXT, void*, void>)(nint)vk.GetInstanceProcAddr(instance, "vkDestroyDebugUtilsMessengerEXT");
                destroy(instance.Handle, messenger, null);
            }
            vk.DestroyInstance(instance, null);
            GC.KeepAlive(Callback);
        }
        if (RetirementProbe)
        {
            Retirement.Complete(); // Includes actual VkDevice/instance teardown.
            FullNativeRetirementMilliseconds = Stopwatch.GetElapsedTime(RetirementCloseAt).TotalMilliseconds;
            if (Mode != "retire-delayed" && FullNativeRetirementMilliseconds >= 5000)
                throw new InvalidOperationException("Full native retirement exceeded five seconds.");
        }
        Report["normalTeardown"] = "PASS";
    }

    private static void Draw(SKGraphiteContext context, bool sync)
    {
        using var recorder = context.CreateRecorder(64L * 1024 * 1024,
            (owner, raster, mipmapped) => raster.ToTextureImage(owner, mipmapped)) ?? throw new InvalidOperationException("Recorder null.");
        var info = new SKImageInfo(128, 128, SKColorType.Rgba8888, SKAlphaType.Premul);
        using var surface = SKSurface.Create(recorder, info) ?? throw new InvalidOperationException("Surface null.");
        var frames = new List<object>(); Report["frames"] = frames;
        for (var frame = 0; frame < 2; frame++)
        {
            Step("draw-" + frame);
            var canvas = surface.Canvas;
            canvas.Clear(frame == 0 ? SKColors.White : SKColors.Black);
            using var green = new SKPaint { Color = SKColors.Lime };
            canvas.DrawRect(0, 0, 16, 16, green);
            using var bitmap = new SKBitmap(8, 8); bitmap.Erase(SKColors.Blue);
            for (var y = 0; y < 8; y++) for (var x = 4; x < 8; x++) bitmap.SetPixel(x, y, new SKColor(255, 0, 0, 128));
            using var image = SKImage.FromBitmap(bitmap);
            canvas.DrawImage(image, 112, 112, SKSamplingOptions.Default);
            using var gradient = SKShader.CreateLinearGradient(new(20, 20), new(100, 20), [SKColors.Red, SKColors.Blue], null, SKShaderTileMode.Clamp);
            using var paint = new SKPaint { Shader = gradient };
            canvas.DrawRect(20, 20, 80, 12, paint);
            using var font = new SKFont(SKTypeface.Default, 16);
            using var text = new SKPaint { Color = SKColors.Magenta, IsAntialias = true };
            canvas.DrawText("Graphite", 5, 60, SKTextAlign.Left, font, text);
            canvas.Save(); canvas.ClipRect(new SKRect(20, 70, 60, 100));
            using var alpha = new SKPaint { Color = new SKColor(255, 0, 0, 128) };
            canvas.DrawRect(10, 65, 60, 45, alpha); canvas.Restore();
            using var effect = SKRuntimeEffect.CreateShader("half4 main(float2 p) { return half4(1, 1, 0, 1); }", out var errors)
                ?? throw new InvalidOperationException("Runtime shader: " + errors);
            using var shader = effect.ToShader();
            using var effectPaint = new SKPaint { Shader = shader };
            canvas.DrawRect(80, 75, 15, 15, effectPaint);
            using var blur = SKImageFilter.CreateBlur(2, 2);
            using var blurPaint = new SKPaint { Color = SKColors.Cyan, ImageFilter = blur };
            canvas.DrawCircle(100, 50, 8, blurPaint);
            using var recording = recorder.Snap() ?? throw new InvalidOperationException("Snap null.");
            Step("insert-" + frame);
            if (context.InsertRecording(recording) != SKGraphiteInsertStatus.Success) throw new InvalidOperationException("Insert failed.");
            Step("submit-" + frame);
            if (!context.Submit(new SKGraphiteSubmitInfo { Sync = sync })) throw new InvalidOperationException("Submit failed.");
            byte[]? pixels = null; int stride = 0; bool complete = false; Exception? callbackError = null;
            context.RequestReadPixels(surface, info, new(0, 0, 128, 128), SKImageRescaleGamma.Src, SKImageRescaleMode.Nearest, result =>
            {
                try { pixels = result?.ToArray(0); stride = result?.GetPlaneRowBytes(0) ?? 0; }
                catch (Exception e) { callbackError = e; }
                finally { complete = true; }
            });
            if (!context.Submit(new SKGraphiteSubmitInfo { Sync = sync })) throw new InvalidOperationException("Readback submit failed.");
            var timer = Stopwatch.StartNew();
            while (!complete && timer.Elapsed.TotalSeconds < 5) { context.CheckAsyncWorkCompletion(); Thread.Sleep(1); }
            // A readback timeout is a failed diagnostic; it never authorizes product resource reuse.
            if (!complete || pixels is null || callbackError != null) throw new InvalidOperationException("Readback incomplete.", callbackError);
            if (stride < 512 || pixels.Length < stride * 128) throw new InvalidOperationException("Invalid readback dimensions.");
            void Pixel(int x, int y, int r, int g, int b, int tolerance = 0)
            {
                var i = y * stride + x * 4;
                if (Math.Abs(pixels[i] - r) > tolerance || Math.Abs(pixels[i + 1] - g) > tolerance || Math.Abs(pixels[i + 2] - b) > tolerance || pixels[i + 3] != 255)
                    throw new InvalidOperationException($"Pixel ({x},{y}): {pixels[i]},{pixels[i+1]},{pixels[i+2]},{pixels[i+3]} expected {r},{g},{b},255.");
            }
            Pixel(2, 2, 0, 255, 0); Pixel(115, 115, 0, 0, 255); Pixel(85, 80, 255, 255, 0);
            Pixel(25, 80, 255 * (1 - frame) + 128 * frame, 127 * (1 - frame), 127 * (1 - frame), 1);
            Pixel(118, 115, 255 * (1 - frame) + 128 * frame, 127 * (1 - frame), 127 * (1 - frame), 1);
            Pixel(15, 80, 255 * (1 - frame), 255 * (1 - frame), 255 * (1 - frame)); // Outside clip.
            Pixel(60, 25, 126, 0, 129, 2); // Gradient interior, no edge AA.
            Pixel(100, 50, 0, 255, 255, 2); // Blur interior.
            int textPixels = 0, haloPixels = 0;
            for (int y = 43; y < 60; y++) for (int x = 5; x < 75; x++)
            {
                var i = y * stride + x * 4;
                if (pixels[i] > pixels[i + 1] + 30 && pixels[i + 2] > pixels[i + 1] + 30) textPixels++;
            }
            for (int y = 38; y < 63; y++) for (int x = 88; x < 113; x++)
            {
                if ((x - 100) * (x - 100) + (y - 50) * (y - 50) < 100) continue;
                var i = y * stride + x * 4;
                if (pixels[i + 1] > pixels[i] + 2 && pixels[i + 2] > pixels[i] + 2) haloPixels++;
            }
            if (textPixels < 40 || haloPixels < 10) throw new InvalidOperationException($"Missing text/filter coverage: text={textPixels}, halo={haloPixels}.");
            File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(Output)!, $"frame-{frame}.rgba"), pixels);
            using var capture = new SKBitmap(info);
            for (int row = 0; row < 128; row++) Marshal.Copy(pixels, row * stride, capture.GetPixels() + row * capture.RowBytes, 512);
            using var encoded = capture.Encode(SKEncodedImageFormat.Png, 100);
            using (var file = File.Create(Path.Combine(Path.GetDirectoryName(Output)!, $"frame-{frame}.png"))) encoded.SaveTo(file);
            frames.Add(new { frame, pixelChecks = "PASS", textPixels, haloPixels, rgbaSha256 = Convert.ToHexString(SHA256.HashData(pixels)), stride });
            Save();
        }
    }

    private sealed class Utf8Names : IDisposable
    {
        public byte** Pointer { get; }
        private readonly int Count;
        public Utf8Names(params string[] names)
        {
            Count = names.Length; Pointer = (byte**)NativeMemory.Alloc((nuint)(Count * sizeof(nint)));
            for (int i = 0; i < Count; i++) Pointer[i] = (byte*)Marshal.StringToCoTaskMemUTF8(names[i]);
        }
        public void Dispose() { for (int i = 0; i < Count; i++) Marshal.FreeCoTaskMem((nint)Pointer[i]); NativeMemory.Free(Pointer); }
    }
}
