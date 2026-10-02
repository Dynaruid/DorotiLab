#if MACOS
using AppKit;
using CoreGraphics;
using Doroti.Host.Maui;
using Doroti.Ui;
using Foundation;
using SkiaSharp;
using System.Text.Json;

namespace DorotiTestbedApp.MacOS;

// Testbed-only native fixtures. Backing scale and keyboard events are synthetic;
// submissions, terminal callbacks and resource retirement use the actual Metal owner.
internal static class AppKitFrameLifecycleProbe
{
    private static readonly MauiSurfaceSnapshot Empty = new(0, 0, 1, 0, 0, 0, "probe", "probe");

    internal static async Task RunAsync(string output)
    {
        List<DorotiMacOSMetalView> views = [];
        List<DorotiMacOSMetalSurface> surfaces = [];
        List<NSWindow> windows = [];
        Dictionary<string, object> checks = [];
        try
        {
            ProbeWindow window = null!;
            DorotiMacOSMetalView first = null!, replacement = null!, independent = null!;
            DorotiMacOSMetalSurface surface = null!;
            List<KeyData> keys = [];
            List<bool> focus = [];
            int initialRoots = 0;
            await AppKitUi.Invoke(() =>
            {
                initialRoots = DorotiMacOSMetalView.RetiringViewCount;
                window = new ProbeWindow();
                windows.Add(window);
                surface = MakeSurface(9001);
                ((IMauiSkiaSurface)surface).Key += keys.Add;
                ((IMauiSkiaSurface)surface).FocusChanged += focus.Add;
                first = MakeView(window, surface);
                window.OrderFront(null);
            });
            await WaitFor(first, s => s.CommandBuffersCompleted > 0 && s.NativeFramePipeline!.PendingGpuFrames == 0);

            Task retirement = null!;
            Task backgroundSnapshots = null!;
            await AppKitUi.Invoke(() =>
            {
                first.RequestFrame();
                first.Draw();
                var submitted = first.CaptureSnapshot(Empty);
                Require(submitted.NativeFramePipeline!.PendingGpuFrames > 0, "No actual Metal submission to retire.");
                SendKey(first, window);
                backgroundSnapshots = Task.Run(async () =>
                {
                    for (var i = 0; i < 20; i++)
                    {
                        Require(!string.IsNullOrEmpty(first.CaptureSnapshot(Empty).MetalDevice), "Device identity disappeared.");
                        await Task.Delay(5);
                    }
                });
                surface.Disconnect(first);
                retirement = first.RetireAsync();
                Require(!retirement.IsCompleted, "Terminal callback must return to the AppKit loop before retirement.");
                Require(keys.Count == 2 && keys[0].type == KeyEventType.down && keys[1].type == KeyEventType.up
                    && keys[1].synthesized && keys[0].physical == keys[1].physical, "Disconnect left a pressed key.");
                Require(focus.Last() == false, "Disconnect did not clear focus.");

                replacement = new DorotiMacOSMetalView();
                views.Add(replacement);
                var rejected = false;
                try { surface.Connect(replacement); }
                catch (InvalidOperationException) { rejected = true; }
                Require(rejected && surface.NativeView is null, "Same owner reconnected before its old queue retired.");

                var otherWindow = new ProbeWindow();
                windows.Add(otherWindow);
                independent = MakeView(otherWindow, MakeSurface(9002));
                otherWindow.OrderFront(null);
                Require(DorotiMacOSMetalView.RetiringViewCount > initialRoots, "Pending owner was not retained.");
                checks["ownerIsolation"] = new { status = "PASS", pendingAtDetach = submitted.NativeFramePipeline.PendingGpuFrames,
                    sameOwnerRejected = rejected, independentOwnerConnected = true, hardwareOverlap = "notMeasured" };
                checks["disconnectInput"] = new { status = "PASS", synthesizedKeyUp = true, focusCleared = true };
            });
            await retirement.WaitAsync(TimeSpan.FromSeconds(10));
            await backgroundSnapshots;
            await AppKitUi.Invoke(() =>
            {
                var retired = first.CaptureSnapshot(Empty);
                Require(retired.NativeFramePipeline!.PendingGpuFrames == 0 && retired.MetalAllocatedBytes is null,
                    "Snapshot still reads retired resources.");
                Require(DorotiMacOSMetalView.RetiringViewCount == initialRoots, "Completed owner remained rooted.");
                checks["retiredSnapshot"] = new { status = "PASS", backgroundSamples = 20, snapshot = retired };
                window.ContentView = replacement;
                surface.Connect(replacement);
                replacement.NeedsLayout = true;
                replacement.LayoutSubtreeIfNeeded();
                replacement.RequestFrame();
            });
            await WaitFor(replacement, s => s.CommandBuffersCompleted > 0 && s.NativeFramePipeline!.PendingGpuFrames == 0);
            await WaitFor(independent, s => s.CommandBuffersCompleted > 0 && s.NativeFramePipeline!.PendingGpuFrames == 0);

            for (var cycle = 0; cycle < 3; cycle++)
            {
                var before = await Read(replacement);
                await AppKitUi.Invoke(() => window.OrderOut(null));
                await AssertIdle(replacement, before.CommandBuffersCommitted);
                await AppKitUi.Invoke(() => { window.OrderFront(null); replacement.RequestFrame(); });
                await WaitFor(replacement, s => s.CommandBuffersCompleted > before.CommandBuffersCompleted
                    && s.NativeFramePipeline!.PendingGpuFrames == 0);
            }
            checks["hideResume"] = new { status = "PASS", cycles = 3, resume = await Read(replacement) };

            var attached = await Read(replacement);
            await AppKitUi.Invoke(() =>
            {
                keys.Clear();
                SendKey(replacement, window);
                window.ContentView = null;
                Require(keys.Count == 2 && keys[1].type == KeyEventType.up && keys[1].synthesized,
                    "Native window detach left a pressed key.");
            });
            await AssertIdle(replacement, attached.CommandBuffersCommitted);
            await AppKitUi.Invoke(() =>
            {
                window.ContentView = replacement;
                replacement.NeedsLayout = true;
                replacement.LayoutSubtreeIfNeeded();
                replacement.RequestFrame();
            });
            await WaitFor(replacement, s => s.CommandBuffersCompleted > attached.CommandBuffersCompleted
                && s.NativeFramePipeline!.PendingGpuFrames == 0);
            checks["detachReattach"] = new { status = "PASS", snapshot = await Read(replacement), synthesizedKeyUp = true };

            List<MauiSurfaceSnapshot> scales = [];
            foreach (var scale in new[] { 1.0, 1.5, 2.0 })
            {
                await AppKitUi.Invoke(() =>
                {
                    window.ProbeScale = (nfloat)scale;
                    replacement.DidChangeBackingProperties();
                    replacement.LayoutSubtreeIfNeeded();
                    replacement.RequestFrame();
                });
                await WaitFor(replacement, s => s.DevicePixelRatio == scale
                    && s.PixelWidth == (int)Math.Round(s.LogicalWidth * scale)
                    && s.PixelHeight == (int)Math.Round(s.LogicalHeight * scale)
                    && s.NativeFramePipeline!.PendingGpuFrames == 0);
                scales.Add(await Read(replacement));
            }
            checks["syntheticBackingScale"] = new { status = "PASS", windowFactors = scales,
                physicalMonitorTransition = "notVerified" };
            await Cleanup();
            await AppKitUi.Invoke(() => Require(DorotiMacOSMetalView.RetiringViewCount == initialRoots, "Retirement roots leaked."));
            checks["finalDrain"] = new { status = "PASS", retiringOwners = initialRoots };
            File.WriteAllText(output, JsonSerializer.Serialize(new { status = "PASS", checks, physicalInput = "notVerified" }));
        }
        catch (Exception error)
        {
            File.WriteAllText(output + ".error", error.ToString());
            await Cleanup();
        }
        await AppKitUi.Invoke(() => NSApplication.SharedApplication.Terminate(NSApplication.SharedApplication));

        DorotiMacOSMetalSurface MakeSurface(ulong id)
        {
            var result = new DorotiMacOSMetalSurface(id);
            surfaces.Add(result);
            ((IMauiSkiaSurface)result).Paint += p => p.Surface.Canvas.Clear(SKColors.CornflowerBlue);
            ((IMauiSkiaSurface)result).PaintFailed += (_, error) => throw new InvalidOperationException("Metal probe paint failed.", error);
            return result;
        }
        DorotiMacOSMetalView MakeView(NSWindow owner, DorotiMacOSMetalSurface virtualView)
        {
            var result = new DorotiMacOSMetalView();
            views.Add(result);
            owner.ContentView = result;
            virtualView.Connect(result);
            result.NeedsLayout = true;
            result.LayoutSubtreeIfNeeded();
            return result;
        }
        async Task Cleanup()
        {
            List<Task> retiring = [];
            await AppKitUi.Invoke(() =>
            {
                foreach (var surface in surfaces) surface.Dispose();
                foreach (var view in views) retiring.Add(view.RetireAsync());
            });
            await Task.WhenAll(retiring).WaitAsync(TimeSpan.FromSeconds(10));
            await AppKitUi.Invoke(() =>
            {
                foreach (var window in windows) { window.ContentView = null; window.OrderOut(null); window.Dispose(); }
                foreach (var view in views) view.Dispose();
                windows.Clear();
                views.Clear();
            });
        }
    }

    private static void SendKey(DorotiMacOSMetalView view, NSWindow window)
    {
        using var key = NSEvent.KeyEvent(NSEventType.KeyDown, CGPoint.Empty, 0, 1, window.WindowNumber,
            null, "a", "a", false, 0);
        view.KeyDown(key!);
    }
    private static async Task<MauiSurfaceSnapshot> Read(DorotiMacOSMetalView view)
    {
        MauiSurfaceSnapshot result = null!;
        await AppKitUi.Invoke(() => result = view.CaptureSnapshot(Empty));
        return result;
    }
    private static async Task WaitFor(DorotiMacOSMetalView view, Func<MauiSurfaceSnapshot, bool> predicate)
    {
        for (var i = 0; i < 30; i++)
        {
            if (predicate(await Read(view))) return;
            await Task.Delay(100);
        }
        throw new TimeoutException("AppKit frame condition did not converge: " + JsonSerializer.Serialize(await Read(view)));
    }
    private static async Task AssertIdle(DorotiMacOSMetalView view, long committed)
    {
        for (var i = 0; i < 3; i++)
        {
            await AppKitUi.Invoke(() => { view.RequestFrame(); view.Draw(); });
            await Task.Delay(100);
            Require((await Read(view)).CommandBuffersCommitted == committed, "Inactive view kept submitting Metal work.");
        }
    }
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
    private sealed class ProbeWindow : NSWindow
    {
        internal nfloat ProbeScale { get; set; } = 2;
        public override nfloat BackingScaleFactor => ProbeScale;
        internal ProbeWindow() : base(new CGRect(120, 120, 320, 220), NSWindowStyle.Titled,
            NSBackingStore.Buffered, false) { Title = "AppKit frame lifecycle fixture"; }
    }
}
#endif
