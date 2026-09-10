using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using SkiaSharp;
using Doroti.Skia.Rendering;
using VkDevice = Silk.NET.Vulkan.Device;

namespace Doroti.Validation.WindowsVulkanCapability;

internal static unsafe partial class Program
{
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate Result SessionQueueSubmit(nint queue, uint count, SubmitInfo* submissions, ulong fence);

    private static void ProbeSessionDeviceLoss(Vk vk, Instance instance, PhysicalDevice physicalDevice,
        VkDevice device, Queue queue, uint family, PhysicalDeviceFeatures2* enabled,
        IReadOnlyList<string> instanceExtensions, IReadOnlyList<string> deviceExtensions)
    {
        var armed = false;
        var realSubmit = Marshal.GetDelegateForFunctionPointer<SessionQueueSubmit>(vk.GetDeviceProcAddr(device, "vkQueueSubmit"));
        SessionQueueSubmit dispatch = (q, count, submissions, fence) => armed
            ? Result.ErrorDeviceLost : realSubmit(q, count, submissions, fence);
        using var session = SkiaGraphiteSession.CreateVulkan(new(instance.Handle, physicalDevice.Handle,
            device.Handle, queue.Handle, family, VulkanApiVersion11,
            (name, inst, dev) => name == "vkQueueSubmit" ? Marshal.GetFunctionPointerForDelegate(dispatch)
                : dev != 0 ? vk.GetDeviceProcAddr(new VkDevice(dev), name) : vk.GetInstanceProcAddr(new Instance(inst), name),
            instanceExtensions, deviceExtensions, EnabledFeatures2: (nint)enabled,
            ResolveNativeSymbol: name => NativeLibrary.GetExport(_graphiteNativeModule, name)), 1, maxFrames: 1);
        // Initialization can submit GPU work. Never simulate a dead device while
        // real initialization commands are outstanding.
        Check(vk.DeviceWaitIdle(device), "vkDeviceWaitIdle(session fault preflight)");
        vk.GetPhysicalDeviceMemoryProperties(physicalDevice, out var properties);
        var imageInfo = new ImageCreateInfo { SType = StructureType.ImageCreateInfo,
            ImageType = ImageType.Type2D, Format = Format.R8G8B8A8Unorm, Extent = new(64, 64, 1),
            MipLevels = 1, ArrayLayers = 1, Samples = SampleCountFlags.Count1Bit,
            Usage = ImageUsageFlags.ColorAttachmentBit | ImageUsageFlags.InputAttachmentBit |
                ImageUsageFlags.SampledBit | ImageUsageFlags.TransferSrcBit | ImageUsageFlags.TransferDstBit };
        Check(vk.CreateImage(device, &imageInfo, null, out var image), "vkCreateImage(session fault)");
        DeviceMemory memory = default;
        SkiaGraphiteSession.VulkanTarget? target = null;
        SkiaGraphiteSession.Frame? frame = null;
        var submissionAttempted = false;
        try
        {
            vk.GetImageMemoryRequirements(device, image, out var requirements);
            uint memoryType = 0;
            while (memoryType < properties.MemoryTypeCount && (requirements.MemoryTypeBits & (1u << (int)memoryType)) == 0) memoryType++;
            if (memoryType == properties.MemoryTypeCount) throw new NotSupportedException("No session fault image memory type.");
            var allocate = new MemoryAllocateInfo { SType = StructureType.MemoryAllocateInfo,
                AllocationSize = requirements.Size, MemoryTypeIndex = memoryType };
            Check(vk.AllocateMemory(device, &allocate, null, out memory), "vkAllocateMemory(session fault)");
            Check(vk.BindImageMemory(device, image, memory, 0), "vkBindImageMemory(session fault)");
            target = session.CreateVulkanTarget(64, 64, new SKGraphiteVkTextureInfo { SampleCount = 1,
                Format = (int)Format.R8G8B8A8Unorm, ImageUsageFlags = (uint)imageInfo.Usage,
                AspectMask = (uint)ImageAspectFlags.ColorBit }, (int)ImageLayout.Undefined, family, (nint)image.Handle, SKColorType.Rgba8888);
            frame = session.BeginVulkanFrame(target);
            frame.Surface.Canvas.Clear(SKColors.Red);
            armed = true;
            var rejected = false;
            try { submissionAttempted = true; frame.Submit(); }
            catch (InvalidOperationException exception) when (exception.Message == "Graphite Submit failed.") { rejected = true; }
            if (!rejected || !session.IsDeviceLost || session.CanBeginFrame)
                throw new InvalidOperationException("Shared session did not reject submission and close admission on simulated device loss.");
            session.NotifyVulkanDeviceLost(); // A duplicate host notification must be safe.
        }
        finally
        {
            Check(vk.DeviceWaitIdle(device), "vkDeviceWaitIdle(session fault teardown)");
            if (frame is not null)
            {
                if (submissionAttempted) frame.CompleteGpuWork();
                else frame.CancelRecording();
            }
            target?.Dispose();
            if (image.Handle != 0) vk.DestroyImage(device, image, null);
            if (memory.Handle != 0) vk.FreeMemory(device, memory, null);
            GC.KeepAlive(dispatch);
        }
    }

    // Diagnostic binding to the exact pinned SkiaSharp constructor. Do not use
    // reflection to mutate handles or pass a context between two Skia libraries.
    // A product package must ship/version its managed and native bindings together.
    private sealed class ExtendedGraphiteContext : IDisposable
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate nint GetProcedure(nint userData, nint name, nint instance, nint device);
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate Result QueueSubmit(nint queue, uint count, SubmitInfo* submissions, ulong fence);

        [StructLayout(LayoutKind.Sequential)]
        private struct BackendInit
        {
            public nint Instance, PhysicalDevice, Device, Queue;
            public uint QueueFamily, MaxApiVersion;
            public nint GetProc, UserData;
            public byte Protected;
        }

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        private static extern SKGraphiteContext WrapContext(nint handle, bool owns);

        [DllImport("libSkiaSharp", CallingConvention = CallingConvention.Cdecl)]
        private static extern nint doroti_graphite_vk_context_create(BackendInit* init,
            SKGraphiteContextOptions* options, PhysicalDeviceFeatures* features,
            PhysicalDeviceFeatures2* features2, uint instanceCount, byte** instanceExtensions,
            uint deviceCount, byte** deviceExtensions);

        [DllImport("libSkiaSharp", CallingConvention = CallingConvention.Cdecl)]
        private static extern void sk_graphite_context_delete(nint context);

        private GCHandle _procedure;
        private readonly QueueSubmit? _lostSubmit;
        private bool _deviceLossArmed;
        public SKGraphiteContext Context { get; }
        private bool _disposed;

        public ExtendedGraphiteContext(Vk vk, Instance instance, PhysicalDevice physicalDevice,
            VkDevice device, Queue queue, uint family, PhysicalDeviceFeatures2* features,
            IReadOnlyList<string> instanceExtensions, IReadOnlyList<string> deviceExtensions, bool injectDeviceLoss = false)
        {
            if (GraphiteInterop.doroti_graphite_interop_version() is not (2 or 3))
                throw new NotSupportedException("Enabled features/extensions require Graphite interop ABI 2.");
            if (sizeof(BackendInit) != 64 || Marshal.OffsetOf<BackendInit>(nameof(BackendInit.GetProc)) != 40)
                throw new PlatformNotSupportedException("Pinned Graphite Vulkan init requires the x64 ABI.");
            if (injectDeviceLoss)
            {
                var originalSubmit = Marshal.GetDelegateForFunctionPointer<QueueSubmit>(vk.GetDeviceProcAddr(device, "vkQueueSubmit"));
                _lostSubmit = (q, count, submissions, fence) => _deviceLossArmed
                    ? Result.ErrorDeviceLost : originalSubmit(q, count, submissions, fence);
            }
            GetProcedure getProc = (_, name, inst, dev) =>
            {
                try
                {
                    // Permit context initialization, then inject only after its
                    // GPU work is drained. This is not physical device reset.
                    if (_lostSubmit is not null && Marshal.PtrToStringUTF8(name) == "vkQueueSubmit")
                        return Marshal.GetFunctionPointerForDelegate(_lostSubmit);
                    return dev != 0 ? vk.GetDeviceProcAddr(new VkDevice(dev), (byte*)name)
                        : vk.GetInstanceProcAddr(new Instance(inst), (byte*)name);
                }
                catch { return 0; } // Never unwind a managed exception through native code.
            };
            _procedure = GCHandle.Alloc(getProc);
            byte** instanceNames = null;
            byte** deviceNames = null;
            nint raw = 0;
            try
            {
                instanceNames = (byte**)SilkMarshal.StringArrayToPtr(instanceExtensions);
                deviceNames = (byte**)SilkMarshal.StringArrayToPtr(deviceExtensions);
                var init = new BackendInit { Instance = instance.Handle, PhysicalDevice = physicalDevice.Handle,
                    Device = device.Handle, Queue = queue.Handle, QueueFamily = family, MaxApiVersion = VulkanApiVersion11,
                    GetProc = Marshal.GetFunctionPointerForDelegate(getProc) };
                var options = new SKGraphiteContextOptions { GpuBudgetInBytes = 256L * 1024 * 1024 };
                raw = doroti_graphite_vk_context_create(&init, &options, null, features,
                    (uint)instanceExtensions.Count, instanceNames, (uint)deviceExtensions.Count, deviceNames);
                if (raw == 0) throw new InvalidOperationException("Extended Graphite Vulkan context creation failed.");
                Context = WrapContext(raw, true);
                raw = 0; // Ownership has transferred to the one managed context wrapper.
            }
            catch
            {
                if (raw != 0) sk_graphite_context_delete(raw);
                _procedure.Free();
                throw;
            }
            finally
            {
                FreeStringArray(instanceNames, instanceExtensions.Count);
                FreeStringArray(deviceNames, deviceExtensions.Count);
            }
        }

        public void ArmDeviceLoss()
        {
            if (_lostSubmit is null) throw new InvalidOperationException("Device-loss injection was not configured.");
            _deviceLossArmed = true;
        }

        public void Dispose()
        {
            if (_disposed) return;
            Context.Dispose(); // Native destructor can still call the procedure callback.
            _procedure.Free();
            _disposed = true;
        }
    }
}
