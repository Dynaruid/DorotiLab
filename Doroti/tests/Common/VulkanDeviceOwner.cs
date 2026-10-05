using System.Runtime.InteropServices;
using Silk.NET.Vulkan;
using VkSemaphore = Silk.NET.Vulkan.Semaphore;

internal sealed unsafe class VulkanDeviceOwner : IDisposable
{
    private readonly Vk _vk = Vk.GetApi();
    internal Instance Instance;
    internal PhysicalDevice Physical;
    internal Device Device;
    internal Silk.NET.Vulkan.Queue Queue;
    internal uint Family;
    internal string Name = "";
    internal bool Software;
    private VkSemaphore _blocker;
    private ulong _value;
    private int _released = 1;
    private Timer? _watchdog;
    internal bool WatchdogFired;
    internal VulkanDeviceOwner()
    {
        var app = new ApplicationInfo { SType = StructureType.ApplicationInfo, ApiVersion = (1u << 22) | (2u << 12) };
        var create = new InstanceCreateInfo { SType = StructureType.InstanceCreateInfo, PApplicationInfo = &app };
        Check(_vk.CreateInstance(&create, null, out Instance));
        uint count = 0; Check(_vk.EnumeratePhysicalDevices(Instance, &count, null));
        var devices = new PhysicalDevice[count];
        fixed (PhysicalDevice* p = devices) Check(_vk.EnumeratePhysicalDevices(Instance, &count, p));
        foreach (var candidate in devices)
        {
            _vk.GetPhysicalDeviceProperties(candidate, out var properties);
            if (properties.ApiVersion < app.ApiVersion) continue;
            uint families = 0; _vk.GetPhysicalDeviceQueueFamilyProperties(candidate, &families, null);
            var queues = new QueueFamilyProperties[families];
            fixed (QueueFamilyProperties* p = queues) _vk.GetPhysicalDeviceQueueFamilyProperties(candidate, &families, p);
            var index = Array.FindIndex(queues, q => (q.QueueFlags & QueueFlags.GraphicsBit) != 0);
            if (index < 0) continue;
            Physical = candidate; Family = (uint)index;
            Name = Marshal.PtrToStringUTF8((nint)properties.DeviceName)!;
            Software = properties.DeviceType == PhysicalDeviceType.Cpu;
            if (!Software) break;
        }
        if (Physical.Handle == 0) throw new PlatformNotSupportedException("Vulkan 1.2 graphics queue is unavailable.");
        float priority = 1;
        var queue = new DeviceQueueCreateInfo { SType = StructureType.DeviceQueueCreateInfo, QueueFamilyIndex = Family, QueueCount = 1, PQueuePriorities = &priority };
        var timeline = new PhysicalDeviceTimelineSemaphoreFeatures { SType = StructureType.PhysicalDeviceTimelineSemaphoreFeatures, TimelineSemaphore = true };
        var device = new DeviceCreateInfo { SType = StructureType.DeviceCreateInfo, PNext = &timeline, QueueCreateInfoCount = 1, PQueueCreateInfos = &queue };
        Check(_vk.CreateDevice(Physical, &device, null, out Device));
        _vk.GetDeviceQueue(Device, Family, 0, out Queue);
        var type = new SemaphoreTypeCreateInfo { SType = StructureType.SemaphoreTypeCreateInfo, SemaphoreType = SemaphoreType.Timeline };
        var semaphore = new SemaphoreCreateInfo { SType = StructureType.SemaphoreCreateInfo, PNext = &type };
        Check(_vk.CreateSemaphore(Device, &semaphore, null, out _blocker));
    }
    internal void Hold()
    {
        WatchdogFired = false;
        _released = 0;
        var value = ++_value;
        var timeline = new TimelineSemaphoreSubmitInfo { SType = StructureType.TimelineSemaphoreSubmitInfo, WaitSemaphoreValueCount = 1, PWaitSemaphoreValues = &value };
        var semaphore = _blocker;
        var stage = PipelineStageFlags.AllCommandsBit;
        var submit = new SubmitInfo { SType = StructureType.SubmitInfo, PNext = &timeline, WaitSemaphoreCount = 1, PWaitSemaphores = &semaphore, PWaitDstStageMask = &stage };
        Check(_vk.QueueSubmit(Queue, 1, &submit, default));
        _watchdog = new Timer(_ => { WatchdogFired = true; Release(); }, null, 3000, Timeout.Infinite);
    }
    internal void Release()
    {
        if (Interlocked.Exchange(ref _released, 1) != 0) return;
        _watchdog?.Dispose();
        var signal = new SemaphoreSignalInfo { SType = StructureType.SemaphoreSignalInfo, Semaphore = _blocker, Value = _value };
        Check(_vk.SignalSemaphore(Device, &signal));
    }
    private static void Check(Result result) { if (result != Result.Success) throw new Exception("Vulkan regression: " + result); }
    public void Dispose()
    {
        Check(_vk.DeviceWaitIdle(Device));
        _vk.DestroySemaphore(Device, _blocker, null);
        _vk.DestroyDevice(Device, null); _vk.DestroyInstance(Instance, null); _vk.Dispose();
    }
}
