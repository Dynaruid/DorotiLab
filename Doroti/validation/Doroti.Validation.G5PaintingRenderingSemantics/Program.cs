using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Text.Json;
using Doroti.Ui;
using Doroti.Host.Desktop;
using Doroti.Host.Desktop.Framework;
using Doroti.Platform;
using SkiaSharp;
using Path = System.IO.Path;
using UiColor = Doroti.Ui.Color;
using UiImage = Doroti.Ui.Image;
using UiPath = Doroti.Ui.Path;
using UiSemanticsAction = Doroti.Ui.SemanticsAction;
using UiSemanticsRole = Doroti.Ui.SemanticsRole;

if (!OperatingSystem.IsWindows())
{
    Console.Error.WriteLine("G5-2 native validation requires Windows.");
    return 2;
}

if (args.FirstOrDefault() == "--automation-target")
{
    return RunAutomationTarget(args.Skip(1).ToArray());
}

var outputPath = args.Length > 0
    ? Path.GetFullPath(args[0])
    : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "artifacts", "g5-2", "graphics.json"));
var failures = new List<string>();
var root = FindDorotiRoot(Environment.CurrentDirectory);
var api = ValidatePromotionApi(root, failures);
var graphics = ValidateNativeGraphics(failures);
var evidence = new
{
    schemaVersion = "doroti.g5-2-graphics-semantics-validation/v1",
    capturedAtUtc = DateTimeOffset.UtcNow,
    success = failures.Count == 0,
    api,
    graphics,
    evidenceBoundary = new
    {
        strictGpu = "verified-current-machine-wgl-opengl-hwnd",
        resize = "verified-current-machine-hwnd",
        dpi = "verified-current-scale-generation;cross-monitor-dpi-transition-not-run",
        deviceLoss = "verified-current-machine-injected-native-surface-loss-and-recovery",
        automation = "run-separately-by-eng-validate-g5-2",
    },
    failures,
};
WriteJson(outputPath, evidence);
Console.WriteLine($"G5-2 product API/strict-GPU validation: {(failures.Count == 0 ? "PASS" : "FAIL")}");
foreach (var failure in failures) Console.Error.WriteLine(failure);
Console.WriteLine($"Evidence: {outputPath}");
return failures.Count == 0 ? 0 : 1;

static object ValidatePromotionApi(string root, List<string> failures)
{
    var manifestPath = Path.Combine(root, "migration", "flutter-framework", "g5-2-api-manifest.json");
    var dispositionPath = Path.Combine(root, "migration", "flutter-framework", "g5-2-painting-rendering-semantics-disposition.json");
    using var manifest = JsonDocument.Parse(File.ReadAllText(manifestPath));
    using var disposition = JsonDocument.Parse(File.ReadAllText(dispositionPath));
    var entries = disposition.RootElement.GetProperty("entries").EnumerateArray().ToArray();
    var unowned = entries.Count(entry => entry.GetProperty("disposition").GetString() != "promoted" ||
        !File.Exists(Path.Combine(root, entry.GetProperty("target").GetString()!.Replace('/', Path.DirectorySeparatorChar))));
    var counts = manifest.RootElement.GetProperty("counts");
    var selected = counts.GetProperty("selectedDeclarations").GetInt32();
    var publicDeclarations = counts.GetProperty("declarationOccurrences").GetInt32();
    var publicMembers = counts.GetProperty("publicMembers").GetInt32();
    var generatedProductSources = new[] { "Painting", "Rendering", "Semantics" }
        .Sum(partition => Directory.EnumerateFiles(
            Path.Combine(root, "src", $"Doroti.Framework.{partition}"), "*.g.cs", SearchOption.TopDirectoryOnly).Count());

    var candidateRoot = Path.Combine(root, "migration", "generated-candidates", "flutter-framework", "56b8e1a851a594b1a154f8ea93270807dab22b9a", "g4-5", "projects");
    var candidateAssemblies = new[]
    {
        Load(Path.Combine(candidateRoot, "Painting", "bin", "Release", "net10.0", "Doroti.Generated.Framework.PaintingRenderingSemantics.Painting.dll")),
        Load(Path.Combine(candidateRoot, "Semantics", "bin", "Release", "net10.0", "Doroti.Generated.Framework.PaintingRenderingSemantics.Semantics.dll")),
        Load(Path.Combine(candidateRoot, "Rendering", "bin", "Release", "net10.0", "Doroti.Generated.Framework.PaintingRenderingSemantics.Rendering.dll")),
    };
    var productAssemblies = new[]
    {
        typeof(global::Doroti.Generated.Framework.Painting.Alignment).Assembly,
        typeof(global::Doroti.Generated.Framework.Semantics.SemanticsNode).Assembly,
        typeof(global::Doroti.Generated.Framework.Rendering.RenderObject).Assembly,
    };
    var candidateSurface = candidateAssemblies.SelectMany(PublicSurface).ToHashSet(StringComparer.Ordinal);
    var productSurface = productAssemblies.SelectMany(PublicSurface).ToHashSet(StringComparer.Ordinal);
    var missing = candidateSurface.Except(productSurface).Order(StringComparer.Ordinal).ToArray();
    var extra = productSurface.Except(candidateSurface).Order(StringComparer.Ordinal).ToArray();
    Require(entries.Length == 663 && selected == 663 && unowned == 0,
        $"promotion: disposition is {entries.Length}/{selected} with {unowned} unowned targets.", failures);
    Require(generatedProductSources == 0, $"promotion: {generatedProductSources} product .g.cs files remain.", failures);
    Require(missing.Length == 0 && extra.Length == 0,
        $"promotion: public API differs (missing {missing.Length}, extra {extra.Length}).", failures);
    return new
    {
        selectedDeclarations = selected,
        publicDeclarations,
        publicMembers,
        dispositionEntries = entries.Length,
        unowned,
        generatedProductSources,
        missing = missing.Length,
        extra = extra.Length,
    };
}

static object ValidateNativeGraphics(List<string> failures)
{
    using var backend = new DesktopWindowBackend();
    using var dispatcher = new PlatformDispatcher();
    using var dispatcherScope = dispatcher.EnterScope();
    using var host = new DesktopFrameworkHost(backend);
    using var view = host.CreateView(dispatcher, 52, new("Doroti G5-2 strict GPU", new(420, 280)));
    view.Show();
    PumpFor(backend, TimeSpan.FromMilliseconds(100));

    var paragraph = view.LayoutParagraph(
        new("Doroti G5-2 한글 HarfBuzz", 320, null, 22),
        DartUiInvocation.Managed("g5-2#paragraph"));
    Require(paragraph.width > 0 && paragraph.height > 0, "graphics: paragraph shaping returned empty metrics.", failures);

    using var encodedBitmap = new SKBitmap(2, 2);
    encodedBitmap.Erase(new SKColor(255, 64, 32, 255));
    using var encodedImage = SKImage.FromBitmap(encodedBitmap);
    using var encodedData = encodedImage.Encode(SKEncodedImageFormat.Png, 100);
    var png = encodedData.ToArray();
    using UiImage image = view.DecodeImageAsync(png, DartUiInvocation.Managed("g5-2#image")).AsTask().GetAwaiter().GetResult();
    Require(image.width == 2 && image.height == 2, $"graphics: image decode returned {image.width}x{image.height}.", failures);

    const int burstFrames = 60;
    for (var index = 0; index < burstFrames; index++)
    {
        SubmitProbe(view, 52, index, paragraph, image, 420, 280);
    }
    PumpUntil(backend, () => Terminal(host.GetFrameDiagnostics(52)) == burstFrames, TimeSpan.FromSeconds(15));
    var beforeResize = host.GetFrameDiagnostics(52);
    Require(Terminal(beforeResize) == burstFrames, $"graphics: only {Terminal(beforeResize)}/{burstFrames} burst terminal ACKs arrived.", failures);
    Require(beforeResize.QueueHighWatermark <= 2, $"graphics: mailbox high watermark was {beforeResize.QueueHighWatermark}.", failures);
    Require(!beforeResize.SoftwareFallbackUsed, "graphics: strict GPU selected a software fallback.", failures);

    const int sustainedPresentedFrames = 180;
    var attempts = 0;
    while (host.GetFrameDiagnostics(52).Presented < sustainedPresentedFrames && attempts < sustainedPresentedFrames + 12)
    {
        var terminalBefore = Terminal(host.GetFrameDiagnostics(52));
        SubmitProbe(view, 52, attempts + 1000, paragraph, image, 420, 280);
        PumpUntil(backend, () => Terminal(host.GetFrameDiagnostics(52)) > terminalBefore, TimeSpan.FromSeconds(3));
        attempts++;
    }
    var sustained = host.GetFrameDiagnostics(52);
    Require(sustained.Presented >= sustainedPresentedFrames,
        $"graphics: sustained strict-GPU run presented {sustained.Presented}/{sustainedPresentedFrames} frames in {attempts} attempts.", failures);
    Require(sustained.QueueDepth == 0 && sustained.ActiveFrames == 0 && sustained.ActiveContexts == 1,
        $"graphics: sustained resources did not settle (queue={sustained.QueueDepth}, frames={sustained.ActiveFrames}, contexts={sustained.ActiveContexts}).", failures);

    var beforeDeviceLoss = sustained;
    host.FailNextGpuFrameForValidation(52);
    SubmitProbe(view, 52, 9000, paragraph, image, 420, 280);
    PumpUntil(backend, () => Terminal(host.GetFrameDiagnostics(52)) > Terminal(beforeDeviceLoss), TimeSpan.FromSeconds(8));
    var afterDeviceLoss = host.GetFrameDiagnostics(52);
    Require(afterDeviceLoss.Failed == beforeDeviceLoss.Failed + 1,
        $"graphics: injected device loss produced {afterDeviceLoss.Failed - beforeDeviceLoss.Failed} failed ACKs instead of one.", failures);
    Require(afterDeviceLoss.RecoveryCount == beforeDeviceLoss.RecoveryCount + 1 &&
        afterDeviceLoss.SurfaceGeneration > beforeDeviceLoss.SurfaceGeneration,
        "graphics: injected device loss did not advance recovery/surface generation exactly once.", failures);

    var presentedBeforeRecovery = afterDeviceLoss.Presented;
    SubmitProbe(view, 52, 9001, paragraph, image, 420, 280);
    PumpUntil(backend, () => Terminal(host.GetFrameDiagnostics(52)) > Terminal(afterDeviceLoss), TimeSpan.FromSeconds(8));
    var afterRecovery = host.GetFrameDiagnostics(52);
    Require(afterRecovery.Presented == presentedBeforeRecovery + 1,
        "graphics: the first post-loss frame was not presented on the recreated strict-GPU surface.", failures);
    Require(!afterRecovery.SoftwareFallbackUsed,
        "graphics: native device-loss recovery selected a software fallback.", failures);

    view.Resize(new(500, 320));
    PumpFor(backend, TimeSpan.FromMilliseconds(150));
    var resizeTerminal = Terminal(host.GetFrameDiagnostics(52));
    {
        var recorder = new PictureRecorder();
        var canvas = new Canvas(recorder, Rect.fromLTWH(0, 0, 500, 320));
        canvas.drawRect(Rect.fromLTWH(30, 30, 180, 100), new Paint { color = new UiColor(0xff38a169) });
        using var picture = recorder.endRecording();
        var builder = new SceneBuilder(52);
        builder.addPicture(Offset.zero, picture);
        using var scene = builder.build();
        view.render(scene);
    }
    PumpUntil(backend, () => Terminal(host.GetFrameDiagnostics(52)) > resizeTerminal, TimeSpan.FromSeconds(8));
    var afterResize = host.GetFrameDiagnostics(52);
    Require(Terminal(afterResize) > resizeTerminal, "graphics: resized frame did not receive a terminal ACK.", failures);

    double visualMeanAbsoluteError;
    using (var probe = backend.CreateWindow(new("Doroti G5-2 visual baseline", new(320, 200)), new WindowSink()))
    {
        probe.Show();
        PumpFor(backend, TimeSpan.FromMilliseconds(100));
        visualMeanAbsoluteError = DesktopGpuFrameSink.MeasureVisualTolerance(probe);
        probe.Close();
        backend.PumpPendingMessages();
    }
    Require(visualMeanAbsoluteError <= 2.0, $"graphics: GPU/software baseline MAE {visualMeanAbsoluteError:R} exceeded 2.0.", failures);
    return new
    {
        submittedFrames = afterResize.Submitted,
        sustainedPresentedFrames,
        sustainedAttempts = attempts,
        afterResize.Presented,
        afterResize.Superseded,
        afterResize.Stale,
        afterResize.Failed,
        afterResize.Cancelled,
        afterResize.QueueHighWatermark,
        afterResize.QueueDepth,
        afterResize.ActiveContexts,
        afterResize.ActiveFrames,
        afterResize.SurfaceGeneration,
        afterResize.RecoveryCount,
        afterResize.BackendIdentity,
        afterResize.SoftwareFallbackUsed,
        paragraph = new { paragraph.width, paragraph.height },
        decodedImage = new { image.width, image.height },
        visualMeanAbsoluteError,
        visualTolerance = 2.0,
        metrics = new { view.metrics.devicePixelRatio, view.metrics.generation, view.metrics.surfaceGeneration },
    };
}

static void SubmitProbe(DorotiView view, ulong viewId, int index, Paragraph paragraph, UiImage image, double width, double height)
{
    var recorder = new PictureRecorder();
    var canvas = new Canvas(recorder, Rect.fromLTWH(0, 0, width, height));
    canvas.drawPaint(new Paint { color = new UiColor(0xff0d1526) });
    canvas.drawRect(Rect.fromLTWH(12 + (index % 17), 16, 160, 84), new Paint { color = new UiColor(0xff26648b) });
    var path = new UiPath();
    path.moveTo(20, 130);
    path.lineTo(95, 190);
    path.lineTo(170, 130);
    path.close();
    canvas.drawPath(path, new Paint { color = new UiColor(0xffffc857) });
    canvas.drawParagraph(paragraph, new(24, 70));
    canvas.drawImageRect(image, Rect.fromLTWH(0, 0, 2, 2), Rect.fromLTWH(210, 30, 96, 96), new Paint());
    using var picture = recorder.endRecording();
    var builder = new SceneBuilder(viewId);
    builder.addPicture(Offset.zero, picture);
    using var scene = builder.build();
    view.render(scene);
}

static int RunAutomationTarget(string[] args)
{
    if (args.Length != 3) throw new ArgumentException("Automation target requires ready, actions and stop paths.");
    var readyPath = Path.GetFullPath(args[0]);
    var actionsPath = Path.GetFullPath(args[1]);
    var stopPath = Path.GetFullPath(args[2]);
    using var backend = new DesktopWindowBackend();
    using var dispatcher = new PlatformDispatcher();
    using var scope = dispatcher.EnterScope();
    using var host = new DesktopFrameworkHost(backend);
    using var view = host.CreateView(dispatcher, 53, new("Doroti G5-2 Automation Target", new(520, 360)));
    var actions = new List<object>();
    dispatcher.onSemanticsActionEvent = action =>
    {
        lock (actions)
        {
            actions.Add(new { action.nodeId, action = action.action.ToString(), action.arguments });
            WriteJson(actionsPath, new { capturedAtUtc = DateTimeOffset.UtcNow, actions });
        }
    };
    view.Show();
    dispatcher.setSemanticsTreeEnabled(true);
    var nodes = new[]
    {
        new SemanticsNodeUpdate(1, Rect.fromLTWH(0, 0, 500, 340), "Doroti content", null, UiSemanticsAction.none, [2, 3, 4, 5]),
        new SemanticsNodeUpdate(2, Rect.fromLTWH(20, 20, 180, 50), "Invoke button", null, UiSemanticsAction.tap | UiSemanticsAction.focus, [], new(isButton: true, isEnabled: Tristate.isTrue)),
        new SemanticsNodeUpdate(3, Rect.fromLTWH(20, 90, 180, 50), "Toggle option", "off", UiSemanticsAction.tap, [], new(isToggled: Tristate.isFalse, isEnabled: Tristate.isTrue)),
        new SemanticsNodeUpdate(4, Rect.fromLTWH(20, 160, 280, 50), "Text value", "initial", UiSemanticsAction.setText | UiSemanticsAction.focus, [], new(isTextField: true, isEnabled: Tristate.isTrue)),
        new SemanticsNodeUpdate(5, Rect.fromLTWH(320, 20, 160, 260), "Scrollable list", null, UiSemanticsAction.scrollUp | UiSemanticsAction.scrollDown, [], new(hasImplicitScrolling: true, isEnabled: Tristate.isTrue), UiSemanticsRole.list),
    };
    view.updateSemantics(new(1, nodes));
    PumpFor(backend, TimeSpan.FromMilliseconds(150));
    WriteJson(readyPath, new { processId = Environment.ProcessId, windowHandle = host.GetNativeWindowHandle(53).ToInt64(), title = "Doroti G5-2 Automation Target", nodeCount = nodes.Length });
    var timeout = DateTime.UtcNow + TimeSpan.FromSeconds(45);
    while (!File.Exists(stopPath) && DateTime.UtcNow < timeout)
    {
        backend.PumpPendingMessages();
        Thread.Sleep(2);
    }
    lock (actions) WriteJson(actionsPath, new { capturedAtUtc = DateTimeOffset.UtcNow, actions });
    return File.Exists(stopPath) ? 0 : 3;
}

static long Terminal(DesktopFrameworkFrameDiagnostics value) =>
    value.Presented + value.Superseded + value.Stale + value.Failed + value.Cancelled;

static void PumpUntil(DesktopWindowBackend backend, Func<bool> completed, TimeSpan timeout)
{
    var deadline = DateTime.UtcNow + timeout;
    while (!completed() && DateTime.UtcNow < deadline)
    {
        backend.PumpPendingMessages();
        Thread.Sleep(1);
    }
    backend.PumpPendingMessages();
}

static void PumpFor(DesktopWindowBackend backend, TimeSpan duration) =>
    PumpUntil(backend, () => false, duration);

static Assembly Load(string path) => AssemblyLoadContext.Default.LoadFromAssemblyPath(path);

static IEnumerable<string> PublicSurface(Assembly assembly)
{
    foreach (var type in assembly.GetExportedTypes().OrderBy(type => type.FullName, StringComparer.Ordinal))
    {
        var owner = TypeName(type);
        yield return $"T|{owner}|{TypeName(type.BaseType)}|{string.Join(',', type.GetInterfaces().Select(TypeName).Order(StringComparer.Ordinal))}";
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
        foreach (var constructor in type.GetConstructors(flags)) yield return $"C|{owner}|{Parameters(constructor.GetParameters())}";
        foreach (var method in type.GetMethods(flags).Where(method => !method.IsSpecialName)) yield return $"M|{owner}|{method.Name}|{TypeName(method.ReturnType)}|{Parameters(method.GetParameters())}|{method.GetGenericArguments().Length}";
        foreach (var property in type.GetProperties(flags)) yield return $"P|{owner}|{property.Name}|{TypeName(property.PropertyType)}|{Parameters(property.GetIndexParameters())}|{property.GetMethod is not null}|{property.SetMethod is not null}";
        foreach (var field in type.GetFields(flags)) yield return $"F|{owner}|{field.Name}|{TypeName(field.FieldType)}|{field.IsLiteral}";
        foreach (var @event in type.GetEvents(flags)) yield return $"E|{owner}|{@event.Name}|{TypeName(@event.EventHandlerType)}";
    }
}

static string Parameters(IEnumerable<ParameterInfo> parameters) => string.Join(',', parameters.Select(parameter =>
    $"{TypeName(parameter.ParameterType)}:{parameter.IsOptional}:{parameter.IsOut}"));

static string TypeName(Type? type)
{
    if (type is null) return "";
    if (type.IsByRef) return TypeName(type.GetElementType()) + "&";
    if (type.IsArray) return TypeName(type.GetElementType()) + "[]";
    if (!type.IsGenericType) return type.FullName ?? type.Name;
    var definition = type.GetGenericTypeDefinition().FullName ?? type.Name;
    return definition[..definition.IndexOf('`')] + "<" + string.Join(',', type.GetGenericArguments().Select(TypeName)) + ">";
}

static string FindDorotiRoot(string start)
{
    for (var current = new DirectoryInfo(Path.GetFullPath(start)); current is not null; current = current.Parent)
    {
        var nested = Path.Combine(current.FullName, "Doroti", "Doroti.slnx");
        if (File.Exists(nested)) return Path.GetDirectoryName(nested)!;
        if (File.Exists(Path.Combine(current.FullName, "Doroti.slnx"))) return current.FullName;
    }
    throw new DirectoryNotFoundException("Could not locate Doroti.slnx.");
}

static void WriteJson(string path, object value)
{
    Directory.CreateDirectory(Path.GetDirectoryName(path)!);
    var temporary = path + ".tmp-" + Guid.NewGuid().ToString("N");
    File.WriteAllText(temporary, JsonSerializer.Serialize(value, new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    }) + "\n", new UTF8Encoding(false));
    File.Move(temporary, path, true);
}

static void Require(bool condition, string failure, List<string> failures)
{
    if (!condition) failures.Add(failure);
}

sealed class WindowSink : IWindowEventSink
{
    public void OnMetricsChanged(WindowId window, WindowMetrics metrics) { }
    public void OnCloseRequested(WindowId window) { }
    public void OnClosed(WindowId window) { }
}
