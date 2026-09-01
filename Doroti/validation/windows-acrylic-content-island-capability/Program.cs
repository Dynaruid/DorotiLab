using System.Numerics;
using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Content;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Vortice.Direct3D11;
using Windows.Graphics;
using Windows.Graphics.DirectX;
using Windows.UI.Composition;

namespace Doroti.Validation.WindowsAcrylicContentIslandCapability;

internal static partial class Program
{
    private const uint WsOverlappedWindow = 0x00CF0000;

    [STAThread]
    private static int Main(string[] args)
    {
        var reportPath = ResolveReportPath(args);
        var roResult = RoInitialize(0);
        if (roResult < 0) return Fail(reportPath, $"RoInitialize failed: 0x{roResult:X8}");
        nint window = 0;
        DispatcherQueueController? islandDispatcher = null;
        CompositionWorker? composition = null;
        ContentIsland? island = null;
        DesktopAttachedSiteBridge? siteBridge = null;
        AppWindow? appWindow = null;
        try
        {
            islandDispatcher = DispatcherQueueController.CreateOnCurrentThread();
            composition = new CompositionWorker();
            var root = composition.Invoke(() => composition.Compositor.CreateContainerVisual());
            window = CreateWindowExW(
                0, "STATIC", "Doroti Acrylic ContentIsland B0", WsOverlappedWindow,
                0, 0, 640, 480, 0, 0, 0, 0);
            if (window == 0)
                throw new InvalidOperationException($"CreateWindowExW failed: {Marshal.GetLastWin32Error()}.");
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(window);
            appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.AssociateWithDispatcherQueue(islandDispatcher.DispatcherQueue);
            island = ContentIsland.CreateForSystemVisual(islandDispatcher.DispatcherQueue, root);
            island.RequestSize(new Vector2(640, 480));
            siteBridge = DesktopAttachedSiteBridge.CreateFromWindowId(
                islandDispatcher.DispatcherQueue, windowId);
            siteBridge.ProcessesPointerInput = false;
            siteBridge.Connect(island);

            var backdrop = composition.RunBackdropCapability(island);
            var graphics = composition.RunAngleDirectImport();
            var pass = backdrop.IsSupported && backdrop.AddTargetResult &&
                       backdrop.ControllerCreateCount == 1 &&
                       backdrop.AddTargetCount == 1 &&
                       backdrop.RemoveTargetCount == 1 &&
                       backdrop.ConfigurationApplyCount == 4 &&
                       graphics.RequiredExtensionsPresent &&
                       graphics.D3D11DeviceQuerySucceeded &&
                       graphics.CompositionGraphicsDeviceCreated &&
                       graphics.BeginDrawCount == 1 &&
                       graphics.DirectImportSucceeded &&
                       graphics.MakeCurrentSucceeded &&
                       graphics.GpuClearCount == 1 &&
                       graphics.UnbindCount == 1 &&
                       graphics.EndDrawCount == 1 &&
                       graphics.EglError == 0x3000 &&
                       graphics.GlError == 0 &&
                       island.IsConnected && !siteBridge.IsClosed;
            var report = new CapabilityReport(
                "doroti.windows-acrylic-content-island-b0/v1",
                pass ? "PASS" : "FAIL",
                "Microsoft.WindowsAppSDK/2.4.0",
                Environment.OSVersion.VersionString,
                Environment.CurrentManagedThreadId,
                composition.ThreadId,
                island.IsConnected,
                siteBridge.IsClosed,
                siteBridge.ProcessesPointerInput,
                siteBridge.ProcessesKeyboardInput,
                backdrop,
                graphics,
                new EvidenceBoundary(
                    "PASS",
                    "notVerified",
                    "notVerified",
                    "notVerified",
                    "Capability and direct-import ownership proof only. It is not a B1 presenter, safe-retirement, visible, input, IME, accessibility, or physical acceptance."));
            WriteReport(reportPath, report);
            Console.WriteLine(
                $"B0 status={report.Status} backdropTarget={backdrop.AddTargetResult} " +
                $"directImport={graphics.DirectImportSucceeded} offset={graphics.OffsetX},{graphics.OffsetY} " +
                $"cpuReadback={graphics.CpuReadbackCount}");
            Console.WriteLine($"report={reportPath}");
            return pass ? 0 : 2;
        }
        catch (Exception exception)
        {
            return Fail(reportPath, exception.ToString());
        }
        finally
        {
            siteBridge?.Dispose();
            island?.Dispose();
            appWindow = null;
            composition?.Dispose();
            islandDispatcher?.ShutdownQueue();
            if (window != 0) DestroyWindow(window);
            RoUninitialize();
        }
    }

    private static string ResolveReportPath(string[] args)
    {
        var index = Array.IndexOf(args, "--report");
        return Path.GetFullPath(index >= 0 && index + 1 < args.Length
            ? args[index + 1]
            : Path.Combine(".doroti", "evidence", $"acrylic-content-island-b0-{DateTime.Now:yyyyMMdd-HHmmss}.json"));
    }

    private static int Fail(string reportPath, string message)
    {
        WriteReport(reportPath, new
        {
            schema = "doroti.windows-acrylic-content-island-b0/v1",
            status = "FAIL",
            exception = message,
            visibleStatus = "notVerified",
            physicalStatus = "notVerified",
        });
        Console.Error.WriteLine(message);
        Console.Error.WriteLine($"report={reportPath}");
        return 1;
    }

    private static void WriteReport(string path, object report)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, JsonSerializer.Serialize(report, JsonOptions));
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    [LibraryImport("user32.dll", EntryPoint = "CreateWindowExW", SetLastError = true,
        StringMarshalling = StringMarshalling.Utf16)]
    private static partial nint CreateWindowExW(
        uint extendedStyle, string className, string windowName, uint style,
        int x, int y, int width, int height, nint parent, nint menu, nint instance, nint parameter);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool DestroyWindow(nint window);

    [LibraryImport("combase.dll")]
    private static partial int RoInitialize(uint initType);

    [LibraryImport("combase.dll")]
    private static partial void RoUninitialize();
}

internal sealed class CompositionWorker : IDisposable
{
    private readonly Windows.System.DispatcherQueueController _dispatcher;
    private DesktopAcrylicController? _backdropController;
    private SystemBackdropConfiguration? _backdropConfiguration;
    private bool _disposed;

    internal CompositionWorker()
    {
        _dispatcher = Windows.System.DispatcherQueueController.CreateOnDedicatedThread();
        Invoke(() =>
        {
            ThreadId = Environment.CurrentManagedThreadId;
            Compositor = new Compositor();
        });
    }

    internal int ThreadId { get; private set; }
    internal Compositor Compositor { get; private set; } = null!;

    internal BackdropCapability RunBackdropCapability(ContentIsland island) => Invoke(() =>
    {
        var states = new List<string>();
        var supported = DesktopAcrylicController.IsSupported();
        _backdropController = new DesktopAcrylicController();
        _backdropController.StateChanged += (_, _) => states.Add(_backdropController.State.ToString());
        _backdropConfiguration = new SystemBackdropConfiguration
        {
            IsInputActive = true,
            Theme = SystemBackdropTheme.Dark,
        };
        var applied = new List<BackdropApplication>();
        Apply(DesktopAcrylicKind.Default, null, null, null, "system-default", applied);
        Apply(DesktopAcrylicKind.Base, Windows.UI.Color.FromArgb(255, 64, 96, 160), .35f, .70f,
            "base-custom", applied);
        Apply(DesktopAcrylicKind.Thin, Windows.UI.Color.FromArgb(255, 70, 40, 110), .55f, .45f,
            "thin-custom", applied);
        Apply(DesktopAcrylicKind.Default, null, null, null, "reset-default", applied);
        var added = _backdropController.AddSystemBackdropTarget(island);
        var stateAfterAdd = _backdropController.State.ToString();
        _backdropController.RemoveSystemBackdropTarget(island);
        return new BackdropCapability(
            supported,
            1,
            added,
            1,
            1,
            applied.Count,
            stateAfterAdd,
            states,
            applied);
    });

    internal AngleImportCapability RunAngleDirectImport() => Invoke(() =>
    {
        using var angle = new AngleDevice();
        using var graphicsDevice = CompositionInterop.CreateGraphicsDevice(
            Compositor, angle.D3D11DevicePointer) ??
            throw new InvalidOperationException("Composition graphics device creation returned null.");
        const bool compositionGraphicsDeviceCreated = true;
        using var surface = graphicsDevice.CreateDrawingSurface(
            new Windows.Foundation.Size(320, 180),
            DirectXPixelFormat.B8G8R8A8UIntNormalized,
            DirectXAlphaMode.Premultiplied);
        var draw = CompositionInterop.BeginDraw(surface);
        ID3D11Texture2D? texture = null;
        AngleImportResult import;
        try
        {
            texture = new ID3D11Texture2D(draw.TexturePointer);
            draw.TexturePointer = 0;
            var description = texture.Description;
            import = angle.ImportAndClear(texture.NativePointer, 320, 180, draw.Offset.X, draw.Offset.Y);
            import = import with
            {
                TextureWidth = checked((int)description.Width),
                TextureHeight = checked((int)description.Height),
                TextureFormat = description.Format.ToString(),
                TextureUsage = description.Usage.ToString(),
                TextureBindFlags = description.BindFlags.ToString(),
            };
        }
        finally
        {
            texture?.Dispose();
            if (draw.TexturePointer != 0) Marshal.Release(draw.TexturePointer);
            CompositionInterop.EndDraw(draw);
        }
        return new AngleImportCapability(
            angle.ClientExtensions,
            angle.DisplayExtensions,
            angle.HasD3DTextureClientBuffer,
            angle.HasD3DDeviceQuery,
            angle.HasDeviceQuery,
            angle.HasShareHandleSurface,
            angle.HasFenceSync,
            angle.RequiredExtensionsPresent,
            angle.D3D11DevicePointer != 0,
            compositionGraphicsDeviceCreated,
            1,
            draw.Offset.X,
            draw.Offset.Y,
            import.TextureWidth,
            import.TextureHeight,
            import.TextureFormat,
            import.TextureUsage,
            import.TextureBindFlags,
            import.DirectImportSucceeded,
            import.MakeCurrentSucceeded,
            import.GpuClearCount,
            import.UnbindCount,
            1,
            import.EglError,
            import.GlError,
            0,
            0,
            0,
            0,
            "BeginDraw owns transient texture -> EGL import/current -> GL flush -> EGL unbind/destroy -> texture release -> EndDraw");
    });

    private void Apply(
        DesktopAcrylicKind kind,
        Windows.UI.Color? tint,
        float? tintOpacity,
        float? luminosityOpacity,
        string name,
        List<BackdropApplication> applied)
    {
        var controller = _backdropController!;
        controller.ResetProperties();
        controller.Kind = kind;
        if (tint is Windows.UI.Color tintColor) controller.TintColor = tintColor;
        if (tintOpacity is float tintValue) controller.TintOpacity = tintValue;
        if (luminosityOpacity is float luminosityValue) controller.LuminosityOpacity = luminosityValue;
        controller.SetSystemBackdropConfiguration(_backdropConfiguration!);
        applied.Add(new BackdropApplication(
            name,
            controller.Kind.ToString(),
            ((uint)controller.TintColor.R << 16) |
            ((uint)controller.TintColor.G << 8) |
            controller.TintColor.B,
            controller.TintOpacity,
            controller.LuminosityOpacity,
            _backdropConfiguration!.Theme.ToString()));
    }

    internal T Invoke<T>(Func<T> callback)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (ThreadId != 0 && Environment.CurrentManagedThreadId == ThreadId) return callback();
        using var completed = new ManualResetEventSlim();
        Exception? failure = null;
        T? result = default;
        if (!_dispatcher.DispatcherQueue.TryEnqueue(() =>
            {
                try { result = callback(); }
                catch (Exception exception) { failure = exception; }
                finally { completed.Set(); }
            }))
            throw new InvalidOperationException("Composition DispatcherQueue rejected work.");
        if (!completed.Wait(TimeSpan.FromSeconds(15)))
            throw new TimeoutException("Composition DispatcherQueue work timed out.");
        if (failure is not null)
            throw new InvalidOperationException("Composition DispatcherQueue work failed.", failure);
        return result!;
    }

    internal void Invoke(Action callback) => Invoke(() =>
    {
        callback();
        return true;
    });

    public void Dispose()
    {
        if (_disposed) return;
        Invoke(() =>
        {
            _backdropController?.Dispose();
            _backdropController = null;
            _backdropConfiguration = null;
            Compositor.Dispose();
        });
        _disposed = true;
        _dispatcher.ShutdownQueueAsync().AsTask().GetAwaiter().GetResult();
    }
}

internal sealed class AngleDevice : IDisposable
{
    private const string AngleLibrary = "av_libglesv2.dll";
    private const int EglFalse = 0;
    private const int EglNone = 0x3038;
    private const int EglExtensions = 0x3055;
    private const uint EglPlatformAngle = 0x3202;
    private const int EglPlatformAngleType = 0x3203;
    private const int EglPlatformAngleTypeD3D11 = 0x3208;
    private const int EglPlatformAngleDeviceType = 0x3209;
    private const int EglPlatformAngleDeviceTypeHardware = 0x320A;
    private const int EglSurfaceType = 0x3033;
    private const int EglPbufferBit = 0x0001;
    private const int EglRenderableType = 0x3040;
    private const int EglOpenGles2Bit = 0x0004;
    private const int EglRedSize = 0x3024;
    private const int EglGreenSize = 0x3023;
    private const int EglBlueSize = 0x3022;
    private const int EglAlphaSize = 0x3021;
    private const int EglDepthSize = 0x3025;
    private const int EglStencilSize = 0x3026;
    private const int EglContextClientVersion = 0x3098;
    private const int EglOpenGlesApi = 0x30A0;
    private const int EglDeviceExt = 0x322C;
    private const int EglD3D11DeviceAngle = 0x33A1;
    private const int EglD3DTextureAngle = 0x33A3;
    private const int EglWidth = 0x3057;
    private const int EglHeight = 0x3056;
    private const int EglTextureOffsetXAngle = 0x3490;
    private const int EglTextureOffsetYAngle = 0x3491;
    private const uint GlColorBufferBit = 0x00004000;
    private nint _display;
    private nint _config;
    private nint _context;
    private bool _disposed;

    internal AngleDevice()
    {
        var platformAttributes = new[]
        {
            EglPlatformAngleType, EglPlatformAngleTypeD3D11,
            EglPlatformAngleDeviceType, EglPlatformAngleDeviceTypeHardware,
            EglNone,
        };
        _display = EglGetPlatformDisplayExt(EglPlatformAngle, 0, platformAttributes);
        if (_display == 0 || EglInitialize(_display, out _, out _) == EglFalse)
            ThrowEgl("ANGLE D3D11 display initialization");
        if (EglBindApi(EglOpenGlesApi) == EglFalse) ThrowEgl("eglBindAPI");
        ClientExtensions = Marshal.PtrToStringAnsi(EglQueryString(0, EglExtensions)) ?? string.Empty;
        DisplayExtensions = Marshal.PtrToStringAnsi(EglQueryString(_display, EglExtensions)) ?? string.Empty;
        HasD3DTextureClientBuffer = HasExtension("EGL_ANGLE_d3d_texture_client_buffer");
        HasDeviceQuery = HasExtension("EGL_EXT_device_query");
        HasShareHandleSurface = HasExtension("EGL_ANGLE_surface_d3d_texture_2d_share_handle");
        HasFenceSync = HasExtension("EGL_KHR_fence_sync");
        var queryDisplayPointer = EglGetProcAddress("eglQueryDisplayAttribEXT");
        var queryDevicePointer = EglGetProcAddress("eglQueryDeviceAttribEXT");
        if (queryDisplayPointer == 0 || queryDevicePointer == 0)
            throw new InvalidOperationException("ANGLE device-query entrypoints are unavailable.");
        var queryDisplay = Marshal.GetDelegateForFunctionPointer<QueryDisplayAttrib>(queryDisplayPointer);
        var queryDevice = Marshal.GetDelegateForFunctionPointer<QueryDeviceAttrib>(queryDevicePointer);
        if (queryDisplay(_display, EglDeviceExt, out var eglDevice) == EglFalse || eglDevice == 0)
            ThrowEgl("eglQueryDisplayAttribEXT(EGL_DEVICE_EXT)");
        if (queryDevice(eglDevice, EglD3D11DeviceAngle, out var d3dDevice) == EglFalse || d3dDevice == 0)
            ThrowEgl("eglQueryDeviceAttribEXT(EGL_D3D11_DEVICE_ANGLE)");
        HasD3DDeviceQuery = true;
        RequiredExtensionsPresent = HasD3DTextureClientBuffer && HasDeviceQuery && HasD3DDeviceQuery;
        D3D11DevicePointer = d3dDevice;
        var configAttributes = new[]
        {
            EglSurfaceType, EglPbufferBit,
            EglRenderableType, EglOpenGles2Bit,
            EglRedSize, 8, EglGreenSize, 8, EglBlueSize, 8, EglAlphaSize, 8,
            EglDepthSize, 0, EglStencilSize, 0, EglNone,
        };
        if (EglChooseConfig(_display, configAttributes, out _config, 1, out var count) == EglFalse ||
            count != 1 || _config == 0)
            ThrowEgl("eglChooseConfig(pbuffer RGBA8)");
        _context = EglCreateContext(
            _display, _config, 0, [EglContextClientVersion, 2, EglNone]);
        if (_context == 0) ThrowEgl("eglCreateContext");
    }

    internal string ClientExtensions { get; }
    internal string DisplayExtensions { get; }
    internal bool HasD3DTextureClientBuffer { get; }
    internal bool HasD3DDeviceQuery { get; }
    internal bool HasDeviceQuery { get; }
    internal bool HasShareHandleSurface { get; }
    internal bool HasFenceSync { get; }
    internal bool RequiredExtensionsPresent { get; }
    internal nint D3D11DevicePointer { get; }

    internal AngleImportResult ImportAndClear(
        nint texture,
        int width,
        int height,
        int offsetX,
        int offsetY,
        bool drawAlphaGrid = false,
        int generation = 0,
        float markerScale = 1f)
    {
        var attributes = new[]
        {
            EglWidth, width,
            EglHeight, height,
            EglTextureOffsetXAngle, offsetX,
            EglTextureOffsetYAngle, offsetY,
            EglNone,
        };
        var surface = EglCreatePbufferFromClientBuffer(
            _display, EglD3DTextureAngle, texture, _config, attributes);
        if (surface == 0) ThrowEgl("eglCreatePbufferFromClientBuffer(EGL_D3D_TEXTURE_ANGLE)");
        var madeCurrent = false;
        var unbindCount = 0;
        uint glError;
        try
        {
            if (EglMakeCurrent(_display, surface, surface, _context) == EglFalse)
                ThrowEgl("eglMakeCurrent(imported texture)");
            madeCurrent = true;
            GlViewport(0, 0, width, height);
            GlClearColor(drawAlphaGrid ? 0 : .12f, drawAlphaGrid ? 0 : .42f,
                drawAlphaGrid ? 0 : .78f, drawAlphaGrid ? 0 : .5f);
            GlClear(GlColorBufferBit);
            if (drawAlphaGrid)
            {
                GlEnable(0x0C11); // GL_SCISSOR_TEST
                ClearRegion(0, height / 2, width / 2, height - height / 2, .25f, 0, 0, .25f);
                ClearRegion(width / 2, height / 2, width - width / 2, height - height / 2,
                    0, .50f, 0, .50f);
                ClearRegion(0, 0, width / 2, height / 2, 0, 0, .80f, .80f);
                ClearRegion(width / 2, 0, width - width / 2, height / 2, 1, 1, 1, 1);
                ClearRegion(Math.Max(0, width / 2 - 12), 0, Math.Min(24, width), height,
                    0, 0, 0, 0);
                var markerWidth = Math.Clamp(8 + generation % 53, 8, Math.Max(8, width / 3));
                ClearRegion(0, 0, markerWidth, Math.Min(8, height), 1, 0, 1, 1);
                markerScale = Math.Max(1f, markerScale);
                var bitSize = Math.Max(4, (int)MathF.Round(7 * markerScale));
                var bitGap = Math.Max(1, (int)MathF.Round(markerScale));
                const int bitCount = 12;
                var stripWidth = bitCount * bitSize + (bitCount - 1) * bitGap;
                var startX = width - stripWidth - Math.Max(4, (int)MathF.Round(4 * markerScale));
                var startY = Math.Max(1, (int)MathF.Round(5 * markerScale));
                var gray = generation ^ (generation >> 1);
                for (var bit = 0; bit < bitCount; bit++)
                {
                    var value = (gray & (1 << bit)) != 0 ? 1f : 0f;
                    ClearRegion(
                        startX + bit * (bitSize + bitGap), startY,
                        bitSize, bitSize, value, value, value, 1);
                }
                GlDisable(0x0C11);
            }
            GlFlush();
            glError = GlGetError();
            if (EglMakeCurrent(_display, 0, 0, 0) == EglFalse)
                ThrowEgl("eglMakeCurrent(unbind)");
            unbindCount++;
        }
        finally
        {
            if (madeCurrent && unbindCount == 0) EglMakeCurrent(_display, 0, 0, 0);
            EglDestroySurface(_display, surface);
        }
        return new AngleImportResult(
            true, madeCurrent, 1, unbindCount, EglGetError(), glError,
            0, 0, string.Empty, string.Empty, string.Empty);
    }

    private bool HasExtension(string name) => string.Concat(ClientExtensions, " ", DisplayExtensions)
        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
        .Contains(name, StringComparer.Ordinal);

    private static void ClearRegion(
        int x, int y, int width, int height,
        float red, float green, float blue, float alpha)
    {
        GlScissor(x, y, width, height);
        GlClearColor(red, green, blue, alpha);
        GlClear(GlColorBufferBit);
    }

    private static void ThrowEgl(string operation) =>
        throw new InvalidOperationException($"{operation} failed with EGL error 0x{EglGetError():X4}.");

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_display != 0) EglMakeCurrent(_display, 0, 0, 0);
        if (_context != 0) EglDestroyContext(_display, _context);
        if (_display != 0) EglTerminate(_display);
        _context = _display = _config = 0;
    }

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate int QueryDisplayAttrib(nint display, int attribute, out nint value);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate int QueryDeviceAttrib(nint device, int attribute, out nint value);

    [DllImport(AngleLibrary, EntryPoint = "EGL_GetPlatformDisplayEXT", ExactSpelling = true)]
    private static extern nint EglGetPlatformDisplayExt(uint platform, nint nativeDisplay, int[] attributes);

    [DllImport(AngleLibrary, EntryPoint = "EGL_Initialize", ExactSpelling = true)]
    private static extern int EglInitialize(nint display, out int major, out int minor);

    [DllImport(AngleLibrary, EntryPoint = "EGL_BindAPI", ExactSpelling = true)]
    private static extern int EglBindApi(int api);

    [DllImport(AngleLibrary, EntryPoint = "EGL_QueryString", ExactSpelling = true)]
    private static extern nint EglQueryString(nint display, int name);

    [DllImport(AngleLibrary, EntryPoint = "EGL_ChooseConfig", ExactSpelling = true)]
    private static extern int EglChooseConfig(
        nint display, int[] attributes, out nint config, int configSize, out int count);

    [DllImport(AngleLibrary, EntryPoint = "EGL_CreateContext", ExactSpelling = true)]
    private static extern nint EglCreateContext(
        nint display, nint config, nint sharedContext, int[] attributes);

    [DllImport(AngleLibrary, EntryPoint = "EGL_CreatePbufferFromClientBuffer", ExactSpelling = true)]
    private static extern nint EglCreatePbufferFromClientBuffer(
        nint display, int bufferType, nint buffer, nint config, int[] attributes);

    [DllImport(AngleLibrary, EntryPoint = "EGL_MakeCurrent", ExactSpelling = true)]
    private static extern int EglMakeCurrent(
        nint display, nint drawSurface, nint readSurface, nint context);

    [DllImport(AngleLibrary, EntryPoint = "EGL_DestroySurface", ExactSpelling = true)]
    private static extern int EglDestroySurface(nint display, nint surface);

    [DllImport(AngleLibrary, EntryPoint = "EGL_DestroyContext", ExactSpelling = true)]
    private static extern int EglDestroyContext(nint display, nint context);

    [DllImport(AngleLibrary, EntryPoint = "EGL_Terminate", ExactSpelling = true)]
    private static extern int EglTerminate(nint display);

    [DllImport(AngleLibrary, EntryPoint = "EGL_GetProcAddress", ExactSpelling = true,
        CharSet = CharSet.Ansi)]
    private static extern nint EglGetProcAddress(string name);

    [DllImport(AngleLibrary, EntryPoint = "EGL_GetError", ExactSpelling = true)]
    private static extern int EglGetError();

    [DllImport(AngleLibrary, EntryPoint = "glViewport", ExactSpelling = true)]
    private static extern void GlViewport(int x, int y, int width, int height);

    [DllImport(AngleLibrary, EntryPoint = "glClearColor", ExactSpelling = true)]
    private static extern void GlClearColor(float red, float green, float blue, float alpha);

    [DllImport(AngleLibrary, EntryPoint = "glClear", ExactSpelling = true)]
    private static extern void GlClear(uint mask);

    [DllImport(AngleLibrary, EntryPoint = "glEnable", ExactSpelling = true)]
    private static extern void GlEnable(uint capability);

    [DllImport(AngleLibrary, EntryPoint = "glDisable", ExactSpelling = true)]
    private static extern void GlDisable(uint capability);

    [DllImport(AngleLibrary, EntryPoint = "glScissor", ExactSpelling = true)]
    private static extern void GlScissor(int x, int y, int width, int height);

    [DllImport(AngleLibrary, EntryPoint = "glFlush", ExactSpelling = true)]
    private static extern void GlFlush();

    [DllImport(AngleLibrary, EntryPoint = "glGetError", ExactSpelling = true)]
    private static extern uint GlGetError();
}

internal static class CompositionInterop
{
    private static readonly Guid CompositorInteropIid =
        new("25297D5C-3AD4-4C9C-B5CF-E36A38512330");
    private static readonly Guid DrawingSurfaceInteropIid =
        new("FD04E6E3-FE0C-4C3C-AB19-A07601A576EE");
    private static readonly Guid Texture2DIid =
        new("6F15AAF2-D208-4E89-9AB4-489535D34F9C");

    internal static unsafe CompositionGraphicsDevice CreateGraphicsDevice(
        Compositor compositor,
        nint d3d11Device)
    {
        using var interop = ((WinRT.IWinRTObject)compositor).NativeObject.As(CompositorInteropIid);
        var thisPointer = interop.ThisPtr;
        var vtable = *(nint**)thisPointer;
        var create = (delegate* unmanaged[Stdcall]<nint, nint, nint*, int>)vtable[5];
        nint result = 0;
        Check(create(thisPointer, d3d11Device, &result));
        try { return WinRT.MarshalInterface<CompositionGraphicsDevice>.FromAbi(result); }
        finally { Marshal.Release(result); }
    }

    internal static unsafe ActiveDraw BeginDraw(CompositionDrawingSurface surface)
    {
        var interop = ((WinRT.IWinRTObject)surface).NativeObject.As(DrawingSurfaceInteropIid);
        var thisPointer = interop.ThisPtr;
        var vtable = *(nint**)thisPointer;
        var begin = (delegate* unmanaged[Stdcall]<nint, void*, Guid*, nint*, NativePoint*, int>)vtable[3];
        var iid = Texture2DIid;
        nint texture = 0;
        NativePoint offset = default;
        Check(begin(thisPointer, null, &iid, &texture, &offset));
        return new ActiveDraw(interop, texture, offset);
    }

    internal static unsafe void EndDraw(ActiveDraw draw)
    {
        if (draw.Ended) return;
        var thisPointer = draw.Interop.ThisPtr;
        var vtable = *(nint**)thisPointer;
        var end = (delegate* unmanaged[Stdcall]<nint, int>)vtable[4];
        Check(end(thisPointer));
        draw.Ended = true;
        draw.Interop.Dispose();
    }

    private static void Check(int hresult)
    {
        if (hresult < 0) Marshal.ThrowExceptionForHR(hresult);
    }
}

internal sealed class ActiveDraw(WinRT.IObjectReference interop, nint texturePointer, NativePoint offset)
{
    internal WinRT.IObjectReference Interop { get; } = interop;
    internal nint TexturePointer { get; set; } = texturePointer;
    internal NativePoint Offset { get; } = offset;
    internal bool Ended { get; set; }
}

[StructLayout(LayoutKind.Sequential)]
internal readonly record struct NativePoint(int X, int Y);

internal sealed record BackdropApplication(
    string Name, string Kind, uint TintRgb, float TintOpacity,
    float LuminosityOpacity, string Theme);
internal sealed record BackdropCapability(
    bool IsSupported,
    int ControllerCreateCount,
    bool AddTargetResult,
    int AddTargetCount,
    int RemoveTargetCount,
    int ConfigurationApplyCount,
    string StateAfterAdd,
    IReadOnlyList<string> StateTransitions,
    IReadOnlyList<BackdropApplication> Applications);
internal sealed record AngleImportResult(
    bool DirectImportSucceeded,
    bool MakeCurrentSucceeded,
    int GpuClearCount,
    int UnbindCount,
    int EglError,
    uint GlError,
    int TextureWidth,
    int TextureHeight,
    string TextureFormat,
    string TextureUsage,
    string TextureBindFlags);
internal sealed record AngleImportCapability(
    string ClientExtensions,
    string DisplayExtensions,
    bool HasD3DTextureClientBuffer,
    bool HasD3DDeviceQuery,
    bool HasDeviceQuery,
    bool HasShareHandleSurface,
    bool HasFenceSync,
    bool RequiredExtensionsPresent,
    bool D3D11DeviceQuerySucceeded,
    bool CompositionGraphicsDeviceCreated,
    int BeginDrawCount,
    int OffsetX,
    int OffsetY,
    int TextureWidth,
    int TextureHeight,
    string TextureFormat,
    string TextureUsage,
    string TextureBindFlags,
    bool DirectImportSucceeded,
    bool MakeCurrentSucceeded,
    int GpuClearCount,
    int UnbindCount,
    int EndDrawCount,
    int EglError,
    uint GlError,
    int CpuReadbackCount,
    int StagingMapCount,
    int GdiCopyCount,
    int BitmapUploadCount,
    string OwnershipHandoff);
internal sealed record EvidenceBoundary(
    string AutomatedCapability,
    string DeviceLoss,
    string Visible,
    string Physical,
    string Scope);
internal sealed record CapabilityReport(
    string Schema,
    string Status,
    string WindowsAppSdk,
    string OperatingSystem,
    int IslandThreadId,
    int CompositionThreadId,
    bool IslandConnected,
    bool SiteBridgeClosed,
    bool BridgeProcessesPointerInput,
    bool BridgeProcessesKeyboardInput,
    BackdropCapability Backdrop,
    AngleImportCapability Graphics,
    EvidenceBoundary Evidence);
