using Doroti.Framework.Scheduler;
using Doroti.Framework.Services;
using Doroti.Framework.Widgets;
using Doroti.Runtime;

var assertions = 0;
void Check(bool condition, string message)
{
    if (!condition)
        throw new InvalidOperationException(message);
    assertions++;
}
async Task ThrowsAsync<TException>(Func<Task> action)
    where TException : Exception
{
    try
    {
        await action();
    }
    catch (TException)
    {
        assertions++;
        return;
    }
    throw new InvalidOperationException($"Expected {typeof(TException).Name}");
}

string? reference = "present";
string required = DartRuntimePrimitives.RequireReference(reference);
Check(
    required == "present" && reference.Length == 7,
    "Reference assertion preserves its value and flow state."
);
int? zero = 0;
Check(
    (zero ?? throw new NullReferenceException("Dart null assertion failed.")) == 0
        && zero.Value == 0,
    "Value assertion must accept zero."
);
bool? falseValue = false;
Check(
    (falseValue ?? throw new NullReferenceException("Dart null assertion failed.")) == false,
    "Value assertion must accept false."
);
foreach (
    var action in new Action[]
    {
        () => _ = (int?)null ?? throw new NullReferenceException("Dart null assertion failed."),
        () => _ = (string?)null ?? throw new NullReferenceException("Dart null assertion failed."),
        () => DartRuntimePrimitives.RequireReference<string?>(null),
    }
)
{
    try
    {
        action();
        throw new InvalidOperationException("Null assertion did not throw.");
    }
    catch (NullReferenceException error)
    {
        Check(
            error.Message == "Dart null assertion failed.",
            "Preserve null assertion exception and message."
        );
    }
}
var evaluationCount = 0;
int? NextRequiredValue()
{
    evaluationCount++;
    return 5;
}
var evaluatedOnce =
    NextRequiredValue() ?? throw new NullReferenceException("Dart null assertion failed.");
Check(evaluatedOnce == 5 && evaluationCount == 1, "Null assertion evaluates its operand once.");
string? delayedValue = null;
var delayedInvocations = 0;
Func<string> delayedAssertion = () =>
{
    delayedInvocations++;
    return delayedValue ?? throw new NullReferenceException("Dart null assertion failed.");
};
Check(delayedInvocations == 0, "A callback null assertion remains delayed.");
try
{
    _ = delayedAssertion();
    throw new InvalidOperationException("Delayed null assertion did not throw.");
}
catch (NullReferenceException error)
{
    Check(
        delayedInvocations == 1 && error.Message == "Dart null assertion failed.",
        "A callback null assertion runs at invocation time."
    );
}
Check(
    DartRuntimePrimitives.ConvertValue<string?>(null) is null,
    "Conversion preserves reference null."
);
Check(
    DartRuntimePrimitives.ConvertValue<int?>(null) is null,
    "Conversion preserves nullable value null."
);
Check(
    DartRuntimePrimitives.ConvertValue<int>(null) == 0,
    "Existing null-to-value conversion remains unchanged."
);

var incomplete = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
var future = Future<int>.fromTask(incomplete.Task);
await ThrowsAsync<TimeoutException>(() =>
    future.timeout(Duration.zero, (Func<object>?)null).asTask()
);
await ThrowsAsync<TimeoutException>(() => future.timeout(Duration.zero).asTask());
Check(await future.timeout(Duration.zero, () => 42) == 42, "Timeout returns callback value.");
Func<object> futureRecovery = () => Future<int>.fromTask(Task.FromResult(43));
Check(
    await future.timeout(Duration.zero, futureRecovery) == 43,
    "Timeout awaits a FutureOr recovery."
);
Check(
    await future.timeout(Duration.zero, () => Future<int>.fromTask(Task.FromResult(44))) == 44,
    "The Future callback overload preserves the typed recovery value."
);
Check(
    await Future<int>.fromTask(Task.FromResult(7)).timeout(Duration.zero, (Func<object>?)null) == 7,
    "A completed source must not invoke a missing callback."
);
Func<object> failingRecovery = () => throw new InvalidOperationException("timeout recovery");
await ThrowsAsync<InvalidOperationException>(() =>
    future.timeout(Duration.zero, failingRecovery).asTask()
);
var originalTimeout = new TimeoutException("source failure");
var recoveryInvoked = false;
try
{
    await Future<int>
        .fromTask(Task.FromException<int>(originalTimeout))
        .timeout(
            Duration.zero,
            () =>
            {
                recoveryInvoked = true;
                return 9;
            }
        );
    throw new InvalidOperationException("The source timeout must propagate.");
}
catch (TimeoutException error)
{
    Check(
        ReferenceEquals(error, originalTimeout) && !recoveryInvoked,
        "A source TimeoutException is not a deadline expiry."
    );
}
using var cancellation = new CancellationTokenSource();
cancellation.Cancel();
await ThrowsAsync<TaskCanceledException>(() =>
    Future<int>
        .fromTask(Task.FromCanceled<int>(cancellation.Token))
        .timeout(Duration.zero, (Func<object>?)null)
        .asTask()
);
await ThrowsAsync<TimeoutException>(() => new TickerFuture().timeout(Duration.zero).asTask());
await TickerFuture.CreateComplete().timeout(Duration.zero).asTask();
assertions++;
var pendingTicker = new TickerFuture();
Future baseTicker = pendingTicker;
Check(
    !pendingTicker.GetAwaiter().IsCompleted && !baseTicker.asTask().IsCompleted,
    "Ticker awaits must stay pending until the ticker completes."
);
typeof(TickerFuture)
    .GetMethod(
        "_complete",
        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic
    )!
    .Invoke(pendingTicker, null);
await pendingTicker;
Check(
    baseTicker.asTask().IsCompletedSuccessfully,
    "Ticker completion reaches base Future observers."
);
var canceledTicker = new TickerFuture();
var canceledFuture = canceledTicker.orCancel;
typeof(TickerFuture)
    .GetMethod(
        "_cancel",
        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic
    )!
    .Invoke(canceledTicker, [new Ticker(_ => { })]);
await ThrowsAsync<TickerCanceled>(() => canceledFuture.asTask());
Check(!canceledTicker.asTask().IsCompleted, "Canceled tickers keep the primary future pending.");
incomplete.SetResult(99);
Check(await future == 99, "Timeout must not cancel or replace the source future.");

var messenger = new ContractMessenger();
var messages = new BasicMessageChannel<string>("contract", new StringCodec(), messenger);
Check(await messages.send(null) is null, "A missing response Future means a null message.");
messages.setMessageHandler(value => Future<string?>.fromTask(Task.FromResult(value)));
var handling =
    messenger.Handler?.Invoke(null)
    ?? throw new InvalidOperationException("Handler was not installed.");
Check(
    await handling is null,
    "Null incoming messages and handler results round-trip through the codec."
);
messages.setMessageHandler(null);
Check(messenger.Handler is null, "Clearing a channel removes its handler.");
foreach (
    MethodCodec codec in new MethodCodec[]
    {
        new JSONMethodCodec(),
        new StandardMethodCodec(new StandardMessageCodec()),
    }
)
{
    var call = codec.decodeMethodCall(codec.encodeMethodCall(new MethodCall("nullable")));
    Check(call.method == "nullable" && call.arguments is null, "Method arguments may be absent.");
    Check(
        codec.decodeEnvelope(codec.encodeSuccessEnvelope(null)) is null,
        "A success envelope may contain null."
    );
    messenger.Reply = Future<ByteData?>.fromTask(
        Task.FromResult<ByteData?>(codec.encodeSuccessEnvelope(null))
    );
    Check(
        await new MethodChannel("contract", codec, messenger).invokeMethod<object>("nullable")
            is null,
        "Method channel preserves a null result."
    );
}

var keyA = LogicalKeyboardKey.keyA;
var keyB = LogicalKeyboardKey.keyB;
var firstKeys = new KeySet<LogicalKeyboardKey>(keyA, keyB);
var sameKeys = new KeySet<LogicalKeyboardKey>(keyB, keyA);
Check(firstKeys.keys.Count == 2, "KeySet retains all optional keys.");
Check(
    firstKeys.Equals(sameKeys) && firstKeys.GetHashCode() == sameKeys.GetHashCode(),
    "KeySet hash is order-independent."
);
var keyLookup = new Dictionary<KeySet<LogicalKeyboardKey>, string> { [firstKeys] = "found" };
Check(
    keyLookup[sameKeys] == "found" && keyLookup.Remove(sameKeys),
    "Equivalent key sets support dictionary lookup and removal."
);
var pressed = WidgetState.pressed.asConstraint();
var focused = WidgetState.focused.asConstraint();
foreach (
    var pair in new[]
    {
        (pressed.op_BitwiseAnd(focused), pressed.op_BitwiseAnd(focused)),
        (pressed.op_BitwiseOr(focused), pressed.op_BitwiseOr(focused)),
    }
)
{
    Check(
        pair.Item1.Equals(pair.Item2) && pair.Item1.GetHashCode() == pair.Item2.GetHashCode(),
        "Equivalent widget-state combinations share their hash."
    );
    Check(
        new HashSet<WidgetStatesConstraint> { pair.Item1 }.Remove(pair.Item2),
        "Equivalent widget-state combinations can be removed from a set."
    );
}
Element firstElement = new SizedBox().createElement();
Element otherElement = new SizedBox().createElement();
Check(
    firstElement.Equals(firstElement) && !firstElement.Equals(otherElement),
    "Element equality remains identity-based."
);
Check(
    new HashSet<Element> { firstElement }.Contains(firstElement),
    "An Element can be found by its identity hash."
);

Check(
    new Doroti.Framework.Foundation.ObjectFlagProperty<object>(
        "pending",
        null,
        ifPresent: null,
        ifNull: "unresolved"
    ).toDescription() == "unresolved",
    "Absent diagnostic values preserve their null description."
);
Check(
    new Doroti.Framework.Foundation.DiagnosticsProperty<bool?>(
        "optional",
        null,
        description: null
    ).toDescription() == "null",
    "A nullable diagnostic value accepts an absent description."
);
Check(
    new Doroti.Framework.Foundation.DiagnosticsProperty<int>(
        "value",
        2,
        description: "two"
    ).toDescription() == "two",
    "Diagnostic descriptions override raw value text."
);
var painter = new Doroti.Framework.Painting.TextPainter(textScaleFactor: 2);
Check(
    painter.textScaler.scale(10) == 20,
    "Unspecified TextScaler uses the requested scale factor."
);
painter.dispose();
Check(
    new Doroti.Framework.Rendering.RenderCustomPaint().preferredSize == Doroti.Ui.Size.zero,
    "RenderCustomPaint retains its documented zero-size default."
);

var pressedStates = new HashSet<WidgetState> { WidgetState.pressed };
var emptyStates = new HashSet<WidgetState>();
var red = new Doroti.Ui.Color(0xffff0000);
var colorMap = new DartMap<WidgetStatesConstraint, Doroti.Ui.Color> { [pressed] = red };
Doroti.Ui.Color mappedColor = WidgetStateColor.CreateFromMap(colorMap);
Check(
    ((WidgetStateColor)mappedColor).resolve(pressedStates) == red,
    "Mapped colors are assignable to Color and resolve lazily."
);
await ThrowsAsync<ArgumentException>(() =>
    Task.FromResult(((WidgetStateColor)mappedColor).resolve(emptyStates))
);
await ThrowsAsync<Doroti.Framework.Foundation.FlutterError>(() =>
    Task.FromResult(mappedColor.value)
);
var genericMapper = new WidgetStateMapper<Doroti.Ui.Color>(colorMap);
Check(
    mappedColor.Equals(genericMapper)
        && genericMapper.Equals(mappedColor)
        && mappedColor.GetHashCode() == genericMapper.GetHashCode(),
    "Typed mapper adapters preserve symmetric map equality and hash codes."
);
Check(
    new Dictionary<Doroti.Ui.Color, int> { [mappedColor] = 1 }[
        WidgetStateColor.CreateFromMap(colorMap)
    ] == 1,
    "Color-typed dictionary comparisons dispatch to mapper equality."
);
var cursorMap = WidgetStateMouseCursor.CreateFromMap(
    new DartMap<WidgetStatesConstraint, MouseCursor> { [pressed] = SystemMouseCursors.click }
);
Check(
    cursorMap.resolve(pressedStates) == SystemMouseCursors.click,
    "Mapped cursors resolve without a generic cast."
);
await ThrowsAsync<ArgumentException>(() => Task.FromResult(cursorMap.resolve(emptyStates)));
var sideMap = WidgetStateBorderSide.CreateFromMap(
    new DartMap<WidgetStatesConstraint, Doroti.Framework.Painting.BorderSide?>
    {
        [pressed] = Doroti.Framework.Painting.BorderSide.none,
    }
);
Check(
    sideMap.resolve(emptyStates) is null
        && sideMap.resolve(pressedStates) == Doroti.Framework.Painting.BorderSide.none,
    "Nullable border-side maps preserve unmatched null."
);
await ThrowsAsync<Doroti.Framework.Foundation.FlutterError>(() => Task.FromResult(sideMap.width));
var outline = new Doroti.Framework.Painting.RoundedRectangleBorder();
var outlineMap = WidgetStateOutlinedBorder.CreateFromMap(
    new DartMap<WidgetStatesConstraint, Doroti.Framework.Painting.OutlinedBorder?>
    {
        [pressed] = outline,
    }
);
Check(
    ReferenceEquals(outlineMap.resolve(pressedStates), outline)
        && outlineMap.resolve(emptyStates) is null,
    "Mapped outlined borders preserve nullable resolution."
);
await ThrowsAsync<Doroti.Framework.Foundation.FlutterError>(() => Task.FromResult(outlineMap.side));
Check(
    ReferenceEquals(
        WidgetStateOutlinedBorder.CreateResolveWith(_ => outline).resolve(emptyStates),
        outline
    ),
    "Callback outlined-border factories return their declared CLR type."
);
var style = new Doroti.Framework.Painting.TextStyle(fontSize: 19);
var styleMap = WidgetStateTextStyle.CreateFromMap(
    new DartMap<WidgetStatesConstraint, Doroti.Framework.Painting.TextStyle> { [pressed] = style }
);
Check(
    ReferenceEquals(styleMap.resolve(pressedStates), style),
    "Mapped text styles resolve lazily."
);
await ThrowsAsync<Doroti.Framework.Foundation.FlutterError>(() =>
    Task.FromResult(styleMap.fontSize)
);
Check(
    new TweenAnimationBuilder<double>(
        tween: new Doroti.Framework.Animation.Tween<double>(begin: 0, end: 1),
        duration: Duration.zero,
        builder: (_, _, _) => new SizedBox()
    ).createState() is IState,
    "Generic tween animation factories return a correctly typed state."
);
Check(
    new WidgetStatePropertyAll<string?>(null).GetHashCode()
        == new WidgetStatePropertyAll<string?>(null).GetHashCode(),
    "Nullable state values have a stable hash."
);
Check(
    new Doroti.Framework.Widgets.ListView().childrenDelegate is SliverChildListDelegate,
    "ListView accepts its documented empty children default."
);

var bindingFlags =
    System.Reflection.BindingFlags.Public
    | System.Reflection.BindingFlags.NonPublic
    | System.Reflection.BindingFlags.Instance
    | System.Reflection.BindingFlags.Static;
var namedInfo = typeof(Navigator).Assembly.GetType(
    "Doroti.Framework.Widgets._NamedRestorationInformation__navigator",
    true
)!;
var restorationCtor = namedInfo.GetConstructors(bindingFlags).Single();
foreach (object? arguments in new object?[] { null, "payload" })
{
    var information = restorationCtor.Invoke(["/contract", arguments, 7L]);
    var serialized =
        (List<object>)
            namedInfo
                .GetMethod("computeSerializableData", bindingFlags)!
                .Invoke(information, null)!;
    Check(
        serialized.Count == (arguments is null ? 3 : 4),
        "Route restoration omits absent arguments and retains supplied ones."
    );
    var restored = namedInfo
        .GetMethod("CreateFromSerializableData", bindingFlags)!
        .Invoke(null, [serialized.Skip(1).ToList()])!;
    Check(
        Equals(namedInfo.GetProperty("arguments", bindingFlags)!.GetValue(restored), arguments),
        "Route arguments round-trip through restoration."
    );
}
var historyType = typeof(Navigator).Assembly.GetType(
    "Doroti.Framework.Widgets._HistoryProperty__navigator",
    true
)!;
var historyProperty = Activator.CreateInstance(historyType, nonPublic: true)!;
var firstHistory = new DartMap<string?, List<object>> { [null] = new List<object> { "root" } };
var secondHistory = new DartMap<string?, List<object>> { [null] = new List<object> { "root" } };
Check(
    (bool)
        historyType
            .GetMethod("_debugMapsEqual", bindingFlags)!
            .Invoke(historyProperty, [firstHistory, secondHistory])!,
    "Root pageless routes use a valid null restoration key."
);

var cupertinoTheme = new Doroti.Framework.Cupertino.CupertinoThemeData();
Check(
    cupertinoTheme.primaryColor is not null
        && cupertinoTheme.textTheme is not null
        && cupertinoTheme.barBackgroundColor is not null,
    "Default Cupertino themes supply required colors and typography."
);
var unconfiguredTheme = new Doroti.Framework.Cupertino.NoDefaultCupertinoThemeData();
Check(
    unconfiguredTheme.primaryColor is null && unconfiguredTheme.textTheme is null,
    "No-default Cupertino themes retain optional values."
);
var inputBorder = new Doroti.Framework.Material.OutlineInputBorder();
var mappedInput = Doroti.Framework.Material.WidgetStateInputBorder.CreateFromMap(
    new DartMap<WidgetStatesConstraint, Doroti.Framework.Material.InputBorder>
    {
        [pressed] = inputBorder,
    }
);
Check(
    ReferenceEquals(mappedInput.resolve(pressedStates), inputBorder),
    "Input-border maps return an assignable, lazy resolver."
);
await ThrowsAsync<ArgumentException>(() => Task.FromResult(mappedInput.resolve(emptyStates)));
await ThrowsAsync<Doroti.Framework.Foundation.FlutterError>(() =>
    Task.FromResult(mappedInput.borderSide)
);
Check(
    ReferenceEquals(
        Doroti
            .Framework.Material.WidgetStateInputBorder.CreateResolveWith(_ => inputBorder)
            .resolve(emptyStates),
        inputBorder
    ),
    "Input-border callback factories preserve their CLR contract."
);
Check(
    Doroti
        .Framework.Material.WidgetStateInputBorder.CreateResolveWith(_ => inputBorder)
        .borderSide.width == inputBorder.borderSide.width,
    "The unresolved callback input border retains OutlineInputBorder defaults."
);
WidgetStateProperty<Doroti.Ui.Color?> covariantProperty =
    new WidgetStatePropertyAll<Doroti.Ui.Color>(red);
Check(
    covariantProperty.resolve(emptyStates) == red,
    "Read-only state properties support covariant consumers."
);
var switchTheme = new Doroti.Framework.Material.SwitchThemeData(thumbColor: covariantProperty);
Check(
    switchTheme.thumbColor?.resolve(emptyStates) == red,
    "Theme nullable state properties accept required-value providers."
);

var inspector = WidgetInspectorService.instance;
var inspectedId =
    inspector.toId(firstElement, "a1-contract")
    ?? throw new InvalidOperationException("Inspector did not assign an ID.");
Check(
    ReferenceEquals(inspector.toObject(inspectedId), firstElement),
    "Inspector IDs retain the inspected reference."
);
Check(
    Equals(new InspectorReferenceData(42L, "number").value, 42L),
    "Inspector value-type references retain their value."
);
var chain = inspector._getParentChain(inspectedId, "a1-contract");
Check(
    chain.Count == 1
        && chain[0] is DartMap<string, object?> chainNode
        && chainNode.ContainsKey("node")
        && chainNode.ContainsKey("children"),
    "Inspector parent chains serialize actual nodes rather than returning an empty list."
);
var chainJson = System.Text.Json.JsonDocument.Parse(
    inspector.getParentChain(inspectedId, "a1-contract")
);
Check(
    chainJson.RootElement.GetArrayLength() == 1,
    "Inspector parent chains are serializable JSON."
);
inspector.disposeGroup("a1-contract");

using var messagingDispatcher = new Doroti.Ui.PlatformDispatcher();
using var messagingView = messagingDispatcher.RegisterView(
    1,
    new Doroti.Ui.DorotiViewCapabilities("a1-contract").Register<Doroti.Ui.IViewHostCapability>(
        Doroti.Ui.DorotiCapabilityIds.ViewLifecycleMetrics,
        new ContractViewHost()
    )
);
using var messagingScope = messagingDispatcher.EnterScope();
var defaultMessengerType = typeof(BinaryMessenger).Assembly.GetType(
    "Doroti.Framework.Services._DefaultBinaryMessenger",
    true
)!;
var defaultMessenger = (BinaryMessenger)
    Activator.CreateInstance(defaultMessengerType, nonPublic: true)!;
foreach (var emptyFuture in new[] { true, false })
{
    var reply = new TaskCompletionSource<ByteData?>(
        TaskCreationOptions.RunContinuationsAsynchronously
    );
    defaultMessenger.setMessageHandler(
        "a1-null-reply",
        _ => emptyFuture ? null : Future<ByteData?>.value(null)
    );
    await defaultMessenger.handlePlatformMessage(
        "a1-null-reply",
        null,
        result => reply.TrySetResult(result)
    );
    Check(
        await reply.Task.WaitAsync(TimeSpan.FromSeconds(3)) is null,
        "The actual binary messenger delivers its response callback for nullable Futures and null values."
    );
}
defaultMessenger.setMessageHandler("a1-null-reply", null);

// Ordinary Action<T> must resolve to the BCL delegate even with Widgets imported.
var intentCalls = 0;
Action<IntentActionProbeIntent> intentCallback = _ => intentCalls++;
Check(
    intentCallback.GetType() == typeof(System.Action<IntentActionProbeIntent>),
    "Action<T> is the BCL callback."
);
Check(
    typeof(IntentAction<>).Assembly.GetType("Doroti.Framework.Widgets.Action`1") is null,
    "The old framework Action<T> name must not keep shadowing BCL callbacks."
);
var probeIntent = new IntentActionProbeIntent();
var callbackCommand = new CallbackAction<IntentActionProbeIntent>(intentCallback);
Check(
    callbackCommand is IntentAction<IntentActionProbeIntent>,
    "Callback commands retain the renamed base contract."
);
Check(
    callbackCommand.invoke(probeIntent) is null && intentCalls == 1,
    "BCL callbacks adapt to command invocation."
);
var probeCommand = new IntentActionProbe();
IIntentAction erasedCommand = probeCommand;
Check(
    erasedCommand.IntentType == typeof(IntentActionProbeIntent),
    "Intent command type identity is preserved."
);
Check(
    !erasedCommand.IsEnabledForIntent(probeIntent, null),
    "Disabled commands retain their enabled-state contract."
);
probeCommand.Enabled = true;
Check(erasedCommand.IsEnabledForIntent(probeIntent, null), "Command enablement can change.");
Check(
    Equals(erasedCommand.InvokeIntent(probeIntent, null), 42),
    "Renamed commands preserve virtual dispatch and return values."
);
Check(
    erasedCommand.ToKeyEventResultForIntent(probeIntent, null)
        == KeyEventResult.skipRemainingHandlers,
    "Commands retain their key-event consumption policy."
);
var commandNotifications = 0;
Action<object> commandListener = sender =>
{
    Check(ReferenceEquals(sender, probeCommand), "Listener receives the command.");
    commandNotifications++;
};
probeCommand.addActionListener(commandListener);
probeCommand.notifyActionListeners();
probeCommand.removeActionListener(commandListener);
probeCommand.notifyActionListeners();
Check(
    commandNotifications == 1,
    "Command change notifications and listener removal are preserved."
);

Console.WriteLine($"A1 contract checks: {assertions} assertions passed.");

sealed class IntentActionProbeIntent : Intent;

sealed class IntentActionProbe : IntentAction<IntentActionProbeIntent>
{
    public bool Enabled { get; set; }

    public override bool isEnabled(IntentActionProbeIntent intent, BuildContext? context = null) =>
        Enabled;

    public override bool consumesKey(IntentActionProbeIntent intent) => false;

    public override object? invoke(IntentActionProbeIntent intent, BuildContext? context = null) =>
        42;
}

sealed class ContractMessenger : BinaryMessenger
{
    public Future<ByteData?>? Reply { get; set; }
    public Func<ByteData?, Future<ByteData?>?>? Handler { get; private set; }

    public Future<ByteData?>? send(string channel, ByteData? message) => Reply;

    public void setMessageHandler(string channel, Func<ByteData?, Future<ByteData?>?>? handler) =>
        Handler = handler;

    public Future handlePlatformMessage(
        string channel,
        ByteData? data,
        System.Action<ByteData?>? callback
    ) => throw new NotSupportedException();
}

sealed class ContractViewHost : Doroti.Ui.IViewHostCapability
{
    public Doroti.Ui.ViewMetrics Metrics { get; } =
        new(
            new Doroti.Ui.Size(100, 100),
            1,
            Doroti.Ui.ViewPadding.zero,
            Doroti.Ui.ViewPadding.zero,
            Doroti.Ui.ViewPadding.zero,
            Doroti.Ui.AppLifecycleState.resumed,
            1,
            1
        );
    public Doroti.Ui.DorotiViewEpoch ViewEpoch { get; } = new(1, 1, 1, 100, 100, 100, 100, 1, 1, 0);
    public event System.Action<Doroti.Ui.ViewMetrics>? MetricsChanged
    {
        add { }
        remove { }
    }
    public event System.Action<Doroti.Ui.AppLifecycleState>? LifecycleChanged
    {
        add { }
        remove { }
    }
    public event Action? CloseRequested
    {
        add { }
        remove { }
    }
    public event Action? Closed
    {
        add { }
        remove { }
    }

    public void Show() { }

    public void Resize(Doroti.Ui.Size logicalSize) => throw new NotSupportedException();

    public void Close() { }

    public void Dispose() { }
}
