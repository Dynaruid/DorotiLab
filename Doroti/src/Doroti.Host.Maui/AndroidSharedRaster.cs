#if ANDROID
using System.Runtime.InteropServices;
using Android.Graphics;
using Android.Hardware;
using Android.Runtime;
using SkiaSharp;

namespace Doroti.Host.Maui;

/// <summary>Immutable hardware bitmap. Vulkan's import and HWUI's bitmap each
/// retain the allocation independently; never recycle or write a published bitmap.</summary>
internal sealed class AndroidSharedRaster : IDisposable
{
    private HardwareBuffer? _buffer;
    internal SKCanvas Canvas { get; }
    internal AndroidSharedRaster(DorotiAndroidVulkanView owner, int width, int height)
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(29)) throw new PlatformNotSupportedException();
        _buffer = HardwareBuffer.Create(width, height, HardwareBufferFormat.Rgba8888, 1,
            HardwareBufferUsage.UsageGpuSampledImage | HardwareBufferUsage.UsageGpuColorOutput);
        try
        {
            var native = FromJava(JNIEnv.Handle, _buffer.Handle);
            if (native == 0) throw new InvalidOperationException("No native hardware buffer.");
            Canvas = owner.CreateHardwareBufferRaster(native, width, height).Canvas;
        }
        catch { Dispose(); throw; }
    }
    // Caller has already observed the Vulkan producer fence. Bitmap wrapping
    // does not copy pixels or wait for GPU work on its own.
    internal Bitmap CreateBitmap()
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(29)) throw new PlatformNotSupportedException();
        return Bitmap.WrapHardwareBuffer(_buffer ?? throw new ObjectDisposedException(nameof(AndroidSharedRaster)),
            ColorSpace.Get(ColorSpace.Named.Srgb!)) ?? throw new InvalidOperationException("Hardware bitmap wrapping failed.");
    }
    public void Dispose() { if (OperatingSystem.IsAndroidVersionAtLeast(26)) _buffer?.Close(); _buffer?.Dispose(); _buffer = null; }
    [DllImport("android", EntryPoint = "AHardwareBuffer_fromHardwareBuffer")]
    private static extern nint FromJava(nint environment, nint buffer);
}
#endif
