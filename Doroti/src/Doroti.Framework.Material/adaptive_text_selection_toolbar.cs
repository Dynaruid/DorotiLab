// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/adaptive_text_selection_toolbar.dart

using Doroti.Runtime;

namespace Doroti.Framework.Material;

public class AdaptiveTextSelectionToolbar : StatelessWidget
{
    public virtual List<ContextMenuButtonItem>? buttonItems { get; private set; }
    public virtual List<Widget>? children { get; private set; }
    public virtual TextSelectionToolbarAnchors anchors { get; private set; } = default!;

    public AdaptiveTextSelectionToolbar(
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

    public static AdaptiveTextSelectionToolbar CreateButtonItems(
        Key? key = null,
        List<ContextMenuButtonItem>? buttonItems = default!,
        TextSelectionToolbarAnchors anchors = default!
    )
    {
        var __instance = new AdaptiveTextSelectionToolbar(
            key: key,
            children: default!,
            anchors: anchors
        );
        __instance.buttonItems = buttonItems;
        __instance.anchors = anchors;
        __instance.children = null;
        return __instance;
    }

    public static AdaptiveTextSelectionToolbar CreateEditable(
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
        var __instance = new AdaptiveTextSelectionToolbar(
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

    public static AdaptiveTextSelectionToolbar CreateEditableText(
        Key? key = null,
        EditableTextState editableTextState = default!
    )
    {
        var __instance = new AdaptiveTextSelectionToolbar(
            key: key,
            children: default!,
            anchors: default!
        );
        __instance.children = null;
        __instance.buttonItems = editableTextState.contextMenuButtonItems;
        __instance.anchors = editableTextState.contextMenuAnchors;
        return __instance;
    }

    public static AdaptiveTextSelectionToolbar CreateSelectable(
        Key? key = null,
        Action onCopy = default!,
        Action onSelectAll = default!,
        Action? onShare = default!,
        SelectionGeometry selectionGeometry = default!,
        TextSelectionToolbarAnchors anchors = default!
    )
    {
        var __instance = new AdaptiveTextSelectionToolbar(
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
            onShare: onShare
        );
        return __instance;
    }

    public static AdaptiveTextSelectionToolbar CreateSelectableRegion(
        Key? key = null,
        SelectableRegionState selectableRegionState = default!
    )
    {
        var __instance = new AdaptiveTextSelectionToolbar(
            key: key,
            children: default!,
            anchors: default!
        );
        __instance.children = null;
        __instance.buttonItems = selectableRegionState.contextMenuButtonItems;
        __instance.anchors = selectableRegionState.contextMenuAnchors;
        return __instance;
    }

    public static string getButtonLabel(BuildContext context, ContextMenuButtonItem buttonItem)
    {
        if (buttonItem.label is not null)
        {
            return buttonItem.label!;
        }
        switch (Theme.of(context).platform)
        {
            case TargetPlatform.iOS:
            case TargetPlatform.macOS:
            {
                return CupertinoTextSelectionToolbarButton.getButtonLabel(context, buttonItem);
            }
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
            {
                DartRuntimePrimitives.Assert(() =>
                    DebugLibrary.debugCheckHasMaterialLocalizations(context)
                );
                MaterialLocalizations localizations = MaterialLocalizations.of(context);
                return buttonItem.type switch
                {
                    ContextMenuButtonType.cut => localizations.cutButtonLabel,
                    ContextMenuButtonType.copy => localizations.copyButtonLabel,
                    ContextMenuButtonType.paste => localizations.pasteButtonLabel,
                    ContextMenuButtonType.selectAll => localizations.selectAllButtonLabel,
                    ContextMenuButtonType.delete => localizations.deleteButtonTooltip.toUpperCase(),
                    ContextMenuButtonType.lookUp => localizations.lookUpButtonLabel,
                    ContextMenuButtonType.searchWeb => localizations.searchWebButtonLabel,
                    ContextMenuButtonType.share => localizations.shareButtonLabel,
                    ContextMenuButtonType.liveTextInput => localizations.scanTextButtonLabel,
                    ContextMenuButtonType.custom => "",
                    _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                        throw new InvalidOperationException("Non-exhaustive Dart switch value."),
                };
            }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static IEnumerable<Widget> getAdaptiveButtons(
        BuildContext context,
        List<ContextMenuButtonItem> buttonItems
    )
    {
        switch (Theme.of(context).platform)
        {
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
            case TargetPlatform.fuchsia:
            case TargetPlatform.android:
            {
                var buttons = new List<Widget>();
                for (var i = 0L; i < buttonItems.Count; i++)
                {
                    ContextMenuButtonItem buttonItemLocal = buttonItems[(int)i];
                    buttons.Add(
                        new TextSelectionToolbarTextButton(
                            padding: TextSelectionToolbarTextButton.getPadding(
                                i,
                                buttonItems.Count
                            ),
                            onPressed: buttonItemLocal.onPressed,
                            alignment: AlignmentDirectional.centerStart,
                            child: new Text(getButtonLabel(context, buttonItemLocal))
                        )
                    );
                }
                return buttons;
            }
            case TargetPlatform.linux:
            case TargetPlatform.windows:
            {
                return buttonItems.map<ContextMenuButtonItem, Widget>(
                    (buttonItem) =>
                    {
                        return DesktopTextSelectionToolbarButton.CreateText(
                            context: context,
                            onPressed: buttonItem.onPressed,
                            text: getButtonLabel(context, buttonItem)
                        );
                        throw new InvalidOperationException(
                            "Dart closure completed without a value."
                        );
                    }
                );
            }
            case TargetPlatform.macOS:
            {
                return buttonItems.map<ContextMenuButtonItem, Widget>(
                    (buttonItem) =>
                    {
                        return CupertinoDesktopTextSelectionToolbarButton.CreateText(
                            onPressed: buttonItem.onPressed,
                            text: getButtonLabel(context, buttonItem)
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
        if ((children is null || !children.Any()) && (buttonItems is null || !buttonItems.Any()))
        {
            return SizedBox.CreateShrink();
        }
        List<Widget> resultChildren = (
            (children is not null) ? children! : getAdaptiveButtons(context, buttonItems!).ToList()
        ).ToList();
        switch (Theme.of(context).platform)
        {
            case TargetPlatform.iOS:
            {
                return new CupertinoTextSelectionToolbar(
                    anchorAbove: anchors.primaryAnchor,
                    anchorBelow: (anchors.secondaryAnchor is null)
                        ? anchors.primaryAnchor
                        : DartRuntimePrimitives.RequireValue(anchors.secondaryAnchor),
                    children: resultChildren
                );
            }
            case TargetPlatform.android:
            {
                return new TextSelectionToolbar(
                    anchorAbove: anchors.primaryAnchor,
                    anchorBelow: (anchors.secondaryAnchor is null)
                        ? anchors.primaryAnchor
                        : DartRuntimePrimitives.RequireValue(anchors.secondaryAnchor),
                    children: resultChildren
                );
            }
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
            {
                return new DesktopTextSelectionToolbar(
                    anchor: anchors.primaryAnchor,
                    children: resultChildren
                );
            }
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
