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
    internal virtual global::Doroti.Framework.Services.LogicalKeyboardKey? _trigger { get; private set; }
    internal virtual string? _character { get; private set; }
    internal virtual bool? _alt { get; private set; }
    internal virtual bool? _control { get; private set; }
    internal virtual bool? _meta { get; private set; }
    internal virtual bool? _shift { get; private set; }
    internal static long _shortcutModifierAlt = 1L << (int)2L;
    internal static long _shortcutModifierControl = 1L << (int)3L;
    internal static long _shortcutModifierMeta = 1L << (int)0L;
    internal static long _shortcutModifierShift = 1L << (int)1L;

    public ShortcutSerialization(string character, bool alt = false, bool control = false, bool meta = false)
    {
        _character = character;
        _trigger = null;
        _alt = alt;
        _control = control;
        _meta = meta;
        _shift = null;
        _internal = new DartMap<string, object?> { [Platform_menu_barLibrary._kShortcutCharacter] = character, [Platform_menu_barLibrary._kShortcutModifiers] = (control ? _shortcutModifierControl : 0L) | (alt ? _shortcutModifierAlt : 0L) | (meta ? _shortcutModifierMeta : 0L) };
        System.Diagnostics.Debug.Assert(character.Length == 1L);
    }

    public static ShortcutSerialization CreateModifier(global::Doroti.Framework.Services.LogicalKeyboardKey trigger, bool alt = false, bool control = false, bool meta = false, bool shift = false)
    {
        var __instance = new ShortcutSerialization(default!, alt, control, meta);
        __instance._trigger = trigger;
        __instance._character = null;
        __instance._alt = alt;
        __instance._control = control;
        __instance._meta = meta;
        __instance._shift = shift;
        __instance._internal = new DartMap<string, object?> { [Platform_menu_barLibrary._kShortcutTrigger] = trigger.keyId, [Platform_menu_barLibrary._kShortcutModifiers] = (alt ? _shortcutModifierAlt : 0L) | (control ? _shortcutModifierControl : 0L) | (meta ? _shortcutModifierMeta : 0L) | (shift ? _shortcutModifierShift : 0L) };
        return __instance;
    }

    public virtual global::Doroti.Framework.Services.LogicalKeyboardKey? trigger => _trigger;
    public virtual string? character => _character;
    public virtual bool? alt => _alt;
    public virtual bool? control => _control;
    public virtual bool? meta => _meta;
    public virtual bool? shift => _shift;
    public virtual DartMap<string, object?> toChannelRepresentation() => DartRuntimePrimitives.ConvertValue<DartMap<string, object?>>(_internal);
}

public interface MenuSerializableShortcut
{
    public ShortcutSerialization serializeForMenu();
}

public interface PlatformMenuDelegate
{
    public void setMenus(List<PlatformMenuItem> topLevelMenus);
    public void clearMenus();
    public bool debugLockDelegate(BuildContext context);
    public bool debugUnlockDelegate(BuildContext context);
}

public delegate long MenuItemSerializableIdGenerator(PlatformMenuItem item);

public class DefaultPlatformMenuDelegate : PlatformMenuDelegate
{
    internal virtual DartMap<long, PlatformMenuItem> _idMap { get; private set; } = default!;
    internal virtual long _serial { get; set; } = 0L;
    internal virtual BuildContext? _lockedContext { get; set; } = default;
    public virtual global::Doroti.Framework.Services.MethodChannel channel { get; private set; } = default!;

    public DefaultPlatformMenuDelegate(global::Doroti.Framework.Services.MethodChannel? channel = null)
    {
        this.channel = channel ?? SystemChannels.menu;
        _idMap = new DartMap<long, PlatformMenuItem>();
    }

    public virtual void clearMenus() => setMenus(new List<PlatformMenuItem>());
    public virtual void setMenus(List<PlatformMenuItem> topLevelMenus)
    {
        _idMap.Clear();
        var representation = new List<DartMap<string, object?>>();
        if (Enumerable.Any(topLevelMenus))
        {
            foreach (var childItem in topLevelMenus)
            {
                representation.AddRange(childItem.toChannelRepresentation(this, getId: _getId));
            }
        }
        var windowMenu = new DartMap<string, object?> { ["0"] = representation };
        DartRuntimePrimitives.Ignore(channel.invokeMethod<object?>(Platform_menu_barLibrary._kMenuSetMethod, windowMenu).then((_) =>
        {
        }, onError: (error, stack) =>
        {
            FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: error, stack: stack, library: "widget library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while setting the platform menu")));
        }));
    }

    internal virtual long _getId(PlatformMenuItem item)
    {
        _serial += 1L;
        _idMap[_serial] = item;
        return _serial;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool debugLockDelegate(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((_lockedContext is not null) && (!Equals(_lockedContext, context)))
                {
                    return false;
                }
                _lockedContext = context;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool debugUnlockDelegate(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((_lockedContext is not null) && (!Equals(_lockedContext, context)))
                {
                    return false;
                }
                _lockedContext = null;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal async virtual Future _methodCallHandler(global::Doroti.Framework.Services.MethodCall call)
    {
        var id = call.arguments is long menuId
            ? menuId : throw new FormatException("Platform menu callbacks require an integer menu ID.");
        DartRuntimePrimitives.Assert(() => _idMap.ContainsKey(id), () => (object?)$"Received a menu {call.method} for a menu item with an ID that was not recognized: {id}");
        if (!_idMap.ContainsKey(id))
        {
            return;
        }
        PlatformMenuItem item = _idMap.GetValueOrDefault(id)!;
        if (call.method == Platform_menu_barLibrary._kMenuSelectedCallbackMethod)
        {
            DartRuntimePrimitives.Assert(() => (item.onSelected is null) || (item.onSelectedIntent is null), () => (object?)"Only one of PlatformMenuItem.onSelected or PlatformMenuItem.onSelectedIntent may be specified");
            item.onSelected?.Invoke();
            if (item.onSelectedIntent is not null)
            {
                Actions.maybeInvoke(FocusManager.instance.primaryFocus!.context!, item.onSelectedIntent!);
            }
        }
        else
        {
            if (call.method == Platform_menu_barLibrary._kMenuItemOpenedMethod)
            {
                item.onOpen?.Invoke();
            }
            else
            {
                if (call.method == Platform_menu_barLibrary._kMenuItemClosedMethod)
                {
                    item.onClose?.Invoke();
                }
            }
        }
    }

}

public class PlatformMenuBar : StatefulWidget
{
    public virtual Widget? child { get; private set; }
    public virtual List<PlatformMenuItem> menus { get; private set; } = default!;

    public PlatformMenuBar(global::Doroti.Framework.Foundation.Key? key = null, List<PlatformMenuItem> menus = default!, Widget? child = null) : base(key: key)
    {
        this.menus = menus;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _PlatformMenuBarState__platform_menu_bar());
    public override List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        return menus.map<PlatformMenuItem, global::Doroti.Framework.Foundation.DiagnosticsNode>((child) => ((Diagnosticable)child).toDiagnosticsNode()).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _PlatformMenuBarState__platform_menu_bar : State<PlatformMenuBar>
{
    public virtual List<PlatformMenuItem> descendants { get; set; } = new List<PlatformMenuItem>();

    public override void initState()
    {
        base.initState();
        DartRuntimePrimitives.Assert(() => WidgetsBinding.instance.platformMenuDelegate.debugLockDelegate(context), () => (object?)$"More than one active {typeof(PlatformMenuBar)} detected. Only one active " + "platform-rendered menu bar is allowed at a time.");
        WidgetsBinding.instance.platformMenuDelegate.clearMenus();
        _updateMenu();
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() => WidgetsBinding.instance.platformMenuDelegate.debugUnlockDelegate(context), () => (object?)$"tried to unlock the {typeof(DefaultPlatformMenuDelegate)} more than once with context {context}.");
        WidgetsBinding.instance.platformMenuDelegate.clearMenus();
        base.dispose();
    }

    public override void didUpdateWidget(PlatformMenuBar oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        var newDescendants = new List<PlatformMenuItem>();
        if (!CollectionsLibrary.listEquals(newDescendants, descendants))
        {
            descendants = newDescendants;
            _updateMenu();
        }
    }

    internal virtual void _updateMenu()
    {
        WidgetsBinding.instance.platformMenuDelegate.setMenus(widget.menus);
    }

    public override Widget build(BuildContext context)
    {
        return widget.child ?? new SizedBox();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class PlatformMenu : PlatformMenuItem
{
    private global::System.Action? __field_onOpen = default!;
    public override global::System.Action? onOpen { get => __field_onOpen; }
    private global::System.Action? __field_onClose = default!;
    public override global::System.Action? onClose { get => __field_onClose; }
    public virtual List<PlatformMenuItem> menus { get; private set; } = default!;

    public PlatformMenu(string label, string? tooltip = null, global::System.Action? onOpen = null, global::System.Action? onClose = null, List<PlatformMenuItem> menus = default!) : base(label: label, tooltip: tooltip)
    {
        __field_onOpen = onOpen;
        __field_onClose = onClose;
        this.menus = menus;
    }

    public override List<PlatformMenuItem> descendants => getDescendants(this);
    public static List<PlatformMenuItem> getDescendants(PlatformMenu item)
    {
        return new List<PlatformMenuItem>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IEnumerable<DartMap<string, object?>> toChannelRepresentation(PlatformMenuDelegate @delegate, global::System.Func<PlatformMenuItem, long> getId)
    {
        return new List<DartMap<string, object?>> { serialize(this, @delegate, getId) };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DartMap<string, object?> serialize(PlatformMenu item, PlatformMenuDelegate @delegate, global::System.Func<PlatformMenuItem, long> getId)
    {
        var result = new List<DartMap<string, object?>>();
        foreach (PlatformMenuItem childItem in item.menus)
        {
            result.AddRange(childItem.toChannelRepresentation(@delegate, getId: getId));
        }
        DartMap<string, object?>? previousItem = default!;
        result.removeWhere((item) =>
        {
            if ((previousItem is null) && Equals(item.GetValueOrDefault(Platform_menu_barLibrary._kIsDividerKey), true))
            {
                return true;
            }
            if ((previousItem is not null) && Equals(previousItem!.GetValueOrDefault(Platform_menu_barLibrary._kIsDividerKey), true) && Equals(item.GetValueOrDefault(Platform_menu_barLibrary._kIsDividerKey), true))
            {
                return true;
            }
            previousItem = item.cast<string, object?>();
            return false;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        if (result.LastOrDefault() is var __match22940 && DartPatternRuntime.IsMap(__match22940) && DartPatternRuntime.TryGetMapValue(__match22940, Platform_menu_barLibrary._kIsDividerKey, out var __entry22940_0) && __entry22940_0 is true)
        {
            result.removeLast<DartMap<string, object?>>();
        }
        return new DartMap<string, object?> { [Platform_menu_barLibrary._kIdKey] = getId(item), [Platform_menu_barLibrary._kLabelKey] = item.label, [Platform_menu_barLibrary._kEnabledKey] = Enumerable.Any(item.menus), [Platform_menu_barLibrary._kChildrenKey] = result };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        return menus.map<PlatformMenuItem, global::Doroti.Framework.Foundation.DiagnosticsNode>((child) => ((Diagnosticable)child).toDiagnosticsNode()).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.StringProperty("label", label));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("enabled", value: Enumerable.Any(menus), ifFalse: "DISABLED"));
    }

}

public class PlatformMenuItemGroup : PlatformMenuItem
{
    private List<PlatformMenuItem> __field_members = default!;
    public override List<PlatformMenuItem> members { get => __field_members; }

    public PlatformMenuItemGroup(List<PlatformMenuItem> members) : base(label: "")
    {
        __field_members = members;
    }

    public override IEnumerable<DartMap<string, object?>> toChannelRepresentation(PlatformMenuDelegate @delegate, global::System.Func<PlatformMenuItem, long> getId)
    {
        DartRuntimePrimitives.Assert(() => Enumerable.Any(members), () => (object?)"There must be at least one member in a PlatformMenuItemGroup");
        return serialize(this, @delegate, getId: getId);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public new static IEnumerable<DartMap<string, object?>> serialize(PlatformMenuItem group, PlatformMenuDelegate @delegate, global::System.Func<PlatformMenuItem, long> getId)
    {
        return new List<DartMap<string, object?>> { new DartMap<string, object?> { [Platform_menu_barLibrary._kIdKey] = getId(group), [Platform_menu_barLibrary._kIsDividerKey] = true }, new DartMap<string, object?> { [Platform_menu_barLibrary._kIdKey] = getId(group), [Platform_menu_barLibrary._kIsDividerKey] = true } };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.IterableProperty<PlatformMenuItem>("members", members.Cast<PlatformMenuItem>()));
    }

}

public class PlatformMenuItem : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual string label { get; private set; } = default!;
    public virtual string? tooltip { get; private set; }
    public virtual MenuSerializableShortcut? shortcut { get; private set; }
    public virtual global::System.Action? onSelected { get; private set; }
    public virtual Intent? onSelectedIntent { get; private set; }

    public PlatformMenuItem(string label, string? tooltip = null, MenuSerializableShortcut? shortcut = null, global::System.Action? onSelected = null, Intent? onSelectedIntent = null)
    {
        this.label = label;
        this.tooltip = tooltip;
        this.shortcut = shortcut;
        this.onSelected = onSelected;
        this.onSelectedIntent = onSelectedIntent;
        System.Diagnostics.Debug.Assert((onSelected is null) || (onSelectedIntent is null));
    }

    public virtual global::System.Action? onOpen => DartRuntimePrimitives.ConvertValue<global::System.Action>(null);
    public virtual global::System.Action? onClose => DartRuntimePrimitives.ConvertValue<global::System.Action>(null);
    public virtual List<PlatformMenuItem> descendants => new List<PlatformMenuItem>();
    public virtual List<PlatformMenuItem> members => new List<PlatformMenuItem>();
    public virtual IEnumerable<DartMap<string, object?>> toChannelRepresentation(PlatformMenuDelegate @delegate, global::System.Func<PlatformMenuItem, long> getId)
    {
        return new List<DartMap<string, object?>> { serialize(this, @delegate, getId) };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DartMap<string, object?> serialize(PlatformMenuItem item, PlatformMenuDelegate @delegate, global::System.Func<PlatformMenuItem, long> getId)
    {
        MenuSerializableShortcut? shortcutLocal = item.shortcut;
        return new DartMap<string, object?> { [Platform_menu_barLibrary._kIdKey] = getId(item), [Platform_menu_barLibrary._kLabelKey] = item.label, [Platform_menu_barLibrary._kEnabledKey] = (item.onSelected is not null) || (item.onSelectedIntent is not null) };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string toStringShort() => $"{DiagnosticsLibrary.describeIdentity(this)}({label})";
    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.StringProperty("label", label));
        properties.add(new global::Doroti.Framework.Foundation.StringProperty("tooltip", tooltip, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<MenuSerializableShortcut?>("shortcut", shortcut, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("enabled", value: onSelected is not null, ifFalse: "DISABLED"));
    }

    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class PlatformProvidedMenuItem : PlatformMenuItem
{
    public virtual PlatformProvidedMenuItemType type { get; private set; } = default!;
    public virtual bool enabled { get; private set; } = default!;

    public PlatformProvidedMenuItem(PlatformProvidedMenuItemType type, bool enabled = true) : base(label: "")
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
                    return new HashSet<PlatformProvidedMenuItemType> { PlatformProvidedMenuItemType.about, PlatformProvidedMenuItemType.quit, PlatformProvidedMenuItemType.servicesSubmenu, PlatformProvidedMenuItemType.hide, PlatformProvidedMenuItemType.hideOtherApplications, PlatformProvidedMenuItemType.showAllApplications, PlatformProvidedMenuItemType.startSpeaking, PlatformProvidedMenuItemType.stopSpeaking, PlatformProvidedMenuItemType.toggleFullScreen, PlatformProvidedMenuItemType.minimizeWindow, PlatformProvidedMenuItemType.zoomWindow, PlatformProvidedMenuItemType.arrangeWindowsInFront }.Contains(menu);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IEnumerable<DartMap<string, object?>> toChannelRepresentation(PlatformMenuDelegate @delegate, global::System.Func<PlatformMenuItem, long> getId)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (!hasMenu(type))
                {
                    throw DartRuntimePrimitives.AsException(new DartArgumentError($"Platform {PlatformLibrary.defaultTargetPlatform.ToString()} has no platform provided menu for " + $"{type}. Call PlatformProvidedMenuItem.hasMenu to determine this before " + "instantiating one."));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return new List<DartMap<string, object?>> { new DartMap<string, object?> { [Platform_menu_barLibrary._kIdKey] = getId(this), [Platform_menu_barLibrary._kEnabledKey] = enabled, [Platform_menu_barLibrary._kPlatformDefaultMenuKey] = FoundationRuntimePorts.EnumIndex(type) } };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("enabled", value: enabled, ifFalse: "DISABLED"));
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
    arrangeWindowsInFront
}

