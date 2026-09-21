// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/text_field.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public delegate Widget? InputCounterWidgetBuilder(
    BuildContext context,
    long currentLength,
    bool isFocused,
    long? maxLength
);

internal class _TextFieldSelectionGestureDetectorBuilder__text_field
    : TextSelectionGestureDetectorBuilder
{
    internal virtual _TextFieldState__text_field _state { get; private set; } = default!;

    internal _TextFieldSelectionGestureDetectorBuilder__text_field(
        _TextFieldState__text_field state
    )
        : base(@delegate: state)
    {
        _state = state;
    }

    public override bool onUserTapAlwaysCalled => _state.widget.onTapAlwaysCalled;

    public override void onUserTap()
    {
        _state.widget.onTap?.Invoke();
    }
}

public class TextField : StatefulWidget
{
    public virtual TextMagnifierConfiguration? magnifierConfiguration { get; private set; }
    public virtual object groupId { get; private set; } = default!;
    public virtual TextEditingController? controller { get; private set; }
    public virtual FocusNode? focusNode { get; private set; }
    public virtual InputDecoration? decoration { get; private set; }
    public virtual TextInputType keyboardType { get; private set; } = default!;
    public virtual TextInputAction? textInputAction { get; private set; }
    public virtual TextCapitalization textCapitalization { get; private set; } = default!;
    public virtual TextStyle? style { get; private set; }
    public virtual Painting.StrutStyle? strutStyle { get; private set; }
    public virtual TextAlign textAlign { get; private set; } = default!;
    public virtual TextAlignVertical? textAlignVertical { get; private set; }
    public virtual TextDirection? textDirection { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual WidgetStatesController? statesController { get; private set; }
    public virtual string obscuringCharacter { get; private set; } = default!;
    public virtual bool obscureText { get; private set; } = default!;
    public virtual bool? autocorrect { get; private set; }
    public virtual SmartDashesType smartDashesType { get; private set; } = default!;
    public virtual SmartQuotesType smartQuotesType { get; private set; } = default!;
    public virtual bool enableSuggestions { get; private set; } = default!;
    public virtual long? maxLines { get; private set; }
    public virtual long? minLines { get; private set; }
    public virtual bool expands { get; private set; } = default!;
    public virtual bool readOnly { get; private set; } = default!;
    public virtual ToolbarOptions? toolbarOptions { get; private set; }
    public virtual bool? showCursor { get; private set; }
    public static long noMaxLength = -1L;
    public virtual long? maxLength { get; private set; }
    public virtual MaxLengthEnforcement? maxLengthEnforcement { get; private set; }
    public virtual Action<string>? onChanged { get; private set; }
    public virtual Action? onEditingComplete { get; private set; }
    public virtual Action<string>? onSubmitted { get; private set; }
    public virtual Action<string, DartMap<string, object?>>? onAppPrivateCommand
    {
        get;
        private set;
    }
    public virtual List<TextInputFormatter>? inputFormatters { get; private set; }
    public virtual bool? enabled { get; private set; }
    public virtual bool? ignorePointers { get; private set; }
    public virtual double cursorWidth { get; private set; } = default!;
    public virtual double? cursorHeight { get; private set; }
    public virtual Radius? cursorRadius { get; private set; }
    public virtual bool? cursorOpacityAnimates { get; private set; }
    public virtual Color? cursorColor { get; private set; }
    public virtual Color? cursorErrorColor { get; private set; }
    public virtual BoxHeightStyle? selectionHeightStyle { get; private set; }
    public virtual BoxWidthStyle? selectionWidthStyle { get; private set; }
    public virtual Brightness? keyboardAppearance { get; private set; }
    public virtual EdgeInsets scrollPadding { get; private set; } = default!;
    public virtual bool enableInteractiveSelection { get; private set; } = default!;
    public virtual bool? selectAllOnFocus { get; private set; }
    public virtual TextSelectionControls? selectionControls { get; private set; }
    public virtual Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual Action? onTap { get; private set; }
    public virtual bool onTapAlwaysCalled { get; private set; } = default!;
    public virtual Action<Gestures.PointerDownEvent>? onTapOutside { get; private set; }
    public virtual Action<Gestures.PointerUpEvent>? onTapUpOutside { get; private set; }
    public virtual MouseCursor? mouseCursor { get; private set; }
    public virtual InputCounterWidgetBuilder? buildCounter { get; private set; }
    public virtual ScrollPhysics? scrollPhysics { get; private set; }
    public virtual ScrollController? scrollController { get; private set; }
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
    public virtual bool canRequestFocus { get; private set; } = default!;
    public virtual UndoHistoryController? undoController { get; private set; }
    public virtual List<Locale>? hintLocales { get; private set; }
    public virtual SpellCheckConfiguration? spellCheckConfiguration { get; private set; }
    public static TextStyle materialMisspelledTextStyle = new TextStyle(
        decoration: TextDecoration.underline,
        decorationColor: Colors.red,
        decorationStyle: TextDecorationStyle.wavy
    );

    public TextField(
        Key? key = null,
        object groupId = default!,
        TextEditingController? controller = null,
        FocusNode? focusNode = null,
        UndoHistoryController? undoController = null,
        InputDecoration? decoration = default!,
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
        WidgetStatesController? statesController = null,
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
        Action<string, DartMap<string, object?>>? onAppPrivateCommand = null,
        List<TextInputFormatter>? inputFormatters = null,
        bool? enabled = null,
        bool? ignorePointers = null,
        double cursorWidth = 2.0,
        double? cursorHeight = null,
        Radius? cursorRadius = null,
        bool? cursorOpacityAnimates = null,
        Color? cursorColor = null,
        Color? cursorErrorColor = null,
        BoxHeightStyle? selectionHeightStyle = null,
        BoxWidthStyle? selectionWidthStyle = null,
        Brightness? keyboardAppearance = null,
        EdgeInsets scrollPadding = default!,
        Gestures.DragStartBehavior dragStartBehavior = Gestures.DragStartBehavior.start,
        bool? enableInteractiveSelection = null,
        bool? selectAllOnFocus = null,
        TextSelectionControls? selectionControls = null,
        Action? onTap = null,
        bool onTapAlwaysCalled = false,
        Action<Gestures.PointerDownEvent>? onTapOutside = null,
        Action<Gestures.PointerUpEvent>? onTapUpOutside = null,
        MouseCursor? mouseCursor = null,
        InputCounterWidgetBuilder? buildCounter = null,
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
        bool canRequestFocus = true,
        SpellCheckConfiguration? spellCheckConfiguration = null,
        TextMagnifierConfiguration? magnifierConfiguration = null,
        List<Locale>? hintLocales = null
    )
        : base(key: key)
    {
        object __groupId = groupId ?? typeof(EditableText);
        InputDecoration? __decoration = decoration ?? new InputDecoration();
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
        this.statesController = statesController;
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
        this.onAppPrivateCommand = onAppPrivateCommand;
        this.inputFormatters = inputFormatters;
        this.enabled = enabled;
        this.ignorePointers = ignorePointers;
        this.cursorWidth = cursorWidth;
        this.cursorHeight = cursorHeight;
        this.cursorRadius = cursorRadius;
        this.cursorOpacityAnimates = cursorOpacityAnimates;
        this.cursorColor = cursorColor;
        this.cursorErrorColor = cursorErrorColor;
        this.selectionHeightStyle = selectionHeightStyle;
        this.selectionWidthStyle = selectionWidthStyle;
        this.keyboardAppearance = keyboardAppearance;
        this.scrollPadding = __scrollPadding;
        this.dragStartBehavior = dragStartBehavior;
        this.selectAllOnFocus = selectAllOnFocus;
        this.selectionControls = selectionControls;
        this.onTap = onTap;
        this.onTapAlwaysCalled = onTapAlwaysCalled;
        this.onTapOutside = onTapOutside;
        this.onTapUpOutside = onTapUpOutside;
        this.mouseCursor = mouseCursor;
        this.buildCounter = buildCounter;
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
        this.canRequestFocus = canRequestFocus;
        this.spellCheckConfiguration = spellCheckConfiguration;
        this.magnifierConfiguration = magnifierConfiguration;
        this.hintLocales = hintLocales;
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
                            "A required value was null."
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
                            "A required value was null."
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
                            "A required value was null."
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
                            "A required value was null."
                        )
                    ) == noMaxLength
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
        System.Diagnostics.Debug.Assert(
            !DartRuntimePrimitives.Identical(textInputAction, TextInputAction.newline)
                || (maxLines == 1L)
                || !DartRuntimePrimitives.Identical(keyboardType, TextInputType.text)
        );
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
        return AdaptiveTextSelectionToolbar.CreateEditableText(
            editableTextState: editableTextState
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static Widget defaultSpellCheckSuggestionsToolbarBuilder(
        BuildContext context,
        EditableTextState editableTextState
    )
    {
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.iOS:
            case TargetPlatform.macOS:
            {
                return CupertinoSpellCheckSuggestionsToolbar.CreateEditableText(
                    editableTextState: editableTextState
                );
            }
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
            {
                return SpellCheckSuggestionsToolbar.CreateEditableText(
                    editableTextState: editableTextState
                );
            }
            default:
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                );
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static SpellCheckConfiguration inferAndroidSpellCheckConfiguration(
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
            misspelledTextStyle: configuration.misspelledTextStyle ?? materialMisspelledTextStyle,
            spellCheckSuggestionsToolbarBuilder: configuration.spellCheckSuggestionsToolbarBuilder
                ?? defaultSpellCheckSuggestionsToolbarBuilder
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _TextFieldState__text_field());

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
        properties.add(new DiagnosticsProperty<bool>("enabled", enabled, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<InputDecoration>(
                "decoration",
                decoration,
                defaultValue: new InputDecoration()
            )
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
        properties.add(
            new EnumProperty<TextInputAction>(
                "textInputAction",
                textInputAction,
                defaultValue: null
            )
        );
        properties.add(
            new EnumProperty<TextCapitalization>(
                "textCapitalization",
                textCapitalization,
                defaultValue: TextCapitalization.none
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
        properties.add(new DoubleProperty("cursorWidth", cursorWidth, defaultValue: 2.0));
        properties.add(new DoubleProperty("cursorHeight", cursorHeight, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<Radius>("cursorRadius", cursorRadius, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<bool>(
                "cursorOpacityAnimates",
                cursorOpacityAnimates,
                defaultValue: null
            )
        );
        properties.add(new ColorProperty("cursorColor", cursorColor, defaultValue: null));
        properties.add(new ColorProperty("cursorErrorColor", cursorErrorColor, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<Brightness>(
                "keyboardAppearance",
                keyboardAppearance,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<EdgeInsetsGeometry>(
                "scrollPadding",
                scrollPadding,
                defaultValue: EdgeInsets.CreateAll(20.0)
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
        properties.add(
            new DiagnosticsProperty<List<Locale>?>("hintLocales", hintLocales, defaultValue: null)
        );
    }
}

internal class _TextFieldState__text_field
    : State<TextField>,
        RestorationMixin<TextField>,
        TextSelectionGestureDetectorBuilderDelegate,
        AutofillClient
{
    internal virtual RestorableTextEditingController? _controller { get; set; } = default;
    internal virtual FocusNode? _focusNode { get; set; } = default;
    internal virtual bool _isHovering { get; set; } = false;
    internal virtual bool _showSelectionHandles { get; set; } = false;
    internal virtual _TextFieldSelectionGestureDetectorBuilder__text_field _selectionGestureDetectorBuilder { get; set; } =
        default!;
    public virtual bool forcePressEnabled { get; set; } = default!;
    public virtual GlobalKey<EditableTextState> editableTextKey { get; private set; } =
        GlobalKey<EditableTextState>.Create();
    internal virtual WidgetStatesController? _internalStatesController { get; set; } = default;
    public virtual RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<IRestorableProperty, Action> _properties { get; set; } =
        new DartMap<IRestorableProperty, Action>();
    public virtual List<IRestorableProperty>? _debugPropertiesWaitingForReregistration { get; set; } =
        default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual RestorationBucket? _currentParent { get; set; } = default;

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
                ?? LengthLimitingTextInputFormatter.getDefaultMaxLengthEnforcement(
                    Theme.of(context).platform
                )
        );
    public virtual bool needsCounter =>
        DartRuntimePrimitives.ConvertValue<bool>(
            (widget.maxLength is not null)
                && (widget.decoration is not null)
                && (widget.decoration!.counterText is null)
        );
    public virtual bool selectionEnabled =>
        DartRuntimePrimitives.ConvertValue<bool>(widget.selectionEnabled && _isEnabled);
    internal virtual bool _isEnabled =>
        DartRuntimePrimitives.ConvertValue<bool>(
            (widget.enabled ?? widget.decoration?.enabled) ?? true
        );
    internal virtual long _currentLength => _effectiveController.value.text.characters().Count;
    internal virtual bool _hasIntrinsicError =>
        DartRuntimePrimitives.ConvertValue<bool>(
            (widget.maxLength is not null)
                && (
                    (
                        widget.maxLength
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ) > 0L
                )
                && (
                    (widget.controller is null)
                        ? (
                            !restorePending
                            && (
                                _effectiveController.value.text.characters().Count
                                > (
                                    widget.maxLength
                                    ?? throw new global::System.NullReferenceException(
                                        "A required value was null."
                                    )
                                )
                            )
                        )
                        : (
                            _effectiveController.value.text.characters().Count
                            > (
                                widget.maxLength
                                ?? throw new global::System.NullReferenceException(
                                    "A required value was null."
                                )
                            )
                        )
                )
        );
    internal virtual bool _hasError =>
        DartRuntimePrimitives.ConvertValue<bool>(
            (widget.decoration?.errorText is not null)
                || (widget.decoration?.error is not null)
                || _hasIntrinsicError
        );
    internal virtual Color _errorColor =>
        DartRuntimePrimitives.ConvertValue<Color>(
            (widget.cursorErrorColor ?? _getEffectiveDecoration().errorStyle?.color)
                ?? Theme.of(context).colorScheme.error
        );

    internal virtual InputDecoration _getEffectiveDecoration()
    {
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        ThemeData themeData = Theme.of(context);
        InputDecorationThemeData decorationTheme = InputDecorationTheme.of(context);
        InputDecoration effectiveDecoration = (widget.decoration ?? new InputDecoration())
            .applyDefaults(decorationTheme)
            .copyWith(
                enabled: _isEnabled,
                hintMaxLines: (widget.decoration?.hintMaxLines ?? decorationTheme.hintMaxLines)
                    ?? widget.maxLines
            );
        if (
            (effectiveDecoration.counter is not null)
            || (effectiveDecoration.counterText is not null)
        )
        {
            return effectiveDecoration;
        }
        Widget? counterLocal = default!;
        long currentLengthLocal = _currentLength;
        if (
            (effectiveDecoration.counter is null)
            && (effectiveDecoration.counterText is null)
            && (widget.buildCounter is not null)
        )
        {
            bool isFocusedLocal = _effectiveFocusNode.hasFocus;
            Widget? builtCounter = widget.buildCounter!(
                context,
                currentLength: currentLengthLocal,
                maxLength: widget.maxLength,
                isFocused: isFocusedLocal
            );
            if (builtCounter is not null)
            {
                counterLocal = DartRuntimePrimitives.ConvertValue<Widget>(
                    new Widgets.Semantics(
                        container: true,
                        liveRegion: isFocusedLocal,
                        child: builtCounter
                    )
                );
            }
            return effectiveDecoration.copyWith(counter: counterLocal);
        }
        if (widget.maxLength is null)
        {
            return effectiveDecoration;
        }
        var counterTextLocal = $"{currentLengthLocal}";
        var semanticCounterTextLocal = "";
        if (
            (
                widget.maxLength
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ) > 0L
        )
        {
            counterTextLocal += $"/{widget.maxLength}";
            long remaining = (
                (
                    widget.maxLength
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ) - currentLengthLocal
            ).clamp(
                0L,
                (
                    widget.maxLength
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
            );
            semanticCounterTextLocal = localizations.remainingTextFieldCharacterCount(remaining);
        }
        if (_hasIntrinsicError)
        {
            return effectiveDecoration.copyWith(
                errorText: effectiveDecoration.errorText ?? "",
                counterStyle: effectiveDecoration.errorStyle
                    ?? Text_fieldLibrary._m3CounterErrorStyle(context),
                counterText: counterTextLocal,
                semanticCounterText: semanticCounterTextLocal
            );
        }
        return effectiveDecoration.copyWith(
            counterText: counterTextLocal,
            semanticCounterText: semanticCounterTextLocal
        );
    }

    public override void initState()
    {
        base.initState();
        _selectionGestureDetectorBuilder =
            new _TextFieldSelectionGestureDetectorBuilder__text_field(state: this);
        if (widget.controller is null)
        {
            _createLocalController();
        }
        _effectiveFocusNode.canRequestFocus = widget.canRequestFocus && _isEnabled;
        _effectiveFocusNode.addListener(_handleFocusChanged);
        _initStatesController();
    }

    internal virtual bool _canRequestFocus
    {
        get
        {
            NavigationMode mode =
                MediaQuery.maybeNavigationModeOf(context) ?? NavigationMode.traditional;
            return mode switch
            {
                NavigationMode.traditional => widget.canRequestFocus && _isEnabled,
                NavigationMode.directional => true,
                _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                    throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    ),
            };
        }
    }

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
        _effectiveFocusNode.canRequestFocus = _canRequestFocus;
    }

    public override void didUpdateWidget(TextField oldWidget)
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
                unregisterFromRestoration(
                    DartRuntimePrimitives.ConvertValue<RestorableProperty<object>>(_controller!)
                );
                _controller!.dispose();
                _controller = null;
            }
        }
        if (!Equals(widget.focusNode, oldWidget.focusNode))
        {
            (oldWidget.focusNode ?? _focusNode)?.removeListener(_handleFocusChanged);
            (widget.focusNode ?? _focusNode)?.addListener(_handleFocusChanged);
        }
        _effectiveFocusNode.canRequestFocus = _canRequestFocus;
        if (_effectiveFocusNode.hasFocus && (widget.readOnly != oldWidget.readOnly) && _isEnabled)
        {
            if (_effectiveController.selection.isCollapsed)
            {
                _showSelectionHandles = !widget.readOnly;
            }
        }
        if (Equals(widget.statesController, oldWidget.statesController))
        {
            _statesController.update(WidgetState.disabled, !_isEnabled);
            _statesController.update(WidgetState.hovered, _isHovering);
            _statesController.update(WidgetState.focused, _effectiveFocusNode.hasFocus);
            _statesController.update(WidgetState.error, _hasError);
        }
        else
        {
            oldWidget.statesController?.removeListener(_handleStatesControllerChange);
            if (widget.statesController is not null)
            {
                _internalStatesController?.dispose();
                _internalStatesController = null;
            }
            _initStatesController();
        }
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
        _statesController.removeListener(_handleStatesControllerChange);
        _internalStatesController?.dispose();
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

    internal virtual EditableTextState? _editableText => editableTextKey.currentState;

    internal virtual void _requestKeyboard()
    {
        _editableText?.requestKeyboard();
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
        if (Equals(cause, SelectionChangedCause.keyboard))
        {
            return false;
        }
        if (widget.readOnly && _effectiveController.selection.isCollapsed)
        {
            return false;
        }
        if (!_isEnabled)
        {
            return false;
        }
        if (
            Equals(cause, SelectionChangedCause.longPress)
            || Equals(cause, SelectionChangedCause.stylusHandwriting)
        )
        {
            return true;
        }
        if (_effectiveController.text.Length != 0)
        {
            return true;
        }
        return false;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _handleFocusChanged()
    {
        setState(() => { });
        _statesController.update(WidgetState.focused, _effectiveFocusNode.hasFocus);
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
        switch (Theme.of(context).platform)
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
                    _editableText?.bringIntoView(selection.extent);
                }
                break;
            }
        }
        switch (Theme.of(context).platform)
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
                    _editableText?.hideToolbar();
                }
                break;
            }
        }
    }

    internal virtual void _handleSelectionHandleTapped()
    {
        if (_effectiveController.selection.isCollapsed)
        {
            _editableText!.toggleToolbar();
        }
    }

    internal virtual void _handleHover(bool hovering)
    {
        if (hovering != _isHovering)
        {
            setState(() =>
            {
                _isHovering = hovering;
            });
            _statesController.update(WidgetState.hovered, _isHovering);
        }
    }

    internal virtual void _handleStatesControllerChange()
    {
        setState(() => { });
    }

    internal virtual WidgetStatesController _statesController =>
        DartRuntimePrimitives.ConvertValue<WidgetStatesController>(
            widget.statesController ?? _internalStatesController!
        );

    internal virtual void _initStatesController()
    {
        if (widget.statesController is null)
        {
            _internalStatesController = new WidgetStatesController();
        }
        _statesController.update(WidgetState.disabled, !_isEnabled);
        _statesController.update(WidgetState.hovered, _isHovering);
        _statesController.update(WidgetState.focused, _effectiveFocusNode.hasFocus);
        _statesController.update(WidgetState.error, _hasError);
        _statesController.addListener(_handleStatesControllerChange);
    }

    public virtual string autofillId => _editableText!.autofillId;

    public virtual void autofill(TextEditingValue newEditingValue) =>
        _editableText!.autofill(newEditingValue);

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
                        hintText: (widget.decoration ?? new InputDecoration()).hintText
                    )
                    : AutofillConfiguration.disabled;
            return _editableText!.textInputConfiguration.copyWith(
                autofillConfiguration: autofillConfigurationLocal
            );
        }
    }

    internal virtual TextStyle _getInputStyleForState(TextStyle style)
    {
        ThemeData theme = Theme.of(context);
        TextStyle stateStyle = WidgetStateProperty.resolveAs(
            Text_fieldLibrary._m3StateInputStyle(context)!,
            _statesController.value
        );
        TextStyle providedStyle = WidgetStateProperty.resolveAs(style, _statesController.value);
        return providedStyle.merge(stateStyle);
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        DartRuntimePrimitives.Assert(() =>
            DebugLibrary.debugCheckHasMaterialLocalizations(context)
        );
        DartRuntimePrimitives.Assert(() =>
            Widgets.DebugLibrary.debugCheckHasDirectionality(context)
        );
        DartRuntimePrimitives.Assert(
            () =>
                !(
                    (widget.style is not null)
                    && !widget.style!.inherit
                    && ((widget.style!.fontSize is null) || (widget.style!.textBaseline is null))
                ),
            () => (object?)"inherit false style must supply fontSize and textBaseline"
        );
        ThemeData theme = Theme.of(context);
        DefaultSelectionStyle selectionStyle = DefaultSelectionStyle.of(context);
        TextStyle? providedStyle = WidgetStateProperty.resolveAs(
            widget.style,
            _statesController.value
        );
        TextStyle styleLocal = _getInputStyleForState(Text_fieldLibrary._m3InputStyle(context))
            .merge(providedStyle);
        Brightness keyboardAppearanceLocal = widget.keyboardAppearance ?? theme.brightness;
        TextEditingController controllerLocal = _effectiveController;
        FocusNode focusNodeLocal = _effectiveFocusNode;
        var formatters = (
            (Func<List<TextInputFormatter>>)(
                () =>
                {
                    var __collection58738 = new List<TextInputFormatter>();
                    var __collectionSpread58766 = widget.inputFormatters;
                    if (__collectionSpread58766 is not null)
                    {
                        __collection58738.AddRange(__collectionSpread58766);
                    }
                    if (widget.maxLength is not null)
                    {
                        __collection58738.Add(
                            new LengthLimitingTextInputFormatter(
                                widget.maxLength,
                                maxLengthEnforcement: _effectiveMaxLengthEnforcement
                            )
                        );
                    }
                    return __collection58738;
                }
            )
        )();
        SpellCheckConfiguration spellCheckConfigurationLocal = default!;
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.iOS:
            case TargetPlatform.macOS:
            {
                spellCheckConfigurationLocal = CupertinoTextField.inferIOSSpellCheckConfiguration(
                    widget.spellCheckConfiguration
                );
                break;
            }
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
            {
                spellCheckConfigurationLocal = TextField.inferAndroidSpellCheckConfiguration(
                    widget.spellCheckConfiguration
                );
                break;
            }
        }
        TextSelectionControls? textSelectionControls = widget.selectionControls;
        bool paintCursorAboveTextLocal = default!;
        bool? cursorOpacityAnimatesLocal = widget.cursorOpacityAnimates;
        Offset? cursorOffsetLocal = default!;
        Color cursorColorLocal = default!;
        Color selectionColorLocal = default!;
        Color? autocorrectionTextRectColorLocal = default!;
        Radius? cursorRadiusLocal = widget.cursorRadius;
        Action? handleDidGainAccessibilityFocus = default!;
        Action? handleDidLoseAccessibilityFocus = default!;
        switch (theme.platform)
        {
            case TargetPlatform.iOS:
            {
                CupertinoThemeData cupertinoTheme = CupertinoTheme.of(context);
                forcePressEnabled = true;
                textSelectionControls ??= Cupertino
                    .Text_selectionLibrary
                    .cupertinoTextSelectionHandleControls;
                paintCursorAboveTextLocal = true;
                cursorOpacityAnimatesLocal ??= true;
                cursorColorLocal = _hasError
                    ? _errorColor
                    : (
                        (widget.cursorColor ?? selectionStyle.cursorColor)
                        ?? cupertinoTheme.primaryColor
                    );
                selectionColorLocal =
                    selectionStyle.selectionColor ?? cupertinoTheme.primaryColor.withOpacity(0.4);
                cursorRadiusLocal ??= Radius.circular(2.0);
                cursorOffsetLocal = new Offset(
                    Selectable_textLibrary.iOSHorizontalOffset
                        / MediaQuery.devicePixelRatioOf(context),
                    0
                );
                autocorrectionTextRectColorLocal = selectionColorLocal;
                break;
            }
            case TargetPlatform.macOS:
            {
                CupertinoThemeData cupertinoThemeLocal = CupertinoTheme.of(context);
                forcePressEnabled = false;
                textSelectionControls ??=
                    Desktop_text_selectionLibrary.desktopTextSelectionHandleControls;
                paintCursorAboveTextLocal = true;
                cursorOpacityAnimatesLocal ??= false;
                cursorColorLocal = _hasError
                    ? _errorColor
                    : (
                        (widget.cursorColor ?? selectionStyle.cursorColor)
                        ?? cupertinoThemeLocal.primaryColor
                    );
                selectionColorLocal =
                    selectionStyle.selectionColor
                    ?? cupertinoThemeLocal.primaryColor.withOpacity(0.4);
                cursorRadiusLocal ??= Radius.circular(2.0);
                cursorOffsetLocal = new Offset(
                    Selectable_textLibrary.iOSHorizontalOffset
                        / MediaQuery.devicePixelRatioOf(context),
                    0
                );
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
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            {
                forcePressEnabled = false;
                textSelectionControls ??= Text_selectionLibrary.materialTextSelectionHandleControls;
                paintCursorAboveTextLocal = false;
                cursorOpacityAnimatesLocal ??= false;
                cursorColorLocal = _hasError
                    ? _errorColor
                    : (
                        (widget.cursorColor ?? selectionStyle.cursorColor)
                        ?? theme.colorScheme.primary
                    );
                selectionColorLocal =
                    selectionStyle.selectionColor ?? theme.colorScheme.primary.withOpacity(0.4);
                break;
            }
            case TargetPlatform.linux:
            {
                forcePressEnabled = false;
                textSelectionControls ??=
                    Desktop_text_selectionLibrary.desktopTextSelectionHandleControls;
                paintCursorAboveTextLocal = false;
                cursorOpacityAnimatesLocal ??= false;
                cursorColorLocal = _hasError
                    ? _errorColor
                    : (
                        (widget.cursorColor ?? selectionStyle.cursorColor)
                        ?? theme.colorScheme.primary
                    );
                selectionColorLocal =
                    selectionStyle.selectionColor ?? theme.colorScheme.primary.withOpacity(0.4);
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
            case TargetPlatform.windows:
            {
                forcePressEnabled = false;
                textSelectionControls ??=
                    Desktop_text_selectionLibrary.desktopTextSelectionHandleControls;
                paintCursorAboveTextLocal = false;
                cursorOpacityAnimatesLocal ??= false;
                cursorColorLocal = _hasError
                    ? _errorColor
                    : (
                        (widget.cursorColor ?? selectionStyle.cursorColor)
                        ?? theme.colorScheme.primary
                    );
                selectionColorLocal =
                    selectionStyle.selectionColor ?? theme.colorScheme.primary.withOpacity(0.4);
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
        Widget childLocal = new RepaintBoundary(
            child: new UnmanagedRestorationScope(
                bucket: bucket,
                child: new EditableText(
                    key: editableTextKey,
                    readOnly: widget.readOnly || !_isEnabled,
                    toolbarOptions: widget.toolbarOptions,
                    showCursor: widget.showCursor,
                    showSelectionHandles: _showSelectionHandles,
                    controller: controllerLocal,
                    focusNode: focusNodeLocal,
                    undoController: widget.undoController,
                    keyboardType: widget.keyboardType,
                    textInputAction: widget.textInputAction,
                    textCapitalization: widget.textCapitalization,
                    style: styleLocal,
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
                    selectionColor: focusNodeLocal.hasFocus ? selectionColorLocal : null,
                    selectionControls: widget.selectionEnabled ? textSelectionControls : null,
                    onChanged: widget.onChanged,
                    onSelectionChanged: _handleSelectionChanged,
                    onEditingComplete: widget.onEditingComplete,
                    onSubmitted: widget.onSubmitted,
                    onAppPrivateCommand: widget.onAppPrivateCommand,
                    groupId: widget.groupId,
                    onSelectionHandleTapped: _handleSelectionHandleTapped,
                    onTapOutside: widget.onTapOutside,
                    onTapUpOutside: widget.onTapUpOutside,
                    inputFormatters: formatters,
                    rendererIgnoresPointer: true,
                    mouseCursor: MouseCursor.defer,
                    cursorWidth: widget.cursorWidth,
                    cursorHeight: widget.cursorHeight,
                    cursorRadius: cursorRadiusLocal,
                    cursorColor: cursorColorLocal,
                    selectionHeightStyle: widget.selectionHeightStyle,
                    selectionWidthStyle: widget.selectionWidthStyle,
                    cursorOpacityAnimates: cursorOpacityAnimatesLocal ?? false,
                    cursorOffset: cursorOffsetLocal,
                    paintCursorAboveText: paintCursorAboveTextLocal,
                    backgroundCursorColor: CupertinoColors.inactiveGray,
                    scrollPadding: widget.scrollPadding,
                    keyboardAppearance: keyboardAppearanceLocal,
                    enableInteractiveSelection: widget.enableInteractiveSelection,
                    selectAllOnFocus: widget.selectAllOnFocus ?? false,
                    dragStartBehavior: widget.dragStartBehavior,
                    scrollController: widget.scrollController,
                    scrollPhysics: widget.scrollPhysics,
                    autofillHints: widget.autofillHints,
                    autofillClient: this,
                    autocorrectionTextRectColor: autocorrectionTextRectColorLocal,
                    clipBehavior: widget.clipBehavior,
                    restorationId: "editable",
                    scribbleEnabled: widget.scribbleEnabled,
                    stylusHandwritingEnabled: widget.stylusHandwritingEnabled,
                    enableIMEPersonalizedLearning: widget.enableIMEPersonalizedLearning,
                    enableInlinePrediction: widget.enableInlinePrediction,
                    contentInsertionConfiguration: widget.contentInsertionConfiguration,
                    contextMenuBuilder: widget.contextMenuBuilder,
                    spellCheckConfiguration: spellCheckConfigurationLocal,
                    magnifierConfiguration: widget.magnifierConfiguration
                        ?? TextMagnifier.adaptiveMagnifierConfiguration,
                    hintLocales: widget.hintLocales
                )
            )
        );
        if (widget.decoration is not null)
        {
            childLocal = DartRuntimePrimitives.ConvertValue<Widget>(
                new AnimatedBuilder(
                    animation: Listenable.CreateMerge(
                        new List<Listenable> { focusNodeLocal, controllerLocal }.Cast<Listenable?>()
                    ),
                    builder: (context, child) =>
                    {
                        return new InputDecorator(
                            decoration: _getEffectiveDecoration(),
                            baseStyle: widget.style,
                            textAlign: widget.textAlign,
                            textAlignVertical: widget.textAlignVertical,
                            isHovering: _isHovering,
                            isFocused: focusNodeLocal.hasFocus,
                            isEmpty: controllerLocal.value.text.Length == 0,
                            expands: widget.expands,
                            child: child
                        );
                        throw new InvalidOperationException(
                            "Callback completed without returning a value."
                        );
                    },
                    child: childLocal
                )
            );
        }
        MouseCursor effectiveMouseCursor = WidgetStateProperty.resolveAs(
            widget.mouseCursor ?? WidgetStateMouseCursor.textable,
            _statesController.value
        );
        long? semanticsMaxValueLength = default!;
        if (
            (!Equals(_effectiveMaxLengthEnforcement, MaxLengthEnforcement.none))
            && (widget.maxLength is not null)
            && (
                (
                    widget.maxLength
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ) > 0L
            )
        )
        {
            semanticsMaxValueLength = widget.maxLength;
        }
        else
        {
            semanticsMaxValueLength = null;
        }
        return new MouseRegion(
            cursor: effectiveMouseCursor,
            onEnter: (@event) =>
            {
                _handleHover(true);
            },
            onExit: (@event) =>
            {
                _handleHover(false);
            },
            child: new TextFieldTapRegion(
                child: new IgnorePointer(
                    ignoring: widget.ignorePointers ?? !_isEnabled,
                    child: new AnimatedBuilder(
                        animation: controllerLocal,
                        builder: (context, child) =>
                        {
                            return new Widgets.Semantics(
                                enabled: _isEnabled,
                                maxValueLength: semanticsMaxValueLength,
                                currentValueLength: _currentLength,
                                onTap: widget.readOnly
                                    ? null
                                    : (
                                        () =>
                                        {
                                            if (!_effectiveController.selection.isValid)
                                            {
                                                _effectiveController.selection =
                                                    TextSelection.CreateCollapsed(
                                                        offset: _effectiveController.text.Length
                                                    );
                                            }
                                            _requestKeyboard();
                                        }
                                    ),
                                onDidGainAccessibilityFocus: () =>
                                    handleDidGainAccessibilityFocus(),
                                onDidLoseAccessibilityFocus: () =>
                                    handleDidLoseAccessibilityFocus(),
                                onFocus: _isEnabled
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
                                            if (
                                                _effectiveFocusNode.canRequestFocus
                                                && !_effectiveFocusNode.hasFocus
                                            )
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
                                child: child
                            );
                            throw new InvalidOperationException(
                                "Callback completed without returning a value."
                            );
                        },
                        child: _selectionGestureDetectorBuilder.buildGestureDetector(
                            behavior: HitTestBehavior.translucent,
                            child: childLocal
                        )
                    )
                )
            )
        );
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
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
}

public static partial class Text_fieldLibrary
{
    internal static TextStyle? _m2StateInputStyle(BuildContext context) =>
        WidgetStateTextStyle.CreateResolveWith(
            (states) =>
            {
                ThemeData theme = Theme.of(context);
                if (states.Contains(WidgetState.disabled))
                {
                    return new TextStyle(color: theme.disabledColor);
                }
                return new TextStyle(color: theme.textTheme.titleMedium?.color);
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
}

public static partial class Text_fieldLibrary
{
    internal static TextStyle _m2CounterErrorStyle(BuildContext context) =>
        Theme.of(context).textTheme.bodySmall!.copyWith(color: Theme.of(context).colorScheme.error);
}

public static partial class Text_fieldLibrary
{
    internal static TextStyle? _m3StateInputStyle(BuildContext context) =>
        WidgetStateTextStyle.CreateResolveWith(
            (states) =>
            {
                if (states.Contains(WidgetState.disabled))
                {
                    return new TextStyle(
                        color: Theme.of(context).textTheme.bodyLarge!.color?.withOpacity(0.38)
                    );
                }
                return new TextStyle(color: Theme.of(context).textTheme.bodyLarge!.color);
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
}

public static partial class Text_fieldLibrary
{
    internal static TextStyle _m3InputStyle(BuildContext context) =>
        Theme.of(context).textTheme.bodyLarge!;
}

public static partial class Text_fieldLibrary
{
    internal static TextStyle _m3CounterErrorStyle(BuildContext context) =>
        Theme.of(context).textTheme.bodySmall!.copyWith(color: Theme.of(context).colorScheme.error);
}
