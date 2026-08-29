#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/text_input.dart
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

public enum SmartDashesType
{
    disabled,
    enabled
}

public enum SmartQuotesType
{
    disabled,
    enabled
}

public class TextInputType
{
    public virtual long index { get; private set; } = default!;
    public virtual bool? signed { get; private set; }
    public virtual bool? @decimal { get; private set; }
    public static TextInputType text = new TextInputType(0L);
    public static TextInputType multiline = new TextInputType(1L);
    public static TextInputType number = TextInputType.CreateNumberWithOptions();
    public static TextInputType phone = new TextInputType(3L);
    public static TextInputType datetime = new TextInputType(4L);
    public static TextInputType emailAddress = new TextInputType(5L);
    public static TextInputType url = new TextInputType(6L);
    public static TextInputType visiblePassword = new TextInputType(7L);
    public static TextInputType name = new TextInputType(8L);
    public static TextInputType streetAddress = new TextInputType(9L);
    public static TextInputType none = new TextInputType(10L);
    public static TextInputType webSearch = new TextInputType(11L);
    public static TextInputType twitter = new TextInputType(12L);
    public static List<TextInputType> values = new List<TextInputType> { text, multiline, number, phone, datetime, emailAddress, url, visiblePassword, name, streetAddress, none, webSearch, twitter };
    internal static List<string> _names = new List<string> { "text", "multiline", "number", "phone", "datetime", "emailAddress", "url", "visiblePassword", "name", "address", "none", "webSearch", "twitter" };

    public TextInputType(long index)
    {
        this.index = index;
        this.signed = null;
        this.@decimal = null;
    }

    public static TextInputType CreateNumberWithOptions(bool? signed = false, bool? @decimal = false)
    {
        var __instance = new TextInputType(default!);
        __instance.index = 2L;
        return __instance;
    }

    internal virtual string _name => $"TextInputType.{_names[(int)(index)]}";
    public virtual DartMap<string, object> toJson()
    {
        return new DartMap<string, object> { ["name"] = _name, ["signed"] = signed, ["decimal"] = @decimal };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "TextInputType"))}(" + $"name: {_name}, " + $"signed: {signed}, " + $"decimal: {@decimal})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as TextInputType;
        if (__other is null) return false;
        return ((((__other is TextInputType) && (FoundationRuntimePorts.EnumIndex(__other) == index)) && (((TextInputType)__other).signed == signed)) && (((TextInputType)__other).@decimal == @decimal));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(index, signed, @decimal);
}

public enum TextInputAction
{
    none,
    unspecified,
    done,
    go,
    search,
    send,
    next,
    previous,
    continueAction,
    join,
    route,
    emergencyCall,
    newline
}

public enum TextCapitalization
{
    words,
    sentences,
    characters,
    none
}

public class TextInputConfiguration
{
    public virtual long? viewId { get; private set; }
    public virtual TextInputType inputType { get; private set; } = default!;
    public virtual bool readOnly { get; private set; } = default!;
    public virtual bool obscureText { get; private set; } = default!;
    public virtual bool autocorrect { get; private set; } = default!;
    public virtual AutofillConfiguration autofillConfiguration { get; private set; } = default!;
    public virtual SmartDashesType smartDashesType { get; private set; } = default!;
    public virtual SmartQuotesType smartQuotesType { get; private set; } = default!;
    public virtual bool enableSuggestions { get; private set; } = default!;
    public virtual bool enableInteractiveSelection { get; private set; } = default!;
    public virtual string? actionLabel { get; private set; }
    public virtual TextInputAction inputAction { get; private set; } = default!;
    public virtual TextCapitalization textCapitalization { get; private set; } = default!;
    public virtual Brightness keyboardAppearance { get; private set; } = default!;
    public virtual bool enableIMEPersonalizedLearning { get; private set; } = default!;
    public virtual List<string> allowedMimeTypes { get; private set; } = default!;
    public virtual List<Locale>? hintLocales { get; private set; }
    public virtual bool? enableInlinePrediction { get; private set; }
    public virtual bool enableDeltaModel { get; private set; } = default!;

    public TextInputConfiguration(long? viewId = null, TextInputType inputType = default!, bool readOnly = false, bool obscureText = false, bool autocorrect = true, SmartDashesType? smartDashesType = null, SmartQuotesType? smartQuotesType = null, bool enableSuggestions = true, bool enableInteractiveSelection = true, string? actionLabel = null, TextInputAction inputAction = TextInputAction.done, Brightness keyboardAppearance = Brightness.light, TextCapitalization textCapitalization = TextCapitalization.none, AutofillConfiguration autofillConfiguration = default!, bool enableIMEPersonalizedLearning = true, List<string> allowedMimeTypes = default!, bool enableDeltaModel = false, List<Locale>? hintLocales = default!, bool? enableInlinePrediction = null)
    {
        this.viewId = viewId;
        this.inputType = inputType;
        this.readOnly = readOnly;
        this.obscureText = obscureText;
        this.autocorrect = autocorrect;
        this.enableSuggestions = enableSuggestions;
        this.enableInteractiveSelection = enableInteractiveSelection;
        this.actionLabel = actionLabel;
        this.inputAction = inputAction;
        this.keyboardAppearance = keyboardAppearance;
        this.textCapitalization = textCapitalization;
        this.autofillConfiguration = autofillConfiguration;
        this.enableIMEPersonalizedLearning = enableIMEPersonalizedLearning;
        this.allowedMimeTypes = allowedMimeTypes;
        this.enableDeltaModel = enableDeltaModel;
        this.hintLocales = hintLocales;
        this.enableInlinePrediction = enableInlinePrediction;
        this.smartDashesType = (smartDashesType ?? ((obscureText ? SmartDashesType.disabled : SmartDashesType.enabled)));
        this.smartQuotesType = (smartQuotesType ?? ((obscureText ? SmartQuotesType.disabled : SmartQuotesType.enabled)));
    }

    public virtual TextInputConfiguration copyWith(long? viewId = null, TextInputType? inputType = null, bool? readOnly = null, bool? obscureText = null, bool? autocorrect = null, SmartDashesType? smartDashesType = null, SmartQuotesType? smartQuotesType = null, bool? enableSuggestions = null, bool? enableInteractiveSelection = null, string? actionLabel = null, TextInputAction? inputAction = null, Brightness? keyboardAppearance = null, TextCapitalization? textCapitalization = null, bool? enableIMEPersonalizedLearning = null, List<string>? allowedMimeTypes = null, AutofillConfiguration? autofillConfiguration = null, bool? enableDeltaModel = null, List<Locale>? hintLocales = null, bool? enableInlinePrediction = null)
    {
        return new TextInputConfiguration(viewId: (viewId ?? this.viewId), inputType: (inputType ?? this.inputType), readOnly: (readOnly ?? this.readOnly), obscureText: (obscureText ?? this.obscureText), autocorrect: (autocorrect ?? this.autocorrect), smartDashesType: (smartDashesType ?? this.smartDashesType), smartQuotesType: (smartQuotesType ?? this.smartQuotesType), enableSuggestions: (enableSuggestions ?? this.enableSuggestions), enableInteractiveSelection: (enableInteractiveSelection ?? this.enableInteractiveSelection), actionLabel: (actionLabel ?? this.actionLabel), inputAction: (inputAction ?? this.inputAction), textCapitalization: (textCapitalization ?? this.textCapitalization), keyboardAppearance: (keyboardAppearance ?? this.keyboardAppearance), enableIMEPersonalizedLearning: (enableIMEPersonalizedLearning ?? this.enableIMEPersonalizedLearning), allowedMimeTypes: (allowedMimeTypes ?? this.allowedMimeTypes), autofillConfiguration: (autofillConfiguration ?? this.autofillConfiguration), enableDeltaModel: (enableDeltaModel ?? this.enableDeltaModel), hintLocales: (hintLocales ?? this.hintLocales), enableInlinePrediction: (enableInlinePrediction ?? this.enableInlinePrediction));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DartMap<string, object> toJson()
    {
        DartMap<string, object>? autofill = autofillConfiguration.toJson();
        return new DartMap<string, object> { ["viewId"] = viewId, ["inputType"] = inputType.toJson(), ["readOnly"] = readOnly, ["obscureText"] = obscureText, ["autocorrect"] = autocorrect, ["smartDashesType"] = FoundationRuntimePorts.EnumIndex(smartDashesType).ToString(), ["smartQuotesType"] = FoundationRuntimePorts.EnumIndex(smartQuotesType).ToString(), ["enableSuggestions"] = enableSuggestions, ["enableInteractiveSelection"] = enableInteractiveSelection, ["actionLabel"] = actionLabel, ["inputAction"] = inputAction.ToString(), ["textCapitalization"] = textCapitalization.ToString(), ["keyboardAppearance"] = keyboardAppearance.ToString(), ["enableIMEPersonalizedLearning"] = enableIMEPersonalizedLearning, ["contentCommitMimeTypes"] = allowedMimeTypes, ["autofill"] = autofill, ["enableDeltaModel"] = enableDeltaModel, ["hintLocales"] = hintLocales.map(((locale) => locale.toLanguageTag())).ToList(), ["enableInlinePrediction"] = enableInlinePrediction };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as TextInputConfiguration;
        if (__other is null) return false;
        if (ReferenceEquals(this, __other))
        {
            return true;
        }
        if ((!object.Equals(__other.GetType(), this.GetType())))
        {
            return false;
        }
        return ((((((((((((((((((((__other is TextInputConfiguration) && (((TextInputConfiguration)__other).viewId == viewId)) && (object.Equals(((TextInputConfiguration)__other).inputType, inputType))) && (((TextInputConfiguration)__other).readOnly == readOnly)) && (((TextInputConfiguration)__other).obscureText == obscureText)) && (((TextInputConfiguration)__other).autocorrect == autocorrect)) && (object.Equals(((TextInputConfiguration)__other).smartDashesType, smartDashesType))) && (object.Equals(((TextInputConfiguration)__other).smartQuotesType, smartQuotesType))) && (((TextInputConfiguration)__other).enableSuggestions == enableSuggestions)) && (((TextInputConfiguration)__other).enableInteractiveSelection == enableInteractiveSelection)) && (((TextInputConfiguration)__other).actionLabel == actionLabel)) && (object.Equals(((TextInputConfiguration)__other).inputAction, inputAction))) && (object.Equals(((TextInputConfiguration)__other).keyboardAppearance, keyboardAppearance))) && (object.Equals(((TextInputConfiguration)__other).textCapitalization, textCapitalization))) && (object.Equals(((TextInputConfiguration)__other).autofillConfiguration, autofillConfiguration))) && (((TextInputConfiguration)__other).enableIMEPersonalizedLearning == enableIMEPersonalizedLearning)) && global::Doroti.Framework.Foundation.CollectionsLibrary.listEquals(((TextInputConfiguration)__other).allowedMimeTypes, allowedMimeTypes)) && (((TextInputConfiguration)__other).enableDeltaModel == enableDeltaModel)) && (object.Equals(((TextInputConfiguration)__other).hintLocales, hintLocales))) && (((TextInputConfiguration)__other).enableInlinePrediction == enableInlinePrediction));
    }

    public override int GetHashCode()
    {
        return FoundationRuntimePorts.ObjectHash(viewId, inputType, readOnly, obscureText, autocorrect, smartDashesType, smartQuotesType, enableSuggestions, enableInteractiveSelection, actionLabel, inputAction, keyboardAppearance, textCapitalization, autofillConfiguration, enableIMEPersonalizedLearning, FoundationRuntimePorts.ObjectHashAll(allowedMimeTypes), enableDeltaModel, hintLocales, enableInlinePrediction);
    }
    public override string ToString()
    {
        var description = new List<string> { $"inputType: {inputType}", $"readOnly: {readOnly}", $"obscureText: {obscureText}", $"autocorrect: {autocorrect}", $"smartDashesType: {smartDashesType}", $"smartQuotesType: {smartQuotesType}", $"enableSuggestions: {enableSuggestions}", $"enableInteractiveSelection: {enableInteractiveSelection}", $"inputAction: {inputAction}", $"keyboardAppearance: {keyboardAppearance}", $"textCapitalization: {textCapitalization}", $"autofillConfiguration: {autofillConfiguration}", $"enableIMEPersonalizedLearning: {enableIMEPersonalizedLearning}", $"allowedMimeTypes: {allowedMimeTypes}", $"enableDeltaModel: {enableDeltaModel}" };
        return $"TextInputConfiguration({string.Join(", ", description)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Text_inputLibrary
{
    internal static TextAffinity? _toTextAffinity(string? affinity)
    {
        return (affinity switch { var __case35229 when object.Equals(__case35229, "TextAffinity.downstream") => TextAffinity.downstream, var __case35287 when object.Equals(__case35287, "TextAffinity.upstream") => TextAffinity.upstream, _ => null });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public enum FloatingCursorDragState
{
    Start,
    Update,
    End
}

public class RawFloatingCursorPoint
{
    public virtual Offset? offset { get; private set; }
    public virtual (Offset, TextPosition)? startLocation { get; private set; }
    public virtual FloatingCursorDragState state { get; private set; } = default!;

    public RawFloatingCursorPoint(Offset? offset = null, (Offset, TextPosition)? startLocation = null, FloatingCursorDragState state = default!)
    {
        this.offset = offset;
        this.startLocation = startLocation;
        this.state = state;
        System.Diagnostics.Debug.Assert(((!object.Equals(state, FloatingCursorDragState.Update)) || (offset is not null)));
    }

}

public class TextEditingValue
{
    public virtual string text { get; private set; } = default!;
    public virtual TextSelection selection { get; private set; } = default!;
    public virtual TextRange composing { get; private set; } = default!;
    public static TextEditingValue empty = new TextEditingValue();

    public TextEditingValue(string text = "", TextSelection? selection = null, TextRange? composing = null)
    {
        this.text = text;
        this.selection = selection ?? TextSelection.CreateCollapsed(-1);
        this.composing = composing ?? TextRange.empty;
    }

    public static TextEditingValue CreateFromJSON(DartMap<string, object> encoded)
    {
        var text = ((string?)encoded.GetValueOrDefault("text"))!;
        var selection = new TextSelection(baseOffset: (((long?)encoded.GetValueOrDefault("selectionBase")) ?? -1L), extentOffset: (((long?)encoded.GetValueOrDefault("selectionExtent")) ?? -1L), affinity: (Text_inputLibrary._toTextAffinity(((string?)encoded.GetValueOrDefault("selectionAffinity"))!) ?? TextAffinity.downstream), isDirectional: (((bool?)encoded.GetValueOrDefault("selectionIsDirectional")) ?? false));
        var composing = new global::Doroti.Ui.TextRange(start: (((long?)encoded.GetValueOrDefault("composingBase")) ?? -1L), end: (((long?)encoded.GetValueOrDefault("composingExtent")) ?? -1L));
        DartRuntimePrimitives.Assert(() => _textRangeIsValid(selection, text));
        DartRuntimePrimitives.Assert(() => _textRangeIsValid(composing, text));
        return new TextEditingValue(text: text, selection: selection, composing: composing);
    }

    public virtual TextEditingValue copyWith(string? text = null, TextSelection? selection = null, TextRange? composing = null)
    {
        return new TextEditingValue(text: (text ?? this.text), selection: (selection ?? this.selection), composing: (composing ?? this.composing));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool isComposingRangeValid => ((composing.isValid && composing.isNormalized) && (composing.end <= text.Length));
    public virtual TextEditingValue replaced(TextRange replacementRange, string replacementString)
    {
        if (!replacementRange.isValid)
        {
            return this;
        }
        string newText = text.replaceRange(replacementRange.start, replacementRange.end, replacementString);
        if (((replacementRange.end - replacementRange.start) == replacementString.Length))
        {
            return copyWith(text: newText);
        }
        long adjustIndex(long originalIndex)
        {
            long replacedLength = (((originalIndex <= replacementRange.start) && (originalIndex < replacementRange.end)) ? 0L : replacementString.Length);
            long removedLength = (originalIndex.clamp(replacementRange.start, replacementRange.end) - replacementRange.start);
            return ((originalIndex + replacedLength) - removedLength);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        var adjustedSelection = new TextSelection(baseOffset: adjustIndex(selection.baseOffset), extentOffset: adjustIndex(selection.extentOffset));
        var adjustedComposing = new global::Doroti.Ui.TextRange(start: adjustIndex(composing.start), end: adjustIndex(composing.end));
        DartRuntimePrimitives.Assert(() => _textRangeIsValid(adjustedSelection, newText));
        DartRuntimePrimitives.Assert(() => _textRangeIsValid(adjustedComposing, newText));
        return new TextEditingValue(text: newText, selection: adjustedSelection, composing: adjustedComposing);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DartMap<string, object> toJSON()
    {
        DartRuntimePrimitives.Assert(() => _textRangeIsValid(selection, text));
        DartRuntimePrimitives.Assert(() => _textRangeIsValid(composing, text));
        return new DartMap<string, object> { ["text"] = text, ["selectionBase"] = selection.baseOffset, ["selectionExtent"] = selection.extentOffset, ["selectionAffinity"] = selection.affinity.ToString(), ["selectionIsDirectional"] = selection.isDirectional, ["composingBase"] = composing.start, ["composingExtent"] = composing.end };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "TextEditingValue"))}(text: ┤{text}├, selection: {selection}, composing: {composing})";
    public override bool Equals(object? other)
    {
        var __other = other as TextEditingValue;
        if (__other is null) return false;
        if (ReferenceEquals(this, __other))
        {
            return true;
        }
        return ((((__other is TextEditingValue) && (((TextEditingValue)__other).text == text)) && (object.Equals(((TextEditingValue)__other).selection, selection))) && (object.Equals(((TextEditingValue)__other).composing, composing)));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(text.GetHashCode(), selection.GetHashCode(), composing.GetHashCode());
    internal static bool _textRangeIsValid(TextRange range, string text)
    {
        if (((range.start == -1L) && (range.end == -1L)))
        {
            return true;
        }
        DartRuntimePrimitives.Assert(() => ((range.start >= 0L) && (range.start <= text.Length)));
        DartRuntimePrimitives.Assert(() => ((range.end >= 0L) && (range.end <= text.Length)));
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public enum SelectionChangedCause
{
    tap,
    doubleTap,
    longPress,
    forcePress,
    keyboard,
    toolbar,
    drag,
    stylusHandwriting
}

public interface TextSelectionDelegate
{
    public TextEditingValue textEditingValue { get; }
    public void userUpdateTextEditingValue(TextEditingValue value, SelectionChangedCause cause);
    public void hideToolbar(bool hideHandles = true);
    public void bringIntoView(TextPosition position);
    public bool cutEnabled => true;
    public bool copyEnabled => true;
    public bool pasteEnabled => true;
    public bool selectAllEnabled => true;
    public bool lookUpEnabled => true;
    public bool searchWebEnabled => true;
    public bool shareEnabled => true;
    public bool liveTextInputEnabled => false;
    public void cutSelection(SelectionChangedCause cause);
    public Future pasteText(SelectionChangedCause cause);
    public void selectAll(SelectionChangedCause cause);
    public void copySelection(SelectionChangedCause cause);
}

public interface TextInputClient
{
    public TextEditingValue? currentTextEditingValue { get; }
    public AutofillScope? currentAutofillScope { get; }
    public void updateEditingValue(TextEditingValue value);
    public void performAction(TextInputAction action);
    public void insertContent(KeyboardInsertedContent content)
    {
    }

    public void performPrivateCommand(string action, DartMap<string, object> data);
    public void updateFloatingCursor(RawFloatingCursorPoint point);
    public void showAutocorrectionPromptRect(long start, long end);
    public bool onFocusReceived() => false;
    public void connectionClosed();
    public void didChangeInputControl(TextInputControl? oldControl, TextInputControl? newControl)
    {
    }

    public void showToolbar()
    {
    }

    public void insertTextPlaceholder(Size size)
    {
    }

    public void removeTextPlaceholder()
    {
    }

    public void performSelector(string selectorName)
    {
    }

}

public interface ScribbleClient
{
    public string elementIdentifier { get; }
    public void onScribbleFocus(Offset offset);
    public bool isInScribbleRect(Rect rect);
    public Rect bounds { get; }
}

public class SelectionRect
{
    public virtual long position { get; private set; } = default!;
    public virtual Rect bounds { get; private set; } = default!;
    public virtual TextDirection direction { get; private set; } = default!;

    public SelectionRect(long position, Rect bounds, TextDirection direction = TextDirection.ltr)
    {
        this.position = position;
        this.bounds = bounds;
        this.direction = direction;
    }

    public override bool Equals(object? other)
    {
        var __other = other as SelectionRect;
        if (__other is null) return false;
        if (ReferenceEquals(this, __other))
        {
            return true;
        }
        if ((!object.Equals(this.GetType(), __other.GetType())))
        {
            return false;
        }
        return ((((__other is SelectionRect) && (((SelectionRect)__other).position == position)) && (object.Equals(((SelectionRect)__other).bounds, bounds))) && (object.Equals(((SelectionRect)__other).direction, direction)));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(position, bounds);
    public override string ToString() => $"SelectionRect({position}, {bounds})";
}

public interface DeltaTextInputClient : TextInputClient
{
    public void updateEditingValueWithDeltas(List<TextEditingDelta> textEditingDeltas);
}

public class TextInputStyle : Diagnosticable
{
    public virtual string? fontFamily { get; private set; }
    public virtual double? fontSize { get; private set; }
    public virtual FontWeight? fontWeight { get; private set; }
    public virtual TextDirection textDirection { get; private set; } = default!;
    public virtual TextAlign textAlign { get; private set; } = default!;
    public virtual double? letterSpacing { get; private set; }
    public virtual double? wordSpacing { get; private set; }
    public virtual double? lineHeight { get; private set; }

    public TextInputStyle(string? fontFamily = null, double? fontSize = null, FontWeight? fontWeight = null, TextDirection textDirection = default!, TextAlign textAlign = default!, double? letterSpacing = null, double? wordSpacing = null, double? lineHeight = null)
    {
        this.fontFamily = fontFamily;
        this.fontSize = fontSize;
        this.fontWeight = fontWeight;
        this.textDirection = textDirection;
        this.textAlign = textAlign;
        this.letterSpacing = letterSpacing;
        this.wordSpacing = wordSpacing;
        this.lineHeight = lineHeight;
    }

    public override bool Equals(object? other)
    {
        var __other = other as TextInputStyle;
        if (__other is null) return false;
        if (ReferenceEquals(this, __other))
        {
            return true;
        }
        if ((!object.Equals(__other.GetType(), this.GetType())))
        {
            return false;
        }
        return (((((((((__other is TextInputStyle) && (((TextInputStyle)__other).fontFamily == fontFamily)) && (((TextInputStyle)__other).fontSize == fontSize)) && (object.Equals(((TextInputStyle)__other).fontWeight, fontWeight))) && (object.Equals(((TextInputStyle)__other).textDirection, textDirection))) && (object.Equals(((TextInputStyle)__other).textAlign, textAlign))) && (((TextInputStyle)__other).letterSpacing == letterSpacing)) && (((TextInputStyle)__other).wordSpacing == wordSpacing)) && (((TextInputStyle)__other).lineHeight == lineHeight));
    }

    public override int GetHashCode()
    {
        return FoundationRuntimePorts.ObjectHash(fontFamily, fontSize, fontWeight, textDirection, textAlign, letterSpacing, wordSpacing, lineHeight);
    }
    public virtual DartMap<string, object> toJson()
    {
        return new DartMap<string, object> { ["fontFamily"] = fontFamily, ["fontSize"] = fontSize, ["fontWeightIndex"] = FoundationRuntimePorts.EnumIndex(fontWeight), ["textAlignIndex"] = FoundationRuntimePorts.EnumIndex(textAlign), ["textDirectionIndex"] = FoundationRuntimePorts.EnumIndex(textDirection), ["letterSpacing"] = letterSpacing, ["wordSpacing"] = wordSpacing, ["lineHeight"] = lineHeight };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.Add(new StringProperty("fontFamily", fontFamily, defaultValue: null));
        properties.Add(new DoubleProperty("fontSize", fontSize, defaultValue: null));
        properties.Add(new DiagnosticsProperty<global::Doroti.Ui.FontWeight>("fontWeight", fontWeight, defaultValue: null));
        properties.Add(new EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", textDirection));
        properties.Add(new EnumProperty<global::Doroti.Ui.TextAlign>("textAlign", textAlign));
        properties.Add(new DoubleProperty("letterSpacing", letterSpacing, defaultValue: null));
        properties.Add(new DoubleProperty("wordSpacing", wordSpacing, defaultValue: null));
        properties.Add(new DoubleProperty("lineHeight", lineHeight, defaultValue: null));
    }

}

public class TextInputConnection
{
    internal virtual Size? _cachedSize { get; set; } = default;
    internal virtual Matrix4? _cachedTransform { get; set; } = default;
    internal virtual Rect? _cachedRect { get; set; } = default;
    internal virtual Rect? _cachedCaretRect { get; set; } = default;
    internal virtual List<SelectionRect> _cachedSelectionRects { get; set; } = new List<SelectionRect>();
    internal static long _nextId = 1L;
    internal virtual long _id { get; private set; } = default!;
    internal virtual TextInputClient _client { get; private set; } = default!;

    public TextInputConnection(TextInputClient _client)
    {
        this._client = _client;
        this._id = _nextId++;
    }

    public static void debugResetId(long to = 1)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                _nextId = to;
                return true;
            });
    }

    public virtual bool attached => (object.Equals(TextInput._instance._currentConnection, this));
    public virtual bool scribbleInProgress => TextInput._instance.scribbleInProgress;
    public virtual void show()
    {
        DartRuntimePrimitives.Assert(() => attached);
        TextInput._instance._show();
    }

    public virtual void requestAutofill()
    {
        DartRuntimePrimitives.Assert(() => attached);
        TextInput._instance._requestAutofill();
    }

    public virtual void updateConfig(TextInputConfiguration configuration)
    {
        DartRuntimePrimitives.Assert(() => attached);
        TextInput._instance._updateConfig(configuration);
    }

    public virtual void setEditingState(TextEditingValue value)
    {
        DartRuntimePrimitives.Assert(() => attached);
        TextInput._instance._setEditingState(value);
    }

    public virtual void setEditableSizeAndTransform(Size editableBoxSize, Matrix4 transform)
    {
        if (((!object.Equals(editableBoxSize, _cachedSize)) || (!object.Equals(transform, _cachedTransform))))
        {
            _cachedSize = editableBoxSize;
            _cachedTransform = transform;
            TextInput._instance._setEditableSizeAndTransform(editableBoxSize, transform);
        }
    }

    public virtual void setComposingRect(Rect rect)
    {
        if ((object.Equals(rect, _cachedRect)))
        {
            return;
        }
        _cachedRect = rect;
        global::Doroti.Ui.Rect validRect = (rect.isFinite ? rect : (Offset.zero & new global::Doroti.Ui.Size(-1, -1)));
        TextInput._instance._setComposingTextRect(validRect);
    }

    public virtual void setCaretRect(Rect rect)
    {
        if ((object.Equals(rect, _cachedCaretRect)))
        {
            return;
        }
        _cachedCaretRect = rect;
        global::Doroti.Ui.Rect validRect = (rect.isFinite ? rect : (Offset.zero & new global::Doroti.Ui.Size(-1, -1)));
        TextInput._instance._setCaretRect(validRect);
    }

    public virtual void setSelectionRects(List<SelectionRect> selectionRects)
    {
        if (!global::Doroti.Framework.Foundation.CollectionsLibrary.listEquals(_cachedSelectionRects, selectionRects))
        {
            _cachedSelectionRects = selectionRects;
            TextInput._instance._setSelectionRects(selectionRects);
        }
    }

    public virtual void setStyle(string? fontFamily, double? fontSize, FontWeight? fontWeight, TextDirection textDirection, TextAlign textAlign)
    {
        updateStyle(new TextInputStyle(fontFamily: fontFamily, fontSize: fontSize, fontWeight: fontWeight, textDirection: textDirection, textAlign: textAlign));
    }

    public virtual void updateStyle(TextInputStyle style)
    {
        DartRuntimePrimitives.Assert(() => attached);
        TextInput._instance._updateStyle(style);
    }

    public virtual void close()
    {
        if (attached)
        {
            TextInput._instance._clearClient();
        }
        DartRuntimePrimitives.Assert(() => !attached);
    }

    public virtual void connectionClosedReceived()
    {
        TextInput._instance._currentConnection = null;
        DartRuntimePrimitives.Assert(() => !attached);
    }

}

public static partial class Text_inputLibrary
{
    internal static TextInputAction _toTextInputAction(string action)
    {
        return (action switch { var __case70013 when object.Equals(__case70013, "TextInputAction.none") => TextInputAction.none, var __case70065 when object.Equals(__case70065, "TextInputAction.unspecified") => TextInputAction.unspecified, var __case70131 when object.Equals(__case70131, "TextInputAction.go") => TextInputAction.go, var __case70179 when object.Equals(__case70179, "TextInputAction.search") => TextInputAction.search, var __case70235 when object.Equals(__case70235, "TextInputAction.send") => TextInputAction.send, var __case70287 when object.Equals(__case70287, "TextInputAction.next") => TextInputAction.next, var __case70339 when object.Equals(__case70339, "TextInputAction.previous") => TextInputAction.previous, var __case70399 when object.Equals(__case70399, "TextInputAction.continueAction") => TextInputAction.continueAction, var __case70471 when object.Equals(__case70471, "TextInputAction.join") => TextInputAction.join, var __case70523 when object.Equals(__case70523, "TextInputAction.route") => TextInputAction.route, var __case70577 when object.Equals(__case70577, "TextInputAction.emergencyCall") => TextInputAction.emergencyCall, var __case70647 when object.Equals(__case70647, "TextInputAction.done") => TextInputAction.done, var __case70699 when object.Equals(__case70699, "TextInputAction.newline") => TextInputAction.newline, _ => throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"Unknown text input action: {action}") }) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Text_inputLibrary
{
    internal static FloatingCursorDragState _toTextCursorAction(string state)
    {
        return (state switch { var __case70974 when object.Equals(__case70974, "FloatingCursorDragState.start") => FloatingCursorDragState.Start, var __case71044 when object.Equals(__case71044, "FloatingCursorDragState.update") => FloatingCursorDragState.Update, var __case71116 when object.Equals(__case71116, "FloatingCursorDragState.end") => FloatingCursorDragState.End, _ => throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"Unknown text cursor action: {state}") }) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Text_inputLibrary
{
    internal static RawFloatingCursorPoint _toTextPoint(FloatingCursorDragState state, DartMap<string, object> encoded)
    {
        DartRuntimePrimitives.Assert(() => (encoded.GetValueOrDefault("X") is not null));
        DartRuntimePrimitives.Assert(() => (encoded.GetValueOrDefault("Y") is not null));
        global::Doroti.Ui.Offset offset = ((object.Equals(state, FloatingCursorDragState.Update)) ? new global::Doroti.Ui.Offset((((double)encoded.GetValueOrDefault("X"))).toDouble(), (((double)encoded.GetValueOrDefault("Y"))).toDouble()) : Offset.zero);
        return new RawFloatingCursorPoint(offset: offset, state: state);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Text_inputLibrary
{
    internal static void _reportError(object exception, StackTrace stack, string context, InformationCollector? informationCollector = null)
    {
        FlutterError.reportError(new FlutterErrorDetails(exception: exception, stack: stack, library: "services library", context: new ErrorDescription(context), informationCollector: informationCollector));
    }
}

public class TextInput
{
    internal static TextInput _instance = new TextInput();

    // Keep the singleton initialization tied to the first explicit TextInput
    // access. Without an explicit type initializer the CLR marks this type as
    // beforefieldinit, so AOT runtimes may construct TextInput before
    // ServicesBinding.initInstances has installed its binary messenger.
    static TextInput()
    {
    }

    internal virtual TextInputControl? _currentControl { get; set; } = _HostTextInputControl.instance;
    internal virtual HashSet<TextInputControl> _inputControls { get; private set; } = new HashSet<TextInputControl> { _HostTextInputControl.instance };
    internal static List<TextInputAction> _androidSupportedInputActions = new List<TextInputAction> { TextInputAction.none, TextInputAction.unspecified, TextInputAction.done, TextInputAction.send, TextInputAction.go, TextInputAction.search, TextInputAction.next, TextInputAction.previous, TextInputAction.newline };
    internal static List<TextInputAction> _iOSSupportedInputActions = new List<TextInputAction> { TextInputAction.unspecified, TextInputAction.done, TextInputAction.send, TextInputAction.go, TextInputAction.search, TextInputAction.next, TextInputAction.newline, TextInputAction.continueAction, TextInputAction.join, TextInputAction.route, TextInputAction.emergencyCall };
    internal virtual MethodChannel _channel { get; set; } = default!;
    internal virtual TextInputConnection? _currentConnection { get; set; } = default;
    internal virtual TextInputConfiguration _currentConfiguration { get; set; } = default!;
    internal virtual TextInputConnection? _lastConnection { get; set; } = default;
    internal virtual DartMap<string, ScribbleClient> _scribbleClients { get; private set; } = new DartMap<string, ScribbleClient>();
    internal virtual bool _scribbleInProgress { get; set; } = false;
    internal virtual bool _hidePending { get; set; } = false;

    public TextInput()
    {
        _channel = SystemChannels.textInput;
        _channel.setMethodCallHandler(_loudlyHandleTextInputInvocation);
    }

    public static void setChannel(MethodChannel newChannel)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                _instance._channel = ((Func<MethodChannel>)(() =>
{
    var __cascade = newChannel;
    __cascade.setMethodCallHandler(_instance._loudlyHandleTextInputInvocation);
    return __cascade;
}))();
                return true;
            });
    }

    internal static void _addInputControl(TextInputControl control)
    {
        if ((!object.Equals(control, _HostTextInputControl.instance)))
        {
            _instance._inputControls.Add(control);
        }
    }

    internal static void _removeInputControl(TextInputControl control)
    {
        if ((!object.Equals(control, _HostTextInputControl.instance)))
        {
            _instance._inputControls.Remove(control);
        }
    }

    public static void setInputControl(TextInputControl? newControl)
    {
        TextInputControl? oldControl = _instance._currentControl;
        if ((object.Equals(newControl, oldControl)))
        {
            return;
        }
        if ((newControl is not null))
        {
            _addInputControl(newControl);
        }
        if ((oldControl is not null))
        {
            _removeInputControl(oldControl);
        }
        _instance._currentControl = newControl;
        TextInputClient? client = _instance._currentConnection?._client;
        client?.didChangeInputControl(oldControl, newControl);
    }

    public static void restorePlatformInputControl()
    {
        setInputControl(_HostTextInputControl.instance);
    }

    public static void ensureInitialized()
    {
        _ = _instance;
    }

    public static TextInputConnection attach(TextInputClient client, TextInputConfiguration configuration)
    {
        var connection = new TextInputConnection(client);
        _instance._attach(connection, configuration);
        return connection;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _attach(TextInputConnection connection, TextInputConfiguration configuration)
    {
        DartRuntimePrimitives.Assert(() => _debugEnsureInputActionWorksOnPlatform(configuration.inputAction));
        _currentConnection = connection;
        _currentConfiguration = configuration;
        _lastConnection = connection;
        _setClient(connection._client, configuration);
    }

    internal static bool _debugEnsureInputActionWorksOnPlatform(TextInputAction inputAction)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb)
                {
                    return true;
                }
                if (Platform.isIOS)
                {
                    DartRuntimePrimitives.Assert(() => _iOSSupportedInputActions.Contains(inputAction));
                }
                else
                {
                    if (Platform.isAndroid)
                    {
                        DartRuntimePrimitives.Assert(() => _androidSupportedInputActions.Contains(inputAction));
                    }
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DartMap<string, ScribbleClient> scribbleClients => TextInput._instance._scribbleClients;
    public virtual bool scribbleInProgress => _scribbleInProgress;
    internal async virtual Future<object> _loudlyHandleTextInputInvocation(MethodCall call)
    {
        try
        {
            return await _handleTextInputInvocation(call);
        }
        catch (Exception exception)
        {
            var stack = new System.Diagnostics.StackTrace();
            Text_inputLibrary._reportError(exception, stack, $"during method call {call.method}", (() => new List<DiagnosticsNode> { new DiagnosticsProperty<MethodCall>("call", call, style: DiagnosticsTreeStyle.errorProperty) }));
            throw;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal async virtual Future<object> _handleTextInputInvocation(MethodCall methodCall)
    {
        string method = methodCall.method;
        switch (method)
        {
            case var __case81975 when object.Equals(__case81975, "TextInputClient.focusElement"):
                {
                    var argsLocal = ((List<object>?)methodCall.arguments)!;
                    _scribbleClients.GetValueOrDefault(argsLocal[(int)(0L)])?.onScribbleFocus(new global::Doroti.Ui.Offset((((double)argsLocal[(int)(1L)])).toDouble(), (((double)argsLocal[(int)(2L)])).toDouble()));
                    return default!;
                }
            case var __case82233 when object.Equals(__case82233, "TextInputClient.requestElementsInRect"):
                {
                    List<double> argsAlternate = (((List<object>?)methodCall.arguments)!).cast<double>().map(((value) => value.toDouble())).ToList();
                    return _scribbleClients.Keys.where(((elementIdentifier) =>
                    {
                        var rect = new global::Doroti.Ui.Rect(argsAlternate[(int)(0L)], argsAlternate[(int)(1L)], argsAlternate[(int)(2L)], argsAlternate[(int)(3L)]);
                        if (!((_scribbleClients.GetValueOrDefault(elementIdentifier)?.isInScribbleRect(rect) ?? false)))
                        {
                            return false;
                        }
                        global::Doroti.Ui.Rect boundsLocal = (_scribbleClients.GetValueOrDefault(elementIdentifier)?.bounds ?? Rect.zero);
                        return !((((object.Equals(boundsLocal, Rect.zero)) || boundsLocal.hasNaN) || boundsLocal.isInfinite));
                    })).map(((elementIdentifier) =>
                    {
                        global::Doroti.Ui.Rect boundsLocal = _scribbleClients.GetValueOrDefault(elementIdentifier)!.bounds;
                        return new List<object> { elementIdentifier };
                    })).ToList();
                }
            case var __case83288 when object.Equals(__case83288, "TextInputClient.scribbleInteractionBegan"):
                {
                    _scribbleInProgress = true;
                    return default!;
                }
            case var __case83395 when object.Equals(__case83395, "TextInputClient.scribbleInteractionFinished"):
                {
                    _scribbleInProgress = false;
                    return default!;
                }
            case var __case83506 when object.Equals(__case83506, "TextInputClient.onFocusReceived"):
                {
                    var argsNested = ((List<object>?)methodCall.arguments)!;
                    var clientId = ((long)argsNested[(int)(0L)]);
                    if (((_lastConnection is not null) && (_lastConnection!._id == clientId)))
                    {
                        return _lastConnection!._client.onFocusReceived();
                    }
                    return false;
                }
        }
        if ((_currentConnection is null))
        {
            return default!;
        }
        if ((method == "TextInputClient.requestExistingInputState"))
        {
            _attach(_currentConnection!, _currentConfiguration);
            TextEditingValue? editingValue = _currentConnection!._client.currentTextEditingValue;
            if ((editingValue is not null))
            {
                _setEditingState(editingValue);
            }
            return default!;
        }
        var args = ((List<object>?)methodCall.arguments)!;
        if ((method == "TextInputClient.updateEditingStateWithTag"))
        {
            TextInputClient clientLocal = _currentConnection!._client;
            AutofillScope? scope = clientLocal.currentAutofillScope;
            var editingValueLocal = DartRuntimePrimitives.ConvertMap<string, object>((System.Collections.IDictionary)args[(int)(1L)]);
            foreach (string tag in editingValueLocal.Keys)
            {
                var textEditingValue = TextEditingValue.CreateFromJSON(DartRuntimePrimitives.ConvertMap<string, object>((System.Collections.IDictionary)editingValueLocal.GetValueOrDefault(tag)));
                AutofillClient? clientAlternate = scope?.getAutofillClient(tag);
                if (((clientAlternate is not null) && clientAlternate.textInputConfiguration.autofillConfiguration.enabled))
                {
                    clientAlternate.autofill(textEditingValue);
                }
            }
            return default!;
        }
        var client = ((long)args[(int)(0L)]);
        if ((DartRuntimePrimitives.RequireValue(client) != _currentConnection!._id))
        {
            var debugAllowAnyway = false;
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((DartRuntimePrimitives.RequireValue(client) == -1L))
                    {
                        debugAllowAnyway = true;
                    }
                    return true;
                });
            if (!debugAllowAnyway)
            {
                return default!;
            }
        }
        switch (method)
        {
            case var __case85861 when object.Equals(__case85861, "TextInputClient.updateEditingState"):
                {
                    var valueLocal = TextEditingValue.CreateFromJSON(DartRuntimePrimitives.ConvertMap<string, object>((System.Collections.IDictionary)args[(int)(1L)]));
                    TextInput._instance._updateEditingValue(valueLocal, exclude: _PlatformTextInputControl.instance);
                    break;
                }
            case var __case86093 when object.Equals(__case86093, "TextInputClient.updateEditingStateWithDeltas"):
                {
                    DartRuntimePrimitives.Assert(() => (_currentConnection!._client is DeltaTextInputClient));
                    var encoded = DartRuntimePrimitives.ConvertMap<string, object>((System.Collections.IDictionary)args[(int)(1L)]);
                    var deltas = new List<TextEditingDelta>();
                    (((DeltaTextInputClient?)_currentConnection!._client)!).updateEditingValueWithDeltas(deltas);
                    break;
                }
            case var __case86724 when object.Equals(__case86724, "TextInputClient.performAction"):
                {
                    if ((((string?)args[(int)(1L)])! == "TextInputAction.commitContent"))
                    {
                        var content = KeyboardInsertedContent.CreateFromJson(DartRuntimePrimitives.ConvertMap<string, object>((System.Collections.IDictionary)args[(int)(2L)]));
                        _currentConnection!._client.insertContent(content);
                    }
                    else
                    {
                        _currentConnection!._client.performAction(Text_inputLibrary._toTextInputAction(((string?)args[(int)(1L)])!));
                    }
                    break;
                }
            case var __case87110 when object.Equals(__case87110, "TextInputClient.performSelectors"):
                {
                    List<string> selectors = (((List<object>?)args[(int)(1L)])!).cast<string>().ToList();
                    selectors.forEach(_currentConnection!._client.performSelector);
                    break;
                }
            case var __case87311 when object.Equals(__case87311, "TextInputClient.performPrivateCommand"):
                {
                    var firstArg = DartRuntimePrimitives.ConvertMap<string, object>((System.Collections.IDictionary)args[(int)(1L)]);
                    _currentConnection!._client.performPrivateCommand(((string?)firstArg.GetValueOrDefault("action"))!, ((firstArg.GetValueOrDefault("data") is null) ? new DartMap<string, object>() : DartRuntimePrimitives.ConvertMap<string, object>((System.Collections.IDictionary)firstArg.GetValueOrDefault("data"))));
                    break;
                }
            case var __case87632 when object.Equals(__case87632, "TextInputClient.updateFloatingCursor"):
                {
                    _currentConnection!._client.updateFloatingCursor(Text_inputLibrary._toTextPoint(Text_inputLibrary._toTextCursorAction(((string?)args[(int)(1L)])!), DartRuntimePrimitives.ConvertMap<string, object>((System.Collections.IDictionary)args[(int)(2L)])));
                    break;
                }
            case var __case87849 when object.Equals(__case87849, "TextInputClient.onConnectionClosed"):
                {
                    _currentConnection!._client.connectionClosed();
                    break;
                }
            case var __case87954 when object.Equals(__case87954, "TextInputClient.showAutocorrectionPromptRect"):
                {
                    _currentConnection!._client.showAutocorrectionPromptRect(((long)args[(int)(1L)]), ((long)args[(int)(2L)]));
                    break;
                }
            case var __case88111 when object.Equals(__case88111, "TextInputClient.showToolbar"):
                {
                    _currentConnection!._client.showToolbar();
                    break;
                }
            case var __case88204 when object.Equals(__case88204, "TextInputClient.insertTextPlaceholder"):
                {
                    _currentConnection!._client.insertTextPlaceholder(new global::Doroti.Ui.Size((((double)args[(int)(1L)])).toDouble(), (((double)args[(int)(2L)])).toDouble()));
                    break;
                }
            case var __case88400 when object.Equals(__case88400, "TextInputClient.removeTextPlaceholder"):
                {
                    _currentConnection!._client.removeTextPlaceholder();
                    break;
                }
            default:
                {
                    throw new MissingPluginException();
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _scheduleHide()
    {
        if (_hidePending)
        {
            return;
        }
        _hidePending = true;
        DartAsyncRuntime.scheduleMicrotask((() =>
        {
            _hidePending = false;
            if ((_currentConnection is null))
            {
                _hide();
            }
        }));
    }

    internal virtual void _setClient(TextInputClient client, TextInputConfiguration configuration)
    {
        foreach (TextInputControl control in _inputControls)
        {
            control.attach(client, configuration);
        }
    }

    internal virtual void _clearClient()
    {
        TextInputClient client = _currentConnection!._client;
        foreach (TextInputControl control in _inputControls)
        {
            control.detach(client);
        }
        _currentConnection = null;
        _scheduleHide();
    }

    internal virtual void _updateConfig(TextInputConfiguration configuration)
    {
        foreach (TextInputControl control in _inputControls)
        {
            control.updateConfig(configuration);
        }
    }

    internal virtual void _setEditingState(TextEditingValue value)
    {
        foreach (TextInputControl control in _inputControls)
        {
            control.setEditingState(value);
        }
    }

    internal virtual void _show()
    {
        foreach (TextInputControl control in _inputControls)
        {
            control.show();
        }
    }

    internal virtual void _hide()
    {
        foreach (TextInputControl control in _inputControls)
        {
            control.hide();
        }
    }

    internal virtual void _setEditableSizeAndTransform(Size editableBoxSize, Matrix4 transform)
    {
        foreach (TextInputControl control in _inputControls)
        {
            control.setEditableSizeAndTransform(editableBoxSize, transform);
        }
    }

    internal virtual void _setComposingTextRect(Rect rect)
    {
        foreach (TextInputControl control in _inputControls)
        {
            control.setComposingRect(rect);
        }
    }

    internal virtual void _setCaretRect(Rect rect)
    {
        foreach (TextInputControl control in _inputControls)
        {
            control.setCaretRect(rect);
        }
    }

    internal virtual void _setSelectionRects(List<SelectionRect> selectionRects)
    {
        foreach (TextInputControl control in _inputControls)
        {
            control.setSelectionRects(selectionRects);
        }
    }

    internal virtual void _updateStyle(TextInputStyle style)
    {
        foreach (TextInputControl control in _inputControls)
        {
            control.updateStyle(style);
        }
    }

    internal virtual void _requestAutofill()
    {
        foreach (TextInputControl control in _inputControls)
        {
            control.requestAutofill();
        }
    }

    internal virtual void _updateEditingValue(TextEditingValue value, TextInputControl? exclude = null)
    {
        if ((_currentConnection is null))
        {
            return;
        }
        foreach (TextInputControl control in _instance._inputControls)
        {
            if ((!object.Equals(control, exclude)))
            {
                control.setEditingState(value);
            }
        }
        _instance._currentConnection!._client.updateEditingValue(value);
    }

    public static void updateEditingValue(TextEditingValue value)
    {
        _instance._updateEditingValue(value, exclude: _instance._currentControl);
    }

    public static void finishAutofillContext(bool shouldSave = true)
    {
        foreach (TextInputControl control in TextInput._instance._inputControls)
        {
            control.finishAutofillContext(shouldSave: shouldSave);
        }
    }

    public static void registerScribbleElement(string elementIdentifier, ScribbleClient scribbleClient)
    {
        TextInput._instance._scribbleClients[elementIdentifier] = scribbleClient;
    }

    public static void unregisterScribbleElement(string elementIdentifier)
    {
        TextInput._instance._scribbleClients.remove(elementIdentifier);
    }

}

public abstract class TextInputControl
{
    public virtual void attach(TextInputClient client, TextInputConfiguration configuration)
    {
    }

    public virtual void detach(TextInputClient client)
    {
    }

    public virtual void show()
    {
    }

    public virtual void hide()
    {
    }

    public virtual void updateConfig(TextInputConfiguration configuration)
    {
    }

    public virtual void setEditingState(TextEditingValue value)
    {
    }

    public virtual void setEditableSizeAndTransform(Size editableBoxSize, Matrix4 transform)
    {
    }

    public virtual void setComposingRect(Rect rect)
    {
    }

    public virtual void setCaretRect(Rect rect)
    {
    }

    public virtual void setSelectionRects(List<SelectionRect> selectionRects)
    {
    }

    public virtual void setStyle(string? fontFamily, double? fontSize, FontWeight? fontWeight, TextDirection textDirection, TextAlign textAlign)
    {
    }

    public virtual void updateStyle(TextInputStyle style)
    {
    }

    public virtual void requestAutofill()
    {
    }

    public virtual void finishAutofillContext(bool shouldSave = true)
    {
    }

}

/// <summary>
/// Host-neutral bridge from Flutter Services text editing policy to the active
/// view's native IME capability. No concrete platform type crosses
/// this framework boundary.
/// </summary>
internal sealed class _HostTextInputControl : TextInputControl
{
    public static _HostTextInputControl instance { get; } = new();

    private ITextInputHostCapability? _capability;
    private TextInputClient? _client;
    private DorotiView? _view;

    private _HostTextInputControl()
    {
    }

    public override void attach(TextInputClient client, TextInputConfiguration configuration)
    {
        DetachCurrent(clearHost: true);
        var dispatcher = PlatformDispatcher.instance;
        var view = configuration.viewId is { } configuredViewId
            ? dispatcher.view(configuredViewId)
            : dispatcher.implicitView;
        if (view is null)
        {
            throw new DorotiCapabilityException(
                DorotiCapabilityIds.TextInput,
                configuration.viewId is { } id ? checked((ulong)id) : null,
                DartUiInvocation.Managed("package:flutter/services.dart#TextInput.attach"),
                "no matching Flutter view is registered");
        }

        _client = client;
        _view = view;
        _capability = view.RequireCapability<ITextInputHostCapability>(
            DorotiCapabilityIds.TextInput,
            DartUiInvocation.Managed("package:flutter/services.dart#TextInput.attach"));
        _capability.EditingStateChanged += OnEditingStateChanged;
        _capability.ActionPerformed += OnActionPerformed;
        _capability.ConnectionClosed += OnConnectionClosed;
        _capability.SetClient(ToHost(configuration), ToHost(client.currentTextEditingValue ?? TextEditingValue.empty));
    }

    public override void detach(TextInputClient client)
    {
        if (ReferenceEquals(_client, client))
        {
            DetachCurrent(clearHost: true);
        }
    }

    public override void setEditingState(TextEditingValue value) =>
        RequireCapability("TextInput.setEditingState").UpdateState(ToHost(value));

    public override void show() => RequireCapability("TextInput.show").ShowTextInput();

    // TextInput schedules hide after detaching the final client. Native hosts
    // hide as part of ClearClient, so a later scheduled hide must tolerate the
    // capability already having been released.
    public override void hide() => _capability?.HideTextInput();

    public override void setCaretRect(Rect rect) =>
        RequireCapability("TextInput.setCaretRect").SetCaretRect(rect);

    private ITextInputHostCapability RequireCapability(string elementId) =>
        _capability ?? throw new DorotiCapabilityException(
            DorotiCapabilityIds.TextInput,
            null,
            DartUiInvocation.Managed($"package:flutter/services.dart#{elementId}"),
            "no text input client is attached");

    private void OnEditingStateChanged(DorotiTextEditingState state)
    {
        if (_client is null || _view is null)
        {
            return;
        }
        using var environmentScope = _view.EnterPlatformEnvironmentScope();
        var composing = state.composingRange is { } range
            ? new TextRange(range.baseOffset, range.extentOffset)
            : TextRange.empty;
        _client.updateEditingValue(new TextEditingValue(
            state.text,
            new TextSelection(
                baseOffset: state.selection.baseOffset,
                extentOffset: state.selection.extentOffset),
            composing));
    }

    private void OnActionPerformed(DorotiTextInputAction action)
    {
        if (_client is null || _view is null)
        {
            return;
        }
        using var environmentScope = _view.EnterPlatformEnvironmentScope();
        _client?.performAction(action switch
        {
            DorotiTextInputAction.done => TextInputAction.done,
            DorotiTextInputAction.go => TextInputAction.go,
            DorotiTextInputAction.search => TextInputAction.search,
            DorotiTextInputAction.send => TextInputAction.send,
            DorotiTextInputAction.next => TextInputAction.next,
            DorotiTextInputAction.previous => TextInputAction.previous,
            DorotiTextInputAction.continueAction => TextInputAction.continueAction,
            DorotiTextInputAction.join => TextInputAction.join,
            DorotiTextInputAction.route => TextInputAction.route,
            DorotiTextInputAction.emergencyCall => TextInputAction.emergencyCall,
            DorotiTextInputAction.newline => TextInputAction.newline,
            DorotiTextInputAction.unspecified => TextInputAction.unspecified,
            _ => TextInputAction.none,
        });
    }

    private void OnConnectionClosed()
    {
        if (_client is null || _view is null)
        {
            return;
        }

        var client = _client;
        var view = _view;
        // Match Flutter's TextInputClient.onConnectionClosed contract: the
        // native endpoint is already gone, so detach the host bridge before
        // EditableText clears its connection and unfocuses its FocusNode.
        DetachCurrent(clearHost: false);
        using var environmentScope = view.EnterPlatformEnvironmentScope();
        client.connectionClosed();
    }

    private void DetachCurrent(bool clearHost)
    {
        if (_capability is not null)
        {
            _capability.EditingStateChanged -= OnEditingStateChanged;
            _capability.ActionPerformed -= OnActionPerformed;
            _capability.ConnectionClosed -= OnConnectionClosed;
            if (clearHost)
            {
                _capability.ClearClient();
            }
        }
        _capability = null;
        _client = null;
        _view = null;
    }

    private static DorotiTextEditingState ToHost(TextEditingValue value)
    {
        var selection = value.selection ?? new TextSelection(baseOffset: -1, extentOffset: -1);
        DorotiTextSelection? composing = value.composing is { isValid: true } range
            ? new DorotiTextSelection(checked((int)range.start), checked((int)range.end))
            : null;
        return new DorotiTextEditingState(
            value.text,
            new DorotiTextSelection(
                checked((int)selection.baseOffset),
                checked((int)selection.extentOffset)),
            composing);
    }

    private static DorotiTextInputConfiguration ToHost(TextInputConfiguration configuration) => new(
        (DorotiTextInputType)Math.Clamp(configuration.inputType.index, 0, 12),
        (DorotiTextInputAction)configuration.inputAction,
        (DorotiTextCapitalization)configuration.textCapitalization,
        configuration.readOnly,
        configuration.obscureText,
        configuration.autocorrect,
        configuration.enableSuggestions,
        configuration.actionLabel);
}

internal class _PlatformTextInputControl : TextInputControl
{
    public static _PlatformTextInputControl instance = new _PlatformTextInputControl();

    internal _PlatformTextInputControl()
    {
    }

    internal virtual MethodChannel _channel => TextInput._instance._channel;
    internal virtual DartMap<string, object> _configurationToJson(TextInputConfiguration configuration)
    {
        DartMap<string, object> json = configuration.toJson();
        if ((!object.Equals(TextInput._instance._currentControl, _PlatformTextInputControl.instance)))
        {
            DartMap<string, object> noneLocal = TextInputType.none.toJson();
            if (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb)
            {
                noneLocal["isMultiline"] = (object.Equals(configuration.inputType, TextInputType.multiline));
            }
            json["inputType"] = noneLocal;
        }
        return json;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void attach(TextInputClient client, TextInputConfiguration configuration)
    {
        _ = _channel.invokeMethod<object?>("TextInput.setClient", new List<object> { TextInput._instance._currentConnection!._id, _configurationToJson(configuration) }).then(((_) =>
        {
        }), onError: ((error, stack) => Text_inputLibrary._reportError(error, stack, "while attaching the text input client")));
    }

    public override void detach(TextInputClient client)
    {
        _ = _channel.invokeMethod<object?>("TextInput.clearClient").then(((_) =>
        {
        }), onError: ((error, stack) => Text_inputLibrary._reportError(error, stack, "while detaching the text input client")));
    }

    public override void updateConfig(TextInputConfiguration configuration)
    {
        _ = _channel.invokeMethod<object?>("TextInput.updateConfig", _configurationToJson(configuration)).then(((_) =>
        {
        }), onError: ((error, stack) => Text_inputLibrary._reportError(error, stack, "while updating text input configuration")));
    }

    public override void setEditingState(TextEditingValue value)
    {
        _ = _channel.invokeMethod<object?>("TextInput.setEditingState", value.toJSON()).then(((_) =>
        {
        }), onError: ((error, stack) => Text_inputLibrary._reportError(error, stack, "while setting text input editing state")));
    }

    public override void show()
    {
        _ = _channel.invokeMethod<object?>("TextInput.show").then(((_) =>
        {
        }), onError: ((error, stack) => Text_inputLibrary._reportError(error, stack, "while showing the text input client")));
    }

    public override void hide()
    {
        _ = _channel.invokeMethod<object?>("TextInput.hide").then(((_) =>
        {
        }), onError: ((error, stack) => Text_inputLibrary._reportError(error, stack, "while hiding the text input client")));
    }

    public override void setEditableSizeAndTransform(Size editableBoxSize, Matrix4 transform)
    {
        _ = _channel.invokeMethod<object?>("TextInput.setEditableSizeAndTransform", new DartMap<string, object> { ["width"] = editableBoxSize.width, ["height"] = editableBoxSize.height, ["transform"] = transform.storage }).then(((_) =>
        {
        }), onError: ((error, stack) => Text_inputLibrary._reportError(error, stack, "while setting text input size and transform")));
    }

    public override void setComposingRect(Rect rect)
    {
        _ = _channel.invokeMethod<object?>("TextInput.setMarkedTextRect", new DartMap<string, object> { ["width"] = rect.width, ["height"] = rect.height, ["x"] = rect.left, ["y"] = rect.top }).then(((_) =>
        {
        }), onError: ((error, stack) => Text_inputLibrary._reportError(error, stack, "while setting text input composing rect")));
    }

    public override void setCaretRect(Rect rect)
    {
        _ = _channel.invokeMethod<object?>("TextInput.setCaretRect", new DartMap<string, object> { ["width"] = rect.width, ["height"] = rect.height, ["x"] = rect.left, ["y"] = rect.top }).then(((_) =>
        {
        }), onError: ((error, stack) => Text_inputLibrary._reportError(error, stack, "while setting text input caret rect")));
    }

    public override void setSelectionRects(List<SelectionRect> selectionRects)
    {
        _ = _channel.invokeMethod<object?>("TextInput.setSelectionRects", selectionRects.map(((rect) =>
        {
            return new List<double> { rect.bounds.left, rect.bounds.top, rect.bounds.width, rect.bounds.height, rect.position, FoundationRuntimePorts.EnumIndex(rect.direction) };
        })).ToList()).then(((_) =>
        {
        }), onError: ((error, stack) => Text_inputLibrary._reportError(error, stack, "while setting text input selection rects")));
    }

    public override void setStyle(string? fontFamily, double? fontSize, FontWeight? fontWeight, TextDirection textDirection, TextAlign textAlign)
    {
        updateStyle(new TextInputStyle(fontFamily: fontFamily, fontSize: fontSize, fontWeight: fontWeight, textDirection: textDirection, textAlign: textAlign));
    }

    public override void updateStyle(TextInputStyle style)
    {
        _ = _channel.invokeMethod<object?>("TextInput.setStyle", style.toJson()).then(((_) =>
        {
        }), onError: ((error, stack) => Text_inputLibrary._reportError(error, stack, "while updating text input style")));
    }

    public override void requestAutofill()
    {
        _ = _channel.invokeMethod<object?>("TextInput.requestAutofill").then(((_) =>
        {
        }), onError: ((error, stack) => Text_inputLibrary._reportError(error, stack, "while requesting autofill")));
    }

    public override void finishAutofillContext(bool shouldSave = true)
    {
        _ = _channel.invokeMethod<object?>("TextInput.finishAutofillContext", shouldSave).then(((_) =>
        {
        }), onError: ((error, stack) => Text_inputLibrary._reportError(error, stack, "while finishing autofill context")));
    }

}

public class SystemContextMenuController : SystemContextMenuClient, Diagnosticable
{
    public virtual Action? onSystemHide { get; private set; }
    internal static MethodChannel _channel = SystemChannels.platform;
    internal static SystemContextMenuController? _lastShown = default;
    internal virtual DartMap<string, Action> _customActionCallbacks { get; private set; } = new DartMap<string, Action>();
    internal virtual Rect? _lastTargetRect { get; set; } = default;
    internal virtual List<IOSSystemContextMenuItemData>? _lastItems { get; set; } = default;
    internal virtual bool _hiddenBySystem { get; set; } = false;
    internal virtual bool _isDisposed { get; set; } = false;

    public SystemContextMenuController(Action? onSystemHide = null)
    {
        this.onSystemHide = onSystemHide;
    }

    public virtual bool isVisible => ((object.Equals(this, _lastShown)) && !_hiddenBySystem);
    public override void handleSystemHide()
    {
        DartRuntimePrimitives.Assert(() => !_isDisposed);
        DartRuntimePrimitives.Assert(() => isVisible);
        if ((_isDisposed || !isVisible))
        {
            return;
        }
        if ((object.Equals(_lastShown, this)))
        {
            _lastShown = null;
        }
        _hiddenBySystem = true;
        _customActionCallbacks.Clear();
        onSystemHide?.Invoke();
    }

    public override void handleCustomContextMenuAction(string actionId)
    {
        Action? callback = _customActionCallbacks.GetValueOrDefault(actionId);
        DartRuntimePrimitives.Assert(() => (callback is not null));
        callback?.Invoke();
    }

    public virtual Future show(Rect targetRect)
    {
        DartRuntimePrimitives.Assert(() => !_isDisposed);
        DartRuntimePrimitives.Assert(() => (TextInput._instance._currentConnection is not null));
        if ((((_lastShown is not null) && _lastShown!.isVisible) && (object.Equals(_lastShown!._lastTargetRect, targetRect))))
        {
            return Future.value();
        }
        DartRuntimePrimitives.Assert(() => (((_lastShown is null) || (object.Equals(_lastShown, this))) || !_lastShown!.isVisible));
        ServicesBinding.systemContextMenuClient = this;
        _lastTargetRect = targetRect;
        _lastShown = this;
        _hiddenBySystem = false;
        return _channel.invokeMethod<object?>("ContextMenu.showSystemContextMenu", new DartMap<string, object> { ["targetRect"] = new DartMap<string, double> { ["x"] = targetRect.left, ["y"] = targetRect.top, ["width"] = targetRect.width, ["height"] = targetRect.height } });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future showWithItems(Rect targetRect, List<IOSSystemContextMenuItemData> items)
    {
        DartRuntimePrimitives.Assert(() => !_isDisposed);
        DartRuntimePrimitives.Assert(() => (items.Count != 0));
        DartRuntimePrimitives.Assert(() => (TextInput._instance._currentConnection is not null));
        if (((((_lastShown is not null) && _lastShown!.isVisible) && (object.Equals(_lastShown!._lastTargetRect, targetRect))) && global::Doroti.Framework.Foundation.CollectionsLibrary.listEquals(_lastShown!._lastItems, items)))
        {
            return Future.value();
        }
        DartRuntimePrimitives.Assert(() => (((_lastShown is null) || (object.Equals(_lastShown, this))) || !_lastShown!.isVisible));
        ServicesBinding.systemContextMenuClient = this;
        _customActionCallbacks.Clear();
        foreach (var item in items)
        {
            if (item is IOSSystemContextMenuItemDataCustom item__as114122)
            {
                DartRuntimePrimitives.Assert(() => (!_customActionCallbacks.ContainsKey(item__as114122.callbackId) || (object.Equals((Action?)_customActionCallbacks.GetValueOrDefault(item__as114122.callbackId), (Action)item__as114122.onPressed))));
                _customActionCallbacks[item__as114122.callbackId] = item__as114122.onPressed;
            }
        }
        List<DartMap<string, object>> itemsJson = items.map(((item) => item._json)).ToList();
        _lastTargetRect = targetRect;
        _lastItems = items;
        _lastShown = this;
        _hiddenBySystem = false;
        return _channel.invokeMethod<object?>("ContextMenu.showSystemContextMenu", new DartMap<string, object> { ["targetRect"] = new DartMap<string, double> { ["x"] = targetRect.left, ["y"] = targetRect.top, ["width"] = targetRect.width, ["height"] = targetRect.height }, ["items"] = itemsJson });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async virtual Future hide()
    {
        DartRuntimePrimitives.Assert(() => !_isDisposed);
        if ((!object.Equals(this, _lastShown)))
        {
            return;
        }
        _lastShown = null;
        ServicesBinding.systemContextMenuClient = null;
        _customActionCallbacks.Clear();
        await _channel.invokeMethod<object?>("ContextMenu.hideSystemContextMenu");
        return;
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.Add(new DiagnosticsProperty<bool>("isVisible", isVisible));
        properties.Add(new FlagProperty("onSystemHide", value: (onSystemHide is not null), ifTrue: "callback set", ifFalse: "callback null", showName: true));
        properties.Add(new DiagnosticsProperty<bool>("_hiddenBySystem", _hiddenBySystem));
        properties.Add(new DiagnosticsProperty<bool>("_isDisposed", _isDisposed));
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => !_isDisposed);
        _ = hide();
        _isDisposed = true;
    }

}

public abstract class IOSSystemContextMenuItemData
{
    protected IOSSystemContextMenuItemData()
    {
    }

    public virtual string? title => null;
    internal abstract string _jsonType { get; }
    internal virtual DartMap<string, object> _json
    {
        get
        {
            return new DartMap<string, object> { ["callbackId"] = GetHashCode(), ["title"] = title, ["type"] = _jsonType };
        }
    }
    public override int GetHashCode() => title.GetHashCode();
    public override bool Equals(object? other)
    {
        var __other = other as IOSSystemContextMenuItemData;
        if (__other is null) return false;
        if (ReferenceEquals(this, __other))
        {
            return true;
        }
        if ((!object.Equals(__other.GetType(), this.GetType())))
        {
            return false;
        }
        return ((__other is IOSSystemContextMenuItemData) && (((IOSSystemContextMenuItemData)__other).title == title));
    }

}

public class IOSSystemContextMenuItemDataCopy : IOSSystemContextMenuItemData
{
    public IOSSystemContextMenuItemDataCopy()
    {
    }

    internal override string _jsonType => "copy";
}

public class IOSSystemContextMenuItemDataCut : IOSSystemContextMenuItemData
{
    public IOSSystemContextMenuItemDataCut()
    {
    }

    internal override string _jsonType => "cut";
}

public class IOSSystemContextMenuItemDataPaste : IOSSystemContextMenuItemData
{
    public IOSSystemContextMenuItemDataPaste()
    {
    }

    internal override string _jsonType => "paste";
}

public class IOSSystemContextMenuItemDataSelectAll : IOSSystemContextMenuItemData
{
    public IOSSystemContextMenuItemDataSelectAll()
    {
    }

    internal override string _jsonType => "selectAll";
}

public class IOSSystemContextMenuItemDataLookUp : IOSSystemContextMenuItemData, Diagnosticable
{
    public override string? title { get; }

    public IOSSystemContextMenuItemDataLookUp(string title)
    {
        this.title = title;
    }

    internal override string _jsonType => "lookUp";
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.Add(new StringProperty("title", title));
    }

}

public class IOSSystemContextMenuItemDataSearchWeb : IOSSystemContextMenuItemData, Diagnosticable
{
    public override string? title { get; }

    public IOSSystemContextMenuItemDataSearchWeb(string title)
    {
        this.title = title;
    }

    internal override string _jsonType => "searchWeb";
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.Add(new StringProperty("title", title));
    }

}

public class IOSSystemContextMenuItemDataShare : IOSSystemContextMenuItemData, Diagnosticable
{
    public override string? title { get; }

    public IOSSystemContextMenuItemDataShare(string title)
    {
        this.title = title;
    }

    internal override string _jsonType => "share";
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.Add(new StringProperty("title", title));
    }

}

public class IOSSystemContextMenuItemDataLiveText : IOSSystemContextMenuItemData
{
    public IOSSystemContextMenuItemDataLiveText()
    {
    }

    internal override string _jsonType => "captureTextFromCamera";
}

public class IOSSystemContextMenuItemDataCustom : IOSSystemContextMenuItemData, Diagnosticable
{
    public override string? title { get; }
    public virtual Action onPressed { get; private set; } = default!;

    public IOSSystemContextMenuItemDataCustom(string title, Action onPressed)
    {
        this.title = title;
        this.onPressed = onPressed;
    }

    public virtual string callbackId => GetHashCode().ToString();
    internal override string _jsonType => "custom";
    internal override DartMap<string, object> _json
    {
        get
        {
            return new DartMap<string, object> { ["id"] = callbackId, ["title"] = title, ["type"] = _jsonType };
        }
    }
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.Add(new StringProperty("title", title));
        properties.Add(new StringProperty("callbackId", callbackId));
        properties.Add(new DiagnosticsProperty<Action>("onPressed", onPressed));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(title, onPressed);
    public override bool Equals(object? other)
    {
        var __other = other as IOSSystemContextMenuItemDataCustom;
        if (__other is null) return false;
        if (ReferenceEquals(this, __other))
        {
            return true;
        }
        return (((__other is IOSSystemContextMenuItemDataCustom) && (((IOSSystemContextMenuItemDataCustom)__other).title == title)) && (object.Equals((Action)((IOSSystemContextMenuItemDataCustom)__other).onPressed, (Action)onPressed)));
    }

}
