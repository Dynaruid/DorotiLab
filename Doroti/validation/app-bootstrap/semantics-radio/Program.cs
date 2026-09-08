using Doroti.Host.Maui;
using Doroti.Ui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using Rect = Doroti.Ui.Rect;

DispatcherProvider.SetCurrent(new InlineProvider());
var layer = new AbsoluteLayout();
using var bridge = new MauiSemanticsBridge(layer);
var actions = new List<(int Id, SemanticsAction Action)>();
var generation = 0L;
string[] labels = ["Day", "Week", "Month", "Year"];
void Require(bool condition, string message) { if (!condition) throw new Exception(message); }
void Project(int selected, bool anotherGroup = false)
{
    var nodes = labels.Select((label, index) => new SemanticsNodeUpdate(index + 1,
        Rect.fromLTWH(index * 80, 0, 80, 48), label, null, SemanticsAction.tap, [],
        new SemanticsFlags(isSelected: index + 1 == selected ? Tristate.isTrue : Tristate.isFalse,
            isEnabled: Tristate.isTrue, isInMutuallyExclusiveGroup: true))).ToList();
    if (anotherGroup) nodes.Add(new SemanticsNodeUpdate(5, Rect.fromLTWH(0, 80, 80, 48), "Other group", null,
        SemanticsAction.tap, [], new SemanticsFlags(isChecked: CheckedState.isTrue,
            isEnabled: Tristate.isTrue, isInMutuallyExclusiveGroup: true)));
    bridge.Update(new SemanticsUpdate(++generation, nodes, SemanticsUpdateUrgency.immediate),
        (id, action, _) => actions.Add((id, action)));
    Require(actions.Count == 0, $"Projecting selection {selected} emitted native actions: {string.Join(',', actions)}");
    var radios = layer.Children.Cast<RadioButton>().ToArray();
    for (var i = 0; i < 4; i++)
        Require(radios[i].IsChecked == (i + 1 == selected), $"Native radio {labels[i]} disagrees with framework selection {selected}");
    if (anotherGroup) Require(radios[4].IsChecked, "Unrelated radio group was deselected by native grouping");
}

// A later selected sibling must not turn a framework-driven deselection into
// another tap that restores the old value (Year -> Day was observed on Galaxy).
foreach (var selected in new[] { 4, 1, 3, 2, 4, 1 }) Project(selected);
Project(1, anotherGroup: true);
Project(4, anotherGroup: true);
Project(2);

// A genuine assistive activation still reaches the framework exactly once;
// unchecking a radio is not an activation of that radio.
var month = (RadioButton)layer.Children[2];
month.IsChecked = true;
Require(actions.SequenceEqual([(3, SemanticsAction.tap)]), "Native radio activation was lost or duplicated");
actions.Clear();
Project(3);
month.IsChecked = false;
Require(actions.Count == 0, "Native deselection was incorrectly forwarded as a tap");
Project(1);
bridge.Clear();
Require(layer.Children.Count == 0, "Semantics Clear retained native radios");
Project(4);
Project(1);
Console.WriteLine("MAUI semantics radios: reverse/forward selection, no projection feedback, independent groups, native activation, deselection and clear/recreate PASS");

sealed class InlineProvider : IDispatcherProvider
{
    public IDispatcher GetForCurrentThread() => new InlineDispatcher();
}
sealed class InlineDispatcher : IDispatcher
{
    public bool IsDispatchRequired => false;
    public bool Dispatch(Action action) { action(); return true; }
    public bool DispatchDelayed(TimeSpan delay, Action action) { action(); return true; }
    public IDispatcherTimer CreateTimer() => throw new NotSupportedException();
}
