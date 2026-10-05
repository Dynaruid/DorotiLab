using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Doroti.Ui;

namespace Doroti.Host.Web;

/// <summary>Owner-local DOM transport. Framework updates remain unchanged.</summary>
internal sealed class BrowserSemanticsTransport
{
    private static long _nextStream;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };
    private readonly long _stream = Interlocked.Increment(ref _nextStream);
    private readonly Dictionary<int, SemanticsNodeUpdate> _nodes = [];
    private long _revision;
    private long _generation;
    private int[] _roots = [];

    public string? Update(SemanticsUpdate update)
    {
        _generation = update.generation;
        var changes = new Dictionary<int, (SemanticsNodeUpdate Node, SemanticsNodeUpdate? Previous)>();
        var topology = _revision == 0;
        foreach (var node in update.nodes)
        {
            _nodes.TryGetValue(node.id, out var previous);
            if (previous is not null && ContentEquals(previous, node)
                && previous.rect == node.rect && previous.children.SequenceEqual(node.children)
                && previous.indexInParent == node.indexInParent)
                continue;
            // Classify against the last delivered value even if a batch repeats an ID.
            if (changes.TryGetValue(node.id, out var already)) previous = already.Previous;
            changes[node.id] = (node, previous);
            topology |= previous is null || !previous.children.SequenceEqual(node.children)
                || previous.indexInParent != node.indexInParent;
            _nodes[node.id] = node;
        }

        var removed = topology ? PruneUnreachable() : [];
        foreach (var id in removed) changes.Remove(id);
        if (topology) _roots = FindRoots();
        if (_revision == 0) return CaptureSnapshot();
        if (changes.Count == 0 && removed.Count == 0) return null;
        return Encode(false, changes.Values, removed, topology);
    }

    public string CaptureSnapshot() => Encode(true,
        _nodes.Values.OrderBy(node => node.indexInParent ?? int.MaxValue).ThenBy(node => node.id)
            .Select(node => (node, (SemanticsNodeUpdate?)null)), [], true);

    public string Clear()
    {
        Reset();
        return CaptureSnapshot();
    }

    public void Reset()
    {
        _nodes.Clear();
        _roots = [];
        _generation = 0;
    }

    private string Encode(bool full,
        IEnumerable<(SemanticsNodeUpdate Node, SemanticsNodeUpdate? Previous)> changes,
        IReadOnlyList<int> removed, bool topology)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            writer.WriteStartObject();
            writer.WriteNumber("schemaVersion", 1);
            writer.WriteString("kind", full ? "full" : "delta");
            writer.WriteNumber("stream", _stream);
            writer.WriteNumber("baseRevision", full ? 0 : _revision);
            writer.WriteNumber("revision", ++_revision);
            writer.WriteNumber("generation", _generation);
            writer.WriteStartArray("nodes");
            foreach (var (node, previous) in changes)
            {
                if (!full && previous is not null && ContentEquals(previous, node))
                {
                    writer.WriteStartObject();
                    writer.WriteNumber("id", node.id);
                    writer.WriteBoolean("contentUnchanged", true);
                    if (previous.rect != node.rect) WriteRect(writer, node);
                    if (!previous.children.SequenceEqual(node.children))
                    {
                        writer.WritePropertyName("children");
                        JsonSerializer.Serialize(writer, node.children);
                    }
                    writer.WriteEndObject();
                }
                else WriteNode(writer, node);
                FrameworkWorkCounters.Add(FrameworkWork.SemanticsJsonSerializedNode);
            }
            writer.WriteEndArray();
            writer.WritePropertyName("removed");
            JsonSerializer.Serialize(writer, removed);
            if (topology)
            {
                writer.WritePropertyName("roots");
                JsonSerializer.Serialize(writer, _roots);
            }
            writer.WriteEndObject();
        }
        FrameworkWorkCounters.Add(FrameworkWork.SemanticsPayloadBytes, stream.Length);
        return Encoding.UTF8.GetString(stream.GetBuffer(), 0, checked((int)stream.Length));
    }

    private static bool ContentEquals(SemanticsNodeUpdate previous, SemanticsNodeUpdate node) =>
        previous == node || ((previous.controlsNodes ?? []).SequenceEqual(node.controlsNodes ?? [])
        && previous with
        {
            rect = node.rect, children = node.children, indexInParent = node.indexInParent,
            traversalParent = node.traversalParent, controlsNodes = node.controlsNodes,
            coordinateTransform = node.coordinateTransform, platformViewId = node.platformViewId,
            textGeometry = node.textGeometry,
        } == node);

    private List<int> PruneUnreachable()
    {
        var removed = new List<int>();
        // Preserve the legacy forest contract when no framework root is present.
        if (!_nodes.ContainsKey(0)) return removed;
        var reachable = new HashSet<int>();
        var pending = new Stack<int>();
        pending.Push(0);
        while (pending.TryPop(out var id))
        {
            if (!reachable.Add(id) || !_nodes.TryGetValue(id, out var node)) continue;
            foreach (var child in node.children) pending.Push(child);
        }
        foreach (var id in _nodes.Keys.Where(id => !reachable.Contains(id)).ToArray())
        {
            _nodes.Remove(id);
            removed.Add(id);
        }
        return removed;
    }

    private int[] FindRoots()
    {
        var children = _nodes.Values.SelectMany(node => node.children).ToHashSet();
        return _nodes.Values.Where(node => !children.Contains(node.id))
            .OrderBy(node => node.indexInParent ?? int.MaxValue).ThenBy(node => node.id)
            .Select(node => node.id).ToArray();
    }

    private static void WriteRect(Utf8JsonWriter writer, SemanticsNodeUpdate node)
    {
        writer.WriteStartArray("rect");
        writer.WriteNumberValue(node.rect.left);
        writer.WriteNumberValue(node.rect.top);
        writer.WriteNumberValue(node.rect.right);
        writer.WriteNumberValue(node.rect.bottom);
        writer.WriteEndArray();
    }

    private static void WriteNode(Utf8JsonWriter writer, SemanticsNodeUpdate node) =>
        JsonSerializer.Serialize(writer, new
        {
            node.id, node.label, node.value, role = node.role.ToString(), actions = (long)node.actions,
            node.children, node.identifier, node.hint, node.tooltip, node.increasedValue,
            node.decreasedValue, node.headingLevel, node.linkUrl,
            validationResult = node.validationResult.ToString(), hitTestBehavior = node.hitTestBehavior.ToString(),
            inputType = node.inputType.ToString(), node.minValue, node.maxValue, node.maxValueLength,
            node.currentValueLength, node.scrollPosition, node.scrollExtentMin, node.scrollExtentMax,
            node.scrollChildCount, node.scrollIndex, node.controlsNodes, locale = node.locale?.ToString(),
            flags = node.flags is null ? null : new
            {
                @checked = node.flags.isChecked.ToString(), selected = node.flags.isSelected.toBoolOrNull(),
                enabled = node.flags.isEnabled.toBoolOrNull(), toggled = node.flags.isToggled.toBoolOrNull(),
                expanded = node.flags.isExpanded.toBoolOrNull(), required = node.flags.isRequired.toBoolOrNull(),
                focused = node.flags.isFocused.toBoolOrNull(), button = node.flags.isButton,
                textField = node.flags.isTextField, header = node.flags.isHeader, hidden = node.flags.isHidden,
                image = node.flags.isImage, liveRegion = node.flags.isLiveRegion, multiline = node.flags.isMultiline,
                readOnly = node.flags.isReadOnly, link = node.flags.isLink, slider = node.flags.isSlider,
                focusable = node.flags.isFocused != Tristate.none, obscured = node.flags.isObscured,
                mutuallyExclusive = node.flags.isInMutuallyExclusiveGroup, keyboardKey = node.flags.isKeyboardKey,
            },
            node.textSelectionBase, node.textSelectionExtent,
            rect = new[] { node.rect.left, node.rect.top, node.rect.right, node.rect.bottom },
        }, JsonOptions);
}
