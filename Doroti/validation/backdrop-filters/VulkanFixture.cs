using Doroti.Skia.RuntimeEffects;
using Silk.NET.Vulkan;
using SkiaSharp;
using VkDevice = Silk.NET.Vulkan.Device;

sealed unsafe class VulkanFixture : IDisposable
{
    private readonly Vk vk = Vk.GetApi();
    private Instance instance;
    private VkDevice device;
    private GRVkExtensions? extensions;
    public GRContext? Context { get; }
    private readonly SKGraphiteContext? graphite;
    private readonly SKGraphiteRecorder? recorder;

    public VulkanFixture(bool useGraphite = false)
    {
        var app = new ApplicationInfo
        {
            SType = StructureType.ApplicationInfo,
            ApiVersion = Vk.Version11,
        };
        var create = new InstanceCreateInfo
        {
            SType = StructureType.InstanceCreateInfo,
            PApplicationInfo = &app,
        };
        Check(vk.CreateInstance(&create, null, out instance));
        uint count = 0;
        Check(vk.EnumeratePhysicalDevices(instance, &count, null));
        var devices = new PhysicalDevice[count];
        fixed (PhysicalDevice* ptr = devices)
            Check(vk.EnumeratePhysicalDevices(instance, &count, ptr));
        var physical = devices.First();
        vk.GetPhysicalDeviceProperties(physical, out var deviceProperties);
        Console.WriteLine(
            $"GPU: {System.Runtime.InteropServices.Marshal.PtrToStringUTF8((nint)deviceProperties.DeviceName)}; Graphite={useGraphite}"
        );
        uint families = 0;
        vk.GetPhysicalDeviceQueueFamilyProperties(physical, &families, null);
        var props = new QueueFamilyProperties[families];
        fixed (QueueFamilyProperties* ptr = props)
            vk.GetPhysicalDeviceQueueFamilyProperties(physical, &families, ptr);
        var family = (uint)
            Array.FindIndex(props, p => (p.QueueFlags & QueueFlags.GraphicsBit) != 0);
        var priority = 1f;
        var queueCreate = new DeviceQueueCreateInfo
        {
            SType = StructureType.DeviceQueueCreateInfo,
            QueueFamilyIndex = family,
            QueueCount = 1,
            PQueuePriorities = &priority,
        };
        var deviceCreate = new DeviceCreateInfo
        {
            SType = StructureType.DeviceCreateInfo,
            QueueCreateInfoCount = 1,
            PQueueCreateInfos = &queueCreate,
        };
        Check(vk.CreateDevice(physical, &deviceCreate, null, out device));
        vk.GetDeviceQueue(device, family, 0, out var queue);
        if (useGraphite)
        {
            using var backend = new SKGraphiteVkBackendContext
            {
                VkInstance = instance.Handle,
                VkPhysicalDevice = physical.Handle,
                VkDevice = device.Handle,
                VkQueue = queue.Handle,
                GraphicsQueueIndex = family,
                MaxApiVersion = Vk.Version11,
                GetProcedureAddress = (name, inst, dev) =>
                    GetProc(name, new Instance(inst), new VkDevice(dev)),
            };
            graphite =
                SKGraphiteContext.CreateVulkan(backend, new SKGraphiteContextOptions())
                ?? throw new Exception("Graphite Vulkan unavailable");
            recorder =
                graphite.CreateRecorder(64L * 1024 * 1024)
                ?? throw new Exception("Graphite recorder unavailable");
            return;
        }
        extensions = new GRVkExtensions();
        extensions.Initialize(GetProc, instance, physical, [], []);
        Context =
            GRContext.CreateVulkan(
                new GRSilkNetBackendContext
                {
                    VkInstance = instance,
                    VkPhysicalDevice = physical,
                    VkDevice = device,
                    VkQueue = queue,
                    GraphicsQueueIndex = family,
                    GetProcedureAddress = GetProc,
                    Extensions = extensions,
                    MaxAPIVersion = Vk.Version11,
                }
            ) ?? throw new Exception("Vulkan Skia context unavailable");
    }

    public SKSurface CreateSurface(SKImageInfo info) =>
        recorder is null
            ? SKSurface.Create(Context!, true, info)
            : SkiaGpuSurfaces.Register(SKSurface.Create(recorder, info), recorder);

    public (byte[] Expected, byte[] Actual) ReadGraphite(SKSurface expected, SKSurface actual)
    {
        using var recording = recorder!.Snap();
        if (graphite!.InsertRecording(recording) != SKGraphiteInsertStatus.Success)
            throw new Exception("Graphite insert failed");
        byte[]? e = null,
            a = null;
        var info = new SKImageInfo(64, 64, SKColorType.Rgba8888, SKAlphaType.Premul);
        graphite.RequestReadPixels(
            expected,
            info,
            new SKRectI(0, 0, 64, 64),
            SKImageRescaleGamma.Src,
            SKImageRescaleMode.Nearest,
            r => e = r?.ToArray(0)
        );
        graphite.RequestReadPixels(
            actual,
            info,
            new SKRectI(0, 0, 64, 64),
            SKImageRescaleGamma.Src,
            SKImageRescaleMode.Nearest,
            r => a = r?.ToArray(0)
        );
        if (!graphite.Submit(new SKGraphiteSubmitInfo { Sync = true }))
            throw new Exception("Graphite submit failed");
        graphite.CheckAsyncWorkCompletion();
        SkiaGpuSurfaces.CompleteRecording(recorder, false);
        return (
            e ?? throw new Exception("Expected readback missing"),
            a ?? throw new Exception("Actual readback missing")
        );
    }

    private nint GetProc(string name, Instance inst, VkDevice dev) =>
        dev.Handle != 0 ? vk.GetDeviceProcAddr(dev, name) : vk.GetInstanceProcAddr(inst, name);

    private static void Check(Result result)
    {
        if (result != Result.Success)
            throw new Exception($"Vulkan: {result}");
    }

    public void Dispose()
    {
        vk.DeviceWaitIdle(device);
        recorder?.Dispose();
        graphite?.Dispose();
        Context?.Dispose();
        extensions?.Dispose();
        vk.DestroyDevice(device, null);
        vk.DestroyInstance(instance, null);
        vk.Dispose();
    }
}
