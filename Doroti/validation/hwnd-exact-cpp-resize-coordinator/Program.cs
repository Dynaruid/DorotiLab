using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using Doroti.Host.WindowsAppSdk;

namespace Doroti.Validation.HwndExactCppResizeCoordinator;

internal static partial class Program
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    private struct NativeFilteredWaitResult
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal uint Status;
        internal uint Reserved;
        internal ulong SuccessfulWaitCount;
        internal ulong TimeoutWaitCount;
        internal ulong TaskCompletionDispatchCount;
        internal ulong TopLevelRecursiveDispatchCount;
        internal ulong ChildRecursiveDispatchCount;
        internal ulong MaximumWaitElapsedMs;
        internal uint GdiStart;
        internal uint GdiEnd;
        internal uint UserStart;
        internal uint UserEnd;
    }

    [LibraryImport("Doroti.HwndExactCpp.ManagedPresenterProbe", EntryPoint = "doroti_run_filtered_wait_probe_v1")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial uint RunFilteredWaitProbe(ref NativeFilteredWaitResult result);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private static int Main(string[] args)
    {
        var reportPath = ResolveReportPath(args);
        try
        {
            using var coordinator = new WindowsManagedResizeCoordinator(TimeSpan.FromMilliseconds(100));
            var matrix = new List<object>();
            var nativeWait = new NativeFilteredWaitResult
            {
                AbiVersion = 1,
                StructSize = checked((uint)Marshal.SizeOf<NativeFilteredWaitResult>()),
            };
            var nativeWaitStatus = RunFilteredWaitProbe(ref nativeWait);
            ValidateNativeWait(nativeWaitStatus, nativeWait);

            Present(coordinator, Publish(coordinator, 640, 480, 1, "normal"));
            var same1 = Publish(coordinator, 640, 480, 2, "same-size-1");
            Present(coordinator, same1);
            Present(coordinator, Publish(coordinator, 640, 480, 3, "same-size-2"));
            matrix.Add(new { caseName = "same-size repeated generation", status = "PASS" });

            var abaA1 = Publish(coordinator, 700, 500, 4, "aba-a1");
            _ = Publish(coordinator, 760, 540, 5, "aba-b");
            var abaA2 = Publish(coordinator, 700, 500, 6, "aba-a2");
            Supersede(coordinator, abaA1, "A-B-A current superseded");
            Present(coordinator, abaA2);
            matrix.Add(new { caseName = "A-B-A", status = "PASS" });

            Present(coordinator, Publish(coordinator, 900, 620, 7, "grow"));
            Present(coordinator, Publish(coordinator, 620, 420, 8, "shrink"));
            matrix.Add(new { caseName = "grow-shrink", status = "PASS" });

            var directionalSizes = new (string Name, int Width, int Height)[]
            {
                ("left", 641, 420), ("right", 642, 420),
                ("top", 642, 421), ("bottom", 642, 422),
                ("top-left", 643, 423), ("top-right", 644, 424),
                ("bottom-left", 645, 425), ("bottom-right", 646, 426),
            };
            foreach (var direction in directionalSizes)
                Present(coordinator, Publish(coordinator, direction.Width, direction.Height, 9, direction.Name));
            matrix.Add(new { caseName = "eight sizing directions", status = "PASS", samples = directionalSizes.Length });

            var buildCurrent = Publish(coordinator, 650, 430, 10, "before-build-current");
            _ = Publish(coordinator, 651, 431, 11, "before-build-replaced");
            var buildLatest = Publish(coordinator, 652, 432, 12, "before-build-latest");
            Supersede(coordinator, buildCurrent, "superseded before framework build");
            Present(coordinator, buildLatest);
            matrix.Add(new { caseName = "before-build supersede", status = "PASS" });

            var rasterCurrent = Publish(coordinator, 660, 440, 13, "raster-current");
            var rasterLatest = Publish(coordinator, 670, 450, 14, "raster-latest");
            Present(coordinator, rasterCurrent);
            Present(coordinator, rasterLatest);
            matrix.Add(new { caseName = "during-raster stale present", status = "PASS" });

            var copyCurrent = Publish(coordinator, 680, 460, 15, "copy-current");
            var copyLatest = Publish(coordinator, 690, 470, 16, "copy-latest");
            Present(coordinator, copyCurrent);
            Present(coordinator, copyLatest);
            matrix.Add(new { caseName = "lease-return to copy stale present", status = "PASS" });

            var lateCurrent = Publish(coordinator, 700, 480, 17, "late-current");
            var currentWait = coordinator.WaitForCompletion(lateCurrent.Generation);
            Require(currentWait.TimedOut && currentWait.Receipt is null, "Current generation did not time out.");
            Require(currentWait.Elapsed <= TimeSpan.FromMilliseconds(250), "Current timeout exceeded scheduler tolerance.");
            Present(coordinator, lateCurrent);
            matrix.Add(new { caseName = "timeout then late current completion", status = "PASS", elapsedMs = currentWait.Elapsed.TotalMilliseconds });

            var lateStale = Publish(coordinator, 710, 490, 18, "late-stale");
            var staleWait = coordinator.WaitForCompletion(lateStale.Generation);
            Require(staleWait.TimedOut && staleWait.Elapsed <= TimeSpan.FromMilliseconds(250),
                "Stale generation timeout exceeded its contract.");
            var afterLate = Publish(coordinator, 720, 500, 19, "after-late");
            Present(coordinator, lateStale);
            Present(coordinator, afterLate);
            matrix.Add(new { caseName = "timeout then stale completion", status = "PASS", elapsedMs = staleWait.Elapsed.TotalMilliseconds });

            var minimized = Publish(coordinator, 0, 0, 20, "minimize");
            Require(Receipt(coordinator, minimized).Terminal == WindowsResizeTerminal.Failed,
                "Minimize target did not receive a lifecycle terminal.");
            Present(coordinator, Publish(coordinator, 730, 510, 21, "restore"));
            matrix.Add(new { caseName = "minimize-restore", status = "PASS" });

            foreach (var scale in new[] { 1.0, 1.25, 1.5, 1.75, 2.0 })
            {
                var target = coordinator.Publish(1, (int)(600 * scale), (int)(400 * scale), scale, 22);
                Require(coordinator.ValidateExact(target.Generation, target.WidthPx, target.HeightPx),
                    "DPI target failed exact admission.");
                Present(coordinator, target);
            }
            matrix.Add(new { caseName = "DPI matrix", status = "PASS", scales = new[] { 100, 125, 150, 175, 200 } });

            var closeTarget = Publish(coordinator, 740, 520, 23, "close-during-wait");
            var closeWaitTask = Task.Run(() => coordinator.WaitForCompletion(closeTarget.Generation));
            Thread.Sleep(10);
            coordinator.Close();
            var closeWait = closeWaitTask.GetAwaiter().GetResult();
            Require(!closeWait.TimedOut && closeWait.Receipt?.Terminal == WindowsResizeTerminal.Failed,
                "Close did not wake the bounded wait with Failed.");
            matrix.Add(new { caseName = "close-during-wait", status = "PASS", elapsedMs = closeWait.Elapsed.TotalMilliseconds });

            var snapshot = coordinator.Snapshot();
            ValidateSnapshot(snapshot);
            var report = new
            {
                schemaVersion = "doroti.windows.hwnd-exact-cpp-resize-coordinator/v1",
                gate = "C4-managed-coordinator",
                status = "PASS",
                maximumWaitMs = 100,
                schedulerToleranceMs = 150,
                nativeFilteredWait = new
                {
                    nativeWait.SuccessfulWaitCount,
                    nativeWait.TimeoutWaitCount,
                    nativeWait.TaskCompletionDispatchCount,
                    nativeWait.TopLevelRecursiveDispatchCount,
                    nativeWait.ChildRecursiveDispatchCount,
                    nativeWait.MaximumWaitElapsedMs,
                    nativeWait.GdiStart,
                    nativeWait.GdiEnd,
                    nativeWait.UserStart,
                    nativeWait.UserEnd,
                },
                matrix,
                snapshot,
                scopeBoundary = "Automated coordinator contract only. It is not visible resize, compositor cadence, or physical acceptance.",
            };
            Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);
            File.WriteAllText(reportPath, JsonSerializer.Serialize(report, JsonOptions));
            Console.WriteLine(JsonSerializer.Serialize(report, JsonOptions));
            Console.WriteLine($"report={reportPath}");
            return 0;
        }
        catch (Exception exception)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);
            File.WriteAllText(reportPath, JsonSerializer.Serialize(new
            {
                schemaVersion = "doroti.windows.hwnd-exact-cpp-resize-coordinator/v1",
                gate = "C4-managed-coordinator",
                status = "FAIL",
                exception = exception.ToString(),
            }, JsonOptions));
            Console.Error.WriteLine(exception);
            Console.Error.WriteLine($"report={reportPath}");
            return 1;
        }
    }

    private static WindowsResizeTarget Publish(
        WindowsManagedResizeCoordinator coordinator,
        int width,
        int height,
        ulong frameId,
        string caseName)
    {
        var target = coordinator.Publish(1, width, height, 1, frameId);
        Require(coordinator.ValidateExact(target.Generation, width, height), $"{caseName} failed exact admission.");
        return target;
    }

    private static void Present(WindowsManagedResizeCoordinator coordinator, WindowsResizeTarget target)
    {
        Require(coordinator.TryComplete(target.Generation, WindowsResizeTerminal.Presented, "exact managed present"),
            $"Generation {target.Generation} was already terminal.");
    }

    private static void Supersede(WindowsManagedResizeCoordinator coordinator, WindowsResizeTarget target, string detail)
    {
        Require(coordinator.TryComplete(target.Generation, WindowsResizeTerminal.Superseded, detail),
            $"Generation {target.Generation} was already terminal.");
    }

    private static WindowsResizeReceipt Receipt(WindowsManagedResizeCoordinator coordinator, WindowsResizeTarget target) =>
        coordinator.Snapshot().Receipts.Single(value => value.Target.Generation == target.Generation);

    private static void ValidateSnapshot(WindowsResizeCoordinatorSnapshot snapshot)
    {
        Require(snapshot.QueueDepth == 0 && snapshot.MaximumQueueDepth <= 2, "The current+latest queue exceeded its bound.");
        Require(snapshot.AcceptedCount == snapshot.Receipts.Count && snapshot.UnterminatedCount == 0,
            "Not every accepted generation has exactly one terminal.");
        Require(snapshot.DuplicateTerminalCount == 0, "A generation received more than one terminal.");
        Require(snapshot.ExactAdmissionMismatchCount == 0, "An exact admission mismatch occurred.");
        Require(snapshot.StalePresentPreventedCount == 3, "The stale-present matrix changed.");
        Require(snapshot.PlatformWaitTimeoutCount == 2, "The timeout matrix changed.");
        Require(snapshot.Receipts.Count(value => value.PlatformWaitTimedOut) == 2,
            "Late terminal receipts lost the platform-timeout marker.");
    }

    private static void ValidateNativeWait(uint status, NativeFilteredWaitResult result)
    {
        Require(status == 0 && result.Status == 0, "The native filtered-wait probe failed.");
        Require(result.SuccessfulWaitCount == 1 && result.TimeoutWaitCount == 1,
            "The native bounded-wait result count changed.");
        Require(result.TaskCompletionDispatchCount == 1 &&
                result.TopLevelRecursiveDispatchCount == 0 && result.ChildRecursiveDispatchCount == 0,
            "The filtered wait dispatched an unrelated HWND message.");
        Require(result.MaximumWaitElapsedMs is >= 100 and <= 250,
            "The native bounded wait exceeded scheduler tolerance.");
        Require(result.GdiStart == result.GdiEnd && result.UserStart == result.UserEnd,
            "The native filtered-wait probe leaked HWND resources.");
    }

    private static string ResolveReportPath(string[] args)
    {
        var index = Array.IndexOf(args, "--report");
        if (index >= 0 && index + 1 < args.Length) return Path.GetFullPath(args[index + 1]);
        return Path.GetFullPath(Path.Combine(".doroti", "evidence", "hwnd-exact-cpp-c4-resize-coordinator.json"));
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
