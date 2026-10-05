using Doroti.Ui;
namespace Doroti.Desktop;
public sealed partial class DorotiWindowManager
{
    public event Action<WindowEvent>? Changed;
    public WindowCapabilityResult Evaluate(WindowRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (!Enum.IsDefined(request.Kind) || !double.IsFinite(request.Size.width) || !double.IsFinite(request.Size.height) || request.Size.width <= 0 || request.Size.height <= 0)
            throw new ArgumentException("Window kind and logical size must be valid.", nameof(request));
        if (request.Kind != WindowKind.Regular && request.Owner is null)
            return new(WindowAvailability.Unsupported, "Owned window kinds require an explicit owner.");
        if (request.Owner is { } owner && (!TryGetWindow(owner, out var parent) || parent!.State.Closed || parent.IsClosing))
            throw new InvalidOperationException("The owner is not a live window in this application.");
        if (request.Anchor is { } anchor && anchor.Owner != request.Owner)
            throw new ArgumentException("The anchor must belong to the requested owner.");
        var options = _factory.MapRequest(request);
        options.Validate();
        var result = _factory.Evaluate(new WindowCreateOptions { Options = options });
        return result.Support == WindowSupport.Supported ? WindowCapabilityResult.Supported : new(WindowAvailability.Unsupported, result.Reason);
    }
    IReadOnlyList<WindowSnapshot> IWindowService.GetWindows()
    {
        lock (_gate)
            return _windows.Values.Concat(_initializingWindows.Values.Where(window => window.View is not null && !window.State.Closed))
                .Select(Snapshot).ToArray();
    }
    public async ValueTask<WindowSnapshot> CreateAsync(WindowRequest request, CancellationToken cancellationToken = default)
    {
        Evaluate(request).RequireSupported();
        var window = await CreateWindowAsync(new() { Options = _factory.MapRequest(request) }, cancellationToken);
        return Snapshot(window);
    }
    public async ValueTask<WindowSnapshot> ExecuteAsync(WindowActionRequest request, CancellationToken cancellationToken = default)
    {
        if (!TryGetWindow(request.Window, out var window) || window is null) throw new ObjectDisposedException(nameof(WindowId));
        switch (request.Action)
        {
            case WindowAction.Show: await window.ShowAsync(cancellationToken); break;
            case WindowAction.Hide: await window.HideAsync(cancellationToken); break;
            case WindowAction.Focus: await window.FocusAsync(cancellationToken); break;
            case WindowAction.SetSize: await window.SetSizeAsync(request.Size ?? throw new ArgumentException("Size is required."), cancellationToken); break;
            case WindowAction.SetTitle: await window.SetTitleAsync(request.Title ?? throw new ArgumentException("Title is required."), cancellationToken); break;
            default: throw new ArgumentException("Unknown window action.");
        }
        PublishWindow(window); return Snapshot(window);
    }
    public async ValueTask<bool> CloseAsync(WindowId window, CancellationToken cancellationToken = default) =>
        !TryGetWindow(window, out var controller) || controller is null || await controller.CloseAsync(cancellationToken);
    public void AttachView(WindowId window, DorotiView view)
    {
        lock (_gate) { if (!_windows.TryGetValue(window, out var controller) && !_initializingWindows.TryGetValue(window, out controller)) throw new InvalidOperationException("Window is not registered."); controller.View = view; controller.ViewId = view.viewId; }
    }
    internal async Task<bool> CloseOwnedWindowsAsync(WindowId owner)
    {
        await _creation.WaitAsync();
        DorotiWindowController[] children;
        try { children = GetWindows().Where(window => window.OwnerWindowId == owner).Reverse().ToArray(); }
        finally { _creation.Release(); }
        foreach (var child in children)
            if (!await child.CloseAsync()) return false;
        return true;
    }
    private static WindowSnapshot Snapshot(DorotiWindowController window) => new(window.Id, window.Kind, window.ViewId, window.State.ClientSize, window.State.Scale, window.State.Visible, window.State.Closed, window.IsClosing);
    internal void PublishWindow(DorotiWindowController window)
    {
        var value = new WindowEvent(Snapshot(window));
        foreach (var callback in Changed?.GetInvocationList() ?? []) try { ((Action<WindowEvent>)callback)(value); } catch (Exception error) { ReportFailure(window, error); }
    }
}
