using Doroti.Ui;

// Only the OS boundary is replaced. Clipboard and EditableText methods remain
// production code so a missing Services-to-host connection cannot pass.
internal sealed class ClipboardFixtureHost : IViewHostCapability, IPlatformServicesHostCapability
{
    public string? Text;
    public int Writes;
    public int Reads, StatusQueries;
    public ViewMetrics Metrics { get; } = new(Size.zero, 1, default, default, default, AppLifecycleState.resumed, 1, 1);
    public DorotiViewEpoch ViewEpoch { get; } = new(1, 1, 1, 0, 0, 0, 0, 1, 1, 0);
    public event Action<ViewMetrics>? MetricsChanged { add { } remove { } }
    public event Action<AppLifecycleState>? LifecycleChanged { add { } remove { } }
    public event Action? CloseRequested { add { } remove { } }
    public event Action? Closed;
    public void Show() { }
    public void Resize(Size logicalSize) { }
    public void Close() => Closed?.Invoke();
    public void Dispose() { }
    public void SetCursor(DorotiMouseCursorKind cursor) { }
    public ValueTask<string?> GetClipboardTextAsync(CancellationToken cancellationToken = default) { Reads++; return ValueTask.FromResult(Text); }
    public ValueTask<bool> HasClipboardTextAsync(CancellationToken cancellationToken = default) { StatusQueries++; return ValueTask.FromResult(!string.IsNullOrEmpty(Text)); }
    public ValueTask SetClipboardTextAsync(string text, CancellationToken cancellationToken = default)
    {
        Text = text;
        Writes++;
        return ValueTask.CompletedTask;
    }
}
