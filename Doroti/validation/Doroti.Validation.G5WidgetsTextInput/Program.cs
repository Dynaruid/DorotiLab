using System.Text;
using System.Text.Json;
using Doroti.Flutter.Ui;
using Doroti.Generated.Framework.Services;
using Path = System.IO.Path;

var failures = new List<string>();
using var dispatcher = new PlatformDispatcher();
using var scope = dispatcher.EnterScope();
var host = new TextInputFixtureHost();
using var view = dispatcher.RegisterView(53, new FlutterViewCapabilities()
    .Register<IViewHostCapability>(FlutterCapabilityIds.ViewLifecycleMetrics, host)
    .Register<IPlatformMessageHostCapability>(FlutterCapabilityIds.PlatformMessaging, host)
    .Register<ITextInputHostCapability>(FlutterCapabilityIds.TextInput, host));
using var services = new TextInputFixtureServicesBinding(dispatcher);

var client = new TextInputFixtureClient();
var configuration = new TextInputConfiguration(
    viewId: checked((long)view.viewId),
    inputType: TextInputType.text,
    autofillConfiguration: AutofillConfiguration.disabled,
    allowedMimeTypes: [],
    hintLocales: []);
var connection = TextInput.attach(client, configuration);
connection.setEditingState(new TextEditingValue(
    "initial",
    new TextSelection(7, 7),
    TextRange.empty));
connection.setCaretRect(new Rect(11, 13, 2, 17));

host.EmitEditingState(new FlutterTextEditingState(
    "한글",
    new FlutterTextSelection(2, 2),
    new FlutterTextSelection(0, 2)));
host.EmitAction(FlutterTextInputAction.done);

Require(host.ClientSetCount == 1, "host client was not attached exactly once", failures);
Require(host.LastState.text == "initial" && host.LastState.selection.baseOffset == 7,
    "framework editing state did not reach the host capability", failures);
Require(host.CaretRect == new Rect(11, 13, 2, 17),
    "framework caret geometry did not reach the host capability", failures);
Require(client.Value.text == "한글" && client.Value.selection.baseOffset == 2 &&
        client.Value.composing.start == 0 && client.Value.composing.end == 2,
    "host IME selection/composition did not reach Flutter TextInputClient", failures);
Require(client.Action == TextInputAction.done,
    "host IME action did not reach Flutter TextInputClient", failures);

connection.close();
Require(host.ClientClearCount == 1, "host client was not cleared exactly once", failures);

var dorotiRoot = FindDorotiRoot(Environment.CurrentDirectory);
var evidencePath = Path.Combine(dorotiRoot, "migration", "flutter-avalonia", "bridge-validation", "g5-3-text-input.json");
var evidence = new
{
    schemaVersion = "doroti.g5-3-text-input/v1",
    milestone = "G5-3",
    capturedAtUtc = DateTimeOffset.UtcNow,
    success = failures.Count == 0,
    boundary = "Flutter Services -> ITextInputHostCapability -> native host",
    verified = new[] { "attach", "editing-state", "selection", "composition", "caret", "action", "detach" },
    physicalIme = "notVerified",
    failures,
};
Directory.CreateDirectory(Path.GetDirectoryName(evidencePath)!);
var temporaryPath = evidencePath + ".tmp-" + Guid.NewGuid().ToString("N");
File.WriteAllText(temporaryPath, JsonSerializer.Serialize(evidence, new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true,
}) + "\n", new UTF8Encoding(false));
File.Move(temporaryPath, evidencePath, true);

Console.WriteLine($"G5-3 Widgets text-input capability validation: {(failures.Count == 0 ? "PASS" : "FAIL")}");
Console.WriteLine($"Evidence: {evidencePath}");
foreach (var failure in failures) Console.Error.WriteLine(failure);
return failures.Count == 0 ? 0 : 1;

static void Require(bool condition, string message, List<string> failures)
{
    if (!condition) failures.Add(message);
}

static string FindDorotiRoot(string start)
{
    for (var directory = new DirectoryInfo(Path.GetFullPath(start)); directory is not null; directory = directory.Parent)
    {
        if (File.Exists(Path.Combine(directory.FullName, "Doroti.slnx"))) return directory.FullName;
        var nested = Path.Combine(directory.FullName, "Doroti");
        if (File.Exists(Path.Combine(nested, "Doroti.slnx"))) return nested;
    }
    throw new DirectoryNotFoundException("Doroti.slnx was not found.");
}

internal sealed class TextInputFixtureClient : TextInputClient
{
    public TextEditingValue Value { get; private set; } = TextEditingValue.empty;
    public TextInputAction Action { get; private set; }
    public TextEditingValue? currentTextEditingValue => Value;
    public AutofillScope? currentAutofillScope => null;
    public void updateEditingValue(TextEditingValue value) => Value = value;
    public void performAction(TextInputAction action) => Action = action;
    public void performPrivateCommand(string action, Doroti.Flutter.Runtime.DartMap<string, object> data) { }
    public void updateFloatingCursor(RawFloatingCursorPoint point) { }
    public void showAutocorrectionPromptRect(long start, long end) { }
    public void connectionClosed() { }
}

internal sealed class TextInputFixtureServicesBinding : ServicesBinding
{
    public TextInputFixtureServicesBinding(PlatformDispatcher dispatcher)
        : base(dispatcher)
    {
    }
}

internal sealed class TextInputFixtureHost : IViewHostCapability, ITextInputHostCapability, IPlatformMessageHostCapability
{
    public ViewMetrics Metrics { get; } = new(new Size(800, 600), 1, ViewPadding.zero, ViewPadding.zero, ViewPadding.zero, AppLifecycleState.resumed, 0, 0);
    public FlutterTextEditingState LastState { get; private set; }
    public Rect CaretRect { get; private set; }
    public int ClientSetCount { get; private set; }
    public int ClientClearCount { get; private set; }
    public event Action<ViewMetrics>? MetricsChanged { add { } remove { } }
    public event Action<AppLifecycleState>? LifecycleChanged { add { } remove { } }
    public event Action? CloseRequested { add { } remove { } }
    public event Action? Closed { add { } remove { } }
    public event Action<FlutterTextEditingState>? EditingStateChanged;
    public event Action<FlutterTextInputAction>? ActionPerformed;
    public void SetClient(FlutterTextEditingState initialState) { ClientSetCount++; LastState = initialState; }
    public void UpdateState(FlutterTextEditingState state) => LastState = state;
    public void SetCaretRect(Rect logicalRect) => CaretRect = logicalRect;
    public void ClearClient() => ClientClearCount++;
    public void EmitEditingState(FlutterTextEditingState state) => EditingStateChanged?.Invoke(state);
    public void EmitAction(FlutterTextInputAction action) => ActionPerformed?.Invoke(action);
    public ValueTask<ReadOnlyMemory<byte>?> SendAsync(string channel, ReadOnlyMemory<byte>? data, CancellationToken cancellationToken = default) =>
        ValueTask.FromResult<ReadOnlyMemory<byte>?>(null);
    public void SetMessageHandler(string channel, PlatformMessageHandler? handler) { }
    public void Show() { }
    public void Resize(Size logicalSize) { }
    public void Close() { }
    public void Dispose() { }
}
