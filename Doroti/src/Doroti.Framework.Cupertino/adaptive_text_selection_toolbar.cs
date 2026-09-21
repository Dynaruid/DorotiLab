// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/adaptive_text_selection_toolbar.dart

using Doroti.Runtime;

namespace Doroti.Framework.Cupertino;

public class CupertinoAdaptiveTextSelectionToolbar : StatelessWidget
{
    public virtual TextSelectionToolbarAnchors anchors { get; private set; } = default!;
    public virtual List<Widget>? children { get; private set; }
    public virtual List<ContextMenuButtonItem>? buttonItems { get; private set; }

    public CupertinoAdaptiveTextSelectionToolbar(
        Key? key = null,
        List<Widget>? children = default!,
        TextSelectionToolbarAnchors anchors = default!
    )
        : base(key: key)
    {
        this.children = children;
        this.anchors = anchors;
        buttonItems = null;
    }

    public static CupertinoAdaptiveTextSelectionToolbar CreateButtonItems(
        Key? key = null,
        List<ContextMenuButtonItem>? buttonItems = default!,
        TextSelectionToolbarAnchors anchors = default!
    )
    {
        var __instance = new CupertinoAdaptiveTextSelectionToolbar(
            key: key,
            children: default!,
            anchors: anchors
        );
        __instance.buttonItems = buttonItems;
        __instance.anchors = anchors;
        __instance.children = null;
        return __instance;
    }

    public static CupertinoAdaptiveTextSelectionToolbar CreateEditable(
        Key? key = null,
        ClipboardStatus clipboardStatus = default!,
        Action? onCopy = default!,
        Action? onCut = default!,
        Action? onPaste = default!,
        Action? onSelectAll = default!,
        Action? onLookUp = default!,
        Action? onSearchWeb = default!,
        Action? onShare = default!,
        Action? onLiveTextInput = default!,
        TextSelectionToolbarAnchors anchors = default!
    )
    {
        var __instance = new CupertinoAdaptiveTextSelectionToolbar(
            key: key,
            children: default!,
            anchors: anchors
        );
        __instance.anchors = anchors;
        __instance.children = null;
        __instance.buttonItems = EditableText.getEditableButtonItems(
            clipboardStatus: clipboardStatus,
            onCopy: onCopy,
            onCut: onCut,
            onPaste: onPaste,
            onSelectAll: onSelectAll,
            onLookUp: onLookUp,
            onSearchWeb: onSearchWeb,
            onShare: onShare,
            onLiveTextInput: onLiveTextInput
        );
        return __instance;
    }

    public static CupertinoAdaptiveTextSelectionToolbar CreateEditableText(
        Key? key = null,
        EditableTextState editableTextState = default!
    )
    {
        var __instance = new CupertinoAdaptiveTextSelectionToolbar(
            key: key,
            children: default!,
            anchors: default!
        );
        __instance.children = null;
        __instance.buttonItems = editableTextState.contextMenuButtonItems;
        __instance.anchors = editableTextState.contextMenuAnchors;
        return __instance;
    }

    public static CupertinoAdaptiveTextSelectionToolbar CreateSelectable(
        Key? key = null,
        Action onCopy = default!,
        Action onSelectAll = default!,
        SelectionGeometry selectionGeometry = default!,
        TextSelectionToolbarAnchors anchors = default!
    )
    {
        var __instance = new CupertinoAdaptiveTextSelectionToolbar(
            key: key,
            children: default!,
            anchors: anchors
        );
        __instance.anchors = anchors;
        __instance.children = null;
        __instance.buttonItems = SelectableRegion.getSelectableButtonItems(
            selectionGeometry: selectionGeometry,
            onCopy: onCopy,
            onSelectAll: onSelectAll,
            onShare: null
        );
        return __instance;
    }

    public static IEnumerable<Widget> getAdaptiveButtons(
        BuildContext context,
        List<ContextMenuButtonItem> buttonItems
    )
    {
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.iOS:
            {
                return buttonItems.map<ContextMenuButtonItem, Widget>(
                    (buttonItem) =>
                    {
                        return CupertinoTextSelectionToolbarButton.CreateButtonItem(
                            buttonItem: buttonItem
                        );
                        throw new InvalidOperationException(
                            "Dart closure completed without a value."
                        );
                    }
                );
            }
            case TargetPlatform.linux:
            case TargetPlatform.windows:
            case TargetPlatform.macOS:
            {
                return buttonItems.map<ContextMenuButtonItem, Widget>(
                    (buttonItem) =>
                    {
                        return CupertinoDesktopTextSelectionToolbarButton.CreateButtonItem(
                            buttonItem: buttonItem
                        );
                        throw new InvalidOperationException(
                            "Dart closure completed without a value."
                        );
                    }
                );
            }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        if (children is null ? buttonItems is null || !buttonItems.Any() : !children.Any())
        {
            return SizedBox.CreateShrink();
        }
        List<Widget> resultChildren = (
            children ?? getAdaptiveButtons(context, buttonItems!).ToList()
        ).ToList();
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.android:
            case TargetPlatform.iOS:
            case TargetPlatform.fuchsia:
            {
                return new CupertinoTextSelectionToolbar(
                    anchorAbove: anchors.primaryAnchor,
                    anchorBelow: anchors.secondaryAnchor ?? anchors.primaryAnchor,
                    children: resultChildren
                );
            }
            case TargetPlatform.linux:
            case TargetPlatform.windows:
            case TargetPlatform.macOS:
            {
                return new CupertinoDesktopTextSelectionToolbar(
                    anchor: anchors.primaryAnchor,
                    children: resultChildren
                );
            }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
