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

public class SpellCheckSuggestionsToolbar : StatelessWidget
{
    public virtual Offset anchor { get; private set; } = default!;
    public virtual List<ContextMenuButtonItem> buttonItems { get; private set; } = default!;

    public SpellCheckSuggestionsToolbar(
        Key? key = null,
        Offset anchor = default!,
        List<ContextMenuButtonItem> buttonItems = default!
    )
        : base(key: key)
    {
        this.anchor = anchor;
        this.buttonItems = buttonItems;
        System.Diagnostics.Debug.Assert(
            checked(buttonItems.Count)
                <= (Spell_check_suggestions_toolbarLibrary._kMaxSuggestions + 1L)
        );
    }

    public static SpellCheckSuggestionsToolbar CreateEditableText(
        Key? key = null,
        EditableTextState editableTextState = default!
    )
    {
        var __instance = new SpellCheckSuggestionsToolbar(
            key: key,
            anchor: default!,
            buttonItems: default!
        );
        __instance.buttonItems =
            buildButtonItems(editableTextState) ?? new List<ContextMenuButtonItem>();
        __instance.anchor = getToolbarAnchor(editableTextState.contextMenuAnchors);
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
        var deleteButton = new ContextMenuButtonItem(
            onPressed: () =>
            {
                if (!editableTextState.mounted)
                {
                    return;
                }
                _replaceText(
                    editableTextState,
                    "",
                    editableTextState.currentTextEditingValue.composing
                );
            },
            type: ContextMenuButtonType.delete
        );
        buttonItems.Add(deleteButton);
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
        TextEditingValue newValue = editableTextState.textEditingValue.replaced(
            replacementRange,
            text
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
            debugLabel: "SpellCheckerSuggestionsToolbar.bringIntoView"
        );
        editableTextState.hideToolbar();
    }

    public static Offset getToolbarAnchor(TextSelectionToolbarAnchors anchors)
    {
        return (anchors.secondaryAnchor is null)
            ? anchors.primaryAnchor
            : (
                anchors.secondaryAnchor
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual List<Widget> _buildToolbarButtons(BuildContext context)
    {
        return buttonItems
            .map<ContextMenuButtonItem, Widget>(
                (buttonItem) =>
                {
                    var button = new TextSelectionToolbarTextButton(
                        padding: new EdgeInsets(20, 0, 0, 0),
                        onPressed: buttonItem.onPressed,
                        alignment: Alignment.centerLeft,
                        child: new Text(
                            AdaptiveTextSelectionToolbar.getButtonLabel(context, buttonItem),
                            style: Equals(buttonItem.type, ContextMenuButtonType.delete)
                                ? new TextStyle(color: Colors.blue)
                                : null
                        )
                    );
                    if (!Equals(buttonItem.type, ContextMenuButtonType.delete))
                    {
                        return button;
                    }
                    return new DecoratedBox(
                        decoration: new BoxDecoration(
                            border: new Border(top: new BorderSide(color: Colors.grey))
                        ),
                        child: button
                    );
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                }
            )
            .ToList();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget build(BuildContext context)
    {
        if (!Enumerable.Any(buttonItems))
        {
            return SizedBox.CreateShrink();
        }
        double spellCheckSuggestionsToolbarHeight =
            Spell_check_suggestions_toolbarLibrary._kDefaultToolbarHeight
            - (48.0 * (4L - checked(buttonItems.Count)));
        MediaQueryData mediaQueryData = MediaQuery.of(context);
        double softKeyboardViewInsetsBottom = mediaQueryData.viewInsets.bottom;
        double paddingAbove =
            mediaQueryData.padding.top + CupertinoTextSelectionToolbar.kToolbarScreenPadding;
        var localAdjustment = new Offset(
            CupertinoTextSelectionToolbar.kToolbarScreenPadding,
            paddingAbove
        );
        return new Padding(
            padding: new EdgeInsets(
                CupertinoTextSelectionToolbar.kToolbarScreenPadding,
                paddingAbove,
                CupertinoTextSelectionToolbar.kToolbarScreenPadding,
                CupertinoTextSelectionToolbar.kToolbarScreenPadding + softKeyboardViewInsetsBottom
            ),
            child: new CustomSingleChildLayout(
                @delegate: new SpellCheckSuggestionsToolbarLayoutDelegate(
                    anchor: anchor - localAdjustment
                ),
                child: new AnimatedSize(
                    duration: Duration.Create(milliseconds: 140L),
                    child: new _SpellCheckSuggestionsToolbarContainer__spell_check_suggestions_toolbar(
                        height: spellCheckSuggestionsToolbarHeight,
                        children: (
                            (Func<List<Widget>>)(
                                () =>
                                {
                                    var __collection8142 = new List<Widget>();
                                    __collection8142.AddRange(_buildToolbarButtons(context));
                                    return __collection8142;
                                }
                            )
                        )()
                    )
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _SpellCheckSuggestionsToolbarContainer__spell_check_suggestions_toolbar
    : StatelessWidget
{
    public virtual double height { get; private set; } = default!;
    public virtual List<Widget> children { get; private set; } = default!;

    internal _SpellCheckSuggestionsToolbarContainer__spell_check_suggestions_toolbar(
        double height,
        List<Widget> children
    )
    {
        this.height = height;
        this.children = children;
    }

    public override Widget build(BuildContext context)
    {
        return new Material(
            elevation: 2.0,
            type: MaterialType.card,
            child: new SizedBox(
                width: 165.0,
                height: height,
                child: new Column(
                    mainAxisSize: MainAxisSize.min,
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: children
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
