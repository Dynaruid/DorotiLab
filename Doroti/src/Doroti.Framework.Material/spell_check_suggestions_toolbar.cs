// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/spell_check_suggestions_toolbar.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class Spell_check_suggestions_toolbarLibrary
{
    internal static double _kDefaultToolbarHeight = 193.0;
}

public static partial class Spell_check_suggestions_toolbarLibrary
{
    internal static long _kMaxSuggestions = 3L;
}

public class SpellCheckSuggestionsToolbar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual Offset anchor { get; private set; } = default!;
    public virtual List<global::Doroti.Framework.Widgets.ContextMenuButtonItem> buttonItems { get; private set; } = default!;

    public SpellCheckSuggestionsToolbar(global::Doroti.Framework.Foundation.Key? key = null, Offset anchor = default!, List<global::Doroti.Framework.Widgets.ContextMenuButtonItem> buttonItems = default!) : base(key: key)
    {
        this.anchor = anchor;
        this.buttonItems = buttonItems;
        System.Diagnostics.Debug.Assert(checked(buttonItems.Count) <= (Spell_check_suggestions_toolbarLibrary._kMaxSuggestions + 1L));
    }

    public static SpellCheckSuggestionsToolbar CreateEditableText(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.EditableTextState editableTextState = default!)
    {
        var __instance = new SpellCheckSuggestionsToolbar(key: key, anchor: default!, buttonItems: default!);
        __instance.buttonItems = buildButtonItems(editableTextState) ?? new List<global::Doroti.Framework.Widgets.ContextMenuButtonItem>();
        __instance.anchor = getToolbarAnchor(editableTextState.contextMenuAnchors);
        return __instance;
    }

    public static List<global::Doroti.Framework.Widgets.ContextMenuButtonItem>? buildButtonItems(global::Doroti.Framework.Widgets.EditableTextState editableTextState)
    {
        global::Doroti.Framework.Services.SuggestionSpan? spanAtCursorIndex = editableTextState.findSuggestionSpanAtCursorIndex(editableTextState.currentTextEditingValue.selection.baseOffset);
        if (spanAtCursorIndex is null)
        {
            return null;
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
        var deleteButton = new global::Doroti.Framework.Widgets.ContextMenuButtonItem(onPressed: () =>
        {
            if (!editableTextState.mounted)
            {
                return;
            }
            _replaceText(editableTextState, "", editableTextState.currentTextEditingValue.composing);
        }, type: ContextMenuButtonType.delete);
        buttonItems.Add(deleteButton);
        return buttonItems;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static void _replaceText(global::Doroti.Framework.Widgets.EditableTextState editableTextState, string text, TextRange replacementRange)
    {
        DartRuntimePrimitives.Assert(() => !editableTextState.widget.readOnly && !editableTextState.widget.obscureText);
        global::Doroti.Framework.Services.TextEditingValue newValue = editableTextState.textEditingValue.replaced(replacementRange, text);
        editableTextState.userUpdateTextEditingValue(newValue, SelectionChangedCause.toolbar);
        Scheduler.SchedulerBinding.instance.addPostFrameCallback((duration) =>
        {
            if (editableTextState.mounted)
            {
                editableTextState.bringIntoView(editableTextState.textEditingValue.selection.extent);
            }
        }, debugLabel: "SpellCheckerSuggestionsToolbar.bringIntoView");
        editableTextState.hideToolbar();
    }

    public static global::Doroti.Ui.Offset getToolbarAnchor(global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors anchors)
    {
        return (anchors.secondaryAnchor is null) ? anchors.primaryAnchor : DartRuntimePrimitives.RequireValue(anchors.secondaryAnchor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<global::Doroti.Framework.Widgets.Widget> _buildToolbarButtons(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return buttonItems.map<global::Doroti.Framework.Widgets.ContextMenuButtonItem, global::Doroti.Framework.Widgets.Widget>((buttonItem) =>
        {
            var button = new TextSelectionToolbarTextButton(padding: new global::Doroti.Framework.Painting.EdgeInsets(20, 0, 0, 0), onPressed: buttonItem.onPressed, alignment: Alignment.centerLeft, child: new global::Doroti.Framework.Widgets.Text(AdaptiveTextSelectionToolbar.getButtonLabel(context, buttonItem), style: Equals(buttonItem.type, ContextMenuButtonType.delete) ? new global::Doroti.Framework.Painting.TextStyle(color: Colors.blue) : null));
            if (!Equals(buttonItem.type, ContextMenuButtonType.delete))
            {
                return button;
            }
            return new global::Doroti.Framework.Widgets.DecoratedBox(decoration: new global::Doroti.Framework.Painting.BoxDecoration(border: new global::Doroti.Framework.Painting.Border(top: new global::Doroti.Framework.Painting.BorderSide(color: Colors.grey))), child: button);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if (!Enumerable.Any(buttonItems))
        {
            return SizedBox.CreateShrink();
        }
        double spellCheckSuggestionsToolbarHeight = Spell_check_suggestions_toolbarLibrary._kDefaultToolbarHeight - 48.0 * (4L - checked(buttonItems.Count));
        global::Doroti.Framework.Widgets.MediaQueryData mediaQueryData = MediaQuery.of(context);
        double softKeyboardViewInsetsBottom = mediaQueryData.viewInsets.bottom;
        double paddingAbove = mediaQueryData.padding.top + CupertinoTextSelectionToolbar.kToolbarScreenPadding;
        var localAdjustment = new global::Doroti.Ui.Offset(CupertinoTextSelectionToolbar.kToolbarScreenPadding, paddingAbove);
        return new global::Doroti.Framework.Widgets.Padding(padding: new global::Doroti.Framework.Painting.EdgeInsets(CupertinoTextSelectionToolbar.kToolbarScreenPadding, paddingAbove, CupertinoTextSelectionToolbar.kToolbarScreenPadding, CupertinoTextSelectionToolbar.kToolbarScreenPadding + softKeyboardViewInsetsBottom), child: new global::Doroti.Framework.Widgets.CustomSingleChildLayout(@delegate: new SpellCheckSuggestionsToolbarLayoutDelegate(anchor: anchor - localAdjustment), child: new global::Doroti.Framework.Widgets.AnimatedSize(duration: Duration.Create(milliseconds: 140L), child: new _SpellCheckSuggestionsToolbarContainer__spell_check_suggestions_toolbar(height: spellCheckSuggestionsToolbarHeight, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection8142 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection8142.AddRange(_buildToolbarButtons(context)); return __collection8142; }))()))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SpellCheckSuggestionsToolbarContainer__spell_check_suggestions_toolbar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual double height { get; private set; } = default!;
    public virtual List<global::Doroti.Framework.Widgets.Widget> children { get; private set; } = default!;

    internal _SpellCheckSuggestionsToolbarContainer__spell_check_suggestions_toolbar(double height, List<global::Doroti.Framework.Widgets.Widget> children)
    {
        this.height = height;
        this.children = children;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new Material(elevation: 2.0, type: MaterialType.card, child: new global::Doroti.Framework.Widgets.SizedBox(width: 165.0, height: height, child: new global::Doroti.Framework.Widgets.Column(mainAxisSize: MainAxisSize.min, crossAxisAlignment: CrossAxisAlignment.stretch, children: children)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
