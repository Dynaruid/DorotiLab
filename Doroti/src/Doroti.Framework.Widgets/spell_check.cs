// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/spell_check.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class SpellCheckConfiguration
{
    public virtual SpellCheckService? spellCheckService { get; private set; }
    public virtual Color? misspelledSelectionColor { get; private set; }
    public virtual TextStyle? misspelledTextStyle { get; private set; }
    public virtual Func<BuildContext, EditableTextState, Widget>? spellCheckSuggestionsToolbarBuilder { get; private set; }
    internal virtual bool _spellCheckEnabled { get; private set; } = default!;

    public SpellCheckConfiguration(SpellCheckService? spellCheckService = null, Color? misspelledSelectionColor = null, TextStyle? misspelledTextStyle = null, Func<BuildContext, EditableTextState, Widget>? spellCheckSuggestionsToolbarBuilder = null)
    {
        this.spellCheckService = spellCheckService;
        this.misspelledSelectionColor = misspelledSelectionColor;
        this.misspelledTextStyle = misspelledTextStyle;
        this.spellCheckSuggestionsToolbarBuilder = spellCheckSuggestionsToolbarBuilder;
        _spellCheckEnabled = true;
    }

    public static SpellCheckConfiguration CreateDisabled()
    {
        var __instance = new SpellCheckConfiguration(default!, default!, default!, default!);
        __instance._spellCheckEnabled = false;
        __instance.spellCheckService = null;
        __instance.spellCheckSuggestionsToolbarBuilder = null;
        __instance.misspelledTextStyle = null;
        __instance.misspelledSelectionColor = null;
        return __instance;
    }

    public virtual bool spellCheckEnabled => _spellCheckEnabled;
    public virtual SpellCheckConfiguration copyWith(SpellCheckService? spellCheckService = null, Color? misspelledSelectionColor = null, TextStyle? misspelledTextStyle = null, Func<BuildContext, EditableTextState, Widget>? spellCheckSuggestionsToolbarBuilder = null)
    {
        if (!_spellCheckEnabled)
        {
            return CreateDisabled();
        }
        return new SpellCheckConfiguration(spellCheckService: spellCheckService ?? this.spellCheckService, misspelledSelectionColor: misspelledSelectionColor ?? this.misspelledSelectionColor, misspelledTextStyle: misspelledTextStyle ?? this.misspelledTextStyle, spellCheckSuggestionsToolbarBuilder: spellCheckSuggestionsToolbarBuilder ?? this.spellCheckSuggestionsToolbarBuilder);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "SpellCheckConfiguration")}(" + $"{(_spellCheckEnabled ? "enabled" : "disabled")}, " + $"service: {spellCheckService}, " + $"text style: {misspelledTextStyle}, " + $"toolbar builder: {spellCheckSuggestionsToolbarBuilder}" + ")";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as SpellCheckConfiguration;
        if (__other is null) return false;
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is SpellCheckConfiguration) && Equals(__other.spellCheckService, spellCheckService) && Equals(__other.misspelledTextStyle, misspelledTextStyle) && Equals(__other.spellCheckSuggestionsToolbarBuilder, spellCheckSuggestionsToolbarBuilder) && (__other._spellCheckEnabled == _spellCheckEnabled);
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(spellCheckService, misspelledTextStyle, spellCheckSuggestionsToolbarBuilder, _spellCheckEnabled));
}

public static partial class Spell_checkLibrary
{
    internal static List<SuggestionSpan> _correctSpellCheckResults(string newText, string resultsText, List<SuggestionSpan> results)
    {
        var correctedSpellCheckResults = new List<SuggestionSpan>();
        var spanPointer = 0L;
        var offset = 0L;
        var searchStart = 0L;
        while (spanPointer < checked(results.Count))
        {
            SuggestionSpan currentSpan = results[(int)spanPointer];
            string currentSpanText = resultsText.substring(currentSpan.range.start, currentSpan.range.end);
            long spanLength = currentSpan.range.end - currentSpan.range.start;
            string escapedText = Dart_coreLibrary.escape(currentSpanText);
            var currentSpanTextRegexp = new RegExp($"\\b{escapedText}\\b");
            long foundIndex = currentSpanTextRegexp.allMatches(newText.substring(searchStart)).FirstOrDefault()?.start ?? -1L;
            var currentSpanFoundExactly = currentSpan.range.start == (foundIndex + searchStart);
            var currentSpanFoundExactlyWithOffset = (currentSpan.range.start + offset) == (foundIndex + searchStart);
            bool currentSpanFoundElsewhere = foundIndex >= 0L;
            if (currentSpanFoundExactly || currentSpanFoundExactlyWithOffset)
            {
                var adjustedSpan = new SuggestionSpan(new TextRange(start: currentSpan.range.start + offset, end: currentSpan.range.end + offset), currentSpan.suggestions);
                searchStart = Math.Min(currentSpan.range.end + 1L + offset, newText.Length);
                correctedSpellCheckResults.Add(adjustedSpan);
            }
            else
            {
                if (currentSpanFoundElsewhere)
                {
                    long adjustedSpanStart = searchStart + foundIndex;
                    long adjustedSpanEnd = adjustedSpanStart + spanLength;
                    var adjustedSpanLocal = new SuggestionSpan(new TextRange(start: adjustedSpanStart, end: adjustedSpanEnd), currentSpan.suggestions);
                    searchStart = Math.Min(adjustedSpanEnd + 1L, newText.Length);
                    offset = adjustedSpanStart - currentSpan.range.start;
                    correctedSpellCheckResults.Add(adjustedSpanLocal);
                }
            }
            spanPointer++;
        }
        return correctedSpellCheckResults;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Spell_checkLibrary
{
    public static TextSpan buildTextSpanWithSpellCheckSuggestions(TextEditingValue value, bool composingWithinCurrentTextRange, TextStyle? style, TextStyle misspelledTextStyle, SpellCheckResults spellCheckResults)
    {
        List<SuggestionSpan> spellCheckResultsSpans = spellCheckResults.suggestionSpans.ToList();
        string spellCheckResultsText = spellCheckResults.spellCheckedText;
        if (spellCheckResultsText != value.text)
        {
            spellCheckResultsSpans = _correctSpellCheckResults(value.text, spellCheckResultsText, spellCheckResultsSpans);
        }
        var shouldConsiderComposingRegion = Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.android);
        if (shouldConsiderComposingRegion)
        {
            return new TextSpan(style: style, children: _buildSubtreesWithComposingRegion(spellCheckResultsSpans, value, style, misspelledTextStyle, composingWithinCurrentTextRange).Cast<InlineSpan>().ToList());
        }
        return new TextSpan(style: style, children: _buildSubtreesWithoutComposingRegion(spellCheckResultsSpans, value, style, misspelledTextStyle, value.selection.baseOffset).Cast<InlineSpan>().ToList());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Spell_checkLibrary
{
    internal static List<TextSpan> _buildSubtreesWithoutComposingRegion(List<SuggestionSpan>? spellCheckSuggestions, TextEditingValue value, TextStyle? style, TextStyle misspelledStyle, long cursorIndex)
    {
        var textSpanTreeChildren = new List<TextSpan>();
        var textPointer = 0L;
        var currentSpanPointer = 0L;
        long endIndex = default!;
        string textLocal = value.text;
        TextStyle misspelledJointStyle = style?.merge(misspelledStyle) ?? misspelledStyle;
        var cursorInCurrentSpan = false;
        if (spellCheckSuggestions is not null)
        {
            while ((textPointer < textLocal.Length) && (currentSpanPointer < checked(spellCheckSuggestions.Count)))
            {
                SuggestionSpan currentSpan = spellCheckSuggestions[(int)currentSpanPointer];
                if (currentSpan.range.start > textPointer)
                {
                    endIndex = (currentSpan.range.start < textLocal.Length) ? currentSpan.range.start : textLocal.Length;
                    textSpanTreeChildren.Add(new TextSpan(style: style, text: textLocal.substring(textPointer, endIndex)));
                    textPointer = endIndex;
                }
                else
                {
                    endIndex = (currentSpan.range.end < textLocal.Length) ? currentSpan.range.end : textLocal.Length;
                    cursorInCurrentSpan = (currentSpan.range.start <= cursorIndex) && (currentSpan.range.end >= cursorIndex);
                    textSpanTreeChildren.Add(new TextSpan(style: cursorInCurrentSpan ? style : misspelledJointStyle, text: textLocal.substring(currentSpan.range.start, endIndex)));
                    textPointer = endIndex;
                    currentSpanPointer++;
                }
            }
        }
        if (textPointer < textLocal.Length)
        {
            textSpanTreeChildren.Add(new TextSpan(style: style, text: textLocal.substring(textPointer, textLocal.Length)));
        }
        return textSpanTreeChildren;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Spell_checkLibrary
{
    internal static List<TextSpan> _buildSubtreesWithComposingRegion(List<SuggestionSpan>? spellCheckSuggestions, TextEditingValue value, TextStyle? style, TextStyle misspelledStyle, bool composingWithinCurrentTextRange)
    {
        var textSpanTreeChildren = new List<TextSpan>();
        var textPointer = 0L;
        var currentSpanPointer = 0L;
        long endIndex = default!;
        SuggestionSpan currentSpan = default!;
        string textLocal = value.text;
        TextRange composingRegion = value.composing;
        TextStyle composingTextStyle = style?.merge(new TextStyle(decoration: TextDecoration.underline)) ?? new TextStyle(decoration: TextDecoration.underline);
        TextStyle misspelledJointStyle = style?.merge(misspelledStyle) ?? misspelledStyle;
        var textPointerWithinComposingRegion = false;
        var currentSpanIsComposingRegion = false;
        if (spellCheckSuggestions is not null)
        {
            while ((textPointer < textLocal.Length) && (currentSpanPointer < checked(spellCheckSuggestions.Count)))
            {
                currentSpan = spellCheckSuggestions[(int)currentSpanPointer];
                if (currentSpan.range.start > textPointer)
                {
                    endIndex = (currentSpan.range.start < textLocal.Length) ? currentSpan.range.start : textLocal.Length;
                    textPointerWithinComposingRegion = (composingRegion.start >= textPointer) && (composingRegion.end <= endIndex) && !composingWithinCurrentTextRange;
                    if (textPointerWithinComposingRegion)
                    {
                        _addComposingRegionTextSpans(textSpanTreeChildren, textLocal, textPointer, composingRegion, style, composingTextStyle);
                        textSpanTreeChildren.Add(new TextSpan(style: style, text: textLocal.substring(composingRegion.end, endIndex)));
                    }
                    else
                    {
                        textSpanTreeChildren.Add(new TextSpan(style: style, text: textLocal.substring(textPointer, endIndex)));
                    }
                    textPointer = endIndex;
                }
                else
                {
                    endIndex = (currentSpan.range.end < textLocal.Length) ? currentSpan.range.end : textLocal.Length;
                    currentSpanIsComposingRegion = (textPointer >= composingRegion.start) && (endIndex <= composingRegion.end) && !composingWithinCurrentTextRange;
                    textSpanTreeChildren.Add(new TextSpan(style: currentSpanIsComposingRegion ? composingTextStyle : misspelledJointStyle, text: textLocal.substring(currentSpan.range.start, endIndex)));
                    textPointer = endIndex;
                    currentSpanPointer++;
                }
            }
        }
        if (textPointer < textLocal.Length)
        {
            if ((textPointer < composingRegion.start) && !composingWithinCurrentTextRange)
            {
                _addComposingRegionTextSpans(textSpanTreeChildren, textLocal, textPointer, composingRegion, style, composingTextStyle);
                if (composingRegion.end != textLocal.Length)
                {
                    textSpanTreeChildren.Add(new TextSpan(style: style, text: textLocal.substring(composingRegion.end, textLocal.Length)));
                }
            }
            else
            {
                textSpanTreeChildren.Add(new TextSpan(style: style, text: textLocal.substring(textPointer, textLocal.Length)));
            }
        }
        return textSpanTreeChildren;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Spell_checkLibrary
{
    internal static void _addComposingRegionTextSpans(List<TextSpan> treeChildren, string text, long start, TextRange composingRegion, TextStyle? style, TextStyle composingTextStyle)
    {
        treeChildren.Add(new TextSpan(style: style, text: text.substring(start, composingRegion.start)));
        treeChildren.Add(new TextSpan(style: composingTextStyle, text: text.substring(composingRegion.start, composingRegion.end)));
    }
}

