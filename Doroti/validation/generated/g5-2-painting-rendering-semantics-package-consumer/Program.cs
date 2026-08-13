using Doroti.Generated.Framework.Painting;
using Doroti.Generated.Framework.Rendering;
using Doroti.Generated.Framework.Semantics;

var alignment = new Alignment(0.25, -0.5);
if (alignment.x != 0.25 || alignment.y != -0.5)
{
    throw new InvalidOperationException("Painting package behavior drifted.");
}

var constraints = BoxConstraints.CreateTightFor(width: 320, height: 180);
if (constraints.minWidth != 320 || constraints.maxWidth != 320 ||
    constraints.minHeight != 180 || constraints.maxHeight != 180)
{
    throw new InvalidOperationException("Rendering package behavior drifted.");
}

var tag = new SemanticsTag("external-package-consumer");
if (tag.name != "external-package-consumer")
{
    throw new InvalidOperationException("Semantics package behavior drifted.");
}

Console.WriteLine("G5-2-PAINTING-RENDERING-SEMANTICS-PACKAGE-CONSUMER-PASS");
