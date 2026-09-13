#if IOS && !MACCATALYST
using System.Runtime.Versioning;
using System.Text.Json;
using CoreGraphics;
using Doroti.Ui;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Doroti.Host.Maui;

internal sealed record UIKitMenuItem(string Type, string? Title, string? CallbackId);

internal sealed class MauiUIKitContextMenuChannel : IPlatformMessageHostCapability
{
    private readonly IPlatformMessageHostCapability _fallback;
    private readonly MauiTextInputBridge _input;
    private PlatformMessageHandler? _frameworkHandler;
    internal Action<Action>? Dispatch { get; set; }
    public MauiUIKitContextMenuChannel(IPlatformMessageHostCapability fallback, MauiTextInputBridge input)
    {
        _fallback = fallback;
        _input = input;
        input.SystemContextMenuEvent += (method, id) =>
        {
            if (_frameworkHandler is not { } handler) return;
            using var stream = new MemoryStream();
            using (var writer = new Utf8JsonWriter(stream))
            {
                writer.WriteStartObject(); writer.WriteString("method", method); writer.WritePropertyName("args");
                if (id is null) writer.WriteNullValue();
                else { writer.WriteStartArray(); writer.WriteNumberValue(0); writer.WriteStringValue(id); writer.WriteEndArray(); }
                writer.WriteEndObject();
            }
            var bytes = stream.ToArray();
            Dispatch?.Invoke(() => { _ = handler(bytes, CancellationToken.None); });
        };
    }
    public async ValueTask<ReadOnlyMemory<byte>?> SendAsync(string channel, ReadOnlyMemory<byte>? data, CancellationToken cancellationToken = default)
    {
        if (channel == "flutter/platform" && data is { } bytes)
        {
            using var doc = JsonDocument.Parse(bytes);
            var root = doc.RootElement;
            var method = root.GetProperty("method").GetString();
            if (method == "ContextMenu.hideSystemContextMenu")
            {
                await _input.RunUIKitMenuMutation(_input.HideSystemContextMenu);
                return "[null]"u8.ToArray();
            }
            if (method == "ContextMenu.showSystemContextMenu")
            {
                if (!_input.SupportsSystemContextMenu) return "[\"unsupported\",\"iOS system menu unavailable\",null]"u8.ToArray();
                var args = root.GetProperty("args");
                var rect = args.GetProperty("targetRect");
                var target = new CGRect(rect.GetProperty("x").GetDouble(), rect.GetProperty("y").GetDouble(),
                    rect.GetProperty("width").GetDouble(), rect.GetProperty("height").GetDouble());
                var items = new List<UIKitMenuItem>();
                if (args.TryGetProperty("items", out var list))
                    foreach (var item in list.EnumerateArray())
                        items.Add(new(item.GetProperty("type").GetString()!,
                            item.TryGetProperty("title", out var title) ? title.GetString() : null,
                            item.TryGetProperty("callbackId", out var id) ? id.ToString() : null));
                cancellationToken.ThrowIfCancellationRequested();
                await _input.RunUIKitMenuMutation(() => _input.ShowSystemContextMenu(target, items));
                return "[null]"u8.ToArray();
            }
        }
        return await _fallback.SendAsync(channel, data, cancellationToken);
    }
    public void SetMessageHandler(string channel, PlatformMessageHandler? handler)
    {
        if (channel == "flutter/platform") _frameworkHandler = handler;
        _fallback.SetMessageHandler(channel, handler);
    }
}

public sealed partial class MauiTextInputBridge
{
    private UIEditMenuInteraction? _systemMenu;
    private UIView? _systemMenuView;
    private UIKitEditMenuDelegate? _systemMenuDelegate;
    internal bool SupportsSystemContextMenu => OperatingSystem.IsIOSVersionAtLeast(16) && _visualHost is not null;
    internal event Action<string, string?>? SystemContextMenuEvent;

    internal Task RunUIKitMenuMutation(Action action)
    {
        // Share the text-client queue: a focus request may have queued native
        // attachment immediately before the framework asks to show the menu.
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        DispatchInputMutation(() =>
        {
            try { action(); completion.SetResult(); }
            catch (Exception error) { completion.SetException(error); }
        });
        return completion.Task;
    }

    internal void ShowSystemContextMenu(CGRect target, IReadOnlyList<UIKitMenuItem> items)
    {
        if (!OperatingSystem.IsIOSVersionAtLeast(16)) return;
        HideSystemContextMenu();
        if (_disposed || !_hasClient || _configuration.readOnly || _visualHost?.Handler?.PlatformView is not UIView view ||
            _active?.Handler?.PlatformView is not UIView input || !SupportsSystemContextMenu) return;
        var menuDelegate = new UIKitEditMenuDelegate(this, target, items, input);
        _systemMenuDelegate = menuDelegate;
        _systemMenu = new UIEditMenuInteraction(menuDelegate);
        // The visible menu anchor must route UIKit's standard edit commands
        // to the invisible first responder, which is a sibling of this view.
        _systemMenuView = new UIKitEditMenuView(input) { Frame = view.Bounds, BackgroundColor = UIColor.Clear };
        view.AddSubview(_systemMenuView);
        _systemMenuView.AddInteraction(_systemMenu);
        _systemMenu.PresentEditMenu(UIEditMenuConfiguration.Create(null, new CGPoint(target.GetMidX(), target.GetMidY())));
    }
    internal void HideSystemContextMenu()
    {
        if (!OperatingSystem.IsIOSVersionAtLeast(16)) return;
        var menu = _systemMenu;
        _systemMenu = null;
        _systemMenuDelegate = null;
        if (menu is null) return;
        menu.DismissMenu();
        menu.View?.RemoveInteraction(menu);
        menu.Dispose();
        _systemMenuView?.RemoveFromSuperview();
        _systemMenuView?.Dispose();
        _systemMenuView = null;
    }
    private void SystemMenuDismissed(UIKitEditMenuDelegate menu)
    {
        if (!ReferenceEquals(_systemMenuDelegate, menu)) return;
        // Defer cleanup beyond UIKit's delegate callback.
        _inputDispatcher.Dispatch(() =>
        {
            if (!ReferenceEquals(_systemMenuDelegate, menu)) return;
            HideSystemContextMenu();
            SystemContextMenuEvent?.Invoke("ContextMenu.onDismissSystemContextMenu", null);
        });
    }
    private sealed class UIKitEditMenuView(UIView input) : UIView
    {
        public override UIResponder NextResponder => input;
        public override bool CanPerform(Selector action, NSObject? sender) => input.CanPerform(action, sender);
        public override NSObject? GetTargetForAction(Selector action, NSObject? sender) => input.GetTargetForAction(action, sender);
        public override UIView? HitTest(CGPoint point, UIEvent? uievent) => null;
    }

    [SupportedOSPlatform("ios16.0")]
    private sealed class UIKitEditMenuDelegate(MauiTextInputBridge owner, CGRect target,
        IReadOnlyList<UIKitMenuItem> items, UIView input) : UIEditMenuInteractionDelegate
    {
        public override CGRect GetTargetRect(UIEditMenuInteraction interaction, UIEditMenuConfiguration configuration) => target;
        public override UIMenu GetMenu(UIEditMenuInteraction interaction, UIEditMenuConfiguration configuration, UIMenuElement[] suggestedActions)
        {
            if (items.Count == 0) return UIMenu.Create(suggestedActions);
            var commands = new List<UIMenuElement>();
            foreach (var item in items)
            {
                var selector = item.Type switch
                {
                    "copy" => "copy:", "cut" => "cut:", "paste" => "paste:", "selectAll" => "selectAll:",
                    "captureTextFromCamera" => "captureTextFromCamera:", _ => null,
                };
                if (selector is not null)
                {
                    // Preserve UIKit's localized standard actions and paste
                    // provenance, including the OS clipboard access behavior.
                    var suggested = FindCommand(suggestedActions, selector);
                    if (suggested is not null) commands.Add(suggested);

                }
                else
                {
                    commands.Add(UIAction.Create(item.Title ?? item.Type, null, null, _ => Perform(item)));
                }
            }
            return UIMenu.Create(commands.ToArray());
        }
        private static UICommand? FindCommand(IEnumerable<UIMenuElement> elements, string selector)
        {
            foreach (var element in elements)
            {
                if (element is UICommand command && command.Action.Name == selector) return command;
                if (element is UIMenu menu && FindCommand(menu.Children, selector) is { } nested) return nested;
            }
            return null;
        }

        private void Perform(UIKitMenuItem item)
        {
            if (owner._disposed || !owner._hasClient || owner.HasPendingClientChange || !ReferenceEquals(owner._active?.Handler?.PlatformView, input)) return;
            if (item.Type == "custom")
            {
                owner.SystemContextMenuEvent?.Invoke("ContextMenu.onPerformCustomAction", item.CallbackId);
                return;
            }
            var text = owner._active!.Text ?? "";
            var start = Math.Clamp(owner._active.CursorPosition, 0, text.Length);
            var selected = text.Substring(start, Math.Clamp(owner._active.SelectionLength, 0, text.Length - start));
            var presenter = input.Window?.RootViewController;
            while (presenter?.PresentedViewController is { } next) presenter = next;
            if (presenter is null || selected.Length == 0) return;
            if (item.Type == "lookUp") presenter.PresentViewController(new UIReferenceLibraryViewController(selected), true, null);
            else if (item.Type == "share")
            {
                var sheet = new UIActivityViewController([new NSString(selected)], null);
                if (sheet.PopoverPresentationController is { } popover) { popover.SourceView = (UIView)owner._visualHost!.Handler!.PlatformView!; popover.SourceRect = target; }
                presenter.PresentViewController(sheet, true, null);
            }
            else if (item.Type == "searchWeb")
                UIApplication.SharedApplication.OpenUrl(new NSUrl("https://www.google.com/search?q=" + Uri.EscapeDataString(selected)), new UIApplicationOpenUrlOptions(), null);
        }
        public override void WillDismissMenu(UIEditMenuInteraction interaction, UIEditMenuConfiguration configuration, IUIEditMenuInteractionAnimating animator)
            => owner.SystemMenuDismissed(this);
    }
}
#endif
