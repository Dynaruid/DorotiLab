#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/spell_check.dart
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

public class SuggestionSpan
{
    public virtual TextRange range { get; private set; } = default!;
    public virtual List<string> suggestions { get; private set; } = default!;

    public SuggestionSpan(TextRange range, List<string> suggestions)
    {
        this.range = range;
        this.suggestions = suggestions;
    }

    public override bool Equals(object? other)
    {
        var __other = other as SuggestionSpan;
        if (__other is null) return false;
        if (ReferenceEquals(this, __other))
        {
            return true;
        }
        return ((((__other is SuggestionSpan) && (((SuggestionSpan)__other).range.start == range.start)) && (((SuggestionSpan)__other).range.end == range.end)) && global::Doroti.Framework.Foundation.CollectionsLibrary.listEquals<string>(((SuggestionSpan)__other).suggestions, suggestions));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(range.start, range.end, FoundationRuntimePorts.ObjectHashAll(suggestions));
    public override string ToString()
    {
        return $"SuggestionSpan(range: {range}, suggestions: {suggestions})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SpellCheckResults
{
    public virtual string spellCheckedText { get; private set; } = default!;
    public virtual List<SuggestionSpan> suggestionSpans { get; private set; } = default!;

    public SpellCheckResults(string spellCheckedText, List<SuggestionSpan> suggestionSpans)
    {
        this.spellCheckedText = spellCheckedText;
        this.suggestionSpans = suggestionSpans;
    }

    public override bool Equals(object? other)
    {
        var __other = other as SpellCheckResults;
        if (__other is null) return false;
        if (ReferenceEquals(this, __other))
        {
            return true;
        }
        return (((__other is SpellCheckResults) && (((SpellCheckResults)__other).spellCheckedText == spellCheckedText)) && global::Doroti.Framework.Foundation.CollectionsLibrary.listEquals<SuggestionSpan>(((SpellCheckResults)__other).suggestionSpans, suggestionSpans));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(spellCheckedText, FoundationRuntimePorts.ObjectHashAll(suggestionSpans));
    public override string ToString()
    {
        return $"SpellCheckResults(spellCheckText: {spellCheckedText}, suggestionSpans: {suggestionSpans})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public interface SpellCheckService
{
    public Future<List<SuggestionSpan>?> fetchSpellCheckSuggestions(Locale locale, string text);
}

public class DefaultSpellCheckService : SpellCheckService
{
    public virtual SpellCheckResults? lastSavedResults { get; set; } = default;
    public virtual MethodChannel spellCheckChannel { get; set; } = default!;

    public DefaultSpellCheckService()
    {
    }

    public static List<SuggestionSpan> mergeResults(List<SuggestionSpan> oldResults, List<SuggestionSpan> newResults)
    {
        var mergedResults = new List<SuggestionSpan>();
        SuggestionSpan oldSpan = default!;
        SuggestionSpan newSpan = default!;
        var oldSpanPointer = 0L;
        var newSpanPointer = 0L;
        while (((oldSpanPointer < oldResults.Count) && (newSpanPointer < newResults.Count)))
        {
            oldSpan = oldResults[(int)(oldSpanPointer)];
            newSpan = newResults[(int)(newSpanPointer)];
            if ((oldSpan.range.start == newSpan.range.start))
            {
                mergedResults.Add(oldSpan);
                oldSpanPointer++;
                newSpanPointer++;
            }
            else
            {
                if ((oldSpan.range.start < newSpan.range.start))
                {
                    mergedResults.Add(oldSpan);
                    oldSpanPointer++;
                }
                else
                {
                    mergedResults.Add(newSpan);
                    newSpanPointer++;
                }
            }
        }
        mergedResults.AddRange(oldResults.Skip(checked((int)oldSpanPointer)).ToList());
        mergedResults.AddRange(newResults.Skip(checked((int)newSpanPointer)).ToList());
        return mergedResults;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async virtual Future<List<SuggestionSpan>?> fetchSpellCheckSuggestions(Locale locale, string text)
    {
        List<object> rawResults = default!;
        string languageTag = locale.toLanguageTag();
        try
        {
            rawResults = ((List<object>?)await spellCheckChannel.invokeMethod<object>("SpellCheck.initiateSpellCheck", new List<string> { languageTag, text }))!;
        }
        catch (Exception e)
        {
            return null;
        }
        var suggestionSpans = new List<SuggestionSpan>();
        if ((lastSavedResults is not null))
        {
            var textHasNotChanged__7033 = (lastSavedResults!.spellCheckedText == text);
            bool spansHaveChanged__7114 = global::Doroti.Framework.Foundation.CollectionsLibrary.listEquals(lastSavedResults!.suggestionSpans, suggestionSpans);
            if ((textHasNotChanged__7033 && spansHaveChanged__7114))
            {
                suggestionSpans = mergeResults(lastSavedResults!.suggestionSpans, suggestionSpans);
            }
        }
        lastSavedResults = new SpellCheckResults(text, suggestionSpans);
        return suggestionSpans;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

