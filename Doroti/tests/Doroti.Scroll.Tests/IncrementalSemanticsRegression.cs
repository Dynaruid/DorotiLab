using System.Diagnostics;
using System.Text.Json;
using Doroti.Host.Shared;
using Doroti.Ui;

internal static class IncrementalSemanticsRegression
{
    internal static void Run()
    {
        ProjectionMatchesWholeTree();
        NativePacketsRecoverAndStaySparse();
        Console.WriteLine("PASS: retained semantics projection, visibility, sparse native packets and snapshot recovery");
        if (Environment.GetEnvironmentVariable("DOROTI_MEASURE_SEMANTICS") == "1") MeasureSingleNodeMovement();
    }

    private static void MeasureSingleNodeMovement()
    {
        const int count = 2048;
        const int samples = 20;
        var nodes = Enumerable.Range(0, count).Select(Node).ToArray();
        nodes[0] = nodes[0] with { children = Enumerable.Range(1, count - 1).ToArray() };
        var retained = new RetainedSemanticsProjection();
        var legacy = new NativeSemanticsTransport();
        var sparse = new NativeSemanticsTransport();
        var initial = retained.Update(new(1, nodes));
        var fullBytes = 0;
        var sparseBytes = 0;
        void CaptureFull(byte[] bytes) => fullBytes = bytes.Length;
        uint CaptureSparse(byte[] bytes, long revision, long basis)
        {
            sparseBytes = bytes.Length;
            return 0;
        }
        legacy.Update(initial, null, CaptureFull);
        sparse.Update(initial, CaptureSparse, CaptureFull);
        var fullProjection = new (double Us, long Bytes)[samples];
        var incrementalProjection = new (double Us, long Bytes)[samples];
        var fullTransport = new (double Us, long Bytes)[samples];
        var incrementalTransport = new (double Us, long Bytes)[samples];
        for (var sample = -5; sample < samples; sample++)
        {
            nodes[1] = nodes[1] with { rect = nodes[1].rect.shift(new Offset(0.25, 0)) };
            var started = Stopwatch.GetTimestamp();
            var allocated = GC.GetAllocatedBytesForCurrentThread();
            var all = SemanticsGeometryProjection.ToViewCoordinates(nodes).OrderBy(node => node.id).ToArray();
            var priorProjection = Elapsed(started, allocated);

            var dirty = new SemanticsUpdate(1, [nodes[1]]);
            started = Stopwatch.GetTimestamp();
            allocated = GC.GetAllocatedBytesForCurrentThread();
            var next = retained.Update(dirty);
            var nextProjection = Elapsed(started, allocated);

            started = Stopwatch.GetTimestamp();
            allocated = GC.GetAllocatedBytesForCurrentThread();
            legacy.Update(new(1, all), null, CaptureFull);
            var priorTransport = Elapsed(started, allocated);

            started = Stopwatch.GetTimestamp();
            allocated = GC.GetAllocatedBytesForCurrentThread();
            sparse.Update(next, CaptureSparse, CaptureFull);
            var nextTransport = Elapsed(started, allocated);
            if (sample < 0) continue;
            fullProjection[sample] = priorProjection;
            incrementalProjection[sample] = nextProjection;
            fullTransport[sample] = priorTransport;
            incrementalTransport[sample] = nextTransport;
        }

        Console.WriteLine($"SEMANTICS {count} nodes, {samples} samples, one fractional move, no assistive technology");
        Report("full projection", fullProjection);
        Report("retained projection", incrementalProjection);
        Report("full snapshot encoding", fullTransport);
        Report("sparse encoding", incrementalTransport);
        Console.WriteLine($"SEMANTICS packet bytes at last step: full {fullBytes}; sparse {sparseBytes}");

        static (double, long) Elapsed(long started, long allocated) =>
            (Stopwatch.GetElapsedTime(started).TotalMicroseconds, GC.GetAllocatedBytesForCurrentThread() - allocated);
        static void Report(string name, (double Us, long Bytes)[] values)
        {
            var rank = (int)Math.Ceiling(values.Length * 0.95) - 1;
            Console.WriteLine(FormattableString.Invariant(
                $"SEMANTICS {name} p95: {values.Select(value => value.Us).Order().ElementAt(rank):0.00} us, {values.Select(value => value.Bytes).Order().ElementAt(rank)} B"));
        }
    }

    private static void ProjectionMatchesWholeTree()
    {
        var text = new SemanticsTextGeometry("א👩‍💻한", 7,
        [
            new(0, 1, Rect.fromLTWH(8, 2, 5, 10), 0, 7, TextDirection.rtl),
            new(1, 6, Rect.fromLTWH(1, 2, 7, 10), 0, 7),
        ]);
        var original = Enumerable.Range(0, 1025).Select(id => Node(id)).ToArray();
        original[0] = original[0] with
        {
            children = Enumerable.Range(1, 1024).Where(id => id != 2).ToArray(),
            coordinateTransform = Matrix4.diagonal3Values(2, 2, 1).storage,
        };
        original[1] = original[1] with { children = [2], coordinateTransform = Matrix4.translationValues(5, 2, 0).storage };
        original[2] = original[2] with { textGeometry = text };
        var projection = new RetainedSemanticsProjection();
        var first = projection.Update(new(1, original, viewDevicePixelRatio: 2));
        var unchanged = projection.Update(new(1, [original[2] with { }], viewDevicePixelRatio: 2));
        Check(ReferenceEquals(first.nodes, unchanged.nodes), "an equivalent dirty node keeps the entire snapshot");
        var visible = new SemanticsVisibleTree(SemanticsPlatformViewExposure.ExcludeSubtree);
        visible.Commit(visible.Prepare(first));
        var tree = SemanticsTreeSnapshot.IndexRetainedNodes(visible.Nodes);
        var children = tree.Children(1);

        original[2] = original[2] with { rect = original[2].rect.shift(new Offset(2.25, 4)) };
        var moved = projection.Update(new(1, [original[2]], viewDevicePixelRatio: 2));
        Check(moved.snapshotChanges?.changedNodes.Single().id == 2 && ReferenceEquals(first.nodes[100], moved.nodes[100]),
            "one moved leaf preserves other projected objects and only publishes the leaf");
        Check(first.nodes[2].rect != moved.nodes[2].rect, "a previously delivered snapshot does not change");
        Compare(moved, original, 2);
        var delta = visible.Prepare(moved);
        Check(!delta.TopologyChanged && delta.Changes.Count == 1, "a consumer can compare from an older equivalent snapshot");
        visible.Commit(delta);
        Check(ReferenceEquals(children, tree.Children(1)) && tree.Nodes[2] == moved.nodes[2], "the retained hierarchy reads the current leaf");

        original[2] = original[2] with { textSelectionBase = 0, textSelectionExtent = 6 };
        var selected = projection.Update(new(1, [original[2]], viewDevicePixelRatio: 2));
        Check(ReferenceEquals(moved.nodes[2].textGeometry, selected.nodes[2].textGeometry),
            "selection reuses projected RTL and ZWJ glyph geometry");
        original[1] = original[1] with { coordinateTransform = Matrix4.translationValues(30, 7, 0).storage };
        var ancestor = projection.Update(new(1, [original[1]], viewDevicePixelRatio: 2));
        Check(ancestor.snapshotChanges?.changedNodes.Select(node => node.id).SequenceEqual([1, 2]) == true,
            "an ancestor transform refreshes only itself and its affected descendants");
        Compare(ancestor, original, 2);
        Compare(projection.Update(new(1, [], viewDevicePixelRatio: 1.25)), original, 1.25);

        original[1] = original[1] with { flags = new SemanticsFlags(isHidden: true) };
        var hidden = projection.Update(new(1, [original[1]], viewDevicePixelRatio: 2));
        var hiddenDelta = visible.Prepare(hidden);
        Check(hiddenDelta.TopologyChanged && hiddenDelta.Removed.Order().SequenceEqual([1, 2]),
            "hidden ancestors remove their descendants from the exposed tree");
        visible.Commit(hiddenDelta);
        original[1] = original[1] with { flags = null, children = [] };
        var pruned = projection.Update(new(1, [original[1]], viewDevicePixelRatio: 2));
        Compare(pruned, original.Where(node => node.id != 2), 2);
        projection.Clear();
        var flat = new[] { Node(101) with { children = [102] }, Node(102), Node(201) };
        Compare(projection.Update(new(1, flat, viewDevicePixelRatio: 2)), flat, 2);
    }

    private static void NativePacketsRecoverAndStaySparse()
    {
        var transport = new NativeSemanticsTransport(qt: true);
        var receiver = new Receiver();
        var text = new SemanticsTextGeometry("한", 4, [new(0, 1, Rect.fromLTWH(1, 1, 6, 7), 0, 1)]);
        var root = Node(0) with { children = [1, 2] };
        var one = Node(1) with { textGeometry = text };
        var two = Node(2);
        byte[]? legacyPayload = null;
        void Send(params SemanticsNodeUpdate[] nodes) => transport.Update(new(1, nodes), receiver.Submit,
            bytes => legacyPayload = bytes);
        Send(root, one, two);
        Check(receiver.Base == 0 && receiver.Payload.GetProperty("nodes").GetArrayLength() == 3, "first native packet is a full snapshot");
        Send(root, one, two);
        Check(receiver.Packets == 1, "equal snapshots send no bytes");
        one = one with { rect = one.rect.shift(new Offset(4, 0)), textGeometry = text.Transform(rect => rect.shift(new Offset(4, 0))) };
        Send(root, one, two);
        var change = receiver.Payload.GetProperty("nodes")[0];
        Check(receiver.Base == 1 && change.EnumerateObject().Select(property => property.Name).SequenceEqual(["id", "rect", "textBounds"])
            && change.GetProperty("textBounds")[0][1].GetDouble() == 5, "movement sends only the changed node and rectangle fields");
        one = one with { label = "새 이름", scrollPosition = 23.125, textSelectionExtent = 1 };
        Send(root, one, two);
        change = receiver.Payload.GetProperty("nodes")[0];
        Check(change.EnumerateObject().Select(property => property.Name).SequenceEqual(["id", "label", "textSelectionExtent", "scrollPosition"])
            && change.GetProperty("scrollPosition").GetDouble() == 23.125, "field patches retain fractional offsets and selection");
        receiver.LoseBaseline();
        Send(root with { children = [1] }, one);
        Check(receiver.Packets == 5 && receiver.Base == 0 && receiver.Payload.GetProperty("nodes").GetArrayLength() == 2,
            "a rejected patch immediately retries the latest snapshot");
        one = one with { flags = new SemanticsFlags(isHidden: true) };
        Send(root, one, two);
        Check(receiver.Payload.GetProperty("removed")[0].GetInt32() == 1
            && receiver.Payload.GetProperty("nodes")[0].GetProperty("children")[0].GetInt32() == 2,
            "hiding a node sends removal and changed final child order");
        var revision = receiver.Revision;
        transport.Clear();
        receiver.LoseBaseline();
        Send(root, one, two);
        Check(receiver.Base == 0 && receiver.Revision > revision, "Clear forces a full snapshot without reusing a revision");
        new NativeSemanticsTransport().Update(new(1, [root, one, two]), null, bytes => legacyPayload = bytes);
        using var legacy = JsonDocument.Parse(legacyPayload!);
        Check(legacy.RootElement.GetProperty("nodes").GetArrayLength() == 2, "legacy hosts still receive complete snapshots");
    }

    private static SemanticsNodeUpdate Node(int id) =>
        new(id, Rect.fromLTWH(0, 0, 50, 40), $"node {id}", null, SemanticsAction.tap, []);

    private static void Compare(SemanticsUpdate projected, IEnumerable<SemanticsNodeUpdate> source, double ratio)
    {
        var expected = SemanticsGeometryProjection.ToViewCoordinates(source, ratio)
            .OrderBy(node => node.indexInParent ?? int.MaxValue).ThenBy(node => node.id);
        Check(projected.nodes.Select(node => node.id).SequenceEqual(expected.Select(node => node.id)),
            "reference and retained snapshots have the same order and reachability");
        Check(projected.nodes.Zip(expected).All(pair => SemanticsUpdateDiffer.ChangedProperties(pair.First, pair.Second) == SemanticsNodeProperty.none),
            "retained bounds, text geometry and metadata match the full projection");
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private sealed class Receiver
    {
        private byte[] _payload = [];
        internal JsonElement Payload => JsonSerializer.Deserialize<JsonElement>(_payload);
        internal long Revision { get; private set; }
        internal long Base { get; private set; }
        internal int Packets { get; private set; }

        internal void LoseBaseline() => Revision = 0;

        internal uint Submit(byte[] bytes, long revision, long baseline)
        {
            Packets++;
            if (baseline != 0 && baseline != Revision) return NativeSemanticsTransport.SnapshotRequired;
            _payload = bytes;
            Revision = revision;
            Base = baseline;
            return 0;
        }
    }
}
