// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/spell_check_suggestions_toolbar.dart
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
        System.Diagnostics.Debug.Assert((checked((long)(buttonItems.Count)) <= (Spell_check_suggestions_toolbarLibrary._kMaxSuggestions + 1L)));
    }

    public static SpellCheckSuggestionsToolbar CreateEditableText(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.EditableTextState editableTextState = default!)
    {
        var __instance = new SpellCheckSuggestionsToolbar(key: key, anchor: default!, buttonItems: default!);
        __instance.buttonItems = (SpellCheckSuggestionsToolbar.buildButtonItems(editableTextState) ?? new List<global::Doroti.Framework.Widgets.ContextMenuButtonItem>());
        __instance.anchor = SpellCheckSuggestionsToolbar.getToolbarAnchor(((global::Doroti.Framework.Widgets.EditableTextState)editableTextState).contextMenuAnchors);
        return __instance;
    }

    public static List<global::Doroti.Framework.Widgets.ContextMenuButtonItem>? buildButtonItems(global::Doroti.Framework.Widgets.EditableTextState editableTextState)
    {
        global::Doroti.Framework.Services.SuggestionSpan? spanAtCursorIndex = ((global::Doroti.Framework.Services.SuggestionSpan?)(object?)editableTextState.findSuggestionSpanAtCursorIndex(((global::Doroti.Framework.Widgets.EditableTextState)editableTextState).currentTextEditingValue.selection.baseOffset));
        if ((spanAtCursorIndex is null))
        {
            return null;
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
                SpellCheckSuggestionsToolbar._replaceText(editableTextState, suggestion, ((global::Doroti.Framework.Services.SuggestionSpan)spanAtCursorIndex).range);
            })), label: suggestion));
        }
        var deleteButton = new global::Doroti.Framework.Widgets.ContextMenuButtonItem(onPressed: ((global::System.Action)(() =>
        {
            if (!editableTextState.mounted)
            {
                return;
            }
            SpellCheckSuggestionsToolbar._replaceText(editableTextState, "", ((global::Doroti.Framework.Widgets.EditableTextState)editableTextState).currentTextEditingValue.composing);
        })), type: global::Doroti.Framework.Widgets.ContextMenuButtonType.delete);
        buttonItems.Add(deleteButton);
        return buttonItems;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static void _replaceText(global::Doroti.Framework.Widgets.EditableTextState editableTextState, string text, TextRange replacementRange)
    {
        DartRuntimePrimitives.Assert(() => (!editableTextState.widget.readOnly && !editableTextState.widget.obscureText));
        global::Doroti.Framework.Services.TextEditingValue newValue = ((global::Doroti.Framework.Services.TextEditingValue)(object?)((global::Doroti.Framework.Widgets.EditableTextState)editableTextState).textEditingValue.replaced(replacementRange, text));
        editableTextState.userUpdateTextEditingValue(newValue, global::Doroti.Framework.Services.SelectionChangedCause.toolbar);
        global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((duration) =>
        {
            if (editableTextState.mounted)
            {
                editableTextState.bringIntoView(((global::Doroti.Framework.Widgets.EditableTextState)editableTextState).textEditingValue.selection.extent);
            }
        })), debugLabel: "SpellCheckerSuggestionsToolbar.bringIntoView");
        editableTextState.hideToolbar();
    }

    public static global::Doroti.Ui.Offset getToolbarAnchor(global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors anchors)
    {
        return ((((global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors)anchors).secondaryAnchor is null) ? ((global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors)anchors).primaryAnchor : DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Widgets.TextSelectionToolbarAnchors)anchors).secondaryAnchor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<global::Doroti.Framework.Widgets.Widget> _buildToolbarButtons(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return this.buttonItems.map<global::Doroti.Framework.Widgets.ContextMenuButtonItem, global::Doroti.Framework.Widgets.Widget>(((buttonItem) =>
        {
            var button = new TextSelectionToolbarTextButton(padding: new global::Doroti.Framework.Painting.EdgeInsets(20, 0, 0, 0), onPressed: () => ((global::Doroti.Framework.Widgets.ContextMenuButtonItem)buttonItem).onPressed(), alignment: global::Doroti.Framework.Painting.Alignment.centerLeft, child: new global::Doroti.Framework.Widgets.Text(AdaptiveTextSelectionToolbar.getButtonLabel(context, buttonItem), style: ((object.Equals(((global::Doroti.Framework.Widgets.ContextMenuButtonItem)buttonItem).type, global::Doroti.Framework.Widgets.ContextMenuButtonType.delete)) ? new global::Doroti.Framework.Painting.TextStyle(color: Colors.blue) : null)));
            if ((!object.Equals(((global::Doroti.Framework.Widgets.ContextMenuButtonItem)buttonItem).type, global::Doroti.Framework.Widgets.ContextMenuButtonType.delete)))
            {
                return ((global::Doroti.Framework.Widgets.Widget)(object?)button);
            }
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.DecoratedBox(decoration: new global::Doroti.Framework.Painting.BoxDecoration(border: new global::Doroti.Framework.Painting.Border(top: new global::Doroti.Framework.Painting.BorderSide(color: Colors.grey))), child: button));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if (!System.Linq.Enumerable.Any(this.buttonItems))
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.SizedBox.CreateShrink());
        }
        double spellCheckSuggestionsToolbarHeight = (Spell_check_suggestions_toolbarLibrary._kDefaultToolbarHeight - ((48.0 * ((4L - checked((long)(this.buttonItems.Count)))))));
        global::Doroti.Framework.Widgets.MediaQueryData mediaQueryData = ((global::Doroti.Framework.Widgets.MediaQueryData)(object?)MediaQuery.of(context));
        double softKeyboardViewInsetsBottom = ((global::Doroti.Framework.Widgets.MediaQueryData)mediaQueryData).viewInsets.bottom;
        double paddingAbove = (((global::Doroti.Framework.Widgets.MediaQueryData)mediaQueryData).padding.top + CupertinoTextSelectionToolbar.kToolbarScreenPadding);
        var localAdjustment = new global::Doroti.Ui.Offset(CupertinoTextSelectionToolbar.kToolbarScreenPadding, paddingAbove);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Padding(padding: new global::Doroti.Framework.Painting.EdgeInsets(CupertinoTextSelectionToolbar.kToolbarScreenPadding, paddingAbove, CupertinoTextSelectionToolbar.kToolbarScreenPadding, (CupertinoTextSelectionToolbar.kToolbarScreenPadding + softKeyboardViewInsetsBottom)), child: new global::Doroti.Framework.Widgets.CustomSingleChildLayout(@delegate: new SpellCheckSuggestionsToolbarLayoutDelegate(anchor: (this.anchor - localAdjustment)), child: new global::Doroti.Framework.Widgets.AnimatedSize(duration: Duration.Create(milliseconds: 140L), child: new _SpellCheckSuggestionsToolbarContainer__spell_check_suggestions_toolbar(height: spellCheckSuggestionsToolbarHeight, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection8142 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection8142.AddRange(_buildToolbarButtons(context)); return __collection8142; }))())))));
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
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new Material(elevation: 2.0, type: MaterialType.card, child: new global::Doroti.Framework.Widgets.SizedBox(width: 165.0, height: this.height, child: new global::Doroti.Framework.Widgets.Column(mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.stretch, children: this.children))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
