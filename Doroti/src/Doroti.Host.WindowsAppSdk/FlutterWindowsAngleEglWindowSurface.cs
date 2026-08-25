using System.ComponentModel;
using System.Runtime.InteropServices;
using SkiaSharp;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>
/// Raster-thread-owned ANGLE display/context/cache.  The parking pbuffer is
/// intentionally independent from F4's child-HWND window surface: a resize
/// may replace the latter without losing the shared Skia GPU cache.
/// </summary>
internal sealed class FlutterWindowsAngleEglSharedContext : IDisposable
{
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
    private const int EglWindowBit = 0x0004;
    private const int EglRenderableType = 0x3040;
    private const int EglOpenGles2Bit = 0x0004;
    private const int EglContextClientVersion = 0x3098;
    private const int EglOpenGlesApi = 0x30A0;
    private const int EglWidth = 0x3057;
    private const int EglHeight = 0x3056;

    private readonly int _rasterManagedThreadId;
    private readonly uint _rasterNativeThreadId;
    private nint _display;
    private nint _config;
    private nint _eglContext;
    private nint _parkingSurface;
    private GRGlInterface? _glInterface;
    private GRContext? _grContext;
    private int _contextGeneration;
    private bool _disposed;

    private FlutterWindowsAngleEglSharedContext(
        FlutterWindowsAngleNativeProvenance nativeProvenance,
        int rasterManagedThreadId,
        uint rasterNativeThreadId)
    {
        NativeProvenance = nativeProvenance;
        _rasterManagedThreadId = rasterManagedThreadId;
        _rasterNativeThreadId = rasterNativeThreadId;
    }

    internal FlutterWindowsAngleNativeProvenance NativeProvenance { get; }

    internal string Renderer { get; private set; } = "uninitialized";

    internal bool SoftwareFallback =>
        Renderer.Contains("warp", StringComparison.OrdinalIgnoreCase) ||
        Renderer.Contains("swiftshader", StringComparison.OrdinalIgnoreCase) ||
        Renderer.Contains("software", StringComparison.OrdinalIgnoreCase) ||
        Renderer.Contains("basic render driver", StringComparison.OrdinalIgnoreCase);

    internal int ContextGeneration => _contextGeneration;

    internal int RasterManagedThreadId => _rasterManagedThreadId;

    internal uint RasterNativeThreadId => _rasterNativeThreadId;

    internal nint Display => _display;

    internal nint Config => _config;

    internal GRContext SkiaContext => _grContext ?? throw new ObjectDisposedException(
        nameof(FlutterWindowsAngleEglSharedContext));

    internal static FlutterWindowsAngleEglSharedContext CreateOnCurrentRasterThread()
    {
        var nativeProvenance = FlutterWindowsAngleEglContext
            .EnsureNativeArtifactsForWindowSurface();
        var result = new FlutterWindowsAngleEglSharedContext(
            nativeProvenance,
            Environment.CurrentManagedThreadId,
            FlutterWindowsAngleEglNative.GetCurrentThreadId());
        try
        {
            result.InitializeContextAndCache();
            return result;
        }
        catch
        {
            result.DisposeAfterFailedInitialization();
            throw;
        }
    }

    /// <summary>
    /// Binds the persistent context to an F4 child-HWND window surface and
    /// explicitly reapplies the presentation interval after every recreate.
    /// </summary>
    internal void MakeWindowSurfaceCurrent(nint windowSurface)
    {
        EnsureRasterThread();
        ThrowIfDisposed();
        if (windowSurface == 0)
            throw new ArgumentOutOfRangeException(nameof(windowSurface));
        if (FlutterWindowsAngleEglNative.EglMakeCurrent(
                _display, windowSurface, windowSurface, _eglContext) == EglFalse)
        {
            ThrowEgl("eglMakeCurrent(child window surface)");
        }
        SetSwapInterval();
    }

    /// <summary>
    /// Restores the reusable cache context to its private parking pbuffer
    /// before an F4 window surface is destroyed or replaced.
    /// </summary>
    internal void RestoreParkingSurfaceCurrent()
    {
        EnsureRasterThread();
        ThrowIfDisposed();
        if (FlutterWindowsAngleEglNative.EglMakeCurrent(
                _display, _parkingSurface, _parkingSurface, _eglContext) == EglFalse)
        {
            ThrowEgl("eglMakeCurrent(shared parking surface)");
        }
        SetSwapInterval();
    }

    /// <summary>
    /// Recreates only the lost EGL context/cache after the window owner has
    /// released its dependent surface.  This is intentionally not a resize
    /// path: normal child-HWND recreates retain the existing cache/context.
    /// </summary>
    internal void RecreateAfterContextOrDeviceLoss()
    {
        EnsureRasterThread();
        ThrowIfDisposed();
        DisposeContextAndCache(tolerateEglFailure: true);
        InitializeContextAndCache();
    }

    internal void EnsureRasterThread()
    {
        if (Environment.CurrentManagedThreadId != _rasterManagedThreadId ||
            FlutterWindowsAngleEglNative.GetCurrentThreadId() != _rasterNativeThreadId)
        {
            throw new InvalidOperationException(
                "The F4 ANGLE/EGL shared context and Skia cache are raster-thread-affine.");
        }
    }

    internal void ThrowEgl(string operation)
    {
        var error = FlutterWindowsAngleEglNative.EglGetError();
        throw new InvalidOperationException($"{operation} failed with EGL error 0x{error:x4}.");
    }

    public void Dispose()
    {
        if (_disposed) return;
        EnsureRasterThread();
        _disposed = true;
        DisposeContextAndCache(tolerateEglFailure: false);
    }

    private void InitializeContextAndCache()
    {
        EnsureRasterThread();
        _display = FlutterWindowsAngleEglNative.EglGetDisplay(0);
        if (_display == 0) ThrowEgl("eglGetDisplay");
        if (FlutterWindowsAngleEglNative.EglInitialize(_display, out _, out _) == EglFalse)
            ThrowEgl("eglInitialize");
        if (FlutterWindowsAngleEglNative.EglBindApi(EglOpenGlesApi) == EglFalse)
            ThrowEgl("eglBindAPI(EGL_OPENGL_ES_API)");

        var configAttributes = new[]
        {
            EglSurfaceType, EglPbufferBit | EglWindowBit,
            EglRenderableType, EglOpenGles2Bit,
            EglRedSize, 8,
            EglGreenSize, 8,
            EglBlueSize, 8,
            EglAlphaSize, 8,
            EglDepthSize, 8,
            EglStencilSize, 8,
            EglNone,
        };
        if (FlutterWindowsAngleEglNative.EglChooseConfig(
                _display,
                configAttributes,
                out _config,
                configSize: 1,
                out var configCount) == EglFalse || configCount < 1 || _config == 0)
        {
            ThrowEgl("eglChooseConfig(window+pbuffer ES2)");
        }

        var contextAttributes = new[]
        {
            EglContextClientVersion, 2,
            EglNone,
        };
        _eglContext = FlutterWindowsAngleEglNative.EglCreateContext(
            _display, _config, 0, contextAttributes);
        if (_eglContext == 0) ThrowEgl("eglCreateContext(ES2)");

        var pbufferAttributes = new[]
        {
            EglWidth, 1,
            EglHeight, 1,
            EglNone,
        };
        _parkingSurface = FlutterWindowsAngleEglNative.EglCreatePbufferSurface(
            _display, _config, pbufferAttributes);
        if (_parkingSurface == 0) ThrowEgl("eglCreatePbufferSurface(shared parking)");

        if (FlutterWindowsAngleEglNative.EglMakeCurrent(
                _display, _parkingSurface, _parkingSurface, _eglContext) == EglFalse)
        {
            ThrowEgl("eglMakeCurrent(shared parking initialization)");
        }
        SetSwapInterval();

        _glInterface = GRGlInterface.CreateAngle(FlutterWindowsAngleEglNative.EglGetProcAddress) ??
            throw new InvalidOperationException(
                "Skia could not resolve the bundled ANGLE GLES interface for the F4 shared context.");
        _grContext = GRContext.CreateGl(_glInterface) ??
            throw new InvalidOperationException(
                "Skia could not create the F4 shared GPU cache over bundled ANGLE EGL.");
        _grContext.ResetContext(GRGlBackendState.All);
        var renderer = FlutterWindowsAngleEglNative.GlGetString(FlutterWindowsAngleEglNative.GlRenderer);
        Renderer = renderer == 0
            ? "ANGLE renderer unavailable"
            : Marshal.PtrToStringAnsi(renderer) ?? "ANGLE renderer unavailable";
        checked { _contextGeneration++; }
    }

    private void SetSwapInterval()
    {
        // Flutter lets DWM own vblank pacing while desktop composition is
        // enabled. Waiting in both eglSwapBuffers and the post-present
        // DwmFlush serializes two compositor waits and makes interactive
        // resize visibly step at a fraction of the display cadence.
        var interval = 1;
        if (FlutterWindowsAngleEglNative.DwmIsCompositionEnabled(out var compositionEnabled) >= 0 &&
            compositionEnabled)
        {
            interval = 0;
        }
        if (FlutterWindowsAngleEglNative.EglSwapInterval(_display, interval) == EglFalse)
            ThrowEgl($"eglSwapInterval({interval})");
    }

    private void DisposeAfterFailedInitialization()
    {
        try
        {
            DisposeContextAndCache(tolerateEglFailure: true);
        }
        catch
        {
            // Preserve the original initialization failure.
        }
    }

    private void DisposeContextAndCache(bool tolerateEglFailure)
    {
        var failures = new List<string>();
        if (_display != 0 && _parkingSurface != 0 && _eglContext != 0)
        {
            CaptureEglCleanup(
                "eglMakeCurrent(shared parking teardown)",
                () => FlutterWindowsAngleEglNative.EglMakeCurrent(
                    _display, _parkingSurface, _parkingSurface, _eglContext),
                failures);
        }
        CaptureManagedCleanup("Skia shared GRContext", () => _grContext?.Dispose(), failures);
        _grContext = null;
        CaptureManagedCleanup("Skia shared GL interface", () => _glInterface?.Dispose(), failures);
        _glInterface = null;

        if (_display != 0)
        {
            CaptureEglCleanup(
                "eglMakeCurrent(no context)",
                () => FlutterWindowsAngleEglNative.EglMakeCurrent(_display, 0, 0, 0),
                failures);
        }
        if (_display != 0 && _parkingSurface != 0)
        {
            CaptureEglCleanup(
                "eglDestroySurface(shared parking)",
                () => FlutterWindowsAngleEglNative.EglDestroySurface(_display, _parkingSurface),
                failures);
        }
        if (_display != 0 && _eglContext != 0)
        {
            CaptureEglCleanup(
                "eglDestroyContext(shared)",
                () => FlutterWindowsAngleEglNative.EglDestroyContext(_display, _eglContext),
                failures);
        }
        if (_display != 0)
        {
            CaptureEglCleanup(
                "eglTerminate(shared)",
                () => FlutterWindowsAngleEglNative.EglTerminate(_display),
                failures);
        }

        _display = 0;
        _config = 0;
        _eglContext = 0;
        _parkingSurface = 0;
        if (!tolerateEglFailure && failures.Count != 0)
        {
            throw new InvalidOperationException(
                $"F4 shared ANGLE/EGL teardown failed: {string.Join("; ", failures)}");
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
            failures.Add($"{operation}: {exception.GetType().Name}");
        }
    }

    private static void CaptureEglCleanup(
        string operation,
        Func<int> cleanup,
        ICollection<string> failures)
    {
        try
        {
            if (cleanup() != EglFalse) return;
            failures.Add($"{operation} failed with EGL error 0x{FlutterWindowsAngleEglNative.EglGetError():x4}");
        }
        catch (Exception exception)
        {
            failures.Add($"{operation}: {exception.GetType().Name}");
        }
    }

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed, this);
}

/// <summary>
/// Per-child-HWND F4 window surface.  Only this object owns the EGL window
/// surface and default-framebuffer Skia target; it never owns the shared
/// ANGLE context or Skia cache.
/// </summary>
internal sealed class FlutterWindowsAngleEglWindowSurface : IFlutterWindowsScheduledSurface
{
    private const int EglFalse = 0;
    private const int EglTrue = 1;
    private const int EglNone = 0x3038;
    private const int EglWidth = 0x3057;
    private const int EglHeight = 0x3056;
    private const int EglFixedSizeAngle = 0x3201;
    private const int EglContextLost = 0x300E;
    private const uint GlRgba8 = 0x8058;

    private readonly FlutterWindowsAngleEglSharedContext _sharedContext;
    private readonly nint _childHwnd;
    private nint _eglWindowSurface;
    private GRBackendRenderTarget? _renderTarget;
    private SKSurface? _skSurface;
    private WindowsViewMetrics? _targetMetrics;
    private long _surfaceGeneration;
    private long _createCount;
    private long _destroyCount;
    private long _recreateCount;
    private long _swapAttemptCount;
    private long _successfulSwapCount;
    private long _exactExtentMismatchCount;
    private long _zeroSizedSurfaceCreateCount;
    private long _contextLossInjectionCount;
    private long _contextLossDetectedCount;
    private long _recoveryCount;
    private long _threadAffinityViolationCount;
    private long _createOutsideRasterThreadCount;
    private long _recreateOutsideRasterThreadCount;
    private long _swapOutsideRasterThreadCount;
    private bool _contextLossInjectionPending;
    private bool _validFrameAfterRecovery;
    private bool _disposed;

    private FlutterWindowsAngleEglWindowSurface(
        FlutterWindowsAngleEglSharedContext sharedContext,
        nint childHwnd)
    {
        _sharedContext = sharedContext;
        _childHwnd = childHwnd;
    }

    internal static FlutterWindowsAngleEglWindowSurface CreateOnCurrentRasterThread(
        FlutterWindowsAngleEglSharedContext sharedContext,
        nint childHwnd,
        WindowsViewMetrics targetMetrics)
    {
        ArgumentNullException.ThrowIfNull(sharedContext);
        ArgumentNullException.ThrowIfNull(targetMetrics);
        sharedContext.EnsureRasterThread();
        if (childHwnd == 0 || !FlutterWindowsAngleEglNative.IsWindow(childHwnd))
            throw new ArgumentOutOfRangeException(nameof(childHwnd), "The F4 child HWND is not valid.");

        var result = new FlutterWindowsAngleEglWindowSurface(sharedContext, childHwnd);
        try
        {
            result.UpdateForMetrics(targetMetrics);
            return result;
        }
        catch
        {
            result.DisposeAfterFailedCreate();
            throw;
        }
    }

    /// <summary>
    /// Applies one immutable F3 child-client metrics publication.  A zero-size
    /// publication destroys the native surface; an identical publication does
    /// not recreate it; every changed drawable publication is exact-pixel
    /// verified before the new EGL window surface becomes current.
    /// </summary>
    public FlutterWindowsAngleEglSurfaceUpdateResult UpdateForMetrics(WindowsViewMetrics targetMetrics)
    {
        ArgumentNullException.ThrowIfNull(targetMetrics);
        EnsureRasterThread(RasterOperation.Recreate);
        ThrowIfDisposed();
        if (!targetMetrics.HasDrawableSize)
        {
            if (targetMetrics.PhysicalWidth != 0 || targetMetrics.PhysicalHeight != 0)
            {
                throw new InvalidOperationException(
                    "A non-drawable F3 metrics publication must be a zero-sized child HWND.");
            }
            var destroyed = DestroyWindowSurfaceCore(tolerateEglFailure: false, retainTargetMetrics: false);
            _targetMetrics = targetMetrics;
            return new FlutterWindowsAngleEglSurfaceUpdateResult(
                Created: false,
                Recreated: false,
                DestroyedForSuspension: destroyed,
                SurfaceGeneration: _surfaceGeneration,
                PhysicalWidth: 0,
                PhysicalHeight: 0);
        }

        ValidateExactChildTarget(targetMetrics);
        if (_eglWindowSurface != 0 && _targetMetrics == targetMetrics)
        {
            return new FlutterWindowsAngleEglSurfaceUpdateResult(
                Created: false,
                Recreated: false,
                DestroyedForSuspension: false,
                SurfaceGeneration: _surfaceGeneration,
                PhysicalWidth: targetMetrics.PhysicalWidth,
                PhysicalHeight: targetMetrics.PhysicalHeight);
        }

        var hadEarlierSurface = _createCount != 0;
        if (_eglWindowSurface != 0)
            _ = DestroyWindowSurfaceCore(tolerateEglFailure: false, retainTargetMetrics: false);
        CreateWindowSurfaceCore(targetMetrics, hadEarlierSurface);
        return new FlutterWindowsAngleEglSurfaceUpdateResult(
            Created: !hadEarlierSurface,
            Recreated: hadEarlierSurface,
            DestroyedForSuspension: false,
            SurfaceGeneration: _surfaceGeneration,
            PhysicalWidth: targetMetrics.PhysicalWidth,
            PhysicalHeight: targetMetrics.PhysicalHeight);
    }

    /// <summary>
    /// Renders to the current exact F3 metrics target and ends in a real
    /// EGL_SwapBuffers call.  The caller can use the result, rather than a
    /// queued render intent, as the first-frame-show prerequisite.
    /// </summary>
    internal FlutterWindowsAngleEglPresentResult RenderAndSwap(
        WindowsViewMetrics targetMetrics,
        SKColor clearColor)
        => RenderAndSwap(targetMetrics, surface => surface.Canvas.Clear(clearColor));

    /// <summary>
    /// Paints the exact current F3 target and performs one real
    /// <c>eglSwapBuffers</c>.  The callback runs only after the raster owner
    /// has rebound the matching child-HWND surface; it must paint the supplied
    /// Skia surface and must not retain it beyond the callback.
    /// </summary>
    public FlutterWindowsAngleEglPresentResult RenderAndSwap(
        WindowsViewMetrics targetMetrics,
        Action<SKSurface> paint)
    {
        ArgumentNullException.ThrowIfNull(targetMetrics);
        ArgumentNullException.ThrowIfNull(paint);
        EnsureRasterThread(RasterOperation.Swap);
        ThrowIfDisposed();
        if (_eglWindowSurface == 0 || _skSurface is null || _targetMetrics != targetMetrics)
        {
            throw new InvalidOperationException(
                "F4 can only swap the exact currently bound WindowsViewMetrics target.");
        }

        var recoveredThisPresent = false;
        if (_contextLossInjectionPending)
        {
            _contextLossInjectionPending = false;
            Interlocked.Increment(ref _contextLossDetectedCount);
            RecoverFromContextOrDeviceLoss(targetMetrics, "deterministic context/device-loss injection");
            recoveredThisPresent = true;
        }

        for (var attempt = 0; attempt != 2; attempt++)
        {
            EnsureCurrentExactTarget(targetMetrics);
            var surface = _skSurface ?? throw new InvalidOperationException(
                "The F4 Skia window surface disappeared before presentation.");
            var context = _sharedContext.SkiaContext;
            paint(surface);
            surface.Canvas.Flush();
            context.Flush(surface);
            context.Submit(false);
            Interlocked.Increment(ref _swapAttemptCount);
            if (FlutterWindowsAngleEglNative.EglSwapBuffers(_sharedContext.Display, _eglWindowSurface) != EglFalse)
            {
                Interlocked.Increment(ref _successfulSwapCount);
                if (recoveredThisPresent) _validFrameAfterRecovery = true;
                return new FlutterWindowsAngleEglPresentResult(
                    targetMetrics.PhysicalWidth,
                    targetMetrics.PhysicalHeight,
                    _surfaceGeneration,
                    RecoveredFromContextLoss: recoveredThisPresent,
                    SuccessfulSwap: true);
            }

            var error = FlutterWindowsAngleEglNative.EglGetError();
            if (error != EglContextLost || attempt != 0)
            {
                throw new InvalidOperationException(
                    $"eglSwapBuffers(child window surface) failed with EGL error 0x{error:x4}.");
            }

            Interlocked.Increment(ref _contextLossDetectedCount);
            RecoverFromContextOrDeviceLoss(targetMetrics, "EGL_CONTEXT_LOST from eglSwapBuffers");
            recoveredThisPresent = true;
        }

        throw new InvalidOperationException("F4 exhausted its context-loss recovery presentation retry.");
    }

    /// <summary>
    /// Test-only deterministic loss signal.  The next present releases the
    /// live window surface, rebuilds the shared EGL/Skia cache, recreates the
    /// child surface, and proves recovery with a real swap.
    /// </summary>
    internal void InjectContextLossForValidation()
    {
        EnsureRasterThread(RasterOperation.Recreate);
        ThrowIfDisposed();
        if (_eglWindowSurface == 0 || _targetMetrics is null || !_targetMetrics.HasDrawableSize)
            throw new InvalidOperationException("F4 cannot inject context loss without a live drawable window surface.");
        _contextLossInjectionPending = true;
        Interlocked.Increment(ref _contextLossInjectionCount);
    }

    /// <summary>
    /// Requests the same bounded context/device rebuild used by the validated
    /// loss path, but records it as a product lifecycle recovery rather than a
    /// test injection. The rebuild occurs immediately before the next exact
    /// present so the current child metrics remain the sole surface authority.
    /// </summary>
    internal void RequestLifecycleRecovery()
    {
        EnsureRasterThread(RasterOperation.Recreate);
        ThrowIfDisposed();
        if (_eglWindowSurface == 0 || _targetMetrics is null || !_targetMetrics.HasDrawableSize)
            return;
        _contextLossInjectionPending = true;
    }

    internal FlutterWindowsAngleEglWindowSurfaceSnapshot Snapshot => new(
        _childHwnd,
        _targetMetrics,
        _sharedContext.ContextGeneration,
        _surfaceGeneration,
        Interlocked.Read(ref _createCount),
        Interlocked.Read(ref _destroyCount),
        Interlocked.Read(ref _recreateCount),
        Interlocked.Read(ref _swapAttemptCount),
        Interlocked.Read(ref _successfulSwapCount),
        Interlocked.Read(ref _exactExtentMismatchCount),
        Interlocked.Read(ref _zeroSizedSurfaceCreateCount),
        Interlocked.Read(ref _contextLossInjectionCount),
        Interlocked.Read(ref _contextLossDetectedCount),
        Interlocked.Read(ref _recoveryCount),
        _validFrameAfterRecovery,
        SharedContextResourceLifetimeSeparated: true,
        EglSurfaceLeakCount: _eglWindowSurface == 0 ? 0 : 1,
        SkiaResourceLeakCount: (_renderTarget is null ? 0 : 1) + (_skSurface is null ? 0 : 1),
        _sharedContext.Renderer,
        _sharedContext.SoftwareFallback,
        _sharedContext.RasterManagedThreadId,
        _sharedContext.RasterNativeThreadId,
        Interlocked.Read(ref _createOutsideRasterThreadCount),
        Interlocked.Read(ref _recreateOutsideRasterThreadCount),
        Interlocked.Read(ref _swapOutsideRasterThreadCount),
        Interlocked.Read(ref _threadAffinityViolationCount),
        _disposed);

    public void Dispose()
    {
        if (_disposed) return;
        EnsureRasterThread(RasterOperation.Dispose);
        _disposed = true;
        _ = DestroyWindowSurfaceCore(tolerateEglFailure: false, retainTargetMetrics: false);
    }

    private void CreateWindowSurfaceCore(WindowsViewMetrics targetMetrics, bool recreation)
    {
        ValidateExactChildTarget(targetMetrics);
        nint nativeSurface = 0;
        GRBackendRenderTarget? renderTarget = null;
        SKSurface? skSurface = null;
        try
        {
            nativeSurface = FlutterWindowsAngleEglNative.EglCreateWindowSurface(
                _sharedContext.Display,
                _sharedContext.Config,
                _childHwnd,
                [
                    EglFixedSizeAngle, EglTrue,
                    EglWidth, targetMetrics.PhysicalWidth,
                    EglHeight, targetMetrics.PhysicalHeight,
                    EglNone,
                ]);
            if (nativeSurface == 0)
                _sharedContext.ThrowEgl("eglCreateWindowSurface(child HWND)");

            _sharedContext.MakeWindowSurfaceCurrent(nativeSurface);
            var eglWidth = 0;
            var eglHeight = 0;
            if (FlutterWindowsAngleEglNative.EglQuerySurface(
                    _sharedContext.Display, nativeSurface, EglWidth, out eglWidth) == EglFalse ||
                FlutterWindowsAngleEglNative.EglQuerySurface(
                    _sharedContext.Display, nativeSurface, EglHeight, out eglHeight) == EglFalse)
            {
                _sharedContext.ThrowEgl("eglQuerySurface(child window extent)");
            }
            if (eglWidth != targetMetrics.PhysicalWidth || eglHeight != targetMetrics.PhysicalHeight)
            {
                Interlocked.Increment(ref _exactExtentMismatchCount);
                throw new InvalidOperationException(
                    "The EGL child-window surface extent did not exactly match F3 WindowsViewMetrics: " +
                    $"EGL {eglWidth}x{eglHeight}; F3 {targetMetrics.PhysicalWidth}x{targetMetrics.PhysicalHeight}.");
            }

            FlutterWindowsAngleEglNative.GlGetIntegerv(
                FlutterWindowsAngleEglNative.GlSamples, out var samples);
            FlutterWindowsAngleEglNative.GlGetIntegerv(
                FlutterWindowsAngleEglNative.GlStencilBits, out var stencilBits);
            renderTarget = new GRBackendRenderTarget(
                targetMetrics.PhysicalWidth,
                targetMetrics.PhysicalHeight,
                Math.Max(0, samples),
                Math.Max(0, stencilBits),
                new GRGlFramebufferInfo(0, GlRgba8));
            skSurface = SKSurface.Create(
                _sharedContext.SkiaContext,
                renderTarget,
                GRSurfaceOrigin.BottomLeft,
                SKColorType.Rgba8888) ?? throw new InvalidOperationException(
                    "Skia could not wrap the exact EGL child-window default framebuffer.");

            _eglWindowSurface = nativeSurface;
            nativeSurface = 0;
            _renderTarget = renderTarget;
            renderTarget = null;
            _skSurface = skSurface;
            skSurface = null;
            _targetMetrics = targetMetrics;
            checked { _surfaceGeneration++; }
            Interlocked.Increment(ref _createCount);
            if (recreation) Interlocked.Increment(ref _recreateCount);
        }
        catch
        {
            try { skSurface?.Dispose(); } catch { }
            try { renderTarget?.Dispose(); } catch { }
            if (nativeSurface != 0)
            {
                try { _sharedContext.RestoreParkingSurfaceCurrent(); } catch { }
                _ = FlutterWindowsAngleEglNative.EglDestroySurface(_sharedContext.Display, nativeSurface);
            }
            throw;
        }
    }

    private bool DestroyWindowSurfaceCore(bool tolerateEglFailure, bool retainTargetMetrics)
    {
        if (_eglWindowSurface == 0)
        {
            if (!retainTargetMetrics) _targetMetrics = null;
            return false;
        }

        var nativeSurface = _eglWindowSurface;
        _eglWindowSurface = 0;
        var skSurface = _skSurface;
        _skSurface = null;
        var renderTarget = _renderTarget;
        _renderTarget = null;
        var failures = new List<string>();
        try
        {
            _sharedContext.MakeWindowSurfaceCurrent(nativeSurface);
        }
        catch (Exception exception)
        {
            failures.Add($"eglMakeCurrent(window-surface teardown): {exception.GetType().Name}");
        }
        CaptureManagedCleanup("Skia child window surface", () => skSurface?.Dispose(), failures);
        CaptureManagedCleanup("Skia child render target", () => renderTarget?.Dispose(), failures);
        try
        {
            _sharedContext.RestoreParkingSurfaceCurrent();
        }
        catch (Exception exception)
        {
            failures.Add($"eglMakeCurrent(shared parking restore): {exception.GetType().Name}");
        }
        try
        {
            if (FlutterWindowsAngleEglNative.EglDestroySurface(_sharedContext.Display, nativeSurface) == EglFalse)
            {
                failures.Add(
                    $"eglDestroySurface(child window) failed with EGL error 0x{FlutterWindowsAngleEglNative.EglGetError():x4}");
            }
        }
        catch (Exception exception)
        {
            failures.Add($"eglDestroySurface(child window): {exception.GetType().Name}");
        }

        Interlocked.Increment(ref _destroyCount);
        if (!retainTargetMetrics) _targetMetrics = null;
        if (!tolerateEglFailure && failures.Count != 0)
        {
            throw new InvalidOperationException(
                $"F4 child EGL window-surface teardown failed: {string.Join("; ", failures)}");
        }
        return true;
    }

    private void RecoverFromContextOrDeviceLoss(WindowsViewMetrics targetMetrics, string reason)
    {
        if (_targetMetrics != targetMetrics)
        {
            throw new InvalidOperationException(
                "F4 context recovery refused a stale WindowsViewMetrics target.");
        }
        _ = DestroyWindowSurfaceCore(tolerateEglFailure: true, retainTargetMetrics: true);
        _sharedContext.RecreateAfterContextOrDeviceLoss();
        CreateWindowSurfaceCore(targetMetrics, recreation: true);
        Interlocked.Increment(ref _recoveryCount);
        _ = reason; // Preserved in the explicit recovery call site for diagnostics/profiling hooks.
    }

    private void EnsureCurrentExactTarget(WindowsViewMetrics targetMetrics)
    {
        if (_targetMetrics != targetMetrics || _eglWindowSurface == 0 || _skSurface is null)
            throw new InvalidOperationException("F4 lost its exact child-window render target before presentation.");
        _sharedContext.MakeWindowSurfaceCurrent(_eglWindowSurface);
        _sharedContext.SkiaContext.ResetContext(GRGlBackendState.All);
    }

    private void ValidateExactChildTarget(WindowsViewMetrics targetMetrics)
    {
        if (!targetMetrics.HasDrawableSize)
        {
            Interlocked.Increment(ref _zeroSizedSurfaceCreateCount);
            throw new InvalidOperationException("F4 must not create an EGL window surface for suspended metrics.");
        }
        if (!FlutterWindowsAngleEglNative.GetClientRect(_childHwnd, out var clientRect))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(),
                "GetClientRect failed while validating the F4 child HWND target.");
        }
        if (clientRect.Width != targetMetrics.PhysicalWidth ||
            clientRect.Height != targetMetrics.PhysicalHeight)
        {
            Interlocked.Increment(ref _exactExtentMismatchCount);
            throw new InvalidOperationException(
                "F4 refused a surface target that diverged from the F3 child-client physical metrics: " +
                $"HWND {clientRect.Width}x{clientRect.Height}; F3 {targetMetrics.PhysicalWidth}x{targetMetrics.PhysicalHeight}.");
        }
    }

    private void EnsureRasterThread(RasterOperation operation)
    {
        try
        {
            _sharedContext.EnsureRasterThread();
        }
        catch (InvalidOperationException)
        {
            Interlocked.Increment(ref _threadAffinityViolationCount);
            switch (operation)
            {
                case RasterOperation.Create:
                    Interlocked.Increment(ref _createOutsideRasterThreadCount);
                    break;
                case RasterOperation.Recreate:
                    Interlocked.Increment(ref _recreateOutsideRasterThreadCount);
                    break;
                case RasterOperation.Swap:
                    Interlocked.Increment(ref _swapOutsideRasterThreadCount);
                    break;
            }
            throw;
        }
    }

    private void DisposeAfterFailedCreate()
    {
        try
        {
            _ = DestroyWindowSurfaceCore(tolerateEglFailure: true, retainTargetMetrics: false);
        }
        catch
        {
            // Preserve the original create failure.
        }
    }

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed, this);

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
            failures.Add($"{operation}: {exception.GetType().Name}");
        }
    }

    private enum RasterOperation
    {
        Create,
        Recreate,
        Swap,
        Dispose,
    }
}

internal sealed record FlutterWindowsAngleEglSurfaceUpdateResult(
    bool Created,
    bool Recreated,
    bool DestroyedForSuspension,
    long SurfaceGeneration,
    int PhysicalWidth,
    int PhysicalHeight);

internal sealed record FlutterWindowsAngleEglPresentResult(
    int PhysicalWidth,
    int PhysicalHeight,
    long SurfaceGeneration,
    bool RecoveredFromContextLoss,
    bool SuccessfulSwap);

internal sealed record FlutterWindowsAngleEglWindowSurfaceSnapshot(
    nint ChildHwnd,
    WindowsViewMetrics? TargetMetrics,
    int SharedContextGeneration,
    long SurfaceGeneration,
    long CreateCount,
    long DestroyCount,
    long RecreateCount,
    long SwapAttemptCount,
    long SuccessfulSwapCount,
    long ExactExtentMismatchCount,
    long ZeroSizedSurfaceCreateCount,
    long ContextLossInjectionCount,
    long ContextLossDetectedCount,
    long RecoveryCount,
    bool ValidFrameAfterRecovery,
    bool SharedContextResourceLifetimeSeparated,
    long EglSurfaceLeakCount,
    long SkiaResourceLeakCount,
    string Renderer,
    bool SoftwareFallback,
    int RasterManagedThreadId,
    uint RasterNativeThreadId,
    long CreateOutsideRasterThreadCount,
    long RecreateOutsideRasterThreadCount,
    long SwapOutsideRasterThreadCount,
    long ThreadAffinityViolationCount,
    bool Disposed);

/// <summary>
/// F4-local native entry points.  The F1 class installs the assembly resolver
/// and verifies the exact app-directory module before this type is used.
/// </summary>
internal static class FlutterWindowsAngleEglNative
{
    internal const uint GlSamples = 0x80A9;
    internal const uint GlStencilBits = 0x0D57;
    internal const uint GlRenderer = 0x1F01;

    [DllImport(FlutterWindowsAngleEglContext.AngleFileName, EntryPoint = "EGL_GetDisplay",
        ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint EglGetDisplay(nint nativeDisplay);

    [DllImport(FlutterWindowsAngleEglContext.AngleFileName, EntryPoint = "EGL_Initialize",
        ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int EglInitialize(nint display, out int major, out int minor);

    [DllImport(FlutterWindowsAngleEglContext.AngleFileName, EntryPoint = "EGL_BindAPI",
        ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int EglBindApi(int api);

    [DllImport(FlutterWindowsAngleEglContext.AngleFileName, EntryPoint = "EGL_ChooseConfig",
        ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int EglChooseConfig(
        nint display,
        int[] attributes,
        out nint config,
        int configSize,
        out int configCount);

    [DllImport(FlutterWindowsAngleEglContext.AngleFileName, EntryPoint = "EGL_CreateContext",
        ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint EglCreateContext(
        nint display,
        nint config,
        nint sharedContext,
        int[] attributes);

    [DllImport(FlutterWindowsAngleEglContext.AngleFileName, EntryPoint = "EGL_CreatePbufferSurface",
        ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint EglCreatePbufferSurface(
        nint display,
        nint config,
        int[] attributes);

    [DllImport(FlutterWindowsAngleEglContext.AngleFileName, EntryPoint = "EGL_CreateWindowSurface",
        ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint EglCreateWindowSurface(
        nint display,
        nint config,
        nint nativeWindow,
        int[] attributes);

    [DllImport(FlutterWindowsAngleEglContext.AngleFileName, EntryPoint = "EGL_QuerySurface",
        ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int EglQuerySurface(
        nint display,
        nint surface,
        int attribute,
        out int value);

    [DllImport(FlutterWindowsAngleEglContext.AngleFileName, EntryPoint = "EGL_MakeCurrent",
        ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int EglMakeCurrent(
        nint display,
        nint drawSurface,
        nint readSurface,
        nint context);

    [DllImport(FlutterWindowsAngleEglContext.AngleFileName, EntryPoint = "EGL_SwapInterval",
        ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int EglSwapInterval(nint display, int interval);

    [DllImport(FlutterWindowsAngleEglContext.AngleFileName, EntryPoint = "EGL_SwapBuffers",
        ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int EglSwapBuffers(nint display, nint surface);

    [DllImport(FlutterWindowsAngleEglContext.AngleFileName, EntryPoint = "EGL_DestroySurface",
        ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int EglDestroySurface(nint display, nint surface);

    [DllImport(FlutterWindowsAngleEglContext.AngleFileName, EntryPoint = "EGL_DestroyContext",
        ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int EglDestroyContext(nint display, nint context);

    [DllImport(FlutterWindowsAngleEglContext.AngleFileName, EntryPoint = "EGL_Terminate",
        ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int EglTerminate(nint display);

    [DllImport(FlutterWindowsAngleEglContext.AngleFileName, EntryPoint = "EGL_GetError",
        ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int EglGetError();

    [DllImport(FlutterWindowsAngleEglContext.AngleFileName, EntryPoint = "EGL_GetProcAddress",
        ExactSpelling = true, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern nint EglGetProcAddress(string name);

    [DllImport(FlutterWindowsAngleEglContext.AngleFileName, EntryPoint = "glGetIntegerv",
        ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void GlGetIntegerv(uint name, out int value);

    [DllImport(FlutterWindowsAngleEglContext.AngleFileName, EntryPoint = "glGetString",
        ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint GlGetString(uint name);

    [DllImport("kernel32.dll")]
    internal static extern uint GetCurrentThreadId();

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool IsWindow(nint hwnd);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetClientRect(nint hwnd, out NativeRect rect);

    [DllImport("dwmapi.dll")]
    internal static extern int DwmIsCompositionEnabled(
        [MarshalAs(UnmanagedType.Bool)] out bool enabled);

    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeRect
    {
        internal int Left;
        internal int Top;
        internal int Right;
        internal int Bottom;
        internal int Width => Right - Left;
        internal int Height => Bottom - Top;
    }
}
