using Doroti.Ui;

var root = Node(0, 0, 0, 100, 100, "root", children: [1]);
var button = Node(1, 0, 0, 40, 20, "Save", SemanticsAction.tap);
var baseline = new Dictionary<int, SemanticsNodeUpdate> { [0] = root, [1] = button };

var geometry = new[]
{
    root with { rect = Rect.fromLTWH(0, -12, 100, 100) },
    button with { rect = Rect.fromLTWH(0, -12, 40, 20) },
};
var geometryDelta = SemanticsUpdateDiffer.Diff(baseline, geometry);
Require(geometryDelta.HasChanges && geometryDelta.IsGeometryOnly, "scroll bounds-only delta is coalescible");
Require(!geometryDelta.RequiresImmediateFlush, "bounds-only delta is not interaction critical");
Require(geometryDelta.changedNodes.All(delta => delta.previousContentHash == delta.contentHash), "content hash excludes geometry");

var labelDelta = SemanticsUpdateDiffer.Diff(baseline, [root, button with { label = "Saved" }]);
Require(labelDelta.RequiresImmediateFlush, "label update flushes immediately");
Require(labelDelta.changedNodes.Single(delta => delta.id == 1).changedProperties.HasFlag(SemanticsNodeProperty.label), "label delta is exact");

var selectionDelta = SemanticsUpdateDiffer.Diff(baseline, [root, button with { textSelectionBase = 1, textSelectionExtent = 4 }]);
Require(selectionDelta.RequiresImmediateFlush, "selection update flushes immediately");
Require(selectionDelta.changedNodes.Single(delta => delta.id == 1).changedProperties.HasFlag(SemanticsNodeProperty.selection), "selection delta is exact");

var removedDelta = SemanticsUpdateDiffer.Diff(baseline, [root with { children = [] }]);
Require(removedDelta.HasTopologyChange && !removedDelta.RequiresImmediateFlush,
    "virtualized topology churn remains bounded unless urgency is explicit");
Require(removedDelta.removedNodeIds.SequenceEqual([1]), "removed identity is retained for native pruning");

var inserted = Node(2, 0, 20, 40, 20, "Next", SemanticsAction.tap);
var insertedDelta = SemanticsUpdateDiffer.Diff(baseline,
    [root with { children = [1, 2] }, button, inserted]);
Require(insertedDelta.HasTopologyChange && !insertedDelta.RequiresImmediateFlush,
    "new virtualized nodes do not bypass the native apply interval");

var end = new SemanticsUpdate(9, geometry, SemanticsUpdateUrgency.scrollEnd);
Require(end.urgency == SemanticsUpdateUrgency.scrollEnd, "scroll end is an explicit immediate-host signal");
Console.WriteLine($"FCR-6 semantics runtime contract: PASS (configuration={ConfigurationName()})");

static SemanticsNodeUpdate Node(int id, double x, double y, double width, double height, string label,
    SemanticsAction actions = SemanticsAction.none, IReadOnlyList<int>? children = null) =>
    new(id, Rect.fromLTWH(x, y, width, height), label, null, actions, children ?? []);

static string ConfigurationName() =>
#if DEBUG
    "Debug";
#else
    "Release";
#endif

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}
