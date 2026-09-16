// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/shortcuts.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public static partial class ShortcutsLibrary
{
    internal static HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> _controlSynonyms = LogicalKeyboardKey.expandSynonyms(new HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> { LogicalKeyboardKey.control });
}

public static partial class ShortcutsLibrary
{
    internal static HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> _shiftSynonyms = LogicalKeyboardKey.expandSynonyms(new HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> { LogicalKeyboardKey.shift });
}

public static partial class ShortcutsLibrary
{
    internal static HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> _altSynonyms = LogicalKeyboardKey.expandSynonyms(new HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> { LogicalKeyboardKey.alt });
}

public static partial class ShortcutsLibrary
{
    internal static HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> _metaSynonyms = LogicalKeyboardKey.expandSynonyms(new HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> { LogicalKeyboardKey.meta });
}

public class KeySet<T> where T : global::Doroti.Framework.Services.KeyboardKey
{
    internal virtual HashSet<T> _keys { get; private set; } = default!;
    private bool __late_hashCode_initialized;
    private long __late_hashCode = default!;
    public virtual long hashCode
    {
        get
        {
            if (!__late_hashCode_initialized)
            {
                __late_hashCode = _computeHashCode(_keys);
                __late_hashCode_initialized = true;
            }
            return __late_hashCode;
        }
    }
    internal static List<long> _tempHashStore3 = new List<long> { 0L, 0L, 0L };
    internal static List<long> _tempHashStore4 = new List<long> { 0L, 0L, 0L, 0L };

    public KeySet(T key1, T? key2 = default, T? key3 = default, T? key4 = default)
    {
        _keys = ((Func<HashSet<T>>)(() =>
{
    var __cascade = new HashSet<T>();
    __cascade.Add(key1);
    if (key2 is not null) __cascade.Add(key2);
    if (key3 is not null) __cascade.Add(key3);
    if (key4 is not null) __cascade.Add(key4);
    return __cascade;
}))();
    }

    protected KeySet(HashSet<T> keys)
    {
        _keys = new HashSet<T>(keys);
    }

    public static KeySet<T> CreateFromSet(HashSet<T> keys)
    {
        var __instance = new KeySet<T>(default!, default!, default!, default!);
        __instance._keys = new HashSet<T>(keys);
        return __instance;
    }

    public virtual HashSet<T> keys => _keys.toSet();
    public override bool Equals(object? other)
    {
        var __other = other as KeySet<T>;
        if (__other is null) return false;
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is KeySet<T>) && CollectionsLibrary.setEquals<T>(__other._keys, _keys);
    }

    public override int GetHashCode() => hashCode.GetHashCode();

    internal static long _computeHashCode<TKey>(HashSet<TKey> keys) where TKey : notnull
    {
        long length = checked(keys.Count);
        IEnumerator<TKey> iterator = keys.GetEnumerator();
        iterator.MoveNext();
        var h1 = iterator.Current.GetHashCode();
        if (length == 1L)
        {
            return h1;
        }
        iterator.MoveNext();
        var h2 = iterator.Current.GetHashCode();
        if (length == 2L)
        {
            return (h1 < h2) ? FoundationRuntimePorts.ObjectHash(h1, h2) : FoundationRuntimePorts.ObjectHash(h2, h1);
        }
        List<long> sortedHashes = ((length == 3L) ? _tempHashStore3 : _tempHashStore4).ToList();
        sortedHashes[(int)0L] = h1;
        sortedHashes[(int)1L] = h2;
        iterator.MoveNext();
        sortedHashes[(int)2L] = iterator.Current.GetHashCode();
        if (length == 4L)
        {
            iterator.MoveNext();
            sortedHashes[(int)3L] = iterator.Current.GetHashCode();
        }
        sortedHashes.sort();
        return FoundationRuntimePorts.ObjectHashAll(sortedHashes);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public enum LockState
{
    ignored,
    locked,
    unlocked
}

public abstract class ShortcutActivator
{
    protected ShortcutActivator()
    {
    }

    public virtual IEnumerable<global::Doroti.Framework.Services.LogicalKeyboardKey>? triggers => DartRuntimePrimitives.ConvertValue<IEnumerable<global::Doroti.Framework.Services.LogicalKeyboardKey>>(null);
    public abstract bool accepts(global::Doroti.Framework.Services.KeyEvent @event, global::Doroti.Framework.Services.HardwareKeyboard state);
    public static bool isActivatedBy(ShortcutActivator activator, global::Doroti.Framework.Services.KeyEvent @event)
    {
        return activator.accepts(@event, HardwareKeyboard.instance);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract string debugDescribeKeys();
}

public class LogicalKeySet : KeySet<global::Doroti.Framework.Services.LogicalKeyboardKey>, global::Doroti.Framework.Foundation.Diagnosticable
{
    private bool __late__triggers_initialized;
    private HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> __late__triggers = default!;
    internal virtual HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> _triggers
    {
        get
        {
            if (!__late__triggers_initialized)
            {
                __late__triggers = keys.expand((key) => _unmapSynonyms.GetValueOrDefault(key) ?? new List<global::Doroti.Framework.Services.LogicalKeyboardKey> { key }).toSet();
                __late__triggers_initialized = true;
            }
            return __late__triggers;
        }
    }
    internal static HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> _modifiers = new HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> { LogicalKeyboardKey.alt, LogicalKeyboardKey.control, LogicalKeyboardKey.meta, LogicalKeyboardKey.shift };
    internal static DartMap<global::Doroti.Framework.Services.LogicalKeyboardKey, List<global::Doroti.Framework.Services.LogicalKeyboardKey>> _unmapSynonyms = new DartMap<global::Doroti.Framework.Services.LogicalKeyboardKey, List<global::Doroti.Framework.Services.LogicalKeyboardKey>> { [LogicalKeyboardKey.control] = new List<global::Doroti.Framework.Services.LogicalKeyboardKey> { LogicalKeyboardKey.controlLeft, LogicalKeyboardKey.controlRight }, [LogicalKeyboardKey.shift] = new List<global::Doroti.Framework.Services.LogicalKeyboardKey> { LogicalKeyboardKey.shiftLeft, LogicalKeyboardKey.shiftRight }, [LogicalKeyboardKey.alt] = new List<global::Doroti.Framework.Services.LogicalKeyboardKey> { LogicalKeyboardKey.altLeft, LogicalKeyboardKey.altRight }, [LogicalKeyboardKey.meta] = new List<global::Doroti.Framework.Services.LogicalKeyboardKey> { LogicalKeyboardKey.metaLeft, LogicalKeyboardKey.metaRight } };

    public LogicalKeySet(global::Doroti.Framework.Services.LogicalKeyboardKey key1, global::Doroti.Framework.Services.LogicalKeyboardKey? key2 = null, global::Doroti.Framework.Services.LogicalKeyboardKey? key3 = null, global::Doroti.Framework.Services.LogicalKeyboardKey? key4 = null) : base(key1, key2, key3, key4)
    {
    }

    public new static LogicalKeySet CreateFromSet(HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> keys)
    {
        return new LogicalKeySet(keys);
    }

    private LogicalKeySet(HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> keys) : base(keys)
    {
    }

    public virtual IEnumerable<global::Doroti.Framework.Services.LogicalKeyboardKey> triggers => DartRuntimePrimitives.ConvertValue<IEnumerable<global::Doroti.Framework.Services.LogicalKeyboardKey>>(_triggers);
    internal virtual bool _checkKeyRequirements(HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> pressed)
    {
        HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> collapsedRequired = LogicalKeyboardKey.collapseSynonyms(keys);
        HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> collapsedPressed = LogicalKeyboardKey.collapseSynonyms(pressed);
        return (checked(collapsedRequired.Count) == checked((long)collapsedPressed.Count)) && !Enumerable.Any(collapsedRequired.difference<global::Doroti.Framework.Services.LogicalKeyboardKey>(collapsedPressed));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool accepts(global::Doroti.Framework.Services.KeyEvent @event, global::Doroti.Framework.Services.HardwareKeyboard state)
    {
        if ((@event is not KeyDownEvent) && (@event is not KeyRepeatEvent))
        {
            return false;
        }
        return triggers.contains(@event.logicalKey) && _checkKeyRequirements(state.logicalKeysPressed);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string debugDescribeKeys()
    {
        List<global::Doroti.Framework.Services.LogicalKeyboardKey> sortedKeys = ((Func<List<global::Doroti.Framework.Services.LogicalKeyboardKey>>)(() =>
{
    var __cascade = keys.ToList();
    __cascade.sort((a, b) =>
    {
        bool aIsModifier = Enumerable.Any(a.synonyms) || _modifiers.Contains(a);
        bool bIsModifier = Enumerable.Any(b.synonyms) || _modifiers.Contains(b);
        if (aIsModifier && !bIsModifier)
        {
            return -1L;
        }
        else
        {
            if (bIsModifier && !aIsModifier)
            {
                return 1L;
            }
        }
        return a.debugName!.CompareTo(b.debugName!);
        throw new InvalidOperationException("Dart closure completed without a value.");
    });
    return __cascade;
}))().ToList();
        return string.Join(" + ", sortedKeys.map<global::Doroti.Framework.Services.LogicalKeyboardKey, string>((key) => $"{key.debugName}"));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey>>("keys", _keys, description: debugDescribeKeys()));
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);
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

public class ShortcutMapProperty : global::Doroti.Framework.Foundation.DiagnosticsProperty<DartMap<ShortcutActivator, Intent>>
{
    public ShortcutMapProperty(string name, DartMap<ShortcutActivator, Intent> value, bool showName = true, object defaultValue = default!, global::Doroti.Framework.Foundation.DiagnosticLevel level = DiagnosticLevel.info, string? description = null) : base(name, value, showName: showName, defaultValue: defaultValue ?? DiagnosticsLibrary.kNoDefaultValue, level: level, description: description)
    {
    }

    public new virtual DartMap<ShortcutActivator, Intent> value => DartRuntimePrimitives.RequireReference(base.value);
    public virtual string valueToString(global::Doroti.Framework.Foundation.TextTreeConfiguration? parentConfiguration = null)
    {
        return $"{{{string.Join(", ", value.Keys.map<ShortcutActivator, string>((keySet) => $"{{{keySet.debugDescribeKeys()}}}: {value.GetValueOrDefault(keySet)}"))}}}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SingleActivator : ShortcutActivator, global::Doroti.Framework.Foundation.Diagnosticable, MenuSerializableShortcut
{
    public virtual global::Doroti.Framework.Services.LogicalKeyboardKey trigger { get; private set; } = default!;
    public virtual bool control { get; private set; } = default!;
    public virtual bool shift { get; private set; } = default!;
    public virtual bool alt { get; private set; } = default!;
    public virtual bool meta { get; private set; } = default!;
    public virtual LockState numLock { get; private set; } = default!;
    public virtual bool includeRepeats { get; private set; } = default!;

    public SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey trigger, bool control = false, bool shift = false, bool alt = false, bool meta = false, LockState numLock = LockState.ignored, bool includeRepeats = true)
    {
        this.trigger = trigger;
        this.control = control;
        this.shift = shift;
        this.alt = alt;
        this.meta = meta;
        this.numLock = numLock;
        this.includeRepeats = includeRepeats;
        System.Diagnostics.Debug.Assert(!DartRuntimePrimitives.Identical(trigger, LogicalKeyboardKey.control) && !DartRuntimePrimitives.Identical(trigger, LogicalKeyboardKey.controlLeft) && !DartRuntimePrimitives.Identical(trigger, LogicalKeyboardKey.controlRight) && !DartRuntimePrimitives.Identical(trigger, LogicalKeyboardKey.shift) && !DartRuntimePrimitives.Identical(trigger, LogicalKeyboardKey.shiftLeft) && !DartRuntimePrimitives.Identical(trigger, LogicalKeyboardKey.shiftRight) && !DartRuntimePrimitives.Identical(trigger, LogicalKeyboardKey.alt) && !DartRuntimePrimitives.Identical(trigger, LogicalKeyboardKey.altLeft) && !DartRuntimePrimitives.Identical(trigger, LogicalKeyboardKey.altRight) && !DartRuntimePrimitives.Identical(trigger, LogicalKeyboardKey.meta) && !DartRuntimePrimitives.Identical(trigger, LogicalKeyboardKey.metaLeft) && !DartRuntimePrimitives.Identical(trigger, LogicalKeyboardKey.metaRight));
    }

    public override IEnumerable<global::Doroti.Framework.Services.LogicalKeyboardKey> triggers => DartRuntimePrimitives.ConvertValue<IEnumerable<global::Doroti.Framework.Services.LogicalKeyboardKey>>(new List<global::Doroti.Framework.Services.LogicalKeyboardKey> { trigger });
    internal virtual bool _shouldAcceptModifiers(HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> pressed)
    {
        return (control == Enumerable.Any(pressed.intersection(ShortcutsLibrary._controlSynonyms))) && (shift == Enumerable.Any(pressed.intersection(ShortcutsLibrary._shiftSynonyms))) && (alt == Enumerable.Any(pressed.intersection(ShortcutsLibrary._altSynonyms))) && (meta == Enumerable.Any(pressed.intersection(ShortcutsLibrary._metaSynonyms)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _shouldAcceptNumLock(global::Doroti.Framework.Services.HardwareKeyboard state)
    {
        return numLock switch { LockState.ignored => true, LockState.locked => state.lockModesEnabled.Contains(KeyboardLockMode.numLock), LockState.unlocked => !state.lockModesEnabled.Contains(KeyboardLockMode.numLock), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool accepts(global::Doroti.Framework.Services.KeyEvent @event, global::Doroti.Framework.Services.HardwareKeyboard state)
    {
        return ((@event is global::Doroti.Framework.Services.KeyDownEvent) || includeRepeats && (@event is global::Doroti.Framework.Services.KeyRepeatEvent)) && triggers.contains(@event.logicalKey) && _shouldAcceptModifiers(state.logicalKeysPressed) && _shouldAcceptNumLock(state);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ShortcutSerialization serializeForMenu()
    {
        return ShortcutSerialization.CreateModifier(trigger, shift: shift, alt: alt, meta: meta, control: control);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string debugDescribeKeys()
    {
        var result = "";
        DartRuntimePrimitives.Assert(() =>
            {
                var keys = new List<string> { trigger.debugName ?? ((Diagnosticable)trigger).toStringShort() };
                result = string.Join(" + ", keys);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.MessageProperty("keys", debugDescribeKeys()));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("includeRepeats", value: includeRepeats, ifFalse: "excluding repeats"));
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);
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

public class CharacterActivator : ShortcutActivator, global::Doroti.Framework.Foundation.Diagnosticable, MenuSerializableShortcut
{
    public virtual bool alt { get; private set; } = default!;
    public virtual bool control { get; private set; } = default!;
    public virtual bool meta { get; private set; } = default!;
    public virtual bool includeRepeats { get; private set; } = default!;
    public virtual string character { get; private set; } = default!;

    public CharacterActivator(string character, bool alt = false, bool control = false, bool meta = false, bool includeRepeats = true)
    {
        this.character = character;
        this.alt = alt;
        this.control = control;
        this.meta = meta;
        this.includeRepeats = includeRepeats;
    }

    public override IEnumerable<global::Doroti.Framework.Services.LogicalKeyboardKey>? triggers => null;
    internal virtual bool _shouldAcceptModifiers(HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> pressed)
    {
        return (control == Enumerable.Any(pressed.intersection(ShortcutsLibrary._controlSynonyms))) && (alt == Enumerable.Any(pressed.intersection(ShortcutsLibrary._altSynonyms))) && (meta == Enumerable.Any(pressed.intersection(ShortcutsLibrary._metaSynonyms)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool accepts(global::Doroti.Framework.Services.KeyEvent @event, global::Doroti.Framework.Services.HardwareKeyboard state)
    {
        return (@event.character == character) && ((@event is global::Doroti.Framework.Services.KeyDownEvent) || includeRepeats && (@event is global::Doroti.Framework.Services.KeyRepeatEvent)) && _shouldAcceptModifiers(state.logicalKeysPressed);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string debugDescribeKeys()
    {
        var result = "";
        DartRuntimePrimitives.Assert(() =>
            {
                var keys = new List<string> { $"'{character}'" };
                result = string.Join(" + ", keys);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ShortcutSerialization serializeForMenu()
    {
        return new ShortcutSerialization(character, alt: alt, control: control, meta: meta);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.MessageProperty("character", debugDescribeKeys()));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("includeRepeats", value: includeRepeats, ifFalse: "excluding repeats"));
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);
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

internal class _ActivatorIntentPair__shortcuts : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual ShortcutActivator activator { get; private set; } = default!;
    public virtual Intent intent { get; private set; } = default!;

    internal _ActivatorIntentPair__shortcuts(ShortcutActivator activator, Intent intent)
    {
        this.activator = activator;
        this.intent = intent;
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<string>("activator", activator.debugDescribeKeys()));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Intent>("intent", intent));
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);
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

public class ShortcutManager : ChangeNotifier, global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual bool modal { get; private set; } = default!;
    internal virtual DartMap<ShortcutActivator, Intent> _shortcuts { get; set; } = new DartMap<ShortcutActivator, Intent>();
    internal virtual DartMap<global::Doroti.Framework.Services.LogicalKeyboardKey?, List<_ActivatorIntentPair__shortcuts>>? _indexedShortcutsCache { get; set; } = default;

    public ShortcutManager(DartMap<ShortcutActivator, Intent> shortcuts = default!, bool modal = false)
    {
        DartMap<ShortcutActivator, Intent> __shortcuts = shortcuts ?? new DartMap<ShortcutActivator, Intent>();
        this.modal = modal;
        _shortcuts = __shortcuts;
    }

    public virtual DartMap<ShortcutActivator, Intent> shortcuts
    {
        get => _shortcuts;
        set
        {
            var __value = value;
            if (!CollectionsLibrary.mapEquals<ShortcutActivator, Intent>(_shortcuts, __value))
            {
                _shortcuts = __value;
                _indexedShortcutsCache = null;
                notifyListeners();
            }
        }
    }
    internal static DartMap<global::Doroti.Framework.Services.LogicalKeyboardKey?, List<_ActivatorIntentPair__shortcuts>> _indexShortcuts(DartMap<ShortcutActivator, Intent> source)
    {
        var result = new DartMap<global::Doroti.Framework.Services.LogicalKeyboardKey?, List<_ActivatorIntentPair__shortcuts>>();
        source.forEach((activator, intent) =>
        {
            IEnumerable<global::Doroti.Framework.Services.LogicalKeyboardKey?>? nullableTriggers = (IEnumerable<global::Doroti.Framework.Services.LogicalKeyboardKey?>?)activator.triggers;
            foreach (global::Doroti.Framework.Services.LogicalKeyboardKey? trigger in nullableTriggers ?? new List<global::Doroti.Framework.Services.LogicalKeyboardKey?> { null })
            {
                result.putIfAbsent(trigger, () => new List<_ActivatorIntentPair__shortcuts>()).Add(new _ActivatorIntentPair__shortcuts(activator, intent));
            }
        });
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual DartMap<global::Doroti.Framework.Services.LogicalKeyboardKey?, List<_ActivatorIntentPair__shortcuts>> _indexedShortcuts
    {
        get
        {
            return _indexedShortcutsCache ??= _indexShortcuts(shortcuts);
        }
    }
    internal virtual IEnumerable<_ActivatorIntentPair__shortcuts> _getCandidates(global::Doroti.Framework.Services.LogicalKeyboardKey key)
    {
        // Match the trigger-specific entries first, then activators that accept
        // any trigger. Preserve registration order within each group.
        if (_indexedShortcuts.TryGetValue(key, out var indexed))
            foreach (var candidate in indexed) yield return candidate;
        if (_indexedShortcuts.TryGetValue(null, out var unindexed))
            foreach (var candidate in unindexed) yield return candidate;
    }

    internal virtual Intent? _find(global::Doroti.Framework.Services.KeyEvent @event, global::Doroti.Framework.Services.HardwareKeyboard state)
    {
        foreach (_ActivatorIntentPair__shortcuts activatorIntent in _getCandidates(@event.logicalKey))
        {
            if (activatorIntent.activator.accepts(@event, state))
            {
                return activatorIntent.intent;
            }
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual KeyEventResult handleKeypress(BuildContext context, global::Doroti.Framework.Services.KeyEvent @event)
    {
        Intent? intentLocal = _find(@event, HardwareKeyboard.instance);
        BuildContext? contextLocal = Focus_managerLibrary.primaryFocus?.context;
        if (intentLocal is not null && contextLocal is not null)
        {
            // Action<SpecificIntent> is not Action<Intent> in C#. Use the
            // type-erased bridge while retaining scoped dispatch and overrides.
            var action = Actions._maybeFindWithoutDependingOn(contextLocal, intentLocal, declareDependency: true);
            if (action is not null)
            {
                var (enabled, invokeResult) = Actions.of(contextLocal).invokeActionIfEnabled(action, intentLocal, contextLocal);
                if (enabled) return action.ToKeyEventResultForIntent(intentLocal, invokeResult);
            }
        }
        return modal ? KeyEventResult.skipRemainingHandlers : KeyEventResult.ignored;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<DartMap<ShortcutActivator, Intent>>("shortcuts", shortcuts));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("modal", value: modal, ifTrue: "modal", defaultValue: false));
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);
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

public class Shortcuts : StatefulWidget
{
    public virtual ShortcutManager? manager { get; private set; }
    internal virtual DartMap<ShortcutActivator, Intent> _shortcuts { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;
    public virtual string? debugLabel { get; private set; }
    public virtual bool includeSemantics { get; private set; } = default!;

    public Shortcuts(global::Doroti.Framework.Foundation.Key? key = null, DartMap<ShortcutActivator, Intent> shortcuts = default!, Widget child = default!, string? debugLabel = null, bool includeSemantics = true) : base(key: key)
    {
        this.child = child;
        this.debugLabel = debugLabel;
        this.includeSemantics = includeSemantics;
        _shortcuts = shortcuts;
        manager = null;
    }

    public static Shortcuts CreateManager(global::Doroti.Framework.Foundation.Key? key = null, ShortcutManager manager = default!, Widget child = default!, string? debugLabel = null, bool includeSemantics = true)
    {
        var __instance = new Shortcuts(key, default!, child, debugLabel, includeSemantics);
        __instance.manager = manager;
        __instance.child = child;
        __instance.debugLabel = debugLabel;
        __instance.includeSemantics = includeSemantics;
        __instance._shortcuts = new DartMap<ShortcutActivator, Intent>();
        return __instance;
    }

    public virtual DartMap<ShortcutActivator, Intent> shortcuts
    {
        get
        {
            return (manager is null) ? _shortcuts : manager!.shortcuts;
        }
    }
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ShortcutsState__shortcuts());
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ShortcutManager>("manager", manager, defaultValue: null));
        properties.add(new ShortcutMapProperty("shortcuts", shortcuts, description: ((debugLabel is null ? (bool?)null : debugLabel.Length != 0) ?? false) ? debugLabel : null));
    }

}

internal class _ShortcutsState__shortcuts : State<Shortcuts>
{
    internal virtual ShortcutManager? _internalManager { get; set; } = default;

    public virtual ShortcutManager manager => DartRuntimePrimitives.ConvertValue<ShortcutManager>(widget.manager ?? _internalManager!);
    public override void dispose()
    {
        _internalManager?.dispose();
        base.dispose();
    }

    public override void initState()
    {
        base.initState();
        if (widget.manager is null)
        {
            _internalManager = new ShortcutManager();
            _internalManager!.shortcuts = widget.shortcuts;
        }
    }

    public override void didUpdateWidget(Shortcuts oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(widget.manager, oldWidget.manager))
        {
            if (widget.manager is not null)
            {
                _internalManager?.dispose();
                _internalManager = null;
            }
            else
            {
                _internalManager ??= new ShortcutManager();
            }
        }
        _internalManager?.shortcuts = widget.shortcuts;
    }

    internal virtual KeyEventResult _handleOnKeyEvent(FocusNode node, global::Doroti.Framework.Services.KeyEvent @event)
    {
        if (node.context is null)
        {
            return KeyEventResult.ignored;
        }
        return manager.handleKeypress(node.context!, @event);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        return new Focus(debugLabel: (widget.debugLabel is not null) ? $"{typeof(Shortcuts)}: {widget.debugLabel}" : $"{typeof(Shortcuts)}", canRequestFocus: false, onKeyEvent: _handleOnKeyEvent, includeSemantics: widget.includeSemantics, child: widget.child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CallbackShortcuts : StatelessWidget
{
    public virtual DartMap<ShortcutActivator, global::System.Action> bindings { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public CallbackShortcuts(global::Doroti.Framework.Foundation.Key? key = null, DartMap<ShortcutActivator, global::System.Action> bindings = default!, Widget child = default!) : base(key: key)
    {
        this.bindings = bindings;
        this.child = child;
    }

    internal virtual bool _applyKeyEventBinding(ShortcutActivator activator, global::Doroti.Framework.Services.KeyEvent @event)
    {
        if (activator.accepts(@event, HardwareKeyboard.instance))
        {
            bindings.GetValueOrDefault(activator)!?.Invoke();
            return true;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        return new Focus(canRequestFocus: false, skipTraversal: true, onKeyEvent: (node, @event) =>
        {
            KeyEventResult result = KeyEventResult.ignored;
            foreach (ShortcutActivator activator in bindings.Keys)
            {
                result = _applyKeyEventBinding(activator, @event) ? KeyEventResult.handled : result;
            }
            return result;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ShortcutRegistryEntry
{
    public virtual ShortcutRegistry registry { get; private set; } = default!;

    public ShortcutRegistryEntry(ShortcutRegistry registry)
    {
        this.registry = registry;
    }

    public virtual void replaceAll(DartMap<ShortcutActivator, Intent> value)
    {
        registry._replaceAll(this, value);
    }

    public virtual void dispose()
    {
        registry._disposeEntry(this);
    }

}

public class ShortcutRegistry : ChangeNotifier
{
    internal virtual bool _notificationScheduled { get; set; } = false;
    internal virtual bool _disposed { get; set; } = false;
    internal virtual DartMap<ShortcutRegistryEntry, DartMap<ShortcutActivator, Intent>> _registeredShortcuts { get; private set; } = new DartMap<ShortcutRegistryEntry, DartMap<ShortcutActivator, Intent>>();

    public ShortcutRegistry()
    {
    }

    public override void dispose()
    {
        base.dispose();
        _disposed = true;
    }

    public virtual DartMap<ShortcutActivator, Intent> shortcuts
    {
        get
        {
            DartRuntimePrimitives.Assert(() => debugAssertNotDisposed(this));
            return new DartMap<ShortcutActivator, Intent>();
        }
    }
    public virtual ShortcutRegistryEntry addAll(DartMap<ShortcutActivator, Intent> value)
    {
        DartRuntimePrimitives.Assert(() => debugAssertNotDisposed(this));
        DartRuntimePrimitives.Assert(() => Enumerable.Any(value), () => (object?)"Cannot register an empty map of shortcuts");
        var entry = new ShortcutRegistryEntry(this);
        _registeredShortcuts[entry] = value;
        DartRuntimePrimitives.Assert(() => _debugCheckForDuplicates());
        _notifyListenersNextFrame();
        return entry;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _notifyListenersNextFrame()
    {
        if (!_notificationScheduled)
        {
            Scheduler.SchedulerBinding.instance.addPostFrameCallback((_) =>
            {
                _notificationScheduled = false;
                if (!_disposed)
                {
                    notifyListeners();
                }
            }, debugLabel: "ShortcutRegistry.notifyListeners");
            _notificationScheduled = true;
        }
    }

    public static ShortcutRegistry of(BuildContext context)
    {
        _ShortcutRegistrarScope__shortcuts? inherited = context.dependOnInheritedWidgetOfExactType<_ShortcutRegistrarScope__shortcuts>();
        DartRuntimePrimitives.Assert(() =>
            {
                if (inherited is null)
                {
                    throw DartRuntimePrimitives.AsException(FlutterError.Create($"Unable to find a {typeof(ShortcutRegistrar)} widget in the context.\n" + $"{typeof(ShortcutRegistrar)}.of() was called with a context that does not contain a " + $"{typeof(ShortcutRegistrar)} widget.\n" + $"No {typeof(ShortcutRegistrar)} ancestor could be found starting from the context that was " + $"passed to {typeof(ShortcutRegistrar)}.of().\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return inherited!.registry;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ShortcutRegistry? maybeOf(BuildContext context)
    {
        _ShortcutRegistrarScope__shortcuts? inherited = context.dependOnInheritedWidgetOfExactType<_ShortcutRegistrarScope__shortcuts>();
        return inherited?.registry;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _replaceAll(ShortcutRegistryEntry entry, DartMap<ShortcutActivator, Intent> value)
    {
        DartRuntimePrimitives.Assert(() => debugAssertNotDisposed(this));
        DartRuntimePrimitives.Assert(() => _debugCheckEntryIsValid(entry));
        _registeredShortcuts[entry] = value;
        DartRuntimePrimitives.Assert(() => _debugCheckForDuplicates());
        _notifyListenersNextFrame();
    }

    internal virtual void _disposeEntry(ShortcutRegistryEntry entry)
    {
        DartRuntimePrimitives.Assert(() => _debugCheckEntryIsValid(entry));
        if (_registeredShortcuts.remove(entry) is not null)
        {
            _notifyListenersNextFrame();
        }
    }

    internal virtual bool _debugCheckEntryIsValid(ShortcutRegistryEntry entry)
    {
        if (!_registeredShortcuts.ContainsKey(entry))
        {
            if (Equals(entry.registry, this))
            {
                throw DartRuntimePrimitives.AsException(FlutterError.Create($"entry {DiagnosticsLibrary.describeIdentity(entry)} is invalid.\n" + "The entry has already been disposed of. Tokens are not valid after " + "dispose is called on them, and should no longer be used."));
            }
            else
            {
                throw DartRuntimePrimitives.AsException(FlutterError.Create($"Foreign entry {DiagnosticsLibrary.describeIdentity(entry)} used.\n" + "This entry was not created by this registry, it was created by " + $"{DiagnosticsLibrary.describeIdentity(entry.registry)}, and should be used with that " + "registry instead."));
            }
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _debugCheckForDuplicates()
    {
        var previous = new DartMap<ShortcutActivator, ShortcutRegistryEntry?>();
        foreach (MapEntry<ShortcutRegistryEntry, DartMap<ShortcutActivator, Intent>> tokenEntry in _registeredShortcuts.entries)
        {
            foreach (ShortcutActivator shortcut in tokenEntry.value.Keys)
            {
                if (previous.ContainsKey(shortcut))
                {
                    throw DartRuntimePrimitives.AsException(FlutterError.Create($"{typeof(ShortcutRegistry)}: Received a duplicate registration for the " + $"shortcut {shortcut} in {DiagnosticsLibrary.describeIdentity(tokenEntry.key)} and {previous.GetValueOrDefault(shortcut)}."));
                }
                previous[shortcut] = tokenEntry.key;
            }
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ShortcutRegistrar : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;

    public ShortcutRegistrar(global::Doroti.Framework.Foundation.Key? key = null, Widget child = default!) : base(key: key)
    {
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ShortcutRegistrarState__shortcuts());
}

internal class _ShortcutRegistrarState__shortcuts : State<ShortcutRegistrar>
{
    public virtual ShortcutRegistry registry { get; private set; } = new ShortcutRegistry();
    public virtual ShortcutManager manager { get; private set; } = new ShortcutManager();

    public override void initState()
    {
        base.initState();
        registry.addListener(_shortcutsChanged);
    }

    internal virtual void _shortcutsChanged()
    {
        manager.shortcuts = registry.shortcuts;
    }

    public override void dispose()
    {
        registry.removeListener(_shortcutsChanged);
        registry.dispose();
        manager.dispose();
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return new _ShortcutRegistrarScope__shortcuts(registry: registry, child: Shortcuts.CreateManager(manager: manager, debugLabel: "<Shortcut Registrar>", child: widget.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ShortcutRegistrarScope__shortcuts : InheritedWidget
{
    public virtual ShortcutRegistry registry { get; private set; } = default!;

    internal _ShortcutRegistrarScope__shortcuts(ShortcutRegistry registry, Widget child) : base(child: child)
    {
        this.registry = registry;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (_ShortcutRegistrarScope__shortcuts)oldWidget;
        return !Equals(registry, __oldWidget.registry);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
