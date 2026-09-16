// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/editable_text.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public delegate void SelectionChangedCallback(global::Doroti.Framework.Services.TextSelection selection, global::Doroti.Framework.Services.SelectionChangedCause? cause);

public delegate void AppPrivateCommandCallback(string action, DartMap<string, object?> data);

public delegate Widget EditableTextContextMenuBuilder(BuildContext context, EditableTextState editableTextState);

internal delegate TextPosition _ApplyTextBoundary__editable_text(TextPosition __unused0, bool __unused1, global::Doroti.Framework.Services.TextBoundary __unused2);

public static partial class Editable_textLibrary
{
    internal static Duration _kCursorBlinkHalfPeriod = Duration.Create(milliseconds: 500L);
}

public static partial class Editable_textLibrary
{
    internal static long _kObscureShowLatestCharCursorTicks = 3L;
}

public static partial class Editable_textLibrary
{
    public static List<string> kDefaultContentInsertionMimeTypes = new List<string> { "image/png", "image/bmp", "image/jpg", "image/tiff", "image/gif", "image/jpeg", "image/webp" };
}

internal class _CompositionCallback__editable_text : SingleChildRenderObjectWidget
{
    public virtual global::System.Action<global::Doroti.Framework.Rendering.Layer> compositeCallback { get; private set; } = default!;
    public virtual bool enabled { get; private set; } = default!;

    internal _CompositionCallback__editable_text(global::System.Action<global::Doroti.Framework.Rendering.Layer> compositeCallback, bool enabled, Widget? child = null) : base(child: child)
    {
        this.compositeCallback = compositeCallback;
        this.enabled = enabled;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderCompositionCallback__editable_text(compositeCallback, enabled);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderCompositionCallback__editable_text)renderObject;
        base.updateRenderObject(context, __renderObject);
        DartRuntimePrimitives.Assert(() => Equals(__renderObject.compositeCallback, compositeCallback));
        __renderObject.enabled = enabled;
    }

}

public class _RenderCompositionCallback__editable_text : global::Doroti.Framework.Rendering.RenderProxyBox
{
    public virtual global::System.Action<global::Doroti.Framework.Rendering.Layer> compositeCallback { get; private set; } = default!;
    internal virtual global::System.Action? _cancelCallback { get; set; } = default;
    internal virtual bool _enabled { get; set; } = false;

    internal _RenderCompositionCallback__editable_text(global::System.Action<global::Doroti.Framework.Rendering.Layer> compositeCallback, bool _enabled)
    {
        this.compositeCallback = compositeCallback;
        this._enabled = _enabled;
    }

    public virtual bool enabled
    {
        get => _enabled;
        set
        {
            var newValue = value;
            _enabled = newValue;
            if (!newValue)
            {
                _cancelCallback?.Invoke();
                _cancelCallback = null;
            }
            else
            {
                if (_cancelCallback is null)
                {
                    markNeedsPaint();
                }
            }
        }
    }
    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        if (enabled)
        {
            _cancelCallback ??= context.addCompositionCallback(compositeCallback);
        }
        base.paint(context, offset);
    }

}

public class TextEditingController : global::Doroti.Framework.Foundation.ValueNotifier<global::Doroti.Framework.Services.TextEditingValue>
{
    public TextEditingController(string? text = null) : base((text is null) ? TextEditingValue.empty : new global::Doroti.Framework.Services.TextEditingValue(text: text))
    {
    }

    public static TextEditingController CreateFromValue(global::Doroti.Framework.Services.TextEditingValue? value)
    {
        var __instance = new TextEditingController();
        __instance.value = value ?? TextEditingValue.empty;
        return __instance;
    }

    public virtual string text
    {
        get => value.text;
        set
        {
            var newText = value;
            this.value = this.value.copyWith(text: newText, selection: TextSelection.CreateCollapsed(offset: -1L), composing: TextRange.empty);
        }
    }
    public override global::Doroti.Framework.Services.TextEditingValue value
    {
        get => base.value;
        set
        {
            var newValue = value;
            DartRuntimePrimitives.Assert(() => !newValue.composing.isValid || newValue.isComposingRangeValid, () => (object?)$"New TextEditingValue {newValue} has an invalid non-empty composing range " + $"{newValue.composing}. It is recommended to use a valid composing range, " + "even for readonly text fields.");
            base.value = newValue;
        }
    }
    public virtual global::Doroti.Framework.Painting.TextSpan buildTextSpan(BuildContext context, global::Doroti.Framework.Painting.TextStyle? style = null, bool withComposing = default!)
    {
        DartRuntimePrimitives.Assert(() => !value.composing.isValid || !withComposing || value.isComposingRangeValid);
        bool composingRegionOutOfRange = !value.isComposingRangeValid || !withComposing;
        if (composingRegionOutOfRange)
        {
            return new global::Doroti.Framework.Painting.TextSpan(style: style, text: text);
        }
        global::Doroti.Framework.Painting.TextStyle composingStyle = style?.merge(new global::Doroti.Framework.Painting.TextStyle(decoration: TextDecoration.underline)) ?? new global::Doroti.Framework.Painting.TextStyle(decoration: TextDecoration.underline);
        return new global::Doroti.Framework.Painting.TextSpan(style: style, children: new List<global::Doroti.Framework.Painting.TextSpan> { new global::Doroti.Framework.Painting.TextSpan(text: value.composing.textBefore(value.text)), new global::Doroti.Framework.Painting.TextSpan(style: composingStyle, text: value.composing.textInside(value.text)), new global::Doroti.Framework.Painting.TextSpan(text: value.composing.textAfter(value.text)) }.Cast<global::Doroti.Framework.Painting.InlineSpan>().ToList());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Services.TextSelection selection
    {
        get => value.selection;
        set
        {
            var newSelection = value;
            if ((text.Length < newSelection.end) || (text.Length < newSelection.start))
            {
                throw DartRuntimePrimitives.AsException(FlutterError.Create($"invalid text selection: {newSelection}"));
            }
            global::Doroti.Ui.TextRange newComposing = _isSelectionWithinComposingRange(newSelection) ? this.value.composing : TextRange.empty;
            this.value = this.value.copyWith(selection: newSelection, composing: newComposing);
        }
    }
    public virtual void clear()
    {
        value = new global::Doroti.Framework.Services.TextEditingValue(selection: TextSelection.CreateCollapsed(offset: 0L));
    }

    public virtual void clearComposing()
    {
        value = value.copyWith(composing: TextRange.empty);
    }

    internal virtual bool _isSelectionWithinComposingRange(global::Doroti.Framework.Services.TextSelection selection)
    {
        return (selection.start >= value.composing.start) && (selection.end <= value.composing.end);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ToolbarOptions
{
    public static ToolbarOptions empty = new ToolbarOptions();
    public virtual bool copy { get; private set; } = default!;
    public virtual bool cut { get; private set; } = default!;
    public virtual bool paste { get; private set; } = default!;
    public virtual bool selectAll { get; private set; } = default!;

    public ToolbarOptions(bool copy = false, bool cut = false, bool paste = false, bool selectAll = false)
    {
        this.copy = copy;
        this.cut = cut;
        this.paste = paste;
        this.selectAll = selectAll;
    }

}

public class ContentInsertionConfiguration
{
    public virtual global::System.Action<global::Doroti.Framework.Services.KeyboardInsertedContent> onContentInserted { get; private set; } = default!;
    public virtual List<string> allowedMimeTypes { get; private set; } = default!;

    public ContentInsertionConfiguration(global::System.Action<global::Doroti.Framework.Services.KeyboardInsertedContent> onContentInserted, List<string> allowedMimeTypes = default!)
    {
        List<string> __allowedMimeTypes = allowedMimeTypes ?? Editable_textLibrary.kDefaultContentInsertionMimeTypes;
        this.onContentInserted = onContentInserted;
        this.allowedMimeTypes = __allowedMimeTypes;
        System.Diagnostics.Debug.Assert(Enumerable.Any(__allowedMimeTypes));
    }

}

internal class _KeyFrame__editable_text
{
    public static List<_KeyFrame__editable_text> iOSBlinkingCaretKeyFrames = new List<_KeyFrame__editable_text> { new _KeyFrame__editable_text(0, 1), new _KeyFrame__editable_text(0.5, 1), new _KeyFrame__editable_text(0.5375, 0.75), new _KeyFrame__editable_text(0.575, 0.5), new _KeyFrame__editable_text(0.6125, 0.25), new _KeyFrame__editable_text(0.65, 0), new _KeyFrame__editable_text(0.85, 0), new _KeyFrame__editable_text(0.8875, 0.25), new _KeyFrame__editable_text(0.925, 0.5), new _KeyFrame__editable_text(0.9625, 0.75), new _KeyFrame__editable_text(1, 1) };
    public virtual double time { get; private set; } = default!;
    public virtual double value { get; private set; } = default!;

    internal _KeyFrame__editable_text(double time, double value)
    {
        this.time = time;
        this.value = value;
    }

}

internal class _DiscreteKeyFrameSimulation__editable_text : global::Doroti.Framework.Physics.Simulation
{
    public virtual double maxDuration { get; private set; } = default!;
    internal virtual List<_KeyFrame__editable_text> _keyFrames { get; private set; } = default!;
    internal virtual long _lastKeyFrameIndex { get; set; } = 0L;

    internal static _DiscreteKeyFrameSimulation__editable_text CreateIOSBlinkingCaret()
    {
        return new _DiscreteKeyFrameSimulation__editable_text(_KeyFrame__editable_text.iOSBlinkingCaretKeyFrames, 1);
    }

    internal _DiscreteKeyFrameSimulation__editable_text(List<_KeyFrame__editable_text> _keyFrames, double maxDuration)
    {
        this._keyFrames = _keyFrames;
        this.maxDuration = maxDuration;
        System.Diagnostics.Debug.Assert(Enumerable.Any(_keyFrames));
        System.Diagnostics.Debug.Assert(_keyFrames.Last().time <= maxDuration);
        System.Diagnostics.Debug.Assert(((global::System.Func<bool>)(() =>
        {
            for (var i = 0L; i < (checked(_keyFrames.Count) - 1L); i += 1L)
            {
                if (_keyFrames[(int)i].time > _keyFrames[(int)(i + 1L)].time)
                {
                    return false;
                }
            }
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))());
    }

    public override double dx(double time) => 0;
    public override bool isDone(double time) => DartRuntimePrimitives.ConvertValue<bool>(time >= maxDuration);
    public override double x(double time)
    {
        long length = checked(_keyFrames.Count);
        long searchIndex = default!;
        long endIndex = default!;
        if (_keyFrames[(int)_lastKeyFrameIndex].time > time)
        {
            searchIndex = 0L;
            endIndex = _lastKeyFrameIndex;
        }
        else
        {
            searchIndex = _lastKeyFrameIndex;
            endIndex = length;
        }
        while (searchIndex < (endIndex - 1L))
        {
            DartRuntimePrimitives.Assert(() => _keyFrames[(int)searchIndex].time <= time);
            _KeyFrame__editable_text next = _keyFrames[(int)(searchIndex + 1L)];
            if (time < next.time)
            {
                break;
            }
            searchIndex += 1L;
        }
        _lastKeyFrameIndex = searchIndex;
        return _keyFrames[(int)_lastKeyFrameIndex].value;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class EditableText : StatefulWidget
{
    public virtual TextEditingController controller { get; private set; } = default!;
    public virtual FocusNode focusNode { get; private set; } = default!;
    public virtual string obscuringCharacter { get; private set; } = default!;
    public virtual bool obscureText { get; private set; } = default!;
    public virtual TextHeightBehavior? textHeightBehavior { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextWidthBasis textWidthBasis { get; private set; } = default!;
    public virtual bool readOnly { get; private set; } = default!;
    public virtual bool forceLine { get; private set; } = default!;
    public virtual ToolbarOptions toolbarOptions { get; private set; } = default!;
    public virtual bool showSelectionHandles { get; private set; } = default!;
    public virtual bool showCursor { get; private set; } = default!;
    public virtual bool autocorrect { get; private set; } = default!;
    public virtual global::Doroti.Framework.Services.SmartDashesType smartDashesType { get; private set; } = default!;
    public virtual global::Doroti.Framework.Services.SmartQuotesType smartQuotesType { get; private set; } = default!;
    public virtual bool enableSuggestions { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle style { get; private set; } = default!;
    public virtual UndoHistoryController? undoController { get; private set; }
    internal virtual global::Doroti.Framework.Painting.StrutStyle? _strutStyle { get; private set; }
    public virtual TextAlign textAlign { get; private set; } = default!;
    public virtual TextDirection? textDirection { get; private set; }
    public virtual global::Doroti.Framework.Services.TextCapitalization textCapitalization { get; private set; } = default!;
    public virtual Locale? locale { get; private set; }
    public virtual double? textScaleFactor { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextScaler? textScaler { get; private set; }
    public virtual Color cursorColor { get; private set; } = default!;
    public virtual Color? autocorrectionTextRectColor { get; private set; }
    public virtual Color backgroundCursorColor { get; private set; } = default!;
    public virtual long? maxLines { get; private set; }
    public virtual long? minLines { get; private set; }
    public virtual bool expands { get; private set; } = default!;
    public virtual bool autofocus { get; private set; } = default!;
    public virtual Color? selectionColor { get; private set; }
    public virtual TextSelectionControls? selectionControls { get; private set; }
    public virtual global::Doroti.Framework.Services.TextInputType keyboardType { get; private set; } = default!;
    public virtual global::Doroti.Framework.Services.TextInputAction? textInputAction { get; private set; }
    public virtual global::System.Action<string>? onChanged { get; private set; }
    public virtual global::System.Action? onEditingComplete { get; private set; }
    public virtual global::System.Action<string>? onSubmitted { get; private set; }
    public virtual global::System.Action<string, DartMap<string, object?>>? onAppPrivateCommand { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Services.TextSelection, global::Doroti.Framework.Services.SelectionChangedCause?>? onSelectionChanged { get; private set; }
    public virtual global::System.Action? onSelectionHandleTapped { get; private set; }
    public virtual object groupId { get; private set; } = default!;
    public virtual global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>? onTapOutside { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.PointerUpEvent>? onTapUpOutside { get; private set; }
    public virtual List<global::Doroti.Framework.Services.TextInputFormatter>? inputFormatters { get; private set; }
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual bool rendererIgnoresPointer { get; private set; } = default!;
    public virtual double cursorWidth { get; private set; } = default!;
    public virtual double? cursorHeight { get; private set; }
    public virtual Radius? cursorRadius { get; private set; }
    public virtual bool cursorOpacityAnimates { get; private set; } = default!;
    public virtual Offset? cursorOffset { get; private set; }
    public virtual bool paintCursorAboveText { get; private set; } = default!;
    public virtual BoxHeightStyle selectionHeightStyle { get; private set; } = default!;
    public virtual BoxWidthStyle selectionWidthStyle { get; private set; } = default!;
    public virtual Brightness keyboardAppearance { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsets scrollPadding { get; private set; } = default!;
    public virtual bool enableInteractiveSelection { get; private set; } = default!;
    public static bool debugDeterministicCursor = false;
    public virtual global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual ScrollController? scrollController { get; private set; }
    public virtual ScrollPhysics? scrollPhysics { get; private set; }
    public virtual bool scribbleEnabled { get; private set; } = default!;
    public virtual bool stylusHandwritingEnabled { get; private set; } = default!;
    public virtual bool selectAllOnFocus { get; private set; } = default!;
    public virtual IEnumerable<string>? autofillHints { get; private set; }
    public virtual global::Doroti.Framework.Services.AutofillClient? autofillClient { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual string? restorationId { get; private set; }
    public virtual ScrollBehavior? scrollBehavior { get; private set; }
    public virtual bool enableIMEPersonalizedLearning { get; private set; } = default!;
    public virtual ContentInsertionConfiguration? contentInsertionConfiguration { get; private set; }
    public virtual global::System.Func<BuildContext, EditableTextState, Widget>? contextMenuBuilder { get; private set; }
    public virtual SpellCheckConfiguration? spellCheckConfiguration { get; private set; }
    public virtual TextMagnifierConfiguration magnifierConfiguration { get; private set; } = default!;
    public virtual List<Locale>? hintLocales { get; private set; }
    public virtual bool? enableInlinePrediction { get; private set; }
    public const bool defaultStylusHandwritingEnabled = true;

    public EditableText(global::Doroti.Framework.Foundation.Key? key = null, TextEditingController controller = default!, FocusNode focusNode = default!, bool readOnly = false, string obscuringCharacter = "•", bool obscureText = false, bool? autocorrect = null, global::Doroti.Framework.Services.SmartDashesType? smartDashesType = null, global::Doroti.Framework.Services.SmartQuotesType? smartQuotesType = null, bool enableSuggestions = true, global::Doroti.Framework.Painting.TextStyle style = default!, global::Doroti.Framework.Painting.StrutStyle? strutStyle = null, Color cursorColor = default!, Color backgroundCursorColor = default!, TextAlign textAlign = TextAlign.start, TextDirection? textDirection = null, Locale? locale = null, double? textScaleFactor = null, global::Doroti.Framework.Painting.TextScaler? textScaler = null, long? maxLines = 1, long? minLines = null, bool expands = false, bool forceLine = true, TextHeightBehavior? textHeightBehavior = null, global::Doroti.Framework.Painting.TextWidthBasis textWidthBasis = TextWidthBasis.parent, bool autofocus = false, bool? showCursor = null, bool showSelectionHandles = false, Color? selectionColor = null, TextSelectionControls? selectionControls = null, global::Doroti.Framework.Services.TextInputType? keyboardType = null, global::Doroti.Framework.Services.TextInputAction? textInputAction = null, global::Doroti.Framework.Services.TextCapitalization textCapitalization = TextCapitalization.none, global::System.Action<string>? onChanged = null, global::System.Action? onEditingComplete = null, global::System.Action<string>? onSubmitted = null, global::System.Action<string, DartMap<string, object?>>? onAppPrivateCommand = null, global::System.Action<global::Doroti.Framework.Services.TextSelection, global::Doroti.Framework.Services.SelectionChangedCause?>? onSelectionChanged = null, global::System.Action? onSelectionHandleTapped = null, object groupId = default!, global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>? onTapOutside = null, global::System.Action<global::Doroti.Framework.Gestures.PointerUpEvent>? onTapUpOutside = null, List<global::Doroti.Framework.Services.TextInputFormatter>? inputFormatters = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, bool rendererIgnoresPointer = false, double cursorWidth = 2.0, double? cursorHeight = null, Radius? cursorRadius = null, bool cursorOpacityAnimates = false, Offset? cursorOffset = null, bool paintCursorAboveText = false, BoxHeightStyle? selectionHeightStyle = null, BoxWidthStyle? selectionWidthStyle = null, global::Doroti.Framework.Painting.EdgeInsets scrollPadding = default!, Brightness keyboardAppearance = Brightness.light, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = DragStartBehavior.start, bool? enableInteractiveSelection = null, bool? selectAllOnFocus = null, ScrollController? scrollController = null, ScrollPhysics? scrollPhysics = null, Color? autocorrectionTextRectColor = null, ToolbarOptions? toolbarOptions = null, IEnumerable<string>? autofillHints = default!, global::Doroti.Framework.Services.AutofillClient? autofillClient = null, Clip clipBehavior = Clip.hardEdge, string? restorationId = null, ScrollBehavior? scrollBehavior = null, bool scribbleEnabled = true, bool? stylusHandwritingEnabled = null, bool enableIMEPersonalizedLearning = true, ContentInsertionConfiguration? contentInsertionConfiguration = null, global::System.Func<BuildContext, EditableTextState, Widget>? contextMenuBuilder = null, SpellCheckConfiguration? spellCheckConfiguration = null, TextMagnifierConfiguration magnifierConfiguration = default!, UndoHistoryController? undoController = null, List<Locale>? hintLocales = null, bool? enableInlinePrediction = null) : base(key: key)
    {
        object __groupId = groupId ?? typeof(EditableText);
        global::Doroti.Framework.Painting.EdgeInsets __scrollPadding = scrollPadding ?? EdgeInsets.CreateAll(20.0);
        IEnumerable<string>? __autofillHints = autofillHints ?? new List<string>();
        bool __stylusHandwritingEnabled = stylusHandwritingEnabled ?? defaultStylusHandwritingEnabled;
        TextMagnifierConfiguration __magnifierConfiguration = magnifierConfiguration ?? TextMagnifierConfiguration.disabled;
        this.controller = controller;
        this.focusNode = focusNode;
        this.readOnly = readOnly;
        this.obscuringCharacter = obscuringCharacter;
        this.obscureText = obscureText;
        this.enableSuggestions = enableSuggestions;
        this.style = style;
        this.cursorColor = cursorColor;
        this.backgroundCursorColor = backgroundCursorColor;
        this.textAlign = textAlign;
        this.textDirection = textDirection;
        this.locale = locale;
        this.textScaleFactor = textScaleFactor;
        this.textScaler = textScaler;
        this.maxLines = maxLines;
        this.minLines = minLines;
        this.expands = expands;
        this.forceLine = forceLine;
        this.textHeightBehavior = textHeightBehavior;
        this.textWidthBasis = textWidthBasis;
        this.autofocus = autofocus;
        this.showSelectionHandles = showSelectionHandles;
        this.selectionColor = selectionColor;
        this.selectionControls = selectionControls;
        this.textInputAction = textInputAction;
        this.textCapitalization = textCapitalization;
        this.onChanged = onChanged;
        this.onEditingComplete = onEditingComplete;
        this.onSubmitted = onSubmitted;
        this.onAppPrivateCommand = onAppPrivateCommand;
        this.onSelectionChanged = onSelectionChanged;
        this.onSelectionHandleTapped = onSelectionHandleTapped;
        this.groupId = __groupId;
        this.onTapOutside = onTapOutside;
        this.onTapUpOutside = onTapUpOutside;
        this.mouseCursor = mouseCursor;
        this.rendererIgnoresPointer = rendererIgnoresPointer;
        this.cursorWidth = cursorWidth;
        this.cursorHeight = cursorHeight;
        this.cursorRadius = cursorRadius;
        this.cursorOpacityAnimates = cursorOpacityAnimates;
        this.cursorOffset = cursorOffset;
        this.paintCursorAboveText = paintCursorAboveText;
        this.scrollPadding = __scrollPadding;
        this.keyboardAppearance = keyboardAppearance;
        this.dragStartBehavior = dragStartBehavior;
        this.scrollController = scrollController;
        this.scrollPhysics = scrollPhysics;
        this.autocorrectionTextRectColor = autocorrectionTextRectColor;
        this.autofillHints = __autofillHints;
        this.autofillClient = autofillClient;
        this.clipBehavior = clipBehavior;
        this.restorationId = restorationId;
        this.scrollBehavior = scrollBehavior;
        this.scribbleEnabled = scribbleEnabled;
        this.stylusHandwritingEnabled = __stylusHandwritingEnabled;
        this.enableIMEPersonalizedLearning = enableIMEPersonalizedLearning;
        this.contentInsertionConfiguration = contentInsertionConfiguration;
        this.contextMenuBuilder = contextMenuBuilder;
        this.spellCheckConfiguration = spellCheckConfiguration;
        this.magnifierConfiguration = __magnifierConfiguration;
        this.undoController = undoController;
        this.hintLocales = hintLocales;
        this.enableInlinePrediction = enableInlinePrediction;
        this.autocorrect = autocorrect ?? _inferAutocorrect(autofillHints: autofillHints);
        this.smartDashesType = smartDashesType ?? (obscureText ? SmartDashesType.disabled : SmartDashesType.enabled);
        this.smartQuotesType = smartQuotesType ?? (obscureText ? SmartQuotesType.disabled : SmartQuotesType.enabled);
        this.enableInteractiveSelection = enableInteractiveSelection ?? !readOnly || !obscureText;
        this.selectAllOnFocus = selectAllOnFocus ?? _defaultSelectAllOnFocus;
        this.toolbarOptions = ((selectionControls is TextSelectionHandleControls) && (toolbarOptions is null)) ? ToolbarOptions.empty : (toolbarOptions ?? (obscureText ? (readOnly ? ToolbarOptions.empty : new ToolbarOptions(selectAll: true, paste: true)) : (readOnly ? new ToolbarOptions(selectAll: true, copy: true) : new ToolbarOptions(copy: true, cut: true, selectAll: true, paste: true))));
        _strutStyle = strutStyle;
        this.keyboardType = keyboardType ?? _inferKeyboardType(autofillHints: autofillHints, maxLines: maxLines);
        this.inputFormatters = (maxLines == 1L) ? new List<global::Doroti.Framework.Services.TextInputFormatter> { FilteringTextInputFormatter.singleLineFormatter } : inputFormatters;
        this.showCursor = showCursor ?? !readOnly;
        this.selectionHeightStyle = selectionHeightStyle ?? defaultSelectionHeightStyle;
        this.selectionWidthStyle = selectionWidthStyle ?? defaultSelectionWidthStyle;
        System.Diagnostics.Debug.Assert(obscuringCharacter.Length == 1L);
        System.Diagnostics.Debug.Assert((minLines is null) || (DartRuntimePrimitives.RequireValue(minLines) > 0L));
        System.Diagnostics.Debug.Assert(maxLines is null || minLines is null || maxLines >= DartRuntimePrimitives.RequireValue(minLines));
        System.Diagnostics.Debug.Assert(!expands || (maxLines is null) && (minLines is null));
        System.Diagnostics.Debug.Assert(!obscureText || (maxLines == 1L));
        System.Diagnostics.Debug.Assert((spellCheckConfiguration is null) || Equals(spellCheckConfiguration, SpellCheckConfiguration.CreateDisabled()) || (spellCheckConfiguration.misspelledTextStyle is not null));
    }

    public virtual global::Doroti.Framework.Painting.StrutStyle strutStyle
    {
        get
        {
            if (_strutStyle is null)
            {
                return Painting.StrutStyle.CreateFromTextStyle(style, forceStrutHeight: true);
            }
            return _strutStyle.inheritFromTextStyle(style);
        }
    }
    public virtual bool selectionEnabled => enableInteractiveSelection;
    public static global::Doroti.Ui.BoxHeightStyle defaultSelectionHeightStyle
    {
        get
        {
            if (Foundation.ConstantsLibrary.kIsWeb)
            {
                return BoxHeightStyle.max;
            }
            return BoxHeightStyle.includeLineSpacingMiddle;
        }
    }
    public static global::Doroti.Ui.BoxWidthStyle defaultSelectionWidthStyle
    {
        get
        {
            if (Foundation.ConstantsLibrary.kIsWeb)
            {
                if (Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.iOS) || WebBrowserDetectionIo.isSafari)
                {
                    return BoxWidthStyle.max;
                }
                return BoxWidthStyle.tight;
            }
            return BoxWidthStyle.max;
        }
    }
    internal virtual bool _userSelectionEnabled => DartRuntimePrimitives.ConvertValue<bool>(enableInteractiveSelection && (!readOnly || !obscureText));
    internal static bool _defaultSelectAllOnFocus
    {
        get
        {
            if (Foundation.ConstantsLibrary.kIsWeb)
            {
                return true;
            }
            return PlatformLibrary.defaultTargetPlatform switch { TargetPlatform.android => false, TargetPlatform.iOS => false, TargetPlatform.fuchsia => false, TargetPlatform.linux => true, TargetPlatform.macOS => true, TargetPlatform.windows => true, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        }
    }
    public static List<ContextMenuButtonItem> getEditableButtonItems(ClipboardStatus? clipboardStatus, global::System.Action? onCopy, global::System.Action? onCut, global::System.Action? onPaste, global::System.Action? onSelectAll, global::System.Action? onLookUp, global::System.Action? onSearchWeb, global::System.Action? onShare, global::System.Action? onLiveTextInput)
    {
        var resultButtonItem = new List<ContextMenuButtonItem>();
        if ((onPaste is null) || (!Equals(clipboardStatus, ClipboardStatus.unknown)))
        {
            var showShareBeforeSelectAll = Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.android);
            if (onCut is not null)
            {
                resultButtonItem.Add(new ContextMenuButtonItem(onPressed: onCut, type: ContextMenuButtonType.cut));
            }
            if (onCopy is not null)
            {
                resultButtonItem.Add(new ContextMenuButtonItem(onPressed: onCopy, type: ContextMenuButtonType.copy));
            }
            if (onPaste is not null)
            {
                resultButtonItem.Add(new ContextMenuButtonItem(onPressed: onPaste, type: ContextMenuButtonType.paste));
            }
            if ((onShare is not null) && showShareBeforeSelectAll)
            {
                resultButtonItem.Add(new ContextMenuButtonItem(onPressed: onShare, type: ContextMenuButtonType.share));
            }
            if (onSelectAll is not null)
            {
                resultButtonItem.Add(new ContextMenuButtonItem(onPressed: onSelectAll, type: ContextMenuButtonType.selectAll));
            }
            if (onLookUp is not null)
            {
                resultButtonItem.Add(new ContextMenuButtonItem(onPressed: onLookUp, type: ContextMenuButtonType.lookUp));
            }
            if (onSearchWeb is not null)
            {
                resultButtonItem.Add(new ContextMenuButtonItem(onPressed: onSearchWeb, type: ContextMenuButtonType.searchWeb));
            }
            if ((onShare is not null) && !showShareBeforeSelectAll)
            {
                resultButtonItem.Add(new ContextMenuButtonItem(onPressed: onShare, type: ContextMenuButtonType.share));
            }
        }
        if (onLiveTextInput is not null)
        {
            resultButtonItem.Add(new ContextMenuButtonItem(onPressed: () => onLiveTextInput(), type: ContextMenuButtonType.liveTextInput));
        }
        return resultButtonItem;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static bool _inferAutocorrect(IEnumerable<string>? autofillHints)
    {
        if ((autofillHints is null) || !Enumerable.Any(autofillHints) || Foundation.ConstantsLibrary.kIsWeb)
        {
            return true;
        }
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.iOS:
                {
                    bool passwordRelatedHint = autofillHints.any((hint) => (hint == AutofillHints.username) || (hint == AutofillHints.password) || (hint == AutofillHints.newPassword));
                    if (passwordRelatedHint)
                    {
                        return false;
                    }
                    break;
                }
            case TargetPlatform.macOS:
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
                {
                    break;
                }
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Framework.Services.TextInputType _inferKeyboardType(IEnumerable<string>? autofillHints, long? maxLines)
    {
        if ((autofillHints is null) || !Enumerable.Any(autofillHints))
        {
            return (maxLines == 1L) ? TextInputType.text : TextInputType.multiline;
        }
        string effectiveHint = autofillHints.First();
        if (!Foundation.ConstantsLibrary.kIsWeb)
        {
            switch (PlatformLibrary.defaultTargetPlatform)
            {
                case TargetPlatform.iOS:
                case TargetPlatform.macOS:
                    {
                        var iOSKeyboardType = new DartMap<string, global::Doroti.Framework.Services.TextInputType> { [AutofillHints.addressCity] = TextInputType.name, [AutofillHints.addressCityAndState] = TextInputType.name, [AutofillHints.addressState] = TextInputType.name, [AutofillHints.countryName] = TextInputType.name, [AutofillHints.creditCardNumber] = TextInputType.number, [AutofillHints.email] = TextInputType.emailAddress, [AutofillHints.emailOTPCode] = TextInputType.text, [AutofillHints.familyName] = TextInputType.name, [AutofillHints.fullStreetAddress] = TextInputType.name, [AutofillHints.givenName] = TextInputType.name, [AutofillHints.jobTitle] = TextInputType.name, [AutofillHints.location] = TextInputType.name, [AutofillHints.middleName] = TextInputType.name, [AutofillHints.name] = TextInputType.name, [AutofillHints.namePrefix] = TextInputType.name, [AutofillHints.nameSuffix] = TextInputType.name, [AutofillHints.newPassword] = TextInputType.text, [AutofillHints.newUsername] = TextInputType.text, [AutofillHints.nickname] = TextInputType.name, [AutofillHints.oneTimeCode] = TextInputType.number, [AutofillHints.organizationName] = TextInputType.text, [AutofillHints.password] = TextInputType.text, [AutofillHints.postalCode] = TextInputType.name, [AutofillHints.streetAddressLine1] = TextInputType.name, [AutofillHints.streetAddressLine2] = TextInputType.name, [AutofillHints.sublocality] = TextInputType.name, [AutofillHints.telephoneNumber] = TextInputType.name, [AutofillHints.url] = TextInputType.url, [AutofillHints.username] = TextInputType.text };
                        global::Doroti.Framework.Services.TextInputType? keyboardType = iOSKeyboardType.GetValueOrDefault(effectiveHint);
                        if (keyboardType is not null)
                        {
                            return keyboardType;
                        }
                        break;
                    }
                case TargetPlatform.android:
                case TargetPlatform.fuchsia:
                case TargetPlatform.linux:
                case TargetPlatform.windows:
                    {
                        break;
                    }
            }
        }
        if (maxLines != 1L)
        {
            return TextInputType.multiline;
        }
        var inferKeyboardType = new DartMap<string, global::Doroti.Framework.Services.TextInputType> { [AutofillHints.addressCity] = TextInputType.streetAddress, [AutofillHints.addressCityAndState] = TextInputType.streetAddress, [AutofillHints.addressState] = TextInputType.streetAddress, [AutofillHints.birthday] = TextInputType.datetime, [AutofillHints.birthdayDay] = TextInputType.datetime, [AutofillHints.birthdayMonth] = TextInputType.datetime, [AutofillHints.birthdayYear] = TextInputType.datetime, [AutofillHints.countryCode] = TextInputType.number, [AutofillHints.countryName] = TextInputType.text, [AutofillHints.creditCardExpirationDate] = TextInputType.datetime, [AutofillHints.creditCardExpirationDay] = TextInputType.datetime, [AutofillHints.creditCardExpirationMonth] = TextInputType.datetime, [AutofillHints.creditCardExpirationYear] = TextInputType.datetime, [AutofillHints.creditCardFamilyName] = TextInputType.name, [AutofillHints.creditCardGivenName] = TextInputType.name, [AutofillHints.creditCardMiddleName] = TextInputType.name, [AutofillHints.creditCardName] = TextInputType.name, [AutofillHints.creditCardNumber] = TextInputType.number, [AutofillHints.creditCardSecurityCode] = TextInputType.number, [AutofillHints.creditCardType] = TextInputType.text, [AutofillHints.email] = TextInputType.emailAddress, [AutofillHints.emailOTPCode] = TextInputType.text, [AutofillHints.familyName] = TextInputType.name, [AutofillHints.fullStreetAddress] = TextInputType.streetAddress, [AutofillHints.gender] = TextInputType.text, [AutofillHints.givenName] = TextInputType.name, [AutofillHints.impp] = TextInputType.url, [AutofillHints.jobTitle] = TextInputType.text, [AutofillHints.language] = TextInputType.text, [AutofillHints.location] = TextInputType.streetAddress, [AutofillHints.middleInitial] = TextInputType.name, [AutofillHints.middleName] = TextInputType.name, [AutofillHints.name] = TextInputType.name, [AutofillHints.namePrefix] = TextInputType.name, [AutofillHints.nameSuffix] = TextInputType.name, [AutofillHints.newPassword] = TextInputType.text, [AutofillHints.newUsername] = TextInputType.text, [AutofillHints.nickname] = TextInputType.text, [AutofillHints.oneTimeCode] = TextInputType.text, [AutofillHints.organizationName] = TextInputType.text, [AutofillHints.password] = TextInputType.text, [AutofillHints.photo] = TextInputType.text, [AutofillHints.postalAddress] = TextInputType.streetAddress, [AutofillHints.postalAddressExtended] = TextInputType.streetAddress, [AutofillHints.postalAddressExtendedPostalCode] = TextInputType.number, [AutofillHints.postalCode] = TextInputType.number, [AutofillHints.streetAddressLevel1] = TextInputType.streetAddress, [AutofillHints.streetAddressLevel2] = TextInputType.streetAddress, [AutofillHints.streetAddressLevel3] = TextInputType.streetAddress, [AutofillHints.streetAddressLevel4] = TextInputType.streetAddress, [AutofillHints.streetAddressLine1] = TextInputType.streetAddress, [AutofillHints.streetAddressLine2] = TextInputType.streetAddress, [AutofillHints.streetAddressLine3] = TextInputType.streetAddress, [AutofillHints.sublocality] = TextInputType.streetAddress, [AutofillHints.telephoneNumber] = TextInputType.phone, [AutofillHints.telephoneNumberAreaCode] = TextInputType.phone, [AutofillHints.telephoneNumberCountryCode] = TextInputType.phone, [AutofillHints.telephoneNumberDevice] = TextInputType.phone, [AutofillHints.telephoneNumberExtension] = TextInputType.phone, [AutofillHints.telephoneNumberLocal] = TextInputType.phone, [AutofillHints.telephoneNumberLocalPrefix] = TextInputType.phone, [AutofillHints.telephoneNumberLocalSuffix] = TextInputType.phone, [AutofillHints.telephoneNumberNational] = TextInputType.phone, [AutofillHints.transactionAmount] = TextInputType.CreateNumberWithOptions(@decimal: true), [AutofillHints.transactionCurrency] = TextInputType.text, [AutofillHints.url] = TextInputType.url, [AutofillHints.username] = TextInputType.text };
        return inferKeyboardType.GetValueOrDefault(effectiveHint) ?? TextInputType.text;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new EditableTextState());
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<TextEditingController>("controller", controller));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<FocusNode>("focusNode", focusNode));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("obscureText", obscureText, defaultValue: false));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("readOnly", readOnly, defaultValue: false));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("autocorrect", autocorrect, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Services.SmartDashesType>("smartDashesType", smartDashesType, defaultValue: obscureText ? SmartDashesType.disabled : SmartDashesType.enabled));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Services.SmartQuotesType>("smartQuotesType", smartQuotesType, defaultValue: obscureText ? SmartQuotesType.disabled : SmartQuotesType.enabled));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("enableSuggestions", enableSuggestions, defaultValue: true));
        style.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextAlign>("textAlign", textAlign, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", textDirection, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Locale>("locale", locale, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextScaler>("textScaler", textScaler, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("maxLines", maxLines, defaultValue: 1L));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("minLines", minLines, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("expands", expands, defaultValue: false));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("autofocus", autofocus, defaultValue: false));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Services.TextInputType>("keyboardType", keyboardType, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ScrollController>("scrollController", scrollController, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ScrollPhysics>("scrollPhysics", scrollPhysics, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<IEnumerable<string>>("autofillHints", autofillHints, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.TextHeightBehavior>("textHeightBehavior", textHeightBehavior, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("scribbleEnabled", scribbleEnabled, defaultValue: true));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("stylusHandwritingEnabled", DartRuntimePrimitives.RequireValue(stylusHandwritingEnabled), defaultValue: defaultStylusHandwritingEnabled));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("enableIMEPersonalizedLearning", enableIMEPersonalizedLearning, defaultValue: true));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool?>("enableInlinePrediction", enableInlinePrediction, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("enableInteractiveSelection", enableInteractiveSelection, defaultValue: true));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<UndoHistoryController>("undoController", undoController, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<SpellCheckConfiguration>("spellCheckConfiguration", spellCheckConfiguration, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<List<string>>("contentCommitMimeTypes", contentInsertionConfiguration?.allowedMimeTypes ?? new List<string>(), defaultValue: (contentInsertionConfiguration is null) ? new List<string>() : Editable_textLibrary.kDefaultContentInsertionMimeTypes));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<List<global::Doroti.Ui.Locale>?>("hintLocales", hintLocales, defaultValue: null));
    }

}

public class EditableTextState : State<EditableText>, AutomaticKeepAliveClientMixin<EditableText>, WidgetsBindingObserver, TickerProviderStateMixin<EditableText>, global::Doroti.Framework.Services.TextSelectionDelegate, global::Doroti.Framework.Services.TextInputClient, global::Doroti.Framework.Services.AutofillClient
{
    internal virtual Timer? _cursorTimer { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.AnimationController? _backingCursorBlinkOpacityController { get; set; } = default;
    private bool __late__iosBlinkCursorSimulation_initialized;
    private global::Doroti.Framework.Physics.Simulation __late__iosBlinkCursorSimulation = default!;
    internal virtual global::Doroti.Framework.Physics.Simulation _iosBlinkCursorSimulation
    {
        get
        {
            if (!__late__iosBlinkCursorSimulation_initialized)
            {
                __late__iosBlinkCursorSimulation = _DiscreteKeyFrameSimulation__editable_text.CreateIOSBlinkingCaret();
                __late__iosBlinkCursorSimulation_initialized = true;
            }
            return __late__iosBlinkCursorSimulation;
        }
    }
    internal virtual global::Doroti.Framework.Foundation.ValueNotifier<bool> _cursorVisibilityNotifier { get; private set; } = new global::Doroti.Framework.Foundation.ValueNotifier<bool>(true);
    internal virtual GlobalKey<IState> _editableKey { get; private set; } = GlobalKey<IState>.Create();
    public virtual ClipboardStatusNotifier clipboardStatus { get; private set; } = Foundation.ConstantsLibrary.kIsWeb ? new _WebClipboardStatusNotifier__editable_text() : new ClipboardStatusNotifier();
    internal virtual LiveTextInputStatusNotifier? _liveTextInputStatus { get; private set; } = Foundation.ConstantsLibrary.kIsWeb ? null : new LiveTextInputStatusNotifier();
    internal virtual global::Doroti.Framework.Services.TextInputConnection? _textInputConnection { get; set; } = default;
    internal virtual TextSelectionOverlay? _selectionOverlay { get; set; } = default;
    internal virtual ScrollNotificationObserverState? _scrollNotificationObserver { get; set; } = default;
    internal virtual (Rect selectionBounds, global::Doroti.Framework.Services.TextEditingValue value)? _dataWhenToolbarShowScheduled { get; set; } = default;
    internal virtual bool _listeningToScrollNotificationObserver { get; set; } = false;
    internal virtual GlobalKey<IState> _scrollableKey { get; private set; } = GlobalKey<IState>.Create();
    internal virtual ScrollController? _internalScrollController { get; set; } = default;
    internal virtual global::Doroti.Framework.Rendering.LayerLink _toolbarLayerLink { get; private set; } = new global::Doroti.Framework.Rendering.LayerLink();
    internal virtual global::Doroti.Framework.Rendering.LayerLink _startHandleLayerLink { get; private set; } = new global::Doroti.Framework.Rendering.LayerLink();
    internal virtual global::Doroti.Framework.Rendering.LayerLink _endHandleLayerLink { get; private set; } = new global::Doroti.Framework.Rendering.LayerLink();
    internal virtual bool _didAutoFocus { get; set; } = false;
    internal virtual AutofillGroupState? _currentAutofillScope { get; set; } = default;
    internal virtual SpellCheckConfiguration _spellCheckConfiguration { get; set; } = default!;
    internal virtual global::Doroti.Framework.Painting.TextStyle _style { get; set; } = default!;
    public virtual global::Doroti.Framework.Services.SpellCheckResults? spellCheckResults { get; set; } = default;
    internal virtual global::Doroti.Framework.Services.ProcessTextService _processTextService { get; private set; } = new global::Doroti.Framework.Services.DefaultProcessTextService();
    internal virtual List<global::Doroti.Framework.Services.ProcessTextAction> _processTextActions { get; private set; } = new List<global::Doroti.Framework.Services.ProcessTextAction>();
    internal static Duration _floatingCursorResetTime = Duration.Create(milliseconds: 125L);
    internal virtual global::Doroti.Framework.Animation.AnimationController? _floatingCursorResetController { get; set; } = default;
    internal virtual Orientation? _lastOrientation { get; set; } = default;
    internal virtual AppLifecycleListener _appLifecycleListener { get; private set; } = default!;
    internal virtual bool _justResumed { get; set; } = false;
    internal virtual bool _tickersEnabled { get; set; } = true;
    internal virtual global::Doroti.Framework.Services.TextEditingValue? _lastKnownRemoteTextEditingValue { get; set; } = default;
    internal virtual Offset? _startCaretCenter { get; set; } = default;
    internal virtual TextPosition? _lastTextPosition { get; set; } = default;
    internal virtual Offset? _pointOffsetOrigin { get; set; } = default;
    internal virtual Offset? _lastBoundedOffset { get; set; } = default;
    internal virtual long _batchEditDepth { get; set; } = 0L;
    internal virtual bool _hadFocusOnTapDown { get; set; } = false;
    internal virtual bool _restartConnectionScheduled { get; set; } = false;
    internal virtual bool _nextFocusChangeIsInternal { get; set; } = false;
    internal virtual bool _platformSupportsFadeOnScroll { get; private set; } = PlatformLibrary.defaultTargetPlatform switch { TargetPlatform.android => true, TargetPlatform.iOS => true, TargetPlatform.fuchsia or TargetPlatform.linux or TargetPlatform.macOS => false, TargetPlatform.windows => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
    internal virtual bool _showToolbarOnScreenScheduled { get; set; } = false;
    internal static Duration _caretAnimationDuration = Duration.Create(milliseconds: 100L);
    internal static global::Doroti.Framework.Animation.Curve _caretAnimationCurve = Curves.fastOutSlowIn;
    internal virtual bool _showCaretOnScreenScheduled { get; set; } = false;
    internal virtual double _lastBottomViewInset { get; set; } = default!;
    internal virtual long _obscureShowCharTicksPending { get; set; } = 0L;
    internal virtual long? _obscureLatestCharIndex { get; set; } = default;
    internal virtual _ScribbleCacheKey__editable_text? _scribbleCacheKey { get; set; } = default;
    private bool __late_renderEditable_initialized;
    private global::Doroti.Framework.Rendering.RenderEditable __late_renderEditable = default!;
    public virtual global::Doroti.Framework.Rendering.RenderEditable renderEditable
    {
        get
        {
            if (!__late_renderEditable_initialized)
            {
                __late_renderEditable = ((global::Doroti.Framework.Rendering.RenderEditable?)_editableKey.currentContext!.findRenderObject()!)!;
                __late_renderEditable_initialized = true;
            }
            return __late_renderEditable;
        }
    }
    internal virtual long _placeholderLocation { get; set; } = -1L;
    internal virtual long? _viewId { get; set; } = default;
    internal virtual TextRange? _currentPromptRectRange { get; set; } = default;
    private bool __late__transposeCharactersAction_initialized;
    private Action<TransposeCharactersIntent> __late__transposeCharactersAction = default!;
    internal virtual Action<TransposeCharactersIntent> _transposeCharactersAction
    {
        get
        {
            if (!__late__transposeCharactersAction_initialized)
            {
                __late__transposeCharactersAction = new CallbackAction<TransposeCharactersIntent>(onInvoke: (__arg0) => { ((global::System.Action<TransposeCharactersIntent>)_transposeCharacters)(__arg0); return default!; });
                __late__transposeCharactersAction_initialized = true;
            }
            return __late__transposeCharactersAction;
        }
    }
    private bool __late__replaceTextAction_initialized;
    private Action<ReplaceTextIntent> __late__replaceTextAction = default!;
    internal virtual Action<ReplaceTextIntent> _replaceTextAction
    {
        get
        {
            if (!__late__replaceTextAction_initialized)
            {
                __late__replaceTextAction = new CallbackAction<ReplaceTextIntent>(onInvoke: (__arg0) => { ((global::System.Action<ReplaceTextIntent>)_replaceText)(__arg0); return default!; });
                __late__replaceTextAction_initialized = true;
            }
            return __late__replaceTextAction;
        }
    }
    private bool __late__updateSelectionAction_initialized;
    private Action<UpdateSelectionIntent> __late__updateSelectionAction = default!;
    internal virtual Action<UpdateSelectionIntent> _updateSelectionAction
    {
        get
        {
            if (!__late__updateSelectionAction_initialized)
            {
                __late__updateSelectionAction = new CallbackAction<UpdateSelectionIntent>(onInvoke: (__arg0) => { ((global::System.Action<UpdateSelectionIntent>)_updateSelection)(__arg0); return default!; });
                __late__updateSelectionAction_initialized = true;
            }
            return __late__updateSelectionAction;
        }
    }
    private bool __late__verticalSelectionUpdateAction_initialized;
    private _UpdateTextSelectionVerticallyAction__editable_text<DirectionalCaretMovementIntent> __late__verticalSelectionUpdateAction = default!;
    internal virtual _UpdateTextSelectionVerticallyAction__editable_text<DirectionalCaretMovementIntent> _verticalSelectionUpdateAction
    {
        get
        {
            if (!__late__verticalSelectionUpdateAction_initialized)
            {
                __late__verticalSelectionUpdateAction = new _UpdateTextSelectionVerticallyAction__editable_text<DirectionalCaretMovementIntent>(this);
                __late__verticalSelectionUpdateAction_initialized = true;
            }
            return __late__verticalSelectionUpdateAction;
        }
    }
    private bool __late__actions_initialized;
    private DartMap<Type, dynamic> __late__actions = default!;
    internal virtual DartMap<Type, dynamic> _actions
    {
        get
        {
            if (!__late__actions_initialized)
            {
                __late__actions = new DartMap<Type, dynamic> { [typeof(DoNothingAndStopPropagationTextIntent)] = new DoNothingAction(consumesKey: false), [typeof(ReplaceTextIntent)] = _replaceTextAction, [typeof(UpdateSelectionIntent)] = _updateSelectionAction, [typeof(DirectionalFocusIntent)] = DirectionalFocusAction.CreateForTextField(), [typeof(DismissIntent)] = new CallbackAction<DismissIntent>(onInvoke: _hideToolbarIfVisible), [typeof(DeleteCharacterIntent)] = _makeOverridable(new _DeleteTextAction__editable_text<DeleteCharacterIntent>(this, _characterBoundary, _moveBeyondTextBoundary)), [typeof(DeleteToNextWordBoundaryIntent)] = _makeOverridable(new _DeleteTextAction__editable_text<DeleteToNextWordBoundaryIntent>(this, _nextWordBoundary, _moveBeyondTextBoundary)), [typeof(DeleteToLineBreakIntent)] = _makeOverridable(new _DeleteTextAction__editable_text<DeleteToLineBreakIntent>(this, _linebreak, _moveToTextBoundary)), [typeof(ExtendSelectionByCharacterIntent)] = _makeOverridable(new _UpdateTextSelectionAction__editable_text<ExtendSelectionByCharacterIntent>(this, _characterBoundary, _moveBeyondTextBoundary, ignoreNonCollapsedSelection: false)), [typeof(ExtendSelectionToNextWordBoundaryIntent)] = _makeOverridable(new _UpdateTextSelectionAction__editable_text<ExtendSelectionToNextWordBoundaryIntent>(this, _nextWordBoundary, _moveBeyondTextBoundary, ignoreNonCollapsedSelection: true)), [typeof(ExtendSelectionToNextParagraphBoundaryIntent)] = _makeOverridable(new _UpdateTextSelectionAction__editable_text<ExtendSelectionToNextParagraphBoundaryIntent>(this, _paragraphBoundary, _moveBeyondTextBoundary, ignoreNonCollapsedSelection: true)), [typeof(ExtendSelectionToLineBreakIntent)] = _makeOverridable(new _UpdateTextSelectionAction__editable_text<ExtendSelectionToLineBreakIntent>(this, _linebreak, _moveToTextBoundary, ignoreNonCollapsedSelection: true)), [typeof(ExtendSelectionVerticallyToAdjacentLineIntent)] = _makeOverridable(_verticalSelectionUpdateAction), [typeof(ExtendSelectionVerticallyToAdjacentPageIntent)] = _makeOverridable(_verticalSelectionUpdateAction), [typeof(ExtendSelectionToNextParagraphBoundaryOrCaretLocationIntent)] = _makeOverridable(new _UpdateTextSelectionAction__editable_text<ExtendSelectionToNextParagraphBoundaryOrCaretLocationIntent>(this, _paragraphBoundary, _moveBeyondTextBoundary, ignoreNonCollapsedSelection: true)), [typeof(ExtendSelectionToDocumentBoundaryIntent)] = _makeOverridable(new _UpdateTextSelectionAction__editable_text<ExtendSelectionToDocumentBoundaryIntent>(this, _documentBoundary, _moveBeyondTextBoundary, ignoreNonCollapsedSelection: true)), [typeof(ExtendSelectionToNextWordBoundaryOrCaretLocationIntent)] = _makeOverridable(new _UpdateTextSelectionAction__editable_text<ExtendSelectionToNextWordBoundaryOrCaretLocationIntent>(this, _nextWordBoundary, _moveBeyondTextBoundary, ignoreNonCollapsedSelection: true)), [typeof(ScrollToDocumentBoundaryIntent)] = _makeOverridable(new _WebComposingDisablingCallbackAction__editable_text<ScrollToDocumentBoundaryIntent>(this, onInvoke: (__arg0) => { ((global::System.Action<ScrollToDocumentBoundaryIntent>)_scrollToDocumentBoundary)(__arg0); return default!; })), [typeof(ScrollIntent)] = new CallbackAction<ScrollIntent>(onInvoke: (__arg0) => { ((global::System.Action<ScrollIntent>)_scroll)(__arg0); return default!; }), [typeof(ExpandSelectionToLineBreakIntent)] = _makeOverridable(new _UpdateTextSelectionAction__editable_text<ExpandSelectionToLineBreakIntent>(this, _linebreak, _moveToTextBoundary, ignoreNonCollapsedSelection: true, isExpand: true)), [typeof(ExpandSelectionToDocumentBoundaryIntent)] = _makeOverridable(new _UpdateTextSelectionAction__editable_text<ExpandSelectionToDocumentBoundaryIntent>(this, _documentBoundary, _moveToTextBoundary, ignoreNonCollapsedSelection: true, isExpand: true, extentAtIndex: true)), [typeof(SelectAllTextIntent)] = _makeOverridable(new _SelectAllAction__editable_text(this)), [typeof(CopySelectionTextIntent)] = _makeOverridable(new _CopySelectionAction__editable_text(this)), [typeof(PasteTextIntent)] = _makeOverridable(new _PasteSelectionAction__editable_text(this)), [typeof(TransposeCharactersIntent)] = _makeOverridable<TransposeCharactersIntent>(_transposeCharactersAction), [typeof(EditableTextTapOutsideIntent)] = _makeOverridable(new _EditableTextTapOutsideAction__editable_text()), [typeof(EditableTextTapUpOutsideIntent)] = _makeOverridable(new _EditableTextTapUpOutsideAction__editable_text()) };
                __late__actions_initialized = true;
            }
            return __late__actions;
        }
    }
    public virtual KeepAliveHandle? _keepAliveHandle { get; set; } = default;
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual global::Doroti.Framework.Animation.AnimationController _cursorBlinkOpacityController
    {
        get
        {
            return _backingCursorBlinkOpacityController ??= ((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{
    var __cascade = new global::Doroti.Framework.Animation.AnimationController(vsync: this);
    __cascade.addListener(_onCursorColorTick);
    return __cascade;
}))();
        }
    }
    internal virtual bool _hasInputConnection => DartRuntimePrimitives.ConvertValue<bool>(_textInputConnection?.attached ?? false);
    internal virtual bool _webContextMenuEnabled => DartRuntimePrimitives.ConvertValue<bool>(Foundation.ConstantsLibrary.kIsWeb && BrowserContextMenu.enabled);
    internal virtual ScrollController _scrollController => DartRuntimePrimitives.ConvertValue<ScrollController>(widget.scrollController ?? (_internalScrollController ??= new ScrollController()));
    public virtual global::Doroti.Framework.Services.AutofillScope? currentAutofillScope => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Services.AutofillScope>(_currentAutofillScope);
    internal virtual global::Doroti.Framework.Services.AutofillClient _effectiveAutofillClient => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Services.AutofillClient>((object?)widget.autofillClient ?? (object?)this);
    public virtual SpellCheckConfiguration spellCheckConfiguration => _spellCheckConfiguration;
    public virtual bool spellCheckEnabled => _spellCheckConfiguration.spellCheckEnabled;
    internal virtual bool _spellCheckResultsReceived => DartRuntimePrimitives.ConvertValue<bool>(spellCheckEnabled && (spellCheckResults is not null) && Enumerable.Any(spellCheckResults!.suggestionSpans));
    internal virtual bool _shouldCreateInputConnection => DartRuntimePrimitives.ConvertValue<bool>(Foundation.ConstantsLibrary.kIsWeb || Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.macOS) || !widget.readOnly);
    internal virtual bool _stylusHandwritingEnabled
    {
        get
        {
            if (!widget.scribbleEnabled)
            {
                return widget.scribbleEnabled;
            }
            return widget.stylusHandwritingEnabled;
        }
    }
    public virtual bool wantKeepAlive => widget.focusNode.hasFocus;
    internal virtual global::Doroti.Ui.Color _cursorColor
    {
        get
        {
            double effectiveOpacity = Math.Min(widget.cursorColor.alpha / 255.0, _cursorBlinkOpacityController.value);
            return widget.cursorColor.withOpacity(effectiveOpacity);
        }
    }
    public virtual bool cutEnabled
    {
        get
        {
            if (widget.selectionControls is not TextSelectionHandleControls)
            {
                return widget.toolbarOptions.cut && !widget.readOnly && !widget.obscureText;
            }
            return !widget.readOnly && !widget.obscureText && !textEditingValue.selection.isCollapsed;
        }
    }
    public virtual bool copyEnabled
    {
        get
        {
            if (widget.selectionControls is not TextSelectionHandleControls)
            {
                return widget.toolbarOptions.copy && !widget.obscureText;
            }
            return !widget.obscureText && !textEditingValue.selection.isCollapsed;
        }
    }
    public virtual bool pasteEnabled
    {
        get
        {
            if (widget.selectionControls is not TextSelectionHandleControls)
            {
                return widget.toolbarOptions.paste && !widget.readOnly;
            }
            return !widget.readOnly && Equals(clipboardStatus.value, ClipboardStatus.pasteable);
        }
    }
    public virtual bool selectAllEnabled
    {
        get
        {
            if (widget.selectionControls is not TextSelectionHandleControls)
            {
                return widget.toolbarOptions.selectAll && (!widget.readOnly || !widget.obscureText) && widget.enableInteractiveSelection;
            }
            if (!widget.enableInteractiveSelection || widget.readOnly && widget.obscureText)
            {
                return false;
            }
            switch (PlatformLibrary.defaultTargetPlatform)
            {
                case TargetPlatform.macOS:
                    {
                        return false;
                    }
                case TargetPlatform.iOS:
                    {
                        return (textEditingValue.text.Length != 0) && textEditingValue.selection.isCollapsed;
                    }
                case TargetPlatform.android:
                case TargetPlatform.fuchsia:
                case TargetPlatform.linux:
                case TargetPlatform.windows:
                    {
                        return (textEditingValue.text.Length != 0) && !((textEditingValue.selection.start == 0L) && (textEditingValue.selection.end == textEditingValue.text.Length));
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
        }
    }
    public virtual bool lookUpEnabled
    {
        get
        {
            if (!Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.iOS))
            {
                return false;
            }
            return !widget.obscureText && !textEditingValue.selection.isCollapsed && (textEditingValue.selection.textInside(textEditingValue.text).Trim() != "");
        }
    }
    public virtual bool searchWebEnabled
    {
        get
        {
            if (!Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.iOS))
            {
                return false;
            }
            return !widget.obscureText && !textEditingValue.selection.isCollapsed && (textEditingValue.selection.textInside(textEditingValue.text).Trim() != "");
        }
    }
    public virtual bool shareEnabled
    {
        get
        {
            switch (PlatformLibrary.defaultTargetPlatform)
            {
                case TargetPlatform.android:
                case TargetPlatform.iOS:
                    {
                        return !widget.obscureText && !textEditingValue.selection.isCollapsed && (textEditingValue.selection.textInside(textEditingValue.text).Trim() != "");
                    }
                case TargetPlatform.macOS:
                case TargetPlatform.fuchsia:
                case TargetPlatform.linux:
                case TargetPlatform.windows:
                    {
                        return false;
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
        }
    }
    public virtual bool liveTextInputEnabled
    {
        get
        {
            return Equals(_liveTextInputStatus?.value, LiveTextInputStatus.enabled) && !widget.obscureText && !widget.readOnly && textEditingValue.selection.isCollapsed;
        }
    }
    internal virtual void _onChangedClipboardStatus()
    {
        _selectionOverlay?.markNeedsBuild();
        setState(() =>
        {
        });
    }

    internal virtual void _onChangedLiveTextInputStatus()
    {
        setState(() =>
        {
        });
    }

    internal virtual global::Doroti.Framework.Services.TextEditingValue _textEditingValueforTextLayoutMetrics
    {
        get
        {
            Widget? editableWidget = _editableKey.currentContext?.widget;
            if (editableWidget is not _Editable__editable_text)
            {
                throw new InvalidOperationException("_Editable must be mounted.");
            }
            return ((_Editable__editable_text)editableWidget).value;
        }
    }
    public virtual void copySelection(global::Doroti.Framework.Services.SelectionChangedCause cause)
    {
        global::Doroti.Framework.Services.TextSelection selectionLocal = textEditingValue.selection;
        if (selectionLocal.isCollapsed || widget.obscureText)
        {
            return;
        }
        string textLocal = textEditingValue.text;
        DartRuntimePrimitives.Ignore(Clipboard.setData(new global::Doroti.Framework.Services.ClipboardData(text: selectionLocal.textInside(textLocal))).catchError(_reportClipboardError("while copying selection to clipboard")));
        if (Equals(DartRuntimePrimitives.RequireValue(cause), SelectionChangedCause.toolbar))
        {
            bringIntoView(textEditingValue.selection.extent);
            hideToolbar(false);
            switch (PlatformLibrary.defaultTargetPlatform)
            {
                case TargetPlatform.iOS:
                case TargetPlatform.macOS:
                case TargetPlatform.linux:
                case TargetPlatform.windows:
                    {
                        break;
                    }
                case TargetPlatform.android:
                case TargetPlatform.fuchsia:
                    {
                        userUpdateTextEditingValue(new global::Doroti.Framework.Services.TextEditingValue(text: textEditingValue.text, selection: TextSelection.CreateCollapsed(offset: textEditingValue.selection.end)), SelectionChangedCause.toolbar);
                        break;
                    }
            }
        }
        DartRuntimePrimitives.Ignore(clipboardStatus.update());
    }

    public virtual void cutSelection(global::Doroti.Framework.Services.SelectionChangedCause cause)
    {
        if (widget.readOnly || widget.obscureText)
        {
            return;
        }
        global::Doroti.Framework.Services.TextSelection selectionLocal = textEditingValue.selection;
        string textLocal = textEditingValue.text;
        if (selectionLocal.isCollapsed)
        {
            return;
        }
        DartRuntimePrimitives.Ignore(Clipboard.setData(new global::Doroti.Framework.Services.ClipboardData(text: selectionLocal.textInside(textLocal))).catchError(_reportClipboardError("while cutting selection to clipboard")));
        _replaceText(new ReplaceTextIntent(textEditingValue, "", selectionLocal, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(cause))));
        if (Equals(DartRuntimePrimitives.RequireValue(cause), SelectionChangedCause.toolbar))
        {
            Scheduler.SchedulerBinding.instance.addPostFrameCallback((_) =>
            {
                if (mounted)
                {
                    bringIntoView(textEditingValue.selection.extent);
                }
            }, debugLabel: "EditableText.bringSelectionIntoView");
            hideToolbar();
        }
        DartRuntimePrimitives.Ignore(clipboardStatus.update());
    }

    internal virtual global::System.Action<object, global::System.Diagnostics.StackTrace?> _reportClipboardError(string context)
    {
        return (exception, stack) =>
        {
            FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exception, stack: stack, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription(context)));
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _allowPaste
    {
        get
        {
            return !widget.readOnly && textEditingValue.selection.isValid;
        }
    }
    public async virtual Future pasteText(global::Doroti.Framework.Services.SelectionChangedCause cause)
    {
        if (!_allowPaste)
        {
            return;
        }
        global::Doroti.Framework.Services.ClipboardData? data = await Clipboard.getData(Clipboard.kTextPlain);
        if (data is null)
        {
            return;
        }
        _pasteText(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(cause)), data.text!);
    }

    internal virtual void _pasteText(global::Doroti.Framework.Services.SelectionChangedCause cause, string text)
    {
        if (!_allowPaste)
        {
            return;
        }
        global::Doroti.Framework.Services.TextSelection selectionLocal = textEditingValue.selection;
        long lastSelectionIndex = Math.Max(selectionLocal.baseOffset, selectionLocal.extentOffset);
        global::Doroti.Framework.Services.TextEditingValue collapsedTextEditingValue = textEditingValue.copyWith(selection: TextSelection.CreateCollapsed(offset: lastSelectionIndex));
        userUpdateTextEditingValue(collapsedTextEditingValue.replaced(selectionLocal, text), DartRuntimePrimitives.RequireValue(cause));
        if (Equals(DartRuntimePrimitives.RequireValue(cause), SelectionChangedCause.toolbar))
        {
            Scheduler.SchedulerBinding.instance.addPostFrameCallback((_) =>
            {
                if (mounted)
                {
                    bringIntoView(textEditingValue.selection.extent);
                }
            }, debugLabel: "EditableText.bringSelectionIntoView");
            hideToolbar();
        }
    }

    internal async virtual Future _pasteTextWithReporting(global::Doroti.Framework.Services.SelectionChangedCause cause)
    {
        try
        {
            await pasteText(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(cause)));
        }
        catch (Exception error)
        {
            var stackLocal = new System.Diagnostics.StackTrace();
            FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: error, stack: stackLocal, library: "widgets", context: new global::Doroti.Framework.Foundation.ErrorDescription("while pasting text to EditableText")));
        }
    }

    public virtual void selectAll(global::Doroti.Framework.Services.SelectionChangedCause cause)
    {
        if (widget.readOnly && widget.obscureText)
        {
            return;
        }
        userUpdateTextEditingValue(textEditingValue.copyWith(selection: new global::Doroti.Framework.Services.TextSelection(baseOffset: 0L, extentOffset: textEditingValue.text.Length)), DartRuntimePrimitives.RequireValue(cause));
        if (Equals(DartRuntimePrimitives.RequireValue(cause), SelectionChangedCause.toolbar))
        {
            switch (PlatformLibrary.defaultTargetPlatform)
            {
                case TargetPlatform.android:
                case TargetPlatform.iOS:
                case TargetPlatform.fuchsia:
                    {
                        break;
                    }
                case TargetPlatform.macOS:
                case TargetPlatform.linux:
                case TargetPlatform.windows:
                    {
                        hideToolbar();
                        break;
                    }
            }
            switch (PlatformLibrary.defaultTargetPlatform)
            {
                case TargetPlatform.android:
                case TargetPlatform.fuchsia:
                case TargetPlatform.linux:
                case TargetPlatform.windows:
                    {
                        bringIntoView(textEditingValue.selection.extent);
                        break;
                    }
                case TargetPlatform.macOS:
                case TargetPlatform.iOS:
                    {
                        break;
                    }
            }
        }
    }

    public async virtual Future lookUpSelection(global::Doroti.Framework.Services.SelectionChangedCause cause)
    {
        DartRuntimePrimitives.Assert(() => !widget.obscureText);
        string textLocal = textEditingValue.selection.textInside(textEditingValue.text);
        if (widget.obscureText || (textLocal.Length == 0))
        {
            return;
        }
        await SystemChannels.platform.invokeMethod<object>("LookUp.invoke", textLocal);
    }

    public async virtual Future searchWebForSelection(global::Doroti.Framework.Services.SelectionChangedCause cause)
    {
        DartRuntimePrimitives.Assert(() => !widget.obscureText);
        if (widget.obscureText)
        {
            return;
        }
        string textLocal = textEditingValue.selection.textInside(textEditingValue.text);
        if (textLocal.Length != 0)
        {
            await SystemChannels.platform.invokeMethod<object>("SearchWeb.invoke", textLocal);
        }
    }

    public async virtual Future shareSelection(global::Doroti.Framework.Services.SelectionChangedCause cause)
    {
        DartRuntimePrimitives.Assert(() => !widget.obscureText);
        if (widget.obscureText)
        {
            return;
        }
        string textLocal = textEditingValue.selection.textInside(textEditingValue.text);
        if (textLocal.Length != 0)
        {
            await SystemChannels.platform.invokeMethod<object>("Share.invoke", textLocal);
        }
    }

    internal virtual void _startLiveTextInput(global::Doroti.Framework.Services.SelectionChangedCause cause)
    {
        if (!liveTextInputEnabled)
        {
            return;
        }
        if (_hasInputConnection)
        {
            DartRuntimePrimitives.Ignore(LiveText.startLiveTextInput().then((_) =>
            {
            }, onError: (error, stack) =>
            {
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: error, stack: stack, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while starting Live Text input")));
            }));
        }
        if (Equals(DartRuntimePrimitives.RequireValue(cause), SelectionChangedCause.toolbar))
        {
            hideToolbar();
        }
    }

    public virtual global::Doroti.Framework.Services.SuggestionSpan? findSuggestionSpanAtCursorIndex(long cursorIndex)
    {
        if (!_spellCheckResultsReceived || (spellCheckResults!.suggestionSpans.Last().range.end < cursorIndex))
        {
            return null;
        }
        List<global::Doroti.Framework.Services.SuggestionSpan> suggestionSpansLocal = spellCheckResults!.suggestionSpans.ToList();
        var leftIndex = 0L;
        long rightIndex = checked(suggestionSpansLocal.Count) - 1L;
        var midIndex = 0L;
        while (leftIndex <= rightIndex)
        {
            midIndex = ((leftIndex + rightIndex) / 2L).floor();
            long currentSpanStart = suggestionSpansLocal[(int)midIndex].range.start;
            long currentSpanEnd = suggestionSpansLocal[(int)midIndex].range.end;
            if ((cursorIndex <= currentSpanEnd) && (cursorIndex >= currentSpanStart))
            {
                return suggestionSpansLocal[(int)midIndex];
            }
            else
            {
                if (cursorIndex <= currentSpanStart)
                {
                    rightIndex = midIndex - 1L;
                }
                else
                {
                    leftIndex = midIndex + 1L;
                }
            }
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static SpellCheckConfiguration _inferSpellCheckConfiguration(SpellCheckConfiguration? configuration, bool obscureText, global::Doroti.Framework.Services.TextInputType keyboardType, IEnumerable<string>? autofillHints)
    {
        global::Doroti.Framework.Services.SpellCheckService? spellCheckServiceLocal = configuration?.spellCheckService;
        bool spellCheckAutomaticallyDisabled = _isPasswordInput(obscureText: obscureText, keyboardType: keyboardType, autofillHints: autofillHints) || (configuration is null) || Equals(configuration, SpellCheckConfiguration.CreateDisabled());
        bool spellCheckServiceIsConfigured = (spellCheckServiceLocal is not null) || WidgetsBinding.instance.platformDispatcher.nativeSpellCheckServiceDefined;
        if (configuration is null || spellCheckAutomaticallyDisabled || !spellCheckServiceIsConfigured)
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    if (!spellCheckAutomaticallyDisabled && !spellCheckServiceIsConfigured)
                    {
                        FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: FlutterError.Create("Spell check was enabled with spellCheckConfiguration, but the " + "current platform does not have a supported spell check " + "service, and none was provided. Consider disabling spell " + "check for this platform or passing a SpellCheckConfiguration " + "with a specified spell check service."), library: "widget library", stack: new global::System.Diagnostics.StackTrace(true)));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            return SpellCheckConfiguration.CreateDisabled();
        }
        return configuration.copyWith(spellCheckService: DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Services.SpellCheckService>(spellCheckServiceLocal ?? new global::Doroti.Framework.Services.DefaultSpellCheckService()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static bool _isPasswordInput(bool obscureText, global::Doroti.Framework.Services.TextInputType keyboardType, IEnumerable<string>? autofillHints)
    {
        return obscureText || Equals(keyboardType, TextInputType.visiblePassword) || (autofillHints?.any((hint) => (hint == AutofillHints.password) || (hint == AutofillHints.newPassword)) ?? false);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<ContextMenuButtonItem>? buttonItemsForToolbarOptions(global::Doroti.Framework.Foundation.TargetPlatform? targetPlatform = null)
    {
        ToolbarOptions toolbarOptionsLocal = widget.toolbarOptions;
        if (Equals(toolbarOptionsLocal, ToolbarOptions.empty))
        {
            return null;
        }
        var buttonItems = new List<ContextMenuButtonItem>();
        if (toolbarOptionsLocal.cut && cutEnabled)
        {
            buttonItems.Add(new ContextMenuButtonItem(onPressed: () => cutSelection(SelectionChangedCause.toolbar), type: ContextMenuButtonType.cut));
        }
        if (toolbarOptionsLocal.copy && copyEnabled)
        {
            buttonItems.Add(new ContextMenuButtonItem(onPressed: () => copySelection(SelectionChangedCause.toolbar), type: ContextMenuButtonType.copy));
        }
        if (toolbarOptionsLocal.paste && pasteEnabled)
        {
            buttonItems.Add(new ContextMenuButtonItem(onPressed: () => { _ = _pasteTextWithReporting(SelectionChangedCause.toolbar); }, type: ContextMenuButtonType.paste));
        }
        if (toolbarOptionsLocal.selectAll && selectAllEnabled)
        {
            buttonItems.Add(new ContextMenuButtonItem(onPressed: () => selectAll(SelectionChangedCause.toolbar), type: ContextMenuButtonType.selectAll));
        }
        return buttonItems;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual (double startGlyphHeight, double endGlyphHeight) getGlyphHeights()
    {
        global::Doroti.Framework.Services.TextSelection selectionLocal = textEditingValue.selection;
        global::Doroti.Framework.Painting.InlineSpan span = renderEditable.text!;
        string prevText = span.toPlainText();
        string currText = textEditingValue.text;
        if ((prevText != currText) || !selectionLocal.isValid || selectionLocal.isCollapsed)
        {
            return (startGlyphHeight: renderEditable.preferredLineHeight, endGlyphHeight: renderEditable.preferredLineHeight);
        }
        string selectedGraphemes = selectionLocal.textInside(currText);
        long firstSelectedGraphemeExtent = selectedGraphemes.characters().first.Length;
        global::Doroti.Ui.Rect? startCharacterRect = renderEditable.getRectForComposingRange(new global::Doroti.Ui.TextRange(start: selectionLocal.start, end: selectionLocal.start + firstSelectedGraphemeExtent));
        long lastSelectedGraphemeExtent = selectedGraphemes.characters().last.Length;
        global::Doroti.Ui.Rect? endCharacterRect = renderEditable.getRectForComposingRange(new global::Doroti.Ui.TextRange(start: selectionLocal.end - lastSelectedGraphemeExtent, end: selectionLocal.end));
        return (startGlyphHeight: startCharacterRect?.height ?? (double)renderEditable.preferredLineHeight, endGlyphHeight: endCharacterRect?.height ?? (double)renderEditable.preferredLineHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TextSelectionToolbarAnchors contextMenuAnchors
    {
        get
        {
            if (renderEditable.lastSecondaryTapDownPosition is not null)
            {
                return new TextSelectionToolbarAnchors(primaryAnchor: DartRuntimePrimitives.RequireValue(renderEditable.lastSecondaryTapDownPosition));
            }
            var (startGlyphHeightLocal, endGlyphHeightLocal) = getGlyphHeights();
            global::Doroti.Framework.Services.TextSelection selectionLocal = textEditingValue.selection;
            List<global::Doroti.Framework.Rendering.TextSelectionPoint> points = renderEditable.getEndpointsForSelection(selectionLocal);
            return TextSelectionToolbarAnchors.CreateFromSelection(renderBox: renderEditable, startGlyphHeight: startGlyphHeightLocal, endGlyphHeight: endGlyphHeightLocal, selectionEndpoints: points);
        }
    }
    public virtual List<ContextMenuButtonItem> contextMenuButtonItems
    {
        get
        {
            return ((Func<List<ContextMenuButtonItem>>)(() =>
{
    var __cascade = buttonItemsForToolbarOptions() ?? EditableText.getEditableButtonItems(clipboardStatus: clipboardStatus.value, onCopy: copyEnabled ? (() => { copySelection(SelectionChangedCause.toolbar); }) : null, onCut: cutEnabled ? (() => { cutSelection(SelectionChangedCause.toolbar); }) : null, onPaste: pasteEnabled ? (() => { _ = _pasteTextWithReporting(SelectionChangedCause.toolbar); }) : null, onSelectAll: selectAllEnabled ? (() => { selectAll(SelectionChangedCause.toolbar); }) : null, onLookUp: lookUpEnabled ? (() => { _ = lookUpSelection(SelectionChangedCause.toolbar); }) : null, onSearchWeb: searchWebEnabled ? (() => { _ = searchWebForSelection(SelectionChangedCause.toolbar); }) : null, onShare: shareEnabled ? (() => { _ = shareSelection(SelectionChangedCause.toolbar); }) : null, onLiveTextInput: liveTextInputEnabled ? (() => { _startLiveTextInput(SelectionChangedCause.toolbar); }) : null);
    __cascade.AddRange(_textProcessingActionButtonItems.Cast<ContextMenuButtonItem>());
    // An empty desktop field still has a context menu when the clipboard is
    // empty (or its asynchronous availability query is pending).
    if (__cascade.Count == 0 && widget.selectionEnabled && !widget.readOnly &&
        widget.selectionControls is TextSelectionHandleControls &&
        Equals(widget.toolbarOptions, ToolbarOptions.empty) &&
        textEditingValue.text.Length == 0 &&
        PlatformLibrary.defaultTargetPlatform is
            TargetPlatform.windows or
            TargetPlatform.linux or
            TargetPlatform.macOS)
    {
        __cascade.Add(new ContextMenuButtonItem(onPressed: null, type: ContextMenuButtonType.paste));
    }
    return __cascade;
}))();
        }
    }
    internal virtual List<ContextMenuButtonItem> _textProcessingActionButtonItems
    {
        get
        {
            var buttonItems = new List<ContextMenuButtonItem>();
            global::Doroti.Framework.Services.TextSelection selectionLocal = textEditingValue.selection;
            if (widget.obscureText || !selectionLocal.isValid || selectionLocal.isCollapsed)
            {
                return buttonItems;
            }
            foreach (global::Doroti.Framework.Services.ProcessTextAction action in _processTextActions)
            {
                buttonItems.Add(new ContextMenuButtonItem(label: action.label, onPressed: async () =>
                {
                    string selectedText = selectionLocal.textInside(textEditingValue.text);
                    if (selectedText.Length != 0)
                    {
                        string? processedText = await _processTextService.processTextAction(action.id, selectedText, widget.readOnly);
                        if ((processedText is not null) && _allowPaste)
                        {
                            _pasteText(SelectionChangedCause.toolbar, processedText);
                        }
                        else
                        {
                            hideToolbar();
                        }
                    }
                }));
            }
            return buttonItems;
        }
    }
    public override void initState()
    {
        base.initState();
        if (wantKeepAlive)
        {
            _ensureKeepAlive();
        }
        _liveTextInputStatus?.addListener(_onChangedLiveTextInputStatus);
        clipboardStatus.addListener(_onChangedClipboardStatus);
        widget.controller.addListener(_didChangeTextEditingValue);
        widget.focusNode.addListener(_handleFocusChanged);
        _cursorVisibilityNotifier.value = widget.showCursor;
        _spellCheckConfiguration = _inferSpellCheckConfiguration(widget.spellCheckConfiguration, obscureText: widget.obscureText, keyboardType: widget.keyboardType, autofillHints: widget.autofillHints);
        _appLifecycleListener = new AppLifecycleListener(onResume: () => _onResume());
        DartRuntimePrimitives.Ignore(_initProcessTextActions());
    }

    internal virtual void _onResume()
    {
        _justResumed = true;
        FocusManager.instance.removeListener(_resetJustResumed);
        FocusManager.instance.addListener(_resetJustResumed);
    }

    internal virtual void _resetJustResumed()
    {
        _justResumed = false;
        FocusManager.instance.removeListener(_resetJustResumed);
    }

    internal async virtual Future _initProcessTextActions()
    {
        _processTextActions.Clear();
        _processTextActions.AddRange((await _processTextService.queryTextActions()).Cast<global::Doroti.Framework.Services.ProcessTextAction>());
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _style = MediaQuery.boldTextOf(context) ? widget.style.merge(new global::Doroti.Framework.Painting.TextStyle(fontWeight: FontWeight.bold)) : widget.style;
        AutofillGroupState? newAutofillGroup = AutofillGroup.maybeOf(context);
        if (!Equals(currentAutofillScope, newAutofillGroup))
        {
            _currentAutofillScope?.unregister(autofillId);
            _currentAutofillScope = newAutofillGroup;
            _currentAutofillScope?.register(_effectiveAutofillClient);
        }
        if (!_didAutoFocus && widget.autofocus)
        {
            _didAutoFocus = true;
            Scheduler.SchedulerBinding.instance.addPostFrameCallback((_) =>
            {
                if (mounted && renderEditable.hasSize)
                {
                    _flagInternalFocus();
                    FocusScope.of(context).autofocus(widget.focusNode);
                }
            }, debugLabel: "EditableText.autofocus");
        }
        bool newTickerEnabled = TickerMode.of(context);
        if (_tickersEnabled != newTickerEnabled)
        {
            _tickersEnabled = newTickerEnabled;
            if (_showBlinkingCursor)
            {
                _startCursorBlink();
            }
            else
            {
                if (!_tickersEnabled && (_cursorTimer is not null))
                {
                    _stopCursorBlink();
                }
            }
        }
        if (_hasInputConnection)
        {
            long newViewId = checked((long)View.of(context).viewId);
            if (newViewId != _viewId)
            {
                _textInputConnection!.updateConfig(_effectiveAutofillClient.textInputConfiguration);
            }
            Scheduler.SchedulerBinding.instance.addPostFrameCallback((_) =>
            {
                if (!mounted || !_hasInputConnection)
                {
                    return;
                }
                _textInputConnection!.updateStyle(_getTextInputStyle(context));
            }, debugLabel: "EditableText.updateStyle");
        }
        if ((!Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.iOS)) && (!Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.android)))
        {
            return;
        }
        Orientation orientation = MediaQuery.orientationOf(context);
        if (_lastOrientation is null)
        {
            _lastOrientation = orientation;
            return;
        }
        if (!Equals(orientation, _lastOrientation))
        {
            _lastOrientation = orientation;
            if (Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.iOS))
            {
                hideToolbar(false);
            }
            if (Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.android))
            {
                hideToolbar();
            }
        }
        if (_listeningToScrollNotificationObserver)
        {
            _scrollNotificationObserver?.removeListener(_handleContextMenuOnParentScroll);
            _scrollNotificationObserver = ScrollNotificationObserver.maybeOf(context);
            _scrollNotificationObserver?.addListener(_handleContextMenuOnParentScroll);
        }
    }

    public override void didUpdateWidget(EditableText oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(widget.controller, oldWidget.controller))
        {
            oldWidget.controller.removeListener(_didChangeTextEditingValue);
            widget.controller.addListener(_didChangeTextEditingValue);
            _updateRemoteEditingValueIfNeeded();
        }
        TextSelectionOverlay? selectionOverlay = _selectionOverlay;
        if ((selectionOverlay is not null) && selectionOverlay.toolbarIsVisible && (!Equals(widget.contextMenuBuilder, oldWidget.contextMenuBuilder)) && (widget.contextMenuBuilder is null == oldWidget.contextMenuBuilder is null))
        {
            WidgetsBinding.instance.addPostFrameCallback((_) =>
            {
                if (mounted && (_selectionOverlay?.toolbarIsVisible ?? false))
                {
                    _selectionOverlay!.showToolbar();
                }
            });
        }
        if ((_selectionOverlay is not null) && ((widget.contextMenuBuilder is null != oldWidget.contextMenuBuilder is null) || (!Equals(widget.selectionControls, oldWidget.selectionControls)) || (!Equals(widget.onSelectionHandleTapped, oldWidget.onSelectionHandleTapped)) || (!Equals(widget.dragStartBehavior, oldWidget.dragStartBehavior)) || (!Equals(widget.magnifierConfiguration, oldWidget.magnifierConfiguration))))
        {
            bool shouldShowToolbar = _selectionOverlay!.toolbarIsVisible;
            bool shouldShowHandles = _selectionOverlay!.handlesVisible;
            _selectionOverlay!.dispose();
            _selectionOverlay = _createSelectionOverlay();
            if (shouldShowToolbar || shouldShowHandles)
            {
                Scheduler.SchedulerBinding.instance.addPostFrameCallback((_) =>
                {
                    if (shouldShowToolbar)
                    {
                        _selectionOverlay!.showToolbar();
                    }
                    if (shouldShowHandles)
                    {
                        _selectionOverlay!.showHandles();
                    }
                });
            }
        }
        else
        {
            if (!Equals(widget.controller.selection, oldWidget.controller.selection))
            {
                _selectionOverlay?.update(_value);
            }
        }
        _selectionOverlay?.handlesVisible = widget.showSelectionHandles;
        if (!Equals(widget.autofillClient, oldWidget.autofillClient))
        {
            _currentAutofillScope?.unregister(oldWidget.autofillClient?.autofillId ?? autofillId);
            _currentAutofillScope?.register(_effectiveAutofillClient);
        }
        if (!Equals(widget.focusNode, oldWidget.focusNode))
        {
            oldWidget.focusNode.removeListener(_handleFocusChanged);
            widget.focusNode.addListener(_handleFocusChanged);
            updateKeepAlive();
        }
        if (!_shouldCreateInputConnection)
        {
            _closeInputConnectionIfNeeded();
        }
        else
        {
            if (oldWidget.readOnly && _hasFocus)
            {
                Scheduler.SchedulerBinding.instance.addPostFrameCallback((_) =>
                {
                    _openInputConnection();
                }, debugLabel: "EditableText.openInputConnection");
            }
        }
        if (Foundation.ConstantsLibrary.kIsWeb && _hasInputConnection)
        {
            if (oldWidget.readOnly != widget.readOnly)
            {
                _textInputConnection!.updateConfig(_effectiveAutofillClient.textInputConfiguration);
            }
        }
        if (_hasInputConnection)
        {
            var obscureTextChanged = oldWidget.obscureText != widget.obscureText;
            if (obscureTextChanged || (!Equals(oldWidget.keyboardType, widget.keyboardType)))
            {
                if (obscureTextChanged)
                {
                    _obscureShowCharTicksPending = 0L;
                    _obscureLatestCharIndex = null;
                }
                _textInputConnection!.updateConfig(_effectiveAutofillClient.textInputConfiguration);
            }
        }
        if ((!Equals(oldWidget.spellCheckConfiguration, widget.spellCheckConfiguration)) || (oldWidget.obscureText != widget.obscureText) || (!Equals(oldWidget.keyboardType, widget.keyboardType)) || !CollectionsLibrary.listEquals<string>(oldWidget.autofillHints?.ToList().Cast<string>().ToList(), widget.autofillHints?.ToList().Cast<string>().ToList()))
        {
            _spellCheckConfiguration = _inferSpellCheckConfiguration(widget.spellCheckConfiguration, obscureText: widget.obscureText, keyboardType: widget.keyboardType, autofillHints: widget.autofillHints);
            if (spellCheckEnabled)
            {
                if (textEditingValue.text.Length != 0)
                {
                    DartRuntimePrimitives.Ignore(_performSpellCheck(textEditingValue.text));
                }
            }
            else
            {
                spellCheckResults = null;
            }
        }
        if (!Equals(widget.style, oldWidget.style))
        {
            _style = MediaQuery.boldTextOf(context) ? widget.style.merge(new global::Doroti.Framework.Painting.TextStyle(fontWeight: FontWeight.bold)) : widget.style;
            if (_hasInputConnection)
            {
                Scheduler.SchedulerBinding.instance.addPostFrameCallback((_) =>
                {
                    if (!mounted || !_hasInputConnection)
                    {
                        return;
                    }
                    _textInputConnection!.updateStyle(_getTextInputStyle(context));
                }, debugLabel: "EditableText.updateStyle");
            }
        }
        if (widget.showCursor != oldWidget.showCursor)
        {
            _startOrStopCursorTimerIfNeeded();
        }
        bool canPasteLocal = (widget.selectionControls is TextSelectionHandleControls) ? pasteEnabled : (widget.selectionControls?.canPaste(this) ?? false);
        if (widget.selectionEnabled && pasteEnabled && canPasteLocal)
        {
            DartRuntimePrimitives.Ignore(clipboardStatus.update());
        }
    }

    internal virtual void _disposeScrollNotificationObserver()
    {
        _listeningToScrollNotificationObserver = false;
        if (_scrollNotificationObserver is not null)
        {
            _scrollNotificationObserver!.removeListener(_handleContextMenuOnParentScroll);
            _scrollNotificationObserver = null;
        }
    }

    internal virtual global::Doroti.Framework.Services.TextInputStyle _getTextInputStyle(BuildContext context)
    {
        double? letterSpacingOverride = MediaQuery.maybeLetterSpacingOverrideOf(context);
        double? wordSpacingOverride = MediaQuery.maybeWordSpacingOverrideOf(context);
        return new global::Doroti.Framework.Services.TextInputStyle(fontFamily: _style.fontFamily, fontSize: _style.fontSize, fontWeight: _style.fontWeight, textDirection: _textDirection, textAlign: widget.textAlign, letterSpacing: letterSpacingOverride ?? _style.letterSpacing, wordSpacing: wordSpacingOverride ?? _style.wordSpacing, lineHeight: renderEditable.preferredLineHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        _internalScrollController?.dispose();
        _currentAutofillScope?.unregister(autofillId);
        widget.controller.removeListener(_didChangeTextEditingValue);
        _floatingCursorResetController?.dispose();
        _floatingCursorResetController = null;
        _closeInputConnectionIfNeeded();
        DartRuntimePrimitives.Assert(() => !_hasInputConnection);
        _cursorTimer?.cancel();
        _cursorTimer = null;
        _backingCursorBlinkOpacityController?.dispose();
        _backingCursorBlinkOpacityController = null;
        _selectionOverlay?.dispose();
        _selectionOverlay = null;
        widget.focusNode.removeListener(_handleFocusChanged);
        WidgetsBinding.instance.removeObserver(this);
        _liveTextInputStatus?.removeListener(_onChangedLiveTextInputStatus);
        _liveTextInputStatus?.dispose();
        clipboardStatus.removeListener(_onChangedClipboardStatus);
        clipboardStatus.dispose();
        _cursorVisibilityNotifier.dispose();
        _appLifecycleListener.dispose();
        FocusManager.instance.removeListener(_unflagInternalFocus);
        FocusManager.instance.removeListener(_resetJustResumed);
        _disposeScrollNotificationObserver();
        DartRuntimePrimitives.Assert(() =>
            {
                if (_tickers is not null)
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in _tickers!)
                    {
                        if (ticker.isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
        DartRuntimePrimitives.Assert(() => _batchEditDepth <= 0L, () => (object?)$"unfinished batch edits: {_batchEditDepth}");
    }

    public virtual global::Doroti.Framework.Services.TextEditingValue currentTextEditingValue => _value;
    public virtual void updateEditingValue(global::Doroti.Framework.Services.TextEditingValue value)
    {
        if (!_shouldCreateInputConnection)
        {
            return;
        }
        if (_checkNeedsAdjustAffinity(value))
        {
            value = value.copyWith(selection: value.selection.copyWith(affinity: _value.selection.affinity));
        }
        if (widget.readOnly)
        {
            value = _value.copyWith(selection: value.selection);
        }
        _lastKnownRemoteTextEditingValue = value;
        if (Equals(value, _value))
        {
            return;
        }
        if ((value.text == _value.text) && Equals(value.composing, _value.composing))
        {
            global::Doroti.Framework.Services.SelectionChangedCause cause = default!;
            if (_textInputConnection?.scribbleInProgress ?? false)
            {
                cause = SelectionChangedCause.stylusHandwriting;
            }
            else
            {
                if (_pointOffsetOrigin is not null)
                {
                    cause = SelectionChangedCause.forcePress;
                }
                else
                {
                    cause = SelectionChangedCause.keyboard;
                }
            }
            _handleSelectionChanged(value.selection, DartRuntimePrimitives.RequireValue(cause));
        }
        else
        {
            if (value.text != _value.text)
            {
                hideToolbar(false);
            }
            _currentPromptRectRange = null;
            bool revealObscuredInput = _hasInputConnection && widget.obscureText && WidgetsBinding.instance.platformDispatcher.brieflyShowPassword && (value.text.Length == (_value.text.Length + 1L));
            _obscureShowCharTicksPending = revealObscuredInput ? Editable_textLibrary._kObscureShowLatestCharCursorTicks : 0L;
            _obscureLatestCharIndex = revealObscuredInput ? _value.selection.baseOffset : null;
            _formatAndSetValue(value, SelectionChangedCause.keyboard);
        }
        if (_showBlinkingCursor && (_cursorTimer is not null))
        {
            _stopCursorBlink(resetCharTicks: false);
            _startCursorBlink();
        }
        _scheduleShowCaretOnScreen(withAnimation: true);
    }

    internal virtual bool _checkNeedsAdjustAffinity(global::Doroti.Framework.Services.TextEditingValue value)
    {
        return (value.text == _value.text) && (value.selection.isCollapsed == _value.selection.isCollapsed) && (value.selection.start == _value.selection.start) && (!Equals(value.selection.affinity, _value.selection.affinity));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void performAction(global::Doroti.Framework.Services.TextInputAction action)
    {
        switch (action)
        {
            case TextInputAction.newline:
                {
                    if (!_isMultiline)
                    {
                        _finalizeEditing(action, shouldUnfocus: true);
                    }
                    break;
                }
            case TextInputAction.done:
            case TextInputAction.go:
            case TextInputAction.next:
            case TextInputAction.previous:
            case TextInputAction.search:
            case TextInputAction.send:
                {
                    _finalizeEditing(action, shouldUnfocus: true);
                    break;
                }
            case TextInputAction.continueAction:
            case TextInputAction.emergencyCall:
            case TextInputAction.join:
            case TextInputAction.none:
            case TextInputAction.route:
            case TextInputAction.unspecified:
                {
                    _finalizeEditing(action, shouldUnfocus: false);
                    break;
                }
        }
    }

    public virtual void performPrivateCommand(string action, DartMap<string, object?> data)
    {
        widget.onAppPrivateCommand?.Invoke(action, data);
    }

    public virtual void insertContent(global::Doroti.Framework.Services.KeyboardInsertedContent content)
    {
        DartRuntimePrimitives.Assert(() => widget.contentInsertionConfiguration?.allowedMimeTypes.Contains(content.mimeType) ?? false);
        widget.contentInsertionConfiguration?.onContentInserted?.Invoke(content);
    }

    internal virtual global::Doroti.Ui.Offset _floatingCursorOffset => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Offset>(new global::Doroti.Ui.Offset(0, renderEditable.preferredLineHeight / 2L));
    public virtual void updateFloatingCursor(global::Doroti.Framework.Services.RawFloatingCursorPoint point)
    {
        _floatingCursorResetController ??= ((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{
    var __cascade = new global::Doroti.Framework.Animation.AnimationController(vsync: this);
    __cascade.addListener(_onFloatingCursorResetTick);
    return __cascade;
}))();
        switch (point.state)
        {
            case FloatingCursorDragState.Start:
                {
                    if (_floatingCursorResetController!.isAnimating)
                    {
                        _floatingCursorResetController!.stop();
                        _onFloatingCursorResetTick();
                    }
                    _stopCursorBlink(resetCharTicks: false);
                    _cursorBlinkOpacityController.value = 1.0;
                    _pointOffsetOrigin = point.offset;
                    global::Doroti.Ui.Offset startCaretCenter = default!;
                    global::Doroti.Ui.TextPosition currentTextPosition = default!;
                    bool shouldResetOriginLocal = default!;
                    if (point.startLocation is not null)
                    {
                        shouldResetOriginLocal = false;
                        DartRuntimePrimitives.Ignore((startCaretCenter, currentTextPosition) = DartRuntimePrimitives.RequireValue(point.startLocation));
                    }
                    else
                    {
                        shouldResetOriginLocal = true;
                        currentTextPosition = new global::Doroti.Ui.TextPosition(offset: renderEditable.selection!.baseOffset, affinity: renderEditable.selection!.affinity);
                        startCaretCenter = renderEditable.getLocalRectForCaret(currentTextPosition).center;
                    }
                    _startCaretCenter = startCaretCenter;
                    _lastBoundedOffset = renderEditable.calculateBoundedFloatingCursorOffset(DartRuntimePrimitives.RequireValue(_startCaretCenter) - _floatingCursorOffset, shouldResetOrigin: shouldResetOriginLocal);
                    _lastTextPosition = currentTextPosition;
                    renderEditable.setFloatingCursor(point.state, DartRuntimePrimitives.RequireValue(_lastBoundedOffset), _lastTextPosition!);
                    break;
                }
            case FloatingCursorDragState.Update:
                {
                    global::Doroti.Ui.Offset centeredPoint = DartRuntimePrimitives.RequireValue(point.offset) - DartRuntimePrimitives.RequireValue(_pointOffsetOrigin);
                    global::Doroti.Ui.Offset rawCursorOffset = DartRuntimePrimitives.RequireValue(_startCaretCenter) + centeredPoint - _floatingCursorOffset;
                    _lastBoundedOffset = renderEditable.calculateBoundedFloatingCursorOffset(rawCursorOffset);
                    _lastTextPosition = renderEditable.getPositionForPoint(renderEditable.localToGlobal(DartRuntimePrimitives.RequireValue(_lastBoundedOffset) + _floatingCursorOffset));
                    renderEditable.setFloatingCursor(point.state, DartRuntimePrimitives.RequireValue(_lastBoundedOffset), _lastTextPosition!);
                    break;
                }
            case FloatingCursorDragState.End:
                {
                    if (_hasFocus)
                    {
                        _startCursorBlink();
                    }
                    if ((_lastTextPosition is not null) && (_lastBoundedOffset is not null))
                    {
                        _floatingCursorResetController!.value = 0.0;
                        _floatingCursorResetController!.animateTo(1.0, duration: _floatingCursorResetTime, curve: Curves.decelerate);
                    }
                    break;
                }
        }
    }

    internal virtual void _onFloatingCursorResetTick()
    {
        global::Doroti.Ui.Offset finalPosition = renderEditable.getLocalRectForCaret(_lastTextPosition!).centerLeft - _floatingCursorOffset;
        if (_floatingCursorResetController!.isCompleted)
        {
            renderEditable.setFloatingCursor(FloatingCursorDragState.End, finalPosition, _lastTextPosition!);
            if (renderEditable.selection!.isCollapsed)
            {
                _handleSelectionChanged(TextSelection.CreateFromPosition(_lastTextPosition!), SelectionChangedCause.forcePress);
            }
            _startCaretCenter = null;
            _lastTextPosition = null;
            _pointOffsetOrigin = null;
            _lastBoundedOffset = null;
        }
        else
        {
            double lerpValue = _floatingCursorResetController!.value;
            double lerpX = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(DartRuntimePrimitives.RequireValue(_lastBoundedOffset).dx, finalPosition.dx, lerpValue));
            double lerpY = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(DartRuntimePrimitives.RequireValue(_lastBoundedOffset).dy, finalPosition.dy, lerpValue));
            renderEditable.setFloatingCursor(FloatingCursorDragState.Update, new global::Doroti.Ui.Offset(lerpX, lerpY), _lastTextPosition!, resetLerpValue: lerpValue);
        }
    }

    internal virtual void _finalizeEditing(global::Doroti.Framework.Services.TextInputAction action, bool shouldUnfocus)
    {
        if (widget.onEditingComplete is not null)
        {
            try
            {
                widget.onEditingComplete!();
            }
            catch (Exception exceptionLocal)
            {
                var stackLocal = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exceptionLocal, stack: stackLocal, library: "widgets", context: new global::Doroti.Framework.Foundation.ErrorDescription($"while calling onEditingComplete for {action}")));
            }
        }
        else
        {
            widget.controller.clearComposing();
            if (shouldUnfocus)
            {
                switch (action)
                {
                    case TextInputAction.none:
                    case TextInputAction.unspecified:
                    case TextInputAction.done:
                    case TextInputAction.go:
                    case TextInputAction.search:
                    case TextInputAction.send:
                    case TextInputAction.continueAction:
                    case TextInputAction.join:
                    case TextInputAction.route:
                    case TextInputAction.emergencyCall:
                    case TextInputAction.newline:
                        {
                            widget.focusNode.unfocus();
                            break;
                        }
                    case TextInputAction.next:
                        {
                            widget.focusNode.nextFocus();
                            break;
                        }
                    case TextInputAction.previous:
                        {
                            widget.focusNode.previousFocus();
                            break;
                        }
                }
            }
        }
        global::System.Action<string>? onSubmittedLocal = widget.onSubmitted;
        if (onSubmittedLocal is null)
        {
            return;
        }
        try
        {
            onSubmittedLocal(_value.text);
        }
        catch (Exception exceptionAlternate)
        {
            var stackAlternate = new System.Diagnostics.StackTrace();
            FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exceptionAlternate, stack: stackAlternate, library: "widgets", context: new global::Doroti.Framework.Foundation.ErrorDescription($"while calling onSubmitted for {action}")));
        }
        if (shouldUnfocus)
        {
            _scheduleRestartConnection();
        }
    }

    public virtual void beginBatchEdit()
    {
        _batchEditDepth += 1L;
    }

    public virtual void endBatchEdit()
    {
        _batchEditDepth -= 1L;
        DartRuntimePrimitives.Assert(() => _batchEditDepth >= 0L, () => (object?)"Unbalanced call to endBatchEdit: beginBatchEdit must be called first.");
        _updateRemoteEditingValueIfNeeded();
    }

    internal virtual void _updateRemoteEditingValueIfNeeded()
    {
        if ((_batchEditDepth > 0L) || !_hasInputConnection)
        {
            return;
        }
        global::Doroti.Framework.Services.TextEditingValue localValue = _value;
        if (Equals(localValue, _lastKnownRemoteTextEditingValue))
        {
            return;
        }
        _textInputConnection!.setEditingState(localValue);
        _lastKnownRemoteTextEditingValue = localValue;
    }

    internal virtual global::Doroti.Framework.Services.TextEditingValue _value
    {
        get => widget.controller.value;
        set
        {
            var __value = value;
            widget.controller.value = __value;
        }
    }
    internal virtual bool _hasFocus => widget.focusNode.hasFocus;
    internal virtual bool _isMultiline => DartRuntimePrimitives.ConvertValue<bool>(widget.maxLines != 1L);
    internal virtual global::Doroti.Framework.Rendering.RevealedOffset _getOffsetToRevealCaret(Rect rect)
    {
        if (!_scrollController.position.allowImplicitScrolling)
        {
            return new global::Doroti.Framework.Rendering.RevealedOffset(offset: _scrollController.offset, rect: rect);
        }
        global::Doroti.Ui.Size editableSize = renderEditable.size;
        double additionalOffset = default!;
        global::Doroti.Ui.Offset unitOffset = default!;
        if (!_isMultiline)
        {
            additionalOffset = (rect.width >= editableSize.width) ? ((editableSize.width / 2L) - rect.center.dx) : Dart_uiLibrary.clampDouble(0.0, rect.right - editableSize.width, rect.left);
            unitOffset = new global::Doroti.Ui.Offset(1, 0);
        }
        else
        {
            var expandedRect = Rect.fromCenter(center: rect.center, width: rect.width, height: Math.Max(rect.height, renderEditable.preferredLineHeight));
            additionalOffset = (expandedRect.height >= editableSize.height) ? ((editableSize.height / 2L) - expandedRect.center.dy) : Dart_uiLibrary.clampDouble(0.0, expandedRect.bottom - editableSize.height, expandedRect.top);
            unitOffset = new global::Doroti.Ui.Offset(0, 1);
        }
        double targetOffset = Dart_uiLibrary.clampDouble(additionalOffset + _scrollController.offset, _scrollController.position.minScrollExtent, _scrollController.position.maxScrollExtent);
        double offsetDelta = _scrollController.offset - targetOffset;
        return new global::Doroti.Framework.Rendering.RevealedOffset(rect: rect.shift(unitOffset * offsetDelta), offset: targetOffset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _needsAutofill => _effectiveAutofillClient.textInputConfiguration.autofillConfiguration.enabled;
    internal virtual void _openInputConnection()
    {
        if (!_shouldCreateInputConnection)
        {
            return;
        }
        if (!_hasInputConnection)
        {
            global::Doroti.Framework.Services.TextEditingValue localValue = _value;
            _textInputConnection = (_needsAutofill && (currentAutofillScope is not null)) ? currentAutofillScope!.attach(this, _effectiveAutofillClient.textInputConfiguration) : TextInput.attach(this, _effectiveAutofillClient.textInputConfiguration);
            _updateSizeAndTransform();
            _schedulePeriodicPostFrameCallbacks();
            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Services.TextInputConnection>)(() =>
{
    var __cascade = _textInputConnection!;
    __cascade.updateStyle(_getTextInputStyle(context));
    __cascade.setEditingState(localValue);
    __cascade.show();
    return __cascade;
}))());
            if (_needsAutofill)
            {
                _textInputConnection!.requestAutofill();
            }
            _lastKnownRemoteTextEditingValue = localValue;
        }
        else
        {
            _textInputConnection!.show();
        }
    }

    internal virtual void _closeInputConnectionIfNeeded()
    {
        if (_hasInputConnection)
        {
            _textInputConnection!.close();
            _textInputConnection = null;
            _lastKnownRemoteTextEditingValue = null;
            _scribbleCacheKey = null;
            removeTextPlaceholder();
        }
    }

    internal virtual void _openOrCloseInputConnectionIfNeeded()
    {
        if (_hasFocus && widget.focusNode.consumeKeyboardToken())
        {
            _openInputConnection();
        }
        else
        {
            if (!_hasFocus)
            {
                _closeInputConnectionIfNeeded();
                widget.controller.clearComposing();
            }
        }
    }

    internal virtual void _scheduleRestartConnection()
    {
        if (_restartConnectionScheduled)
        {
            return;
        }
        _restartConnectionScheduled = true;
        DartAsyncRuntime.scheduleMicrotask(_restartConnectionIfNeeded);
    }

    internal virtual void _restartConnectionIfNeeded()
    {
        _restartConnectionScheduled = false;
        if (!_hasInputConnection || !_shouldCreateInputConnection)
        {
            return;
        }
        _textInputConnection!.close();
        _textInputConnection = null;
        _lastKnownRemoteTextEditingValue = null;
        global::Doroti.Framework.Services.AutofillScope? currentAutofillScopeLocal = _needsAutofill ? currentAutofillScope : null;
        global::Doroti.Framework.Services.TextInputConnection newConnection = currentAutofillScopeLocal?.attach(this, textInputConfiguration) ?? TextInput.attach(this, _effectiveAutofillClient.textInputConfiguration);
        _textInputConnection = newConnection;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Services.TextInputConnection>)(() =>
{
    var __cascade = newConnection;
    __cascade.show();
    __cascade.updateStyle(_getTextInputStyle(context));
    __cascade.setEditingState(_value);
    return __cascade;
}))());
        _lastKnownRemoteTextEditingValue = _value;
    }

    public virtual void didChangeInputControl(global::Doroti.Framework.Services.TextInputControl? oldControl, global::Doroti.Framework.Services.TextInputControl? newControl)
    {
        if (_hasFocus && _hasInputConnection)
        {
            oldControl?.hide();
            newControl?.show();
        }
    }

    public virtual bool onFocusReceived()
    {
        if (mounted && !_hasFocus && widget.focusNode.canRequestFocus)
        {
            widget.focusNode.requestFocus();
            return true;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void connectionClosed()
    {
        if (_hasInputConnection)
        {
            _textInputConnection!.connectionClosedReceived();
            _textInputConnection = null;
            _lastKnownRemoteTextEditingValue = null;
            widget.focusNode.unfocus();
        }
    }

    internal virtual void _flagInternalFocus()
    {
        _nextFocusChangeIsInternal = true;
        FocusManager.instance.addListener(_unflagInternalFocus);
    }

    internal virtual void _unflagInternalFocus()
    {
        _nextFocusChangeIsInternal = false;
        FocusManager.instance.removeListener(_unflagInternalFocus);
    }

    public virtual void requestKeyboard()
    {
        if (_hasFocus)
        {
            _openInputConnection();
        }
        else
        {
            _flagInternalFocus();
            widget.focusNode.requestFocus();
        }
    }

    internal virtual void _updateOrDisposeSelectionOverlayIfNeeded()
    {
        if (_selectionOverlay is not null)
        {
            if (_hasFocus)
            {
                _selectionOverlay!.update(_value);
            }
            else
            {
                _selectionOverlay!.dispose();
                _selectionOverlay = null;
            }
        }
    }

    internal virtual bool _isInternalScrollableNotification(BuildContext? notificationContext)
    {
        ScrollableState? scrollableState = notificationContext?.findAncestorStateOfType<ScrollableState>();
        return Equals(_scrollableKey.currentContext, scrollableState?.context);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _scrollableNotificationIsFromSameSubtree(BuildContext? notificationContext)
    {
        if (notificationContext is null)
        {
            return false;
        }
        BuildContext? currentContext = context;
        ScrollableState? notificationScrollableState = notificationContext.findAncestorStateOfType<ScrollableState>();
        if (notificationScrollableState is null)
        {
            return false;
        }
        while (currentContext is not null)
        {
            ScrollableState? scrollableState = currentContext.findAncestorStateOfType<ScrollableState>();
            if (Equals(scrollableState, notificationScrollableState))
            {
                return true;
            }
            currentContext = scrollableState?.context;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleContextMenuOnParentScroll(ScrollNotification notification)
    {
        if ((notification is not ScrollStartNotification) && (notification is not ScrollEndNotification))
        {
            return;
        }
        switch (notification)
        {
            case ScrollStartNotification __object177981 when _dataWhenToolbarShowScheduled is not null:
            case ScrollEndNotification __object178062 when _dataWhenToolbarShowScheduled is null:
                {
                    break;
                }
            case ScrollEndNotification __object178156 when !Equals(DartRuntimePrimitives.RequireValue(_dataWhenToolbarShowScheduled).value, _value):
                {
                    _dataWhenToolbarShowScheduled = null;
                    _disposeScrollNotificationObserver();
                    break;
                }
            case ScrollNotification { context: BuildContext contextLocal } __object178336 when !_isInternalScrollableNotification(contextLocal) && _scrollableNotificationIsFromSameSubtree(contextLocal):
                {
                    _handleContextMenuOnScroll(notification);
                    break;
                }
        }
    }

    internal virtual global::Doroti.Ui.Rect _calculateDeviceRect()
    {
        global::Doroti.Ui.Size screenSize = MediaQuery.sizeOf(context);
        global::Doroti.Ui.DorotiView view = View.of(context);
        double obscuredVertical = (view.padding.top + view.padding.bottom + view.viewInsets.bottom) / view.devicePixelRatio;
        double obscuredHorizontal = (view.padding.left + view.padding.right) / view.devicePixelRatio;
        var visibleScreenSize = new global::Doroti.Ui.Size(screenSize.width - obscuredHorizontal, screenSize.height - obscuredVertical);
        return Rect.fromLTWH(view.padding.left / view.devicePixelRatio, view.padding.top / view.devicePixelRatio, visibleScreenSize.width, visibleScreenSize.height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleContextMenuOnScroll(ScrollNotification notification)
    {
        if (_webContextMenuEnabled)
        {
            return;
        }
        if (!_platformSupportsFadeOnScroll)
        {
            _selectionOverlay?.updateForScroll();
            return;
        }
        if (notification is ScrollStartNotification)
        {
            ScrollStartNotification notification__as179959 = (ScrollStartNotification)notification;
            if (_dataWhenToolbarShowScheduled is not null)
            {
                return;
            }
            bool toolbarIsVisibleLocal = (_selectionOverlay is not null) && _selectionOverlay!.toolbarIsVisible && !_selectionOverlay!.spellCheckToolbarIsVisible;
            if (!toolbarIsVisibleLocal)
            {
                return;
            }
            List<global::Doroti.Ui.TextBox> selectionBoxes = renderEditable.getBoxesForSelection(_value.selection);
            global::Doroti.Ui.Rect selectionBoundsLocal = (_value.selection.isCollapsed || !Enumerable.Any(selectionBoxes)) ? renderEditable.getLocalRectForCaret(_value.selection.extent) : selectionBoxes.map<TextBox, Rect>((box) => box.toRect()).reduce((result, rect) => result.expandToInclude(rect));
            _dataWhenToolbarShowScheduled = (selectionBounds: selectionBoundsLocal, value: _value);
            _selectionOverlay?.hideToolbar();
        }
        else
        {
            if (notification is ScrollEndNotification)
            {
                ScrollEndNotification notification__as180881 = (ScrollEndNotification)notification;
                if (_dataWhenToolbarShowScheduled is null)
                {
                    return;
                }
                if (!Equals(DartRuntimePrimitives.RequireValue(_dataWhenToolbarShowScheduled).value, _value))
                {
                    _dataWhenToolbarShowScheduled = null;
                    _disposeScrollNotificationObserver();
                    return;
                }
                if (_showToolbarOnScreenScheduled)
                {
                    return;
                }
                _showToolbarOnScreenScheduled = true;
                void scheduleToolbar(Duration _)
                {
                    _showToolbarOnScreenScheduled = false;
                    if (!mounted || (_dataWhenToolbarShowScheduled is null))
                    {
                        return;
                    }
                    if (!Equals(DartRuntimePrimitives.RequireValue(_dataWhenToolbarShowScheduled).value, _value))
                    {
                        _dataWhenToolbarShowScheduled = null;
                        _disposeScrollNotificationObserver();
                        return;
                    }
                    global::Doroti.Ui.Rect deviceRect = _calculateDeviceRect();
                    bool selectionVisibleInEditable = renderEditable.selectionStartInViewport.value || renderEditable.selectionEndInViewport.value;
                    global::Doroti.Ui.Rect selectionBoundsAlternate = MatrixUtils.transformRect(renderEditable.getTransformTo(null), DartRuntimePrimitives.RequireValue(_dataWhenToolbarShowScheduled).selectionBounds);
                    bool selectionOverlapsWithDeviceRect = !selectionBoundsAlternate.hasNaN && deviceRect.overlaps(selectionBoundsAlternate);
                    if (selectionVisibleInEditable && selectionOverlapsWithDeviceRect && _selectionInViewport(DartRuntimePrimitives.RequireValue(_dataWhenToolbarShowScheduled).selectionBounds))
                    {
                        showToolbar();
                        _dataWhenToolbarShowScheduled = null;
                    }
                }
                switch (Scheduler.SchedulerBinding.instance.schedulerPhase)
                {
                    case Scheduler.SchedulerPhase.idle:
                    case Scheduler.SchedulerPhase.postFrameCallbacks:
                        {
                            Scheduler.SchedulerBinding.instance.scheduleFrameCallback(scheduleToolbar);
                            break;
                        }
                    case Scheduler.SchedulerPhase.transientCallbacks:
                    case Scheduler.SchedulerPhase.midFrameMicrotasks:
                    case Scheduler.SchedulerPhase.persistentCallbacks:
                        {
                            Scheduler.SchedulerBinding.instance.addPostFrameCallback((__arg0) => ((global::System.Action<Duration>)scheduleToolbar)(__arg0), debugLabel: "EditableText.scheduleToolbar");
                            break;
                        }
                }
            }
        }
    }

    internal virtual bool _selectionInViewport(Rect selectionBounds)
    {
        global::Doroti.Framework.Rendering.RenderAbstractViewport? closestViewport = RenderAbstractViewport.maybeOf(renderEditable);
        while (closestViewport is not null)
        {
            global::Doroti.Ui.Rect selectionBoundsLocalToViewport = MatrixUtils.transformRect(renderEditable.getTransformTo(closestViewport), selectionBounds);
            if (selectionBoundsLocalToViewport.hasNaN || closestViewport.paintBounds.hasNaN || !closestViewport.paintBounds.overlaps(selectionBoundsLocalToViewport))
            {
                return false;
            }
            closestViewport = RenderAbstractViewport.maybeOf(closestViewport.parent);
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _contextMenuBuilder(BuildContext context)
    {
        return widget.contextMenuBuilder!(context, this);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual TextSelectionOverlay _createSelectionOverlay()
    {
        var selectionOverlay = new TextSelectionOverlay(clipboardStatus: clipboardStatus, context: context, value: _value, debugRequiredFor: widget, toolbarLayerLink: _toolbarLayerLink, startHandleLayerLink: _startHandleLayerLink, endHandleLayerLink: _endHandleLayerLink, renderObject: renderEditable, selectionControls: widget.selectionControls, selectionDelegate: this, dragStartBehavior: widget.dragStartBehavior, onSelectionHandleTapped: widget.onSelectionHandleTapped, contextMenuBuilder: ((widget.contextMenuBuilder is null) || _webContextMenuEnabled) ? null : _contextMenuBuilder, magnifierConfiguration: widget.magnifierConfiguration);
        return selectionOverlay;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleSelectionChanged(global::Doroti.Framework.Services.TextSelection selection, global::Doroti.Framework.Services.SelectionChangedCause? cause)
    {
        string textLocal = widget.controller.value.text;
        if ((textLocal.Length < selection.end) || (textLocal.Length < selection.start))
        {
            return;
        }
        widget.controller.selection = selection;
        switch (cause)
        {
            case null:
            case SelectionChangedCause.doubleTap:
            case SelectionChangedCause.drag:
            case SelectionChangedCause.forcePress:
            case SelectionChangedCause.longPress:
            case SelectionChangedCause.stylusHandwriting:
            case SelectionChangedCause.tap:
            case SelectionChangedCause.toolbar:
                {
                    requestKeyboard();
                    break;
                }
            case SelectionChangedCause.keyboard:
                break;
        }
        if ((widget.selectionControls is null) && (widget.contextMenuBuilder is null))
        {
            _selectionOverlay?.dispose();
            _selectionOverlay = null;
        }
        else
        {
            if (_selectionOverlay is null)
            {
                _selectionOverlay = _createSelectionOverlay();
            }
            else
            {
                _selectionOverlay!.update(_value);
            }
            _selectionOverlay!.handlesVisible = widget.showSelectionHandles;
            _selectionOverlay!.showHandles();
        }
        try
        {
            widget.onSelectionChanged?.Invoke(selection, cause);
        }
        catch (Exception exceptionLocal)
        {
            var stackLocal = new System.Diagnostics.StackTrace();
            FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exceptionLocal, stack: stackLocal, library: "widgets", context: new global::Doroti.Framework.Foundation.ErrorDescription($"while calling onSelectionChanged for {cause}")));
        }
        if (_showBlinkingCursor && (_cursorTimer is not null))
        {
            _stopCursorBlink(resetCharTicks: false);
            _startCursorBlink();
        }
    }

    internal virtual void _scheduleShowCaretOnScreen(bool withAnimation)
    {
        if (_showCaretOnScreenScheduled)
        {
            return;
        }
        _showCaretOnScreenScheduled = true;
        Scheduler.SchedulerBinding.instance.addPostFrameCallback((_) =>
        {
            _showCaretOnScreenScheduled = false;
            var renderEditable = ((global::Doroti.Framework.Rendering.RenderEditable?)_editableKey.currentContext?.findRenderObject())!;
            if ((renderEditable is null) || !(renderEditable.selection?.isValid ?? false) || !_scrollController.hasClients)
            {
                return;
            }
            double lineHeight = renderEditable.preferredLineHeight;
            double bottomSpacing = widget.scrollPadding.bottom;
            if (_selectionOverlay?.selectionControls is not null)
            {
                double handleHeight = _selectionOverlay!.selectionControls!.getHandleSize(lineHeight).height;
                double interactiveHandleHeight = Math.Max(handleHeight, ConstantsLibrary.kMinInteractiveDimension);
                global::Doroti.Ui.Offset anchor = _selectionOverlay!.selectionControls!.getHandleAnchor(TextSelectionHandleType.collapsed, lineHeight);
                double handleCenter = (handleHeight / 2L) - anchor.dy;
                bottomSpacing = Math.Max(handleCenter + (interactiveHandleHeight / 2L), bottomSpacing);
            }
            global::Doroti.Framework.Painting.EdgeInsets caretPadding = widget.scrollPadding.copyWith(bottom: bottomSpacing);
            global::Doroti.Ui.Rect caretRect = renderEditable.getLocalRectForCaret(renderEditable.selection!.extent);
            global::Doroti.Framework.Rendering.RevealedOffset targetOffset = _getOffsetToRevealCaret(caretRect);
            global::Doroti.Ui.Rect rectToReveal = default!;
            global::Doroti.Framework.Services.TextSelection selectionLocal = textEditingValue.selection;
            if (selectionLocal.isCollapsed)
            {
                rectToReveal = targetOffset.rect;
            }
            else
            {
                List<global::Doroti.Ui.TextBox> selectionBoxes = renderEditable.getBoxesForSelection(selectionLocal);
                if (!Enumerable.Any(selectionBoxes))
                {
                    rectToReveal = targetOffset.rect;
                }
                else
                {
                    rectToReveal = (selectionLocal.baseOffset < selectionLocal.extentOffset) ? selectionBoxes.Last().toRect() : selectionBoxes.First().toRect();
                }
            }
            if (withAnimation)
            {
                DartRuntimePrimitives.Ignore(_scrollController.animateTo(targetOffset.offset, duration: _caretAnimationDuration, curve: _caretAnimationCurve));
                renderEditable.showOnScreen(rect: caretPadding.inflateRect(rectToReveal), duration: _caretAnimationDuration, curve: _caretAnimationCurve);
            }
            else
            {
                _scrollController.jumpTo(targetOffset.offset);
                renderEditable.showOnScreen(rect: caretPadding.inflateRect(rectToReveal));
            }
        }, debugLabel: "EditableText.showCaret");
    }

    public virtual void didChangeMetrics()
    {
        if (!mounted)
        {
            return;
        }
        global::Doroti.Ui.DorotiView view = View.of(context);
        if (_lastBottomViewInset != view.viewInsets.bottom)
        {
            Scheduler.SchedulerBinding.instance.addPostFrameCallback((_) =>
            {
                _selectionOverlay?.updateForScroll();
            }, debugLabel: "EditableText.updateForScroll");
            if (_lastBottomViewInset < view.viewInsets.bottom)
            {
                _scheduleShowCaretOnScreen(withAnimation: false);
            }
        }
        _lastBottomViewInset = view.viewInsets.bottom;
    }

    internal async virtual Future _performSpellCheck(string text)
    {
        try
        {
            global::Doroti.Ui.Locale? localeForSpellChecking = widget.locale ?? Localizations.maybeLocaleOf(context);
            DartRuntimePrimitives.Assert(() => localeForSpellChecking is not null, () => (object?)"Locale must be specified in widget or Localization widget must be in scope");
            List<global::Doroti.Framework.Services.SuggestionSpan>? suggestions = (await _spellCheckConfiguration.spellCheckService!.fetchSpellCheckSuggestions(DartRuntimePrimitives.RequireValue(localeForSpellChecking), text))?.ToList();
            if ((suggestions is null) || !mounted || !spellCheckEnabled)
            {
                return;
            }
            spellCheckResults = new global::Doroti.Framework.Services.SpellCheckResults(text, suggestions);
            double? lineHeightScaleFactorLocal = MediaQuery.maybeLineHeightScaleFactorOverrideOf(context);
            double? letterSpacingLocal = MediaQuery.maybeLetterSpacingOverrideOf(context);
            double? wordSpacingLocal = MediaQuery.maybeWordSpacingOverrideOf(context);
            renderEditable.text = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.InlineSpan>(_OverridingTextStyleTextSpanUtils__editable_text.applyTextSpacingOverrides(lineHeightScaleFactor: lineHeightScaleFactorLocal, letterSpacing: letterSpacingLocal, wordSpacing: wordSpacingLocal, textSpan: buildTextSpan()));
        }
        catch (Exception exceptionLocal)
        {
            var stackLocal = new System.Diagnostics.StackTrace();
            FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exceptionLocal, stack: stackLocal, library: "widgets", context: new global::Doroti.Framework.Foundation.ErrorDescription("while performing spell check")));
        }
    }

    internal virtual void _formatAndSetValue(global::Doroti.Framework.Services.TextEditingValue value, global::Doroti.Framework.Services.SelectionChangedCause? cause, bool userInteraction = false)
    {
        global::Doroti.Framework.Services.TextEditingValue oldValue = _value;
        var textChanged = oldValue.text != value.text;
        bool textCommitted = !oldValue.composing.isCollapsed && value.composing.isCollapsed;
        var selectionChanged = !Equals(oldValue.selection, value.selection);
        if (textChanged || textCommitted)
        {
            try
            {
                foreach (var formatter in widget.inputFormatters ?? Enumerable.Empty<global::Doroti.Framework.Services.TextInputFormatter>())
                    value = formatter.formatEditUpdate(_value, value);
                if (spellCheckEnabled && (value.text.Length != 0) && (_value.text != value.text))
                {
                    DartRuntimePrimitives.Ignore(_performSpellCheck(value.text));
                }
            }
            catch (Exception exceptionLocal)
            {
                var stackLocal = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exceptionLocal, stack: stackLocal, library: "widgets", context: new global::Doroti.Framework.Foundation.ErrorDescription("while applying input formatters")));
            }
        }
        global::Doroti.Framework.Services.TextSelection oldTextSelection = textEditingValue.selection;
        beginBatchEdit();
        _value = value;
        if (selectionChanged || userInteraction && (Equals(cause, SelectionChangedCause.longPress) || Equals(cause, SelectionChangedCause.keyboard)))
        {
            _handleSelectionChanged(_value.selection, cause);
            _bringIntoViewBySelectionState(oldTextSelection, value.selection, cause);
        }
        string currentText = _value.text;
        if (oldValue.text != currentText)
        {
            try
            {
                widget.onChanged?.Invoke(currentText);
            }
            catch (Exception exceptionAlternate)
            {
                var stackAlternate = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exceptionAlternate, stack: stackAlternate, library: "widgets", context: new global::Doroti.Framework.Foundation.ErrorDescription("while calling onChanged")));
            }
        }
        endBatchEdit();
    }

    internal virtual void _bringIntoViewBySelectionState(global::Doroti.Framework.Services.TextSelection oldSelection, global::Doroti.Framework.Services.TextSelection newSelection, global::Doroti.Framework.Services.SelectionChangedCause? cause)
    {
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.iOS:
            case TargetPlatform.macOS:
                {
                    if (Equals(cause, SelectionChangedCause.longPress) || Equals(cause, SelectionChangedCause.drag))
                    {
                        bringIntoView(newSelection.extent);
                    }
                    break;
                }
            case TargetPlatform.linux:
            case TargetPlatform.windows:
            case TargetPlatform.fuchsia:
            case TargetPlatform.android:
                {
                    if (Equals(cause, SelectionChangedCause.drag))
                    {
                        if (oldSelection.baseOffset != newSelection.baseOffset)
                        {
                            bringIntoView(newSelection.@base);
                        }
                        else
                        {
                            if (oldSelection.extentOffset != newSelection.extentOffset)
                            {
                                bringIntoView(newSelection.extent);
                            }
                        }
                    }
                    break;
                }
        }
    }

    internal virtual void _onCursorColorTick()
    {
        double effectiveOpacity = Math.Min(widget.cursorColor.alpha / 255.0, _cursorBlinkOpacityController.value);
        renderEditable.cursorColor = widget.cursorColor.withOpacity(effectiveOpacity);
        _cursorVisibilityNotifier.value = widget.showCursor && (EditableText.debugDeterministicCursor || (_cursorBlinkOpacityController.value > 0L));
    }

    internal virtual bool _showBlinkingCursor => DartRuntimePrimitives.ConvertValue<bool>(_hasFocus && _value.selection.isCollapsed && widget.showCursor && _tickersEnabled && !renderEditable.floatingCursorOn);
    public virtual bool cursorCurrentlyVisible => DartRuntimePrimitives.ConvertValue<bool>(_cursorBlinkOpacityController.value > 0L);
    public virtual Duration cursorBlinkInterval => Editable_textLibrary._kCursorBlinkHalfPeriod;
    public virtual TextSelectionOverlay? selectionOverlay => _selectionOverlay;
    internal virtual void _startCursorBlink()
    {
        DartRuntimePrimitives.Assert(() => !(_cursorTimer?.isActive ?? false) || !(_backingCursorBlinkOpacityController?.isAnimating ?? false));
        if (!widget.showCursor)
        {
            return;
        }
        if (!_tickersEnabled)
        {
            return;
        }
        _cursorTimer?.cancel();
        _cursorBlinkOpacityController.value = 1.0;
        if (EditableText.debugDeterministicCursor)
        {
            return;
        }
        if (widget.cursorOpacityAnimates)
        {
            DartRuntimePrimitives.Ignore(_cursorBlinkOpacityController.animateWith(_iosBlinkCursorSimulation).whenComplete(() => { ((Action)_onCursorTick)(); return default!; }));
        }
        else
        {
            _cursorTimer = new Timer(Editable_textLibrary._kCursorBlinkHalfPeriod, (timer) =>
            {
                _onCursorTick();
            });
        }
    }

    internal virtual void _onCursorTick()
    {
        if (_obscureShowCharTicksPending > 0L)
        {
            _obscureShowCharTicksPending = WidgetsBinding.instance.platformDispatcher.brieflyShowPassword ? (_obscureShowCharTicksPending - 1L) : 0L;
            if (_obscureShowCharTicksPending == 0L)
            {
                setState(() =>
                {
                });
            }
        }
        if (widget.cursorOpacityAnimates)
        {
            _cursorTimer?.cancel();
            _cursorTimer = new Timer(Duration.zero, () => { _ = _cursorBlinkOpacityController.animateWith(_iosBlinkCursorSimulation).whenComplete(() => { ((Action)_onCursorTick)(); return default!; }); });
        }
        else
        {
            if (!(_cursorTimer?.isActive ?? false) && _tickersEnabled)
            {
                _cursorTimer = new Timer(Editable_textLibrary._kCursorBlinkHalfPeriod, (timer) =>
                {
                    _onCursorTick();
                });
            }
            _cursorBlinkOpacityController.value = (_cursorBlinkOpacityController.value == 0L) ? 1 : 0;
        }
    }

    internal virtual void _stopCursorBlink(bool resetCharTicks = true)
    {
        _cursorBlinkOpacityController.value = renderEditable.floatingCursorOn ? 1.0 : 0.0;
        _cursorTimer?.cancel();
        _cursorTimer = null;
        if (resetCharTicks)
        {
            _obscureShowCharTicksPending = 0L;
        }
    }

    internal virtual void _startOrStopCursorTimerIfNeeded()
    {
        if (!_showBlinkingCursor)
        {
            _stopCursorBlink();
        }
        else
        {
            if (_cursorTimer is null)
            {
                _startCursorBlink();
            }
        }
    }

    internal virtual void _didChangeTextEditingValue()
    {
        if (_hasFocus && !_value.selection.isValid)
        {
            widget.controller.removeListener(_didChangeTextEditingValue);
            widget.controller.selection = _adjustedSelectionWhenFocused()!;
            widget.controller.addListener(_didChangeTextEditingValue);
        }
        _updateRemoteEditingValueIfNeeded();
        _startOrStopCursorTimerIfNeeded();
        _updateOrDisposeSelectionOverlayIfNeeded();
        setState(() =>
        {
        });
        _verticalSelectionUpdateAction.stopCurrentVerticalRunIfSelectionChanges();
    }

    internal virtual void _handleFocusChanged()
    {
        _openOrCloseInputConnectionIfNeeded();
        _startOrStopCursorTimerIfNeeded();
        _updateOrDisposeSelectionOverlayIfNeeded();
        if (_hasFocus)
        {
            WidgetsBinding.instance.addObserver(this);
            _lastBottomViewInset = View.of(context).viewInsets.bottom;
            if (!widget.readOnly)
            {
                _scheduleShowCaretOnScreen(withAnimation: true);
            }
            global::Doroti.Framework.Services.TextSelection? updatedSelection = _adjustedSelectionWhenFocused();
            if (updatedSelection is not null)
            {
                _handleSelectionChanged(updatedSelection, null);
            }
        }
        else
        {
            WidgetsBinding.instance.removeObserver(this);
            setState(() =>
            {
                _currentPromptRectRange = null;
            });
        }
        updateKeepAlive();
    }

    internal virtual global::Doroti.Framework.Services.TextSelection? _adjustedSelectionWhenFocused()
    {
        global::Doroti.Framework.Services.TextSelection? selectionLocal = default!;
        bool shouldSelectAll = widget.selectAllOnFocus && widget.selectionEnabled && !_isMultiline && !_nextFocusChangeIsInternal && !_justResumed;
        _justResumed = false;
        if (shouldSelectAll)
        {
            selectionLocal = new global::Doroti.Framework.Services.TextSelection(baseOffset: 0L, extentOffset: _value.text.Length);
        }
        else
        {
            if (!_value.selection.isValid)
            {
                selectionLocal = TextSelection.CreateCollapsed(offset: _value.text.Length);
            }
        }
        return selectionLocal;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _compositeCallback(global::Doroti.Framework.Rendering.Layer layer)
    {
        if (!renderEditable.attached || !_hasInputConnection)
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => mounted);
        DartRuntimePrimitives.Assert(() => ((Element?)context)!.debugIsActive);
        _updateSizeAndTransform();
    }

    internal virtual void _updateSizeAndTransform()
    {
        global::Doroti.Ui.Size sizeLocal = renderEditable.size;
        Matrix4 transform = renderEditable.getTransformTo(null);
        _textInputConnection!.setEditableSizeAndTransform(sizeLocal, transform);
    }

    internal virtual void _schedulePeriodicPostFrameCallbacks(Duration? duration = null)
    {
        if (!_hasInputConnection)
        {
            return;
        }
        _updateSelectionRects();
        _updateComposingRectIfNeeded();
        _updateCaretRectIfNeeded();
        Scheduler.SchedulerBinding.instance.addPostFrameCallback((__arg0) => ((global::System.Action<Duration?>)_schedulePeriodicPostFrameCallbacks)(DartRuntimePrimitives.ConvertValue<Duration>(__arg0)), debugLabel: "EditableText.postFrameCallbacks");
    }

    internal virtual void _updateSelectionRects(bool force = false)
    {
        if (!_stylusHandwritingEnabled || (!Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.iOS)))
        {
            return;
        }
        global::Doroti.Framework.Rendering.ScrollDirection scrollDirection = _scrollController.position.userScrollDirection;
        if (!Equals(scrollDirection, ScrollDirection.idle))
        {
            return;
        }
        global::Doroti.Framework.Painting.InlineSpan inlineSpanLocal = renderEditable.text!;
        double? lineHeightScaleFactor = MediaQuery.maybeLineHeightScaleFactorOverrideOf(context);
        global::Doroti.Framework.Painting.TextScaler effectiveTextScaler = (widget.textScaler, widget.textScaleFactor) switch { (global::Doroti.Framework.Painting.TextScaler textScalerLocal, _) => textScalerLocal, (null, double textScaleFactorLocal) => TextScaler.CreateLinear(textScaleFactorLocal), (null, null) => MediaQuery.textScalerOf(context) };
        var newCacheKey = new _ScribbleCacheKey__editable_text(inlineSpan: inlineSpanLocal, textAlign: widget.textAlign, textDirection: _textDirection, textScaler: effectiveTextScaler, textHeightBehavior: widget.textHeightBehavior ?? DefaultTextHeightBehavior.maybeOf(context), locale: widget.locale, structStyle: widget.strutStyle.merge(new global::Doroti.Framework.Painting.StrutStyle(height: lineHeightScaleFactor)), placeholder: _placeholderLocation, size: renderEditable.size);
        global::Doroti.Framework.Painting.RenderComparison comparison = force ? RenderComparison.layout : (_scribbleCacheKey?.compare(newCacheKey) ?? RenderComparison.layout);
        if (FoundationRuntimePorts.EnumIndex(comparison) < FoundationRuntimePorts.EnumIndex(RenderComparison.layout))
        {
            return;
        }
        _scribbleCacheKey = newCacheKey;
        var rects = new List<global::Doroti.Framework.Services.SelectionRect>();
        var graphemeStart = 0L;
        string plainText = inlineSpanLocal.toPlainText(includeSemanticsLabels: false);
        var characterRange = new CharacterRange(plainText);
        while (characterRange.MoveNext())
        {
            long graphemeEnd = graphemeStart + characterRange.Current.Length;
            List<global::Doroti.Ui.TextBox> boxes = renderEditable.getBoxesForSelection(new global::Doroti.Framework.Services.TextSelection(baseOffset: graphemeStart, extentOffset: graphemeEnd));
            global::Doroti.Ui.TextBox? box = !Enumerable.Any(boxes) ? null : boxes.First();
            if (box is not null)
            {
                global::Doroti.Ui.Rect paintBoundsLocal = renderEditable.paintBounds;
                if (paintBoundsLocal.bottom <= box.top)
                {
                    break;
                }
                if ((paintBoundsLocal.left <= box.right) && (box.left <= paintBoundsLocal.right) && (paintBoundsLocal.top <= box.bottom))
                {
                    rects.Add(new global::Doroti.Framework.Services.SelectionRect(position: graphemeStart, bounds: box.toRect(), direction: box.direction));
                }
            }
            graphemeStart = graphemeEnd;
        }
        _textInputConnection!.setSelectionRects(rects);
    }

    internal virtual void _updateComposingRectIfNeeded()
    {
        global::Doroti.Ui.TextRange composingRange = _value.composing;
        DartRuntimePrimitives.Assert(() => mounted);
        global::Doroti.Ui.Rect? composingRect = renderEditable.getRectForComposingRange(composingRange);
        if (composingRect is null)
        {
            long offsetLocal = composingRange.isValid ? composingRange.start : 0L;
            composingRect = renderEditable.getLocalRectForCaret(new global::Doroti.Ui.TextPosition(offset: offsetLocal));
        }
        _textInputConnection!.setComposingRect(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(composingRect)));
    }

    internal virtual void _updateCaretRectIfNeeded()
    {
        global::Doroti.Framework.Services.TextSelection? selectionLocal = renderEditable.selection;
        if ((selectionLocal is null) || !selectionLocal.isValid)
        {
            return;
        }
        var currentTextPosition = new global::Doroti.Ui.TextPosition(offset: selectionLocal.start);
        global::Doroti.Ui.Rect caretRect = renderEditable.getLocalRectForCaret(currentTextPosition);
        _textInputConnection!.setCaretRect(caretRect);
    }

    internal virtual global::Doroti.Ui.TextDirection _textDirection => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.TextDirection>(widget.textDirection ?? Directionality.of(context));
    public virtual global::Doroti.Framework.Services.TextEditingValue textEditingValue => _value;
    internal virtual double _devicePixelRatio => MediaQuery.devicePixelRatioOf(context);
    public virtual void userUpdateTextEditingValue(global::Doroti.Framework.Services.TextEditingValue value, global::Doroti.Framework.Services.SelectionChangedCause cause)
    {
        var shouldShowCaret = widget.readOnly ? (!Equals(_value.selection, value.selection)) : (!Equals(_value, value));
        if (shouldShowCaret)
        {
            _scheduleShowCaretOnScreen(withAnimation: true);
        }
        if (Equals(value, textEditingValue))
        {
            if (!widget.focusNode.hasFocus)
            {
                _flagInternalFocus();
                widget.focusNode.requestFocus();
                _selectionOverlay ??= _createSelectionOverlay();
            }
            return;
        }
        _formatAndSetValue(value, cause, userInteraction: true);
    }

    public virtual void bringIntoView(TextPosition position)
    {
        global::Doroti.Ui.Rect localRect = renderEditable.getLocalRectForCaret(position);
        global::Doroti.Framework.Rendering.RevealedOffset targetOffset = _getOffsetToRevealCaret(localRect);
        _scrollController.jumpTo(targetOffset.offset);
        renderEditable.showOnScreen(rect: targetOffset.rect);
    }

    public virtual void showToolbar()
    {
        if (_webContextMenuEnabled)
        {
            _ = false;
            return;
        }
        if (_selectionOverlay is null)
        {
            _ = false;
            return;
        }
        if (_selectionOverlay!.toolbarIsVisible)
        {
            _ = false;
            return;
        }
        DartRuntimePrimitives.Ignore(_liveTextInputStatus?.update());
        DartRuntimePrimitives.Ignore(clipboardStatus.update());
        _selectionOverlay!.showToolbar();
        if (_platformSupportsFadeOnScroll)
        {
            _listeningToScrollNotificationObserver = true;
            _scrollNotificationObserver?.removeListener(_handleContextMenuOnParentScroll);
            _scrollNotificationObserver = ScrollNotificationObserver.maybeOf(context);
            _scrollNotificationObserver?.addListener(_handleContextMenuOnParentScroll);
        }
        _ = true;
        return;
    }

    public virtual void hideToolbar(bool hideHandles = true)
    {
        _disposeScrollNotificationObserver();
        if (hideHandles)
        {
            _selectionOverlay?.hide();
        }
        else
        {
            if (_selectionOverlay?.toolbarIsVisible ?? false)
            {
                _selectionOverlay?.hideToolbar();
            }
        }
    }

    public virtual void toggleToolbar(bool hideHandles = true)
    {
        TextSelectionOverlay selectionOverlay = _selectionOverlay ??= _createSelectionOverlay();
        if (selectionOverlay.toolbarIsVisible)
        {
            hideToolbar(hideHandles);
        }
        else
        {
            showToolbar();
        }
    }

    public virtual bool showSpellCheckSuggestionsToolbar()
    {
        if (!spellCheckEnabled || _webContextMenuEnabled || widget.readOnly || (_selectionOverlay is null) || !_spellCheckResultsReceived || (findSuggestionSpanAtCursorIndex(textEditingValue.selection.extentOffset) is null))
        {
            return false;
        }
        DartRuntimePrimitives.Assert(() => _spellCheckConfiguration.spellCheckSuggestionsToolbarBuilder is not null, () => (object?)"spellCheckSuggestionsToolbarBuilder must be defined in " + "SpellCheckConfiguration to show a toolbar with spell check " + "suggestions");
        _selectionOverlay!.showSpellCheckSuggestionsToolbar((context) =>
        {
            return _spellCheckConfiguration.spellCheckSuggestionsToolbarBuilder!(context, this);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void showMagnifier(Offset positionToShow)
    {
        if (_selectionOverlay is null)
        {
            return;
        }
        if (_selectionOverlay!.magnifierExists)
        {
            _selectionOverlay!.updateMagnifier(positionToShow);
        }
        else
        {
            _selectionOverlay!.showMagnifier(positionToShow);
        }
    }

    public virtual void hideMagnifier()
    {
        if (_selectionOverlay is null)
        {
            return;
        }
        _selectionOverlay!.hideMagnifier();
    }

    public virtual void insertTextPlaceholder(Size size)
    {
        if (!_stylusHandwritingEnabled)
        {
            return;
        }
        if (!widget.controller.selection.isValid)
        {
            return;
        }
        setState(() =>
        {
            _placeholderLocation = _value.text.Length - widget.controller.selection.end;
        });
    }

    public virtual void removeTextPlaceholder()
    {
        if (!_stylusHandwritingEnabled || (_placeholderLocation == -1L))
        {
            return;
        }
        setState(() =>
        {
            _placeholderLocation = -1L;
        });
    }

    public virtual void performSelector(string selectorName)
    {
        Intent? intent = Default_text_editing_shortcutsLibrary.intentForMacOSSelector(selectorName);
        if (intent is not null)
        {
            BuildContext? primaryContext = Focus_managerLibrary.primaryFocus?.context;
            if (primaryContext is not null)
            {
                Actions.invoke(primaryContext, intent);
            }
        }
    }

    public virtual string autofillId => $"EditableText-{GetHashCode()}";
    public virtual global::Doroti.Framework.Services.TextInputConfiguration textInputConfiguration
    {
        get
        {
            List<string>? autofillHintsLocal = widget.autofillHints?.ToList().ToList();
            global::Doroti.Framework.Services.AutofillConfiguration autofillConfigurationLocal = (autofillHintsLocal is not null) ? new global::Doroti.Framework.Services.AutofillConfiguration(uniqueIdentifier: autofillId, autofillHints: autofillHintsLocal, currentEditingValue: currentTextEditingValue) : AutofillConfiguration.disabled;
            _viewId = checked((long)View.of(context).viewId);
            return new global::Doroti.Framework.Services.TextInputConfiguration(viewId: _viewId, inputType: widget.keyboardType, readOnly: widget.readOnly, obscureText: widget.obscureText, autocorrect: widget.autocorrect, smartDashesType: widget.smartDashesType, smartQuotesType: widget.smartQuotesType, enableSuggestions: widget.enableSuggestions, enableInteractiveSelection: widget._userSelectionEnabled, inputAction: widget.textInputAction ?? (Equals(widget.keyboardType, TextInputType.multiline) ? TextInputAction.newline : TextInputAction.done), textCapitalization: widget.textCapitalization, keyboardAppearance: widget.keyboardAppearance, autofillConfiguration: autofillConfigurationLocal, enableIMEPersonalizedLearning: widget.enableIMEPersonalizedLearning, allowedMimeTypes: (widget.contentInsertionConfiguration is null) ? new List<string>() : widget.contentInsertionConfiguration!.allowedMimeTypes, hintLocales: widget.hintLocales, enableInlinePrediction: widget.enableInlinePrediction);
        }
    }
    public virtual void autofill(global::Doroti.Framework.Services.TextEditingValue newEditingValue) => updateEditingValue(newEditingValue);
    public virtual void showAutocorrectionPromptRect(long start, long end)
    {
        setState(() =>
        {
            _currentPromptRectRange = new global::Doroti.Ui.TextRange(start: start, end: end);
        });
    }

    internal virtual global::System.Action? _semanticsOnCopy(TextSelectionControls? controls)
    {
        return (widget.selectionEnabled && _hasFocus && ((widget.selectionControls is TextSelectionHandleControls) ? copyEnabled : (copyEnabled && (widget.selectionControls?.canCopy(this) ?? false)))) ? (() =>
        {
            controls?.handleCopy(this);
            copySelection(SelectionChangedCause.toolbar);
        }) : null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::System.Action? _semanticsOnCut(TextSelectionControls? controls)
    {
        return (widget.selectionEnabled && _hasFocus && ((widget.selectionControls is TextSelectionHandleControls) ? cutEnabled : (cutEnabled && (widget.selectionControls?.canCut(this) ?? false)))) ? (() =>
        {
            controls?.handleCut(this);
            cutSelection(SelectionChangedCause.toolbar);
        }) : null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::System.Action? _semanticsOnPaste(TextSelectionControls? controls)
    {
        return DartRuntimePrimitives.AdaptAsyncCallback((widget.selectionEnabled && _hasFocus && ((widget.selectionControls is TextSelectionHandleControls) ? pasteEnabled : (pasteEnabled && (widget.selectionControls?.canPaste(this) ?? false))) && Equals(clipboardStatus.value, ClipboardStatus.pasteable)) ? (async () =>
        {
            if (controls is not null) await controls.handlePaste(this);
            await _pasteTextWithReporting(SelectionChangedCause.toolbar);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }) : null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.TextPosition _moveBeyondTextBoundary(TextPosition extent, bool forward, global::Doroti.Framework.Services.TextBoundary textBoundary)
    {
        DartRuntimePrimitives.Assert(() => extent.offset >= 0L);
        long newOffset = forward ? (textBoundary.getTrailingTextBoundaryAt(extent.offset) ?? _value.text.Length) : (textBoundary.getLeadingTextBoundaryAt(extent.offset - 1L) ?? 0L);
        return new global::Doroti.Ui.TextPosition(offset: newOffset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.TextPosition _moveToTextBoundary(TextPosition extent, bool forward, global::Doroti.Framework.Services.TextBoundary textBoundary)
    {
        DartRuntimePrimitives.Assert(() => extent.offset >= 0L);
        long caretOffset = default!;
        switch (extent.affinity)
        {
            case TextAffinity.upstream:
                {
                    if ((extent.offset < 1L) && !forward)
                    {
                        DartRuntimePrimitives.Assert(() => extent.offset == 0L);
                        return new global::Doroti.Ui.TextPosition(offset: 0L);
                    }
                    caretOffset = Math.Max(0L, extent.offset - 1L);
                    break;
                }
            case TextAffinity.downstream:
                {
                    caretOffset = extent.offset;
                    break;
                }
        }
        return forward ? new global::Doroti.Ui.TextPosition(offset: textBoundary.getTrailingTextBoundaryAt(caretOffset) ?? _value.text.Length, affinity: TextAffinity.upstream) : new global::Doroti.Ui.TextPosition(offset: textBoundary.getLeadingTextBoundaryAt(caretOffset) ?? 0L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Services.TextBoundary _characterBoundary() => widget.obscureText ? new _CodePointBoundary__editable_text(_value.text) : new global::Doroti.Framework.Services.CharacterBoundary(_value.text);
    internal virtual global::Doroti.Framework.Services.TextBoundary _nextWordBoundary() => widget.obscureText ? _documentBoundary() : renderEditable.wordBoundaries.moveByWordBoundary;
    internal virtual global::Doroti.Framework.Services.TextBoundary _linebreak() => widget.obscureText ? _documentBoundary() : new global::Doroti.Framework.Services.LineBoundary(renderEditable);
    internal virtual global::Doroti.Framework.Services.TextBoundary _paragraphBoundary() => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Services.TextBoundary>(new global::Doroti.Framework.Services.ParagraphBoundary(_value.text));
    internal virtual global::Doroti.Framework.Services.TextBoundary _documentBoundary() => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Services.TextBoundary>(new global::Doroti.Framework.Services.DocumentBoundary(_value.text));
    internal virtual Action<T> _makeOverridable<T>(Action<T> defaultAction) where T : Intent
    {
        return Action<T>.CreateOverridable(context: context, defaultAction: defaultAction);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _transposeCharacters(TransposeCharactersIntent intent)
    {
        if ((_value.text.characters().Count <= 1L) || !_value.selection.isCollapsed || (_value.selection.baseOffset == 0L))
        {
            return;
        }
        string textLocal = _value.text;
        global::Doroti.Framework.Services.TextSelection selectionLocal = _value.selection;
        var atEnd = selectionLocal.baseOffset == textLocal.Length;
        var transposing = new CharacterRange(textLocal, selectionLocal.baseOffset);
        if (atEnd)
        {
            transposing.moveBack(2L);
        }
        else
        {
            DartRuntimePrimitives.Ignore(((Func<CharacterRange>)(() =>
{
    var __cascade = transposing;
    __cascade.moveBack();
    __cascade.expandNext();
    return __cascade;
}))());
        }
        DartRuntimePrimitives.Assert(() => transposing.currentCharacters.Count == 2L);
        userUpdateTextEditingValue(new global::Doroti.Framework.Services.TextEditingValue(text: transposing.stringBefore + transposing.currentCharacters.last + transposing.currentCharacters.first + transposing.stringAfter, selection: TextSelection.CreateCollapsed(offset: transposing.stringBeforeLength + transposing.Current.Length)), SelectionChangedCause.keyboard);
    }

    internal virtual void _replaceText(ReplaceTextIntent intent)
    {
        global::Doroti.Framework.Services.TextEditingValue oldValue = _value;
        global::Doroti.Framework.Services.TextEditingValue newValue = intent.currentTextEditingValue.replaced(intent.replacementRange, intent.replacementText);
        userUpdateTextEditingValue(newValue, intent.cause);
        if (Equals(newValue, oldValue))
        {
            _didChangeTextEditingValue();
        }
    }

    internal virtual void _scrollToDocumentBoundary(ScrollToDocumentBoundaryIntent intent)
    {
        if (intent.forward)
        {
            bringIntoView(new global::Doroti.Ui.TextPosition(offset: _value.text.Length));
        }
        else
        {
            bringIntoView(new global::Doroti.Ui.TextPosition(offset: 0L));
        }
    }

    internal virtual void _scroll(ScrollIntent intent)
    {
        if (!Equals(intent.type, ScrollIncrementType.page))
        {
            return;
        }
        ScrollPosition positionLocal = _scrollController.position;
        if (widget.maxLines == 1L)
        {
            _scrollController.jumpTo(positionLocal.maxScrollExtent);
            return;
        }
        if ((positionLocal.maxScrollExtent == 0.0) && (positionLocal.minScrollExtent == 0.0))
        {
            return;
        }
        var state = ((ScrollableState?)_scrollableKey.currentState)!;
        double increment = ScrollAction.getDirectionalIncrement(DartRuntimePrimitives.RequireValue(state), intent);
        double destination = Dart_uiLibrary.clampDouble(positionLocal.pixels + increment, positionLocal.minScrollExtent, positionLocal.maxScrollExtent);
        if (destination == positionLocal.pixels)
        {
            return;
        }
        _scrollController.jumpTo(destination);
    }

    internal virtual void _updateSelection(UpdateSelectionIntent intent)
    {
        DartRuntimePrimitives.Assert(() => intent.newSelection.start <= intent.currentTextEditingValue.text.Length, () => (object?)$"invalid selection: {intent.newSelection}: it must not exceed the current text length {intent.currentTextEditingValue.text.Length}");
        DartRuntimePrimitives.Assert(() => intent.newSelection.end <= intent.currentTextEditingValue.text.Length, () => (object?)$"invalid selection: {intent.newSelection}: it must not exceed the current text length {intent.currentTextEditingValue.text.Length}");
        bringIntoView(intent.newSelection.extent);
        userUpdateTextEditingValue(intent.currentTextEditingValue.copyWith(selection: intent.newSelection), intent.cause);
    }

    internal virtual object? _hideToolbarIfVisible(DismissIntent intent)
    {
        if (_selectionOverlay?.toolbarIsVisible ?? false)
        {
            hideToolbar(false);
            return null;
        }
        return Actions.invoke(context, intent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _onTapOutside(BuildContext context, global::Doroti.Framework.Gestures.PointerDownEvent @event)
    {
        _hadFocusOnTapDown = true;
        if (widget.onTapOutside is not null)
        {
            widget.onTapOutside!(@event);
        }
        else
        {
            _defaultOnTapOutside(context, @event);
        }
    }

    internal virtual void _onTapUpOutside(BuildContext context, global::Doroti.Framework.Gestures.PointerUpEvent @event)
    {
        if (!_hadFocusOnTapDown)
        {
            return;
        }
        _hadFocusOnTapDown = false;
        if (widget.onTapUpOutside is not null)
        {
            widget.onTapUpOutside!(@event);
        }
        else
        {
            _defaultOnTapUpOutside(context, @event);
        }
    }

    internal virtual void _defaultOnTapOutside(BuildContext context, global::Doroti.Framework.Gestures.PointerDownEvent @event)
    {
        Actions.invoke(context, new EditableTextTapOutsideIntent(focusNode: widget.focusNode, pointerDownEvent: @event));
    }

    internal virtual void _defaultOnTapUpOutside(BuildContext context, global::Doroti.Framework.Gestures.PointerUpEvent @event)
    {
        Actions.invoke(context, new EditableTextTapUpOutsideIntent(focusNode: widget.focusNode, pointerUpEvent: @event));
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMediaQuery(context));
        if (wantKeepAlive && (_keepAliveHandle is null))
        {
            _ensureKeepAlive();
        }
        TextSelectionControls? controls = widget.selectionControls;
        global::Doroti.Framework.Painting.TextScaler effectiveTextScaler = (widget.textScaler, widget.textScaleFactor) switch { (global::Doroti.Framework.Painting.TextScaler textScalerLocal, _) => textScalerLocal, (null, double textScaleFactorLocal) => TextScaler.CreateLinear(textScaleFactorLocal), (null, null) => MediaQuery.textScalerOf(context) };
        double? lineHeightScaleFactorLocal = MediaQuery.maybeLineHeightScaleFactorOverrideOf(context);
        double? letterSpacingLocal = MediaQuery.maybeLetterSpacingOverrideOf(context);
        double? wordSpacingLocal = MediaQuery.maybeWordSpacingOverrideOf(context);
        global::Doroti.Ui.SemanticsInputType inputTypeLocal = default!;
        switch (widget.keyboardType)
        {
            case var __constant235617 when Equals(__constant235617, TextInputType.phone):
                {
                    inputTypeLocal = SemanticsInputType.phone;
                    break;
                }
            case var __constant235698 when Equals(__constant235698, TextInputType.url):
                {
                    inputTypeLocal = SemanticsInputType.url;
                    break;
                }
            case var __constant235775 when Equals(__constant235775, TextInputType.emailAddress):
                {
                    inputTypeLocal = SemanticsInputType.email;
                    break;
                }
            default:
                {
                    inputTypeLocal = SemanticsInputType.text;
                    break;
                }
        }
        return new _CompositionCallback__editable_text(compositeCallback: _compositeCallback, enabled: _hasInputConnection, child: new Actions(actions: _actions, child: new Builder(builder: (context) =>
        {
            return new TextFieldTapRegion(groupId: widget.groupId, onTapOutside: _hasFocus ? ((@event) => { _onTapOutside(context, @event); }) : null, onTapUpOutside: (@event) => { _onTapUpOutside(context, @event); }, debugLabel: Foundation.ConstantsLibrary.kReleaseMode ? null : "EditableText", child: new MouseRegion(cursor: widget.mouseCursor ?? SystemMouseCursors.text, child: new UndoHistory<global::Doroti.Framework.Services.TextEditingValue>(value: widget.controller, onTriggered: (value) =>
            {
                userUpdateTextEditingValue(value, SelectionChangedCause.keyboard);
            }, shouldChangeUndoStack: (oldValue, newValue) =>
            {
                if (!newValue.selection.isValid)
                {
                    return false;
                }
                if (oldValue is null)
                {
                    return true;
                }
                switch (PlatformLibrary.defaultTargetPlatform)
                {
                    case TargetPlatform.iOS:
                    case TargetPlatform.macOS:
                    case TargetPlatform.fuchsia:
                    case TargetPlatform.linux:
                    case TargetPlatform.windows:
                        {
                            if (!widget.controller.value.composing.isCollapsed)
                            {
                                return false;
                            }
                            break;
                        }
                    case TargetPlatform.android:
                        {
                            break;
                        }
                }
                return (oldValue.text != newValue.text) || (!Equals(oldValue.composing, newValue.composing));
                throw new InvalidOperationException("Dart closure completed without a value.");
            }, undoStackModifier: (value) =>
            {
                return Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.android) ? value.copyWith(composing: TextRange.empty) : value;
                throw new InvalidOperationException("Dart closure completed without a value.");
            }, focusNode: widget.focusNode, controller: widget.undoController, child: new Focus(focusNode: widget.focusNode, includeSemantics: false, debugLabel: Foundation.ConstantsLibrary.kReleaseMode ? null : "EditableText", child: new NotificationListener<ScrollNotification>(onNotification: (notification) =>
            {
                _handleContextMenuOnScroll(notification);
                _scribbleCacheKey = null;
                return false;
                throw new InvalidOperationException("Dart closure completed without a value.");
            }, child: new Scrollable(key: _scrollableKey, excludeFromSemantics: true, axisDirection: _isMultiline ? AxisDirection.down : AxisDirection.right, controller: _scrollController, physics: widget.scrollPhysics ?? ((!_isMultiline && Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.iOS)) ? new _NeverUserScrollableScrollPhysics__editable_text() : null), dragStartBehavior: widget.dragStartBehavior, restorationId: widget.restorationId, scrollBehavior: widget.scrollBehavior ?? ScrollConfiguration.of(context).copyWith(scrollbars: _isMultiline, overscroll: false), viewportBuilder: (context, offset) =>
            {
                return new CompositedTransformTarget(link: _toolbarLayerLink, child: new Semantics(inputType: inputTypeLocal, onCopy: _semanticsOnCopy(controls), onCut: _semanticsOnCut(controls), onPaste: _semanticsOnPaste(controls), child: new _ScribbleFocusable__editable_text(editableKey: _editableKey, enabled: _stylusHandwritingEnabled, focusNode: widget.focusNode, updateSelectionRects: () =>
                {
                    _openInputConnection();
                    _updateSelectionRects(force: true);
                }, child: new SizeChangedLayoutNotifier(child: new _Editable__editable_text(key: _editableKey, startHandleLayerLink: _startHandleLayerLink, endHandleLayerLink: _endHandleLayerLink, inlineSpan: _OverridingTextStyleTextSpanUtils__editable_text.applyTextSpacingOverrides(lineHeightScaleFactor: lineHeightScaleFactorLocal, letterSpacing: letterSpacingLocal, wordSpacing: wordSpacingLocal, textSpan: buildTextSpan()), value: _value, cursorColor: _cursorColor, backgroundCursorColor: widget.backgroundCursorColor, showCursor: _cursorVisibilityNotifier, forceLine: widget.forceLine, readOnly: widget.readOnly, hasFocus: _hasFocus, maxLines: widget.maxLines, minLines: widget.minLines, expands: widget.expands, strutStyle: widget.strutStyle.merge(new global::Doroti.Framework.Painting.StrutStyle(height: lineHeightScaleFactorLocal)), selectionColor: (_selectionOverlay?.spellCheckToolbarIsVisible ?? false) ? (_spellCheckConfiguration.misspelledSelectionColor ?? widget.selectionColor) : widget.selectionColor, textScaler: effectiveTextScaler, textAlign: widget.textAlign, textDirection: _textDirection, locale: widget.locale, textHeightBehavior: widget.textHeightBehavior ?? DefaultTextHeightBehavior.maybeOf(context), textWidthBasis: widget.textWidthBasis, obscuringCharacter: widget.obscuringCharacter, obscureText: widget.obscureText, offset: offset, rendererIgnoresPointer: widget.rendererIgnoresPointer, cursorWidth: widget.cursorWidth, cursorHeight: widget.cursorHeight, cursorRadius: widget.cursorRadius, cursorOffset: widget.cursorOffset ?? Offset.zero, selectionHeightStyle: widget.selectionHeightStyle, selectionWidthStyle: widget.selectionWidthStyle, paintCursorAboveText: widget.paintCursorAboveText, enableInteractiveSelection: widget._userSelectionEnabled, textSelectionDelegate: this, devicePixelRatio: _devicePixelRatio, promptRectRange: _currentPromptRectRange, promptRectColor: widget.autocorrectionTextRectColor, clipBehavior: widget.clipBehavior)))));
                throw new InvalidOperationException("Dart closure completed without a value.");
            }))))));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Painting.TextSpan buildTextSpan()
    {
        if (widget.obscureText)
        {
            string textLocal = _value.text;
            textLocal = DartCoreExtensions.repeat(widget.obscuringCharacter, textLocal.Length);
            var mobilePlatforms = new HashSet<global::Doroti.Framework.Foundation.TargetPlatform> { TargetPlatform.android, TargetPlatform.fuchsia, TargetPlatform.iOS };
            bool brieflyShowPasswordLocal = WidgetsBinding.instance.platformDispatcher.brieflyShowPassword && mobilePlatforms.Contains(PlatformLibrary.defaultTargetPlatform);
            if (brieflyShowPasswordLocal)
            {
                long? o = (_obscureShowCharTicksPending > 0L) ? _obscureLatestCharIndex : null;
                if ((o is not null) && (o >= 0L) && (DartRuntimePrimitives.RequireValue(o) < textLocal.Length))
                {
                    long o__246733__value246816 = DartRuntimePrimitives.RequireValue(o);
                    textLocal = textLocal.replaceRange(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(o__246733__value246816)), DartRuntimePrimitives.RequireValue(o__246733__value246816) + 1L, _value.text.substring(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(o__246733__value246816)), DartRuntimePrimitives.RequireValue(o__246733__value246816) + 1L));
                }
            }
            return new global::Doroti.Framework.Painting.TextSpan(style: _style, text: textLocal);
        }
        if ((_placeholderLocation >= 0L) && (_placeholderLocation <= _value.text.Length))
        {
            var placeholders = new List<_ScribblePlaceholder__editable_text>();
            long placeholderLocation = _value.text.Length - _placeholderLocation;
            if (_isMultiline)
            {
                placeholders.Add(new _ScribblePlaceholder__editable_text(child: SizedBox.CreateShrink(), size: Size.zero));
                placeholders.Add(new _ScribblePlaceholder__editable_text(child: SizedBox.CreateShrink(), size: new global::Doroti.Ui.Size(renderEditable.size.width, 0.0)));
            }
            else
            {
                placeholders.Add(new _ScribblePlaceholder__editable_text(child: SizedBox.CreateShrink(), size: new global::Doroti.Ui.Size(100.0, 0.0)));
            }
            return new global::Doroti.Framework.Painting.TextSpan(style: _style, children: new List<global::Doroti.Framework.Painting.InlineSpan> { new global::Doroti.Framework.Painting.TextSpan(text: _value.text.substring(0L, placeholderLocation)), new global::Doroti.Framework.Painting.TextSpan(text: _value.text.substring(placeholderLocation)) });
        }
        bool withComposingLocal = !widget.readOnly && _hasFocus;
        if (_spellCheckResultsReceived)
        {
            DartRuntimePrimitives.Assert(() => !_value.composing.isValid || !withComposingLocal || _value.isComposingRangeValid);
            bool composingRegionOutOfRange = !_value.isComposingRangeValid || !withComposingLocal;
            return Spell_checkLibrary.buildTextSpanWithSpellCheckSuggestions(_value, composingRegionOutOfRange, _style, _spellCheckConfiguration.misspelledTextStyle!, spellCheckResults!);
        }
        return widget.controller.buildTextSpan(context: context, style: _style, withComposing: withComposingLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _ensureKeepAlive()
    {
        DartRuntimePrimitives.Assert(() => _keepAliveHandle is null);
        _keepAliveHandle = new KeepAliveHandle();
        new KeepAliveNotification(_keepAliveHandle!).dispatch(context);
    }

    public virtual void _releaseKeepAlive()
    {
        _keepAliveHandle!.dispose();
        _keepAliveHandle = null;
    }

    public virtual void updateKeepAlive()
    {
        if (wantKeepAlive)
        {
            if (_keepAliveHandle is null)
            {
                _ensureKeepAlive();
            }
        }
        else
        {
            if (_keepAliveHandle is not null)
            {
                _releaseKeepAlive();
            }
        }
    }

    public override void deactivate()
    {
        if (_keepAliveHandle is not null)
        {
            _releaseKeepAlive();
        }
        base.deactivate();
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = ((Func<_WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider(onTick, this, debugLabel: Foundation.ConstantsLibrary.kDebugMode ? $"created by {DiagnosticsLibrary.describeIdentity(this)}" : null);
    __cascade.muted = !values.enabled;
    __cascade.forceFrames = values.forceFrames;
    return __cascade;
}))();
        _tickers!.Add(result);
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(_WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => _tickers is not null);
        DartRuntimePrimitives.Assert(() => _tickers!.Contains(ticker));
        _tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if (_tickers is not null)
        {
            TickerModeData values = _tickerModeNotifier!.value;
            bool mutedLocal = !values.enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", _tickers, description: (_tickers is not null) ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}" : null, defaultValue: default));
    }

}

internal class _Editable__editable_text : MultiChildRenderObjectWidget
{
    public virtual global::Doroti.Framework.Painting.InlineSpan inlineSpan { get; private set; } = default!;
    public virtual global::Doroti.Framework.Services.TextEditingValue value { get; private set; } = default!;
    public virtual Color? cursorColor { get; private set; }
    public virtual global::Doroti.Framework.Rendering.LayerLink startHandleLayerLink { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.LayerLink endHandleLayerLink { get; private set; } = default!;
    public virtual Color? backgroundCursorColor { get; private set; }
    public virtual global::Doroti.Framework.Foundation.ValueNotifier<bool> showCursor { get; private set; } = default!;
    public virtual bool forceLine { get; private set; } = default!;
    public virtual bool readOnly { get; private set; } = default!;
    public virtual bool hasFocus { get; private set; } = default!;
    public virtual long? maxLines { get; private set; }
    public virtual long? minLines { get; private set; }
    public virtual bool expands { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.StrutStyle? strutStyle { get; private set; }
    public virtual Color? selectionColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextScaler textScaler { get; private set; } = default!;
    public virtual TextAlign textAlign { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;
    public virtual Locale? locale { get; private set; }
    public virtual string obscuringCharacter { get; private set; } = default!;
    public virtual bool obscureText { get; private set; } = default!;
    public virtual TextHeightBehavior? textHeightBehavior { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextWidthBasis textWidthBasis { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.ViewportOffset offset { get; private set; } = default!;
    public virtual bool rendererIgnoresPointer { get; private set; } = default!;
    public virtual double cursorWidth { get; private set; } = default!;
    public virtual double? cursorHeight { get; private set; }
    public virtual Radius? cursorRadius { get; private set; }
    public virtual Offset cursorOffset { get; private set; } = default!;
    public virtual bool paintCursorAboveText { get; private set; } = default!;
    public virtual BoxHeightStyle selectionHeightStyle { get; private set; } = default!;
    public virtual BoxWidthStyle selectionWidthStyle { get; private set; } = default!;
    public virtual bool enableInteractiveSelection { get; private set; } = default!;
    public virtual global::Doroti.Framework.Services.TextSelectionDelegate textSelectionDelegate { get; private set; } = default!;
    public virtual double devicePixelRatio { get; private set; } = default!;
    public virtual TextRange? promptRectRange { get; private set; }
    public virtual Color? promptRectColor { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;

    internal _Editable__editable_text(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.InlineSpan inlineSpan = default!, global::Doroti.Framework.Services.TextEditingValue value = default!, global::Doroti.Framework.Rendering.LayerLink startHandleLayerLink = default!, global::Doroti.Framework.Rendering.LayerLink endHandleLayerLink = default!, Color? cursorColor = null, Color? backgroundCursorColor = null, global::Doroti.Framework.Foundation.ValueNotifier<bool> showCursor = default!, bool forceLine = default!, bool readOnly = default!, TextHeightBehavior? textHeightBehavior = null, global::Doroti.Framework.Painting.TextWidthBasis textWidthBasis = default!, bool hasFocus = default!, long? maxLines = default!, long? minLines = null, bool expands = default!, global::Doroti.Framework.Painting.StrutStyle? strutStyle = null, Color? selectionColor = null, global::Doroti.Framework.Painting.TextScaler textScaler = default!, TextAlign textAlign = default!, TextDirection textDirection = default!, Locale? locale = null, string obscuringCharacter = default!, bool obscureText = default!, global::Doroti.Framework.Rendering.ViewportOffset offset = default!, bool rendererIgnoresPointer = false, double cursorWidth = default!, double? cursorHeight = null, Radius? cursorRadius = null, Offset cursorOffset = default!, bool paintCursorAboveText = default!, BoxHeightStyle? selectionHeightStyle = null, BoxWidthStyle? selectionWidthStyle = null, bool enableInteractiveSelection = true, global::Doroti.Framework.Services.TextSelectionDelegate textSelectionDelegate = default!, double devicePixelRatio = default!, TextRange? promptRectRange = null, Color? promptRectColor = null, Clip clipBehavior = default!) : base(key: key, children: WidgetSpan.extractFromInlineSpan(inlineSpan, textScaler))
    {
        this.inlineSpan = inlineSpan;
        this.value = value;
        this.startHandleLayerLink = startHandleLayerLink;
        this.endHandleLayerLink = endHandleLayerLink;
        this.cursorColor = cursorColor;
        this.backgroundCursorColor = backgroundCursorColor;
        this.showCursor = showCursor;
        this.forceLine = forceLine;
        this.readOnly = readOnly;
        this.textHeightBehavior = textHeightBehavior;
        this.textWidthBasis = textWidthBasis;
        this.hasFocus = hasFocus;
        this.maxLines = maxLines;
        this.minLines = minLines;
        this.expands = expands;
        this.strutStyle = strutStyle;
        this.selectionColor = selectionColor;
        this.textScaler = textScaler;
        this.textAlign = textAlign;
        this.textDirection = textDirection;
        this.locale = locale;
        this.obscuringCharacter = obscuringCharacter;
        this.obscureText = obscureText;
        this.offset = offset;
        this.rendererIgnoresPointer = rendererIgnoresPointer;
        this.cursorWidth = cursorWidth;
        this.cursorHeight = cursorHeight;
        this.cursorRadius = cursorRadius;
        this.cursorOffset = cursorOffset;
        this.paintCursorAboveText = paintCursorAboveText;
        this.enableInteractiveSelection = enableInteractiveSelection;
        this.textSelectionDelegate = textSelectionDelegate;
        this.devicePixelRatio = devicePixelRatio;
        this.promptRectRange = promptRectRange;
        this.promptRectColor = promptRectColor;
        this.clipBehavior = clipBehavior;
        this.selectionHeightStyle = selectionHeightStyle ?? EditableText.defaultSelectionHeightStyle;
        this.selectionWidthStyle = selectionWidthStyle ?? EditableText.defaultSelectionWidthStyle;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return new global::Doroti.Framework.Rendering.RenderEditable(text: inlineSpan, cursorColor: cursorColor, startHandleLayerLink: startHandleLayerLink, endHandleLayerLink: endHandleLayerLink, backgroundCursorColor: backgroundCursorColor, showCursor: showCursor, forceLine: forceLine, readOnly: readOnly, hasFocus: hasFocus, maxLines: maxLines, minLines: minLines, expands: expands, strutStyle: strutStyle, selectionColor: selectionColor, textScaler: textScaler, textAlign: textAlign, textDirection: textDirection, locale: locale ?? Localizations.maybeLocaleOf(context), selection: value.selection, offset: offset, ignorePointer: rendererIgnoresPointer, obscuringCharacter: obscuringCharacter, obscureText: obscureText, textHeightBehavior: textHeightBehavior, textWidthBasis: textWidthBasis, cursorWidth: cursorWidth, cursorHeight: cursorHeight, cursorRadius: cursorRadius, cursorOffset: cursorOffset, paintCursorAboveText: paintCursorAboveText, selectionHeightStyle: DartRuntimePrimitives.RequireValue(selectionHeightStyle), selectionWidthStyle: DartRuntimePrimitives.RequireValue(selectionWidthStyle), enableInteractiveSelection: enableInteractiveSelection, textSelectionDelegate: textSelectionDelegate, devicePixelRatio: devicePixelRatio, promptRectRange: promptRectRange, promptRectColor: promptRectColor, clipBehavior: clipBehavior);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderEditable)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Rendering.RenderEditable>)(() =>
{
    var __cascade = __renderObject;
    __cascade.text = inlineSpan;
    __cascade.cursorColor = cursorColor;
    __cascade.startHandleLayerLink = startHandleLayerLink;
    __cascade.endHandleLayerLink = endHandleLayerLink;
    __cascade.backgroundCursorColor = backgroundCursorColor;
    __cascade.showCursor = showCursor;
    __cascade.forceLine = forceLine;
    __cascade.readOnly = readOnly;
    __cascade.hasFocus = hasFocus;
    __cascade.maxLines = maxLines;
    __cascade.minLines = minLines;
    __cascade.expands = expands;
    __cascade.strutStyle = strutStyle;
    __cascade.selectionColor = selectionColor;
    __cascade.textScaler = textScaler;
    __cascade.textAlign = textAlign;
    __cascade.textDirection = textDirection;
    __cascade.locale = locale ?? Localizations.maybeLocaleOf(context);
    __cascade.selection = value.selection;
    __cascade.offset = offset;
    __cascade.ignorePointer = rendererIgnoresPointer;
    __cascade.textHeightBehavior = textHeightBehavior;
    __cascade.textWidthBasis = textWidthBasis;
    __cascade.obscuringCharacter = obscuringCharacter;
    __cascade.obscureText = obscureText;
    __cascade.cursorWidth = cursorWidth;
    __cascade.setCursorHeight(cursorHeight);
    __cascade.cursorRadius = cursorRadius;
    __cascade.cursorOffset = cursorOffset;
    __cascade.selectionHeightStyle = selectionHeightStyle;
    __cascade.selectionWidthStyle = selectionWidthStyle;
    __cascade.enableInteractiveSelection = enableInteractiveSelection;
    __cascade.textSelectionDelegate = textSelectionDelegate;
    __cascade.devicePixelRatio = devicePixelRatio;
    __cascade.paintCursorAboveText = paintCursorAboveText;
    __cascade.promptRectColor = promptRectColor;
    __cascade.clipBehavior = clipBehavior;
    __cascade.setPromptRectRange(promptRectRange);
    return __cascade;
}))());
    }

}

internal class _NeverUserScrollableScrollPhysics__editable_text : ScrollPhysics
{
    internal _NeverUserScrollableScrollPhysics__editable_text(ScrollPhysics? parent = null) : base(parent: parent)
    {
    }

    public override _NeverUserScrollableScrollPhysics__editable_text applyTo(ScrollPhysics? ancestor)
    {
        return new _NeverUserScrollableScrollPhysics__editable_text(parent: buildParent(ancestor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool allowUserScrolling => false;
}

internal class _ScribbleCacheKey__editable_text
{
    public virtual TextAlign textAlign { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextScaler textScaler { get; private set; } = default!;
    public virtual TextHeightBehavior? textHeightBehavior { get; private set; }
    public virtual Locale? locale { get; private set; }
    public virtual global::Doroti.Framework.Painting.StrutStyle structStyle { get; private set; } = default!;
    public virtual long placeholder { get; private set; } = default!;
    public virtual Size size { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.InlineSpan inlineSpan { get; private set; } = default!;

    internal _ScribbleCacheKey__editable_text(global::Doroti.Framework.Painting.InlineSpan inlineSpan, TextAlign textAlign, TextDirection textDirection, global::Doroti.Framework.Painting.TextScaler textScaler, TextHeightBehavior? textHeightBehavior, Locale? locale, global::Doroti.Framework.Painting.StrutStyle structStyle, long placeholder, Size size)
    {
        this.inlineSpan = inlineSpan;
        this.textAlign = textAlign;
        this.textDirection = textDirection;
        this.textScaler = textScaler;
        this.textHeightBehavior = textHeightBehavior;
        this.locale = locale;
        this.structStyle = structStyle;
        this.placeholder = placeholder;
        this.size = size;
    }

    public virtual global::Doroti.Framework.Painting.RenderComparison compare(_ScribbleCacheKey__editable_text other)
    {
        if (DartRuntimePrimitives.Identical(other, this))
        {
            return RenderComparison.identical;
        }
        bool needsLayout = (!Equals(textAlign, other.textAlign)) || (!Equals(textDirection, other.textDirection)) || (!Equals(textScaler, other.textScaler)) || (!Equals(textHeightBehavior ?? new global::Doroti.Ui.TextHeightBehavior(), other.textHeightBehavior ?? new global::Doroti.Ui.TextHeightBehavior())) || (!Equals(locale, other.locale)) || (!Equals(structStyle, other.structStyle)) || (placeholder != other.placeholder) || (!Equals(size, other.size));
        return needsLayout ? RenderComparison.layout : inlineSpan.compareTo(other.inlineSpan);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _ScribbleFocusable__editable_text : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual FocusNode focusNode { get; private set; } = default!;
    public virtual GlobalKey<IState> editableKey { get; private set; } = default!;
    public virtual global::System.Action updateSelectionRects { get; private set; } = default!;
    public virtual bool enabled { get; private set; } = default!;

    internal _ScribbleFocusable__editable_text(Widget child, FocusNode focusNode, GlobalKey<IState> editableKey, global::System.Action updateSelectionRects, bool enabled)
    {
        this.child = child;
        this.focusNode = focusNode;
        this.editableKey = editableKey;
        this.updateSelectionRects = updateSelectionRects;
        this.enabled = enabled;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ScribbleFocusableState__editable_text());
}

public class _ScribbleFocusableState__editable_text : State<_ScribbleFocusable__editable_text>, global::Doroti.Framework.Services.ScribbleClient
{
    internal static long _nextElementIdentifier = 1L;
    internal virtual string _elementIdentifier { get; private set; } = default!;

    internal _ScribbleFocusableState__editable_text()
    {
        _elementIdentifier = _nextElementIdentifier++.ToString();
    }

    public override void initState()
    {
        base.initState();
        if (widget.enabled)
        {
            TextInput.registerScribbleElement(elementIdentifier, this);
        }
    }

    public override void didUpdateWidget(_ScribbleFocusable__editable_text oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!oldWidget.enabled && widget.enabled)
        {
            TextInput.registerScribbleElement(elementIdentifier, this);
        }
        if (oldWidget.enabled && !widget.enabled)
        {
            TextInput.unregisterScribbleElement(elementIdentifier);
        }
    }

    public override void dispose()
    {
        TextInput.unregisterScribbleElement(elementIdentifier);
        base.dispose();
    }

    public virtual global::Doroti.Framework.Rendering.RenderEditable? renderEditable => ((global::Doroti.Framework.Rendering.RenderEditable?)widget.editableKey.currentContext?.findRenderObject())!;
    public virtual string elementIdentifier => _elementIdentifier;
    public virtual void onScribbleFocus(Offset offset)
    {
        widget.focusNode.requestFocus();
        renderEditable?.selectPositionAt(from: offset, cause: SelectionChangedCause.stylusHandwriting);
        widget.updateSelectionRects();
    }

    public virtual bool isInScribbleRect(Rect rect)
    {
        global::Doroti.Ui.Rect calculatedBounds = bounds;
        if (renderEditable?.readOnly ?? false)
        {
            return false;
        }
        if (Equals(calculatedBounds, Rect.zero))
        {
            return false;
        }
        if (!calculatedBounds.overlaps(rect))
        {
            return false;
        }
        global::Doroti.Ui.Rect intersection = calculatedBounds.intersect(rect);
        var result = new global::Doroti.Framework.Gestures.HitTestResult();
        WidgetsBinding.instance.hitTestInView(result, intersection.center, checked((long)View.of(context).viewId));
        return result.path.any((entry) => Equals(entry.target, renderEditable));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Rect bounds
    {
        get
        {
            var box = ((global::Doroti.Framework.Rendering.RenderBox?)context.findRenderObject())!;
            if ((box is null) || !mounted || !box.attached)
            {
                return Rect.zero;
            }
            Matrix4 transform = box.getTransformTo(null);
            return MatrixUtils.transformRect(transform, Rect.fromLTWH(0, 0, box.size.width, box.size.height));
        }
    }
    public override Widget build(BuildContext context)
    {
        return widget.child;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ScribblePlaceholder__editable_text : WidgetSpan
{
    public virtual Size size { get; private set; } = default!;

    internal _ScribblePlaceholder__editable_text(Widget child, Size size) : base(child: child)
    {
        this.size = size;
    }

    public override void build(ParagraphBuilder builder, global::Doroti.Framework.Painting.TextScaler textScaler = default!, List<global::Doroti.Framework.Painting.PlaceholderDimensions>? dimensions = null)
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        var hasStyle = style is not null;
        if (hasStyle)
        {
            builder.pushStyle(style!.getTextStyle(textScaler: textScaler));
        }
        builder.addPlaceholder(size.width, size.height, alignment);
        if (hasStyle)
        {
            builder.pop();
        }
    }

}

internal class _CodePointBoundary__editable_text : global::Doroti.Framework.Services.TextBoundary
{
    internal virtual string _text { get; private set; } = default!;

    internal _CodePointBoundary__editable_text(string _text)
    {
        this._text = _text;
    }

    internal virtual bool _breaksSurrogatePair(long position)
    {
        DartRuntimePrimitives.Assert(() => (position > 0L) && (position < _text.Length) && (_text.Length > 1L));
        return TextPainter.isHighSurrogate(_text.codeUnitAt(position - 1L)) && TextPainter.isLowSurrogate(_text.codeUnitAt(position));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long? getLeadingTextBoundaryAt(long position)
    {
        if ((_text.Length == 0) || (position < 0L))
        {
            return null;
        }
        if (position == 0L)
        {
            return 0L;
        }
        if (position >= _text.Length)
        {
            return _text.Length;
        }
        if (_text.Length <= 1L)
        {
            return position;
        }
        return _breaksSurrogatePair(position) ? (position - 1L) : position;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long? getTrailingTextBoundaryAt(long position)
    {
        if ((_text.Length == 0) || (position >= _text.Length))
        {
            return null;
        }
        if (position < 0L)
        {
            return 0L;
        }
        if (position == (_text.Length - 1L))
        {
            return _text.Length;
        }
        if (_text.Length <= 1L)
        {
            return position;
        }
        return _breaksSurrogatePair(position + 1L) ? (position + 2L) : (position + 1L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DeleteTextAction__editable_text<T> : ContextAction<T> where T : DirectionalTextEditingIntent
{
    public virtual EditableTextState state { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Framework.Services.TextBoundary> getTextBoundary { get; private set; } = default!;
    internal virtual global::System.Func<TextPosition, bool, global::Doroti.Framework.Services.TextBoundary, TextPosition> _applyTextBoundary { get; private set; } = default!;

    internal _DeleteTextAction__editable_text(EditableTextState state, global::System.Func<global::Doroti.Framework.Services.TextBoundary> getTextBoundary, global::System.Func<TextPosition, bool, global::Doroti.Framework.Services.TextBoundary, TextPosition> _applyTextBoundary)
    {
        this.state = state;
        this.getTextBoundary = getTextBoundary;
        this._applyTextBoundary = _applyTextBoundary;
    }

    internal virtual void _hideToolbarIfTextChanged(ReplaceTextIntent intent)
    {
        if ((state._selectionOverlay is null) || !state.selectionOverlay!.toolbarIsVisible)
        {
            return;
        }
        global::Doroti.Framework.Services.TextEditingValue oldValue = intent.currentTextEditingValue;
        global::Doroti.Framework.Services.TextEditingValue newValue = intent.currentTextEditingValue.replaced(intent.replacementRange, intent.replacementText);
        if (oldValue.text != newValue.text)
        {
            state.hideToolbar(false);
        }
    }

    public override object? invoke(T intent, BuildContext? context = null)
    {
        global::Doroti.Framework.Services.TextSelection selectionLocal = state._value.selection;
        if (!selectionLocal.isValid)
        {
            return null;
        }
        DartRuntimePrimitives.Assert(() => selectionLocal.isValid);
        global::Doroti.Framework.Services.TextBoundary atomicBoundary = state._characterBoundary();
        if (!selectionLocal.isCollapsed)
        {
            var range = new global::Doroti.Ui.TextRange(start: atomicBoundary.getLeadingTextBoundaryAt(selectionLocal.start) ?? state._value.text.Length, end: atomicBoundary.getTrailingTextBoundaryAt(selectionLocal.end - 1L) ?? 0L);
            var replaceTextIntent = new ReplaceTextIntent(state._value, "", range, SelectionChangedCause.keyboard);
            _hideToolbarIfTextChanged(replaceTextIntent);
            return Actions.invoke(context!, replaceTextIntent);
        }
        long target = _applyTextBoundary(selectionLocal.@base, intent.forward, getTextBoundary()).offset;
        global::Doroti.Ui.TextRange rangeToDelete = new global::Doroti.Framework.Services.TextSelection(baseOffset: intent.forward ? (atomicBoundary.getLeadingTextBoundaryAt(selectionLocal.baseOffset) ?? state._value.text.Length) : (atomicBoundary.getTrailingTextBoundaryAt(selectionLocal.baseOffset - 1L) ?? 0L), extentOffset: target);
        var replaceTextIntentLocal = new ReplaceTextIntent(state._value, "", rangeToDelete, SelectionChangedCause.keyboard);
        _hideToolbarIfTextChanged(replaceTextIntentLocal);
        return Actions.invoke(context!, replaceTextIntentLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isActionEnabled => DartRuntimePrimitives.ConvertValue<bool>(!state.widget.readOnly && state._value.selection.isValid);
}

internal class _UpdateTextSelectionAction__editable_text<T> : ContextAction<T> where T : DirectionalCaretMovementIntent
{
    public virtual EditableTextState state { get; private set; } = default!;
    public virtual bool ignoreNonCollapsedSelection { get; private set; } = default!;
    public virtual bool isExpand { get; private set; } = default!;
    public virtual bool extentAtIndex { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Framework.Services.TextBoundary> getTextBoundary { get; private set; } = default!;
    public virtual global::System.Func<TextPosition, bool, global::Doroti.Framework.Services.TextBoundary, TextPosition> applyTextBoundary { get; private set; } = default!;
    public const long NEWLINE_CODE_UNIT = 10L;

    internal _UpdateTextSelectionAction__editable_text(EditableTextState state, global::System.Func<global::Doroti.Framework.Services.TextBoundary> getTextBoundary, global::System.Func<TextPosition, bool, global::Doroti.Framework.Services.TextBoundary, TextPosition> applyTextBoundary, bool ignoreNonCollapsedSelection, bool isExpand = false, bool extentAtIndex = false)
    {
        this.state = state;
        this.getTextBoundary = getTextBoundary;
        this.applyTextBoundary = applyTextBoundary;
        this.ignoreNonCollapsedSelection = ignoreNonCollapsedSelection;
        this.isExpand = isExpand;
        this.extentAtIndex = extentAtIndex;
    }

    internal virtual bool _isAtWordwrapUpstream(TextPosition position)
    {
        var endLocal = new global::Doroti.Ui.TextPosition(offset: state.renderEditable.getLineAtOffset(position).end, affinity: TextAffinity.upstream);
        return Equals(endLocal, position) && (endLocal.offset != state.textEditingValue.text.Length) && (state.textEditingValue.text.codeUnitAt(position.offset) != NEWLINE_CODE_UNIT);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isAtWordwrapDownstream(TextPosition position)
    {
        var startLocal = new global::Doroti.Ui.TextPosition(offset: state.renderEditable.getLineAtOffset(position).start);
        return Equals(startLocal, position) && (startLocal.offset != 0L) && (state.textEditingValue.text.codeUnitAt(position.offset - 1L) != NEWLINE_CODE_UNIT);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override object? invoke(T intent, BuildContext? context = null)
    {
        global::Doroti.Framework.Services.TextSelection selectionLocal = state._value.selection;
        DartRuntimePrimitives.Assert(() => selectionLocal.isValid);
        bool collapseSelectionLocal = intent.collapseSelection || !state.widget.selectionEnabled;
        if (!selectionLocal.isCollapsed && !ignoreNonCollapsedSelection && collapseSelectionLocal)
        {
            return Actions.invoke(context!, new UpdateSelectionIntent(state._value, TextSelection.CreateCollapsed(offset: intent.forward ? selectionLocal.end : selectionLocal.start), SelectionChangedCause.keyboard));
        }
        global::Doroti.Ui.TextPosition extentLocal = selectionLocal.extent;
        if (intent.continuesAtWrap)
        {
            if (intent.forward && _isAtWordwrapUpstream(extentLocal))
            {
                extentLocal = new global::Doroti.Ui.TextPosition(offset: extentLocal.offset);
            }
            else
            {
                if (!intent.forward && _isAtWordwrapDownstream(extentLocal))
                {
                    extentLocal = new global::Doroti.Ui.TextPosition(offset: extentLocal.offset, affinity: TextAffinity.upstream);
                }
            }
        }
        bool shouldTargetBase = isExpand && (intent.forward ? (selectionLocal.baseOffset > selectionLocal.extentOffset) : (selectionLocal.baseOffset < selectionLocal.extentOffset));
        global::Doroti.Ui.TextPosition newExtent = applyTextBoundary(shouldTargetBase ? selectionLocal.@base : extentLocal, intent.forward, getTextBoundary());
        global::Doroti.Framework.Services.TextSelection newSelection = (collapseSelectionLocal || !isExpand && (newExtent.offset == selectionLocal.baseOffset)) ? TextSelection.CreateFromPosition(newExtent) : (isExpand ? selectionLocal.expandTo(newExtent, extentAtIndex || selectionLocal.isCollapsed) : selectionLocal.extendTo(newExtent));
        bool shouldCollapseToBase = intent.collapseAtReversal && (((selectionLocal.baseOffset - selectionLocal.extentOffset) * (selectionLocal.baseOffset - newSelection.extentOffset)) < 0L);
        var newRange = shouldCollapseToBase ? TextSelection.CreateFromPosition(selectionLocal.@base) : newSelection;
        return Actions.invoke(context!, new UpdateSelectionIntent(state._value, newRange, SelectionChangedCause.keyboard));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isActionEnabled
    {
        get
        {
            if (Foundation.ConstantsLibrary.kIsWeb && state.widget.selectionEnabled && state._value.composing.isValid)
            {
                return false;
            }
            return state._value.selection.isValid;
        }
    }
}

internal class _UpdateTextSelectionVerticallyAction__editable_text<T> : ContextAction<T> where T : DirectionalCaretMovementIntent
{
    public virtual EditableTextState state { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Rendering.VerticalCaretMovementRun? _verticalMovementRun { get; set; } = default;
    internal virtual global::Doroti.Framework.Services.TextSelection? _runSelection { get; set; } = default;

    internal _UpdateTextSelectionVerticallyAction__editable_text(EditableTextState state)
    {
        this.state = state;
    }

    public virtual void stopCurrentVerticalRunIfSelectionChanges()
    {
        global::Doroti.Framework.Services.TextSelection? runSelection = _runSelection;
        if (runSelection is null)
        {
            DartRuntimePrimitives.Assert(() => _verticalMovementRun is null);
            return;
        }
        _runSelection = state._value.selection;
        global::Doroti.Framework.Services.TextSelection currentSelection = state.widget.controller.selection;
        bool continueCurrentRun = currentSelection.isValid && currentSelection.isCollapsed && (currentSelection.baseOffset == runSelection.baseOffset) && (currentSelection.extentOffset == runSelection.extentOffset);
        if (!continueCurrentRun)
        {
            _verticalMovementRun = null;
            _runSelection = null;
        }
    }

    public override object? invoke(T intent, BuildContext? context = null)
    {
        DartRuntimePrimitives.Assert(() => state._value.selection.isValid);
        bool collapseSelectionLocal = intent.collapseSelection || !state.widget.selectionEnabled;
        global::Doroti.Framework.Services.TextEditingValue value = state._textEditingValueforTextLayoutMetrics;
        if (!value.selection.isValid)
        {
            return default!;
        }
        if (_verticalMovementRun?.isValid == false)
        {
            _verticalMovementRun = null;
            _runSelection = null;
        }
        global::Doroti.Framework.Rendering.VerticalCaretMovementRun currentRun = _verticalMovementRun ?? state.renderEditable.startVerticalCaretMovement(state.renderEditable.selection!.extent);
        bool shouldMove = (intent is ExtendSelectionVerticallyToAdjacentPageIntent) ? currentRun.moveByOffset((intent.forward ? 1.0 : -1.0) * state.renderEditable.size.height) : (intent.forward ? currentRun.moveNext() : currentRun.movePrevious());
        global::Doroti.Ui.TextPosition newExtent = shouldMove ? currentRun.current : (intent.forward ? new global::Doroti.Ui.TextPosition(offset: value.text.Length) : new global::Doroti.Ui.TextPosition(offset: 0L));
        global::Doroti.Framework.Services.TextSelection newSelection = collapseSelectionLocal ? TextSelection.CreateFromPosition(newExtent) : value.selection.extendTo(newExtent);
        Actions.invoke(context!, new UpdateSelectionIntent(value, newSelection, SelectionChangedCause.keyboard));
        if (Equals(state._value.selection, newSelection))
        {
            _verticalMovementRun = currentRun;
            _runSelection = newSelection;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isActionEnabled
    {
        get
        {
            if (Foundation.ConstantsLibrary.kIsWeb && state.widget.selectionEnabled && state._value.composing.isValid)
            {
                return false;
            }
            return state._value.selection.isValid;
        }
    }
}

internal class _WebComposingDisablingCallbackAction__editable_text<T> : CallbackAction<T> where T : Intent
{
    public virtual EditableTextState state { get; private set; } = default!;

    internal _WebComposingDisablingCallbackAction__editable_text(EditableTextState state, global::System.Func<T, object?> onInvoke) : base(onInvoke: onInvoke)
    {
        this.state = state;
    }

    public override bool isActionEnabled
    {
        get
        {
            if (Foundation.ConstantsLibrary.kIsWeb && state.widget.selectionEnabled && state._value.composing.isValid)
            {
                return false;
            }
            return base.isActionEnabled;
        }
    }
}

internal class _SelectAllAction__editable_text : ContextAction<SelectAllTextIntent>
{
    public virtual EditableTextState state { get; private set; } = default!;

    internal _SelectAllAction__editable_text(EditableTextState state)
    {
        this.state = state;
    }

    public override object? invoke(SelectAllTextIntent intent, BuildContext? context = null)
    {
        if (!state.widget.selectionEnabled)
        {
            return null;
        }
        return Actions.invoke(context!, new UpdateSelectionIntent(state._value, new global::Doroti.Framework.Services.TextSelection(baseOffset: 0L, extentOffset: state._value.text.Length), intent.cause));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CopySelectionAction__editable_text : ContextAction<CopySelectionTextIntent>
{
    public virtual EditableTextState state { get; private set; } = default!;

    internal _CopySelectionAction__editable_text(EditableTextState state)
    {
        this.state = state;
    }

    public override object? invoke(CopySelectionTextIntent intent, BuildContext? context = null)
    {
        if (!state._value.selection.isValid || state._value.selection.isCollapsed)
        {
            return default!;
        }
        if (!state.widget.selectionEnabled)
        {
            return default!;
        }
        if (intent.collapseSelection)
        {
            state.cutSelection(intent.cause);
        }
        else
        {
            state.copySelection(intent.cause);
        }
        return null;
    }

}

internal class _PasteSelectionAction__editable_text : ContextAction<PasteTextIntent>
{
    public virtual EditableTextState state { get; private set; } = default!;

    internal _PasteSelectionAction__editable_text(EditableTextState state)
    {
        this.state = state;
    }

    public override object? invoke(PasteTextIntent intent, BuildContext? context = null)
    {
        if (!state.widget.selectionEnabled)
        {
            return default!;
        }
        DartRuntimePrimitives.Ignore(state._pasteTextWithReporting(intent.cause));
        return null;
    }

}

internal class _WebClipboardStatusNotifier__editable_text : ClipboardStatusNotifier
{
    internal _WebClipboardStatusNotifier__editable_text() : base(ClipboardStatus.pasteable) { }

    public override Future update()
    {
        return Future.value();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _EditableTextTapOutsideAction__editable_text : ContextAction<EditableTextTapOutsideIntent>
{
    internal _EditableTextTapOutsideAction__editable_text()
    {
    }

    public override object? invoke(EditableTextTapOutsideIntent intent, BuildContext? context = null)
    {
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.android:
            case TargetPlatform.iOS:
            case TargetPlatform.fuchsia:
                {
                    switch (intent.pointerDownEvent.kind)
                    {
                        case PointerDeviceKind.touch:
                            {
                                if (Foundation.ConstantsLibrary.kIsWeb)
                                {
                                    intent.focusNode.unfocus();
                                }
                                break;
                            }
                        case PointerDeviceKind.mouse:
                        case PointerDeviceKind.stylus:
                        case PointerDeviceKind.invertedStylus:
                        case PointerDeviceKind.unknown:
                            {
                                intent.focusNode.unfocus();
                                break;
                            }
                        case PointerDeviceKind.trackpad:
                            {
                                throw new NotImplementedException("Unexpected pointer down event for trackpad");
                            }
                    }
                    break;
                }
            case TargetPlatform.linux:
            case TargetPlatform.macOS:
            case TargetPlatform.windows:
                {
                    intent.focusNode.unfocus();
                    break;
                }
        }
        return null;
    }

}

internal class _EditableTextTapUpOutsideAction__editable_text : ContextAction<EditableTextTapUpOutsideIntent>
{
    internal _EditableTextTapUpOutsideAction__editable_text()
    {
    }

    public override object? invoke(EditableTextTapUpOutsideIntent intent, BuildContext? context = null)
    {
        return null;
    }

}

internal class _OverridingTextStyleTextSpanUtils__editable_text
{
    public static global::Doroti.Framework.Painting.TextSpan applyTextSpacingOverrides(double? lineHeightScaleFactor = null, double? letterSpacing = null, double? wordSpacing = null, global::Doroti.Framework.Painting.TextSpan textSpan = default!)
    {
        if ((lineHeightScaleFactor is null) && (letterSpacing is null) && (wordSpacing is null))
        {
            return textSpan;
        }
        return _applyTextStyleOverrides(new global::Doroti.Framework.Painting.TextStyle(height: lineHeightScaleFactor, letterSpacing: letterSpacing, wordSpacing: wordSpacing), textSpan);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Framework.Painting.TextSpan _applyTextStyleOverrides(global::Doroti.Framework.Painting.TextStyle overrideTextStyle, global::Doroti.Framework.Painting.TextSpan textSpan)
    {
        return new global::Doroti.Framework.Painting.TextSpan(text: textSpan.text, children: textSpan.children?.map<global::Doroti.Framework.Painting.InlineSpan, global::Doroti.Framework.Painting.InlineSpan>((child) =>
        {
            if ((child is global::Doroti.Framework.Painting.TextSpan) && Equals(DartRuntimePrimitives.RuntimeType((global::Doroti.Framework.Painting.TextSpan)child), typeof(global::Doroti.Framework.Painting.TextSpan)))
            {
                return _applyTextStyleOverrides(overrideTextStyle, (global::Doroti.Framework.Painting.TextSpan)child);
            }
            return child;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }).ToList(), style: textSpan.style?.merge(overrideTextStyle) ?? overrideTextStyle, recognizer: textSpan.recognizer, mouseCursor: textSpan.mouseCursor, onEnter: textSpan.onEnter, onExit: textSpan.onExit, semanticsLabel: textSpan.semanticsLabel, semanticsIdentifier: textSpan.semanticsIdentifier, locale: textSpan.locale, spellOut: textSpan.spellOut);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
