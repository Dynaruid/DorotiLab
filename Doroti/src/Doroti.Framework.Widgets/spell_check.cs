// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/spell_check.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Widgets;

public class SpellCheckConfiguration
{
    public virtual global::Doroti.Framework.Services.SpellCheckService? spellCheckService { get; private set; }
    public virtual Color? misspelledSelectionColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? misspelledTextStyle { get; private set; }
    public virtual global::System.Func<BuildContext, EditableTextState, Widget>? spellCheckSuggestionsToolbarBuilder { get; private set; }
    internal virtual bool _spellCheckEnabled { get; private set; } = default!;

    public SpellCheckConfiguration(global::Doroti.Framework.Services.SpellCheckService? spellCheckService = null, Color? misspelledSelectionColor = null, global::Doroti.Framework.Painting.TextStyle? misspelledTextStyle = null, global::System.Func<BuildContext, EditableTextState, Widget>? spellCheckSuggestionsToolbarBuilder = null)
    {
        this.spellCheckService = spellCheckService;
        this.misspelledSelectionColor = misspelledSelectionColor;
        this.misspelledTextStyle = misspelledTextStyle;
        this.spellCheckSuggestionsToolbarBuilder = spellCheckSuggestionsToolbarBuilder;
        this._spellCheckEnabled = true;
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

    public virtual bool spellCheckEnabled => this._spellCheckEnabled;
    public virtual SpellCheckConfiguration copyWith(global::Doroti.Framework.Services.SpellCheckService? spellCheckService = null, Color? misspelledSelectionColor = null, global::Doroti.Framework.Painting.TextStyle? misspelledTextStyle = null, global::System.Func<BuildContext, EditableTextState, Widget>? spellCheckSuggestionsToolbarBuilder = null)
    {
        if (!this._spellCheckEnabled)
        {
            return SpellCheckConfiguration.CreateDisabled();
        }
        return new SpellCheckConfiguration(spellCheckService: (spellCheckService ?? this.spellCheckService), misspelledSelectionColor: (misspelledSelectionColor ?? this.misspelledSelectionColor), misspelledTextStyle: (misspelledTextStyle ?? this.misspelledTextStyle), spellCheckSuggestionsToolbarBuilder: ((spellCheckSuggestionsToolbarBuilder ?? (global::System.Func<BuildContext, EditableTextState, Widget>)this.spellCheckSuggestionsToolbarBuilder)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "SpellCheckConfiguration"))}(" + $"{(this._spellCheckEnabled ? "enabled" : "disabled")}, " + $"service: {this.spellCheckService}, " + $"text style: {this.misspelledTextStyle}, " + $"toolbar builder: {this.spellCheckSuggestionsToolbarBuilder}" + ")";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as SpellCheckConfiguration;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((__other is SpellCheckConfiguration) && (object.Equals(((SpellCheckConfiguration)((SpellCheckConfiguration)__other)).spellCheckService, this.spellCheckService))) && (object.Equals(((SpellCheckConfiguration)((SpellCheckConfiguration)__other)).misspelledTextStyle, this.misspelledTextStyle))) && (object.Equals((global::System.Func<BuildContext, EditableTextState, Widget>?)((SpellCheckConfiguration)((SpellCheckConfiguration)__other)).spellCheckSuggestionsToolbarBuilder, (global::System.Func<BuildContext, EditableTextState, Widget>?)this.spellCheckSuggestionsToolbarBuilder))) && (((SpellCheckConfiguration)((SpellCheckConfiguration)__other))._spellCheckEnabled == this._spellCheckEnabled));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.spellCheckService, this.misspelledTextStyle, this.spellCheckSuggestionsToolbarBuilder, this._spellCheckEnabled));
}

public static partial class Spell_checkLibrary
{
    internal static List<global::Doroti.Framework.Services.SuggestionSpan> _correctSpellCheckResults(string newText, string resultsText, List<global::Doroti.Framework.Services.SuggestionSpan> results)
    {
        var correctedSpellCheckResults = new List<global::Doroti.Framework.Services.SuggestionSpan>();
        var spanPointer = 0L;
        var offset = 0L;
        var searchStart = 0L;
        while ((spanPointer < checked((long)(results.Count))))
        {
            global::Doroti.Framework.Services.SuggestionSpan currentSpan = results[(int)(spanPointer)];
            string currentSpanText = resultsText.substring(((global::Doroti.Framework.Services.SuggestionSpan)currentSpan).range.start, ((global::Doroti.Framework.Services.SuggestionSpan)currentSpan).range.end);
            long spanLength = (((global::Doroti.Framework.Services.SuggestionSpan)currentSpan).range.end - ((global::Doroti.Framework.Services.SuggestionSpan)currentSpan).range.start);
            string escapedText = Dart_coreLibrary.escape(currentSpanText);
            var currentSpanTextRegexp = new RegExp($"\\b{escapedText}\\b");
            long foundIndex = ((long)((dynamic)newText.substring(searchStart)).IndexOf(currentSpanTextRegexp));
            var currentSpanFoundExactly = (((global::Doroti.Framework.Services.SuggestionSpan)currentSpan).range.start == (foundIndex + searchStart));
            var currentSpanFoundExactlyWithOffset = ((((global::Doroti.Framework.Services.SuggestionSpan)currentSpan).range.start + offset) == (foundIndex + searchStart));
            bool currentSpanFoundElsewhere = (foundIndex >= 0L);
            if ((currentSpanFoundExactly || currentSpanFoundExactlyWithOffset))
            {
                var adjustedSpan = new global::Doroti.Framework.Services.SuggestionSpan(new global::Doroti.Ui.TextRange(start: (((global::Doroti.Framework.Services.SuggestionSpan)currentSpan).range.start + offset), end: (((global::Doroti.Framework.Services.SuggestionSpan)currentSpan).range.end + offset)), ((global::Doroti.Framework.Services.SuggestionSpan)currentSpan).suggestions);
                searchStart = Math.Min(((((global::Doroti.Framework.Services.SuggestionSpan)currentSpan).range.end + 1L) + offset), newText.Length);
                correctedSpellCheckResults.Add(adjustedSpan);
            }
            else
            {
                if (currentSpanFoundElsewhere)
                {
                    long adjustedSpanStart = (searchStart + foundIndex);
                    long adjustedSpanEnd = (adjustedSpanStart + spanLength);
                    var adjustedSpanLocal = new global::Doroti.Framework.Services.SuggestionSpan(new global::Doroti.Ui.TextRange(start: adjustedSpanStart, end: adjustedSpanEnd), ((global::Doroti.Framework.Services.SuggestionSpan)currentSpan).suggestions);
                    searchStart = Math.Min((adjustedSpanEnd + 1L), newText.Length);
                    offset = (adjustedSpanStart - ((global::Doroti.Framework.Services.SuggestionSpan)currentSpan).range.start);
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
    public static global::Doroti.Framework.Painting.TextSpan buildTextSpanWithSpellCheckSuggestions(global::Doroti.Framework.Services.TextEditingValue value, bool composingWithinCurrentTextRange, global::Doroti.Framework.Painting.TextStyle? style, global::Doroti.Framework.Painting.TextStyle misspelledTextStyle, global::Doroti.Framework.Services.SpellCheckResults spellCheckResults)
    {
        List<global::Doroti.Framework.Services.SuggestionSpan> spellCheckResultsSpans = ((global::Doroti.Framework.Services.SpellCheckResults)spellCheckResults).suggestionSpans.ToList();
        string spellCheckResultsText = ((global::Doroti.Framework.Services.SpellCheckResults)spellCheckResults).spellCheckedText;
        if ((spellCheckResultsText != ((global::Doroti.Framework.Services.TextEditingValue)value).text))
        {
            spellCheckResultsSpans = Spell_checkLibrary._correctSpellCheckResults(((global::Doroti.Framework.Services.TextEditingValue)value).text, spellCheckResultsText, spellCheckResultsSpans);
        }
        var shouldConsiderComposingRegion = (object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.android));
        if (shouldConsiderComposingRegion)
        {
            return new global::Doroti.Framework.Painting.TextSpan(style: style, children: Spell_checkLibrary._buildSubtreesWithComposingRegion(spellCheckResultsSpans, value, style, misspelledTextStyle, composingWithinCurrentTextRange).Cast<global::Doroti.Framework.Painting.InlineSpan>().ToList());
        }
        return new global::Doroti.Framework.Painting.TextSpan(style: style, children: Spell_checkLibrary._buildSubtreesWithoutComposingRegion(spellCheckResultsSpans, value, style, misspelledTextStyle, ((global::Doroti.Framework.Services.TextEditingValue)value).selection.baseOffset).Cast<global::Doroti.Framework.Painting.InlineSpan>().ToList());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Spell_checkLibrary
{
    internal static List<global::Doroti.Framework.Painting.TextSpan> _buildSubtreesWithoutComposingRegion(List<global::Doroti.Framework.Services.SuggestionSpan>? spellCheckSuggestions, global::Doroti.Framework.Services.TextEditingValue value, global::Doroti.Framework.Painting.TextStyle? style, global::Doroti.Framework.Painting.TextStyle misspelledStyle, long cursorIndex)
    {
        var textSpanTreeChildren = new List<global::Doroti.Framework.Painting.TextSpan>();
        var textPointer = 0L;
        var currentSpanPointer = 0L;
        long endIndex = default!;
        string textLocal = ((global::Doroti.Framework.Services.TextEditingValue)value).text;
        global::Doroti.Framework.Painting.TextStyle misspelledJointStyle = (style?.merge(misspelledStyle) ?? misspelledStyle);
        var cursorInCurrentSpan = false;
        if ((spellCheckSuggestions is not null))
        {
            while (((textPointer < textLocal.Length) && (currentSpanPointer < checked((long)(spellCheckSuggestions.Count)))))
            {
                global::Doroti.Framework.Services.SuggestionSpan currentSpan = spellCheckSuggestions[(int)(currentSpanPointer)];
                if ((((global::Doroti.Framework.Services.SuggestionSpan)currentSpan).range.start > textPointer))
                {
                    endIndex = ((((global::Doroti.Framework.Services.SuggestionSpan)currentSpan).range.start < textLocal.Length) ? ((global::Doroti.Framework.Services.SuggestionSpan)currentSpan).range.start : textLocal.Length);
                    textSpanTreeChildren.Add(new global::Doroti.Framework.Painting.TextSpan(style: style, text: textLocal.substring(textPointer, endIndex)));
                    textPointer = endIndex;
                }
                else
                {
                    endIndex = ((((global::Doroti.Framework.Services.SuggestionSpan)currentSpan).range.end < textLocal.Length) ? ((global::Doroti.Framework.Services.SuggestionSpan)currentSpan).range.end : textLocal.Length);
                    cursorInCurrentSpan = ((((global::Doroti.Framework.Services.SuggestionSpan)currentSpan).range.start <= cursorIndex) && (((global::Doroti.Framework.Services.SuggestionSpan)currentSpan).range.end >= cursorIndex));
                    textSpanTreeChildren.Add(new global::Doroti.Framework.Painting.TextSpan(style: (cursorInCurrentSpan ? style : misspelledJointStyle), text: textLocal.substring(((global::Doroti.Framework.Services.SuggestionSpan)currentSpan).range.start, endIndex)));
                    textPointer = endIndex;
                    currentSpanPointer++;
                }
            }
        }
        if ((textPointer < textLocal.Length))
        {
            textSpanTreeChildren.Add(new global::Doroti.Framework.Painting.TextSpan(style: style, text: textLocal.substring(textPointer, textLocal.Length)));
        }
        return textSpanTreeChildren;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Spell_checkLibrary
{
    internal static List<global::Doroti.Framework.Painting.TextSpan> _buildSubtreesWithComposingRegion(List<global::Doroti.Framework.Services.SuggestionSpan>? spellCheckSuggestions, global::Doroti.Framework.Services.TextEditingValue value, global::Doroti.Framework.Painting.TextStyle? style, global::Doroti.Framework.Painting.TextStyle misspelledStyle, bool composingWithinCurrentTextRange)
    {
        var textSpanTreeChildren = new List<global::Doroti.Framework.Painting.TextSpan>();
        var textPointer = 0L;
        var currentSpanPointer = 0L;
        long endIndex = default!;
        global::Doroti.Framework.Services.SuggestionSpan currentSpan = default!;
        string textLocal = ((global::Doroti.Framework.Services.TextEditingValue)value).text;
        global::Doroti.Ui.TextRange composingRegion = ((global::Doroti.Ui.TextRange)(object?)((global::Doroti.Framework.Services.TextEditingValue)value).composing);
        global::Doroti.Framework.Painting.TextStyle composingTextStyle = (style?.merge(new global::Doroti.Framework.Painting.TextStyle(decoration: TextDecoration.underline)) ?? new global::Doroti.Framework.Painting.TextStyle(decoration: TextDecoration.underline));
        global::Doroti.Framework.Painting.TextStyle misspelledJointStyle = (style?.merge(misspelledStyle) ?? misspelledStyle);
        var textPointerWithinComposingRegion = false;
        var currentSpanIsComposingRegion = false;
        if ((spellCheckSuggestions is not null))
        {
            while (((textPointer < textLocal.Length) && (currentSpanPointer < checked((long)(spellCheckSuggestions.Count)))))
            {
                currentSpan = spellCheckSuggestions[(int)(currentSpanPointer)];
                if ((((global::Doroti.Framework.Services.SuggestionSpan)currentSpan).range.start > textPointer))
                {
                    endIndex = ((((global::Doroti.Framework.Services.SuggestionSpan)currentSpan).range.start < textLocal.Length) ? ((global::Doroti.Framework.Services.SuggestionSpan)currentSpan).range.start : textLocal.Length);
                    textPointerWithinComposingRegion = (((composingRegion.start >= textPointer) && (composingRegion.end <= endIndex)) && !composingWithinCurrentTextRange);
                    if (textPointerWithinComposingRegion)
                    {
                        Spell_checkLibrary._addComposingRegionTextSpans(textSpanTreeChildren, textLocal, textPointer, composingRegion, style, composingTextStyle);
                        textSpanTreeChildren.Add(new global::Doroti.Framework.Painting.TextSpan(style: style, text: textLocal.substring(composingRegion.end, endIndex)));
                    }
                    else
                    {
                        textSpanTreeChildren.Add(new global::Doroti.Framework.Painting.TextSpan(style: style, text: textLocal.substring(textPointer, endIndex)));
                    }
                    textPointer = endIndex;
                }
                else
                {
                    endIndex = ((((global::Doroti.Framework.Services.SuggestionSpan)currentSpan).range.end < textLocal.Length) ? ((global::Doroti.Framework.Services.SuggestionSpan)currentSpan).range.end : textLocal.Length);
                    currentSpanIsComposingRegion = (((textPointer >= composingRegion.start) && (endIndex <= composingRegion.end)) && !composingWithinCurrentTextRange);
                    textSpanTreeChildren.Add(new global::Doroti.Framework.Painting.TextSpan(style: (currentSpanIsComposingRegion ? composingTextStyle : misspelledJointStyle), text: textLocal.substring(((global::Doroti.Framework.Services.SuggestionSpan)currentSpan).range.start, endIndex)));
                    textPointer = endIndex;
                    currentSpanPointer++;
                }
            }
        }
        if ((textPointer < textLocal.Length))
        {
            if (((textPointer < composingRegion.start) && !composingWithinCurrentTextRange))
            {
                Spell_checkLibrary._addComposingRegionTextSpans(textSpanTreeChildren, textLocal, textPointer, composingRegion, style, composingTextStyle);
                if ((composingRegion.end != textLocal.Length))
                {
                    textSpanTreeChildren.Add(new global::Doroti.Framework.Painting.TextSpan(style: style, text: textLocal.substring(composingRegion.end, textLocal.Length)));
                }
            }
            else
            {
                textSpanTreeChildren.Add(new global::Doroti.Framework.Painting.TextSpan(style: style, text: textLocal.substring(textPointer, textLocal.Length)));
            }
        }
        return textSpanTreeChildren;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Spell_checkLibrary
{
    internal static void _addComposingRegionTextSpans(List<global::Doroti.Framework.Painting.TextSpan> treeChildren, string text, long start, TextRange composingRegion, global::Doroti.Framework.Painting.TextStyle? style, global::Doroti.Framework.Painting.TextStyle composingTextStyle)
    {
        treeChildren.Add(new global::Doroti.Framework.Painting.TextSpan(style: style, text: text.substring(start, composingRegion.start)));
        treeChildren.Add(new global::Doroti.Framework.Painting.TextSpan(style: composingTextStyle, text: text.substring(composingRegion.start, composingRegion.end)));
    }
}

