using System.Reflection;
using System.Text.Json;
using Doroti.Host.Web;
using Doroti.Ui;

internal static class BrowserSemanticsRegression
{
    internal static void Run()
    {
        var compare = typeof(BrowserSemanticsTransport).GetMethod("ContentEquals", BindingFlags.Static | BindingFlags.NonPublic)!
            .CreateDelegate<Func<SemanticsNodeUpdate, SemanticsNodeUpdate, bool>>();
        var node = new SemanticsNodeUpdate(1, Rect.fromLTWH(0, 0, 100, 40), "한글", "value", SemanticsAction.tap, [2],
            flags: new SemanticsFlags(isButton: true), controlsNodes: ["target"], locale: new Locale("ko"));
        // Compare every record property against the original transport contract.
        // This also catches a newly added field that is missing from the comparer.
        foreach (var property in typeof(SemanticsNodeUpdate).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (property.SetMethod is null) continue;
            var changed = node with { };
            var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            var value = property.GetValue(node);
            object? replacement = type == typeof(string) ? "other"
                : type == typeof(Rect) ? Rect.fromLTWH(5, 5, 10, 10)
                : type == typeof(int) ? 9 : type == typeof(long) ? 9L
                : type == typeof(double) ? 9.25
                : type.IsEnum ? Enum.ToObject(type, Convert.ToInt64(value ?? 0) + 1)
                : type == typeof(SemanticsFlags) ? new SemanticsFlags(isButton: false)
                : type == typeof(Locale) ? new Locale("en")
                : type == typeof(IReadOnlyList<int>) ? new int[] { 3 }
                : type == typeof(IReadOnlyList<string>) ? new string[] { "other" }
                : type == typeof(IReadOnlyList<double>) ? Matrix4.identity().storage
                : type == typeof(SemanticsTextGeometry) ? null
                : throw new InvalidOperationException($"Add a comparator case for {property.Name}");
            property.SetValue(changed, replacement);
            Check(compare(node, changed) == LegacyCompare(node, changed), $"wire comparison preserves {property.Name}");
        }
        var shifted = node with { rect = Rect.fromLTWH(0, 20, 100, 40), children = [2], controlsNodes = ["target"] };
        compare(node, shifted);
        var bytes = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 20; i++) Check(compare(node, shifted), "geometry retains wire content");
        Check(GC.GetAllocatedBytesForCurrentThread() == bytes, "retained content comparison allocates no record copies");

        var transport = new BrowserSemanticsTransport();
        transport.Update(new SemanticsUpdate(1, [node]));
        Check(transport.Update(new SemanticsUpdate(2, [node with { children = [2], controlsNodes = ["target"] }])) is null,
            "equal list contents do not create a patch");
        using (var delta = JsonDocument.Parse(transport.Update(new SemanticsUpdate(3, [shifted]))!))
            Check(delta.RootElement.GetProperty("nodes")[0].GetProperty("contentUnchanged").GetBoolean(),
                "scroll geometry still uses a compact patch");
        using (var delta = JsonDocument.Parse(transport.Update(new SemanticsUpdate(4,
            [shifted with { textSelectionBase = 0, textSelectionExtent = 2, scrollPosition = 20.5 }]))!))
            Check(delta.RootElement.GetProperty("nodes")[0].GetProperty("textSelectionExtent").GetInt64() == 2,
                "selection and scroll values remain immediate content changes");
        Console.WriteLine("PASS: allocation-free browser semantics comparison, all fields, compact geometry and immediate selection/scroll values");
    }

    private static bool LegacyCompare(SemanticsNodeUpdate previous, SemanticsNodeUpdate node) =>
        previous == node || ((previous.controlsNodes ?? []).SequenceEqual(node.controlsNodes ?? []) && previous with
        {
            rect = node.rect, children = node.children, indexInParent = node.indexInParent,
            traversalParent = node.traversalParent, controlsNodes = node.controlsNodes,
            coordinateTransform = node.coordinateTransform, platformViewId = node.platformViewId,
            textGeometry = node.textGeometry,
        } == node);
    private static void Check(bool value, string message)
    {
        if (!value) throw new InvalidOperationException(message);
    }
}
