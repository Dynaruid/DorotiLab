// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/spell_check_suggestions_toolbar.dart
#pragma warning disable CS8600, CS8602, CS8603, CS8604
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class Spell_check_suggestions_toolbarLibrary
{
    internal static long _kMaxSuggestions = 3L;
}

public class CupertinoSpellCheckSuggestionsToolbar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors anchors { get; private set; } = default!;
    public virtual List<global::Doroti.Framework.Widgets.ContextMenuButtonItem> buttonItems { get; private set; } = default!;

    public CupertinoSpellCheckSuggestionsToolbar(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors anchors = default!, List<global::Doroti.Framework.Widgets.ContextMenuButtonItem> buttonItems = default!) : base(key: key)
    {
        this.anchors = anchors;
        this.buttonItems = buttonItems;
        System.Diagnostics.Debug.Assert((checked((long)(buttonItems.Count)) <= Spell_check_suggestions_toolbarLibrary._kMaxSuggestions));
    }

    public static CupertinoSpellCheckSuggestionsToolbar CreateEditableText(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.EditableTextState editableTextState = default!)
    {
        var __instance = new CupertinoSpellCheckSuggestionsToolbar(key: key, anchors: default!, buttonItems: default!);
        __instance.buttonItems = (CupertinoSpellCheckSuggestionsToolbar.buildButtonItems(editableTextState) ?? new List<global::Doroti.Framework.Widgets.ContextMenuButtonItem>());
        __instance.anchors = ((global::Doroti.Framework.Widgets.EditableTextState)editableTextState).contextMenuAnchors;
        return __instance;
    }

    public static List<global::Doroti.Framework.Widgets.ContextMenuButtonItem>? buildButtonItems(global::Doroti.Framework.Widgets.EditableTextState editableTextState)
    {
        global::Doroti.Framework.Services.SuggestionSpan? spanAtCursorIndex = ((global::Doroti.Framework.Services.SuggestionSpan?)(object?)editableTextState.findSuggestionSpanAtCursorIndex(((global::Doroti.Framework.Widgets.EditableTextState)editableTextState).currentTextEditingValue.selection.baseOffset));
        if ((spanAtCursorIndex is null))
        {
            return null;
        }
        if (!System.Linq.Enumerable.Any(((global::Doroti.Framework.Services.SuggestionSpan)spanAtCursorIndex).suggestions))
        {
            DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasCupertinoLocalizations(editableTextState.context));
            CupertinoLocalizations localizations = ((CupertinoLocalizations)(object?)CupertinoLocalizations.of(editableTextState.context));
            return new List<global::Doroti.Framework.Widgets.ContextMenuButtonItem> { new global::Doroti.Framework.Widgets.ContextMenuButtonItem(onPressed: null, label: ((CupertinoLocalizations)localizations).noSpellCheckReplacementsLabel) };
        }
        var buttonItems = new List<global::Doroti.Framework.Widgets.ContextMenuButtonItem>();
        foreach (string suggestion in ((global::Doroti.Framework.Services.SuggestionSpan)spanAtCursorIndex).suggestions.take(Spell_check_suggestions_toolbarLibrary._kMaxSuggestions))
        {
            buttonItems.Add(new global::Doroti.Framework.Widgets.ContextMenuButtonItem(onPressed: ((global::System.Action)(() =>
            {
                if (!editableTextState.mounted)
                {
                    return;
                }
                CupertinoSpellCheckSuggestionsToolbar._replaceText(editableTextState, suggestion, ((global::Doroti.Framework.Services.SuggestionSpan)spanAtCursorIndex).range);
            })), label: suggestion));
        }
        return buttonItems;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static void _replaceText(global::Doroti.Framework.Widgets.EditableTextState editableTextState, string text, TextRange replacementRange)
    {
        DartRuntimePrimitives.Assert(() => (!editableTextState.widget.readOnly && !editableTextState.widget.obscureText));
        global::Doroti.Framework.Services.TextEditingValue newValue = ((global::Doroti.Framework.Services.TextEditingValue)(object?)((global::Doroti.Framework.Widgets.EditableTextState)editableTextState).textEditingValue.replaced(replacementRange, text).copyWith(selection: global::Doroti.Framework.Services.TextSelection.CreateCollapsed(offset: (replacementRange.start + text.Length))));
        editableTextState.userUpdateTextEditingValue(newValue, global::Doroti.Framework.Services.SelectionChangedCause.toolbar);
        global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((duration) =>
        {
            if (editableTextState.mounted)
            {
                editableTextState.bringIntoView(((global::Doroti.Framework.Widgets.EditableTextState)editableTextState).textEditingValue.selection.extent);
            }
        })), debugLabel: "SpellCheckSuggestions.bringIntoView");
        editableTextState.hideToolbar();
    }

    internal virtual List<global::Doroti.Framework.Widgets.Widget> _buildToolbarButtons(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return this.buttonItems.map<global::Doroti.Framework.Widgets.ContextMenuButtonItem, CupertinoTextSelectionToolbarButton>(((buttonItem) =>
        {
            return CupertinoTextSelectionToolbarButton.CreateButtonItem(buttonItem: buttonItem);
            throw new InvalidOperationException("Dart closure completed without a value.");
        })).Cast<global::Doroti.Framework.Widgets.Widget>().ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if (!System.Linq.Enumerable.Any(this.buttonItems))
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.SizedBox.CreateShrink());
        }
        List<global::Doroti.Framework.Widgets.Widget> childrenLocal = ((List<global::Doroti.Framework.Widgets.Widget>)(object?)_buildToolbarButtons(context));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new CupertinoTextSelectionToolbar(anchorAbove: ((global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors)this.anchors).primaryAnchor, anchorBelow: ((((global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors)this.anchors).secondaryAnchor is null) ? ((global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors)this.anchors).primaryAnchor : DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors)this.anchors).secondaryAnchor)), children: childrenLocal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
