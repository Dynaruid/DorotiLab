using Doroti.Host.Maui;
using Doroti.Ui;

internal static class ProjectionRegression
{
    internal static void Run()
    {
        Check(AndroidFrameTiming.Timestamp(TimeSpan.FromMilliseconds(500), 2_000_000_000, 1_960_000_000)
            == TimeSpan.FromMilliseconds(460), "delayed first vsync retains 40 ms callback age");
        Check(AndroidFrameTiming.Timestamp(TimeSpan.FromMilliseconds(508), 2_008_000_000, 2_008_000_000)
            == TimeSpan.FromMilliseconds(508), "later pulse does not inherit first callback delay");
        Check(AndroidFrameTiming.Timestamp(TimeSpan.FromMilliseconds(5), 2_000_000_000, 1_960_000_000)
            == TimeSpan.Zero, "vsync cannot precede framework origin");
        Check(AndroidFrameTiming.Timestamp(TimeSpan.FromMilliseconds(5), 2_000_000_000, 0)
            == TimeSpan.FromMilliseconds(5), "missing vsync uses arrival");

        double[] values = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16];
        var alias = new Matrix4(values);
        alias.multiply(alias);
        Check(alias.storage.SequenceEqual(new double[] {90,100,110,120,202,228,254,280,314,356,398,440,426,484,542,600}),
            "in-place matrix multiplication preserves aliasing and column order");
        var identity = Matrix4.identity();
        alias.multiply(identity); // warm the measured method
        var allocated = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 20; i++) alias.multiply(identity);
        Check(GC.GetAllocatedBytesForCurrentThread() == allocated, "in-place matrix composition allocates no heap temporaries");

        // Perspective, rotation, zero-w and legacy child origins all use the
        // public projection boundary. Expected corners use the independent
        // vector API and the non-mutating matrix multiplication path.
        foreach (var perspective in new[] { 0.0, 0.001, -0.01 })
        {
            var transform = new Matrix4([0, 1, 0, perspective, -1, 0, 0, 0, 0, 0, 1, 0, 30, 40, 0, 1]);
            var rect = Rect.fromLTWH(0, 2, 100, 10);
            var nodes = SemanticsGeometryProjection.ToViewCoordinates([
                new SemanticsNodeUpdate(1, Rect.fromLTWH(0, 0, 300, 300), "root", null, SemanticsAction.tap, [2],
                    coordinateTransform: Matrix4.diagonal3Values(3, 3, 1).storage),
                new SemanticsNodeUpdate(2, rect, "child", null, SemanticsAction.tap, [], coordinateTransform: transform.storage),
            ], 3);
            var projected = nodes.Single(n => n.id == 2);
            var corners = new[] { new Vector3(rect.left,rect.top,0), new Vector3(rect.right,rect.top,0),
                new Vector3(rect.left,rect.bottom,0), new Vector3(rect.right,rect.bottom,0) }.Select(transform.perspectiveTransform).ToArray();
            Check(projected.rect == new Rect(corners.Min(p=>p.x),corners.Min(p=>p.y),corners.Max(p=>p.x),corners.Max(p=>p.y)),
                "semantics projection preserves perspective corner bounds and DPR");
        }
        var legacy = SemanticsGeometryProjection.ToViewCoordinates([
            new SemanticsNodeUpdate(1, Rect.fromLTWH(10, 20, 100, 100), "root", null, SemanticsAction.tap, [2]),
            new SemanticsNodeUpdate(2, Rect.fromLTWH(2, 3, 10, 20), "child", null, SemanticsAction.tap, []),
        ]);
        Check(legacy[1].rect == Rect.fromLTWH(12, 23, 10, 20), "legacy rect-relative child origins survive");
        var builder = new SemanticsUpdateBuilder();
        var original = new SemanticsNodeUpdate(1, Rect.fromLTWH(0, 0, 10, 10), "node", null, SemanticsAction.tap, []);
        builder.updateNode(original);
        builder.UpdateTextGeometry(1, null);
        Check(ReferenceEquals(builder.build().nodes[0], original), "absent text geometry retains the original node snapshot");
        var geometry = new SemanticsTextGeometry("a", 1, [new SemanticsTextRun(0, 1, original.rect, 0, 1)]);
        builder.UpdateTextGeometry(1, geometry);
        var withGeometry = builder.build().nodes[0];
        Check(ReferenceEquals(withGeometry.textGeometry, geometry)
            && (withGeometry.actions & SemanticsAction.scrollToTextRange) != 0, "layout text geometry still enables range reveal");
        builder.UpdateTextGeometry(1, geometry);
        Check(ReferenceEquals(withGeometry, builder.build().nodes[0]), "identical text geometry retains its snapshot");
        builder.updateNode(original with { flags = new SemanticsFlags(isObscured: true), textGeometry = geometry });
        builder.UpdateTextGeometry(1, geometry);
        Check(builder.build().nodes[0].textGeometry is null, "obscured text geometry is still removed");
        Console.WriteLine("PASS: Android vsync clock, matrix ownership/allocation, perspective and legacy semantics projection");
    }
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
