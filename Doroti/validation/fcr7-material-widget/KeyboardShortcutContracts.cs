using System.Reflection;
using System.Runtime.CompilerServices;
using Doroti.Framework.Services;
using Doroti.Framework.Widgets;
using Doroti.Runtime;
using Doroti.Host.WindowsAppSdk;

internal static class KeyboardShortcutContracts
{
    public static void Verify()
    {
        VerifyPlatformKeyMaps();
        VerifyReadingOrderTraversal();
        var appActions = WidgetsApp.defaultActionsForContext(new ActionContext((IIntentAction?)null, new ActionDispatcher()));
        Require(appActions[typeof(NextFocusIntent)] is NextFocusAction &&
            appActions[typeof(PreviousFocusIntent)] is PreviousFocusAction &&
            appActions.ContainsKey(typeof(RequestFocusIntent)) && appActions.ContainsKey(typeof(PrioritizedIntents)) &&
            appActions.Count == WidgetsApp.defaultActions.Count,
            "WidgetsApp installs all default actions while overriding only scrolling");
        var keyboard = new HardwareKeyboard();
        var manager = new ShortcutManager(DefaultTextEditingShortcuts._windowsShortcuts);
        keyboard.handleKeyEvent(new KeyDownEvent(PhysicalKeyboardKey.controlLeft, LogicalKeyboardKey.controlLeft, timeStamp: Duration.zero));
        foreach (var (key, physical, intent) in new[]
        {
            (LogicalKeyboardKey.keyA, PhysicalKeyboardKey.keyA, typeof(SelectAllTextIntent)),
            (LogicalKeyboardKey.keyC, PhysicalKeyboardKey.keyC, typeof(CopySelectionTextIntent)),
            (LogicalKeyboardKey.keyX, PhysicalKeyboardKey.keyX, typeof(CopySelectionTextIntent)),
            (LogicalKeyboardKey.keyV, PhysicalKeyboardKey.keyV, typeof(PasteTextIntent)),
            (LogicalKeyboardKey.keyZ, PhysicalKeyboardKey.keyZ, typeof(UndoTextIntent)),
        })
        {
            Require(manager._find(Down(physical, key), keyboard)?.GetType() == intent, $"Windows Ctrl+{key.keyLabel} resolves its editing intent");
            Require(manager._find(new KeyUpEvent(physical, key, Duration.zero), keyboard) is null, "key release cannot invoke an editing action");
        }
        keyboard.handleKeyEvent(new KeyUpEvent(PhysicalKeyboardKey.controlLeft, LogicalKeyboardKey.controlLeft, Duration.zero));
        Require(manager._find(Down(PhysicalKeyboardKey.keyA, LogicalKeyboardKey.keyA), keyboard) is null, "plain A does not select all");

        var fallback = new DoNothingIntent();
        var specific = new SelectAllTextIntent(SelectionChangedCause.keyboard);
        var first = new PasteTextIntent(SelectionChangedCause.keyboard);
        manager.shortcuts = new DartMap<ShortcutActivator, Intent>
        {
            [new AnyKeyActivator()] = fallback,
            [new SingleActivator(LogicalKeyboardKey.keyA)] = specific,
            [new SingleActivator(LogicalKeyboardKey.keyA)] = first,
        };
        Require(ReferenceEquals(manager._find(Down(PhysicalKeyboardKey.keyA, LogicalKeyboardKey.keyA), keyboard), specific), "indexed candidates precede wildcard candidates and preserve registration order");
        Require(ReferenceEquals(manager._find(Down(PhysicalKeyboardKey.keyV, LogicalKeyboardKey.keyV), keyboard), fallback), "null-trigger activators remain reachable");
        manager.shortcuts = new DartMap<ShortcutActivator, Intent> { [new SingleActivator(LogicalKeyboardKey.keyA)] = first };
        Require(ReferenceEquals(manager._find(Down(PhysicalKeyboardKey.keyA, LogicalKeyboardKey.keyA), keyboard), first), "changing shortcuts invalidates the index");
        VerifyDispatch();
        Console.WriteLine("PASS keyboard shortcut matching and typed action dispatch");
    }

    private static void VerifyReadingOrderTraversal()
    {
        var left = new GeometryFocusNode(0, 0);
        var right = new GeometryFocusNode(50, 0);
        var nextRow = new GeometryFocusNode(0, 40);
        Require(ReadingOrderTraversalPolicy.sort([nextRow, right, left]).SequenceEqual([left, right, nextRow]),
            "reading-order traversal populates and sorts candidates by geometry");
        var scope = new FocusScopeNode();
        foreach (var node in new[] { nextRow, right, left }) { node._parent = scope; scope._children.Add(node); }
        scope._focusedChildren.Add(right);
        FocusNode? requested = null;
        var policy = new ReadingOrderTraversalPolicy((node, alignmentPolicy, alignment, duration, curve) => requested = node);
        Require(policy.previous(right) && ReferenceEquals(requested, left), "Shift+Tab selects the previous node with no enclosing traversal group");
        Require(policy.next(right) && ReferenceEquals(requested, nextRow), "Tab selects the next node with no enclosing traversal group");
    }

    private static void VerifyPlatformKeyMaps()
    {
        foreach (var (name, physical, logical) in new[]
        {
            ("ControlLeft", PhysicalKeyboardKey.controlLeft, LogicalKeyboardKey.controlLeft),
            ("ControlRight", PhysicalKeyboardKey.controlRight, LogicalKeyboardKey.controlRight),
            ("ShiftLeft", PhysicalKeyboardKey.shiftLeft, LogicalKeyboardKey.shiftLeft),
            ("ShiftRight", PhysicalKeyboardKey.shiftRight, LogicalKeyboardKey.shiftRight),
            ("AltLeft", PhysicalKeyboardKey.altLeft, LogicalKeyboardKey.altLeft),
            ("AltRight", PhysicalKeyboardKey.altRight, LogicalKeyboardKey.altRight),
            ("MetaLeft", PhysicalKeyboardKey.metaLeft, LogicalKeyboardKey.metaLeft),
            ("MetaRight", PhysicalKeyboardKey.metaRight, LogicalKeyboardKey.metaRight),
        })
        {
            Require(Doroti.Host.Web.BrowserKeyMap.Physical(name) == physical.usbHidUsage &&
                Doroti.Host.Web.BrowserKeyMap.Logical(name, name) == logical.keyId, $"Web {name} matches framework identity");
            Require(Doroti.Host.Maui.MauiKeyMap.Logical(name, physical.usbHidUsage) == logical.keyId &&
                Doroti.Host.Maui.MauiKeyMap.Logical("", physical.usbHidUsage) == logical.keyId, $"MAUI {name} name/HID matches framework identity");
        }
        Require(Doroti.Host.Qt.QtKeyMap.Logical(0x01000021, "") == LogicalKeyboardKey.controlLeft.keyId &&
            Doroti.Host.Qt.QtKeyMap.Logical(0x01000020, "") == LogicalKeyboardKey.shiftLeft.keyId, "Qt Ctrl/Shift match framework identity");
        foreach (var letter in "ACXVZ")
        {
            Require(Doroti.Host.Qt.QtKeyMap.Logical(letter, ((char)(letter - 'A' + 1)).ToString()) == char.ToLowerInvariant(letter) &&
                Doroti.Host.Qt.QtKeyMap.Logical(letter, "") == char.ToLowerInvariant(letter), $"Qt Ctrl+{letter} control character and release preserve letter identity");
        }
    }

    private static void VerifyDispatch()
    {
        var servicesField = typeof(ServicesBinding).GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic)!;
        var previousServices = servicesField.GetValue(null);
        var previousWidgets = WidgetsBinding._instance;
        var binding = (ShortcutBinding)RuntimeHelpers.GetUninitializedObject(typeof(ShortcutBinding));
        binding.TestKeyboard = new HardwareKeyboard();
        var focusManager = new FocusManager();
        binding._buildOwner = new BuildOwner(focusManager: focusManager);
        servicesField.SetValue(null, binding);
        WidgetsBinding._instance = binding;
        try
        {
            var action = new SelectionAction();
            var dispatcher = new CountingDispatcher();
            var context = new ActionContext(action, dispatcher);
            focusManager._primaryFocus = new FocusNode { _context = context };
            var prioritized = new PrioritizedAction();
            var prioritizedIntent = new PrioritizedIntents([new SelectAllTextIntent(SelectionChangedCause.keyboard)]);
            Require(prioritized.isEnabled(prioritizedIntent, context) && prioritized.invoke(prioritizedIntent, context) is null && action.Calls == 1,
                "default prioritized action invokes the first enabled candidate without a trailing exception");
            action.Calls = 0;
            var requestCalls = 0;
            var request = new RequestFocusIntent(focusManager._primaryFocus,
                (node, policy, alignment, duration, curve) => requestCalls++);
            Require(new RequestFocusAction().invoke(request, context) is null && requestCalls == 1,
                "default focus request action completes once without a trailing exception");
            var manager = new ShortcutManager(new DartMap<ShortcutActivator, Intent>
            {
                [new SingleActivator(LogicalKeyboardKey.keyA)] = new SelectAllTextIntent(SelectionChangedCause.keyboard),
            });
            var key = Down(PhysicalKeyboardKey.keyA, LogicalKeyboardKey.keyA);
            Require(manager.handleKeypress(context, key) == KeyEventResult.handled && action.Calls == 1 && dispatcher.Calls == 1,
                "ShortcutManager finds and invokes Action<SelectAllTextIntent> through the scoped dispatcher exactly once");
            action.Result = KeyEventResult.ignored;
            Require(manager.handleKeypress(context, key) == KeyEventResult.ignored && action.Calls == 2, "custom action key-result override is retained");
            action.Enabled = false;
            Require(manager.handleKeypress(context, key) == KeyEventResult.ignored && action.Calls == 2, "disabled actions do not run");
            var modal = new ShortcutManager(manager.shortcuts, modal: true);
            Require(modal.handleKeypress(context, key) == KeyEventResult.skipRemainingHandlers, "disabled modal shortcuts stop framework propagation");
            Require(manager.handleKeypress(context, Down(PhysicalKeyboardKey.keyV, LogicalKeyboardKey.keyV)) == KeyEventResult.ignored, "unmatched key does not look up a null intent");
            focusManager._primaryFocus = null;
            Require(manager.handleKeypress(context, key) == KeyEventResult.ignored, "matched shortcut with no focus is safe");
            focusManager._primaryFocus = new FocusNode { _context = new ActionContext((IIntentAction?)null, dispatcher) };
            Require(manager.handleKeypress(context, key) == KeyEventResult.ignored, "matched shortcut with no action is safe");
            VerifyEditingActions(binding.TestKeyboard, focusManager, 0x1d);
            VerifyEditingActions(binding.TestKeyboard, focusManager, 0x11d);
            using var clipboardDispatcher = new Doroti.Ui.PlatformDispatcher();
            using var clipboardScope = clipboardDispatcher.EnterScope();
            var clipboardHost = new ClipboardFixtureHost();
            clipboardDispatcher.RegisterView(1, new Doroti.Ui.DorotiViewCapabilities("clipboard-fixture")
                .Register<Doroti.Ui.IViewHostCapability>(Doroti.Ui.DorotiCapabilityIds.ViewLifecycleMetrics, clipboardHost)
                .Register<Doroti.Ui.IPlatformServicesHostCapability>(Doroti.Ui.DorotiCapabilityIds.PlatformServices, clipboardHost));
            Require(!Clipboard.hasStrings().GetAwaiter().GetResult(), "absent host clipboard has no strings");
            Require(clipboardHost.StatusQueries == 1 && clipboardHost.Reads == 0,
                "clipboard availability uses the host status API without reading content");
            VerifyEditingActions(binding.TestKeyboard, focusManager, 0x1d, clipboardHost);
            Require(clipboardHost.Writes == 2, "real copy/cut write to the registered host once each");
            Require(Clipboard.getData("image/png").GetAwaiter().GetResult() is null, "unsupported clipboard format is absent");
            VerifyWindowsModifiers();
            VerifyPlatformShortcutIngress(binding.TestKeyboard, focusManager);
        }
        finally
        {
            servicesField.SetValue(null, previousServices);
            WidgetsBinding._instance = previousWidgets;
        }
    }

    private static void VerifyPlatformShortcutIngress(HardwareKeyboard keyboard, FocusManager focusManager)
    {
        using var environment = Doroti.Ui.PlatformEnvironmentContext.Enter(new Doroti.Ui.PlatformConfiguration(
            locales: [], platformBrightness: Doroti.Ui.Brightness.light, alwaysUse24HourFormat: false,
            nativeSpellCheckServiceDefined: false, operatingSystem: Doroti.Ui.HostOperatingSystem.windows));
        var routes = new (string Name, long ModifierPhysical, long ModifierLogical, bool Meta, Func<char, (long Physical, long Logical)> Letter)[]
        {
            ("Web", Doroti.Host.Web.BrowserKeyMap.Physical("ControlLeft"), Doroti.Host.Web.BrowserKeyMap.Logical("ControlLeft", "Control"), false,
                c => (Doroti.Host.Web.BrowserKeyMap.Physical($"Key{c}"), Doroti.Host.Web.BrowserKeyMap.Logical($"Key{c}", c.ToString()))),
            ("Qt", Doroti.Host.Qt.QtKeyMap.Physical(37, 0x01000021), Doroti.Host.Qt.QtKeyMap.Logical(0x01000021, ""), false,
                c => (Doroti.Host.Qt.QtKeyMap.Physical(0, c), Doroti.Host.Qt.QtKeyMap.Logical(c, ((char)(c - 'A' + 1)).ToString()))),
            ("Android", Doroti.Host.Maui.MauiKeyMap.AndroidPhysical(113), Doroti.Host.Maui.MauiKeyMap.Logical("CtrlLeft", Doroti.Host.Maui.MauiKeyMap.AndroidPhysical(113)), false,
                c => (Doroti.Host.Maui.MauiKeyMap.AndroidPhysical(c - 'A' + 29), Doroti.Host.Maui.MauiKeyMap.Logical(((char)(c - 'A' + 1)).ToString(), Doroti.Host.Maui.MauiKeyMap.AndroidPhysical(c - 'A' + 29)))),
            ("UIKit", 0x700e3, Doroti.Host.Maui.MauiKeyMap.Logical("KeyboardLeftGUI", 0x700e3), true,
                c => (0x70004 + c - 'A', Doroti.Host.Maui.MauiKeyMap.Logical(c.ToString(), 0x70004 + c - 'A'))),
            ("AppKit", Doroti.Host.Maui.MacOSKeyMap.Physical(0x37), Doroti.Host.Maui.MauiKeyMap.Logical("", Doroti.Host.Maui.MacOSKeyMap.Physical(0x37)), true,
                c => { var scan = c switch { 'A' => 0x00, 'C' => 0x08, 'X' => 0x07, 'V' => 0x09, _ => throw new InvalidOperationException() }; var p = Doroti.Host.Maui.MacOSKeyMap.Physical((ushort)scan); return (p, Doroti.Host.Maui.MauiKeyMap.Logical(c.ToString(), p)); }),
        };
        foreach (var route in routes)
        {
            var controller = TextEditingController.CreateFromValue(new TextEditingValue(text: "route 한글", selection: TextSelection.CreateCollapsed(2)));
            var state = new EditingState { _widget = new EditableText(controller: controller, focusNode: new FocusNode(),
                style: new Doroti.Framework.Painting.TextStyle(), cursorColor: new Doroti.Ui.Color(0xff000000),
                backgroundCursorColor: new Doroti.Ui.Color(0xff808080), autofillHints: []) };
            var context = new ActionContext(state._actions, new ActionDispatcher());
            var manager = new ShortcutManager(route.Meta ? DefaultTextEditingShortcuts._macShortcuts : DefaultTextEditingShortcuts._linuxShortcuts);
            focusManager._primaryFocus = new FocusNode { _context = context, onKeyEvent = (node, key) => manager.handleKeypress(node.context!, key) };
            var ingress = new KeyEventManager(keyboard, new RawKeyboard()) { keyMessageHandler = focusManager._highlightManager.handleKeyMessage };
            bool Send(long physical, long logical, Doroti.Ui.KeyEventType type) => ingress.handleKeyData(new Doroti.Ui.KeyData(1, TimeSpan.Zero, type, physical, logical, false));
            Send(route.ModifierPhysical, route.ModifierLogical, Doroti.Ui.KeyEventType.down);
            foreach (var letter in "ACXV")
            {
                var key = route.Letter(letter);
                Require(Send(key.Physical, key.Logical, Doroti.Ui.KeyEventType.down), $"{route.Name}: shortcut {letter} is handled");
                Require(!Send(key.Physical, key.Logical, Doroti.Ui.KeyEventType.up), $"{route.Name}: release does not edit twice");
                if (letter == 'A') Require(controller.selection.start == 0 && controller.selection.end == controller.text.Length, $"{route.Name}: select all");
                if (letter == 'C') Require(state.Clipboard == "route 한글" && state.Copies == 1, $"{route.Name}: copy once");
                if (letter == 'X') Require(controller.text == "" && state.Cuts == 1, $"{route.Name}: cut once");
                if (letter == 'V') Require(controller.text == "route 한글" && state.Pastes == 1, $"{route.Name}: paste once");
            }
            Send(route.ModifierPhysical, route.ModifierLogical, Doroti.Ui.KeyEventType.up);
            Require(keyboard.logicalKeysPressed.Count == 0, $"{route.Name}: no stuck keys");
            controller.dispose();
            state.widget.focusNode.dispose();
        }
        foreach (var (android, mac, physical, logical) in new[]
        {
            (113, 0x3b, PhysicalKeyboardKey.controlLeft, LogicalKeyboardKey.controlLeft),
            (114, 0x3e, PhysicalKeyboardKey.controlRight, LogicalKeyboardKey.controlRight),
            (59, 0x38, PhysicalKeyboardKey.shiftLeft, LogicalKeyboardKey.shiftLeft),
            (60, 0x3c, PhysicalKeyboardKey.shiftRight, LogicalKeyboardKey.shiftRight),
            (57, 0x3a, PhysicalKeyboardKey.altLeft, LogicalKeyboardKey.altLeft),
            (58, 0x3d, PhysicalKeyboardKey.altRight, LogicalKeyboardKey.altRight),
            (117, 0x37, PhysicalKeyboardKey.metaLeft, LogicalKeyboardKey.metaLeft),
            (118, 0x36, PhysicalKeyboardKey.metaRight, LogicalKeyboardKey.metaRight),
        })
        {
            Require(Doroti.Host.Maui.MauiKeyMap.AndroidPhysical(android) == physical.usbHidUsage &&
                Doroti.Host.Maui.MacOSKeyMap.Physical((ushort)mac) == physical.usbHidUsage, "Android/AppKit modifier physical identities");
            Require(Doroti.Host.Maui.MacOSKeyMap.ModifierMask((ushort)mac) != 0 &&
                Doroti.Host.Maui.MauiKeyMap.Logical("", physical.usbHidUsage) == logical.keyId, "AppKit FlagsChanged key identity");
        }
        Require(Doroti.Host.Maui.MauiKeyMap.Logical("\uf702", 0x70050) == LogicalKeyboardKey.arrowLeft.keyId,
            "Apple private-use arrow character remains a navigation key");
    }

    private static void VerifyEditingActions(HardwareKeyboard keyboard, FocusManager focusManager, int controlScan, ClipboardFixtureHost? host = null)
    {
        using var environment = Doroti.Ui.PlatformEnvironmentContext.Enter(new Doroti.Ui.PlatformConfiguration(
            locales: [], platformBrightness: Doroti.Ui.Brightness.light, alwaysUse24HourFormat: false,
            nativeSpellCheckServiceDefined: false, operatingSystem: Doroti.Ui.HostOperatingSystem.windows));
        var controller = TextEditingController.CreateFromValue(new TextEditingValue(text: "Hello 한글", selection: TextSelection.CreateCollapsed(2)));
        var state = new EditingState { Host = host, _widget = new EditableText(controller: controller, focusNode: new FocusNode(),
            style: new Doroti.Framework.Painting.TextStyle(), cursorColor: new Doroti.Ui.Color(0xff000000),
            backgroundCursorColor: new Doroti.Ui.Color(0xff808080), autofillHints: [],
            selectionControls: Doroti.Framework.Material.Desktop_text_selectionLibrary.desktopTextSelectionHandleControls) };
        // Use EditableText's real action map, including overridable actions and
        // UpdateSelectionIntent. Only the clipboard endpoint and rendering side
        // effects are replaced; selection/text changes use the real controller.
        var context = new ActionContext(state._actions, new ActionDispatcher());
        var manager = new ShortcutManager(DefaultTextEditingShortcuts._windowsShortcuts);
        var shortcutFocus = new FocusNode { _context = context, onKeyEvent = (node, key) => manager.handleKeypress(node.context!, key) };
        focusManager._primaryFocus = new FocusNode { _context = context, _parent = shortcutFocus };
        focusManager._highlightManager._lastInteractionRequiresTraditionalHighlights = false;
        var ingress = new KeyEventManager(keyboard, new RawKeyboard())
        {
            keyMessageHandler = focusManager._highlightManager.handleKeyMessage,
        };
        bool NativeKey(int virtualKey, int scanCode, Doroti.Ui.KeyEventType type, string character = "") =>
            ingress.handleKeyData(new Doroti.Ui.KeyData(1, TimeSpan.Zero, type,
                WindowsKeyMap.Physical(scanCode, virtualKey), WindowsKeyMap.Logical(scanCode, virtualKey, character),
                false, type == Doroti.Ui.KeyEventType.up || character.Length == 0 ? null : character));
        void Press(int virtualKey, int scanCode)
        {
            // ToUnicodeEx emits a control character for Ctrl+letter, not the
            // printable letter used by the old framework-only fixture.
            Require(NativeKey(virtualKey, scanCode, Doroti.Ui.KeyEventType.down, ((char)(virtualKey - 'A' + 1)).ToString()),
                $"Win32 Ctrl+{(char)virtualKey} reaches the focused editing action");
            Require(!NativeKey(virtualKey, scanCode, Doroti.Ui.KeyEventType.up), "native key release does not invoke the action");
        }
        NativeKey(0x11, controlScan, Doroti.Ui.KeyEventType.down);
        Press('A', 0x1e);
        Require(controller.selection.start == 0 && controller.selection.end == controller.text.Length, "Ctrl+A selects the full controller text");
        Press('C', 0x2e);
        Require(state.Clipboard == "Hello 한글" && state.Copies == 1, "Ctrl+C reaches the clipboard endpoint once");
        Press('X', 0x2d);
        Require(controller.text == "" && state.Cuts == 1, "Ctrl+X cuts the selected text once");
        if (host is not null)
        {
            Require(state.clipboardStatus.value == ClipboardStatus.pasteable &&
                state.contextMenuButtonItems is [{ type: ContextMenuButtonType.paste, onPressed: not null }],
                "empty field menu obtains enabled paste from the real clipboard status query");
        }
        Press('V', 0x2f);
        Require(controller.text == "Hello 한글" && state.Pastes == 1, "Ctrl+V replaces the selection once through EditableText paste logic");
        Press('A', 0x1e);
        state.Clipboard = "replacement";
        Press('V', 0x2f);
        Require(controller.text == "replacement" && controller.selection.isCollapsed && controller.selection.extentOffset == 11,
            "select all then paste replaces text and collapses the caret");
        NativeKey(0x11, controlScan, Doroti.Ui.KeyEventType.up);
        Require(!keyboard.isControlPressed && !keyboard.isShiftPressed, "native modifier release clears the correct state");
        if (host is not null)
        {
            controller.value = new TextEditingValue(text: "", selection: TextSelection.CreateCollapsed(0));
            host.Text = "";
            state.clipboardStatus.update().GetAwaiter().GetResult();
            Require(state.contextMenuButtonItems is [{ type: ContextMenuButtonType.paste, onPressed: null }],
                "empty OS clipboard retains a disabled paste menu item on an empty desktop field");
            host.Text = "external 한글";
            state.clipboardStatus.update().GetAwaiter().GetResult();
            Require(state.contextMenuButtonItems is [{ type: ContextMenuButtonType.paste, onPressed: not null }],
                "clipboard refresh enables paste after another app supplies text");
        }
        state.widget.focusNode.dispose();
        controller.dispose();
    }

    private static void VerifyWindowsModifiers()
    {
        foreach (var (virtualKey, scan, physical, logical) in new[]
        {
            (0x11, 0x1d, PhysicalKeyboardKey.controlLeft, LogicalKeyboardKey.controlLeft),
            (0x11, 0x11d, PhysicalKeyboardKey.controlRight, LogicalKeyboardKey.controlRight),
            (0x10, 0x2a, PhysicalKeyboardKey.shiftLeft, LogicalKeyboardKey.shiftLeft),
            (0x10, 0x36, PhysicalKeyboardKey.shiftRight, LogicalKeyboardKey.shiftRight),
            (0x12, 0x38, PhysicalKeyboardKey.altLeft, LogicalKeyboardKey.altLeft),
            (0x12, 0x138, PhysicalKeyboardKey.altRight, LogicalKeyboardKey.altRight),
            (0x5b, 0x15b, PhysicalKeyboardKey.metaLeft, LogicalKeyboardKey.metaLeft),
            (0x5c, 0x15c, PhysicalKeyboardKey.metaRight, LogicalKeyboardKey.metaRight),
        })
        {
            Require(WindowsKeyMap.Physical(scan, virtualKey) == physical.usbHidUsage &&
                WindowsKeyMap.Logical(scan, virtualKey, "") == logical.keyId,
                $"Win32 modifier identity matches framework {logical.keyLabel}");
        }
    }

    private static KeyDownEvent Down(PhysicalKeyboardKey physical, LogicalKeyboardKey logical) => new(physical, logical, timeStamp: Duration.zero);
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private sealed class AnyKeyActivator : ShortcutActivator
    {
        public override IEnumerable<LogicalKeyboardKey>? triggers => null;
        public override bool accepts(KeyEvent @event, HardwareKeyboard state) => @event is KeyDownEvent;
        public override string debugDescribeKeys() => "any";
    }
    private sealed class ShortcutBinding : WidgetsFlutterBinding
    {
        public HardwareKeyboard TestKeyboard = null!;
        public override HardwareKeyboard keyboard => TestKeyboard;
        public override BinaryMessenger defaultBinaryMessenger => new UnimplementedPlatformMessenger();
    }
    private sealed class UnimplementedPlatformMessenger : BinaryMessenger
    {
        public Future<Doroti.Runtime.ByteData?> send(string channel, Doroti.Runtime.ByteData? message) => Future<Doroti.Runtime.ByteData?>.value(null);
        public Future handlePlatformMessage(string channel, Doroti.Runtime.ByteData? data, System.Action<Doroti.Runtime.ByteData?>? callback)
        { callback?.Invoke(null); return Future.value(); }
        public void setMessageHandler(string channel, Func<Doroti.Runtime.ByteData?, Future<Doroti.Runtime.ByteData?>?>? handler) { }
    }
    private sealed class SelectionAction : Doroti.Framework.Widgets.Action<SelectAllTextIntent>
    {
        public int Calls;
        public bool Enabled = true;
        public KeyEventResult Result = KeyEventResult.handled;
        public override bool isEnabled(SelectAllTextIntent intent, BuildContext? context = null) => Enabled;
        public override object? invoke(SelectAllTextIntent intent, BuildContext? context = null) { Calls++; return "selected"; }
        public override KeyEventResult toKeyEventResult(SelectAllTextIntent intent, object? invokeResult)
        {
            Require(Equals(invokeResult, "selected"), "action receives its invocation result");
            return Result;
        }
    }
    private sealed class CountingDispatcher : ActionDispatcher
    {
        public int Calls;
        public override (bool, object?) invokeActionIfEnabled(dynamic action, Intent intent, BuildContext? context = null)
        {
            Calls++;
            return base.invokeActionIfEnabled((object)action, intent, context);
        }
    }
    private sealed class ActionContext : StatelessElement
    {
        private readonly InheritedElement _scope;
        private readonly bool _hasAction;
        public ActionContext(IIntentAction? action, ActionDispatcher dispatcher) : base(new Builder(builder: _ => SizedBox.CreateShrink()))
        {
            _hasAction = action is not null;
            var actions = new DartMap<Type, IIntentAction>();
            if (action is not null) actions[typeof(SelectAllTextIntent)] = action;
            _scope = new ActionScope(new _ActionsScope__actions(actions: actions, dispatcher: dispatcher, rebuildKey: new object(), child: SizedBox.CreateShrink()));
        }
        public ActionContext(DartMap<Type, dynamic> source, ActionDispatcher dispatcher) : base(new Builder(builder: _ => SizedBox.CreateShrink()))
        {
            _hasAction = true;
            var actions = new DartMap<Type, IIntentAction>();
            foreach (var pair in source) actions[pair.Key] = (IIntentAction)pair.Value;
            _scope = new ActionScope(new _ActionsScope__actions(actions: actions, dispatcher: dispatcher, rebuildKey: new object(), child: SizedBox.CreateShrink()));
        }
        public override InheritedElement? getElementForInheritedWidgetOfExactType<T>() => _hasAction && typeof(T) == typeof(_ActionsScope__actions) ? _scope : null;
        public override InheritedWidget dependOnInheritedElement(InheritedElement ancestor, object? aspect = null) => (InheritedWidget)ancestor.widget;
        public override T? dependOnInheritedWidgetOfExactType<T>(object? aspect = null) where T : default => _scope.widget is T value ? value : default;
    }
    private sealed class ActionScope(_ActionsScope__actions widget) : InheritedElement(widget)
    {
        public override InheritedElement? getElementForInheritedWidgetOfExactType<T>() => typeof(T) == typeof(_ActionsScope__actions) ? this : null;
    }
    private sealed class EditingState : EditableTextState
    {
        private string _clipboard = "";
        public ClipboardFixtureHost? Host;
        public string Clipboard { get => Host is null ? _clipboard : Host.Text ?? ""; set { if (Host is null) _clipboard = value; else Host.Text = value; } }
        public int Copies, Cuts, Pastes;
        public override BuildContext context => new ActionContext((IIntentAction?)null, new ActionDispatcher());
        public override TextEditingValue textEditingValue => widget.controller.value;
        public override void userUpdateTextEditingValue(TextEditingValue value, SelectionChangedCause cause) => widget.controller.value = value;
        public override void bringIntoView(Doroti.Ui.TextPosition position) { }
        public override void copySelection(SelectionChangedCause cause) { Copies++; if (Host is not null) base.copySelection(cause); else Clipboard = textEditingValue.selection.textInside(textEditingValue.text); }
        public override void cutSelection(SelectionChangedCause cause)
        {
            Cuts++;
            if (Host is not null) { base.cutSelection(cause); return; }
            Clipboard = textEditingValue.selection.textInside(textEditingValue.text);
            widget.controller.value = textEditingValue.replaced(textEditingValue.selection, "");
        }
        public override Future pasteText(SelectionChangedCause cause) { Pastes++; if (Host is not null) return base.pasteText(cause); _pasteText(cause, Clipboard); return Future.value(); }
    }
    private sealed class GeometryFocusNode(double left, double top) : FocusNode
    {
        private readonly BuildContext _geometryContext = new GeometryContext();
        public override BuildContext context => _geometryContext;
        public override Doroti.Ui.Rect rect => Doroti.Ui.Rect.fromLTWH(left, top, 30, 20);
    }
    private sealed class GeometryContext() : StatelessElement(new Builder(builder: _ => SizedBox.CreateShrink()))
    {
        private readonly Directionality _directionality = new(textDirection: Doroti.Ui.TextDirection.ltr, child: SizedBox.CreateShrink());
        public override T? getInheritedWidgetOfExactType<T>() where T : default => _directionality is T value ? value : default;
        public override InheritedElement? getElementForInheritedWidgetOfExactType<T>() => null;
    }
}
