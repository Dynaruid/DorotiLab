using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using SkiaSharp;
using VkDevice = Silk.NET.Vulkan.Device;

namespace Doroti.Validation.WindowsVulkanCapability;

internal static unsafe partial class Program
{
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
            if (GraphiteInterop.doroti_graphite_interop_version() != 2)
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
