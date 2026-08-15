// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/spell_check.dart
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

namespace Doroti.Generated.Framework.Widgets;

public class SpellCheckConfiguration
{
    public virtual global::Doroti.Generated.Framework.Services.SpellCheckService? spellCheckService { get; private set; }
    public virtual Color? misspelledSelectionColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? misspelledTextStyle { get; private set; }
    public virtual global::System.Func<BuildContext, EditableTextState, Widget>? spellCheckSuggestionsToolbarBuilder { get; private set; }
    internal virtual bool _spellCheckEnabled { get; private set; } = default!;

    public SpellCheckConfiguration(global::Doroti.Generated.Framework.Services.SpellCheckService? spellCheckService = null, Color? misspelledSelectionColor = null, global::Doroti.Generated.Framework.Painting.TextStyle? misspelledTextStyle = null, global::System.Func<BuildContext, EditableTextState, Widget>? spellCheckSuggestionsToolbarBuilder = null)
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
    public virtual SpellCheckConfiguration copyWith(global::Doroti.Generated.Framework.Services.SpellCheckService? spellCheckService = null, Color? misspelledSelectionColor = null, global::Doroti.Generated.Framework.Painting.TextStyle? misspelledTextStyle = null, global::System.Func<BuildContext, EditableTextState, Widget>? spellCheckSuggestionsToolbarBuilder = null)
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
        return $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "SpellCheckConfiguration"))}(" + $"{(this._spellCheckEnabled ? "enabled" : "disabled")}, " + $"service: {this.spellCheckService}, " + $"text style: {this.misspelledTextStyle}, " + $"toolbar builder: {this.spellCheckSuggestionsToolbarBuilder}" + ")";
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
    internal static List<global::Doroti.Generated.Framework.Services.SuggestionSpan> _correctSpellCheckResults(string newText, string resultsText, List<global::Doroti.Generated.Framework.Services.SuggestionSpan> results)
    {
        var correctedSpellCheckResults__4821 = new List<global::Doroti.Generated.Framework.Services.SuggestionSpan>();
        var spanPointer__4876 = 0L;
        var offset__4899 = 0L;
        var searchStart__5062 = 0L;
        while ((spanPointer__4876 < checked((long)(results.Count))))
        {
            global::Doroti.Generated.Framework.Services.SuggestionSpan currentSpan__5146 = results[(int)(spanPointer__4876)];
            string currentSpanText__5199 = resultsText.substring(((global::Doroti.Generated.Framework.Services.SuggestionSpan)currentSpan__5146).range.start, ((global::Doroti.Generated.Framework.Services.SuggestionSpan)currentSpan__5146).range.end);
            long spanLength__5321 = (((global::Doroti.Generated.Framework.Services.SuggestionSpan)currentSpan__5146).range.end - ((global::Doroti.Generated.Framework.Services.SuggestionSpan)currentSpan__5146).range.start);
            string escapedText__5465 = Dart_coreLibrary.escape(currentSpanText__5199);
            var currentSpanTextRegexp__5521 = new RegExp($"\\b{escapedText__5465}\\b");
            long foundIndex__5589 = ((long)((dynamic)newText.substring(searchStart__5062)).IndexOf(currentSpanTextRegexp__5521));
            var currentSpanFoundExactly__5764 = (((global::Doroti.Generated.Framework.Services.SuggestionSpan)currentSpan__5146).range.start == (foundIndex__5589 + searchStart__5062));
            var currentSpanFoundExactlyWithOffset__5853 = ((((global::Doroti.Generated.Framework.Services.SuggestionSpan)currentSpan__5146).range.start + offset__4899) == (foundIndex__5589 + searchStart__5062));
            bool currentSpanFoundElsewhere__5974 = (foundIndex__5589 >= 0L);
            if ((currentSpanFoundExactly__5764 || currentSpanFoundExactlyWithOffset__5853))
            {
                var adjustedSpan__6362 = new global::Doroti.Generated.Framework.Services.SuggestionSpan(new global::Doroti.Ui.TextRange(start: (((global::Doroti.Generated.Framework.Services.SuggestionSpan)currentSpan__5146).range.start + offset__4899), end: (((global::Doroti.Generated.Framework.Services.SuggestionSpan)currentSpan__5146).range.end + offset__4899)), ((global::Doroti.Generated.Framework.Services.SuggestionSpan)currentSpan__5146).suggestions);
                searchStart__5062 = Math.Min(((((global::Doroti.Generated.Framework.Services.SuggestionSpan)currentSpan__5146).range.end + 1L) + offset__4899), newText.Length);
                correctedSpellCheckResults__4821.Add(adjustedSpan__6362);
            }
            else
            {
                if (currentSpanFoundElsewhere__5974)
                {
                    long adjustedSpanStart__6856 = (searchStart__5062 + foundIndex__5589);
                    long adjustedSpanEnd__6918 = (adjustedSpanStart__6856 + spanLength__5321);
                    var adjustedSpan__6980 = new global::Doroti.Generated.Framework.Services.SuggestionSpan(new global::Doroti.Ui.TextRange(start: adjustedSpanStart__6856, end: adjustedSpanEnd__6918), ((global::Doroti.Generated.Framework.Services.SuggestionSpan)currentSpan__5146).suggestions);
                    searchStart__5062 = Math.Min((adjustedSpanEnd__6918 + 1L), newText.Length);
                    offset__4899 = (adjustedSpanStart__6856 - ((global::Doroti.Generated.Framework.Services.SuggestionSpan)currentSpan__5146).range.start);
                    correctedSpellCheckResults__4821.Add(adjustedSpan__6980);
                }
            }
            spanPointer__4876++;
        }
        return correctedSpellCheckResults__4821;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Spell_checkLibrary
{
    public static global::Doroti.Generated.Framework.Painting.TextSpan buildTextSpanWithSpellCheckSuggestions(global::Doroti.Generated.Framework.Services.TextEditingValue value, bool composingWithinCurrentTextRange, global::Doroti.Generated.Framework.Painting.TextStyle? style, global::Doroti.Generated.Framework.Painting.TextStyle misspelledTextStyle, global::Doroti.Generated.Framework.Services.SpellCheckResults spellCheckResults)
    {
        List<global::Doroti.Generated.Framework.Services.SuggestionSpan> spellCheckResultsSpans__8437 = ((global::Doroti.Generated.Framework.Services.SpellCheckResults)spellCheckResults).suggestionSpans.ToList();
        string spellCheckResultsText__8512 = ((global::Doroti.Generated.Framework.Services.SpellCheckResults)spellCheckResults).spellCheckedText;
        if ((spellCheckResultsText__8512 != ((global::Doroti.Generated.Framework.Services.TextEditingValue)value).text))
        {
            spellCheckResultsSpans__8437 = Spell_checkLibrary._correctSpellCheckResults(((global::Doroti.Generated.Framework.Services.TextEditingValue)value).text, spellCheckResultsText__8512, spellCheckResultsSpans__8437);
        }
        var shouldConsiderComposingRegion__9066 = (object.Equals(global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Generated.Framework.Foundation.TargetPlatform.android));
        if (shouldConsiderComposingRegion__9066)
        {
            return new global::Doroti.Generated.Framework.Painting.TextSpan(style: style, children: Spell_checkLibrary._buildSubtreesWithComposingRegion(spellCheckResultsSpans__8437, value, style, misspelledTextStyle, composingWithinCurrentTextRange).Cast<global::Doroti.Generated.Framework.Painting.InlineSpan>().ToList());
        }
        return new global::Doroti.Generated.Framework.Painting.TextSpan(style: style, children: Spell_checkLibrary._buildSubtreesWithoutComposingRegion(spellCheckResultsSpans__8437, value, style, misspelledTextStyle, ((global::Doroti.Generated.Framework.Services.TextEditingValue)value).selection.baseOffset).Cast<global::Doroti.Generated.Framework.Painting.InlineSpan>().ToList());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Spell_checkLibrary
{
    internal static List<global::Doroti.Generated.Framework.Painting.TextSpan> _buildSubtreesWithoutComposingRegion(List<global::Doroti.Generated.Framework.Services.SuggestionSpan>? spellCheckSuggestions, global::Doroti.Generated.Framework.Services.TextEditingValue value, global::Doroti.Generated.Framework.Painting.TextStyle? style, global::Doroti.Generated.Framework.Painting.TextStyle misspelledStyle, long cursorIndex)
    {
        var textSpanTreeChildren__10161 = new List<global::Doroti.Generated.Framework.Painting.TextSpan>();
        var textPointer__10205 = 0L;
        var currentSpanPointer__10228 = 0L;
        long endIndex__10258 = default!;
        string text__10283 = ((global::Doroti.Generated.Framework.Services.TextEditingValue)value).text;
        global::Doroti.Generated.Framework.Painting.TextStyle misspelledJointStyle__10320 = (style?.merge(misspelledStyle) ?? misspelledStyle);
        var cursorInCurrentSpan__10399 = false;
        if ((spellCheckSuggestions is not null))
        {
            while (((textPointer__10205 < text__10283.Length) && (currentSpanPointer__10228 < checked((long)(spellCheckSuggestions.Count)))))
            {
                global::Doroti.Generated.Framework.Services.SuggestionSpan currentSpan__10652 = spellCheckSuggestions[(int)(currentSpanPointer__10228)];
                if ((((global::Doroti.Generated.Framework.Services.SuggestionSpan)currentSpan__10652).range.start > textPointer__10205))
                {
                    endIndex__10258 = ((((global::Doroti.Generated.Framework.Services.SuggestionSpan)currentSpan__10652).range.start < text__10283.Length) ? ((global::Doroti.Generated.Framework.Services.SuggestionSpan)currentSpan__10652).range.start : text__10283.Length);
                    textSpanTreeChildren__10161.Add(new global::Doroti.Generated.Framework.Painting.TextSpan(style: style, text: text__10283.substring(textPointer__10205, endIndex__10258)));
                    textPointer__10205 = endIndex__10258;
                }
                else
                {
                    endIndex__10258 = ((((global::Doroti.Generated.Framework.Services.SuggestionSpan)currentSpan__10652).range.end < text__10283.Length) ? ((global::Doroti.Generated.Framework.Services.SuggestionSpan)currentSpan__10652).range.end : text__10283.Length);
                    cursorInCurrentSpan__10399 = ((((global::Doroti.Generated.Framework.Services.SuggestionSpan)currentSpan__10652).range.start <= cursorIndex) && (((global::Doroti.Generated.Framework.Services.SuggestionSpan)currentSpan__10652).range.end >= cursorIndex));
                    textSpanTreeChildren__10161.Add(new global::Doroti.Generated.Framework.Painting.TextSpan(style: (cursorInCurrentSpan__10399 ? style : misspelledJointStyle__10320), text: text__10283.substring(((global::Doroti.Generated.Framework.Services.SuggestionSpan)currentSpan__10652).range.start, endIndex__10258)));
                    textPointer__10205 = endIndex__10258;
                    currentSpanPointer__10228++;
                }
            }
        }
        if ((textPointer__10205 < text__10283.Length))
        {
            textSpanTreeChildren__10161.Add(new global::Doroti.Generated.Framework.Painting.TextSpan(style: style, text: text__10283.substring(textPointer__10205, text__10283.Length)));
        }
        return textSpanTreeChildren__10161;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Spell_checkLibrary
{
    internal static List<global::Doroti.Generated.Framework.Painting.TextSpan> _buildSubtreesWithComposingRegion(List<global::Doroti.Generated.Framework.Services.SuggestionSpan>? spellCheckSuggestions, global::Doroti.Generated.Framework.Services.TextEditingValue value, global::Doroti.Generated.Framework.Painting.TextStyle? style, global::Doroti.Generated.Framework.Painting.TextStyle misspelledStyle, bool composingWithinCurrentTextRange)
    {
        var textSpanTreeChildren__12125 = new List<global::Doroti.Generated.Framework.Painting.TextSpan>();
        var textPointer__12169 = 0L;
        var currentSpanPointer__12192 = 0L;
        long endIndex__12222 = default!;
        global::Doroti.Generated.Framework.Services.SuggestionSpan currentSpan__12249 = default!;
        string text__12277 = ((global::Doroti.Generated.Framework.Services.TextEditingValue)value).text;
        global::Doroti.Ui.TextRange composingRegion__12314 = ((global::Doroti.Ui.TextRange)(object?)((global::Doroti.Generated.Framework.Services.TextEditingValue)value).composing);
        global::Doroti.Generated.Framework.Painting.TextStyle composingTextStyle__12367 = (style?.merge(new global::Doroti.Generated.Framework.Painting.TextStyle(decoration: TextDecoration.underline)) ?? new global::Doroti.Generated.Framework.Painting.TextStyle(decoration: TextDecoration.underline));
        global::Doroti.Generated.Framework.Painting.TextStyle misspelledJointStyle__12544 = (style?.merge(misspelledStyle) ?? misspelledStyle);
        var textPointerWithinComposingRegion__12623 = false;
        var currentSpanIsComposingRegion__12671 = false;
        if ((spellCheckSuggestions is not null))
        {
            while (((textPointer__12169 < text__12277.Length) && (currentSpanPointer__12192 < checked((long)(spellCheckSuggestions.Count)))))
            {
                currentSpan__12249 = spellCheckSuggestions[(int)(currentSpanPointer__12192)];
                if ((((global::Doroti.Generated.Framework.Services.SuggestionSpan)currentSpan__12249).range.start > textPointer__12169))
                {
                    endIndex__12222 = ((((global::Doroti.Generated.Framework.Services.SuggestionSpan)currentSpan__12249).range.start < text__12277.Length) ? ((global::Doroti.Generated.Framework.Services.SuggestionSpan)currentSpan__12249).range.start : text__12277.Length);
                    textPointerWithinComposingRegion__12623 = (((composingRegion__12314.start >= textPointer__12169) && (composingRegion__12314.end <= endIndex__12222)) && !composingWithinCurrentTextRange);
                    if (textPointerWithinComposingRegion__12623)
                    {
                        Spell_checkLibrary._addComposingRegionTextSpans(textSpanTreeChildren__12125, text__12277, textPointer__12169, composingRegion__12314, style, composingTextStyle__12367);
                        textSpanTreeChildren__12125.Add(new global::Doroti.Generated.Framework.Painting.TextSpan(style: style, text: text__12277.substring(composingRegion__12314.end, endIndex__12222)));
                    }
                    else
                    {
                        textSpanTreeChildren__12125.Add(new global::Doroti.Generated.Framework.Painting.TextSpan(style: style, text: text__12277.substring(textPointer__12169, endIndex__12222)));
                    }
                    textPointer__12169 = endIndex__12222;
                }
                else
                {
                    endIndex__12222 = ((((global::Doroti.Generated.Framework.Services.SuggestionSpan)currentSpan__12249).range.end < text__12277.Length) ? ((global::Doroti.Generated.Framework.Services.SuggestionSpan)currentSpan__12249).range.end : text__12277.Length);
                    currentSpanIsComposingRegion__12671 = (((textPointer__12169 >= composingRegion__12314.start) && (endIndex__12222 <= composingRegion__12314.end)) && !composingWithinCurrentTextRange);
                    textSpanTreeChildren__12125.Add(new global::Doroti.Generated.Framework.Painting.TextSpan(style: (currentSpanIsComposingRegion__12671 ? composingTextStyle__12367 : misspelledJointStyle__12544), text: text__12277.substring(((global::Doroti.Generated.Framework.Services.SuggestionSpan)currentSpan__12249).range.start, endIndex__12222)));
                    textPointer__12169 = endIndex__12222;
                    currentSpanPointer__12192++;
                }
            }
        }
        if ((textPointer__12169 < text__12277.Length))
        {
            if (((textPointer__12169 < composingRegion__12314.start) && !composingWithinCurrentTextRange))
            {
                Spell_checkLibrary._addComposingRegionTextSpans(textSpanTreeChildren__12125, text__12277, textPointer__12169, composingRegion__12314, style, composingTextStyle__12367);
                if ((composingRegion__12314.end != text__12277.Length))
                {
                    textSpanTreeChildren__12125.Add(new global::Doroti.Generated.Framework.Painting.TextSpan(style: style, text: text__12277.substring(composingRegion__12314.end, text__12277.Length)));
                }
            }
            else
            {
                textSpanTreeChildren__12125.Add(new global::Doroti.Generated.Framework.Painting.TextSpan(style: style, text: text__12277.substring(textPointer__12169, text__12277.Length)));
            }
        }
        return textSpanTreeChildren__12125;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Spell_checkLibrary
{
    internal static void _addComposingRegionTextSpans(List<global::Doroti.Generated.Framework.Painting.TextSpan> treeChildren, string text, long start, TextRange composingRegion, global::Doroti.Generated.Framework.Painting.TextStyle? style, global::Doroti.Generated.Framework.Painting.TextStyle composingTextStyle)
    {
        treeChildren.Add(new global::Doroti.Generated.Framework.Painting.TextSpan(style: style, text: text.substring(start, composingRegion.start)));
        treeChildren.Add(new global::Doroti.Generated.Framework.Painting.TextSpan(style: composingTextStyle, text: text.substring(composingRegion.start, composingRegion.end)));
    }
}

