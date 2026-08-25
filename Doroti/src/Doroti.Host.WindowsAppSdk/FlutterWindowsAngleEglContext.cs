using System.Runtime.InteropServices;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using SkiaSharp;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>
/// Raster-thread-only ANGLE/EGL bootstrap for the Flutter-style Windows path.
/// F1 deliberately creates only a 1x1 pbuffer; F4 owns child-HWND window
/// surfaces and their exact-size recreate/present lifecycle.
/// </summary>
internal sealed class FlutterWindowsAngleEglContext : IDisposable
{
    internal const string AnglePackageId = "Avalonia.Angle.Windows.Natives";
    internal const string AnglePackageVersion = "2.1.27548.20260419";
    internal const string SkiaPackageId = "SkiaSharp.NativeAssets.Win32";
    internal const string SkiaPackageVersion = "4.151.1";

    // F4 uses the same explicitly loaded module through the assembly-level
    // resolver below.  Keeping this identifier shared prevents a window
    // surface from silently falling back to a PATH-resolved ANGLE copy.
    internal const string AngleFileName = "av_libglesv2.dll";
    private const string SkiaFileName = "libSkiaSharp.dll";
    private const string ExpectedAngleSha256 =
        "53191a77fe783cd757ca7767077c2a64a662e7043777a5b4ab74980d4a0b73e3";
    private const string ExpectedSkiaSha256 =
        "0d98e29c028b3315d0e0344d02cd7aa4080abdf17fa67086847da13435320f2a";

    private const int EglFalse = 0;
    private const int EglNone = 0x3038;
    private const int EglRedSize = 0x3024;
    private const int EglGreenSize = 0x3023;
    private const int EglBlueSize = 0x3022;
    private const int EglAlphaSize = 0x3021;
    private const int EglDepthSize = 0x3025;
    private const int EglStencilSize = 0x3026;
    private const int EglSurfaceType = 0x3033;
    private const int EglPbufferBit = 0x0001;
    private const int EglRenderableType = 0x3040;
    private const int EglOpenGles2Bit = 0x0004;
    private const int EglContextClientVersion = 0x3098;
    private const int EglOpenGlesApi = 0x30A0;
    private const int EglWidth = 0x3057;
    private const int EglHeight = 0x3056;
    private const uint GlSamples = 0x80A9;
    private const uint GlStencilBits = 0x0D57;
    private const uint GlRenderer = 0x1F01;
    private const uint GlRgba8 = 0x8058;

    private static readonly object NativeLibraryGate = new();
    private static nint _angleLibrary;
    private static nint _skiaLibrary;
    private static FlutterWindowsAngleNativeProvenance? _nativeProvenance;
    private static int _teardownFailureCount;

    private readonly int _rasterManagedThreadId;
    private readonly uint _rasterNativeThreadId;
    private nint _display;
    private nint _config;
    private nint _eglContext;
    private nint _pbufferSurface;
    private GRGlInterface? _glInterface;
    private GRContext? _grContext;
    private GRBackendRenderTarget? _renderTarget;
    private SKSurface? _surface;
    private bool _disposed;

    static FlutterWindowsAngleEglContext()
    {
        NativeLibrary.SetDllImportResolver(
            typeof(FlutterWindowsAngleEglContext).Assembly,
            ResolveBundledAngleLibrary);
    }

    private FlutterWindowsAngleEglContext(
        int rasterManagedThreadId,
        uint rasterNativeThreadId)
    {
        _rasterManagedThreadId = rasterManagedThreadId;
        _rasterNativeThreadId = rasterNativeThreadId;
    }

    internal FlutterWindowsAngleNativeProvenance NativeProvenance =>
        _nativeProvenance ?? throw new InvalidOperationException(
            "The bundled ANGLE native artifacts have not been loaded.");

    internal string Renderer { get; private set; } = "uninitialized";

    internal static int TeardownFailureCount => Volatile.Read(ref _teardownFailureCount);

    /// <summary>
    /// Makes the F1-proven, app-directory ANGLE/Skia graph available to the
    /// later F4 child-HWND surface owner without constructing F1's pbuffer.
    /// The caller still owns all EGL display/context/surface lifetime on its
    /// raster thread.
    /// </summary>
    internal static FlutterWindowsAngleNativeProvenance EnsureNativeArtifactsForWindowSurface()
    {
        if (!OperatingSystem.IsWindows())
            throw new PlatformNotSupportedException(
                "The Flutter ANGLE/EGL window surface can only run on Windows.");
        if (RuntimeInformation.ProcessArchitecture != Architecture.X64)
            throw new PlatformNotSupportedException(
                "The F4 Flutter ANGLE/EGL window surface is pinned to win-x64.");

        EnsureBundledNativeArtifacts();
        return _nativeProvenance ?? throw new InvalidOperationException(
            "The bundled ANGLE native artifacts were not available after loading.");
    }

    /// <summary>
    /// Creates the ANGLE display/config/context on the current raster thread,
    /// verifies explicit app-directory native artifacts, and prepares a 1x1
    /// pbuffer. It does not create a window surface.
    /// </summary>
    internal static FlutterWindowsAngleEglContext CreateOffscreenOnCurrentThread()
    {
        if (!OperatingSystem.IsWindows())
            throw new PlatformNotSupportedException(
                "The Flutter ANGLE/EGL bootstrap can only run on Windows.");
        if (RuntimeInformation.ProcessArchitecture != Architecture.X64)
            throw new PlatformNotSupportedException(
                "The F1 Flutter ANGLE/EGL bootstrap is pinned to win-x64.");

        EnsureBundledNativeArtifacts();
        var result = new FlutterWindowsAngleEglContext(
            Environment.CurrentManagedThreadId,
            GetCurrentThreadId());
        try
        {
            result.InitializeOnRasterThread();
            return result;
        }
        catch
        {
            result.Dispose();
            throw;
        }
    }

    /// <summary>
    /// Executes the F1 offscreen smoke. Successful Skia submit and EGL pbuffer
    /// swap prove the context ABI only; they are not a window present claim.
    /// </summary>
    internal FlutterWindowsAngleEglSmokeResult RunOffscreenSmoke()
    {
        EnsureRasterThread();
        ThrowIfDisposed();
        var surface = _surface ?? throw new InvalidOperationException(
            "The ANGLE Skia pbuffer surface is unavailable.");
        var context = _grContext ?? throw new InvalidOperationException(
            "The ANGLE Skia GPU context is unavailable.");

        surface.Canvas.Clear(SKColors.CornflowerBlue);
        surface.Canvas.Flush();
        context.Flush(surface);
        context.Submit(false);
        if (EglSwapBuffers(_display, _pbufferSurface) == EglFalse)
            ThrowEgl("eglSwapBuffers(pbuffer)");

        return new FlutterWindowsAngleEglSmokeResult(
            Renderer,
            NativeProvenance,
            _rasterManagedThreadId,
            _rasterNativeThreadId,
            1,
            1);
    }

    private void InitializeOnRasterThread()
    {
        EnsureRasterThread();
        _display = EglGetDisplay(0);
        if (_display == 0) ThrowEgl("eglGetDisplay");
        if (EglInitialize(_display, out _, out _) == EglFalse)
            ThrowEgl("eglInitialize");
        if (EglBindApi(EglOpenGlesApi) == EglFalse)
            ThrowEgl("eglBindAPI(EGL_OPENGL_ES_API)");

        var configAttributes = new[]
        {
            EglSurfaceType, EglPbufferBit,
            EglRenderableType, EglOpenGles2Bit,
            EglRedSize, 8,
            EglGreenSize, 8,
            EglBlueSize, 8,
            EglAlphaSize, 8,
            EglDepthSize, 8,
            EglStencilSize, 8,
            EglNone,
        };
        if (EglChooseConfig(
                _display,
                configAttributes,
                out _config,
                configSize: 1,
                out var configCount) == EglFalse || configCount != 1 || _config == 0)
        {
            ThrowEgl("eglChooseConfig(pbuffer ES2)");
        }

        var contextAttributes = new[]
        {
            EglContextClientVersion, 2,
            EglNone,
        };
        _eglContext = EglCreateContext(_display, _config, 0, contextAttributes);
        if (_eglContext == 0) ThrowEgl("eglCreateContext(ES2)");

        var pbufferAttributes = new[]
        {
            EglWidth, 1,
            EglHeight, 1,
            EglNone,
        };
        _pbufferSurface = EglCreatePbufferSurface(_display, _config, pbufferAttributes);
        if (_pbufferSurface == 0) ThrowEgl("eglCreatePbufferSurface(1x1)");
        MakeCurrent();

        _glInterface = GRGlInterface.CreateAngle(EglGetProcAddress) ??
            throw new InvalidOperationException(
                "Skia could not resolve the bundled ANGLE GLES interface.");
        _grContext = GRContext.CreateGl(_glInterface) ??
            throw new InvalidOperationException(
                "Skia could not create a GPU context over bundled ANGLE EGL.");
        _grContext.ResetContext(GRGlBackendState.All);

        GlGetIntegerv(GlSamples, out var samples);
        GlGetIntegerv(GlStencilBits, out var stencilBits);
        _renderTarget = new GRBackendRenderTarget(
            width: 1,
            height: 1,
            sampleCount: Math.Max(0, samples),
            stencilBits: Math.Max(0, stencilBits),
            new GRGlFramebufferInfo(0, GlRgba8));
        _surface = SKSurface.Create(
            _grContext,
            _renderTarget,
            GRSurfaceOrigin.BottomLeft,
            SKColorType.Rgba8888) ?? throw new InvalidOperationException(
                "Skia could not wrap the ANGLE pbuffer default framebuffer.");
        var renderer = GlGetString(GlRenderer);
        Renderer = renderer == 0
            ? "ANGLE renderer unavailable"
            : Marshal.PtrToStringAnsi(renderer) ?? "ANGLE renderer unavailable";
    }

    private void MakeCurrent()
    {
        if (_display == 0 || _pbufferSurface == 0 || _eglContext == 0)
            throw new InvalidOperationException("The ANGLE pbuffer context is incomplete.");
        if (EglMakeCurrent(_display, _pbufferSurface, _pbufferSurface, _eglContext) == EglFalse)
            ThrowEgl("eglMakeCurrent(pbuffer)");
    }

    private void EnsureRasterThread()
    {
        if (Environment.CurrentManagedThreadId != _rasterManagedThreadId ||
            GetCurrentThreadId() != _rasterNativeThreadId)
        {
            throw new InvalidOperationException(
                "ANGLE/EGL display, context, pbuffer, and Skia context are raster-thread-affine.");
        }
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    public void Dispose()
    {
        if (_disposed) return;
        EnsureRasterThread();
        _disposed = true;

        var failures = new List<string>();
        if (_display != 0 && _pbufferSurface != 0 && _eglContext != 0)
            CaptureEglCleanupResult(
                "eglMakeCurrent(pbuffer teardown)",
                () => EglMakeCurrent(_display, _pbufferSurface, _pbufferSurface, _eglContext),
                failures);
        CaptureManagedCleanup("Skia surface", () => _surface?.Dispose(), failures);
        _surface = null;
        CaptureManagedCleanup("Skia render target", () => _renderTarget?.Dispose(), failures);
        _renderTarget = null;
        CaptureManagedCleanup("Skia GRContext", () => _grContext?.Dispose(), failures);
        _grContext = null;
        CaptureManagedCleanup("Skia GL interface", () => _glInterface?.Dispose(), failures);
        _glInterface = null;

        if (_display != 0)
            CaptureEglCleanupResult(
                "eglMakeCurrent(no context)",
                () => EglMakeCurrent(_display, 0, 0, 0),
                failures);
        if (_display != 0 && _pbufferSurface != 0)
            CaptureEglCleanupResult(
                "eglDestroySurface(pbuffer)",
                () => EglDestroySurface(_display, _pbufferSurface),
                failures);
        if (_display != 0 && _eglContext != 0)
            CaptureEglCleanupResult(
                "eglDestroyContext", () => EglDestroyContext(_display, _eglContext), failures);
        if (_display != 0)
            CaptureEglCleanupResult("eglTerminate", () => EglTerminate(_display), failures);

        _display = 0;
        _config = 0;
        _eglContext = 0;
        _pbufferSurface = 0;

        if (failures.Count != 0)
            throw new InvalidOperationException(
                $"ANGLE/EGL teardown failed: {string.Join("; ", failures)}");
    }

    private static void EnsureBundledNativeArtifacts()
    {
        lock (NativeLibraryGate)
        {
            if (_nativeProvenance is not null) return;
            var baseDirectory = Path.GetFullPath(AppContext.BaseDirectory);
            var anglePath = Path.Combine(baseDirectory, AngleFileName);
            var skiaPath = Path.Combine(baseDirectory, SkiaFileName);
            var angleHash = VerifyBundledFile(anglePath, ExpectedAngleSha256, AnglePackageId);
            var skiaHash = VerifyBundledFile(skiaPath, ExpectedSkiaSha256, SkiaPackageId);

            // Preload the desktop ANGLE DLL by absolute path. The DllImport
            // resolver below returns this handle, so a missing output file cannot
            // fall back to a developer-machine PATH copy.
            var angleLibrary = NativeLibrary.Load(anglePath);
            var skiaLibrary = NativeLibrary.Load(skiaPath);
            VerifyLoadedModulePath(angleLibrary, anglePath, AnglePackageId);
            VerifyLoadedModulePath(skiaLibrary, skiaPath, SkiaPackageId);
            _angleLibrary = angleLibrary;
            _skiaLibrary = skiaLibrary;
            _nativeProvenance = new FlutterWindowsAngleNativeProvenance(
                AnglePackageId,
                AnglePackageVersion,
                anglePath,
                angleHash,
                SkiaPackageId,
                SkiaPackageVersion,
                skiaPath,
                skiaHash,
                RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant(),
                PathFallbackUsed: false);
        }
    }

    private static nint ResolveBundledAngleLibrary(
        string libraryName,
        Assembly assembly,
        DllImportSearchPath? searchPath)
    {
        _ = assembly;
        _ = searchPath;
        if (!string.Equals(libraryName, AngleFileName, StringComparison.Ordinal))
            return 0;
        EnsureBundledNativeArtifacts();
        return _angleLibrary;
    }

    private static string VerifyBundledFile(
        string path,
        string expectedSha256,
        string packageId)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException(
                $"Required {packageId} native artifact is absent from the app directory.", path);
        var hash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant();
        if (!string.Equals(hash, expectedSha256, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Required {packageId} native artifact hash drifted: {Path.GetFileName(path)}.");
        }
        return hash;
    }

    private static void VerifyLoadedModulePath(
        nint module,
        string expectedPath,
        string packageId)
    {
        var buffer = new StringBuilder(32768);
        var length = GetModuleFileNameW(module, buffer, buffer.Capacity);
        if (length == 0 || length >= buffer.Capacity)
        {
            throw new InvalidOperationException(
                $"Could not resolve the loaded {packageId} native module path.");
        }

        var actualPath = Path.GetFullPath(buffer.ToString());
        if (!string.Equals(actualPath, Path.GetFullPath(expectedPath),
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"{packageId} loaded from an unexpected native path: {actualPath}");
        }
    }

    private static void CaptureManagedCleanup(
        string operation,
        Action cleanup,
        ICollection<string> failures)
    {
        try
        {
            cleanup();
        }
        catch (Exception exception)
        {
            Interlocked.Increment(ref _teardownFailureCount);
            failures.Add($"{operation}: {exception.GetType().Name}");
        }
    }

    private static void CaptureEglCleanupResult(
        string operation,
        Func<int> cleanup,
        ICollection<string> failures)
    {
        int result;
        try
        {
            result = cleanup();
        }
        catch (Exception exception)
        {
            Interlocked.Increment(ref _teardownFailureCount);
            failures.Add($"{operation}: {exception.GetType().Name}");
            return;
        }
        if (result != EglFalse) return;
        Interlocked.Increment(ref _teardownFailureCount);
        failures.Add($"{operation} failed with EGL error 0x{EglGetError():x4}");
    }

    private void ThrowEgl(string operation)
    {
        var error = EglGetError();
        throw new InvalidOperationException($"{operation} failed with EGL error 0x{error:x4}.");
    }

    [DllImport(AngleFileName, EntryPoint = "EGL_GetDisplay", ExactSpelling = true,
        CallingConvention = CallingConvention.Cdecl)]
    private static extern nint EglGetDisplay(nint nativeDisplay);

    [DllImport(AngleFileName, EntryPoint = "EGL_Initialize", ExactSpelling = true,
        CallingConvention = CallingConvention.Cdecl)]
    private static extern int EglInitialize(nint display, out int major, out int minor);

    [DllImport(AngleFileName, EntryPoint = "EGL_BindAPI", ExactSpelling = true,
        CallingConvention = CallingConvention.Cdecl)]
    private static extern int EglBindApi(int api);

    [DllImport(AngleFileName, EntryPoint = "EGL_ChooseConfig", ExactSpelling = true,
        CallingConvention = CallingConvention.Cdecl)]
    private static extern int EglChooseConfig(
        nint display,
        int[] attributes,
        out nint config,
        int configSize,
        out int configCount);

    [DllImport(AngleFileName, EntryPoint = "EGL_CreateContext", ExactSpelling = true,
        CallingConvention = CallingConvention.Cdecl)]
    private static extern nint EglCreateContext(
        nint display,
        nint config,
        nint sharedContext,
        int[] attributes);

    [DllImport(AngleFileName, EntryPoint = "EGL_CreatePbufferSurface", ExactSpelling = true,
        CallingConvention = CallingConvention.Cdecl)]
    private static extern nint EglCreatePbufferSurface(
        nint display,
        nint config,
        int[] attributes);

    [DllImport(AngleFileName, EntryPoint = "EGL_MakeCurrent", ExactSpelling = true,
        CallingConvention = CallingConvention.Cdecl)]
    private static extern int EglMakeCurrent(
        nint display,
        nint drawSurface,
        nint readSurface,
        nint context);

    [DllImport(AngleFileName, EntryPoint = "EGL_SwapBuffers", ExactSpelling = true,
        CallingConvention = CallingConvention.Cdecl)]
    private static extern int EglSwapBuffers(nint display, nint surface);

    [DllImport(AngleFileName, EntryPoint = "EGL_DestroySurface", ExactSpelling = true,
        CallingConvention = CallingConvention.Cdecl)]
    private static extern int EglDestroySurface(nint display, nint surface);

    [DllImport(AngleFileName, EntryPoint = "EGL_DestroyContext", ExactSpelling = true,
        CallingConvention = CallingConvention.Cdecl)]
    private static extern int EglDestroyContext(nint display, nint context);

    [DllImport(AngleFileName, EntryPoint = "EGL_Terminate", ExactSpelling = true,
        CallingConvention = CallingConvention.Cdecl)]
    private static extern int EglTerminate(nint display);

    [DllImport(AngleFileName, EntryPoint = "EGL_GetError", ExactSpelling = true,
        CallingConvention = CallingConvention.Cdecl)]
    private static extern int EglGetError();

    [DllImport(AngleFileName, EntryPoint = "EGL_GetProcAddress", ExactSpelling = true,
        CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern nint EglGetProcAddress(string name);

    [DllImport(AngleFileName, EntryPoint = "glGetIntegerv", ExactSpelling = true,
        CallingConvention = CallingConvention.Cdecl)]
    private static extern void GlGetIntegerv(uint name, out int value);

    [DllImport(AngleFileName, EntryPoint = "glGetString", ExactSpelling = true,
        CallingConvention = CallingConvention.Cdecl)]
    private static extern nint GlGetString(uint name);

    [DllImport("kernel32.dll")]
    private static extern uint GetCurrentThreadId();

    [DllImport("kernel32.dll", EntryPoint = "GetModuleFileNameW", CharSet = CharSet.Unicode,
        SetLastError = true)]
    private static extern uint GetModuleFileNameW(nint module, StringBuilder fileName, int size);
}

/// <summary>
/// Immutable exact-path provenance for the bundled F1 native graph.
/// </summary>
internal sealed record FlutterWindowsAngleNativeProvenance(
    string AnglePackageId,
    string AnglePackageVersion,
    string AnglePath,
    string AngleSha256,
    string SkiaPackageId,
    string SkiaPackageVersion,
    string SkiaPath,
    string SkiaSha256,
    string Architecture,
    bool PathFallbackUsed);

/// <summary>
/// Records only an offscreen F1 ABI smoke; it is not a visible window frame.
/// </summary>
internal sealed record FlutterWindowsAngleEglSmokeResult(
    string Renderer,
    FlutterWindowsAngleNativeProvenance NativeProvenance,
    int RasterManagedThreadId,
    uint RasterNativeThreadId,
    int PbufferWidth,
    int PbufferHeight)
{
    internal bool SoftwareFallback =>
        Renderer.Contains("warp", StringComparison.OrdinalIgnoreCase) ||
        Renderer.Contains("swiftshader", StringComparison.OrdinalIgnoreCase) ||
        Renderer.Contains("software", StringComparison.OrdinalIgnoreCase) ||
        Renderer.Contains("basic render driver", StringComparison.OrdinalIgnoreCase);
}
