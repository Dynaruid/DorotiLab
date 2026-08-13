// Temporary pre-A2 adaptation from Avalonia Win32 WGL sources; see migration/avalonia-shell/a1-source-port-provenance.json.
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Doroti.Vendor.Avalonia.Win32;

internal sealed class NativeOpenGlContext : IDisposable
{
    private const uint GlRenderer = 0x1F01;
    private const uint GlVersion = 0x1F02;
    private readonly nint _window;
    private readonly nint _deviceContext;
    private readonly nint _context;
    private readonly int _ownerThreadId;
    private bool _disposed;

    internal NativeOpenGlContext(nint window)
    {
        if (window == 0)
        {
            throw new InvalidOperationException("Cannot create an OpenGL context for a closed window.");
        }

        _ownerThreadId = Environment.CurrentManagedThreadId;
        _window = window;
        _deviceContext = NativeInterop.GetDc(window);
        if (_deviceContext == 0)
        {
            throw new Win32Exception(Marshal.GetLastPInvokeError(), "GetDC failed for the OpenGL window.");
        }

        try
        {
            ConfigurePixelFormat(_deviceContext);
            _context = NativeInterop.WglCreateContext(_deviceContext);
            if (_context == 0)
            {
                throw new Win32Exception(Marshal.GetLastPInvokeError(), "wglCreateContext failed.");
            }

            using (MakeCurrent())
            {
                Renderer = ReadGlString(GlRenderer);
                Version = ReadGlString(GlVersion);
            }
            IsHardwareAccelerated = !Renderer.Contains("GDI Generic", StringComparison.OrdinalIgnoreCase) &&
                                    !Renderer.Contains("Microsoft Basic Render", StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            if (_context != 0)
            {
                _ = NativeInterop.WglDeleteContext(_context);
            }
            _ = NativeInterop.ReleaseDc(_window, _deviceContext);
            throw;
        }
    }

    internal string Renderer { get; }

    internal string Version { get; }

    internal bool IsHardwareAccelerated { get; }

    internal IDisposable MakeCurrent()
    {
        ThrowIfInvalidThread();
        ObjectDisposedException.ThrowIf(_disposed, this);
        var previousDeviceContext = NativeInterop.WglGetCurrentDc();
        var previousContext = NativeInterop.WglGetCurrentContext();
        if (!NativeInterop.WglMakeCurrent(_deviceContext, _context))
        {
            throw new Win32Exception(Marshal.GetLastPInvokeError(), "wglMakeCurrent failed.");
        }
        return new RestoreContext(previousDeviceContext, previousContext);
    }

    internal void Present()
    {
        ThrowIfInvalidThread();
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!NativeInterop.SwapBuffers(_deviceContext))
        {
            throw new Win32Exception(Marshal.GetLastPInvokeError(), "SwapBuffers failed.");
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        ThrowIfInvalidThread();
        _disposed = true;
        Exception? failure = null;
        if (NativeInterop.WglGetCurrentContext() == _context)
        {
            if (!NativeInterop.WglMakeCurrent(0, 0))
            {
                failure = new Win32Exception(Marshal.GetLastPInvokeError(), "Clearing the current WGL context failed during disposal.");
            }
        }
        if (!NativeInterop.WglDeleteContext(_context) && failure is null)
        {
            failure = new Win32Exception(Marshal.GetLastPInvokeError(), "wglDeleteContext failed during disposal.");
        }
        // Doroti registers its window class with CS_OWNDC. ReleaseDC has no effect for
        // an owned DC and may return zero, so the HWND destruction owns that lifetime.
        _ = NativeInterop.ReleaseDc(_window, _deviceContext);
        if (failure is not null)
        {
            throw failure;
        }
    }

    private static void ConfigurePixelFormat(nint deviceContext)
    {
        if (NativeInterop.GetPixelFormat(deviceContext) != 0)
        {
            return;
        }

        var descriptor = NativeInterop.PixelFormatDescriptor.Create();
        var format = NativeInterop.ChoosePixelFormat(deviceContext, in descriptor);
        if (format == 0)
        {
            throw new Win32Exception(Marshal.GetLastPInvokeError(), "ChoosePixelFormat failed.");
        }
        if (!NativeInterop.SetPixelFormat(deviceContext, format, in descriptor))
        {
            throw new Win32Exception(Marshal.GetLastPInvokeError(), "SetPixelFormat failed.");
        }
    }

    private static string ReadGlString(uint name)
    {
        var value = NativeInterop.GlGetString(name);
        return value == 0 ? "unknown" : Marshal.PtrToStringAnsi(value) ?? "unknown";
    }

    private void ThrowIfInvalidThread()
    {
        if (Environment.CurrentManagedThreadId != _ownerThreadId)
        {
            throw new InvalidOperationException("The WGL context must only be used and disposed on its creator raster thread.");
        }
    }

    private sealed class RestoreContext(nint deviceContext, nint context) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            if (!NativeInterop.WglMakeCurrent(deviceContext, context))
            {
                // A view can close while an asynchronously submitted raster
                // frame is unwinding. In that case its captured previous HGLRC
                // may already have been deleted. Detach the current context so
                // the raster thread is left in a valid neutral state; report a
                // failure only when even that recovery is rejected by WGL.
                _ = NativeInterop.WglMakeCurrent(0, 0);
            }
        }
    }
}
