#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/text_editing_delta.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Services;

public static partial class Text_editing_deltaLibrary
{
    internal static TextAffinity? _toTextAffinity(string? affinity)
    {
        return (affinity switch { var __case576 when object.Equals(__case576, "TextAffinity.downstream") => TextAffinity.downstream, var __case634 when object.Equals(__case634, "TextAffinity.upstream") => TextAffinity.upstream, _ => null });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Text_editing_deltaLibrary
{
    internal static string _replace(string originalText, string replacementText, TextRange replacementRange)
    {
        DartRuntimePrimitives.Assert(() => replacementRange.isValid);
        return originalText.replaceRange(replacementRange.start, replacementRange.end, replacementText);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Text_editing_deltaLibrary
{
    internal static bool _debugTextRangeIsValid(TextRange range, string text)
    {
        if (!range.isValid)
        {
            return true;
        }
        return ((((range.start >= 0L) && (range.start <= text.Length))) && (((range.end >= 0L) && (range.end <= text.Length))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public abstract class TextEditingDelta : Diagnosticable
{
    public virtual string oldText { get; private set; } = default!;
    public virtual TextSelection selection { get; private set; } = default!;
    public virtual TextRange composing { get; private set; } = default!;

    protected TextEditingDelta(string oldText, TextSelection selection, TextRange composing)
    {
        this.oldText = oldText;
        this.selection = selection;
        this.composing = composing;
    }

    public static TextEditingDelta CreateFromJSON(DartMap<string, object> encoded)
    {
        var oldText = ((string?)encoded.GetValueOrDefault("oldText"))!;
        var replacementDestinationStart = ((long)encoded.GetValueOrDefault("deltaStart"));
        var replacementDestinationEnd = ((long)encoded.GetValueOrDefault("deltaEnd"));
        var replacementSource = ((string?)encoded.GetValueOrDefault("deltaText"))!;
        var replacementSourceStart = 0L;
        long replacementSourceEnd = replacementSource.Length;
        bool isNonTextUpdate = ((replacementDestinationStart == -1L) && (replacementDestinationStart == replacementDestinationEnd));
        var newComposing = new global::Doroti.Ui.TextRange(start: (((long?)encoded.GetValueOrDefault("composingBase")) ?? -1L), end: (((long?)encoded.GetValueOrDefault("composingExtent")) ?? -1L));
        var newSelection = new TextSelection(baseOffset: (((long?)encoded.GetValueOrDefault("selectionBase")) ?? -1L), extentOffset: (((long?)encoded.GetValueOrDefault("selectionExtent")) ?? -1L), affinity: (Text_editing_deltaLibrary._toTextAffinity(((string?)encoded.GetValueOrDefault("selectionAffinity"))!) ?? TextAffinity.downstream), isDirectional: (((bool?)encoded.GetValueOrDefault("selectionIsDirectional")) ?? false));
        if (isNonTextUpdate)
        {
            DartRuntimePrimitives.Assert(() => Text_editing_deltaLibrary._debugTextRangeIsValid(newSelection, oldText));
            DartRuntimePrimitives.Assert(() => Text_editing_deltaLibrary._debugTextRangeIsValid(newComposing, oldText));
            return new TextEditingDeltaNonTextUpdate(oldText: oldText, selection: newSelection, composing: newComposing);
        }
        DartRuntimePrimitives.Assert(() => Text_editing_deltaLibrary._debugTextRangeIsValid(new global::Doroti.Ui.TextRange(start: replacementDestinationStart, end: replacementDestinationEnd), oldText));
        string newText = Text_editing_deltaLibrary._replace(oldText, replacementSource, new global::Doroti.Ui.TextRange(start: replacementDestinationStart, end: replacementDestinationEnd));
        DartRuntimePrimitives.Assert(() => Text_editing_deltaLibrary._debugTextRangeIsValid(newSelection, newText));
        DartRuntimePrimitives.Assert(() => Text_editing_deltaLibrary._debugTextRangeIsValid(newComposing, newText));
        var isEqual = (oldText == newText);
        bool isDeletionGreaterThanOne = ((((replacementDestinationEnd - replacementDestinationStart)) - ((replacementSourceEnd - replacementSourceStart))) > 1L);
        bool isDeletingByReplacingWithEmpty = (((replacementSource.Length == 0) && (replacementSourceStart == 0L)) && (replacementSourceStart == replacementSourceEnd));
        bool isReplacedByShorter = (isDeletionGreaterThanOne && (((replacementSourceEnd - replacementSourceStart) < (replacementDestinationEnd - replacementDestinationStart))));
        bool isReplacedByLonger = ((replacementSourceEnd - replacementSourceStart) > (replacementDestinationEnd - replacementDestinationStart));
        var isReplacedBySame = ((replacementSourceEnd - replacementSourceStart) == (replacementDestinationEnd - replacementDestinationStart));
        bool isInsertingInsideComposingRegion = ((replacementDestinationStart + replacementSourceEnd) > replacementDestinationEnd);
        bool isDeletingInsideComposingRegion = ((!isReplacedByShorter && !isDeletingByReplacingWithEmpty) && ((replacementDestinationStart + replacementSourceEnd) < replacementDestinationEnd));
        string newComposingText = default!;
        string originalComposingText = default!;
        if (((isDeletingByReplacingWithEmpty || isDeletingInsideComposingRegion) || isReplacedByShorter))
        {
            newComposingText = replacementSource.substring(replacementSourceStart, replacementSourceEnd);
            originalComposingText = oldText.substring(replacementDestinationStart, (replacementDestinationStart + replacementSourceEnd));
        }
        else
        {
            newComposingText = replacementSource.substring(replacementSourceStart, (replacementSourceStart + ((replacementDestinationEnd - replacementDestinationStart))));
            originalComposingText = oldText.substring(replacementDestinationStart, replacementDestinationEnd);
        }
        bool isOriginalComposingRegionTextChanged = !((originalComposingText == newComposingText));
        bool isReplaced = (isOriginalComposingRegionTextChanged || (((isReplacedByLonger || isReplacedByShorter) || isReplacedBySame)));
        if (isEqual)
        {
            return new TextEditingDeltaNonTextUpdate(oldText: oldText, selection: newSelection, composing: newComposing);
        }
        else
        {
            if ((((isDeletingByReplacingWithEmpty || isDeletingInsideComposingRegion)) && !isOriginalComposingRegionTextChanged))
            {
                var actualStart__9997 = replacementDestinationStart;
                if (!isDeletionGreaterThanOne)
                {
                    actualStart__9997 = (replacementDestinationEnd - 1L);
                }
                return new TextEditingDeltaDeletion(oldText: oldText, deletedRange: new global::Doroti.Ui.TextRange(start: actualStart__9997, end: replacementDestinationEnd), selection: newSelection, composing: newComposing);
            }
            else
            {
                if (((((replacementDestinationStart == replacementDestinationEnd) || isInsertingInsideComposingRegion)) && !isOriginalComposingRegionTextChanged))
                {
                    return new TextEditingDeltaInsertion(oldText: oldText, textInserted: replacementSource.substring((replacementDestinationEnd - replacementDestinationStart), (((replacementDestinationEnd - replacementDestinationStart)) + ((replacementSource.Length - ((replacementDestinationEnd - replacementDestinationStart)))))), insertionOffset: replacementDestinationEnd, selection: newSelection, composing: newComposing);
                }
                else
                {
                    if (isReplaced)
                    {
                        return new TextEditingDeltaReplacement(oldText: oldText, replacementText: replacementSource, replacedRange: new global::Doroti.Ui.TextRange(start: replacementDestinationStart, end: replacementDestinationEnd), selection: newSelection, composing: newComposing);
                    }
                }
            }
        }
        DartRuntimePrimitives.Assert(() => false);
        return new TextEditingDeltaNonTextUpdate(oldText: oldText, selection: newSelection, composing: newComposing);
    }

    public abstract TextEditingValue apply(TextEditingValue value);
}

public class TextEditingDeltaInsertion : TextEditingDelta
{
    public virtual string textInserted { get; private set; } = default!;
    public virtual long insertionOffset { get; private set; } = default!;

    public TextEditingDeltaInsertion(string oldText, string textInserted, long insertionOffset, TextSelection selection, TextRange composing) : base(oldText: oldText, selection: selection, composing: composing)
    {
        this.textInserted = textInserted;
        this.insertionOffset = insertionOffset;
    }

    public override TextEditingValue apply(TextEditingValue value)
    {
        string newText = oldText;
        DartRuntimePrimitives.Assert(() => Text_editing_deltaLibrary._debugTextRangeIsValid(new global::Doroti.Ui.TextRange(insertionOffset), newText));
        newText = Text_editing_deltaLibrary._replace(newText, textInserted, new global::Doroti.Ui.TextRange(insertionOffset));
        DartRuntimePrimitives.Assert(() => Text_editing_deltaLibrary._debugTextRangeIsValid(selection, newText));
        DartRuntimePrimitives.Assert(() => Text_editing_deltaLibrary._debugTextRangeIsValid(composing, newText));
        return value.copyWith(text: newText, selection: selection, composing: composing);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.Add(new DiagnosticsProperty<string>("oldText", oldText));
        properties.Add(new DiagnosticsProperty<string>("textInserted", textInserted));
        properties.Add(new DiagnosticsProperty<long>("insertionOffset", insertionOffset));
        properties.Add(new DiagnosticsProperty<TextSelection>("selection", selection));
        properties.Add(new DiagnosticsProperty<global::Doroti.Ui.TextRange>("composing", composing));
    }

}

public class TextEditingDeltaDeletion : TextEditingDelta
{
    public virtual TextRange deletedRange { get; private set; } = default!;

    public TextEditingDeltaDeletion(string oldText, TextRange deletedRange, TextSelection selection, TextRange composing) : base(oldText: oldText, selection: selection, composing: composing)
    {
        this.deletedRange = deletedRange;
    }

    public virtual string textDeleted => oldText.substring(deletedRange.start, deletedRange.end);
    public override TextEditingValue apply(TextEditingValue value)
    {
        string newText = oldText;
        DartRuntimePrimitives.Assert(() => Text_editing_deltaLibrary._debugTextRangeIsValid(deletedRange, newText));
        newText = Text_editing_deltaLibrary._replace(newText, "", deletedRange);
        DartRuntimePrimitives.Assert(() => Text_editing_deltaLibrary._debugTextRangeIsValid(selection, newText));
        DartRuntimePrimitives.Assert(() => Text_editing_deltaLibrary._debugTextRangeIsValid(composing, newText));
        return value.copyWith(text: newText, selection: selection, composing: composing);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.Add(new DiagnosticsProperty<string>("oldText", oldText));
        properties.Add(new DiagnosticsProperty<string>("textDeleted", textDeleted));
        properties.Add(new DiagnosticsProperty<global::Doroti.Ui.TextRange>("deletedRange", deletedRange));
        properties.Add(new DiagnosticsProperty<TextSelection>("selection", selection));
        properties.Add(new DiagnosticsProperty<global::Doroti.Ui.TextRange>("composing", composing));
    }

}

public class TextEditingDeltaReplacement : TextEditingDelta
{
    public virtual string replacementText { get; private set; } = default!;
    public virtual TextRange replacedRange { get; private set; } = default!;

    public TextEditingDeltaReplacement(string oldText, string replacementText, TextRange replacedRange, TextSelection selection, TextRange composing) : base(oldText: oldText, selection: selection, composing: composing)
    {
        this.replacementText = replacementText;
        this.replacedRange = replacedRange;
    }

    public virtual string textReplaced => oldText.substring(replacedRange.start, replacedRange.end);
    public override TextEditingValue apply(TextEditingValue value)
    {
        string newText = oldText;
        DartRuntimePrimitives.Assert(() => Text_editing_deltaLibrary._debugTextRangeIsValid(replacedRange, newText));
        newText = Text_editing_deltaLibrary._replace(newText, replacementText, replacedRange);
        DartRuntimePrimitives.Assert(() => Text_editing_deltaLibrary._debugTextRangeIsValid(selection, newText));
        DartRuntimePrimitives.Assert(() => Text_editing_deltaLibrary._debugTextRangeIsValid(composing, newText));
        return value.copyWith(text: newText, selection: selection, composing: composing);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.Add(new DiagnosticsProperty<string>("oldText", oldText));
        properties.Add(new DiagnosticsProperty<string>("textReplaced", textReplaced));
        properties.Add(new DiagnosticsProperty<string>("replacementText", replacementText));
        properties.Add(new DiagnosticsProperty<global::Doroti.Ui.TextRange>("replacedRange", replacedRange));
        properties.Add(new DiagnosticsProperty<TextSelection>("selection", selection));
        properties.Add(new DiagnosticsProperty<global::Doroti.Ui.TextRange>("composing", composing));
    }

}

public class TextEditingDeltaNonTextUpdate : TextEditingDelta
{
    public TextEditingDeltaNonTextUpdate(string oldText, TextSelection selection, TextRange composing) : base(oldText: oldText, selection: selection, composing: composing)
    {
    }

    public override TextEditingValue apply(TextEditingValue value)
    {
        DartRuntimePrimitives.Assert(() => Text_editing_deltaLibrary._debugTextRangeIsValid(selection, oldText));
        DartRuntimePrimitives.Assert(() => Text_editing_deltaLibrary._debugTextRangeIsValid(composing, oldText));
        return new TextEditingValue(text: oldText, selection: selection, composing: composing);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.Add(new DiagnosticsProperty<string>("oldText", oldText));
        properties.Add(new DiagnosticsProperty<TextSelection>("selection", selection));
        properties.Add(new DiagnosticsProperty<global::Doroti.Ui.TextRange>("composing", composing));
    }

}
