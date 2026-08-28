using Doroti.Ui;
using Doroti.Framework.Foundation;
using Doroti.Framework.Widgets;

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

var metadataNode = button with { hint = "Saves the document", headingLevel = 2, identifier = "save-action" };
var metadataDelta = SemanticsUpdateDiffer.Diff(baseline, [root, metadataNode]);
Require(metadataDelta.RequiresImmediateFlush, "assistive metadata changes flush immediately");
Require(metadataDelta.changedNodes.Single(delta => delta.id == 1).changedProperties.HasFlag(SemanticsNodeProperty.metadata),
    "assistive metadata is retained as its own delta class");

var localRoot = Node(20, 10, 20, 100, 100, "projection root", children: [21]);
var localChild = Node(21, 5, 7, 40, 20, "projection child", children: [22]);
var localGrandchild = Node(22, 2, 3, 10, 8, "projection grandchild");
var viewCoordinates = SemanticsGeometryProjection.ToViewCoordinates([localRoot, localChild, localGrandchild]);
Require(viewCoordinates.Single(node => node.id == 20).rect == Rect.fromLTWH(10, 20, 100, 100),
    "root semantics bounds stay in view coordinates");
Require(viewCoordinates.Single(node => node.id == 21).rect == Rect.fromLTWH(15, 27, 40, 20),
    "native hosts receive parent-relative child bounds projected into view coordinates");
Require(viewCoordinates.Single(node => node.id == 22).rect == Rect.fromLTWH(17, 30, 10, 8),
    "nested semantics bounds accumulate every parent origin");

var builder = new SemanticsUpdateBuilder();
builder.updateNode(
    id: 7,
    flags: new SemanticsFlags(isFocused: Tristate.isFalse, isTextField: true, isRequired: Tristate.isTrue),
    actions: (long)(SemanticsAction.setText | SemanticsAction.focus),
    rect: Rect.fromLTWH(2, 3, 40, 20),
    identifier: "email-field",
    label: "Email",
    labelAttributes: Array.Empty<object>(),
    value: "person@example.com",
    valueAttributes: Array.Empty<object>(),
    increasedValue: "",
    increasedValueAttributes: Array.Empty<object>(),
    decreasedValue: "",
    decreasedValueAttributes: Array.Empty<object>(),
    hint: "Work address",
    hintAttributes: Array.Empty<object>(),
    tooltip: "Used for notifications",
    textDirection: null,
    textSelectionBase: 2,
    textSelectionExtent: 4,
    platformViewId: -1,
    maxValueLength: 80,
    currentValueLength: 18,
    scrollChildren: -1,
    scrollIndex: -1,
    scrollPosition: double.NaN,
    scrollExtentMax: double.NaN,
    scrollExtentMin: double.NaN,
    transform: new double[] { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 },
    traversalParent: -1,
    hitTestTransform: Array.Empty<double>(),
    childrenInTraversalOrder: Array.Empty<int>(),
    childrenInHitTestOrder: Array.Empty<int>(),
    additionalActions: Array.Empty<int>(),
    headingLevel: 2,
    linkUrl: "mailto:person@example.com",
    role: SemanticsRole.form,
    controlsNodes: ["submit-action"],
    validationResult: SemanticsValidationResult.invalid,
    hitTestBehavior: SemanticsHitTestBehavior.opaque,
    inputType: SemanticsInputType.email,
    locale: new Locale("ko", "KR"),
    minValue: "",
    maxValue: "");
var projected = builder.build(12).nodes.Single();
Require(projected.identifier == "email-field" && projected.hint == "Work address" &&
        projected.tooltip == "Used for notifications" && projected.headingLevel == 2 &&
        projected.linkUrl == "mailto:person@example.com" && projected.validationResult == SemanticsValidationResult.invalid &&
        projected.hitTestBehavior == SemanticsHitTestBehavior.opaque && projected.inputType == SemanticsInputType.email &&
        projected.maxValueLength == 80 && projected.currentValueLength == 18 &&
        projected.controlsNodes!.SequenceEqual(["submit-action"]) && projected.locale == new Locale("ko", "KR"),
    "the Flutter semantics builder preserves host accessibility metadata");

var focusNode = new FocusNode();
Require(Focus.CreateSemanticsFocusAction(TargetPlatform.windows, false, focusNode) is null,
    "a non-focusable semantics node does not publish a callable focus action");
var focusAction = Focus.CreateSemanticsFocusAction(TargetPlatform.windows, true, focusNode);
Require(focusAction is not null, "a focusable Windows semantics node publishes a focus action");
focusAction!();
Require(Focus.CreateSemanticsFocusAction(TargetPlatform.iOS, true, focusNode) is null,
    "iOS keeps Flutter's semantics focus-action exclusion");
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
