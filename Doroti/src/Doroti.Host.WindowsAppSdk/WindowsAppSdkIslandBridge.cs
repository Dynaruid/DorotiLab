using System.Numerics;
using Microsoft.UI;
using Microsoft.UI.Content;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;

namespace Doroti.Host.WindowsAppSdk;

internal sealed class WindowsAppSdkIslandBridge : IDisposable
{
    private readonly DispatcherQueueController _dispatcherController;
    private readonly Windows.System.DispatcherQueueController _systemDispatcherController;
    private readonly Windows.UI.Composition.Compositor _systemCompositor;
    private readonly ContentIsland _island;
    private readonly DesktopAttachedSiteBridge _siteBridge;
    private bool _disposed;

    internal WindowsAppSdkIslandBridge(nint hwnd, int width, int height)
    {
        if (hwnd == 0) throw new ArgumentOutOfRangeException(nameof(hwnd));
        _dispatcherController = DispatcherQueueController.CreateOnCurrentThread();
        _systemDispatcherController =
            Windows.System.DispatcherQueueController.CreateOnDedicatedThread();

        Windows.UI.Composition.Compositor? compositor = null;
        using (var compositorReady = new ManualResetEventSlim())
        {
            if (!_systemDispatcherController.DispatcherQueue.TryEnqueue(() =>
                {
                    compositor = new Windows.UI.Composition.Compositor();
                    compositorReady.Set();
                }))
                throw new InvalidOperationException("System compositor dispatch was rejected.");
            if (!compositorReady.Wait(TimeSpan.FromSeconds(5)))
                throw new TimeoutException("System compositor creation timed out.");
        }
        _systemCompositor = compositor ??
            throw new InvalidOperationException("System compositor was not created.");
        var root = _systemCompositor.CreateContainerVisual();
        root.RelativeSizeAdjustment = Vector2.One;
        _island = ContentIsland.CreateForSystemVisual(
            _dispatcherController.DispatcherQueue, root);
        _island.RequestSize(new Vector2(width, height));

        var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);
        appWindow.AssociateWithDispatcherQueue(_dispatcherController.DispatcherQueue);
        _siteBridge = DesktopAttachedSiteBridge.CreateFromWindowId(
            _dispatcherController.DispatcherQueue, windowId);
        // Arm N's raw HWND owns pointer delivery. Windows App SDK 2.4 fail-fasts
        // in Microsoft.UI.Input when keyboard processing is disabled on this
        // top-level bridge, so site keyboard processing must remain enabled.
        // The product WndProc translation is retained until A7 selects and
        // validates one end-to-end keyboard/IME owner.
        _siteBridge.ProcessesPointerInput = false;
        _siteBridge.Connect(_island);
        if (!_island.IsConnected)
            throw new InvalidOperationException("ContentIsland did not connect to the raw HWND.");
        WasConnected = true;
    }

    internal bool IsConnected => !_disposed && _island.IsConnected;

    internal bool WasConnected { get; }

    internal ContentIsland Island => _island;

    internal void RequestSize(int width, int height)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _island.RequestSize(new Vector2(width, height));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _siteBridge.Dispose();
        _island.Dispose();
        using var compositorDisposed = new ManualResetEventSlim();
        if (_systemDispatcherController.DispatcherQueue.TryEnqueue(() =>
            {
                _systemCompositor.Dispose();
                compositorDisposed.Set();
            }))
            compositorDisposed.Wait(TimeSpan.FromSeconds(5));
        _ = _systemDispatcherController.ShutdownQueueAsync();
        _dispatcherController.ShutdownQueue();
    }
}
