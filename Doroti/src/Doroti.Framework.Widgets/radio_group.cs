// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/radio_group.dart
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

public static class RadioGroup
{
    public static RadioGroupRegistry<T>? maybeOf<T>(BuildContext context) => RadioGroup<T>.maybeOf<T>(context);
}

public class RadioGroup<T> : StatefulWidget
{
    public virtual T? groupValue { get; private set; }
    public virtual global::System.Action<T?> onChanged { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public RadioGroup(global::Doroti.Framework.Foundation.Key? key = null, T? groupValue = default, global::System.Action<T?> onChanged = default!, Widget child = default!) : base(key: key)
    {
        this.groupValue = groupValue;
        this.onChanged = onChanged;
        this.child = child;
    }

    public static RadioGroupRegistry<T>? maybeOf<T>(BuildContext context)
    {
        return ((RadioGroupRegistry<T>?)(object?)DartRuntimePrimitives.NullAware(context.dependOnInheritedWidgetOfExactType<_RadioGroupStateScope__radio_group<T>>(), __target => __target.state));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _RadioGroupState__radio_group<T>());
}

internal class _RadioGroupState__radio_group<T> : State<RadioGroup<T>>, RadioGroupRegistry<T>
{
    private bool __late__radioGroupShortcuts_initialized;
    private DartMap<ShortcutActivator, Intent> __late__radioGroupShortcuts = default!;
    internal virtual DartMap<ShortcutActivator, Intent> _radioGroupShortcuts
    {
        get
        {
            if (!__late__radioGroupShortcuts_initialized)
            {
                __late__radioGroupShortcuts = new DartMap<ShortcutActivator, Intent> { [new SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.arrowLeft)] = ((Intent)(object?)new VoidCallbackIntent(() => this._selectPreviousRadio())), [new SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.arrowRight)] = ((Intent)(object?)new VoidCallbackIntent(() => this._selectNextRadio())), [new SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.arrowDown)] = ((Intent)(object?)new VoidCallbackIntent(() => this._selectNextRadio())), [new SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.arrowUp)] = ((Intent)(object?)new VoidCallbackIntent(() => this._selectPreviousRadio())), [new SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.space)] = ((Intent)(object?)new VoidCallbackIntent(() => this._toggleFocusedRadio())) };
                __late__radioGroupShortcuts_initialized = true;
            }
            return __late__radioGroupShortcuts;
        }
    }
    private bool __late__radioGroupShortcutManager_initialized;
    private _RadioGroupShortcutManager__radio_group<T> __late__radioGroupShortcutManager = default!;
    internal virtual _RadioGroupShortcutManager__radio_group<T> _radioGroupShortcutManager
    {
        get
        {
            if (!__late__radioGroupShortcutManager_initialized)
            {
                __late__radioGroupShortcutManager = new _RadioGroupShortcutManager__radio_group<T>(shortcuts: this._radioGroupShortcuts, state: this);
                __late__radioGroupShortcutManager_initialized = true;
            }
            return __late__radioGroupShortcutManager;
        }
    }
    internal virtual HashSet<RadioClient<T>> _radios { get; private set; } = new HashSet<RadioClient<T>>();
    internal virtual bool _debugHasScheduledSingleSelectionCheck { get; set; } = false;

    internal virtual bool _debugScheduleSingleSelectionCheck()
    {
        if (this._debugHasScheduledSingleSelectionCheck)
        {
            return true;
        }
        WidgetsBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) =>
        {
            _debugHasScheduledSingleSelectionCheck = false;
            if ((!this.mounted || _debugCheckOnlySingleSelection()))
            {
                return;
            }
            throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("RadioGroupPolicy can't be used for a radio group that allows multiple selection."));
        })), debugLabel: "RadioGroup.singleSelectionCheck");
        _debugHasScheduledSingleSelectionCheck = true;
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _debugCheckOnlySingleSelection()
    {
        return (this._radios.where(((radio) => EqualityComparer<T>.Default.Equals(((RadioClient<T>)radio).radioValue, this.groupValue))).Count() < 2L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual T? groupValue => ((RadioGroup<T>)(object)this.widget).groupValue;
    public override void dispose()
    {
        this._radioGroupShortcutManager.dispose();
        base.dispose();
    }

    public virtual void registerClient(RadioClient<T> radio)
    {
        this._radios.Add(radio);
        DartRuntimePrimitives.Assert(() => _debugScheduleSingleSelectionCheck());
    }

    public virtual void unregisterClient(RadioClient<T> radio) => this._radios.Remove(radio);
    internal virtual void _toggleFocusedRadio()
    {
        RadioClient<T>? radio__5357 = this._radios.firstWhereOrNull(((radio) => ((RadioClient<T>)radio).focusNode.hasFocus));
        if ((radio__5357 is null))
        {
            return;
        }
        if (!EqualityComparer<T>.Default.Equals(((RadioClient<T>)radio__5357).radioValue, ((RadioGroup<T>)(object)this.widget).groupValue))
        {
            this.onChanged(((RadioClient<T>)radio__5357).radioValue);
            return;
        }
        if (((RadioClient<T>)radio__5357).tristate)
        {
            this.onChanged(default);
        }
    }

    public virtual global::System.Action<T?> onChanged => ((RadioGroup<T>)(object)this.widget).onChanged;
    internal virtual void _selectNextRadio() => _selectRadioInDirection(true);
    internal virtual void _selectPreviousRadio() => _selectRadioInDirection(false);
    internal virtual void _selectRadioInDirection(bool forward)
    {
        if ((checked((long)(this._radios.Count)) < 2L))
        {
            return;
        }
        FocusNode? currentFocus__5978 = this._radios.firstWhereOrNull(((radio) => ((RadioClient<T>)radio).focusNode.hasFocus))?.focusNode;
        if ((currentFocus__5978 is null))
        {
            return;
        }
        List<FocusNode> sorted__6257 = ReadingOrderTraversalPolicy.sort(this._radios.where(((radio) => ((RadioClient<T>)radio).enabled)).map<RadioClient<T>, FocusNode>(((radio) => ((RadioClient<T>)radio).focusNode)).Cast<FocusNode>()).ToList().ToList();
        DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(sorted__6257));
        IEnumerable<FocusNode> nodesInEffectiveOrder__6519 = (forward ? sorted__6257 : System.Linq.Enumerable.Reverse(sorted__6257));
        IEnumerator<FocusNode> iterator__6610 = nodesInEffectiveOrder__6519.GetEnumerator();
        FocusNode? nextFocus__6668 = default!;
        while (iterator__6610.MoveNext())
        {
            if ((object.Equals(iterator__6610.Current, currentFocus__5978)))
            {
                if (iterator__6610.MoveNext())
                {
                    nextFocus__6668 = iterator__6610.Current;
                }
                break;
            }
        }
        nextFocus__6668 ??= nodesInEffectiveOrder__6519.First();
        RadioClient<T> radioToSelect__7016 = this._radios.firstWhere(((radio) => (object.Equals(((RadioClient<T>)radio).focusNode, nextFocus__6668))));
        this.onChanged(((RadioClient<T>)radioToSelect__7016).radioValue);
        nextFocus__6668.requestFocus();
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => _debugScheduleSingleSelectionCheck());
        return ((Widget)(object?)new Semantics(container: true, role: SemanticsRole.radioGroup, child: Shortcuts.CreateManager(manager: this._radioGroupShortcutManager, child: new FocusTraversalGroup(policy: new _SkipUnselectedRadioPolicy__radio_group<T>(this._radios, ((RadioGroup<T>)(object)this.widget).groupValue), child: new _RadioGroupStateScope__radio_group<T>(state: this, groupValue: ((RadioGroup<T>)(object)this.widget).groupValue, child: ((RadioGroup<T>)(object)this.widget).child)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _RadioGroupShortcutManager__radio_group<T> : ShortcutManager
{
    public virtual _RadioGroupState__radio_group<T> state { get; private set; } = default!;

    internal _RadioGroupShortcutManager__radio_group(DartMap<ShortcutActivator, Intent> shortcuts, _RadioGroupState__radio_group<T> state) : base(shortcuts: shortcuts)
    {
        this.state = state;
    }

    public override KeyEventResult handleKeypress(BuildContext context, global::Doroti.Framework.Services.KeyEvent @event)
    {
        bool radioHasFocus__8040 = ((_RadioGroupState__radio_group<T>)this.state)._radios.any(((radio) => ((RadioClient<T>)radio).focusNode.hasFocus));
        if (!radioHasFocus__8040)
        {
            return KeyEventResult.ignored;
        }
        return base.handleKeypress(context, @event);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _RadioGroupStateScope__radio_group<T> : InheritedWidget
{
    public virtual _RadioGroupState__radio_group<T> state { get; private set; } = default!;
    public virtual T? groupValue { get; private set; }

    internal _RadioGroupStateScope__radio_group(_RadioGroupState__radio_group<T> state, T? groupValue, Widget child) : base(child)
    {
        this.state = state;
        this.groupValue = groupValue;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (_RadioGroupStateScope__radio_group<T>)(object)oldWidget;
        return ((!object.Equals(this.state, ((_RadioGroupStateScope__radio_group<T>)__oldWidget).state)) || !EqualityComparer<T>.Default.Equals(this.groupValue, ((_RadioGroupStateScope__radio_group<T>)__oldWidget).groupValue));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public interface RadioGroupRegistry<T>
{
    public T? groupValue { get; }
    public void registerClient(RadioClient<T> radio);
    public void unregisterClient(RadioClient<T> radio);
    public global::System.Action<T?> onChanged { get; }
}

public interface RadioClient<T>
{
    RadioGroupRegistry<T>? _registry { get; set; }

    public bool tristate { get; }
    public T radioValue { get; }
    public bool enabled { get; }
    public FocusNode focusNode { get; }
    public RadioGroupRegistry<T>? registry { get; set; }
}

internal class _SkipUnselectedRadioPolicy__radio_group<T> : ReadingOrderTraversalPolicy
{
    public virtual HashSet<RadioClient<T>> radios { get; private set; } = default!;
    public virtual T? groupValue { get; private set; }

    internal _SkipUnselectedRadioPolicy__radio_group(HashSet<RadioClient<T>> radios, T? groupValue)
    {
        this.radios = radios;
        this.groupValue = groupValue;
    }

    internal virtual bool _radioSelected(RadioClient<T> radio) => DartRuntimePrimitives.ConvertValue<bool>(EqualityComparer<T>.Default.Equals(((RadioClient<T>)radio).radioValue, this.groupValue));
    public override IEnumerable<FocusNode> sortDescendants(IEnumerable<FocusNode> descendants, FocusNode currentNode)
    {
        IEnumerable<FocusNode> nodesInReadOrder__12004 = ((IEnumerable<FocusNode>)(object?)base.sortDescendants(descendants.Cast<FocusNode>(), currentNode));
        RadioClient<T>? selected__12092 = this.radios.firstWhereOrNull(this._radioSelected);
        if ((selected__12092 is null))
        {
            var radioFocusNodes__12264 = new DartMap<FocusNode, RadioClient<T>>();
            foreach (RadioClient<T> radio__12345 in this.radios)
            {
                radioFocusNodes__12264[((RadioClient<T>)radio__12345).focusNode] = radio__12345;
            }
            foreach (var node__12440 in nodesInReadOrder__12004)
            {
                selected__12092 = radioFocusNodes__12264.GetValueOrDefault(node__12440);
                if ((selected__12092 is not null))
                {
                    break;
                }
            }
        }
        if ((selected__12092 is null))
        {
            return nodesInReadOrder__12004;
        }
        HashSet<FocusNode> nodeToSkip__12886 = this.radios.where(((radio) => ((!object.Equals(selected__12092, radio)) && (!object.Equals(((RadioClient<T>)radio).focusNode, currentNode))))).map<RadioClient<T>, FocusNode>(((radio) => ((RadioClient<T>)radio).focusNode)).toSet();
        IEnumerable<FocusNode> skipsNonSelected__13115 = descendants.where(((node) => !nodeToSkip__12886.Contains(node)));
        return ((IEnumerable<FocusNode>)(object?)base.sortDescendants(skipsNonSelected__13115.Cast<FocusNode>(), currentNode));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
