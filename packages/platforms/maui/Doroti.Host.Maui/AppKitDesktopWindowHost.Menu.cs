#if MACOS
using AppKit;
using CoreGraphics;
using Doroti.Ui;
using Foundation;

namespace Doroti.Host.Maui;

internal sealed partial class AppKitDesktopWindowHost
{
    private NSMenu? _trackingMenu;
    private MenuBarRegistration? _menuBar;

    public WindowCapabilityResult Evaluate(PlatformMenuRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        RequireMenuOwner(request.Window);
        if (!Enum.IsDefined(request.Presentation)) throw new ArgumentException("Unknown menu presentation.");
        var rect = request.Anchor;
        if (!double.IsFinite(rect.left) || !double.IsFinite(rect.top) || !double.IsFinite(rect.right) || !double.IsFinite(rect.bottom) || rect.width < 0 || rect.height < 0)
            throw new ArgumentException("A finite logical menu anchor is required.");
        if (request.Presentation == PlatformMenuPresentation.Overlay)
            return new(WindowAvailability.Unsupported, "Overlay menus belong to the design package.");
        return EvaluateMenuItems(request.Items);
    }

    private void RequireMenuOwner(WindowId window)
    {
        if (_disposed || _destroying || window != _id || _surface?.FrameworkView is not { } view || view.InvocationLifetime.IsCancellationRequested)
            throw new InvalidOperationException("The menu owner is not a live AppKit view.");
    }

    private static WindowCapabilityResult EvaluateMenuItems(IReadOnlyList<PlatformMenuItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        if (items.Count == 0) throw new ArgumentException("A menu must have items.");
        var ids = new HashSet<string>(StringComparer.Ordinal);
        var count = 0;
        var unsupported = false;
        void Visit(IReadOnlyList<PlatformMenuItem> branch, int depth)
        {
            if (depth > 16) throw new ArgumentException("Menu depth exceeds 16.");
            foreach (var item in branch)
            {
                ArgumentNullException.ThrowIfNull(item);
                if (++count > 512) throw new ArgumentException("Menu size exceeds 512.");
                if (!item.Separator && (string.IsNullOrWhiteSpace(item.Id) || !ids.Add(item.Id)))
                    throw new ArgumentException("Menu item IDs must be nonempty and unique.");
                ArgumentNullException.ThrowIfNull(item.Label);
                if (item.PlatformRole is not null || item.Shortcut is { LogicalKey: not null }) unsupported = true;
                if (item.Shortcut is { } shortcut && ((shortcut.Modifiers & ~(PlatformMenuModifiers.Shift | PlatformMenuModifiers.Control | PlatformMenuModifiers.Alt | PlatformMenuModifiers.Meta)) != 0 || shortcut.LogicalKey is null && string.IsNullOrEmpty(shortcut.Character)))
                    throw new ArgumentException("A menu shortcut requires a character and known modifiers.");
                if (item.Children is { } children) Visit(children, depth + 1);
            }
        }
        Visit(items, 0);
        return unsupported ? new(WindowAvailability.Unsupported, "AppKit menu roles and logical-key shortcuts are not implemented; use character shortcuts.") : WindowCapabilityResult.Supported;
    }

    private NSMenu BuildMenu(IReadOnlyList<PlatformMenuItem> items, Action<PlatformMenuItem> selected)
    {
        var menu = new NSMenu { AutoEnablesItems = false };
        try
        {
            foreach (var item in items)
            {
                if (item.Separator) { menu.AddItem(NSMenuItem.SeparatorItem); continue; }
                var native = new NSMenuItem(item.Label, (_, _) => selected(item))
                { Enabled = item.Enabled, State = item.Checked ? NSCellStateValue.On : NSCellStateValue.Off };
                if (item.Shortcut is { } shortcut)
                {
                    native.KeyEquivalent = shortcut.Character!;
                    native.KeyEquivalentModifierMask =
                        (shortcut.Modifiers.HasFlag(PlatformMenuModifiers.Shift) ? NSEventModifierMask.ShiftKeyMask : 0) |
                        (shortcut.Modifiers.HasFlag(PlatformMenuModifiers.Control) ? NSEventModifierMask.ControlKeyMask : 0) |
                        (shortcut.Modifiers.HasFlag(PlatformMenuModifiers.Alt) ? NSEventModifierMask.AlternateKeyMask : 0) |
                        (shortcut.Modifiers.HasFlag(PlatformMenuModifiers.Meta) ? NSEventModifierMask.CommandKeyMask : 0);
                }
                if (item.Children is { Count: > 0 } children) native.Submenu = BuildMenu(children, selected);
                menu.AddItem(native);
            }
            return menu;
        }
        catch { menu.Dispose(); throw; }
    }

    public async ValueTask<PlatformMenuResult> ShowAsync(PlatformMenuRequest request, CancellationToken cancellationToken = default)
    {
        Evaluate(request).RequireSupported();
        return await OnUiAsync(() =>
        {
            RequireMenuOwner(request.Window);
            if (_trackingMenu is not null) throw new InvalidOperationException("A menu is already tracking for this window.");
            string? selected = null;
            using var menu = BuildMenu(request.Items, item =>
            { if (!_disposed && !_destroying && _surface?.FrameworkView?.InvocationLifetime.IsCancellationRequested == false && item.Enabled && !cancellationToken.IsCancellationRequested) selected ??= item.Id; });
            _trackingMenu = menu;
            using var cancellation = cancellationToken.Register(() => NSApplication.SharedApplication.BeginInvokeOnMainThread(() =>
            { if (ReferenceEquals(_trackingMenu, menu)) menu.CancelTracking(); }));
            try
            {
                menu.PopUpMenu(null!, new CGPoint(request.Anchor.left, request.Anchor.bottom), _root!);
                cancellationToken.ThrowIfCancellationRequested();
                var dismissal = _disposed || _destroying || _surface?.FrameworkView?.InvocationLifetime.IsCancellationRequested != false
                    ? PlatformMenuDismissal.WindowClosed : selected is null ? PlatformMenuDismissal.Canceled : PlatformMenuDismissal.Selected;
                return new PlatformMenuResult(dismissal, dismissal == PlatformMenuDismissal.Selected ? selected : null);
            }
            finally { _trackingMenu = null; }
        }, cancellationToken);
    }

    public WindowCapabilityResult Evaluate(PlatformMenuBarRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        RequireMenuOwner(request.Window);
        var view = _surface!.FrameworkView!;
        if (view.viewId != request.ViewId || view.SceneOwner.Generation != request.Generation)
            throw new InvalidOperationException("The menubar request belongs to a stale view generation.");
        return EvaluateMenuItems(request.Items);
    }

    public async ValueTask<IPlatformMenuBarRegistration> SetAsync(PlatformMenuBarRequest request, Action<PlatformMenuEvent> callback, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(callback);
        Evaluate(request).RequireSupported();
        return await OnUiAsync<IPlatformMenuBarRegistration>(() =>
        {
            Evaluate(request).RequireSupported();
            _menuBar?.DisposeOnUi();
            var registration = new MenuBarRegistration(this, request, callback);
            registration.Menu = BuildMenu(request.Items, item => registration.Select(item));
            _factory!.DefaultMenu ??= NSApplication.SharedApplication.MainMenu;
            if (_factory.DefaultMenu?.Items.FirstOrDefault() is { Submenu: { } applicationMenu } applicationItem)
                registration.Menu.InsertItem(new NSMenuItem(applicationItem.Title) { Submenu = (NSMenu)applicationMenu.Copy() }, 0);
            _menuBar = registration;
            ActivateMenuBar();
            return registration;
        }, cancellationToken);
    }

    private void ActivateMenuBar()
    {
        if (_native?.IsKeyWindow != true) return;
        if (_menuBar is not { } registration)
        {
            if (_factory?.DefaultMenu is { } defaultMenu) NSApplication.SharedApplication.MainMenu = defaultMenu;
            return;
        }
        NSApplication.SharedApplication.MainMenu = registration.Menu;
    }

    private sealed class MenuBarRegistration(AppKitDesktopWindowHost host, PlatformMenuBarRequest request, Action<PlatformMenuEvent> callback) : IPlatformMenuBarRegistration
    {
        internal NSMenu Menu = null!;
        private bool _disposed;
        internal void Select(PlatformMenuItem item)
        {
            if (_disposed || !item.Enabled || host._disposed || host._destroying || host._native?.IsKeyWindow != true ||
                host._surface?.FrameworkView is not { } view || view.InvocationLifetime.IsCancellationRequested ||
                view.viewId != request.ViewId || view.SceneOwner.Generation != request.Generation) return;
            view.DispatchPlatformEvent(() => callback(new(request.Window, request.ViewId, request.Generation, item.Id, PlatformMenuEventKind.Selected)));
        }
        internal void DisposeOnUi()
        {
            if (_disposed) return;
            _disposed = true;
            Menu.CancelTracking();
            if (ReferenceEquals(NSApplication.SharedApplication.MainMenu, Menu))
                NSApplication.SharedApplication.MainMenu = host._factory!.DefaultMenu ?? new NSMenu();
            if (ReferenceEquals(host._menuBar, this)) host._menuBar = null;
            Menu.Dispose();
        }
        public async ValueTask DisposeAsync() => await OnUiAsync(() => { DisposeOnUi(); return true; }, CancellationToken.None);
    }
}
#endif
