using Doroti.Graphics;
using Doroti.Platform;

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

/// <summary>Backend-neutral native input emitted by a shell top level.</summary>
public interface IShellInputService
{
    InputCapabilities Capabilities { get; }

    event Action<RawPointerEvent>? Pointer;

    event Action<RawKeyEvent>? Key;
}

public enum ShellTextEventKind
{
    Text,
    CompositionStarted,
    CompositionUpdated,
    CompositionEnded,
}

public readonly record struct ShellTextEvent(ShellTextEventKind Kind, string Text);

/// <summary>Native text-system bridge. Native IME objects never escape the vendor assembly.</summary>
public interface IShellTextInputService
{
    event Action<ShellTextEvent>? Text;

    void SetCaretRect(Rect logicalRect);
}

public interface IShellClipboardService
{
    ValueTask<ClipboardResult> GetTextAsync(CancellationToken cancellationToken = default);

    ValueTask<ClipboardResult> SetTextAsync(string text, CancellationToken cancellationToken = default);
}

public interface IShellCursorService
{
    void SetCursor(CursorKind cursor);
}

/// <summary>Framebuffer/OpenGL service shared by WGL and AppKit without exposing either native API.</summary>
public interface IShellGraphicsService
{
    string BackendIdentity { get; }

    void Present(ReadOnlySpan<byte> pixels, int width, int height, int rowBytes);

    IOpenGlWindowContext CreateOpenGlContext();
}

public interface IShellFocusService
{
    void RequestFocus(bool focused);
}

/// <summary>Posts validation input through the target's real native event path.</summary>
public interface IShellInputTestService
{
    void PostPointerMove(Offset logicalPosition);
    void PostPointerLeave(Offset logicalPosition);
    void PostPointerDown(Offset logicalPosition);
    void PostPointerUp(Offset logicalPosition);
    void PostPointerTap(Offset logicalPosition);
    void PostPointerDrag(Offset logicalStart, Offset logicalEnd);
    void PostPointerWheel(Offset logicalPosition, Offset wheelDelta);
    void PostPointerCaptureLoss(Offset logicalPosition);
    void PostKeyboardActivation(uint logicalKey);
    void PostTextInput(string text);
}

public interface IShellAccessibilityService
{
    void Update(SemanticsTreeSnapshot snapshot, Func<SemanticsActionRequest, bool> performAction);

    bool InvokeAction(int nodeId, SemanticsAction action, object? arguments = null);

    void Clear();
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
