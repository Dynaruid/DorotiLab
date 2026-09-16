// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/adaptive_text_selection_toolbar.dart

using Doroti.Runtime;

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
        buttonItems = null;
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
        __instance.buttonItems = EditableText.getEditableButtonItems(clipboardStatus: clipboardStatus, onCopy: onCopy, onCut: onCut, onPaste: onPaste, onSelectAll: onSelectAll, onLookUp: onLookUp, onSearchWeb: onSearchWeb, onShare: onShare, onLiveTextInput: onLiveTextInput);
        return __instance;
    }

    public static CupertinoAdaptiveTextSelectionToolbar CreateEditableText(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.EditableTextState editableTextState = default!)
    {
        var __instance = new CupertinoAdaptiveTextSelectionToolbar(key: key, children: default!, anchors: default!);
        __instance.children = null;
        __instance.buttonItems = editableTextState.contextMenuButtonItems;
        __instance.anchors = editableTextState.contextMenuAnchors;
        return __instance;
    }

    public static CupertinoAdaptiveTextSelectionToolbar CreateSelectable(global::Doroti.Framework.Foundation.Key? key = null, global::System.Action onCopy = default!, global::System.Action onSelectAll = default!, global::Doroti.Framework.Rendering.SelectionGeometry selectionGeometry = default!, global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors anchors = default!)
    {
        var __instance = new CupertinoAdaptiveTextSelectionToolbar(key: key, children: default!, anchors: anchors);
        __instance.anchors = anchors;
        __instance.children = null;
        __instance.buttonItems = SelectableRegion.getSelectableButtonItems(selectionGeometry: selectionGeometry, onCopy: onCopy, onSelectAll: onSelectAll, onShare: null);
        return __instance;
    }

    public static IEnumerable<global::Doroti.Framework.Widgets.Widget> getAdaptiveButtons(global::Doroti.Framework.Widgets.BuildContext context, List<global::Doroti.Framework.Widgets.ContextMenuButtonItem> buttonItems)
    {
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.iOS:
                {
                    return buttonItems.map<global::Doroti.Framework.Widgets.ContextMenuButtonItem, global::Doroti.Framework.Widgets.Widget>((buttonItem) =>
                    {
                        return CupertinoTextSelectionToolbarButton.CreateButtonItem(buttonItem: buttonItem);
                        throw new InvalidOperationException("Dart closure completed without a value.");
                    });
                }
            case TargetPlatform.linux:
            case TargetPlatform.windows:
            case TargetPlatform.macOS:
                {
                    return buttonItems.map<global::Doroti.Framework.Widgets.ContextMenuButtonItem, global::Doroti.Framework.Widgets.Widget>((buttonItem) =>
                    {
                        return CupertinoDesktopTextSelectionToolbarButton.CreateButtonItem(buttonItem: buttonItem);
                        throw new InvalidOperationException("Dart closure completed without a value.");
                    });
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if (children is null ? buttonItems is null || !buttonItems.Any() : !children.Any())
        {
            return SizedBox.CreateShrink();
        }
        List<global::Doroti.Framework.Widgets.Widget> resultChildren = (children ?? getAdaptiveButtons(context, buttonItems!).ToList()).ToList();
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.android:
            case TargetPlatform.iOS:
            case TargetPlatform.fuchsia:
                {
                    return new CupertinoTextSelectionToolbar(anchorAbove: anchors.primaryAnchor, anchorBelow: anchors.secondaryAnchor ?? anchors.primaryAnchor, children: resultChildren);
                }
            case TargetPlatform.linux:
            case TargetPlatform.windows:
            case TargetPlatform.macOS:
                {
                    return new CupertinoDesktopTextSelectionToolbar(anchor: anchors.primaryAnchor, children: resultChildren);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
