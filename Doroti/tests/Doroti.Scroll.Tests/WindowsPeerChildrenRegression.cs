using Doroti.Host.SharedWindows;

internal static class WindowsPeerChildrenRegression
{
    private sealed class Peer { internal string Label = "current"; }
    private sealed class Native { internal Peer? Peer = new(); }
    private sealed class Child { internal Native? Native = new(); }
    private static readonly Func<Child, object?> Identity = child => child.Native;
    private static readonly Func<object, Peer?> Resolve = native => ((Native)native).Peer;

    internal static void Run()
    {
        var cache = new WindowsPeerChildrenCache<Child, Peer>();
        var first = new Child();
        var second = new Child();
        var ignored = new Child { Native = null };
        IList<Child> source = new List<Child> { first, ignored, second };
        var original = cache.Capture(source, Identity, Resolve);
        Check(original.Count == 2 && ReferenceEquals(original[0], first.Native!.Peer), "semantic child filter/order");
        var allocated = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 20; i++)
            if (!ReferenceEquals(original, cache.Capture(source, Identity, Resolve)))
                throw new InvalidOperationException("unchanged native topology replaced its UIA list");
        Check(allocated == GC.GetAllocatedBytesForCurrentThread(), "stable UIA child queries allocate nothing");
        first.Native!.Peer!.Label = "updated";
        Check(original[0].Label == "updated", "cached children keep live peer values");

        source[0] = second;
        source[2] = first;
        var reordered = cache.Capture(source, Identity, Resolve);
        Check(!ReferenceEquals(original, reordered) && ReferenceEquals(reordered[0], second.Native!.Peer), "same-count reordering refreshes UIA children");
        source.RemoveAt(2);
        var removed = cache.Capture(source, Identity, Resolve);
        Check(removed.Count == 1 && original.Count == 2, "removal refreshes children and preserves an issued snapshot");
        second.Native = new();
        var reconnected = cache.Capture(source, Identity, Resolve);
        Check(!ReferenceEquals(removed[0], reconnected[0]), "native handler replacement refreshes the peer even for the same virtual child");

        var pending = new Child { Native = new() { Peer = null } };
        source.Add(pending);
        Check(cache.Capture(source, Identity, Resolve).Count == 1, "unready peer is omitted");
        pending.Native!.Peer = new();
        Check(cache.Capture(source, Identity, Resolve).Count == 2, "an unready peer is retried without a topology change");
        source.Clear();
        Check(cache.Capture(source, Identity, Resolve).Count == 0, "teardown releases the cached topology");
        Console.WriteLine("PASS: stable Windows UIA child lists, live values, order/removal/handler refresh and unready-peer retry");
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
