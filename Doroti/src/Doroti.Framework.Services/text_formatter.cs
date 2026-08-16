#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/text_formatter.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Services;

public enum MaxLengthEnforcement
{
    none,
    enforced,
    truncateAfterCompositionEnds
}

public interface TextInputFormatter
{
    public static TextInputFormatter CreateWithFunction(Func<TextEditingValue, TextEditingValue, TextEditingValue> formatFunction)
        => new _SimpleTextInputFormatter(formatFunction);

    public TextEditingValue formatEditUpdate(TextEditingValue oldValue, TextEditingValue newValue);
}

public delegate TextEditingValue TextInputFormatFunction(TextEditingValue oldValue, TextEditingValue newValue);

internal class _SimpleTextInputFormatter : TextInputFormatter
{
    public virtual Func<TextEditingValue, TextEditingValue, TextEditingValue> formatFunction { get; private set; } = default!;

    internal _SimpleTextInputFormatter(Func<TextEditingValue, TextEditingValue, TextEditingValue> formatFunction)
    {
        this.formatFunction = formatFunction;
    }

    public virtual TextEditingValue formatEditUpdate(TextEditingValue oldValue, TextEditingValue newValue)
    {
        return formatFunction(oldValue, newValue);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MutableTextRange
{
    public virtual long @base { get; set; } = default!;
    public virtual long extent { get; set; } = default!;

    internal _MutableTextRange(long @base, long extent)
    {
        this.@base = @base;
        this.extent = extent;
    }

    public static _MutableTextRange? fromComposingRange(TextRange range)
    {
        return ((range.isValid && !range.isCollapsed) ? new _MutableTextRange(range.start, range.end) : null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static _MutableTextRange? fromTextSelection(TextSelection selection)
    {
        return (selection.isValid ? new _MutableTextRange(selection.baseOffset, selection.extentOffset) : null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TextEditingValueAccumulator
{
    public virtual TextEditingValue inputValue { get; private set; } = default!;
    public virtual StringBuffer stringBuffer { get; private set; } = new StringBuffer();
    public virtual _MutableTextRange? selection { get; private set; }
    public virtual _MutableTextRange? composingRegion { get; private set; }
    public virtual bool debugFinalized { get; set; } = false;

    internal _TextEditingValueAccumulator(TextEditingValue inputValue)
    {
        this.inputValue = inputValue;
        this.selection = _MutableTextRange.fromTextSelection(inputValue.selection);
        this.composingRegion = _MutableTextRange.fromComposingRange(inputValue.composing);
    }

    public virtual TextEditingValue finalize()
    {
        debugFinalized = true;
        _MutableTextRange? selection = this.selection;
        _MutableTextRange? composingRegion = this.composingRegion;
        return new TextEditingValue(text: stringBuffer.ToString(), composing: (((composingRegion is null) || (composingRegion.@base == composingRegion.extent)) ? TextRange.empty : new global::Doroti.Ui.TextRange(start: composingRegion.@base, end: composingRegion.extent)), selection: ((selection is null) ? TextSelection.CreateCollapsed(offset: -1L) : new TextSelection(baseOffset: selection.@base, extentOffset: selection.extent, affinity: inputValue.selection.affinity, isDirectional: inputValue.selection.isDirectional)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class FilteringTextInputFormatter : TextInputFormatter
{
    public virtual Pattern filterPattern { get; private set; } = default!;
    public virtual bool allow { get; private set; } = default!;
    public virtual string replacementString { get; private set; } = default!;
    public static TextInputFormatter singleLineFormatter = FilteringTextInputFormatter.CreateDeny("\n");
    public static TextInputFormatter digitsOnly = FilteringTextInputFormatter.CreateAllow(new RegExp("[0-9]"));

    public FilteringTextInputFormatter(Pattern filterPattern, bool allow, string replacementString = "")
    {
        this.filterPattern = filterPattern;
        this.allow = allow;
        this.replacementString = replacementString;
    }

    public static FilteringTextInputFormatter CreateAllow(Pattern filterPattern, string replacementString = "")
    {
        return new FilteringTextInputFormatter(filterPattern, true, replacementString);
    }

    public static FilteringTextInputFormatter CreateDeny(Pattern filterPattern, string replacementString = "")
    {
        return new FilteringTextInputFormatter(filterPattern, false, replacementString);
    }

    public virtual TextEditingValue formatEditUpdate(TextEditingValue oldValue, TextEditingValue newValue)
    {
        var formatState = new _TextEditingValueAccumulator(newValue);
        DartRuntimePrimitives.Assert(() => !formatState.debugFinalized);
        IEnumerable<Match> matches = filterPattern.allMatches(newValue.text);
        Match? previousMatch = default!;
        foreach (var match in matches)
        {
            DartRuntimePrimitives.Assert(() => (match.end >= match.start));
            _processRegion(allow, (previousMatch?.end ?? 0L), match.start, formatState);
            DartRuntimePrimitives.Assert(() => !formatState.debugFinalized);
            _processRegion(!allow, match.start, match.end, formatState);
            DartRuntimePrimitives.Assert(() => !formatState.debugFinalized);
            previousMatch = match;
        }
        _processRegion(allow, (previousMatch?.end ?? 0L), newValue.text.Length, formatState);
        DartRuntimePrimitives.Assert(() => !formatState.debugFinalized);
        return formatState.finalize();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _processRegion(bool isBannedRegion, long regionStart, long regionEnd, _TextEditingValueAccumulator state)
    {
        string replacementString = (isBannedRegion ? (((regionStart == regionEnd) ? "" : this.replacementString)) : state.inputValue.text.substring(regionStart, regionEnd));
        state.stringBuffer.write(replacementString);
        if ((replacementString.Length == (regionEnd - regionStart)))
        {
            return;
        }
        long adjustIndex(long originalIndex)
        {
            long replacedLength = (((originalIndex <= regionStart) && (originalIndex < regionEnd)) ? 0L : replacementString.Length);
            long removedLength = (originalIndex.clamp(regionStart, regionEnd) - regionStart);
            return (replacedLength - removedLength);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        state.selection?.@base += adjustIndex(state.inputValue.selection.baseOffset);
        state.selection?.extent += adjustIndex(state.inputValue.selection.extentOffset);
        state.composingRegion?.@base += adjustIndex(state.inputValue.composing.start);
        state.composingRegion?.extent += adjustIndex(state.inputValue.composing.end);
    }

}

public class LengthLimitingTextInputFormatter : TextInputFormatter
{
    public virtual long? maxLength { get; private set; }
    public virtual MaxLengthEnforcement? maxLengthEnforcement { get; private set; }

    public LengthLimitingTextInputFormatter(long? maxLength, MaxLengthEnforcement? maxLengthEnforcement = null)
    {
        this.maxLength = maxLength;
        this.maxLengthEnforcement = maxLengthEnforcement;
        System.Diagnostics.Debug.Assert((((maxLength is null) || (DartRuntimePrimitives.RequireValue(maxLength) == -1L)) || (DartRuntimePrimitives.RequireValue(maxLength) > 0L)));
    }

    public static MaxLengthEnforcement getDefaultMaxLengthEnforcement(TargetPlatform? platform = null)
    {
        if (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb)
        {
            return MaxLengthEnforcement.truncateAfterCompositionEnds;
        }
        else
        {
            switch ((platform ?? global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform))
            {
                case var __case22082 when object.Equals(__case22082, TargetPlatform.android):
                case var __case22119 when object.Equals(__case22119, TargetPlatform.windows):
                    {
                        return MaxLengthEnforcement.enforced;
                    }
                case var __case22204 when object.Equals(__case22204, TargetPlatform.iOS):
                case var __case22237 when object.Equals(__case22237, TargetPlatform.macOS):
                case var __case22272 when object.Equals(__case22272, TargetPlatform.linux):
                case var __case22307 when object.Equals(__case22307, TargetPlatform.fuchsia):
                    {
                        return MaxLengthEnforcement.truncateAfterCompositionEnds;
                    }
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static TextEditingValue truncate(TextEditingValue value, long maxLength)
    {
        var iterator = new CharacterRange(value.text);
        if ((value.text.characters().Count > maxLength))
        {
            iterator.expandNext(maxLength);
        }
        string truncated = iterator.Current;
        return new TextEditingValue(text: truncated, selection: value.selection.copyWith(baseOffset: Math.Min(value.selection.start, truncated.Length), extentOffset: Math.Min(value.selection.end, truncated.Length)), composing: ((!value.composing.isCollapsed && (truncated.Length > value.composing.start)) ? new global::Doroti.Ui.TextRange(start: value.composing.start, end: Math.Min(value.composing.end, truncated.Length)) : TextRange.empty));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TextEditingValue formatEditUpdate(TextEditingValue oldValue, TextEditingValue newValue)
    {
        long? maxLength = this.maxLength;
        if ((((maxLength is null) || (DartRuntimePrimitives.RequireValue(maxLength) == -1L)) || (newValue.text.characters().Count <= DartRuntimePrimitives.RequireValue(maxLength))))
        {
            return newValue;
        }
        DartRuntimePrimitives.Assert(() => (DartRuntimePrimitives.RequireValue(maxLength) > 0L));
        switch ((maxLengthEnforcement ?? getDefaultMaxLengthEnforcement()))
        {
            case var __case23944 when object.Equals(__case23944, MaxLengthEnforcement.none):
                {
                    return newValue;
                }
            case var __case24007 when object.Equals(__case24007, MaxLengthEnforcement.enforced):
                {
                    if (((oldValue.text.characters().Count == DartRuntimePrimitives.RequireValue(maxLength)) && oldValue.selection.isCollapsed))
                    {
                        return oldValue;
                    }
                    return truncate(newValue, DartRuntimePrimitives.RequireValue(maxLength));
                }
            case var __case24396 when object.Equals(__case24396, MaxLengthEnforcement.truncateAfterCompositionEnds):
                {
                    if (((oldValue.text.characters().Count == DartRuntimePrimitives.RequireValue(maxLength)) && !oldValue.composing.isValid))
                    {
                        return oldValue;
                    }
                    if (newValue.composing.isValid)
                    {
                        return newValue;
                    }
                    return truncate(newValue, DartRuntimePrimitives.RequireValue(maxLength));
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
