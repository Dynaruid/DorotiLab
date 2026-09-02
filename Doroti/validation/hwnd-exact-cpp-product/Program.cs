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
        var requestedPresenter = ResolveOption(args, "--presenter") ?? "AngleD3D11";
        var injectedResult = ResolveOption(args, "--inject-vulkan-result");
        var resetCountText = ResolveOption(args, "--device-resets");
        var smokeMilliseconds = ResolveOption(args, "--smoke-ms") ?? "5000";
        var lifecycleCyclesText = ResolveOption(args, "--lifecycle-cycles") ?? "1";
        var lifecycleCycles = int.Parse(lifecycleCyclesText);
        var resizeCyclesText = ResolveOption(args, "--resize-cycles") ?? "0";
        var resizeCycles = int.Parse(resizeCyclesText);
        var externalResize = args.Contains("--external-resize", StringComparer.Ordinal);
        var skipResizeBurst = args.Contains("--no-resize-burst", StringComparer.Ordinal);
        Environment.SetEnvironmentVariable("DOROTI_WINDOWS_ADAPTER", "HwndExactCpp");
        Environment.SetEnvironmentVariable("DOROTI_WINDOWS_PRESENTER", requestedPresenter);
        Environment.SetEnvironmentVariable("DOROTI_WINDOWS_VULKAN_INJECT_RESULT", injectedResult);
        Environment.SetEnvironmentVariable("DOROTI_WINDOWS_VULKAN_INJECT_AFTER_PRESENTS",
            injectedResult is null ? null : "1");
        Environment.SetEnvironmentVariable("DOROTI_WINDOWS_APPSDK_DEVICE_RESET_COUNT", resetCountText);
        Environment.SetEnvironmentVariable("DOROTI_WINDOWS_APPSDK_LIFECYCLE_CYCLES", lifecycleCyclesText);
        Environment.SetEnvironmentVariable("DOROTI_WINDOWS_APPSDK_PRODUCT_RESIZE_CYCLES", resizeCyclesText);
        Environment.SetEnvironmentVariable("DOROTI_WINDOWS_APPSDK_EXTERNAL_RESIZE", externalResize ? "1" : null);
        Environment.SetEnvironmentVariable("DOROTI_WINDOWS_APPSDK_SKIP_RESIZE_BURST", skipResizeBurst ? "1" : null);
        Environment.SetEnvironmentVariable("DOROTI_WINDOWS_APPSDK_SMOKE_MS", smokeMilliseconds);
        Environment.SetEnvironmentVariable("DOROTI_WINDOWS_APPSDK_DIAGNOSTICS", "1");
        Environment.SetEnvironmentVariable("DOROTI_WINDOWS_APPSDK_INPUT_SMOKE", "1");
        Environment.SetEnvironmentVariable("DOROTI_WINDOWS_APPSDK_C7_SMOKE", "1");
        Environment.SetEnvironmentVariable("DOROTI_WINDOWS_APPSDK_C8_SMOKE", lifecycleCycles == 0 ? "0" : "1");
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
            var expectedRecoveryFailure = injectedResult is "DEVICE_LOST" or "SURFACE_LOST" ? 1 : 0;
            Require(diagnostics.RenderCallbacks >= 1 && diagnostics.PresentedTerminals >= 1 &&
                    diagnostics.FailedTerminals == expectedRecoveryFailure,
                "Product frame terminal coverage failed.");
            Require(diagnostics.PlatformThreadId != 0 && diagnostics.RasterThreadId != 0 &&
                    diagnostics.InputThreadId == diagnostics.PlatformThreadId &&
                    diagnostics.RasterThreadId != diagnostics.PlatformThreadId,
                "C6 platform/input pump was not isolated from managed raster presentation.");
            Require(diagnostics.VisibleAfterExactPresent, "The native window was not shown after an exact frame.");
            Require(diagnostics.AcceptedResizeGenerations >= 1 && diagnostics.UnterminatedResizeGenerations == 0 &&
                    diagnostics.DuplicateResizeTerminals == 0, "Product resize generation did not drain exactly once.");
            if (resizeCycles == 0 && !externalResize && !skipResizeBurst)
            {
                Require(diagnostics.AcceptedResizeGenerations >= 2 &&
                        diagnostics.AcceptedResizeGenerations < 20 &&
                        diagnostics.PresentedResizeGenerations >= 1 &&
                        diagnostics.PresentedResizeGenerations + diagnostics.SupersededResizeGenerations +
                            diagnostics.FailedResizeGenerations == diagnostics.AcceptedResizeGenerations &&
                        diagnostics.FailedResizeGenerations <= expectedRecoveryFailure,
                    "The C5-A presenter resize burst did not coalesce to exact raster admissions.");
            }
            else if (resizeCycles > 0)
            {
                var resizeSurfaceContract = diagnostics.Vulkan is { } resizeVulkan
                    ? resizeVulkan.SurfaceWidth >= resizeVulkan.Width &&
                      resizeVulkan.SurfaceHeight >= resizeVulkan.Height &&
                      resizeVulkan.SurfaceRecreates >= 1 &&
                      resizeVulkan.RetainedSurfaceReuses >= (ulong)Math.Max(0, resizeCycles - 1)
                    : diagnostics.ResizeBuffers >= (ulong)resizeCycles;
                Require(ProductEntrypoint.CompletedResizeRequests == resizeCycles &&
                        diagnostics.AcceptedResizeGenerations >= resizeCycles &&
                        diagnostics.PresentedResizeGenerations >= 1 &&
                        diagnostics.PresentedResizeGenerations + diagnostics.SupersededResizeGenerations +
                            diagnostics.FailedResizeGenerations == diagnostics.AcceptedResizeGenerations &&
                        resizeSurfaceContract &&
                        diagnostics.Presents >= (ulong)resizeCycles &&
                        diagnostics.GpuCopies >= (ulong)resizeCycles &&
                        diagnostics.FailedResizeGenerations <= expectedRecoveryFailure,
                    "The requested exact-size resize cycles did not each reach presentation.");
            }
            else if (externalResize)
            {
                Require(diagnostics.AcceptedResizeGenerations >= 2 &&
                        diagnostics.PresentedResizeGenerations >= 1 &&
                        diagnostics.PresentedResizeGenerations + diagnostics.SupersededResizeGenerations +
                            diagnostics.FailedResizeGenerations == diagnostics.AcceptedResizeGenerations &&
                        diagnostics.FailedResizeGenerations == expectedRecoveryFailure,
                    "External live resize generations did not drain one-to-one.");
            }
            else
            {
                Require(diagnostics.AcceptedResizeGenerations >= 1 &&
                        diagnostics.PresentedResizeGenerations + diagnostics.SupersededResizeGenerations +
                            diagnostics.FailedResizeGenerations == diagnostics.AcceptedResizeGenerations &&
                        diagnostics.FailedResizeGenerations <= expectedRecoveryFailure,
                    "No-burst qualification did not drain its resize generation one-to-one.");
            }
            var presentationContract = diagnostics.Vulkan is { } vulkan
                ? vulkan.OutstandingAcquired == 0 &&
                  vulkan.OutstandingCopySubmission <= 1 &&
                  vulkan.Acquired == vulkan.Presented &&
                  vulkan.Presented == diagnostics.GpuCopies &&
                  vulkan.DeferredCopySubmissions == diagnostics.GpuCopies &&
                  vulkan.SuccessfulPresents == diagnostics.Presents
                : diagnostics.Presents == diagnostics.GpuCopies;
            Require(diagnostics.CompletedDeviceResets == diagnostics.RequestedDeviceResets &&
                    diagnostics.DeviceGenerations ==
                        1UL + checked((ulong)diagnostics.CompletedDeviceResets) +
                        checked((ulong)diagnostics.VulkanDeviceLossRecoveries) +
                        checked((ulong)diagnostics.VulkanSurfaceLossRecoveries) &&
                    diagnostics.Presents >= 1 &&
                    presentationContract &&
                    diagnostics.GpuCopies <= diagnostics.GpuSubmits,
                "Managed presenter ordering or injected device recreation differs in the product path.");
            var expectedBackend = requestedPresenter.Equals("Vulkan", StringComparison.OrdinalIgnoreCase)
                ? "Vulkan" : "ANGLE/EGL-D3D11";
            Require(diagnostics.PresenterBackend == expectedBackend &&
                    diagnostics.RequestedPresenter.Equals(requestedPresenter, StringComparison.OrdinalIgnoreCase) &&
                    diagnostics.EffectivePresenter == expectedBackend,
                "Product validation did not select the explicitly requested presenter.");
            Require((expectedBackend == "Vulkan" ||
                     diagnostics.AdapterDescription.Contains("ANGLE", StringComparison.OrdinalIgnoreCase) &&
                     (diagnostics.AdapterDescription.Contains("D3D11", StringComparison.OrdinalIgnoreCase) ||
                      diagnostics.AdapterDescription.Contains("Direct3D11", StringComparison.OrdinalIgnoreCase))) &&
                    !diagnostics.AdapterDescription.Contains("SwiftShader", StringComparison.OrdinalIgnoreCase) &&
                    !diagnostics.AdapterDescription.Contains("WARP", StringComparison.OrdinalIgnoreCase) &&
                    !diagnostics.AdapterDescription.Contains("llvmpipe", StringComparison.OrdinalIgnoreCase),
                "Product validation did not bind the presenter to a hardware renderer.");
            Require(diagnostics.OperationalDebugErrors == 0, "Product presentation emitted operational GPU errors.");
            if (injectedResult == "DEVICE_LOST")
                Require(diagnostics.VulkanDeviceLossRecoveries == 1 && diagnostics.Vulkan is { DeviceLostResults: 1 },
                    "Injected Vulkan DEVICE_LOST did not take the single recovery branch.");
            if (injectedResult == "SURFACE_LOST")
                Require(diagnostics.VulkanSurfaceLossRecoveries == 1 && diagnostics.Vulkan is { SurfaceLostResults: 1 },
                    "Injected Vulkan SURFACE_LOST did not take the single recovery branch.");
            if (injectedResult == "OUT_OF_DATE")
                Require(diagnostics.Vulkan is { OutOfDateResults: >= 1 },
                    "Injected Vulkan OUT_OF_DATE did not take the acquire branch.");
            if (injectedResult == "SUBOPTIMAL")
                Require(diagnostics.Vulkan is { SuboptimalResults: >= 1 },
                    "Injected Vulkan SUBOPTIMAL did not take the present branch.");
            var layout = WindowsNativeV1.ValidateLayout();
            Require(layout.GpuPointerCount == 0, "Product ABI exposes a GPU pointer.");
            Require(ProductEntrypoint.PointerChanges.Take(5).SequenceEqual([
                        PointerChange.add, PointerChange.hover, PointerChange.down,
                        PointerChange.move, PointerChange.up]),
                "Synthetic pointer lifecycle or re-entry coordinates differ.");
            Require(ProductEntrypoint.WheelSignals == 1,
                "Synthetic mouse wheel did not cross the PointerSignalKind.scroll path exactly once.");
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
            Require(ProductEntrypoint.TextEditingStates.Any(state =>
                        state.text == "한" && state.composingRange == new DorotiTextSelection(0, 1)) &&
                    ProductEntrypoint.TextEditingStates.Any(state =>
                        state.text == "한글" && state.composingRange is null &&
                        state.selection == new DorotiTextSelection(2, 2)),
                "C7 IMM32 composition/editing transport differs.");
            Require(ProductEntrypoint.TextActions.Contains(DorotiTextInputAction.done),
                "C7 IME action transport differs.");
            Require(ProductEntrypoint.SemanticsActions.Any(action =>
                        action.nodeId == 0 && action.action == SemanticsAction.tap),
                "C7 UIA fragment invoke did not reach Doroti semantics dispatch.");
            Require(ProductEntrypoint.SemanticsActions.Any(action =>
                        action.nodeId == 1 && action.action == SemanticsAction.tap),
                "C7 UIA Toggle pattern did not reach Doroti semantics dispatch.");
            Require(ProductEntrypoint.SemanticsActions.Any(action =>
                        action.nodeId == 3 && action.action == SemanticsAction.tap),
                "C7 UIA SelectionItem pattern did not reach Doroti semantics dispatch.");
            Require(ProductEntrypoint.LifecycleStates.Contains(AppLifecycleState.detached),
                "C8 detach lifecycle event is missing.");
            if (lifecycleCycles > 0)
            {
                Require(new[] { AppLifecycleState.inactive, AppLifecycleState.hidden,
                                AppLifecycleState.paused, AppLifecycleState.resumed }
                        .All(ProductEntrypoint.LifecycleStates.Contains),
                    "C8 minimize/restore lifecycle sequence differs.");
                var inactiveCount = ProductEntrypoint.LifecycleStates.Count(state => state == AppLifecycleState.inactive);
                var hiddenCount = ProductEntrypoint.LifecycleStates.Count(state => state == AppLifecycleState.hidden);
                var pausedCount = ProductEntrypoint.LifecycleStates.Count(state => state == AppLifecycleState.paused);
                var resumedCount = ProductEntrypoint.LifecycleStates.Count(state => state == AppLifecycleState.resumed);
                Require(inactiveCount >= lifecycleCycles && hiddenCount >= lifecycleCycles &&
                        pausedCount >= lifecycleCycles && resumedCount >= lifecycleCycles,
                    $"Requested minimize/restore lifecycle cycles did not all cross the managed boundary " +
                    $"(inactive={inactiveCount}, hidden={hiddenCount}, paused={pausedCount}, resumed={resumedCount}, " +
                    $"requested={lifecycleCycles}).");
            }
            var provenance = diagnostics.NativeProvenance;
            var provenanceAudit = string.Equals(
                Environment.GetEnvironmentVariable("DOROTI_WINDOWS_NATIVE_AUDIT"),
                "1",
                StringComparison.Ordinal);
            Require(IoPath.GetDirectoryName(provenance.NativeHostPath) ==
                        provenance.ApplicationDirectory.TrimEnd(IoPath.DirectorySeparatorChar) &&
                    provenance.NativeHostLength > 0 &&
                    provenance.BootstrapLength > 0 &&
                    provenance.AngleRuntimeLength > 0 &&
                    provenance.FullHashAudit == provenanceAudit &&
                    (provenanceAudit
                        ? provenance.NativeHostSha256?.Length == 64 &&
                          provenance.BootstrapSha256?.Length == 64 &&
                          provenance.AngleRuntimeSha256?.Length == 64
                        : provenance.NativeHostSha256 is null &&
                          provenance.BootstrapSha256 is null &&
                          provenance.AngleRuntimeSha256 is null) &&
                    provenance.SearchPolicy.Contains("PATH/current-directory excluded", StringComparison.Ordinal),
                "C9 app-directory native provenance or restricted search policy differs.");

            var report = new
            {
                schemaVersion = "doroti.windows.hwnd-exact-cpp-product-validation/v2",
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
                lifecycleStates = ProductEntrypoint.LifecycleStates,
                diagnostics,
                resizeRequests = resizeCycles == 0 ? 20 : resizeCycles,
                completedResizeRequests = ProductEntrypoint.CompletedResizeRequests,
                externalResize,
                skipResizeBurst,
                injectedVulkanResult = injectedResult,
                lifecycleCycles,
                abiGpuPointerCount = layout.GpuPointerCount,
                scopeBoundary = $"Automated product bootstrap, framework scene, managed {expectedBackend} exact presentation, and clean close. Physical scan-out and human resize behavior remain notVerified.",
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
            var c7ReportPath = IoPath.GetFullPath(IoPath.Combine(".doroti", "evidence", "hwnd-exact-cpp-c7-ime-uia.json"));
            Write(c7ReportPath, new
            {
                schemaVersion = "doroti.windows.hwnd-exact-cpp-ime-uia-validation/v1",
                gate = "C7-automated",
                status = "PASS",
                ime = new
                {
                    states = ProductEntrypoint.TextEditingStates,
                    actions = ProductEntrypoint.TextActions,
                    transport = "IMM32 composition/result state through versioned native ABI",
                },
                uia = new
                {
                    root = "IRawElementProviderFragmentRoot",
                    fragment = "IRawElementProviderFragment",
                    patterns = new[] { "Invoke", "Value", "Toggle", "SelectionItem", "RangeValue" },
                    actions = ProductEntrypoint.SemanticsActions,
                    boundsAuthority = "current child HWND screen origin plus current metrics scale",
                },
                scopeBoundary = "Automated ABI, composition-state, UIA provider tree/bounds/pattern, and action dispatch contract. Physical Korean two-beolsik candidate UI, Narrator, and Accessibility Insights remain notVerified.",
            });
            var c8ReportPath = IoPath.GetFullPath(IoPath.Combine(".doroti", "evidence", "hwnd-exact-cpp-c8-lifecycle-device.json"));
            Write(c8ReportPath, new
            {
                schemaVersion = "doroti.windows.hwnd-exact-cpp-lifecycle-validation/v1",
                gate = "C8-automated-partial",
                status = "PASS",
                lifecycle = ProductEntrypoint.LifecycleStates,
                deviceGenerations = diagnostics.DeviceGenerations,
                diagnostics.AcceptedResizeGenerations,
                diagnostics.PresentedResizeGenerations,
                diagnostics.UnterminatedResizeGenerations,
                diagnostics.DuplicateResizeTerminals,
                scopeBoundary = "Automated minimize/restore/display-change/detach, ANGLE device recreation, and terminal drain only. DPI matrix, mixed-monitor, Snap/system-menu/keyboard sizing, injected hardware removal, visible first-frame/restore, and shutdown-at-each-wait-point remain notVerified.",
            });
            var c9ReportPath = IoPath.GetFullPath(IoPath.Combine(".doroti", "evidence", "hwnd-exact-cpp-c9-provenance.json"));
            Write(c9ReportPath, new
            {
                schemaVersion = "doroti.windows.hwnd-exact-cpp-provenance-validation/v1",
                gate = "C9-runtime-provenance",
                status = "PASS",
                adapter = "HwndExactCpp",
                nativeViewType = "Win32.ChildHwnd",
                graphicsBackend = diagnostics.PresenterBackend,
                diagnostics.AdapterDescription,
                provenance,
                abiGpuPointerCount = layout.GpuPointerCount,
                scopeBoundary = "Actual app-directory backend launch and restricted native search provenance. Clean publish and negative missing/wrong-architecture/wrong-version launch probes are recorded by the C9 publish gate.",
            });
            Console.WriteLine(JsonSerializer.Serialize(report, JsonOptions));
            Console.WriteLine($"report={reportPath}");
            return 0;
        }
        catch (Exception exception)
        {
            Write(reportPath, new
            {
                schemaVersion = "doroti.windows.hwnd-exact-cpp-product-validation/v2",
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

    private static string? ResolveOption(string[] args, string name)
    {
        var index = Array.IndexOf(args, name);
        if (index < 0) return null;
        if (index + 1 >= args.Length || string.IsNullOrWhiteSpace(args[index + 1]))
            throw new ArgumentException($"{name} requires a value.");
        return args[index + 1];
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
        .UseView(new DorotiViewConfiguration("Doroti C5-A presenter validation", new Size(640, 480)));
}

public sealed class ProductEntrypoint : IDorotiViewEntrypoint
{
    private PlatformDispatcher? _dispatcher;
    private DorotiView? _view;
    public static int AttachCount;
    public static int DetachCount;
    public static int DrawCount;
    public static int ShutdownCount;
    public static int CompletedResizeRequests;
    public static List<PointerChange> PointerChanges { get; } = [];
    public static int WheelSignals;
    public static List<KeyEventType> KeyTypes { get; } = [];
    public static List<bool> FocusStates { get; } = [];
    public static HashSet<int> InputDispatchThreadIds { get; } = [];
    public static HashSet<int> DrawThreadIds { get; } = [];
    public static string? ClipboardRoundTrip;
    public static List<DorotiTextEditingState> TextEditingStates { get; } = [];
    public static List<DorotiTextInputAction> TextActions { get; } = [];
    public static List<SemanticsActionEvent> SemanticsActions { get; } = [];
    public static List<AppLifecycleState> LifecycleStates { get; } = [];
    private IPlatformServicesHostCapability? _services;
    private ITextInputHostCapability? _textInput;
    private CancellationTokenSource? _qualificationFrames;

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
                if (pointer.signalKind == PointerSignalKind.scroll)
                {
                    Interlocked.Increment(ref WheelSignals);
                    (_view ?? throw new InvalidOperationException("C6 view is unavailable."))
                        .ScheduleFrame(DartUiInvocation.Managed("c6-product#wheel"));
                }
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
        dispatcher.onSemanticsActionEvent = action =>
        {
            InputDispatchThreadIds.Add(Environment.CurrentManagedThreadId);
            SemanticsActions.Add(action);
        };
        dispatcher.onAppLifecycleStateChanged = (_, state) => LifecycleStates.Add(state);
    }

    public void AttachView(DorotiView view)
    {
        _view = view;
        Interlocked.Increment(ref AttachCount);
        var services = view.RequireCapability<IPlatformServicesHostCapability>(
            DorotiCapabilityIds.PlatformServices, DartUiInvocation.Managed("c6-product#services"));
        _services = services;
        var textInput = view.RequireCapability<ITextInputHostCapability>(
            DorotiCapabilityIds.TextInput, DartUiInvocation.Managed("c7-product#text-input"));
        _textInput = textInput;
        textInput.EditingStateChanged += HandleEditingState;
        textInput.ActionPerformed += HandleTextAction;
        var configuration = new DorotiTextInputConfiguration(
            DorotiTextInputType.text, DorotiTextInputAction.done,
            DorotiTextCapitalization.none, false, false, true, true);
        try
        {
            textInput.SetClient(configuration, new("", new(-1, 0), null));
            throw new InvalidOperationException("A half-invalid text selection crossed the managed host boundary.");
        }
        catch (ArgumentOutOfRangeException)
        {
        }
        var emptyState = new DorotiTextEditingState("", new(-1, -1), null);
        textInput.SetClient(configuration, emptyState);
        textInput.UpdateState(emptyState);
        textInput.SetCaretRect(Rect.fromLTWH(24, 36, 2, 20));
        (_dispatcher ?? throw new InvalidOperationException("C7 dispatcher is unavailable."))
            .setSemanticsTreeEnabled(true);
        view.updateSemantics(new SemanticsUpdate(1,
        [
            new SemanticsNodeUpdate(0, Rect.fromLTWH(20, 30, 180, 48),
                "Doroti C7 action", "ready", SemanticsAction.tap | SemanticsAction.focus,
                [1, 2, 3, 4], new SemanticsFlags(isButton: true, isFocused: Tristate.isTrue)),
            new SemanticsNodeUpdate(1, Rect.fromLTWH(4, 54, 180, 40),
                "Doroti C7 toggle", null, SemanticsAction.tap,
                [], new SemanticsFlags(isChecked: CheckedState.isFalse, isFocused: Tristate.isFalse)),
            new SemanticsNodeUpdate(2, Rect.fromLTWH(4, 98, 180, 40),
                "Doroti C7 hidden", null, SemanticsAction.tap,
                [], new SemanticsFlags(isButton: true, isHidden: true)),
            new SemanticsNodeUpdate(3, Rect.fromLTWH(4, 142, 180, 40),
                "Doroti C7 radio", null, SemanticsAction.tap,
                [], new SemanticsFlags(isSelected: Tristate.isFalse,
                    isFocused: Tristate.isFalse, isInMutuallyExclusiveGroup: true)),
            new SemanticsNodeUpdate(4, Rect.fromLTWH(4, 186, 180, 40),
                "Doroti C7 slider", "0.2", SemanticsAction.increase | SemanticsAction.decrease,
                [], new SemanticsFlags(isSlider: true, isFocused: Tristate.isFalse),
                increasedValue: "0.3", decreasedValue: "0.1", minValue: "0", maxValue: "1"),
        ], SemanticsUpdateUrgency.immediate));
        services.SetCursor(DorotiMouseCursorKind.click);
        services.SetClipboardTextAsync("Doroti C6 한글 clipboard").AsTask().GetAwaiter().GetResult();
        var resizeCycles = int.TryParse(
            Environment.GetEnvironmentVariable("DOROTI_WINDOWS_APPSDK_PRODUCT_RESIZE_CYCLES"),
            out var configuredResizeCycles) ? configuredResizeCycles : 0;
        var externalResize = string.Equals(
            Environment.GetEnvironmentVariable("DOROTI_WINDOWS_APPSDK_EXTERNAL_RESIZE"),
            "1", StringComparison.Ordinal);
        var skipResizeBurst = string.Equals(
            Environment.GetEnvironmentVariable("DOROTI_WINDOWS_APPSDK_SKIP_RESIZE_BURST"),
            "1", StringComparison.Ordinal);
        if (resizeCycles == 0 && !externalResize && !skipResizeBurst)
        {
            for (var index = 0; index < 20; index++)
            {
                var direction = index < 10 ? index : 19 - index;
                view.Resize(new Size(640 + direction * 37, 480 + direction * 23));
            }
        }
        view.ScheduleFrame(DartUiInvocation.Managed("c5-product#AttachView"));
        var resetCount = int.TryParse(
            Environment.GetEnvironmentVariable("DOROTI_WINDOWS_APPSDK_DEVICE_RESET_COUNT"),
            out var configuredResetCount) ? configuredResetCount : 1;
        _qualificationFrames = new CancellationTokenSource();
        var token = _qualificationFrames.Token;
        _ = Task.Run(async () =>
        {
            await Task.Delay(1000, token).ConfigureAwait(false);
            var qualificationCycles = Math.Max(resetCount + 6, resizeCycles);
            for (var index = 0; index < qualificationCycles && !token.IsCancellationRequested; index++)
            {
                var resized = false;
                if (_view is { } current)
                {
                    if (index < resizeCycles)
                    {
                        var direction = index + 1;
                        current.Resize(new Size(640 + direction * 31, 480 + direction * 19));
                        Interlocked.Increment(ref CompletedResizeRequests);
                        resized = true;
                    }
                    if (!resized)
                        current.ScheduleFrame(DartUiInvocation.Managed("vulkan-product#qualification-frame"));
                }
                if (resized)
                {
                    await Task.Delay(450, token).ConfigureAwait(false);
                    if (_view is { } settledView)
                        settledView.ScheduleFrame(DartUiInvocation.Managed("vulkan-product#resize-settle-frame"));
                    await Task.Delay(550, token).ConfigureAwait(false);
                }
                else
                {
                    await Task.Delay(600, token).ConfigureAwait(false);
                }
            }
        }, token);
    }

    private void Draw()
    {
        DrawThreadIds.Add(Environment.CurrentManagedThreadId);
        var view = _view ?? throw new InvalidOperationException("C5 view is unavailable.");
        ClipboardRoundTrip ??= (_services ?? throw new InvalidOperationException("C6 services are unavailable."))
            .GetClipboardTextAsync().AsTask().GetAwaiter().GetResult();
        var recorder = new PictureRecorder();
        var canvas = new Canvas(recorder);
        canvas.drawColor(new Color(0xff10243a), BlendMode.src);
        canvas.drawRect(Rect.fromLTWH(32, 32, 220, 124),
            new Paint { color = new Color(0xff4fc3f7) });
        canvas.drawCircle(new Offset(312, 116), 72,
            new Paint { color = new Color(0x80ff4081) });
        using var picture = recorder.endRecording();
        var builder = new SceneBuilder(view.viewId);
        builder.addPicture(Offset.zero, picture);
        using var scene = builder.build();
        view.render(scene);
        Interlocked.Increment(ref DrawCount);
    }

    public void DetachView(DorotiView view)
    {
        _ = view;
        if (_textInput is { } textInput)
        {
            textInput.EditingStateChanged -= HandleEditingState;
            textInput.ActionPerformed -= HandleTextAction;
            textInput.ClearClient();
            _textInput = null;
        }
        _view = null;
        _qualificationFrames?.Cancel();
        _qualificationFrames?.Dispose();
        _qualificationFrames = null;
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
            _dispatcher.onSemanticsActionEvent = null;
            _dispatcher.onAppLifecycleStateChanged = null;
        }
        Interlocked.Increment(ref ShutdownCount);
    }

    private static void HandleEditingState(DorotiTextEditingState state)
    {
        InputDispatchThreadIds.Add(Environment.CurrentManagedThreadId);
        TextEditingStates.Add(state);
    }

    private static void HandleTextAction(DorotiTextInputAction action)
    {
        InputDispatchThreadIds.Add(Environment.CurrentManagedThreadId);
        TextActions.Add(action);
    }
}
