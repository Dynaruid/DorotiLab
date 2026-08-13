using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DorotiSize = Doroti.Graphics.Size;

namespace Doroti.Host.Avalonia;

/// <summary>
/// Windows-only target diagnostic. It captures the visible DWM window bounds from the desktop so
/// evidence includes the native frame instead of only the Doroti render control.
/// </summary>
[SupportedOSPlatform("windows")]
internal static partial class WindowsWindowCapture
{
    private const uint DwmExtendedFrameBounds = 9;
    private const uint Srccopy = 0x00CC0020;
    private const uint CaptureBlt = 0x40000000;
    private const uint DibRgbColors = 0;
    private const uint BiRgb = 0;

    internal static AvaloniaWindowCapture Capture(Window window, double scaleFactor, string screenshotPath)
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException("Native window screenshots are currently available only on Windows target verification.");
        }
        var handle = window.TryGetPlatformHandle()?.Handle ?? 0;
        if (handle == 0)
        {
            throw new InvalidOperationException("The Avalonia window does not expose a native handle.");
        }

        if (DwmGetWindowAttribute(handle, DwmExtendedFrameBounds, out var windowRect, Marshal.SizeOf<NativeRect>()) != 0 &&
            !GetWindowRect(handle, out windowRect))
        {
            throw LastWin32("Could not read the native window bounds.");
        }
        if (!GetClientRect(handle, out var clientRect))
        {
            throw LastWin32("Could not read the native client bounds.");
        }
        var clientOrigin = new NativePoint();
        if (!ClientToScreen(handle, ref clientOrigin))
        {
            throw LastWin32("Could not map the native client bounds to the desktop.");
        }

        var windowBounds = ToBounds(windowRect);
        var clientBounds = new AvaloniaPixelBounds(
            clientOrigin.X,
            clientOrigin.Y,
            checked(clientRect.Right - clientRect.Left),
            checked(clientRect.Bottom - clientRect.Top));
        if (windowBounds.Width <= 0 || windowBounds.Height <= 0 || clientBounds.Width <= 0 || clientBounds.Height <= 0)
        {
            throw new InvalidOperationException("The native window or client bounds are empty.");
        }

        var pixels = CaptureDesktopPixels(windowBounds);
        var readback = new AvaloniaPixelReadback(new DorotiSize(windowBounds.Width, windowBounds.Height), checked(windowBounds.Width * 4), pixels);
        SavePng(readback, screenshotPath);
        return new(windowBounds, clientBounds, scaleFactor, readback);
    }

    private static byte[] CaptureDesktopPixels(AvaloniaPixelBounds bounds)
    {
        var screenDc = GetDC(0);
        if (screenDc == 0)
        {
            throw LastWin32("Could not acquire the desktop device context.");
        }
        nint memoryDc = 0;
        nint bitmap = 0;
        nint previous = 0;
        try
        {
            memoryDc = CreateCompatibleDC(screenDc);
            bitmap = CreateCompatibleBitmap(screenDc, bounds.Width, bounds.Height);
            if (memoryDc == 0 || bitmap == 0)
            {
                throw LastWin32("Could not allocate the window screenshot surface.");
            }
            previous = SelectObject(memoryDc, bitmap);
            if (previous == 0 || !BitBlt(memoryDc, 0, 0, bounds.Width, bounds.Height, screenDc, bounds.X, bounds.Y, Srccopy | CaptureBlt))
            {
                throw LastWin32("Could not copy the visible window pixels.");
            }
            _ = SelectObject(memoryDc, previous);
            previous = 0;

            var rowBytes = checked(bounds.Width * 4);
            var pixels = GC.AllocateUninitializedArray<byte>(checked(rowBytes * bounds.Height));
            var info = new BitmapInfo
            {
                Header = new BitmapInfoHeader
                {
                    Size = (uint)Marshal.SizeOf<BitmapInfoHeader>(),
                    Width = bounds.Width,
                    Height = -bounds.Height,
                    Planes = 1,
                    BitCount = 32,
                    Compression = BiRgb,
                    SizeImage = (uint)pixels.Length,
                },
            };
            var pinned = GCHandle.Alloc(pixels, GCHandleType.Pinned);
            try
            {
                if (GetDIBits(screenDc, bitmap, 0, (uint)bounds.Height, pinned.AddrOfPinnedObject(), ref info, DibRgbColors) != bounds.Height)
                {
                    throw LastWin32("Could not read the window screenshot pixels.");
                }
            }
            finally
            {
                pinned.Free();
            }
            for (var index = 3; index < pixels.Length; index += 4)
            {
                pixels[index] = 255;
            }
            return pixels;
        }
        finally
        {
            if (previous != 0 && memoryDc != 0)
            {
                _ = SelectObject(memoryDc, previous);
            }
            if (bitmap != 0)
            {
                _ = DeleteObject(bitmap);
            }
            if (memoryDc != 0)
            {
                _ = DeleteDC(memoryDc);
            }
            _ = ReleaseDC(0, screenDc);
        }
    }

    private static void SavePng(AvaloniaPixelReadback readback, string path)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        var pixelSize = new PixelSize((int)readback.PixelSize.Width, (int)readback.PixelSize.Height);
        using var bitmap = new WriteableBitmap(pixelSize, new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Opaque);
        using (var framebuffer = bitmap.Lock())
        {
            for (var row = 0; row < pixelSize.Height; row++)
            {
                Marshal.Copy(readback.Bgra8888Pixels, row * readback.RowBytes, framebuffer.Address + (row * framebuffer.RowBytes), readback.RowBytes);
            }
        }
        bitmap.Save(path, PngBitmapEncoderOptions.Default);
    }

    private static AvaloniaPixelBounds ToBounds(NativeRect rect) => new(
        rect.Left,
        rect.Top,
        checked(rect.Right - rect.Left),
        checked(rect.Bottom - rect.Top));

    private static Win32Exception LastWin32(string message) => new(Marshal.GetLastPInvokeError(), message);

    [StructLayout(LayoutKind.Sequential)]
    private struct NativePoint
    {
        internal int X;
        internal int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect
    {
        internal int Left;
        internal int Top;
        internal int Right;
        internal int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct BitmapInfoHeader
    {
        internal uint Size;
        internal int Width;
        internal int Height;
        internal ushort Planes;
        internal ushort BitCount;
        internal uint Compression;
        internal uint SizeImage;
        internal int XPixelsPerMeter;
        internal int YPixelsPerMeter;
        internal uint ColorsUsed;
        internal uint ColorsImportant;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct BitmapInfo
    {
        internal BitmapInfoHeader Header;
        internal uint Colors;
    }

    [LibraryImport("dwmapi.dll")]
    private static partial int DwmGetWindowAttribute(nint window, uint attribute, out NativeRect value, int valueSize);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetWindowRect(nint window, out NativeRect rect);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetClientRect(nint window, out NativeRect rect);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ClientToScreen(nint window, ref NativePoint point);

    [LibraryImport("user32.dll", SetLastError = true)]
    private static partial nint GetDC(nint window);

    [LibraryImport("user32.dll")]
    private static partial int ReleaseDC(nint window, nint deviceContext);

    [LibraryImport("gdi32.dll", SetLastError = true)]
    private static partial nint CreateCompatibleDC(nint deviceContext);

    [LibraryImport("gdi32.dll", SetLastError = true)]
    private static partial nint CreateCompatibleBitmap(nint deviceContext, int width, int height);

    [LibraryImport("gdi32.dll", SetLastError = true)]
    private static partial nint SelectObject(nint deviceContext, nint value);

    [LibraryImport("gdi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool BitBlt(nint destination, int x, int y, int width, int height, nint source, int sourceX, int sourceY, uint operation);

    [LibraryImport("gdi32.dll", SetLastError = true)]
    private static partial int GetDIBits(nint deviceContext, nint bitmap, uint startScan, uint scanLines, nint bits, ref BitmapInfo info, uint usage);

    [LibraryImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool DeleteObject(nint value);

    [LibraryImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool DeleteDC(nint deviceContext);
}
