// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/radio_group.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public static class RadioGroup
{
    public static RadioGroupRegistry<T>? maybeOf<T>(BuildContext context) => RadioGroup<T>.maybeOf<T>(context);
}

public class RadioGroup<T> : StatefulWidget
{
    public virtual T? groupValue { get; private set; }
    public virtual System.Action<T?> onChanged { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public RadioGroup(Key? key = null, T? groupValue = default, System.Action<T?> onChanged = default!, Widget child = default!) : base(key: key)
    {
        this.groupValue = groupValue;
        this.onChanged = onChanged;
        this.child = child;
    }

    public static RadioGroupRegistry<TValue>? maybeOf<TValue>(BuildContext context)
    {
        return DartRuntimePrimitives.NullAware(context.dependOnInheritedWidgetOfExactType<_RadioGroupStateScope__radio_group<TValue>>(), __target => __target.state);
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
                __late__radioGroupShortcuts = new DartMap<ShortcutActivator, Intent> { [new SingleActivator(LogicalKeyboardKey.arrowLeft)] = new VoidCallbackIntent(() => _selectPreviousRadio()), [new SingleActivator(LogicalKeyboardKey.arrowRight)] = new VoidCallbackIntent(() => _selectNextRadio()), [new SingleActivator(LogicalKeyboardKey.arrowDown)] = new VoidCallbackIntent(() => _selectNextRadio()), [new SingleActivator(LogicalKeyboardKey.arrowUp)] = new VoidCallbackIntent(() => _selectPreviousRadio()), [new SingleActivator(LogicalKeyboardKey.space)] = new VoidCallbackIntent(() => _toggleFocusedRadio()) };
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
                __late__radioGroupShortcutManager = new _RadioGroupShortcutManager__radio_group<T>(shortcuts: _radioGroupShortcuts, state: this);
                __late__radioGroupShortcutManager_initialized = true;
            }
            return __late__radioGroupShortcutManager;
        }
    }
    internal virtual HashSet<RadioClient<T>> _radios { get; private set; } = new HashSet<RadioClient<T>>();
    internal virtual bool _debugHasScheduledSingleSelectionCheck { get; set; } = false;

    internal virtual bool _debugScheduleSingleSelectionCheck()
    {
        if (_debugHasScheduledSingleSelectionCheck)
        {
            return true;
        }
        WidgetsBinding.instance.addPostFrameCallback((_) =>
        {
            _debugHasScheduledSingleSelectionCheck = false;
            if (!mounted || _debugCheckOnlySingleSelection())
            {
                return;
            }
            throw DartRuntimePrimitives.AsException(FlutterError.Create("RadioGroupPolicy can't be used for a radio group that allows multiple selection."));
        }, debugLabel: "RadioGroup.singleSelectionCheck");
        _debugHasScheduledSingleSelectionCheck = true;
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _debugCheckOnlySingleSelection()
    {
        return _radios.where((radio) => EqualityComparer<T>.Default.Equals(radio.radioValue, groupValue)).Count() < 2L;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual T? groupValue => widget.groupValue;
    public override void dispose()
    {
        _radioGroupShortcutManager.dispose();
        base.dispose();
    }

    public virtual void registerClient(RadioClient<T> radio)
    {
        _radios.Add(radio);
        DartRuntimePrimitives.Assert(() => _debugScheduleSingleSelectionCheck());
    }

    public virtual void unregisterClient(RadioClient<T> radio) => _radios.Remove(radio);
    internal virtual void _toggleFocusedRadio()
    {
        RadioClient<T>? radioLocal = _radios.firstWhereOrNull((radio) => radio.focusNode.hasFocus);
        if (radioLocal is null)
        {
            return;
        }
        if (!EqualityComparer<T>.Default.Equals(radioLocal.radioValue, widget.groupValue))
        {
            onChanged(radioLocal.radioValue);
            return;
        }
        if (radioLocal.tristate)
        {
            onChanged(default);
        }
    }

    public virtual System.Action<T?> onChanged => widget.onChanged;
    internal virtual void _selectNextRadio() => _selectRadioInDirection(true);
    internal virtual void _selectPreviousRadio() => _selectRadioInDirection(false);
    internal virtual void _selectRadioInDirection(bool forward)
    {
        if (checked(_radios.Count) < 2L)
        {
            return;
        }
        FocusNode? currentFocus = _radios.firstWhereOrNull((radio) => radio.focusNode.hasFocus)?.focusNode;
        if (currentFocus is null)
        {
            return;
        }
        List<FocusNode> sorted = ReadingOrderTraversalPolicy.sort(_radios.where((radio) => radio.enabled).map((radio) => radio.focusNode).Cast<FocusNode>()).ToList().ToList();
        DartRuntimePrimitives.Assert(() => Enumerable.Any(sorted));
        IEnumerable<FocusNode> nodesInEffectiveOrder = forward ? sorted : Enumerable.Reverse(sorted);
        IEnumerator<FocusNode> iterator = nodesInEffectiveOrder.GetEnumerator();
        FocusNode? nextFocus = default!;
        while (iterator.MoveNext())
        {
            if (Equals(iterator.Current, currentFocus))
            {
                if (iterator.MoveNext())
                {
                    nextFocus = iterator.Current;
                }
                break;
            }
        }
        nextFocus ??= nodesInEffectiveOrder.First();
        RadioClient<T> radioToSelect = _radios.firstWhere((radio) => Equals(radio.focusNode, nextFocus));
        onChanged(radioToSelect.radioValue);
        nextFocus.requestFocus();
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => _debugScheduleSingleSelectionCheck());
        return new Semantics(container: true, role: SemanticsRole.radioGroup, child: Shortcuts.CreateManager(manager: _radioGroupShortcutManager, child: new FocusTraversalGroup(policy: new _SkipUnselectedRadioPolicy__radio_group<T>(_radios, widget.groupValue), child: new _RadioGroupStateScope__radio_group<T>(state: this, groupValue: widget.groupValue, child: widget.child))));
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

    public override KeyEventResult handleKeypress(BuildContext context, KeyEvent @event)
    {
        bool radioHasFocus = state._radios.any((radio) => radio.focusNode.hasFocus);
        if (!radioHasFocus)
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
        var __oldWidget = (_RadioGroupStateScope__radio_group<T>)oldWidget;
        return (!Equals(state, __oldWidget.state)) || !EqualityComparer<T>.Default.Equals(groupValue, __oldWidget.groupValue);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public interface RadioGroupRegistry<T>
{
    public T? groupValue { get; }
    public void registerClient(RadioClient<T> radio);
    public void unregisterClient(RadioClient<T> radio);
    public System.Action<T?> onChanged { get; }
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

    internal virtual bool _radioSelected(RadioClient<T> radio) => DartRuntimePrimitives.ConvertValue<bool>(EqualityComparer<T>.Default.Equals(radio.radioValue, groupValue));
    public override IEnumerable<FocusNode> sortDescendants(IEnumerable<FocusNode> descendants, FocusNode currentNode)
    {
        IEnumerable<FocusNode> nodesInReadOrder = base.sortDescendants(descendants.Cast<FocusNode>(), currentNode);
        RadioClient<T>? selected = radios.firstWhereOrNull(_radioSelected);
        if (selected is null)
        {
            var radioFocusNodes = new DartMap<FocusNode, RadioClient<T>>();
            foreach (RadioClient<T> radioLocal in radios)
            {
                radioFocusNodes[radioLocal.focusNode] = radioLocal;
            }
            foreach (var nodeLocal in nodesInReadOrder)
            {
                selected = radioFocusNodes.GetValueOrDefault(nodeLocal);
                if (selected is not null)
                {
                    break;
                }
            }
        }
        if (selected is null)
        {
            return nodesInReadOrder;
        }
        HashSet<FocusNode> nodeToSkip = radios.where((radio) => (!Equals(selected, radio)) && (!Equals(radio.focusNode, currentNode))).map((radio) => radio.focusNode).toSet();
        IEnumerable<FocusNode> skipsNonSelected = descendants.where((node) => !nodeToSkip.Contains(node));
        return base.sortDescendants(skipsNonSelected.Cast<FocusNode>(), currentNode);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
