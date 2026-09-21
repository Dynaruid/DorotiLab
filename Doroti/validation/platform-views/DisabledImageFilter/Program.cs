using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Ui;

var context = new SizedBox().createElement();
var first = new ImageFilter(sigmaX: 2, sigmaY: 2);
var latest = new ImageFilter(sigmaX: 4, sigmaY: 4);
var widget = new ImageFiltered(imageFilter: first, enabled: false);
var filter = (RenderBox)widget.createRenderObject(context);
var root = new CountingBoundary(filter);
var pipeline = new PipelineOwner { rootNode = root };
root.layout(BoxConstraints.CreateTightFor(width: 32, height: 32));
pipeline.flushCompositingBits();
PaintingContext.repaintCompositedChild(root, debugAlsoPaintedParent: true);
root.Invalidations = 0;
new ImageFiltered(imageFilter: latest, enabled: false).updateRenderObject(context, filter);
Check("disabled filter updates do not invalidate the ancestor picture", root.Invalidations == 0);
new ImageFiltered(imageFilter: latest, enabled: true).updateRenderObject(context, filter);
Check("enabling the filter invalidates the ancestor", root.Invalidations > 0);
var layer = (ImageFilterLayer)filter.updateCompositedLayer(null);
Check("enabling uses the latest disabled-state filter", ReferenceEquals(layer.imageFilter, latest));
Console.WriteLine("PASS 3 disabled image filter lifecycle checks");

static void Check(string name, bool condition)
{
    if (!condition)
        throw new InvalidOperationException(name);
    Console.WriteLine("PASS " + name);
}

sealed class CountingBoundary(RenderBox child) : RenderRepaintBoundary(child: child)
{
    internal int Invalidations;

    public override void markNeedsPaint()
    {
        Invalidations++;
        base.markNeedsPaint();
    }
}
