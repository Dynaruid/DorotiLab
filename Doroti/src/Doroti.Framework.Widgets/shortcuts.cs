// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/shortcuts.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Widgets;

public static partial class ShortcutsLibrary
{
    internal static HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> _controlSynonyms = LogicalKeyboardKey.expandSynonyms(new HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> { global::Doroti.Framework.Services.LogicalKeyboardKey.control });
}

public static partial class ShortcutsLibrary
{
    internal static HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> _shiftSynonyms = LogicalKeyboardKey.expandSynonyms(new HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> { global::Doroti.Framework.Services.LogicalKeyboardKey.shift });
}

public static partial class ShortcutsLibrary
{
    internal static HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> _altSynonyms = LogicalKeyboardKey.expandSynonyms(new HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> { global::Doroti.Framework.Services.LogicalKeyboardKey.alt });
}

public static partial class ShortcutsLibrary
{
    internal static HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> _metaSynonyms = LogicalKeyboardKey.expandSynonyms(new HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> { global::Doroti.Framework.Services.LogicalKeyboardKey.meta });
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
                __late_hashCode = KeySet<T>._computeHashCode(this._keys);
                __late_hashCode_initialized = true;
            }
            return __late_hashCode;
        }
    }
    internal static List<long> _tempHashStore3 = new List<long> { 0L, 0L, 0L };
    internal static List<long> _tempHashStore4 = new List<long> { 0L, 0L, 0L, 0L };

    public KeySet(T key1, T? key2 = default, T? key3 = default, T? key4 = default)
    {
        this._keys = ((Func<HashSet<T>>)(() =>
{
    var __cascade = new HashSet<T>();
    __cascade.Add(key1);
    return __cascade;
}))();
    }

    public static KeySet<T> CreateFromSet(HashSet<T> keys)
    {
        var __instance = new KeySet<T>(default!, default!, default!, default!);
        __instance._keys = new HashSet<T>(keys);
        return __instance;
    }

    public virtual HashSet<T> keys => this._keys.toSet();
    public override bool Equals(object? other)
    {
        var __other = other as KeySet<T>;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((__other is KeySet<T>) && global::Doroti.Framework.Foundation.CollectionsLibrary.setEquals<T>(((KeySet<T>)((KeySet<T>)__other))._keys, this._keys));
    }

    internal static long _computeHashCode<T>(HashSet<T> keys) where T : notnull
    {
        long length__3663 = checked((long)(keys.Count));
        IEnumerator<T> iterator__3707 = keys.GetEnumerator();
        iterator__3707.MoveNext();
        var h1__3826 = iterator__3707.Current.GetHashCode();
        if ((length__3663 == 1L))
        {
            return h1__3826;
        }
        iterator__3707.MoveNext();
        var h2__4002 = iterator__3707.Current.GetHashCode();
        if ((length__3663 == 2L))
        {
            return ((h1__3826 < h2__4002) ? FoundationRuntimePorts.ObjectHash(h1__3826, h2__4002) : FoundationRuntimePorts.ObjectHash(h2__4002, h1__3826));
        }
        List<long> sortedHashes__4344 = ((length__3663 == 3L) ? _tempHashStore3 : _tempHashStore4).ToList();
        sortedHashes__4344[(int)(0L)] = h1__3826;
        sortedHashes__4344[(int)(1L)] = h2__4002;
        iterator__3707.MoveNext();
        sortedHashes__4344[(int)(2L)] = iterator__3707.Current.GetHashCode();
        if ((length__3663 == 4L))
        {
            iterator__3707.MoveNext();
            sortedHashes__4344[(int)(3L)] = iterator__3707.Current.GetHashCode();
        }
        sortedHashes__4344.sort();
        return FoundationRuntimePorts.ObjectHashAll(sortedHashes__4344);
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
        return activator.accepts(@event, global::Doroti.Framework.Services.HardwareKeyboard.instance);
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
                __late__triggers = this.keys.expand(((key) => (_unmapSynonyms.GetValueOrDefault(key) ?? new List<global::Doroti.Framework.Services.LogicalKeyboardKey> { key }))).toSet();
                __late__triggers_initialized = true;
            }
            return __late__triggers;
        }
    }
    internal static HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> _modifiers = new HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> { global::Doroti.Framework.Services.LogicalKeyboardKey.alt, global::Doroti.Framework.Services.LogicalKeyboardKey.control, global::Doroti.Framework.Services.LogicalKeyboardKey.meta, global::Doroti.Framework.Services.LogicalKeyboardKey.shift };
    internal static DartMap<global::Doroti.Framework.Services.LogicalKeyboardKey, List<global::Doroti.Framework.Services.LogicalKeyboardKey>> _unmapSynonyms = new DartMap<global::Doroti.Framework.Services.LogicalKeyboardKey, List<global::Doroti.Framework.Services.LogicalKeyboardKey>> { [global::Doroti.Framework.Services.LogicalKeyboardKey.control] = new List<global::Doroti.Framework.Services.LogicalKeyboardKey> { global::Doroti.Framework.Services.LogicalKeyboardKey.controlLeft, global::Doroti.Framework.Services.LogicalKeyboardKey.controlRight }, [global::Doroti.Framework.Services.LogicalKeyboardKey.shift] = new List<global::Doroti.Framework.Services.LogicalKeyboardKey> { global::Doroti.Framework.Services.LogicalKeyboardKey.shiftLeft, global::Doroti.Framework.Services.LogicalKeyboardKey.shiftRight }, [global::Doroti.Framework.Services.LogicalKeyboardKey.alt] = new List<global::Doroti.Framework.Services.LogicalKeyboardKey> { global::Doroti.Framework.Services.LogicalKeyboardKey.altLeft, global::Doroti.Framework.Services.LogicalKeyboardKey.altRight }, [global::Doroti.Framework.Services.LogicalKeyboardKey.meta] = new List<global::Doroti.Framework.Services.LogicalKeyboardKey> { global::Doroti.Framework.Services.LogicalKeyboardKey.metaLeft, global::Doroti.Framework.Services.LogicalKeyboardKey.metaRight } };

    public LogicalKeySet(global::Doroti.Framework.Services.LogicalKeyboardKey key1, global::Doroti.Framework.Services.LogicalKeyboardKey? key2 = null, global::Doroti.Framework.Services.LogicalKeyboardKey? key3 = null, global::Doroti.Framework.Services.LogicalKeyboardKey? key4 = null) : base(key1, key2, key3, key4)
    {
    }

    public static LogicalKeySet CreateFromSet(HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> keys)
    {
        var __instance = new LogicalKeySet(default!, default!, default!, default!);
        return __instance;
    }

    public virtual IEnumerable<global::Doroti.Framework.Services.LogicalKeyboardKey> triggers => DartRuntimePrimitives.ConvertValue<IEnumerable<global::Doroti.Framework.Services.LogicalKeyboardKey>>(this._triggers);
    internal virtual bool _checkKeyRequirements(HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> pressed)
    {
        HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> collapsedRequired__11872 = ((HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey>)(object?)LogicalKeyboardKey.collapseSynonyms(this.keys));
        HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> collapsedPressed__11969 = ((HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey>)(object?)LogicalKeyboardKey.collapseSynonyms(pressed));
        return ((checked((long)(collapsedRequired__11872.Count)) == checked((long)(collapsedPressed__11969.Count))) && !System.Linq.Enumerable.Any(collapsedRequired__11872.difference<global::Doroti.Framework.Services.LogicalKeyboardKey>(collapsedPressed__11969)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool accepts(global::Doroti.Framework.Services.KeyEvent @event, global::Doroti.Framework.Services.HardwareKeyboard state)
    {
        if (((@event is not global::Doroti.Framework.Services.KeyDownEvent) && (@event is not global::Doroti.Framework.Services.KeyRepeatEvent)))
        {
            return false;
        }
        return (this.triggers.contains(((global::Doroti.Framework.Services.KeyEvent)@event).logicalKey) && _checkKeyRequirements(((global::Doroti.Framework.Services.HardwareKeyboard)state).logicalKeysPressed));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string debugDescribeKeys()
    {
        List<global::Doroti.Framework.Services.LogicalKeyboardKey> sortedKeys__13441 = ((Func<List<global::Doroti.Framework.Services.LogicalKeyboardKey>>)(() =>
{
    var __cascade = this.keys.ToList();
    __cascade.sort(((a, b) =>
    {
        bool aIsModifier__13666 = (System.Linq.Enumerable.Any(((global::Doroti.Framework.Services.LogicalKeyboardKey)a).synonyms) || _modifiers.Contains(a));
        bool bIsModifier__13748 = (System.Linq.Enumerable.Any(((global::Doroti.Framework.Services.LogicalKeyboardKey)b).synonyms) || _modifiers.Contains(b));
        if ((aIsModifier__13666 && !bIsModifier__13748))
        {
            return -1L;
        }
        else
        {
            if ((bIsModifier__13748 && !aIsModifier__13666))
            {
                return 1L;
            }
        }
        return ((global::Doroti.Framework.Services.LogicalKeyboardKey)a).debugName!.CompareTo(((global::Doroti.Framework.Services.LogicalKeyboardKey)b).debugName!);
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    return __cascade;
}))().ToList();
        return string.Join(" + ", sortedKeys__13441.map<global::Doroti.Framework.Services.LogicalKeyboardKey, string>(((key) => ((global::Doroti.Framework.Services.LogicalKeyboardKey)key).debugName.ToString())));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey>>("keys", this._keys, description: debugDescribeKeys()));
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString__105654 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString__105654 = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((fullString__105654 ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ShortcutMapProperty : global::Doroti.Framework.Foundation.DiagnosticsProperty<DartMap<ShortcutActivator, Intent>>
{
    public ShortcutMapProperty(string name, DartMap<ShortcutActivator, Intent> value, bool showName = true, object defaultValue = default!, global::Doroti.Framework.Foundation.DiagnosticLevel level = global::Doroti.Framework.Foundation.DiagnosticLevel.info, string? description = null) : base(name, value, showName: showName, defaultValue: defaultValue ?? global::Doroti.Framework.Foundation.DiagnosticsLibrary.kNoDefaultValue, level: level, description: description)
    {
    }

    public virtual DartMap<ShortcutActivator, Intent>? value => DartRuntimePrimitives.ConvertValue<DartMap<ShortcutActivator, Intent>>(base.value!);
    public virtual string valueToString(global::Doroti.Framework.Foundation.TextTreeConfiguration? parentConfiguration = null)
    {
        return $"{{{string.Join(", ", this.value.Keys.map<ShortcutActivator, string>(((keySet) => $"{{{keySet.debugDescribeKeys()}}}: {this.value.GetValueOrDefault(keySet)}")))}}}";
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
        System.Diagnostics.Debug.Assert((((((((((((!DartRuntimePrimitives.Identical(trigger, global::Doroti.Framework.Services.LogicalKeyboardKey.control) && !DartRuntimePrimitives.Identical(trigger, global::Doroti.Framework.Services.LogicalKeyboardKey.controlLeft)) && !DartRuntimePrimitives.Identical(trigger, global::Doroti.Framework.Services.LogicalKeyboardKey.controlRight)) && !DartRuntimePrimitives.Identical(trigger, global::Doroti.Framework.Services.LogicalKeyboardKey.shift)) && !DartRuntimePrimitives.Identical(trigger, global::Doroti.Framework.Services.LogicalKeyboardKey.shiftLeft)) && !DartRuntimePrimitives.Identical(trigger, global::Doroti.Framework.Services.LogicalKeyboardKey.shiftRight)) && !DartRuntimePrimitives.Identical(trigger, global::Doroti.Framework.Services.LogicalKeyboardKey.alt)) && !DartRuntimePrimitives.Identical(trigger, global::Doroti.Framework.Services.LogicalKeyboardKey.altLeft)) && !DartRuntimePrimitives.Identical(trigger, global::Doroti.Framework.Services.LogicalKeyboardKey.altRight)) && !DartRuntimePrimitives.Identical(trigger, global::Doroti.Framework.Services.LogicalKeyboardKey.meta)) && !DartRuntimePrimitives.Identical(trigger, global::Doroti.Framework.Services.LogicalKeyboardKey.metaLeft)) && !DartRuntimePrimitives.Identical(trigger, global::Doroti.Framework.Services.LogicalKeyboardKey.metaRight)));
    }

    public override IEnumerable<global::Doroti.Framework.Services.LogicalKeyboardKey>? triggers => DartRuntimePrimitives.ConvertValue<IEnumerable<global::Doroti.Framework.Services.LogicalKeyboardKey>>(new List<global::Doroti.Framework.Services.LogicalKeyboardKey> { this.trigger });
    internal virtual bool _shouldAcceptModifiers(HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> pressed)
    {
        return ((((this.control == System.Linq.Enumerable.Any(pressed.intersection(ShortcutsLibrary._controlSynonyms))) && (this.shift == System.Linq.Enumerable.Any(pressed.intersection(ShortcutsLibrary._shiftSynonyms)))) && (this.alt == System.Linq.Enumerable.Any(pressed.intersection(ShortcutsLibrary._altSynonyms)))) && (this.meta == System.Linq.Enumerable.Any(pressed.intersection(ShortcutsLibrary._metaSynonyms))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _shouldAcceptNumLock(global::Doroti.Framework.Services.HardwareKeyboard state)
    {
        return (this.numLock switch { LockState.ignored => true, LockState.locked => ((global::Doroti.Framework.Services.HardwareKeyboard)state).lockModesEnabled.Contains(global::Doroti.Framework.Services.KeyboardLockMode.numLock), LockState.unlocked => !((global::Doroti.Framework.Services.HardwareKeyboard)state).lockModesEnabled.Contains(global::Doroti.Framework.Services.KeyboardLockMode.numLock), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool accepts(global::Doroti.Framework.Services.KeyEvent @event, global::Doroti.Framework.Services.HardwareKeyboard state)
    {
        return ((((((@event is global::Doroti.Framework.Services.KeyDownEvent) || ((this.includeRepeats && (@event is global::Doroti.Framework.Services.KeyRepeatEvent))))) && this.triggers.contains(((global::Doroti.Framework.Services.KeyEvent)@event).logicalKey)) && _shouldAcceptModifiers(((global::Doroti.Framework.Services.HardwareKeyboard)state).logicalKeysPressed)) && _shouldAcceptNumLock(state));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ShortcutSerialization serializeForMenu()
    {
        return ShortcutSerialization.CreateModifier(this.trigger, shift: this.shift, alt: this.alt, meta: this.meta, control: this.control);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string debugDescribeKeys()
    {
        var result__23513 = "";
        DartRuntimePrimitives.Assert(() =>
            {
                var keys__23554 = new List<string> { ((((global::Doroti.Framework.Services.LogicalKeyboardKey)this.trigger).debugName ?? (string)((Diagnosticable)this.trigger).toStringShort())) };
                result__23513 = string.Join(" + ", keys__23554);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return result__23513;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.MessageProperty("keys", debugDescribeKeys()));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("includeRepeats", value: this.includeRepeats, ifFalse: "excluding repeats"));
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString__105654 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString__105654 = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((fullString__105654 ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
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

    public override IEnumerable<global::Doroti.Framework.Services.LogicalKeyboardKey>? triggers => DartRuntimePrimitives.ConvertValue<IEnumerable<global::Doroti.Framework.Services.LogicalKeyboardKey>>(null);
    internal virtual bool _shouldAcceptModifiers(HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> pressed)
    {
        return (((this.control == System.Linq.Enumerable.Any(pressed.intersection(ShortcutsLibrary._controlSynonyms))) && (this.alt == System.Linq.Enumerable.Any(pressed.intersection(ShortcutsLibrary._altSynonyms)))) && (this.meta == System.Linq.Enumerable.Any(pressed.intersection(ShortcutsLibrary._metaSynonyms))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool accepts(global::Doroti.Framework.Services.KeyEvent @event, global::Doroti.Framework.Services.HardwareKeyboard state)
    {
        return (((((global::Doroti.Framework.Services.KeyEvent)@event).character == this.character) && (((@event is global::Doroti.Framework.Services.KeyDownEvent) || ((this.includeRepeats && (@event is global::Doroti.Framework.Services.KeyRepeatEvent)))))) && _shouldAcceptModifiers(((global::Doroti.Framework.Services.HardwareKeyboard)state).logicalKeysPressed));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string debugDescribeKeys()
    {
        var result__30438 = "";
        DartRuntimePrimitives.Assert(() =>
            {
                var keys__30479 = new List<string> { $"'{this.character}'" };
                result__30438 = string.Join(" + ", keys__30479);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return result__30438;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ShortcutSerialization serializeForMenu()
    {
        return new ShortcutSerialization(this.character, alt: this.alt, control: this.control, meta: this.meta);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.MessageProperty("character", debugDescribeKeys()));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("includeRepeats", value: this.includeRepeats, ifFalse: "excluding repeats"));
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString__105654 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString__105654 = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((fullString__105654 ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
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
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<string>("activator", this.activator.debugDescribeKeys()));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Intent>("intent", this.intent));
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString__105654 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString__105654 = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((fullString__105654 ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
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
        this._shortcuts = __shortcuts;
    }

    public virtual DartMap<ShortcutActivator, Intent> shortcuts
    {
        get => this._shortcuts;
        set
        {
            var __value = value;
            if (!global::Doroti.Framework.Foundation.CollectionsLibrary.mapEquals<ShortcutActivator, Intent>(this._shortcuts, __value))
            {
                _shortcuts = __value;
                _indexedShortcutsCache = null;
                notifyListeners();
            }
        }
    }
    internal static DartMap<global::Doroti.Framework.Services.LogicalKeyboardKey?, List<_ActivatorIntentPair__shortcuts>> _indexShortcuts(DartMap<ShortcutActivator, Intent> source)
    {
        var result__33702 = new DartMap<global::Doroti.Framework.Services.LogicalKeyboardKey?, List<_ActivatorIntentPair__shortcuts>>();
        source.forEach(((global::System.Action<ShortcutActivator, Intent>)((activator, intent) =>
        {
            IEnumerable<global::Doroti.Framework.Services.LogicalKeyboardKey?>? nullableTriggers__33952 = ((IEnumerable<global::Doroti.Framework.Services.LogicalKeyboardKey?>?)(object?)((ShortcutActivator)activator).triggers);
            foreach (global::Doroti.Framework.Services.LogicalKeyboardKey? trigger__34028 in (nullableTriggers__33952 ?? new List<global::Doroti.Framework.Services.LogicalKeyboardKey?> { null }))
            {
                result__33702.putIfAbsent(trigger__34028, (() => new List<_ActivatorIntentPair__shortcuts>())).Add(new _ActivatorIntentPair__shortcuts(activator, intent));
            }
        })));
        return result__33702;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual DartMap<global::Doroti.Framework.Services.LogicalKeyboardKey?, List<_ActivatorIntentPair__shortcuts>> _indexedShortcuts
    {
        get
        {
            return _indexedShortcutsCache ??= ShortcutManager._indexShortcuts(this.shortcuts);
            return default!;
        }
    }
    internal virtual IEnumerable<_ActivatorIntentPair__shortcuts> _getCandidates(global::Doroti.Framework.Services.LogicalKeyboardKey key)
    {
        return ((IEnumerable<_ActivatorIntentPair__shortcuts>)(object?)new List<_ActivatorIntentPair__shortcuts>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Intent? _find(global::Doroti.Framework.Services.KeyEvent @event, global::Doroti.Framework.Services.HardwareKeyboard state)
    {
        foreach (_ActivatorIntentPair__shortcuts activatorIntent__35006 in _getCandidates(((global::Doroti.Framework.Services.KeyEvent)@event).logicalKey))
        {
            if (((_ActivatorIntentPair__shortcuts)activatorIntent__35006).activator.accepts(@event, state))
            {
                return ((_ActivatorIntentPair__shortcuts)activatorIntent__35006).intent;
            }
        }
        return ((Intent)(object)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual KeyEventResult handleKeypress(BuildContext context, global::Doroti.Framework.Services.KeyEvent @event)
    {
        Intent? intent__36280 = ((Intent?)(object?)_find(@event, global::Doroti.Framework.Services.HardwareKeyboard.instance));
        BuildContext? context__36354 = global::Doroti.Framework.Widgets.Focus_managerLibrary.primaryFocus?.context;
        dynamic action__36418 = Actions.maybeFind<Intent>(context__36354!, intent: intent__36280);
        if ((((intent__36280 is not null) && (context__36354 is not null)) && (action__36418 is not null)))
        {
            var (enabled__36562, invokeResult__36579) = DartRuntimePrimitives.ConvertValue<(bool, object?)>((object)Actions.of(context__36354).invokeActionIfEnabled(action__36418, intent__36280, context__36354));
            if (enabled__36562)
            {
                return ((KeyEventResult)((dynamic)action__36418).toKeyEventResult(intent__36280, invokeResult__36579));
            }
        }
        return (this.modal ? KeyEventResult.skipRemainingHandlers : KeyEventResult.ignored);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<DartMap<ShortcutActivator, Intent>>("shortcuts", this.shortcuts));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("modal", value: this.modal, ifTrue: "modal", defaultValue: false));
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString__105654 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString__105654 = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((fullString__105654 ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
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
        this._shortcuts = shortcuts;
        this.manager = null;
    }

    public static Shortcuts CreateManager(global::Doroti.Framework.Foundation.Key? key = null, ShortcutManager manager = default!, Widget child = default!, string? debugLabel = null, bool includeSemantics = true)
    {
        var __instance = new Shortcuts(default!, default!, default!, default!, default!);
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
            return ((this.manager is null) ? this._shortcuts : this.manager!.shortcuts);
            return default!;
        }
    }
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ShortcutsState__shortcuts());
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ShortcutManager>("manager", this.manager, defaultValue: null));
        properties.add(new ShortcutMapProperty("shortcuts", this.shortcuts, description: (((this.debugLabel is null ? (bool?)null : this.debugLabel.Length != 0) ?? false) ? this.debugLabel : null)));
    }

}

internal class _ShortcutsState__shortcuts : State<Shortcuts>
{
    internal virtual ShortcutManager? _internalManager { get; set; } = default;

    public virtual ShortcutManager manager => DartRuntimePrimitives.ConvertValue<ShortcutManager>((((Shortcuts)(object)this.widget).manager ?? this._internalManager!));
    public override void dispose()
    {
        this._internalManager?.dispose();
        base.dispose();
    }

    public override void initState()
    {
        base.initState();
        if ((((Shortcuts)(object)this.widget).manager is null))
        {
            _internalManager = new ShortcutManager();
            this._internalManager!.shortcuts = ((Shortcuts)(object)this.widget).shortcuts;
        }
    }

    public override void didUpdateWidget(Shortcuts oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((Shortcuts)(object)this.widget).manager, ((Shortcuts)oldWidget).manager)))
        {
            if ((((Shortcuts)(object)this.widget).manager is not null))
            {
                this._internalManager?.dispose();
                _internalManager = null;
            }
            else
            {
                _internalManager ??= new ShortcutManager();
            }
        }
        this._internalManager?.shortcuts = ((Shortcuts)(object)this.widget).shortcuts;
    }

    internal virtual KeyEventResult _handleOnKeyEvent(FocusNode node, global::Doroti.Framework.Services.KeyEvent @event)
    {
        if ((((FocusNode)node).context is null))
        {
            return KeyEventResult.ignored;
        }
        return this.manager.handleKeypress(((FocusNode)node).context!, @event);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new Focus(debugLabel: ((((Shortcuts)(object)this.widget).debugLabel is not null) ? $"{typeof(Shortcuts)}: {((Shortcuts)(object)this.widget).debugLabel}" : $"{typeof(Shortcuts)}"), canRequestFocus: false, onKeyEvent: (global::System.Func<FocusNode, global::Doroti.Framework.Services.KeyEvent, KeyEventResult>)this._handleOnKeyEvent, includeSemantics: ((Shortcuts)(object)this.widget).includeSemantics, child: ((Shortcuts)(object)this.widget).child));
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
        if (activator.accepts(@event, global::Doroti.Framework.Services.HardwareKeyboard.instance))
        {
            this.bindings.GetValueOrDefault(activator)!?.Invoke();
            return true;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new Focus(canRequestFocus: false, skipTraversal: true, onKeyEvent: ((global::System.Func<FocusNode, global::Doroti.Framework.Services.KeyEvent, KeyEventResult>?)((node, @event) =>
        {
            KeyEventResult result__47674 = KeyEventResult.ignored;
            foreach (ShortcutActivator activator__47830 in this.bindings.Keys)
            {
                result__47674 = (_applyKeyEventBinding(activator__47830, @event) ? KeyEventResult.handled : result__47674);
            }
            return result__47674;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), child: this.child));
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
        this.registry._replaceAll(this, value);
    }

    public virtual void dispose()
    {
        this.registry._disposeEntry(this);
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

    public virtual void dispose()
    {
        base.dispose();
        _disposed = true;
    }

    public virtual DartMap<ShortcutActivator, Intent> shortcuts
    {
        get
        {
            DartRuntimePrimitives.Assert(() => ChangeNotifier.debugAssertNotDisposed(this));
            return new DartMap<ShortcutActivator, Intent>();
            return default!;
        }
    }
    public virtual ShortcutRegistryEntry addAll(DartMap<ShortcutActivator, Intent> value)
    {
        DartRuntimePrimitives.Assert(() => ChangeNotifier.debugAssertNotDisposed(this));
        DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(value), () => (object?)"Cannot register an empty map of shortcuts");
        var entry__52514 = new ShortcutRegistryEntry(this);
        this._registeredShortcuts[entry__52514] = value;
        DartRuntimePrimitives.Assert(() => _debugCheckForDuplicates());
        _notifyListenersNextFrame();
        return entry__52514;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _notifyListenersNextFrame()
    {
        if (!this._notificationScheduled)
        {
            global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) =>
            {
                _notificationScheduled = false;
                if (!this._disposed)
                {
                    notifyListeners();
                }
            })), debugLabel: "ShortcutRegistry.notifyListeners");
            _notificationScheduled = true;
        }
    }

    public static ShortcutRegistry of(BuildContext context)
    {
        _ShortcutRegistrarScope__shortcuts? inherited__54094 = ((_ShortcutRegistrarScope__shortcuts?)(object?)context.dependOnInheritedWidgetOfExactType<_ShortcutRegistrarScope__shortcuts>());
        DartRuntimePrimitives.Assert(() =>
            {
                if ((inherited__54094 is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create($"Unable to find a {typeof(ShortcutRegistrar)} widget in the context.\n" + $"{typeof(ShortcutRegistrar)}.of() was called with a context that does not contain a " + $"{typeof(ShortcutRegistrar)} widget.\n" + $"No {typeof(ShortcutRegistrar)} ancestor could be found starting from the context that was " + $"passed to {typeof(ShortcutRegistrar)}.of().\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return inherited__54094!.registry;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ShortcutRegistry? maybeOf(BuildContext context)
    {
        _ShortcutRegistrarScope__shortcuts? inherited__55479 = ((_ShortcutRegistrarScope__shortcuts?)(object?)context.dependOnInheritedWidgetOfExactType<_ShortcutRegistrarScope__shortcuts>());
        return inherited__55479?.registry;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _replaceAll(ShortcutRegistryEntry entry, DartMap<ShortcutActivator, Intent> value)
    {
        DartRuntimePrimitives.Assert(() => ChangeNotifier.debugAssertNotDisposed(this));
        DartRuntimePrimitives.Assert(() => _debugCheckEntryIsValid(entry));
        this._registeredShortcuts[entry] = value;
        DartRuntimePrimitives.Assert(() => _debugCheckForDuplicates());
        _notifyListenersNextFrame();
    }

    internal virtual void _disposeEntry(ShortcutRegistryEntry entry)
    {
        DartRuntimePrimitives.Assert(() => _debugCheckEntryIsValid(entry));
        if ((this._registeredShortcuts.remove(entry) is not null))
        {
            _notifyListenersNextFrame();
        }
    }

    internal virtual bool _debugCheckEntryIsValid(ShortcutRegistryEntry entry)
    {
        if (!this._registeredShortcuts.ContainsKey(entry))
        {
            if ((object.Equals(((ShortcutRegistryEntry)entry).registry, this)))
            {
                throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create($"entry {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(entry))} is invalid.\n" + "The entry has already been disposed of. Tokens are not valid after " + "dispose is called on them, and should no longer be used."));
            }
            else
            {
                throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create($"Foreign entry {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(entry))} used.\n" + "This entry was not created by this registry, it was created by " + $"{(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(((ShortcutRegistryEntry)entry).registry))}, and should be used with that " + "registry instead."));
            }
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _debugCheckForDuplicates()
    {
        var previous__57070 = new DartMap<ShortcutActivator, ShortcutRegistryEntry?>();
        foreach (MapEntry<ShortcutRegistryEntry, DartMap<ShortcutActivator, Intent>> tokenEntry__57207 in this._registeredShortcuts.entries)
        {
            foreach (ShortcutActivator shortcut__57296 in tokenEntry__57207.value.Keys)
            {
                if (previous__57070.ContainsKey(shortcut__57296))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create($"{typeof(ShortcutRegistry)}: Received a duplicate registration for the " + $"shortcut {shortcut__57296} in {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(tokenEntry__57207.key))} and {previous__57070.GetValueOrDefault(shortcut__57296)}."));
                }
                previous__57070[shortcut__57296] = tokenEntry__57207.key;
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
        this.registry.addListener(() => this._shortcutsChanged());
    }

    internal virtual void _shortcutsChanged()
    {
        this.manager.shortcuts = ((ShortcutRegistry)this.registry).shortcuts;
    }

    public override void dispose()
    {
        this.registry.removeListener(() => this._shortcutsChanged());
        this.registry.dispose();
        this.manager.dispose();
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new _ShortcutRegistrarScope__shortcuts(registry: this.registry, child: Shortcuts.CreateManager(manager: this.manager, debugLabel: "<Shortcut Registrar>", child: ((ShortcutRegistrar)(object)this.widget).child)));
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
        var __oldWidget = (_ShortcutRegistrarScope__shortcuts)(object)oldWidget;
        return (!object.Equals(this.registry, ((_ShortcutRegistrarScope__shortcuts)__oldWidget).registry));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

