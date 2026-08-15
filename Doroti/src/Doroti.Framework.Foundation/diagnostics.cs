// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/foundation/diagnostics.dart
using System.Collections;
using System.Globalization;
using System.Text;
using Doroti.Runtime;

namespace Doroti.Generated.Framework.Foundation;

public delegate T? ComputePropertyValueCallback<out T>();

public enum DiagnosticLevel
{
    hidden,
    fine,
    debug,
    info,
    warning,
    hint,
    summary,
    error,
    off,
}

public enum DiagnosticsTreeStyle
{
    none,
    sparse,
    offstage,
    dense,
    transition,
    error,
    whitespace,
    flat,
    singleLine,
    errorProperty,
    shallow,
    truncateChildren,
}

public interface Diagnosticable
{
    string toStringShort() => DiagnosticsLibrary.describeIdentity(this);

    void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
    }

    DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null) =>
        new DiagnosticableNode<Diagnosticable>(name, this, style: style);
}

public static class DiagnosticableDefaults
{
    public static void debugFillProperties(DiagnosticPropertiesBuilder properties) =>
        ArgumentNullException.ThrowIfNull(properties);
}

public interface DiagnosticableTree : Diagnosticable
{
    IEnumerable<DiagnosticsNode> debugDescribeChildren() => [];

    new DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null) =>
        new DiagnosticableTreeNode<DiagnosticableTree>(name, this, style: style);

    string toStringDeep(string prefixLineOne = "", string? prefixOtherLines = null, DiagnosticLevel minLevel = DiagnosticLevel.debug, long? wrapWidth = null) =>
        toDiagnosticsNode().toStringDeep(prefixLineOne, prefixOtherLines, minLevel, wrapWidth);
}

public abstract class DiagnosticableTreeMixin : DiagnosticableTree
{
    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
    }

    public virtual IEnumerable<DiagnosticsNode> debugDescribeChildren() => [];

    public DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null) =>
        new DiagnosticableTreeNode<DiagnosticableTree>(name, this, style: style);

    public virtual string toStringDeep(string prefixLineOne = "", string? prefixOtherLines = null, DiagnosticLevel minLevel = DiagnosticLevel.debug, long? wrapWidth = null) =>
        toDiagnosticsNode().toStringDeep(prefixLineOne, prefixOtherLines, minLevel, wrapWidth);

    public virtual string toStringShallow(string joiner = ", ", DiagnosticLevel minLevel = DiagnosticLevel.debug) =>
        string.Join(joiner, debugDescribeChildren().Where(item => !item.isFiltered(minLevel)));

    public override string ToString() => toDiagnosticsNode().ToString();
}

public class DiagnosticPropertiesBuilder
{
    private readonly List<DiagnosticsNode> _properties = [];

    public IReadOnlyList<DiagnosticsNode> properties => _properties;

    public DiagnosticsTreeStyle defaultDiagnosticsTreeStyle { get; set; } = DiagnosticsTreeStyle.sparse;

    public string? emptyBodyDescription { get; set; }

    public void add(DiagnosticsNode property)
    {
        ArgumentNullException.ThrowIfNull(property);
        _properties.Add(property);
    }

    // CLR collection member spelling used by generated Dart List lowering.
    public void Add(DiagnosticsNode property) => add(property);

    public void addAll(IEnumerable<DiagnosticsNode> properties)
    {
        ArgumentNullException.ThrowIfNull(properties);
        foreach (var property in properties)
        {
            add(property);
        }
    }
}

public class DiagnosticsNode
{
    private readonly Func<string>? _description;
    private readonly Func<IEnumerable<DiagnosticsNode>>? _properties;
    private readonly Func<IEnumerable<DiagnosticsNode>>? _children;

    public DiagnosticsNode(
        string? name = null,
        object? value = null,
        DiagnosticsTreeStyle style = DiagnosticsTreeStyle.sparse,
        DiagnosticLevel level = DiagnosticLevel.info,
        bool showName = true,
        bool showSeparator = true,
        bool allowWrap = true,
        Func<string>? description = null,
        Func<IEnumerable<DiagnosticsNode>>? properties = null,
        Func<IEnumerable<DiagnosticsNode>>? children = null)
    {
        this.name = name;
        this.value = value;
        this.style = style;
        this.level = level;
        this.showName = showName;
        this.showSeparator = showSeparator;
        this.allowWrap = allowWrap;
        _description = description;
        _properties = properties;
        _children = children;
    }

    public DiagnosticsNode toDiagnosticsNode() => this;

    public static DiagnosticsNode CreateMessage(string message) =>
        new(value: message, description: () => message);

    public string? name { get; }
    public object? value { get; }
    public DiagnosticsTreeStyle style { get; }
    public DiagnosticLevel level { get; }
    public bool showName { get; }
    public bool showSeparator { get; }
    public bool allowWrap { get; }
    public virtual bool allowTruncate => false;
    public virtual bool isFiltered(DiagnosticLevel minLevel) => level < minLevel;
    public virtual string toDescription() => _description?.Invoke() ?? value?.ToString() ?? "null";
    public virtual IEnumerable<DiagnosticsNode> getProperties() => _properties?.Invoke() ?? [];
    public virtual IEnumerable<DiagnosticsNode> getChildren() => _children?.Invoke() ?? [];

    public virtual DartMap<string, string> toTimelineArguments() => new()
    {
        ["name"] = name ?? string.Empty,
        ["description"] = toDescription(),
    };

    public virtual DartMap<string, object?> toJsonMap(DiagnosticsSerializationDelegate? serializer = null)
    {
        serializer ??= new DiagnosticsSerializationDelegate();
        var result = new DartMap<string, object?>
        {
            ["name"] = name,
            ["description"] = toDescription(),
            ["level"] = level.ToString(),
            ["showName"] = showName,
            ["showSeparator"] = showSeparator,
            ["style"] = style.ToString(),
        };
        if (serializer.includeProperties)
        {
            result["properties"] = getProperties().Where(item => !item.isFiltered(serializer.minLevel)).Select(item => item.toJsonMap(serializer)).ToArray();
        }
        if (serializer.subtreeDepth > 0)
        {
            result["children"] = getChildren().Where(item => !item.isFiltered(serializer.minLevel)).Select(item => item.toJsonMap(serializer.copyWith(subtreeDepth: serializer.subtreeDepth - 1))).ToArray();
        }
        return result;
    }

    public virtual DartMap<string, object?> toJsonMapIterative(DiagnosticsSerializationDelegate? serializer = null) =>
        toJsonMap(serializer);

    public static List<DartMap<string, object>> toJsonList(
        List<DiagnosticsNode>? nodes,
        DiagnosticsNode? parent,
        DiagnosticsSerializationDelegate serializer)
    {
        if (nodes is null)
        {
            return [];
        }

        var originalCount = nodes.Count;
        var selected = serializer.truncateNodesList(nodes, parent).ToList();
        var truncated = selected.Count != originalCount;
        if (truncated)
        {
            selected.Add(CreateMessage("..."));
        }

        var result = selected.Select(node =>
        {
            var source = node.toJsonMap(serializer.delegateForNode(node));
            return new DartMap<string, object>(source.Select(pair =>
                new KeyValuePair<string, object>(pair.Key, pair.Value!)));
        }).ToList();
        if (truncated)
        {
            result[^1]["truncated"] = true;
        }
        return result;
    }

    public virtual string toStringDeep(string prefixLineOne = "", string? prefixOtherLines = null, DiagnosticLevel minLevel = DiagnosticLevel.debug, long? wrapWidth = null) =>
        new TextTreeRenderer().render(this, prefixLineOne, prefixOtherLines ?? prefixLineOne, minLevel);

    public override string ToString()
    {
        var description = toDescription();
        if (!showName || string.IsNullOrEmpty(name))
        {
            return description;
        }
        return showSeparator ? $"{name}: {description}" : $"{name}{description}";
    }
}

public class DiagnosticableNode<T> : DiagnosticsNode
    where T : class, Diagnosticable
{
    public DiagnosticableNode(string? name, T value, DiagnosticsTreeStyle? style = null)
        : base(name, value, style ?? DiagnosticsTreeStyle.sparse, description: value.toStringShort, properties: () => Properties(value))
    {
        this.value = value;
    }

    public new T value { get; }

    private static IEnumerable<DiagnosticsNode> Properties(T value)
    {
        var builder = new DiagnosticPropertiesBuilder();
        value.debugFillProperties(builder);
        return builder.properties;
    }
}

public class DiagnosticableTreeNode<T> : DiagnosticableNode<T>
    where T : class, DiagnosticableTree
{
    public DiagnosticableTreeNode(string? name, T value, DiagnosticsTreeStyle? style = null)
        : base(name, value, style)
    {
        treeValue = value;
    }

    public T treeValue { get; }

    public override IEnumerable<DiagnosticsNode> getChildren() => treeValue.debugDescribeChildren();
}

public class DiagnosticsProperty<T> : DiagnosticsNode
{
    private readonly ComputePropertyValueCallback<T>? _computeValue;
    private readonly object? _defaultValue;

    public DiagnosticsProperty(
        string? name,
        object? value = default,
        DiagnosticsTreeStyle style = DiagnosticsTreeStyle.singleLine,
        DiagnosticLevel level = DiagnosticLevel.info,
        bool showName = true,
        object? defaultValue = null,
        string? description = null,
        ComputePropertyValueCallback<T>? computeValue = null,
        string? ifNull = null,
        bool missingIfNull = false,
        string? tooltip = null,
        bool showSeparator = true,
        string? linePrefix = null)
        : base(name, value, style, level, showName, showSeparator, description: value is null && ifNull is not null ? () => ifNull : description is null ? null : () => description)
    {
        _computeValue = computeValue;
        _defaultValue = defaultValue;
        _ = missingIfNull;
        _ = tooltip;
        _ = linePrefix;
    }

    public DiagnosticsProperty(string? name, object? value, string description)
        : this(name, value, DiagnosticsTreeStyle.singleLine, description: description)
    {
    }

    public DiagnosticsProperty(string? name, object? value, object? expandableValue, DiagnosticsTreeStyle style = DiagnosticsTreeStyle.singleLine)
        : this(name, value, style: style)
    {
        _ = expandableValue;
    }

    public new T? value => propertyValue;

    public T? propertyValue => _computeValue is null ? base.value is T typed ? typed : default : _computeValue();

    public bool isInteresting => true;

    public override string toDescription() => propertyValue?.ToString() ?? "null";

    public override bool isFiltered(DiagnosticLevel minLevel) => base.isFiltered(minLevel) ||
        _defaultValue is not null && Equals(propertyValue, _defaultValue);
}

public sealed class StringProperty : DiagnosticsProperty<string>
{
    public StringProperty(string? name, string? value, string? description = null, bool quoted = true, DiagnosticLevel level = DiagnosticLevel.info, string? defaultValue = null, bool showName = true)
        : base(name, value, level: level, description: description, defaultValue: defaultValue, showName: showName)
    {
        this.quoted = quoted;
    }

    public bool quoted { get; }

    public override string toDescription()
    {
        var text = base.toDescription();
        return quoted && propertyValue is not null ? $"\"{text}\"" : text;
    }
}

public sealed class DoubleProperty : DiagnosticsProperty<double?>
{
    public DoubleProperty(string? name, double? value, string? unit = null, DiagnosticLevel level = DiagnosticLevel.info, double? defaultValue = null, string? ifNull = null, string? tooltip = null)
        : base(name, value, level: level, defaultValue: defaultValue, ifNull: ifNull, tooltip: tooltip) => this.unit = unit;
    public DoubleProperty(string? name, Func<double> computeValue, string? unit = null, DiagnosticLevel level = DiagnosticLevel.info, double? defaultValue = null, string? ifNull = null, string? tooltip = null)
        : base(name, computeValue: () => computeValue(), level: level, defaultValue: defaultValue, ifNull: ifNull, tooltip: tooltip) => this.unit = unit;
    public string? unit { get; }
    public override string toDescription() => propertyValue is { } value ? $"{value.ToString("G", CultureInfo.InvariantCulture)}{unit}" : "null";
}

public sealed class IntProperty : DiagnosticsProperty<long?>
{
    public IntProperty(string? name, long? value, string? unit = null, DiagnosticLevel level = DiagnosticLevel.info, long? defaultValue = null, string? ifNull = null, string? tooltip = null)
        : base(name, value, level: level, defaultValue: defaultValue, ifNull: ifNull, tooltip: tooltip) => this.unit = unit;
    public string? unit { get; }
    public override string toDescription() => propertyValue is { } value ? $"{value.ToString(CultureInfo.InvariantCulture)}{unit}" : "null";
}

public sealed class PercentProperty : DiagnosticsProperty<double?>
{
    public PercentProperty(string? name, double? fraction, DiagnosticLevel level = DiagnosticLevel.info, string? unit = null, string? tooltip = null, bool showName = true, string? ifNull = null)
        : base(name, fraction, level: level, tooltip: tooltip, showName: showName, ifNull: ifNull) => this.unit = unit;
    public string? unit { get; }
    public override string toDescription() => propertyValue is { } value ? $"{value * 100:0.0}{unit ?? "%"}" : "null";
}

public sealed class EnumProperty<T> : DiagnosticsProperty<T?>
    where T : struct, Enum
{
    public EnumProperty(string? name, T? value, DiagnosticLevel level = DiagnosticLevel.info, T? defaultValue = null) : base(name, value, level: level, defaultValue: defaultValue) { }
}

public sealed class FlagProperty : DiagnosticsProperty<bool?>
{
    public FlagProperty(string? name, bool? value, string ifTrue = "true", string? ifFalse = null, bool showName = true, DiagnosticLevel level = DiagnosticLevel.info, bool? defaultValue = null)
        : base(name, value, level: level, defaultValue: defaultValue, showName: showName)
    {
        this.ifTrue = ifTrue;
        this.ifFalse = ifFalse;
        _ = showName;
    }
    public string ifTrue { get; }
    public string? ifFalse { get; }
    public override string toDescription() => propertyValue switch { true => ifTrue, false => ifFalse ?? "false", null => "null" };
}

public sealed class ObjectFlagProperty<T> : DiagnosticsProperty<T>
{
    public ObjectFlagProperty(string? name, T? value, string ifPresent = "present", DiagnosticLevel level = DiagnosticLevel.info, string? ifNull = null)
        : base(name, value, level: level, description: value is null ? ifNull : null) => this.ifPresent = ifPresent;
    public string ifPresent { get; }
    public override string toDescription() => propertyValue is null ? "null" : ifPresent;
    public static ObjectFlagProperty<T> CreateHas(
        string? name,
        T? value,
        DiagnosticLevel level = DiagnosticLevel.info) =>
        new(name, value, "has value", level);
}

public sealed class IterableProperty<T> : DiagnosticsProperty<IEnumerable<T>>
{
    private readonly string? _ifEmpty;
    public IterableProperty(string? name, IEnumerable<T>? value, DiagnosticLevel level = DiagnosticLevel.info, IEnumerable<T>? defaultValue = null, DiagnosticsTreeStyle style = DiagnosticsTreeStyle.singleLine, string? ifEmpty = null, string? ifNull = null) : base(name, value, level: level, defaultValue: defaultValue, style: style, ifNull: ifNull) => _ifEmpty = ifEmpty;
    public override string toDescription() => propertyValue is null ? "null" : propertyValue.Any() ? $"[{string.Join(", ", propertyValue)}]" : _ifEmpty ?? "[]";
}

public sealed class MessageProperty : DiagnosticsNode
{
    public MessageProperty(string message, DiagnosticLevel level = DiagnosticLevel.info) : base(null, message, level: level, showName: false) { }
    public MessageProperty(string? name, string message, DiagnosticLevel level = DiagnosticLevel.info) : base(name, message, level: level) { }
}

public sealed class FlagsSummary : DiagnosticsNode
{
    public FlagsSummary(string? name, IReadOnlyDictionary<string, bool?> values, string? ifEmpty = null)
        : base(name, values, description: () => string.Join(", ", values.Where(item => item.Value == true).Select(item => item.Key).DefaultIfEmpty(ifEmpty ?? "none"))) { }
}

public sealed class FlagsSummary<T> : DiagnosticsNode
{
    public FlagsSummary(string? name, IReadOnlyDictionary<string, T> values, string? ifEmpty = null)
        : base(name, values, description: () => string.Join(", ", values.Where(item => item.Value is not null).Select(item => item.Key).DefaultIfEmpty(ifEmpty ?? "none"))) { }
}

public class DiagnosticsBlock : DiagnosticsNode
{
    private readonly IReadOnlyList<DiagnosticsNode> _children;
    public DiagnosticsBlock(
        string? name,
        string description,
        IEnumerable<DiagnosticsNode> children,
        DiagnosticsTreeStyle style = DiagnosticsTreeStyle.whitespace,
        bool showSeparator = true)
        : base(name, description, style, showSeparator: showSeparator) => _children = [.. children];
    public DiagnosticsBlock(string? name = null, IEnumerable<DiagnosticsNode>? children = null, string description = "", DiagnosticsTreeStyle style = DiagnosticsTreeStyle.whitespace, bool allowTruncate = false)
        : this(name, description, children ?? [], style) => this.allowTruncate = allowTruncate;
    public DiagnosticsBlock(string? name = null, IEnumerable<DiagnosticsNode>? properties = null, string description = "", DiagnosticsTreeStyle style = DiagnosticsTreeStyle.whitespace, bool allowTruncate = false, object? propertiesCompatibility = null)
        : this(name, description, properties ?? [], style)
    {
        this.allowTruncate = allowTruncate;
        _ = propertiesCompatibility;
    }
    public override bool allowTruncate { get; }
    public override IEnumerable<DiagnosticsNode> getChildren() => _children;
}

public class DiagnosticsSerializationDelegate
{
    public DiagnosticsSerializationDelegate(int subtreeDepth = 0, bool includeProperties = true, DiagnosticLevel minLevel = DiagnosticLevel.debug)
    {
        this.subtreeDepth = subtreeDepth;
        this.includeProperties = includeProperties;
        this.minLevel = minLevel;
    }
    public int subtreeDepth { get; }
    public bool includeProperties { get; }
    public DiagnosticLevel minLevel { get; }
    public DiagnosticsSerializationDelegate copyWith(int? subtreeDepth = null, bool? includeProperties = null, DiagnosticLevel? minLevel = null) =>
        new(subtreeDepth ?? this.subtreeDepth, includeProperties ?? this.includeProperties, minLevel ?? this.minLevel);
    public virtual DiagnosticsSerializationDelegate delegateForNode(DiagnosticsNode node) => this;
    public virtual List<DiagnosticsNode> truncateNodesList(List<DiagnosticsNode> nodes, DiagnosticsNode? owner) => nodes;
}

public sealed record TextTreeConfiguration(
    string prefixLineOne = "",
    string prefixOtherLines = "",
    string prefixLastChildLineOne = "└─",
    string prefixOtherLinesRootNode = "",
    string linkCharacter = "│",
    bool lineBreakProperties = true);

public sealed class TextTreeRenderer
{
    public TextTreeRenderer(
        DiagnosticLevel minLevel = DiagnosticLevel.debug,
        long wrapWidth = 100,
        long wrapWidthProperties = 65,
        long maxDescendentsTruncatableNode = -1)
    {
        _ = minLevel;
        _ = wrapWidth;
        _ = wrapWidthProperties;
        _ = maxDescendentsTruncatableNode;
    }

    public string render(DiagnosticsNode node, string prefixLineOne = "", string prefixOtherLines = "", DiagnosticLevel minLevel = DiagnosticLevel.debug)
    {
        var builder = new StringBuilder();
        RenderNode(builder, node, prefixLineOne, prefixOtherLines, minLevel);
        return builder.ToString().TrimEnd();
    }

    private static void RenderNode(StringBuilder builder, DiagnosticsNode node, string first, string other, DiagnosticLevel minLevel)
    {
        if (node.isFiltered(minLevel))
        {
            return;
        }
        var lines = node.ToString().Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n');
        for (var index = 0; index < lines.Length; index++)
        {
            builder.Append(index == 0 ? first : other).AppendLine(lines[index]);
        }
        var children = node.getProperties().Concat(node.getChildren()).Where(item => !item.isFiltered(minLevel)).ToArray();
        for (var index = 0; index < children.Length; index++)
        {
            var last = index == children.Length - 1;
            RenderNode(builder, children[index], other + (last ? "└─" : "├─"), other + (last ? "  " : "│ "), minLevel);
        }
    }
}

internal sealed class _DefaultDiagnosticsSerializationDelegate : DiagnosticsSerializationDelegate { }
internal sealed class _JsonDiagnosticsNode(string? name, object? value) : DiagnosticsNode(name, value);
internal sealed class _NoDefaultValue { public static _NoDefaultValue instance { get; } = new(); private _NoDefaultValue() { } }
internal sealed class _NodesToJsonifyStack : Stack<DiagnosticsNode>;
internal sealed class _NumProperty(string? name, double? value) : DiagnosticsProperty<double?>(name, value);
internal sealed class _PrefixedStringBuilder
{
    private readonly StringBuilder _builder = new();
    public void write(string value) => _builder.Append(value);
    public void writeln(string value = "") => _builder.AppendLine(value);
    public override string ToString() => _builder.ToString();
}
internal enum _DiagnosticsWordWrapParseMode { inSpace, inWord, atBreak }

public static class DiagnosticsLibrary
{
    public static object kNoDefaultValue => _NoDefaultValue.instance;
    public static TextTreeConfiguration sparseTextConfiguration { get; } = new();
    public static TextTreeConfiguration dashedTextConfiguration { get; } = new(prefixLastChildLineOne: "└╌");
    public static TextTreeConfiguration denseTextConfiguration { get; } = new();
    public static TextTreeConfiguration transitionTextConfiguration { get; } = new();
    public static TextTreeConfiguration errorTextConfiguration { get; } = new(prefixLastChildLineOne: "└─");
    public static TextTreeConfiguration whitespaceTextConfiguration { get; } = new(prefixLastChildLineOne: "  ");
    public static TextTreeConfiguration flatTextConfiguration { get; } = new();
    public static TextTreeConfiguration singleLineTextConfiguration { get; } = new();
    public static TextTreeConfiguration errorPropertyTextConfiguration { get; } = new();
    public static TextTreeConfiguration shallowTextConfiguration { get; } = new();

    public static string describeEnum(Enum value) => value.ToString();
    public static string shortHash(object? value) => ((value?.GetHashCode() ?? 0) & 0xFFFFF).ToString("x5", CultureInfo.InvariantCulture);
    public static string describeIdentity(object? value) => $"{DartRuntimePrimitives.RuntimeTypeName(value)}#{shortHash(value)}";
    internal static bool _isSingleLine(string value) => !value.Contains('\n', StringComparison.Ordinal);
}
