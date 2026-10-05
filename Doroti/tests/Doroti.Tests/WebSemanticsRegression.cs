using System.Text.Json;
using Doroti.Host.Web;
using Doroti.Ui;

internal static class WebSemanticsRegression
{
    public static void Run(string? output = null)
    {
        static void Require(bool condition, string message)
        { if (!condition) throw new InvalidOperationException(message); }
        static JsonElement Parse(string json)
        { using var document = JsonDocument.Parse(json); return document.RootElement.Clone(); }
        static SemanticsNodeUpdate Node(int id, IReadOnlyList<int>? children = null) =>
            new(id, Rect.fromLTWH(id * 5, id * 5, 100, 30), $"node {id}", null, SemanticsAction.tap, children ?? []);

        var large = new BrowserSemanticsTransport();
        var initial = large.Update(new(1, [Node(0, Enumerable.Range(1, 1000).ToArray()),
            .. Enumerable.Range(1, 1000).Select(id => Node(id))]))!;
        var moved = large.Update(new(2, [Node(42) with { rect = Rect.fromLTWH(220, 220, 100, 30) }]))!;
        var delta = Parse(moved);
        Require(delta.GetProperty("kind").GetString() == "delta" && delta.GetProperty("nodes").GetArrayLength() == 1,
            "One-node motion retransmitted the full semantics tree.");
        Require(!delta.GetProperty("nodes")[0].TryGetProperty("children", out _), "Motion retransmitted unchanged children.");
        Require(!delta.TryGetProperty("roots", out _) && moved.Length < initial.Length / 100, "Motion did not bound payload work.");
        Require(large.Update(new(3, [Node(42) with { rect = Rect.fromLTWH(220, 220, 100, 30), children = new int[0] }])) is null,
            "Value-identical nodes with new collection instances generated traffic.");

        var sender = new BrowserSemanticsTransport();
        var state = new Dictionary<int, SemanticsNodeUpdate>
        {
            [0] = Node(0, [1, 2, 3]), [1] = Node(1, [4, 5]), [2] = Node(2),
            [3] = Node(3) with { controlsNodes = ["details"] },
            [4] = Node(4) with { label = "Editor", value = "한글 abc", textSelectionBase = 1, textSelectionExtent = 3,
                actions = SemanticsAction.setText | SemanticsAction.setSelection | SemanticsAction.focus,
                flags = new(isTextField: true, isFocused: Tristate.isTrue) },
            [5] = Node(5) with { identifier = "details" },
        };
        var steps = new List<object>();
        long generation = 0;
        JsonElement? Step(string name, params int[] changed)
        {
            var packet = sender.Update(new(++generation, changed.Select(id => state[id]).ToArray()));
            var oracle = new BrowserSemanticsTransport();
            var full = oracle.Update(new(generation, state.Values.ToArray()))!;
            var wire = packet is null ? (JsonElement?)null : Parse(packet);
            steps.Add(new { name, packet = wire, snapshot = Parse(full) });
            return wire;
        }
        Step("initial", 0, 1, 2, 3, 4, 5);
        state[1] = state[1] with { rect = Rect.fromLTWH(15, 20, 100, 30) };
        Step("parent-motion", 1);
        state[4] = state[4] with { value = "수정 abc", textSelectionBase = 2, textSelectionExtent = 4 };
        Step("editing", 4);
        state[4] = state[4] with { children = [], controlsNodes = null };
        Step("unchanged", 4);
        state[1] = state[1] with { children = [5, 4] };
        Step("reorder", 1);
        state[1] = state[1] with { children = [5] };
        state[2] = state[2] with { children = [4] };
        Step("reparent", 1, 2);
        state[5] = state[5] with { identifier = "other-details" };
        Step("identifier-removed", 5);
        state[6] = Node(6) with { identifier = "details" };
        state[1] = state[1] with { children = [5, 6] };
        Step("identifier-added", 1, 6);
        state[0] = state[0] with { children = [2, 3] };
        state.Remove(1); state.Remove(5); state.Remove(6);
        var deletion = Step("branch-removed", 0)!.Value;
        Require(deletion.GetProperty("removed").GetArrayLength() == 3, "Unreachable subtree was not removed.");
        state[4] = state[4] with { flags = state[4].flags! with { isMultiline = true } };
        Step("element-kind", 4);
        var cleared = Parse(sender.Clear());
        Require(cleared.GetProperty("kind").GetString() == "full" && cleared.GetProperty("nodes").GetArrayLength() == 0,
            "Clear did not reset the receiver explicitly.");
        steps.Add(new { name = "clear", packet = (JsonElement?)cleared, snapshot = cleared });
        Step("rebuilt", 0, 2, 3, 4);
        var recovery = Parse(sender.CaptureSnapshot());
        Require(recovery.GetProperty("kind").GetString() == "full" && recovery.GetProperty("nodes").GetArrayLength() == 4,
            "Recovery did not send complete retained content.");
        if (output is not null)
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(output))!);
            File.WriteAllText(output, JsonSerializer.Serialize(new { steps, recovery, baselineBytes = initial.Length, deltaBytes = moved.Length }));
        }
        Console.WriteLine($"Web semantics: PASS; 1001-node snapshot {initial.Length} chars -> one-node geometry {moved.Length} chars; 12 topology/editing steps.");
    }
}
