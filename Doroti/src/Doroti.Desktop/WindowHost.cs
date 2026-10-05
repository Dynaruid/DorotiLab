using WindowId = Doroti.Ui.WindowId;

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
    /// <summary>Maps logical presentation to the provider's visibility and shell policy before evaluation.</summary>
    WindowOptions MapRequest(Doroti.Ui.WindowRequest request) => new()
    {
        Title = request.Title, Size = request.Size, Kind = request.Kind,
        OwnerWindowId = request.Owner, Anchor = request.Anchor, Modal = request.Modal,
        Activate = request.Activate, SkipTaskbar = request.Kind != Doroti.Ui.WindowKind.Regular,
        Resizable = request.Kind is Doroti.Ui.WindowKind.Regular or Doroti.Ui.WindowKind.Satellite,
        StartupVisibility = WindowStartupVisibility.Manual,
    };
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

public sealed record WindowCreateOptions
{
    public WindowOptions Options { get; init; } = new();

    // Native ownership is evaluated by the selected provider before allocation.
    public WindowId? OwnerWindowId { get; init; }
    public Func<DesktopWindowContext, CancellationToken, Task>? OnCreated { get; init; }
}

internal sealed class WindowSubscription(Action unsubscribe) : IDisposable
{
    private Action? _unsubscribe = unsubscribe;

    public void Dispose() => Interlocked.Exchange(ref _unsubscribe, null)?.Invoke();
}
