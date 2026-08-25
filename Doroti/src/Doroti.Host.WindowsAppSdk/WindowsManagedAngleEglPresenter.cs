using System.Runtime.InteropServices;
using Doroti.Skia.RuntimeEffects;
using SkiaSharp;

namespace Doroti.Host.WindowsAppSdk;

internal sealed class WindowsManagedAngleEglPresenter : WindowsManagedHwndPresenterBase
{
    private const string AngleLibrary = "av_libglesv2.dll";
    private const int EglFalse = 0;
    private const int EglNone = 0x3038;
    private const uint EglPlatformAngle = 0x3202;
    private const int EglPlatformAngleType = 0x3203;
    private const int EglPlatformAngleTypeD3D11 = 0x3208;
    private const int EglPlatformAngleDeviceType = 0x3209;
    private const int EglPlatformAngleDeviceTypeHardware = 0x320A;
    private const int EglRedSize = 0x3024;
    private const int EglGreenSize = 0x3023;
    private const int EglBlueSize = 0x3022;
    private const int EglAlphaSize = 0x3021;
    private const int EglDepthSize = 0x3025;
    private const int EglStencilSize = 0x3026;
    private const int EglSurfaceType = 0x3033;
    private const int EglWindowBit = 0x0004;
    private const int EglRenderableType = 0x3040;
    private const int EglOpenGles2Bit = 0x0004;
    private const int EglContextClientVersion = 0x3098;
    private const int EglOpenGlesApi = 0x30A0;
    private const int EglWidth = 0x3057;
    private const int EglHeight = 0x3056;
    private const int EglFixedSizeAngle = 0x3201;
    private const int EglBadParameter = 0x300C;
    private const int EglBadMatch = 0x3009;
    private const uint GlNoError = 0;
    private const uint GlRenderer = 0x1F01;
    private const uint GlSamples = 0x80A9;
    private const uint GlStencilBits = 0x0D57;
    private const uint GlRgba8 = 0x8058;

    private readonly bool _diagnosticsEnabled;
    private nint _display;
    private nint _config;
    private nint _eglContext;
    private nint _eglSurface;
    private nint _window;
    private GRGlInterface? _glInterface;
    private GRContext? _context;
    private GRBackendRenderTarget? _windowTarget;
    private SKSurface? _windowSurface;
    private bool _flushAfterResizePresent;
    private bool _debugBaselineSealed;
    private bool _disposed;

    internal WindowsManagedAngleEglPresenter(bool enableDiagnostics)
    {
        _diagnosticsEnabled = enableDiagnostics;
    }

    internal override string BackendName => "ANGLE/EGL-D3D11";
    internal override string RuntimeEffectsBackend => DorotiSkiaRuntimeEffects.WindowsAngleEglBackend;
    internal override string DiagnosticCoverage =>
        "direct exact default-framebuffer raster, swap interval 0, resize DwmFlush, " +
        "explicit EGL return codes, and GLES glGetError";
    internal override int Width { get; set; }
    internal override int Height { get; set; }
    internal override ulong DeviceGeneration { get; set; }
    internal override ulong ResizeBuffersCount { get; set; }
    internal override ulong ResizeInvalidCallCount { get; set; }
    internal override ulong PresentCount { get; set; }
    internal override ulong GpuSubmitCount { get; set; }
    internal override ulong GpuCopyCount { get; set; }
    internal override ulong InitializationDebugMessageCount { get; set; }
    internal override ulong InitializationDebugErrorCount { get; set; }
    internal override ulong OperationalDebugMessageCount { get; set; }
    internal override ulong OperationalDebugErrorCount { get; set; }
    internal override ulong OperationalDebugWarningCount { get; set; }
    internal override string AdapterDescription { get; set; } = "uninitialized";

    internal override void EnsureTarget(nint childWindow, int width, int height)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (childWindow == 0) throw new ArgumentOutOfRangeException(nameof(childWindow));
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

        if (_window != 0 && _window != childWindow)
            ReleaseDevice();
        EnsureDevice();
        _window = childWindow;
        if (_eglSurface != 0 && Width == width && Height == height)
        {
            MakeCurrent();
            return;
        }

        var resized = _eglSurface != 0;
        ReleaseWindowSurface();
        var surfaceAttributes = new[]
        {
            EglFixedSizeAngle, 1,
            EglWidth, width,
            EglHeight, height,
            EglNone,
        };
        _eglSurface = EglCreateWindowSurface(_display, _config, childWindow, surfaceAttributes);
        if (_eglSurface == 0) ThrowEgl("eglCreateWindowSurface(EGL_FIXED_SIZE_ANGLE)");
        Width = width;
        Height = height;
        MakeCurrent();
        ApplyRequestedSwapInterval();

        _context!.ResetContext(GRGlBackendState.All);
        GlGetIntegerv(GlSamples, out var sampleCount);
        GlGetIntegerv(GlStencilBits, out var stencilBits);
        ThrowIfGlErrors("default-framebuffer query");
        _windowTarget = new GRBackendRenderTarget(
            width, height, Math.Max(0, sampleCount), Math.Max(0, stencilBits),
            new GRGlFramebufferInfo(0, GlRgba8));
        _windowSurface = SKSurface.Create(
            _context, _windowTarget, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888)
            ?? throw new InvalidOperationException("Skia could not wrap the ANGLE default framebuffer.");
        if (resized)
        {
            ResizeBuffersCount++;
            _flushAfterResizePresent = true;
        }
    }

    internal override void SealInitializationDebugBaseline()
    {
        if (_debugBaselineSealed) return;
        CaptureGlErrors("initialization", initialization: true);
        _debugBaselineSealed = true;
    }

    internal override void CaptureOperationalDebugMessages()
    {
        if (!_debugBaselineSealed || _eglSurface == 0) return;
        MakeCurrent();
        CaptureGlErrors("operation", initialization: false);
    }

    internal override T RenderAndPresent<T>(Func<SKSurface, T> paint, Predicate<T> shouldPresent)
    {
        ArgumentNullException.ThrowIfNull(paint);
        ArgumentNullException.ThrowIfNull(shouldPresent);
        ObjectDisposedException.ThrowIf(_disposed, this);
        MakeCurrent();
        var context = _context ?? throw new InvalidOperationException("The managed ANGLE Skia context is unavailable.");
        var windowSurface = _windowSurface ?? throw new InvalidOperationException("The ANGLE window surface is unavailable.");
        var result = paint(windowSurface);
        if (!shouldPresent(result)) return result;

        windowSurface.Canvas.Flush();
        context.Flush(windowSurface);
        context.Submit(false);
        GpuSubmitCount++;
        if (!shouldPresent(result)) return result;
        ThrowIfGlErrors("managed Skia direct submit");
        if (!shouldPresent(result)) return result;
        if (EglSwapBuffers(_display, _eglSurface) == EglFalse)
            ThrowEgl("eglSwapBuffers");
        PresentCount++;
        if (_flushAfterResizePresent ||
            Environment.GetEnvironmentVariable("DOROTI_WINDOWS_DWM_FLUSH") == "1")
        {
            Marshal.ThrowExceptionForHR(DwmFlush());
            _flushAfterResizePresent = false;
        }
        return result;
    }

    internal override void ResetDevice()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ReleaseDevice();
    }

    private void EnsureDevice()
    {
        if (_display != 0) return;
        var platformAttributes = new[]
        {
            EglPlatformAngleType, EglPlatformAngleTypeD3D11,
            EglPlatformAngleDeviceType, EglPlatformAngleDeviceTypeHardware,
            EglNone,
        };
        _display = EglGetPlatformDisplayExt(EglPlatformAngle, 0, platformAttributes);
        if (_display == 0) ThrowEgl("eglGetPlatformDisplayEXT(D3D11 hardware)");
        if (EglInitialize(_display, out _, out _) == EglFalse) ThrowEgl("eglInitialize");
        if (EglBindApi(EglOpenGlesApi) == EglFalse) ThrowEgl("eglBindAPI(EGL_OPENGL_ES_API)");
        var configAttributes = new[]
        {
            EglSurfaceType, EglWindowBit,
            EglRenderableType, EglOpenGles2Bit,
            EglRedSize, 8,
            EglGreenSize, 8,
            EglBlueSize, 8,
            EglAlphaSize, 8,
            EglDepthSize, 0,
            EglStencilSize, 8,
            EglNone,
        };
        if (EglChooseConfig(_display, configAttributes, out _config, 1, out var configCount) == EglFalse ||
            configCount <= 0 || _config == 0)
            ThrowEgl("eglChooseConfig(window RGBA8/stencil8/GLES2)");
        var contextAttributes = new[] { EglContextClientVersion, 2, EglNone };
        _eglContext = EglCreateContext(_display, _config, 0, contextAttributes);
        if (_eglContext == 0) ThrowEgl("eglCreateContext(GLES2)");

        _debugBaselineSealed = false;
    }

    private void CompleteContextInitialization()
    {
        if (_context is not null) return;
        // Resolve and validate GLES only after EGL has made the HWND context current.
        _glInterface = GRGlInterface.CreateGles(EglGetProcAddress)
            ?? throw new InvalidOperationException("Skia could not resolve the ANGLE GLES interface.");
        _context = GRContext.CreateGl(_glInterface!)
            ?? throw new InvalidOperationException("Skia could not create a managed ANGLE GLES context.");
        var renderer = GlGetString(GlRenderer);
        AdapterDescription = renderer == 0
            ? "ANGLE renderer unavailable"
            : Marshal.PtrToStringAnsi(renderer) ?? "ANGLE renderer unavailable";
        var isAngle = AdapterDescription.Contains("ANGLE", StringComparison.OrdinalIgnoreCase);
        var isD3D11 = AdapterDescription.Contains("D3D11", StringComparison.OrdinalIgnoreCase) ||
                      AdapterDescription.Contains("Direct3D11", StringComparison.OrdinalIgnoreCase);
        var isSoftware = AdapterDescription.Contains("SwiftShader", StringComparison.OrdinalIgnoreCase) ||
                         AdapterDescription.Contains("WARP", StringComparison.OrdinalIgnoreCase) ||
                         AdapterDescription.Contains("Reference", StringComparison.OrdinalIgnoreCase) ||
                         AdapterDescription.Contains("llvmpipe", StringComparison.OrdinalIgnoreCase) ||
                         AdapterDescription.Contains("softpipe", StringComparison.OrdinalIgnoreCase);
        if (!isAngle || !isD3D11 || isSoftware)
            throw new InvalidOperationException(
                $"ANGLE did not select a hardware D3D11 renderer: '{AdapterDescription}'.");
        DeviceGeneration++;
    }

    private void ApplyRequestedSwapInterval()
    {
        var requested = Environment.GetEnvironmentVariable("DOROTI_WINDOWS_EGL_SWAP_INTERVAL") ?? "0";
        if (requested is not ("0" or "1")) return;
        if (EglSwapInterval(_display, requested == "0" ? 0 : 1) == EglFalse)
            ThrowEgl($"eglSwapInterval({requested})");
    }

    private void MakeCurrent()
    {
        if (_display == 0 || _eglSurface == 0 || _eglContext == 0)
            throw new InvalidOperationException("The managed ANGLE EGL target is incomplete.");
        if (EglMakeCurrent(_display, _eglSurface, _eglSurface, _eglContext) == EglFalse)
            ThrowEgl("eglMakeCurrent");
        CompleteContextInitialization();
    }

    private void ThrowIfGlErrors(string operation)
    {
        if (CaptureGlErrors(operation, initialization: !_debugBaselineSealed) != 0)
            throw new InvalidOperationException($"{operation} emitted one or more GLES errors.");
    }

    private ulong CaptureGlErrors(string stage, bool initialization)
    {
        ulong count = 0;
        for (var index = 0; index < 16; index++)
        {
            var error = GlGetError();
            if (error == GlNoError) break;
            count++;
            Console.Error.WriteLine($"GLES {stage} error=0x{error:x4}");
        }
        if (!_diagnosticsEnabled && count == 0) return 0;
        if (initialization)
        {
            InitializationDebugMessageCount += count;
            InitializationDebugErrorCount += count;
        }
        else
        {
            OperationalDebugMessageCount += count;
            OperationalDebugErrorCount += count;
        }
        return count;
    }

    private void ThrowEgl(string operation)
    {
        var error = EglGetError();
        if (error is EglBadParameter or EglBadMatch) ResizeInvalidCallCount++;
        if (_debugBaselineSealed)
        {
            OperationalDebugMessageCount++;
            OperationalDebugErrorCount++;
        }
        else
        {
            InitializationDebugMessageCount++;
            InitializationDebugErrorCount++;
        }
        throw new InvalidOperationException($"{operation} failed with EGL error 0x{error:x4}.");
    }

    private void ReleaseWindowSurface()
    {
        if (_display != 0 && _eglSurface != 0 && _eglContext != 0)
            EglMakeCurrent(_display, _eglSurface, _eglSurface, _eglContext);
        _windowSurface?.Dispose();
        _windowSurface = null;
        _windowTarget?.Dispose();
        _windowTarget = null;
        if (_display != 0 && _eglSurface != 0)
        {
            EglMakeCurrent(_display, 0, 0, 0);
            EglDestroySurface(_display, _eglSurface);
        }
        _eglSurface = 0;
        Width = Height = 0;
    }

    private void ReleaseDevice()
    {
        if (_display == 0) return;
        if (_eglSurface != 0 && _eglContext != 0)
            EglMakeCurrent(_display, _eglSurface, _eglSurface, _eglContext);
        _windowSurface?.Dispose();
        _windowSurface = null;
        _windowTarget?.Dispose();
        _windowTarget = null;
        _context?.AbandonContext(false);
        _context?.Dispose();
        _context = null;
        _glInterface?.Dispose();
        _glInterface = null;
        EglMakeCurrent(_display, 0, 0, 0);
        if (_eglSurface != 0) EglDestroySurface(_display, _eglSurface);
        if (_eglContext != 0) EglDestroyContext(_display, _eglContext);
        EglTerminate(_display);
        _display = _config = _eglContext = _eglSurface = _window = 0;
        _flushAfterResizePresent = false;
        Width = Height = 0;
        AdapterDescription = "uninitialized";
    }

    public override void Dispose()
    {
        if (_disposed) return;
        ReleaseDevice();
        _disposed = true;
    }

    [DllImport(AngleLibrary, EntryPoint = "EGL_GetPlatformDisplayEXT", ExactSpelling = true)]
    private static extern nint EglGetPlatformDisplayExt(uint platform, nint nativeDisplay, int[] attributes);

    [DllImport(AngleLibrary, EntryPoint = "EGL_Initialize", ExactSpelling = true)]
    private static extern int EglInitialize(nint display, out int major, out int minor);

    [DllImport(AngleLibrary, EntryPoint = "EGL_BindAPI", ExactSpelling = true)]
    private static extern int EglBindApi(int api);

    [DllImport(AngleLibrary, EntryPoint = "EGL_ChooseConfig", ExactSpelling = true)]
    private static extern int EglChooseConfig(nint display, int[] attributes, out nint config, int configSize, out int configCount);

    [DllImport(AngleLibrary, EntryPoint = "EGL_CreateContext", ExactSpelling = true)]
    private static extern nint EglCreateContext(nint display, nint config, nint sharedContext, int[] attributes);

    [DllImport(AngleLibrary, EntryPoint = "EGL_CreateWindowSurface", ExactSpelling = true)]
    private static extern nint EglCreateWindowSurface(nint display, nint config, nint nativeWindow, int[] attributes);

    [DllImport(AngleLibrary, EntryPoint = "EGL_MakeCurrent", ExactSpelling = true)]
    private static extern int EglMakeCurrent(nint display, nint drawSurface, nint readSurface, nint context);

    [DllImport(AngleLibrary, EntryPoint = "EGL_SwapInterval", ExactSpelling = true)]
    private static extern int EglSwapInterval(nint display, int interval);

    [DllImport(AngleLibrary, EntryPoint = "EGL_SwapBuffers", ExactSpelling = true)]
    private static extern int EglSwapBuffers(nint display, nint surface);

    [DllImport(AngleLibrary, EntryPoint = "EGL_DestroySurface", ExactSpelling = true)]
    private static extern int EglDestroySurface(nint display, nint surface);

    [DllImport(AngleLibrary, EntryPoint = "EGL_DestroyContext", ExactSpelling = true)]
    private static extern int EglDestroyContext(nint display, nint context);

    [DllImport(AngleLibrary, EntryPoint = "EGL_Terminate", ExactSpelling = true)]
    private static extern int EglTerminate(nint display);

    [DllImport(AngleLibrary, EntryPoint = "EGL_GetError", ExactSpelling = true)]
    private static extern int EglGetError();

    [DllImport(AngleLibrary, EntryPoint = "EGL_GetProcAddress", ExactSpelling = true, CharSet = CharSet.Ansi)]
    private static extern nint EglGetProcAddress(string name);

    [DllImport(AngleLibrary, EntryPoint = "glGetError", ExactSpelling = true)]
    private static extern uint GlGetError();

    [DllImport(AngleLibrary, EntryPoint = "glGetIntegerv", ExactSpelling = true)]
    private static extern void GlGetIntegerv(uint name, out int value);

    [DllImport(AngleLibrary, EntryPoint = "glGetString", ExactSpelling = true)]
    private static extern nint GlGetString(uint name);

    [DllImport("dwmapi.dll", ExactSpelling = true)]
    private static extern int DwmFlush();
}
