using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Doroti.Hosting;
using Doroti.Ui;
using Doroti.Generated.Application.G6Demo.Framework;
using Doroti.Generated.Framework.Foundation;
using Doroti.Generated.Framework.Gestures;
using Doroti.Generated.Framework.Widgets;
using Doroti.Host.Desktop.Framework;
using Doroti.Platform;
using Doroti.Plugin.G6GeneratedDemoEcho.WinX64;
using Doroti.Target.Windows;
using PlatformSemanticsRole = Doroti.Platform.SemanticsRole;

internal static class Program
{
    private const ulong ViewId = 670;

    [STAThread]
    public static int Main(string[] args)
    {
        var evidencePath = ReadOption(args, "--evidence") ?? throw new ArgumentException("--evidence is required.");
        var frameCount = int.TryParse(ReadOption(args, "--frames"), out var parsed) ? parsed : 60;
        var total = Stopwatch.StartNew();
        var process = Process.GetCurrentProcess();
        var initialWorkingSet = process.WorkingSet64;
        var initialHandles = process.HandleCount;
        var peakWorkingSet = initialWorkingSet;
        var entrypoint = new GeneratedDemoEntrypoint();
        DesktopFrameworkTargetDiagnostics? diagnostics = null;
        DesktopFrameworkPixelReadback? initial = null;
        DesktopFrameworkPixelReadback? changed = null;
        NativeResourceSnapshot? resourceClosure = null;
        Exception? failure = null;
        var firstFrameMs = 0d;
        var interactionMs = 0d;
        var sustainedMs = 0d;
        long sustainedPresented = 0;
        long changedPixels = 0;
        IReadOnlyList<string> hitTargets = [];
        IReadOnlyList<object> semanticNodes = [];
        ApplicationBoundaryEvidence? applicationBoundary = null;

        try
        {
            applicationBoundary = ValidateApplicationBoundary();
            using var target = new WindowsTarget();
            using var session = new DorotiHostSession(entrypoint);
            using var scope = session.dispatcher.EnterScope();
            session.Start(deferFrameworkBootstrap: true);
            var view = target.CreateView(session, ViewId,
                new DorotiViewConfiguration("Doroti Generated Dart Demo", new Size(720, 640)));
            view.Show();
            session.dispatcher.setSemanticsTreeEnabled(true);

            WaitUntil(() =>
            {
                diagnostics = target.CaptureDiagnostics(ViewId);
                return diagnostics.Frame.Presented >= 1 && diagnostics.Automation.NodeCount > 0;
            }, target, entrypoint, TimeSpan.FromSeconds(15));
            firstFrameMs = total.Elapsed.TotalMilliseconds;

            initial = CaptureFrame(target, entrypoint);
            var semantics = target.GetSemanticsSnapshotForValidation(ViewId)
                ?? throw new InvalidDataException("Generated DemoApp emitted no semantics tree.");
            var flattened = Flatten(semantics.Root).ToArray();
            semanticNodes = flattened.Select(node => (object)new
            {
                node.Id,
                role = node.Role.ToString(),
                node.Label,
                node.Value,
                state = node.State.ToString(),
                actions = node.Actions.ToString(),
                bounds = new { node.Bounds.Left, node.Bounds.Top, node.Bounds.Width, node.Bounds.Height },
            }).ToArray();
            var button = flattened.FirstOrDefault(node =>
                node.Role == PlatformSemanticsRole.Button &&
                string.Equals(node.Label, "G6 generated button", StringComparison.Ordinal))
                ?? throw new InvalidDataException("Generated DemoApp button semantics node is missing.");
            // The current platform semantics snapshot preserves local node bounds;
            // native pointer validation therefore uses the same stable DemoApp
            // viewport coordinate as the reviewed handwritten fixture.
            var x = 80d;
            var y = 200d;
            hitTargets = entrypoint.HitTestTargetsAt(x, y);

            var interaction = Stopwatch.StartNew();
            target.PostPointerTapForValidation(ViewId, x, y);
            var presentedBeforeInteraction = target.CaptureDiagnostics(ViewId).Frame.Presented;
            WaitUntil(() => target.CaptureDiagnostics(ViewId).Frame.Presented > presentedBeforeInteraction,
                target, entrypoint, TimeSpan.FromSeconds(10));
            changed = CaptureFrame(target, entrypoint);
            interactionMs = interaction.Elapsed.TotalMilliseconds;
            changedPixels = CountChangedPixels(initial, changed);
            if (changedPixels < 100 || !hitTargets.Any(name => name.EndsWith("RenderPointerListener", StringComparison.Ordinal)))
                throw new InvalidDataException("Generated DemoApp native pointer interaction did not change its raster state.");

            var sustained = Stopwatch.StartNew();
            var sustainedBaseline = target.CaptureDiagnostics(ViewId).Frame.Presented;
            for (var index = 0; index < frameCount; index++)
            {
                var before = target.CaptureDiagnostics(ViewId).Frame.Presented;
                entrypoint.RequestFrame();
                WaitUntil(() => target.CaptureDiagnostics(ViewId).Frame.Presented > before,
                    target, entrypoint, TimeSpan.FromSeconds(5));
                process.Refresh();
                peakWorkingSet = Math.Max(peakWorkingSet, process.WorkingSet64);
            }
            sustainedMs = sustained.Elapsed.TotalMilliseconds;
            diagnostics = target.CaptureDiagnostics(ViewId);
            sustainedPresented = diagnostics.Frame.Presented - sustainedBaseline;
            if (diagnostics.Frame.BackendIdentity != "skia-wgl-opengl-gpu" || diagnostics.Frame.SoftwareFallbackUsed ||
                diagnostics.Frame.Failed != 0 || diagnostics.Frame.Cancelled != 0 || sustainedPresented < frameCount)
                throw new InvalidDataException("Generated DemoApp strict-GPU sustained frame gate failed.");

            session.DetachView(view);
            session.Shutdown();
            view.Dispose();
            for (var pump = 0; pump < 20; pump++) target.PumpPendingMessages();
            resourceClosure = target.CaptureResourceSnapshot();
            if (!resourceClosure.Value.IsBalanced)
                throw new InvalidDataException("Generated DemoApp native resources are not balanced after shutdown.");
        }
        catch (Exception exception)
        {
            failure = entrypoint.FirstFrameworkError?.exceptionThrown ?? exception;
        }

        process.Refresh();
        var evidence = new
        {
            schemaVersion = "doroti.g6-generated-demo-run/v1",
            milestone = "G6-7",
            outcome = failure is null ? "presented" : "failed",
            source = new
            {
                kind = "generated-dart-application",
                dartEntrypoint = "package:doroti_demo_app/main.dart",
                generatedAssembly = typeof(DorotiGeneratedDemoApp).Assembly.GetName().Name,
                frameworkAssembly = typeof(WidgetsFlutterBinding).Assembly.GetName().Name,
                consumer = "isolated-package-only",
            },
            target = new
            {
                rid = "win-x64",
                backend = diagnostics?.Frame.BackendIdentity,
                softwareFallbackUsed = diagnostics?.Frame.SoftwareFallbackUsed,
            },
            firstFlutterError = entrypoint.FirstFrameworkError is null ? null : new
            {
                type = entrypoint.FirstFrameworkError.exceptionThrown.GetType().FullName,
                entrypoint.FirstFrameworkError.exceptionThrown.Message,
            },
            failure = failure is null ? null : new { type = failure.GetType().FullName, failure.Message, stack = failure.StackTrace },
            interaction = new { changedPixels, hitTargets },
            semantics = new { nodeCount = diagnostics?.Automation.NodeCount ?? 0, nodes = semanticNodes },
            applicationBoundary,
            performance = new
            {
                firstFrameMs,
                interactionMs,
                sustained = new { requestedFrames = frameCount, presentedFrames = sustainedPresented, elapsedMs = sustainedMs },
                memory = new { initialWorkingSetBytes = initialWorkingSet, peakWorkingSetBytes = peakWorkingSet, finalWorkingSetBytes = process.WorkingSet64 },
                handles = new { initial = initialHandles, final = process.HandleCount },
            },
            frame = diagnostics?.Frame,
            resources = diagnostics?.Resources,
            resourceClosure,
        };
        var fullEvidencePath = System.IO.Path.GetFullPath(evidencePath);
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fullEvidencePath)!);
        File.WriteAllText(fullEvidencePath, JsonSerializer.Serialize(evidence, new JsonSerializerOptions { WriteIndented = true }) + "\n");
        if (failure is not null)
        {
            Console.Error.WriteLine(failure);
            return 1;
        }
        Console.WriteLine("G6-GENERATED-DART-DEMO-PASS");
        return 0;
    }

    private static DesktopFrameworkPixelReadback CaptureFrame(WindowsTarget target, GeneratedDemoEntrypoint entrypoint)
    {
        var capture = target.CaptureNextFrameAsync(ViewId);
        entrypoint.RequestFrame();
        WaitUntil(() => capture.IsCompleted, target, entrypoint, TimeSpan.FromSeconds(10));
        return capture.WaitAsync(TimeSpan.FromSeconds(10)).GetAwaiter().GetResult();
    }

    private static ApplicationBoundaryEvidence ValidateApplicationBoundary()
    {
        using var boundary = DorotiApplicationBoundary.Load(
            typeof(DorotiGeneratedDemoApp).Assembly, "win-x64", [new EchoPluginHandler()]);
        var capabilities = new DorotiViewCapabilities("win-x64/g6-7-generated-demo");
        boundary.Configure(capabilities);
        var resources = capabilities.Require<IApplicationResourceHostCapability>(
            ViewId, DorotiCapabilityIds.ApplicationResources, DartUiInvocation.Managed("g6-7-generated-demo"));
        var brand = resources.LoadAsync("assets/brand.txt").AsTask().GetAwaiter().GetResult();
        if (!Encoding.UTF8.GetString(brand.Span).Contains("Doroti generated Dart DemoApp", StringComparison.Ordinal))
            throw new InvalidDataException("Generated DemoApp asset payload drifted.");
        if (resources.ResolveFont("Doroti Sans").Key != "fonts/DorotiSans")
            throw new InvalidDataException("Generated DemoApp font registration drifted.");
        var localization = resources.ResolveLocalization("en");
        var localized = resources.LoadAsync(localization.Key).AsTask().GetAwaiter().GetResult();
        if (!Encoding.UTF8.GetString(localized.Span).Contains("Doroti Generated Demo", StringComparison.Ordinal))
            throw new InvalidDataException("Generated DemoApp localization payload drifted.");

        var missingResourceRejected = false;
        try
        {
            _ = resources.LoadAsync("assets/missing.txt").AsTask().GetAwaiter().GetResult();
        }
        catch (DorotiCapabilityException exception) when (exception.CapabilityId == DorotiCapabilityIds.ApplicationResources)
        {
            missingResourceRejected = true;
        }
        var messaging = capabilities.Require<IPlatformMessageHostCapability>(
            ViewId, DorotiCapabilityIds.PlatformMessaging, DartUiInvocation.Managed("g6-7-generated-demo"));
        var echo = messaging.SendAsync("g6/generated-demo/echo", Encoding.UTF8.GetBytes("doroti"))
            .AsTask().GetAwaiter().GetResult();
        var echoPassed = echo is not null && Encoding.UTF8.GetString(echo.Value.Span) == "win-x64:doroti";
        if (!echoPassed) throw new InvalidDataException("Generated DemoApp RID plugin response drifted.");
        var missingPluginRejected = false;
        try
        {
            _ = messaging.SendAsync("g6/missing", null).AsTask().GetAwaiter().GetResult();
        }
        catch (DorotiCapabilityException exception) when (exception.CapabilityId == DorotiCapabilityIds.PlatformPlugins)
        {
            missingPluginRejected = true;
        }
        if (!missingResourceRejected || !missingPluginRejected)
            throw new InvalidDataException("Generated DemoApp unsupported capability silently succeeded.");
        return new(boundary.Manifest.Resources.Length, boundary.Manifest.Plugins.Length,
            echoPassed, missingResourceRejected, missingPluginRejected);
    }

    private static void WaitUntil(Func<bool> predicate, WindowsTarget target, GeneratedDemoEntrypoint entrypoint, TimeSpan timeout)
    {
        var elapsed = Stopwatch.StartNew();
        while (!predicate())
        {
            if (entrypoint.FirstFrameworkError is { } error) throw new InvalidOperationException("Generated framework error.", error.exceptionThrown);
            if (elapsed.Elapsed > timeout) throw new TimeoutException($"Generated DemoApp condition timed out after {timeout}.");
            target.PumpPendingMessages();
            Thread.Sleep(1);
        }
    }

    private static IEnumerable<SemanticsNodeSnapshot> Flatten(SemanticsNodeSnapshot root)
    {
        yield return root;
        foreach (var child in root.Children)
        foreach (var node in Flatten(child))
            yield return node;
    }

    private static long CountChangedPixels(DesktopFrameworkPixelReadback before, DesktopFrameworkPixelReadback after)
    {
        if (before.Width != after.Width || before.Height != after.Height) return long.MaxValue;
        long changed = 0;
        for (var y = 0; y < before.Height; y++)
        for (var x = 0; x < before.Width; x++)
        {
            var a = y * before.RowBytes + x * 4;
            var b = y * after.RowBytes + x * 4;
            if (!before.Bgra8888Pixels.AsSpan(a, 4).SequenceEqual(after.Bgra8888Pixels.AsSpan(b, 4))) changed++;
        }
        return changed;
    }

    private static string? ReadOption(string[] args, string name)
    {
        var index = Array.IndexOf(args, name);
        return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
    }
}

internal sealed record ApplicationBoundaryEvidence(
    int ResourceCount,
    int PluginCount,
    bool EchoPluginPassed,
    bool MissingResourceRejected,
    bool MissingPluginRejected);

internal sealed class GeneratedDemoEntrypoint : IDorotiViewEntrypoint
{
    private WidgetsFlutterBinding? _binding;
    private DorotiView? _view;
    internal FlutterErrorDetails? FirstFrameworkError { get; private set; }

    public void Bootstrap(PlatformDispatcher dispatcher)
    {
        FlutterError.onError = details => FirstFrameworkError ??= details;
        _binding = new WidgetsFlutterBinding(dispatcher);
    }

    public void AttachView(DorotiView view)
    {
        if (_binding is null) throw new InvalidOperationException("Generated framework binding was not bootstrapped.");
        _view = view;
        _binding.scheduleFrameCallback(_ =>
            _binding.attachRootWidget(_binding.wrapWithDefaultView(new DorotiGeneratedDemoApp())));
    }

    public void DetachView(DorotiView view)
    {
        if (ReferenceEquals(_view, view)) _view = null;
    }

    internal void RequestFrame() => (_binding ?? throw new InvalidOperationException("Generated binding is unavailable.")).scheduleFrame();

    internal IReadOnlyList<string> HitTestTargetsAt(double x, double y)
    {
        var result = new HitTestResult();
        (_binding ?? throw new InvalidOperationException("Generated binding is unavailable.")).hitTestInView(
            result, new Offset(x, y), checked((long)(_view ?? throw new InvalidOperationException("Generated view is unavailable.")).viewId));
        return result.path.Select(entry => entry.target.GetType().FullName ?? entry.target.GetType().Name).ToArray();
    }

    public void Shutdown()
    {
        _binding?.Dispose();
        _binding = null;
        _view = null;
        FlutterError.onError = null;
    }
}
