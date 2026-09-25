using Doroti.Hosting;

namespace Doroti.Desktop;

public enum WindowCommandKind
{
    Show,
    Hide,
    Focus,
    Size,
    Bounds,
    Center,
    MinimumSize,
    MaximumSize,
    AlwaysOnTop,
    SkipTaskbar,
    Resizable,
    Minimize,
    Maximize,
    Restore,
    FullScreen,
    Title,
    Drag,
    Resize,
}

public sealed record WindowCommand(WindowCommandKind Kind, object? Value = null);

/// <summary>Implemented by an explicit native desktop adapter. All methods must marshal to its owner UI thread.</summary>
public interface IWindowHost : IAsyncDisposable
{
    WindowCapabilities Capabilities { get; }
    WindowState State { get; }
    Task ReadyToShow { get; }
    event Action<WindowState>? StateChanged;
    event Action? CloseRequested;
    event Action? Closed;
    Task InitializeAsync(
        WindowOptions options,
        DesktopWindowContext context,
        IDorotiViewEntrypoint content,
        CancellationToken cancellationToken
    );
    Task<WindowState> ExecuteAsync(WindowCommand command, CancellationToken cancellationToken);
    Task<WindowState> ApplyAppearanceAsync(
        WindowAppearanceOptions appearance,
        CancellationToken cancellationToken
    );

    /// <summary>Drain window-owned work, detach its views, destroy native window, then complete.</summary>
    Task CloseAsync(CancellationToken cancellationToken);
}

public interface IWindowHostFactory
{
    WindowManagerCapabilities Capabilities { get; }
    WindowEvaluation Evaluate(WindowCreateOptions options);
    ValueTask<IWindowHost> CreateAsync(
        WindowId id,
        WindowCreateOptions options,
        CancellationToken cancellationToken
    );
}

public sealed record DesktopWindowContext(
    DorotiWindowController Window,
    DorotiWindowManager Windows
);

public sealed class WindowContent
{
    private readonly Func<DesktopWindowContext, IDorotiViewEntrypoint> _factory;

    private WindowContent(Func<DesktopWindowContext, IDorotiViewEntrypoint> factory) =>
        _factory = factory;

    public static WindowContent FromEntrypoint(Func<IDorotiViewEntrypoint> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        return new(_ => factory());
    }

    public static WindowContent FromEntrypoint(
        Func<DesktopWindowContext, IDorotiViewEntrypoint> factory
    ) => new(factory ?? throw new ArgumentNullException(nameof(factory)));

    internal IDorotiViewEntrypoint Create(DesktopWindowContext context) =>
        _factory(context) ?? throw new InvalidOperationException("Content factory returned null.");
}

public sealed record WindowCreateOptions
{
    public WindowOptions Options { get; init; } = new();
    public required WindowContent Content { get; init; }

    // Owned windows are deliberately reserved and rejected until host ownership is implemented.
    public WindowId? OwnerWindowId { get; init; }
    public Func<DesktopWindowContext, CancellationToken, Task>? OnCreated { get; init; }
}

internal sealed class WindowSubscription(Action unsubscribe) : IDisposable
{
    private Action? _unsubscribe = unsubscribe;

    public void Dispose() => Interlocked.Exchange(ref _unsubscribe, null)?.Invoke();
}
