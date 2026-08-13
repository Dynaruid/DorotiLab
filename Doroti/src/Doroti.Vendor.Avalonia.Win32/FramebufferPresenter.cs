// Temporary pre-A2 adaptation from Avalonia FramebufferManager; see migration/avalonia-shell/a1-source-port-provenance.json.
using System.ComponentModel;

namespace Doroti.Vendor.Avalonia.Win32;

internal static class FramebufferPresenter
{
    internal static unsafe void Present(nint window, ReadOnlySpan<byte> pixels, int width, int height, int rowBytes)
    {
        if (window == 0)
        {
            throw new ObjectDisposedException(nameof(window));
        }
        if (width <= 0 || height <= 0 || rowBytes < checked(width * (int)NativeInterop.BgraBytesPerPixel))
        {
            throw new ArgumentOutOfRangeException(nameof(rowBytes), "The BGRA8888 framebuffer dimensions are invalid.");
        }
        if (pixels.Length < checked(rowBytes * height))
        {
            throw new ArgumentException("The BGRA8888 framebuffer is shorter than its declared extent.", nameof(pixels));
        }

        var deviceContext = NativeInterop.GetDc(window);
        if (deviceContext == 0)
        {
            throw new Win32Exception("GetDC failed while presenting the software framebuffer.");
        }

        try
        {
            var header = new NativeInterop.BitmapInfoHeader
            {
                Size = (uint)sizeof(NativeInterop.BitmapInfoHeader),
                Width = width,
                Height = -height,
                Planes = 1,
                BitCount = 32,
                ImageSize = checked((uint)(rowBytes * height)),
            };
            fixed (byte* address = pixels)
            {
                var result = NativeInterop.StretchDIBits(
                    deviceContext,
                    0,
                    0,
                    width,
                    height,
                    0,
                    0,
                    width,
                    height,
                    (nint)address,
                    in header,
                    NativeInterop.DibRgbColors,
                    NativeInterop.RasterOperationSourceCopy);
                if (result == 0)
                {
                    throw new Win32Exception("StretchDIBits failed while presenting the software framebuffer.");
                }
            }
        }
        finally
        {
            _ = NativeInterop.ReleaseDc(window, deviceContext);
        }
    }
}
