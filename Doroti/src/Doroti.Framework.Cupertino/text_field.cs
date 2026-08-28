// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/text_field.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Cupertino;

public static partial class Text_fieldLibrary
{
    internal static global::Doroti.Framework.Painting.TextStyle _kDefaultPlaceholderStyle = new global::Doroti.Framework.Painting.TextStyle(fontWeight: FontWeight.w400, color: CupertinoColors.placeholderText);
}

public static partial class Text_fieldLibrary
{
    internal static global::Doroti.Framework.Painting.BorderSide _kDefaultRoundedBorderSide = new global::Doroti.Framework.Painting.BorderSide(color: new CupertinoDynamicColor(color: new global::Doroti.Ui.Color(855638016L), darkColor: new global::Doroti.Ui.Color(872415231L)), width: 0.0);
}

public static partial class Text_fieldLibrary
{
    internal static global::Doroti.Framework.Painting.Border _kDefaultRoundedBorder = new global::Doroti.Framework.Painting.Border(top: Text_fieldLibrary._kDefaultRoundedBorderSide, bottom: Text_fieldLibrary._kDefaultRoundedBorderSide, left: Text_fieldLibrary._kDefaultRoundedBorderSide, right: Text_fieldLibrary._kDefaultRoundedBorderSide);
}

public static partial class Text_fieldLibrary
{
    internal static global::Doroti.Framework.Painting.BoxDecoration _kDefaultRoundedBorderDecoration = new global::Doroti.Framework.Painting.BoxDecoration(color: new CupertinoDynamicColor(color: CupertinoColors.white, darkColor: CupertinoColors.black), border: Text_fieldLibrary._kDefaultRoundedBorder, borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(5.0)));
}

public static partial class Text_fieldLibrary
{
    internal static Color _kDisabledBackground = ((Color)(object?)new CupertinoDynamicColor(color: new global::Doroti.Ui.Color(4294638330L), darkColor: new global::Doroti.Ui.Color(4278519045L)));
}

public static partial class Text_fieldLibrary
{
    internal static CupertinoDynamicColor _kClearButtonColor = new CupertinoDynamicColor(color: new global::Doroti.Ui.Color(855638016L), darkColor: new global::Doroti.Ui.Color(872415231L));
}

public static partial class Text_fieldLibrary
{
    internal static long _iOSHorizontalCursorOffsetPixels = -2L;
}

public enum OverlayVisibilityMode
{
    never,
    editing,
    notEditing,
    always
}

internal class _CupertinoTextFieldSelectionGestureDetectorBuilder__text_field : global::Doroti.Framework.Widgets.TextSelectionGestureDetectorBuilder
{
    internal virtual _CupertinoTextFieldState__text_field _state { get; private set; } = default!;

    internal _CupertinoTextFieldSelectionGestureDetectorBuilder__text_field(_CupertinoTextFieldState__text_field state) : base(@delegate: state)
    {
        this._state = state;
    }

    public override void onSingleTapUp(global::Doroti.Framework.Gestures.TapDragUpDetails details)
    {
        if ((((_CupertinoTextFieldState__text_field)this._state)._clearGlobalKey.currentContext is not null))
        {
            var renderBox = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((_CupertinoTextFieldState__text_field)this._state)._clearGlobalKey.currentContext!.findRenderObject()!)!;
            global::Doroti.Ui.Offset localOffset = ((global::Doroti.Ui.Offset)(object?)((Offset)((dynamic)renderBox).globalToLocal(((global::Doroti.Framework.Gestures.TapDragUpDetails)details).globalPosition)));
            if (renderBox.hitTest(new global::Doroti.Framework.Rendering.BoxHitTestResult(), position: localOffset))
            {
                return;
            }
        }
        base.onSingleTapUp(details);
        this._state.widget.onTap?.Invoke();
    }

    public override void onDragSelectionEnd(global::Doroti.Framework.Gestures.TapDragEndDetails details)
    {
        this._state._requestKeyboard();
        base.onDragSelectionEnd(details);
    }

}

public class CupertinoTextField : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual object groupId { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.TextEditingController? controller { get; private set; }
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual global::Doroti.Framework.Painting.BoxDecoration? decoration { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry padding { get; private set; } = default!;
    public virtual string? placeholder { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? placeholderStyle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? prefix { get; private set; }
    public virtual OverlayVisibilityMode prefixMode { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? suffix { get; private set; }
    public virtual OverlayVisibilityMode suffixMode { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.CrossAxisAlignment crossAxisAlignment { get; private set; } = default!;
    public virtual OverlayVisibilityMode clearButtonMode { get; private set; } = default!;
    public virtual string? clearButtonSemanticLabel { get; private set; }
    public virtual global::Doroti.Framework.Services.TextInputType keyboardType { get; private set; } = default!;
    public virtual global::Doroti.Framework.Services.TextInputAction? textInputAction { get; private set; }
    public virtual global::Doroti.Framework.Services.TextCapitalization textCapitalization { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle? style { get; private set; }
    public virtual global::Doroti.Framework.Painting.StrutStyle? strutStyle { get; private set; }
    public virtual TextAlign textAlign { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.ToolbarOptions? toolbarOptions { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextAlignVertical? textAlignVertical { get; private set; }
    public virtual TextDirection? textDirection { get; private set; }
    public virtual bool readOnly { get; private set; } = default!;
    public virtual bool? showCursor { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual string obscuringCharacter { get; private set; } = default!;
    public virtual bool obscureText { get; private set; } = default!;
    public virtual bool? autocorrect { get; private set; }
    public virtual global::Doroti.Framework.Services.SmartDashesType smartDashesType { get; private set; } = default!;
    public virtual global::Doroti.Framework.Services.SmartQuotesType smartQuotesType { get; private set; } = default!;
    public virtual bool enableSuggestions { get; private set; } = default!;
    public virtual long? maxLines { get; private set; }
    public virtual long? minLines { get; private set; }
    public virtual bool expands { get; private set; } = default!;
    public virtual long? maxLength { get; private set; }
    public virtual global::Doroti.Framework.Services.MaxLengthEnforcement? maxLengthEnforcement { get; private set; }
    public virtual global::System.Action<string>? onChanged { get; private set; }
    public virtual global::System.Action? onEditingComplete { get; private set; }
    public virtual global::System.Action<string>? onSubmitted { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>? onTapOutside { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>? onTapUpOutside { get; private set; }
    public virtual List<global::Doroti.Framework.Services.TextInputFormatter>? inputFormatters { get; private set; }
    public virtual bool enabled { get; private set; } = default!;
    public virtual double cursorWidth { get; private set; } = default!;
    public virtual double? cursorHeight { get; private set; }
    public virtual Radius cursorRadius { get; private set; } = default!;
    public virtual bool cursorOpacityAnimates { get; private set; } = default!;
    public virtual Color? cursorColor { get; private set; }
    public virtual BoxHeightStyle? selectionHeightStyle { get; private set; }
    public virtual BoxWidthStyle? selectionWidthStyle { get; private set; }
    public virtual Brightness? keyboardAppearance { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsets scrollPadding { get; private set; } = default!;
    public virtual bool enableInteractiveSelection { get; private set; } = default!;
    public virtual bool? selectAllOnFocus { get; private set; }
    public virtual global::Doroti.Framework.Widgets.TextSelectionControls? selectionControls { get; private set; }
    public virtual global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.ScrollController? scrollController { get; private set; }
    public virtual global::Doroti.Framework.Widgets.ScrollPhysics? scrollPhysics { get; private set; }
    public virtual global::System.Action? onTap { get; private set; }
    public virtual IEnumerable<string>? autofillHints { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual string? restorationId { get; private set; }
    public virtual bool scribbleEnabled { get; private set; } = default!;
    public virtual bool stylusHandwritingEnabled { get; private set; } = default!;
    public virtual bool enableIMEPersonalizedLearning { get; private set; } = default!;
    public virtual bool? enableInlinePrediction { get; private set; }
    public virtual global::Doroti.Framework.Widgets.ContentInsertionConfiguration? contentInsertionConfiguration { get; private set; }
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget>? contextMenuBuilder { get; private set; }
    public virtual global::Doroti.Framework.Widgets.TextMagnifierConfiguration? magnifierConfiguration { get; private set; }
    public virtual global::Doroti.Framework.Widgets.SpellCheckConfiguration? spellCheckConfiguration { get; private set; }
    public static global::Doroti.Framework.Painting.TextStyle cupertinoMisspelledTextStyle = new global::Doroti.Framework.Painting.TextStyle(decoration: TextDecoration.underline, decorationColor: CupertinoColors.systemRed, decorationStyle: TextDecorationStyle.dotted);
    public static Color kMisspelledSelectionColor = new global::Doroti.Ui.Color(1660917401L);
    public virtual global::Doroti.Framework.Widgets.UndoHistoryController? undoController { get; private set; }
    internal static global::Doroti.Framework.Widgets.TextMagnifierConfiguration _iosMagnifierConfiguration = new global::Doroti.Framework.Widgets.TextMagnifierConfiguration(magnifierBuilder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.MagnifierController, global::Doroti.Framework.Foundation.ValueNotifier<global::Doroti.Framework.Widgets.MagnifierInfo>, global::Doroti.Framework.Widgets.Widget?>?)((context, controller, magnifierInfo) =>
    {
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                {
                    return ((global::Doroti.Framework.Widgets.Widget?)(object?)new CupertinoTextMagnifier(controller: controller, magnifierInfo: magnifierInfo));
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    return null;
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart closure completed without a value.");
    })));

    public CupertinoTextField(global::Doroti.Framework.Foundation.Key? key = null, object groupId = default!, global::Doroti.Framework.Widgets.TextEditingController? controller = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::Doroti.Framework.Widgets.UndoHistoryController? undoController = null, global::Doroti.Framework.Painting.BoxDecoration? decoration = default!, global::Doroti.Framework.Painting.EdgeInsetsGeometry padding = default!, string? placeholder = null, global::Doroti.Framework.Painting.TextStyle? placeholderStyle = default!, global::Doroti.Framework.Widgets.Widget? prefix = null, OverlayVisibilityMode prefixMode = OverlayVisibilityMode.always, global::Doroti.Framework.Widgets.Widget? suffix = null, OverlayVisibilityMode suffixMode = OverlayVisibilityMode.always, global::Doroti.Framework.Rendering.CrossAxisAlignment crossAxisAlignment = global::Doroti.Framework.Rendering.CrossAxisAlignment.center, OverlayVisibilityMode clearButtonMode = OverlayVisibilityMode.never, string? clearButtonSemanticLabel = null, global::Doroti.Framework.Services.TextInputType? keyboardType = null, global::Doroti.Framework.Services.TextInputAction? textInputAction = null, global::Doroti.Framework.Services.TextCapitalization textCapitalization = global::Doroti.Framework.Services.TextCapitalization.none, global::Doroti.Framework.Painting.TextStyle? style = null, global::Doroti.Framework.Painting.StrutStyle? strutStyle = null, TextAlign textAlign = TextAlign.start, global::Doroti.Framework.Painting.TextAlignVertical? textAlignVertical = null, TextDirection? textDirection = null, bool readOnly = false, global::Doroti.Framework.Widgets.ToolbarOptions? toolbarOptions = null, bool? showCursor = null, bool autofocus = false, string obscuringCharacter = "•", bool obscureText = false, bool? autocorrect = true, global::Doroti.Framework.Services.SmartDashesType? smartDashesType = null, global::Doroti.Framework.Services.SmartQuotesType? smartQuotesType = null, bool enableSuggestions = true, long? maxLines = 1, long? minLines = null, bool expands = false, long? maxLength = null, global::Doroti.Framework.Services.MaxLengthEnforcement? maxLengthEnforcement = null, global::System.Action<string>? onChanged = null, global::System.Action? onEditingComplete = null, global::System.Action<string>? onSubmitted = null, global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>? onTapOutside = null, global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>? onTapUpOutside = null, List<global::Doroti.Framework.Services.TextInputFormatter>? inputFormatters = null, bool enabled = true, double cursorWidth = 2.0, double? cursorHeight = null, Radius? cursorRadius = null, bool cursorOpacityAnimates = true, Color? cursorColor = null, BoxHeightStyle? selectionHeightStyle = null, BoxWidthStyle? selectionWidthStyle = null, Brightness? keyboardAppearance = null, global::Doroti.Framework.Painting.EdgeInsets scrollPadding = default!, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.start, bool? enableInteractiveSelection = null, bool? selectAllOnFocus = null, global::Doroti.Framework.Widgets.TextSelectionControls? selectionControls = null, global::System.Action? onTap = null, global::Doroti.Framework.Widgets.ScrollController? scrollController = null, global::Doroti.Framework.Widgets.ScrollPhysics? scrollPhysics = null, IEnumerable<string>? autofillHints = default!, global::Doroti.Framework.Widgets.ContentInsertionConfiguration? contentInsertionConfiguration = null, Clip clipBehavior = Clip.hardEdge, string? restorationId = null, bool scribbleEnabled = true, bool? stylusHandwritingEnabled = null, bool enableIMEPersonalizedLearning = true, bool? enableInlinePrediction = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget>? contextMenuBuilder = default!, global::Doroti.Framework.Widgets.SpellCheckConfiguration? spellCheckConfiguration = null, global::Doroti.Framework.Widgets.TextMagnifierConfiguration? magnifierConfiguration = null) : base(key: key)
    {
        object __groupId = groupId ?? typeof(global::Doroti.Framework.Widgets.EditableText);
        global::Doroti.Framework.Painting.BoxDecoration? __decoration = decoration ?? Text_fieldLibrary._kDefaultRoundedBorderDecoration;
        global::Doroti.Framework.Painting.EdgeInsetsGeometry __padding = padding ?? global::Doroti.Framework.Painting.EdgeInsets.CreateAll(7.0);
        global::Doroti.Framework.Painting.TextStyle? __placeholderStyle = placeholderStyle ?? new global::Doroti.Framework.Painting.TextStyle(fontWeight: FontWeight.w400, color: CupertinoColors.placeholderText);
        Radius __cursorRadius = cursorRadius ?? Radius.CreateCircular(2.0);
        global::Doroti.Framework.Painting.EdgeInsets __scrollPadding = scrollPadding ?? global::Doroti.Framework.Painting.EdgeInsets.CreateAll(20.0);
        IEnumerable<string>? __autofillHints = autofillHints ?? new List<string>();
        bool __stylusHandwritingEnabled = stylusHandwritingEnabled ?? global::Doroti.Framework.Widgets.EditableText.defaultStylusHandwritingEnabled;
        global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget>? __contextMenuBuilder = contextMenuBuilder ?? _defaultContextMenuBuilder;
        this.groupId = __groupId;
        this.controller = controller;
        this.focusNode = focusNode;
        this.undoController = undoController;
        this.decoration = __decoration;
        this.padding = __padding;
        this.placeholder = placeholder;
        this.placeholderStyle = __placeholderStyle;
        this.prefix = prefix;
        this.prefixMode = prefixMode;
        this.suffix = suffix;
        this.suffixMode = suffixMode;
        this.crossAxisAlignment = crossAxisAlignment;
        this.clearButtonMode = clearButtonMode;
        this.clearButtonSemanticLabel = clearButtonSemanticLabel;
        this.textInputAction = textInputAction;
        this.textCapitalization = textCapitalization;
        this.style = style;
        this.strutStyle = strutStyle;
        this.textAlign = textAlign;
        this.textAlignVertical = textAlignVertical;
        this.textDirection = textDirection;
        this.readOnly = readOnly;
        this.toolbarOptions = toolbarOptions;
        this.showCursor = showCursor;
        this.autofocus = autofocus;
        this.obscuringCharacter = obscuringCharacter;
        this.obscureText = obscureText;
        this.autocorrect = autocorrect;
        this.enableSuggestions = enableSuggestions;
        this.maxLines = maxLines;
        this.minLines = minLines;
        this.expands = expands;
        this.maxLength = maxLength;
        this.maxLengthEnforcement = maxLengthEnforcement;
        this.onChanged = onChanged;
        this.onEditingComplete = onEditingComplete;
        this.onSubmitted = onSubmitted;
        this.onTapOutside = onTapOutside;
        this.onTapUpOutside = onTapUpOutside;
        this.inputFormatters = inputFormatters;
        this.enabled = enabled;
        this.cursorWidth = cursorWidth;
        this.cursorHeight = cursorHeight;
        this.cursorRadius = __cursorRadius;
        this.cursorOpacityAnimates = cursorOpacityAnimates;
        this.cursorColor = cursorColor;
        this.selectionHeightStyle = selectionHeightStyle;
        this.selectionWidthStyle = selectionWidthStyle;
        this.keyboardAppearance = keyboardAppearance;
        this.scrollPadding = __scrollPadding;
        this.dragStartBehavior = dragStartBehavior;
        this.selectAllOnFocus = selectAllOnFocus;
        this.selectionControls = selectionControls;
        this.onTap = onTap;
        this.scrollController = scrollController;
        this.scrollPhysics = scrollPhysics;
        this.autofillHints = __autofillHints;
        this.contentInsertionConfiguration = contentInsertionConfiguration;
        this.clipBehavior = clipBehavior;
        this.restorationId = restorationId;
        this.scribbleEnabled = scribbleEnabled;
        this.stylusHandwritingEnabled = __stylusHandwritingEnabled;
        this.enableIMEPersonalizedLearning = enableIMEPersonalizedLearning;
        this.enableInlinePrediction = enableInlinePrediction;
        this.contextMenuBuilder = __contextMenuBuilder;
        this.spellCheckConfiguration = spellCheckConfiguration;
        this.magnifierConfiguration = magnifierConfiguration;
        this.smartDashesType = (smartDashesType ?? ((obscureText ? global::Doroti.Framework.Services.SmartDashesType.disabled : global::Doroti.Framework.Services.SmartDashesType.enabled)));
        this.smartQuotesType = (smartQuotesType ?? ((obscureText ? global::Doroti.Framework.Services.SmartQuotesType.disabled : global::Doroti.Framework.Services.SmartQuotesType.enabled)));
        this.keyboardType = (keyboardType ?? (((maxLines == 1L) ? global::Doroti.Framework.Services.TextInputType.text : global::Doroti.Framework.Services.TextInputType.multiline)));
        this.enableInteractiveSelection = (enableInteractiveSelection ?? ((!readOnly || !obscureText)));
        System.Diagnostics.Debug.Assert((obscuringCharacter.Length == 1L));
        System.Diagnostics.Debug.Assert(((maxLines is null) || (DartRuntimePrimitives.RequireValue(maxLines) > 0L)));
        System.Diagnostics.Debug.Assert(((minLines is null) || (DartRuntimePrimitives.RequireValue(minLines) > 0L)));
        System.Diagnostics.Debug.Assert(((((maxLines is null)) || ((minLines is null))) || ((maxLines >= DartRuntimePrimitives.RequireValue(minLines)))));
        System.Diagnostics.Debug.Assert((!expands || (((maxLines is null) && (minLines is null)))));
        System.Diagnostics.Debug.Assert((!obscureText || (maxLines == 1L)));
        System.Diagnostics.Debug.Assert(((maxLength is null) || (DartRuntimePrimitives.RequireValue(maxLength) > 0L)));
        System.Diagnostics.Debug.Assert(((!DartRuntimePrimitives.Identical(textInputAction, global::Doroti.Framework.Services.TextInputAction.newline) || (maxLines == 1L)) || !DartRuntimePrimitives.Identical(keyboardType, global::Doroti.Framework.Services.TextInputType.text)));
    }

    public static CupertinoTextField CreateBorderless(global::Doroti.Framework.Foundation.Key? key = null, object groupId = default!, global::Doroti.Framework.Widgets.TextEditingController? controller = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::Doroti.Framework.Widgets.UndoHistoryController? undoController = null, global::Doroti.Framework.Painting.BoxDecoration? decoration = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry padding = default!, string? placeholder = null, global::Doroti.Framework.Painting.TextStyle? placeholderStyle = default!, global::Doroti.Framework.Widgets.Widget? prefix = null, OverlayVisibilityMode prefixMode = OverlayVisibilityMode.always, global::Doroti.Framework.Widgets.Widget? suffix = null, OverlayVisibilityMode suffixMode = OverlayVisibilityMode.always, global::Doroti.Framework.Rendering.CrossAxisAlignment crossAxisAlignment = global::Doroti.Framework.Rendering.CrossAxisAlignment.center, OverlayVisibilityMode clearButtonMode = OverlayVisibilityMode.never, string? clearButtonSemanticLabel = null, global::Doroti.Framework.Services.TextInputType? keyboardType = null, global::Doroti.Framework.Services.TextInputAction? textInputAction = null, global::Doroti.Framework.Services.TextCapitalization textCapitalization = global::Doroti.Framework.Services.TextCapitalization.none, global::Doroti.Framework.Painting.TextStyle? style = null, global::Doroti.Framework.Painting.StrutStyle? strutStyle = null, TextAlign textAlign = TextAlign.start, global::Doroti.Framework.Painting.TextAlignVertical? textAlignVertical = null, TextDirection? textDirection = null, bool readOnly = false, global::Doroti.Framework.Widgets.ToolbarOptions? toolbarOptions = null, bool? showCursor = null, bool autofocus = false, string obscuringCharacter = "•", bool obscureText = false, bool? autocorrect = null, global::Doroti.Framework.Services.SmartDashesType? smartDashesType = null, global::Doroti.Framework.Services.SmartQuotesType? smartQuotesType = null, bool enableSuggestions = true, long? maxLines = 1, long? minLines = null, bool expands = false, long? maxLength = null, global::Doroti.Framework.Services.MaxLengthEnforcement? maxLengthEnforcement = null, global::System.Action<string>? onChanged = null, global::System.Action? onEditingComplete = null, global::System.Action<string>? onSubmitted = null, global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>? onTapOutside = null, global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>? onTapUpOutside = null, List<global::Doroti.Framework.Services.TextInputFormatter>? inputFormatters = null, bool enabled = true, double cursorWidth = 2.0, double? cursorHeight = null, Radius? cursorRadius = null, bool cursorOpacityAnimates = true, Color? cursorColor = null, BoxHeightStyle? selectionHeightStyle = null, BoxWidthStyle? selectionWidthStyle = null, Brightness? keyboardAppearance = null, global::Doroti.Framework.Painting.EdgeInsets scrollPadding = default!, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.start, bool? enableInteractiveSelection = null, bool? selectAllOnFocus = null, global::Doroti.Framework.Widgets.TextSelectionControls? selectionControls = null, global::System.Action? onTap = null, global::Doroti.Framework.Widgets.ScrollController? scrollController = null, global::Doroti.Framework.Widgets.ScrollPhysics? scrollPhysics = null, IEnumerable<string>? autofillHints = default!, global::Doroti.Framework.Widgets.ContentInsertionConfiguration? contentInsertionConfiguration = null, Clip clipBehavior = Clip.hardEdge, string? restorationId = null, bool scribbleEnabled = true, bool stylusHandwritingEnabled = true, bool enableIMEPersonalizedLearning = true, bool? enableInlinePrediction = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget>? contextMenuBuilder = default!, global::Doroti.Framework.Widgets.SpellCheckConfiguration? spellCheckConfiguration = null, global::Doroti.Framework.Widgets.TextMagnifierConfiguration? magnifierConfiguration = null)
    {
        var __instance = new CupertinoTextField(key: key, groupId: groupId, controller: controller, focusNode: focusNode, undoController: undoController, decoration: decoration, padding: padding, placeholder: placeholder, placeholderStyle: placeholderStyle, prefix: prefix, prefixMode: prefixMode, suffix: suffix, suffixMode: suffixMode, crossAxisAlignment: crossAxisAlignment, clearButtonMode: clearButtonMode, clearButtonSemanticLabel: clearButtonSemanticLabel, keyboardType: keyboardType, textInputAction: textInputAction, textCapitalization: textCapitalization, style: style, strutStyle: strutStyle, textAlign: textAlign, textAlignVertical: textAlignVertical, textDirection: textDirection, readOnly: readOnly, toolbarOptions: toolbarOptions, showCursor: showCursor, autofocus: autofocus, obscuringCharacter: obscuringCharacter, obscureText: obscureText, autocorrect: autocorrect, smartDashesType: smartDashesType, smartQuotesType: smartQuotesType, enableSuggestions: enableSuggestions, maxLines: maxLines, minLines: minLines, expands: expands, maxLength: maxLength, maxLengthEnforcement: maxLengthEnforcement, onChanged: onChanged, onEditingComplete: onEditingComplete, onSubmitted: onSubmitted, onTapOutside: onTapOutside, onTapUpOutside: onTapUpOutside, inputFormatters: inputFormatters, enabled: enabled, cursorWidth: cursorWidth, cursorHeight: cursorHeight, cursorRadius: cursorRadius, cursorOpacityAnimates: cursorOpacityAnimates, cursorColor: cursorColor, selectionHeightStyle: selectionHeightStyle, selectionWidthStyle: selectionWidthStyle, keyboardAppearance: keyboardAppearance, scrollPadding: scrollPadding, dragStartBehavior: dragStartBehavior, enableInteractiveSelection: enableInteractiveSelection, selectAllOnFocus: selectAllOnFocus, selectionControls: selectionControls, onTap: onTap, scrollController: scrollController, scrollPhysics: scrollPhysics, autofillHints: autofillHints, contentInsertionConfiguration: contentInsertionConfiguration, clipBehavior: clipBehavior, restorationId: restorationId, scribbleEnabled: scribbleEnabled, stylusHandwritingEnabled: stylusHandwritingEnabled, enableIMEPersonalizedLearning: enableIMEPersonalizedLearning, enableInlinePrediction: enableInlinePrediction, contextMenuBuilder: contextMenuBuilder, spellCheckConfiguration: spellCheckConfiguration, magnifierConfiguration: magnifierConfiguration);
        object __groupId = groupId ?? typeof(global::Doroti.Framework.Widgets.EditableText);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry __padding = padding ?? global::Doroti.Framework.Painting.EdgeInsets.CreateAll(7.0);
        global::Doroti.Framework.Painting.TextStyle? __placeholderStyle = placeholderStyle ?? Text_fieldLibrary._kDefaultPlaceholderStyle;
        Radius __cursorRadius = cursorRadius ?? Radius.CreateCircular(2.0);
        global::Doroti.Framework.Painting.EdgeInsets __scrollPadding = scrollPadding ?? global::Doroti.Framework.Painting.EdgeInsets.CreateAll(20.0);
        IEnumerable<string>? __autofillHints = autofillHints ?? new List<string>();
        global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget>? __contextMenuBuilder = contextMenuBuilder ?? _defaultContextMenuBuilder;
        __instance.groupId = __groupId;
        __instance.controller = controller;
        __instance.focusNode = focusNode;
        __instance.undoController = undoController;
        __instance.decoration = decoration;
        __instance.padding = __padding;
        __instance.placeholder = placeholder;
        __instance.placeholderStyle = __placeholderStyle;
        __instance.prefix = prefix;
        __instance.prefixMode = prefixMode;
        __instance.suffix = suffix;
        __instance.suffixMode = suffixMode;
        __instance.crossAxisAlignment = crossAxisAlignment;
        __instance.clearButtonMode = clearButtonMode;
        __instance.clearButtonSemanticLabel = clearButtonSemanticLabel;
        __instance.textInputAction = textInputAction;
        __instance.textCapitalization = textCapitalization;
        __instance.style = style;
        __instance.strutStyle = strutStyle;
        __instance.textAlign = textAlign;
        __instance.textAlignVertical = textAlignVertical;
        __instance.textDirection = textDirection;
        __instance.readOnly = readOnly;
        __instance.toolbarOptions = toolbarOptions;
        __instance.showCursor = showCursor;
        __instance.autofocus = autofocus;
        __instance.obscuringCharacter = obscuringCharacter;
        __instance.obscureText = obscureText;
        __instance.autocorrect = autocorrect;
        __instance.enableSuggestions = enableSuggestions;
        __instance.maxLines = maxLines;
        __instance.minLines = minLines;
        __instance.expands = expands;
        __instance.maxLength = maxLength;
        __instance.maxLengthEnforcement = maxLengthEnforcement;
        __instance.onChanged = onChanged;
        __instance.onEditingComplete = onEditingComplete;
        __instance.onSubmitted = onSubmitted;
        __instance.onTapOutside = onTapOutside;
        __instance.onTapUpOutside = onTapUpOutside;
        __instance.inputFormatters = inputFormatters;
        __instance.enabled = enabled;
        __instance.cursorWidth = cursorWidth;
        __instance.cursorHeight = cursorHeight;
        __instance.cursorRadius = __cursorRadius;
        __instance.cursorOpacityAnimates = cursorOpacityAnimates;
        __instance.cursorColor = cursorColor;
        __instance.selectionHeightStyle = selectionHeightStyle;
        __instance.selectionWidthStyle = selectionWidthStyle;
        __instance.keyboardAppearance = keyboardAppearance;
        __instance.scrollPadding = __scrollPadding;
        __instance.dragStartBehavior = dragStartBehavior;
        __instance.selectAllOnFocus = selectAllOnFocus;
        __instance.selectionControls = selectionControls;
        __instance.onTap = onTap;
        __instance.scrollController = scrollController;
        __instance.scrollPhysics = scrollPhysics;
        __instance.autofillHints = __autofillHints;
        __instance.contentInsertionConfiguration = contentInsertionConfiguration;
        __instance.clipBehavior = clipBehavior;
        __instance.restorationId = restorationId;
        __instance.scribbleEnabled = scribbleEnabled;
        __instance.stylusHandwritingEnabled = stylusHandwritingEnabled;
        __instance.enableIMEPersonalizedLearning = enableIMEPersonalizedLearning;
        __instance.enableInlinePrediction = enableInlinePrediction;
        __instance.contextMenuBuilder = __contextMenuBuilder;
        __instance.spellCheckConfiguration = spellCheckConfiguration;
        __instance.magnifierConfiguration = magnifierConfiguration;
        __instance.smartDashesType = (smartDashesType ?? ((obscureText ? global::Doroti.Framework.Services.SmartDashesType.disabled : global::Doroti.Framework.Services.SmartDashesType.enabled)));
        __instance.smartQuotesType = (smartQuotesType ?? ((obscureText ? global::Doroti.Framework.Services.SmartQuotesType.disabled : global::Doroti.Framework.Services.SmartQuotesType.enabled)));
        __instance.keyboardType = (keyboardType ?? (((maxLines == 1L) ? global::Doroti.Framework.Services.TextInputType.text : global::Doroti.Framework.Services.TextInputType.multiline)));
        __instance.enableInteractiveSelection = (enableInteractiveSelection ?? ((!readOnly || !obscureText)));
        return __instance;
    }

    public virtual bool selectionEnabled => this.enableInteractiveSelection;
    internal static global::Doroti.Framework.Widgets.Widget _defaultContextMenuBuilder(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.EditableTextState editableTextState)
    {
        if (SystemContextMenu.isSupportedByField(editableTextState))
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.SystemContextMenu.CreateEditableText(editableTextState: editableTextState));
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)CupertinoAdaptiveTextSelectionToolbar.CreateEditableText(editableTextState: editableTextState));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Framework.Widgets.Widget defaultSpellCheckSuggestionsToolbarBuilder(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.EditableTextState editableTextState)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)CupertinoSpellCheckSuggestionsToolbar.CreateEditableText(editableTextState: editableTextState));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoTextFieldState__text_field());
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.TextEditingController>("controller", this.controller, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.FocusNode>("focusNode", this.focusNode, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.UndoHistoryController>("undoController", this.undoController, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.BoxDecoration>("decoration", this.decoration));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("padding", this.padding));
        properties.add(new global::Doroti.Framework.Foundation.StringProperty("placeholder", this.placeholder));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("placeholderStyle", this.placeholderStyle));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<OverlayVisibilityMode>("prefix", ((this.prefix is null) ? null : this.prefixMode)));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<OverlayVisibilityMode>("suffix", ((this.suffix is null) ? null : this.suffixMode)));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<OverlayVisibilityMode>("clearButtonMode", this.clearButtonMode));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<string>("clearButtonSemanticLabel", this.clearButtonSemanticLabel));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Services.TextInputType>("keyboardType", this.keyboardType, defaultValue: global::Doroti.Framework.Services.TextInputType.text));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("style", this.style, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("autofocus", this.autofocus, defaultValue: false));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<string>("obscuringCharacter", this.obscuringCharacter, defaultValue: "•"));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("obscureText", this.obscureText, defaultValue: false));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("autocorrect", this.autocorrect, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Services.SmartDashesType>("smartDashesType", this.smartDashesType, defaultValue: (this.obscureText ? global::Doroti.Framework.Services.SmartDashesType.disabled : global::Doroti.Framework.Services.SmartDashesType.enabled)));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Services.SmartQuotesType>("smartQuotesType", this.smartQuotesType, defaultValue: (this.obscureText ? global::Doroti.Framework.Services.SmartQuotesType.disabled : global::Doroti.Framework.Services.SmartQuotesType.enabled)));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("enableSuggestions", this.enableSuggestions, defaultValue: true));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("maxLines", this.maxLines, defaultValue: 1L));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("minLines", this.minLines, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("expands", this.expands, defaultValue: false));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("maxLength", this.maxLength, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Services.MaxLengthEnforcement>("maxLengthEnforcement", this.maxLengthEnforcement, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("cursorWidth", this.cursorWidth, defaultValue: 2.0));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("cursorHeight", this.cursorHeight, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Radius>("cursorRadius", DartRuntimePrimitives.RequireValue(this.cursorRadius), defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("cursorOpacityAnimates", this.cursorOpacityAnimates, defaultValue: true));
        properties.add(ColorsLibrary.createCupertinoColorProperty("cursorColor", this.cursorColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("selectionEnabled", value: this.selectionEnabled, defaultValue: true, ifFalse: "selection disabled"));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.TextSelectionControls>("selectionControls", this.selectionControls, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.ScrollController>("scrollController", this.scrollController, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.ScrollPhysics>("scrollPhysics", this.scrollPhysics, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextAlign>("textAlign", this.textAlign, defaultValue: global::Doroti.Ui.TextAlign.start));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextAlignVertical>("textAlignVertical", this.textAlignVertical, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", this.textDirection, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Clip>("clipBehavior", this.clipBehavior, defaultValue: Clip.hardEdge));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("scribbleEnabled", this.scribbleEnabled, defaultValue: true));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("stylusHandwritingEnabled", DartRuntimePrimitives.RequireValue(this.stylusHandwritingEnabled), defaultValue: global::Doroti.Framework.Widgets.EditableText.defaultStylusHandwritingEnabled));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("enableIMEPersonalizedLearning", this.enableIMEPersonalizedLearning, defaultValue: true));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool?>("enableInlinePrediction", this.enableInlinePrediction, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.SpellCheckConfiguration>("spellCheckConfiguration", this.spellCheckConfiguration, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<List<string>>("contentCommitMimeTypes", (this.contentInsertionConfiguration?.allowedMimeTypes ?? new List<string>()), defaultValue: ((this.contentInsertionConfiguration is null) ? new List<string>() : global::Doroti.Framework.Widgets.Editable_textLibrary.kDefaultContentInsertionMimeTypes)));
    }

    public static global::Doroti.Framework.Widgets.SpellCheckConfiguration inferIOSSpellCheckConfiguration(global::Doroti.Framework.Widgets.SpellCheckConfiguration? configuration)
    {
        if (((configuration is null) || (object.Equals(configuration, global::Doroti.Framework.Widgets.SpellCheckConfiguration.CreateDisabled()))))
        {
            return global::Doroti.Framework.Widgets.SpellCheckConfiguration.CreateDisabled();
        }
        return ((global::Doroti.Framework.Widgets.SpellCheckConfiguration)(object?)configuration.copyWith(misspelledTextStyle: (((global::Doroti.Framework.Widgets.SpellCheckConfiguration)configuration).misspelledTextStyle ?? CupertinoTextField.cupertinoMisspelledTextStyle), misspelledSelectionColor: (((global::Doroti.Framework.Widgets.SpellCheckConfiguration)configuration).misspelledSelectionColor ?? CupertinoTextField.kMisspelledSelectionColor), spellCheckSuggestionsToolbarBuilder: ((((global::Doroti.Framework.Widgets.SpellCheckConfiguration)configuration).spellCheckSuggestionsToolbarBuilder ?? (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget>)CupertinoTextField.defaultSpellCheckSuggestionsToolbarBuilder))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CupertinoTextFieldState__text_field : global::Doroti.Framework.Widgets.State<CupertinoTextField>, global::Doroti.Framework.Widgets.RestorationMixin<CupertinoTextField>, global::Doroti.Framework.Widgets.AutomaticKeepAliveClientMixin<CupertinoTextField>, global::Doroti.Framework.Widgets.TextSelectionGestureDetectorBuilderDelegate, global::Doroti.Framework.Services.AutofillClient
{
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _clearGlobalKey { get; private set; } = global::Doroti.Framework.Widgets.GlobalKey<IState>.Create();
    internal virtual global::Doroti.Framework.Widgets.RestorableTextEditingController? _controller { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.FocusNode? _focusNode { get; set; } = default;
    internal virtual bool _showSelectionHandles { get; set; } = false;
    internal virtual _CupertinoTextFieldSelectionGestureDetectorBuilder__text_field _selectionGestureDetectorBuilder { get; set; } = default!;
    public virtual global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.EditableTextState> editableTextKey { get; private set; } = global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.EditableTextState>.Create();
    public virtual global::Doroti.Framework.Services.RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<dynamic, global::System.Action> _properties { get; set; } = new DartMap<dynamic, global::System.Action>();
    public virtual List<dynamic>? _debugPropertiesWaitingForReregistration { get; set; } = default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual global::Doroti.Framework.Services.RestorationBucket? _currentParent { get; set; } = default;
    public virtual KeepAliveHandle? _keepAliveHandle { get; set; } = default;

    internal virtual global::Doroti.Framework.Widgets.TextEditingController _effectiveController => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.TextEditingController>((((CupertinoTextField)this.widget).controller ?? this._controller!.value));
    internal virtual global::Doroti.Framework.Widgets.FocusNode _effectiveFocusNode => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.FocusNode>((((CupertinoTextField)this.widget).focusNode ?? (_focusNode ??= new global::Doroti.Framework.Widgets.FocusNode())));
    internal virtual global::Doroti.Framework.Services.MaxLengthEnforcement _effectiveMaxLengthEnforcement => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Services.MaxLengthEnforcement>(((((CupertinoTextField)this.widget).maxLengthEnforcement ?? (global::Doroti.Framework.Services.MaxLengthEnforcement)LengthLimitingTextInputFormatter.getDefaultMaxLengthEnforcement())));
    public virtual bool forcePressEnabled => true;
    public virtual bool selectionEnabled => ((CupertinoTextField)this.widget).selectionEnabled;
    public override void initState()
    {
        base.initState();
        if (this.wantKeepAlive)
        {
            _ensureKeepAlive();
        }
        _selectionGestureDetectorBuilder = new _CupertinoTextFieldSelectionGestureDetectorBuilder__text_field(state: this);
        if ((((CupertinoTextField)this.widget).controller is null))
        {
            _createLocalController();
        }
        this._effectiveFocusNode.canRequestFocus = ((CupertinoTextField)this.widget).enabled;
        this._effectiveFocusNode.addListener(this._handleFocusChanged);
    }

    public override void didUpdateWidget(CupertinoTextField oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        didUpdateRestorationId();
        if (((((CupertinoTextField)this.widget).controller is null) && (((CupertinoTextField)oldWidget).controller is not null)))
        {
            _createLocalController(((CupertinoTextField)oldWidget).controller!.value);
        }
        else
        {
            if (((((CupertinoTextField)this.widget).controller is not null) && (((CupertinoTextField)oldWidget).controller is null)))
            {
                unregisterFromRestoration(this._controller!);
                this._controller!.dispose();
                _controller = null;
            }
        }
        if ((!object.Equals(((CupertinoTextField)this.widget).focusNode, ((CupertinoTextField)oldWidget).focusNode)))
        {
            ((((CupertinoTextField)oldWidget).focusNode ?? this._focusNode))?.removeListener(this._handleFocusChanged);
            ((((CupertinoTextField)this.widget).focusNode ?? this._focusNode))?.addListener(this._handleFocusChanged);
        }
        this._effectiveFocusNode.canRequestFocus = ((CupertinoTextField)this.widget).enabled;
    }

    public virtual void restoreState(global::Doroti.Framework.Services.RestorationBucket? oldBucket, bool initialRestore)
    {
        if ((this._controller is not null))
        {
            _registerController();
        }
    }

    internal virtual void _registerController()
    {
        DartRuntimePrimitives.Assert(() => (this._controller is not null));
        registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this._controller!), "controller");
        this._controller!.value.addListener(this.updateKeepAlive);
    }

    internal virtual void _createLocalController(global::Doroti.Framework.Services.TextEditingValue? value = null)
    {
        DartRuntimePrimitives.Assert(() => (this._controller is null));
        _controller = ((value is null) ? global::Doroti.Framework.Widgets.RestorableTextEditingController.Create() : new global::Doroti.Framework.Widgets.RestorableTextEditingController(value));
        if (!this.restorePending)
        {
            _registerController();
        }
    }

    public virtual string? restorationId => ((CupertinoTextField)this.widget).restorationId;
    public override void dispose()
    {
        this._effectiveFocusNode.removeListener(this._handleFocusChanged);
        this._focusNode?.dispose();
        this._controller?.dispose();
        this._properties.forEach(((global::System.Action<dynamic, global::System.Action>)((property, listener) =>
        {
            if (!((dynamic)property)._disposed)
            {
                property.removeListener(listener);
            }
        })));
        this._bucket?.dispose();
        _bucket = null;
        base.dispose();
    }

    internal virtual global::Doroti.Framework.Widgets.EditableTextState _editableText => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.EditableTextState>(((global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.EditableTextState>)this.editableTextKey).currentState!);
    internal virtual void _requestKeyboard()
    {
        this._editableText.requestKeyboard();
    }

    internal virtual void _handleFocusChanged()
    {
        setState(((global::System.Action)(() =>
        {
        })));
    }

    internal virtual bool _shouldShowSelectionHandles(global::Doroti.Framework.Services.SelectionChangedCause? cause)
    {
        if ((!this._selectionGestureDetectorBuilder.shouldShowSelectionToolbar || !this._selectionGestureDetectorBuilder.shouldShowSelectionHandles))
        {
            return false;
        }
        if (((global::Doroti.Framework.Widgets.TextEditingController)this._effectiveController).selection.isCollapsed)
        {
            return false;
        }
        if ((object.Equals(cause, global::Doroti.Framework.Services.SelectionChangedCause.keyboard)))
        {
            return false;
        }
        if ((object.Equals(cause, global::Doroti.Framework.Services.SelectionChangedCause.stylusHandwriting)))
        {
            return true;
        }
        if ((((global::Doroti.Framework.Widgets.TextEditingController)this._effectiveController).text.Length != 0))
        {
            return true;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleSelectionChanged(global::Doroti.Framework.Services.TextSelection selection, global::Doroti.Framework.Services.SelectionChangedCause? cause)
    {
        bool willShowSelectionHandles = _shouldShowSelectionHandles(cause);
        if ((willShowSelectionHandles != this._showSelectionHandles))
        {
            setState(((global::System.Action)(() =>
            {
                _showSelectionHandles = willShowSelectionHandles;
            })));
        }
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
                {
                    if ((object.Equals(cause, global::Doroti.Framework.Services.SelectionChangedCause.longPress)))
                    {
                        this._editableText.bringIntoView(((global::Doroti.Framework.Services.TextSelection)selection).extent);
                    }
                    break;
                }
        }
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
                {
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    if ((object.Equals(cause, global::Doroti.Framework.Services.SelectionChangedCause.drag)))
                    {
                        this._editableText.hideToolbar();
                    }
                    break;
                }
        }
    }

    public virtual bool wantKeepAlive => DartRuntimePrimitives.ConvertValue<bool>((this._controller is not null && this._controller.value.text.Length != 0));
    internal static bool _shouldShowAttachment(OverlayVisibilityMode attachment, bool hasText)
    {
        return (attachment switch { OverlayVisibilityMode.never => false, OverlayVisibilityMode.always => true, OverlayVisibilityMode.editing => hasText, OverlayVisibilityMode.notEditing => !hasText, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _hasDecoration
    {
        get
        {
            return ((((((CupertinoTextField)this.widget).placeholder is not null) || (!object.Equals(((CupertinoTextField)this.widget).clearButtonMode, OverlayVisibilityMode.never))) || (((CupertinoTextField)this.widget).prefix is not null)) || (((CupertinoTextField)this.widget).suffix is not null));
            return default!;
        }
    }
    internal virtual global::Doroti.Framework.Painting.TextAlignVertical _textAlignVertical
    {
        get
        {
            if ((((CupertinoTextField)this.widget).textAlignVertical is not null))
            {
                return ((CupertinoTextField)this.widget).textAlignVertical!;
            }
            return (this._hasDecoration ? global::Doroti.Framework.Painting.TextAlignVertical.center : global::Doroti.Framework.Painting.TextAlignVertical.top);
            return default!;
        }
    }
    internal virtual void _onClearButtonTapped()
    {
        bool hadText = (((global::Doroti.Framework.Widgets.TextEditingController)this._effectiveController).text.Length != 0);
        this._effectiveController.clear();
        if (hadText)
        {
            ((CupertinoTextField)this.widget).onChanged?.Invoke(((global::Doroti.Framework.Widgets.TextEditingController)this._effectiveController).text);
        }
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildClearButton()
    {
        string clearLabel = (((CupertinoTextField)this.widget).clearButtonSemanticLabel ?? CupertinoLocalizations.of(this.context).clearButtonLabel);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(button: true, label: clearLabel, child: new global::Doroti.Framework.Widgets.GestureDetector(key: this._clearGlobalKey, onTap: ((global::System.Action)(((CupertinoTextField)this.widget).enabled ? this._onClearButtonTapped : null)), child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 6.0), child: new global::Doroti.Framework.Widgets.Icon(CupertinoIcons.clear_thick_circled, size: 18.0, color: CupertinoDynamicColor.resolve(Text_fieldLibrary._kClearButtonColor, this.context))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _addTextDependentAttachments(global::Doroti.Framework.Widgets.Widget editableText, global::Doroti.Framework.Painting.TextStyle textStyle, global::Doroti.Framework.Painting.TextStyle placeholderStyle)
    {
        if (!this._hasDecoration)
        {
            return editableText;
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.ValueListenableBuilder<global::Doroti.Framework.Services.TextEditingValue>(valueListenable: this._effectiveController, child: editableText, builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Services.TextEditingValue, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Row>)((context, text, child) =>
        {
            bool hasTextLocal = (((global::Doroti.Framework.Services.TextEditingValue)text).text.Length != 0);
            string? placeholderText = ((CupertinoTextField)this.widget).placeholder;
            global::Doroti.Framework.Widgets.Widget? placeholderLocal = ((global::Doroti.Framework.Widgets.Widget?)(object?)((placeholderText is null) ? null : new global::Doroti.Framework.Widgets.Visibility(maintainAnimation: true, maintainSize: true, maintainState: true, visible: !hasTextLocal, child: new global::Doroti.Framework.Widgets.SizedBox(width: double.PositiveInfinity, child: new global::Doroti.Framework.Widgets.Padding(padding: ((CupertinoTextField)this.widget).padding, child: new global::Doroti.Framework.Widgets.Text(placeholderText, maxLines: (hasTextLocal ? 1L : ((CupertinoTextField)this.widget).maxLines), overflow: ((global::Doroti.Framework.Painting.TextStyle)placeholderStyle).overflow, style: placeholderStyle, textAlign: ((CupertinoTextField)this.widget).textAlign))))));
            global::Doroti.Framework.Widgets.Widget? prefixWidget = (_CupertinoTextFieldState__text_field._shouldShowAttachment(attachment: ((CupertinoTextField)this.widget).prefixMode, hasText: hasTextLocal) ? ((CupertinoTextField)this.widget).prefix : null);
            bool showUserSuffix = _CupertinoTextFieldState__text_field._shouldShowAttachment(attachment: ((CupertinoTextField)this.widget).suffixMode, hasText: hasTextLocal);
            bool showClearButton = _CupertinoTextFieldState__text_field._shouldShowAttachment(attachment: ((CupertinoTextField)this.widget).clearButtonMode, hasText: hasTextLocal);
            global::Doroti.Framework.Widgets.Widget? suffixWidget = ((showUserSuffix, showClearButton) switch { (false, false) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(null), (true, false) => ((CupertinoTextField)this.widget).suffix, (true, true) => ((((CupertinoTextField)this.widget).suffix ?? (global::Doroti.Framework.Widgets.Widget)_buildClearButton())), (false, true) => _buildClearButton() });
            return new global::Doroti.Framework.Widgets.Row(crossAxisAlignment: ((CupertinoTextField)this.widget).crossAxisAlignment, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection51543 = new List<global::Doroti.Framework.Widgets.Widget>(); var __collectionElement51686 = prefixWidget; if (__collectionElement51686 is { } __nonNullCollectionElement51686) { __collection51543.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement51686)); } __collection51543.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.Directionality(textDirection: ((((CupertinoTextField)this.widget).textDirection ?? (TextDirection)Directionality.of(context))), child: new _BaselineAlignedStack__text_field(placeholder: placeholderLocal, editableText: editableText, textAlignVertical: this._textAlignVertical, editableTextBaseline: (((global::Doroti.Framework.Painting.TextStyle)textStyle).textBaseline ?? TextBaseline.alphabetic), placeholderBaseline: (((global::Doroti.Framework.Painting.TextStyle)placeholderStyle).textBaseline ?? TextBaseline.alphabetic)))))); var __collectionElement52402 = suffixWidget; if (__collectionElement52402 is { } __nonNullCollectionElement52402) { __collection51543.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement52402)); } return __collection51543; }))());
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string autofillId => ((global::Doroti.Framework.Widgets.EditableTextState)this._editableText).autofillId;
    public virtual void autofill(global::Doroti.Framework.Services.TextEditingValue newEditingValue) => this._editableText.autofill(newEditingValue);
    public virtual global::Doroti.Framework.Services.TextInputConfiguration textInputConfiguration
    {
        get
        {
            List<string>? autofillHintsLocal = ((CupertinoTextField)this.widget).autofillHints?.ToList().ToList();
            global::Doroti.Framework.Services.AutofillConfiguration autofillConfigurationLocal = ((autofillHintsLocal is not null) ? new global::Doroti.Framework.Services.AutofillConfiguration(uniqueIdentifier: this.autofillId, autofillHints: autofillHintsLocal, currentEditingValue: this._effectiveController.value, hintText: ((CupertinoTextField)this.widget).placeholder) : global::Doroti.Framework.Services.AutofillConfiguration.disabled);
            return ((global::Doroti.Framework.Services.TextInputConfiguration)(object?)((global::Doroti.Framework.Widgets.EditableTextState)this._editableText).textInputConfiguration.copyWith(autofillConfiguration: autofillConfigurationLocal));
            return default!;
        }
    }
    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if ((this.wantKeepAlive && (this._keepAliveHandle is null)))
        {
            _ensureKeepAlive();
        }
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        global::Doroti.Framework.Widgets.TextEditingController controllerLocal = this._effectiveController;
        global::Doroti.Framework.Widgets.TextSelectionControls? textSelectionControls = ((CupertinoTextField)this.widget).selectionControls;
        global::System.Action? handleDidGainAccessibilityFocus = default!;
        global::System.Action? handleDidLoseAccessibilityFocus = default!;
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                {
                    textSelectionControls ??= Text_selectionLibrary.cupertinoTextSelectionHandleControls;
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    textSelectionControls ??= Desktop_text_selectionLibrary.cupertinoDesktopTextSelectionHandleControls;
                    handleDidGainAccessibilityFocus = (global::System.Action)(() =>
                    {
                        if ((!((global::Doroti.Framework.Widgets.FocusNode)this._effectiveFocusNode).hasFocus && ((global::Doroti.Framework.Widgets.FocusNode)this._effectiveFocusNode).canRequestFocus))
                        {
                            this._effectiveFocusNode.requestFocus();
                        }
                    });
                    handleDidLoseAccessibilityFocus = (global::System.Action)(() =>
                    {
                        this._effectiveFocusNode.unfocus();
                    });
                    break;
                }
        }
        bool enabledLocal = ((CupertinoTextField)this.widget).enabled;
        var cursorOffsetLocal = new global::Doroti.Ui.Offset((Text_fieldLibrary._iOSHorizontalCursorOffsetPixels / MediaQuery.devicePixelRatioOf(context)), 0);
        var formatters = ((Func<List<global::Doroti.Framework.Services.TextInputFormatter>>)(() => { var __collection54739 = new List<global::Doroti.Framework.Services.TextInputFormatter>(); var __collectionSpread54767 = ((CupertinoTextField)this.widget).inputFormatters; if (__collectionSpread54767 is not null) { __collection54739.AddRange(__collectionSpread54767); } if ((((CupertinoTextField)this.widget).maxLength is not null)) { __collection54739.Add(new global::Doroti.Framework.Services.LengthLimitingTextInputFormatter(((CupertinoTextField)this.widget).maxLength, maxLengthEnforcement: this._effectiveMaxLengthEnforcement)); } return __collection54739; }))();
        CupertinoThemeData themeData = CupertinoTheme.of(context);
        global::Doroti.Framework.Painting.TextStyle? resolvedStyle = ((global::Doroti.Framework.Painting.TextStyle?)(object?)((CupertinoTextField)this.widget).style?.copyWith(color: CupertinoDynamicColor.maybeResolve(((CupertinoTextField)this.widget).style?.color, context), backgroundColor: CupertinoDynamicColor.maybeResolve(((CupertinoTextField)this.widget).style?.backgroundColor, context)));
        global::Doroti.Framework.Painting.TextStyle textStyleLocal = ((global::Doroti.Framework.Painting.TextStyle)(object?)themeData.textTheme.textStyle.merge(resolvedStyle));
        global::Doroti.Framework.Painting.TextStyle? resolvedPlaceholderStyle = ((global::Doroti.Framework.Painting.TextStyle?)(object?)((CupertinoTextField)this.widget).placeholderStyle?.copyWith(color: CupertinoDynamicColor.maybeResolve(((CupertinoTextField)this.widget).placeholderStyle?.color, context), backgroundColor: CupertinoDynamicColor.maybeResolve(((CupertinoTextField)this.widget).placeholderStyle?.backgroundColor, context)));
        global::Doroti.Framework.Painting.TextStyle placeholderStyleLocal = ((global::Doroti.Framework.Painting.TextStyle)(object?)textStyleLocal.merge(resolvedPlaceholderStyle));
        global::Doroti.Ui.Brightness keyboardAppearanceLocal = (((CupertinoTextField)this.widget).keyboardAppearance ?? CupertinoTheme.brightnessOf(context));
        global::Doroti.Ui.Color cursorColorLocal = ((global::Doroti.Ui.Color)(object?)(CupertinoDynamicColor.maybeResolve((((CupertinoTextField)this.widget).cursorColor ?? DefaultSelectionStyle.of(context).cursorColor), context) ?? themeData.primaryColor));
        global::Doroti.Ui.Color disabledColor = ((global::Doroti.Ui.Color)(object?)CupertinoDynamicColor.resolve(Text_fieldLibrary._kDisabledBackground, context));
        global::Doroti.Ui.Color? decorationColor = ((global::Doroti.Ui.Color?)(object?)CupertinoDynamicColor.maybeResolve(((CupertinoTextField)this.widget).decoration?.color, context));
        global::Doroti.Framework.Painting.BoxBorder? borderLocal = ((CupertinoTextField)this.widget).decoration?.border;
        var resolvedBorder = ((global::Doroti.Framework.Painting.Border?)(object?)borderLocal)!;
        if (true)
        {
            global::Doroti.Framework.Painting.Border border__56361__as56449 = (global::Doroti.Framework.Painting.Border)borderLocal;
            global::Doroti.Framework.Painting.BorderSide resolveBorderSide(global::Doroti.Framework.Painting.BorderSide side)
            {
                return ((object.Equals(side, global::Doroti.Framework.Painting.BorderSide.none)) ? side : side.copyWith(color: CupertinoDynamicColor.resolve(((global::Doroti.Framework.Painting.BorderSide)side).color, context)));
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            resolvedBorder = ((!object.Equals(DartRuntimePrimitives.RuntimeType(((global::Doroti.Framework.Painting.Border)border__56361__as56449)), typeof(global::Doroti.Framework.Painting.Border))) ? ((global::Doroti.Framework.Painting.Border)border__56361__as56449) : new global::Doroti.Framework.Painting.Border(top: resolveBorderSide(((global::Doroti.Framework.Painting.Border)((global::Doroti.Framework.Painting.Border)border__56361__as56449)).top), left: resolveBorderSide(((global::Doroti.Framework.Painting.Border)((global::Doroti.Framework.Painting.Border)border__56361__as56449)).left), bottom: resolveBorderSide(((global::Doroti.Framework.Painting.Border)((global::Doroti.Framework.Painting.Border)border__56361__as56449)).bottom), right: resolveBorderSide(((global::Doroti.Framework.Painting.Border)((global::Doroti.Framework.Painting.Border)border__56361__as56449)).right)));
        }
        global::Doroti.Framework.Painting.BoxDecoration? effectiveDecoration = ((global::Doroti.Framework.Painting.BoxDecoration?)(object?)((CupertinoTextField)this.widget).decoration?.copyWith(border: resolvedBorder, color: (enabledLocal ? decorationColor : (((object.Equals(((CupertinoTextField)this.widget).decoration, Text_fieldLibrary._kDefaultRoundedBorderDecoration)) ? disabledColor : ((CupertinoTextField)this.widget).decoration?.color)))));
        global::Doroti.Ui.Color selectionColorLocal = ((global::Doroti.Ui.Color)(object?)(CupertinoDynamicColor.maybeResolve(DefaultSelectionStyle.of(context).selectionColor, context) ?? CupertinoTheme.of(context).primaryColor.withOpacity(0.2)));
        global::Doroti.Framework.Widgets.SpellCheckConfiguration spellCheckConfigurationLocal = ((global::Doroti.Framework.Widgets.SpellCheckConfiguration)(object?)CupertinoTextField.inferIOSSpellCheckConfiguration(((CupertinoTextField)this.widget).spellCheckConfiguration));
        global::Doroti.Framework.Widgets.Widget paddedEditable = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Padding(padding: ((CupertinoTextField)this.widget).padding, child: new global::Doroti.Framework.Widgets.RepaintBoundary(child: new global::Doroti.Framework.Widgets.UnmanagedRestorationScope(bucket: this.bucket, child: new global::Doroti.Framework.Widgets.EditableText(key: this.editableTextKey, controller: controllerLocal, undoController: ((CupertinoTextField)this.widget).undoController, readOnly: (((CupertinoTextField)this.widget).readOnly || !enabledLocal), toolbarOptions: ((CupertinoTextField)this.widget).toolbarOptions, showCursor: ((CupertinoTextField)this.widget).showCursor, showSelectionHandles: this._showSelectionHandles, focusNode: this._effectiveFocusNode, keyboardType: ((CupertinoTextField)this.widget).keyboardType, textInputAction: ((CupertinoTextField)this.widget).textInputAction, textCapitalization: ((CupertinoTextField)this.widget).textCapitalization, style: textStyleLocal, strutStyle: ((CupertinoTextField)this.widget).strutStyle, textAlign: ((CupertinoTextField)this.widget).textAlign, textDirection: ((CupertinoTextField)this.widget).textDirection, autofocus: ((CupertinoTextField)this.widget).autofocus, obscuringCharacter: ((CupertinoTextField)this.widget).obscuringCharacter, obscureText: ((CupertinoTextField)this.widget).obscureText, autocorrect: ((CupertinoTextField)this.widget).autocorrect, smartDashesType: ((CupertinoTextField)this.widget).smartDashesType, smartQuotesType: ((CupertinoTextField)this.widget).smartQuotesType, enableSuggestions: ((CupertinoTextField)this.widget).enableSuggestions, maxLines: ((CupertinoTextField)this.widget).maxLines, minLines: ((CupertinoTextField)this.widget).minLines, expands: ((CupertinoTextField)this.widget).expands, magnifierConfiguration: (((CupertinoTextField)this.widget).magnifierConfiguration ?? CupertinoTextField._iosMagnifierConfiguration), selectionColor: (((global::Doroti.Framework.Widgets.FocusNode)this._effectiveFocusNode).hasFocus ? selectionColorLocal : null), selectionControls: (((CupertinoTextField)this.widget).selectionEnabled ? textSelectionControls : null), groupId: ((CupertinoTextField)this.widget).groupId, onChanged: (global::System.Action<string>?)((CupertinoTextField)this.widget).onChanged, onSelectionChanged: (global::System.Action<global::Doroti.Framework.Services.TextSelection, global::Doroti.Framework.Services.SelectionChangedCause?>)this._handleSelectionChanged, onEditingComplete: () => ((CupertinoTextField)this.widget).onEditingComplete(), onSubmitted: (global::System.Action<string>?)((CupertinoTextField)this.widget).onSubmitted, onTapOutside: (global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>?)((CupertinoTextField)this.widget).onTapOutside, inputFormatters: formatters, rendererIgnoresPointer: true, cursorWidth: ((CupertinoTextField)this.widget).cursorWidth, cursorHeight: ((CupertinoTextField)this.widget).cursorHeight, cursorRadius: ((CupertinoTextField)this.widget).cursorRadius, cursorColor: cursorColorLocal, cursorOpacityAnimates: ((CupertinoTextField)this.widget).cursorOpacityAnimates, cursorOffset: cursorOffsetLocal, paintCursorAboveText: true, autocorrectionTextRectColor: selectionColorLocal, backgroundCursorColor: CupertinoDynamicColor.resolve(CupertinoColors.inactiveGray, context), selectionHeightStyle: ((CupertinoTextField)this.widget).selectionHeightStyle, selectionWidthStyle: ((CupertinoTextField)this.widget).selectionWidthStyle, scrollPadding: ((CupertinoTextField)this.widget).scrollPadding, keyboardAppearance: keyboardAppearanceLocal, dragStartBehavior: ((CupertinoTextField)this.widget).dragStartBehavior, scrollController: ((CupertinoTextField)this.widget).scrollController, scrollPhysics: ((CupertinoTextField)this.widget).scrollPhysics, enableInteractiveSelection: ((CupertinoTextField)this.widget).enableInteractiveSelection, selectAllOnFocus: ((CupertinoTextField)this.widget).selectAllOnFocus, autofillClient: this, clipBehavior: ((CupertinoTextField)this.widget).clipBehavior, restorationId: "editable", scribbleEnabled: ((CupertinoTextField)this.widget).scribbleEnabled, stylusHandwritingEnabled: ((CupertinoTextField)this.widget).stylusHandwritingEnabled, enableIMEPersonalizedLearning: ((CupertinoTextField)this.widget).enableIMEPersonalizedLearning, enableInlinePrediction: ((CupertinoTextField)this.widget).enableInlinePrediction, contentInsertionConfiguration: ((CupertinoTextField)this.widget).contentInsertionConfiguration, contextMenuBuilder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget>?)((CupertinoTextField)this.widget).contextMenuBuilder, spellCheckConfiguration: spellCheckConfigurationLocal)))));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(enabled: enabledLocal, onTap: ((global::System.Action)((!enabledLocal || ((CupertinoTextField)this.widget).readOnly) ? null : (() =>
        {
            if (!((global::Doroti.Framework.Widgets.TextEditingController)controllerLocal).selection.isValid)
            {
                controllerLocal.selection = global::Doroti.Framework.Services.TextSelection.CreateCollapsed(offset: ((global::Doroti.Framework.Widgets.TextEditingController)controllerLocal).text.Length);
            }
            _requestKeyboard();
        }))), onDidGainAccessibilityFocus: () => handleDidGainAccessibilityFocus(), onDidLoseAccessibilityFocus: () => handleDidLoseAccessibilityFocus(), onFocus: ((global::System.Action)(enabledLocal ? (() =>
        {
            DartRuntimePrimitives.Assert(() => ((global::Doroti.Framework.Widgets.FocusNode)this._effectiveFocusNode).canRequestFocus, () => (object?)"Received SemanticsAction.focus from the engine. However, the FocusNode " + "of this text field cannot gain focus. This likely indicates a bug. " + "If this text field cannot be focused (e.g. because it is not " + "enabled), then its corresponding semantics node must be configured " + "such that the assistive technology cannot request focus on it.");
            if ((((global::Doroti.Framework.Widgets.FocusNode)this._effectiveFocusNode).canRequestFocus && !((global::Doroti.Framework.Widgets.FocusNode)this._effectiveFocusNode).hasFocus))
            {
                this._effectiveFocusNode.requestFocus();
            }
            else
            {
                if (!((CupertinoTextField)this.widget).readOnly)
                {
                    _requestKeyboard();
                }
            }
        }) : null)), child: new global::Doroti.Framework.Widgets.TextFieldTapRegion(child: new global::Doroti.Framework.Widgets.IgnorePointer(ignoring: !enabledLocal, child: new global::Doroti.Framework.Widgets.Container(decoration: effectiveDecoration, color: ((!enabledLocal && (effectiveDecoration is null)) ? disabledColor : null), child: this._selectionGestureDetectorBuilder.buildGestureDetector(behavior: global::Doroti.Framework.Rendering.HitTestBehavior.translucent, child: new global::Doroti.Framework.Widgets.Align(alignment: new global::Doroti.Framework.Painting.Alignment(-1.0, ((global::Doroti.Framework.Painting.TextAlignVertical)this._textAlignVertical).y), widthFactor: 1.0, heightFactor: 1.0, child: _addTextDependentAttachments(paddedEditable, textStyleLocal, placeholderStyleLocal))))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Services.RestorationBucket? bucket => this._bucket;
    public virtual void didToggleBucket(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => (this._bucket?.isReplacing != true));
    }

    public virtual void registerForRestoration(dynamic property, string restorationId)
    {
        DartRuntimePrimitives.Assert(() => ((((dynamic)property)._restorationId is null) || ((this._debugDoingRestore && (((dynamic)property)._restorationId == restorationId)))), () => (object?)$"Property is already registered under {((dynamic)property)._restorationId}.");
        DartRuntimePrimitives.Assert(() => (this._debugDoingRestore || !this._properties.Keys.map<dynamic, string?>(((r) => ((dynamic)r)._restorationId)).contains(restorationId)), () => (object?)$"\"{restorationId}\" is already registered to another property.");
        bool hasSerializedValue = (this.bucket?.contains(restorationId) ?? false);
        object? initialValue = (hasSerializedValue ? property.fromPrimitives(this.bucket!.read<object>(restorationId)) : property.createDefaultValue());
        if (!((dynamic)property).isRegistered)
        {
            property._register(restorationId, this);
            void listener()
            {
                if ((this.bucket is null))
                {
                    return;
                }
                _updateProperty(property);
            }
            property.addListener((global::System.Action)listener);
            this._properties[property] = (global::System.Action)listener;
        }
        DartRuntimePrimitives.Assert(() => (((((dynamic)property)._restorationId == restorationId) && (object.Equals(((dynamic)property)._owner, this))) && this._properties.ContainsKey(property)));
        property.initWithValue((dynamic)initialValue);
        if (((!hasSerializedValue && ((dynamic)property).enabled) && (this.bucket is not null)))
        {
            _updateProperty(property);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
    }

    public virtual void unregisterFromRestoration(dynamic property)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((dynamic)property)._owner, this)));
        this._bucket?.remove<object?>(((dynamic)property)._restorationId!);
        _unregister(property);
    }

    public virtual void didUpdateRestorationId()
    {
        if ((((this._currentParent is null) || (this._bucket?.restorationId == this.restorationId)) || this.restorePending))
        {
            return;
        }
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        DartRuntimePrimitives.Assert(() => !this.restorePending);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: this._currentParent, restorePending: false);
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket, this._bucket)));
            DartRuntimePrimitives.Assert(() => ((this._bucket is null) || (oldBucket is null)));
            oldBucket?.dispose();
        }
    }

    public virtual bool restorePending
    {
        get
        {
            if (this._firstRestorePending)
            {
                return true;
            }
            if ((this.restorationId is null))
            {
                return false;
            }
            global::Doroti.Framework.Services.RestorationBucket? potentialNewParent = ((global::Doroti.Framework.Services.RestorationBucket?)(object?)RestorationScope.maybeOf(this.context));
            return ((!object.Equals(potentialNewParent, this._currentParent)) && ((potentialNewParent?.isReplacing ?? false)));
            return default!;
        }
    }
    public virtual bool _debugDoingRestore => DartRuntimePrimitives.ConvertValue<bool>((this._debugPropertiesWaitingForReregistration is not null));
    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        bool needsRestore = this.restorePending;
        this._currentParent = RestorationScope.maybeOf(this.context);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: this._currentParent, restorePending: needsRestore);
        if (needsRestore)
        {
            _doRestore(oldBucket);
        }
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket, this._bucket)));
            oldBucket?.dispose();
        }
    }

    public virtual void _doRestore(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration = this._properties.Keys.ToList();
                return true;
            });
        restoreState(oldBucket, this._firstRestorePending);
        this._firstRestorePending = false;
        DartRuntimePrimitives.Assert(() =>
            {
                if (System.Linq.Enumerable.Any(this._debugPropertiesWaitingForReregistration!))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(((Func<List<global::Doroti.Framework.Foundation.DiagnosticsNode>>)(() => { var __collection41817 = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>(); __collection41817.Add(new global::Doroti.Framework.Foundation.ErrorSummary("Previously registered RestorableProperties must be re-registered in \"restoreState\".")); __collection41817.Add(new global::Doroti.Framework.Foundation.ErrorDescription($"The RestorableProperties with the following IDs were not re-registered to {this} when " + "\"restoreState\" was called:")); __collection41817.AddRange(this._debugPropertiesWaitingForReregistration!.map<dynamic, global::Doroti.Framework.Foundation.DiagnosticsNode>(((property) => new global::Doroti.Framework.Foundation.ErrorDescription($" * {((dynamic)property)._restorationId}")))); return __collection41817; }))()));
                }
                this._debugPropertiesWaitingForReregistration = null;
                return true;
            });
    }

    public virtual bool _updateBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? parent, bool restorePending)
    {
        if (((this.restorationId is null) || (parent is null)))
        {
            bool didReplace = _setNewBucketIfNecessary(newBucket: null, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (this._bucket is null));
            return didReplace;
        }
        DartRuntimePrimitives.Assert(() => (this.restorationId is not null));
        if ((restorePending || (this._bucket is null)))
        {
            global::Doroti.Framework.Services.RestorationBucket newBucketLocal = ((global::Doroti.Framework.Services.RestorationBucket)(object?)parent.claimChild(this.restorationId!, debugOwner: this));
            bool didReplaceLocal = _setNewBucketIfNecessary(newBucket: newBucketLocal, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (object.Equals(this._bucket, newBucketLocal)));
            return didReplaceLocal;
        }
        DartRuntimePrimitives.Assert(() => (this._bucket is not null));
        DartRuntimePrimitives.Assert(() => !restorePending);
        this._bucket!.rename(this.restorationId!);
        parent.adoptChild(this._bucket!);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _setNewBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? newBucket, bool restorePending)
    {
        if ((object.Equals(newBucket, this._bucket)))
        {
            return false;
        }
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        this._bucket = newBucket;
        if (!restorePending)
        {
            if ((this._bucket is not null))
            {
                this._properties.Keys.forEach((__arg0) => ((global::System.Action<dynamic>)this._updateProperty)(__arg0));
            }
            didToggleBucket(oldBucket);
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _updateProperty(dynamic property)
    {
        if (((dynamic)property).enabled)
        {
            this._bucket?.write(((dynamic)property)._restorationId!, property.toPrimitives());
        }
        else
        {
            this._bucket?.remove<object>(((dynamic)property)._restorationId!);
        }
    }

    public virtual void _unregister(dynamic property)
    {
        global::System.Action listener = this._properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
        property.removeListener(listener);
        property._unregister();
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

}

public enum _BaselineAlignedStackSlot__text_field
{
    placeholder,
    editableText
}

internal class _BaselineAlignedStack__text_field : global::Doroti.Framework.Widgets.SlottedMultiChildRenderObjectWidget<_BaselineAlignedStackSlot__text_field, global::Doroti.Framework.Rendering.RenderBox>
{
    public virtual TextBaseline editableTextBaseline { get; private set; } = default!;
    public virtual TextBaseline placeholderBaseline { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextAlignVertical textAlignVertical { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget editableText { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? placeholder { get; private set; }

    internal _BaselineAlignedStack__text_field(TextBaseline editableTextBaseline, TextBaseline placeholderBaseline, global::Doroti.Framework.Painting.TextAlignVertical textAlignVertical, global::Doroti.Framework.Widgets.Widget editableText, global::Doroti.Framework.Widgets.Widget? placeholder = null)
    {
        this.editableTextBaseline = editableTextBaseline;
        this.placeholderBaseline = placeholderBaseline;
        this.textAlignVertical = textAlignVertical;
        this.editableText = editableText;
        this.placeholder = placeholder;
    }

    public override IEnumerable<_BaselineAlignedStackSlot__text_field> slots => DartRuntimePrimitives.ConvertValue<IEnumerable<_BaselineAlignedStackSlot__text_field>>(System.Enum.GetValues<_BaselineAlignedStackSlot__text_field>().ToList());
    public override global::Doroti.Framework.Widgets.Widget? childForSlot(_BaselineAlignedStackSlot__text_field slot)
    {
        return (slot switch { _BaselineAlignedStackSlot__text_field.placeholder => this.placeholder, _BaselineAlignedStackSlot__text_field.editableText => this.editableText, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderBaselineAlignedStack__text_field(textAlignVertical: this.textAlignVertical, editableTextBaseline: this.editableTextBaseline, placeholderBaseline: this.placeholderBaseline));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.SlottedContainerRenderObjectMixin<_BaselineAlignedStackSlot__text_field, global::Doroti.Framework.Rendering.RenderBox> renderObject)
    {
        var __renderObject = (_RenderBaselineAlignedStack__text_field)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderBaselineAlignedStack__text_field>)(() =>
{
    var __cascade = __renderObject;
    __cascade.textAlignVertical = this.textAlignVertical;
    __cascade.editableTextBaseline = this.editableTextBaseline;
    __cascade.placeholderBaseline = this.placeholderBaseline;
    return __cascade;
}))());
    }

}

internal class _BaselineAlignedStackParentData__text_field : global::Doroti.Framework.Rendering.ContainerBoxParentData<global::Doroti.Framework.Rendering.RenderBox>
{
}

public class _RenderBaselineAlignedStack__text_field : global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Widgets.SlottedContainerRenderObjectMixin<_BaselineAlignedStackSlot__text_field, global::Doroti.Framework.Rendering.RenderBox>
{
    internal virtual global::Doroti.Framework.Painting.TextAlignVertical _textAlignVertical { get; set; } = default!;
    internal virtual TextBaseline _editableTextBaseline { get; set; } = default!;
    internal virtual TextBaseline _placeholderBaseline { get; set; } = default!;
    public virtual DartMap<_BaselineAlignedStackSlot__text_field, global::Doroti.Framework.Rendering.RenderBox> _slotToChild { get; set; } = new DartMap<_BaselineAlignedStackSlot__text_field, global::Doroti.Framework.Rendering.RenderBox>();

    internal _RenderBaselineAlignedStack__text_field(global::Doroti.Framework.Painting.TextAlignVertical textAlignVertical, TextBaseline editableTextBaseline, TextBaseline placeholderBaseline)
    {
        this._textAlignVertical = textAlignVertical;
        this._editableTextBaseline = editableTextBaseline;
        this._placeholderBaseline = placeholderBaseline;
    }

    public virtual global::Doroti.Framework.Painting.TextAlignVertical textAlignVertical
    {
        get => this._textAlignVertical;
        set
        {
            var __value = value;
            if ((object.Equals(this._textAlignVertical, __value)))
            {
                return;
            }
            _textAlignVertical = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.TextBaseline editableTextBaseline
    {
        get => this._editableTextBaseline;
        set
        {
            var __value = value;
            if ((object.Equals(this._editableTextBaseline, __value)))
            {
                return;
            }
            _editableTextBaseline = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.TextBaseline placeholderBaseline
    {
        get => this._placeholderBaseline;
        set
        {
            var __value = value;
            if ((object.Equals(this._placeholderBaseline, __value)))
            {
                return;
            }
            _placeholderBaseline = __value;
            markNeedsLayout();
        }
    }
    public override void setupParentData(global::Doroti.Framework.Rendering.RenderObject child)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)(object)child;
        if ((__child.parentData is not _BaselineAlignedStackParentData__text_field))
        {
            __child.parentData = new _BaselineAlignedStackParentData__text_field();
        }
    }

    internal virtual global::Doroti.Framework.Rendering.RenderBox? _placeholderChild
    {
        get
        {
            return ((global::Doroti.Framework.Rendering.RenderBox?)(object?)childForSlot(_BaselineAlignedStackSlot__text_field.placeholder));
            return default!;
        }
    }
    internal virtual global::Doroti.Framework.Rendering.RenderBox _editableTextChild
    {
        get
        {
            global::Doroti.Framework.Rendering.RenderBox? child = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)childForSlot(_BaselineAlignedStackSlot__text_field.editableText));
            DartRuntimePrimitives.Assert(() => (child is not null));
            return child!;
            return default!;
        }
    }
    public override double computeMinIntrinsicHeight(double width)
    {
        return Math.Max((this._placeholderChild?.getMinIntrinsicHeight(width) ?? 0.0), this._editableTextChild.getMinIntrinsicHeight(width));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return Math.Max((this._placeholderChild?.getMaxIntrinsicHeight(width) ?? 0.0), this._editableTextChild.getMaxIntrinsicHeight(width));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        return Math.Max((this._placeholderChild?.getMinIntrinsicWidth(height) ?? 0.0), this._editableTextChild.getMinIntrinsicWidth(height));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        return Math.Max((this._placeholderChild?.getMaxIntrinsicWidth(height) ?? 0.0), this._editableTextChild.getMaxIntrinsicWidth(height));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        DartRuntimePrimitives.Assert(() => ((global::Doroti.Framework.Rendering.BoxConstraints)this.constraints).hasTightWidth);
        global::Doroti.Framework.Rendering.RenderBox? placeholder = this._placeholderChild;
        global::Doroti.Framework.Rendering.RenderBox editableText = this._editableTextChild;
        var editableTextParentData = ((_BaselineAlignedStackParentData__text_field?)(object?)editableText.parentData!)!;
        var placeholderParentData = ((_BaselineAlignedStackParentData__text_field?)(object?)placeholder?.parentData)!;
        size = _computeSize(constraints: this.constraints, layoutChild: (global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size>)global::Doroti.Framework.Rendering.ChildLayoutHelper.layoutChild, getBaseline: (global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, TextBaseline, double?>)global::Doroti.Framework.Rendering.ChildLayoutHelper.getBaseline);
        double editableTextBaselineValue = DartRuntimePrimitives.RequireValue(editableText.getDistanceToBaseline(this.editableTextBaseline));
        double? placeholderBaselineValue = placeholder?.getDistanceToBaseline(this.placeholderBaseline);
        DartRuntimePrimitives.Assert(() => ((placeholder is not null) || (placeholderBaselineValue is null)));
        global::Doroti.Ui.Offset baselineDiff = ((global::Doroti.Ui.Offset)(object?)((placeholderBaselineValue is not null) ? new global::Doroti.Ui.Offset(0.0, (editableTextBaselineValue - DartRuntimePrimitives.RequireValue(placeholderBaselineValue))) : Offset.zero));
        var verticalAlignment = new global::Doroti.Framework.Painting.Alignment(0.0, ((global::Doroti.Framework.Painting.TextAlignVertical)this.textAlignVertical).y);
        editableTextParentData.offset = verticalAlignment.alongOffset((this.size - ((global::Doroti.Framework.Rendering.RenderBox)editableText).size));
        placeholderParentData?.offset = (editableTextParentData.offset + baselineDiff);
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        global::Doroti.Framework.Rendering.RenderBox? placeholder = this._placeholderChild;
        global::Doroti.Framework.Rendering.RenderBox editableText = this._editableTextChild;
        if ((placeholder is not null))
        {
            var placeholderParentData = ((_BaselineAlignedStackParentData__text_field?)(object?)placeholder.parentData!)!;
            context.paintChild(placeholder, (offset + placeholderParentData.offset));
        }
        var editableTextParentData = ((_BaselineAlignedStackParentData__text_field?)(object?)editableText.parentData!)!;
        context.paintChild(editableText, (offset + editableTextParentData.offset));
    }

    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return _computeSize(constraints: constraints, layoutChild: (global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size>)global::Doroti.Framework.Rendering.ChildLayoutHelper.dryLayoutChild, getBaseline: (global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, TextBaseline, double?>)global::Doroti.Framework.Rendering.ChildLayoutHelper.getDryBaseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Size _computeSize(global::Doroti.Framework.Rendering.BoxConstraints constraints, global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size> layoutChild, global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, TextBaseline, double?> getBaseline)
    {
        double widthLocal = ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).minWidth;
        double heightLocal = ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).minHeight;
        global::Doroti.Framework.Rendering.RenderBox editableText = this._editableTextChild;
        global::Doroti.Ui.Size editableTextSize = ((global::Doroti.Ui.Size)(object?)layoutChild(editableText, constraints));
        double editableTextBaselineValue = DartRuntimePrimitives.RequireValue(getBaseline(editableText, constraints, this.editableTextBaseline));
        double editableTextDescent = (editableTextSize.height - editableTextBaselineValue);
        global::Doroti.Ui.Size? placeholderSize = default!;
        double? placeholderBaselineValue = default!;
        global::Doroti.Framework.Rendering.RenderBox? placeholder = this._placeholderChild;
        if ((placeholder is not null))
        {
            placeholderSize = layoutChild(placeholder, constraints);
            widthLocal = Math.Max(widthLocal, placeholderSize.width);
            placeholderBaselineValue = getBaseline(placeholder, constraints, this.placeholderBaseline);
            double placeholderDescent = (placeholderSize.height - DartRuntimePrimitives.RequireValue(placeholderBaselineValue));
            double maxExtentBaseline = (Math.Max(editableTextBaselineValue, DartRuntimePrimitives.RequireValue(placeholderBaselineValue)) + Math.Max(editableTextDescent, placeholderDescent));
            heightLocal = Math.Max(heightLocal, maxExtentBaseline);
        }
        heightLocal = Math.Max(heightLocal, editableTextSize.height);
        widthLocal = Math.Max(widthLocal, editableTextSize.width);
        var size = new global::Doroti.Ui.Size(widthLocal, heightLocal);
        DartRuntimePrimitives.Assert(() => size.isFinite);
        return ((global::Doroti.Ui.Size)(object?)constraints.constrain(size));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestChildren(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        global::Doroti.Framework.Rendering.RenderBox editableText = this._editableTextChild;
        var editableTextParentData = ((_BaselineAlignedStackParentData__text_field?)(object?)editableText.parentData!)!;
        return result.addWithPaintOffset(offset: editableTextParentData.offset, position: position, hitTest: ((global::System.Func<global::Doroti.Framework.Rendering.BoxHitTestResult, Offset, bool>)((result, transformed) =>
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - editableTextParentData.offset))));
            return editableText.hitTest(result, position: transformed);
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Rendering.RenderBox? childForSlot(_BaselineAlignedStackSlot__text_field slot) => this._slotToChild.GetValueOrDefault(slot);
    public virtual IEnumerable<global::Doroti.Framework.Rendering.RenderBox> children => this._slotToChild.Values;
    public virtual string debugNameForSlot(_BaselineAlignedStackSlot__text_field slot)
    {
        if (true)
        {
            return slot.ToString();
        }
        return slot.ToString();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void attach(global::Doroti.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        foreach (global::Doroti.Framework.Rendering.RenderBox child in this.children)
        {
            ((dynamic)child).attach(owner);
        }
    }

    public override void detach()
    {
        base.detach();
        foreach (global::Doroti.Framework.Rendering.RenderBox child in this.children)
        {
            ((dynamic)child).detach();
        }
    }

    public override void redepthChildren()
    {
        this.children.forEach((__arg0) => ((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)this.redepthChild)(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(__arg0)));
    }

    public override void visitChildren(global::System.Action<global::Doroti.Framework.Rendering.RenderObject> visitor)
    {
        this.children.forEach((__arg0) => ((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)visitor)(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(__arg0)));
    }

    public override List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        var value = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        var childToSlot = new DartMap<global::Doroti.Framework.Rendering.RenderBox, _BaselineAlignedStackSlot__text_field>(this._slotToChild.Values, this._slotToChild.Keys);
        foreach (global::Doroti.Framework.Rendering.RenderBox child in this.children)
        {
            _addDiagnostics(child, value, debugNameForSlot(((_BaselineAlignedStackSlot__text_field)DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<_BaselineAlignedStackSlot__text_field>(childToSlot, child)))));
        }
        return value;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _addDiagnostics(global::Doroti.Framework.Rendering.RenderBox child, List<global::Doroti.Framework.Foundation.DiagnosticsNode> value, string name)
    {
        value.Add(((Diagnosticable)child).toDiagnosticsNode(name: name));
    }

    public virtual void _setChild(global::Doroti.Framework.Rendering.RenderBox? child, _BaselineAlignedStackSlot__text_field slot)
    {
        global::Doroti.Framework.Rendering.RenderBox? oldChild = this._slotToChild.GetValueOrDefault(slot);
        if ((oldChild is not null))
        {
            dropChild(oldChild);
            this._slotToChild.remove(slot);
        }
        if ((child is not null))
        {
            this._slotToChild[slot] = child;
            adoptChild(child);
        }
    }

    public virtual void _moveChild(global::Doroti.Framework.Rendering.RenderBox child, _BaselineAlignedStackSlot__text_field slot, _BaselineAlignedStackSlot__text_field oldSlot)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(slot, oldSlot)));
        global::Doroti.Framework.Rendering.RenderBox? oldChild = this._slotToChild.GetValueOrDefault(oldSlot);
        if ((object.Equals(oldChild, child)))
        {
            _setChild(null, oldSlot);
        }
        _setChild(child, slot);
    }

}
