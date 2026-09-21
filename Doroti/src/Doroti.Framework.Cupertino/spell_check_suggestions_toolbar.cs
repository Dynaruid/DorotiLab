// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/spell_check_suggestions_toolbar.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class Spell_check_suggestions_toolbarLibrary
{
    internal static long _kMaxSuggestions = 3L;
}

public class CupertinoSpellCheckSuggestionsToolbar : StatelessWidget
{
    public virtual TextSelectionToolbarAnchors anchors { get; private set; } = default!;
    public virtual List<ContextMenuButtonItem> buttonItems { get; private set; } = default!;

    public CupertinoSpellCheckSuggestionsToolbar(
        Key? key = null,
        TextSelectionToolbarAnchors anchors = default!,
        List<ContextMenuButtonItem> buttonItems = default!
    )
        : base(key: key)
    {
        this.anchors = anchors;
        this.buttonItems = buttonItems;
        System.Diagnostics.Debug.Assert(
            checked(buttonItems.Count) <= Spell_check_suggestions_toolbarLibrary._kMaxSuggestions
        );
    }

    public static CupertinoSpellCheckSuggestionsToolbar CreateEditableText(
        Key? key = null,
        EditableTextState editableTextState = default!
    )
    {
        var __instance = new CupertinoSpellCheckSuggestionsToolbar(
            key: key,
            anchors: default!,
            buttonItems: default!
        );
        __instance.buttonItems =
            buildButtonItems(editableTextState) ?? new List<ContextMenuButtonItem>();
        __instance.anchors = editableTextState.contextMenuAnchors;
        return __instance;
    }

    public static List<ContextMenuButtonItem>? buildButtonItems(EditableTextState editableTextState)
    {
        SuggestionSpan? spanAtCursorIndex = editableTextState.findSuggestionSpanAtCursorIndex(
            editableTextState.currentTextEditingValue.selection.baseOffset
        );
        if (spanAtCursorIndex is null)
        {
            return null;
        }
        if (!Enumerable.Any(spanAtCursorIndex.suggestions))
        {
            DartRuntimePrimitives.Assert(() =>
                DebugLibrary.debugCheckHasCupertinoLocalizations(editableTextState.context)
            );
            CupertinoLocalizations localizations = CupertinoLocalizations.of(
                editableTextState.context
            );
            return new List<ContextMenuButtonItem>
            {
                new ContextMenuButtonItem(
                    onPressed: null,
                    label: localizations.noSpellCheckReplacementsLabel
                ),
            };
        }
        var buttonItems = new List<ContextMenuButtonItem>();
        foreach (
            string suggestion in spanAtCursorIndex.suggestions.take(
                Spell_check_suggestions_toolbarLibrary._kMaxSuggestions
            )
        )
        {
            buttonItems.Add(
                new ContextMenuButtonItem(
                    onPressed: () =>
                    {
                        if (!editableTextState.mounted)
                        {
                            return;
                        }
                        _replaceText(editableTextState, suggestion, spanAtCursorIndex.range);
                    },
                    label: suggestion
                )
            );
        }
        return buttonItems;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static void _replaceText(
        EditableTextState editableTextState,
        string text,
        TextRange replacementRange
    )
    {
        DartRuntimePrimitives.Assert(() =>
            !editableTextState.widget.readOnly && !editableTextState.widget.obscureText
        );
        TextEditingValue newValue = editableTextState
            .textEditingValue.replaced(replacementRange, text)
            .copyWith(
                selection: TextSelection.CreateCollapsed(
                    offset: replacementRange.start + text.Length
                )
            );
        editableTextState.userUpdateTextEditingValue(newValue, SelectionChangedCause.toolbar);
        Scheduler.SchedulerBinding.instance.addPostFrameCallback(
            (duration) =>
            {
                if (editableTextState.mounted)
                {
                    editableTextState.bringIntoView(
                        editableTextState.textEditingValue.selection.extent
                    );
                }
            },
            debugLabel: "SpellCheckSuggestions.bringIntoView"
        );
        editableTextState.hideToolbar();
    }

    internal virtual List<Widget> _buildToolbarButtons(BuildContext context)
    {
        return buttonItems
            .map(
                (buttonItem) =>
                {
                    return CupertinoTextSelectionToolbarButton.CreateButtonItem(
                        buttonItem: buttonItem
                    );
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                }
            )
            .Cast<Widget>()
            .ToList();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget build(BuildContext context)
    {
        if (!Enumerable.Any(buttonItems))
        {
            return SizedBox.CreateShrink();
        }
        List<Widget> childrenLocal = _buildToolbarButtons(context);
        return new CupertinoTextSelectionToolbar(
            anchorAbove: anchors.primaryAnchor,
            anchorBelow: (anchors.secondaryAnchor is null)
                ? anchors.primaryAnchor
                : (
                    anchors.secondaryAnchor
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
            children: childrenLocal
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
