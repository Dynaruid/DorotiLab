using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SkiaSharp;

namespace Doroti.Skia.Rendering;

/// <summary>
/// The exact descriptor used to create the host's Vulkan device. Feature pointers
/// (including pNext) are borrowed during CreateVulkan only. Dispatch and device
/// must remain live through session destruction. No supported-feature query is inferred.
/// </summary>
public sealed record SkiaGraphiteVulkanOptions(
    nint Instance, nint PhysicalDevice, nint Device, nint Queue,
    uint QueueFamily, uint MaxApiVersion,
    Func<string, nint, nint, nint> GetProcedure,
    IReadOnlyList<string> InstanceExtensions, IReadOnlyList<string> DeviceExtensions,
    nint EnabledFeatures = 0, nint EnabledFeatures2 = 0,
    Func<string, nint>? ResolveNativeSymbol = null);

// ABI 3 belongs to the SAME pinned libSkiaSharp used by SkiaSharp.dll. Hosts must
// qualify and deploy that pair. This binding does not replace a loaded module or
// install a diagnostic library into the user's NuGet cache.
internal sealed unsafe class SkiaGraphiteVulkanInterop : IDisposable
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate nint Procedure(nint userData, nint name, nint instance, nint device);
    [StructLayout(LayoutKind.Sequential)]
    private struct Init
    {
        public nint Instance, PhysicalDevice, Device, Queue;
        public uint QueueFamily, MaxApiVersion;
        public nint GetProc, UserData;
        public byte Protected;
    }
    private GCHandle _procedure;
    private readonly delegate* unmanaged[Cdecl]<nint, byte> _reportDeviceLost;
    private readonly delegate* unmanaged[Cdecl]<nint, int*, uint*, byte> _getState;
    private readonly delegate* unmanaged[Cdecl]<nint, int, uint, byte> _setState;
    private readonly delegate* unmanaged[Cdecl]<nint, nint, uint, ulong*, uint, ulong*, byte> _insert;
    internal SKGraphiteContext Context { get; }

    internal SkiaGraphiteVulkanInterop(SkiaGraphiteVulkanOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(options.GetProcedure);
        if (OperatingSystem.IsBrowser()) throw new PlatformNotSupportedException("Native Vulkan sessions are unavailable in the browser.");
        ArgumentNullException.ThrowIfNull(options.ResolveNativeSymbol);
        if (IntPtr.Size != 8 || sizeof(Init) != 64 || Marshal.OffsetOf<Init>(nameof(Init.GetProc)) != 40)
            throw new PlatformNotSupportedException("Graphite Vulkan ABI 3 requires a qualified 64-bit target.");
        if (options.EnabledFeatures != 0 && options.EnabledFeatures2 != 0)
            throw new ArgumentException("Pass enabled Features OR Features2, never both.", nameof(options));
        nint Symbol(string name) => options.ResolveNativeSymbol(name) is var symbol && symbol != 0
            ? symbol : throw new EntryPointNotFoundException($"Required Graphite native symbol is missing: {name}");
        var version = (delegate* unmanaged[Cdecl]<uint>)Symbol("doroti_graphite_interop_version");
        var create = (delegate* unmanaged[Cdecl]<Init*, SKGraphiteContextOptions*, nint, nint, uint, nint*, uint, nint*, nint>)
            Symbol("doroti_graphite_vk_context_create");
        var delete = (delegate* unmanaged[Cdecl]<nint, void>)Symbol("sk_graphite_context_delete");
        _reportDeviceLost = (delegate* unmanaged[Cdecl]<nint, byte>)Symbol("doroti_graphite_vk_context_report_device_lost");
        _getState = (delegate* unmanaged[Cdecl]<nint, int*, uint*, byte>)Symbol("doroti_graphite_vk_texture_get_state");
        _setState = (delegate* unmanaged[Cdecl]<nint, int, uint, byte>)Symbol("doroti_graphite_vk_texture_set_state");
        _insert = (delegate* unmanaged[Cdecl]<nint, nint, uint, ulong*, uint, ulong*, byte>)Symbol("doroti_graphite_vk_insert_recording");
        try
        {
            if (version() != 3)
                throw new NotSupportedException("Graphite Vulkan session requires the pinned ABI 3 native asset.");
        }
        catch (EntryPointNotFoundException exception)
        {
            throw new NotSupportedException("Loaded Skia asset has no Doroti Graphite Vulkan ABI 3. Qualify and deploy the managed/native pair.", exception);
        }
        if (!SKGraphiteContext.IsBackendAvailable(SKGraphiteBackend.Vulkan))
            throw new PlatformNotSupportedException("Loaded Skia asset has no Graphite Vulkan backend.");
        Procedure callback = (_, name, instance, device) =>
        {
            try { return options.GetProcedure(Marshal.PtrToStringUTF8(name)!, instance, device); }
            catch { return 0; }
        };
        _procedure = GCHandle.Alloc(callback);
        var allocations = new List<nint>();
        nint raw = 0;
        try
        {
            nint[] Names(IReadOnlyList<string> names) => names.Select(name =>
            {
                if (string.IsNullOrWhiteSpace(name) || name.Contains('\0'))
                    throw new ArgumentException("Vulkan extension names must be nonempty UTF-8 strings.");
                var pointer = Marshal.StringToCoTaskMemUTF8(name);
                allocations.Add(pointer);
                return pointer;
            }).ToArray();
            var instances = Names(options.InstanceExtensions);
            var devices = Names(options.DeviceExtensions);
            var init = new Init { Instance = options.Instance, PhysicalDevice = options.PhysicalDevice,
                Device = options.Device, Queue = options.Queue, QueueFamily = options.QueueFamily,
                MaxApiVersion = options.MaxApiVersion, GetProc = Marshal.GetFunctionPointerForDelegate(callback) };
            var contextOptions = new SKGraphiteContextOptions { GpuBudgetInBytes = SkiaGraphiteSession.ContextBudgetBytes };
            fixed (nint* instanceNames = instances)
            fixed (nint* deviceNames = devices)
                raw = create(&init, &contextOptions,
                    options.EnabledFeatures, options.EnabledFeatures2,
                    (uint)instances.Length, instanceNames, (uint)devices.Length, deviceNames);
            if (raw == 0) throw new InvalidOperationException("Graphite Vulkan context creation failed with the enabled device descriptor.");
            Context = WrapContext(raw, true);
            raw = 0;
        }
        catch
        {
            if (raw != 0) delete(raw);
            _procedure.Free();
            throw;
        }
        finally { foreach (var allocation in allocations) Marshal.FreeCoTaskMem(allocation); }
    }

    public void Dispose()
    {
        if (!_procedure.IsAllocated) return;
        Context.Dispose();
        _procedure.Free();
    }

    internal bool Insert(nint context, nint recording, ReadOnlySpan<ulong> waits, ReadOnlySpan<ulong> signals)
    {
        foreach (var handle in waits) if (handle == 0) throw new ArgumentException("Null wait semaphore.");
        foreach (var handle in signals) if (handle == 0) throw new ArgumentException("Null signal semaphore.");
        fixed (ulong* waitHandles = waits)
        fixed (ulong* signalHandles = signals)
            return _insert(context, recording,
                (uint)waits.Length, waitHandles, (uint)signals.Length, signalHandles) != 0;
    }

    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    private static extern SKGraphiteContext WrapContext(nint handle, bool owns);
    internal bool ReportDeviceLost(nint context) => _reportDeviceLost(context) != 0;
    internal bool SetState(nint texture, int layout, uint family) => _setState(texture, layout, family) != 0;
    internal bool GetState(nint texture, out int layout, out uint family)
    {
        int actualLayout = 0;
        uint actualFamily = 0;
        var result = _getState(texture, &actualLayout, &actualFamily) != 0;
        layout = actualLayout; family = actualFamily;
        return result;
    }
}
