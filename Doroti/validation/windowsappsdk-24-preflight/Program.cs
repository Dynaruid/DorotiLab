using System.Numerics;
using System.Runtime.InteropServices;
using Microsoft.UI;
using Microsoft.UI.Content;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;

internal static partial class Program
{
    private const uint WsOverlappedWindow = 0x00CF0000;

    [STAThread]
    private static int Main()
    {
        Console.Error.WriteLine("preflight-stage=process-entry");
        Console.Error.Flush();
        var roResult = RoInitialize(0);
        if (roResult < 0)
            return Fail($"RoInitialize failed: 0x{roResult:X8}");
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
            Console.Error.WriteLine("preflight-stage=dispatcher-create");
            Console.Error.Flush();
            dispatcher = DispatcherQueueController.CreateOnCurrentThread();
            systemDispatcher = Windows.System.DispatcherQueueController.CreateOnDedicatedThread();
            Console.Error.WriteLine("preflight-stage=dispatcher-created");
            Console.Error.Flush();
            Console.Error.WriteLine("preflight-stage=hwnd-create");
            Console.Error.Flush();
            hwnd = CreateWindowExW(
                0, "STATIC", "Doroti Windows App SDK 2.4 preflight", WsOverlappedWindow,
                0, 0, 640, 480, 0, 0, 0, 0);
            if (hwnd == 0)
                return Fail($"CreateWindowExW failed: {Marshal.GetLastWin32Error()}");
            Console.Error.WriteLine("preflight-stage=hwnd-created");
            Console.Error.Flush();
            Console.Error.WriteLine("preflight-stage=compositor-create");
            Console.Error.Flush();
            using (var compositorReady = new ManualResetEventSlim())
            {
                if (!systemDispatcher.DispatcherQueue.TryEnqueue(() =>
                    {
                        compositor = new Windows.UI.Composition.Compositor();
                        compositorReady.Set();
                    }))
                    return Fail("system compositor dispatch was rejected");
                if (!compositorReady.Wait(TimeSpan.FromSeconds(5)))
                    return Fail("system compositor creation timed out");
            }
            Console.Error.WriteLine("preflight-stage=compositor-created");
            Console.Error.Flush();
            var systemCompositor = compositor ??
                throw new InvalidOperationException("system compositor was not created");
            var root = systemCompositor.CreateContainerVisual();
            Console.Error.WriteLine("preflight-stage=island-create");
            Console.Error.Flush();
            island = ContentIsland.CreateForSystemVisual(dispatcher.DispatcherQueue, root);
            island.RequestSize(new Vector2(640, 480));

            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.AssociateWithDispatcherQueue(dispatcher.DispatcherQueue);
            appWindow.Show();
            Console.Error.WriteLine("preflight-stage=site-create");
            Console.Error.Flush();
            siteBridge = DesktopAttachedSiteBridge.CreateFromWindowId(
                dispatcher.DispatcherQueue, windowId);
            siteBridge.ProcessesPointerInput = false;
            siteBridge.Connect(island);
            Console.Error.WriteLine("preflight-stage=site-connected");
            Console.Error.Flush();

            Console.WriteLine(
                $"windows-app-sdk-preflight package=2.4.0 hwnd=0x{hwnd:X} " +
                $"connected={island.IsConnected} siteClosed={siteBridge.IsClosed} " +
                $"childBridgeApi={typeof(DesktopChildSiteBridge).FullName is not null} " +
                $"pointerOwner={(siteBridge.ProcessesPointerInput ? "island" : "raw-hwnd")} " +
                $"keyboardOwner={(siteBridge.ProcessesKeyboardInput ? "island" : "raw-hwnd")}");
            return island.IsConnected && !siteBridge.IsClosed ? 0 : 2;
        }
        catch (Exception exception)
        {
            return Fail(exception.ToString());
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

    private static int Fail(string message)
    {
        Console.Error.WriteLine($"windows-app-sdk-preflight FAIL: {message}");
        return 1;
    }

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
