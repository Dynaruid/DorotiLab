using Doroti.Framework.Foundation;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Runtime;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Testing;

/// <summary>One serial test scope. Input uses platform packets, hit testing and GestureArena.</summary>
public sealed class WidgetTester : IDisposable
{
    [ThreadStatic] private static int _active;
    private readonly TestHost _host;
    private readonly PlatformDispatcher _dispatcher;
    private readonly WidgetsFlutterBinding _binding;
    private readonly SkiaSceneRenderer _renderer;
    private SKSurface _surface;
    private readonly IDisposable _clockScope;
    private readonly IDisposable _dispatcherScope;
    private readonly FlutterExceptionHandler? _previousError;
    private readonly List<Exception> _errors = [];
    private bool _disposed;
    private readonly int _ownerThread = Environment.CurrentManagedThreadId;
    private ulong _pointer;
    public TestClock Clock { get; } = new();
    public DorotiView View { get; }
    public int Frames { get; private set; }
    public SemanticsUpdate? Semantics => _host.Semantics;
    public bool HasTextClient => _host.HasTextClient;
    public SkiaFrameDiagnostics RendererDiagnostics => _renderer.Diagnostics;
    public SkiaCacheMemoryDiagnostics CacheMemory => _renderer.CaptureCacheMemory();
    public IReadOnlyList<DorotiFrameTraceEntry> FrameTrace => _dispatcher.frameTrace.Snapshot();

    public WidgetTester(Size? size = null, double devicePixelRatio = 1,
        IApplicationNavigationHostCapability? navigation = null)
    {
        if (!double.IsFinite(devicePixelRatio) || devicePixelRatio <= 0 ||
            size is { IsFinite: false } || size is { IsEmpty: true })
            throw new ArgumentOutOfRangeException(nameof(devicePixelRatio));
        if (Interlocked.CompareExchange(ref _active, 1, 0) != 0)
            throw new InvalidOperationException("WidgetTester currently requires serial, non-nested execution.");
        _previousError = FlutterError.onError;
        try
        {
        _clockScope = DartAsyncRuntime.enterTimeProvider(Clock);
        _dispatcher = new();
        _dispatcherScope = _dispatcher.EnterScope();
        _host = new(size ?? new Size(800, 600), devicePixelRatio);
        _renderer = new(1, _host, new Color(0xffffffff), null, "Testing/CPU", "cpu", "cpu", false);
        var capabilities = new DorotiViewCapabilities("Testing/CPU")
            .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, _host)
            .Register<IFrameHostCapability>(DorotiCapabilityIds.ViewFrameDispatch, _host)
            .Register<IInputHostCapability>(DorotiCapabilityIds.InputEvents, _host)
            .Register<ITextInputHostCapability>(DorotiCapabilityIds.TextInput, _host)
            .Register<IPlatformServicesHostCapability>(DorotiCapabilityIds.PlatformServices, _host)
            .Register<IPlatformEnvironmentHostCapability>(DorotiCapabilityIds.PlatformEnvironment, _host)
            .Register<IPlatformMessageHostCapability>(DorotiCapabilityIds.PlatformMessaging, _host)
            .Register<ISceneHostCapability>(DorotiCapabilityIds.GraphicsScene, _renderer)
            .Register<IParagraphHostCapability>(DorotiCapabilityIds.GraphicsText, _renderer)
            .Register<IFontHostCapability>(DorotiCapabilityIds.GraphicsFont, _renderer)
            .Register<IImageHostCapability>(DorotiCapabilityIds.GraphicsImage, _renderer)
            .Register<ITextureHostCapability>(DorotiCapabilityIds.GraphicsTexture, _renderer)
            .Register<ISemanticsHostCapability>(DorotiCapabilityIds.AccessibilitySemantics, _renderer);
        if (navigation is not null)
        {
            capabilities.Register<IApplicationNavigationHostCapability>(DorotiCapabilityIds.ApplicationNavigation, navigation);
            _dispatcher.defaultRouteName = navigation.Current.Location;
        }
        View = _dispatcher.RegisterView(1, capabilities);
        _dispatcher.frameTrace.MeasureRecordingTime = true;
        _dispatcher.frameTrace.ActivityClock = () => Clock.Elapsed;
        _renderer.AttachFrameworkTrace(_dispatcher.frameTrace);
        FlutterError.onError = details => _errors.Add(details.exception as Exception ?? new Exception(details.exception.ToString()));
        using (View.EnterPlatformEnvironmentScope()) _binding = new(_dispatcher);
        _surface = SKSurface.Create(new SKImageInfo((int)View.physicalSize.width, (int)View.physicalSize.height))
            ?? throw new InvalidOperationException("Cannot allocate the test CPU surface.");
        }
        catch
        {
            Release(_binding);
            ReleaseResources();
            throw;
        }
    }

    public void pumpWidget(Widget widget)
    {
        CheckAlive();
        View.DispatchPlatformEvent(() => _binding.attachRootWidget(_binding.wrapWithDefaultView(widget)));
        _binding.scheduleForcedFrame();
        pump();
    }
    public void setViewport(Size size, double devicePixelRatio)
    {
        CheckAlive();
        if (!size.IsFinite || size.IsEmpty || !double.IsFinite(devicePixelRatio) || devicePixelRatio <= 0)
            throw new ArgumentOutOfRangeException(nameof(size));
        var next = SKSurface.Create(new SKImageInfo((int)Math.Ceiling(size.width * devicePixelRatio),
            (int)Math.Ceiling(size.height * devicePixelRatio))) ?? throw new InvalidOperationException("Cannot allocate viewport.");
        _surface.Dispose();
        _surface = next;
        View.DispatchPlatformEvent(() => _host.SetMetrics(size, devicePixelRatio));
        _binding.scheduleForcedFrame();
        pump();
    }
    public SKColor pixel(int x, int y)
    {
        using var image = _surface.Snapshot();
        using var bitmap = SKBitmap.FromImage(image);
        return bitmap.GetPixel(x, y);
    }
    public Task reassemble()
    {
        Task? task = null;
        View.DispatchPlatformEvent(() => task = _binding.reassembleApplication());
        return task!;
    }
    public void pump(TimeSpan? duration = null)
    {
        CheckAlive();
        View.DispatchPlatformEvent(() => Clock.Advance(duration ?? TimeSpan.Zero));
        _host.Frame(Clock.Elapsed);
        _renderer.Paint(_surface, (int)View.physicalSize.width, (int)View.physicalSize.height);
        Frames++;
        ThrowErrors();
    }
    public int pumpAndSettle(TimeSpan? step = null, TimeSpan? timeout = null)
    {
        var interval = step ?? TimeSpan.FromMilliseconds(16);
        var limit = timeout ?? TimeSpan.FromSeconds(10);
        if (interval <= TimeSpan.Zero || limit <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(step));
        var deadline = Clock.Elapsed + limit;
        var count = 0;
        do
        {
            if (Clock.Elapsed >= deadline)
                throw new TimeoutException($"pumpAndSettle timed out: frames={count}, timers={Clock.PendingTimers}, scheduled={_binding.hasScheduledFrame}\n{DumpTree()}");
            pump(interval < deadline - Clock.Elapsed ? interval : deadline - Clock.Elapsed);
            count++;
        } while (_host.PendingFrame is not null || _binding.hasScheduledFrame || Clock.PendingTimers > 0);
        return count;
    }
    public IReadOnlyList<Element> find(Func<Widget, bool> predicate)
    {
        CheckAlive();
        var found = new List<Element>();
        void Visit(Element element)
        {
            if (predicate(element.widget)) found.Add(element);
            element.visitChildren(Visit);
        }
        if (_binding.rootElement is { } root) Visit(root);
        return found;
    }
    public IReadOnlyList<Element> byType<T>() where T : Widget => find(widget => widget is T);
    public IReadOnlyList<Element> byKey(Key key) => find(widget => Equals(widget.key, key));
    public IReadOnlyList<Element> text(string value) => find(widget => widget is Text text && text.data == value);
    public Offset center(Element element)
    {
        if (element.findRenderObject() is not RenderBox box || !box.hasSize)
            throw new InvalidOperationException("The element has no laid out RenderBox.");
        return box.localToGlobal(box.size.center(Offset.zero));
    }
    public void tap(Element element) => tapAt(center(element));
    public void tapAt(Offset position)
    {
        var id = ++_pointer;
        Send(PointerChange.add, position, id);
        Send(PointerChange.down, position, id);
        pump(TimeSpan.FromMilliseconds(1));
        Send(PointerChange.up, position, id);
        Send(PointerChange.remove, position, id);
        pump();
    }
    public void drag(Element element, Offset delta, int steps = 10)
    {
        if (steps is < 1 or > 30) throw new ArgumentOutOfRangeException(nameof(steps));
        var start = center(element);
        var id = ++_pointer;
        Send(PointerChange.add, start, id);
        Send(PointerChange.down, start, id);
        for (var i = 1; i <= steps; i++)
        {
            Send(PointerChange.move, start + delta * ((double)i / steps), id);
            pump(TimeSpan.FromMilliseconds(16));
        }
        Send(PointerChange.up, start + delta, id);
        Send(PointerChange.remove, start + delta, id);
    }
    private void Send(PointerChange change, Offset point, ulong id) => _host.Pointer(new(
        1, Clock.Elapsed, change, PointerDeviceKind.touch, id, point.dx * View.devicePixelRatio,
        point.dy * View.devicePixelRatio, 0, 0, change is PointerChange.down or PointerChange.move ? 1 : 0,
        pointerIdentifier: id));
    public void sendKey(KeyData data) => _host.Key(data);
    public void enterText(DorotiTextEditingState state) { _host.Edit(state); pump(); }
    public void performSemanticsAction(int id, SemanticsAction action, object? args = null) => _host.PerformSemanticsAction(id, action, args);
    public string DumpTree() => string.Join("\n", find(_ => true).Select(e => e.widget.toStringShort()));
    public void WritePng(string path)
    {
        using var image = _surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var file = File.Create(path);
        data.SaveTo(file);
    }
    private void CheckAlive()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (Environment.CurrentManagedThreadId != _ownerThread)
            throw new InvalidOperationException("Use WidgetTester on its owning thread.");
    }
    private void ThrowErrors()
    {
        if (_errors.Count == 0) return;
        var errors = _errors.ToArray();
        _errors.Clear();
        throw new AggregateException("Unhandled framework errors", errors);
    }
    public void Dispose()
    {
        if (_disposed) return;
        CheckAlive();
        _disposed = true;
        try { View.DispatchPlatformEvent(_binding.Dispose); }
        catch (Exception error) { _errors.Add(error); }
        finally { ReleaseResources(); }
        ThrowErrors();
    }
    private void Release(IDisposable? resource)
    {
        try { resource?.Dispose(); }
        catch (Exception error) { _errors.Add(error); }
    }
    private void ReleaseResources()
    {
        Release(_dispatcher);
        Release(_renderer);
        Release(_surface);
        Release(Clock);
        Release(_dispatcherScope);
        Release(_clockScope);
        FlutterError.onError = _previousError;
        Volatile.Write(ref _active, 0);
    }
}
