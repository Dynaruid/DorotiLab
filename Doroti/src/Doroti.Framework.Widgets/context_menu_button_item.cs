// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/context_menu_button_item.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public enum ContextMenuButtonType
{
    cut,
    copy,
    paste,
    selectAll,
    delete,
    lookUp,
    searchWeb,
    share,
    liveTextInput,
    custom
}

public class ContextMenuButtonItem
{
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual ContextMenuButtonType type { get; private set; } = default!;
    public virtual string? label { get; private set; }

    public ContextMenuButtonItem(global::System.Action? onPressed, ContextMenuButtonType type = ContextMenuButtonType.custom, string? label = null)
    {
        this.onPressed = onPressed;
        this.type = type;
        this.label = label;
    }

    public virtual ContextMenuButtonItem copyWith(global::System.Action? onPressed = null, ContextMenuButtonType? type = null, string? label = null)
    {
        return new ContextMenuButtonItem(onPressed: onPressed ?? this.onPressed, type: type ?? this.type, label: label ?? this.label);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as ContextMenuButtonItem;
        if (__other is null) return false;
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is ContextMenuButtonItem) && (__other.label == label) && Equals(__other.onPressed, onPressed) && Equals(__other.type, type);
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(label, onPressed, type));
    public override string ToString() => $"ContextMenuButtonItem {type}, {label}";
}

