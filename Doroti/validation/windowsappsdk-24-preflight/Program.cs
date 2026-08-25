using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.UI;
using Microsoft.UI.Content;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;

internal static partial class Program
{
    private const uint WsOverlappedWindow = 0x00CF0000;
    private static readonly Guid SystemCompositorInteropIid =
        new("25297D5C-3AD4-4C9C-B5CF-E36A38512330");

    [STAThread]
    private static int Main(string[] args)
    {
        var reportPath = GetOption(args, "--report");
        Console.Error.WriteLine("preflight-stage=process-entry");
        Console.Error.Flush();
        var roResult = RoInitialize(0);
        if (roResult < 0)
            return Fail($"RoInitialize failed: 0x{roResult:X8}", reportPath);
        Console.Error.WriteLine($"preflight-stage=ro-initialized hr=0x{roResult:X8}");
        Console.Error.Flush();
        nint hwnd = 0;
        DispatcherQueueController? dispatcher = null;
        Windows.System.DispatcherQueueController? systemDispatcher = null;
        Windows.UI.Composition.Compositor? compositor = null;
        ContentIsland? island = null;
        DesktopAttachedSiteBridge? siteBridge = null;
        try
        {
            var nativeEntrypoints = ProbeNativeEntrypoints();
            Console.Error.WriteLine("preflight-stage=dispatcher-create");
            Console.Error.Flush();
            dispatcher = DispatcherQueueController.CreateOnCurrentThread();
            systemDispatcher = Windows.System.DispatcherQueueController.CreateOnDedicatedThread();
            Console.Error.WriteLine("preflight-stage=dispatcher-created");
            Console.Error.Flush();
            Console.Error.WriteLine("preflight-stage=hwnd-create");
            Console.Error.Flush();
            hwnd = CreateWindowExW(
                0, "STATIC", "Doroti Windows App SDK 2.4 W0 preflight", WsOverlappedWindow,
                0, 0, 640, 480, 0, 0, 0, 0);
            if (hwnd == 0)
                return Fail($"CreateWindowExW failed: {Marshal.GetLastWin32Error()}", reportPath);
            Console.Error.WriteLine("preflight-stage=hwnd-created");
            Console.Error.Flush();
            Console.Error.WriteLine("preflight-stage=compositor-create");
            Console.Error.Flush();
            using (var compositorReady = new ManualResetEventSlim())
            {
                Exception? compositorFailure = null;
                if (!systemDispatcher.DispatcherQueue.TryEnqueue(() =>
                    {
                        try
                        {
                            compositor = new Windows.UI.Composition.Compositor();
                        }
                        catch (Exception exception)
                        {
                            compositorFailure = exception;
                        }
                        finally
                        {
                            compositorReady.Set();
                        }
                    }))
                    return Fail("system compositor dispatch was rejected", reportPath);
                if (!compositorReady.Wait(TimeSpan.FromSeconds(5)))
                    return Fail("system compositor creation timed out", reportPath);
                if (compositorFailure is not null)
                    throw new InvalidOperationException("system compositor activation failed", compositorFailure);
            }
            Console.Error.WriteLine("preflight-stage=compositor-created");
            Console.Error.Flush();
            var systemCompositor = compositor ??
                throw new InvalidOperationException("system compositor was not created");
            var root = systemCompositor.CreateContainerVisual();
            Console.Error.WriteLine("preflight-stage=island-create");
            Console.Error.Flush();
            island = ContentIsland.CreateForSystemVisual(dispatcher.DispatcherQueue, root);
            var automationRequestCount = 0;
            island.RequestSize(new Vector2(640, 480));

            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.AssociateWithDispatcherQueue(dispatcher.DispatcherQueue);
            Console.Error.WriteLine("preflight-stage=site-create");
            Console.Error.Flush();
            siteBridge = DesktopAttachedSiteBridge.CreateFromWindowId(
                dispatcher.DispatcherQueue, windowId);
            siteBridge.ProcessesPointerInput = false;
            siteBridge.Connect(island);
            var keyboardDisableAttempt = "notAttempted";
            string? keyboardDisableFailure = null;
            try
            {
                siteBridge.ProcessesKeyboardInput = false;
                keyboardDisableAttempt = siteBridge.ProcessesKeyboardInput ? "ineffective" : "disabled";
            }
            catch (Exception exception)
            {
                keyboardDisableAttempt = "rejected";
                keyboardDisableFailure = exception is COMException comException
                    ? $"{exception.GetType().FullName} (0x{comException.HResult:X8}): {exception.Message}"
                    : $"{exception.GetType().FullName}: {exception.Message}";
            }
            Console.Error.WriteLine("preflight-stage=site-connected");
            Console.Error.Flush();

            island.AutomationProviderRequested += (_, _) => automationRequestCount++;
            using var compositorInterop =
                ((WinRT.IWinRTObject)systemCompositor).NativeObject.As(SystemCompositorInteropIid);
            var siteView = siteBridge.SiteView;
            var pointerSource = InputPointerSource.GetForIsland(island);
            var keyboardSource = InputKeyboardSource.GetForIsland(island);
            var focusController = InputFocusController.GetForIsland(island);
            var projectionAssembly = typeof(ContentIsland).Assembly;
            var contentAppWindowBridge = projectionAssembly.GetType(
                "Microsoft.UI.Content.ContentAppWindowBridge", throwOnError: false);
            var requiredApis = ProbeRequiredApis();
            var requiredAbsentApis = ProbeRequiredAbsentApis();
            var report = new
            {
                schema = "doroti.winrt-composition-w0-runtime/v3",
                status = requiredApis.All(api => api.Available) &&
                    requiredAbsentApis.All(api => !api.Available) &&
                    nativeEntrypoints.All(entrypoint => entrypoint.Available) &&
                    contentAppWindowBridge is null && island.IsConnected && !siteBridge.IsClosed
                        ? "PASS"
                        : "FAIL",
                package = "Microsoft.WindowsAppSDK/2.4.0",
                process = new
                {
                    executable = Environment.ProcessPath,
                    processArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
                    osDescription = RuntimeInformation.OSDescription,
                    osVersion = Environment.OSVersion.VersionString,
                    framework = RuntimeInformation.FrameworkDescription,
                },
                projection = DescribeAssembly(projectionAssembly),
                runtime = DescribeAssembly(typeof(AppWindow).Assembly),
                boundary = new
                {
                    model = "attached HWND shim + ContentIsland",
                    pureWinRt = false,
                    childRenderHwndCount = 0,
                    compositorInteropAvailable = compositorInterop.ThisPtr != 0,
                    contentAppWindowBridgeAvailable = contentAppWindowBridge is not null,
                },
                activation = new
                {
                    hwnd = $"0x{hwnd:X}",
                    windowId = windowId.Value,
                    islandConnected = island.IsConnected,
                    siteBridgeClosed = siteBridge.IsClosed,
                    pointerOwner = siteBridge.ProcessesPointerInput ? "ContentIsland" : "raw-hwnd",
                    keyboardOwner = siteBridge.ProcessesKeyboardInput ? "ContentIsland" : "raw-hwnd",
                    keyboardDisableAttempt,
                    keyboardDisableFailure,
                    pointerSource = pointerSource.GetType().FullName,
                    keyboardSource = keyboardSource.GetType().FullName,
                    focusController = focusController.GetType().FullName,
                    automationProviderEventSubscribed = true,
                    automationRequestCount,
                },
                systemIslandInputOwnership = new
                {
                    bridgeProcessesPointerInput = siteBridge.ProcessesPointerInput,
                    bridgeProcessesKeyboardInput = siteBridge.ProcessesKeyboardInput,
                    rootIslandInputSourceRegistrationAllowed = false,
                    keyboardRuntimeFloor = "bridge-processing-enabled-without-root-input-source-registration",
                    inputActivationListenerRole = "observation-only",
                    packetOwner = "sole-top-level-hwnd-native-ingress",
                    nativeIngressCallsite = "WinRtTopLevelNativeIngress",
                },
                metrics = new
                {
                    clientSize = new { siteView.ClientSize.Width, siteView.ClientSize.Height },
                    actualSize = new { siteView.ActualSize.X, siteView.ActualSize.Y },
                    siteView.RasterizationScale,
                },
                requiredApis,
                requiredAbsentApis,
                nativeEntrypoints,
            };
            WriteReport(reportPath, report);
            Console.WriteLine(
                $"windows-app-sdk-preflight status={report.status} package=2.4.0 hwnd=0x{hwnd:X} " +
                $"connected={island.IsConnected} siteClosed={siteBridge.IsClosed} " +
                $"contentAppWindowBridge={contentAppWindowBridge is not null} " +
                $"pointerOwner={(siteBridge.ProcessesPointerInput ? "ContentIsland" : "raw-hwnd")} " +
                $"keyboardOwner={(siteBridge.ProcessesKeyboardInput ? "ContentIsland" : "raw-hwnd")}");
            return report.status == "PASS" ? 0 : 2;
        }
        catch (Exception exception)
        {
            return Fail(exception.ToString(), reportPath);
        }
        finally
        {
            siteBridge?.Dispose();
            island?.Dispose();
            compositor?.Dispose();
            systemDispatcher?.ShutdownQueueAsync();
            dispatcher?.ShutdownQueue();
            if (hwnd != 0)
                DestroyWindow(hwnd);
            RoUninitialize();
        }
    }

    private static ApiProbe[] ProbeRequiredApis()
    {
        var specs = new (Type Type, string Member, MemberTypes Kind)[]
        {
            (typeof(ContentIsland), nameof(ContentIsland.CreateForSystemVisual), MemberTypes.Method),
            (typeof(DesktopAttachedSiteBridge), nameof(DesktopAttachedSiteBridge.CreateFromWindowId), MemberTypes.Method),
            (typeof(ContentSiteView), nameof(ContentSiteView.ClientSize), MemberTypes.Property),
            (typeof(ContentSiteView), nameof(ContentSiteView.ActualSize), MemberTypes.Property),
            (typeof(ContentSiteView), nameof(ContentSiteView.RasterizationScale), MemberTypes.Property),
            (typeof(InputPointerSource), nameof(InputPointerSource.GetForIsland), MemberTypes.Method),
            (typeof(InputKeyboardSource), nameof(InputKeyboardSource.GetForIsland), MemberTypes.Method),
            (typeof(InputPreTranslateKeyboardSource), nameof(InputPreTranslateKeyboardSource.GetForIsland), MemberTypes.Method),
            (typeof(InputFocusController), nameof(InputFocusController.GetForIsland), MemberTypes.Method),
            (typeof(InputActivationListener), nameof(InputActivationListener.GetForWindowId), MemberTypes.Method),
            (typeof(ContentIsland), nameof(ContentIsland.AutomationProviderRequested), MemberTypes.Event),
        };
        return specs.Select(spec => new ApiProbe(
                $"{spec.Type.FullName}.{spec.Member}",
                spec.Type.GetMember(spec.Member, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
                    .Any(member => member.MemberType == spec.Kind)))
            .ToArray();
    }

    private static ApiProbe[] ProbeRequiredAbsentApis()
    {
        var specs = new (Type Type, string Member)[]
        {
            (typeof(InputPointerSource), "GetForWindowId"),
            (typeof(InputKeyboardSource), "GetForWindowId"),
            (typeof(InputPreTranslateKeyboardSource), "GetForWindowId"),
            (typeof(InputFocusController), "GetForWindowId"),
        };
        return specs.Select(spec => new ApiProbe(
                $"{spec.Type.FullName}.{spec.Member}",
                spec.Type.GetMember(spec.Member, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
                    .Any(member => member.MemberType == MemberTypes.Method)))
            .ToArray();
    }

    private static NativeProbe[] ProbeNativeEntrypoints()
    {
        var specs = new[]
        {
            new NativeSpec("user32.dll", "CreateWindowExW"),
            new NativeSpec("user32.dll", "DestroyWindow"),
            new NativeSpec("user32.dll", "GetClientRect"),
            new NativeSpec("user32.dll", "SetWindowPos"),
            new NativeSpec("user32.dll", "EnableMouseInPointer"),
            new NativeSpec("user32.dll", "GetPointerInfo"),
            new NativeSpec("user32.dll", "GetPointerInfoHistory"),
            new NativeSpec("user32.dll", "SetCapture"),
            new NativeSpec("user32.dll", "ReleaseCapture"),
            new NativeSpec("user32.dll", "GetCapture"),
            new NativeSpec("user32.dll", "ScreenToClient"),
            new NativeSpec("user32.dll", "GetKeyState"),
            new NativeSpec("user32.dll", "LoadCursorW"),
            new NativeSpec("user32.dll", "SetCursor"),
            new NativeSpec("imm32.dll", "ImmGetContext"),
            new NativeSpec("imm32.dll", "ImmReleaseContext"),
            new NativeSpec("UIAutomationCore.dll", "UiaReturnRawElementProvider"),
            new NativeSpec("combase.dll", "RoInitialize"),
            new NativeSpec("combase.dll", "RoUninitialize"),
            new NativeSpec("d3d11.dll", "D3D11CreateDevice"),
            new NativeSpec("dxgi.dll", "CreateDXGIFactory2"),
        };
        return specs.Select(spec =>
        {
            var loaded = NativeLibrary.TryLoad(spec.Library, out var handle);
            try
            {
                var available = loaded && NativeLibrary.TryGetExport(handle, spec.Export, out _);
                return new NativeProbe(spec.Library, spec.Export, available);
            }
            finally
            {
                if (loaded) NativeLibrary.Free(handle);
            }
        }).ToArray();
    }

    private static object DescribeAssembly(Assembly assembly)
    {
        var path = assembly.Location;
        var fileVersion = FileVersionInfo.GetVersionInfo(path);
        return new
        {
            assembly = assembly.GetName().Name,
            assemblyVersion = assembly.GetName().Version?.ToString(),
            fileVersion = fileVersion.FileVersion,
            productVersion = fileVersion.ProductVersion,
            path,
            sha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant(),
        };
    }

    private static string? GetOption(string[] args, string name)
    {
        var index = Array.IndexOf(args, name);
        return index >= 0 && index + 1 < args.Length ? Path.GetFullPath(args[index + 1]) : null;
    }

    private static void WriteReport(string? path, object report)
    {
        if (path is null) return;
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, JsonSerializer.Serialize(report, JsonOptions));
        Console.WriteLine($"report={path}");
    }

    private static int Fail(string message, string? reportPath)
    {
        Console.Error.WriteLine($"windows-app-sdk-preflight FAIL: {message}");
        WriteReport(reportPath, new
        {
            schema = "doroti.winrt-composition-w0-runtime/v3",
            status = "FAIL",
            exception = message,
        });
        return 1;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private sealed record ApiProbe(string Api, bool Available);
    private sealed record NativeSpec(string Library, string Export);
    private sealed record NativeProbe(string Library, string Export, bool Available);

    [LibraryImport("user32.dll", EntryPoint = "CreateWindowExW", SetLastError = true,
        StringMarshalling = StringMarshalling.Utf16)]
    private static partial nint CreateWindowExW(
        uint extendedStyle,
        string className,
        string windowName,
        uint style,
        int x,
        int y,
        int width,
        int height,
        nint parent,
        nint menu,
        nint instance,
        nint parameter);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool DestroyWindow(nint hwnd);

    [LibraryImport("combase.dll")]
    private static partial int RoInitialize(uint initType);

    [LibraryImport("combase.dll")]
    private static partial void RoUninitialize();
}
