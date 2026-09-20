using System.Runtime.InteropServices;
using Doroti.Skia.Vulkan;
using SkiaSharp;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>Immutable shared allocation, retained across the raster/UI handoff.</summary>
internal sealed class WindowsSharedRaster(nint resource, nint handle, SKCanvas canvas) : IDisposable
{
    private nint _resource = resource;
    internal nint Handle { get; } = handle; // KMT handle, never CloseHandle.
    internal SKCanvas Canvas { get; } = canvas;
    public void Dispose() { var value = Interlocked.Exchange(ref _resource, 0); if (value != 0) Marshal.Release(value); }
}

internal sealed unsafe partial class WindowsManagedVulkanPresenter
{
    private nint _sharedRasterDevice;
    private readonly List<VulkanSharedRaster> _sharedRasters = [];
    internal (uint Low, int High) RasterAdapter => (_adapterLuidLow, _adapterLuidHigh);
    internal WindowsSharedRaster CreateSharedRaster(int width, int height)
    {
        if (_graphiteFrame is null) throw new InvalidOperationException("Shared raster requires an active Graphite frame.");
        if (_sharedRasterDevice == 0)
            Marshal.ThrowExceptionForHR(CreateSharedRasterDevice(_adapterLuidLow, _adapterLuidHigh, out _sharedRasterDevice));
        Marshal.ThrowExceptionForHR(CreateSharedRasterResource(_sharedRasterDevice, (uint)width, (uint)height, out var resource, out var handle));
        try
        {
            // Unlike NT handles, KMT imports do not retain the original D3D
            // allocation. Keep a separate producer reference even if a failed
            // UI/frame handoff disposes its own source before terminal GPU drain.
            Marshal.AddRef(resource);
            var output = new VulkanSharedRaster(_vk, _instance, _physicalDevice, _device, _queueFamily,
                _stockObserver!, _graphite!, handle, width, height, android: false,
                releaseSource: () => Marshal.Release(resource));
            _sharedRasters.Add(output);
            return new(resource, handle, output.Canvas);
        }
        catch { Marshal.Release(resource); throw; }
    }
    [DllImport(WindowsNativeV1.LibraryName, EntryPoint = "doroti_windows_shared_raster_device_v1", CallingConvention = CallingConvention.Cdecl)]
    private static extern int CreateSharedRasterDevice(uint low, int high, out nint device);
    [DllImport(WindowsNativeV1.LibraryName, EntryPoint = "doroti_windows_shared_raster_create_v1", CallingConvention = CallingConvention.Cdecl)]
    private static extern int CreateSharedRasterResource(nint device, uint width, uint height, out nint resource, out nint handle);
}
