// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/text_editing_intents.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public class DoNothingAndStopPropagationTextIntent : Intent
{
    public DoNothingAndStopPropagationTextIntent()
    {
    }

}

public abstract class DirectionalTextEditingIntent : Intent
{
    public virtual bool forward { get; private set; } = default!;

    protected DirectionalTextEditingIntent(bool forward)
    {
        this.forward = forward;
    }

}

public class DeleteCharacterIntent : DirectionalTextEditingIntent
{
    public DeleteCharacterIntent(bool forward) : base(forward)
    {
    }

}

public class DeleteToNextWordBoundaryIntent : DirectionalTextEditingIntent
{
    public DeleteToNextWordBoundaryIntent(bool forward) : base(forward)
    {
    }

}

public class DeleteToLineBreakIntent : DirectionalTextEditingIntent
{
    public DeleteToLineBreakIntent(bool forward) : base(forward)
    {
    }

}

public abstract class DirectionalCaretMovementIntent : DirectionalTextEditingIntent
{
    public virtual bool collapseSelection { get; private set; } = default!;
    public virtual bool collapseAtReversal { get; private set; } = default!;
    public virtual bool continuesAtWrap { get; private set; } = default!;

    protected DirectionalCaretMovementIntent(bool forward, bool collapseSelection, bool collapseAtReversal = false, bool continuesAtWrap = false) : base(forward)
    {
        this.collapseSelection = collapseSelection;
        this.collapseAtReversal = collapseAtReversal;
        this.continuesAtWrap = continuesAtWrap;
        System.Diagnostics.Debug.Assert((!collapseSelection || !collapseAtReversal));
    }

}

public class ExtendSelectionByCharacterIntent : DirectionalCaretMovementIntent
{
    public ExtendSelectionByCharacterIntent(bool forward, bool collapseSelection) : base(forward, collapseSelection)
    {
    }

}

public class ExtendSelectionToNextWordBoundaryIntent : DirectionalCaretMovementIntent
{
    public ExtendSelectionToNextWordBoundaryIntent(bool forward, bool collapseSelection) : base(forward, collapseSelection)
    {
    }

}

public class ExtendSelectionToNextWordBoundaryOrCaretLocationIntent : DirectionalCaretMovementIntent
{
    public ExtendSelectionToNextWordBoundaryOrCaretLocationIntent(bool forward) : base(forward, false, true)
    {
    }

}

public class ExpandSelectionToDocumentBoundaryIntent : DirectionalCaretMovementIntent
{
    public ExpandSelectionToDocumentBoundaryIntent(bool forward) : base(forward, false)
    {
    }

}

public class ExpandSelectionToLineBreakIntent : DirectionalCaretMovementIntent
{
    public ExpandSelectionToLineBreakIntent(bool forward) : base(forward, false)
    {
    }

}

public class ExtendSelectionToLineBreakIntent : DirectionalCaretMovementIntent
{
    public ExtendSelectionToLineBreakIntent(bool forward, bool collapseSelection, bool collapseAtReversal = false, bool continuesAtWrap = false) : base(forward, collapseSelection, collapseAtReversal, continuesAtWrap)
    {
        System.Diagnostics.Debug.Assert((!collapseSelection || !collapseAtReversal));
    }

}

public class ExtendSelectionVerticallyToAdjacentLineIntent : DirectionalCaretMovementIntent
{
    public ExtendSelectionVerticallyToAdjacentLineIntent(bool forward, bool collapseSelection) : base(forward, collapseSelection)
    {
    }

}

public class ExtendSelectionVerticallyToAdjacentPageIntent : DirectionalCaretMovementIntent
{
    public ExtendSelectionVerticallyToAdjacentPageIntent(bool forward, bool collapseSelection) : base(forward, collapseSelection)
    {
    }

}

public class ExtendSelectionToNextParagraphBoundaryIntent : DirectionalCaretMovementIntent
{
    public ExtendSelectionToNextParagraphBoundaryIntent(bool forward, bool collapseSelection) : base(forward, collapseSelection)
    {
    }

}

public class ExtendSelectionToNextParagraphBoundaryOrCaretLocationIntent : DirectionalCaretMovementIntent
{
    public ExtendSelectionToNextParagraphBoundaryOrCaretLocationIntent(bool forward) : base(forward, false, true)
    {
    }

}

public class ExtendSelectionToDocumentBoundaryIntent : DirectionalCaretMovementIntent
{
    public ExtendSelectionToDocumentBoundaryIntent(bool forward, bool collapseSelection) : base(forward, collapseSelection)
    {
    }

}

public class ScrollToDocumentBoundaryIntent : DirectionalTextEditingIntent
{
    public ScrollToDocumentBoundaryIntent(bool forward) : base(forward)
    {
    }

}

public class SelectAllTextIntent : Intent
{
    public virtual global::Doroti.Generated.Framework.Services.SelectionChangedCause cause { get; private set; } = default!;

    public SelectAllTextIntent(global::Doroti.Generated.Framework.Services.SelectionChangedCause cause)
    {
        this.cause = cause;
    }

}

public class CopySelectionTextIntent : Intent
{
    public static CopySelectionTextIntent copy = new CopySelectionTextIntent(global::Doroti.Generated.Framework.Services.SelectionChangedCause.keyboard, false);
    public virtual global::Doroti.Generated.Framework.Services.SelectionChangedCause cause { get; private set; } = default!;
    public virtual bool collapseSelection { get; private set; } = default!;

    public CopySelectionTextIntent(global::Doroti.Generated.Framework.Services.SelectionChangedCause cause, bool collapseSelection)
    {
        this.cause = cause;
        this.collapseSelection = collapseSelection;
    }

    public static CopySelectionTextIntent CreateCut(global::Doroti.Generated.Framework.Services.SelectionChangedCause cause)
    {
        return new CopySelectionTextIntent(cause, true);
    }

}

public class PasteTextIntent : Intent
{
    public virtual global::Doroti.Generated.Framework.Services.SelectionChangedCause cause { get; private set; } = default!;

    public PasteTextIntent(global::Doroti.Generated.Framework.Services.SelectionChangedCause cause)
    {
        this.cause = cause;
    }

}

public class RedoTextIntent : Intent
{
    public virtual global::Doroti.Generated.Framework.Services.SelectionChangedCause cause { get; private set; } = default!;

    public RedoTextIntent(global::Doroti.Generated.Framework.Services.SelectionChangedCause cause)
    {
        this.cause = cause;
    }

}

public class ReplaceTextIntent : Intent
{
    public virtual global::Doroti.Generated.Framework.Services.TextEditingValue currentTextEditingValue { get; private set; } = default!;
    public virtual string replacementText { get; private set; } = default!;
    public virtual TextRange replacementRange { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Services.SelectionChangedCause cause { get; private set; } = default!;

    public ReplaceTextIntent(global::Doroti.Generated.Framework.Services.TextEditingValue currentTextEditingValue, string replacementText, TextRange replacementRange, global::Doroti.Generated.Framework.Services.SelectionChangedCause cause)
    {
        this.currentTextEditingValue = currentTextEditingValue;
        this.replacementText = replacementText;
        this.replacementRange = replacementRange;
        this.cause = cause;
    }

}

public class UndoTextIntent : Intent
{
    public virtual global::Doroti.Generated.Framework.Services.SelectionChangedCause cause { get; private set; } = default!;

    public UndoTextIntent(global::Doroti.Generated.Framework.Services.SelectionChangedCause cause)
    {
        this.cause = cause;
    }

}

public class UpdateSelectionIntent : Intent
{
    public virtual global::Doroti.Generated.Framework.Services.TextEditingValue currentTextEditingValue { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Services.TextSelection newSelection { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Services.SelectionChangedCause cause { get; private set; } = default!;

    public UpdateSelectionIntent(global::Doroti.Generated.Framework.Services.TextEditingValue currentTextEditingValue, global::Doroti.Generated.Framework.Services.TextSelection newSelection, global::Doroti.Generated.Framework.Services.SelectionChangedCause cause)
    {
        this.currentTextEditingValue = currentTextEditingValue;
        this.newSelection = newSelection;
        this.cause = cause;
    }

}

public class TransposeCharactersIntent : Intent
{
    public TransposeCharactersIntent()
    {
    }

}

public class EditableTextTapOutsideIntent : Intent
{
    public virtual FocusNode focusNode { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Gestures.PointerDownEvent pointerDownEvent { get; private set; } = default!;

    public EditableTextTapOutsideIntent(FocusNode focusNode, global::Doroti.Generated.Framework.Gestures.PointerDownEvent pointerDownEvent)
    {
        this.focusNode = focusNode;
        this.pointerDownEvent = pointerDownEvent;
    }

}

public class EditableTextTapUpOutsideIntent : Intent
{
    public virtual FocusNode focusNode { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Gestures.PointerUpEvent pointerUpEvent { get; private set; } = default!;

    public EditableTextTapUpOutsideIntent(FocusNode focusNode, global::Doroti.Generated.Framework.Gestures.PointerUpEvent pointerUpEvent)
    {
        this.focusNode = focusNode;
        this.pointerUpEvent = pointerUpEvent;
    }

}

