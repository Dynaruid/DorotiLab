// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/text_field.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public delegate global::Doroti.Framework.Widgets.Widget? InputCounterWidgetBuilder(global::Doroti.Framework.Widgets.BuildContext context, long currentLength, bool isFocused, long? maxLength);

internal class _TextFieldSelectionGestureDetectorBuilder__text_field : global::Doroti.Framework.Widgets.TextSelectionGestureDetectorBuilder
{
    internal virtual _TextFieldState__text_field _state { get; private set; } = default!;

    internal _TextFieldSelectionGestureDetectorBuilder__text_field(_TextFieldState__text_field state) : base(@delegate: state)
    {
        _state = state;
    }

    public override bool onUserTapAlwaysCalled => _state.widget.onTapAlwaysCalled;
    public override void onUserTap()
    {
        _state.widget.onTap?.Invoke();
    }

}

public class TextField : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Widgets.TextMagnifierConfiguration? magnifierConfiguration { get; private set; }
    public virtual object groupId { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.TextEditingController? controller { get; private set; }
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual InputDecoration? decoration { get; private set; }
    public virtual global::Doroti.Framework.Services.TextInputType keyboardType { get; private set; } = default!;
    public virtual global::Doroti.Framework.Services.TextInputAction? textInputAction { get; private set; }
    public virtual global::Doroti.Framework.Services.TextCapitalization textCapitalization { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle? style { get; private set; }
    public virtual global::Doroti.Framework.Painting.StrutStyle? strutStyle { get; private set; }
    public virtual TextAlign textAlign { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextAlignVertical? textAlignVertical { get; private set; }
    public virtual TextDirection? textDirection { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.WidgetStatesController? statesController { get; private set; }
    public virtual string obscuringCharacter { get; private set; } = default!;
    public virtual bool obscureText { get; private set; } = default!;
    public virtual bool? autocorrect { get; private set; }
    public virtual global::Doroti.Framework.Services.SmartDashesType smartDashesType { get; private set; } = default!;
    public virtual global::Doroti.Framework.Services.SmartQuotesType smartQuotesType { get; private set; } = default!;
    public virtual bool enableSuggestions { get; private set; } = default!;
    public virtual long? maxLines { get; private set; }
    public virtual long? minLines { get; private set; }
    public virtual bool expands { get; private set; } = default!;
    public virtual bool readOnly { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.ToolbarOptions? toolbarOptions { get; private set; }
    public virtual bool? showCursor { get; private set; }
    public static long noMaxLength = -1L;
    public virtual long? maxLength { get; private set; }
    public virtual global::Doroti.Framework.Services.MaxLengthEnforcement? maxLengthEnforcement { get; private set; }
    public virtual global::System.Action<string>? onChanged { get; private set; }
    public virtual global::System.Action? onEditingComplete { get; private set; }
    public virtual global::System.Action<string>? onSubmitted { get; private set; }
    public virtual global::System.Action<string, DartMap<string, object?>>? onAppPrivateCommand { get; private set; }
    public virtual List<global::Doroti.Framework.Services.TextInputFormatter>? inputFormatters { get; private set; }
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
    public virtual global::Doroti.Framework.Painting.EdgeInsets scrollPadding { get; private set; } = default!;
    public virtual bool enableInteractiveSelection { get; private set; } = default!;
    public virtual bool? selectAllOnFocus { get; private set; }
    public virtual global::Doroti.Framework.Widgets.TextSelectionControls? selectionControls { get; private set; }
    public virtual global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual global::System.Action? onTap { get; private set; }
    public virtual bool onTapAlwaysCalled { get; private set; } = default!;
    public virtual global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>? onTapOutside { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.PointerUpEvent>? onTapUpOutside { get; private set; }
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual InputCounterWidgetBuilder? buildCounter { get; private set; }
    public virtual global::Doroti.Framework.Widgets.ScrollPhysics? scrollPhysics { get; private set; }
    public virtual global::Doroti.Framework.Widgets.ScrollController? scrollController { get; private set; }
    public virtual IEnumerable<string>? autofillHints { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual string? restorationId { get; private set; }
    public virtual bool scribbleEnabled { get; private set; } = default!;
    public virtual bool stylusHandwritingEnabled { get; private set; } = default!;
    public virtual bool enableIMEPersonalizedLearning { get; private set; } = default!;
    public virtual bool? enableInlinePrediction { get; private set; }
    public virtual global::Doroti.Framework.Widgets.ContentInsertionConfiguration? contentInsertionConfiguration { get; private set; }
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget>? contextMenuBuilder { get; private set; }
    public virtual bool canRequestFocus { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.UndoHistoryController? undoController { get; private set; }
    public virtual List<Locale>? hintLocales { get; private set; }
    public virtual global::Doroti.Framework.Widgets.SpellCheckConfiguration? spellCheckConfiguration { get; private set; }
    public static global::Doroti.Framework.Painting.TextStyle materialMisspelledTextStyle = new global::Doroti.Framework.Painting.TextStyle(decoration: TextDecoration.underline, decorationColor: Colors.red, decorationStyle: TextDecorationStyle.wavy);

    public TextField(global::Doroti.Framework.Foundation.Key? key = null, object groupId = default!, global::Doroti.Framework.Widgets.TextEditingController? controller = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::Doroti.Framework.Widgets.UndoHistoryController? undoController = null, InputDecoration? decoration = default!, global::Doroti.Framework.Services.TextInputType? keyboardType = null, global::Doroti.Framework.Services.TextInputAction? textInputAction = null, global::Doroti.Framework.Services.TextCapitalization textCapitalization = TextCapitalization.none, global::Doroti.Framework.Painting.TextStyle? style = null, global::Doroti.Framework.Painting.StrutStyle? strutStyle = null, TextAlign textAlign = TextAlign.start, global::Doroti.Framework.Painting.TextAlignVertical? textAlignVertical = null, TextDirection? textDirection = null, bool readOnly = false, global::Doroti.Framework.Widgets.ToolbarOptions? toolbarOptions = null, bool? showCursor = null, bool autofocus = false, global::Doroti.Framework.Widgets.WidgetStatesController? statesController = null, string obscuringCharacter = "•", bool obscureText = false, bool? autocorrect = null, global::Doroti.Framework.Services.SmartDashesType? smartDashesType = null, global::Doroti.Framework.Services.SmartQuotesType? smartQuotesType = null, bool enableSuggestions = true, long? maxLines = 1, long? minLines = null, bool expands = false, long? maxLength = null, global::Doroti.Framework.Services.MaxLengthEnforcement? maxLengthEnforcement = null, global::System.Action<string>? onChanged = null, global::System.Action? onEditingComplete = null, global::System.Action<string>? onSubmitted = null, global::System.Action<string, DartMap<string, object?>>? onAppPrivateCommand = null, List<global::Doroti.Framework.Services.TextInputFormatter>? inputFormatters = null, bool? enabled = null, bool? ignorePointers = null, double cursorWidth = 2.0, double? cursorHeight = null, Radius? cursorRadius = null, bool? cursorOpacityAnimates = null, Color? cursorColor = null, Color? cursorErrorColor = null, BoxHeightStyle? selectionHeightStyle = null, BoxWidthStyle? selectionWidthStyle = null, Brightness? keyboardAppearance = null, global::Doroti.Framework.Painting.EdgeInsets scrollPadding = default!, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = Gestures.DragStartBehavior.start, bool? enableInteractiveSelection = null, bool? selectAllOnFocus = null, global::Doroti.Framework.Widgets.TextSelectionControls? selectionControls = null, global::System.Action? onTap = null, bool onTapAlwaysCalled = false, global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>? onTapOutside = null, global::System.Action<global::Doroti.Framework.Gestures.PointerUpEvent>? onTapUpOutside = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, InputCounterWidgetBuilder? buildCounter = null, global::Doroti.Framework.Widgets.ScrollController? scrollController = null, global::Doroti.Framework.Widgets.ScrollPhysics? scrollPhysics = null, IEnumerable<string>? autofillHints = default!, global::Doroti.Framework.Widgets.ContentInsertionConfiguration? contentInsertionConfiguration = null, Clip clipBehavior = Clip.hardEdge, string? restorationId = null, bool scribbleEnabled = true, bool? stylusHandwritingEnabled = null, bool enableIMEPersonalizedLearning = true, bool? enableInlinePrediction = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget>? contextMenuBuilder = default!, bool canRequestFocus = true, global::Doroti.Framework.Widgets.SpellCheckConfiguration? spellCheckConfiguration = null, global::Doroti.Framework.Widgets.TextMagnifierConfiguration? magnifierConfiguration = null, List<Locale>? hintLocales = null) : base(key: key)
    {
        object __groupId = groupId ?? typeof(global::Doroti.Framework.Widgets.EditableText);
        InputDecoration? __decoration = decoration ?? new InputDecoration();
        global::Doroti.Framework.Painting.EdgeInsets __scrollPadding = scrollPadding ?? EdgeInsets.CreateAll(20.0);
        IEnumerable<string>? __autofillHints = autofillHints ?? new List<string>();
        bool __stylusHandwritingEnabled = stylusHandwritingEnabled ?? EditableText.defaultStylusHandwritingEnabled;
        global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget>? __contextMenuBuilder = contextMenuBuilder ?? _defaultContextMenuBuilder;
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
        this.smartDashesType = smartDashesType ?? (obscureText ? SmartDashesType.disabled : SmartDashesType.enabled);
        this.smartQuotesType = smartQuotesType ?? (obscureText ? SmartQuotesType.disabled : SmartQuotesType.enabled);
        this.keyboardType = keyboardType ?? ((maxLines == 1L) ? TextInputType.text : TextInputType.multiline);
        this.enableInteractiveSelection = enableInteractiveSelection ?? !readOnly || !obscureText;
        System.Diagnostics.Debug.Assert(obscuringCharacter.Length == 1L);
        System.Diagnostics.Debug.Assert((maxLines is null) || (DartRuntimePrimitives.RequireValue(maxLines) > 0L));
        System.Diagnostics.Debug.Assert((minLines is null) || (DartRuntimePrimitives.RequireValue(minLines) > 0L));
        System.Diagnostics.Debug.Assert(maxLines is null || minLines is null || maxLines >= DartRuntimePrimitives.RequireValue(minLines));
        System.Diagnostics.Debug.Assert(!expands || (maxLines is null) && (minLines is null));
        System.Diagnostics.Debug.Assert(!obscureText || (maxLines == 1L));
        System.Diagnostics.Debug.Assert((maxLength is null) || (DartRuntimePrimitives.RequireValue(maxLength) == noMaxLength) || (DartRuntimePrimitives.RequireValue(maxLength) > 0L));
        System.Diagnostics.Debug.Assert(!DartRuntimePrimitives.Identical(textInputAction, TextInputAction.newline) || (maxLines == 1L) || !DartRuntimePrimitives.Identical(keyboardType, TextInputType.text));
    }

    public virtual bool selectionEnabled => enableInteractiveSelection;
    internal static global::Doroti.Framework.Widgets.Widget _defaultContextMenuBuilder(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.EditableTextState editableTextState)
    {
        if (SystemContextMenu.isSupportedByField(editableTextState))
        {
            return SystemContextMenu.CreateEditableText(editableTextState: editableTextState);
        }
        return AdaptiveTextSelectionToolbar.CreateEditableText(editableTextState: editableTextState);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Framework.Widgets.Widget defaultSpellCheckSuggestionsToolbarBuilder(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.EditableTextState editableTextState)
    {
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.iOS:
            case TargetPlatform.macOS:
                {
                    return CupertinoSpellCheckSuggestionsToolbar.CreateEditableText(editableTextState: editableTextState);
                }
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
                {
                    return SpellCheckSuggestionsToolbar.CreateEditableText(editableTextState: editableTextState);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Framework.Widgets.SpellCheckConfiguration inferAndroidSpellCheckConfiguration(global::Doroti.Framework.Widgets.SpellCheckConfiguration? configuration)
    {
        if ((configuration is null) || Equals(configuration, SpellCheckConfiguration.CreateDisabled()))
        {
            return SpellCheckConfiguration.CreateDisabled();
        }
        return configuration.copyWith(misspelledTextStyle: configuration.misspelledTextStyle ?? materialMisspelledTextStyle, spellCheckSuggestionsToolbarBuilder: configuration.spellCheckSuggestionsToolbarBuilder ?? defaultSpellCheckSuggestionsToolbarBuilder);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _TextFieldState__text_field());
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.TextEditingController>("controller", controller, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.FocusNode>("focusNode", focusNode, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.UndoHistoryController>("undoController", undoController, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("enabled", enabled, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<InputDecoration>("decoration", decoration, defaultValue: new InputDecoration()));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Services.TextInputType>("keyboardType", keyboardType, defaultValue: TextInputType.text));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("style", style, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("autofocus", autofocus, defaultValue: false));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<string>("obscuringCharacter", obscuringCharacter, defaultValue: "•"));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("obscureText", obscureText, defaultValue: false));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("autocorrect", autocorrect, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Services.SmartDashesType>("smartDashesType", smartDashesType, defaultValue: obscureText ? SmartDashesType.disabled : SmartDashesType.enabled));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Services.SmartQuotesType>("smartQuotesType", smartQuotesType, defaultValue: obscureText ? SmartQuotesType.disabled : SmartQuotesType.enabled));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("enableSuggestions", enableSuggestions, defaultValue: true));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("maxLines", maxLines, defaultValue: 1L));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("minLines", minLines, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("expands", expands, defaultValue: false));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("maxLength", maxLength, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Services.MaxLengthEnforcement>("maxLengthEnforcement", maxLengthEnforcement, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Services.TextInputAction>("textInputAction", textInputAction, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Services.TextCapitalization>("textCapitalization", textCapitalization, defaultValue: TextCapitalization.none));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextAlign>("textAlign", textAlign, defaultValue: TextAlign.start));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextAlignVertical>("textAlignVertical", textAlignVertical, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", textDirection, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("cursorWidth", cursorWidth, defaultValue: 2.0));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("cursorHeight", cursorHeight, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Radius>("cursorRadius", cursorRadius, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("cursorOpacityAnimates", cursorOpacityAnimates, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("cursorColor", cursorColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("cursorErrorColor", cursorErrorColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Brightness>("keyboardAppearance", keyboardAppearance, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("scrollPadding", scrollPadding, defaultValue: EdgeInsets.CreateAll(20.0)));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("selectionEnabled", value: selectionEnabled, defaultValue: true, ifFalse: "selection disabled"));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.TextSelectionControls>("selectionControls", selectionControls, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.ScrollController>("scrollController", scrollController, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.ScrollPhysics>("scrollPhysics", scrollPhysics, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Clip>("clipBehavior", clipBehavior, defaultValue: Clip.hardEdge));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("scribbleEnabled", scribbleEnabled, defaultValue: true));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("stylusHandwritingEnabled", DartRuntimePrimitives.RequireValue(stylusHandwritingEnabled), defaultValue: EditableText.defaultStylusHandwritingEnabled));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("enableIMEPersonalizedLearning", enableIMEPersonalizedLearning, defaultValue: true));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool?>("enableInlinePrediction", enableInlinePrediction, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.SpellCheckConfiguration>("spellCheckConfiguration", spellCheckConfiguration, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<List<string>>("contentCommitMimeTypes", contentInsertionConfiguration?.allowedMimeTypes ?? new List<string>(), defaultValue: (contentInsertionConfiguration is null) ? new List<string>() : Editable_textLibrary.kDefaultContentInsertionMimeTypes));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<List<global::Doroti.Ui.Locale>?>("hintLocales", hintLocales, defaultValue: null));
    }

}

internal class _TextFieldState__text_field : global::Doroti.Framework.Widgets.State<TextField>, global::Doroti.Framework.Widgets.RestorationMixin<TextField>, global::Doroti.Framework.Widgets.TextSelectionGestureDetectorBuilderDelegate, global::Doroti.Framework.Services.AutofillClient
{
    internal virtual global::Doroti.Framework.Widgets.RestorableTextEditingController? _controller { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.FocusNode? _focusNode { get; set; } = default;
    internal virtual bool _isHovering { get; set; } = false;
    internal virtual bool _showSelectionHandles { get; set; } = false;
    internal virtual _TextFieldSelectionGestureDetectorBuilder__text_field _selectionGestureDetectorBuilder { get; set; } = default!;
    public virtual bool forcePressEnabled { get; set; } = default!;
    public virtual global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.EditableTextState> editableTextKey { get; private set; } = GlobalKey<EditableTextState>.Create();
    internal virtual global::Doroti.Framework.Widgets.WidgetStatesController? _internalStatesController { get; set; } = default;
    public virtual global::Doroti.Framework.Services.RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<global::Doroti.Framework.Widgets.IRestorableProperty, global::System.Action> _properties { get; set; } = new DartMap<global::Doroti.Framework.Widgets.IRestorableProperty, global::System.Action>();
    public virtual List<global::Doroti.Framework.Widgets.IRestorableProperty>? _debugPropertiesWaitingForReregistration { get; set; } = default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual global::Doroti.Framework.Services.RestorationBucket? _currentParent { get; set; } = default;

    internal virtual global::Doroti.Framework.Widgets.TextEditingController _effectiveController => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.TextEditingController>(widget.controller ?? _controller!.value);
    internal virtual global::Doroti.Framework.Widgets.FocusNode _effectiveFocusNode => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.FocusNode>(widget.focusNode ?? (_focusNode ??= new global::Doroti.Framework.Widgets.FocusNode()));
    internal virtual global::Doroti.Framework.Services.MaxLengthEnforcement _effectiveMaxLengthEnforcement => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Services.MaxLengthEnforcement>(widget.maxLengthEnforcement ?? LengthLimitingTextInputFormatter.getDefaultMaxLengthEnforcement(Theme.of(context).platform));
    public virtual bool needsCounter => DartRuntimePrimitives.ConvertValue<bool>((widget.maxLength is not null) && (widget.decoration is not null) && (widget.decoration!.counterText is null));
    public virtual bool selectionEnabled => DartRuntimePrimitives.ConvertValue<bool>(widget.selectionEnabled && _isEnabled);
    internal virtual bool _isEnabled => DartRuntimePrimitives.ConvertValue<bool>((widget.enabled ?? widget.decoration?.enabled) ?? true);
    internal virtual long _currentLength => _effectiveController.value.text.characters().Count;
    internal virtual bool _hasIntrinsicError => DartRuntimePrimitives.ConvertValue<bool>((widget.maxLength is not null) && (DartRuntimePrimitives.RequireValue(widget.maxLength) > 0L) && ((widget.controller is null) ? (!restorePending && (_effectiveController.value.text.characters().Count > DartRuntimePrimitives.RequireValue(widget.maxLength))) : (_effectiveController.value.text.characters().Count > DartRuntimePrimitives.RequireValue(widget.maxLength))));
    internal virtual bool _hasError => DartRuntimePrimitives.ConvertValue<bool>((widget.decoration?.errorText is not null) || (widget.decoration?.error is not null) || _hasIntrinsicError);
    internal virtual global::Doroti.Ui.Color _errorColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((widget.cursorErrorColor ?? _getEffectiveDecoration().errorStyle?.color) ?? Theme.of(context).colorScheme.error);
    internal virtual InputDecoration _getEffectiveDecoration()
    {
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        ThemeData themeData = Theme.of(context);
        InputDecorationThemeData decorationTheme = InputDecorationTheme.of(context);
        InputDecoration effectiveDecoration = (widget.decoration ?? new InputDecoration()).applyDefaults(decorationTheme).copyWith(enabled: _isEnabled, hintMaxLines: (widget.decoration?.hintMaxLines ?? decorationTheme.hintMaxLines) ?? widget.maxLines);
        if ((effectiveDecoration.counter is not null) || (effectiveDecoration.counterText is not null))
        {
            return effectiveDecoration;
        }
        global::Doroti.Framework.Widgets.Widget? counterLocal = default!;
        long currentLengthLocal = _currentLength;
        if ((effectiveDecoration.counter is null) && (effectiveDecoration.counterText is null) && (widget.buildCounter is not null))
        {
            bool isFocusedLocal = _effectiveFocusNode.hasFocus;
            global::Doroti.Framework.Widgets.Widget? builtCounter = widget.buildCounter!(context, currentLength: currentLengthLocal, maxLength: widget.maxLength, isFocused: isFocusedLocal);
            if (builtCounter is not null)
            {
                counterLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(container: true, liveRegion: isFocusedLocal, child: builtCounter));
            }
            return effectiveDecoration.copyWith(counter: counterLocal);
        }
        if (widget.maxLength is null)
        {
            return effectiveDecoration;
        }
        var counterTextLocal = $"{currentLengthLocal}";
        var semanticCounterTextLocal = "";
        if (DartRuntimePrimitives.RequireValue(widget.maxLength) > 0L)
        {
            counterTextLocal += $"/{widget.maxLength}";
            long remaining = (DartRuntimePrimitives.RequireValue(widget.maxLength) - currentLengthLocal).clamp(0L, DartRuntimePrimitives.RequireValue(widget.maxLength));
            semanticCounterTextLocal = localizations.remainingTextFieldCharacterCount(remaining);
        }
        if (_hasIntrinsicError)
        {
            return effectiveDecoration.copyWith(errorText: effectiveDecoration.errorText ?? "", counterStyle: effectiveDecoration.errorStyle ?? Text_fieldLibrary._m3CounterErrorStyle(context), counterText: counterTextLocal, semanticCounterText: semanticCounterTextLocal);
        }
        return effectiveDecoration.copyWith(counterText: counterTextLocal, semanticCounterText: semanticCounterTextLocal);
    }

    public override void initState()
    {
        base.initState();
        _selectionGestureDetectorBuilder = new _TextFieldSelectionGestureDetectorBuilder__text_field(state: this);
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
            global::Doroti.Framework.Widgets.NavigationMode mode = MediaQuery.maybeNavigationModeOf(context) ?? NavigationMode.traditional;
            return mode switch { NavigationMode.traditional => widget.canRequestFocus && _isEnabled, NavigationMode.directional => true, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        }
    }
    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = _bucket;
        bool needsRestore = restorePending;
        _currentParent = RestorationScope.maybeOf(context);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: _currentParent, restorePending: needsRestore);
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
                unregisterFromRestoration(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.RestorableProperty<object>>(_controller!));
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

    public virtual void restoreState(global::Doroti.Framework.Services.RestorationBucket? oldBucket, bool initialRestore)
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

    internal virtual void _createLocalController(global::Doroti.Framework.Services.TextEditingValue? value = null)
    {
        DartRuntimePrimitives.Assert(() => _controller is null);
        _controller = (value is null) ? RestorableTextEditingController.Create() : new global::Doroti.Framework.Widgets.RestorableTextEditingController(value);
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
        _properties.forEach((property, listener) =>
        {
            if (!property._disposed)
            {
                property.removeListener(listener);
            }
        });
        _bucket?.dispose();
        _bucket = null;
        base.dispose();
    }

    internal virtual global::Doroti.Framework.Widgets.EditableTextState? _editableText => editableTextKey.currentState;
    internal virtual void _requestKeyboard()
    {
        _editableText?.requestKeyboard();
    }

    internal virtual bool _shouldShowSelectionHandles(global::Doroti.Framework.Services.SelectionChangedCause? cause)
    {
        if (!_selectionGestureDetectorBuilder.shouldShowSelectionToolbar || !_selectionGestureDetectorBuilder.shouldShowSelectionHandles)
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
        if (Equals(cause, SelectionChangedCause.longPress) || Equals(cause, SelectionChangedCause.stylusHandwriting))
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

    internal virtual void _handleFocusChanged()
    {
        setState(() =>
        {
        });
        _statesController.update(WidgetState.focused, _effectiveFocusNode.hasFocus);
    }

    internal virtual void _handleSelectionChanged(global::Doroti.Framework.Services.TextSelection selection, global::Doroti.Framework.Services.SelectionChangedCause? cause)
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
        setState(() =>
        {
        });
    }

    internal virtual global::Doroti.Framework.Widgets.WidgetStatesController _statesController => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStatesController>(widget.statesController ?? _internalStatesController!);
    internal virtual void _initStatesController()
    {
        if (widget.statesController is null)
        {
            _internalStatesController = new global::Doroti.Framework.Widgets.WidgetStatesController();
        }
        _statesController.update(WidgetState.disabled, !_isEnabled);
        _statesController.update(WidgetState.hovered, _isHovering);
        _statesController.update(WidgetState.focused, _effectiveFocusNode.hasFocus);
        _statesController.update(WidgetState.error, _hasError);
        _statesController.addListener(_handleStatesControllerChange);
    }

    public virtual string autofillId => _editableText!.autofillId;
    public virtual void autofill(global::Doroti.Framework.Services.TextEditingValue newEditingValue) => _editableText!.autofill(newEditingValue);
    public virtual global::Doroti.Framework.Services.TextInputConfiguration textInputConfiguration
    {
        get
        {
            List<string>? autofillHintsLocal = widget.autofillHints?.ToList().ToList();
            global::Doroti.Framework.Services.AutofillConfiguration autofillConfigurationLocal = (autofillHintsLocal is not null) ? new global::Doroti.Framework.Services.AutofillConfiguration(uniqueIdentifier: autofillId, autofillHints: autofillHintsLocal, currentEditingValue: _effectiveController.value, hintText: (widget.decoration ?? new InputDecoration()).hintText) : AutofillConfiguration.disabled;
            return _editableText!.textInputConfiguration.copyWith(autofillConfiguration: autofillConfigurationLocal);
        }
    }
    internal virtual global::Doroti.Framework.Painting.TextStyle _getInputStyleForState(global::Doroti.Framework.Painting.TextStyle style)
    {
        ThemeData theme = Theme.of(context);
        global::Doroti.Framework.Painting.TextStyle stateStyle = WidgetStateProperty.resolveAs(Text_fieldLibrary._m3StateInputStyle(context)!, _statesController.value);
        global::Doroti.Framework.Painting.TextStyle providedStyle = WidgetStateProperty.resolveAs(style, _statesController.value);
        return providedStyle.merge(stateStyle);
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        DartRuntimePrimitives.Assert(() => !((widget.style is not null) && !widget.style!.inherit && ((widget.style!.fontSize is null) || (widget.style!.textBaseline is null))), () => (object?)"inherit false style must supply fontSize and textBaseline");
        ThemeData theme = Theme.of(context);
        global::Doroti.Framework.Widgets.DefaultSelectionStyle selectionStyle = DefaultSelectionStyle.of(context);
        global::Doroti.Framework.Painting.TextStyle? providedStyle = WidgetStateProperty.resolveAs(widget.style, _statesController.value);
        global::Doroti.Framework.Painting.TextStyle styleLocal = _getInputStyleForState(Text_fieldLibrary._m3InputStyle(context)).merge(providedStyle);
        global::Doroti.Ui.Brightness keyboardAppearanceLocal = widget.keyboardAppearance ?? theme.brightness;
        global::Doroti.Framework.Widgets.TextEditingController controllerLocal = _effectiveController;
        global::Doroti.Framework.Widgets.FocusNode focusNodeLocal = _effectiveFocusNode;
        var formatters = ((Func<List<global::Doroti.Framework.Services.TextInputFormatter>>)(() => { var __collection58738 = new List<global::Doroti.Framework.Services.TextInputFormatter>(); var __collectionSpread58766 = widget.inputFormatters; if (__collectionSpread58766 is not null) { __collection58738.AddRange(__collectionSpread58766); } if (widget.maxLength is not null) { __collection58738.Add(new global::Doroti.Framework.Services.LengthLimitingTextInputFormatter(widget.maxLength, maxLengthEnforcement: _effectiveMaxLengthEnforcement)); } return __collection58738; }))();
        global::Doroti.Framework.Widgets.SpellCheckConfiguration spellCheckConfigurationLocal = default!;
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.iOS:
            case TargetPlatform.macOS:
                {
                    spellCheckConfigurationLocal = CupertinoTextField.inferIOSSpellCheckConfiguration(widget.spellCheckConfiguration);
                    break;
                }
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
                {
                    spellCheckConfigurationLocal = TextField.inferAndroidSpellCheckConfiguration(widget.spellCheckConfiguration);
                    break;
                }
        }
        global::Doroti.Framework.Widgets.TextSelectionControls? textSelectionControls = widget.selectionControls;
        bool paintCursorAboveTextLocal = default!;
        bool? cursorOpacityAnimatesLocal = widget.cursorOpacityAnimates;
        global::Doroti.Ui.Offset? cursorOffsetLocal = default!;
        global::Doroti.Ui.Color cursorColorLocal = default!;
        global::Doroti.Ui.Color selectionColorLocal = default!;
        global::Doroti.Ui.Color? autocorrectionTextRectColorLocal = default!;
        global::Doroti.Ui.Radius? cursorRadiusLocal = widget.cursorRadius;
        global::System.Action? handleDidGainAccessibilityFocus = default!;
        global::System.Action? handleDidLoseAccessibilityFocus = default!;
        switch (theme.platform)
        {
            case TargetPlatform.iOS:
                {
                    CupertinoThemeData cupertinoTheme = CupertinoTheme.of(context);
                    forcePressEnabled = true;
                    textSelectionControls ??= Cupertino.Text_selectionLibrary.cupertinoTextSelectionHandleControls;
                    paintCursorAboveTextLocal = true;
                    cursorOpacityAnimatesLocal ??= true;
                    cursorColorLocal = _hasError ? _errorColor : ((widget.cursorColor ?? selectionStyle.cursorColor) ?? cupertinoTheme.primaryColor);
                    selectionColorLocal = selectionStyle.selectionColor ?? cupertinoTheme.primaryColor.withOpacity(0.4);
                    cursorRadiusLocal ??= Radius.circular(2.0);
                    cursorOffsetLocal = new global::Doroti.Ui.Offset(Selectable_textLibrary.iOSHorizontalOffset / MediaQuery.devicePixelRatioOf(context), 0);
                    autocorrectionTextRectColorLocal = selectionColorLocal;
                    break;
                }
            case TargetPlatform.macOS:
                {
                    CupertinoThemeData cupertinoThemeLocal = CupertinoTheme.of(context);
                    forcePressEnabled = false;
                    textSelectionControls ??= Desktop_text_selectionLibrary.desktopTextSelectionHandleControls;
                    paintCursorAboveTextLocal = true;
                    cursorOpacityAnimatesLocal ??= false;
                    cursorColorLocal = _hasError ? _errorColor : ((widget.cursorColor ?? selectionStyle.cursorColor) ?? cupertinoThemeLocal.primaryColor);
                    selectionColorLocal = selectionStyle.selectionColor ?? cupertinoThemeLocal.primaryColor.withOpacity(0.4);
                    cursorRadiusLocal ??= Radius.circular(2.0);
                    cursorOffsetLocal = new global::Doroti.Ui.Offset(Selectable_textLibrary.iOSHorizontalOffset / MediaQuery.devicePixelRatioOf(context), 0);
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
                    cursorColorLocal = _hasError ? _errorColor : ((widget.cursorColor ?? selectionStyle.cursorColor) ?? theme.colorScheme.primary);
                    selectionColorLocal = selectionStyle.selectionColor ?? theme.colorScheme.primary.withOpacity(0.4);
                    break;
                }
            case TargetPlatform.linux:
                {
                    forcePressEnabled = false;
                    textSelectionControls ??= Desktop_text_selectionLibrary.desktopTextSelectionHandleControls;
                    paintCursorAboveTextLocal = false;
                    cursorOpacityAnimatesLocal ??= false;
                    cursorColorLocal = _hasError ? _errorColor : ((widget.cursorColor ?? selectionStyle.cursorColor) ?? theme.colorScheme.primary);
                    selectionColorLocal = selectionStyle.selectionColor ?? theme.colorScheme.primary.withOpacity(0.4);
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
                    textSelectionControls ??= Desktop_text_selectionLibrary.desktopTextSelectionHandleControls;
                    paintCursorAboveTextLocal = false;
                    cursorOpacityAnimatesLocal ??= false;
                    cursorColorLocal = _hasError ? _errorColor : ((widget.cursorColor ?? selectionStyle.cursorColor) ?? theme.colorScheme.primary);
                    selectionColorLocal = selectionStyle.selectionColor ?? theme.colorScheme.primary.withOpacity(0.4);
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
        global::Doroti.Framework.Widgets.Widget childLocal = new global::Doroti.Framework.Widgets.RepaintBoundary(child: new global::Doroti.Framework.Widgets.UnmanagedRestorationScope(bucket: bucket, child: new global::Doroti.Framework.Widgets.EditableText(key: editableTextKey, readOnly: widget.readOnly || !_isEnabled, toolbarOptions: widget.toolbarOptions, showCursor: widget.showCursor, showSelectionHandles: _showSelectionHandles, controller: controllerLocal, focusNode: focusNodeLocal, undoController: widget.undoController, keyboardType: widget.keyboardType, textInputAction: widget.textInputAction, textCapitalization: widget.textCapitalization, style: styleLocal, strutStyle: widget.strutStyle, textAlign: widget.textAlign, textDirection: widget.textDirection, autofocus: widget.autofocus, obscuringCharacter: widget.obscuringCharacter, obscureText: widget.obscureText, autocorrect: widget.autocorrect, smartDashesType: widget.smartDashesType, smartQuotesType: widget.smartQuotesType, enableSuggestions: widget.enableSuggestions, maxLines: widget.maxLines, minLines: widget.minLines, expands: widget.expands, selectionColor: focusNodeLocal.hasFocus ? selectionColorLocal : null, selectionControls: widget.selectionEnabled ? textSelectionControls : null, onChanged: widget.onChanged, onSelectionChanged: _handleSelectionChanged, onEditingComplete: widget.onEditingComplete, onSubmitted: widget.onSubmitted, onAppPrivateCommand: widget.onAppPrivateCommand, groupId: widget.groupId, onSelectionHandleTapped: _handleSelectionHandleTapped, onTapOutside: widget.onTapOutside, onTapUpOutside: widget.onTapUpOutside, inputFormatters: formatters, rendererIgnoresPointer: true, mouseCursor: MouseCursor.defer, cursorWidth: widget.cursorWidth, cursorHeight: widget.cursorHeight, cursorRadius: cursorRadiusLocal, cursorColor: cursorColorLocal, selectionHeightStyle: widget.selectionHeightStyle, selectionWidthStyle: widget.selectionWidthStyle, cursorOpacityAnimates: cursorOpacityAnimatesLocal ?? false, cursorOffset: cursorOffsetLocal, paintCursorAboveText: paintCursorAboveTextLocal, backgroundCursorColor: CupertinoColors.inactiveGray, scrollPadding: widget.scrollPadding, keyboardAppearance: keyboardAppearanceLocal, enableInteractiveSelection: widget.enableInteractiveSelection, selectAllOnFocus: widget.selectAllOnFocus ?? false, dragStartBehavior: widget.dragStartBehavior, scrollController: widget.scrollController, scrollPhysics: widget.scrollPhysics, autofillHints: widget.autofillHints, autofillClient: this, autocorrectionTextRectColor: autocorrectionTextRectColorLocal, clipBehavior: widget.clipBehavior, restorationId: "editable", scribbleEnabled: widget.scribbleEnabled, stylusHandwritingEnabled: widget.stylusHandwritingEnabled, enableIMEPersonalizedLearning: widget.enableIMEPersonalizedLearning, enableInlinePrediction: widget.enableInlinePrediction, contentInsertionConfiguration: widget.contentInsertionConfiguration, contextMenuBuilder: widget.contextMenuBuilder, spellCheckConfiguration: spellCheckConfigurationLocal, magnifierConfiguration: widget.magnifierConfiguration ?? TextMagnifier.adaptiveMagnifierConfiguration, hintLocales: widget.hintLocales)));
        if (widget.decoration is not null)
        {
            childLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: Listenable.CreateMerge(new List<global::Doroti.Framework.Foundation.Listenable> { focusNodeLocal, controllerLocal }.Cast<global::Doroti.Framework.Foundation.Listenable?>()), builder: (context, child) =>
            {
                return new InputDecorator(decoration: _getEffectiveDecoration(), baseStyle: widget.style, textAlign: widget.textAlign, textAlignVertical: widget.textAlignVertical, isHovering: _isHovering, isFocused: focusNodeLocal.hasFocus, isEmpty: controllerLocal.value.text.Length == 0, expands: widget.expands, child: child);
                throw new InvalidOperationException("Dart closure completed without a value.");
            }, child: childLocal));
        }
        global::Doroti.Framework.Services.MouseCursor effectiveMouseCursor = WidgetStateProperty.resolveAs<global::Doroti.Framework.Services.MouseCursor>(widget.mouseCursor ?? WidgetStateMouseCursor.textable, _statesController.value);
        long? semanticsMaxValueLength = default!;
        if ((!Equals(_effectiveMaxLengthEnforcement, MaxLengthEnforcement.none)) && (widget.maxLength is not null) && (DartRuntimePrimitives.RequireValue(widget.maxLength) > 0L))
        {
            semanticsMaxValueLength = widget.maxLength;
        }
        else
        {
            semanticsMaxValueLength = null;
        }
        return new global::Doroti.Framework.Widgets.MouseRegion(cursor: effectiveMouseCursor, onEnter: (@event) => { _handleHover(true); }, onExit: (@event) => { _handleHover(false); }, child: new global::Doroti.Framework.Widgets.TextFieldTapRegion(child: new global::Doroti.Framework.Widgets.IgnorePointer(ignoring: widget.ignorePointers ?? !_isEnabled, child: new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: controllerLocal, builder: (context, child) =>
        {
            return new global::Doroti.Framework.Widgets.Semantics(enabled: _isEnabled, maxValueLength: semanticsMaxValueLength, currentValueLength: _currentLength, onTap: widget.readOnly ? null : (() =>
            {
                if (!_effectiveController.selection.isValid)
                {
                    _effectiveController.selection = TextSelection.CreateCollapsed(offset: _effectiveController.text.Length);
                }
                _requestKeyboard();
            }), onDidGainAccessibilityFocus: () => handleDidGainAccessibilityFocus(), onDidLoseAccessibilityFocus: () => handleDidLoseAccessibilityFocus(), onFocus: _isEnabled ? (() =>
            {
                DartRuntimePrimitives.Assert(() => _effectiveFocusNode.canRequestFocus, () => (object?)"Received SemanticsAction.focus from the engine. However, the FocusNode " + "of this text field cannot gain focus. This likely indicates a bug. " + "If this text field cannot be focused (e.g. because it is not " + "enabled), then its corresponding semantics node must be configured " + "such that the assistive technology cannot request focus on it.");
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
            }) : null, child: child);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, child: _selectionGestureDetectorBuilder.buildGestureDetector(behavior: HitTestBehavior.translucent, child: childLocal)))));
    }

    public virtual global::Doroti.Framework.Services.RestorationBucket? bucket => _bucket;
    public virtual void didToggleBucket(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => _bucket?.isReplacing != true);
    }

    public virtual void registerForRestoration(global::Doroti.Framework.Widgets.IRestorableProperty property, string restorationId)
    {
        DartRuntimePrimitives.Assert(() => (property._restorationId is null) || _debugDoingRestore && (property._restorationId == restorationId), () => (object?)$"Property is already registered under {property._restorationId}.");
        DartRuntimePrimitives.Assert(() => _debugDoingRestore || !_properties.Keys.map<global::Doroti.Framework.Widgets.IRestorableProperty, string?>((r) => r._restorationId).contains(restorationId), () => (object?)$"\"{restorationId}\" is already registered to another property.");
        bool hasSerializedValue = bucket?.contains(restorationId) ?? false;
        object? initialValue = hasSerializedValue ? property.fromPrimitivesObject(bucket!.read<object>(restorationId)) : property.createDefaultValueObject();
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
        DartRuntimePrimitives.Assert(() => (property._restorationId == restorationId) && Equals(property._owner, this) && _properties.ContainsKey(property));
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

    public virtual void unregisterFromRestoration(global::Doroti.Framework.Widgets.IRestorableProperty property)
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
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = _bucket;
        DartRuntimePrimitives.Assert(() => !restorePending);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: _currentParent, restorePending: false);
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
            global::Doroti.Framework.Services.RestorationBucket? potentialNewParent = RestorationScope.maybeOf(context);
            return (!Equals(potentialNewParent, _currentParent)) && (potentialNewParent?.isReplacing ?? false);
        }
    }
    public virtual bool _debugDoingRestore => DartRuntimePrimitives.ConvertValue<bool>(_debugPropertiesWaitingForReregistration is not null);
    public virtual void _doRestore(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
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
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(((Func<List<global::Doroti.Framework.Foundation.DiagnosticsNode>>)(() => { var __collection41817 = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>(); __collection41817.Add(new global::Doroti.Framework.Foundation.ErrorSummary("Previously registered RestorableProperties must be re-registered in \"restoreState\".")); __collection41817.Add(new global::Doroti.Framework.Foundation.ErrorDescription($"The RestorableProperties with the following IDs were not re-registered to {this} when " + "\"restoreState\" was called:")); __collection41817.AddRange(_debugPropertiesWaitingForReregistration!.map<global::Doroti.Framework.Widgets.IRestorableProperty, global::Doroti.Framework.Foundation.DiagnosticsNode>((property) => new global::Doroti.Framework.Foundation.ErrorDescription($" * {property._restorationId}"))); return __collection41817; }))()));
                }
                _debugPropertiesWaitingForReregistration = null;
                return true;
            });
    }

    public virtual bool _updateBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? parent, bool restorePending)
    {
        if ((restorationId is null) || (parent is null))
        {
            bool didReplace = _setNewBucketIfNecessary(newBucket: null, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => _bucket is null);
            return didReplace;
        }
        DartRuntimePrimitives.Assert(() => restorationId is not null);
        if (restorePending || (_bucket is null))
        {
            global::Doroti.Framework.Services.RestorationBucket newBucketLocal = parent.claimChild(restorationId!, debugOwner: this);
            bool didReplaceLocal = _setNewBucketIfNecessary(newBucket: newBucketLocal, restorePending: restorePending);
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

    public virtual bool _setNewBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? newBucket, bool restorePending)
    {
        if (Equals(newBucket, _bucket))
        {
            return false;
        }
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = _bucket;
        _bucket = newBucket;
        if (!restorePending)
        {
            if (_bucket is not null)
            {
                _properties.Keys.forEach((__arg0) => ((global::System.Action<global::Doroti.Framework.Widgets.IRestorableProperty>)_updateProperty)(__arg0));
            }
            didToggleBucket(oldBucket);
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _updateProperty(global::Doroti.Framework.Widgets.IRestorableProperty property)
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

    public virtual void _unregister(global::Doroti.Framework.Widgets.IRestorableProperty property)
    {
        global::System.Action listener = _properties.remove(property)!;
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
    internal static global::Doroti.Framework.Painting.TextStyle? _m2StateInputStyle(global::Doroti.Framework.Widgets.BuildContext context) => WidgetStateTextStyle.CreateResolveWith((states) =>
    {
        ThemeData theme = Theme.of(context);
        if (states.Contains(WidgetState.disabled))
        {
            return new global::Doroti.Framework.Painting.TextStyle(color: theme.disabledColor);
        }
        return new global::Doroti.Framework.Painting.TextStyle(color: theme.textTheme.titleMedium?.color);
        throw new InvalidOperationException("Dart closure completed without a value.");
    });
}

public static partial class Text_fieldLibrary
{
    internal static global::Doroti.Framework.Painting.TextStyle _m2CounterErrorStyle(global::Doroti.Framework.Widgets.BuildContext context) => Theme.of(context).textTheme.bodySmall!.copyWith(color: Theme.of(context).colorScheme.error);
}

public static partial class Text_fieldLibrary
{
    internal static global::Doroti.Framework.Painting.TextStyle? _m3StateInputStyle(global::Doroti.Framework.Widgets.BuildContext context) => WidgetStateTextStyle.CreateResolveWith((states) =>
    {
        if (states.Contains(WidgetState.disabled))
        {
            return new global::Doroti.Framework.Painting.TextStyle(color: Theme.of(context).textTheme.bodyLarge!.color?.withOpacity(0.38));
        }
        return new global::Doroti.Framework.Painting.TextStyle(color: Theme.of(context).textTheme.bodyLarge!.color);
        throw new InvalidOperationException("Dart closure completed without a value.");
    });
}

public static partial class Text_fieldLibrary
{
    internal static global::Doroti.Framework.Painting.TextStyle _m3InputStyle(global::Doroti.Framework.Widgets.BuildContext context) => Theme.of(context).textTheme.bodyLarge!;
}

public static partial class Text_fieldLibrary
{
    internal static global::Doroti.Framework.Painting.TextStyle _m3CounterErrorStyle(global::Doroti.Framework.Widgets.BuildContext context) => Theme.of(context).textTheme.bodySmall!.copyWith(color: Theme.of(context).colorScheme.error);
}
