// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/adaptive_text_selection_toolbar.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

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
        this.buttonItems = null;
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
        __instance.buttonItems = EditableText.getEditableButtonItems(clipboardStatus: clipboardStatus, onCopy: () => onCopy(), onCut: () => onCut(), onPaste: () => onPaste(), onSelectAll: () => onSelectAll(), onLookUp: () => onLookUp(), onSearchWeb: () => onSearchWeb(), onShare: () => onShare(), onLiveTextInput: () => onLiveTextInput());
        return __instance;
    }

    public static AdaptiveTextSelectionToolbar CreateEditableText(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.EditableTextState editableTextState = default!)
    {
        var __instance = new AdaptiveTextSelectionToolbar(key: key, children: default!, anchors: default!);
        __instance.children = null;
        __instance.buttonItems = ((global::Doroti.Framework.Widgets.EditableTextState)editableTextState).contextMenuButtonItems;
        __instance.anchors = ((global::Doroti.Framework.Widgets.EditableTextState)editableTextState).contextMenuAnchors;
        return __instance;
    }

    public static AdaptiveTextSelectionToolbar CreateSelectable(global::Doroti.Framework.Foundation.Key? key = null, global::System.Action onCopy = default!, global::System.Action onSelectAll = default!, global::System.Action? onShare = default!, global::Doroti.Framework.Rendering.SelectionGeometry selectionGeometry = default!, global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors anchors = default!)
    {
        var __instance = new AdaptiveTextSelectionToolbar(key: key, children: default!, anchors: anchors);
        __instance.anchors = anchors;
        __instance.children = null;
        __instance.buttonItems = SelectableRegion.getSelectableButtonItems(selectionGeometry: selectionGeometry, onCopy: () => onCopy(), onSelectAll: () => onSelectAll(), onShare: () => onShare());
        return __instance;
    }

    public static AdaptiveTextSelectionToolbar CreateSelectableRegion(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.SelectableRegionState selectableRegionState = default!)
    {
        var __instance = new AdaptiveTextSelectionToolbar(key: key, children: default!, anchors: default!);
        __instance.children = null;
        __instance.buttonItems = ((global::Doroti.Framework.Widgets.SelectableRegionState)selectableRegionState).contextMenuButtonItems;
        __instance.anchors = ((global::Doroti.Framework.Widgets.SelectableRegionState)selectableRegionState).contextMenuAnchors;
        return __instance;
    }

    public static string getButtonLabel(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.ContextMenuButtonItem buttonItem)
    {
        if ((((global::Doroti.Framework.Widgets.ContextMenuButtonItem)buttonItem).label is not null))
        {
            return ((global::Doroti.Framework.Widgets.ContextMenuButtonItem)buttonItem).label!;
        }
        switch (Theme.of(context).platform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                {
                    return ((string)(object?)CupertinoTextSelectionToolbarButton.getButtonLabel(context, buttonItem));
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
                    MaterialLocalizations localizations__9033 = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
                    return (((global::Doroti.Framework.Widgets.ContextMenuButtonItem)buttonItem).type switch { global::Doroti.Framework.Widgets.ContextMenuButtonType.cut => ((MaterialLocalizations)localizations__9033).cutButtonLabel, global::Doroti.Framework.Widgets.ContextMenuButtonType.copy => ((MaterialLocalizations)localizations__9033).copyButtonLabel, global::Doroti.Framework.Widgets.ContextMenuButtonType.paste => ((MaterialLocalizations)localizations__9033).pasteButtonLabel, global::Doroti.Framework.Widgets.ContextMenuButtonType.selectAll => ((MaterialLocalizations)localizations__9033).selectAllButtonLabel, global::Doroti.Framework.Widgets.ContextMenuButtonType.delete => ((MaterialLocalizations)localizations__9033).deleteButtonTooltip.toUpperCase(), global::Doroti.Framework.Widgets.ContextMenuButtonType.lookUp => ((MaterialLocalizations)localizations__9033).lookUpButtonLabel, global::Doroti.Framework.Widgets.ContextMenuButtonType.searchWeb => ((MaterialLocalizations)localizations__9033).searchWebButtonLabel, global::Doroti.Framework.Widgets.ContextMenuButtonType.share => ((MaterialLocalizations)localizations__9033).shareButtonLabel, global::Doroti.Framework.Widgets.ContextMenuButtonType.liveTextInput => ((MaterialLocalizations)localizations__9033).scanTextButtonLabel, global::Doroti.Framework.Widgets.ContextMenuButtonType.custom => "", _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
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
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                {
                    return buttonItems.map<global::Doroti.Framework.Widgets.ContextMenuButtonItem, global::Doroti.Framework.Widgets.Widget>(((buttonItem) => {
return CupertinoTextSelectionToolbarButton.CreateButtonItem(buttonItem: buttonItem);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
                {
                    var buttons__11165 = new List<global::Doroti.Framework.Widgets.Widget>();
                    for (var i__11204 = 0L; (i__11204 < buttonItems.Count); i__11204++)
                    {
                        global::Doroti.Framework.Widgets.ContextMenuButtonItem buttonItem__11280 = buttonItems[(int)(i__11204)];
                        buttons__11165.Add(new TextSelectionToolbarTextButton(padding: TextSelectionToolbarTextButton.getPadding(i__11204, buttonItems.Count), onPressed: ((global::Doroti.Framework.Widgets.ContextMenuButtonItem)buttonItem__11280).onPressed, alignment: global::Doroti.Framework.Painting.AlignmentDirectional.centerStart, child: new global::Doroti.Framework.Widgets.Text(AdaptiveTextSelectionToolbar.getButtonLabel(context, buttonItem__11280))));
                    }
                    return ((IEnumerable<global::Doroti.Framework.Widgets.Widget>)(object?)buttons__11165);
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    return buttonItems.map<global::Doroti.Framework.Widgets.ContextMenuButtonItem, global::Doroti.Framework.Widgets.Widget>(((buttonItem) => {
return DesktopTextSelectionToolbarButton.CreateText(context: context, onPressed: () => ((global::Doroti.Framework.Widgets.ContextMenuButtonItem)buttonItem).onPressed(), text: AdaptiveTextSelectionToolbar.getButtonLabel(context, buttonItem));
throw new InvalidOperationException("Dart closure completed without a value.");
}));
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                {
                    return buttonItems.map<global::Doroti.Framework.Widgets.ContextMenuButtonItem, global::Doroti.Framework.Widgets.Widget>(((buttonItem) => {
return CupertinoDesktopTextSelectionToolbarButton.CreateText(onPressed: ((global::Doroti.Framework.Widgets.ContextMenuButtonItem)buttonItem).onPressed, text: AdaptiveTextSelectionToolbar.getButtonLabel(context, buttonItem));
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
        if ((this.children is null || !this.children.Any()) && (this.buttonItems is null || !this.buttonItems.Any()))
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.SizedBox.CreateShrink());
        }
        List<global::Doroti.Framework.Widgets.Widget> resultChildren__12590 = ((this.children is not null) ? this.children! : AdaptiveTextSelectionToolbar.getAdaptiveButtons(context, this.buttonItems!).ToList()).ToList();
        switch (Theme.of(context).platform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                {
                    return ((global::Doroti.Framework.Widgets.Widget)(object?)new CupertinoTextSelectionToolbar(anchorAbove: ((global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors)this.anchors).primaryAnchor, anchorBelow: ((((global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors)this.anchors).secondaryAnchor is null) ? ((global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors)this.anchors).primaryAnchor : DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors)this.anchors).secondaryAnchor)), children: resultChildren__12590));
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
                {
                    return ((global::Doroti.Framework.Widgets.Widget)(object?)new TextSelectionToolbar(anchorAbove: ((global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors)this.anchors).primaryAnchor, anchorBelow: ((((global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors)this.anchors).secondaryAnchor is null) ? ((global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors)this.anchors).primaryAnchor : DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors)this.anchors).secondaryAnchor)), children: resultChildren__12590));
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    return ((global::Doroti.Framework.Widgets.Widget)(object?)new DesktopTextSelectionToolbar(anchor: ((global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors)this.anchors).primaryAnchor, children: resultChildren__12590));
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                {
                    return ((global::Doroti.Framework.Widgets.Widget)(object?)new CupertinoDesktopTextSelectionToolbar(anchor: ((global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors)this.anchors).primaryAnchor, children: resultChildren__12590));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
