namespace Doroti.Ui;

/// <summary>Application window identity, never a native handle or a view identifier.</summary>
public readonly record struct WindowId(Guid Value);
public enum WindowKind { Regular, Dialog, Popup, Tooltip, Satellite }
public enum WindowAvailability { Supported, Unsupported }
public enum WindowCloseReason { Application, NativeRequest, OwnerClosed, ApplicationExit }
public enum WindowAction { Show, Hide, Focus, SetSize, SetTitle }
public sealed record WindowCapabilityResult(WindowAvailability Availability, string? Reason = null)
{
    public static WindowCapabilityResult Supported { get; } = new(WindowAvailability.Supported);
    public void RequireSupported() { if (Availability != WindowAvailability.Supported) throw new NotSupportedException(Reason ?? "Window request is unsupported."); }
}
public sealed record WindowAnchor(WindowId Owner, Rect LogicalBounds);
public sealed record WindowRequest(WindowKind Kind, string Title, Size Size, WindowId? Owner = null, WindowAnchor? Anchor = null, bool Modal = false, bool Activate = true);
public sealed record WindowSnapshot(WindowId Id, WindowKind Kind, ulong? ViewId, Size Size, double DevicePixelRatio, bool Visible, bool Closed, bool Closing = false);
public sealed record WindowActionRequest(WindowId Window, WindowAction Action, Size? Size = null, string? Title = null);
public sealed record WindowEvent(WindowSnapshot Window, WindowCloseReason? CloseReason = null);

/// <summary>One application registry and command owner shared by every view.</summary>
public interface IWindowService
{
    WindowCapabilityResult Evaluate(WindowRequest request);
    IReadOnlyList<WindowSnapshot> GetWindows();
    event Action<WindowEvent>? Changed;
    ValueTask<WindowSnapshot> CreateAsync(WindowRequest request, CancellationToken cancellationToken = default);
    ValueTask<WindowSnapshot> ExecuteAsync(WindowActionRequest request, CancellationToken cancellationToken = default);
    ValueTask<bool> CloseAsync(WindowId window, CancellationToken cancellationToken = default);
}
public interface IWindowingHostCapability : IWindowService { }
public enum PlatformMenuPresentation { Auto, Native, Overlay }
[Flags] public enum PlatformMenuModifiers { None = 0, Shift = 1, Control = 2, Alt = 4, Meta = 8 }
public sealed record PlatformMenuShortcut(string? Character, long? LogicalKey = null, PlatformMenuModifiers Modifiers = PlatformMenuModifiers.None);
public sealed record PlatformMenuItem(string Id, string Label, bool Enabled = true, bool Checked = false, IReadOnlyList<PlatformMenuItem>? Children = null, bool Separator = false, PlatformMenuShortcut? Shortcut = null, string? PlatformRole = null);
public sealed record PlatformMenuRequest(WindowId Window, IReadOnlyList<PlatformMenuItem> Items, Rect Anchor, PlatformMenuPresentation Presentation = PlatformMenuPresentation.Auto);
public enum PlatformMenuDismissal { Selected, Canceled, WindowClosed }
public sealed record PlatformMenuResult(PlatformMenuDismissal Dismissal, string? ItemId = null);
public interface IPlatformMenuHostCapability
{
    WindowCapabilityResult Evaluate(PlatformMenuRequest request);
    ValueTask<PlatformMenuResult> ShowAsync(PlatformMenuRequest request, CancellationToken cancellationToken = default);
}

public sealed record PlatformMenuBarRequest(WindowId Window, ulong ViewId, long Generation, IReadOnlyList<PlatformMenuItem> Items);
public enum PlatformMenuEventKind { Selected, Opened, Closed }
public sealed record PlatformMenuEvent(WindowId Window, ulong ViewId, long Generation, string ItemId, PlatformMenuEventKind Kind);
public interface IPlatformMenuBarRegistration : IAsyncDisposable { }
public interface IPlatformMenuBarHostCapability
{
    WindowCapabilityResult Evaluate(PlatformMenuBarRequest request);
    ValueTask<IPlatformMenuBarRegistration> SetAsync(PlatformMenuBarRequest request, Action<PlatformMenuEvent> callback, CancellationToken cancellationToken = default);
}
