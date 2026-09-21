using Doroti.Framework.Foundation;
using Doroti.Framework.Material;
using Doroti.Framework.Widgets;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;
using Path = System.IO.Path;

using var dispatcher = new PlatformDispatcher();
using var scope = dispatcher.EnterScope();
var portrait = args.Contains("--portrait", StringComparer.Ordinal);
var host = new BuildHost(portrait ? new Size(400, 800) : new Size(800, 600));
using var renderer = new SkiaSceneRenderer(
    1,
    host,
    null,
    null,
    "picker-input",
    "skia-raster",
    "picker-input",
    enablePictureRasterCache: false
);
foreach (
    var font in new[]
    {
        "Roboto-medium.ttf",
        "Roboto-bold.ttf",
        "Roboto-regular.ttf",
        "MaterialIcons-Regular.otf",
    }
)
{
    using var stream = typeof(BuildHost).Assembly.GetManifestResourceStream("Fonts." + font)!;
    using var bytes = new MemoryStream();
    stream.CopyTo(bytes);
    renderer
        .RegisterFontAsync(
            bytes.ToArray(),
            font.StartsWith("Roboto", StringComparison.Ordinal) ? "Roboto" : "MaterialIcons"
        )
        .GetAwaiter()
        .GetResult();
}
using var view = dispatcher.RegisterView(
    1,
    new DorotiViewCapabilities("picker-input-build")
        .Register(DorotiCapabilityIds.ViewLifecycleMetrics, host)
        .Register(DorotiCapabilityIds.ViewFrameDispatch, host)
        .Register(DorotiCapabilityIds.PlatformMessaging, host)
        .Register(DorotiCapabilityIds.PlatformServices, host)
        .Register(DorotiCapabilityIds.GraphicsText, renderer)
        .Register(DorotiCapabilityIds.GraphicsScene, renderer)
        .Register(DorotiCapabilityIds.GraphicsFont, renderer)
        .Register(DorotiCapabilityIds.TextInput, host)
        .Register(DorotiCapabilityIds.InputEvents, host)
        .Register(DorotiCapabilityIds.PlatformEnvironment, host)
);
using var environment = view.EnterPlatformEnvironmentScope();
using var binding = new WidgetsFlutterBinding(dispatcher);
var errors = new List<FlutterErrorDetails>();
FlutterError.onError = errors.Add;
foreach (bool? handwriting in new bool?[] { null, false, true })
{
    Mount(
        new Material(
            child: new TextFormField(
                initialValue: "9/21/2026",
                stylusHandwritingEnabled: handwriting
            )
        )
    );
    Check(
        Find<TextField>().Single().stylusHandwritingEnabled
            == (handwriting ?? EditableText.defaultStylusHandwritingEnabled),
        "handwriting default/override"
    );
}

Mount(
    new DatePickerDialog(
        initialDate: new DateTime(2026, 9, 21),
        firstDate: new DateTime(2024, 1, 1),
        lastDate: new DateTime(2027, 1, 1)
    )
);
Toggle("Switch to input");
Check(Find<TextField>().Single().controller!.text == "09/21/2026", "date initial value");
Snapshot("date-input");
Find<TextField>().Single().controller!.text = "invalid";
Check(!ValidateForm(), "invalid date is rejected");
Pump();
Find<TextField>().Single().controller!.text = "09/22/2026";
Check(ValidateForm(), "valid date is accepted");
Toggle("Switch to calendar");
Check(!Find<TextField>().Any(), "date returns to calendar");
Toggle("Switch to input");
Check(
    Find<TextField>().Single().controller!.text == "09/22/2026",
    "edited date survives mode round trip"
);

Mount(new TimePickerDialog(initialTime: new TimeOfDay(hour: 13, minute: 45)));
Toggle("Switch to text input mode");
Check(Find<TextField>().Count() == 2, "time input builds both fields");
Snapshot("time-input");
Check(
    Find<TextField>().Select(f => f.controller!.text).SequenceEqual(new[] { "13", "45" }),
    "24-hour time initial value"
);
Find<TextField>().First().controller!.text = "99";
Check(!ValidateForm(), "invalid hour is rejected");
Pump();
Find<TextField>().First().controller!.text = "18";
Check(ValidateForm(), "valid hour is accepted");
Toggle("Switch to dial picker mode");
Check(!Find<TextField>().Any(), "time returns to dial");
Toggle("Switch to text input mode");
Check(Find<TextField>().Count() == 2, "time input survives mode round trip");
Check(
    Find<TextField>().Select(f => f.controller!.text).SequenceEqual(new[] { "18", "45" }),
    "edited time survives mode round trip"
);
Mount(
    new DatePickerDialog(
        initialDate: new DateTime(2026, 9, 21),
        firstDate: new DateTime(2024, 1, 1),
        lastDate: new DateTime(2027, 1, 1),
        initialEntryMode: DatePickerEntryMode.inputOnly
    )
);
Check(Find<TextField>().Count() == 1, "date inputOnly");
Mount(
    new TimePickerDialog(
        initialTime: new TimeOfDay(hour: 13, minute: 45),
        initialEntryMode: TimePickerEntryMode.inputOnly
    )
);
Check(Find<TextField>().Count() == 2, "time inputOnly");
Console.WriteLine("PASS picker input build/layout and mode transitions");

void Mount(Widget child)
{
    binding.attachRootWidget(
        binding.wrapWithDefaultView(
            new MaterialApp(
                debugShowCheckedModeBanner: false,
                theme: ThemeData.Create(fontFamily: "Roboto"),
                home: child
            )
        )
    );
    Pump();
}
void Pump()
{
    for (var i = 0; i < 3; i++)
    {
        binding.scheduleForcedFrame();
        host.Pump();
    }
    if (errors.Count > 0)
        throw new InvalidOperationException(
            string.Join("\n", errors.Select(e => e.exceptionThrown))
        );
    Check(!Find<ErrorWidget>().Any(), "no ErrorWidget");
}
IEnumerable<T> Find<T>()
    where T : Widget => Elements(binding.rootElement!).Select(e => e.widget).OfType<T>();
IEnumerable<Element> Elements(Element element)
{
    yield return element;
    var children = new List<Element>();
    element.visitChildren(children.Add);
    foreach (var child in children)
    foreach (var descendant in Elements(child))
        yield return descendant;
}
void Toggle(string tooltip)
{
    var buttons = Find<IconButton>().ToArray();
    var button =
        buttons.SingleOrDefault(b => b.tooltip == tooltip)
        ?? throw new InvalidOperationException(
            "Missing toggle " + tooltip + ": " + string.Join(",", buttons.Select(b => b.tooltip))
        );
    button.onPressed!();
    Pump();
}
void Snapshot(string name)
{
    var size = host.Metrics.physicalSize;
    using var surface = SKSurface.Create(new SKImageInfo((int)size.width, (int)size.height));
    Check(
        renderer.Paint(surface, (int)size.width, (int)size.height) is not null,
        "Skia paints picker"
    );
    var directory = Path.Combine(AppContext.BaseDirectory, "snapshots");
    Directory.CreateDirectory(directory);
    using var image = surface.Snapshot();
    using var data = image.Encode(SKEncodedImageFormat.Png, 100);
    File.WriteAllBytes(
        Path.Combine(directory, name + (portrait ? "-portrait" : "-landscape") + ".png"),
        data.ToArray()
    );
}
bool ValidateForm() =>
    (
        (FormState)
            ((StatefulElement)Elements(binding.rootElement!).Single(e => e.widget is Form)).state
    ).validate();
static void Check(bool condition, string message)
{
    if (!condition)
        throw new InvalidOperationException(message);
    Console.WriteLine("PASS " + message);
}

// Deterministic host services; layout and raster use the production framework and Skia.
// Native OS input, window presentation, and keyboard UI are outside this regression.
sealed class BuildHost(Size size)
    : IViewHostCapability,
        IFrameHostCapability,
        IPlatformMessageHostCapability,
        IPlatformEnvironmentHostCapability,
        IPlatformServicesHostCapability,
        ISkiaSceneRendererHost,
        ITextInputHostCapability,
        IInputHostCapability,
        IViewFocusRequestCapability
{
    public ViewMetrics Metrics { get; } =
        new(
            size,
            1,
            ViewPadding.zero,
            ViewPadding.zero,
            ViewPadding.zero,
            AppLifecycleState.resumed,
            1,
            1
        );
    public DorotiViewEpoch ViewEpoch =>
        new(1, 1, 1, size.width, size.height, (int)size.width, (int)size.height, 1, 1, 1);
    public PlatformConfiguration Configuration { get; } =
        new([new Locale("en", "US")], Brightness.light, true, false, HostOperatingSystem.windows);
    public event Action<ViewMetrics>? MetricsChanged
    {
        add { }
        remove { }
    }
    public event Action<AppLifecycleState>? LifecycleChanged
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
    public event Action<PlatformConfiguration>? ConfigurationChanged
    {
        add { }
        remove { }
    }

    public void Show() { }

    public void Resize(Size size) { }

    public void Close() { }

    public void Dispose() { }

    private Action<TimeSpan>? _frame;
    private long _milliseconds;

    public void ScheduleFrame(Action<TimeSpan> callback) => _frame = callback;

    public void Pump()
    {
        var callback = _frame;
        _frame = null;
        callback?.Invoke(TimeSpan.FromMilliseconds(_milliseconds += 100));
    }

    public ValueTask<ReadOnlyMemory<byte>?> SendAsync(
        string channel,
        ReadOnlyMemory<byte>? data,
        CancellationToken cancellationToken = default
    ) => ValueTask.FromResult<ReadOnlyMemory<byte>?>(null);

    public void SetMessageHandler(string channel, PlatformMessageHandler? handler) { }

    public ValueTask<string?> GetClipboardTextAsync(
        CancellationToken cancellationToken = default
    ) => ValueTask.FromResult<string?>(null);

    public ValueTask SetClipboardTextAsync(
        string text,
        CancellationToken cancellationToken = default
    ) => ValueTask.CompletedTask;

    public void SetCursor(DorotiMouseCursorKind cursor) { }

    public long InputSequence => 0;
    public long SurfaceGeneration => 1;
    public DorotiResizeEpoch ResizeTarget =>
        new(1, size.width, size.height, (int)size.width, (int)size.height, 1, 1);
    public event Action<int, SemanticsAction, object?>? SemanticsAction
    {
        add { }
        remove { }
    }
    public event Action<long, TimeSpan>? InputReceived
    {
        add { }
        remove { }
    }

    public void UpdateSemantics(SemanticsUpdate update) { }

    public void ClearSemantics() { }

    public void RequestInvalidate() { }

    public event Action<DorotiTextEditingState>? EditingStateChanged
    {
        add { }
        remove { }
    }
    public event Action<DorotiTextInputAction>? ActionPerformed
    {
        add { }
        remove { }
    }

    public void SetClient(
        DorotiTextInputConfiguration configuration,
        DorotiTextEditingState initialState
    ) { }

    public void UpdateState(DorotiTextEditingState state) { }

    public void SetCaretRect(Rect logicalRect) { }

    public void ClearClient() { }

    public event Action<PointerDataPacket>? PointerData
    {
        add { }
        remove { }
    }
    public event Action<KeyData>? KeyData
    {
        add { }
        remove { }
    }
    public event Action<RawFocusData>? FocusData
    {
        add { }
        remove { }
    }

    public void RequestFocus(ViewFocusState state, ViewFocusDirection direction) { }
}
