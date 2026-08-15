// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/context_menu_button_item.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

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
        return new ContextMenuButtonItem(onPressed: ((onPressed ?? (global::System.Action)this.onPressed)), type: (type ?? this.type), label: (label ?? this.label));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as ContextMenuButtonItem;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((__other is ContextMenuButtonItem) && (((ContextMenuButtonItem)((ContextMenuButtonItem)__other)).label == this.label)) && (object.Equals((global::System.Action?)((ContextMenuButtonItem)((ContextMenuButtonItem)__other)).onPressed, (global::System.Action?)this.onPressed))) && (object.Equals(((ContextMenuButtonItem)((ContextMenuButtonItem)__other)).type, this.type)));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.label, this.onPressed, this.type));
    public override string ToString() => $"ContextMenuButtonItem {this.type}, {this.label}";
}

