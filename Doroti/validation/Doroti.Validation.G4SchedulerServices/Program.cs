using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Text.Json;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using Doroti.Generated.Framework.Scheduler;
using Doroti.Generated.Framework.Services;
using Path = System.IO.Path;

var dorotiRoot = FindDorotiRoot(Environment.CurrentDirectory);
var failures = new List<string>();
var trace = new List<string>();

using var dispatcher = new PlatformDispatcher();
using var dispatcherScope = dispatcher.EnterScope();
var host = new FixtureHost();
var capabilities = new FlutterViewCapabilities()
    .Register<IViewHostCapability>(FlutterCapabilityIds.WindowLifecycle, host)
    .Register<IViewHostCapability>(FlutterCapabilityIds.ViewLifecycleMetrics, host)
    .Register<IFrameHostCapability>(FlutterCapabilityIds.ViewFrameDispatch, host)
    .Register<IInputHostCapability>(FlutterCapabilityIds.InputEvents, host)
    .Register<IPlatformMessageHostCapability>(FlutterCapabilityIds.PlatformMessaging, host)
    .Register<IPlatformServicesHostCapability>(FlutterCapabilityIds.PlatformServices, host)
    .Register<ITextInputHostCapability>(FlutterCapabilityIds.TextInput, host)
    .Register<IPlatformEnvironmentHostCapability>(FlutterCapabilityIds.PlatformEnvironment, host);
using var view = dispatcher.RegisterView(43, capabilities);
using var environmentScope = PlatformEnvironmentContext.Enter(host.Configuration);
using var services = new FixtureServicesBinding(dispatcher);
SchedulerBinding scheduler = services;
dispatcher.onKeyData ??= services.keyEventManager.handleKeyData;

ValidateBoundaries(dorotiRoot, failures);
ValidateInventory(dorotiRoot, failures);
var promotion = ValidatePromotion(dorotiRoot, failures);
ValidateFrameOrdering(dispatcher, host, scheduler, trace, failures);
ValidateMetricsAndLifecycle(dispatcher, host, view, scheduler, trace, failures);
ValidateInput(dispatcher, host, view, services, trace, failures);
await ValidateChannelsAndServicesAsync(host, view, services, trace, failures);
ValidateAssetsAndRestoration(failures);
ValidateFailClosed(dispatcher, failures);

var success = failures.Count == 0;
var evidenceDirectory = Path.Combine(dorotiRoot, "migration", "flutter-avalonia", "bridge-validation");
Directory.CreateDirectory(evidenceDirectory);
var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };
var evidencePath = Path.Combine(evidenceDirectory, "g4-3.json");
var evidence = new
{
    schemaVersion = "doroti.g5-0-truth-reset/v2",
    milestone = "G4-3",
    previousBaseline = new
    {
        artifactSchema = "doroti.g4-scheduler-services-evidence/v1",
        claimedSuccess = true,
        knownApiDrift = new { missing = 205, extra = 24 },
    },
    currentRun = new
    {
        capturedAtUtc = DateTimeOffset.UtcNow,
        timeoutMilliseconds = 15 * 60 * 1000,
        flutterRevision = "56b8e1a851a594b1a154f8ea93270807dab22b9a",
        sourceInventory = new { libraries = 58, declarations = 287, members = 2426 },
        candidate = new
        {
            status = failures.Any(item => item.StartsWith("candidate:", StringComparison.Ordinal)) ? "failed" : "verified",
            generatedProductSources = promotion.GeneratedProductSources,
        },
        aggregate = new
        {
            status = failures.Any(item => item.StartsWith("aggregate:", StringComparison.Ordinal)) ? "failed" : "verified",
            warnings = 0,
            errors = 0,
        },
        promotion = new
        {
            status = promotion.ApiManifestDiff == 0 && promotion.Unowned == 0 ? "verified" : "failed",
            missingCount = promotion.MissingApi.Count,
            extraCount = promotion.ExtraApi.Count,
            promotion.MissingApi,
            promotion.ExtraApi,
            promotion.Dispositions,
            promotion.Unowned,
        },
        productBehavior = new
        {
            status = success ? "verified" : "failed",
            trace,
            failures,
        },
        nativeEvidence = new
        {
            status = "not-verified",
            reasons = new[] { "No physical input device or target-machine native run is part of G5-0." },
        },
    },
};
WriteAtomicText(evidencePath, JsonSerializer.Serialize(evidence, options) + "\n");

Console.WriteLine($"G4-3 Scheduler/Services validation: {(success ? "PASS" : "FAIL")}");
foreach (var failure in failures)
{
    Console.Error.WriteLine(failure);
}
return success ? 0 : 2;

static void WriteAtomicText(string path, string contents)
{
    var temporary = path + ".tmp-" + Guid.NewGuid().ToString("N");
    File.WriteAllText(temporary, contents, new UTF8Encoding(false));
    File.Move(temporary, path, true);
}

static void ValidateBoundaries(string root, List<string> failures)
{
    var forbiddenPrefixes = new[] { "Doroti.Host.", "Doroti.Shell.", "Doroti.Vendor.", "Doroti.Platform", "Doroti.Graphics", "Doroti.Engine", "Doroti.Rendering", "Doroti.Widgets", "SkiaSharp", "Avalonia", "Windows.Win32" };
    foreach (var assembly in new[] { typeof(SchedulerBinding).Assembly, typeof(ServicesBinding).Assembly })
    {
        var forbidden = assembly.GetReferencedAssemblies().Select(item => item.Name ?? string.Empty)
            .Where(name => forbiddenPrefixes.Any(prefix => name.StartsWith(prefix, StringComparison.Ordinal))).ToArray();
        if (forbidden.Length > 0)
        {
            failures.Add($"boundary: {assembly.GetName().Name} references {string.Join(", ", forbidden)}.");
        }
    }
    foreach (var removed in new[] { "HardwareKeyboard.cs", "ChannelContracts.cs" })
    {
        if (File.Exists(Path.Combine(root, "src", "Doroti.Platform", removed)))
        {
            failures.Add($"boundary: Flutter-derived Doroti.Platform owner remains: {removed}.");
        }
    }
}

static void ValidateInventory(string root, List<string> failures)
{
    using var document = JsonDocument.Parse(File.ReadAllText(Path.Combine(root, "migration", "flutter-framework", "f1-closure.json")));
    var libraries = document.RootElement.GetProperty("libraries").EnumerateArray()
        .Where(item => item.GetProperty("path").GetString() is { } path &&
            (path.StartsWith("src/scheduler/", StringComparison.Ordinal) || path.StartsWith("src/services/", StringComparison.Ordinal)))
        .ToArray();
    var declarations = libraries.Sum(item => item.GetProperty("declarations").GetArrayLength());
    var members = libraries.Sum(item => item.GetProperty("declarations").EnumerateArray()
        .Sum(declaration => declaration.GetProperty("members").GetArrayLength()));
    if (libraries.Length != 58 || declarations != 287 || members != 2426)
    {
        failures.Add($"boundary: pinned Scheduler/Services inventory drifted ({libraries.Length}, {declarations}, {members}).");
    }
}

static PromotionResult ValidatePromotion(string root, List<string> failures)
{
    var dispositionPath = Path.Combine(root, "migration", "flutter-framework", "g4-3-scheduler-services-disposition.json");
    var manifestPath = Path.Combine(root, "migration", "flutter-framework", "g4-3-api-manifest.json");
    using var disposition = JsonDocument.Parse(File.ReadAllText(dispositionPath));
    using var manifest = JsonDocument.Parse(File.ReadAllText(manifestPath));
    var entries = disposition.RootElement.GetProperty("entries").EnumerateArray().ToArray();
    var unowned = entries.Count(entry => entry.GetProperty("disposition").GetString() != "promoted" ||
        !File.Exists(Path.Combine(root, entry.GetProperty("target").GetString()!)));
    var counts = manifest.RootElement.GetProperty("counts");
    var declarationOccurrences = counts.GetProperty("declarationOccurrences").GetInt32();
    var uniqueDeclarationNames = counts.GetProperty("uniqueDeclarationNames").GetInt32();
    var publicMembers = counts.GetProperty("publicMembers").GetInt32();
    var generatedProductSources = Directory.EnumerateFiles(Path.Combine(root, "src", "Doroti.Flutter.Framework.Scheduler"), "*.g.cs", SearchOption.TopDirectoryOnly)
        .Concat(Directory.EnumerateFiles(Path.Combine(root, "src", "Doroti.Flutter.Framework.Services"), "*.g.cs", SearchOption.TopDirectoryOnly))
        .Count();

    var candidateRoot = Environment.GetEnvironmentVariable("DOROTI_G4_3_CANDIDATE_ROOT");
    candidateRoot = string.IsNullOrWhiteSpace(candidateRoot)
        ? Path.Combine(root, "migration", "generated-candidates", "flutter-framework", "56b8e1a851a594b1a154f8ea93270807dab22b9a", "g4-3", "projects")
        : Path.GetFullPath(candidateRoot);
    var candidateScheduler = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(candidateRoot, "Scheduler", "bin", "Debug", "net10.0", "Doroti.Generated.Framework.SchedulerServices.Scheduler.dll"));
    var candidateServices = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(candidateRoot, "Services", "bin", "Debug", "net10.0", "Doroti.Generated.Framework.SchedulerServices.Services.dll"));
    var candidateSurface = PublicSurface(candidateScheduler).Concat(PublicSurface(candidateServices)).ToHashSet(StringComparer.Ordinal);
    var productSurface = PublicSurface(typeof(SchedulerBinding).Assembly).Concat(PublicSurface(typeof(ServicesBinding).Assembly)).ToHashSet(StringComparer.Ordinal);
    var missing = candidateSurface.Except(productSurface, StringComparer.Ordinal).Order(StringComparer.Ordinal).ToArray();
    var extra = productSurface.Except(candidateSurface, StringComparer.Ordinal).Order(StringComparer.Ordinal).ToArray();

    if (entries.Length != 287 || unowned != 0)
    {
        failures.Add($"boundary: promotion disposition drifted ({entries.Length} entries, {unowned} unowned/missing targets).");
    }
    if (declarationOccurrences != 235 || uniqueDeclarationNames != 233 || publicMembers != 1960)
    {
        failures.Add($"boundary: API manifest census drifted ({declarationOccurrences}/{uniqueDeclarationNames}/{publicMembers}).");
    }
    if (generatedProductSources != 0 || missing.Length != 0 || extra.Length != 0)
    {
        failures.Add($"boundary: promoted API surface drifted (missing {missing.Length}, extra {extra.Length}, product .g.cs {generatedProductSources}).");
    }
    return new(
        entries.Length,
        unowned,
        declarationOccurrences,
        uniqueDeclarationNames,
        publicMembers,
        missing.Length + extra.Length,
        generatedProductSources,
        missing,
        extra);
}

static IEnumerable<string> PublicSurface(Assembly assembly)
{
    foreach (var type in assembly.GetExportedTypes().OrderBy(type => type.FullName, StringComparer.Ordinal))
    {
        var owner = TypeName(type);
        yield return $"T|{owner}|{TypeName(type.BaseType)}|{string.Join(',', type.GetInterfaces().Select(TypeName).Order(StringComparer.Ordinal))}";
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
        foreach (var constructor in type.GetConstructors(flags))
            yield return $"C|{owner}|{Parameters(constructor.GetParameters())}";
        foreach (var method in type.GetMethods(flags).Where(method => !method.IsSpecialName))
            yield return $"M|{owner}|{method.Name}|{TypeName(method.ReturnType)}|{Parameters(method.GetParameters())}|{method.GetGenericArguments().Length}";
        foreach (var property in type.GetProperties(flags))
            yield return $"P|{owner}|{property.Name}|{TypeName(property.PropertyType)}|{Parameters(property.GetIndexParameters())}|{property.GetMethod is not null}|{property.SetMethod is not null}";
        foreach (var field in type.GetFields(flags))
            yield return $"F|{owner}|{field.Name}|{TypeName(field.FieldType)}|{field.IsLiteral}";
        foreach (var @event in type.GetEvents(flags))
            yield return $"E|{owner}|{@event.Name}|{TypeName(@event.EventHandlerType)}";
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

static void ValidateFrameOrdering(
    PlatformDispatcher dispatcher,
    FixtureHost host,
    SchedulerBinding scheduler,
    List<string> trace,
    List<string> failures)
{
    var phases = new List<string>();
    var transient = 0;
    var persistent = 0;
    var post = 0;
    scheduler.scheduleFrameCallback(_ => { transient++; phases.Add("transient"); });
    scheduler.addPersistentFrameCallback(_ => { persistent++; phases.Add("persistent"); });
    scheduler.addPostFrameCallback(_ => { post++; phases.Add("post"); });
    scheduler.scheduleFrame();
    scheduler.scheduleFrame();
    if (host.PendingFrames != 1)
    {
        failures.Add($"frame: duplicate schedule was not coalesced ({host.PendingFrames}).");
    }
    host.PumpFrame(dispatcher, TimeSpan.FromMilliseconds(16));
    trace.AddRange(phases.Select(phase => $"frame:{phase}"));
    if (!phases.SequenceEqual(["transient", "persistent", "post"]) ||
        scheduler.schedulerPhase != SchedulerPhase.idle || transient != 1 || persistent != 1 || post != 1 || host.FrameCallbacksInvoked != 1)
    {
        failures.Add($"frame: phase/callback ordering mismatch ({string.Join(",", phases)}; {transient}/{persistent}/{post}; ticks {host.FrameCallbacksInvoked}).");
    }

    var tickerTicks = new List<TimeSpan>();
    var ticker = new Ticker(duration => tickerTicks.Add((TimeSpan)duration));
    var future = ticker.start();
    host.PumpFrame(dispatcher, TimeSpan.FromMilliseconds(32));
    host.PumpFrame(dispatcher, TimeSpan.FromMilliseconds(48));
    ticker.stop();
    if (host.PendingFrames > 0)
    {
        host.PumpFrame(dispatcher, TimeSpan.FromMilliseconds(56));
    }
    ticker.dispose();
    if (!future.asTask().IsCompletedSuccessfully || tickerTicks.Count != 2 || tickerTicks[0] != TimeSpan.Zero || tickerTicks[1] != TimeSpan.FromMilliseconds(16))
    {
        failures.Add("frame: ticker elapsed/completion semantics diverged.");
    }

    var completed = TickerFuture.CreateComplete();
    if (!completed.asTask().IsCompletedSuccessfully || !completed.orCancel.asTask().IsCompletedSuccessfully)
    {
        failures.Add("frame: TickerFuture.complete did not complete both futures synchronously.");
    }

    var mutedTicks = new List<TimeSpan>();
    var mutedTicker = new Ticker(duration => mutedTicks.Add((TimeSpan)duration));
    mutedTicker.start();
    mutedTicker.muted = true;
    if (host.PendingFrames > 0) host.PumpFrame(dispatcher, TimeSpan.FromMilliseconds(64));
    if (mutedTicks.Count != 0 || mutedTicker.isTicking)
    {
        failures.Add("frame: muted ticker delivered a callback or remained ticking.");
    }
    mutedTicker.muted = false;
    host.PumpFrame(dispatcher, TimeSpan.FromMilliseconds(80));
    mutedTicker.stop();
    mutedTicker.dispose();
    if (mutedTicks.Count != 1 || mutedTicks[0] != TimeSpan.Zero)
    {
        failures.Add("frame: unmuted ticker did not resume with the correct elapsed origin.");
    }

    var canceledTicker = new Ticker(_ => { });
    var canceled = canceledTicker.start();
    var orCancel = canceled.orCancel.asTask();
    canceledTicker.stop(canceled: true);
    canceledTicker.dispose();
    if (!orCancel.IsFaulted || orCancel.Exception?.GetBaseException() is not TickerCanceled)
    {
        failures.Add("frame: canceled ticker did not fault orCancel with TickerCanceled.");
    }
    if (host.PendingFrames > 0) host.PumpFrame(dispatcher, TimeSpan.FromMilliseconds(96));
    trace.Add("frame:ticker-complete-cancel-mute-resume");
}

static void ValidateMetricsAndLifecycle(
    PlatformDispatcher dispatcher,
    FixtureHost host,
    FlutterView view,
    SchedulerBinding scheduler,
    List<string> trace,
    List<string> failures)
{
    var generations = new List<long>();
    dispatcher.onMetricsChanged = changedView =>
    {
        if (ReferenceEquals(changedView, view))
        {
            generations.Add(changedView.metrics.generation);
            trace.Add($"metrics:{changedView.metrics.generation}:{changedView.metrics.devicePixelRatio}");
        }
    };
    host.EmitMetrics(new(new(1200, 800), 2, ViewPadding.zero, ViewPadding.zero, ViewPadding.zero, AppLifecycleState.resumed, 1, 1));
    host.EmitMetrics(new(new(1800, 1200), 3, ViewPadding.zero, ViewPadding.zero, ViewPadding.zero, AppLifecycleState.resumed, 2, 2));
    host.EmitLifecycle(AppLifecycleState.hidden);
    scheduler.handleAppLifecycleStateChanged(AppLifecycleState.hidden);
    scheduler.scheduleFrameCallback(_ => { });
    var hiddenFrames = host.PendingFrames;
    host.EmitLifecycle(AppLifecycleState.resumed);
    scheduler.handleAppLifecycleStateChanged(AppLifecycleState.resumed);
    if (!generations.SequenceEqual([1L, 2L]) || hiddenFrames != 0 || host.PendingFrames != 1)
    {
        failures.Add($"metrics: generation/lifecycle ordering mismatch ({string.Join(",", generations)}; {hiddenFrames}/{host.PendingFrames}).");
    }
    host.PumpFrame(dispatcher, TimeSpan.FromMilliseconds(64));
}

static void ValidateInput(
    PlatformDispatcher dispatcher,
    FixtureHost host,
    FlutterView view,
    ServicesBinding services,
    List<string> trace,
    List<string> failures)
{
    PointerDataPacket? pointer = null;
    dispatcher.onPointerDataPacket = (changedView, packet) =>
    {
        if (ReferenceEquals(changedView, view))
        {
            pointer = packet;
            trace.Add("input:pointer");
        }
    };
    host.EmitPointer(new([new(43, TimeSpan.FromMilliseconds(3), PointerChange.down, PointerDeviceKind.mouse, 7, 10.5, 20.25, 0, 0, 1)]));
    host.EmitKey(new(43, TimeSpan.FromMilliseconds(4), KeyEventType.down, 0x70004, 0x61, true, "a"));
    var physicalKey = PhysicalKeyboardKey.findKeyByCode(0x70004);
    if (pointer?.data.Single().physicalX != 10.5 || physicalKey is null || !services.keyboard.physicalKeysPressed.Contains(physicalKey))
    {
        failures.Add($"input: pointer precision or HardwareKeyboard state was lost ({pointer?.data.Single().physicalX}; expected={physicalKey?.usbHidUsage}; pressed={string.Join(',', services.keyboard.physicalKeysPressed.Select(key => key.usbHidUsage))}).");
    }
    host.EmitFocus(new(43, false, TimeSpan.FromMilliseconds(5)));
    if (services.keyboard.physicalKeysPressed.Count != 0)
    {
        failures.Add("input: focus loss did not clear pressed keys.");
    }

    var client = new FixtureTextClient();
    var configuration = new TextInputConfiguration(
        viewId: (long)view.viewId,
        inputType: TextInputType.text,
        autofillConfiguration: AutofillConfiguration.disabled,
        allowedMimeTypes: [],
        hintLocales: []);
    var connection = TextInput.attach(client, configuration);
    connection.setEditingState(new TextEditingValue("a", new TextSelection(1, 1), TextRange.empty));
    connection.setCaretRect(new(2, 3, 4, 8));
    host.EmitEditingState(new(
        "한",
        new FlutterTextSelection(1, 1),
        new FlutterTextSelection(0, 1)));
    host.EmitTextAction(FlutterTextInputAction.done);
    if (host.LastTextState.text != "a" || host.LastTextState.selection.baseOffset != 1 ||
        host.CaretRect != new Rect(2, 3, 4, 8) ||
        client.LastValue.text != "한" || client.LastValue.selection.baseOffset != 1 ||
        client.LastValue.composing.start != 0 || client.LastValue.composing.end != 1 ||
        client.LastAction != TextInputAction.done)
    {
        failures.Add("input: text/IME selection, composition, caret, or action trace diverged.");
    }
    connection.close();
    trace.Add("input:key-focus-text-ime");
}

static async Task ValidateChannelsAndServicesAsync(
    FixtureHost host,
    FlutterView view,
    ServicesBinding services,
    List<string> trace,
    List<string> failures)
{
    var standardCodec = new StandardMethodCodec(new StandardMessageCodec());
    host.SetMessageHandler("fixture/method", (message, _) =>
    {
        var call = standardCodec.decodeMethodCall((ByteData)message!.Value);
        return ValueTask.FromResult<ReadOnlyMemory<byte>?>(standardCodec.encodeSuccessEnvelope($"{call.method}:{call.arguments}").asMemory());
    });
    var channel = new MethodChannel("fixture/method", standardCodec, services.defaultBinaryMessenger);
    var result = await channel.invokeMethod<string>("echo", "value");
    var jsonCodec = new JSONMethodCodec();
    var jsonCall = jsonCodec.decodeMethodCall(jsonCodec.encodeMethodCall(new("json", 7L)));
    if (result != "echo:value" || jsonCall.method != "json" || jsonCall.arguments is not long jsonArgument || jsonArgument != 7)
    {
        failures.Add("service: method codec/channel round trip failed.");
    }

    host.SetMessageHandler("flutter/platform", async (message, cancellationToken) =>
    {
        var call = jsonCodec.decodeMethodCall((ByteData)message!.Value);
        if (call.method == "Clipboard.setData")
        {
            _ = DartPatternRuntime.TryGetMapValue(call.arguments, "text", out var textValue);
            await host.SetClipboardTextAsync((string)textValue!, cancellationToken);
            return jsonCodec.encodeSuccessEnvelope(null).asMemory();
        }
        if (call.method == "Clipboard.getData")
        {
            var text = await host.GetClipboardTextAsync(cancellationToken);
            return jsonCodec.encodeSuccessEnvelope(new DartMap<string, object?> { ["text"] = text }).asMemory();
        }
        return null;
    });
    host.SetMessageHandler("flutter/mousecursor", (message, cancellationToken) =>
    {
        _ = cancellationToken;
        var call = standardCodec.decodeMethodCall((ByteData)message!.Value);
        _ = DartPatternRuntime.TryGetMapValue(call.arguments, "kind", out var kindValue);
        host.SetCursor(Enum.Parse<FlutterMouseCursorKind>((string)kindValue!));
        return ValueTask.FromResult<ReadOnlyMemory<byte>?>(standardCodec.encodeSuccessEnvelope(null).asMemory());
    });
    await Clipboard.setData(new("doroti"));
    var clipboard = await Clipboard.getData(Clipboard.kTextPlain);
    await SystemMouseCursors.text.createSession(1).activate();
    if (clipboard?.text != "doroti" || host.Cursor != FlutterMouseCursorKind.text)
    {
        failures.Add("service: clipboard/cursor capability round trip failed.");
    }

    try
    {
        var missing = new MethodChannel("fixture/missing", standardCodec, services.defaultBinaryMessenger);
        await missing.invokeMethod<object?>("missing");
        failures.Add("service: an unimplemented method channel silently succeeded.");
    }
    catch (MissingPluginException)
    {
    }
    trace.Add("service:codec-channel-clipboard-cursor");
}

static void ValidateAssetsAndRestoration(List<string> failures)
{
    var caching = new FixtureAssetBundle(new Dictionary<string, ReadOnlyMemory<byte>>
    {
        ["asset.txt"] = Encoding.UTF8.GetBytes("asset"),
    });
    if (caching.loadString("asset.txt").asTask().GetAwaiter().GetResult() != "asset")
    {
        failures.Add("service: asset bundle load/cache behavior failed.");
    }
    var bucket = new RestorationBucket("root", debugOwner: "fixture");
    bucket.write("count", 3L);
    if (bucket.read<long>("count") != 3L || RestorationLibrary.debugIsSerializableForRestoration(new object()))
    {
        failures.Add("service: restoration serialization/state behavior failed.");
    }
}

static void ValidateFailClosed(PlatformDispatcher dispatcher, List<string> failures)
{
    var isolatedHost = new FixtureHost();
    var capabilities = new FlutterViewCapabilities()
        .Register<IViewHostCapability>(FlutterCapabilityIds.ViewLifecycleMetrics, isolatedHost);
    using var isolatedView = dispatcher.RegisterView(99, capabilities);
    try
    {
        _ = isolatedView.RequireCapability<IPlatformServicesHostCapability>(
            FlutterCapabilityIds.PlatformServices,
            DartUiInvocation.Managed("package:flutter/src/services/clipboard.dart#Clipboard.getData"));
        failures.Add("fail-closed: missing platform.services silently succeeded.");
    }
    catch (FlutterCapabilityException exception) when (exception.CapabilityId == FlutterCapabilityIds.PlatformServices)
    {
    }
    try
    {
        _ = isolatedView.RequireCapability<ITextInputHostCapability>(
            FlutterCapabilityIds.TextInput,
            DartUiInvocation.Managed("package:flutter/src/services/text_input.dart#TextInput.attach"));
        failures.Add("fail-closed: missing text.input silently succeeded.");
    }
    catch (FlutterCapabilityException exception) when (exception.CapabilityId == FlutterCapabilityIds.TextInput)
    {
    }
}

static string FindDorotiRoot(string start)
{
    for (var directory = new DirectoryInfo(Path.GetFullPath(start)); directory is not null; directory = directory.Parent)
    {
        if (File.Exists(Path.Combine(directory.FullName, "Doroti.slnx")))
        {
            return directory.FullName;
        }
        var nested = Path.Combine(directory.FullName, "Doroti");
        if (File.Exists(Path.Combine(nested, "Doroti.slnx")))
        {
            return nested;
        }
    }
    throw new DirectoryNotFoundException("Doroti.slnx was not found.");
}

internal sealed class FixtureHost :
    IViewHostCapability,
    IFrameHostCapability,
    IInputHostCapability,
    IPlatformMessageHostCapability,
    IPlatformServicesHostCapability,
    ITextInputHostCapability,
    IPlatformEnvironmentHostCapability
{
    private readonly Queue<Action<TimeSpan>> _frames = [];
    private readonly Dictionary<string, PlatformMessageHandler> _handlers = new(StringComparer.Ordinal);

    public ViewMetrics Metrics { get; private set; } = new(new(800, 600), 1, ViewPadding.zero, ViewPadding.zero, ViewPadding.zero, AppLifecycleState.resumed, 0, 0);

    public int PendingFrames => _frames.Count;

    public int FrameCallbacksInvoked { get; private set; }

    public string? ClipboardText { get; private set; }

    public FlutterMouseCursorKind Cursor { get; private set; }

    public FlutterTextEditingState LastTextState { get; private set; }

    public Rect CaretRect { get; private set; }

    public PlatformConfiguration Configuration { get; } = new([], Brightness.light, true, false, HostOperatingSystem.windows);

    public event Action<ViewMetrics>? MetricsChanged;

    public event Action<AppLifecycleState>? LifecycleChanged;

    public event Action? CloseRequested;

    public event Action? Closed;

    public event Action<PointerDataPacket>? PointerData;

    public event Action<KeyData>? KeyData;

    public event Action<RawFocusData>? FocusData;

    public event Action<FlutterTextEditingState>? EditingStateChanged;

    public event Action<FlutterTextInputAction>? ActionPerformed;

    public event Action<PlatformConfiguration>? ConfigurationChanged
    {
        add { }
        remove { }
    }

    public void ScheduleFrame(Action<TimeSpan> callback) => _frames.Enqueue(callback);

    public void PumpFrame(PlatformDispatcher dispatcher, TimeSpan timestamp)
    {
        _ = dispatcher;
        if (!_frames.TryDequeue(out var callback))
        {
            throw new InvalidOperationException("No frame was pending.");
        }
        FrameCallbacksInvoked++;
        callback(timestamp);
    }

    public ValueTask<ReadOnlyMemory<byte>?> SendAsync(string channel, ReadOnlyMemory<byte>? data, CancellationToken cancellationToken = default) =>
        _handlers.TryGetValue(channel, out var handler)
            ? handler(data, cancellationToken)
            : ValueTask.FromResult<ReadOnlyMemory<byte>?>(null);

    public void SetMessageHandler(string channel, PlatformMessageHandler? handler)
    {
        if (handler is null)
        {
            _handlers.Remove(channel);
        }
        else
        {
            _handlers[channel] = handler;
        }
    }

    public ValueTask<string?> GetClipboardTextAsync(CancellationToken cancellationToken = default) => ValueTask.FromResult(ClipboardText);

    public ValueTask SetClipboardTextAsync(string text, CancellationToken cancellationToken = default)
    {
        ClipboardText = text;
        return ValueTask.CompletedTask;
    }

    public void SetCursor(FlutterMouseCursorKind cursor) => Cursor = cursor;

    public void SetClient(FlutterTextEditingState initialState) => LastTextState = initialState;

    public void UpdateState(FlutterTextEditingState state) => LastTextState = state;

    public void SetCaretRect(Rect logicalRect) => CaretRect = logicalRect;

    public void ClearClient()
    {
    }

    public void Show()
    {
    }

    public void Resize(Size logicalSize)
    {
    }

    public void Close()
    {
        CloseRequested?.Invoke();
        Closed?.Invoke();
    }

    public void Dispose()
    {
    }

    internal void EmitMetrics(ViewMetrics metrics)
    {
        Metrics = metrics;
        MetricsChanged?.Invoke(metrics);
    }

    internal void EmitLifecycle(AppLifecycleState state) => LifecycleChanged?.Invoke(state);

    internal void EmitPointer(PointerDataPacket packet) => PointerData?.Invoke(packet);

    internal void EmitKey(KeyData data) => KeyData?.Invoke(data);

    internal void EmitFocus(RawFocusData data) => FocusData?.Invoke(data);

    internal void EmitEditingState(FlutterTextEditingState state) => EditingStateChanged?.Invoke(state);

    internal void EmitTextAction(FlutterTextInputAction action) => ActionPerformed?.Invoke(action);
}

internal sealed class FixtureTextClient : TextInputClient
{
    public TextEditingValue LastValue { get; private set; } = TextEditingValue.empty;

    public TextInputAction LastAction { get; private set; }

    public TextEditingValue? currentTextEditingValue => LastValue;

    public AutofillScope? currentAutofillScope => null;

    public void updateEditingValue(TextEditingValue value) => LastValue = value;

    public void performAction(TextInputAction action) => LastAction = action;

    public void performPrivateCommand(string action, Doroti.Flutter.Runtime.DartMap<string, object> data) { }

    public void updateFloatingCursor(RawFloatingCursorPoint point) { }

    public void showAutocorrectionPromptRect(long start, long end) { }

    public void connectionClosed() { }
}

internal sealed class FixtureServicesBinding : ServicesBinding
{
    public FixtureServicesBinding(PlatformDispatcher dispatcher)
        : base(dispatcher)
    {
    }

    public override void handleViewFocusChanged(ViewFocusEvent @event)
    {
        base.handleViewFocusChanged(@event);
        if (!@event.hasFocus)
        {
            keyboard.clearState();
        }
    }
}

internal sealed class FixtureAssetBundle : CachingAssetBundle
{
    private readonly IReadOnlyDictionary<string, ReadOnlyMemory<byte>> _assets;

    public FixtureAssetBundle(IReadOnlyDictionary<string, ReadOnlyMemory<byte>> assets) => _assets = assets;

    public override Future<ByteData> load(string key) => _assets.TryGetValue(key, out var data)
        ? Future<ByteData>.value((ByteData)data)
        : Future<ByteData>.error(new FileNotFoundException($"Asset not found: {key}", key));
}

internal sealed record PromotionResult(
    int Dispositions,
    int Unowned,
    int PublicDeclarationOccurrences,
    int UniquePublicDeclarationNames,
    int PublicMembers,
    int ApiManifestDiff,
    int GeneratedProductSources,
    IReadOnlyList<string> MissingApi,
    IReadOnlyList<string> ExtraApi);
