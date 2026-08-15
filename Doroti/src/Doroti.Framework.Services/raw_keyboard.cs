#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/raw_keyboard.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Services;

public enum KeyboardSide
{
    any,
    left,
    right,
    all
}

public enum ModifierKey
{
    controlModifier,
    shiftModifier,
    altModifier,
    metaModifier,
    capsLockModifier,
    numLockModifier,
    scrollLockModifier,
    functionModifier,
    symbolModifier
}

public abstract class RawKeyEventData : Diagnosticable
{
    protected RawKeyEventData()
    {
    }

    public abstract bool isModifierPressed(ModifierKey key, KeyboardSide side = KeyboardSide.any);
    public abstract KeyboardSide? getModifierSide(ModifierKey key);
    public virtual bool isControlPressed => isModifierPressed(ModifierKey.controlModifier);
    public virtual bool isShiftPressed => isModifierPressed(ModifierKey.shiftModifier);
    public virtual bool isAltPressed => isModifierPressed(ModifierKey.altModifier);
    public virtual bool isMetaPressed => isModifierPressed(ModifierKey.metaModifier);
    public virtual DartMap<ModifierKey, KeyboardSide> modifiersPressed
    {
        get
        {
            var result = new DartMap<ModifierKey, KeyboardSide>();
            foreach (ModifierKey key in System.Enum.GetValues<ModifierKey>().ToList())
            {
                if (isModifierPressed(key))
                {
                    KeyboardSide? side__9332 = getModifierSide(key);
                    if ((side__9332 is not null))
                    {
                        result[key] = DartRuntimePrimitives.RequireValue(side__9332);
                    }
                    DartRuntimePrimitives.Assert(() =>
                        {
                            if ((side__9332 is null))
                            {
                                global::Doroti.Generated.Framework.Foundation.PrintLibrary.debugPrint("Raw key data is returning inconsistent information for " + $"pressed modifiers. isModifierPressed returns true for {key} " + "being pressed, but when getModifierSide is called, it says " + "that no modifiers are pressed.");
                                if ((this is RawKeyEventDataAndroid))
                                {
                                    global::Doroti.Generated.Framework.Foundation.PrintLibrary.debugPrint($"Android raw key metaState: {(((RawKeyEventDataAndroid?)this)!).metaState}");
                                }
                            }
                            return true;
                        });
                }
            }
            return result;
        }
    }
    public abstract PhysicalKeyboardKey physicalKey { get; }
    public abstract LogicalKeyboardKey logicalKey { get; }
    public abstract string keyLabel { get; }
    public virtual bool shouldDispatchEvent()
    {
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class RawKeyEvent : Diagnosticable
{
    public virtual string? character { get; private set; }
    public virtual bool repeat { get; private set; } = default!;
    public virtual RawKeyEventData data { get; private set; } = default!;

    protected RawKeyEvent(RawKeyEventData data, string? character = null, bool repeat = false)
    {
        this.data = data;
        this.character = character;
        this.repeat = repeat;
    }

    public static RawKeyEvent CreateFromMessage(DartMap<string, object?> message)
    {
        string? character = default!;
        RawKeyEventData dataFromWeb()
        {
            var key = ((string?)message.GetValueOrDefault("key"))!;
            if ((((key is not null) && (key.Length != 0)) && (key.Length == 1L)))
            {
                character = key;
            }
            return new RawKeyEventDataWeb(code: (((string?)message.GetValueOrDefault("code"))! ?? ""), key: (key ?? ""), location: (((long?)message.GetValueOrDefault("location")) ?? 0L), metaState: (((long?)message.GetValueOrDefault("metaState")) ?? 0L), keyCode: (((long?)message.GetValueOrDefault("keyCode")) ?? 0L));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        RawKeyEventData data = default!;
        if (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb)
        {
            data = dataFromWeb();
        }
        else
        {
            var keymap__15529 = ((string?)message.GetValueOrDefault("keymap__15529")!)!;
            switch (keymap__15529)
            {
                case var __case15605 when object.Equals(__case15605, "android"):
                    {
                        data = new RawKeyEventDataAndroid(flags: (((long?)message.GetValueOrDefault("flags")) ?? 0L), codePoint: (((long?)message.GetValueOrDefault("codePoint")) ?? 0L), keyCode: (((long?)message.GetValueOrDefault("keyCode")) ?? 0L), plainCodePoint: (((long?)message.GetValueOrDefault("plainCodePoint")) ?? 0L), scanCode: (((long?)message.GetValueOrDefault("scanCode")) ?? 0L), metaState: (((long?)message.GetValueOrDefault("metaState")) ?? 0L), eventSource: (((long?)message.GetValueOrDefault("source")) ?? 0L), vendorId: (((long?)message.GetValueOrDefault("vendorId")) ?? 0L), productId: (((long?)message.GetValueOrDefault("productId")) ?? 0L), deviceId: (((long?)message.GetValueOrDefault("deviceId")) ?? 0L), repeatCount: (((long?)message.GetValueOrDefault("repeatCount")) ?? 0L));
                        if (message.ContainsKey("character"))
                        {
                            character = ((string?)message.GetValueOrDefault("character"))!;
                        }
                        break;
                    }
                case var __case16435 when object.Equals(__case16435, "fuchsia"):
                    {
                        long codePoint__16466 = (((long?)message.GetValueOrDefault("codePoint__16466")) ?? 0L);
                        data = new RawKeyEventDataFuchsia(hidUsage: (((long?)message.GetValueOrDefault("hidUsage")) ?? 0L), codePoint: codePoint__16466, modifiers: (((long?)message.GetValueOrDefault("modifiers")) ?? 0L));
                        if ((codePoint__16466 != 0L))
                        {
                            character = char.ConvertFromUtf32(checked((int)codePoint__16466));
                        }
                        break;
                    }
                case var __case16828 when object.Equals(__case16828, "macos"):
                    {
                        data = new RawKeyEventDataMacOs(characters: (((string?)message.GetValueOrDefault("characters"))! ?? ""), charactersIgnoringModifiers: (((string?)message.GetValueOrDefault("charactersIgnoringModifiers"))! ?? ""), keyCode: (((long?)message.GetValueOrDefault("keyCode")) ?? 0L), modifiers: (((long?)message.GetValueOrDefault("modifiers")) ?? 0L), specifiedLogicalKey: ((long?)message.GetValueOrDefault("specifiedLogicalKey")));
                        character = ((string?)message.GetValueOrDefault("characters"))!;
                        break;
                    }
                case var __case17305 when object.Equals(__case17305, "ios"):
                    {
                        data = new RawKeyEventDataIos(characters: (((string?)message.GetValueOrDefault("characters__17660"))! ?? ""), charactersIgnoringModifiers: (((string?)message.GetValueOrDefault("charactersIgnoringModifiers"))! ?? ""), keyCode: (((long?)message.GetValueOrDefault("keyCode")) ?? 0L), modifiers: (((long?)message.GetValueOrDefault("modifiers")) ?? 0L));
                        object? characters__17660 = message.GetValueOrDefault("characters__17660");
                        if (((characters__17660 is string) && (((string)characters__17660).Length != 0)))
                        {
                            character = ((string)characters__17660);
                        }
                        break;
                    }
                case var __case17820 when object.Equals(__case17820, "linux"):
                    {
                        long unicodeScalarValues__17849 = (((long?)message.GetValueOrDefault("unicodeScalarValues__17849")) ?? 0L);
                        data = new RawKeyEventDataLinux(keyHelper: KeyHelper.Create((((string?)message.GetValueOrDefault("toolkit"))! ?? "")), unicodeScalarValues: unicodeScalarValues__17849, keyCode: (((long?)message.GetValueOrDefault("keyCode")) ?? 0L), scanCode: (((long?)message.GetValueOrDefault("scanCode")) ?? 0L), modifiers: (((long?)message.GetValueOrDefault("modifiers")) ?? 0L), isDown: (object.Equals(message.GetValueOrDefault("type"), "keydown")), specifiedLogicalKey: ((long?)message.GetValueOrDefault("specifiedLogicalKey")));
                        if ((unicodeScalarValues__17849 != 0L))
                        {
                            character = char.ConvertFromUtf32(checked((int)unicodeScalarValues__17849));
                        }
                        break;
                    }
                case var __case18517 when object.Equals(__case18517, "windows"):
                    {
                        long characterCodePoint__18548 = (((long?)message.GetValueOrDefault("characterCodePoint__18548")) ?? 0L);
                        data = new RawKeyEventDataWindows(keyCode: (((long?)message.GetValueOrDefault("keyCode")) ?? 0L), scanCode: (((long?)message.GetValueOrDefault("scanCode")) ?? 0L), characterCodePoint: characterCodePoint__18548, modifiers: (((long?)message.GetValueOrDefault("modifiers")) ?? 0L));
                        if ((characterCodePoint__18548 != 0L))
                        {
                            character = char.ConvertFromUtf32(checked((int)characterCodePoint__18548));
                        }
                        break;
                    }
                case var __case19018 when object.Equals(__case19018, "web"):
                    {
                        data = dataFromWeb();
                        break;
                    }
                default:
                    {
                        throw new FlutterError($"Unknown keymap__15529 for key events: {keymap__15529}");
                    }
            }
        }
        bool repeat = RawKeyboard.instance.physicalKeysPressed.Contains(data.physicalKey);
        var type = ((string?)message.GetValueOrDefault("type")!)!;
        return (type switch { var __case19589 when object.Equals(__case19589, "keydown") => new RawKeyDownEvent(data: data, character: character, repeat: repeat), var __case19675 when object.Equals(__case19675, "keyup") => new RawKeyUpEvent(data: data), _ => throw new FlutterError($"Unknown key event type: {type}") });
    }

    public virtual bool isKeyPressed(LogicalKeyboardKey key) => RawKeyboard.instance.keysPressed.Contains(key);
    public virtual bool isControlPressed
    {
        get
        {
            return (isKeyPressed(LogicalKeyboardKey.controlLeft) || isKeyPressed(LogicalKeyboardKey.controlRight));
        }
    }
    public virtual bool isShiftPressed
    {
        get
        {
            return (isKeyPressed(LogicalKeyboardKey.shiftLeft) || isKeyPressed(LogicalKeyboardKey.shiftRight));
        }
    }
    public virtual bool isAltPressed
    {
        get
        {
            return (isKeyPressed(LogicalKeyboardKey.altLeft) || isKeyPressed(LogicalKeyboardKey.altRight));
        }
    }
    public virtual bool isMetaPressed
    {
        get
        {
            return (isKeyPressed(LogicalKeyboardKey.metaLeft) || isKeyPressed(LogicalKeyboardKey.metaRight));
        }
    }
    public virtual PhysicalKeyboardKey physicalKey => data.physicalKey;
    public virtual LogicalKeyboardKey logicalKey => data.logicalKey;
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.Add(new DiagnosticsProperty<LogicalKeyboardKey>("logicalKey", logicalKey));
        properties.Add(new DiagnosticsProperty<PhysicalKeyboardKey>("physicalKey", physicalKey));
        if ((this is RawKeyDownEvent))
        {
            properties.Add(new DiagnosticsProperty<bool>("repeat", repeat));
        }
    }

}

public class RawKeyDownEvent : RawKeyEvent
{
    public RawKeyDownEvent(RawKeyEventData data, string? character = null, bool repeat = false) : base(data: data, character: character, repeat: repeat)
    {
    }

}

public class RawKeyUpEvent : RawKeyEvent
{
    public RawKeyUpEvent(RawKeyEventData data, string? character = null) : base(data, character, repeat: false)
    {
    }

}

public delegate bool RawKeyEventHandler(RawKeyEvent @event);

public class RawKeyboard
{
    public static RawKeyboard instance = new RawKeyboard();
    internal virtual List<Action<RawKeyEvent>> _listeners { get; private set; } = new List<Action<RawKeyEvent>>();
    internal virtual Func<RawKeyEvent, bool>? _cachedKeyEventHandler { get; set; } = default;
    internal virtual Func<KeyMessage, bool>? _cachedKeyMessageHandler { get; set; } = default;
    internal static DartMap<_ModifierSidePair, HashSet<PhysicalKeyboardKey>> _modifierKeyMap = new DartMap<_ModifierSidePair, HashSet<PhysicalKeyboardKey>> { [new _ModifierSidePair(ModifierKey.altModifier, KeyboardSide.left)] = new HashSet<PhysicalKeyboardKey> { PhysicalKeyboardKey.altLeft }, [new _ModifierSidePair(ModifierKey.altModifier, KeyboardSide.right)] = new HashSet<PhysicalKeyboardKey> { PhysicalKeyboardKey.altRight }, [new _ModifierSidePair(ModifierKey.altModifier, KeyboardSide.all)] = new HashSet<PhysicalKeyboardKey> { PhysicalKeyboardKey.altLeft, PhysicalKeyboardKey.altRight }, [new _ModifierSidePair(ModifierKey.altModifier, KeyboardSide.any)] = new HashSet<PhysicalKeyboardKey> { PhysicalKeyboardKey.altLeft }, [new _ModifierSidePair(ModifierKey.shiftModifier, KeyboardSide.left)] = new HashSet<PhysicalKeyboardKey> { PhysicalKeyboardKey.shiftLeft }, [new _ModifierSidePair(ModifierKey.shiftModifier, KeyboardSide.right)] = new HashSet<PhysicalKeyboardKey> { PhysicalKeyboardKey.shiftRight }, [new _ModifierSidePair(ModifierKey.shiftModifier, KeyboardSide.all)] = new HashSet<PhysicalKeyboardKey> { PhysicalKeyboardKey.shiftLeft, PhysicalKeyboardKey.shiftRight }, [new _ModifierSidePair(ModifierKey.shiftModifier, KeyboardSide.any)] = new HashSet<PhysicalKeyboardKey> { PhysicalKeyboardKey.shiftLeft }, [new _ModifierSidePair(ModifierKey.controlModifier, KeyboardSide.left)] = new HashSet<PhysicalKeyboardKey> { PhysicalKeyboardKey.controlLeft }, [new _ModifierSidePair(ModifierKey.controlModifier, KeyboardSide.right)] = new HashSet<PhysicalKeyboardKey> { PhysicalKeyboardKey.controlRight }, [new _ModifierSidePair(ModifierKey.controlModifier, KeyboardSide.all)] = new HashSet<PhysicalKeyboardKey> { PhysicalKeyboardKey.controlLeft, PhysicalKeyboardKey.controlRight }, [new _ModifierSidePair(ModifierKey.controlModifier, KeyboardSide.any)] = new HashSet<PhysicalKeyboardKey> { PhysicalKeyboardKey.controlLeft }, [new _ModifierSidePair(ModifierKey.metaModifier, KeyboardSide.left)] = new HashSet<PhysicalKeyboardKey> { PhysicalKeyboardKey.metaLeft }, [new _ModifierSidePair(ModifierKey.metaModifier, KeyboardSide.right)] = new HashSet<PhysicalKeyboardKey> { PhysicalKeyboardKey.metaRight }, [new _ModifierSidePair(ModifierKey.metaModifier, KeyboardSide.all)] = new HashSet<PhysicalKeyboardKey> { PhysicalKeyboardKey.metaLeft, PhysicalKeyboardKey.metaRight }, [new _ModifierSidePair(ModifierKey.metaModifier, KeyboardSide.any)] = new HashSet<PhysicalKeyboardKey> { PhysicalKeyboardKey.metaLeft }, [new _ModifierSidePair(ModifierKey.capsLockModifier, KeyboardSide.all)] = new HashSet<PhysicalKeyboardKey> { PhysicalKeyboardKey.capsLock }, [new _ModifierSidePair(ModifierKey.numLockModifier, KeyboardSide.all)] = new HashSet<PhysicalKeyboardKey> { PhysicalKeyboardKey.numLock }, [new _ModifierSidePair(ModifierKey.scrollLockModifier, KeyboardSide.all)] = new HashSet<PhysicalKeyboardKey> { PhysicalKeyboardKey.scrollLock }, [new _ModifierSidePair(ModifierKey.functionModifier, KeyboardSide.all)] = new HashSet<PhysicalKeyboardKey> { PhysicalKeyboardKey.fn } };
    internal static DartMap<PhysicalKeyboardKey, LogicalKeyboardKey> _allModifiersExceptFn = new DartMap<PhysicalKeyboardKey, LogicalKeyboardKey> { [PhysicalKeyboardKey.altLeft] = LogicalKeyboardKey.altLeft, [PhysicalKeyboardKey.altRight] = LogicalKeyboardKey.altRight, [PhysicalKeyboardKey.shiftLeft] = LogicalKeyboardKey.shiftLeft, [PhysicalKeyboardKey.shiftRight] = LogicalKeyboardKey.shiftRight, [PhysicalKeyboardKey.controlLeft] = LogicalKeyboardKey.controlLeft, [PhysicalKeyboardKey.controlRight] = LogicalKeyboardKey.controlRight, [PhysicalKeyboardKey.metaLeft] = LogicalKeyboardKey.metaLeft, [PhysicalKeyboardKey.metaRight] = LogicalKeyboardKey.metaRight, [PhysicalKeyboardKey.capsLock] = LogicalKeyboardKey.capsLock, [PhysicalKeyboardKey.numLock] = LogicalKeyboardKey.numLock, [PhysicalKeyboardKey.scrollLock] = LogicalKeyboardKey.scrollLock };
    internal static DartMap<PhysicalKeyboardKey, LogicalKeyboardKey> _allModifiers = new DartMap<PhysicalKeyboardKey, LogicalKeyboardKey> { [PhysicalKeyboardKey.fn] = LogicalKeyboardKey.fn };
    internal virtual DartMap<PhysicalKeyboardKey, LogicalKeyboardKey> _keysPressed { get; private set; } = new DartMap<PhysicalKeyboardKey, LogicalKeyboardKey>();

    public RawKeyboard()
    {
    }

    public virtual void addListener(Action<RawKeyEvent> listener)
    {
        _listeners.Add(listener);
    }

    public virtual void removeListener(Action<RawKeyEvent> listener)
    {
        _listeners.Remove(listener);
    }

    public virtual Func<RawKeyEvent, bool>? keyEventHandler
    {
        get
        {
            if ((!object.Equals((Func<KeyMessage, bool>?)ServicesBinding.instance.keyEventManager.keyMessageHandler, (Func<KeyMessage, bool>?)_cachedKeyMessageHandler)))
            {
                _cachedKeyMessageHandler = ServicesBinding.instance.keyEventManager.keyMessageHandler;
                _cachedKeyEventHandler = ((_cachedKeyMessageHandler is null) ? null : ((@event) =>
                {
                    DartRuntimePrimitives.Assert(() => false);
                    return true;
                }));
            }
            return _cachedKeyEventHandler;
        }
        set
        {
            var handler = value;
            _cachedKeyEventHandler = handler;
            _cachedKeyMessageHandler = ((handler is null) ? null : ((message) =>
            {
                if ((message.rawEvent is not null))
                {
                    return handler(message.rawEvent!);
                }
                return false;
            }));
            ServicesBinding.instance.keyEventManager.keyMessageHandler = _cachedKeyMessageHandler;
        }
    }
    public virtual bool handleRawKeyEvent(RawKeyEvent @event)
    {
        if (@event is RawKeyDownEvent @event__as34058)
        {
            _keysPressed[@event__as34058.physicalKey] = @event__as34058.logicalKey;
        }
        else
        {
            if (@event is RawKeyUpEvent @event__as34159)
            {
                _keysPressed.remove(@event__as34159.physicalKey);
            }
        }
        _synchronizeModifiers(@event);
        DartRuntimePrimitives.Assert(() => ((@event is not RawKeyDownEvent) || (_keysPressed.Count != 0)));
        foreach (var listener in new List<Action<RawKeyEvent>>(_listeners))
        {
            try
            {
                if (_listeners.Contains(listener))
                {
                    listener(@event);
                }
            }
            catch (Exception exception)
            {
                var stack = new System.Diagnostics.StackTrace();
                InformationCollector? collector = default!;
                DartRuntimePrimitives.Assert(() =>
                    {
                        collector = (() => new List<DiagnosticsNode> { new DiagnosticsProperty<RawKeyEvent>("Event", @event) });
                        return true;
                    });
                FlutterError.reportError(new FlutterErrorDetails(exception: exception, stack: stack, library: "services library", context: new ErrorDescription("while processing a raw key listener"), informationCollector: collector));
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _synchronizeModifiers(RawKeyEvent @event)
    {
        DartMap<ModifierKey, KeyboardSide?> modifiersPressed = @event.data.modifiersPressed.cast<ModifierKey, KeyboardSide?>();
        var modifierKeys = new DartMap<PhysicalKeyboardKey, LogicalKeyboardKey>();
        var anySideKeys = new HashSet<PhysicalKeyboardKey>();
        var keysPressedAfterEvent = new HashSet<PhysicalKeyboardKey>();
        ModifierKey? thisKeyModifier = default!;
        foreach (ModifierKey key in System.Enum.GetValues<ModifierKey>().ToList())
        {
            HashSet<PhysicalKeyboardKey>? thisModifierKeys__42120 = _modifierKeyMap.GetValueOrDefault(new _ModifierSidePair(key, KeyboardSide.all));
            if ((thisModifierKeys__42120 is null))
            {
                continue;
            }
            if (thisModifierKeys__42120.Contains(@event.physicalKey))
            {
                thisKeyModifier = key;
            }
            if ((object.Equals(modifiersPressed.GetValueOrDefault(key), KeyboardSide.any)))
            {
                anySideKeys.UnionWith(thisModifierKeys__42120);
                if (thisModifierKeys__42120.any(keysPressedAfterEvent.contains))
                {
                    continue;
                }
            }
            HashSet<PhysicalKeyboardKey>? mappedKeys__42614 = ((modifiersPressed.GetValueOrDefault(key) is null) ? new HashSet<PhysicalKeyboardKey>() : _modifierKeyMap.GetValueOrDefault(new _ModifierSidePair(key, modifiersPressed.GetValueOrDefault(key))));
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((mappedKeys__42614 is null))
                    {
                        global::Doroti.Generated.Framework.Foundation.PrintLibrary.debugPrint($"Platform key support for {Platform.operatingSystem} is " + "producing unsupported modifier combinations for " + $"modifier {key} on side {modifiersPressed.GetValueOrDefault(key)}.");
                        if ((@event.data is RawKeyEventDataAndroid))
                        {
                            global::Doroti.Generated.Framework.Foundation.PrintLibrary.debugPrint($"Android raw key metaState: {(((RawKeyEventDataAndroid?)@event.data)!).metaState}");
                        }
                    }
                    return true;
                });
            if ((mappedKeys__42614 is null))
            {
                continue;
            }
            foreach (PhysicalKeyboardKey physicalModifier in mappedKeys__42614)
            {
                modifierKeys[physicalModifier] = _allModifiers.GetValueOrDefault(physicalModifier)!;
            }
        }
        bool nonModifierCapsLock = (((((@event.data is RawKeyEventDataLinux) || (@event.data is RawKeyEventDataWeb))) && (_keysPressed.GetValueOrDefault(PhysicalKeyboardKey.capsLock) is not null)) && (!object.Equals(_keysPressed.GetValueOrDefault(PhysicalKeyboardKey.capsLock), LogicalKeyboardKey.capsLock)));
        foreach (PhysicalKeyboardKey physicalKey in _allModifiersExceptFn.Keys)
        {
            bool skipReleasingKey__44062 = (nonModifierCapsLock && (object.Equals(physicalKey, PhysicalKeyboardKey.capsLock)));
            if ((!anySideKeys.Contains(physicalKey) && !skipReleasingKey__44062))
            {
                _keysPressed.remove(physicalKey);
            }
        }
        if (((@event.data is not RawKeyEventDataFuchsia) && (@event.data is not RawKeyEventDataMacOs)))
        {
            _keysPressed.remove(PhysicalKeyboardKey.fn);
        }
        _keysPressed.AddRange(modifierKeys);
        if ((((@event is RawKeyDownEvent) && (thisKeyModifier is not null)) && !_keysPressed.ContainsKey(@event.physicalKey)))
        {
            if ((((((@event.data is RawKeyEventDataLinux) && (object.Equals(@event.physicalKey, PhysicalKeyboardKey.altRight)))) || (@event.data is RawKeyEventDataIos)) || (@event.data is RawKeyEventDataAndroid)))
            {
                LogicalKeyboardKey? logicalKey__45274 = _allModifiersExceptFn.GetValueOrDefault(@event.physicalKey);
                if ((logicalKey__45274 is not null))
                {
                    _keysPressed[@event.physicalKey] = logicalKey__45274;
                }
            }
            if (((@event.data is RawKeyEventDataWeb) && (object.Equals(@event.physicalKey, PhysicalKeyboardKey.altRight))))
            {
                _keysPressed[@event.physicalKey] = @event.logicalKey;
            }
        }
    }

    public virtual HashSet<LogicalKeyboardKey> keysPressed => _keysPressed.Values.toSet();
    public virtual HashSet<PhysicalKeyboardKey> physicalKeysPressed => _keysPressed.Keys.toSet();
    public virtual LogicalKeyboardKey? lookUpLayout(PhysicalKeyboardKey physicalKey) => _keysPressed.GetValueOrDefault(physicalKey);
    public virtual void clearKeysPressed() => _keysPressed.Clear();
}

internal class _ModifierSidePair
{
    public virtual ModifierKey modifier { get; private set; } = default!;
    public virtual KeyboardSide? side { get; private set; }

    internal _ModifierSidePair(ModifierKey modifier, KeyboardSide? side)
    {
        this.modifier = modifier;
        this.side = side;
    }

    public override bool Equals(object? other)
    {
        var __other = other as _ModifierSidePair;
        if (__other is null) return false;
        if ((!object.Equals(__other.GetType(), this.GetType())))
        {
            return false;
        }
        return (((__other is _ModifierSidePair) && (object.Equals(((_ModifierSidePair)__other).modifier, modifier))) && (object.Equals(((_ModifierSidePair)__other).side, side)));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(modifier, side);
}

