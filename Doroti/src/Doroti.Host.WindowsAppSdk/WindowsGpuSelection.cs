using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace Doroti.Host.WindowsAppSdk;

internal enum WindowsGpuPreference : uint
{
    NoPreference = 0,
    LowPowerPreference = 1,
    HighPerformancePreference = 2,
}

internal static class WindowsGpuSelection
{
    internal static WindowsGpuPreference RequestedPreference =>
        ParsePreference(Environment.GetEnvironmentVariable("DOROTI_WINDOWS_GPU_PREFERENCE"));

    internal static WindowsGpuPreference ParsePreference(string? value) => value?.Trim().ToLowerInvariant() switch
    {
        null or "" or "nopreference" => WindowsGpuPreference.NoPreference,
        "lowpowerpreference" => WindowsGpuPreference.LowPowerPreference,
        "highperformancepreference" => WindowsGpuPreference.HighPerformancePreference,
        _ => throw new InvalidOperationException(
            $"Unsupported DOROTI_WINDOWS_GPU_PREFERENCE='{value}'. " +
            "Expected NoPreference, LowPowerPreference, or HighPerformancePreference."),
    };

    internal static ulong SelectAdapter(WindowsGpuPreference preference, ulong[]? eligibleLuids = null)
    {
        if (eligibleLuids is { Length: 0 })
            throw new ArgumentException("The eligible GPU list must not be empty.", nameof(eligibleLuids));
        var result = SelectNativeAdapter(preference, eligibleLuids,
            checked((uint)(eligibleLuids?.Length ?? 0)), out var luid);
        if (result < 0)
            throw new InvalidOperationException(
                $"Windows GPU selection failed for {preference} (HRESULT 0x{result:X8}).",
                Marshal.GetExceptionForHR(result));
        return luid;
    }

    internal static ulong ParseVulkanLuid(string value) =>
        BinaryPrimitives.ReadUInt64LittleEndian(Convert.FromHexString(value));

    internal static int[] AnglePlatformAttributes()
    {
        const int eglNone = 0x3038;
        // ANGLE D3D11, hardware only. NoPreference leaves ANGLE's default intact.
        int[] attributes = [0x3203, 0x3208, 0x3209, 0x320A];
        var preference = RequestedPreference;
        if (preference == WindowsGpuPreference.NoPreference)
            return [.. attributes, eglNone];
        var luid = SelectAdapter(preference);
        return [.. attributes,
            0x34A0, unchecked((int)(luid >> 32)), // EGL_PLATFORM_ANGLE_D3D_LUID_HIGH_ANGLE
            0x34A1, unchecked((int)luid),         // EGL_PLATFORM_ANGLE_D3D_LUID_LOW_ANGLE
            eglNone];
    }

    [DllImport("doroti_windows_appsdk_host_v1.dll",
        EntryPoint = "doroti_windows_gpu_select_adapter_v1", ExactSpelling = true,
        CallingConvention = CallingConvention.Cdecl)]
    private static extern int SelectNativeAdapter(WindowsGpuPreference preference,
        [In] ulong[]? eligibleLuids, uint eligibleCount, out ulong selectedLuid);
}
