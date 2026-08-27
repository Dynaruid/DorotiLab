// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/adaptive_text_selection_toolbar.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Cupertino;

public class CupertinoAdaptiveTextSelectionToolbar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors anchors { get; private set; } = default!;
    public virtual List<global::Doroti.Framework.Widgets.Widget>? children { get; private set; }
    public virtual List<global::Doroti.Framework.Widgets.ContextMenuButtonItem>? buttonItems { get; private set; }

    public CupertinoAdaptiveTextSelectionToolbar(global::Doroti.Framework.Foundation.Key? key = null, List<global::Doroti.Framework.Widgets.Widget>? children = default!, global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors anchors = default!) : base(key: key)
    {
        this.children = children;
        this.anchors = anchors;
        this.buttonItems = null;
    }

    public static CupertinoAdaptiveTextSelectionToolbar CreateButtonItems(global::Doroti.Framework.Foundation.Key? key = null, List<global::Doroti.Framework.Widgets.ContextMenuButtonItem>? buttonItems = default!, global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors anchors = default!)
    {
        var __instance = new CupertinoAdaptiveTextSelectionToolbar(key: key, children: default!, anchors: anchors);
        __instance.buttonItems = buttonItems;
        __instance.anchors = anchors;
        __instance.children = null;
        return __instance;
    }

    public static CupertinoAdaptiveTextSelectionToolbar CreateEditable(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.ClipboardStatus clipboardStatus = default!, global::System.Action? onCopy = default!, global::System.Action? onCut = default!, global::System.Action? onPaste = default!, global::System.Action? onSelectAll = default!, global::System.Action? onLookUp = default!, global::System.Action? onSearchWeb = default!, global::System.Action? onShare = default!, global::System.Action? onLiveTextInput = default!, global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors anchors = default!)
    {
        var __instance = new CupertinoAdaptiveTextSelectionToolbar(key: key, children: default!, anchors: anchors);
        __instance.anchors = anchors;
        __instance.children = null;
        __instance.buttonItems = EditableText.getEditableButtonItems(clipboardStatus: clipboardStatus, onCopy: () => onCopy(), onCut: () => onCut(), onPaste: () => onPaste(), onSelectAll: () => onSelectAll(), onLookUp: () => onLookUp(), onSearchWeb: () => onSearchWeb(), onShare: () => onShare(), onLiveTextInput: () => onLiveTextInput());
        return __instance;
    }

    public static CupertinoAdaptiveTextSelectionToolbar CreateEditableText(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.EditableTextState editableTextState = default!)
    {
        var __instance = new CupertinoAdaptiveTextSelectionToolbar(key: key, children: default!, anchors: default!);
        __instance.children = null;
        __instance.buttonItems = ((global::Doroti.Framework.Widgets.EditableTextState)editableTextState).contextMenuButtonItems;
        __instance.anchors = ((global::Doroti.Framework.Widgets.EditableTextState)editableTextState).contextMenuAnchors;
        return __instance;
    }

    public static CupertinoAdaptiveTextSelectionToolbar CreateSelectable(global::Doroti.Framework.Foundation.Key? key = null, global::System.Action onCopy = default!, global::System.Action onSelectAll = default!, global::Doroti.Framework.Rendering.SelectionGeometry selectionGeometry = default!, global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors anchors = default!)
    {
        var __instance = new CupertinoAdaptiveTextSelectionToolbar(key: key, children: default!, anchors: anchors);
        __instance.anchors = anchors;
        __instance.children = null;
        __instance.buttonItems = SelectableRegion.getSelectableButtonItems(selectionGeometry: selectionGeometry, onCopy: () => onCopy(), onSelectAll: () => onSelectAll(), onShare: null);
        return __instance;
    }

    public static IEnumerable<global::Doroti.Framework.Widgets.Widget> getAdaptiveButtons(global::Doroti.Framework.Widgets.BuildContext context, List<global::Doroti.Framework.Widgets.ContextMenuButtonItem> buttonItems)
    {
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                {
                    return buttonItems.map<global::Doroti.Framework.Widgets.ContextMenuButtonItem, global::Doroti.Framework.Widgets.Widget>(((buttonItem) =>
                    {
                        return CupertinoTextSelectionToolbarButton.CreateButtonItem(buttonItem: buttonItem);
                        throw new InvalidOperationException("Dart closure completed without a value.");
                    }));
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                {
                    return buttonItems.map<global::Doroti.Framework.Widgets.ContextMenuButtonItem, global::Doroti.Framework.Widgets.Widget>(((buttonItem) =>
                    {
                        return CupertinoDesktopTextSelectionToolbarButton.CreateButtonItem(buttonItem: buttonItem);
                        throw new InvalidOperationException("Dart closure completed without a value.");
                    }));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if (this.children is null ? !this.buttonItems.Any() : !this.children.Any())
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.SizedBox.CreateShrink());
        }
        List<global::Doroti.Framework.Widgets.Widget> resultChildren = (this.children ?? CupertinoAdaptiveTextSelectionToolbar.getAdaptiveButtons(context, this.buttonItems!).ToList()).ToList();
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                {
                    return ((global::Doroti.Framework.Widgets.Widget)(object?)new CupertinoTextSelectionToolbar(anchorAbove: ((global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors)this.anchors).primaryAnchor, anchorBelow: (((global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors)this.anchors).secondaryAnchor ?? ((global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors)this.anchors).primaryAnchor), children: resultChildren));
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                {
                    return ((global::Doroti.Framework.Widgets.Widget)(object?)new CupertinoDesktopTextSelectionToolbar(anchor: ((global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors)this.anchors).primaryAnchor, children: resultChildren));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
