using Doroti.Framework.Foundation;
using Doroti.Framework.Services;
using Doroti.Framework.Widgets;
using Doroti.Ui;
using Material = Doroti.Framework.Material;
using Cupertino = Doroti.Framework.Cupertino;

internal static class ContextMenuContracts
{
    public static void Verify()
    {
        ClipboardStatusNotifier webStatus = new _WebClipboardStatusNotifier__editable_text();
        Require(webStatus.value == ClipboardStatus.pasteable, "web clipboard status is pasteable through the base notifier API");
        webStatus.update().GetAwaiter().GetResult();
        Require(webStatus.value == ClipboardStatus.pasteable, "web status update needs no clipboard read permission");
        foreach (var platform in Enum.GetValues<TargetPlatform>())
        {
            using var environment = PlatformEnvironmentContext.Enter(new PlatformConfiguration(
                locales: [], platformBrightness: Brightness.light, alwaysUse24HourFormat: false,
                nativeSpellCheckServiceDefined: false,
                operatingSystem: Enum.Parse<HostOperatingSystem>(platform.ToString())));
            var context = new MenuContext(platform);
            var calls = 0;
            Action action = () => calls++;
            var items = new List<ContextMenuButtonItem>
            {
                new(action, label: "Custom action"),
                new(null, label: "Unavailable action"),
            };
            var anchors = new TextSelectionToolbarAnchors(new Offset(80, 60), new Offset(80, 100));
            var toolbar = Material.AdaptiveTextSelectionToolbar.CreateButtonItems(buttonItems: items, anchors: anchors);
            var expected = platform switch
            {
                TargetPlatform.android => typeof(Material.TextSelectionToolbar),
                TargetPlatform.iOS => typeof(Cupertino.CupertinoTextSelectionToolbar),
                TargetPlatform.macOS => typeof(Cupertino.CupertinoDesktopTextSelectionToolbar),
                _ => typeof(Material.DesktopTextSelectionToolbar),
            };
            Require(toolbar.build(context).GetType() == expected, $"{platform}: platform toolbar");
            var buttons = Material.AdaptiveTextSelectionToolbar.getAdaptiveButtons(context, items).ToList();
            Action? Callback(Widget button) => button switch
            {
                Material.DesktopTextSelectionToolbarButton b => b.onPressed,
                Material.TextSelectionToolbarTextButton b => b.onPressed,
                Cupertino.CupertinoTextSelectionToolbarButton b => b.onPressed,
                Cupertino.CupertinoDesktopTextSelectionToolbarButton b => b.onPressed,
                _ => throw new InvalidOperationException(button.GetType().Name),
            };
            Require(Callback(buttons[1]) is null, $"{platform}: disabled item stays disabled");
            Callback(buttons[0])!();
            Require(calls == 1, $"{platform}: callback runs once");
            Require(Material.AdaptiveTextSelectionToolbar.getButtonLabel(context, items[0]) == "Custom action",
                $"{platform}: explicit label survives adaptation");
            Require(Material.AdaptiveTextSelectionToolbar.CreateButtonItems(buttonItems: [], anchors: anchors)
                .build(context) is SizedBox, $"{platform}: empty menu is hidden");
            var cupertino = Cupertino.CupertinoAdaptiveTextSelectionToolbar.CreateButtonItems(buttonItems: items, anchors: anchors);
            Require(cupertino.build(context).GetType() == (platform is TargetPlatform.android or TargetPlatform.iOS or TargetPlatform.fuchsia
                ? typeof(Cupertino.CupertinoTextSelectionToolbar) : typeof(Cupertino.CupertinoDesktopTextSelectionToolbar)),
                $"{platform}: Cupertino mobile/desktop adaptation");

            var controller = TextEditingController.CreateFromValue(new TextEditingValue(
                text: "Hello Doroti", selection: new TextSelection(baseOffset: 0, extentOffset: 5)));
            List<ContextMenuButtonType> Items(bool readOnly = false, bool obscure = false, ClipboardStatus status = ClipboardStatus.pasteable)
            {
                var field = new EditableText(controller: controller, focusNode: new FocusNode(),
                    readOnly: readOnly, obscureText: obscure,
                    style: new Doroti.Framework.Painting.TextStyle(), cursorColor: new Color(0xff000000),
                    backgroundCursorColor: new Color(0xff808080), autofillHints: [],
                    selectionControls: Material.Desktop_text_selectionLibrary.desktopTextSelectionHandleControls);
                var state = new EditableTextState { _widget = field };
                state.clipboardStatus.value = status;
                if (controller.text.Length == 0 && status != ClipboardStatus.pasteable)
                    Require(state.contextMenuButtonItems.All(item => item.onPressed is null), $"{platform}: unavailable empty-field actions are disabled");
                var result = state.contextMenuButtonItems.Select(item => item.type).ToList();
                // The state is deliberately unmounted: initState has not registered
                // clipboard lifecycle observers with a WidgetsBinding.
                field.focusNode.dispose();
                return result;
            }
            var expectedItems = platform switch
            {
                TargetPlatform.android => new[] { ContextMenuButtonType.cut, ContextMenuButtonType.copy, ContextMenuButtonType.paste, ContextMenuButtonType.share, ContextMenuButtonType.selectAll },
                TargetPlatform.iOS => [ContextMenuButtonType.cut, ContextMenuButtonType.copy, ContextMenuButtonType.paste, ContextMenuButtonType.lookUp, ContextMenuButtonType.searchWeb, ContextMenuButtonType.share],
                TargetPlatform.macOS => [ContextMenuButtonType.cut, ContextMenuButtonType.copy, ContextMenuButtonType.paste],
                _ => [ContextMenuButtonType.cut, ContextMenuButtonType.copy, ContextMenuButtonType.paste, ContextMenuButtonType.selectAll],
            };
            Require(Items().SequenceEqual(expectedItems), $"{platform}: selected text actions and ordering");
            var readOnlyItems = Items(readOnly: true);
            Require(readOnlyItems.Contains(ContextMenuButtonType.copy) &&
                !readOnlyItems.Contains(ContextMenuButtonType.cut) && !readOnlyItems.Contains(ContextMenuButtonType.paste),
                $"{platform}: read-only selection cannot cut/paste");
            var passwordItems = Items(obscure: true);
            Require(!passwordItems.Contains(ContextMenuButtonType.copy) && !passwordItems.Contains(ContextMenuButtonType.cut) &&
                !passwordItems.Contains(ContextMenuButtonType.share), $"{platform}: password selection is not exposed");
            controller.selection = TextSelection.CreateCollapsed(offset: 5);
            var collapsedItems = Items();
            Require(!collapsedItems.Contains(ContextMenuButtonType.copy) && !collapsedItems.Contains(ContextMenuButtonType.cut),
                $"{platform}: caret alone cannot copy/cut");
            Require(collapsedItems.Contains(ContextMenuButtonType.selectAll) == (platform != TargetPlatform.macOS),
                $"{platform}: select-all follows platform rules");
            controller.value = new TextEditingValue(text: "", selection: TextSelection.CreateCollapsed(0));
            Require(Items().SequenceEqual([ContextMenuButtonType.paste]), $"{platform}: empty field offers paste from the clipboard");
            foreach (var status in new[] { ClipboardStatus.unknown, ClipboardStatus.notPasteable })
            {
                var emptyItems = Items(status: status);
                Require(emptyItems.Contains(ContextMenuButtonType.paste) ==
                    (platform is TargetPlatform.windows or TargetPlatform.linux or TargetPlatform.macOS),
                    $"{platform}: empty desktop field retains a disabled paste item ({status})");
                Require(Items(readOnly: true, status: status).Count == 0, $"{platform}: read-only empty field cannot paste");
            }
            controller.dispose();
        }
        VerifyOverlayThemes();
        Require(BrowserContextMenu.enabled, "web retains browser-native menu by default");
        Console.WriteLine("Context menus: PASS (six platforms, action states, disabled callbacks, overlay themes, web default)");
    }

    private static void VerifyOverlayThemes()
    {
        var previousBinding = WidgetsBinding._instance;
        var binding = (WidgetsFlutterBinding)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(WidgetsFlutterBinding));
        binding._buildOwner = new BuildOwner(focusManager: new FocusManager());
        WidgetsBinding._instance = binding;
        var root = new MenuContext(TargetPlatform.windows);
        var overlay = new MenuOverlay(root);
        var entryState = new _OverlayEntryWidgetState__overlay
        {
            _widget = new _OverlayEntryWidget__overlay(key: null!, entry: new OverlayEntry(_ => SizedBox.CreateShrink()), overlayState: overlay),
        };
        var marker = new _RenderTheaterMarker__overlay(null!, entryState, SizedBox.CreateShrink()).createElement();
        var caller = new MenuContext(TargetPlatform.macOS, marker, root, Brightness.dark);
        var controller = new ContextMenuController();
        var builds = 0;
        try
        {
            controller.show(caller, context => { builds++; return SizedBox.CreateShrink(); });
            var entry = overlay.Entry!;
            Require(ContextMenuController._capturedThemes!._themes.Contains(caller.LocalTheme),
                "root overlay captures the caller's platform/palette theme");
            var captured = (_CaptureAll__inherited_theme)entry.builder(root);
            Require(builds == 0 && captured.child is Builder, "menu builder waits until captured themes are installed");
            var wrapped = (Material.Theme)captured.build(root);
            Require(wrapped.data.platform == TargetPlatform.macOS && wrapped.data.brightness == Brightness.dark,
                "overlay retains caller platform override and dark palette");
            ((Builder)wrapped.child).build(caller);
            Require(builds == 1, "menu builder is invoked under themed subtree");
            var secondCaller = new MenuContext(TargetPlatform.iOS, marker, root);
            controller.show(secondCaller, _ => SizedBox.CreateShrink());
            Require(ReferenceEquals(entry, overlay.Entry) && ContextMenuController._capturedThemes!._themes.Contains(secondCaller.LocalTheme),
                "updating an open menu refreshes captured themes without reinserting the overlay");
        }
        finally
        {
            controller.remove();
            WidgetsBinding._instance = previousBinding;
        }
        Require(ContextMenuController._capturedThemes is null && !controller.isShown, "removal releases captured themes");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private sealed class MenuContext : StatelessElement
    {
        public InheritedTheme LocalTheme { get; }
        private readonly InheritedElement? _marker;
        private readonly Element? _root;
        public MenuContext(TargetPlatform platform, InheritedElement? marker = null, Element? root = null,
            Brightness brightness = Brightness.light)
            : base(new Builder(builder: _ => SizedBox.CreateShrink()))
        {
            LocalTheme = (InheritedTheme)new Material.Theme(
                data: Material.ThemeData.Create(platform: platform, brightness: brightness), child: SizedBox.CreateShrink()).build(this);
            _marker = marker;
            _root = root;
        }
        public override T? dependOnInheritedWidgetOfExactType<T>(object? aspect = null) where T : default =>
            LocalTheme is T result ? result : default;
        public override InheritedElement? getElementForInheritedWidgetOfExactType<T>() =>
            typeof(T) == typeof(_RenderTheaterMarker__overlay) ? _marker : null;
        public override void visitAncestorElements(Func<Element, bool> visitor)
        {
            if (visitor(LocalTheme.createElement()) && _root is not null) visitor(_root);
        }
    }

    private sealed class MenuOverlay(BuildContext root) : OverlayState
    {
        public OverlayEntry? Entry { get; private set; }
        public override BuildContext context => root;
        public override void insert(OverlayEntry entry, OverlayEntry? below = null, OverlayEntry? above = null)
        {
            Entry = entry;
            entry._overlay = this;
        }
    }
}
