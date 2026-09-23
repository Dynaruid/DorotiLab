using System.Runtime.InteropServices;
using System.Diagnostics;
using Doroti.Skia.Vulkan;
using SkiaSharp;

namespace Doroti.Host.Qt;

internal sealed class QtSkiaSurface(GRGlGetProcedureAddressDelegate getProcedureAddress)
    : IDisposable
{
    internal static bool GraphiteEnabled =>
        Environment.GetEnvironmentVariable("DOROTI_LINUX_GRAPHITE") != "0";
    private GraphiteVulkanWindow? _vulkan;
    internal GraphiteVulkanQuick? QuickGpu { get; private set; }
    internal QtQuickNative.Part[] QuickParts { get; set; } = [];
    internal bool QuickEnabled { get; private set; }
    internal ulong QuickPeakReservedBytes { get; private set; }
    internal int QuickPeakRetiringLayers { get; private set; }
    internal GraphiteVulkanQuick.TimingSummary? QuickTimings { get; private set; }
    private readonly List<double> _quickCommitMs = [];
    internal GraphiteVulkanQuick.Percentiles QuickNativeCommitTimings
    {
        get
        {
            if (_quickCommitMs.Count == 0) return new(0, null, null, null);
            var values = _quickCommitMs.Order().ToArray();
            double At(double p) => values[Math.Clamp((int)Math.Ceiling(p * values.Length) - 1, 0, values.Length - 1)];
            return new(values.Length, At(.5), At(.95), At(.99));
        }
    }
    private nint _quickWindow;
    private bool _nativeTextures;
    internal bool NativeTexturesConfigured => !QuickEnabled || _nativeTextures;

    internal void ConfigureQuick(nint window, bool enabled, bool nativeTextures)
    {
        _quickWindow = window;
        QuickEnabled = enabled;
        _nativeTextures = nativeTextures;
    }

    internal SKCanvas QuickCaptionCanvas(in QtNativeV2.Surface descriptor)
    {
        var index = QuickParts.Count(part => part.Kind == 0);
        var canvas = QuickGpu!.Canvas(index);
        var bounds = new QtPlatformViewHost.NativeRect(
            0,
            0,
            descriptor.PixelWidth / descriptor.DevicePixelRatio,
            descriptor.PixelHeight / descriptor.DevicePixelRatio
        );
        QuickParts =
        [
            .. QuickParts,
            new QtQuickNative.Part
            {
                Size = 96,
                Kind = 0,
                Id = QuickGpu.Identity(index),
                Image = QuickGpu.Image(index),
                PixelWidth = (uint)descriptor.PixelWidth,
                PixelHeight = (uint)descriptor.PixelHeight,
                Bounds = bounds,
                Clip = bounds,
            },
        ];
        return canvas;
    }

    internal bool SoftwareVulkan { get; private set; }
    internal event Action? GpuResourcesReleasing;
    private readonly GRGlGetProcedureAddressDelegate _getProcedureAddress =
        getProcedureAddress ?? throw new ArgumentNullException(nameof(getProcedureAddress));
    private GRGlInterface? _interface;
    private GRContext? _context;
    private GRBackendRenderTarget? _target;
    private SKSurface? _surface;
    private ulong _surfaceGeneration;
    private ulong _contextIdentity;
    private uint _framebufferObject;
    private int _pixelWidth;
    private int _pixelHeight;
    private int _sampleCount;
    private int _stencilBits;
    private uint _colorFormat;
    private bool _usePlatformGlResolver;
    private bool _disposed;

    internal unsafe bool Render(
        in QtNativeV2.Surface descriptor,
        Action<SKSurface, int, int> render,
        Func<bool>? shouldPresent = null,
        Action? beforePresent = null,
        ulong frameToken = 0
    )
    {
        ArgumentNullException.ThrowIfNull(render);
        ObjectDisposedException.ThrowIf(_disposed, this);
        Validate(descriptor);
        if (QuickEnabled)
        {
            if (QuickGpu is null)
            {
                var gpu = QtQuickNative.Get(_quickWindow);
                QuickGpu = new GraphiteVulkanQuick(
                    gpu.Instance,
                    gpu.Physical,
                    gpu.Device,
                    gpu.Queue,
                    gpu.Family,
                    gpu.ApiVersion,
                    _nativeTextures
                );
                QuickGpu.ResourcesReleasing += () => GpuResourcesReleasing?.Invoke();
                SoftwareVulkan = QuickGpu.IsSoftwareDevice;
                _contextIdentity = descriptor.ContextIdentity;
                _surfaceGeneration = descriptor.SurfaceGeneration;
            }
            var width = descriptor.PixelWidth;
            var height = descriptor.PixelHeight;
            QuickGpu.FrameToken = frameToken;
            var target = QuickGpu.Begin(width, height);
            var bounds = new QtPlatformViewHost.NativeRect(
                0,
                0,
                width / descriptor.DevicePixelRatio,
                height / descriptor.DevicePixelRatio
            );
            QuickParts =
            [
                new QtQuickNative.Part
                {
                    Size = 96,
                    Kind = 0,
                    Id = QuickGpu.Identity(0),
                    Image = QuickGpu.Image(0),
                    PixelWidth = (uint)width,
                    PixelHeight = (uint)height,
                    Bounds = bounds,
                    Clip = bounds,
                },
            ];
            try
            {
                render(target, width, height);
                if (shouldPresent?.Invoke() == false)
                {
                    QuickGpu.Cancel();
                    return false;
                }
                var commitStart = Stopwatch.GetTimestamp();
                QtQuickNative.Commit(_quickWindow, QuickParts, apply: false);
                var prepareMs = Stopwatch.GetElapsedTime(commitStart).TotalMilliseconds;
                QuickGpu.Complete();
                commitStart = Stopwatch.GetTimestamp();
                QtQuickNative.Commit(_quickWindow, QuickParts, apply: true);
                if (_quickCommitMs.Count < 10000)
                    _quickCommitMs.Add(prepareMs + Stopwatch.GetElapsedTime(commitStart).TotalMilliseconds);
                QuickGpu.MarkPublished();
                return true;
            }
            catch
            {
                if (QuickGpu.Failure == GraphiteVulkanQuick.FailureKind.None)
                {
                    try { QuickGpu.Cancel(); }
                    catch { /* Preserve the render/submit failure. */ }
                }
                throw;
            }
        }
        if (GraphiteEnabled)
        {
            if (
                descriptor.StructSize < 128
                || descriptor.VulkanInstance == 0
                || descriptor.VulkanSurface == 0
            )
            {
                throw new InvalidDataException(
                    "Qt Graphite requires the negotiated Vulkan surface descriptor."
                );
            }

            if (
                _vulkan is null
                || _contextIdentity != descriptor.ContextIdentity
                || _surfaceGeneration != descriptor.SurfaceGeneration
            )
            {
                ReleaseGpuResources();
                var extensions = descriptor.VulkanInstanceExtensions;
                if (extensions.Length > 65536)
                {
                    throw new InvalidDataException("Qt Vulkan extension list is too long.");
                }

                var names = Marshal
                    .PtrToStringUTF8((nint)extensions.Data, (int)extensions.Length)!
                    .Split('\n', StringSplitOptions.RemoveEmptyEntries);
                _vulkan = GraphiteVulkanWindow.FromQt(
                    descriptor.VulkanInstance,
                    descriptor.VulkanSurface,
                    names,
                    descriptor.VulkanInstanceApiVersion
                );
                SoftwareVulkan = _vulkan.IsSoftwareDevice;
                _vulkan.ResourcesReleasing += () => GpuResourcesReleasing?.Invoke();
                _contextIdentity = descriptor.ContextIdentity;
                _surfaceGeneration = descriptor.SurfaceGeneration;
            }
            return _vulkan.Render(
                descriptor.PixelWidth,
                descriptor.PixelHeight,
                render,
                shouldPresent,
                beforePresent
            );
        }
        if (RequiresRecreate(descriptor))
        {
            CreateSurface(descriptor);
        }
        // The native host clears the Qt-bound FBO before this callback. Forget
        // Skia's cached GL state so it cannot assume state left by the previous
        // frame (notably scissor and color-write masks).
        _context!.ResetContext();
        render(_surface!, descriptor.PixelWidth, descriptor.PixelHeight);
        _surface!.Canvas.Flush();
        _context!.Flush(_surface);
        _context.Submit(false);
        return true;
    }

    internal void Release(ulong surfaceGeneration, ulong contextIdentity)
    {
        if (_disposed)
        {
            return;
        }

        if (_surfaceGeneration != surfaceGeneration || _contextIdentity != contextIdentity)
        {
            return;
        }

        ReleaseGpuResources();
    }

    // Qt invokes this on the render owner even when no new frame is requested.
    internal bool PollGpuWork() => _vulkan?.PollGpuWork() ?? true;

    internal void SetQpaPlatform(string platform) =>
        _usePlatformGlResolver = string.Equals(platform, "xcb", StringComparison.OrdinalIgnoreCase);

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        ReleaseGpuResources();
    }

    private bool RequiresRecreate(in QtNativeV2.Surface descriptor) =>
        _surface is null
        || _surfaceGeneration != descriptor.SurfaceGeneration
        || _contextIdentity != descriptor.ContextIdentity
        || _framebufferObject != descriptor.FramebufferObject
        || _pixelWidth != descriptor.PixelWidth
        || _pixelHeight != descriptor.PixelHeight
        || _sampleCount != Math.Max(0, descriptor.SampleCount)
        || _stencilBits != Math.Max(0, descriptor.StencilBits)
        || _colorFormat != descriptor.ColorFormat;

    private void CreateSurface(in QtNativeV2.Surface descriptor)
    {
        var contextChanged = _context is null || _contextIdentity != descriptor.ContextIdentity;
        if (contextChanged)
        {
            ReleaseGpuResources();
            // Qt's GLX resolver returned thunk addresses that SkiaSharp 4.151.1 could not
            // safely assemble on this Mesa/xcb path. libGL's current-context resolver
            // is correct for GLX; Wayland/EGL continues to use Qt's resolver.
            _interface = _usePlatformGlResolver
                ? GRGlInterface.Create()
                : GRGlInterface.Create(_getProcedureAddress);
            if (_interface is null || !_interface.Validate())
            {
                throw new InvalidOperationException(
                    "SkiaSharp could not create a valid interface for the current Qt OpenGL context."
                );
            }

            _context =
                GRContext.CreateGl(_interface)
                ?? throw new InvalidOperationException(
                    "SkiaSharp could not create a GPU context for the current Qt OpenGL context."
                );
        }
        else
        {
            ReleaseRenderTarget();
        }
        var framebuffer = new GRGlFramebufferInfo(
            descriptor.FramebufferObject,
            descriptor.ColorFormat
        );
        _target = new GRBackendRenderTarget(
            descriptor.PixelWidth,
            descriptor.PixelHeight,
            Math.Max(0, descriptor.SampleCount),
            Math.Max(0, descriptor.StencilBits),
            framebuffer
        );
        _surface =
            SKSurface.Create(_context, _target, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888)
            ?? throw new InvalidOperationException(
                "SkiaSharp could not wrap the Qt default framebuffer object."
            );
        _surfaceGeneration = descriptor.SurfaceGeneration;
        _contextIdentity = descriptor.ContextIdentity;
        _framebufferObject = descriptor.FramebufferObject;
        _pixelWidth = descriptor.PixelWidth;
        _pixelHeight = descriptor.PixelHeight;
        _sampleCount = Math.Max(0, descriptor.SampleCount);
        _stencilBits = Math.Max(0, descriptor.StencilBits);
        _colorFormat = descriptor.ColorFormat;
    }

    private void ReleaseGpuResources()
    {
        Exception? quickError = null;
        if (QuickGpu is { } quick)
        {
            QuickPeakReservedBytes = Math.Max(QuickPeakReservedBytes, quick.PeakReservedBytes);
            QuickPeakRetiringLayers = Math.Max(QuickPeakRetiringLayers, quick.PeakRetiringLayers);
            QuickTimings = quick.Timings;
            try { quick.Dispose(); }
            catch (Exception error) { quickError = error; }
        }
        QuickGpu = null;
        QuickParts = [];
        _vulkan?.Dispose();
        _vulkan = null;
        ReleaseRenderTarget();
        _context?.Dispose();
        _context = null;
        _interface?.Dispose();
        _interface = null;
        _contextIdentity = 0;
        if (quickError is not null) throw quickError;
    }

    private void ReleaseRenderTarget()
    {
        _surface?.Dispose();
        _surface = null;
        _target?.Dispose();
        _target = null;
        _surfaceGeneration = 0;
        _framebufferObject = 0;
        _pixelWidth = 0;
        _pixelHeight = 0;
        _sampleCount = 0;
        _stencilBits = 0;
        _colorFormat = 0;
    }

    private static void Validate(in QtNativeV2.Surface descriptor)
    {
        if (descriptor.AbiVersion != QtNativeV2.AbiVersion || descriptor.StructSize < 144)
        {
            throw new InvalidDataException(
                "The Qt surface descriptor does not match doroti.qt-host/v2."
            );
        }

        if (
            descriptor.ContextIdentity == 0
            || descriptor.PixelWidth <= 0
            || descriptor.PixelHeight <= 0
        )
        {
            throw new InvalidDataException(
                "The Qt surface descriptor is missing a current context or physical size."
            );
        }
    }
}
