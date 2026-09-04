using System.Runtime.InteropServices;
using System.Text.Json;
using Doroti.Host.WindowsAppSdk;
using SkiaSharp;

internal static class PreparedMovingFrameFixture
{
    internal static void Run()
    {
        static void Require(bool value, string message)
        {
            if (!value) throw new InvalidOperationException(message);
        }
        var key = new MovingFrameKey(1, 1, 0, 4, 0, 0, 320, 240, 320, 240, 1);
        var ledger = new PreparedMovingFrameLedger();
        ledger.Reserve(new(key, 0, 1));
        Require(ledger.IsReserved(0) && !ledger.IsReserved(1), "Slot reservation failed.");
        Require(!ledger.Complete(key with { InputSequence = 1 }, 1), "Stale input committed.");
        Require(!ledger.Complete(key, 2), "Stale viewport committed.");
        ledger.Reserve(new(key with { MetricsGeneration = 2 }, 1, 2));
        Require(ledger.Cancelled == 1 && !ledger.IsReserved(0), "Replacement leaked a reservation.");
        Require(ledger.Cancel() && !ledger.Cancel(), "Cancel was not idempotent.");
        Require(ledger.Prepared == ledger.Cancelled && ledger.Current is null, "Terminal ledger did not drain.");

        Marshal.ThrowExceptionForHR(CoInitializeEx(0, 0));
        var window = CreateWindowExW(0, "STATIC", "Doroti J1 prepared fixture", 0x00CF0000,
            100, 100, 360, 280, 0, 0, 0, 0);
        Require(window != 0, "Fixture HWND creation failed.");
        // A CompositionFrame receipt requires a visible, unoccluded surface.
        ShowWindow(window, 5);
        Require(SetWindowPos(window, new nint(-1), 0, 0, 0, 0, 0x0013),
            "Could not expose the fixture for composition receipts.");
        try
        {
            using var presenter = new WindowsManagedVulkanPresenter(enableDiagnostics: true);
            presenter.AttachWindow(window);
            presenter.ResizeViewport(320, 240, 1, 4, true);
            for (ulong generation = 1; generation <= 3; generation++)
            {
                Require(presenter.EnsureTarget(window, 320, 240), "Prepare acquire failed.");
                presenter.RenderAndPrepare(key with { MetricsGeneration = generation }, surface =>
                {
                    surface.Canvas.Clear(SKColors.DarkBlue);
                    return true;
                }, static result => result);
                Require(presenter.LastPrepareSucceeded && !presenter.LastPresentSucceeded,
                    "Prepare unexpectedly presented or failed.");
                Require(presenter.PresentCount == 0 && presenter.GpuCopyCount == generation,
                    "Prepare did not stop after synchronous copy.");
                Require(presenter.PreparedSlotIsAvailableForValidation(),
                    "Non-visible prepared slot availability was not signaled.");
                Require(presenter.PreparedDiagnostics.Reserved == 1,
                    "More than one prepared slot exists.");
            }
            presenter.CancelPreparedMovingFrame();
            Require(presenter.PreparedDiagnostics is { Prepared: 3, Cancelled: 3, Reserved: 0 },
                "Latest-wins cancel ledger did not drain.");
            Require(presenter.EnsureTarget(window, 320, 240), "Stale prepare acquire failed.");
            presenter.RenderAndPrepare(key with { InputSequence = 1 },
                surface => { surface.Canvas.Clear(SKColors.Black); return false; }, static value => value);
            Require(!presenter.LastPrepareSucceeded && presenter.PreparedDiagnostics.Reserved == 0 &&
                presenter.GpuCopyCount == 3, "Stale input produced a prepared copy.");
            Require(presenter.EnsureTarget(window, 320, 240), "Cancelled slot could not be acquired.");
            presenter.RenderAndPrepare(key with { MetricsGeneration = 4 }, surface =>
            {
                surface.Canvas.Clear(SKColors.DarkGreen);
                return true;
            }, static result => result);
            presenter.ResetDevice();
            Require(presenter.PreparedDiagnostics.Reserved == 0, "Reset leaked a reservation.");
            presenter.ResizeViewport(320, 240, 1, 4, true);
            Require(presenter.EnsureTarget(window, 320, 240), "Acquire after reset failed.");
            presenter.RenderAndPresent(surface => { surface.Canvas.Clear(SKColors.DarkRed); return true; }, static value => value);
            Require(presenter.LastPresentSucceeded && presenter.PresentCount == 1,
                "Ordinary present after prepare/reset failed.");
            Require(presenter.Snapshot().ResizeClockWaits == 0,
                "Preparation, cancellation, reset or ordinary present waited for the resize clock.");
            Require(presenter.EnsureTarget(window, 320, 240), "Final prepare acquire failed.");
            presenter.RenderAndPrepare(key with { MetricsGeneration = 5 },
                surface => { surface.Canvas.Clear(SKColors.DarkBlue); return true; }, static value => value);
            Require(presenter.EnsureTarget(window, 320, 240), "Device-loss prepare acquire failed.");
            Environment.SetEnvironmentVariable("DOROTI_WINDOWS_VULKAN_INJECT_RESULT", "DEVICE_LOST");
            Environment.SetEnvironmentVariable("DOROTI_WINDOWS_VULKAN_INJECT_AFTER_PRESENTS", "1");
            var lost = false;
            try
            {
                presenter.RenderAndPrepare(key with { MetricsGeneration = 6 },
                    surface => { surface.Canvas.Clear(SKColors.Black); return true; }, static value => value);
            }
            catch (WindowsManagedVulkanDeviceLostException)
            {
                lost = true;
                Require(presenter.PreparedDiagnostics.Reserved == 0,
                    "Device loss leaked a prepared slot.");
                presenter.AbandonContextForDeviceLoss();
                presenter.RecoverAfterDeviceLoss();
            }
            finally
            {
                Environment.SetEnvironmentVariable("DOROTI_WINDOWS_VULKAN_INJECT_RESULT", null);
                Environment.SetEnvironmentVariable("DOROTI_WINDOWS_VULKAN_INJECT_AFTER_PRESENTS", null);
            }
            Require(lost, "Device-loss injection did not run.");
            presenter.ResizeViewport(320, 240, 1, 4, true);
            Require(presenter.EnsureTarget(window, 320, 240), "Prepare after device loss failed.");
            presenter.RenderAndPrepare(key with { MetricsGeneration = 7 },
                surface => { surface.Canvas.Clear(SKColors.DarkBlue); return true; }, static value => value);
            Require(presenter.CommitPreparedMovingFrame(key with { MetricsGeneration = 7, Left = 1 }) == 1 &&
                presenter.PreparedDiagnostics.Reserved == 0 && presenter.PresentCount == 1,
                "Mismatched prepared geometry became visible.");
            Require(presenter.EnsureTarget(window, 320, 240), "Unaligned commit acquire failed.");
            presenter.RenderAndPrepare(key with { MetricsGeneration = 7 },
                surface => { surface.Canvas.Clear(SKColors.DarkBlue); return true; }, static value => value);
            Require(presenter.CommitPreparedMovingFrame(key with { MetricsGeneration = 7 }) == 1 &&
                presenter.PreparedDiagnostics.Reserved == 0 && presenter.PresentCount == 1 &&
                presenter.Snapshot().ResizeClockWaits == 0,
                "An unaligned frame was committed or waited after geometry.");
            Require(presenter.EnsureTarget(window, 320, 240), "Commit test acquire failed.");
            var commitKey = key with { ResizeEpoch = 2, MetricsGeneration = 8 };
            presenter.RenderAndPrepare(commitKey,
                surface => { surface.Canvas.Clear(SKColors.DarkGreen); return true; }, static value => value);
            Require(presenter.AlignPreparedMovingFrame(commitKey) == 0 &&
                presenter.AlignPreparedMovingFrame(commitKey) == 0 && presenter.PresentCount == 1,
                "Clock alignment presented pixels or was not idempotent.");
            Require(presenter.CommitPreparedMovingFrame(commitKey) == 0 && presenter.PresentCount == 2,
                "Exact prepared commit failed.");
            Require(presenter.CommitPreparedMovingFrame(commitKey) == 1 && presenter.PresentCount == 2,
                "A prepared frame was presented twice.");
            Require(presenter.Snapshot() is { ResizeClockWaits: 1, ResizeClockSignals: 1, ResizeClockFailures: 0 },
                "Exact commit did not consume one clock signal, or mismatch/double commit waited.");
            Require(presenter.EnsureTarget(window, 320, 240), "Clock failure prepare acquire failed.");
            var timeoutKey = key with { ResizeEpoch = 3, MetricsGeneration = 9 };
            presenter.RenderAndPrepare(timeoutKey,
                surface => { surface.Canvas.Clear(SKColors.DarkBlue); return true; }, static value => value);
            Environment.SetEnvironmentVariable("DOROTI_WINDOWS_VULKAN_INJECT_RESULT", "COMPOSITOR_CLOCK_TIMEOUT");
            try
            {
                Require(presenter.AlignPreparedMovingFrame(timeoutKey) == 0 && presenter.PresentCount == 2,
                    "Clock timeout presented before geometry.");
                Require(presenter.CommitPreparedMovingFrame(timeoutKey) == 0 && presenter.PresentCount == 3,
                    "Clock timeout stranded the prepared frame instead of submitting.");
                Require(presenter.Snapshot() is { ResizeClockWaits: 2, ResizeClockSignals: 1, ResizeClockFailures: 1, LastResizeClockStatus: 0x102 },
                    "Clock timeout was counted as a signal or its failure was lost.");
            }
            finally
            {
                Environment.SetEnvironmentVariable("DOROTI_WINDOWS_VULKAN_INJECT_RESULT", null);
            }
            Require(presenter.EnsureTarget(window, 320, 240), "Shutdown prepare acquire failed.");
            presenter.RenderAndPrepare(key with { ResizeEpoch = 4, MetricsGeneration = 10 },
                surface => { surface.Canvas.Clear(SKColors.DarkBlue); return true; }, static value => value);
            presenter.ReleaseCompositionResources();
            Require(presenter.PreparedDiagnostics.Reserved == 0, "Shutdown leaked a reservation.");
            Console.WriteLine(JsonSerializer.Serialize(new {
                status = "PASS-prepared-clock-commit", prepared = presenter.PreparedDiagnostics.Prepared,
                cancelled = presenter.PreparedDiagnostics.Cancelled, reserved = presenter.PreparedDiagnostics.Reserved,
                presents = presenter.PresentCount, clock = presenter.Snapshot().ResizeClockWaits,
                clockSignals = presenter.Snapshot().ResizeClockSignals,
                injectedClockFailures = presenter.Snapshot().ResizeClockFailures, physical = "notVerified"
            }));
        }
        finally
        {
            DestroyWindow(window);
            CoUninitialize();
        }
    }

    [DllImport("ole32.dll")] private static extern int CoInitializeEx(nint reserved, uint flags);
    [DllImport("ole32.dll")] private static extern void CoUninitialize();
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern nint CreateWindowExW(uint exStyle, string className, string title,
        uint style, int x, int y, int width, int height, nint parent, nint menu, nint instance, nint parameter);
    [DllImport("user32.dll")] private static extern bool DestroyWindow(nint window);
    [DllImport("user32.dll")] private static extern bool ShowWindow(nint window, int command);
    [DllImport("user32.dll")] private static extern bool SetWindowPos(nint window, nint after, int x, int y, int width, int height, uint flags);
}
