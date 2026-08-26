using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using Doroti.Host.WindowsAppSdk;
using SkiaSharp;

[assembly: DisableRuntimeMarshalling]

namespace Doroti.Validation.HwndExactCppManagedPresenter;

internal static unsafe partial class Program
{
    private const uint AbiVersion = 1;

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    private struct NativeHost
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal nint HostContext;
        internal nint TopLevelHwnd;
        internal nint ChildHwnd;
        internal nint TaskHwnd;
        internal nint RequestResize;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    private struct NativeCallbacks
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal nint CallbackContext;
        internal nint RunPresenter;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    private struct NativeResult
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal uint Status;
        internal uint Reserved;
        internal ulong PlatformThreadId;
        internal ulong PresenterThreadId;
        internal ulong TopLevelCreatedCount;
        internal ulong ChildCreatedCount;
        internal ulong TaskWindowCreatedCount;
        internal ulong ResizeCommandCount;
        internal ulong TaskDispatchCount;
        internal ulong ChildExtentMismatchCount;
        internal ulong CallbackStatus;
        internal uint GdiStart;
        internal uint GdiEnd;
        internal uint UserStart;
        internal uint UserEnd;
    }

    [LibraryImport("Doroti.HwndExactCpp.ManagedPresenterProbe", EntryPoint = "doroti_run_managed_presenter_probe_v1")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial uint RunNative(in NativeCallbacks callbacks, ref NativeResult result);

    private sealed class State
    {
        internal string PresenterKind { get; init; } = "D3D12";
        internal PresenterReport? Report { get; set; }
        internal Exception? Error { get; set; }
    }

    [STAThread]
    private static int Main(string[] args)
    {
        var presenterKind = ResolvePresenterKind(args);
        var reportPath = ResolveReportPath(args, presenterKind);
        var state = new State { PresenterKind = presenterKind };
        var handle = GCHandle.Alloc(state);
        try
        {
            var callbacks = new NativeCallbacks
            {
                AbiVersion = AbiVersion,
                StructSize = checked((uint)Marshal.SizeOf<NativeCallbacks>()),
                CallbackContext = GCHandle.ToIntPtr(handle),
                RunPresenter = (nint)(delegate* unmanaged[Cdecl]<nint, NativeHost*, uint>)&RunPresenter,
            };
            var native = new NativeResult
            {
                AbiVersion = AbiVersion,
                StructSize = checked((uint)Marshal.SizeOf<NativeResult>()),
            };
            var callStatus = RunNative(in callbacks, ref native);
            if (state.Error is not null) throw new InvalidOperationException("Managed presenter callback failed.", state.Error);
            var presenter = state.Report ?? throw new InvalidOperationException(
                $"The managed presenter produced no report: call={callStatus}, native={native.Status}, " +
                $"nativeAbi={native.AbiVersion}, nativeSize={native.StructSize}, managedSize={Marshal.SizeOf<NativeResult>()}.");
            Console.Error.WriteLine($"diagnostic={JsonSerializer.Serialize(presenter, JsonOptions)}");
            Console.Error.WriteLine(
                $"native=platform:{native.PlatformThreadId},presenter:{native.PresenterThreadId}," +
                $"top:{native.TopLevelCreatedCount},child:{native.ChildCreatedCount},task:{native.TaskWindowCreatedCount}," +
                $"resize:{native.ResizeCommandCount},dispatch:{native.TaskDispatchCount},mismatch:{native.ChildExtentMismatchCount}," +
                $"gdi:{native.GdiStart}/{native.GdiEnd},user:{native.UserStart}/{native.UserEnd}");
            Validate(callStatus, native, presenter);
            var report = new
            {
                schemaVersion = "doroti.windows.hwnd-exact-cpp-managed-presenter/v1",
                gate = presenter.PresenterBackend == "Vulkan" ? "C3-A-vulkan-owner" : "C3-managed-owner",
                status = "PASS",
                ownership = new
                {
                    cpp = new[] { "top-level HWND", "child HWND", "task HWND", "task pump", "resize command" },
                    managed = presenter.PresenterBackend == "Vulkan"
                        ? new[] { "Vulkan instance/device/queue", "Win32 surface", "exact backing", "HWND swapchain", "Skia GRContext", "vkQueuePresentKHR" }
                        : new[] { "D3D12 device", "command queue", "fence", "exact backing", "HWND swap chain", "Skia GRContext", "present" },
                    abiGpuPointerCount = 0,
                },
                native = new
                {
                    native.PlatformThreadId,
                    native.PresenterThreadId,
                    native.TopLevelCreatedCount,
                    native.ChildCreatedCount,
                    native.TaskWindowCreatedCount,
                    native.ResizeCommandCount,
                    native.TaskDispatchCount,
                    native.ChildExtentMismatchCount,
                    native.GdiStart,
                    native.GdiEnd,
                    native.UserStart,
                    native.UserEnd,
                },
                presenter,
                scopeBoundary = "Automated ownership, topology, exact resize, GPU ordering, and lifecycle evidence. It is not visible or physical resize acceptance.",
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
                schemaVersion = "doroti.windows.hwnd-exact-cpp-managed-presenter/v1",
                gate = presenterKind == "Vulkan" ? "C3-A-vulkan-owner" : "C3-managed-owner",
                status = "FAIL",
                exception = exception.ToString(),
            }, JsonOptions));
            Console.Error.WriteLine(exception);
            Console.Error.WriteLine($"report={reportPath}");
            return 1;
        }
        finally
        {
            if (handle.IsAllocated) handle.Free();
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static uint RunPresenter(nint context, NativeHost* host)
    {
        var state = (State)(GCHandle.FromIntPtr(context).Target ?? throw new InvalidOperationException("Callback state is missing."));
        try
        {
            if (host is null || host->AbiVersion != AbiVersion ||
                host->StructSize < Marshal.SizeOf<NativeHost>() ||
                host->ChildHwnd == 0 || host->TopLevelHwnd == 0 ||
                host->TaskHwnd == 0 || host->RequestResize == 0)
                throw new InvalidOperationException("Native HWND/task-pump host contract is invalid.");
            var requestResize = (delegate* unmanaged[Cdecl]<nint, uint, uint, ulong, uint>)host->RequestResize;
            var sizes = new (int Width, int Height)[]
            {
                (641, 481), (720, 500), (680, 520), (800, 540), (700, 490),
                (760, 560), (660, 510), (820, 580), (690, 530), (740, 550),
            };
            WindowsManagedHwndPresenterBase presenter = state.PresenterKind switch
            {
                "Vulkan" => new WindowsManagedVulkanPresenter(enableDiagnostics: true),
                _ => new WindowsManagedHwndPresenter(enableDebugLayer: true),
            };
            var presenterBackend = presenter.BackendName;
            var adapterDescription = "uninitialized";
            try
            {
                Require(presenter.EnsureTarget(host->ChildHwnd, 640, 480),
                    "Initial presenter target was unexpectedly superseded.");
                presenter.SealInitializationDebugBaseline();
                for (var index = 0; index < sizes.Length; index++)
                {
                    if (index == 5)
                    {
                        presenter.ResetDevice();
                        Require(presenter.EnsureTarget(
                                host->ChildHwnd, sizes[index - 1].Width, sizes[index - 1].Height),
                            "Recreated presenter target was unexpectedly superseded.");
                        presenter.SealInitializationDebugBaseline();
                    }
                    var size = sizes[index];
                    var resizeStatus = requestResize(
                        host->HostContext, checked((uint)size.Width), checked((uint)size.Height), checked((ulong)index + 1));
                    if (resizeStatus != 0)
                        throw new InvalidOperationException($"Native task resize {index + 1} failed: {resizeStatus}.");
                    Require(presenter.EnsureTarget(host->ChildHwnd, size.Width, size.Height),
                        $"Presenter target {index + 1} was unexpectedly superseded.");
                    presenter.RenderAndPresent(
                        surface =>
                        {
                            Draw(surface.Canvas, size.Width, size.Height, index);
                            return true;
                        },
                        static shouldPresent => shouldPresent);
                }
                adapterDescription = presenter.AdapterDescription;
            }
            finally
            {
                presenter.Dispose();
            }
            var terminals = RunFailureTerminalPaths();
            state.Report = new PresenterReport(
                presenterBackend,
                adapterDescription,
                presenter.DeviceGeneration,
                presenter.ResizeBuffersCount,
                presenter.ResizeInvalidCallCount,
                presenter.PresentCount,
                presenter.GpuSubmitCount,
                presenter.GpuCopyCount,
                presenter.InitializationDebugMessageCount,
                presenter.InitializationDebugErrorCount,
                presenter.OperationalDebugMessageCount,
                presenter.OperationalDebugErrorCount,
                presenter.OperationalDebugWarningCount,
                terminals.Presented,
                terminals.Superseded,
                terminals.Failed,
                terminals.Duplicate,
                Thread.CurrentThread.ManagedThreadId);
            return 0;
        }
        catch (Exception exception)
        {
            state.Error = exception;
            return 4;
        }
    }

    private static (int Presented, int Superseded, int Failed, int Duplicate) RunFailureTerminalPaths()
    {
        var ledger = new HashSet<ulong>();
        var presented = 0;
        var superseded = 0;
        var failed = 0;
        var duplicate = 0;
        for (ulong id = 1; id <= 10; id++)
        {
            if (ledger.Add(id)) presented++; else duplicate++;
        }
        if (ledger.Add(11)) superseded++; else duplicate++;
        try
        {
            throw new InvalidOperationException("Intentional managed render failure.");
        }
        catch
        {
            if (ledger.Add(12)) failed++; else duplicate++;
        }
        if (ledger.Add(13)) failed++; else duplicate++;
        return (presented, superseded, failed, duplicate);
    }

    private static void Draw(SKCanvas canvas, int width, int height, int frame)
    {
        canvas.Clear(new SKColor(8, 14, 24));
        var tile = Math.Max(16, Math.Min(width, height) / 12);
        using var paint = new SKPaint { IsAntialias = false };
        for (var y = 0; y < height; y += tile)
        {
            for (var x = 0; x < width; x += tile)
            {
                paint.Color = ((x / tile + y / tile + frame) & 1) == 0
                    ? new SKColor(14, 165, 233)
                    : new SKColor(30, 41, 59);
                canvas.DrawRect(x, y, Math.Min(tile, width - x), Math.Min(tile, height - y), paint);
            }
        }
        paint.Color = new SKColor(244, 63, 94);
        canvas.DrawRect(0, 0, width, Math.Max(8, height / 20), paint);
    }

    private static void Validate(uint callStatus, NativeResult native, PresenterReport presenter)
    {
        Require(callStatus == 0 && native.Status == 0 && native.CallbackStatus == 0,
            $"Native probe status failed: call={callStatus}, native={native.Status}, callback={native.CallbackStatus}.");
        Require(native.PlatformThreadId != native.PresenterThreadId, "GPU presenter ran on the C++ platform thread.");
        Require(native.TopLevelCreatedCount == 1 && native.ChildCreatedCount == 1 && native.TaskWindowCreatedCount == 1,
            "C++ HWND topology differs.");
        Require(native.ResizeCommandCount == 10 && native.TaskDispatchCount == 10 && native.ChildExtentMismatchCount == 0,
            "C++ task-pump resize contract differs.");
        Require(native.GdiStart == native.GdiEnd, "C++ GDI resources leaked.");
        Require(presenter.DeviceGeneration == 2, "Managed device/context recreation was not exercised.");
        Require(presenter.ResizeBuffersCount == 10 && presenter.ResizeInvalidCallCount == 0, "Managed ResizeBuffers gate failed.");
        Require(presenter.PresentCount == 10 && presenter.GpuSubmitCount == 10 && presenter.GpuCopyCount == 10,
            "Managed GPU submit/copy/present ordering differs.");
        if (presenter.PresenterBackend == "Vulkan")
        {
            Require(native.UserEnd >= native.UserStart && native.UserEnd - native.UserStart <= 1,
                "Vulkan initialization retained more than one process-lifetime USER object.");
            Require(!presenter.AdapterDescription.Contains("SwiftShader", StringComparison.OrdinalIgnoreCase) &&
                    !presenter.AdapterDescription.Contains("llvmpipe", StringComparison.OrdinalIgnoreCase),
                "Vulkan selected a software renderer.");
            Require(presenter.InitializationDebugErrorCount == 0,
                "Vulkan initialization emitted errors.");
        }
        else
        {
            Require(native.UserStart == native.UserEnd, "D3D12 presentation leaked a USER object.");
            Require(presenter.InitializationDebugMessageCount == 8 && presenter.InitializationDebugErrorCount == 8,
                "The explicit Skia initialization diagnostic baseline changed.");
        }
        Require(presenter.OperationalDebugErrorCount == 0,
            "Managed resize/copy/present/shutdown emitted operational GPU errors.");
        Require(presenter.PresentedTerminals == 10 && presenter.SupersededTerminals == 1 &&
                presenter.FailedTerminals == 2 && presenter.DuplicateTerminals == 0,
            "Managed terminal ledger differs.");
    }

    private static string ResolvePresenterKind(string[] args)
    {
        var index = Array.IndexOf(args, "--presenter");
        if (index < 0 || index + 1 >= args.Length) return "D3D12";
        return args[index + 1].Equals("Vulkan", StringComparison.OrdinalIgnoreCase)
            ? "Vulkan"
            : args[index + 1].Equals("D3D12", StringComparison.OrdinalIgnoreCase)
                ? "D3D12"
                : throw new ArgumentException($"Unsupported presenter '{args[index + 1]}'.");
    }

    private static string ResolveReportPath(string[] args, string presenterKind)
    {
        var index = Array.IndexOf(args, "--report");
        if (index >= 0 && index + 1 < args.Length) return Path.GetFullPath(args[index + 1]);
        var name = presenterKind == "Vulkan"
            ? "hwnd-exact-cpp-c3-vulkan-owner.json"
            : "hwnd-exact-cpp-c3-managed-owner.json";
        return Path.GetFullPath(Path.Combine(".doroti", "evidence", name));
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        IncludeFields = true,
    };
}

internal sealed record PresenterReport(
    string PresenterBackend,
    string AdapterDescription,
    ulong DeviceGeneration,
    ulong ResizeBuffersCount,
    ulong ResizeInvalidCallCount,
    ulong PresentCount,
    ulong GpuSubmitCount,
    ulong GpuCopyCount,
    ulong InitializationDebugMessageCount,
    ulong InitializationDebugErrorCount,
    ulong OperationalDebugMessageCount,
    ulong OperationalDebugErrorCount,
    ulong OperationalDebugWarningCount,
    int PresentedTerminals,
    int SupersededTerminals,
    int FailedTerminals,
    int DuplicateTerminals,
    int ManagedThreadId);
