using System.Reflection;
using System.Text;
using System.Text.Json;
using Doroti.Hosting;
using Doroti.Runtime;
using Doroti.Ui;
using Path = System.IO.Path;

var dorotiRoot = FindDorotiRoot(Environment.CurrentDirectory);
var failures = new List<string>();

ValidateAssemblyBoundaries(dorotiRoot, failures);
ValidateFailClosed(failures);
await ValidateTwoViewsAsync(failures);
ValidateBootstrap(failures);
ValidateDesktopPublicApi(dorotiRoot, failures);
ValidateCompilerBinding(dorotiRoot, failures);

var report = new G4UiValidationReport(
    "doroti.flutter-avalonia-bridge-validation/v1",
    "G4-1",
    failures.Count == 0,
    new(
        HostReferences(typeof(DartRuntimePrimitives).Assembly).Length,
        HostReferences(typeof(PlatformDispatcher).Assembly).Length,
        !failures.Any(item => item.StartsWith("two-view", StringComparison.Ordinal)),
        !failures.Any(item => item.StartsWith("fail-closed", StringComparison.Ordinal)),
        !failures.Any(item => item.StartsWith("bootstrap", StringComparison.Ordinal)),
        failures.Count(item => item.StartsWith("desktop-public-api", StringComparison.Ordinal)),
        !failures.Any(item => item.StartsWith("compiler-binding", StringComparison.Ordinal))),
    failures.ToArray());

var artifactDirectory = Path.Combine(dorotiRoot, "migration", "flutter-avalonia", "bridge-validation");
Directory.CreateDirectory(artifactDirectory);
var artifactPath = Path.Combine(artifactDirectory, "g4-1.json");
File.WriteAllText(
    artifactPath,
    JsonSerializer.Serialize(report, new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    }) + "\n",
    new UTF8Encoding(false));

Console.WriteLine($"G4-1 dart:ui/Avalonia ABI validation: {(report.Success ? "PASS" : "FAIL")}");
Console.WriteLine($"Artifact: {artifactPath}");
foreach (var failure in failures)
{
    Console.Error.WriteLine(failure);
}
return report.Success ? 0 : 2;

static void ValidateAssemblyBoundaries(string dorotiRoot, List<string> failures)
{
    var runtimeReferences = HostReferences(typeof(DartRuntimePrimitives).Assembly);
    if (runtimeReferences.Length != 0)
    {
        failures.Add($"runtime-boundary: host references found: {string.Join(", ", runtimeReferences)}");
    }
    var uiReferences = HostReferences(typeof(PlatformDispatcher).Assembly);
    if (uiReferences.Length != 0)
    {
        failures.Add($"ui-boundary: host references found: {string.Join(", ", uiReferences)}");
    }

    var runtimeProject = File.ReadAllText(Path.Combine(dorotiRoot, "src", "Doroti.Runtime", "Doroti.Runtime.csproj"));
    if (runtimeProject.Contains("ProjectReference", StringComparison.Ordinal))
    {
        failures.Add("runtime-boundary: Doroti.Runtime must have zero project references.");
    }
}

static string[] HostReferences(Assembly assembly) => assembly.GetReferencedAssemblies()
    .Select(reference => reference.Name ?? string.Empty)
    .Where(name => name.StartsWith("Doroti.Host.", StringComparison.Ordinal) ||
        name.StartsWith("Doroti.Shell.", StringComparison.Ordinal) ||
        name.StartsWith("Doroti.Vendor.", StringComparison.Ordinal) ||
        name.StartsWith("Doroti.Backends.", StringComparison.Ordinal) ||
        name is "Doroti.Core" or "Doroti.Platform" or "Doroti.Graphics" or "Doroti.Composition" or "Doroti.Engine" or "SkiaSharp" ||
        name.StartsWith("Avalonia", StringComparison.Ordinal) ||
        name.StartsWith("Windows.Win32", StringComparison.Ordinal))
    .Order(StringComparer.Ordinal)
    .ToArray();

static void ValidateFailClosed(List<string> failures)
{
    using var dispatcher = new PlatformDispatcher();
    var host = new FakeViewHost(1, new(new(800, 600), 1, ViewPadding.zero, ViewPadding.zero, ViewPadding.zero, AppLifecycleState.resumed, 1, 1));
    using var view = dispatcher.RegisterView(1, new DorotiViewCapabilities()
        .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, host));
    var invocation = new DartUiInvocation(
        "dart:ui#PlatformDispatcher.scheduleFrame",
        new("packages/flutter/lib/src/scheduler/binding.dart", 412, 13));
    try
    {
        view.ScheduleFrame(invocation);
        failures.Add("fail-closed: a missing frame capability silently succeeded.");
    }
    catch (DorotiCapabilityException exception)
    {
        if (exception.CapabilityId != DorotiCapabilityIds.ViewFrameDispatch ||
            exception.ViewId != 1 ||
            exception.ElementId != invocation.ElementId ||
            exception.SourceSpan != invocation.SourceSpan)
        {
            failures.Add("fail-closed: typed diagnostic lost its capability, view, canonical element ID, or source span.");
        }
    }
}

static async Task ValidateTwoViewsAsync(List<string> failures)
{
    using var dispatcher = new PlatformDispatcher();
    var firstHost = new FakeViewHost(11, new(new(800, 600), 1, ViewPadding.zero, ViewPadding.zero, ViewPadding.zero, AppLifecycleState.resumed, 1, 1));
    var secondHost = new FakeViewHost(22, new(new(2400, 1600), 2, ViewPadding.zero, ViewPadding.zero, ViewPadding.zero, AppLifecycleState.resumed, 8, 3));
    var firstMessages = new FakeMessages(0x11);
    var secondMessages = new FakeMessages(0x22);
    using var first = dispatcher.RegisterView(11, Capabilities(firstHost, firstMessages));
    using var second = dispatcher.RegisterView(22, Capabilities(secondHost, secondMessages));

    var metricsCallbacks = new List<ulong>();
    var pointerCallbacks = new List<ulong>();
    var frameCallbacks = new List<string>();
    dispatcher.onMetricsChanged = view => metricsCallbacks.Add(view.viewId);
    dispatcher.onPointerDataPacket = (view, packet) => pointerCallbacks.Add(packet.data.Single().viewId == view.viewId ? view.viewId : 0);
    dispatcher.beginFrame += (view, _) => frameCallbacks.Add($"begin:{view.viewId}");
    dispatcher.drawFrame += view => frameCallbacks.Add($"draw:{view.viewId}");

    firstHost.SetMetrics(firstHost.Metrics with { physicalSize = new(900, 700), generation = 2 });
    secondHost.EmitPointer(25, 50);
    first.ScheduleFrame(DartUiInvocation.Managed("dart:ui#PlatformDispatcher.scheduleFrame"));
    var firstResponse = await first.SendPlatformMessageAsync("test", null, DartUiInvocation.Managed("dart:ui#PlatformDispatcher.sendPlatformMessage"));
    var secondResponse = await second.SendPlatformMessageAsync("test", null, DartUiInvocation.Managed("dart:ui#PlatformDispatcher.sendPlatformMessage"));

    if (!metricsCallbacks.SequenceEqual([11UL]) || first.metrics.generation != 2 || second.metrics.generation != 8)
    {
        failures.Add("two-view: metrics state crossed view boundaries.");
    }
    if (!pointerCallbacks.SequenceEqual([22UL]))
    {
        failures.Add("two-view: pointer routing crossed view boundaries.");
    }
    if (!frameCallbacks.SequenceEqual(["begin:11", "draw:11"]))
    {
        failures.Add("two-view: begin-frame/draw-frame ordering or view routing is incorrect.");
    }
    if (firstResponse?.Span[0] != 0x11 || secondResponse?.Span[0] != 0x22)
    {
        failures.Add("two-view: platform message state crossed view boundaries.");
    }

    var firstRelease = 0;
    var secondRelease = 0;
    using (var firstImage = new Image(11, 1, 1, () => firstRelease++))
    using (var secondImage = new Image(22, 1, 1, () => secondRelease++))
    {
        firstImage.Dispose();
        if (firstRelease != 1 || secondRelease != 0 || firstImage.viewId == secondImage.viewId)
        {
            failures.Add("two-view: resource lifetime state crossed view boundaries.");
        }
    }
    if (firstRelease != 1 || secondRelease != 1)
    {
        failures.Add("two-view: resource release was not exactly once per view.");
    }
}

static DorotiViewCapabilities Capabilities(FakeViewHost host, FakeMessages messages) => new DorotiViewCapabilities()
    .Register<IViewHostCapability>(DorotiCapabilityIds.WindowLifecycle, host)
    .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, host)
    .Register<IInputHostCapability>(DorotiCapabilityIds.InputEvents, host)
    .Register<IFrameHostCapability>(DorotiCapabilityIds.ViewFrameDispatch, host)
    .Register<IPlatformMessageHostCapability>(DorotiCapabilityIds.PlatformMessaging, messages);

static void ValidateBootstrap(List<string> failures)
{
    var entrypoint = new FakeEntrypoint();
    using var session = new DorotiHostSession(entrypoint);
    session.Start();
    session.Shutdown();
    session.Shutdown();
    if (entrypoint.BootstrapCount != 1 || entrypoint.ShutdownCount != 1 || session.state != DorotiHostSessionState.shutDown)
    {
        failures.Add("bootstrap: bootstrap/shutdown was not exactly once.");
    }
}

static void ValidateDesktopPublicApi(string dorotiRoot, List<string> failures)
{
    var assemblyPath = Path.Combine(dorotiRoot, "src", "Doroti.Host.Desktop.Framework", "bin", "Debug", "net10.0", "Doroti.Host.Desktop.Framework.dll");
    if (!File.Exists(assemblyPath))
    {
        failures.Add("desktop-public-api: desktop Flutter adapter assembly was not built.");
        return;
    }
    var assembly = Assembly.LoadFrom(assemblyPath);
    var forbidden = new[] { "Avalonia", "SkiaSharp", "Windows.Win32", "HWND", "Hwnd" };
    foreach (var type in assembly.GetExportedTypes())
    {
        foreach (var signatureType in PublicSignatureTypes(type))
        {
            var name = signatureType.FullName ?? signatureType.Name;
            if (forbidden.Any(prefix => name.Contains(prefix, StringComparison.Ordinal)))
            {
                failures.Add($"desktop-public-api: forbidden concrete type leaked: {type.FullName} -> {name}");
            }
        }
    }
}

static IEnumerable<Type> PublicSignatureTypes(Type type)
{
    yield return type;
    foreach (var constructor in type.GetConstructors())
        foreach (var parameter in constructor.GetParameters())
        {
            yield return Unwrap(parameter.ParameterType);
        }
    foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly))
    {
        yield return Unwrap(method.ReturnType);
        foreach (var parameter in method.GetParameters())
        {
            yield return Unwrap(parameter.ParameterType);
        }
    }
    foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly))
    {
        yield return Unwrap(property.PropertyType);
    }
}

static Type Unwrap(Type type) => type.IsByRef || type.IsPointer || type.IsArray ? type.GetElementType()! : type;

static void ValidateCompilerBinding(string dorotiRoot, List<string> failures)
{
    var repositoryRoot = Directory.GetParent(dorotiRoot)!.FullName;
    var loweringDirectory = Path.Combine(repositoryRoot, "tools", "Doroti.DartToCSharp", "src", "Backend", "CSharp", "Lowering");
    var lowerer = string.Join("\n", Directory.GetFiles(loweringDirectory, "*.cs", SearchOption.AllDirectories)
        .OrderBy(path => path, StringComparer.Ordinal)
        .Select(File.ReadAllText));
    var graph = File.ReadAllText(Path.Combine(repositoryRoot, "tools", "Doroti.DartToCSharp", "src", "Application", "FrameworkProjectGraph.cs"));
    var fingerprint = File.ReadAllText(Path.Combine(repositoryRoot, "tools", "Doroti.DartToCSharp", "src", "Identity", "WorkspaceFingerprint.cs"));
    if (!lowerer.Contains("using Doroti.Ui;", StringComparison.Ordinal) ||
        !lowerer.Contains("global::Doroti.Ui.", StringComparison.Ordinal) ||
        !graph.Contains("Doroti.Ui.csproj", StringComparison.Ordinal) ||
        !graph.Contains("Doroti.Ui\"", StringComparison.Ordinal) ||
        !fingerprint.Contains("dart-ui-contract:", StringComparison.Ordinal))
    {
        failures.Add("compiler-binding: generated framework code/project graph is not bound to Doroti.Ui.");
    }
}

static string FindDorotiRoot(string start)
{
    for (var directory = new DirectoryInfo(Path.GetFullPath(start)); directory is not null; directory = directory.Parent)
    {
        if (File.Exists(Path.Combine(directory.FullName, "Doroti.slnx")) && Directory.Exists(Path.Combine(directory.FullName, "migration")))
        {
            return directory.FullName;
        }
        var nested = Path.Combine(directory.FullName, "Doroti", "Doroti.slnx");
        if (File.Exists(nested))
        {
            return Path.GetDirectoryName(nested)!;
        }
    }
    throw new DirectoryNotFoundException($"Could not find Doroti root from {start}.");
}

internal sealed class FakeViewHost : IViewHostCapability, IInputHostCapability, IFrameHostCapability
{
    private readonly ulong _viewId;

    internal FakeViewHost(ulong viewId, ViewMetrics metrics)
    {
        _viewId = viewId;
        Metrics = metrics;
    }

    public ViewMetrics Metrics { get; private set; }

    public event Action<ViewMetrics>? MetricsChanged;
    public event Action<AppLifecycleState>? LifecycleChanged;
    public event Action? CloseRequested;
    public event Action? Closed;
    public event Action<PointerDataPacket>? PointerData;
    public event Action<KeyData>? KeyData;
    public event Action<RawFocusData>? FocusData;

    public void SetMetrics(ViewMetrics metrics)
    {
        Metrics = metrics;
        MetricsChanged?.Invoke(metrics);
    }

    public void EmitPointer(double x, double y) => PointerData?.Invoke(new([
        new(_viewId, TimeSpan.Zero, PointerChange.hover, PointerDeviceKind.mouse, 1, x, y, 0, 0, 0),
    ]));

    public void ScheduleFrame(Action<TimeSpan> callback) => callback(TimeSpan.FromMilliseconds(16));
    public void Show() { }
    public void Resize(Size logicalSize) { }
    public void Close() => Closed?.Invoke();
    public void Dispose() { }

    internal void KeepCompilerHappy()
    {
        LifecycleChanged?.Invoke(AppLifecycleState.resumed);
        CloseRequested?.Invoke();
        KeyData?.Invoke(default);
        FocusData?.Invoke(default);
    }
}

internal sealed class FakeMessages(byte value) : IPlatformMessageHostCapability
{
    public ValueTask<ReadOnlyMemory<byte>?> SendAsync(string channel, ReadOnlyMemory<byte>? data, CancellationToken cancellationToken = default) =>
        ValueTask.FromResult<ReadOnlyMemory<byte>?>(new byte[] { value });

    public void SetMessageHandler(string channel, PlatformMessageHandler? handler) { }
}

internal sealed class FakeEntrypoint : IDorotiFrameworkEntrypoint
{
    public int BootstrapCount { get; private set; }
    public int ShutdownCount { get; private set; }
    public void Bootstrap(PlatformDispatcher dispatcher) => BootstrapCount++;
    public void Shutdown() => ShutdownCount++;
}

internal sealed record G4UiValidationReport(
    string SchemaVersion,
    string Milestone,
    bool Success,
    G4UiValidationSummary Summary,
    string[] Findings);

internal sealed record G4UiValidationSummary(
    int RuntimeHostReferenceCount,
    int UiHostReferenceCount,
    bool TwoViewIsolation,
    bool FailClosedDiagnostic,
    bool BootstrapShutdown,
    int DesktopPublicTypeLeakCount,
    bool CompilerDartUiBinding);
