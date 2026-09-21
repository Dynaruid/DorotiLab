// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/text_field.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class Text_fieldLibrary
{
    internal static TextStyle _kDefaultPlaceholderStyle = new TextStyle(
        fontWeight: FontWeight.w400,
        color: CupertinoColors.placeholderText
    );
}

public static partial class Text_fieldLibrary
{
    internal static BorderSide _kDefaultRoundedBorderSide = new BorderSide(
        color: new CupertinoDynamicColor(
            color: new Color(855638016L),
            darkColor: new Color(872415231L)
        ),
        width: 0.0
    );
}

public static partial class Text_fieldLibrary
{
    internal static Border _kDefaultRoundedBorder = new Border(
        top: _kDefaultRoundedBorderSide,
        bottom: _kDefaultRoundedBorderSide,
        left: _kDefaultRoundedBorderSide,
        right: _kDefaultRoundedBorderSide
    );
}

public static partial class Text_fieldLibrary
{
    internal static BoxDecoration _kDefaultRoundedBorderDecoration = new BoxDecoration(
        color: new CupertinoDynamicColor(
            color: CupertinoColors.white,
            darkColor: CupertinoColors.black
        ),
        border: _kDefaultRoundedBorder,
        borderRadius: BorderRadius.CreateAll(Radius.circular(5.0))
    );
}

public static partial class Text_fieldLibrary
{
    internal static Color _kDisabledBackground = new CupertinoDynamicColor(
        color: new Color(4294638330L),
        darkColor: new Color(4278519045L)
    );
}

public static partial class Text_fieldLibrary
{
    internal static CupertinoDynamicColor _kClearButtonColor = new CupertinoDynamicColor(
        color: new Color(855638016L),
        darkColor: new Color(872415231L)
    );
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
    always,
}

internal class _CupertinoTextFieldSelectionGestureDetectorBuilder__text_field
    : TextSelectionGestureDetectorBuilder
{
    internal virtual _CupertinoTextFieldState__text_field _state { get; private set; } = default!;

    internal _CupertinoTextFieldSelectionGestureDetectorBuilder__text_field(
        _CupertinoTextFieldState__text_field state
    )
        : base(@delegate: state)
    {
        _state = state;
    }

    public override void onSingleTapUp(Gestures.TapDragUpDetails details)
    {
        if (_state._clearGlobalKey.currentContext is not null)
        {
            var renderBox = (
                (RenderBox?)_state._clearGlobalKey.currentContext!.findRenderObject()!
            )!;
            Offset localOffset = renderBox.globalToLocal(details.globalPosition);
            if (renderBox.hitTest(new BoxHitTestResult(), position: localOffset))
            {
                return;
            }
        }
        base.onSingleTapUp(details);
        _state.widget.onTap?.Invoke();
    }

    public override void onDragSelectionEnd(Gestures.TapDragEndDetails details)
    {
        _state._requestKeyboard();
        base.onDragSelectionEnd(details);
    }
}

public class CupertinoTextField : StatefulWidget
{
    public virtual object groupId { get; private set; } = default!;
    public virtual TextEditingController? controller { get; private set; }
    public virtual FocusNode? focusNode { get; private set; }
    public virtual BoxDecoration? decoration { get; private set; }
    public virtual EdgeInsetsGeometry padding { get; private set; } = default!;
    public virtual string? placeholder { get; private set; }
    public virtual TextStyle? placeholderStyle { get; private set; }
    public virtual Widget? prefix { get; private set; }
    public virtual OverlayVisibilityMode prefixMode { get; private set; } = default!;
    public virtual Widget? suffix { get; private set; }
    public virtual OverlayVisibilityMode suffixMode { get; private set; } = default!;
    public virtual CrossAxisAlignment crossAxisAlignment { get; private set; } = default!;
    public virtual OverlayVisibilityMode clearButtonMode { get; private set; } = default!;
    public virtual string? clearButtonSemanticLabel { get; private set; }
    public virtual TextInputType keyboardType { get; private set; } = default!;
    public virtual TextInputAction? textInputAction { get; private set; }
    public virtual TextCapitalization textCapitalization { get; private set; } = default!;
    public virtual TextStyle? style { get; private set; }
    public virtual Painting.StrutStyle? strutStyle { get; private set; }
    public virtual TextAlign textAlign { get; private set; } = default!;
    public virtual ToolbarOptions? toolbarOptions { get; private set; }
    public virtual TextAlignVertical? textAlignVertical { get; private set; }
    public virtual TextDirection? textDirection { get; private set; }
    public virtual bool readOnly { get; private set; } = default!;
    public virtual bool? showCursor { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual string obscuringCharacter { get; private set; } = default!;
    public virtual bool obscureText { get; private set; } = default!;
    public virtual bool? autocorrect { get; private set; }
    public virtual SmartDashesType smartDashesType { get; private set; } = default!;
    public virtual SmartQuotesType smartQuotesType { get; private set; } = default!;
    public virtual bool enableSuggestions { get; private set; } = default!;
    public virtual long? maxLines { get; private set; }
    public virtual long? minLines { get; private set; }
    public virtual bool expands { get; private set; } = default!;
    public virtual long? maxLength { get; private set; }
    public virtual MaxLengthEnforcement? maxLengthEnforcement { get; private set; }
    public virtual Action<string>? onChanged { get; private set; }
    public virtual Action? onEditingComplete { get; private set; }
    public virtual Action<string>? onSubmitted { get; private set; }
    public virtual Action<Gestures.PointerDownEvent>? onTapOutside { get; private set; }
    public virtual Action<Gestures.PointerDownEvent>? onTapUpOutside { get; private set; }
    public virtual List<TextInputFormatter>? inputFormatters { get; private set; }
    public virtual bool enabled { get; private set; } = default!;
    public virtual double cursorWidth { get; private set; } = default!;
    public virtual double? cursorHeight { get; private set; }
    public virtual Radius cursorRadius { get; private set; } = default!;
    public virtual bool cursorOpacityAnimates { get; private set; } = default!;
    public virtual Color? cursorColor { get; private set; }
    public virtual BoxHeightStyle? selectionHeightStyle { get; private set; }
    public virtual BoxWidthStyle? selectionWidthStyle { get; private set; }
    public virtual Brightness? keyboardAppearance { get; private set; }
    public virtual EdgeInsets scrollPadding { get; private set; } = default!;
    public virtual bool enableInteractiveSelection { get; private set; } = default!;
    public virtual bool? selectAllOnFocus { get; private set; }
    public virtual TextSelectionControls? selectionControls { get; private set; }
    public virtual Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual ScrollController? scrollController { get; private set; }
    public virtual ScrollPhysics? scrollPhysics { get; private set; }
    public virtual Action? onTap { get; private set; }
    public virtual IEnumerable<string>? autofillHints { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual string? restorationId { get; private set; }
    public virtual bool scribbleEnabled { get; private set; } = default!;
    public virtual bool stylusHandwritingEnabled { get; private set; } = default!;
    public virtual bool enableIMEPersonalizedLearning { get; private set; } = default!;
    public virtual bool? enableInlinePrediction { get; private set; }
    public virtual ContentInsertionConfiguration? contentInsertionConfiguration
    {
        get;
        private set;
    }
    public virtual Func<BuildContext, EditableTextState, Widget>? contextMenuBuilder
    {
        get;
        private set;
    }
    public virtual TextMagnifierConfiguration? magnifierConfiguration { get; private set; }
    public virtual SpellCheckConfiguration? spellCheckConfiguration { get; private set; }
    public static TextStyle cupertinoMisspelledTextStyle = new TextStyle(
        decoration: TextDecoration.underline,
        decorationColor: CupertinoColors.systemRed,
        decorationStyle: TextDecorationStyle.dotted
    );
    public static Color kMisspelledSelectionColor = new Color(1660917401L);
    public virtual UndoHistoryController? undoController { get; private set; }
    internal static TextMagnifierConfiguration _iosMagnifierConfiguration =
        new TextMagnifierConfiguration(
            magnifierBuilder: (context, controller, magnifierInfo) =>
            {
                switch (PlatformLibrary.defaultTargetPlatform)
                {
                    case TargetPlatform.android:
                    case TargetPlatform.iOS:
                    {
                        return (Widget?)
                            new CupertinoTextMagnifier(
                                controller: controller,
                                magnifierInfo: magnifierInfo
                            );
                    }
                    case TargetPlatform.fuchsia:
                    case TargetPlatform.linux:
                    case TargetPlatform.macOS:
                    case TargetPlatform.windows:
                    {
                        return null;
                    }
                    default:
                        throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                }
                throw new InvalidOperationException("Dart closure completed without a value.");
            }
        );

    public CupertinoTextField(
        Key? key = null,
        object groupId = default!,
        TextEditingController? controller = null,
        FocusNode? focusNode = null,
        UndoHistoryController? undoController = null,
        BoxDecoration? decoration = default!,
        EdgeInsetsGeometry padding = default!,
        string? placeholder = null,
        TextStyle? placeholderStyle = default!,
        Widget? prefix = null,
        OverlayVisibilityMode prefixMode = OverlayVisibilityMode.always,
        Widget? suffix = null,
        OverlayVisibilityMode suffixMode = OverlayVisibilityMode.always,
        CrossAxisAlignment crossAxisAlignment = CrossAxisAlignment.center,
        OverlayVisibilityMode clearButtonMode = OverlayVisibilityMode.never,
        string? clearButtonSemanticLabel = null,
        TextInputType? keyboardType = null,
        TextInputAction? textInputAction = null,
        TextCapitalization textCapitalization = TextCapitalization.none,
        TextStyle? style = null,
        Painting.StrutStyle? strutStyle = null,
        TextAlign textAlign = TextAlign.start,
        TextAlignVertical? textAlignVertical = null,
        TextDirection? textDirection = null,
        bool readOnly = false,
        ToolbarOptions? toolbarOptions = null,
        bool? showCursor = null,
        bool autofocus = false,
        string obscuringCharacter = "•",
        bool obscureText = false,
        bool? autocorrect = true,
        SmartDashesType? smartDashesType = null,
        SmartQuotesType? smartQuotesType = null,
        bool enableSuggestions = true,
        long? maxLines = 1,
        long? minLines = null,
        bool expands = false,
        long? maxLength = null,
        MaxLengthEnforcement? maxLengthEnforcement = null,
        Action<string>? onChanged = null,
        Action? onEditingComplete = null,
        Action<string>? onSubmitted = null,
        Action<Gestures.PointerDownEvent>? onTapOutside = null,
        Action<Gestures.PointerDownEvent>? onTapUpOutside = null,
        List<TextInputFormatter>? inputFormatters = null,
        bool enabled = true,
        double cursorWidth = 2.0,
        double? cursorHeight = null,
        Radius? cursorRadius = null,
        bool cursorOpacityAnimates = true,
        Color? cursorColor = null,
        BoxHeightStyle? selectionHeightStyle = null,
        BoxWidthStyle? selectionWidthStyle = null,
        Brightness? keyboardAppearance = null,
        EdgeInsets scrollPadding = default!,
        Gestures.DragStartBehavior dragStartBehavior = Gestures.DragStartBehavior.start,
        bool? enableInteractiveSelection = null,
        bool? selectAllOnFocus = null,
        TextSelectionControls? selectionControls = null,
        Action? onTap = null,
        ScrollController? scrollController = null,
        ScrollPhysics? scrollPhysics = null,
        IEnumerable<string>? autofillHints = default!,
        ContentInsertionConfiguration? contentInsertionConfiguration = null,
        Clip clipBehavior = Clip.hardEdge,
        string? restorationId = null,
        bool scribbleEnabled = true,
        bool? stylusHandwritingEnabled = null,
        bool enableIMEPersonalizedLearning = true,
        bool? enableInlinePrediction = null,
        Func<BuildContext, EditableTextState, Widget>? contextMenuBuilder = default!,
        SpellCheckConfiguration? spellCheckConfiguration = null,
        TextMagnifierConfiguration? magnifierConfiguration = null
    )
        : base(key: key)
    {
        object __groupId = groupId ?? typeof(EditableText);
        BoxDecoration? __decoration =
            decoration ?? Text_fieldLibrary._kDefaultRoundedBorderDecoration;
        EdgeInsetsGeometry __padding = padding ?? EdgeInsets.CreateAll(7.0);
        TextStyle? __placeholderStyle =
            placeholderStyle
            ?? new TextStyle(fontWeight: FontWeight.w400, color: CupertinoColors.placeholderText);
        Radius __cursorRadius = cursorRadius ?? Radius.CreateCircular(2.0);
        EdgeInsets __scrollPadding = scrollPadding ?? EdgeInsets.CreateAll(20.0);
        IEnumerable<string>? __autofillHints = autofillHints ?? new List<string>();
        bool __stylusHandwritingEnabled =
            stylusHandwritingEnabled ?? EditableText.defaultStylusHandwritingEnabled;
        Func<BuildContext, EditableTextState, Widget>? __contextMenuBuilder =
            contextMenuBuilder ?? _defaultContextMenuBuilder;
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
        this.smartDashesType =
            smartDashesType ?? (obscureText ? SmartDashesType.disabled : SmartDashesType.enabled);
        this.smartQuotesType =
            smartQuotesType ?? (obscureText ? SmartQuotesType.disabled : SmartQuotesType.enabled);
        this.keyboardType =
            keyboardType ?? ((maxLines == 1L) ? TextInputType.text : TextInputType.multiline);
        this.enableInteractiveSelection = enableInteractiveSelection ?? (!readOnly || !obscureText);
        System.Diagnostics.Debug.Assert(obscuringCharacter.Length == 1L);
        System.Diagnostics.Debug.Assert(
            (maxLines is null)
                || (
                    (
                        maxLines
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ) > 0L
                )
        );
        System.Diagnostics.Debug.Assert(
            (minLines is null)
                || (
                    (
                        minLines
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ) > 0L
                )
        );
        System.Diagnostics.Debug.Assert(
            maxLines is null
                || minLines is null
                || maxLines
                    >= (
                        minLines
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    )
        );
        System.Diagnostics.Debug.Assert(!expands || ((maxLines is null) && (minLines is null)));
        System.Diagnostics.Debug.Assert(!obscureText || (maxLines == 1L));
        System.Diagnostics.Debug.Assert(
            (maxLength is null)
                || (
                    (
                        maxLength
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ) > 0L
                )
        );
        System.Diagnostics.Debug.Assert(
            !DartRuntimePrimitives.Identical(textInputAction, TextInputAction.newline)
                || (maxLines == 1L)
                || !DartRuntimePrimitives.Identical(keyboardType, TextInputType.text)
        );
    }

    public static CupertinoTextField CreateBorderless(
        Key? key = null,
        object groupId = default!,
        TextEditingController? controller = null,
        FocusNode? focusNode = null,
        UndoHistoryController? undoController = null,
        BoxDecoration? decoration = null,
        EdgeInsetsGeometry padding = default!,
        string? placeholder = null,
        TextStyle? placeholderStyle = default!,
        Widget? prefix = null,
        OverlayVisibilityMode prefixMode = OverlayVisibilityMode.always,
        Widget? suffix = null,
        OverlayVisibilityMode suffixMode = OverlayVisibilityMode.always,
        CrossAxisAlignment crossAxisAlignment = CrossAxisAlignment.center,
        OverlayVisibilityMode clearButtonMode = OverlayVisibilityMode.never,
        string? clearButtonSemanticLabel = null,
        TextInputType? keyboardType = null,
        TextInputAction? textInputAction = null,
        TextCapitalization textCapitalization = TextCapitalization.none,
        TextStyle? style = null,
        Painting.StrutStyle? strutStyle = null,
        TextAlign textAlign = TextAlign.start,
        TextAlignVertical? textAlignVertical = null,
        TextDirection? textDirection = null,
        bool readOnly = false,
        ToolbarOptions? toolbarOptions = null,
        bool? showCursor = null,
        bool autofocus = false,
        string obscuringCharacter = "•",
        bool obscureText = false,
        bool? autocorrect = null,
        SmartDashesType? smartDashesType = null,
        SmartQuotesType? smartQuotesType = null,
        bool enableSuggestions = true,
        long? maxLines = 1,
        long? minLines = null,
        bool expands = false,
        long? maxLength = null,
        MaxLengthEnforcement? maxLengthEnforcement = null,
        Action<string>? onChanged = null,
        Action? onEditingComplete = null,
        Action<string>? onSubmitted = null,
        Action<Gestures.PointerDownEvent>? onTapOutside = null,
        Action<Gestures.PointerDownEvent>? onTapUpOutside = null,
        List<TextInputFormatter>? inputFormatters = null,
        bool enabled = true,
        double cursorWidth = 2.0,
        double? cursorHeight = null,
        Radius? cursorRadius = null,
        bool cursorOpacityAnimates = true,
        Color? cursorColor = null,
        BoxHeightStyle? selectionHeightStyle = null,
        BoxWidthStyle? selectionWidthStyle = null,
        Brightness? keyboardAppearance = null,
        EdgeInsets scrollPadding = default!,
        Gestures.DragStartBehavior dragStartBehavior = Gestures.DragStartBehavior.start,
        bool? enableInteractiveSelection = null,
        bool? selectAllOnFocus = null,
        TextSelectionControls? selectionControls = null,
        Action? onTap = null,
        ScrollController? scrollController = null,
        ScrollPhysics? scrollPhysics = null,
        IEnumerable<string>? autofillHints = default!,
        ContentInsertionConfiguration? contentInsertionConfiguration = null,
        Clip clipBehavior = Clip.hardEdge,
        string? restorationId = null,
        bool scribbleEnabled = true,
        bool stylusHandwritingEnabled = true,
        bool enableIMEPersonalizedLearning = true,
        bool? enableInlinePrediction = null,
        Func<BuildContext, EditableTextState, Widget>? contextMenuBuilder = default!,
        SpellCheckConfiguration? spellCheckConfiguration = null,
        TextMagnifierConfiguration? magnifierConfiguration = null
    )
    {
        var __instance = new CupertinoTextField(
            key: key,
            groupId: groupId,
            controller: controller,
            focusNode: focusNode,
            undoController: undoController,
            decoration: decoration,
            padding: padding,
            placeholder: placeholder,
            placeholderStyle: placeholderStyle,
            prefix: prefix,
            prefixMode: prefixMode,
            suffix: suffix,
            suffixMode: suffixMode,
            crossAxisAlignment: crossAxisAlignment,
            clearButtonMode: clearButtonMode,
            clearButtonSemanticLabel: clearButtonSemanticLabel,
            keyboardType: keyboardType,
            textInputAction: textInputAction,
            textCapitalization: textCapitalization,
            style: style,
            strutStyle: strutStyle,
            textAlign: textAlign,
            textAlignVertical: textAlignVertical,
            textDirection: textDirection,
            readOnly: readOnly,
            toolbarOptions: toolbarOptions,
            showCursor: showCursor,
            autofocus: autofocus,
            obscuringCharacter: obscuringCharacter,
            obscureText: obscureText,
            autocorrect: autocorrect,
            smartDashesType: smartDashesType,
            smartQuotesType: smartQuotesType,
            enableSuggestions: enableSuggestions,
            maxLines: maxLines,
            minLines: minLines,
            expands: expands,
            maxLength: maxLength,
            maxLengthEnforcement: maxLengthEnforcement,
            onChanged: onChanged,
            onEditingComplete: onEditingComplete,
            onSubmitted: onSubmitted,
            onTapOutside: onTapOutside,
            onTapUpOutside: onTapUpOutside,
            inputFormatters: inputFormatters,
            enabled: enabled,
            cursorWidth: cursorWidth,
            cursorHeight: cursorHeight,
            cursorRadius: cursorRadius,
            cursorOpacityAnimates: cursorOpacityAnimates,
            cursorColor: cursorColor,
            selectionHeightStyle: selectionHeightStyle,
            selectionWidthStyle: selectionWidthStyle,
            keyboardAppearance: keyboardAppearance,
            scrollPadding: scrollPadding,
            dragStartBehavior: dragStartBehavior,
            enableInteractiveSelection: enableInteractiveSelection,
            selectAllOnFocus: selectAllOnFocus,
            selectionControls: selectionControls,
            onTap: onTap,
            scrollController: scrollController,
            scrollPhysics: scrollPhysics,
            autofillHints: autofillHints,
            contentInsertionConfiguration: contentInsertionConfiguration,
            clipBehavior: clipBehavior,
            restorationId: restorationId,
            scribbleEnabled: scribbleEnabled,
            stylusHandwritingEnabled: stylusHandwritingEnabled,
            enableIMEPersonalizedLearning: enableIMEPersonalizedLearning,
            enableInlinePrediction: enableInlinePrediction,
            contextMenuBuilder: contextMenuBuilder,
            spellCheckConfiguration: spellCheckConfiguration,
            magnifierConfiguration: magnifierConfiguration
        );
        object __groupId = groupId ?? typeof(EditableText);
        EdgeInsetsGeometry __padding = padding ?? EdgeInsets.CreateAll(7.0);
        TextStyle? __placeholderStyle =
            placeholderStyle ?? Text_fieldLibrary._kDefaultPlaceholderStyle;
        Radius __cursorRadius = cursorRadius ?? Radius.CreateCircular(2.0);
        EdgeInsets __scrollPadding = scrollPadding ?? EdgeInsets.CreateAll(20.0);
        IEnumerable<string>? __autofillHints = autofillHints ?? new List<string>();
        Func<BuildContext, EditableTextState, Widget>? __contextMenuBuilder =
            contextMenuBuilder ?? _defaultContextMenuBuilder;
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
        __instance.smartDashesType =
            smartDashesType ?? (obscureText ? SmartDashesType.disabled : SmartDashesType.enabled);
        __instance.smartQuotesType =
            smartQuotesType ?? (obscureText ? SmartQuotesType.disabled : SmartQuotesType.enabled);
        __instance.keyboardType =
            keyboardType ?? ((maxLines == 1L) ? TextInputType.text : TextInputType.multiline);
        __instance.enableInteractiveSelection =
            enableInteractiveSelection ?? (!readOnly || !obscureText);
        return __instance;
    }

    public virtual bool selectionEnabled => enableInteractiveSelection;

    internal static Widget _defaultContextMenuBuilder(
        BuildContext context,
        EditableTextState editableTextState
    )
    {
        if (SystemContextMenu.isSupportedByField(editableTextState))
        {
            return SystemContextMenu.CreateEditableText(editableTextState: editableTextState);
        }
        return CupertinoAdaptiveTextSelectionToolbar.CreateEditableText(
            editableTextState: editableTextState
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Widget defaultSpellCheckSuggestionsToolbarBuilder(
        BuildContext context,
        EditableTextState editableTextState
    )
    {
        return CupertinoSpellCheckSuggestionsToolbar.CreateEditableText(
            editableTextState: editableTextState
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoTextFieldState__text_field());

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new DiagnosticsProperty<TextEditingController>(
                "controller",
                controller,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<FocusNode>("focusNode", focusNode, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<UndoHistoryController>(
                "undoController",
                undoController,
                defaultValue: null
            )
        );
        properties.add(new DiagnosticsProperty<BoxDecoration>("decoration", decoration));
        properties.add(new DiagnosticsProperty<EdgeInsetsGeometry>("padding", padding));
        properties.add(new StringProperty("placeholder", placeholder));
        properties.add(new DiagnosticsProperty<TextStyle>("placeholderStyle", placeholderStyle));
        properties.add(
            new DiagnosticsProperty<OverlayVisibilityMode>(
                "prefix",
                (prefix is null) ? null : prefixMode
            )
        );
        properties.add(
            new DiagnosticsProperty<OverlayVisibilityMode>(
                "suffix",
                (suffix is null) ? null : suffixMode
            )
        );
        properties.add(
            new DiagnosticsProperty<OverlayVisibilityMode>("clearButtonMode", clearButtonMode)
        );
        properties.add(
            new DiagnosticsProperty<string>("clearButtonSemanticLabel", clearButtonSemanticLabel)
        );
        properties.add(
            new DiagnosticsProperty<TextInputType>(
                "keyboardType",
                keyboardType,
                defaultValue: TextInputType.text
            )
        );
        properties.add(new DiagnosticsProperty<TextStyle>("style", style, defaultValue: null));
        properties.add(new DiagnosticsProperty<bool>("autofocus", autofocus, defaultValue: false));
        properties.add(
            new DiagnosticsProperty<string>(
                "obscuringCharacter",
                obscuringCharacter,
                defaultValue: "•"
            )
        );
        properties.add(
            new DiagnosticsProperty<bool>("obscureText", obscureText, defaultValue: false)
        );
        properties.add(
            new DiagnosticsProperty<bool>("autocorrect", autocorrect, defaultValue: null)
        );
        properties.add(
            new EnumProperty<SmartDashesType>(
                "smartDashesType",
                smartDashesType,
                defaultValue: obscureText ? SmartDashesType.disabled : SmartDashesType.enabled
            )
        );
        properties.add(
            new EnumProperty<SmartQuotesType>(
                "smartQuotesType",
                smartQuotesType,
                defaultValue: obscureText ? SmartQuotesType.disabled : SmartQuotesType.enabled
            )
        );
        properties.add(
            new DiagnosticsProperty<bool>(
                "enableSuggestions",
                enableSuggestions,
                defaultValue: true
            )
        );
        properties.add(new IntProperty("maxLines", maxLines, defaultValue: 1L));
        properties.add(new IntProperty("minLines", minLines, defaultValue: null));
        properties.add(new DiagnosticsProperty<bool>("expands", expands, defaultValue: false));
        properties.add(new IntProperty("maxLength", maxLength, defaultValue: null));
        properties.add(
            new EnumProperty<MaxLengthEnforcement>(
                "maxLengthEnforcement",
                maxLengthEnforcement,
                defaultValue: null
            )
        );
        properties.add(new DoubleProperty("cursorWidth", cursorWidth, defaultValue: 2.0));
        properties.add(new DoubleProperty("cursorHeight", cursorHeight, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<Radius>("cursorRadius", (cursorRadius), defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<bool>(
                "cursorOpacityAnimates",
                cursorOpacityAnimates,
                defaultValue: true
            )
        );
        properties.add(
            ColorsLibrary.createCupertinoColorProperty(
                "cursorColor",
                cursorColor,
                defaultValue: null
            )
        );
        properties.add(
            new FlagProperty(
                "selectionEnabled",
                value: selectionEnabled,
                defaultValue: true,
                ifFalse: "selection disabled"
            )
        );
        properties.add(
            new DiagnosticsProperty<TextSelectionControls>(
                "selectionControls",
                selectionControls,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<ScrollController>(
                "scrollController",
                scrollController,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<ScrollPhysics>(
                "scrollPhysics",
                scrollPhysics,
                defaultValue: null
            )
        );
        properties.add(
            new EnumProperty<TextAlign>("textAlign", textAlign, defaultValue: TextAlign.start)
        );
        properties.add(
            new DiagnosticsProperty<TextAlignVertical>(
                "textAlignVertical",
                textAlignVertical,
                defaultValue: null
            )
        );
        properties.add(
            new EnumProperty<TextDirection>("textDirection", textDirection, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<Clip>("clipBehavior", clipBehavior, defaultValue: Clip.hardEdge)
        );
        properties.add(
            new DiagnosticsProperty<bool>("scribbleEnabled", scribbleEnabled, defaultValue: true)
        );
        properties.add(
            new DiagnosticsProperty<bool>(
                "stylusHandwritingEnabled",
                (stylusHandwritingEnabled),
                defaultValue: EditableText.defaultStylusHandwritingEnabled
            )
        );
        properties.add(
            new DiagnosticsProperty<bool>(
                "enableIMEPersonalizedLearning",
                enableIMEPersonalizedLearning,
                defaultValue: true
            )
        );
        properties.add(
            new DiagnosticsProperty<bool?>(
                "enableInlinePrediction",
                enableInlinePrediction,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<SpellCheckConfiguration>(
                "spellCheckConfiguration",
                spellCheckConfiguration,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<List<string>>(
                "contentCommitMimeTypes",
                contentInsertionConfiguration?.allowedMimeTypes ?? new List<string>(),
                defaultValue: (contentInsertionConfiguration is null)
                    ? new List<string>()
                    : Editable_textLibrary.kDefaultContentInsertionMimeTypes
            )
        );
    }

    public static SpellCheckConfiguration inferIOSSpellCheckConfiguration(
        SpellCheckConfiguration? configuration
    )
    {
        if (
            (configuration is null)
            || Equals(configuration, SpellCheckConfiguration.CreateDisabled())
        )
        {
            return SpellCheckConfiguration.CreateDisabled();
        }
        return configuration.copyWith(
            misspelledTextStyle: configuration.misspelledTextStyle ?? cupertinoMisspelledTextStyle,
            misspelledSelectionColor: configuration.misspelledSelectionColor
                ?? kMisspelledSelectionColor,
            spellCheckSuggestionsToolbarBuilder: configuration.spellCheckSuggestionsToolbarBuilder
                ?? defaultSpellCheckSuggestionsToolbarBuilder
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _CupertinoTextFieldState__text_field
    : State<CupertinoTextField>,
        RestorationMixin<CupertinoTextField>,
        AutomaticKeepAliveClientMixin<CupertinoTextField>,
        TextSelectionGestureDetectorBuilderDelegate,
        AutofillClient
{
    internal virtual GlobalKey<IState> _clearGlobalKey { get; private set; } =
        GlobalKey<IState>.Create();
    internal virtual RestorableTextEditingController? _controller { get; set; } = default;
    internal virtual FocusNode? _focusNode { get; set; } = default;
    internal virtual bool _showSelectionHandles { get; set; } = false;
    internal virtual _CupertinoTextFieldSelectionGestureDetectorBuilder__text_field _selectionGestureDetectorBuilder { get; set; } =
        default!;
    public virtual GlobalKey<EditableTextState> editableTextKey { get; private set; } =
        GlobalKey<EditableTextState>.Create();
    public virtual RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<IRestorableProperty, Action> _properties { get; set; } =
        new DartMap<IRestorableProperty, Action>();
    public virtual List<IRestorableProperty>? _debugPropertiesWaitingForReregistration { get; set; } =
        default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual RestorationBucket? _currentParent { get; set; } = default;
    public virtual KeepAliveHandle? _keepAliveHandle { get; set; } = default;

    internal virtual TextEditingController _effectiveController =>
        DartRuntimePrimitives.ConvertValue<TextEditingController>(
            widget.controller ?? _controller!.value
        );
    internal virtual FocusNode _effectiveFocusNode =>
        DartRuntimePrimitives.ConvertValue<FocusNode>(
            widget.focusNode ?? (_focusNode ??= new FocusNode())
        );
    internal virtual MaxLengthEnforcement _effectiveMaxLengthEnforcement =>
        DartRuntimePrimitives.ConvertValue<MaxLengthEnforcement>(
            widget.maxLengthEnforcement
                ?? LengthLimitingTextInputFormatter.getDefaultMaxLengthEnforcement()
        );
    public virtual bool forcePressEnabled => true;
    public virtual bool selectionEnabled => widget.selectionEnabled;

    public override void initState()
    {
        base.initState();
        if (wantKeepAlive)
        {
            _ensureKeepAlive();
        }
        _selectionGestureDetectorBuilder =
            new _CupertinoTextFieldSelectionGestureDetectorBuilder__text_field(state: this);
        if (widget.controller is null)
        {
            _createLocalController();
        }
        _effectiveFocusNode.canRequestFocus = widget.enabled;
        _effectiveFocusNode.addListener(_handleFocusChanged);
    }

    public override void didUpdateWidget(CupertinoTextField oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        didUpdateRestorationId();
        if ((widget.controller is null) && (oldWidget.controller is not null))
        {
            _createLocalController(oldWidget.controller!.value);
        }
        else
        {
            if ((widget.controller is not null) && (oldWidget.controller is null))
            {
                unregisterFromRestoration(_controller!);
                _controller!.dispose();
                _controller = null;
            }
        }
        if (!Equals(widget.focusNode, oldWidget.focusNode))
        {
            (oldWidget.focusNode ?? _focusNode)?.removeListener(_handleFocusChanged);
            (widget.focusNode ?? _focusNode)?.addListener(_handleFocusChanged);
        }
        _effectiveFocusNode.canRequestFocus = widget.enabled;
    }

    public virtual void restoreState(RestorationBucket? oldBucket, bool initialRestore)
    {
        if (_controller is not null)
        {
            _registerController();
        }
    }

    internal virtual void _registerController()
    {
        DartRuntimePrimitives.Assert(() => _controller is not null);
        registerForRestoration(_controller!, "controller");
        _controller!.value.addListener(updateKeepAlive);
    }

    internal virtual void _createLocalController(TextEditingValue? value = null)
    {
        DartRuntimePrimitives.Assert(() => _controller is null);
        _controller =
            (value is null)
                ? RestorableTextEditingController.Create()
                : new RestorableTextEditingController(value);
        if (!restorePending)
        {
            _registerController();
        }
    }

    public virtual string? restorationId => widget.restorationId;

    public override void dispose()
    {
        _effectiveFocusNode.removeListener(_handleFocusChanged);
        _focusNode?.dispose();
        _controller?.dispose();
        _properties.forEach(
            (property, listener) =>
            {
                if (!property._disposed)
                {
                    property.removeListener(listener);
                }
            }
        );
        _bucket?.dispose();
        _bucket = null;
        base.dispose();
    }

    internal virtual EditableTextState _editableText =>
        DartRuntimePrimitives.ConvertValue<EditableTextState>(editableTextKey.currentState!);

    internal virtual void _requestKeyboard()
    {
        _editableText.requestKeyboard();
    }

    internal virtual void _handleFocusChanged()
    {
        setState(() => { });
    }

    internal virtual bool _shouldShowSelectionHandles(SelectionChangedCause? cause)
    {
        if (
            !_selectionGestureDetectorBuilder.shouldShowSelectionToolbar
            || !_selectionGestureDetectorBuilder.shouldShowSelectionHandles
        )
        {
            return false;
        }
        if (_effectiveController.selection.isCollapsed)
        {
            return false;
        }
        if (Equals(cause, SelectionChangedCause.keyboard))
        {
            return false;
        }
        if (Equals(cause, SelectionChangedCause.stylusHandwriting))
        {
            return true;
        }
        if (_effectiveController.text.Length != 0)
        {
            return true;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleSelectionChanged(
        TextSelection selection,
        SelectionChangedCause? cause
    )
    {
        bool willShowSelectionHandles = _shouldShowSelectionHandles(cause);
        if (willShowSelectionHandles != _showSelectionHandles)
        {
            setState(() =>
            {
                _showSelectionHandles = willShowSelectionHandles;
            });
        }
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.iOS:
            case TargetPlatform.macOS:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
            case TargetPlatform.fuchsia:
            case TargetPlatform.android:
            {
                if (Equals(cause, SelectionChangedCause.longPress))
                {
                    _editableText.bringIntoView(selection.extent);
                }
                break;
            }
        }
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.iOS:
            case TargetPlatform.fuchsia:
            case TargetPlatform.android:
            {
                break;
            }
            case TargetPlatform.macOS:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
            {
                if (Equals(cause, SelectionChangedCause.drag))
                {
                    _editableText.hideToolbar();
                }
                break;
            }
        }
    }

    public virtual bool wantKeepAlive =>
        DartRuntimePrimitives.ConvertValue<bool>(
            _controller is not null && _controller.value.text.Length != 0
        );

    internal static bool _shouldShowAttachment(OverlayVisibilityMode attachment, bool hasText)
    {
        return attachment switch
        {
            OverlayVisibilityMode.never => false,
            OverlayVisibilityMode.always => true,
            OverlayVisibilityMode.editing => hasText,
            OverlayVisibilityMode.notEditing => !hasText,
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _hasDecoration
    {
        get
        {
            return (widget.placeholder is not null)
                || (!Equals(widget.clearButtonMode, OverlayVisibilityMode.never))
                || (widget.prefix is not null)
                || (widget.suffix is not null);
        }
    }
    internal virtual TextAlignVertical _textAlignVertical
    {
        get
        {
            if (widget.textAlignVertical is not null)
            {
                return widget.textAlignVertical!;
            }
            return _hasDecoration ? TextAlignVertical.center : TextAlignVertical.top;
        }
    }

    internal virtual void _onClearButtonTapped()
    {
        bool hadText = _effectiveController.text.Length != 0;
        _effectiveController.clear();
        if (hadText)
        {
            widget.onChanged?.Invoke(_effectiveController.text);
        }
    }

    internal virtual Widget _buildClearButton()
    {
        string clearLabel =
            widget.clearButtonSemanticLabel ?? CupertinoLocalizations.of(context).clearButtonLabel;
        return new Widgets.Semantics(
            button: true,
            label: clearLabel,
            child: new GestureDetector(
                key: _clearGlobalKey,
                onTap: widget.enabled ? _onClearButtonTapped : null,
                child: new Padding(
                    padding: EdgeInsets.CreateSymmetric(horizontal: 6.0),
                    child: new Icon(
                        CupertinoIcons.clear_thick_circled,
                        size: 18.0,
                        color: CupertinoDynamicColor.resolve(
                            Text_fieldLibrary._kClearButtonColor,
                            context
                        )
                    )
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _addTextDependentAttachments(
        Widget editableText,
        TextStyle textStyle,
        TextStyle placeholderStyle
    )
    {
        if (!_hasDecoration)
        {
            return editableText;
        }
        return new ValueListenableBuilder<TextEditingValue>(
            valueListenable: _effectiveController,
            child: editableText,
            builder: (Func<BuildContext, TextEditingValue, Widget?, Row>)(
                (context, text, child) =>
                {
                    bool hasTextLocal = text.text.Length != 0;
                    string? placeholderText = widget.placeholder;
                    Widget? placeholderLocal =
                        (placeholderText is null)
                            ? null
                            : new Visibility(
                                maintainAnimation: true,
                                maintainSize: true,
                                maintainState: true,
                                visible: !hasTextLocal,
                                child: new SizedBox(
                                    width: double.PositiveInfinity,
                                    child: new Padding(
                                        padding: widget.padding,
                                        child: new Text(
                                            placeholderText,
                                            maxLines: hasTextLocal ? 1L : widget.maxLines,
                                            overflow: placeholderStyle.overflow,
                                            style: placeholderStyle,
                                            textAlign: widget.textAlign
                                        )
                                    )
                                )
                            );
                    Widget? prefixWidget = _shouldShowAttachment(
                        attachment: widget.prefixMode,
                        hasText: hasTextLocal
                    )
                        ? widget.prefix
                        : null;
                    bool showUserSuffix = _shouldShowAttachment(
                        attachment: widget.suffixMode,
                        hasText: hasTextLocal
                    );
                    bool showClearButton = _shouldShowAttachment(
                        attachment: widget.clearButtonMode,
                        hasText: hasTextLocal
                    );
                    Widget? suffixWidget = (showUserSuffix, showClearButton) switch
                    {
                        (false, false) => DartRuntimePrimitives.ConvertValue<Widget>(null),
                        (true, false) => widget.suffix,
                        (true, true) => widget.suffix ?? _buildClearButton(),
                        (false, true) => _buildClearButton(),
                    };
                    return new Row(
                        crossAxisAlignment: widget.crossAxisAlignment,
                        children: (
                            (Func<List<Widget>>)(
                                () =>
                                {
                                    var __collection51543 = new List<Widget>();
                                    var __collectionElement51686 = prefixWidget;
                                    if (
                                        __collectionElement51686 is
                                        { } __nonNullCollectionElement51686
                                    )
                                    {
                                        __collection51543.Add(
                                            DartRuntimePrimitives.ConvertValue<Widget>(
                                                __nonNullCollectionElement51686
                                            )
                                        );
                                    }
                                    __collection51543.Add(
                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                            new Expanded(
                                                child: new Directionality(
                                                    textDirection: widget.textDirection
                                                        ?? Directionality.of(context),
                                                    child: new _BaselineAlignedStack__text_field(
                                                        placeholder: placeholderLocal,
                                                        editableText: editableText,
                                                        textAlignVertical: _textAlignVertical,
                                                        editableTextBaseline: textStyle.textBaseline
                                                            ?? TextBaseline.alphabetic,
                                                        placeholderBaseline: placeholderStyle.textBaseline
                                                            ?? TextBaseline.alphabetic
                                                    )
                                                )
                                            )
                                        )
                                    );
                                    var __collectionElement52402 = suffixWidget;
                                    if (
                                        __collectionElement52402 is
                                        { } __nonNullCollectionElement52402
                                    )
                                    {
                                        __collection51543.Add(
                                            DartRuntimePrimitives.ConvertValue<Widget>(
                                                __nonNullCollectionElement52402
                                            )
                                        );
                                    }
                                    return __collection51543;
                                }
                            )
                        )()
                    );
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string autofillId => _editableText.autofillId;

    public virtual void autofill(TextEditingValue newEditingValue) =>
        _editableText.autofill(newEditingValue);

    public virtual TextInputConfiguration textInputConfiguration
    {
        get
        {
            List<string>? autofillHintsLocal = widget.autofillHints?.ToList().ToList();
            AutofillConfiguration autofillConfigurationLocal =
                (autofillHintsLocal is not null)
                    ? new AutofillConfiguration(
                        uniqueIdentifier: autofillId,
                        autofillHints: autofillHintsLocal,
                        currentEditingValue: _effectiveController.value,
                        hintText: widget.placeholder
                    )
                    : AutofillConfiguration.disabled;
            return _editableText.textInputConfiguration.copyWith(
                autofillConfiguration: autofillConfigurationLocal
            );
        }
    }

    public override Widget build(BuildContext context)
    {
        if (wantKeepAlive && (_keepAliveHandle is null))
        {
            _ensureKeepAlive();
        }
        DartRuntimePrimitives.Assert(() =>
            Widgets.DebugLibrary.debugCheckHasDirectionality(context)
        );
        TextEditingController controllerLocal = _effectiveController;
        TextSelectionControls? textSelectionControls = widget.selectionControls;
        Action? handleDidGainAccessibilityFocus = default!;
        Action? handleDidLoseAccessibilityFocus = default!;
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.iOS:
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            {
                textSelectionControls ??=
                    Text_selectionLibrary.cupertinoTextSelectionHandleControls;
                break;
            }
            case TargetPlatform.linux:
            case TargetPlatform.macOS:
            case TargetPlatform.windows:
            {
                textSelectionControls ??=
                    Desktop_text_selectionLibrary.cupertinoDesktopTextSelectionHandleControls;
                handleDidGainAccessibilityFocus = () =>
                {
                    if (!_effectiveFocusNode.hasFocus && _effectiveFocusNode.canRequestFocus)
                    {
                        _effectiveFocusNode.requestFocus();
                    }
                };
                handleDidLoseAccessibilityFocus = () =>
                {
                    _effectiveFocusNode.unfocus();
                };
                break;
            }
        }
        bool enabledLocal = widget.enabled;
        var cursorOffsetLocal = new Offset(
            Text_fieldLibrary._iOSHorizontalCursorOffsetPixels
                / MediaQuery.devicePixelRatioOf(context),
            0
        );
        var formatters = (
            (Func<List<TextInputFormatter>>)(
                () =>
                {
                    var __collection54739 = new List<TextInputFormatter>();
                    var __collectionSpread54767 = widget.inputFormatters;
                    if (__collectionSpread54767 is not null)
                    {
                        __collection54739.AddRange(__collectionSpread54767);
                    }
                    if (widget.maxLength is not null)
                    {
                        __collection54739.Add(
                            new LengthLimitingTextInputFormatter(
                                widget.maxLength,
                                maxLengthEnforcement: _effectiveMaxLengthEnforcement
                            )
                        );
                    }
                    return __collection54739;
                }
            )
        )();
        CupertinoThemeData themeData = CupertinoTheme.of(context);
        TextStyle? resolvedStyle = widget.style?.copyWith(
            color: CupertinoDynamicColor.maybeResolve(widget.style?.color, context),
            backgroundColor: CupertinoDynamicColor.maybeResolve(
                widget.style?.backgroundColor,
                context
            )
        );
        TextStyle textStyleLocal = themeData.textTheme.textStyle.merge(resolvedStyle);
        TextStyle? resolvedPlaceholderStyle = widget.placeholderStyle?.copyWith(
            color: CupertinoDynamicColor.maybeResolve(widget.placeholderStyle?.color, context),
            backgroundColor: CupertinoDynamicColor.maybeResolve(
                widget.placeholderStyle?.backgroundColor,
                context
            )
        );
        TextStyle placeholderStyleLocal = textStyleLocal.merge(resolvedPlaceholderStyle);
        Brightness keyboardAppearanceLocal =
            widget.keyboardAppearance ?? CupertinoTheme.brightnessOf(context);
        Color cursorColorLocal =
            CupertinoDynamicColor.maybeResolve(
                widget.cursorColor ?? DefaultSelectionStyle.of(context).cursorColor,
                context
            ) ?? themeData.primaryColor;
        Color disabledColor = CupertinoDynamicColor.resolve(
            Text_fieldLibrary._kDisabledBackground,
            context
        );
        Color? decorationColor = CupertinoDynamicColor.maybeResolve(
            widget.decoration?.color,
            context
        );
        BoxBorder? borderLocal = widget.decoration?.border;
        var resolvedBorder = ((Border?)borderLocal)!;
        if (borderLocal is Border border__56361__as56449)
        {
            BorderSide resolveBorderSide(BorderSide side)
            {
                return Equals(side, BorderSide.none)
                    ? side
                    : side.copyWith(color: CupertinoDynamicColor.resolve(side.color, context));
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            resolvedBorder =
                (!Equals(DartRuntimePrimitives.RuntimeType(border__56361__as56449), typeof(Border)))
                    ? border__56361__as56449
                    : new Border(
                        top: resolveBorderSide(border__56361__as56449.top),
                        left: resolveBorderSide(border__56361__as56449.left),
                        bottom: resolveBorderSide(border__56361__as56449.bottom),
                        right: resolveBorderSide(border__56361__as56449.right)
                    );
        }
        BoxDecoration? effectiveDecoration = widget.decoration?.copyWith(
            border: resolvedBorder,
            color: enabledLocal
                ? decorationColor
                : (
                    Equals(widget.decoration, Text_fieldLibrary._kDefaultRoundedBorderDecoration)
                        ? disabledColor
                        : widget.decoration?.color
                )
        );
        Color selectionColorLocal =
            CupertinoDynamicColor.maybeResolve(
                DefaultSelectionStyle.of(context).selectionColor,
                context
            ) ?? CupertinoTheme.of(context).primaryColor.withOpacity(0.2);
        SpellCheckConfiguration spellCheckConfigurationLocal =
            CupertinoTextField.inferIOSSpellCheckConfiguration(widget.spellCheckConfiguration);
        Widget paddedEditable = new Padding(
            padding: widget.padding,
            child: new RepaintBoundary(
                child: new UnmanagedRestorationScope(
                    bucket: bucket,
                    child: new EditableText(
                        key: editableTextKey,
                        controller: controllerLocal,
                        undoController: widget.undoController,
                        readOnly: widget.readOnly || !enabledLocal,
                        toolbarOptions: widget.toolbarOptions,
                        showCursor: widget.showCursor,
                        showSelectionHandles: _showSelectionHandles,
                        focusNode: _effectiveFocusNode,
                        keyboardType: widget.keyboardType,
                        textInputAction: widget.textInputAction,
                        textCapitalization: widget.textCapitalization,
                        style: textStyleLocal,
                        strutStyle: widget.strutStyle,
                        textAlign: widget.textAlign,
                        textDirection: widget.textDirection,
                        autofocus: widget.autofocus,
                        obscuringCharacter: widget.obscuringCharacter,
                        obscureText: widget.obscureText,
                        autocorrect: widget.autocorrect,
                        smartDashesType: widget.smartDashesType,
                        smartQuotesType: widget.smartQuotesType,
                        enableSuggestions: widget.enableSuggestions,
                        maxLines: widget.maxLines,
                        minLines: widget.minLines,
                        expands: widget.expands,
                        magnifierConfiguration: widget.magnifierConfiguration
                            ?? CupertinoTextField._iosMagnifierConfiguration,
                        selectionColor: _effectiveFocusNode.hasFocus ? selectionColorLocal : null,
                        selectionControls: widget.selectionEnabled ? textSelectionControls : null,
                        groupId: widget.groupId,
                        onChanged: widget.onChanged,
                        onSelectionChanged: _handleSelectionChanged,
                        onEditingComplete: widget.onEditingComplete,
                        onSubmitted: widget.onSubmitted,
                        onTapOutside: widget.onTapOutside,
                        inputFormatters: formatters,
                        rendererIgnoresPointer: true,
                        cursorWidth: widget.cursorWidth,
                        cursorHeight: widget.cursorHeight,
                        cursorRadius: widget.cursorRadius,
                        cursorColor: cursorColorLocal,
                        cursorOpacityAnimates: widget.cursorOpacityAnimates,
                        cursorOffset: cursorOffsetLocal,
                        paintCursorAboveText: true,
                        autocorrectionTextRectColor: selectionColorLocal,
                        backgroundCursorColor: CupertinoDynamicColor.resolve(
                            CupertinoColors.inactiveGray,
                            context
                        ),
                        selectionHeightStyle: widget.selectionHeightStyle,
                        selectionWidthStyle: widget.selectionWidthStyle,
                        scrollPadding: widget.scrollPadding,
                        keyboardAppearance: keyboardAppearanceLocal,
                        dragStartBehavior: widget.dragStartBehavior,
                        scrollController: widget.scrollController,
                        scrollPhysics: widget.scrollPhysics,
                        enableInteractiveSelection: widget.enableInteractiveSelection,
                        selectAllOnFocus: widget.selectAllOnFocus,
                        autofillClient: this,
                        clipBehavior: widget.clipBehavior,
                        restorationId: "editable",
                        scribbleEnabled: widget.scribbleEnabled,
                        stylusHandwritingEnabled: widget.stylusHandwritingEnabled,
                        enableIMEPersonalizedLearning: widget.enableIMEPersonalizedLearning,
                        enableInlinePrediction: widget.enableInlinePrediction,
                        contentInsertionConfiguration: widget.contentInsertionConfiguration,
                        contextMenuBuilder: widget.contextMenuBuilder,
                        spellCheckConfiguration: spellCheckConfigurationLocal
                    )
                )
            )
        );
        return new Widgets.Semantics(
            enabled: enabledLocal,
            onTap: (!enabledLocal || widget.readOnly)
                ? null
                : (
                    () =>
                    {
                        if (!controllerLocal.selection.isValid)
                        {
                            controllerLocal.selection = TextSelection.CreateCollapsed(
                                offset: controllerLocal.text.Length
                            );
                        }
                        _requestKeyboard();
                    }
                ),
            onDidGainAccessibilityFocus: () => handleDidGainAccessibilityFocus(),
            onDidLoseAccessibilityFocus: () => handleDidLoseAccessibilityFocus(),
            onFocus: enabledLocal
                ? (
                    () =>
                    {
                        DartRuntimePrimitives.Assert(
                            () => _effectiveFocusNode.canRequestFocus,
                            () =>
                                (object?)
                                    "Received SemanticsAction.focus from the engine. However, the FocusNode "
                                + "of this text field cannot gain focus. This likely indicates a bug. "
                                + "If this text field cannot be focused (e.g. because it is not "
                                + "enabled), then its corresponding semantics node must be configured "
                                + "such that the assistive technology cannot request focus on it."
                        );
                        if (_effectiveFocusNode.canRequestFocus && !_effectiveFocusNode.hasFocus)
                        {
                            _effectiveFocusNode.requestFocus();
                        }
                        else
                        {
                            if (!widget.readOnly)
                            {
                                _requestKeyboard();
                            }
                        }
                    }
                )
                : null,
            child: new TextFieldTapRegion(
                child: new IgnorePointer(
                    ignoring: !enabledLocal,
                    child: new Container(
                        decoration: effectiveDecoration,
                        color: (!enabledLocal && (effectiveDecoration is null))
                            ? disabledColor
                            : null,
                        child: _selectionGestureDetectorBuilder.buildGestureDetector(
                            behavior: HitTestBehavior.translucent,
                            child: new Align(
                                alignment: new Alignment(-1.0, _textAlignVertical.y),
                                widthFactor: 1.0,
                                heightFactor: 1.0,
                                child: _addTextDependentAttachments(
                                    paddedEditable,
                                    textStyleLocal,
                                    placeholderStyleLocal
                                )
                            )
                        )
                    )
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RestorationBucket? bucket => _bucket;

    public virtual void didToggleBucket(RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => _bucket?.isReplacing != true);
    }

    public virtual void registerForRestoration(IRestorableProperty property, string restorationId)
    {
        DartRuntimePrimitives.Assert(
            () =>
                (property._restorationId is null)
                || (_debugDoingRestore && (property._restorationId == restorationId)),
            () => (object?)$"Property is already registered under {property._restorationId}."
        );
        DartRuntimePrimitives.Assert(
            () =>
                _debugDoingRestore
                || !_properties.Keys.map((r) => r._restorationId).contains(restorationId),
            () => (object?)$"\"{restorationId}\" is already registered to another property."
        );
        bool hasSerializedValue = bucket?.contains(restorationId) ?? false;
        object? initialValue = hasSerializedValue
            ? property.fromPrimitivesObject(bucket!.read<object>(restorationId))
            : property.createDefaultValueObject();
        if (!property.isRegistered)
        {
            property._register(restorationId, this);
            void listener()
            {
                if (bucket is null)
                {
                    return;
                }
                _updateProperty(property);
            }
            property.addListener(listener);
            _properties[property] = listener;
        }
        DartRuntimePrimitives.Assert(() =>
            (property._restorationId == restorationId)
            && Equals(property._owner, this)
            && _properties.ContainsKey(property)
        );
        property.initWithValueObject(initialValue);
        if (!hasSerializedValue && property.enabled && (bucket is not null))
        {
            _updateProperty(property);
        }
        DartRuntimePrimitives.Assert(() =>
        {
            _debugPropertiesWaitingForReregistration?.Remove(property);
            return true;
        });
    }

    public virtual void unregisterFromRestoration(IRestorableProperty property)
    {
        DartRuntimePrimitives.Assert(() => Equals(property._owner, this));
        _bucket?.remove<object?>(property._restorationId!);
        _unregister(property);
    }

    public virtual void didUpdateRestorationId()
    {
        if ((_currentParent is null) || (_bucket?.restorationId == restorationId) || restorePending)
        {
            return;
        }
        RestorationBucket? oldBucket = _bucket;
        DartRuntimePrimitives.Assert(() => !restorePending);
        bool didReplaceBucket = _updateBucketIfNecessary(
            parent: _currentParent,
            restorePending: false
        );
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => !Equals(oldBucket, _bucket));
            DartRuntimePrimitives.Assert(() => (_bucket is null) || (oldBucket is null));
            oldBucket?.dispose();
        }
    }

    public virtual bool restorePending
    {
        get
        {
            if (_firstRestorePending)
            {
                return true;
            }
            if (restorationId is null)
            {
                return false;
            }
            RestorationBucket? potentialNewParent = RestorationScope.maybeOf(context);
            return (!Equals(potentialNewParent, _currentParent))
                && (potentialNewParent?.isReplacing ?? false);
        }
    }
    public virtual bool _debugDoingRestore =>
        DartRuntimePrimitives.ConvertValue<bool>(
            _debugPropertiesWaitingForReregistration is not null
        );

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        RestorationBucket? oldBucket = _bucket;
        bool needsRestore = restorePending;
        _currentParent = RestorationScope.maybeOf(context);
        bool didReplaceBucket = _updateBucketIfNecessary(
            parent: _currentParent,
            restorePending: needsRestore
        );
        if (needsRestore)
        {
            _doRestore(oldBucket);
        }
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => !Equals(oldBucket, _bucket));
            oldBucket?.dispose();
        }
    }

    public virtual void _doRestore(RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            _debugPropertiesWaitingForReregistration = _properties.Keys.ToList();
            return true;
        });
        restoreState(oldBucket, _firstRestorePending);
        _firstRestorePending = false;
        DartRuntimePrimitives.Assert(() =>
        {
            if (Enumerable.Any(_debugPropertiesWaitingForReregistration!))
            {
                throw DartRuntimePrimitives.AsException(
                    new FlutterError(
                        (
                            (Func<List<DiagnosticsNode>>)(
                                () =>
                                {
                                    var __collection41817 = new List<DiagnosticsNode>();
                                    __collection41817.Add(
                                        new ErrorSummary(
                                            "Previously registered RestorableProperties must be re-registered in \"restoreState\"."
                                        )
                                    );
                                    __collection41817.Add(
                                        new ErrorDescription(
                                            $"The RestorableProperties with the following IDs were not re-registered to {this} when "
                                                + "\"restoreState\" was called:"
                                        )
                                    );
                                    __collection41817.AddRange(
                                        _debugPropertiesWaitingForReregistration!.map<
                                            IRestorableProperty,
                                            DiagnosticsNode
                                        >(
                                            (property) =>
                                                new ErrorDescription(
                                                    $" * {property._restorationId}"
                                                )
                                        )
                                    );
                                    return __collection41817;
                                }
                            )
                        )()
                    )
                );
            }
            _debugPropertiesWaitingForReregistration = null;
            return true;
        });
    }

    public virtual bool _updateBucketIfNecessary(RestorationBucket? parent, bool restorePending)
    {
        if ((restorationId is null) || (parent is null))
        {
            bool didReplace = _setNewBucketIfNecessary(
                newBucket: null,
                restorePending: restorePending
            );
            DartRuntimePrimitives.Assert(() => _bucket is null);
            return didReplace;
        }
        DartRuntimePrimitives.Assert(() => restorationId is not null);
        if (restorePending || (_bucket is null))
        {
            RestorationBucket newBucketLocal = parent.claimChild(restorationId!, debugOwner: this);
            bool didReplaceLocal = _setNewBucketIfNecessary(
                newBucket: newBucketLocal,
                restorePending: restorePending
            );
            DartRuntimePrimitives.Assert(() => Equals(_bucket, newBucketLocal));
            return didReplaceLocal;
        }
        DartRuntimePrimitives.Assert(() => _bucket is not null);
        DartRuntimePrimitives.Assert(() => !restorePending);
        _bucket!.rename(restorationId!);
        parent.adoptChild(_bucket!);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _setNewBucketIfNecessary(RestorationBucket? newBucket, bool restorePending)
    {
        if (Equals(newBucket, _bucket))
        {
            return false;
        }
        RestorationBucket? oldBucket = _bucket;
        _bucket = newBucket;
        if (!restorePending)
        {
            if (_bucket is not null)
            {
                _properties.Keys.forEach(
                    (__arg0) => ((Action<IRestorableProperty>)_updateProperty)(__arg0)
                );
            }
            didToggleBucket(oldBucket);
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _updateProperty(IRestorableProperty property)
    {
        if (property.enabled)
        {
            _bucket?.write(property._restorationId!, property.toPrimitives());
        }
        else
        {
            _bucket?.remove<object>(property._restorationId!);
        }
    }

    public virtual void _unregister(IRestorableProperty property)
    {
        Action listener = _properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
        {
            _debugPropertiesWaitingForReregistration?.Remove(property);
            return true;
        });
        property.removeListener(listener);
        property._unregister();
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
}

public enum _BaselineAlignedStackSlot__text_field
{
    placeholder,
    editableText,
}

internal class _BaselineAlignedStack__text_field
    : SlottedMultiChildRenderObjectWidget<_BaselineAlignedStackSlot__text_field, RenderBox>
{
    public virtual TextBaseline editableTextBaseline { get; private set; } = default!;
    public virtual TextBaseline placeholderBaseline { get; private set; } = default!;
    public virtual TextAlignVertical textAlignVertical { get; private set; } = default!;
    public virtual Widget editableText { get; private set; } = default!;
    public virtual Widget? placeholder { get; private set; }

    internal _BaselineAlignedStack__text_field(
        TextBaseline editableTextBaseline,
        TextBaseline placeholderBaseline,
        TextAlignVertical textAlignVertical,
        Widget editableText,
        Widget? placeholder = null
    )
    {
        this.editableTextBaseline = editableTextBaseline;
        this.placeholderBaseline = placeholderBaseline;
        this.textAlignVertical = textAlignVertical;
        this.editableText = editableText;
        this.placeholder = placeholder;
    }

    public override IEnumerable<_BaselineAlignedStackSlot__text_field> slots =>
        DartRuntimePrimitives.ConvertValue<IEnumerable<_BaselineAlignedStackSlot__text_field>>(
            Enum.GetValues<_BaselineAlignedStackSlot__text_field>().ToList()
        );

    public override Widget? childForSlot(_BaselineAlignedStackSlot__text_field slot)
    {
        return slot switch
        {
            _BaselineAlignedStackSlot__text_field.placeholder => placeholder,
            _BaselineAlignedStackSlot__text_field.editableText => editableText,
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderBaselineAlignedStack__text_field(
            textAlignVertical: textAlignVertical,
            editableTextBaseline: editableTextBaseline,
            placeholderBaseline: placeholderBaseline
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderBaselineAlignedStack__text_field)renderObject;
        DartRuntimePrimitives.Ignore(
            (
                (Func<_RenderBaselineAlignedStack__text_field>)(
                    () =>
                    {
                        var __cascade = __renderObject;
                        __cascade.textAlignVertical = textAlignVertical;
                        __cascade.editableTextBaseline = editableTextBaseline;
                        __cascade.placeholderBaseline = placeholderBaseline;
                        return __cascade;
                    }
                )
            )()
        );
    }
}

internal class _BaselineAlignedStackParentData__text_field : ContainerBoxParentData<RenderBox> { }

public class _RenderBaselineAlignedStack__text_field
    : RenderBox,
        SlottedContainerRenderObjectMixin<_BaselineAlignedStackSlot__text_field, RenderBox>
{
    internal virtual TextAlignVertical _textAlignVertical { get; set; } = default!;
    internal virtual TextBaseline _editableTextBaseline { get; set; } = default!;
    internal virtual TextBaseline _placeholderBaseline { get; set; } = default!;
    public virtual DartMap<
        _BaselineAlignedStackSlot__text_field,
        RenderBox
    > _slotToChild { get; set; } = new DartMap<_BaselineAlignedStackSlot__text_field, RenderBox>();

    internal _RenderBaselineAlignedStack__text_field(
        TextAlignVertical textAlignVertical,
        TextBaseline editableTextBaseline,
        TextBaseline placeholderBaseline
    )
    {
        _textAlignVertical = textAlignVertical;
        _editableTextBaseline = editableTextBaseline;
        _placeholderBaseline = placeholderBaseline;
    }

    public virtual TextAlignVertical textAlignVertical
    {
        get => _textAlignVertical;
        set
        {
            var __value = value;
            if (Equals(_textAlignVertical, __value))
            {
                return;
            }
            _textAlignVertical = __value;
            markNeedsLayout();
        }
    }
    public virtual TextBaseline editableTextBaseline
    {
        get => _editableTextBaseline;
        set
        {
            var __value = value;
            if (Equals(_editableTextBaseline, __value))
            {
                return;
            }
            _editableTextBaseline = __value;
            markNeedsLayout();
        }
    }
    public virtual TextBaseline placeholderBaseline
    {
        get => _placeholderBaseline;
        set
        {
            var __value = value;
            if (Equals(_placeholderBaseline, __value))
            {
                return;
            }
            _placeholderBaseline = __value;
            markNeedsLayout();
        }
    }

    public override void setupParentData(RenderObject child)
    {
        var __child = (RenderBox)child;
        if (__child.parentData is not _BaselineAlignedStackParentData__text_field)
        {
            __child.parentData = new _BaselineAlignedStackParentData__text_field();
        }
    }

    internal virtual RenderBox? _placeholderChild
    {
        get { return childForSlot(_BaselineAlignedStackSlot__text_field.placeholder); }
    }
    internal virtual RenderBox _editableTextChild
    {
        get
        {
            RenderBox? child = childForSlot(_BaselineAlignedStackSlot__text_field.editableText);
            DartRuntimePrimitives.Assert(() => child is not null);
            return child!;
        }
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return Math.Max(
            _placeholderChild?.getMinIntrinsicHeight(width) ?? 0.0,
            _editableTextChild.getMinIntrinsicHeight(width)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return Math.Max(
            _placeholderChild?.getMaxIntrinsicHeight(width) ?? 0.0,
            _editableTextChild.getMaxIntrinsicHeight(width)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        return Math.Max(
            _placeholderChild?.getMinIntrinsicWidth(height) ?? 0.0,
            _editableTextChild.getMinIntrinsicWidth(height)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        return Math.Max(
            _placeholderChild?.getMaxIntrinsicWidth(height) ?? 0.0,
            _editableTextChild.getMaxIntrinsicWidth(height)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        DartRuntimePrimitives.Assert(() => constraints.hasTightWidth);
        RenderBox? placeholder = _placeholderChild;
        RenderBox editableText = _editableTextChild;
        var editableTextParentData = (
            (_BaselineAlignedStackParentData__text_field?)editableText.parentData!
        )!;
        var placeholderParentData = (
            (_BaselineAlignedStackParentData__text_field?)placeholder?.parentData
        )!;
        size = _computeSize(
            constraints: constraints,
            layoutChild: ChildLayoutHelper.layoutChild,
            getBaseline: ChildLayoutHelper.getBaseline
        );
        double editableTextBaselineValue = (
            editableText.getDistanceToBaseline(editableTextBaseline)
            ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
        );
        double? placeholderBaselineValue = placeholder?.getDistanceToBaseline(placeholderBaseline);
        DartRuntimePrimitives.Assert(() =>
            (placeholder is not null) || (placeholderBaselineValue is null)
        );
        Offset baselineDiff =
            (placeholderBaselineValue is not null)
                ? new Offset(
                    0.0,
                    editableTextBaselineValue
                        - (
                            placeholderBaselineValue
                            ?? throw new global::System.NullReferenceException(
                                "Dart null assertion failed."
                            )
                        )
                )
                : Offset.zero;
        var verticalAlignment = new Alignment(0.0, textAlignVertical.y);
        editableTextParentData.offset = verticalAlignment.alongOffset(size - editableText.size);
        placeholderParentData?.offset = editableTextParentData.offset + baselineDiff;
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        RenderBox? placeholder = _placeholderChild;
        RenderBox editableText = _editableTextChild;
        if (placeholder is not null)
        {
            var placeholderParentData = (
                (_BaselineAlignedStackParentData__text_field?)placeholder.parentData!
            )!;
            context.paintChild(placeholder, offset + placeholderParentData.offset);
        }
        var editableTextParentData = (
            (_BaselineAlignedStackParentData__text_field?)editableText.parentData!
        )!;
        context.paintChild(editableText, offset + editableTextParentData.offset);
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return _computeSize(
            constraints: constraints,
            layoutChild: ChildLayoutHelper.dryLayoutChild,
            getBaseline: ChildLayoutHelper.getDryBaseline
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Size _computeSize(
        BoxConstraints constraints,
        Func<RenderBox, BoxConstraints, Size> layoutChild,
        Func<RenderBox, BoxConstraints, TextBaseline, double?> getBaseline
    )
    {
        double widthLocal = constraints.minWidth;
        double heightLocal = constraints.minHeight;
        RenderBox editableText = _editableTextChild;
        Size editableTextSize = layoutChild(editableText, constraints);
        double editableTextBaselineValue = (
            getBaseline(editableText, constraints, editableTextBaseline)
            ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
        );
        double editableTextDescent = editableTextSize.height - editableTextBaselineValue;
        Size? placeholderSize = default!;
        double? placeholderBaselineValue = default!;
        RenderBox? placeholder = _placeholderChild;
        if (placeholder is not null)
        {
            placeholderSize = layoutChild(placeholder, constraints);
            widthLocal = Math.Max(widthLocal, placeholderSize.width);
            placeholderBaselineValue = getBaseline(placeholder, constraints, placeholderBaseline);
            double placeholderDescent =
                placeholderSize.height
                - (
                    placeholderBaselineValue
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                );
            double maxExtentBaseline =
                Math.Max(
                    editableTextBaselineValue,
                    (
                        placeholderBaselineValue
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    )
                ) + Math.Max(editableTextDescent, placeholderDescent);
            heightLocal = Math.Max(heightLocal, maxExtentBaseline);
        }
        heightLocal = Math.Max(heightLocal, editableTextSize.height);
        widthLocal = Math.Max(widthLocal, editableTextSize.width);
        var size = new Size(widthLocal, heightLocal);
        DartRuntimePrimitives.Assert(() => size.isFinite);
        return constraints.constrain(size);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox editableText = _editableTextChild;
        var editableTextParentData = (
            (_BaselineAlignedStackParentData__text_field?)editableText.parentData!
        )!;
        return result.addWithPaintOffset(
            offset: editableTextParentData.offset,
            position: position,
            hitTest: (result, transformed) =>
            {
                DartRuntimePrimitives.Assert(() =>
                    Equals(transformed, position - editableTextParentData.offset)
                );
                return editableText.hitTest(result, position: transformed);
                throw new InvalidOperationException("Dart closure completed without a value.");
            }
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childForSlot(_BaselineAlignedStackSlot__text_field slot) =>
        _slotToChild.GetValueOrDefault(slot);

    public virtual IEnumerable<RenderBox> children => _slotToChild.Values;

    public virtual string debugNameForSlot(_BaselineAlignedStackSlot__text_field slot)
    {
        return slot.ToString();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        foreach (RenderBox child in children)
        {
            child.attach(owner);
        }
    }

    public override void detach()
    {
        base.detach();
        foreach (RenderBox child in children)
        {
            child.detach();
        }
    }

    public override void redepthChildren()
    {
        children.forEach(
            (__arg0) =>
                ((Action<RenderObject>)redepthChild)(
                    DartRuntimePrimitives.ConvertValue<RenderObject>(__arg0)
                )
        );
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        children.forEach(
            (__arg0) => visitor(DartRuntimePrimitives.ConvertValue<RenderObject>(__arg0))
        );
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        var value = new List<DiagnosticsNode>();
        var childToSlot = new DartMap<RenderBox, _BaselineAlignedStackSlot__text_field>(
            _slotToChild.Values,
            _slotToChild.Keys
        );
        foreach (RenderBox child in children)
        {
            _addDiagnostics(
                child,
                value,
                debugNameForSlot(
                    (
                        DartCollectionRuntime.NullableMapValue<_BaselineAlignedStackSlot__text_field>(
                            childToSlot,
                            child
                        )
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    )
                )
            );
        }
        return value;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _addDiagnostics(RenderBox child, List<DiagnosticsNode> value, string name)
    {
        value.Add(((Diagnosticable)child).toDiagnosticsNode(name: name));
    }

    public virtual void _setChild(RenderBox? child, _BaselineAlignedStackSlot__text_field slot)
    {
        RenderBox? oldChild = _slotToChild.GetValueOrDefault(slot);
        if (oldChild is not null)
        {
            dropChild(oldChild);
            _slotToChild.remove(slot);
        }
        if (child is not null)
        {
            _slotToChild[slot] = child;
            adoptChild(child);
        }
    }

    public virtual void _moveChild(
        RenderBox child,
        _BaselineAlignedStackSlot__text_field slot,
        _BaselineAlignedStackSlot__text_field oldSlot
    )
    {
        DartRuntimePrimitives.Assert(() => !Equals(slot, oldSlot));
        RenderBox? oldChild = _slotToChild.GetValueOrDefault(oldSlot);
        if (Equals(oldChild, child))
        {
            _setChild(null, oldSlot);
        }
        _setChild(child, slot);
    }
}
