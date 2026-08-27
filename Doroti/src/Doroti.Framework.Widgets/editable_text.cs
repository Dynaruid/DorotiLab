// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/editable_text.dart
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

public delegate void SelectionChangedCallback(global::Doroti.Framework.Services.TextSelection selection, global::Doroti.Framework.Services.SelectionChangedCause? cause);

public delegate void AppPrivateCommandCallback(string action, DartMap<string, object> data);

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
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderCompositionCallback__editable_text((global::System.Action<global::Doroti.Framework.Rendering.Layer>)this.compositeCallback, this.enabled));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderCompositionCallback__editable_text)(object)renderObject;
        base.updateRenderObject(context, __renderObject);
        DartRuntimePrimitives.Assert(() => (object.Equals((global::System.Action<global::Doroti.Framework.Rendering.Layer>)((_RenderCompositionCallback__editable_text)__renderObject).compositeCallback, (global::System.Action<global::Doroti.Framework.Rendering.Layer>)this.compositeCallback)));
        __renderObject.enabled = this.enabled;
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
        get => this._enabled;
        set
        {
            var newValue = value;
            _enabled = newValue;
            if (!newValue)
            {
                this._cancelCallback?.Invoke();
                _cancelCallback = null;
            }
            else
            {
                if ((this._cancelCallback is null))
                {
                    markNeedsPaint();
                }
            }
        }
    }
    public virtual void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        if (this.enabled)
        {
            _cancelCallback ??= context.addCompositionCallback((global::System.Action<global::Doroti.Framework.Rendering.Layer>)this.compositeCallback);
        }
        base.paint(context, offset);
    }

}

public class TextEditingController : global::Doroti.Framework.Foundation.ValueNotifier<global::Doroti.Framework.Services.TextEditingValue>
{
    public TextEditingController(string? text = null) : base(((text is null) ? global::Doroti.Framework.Services.TextEditingValue.empty : new global::Doroti.Framework.Services.TextEditingValue(text: text)))
    {
    }

    public static TextEditingController CreateFromValue(global::Doroti.Framework.Services.TextEditingValue? value)
    {
        var __instance = new TextEditingController(default!);
        return __instance;
    }

    public virtual string text
    {
        get => ((global::Doroti.Framework.Services.TextEditingValue)(object)this.value).text;
        set
        {
            var newText = value;
            this.value = this.value.copyWith(text: newText, selection: global::Doroti.Framework.Services.TextSelection.CreateCollapsed(offset: -1L), composing: TextRange.empty);
        }
    }
    public override global::Doroti.Framework.Services.TextEditingValue value
    {
        get => base.value;
        set
        {
            var newValue = value;
            DartRuntimePrimitives.Assert(() => (!((global::Doroti.Framework.Services.TextEditingValue)newValue).composing.isValid || ((global::Doroti.Framework.Services.TextEditingValue)newValue).isComposingRangeValid), () => (object?)$"New TextEditingValue {newValue} has an invalid non-empty composing range " + $"{(((global::Doroti.Framework.Services.TextEditingValue)newValue).composing)}. It is recommended to use a valid composing range, " + "even for readonly text fields.");
            base.value = newValue;
        }
    }
    public virtual global::Doroti.Framework.Painting.TextSpan buildTextSpan(BuildContext context, global::Doroti.Framework.Painting.TextStyle? style = null, bool withComposing = default!)
    {
        DartRuntimePrimitives.Assert(() => ((!((global::Doroti.Framework.Services.TextEditingValue)(object)this.value).composing.isValid || !withComposing) || ((global::Doroti.Framework.Services.TextEditingValue)(object)this.value).isComposingRangeValid));
        bool composingRegionOutOfRange = (!((global::Doroti.Framework.Services.TextEditingValue)(object)this.value).isComposingRangeValid || !withComposing);
        if (composingRegionOutOfRange)
        {
            return new global::Doroti.Framework.Painting.TextSpan(style: style, text: this.text);
        }
        global::Doroti.Framework.Painting.TextStyle composingStyle = (style?.merge(new global::Doroti.Framework.Painting.TextStyle(decoration: TextDecoration.underline)) ?? new global::Doroti.Framework.Painting.TextStyle(decoration: TextDecoration.underline));
        return new global::Doroti.Framework.Painting.TextSpan(style: style, children: new List<global::Doroti.Framework.Painting.TextSpan> { new global::Doroti.Framework.Painting.TextSpan(text: ((global::Doroti.Framework.Services.TextEditingValue)(object)this.value).composing.textBefore(((global::Doroti.Framework.Services.TextEditingValue)(object)this.value).text)), new global::Doroti.Framework.Painting.TextSpan(style: composingStyle, text: ((global::Doroti.Framework.Services.TextEditingValue)(object)this.value).composing.textInside(((global::Doroti.Framework.Services.TextEditingValue)(object)this.value).text)), new global::Doroti.Framework.Painting.TextSpan(text: ((global::Doroti.Framework.Services.TextEditingValue)(object)this.value).composing.textAfter(((global::Doroti.Framework.Services.TextEditingValue)(object)this.value).text)) }.Cast<global::Doroti.Framework.Painting.InlineSpan>().ToList());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Services.TextSelection selection
    {
        get => ((global::Doroti.Framework.Services.TextEditingValue)(object)this.value).selection;
        set
        {
            var newSelection = value;
            if (((this.text.Length < newSelection.end) || (this.text.Length < newSelection.start)))
            {
                throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create($"invalid text selection: {newSelection}"));
            }
            global::Doroti.Ui.TextRange newComposing = ((global::Doroti.Ui.TextRange)(object?)(_isSelectionWithinComposingRange(newSelection) ? ((global::Doroti.Framework.Services.TextEditingValue)(object)this.value).composing : TextRange.empty));
            this.value = this.value.copyWith(selection: newSelection, composing: newComposing);
        }
    }
    public virtual void clear()
    {
        this.value = new global::Doroti.Framework.Services.TextEditingValue(selection: global::Doroti.Framework.Services.TextSelection.CreateCollapsed(offset: 0L));
    }

    public virtual void clearComposing()
    {
        this.value = this.value.copyWith(composing: TextRange.empty);
    }

    internal virtual bool _isSelectionWithinComposingRange(global::Doroti.Framework.Services.TextSelection selection)
    {
        return ((selection.start >= ((global::Doroti.Framework.Services.TextEditingValue)(object)this.value).composing.start) && (selection.end <= ((global::Doroti.Framework.Services.TextEditingValue)(object)this.value).composing.end));
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
        System.Diagnostics.Debug.Assert(System.Linq.Enumerable.Any(__allowedMimeTypes));
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
        System.Diagnostics.Debug.Assert(System.Linq.Enumerable.Any(_keyFrames));
        System.Diagnostics.Debug.Assert((_keyFrames.Last().time <= maxDuration));
        System.Diagnostics.Debug.Assert(((global::System.Func<bool>)(() =>
        {
            for (var i = 0L; (i < (checked((long)(_keyFrames.Count)) - 1L)); i += 1L)
            {
                if ((_keyFrames[(int)(i)].time > _keyFrames[(int)((i + 1L))].time))
                {
                    return false;
                }
            }
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))());
    }

    public override double dx(double time) => 0;
    public override bool isDone(double time) => DartRuntimePrimitives.ConvertValue<bool>((time >= this.maxDuration));
    public override double x(double time)
    {
        long length = checked((long)(this._keyFrames.Count));
        long searchIndex = default!;
        long endIndex = default!;
        if ((this._keyFrames[(int)(this._lastKeyFrameIndex)].time > time))
        {
            searchIndex = 0L;
            endIndex = this._lastKeyFrameIndex;
        }
        else
        {
            searchIndex = this._lastKeyFrameIndex;
            endIndex = length;
        }
        while ((searchIndex < (endIndex - 1L)))
        {
            DartRuntimePrimitives.Assert(() => (this._keyFrames[(int)(searchIndex)].time <= time));
            _KeyFrame__editable_text next = this._keyFrames[(int)((searchIndex + 1L))];
            if ((time < ((_KeyFrame__editable_text)next).time))
            {
                break;
            }
            searchIndex += 1L;
        }
        _lastKeyFrameIndex = searchIndex;
        return this._keyFrames[(int)(this._lastKeyFrameIndex)].value;
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
    public virtual global::System.Action<string, DartMap<string, object>>? onAppPrivateCommand { get; private set; }
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

    public EditableText(global::Doroti.Framework.Foundation.Key? key = null, TextEditingController controller = default!, FocusNode focusNode = default!, bool readOnly = false, string obscuringCharacter = "•", bool obscureText = false, bool? autocorrect = null, global::Doroti.Framework.Services.SmartDashesType? smartDashesType = null, global::Doroti.Framework.Services.SmartQuotesType? smartQuotesType = null, bool enableSuggestions = true, global::Doroti.Framework.Painting.TextStyle style = default!, global::Doroti.Framework.Painting.StrutStyle? strutStyle = null, Color cursorColor = default!, Color backgroundCursorColor = default!, TextAlign textAlign = TextAlign.start, TextDirection? textDirection = null, Locale? locale = null, double? textScaleFactor = null, global::Doroti.Framework.Painting.TextScaler? textScaler = null, long? maxLines = 1, long? minLines = null, bool expands = false, bool forceLine = true, TextHeightBehavior? textHeightBehavior = null, global::Doroti.Framework.Painting.TextWidthBasis textWidthBasis = global::Doroti.Framework.Painting.TextWidthBasis.parent, bool autofocus = false, bool? showCursor = null, bool showSelectionHandles = false, Color? selectionColor = null, TextSelectionControls? selectionControls = null, global::Doroti.Framework.Services.TextInputType? keyboardType = null, global::Doroti.Framework.Services.TextInputAction? textInputAction = null, global::Doroti.Framework.Services.TextCapitalization textCapitalization = global::Doroti.Framework.Services.TextCapitalization.none, global::System.Action<string>? onChanged = null, global::System.Action? onEditingComplete = null, global::System.Action<string>? onSubmitted = null, global::System.Action<string, DartMap<string, object>>? onAppPrivateCommand = null, global::System.Action<global::Doroti.Framework.Services.TextSelection, global::Doroti.Framework.Services.SelectionChangedCause?>? onSelectionChanged = null, global::System.Action? onSelectionHandleTapped = null, object groupId = default!, global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>? onTapOutside = null, global::System.Action<global::Doroti.Framework.Gestures.PointerUpEvent>? onTapUpOutside = null, List<global::Doroti.Framework.Services.TextInputFormatter>? inputFormatters = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, bool rendererIgnoresPointer = false, double cursorWidth = 2.0, double? cursorHeight = null, Radius? cursorRadius = null, bool cursorOpacityAnimates = false, Offset? cursorOffset = null, bool paintCursorAboveText = false, BoxHeightStyle? selectionHeightStyle = null, BoxWidthStyle? selectionWidthStyle = null, global::Doroti.Framework.Painting.EdgeInsets scrollPadding = default!, Brightness keyboardAppearance = Brightness.light, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.start, bool? enableInteractiveSelection = null, bool? selectAllOnFocus = null, ScrollController? scrollController = null, ScrollPhysics? scrollPhysics = null, Color? autocorrectionTextRectColor = null, ToolbarOptions? toolbarOptions = null, IEnumerable<string>? autofillHints = default!, global::Doroti.Framework.Services.AutofillClient? autofillClient = null, Clip clipBehavior = Clip.hardEdge, string? restorationId = null, ScrollBehavior? scrollBehavior = null, bool scribbleEnabled = true, bool? stylusHandwritingEnabled = null, bool enableIMEPersonalizedLearning = true, ContentInsertionConfiguration? contentInsertionConfiguration = null, global::System.Func<BuildContext, EditableTextState, Widget>? contextMenuBuilder = null, SpellCheckConfiguration? spellCheckConfiguration = null, TextMagnifierConfiguration magnifierConfiguration = default!, UndoHistoryController? undoController = null, List<Locale>? hintLocales = null, bool? enableInlinePrediction = null) : base(key: key)
    {
        object __groupId = groupId ?? typeof(EditableText);
        global::Doroti.Framework.Painting.EdgeInsets __scrollPadding = scrollPadding ?? global::Doroti.Framework.Painting.EdgeInsets.CreateAll(20.0);
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
        this.autocorrect = ((autocorrect ?? (bool)EditableText._inferAutocorrect(autofillHints: autofillHints.Cast<string>())));
        this.smartDashesType = (smartDashesType ?? ((obscureText ? global::Doroti.Framework.Services.SmartDashesType.disabled : global::Doroti.Framework.Services.SmartDashesType.enabled)));
        this.smartQuotesType = (smartQuotesType ?? ((obscureText ? global::Doroti.Framework.Services.SmartQuotesType.disabled : global::Doroti.Framework.Services.SmartQuotesType.enabled)));
        this.enableInteractiveSelection = (enableInteractiveSelection ?? ((!readOnly || !obscureText)));
        this.selectAllOnFocus = ((selectAllOnFocus ?? (bool)_defaultSelectAllOnFocus));
        this.toolbarOptions = (((selectionControls is TextSelectionHandleControls) && (toolbarOptions is null)) ? ToolbarOptions.empty : (toolbarOptions ?? ((obscureText ? ((readOnly ? ToolbarOptions.empty : new ToolbarOptions(selectAll: true, paste: true))) : ((readOnly ? new ToolbarOptions(selectAll: true, copy: true) : new ToolbarOptions(copy: true, cut: true, selectAll: true, paste: true)))))));
        this._strutStyle = strutStyle;
        this.keyboardType = ((keyboardType ?? (global::Doroti.Framework.Services.TextInputType)EditableText._inferKeyboardType(autofillHints: autofillHints.Cast<string>(), maxLines: maxLines)));
        this.inputFormatters = ((maxLines == 1L) ? new List<global::Doroti.Framework.Services.TextInputFormatter> { global::Doroti.Framework.Services.FilteringTextInputFormatter.singleLineFormatter } : inputFormatters);
        this.showCursor = (showCursor ?? !readOnly);
        this.selectionHeightStyle = ((selectionHeightStyle ?? (BoxHeightStyle)defaultSelectionHeightStyle));
        this.selectionWidthStyle = ((selectionWidthStyle ?? (BoxWidthStyle)defaultSelectionWidthStyle));
        System.Diagnostics.Debug.Assert((obscuringCharacter.Length == 1L));
        System.Diagnostics.Debug.Assert(((minLines is null) || (DartRuntimePrimitives.RequireValue(minLines) > 0L)));
        System.Diagnostics.Debug.Assert(((((maxLines is null)) || ((minLines is null))) || ((maxLines >= DartRuntimePrimitives.RequireValue(minLines)))));
        System.Diagnostics.Debug.Assert((!expands || (((maxLines is null) && (minLines is null)))));
        System.Diagnostics.Debug.Assert((!obscureText || (maxLines == 1L)));
        System.Diagnostics.Debug.Assert((((spellCheckConfiguration is null) || (object.Equals(spellCheckConfiguration, SpellCheckConfiguration.CreateDisabled()))) || (((SpellCheckConfiguration)spellCheckConfiguration).misspelledTextStyle is not null)));
    }

    public virtual global::Doroti.Framework.Painting.StrutStyle strutStyle
    {
        get
        {
            if ((this._strutStyle is null))
            {
                return global::Doroti.Framework.Painting.StrutStyle.CreateFromTextStyle(this.style, forceStrutHeight: true);
            }
            return ((global::Doroti.Framework.Painting.StrutStyle)(object?)this._strutStyle.inheritFromTextStyle(this.style));
            return default!;
        }
    }
    public virtual bool selectionEnabled => this.enableInteractiveSelection;
    public static global::Doroti.Ui.BoxHeightStyle defaultSelectionHeightStyle
    {
        get
        {
            if (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb)
            {
                return BoxHeightStyle.max;
            }
            return BoxHeightStyle.includeLineSpacingMiddle;
            return default!;
        }
    }
    public static global::Doroti.Ui.BoxWidthStyle defaultSelectionWidthStyle
    {
        get
        {
            if (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb)
            {
                if (((object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.iOS)) || WebBrowserDetectionIo.isSafari))
                {
                    return BoxWidthStyle.max;
                }
                return BoxWidthStyle.tight;
            }
            return BoxWidthStyle.max;
            return default!;
        }
    }
    internal virtual bool _userSelectionEnabled => DartRuntimePrimitives.ConvertValue<bool>((this.enableInteractiveSelection && ((!this.readOnly || !this.obscureText))));
    internal static bool _defaultSelectAllOnFocus
    {
        get
        {
            if (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb)
            {
                return true;
            }
            return (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform switch { global::Doroti.Framework.Foundation.TargetPlatform.android => false, global::Doroti.Framework.Foundation.TargetPlatform.iOS => false, global::Doroti.Framework.Foundation.TargetPlatform.fuchsia => false, global::Doroti.Framework.Foundation.TargetPlatform.linux => true, global::Doroti.Framework.Foundation.TargetPlatform.macOS => true, global::Doroti.Framework.Foundation.TargetPlatform.windows => true, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            return default!;
        }
    }
    public static List<ContextMenuButtonItem> getEditableButtonItems(ClipboardStatus? clipboardStatus, global::System.Action? onCopy, global::System.Action? onCut, global::System.Action? onPaste, global::System.Action? onSelectAll, global::System.Action? onLookUp, global::System.Action? onSearchWeb, global::System.Action? onShare, global::System.Action? onLiveTextInput)
    {
        var resultButtonItem = new List<ContextMenuButtonItem>();
        if (((onPaste is null) || (!object.Equals(clipboardStatus, ClipboardStatus.unknown))))
        {
            var showShareBeforeSelectAll = (object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.android));
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
        if ((onLiveTextInput is not null))
        {
            resultButtonItem.Add(new ContextMenuButtonItem(onPressed: () => onLiveTextInput(), type: ContextMenuButtonType.liveTextInput));
        }
        return resultButtonItem;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static bool _inferAutocorrect(IEnumerable<string>? autofillHints)
    {
        if ((((autofillHints is null) || !System.Linq.Enumerable.Any(autofillHints)) || global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb))
        {
            return true;
        }
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                {
                    bool passwordRelatedHint = autofillHints.any(((hint) => (((hint == global::Doroti.Framework.Services.AutofillHints.username) || (hint == global::Doroti.Framework.Services.AutofillHints.password)) || (hint == global::Doroti.Framework.Services.AutofillHints.newPassword))));
                    if (passwordRelatedHint)
                    {
                        return false;
                    }
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    break;
                }
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Framework.Services.TextInputType _inferKeyboardType(IEnumerable<string>? autofillHints, long? maxLines)
    {
        if (((autofillHints is null) || !System.Linq.Enumerable.Any(autofillHints)))
        {
            return ((maxLines == 1L) ? global::Doroti.Framework.Services.TextInputType.text : global::Doroti.Framework.Services.TextInputType.multiline);
        }
        string effectiveHint = autofillHints.First();
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb)
        {
            switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
            {
                case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                    {
                        var iOSKeyboardType = new DartMap<string, global::Doroti.Framework.Services.TextInputType> { [global::Doroti.Framework.Services.AutofillHints.addressCity] = global::Doroti.Framework.Services.TextInputType.name, [global::Doroti.Framework.Services.AutofillHints.addressCityAndState] = global::Doroti.Framework.Services.TextInputType.name, [global::Doroti.Framework.Services.AutofillHints.addressState] = global::Doroti.Framework.Services.TextInputType.name, [global::Doroti.Framework.Services.AutofillHints.countryName] = global::Doroti.Framework.Services.TextInputType.name, [global::Doroti.Framework.Services.AutofillHints.creditCardNumber] = global::Doroti.Framework.Services.TextInputType.number, [global::Doroti.Framework.Services.AutofillHints.email] = global::Doroti.Framework.Services.TextInputType.emailAddress, [global::Doroti.Framework.Services.AutofillHints.emailOTPCode] = global::Doroti.Framework.Services.TextInputType.text, [global::Doroti.Framework.Services.AutofillHints.familyName] = global::Doroti.Framework.Services.TextInputType.name, [global::Doroti.Framework.Services.AutofillHints.fullStreetAddress] = global::Doroti.Framework.Services.TextInputType.name, [global::Doroti.Framework.Services.AutofillHints.givenName] = global::Doroti.Framework.Services.TextInputType.name, [global::Doroti.Framework.Services.AutofillHints.jobTitle] = global::Doroti.Framework.Services.TextInputType.name, [global::Doroti.Framework.Services.AutofillHints.location] = global::Doroti.Framework.Services.TextInputType.name, [global::Doroti.Framework.Services.AutofillHints.middleName] = global::Doroti.Framework.Services.TextInputType.name, [global::Doroti.Framework.Services.AutofillHints.name] = global::Doroti.Framework.Services.TextInputType.name, [global::Doroti.Framework.Services.AutofillHints.namePrefix] = global::Doroti.Framework.Services.TextInputType.name, [global::Doroti.Framework.Services.AutofillHints.nameSuffix] = global::Doroti.Framework.Services.TextInputType.name, [global::Doroti.Framework.Services.AutofillHints.newPassword] = global::Doroti.Framework.Services.TextInputType.text, [global::Doroti.Framework.Services.AutofillHints.newUsername] = global::Doroti.Framework.Services.TextInputType.text, [global::Doroti.Framework.Services.AutofillHints.nickname] = global::Doroti.Framework.Services.TextInputType.name, [global::Doroti.Framework.Services.AutofillHints.oneTimeCode] = global::Doroti.Framework.Services.TextInputType.number, [global::Doroti.Framework.Services.AutofillHints.organizationName] = global::Doroti.Framework.Services.TextInputType.text, [global::Doroti.Framework.Services.AutofillHints.password] = global::Doroti.Framework.Services.TextInputType.text, [global::Doroti.Framework.Services.AutofillHints.postalCode] = global::Doroti.Framework.Services.TextInputType.name, [global::Doroti.Framework.Services.AutofillHints.streetAddressLine1] = global::Doroti.Framework.Services.TextInputType.name, [global::Doroti.Framework.Services.AutofillHints.streetAddressLine2] = global::Doroti.Framework.Services.TextInputType.name, [global::Doroti.Framework.Services.AutofillHints.sublocality] = global::Doroti.Framework.Services.TextInputType.name, [global::Doroti.Framework.Services.AutofillHints.telephoneNumber] = global::Doroti.Framework.Services.TextInputType.name, [global::Doroti.Framework.Services.AutofillHints.url] = global::Doroti.Framework.Services.TextInputType.url, [global::Doroti.Framework.Services.AutofillHints.username] = global::Doroti.Framework.Services.TextInputType.text };
                        global::Doroti.Framework.Services.TextInputType? keyboardType = iOSKeyboardType.GetValueOrDefault(effectiveHint);
                        if ((keyboardType is not null))
                        {
                            return keyboardType;
                        }
                        break;
                    }
                case global::Doroti.Framework.Foundation.TargetPlatform.android:
                case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                    {
                        break;
                    }
            }
        }
        if ((maxLines != 1L))
        {
            return global::Doroti.Framework.Services.TextInputType.multiline;
        }
        var inferKeyboardType = new DartMap<string, global::Doroti.Framework.Services.TextInputType> { [global::Doroti.Framework.Services.AutofillHints.addressCity] = global::Doroti.Framework.Services.TextInputType.streetAddress, [global::Doroti.Framework.Services.AutofillHints.addressCityAndState] = global::Doroti.Framework.Services.TextInputType.streetAddress, [global::Doroti.Framework.Services.AutofillHints.addressState] = global::Doroti.Framework.Services.TextInputType.streetAddress, [global::Doroti.Framework.Services.AutofillHints.birthday] = global::Doroti.Framework.Services.TextInputType.datetime, [global::Doroti.Framework.Services.AutofillHints.birthdayDay] = global::Doroti.Framework.Services.TextInputType.datetime, [global::Doroti.Framework.Services.AutofillHints.birthdayMonth] = global::Doroti.Framework.Services.TextInputType.datetime, [global::Doroti.Framework.Services.AutofillHints.birthdayYear] = global::Doroti.Framework.Services.TextInputType.datetime, [global::Doroti.Framework.Services.AutofillHints.countryCode] = global::Doroti.Framework.Services.TextInputType.number, [global::Doroti.Framework.Services.AutofillHints.countryName] = global::Doroti.Framework.Services.TextInputType.text, [global::Doroti.Framework.Services.AutofillHints.creditCardExpirationDate] = global::Doroti.Framework.Services.TextInputType.datetime, [global::Doroti.Framework.Services.AutofillHints.creditCardExpirationDay] = global::Doroti.Framework.Services.TextInputType.datetime, [global::Doroti.Framework.Services.AutofillHints.creditCardExpirationMonth] = global::Doroti.Framework.Services.TextInputType.datetime, [global::Doroti.Framework.Services.AutofillHints.creditCardExpirationYear] = global::Doroti.Framework.Services.TextInputType.datetime, [global::Doroti.Framework.Services.AutofillHints.creditCardFamilyName] = global::Doroti.Framework.Services.TextInputType.name, [global::Doroti.Framework.Services.AutofillHints.creditCardGivenName] = global::Doroti.Framework.Services.TextInputType.name, [global::Doroti.Framework.Services.AutofillHints.creditCardMiddleName] = global::Doroti.Framework.Services.TextInputType.name, [global::Doroti.Framework.Services.AutofillHints.creditCardName] = global::Doroti.Framework.Services.TextInputType.name, [global::Doroti.Framework.Services.AutofillHints.creditCardNumber] = global::Doroti.Framework.Services.TextInputType.number, [global::Doroti.Framework.Services.AutofillHints.creditCardSecurityCode] = global::Doroti.Framework.Services.TextInputType.number, [global::Doroti.Framework.Services.AutofillHints.creditCardType] = global::Doroti.Framework.Services.TextInputType.text, [global::Doroti.Framework.Services.AutofillHints.email] = global::Doroti.Framework.Services.TextInputType.emailAddress, [global::Doroti.Framework.Services.AutofillHints.emailOTPCode] = global::Doroti.Framework.Services.TextInputType.text, [global::Doroti.Framework.Services.AutofillHints.familyName] = global::Doroti.Framework.Services.TextInputType.name, [global::Doroti.Framework.Services.AutofillHints.fullStreetAddress] = global::Doroti.Framework.Services.TextInputType.streetAddress, [global::Doroti.Framework.Services.AutofillHints.gender] = global::Doroti.Framework.Services.TextInputType.text, [global::Doroti.Framework.Services.AutofillHints.givenName] = global::Doroti.Framework.Services.TextInputType.name, [global::Doroti.Framework.Services.AutofillHints.impp] = global::Doroti.Framework.Services.TextInputType.url, [global::Doroti.Framework.Services.AutofillHints.jobTitle] = global::Doroti.Framework.Services.TextInputType.text, [global::Doroti.Framework.Services.AutofillHints.language] = global::Doroti.Framework.Services.TextInputType.text, [global::Doroti.Framework.Services.AutofillHints.location] = global::Doroti.Framework.Services.TextInputType.streetAddress, [global::Doroti.Framework.Services.AutofillHints.middleInitial] = global::Doroti.Framework.Services.TextInputType.name, [global::Doroti.Framework.Services.AutofillHints.middleName] = global::Doroti.Framework.Services.TextInputType.name, [global::Doroti.Framework.Services.AutofillHints.name] = global::Doroti.Framework.Services.TextInputType.name, [global::Doroti.Framework.Services.AutofillHints.namePrefix] = global::Doroti.Framework.Services.TextInputType.name, [global::Doroti.Framework.Services.AutofillHints.nameSuffix] = global::Doroti.Framework.Services.TextInputType.name, [global::Doroti.Framework.Services.AutofillHints.newPassword] = global::Doroti.Framework.Services.TextInputType.text, [global::Doroti.Framework.Services.AutofillHints.newUsername] = global::Doroti.Framework.Services.TextInputType.text, [global::Doroti.Framework.Services.AutofillHints.nickname] = global::Doroti.Framework.Services.TextInputType.text, [global::Doroti.Framework.Services.AutofillHints.oneTimeCode] = global::Doroti.Framework.Services.TextInputType.text, [global::Doroti.Framework.Services.AutofillHints.organizationName] = global::Doroti.Framework.Services.TextInputType.text, [global::Doroti.Framework.Services.AutofillHints.password] = global::Doroti.Framework.Services.TextInputType.text, [global::Doroti.Framework.Services.AutofillHints.photo] = global::Doroti.Framework.Services.TextInputType.text, [global::Doroti.Framework.Services.AutofillHints.postalAddress] = global::Doroti.Framework.Services.TextInputType.streetAddress, [global::Doroti.Framework.Services.AutofillHints.postalAddressExtended] = global::Doroti.Framework.Services.TextInputType.streetAddress, [global::Doroti.Framework.Services.AutofillHints.postalAddressExtendedPostalCode] = global::Doroti.Framework.Services.TextInputType.number, [global::Doroti.Framework.Services.AutofillHints.postalCode] = global::Doroti.Framework.Services.TextInputType.number, [global::Doroti.Framework.Services.AutofillHints.streetAddressLevel1] = global::Doroti.Framework.Services.TextInputType.streetAddress, [global::Doroti.Framework.Services.AutofillHints.streetAddressLevel2] = global::Doroti.Framework.Services.TextInputType.streetAddress, [global::Doroti.Framework.Services.AutofillHints.streetAddressLevel3] = global::Doroti.Framework.Services.TextInputType.streetAddress, [global::Doroti.Framework.Services.AutofillHints.streetAddressLevel4] = global::Doroti.Framework.Services.TextInputType.streetAddress, [global::Doroti.Framework.Services.AutofillHints.streetAddressLine1] = global::Doroti.Framework.Services.TextInputType.streetAddress, [global::Doroti.Framework.Services.AutofillHints.streetAddressLine2] = global::Doroti.Framework.Services.TextInputType.streetAddress, [global::Doroti.Framework.Services.AutofillHints.streetAddressLine3] = global::Doroti.Framework.Services.TextInputType.streetAddress, [global::Doroti.Framework.Services.AutofillHints.sublocality] = global::Doroti.Framework.Services.TextInputType.streetAddress, [global::Doroti.Framework.Services.AutofillHints.telephoneNumber] = global::Doroti.Framework.Services.TextInputType.phone, [global::Doroti.Framework.Services.AutofillHints.telephoneNumberAreaCode] = global::Doroti.Framework.Services.TextInputType.phone, [global::Doroti.Framework.Services.AutofillHints.telephoneNumberCountryCode] = global::Doroti.Framework.Services.TextInputType.phone, [global::Doroti.Framework.Services.AutofillHints.telephoneNumberDevice] = global::Doroti.Framework.Services.TextInputType.phone, [global::Doroti.Framework.Services.AutofillHints.telephoneNumberExtension] = global::Doroti.Framework.Services.TextInputType.phone, [global::Doroti.Framework.Services.AutofillHints.telephoneNumberLocal] = global::Doroti.Framework.Services.TextInputType.phone, [global::Doroti.Framework.Services.AutofillHints.telephoneNumberLocalPrefix] = global::Doroti.Framework.Services.TextInputType.phone, [global::Doroti.Framework.Services.AutofillHints.telephoneNumberLocalSuffix] = global::Doroti.Framework.Services.TextInputType.phone, [global::Doroti.Framework.Services.AutofillHints.telephoneNumberNational] = global::Doroti.Framework.Services.TextInputType.phone, [global::Doroti.Framework.Services.AutofillHints.transactionAmount] = global::Doroti.Framework.Services.TextInputType.CreateNumberWithOptions(@decimal: true), [global::Doroti.Framework.Services.AutofillHints.transactionCurrency] = global::Doroti.Framework.Services.TextInputType.text, [global::Doroti.Framework.Services.AutofillHints.url] = global::Doroti.Framework.Services.TextInputType.url, [global::Doroti.Framework.Services.AutofillHints.username] = global::Doroti.Framework.Services.TextInputType.text };
        return (inferKeyboardType.GetValueOrDefault(effectiveHint) ?? global::Doroti.Framework.Services.TextInputType.text);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new EditableTextState());
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<TextEditingController>("controller", this.controller));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<FocusNode>("focusNode", this.focusNode));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("obscureText", this.obscureText, defaultValue: false));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("readOnly", this.readOnly, defaultValue: false));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("autocorrect", this.autocorrect, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Services.SmartDashesType>("smartDashesType", this.smartDashesType, defaultValue: (this.obscureText ? global::Doroti.Framework.Services.SmartDashesType.disabled : global::Doroti.Framework.Services.SmartDashesType.enabled)));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Services.SmartQuotesType>("smartQuotesType", this.smartQuotesType, defaultValue: (this.obscureText ? global::Doroti.Framework.Services.SmartQuotesType.disabled : global::Doroti.Framework.Services.SmartQuotesType.enabled)));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("enableSuggestions", this.enableSuggestions, defaultValue: true));
        this.style.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextAlign>("textAlign", this.textAlign, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", this.textDirection, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Locale>("locale", this.locale, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextScaler>("textScaler", this.textScaler, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("maxLines", this.maxLines, defaultValue: 1L));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("minLines", this.minLines, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("expands", this.expands, defaultValue: false));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("autofocus", this.autofocus, defaultValue: false));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Services.TextInputType>("keyboardType", this.keyboardType, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ScrollController>("scrollController", this.scrollController, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ScrollPhysics>("scrollPhysics", this.scrollPhysics, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<IEnumerable<string>>("autofillHints", this.autofillHints, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.TextHeightBehavior>("textHeightBehavior", this.textHeightBehavior, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("scribbleEnabled", this.scribbleEnabled, defaultValue: true));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("stylusHandwritingEnabled", DartRuntimePrimitives.RequireValue(this.stylusHandwritingEnabled), defaultValue: defaultStylusHandwritingEnabled));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("enableIMEPersonalizedLearning", this.enableIMEPersonalizedLearning, defaultValue: true));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool?>("enableInlinePrediction", this.enableInlinePrediction, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("enableInteractiveSelection", this.enableInteractiveSelection, defaultValue: true));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<UndoHistoryController>("undoController", this.undoController, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<SpellCheckConfiguration>("spellCheckConfiguration", this.spellCheckConfiguration, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<List<string>>("contentCommitMimeTypes", (this.contentInsertionConfiguration?.allowedMimeTypes ?? new List<string>()), defaultValue: ((this.contentInsertionConfiguration is null) ? new List<string>() : Editable_textLibrary.kDefaultContentInsertionMimeTypes)));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<List<global::Doroti.Ui.Locale>?>("hintLocales", this.hintLocales, defaultValue: null));
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
                __late__iosBlinkCursorSimulation = ((global::Doroti.Framework.Physics.Simulation)(object?)_DiscreteKeyFrameSimulation__editable_text.CreateIOSBlinkingCaret());
                __late__iosBlinkCursorSimulation_initialized = true;
            }
            return __late__iosBlinkCursorSimulation;
        }
    }
    internal virtual global::Doroti.Framework.Foundation.ValueNotifier<bool> _cursorVisibilityNotifier { get; private set; } = new global::Doroti.Framework.Foundation.ValueNotifier<bool>(true);
    internal virtual GlobalKey<IState> _editableKey { get; private set; } = GlobalKey<IState>.Create();
    public virtual ClipboardStatusNotifier clipboardStatus { get; private set; } = (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb ? new _WebClipboardStatusNotifier__editable_text() : new ClipboardStatusNotifier());
    internal virtual LiveTextInputStatusNotifier? _liveTextInputStatus { get; private set; } = (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb ? null : new LiveTextInputStatusNotifier());
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
    internal virtual global::Doroti.Framework.Services.ProcessTextService _processTextService { get; private set; } = ((global::Doroti.Framework.Services.ProcessTextService)(object?)new global::Doroti.Framework.Services.DefaultProcessTextService());
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
    internal virtual bool _platformSupportsFadeOnScroll { get; private set; } = (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform switch { global::Doroti.Framework.Foundation.TargetPlatform.android => true, global::Doroti.Framework.Foundation.TargetPlatform.iOS => true, global::Doroti.Framework.Foundation.TargetPlatform.fuchsia or global::Doroti.Framework.Foundation.TargetPlatform.linux or global::Doroti.Framework.Foundation.TargetPlatform.macOS => false, global::Doroti.Framework.Foundation.TargetPlatform.windows => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    internal virtual bool _showToolbarOnScreenScheduled { get; set; } = false;
    internal static Duration _caretAnimationDuration = Duration.Create(milliseconds: 100L);
    internal static global::Doroti.Framework.Animation.Curve _caretAnimationCurve = ((global::Doroti.Framework.Animation.Curve)(object?)global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
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
                __late_renderEditable = ((global::Doroti.Framework.Rendering.RenderEditable?)(object?)((GlobalKey<IState>)this._editableKey).currentContext!.findRenderObject()!)!;
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
                __late__transposeCharactersAction = ((Action<TransposeCharactersIntent>)(object?)new CallbackAction<TransposeCharactersIntent>(onInvoke: (__arg0) => { ((global::System.Action<TransposeCharactersIntent>)this._transposeCharacters)(__arg0); return default!; }));
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
                __late__replaceTextAction = ((Action<ReplaceTextIntent>)(object?)new CallbackAction<ReplaceTextIntent>(onInvoke: (__arg0) => { ((global::System.Action<ReplaceTextIntent>)this._replaceText)(__arg0); return default!; }));
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
                __late__updateSelectionAction = ((Action<UpdateSelectionIntent>)(object?)new CallbackAction<UpdateSelectionIntent>(onInvoke: (__arg0) => { ((global::System.Action<UpdateSelectionIntent>)this._updateSelection)(__arg0); return default!; }));
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
                __late__actions = new DartMap<Type, dynamic> { [typeof(DoNothingAndStopPropagationTextIntent)] = new DoNothingAction(consumesKey: false), [typeof(ReplaceTextIntent)] = this._replaceTextAction, [typeof(UpdateSelectionIntent)] = this._updateSelectionAction, [typeof(DirectionalFocusIntent)] = DirectionalFocusAction.CreateForTextField(), [typeof(DismissIntent)] = new CallbackAction<DismissIntent>(onInvoke: (global::System.Func<DismissIntent, object?>)this._hideToolbarIfVisible), [typeof(DeleteCharacterIntent)] = _makeOverridable(new _DeleteTextAction__editable_text<DeleteCharacterIntent>(this, (global::System.Func<global::Doroti.Framework.Services.TextBoundary>)this._characterBoundary, (global::System.Func<TextPosition, bool, global::Doroti.Framework.Services.TextBoundary, TextPosition>)this._moveBeyondTextBoundary)), [typeof(DeleteToNextWordBoundaryIntent)] = _makeOverridable(new _DeleteTextAction__editable_text<DeleteToNextWordBoundaryIntent>(this, (global::System.Func<global::Doroti.Framework.Services.TextBoundary>)this._nextWordBoundary, (global::System.Func<TextPosition, bool, global::Doroti.Framework.Services.TextBoundary, TextPosition>)this._moveBeyondTextBoundary)), [typeof(DeleteToLineBreakIntent)] = _makeOverridable(new _DeleteTextAction__editable_text<DeleteToLineBreakIntent>(this, (global::System.Func<global::Doroti.Framework.Services.TextBoundary>)this._linebreak, (global::System.Func<TextPosition, bool, global::Doroti.Framework.Services.TextBoundary, TextPosition>)this._moveToTextBoundary)), [typeof(ExtendSelectionByCharacterIntent)] = _makeOverridable(new _UpdateTextSelectionAction__editable_text<ExtendSelectionByCharacterIntent>(this, (global::System.Func<global::Doroti.Framework.Services.TextBoundary>)this._characterBoundary, (global::System.Func<TextPosition, bool, global::Doroti.Framework.Services.TextBoundary, TextPosition>)this._moveBeyondTextBoundary, ignoreNonCollapsedSelection: false)), [typeof(ExtendSelectionToNextWordBoundaryIntent)] = _makeOverridable(new _UpdateTextSelectionAction__editable_text<ExtendSelectionToNextWordBoundaryIntent>(this, (global::System.Func<global::Doroti.Framework.Services.TextBoundary>)this._nextWordBoundary, (global::System.Func<TextPosition, bool, global::Doroti.Framework.Services.TextBoundary, TextPosition>)this._moveBeyondTextBoundary, ignoreNonCollapsedSelection: true)), [typeof(ExtendSelectionToNextParagraphBoundaryIntent)] = _makeOverridable(new _UpdateTextSelectionAction__editable_text<ExtendSelectionToNextParagraphBoundaryIntent>(this, (global::System.Func<global::Doroti.Framework.Services.TextBoundary>)this._paragraphBoundary, (global::System.Func<TextPosition, bool, global::Doroti.Framework.Services.TextBoundary, TextPosition>)this._moveBeyondTextBoundary, ignoreNonCollapsedSelection: true)), [typeof(ExtendSelectionToLineBreakIntent)] = _makeOverridable(new _UpdateTextSelectionAction__editable_text<ExtendSelectionToLineBreakIntent>(this, (global::System.Func<global::Doroti.Framework.Services.TextBoundary>)this._linebreak, (global::System.Func<TextPosition, bool, global::Doroti.Framework.Services.TextBoundary, TextPosition>)this._moveToTextBoundary, ignoreNonCollapsedSelection: true)), [typeof(ExtendSelectionVerticallyToAdjacentLineIntent)] = _makeOverridable(this._verticalSelectionUpdateAction), [typeof(ExtendSelectionVerticallyToAdjacentPageIntent)] = _makeOverridable(this._verticalSelectionUpdateAction), [typeof(ExtendSelectionToNextParagraphBoundaryOrCaretLocationIntent)] = _makeOverridable(new _UpdateTextSelectionAction__editable_text<ExtendSelectionToNextParagraphBoundaryOrCaretLocationIntent>(this, (global::System.Func<global::Doroti.Framework.Services.TextBoundary>)this._paragraphBoundary, (global::System.Func<TextPosition, bool, global::Doroti.Framework.Services.TextBoundary, TextPosition>)this._moveBeyondTextBoundary, ignoreNonCollapsedSelection: true)), [typeof(ExtendSelectionToDocumentBoundaryIntent)] = _makeOverridable(new _UpdateTextSelectionAction__editable_text<ExtendSelectionToDocumentBoundaryIntent>(this, (global::System.Func<global::Doroti.Framework.Services.TextBoundary>)this._documentBoundary, (global::System.Func<TextPosition, bool, global::Doroti.Framework.Services.TextBoundary, TextPosition>)this._moveBeyondTextBoundary, ignoreNonCollapsedSelection: true)), [typeof(ExtendSelectionToNextWordBoundaryOrCaretLocationIntent)] = _makeOverridable(new _UpdateTextSelectionAction__editable_text<ExtendSelectionToNextWordBoundaryOrCaretLocationIntent>(this, (global::System.Func<global::Doroti.Framework.Services.TextBoundary>)this._nextWordBoundary, (global::System.Func<TextPosition, bool, global::Doroti.Framework.Services.TextBoundary, TextPosition>)this._moveBeyondTextBoundary, ignoreNonCollapsedSelection: true)), [typeof(ScrollToDocumentBoundaryIntent)] = _makeOverridable(new _WebComposingDisablingCallbackAction__editable_text<ScrollToDocumentBoundaryIntent>(this, onInvoke: (__arg0) => { ((global::System.Action<ScrollToDocumentBoundaryIntent>)this._scrollToDocumentBoundary)(__arg0); return default!; })), [typeof(ScrollIntent)] = new CallbackAction<ScrollIntent>(onInvoke: (__arg0) => { ((global::System.Action<ScrollIntent>)this._scroll)(__arg0); return default!; }), [typeof(ExpandSelectionToLineBreakIntent)] = _makeOverridable(new _UpdateTextSelectionAction__editable_text<ExpandSelectionToLineBreakIntent>(this, (global::System.Func<global::Doroti.Framework.Services.TextBoundary>)this._linebreak, (global::System.Func<TextPosition, bool, global::Doroti.Framework.Services.TextBoundary, TextPosition>)this._moveToTextBoundary, ignoreNonCollapsedSelection: true, isExpand: true)), [typeof(ExpandSelectionToDocumentBoundaryIntent)] = _makeOverridable(new _UpdateTextSelectionAction__editable_text<ExpandSelectionToDocumentBoundaryIntent>(this, (global::System.Func<global::Doroti.Framework.Services.TextBoundary>)this._documentBoundary, (global::System.Func<TextPosition, bool, global::Doroti.Framework.Services.TextBoundary, TextPosition>)this._moveToTextBoundary, ignoreNonCollapsedSelection: true, isExpand: true, extentAtIndex: true)), [typeof(SelectAllTextIntent)] = _makeOverridable(new _SelectAllAction__editable_text(this)), [typeof(CopySelectionTextIntent)] = _makeOverridable(new _CopySelectionAction__editable_text(this)), [typeof(PasteTextIntent)] = _makeOverridable(new _PasteSelectionAction__editable_text(this)), [typeof(TransposeCharactersIntent)] = _makeOverridable<TransposeCharactersIntent>(this._transposeCharactersAction), [typeof(EditableTextTapOutsideIntent)] = _makeOverridable(new _EditableTextTapOutsideAction__editable_text()), [typeof(EditableTextTapUpOutsideIntent)] = _makeOverridable(new _EditableTextTapUpOutsideAction__editable_text()) };
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
    __cascade.addListener(() => this._onCursorColorTick());
    return __cascade;
}))();
            return default!;
        }
    }
    internal virtual bool _hasInputConnection => DartRuntimePrimitives.ConvertValue<bool>((this._textInputConnection?.attached ?? false));
    internal virtual bool _webContextMenuEnabled => DartRuntimePrimitives.ConvertValue<bool>((global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb && global::Doroti.Framework.Services.BrowserContextMenu.enabled));
    internal virtual ScrollController _scrollController => DartRuntimePrimitives.ConvertValue<ScrollController>((((EditableText)(object)this.widget).scrollController ?? (_internalScrollController ??= new ScrollController())));
    public virtual global::Doroti.Framework.Services.AutofillScope? currentAutofillScope => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Services.AutofillScope>(this._currentAutofillScope);
    internal virtual global::Doroti.Framework.Services.AutofillClient _effectiveAutofillClient => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Services.AutofillClient>((((object?)((EditableText)(object)this.widget).autofillClient ?? (object?)this)));
    public virtual SpellCheckConfiguration spellCheckConfiguration => this._spellCheckConfiguration;
    public virtual bool spellCheckEnabled => ((SpellCheckConfiguration)this._spellCheckConfiguration).spellCheckEnabled;
    internal virtual bool _spellCheckResultsReceived => DartRuntimePrimitives.ConvertValue<bool>(((this.spellCheckEnabled && (this.spellCheckResults is not null)) && System.Linq.Enumerable.Any(this.spellCheckResults!.suggestionSpans)));
    internal virtual bool _shouldCreateInputConnection => DartRuntimePrimitives.ConvertValue<bool>(((global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb || (object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.macOS))) || !((EditableText)(object)this.widget).readOnly));
    internal virtual bool _stylusHandwritingEnabled
    {
        get
        {
            if (!((EditableText)(object)this.widget).scribbleEnabled)
            {
                return ((EditableText)(object)this.widget).scribbleEnabled;
            }
            return ((EditableText)(object)this.widget).stylusHandwritingEnabled;
            return default!;
        }
    }
    public virtual bool wantKeepAlive => ((EditableText)(object)this.widget).focusNode.hasFocus;
    internal virtual global::Doroti.Ui.Color _cursorColor
    {
        get
        {
            double effectiveOpacity = Math.Min((((EditableText)(object)this.widget).cursorColor.alpha / 255.0), ((global::Doroti.Framework.Animation.AnimationController)this._cursorBlinkOpacityController).value);
            return ((EditableText)(object)this.widget).cursorColor.withOpacity(effectiveOpacity);
            return default!;
        }
    }
    public virtual bool cutEnabled
    {
        get
        {
            if ((((EditableText)(object)this.widget).selectionControls is not TextSelectionHandleControls))
            {
                return ((((EditableText)(object)this.widget).toolbarOptions.cut && !((EditableText)(object)this.widget).readOnly) && !((EditableText)(object)this.widget).obscureText);
            }
            return ((!((EditableText)(object)this.widget).readOnly && !((EditableText)(object)this.widget).obscureText) && !((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).selection.isCollapsed);
            return default!;
        }
    }
    public virtual bool copyEnabled
    {
        get
        {
            if ((((EditableText)(object)this.widget).selectionControls is not TextSelectionHandleControls))
            {
                return (((EditableText)(object)this.widget).toolbarOptions.copy && !((EditableText)(object)this.widget).obscureText);
            }
            return (!((EditableText)(object)this.widget).obscureText && !((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).selection.isCollapsed);
            return default!;
        }
    }
    public virtual bool pasteEnabled
    {
        get
        {
            if ((((EditableText)(object)this.widget).selectionControls is not TextSelectionHandleControls))
            {
                return (((EditableText)(object)this.widget).toolbarOptions.paste && !((EditableText)(object)this.widget).readOnly);
            }
            return (!((EditableText)(object)this.widget).readOnly && ((object.Equals(this.clipboardStatus.value, ClipboardStatus.pasteable))));
            return default!;
        }
    }
    public virtual bool selectAllEnabled
    {
        get
        {
            if ((((EditableText)(object)this.widget).selectionControls is not TextSelectionHandleControls))
            {
                return ((((EditableText)(object)this.widget).toolbarOptions.selectAll && ((!((EditableText)(object)this.widget).readOnly || !((EditableText)(object)this.widget).obscureText))) && ((EditableText)(object)this.widget).enableInteractiveSelection);
            }
            if ((!((EditableText)(object)this.widget).enableInteractiveSelection || ((((EditableText)(object)this.widget).readOnly && ((EditableText)(object)this.widget).obscureText))))
            {
                return false;
            }
            switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
            {
                case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                    {
                        return false;
                    }
                case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                    {
                        return ((((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).text.Length != 0) && ((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).selection.isCollapsed);
                    }
                case global::Doroti.Framework.Foundation.TargetPlatform.android:
                case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                    {
                        return ((((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).text.Length != 0) && !(((((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).selection.start == 0L) && (((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).selection.end == ((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).text.Length))));
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
            return default!;
        }
    }
    public virtual bool lookUpEnabled
    {
        get
        {
            if ((!object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.iOS)))
            {
                return false;
            }
            return ((!((EditableText)(object)this.widget).obscureText && !((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).selection.isCollapsed) && (((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).selection.textInside(((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).text).Trim() != ""));
            return default!;
        }
    }
    public virtual bool searchWebEnabled
    {
        get
        {
            if ((!object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.iOS)))
            {
                return false;
            }
            return ((!((EditableText)(object)this.widget).obscureText && !((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).selection.isCollapsed) && (((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).selection.textInside(((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).text).Trim() != ""));
            return default!;
        }
    }
    public virtual bool shareEnabled
    {
        get
        {
            switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
            {
                case global::Doroti.Framework.Foundation.TargetPlatform.android:
                case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                    {
                        return ((!((EditableText)(object)this.widget).obscureText && !((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).selection.isCollapsed) && (((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).selection.textInside(((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).text).Trim() != ""));
                    }
                case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                    {
                        return false;
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
            return default!;
        }
    }
    public virtual bool liveTextInputEnabled
    {
        get
        {
            return ((((object.Equals(this._liveTextInputStatus?.value, LiveTextInputStatus.enabled)) && !((EditableText)(object)this.widget).obscureText) && !((EditableText)(object)this.widget).readOnly) && ((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).selection.isCollapsed);
            return default!;
        }
    }
    internal virtual void _onChangedClipboardStatus()
    {
        this._selectionOverlay?.markNeedsBuild();
        setState(((global::System.Action)(() =>
        {
        })));
    }

    internal virtual void _onChangedLiveTextInputStatus()
    {
        setState(((global::System.Action)(() =>
        {
        })));
    }

    internal virtual global::Doroti.Framework.Services.TextEditingValue _textEditingValueforTextLayoutMetrics
    {
        get
        {
            Widget? editableWidget = ((GlobalKey<IState>)this._editableKey).currentContext?.widget;
            if ((editableWidget is not _Editable__editable_text))
            {
                throw new InvalidOperationException("_Editable must be mounted.");
            }
            return ((_Editable__editable_text)((_Editable__editable_text)editableWidget)).value;
            return default!;
        }
    }
    public virtual void copySelection(global::Doroti.Framework.Services.SelectionChangedCause cause)
    {
        global::Doroti.Framework.Services.TextSelection selectionLocal = ((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).selection;
        if ((selectionLocal.isCollapsed || ((EditableText)(object)this.widget).obscureText))
        {
            return;
        }
        string textLocal = ((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).text;
        DartRuntimePrimitives.Ignore(Clipboard.setData(new global::Doroti.Framework.Services.ClipboardData(text: selectionLocal.textInside(textLocal))).catchError(_reportClipboardError("while copying selection to clipboard")));
        if ((object.Equals(DartRuntimePrimitives.RequireValue(cause), global::Doroti.Framework.Services.SelectionChangedCause.toolbar)))
        {
            bringIntoView(((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).selection.extent);
            hideToolbar(false);
            switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
            {
                case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                    {
                        break;
                    }
                case global::Doroti.Framework.Foundation.TargetPlatform.android:
                case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                    {
                        userUpdateTextEditingValue(new global::Doroti.Framework.Services.TextEditingValue(text: ((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).text, selection: global::Doroti.Framework.Services.TextSelection.CreateCollapsed(offset: ((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).selection.end)), global::Doroti.Framework.Services.SelectionChangedCause.toolbar);
                        break;
                    }
            }
        }
        DartRuntimePrimitives.Ignore(this.clipboardStatus.update());
    }

    public virtual void cutSelection(global::Doroti.Framework.Services.SelectionChangedCause cause)
    {
        if ((((EditableText)(object)this.widget).readOnly || ((EditableText)(object)this.widget).obscureText))
        {
            return;
        }
        global::Doroti.Framework.Services.TextSelection selectionLocal = ((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).selection;
        string textLocal = ((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).text;
        if (selectionLocal.isCollapsed)
        {
            return;
        }
        DartRuntimePrimitives.Ignore(Clipboard.setData(new global::Doroti.Framework.Services.ClipboardData(text: selectionLocal.textInside(textLocal))).catchError(_reportClipboardError("while cutting selection to clipboard")));
        _replaceText(new ReplaceTextIntent(this.textEditingValue, "", selectionLocal, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(cause))));
        if ((object.Equals(DartRuntimePrimitives.RequireValue(cause), global::Doroti.Framework.Services.SelectionChangedCause.toolbar)))
        {
            global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) =>
            {
                if (this.mounted)
                {
                    bringIntoView(((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).selection.extent);
                }
            })), debugLabel: "EditableText.bringSelectionIntoView");
            hideToolbar();
        }
        DartRuntimePrimitives.Ignore(this.clipboardStatus.update());
    }

    internal virtual global::System.Action<object, global::System.Diagnostics.StackTrace> _reportClipboardError(string context)
    {
        return ((global::System.Action<object, global::System.Diagnostics.StackTrace>)((exception, stack) =>
        {
            FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exception, stack: stack, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription(context)));
        }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _allowPaste
    {
        get
        {
            return (!((EditableText)(object)this.widget).readOnly && ((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).selection.isValid);
            return default!;
        }
    }
    public async virtual Future pasteText(global::Doroti.Framework.Services.SelectionChangedCause cause)
    {
        if (!this._allowPaste)
        {
            return;
        }
        global::Doroti.Framework.Services.ClipboardData? data = await Clipboard.getData(global::Doroti.Framework.Services.Clipboard.kTextPlain);
        if ((data is null))
        {
            return;
        }
        _pasteText(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(cause)), ((global::Doroti.Framework.Services.ClipboardData)data).text!);
    }

    internal virtual void _pasteText(global::Doroti.Framework.Services.SelectionChangedCause cause, string text)
    {
        if (!this._allowPaste)
        {
            return;
        }
        global::Doroti.Framework.Services.TextSelection selectionLocal = ((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).selection;
        long lastSelectionIndex = Math.Max(((global::Doroti.Framework.Services.TextSelection)selectionLocal).baseOffset, ((global::Doroti.Framework.Services.TextSelection)selectionLocal).extentOffset);
        global::Doroti.Framework.Services.TextEditingValue collapsedTextEditingValue = ((global::Doroti.Framework.Services.TextEditingValue)(object?)this.textEditingValue.copyWith(selection: global::Doroti.Framework.Services.TextSelection.CreateCollapsed(offset: lastSelectionIndex)));
        userUpdateTextEditingValue(collapsedTextEditingValue.replaced(selectionLocal, text), DartRuntimePrimitives.RequireValue(cause));
        if ((object.Equals(DartRuntimePrimitives.RequireValue(cause), global::Doroti.Framework.Services.SelectionChangedCause.toolbar)))
        {
            global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) =>
            {
                if (this.mounted)
                {
                    bringIntoView(((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).selection.extent);
                }
            })), debugLabel: "EditableText.bringSelectionIntoView");
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
        if ((((EditableText)(object)this.widget).readOnly && ((EditableText)(object)this.widget).obscureText))
        {
            return;
        }
        userUpdateTextEditingValue(this.textEditingValue.copyWith(selection: new global::Doroti.Framework.Services.TextSelection(baseOffset: 0L, extentOffset: ((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).text.Length)), DartRuntimePrimitives.RequireValue(cause));
        if ((object.Equals(DartRuntimePrimitives.RequireValue(cause), global::Doroti.Framework.Services.SelectionChangedCause.toolbar)))
        {
            switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
            {
                case global::Doroti.Framework.Foundation.TargetPlatform.android:
                case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                    {
                        break;
                    }
                case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                    {
                        hideToolbar();
                        break;
                    }
            }
            switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
            {
                case global::Doroti.Framework.Foundation.TargetPlatform.android:
                case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                    {
                        bringIntoView(((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).selection.extent);
                        break;
                    }
                case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                    {
                        break;
                    }
            }
        }
    }

    public async virtual Future lookUpSelection(global::Doroti.Framework.Services.SelectionChangedCause cause)
    {
        DartRuntimePrimitives.Assert(() => !((EditableText)(object)this.widget).obscureText);
        string textLocal = ((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).selection.textInside(((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).text);
        if ((((EditableText)(object)this.widget).obscureText || (textLocal.Length == 0)))
        {
            return;
        }
        await global::Doroti.Framework.Services.SystemChannels.platform.invokeMethod<object>("LookUp.invoke", textLocal);
    }

    public async virtual Future searchWebForSelection(global::Doroti.Framework.Services.SelectionChangedCause cause)
    {
        DartRuntimePrimitives.Assert(() => !((EditableText)(object)this.widget).obscureText);
        if (((EditableText)(object)this.widget).obscureText)
        {
            return;
        }
        string textLocal = ((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).selection.textInside(((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).text);
        if ((textLocal.Length != 0))
        {
            await global::Doroti.Framework.Services.SystemChannels.platform.invokeMethod<object>("SearchWeb.invoke", textLocal);
        }
    }

    public async virtual Future shareSelection(global::Doroti.Framework.Services.SelectionChangedCause cause)
    {
        DartRuntimePrimitives.Assert(() => !((EditableText)(object)this.widget).obscureText);
        if (((EditableText)(object)this.widget).obscureText)
        {
            return;
        }
        string textLocal = ((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).selection.textInside(((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).text);
        if ((textLocal.Length != 0))
        {
            await global::Doroti.Framework.Services.SystemChannels.platform.invokeMethod<object>("Share.invoke", textLocal);
        }
    }

    internal virtual void _startLiveTextInput(global::Doroti.Framework.Services.SelectionChangedCause cause)
    {
        if (!this.liveTextInputEnabled)
        {
            return;
        }
        if (this._hasInputConnection)
        {
            DartRuntimePrimitives.Ignore(LiveText.startLiveTextInput().then(((_) =>
            {
            }), onError: ((error, stack) =>
            {
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: error, stack: stack, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while starting Live Text input")));
            })));
        }
        if ((object.Equals(DartRuntimePrimitives.RequireValue(cause), global::Doroti.Framework.Services.SelectionChangedCause.toolbar)))
        {
            hideToolbar();
        }
    }

    public virtual global::Doroti.Framework.Services.SuggestionSpan? findSuggestionSpanAtCursorIndex(long cursorIndex)
    {
        if ((!this._spellCheckResultsReceived || (this.spellCheckResults!.suggestionSpans.Last().range.end < cursorIndex)))
        {
            return ((global::Doroti.Framework.Services.SuggestionSpan)(object)null);
        }
        List<global::Doroti.Framework.Services.SuggestionSpan> suggestionSpansLocal = this.spellCheckResults!.suggestionSpans.ToList();
        var leftIndex = 0L;
        long rightIndex = (checked((long)(suggestionSpansLocal.Count)) - 1L);
        var midIndex = 0L;
        while ((leftIndex <= rightIndex))
        {
            midIndex = ((((leftIndex + rightIndex)) / 2L)).floor();
            long currentSpanStart = suggestionSpansLocal[(int)(midIndex)].range.start;
            long currentSpanEnd = suggestionSpansLocal[(int)(midIndex)].range.end;
            if (((cursorIndex <= currentSpanEnd) && (cursorIndex >= currentSpanStart)))
            {
                return suggestionSpansLocal[(int)(midIndex)];
            }
            else
            {
                if ((cursorIndex <= currentSpanStart))
                {
                    rightIndex = (midIndex - 1L);
                }
                else
                {
                    leftIndex = (midIndex + 1L);
                }
            }
        }
        return ((global::Doroti.Framework.Services.SuggestionSpan)(object)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static SpellCheckConfiguration _inferSpellCheckConfiguration(SpellCheckConfiguration? configuration, bool obscureText, global::Doroti.Framework.Services.TextInputType keyboardType, IEnumerable<string>? autofillHints)
    {
        global::Doroti.Framework.Services.SpellCheckService? spellCheckServiceLocal = configuration?.spellCheckService;
        bool spellCheckAutomaticallyDisabled = ((EditableTextState._isPasswordInput(obscureText: obscureText, keyboardType: keyboardType, autofillHints: autofillHints.Cast<string>()) || (configuration is null)) || (object.Equals(configuration, SpellCheckConfiguration.CreateDisabled())));
        bool spellCheckServiceIsConfigured = ((spellCheckServiceLocal is not null) || WidgetsBinding.instance.platformDispatcher.nativeSpellCheckServiceDefined);
        if ((spellCheckAutomaticallyDisabled || !spellCheckServiceIsConfigured))
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((!spellCheckAutomaticallyDisabled && !spellCheckServiceIsConfigured))
                    {
                        FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: global::Doroti.Framework.Foundation.FlutterError.Create("Spell check was enabled with spellCheckConfiguration, but the " + "current platform does not have a supported spell check " + "service, and none was provided. Consider disabling spell " + "check for this platform or passing a SpellCheckConfiguration " + "with a specified spell check service."), library: "widget library", stack: new global::System.Diagnostics.StackTrace(true)));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            return SpellCheckConfiguration.CreateDisabled();
        }
        return ((SpellCheckConfiguration)(object?)configuration.copyWith(spellCheckService: DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Services.SpellCheckService>(spellCheckServiceLocal ?? new global::Doroti.Framework.Services.DefaultSpellCheckService())));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static bool _isPasswordInput(bool obscureText, global::Doroti.Framework.Services.TextInputType keyboardType, IEnumerable<string>? autofillHints)
    {
        return ((obscureText || (object.Equals(keyboardType, global::Doroti.Framework.Services.TextInputType.visiblePassword))) || ((autofillHints?.any(((hint) => ((hint == global::Doroti.Framework.Services.AutofillHints.password) || (hint == global::Doroti.Framework.Services.AutofillHints.newPassword)))) ?? false)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<ContextMenuButtonItem>? buttonItemsForToolbarOptions(global::Doroti.Framework.Foundation.TargetPlatform? targetPlatform = null)
    {
        ToolbarOptions toolbarOptionsLocal = ((EditableText)(object)this.widget).toolbarOptions;
        if ((object.Equals(toolbarOptionsLocal, ToolbarOptions.empty)))
        {
            return ((List<ContextMenuButtonItem>)(object)null);
        }
        var buttonItems = new List<ContextMenuButtonItem>();
        if (toolbarOptionsLocal.cut && cutEnabled)
        {
            buttonItems.Add(new ContextMenuButtonItem(onPressed: () => cutSelection(global::Doroti.Framework.Services.SelectionChangedCause.toolbar), type: ContextMenuButtonType.cut));
        }
        if (toolbarOptionsLocal.copy && copyEnabled)
        {
            buttonItems.Add(new ContextMenuButtonItem(onPressed: () => copySelection(global::Doroti.Framework.Services.SelectionChangedCause.toolbar), type: ContextMenuButtonType.copy));
        }
        if (toolbarOptionsLocal.paste && pasteEnabled)
        {
            buttonItems.Add(new ContextMenuButtonItem(onPressed: () => { _ = _pasteTextWithReporting(global::Doroti.Framework.Services.SelectionChangedCause.toolbar); }, type: ContextMenuButtonType.paste));
        }
        if (toolbarOptionsLocal.selectAll && selectAllEnabled)
        {
            buttonItems.Add(new ContextMenuButtonItem(onPressed: () => selectAll(global::Doroti.Framework.Services.SelectionChangedCause.toolbar), type: ContextMenuButtonType.selectAll));
        }
        return buttonItems;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual (double startGlyphHeight, double endGlyphHeight) getGlyphHeights()
    {
        global::Doroti.Framework.Services.TextSelection selectionLocal = ((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).selection;
        global::Doroti.Framework.Painting.InlineSpan span = ((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).text!;
        string prevText = ((string)(object?)span.toPlainText());
        string currText = ((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).text;
        if ((((prevText != currText) || !selectionLocal.isValid) || selectionLocal.isCollapsed))
        {
            return (startGlyphHeight: ((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).preferredLineHeight, endGlyphHeight: ((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).preferredLineHeight);
        }
        string selectedGraphemes = selectionLocal.textInside(currText);
        long firstSelectedGraphemeExtent = selectedGraphemes.characters().first.Length;
        global::Doroti.Ui.Rect? startCharacterRect = ((global::Doroti.Ui.Rect?)(object?)((Rect?)((dynamic)this.renderEditable).getRectForComposingRange(new global::Doroti.Ui.TextRange(start: selectionLocal.start, end: (selectionLocal.start + firstSelectedGraphemeExtent)))));
        long lastSelectedGraphemeExtent = selectedGraphemes.characters().last.Length;
        global::Doroti.Ui.Rect? endCharacterRect = ((global::Doroti.Ui.Rect?)(object?)((Rect?)((dynamic)this.renderEditable).getRectForComposingRange(new global::Doroti.Ui.TextRange(start: (selectionLocal.end - lastSelectedGraphemeExtent), end: selectionLocal.end))));
        return (startGlyphHeight: ((startCharacterRect?.height ?? (double)((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).preferredLineHeight)), endGlyphHeight: ((endCharacterRect?.height ?? (double)((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).preferredLineHeight)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TextSelectionToolbarAnchors contextMenuAnchors
    {
        get
        {
            if ((((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).lastSecondaryTapDownPosition is not null))
            {
                return new TextSelectionToolbarAnchors(primaryAnchor: DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).lastSecondaryTapDownPosition));
            }
            var (startGlyphHeightLocal, endGlyphHeightLocal) = getGlyphHeights();
            global::Doroti.Framework.Services.TextSelection selectionLocal = ((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).selection;
            List<global::Doroti.Framework.Rendering.TextSelectionPoint> points = ((List<global::Doroti.Framework.Rendering.TextSelectionPoint>)(object?)((List<global::Doroti.Framework.Rendering.TextSelectionPoint>)((dynamic)this.renderEditable).getEndpointsForSelection(selectionLocal)));
            return TextSelectionToolbarAnchors.CreateFromSelection(renderBox: this.renderEditable, startGlyphHeight: startGlyphHeightLocal, endGlyphHeight: endGlyphHeightLocal, selectionEndpoints: points);
            return default!;
        }
    }
    public virtual List<ContextMenuButtonItem> contextMenuButtonItems
    {
        get
        {
            return ((Func<List<ContextMenuButtonItem>>)(() =>
{
    var __cascade = ((buttonItemsForToolbarOptions() ?? (List<ContextMenuButtonItem>)EditableText.getEditableButtonItems(clipboardStatus: this.clipboardStatus.value, onCopy: ((global::System.Action)(this.copyEnabled ? (() => { copySelection(global::Doroti.Framework.Services.SelectionChangedCause.toolbar); }) : null)), onCut: ((global::System.Action)(this.cutEnabled ? (() => { cutSelection(global::Doroti.Framework.Services.SelectionChangedCause.toolbar); }) : null)), onPaste: ((global::System.Action)(this.pasteEnabled ? (() => { _ = _pasteTextWithReporting(global::Doroti.Framework.Services.SelectionChangedCause.toolbar); }) : null)), onSelectAll: ((global::System.Action)(this.selectAllEnabled ? (() => { selectAll(global::Doroti.Framework.Services.SelectionChangedCause.toolbar); }) : null)), onLookUp: ((global::System.Action)(this.lookUpEnabled ? (() => { _ = lookUpSelection(global::Doroti.Framework.Services.SelectionChangedCause.toolbar); }) : null)), onSearchWeb: ((global::System.Action)(this.searchWebEnabled ? (() => { _ = searchWebForSelection(global::Doroti.Framework.Services.SelectionChangedCause.toolbar); }) : null)), onShare: ((global::System.Action)(this.shareEnabled ? (() => { _ = shareSelection(global::Doroti.Framework.Services.SelectionChangedCause.toolbar); }) : null)), onLiveTextInput: ((global::System.Action)(this.liveTextInputEnabled ? (() => { _startLiveTextInput(global::Doroti.Framework.Services.SelectionChangedCause.toolbar); }) : null)))));
    __cascade.AddRange(this._textProcessingActionButtonItems.Cast<ContextMenuButtonItem>());
    return __cascade;
}))();
            return default!;
        }
    }
    internal virtual List<ContextMenuButtonItem> _textProcessingActionButtonItems
    {
        get
        {
            var buttonItems = new List<ContextMenuButtonItem>();
            global::Doroti.Framework.Services.TextSelection selectionLocal = ((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).selection;
            if (((((EditableText)(object)this.widget).obscureText || !selectionLocal.isValid) || selectionLocal.isCollapsed))
            {
                return buttonItems;
            }
            foreach (global::Doroti.Framework.Services.ProcessTextAction action in this._processTextActions)
            {
                buttonItems.Add(new ContextMenuButtonItem(label: ((global::Doroti.Framework.Services.ProcessTextAction)action).label, onPressed: ((global::System.Action)(async () =>
                {
                    string selectedText = selectionLocal.textInside(((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).text);
                    if ((selectedText.Length != 0))
                    {
                        string? processedText = await this._processTextService.processTextAction(((global::Doroti.Framework.Services.ProcessTextAction)action).id, selectedText, ((EditableText)(object)this.widget).readOnly);
                        if (((processedText is not null) && this._allowPaste))
                        {
                            _pasteText(global::Doroti.Framework.Services.SelectionChangedCause.toolbar, processedText);
                        }
                        else
                        {
                            hideToolbar();
                        }
                    }
                }))));
            }
            return buttonItems;
            return default!;
        }
    }
    public override void initState()
    {
        base.initState();
        if (this.wantKeepAlive)
        {
            _ensureKeepAlive();
        }
        this._liveTextInputStatus?.addListener(this._onChangedLiveTextInputStatus);
        this.clipboardStatus.addListener(this._onChangedClipboardStatus);
        ((EditableText)(object)this.widget).controller.addListener(this._didChangeTextEditingValue);
        ((EditableText)(object)this.widget).focusNode.addListener(this._handleFocusChanged);
        this._cursorVisibilityNotifier.value = ((EditableText)(object)this.widget).showCursor;
        _spellCheckConfiguration = EditableTextState._inferSpellCheckConfiguration(((EditableText)(object)this.widget).spellCheckConfiguration, obscureText: ((EditableText)(object)this.widget).obscureText, keyboardType: ((EditableText)(object)this.widget).keyboardType, autofillHints: ((EditableText)(object)this.widget).autofillHints.Cast<string>());
        _appLifecycleListener = new AppLifecycleListener(onResume: () => this._onResume());
        DartRuntimePrimitives.Ignore(_initProcessTextActions());
    }

    internal virtual void _onResume()
    {
        _justResumed = true;
        FocusManager.instance.removeListener(() => this._resetJustResumed());
        FocusManager.instance.addListener(() => this._resetJustResumed());
    }

    internal virtual void _resetJustResumed()
    {
        _justResumed = false;
        FocusManager.instance.removeListener(() => this._resetJustResumed());
    }

    internal async virtual Future _initProcessTextActions()
    {
        this._processTextActions.Clear();
        this._processTextActions.AddRange((await this._processTextService.queryTextActions()).Cast<global::Doroti.Framework.Services.ProcessTextAction>());
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _style = (MediaQuery.boldTextOf(this.context) ? ((EditableText)(object)this.widget).style.merge(new global::Doroti.Framework.Painting.TextStyle(fontWeight: FontWeight.bold)) : ((EditableText)(object)this.widget).style);
        AutofillGroupState? newAutofillGroup = ((AutofillGroupState?)(object?)AutofillGroup.maybeOf(this.context));
        if ((!object.Equals(this.currentAutofillScope, newAutofillGroup)))
        {
            this._currentAutofillScope?.unregister(this.autofillId);
            _currentAutofillScope = newAutofillGroup;
            this._currentAutofillScope?.register(this._effectiveAutofillClient);
        }
        if ((!this._didAutoFocus && ((EditableText)(object)this.widget).autofocus))
        {
            _didAutoFocus = true;
            global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) =>
            {
                if ((this.mounted && this.renderEditable.hasSize))
                {
                    _flagInternalFocus();
                    FocusScope.of(this.context).autofocus(((EditableText)(object)this.widget).focusNode);
                }
            })), debugLabel: "EditableText.autofocus");
        }
        bool newTickerEnabled = TickerMode.of(this.context);
        if ((this._tickersEnabled != newTickerEnabled))
        {
            _tickersEnabled = newTickerEnabled;
            if (this._showBlinkingCursor)
            {
                _startCursorBlink();
            }
            else
            {
                if ((!this._tickersEnabled && (this._cursorTimer is not null)))
                {
                    _stopCursorBlink();
                }
            }
        }
        if (this._hasInputConnection)
        {
            long newViewId = checked((long)View.of(this.context).viewId);
            if ((newViewId != this._viewId))
            {
                this._textInputConnection!.updateConfig(((global::Doroti.Framework.Services.AutofillClient)this._effectiveAutofillClient).textInputConfiguration);
            }
            global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) =>
            {
                if ((!this.mounted || !this._hasInputConnection))
                {
                    return;
                }
                this._textInputConnection!.updateStyle(_getTextInputStyle(this.context));
            })), debugLabel: "EditableText.updateStyle");
        }
        if (((!object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.iOS)) && (!object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.android))))
        {
            return;
        }
        Orientation orientation = MediaQuery.orientationOf(this.context);
        if ((this._lastOrientation is null))
        {
            _lastOrientation = orientation;
            return;
        }
        if ((!object.Equals(orientation, this._lastOrientation)))
        {
            _lastOrientation = orientation;
            if ((object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.iOS)))
            {
                hideToolbar(false);
            }
            if ((object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.android)))
            {
                hideToolbar();
            }
        }
        if (this._listeningToScrollNotificationObserver)
        {
            this._scrollNotificationObserver?.removeListener((global::System.Action<ScrollNotification>)this._handleContextMenuOnParentScroll);
            _scrollNotificationObserver = ScrollNotificationObserver.maybeOf(this.context);
            this._scrollNotificationObserver?.addListener((global::System.Action<ScrollNotification>)this._handleContextMenuOnParentScroll);
        }
    }

    public override void didUpdateWidget(EditableText oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((EditableText)(object)this.widget).controller, ((EditableText)oldWidget).controller)))
        {
            ((EditableText)oldWidget).controller.removeListener(this._didChangeTextEditingValue);
            ((EditableText)(object)this.widget).controller.addListener(this._didChangeTextEditingValue);
            _updateRemoteEditingValueIfNeeded();
        }
        TextSelectionOverlay? selectionOverlay = this._selectionOverlay;
        if (((((selectionOverlay is not null) && ((TextSelectionOverlay)selectionOverlay).toolbarIsVisible) && (!object.Equals((global::System.Func<BuildContext, EditableTextState, Widget>?)((EditableText)(object)this.widget).contextMenuBuilder, (global::System.Func<BuildContext, EditableTextState, Widget>?)((EditableText)oldWidget).contextMenuBuilder))) && (((((EditableText)(object)this.widget).contextMenuBuilder is null)) == ((((EditableText)oldWidget).contextMenuBuilder is null)))))
        {
            WidgetsBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) =>
            {
                if ((this.mounted && ((this._selectionOverlay?.toolbarIsVisible ?? false))))
                {
                    this._selectionOverlay!.showToolbar();
                }
            })));
        }
        if (((this._selectionOverlay is not null) && ((((((((((EditableText)(object)this.widget).contextMenuBuilder is null)) != ((((EditableText)oldWidget).contextMenuBuilder is null))) || (!object.Equals(((EditableText)(object)this.widget).selectionControls, ((EditableText)oldWidget).selectionControls))) || (!object.Equals((global::System.Action?)((EditableText)(object)this.widget).onSelectionHandleTapped, (global::System.Action?)((EditableText)oldWidget).onSelectionHandleTapped))) || (!object.Equals(((EditableText)(object)this.widget).dragStartBehavior, ((EditableText)oldWidget).dragStartBehavior))) || (!object.Equals(((EditableText)(object)this.widget).magnifierConfiguration, ((EditableText)oldWidget).magnifierConfiguration))))))
        {
            bool shouldShowToolbar = this._selectionOverlay!.toolbarIsVisible;
            bool shouldShowHandles = this._selectionOverlay!.handlesVisible;
            this._selectionOverlay!.dispose();
            _selectionOverlay = _createSelectionOverlay();
            if ((shouldShowToolbar || shouldShowHandles))
            {
                global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) =>
                {
                    if (shouldShowToolbar)
                    {
                        this._selectionOverlay!.showToolbar();
                    }
                    if (shouldShowHandles)
                    {
                        this._selectionOverlay!.showHandles();
                    }
                })));
            }
        }
        else
        {
            if ((!object.Equals(((EditableText)(object)this.widget).controller.selection, ((EditableText)oldWidget).controller.selection)))
            {
                this._selectionOverlay?.update(this._value);
            }
        }
        this._selectionOverlay?.handlesVisible = ((EditableText)(object)this.widget).showSelectionHandles;
        if ((!object.Equals(((EditableText)(object)this.widget).autofillClient, ((EditableText)oldWidget).autofillClient)))
        {
            this._currentAutofillScope?.unregister(((((EditableText)oldWidget).autofillClient?.autofillId ?? (string)this.autofillId)));
            this._currentAutofillScope?.register(this._effectiveAutofillClient);
        }
        if ((!object.Equals(((EditableText)(object)this.widget).focusNode, ((EditableText)oldWidget).focusNode)))
        {
            ((EditableText)oldWidget).focusNode.removeListener(this._handleFocusChanged);
            ((EditableText)(object)this.widget).focusNode.addListener(this._handleFocusChanged);
            updateKeepAlive();
        }
        if (!this._shouldCreateInputConnection)
        {
            _closeInputConnectionIfNeeded();
        }
        else
        {
            if ((((EditableText)oldWidget).readOnly && this._hasFocus))
            {
                global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) =>
                {
                    _openInputConnection();
                })), debugLabel: "EditableText.openInputConnection");
            }
        }
        if ((global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb && this._hasInputConnection))
        {
            if ((((EditableText)oldWidget).readOnly != ((EditableText)(object)this.widget).readOnly))
            {
                this._textInputConnection!.updateConfig(((global::Doroti.Framework.Services.AutofillClient)this._effectiveAutofillClient).textInputConfiguration);
            }
        }
        if (this._hasInputConnection)
        {
            var obscureTextChanged = (((EditableText)oldWidget).obscureText != ((EditableText)(object)this.widget).obscureText);
            if ((obscureTextChanged || (!object.Equals(((EditableText)oldWidget).keyboardType, ((EditableText)(object)this.widget).keyboardType))))
            {
                if (obscureTextChanged)
                {
                    _obscureShowCharTicksPending = 0L;
                    _obscureLatestCharIndex = null;
                }
                this._textInputConnection!.updateConfig(((global::Doroti.Framework.Services.AutofillClient)this._effectiveAutofillClient).textInputConfiguration);
            }
        }
        if (((((!object.Equals(((EditableText)oldWidget).spellCheckConfiguration, ((EditableText)(object)this.widget).spellCheckConfiguration)) || (((EditableText)oldWidget).obscureText != ((EditableText)(object)this.widget).obscureText)) || (!object.Equals(((EditableText)oldWidget).keyboardType, ((EditableText)(object)this.widget).keyboardType))) || !global::Doroti.Framework.Foundation.CollectionsLibrary.listEquals<string>(((EditableText)oldWidget).autofillHints?.ToList().Cast<string>().ToList(), ((EditableText)(object)this.widget).autofillHints?.ToList().Cast<string>().ToList())))
        {
            _spellCheckConfiguration = EditableTextState._inferSpellCheckConfiguration(((EditableText)(object)this.widget).spellCheckConfiguration, obscureText: ((EditableText)(object)this.widget).obscureText, keyboardType: ((EditableText)(object)this.widget).keyboardType, autofillHints: ((EditableText)(object)this.widget).autofillHints.Cast<string>());
            if (this.spellCheckEnabled)
            {
                if ((((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).text.Length != 0))
                {
                    DartRuntimePrimitives.Ignore(_performSpellCheck(((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).text));
                }
            }
            else
            {
                spellCheckResults = null;
            }
        }
        if ((!object.Equals(((EditableText)(object)this.widget).style, ((EditableText)oldWidget).style)))
        {
            _style = (MediaQuery.boldTextOf(this.context) ? ((EditableText)(object)this.widget).style.merge(new global::Doroti.Framework.Painting.TextStyle(fontWeight: FontWeight.bold)) : ((EditableText)(object)this.widget).style);
            if (this._hasInputConnection)
            {
                global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) =>
                {
                    if ((!this.mounted || !this._hasInputConnection))
                    {
                        return;
                    }
                    this._textInputConnection!.updateStyle(_getTextInputStyle(this.context));
                })), debugLabel: "EditableText.updateStyle");
            }
        }
        if ((((EditableText)(object)this.widget).showCursor != ((EditableText)oldWidget).showCursor))
        {
            _startOrStopCursorTimerIfNeeded();
        }
        bool canPasteLocal = ((((EditableText)(object)this.widget).selectionControls is TextSelectionHandleControls) ? this.pasteEnabled : (((EditableText)(object)this.widget).selectionControls?.canPaste(this) ?? false));
        if (((((EditableText)(object)this.widget).selectionEnabled && this.pasteEnabled) && canPasteLocal))
        {
            DartRuntimePrimitives.Ignore(this.clipboardStatus.update());
        }
    }

    internal virtual void _disposeScrollNotificationObserver()
    {
        _listeningToScrollNotificationObserver = false;
        if ((this._scrollNotificationObserver is not null))
        {
            this._scrollNotificationObserver!.removeListener((global::System.Action<ScrollNotification>)this._handleContextMenuOnParentScroll);
            _scrollNotificationObserver = null;
        }
    }

    internal virtual global::Doroti.Framework.Services.TextInputStyle _getTextInputStyle(BuildContext context)
    {
        double? letterSpacingOverride = MediaQuery.maybeLetterSpacingOverrideOf(context);
        double? wordSpacingOverride = MediaQuery.maybeWordSpacingOverrideOf(context);
        return new global::Doroti.Framework.Services.TextInputStyle(fontFamily: ((global::Doroti.Framework.Painting.TextStyle)this._style).fontFamily, fontSize: ((global::Doroti.Framework.Painting.TextStyle)this._style).fontSize, fontWeight: ((global::Doroti.Framework.Painting.TextStyle)this._style).fontWeight, textDirection: this._textDirection, textAlign: ((EditableText)(object)this.widget).textAlign, letterSpacing: (letterSpacingOverride ?? ((global::Doroti.Framework.Painting.TextStyle)this._style).letterSpacing), wordSpacing: (wordSpacingOverride ?? ((global::Doroti.Framework.Painting.TextStyle)this._style).wordSpacing), lineHeight: ((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).preferredLineHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        this._internalScrollController?.dispose();
        this._currentAutofillScope?.unregister(this.autofillId);
        ((EditableText)(object)this.widget).controller.removeListener(this._didChangeTextEditingValue);
        this._floatingCursorResetController?.dispose();
        _floatingCursorResetController = null;
        _closeInputConnectionIfNeeded();
        DartRuntimePrimitives.Assert(() => !this._hasInputConnection);
        this._cursorTimer?.cancel();
        _cursorTimer = null;
        this._backingCursorBlinkOpacityController?.dispose();
        _backingCursorBlinkOpacityController = null;
        this._selectionOverlay?.dispose();
        _selectionOverlay = null;
        ((EditableText)(object)this.widget).focusNode.removeListener(this._handleFocusChanged);
        WidgetsBinding.instance.removeObserver(this);
        this._liveTextInputStatus?.removeListener(this._onChangedLiveTextInputStatus);
        this._liveTextInputStatus?.dispose();
        this.clipboardStatus.removeListener(this._onChangedClipboardStatus);
        this.clipboardStatus.dispose();
        this._cursorVisibilityNotifier.dispose();
        this._appLifecycleListener.dispose();
        FocusManager.instance.removeListener(() => this._unflagInternalFocus());
        FocusManager.instance.removeListener(() => this._resetJustResumed());
        _disposeScrollNotificationObserver();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        _tickerModeNotifier = null;
        base.dispose();
        DartRuntimePrimitives.Assert(() => (this._batchEditDepth <= 0L), () => (object?)$"unfinished batch edits: {this._batchEditDepth}");
    }

    public virtual global::Doroti.Framework.Services.TextEditingValue? currentTextEditingValue => this._value;
    public virtual void updateEditingValue(global::Doroti.Framework.Services.TextEditingValue value)
    {
        if (!this._shouldCreateInputConnection)
        {
            return;
        }
        if (_checkNeedsAdjustAffinity(value))
        {
            value = value.copyWith(selection: ((global::Doroti.Framework.Services.TextEditingValue)value).selection.copyWith(affinity: ((global::Doroti.Framework.Services.TextEditingValue)this._value).selection.affinity));
        }
        if (((EditableText)(object)this.widget).readOnly)
        {
            value = this._value.copyWith(selection: ((global::Doroti.Framework.Services.TextEditingValue)value).selection);
        }
        _lastKnownRemoteTextEditingValue = value;
        if ((object.Equals(value, this._value)))
        {
            return;
        }
        if (((((global::Doroti.Framework.Services.TextEditingValue)value).text == ((global::Doroti.Framework.Services.TextEditingValue)this._value).text) && (object.Equals(((global::Doroti.Framework.Services.TextEditingValue)value).composing, ((global::Doroti.Framework.Services.TextEditingValue)this._value).composing))))
        {
            global::Doroti.Framework.Services.SelectionChangedCause cause = default!;
            if ((this._textInputConnection?.scribbleInProgress ?? false))
            {
                cause = global::Doroti.Framework.Services.SelectionChangedCause.stylusHandwriting;
            }
            else
            {
                if ((this._pointOffsetOrigin is not null))
                {
                    cause = global::Doroti.Framework.Services.SelectionChangedCause.forcePress;
                }
                else
                {
                    cause = global::Doroti.Framework.Services.SelectionChangedCause.keyboard;
                }
            }
            _handleSelectionChanged(((global::Doroti.Framework.Services.TextEditingValue)value).selection, DartRuntimePrimitives.RequireValue(cause));
        }
        else
        {
            if ((((global::Doroti.Framework.Services.TextEditingValue)value).text != ((global::Doroti.Framework.Services.TextEditingValue)this._value).text))
            {
                hideToolbar(false);
            }
            _currentPromptRectRange = null;
            bool revealObscuredInput = (((this._hasInputConnection && ((EditableText)(object)this.widget).obscureText) && WidgetsBinding.instance.platformDispatcher.brieflyShowPassword) && (((global::Doroti.Framework.Services.TextEditingValue)value).text.Length == (((global::Doroti.Framework.Services.TextEditingValue)this._value).text.Length + 1L)));
            _obscureShowCharTicksPending = (revealObscuredInput ? Editable_textLibrary._kObscureShowLatestCharCursorTicks : 0L);
            _obscureLatestCharIndex = (revealObscuredInput ? ((global::Doroti.Framework.Services.TextEditingValue)this._value).selection.baseOffset : null);
            _formatAndSetValue(value, global::Doroti.Framework.Services.SelectionChangedCause.keyboard);
        }
        if ((this._showBlinkingCursor && (this._cursorTimer is not null)))
        {
            _stopCursorBlink(resetCharTicks: false);
            _startCursorBlink();
        }
        _scheduleShowCaretOnScreen(withAnimation: true);
    }

    internal virtual bool _checkNeedsAdjustAffinity(global::Doroti.Framework.Services.TextEditingValue value)
    {
        return ((((((global::Doroti.Framework.Services.TextEditingValue)value).text == ((global::Doroti.Framework.Services.TextEditingValue)this._value).text) && (((global::Doroti.Framework.Services.TextEditingValue)value).selection.isCollapsed == ((global::Doroti.Framework.Services.TextEditingValue)this._value).selection.isCollapsed)) && (((global::Doroti.Framework.Services.TextEditingValue)value).selection.start == ((global::Doroti.Framework.Services.TextEditingValue)this._value).selection.start)) && (!object.Equals(((global::Doroti.Framework.Services.TextEditingValue)value).selection.affinity, ((global::Doroti.Framework.Services.TextEditingValue)this._value).selection.affinity)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void performAction(global::Doroti.Framework.Services.TextInputAction action)
    {
        switch (action)
        {
            case global::Doroti.Framework.Services.TextInputAction.newline:
                {
                    if (!this._isMultiline)
                    {
                        _finalizeEditing(action, shouldUnfocus: true);
                    }
                    break;
                }
            case global::Doroti.Framework.Services.TextInputAction.done:
            case global::Doroti.Framework.Services.TextInputAction.go:
            case global::Doroti.Framework.Services.TextInputAction.next:
            case global::Doroti.Framework.Services.TextInputAction.previous:
            case global::Doroti.Framework.Services.TextInputAction.search:
            case global::Doroti.Framework.Services.TextInputAction.send:
                {
                    _finalizeEditing(action, shouldUnfocus: true);
                    break;
                }
            case global::Doroti.Framework.Services.TextInputAction.continueAction:
            case global::Doroti.Framework.Services.TextInputAction.emergencyCall:
            case global::Doroti.Framework.Services.TextInputAction.join:
            case global::Doroti.Framework.Services.TextInputAction.none:
            case global::Doroti.Framework.Services.TextInputAction.route:
            case global::Doroti.Framework.Services.TextInputAction.unspecified:
                {
                    _finalizeEditing(action, shouldUnfocus: false);
                    break;
                }
        }
    }

    public virtual void performPrivateCommand(string action, DartMap<string, object> data)
    {
        ((EditableText)(object)this.widget).onAppPrivateCommand?.Invoke(action, data);
    }

    public virtual void insertContent(global::Doroti.Framework.Services.KeyboardInsertedContent content)
    {
        DartRuntimePrimitives.Assert(() => (((EditableText)(object)this.widget).contentInsertionConfiguration?.allowedMimeTypes.Contains(((global::Doroti.Framework.Services.KeyboardInsertedContent)content).mimeType) ?? false));
        ((EditableText)(object)this.widget).contentInsertionConfiguration?.onContentInserted?.Invoke(content);
    }

    internal virtual global::Doroti.Ui.Offset _floatingCursorOffset => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Offset>(new global::Doroti.Ui.Offset(0, (((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).preferredLineHeight / 2L)));
    public virtual void updateFloatingCursor(global::Doroti.Framework.Services.RawFloatingCursorPoint point)
    {
        _floatingCursorResetController ??= ((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{
    var __cascade = new global::Doroti.Framework.Animation.AnimationController(vsync: this);
    __cascade.addListener(() => this._onFloatingCursorResetTick());
    return __cascade;
}))();
        switch (((global::Doroti.Framework.Services.RawFloatingCursorPoint)point).state)
        {
            case global::Doroti.Framework.Services.FloatingCursorDragState.Start:
                {
                    if (this._floatingCursorResetController!.isAnimating)
                    {
                        this._floatingCursorResetController!.stop();
                        _onFloatingCursorResetTick();
                    }
                    _stopCursorBlink(resetCharTicks: false);
                    this._cursorBlinkOpacityController.value = 1.0;
                    _pointOffsetOrigin = ((global::Doroti.Framework.Services.RawFloatingCursorPoint)point).offset;
                    global::Doroti.Ui.Offset startCaretCenter = default!;
                    global::Doroti.Ui.TextPosition currentTextPosition = default!;
                    bool shouldResetOriginLocal = default!;
                    if ((((global::Doroti.Framework.Services.RawFloatingCursorPoint)point).startLocation is not null))
                    {
                        shouldResetOriginLocal = false;
                        DartRuntimePrimitives.Ignore((startCaretCenter, currentTextPosition) = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Services.RawFloatingCursorPoint)point).startLocation));
                    }
                    else
                    {
                        shouldResetOriginLocal = true;
                        currentTextPosition = new global::Doroti.Ui.TextPosition(offset: ((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).selection!.baseOffset, affinity: ((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).selection!.affinity);
                        startCaretCenter = ((Offset)((dynamic)this.renderEditable.getLocalRectForCaret(currentTextPosition)).center);
                    }
                    _startCaretCenter = startCaretCenter;
                    _lastBoundedOffset = this.renderEditable.calculateBoundedFloatingCursorOffset((DartRuntimePrimitives.RequireValue(this._startCaretCenter) - this._floatingCursorOffset), shouldResetOrigin: shouldResetOriginLocal);
                    _lastTextPosition = currentTextPosition;
                    this.renderEditable.setFloatingCursor(((global::Doroti.Framework.Services.RawFloatingCursorPoint)point).state, DartRuntimePrimitives.RequireValue(this._lastBoundedOffset), this._lastTextPosition!);
                    break;
                }
            case global::Doroti.Framework.Services.FloatingCursorDragState.Update:
                {
                    global::Doroti.Ui.Offset centeredPoint = ((global::Doroti.Ui.Offset)(object?)(DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Services.RawFloatingCursorPoint)point).offset) - DartRuntimePrimitives.RequireValue(this._pointOffsetOrigin)));
                    global::Doroti.Ui.Offset rawCursorOffset = ((global::Doroti.Ui.Offset)(object?)((DartRuntimePrimitives.RequireValue(this._startCaretCenter) + centeredPoint) - this._floatingCursorOffset));
                    _lastBoundedOffset = this.renderEditable.calculateBoundedFloatingCursorOffset(rawCursorOffset);
                    _lastTextPosition = ((TextPosition)((dynamic)this.renderEditable).getPositionForPoint(((Offset)((dynamic)this.renderEditable).localToGlobal((DartRuntimePrimitives.RequireValue(this._lastBoundedOffset) + this._floatingCursorOffset)))));
                    this.renderEditable.setFloatingCursor(((global::Doroti.Framework.Services.RawFloatingCursorPoint)point).state, DartRuntimePrimitives.RequireValue(this._lastBoundedOffset), this._lastTextPosition!);
                    break;
                }
            case global::Doroti.Framework.Services.FloatingCursorDragState.End:
                {
                    if (this._hasFocus)
                    {
                        _startCursorBlink();
                    }
                    if (((this._lastTextPosition is not null) && (this._lastBoundedOffset is not null)))
                    {
                        this._floatingCursorResetController!.value = 0.0;
                        this._floatingCursorResetController!.animateTo(1.0, duration: _floatingCursorResetTime, curve: global::Doroti.Framework.Animation.Curves.decelerate);
                    }
                    break;
                }
        }
    }

    internal virtual void _onFloatingCursorResetTick()
    {
        global::Doroti.Ui.Offset finalPosition = ((global::Doroti.Ui.Offset)(object?)(this.renderEditable.getLocalRectForCaret(this._lastTextPosition!).centerLeft - this._floatingCursorOffset));
        if (this._floatingCursorResetController!.isCompleted)
        {
            this.renderEditable.setFloatingCursor(global::Doroti.Framework.Services.FloatingCursorDragState.End, finalPosition, this._lastTextPosition!);
            if (((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).selection!.isCollapsed)
            {
                _handleSelectionChanged(global::Doroti.Framework.Services.TextSelection.CreateFromPosition(this._lastTextPosition!), global::Doroti.Framework.Services.SelectionChangedCause.forcePress);
            }
            _startCaretCenter = null;
            _lastTextPosition = null;
            _pointOffsetOrigin = null;
            _lastBoundedOffset = null;
        }
        else
        {
            double lerpValue = this._floatingCursorResetController!.value;
            double lerpX = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(DartRuntimePrimitives.RequireValue(this._lastBoundedOffset).dx, finalPosition.dx, lerpValue));
            double lerpY = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(DartRuntimePrimitives.RequireValue(this._lastBoundedOffset).dy, finalPosition.dy, lerpValue));
            this.renderEditable.setFloatingCursor(global::Doroti.Framework.Services.FloatingCursorDragState.Update, new global::Doroti.Ui.Offset(lerpX, lerpY), this._lastTextPosition!, resetLerpValue: lerpValue);
        }
    }

    internal virtual void _finalizeEditing(global::Doroti.Framework.Services.TextInputAction action, bool shouldUnfocus)
    {
        if ((((EditableText)(object)this.widget).onEditingComplete is not null))
        {
            try
            {
                ((EditableText)(object)this.widget).onEditingComplete!();
            }
            catch (Exception exceptionLocal)
            {
                var stackLocal = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exceptionLocal, stack: stackLocal, library: "widgets", context: new global::Doroti.Framework.Foundation.ErrorDescription($"while calling onEditingComplete for {action}")));
            }
        }
        else
        {
            ((EditableText)(object)this.widget).controller.clearComposing();
            if (shouldUnfocus)
            {
                switch (action)
                {
                    case global::Doroti.Framework.Services.TextInputAction.none:
                    case global::Doroti.Framework.Services.TextInputAction.unspecified:
                    case global::Doroti.Framework.Services.TextInputAction.done:
                    case global::Doroti.Framework.Services.TextInputAction.go:
                    case global::Doroti.Framework.Services.TextInputAction.search:
                    case global::Doroti.Framework.Services.TextInputAction.send:
                    case global::Doroti.Framework.Services.TextInputAction.continueAction:
                    case global::Doroti.Framework.Services.TextInputAction.join:
                    case global::Doroti.Framework.Services.TextInputAction.route:
                    case global::Doroti.Framework.Services.TextInputAction.emergencyCall:
                    case global::Doroti.Framework.Services.TextInputAction.newline:
                        {
                            ((EditableText)(object)this.widget).focusNode.unfocus();
                            break;
                        }
                    case global::Doroti.Framework.Services.TextInputAction.next:
                        {
                            ((EditableText)(object)this.widget).focusNode.nextFocus();
                            break;
                        }
                    case global::Doroti.Framework.Services.TextInputAction.previous:
                        {
                            ((EditableText)(object)this.widget).focusNode.previousFocus();
                            break;
                        }
                }
            }
        }
        global::System.Action<string>? onSubmittedLocal = ((EditableText)(object)this.widget).onSubmitted;
        if ((onSubmittedLocal is null))
        {
            return;
        }
        try
        {
            onSubmittedLocal(((global::Doroti.Framework.Services.TextEditingValue)this._value).text);
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
        DartRuntimePrimitives.Assert(() => (this._batchEditDepth >= 0L), () => (object?)"Unbalanced call to endBatchEdit: beginBatchEdit must be called first.");
        _updateRemoteEditingValueIfNeeded();
    }

    internal virtual void _updateRemoteEditingValueIfNeeded()
    {
        if (((this._batchEditDepth > 0L) || !this._hasInputConnection))
        {
            return;
        }
        global::Doroti.Framework.Services.TextEditingValue localValue = this._value;
        if ((object.Equals(localValue, this._lastKnownRemoteTextEditingValue)))
        {
            return;
        }
        this._textInputConnection!.setEditingState(localValue);
        _lastKnownRemoteTextEditingValue = localValue;
    }

    internal virtual global::Doroti.Framework.Services.TextEditingValue _value
    {
        get => ((EditableText)(object)this.widget).controller.value;
        set
        {
            var __value = value;
            ((EditableText)(object)this.widget).controller.value = __value;
        }
    }
    internal virtual bool _hasFocus => ((EditableText)(object)this.widget).focusNode.hasFocus;
    internal virtual bool _isMultiline => DartRuntimePrimitives.ConvertValue<bool>((((EditableText)(object)this.widget).maxLines != 1L));
    internal virtual global::Doroti.Framework.Rendering.RevealedOffset _getOffsetToRevealCaret(Rect rect)
    {
        if (!((ScrollController)this._scrollController).position.allowImplicitScrolling)
        {
            return new global::Doroti.Framework.Rendering.RevealedOffset(offset: ((ScrollController)this._scrollController).offset, rect: rect);
        }
        global::Doroti.Ui.Size editableSize = ((global::Doroti.Ui.Size)(object?)this.renderEditable.size);
        double additionalOffset = default!;
        global::Doroti.Ui.Offset unitOffset = default!;
        if (!this._isMultiline)
        {
            additionalOffset = ((rect.width >= editableSize.width) ? ((editableSize.width / 2L) - ((Offset)((dynamic)rect).center).dx) : Dart_uiLibrary.clampDouble(0.0, (rect.right - editableSize.width), rect.left));
            unitOffset = new global::Doroti.Ui.Offset(1, 0);
        }
        else
        {
            var expandedRect = global::Doroti.Ui.Rect.fromCenter(center: ((Offset)((dynamic)rect).center), width: rect.width, height: Math.Max(rect.height, ((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).preferredLineHeight));
            additionalOffset = ((expandedRect.height >= editableSize.height) ? ((editableSize.height / 2L) - ((Offset)((dynamic)expandedRect).center).dy) : Dart_uiLibrary.clampDouble(0.0, (expandedRect.bottom - editableSize.height), expandedRect.top));
            unitOffset = new global::Doroti.Ui.Offset(0, 1);
        }
        double targetOffset = Dart_uiLibrary.clampDouble((additionalOffset + ((ScrollController)this._scrollController).offset), ((ScrollController)this._scrollController).position.minScrollExtent, ((ScrollController)this._scrollController).position.maxScrollExtent);
        double offsetDelta = (((ScrollController)this._scrollController).offset - targetOffset);
        return new global::Doroti.Framework.Rendering.RevealedOffset(rect: rect.shift((unitOffset * offsetDelta)), offset: targetOffset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _needsAutofill => ((global::Doroti.Framework.Services.AutofillClient)this._effectiveAutofillClient).textInputConfiguration.autofillConfiguration.enabled;
    internal virtual void _openInputConnection()
    {
        if (!this._shouldCreateInputConnection)
        {
            return;
        }
        if (!this._hasInputConnection)
        {
            global::Doroti.Framework.Services.TextEditingValue localValue = this._value;
            _textInputConnection = ((this._needsAutofill && (this.currentAutofillScope is not null)) ? this.currentAutofillScope!.attach(this, ((global::Doroti.Framework.Services.AutofillClient)this._effectiveAutofillClient).textInputConfiguration) : TextInput.attach(this, ((global::Doroti.Framework.Services.AutofillClient)this._effectiveAutofillClient).textInputConfiguration));
            _updateSizeAndTransform();
            _schedulePeriodicPostFrameCallbacks();
            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Services.TextInputConnection>)(() =>
{
    var __cascade = this._textInputConnection!;
    __cascade.updateStyle(_getTextInputStyle(this.context));
    __cascade.setEditingState(localValue);
    __cascade.show();
    return __cascade;
}))());
            if (this._needsAutofill)
            {
                this._textInputConnection!.requestAutofill();
            }
            _lastKnownRemoteTextEditingValue = localValue;
        }
        else
        {
            this._textInputConnection!.show();
        }
    }

    internal virtual void _closeInputConnectionIfNeeded()
    {
        if (this._hasInputConnection)
        {
            this._textInputConnection!.close();
            _textInputConnection = null;
            _lastKnownRemoteTextEditingValue = null;
            _scribbleCacheKey = null;
            removeTextPlaceholder();
        }
    }

    internal virtual void _openOrCloseInputConnectionIfNeeded()
    {
        if ((this._hasFocus && ((EditableText)(object)this.widget).focusNode.consumeKeyboardToken()))
        {
            _openInputConnection();
        }
        else
        {
            if (!this._hasFocus)
            {
                _closeInputConnectionIfNeeded();
                ((EditableText)(object)this.widget).controller.clearComposing();
            }
        }
    }

    internal virtual void _scheduleRestartConnection()
    {
        if (this._restartConnectionScheduled)
        {
            return;
        }
        _restartConnectionScheduled = true;
        DartAsyncRuntime.scheduleMicrotask(this._restartConnectionIfNeeded);
    }

    internal virtual void _restartConnectionIfNeeded()
    {
        _restartConnectionScheduled = false;
        if ((!this._hasInputConnection || !this._shouldCreateInputConnection))
        {
            return;
        }
        this._textInputConnection!.close();
        _textInputConnection = null;
        _lastKnownRemoteTextEditingValue = null;
        global::Doroti.Framework.Services.AutofillScope? currentAutofillScopeLocal = (this._needsAutofill ? this.currentAutofillScope : null);
        global::Doroti.Framework.Services.TextInputConnection newConnection = ((currentAutofillScopeLocal?.attach(this, this.textInputConfiguration) ?? (global::Doroti.Framework.Services.TextInputConnection)TextInput.attach(this, ((global::Doroti.Framework.Services.AutofillClient)this._effectiveAutofillClient).textInputConfiguration)));
        _textInputConnection = newConnection;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Services.TextInputConnection>)(() =>
{
    var __cascade = newConnection;
    __cascade.show();
    __cascade.updateStyle(_getTextInputStyle(this.context));
    __cascade.setEditingState(this._value);
    return __cascade;
}))());
        _lastKnownRemoteTextEditingValue = this._value;
    }

    public virtual void didChangeInputControl(global::Doroti.Framework.Services.TextInputControl? oldControl, global::Doroti.Framework.Services.TextInputControl? newControl)
    {
        if ((this._hasFocus && this._hasInputConnection))
        {
            oldControl?.hide();
            newControl?.show();
        }
    }

    public virtual bool onFocusReceived()
    {
        if (((this.mounted && !this._hasFocus) && ((EditableText)(object)this.widget).focusNode.canRequestFocus))
        {
            ((EditableText)(object)this.widget).focusNode.requestFocus();
            return true;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void connectionClosed()
    {
        if (this._hasInputConnection)
        {
            this._textInputConnection!.connectionClosedReceived();
            _textInputConnection = null;
            _lastKnownRemoteTextEditingValue = null;
            ((EditableText)(object)this.widget).focusNode.unfocus();
        }
    }

    internal virtual void _flagInternalFocus()
    {
        _nextFocusChangeIsInternal = true;
        FocusManager.instance.addListener(() => this._unflagInternalFocus());
    }

    internal virtual void _unflagInternalFocus()
    {
        _nextFocusChangeIsInternal = false;
        FocusManager.instance.removeListener(() => this._unflagInternalFocus());
    }

    public virtual void requestKeyboard()
    {
        if (this._hasFocus)
        {
            _openInputConnection();
        }
        else
        {
            _flagInternalFocus();
            ((EditableText)(object)this.widget).focusNode.requestFocus();
        }
    }

    internal virtual void _updateOrDisposeSelectionOverlayIfNeeded()
    {
        if ((this._selectionOverlay is not null))
        {
            if (this._hasFocus)
            {
                this._selectionOverlay!.update(this._value);
            }
            else
            {
                this._selectionOverlay!.dispose();
                _selectionOverlay = null;
            }
        }
    }

    internal virtual bool _isInternalScrollableNotification(BuildContext? notificationContext)
    {
        ScrollableState? scrollableState = ((ScrollableState?)(object?)notificationContext?.findAncestorStateOfType<ScrollableState>());
        return (object.Equals(((GlobalKey<IState>)this._scrollableKey).currentContext, scrollableState?.context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _scrollableNotificationIsFromSameSubtree(BuildContext? notificationContext)
    {
        if ((notificationContext is null))
        {
            return false;
        }
        BuildContext? currentContext = this.context;
        ScrollableState? notificationScrollableState = ((ScrollableState?)(object?)notificationContext.findAncestorStateOfType<ScrollableState>());
        if ((notificationScrollableState is null))
        {
            return false;
        }
        while ((currentContext is not null))
        {
            ScrollableState? scrollableState = ((ScrollableState?)(object?)currentContext.findAncestorStateOfType<ScrollableState>());
            if ((object.Equals(scrollableState, notificationScrollableState)))
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
        if (((notification is not ScrollStartNotification) && (notification is not ScrollEndNotification)))
        {
            return;
        }
        switch (notification)
        {
            case ScrollStartNotification __object177981 when ((this._dataWhenToolbarShowScheduled is not null)):
            case ScrollEndNotification __object178062 when ((this._dataWhenToolbarShowScheduled is null)):
                {
                    break;
                }
            case ScrollEndNotification __object178156 when ((!object.Equals(DartRuntimePrimitives.RequireValue(this._dataWhenToolbarShowScheduled).value, this._value))):
                {
                    _dataWhenToolbarShowScheduled = null;
                    _disposeScrollNotificationObserver();
                    break;
                }
            case ScrollNotification { context: BuildContext contextLocal } __object178336 when ((!_isInternalScrollableNotification(contextLocal) && _scrollableNotificationIsFromSameSubtree(contextLocal))):
                {
                    _handleContextMenuOnScroll(notification);
                    break;
                }
        }
    }

    internal virtual global::Doroti.Ui.Rect _calculateDeviceRect()
    {
        global::Doroti.Ui.Size screenSize = ((global::Doroti.Ui.Size)(object?)MediaQuery.sizeOf(this.context));
        global::Doroti.Ui.DorotiView view = ((global::Doroti.Ui.DorotiView)(object?)View.of(this.context));
        double obscuredVertical = ((((view.padding.top + view.padding.bottom) + view.viewInsets.bottom)) / view.devicePixelRatio);
        double obscuredHorizontal = (((view.padding.left + view.padding.right)) / view.devicePixelRatio);
        var visibleScreenSize = new global::Doroti.Ui.Size((screenSize.width - obscuredHorizontal), (screenSize.height - obscuredVertical));
        return global::Doroti.Ui.Rect.fromLTWH((view.padding.left / view.devicePixelRatio), (view.padding.top / view.devicePixelRatio), visibleScreenSize.width, visibleScreenSize.height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleContextMenuOnScroll(ScrollNotification notification)
    {
        if (this._webContextMenuEnabled)
        {
            return;
        }
        if (!this._platformSupportsFadeOnScroll)
        {
            this._selectionOverlay?.updateForScroll();
            return;
        }
        if ((notification is ScrollStartNotification))
        {
            ScrollStartNotification notification__as179959 = (ScrollStartNotification)notification;
            if ((this._dataWhenToolbarShowScheduled is not null))
            {
                return;
            }
            bool toolbarIsVisibleLocal = (((this._selectionOverlay is not null) && this._selectionOverlay!.toolbarIsVisible) && !this._selectionOverlay!.spellCheckToolbarIsVisible);
            if (!toolbarIsVisibleLocal)
            {
                return;
            }
            List<global::Doroti.Ui.TextBox> selectionBoxes = ((List<global::Doroti.Ui.TextBox>)(object?)this.renderEditable.getBoxesForSelection(((global::Doroti.Framework.Services.TextEditingValue)this._value).selection));
            global::Doroti.Ui.Rect selectionBoundsLocal = ((global::Doroti.Ui.Rect)(object?)((((global::Doroti.Framework.Services.TextEditingValue)this._value).selection.isCollapsed || !System.Linq.Enumerable.Any(selectionBoxes)) ? this.renderEditable.getLocalRectForCaret(((global::Doroti.Framework.Services.TextEditingValue)this._value).selection.extent) : selectionBoxes.map<TextBox, Rect>(((box) => box.toRect())).reduce(((result, rect) => result.expandToInclude(rect)))));
            _dataWhenToolbarShowScheduled = (selectionBounds: selectionBoundsLocal, value: this._value);
            this._selectionOverlay?.hideToolbar();
        }
        else
        {
            if ((notification is ScrollEndNotification))
            {
                ScrollEndNotification notification__as180881 = (ScrollEndNotification)notification;
                if ((this._dataWhenToolbarShowScheduled is null))
                {
                    return;
                }
                if ((!object.Equals(DartRuntimePrimitives.RequireValue(this._dataWhenToolbarShowScheduled).value, this._value)))
                {
                    _dataWhenToolbarShowScheduled = null;
                    _disposeScrollNotificationObserver();
                    return;
                }
                if (this._showToolbarOnScreenScheduled)
                {
                    return;
                }
                _showToolbarOnScreenScheduled = true;
                void scheduleToolbar(Duration _)
                {
                    _showToolbarOnScreenScheduled = false;
                    if ((!this.mounted || (this._dataWhenToolbarShowScheduled is null)))
                    {
                        return;
                    }
                    if ((!object.Equals(DartRuntimePrimitives.RequireValue(this._dataWhenToolbarShowScheduled).value, this._value)))
                    {
                        _dataWhenToolbarShowScheduled = null;
                        _disposeScrollNotificationObserver();
                        return;
                    }
                    global::Doroti.Ui.Rect deviceRect = ((global::Doroti.Ui.Rect)(object?)_calculateDeviceRect());
                    bool selectionVisibleInEditable = (((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).selectionStartInViewport.value || ((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).selectionEndInViewport.value);
                    global::Doroti.Ui.Rect selectionBoundsAlternate = ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(this.renderEditable.getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null)), DartRuntimePrimitives.RequireValue(this._dataWhenToolbarShowScheduled).selectionBounds));
                    bool selectionOverlapsWithDeviceRect = (!selectionBoundsAlternate.hasNaN && deviceRect.overlaps(selectionBoundsAlternate));
                    if (((selectionVisibleInEditable && selectionOverlapsWithDeviceRect) && _selectionInViewport(DartRuntimePrimitives.RequireValue(this._dataWhenToolbarShowScheduled).selectionBounds)))
                    {
                        showToolbar();
                        _dataWhenToolbarShowScheduled = null;
                    }
                }
                switch (global::Doroti.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase)
                {
                    case global::Doroti.Framework.Scheduler.SchedulerPhase.idle:
                    case global::Doroti.Framework.Scheduler.SchedulerPhase.postFrameCallbacks:
                        {
                            global::Doroti.Framework.Scheduler.SchedulerBinding.instance.scheduleFrameCallback((global::System.Action<Duration>)scheduleToolbar);
                            break;
                        }
                    case global::Doroti.Framework.Scheduler.SchedulerPhase.transientCallbacks:
                    case global::Doroti.Framework.Scheduler.SchedulerPhase.midFrameMicrotasks:
                    case global::Doroti.Framework.Scheduler.SchedulerPhase.persistentCallbacks:
                        {
                            global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback((__arg0) => ((global::System.Action<Duration>)scheduleToolbar)(__arg0), debugLabel: "EditableText.scheduleToolbar");
                            break;
                        }
                }
            }
        }
    }

    internal virtual bool _selectionInViewport(Rect selectionBounds)
    {
        global::Doroti.Framework.Rendering.RenderAbstractViewport? closestViewport = ((global::Doroti.Framework.Rendering.RenderAbstractViewport?)(object?)RenderAbstractViewport.maybeOf(this.renderEditable));
        while ((closestViewport is not null))
        {
            global::Doroti.Ui.Rect selectionBoundsLocalToViewport = ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(this.renderEditable.getTransformTo(closestViewport), selectionBounds));
            if (((selectionBoundsLocalToViewport.hasNaN || closestViewport.paintBounds.hasNaN) || !closestViewport.paintBounds.overlaps(selectionBoundsLocalToViewport)))
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
        return ((EditableText)(object)this.widget).contextMenuBuilder!(context, this);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual TextSelectionOverlay _createSelectionOverlay()
    {
        var selectionOverlay = new TextSelectionOverlay(clipboardStatus: this.clipboardStatus, context: this.context, value: this._value, debugRequiredFor: this.widget, toolbarLayerLink: this._toolbarLayerLink, startHandleLayerLink: this._startHandleLayerLink, endHandleLayerLink: this._endHandleLayerLink, renderObject: this.renderEditable, selectionControls: ((EditableText)(object)this.widget).selectionControls, selectionDelegate: this, dragStartBehavior: ((EditableText)(object)this.widget).dragStartBehavior, onSelectionHandleTapped: () => ((EditableText)(object)this.widget).onSelectionHandleTapped(), contextMenuBuilder: ((global::System.Func<BuildContext, Widget>)(((((EditableText)(object)this.widget).contextMenuBuilder is null) || this._webContextMenuEnabled) ? null : this._contextMenuBuilder)), magnifierConfiguration: ((EditableText)(object)this.widget).magnifierConfiguration);
        return selectionOverlay;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleSelectionChanged(global::Doroti.Framework.Services.TextSelection selection, global::Doroti.Framework.Services.SelectionChangedCause? cause)
    {
        string textLocal = ((EditableText)(object)this.widget).controller.value.text;
        if (((textLocal.Length < selection.end) || (textLocal.Length < selection.start)))
        {
            return;
        }
        ((EditableText)(object)this.widget).controller.selection = selection;
        switch (cause)
        {
            case null:
            case global::Doroti.Framework.Services.SelectionChangedCause.doubleTap:
            case global::Doroti.Framework.Services.SelectionChangedCause.drag:
            case global::Doroti.Framework.Services.SelectionChangedCause.forcePress:
            case global::Doroti.Framework.Services.SelectionChangedCause.longPress:
            case global::Doroti.Framework.Services.SelectionChangedCause.stylusHandwriting:
            case global::Doroti.Framework.Services.SelectionChangedCause.tap:
            case global::Doroti.Framework.Services.SelectionChangedCause.toolbar:
                {
                    requestKeyboard();
                    break;
                }
            case global::Doroti.Framework.Services.SelectionChangedCause.keyboard:
                break;
        }
        if (((((EditableText)(object)this.widget).selectionControls is null) && (((EditableText)(object)this.widget).contextMenuBuilder is null)))
        {
            this._selectionOverlay?.dispose();
            _selectionOverlay = null;
        }
        else
        {
            if ((this._selectionOverlay is null))
            {
                _selectionOverlay = _createSelectionOverlay();
            }
            else
            {
                this._selectionOverlay!.update(this._value);
            }
            this._selectionOverlay!.handlesVisible = ((EditableText)(object)this.widget).showSelectionHandles;
            this._selectionOverlay!.showHandles();
        }
        try
        {
            ((EditableText)(object)this.widget).onSelectionChanged?.Invoke(selection, cause);
        }
        catch (Exception exceptionLocal)
        {
            var stackLocal = new System.Diagnostics.StackTrace();
            FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exceptionLocal, stack: stackLocal, library: "widgets", context: new global::Doroti.Framework.Foundation.ErrorDescription($"while calling onSelectionChanged for {cause}")));
        }
        if ((this._showBlinkingCursor && (this._cursorTimer is not null)))
        {
            _stopCursorBlink(resetCharTicks: false);
            _startCursorBlink();
        }
    }

    internal virtual void _scheduleShowCaretOnScreen(bool withAnimation)
    {
        if (this._showCaretOnScreenScheduled)
        {
            return;
        }
        _showCaretOnScreenScheduled = true;
        global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) =>
        {
            _showCaretOnScreenScheduled = false;
            var renderEditable = ((global::Doroti.Framework.Rendering.RenderEditable?)(object?)((GlobalKey<IState>)this._editableKey).currentContext?.findRenderObject())!;
            if ((((renderEditable is null) || !((((global::Doroti.Framework.Rendering.RenderEditable)renderEditable).selection?.isValid ?? false))) || !((ScrollController)this._scrollController).hasClients))
            {
                return;
            }
            double lineHeight = ((global::Doroti.Framework.Rendering.RenderEditable)renderEditable).preferredLineHeight;
            double bottomSpacing = ((EditableText)(object)this.widget).scrollPadding.bottom;
            if ((this._selectionOverlay?.selectionControls is not null))
            {
                double handleHeight = this._selectionOverlay!.selectionControls!.getHandleSize(lineHeight).height;
                double interactiveHandleHeight = Math.Max(handleHeight, global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension);
                global::Doroti.Ui.Offset anchor = ((global::Doroti.Ui.Offset)(object?)this._selectionOverlay!.selectionControls!.getHandleAnchor(global::Doroti.Framework.Rendering.TextSelectionHandleType.collapsed, lineHeight));
                double handleCenter = ((handleHeight / 2L) - anchor.dy);
                bottomSpacing = Math.Max((handleCenter + (interactiveHandleHeight / 2L)), bottomSpacing);
            }
            global::Doroti.Framework.Painting.EdgeInsets caretPadding = ((global::Doroti.Framework.Painting.EdgeInsets)(object?)((EditableText)(object)this.widget).scrollPadding.copyWith(bottom: bottomSpacing));
            global::Doroti.Ui.Rect caretRect = ((global::Doroti.Ui.Rect)(object?)renderEditable.getLocalRectForCaret(((global::Doroti.Framework.Rendering.RenderEditable)renderEditable).selection!.extent));
            global::Doroti.Framework.Rendering.RevealedOffset targetOffset = ((global::Doroti.Framework.Rendering.RevealedOffset)(object?)_getOffsetToRevealCaret(caretRect));
            global::Doroti.Ui.Rect rectToReveal = default!;
            global::Doroti.Framework.Services.TextSelection selectionLocal = ((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).selection;
            if (selectionLocal.isCollapsed)
            {
                rectToReveal = ((global::Doroti.Framework.Rendering.RevealedOffset)targetOffset).rect;
            }
            else
            {
                List<global::Doroti.Ui.TextBox> selectionBoxes = ((List<global::Doroti.Ui.TextBox>)(object?)renderEditable.getBoxesForSelection(selectionLocal));
                if (!System.Linq.Enumerable.Any(selectionBoxes))
                {
                    rectToReveal = ((global::Doroti.Framework.Rendering.RevealedOffset)targetOffset).rect;
                }
                else
                {
                    rectToReveal = ((((global::Doroti.Framework.Services.TextSelection)selectionLocal).baseOffset < ((global::Doroti.Framework.Services.TextSelection)selectionLocal).extentOffset) ? selectionBoxes.Last().toRect() : selectionBoxes.First().toRect());
                }
            }
            if (withAnimation)
            {
                DartRuntimePrimitives.Ignore(this._scrollController.animateTo(((global::Doroti.Framework.Rendering.RevealedOffset)targetOffset).offset, duration: _caretAnimationDuration, curve: _caretAnimationCurve));
                renderEditable.showOnScreen(rect: caretPadding.inflateRect(rectToReveal), duration: _caretAnimationDuration, curve: _caretAnimationCurve);
            }
            else
            {
                this._scrollController.jumpTo(((global::Doroti.Framework.Rendering.RevealedOffset)targetOffset).offset);
                renderEditable.showOnScreen(rect: caretPadding.inflateRect(rectToReveal));
            }
        })), debugLabel: "EditableText.showCaret");
    }

    public virtual void didChangeMetrics()
    {
        if (!this.mounted)
        {
            return;
        }
        global::Doroti.Ui.DorotiView view = ((global::Doroti.Ui.DorotiView)(object?)View.of(this.context));
        if ((this._lastBottomViewInset != view.viewInsets.bottom))
        {
            global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) =>
            {
                this._selectionOverlay?.updateForScroll();
            })), debugLabel: "EditableText.updateForScroll");
            if ((this._lastBottomViewInset < view.viewInsets.bottom))
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
            global::Doroti.Ui.Locale? localeForSpellChecking = ((((EditableText)(object)this.widget).locale ?? (Locale)Localizations.maybeLocaleOf(this.context)));
            DartRuntimePrimitives.Assert(() => (localeForSpellChecking is not null), () => (object?)"Locale must be specified in widget or Localization widget must be in scope");
            List<global::Doroti.Framework.Services.SuggestionSpan>? suggestions = (await ((SpellCheckConfiguration)this._spellCheckConfiguration).spellCheckService!.fetchSpellCheckSuggestions(DartRuntimePrimitives.RequireValue(localeForSpellChecking), text)).ToList();
            if ((((suggestions is null) || !this.mounted) || !this.spellCheckEnabled))
            {
                return;
            }
            spellCheckResults = new global::Doroti.Framework.Services.SpellCheckResults(text, suggestions);
            double? lineHeightScaleFactorLocal = MediaQuery.maybeLineHeightScaleFactorOverrideOf(this.context);
            double? letterSpacingLocal = MediaQuery.maybeLetterSpacingOverrideOf(this.context);
            double? wordSpacingLocal = MediaQuery.maybeWordSpacingOverrideOf(this.context);
            this.renderEditable.text = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.InlineSpan>(_OverridingTextStyleTextSpanUtils__editable_text.applyTextSpacingOverrides(lineHeightScaleFactor: lineHeightScaleFactorLocal, letterSpacing: letterSpacingLocal, wordSpacing: wordSpacingLocal, textSpan: buildTextSpan()));
        }
        catch (Exception exceptionLocal)
        {
            var stackLocal = new System.Diagnostics.StackTrace();
            FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exceptionLocal, stack: stackLocal, library: "widgets", context: new global::Doroti.Framework.Foundation.ErrorDescription("while performing spell check")));
        }
    }

    internal virtual void _formatAndSetValue(global::Doroti.Framework.Services.TextEditingValue value, global::Doroti.Framework.Services.SelectionChangedCause? cause, bool userInteraction = false)
    {
        global::Doroti.Framework.Services.TextEditingValue oldValue = this._value;
        var textChanged = (((global::Doroti.Framework.Services.TextEditingValue)oldValue).text != ((global::Doroti.Framework.Services.TextEditingValue)value).text);
        bool textCommitted = (!((global::Doroti.Framework.Services.TextEditingValue)oldValue).composing.isCollapsed && ((global::Doroti.Framework.Services.TextEditingValue)value).composing.isCollapsed);
        var selectionChanged = (!object.Equals(((global::Doroti.Framework.Services.TextEditingValue)oldValue).selection, ((global::Doroti.Framework.Services.TextEditingValue)value).selection));
        if ((textChanged || textCommitted))
        {
            try
            {
                value = (System.Linq.Enumerable.Aggregate(((EditableText)(object)this.widget).inputFormatters, (global::Doroti.Framework.Services.TextEditingValue?)value, ((newValue, formatter) => formatter.formatEditUpdate(this._value, newValue))) ?? value);
                if (((this.spellCheckEnabled && (((global::Doroti.Framework.Services.TextEditingValue)value).text.Length != 0)) && (((global::Doroti.Framework.Services.TextEditingValue)this._value).text != ((global::Doroti.Framework.Services.TextEditingValue)value).text)))
                {
                    DartRuntimePrimitives.Ignore(_performSpellCheck(((global::Doroti.Framework.Services.TextEditingValue)value).text));
                }
            }
            catch (Exception exceptionLocal)
            {
                var stackLocal = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exceptionLocal, stack: stackLocal, library: "widgets", context: new global::Doroti.Framework.Foundation.ErrorDescription("while applying input formatters")));
            }
        }
        global::Doroti.Framework.Services.TextSelection oldTextSelection = ((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).selection;
        beginBatchEdit();
        _value = value;
        if ((selectionChanged || ((userInteraction && (((object.Equals(cause, global::Doroti.Framework.Services.SelectionChangedCause.longPress)) || (object.Equals(cause, global::Doroti.Framework.Services.SelectionChangedCause.keyboard))))))))
        {
            _handleSelectionChanged(((global::Doroti.Framework.Services.TextEditingValue)this._value).selection, cause);
            _bringIntoViewBySelectionState(oldTextSelection, ((global::Doroti.Framework.Services.TextEditingValue)value).selection, cause);
        }
        string currentText = ((global::Doroti.Framework.Services.TextEditingValue)this._value).text;
        if ((((global::Doroti.Framework.Services.TextEditingValue)oldValue).text != currentText))
        {
            try
            {
                ((EditableText)(object)this.widget).onChanged?.Invoke(currentText);
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
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                {
                    if (((object.Equals(cause, global::Doroti.Framework.Services.SelectionChangedCause.longPress)) || (object.Equals(cause, global::Doroti.Framework.Services.SelectionChangedCause.drag))))
                    {
                        bringIntoView(((global::Doroti.Framework.Services.TextSelection)newSelection).extent);
                    }
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
                {
                    if ((object.Equals(cause, global::Doroti.Framework.Services.SelectionChangedCause.drag)))
                    {
                        if ((((global::Doroti.Framework.Services.TextSelection)oldSelection).baseOffset != ((global::Doroti.Framework.Services.TextSelection)newSelection).baseOffset))
                        {
                            bringIntoView(((global::Doroti.Framework.Services.TextSelection)newSelection).@base);
                        }
                        else
                        {
                            if ((((global::Doroti.Framework.Services.TextSelection)oldSelection).extentOffset != ((global::Doroti.Framework.Services.TextSelection)newSelection).extentOffset))
                            {
                                bringIntoView(((global::Doroti.Framework.Services.TextSelection)newSelection).extent);
                            }
                        }
                    }
                    break;
                }
        }
    }

    internal virtual void _onCursorColorTick()
    {
        double effectiveOpacity = Math.Min((((EditableText)(object)this.widget).cursorColor.alpha / 255.0), ((global::Doroti.Framework.Animation.AnimationController)this._cursorBlinkOpacityController).value);
        this.renderEditable.cursorColor = ((EditableText)(object)this.widget).cursorColor.withOpacity(effectiveOpacity);
        this._cursorVisibilityNotifier.value = (((EditableText)(object)this.widget).showCursor && ((EditableText.debugDeterministicCursor || (((global::Doroti.Framework.Animation.AnimationController)this._cursorBlinkOpacityController).value > 0L))));
    }

    internal virtual bool _showBlinkingCursor => DartRuntimePrimitives.ConvertValue<bool>(((((this._hasFocus && ((global::Doroti.Framework.Services.TextEditingValue)this._value).selection.isCollapsed) && ((EditableText)(object)this.widget).showCursor) && this._tickersEnabled) && !((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).floatingCursorOn));
    public virtual bool cursorCurrentlyVisible => DartRuntimePrimitives.ConvertValue<bool>((((global::Doroti.Framework.Animation.AnimationController)this._cursorBlinkOpacityController).value > 0L));
    public virtual Duration cursorBlinkInterval => Editable_textLibrary._kCursorBlinkHalfPeriod;
    public virtual TextSelectionOverlay? selectionOverlay => this._selectionOverlay;
    internal virtual void _startCursorBlink()
    {
        DartRuntimePrimitives.Assert(() => (!((this._cursorTimer?.isActive ?? false)) || !((this._backingCursorBlinkOpacityController?.isAnimating ?? false))));
        if (!((EditableText)(object)this.widget).showCursor)
        {
            return;
        }
        if (!this._tickersEnabled)
        {
            return;
        }
        this._cursorTimer?.cancel();
        this._cursorBlinkOpacityController.value = 1.0;
        if (EditableText.debugDeterministicCursor)
        {
            return;
        }
        if (((EditableText)(object)this.widget).cursorOpacityAnimates)
        {
            DartRuntimePrimitives.Ignore(this._cursorBlinkOpacityController.animateWith(this._iosBlinkCursorSimulation).whenComplete(() => { ((Action)this._onCursorTick)(); return default!; }));
        }
        else
        {
            _cursorTimer = new Timer(Editable_textLibrary._kCursorBlinkHalfPeriod, ((timer) =>
            {
                _onCursorTick();
            }));
        }
    }

    internal virtual void _onCursorTick()
    {
        if ((this._obscureShowCharTicksPending > 0L))
        {
            _obscureShowCharTicksPending = (WidgetsBinding.instance.platformDispatcher.brieflyShowPassword ? (this._obscureShowCharTicksPending - 1L) : 0L);
            if ((this._obscureShowCharTicksPending == 0L))
            {
                setState(((global::System.Action)(() =>
                {
                })));
            }
        }
        if (((EditableText)(object)this.widget).cursorOpacityAnimates)
        {
            this._cursorTimer?.cancel();
            _cursorTimer = new Timer(Duration.zero, (() => { _ = this._cursorBlinkOpacityController.animateWith(this._iosBlinkCursorSimulation).whenComplete(() => { ((Action)this._onCursorTick)(); return default!; }); }));
        }
        else
        {
            if ((!((this._cursorTimer?.isActive ?? false)) && this._tickersEnabled))
            {
                _cursorTimer = new Timer(Editable_textLibrary._kCursorBlinkHalfPeriod, ((timer) =>
                {
                    _onCursorTick();
                }));
            }
            this._cursorBlinkOpacityController.value = ((((global::Doroti.Framework.Animation.AnimationController)this._cursorBlinkOpacityController).value == 0L) ? 1 : 0);
        }
    }

    internal virtual void _stopCursorBlink(bool resetCharTicks = true)
    {
        this._cursorBlinkOpacityController.value = (((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).floatingCursorOn ? 1.0 : 0.0);
        this._cursorTimer?.cancel();
        _cursorTimer = null;
        if (resetCharTicks)
        {
            _obscureShowCharTicksPending = 0L;
        }
    }

    internal virtual void _startOrStopCursorTimerIfNeeded()
    {
        if (!this._showBlinkingCursor)
        {
            _stopCursorBlink();
        }
        else
        {
            if ((this._cursorTimer is null))
            {
                _startCursorBlink();
            }
        }
    }

    internal virtual void _didChangeTextEditingValue()
    {
        if ((this._hasFocus && !((global::Doroti.Framework.Services.TextEditingValue)this._value).selection.isValid))
        {
            ((EditableText)(object)this.widget).controller.removeListener(this._didChangeTextEditingValue);
            ((EditableText)(object)this.widget).controller.selection = _adjustedSelectionWhenFocused()!;
            ((EditableText)(object)this.widget).controller.addListener(this._didChangeTextEditingValue);
        }
        _updateRemoteEditingValueIfNeeded();
        _startOrStopCursorTimerIfNeeded();
        _updateOrDisposeSelectionOverlayIfNeeded();
        setState(((global::System.Action)(() =>
        {
        })));
        this._verticalSelectionUpdateAction.stopCurrentVerticalRunIfSelectionChanges();
    }

    internal virtual void _handleFocusChanged()
    {
        _openOrCloseInputConnectionIfNeeded();
        _startOrStopCursorTimerIfNeeded();
        _updateOrDisposeSelectionOverlayIfNeeded();
        if (this._hasFocus)
        {
            WidgetsBinding.instance.addObserver(this);
            _lastBottomViewInset = View.of(this.context).viewInsets.bottom;
            if (!((EditableText)(object)this.widget).readOnly)
            {
                _scheduleShowCaretOnScreen(withAnimation: true);
            }
            global::Doroti.Framework.Services.TextSelection? updatedSelection = ((global::Doroti.Framework.Services.TextSelection?)(object?)_adjustedSelectionWhenFocused());
            if ((updatedSelection is not null))
            {
                _handleSelectionChanged(updatedSelection, ((global::Doroti.Framework.Services.SelectionChangedCause)(object)null));
            }
        }
        else
        {
            WidgetsBinding.instance.removeObserver(this);
            setState(((global::System.Action)(() =>
            {
                _currentPromptRectRange = null;
            })));
        }
        updateKeepAlive();
    }

    internal virtual global::Doroti.Framework.Services.TextSelection? _adjustedSelectionWhenFocused()
    {
        global::Doroti.Framework.Services.TextSelection? selectionLocal = default!;
        bool shouldSelectAll = ((((((EditableText)(object)this.widget).selectAllOnFocus && ((EditableText)(object)this.widget).selectionEnabled) && !this._isMultiline) && !this._nextFocusChangeIsInternal) && !this._justResumed);
        _justResumed = false;
        if (shouldSelectAll)
        {
            selectionLocal = new global::Doroti.Framework.Services.TextSelection(baseOffset: 0L, extentOffset: ((global::Doroti.Framework.Services.TextEditingValue)this._value).text.Length);
        }
        else
        {
            if (!((global::Doroti.Framework.Services.TextEditingValue)this._value).selection.isValid)
            {
                selectionLocal = global::Doroti.Framework.Services.TextSelection.CreateCollapsed(offset: ((global::Doroti.Framework.Services.TextEditingValue)this._value).text.Length);
            }
        }
        return selectionLocal;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _compositeCallback(global::Doroti.Framework.Rendering.Layer layer)
    {
        if ((!this.renderEditable.attached || !this._hasInputConnection))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => this.mounted);
        DartRuntimePrimitives.Assert(() => (((Element?)(object?)this.context)!).debugIsActive);
        _updateSizeAndTransform();
    }

    internal virtual void _updateSizeAndTransform()
    {
        global::Doroti.Ui.Size sizeLocal = ((global::Doroti.Ui.Size)(object?)this.renderEditable.size);
        Matrix4 transform = ((Matrix4)(object?)this.renderEditable.getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null)));
        this._textInputConnection!.setEditableSizeAndTransform(sizeLocal, transform);
    }

    internal virtual void _schedulePeriodicPostFrameCallbacks(Duration? duration = null)
    {
        if (!this._hasInputConnection)
        {
            return;
        }
        _updateSelectionRects();
        _updateComposingRectIfNeeded();
        _updateCaretRectIfNeeded();
        global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback((__arg0) => ((global::System.Action<Duration?>)this._schedulePeriodicPostFrameCallbacks)(DartRuntimePrimitives.ConvertValue<Duration>(__arg0)), debugLabel: "EditableText.postFrameCallbacks");
    }

    internal virtual void _updateSelectionRects(bool force = false)
    {
        if ((!this._stylusHandwritingEnabled || (!object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.iOS))))
        {
            return;
        }
        global::Doroti.Framework.Rendering.ScrollDirection scrollDirection = ((ScrollController)this._scrollController).position.userScrollDirection;
        if ((!object.Equals(scrollDirection, global::Doroti.Framework.Rendering.ScrollDirection.idle)))
        {
            return;
        }
        global::Doroti.Framework.Painting.InlineSpan inlineSpanLocal = ((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).text!;
        double? lineHeightScaleFactor = MediaQuery.maybeLineHeightScaleFactorOverrideOf(this.context);
        global::Doroti.Framework.Painting.TextScaler effectiveTextScaler = ((((EditableText)(object)this.widget).textScaler, ((EditableText)(object)this.widget).textScaleFactor) switch { (global::Doroti.Framework.Painting.TextScaler textScalerLocal, _) => textScalerLocal, (null, double textScaleFactorLocal) => global::Doroti.Framework.Painting.TextScaler.CreateLinear(textScaleFactorLocal), (null, null) => MediaQuery.textScalerOf(this.context) });
        var newCacheKey = new _ScribbleCacheKey__editable_text(inlineSpan: inlineSpanLocal, textAlign: ((EditableText)(object)this.widget).textAlign, textDirection: this._textDirection, textScaler: effectiveTextScaler, textHeightBehavior: ((((EditableText)(object)this.widget).textHeightBehavior ?? (TextHeightBehavior)DefaultTextHeightBehavior.maybeOf(this.context))), locale: ((EditableText)(object)this.widget).locale, structStyle: ((EditableText)(object)this.widget).strutStyle.merge(new global::Doroti.Framework.Painting.StrutStyle(height: lineHeightScaleFactor)), placeholder: this._placeholderLocation, size: this.renderEditable.size);
        global::Doroti.Framework.Painting.RenderComparison comparison = (force ? global::Doroti.Framework.Painting.RenderComparison.layout : (this._scribbleCacheKey?.compare(newCacheKey) ?? global::Doroti.Framework.Painting.RenderComparison.layout));
        if ((FoundationRuntimePorts.EnumIndex(comparison) < FoundationRuntimePorts.EnumIndex(global::Doroti.Framework.Painting.RenderComparison.layout)))
        {
            return;
        }
        _scribbleCacheKey = newCacheKey;
        var rects = new List<global::Doroti.Framework.Services.SelectionRect>();
        var graphemeStart = 0L;
        string plainText = ((string)(object?)inlineSpanLocal.toPlainText(includeSemanticsLabels: false));
        var characterRange = new CharacterRange(plainText);
        while (characterRange.MoveNext())
        {
            long graphemeEnd = (graphemeStart + characterRange.Current.Length);
            List<global::Doroti.Ui.TextBox> boxes = ((List<global::Doroti.Ui.TextBox>)(object?)this.renderEditable.getBoxesForSelection(new global::Doroti.Framework.Services.TextSelection(baseOffset: graphemeStart, extentOffset: graphemeEnd)));
            global::Doroti.Ui.TextBox? box = ((global::Doroti.Ui.TextBox?)(object?)(!System.Linq.Enumerable.Any(boxes) ? null : boxes.First()));
            if ((box is not null))
            {
                global::Doroti.Ui.Rect paintBoundsLocal = ((global::Doroti.Ui.Rect)(object?)this.renderEditable.paintBounds);
                if ((paintBoundsLocal.bottom <= box.top))
                {
                    break;
                }
                if ((((paintBoundsLocal.left <= box.right) && (box.left <= paintBoundsLocal.right)) && (paintBoundsLocal.top <= box.bottom)))
                {
                    rects.Add(new global::Doroti.Framework.Services.SelectionRect(position: graphemeStart, bounds: box.toRect(), direction: box.direction));
                }
            }
            graphemeStart = graphemeEnd;
        }
        this._textInputConnection!.setSelectionRects(rects);
    }

    internal virtual void _updateComposingRectIfNeeded()
    {
        global::Doroti.Ui.TextRange composingRange = ((global::Doroti.Ui.TextRange)(object?)((global::Doroti.Framework.Services.TextEditingValue)this._value).composing);
        DartRuntimePrimitives.Assert(() => this.mounted);
        global::Doroti.Ui.Rect? composingRect = ((global::Doroti.Ui.Rect?)(object?)((Rect?)((dynamic)this.renderEditable).getRectForComposingRange(composingRange)));
        if ((composingRect is null))
        {
            long offsetLocal = (composingRange.isValid ? composingRange.start : 0L);
            composingRect = this.renderEditable.getLocalRectForCaret(new global::Doroti.Ui.TextPosition(offset: offsetLocal));
        }
        this._textInputConnection!.setComposingRect(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(composingRect)));
    }

    internal virtual void _updateCaretRectIfNeeded()
    {
        global::Doroti.Framework.Services.TextSelection? selectionLocal = ((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).selection;
        if (((selectionLocal is null) || !selectionLocal.isValid))
        {
            return;
        }
        var currentTextPosition = new global::Doroti.Ui.TextPosition(offset: selectionLocal.start);
        global::Doroti.Ui.Rect caretRect = ((global::Doroti.Ui.Rect)(object?)this.renderEditable.getLocalRectForCaret(currentTextPosition));
        this._textInputConnection!.setCaretRect(caretRect);
    }

    internal virtual global::Doroti.Ui.TextDirection _textDirection => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.TextDirection>(((((EditableText)(object)this.widget).textDirection ?? (TextDirection)Directionality.of(this.context))));
    public virtual global::Doroti.Framework.Services.TextEditingValue textEditingValue => this._value;
    internal virtual double _devicePixelRatio => MediaQuery.devicePixelRatioOf(this.context);
    public virtual void userUpdateTextEditingValue(global::Doroti.Framework.Services.TextEditingValue value, global::Doroti.Framework.Services.SelectionChangedCause cause)
    {
        var shouldShowCaret = (((EditableText)(object)this.widget).readOnly ? (!object.Equals(((global::Doroti.Framework.Services.TextEditingValue)this._value).selection, ((global::Doroti.Framework.Services.TextEditingValue)value).selection)) : (!object.Equals(this._value, value)));
        if (shouldShowCaret)
        {
            _scheduleShowCaretOnScreen(withAnimation: true);
        }
        if ((object.Equals(value, this.textEditingValue)))
        {
            if (!((EditableText)(object)this.widget).focusNode.hasFocus)
            {
                _flagInternalFocus();
                ((EditableText)(object)this.widget).focusNode.requestFocus();
                _selectionOverlay ??= _createSelectionOverlay();
            }
            return;
        }
        _formatAndSetValue(value, cause, userInteraction: true);
    }

    public virtual void bringIntoView(TextPosition position)
    {
        global::Doroti.Ui.Rect localRect = ((global::Doroti.Ui.Rect)(object?)this.renderEditable.getLocalRectForCaret(position));
        global::Doroti.Framework.Rendering.RevealedOffset targetOffset = ((global::Doroti.Framework.Rendering.RevealedOffset)(object?)_getOffsetToRevealCaret(localRect));
        this._scrollController.jumpTo(((global::Doroti.Framework.Rendering.RevealedOffset)targetOffset).offset);
        this.renderEditable.showOnScreen(rect: ((global::Doroti.Framework.Rendering.RevealedOffset)targetOffset).rect);
    }

    public virtual void showToolbar()
    {
        if (this._webContextMenuEnabled)
        {
            _ = false;
            return;
        }
        if ((this._selectionOverlay is null))
        {
            _ = false;
            return;
        }
        if (this._selectionOverlay!.toolbarIsVisible)
        {
            _ = false;
            return;
        }
        DartRuntimePrimitives.Ignore(this._liveTextInputStatus?.update());
        DartRuntimePrimitives.Ignore(this.clipboardStatus.update());
        this._selectionOverlay!.showToolbar();
        if (this._platformSupportsFadeOnScroll)
        {
            _listeningToScrollNotificationObserver = true;
            this._scrollNotificationObserver?.removeListener((global::System.Action<ScrollNotification>)this._handleContextMenuOnParentScroll);
            _scrollNotificationObserver = ScrollNotificationObserver.maybeOf(this.context);
            this._scrollNotificationObserver?.addListener((global::System.Action<ScrollNotification>)this._handleContextMenuOnParentScroll);
        }
        _ = true;
        return;
    }

    public virtual void hideToolbar(bool hideHandles = true)
    {
        _disposeScrollNotificationObserver();
        if (hideHandles)
        {
            this._selectionOverlay?.hide();
        }
        else
        {
            if ((this._selectionOverlay?.toolbarIsVisible ?? false))
            {
                this._selectionOverlay?.hideToolbar();
            }
        }
    }

    public virtual void toggleToolbar(bool hideHandles = true)
    {
        TextSelectionOverlay selectionOverlay = _selectionOverlay ??= _createSelectionOverlay();
        if (((TextSelectionOverlay)selectionOverlay).toolbarIsVisible)
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
        if ((((((!this.spellCheckEnabled || this._webContextMenuEnabled) || ((EditableText)(object)this.widget).readOnly) || (this._selectionOverlay is null)) || !this._spellCheckResultsReceived) || (findSuggestionSpanAtCursorIndex(((global::Doroti.Framework.Services.TextEditingValue)this.textEditingValue).selection.extentOffset) is null)))
        {
            return false;
        }
        DartRuntimePrimitives.Assert(() => (((SpellCheckConfiguration)this._spellCheckConfiguration).spellCheckSuggestionsToolbarBuilder is not null), () => (object?)"spellCheckSuggestionsToolbarBuilder must be defined in " + "SpellCheckConfiguration to show a toolbar with spell check " + "suggestions");
        this._selectionOverlay!.showSpellCheckSuggestionsToolbar(((global::System.Func<BuildContext, Widget>)((context) =>
        {
            return ((SpellCheckConfiguration)this._spellCheckConfiguration).spellCheckSuggestionsToolbarBuilder!(context, this);
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void showMagnifier(Offset positionToShow)
    {
        if ((this._selectionOverlay is null))
        {
            return;
        }
        if (this._selectionOverlay!.magnifierExists)
        {
            this._selectionOverlay!.updateMagnifier(positionToShow);
        }
        else
        {
            this._selectionOverlay!.showMagnifier(positionToShow);
        }
    }

    public virtual void hideMagnifier()
    {
        if ((this._selectionOverlay is null))
        {
            return;
        }
        this._selectionOverlay!.hideMagnifier();
    }

    public virtual void insertTextPlaceholder(Size size)
    {
        if (!this._stylusHandwritingEnabled)
        {
            return;
        }
        if (!((EditableText)(object)this.widget).controller.selection.isValid)
        {
            return;
        }
        setState(((global::System.Action)(() =>
        {
            _placeholderLocation = (((global::Doroti.Framework.Services.TextEditingValue)this._value).text.Length - ((EditableText)(object)this.widget).controller.selection.end);
        })));
    }

    public virtual void removeTextPlaceholder()
    {
        if ((!this._stylusHandwritingEnabled || (this._placeholderLocation == -1L)))
        {
            return;
        }
        setState(((global::System.Action)(() =>
        {
            _placeholderLocation = -1L;
        })));
    }

    public virtual void performSelector(string selectorName)
    {
        Intent? intent = global::Doroti.Framework.Widgets.Default_text_editing_shortcutsLibrary.intentForMacOSSelector(selectorName);
        if ((intent is not null))
        {
            BuildContext? primaryContext = global::Doroti.Framework.Widgets.Focus_managerLibrary.primaryFocus?.context;
            if ((primaryContext is not null))
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
            List<string>? autofillHintsLocal = ((EditableText)(object)this.widget).autofillHints?.ToList().ToList();
            global::Doroti.Framework.Services.AutofillConfiguration autofillConfigurationLocal = ((autofillHintsLocal is not null) ? new global::Doroti.Framework.Services.AutofillConfiguration(uniqueIdentifier: this.autofillId, autofillHints: autofillHintsLocal, currentEditingValue: this.currentTextEditingValue) : global::Doroti.Framework.Services.AutofillConfiguration.disabled);
            _viewId = checked((long)View.of(this.context).viewId);
            return new global::Doroti.Framework.Services.TextInputConfiguration(viewId: this._viewId, inputType: ((EditableText)(object)this.widget).keyboardType, readOnly: ((EditableText)(object)this.widget).readOnly, obscureText: ((EditableText)(object)this.widget).obscureText, autocorrect: ((EditableText)(object)this.widget).autocorrect, smartDashesType: ((EditableText)(object)this.widget).smartDashesType, smartQuotesType: ((EditableText)(object)this.widget).smartQuotesType, enableSuggestions: ((EditableText)(object)this.widget).enableSuggestions, enableInteractiveSelection: ((EditableText)(object)this.widget)._userSelectionEnabled, inputAction: (((EditableText)(object)this.widget).textInputAction ?? (((object.Equals(((EditableText)(object)this.widget).keyboardType, global::Doroti.Framework.Services.TextInputType.multiline)) ? global::Doroti.Framework.Services.TextInputAction.newline : global::Doroti.Framework.Services.TextInputAction.done))), textCapitalization: ((EditableText)(object)this.widget).textCapitalization, keyboardAppearance: ((EditableText)(object)this.widget).keyboardAppearance, autofillConfiguration: autofillConfigurationLocal, enableIMEPersonalizedLearning: ((EditableText)(object)this.widget).enableIMEPersonalizedLearning, allowedMimeTypes: ((((EditableText)(object)this.widget).contentInsertionConfiguration is null) ? new List<string>() : ((EditableText)(object)this.widget).contentInsertionConfiguration!.allowedMimeTypes), hintLocales: ((EditableText)(object)this.widget).hintLocales, enableInlinePrediction: ((EditableText)(object)this.widget).enableInlinePrediction);
            return default!;
        }
    }
    public virtual void autofill(global::Doroti.Framework.Services.TextEditingValue newEditingValue) => updateEditingValue(newEditingValue);
    public virtual void showAutocorrectionPromptRect(long start, long end)
    {
        setState(((global::System.Action)(() =>
        {
            _currentPromptRectRange = new global::Doroti.Ui.TextRange(start: start, end: end);
        })));
    }

    internal virtual global::System.Action? _semanticsOnCopy(TextSelectionControls? controls)
    {
        return ((global::System.Action)((global::System.Action)(((((EditableText)(object)this.widget).selectionEnabled && this._hasFocus) && (((((EditableText)(object)this.widget).selectionControls is TextSelectionHandleControls) ? this.copyEnabled : (this.copyEnabled && ((((EditableText)(object)this.widget).selectionControls?.canCopy(this) ?? false)))))) ? (() =>
        {
            controls?.handleCopy(this);
            copySelection(global::Doroti.Framework.Services.SelectionChangedCause.toolbar);
        }) : null)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::System.Action? _semanticsOnCut(TextSelectionControls? controls)
    {
        return ((global::System.Action)((global::System.Action)(((((EditableText)(object)this.widget).selectionEnabled && this._hasFocus) && (((((EditableText)(object)this.widget).selectionControls is TextSelectionHandleControls) ? this.cutEnabled : (this.cutEnabled && ((((EditableText)(object)this.widget).selectionControls?.canCut(this) ?? false)))))) ? (() =>
        {
            controls?.handleCut(this);
            cutSelection(global::Doroti.Framework.Services.SelectionChangedCause.toolbar);
        }) : null)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::System.Action? _semanticsOnPaste(TextSelectionControls? controls)
    {
        return DartRuntimePrimitives.AdaptAsyncCallback((global::System.Func<Future>?)((((((EditableText)(object)this.widget).selectionEnabled && this._hasFocus) && (((((EditableText)(object)this.widget).selectionControls is TextSelectionHandleControls) ? this.pasteEnabled : (this.pasteEnabled && ((((EditableText)(object)this.widget).selectionControls?.canPaste(this) ?? false)))))) && ((object.Equals(this.clipboardStatus.value, ClipboardStatus.pasteable)))) ? (async () =>
        {
            await controls?.handlePaste(this);
            await _pasteTextWithReporting(global::Doroti.Framework.Services.SelectionChangedCause.toolbar);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }) : null));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.TextPosition _moveBeyondTextBoundary(TextPosition extent, bool forward, global::Doroti.Framework.Services.TextBoundary textBoundary)
    {
        DartRuntimePrimitives.Assert(() => (extent.offset >= 0L));
        long newOffset = (forward ? (textBoundary.getTrailingTextBoundaryAt(extent.offset) ?? ((global::Doroti.Framework.Services.TextEditingValue)this._value).text.Length) : (textBoundary.getLeadingTextBoundaryAt((extent.offset - 1L)) ?? 0L));
        return ((global::Doroti.Ui.TextPosition)(object?)new global::Doroti.Ui.TextPosition(offset: newOffset));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.TextPosition _moveToTextBoundary(TextPosition extent, bool forward, global::Doroti.Framework.Services.TextBoundary textBoundary)
    {
        DartRuntimePrimitives.Assert(() => (extent.offset >= 0L));
        long caretOffset = default!;
        switch (extent.affinity)
        {
            case TextAffinity.upstream:
                {
                    if (((extent.offset < 1L) && !forward))
                    {
                        DartRuntimePrimitives.Assert(() => (extent.offset == 0L));
                        return ((global::Doroti.Ui.TextPosition)(object?)new global::Doroti.Ui.TextPosition(offset: 0L));
                    }
                    caretOffset = Math.Max(0L, (extent.offset - 1L));
                    break;
                }
            case TextAffinity.downstream:
                {
                    caretOffset = extent.offset;
                    break;
                }
        }
        return ((global::Doroti.Ui.TextPosition)(object?)(forward ? new global::Doroti.Ui.TextPosition(offset: (textBoundary.getTrailingTextBoundaryAt(caretOffset) ?? ((global::Doroti.Framework.Services.TextEditingValue)this._value).text.Length), affinity: TextAffinity.upstream) : new global::Doroti.Ui.TextPosition(offset: (textBoundary.getLeadingTextBoundaryAt(caretOffset) ?? 0L))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Services.TextBoundary _characterBoundary() => (((EditableText)(object)this.widget).obscureText ? new _CodePointBoundary__editable_text(((global::Doroti.Framework.Services.TextEditingValue)this._value).text) : new global::Doroti.Framework.Services.CharacterBoundary(((global::Doroti.Framework.Services.TextEditingValue)this._value).text));
    internal virtual global::Doroti.Framework.Services.TextBoundary _nextWordBoundary() => (((EditableText)(object)this.widget).obscureText ? _documentBoundary() : ((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).wordBoundaries.moveByWordBoundary);
    internal virtual global::Doroti.Framework.Services.TextBoundary _linebreak() => (((EditableText)(object)this.widget).obscureText ? _documentBoundary() : new global::Doroti.Framework.Services.LineBoundary(this.renderEditable));
    internal virtual global::Doroti.Framework.Services.TextBoundary _paragraphBoundary() => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Services.TextBoundary>(new global::Doroti.Framework.Services.ParagraphBoundary(((global::Doroti.Framework.Services.TextEditingValue)this._value).text));
    internal virtual global::Doroti.Framework.Services.TextBoundary _documentBoundary() => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Services.TextBoundary>(new global::Doroti.Framework.Services.DocumentBoundary(((global::Doroti.Framework.Services.TextEditingValue)this._value).text));
    internal virtual Action<T> _makeOverridable<T>(Action<T> defaultAction) where T : Intent
    {
        return Action<T>.CreateOverridable(context: this.context, defaultAction: defaultAction);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _transposeCharacters(TransposeCharactersIntent intent)
    {
        if ((((((global::Doroti.Framework.Services.TextEditingValue)this._value).text.characters().Count <= 1L) || !((global::Doroti.Framework.Services.TextEditingValue)this._value).selection.isCollapsed) || (((global::Doroti.Framework.Services.TextEditingValue)this._value).selection.baseOffset == 0L)))
        {
            return;
        }
        string textLocal = ((global::Doroti.Framework.Services.TextEditingValue)this._value).text;
        global::Doroti.Framework.Services.TextSelection selectionLocal = ((global::Doroti.Framework.Services.TextEditingValue)this._value).selection;
        var atEnd = (((global::Doroti.Framework.Services.TextSelection)selectionLocal).baseOffset == textLocal.Length);
        var transposing = new CharacterRange(textLocal, ((global::Doroti.Framework.Services.TextSelection)selectionLocal).baseOffset);
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
        DartRuntimePrimitives.Assert(() => (transposing.currentCharacters.Count == 2L));
        userUpdateTextEditingValue(new global::Doroti.Framework.Services.TextEditingValue(text: (((transposing.stringBefore + transposing.currentCharacters.last) + transposing.currentCharacters.first) + transposing.stringAfter), selection: global::Doroti.Framework.Services.TextSelection.CreateCollapsed(offset: (transposing.stringBeforeLength + transposing.Current.Length))), global::Doroti.Framework.Services.SelectionChangedCause.keyboard);
    }

    internal virtual void _replaceText(ReplaceTextIntent intent)
    {
        global::Doroti.Framework.Services.TextEditingValue oldValue = this._value;
        global::Doroti.Framework.Services.TextEditingValue newValue = ((global::Doroti.Framework.Services.TextEditingValue)(object?)((ReplaceTextIntent)intent).currentTextEditingValue.replaced(((ReplaceTextIntent)intent).replacementRange, ((ReplaceTextIntent)intent).replacementText));
        userUpdateTextEditingValue(newValue, ((ReplaceTextIntent)intent).cause);
        if ((object.Equals(newValue, oldValue)))
        {
            _didChangeTextEditingValue();
        }
    }

    internal virtual void _scrollToDocumentBoundary(ScrollToDocumentBoundaryIntent intent)
    {
        if (intent.forward)
        {
            bringIntoView(new global::Doroti.Ui.TextPosition(offset: ((global::Doroti.Framework.Services.TextEditingValue)this._value).text.Length));
        }
        else
        {
            bringIntoView(new global::Doroti.Ui.TextPosition(offset: 0L));
        }
    }

    internal virtual void _scroll(ScrollIntent intent)
    {
        if ((!object.Equals(((ScrollIntent)intent).type, ScrollIncrementType.page)))
        {
            return;
        }
        ScrollPosition positionLocal = ((ScrollController)this._scrollController).position;
        if ((((EditableText)(object)this.widget).maxLines == 1L))
        {
            this._scrollController.jumpTo(((ScrollPosition)positionLocal).maxScrollExtent);
            return;
        }
        if (((((ScrollPosition)positionLocal).maxScrollExtent == 0.0) && (((ScrollPosition)positionLocal).minScrollExtent == 0.0)))
        {
            return;
        }
        var state = ((ScrollableState?)(object?)((GlobalKey<IState>)this._scrollableKey).currentState)!;
        double increment = ScrollAction.getDirectionalIncrement(DartRuntimePrimitives.RequireValue(state), intent);
        double destination = Dart_uiLibrary.clampDouble((((ScrollPosition)positionLocal).pixels + increment), ((ScrollPosition)positionLocal).minScrollExtent, ((ScrollPosition)positionLocal).maxScrollExtent);
        if ((destination == ((ScrollPosition)positionLocal).pixels))
        {
            return;
        }
        this._scrollController.jumpTo(destination);
    }

    internal virtual void _updateSelection(UpdateSelectionIntent intent)
    {
        DartRuntimePrimitives.Assert(() => (((UpdateSelectionIntent)intent).newSelection.start <= ((UpdateSelectionIntent)intent).currentTextEditingValue.text.Length), () => (object?)$"invalid selection: {((UpdateSelectionIntent)intent).newSelection}: it must not exceed the current text length {((UpdateSelectionIntent)intent).currentTextEditingValue.text.Length}");
        DartRuntimePrimitives.Assert(() => (((UpdateSelectionIntent)intent).newSelection.end <= ((UpdateSelectionIntent)intent).currentTextEditingValue.text.Length), () => (object?)$"invalid selection: {((UpdateSelectionIntent)intent).newSelection}: it must not exceed the current text length {((UpdateSelectionIntent)intent).currentTextEditingValue.text.Length}");
        bringIntoView(((UpdateSelectionIntent)intent).newSelection.extent);
        userUpdateTextEditingValue(((UpdateSelectionIntent)intent).currentTextEditingValue.copyWith(selection: ((UpdateSelectionIntent)intent).newSelection), ((UpdateSelectionIntent)intent).cause);
    }

    internal virtual object? _hideToolbarIfVisible(DismissIntent intent)
    {
        if ((this._selectionOverlay?.toolbarIsVisible ?? false))
        {
            hideToolbar(false);
            return null;
        }
        return Actions.invoke(this.context, intent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _onTapOutside(BuildContext context, global::Doroti.Framework.Gestures.PointerDownEvent @event)
    {
        _hadFocusOnTapDown = true;
        if ((((EditableText)(object)this.widget).onTapOutside is not null))
        {
            ((EditableText)(object)this.widget).onTapOutside!(@event);
        }
        else
        {
            _defaultOnTapOutside(context, @event);
        }
    }

    internal virtual void _onTapUpOutside(BuildContext context, global::Doroti.Framework.Gestures.PointerUpEvent @event)
    {
        if (!this._hadFocusOnTapDown)
        {
            return;
        }
        _hadFocusOnTapDown = false;
        if ((((EditableText)(object)this.widget).onTapUpOutside is not null))
        {
            ((EditableText)(object)this.widget).onTapUpOutside!(@event);
        }
        else
        {
            _defaultOnTapUpOutside(context, @event);
        }
    }

    internal virtual void _defaultOnTapOutside(BuildContext context, global::Doroti.Framework.Gestures.PointerDownEvent @event)
    {
        Actions.invoke(context, new EditableTextTapOutsideIntent(focusNode: ((EditableText)(object)this.widget).focusNode, pointerDownEvent: @event));
    }

    internal virtual void _defaultOnTapUpOutside(BuildContext context, global::Doroti.Framework.Gestures.PointerUpEvent @event)
    {
        Actions.invoke(context, new EditableTextTapUpOutsideIntent(focusNode: ((EditableText)(object)this.widget).focusNode, pointerUpEvent: @event));
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        if ((this.wantKeepAlive && (this._keepAliveHandle is null)))
        {
            _ensureKeepAlive();
        }
        TextSelectionControls? controls = ((EditableText)(object)this.widget).selectionControls;
        global::Doroti.Framework.Painting.TextScaler effectiveTextScaler = ((((EditableText)(object)this.widget).textScaler, ((EditableText)(object)this.widget).textScaleFactor) switch { (global::Doroti.Framework.Painting.TextScaler textScalerLocal, _) => textScalerLocal, (null, double textScaleFactorLocal) => global::Doroti.Framework.Painting.TextScaler.CreateLinear(textScaleFactorLocal), (null, null) => MediaQuery.textScalerOf(context) });
        double? lineHeightScaleFactorLocal = MediaQuery.maybeLineHeightScaleFactorOverrideOf(context);
        double? letterSpacingLocal = MediaQuery.maybeLetterSpacingOverrideOf(context);
        double? wordSpacingLocal = MediaQuery.maybeWordSpacingOverrideOf(context);
        global::Doroti.Ui.SemanticsInputType inputTypeLocal = default!;
        switch (((EditableText)(object)this.widget).keyboardType)
        {
            case var __constant235617 when (object.Equals(__constant235617, global::Doroti.Framework.Services.TextInputType.phone)):
                {
                    inputTypeLocal = SemanticsInputType.phone;
                    break;
                }
            case var __constant235698 when (object.Equals(__constant235698, global::Doroti.Framework.Services.TextInputType.url)):
                {
                    inputTypeLocal = SemanticsInputType.url;
                    break;
                }
            case var __constant235775 when (object.Equals(__constant235775, global::Doroti.Framework.Services.TextInputType.emailAddress)):
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
        return ((Widget)(object?)new _CompositionCallback__editable_text(compositeCallback: (global::System.Action<global::Doroti.Framework.Rendering.Layer>)this._compositeCallback, enabled: this._hasInputConnection, child: new Actions(actions: this._actions, child: new Builder(builder: ((global::System.Func<BuildContext, Widget>)((context) =>
        {
            return ((Widget)(object?)new TextFieldTapRegion(groupId: ((EditableText)(object)this.widget).groupId, onTapOutside: ((global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>)(this._hasFocus ? ((@event) => { _onTapOutside(context, @event); }) : null)), onTapUpOutside: ((global::System.Action<global::Doroti.Framework.Gestures.PointerUpEvent>)((@event) => { _onTapUpOutside(context, @event); })), debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode ? null : "EditableText"), child: new MouseRegion(cursor: (((EditableText)(object)this.widget).mouseCursor ?? global::Doroti.Framework.Services.SystemMouseCursors.text), child: new UndoHistory<global::Doroti.Framework.Services.TextEditingValue>(value: ((EditableText)(object)this.widget).controller, onTriggered: ((global::System.Action<global::Doroti.Framework.Services.TextEditingValue>)((value) =>
            {
                userUpdateTextEditingValue(value, global::Doroti.Framework.Services.SelectionChangedCause.keyboard);
            })), shouldChangeUndoStack: ((global::System.Func<global::Doroti.Framework.Services.TextEditingValue?, global::Doroti.Framework.Services.TextEditingValue, bool>)((oldValue, newValue) =>
            {
                if (!((global::Doroti.Framework.Services.TextEditingValue)newValue).selection.isValid)
                {
                    return false;
                }
                if ((oldValue is null))
                {
                    return true;
                }
                switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
                {
                    case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                    case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                    case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                    case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                    case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                        {
                            if (!((EditableText)(object)this.widget).controller.value.composing.isCollapsed)
                            {
                                return false;
                            }
                            break;
                        }
                    case global::Doroti.Framework.Foundation.TargetPlatform.android:
                        {
                            break;
                        }
                }
                return ((((global::Doroti.Framework.Services.TextEditingValue)oldValue).text != ((global::Doroti.Framework.Services.TextEditingValue)newValue).text) || (!object.Equals(((global::Doroti.Framework.Services.TextEditingValue)oldValue).composing, ((global::Doroti.Framework.Services.TextEditingValue)newValue).composing)));
                throw new InvalidOperationException("Dart closure completed without a value.");
            })), undoStackModifier: ((global::System.Func<global::Doroti.Framework.Services.TextEditingValue, global::Doroti.Framework.Services.TextEditingValue>)((value) =>
            {
                return ((object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.android)) ? value.copyWith(composing: TextRange.empty) : value);
                throw new InvalidOperationException("Dart closure completed without a value.");
            })), focusNode: ((EditableText)(object)this.widget).focusNode, controller: ((EditableText)(object)this.widget).undoController, child: new Focus(focusNode: ((EditableText)(object)this.widget).focusNode, includeSemantics: false, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode ? null : "EditableText"), child: new NotificationListener<ScrollNotification>(onNotification: ((global::System.Func<ScrollNotification, bool>?)((notification) =>
            {
                _handleContextMenuOnScroll(notification);
                _scribbleCacheKey = null;
                return false;
                throw new InvalidOperationException("Dart closure completed without a value.");
            })), child: new Scrollable(key: this._scrollableKey, excludeFromSemantics: true, axisDirection: (this._isMultiline ? global::Doroti.Framework.Painting.AxisDirection.down : global::Doroti.Framework.Painting.AxisDirection.right), controller: this._scrollController, physics: (((EditableText)(object)this.widget).scrollPhysics ?? (((!this._isMultiline && (object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.iOS))) ? new _NeverUserScrollableScrollPhysics__editable_text() : null))), dragStartBehavior: ((EditableText)(object)this.widget).dragStartBehavior, restorationId: ((EditableText)(object)this.widget).restorationId, scrollBehavior: ((((EditableText)(object)this.widget).scrollBehavior ?? (ScrollBehavior)ScrollConfiguration.of(context).copyWith(scrollbars: this._isMultiline, overscroll: false))), viewportBuilder: ((global::System.Func<BuildContext, global::Doroti.Framework.Rendering.ViewportOffset, Widget>)((context, offset) =>
            {
                return ((Widget)(object?)new CompositedTransformTarget(link: this._toolbarLayerLink, child: new Semantics(inputType: inputTypeLocal, onCopy: _semanticsOnCopy(controls), onCut: _semanticsOnCut(controls), onPaste: _semanticsOnPaste(controls), child: new _ScribbleFocusable__editable_text(editableKey: this._editableKey, enabled: this._stylusHandwritingEnabled, focusNode: ((EditableText)(object)this.widget).focusNode, updateSelectionRects: ((global::System.Action)(() =>
                {
                    _openInputConnection();
                    _updateSelectionRects(force: true);
                })), child: new SizeChangedLayoutNotifier(child: new _Editable__editable_text(key: this._editableKey, startHandleLayerLink: this._startHandleLayerLink, endHandleLayerLink: this._endHandleLayerLink, inlineSpan: _OverridingTextStyleTextSpanUtils__editable_text.applyTextSpacingOverrides(lineHeightScaleFactor: lineHeightScaleFactorLocal, letterSpacing: letterSpacingLocal, wordSpacing: wordSpacingLocal, textSpan: buildTextSpan()), value: this._value, cursorColor: this._cursorColor, backgroundCursorColor: ((EditableText)(object)this.widget).backgroundCursorColor, showCursor: this._cursorVisibilityNotifier, forceLine: ((EditableText)(object)this.widget).forceLine, readOnly: ((EditableText)(object)this.widget).readOnly, hasFocus: this._hasFocus, maxLines: ((EditableText)(object)this.widget).maxLines, minLines: ((EditableText)(object)this.widget).minLines, expands: ((EditableText)(object)this.widget).expands, strutStyle: ((EditableText)(object)this.widget).strutStyle.merge(new global::Doroti.Framework.Painting.StrutStyle(height: lineHeightScaleFactorLocal)), selectionColor: ((this._selectionOverlay?.spellCheckToolbarIsVisible ?? false) ? (((SpellCheckConfiguration)this._spellCheckConfiguration).misspelledSelectionColor ?? ((EditableText)(object)this.widget).selectionColor) : ((EditableText)(object)this.widget).selectionColor), textScaler: effectiveTextScaler, textAlign: ((EditableText)(object)this.widget).textAlign, textDirection: this._textDirection, locale: ((EditableText)(object)this.widget).locale, textHeightBehavior: ((((EditableText)(object)this.widget).textHeightBehavior ?? (TextHeightBehavior)DefaultTextHeightBehavior.maybeOf(context))), textWidthBasis: ((EditableText)(object)this.widget).textWidthBasis, obscuringCharacter: ((EditableText)(object)this.widget).obscuringCharacter, obscureText: ((EditableText)(object)this.widget).obscureText, offset: offset, rendererIgnoresPointer: ((EditableText)(object)this.widget).rendererIgnoresPointer, cursorWidth: ((EditableText)(object)this.widget).cursorWidth, cursorHeight: ((EditableText)(object)this.widget).cursorHeight, cursorRadius: ((EditableText)(object)this.widget).cursorRadius, cursorOffset: (((EditableText)(object)this.widget).cursorOffset ?? Offset.zero), selectionHeightStyle: ((EditableText)(object)this.widget).selectionHeightStyle, selectionWidthStyle: ((EditableText)(object)this.widget).selectionWidthStyle, paintCursorAboveText: ((EditableText)(object)this.widget).paintCursorAboveText, enableInteractiveSelection: ((EditableText)(object)this.widget)._userSelectionEnabled, textSelectionDelegate: this, devicePixelRatio: this._devicePixelRatio, promptRectRange: this._currentPromptRectRange, promptRectColor: ((EditableText)(object)this.widget).autocorrectionTextRectColor, clipBehavior: ((EditableText)(object)this.widget).clipBehavior))))));
                throw new InvalidOperationException("Dart closure completed without a value.");
            })))))))));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Painting.TextSpan buildTextSpan()
    {
        if (((EditableText)(object)this.widget).obscureText)
        {
            string textLocal = ((global::Doroti.Framework.Services.TextEditingValue)this._value).text;
            textLocal = DartCoreExtensions.repeat(((EditableText)(object)this.widget).obscuringCharacter, textLocal.Length);
            var mobilePlatforms = new HashSet<global::Doroti.Framework.Foundation.TargetPlatform> { global::Doroti.Framework.Foundation.TargetPlatform.android, global::Doroti.Framework.Foundation.TargetPlatform.fuchsia, global::Doroti.Framework.Foundation.TargetPlatform.iOS };
            bool brieflyShowPasswordLocal = (WidgetsBinding.instance.platformDispatcher.brieflyShowPassword && mobilePlatforms.Contains(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform));
            if (brieflyShowPasswordLocal)
            {
                long? o = ((this._obscureShowCharTicksPending > 0L) ? this._obscureLatestCharIndex : null);
                if ((((o is not null) && (o >= 0L)) && (DartRuntimePrimitives.RequireValue(o) < textLocal.Length)))
                {
                    long o__246733__value246816 = DartRuntimePrimitives.RequireValue(o);
                    textLocal = textLocal.replaceRange(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(o__246733__value246816)), (DartRuntimePrimitives.RequireValue(o__246733__value246816) + 1L), ((global::Doroti.Framework.Services.TextEditingValue)this._value).text.substring(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(o__246733__value246816)), (DartRuntimePrimitives.RequireValue(o__246733__value246816) + 1L)));
                }
            }
            return new global::Doroti.Framework.Painting.TextSpan(style: this._style, text: textLocal);
        }
        if (((this._placeholderLocation >= 0L) && (this._placeholderLocation <= ((global::Doroti.Framework.Services.TextEditingValue)this._value).text.Length)))
        {
            var placeholders = new List<_ScribblePlaceholder__editable_text>();
            long placeholderLocation = (((global::Doroti.Framework.Services.TextEditingValue)this._value).text.Length - this._placeholderLocation);
            if (this._isMultiline)
            {
                placeholders.Add(new _ScribblePlaceholder__editable_text(child: SizedBox.CreateShrink(), size: Size.zero));
                placeholders.Add(new _ScribblePlaceholder__editable_text(child: SizedBox.CreateShrink(), size: new global::Doroti.Ui.Size(this.renderEditable.size.width, 0.0)));
            }
            else
            {
                placeholders.Add(new _ScribblePlaceholder__editable_text(child: SizedBox.CreateShrink(), size: new global::Doroti.Ui.Size(100.0, 0.0)));
            }
            return new global::Doroti.Framework.Painting.TextSpan(style: this._style, children: new List<global::Doroti.Framework.Painting.InlineSpan> { new global::Doroti.Framework.Painting.TextSpan(text: ((global::Doroti.Framework.Services.TextEditingValue)this._value).text.substring(0L, placeholderLocation)), new global::Doroti.Framework.Painting.TextSpan(text: ((global::Doroti.Framework.Services.TextEditingValue)this._value).text.substring(placeholderLocation)) });
        }
        bool withComposingLocal = (!((EditableText)(object)this.widget).readOnly && this._hasFocus);
        if (this._spellCheckResultsReceived)
        {
            DartRuntimePrimitives.Assert(() => ((!((global::Doroti.Framework.Services.TextEditingValue)this._value).composing.isValid || !withComposingLocal) || ((global::Doroti.Framework.Services.TextEditingValue)this._value).isComposingRangeValid));
            bool composingRegionOutOfRange = (!((global::Doroti.Framework.Services.TextEditingValue)this._value).isComposingRangeValid || !withComposingLocal);
            return global::Doroti.Framework.Widgets.Spell_checkLibrary.buildTextSpanWithSpellCheckSuggestions(this._value, composingRegionOutOfRange, this._style, ((SpellCheckConfiguration)this._spellCheckConfiguration).misspelledTextStyle!, this.spellCheckResults!);
        }
        return ((global::Doroti.Framework.Painting.TextSpan)(object?)((EditableText)(object)this.widget).controller.buildTextSpan(context: this.context, style: this._style, withComposing: withComposingLocal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _ensureKeepAlive()
    {
        DartRuntimePrimitives.Assert(() => (this._keepAliveHandle is null));
        this._keepAliveHandle = new KeepAliveHandle();
        new KeepAliveNotification(this._keepAliveHandle!).dispatch(this.context);
    }

    public virtual void _releaseKeepAlive()
    {
        this._keepAliveHandle!.dispose();
        this._keepAliveHandle = null;
    }

    public virtual void updateKeepAlive()
    {
        if (this.wantKeepAlive)
        {
            if ((this._keepAliveHandle is null))
            {
                _ensureKeepAlive();
            }
        }
        else
        {
            if ((this._keepAliveHandle is not null))
            {
                _releaseKeepAlive();
            }
        }
    }

    public override void deactivate()
    {
        if ((this._keepAliveHandle is not null))
        {
            _releaseKeepAlive();
        }
        base.deactivate();
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = this._tickerModeNotifier!.value;
        var result = ((Func<_WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
    __cascade.muted = !((TickerModeData)values).enabled;
    __cascade.forceFrames = ((TickerModeData)values).forceFrames;
    return __cascade;
}))();
        this._tickers!.Add(result);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(_WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => (this._tickers is not null));
        DartRuntimePrimitives.Assert(() => this._tickers!.Contains(ticker));
        this._tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if ((this._tickers is not null))
        {
            TickerModeData values = this._tickerModeNotifier!.value;
            bool mutedLocal = !((TickerModeData)values).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = ((TickerModeData)values).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
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
        this.selectionHeightStyle = ((selectionHeightStyle ?? (BoxHeightStyle)EditableText.defaultSelectionHeightStyle));
        this.selectionWidthStyle = ((selectionWidthStyle ?? (BoxWidthStyle)EditableText.defaultSelectionWidthStyle));
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new global::Doroti.Framework.Rendering.RenderEditable(text: this.inlineSpan, cursorColor: this.cursorColor, startHandleLayerLink: this.startHandleLayerLink, endHandleLayerLink: this.endHandleLayerLink, backgroundCursorColor: this.backgroundCursorColor, showCursor: this.showCursor, forceLine: this.forceLine, readOnly: this.readOnly, hasFocus: this.hasFocus, maxLines: this.maxLines, minLines: this.minLines, expands: this.expands, strutStyle: this.strutStyle, selectionColor: this.selectionColor, textScaler: this.textScaler, textAlign: this.textAlign, textDirection: this.textDirection, locale: (this.locale ?? Localizations.maybeLocaleOf(context)), selection: ((global::Doroti.Framework.Services.TextEditingValue)this.value).selection, offset: this.offset, ignorePointer: this.rendererIgnoresPointer, obscuringCharacter: this.obscuringCharacter, obscureText: this.obscureText, textHeightBehavior: this.textHeightBehavior, textWidthBasis: this.textWidthBasis, cursorWidth: this.cursorWidth, cursorHeight: this.cursorHeight, cursorRadius: this.cursorRadius, cursorOffset: this.cursorOffset, paintCursorAboveText: this.paintCursorAboveText, selectionHeightStyle: DartRuntimePrimitives.RequireValue(this.selectionHeightStyle), selectionWidthStyle: DartRuntimePrimitives.RequireValue(this.selectionWidthStyle), enableInteractiveSelection: this.enableInteractiveSelection, textSelectionDelegate: this.textSelectionDelegate, devicePixelRatio: this.devicePixelRatio, promptRectRange: this.promptRectRange, promptRectColor: this.promptRectColor, clipBehavior: this.clipBehavior));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderEditable)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Rendering.RenderEditable>)(() =>
{
    var __cascade = __renderObject;
    __cascade.text = this.inlineSpan;
    __cascade.cursorColor = this.cursorColor;
    __cascade.startHandleLayerLink = this.startHandleLayerLink;
    __cascade.endHandleLayerLink = this.endHandleLayerLink;
    __cascade.backgroundCursorColor = this.backgroundCursorColor;
    __cascade.showCursor = this.showCursor;
    __cascade.forceLine = this.forceLine;
    __cascade.readOnly = this.readOnly;
    __cascade.hasFocus = this.hasFocus;
    __cascade.maxLines = this.maxLines;
    __cascade.minLines = this.minLines;
    __cascade.expands = this.expands;
    __cascade.strutStyle = this.strutStyle;
    __cascade.selectionColor = this.selectionColor;
    __cascade.textScaler = this.textScaler;
    __cascade.textAlign = this.textAlign;
    __cascade.textDirection = this.textDirection;
    __cascade.locale = (this.locale ?? Localizations.maybeLocaleOf(context));
    __cascade.selection = ((global::Doroti.Framework.Services.TextEditingValue)this.value).selection;
    __cascade.offset = this.offset;
    __cascade.ignorePointer = this.rendererIgnoresPointer;
    __cascade.textHeightBehavior = this.textHeightBehavior;
    __cascade.textWidthBasis = this.textWidthBasis;
    __cascade.obscuringCharacter = this.obscuringCharacter;
    __cascade.obscureText = this.obscureText;
    __cascade.cursorWidth = this.cursorWidth;
    __cascade.setCursorHeight(this.cursorHeight);
    __cascade.cursorRadius = this.cursorRadius;
    __cascade.cursorOffset = this.cursorOffset;
    __cascade.selectionHeightStyle = this.selectionHeightStyle;
    __cascade.selectionWidthStyle = this.selectionWidthStyle;
    __cascade.enableInteractiveSelection = this.enableInteractiveSelection;
    __cascade.textSelectionDelegate = this.textSelectionDelegate;
    __cascade.devicePixelRatio = this.devicePixelRatio;
    __cascade.paintCursorAboveText = this.paintCursorAboveText;
    __cascade.promptRectColor = this.promptRectColor;
    __cascade.clipBehavior = this.clipBehavior;
    __cascade.setPromptRectRange(this.promptRectRange);
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
            return global::Doroti.Framework.Painting.RenderComparison.identical;
        }
        bool needsLayout = ((((((((!object.Equals(this.textAlign, ((_ScribbleCacheKey__editable_text)other).textAlign)) || (!object.Equals(this.textDirection, ((_ScribbleCacheKey__editable_text)other).textDirection))) || (!object.Equals(this.textScaler, ((_ScribbleCacheKey__editable_text)other).textScaler))) || (!object.Equals(((this.textHeightBehavior ?? new global::Doroti.Ui.TextHeightBehavior())), ((((_ScribbleCacheKey__editable_text)other).textHeightBehavior ?? new global::Doroti.Ui.TextHeightBehavior()))))) || (!object.Equals(this.locale, ((_ScribbleCacheKey__editable_text)other).locale))) || (!object.Equals(this.structStyle, ((_ScribbleCacheKey__editable_text)other).structStyle))) || (this.placeholder != ((_ScribbleCacheKey__editable_text)other).placeholder)) || (!object.Equals(this.size, ((_ScribbleCacheKey__editable_text)other).size)));
        return (needsLayout ? global::Doroti.Framework.Painting.RenderComparison.layout : this.inlineSpan.compareTo(((_ScribbleCacheKey__editable_text)other).inlineSpan));
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
        this._elementIdentifier = (_nextElementIdentifier++).ToString();
    }

    public override void initState()
    {
        base.initState();
        if (((_ScribbleFocusable__editable_text)(object)this.widget).enabled)
        {
            TextInput.registerScribbleElement(this.elementIdentifier, this);
        }
    }

    public override void didUpdateWidget(_ScribbleFocusable__editable_text oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!((_ScribbleFocusable__editable_text)oldWidget).enabled && ((_ScribbleFocusable__editable_text)(object)this.widget).enabled))
        {
            TextInput.registerScribbleElement(this.elementIdentifier, this);
        }
        if ((((_ScribbleFocusable__editable_text)oldWidget).enabled && !((_ScribbleFocusable__editable_text)(object)this.widget).enabled))
        {
            TextInput.unregisterScribbleElement(this.elementIdentifier);
        }
    }

    public override void dispose()
    {
        TextInput.unregisterScribbleElement(this.elementIdentifier);
        base.dispose();
    }

    public virtual global::Doroti.Framework.Rendering.RenderEditable? renderEditable => ((global::Doroti.Framework.Rendering.RenderEditable?)(object?)((_ScribbleFocusable__editable_text)(object)this.widget).editableKey.currentContext?.findRenderObject())!;
    public virtual string elementIdentifier => this._elementIdentifier;
    public virtual void onScribbleFocus(Offset offset)
    {
        ((_ScribbleFocusable__editable_text)(object)this.widget).focusNode.requestFocus();
        this.renderEditable?.selectPositionAt(from: offset, cause: global::Doroti.Framework.Services.SelectionChangedCause.stylusHandwriting);
        this.widget.updateSelectionRects();
    }

    public virtual bool isInScribbleRect(Rect rect)
    {
        global::Doroti.Ui.Rect calculatedBounds = ((global::Doroti.Ui.Rect)(object?)this.bounds);
        if ((this.renderEditable?.readOnly ?? false))
        {
            return false;
        }
        if ((object.Equals(calculatedBounds, Rect.zero)))
        {
            return false;
        }
        if (!calculatedBounds.overlaps(rect))
        {
            return false;
        }
        global::Doroti.Ui.Rect intersection = ((global::Doroti.Ui.Rect)(object?)calculatedBounds.intersect(rect));
        var result = new global::Doroti.Framework.Gestures.HitTestResult();
        WidgetsBinding.instance.hitTestInView(result, ((Offset)((dynamic)intersection).center), checked((long)View.of(this.context).viewId));
        return ((global::Doroti.Framework.Gestures.HitTestResult)result).path.any(((entry) => (object.Equals(((global::Doroti.Framework.Gestures.HitTestEntry<global::Doroti.Framework.Gestures.HitTestTarget>)entry).target, this.renderEditable))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Rect bounds
    {
        get
        {
            var box = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)this.context.findRenderObject())!;
            if ((((box is null) || !this.mounted) || !box.attached))
            {
                return Rect.zero;
            }
            Matrix4 transform = ((Matrix4)(object?)box.getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null)));
            return MatrixUtils.transformRect(transform, global::Doroti.Ui.Rect.fromLTWH(0, 0, ((global::Doroti.Framework.Rendering.RenderBox)box).size.width, ((global::Doroti.Framework.Rendering.RenderBox)box).size.height));
            return default!;
        }
    }
    public override Widget build(BuildContext context)
    {
        return ((_ScribbleFocusable__editable_text)(object)this.widget).child;
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
        var hasStyle = (this.style is not null);
        if (hasStyle)
        {
            builder.pushStyle(this.style!.getTextStyle(textScaler: textScaler));
        }
        builder.addPlaceholder(this.size.width, this.size.height, this.alignment);
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
        DartRuntimePrimitives.Assert(() => (((position > 0L) && (position < this._text.Length)) && (this._text.Length > 1L)));
        return (TextPainter.isHighSurrogate(this._text.codeUnitAt((position - 1L))) && TextPainter.isLowSurrogate(this._text.codeUnitAt(position)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long? getLeadingTextBoundaryAt(long position)
    {
        if (((this._text.Length == 0) || (position < 0L)))
        {
            return null;
        }
        if ((position == 0L))
        {
            return 0L;
        }
        if ((position >= this._text.Length))
        {
            return this._text.Length;
        }
        if ((this._text.Length <= 1L))
        {
            return position;
        }
        return (_breaksSurrogatePair(position) ? (position - 1L) : position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long? getTrailingTextBoundaryAt(long position)
    {
        if (((this._text.Length == 0) || (position >= this._text.Length)))
        {
            return null;
        }
        if ((position < 0L))
        {
            return 0L;
        }
        if ((position == (this._text.Length - 1L)))
        {
            return this._text.Length;
        }
        if ((this._text.Length <= 1L))
        {
            return position;
        }
        return (_breaksSurrogatePair((position + 1L)) ? (position + 2L) : (position + 1L));
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
        if (((((EditableTextState)this.state)._selectionOverlay is null) || !((EditableTextState)this.state).selectionOverlay!.toolbarIsVisible))
        {
            return;
        }
        global::Doroti.Framework.Services.TextEditingValue oldValue = ((ReplaceTextIntent)intent).currentTextEditingValue;
        global::Doroti.Framework.Services.TextEditingValue newValue = ((global::Doroti.Framework.Services.TextEditingValue)(object?)((ReplaceTextIntent)intent).currentTextEditingValue.replaced(((ReplaceTextIntent)intent).replacementRange, ((ReplaceTextIntent)intent).replacementText));
        if ((((global::Doroti.Framework.Services.TextEditingValue)oldValue).text != ((global::Doroti.Framework.Services.TextEditingValue)newValue).text))
        {
            this.state.hideToolbar(false);
        }
    }

    public override object? invoke(T intent, BuildContext? context = null)
    {
        global::Doroti.Framework.Services.TextSelection selectionLocal = ((EditableTextState)this.state)._value.selection;
        if (!selectionLocal.isValid)
        {
            return null;
        }
        DartRuntimePrimitives.Assert(() => selectionLocal.isValid);
        global::Doroti.Framework.Services.TextBoundary atomicBoundary = ((global::Doroti.Framework.Services.TextBoundary)(object?)this.state._characterBoundary());
        if (!selectionLocal.isCollapsed)
        {
            var range = new global::Doroti.Ui.TextRange(start: (atomicBoundary.getLeadingTextBoundaryAt(selectionLocal.start) ?? ((EditableTextState)this.state)._value.text.Length), end: (atomicBoundary.getTrailingTextBoundaryAt((selectionLocal.end - 1L)) ?? 0L));
            var replaceTextIntent = new ReplaceTextIntent(((EditableTextState)this.state)._value, "", range, global::Doroti.Framework.Services.SelectionChangedCause.keyboard);
            _hideToolbarIfTextChanged(replaceTextIntent);
            return Actions.invoke(context!, replaceTextIntent);
        }
        long target = this._applyTextBoundary(((global::Doroti.Framework.Services.TextSelection)selectionLocal).@base, ((DirectionalTextEditingIntent)(object)intent).forward, this.getTextBoundary()).offset;
        global::Doroti.Ui.TextRange rangeToDelete = ((global::Doroti.Ui.TextRange)(object?)new global::Doroti.Framework.Services.TextSelection(baseOffset: (((DirectionalTextEditingIntent)(object)intent).forward ? (atomicBoundary.getLeadingTextBoundaryAt(((global::Doroti.Framework.Services.TextSelection)selectionLocal).baseOffset) ?? ((EditableTextState)this.state)._value.text.Length) : (atomicBoundary.getTrailingTextBoundaryAt((((global::Doroti.Framework.Services.TextSelection)selectionLocal).baseOffset - 1L)) ?? 0L)), extentOffset: target));
        var replaceTextIntentLocal = new ReplaceTextIntent(((EditableTextState)this.state)._value, "", rangeToDelete, global::Doroti.Framework.Services.SelectionChangedCause.keyboard);
        _hideToolbarIfTextChanged(replaceTextIntentLocal);
        return Actions.invoke(context!, replaceTextIntentLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isActionEnabled => DartRuntimePrimitives.ConvertValue<bool>((!this.state.widget.readOnly && ((EditableTextState)this.state)._value.selection.isValid));
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
        var endLocal = new global::Doroti.Ui.TextPosition(offset: ((EditableTextState)this.state).renderEditable.getLineAtOffset(position).end, affinity: TextAffinity.upstream);
        return (((object.Equals(endLocal, position)) && (endLocal.offset != ((EditableTextState)this.state).textEditingValue.text.Length)) && (((EditableTextState)this.state).textEditingValue.text.codeUnitAt(position.offset) != NEWLINE_CODE_UNIT));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isAtWordwrapDownstream(TextPosition position)
    {
        var startLocal = new global::Doroti.Ui.TextPosition(offset: ((EditableTextState)this.state).renderEditable.getLineAtOffset(position).start);
        return (((object.Equals(startLocal, position)) && (startLocal.offset != 0L)) && (((EditableTextState)this.state).textEditingValue.text.codeUnitAt((position.offset - 1L)) != NEWLINE_CODE_UNIT));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override object? invoke(T intent, BuildContext? context = null)
    {
        global::Doroti.Framework.Services.TextSelection selectionLocal = ((EditableTextState)this.state)._value.selection;
        DartRuntimePrimitives.Assert(() => selectionLocal.isValid);
        bool collapseSelectionLocal = (((DirectionalCaretMovementIntent)(object)intent).collapseSelection || !this.state.widget.selectionEnabled);
        if (((!selectionLocal.isCollapsed && !this.ignoreNonCollapsedSelection) && collapseSelectionLocal))
        {
            return Actions.invoke(context!, new UpdateSelectionIntent(((EditableTextState)this.state)._value, global::Doroti.Framework.Services.TextSelection.CreateCollapsed(offset: (((DirectionalTextEditingIntent)(object)intent).forward ? selectionLocal.end : selectionLocal.start)), global::Doroti.Framework.Services.SelectionChangedCause.keyboard));
        }
        global::Doroti.Ui.TextPosition extentLocal = ((global::Doroti.Ui.TextPosition)(object?)((global::Doroti.Framework.Services.TextSelection)selectionLocal).extent);
        if (((DirectionalCaretMovementIntent)(object)intent).continuesAtWrap)
        {
            if ((((DirectionalTextEditingIntent)(object)intent).forward && _isAtWordwrapUpstream(extentLocal)))
            {
                extentLocal = new global::Doroti.Ui.TextPosition(offset: extentLocal.offset);
            }
            else
            {
                if ((!((DirectionalTextEditingIntent)(object)intent).forward && _isAtWordwrapDownstream(extentLocal)))
                {
                    extentLocal = new global::Doroti.Ui.TextPosition(offset: extentLocal.offset, affinity: TextAffinity.upstream);
                }
            }
        }
        bool shouldTargetBase = (this.isExpand && ((((DirectionalTextEditingIntent)(object)intent).forward ? (((global::Doroti.Framework.Services.TextSelection)selectionLocal).baseOffset > ((global::Doroti.Framework.Services.TextSelection)selectionLocal).extentOffset) : (((global::Doroti.Framework.Services.TextSelection)selectionLocal).baseOffset < ((global::Doroti.Framework.Services.TextSelection)selectionLocal).extentOffset))));
        global::Doroti.Ui.TextPosition newExtent = ((global::Doroti.Ui.TextPosition)(object?)this.applyTextBoundary((shouldTargetBase ? ((global::Doroti.Framework.Services.TextSelection)selectionLocal).@base : extentLocal), ((DirectionalTextEditingIntent)(object)intent).forward, this.getTextBoundary()));
        global::Doroti.Framework.Services.TextSelection newSelection = ((collapseSelectionLocal || ((!this.isExpand && (newExtent.offset == ((global::Doroti.Framework.Services.TextSelection)selectionLocal).baseOffset)))) ? global::Doroti.Framework.Services.TextSelection.CreateFromPosition(newExtent) : (this.isExpand ? selectionLocal.expandTo(newExtent, (this.extentAtIndex || selectionLocal.isCollapsed)) : selectionLocal.extendTo(newExtent)));
        bool shouldCollapseToBase = (((DirectionalCaretMovementIntent)(object)intent).collapseAtReversal && ((((((global::Doroti.Framework.Services.TextSelection)selectionLocal).baseOffset - ((global::Doroti.Framework.Services.TextSelection)selectionLocal).extentOffset)) * ((((global::Doroti.Framework.Services.TextSelection)selectionLocal).baseOffset - ((global::Doroti.Framework.Services.TextSelection)newSelection).extentOffset))) < 0L));
        var newRange = (shouldCollapseToBase ? global::Doroti.Framework.Services.TextSelection.CreateFromPosition(((global::Doroti.Framework.Services.TextSelection)selectionLocal).@base) : newSelection);
        return Actions.invoke(context!, new UpdateSelectionIntent(((EditableTextState)this.state)._value, newRange, global::Doroti.Framework.Services.SelectionChangedCause.keyboard));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isActionEnabled
    {
        get
        {
            if (((global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb && this.state.widget.selectionEnabled) && ((EditableTextState)this.state)._value.composing.isValid))
            {
                return false;
            }
            return ((EditableTextState)this.state)._value.selection.isValid;
            return default!;
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
        global::Doroti.Framework.Services.TextSelection? runSelection = this._runSelection;
        if ((runSelection is null))
        {
            DartRuntimePrimitives.Assert(() => (this._verticalMovementRun is null));
            return;
        }
        _runSelection = ((EditableTextState)this.state)._value.selection;
        global::Doroti.Framework.Services.TextSelection currentSelection = this.state.widget.controller.selection;
        bool continueCurrentRun = (((currentSelection.isValid && currentSelection.isCollapsed) && (((global::Doroti.Framework.Services.TextSelection)currentSelection).baseOffset == ((global::Doroti.Framework.Services.TextSelection)runSelection).baseOffset)) && (((global::Doroti.Framework.Services.TextSelection)currentSelection).extentOffset == ((global::Doroti.Framework.Services.TextSelection)runSelection).extentOffset));
        if (!continueCurrentRun)
        {
            _verticalMovementRun = null;
            _runSelection = null;
        }
    }

    public override object? invoke(T intent, BuildContext? context = null)
    {
        DartRuntimePrimitives.Assert(() => ((EditableTextState)this.state)._value.selection.isValid);
        bool collapseSelectionLocal = (((DirectionalCaretMovementIntent)(object)intent).collapseSelection || !this.state.widget.selectionEnabled);
        global::Doroti.Framework.Services.TextEditingValue value = ((EditableTextState)this.state)._textEditingValueforTextLayoutMetrics;
        if (!((global::Doroti.Framework.Services.TextEditingValue)value).selection.isValid)
        {
            return default!;
        }
        if ((this._verticalMovementRun?.isValid == false))
        {
            _verticalMovementRun = null;
            _runSelection = null;
        }
        global::Doroti.Framework.Rendering.VerticalCaretMovementRun currentRun = ((this._verticalMovementRun ?? (global::Doroti.Framework.Rendering.VerticalCaretMovementRun)((EditableTextState)this.state).renderEditable.startVerticalCaretMovement(((EditableTextState)this.state).renderEditable.selection!.extent)));
        bool shouldMove = ((intent is ExtendSelectionVerticallyToAdjacentPageIntent) ? currentRun.moveByOffset((((intent.forward ? 1.0 : -1.0)) * ((EditableTextState)this.state).renderEditable.size.height)) : (((DirectionalTextEditingIntent)(object)intent).forward ? currentRun.moveNext() : currentRun.movePrevious()));
        global::Doroti.Ui.TextPosition newExtent = ((global::Doroti.Ui.TextPosition)(object?)(shouldMove ? ((global::Doroti.Framework.Rendering.VerticalCaretMovementRun)currentRun).current : (((DirectionalTextEditingIntent)(object)intent).forward ? new global::Doroti.Ui.TextPosition(offset: ((global::Doroti.Framework.Services.TextEditingValue)value).text.Length) : new global::Doroti.Ui.TextPosition(offset: 0L))));
        global::Doroti.Framework.Services.TextSelection newSelection = (collapseSelectionLocal ? global::Doroti.Framework.Services.TextSelection.CreateFromPosition(newExtent) : ((global::Doroti.Framework.Services.TextEditingValue)value).selection.extendTo(newExtent));
        Actions.invoke(context!, new UpdateSelectionIntent(value, newSelection, global::Doroti.Framework.Services.SelectionChangedCause.keyboard));
        if ((object.Equals(((EditableTextState)this.state)._value.selection, newSelection)))
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
            if (((global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb && this.state.widget.selectionEnabled) && ((EditableTextState)this.state)._value.composing.isValid))
            {
                return false;
            }
            return ((EditableTextState)this.state)._value.selection.isValid;
            return default!;
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
            if (((global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb && this.state.widget.selectionEnabled) && ((EditableTextState)this.state)._value.composing.isValid))
            {
                return false;
            }
            return base.isActionEnabled;
            return default!;
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
        if (!this.state.widget.selectionEnabled)
        {
            return null;
        }
        return Actions.invoke(context!, new UpdateSelectionIntent(((EditableTextState)this.state)._value, new global::Doroti.Framework.Services.TextSelection(baseOffset: 0L, extentOffset: ((EditableTextState)this.state)._value.text.Length), ((SelectAllTextIntent)intent).cause));
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
        if ((!((EditableTextState)this.state)._value.selection.isValid || ((EditableTextState)this.state)._value.selection.isCollapsed))
        {
            return default!;
        }
        if (!this.state.widget.selectionEnabled)
        {
            return default!;
        }
        if (((CopySelectionTextIntent)intent).collapseSelection)
        {
            this.state.cutSelection(((CopySelectionTextIntent)intent).cause);
        }
        else
        {
            this.state.copySelection(((CopySelectionTextIntent)intent).cause);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
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
        if (!this.state.widget.selectionEnabled)
        {
            return default!;
        }
        DartRuntimePrimitives.Ignore(this.state._pasteTextWithReporting(((PasteTextIntent)intent).cause));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _WebClipboardStatusNotifier__editable_text : ClipboardStatusNotifier
{
    public virtual ClipboardStatus value { get; set; } = ClipboardStatus.pasteable;

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
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                {
                    switch (((EditableTextTapOutsideIntent)intent).pointerDownEvent.kind)
                    {
                        case PointerDeviceKind.touch:
                            {
                                if (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb)
                                {
                                    ((EditableTextTapOutsideIntent)intent).focusNode.unfocus();
                                }
                                break;
                            }
                        case PointerDeviceKind.mouse:
                        case PointerDeviceKind.stylus:
                        case PointerDeviceKind.invertedStylus:
                        case PointerDeviceKind.unknown:
                            {
                                ((EditableTextTapOutsideIntent)intent).focusNode.unfocus();
                                break;
                            }
                        case PointerDeviceKind.trackpad:
                            {
                                throw new NotImplementedException("Unexpected pointer down event for trackpad");
                            }
                    }
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    ((EditableTextTapOutsideIntent)intent).focusNode.unfocus();
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
        if ((((lineHeightScaleFactor is null) && (letterSpacing is null)) && (wordSpacing is null)))
        {
            return textSpan;
        }
        return ((global::Doroti.Framework.Painting.TextSpan)(object?)_OverridingTextStyleTextSpanUtils__editable_text._applyTextStyleOverrides(new global::Doroti.Framework.Painting.TextStyle(height: lineHeightScaleFactor, letterSpacing: letterSpacing, wordSpacing: wordSpacing), textSpan));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Framework.Painting.TextSpan _applyTextStyleOverrides(global::Doroti.Framework.Painting.TextStyle overrideTextStyle, global::Doroti.Framework.Painting.TextSpan textSpan)
    {
        return new global::Doroti.Framework.Painting.TextSpan(text: ((global::Doroti.Framework.Painting.TextSpan)textSpan).text, children: ((global::Doroti.Framework.Painting.TextSpan)textSpan).children?.map<global::Doroti.Framework.Painting.InlineSpan, global::Doroti.Framework.Painting.InlineSpan>(((child) =>
        {
            if (((child is global::Doroti.Framework.Painting.TextSpan) && (object.Equals(DartRuntimePrimitives.RuntimeType(((global::Doroti.Framework.Painting.TextSpan)child)), typeof(global::Doroti.Framework.Painting.TextSpan)))))
            {
                return ((global::Doroti.Framework.Painting.InlineSpan)(object?)_OverridingTextStyleTextSpanUtils__editable_text._applyTextStyleOverrides(overrideTextStyle, ((global::Doroti.Framework.Painting.TextSpan)child)));
            }
            return child;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })).ToList(), style: (textSpan.style?.merge(overrideTextStyle) ?? overrideTextStyle), recognizer: ((global::Doroti.Framework.Painting.TextSpan)textSpan).recognizer, mouseCursor: ((global::Doroti.Framework.Painting.TextSpan)textSpan).mouseCursor, onEnter: (global::System.Action<global::Doroti.Framework.Gestures.PointerEnterEvent>?)((global::Doroti.Framework.Painting.TextSpan)textSpan).onEnter, onExit: (global::System.Action<global::Doroti.Framework.Gestures.PointerExitEvent>?)((global::Doroti.Framework.Painting.TextSpan)textSpan).onExit, semanticsLabel: ((global::Doroti.Framework.Painting.TextSpan)textSpan).semanticsLabel, semanticsIdentifier: ((global::Doroti.Framework.Painting.TextSpan)textSpan).semanticsIdentifier, locale: ((global::Doroti.Framework.Painting.TextSpan)textSpan).locale, spellOut: ((global::Doroti.Framework.Painting.TextSpan)textSpan).spellOut);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
