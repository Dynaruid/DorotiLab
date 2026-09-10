using Doroti.Framework.Widgets;
using Doroti.Ui;

internal static class FocusGeometryContract
{
    internal static void Verify()
    {
        var origin = new GeometryNode(0, 0);
        var left = new GeometryNode(-30, 0);
        var right = new GeometryNode(30, 0);
        var farRight = new GeometryNode(60, 0);
        var up = new GeometryNode(0, -30);
        var down = new GeometryNode(0, 30);
        var farDown = new GeometryNode(0, 60);
        FocusNode[] nodes = [farRight, down, origin, left, farDown, right, up];
        var widget = new WidgetOrderTraversalPolicy();
        var reading = new ReadingOrderTraversalPolicy();
        var ordered = new OrderedTraversalPolicy();
        foreach (var filter in new Func<TraversalDirection, Rect, IEnumerable<FocusNode>, bool, IEnumerable<FocusNode>>[]
        {
            widget._sortAndFilterHorizontally, reading._sortAndFilterHorizontally, ordered._sortAndFilterHorizontally,
        })
        {
            Require(filter(TraversalDirection.right, origin.rect, nodes, true).SequenceEqual([right, farRight]), "right direction and distance ordering");
            Require(filter(TraversalDirection.left, origin.rect, nodes, true).SequenceEqual([left]), "left direction excludes source rectangle");
            Require(filter(TraversalDirection.right, origin.rect, nodes, false).Contains(left), "reverse traversal searches opposite side");
            try { filter(TraversalDirection.down, origin.rect, nodes, true).ToArray(); throw new Exception("accepted invalid horizontal direction"); }
            catch (Doroti.Runtime.DartArgumentError) { }
            catch (Doroti.Runtime.AssertionError) { }
        }
        foreach (var filter in new Func<TraversalDirection, Rect, IEnumerable<FocusNode>, bool, IEnumerable<FocusNode>>[]
        {
            widget._sortAndFilterVertically, reading._sortAndFilterVertically, ordered._sortAndFilterVertically,
        })
        {
            Require(filter(TraversalDirection.down, origin.rect, nodes, true).SequenceEqual([down, farDown]), "down direction and distance ordering");
            Require(filter(TraversalDirection.up, origin.rect, nodes, true).SequenceEqual([up]), "up direction excludes source rectangle");
        }
        Require(DirectionalFocusTraversalPolicyMixin._sortByDistancePreferHorizontal(origin.rect.center, new[] { farRight, right }).First() == right, "horizontal center-distance sort");
        Require(DirectionalFocusTraversalPolicyMixin._sortByDistancePreferVertical(origin.rect.center, new[] { farDown, down }).First() == down, "vertical center-distance sort");
        foreach (var node in nodes) node.dispose();
        Console.WriteLine("Focus geometry: all three policies, four directions, reverse traversal, center-distance ordering and invalid direction PASS");
    }
    private static void Require(bool condition, string message) { if (!condition) throw new Exception(message); }
    private sealed class GeometryNode(double x, double y) : FocusNode
    {
        public override Rect rect => Rect.fromLTWH(x, y, 10, 10);
    }
}
