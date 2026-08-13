using Doroti.Generated.Application.Framework;
using Doroti.Generated.Framework.Widgets;

var generatedApp = new G53ExternalApp();
if (generatedApp is not StatelessWidget)
{
    throw new InvalidOperationException("The generated Dart application root did not bind to the promoted Widgets package.");
}

var buildCount = 0;
var owner = new BuildOwner(focusManager: new FocusManager());
var root = new RootWidget(child: new PackageProbe(() => buildCount++));
var element = root.attach(owner);
owner.buildScope(element);
if (buildCount != 1) throw new InvalidOperationException("The package-only Widgets tree did not build.");
element.deactivate();
element.unmount();

Console.WriteLine("G5-3-EXTERNAL-DART-APP-PACKAGE-CONSUMER-PASS");

sealed class PackageProbe(Action onBuild) : StatelessWidget
{
    public override Widget build(BuildContext context)
    {
        onBuild();
        return new PackageLeaf();
    }
}

sealed class PackageLeaf : Widget
{
    public override Element createElement() => new PackageLeafElement(this);
}

sealed class PackageLeafElement(Widget widget) : Element(widget);
