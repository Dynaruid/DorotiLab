// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/text_formatter.dart
using Doroti.Runtime;
using Doroti.Ui;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Services;

public enum MaxLengthEnforcement
{
    none,
    enforced,
    truncateAfterCompositionEnds,
}

public interface TextInputFormatter
{
    public static TextInputFormatter CreateWithFunction(
        Func<TextEditingValue, TextEditingValue, TextEditingValue> formatFunction
    ) => new _SimpleTextInputFormatter(formatFunction);

    public TextEditingValue formatEditUpdate(TextEditingValue oldValue, TextEditingValue newValue);
}

public delegate TextEditingValue TextInputFormatFunction(
    TextEditingValue oldValue,
    TextEditingValue newValue
);

internal class _SimpleTextInputFormatter : TextInputFormatter
{
    public virtual Func<TextEditingValue, TextEditingValue, TextEditingValue> formatFunction
    {
        get;
        private set;
    } = default!;

    internal _SimpleTextInputFormatter(
        Func<TextEditingValue, TextEditingValue, TextEditingValue> formatFunction
    )
    {
        this.formatFunction = formatFunction;
    }

    public virtual TextEditingValue formatEditUpdate(
        TextEditingValue oldValue,
        TextEditingValue newValue
    )
    {
        return formatFunction(oldValue, newValue);
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
        return (range.isValid && !range.isCollapsed)
            ? new _MutableTextRange(range.start, range.end)
            : null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static _MutableTextRange? fromTextSelection(TextSelection selection)
    {
        return selection.isValid
            ? new _MutableTextRange(selection.baseOffset, selection.extentOffset)
            : null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _TextEditingValueAccumulator
{
    public virtual TextEditingValue inputValue { get; private set; } = default!;
    public virtual System.Text.StringBuilder stringBuffer { get; private set; } = new System.Text.StringBuilder();
    public virtual _MutableTextRange? selection { get; private set; }
    public virtual _MutableTextRange? composingRegion { get; private set; }
    public virtual bool debugFinalized { get; set; } = false;

    internal _TextEditingValueAccumulator(TextEditingValue inputValue)
    {
        this.inputValue = inputValue;
        selection = _MutableTextRange.fromTextSelection(inputValue.selection);
        composingRegion = _MutableTextRange.fromComposingRange(inputValue.composing);
    }

    public virtual TextEditingValue finalize()
    {
        debugFinalized = true;
        _MutableTextRange? selection = this.selection;
        _MutableTextRange? composingRegion = this.composingRegion;
        return new TextEditingValue(
            text: stringBuffer.ToString(),
            composing: (
                (composingRegion is null) || (composingRegion.@base == composingRegion.extent)
            )
                ? TextRange.empty
                : new TextRange(start: composingRegion.@base, end: composingRegion.extent),
            selection: (selection is null)
                ? TextSelection.CreateCollapsed(offset: -1L)
                : new TextSelection(
                    baseOffset: selection.@base,
                    extentOffset: selection.extent,
                    affinity: inputValue.selection.affinity,
                    isDirectional: inputValue.selection.isDirectional
                )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class FilteringTextInputFormatter : TextInputFormatter
{
    public virtual Pattern filterPattern { get; private set; } = default!;
    public virtual bool allow { get; private set; } = default!;
    public virtual string replacementString { get; private set; } = default!;
    public static TextInputFormatter singleLineFormatter = CreateDeny("\n");
    public static TextInputFormatter digitsOnly = CreateAllow(new RegExp("[0-9]"));

    public FilteringTextInputFormatter(
        Pattern filterPattern,
        bool allow,
        string replacementString = ""
    )
    {
        this.filterPattern = filterPattern;
        this.allow = allow;
        this.replacementString = replacementString;
    }

    public static FilteringTextInputFormatter CreateAllow(
        Pattern filterPattern,
        string replacementString = ""
    )
    {
        return new FilteringTextInputFormatter(filterPattern, true, replacementString);
    }

    public static FilteringTextInputFormatter CreateDeny(
        Pattern filterPattern,
        string replacementString = ""
    )
    {
        return new FilteringTextInputFormatter(filterPattern, false, replacementString);
    }

    public virtual TextEditingValue formatEditUpdate(
        TextEditingValue oldValue,
        TextEditingValue newValue
    )
    {
        var formatState = new _TextEditingValueAccumulator(newValue);
        DartRuntimePrimitives.Assert(() => !formatState.debugFinalized);
        IEnumerable<Match> matches = filterPattern.allMatches(newValue.text);
        Match? previousMatch = default!;
        foreach (var match in matches)
        {
            DartRuntimePrimitives.Assert(() => match.end >= match.start);
            _processRegion(allow, previousMatch?.end ?? 0L, match.start, formatState);
            DartRuntimePrimitives.Assert(() => !formatState.debugFinalized);
            _processRegion(!allow, match.start, match.end, formatState);
            DartRuntimePrimitives.Assert(() => !formatState.debugFinalized);
            previousMatch = match;
        }
        _processRegion(allow, previousMatch?.end ?? 0L, newValue.text.Length, formatState);
        DartRuntimePrimitives.Assert(() => !formatState.debugFinalized);
        return formatState.finalize();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _processRegion(
        bool isBannedRegion,
        long regionStart,
        long regionEnd,
        _TextEditingValueAccumulator state
    )
    {
        string replacementString = isBannedRegion
            ? ((regionStart == regionEnd) ? "" : this.replacementString)
            : state.inputValue.text.substring(regionStart, regionEnd);
        state.stringBuffer.Append(replacementString);
        if (replacementString.Length == (regionEnd - regionStart))
        {
            return;
        }
        long adjustIndex(long originalIndex)
        {
            long replacedLength =
                ((originalIndex <= regionStart) && (originalIndex < regionEnd))
                    ? 0L
                    : replacementString.Length;
            long removedLength = originalIndex.clamp(regionStart, regionEnd) - regionStart;
            return replacedLength - removedLength;
            throw new InvalidOperationException(
                "Control flow completed without returning a value."
            );
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

    public LengthLimitingTextInputFormatter(
        long? maxLength,
        MaxLengthEnforcement? maxLengthEnforcement = null
    )
    {
        this.maxLength = maxLength;
        this.maxLengthEnforcement = maxLengthEnforcement;
        System.Diagnostics.Debug.Assert(
            (maxLength is null)
                || (
                    (
                        maxLength
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ) == -1L
                )
                || (
                    (
                        maxLength
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ) > 0L
                )
        );
    }

    public static MaxLengthEnforcement getDefaultMaxLengthEnforcement(
        TargetPlatform? platform = null
    )
    {
        if (ConstantsLibrary.kIsWeb)
        {
            return MaxLengthEnforcement.truncateAfterCompositionEnds;
        }
        else
        {
            switch (platform ?? PlatformLibrary.defaultTargetPlatform)
            {
                case var __case22082 when Equals(__case22082, TargetPlatform.android):
                case var __case22119 when Equals(__case22119, TargetPlatform.windows):
                {
                    return MaxLengthEnforcement.enforced;
                }
                case var __case22204 when Equals(__case22204, TargetPlatform.iOS):
                case var __case22237 when Equals(__case22237, TargetPlatform.macOS):
                case var __case22272 when Equals(__case22272, TargetPlatform.linux):
                case var __case22307 when Equals(__case22307, TargetPlatform.fuchsia):
                {
                    return MaxLengthEnforcement.truncateAfterCompositionEnds;
                }
            }
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static TextEditingValue truncate(TextEditingValue value, long maxLength)
    {
        var iterator = new CharacterRange(value.text);
        if (value.text.characters().Count > maxLength)
        {
            iterator.expandNext(maxLength);
        }
        string truncated = iterator.Current;
        return new TextEditingValue(
            text: truncated,
            selection: value.selection.copyWith(
                baseOffset: Math.Min(value.selection.start, truncated.Length),
                extentOffset: Math.Min(value.selection.end, truncated.Length)
            ),
            composing: (!value.composing.isCollapsed && (truncated.Length > value.composing.start))
                ? new TextRange(
                    start: value.composing.start,
                    end: Math.Min(value.composing.end, truncated.Length)
                )
                : TextRange.empty
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual TextEditingValue formatEditUpdate(
        TextEditingValue oldValue,
        TextEditingValue newValue
    )
    {
        long? maxLength = this.maxLength;
        if (
            (maxLength is null)
            || (
                (
                    maxLength
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ) == -1L
            )
            || (
                newValue.text.characters().Count
                <= (
                    maxLength
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
            )
        )
        {
            return newValue;
        }
        DartRuntimePrimitives.Assert(() =>
            (
                maxLength
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ) > 0L
        );
        switch (maxLengthEnforcement ?? getDefaultMaxLengthEnforcement())
        {
            case var __case23944 when Equals(__case23944, MaxLengthEnforcement.none):
            {
                return newValue;
            }
            case var __case24007 when Equals(__case24007, MaxLengthEnforcement.enforced):
            {
                if (
                    (
                        oldValue.text.characters().Count
                        == (
                            maxLength
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        )
                    ) && oldValue.selection.isCollapsed
                )
                {
                    return oldValue;
                }
                return truncate(
                    newValue,
                    (
                        maxLength
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                );
            }
            case var __case24396
                when Equals(__case24396, MaxLengthEnforcement.truncateAfterCompositionEnds):
            {
                if (
                    (
                        oldValue.text.characters().Count
                        == (
                            maxLength
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        )
                    ) && !oldValue.composing.isValid
                )
                {
                    return oldValue;
                }
                if (newValue.composing.isValid)
                {
                    return newValue;
                }
                return truncate(
                    newValue,
                    (
                        maxLength
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                );
            }
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
