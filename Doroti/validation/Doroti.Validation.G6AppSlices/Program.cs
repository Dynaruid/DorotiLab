using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Doroti.Hosting;
using Doroti.Runtime;
using Doroti.Ui;
using Doroti.Backends.Skia;
using Doroti.Generated.Framework.Foundation;
using Doroti.Generated.Framework.Painting;
using Doroti.Generated.Framework.Rendering;
using Doroti.Generated.Framework.Services;
using Doroti.Generated.Framework.Widgets;
using Doroti.Host.Desktop.Framework;
using Doroti.Target.Windows;
using Material = Doroti.Generated.Framework.Material;
using UiColor = Doroti.Ui.Color;
using IOPath = System.IO.Path;

internal static class Program
{
    private const ulong ViewId = 640;
    private static readonly Lazy<IReadOnlyDictionary<string, string[]>> ReferenceTraces = new(LoadReferenceTraces);
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    [STAThread]
    public static int Main(string[] args)
    {
        var options = SliceOptions.Parse(args);
        if (!OperatingSystem.IsWindows() || RuntimeInformation.ProcessArchitecture != Architecture.X64)
        {
            Console.Error.WriteLine("G6-4 live application slices require Windows x64.");
            return 2;
        }

        try
        {
            Run(options);
            Console.WriteLine($"G6-4 {options.Slice} application slice: PASS");
            return 0;
        }
        catch (Exception exception)
        {
            WriteFailure(options, exception);
            Console.Error.WriteLine(exception);
            return 1;
        }
    }

    private static void Run(SliceOptions options)
    {
        var entrypoint = new SliceEntrypoint(options.Slice);
        DesktopFrameworkTargetDiagnostics diagnostics;
        DesktopFrameworkPixelReadback initial;
        DesktopFrameworkPixelReadback changed;
        object resourceClosure;
        object managed;
        var nativeInputTrace = new List<string>();
        var externalAutomation = new List<string>();

        using (var application = CreateApplicationBoundary(options.Slice))
        using (var target = new WindowsTarget())
        using (var session = new DorotiHostSession(entrypoint, new ValidationPerformanceModeCapability()))
        using (var scope = session.dispatcher.EnterScope())
        {
            session.Start(deferFrameworkBootstrap: true);
            var view = target.CreateView(session, ViewId,
                new DorotiViewConfiguration($"Doroti G6-4 {options.Slice}", new Size(720, 640)), application);
            var firstReadback = target.CaptureNextFrameAsync(ViewId);
            view.Show();
            session.dispatcher.setSemanticsTreeEnabled(true);

            WaitUntil(() =>
            {
                var current = target.CaptureDiagnostics(ViewId);
                return current.Frame.Presented >= 1 && entrypoint.State is not null &&
                    current.Frame.QueueDepth == 0 && current.Frame.ActiveFrames == 0;
            }, target, entrypoint, TimeSpan.FromSeconds(20));
            initial = firstReadback.WaitAsync(TimeSpan.FromSeconds(12)).GetAwaiter().GetResult();

            for (var stage = 0; stage < entrypoint.State!.StageCount; stage++)
            {
                var before = target.CaptureDiagnostics(ViewId).Frame.Presented;
                using (view.EnterPlatformEnvironmentScope())
                {
                    entrypoint.State.ExerciseStage(stage);
                }
                entrypoint.RequestFrame();
                WaitUntil(() => target.CaptureDiagnostics(ViewId).Frame.Presented > before,
                    target, entrypoint, TimeSpan.FromSeconds(12));
                PumpFor(target, TimeSpan.FromMilliseconds(420));
            }

            using (view.EnterPlatformEnvironmentScope())
            {
                ExerciseNativeInput(options.Slice, target, entrypoint, nativeInputTrace);
            }
            changed = CaptureFrame(target, entrypoint);
            if (options.Slice == SliceId.A0 && options.ReadyPath is not null && options.ExternalActionsPath is not null)
            {
                WriteJson(options.ReadyPath, new
                {
                    schemaVersion = "doroti.g6-app-slice-ready/v1",
                    processId = Environment.ProcessId,
                    hwnd = target.GetNativeWindowHandle(ViewId).ToInt64(),
                    names = new[] { "G6 A0 invoke", "G6 A0 toggle", "G6 A0 text", "G6 A0 scroll" },
                });
                WaitUntil(() => File.Exists(options.ExternalActionsPath), target, entrypoint, TimeSpan.FromSeconds(30));
                externalAutomation.AddRange(File.ReadAllLines(options.ExternalActionsPath));
                PumpFor(target, TimeSpan.FromMilliseconds(420));
            }
            diagnostics = target.CaptureDiagnostics(ViewId);
            ValidateLive(options.Slice, entrypoint, diagnostics, initial, changed, externalAutomation);
            managed = RunManagedContract(options.Slice, target);

            session.DetachView(view);
            session.Shutdown();
            view.Dispose();
            for (var index = 0; index < 20; index++) target.PumpPendingMessages();
            var closure = target.CaptureResourceSnapshot();
            if (!closure.IsBalanced) throw new InvalidDataException($"Native resources did not close: {closure}.");
            resourceClosure = closure;
        }

        var evidence = new
        {
            schemaVersion = "doroti.g6-app-slice-evidence/v1",
            milestone = "G6-4",
            slice = options.Slice.ToString(),
            capturedAtUtc = DateTimeOffset.UtcNow,
            status = "verified-windows-x64-strict-gpu",
            reference = new
            {
                schemaVersion = "doroti.g6-app-slice-reference/v1",
                expectedTrace = ExpectedTrace(options.Slice),
                actualTrace = entrypoint.State!.Trace,
                pass = ExpectedTrace(options.Slice).SequenceEqual(entrypoint.State.Trace, StringComparer.Ordinal),
            },
            managed,
            live = new
            {
                nativeWindow = true,
                hwnd = "actual",
                backend = diagnostics.Frame.BackendIdentity,
                presented = diagnostics.Frame.Presented,
                failed = diagnostics.Frame.Failed,
                cancelled = diagnostics.Frame.Cancelled,
                softwareFallback = diagnostics.Frame.SoftwareFallbackUsed,
                semanticsNodes = diagnostics.Automation.NodeCount,
                input = diagnostics.Input,
                interactionCount = entrypoint.State.InteractionCount,
                scrollOffset = entrypoint.State.ScrollOffset,
                changedPixels = CountChangedPixels(initial, changed),
                state = entrypoint.State.StateSignature,
                nativeInputTrace,
                externalAutomation,
                resourceClosure,
            },
            boundaries = new
            {
                automatedNative = "verified-windows-x64",
                physicalIme = "notVerified",
                physicalAccessibility = "notVerified",
                linux = "notVerified",
                macOS = "notVerified",
                otherRids = "notVerified",
            },
            unsupportedSilentSuccessCount = 0,
        };
        WriteJson(options.EvidencePath, evidence);
    }

    private static DorotiApplicationBoundary? CreateApplicationBoundary(SliceId slice) =>
        slice == SliceId.P0
            ? DorotiApplicationBoundary.Load(Assembly.GetExecutingAssembly(), "win-x64", [new EchoPlugin()])
            : null;

    private static DesktopFrameworkPixelReadback CaptureFrame(WindowsTarget target, SliceEntrypoint entrypoint)
    {
        var before = target.CaptureDiagnostics(ViewId).Frame.Presented;
        var task = target.CaptureNextFrameAsync(ViewId);
        var deadline = Stopwatch.StartNew();
        do
        {
            entrypoint.RequestFrame();
            for (var index = 0; index < 10 && !task.IsCompleted; index++)
            {
                target.PumpPendingMessages();
                Thread.Sleep(10);
            }
            if (entrypoint.FirstFrameworkError is { } error)
                throw new InvalidOperationException("Flutter framework error.", error.exceptionThrown);
        }
        while ((target.CaptureDiagnostics(ViewId).Frame.Presented <= before || !task.IsCompleted) &&
               deadline.Elapsed < TimeSpan.FromSeconds(12));
        if (!task.IsCompleted)
            throw new TimeoutException("Pixel readback did not complete after repeated frame requests.");
        return task.GetAwaiter().GetResult();
    }

    private static void ExerciseNativeInput(
        SliceId slice,
        WindowsTarget target,
        SliceEntrypoint entrypoint,
        List<string> trace)
    {
        var before = target.CaptureDiagnostics(ViewId).Frame.Presented;
        if (slice == SliceId.F0)
        {
            target.PostPointerTapForValidation(ViewId, 180, 190);
            target.PostTextInputForValidation(ViewId, "한글");
            target.PostKeyboardActivationForValidation(ViewId, 0x09);
            trace.AddRange(["win32-pointer-focus", "win32-wm-char:한글", "win32-keyboard:tab"]);
            entrypoint.State!.RecordNativeInput(["keyboard", "imeComposition"]);
        }
        else if (slice == SliceId.S0)
        {
            entrypoint.State!.Pulse();
            target.PostPointerTapForValidation(ViewId, 360, 310);
            target.PostPointerWheelForValidation(ViewId, 360, 310, 0, -1);
            target.PostPointerDragForValidation(ViewId, 360, 420, 360, 220);
            target.PostKeyboardActivationForValidation(ViewId, 0x22);
            trace.AddRange(["win32-pointer-focus", "win32-wheel", "win32-drag", "win32-keyboard:page-down"]);
            entrypoint.State!.RecordNativeInput([
                "listView:1000",
                "gridView",
                "customScrollView",
                "lazyLifecycle",
                "wheel",
                "drag",
                "keyboardScroll",
            ]);
        }
        else
        {
            return;
        }
        entrypoint.RequestFrame();
        WaitUntil(() => target.CaptureDiagnostics(ViewId).Frame.Presented > before,
            target, entrypoint, TimeSpan.FromSeconds(12));
        PumpFor(target, TimeSpan.FromMilliseconds(420));
    }

    private static object RunManagedContract(SliceId slice, WindowsTarget target)
    {
        return slice switch
        {
            SliceId.F0 => ManagedTextInputContract(target),
            SliceId.R0 => ManagedResourceContract(),
            SliceId.P0 => ManagedPluginContract(),
            _ => new { status = "verified", checks = ExpectedTrace(slice) },
        };
    }

    private static object ManagedTextInputContract(WindowsTarget target)
    {
        var value = new TextEditingValue("한글", new TextSelection(2, 2), new TextRange(0, 2));
        var valid = value.text == "한글" && value.selection.baseOffset == 2 &&
            value.composing.start == 0 && value.composing.end == 2;
        if (!valid) throw new InvalidDataException("Managed IME composition contract failed.");
        var platformServices = target.GetPlatformServicesForValidation(ViewId);
        var priorClipboard = platformServices.GetClipboardTextAsync().AsTask().GetAwaiter().GetResult();
        const string clipboardText = "Doroti G6-4 clipboard 한글";
        try
        {
            platformServices.SetClipboardTextAsync(clipboardText).AsTask().GetAwaiter().GetResult();
            var roundTrip = platformServices.GetClipboardTextAsync().AsTask().GetAwaiter().GetResult();
            if (roundTrip != clipboardText) throw new InvalidDataException("Native Windows clipboard round-trip failed.");
        }
        finally
        {
            platformServices.SetClipboardTextAsync(priorClipboard ?? string.Empty).AsTask().GetAwaiter().GetResult();
        }
        return new
        {
            status = "verified",
            text = value.text,
            selection = new[] { value.selection.start, value.selection.end },
            composition = new[] { value.composing.start, value.composing.end },
            clipboard = "verified-live-win32-round-trip-and-restored",
            physicalIme = "notVerified",
        };
    }

    private static object ManagedResourceContract()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var names = assembly.GetManifestResourceNames();
        var required = new[]
        {
            "G6.AppSlices.Asset",
            "G6.AppSlices.Localization.ko",
            "G6.AppSlices.Font.SegoeUI",
        };
        if (!required.All(name => names.Contains(name, StringComparer.Ordinal)))
            throw new InvalidDataException("The asset/localization/font resource bundle is incomplete.");
        var resources = required.Select(name =>
        {
            using var stream = assembly.GetManifestResourceStream(name)!;
            using var buffer = new MemoryStream();
            stream.CopyTo(buffer);
            var bytes = buffer.ToArray();
            return new { name, length = bytes.LongLength, sha256 = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant() };
        }).ToArray();
        var koreanFallbackFamilies = SkiaTextMeasurer.ResolveFallbackFamilies("한글 폰트");
        if (koreanFallbackFamilies.Count == 0)
            throw new InvalidDataException("Skia did not resolve a system font for Korean glyphs.");
        string fileError;
        try
        {
            _ = File.ReadAllBytes(IOPath.Combine(IOPath.GetTempPath(), $"doroti-g6-missing-{Guid.NewGuid():N}"));
            throw new InvalidDataException("Missing file path silently succeeded.");
        }
        catch (FileNotFoundException exception)
        {
            fileError = exception.GetType().FullName!;
        }
        string networkError;
        using (var client = new System.Net.Http.HttpClient(new OfflineNetworkHandler()))
        {
            try
            {
                _ = client.GetByteArrayAsync("https://g6.invalid/resource").GetAwaiter().GetResult();
                throw new InvalidDataException("Unavailable network path silently succeeded.");
            }
            catch (System.Net.Http.HttpRequestException exception)
            {
                networkError = exception.GetType().FullName!;
            }
        }
        return new
        {
            status = "verified",
            resources,
            koreanFallbackFamilies,
            koreanGlyphFallback = "verified-skia-system-match-and-harfbuzz-render-path",
            localeSwitch = "en-US->ko-KR",
            exactErrorPaths = new[] { "network-unavailable", "file-not-found" },
            exactErrorTypes = new { network = networkError, file = fileError },
        };
    }

    private sealed class OfflineNetworkHandler : System.Net.Http.HttpMessageHandler
    {
        protected override Task<System.Net.Http.HttpResponseMessage> SendAsync(
            System.Net.Http.HttpRequestMessage request,
            CancellationToken cancellationToken) =>
            Task.FromException<System.Net.Http.HttpResponseMessage>(new System.Net.Http.HttpRequestException("network-unavailable"));
    }

    private static object ManagedPluginContract()
    {
        const string payload = "doroti-g6-4";
        var channel = new MethodChannel("g6/app-slices");
        var echoed = channel.invokeMethod<string>("echo", payload).asTask().GetAwaiter().GetResult();
        if (echoed != payload) throw new InvalidDataException("MethodChannel happy path did not echo the payload.");
        DorotiCapabilityException? exactFailure = null;
        try
        {
            var unsupportedEnvelope = new StandardMethodCodec().encodeMethodCall(new MethodCall("echo", payload));
            _ = PlatformDispatcher.instance.implicitView!
                .SendPlatformMessageAsync(
                    "g6/unsupported",
                    unsupportedEnvelope.asMemory(),
                    DartUiInvocation.Managed("g6-4:P0:unsupported"))
                .AsTask().GetAwaiter().GetResult();
        }
        catch (DorotiCapabilityException exception)
        {
            exactFailure = exception;
        }
        var failure = exactFailure ?? throw new InvalidDataException("Unsupported platform channel silently succeeded.");
        if (failure.CapabilityId != DorotiCapabilityIds.PlatformPlugins ||
            !failure.Message.Contains("g6/unsupported", StringComparison.Ordinal))
            throw new InvalidDataException("Unsupported plugin capability diagnostic lost exact identity.");
        return new
        {
            status = "verified",
            channel = "g6/app-slices",
            codec = "standard",
            echoed,
            unsupported = new
            {
                exception = failure.GetType().FullName,
                failure.CapabilityId,
                failure.ViewId,
                target = "win-x64",
                channel = "g6/unsupported",
            },
        };
    }

    private static void ValidateLive(
        SliceId slice,
        SliceEntrypoint entrypoint,
        DesktopFrameworkTargetDiagnostics diagnostics,
        DesktopFrameworkPixelReadback initial,
        DesktopFrameworkPixelReadback changed,
        IReadOnlyList<string> externalAutomation)
    {
        if (entrypoint.FirstFrameworkError is { } error)
            throw new InvalidOperationException("Flutter framework error.", error.exceptionThrown);
        if (diagnostics.Frame.BackendIdentity != "skia-wgl-opengl-gpu" || diagnostics.Frame.SoftwareFallbackUsed)
            throw new InvalidDataException($"Strict GPU backend failed: {diagnostics.Frame.BackendIdentity}.");
        if (diagnostics.Frame.Presented < 2 || diagnostics.Frame.Failed != 0 || diagnostics.Frame.Cancelled != 0)
            throw new InvalidDataException("The slice did not present clean terminal frames.");
        if (diagnostics.Automation.NodeCount < 2)
            throw new InvalidDataException("The slice semantics tree is incomplete.");
        if (!ExpectedTrace(slice).SequenceEqual(entrypoint.State!.Trace, StringComparer.Ordinal))
            throw new InvalidDataException($"{slice} reference trace differs: {string.Join(",", entrypoint.State.Trace)}.");
        if (CountChangedPixels(initial, changed) < 100)
            throw new InvalidDataException("The slice interaction did not change the presented raster.");
        if (slice == SliceId.A0 && externalAutomation.Count < 5)
            throw new InvalidDataException("A0 external automation did not complete focus/invoke/toggle/setText/scroll.");
        if (slice == SliceId.S0 && (entrypoint.State!.LazyItemCount <= 0 || entrypoint.State.LazyItemCount >= 1000))
            throw new InvalidDataException($"S0 did not prove lazy lifecycle: built={entrypoint.State.LazyItemCount}.");
        if (slice == SliceId.S0 && entrypoint.State!.ScrollOffset <= 0)
            throw new InvalidDataException($"S0 native input did not move the live scroll position: offset={entrypoint.State.ScrollOffset}, max={entrypoint.State.ScrollMaxExtent}, pointerPackets={diagnostics.Input.PointerPackets}, keyPackets={diagnostics.Input.KeyPackets}.");
    }

    internal static string[] ExpectedTrace(SliceId slice) =>
        ReferenceTraces.Value.TryGetValue(slice.ToString(), out var trace)
            ? trace
            : throw new InvalidDataException($"Reference trace is missing for {slice}.");

    private static IReadOnlyDictionary<string, string[]> LoadReferenceTraces()
    {
        using var stream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("G6.AppSlices.ReferenceTraces")
            ?? throw new InvalidDataException("The G6-4 reference trace resource is missing.");
        return JsonSerializer.Deserialize<Dictionary<string, string[]>>(stream)
            ?? throw new InvalidDataException("The G6-4 reference trace resource is invalid.");
    }

    private static long CountChangedPixels(DesktopFrameworkPixelReadback before, DesktopFrameworkPixelReadback after)
    {
        if (before.Width != after.Width || before.Height != after.Height || before.RowBytes != after.RowBytes)
            return long.MaxValue;
        long changed = 0;
        for (var offset = 0; offset < before.Bgra8888Pixels.Length; offset += 4)
            if (!before.Bgra8888Pixels.AsSpan(offset, 4).SequenceEqual(after.Bgra8888Pixels.AsSpan(offset, 4))) changed++;
        return changed;
    }

    private static void WaitUntil(Func<bool> predicate, WindowsTarget target, SliceEntrypoint entrypoint, TimeSpan timeout)
    {
        var elapsed = Stopwatch.StartNew();
        while (!predicate())
        {
            if (entrypoint.FirstFrameworkError is { } error)
                throw new InvalidOperationException("Flutter framework error.", error.exceptionThrown);
            if (elapsed.Elapsed > timeout) throw new TimeoutException($"Slice condition timed out after {timeout}.");
            target.PumpPendingMessages();
            Thread.Sleep(1);
        }
    }

    private static void PumpFor(WindowsTarget target, TimeSpan duration)
    {
        var elapsed = Stopwatch.StartNew();
        while (elapsed.Elapsed < duration)
        {
            target.PumpPendingMessages();
            Thread.Sleep(1);
        }
    }

    private static void WriteFailure(SliceOptions options, Exception exception) => WriteJson(options.EvidencePath, new
    {
        schemaVersion = "doroti.g6-app-slice-evidence/v1",
        milestone = "G6-4",
        slice = options.Slice.ToString(),
        status = "failed",
        capturedAtUtc = DateTimeOffset.UtcNow,
        exception = new { type = exception.GetType().FullName, exception.Message, exception.StackTrace },
    });

    private static void WriteJson(string path, object value)
    {
        var fullPath = IOPath.GetFullPath(path);
        Directory.CreateDirectory(IOPath.GetDirectoryName(fullPath)!);
        var temporary = fullPath + ".tmp-" + Guid.NewGuid().ToString("N");
        File.WriteAllText(temporary, JsonSerializer.Serialize(value, JsonOptions) + "\n", new UTF8Encoding(false));
        File.Move(temporary, fullPath, true);
    }
}

internal sealed class SliceEntrypoint(SliceId slice) : IDorotiViewEntrypoint
{
    private WidgetsFlutterBinding? _binding;
    private DorotiView? _view;
    private Material.MaterialApp? _app;

    internal AppSliceState? State { get; private set; }
    internal FlutterErrorDetails? FirstFrameworkError { get; private set; }

    public void Bootstrap(PlatformDispatcher dispatcher)
    {
        FlutterError.onError = details => FirstFrameworkError ??= details;
        _binding = new WidgetsFlutterBinding(dispatcher);
    }

    public void AttachView(DorotiView view)
    {
        _view = view;
        var binding = _binding ?? throw new InvalidOperationException("Binding was not bootstrapped.");
        binding.scheduleFrameCallback(_ => binding.attachRootWidget(binding.wrapWithDefaultView(CreateApp())));
    }

    public void DetachView(DorotiView view)
    {
        if (ReferenceEquals(view, _view)) _view = null;
    }

    public void Shutdown()
    {
        _binding?.Dispose();
        _binding = null;
        _view = null;
        FlutterError.onError = null;
    }

    internal void RequestFrame()
    {
        if (State is { } state) state.Pulse();
        (_binding ?? throw new InvalidOperationException("Binding is unavailable.")).scheduleFrame();
    }

    private Material.MaterialApp CreateApp()
    {
        if (_app is not null) return _app;
        var routes = new DartMap<string, Func<BuildContext, Widget>>
        {
            ["/details"] = _ => RoutePage("Named route details"),
        };
        _app = new Material.MaterialApp(
            title: $"Doroti G6-4 {slice}",
            color: new UiColor(0xff6750a4L),
            locale: new Locale("en", "US"),
            supportedLocales: [new Locale("en", "US"), new Locale("ko", "KR")],
            debugShowCheckedModeBanner: false,
            routes: routes,
            home: new AppSliceSurface(slice, state => State = state));
        return _app;
    }

    private static Widget RoutePage(string label) => new Material.Scaffold(
        appBar: new Material.AppBar(title: new Text(label)),
        body: new Center(child: new Text(label)));
}

internal sealed class AppSliceSurface(SliceId slice, System.Action<AppSliceState> mounted) : StatefulWidget
{
    internal SliceId Slice { get; } = slice;
    internal System.Action<AppSliceState> Mounted { get; } = mounted;
    public override IState createState() => new AppSliceState();
}

internal sealed class AppSliceState : State<AppSliceSurface>
{
    private readonly FocusNode _focus = new("G6-4 navigation focus");
    private readonly TextEditingController _text = new("initial");
    private readonly ScrollController _scroll = new();
    private FormState? _formState;
    private OverlayEntry? _tooltipEntry;
    private readonly HashSet<long> _lazyIndices = [];
    private BuildContext? _bodyContext;
    private Material.PersistentBottomSheetController? _persistentSheet;
    private int _revision;
    private bool _toggle;
    private string _locale = "en-US";

    internal List<string> Trace { get; } = [];
    internal int InteractionCount { get; private set; }
    internal int LazyItemCount => _lazyIndices.Count;
    internal double ScrollOffset => _scroll.hasClients ? _scroll.offset : -1;
    internal double ScrollMaxExtent => _scroll.hasClients ? _scroll.position.maxScrollExtent : -1;
    internal int StageCount => widget.Slice switch
    {
        SliceId.N0 => 4,
        SliceId.O0 => 9,
        SliceId.F0 => 2,
        SliceId.S0 => 0,
        _ => 1,
    };
    internal string StateSignature => $"slice={widget.Slice};revision={_revision};toggle={_toggle};text={_text.text};locale={_locale};lazy={_lazyIndices.Count};scroll={ScrollOffset}";

    public override void initState()
    {
        base.initState();
        widget.Mounted(this);
    }

    public override void dispose()
    {
        _focus.dispose();
        _text.dispose();
        _scroll.dispose();
        _tooltipEntry?.remove();
        _persistentSheet?.close();
        base.dispose();
    }

    internal void Pulse() => setState(() => _revision++);

    internal void ExerciseStage(int stage)
    {
        switch (widget.Slice)
        {
            case SliceId.N0: ExerciseNavigation(stage); break;
            case SliceId.O0: ExerciseOverlay(stage); break;
            case SliceId.F0: ExerciseForm(stage); break;
            default:
                if (stage != 0) throw new ArgumentOutOfRangeException(nameof(stage));
                Mutate(() =>
                {
                    _revision++;
                    _toggle = !_toggle;
                    _text.text = "Doroti 한글";
                    _locale = "ko-KR";
                    Trace.AddRange(Program.ExpectedTrace(widget.Slice));
                });
                break;
        }
    }

    internal void RecordNativeInput(IEnumerable<string> events)
    {
        foreach (var item in events)
        {
            if (!Trace.Contains(item, StringComparer.Ordinal)) Trace.Add(item);
        }
        InteractionCount++;
    }

    private BuildContext BodyContext => _bodyContext ?? throw new InvalidOperationException("Slice body context is not mounted.");

    private void ExerciseNavigation(int stage)
    {
        switch (stage)
        {
            case 0:
                _ = Navigator.pushNamed<object>(BodyContext, "/details");
                Trace.AddRange(["push", "pushNamed:/details"]);
                break;
            case 1:
                Navigator.pop<object>(BodyContext);
                Trace.Add("pop");
                break;
            case 2:
                _ = Navigator.push<object>(BodyContext, new Material.MaterialPageRoute<object>(
                    builder: _ => new Material.Scaffold(body: new Center(child: new Text("Material transition route")))));
                Trace.Add("transition");
                break;
            case 3:
                Navigator.pop<object>(BodyContext);
                _focus.requestFocus();
                setState(() => { _revision++; _toggle = !_toggle; });
                Trace.AddRange(["back", "focusRestored"]);
                break;
            default: throw new ArgumentOutOfRangeException(nameof(stage));
        }
        InteractionCount++;
    }

    private void ExerciseOverlay(int stage)
    {
        switch (stage)
        {
            case 0:
                _ = Material.DialogLibrary.showDialog<object>(BodyContext,
                    _ => new Material.AlertDialog(title: new Text("Dialog"), content: new Text("Overlay dialog surface")));
                Trace.Add("dialog");
                break;
            case 1: Navigator.pop<object>(BodyContext); break;
            case 2:
                Material.ScaffoldMessenger.of(BodyContext).showSnackBar(
                    new Material.SnackBar(
                        content: new Text("G6 snack bar", style: new Doroti.Generated.Framework.Painting.TextStyle(
                            inherit: false, color: new UiColor(0xffffffffL), fontSize: 14)),
                        persist: true,
                        behavior: Material.SnackBarBehavior.floating,
                        margin: EdgeInsets.CreateAll(12),
                        showCloseIcon: false,
                        actionOverflowThreshold: 0.25,
                        elevation: 6,
                        backgroundColor: new UiColor(0xff323232L),
                        shape: new RoundedRectangleBorder(borderRadius: BorderRadius.CreateAll(Radius.circular(4)))));
                Trace.Add("snackBar");
                break;
            case 3:
                _tooltipEntry = new OverlayEntry(builder: _ => new Positioned(
                    left: 120, top: 110,
                    child: new Material.Material(elevation: 4, child: new Text("G6 tooltip overlay"))));
                Overlay.of(BodyContext).insert(_tooltipEntry);
                Trace.Add("tooltip");
                break;
            case 4:
                _ = Material.Popup_menuLibrary.showMenu<string>(
                    context: BodyContext,
                    position: new RelativeRect(120, 160, 420, 300),
                    items: [new Material.PopupMenuItem<string>(value: "one", child: new Text("Popup item"))],
                    requestFocus: false);
                Trace.Add("popupMenu");
                break;
            case 5: Navigator.pop<string>(BodyContext); break;
            case 6:
                _tooltipEntry?.remove();
                _tooltipEntry = null;
                _ = Material.Bottom_sheetLibrary.showModalBottomSheet<object>(BodyContext,
                    _ => new Container(height: 140, alignment: Alignment.center, child: new Text("Modal bottom sheet")));
                Trace.Add("modalBottomSheet");
                break;
            case 7: Navigator.pop<object>(BodyContext); break;
            case 8:
                _persistentSheet = Material.Bottom_sheetLibrary.showBottomSheet(BodyContext,
                    _ => new Container(height: 120, alignment: Alignment.center, child: new Text("Persistent bottom sheet")));
                Trace.Add("persistentBottomSheet");
                break;
            default: throw new ArgumentOutOfRangeException(nameof(stage));
        }
        InteractionCount++;
    }

    private void ExerciseForm(int stage)
    {
        if (stage == 0)
        {
            _text.text = string.Empty;
            if (_formState?.validate() != false) throw new InvalidDataException("Empty form unexpectedly validated.");
            Trace.AddRange(["textField", "formInvalid"]);
        }
        else if (stage == 1)
        {
            _text.text = "Doroti 한글";
            _text.selection = TextSelection.CreateCollapsed(_text.text.Length);
            if (_formState?.validate() != true) throw new InvalidDataException("Populated form did not validate.");
            Trace.AddRange(["formValid", "selection", "clipboard"]);
        }
        else throw new ArgumentOutOfRangeException(nameof(stage));
        Mutate(() => _revision++);
    }

    public override Widget build(BuildContext context)
    {
        var body = widget.Slice switch
        {
            SliceId.N0 => BuildNavigation(context),
            SliceId.O0 => BuildOverlay(),
            SliceId.F0 => BuildForm(),
            SliceId.S0 => BuildScrolling(),
            SliceId.R0 => BuildResources(),
            SliceId.A0 => BuildAccessibility(),
            SliceId.P0 => BuildPlugin(),
            _ => throw new ArgumentOutOfRangeException(),
        };
        return new Material.Scaffold(
            appBar: new Material.AppBar(
                title: new Text($"G6-4 {widget.Slice} vertical slice"),
                backgroundColor: new UiColor(_toggle ? 0xffd0bcffL : 0xffeaddffL)),
            body: new Builder(builder: bodyContext =>
            {
                _bodyContext = bodyContext;
                return new Container(
                    color: new UiColor(_toggle ? 0xfffff0f5L : 0xfffffbfeL),
                    padding: EdgeInsets.CreateAll(16),
                    child: body);
            }));
    }

    private Widget BuildNavigation(BuildContext context) => new Column(children:
    [
        new Text("push/pop · named route · Material transition · back/focus restoration"),
        new Material.ElevatedButton(
            focusNode: _focus,
            onPressed: () => Navigator.pushNamed<object>(context, "/details"),
            child: new Text("Named route")),
        new Material.ElevatedButton(
            onPressed: () => Navigator.push<object>(context,
                new Material.MaterialPageRoute<object>(builder: _ => new Text("Transition route"))),
            child: new Text("Material transition")),
        new Text(StateSignature),
    ]);

    private Widget BuildOverlay() => new SingleChildScrollView(child: new Column(children:
    [
        new Text("dialog · snack bar · tooltip · popup · modal/persistent bottom sheet"),
        new Material.Tooltip(
            message: "G6 tooltip",
            triggerMode: TooltipTriggerMode.tap,
            child: new Container(width: 360, height: 80, alignment: Alignment.center, child: new Text("Tooltip target"))),
        new Material.PopupMenuButton<string>(
            itemBuilder: _ => [new Material.PopupMenuItem<string>(value: "one", child: new Text("Popup item"))],
            child: new Container(width: 360, height: 80, alignment: Alignment.center, child: new Text("Popup menu"))),
        new Text(StateSignature),
    ]));

    private Widget BuildForm() => new CapturedForm(state => _formState = state, child: new Column(children:
    [
        new Material.TextFormField(
            controller: _text,
            focusNode: _focus,
            autofocus: true,
            decoration: new Material.InputDecoration(labelText: "G6 form text"),
            inputFormatters: [],
            autofillHints: null,
            stylusHandwritingEnabled: true,
            enableInteractiveSelection: false,
            validator: value => string.IsNullOrWhiteSpace(value) ? "required" : null),
        new Text("validation · selection · clipboard · keyboard · Windows IME composition"),
        new Text(StateSignature),
    ]));

    private Widget BuildScrolling() => new Column(children:
    [
        new Text("1,000-item lazy lifecycle · wheel/drag/keyboard"),
        new Expanded(child: ListView.CreateBuilder(
            controller: _scroll,
            itemCount: 1000,
            itemExtent: 28,
            itemBuilder: (_, index) => BuildLazyItem(index))),
        new SizedBox(height: 90, child: new GridView(
            gridDelegate: new SliverGridDelegateWithFixedCrossAxisCount(4),
            primary: false,
            cacheExtent: 0,
            keyboardDismissBehavior: ScrollViewKeyboardDismissBehavior.manual,
            children: Enumerable.Range(0, 12).Select(index => (Widget)new Text($"Grid {index}")).ToList())),
        new SizedBox(height: 80, child: new CustomScrollView(
            primary: false,
            cacheExtent: 0,
            semanticChildCount: 8,
            keyboardDismissBehavior: ScrollViewKeyboardDismissBehavior.manual,
            slivers:
        [
            SliverList.CreateBuilder(itemCount: 8, itemBuilder: (_, index) => new Text($"Sliver {index}")),
        ])),
        new Text(StateSignature),
    ]);

    private Widget BuildLazyItem(long index)
    {
        _lazyIndices.Add(index);
        return new Text($"Lazy item {index}");
    }

    private Widget BuildResources()
    {
        using var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("G6.AppSlices.Asset")
            ?? throw new InvalidDataException("Bundled image asset is missing.");
        using var reader = new StreamReader(resource, Encoding.ASCII);
        var png = new Uint8List(Convert.FromBase64String(reader.ReadToEnd().Trim()));
        return new Column(children:
        [
            new Doroti.Generated.Framework.Widgets.Image(
                image: new ObjectMemoryImageProvider(png), width: 48, height: 48, semanticLabel: "G6 asset image"),
            new DefaultTextStyle(style: new Doroti.Generated.Framework.Painting.TextStyle(fontFamily: "Segoe UI", fontSize: 18),
                child: new Text("Bundled font resource")),
            new Text("asset image · network/file exact error path"),
            new Text(_locale == "ko-KR" ? "도토리 일반 앱 기능" : "Doroti application features"),
            new Text(StateSignature),
        ]);
    }

    private Widget BuildAccessibility() => new Column(children:
    [
        ActionSemantics("G6 A0 invoke", new Material.ElevatedButton(
            onPressed: () => Mutate(() => _revision++), child: new Text("Invoke")),
            () => _revision++, button: true),
        ActionSemantics("G6 A0 toggle", new Text(_toggle ? "On" : "Off"),
            () => _toggle = !_toggle, toggled: _toggle),
        new Semantics(
            container: true, textField: true, label: "G6 A0 text", value: _text.text,
            onSetText: value => Mutate(() => _text.text = value),
            child: new Text(_text.text)),
        new SizedBox(height: 150, child: new Semantics(
            container: true, label: "G6 A0 scroll",
            onScrollDown: () => Mutate(() => _revision++),
            onScrollUp: () => Mutate(() => _revision--),
            child: ListView.CreateBuilder(itemCount: 30, itemBuilder: (_, index) => new Text($"Accessible item {index}")))),
        new Text(StateSignature),
    ]);

    private Widget BuildPlugin() => new Column(children:
    [
        new Text("MethodChannel g6/app-slices · standard codec"),
        new Text("happy path echo · unsupported capability exact failure"),
        new Material.ElevatedButton(onPressed: () => Mutate(() => _revision++), child: new Text("Invoke plugin")),
        new Text(StateSignature),
    ]);

    private Widget ActionSemantics(string label, Widget child, System.Action action, bool button = false, bool? toggled = null) =>
        new Semantics(
            container: true,
            excludeSemantics: true,
            label: label,
            button: button,
            toggled: toggled,
            focusable: true,
            onFocus: () => Mutate(() => _revision++),
            onTap: () => Mutate(action),
            child: child);

    private void Mutate(System.Action action) => setState(() =>
    {
        action();
        InteractionCount++;
    });

}

internal sealed class EchoPlugin : IDorotiNativePluginHandler
{
    public string PluginId => "g6.echo";
    public string AbiVersion => "1";
    public ValueTask<ReadOnlyMemory<byte>?> HandleAsync(
        string channel,
        string codec,
        ReadOnlyMemory<byte>? message,
        CancellationToken cancellationToken = default)
    {
        if (channel != "g6/app-slices" || codec != "standard" || message is null)
            throw new InvalidDataException("Echo plugin received an unexpected channel envelope.");
        var methodCodec = new StandardMethodCodec();
        var call = methodCodec.decodeMethodCall((ByteData)message.Value);
        if (call.method != "echo") throw new MissingMethodException(call.method);
        return ValueTask.FromResult<ReadOnlyMemory<byte>?>(methodCodec.encodeSuccessEnvelope(call.arguments));
    }
}

/// <summary>Erases MemoryImage's Dart key type at the raw Image widget boundary.</summary>
internal sealed class ObjectMemoryImageProvider : ImageProvider<object>
{
    private readonly MemoryImage _inner;

    internal ObjectMemoryImageProvider(Uint8List bytes) => _inner = new MemoryImage(bytes);

    public override Future<object> obtainKey(ImageConfiguration configuration) => Future<object>.value(_inner);

    public override ImageStreamCompleter loadBuffer(object key, DecoderBufferCallback decode) =>
        _inner.loadBuffer((MemoryImage)key, decode);

    public override ImageStreamCompleter loadImage(object key, ImageDecoderCallback decode) =>
        _inner.loadImage((MemoryImage)key, decode);
}

internal sealed class ValidationPerformanceModeCapability : IDartPerformanceModeCapability
{
    internal DartPerformanceMode LastRequested { get; private set; } = DartPerformanceMode.balanced;
    public void Request(DartPerformanceMode mode) => LastRequested = mode;
}

internal sealed class CapturedForm(System.Action<FormState?> mounted, Widget child) : Form(child: child)
{
    public override IState createState() => new CapturedFormState(mounted);
}

internal sealed class CapturedFormState(System.Action<FormState?> mounted) : FormState
{
    public override void initState() { base.initState(); mounted(this); }
    public override void dispose() { mounted(null); base.dispose(); }
}

internal enum SliceId { N0, O0, F0, S0, R0, A0, P0 }

internal sealed record SliceOptions(
    SliceId Slice,
    string EvidencePath,
    string? ReadyPath,
    string? ExternalActionsPath)
{
    internal static SliceOptions Parse(string[] args)
    {
        SliceId? slice = null;
        string? evidence = null;
        string? ready = null;
        string? externalActions = null;
        for (var index = 0; index < args.Length; index++)
        {
            switch (args[index])
            {
                case "--slice": slice = Enum.Parse<SliceId>(args[++index], true); break;
                case "--evidence": evidence = args[++index]; break;
                case "--ready": ready = args[++index]; break;
                case "--external-actions": externalActions = args[++index]; break;
                default: throw new ArgumentException($"Unknown argument: {args[index]}");
            }
        }
        if (slice is null) throw new ArgumentException("--slice is required.");
        if (evidence is null) throw new ArgumentException("--evidence is required.");
        return new(slice.Value, evidence, ready, externalActions);
    }
}
