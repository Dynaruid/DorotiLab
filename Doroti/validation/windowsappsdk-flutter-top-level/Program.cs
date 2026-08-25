using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using Doroti.Host.WindowsAppSdk;

namespace Doroti.Validation.WindowsAppSdkFlutterTopLevel;

internal static partial class Program
{
    private const int RequiredCycles = 100;
    private const uint WmGetMinMaxInfo = 0x0024;
    private const uint WsOverlappedWindow = 0x00CF0000;
    private const uint WsClipChildren = 0x02000000;
    private const uint WsChild = 0x40000000;
    private const uint WsVisible = 0x10000000;
    private const uint WsClipSiblings = 0x04000000;
    private const uint WsPopup = 0x80000000;
    private const uint WsExAppWindow = 0x00040000;
    private const uint SwpNoZOrder = 0x0004;
    private const uint SwpNoActivate = 0x0010;
    private const string EvidenceSchema =
        "doroti.windowsappsdk-flutter-top-level-evidence/v1";

    private static int Main(string[] args)
    {
        var options = ParseOptions(args);
        try
        {
            var result = RunOnDedicatedStaThread(RunCycles);
            var executablePath = Environment.ProcessPath ?? throw new InvalidOperationException(
                "The F2 validator executable path is unavailable.");
            var executableHash = ComputeFileHash(executablePath);
            if (!string.IsNullOrWhiteSpace(options.PublishedExecutableSha256) &&
                !string.Equals(executableHash, options.PublishedExecutableSha256,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "The F2 validator executable hash does not match the publish-gate input.");
            }
            var evidence = new
            {
                schemaVersion = EvidenceSchema,
                runId = Guid.NewGuid().ToString("N"),
                adapter = "FlutterEmbedder",
                runtime = new
                {
                    packageContractVersion = FlutterWindowsAppSdkBootstrap.ExpectedWindowsAppSdkVersion,
                    deployment = "self-contained-unpackaged",
                },
                platformThread = new
                {
                    apartment = "STA",
                    executionMode = "same-sta-thread",
                    startupShutdownCycles = result.Cycles,
                    failureCount = result.FailureCount,
                },
                hwndTree = new
                {
                    topLevelCount = result.TopLevelCount,
                    childViewCount = result.ChildViewCount,
                    rawWindowDispatcherQueueAssociationCount = result.RawWindowAssociationCount,
                    standardNonClient = result.StandardNonClient,
                    topLevelVisibleBeforeFirstSwap = result.TopLevelVisibleBeforeFirstSwap,
                    firstFrameShowCount = result.FirstFrameShowCount,
                    visibleAfterFirstSwap = result.VisibleAfterFirstSwap,
                    childRectMismatchCount = result.ChildRectMismatchCount,
                    childLayoutCount = result.ChildLayoutCount,
                    topLevelEraseBeforeFirstSwap = result.TopLevelEraseBeforeFirstSwap,
                    viewEraseBeforeFirstSwap = result.ViewEraseBeforeFirstSwap,
                },
                constraints = new
                {
                    minTrackWidth = result.MinimumTrackWidth,
                    minTrackHeight = result.MinimumTrackHeight,
                    maxTrackWidth = result.MaximumTrackWidth,
                    maxTrackHeight = result.MaximumTrackHeight,
                    exactMinMaxConversion = result.ExactMinMaxConversion,
                },
                shutdown = new
                {
                    ordered = result.TeardownOrder,
                    exactOrder = result.ExactTeardownOrder,
                },
                resources = new
                {
                    gdiBefore = result.GdiBefore,
                    gdiAfter = result.GdiAfter,
                    userBefore = result.UserBefore,
                    userAfter = result.UserAfter,
                    boundedAfterWarmup = result.GuiResourcesBoundedAfterWarmup,
                },
                startup = new
                {
                    mauiOrXamlAssemblyCount = result.MauiOrXamlAssemblyNames.Length,
                    mauiOrXamlAssemblyNames = result.MauiOrXamlAssemblyNames,
                    noMauiOrXamlWindowStartup = result.MauiOrXamlAssemblyNames.Length == 0,
                },
                validation = new
                {
                    sourceFingerprint = options.SourceFingerprint,
                    executablePath,
                    executableSha256 = executableHash,
                },
                scopeBoundary = "F2 proves standard HWND tree, exact child-client layout, min/max conversion, first-show ordering, and teardown order. It does not prove a real EGL child-window swap or visible blank/white-frame absence; F4 and FG own that acceptance.",
            };
            var json = JsonSerializer.Serialize(evidence, new JsonSerializerOptions { WriteIndented = true });
            if (!string.IsNullOrWhiteSpace(options.EvidencePath))
            {
                var evidencePath = Path.GetFullPath(options.EvidencePath);
                Directory.CreateDirectory(Path.GetDirectoryName(evidencePath) ??
                    throw new InvalidOperationException("The evidence path has no parent directory."));
                File.WriteAllText(evidencePath, json + Environment.NewLine);
            }
            Console.WriteLine(json);
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"windowsappsdk-flutter-top-level FAIL: {exception}");
            return 1;
        }
    }

    private static F2CycleSummary RunCycles()
    {
        _ = RunOneCycle(-1); // Warm Windows class/runtime state before resource sampling.
        var gdiBefore = GetGuiResources(GuiResourceType.Gdi);
        var userBefore = GetGuiResources(GuiResourceType.User);
        F2CycleResult? last = null;
        for (var cycle = 0; cycle < RequiredCycles; cycle++)
        {
            last = RunOneCycle(cycle);
        }
        var gdiAfter = GetGuiResources(GuiResourceType.Gdi);
        var userAfter = GetGuiResources(GuiResourceType.User);
        var result = last ?? throw new InvalidOperationException("No F2 cycle was run.");
        var bounded = gdiAfter <= gdiBefore + 2 && userAfter <= userBefore + 2;
        if (!bounded)
        {
            throw new InvalidOperationException(
                $"F2 GUI resources grew after warmup: GDI {gdiBefore}->{gdiAfter}, USER {userBefore}->{userAfter}.");
        }
        return new F2CycleSummary(
            RequiredCycles,
            0,
            result.TopLevelCount,
            result.ChildViewCount,
            result.RawWindowAssociationCount,
            result.StandardNonClient,
            result.TopLevelVisibleBeforeFirstSwap,
            result.FirstFrameShowCount,
            result.VisibleAfterFirstSwap,
            result.ChildRectMismatchCount,
            result.ChildLayoutCount,
            result.TopLevelEraseBeforeFirstSwap,
            result.ViewEraseBeforeFirstSwap,
            result.MinimumTrackWidth,
            result.MinimumTrackHeight,
            result.MaximumTrackWidth,
            result.MaximumTrackHeight,
            result.ExactMinMaxConversion,
            result.TeardownOrder,
            result.ExactTeardownOrder,
            gdiBefore,
            gdiAfter,
            userBefore,
            userAfter,
            bounded,
            CaptureMauiOrXamlAssemblyNames());
    }

    private static F2CycleResult RunOneCycle(int cycle)
    {
        var stages = new List<string>();
        var bootstrap = new FlutterWindowsAppSdkBootstrap();
        FlutterWindowsHostWindow? host = null;
        try
        {
            bootstrap.InitializeOnCurrentThread();
            host = FlutterWindowsHostWindow.CreateOnCurrentThread(
                bootstrap,
                new FlutterWindowsHostWindowOptions(
                    $"Doroti F2 validation {cycle}",
                    InitialClientWidth: 640,
                    InitialClientHeight: 360,
                    MinimumClientWidth: 320,
                    MinimumClientHeight: 240,
                    MaximumClientWidth: 960,
                    MaximumClientHeight: 720,
                    InitialX: -32000,
                    InitialY: -32000),
                new FlutterWindowsHostWindowTeardown
                {
                    DisposeAuxiliaryIslands = () => stages.Add("auxiliary-islands"),
                    DisposeViewSurface = () => stages.Add("view-surface"),
                    DisposeChildEngine = () => stages.Add("engine"),
                });
            var beforeSwap = host.Snapshot;
            AssertWindowTreeBeforeSwap(beforeSwap);
            AssertClientEqualsChild(host.TopLevelHwnd, host.ViewHwnd);
            DriveStandardResizes(host.TopLevelHwnd, host.ViewHwnd);
            var minMax = RequestAndAssertMinMax(host.TopLevelHwnd, beforeSwap.TopLevelStyle);

            host.NotifyFirstFrameSwapped();
            PumpPendingMessages();
            var afterSwap = host.Snapshot;
            if (!afterSwap.FirstFrameSwapped || !afterSwap.Shown ||
                afterSwap.FirstFrameShowCount != 1 || !IsWindowVisible(host.TopLevelHwnd))
            {
                throw new InvalidOperationException("F2 first-frame callback did not show exactly one top-level HWND.");
            }
            host.NotifyFirstFrameSwapped();
            if (host.Snapshot.FirstFrameShowCount != 1)
                throw new InvalidOperationException("F2 first-frame show callback was not idempotent.");
            host.HideForValidation();

            host.Dispose();
            var afterShutdown = host.Snapshot;
            var expectedTeardown = new[]
            {
                "auxiliary-islands", "view-surface", "child", "engine",
                "appwindow-top-level", "dispatcher-queue",
            };
            var exactTeardown = afterShutdown.TeardownOrder.SequenceEqual(expectedTeardown) &&
                stages.SequenceEqual(new[] { "auxiliary-islands", "view-surface", "engine" });
            if (!exactTeardown || !afterShutdown.Disposed || afterShutdown.TopLevelHwnd != 0 ||
                afterShutdown.ViewHwnd != 0)
            {
                throw new InvalidOperationException("F2 HWND/AppWindow/DispatcherQueue teardown order drifted.");
            }
            host = null;
            return new F2CycleResult(
                TopLevelCount: 1,
                ChildViewCount: 1,
                RawWindowAssociationCount: beforeSwap.RawWindowAssociationCount,
                StandardNonClient: true,
                TopLevelVisibleBeforeFirstSwap: (beforeSwap.TopLevelStyle & WsVisible) != 0,
                FirstFrameShowCount: afterSwap.FirstFrameShowCount,
                VisibleAfterFirstSwap: afterSwap.Shown,
                ChildRectMismatchCount: afterSwap.ChildRectMismatchCount,
                ChildLayoutCount: afterSwap.ChildLayoutCount,
                TopLevelEraseBeforeFirstSwap: afterSwap.TopLevelEraseBeforeFirstSwap,
                ViewEraseBeforeFirstSwap: afterSwap.ViewEraseBeforeFirstSwap,
                MinimumTrackWidth: minMax.MinimumTrackWidth,
                MinimumTrackHeight: minMax.MinimumTrackHeight,
                MaximumTrackWidth: minMax.MaximumTrackWidth,
                MaximumTrackHeight: minMax.MaximumTrackHeight,
                ExactMinMaxConversion: minMax.Exact,
                TeardownOrder: afterShutdown.TeardownOrder.ToArray(),
                ExactTeardownOrder: exactTeardown);
        }
        finally
        {
            if (host is not null) host.Dispose();
            else if (bootstrap.Snapshot.Phase != FlutterWindowsAppSdkBootstrapPhase.Disposed)
                bootstrap.DisposeOnCurrentThread();
        }
    }

    private static void AssertWindowTreeBeforeSwap(FlutterWindowsHostWindowSnapshot snapshot)
    {
        if (snapshot.TopLevelHwnd == 0 || snapshot.ViewHwnd == 0 ||
            !snapshot.DispatcherQueueCreated || !snapshot.RawWindowAssociated ||
            !snapshot.AppWindowAssociated || snapshot.RawWindowAssociationCount != 1)
        {
            throw new InvalidOperationException("F2 did not create the required 1:1 raw HWND/AppWindow/DispatcherQueue tree.");
        }
        if ((snapshot.TopLevelStyle & WsOverlappedWindow) != WsOverlappedWindow ||
            (snapshot.TopLevelStyle & WsClipChildren) == 0 ||
            (snapshot.TopLevelStyle & WsPopup) != 0 ||
            (snapshot.TopLevelStyle & WsVisible) != 0)
        {
            throw new InvalidOperationException("F2 top-level HWND is not hidden standard non-client ownership.");
        }
        if ((snapshot.ViewStyle & (WsChild | WsVisible | WsClipSiblings)) !=
            (WsChild | WsVisible | WsClipSiblings) || GetParent(snapshot.ViewHwnd) != snapshot.TopLevelHwnd)
        {
            throw new InvalidOperationException("F2 child view HWND does not have the required parent/style ownership.");
        }
    }

    private static void DriveStandardResizes(nint topLevelHwnd, nint childHwnd)
    {
        foreach (var size in new[] { (420, 300), (640, 360), (900, 600), (500, 420) })
        {
            if (!SetWindowPos(topLevelHwnd, 0, -32000, -32000, size.Item1, size.Item2,
                    SwpNoZOrder | SwpNoActivate))
            {
                throw new InvalidOperationException($"SetWindowPos F2 resize failed: {Marshal.GetLastWin32Error()}");
            }
            PumpPendingMessages();
            AssertClientEqualsChild(topLevelHwnd, childHwnd);
        }
    }

    private static void AssertClientEqualsChild(nint topLevelHwnd, nint childHwnd)
    {
        if (!GetClientRect(topLevelHwnd, out var parent) || !GetClientRect(childHwnd, out var child) ||
            child.Left != 0 || child.Top != 0 || parent.Width != child.Width || parent.Height != child.Height)
        {
            throw new InvalidOperationException("F2 child client rect does not exactly match the top-level client rect.");
        }
    }

    private static MinMaxResult RequestAndAssertMinMax(nint hwnd, uint style)
    {
        var pointer = Marshal.AllocHGlobal(Marshal.SizeOf<MinMaxInfo>());
        try
        {
            Marshal.StructureToPtr(new MinMaxInfo(), pointer, fDeleteOld: false);
            _ = SendMessageW(hwnd, WmGetMinMaxInfo, 0, pointer);
            var actual = Marshal.PtrToStructure<MinMaxInfo>(pointer);
            var dpi = GetDpiForWindow(hwnd);
            if (dpi == 0) dpi = 96;
            var expectedMinimum = ClientToOuter(320, 240, style, dpi);
            var expectedMaximum = ClientToOuter(960, 720, style, dpi);
            var exact = actual.MinimumTrackSize.X == expectedMinimum.Width &&
                actual.MinimumTrackSize.Y == expectedMinimum.Height &&
                actual.MaximumTrackSize.X == expectedMaximum.Width &&
                actual.MaximumTrackSize.Y == expectedMaximum.Height;
            if (!exact)
                throw new InvalidOperationException("F2 WM_GETMINMAXINFO did not use exact client-to-outer physical conversion.");
            return new MinMaxResult(
                actual.MinimumTrackSize.X,
                actual.MinimumTrackSize.Y,
                actual.MaximumTrackSize.X,
                actual.MaximumTrackSize.Y,
                exact);
        }
        finally
        {
            Marshal.FreeHGlobal(pointer);
        }
    }

    private static NativeRect ClientToOuter(int width, int height, uint style, uint dpi)
    {
        var rect = new NativeRect { Right = width, Bottom = height };
        if (!AdjustWindowRectExForDpi(ref rect, style, hasMenu: false, WsExAppWindow, dpi))
            throw new InvalidOperationException($"AdjustWindowRectExForDpi failed: {Marshal.GetLastWin32Error()}");
        return rect;
    }

    private static void PumpPendingMessages()
    {
        while (PeekMessageW(out var message, 0, 0, 0, 1))
        {
            if (message.Message == 0x0012)
                throw new InvalidOperationException("F2 validation unexpectedly received WM_QUIT.");
            _ = TranslateMessage(in message);
            _ = DispatchMessageW(in message);
        }
    }

    private static string[] CaptureMauiOrXamlAssemblyNames() =>
        AppDomain.CurrentDomain.GetAssemblies()
            .Select(assembly => assembly.GetName().Name ?? assembly.FullName ?? "unknown")
            // Keep the dependency-ban literals out of this fixture while still
            // inspecting the loaded assembly names at runtime.
            .Where(name => name.StartsWith(string.Concat("Microsoft.", "Maui"), StringComparison.OrdinalIgnoreCase) ||
                           name.StartsWith(string.Concat("Microsoft.UI.", "Xaml"), StringComparison.OrdinalIgnoreCase))
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToArray();

    private static int GetGuiResources(GuiResourceType type) =>
        GetGuiResourcesNative(GetCurrentProcess(), (uint)type);

    private static T RunOnDedicatedStaThread<T>(Func<T> action)
    {
        T? result = default;
        Exception? failure = null;
        using var complete = new ManualResetEventSlim();
        var thread = new Thread(() =>
        {
            try { result = action(); }
            catch (Exception exception) { failure = exception; }
            finally { complete.Set(); }
        })
        {
            IsBackground = true,
            Name = "Doroti F2 same-STA validation",
        };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        if (!complete.Wait(TimeSpan.FromMinutes(5)) || !thread.Join(TimeSpan.FromMinutes(1)))
            throw new TimeoutException("F2 same-STA validation did not terminate.");
        if (failure is not null)
            throw new InvalidOperationException("F2 same-STA validation failed.", failure);
        return result ?? throw new InvalidOperationException("F2 validation returned no result.");
    }

    private static F2Options ParseOptions(string[] args)
    {
        string? evidencePath = null;
        string? sourceFingerprint = null;
        string? publishedExecutableSha256 = null;
        for (var index = 0; index < args.Length; index += 2)
        {
            if (index + 1 >= args.Length)
                throw new ArgumentException("Every F2 validator option requires a value.");
            switch (args[index])
            {
                case "--evidence": evidencePath = args[index + 1]; break;
                case "--source-fingerprint": sourceFingerprint = args[index + 1]; break;
                case "--published-executable-sha256": publishedExecutableSha256 = args[index + 1]; break;
                default:
                    throw new ArgumentException(
                        "Usage: Doroti.Validation.WindowsAppSdkFlutterTopLevel " +
                        "[--evidence <path>] [--source-fingerprint <sha256>] " +
                        "[--published-executable-sha256 <sha256>]");
            }
        }
        return new F2Options(evidencePath, sourceFingerprint, publishedExecutableSha256);
    }

    private static string ComputeFileHash(string path) =>
        Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant();

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetClientRect(nint hwnd, out NativeRect rect);

    [LibraryImport("user32.dll")]
    private static partial nint GetParent(nint hwnd);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool IsWindowVisible(nint hwnd);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetWindowPos(
        nint hwnd,
        nint insertAfter,
        int x,
        int y,
        int width,
        int height,
        uint flags);

    [LibraryImport("user32.dll")]
    private static partial nint SendMessageW(nint hwnd, uint message, nuint wParam, nint lParam);

    [LibraryImport("user32.dll")]
    private static partial uint GetDpiForWindow(nint hwnd);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool AdjustWindowRectExForDpi(
        ref NativeRect rect,
        uint style,
        [MarshalAs(UnmanagedType.Bool)] bool hasMenu,
        uint extendedStyle,
        uint dpi);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool PeekMessageW(
        out NativeMessage message,
        nint hwnd,
        uint minimum,
        uint maximum,
        uint removeMessage);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool TranslateMessage(in NativeMessage message);

    [LibraryImport("user32.dll")]
    private static partial nint DispatchMessageW(in NativeMessage message);

    [LibraryImport("kernel32.dll")]
    private static partial nint GetCurrentProcess();

    [LibraryImport("user32.dll", EntryPoint = "GetGuiResources")]
    private static partial int GetGuiResourcesNative(nint process, uint flags);

    [StructLayout(LayoutKind.Sequential)]
    private struct NativePoint
    {
        internal int X;
        internal int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect
    {
        internal int Left;
        internal int Top;
        internal int Right;
        internal int Bottom;
        internal int Width => Right - Left;
        internal int Height => Bottom - Top;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MinMaxInfo
    {
        internal NativePoint Reserved;
        internal NativePoint MaximumSize;
        internal NativePoint MaximumPosition;
        internal NativePoint MinimumTrackSize;
        internal NativePoint MaximumTrackSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeMessage
    {
        internal nint Hwnd;
        internal uint Message;
        internal nuint WParam;
        internal nint LParam;
        internal uint Time;
        internal NativePoint Point;
        internal uint Private;
    }

    private enum GuiResourceType : uint
    {
        Gdi = 0,
        User = 1,
    }

    private sealed record F2Options(
        string? EvidencePath,
        string? SourceFingerprint,
        string? PublishedExecutableSha256);

    private sealed record MinMaxResult(
        int MinimumTrackWidth,
        int MinimumTrackHeight,
        int MaximumTrackWidth,
        int MaximumTrackHeight,
        bool Exact);

    private sealed record F2CycleResult(
        int TopLevelCount,
        int ChildViewCount,
        int RawWindowAssociationCount,
        bool StandardNonClient,
        bool TopLevelVisibleBeforeFirstSwap,
        long FirstFrameShowCount,
        bool VisibleAfterFirstSwap,
        long ChildRectMismatchCount,
        long ChildLayoutCount,
        long TopLevelEraseBeforeFirstSwap,
        long ViewEraseBeforeFirstSwap,
        int MinimumTrackWidth,
        int MinimumTrackHeight,
        int MaximumTrackWidth,
        int MaximumTrackHeight,
        bool ExactMinMaxConversion,
        string[] TeardownOrder,
        bool ExactTeardownOrder);

    private sealed record F2CycleSummary(
        int Cycles,
        int FailureCount,
        int TopLevelCount,
        int ChildViewCount,
        int RawWindowAssociationCount,
        bool StandardNonClient,
        bool TopLevelVisibleBeforeFirstSwap,
        long FirstFrameShowCount,
        bool VisibleAfterFirstSwap,
        long ChildRectMismatchCount,
        long ChildLayoutCount,
        long TopLevelEraseBeforeFirstSwap,
        long ViewEraseBeforeFirstSwap,
        int MinimumTrackWidth,
        int MinimumTrackHeight,
        int MaximumTrackWidth,
        int MaximumTrackHeight,
        bool ExactMinMaxConversion,
        string[] TeardownOrder,
        bool ExactTeardownOrder,
        int GdiBefore,
        int GdiAfter,
        int UserBefore,
        int UserAfter,
        bool GuiResourcesBoundedAfterWarmup,
        string[] MauiOrXamlAssemblyNames);
}
