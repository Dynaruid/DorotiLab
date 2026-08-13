using Doroti.Graphics;

namespace Doroti.Shell.Core;

public enum ShellWindowState
{
    Normal,
    Minimized,
    Maximized,
    Closed,
}

public readonly record struct ShellNativeHandle(nint Value, string Descriptor)
{
    public bool IsValid => Value != 0;
}

public readonly record struct ShellScreen(ulong Id, Rect WorkArea, double ScaleFactor);

public readonly record struct ShellClipboardResult(bool Success, string? Text = null, string? Diagnostic = null)
{
    public static ShellClipboardResult FromText(string? text) => new(true, text);

    public static ShellClipboardResult Failure(string diagnostic) => new(false, null, diagnostic);
}

public readonly record struct ShellWindowMetrics(
    Size LogicalClientSize,
    Size PhysicalClientSize,
    double ScaleFactor,
    long Generation,
    long ScaleGeneration,
    long SurfaceGeneration,
    ShellWindowState State);

public enum ShellWindowEventKind
{
    Opened,
    Activated,
    Deactivated,
    MetricsChanged,
    CaptureCancelled,
    CloseRequested,
    Closed,
}

public readonly record struct ShellWindowEvent(ShellWindowEventKind Kind, ShellWindowMetrics Metrics);

public interface IShellDispatcher
{
    bool CheckAccess();

    void VerifyAccess();

    void Post(Action callback);
}

public interface IShellEventLoop
{
    bool PumpOnce(bool waitForMessage = false);

    void Run(CancellationToken cancellationToken = default);

    void RequestExit();
}

public interface IShellTopLevel : IDisposable
{
    ulong Id { get; }

    ShellNativeHandle NativeHandle { get; }

    ShellWindowMetrics Metrics { get; }

    ShellPlatformServiceRegistry Services { get; }
}

public interface IShellWindow : IShellTopLevel
{
    event Action<ShellWindowEvent>? WindowEvent;

    IReadOnlyList<ShellScreen> Screens { get; }

    void Show();

    void Resize(Size logicalClientSize);

    void SetState(ShellWindowState state);

    void MoveToScreen(ulong screenId);

    void Close();
}

public interface IShellWindowingPlatform
{
    IShellDispatcher Dispatcher { get; }

    IShellEventLoop EventLoop { get; }

    IShellWindow CreateWindow(string title, Size initialLogicalClientSize);
}

public sealed class ShellPlatformServiceRegistry
{
    private readonly Dictionary<Type, object> _services = [];

    public void Add<TService>(TService service)
        where TService : class
    {
        ArgumentNullException.ThrowIfNull(service);
        if (!_services.TryAdd(typeof(TService), service))
        {
            throw new InvalidOperationException($"A shell service is already registered for {typeof(TService).FullName}.");
        }
    }

    public bool TryGet<TService>(out TService? service)
        where TService : class
    {
        service = _services.TryGetValue(typeof(TService), out var value) ? (TService)value : null;
        return service is not null;
    }

    public TService GetRequired<TService>()
        where TService : class
    {
        return TryGet<TService>(out var service)
            ? service!
            : throw new InvalidOperationException($"The shell service {typeof(TService).FullName} is not registered.");
    }
}
