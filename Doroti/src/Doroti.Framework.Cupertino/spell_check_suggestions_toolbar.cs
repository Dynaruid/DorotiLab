// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/spell_check_suggestions_toolbar.dart

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
        System.Diagnostics.Debug.Assert(checked(buttonItems.Count) <= Spell_check_suggestions_toolbarLibrary._kMaxSuggestions);
    }

    public static CupertinoSpellCheckSuggestionsToolbar CreateEditableText(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.EditableTextState editableTextState = default!)
    {
        var __instance = new CupertinoSpellCheckSuggestionsToolbar(key: key, anchors: default!, buttonItems: default!);
        __instance.buttonItems = buildButtonItems(editableTextState) ?? new List<global::Doroti.Framework.Widgets.ContextMenuButtonItem>();
        __instance.anchors = editableTextState.contextMenuAnchors;
        return __instance;
    }

    public static List<global::Doroti.Framework.Widgets.ContextMenuButtonItem>? buildButtonItems(global::Doroti.Framework.Widgets.EditableTextState editableTextState)
    {
        global::Doroti.Framework.Services.SuggestionSpan? spanAtCursorIndex = editableTextState.findSuggestionSpanAtCursorIndex(editableTextState.currentTextEditingValue.selection.baseOffset);
        if (spanAtCursorIndex is null)
        {
            return null;
        }
        if (!Enumerable.Any(spanAtCursorIndex.suggestions))
        {
            DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasCupertinoLocalizations(editableTextState.context));
            CupertinoLocalizations localizations = CupertinoLocalizations.of(editableTextState.context);
            return new List<global::Doroti.Framework.Widgets.ContextMenuButtonItem> { new global::Doroti.Framework.Widgets.ContextMenuButtonItem(onPressed: null, label: localizations.noSpellCheckReplacementsLabel) };
        }
        var buttonItems = new List<global::Doroti.Framework.Widgets.ContextMenuButtonItem>();
        foreach (string suggestion in spanAtCursorIndex.suggestions.take(Spell_check_suggestions_toolbarLibrary._kMaxSuggestions))
        {
            buttonItems.Add(new global::Doroti.Framework.Widgets.ContextMenuButtonItem(onPressed: () =>
            {
                if (!editableTextState.mounted)
                {
                    return;
                }
                _replaceText(editableTextState, suggestion, spanAtCursorIndex.range);
            }, label: suggestion));
        }
        return buttonItems;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static void _replaceText(global::Doroti.Framework.Widgets.EditableTextState editableTextState, string text, TextRange replacementRange)
    {
        DartRuntimePrimitives.Assert(() => !editableTextState.widget.readOnly && !editableTextState.widget.obscureText);
        global::Doroti.Framework.Services.TextEditingValue newValue = editableTextState.textEditingValue.replaced(replacementRange, text).copyWith(selection: TextSelection.CreateCollapsed(offset: replacementRange.start + text.Length));
        editableTextState.userUpdateTextEditingValue(newValue, SelectionChangedCause.toolbar);
        Scheduler.SchedulerBinding.instance.addPostFrameCallback((duration) =>
        {
            if (editableTextState.mounted)
            {
                editableTextState.bringIntoView(editableTextState.textEditingValue.selection.extent);
            }
        }, debugLabel: "SpellCheckSuggestions.bringIntoView");
        editableTextState.hideToolbar();
    }

    internal virtual List<global::Doroti.Framework.Widgets.Widget> _buildToolbarButtons(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return buttonItems.map<global::Doroti.Framework.Widgets.ContextMenuButtonItem, CupertinoTextSelectionToolbarButton>((buttonItem) =>
        {
            return CupertinoTextSelectionToolbarButton.CreateButtonItem(buttonItem: buttonItem);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }).Cast<global::Doroti.Framework.Widgets.Widget>().ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if (!Enumerable.Any(buttonItems))
        {
            return SizedBox.CreateShrink();
        }
        List<global::Doroti.Framework.Widgets.Widget> childrenLocal = _buildToolbarButtons(context);
        return new CupertinoTextSelectionToolbar(anchorAbove: anchors.primaryAnchor, anchorBelow: (anchors.secondaryAnchor is null) ? anchors.primaryAnchor : DartRuntimePrimitives.RequireValue(anchors.secondaryAnchor), children: childrenLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
