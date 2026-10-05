// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/platform_menu_bar.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public static partial class Platform_menu_barLibrary
{
    internal static string _kMenuSetMethod = "Menu.setMenus";
}

public static partial class Platform_menu_barLibrary
{
    internal static string _kMenuSelectedCallbackMethod = "Menu.selectedCallback";
}

public static partial class Platform_menu_barLibrary
{
    internal static string _kMenuItemOpenedMethod = "Menu.opened";
}

public static partial class Platform_menu_barLibrary
{
    internal static string _kMenuItemClosedMethod = "Menu.closed";
}

public static partial class Platform_menu_barLibrary
{
    internal static string _kIdKey = "id";
}

public static partial class Platform_menu_barLibrary
{
    internal static string _kLabelKey = "label";
}

public static partial class Platform_menu_barLibrary
{
    internal static string _kTooltipKey = "tooltip";
}

public static partial class Platform_menu_barLibrary
{
    internal static string _kEnabledKey = "enabled";
}

public static partial class Platform_menu_barLibrary
{
    internal static string _kChildrenKey = "children";
}

public static partial class Platform_menu_barLibrary
{
    internal static string _kIsDividerKey = "isDivider";
}

public static partial class Platform_menu_barLibrary
{
    internal static string _kPlatformDefaultMenuKey = "platformProvidedMenu";
}

public static partial class Platform_menu_barLibrary
{
    internal static string _kShortcutCharacter = "shortcutCharacter";
}

public static partial class Platform_menu_barLibrary
{
    internal static string _kShortcutTrigger = "shortcutTrigger";
}

public static partial class Platform_menu_barLibrary
{
    internal static string _kShortcutModifiers = "shortcutModifiers";
}

public class ShortcutSerialization
{
    internal virtual DartMap<string, object?> _internal { get; private set; } = default!;
    internal virtual LogicalKeyboardKey? _trigger { get; private set; }
    internal virtual string? _character { get; private set; }
    internal virtual bool? _alt { get; private set; }
    internal virtual bool? _control { get; private set; }
    internal virtual bool? _meta { get; private set; }
    internal virtual bool? _shift { get; private set; }
    internal static long _shortcutModifierAlt = 1L << (int)2L;
    internal static long _shortcutModifierControl = 1L << (int)3L;
    internal static long _shortcutModifierMeta = 1L << (int)0L;
    internal static long _shortcutModifierShift = 1L << (int)1L;

    public ShortcutSerialization(
        string character,
        bool alt = false,
        bool control = false,
        bool meta = false
    )
    {
        _character = character;
        _trigger = null;
        _alt = alt;
        _control = control;
        _meta = meta;
        _shift = null;
        _internal = new DartMap<string, object?>
        {
            [Platform_menu_barLibrary._kShortcutCharacter] = character,
            [Platform_menu_barLibrary._kShortcutModifiers] =
                (control ? _shortcutModifierControl : 0L)
                | (alt ? _shortcutModifierAlt : 0L)
                | (meta ? _shortcutModifierMeta : 0L),
        };
        System.Diagnostics.Debug.Assert(character is null || character.Length == 1L);
    }

    public static ShortcutSerialization CreateModifier(
        LogicalKeyboardKey trigger,
        bool alt = false,
        bool control = false,
        bool meta = false,
        bool shift = false
    )
    {
        var __instance = new ShortcutSerialization(default!, alt, control, meta);
        __instance._trigger = trigger;
        __instance._character = null;
        __instance._alt = alt;
        __instance._control = control;
        __instance._meta = meta;
        __instance._shift = shift;
        __instance._internal = new DartMap<string, object?>
        {
            [Platform_menu_barLibrary._kShortcutTrigger] = trigger.keyId,
            [Platform_menu_barLibrary._kShortcutModifiers] =
                (alt ? _shortcutModifierAlt : 0L)
                | (control ? _shortcutModifierControl : 0L)
                | (meta ? _shortcutModifierMeta : 0L)
                | (shift ? _shortcutModifierShift : 0L),
        };
        return __instance;
    }

    public virtual LogicalKeyboardKey? trigger => _trigger;
    public virtual string? character => _character;
    public virtual bool? alt => _alt;
    public virtual bool? control => _control;
    public virtual bool? meta => _meta;
    public virtual bool? shift => _shift;

    public virtual DartMap<string, object?> toChannelRepresentation() =>
        DartRuntimePrimitives.ConvertValue<DartMap<string, object?>>(_internal);
}

public interface MenuSerializableShortcut
{
    public ShortcutSerialization serializeForMenu();
}

public interface PlatformMenuDelegate
{
    public void setMenus(BuildContext context, List<PlatformMenuItem> topLevelMenus);
    public void clearMenus(BuildContext context);
    public bool debugLockDelegate(BuildContext context);
    public bool debugUnlockDelegate(BuildContext context);
}

public delegate long MenuItemSerializableIdGenerator(PlatformMenuItem item);

public class DefaultPlatformMenuDelegate : PlatformMenuDelegate
{
    private sealed class Owner(BuildContext context, Doroti.Ui.DorotiView view, long generation)
    {
        internal BuildContext Context = context;
        internal Doroti.Ui.DorotiView View = view;
        internal long Generation = generation;
        internal Dictionary<string, PlatformMenuItem> Items = [];
        internal Doroti.Ui.IPlatformMenuBarRegistration? Registration;
        internal bool Closed;
    }
    private readonly Dictionary<ulong, BuildContext> _locks = [];
    private readonly Dictionary<ulong, Owner> _owners = [];
    private long _generation;
    public virtual bool debugLockDelegate(BuildContext context)
    {
        var id = View.of(context).viewId;
        if (_locks.TryGetValue(id, out var previous) && !ReferenceEquals(previous, context)) return false;
        _locks[id] = context; return true;
    }
    public virtual bool debugUnlockDelegate(BuildContext context)
    {
        var id = View.of(context).viewId;
        if (!_locks.TryGetValue(id, out var previous) || !ReferenceEquals(previous, context)) return false;
        _locks.Remove(id); return true;
    }
    private static void Observe(Task work) => DartRuntimePrimitives.Ignore(Future.fromTask(work).then((_) => { }, onError: (error, stack) =>
        FlutterError.reportError(new FlutterErrorDetails(exception: error, stack: stack, library: "typed platform menu"))));
    public virtual void clearMenus(BuildContext context)
    {
        if (!_owners.Remove(View.of(context).viewId, out var previous)) return;
        previous.Closed = true;
        if (previous.Registration is { } registration) Observe(registration.DisposeAsync().AsTask());
    }
    public virtual void setMenus(BuildContext context, List<PlatformMenuItem> topLevelMenus)
    {
        ArgumentNullException.ThrowIfNull(topLevelMenus);
        clearMenus(context);
        if (topLevelMenus.Count == 0) return;
        var view = View.of(context);
        var window = WindowScope.of(context);
        if (window.ViewId != view.viewId || window.Closed) throw new InvalidOperationException("The menu bar owner is not this live view.");
        var capability = view.GetCapabilityOrDefault<Doroti.Ui.IPlatformMenuBarHostCapability>(Doroti.Ui.DorotiCapabilityIds.PlatformMenuBar)
            ?? throw new NotSupportedException("This provider does not support native menu bars.");
        var owner = new Owner(context, view, ++_generation);
        var serial = 0;
        IEnumerable<Doroti.Ui.PlatformMenuItem> Convert(IEnumerable<PlatformMenuItem> source)
        {
            foreach (var item in source)
            {
                if (item is PlatformMenuItemGroup group)
                {
                    yield return new("separator-" + ++serial, "", Separator: true);
                    foreach (var child in Convert(group.members)) yield return child;
                    continue;
                }
                var id = (++serial).ToString(System.Globalization.CultureInfo.InvariantCulture);
                owner.Items.Add(id, item);
                var shortcut = item.shortcut?.serializeForMenu();
                var modifiers = Doroti.Ui.PlatformMenuModifiers.None;
                if (shortcut?.shift == true) modifiers |= Doroti.Ui.PlatformMenuModifiers.Shift;
                if (shortcut?.control == true) modifiers |= Doroti.Ui.PlatformMenuModifiers.Control;
                if (shortcut?.alt == true) modifiers |= Doroti.Ui.PlatformMenuModifiers.Alt;
                if (shortcut?.meta == true) modifiers |= Doroti.Ui.PlatformMenuModifiers.Meta;
                yield return new(id, item.label,
                    Enabled: item is PlatformMenu menu ? menu.menus.Count != 0 : item.onSelected is not null || item.onSelectedIntent is not null,
                    Children: item is PlatformMenu nested ? Convert(nested.menus).ToArray() : null,
                    Shortcut: shortcut is null ? null : new(shortcut.character, shortcut.trigger?.keyId, modifiers),
                    PlatformRole: item is PlatformProvidedMenuItem provided ? provided.type.ToString() : null);
            }
        }
        var request = new Doroti.Ui.PlatformMenuBarRequest(window.Id, view.viewId, owner.Generation, Convert(topLevelMenus).ToArray());
        capability.Evaluate(request).RequireSupported();
        _owners.Add(view.viewId, owner);
        async Task Install()
        {
            var registration = await view.InvokeCapabilityAsync<Doroti.Ui.IPlatformMenuBarHostCapability, Doroti.Ui.IPlatformMenuBarRegistration>(
                Doroti.Ui.DorotiCapabilityIds.PlatformMenuBar, Doroti.Ui.DorotiUiInvocation.Managed("Widgets.MenuBar.set"),
                (host, token) => host.SetAsync(request, value =>
                {
                    if (owner.Closed || value.Window != request.Window || value.ViewId != view.viewId || value.Generation != owner.Generation ||
                        !_owners.TryGetValue(view.viewId, out var active) || !ReferenceEquals(active, owner)) return;
                    view.DispatchPlatformEvent(() =>
                    {
                        if (owner.Closed || !owner.Context.mounted || !owner.Items.TryGetValue(value.ItemId, out var item)) return;
                        switch (value.Kind)
                        {
                            case Doroti.Ui.PlatformMenuEventKind.Selected:
                                item.onSelected?.Invoke();
                                if (item.onSelectedIntent is { } intent) Actions.maybeInvoke(owner.Context, intent);
                                break;
                            case Doroti.Ui.PlatformMenuEventKind.Opened: item.onOpen?.Invoke(); break;
                            case Doroti.Ui.PlatformMenuEventKind.Closed: item.onClose?.Invoke(); break;
                        }
                    });
                }, token));
            if (owner.Closed) await registration.DisposeAsync();
            else owner.Registration = registration;
        }
        Observe(Install());
    }
}

public class PlatformMenuBar : StatefulWidget
{
    public virtual Widget? child { get; private set; }
    public virtual List<PlatformMenuItem> menus { get; private set; } = default!;

    public PlatformMenuBar(
        Key? key = null,
        List<PlatformMenuItem> menus = default!,
        Widget? child = null
    )
        : base(key: key)
    {
        this.menus = menus;
        this.child = child;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _PlatformMenuBarState__platform_menu_bar());

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        return menus.map((child) => ((Diagnosticable)child).toDiagnosticsNode()).ToList();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _PlatformMenuBarState__platform_menu_bar : State<PlatformMenuBar>
{
    public virtual List<PlatformMenuItem> descendants { get; set; } = new List<PlatformMenuItem>();

    public override void initState()
    {
        base.initState();
        if (!WidgetsBinding.instance.platformMenuDelegate.debugLockDelegate(context)) throw new InvalidOperationException("This view already has a native menu bar.");
        DartRuntimePrimitives.Assert(
            () => true,
            () =>
                (object?)
                    $"More than one active {typeof(PlatformMenuBar)} detected. Only one active "
                + "platform-rendered menu bar is allowed at a time."
        );
        WidgetsBinding.instance.platformMenuDelegate.clearMenus(context);
        _updateMenu();
    }

    public override void dispose()
    {
        if (!WidgetsBinding.instance.platformMenuDelegate.debugUnlockDelegate(context)) throw new InvalidOperationException("Native menu bar ownership mismatch.");
        DartRuntimePrimitives.Assert(
            () => true,
            () =>
                (object?)
                    $"tried to unlock the {typeof(DefaultPlatformMenuDelegate)} more than once with context {context}."
        );
        WidgetsBinding.instance.platformMenuDelegate.clearMenus(context);
        base.dispose();
    }

    public override void didUpdateWidget(PlatformMenuBar oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        var newDescendants = widget.menus;
        if (!CollectionsLibrary.listEquals(newDescendants, descendants))
        {
            descendants = newDescendants;
            _updateMenu();
        }
    }

    internal virtual void _updateMenu()
    {
        WidgetsBinding.instance.platformMenuDelegate.setMenus(context, widget.menus);
    }

    public override Widget build(BuildContext context)
    {
        return widget.child ?? new SizedBox();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class PlatformMenu : PlatformMenuItem
{
    private Action? __field_onOpen = default!;
    public override Action? onOpen
    {
        get => __field_onOpen;
    }
    private Action? __field_onClose = default!;
    public override Action? onClose
    {
        get => __field_onClose;
    }
    public virtual List<PlatformMenuItem> menus { get; private set; } = default!;

    public PlatformMenu(
        string label,
        string? tooltip = null,
        Action? onOpen = null,
        Action? onClose = null,
        List<PlatformMenuItem> menus = default!
    )
        : base(label: label, tooltip: tooltip)
    {
        __field_onOpen = onOpen;
        __field_onClose = onClose;
        this.menus = menus;
    }

    public override List<PlatformMenuItem> descendants => getDescendants(this);

    public static List<PlatformMenuItem> getDescendants(PlatformMenu item)
    {
        return new List<PlatformMenuItem>();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override IEnumerable<DartMap<string, object?>> toChannelRepresentation(
        PlatformMenuDelegate @delegate,
        Func<PlatformMenuItem, long> getId
    )
    {
        return new List<DartMap<string, object?>> { serialize(this, @delegate, getId) };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static DartMap<string, object?> serialize(
        PlatformMenu item,
        PlatformMenuDelegate @delegate,
        Func<PlatformMenuItem, long> getId
    )
    {
        var result = new List<DartMap<string, object?>>();
        foreach (PlatformMenuItem childItem in item.menus)
        {
            result.AddRange(childItem.toChannelRepresentation(@delegate, getId: getId));
        }
        DartMap<string, object?>? previousItem = default!;
        result.removeWhere(
            (item) =>
            {
                if (
                    (previousItem is null)
                    && Equals(item.GetValueOrDefault(Platform_menu_barLibrary._kIsDividerKey), true)
                )
                {
                    return true;
                }
                if (
                    (previousItem is not null)
                    && Equals(
                        previousItem!.GetValueOrDefault(Platform_menu_barLibrary._kIsDividerKey),
                        true
                    )
                    && Equals(item.GetValueOrDefault(Platform_menu_barLibrary._kIsDividerKey), true)
                )
                {
                    return true;
                }
                previousItem = item.cast<string, object?>();
                return false;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        if (
            result.LastOrDefault() is var __match22940
            && DartPatternRuntime.IsMap(__match22940)
            && DartPatternRuntime.TryGetMapValue(
                __match22940,
                Platform_menu_barLibrary._kIsDividerKey,
                out var __entry22940_0
            )
            && __entry22940_0 is true
        )
        {
            result.removeLast();
        }
        return new DartMap<string, object?>
        {
            [Platform_menu_barLibrary._kIdKey] = getId(item),
            [Platform_menu_barLibrary._kLabelKey] = item.label,
            [Platform_menu_barLibrary._kEnabledKey] = Enumerable.Any(item.menus),
            [Platform_menu_barLibrary._kChildrenKey] = result,
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual List<DiagnosticsNode> debugDescribeChildren()
    {
        return menus.map((child) => ((Diagnosticable)child).toDiagnosticsNode()).ToList();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(new StringProperty("label", label));
        properties.add(
            new FlagProperty("enabled", value: Enumerable.Any(menus), ifFalse: "DISABLED")
        );
    }
}

public class PlatformMenuItemGroup : PlatformMenuItem
{
    private List<PlatformMenuItem> __field_members = default!;
    public override List<PlatformMenuItem> members
    {
        get => __field_members;
    }

    public PlatformMenuItemGroup(List<PlatformMenuItem> members)
        : base(label: "")
    {
        __field_members = members;
    }

    public override IEnumerable<DartMap<string, object?>> toChannelRepresentation(
        PlatformMenuDelegate @delegate,
        Func<PlatformMenuItem, long> getId
    )
    {
        DartRuntimePrimitives.Assert(
            () => Enumerable.Any(members),
            () => (object?)"There must be at least one member in a PlatformMenuItemGroup"
        );
        return serialize(this, @delegate, getId: getId);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static new IEnumerable<DartMap<string, object?>> serialize(
        PlatformMenuItem group,
        PlatformMenuDelegate @delegate,
        Func<PlatformMenuItem, long> getId
    )
    {
        return new List<DartMap<string, object?>>
        {
            new DartMap<string, object?>
            {
                [Platform_menu_barLibrary._kIdKey] = getId(group),
                [Platform_menu_barLibrary._kIsDividerKey] = true,
            },
            new DartMap<string, object?>
            {
                [Platform_menu_barLibrary._kIdKey] = getId(group),
                [Platform_menu_barLibrary._kIsDividerKey] = true,
            },
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new IterableProperty<PlatformMenuItem>("members", members.Cast<PlatformMenuItem>())
        );
    }
}

public class PlatformMenuItem : Diagnosticable
{
    public virtual string label { get; private set; } = default!;
    public virtual string? tooltip { get; private set; }
    public virtual MenuSerializableShortcut? shortcut { get; private set; }
    public virtual Action? onSelected { get; private set; }
    public virtual Intent? onSelectedIntent { get; private set; }

    public PlatformMenuItem(
        string label,
        string? tooltip = null,
        MenuSerializableShortcut? shortcut = null,
        Action? onSelected = null,
        Intent? onSelectedIntent = null
    )
    {
        this.label = label;
        this.tooltip = tooltip;
        this.shortcut = shortcut;
        this.onSelected = onSelected;
        this.onSelectedIntent = onSelectedIntent;
        System.Diagnostics.Debug.Assert((onSelected is null) || (onSelectedIntent is null));
    }

    public virtual Action? onOpen => DartRuntimePrimitives.ConvertValue<Action>(null);
    public virtual Action? onClose => DartRuntimePrimitives.ConvertValue<Action>(null);
    public virtual List<PlatformMenuItem> descendants => new List<PlatformMenuItem>();
    public virtual List<PlatformMenuItem> members => new List<PlatformMenuItem>();

    public virtual IEnumerable<DartMap<string, object?>> toChannelRepresentation(
        PlatformMenuDelegate @delegate,
        Func<PlatformMenuItem, long> getId
    )
    {
        return new List<DartMap<string, object?>> { serialize(this, @delegate, getId) };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static DartMap<string, object?> serialize(
        PlatformMenuItem item,
        PlatformMenuDelegate @delegate,
        Func<PlatformMenuItem, long> getId
    )
    {
        MenuSerializableShortcut? shortcutLocal = item.shortcut;
        return new DartMap<string, object?>
        {
            [Platform_menu_barLibrary._kIdKey] = getId(item),
            [Platform_menu_barLibrary._kLabelKey] = item.label,
            [Platform_menu_barLibrary._kEnabledKey] =
                (item.onSelected is not null) || (item.onSelectedIntent is not null),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual string toStringShort() =>
        $"{DiagnosticsLibrary.describeIdentity(this)}({label})";

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(new StringProperty("label", label));
        properties.add(new StringProperty("tooltip", tooltip, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<MenuSerializableShortcut?>(
                "shortcut",
                shortcut,
                defaultValue: null
            )
        );
        properties.add(
            new FlagProperty("enabled", value: onSelected is not null, ifFalse: "DISABLED")
        );
    }

    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine)
                .toDiagnosticsNode()
                .toStringDeep(minLevel: minLevel);
            return true;
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(
        string? name = null,
        DiagnosticsTreeStyle? style = null
    )
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class PlatformProvidedMenuItem : PlatformMenuItem
{
    public virtual PlatformProvidedMenuItemType type { get; private set; } = default!;
    public virtual bool enabled { get; private set; } = default!;

    public PlatformProvidedMenuItem(PlatformProvidedMenuItemType type, bool enabled = true)
        : base(label: "")
    {
        this.type = type;
        this.enabled = enabled;
    }

    public static bool hasMenu(PlatformProvidedMenuItemType menu)
    {
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.android:
            case TargetPlatform.iOS:
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
            {
                return false;
            }
            case TargetPlatform.macOS:
            {
                return new HashSet<PlatformProvidedMenuItemType>
                {
                    PlatformProvidedMenuItemType.about,
                    PlatformProvidedMenuItemType.quit,
                    PlatformProvidedMenuItemType.servicesSubmenu,
                    PlatformProvidedMenuItemType.hide,
                    PlatformProvidedMenuItemType.hideOtherApplications,
                    PlatformProvidedMenuItemType.showAllApplications,
                    PlatformProvidedMenuItemType.startSpeaking,
                    PlatformProvidedMenuItemType.stopSpeaking,
                    PlatformProvidedMenuItemType.toggleFullScreen,
                    PlatformProvidedMenuItemType.minimizeWindow,
                    PlatformProvidedMenuItemType.zoomWindow,
                    PlatformProvidedMenuItemType.arrangeWindowsInFront,
                }.Contains(menu);
            }
            default:
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                );
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override IEnumerable<DartMap<string, object?>> toChannelRepresentation(
        PlatformMenuDelegate @delegate,
        Func<PlatformMenuItem, long> getId
    )
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (!hasMenu(type))
            {
                throw DartRuntimePrimitives.AsException(
                    new DartArgumentError(
                        $"Platform {PlatformLibrary.defaultTargetPlatform.ToString()} has no platform provided menu for "
                            + $"{type}. Call PlatformProvidedMenuItem.hasMenu to determine this before "
                            + "instantiating one."
                    )
                );
            }
            return true;
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        return new List<DartMap<string, object?>>
        {
            new DartMap<string, object?>
            {
                [Platform_menu_barLibrary._kIdKey] = getId(this),
                [Platform_menu_barLibrary._kEnabledKey] = enabled,
                [Platform_menu_barLibrary._kPlatformDefaultMenuKey] =
                    FoundationRuntimePorts.EnumIndex(type),
            },
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new FlagProperty("enabled", value: enabled, ifFalse: "DISABLED"));
    }
}

public enum PlatformProvidedMenuItemType
{
    about,
    quit,
    servicesSubmenu,
    hide,
    hideOtherApplications,
    showAllApplications,
    startSpeaking,
    stopSpeaking,
    toggleFullScreen,
    minimizeWindow,
    zoomWindow,
    arrangeWindowsInFront,
}
