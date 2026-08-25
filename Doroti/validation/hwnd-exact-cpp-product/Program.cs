using System.Text.Json;
using Doroti.Host.WindowsAppSdk;
using Doroti.Hosting;
using Doroti.Ui;
using IoPath = System.IO.Path;

namespace Doroti.Validation.HwndExactCppProduct;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        var reportPath = ResolveReportPath(args);
        Environment.SetEnvironmentVariable("DOROTI_WINDOWS_ADAPTER", "HwndExactCpp");
        Environment.SetEnvironmentVariable("DOROTI_WINDOWS_PRESENTER", "AngleD3D11");
        Environment.SetEnvironmentVariable("DOROTI_WINDOWS_APPSDK_SMOKE_MS", "5000");
        Environment.SetEnvironmentVariable("DOROTI_WINDOWS_APPSDK_DIAGNOSTICS", "1");
        Environment.SetEnvironmentVariable("DOROTI_WINDOWS_APPSDK_INPUT_SMOKE", "1");
        try
        {
            var descriptor = DorotiApplicationFactory.Create<ProductStartup>(
                DorotiLaunchContext.Create("WindowsAppSdk", "win-x64"),
                manifestAssembly: typeof(Program).Assembly);
            var exitCode = DorotiWindowsAppSdkRunner.Run(descriptor);
            var diagnostics = DorotiWindowsAppSdkRunner.LastRunDiagnostics ??
                throw new InvalidOperationException("Product runner did not publish diagnostics.");
            Require(exitCode == 0, "Product runner exit code differs.");
            Require(ProductEntrypoint.AttachCount == 1 && ProductEntrypoint.DetachCount == 1 &&
                    ProductEntrypoint.DrawCount >= 1 && ProductEntrypoint.ShutdownCount == 1,
                "Framework session/view lifecycle did not complete exactly once.");
            Require(diagnostics.RenderCallbacks >= 1 && diagnostics.PresentedTerminals >= 1 &&
                    diagnostics.FailedTerminals == 0, "Product frame terminal coverage failed.");
            Require(diagnostics.PlatformThreadId != 0 && diagnostics.RasterThreadId != 0 &&
                    diagnostics.InputThreadId == diagnostics.PlatformThreadId &&
                    diagnostics.RasterThreadId != diagnostics.PlatformThreadId,
                "C6 platform/input pump was not isolated from managed raster presentation.");
            Require(diagnostics.VisibleAfterExactPresent, "The native window was not shown after an exact frame.");
            Require(diagnostics.AcceptedResizeGenerations >= 1 && diagnostics.UnterminatedResizeGenerations == 0 &&
                    diagnostics.DuplicateResizeTerminals == 0, "Product resize generation did not drain exactly once.");
            Require(diagnostics.AcceptedResizeGenerations >= 2 &&
                    diagnostics.AcceptedResizeGenerations < 20 &&
                    diagnostics.PresentedResizeGenerations == diagnostics.AcceptedResizeGenerations,
                "The C5-A resize burst did not coalesce to exact raster admissions.");
            Require(diagnostics.DeviceGenerations == 1 && diagnostics.Presents >= 1 &&
                    diagnostics.Presents == diagnostics.GpuSubmits && diagnostics.GpuCopies == 0,
                "Managed presenter ordering differs in the product path.");
            Require(diagnostics.PresenterBackend == "ANGLE/EGL-D3D11",
                "Product validation did not select the managed ANGLE/EGL-D3D11 presenter.");
            Require(diagnostics.AdapterDescription.Contains("ANGLE", StringComparison.OrdinalIgnoreCase) &&
                    (diagnostics.AdapterDescription.Contains("D3D11", StringComparison.OrdinalIgnoreCase) ||
                     diagnostics.AdapterDescription.Contains("Direct3D11", StringComparison.OrdinalIgnoreCase)),
                "Product validation did not bind ANGLE to D3D11.");
            Require(diagnostics.OperationalDebugErrors == 0, "Product presentation emitted operational EGL/GLES errors.");
            var layout = WindowsNativeV1.ValidateLayout();
            Require(layout.GpuPointerCount == 0, "Product ABI exposes a GPU pointer.");
            Require(ProductEntrypoint.PointerChanges.Take(5).SequenceEqual([
                        PointerChange.add, PointerChange.hover, PointerChange.down,
                        PointerChange.move, PointerChange.up]),
                "Synthetic pointer lifecycle or re-entry coordinates differ.");
            Require(ProductEntrypoint.KeyTypes.SequenceEqual([KeyEventType.down, KeyEventType.up]),
                "Synthetic keyboard lifecycle differs.");
            Require(ProductEntrypoint.FocusStates.Count >= 2 &&
                    ProductEntrypoint.FocusStates.First() && !ProductEntrypoint.FocusStates.Last(),
                "Synthetic focus lifecycle differs.");
            Require(ProductEntrypoint.InputDispatchThreadIds.Count == 1 &&
                    ProductEntrypoint.DrawThreadIds.SetEquals(ProductEntrypoint.InputDispatchThreadIds),
                "Doroti input dispatch did not run on the managed framework/raster worker.");
            Require(ProductEntrypoint.ClipboardRoundTrip == "Doroti C6 한글 clipboard",
                "UTF-8/Unicode clipboard round-trip differs.");

            var report = new
            {
                schemaVersion = "doroti.windows.hwnd-exact-cpp-product-validation/v1",
                gate = "C5-A",
                status = "PASS",
                exitCode,
                framework = new
                {
                    ProductEntrypoint.AttachCount,
                    ProductEntrypoint.DetachCount,
                    ProductEntrypoint.DrawCount,
                    ProductEntrypoint.ShutdownCount,
                },
                diagnostics,
                resizeRequests = 20,
                abiGpuPointerCount = layout.GpuPointerCount,
                scopeBoundary = "Automated product bootstrap, framework scene, managed ANGLE/EGL-D3D11 exact presentation, and clean close. Visible resize behavior remains notVerified.",
            };
            Write(reportPath, report);
            var c6ReportPath = IoPath.GetFullPath(IoPath.Combine(".doroti", "evidence", "hwnd-exact-cpp-c6-input.json"));
            Write(c6ReportPath, new
            {
                schemaVersion = "doroti.windows.hwnd-exact-cpp-input-validation/v1",
                gate = "C6-automated",
                status = "PASS",
                pointerChanges = ProductEntrypoint.PointerChanges,
                keyTypes = ProductEntrypoint.KeyTypes,
                focusStates = ProductEntrypoint.FocusStates,
                ProductEntrypoint.ClipboardRoundTrip,
                cursorRequest = "PASS",
                packetAbi = new { layout.PointerPacketSize, layout.KeySize },
                threadOwnership = new
                {
                    diagnostics.PlatformThreadId,
                    diagnostics.InputThreadId,
                    diagnostics.RasterThreadId,
                    managedInputDispatchThreadIds = ProductEntrypoint.InputDispatchThreadIds,
                    managedDrawThreadIds = ProductEntrypoint.DrawThreadIds,
                },
                scopeBoundary = "Automated WndProc packet, coordinates, capture sequence, cursor request, focus, key, and clipboard contract. Physical mouse/cursor/focus checks remain notVerified.",
            });
            Console.WriteLine(JsonSerializer.Serialize(report, JsonOptions));
            Console.WriteLine($"report={reportPath}");
            return 0;
        }
        catch (Exception exception)
        {
            Write(reportPath, new
            {
                schemaVersion = "doroti.windows.hwnd-exact-cpp-product-validation/v1",
                gate = "C5-A",
                status = "FAIL",
                exception = exception.ToString(),
            });
            Console.Error.WriteLine(exception);
            Console.Error.WriteLine($"report={reportPath}");
            return 1;
        }
    }

    private static void Write(string path, object value)
    {
        Directory.CreateDirectory(IoPath.GetDirectoryName(path)!);
        File.WriteAllText(path, JsonSerializer.Serialize(value, JsonOptions));
    }

    private static string ResolveReportPath(string[] args)
    {
        var index = Array.IndexOf(args, "--report");
        if (index >= 0 && index + 1 < args.Length) return IoPath.GetFullPath(args[index + 1]);
        return IoPath.GetFullPath(IoPath.Combine(".doroti", "evidence", "hwnd-exact-cpp-c5-angle-product.json"));
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
}

public sealed class ProductStartup : IDorotiApplicationStartup
{
    public void Configure(DorotiApplicationBuilder builder) => builder
        .UseEntrypoint(() => new ProductEntrypoint())
        .UseView(new DorotiViewConfiguration("Doroti C5-A managed ANGLE presenter", new Size(640, 480)));
}

public sealed class ProductEntrypoint : IDorotiViewEntrypoint
{
    private PlatformDispatcher? _dispatcher;
    private DorotiView? _view;
    public static int AttachCount;
    public static int DetachCount;
    public static int DrawCount;
    public static int ShutdownCount;
    public static List<PointerChange> PointerChanges { get; } = [];
    public static List<KeyEventType> KeyTypes { get; } = [];
    public static List<bool> FocusStates { get; } = [];
    public static HashSet<int> InputDispatchThreadIds { get; } = [];
    public static HashSet<int> DrawThreadIds { get; } = [];
    public static string? ClipboardRoundTrip;
    private IPlatformServicesHostCapability? _services;

    public void Bootstrap(PlatformDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
        dispatcher.onDrawFrame = Draw;
        dispatcher.onMetricsChanged = view =>
            view.ScheduleFrame(DartUiInvocation.Managed("c5-product#metrics"));
        dispatcher.onPointerDataPacket = (_, packet) =>
        {
            InputDispatchThreadIds.Add(Environment.CurrentManagedThreadId);
            foreach (var pointer in packet.data)
            {
                if (pointer.physicalX < 0 || pointer.physicalY < 0)
                    throw new InvalidOperationException("Pointer coordinates escaped the child client.");
                PointerChanges.Add(pointer.change);
            }
        };
        dispatcher.onKeyData = key =>
        {
            InputDispatchThreadIds.Add(Environment.CurrentManagedThreadId);
            KeyTypes.Add(key.type);
            return true;
        };
        dispatcher.onFocusData = (_, focus) =>
        {
            InputDispatchThreadIds.Add(Environment.CurrentManagedThreadId);
            FocusStates.Add(focus.isFocused);
        };
    }

    public void AttachView(DorotiView view)
    {
        _view = view;
        Interlocked.Increment(ref AttachCount);
        var services = view.RequireCapability<IPlatformServicesHostCapability>(
            DorotiCapabilityIds.PlatformServices, DartUiInvocation.Managed("c6-product#services"));
        _services = services;
        services.SetCursor(DorotiMouseCursorKind.click);
        services.SetClipboardTextAsync("Doroti C6 한글 clipboard").AsTask().GetAwaiter().GetResult();
        for (var index = 0; index < 20; index++)
        {
            var direction = index < 10 ? index : 19 - index;
            view.Resize(new Size(640 + direction * 37, 480 + direction * 23));
        }
        view.ScheduleFrame(DartUiInvocation.Managed("c5-product#AttachView"));
    }

    private void Draw()
    {
        DrawThreadIds.Add(Environment.CurrentManagedThreadId);
        var view = _view ?? throw new InvalidOperationException("C5 view is unavailable.");
        ClipboardRoundTrip ??= (_services ?? throw new InvalidOperationException("C6 services are unavailable."))
            .GetClipboardTextAsync().AsTask().GetAwaiter().GetResult();
        using var scene = new Scene(view.viewId, []);
        view.render(scene);
        Interlocked.Increment(ref DrawCount);
    }

    public void DetachView(DorotiView view)
    {
        _ = view;
        _view = null;
        Interlocked.Increment(ref DetachCount);
    }

    public void Shutdown()
    {
        if (_dispatcher is not null) _dispatcher.onDrawFrame = null;
        if (_dispatcher is not null)
        {
            _dispatcher.onMetricsChanged = null;
            _dispatcher.onPointerDataPacket = null;
            _dispatcher.onKeyData = null;
            _dispatcher.onFocusData = null;
        }
        Interlocked.Increment(ref ShutdownCount);
    }
}
