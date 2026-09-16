// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/adaptive_text_selection_toolbar.dart

using Doroti.Runtime;

namespace Doroti.Framework.Material;

public class AdaptiveTextSelectionToolbar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual List<global::Doroti.Framework.Widgets.ContextMenuButtonItem>? buttonItems { get; private set; }
    public virtual List<global::Doroti.Framework.Widgets.Widget>? children { get; private set; }
    public virtual global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors anchors { get; private set; } = default!;

    public AdaptiveTextSelectionToolbar(global::Doroti.Framework.Foundation.Key? key = null, List<global::Doroti.Framework.Widgets.Widget>? children = default!, global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors anchors = default!) : base(key: key)
    {
        this.children = children;
        this.anchors = anchors;
        buttonItems = null;
    }

    public static AdaptiveTextSelectionToolbar CreateButtonItems(global::Doroti.Framework.Foundation.Key? key = null, List<global::Doroti.Framework.Widgets.ContextMenuButtonItem>? buttonItems = default!, global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors anchors = default!)
    {
        var __instance = new AdaptiveTextSelectionToolbar(key: key, children: default!, anchors: anchors);
        __instance.buttonItems = buttonItems;
        __instance.anchors = anchors;
        __instance.children = null;
        return __instance;
    }

    public static AdaptiveTextSelectionToolbar CreateEditable(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.ClipboardStatus clipboardStatus = default!, global::System.Action? onCopy = default!, global::System.Action? onCut = default!, global::System.Action? onPaste = default!, global::System.Action? onSelectAll = default!, global::System.Action? onLookUp = default!, global::System.Action? onSearchWeb = default!, global::System.Action? onShare = default!, global::System.Action? onLiveTextInput = default!, global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors anchors = default!)
    {
        var __instance = new AdaptiveTextSelectionToolbar(key: key, children: default!, anchors: anchors);
        __instance.anchors = anchors;
        __instance.children = null;
        __instance.buttonItems = EditableText.getEditableButtonItems(clipboardStatus: clipboardStatus, onCopy: onCopy, onCut: onCut, onPaste: onPaste, onSelectAll: onSelectAll, onLookUp: onLookUp, onSearchWeb: onSearchWeb, onShare: onShare, onLiveTextInput: onLiveTextInput);
        return __instance;
    }

    public static AdaptiveTextSelectionToolbar CreateEditableText(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.EditableTextState editableTextState = default!)
    {
        var __instance = new AdaptiveTextSelectionToolbar(key: key, children: default!, anchors: default!);
        __instance.children = null;
        __instance.buttonItems = editableTextState.contextMenuButtonItems;
        __instance.anchors = editableTextState.contextMenuAnchors;
        return __instance;
    }

    public static AdaptiveTextSelectionToolbar CreateSelectable(global::Doroti.Framework.Foundation.Key? key = null, global::System.Action onCopy = default!, global::System.Action onSelectAll = default!, global::System.Action? onShare = default!, global::Doroti.Framework.Rendering.SelectionGeometry selectionGeometry = default!, global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors anchors = default!)
    {
        var __instance = new AdaptiveTextSelectionToolbar(key: key, children: default!, anchors: anchors);
        __instance.anchors = anchors;
        __instance.children = null;
        __instance.buttonItems = SelectableRegion.getSelectableButtonItems(selectionGeometry: selectionGeometry, onCopy: onCopy, onSelectAll: onSelectAll, onShare: onShare);
        return __instance;
    }

    public static AdaptiveTextSelectionToolbar CreateSelectableRegion(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.SelectableRegionState selectableRegionState = default!)
    {
        var __instance = new AdaptiveTextSelectionToolbar(key: key, children: default!, anchors: default!);
        __instance.children = null;
        __instance.buttonItems = selectableRegionState.contextMenuButtonItems;
        __instance.anchors = selectableRegionState.contextMenuAnchors;
        return __instance;
    }

    public static string getButtonLabel(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.ContextMenuButtonItem buttonItem)
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
                    DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
                    MaterialLocalizations localizations = MaterialLocalizations.of(context);
                    return buttonItem.type switch { ContextMenuButtonType.cut => localizations.cutButtonLabel, ContextMenuButtonType.copy => localizations.copyButtonLabel, ContextMenuButtonType.paste => localizations.pasteButtonLabel, ContextMenuButtonType.selectAll => localizations.selectAllButtonLabel, ContextMenuButtonType.delete => localizations.deleteButtonTooltip.toUpperCase(), ContextMenuButtonType.lookUp => localizations.lookUpButtonLabel, ContextMenuButtonType.searchWeb => localizations.searchWebButtonLabel, ContextMenuButtonType.share => localizations.shareButtonLabel, ContextMenuButtonType.liveTextInput => localizations.scanTextButtonLabel, ContextMenuButtonType.custom => "", _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static IEnumerable<global::Doroti.Framework.Widgets.Widget> getAdaptiveButtons(global::Doroti.Framework.Widgets.BuildContext context, List<global::Doroti.Framework.Widgets.ContextMenuButtonItem> buttonItems)
    {
        switch (Theme.of(context).platform)
        {
            case TargetPlatform.iOS:
                {
                    return buttonItems.map<global::Doroti.Framework.Widgets.ContextMenuButtonItem, global::Doroti.Framework.Widgets.Widget>((buttonItem) =>
                    {
                        return CupertinoTextSelectionToolbarButton.CreateButtonItem(buttonItem: buttonItem);
                        throw new InvalidOperationException("Dart closure completed without a value.");
                    });
                }
            case TargetPlatform.fuchsia:
            case TargetPlatform.android:
                {
                    var buttons = new List<global::Doroti.Framework.Widgets.Widget>();
                    for (var i = 0L; i < buttonItems.Count; i++)
                    {
                        global::Doroti.Framework.Widgets.ContextMenuButtonItem buttonItemLocal = buttonItems[(int)i];
                        buttons.Add(new TextSelectionToolbarTextButton(padding: TextSelectionToolbarTextButton.getPadding(i, buttonItems.Count), onPressed: buttonItemLocal.onPressed, alignment: AlignmentDirectional.centerStart, child: new global::Doroti.Framework.Widgets.Text(getButtonLabel(context, buttonItemLocal))));
                    }
                    return buttons;
                }
            case TargetPlatform.linux:
            case TargetPlatform.windows:
                {
                    return buttonItems.map<global::Doroti.Framework.Widgets.ContextMenuButtonItem, global::Doroti.Framework.Widgets.Widget>((buttonItem) =>
                    {
                        return DesktopTextSelectionToolbarButton.CreateText(context: context, onPressed: buttonItem.onPressed, text: getButtonLabel(context, buttonItem));
                        throw new InvalidOperationException("Dart closure completed without a value.");
                    });
                }
            case TargetPlatform.macOS:
                {
                    return buttonItems.map<global::Doroti.Framework.Widgets.ContextMenuButtonItem, global::Doroti.Framework.Widgets.Widget>((buttonItem) =>
                    {
                        return CupertinoDesktopTextSelectionToolbarButton.CreateText(onPressed: buttonItem.onPressed, text: getButtonLabel(context, buttonItem));
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
        if ((children is null || !children.Any()) && (buttonItems is null || !buttonItems.Any()))
        {
            return SizedBox.CreateShrink();
        }
        List<global::Doroti.Framework.Widgets.Widget> resultChildren = ((children is not null) ? children! : getAdaptiveButtons(context, buttonItems!).ToList()).ToList();
        switch (Theme.of(context).platform)
        {
            case TargetPlatform.iOS:
                {
                    return new CupertinoTextSelectionToolbar(anchorAbove: anchors.primaryAnchor, anchorBelow: (anchors.secondaryAnchor is null) ? anchors.primaryAnchor : DartRuntimePrimitives.RequireValue(anchors.secondaryAnchor), children: resultChildren);
                }
            case TargetPlatform.android:
                {
                    return new TextSelectionToolbar(anchorAbove: anchors.primaryAnchor, anchorBelow: (anchors.secondaryAnchor is null) ? anchors.primaryAnchor : DartRuntimePrimitives.RequireValue(anchors.secondaryAnchor), children: resultChildren);
                }
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
                {
                    return new DesktopTextSelectionToolbar(anchor: anchors.primaryAnchor, children: resultChildren);
                }
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
