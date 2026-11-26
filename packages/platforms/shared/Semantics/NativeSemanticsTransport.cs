using System.Buffers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Doroti.Ui;

namespace Doroti.Host.Shared;

/// <summary>One retained transport per native view. Revision acknowledgments cover the native FIFO.</summary>
internal sealed class NativeSemanticsTransport(bool qt = false)
{
    internal const uint SnapshotRequired = 2;
    private readonly SemanticsVisibleTree _visible = new(SemanticsPlatformViewExposure.Include);
    private long _revision;
    private long _baseRevision;

    internal void Update(
        SemanticsUpdate update,
        Func<byte[], long, long, uint>? submit,
        Action<byte[]> sendLegacy
    )
    {
        var delta = _visible.Prepare(update);
        var changes = delta.Changes.Where(HasWireChanges).ToArray();
        _visible.Commit(delta);
        if (_baseRevision != 0 && changes.Length == 0 && delta.Removed.Count == 0) return;

        var full = _baseRevision == 0 || submit is null;
        var revision = ++_revision;
        try
        {
            if (submit is null)
                sendLegacy(Encode(update.generation, true, changes, delta.Removed));
            else
            {
                var status = submit(Encode(update.generation, full, changes, delta.Removed), revision, full ? 0 : _baseRevision);
                if (status == SnapshotRequired && !full)
                {
                    // The rejected packet never entered the FIFO. Resend the current snapshot.
                    status = submit(Encode(update.generation, true, changes, delta.Removed), revision, 0);
                }
                if (status != 0) throw new InvalidOperationException($"Native semantics submission failed: {status}.");
            }
            _baseRevision = revision;
        }
        catch
        {
            _baseRevision = 0;
            throw;
        }
    }

    internal void Clear()
    {
        _baseRevision = 0;
        _visible.Clear();
    }

    private byte[] Encode(long generation, bool full, SemanticsVisibleNodeChange[] changes, IReadOnlyList<int> removed)
    {
        var buffer = new ArrayBufferWriter<byte>();
        using (var writer = new Utf8JsonWriter(buffer))
        {
            writer.WriteStartObject();
            writer.WriteNumber("generation", generation);
            writer.WriteStartArray("nodes");
            if (full)
            {
                foreach (var id in _visible.Order) WriteNode(writer, _visible.Nodes[id], null);
            }
            else
            {
                foreach (var change in changes) WriteNode(writer, change.Current, change.Previous);
            }
            writer.WriteEndArray();
            if (!full && removed.Count != 0)
            {
                writer.WriteStartArray("removed");
                foreach (var id in removed) writer.WriteNumberValue(id);
                writer.WriteEndArray();
            }
            writer.WriteEndObject();
        }
        FrameworkWorkCounters.Add(FrameworkWork.SemanticsPayloadBytes, buffer.WrittenCount);
        return buffer.WrittenSpan.ToArray();
    }

    private static bool HasWireChanges(SemanticsVisibleNodeChange change)
    {
        if (change.Previous is not { } old) return true;
        var node = change.Current;
        return (change.Properties & (SemanticsNodeProperty.bounds | SemanticsNodeProperty.label
            | SemanticsNodeProperty.value | SemanticsNodeProperty.role | SemanticsNodeProperty.actions
            | SemanticsNodeProperty.flags | SemanticsNodeProperty.selection | SemanticsNodeProperty.scroll
            | SemanticsNodeProperty.textGeometry)) != 0
            || old.indexInParent != node.indexInParent
            || !old.children.SequenceEqual(node.children)
            || old.identifier != node.identifier || old.hint != node.hint || old.tooltip != node.tooltip
            || old.headingLevel != node.headingLevel || old.linkUrl != node.linkUrl
            || old.increasedValue != node.increasedValue || old.decreasedValue != node.decreasedValue
            || old.minValue != node.minValue || old.maxValue != node.maxValue;
    }

    private void WriteNode(Utf8JsonWriter writer, SemanticsNodeUpdate node, SemanticsNodeUpdate? old)
    {
        var full = old is null;
        writer.WriteStartObject();
        writer.WriteNumber("id", node.id);
        if (full || old!.rect != node.rect) WriteRect(writer, "rect", node.rect);
        if (full || old!.label != node.label) writer.WriteString("label", node.label);
        if (full || old!.value != node.value) writer.WriteString("value", node.value);
        if (full || old!.role != node.role) writer.WriteString("role", node.role.ToString());
        if (full || old!.actions != node.actions) writer.WriteNumber("actions", (long)node.actions);
        if (full || old!.identifier != node.identifier) writer.WriteString("identifier", node.identifier);
        if (full || old!.hint != node.hint) writer.WriteString("hint", node.hint);
        if (full || old!.tooltip != node.tooltip) writer.WriteString("tooltip", node.tooltip);
        if (full || old!.linkUrl != node.linkUrl) writer.WriteString("linkUrl", node.linkUrl);
        if (full || old!.increasedValue != node.increasedValue) writer.WriteString("increasedValue", node.increasedValue);
        if (full || old!.decreasedValue != node.decreasedValue) writer.WriteString("decreasedValue", node.decreasedValue);
        if (full || old!.minValue != node.minValue) writer.WriteString("minValue", node.minValue);
        if (full || old!.maxValue != node.maxValue) writer.WriteString("maxValue", node.maxValue);
        if (full || old!.headingLevel != node.headingLevel) WriteNumber(writer, "headingLevel", node.headingLevel);
        if (full || old!.indexInParent != node.indexInParent)
            WriteNumber(writer, "indexInParent", (long?)node.indexInParent);
        if (full || old!.textSelectionBase != node.textSelectionBase) writer.WriteNumber("textSelectionBase", node.textSelectionBase);
        if (full || old!.textSelectionExtent != node.textSelectionExtent) writer.WriteNumber("textSelectionExtent", node.textSelectionExtent);
        if (full || old!.scrollPosition != node.scrollPosition) WriteNumber(writer, "scrollPosition", node.scrollPosition);
        if (full || old!.scrollExtentMin != node.scrollExtentMin) WriteNumber(writer, "scrollExtentMin", node.scrollExtentMin);
        if (full || old!.scrollExtentMax != node.scrollExtentMax) WriteNumber(writer, "scrollExtentMax", node.scrollExtentMax);
        if (full || !old!.children.SequenceEqual(node.children))
        {
            writer.WriteStartArray("children");
            foreach (var id in node.children) writer.WriteNumberValue(id);
            writer.WriteEndArray();
        }
        if (full || old!.flags != node.flags) WriteFlags(writer, node.flags);
        WriteTextGeometry(writer, node, old);
        writer.WriteEndObject();
        FrameworkWorkCounters.Add(FrameworkWork.SemanticsJsonSerializedNode);
    }

    private void WriteFlags(Utf8JsonWriter writer, SemanticsFlags? flags)
    {
        if (flags is null)
        {
            writer.WriteNull("flags");
            return;
        }
        writer.WriteStartObject("flags");
        if (qt) writer.WriteNumber("checkedState", (int)flags.isChecked);
        else writer.WriteString("checked", flags.isChecked.ToString());
        WriteBool(writer, "selected", flags.isSelected);
        WriteBool(writer, "toggled", flags.isToggled);
        WriteBool(writer, "expanded", flags.isExpanded);
        WriteBool(writer, "required", flags.isRequired);
        writer.WriteBoolean("enabled", flags.isEnabled != Tristate.isFalse);
        writer.WriteBoolean("focused", flags.isFocused == Tristate.isTrue);
        writer.WriteBoolean("focusable", flags.isFocused != Tristate.none);
        writer.WriteBoolean("button", flags.isButton);
        writer.WriteBoolean("textField", flags.isTextField);
        writer.WriteBoolean("header", flags.isHeader);
        writer.WriteBoolean("hidden", flags.isHidden);
        writer.WriteBoolean("image", flags.isImage);
        writer.WriteBoolean("slider", flags.isSlider);
        writer.WriteBoolean("readOnly", flags.isReadOnly);
        writer.WriteBoolean("obscured", flags.isObscured);
        writer.WriteBoolean("multiline", flags.isMultiline);
        writer.WriteBoolean("mutuallyExclusive", flags.isInMutuallyExclusiveGroup);
        writer.WriteBoolean("liveRegion", flags.isLiveRegion);
        writer.WriteBoolean("link", flags.isLink);
        writer.WriteEndObject();
    }

    private static void WriteTextGeometry(Utf8JsonWriter writer, SemanticsNodeUpdate node, SemanticsNodeUpdate? old)
    {
        var geometry = node.flags?.isObscured == true ? null : node.textGeometry;
        var previous = old?.flags?.isObscured == true ? null : old?.textGeometry;
        if (ReferenceEquals(previous, geometry) || previous?.ContentEquals(geometry) == true) return;
        if (geometry is not null && previous is not null && SameLayout(previous, geometry))
        {
            writer.WriteStartArray("textBounds");
            for (var index = 0; index < geometry.Runs.Count; index++)
            {
                var bounds = geometry.Runs[index].Bounds;
                if (bounds == previous.Runs[index].Bounds) continue;
                writer.WriteStartArray();
                writer.WriteNumberValue(index);
                WriteRectCoordinates(writer, bounds);
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
            return;
        }
        writer.WritePropertyName("textGeometry");
        JsonSerializer.Serialize(writer, geometry?.ToWire(), NativeSemanticsJson.Default.SemanticsTextGeometryWire);
    }

    private static bool SameLayout(SemanticsTextGeometry left, SemanticsTextGeometry right)
    {
        if (left.Revision != right.Revision || left.Text != right.Text || left.Runs.Count != right.Runs.Count
            || !left.CharacterBoundaries.SequenceEqual(right.CharacterBoundaries)) return false;
        for (var index = 0; index < left.Runs.Count; index++)
        {
            var a = left.Runs[index];
            var b = right.Runs[index];
            if (a.Start != b.Start || a.End != b.End || a.LineStart != b.LineStart
                || a.LineEnd != b.LineEnd || a.Direction != b.Direction) return false;
        }
        return true;
    }

    private static void WriteRect(Utf8JsonWriter writer, string name, Rect rect)
    {
        writer.WriteStartArray(name);
        WriteRectCoordinates(writer, rect);
        writer.WriteEndArray();
    }

    private static void WriteRectCoordinates(Utf8JsonWriter writer, Rect rect)
    {
        writer.WriteNumberValue(rect.left);
        writer.WriteNumberValue(rect.top);
        writer.WriteNumberValue(rect.right);
        writer.WriteNumberValue(rect.bottom);
    }

    private static void WriteNumber(Utf8JsonWriter writer, string name, double? value)
    {
        if (value is { } number && double.IsFinite(number)) writer.WriteNumber(name, number);
        else writer.WriteNull(name);
    }

    private static void WriteNumber(Utf8JsonWriter writer, string name, long? value)
    {
        if (value is { } number) writer.WriteNumber(name, number);
        else writer.WriteNull(name);
    }

    private static void WriteBool(Utf8JsonWriter writer, string name, Tristate state)
    {
        if (state == Tristate.none) writer.WriteNull(name);
        else writer.WriteBoolean(name, state == Tristate.isTrue);
    }
}

[JsonSerializable(typeof(SemanticsTextGeometryWire))]
internal sealed partial class NativeSemanticsJson : JsonSerializerContext;
