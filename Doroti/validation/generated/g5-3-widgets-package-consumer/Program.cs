using Doroti.Generated.Framework.Services;
using Doroti.Generated.Framework.Widgets;

var builds = 0;
var owner = new BuildOwner(focusManager: new FocusManager());
var root = new RootWidget(child: new ExternalDartApp(() => builds++));
var element = root.attach(owner);
owner.buildScope(element);
new RootWidget(child: new ExternalDartApp(() => builds++)).attach(owner, element);
owner.buildScope(element);
if (builds != 2) throw new InvalidOperationException($"Generated application rebuild drifted: {builds}.");

using var controller = new TextEditingController("doroti");
controller.selection = TextSelection.CreateCollapsed(3);
if (controller.text != "doroti" || controller.selection.start != 3)
{
    throw new InvalidOperationException("EditableText package contract drifted.");
}

var itemBuilds = 0;
var list = new SliverChildBuilderDelegate(
    (_, _) => { itemBuilds++; return new ExternalLeaf(); },
    childCount: 1000,
    addAutomaticKeepAlives: false,
    addRepaintBoundaries: false,
    addSemanticIndexes: false);
for (var index = 0L; index < 1000; index++)
{
    if (list.build(element, index) is null) throw new InvalidOperationException($"List item {index} was omitted.");
}
if (itemBuilds != 1000 || list.build(element, 1000) is not null)
{
    throw new InvalidOperationException("Long-list boundary drifted.");
}

element.deactivate();
element.unmount();
Console.WriteLine("G5-3-WIDGETS-PACKAGE-CONSUMER-PASS");

// This is the generated package-consumer shape of validation/cases/g5-3-dart-app/main.dart.
sealed class ExternalDartApp(Action onBuild) : StatelessWidget
{
    public override Widget build(BuildContext context)
    {
        onBuild();
        return new ExternalLeaf();
    }
}

sealed class ExternalLeaf : Widget
{
    public override Element createElement() => new ExternalLeafElement(this);
}

sealed class ExternalLeafElement(Widget widget) : Element(widget);
