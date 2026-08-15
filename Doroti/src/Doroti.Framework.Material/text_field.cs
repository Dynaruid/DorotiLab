// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/text_field.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public delegate global::Doroti.Generated.Framework.Widgets.Widget? InputCounterWidgetBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, long currentLength, bool isFocused, long? maxLength);

internal class _TextFieldSelectionGestureDetectorBuilder__text_field : global::Doroti.Generated.Framework.Widgets.TextSelectionGestureDetectorBuilder
{
    internal virtual _TextFieldState__text_field _state { get; private set; } = default!;

    internal _TextFieldSelectionGestureDetectorBuilder__text_field(_TextFieldState__text_field state) : base(@delegate: state)
    {
        this._state = state;
    }

    public override bool onUserTapAlwaysCalled => this._state.widget.onTapAlwaysCalled;
    public override void onUserTap()
    {
        this._state.widget.onTap?.Invoke();
    }

}

public class TextField : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.TextMagnifierConfiguration? magnifierConfiguration { get; private set; }
    public virtual object groupId { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.TextEditingController? controller { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual InputDecoration? decoration { get; private set; }
    public virtual global::Doroti.Generated.Framework.Services.TextInputType keyboardType { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Services.TextInputAction? textInputAction { get; private set; }
    public virtual global::Doroti.Generated.Framework.Services.TextCapitalization textCapitalization { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? style { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.StrutStyle? strutStyle { get; private set; }
    public virtual TextAlign textAlign { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.TextAlignVertical? textAlignVertical { get; private set; }
    public virtual TextDirection? textDirection { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController { get; private set; }
    public virtual string obscuringCharacter { get; private set; } = default!;
    public virtual bool obscureText { get; private set; } = default!;
    public virtual bool? autocorrect { get; private set; }
    public virtual global::Doroti.Generated.Framework.Services.SmartDashesType smartDashesType { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Services.SmartQuotesType smartQuotesType { get; private set; } = default!;
    public virtual bool enableSuggestions { get; private set; } = default!;
    public virtual long? maxLines { get; private set; }
    public virtual long? minLines { get; private set; }
    public virtual bool expands { get; private set; } = default!;
    public virtual bool readOnly { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.ToolbarOptions? toolbarOptions { get; private set; }
    public virtual bool? showCursor { get; private set; }
    public static long noMaxLength = -1L;
    public virtual long? maxLength { get; private set; }
    public virtual global::Doroti.Generated.Framework.Services.MaxLengthEnforcement? maxLengthEnforcement { get; private set; }
    public virtual global::System.Action<string>? onChanged { get; private set; }
    public virtual global::System.Action? onEditingComplete { get; private set; }
    public virtual global::System.Action<string>? onSubmitted { get; private set; }
    public virtual global::System.Action<string, DartMap<string, object>>? onAppPrivateCommand { get; private set; }
    public virtual List<global::Doroti.Generated.Framework.Services.TextInputFormatter>? inputFormatters { get; private set; }
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
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsets scrollPadding { get; private set; } = default!;
    public virtual bool enableInteractiveSelection { get; private set; } = default!;
    public virtual bool? selectAllOnFocus { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.TextSelectionControls? selectionControls { get; private set; }
    public virtual global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual global::System.Action? onTap { get; private set; }
    public virtual bool onTapAlwaysCalled { get; private set; } = default!;
    public virtual global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerDownEvent>? onTapOutside { get; private set; }
    public virtual global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerUpEvent>? onTapUpOutside { get; private set; }
    public virtual global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual InputCounterWidgetBuilder? buildCounter { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.ScrollPhysics? scrollPhysics { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.ScrollController? scrollController { get; private set; }
    public virtual IEnumerable<string>? autofillHints { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual string? restorationId { get; private set; }
    public virtual bool scribbleEnabled { get; private set; } = default!;
    public virtual bool stylusHandwritingEnabled { get; private set; } = default!;
    public virtual bool enableIMEPersonalizedLearning { get; private set; } = default!;
    public virtual bool? enableInlinePrediction { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.ContentInsertionConfiguration? contentInsertionConfiguration { get; private set; }
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.EditableTextState, global::Doroti.Generated.Framework.Widgets.Widget>? contextMenuBuilder { get; private set; }
    public virtual bool canRequestFocus { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.UndoHistoryController? undoController { get; private set; }
    public virtual List<Locale>? hintLocales { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.SpellCheckConfiguration? spellCheckConfiguration { get; private set; }
    public static global::Doroti.Generated.Framework.Painting.TextStyle materialMisspelledTextStyle = new global::Doroti.Generated.Framework.Painting.TextStyle(decoration: TextDecoration.underline, decorationColor: Colors.red, decorationStyle: TextDecorationStyle.wavy);

    public TextField(global::Doroti.Generated.Framework.Foundation.Key? key = null, object groupId = default!, global::Doroti.Generated.Framework.Widgets.TextEditingController? controller = null, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, global::Doroti.Generated.Framework.Widgets.UndoHistoryController? undoController = null, InputDecoration? decoration = default!, global::Doroti.Generated.Framework.Services.TextInputType? keyboardType = null, global::Doroti.Generated.Framework.Services.TextInputAction? textInputAction = null, global::Doroti.Generated.Framework.Services.TextCapitalization textCapitalization = global::Doroti.Generated.Framework.Services.TextCapitalization.none, global::Doroti.Generated.Framework.Painting.TextStyle? style = null, global::Doroti.Generated.Framework.Painting.StrutStyle? strutStyle = null, TextAlign textAlign = TextAlign.start, global::Doroti.Generated.Framework.Painting.TextAlignVertical? textAlignVertical = null, TextDirection? textDirection = null, bool readOnly = false, global::Doroti.Generated.Framework.Widgets.ToolbarOptions? toolbarOptions = null, bool? showCursor = null, bool autofocus = false, global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController = null, string obscuringCharacter = "•", bool obscureText = false, bool? autocorrect = null, global::Doroti.Generated.Framework.Services.SmartDashesType? smartDashesType = null, global::Doroti.Generated.Framework.Services.SmartQuotesType? smartQuotesType = null, bool enableSuggestions = true, long? maxLines = 1, long? minLines = null, bool expands = false, long? maxLength = null, global::Doroti.Generated.Framework.Services.MaxLengthEnforcement? maxLengthEnforcement = null, global::System.Action<string>? onChanged = null, global::System.Action? onEditingComplete = null, global::System.Action<string>? onSubmitted = null, global::System.Action<string, DartMap<string, object>>? onAppPrivateCommand = null, List<global::Doroti.Generated.Framework.Services.TextInputFormatter>? inputFormatters = null, bool? enabled = null, bool? ignorePointers = null, double cursorWidth = 2.0, double? cursorHeight = null, Radius? cursorRadius = null, bool? cursorOpacityAnimates = null, Color? cursorColor = null, Color? cursorErrorColor = null, BoxHeightStyle? selectionHeightStyle = null, BoxWidthStyle? selectionWidthStyle = null, Brightness? keyboardAppearance = null, global::Doroti.Generated.Framework.Painting.EdgeInsets scrollPadding = default!, global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Generated.Framework.Gestures.DragStartBehavior.start, bool? enableInteractiveSelection = null, bool? selectAllOnFocus = null, global::Doroti.Generated.Framework.Widgets.TextSelectionControls? selectionControls = null, global::System.Action? onTap = null, bool onTapAlwaysCalled = false, global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerDownEvent>? onTapOutside = null, global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerUpEvent>? onTapUpOutside = null, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null, InputCounterWidgetBuilder? buildCounter = null, global::Doroti.Generated.Framework.Widgets.ScrollController? scrollController = null, global::Doroti.Generated.Framework.Widgets.ScrollPhysics? scrollPhysics = null, IEnumerable<string>? autofillHints = default!, global::Doroti.Generated.Framework.Widgets.ContentInsertionConfiguration? contentInsertionConfiguration = null, Clip clipBehavior = Clip.hardEdge, string? restorationId = null, bool scribbleEnabled = true, bool? stylusHandwritingEnabled = null, bool enableIMEPersonalizedLearning = true, bool? enableInlinePrediction = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.EditableTextState, global::Doroti.Generated.Framework.Widgets.Widget>? contextMenuBuilder = default!, bool canRequestFocus = true, global::Doroti.Generated.Framework.Widgets.SpellCheckConfiguration? spellCheckConfiguration = null, global::Doroti.Generated.Framework.Widgets.TextMagnifierConfiguration? magnifierConfiguration = null, List<Locale>? hintLocales = null) : base(key: key)
    {
        object __groupId = groupId ?? typeof(global::Doroti.Generated.Framework.Widgets.EditableText);
        InputDecoration? __decoration = decoration ?? new InputDecoration();
        global::Doroti.Generated.Framework.Painting.EdgeInsets __scrollPadding = scrollPadding ?? global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateAll(20.0);
        IEnumerable<string>? __autofillHints = autofillHints ?? new List<string>();
        bool __stylusHandwritingEnabled = stylusHandwritingEnabled ?? global::Doroti.Generated.Framework.Widgets.EditableText.defaultStylusHandwritingEnabled;
        global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.EditableTextState, global::Doroti.Generated.Framework.Widgets.Widget>? __contextMenuBuilder = contextMenuBuilder ?? _defaultContextMenuBuilder;
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
        this.smartDashesType = (smartDashesType ?? ((obscureText ? global::Doroti.Generated.Framework.Services.SmartDashesType.disabled : global::Doroti.Generated.Framework.Services.SmartDashesType.enabled)));
        this.smartQuotesType = (smartQuotesType ?? ((obscureText ? global::Doroti.Generated.Framework.Services.SmartQuotesType.disabled : global::Doroti.Generated.Framework.Services.SmartQuotesType.enabled)));
        this.keyboardType = (keyboardType ?? (((maxLines == 1L) ? global::Doroti.Generated.Framework.Services.TextInputType.text : global::Doroti.Generated.Framework.Services.TextInputType.multiline)));
        this.enableInteractiveSelection = (enableInteractiveSelection ?? ((!readOnly || !obscureText)));
        System.Diagnostics.Debug.Assert((obscuringCharacter.Length == 1L));
        System.Diagnostics.Debug.Assert(((maxLines is null) || (DartRuntimePrimitives.RequireValue(maxLines) > 0L)));
        System.Diagnostics.Debug.Assert(((minLines is null) || (DartRuntimePrimitives.RequireValue(minLines) > 0L)));
        System.Diagnostics.Debug.Assert(((((maxLines is null)) || ((minLines is null))) || ((maxLines >= DartRuntimePrimitives.RequireValue(minLines)))));
        System.Diagnostics.Debug.Assert((!expands || (((maxLines is null) && (minLines is null)))));
        System.Diagnostics.Debug.Assert((!obscureText || (maxLines == 1L)));
        System.Diagnostics.Debug.Assert((((maxLength is null) || (DartRuntimePrimitives.RequireValue(maxLength) == TextField.noMaxLength)) || (DartRuntimePrimitives.RequireValue(maxLength) > 0L)));
        System.Diagnostics.Debug.Assert(((!DartRuntimePrimitives.Identical(textInputAction, global::Doroti.Generated.Framework.Services.TextInputAction.newline) || (maxLines == 1L)) || !DartRuntimePrimitives.Identical(keyboardType, global::Doroti.Generated.Framework.Services.TextInputType.text)));
    }

    public virtual bool selectionEnabled => this.enableInteractiveSelection;
    internal static global::Doroti.Generated.Framework.Widgets.Widget _defaultContextMenuBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.EditableTextState editableTextState)
    {
        if (SystemContextMenu.isSupportedByField(editableTextState))
        {
            return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)global::Doroti.Generated.Framework.Widgets.SystemContextMenu.CreateEditableText(editableTextState: editableTextState));
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)AdaptiveTextSelectionToolbar.CreateEditableText(editableTextState: editableTextState));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Generated.Framework.Widgets.Widget defaultSpellCheckSuggestionsToolbarBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.EditableTextState editableTextState)
    {
        switch (global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
                {
                    return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)CupertinoSpellCheckSuggestionsToolbar.CreateEditableText(editableTextState: editableTextState));
                }
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
                {
                    return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)SpellCheckSuggestionsToolbar.CreateEditableText(editableTextState: editableTextState));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Generated.Framework.Widgets.SpellCheckConfiguration inferAndroidSpellCheckConfiguration(global::Doroti.Generated.Framework.Widgets.SpellCheckConfiguration? configuration)
    {
        if (((configuration is null) || (object.Equals(configuration, global::Doroti.Generated.Framework.Widgets.SpellCheckConfiguration.CreateDisabled()))))
        {
            return global::Doroti.Generated.Framework.Widgets.SpellCheckConfiguration.CreateDisabled();
        }
        return ((global::Doroti.Generated.Framework.Widgets.SpellCheckConfiguration)(object?)configuration.copyWith(misspelledTextStyle: (((global::Doroti.Generated.Framework.Widgets.SpellCheckConfiguration)configuration).misspelledTextStyle ?? TextField.materialMisspelledTextStyle), spellCheckSuggestionsToolbarBuilder: ((((global::Doroti.Generated.Framework.Widgets.SpellCheckConfiguration)configuration).spellCheckSuggestionsToolbarBuilder ?? (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.EditableTextState, global::Doroti.Generated.Framework.Widgets.Widget>)TextField.defaultSpellCheckSuggestionsToolbarBuilder))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _TextFieldState__text_field());
    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.TextEditingController>("controller", this.controller, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.FocusNode>("focusNode", this.focusNode, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.UndoHistoryController>("undoController", this.undoController, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("enabled", this.enabled, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<InputDecoration>("decoration", this.decoration, defaultValue: new InputDecoration()));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Services.TextInputType>("keyboardType", this.keyboardType, defaultValue: global::Doroti.Generated.Framework.Services.TextInputType.text));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("style", this.style, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("autofocus", this.autofocus, defaultValue: false));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<string>("obscuringCharacter", this.obscuringCharacter, defaultValue: "•"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("obscureText", this.obscureText, defaultValue: false));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("autocorrect", this.autocorrect, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Services.SmartDashesType>("smartDashesType", this.smartDashesType, defaultValue: (this.obscureText ? global::Doroti.Generated.Framework.Services.SmartDashesType.disabled : global::Doroti.Generated.Framework.Services.SmartDashesType.enabled)));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Services.SmartQuotesType>("smartQuotesType", this.smartQuotesType, defaultValue: (this.obscureText ? global::Doroti.Generated.Framework.Services.SmartQuotesType.disabled : global::Doroti.Generated.Framework.Services.SmartQuotesType.enabled)));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("enableSuggestions", this.enableSuggestions, defaultValue: true));
        properties.add(new global::Doroti.Generated.Framework.Foundation.IntProperty("maxLines", this.maxLines, defaultValue: 1L));
        properties.add(new global::Doroti.Generated.Framework.Foundation.IntProperty("minLines", this.minLines, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("expands", this.expands, defaultValue: false));
        properties.add(new global::Doroti.Generated.Framework.Foundation.IntProperty("maxLength", this.maxLength, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Services.MaxLengthEnforcement>("maxLengthEnforcement", this.maxLengthEnforcement, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Services.TextInputAction>("textInputAction", this.textInputAction, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Services.TextCapitalization>("textCapitalization", this.textCapitalization, defaultValue: global::Doroti.Generated.Framework.Services.TextCapitalization.none));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextAlign>("textAlign", this.textAlign, defaultValue: global::Doroti.Ui.TextAlign.start));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextAlignVertical>("textAlignVertical", this.textAlignVertical, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", this.textDirection, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("cursorWidth", this.cursorWidth, defaultValue: 2.0));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("cursorHeight", this.cursorHeight, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Radius>("cursorRadius", this.cursorRadius, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("cursorOpacityAnimates", this.cursorOpacityAnimates, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("cursorColor", this.cursorColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("cursorErrorColor", this.cursorErrorColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Brightness>("keyboardAppearance", this.keyboardAppearance, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>("scrollPadding", this.scrollPadding, defaultValue: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateAll(20.0)));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("selectionEnabled", value: this.selectionEnabled, defaultValue: true, ifFalse: "selection disabled"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.TextSelectionControls>("selectionControls", this.selectionControls, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.ScrollController>("scrollController", this.scrollController, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.ScrollPhysics>("scrollPhysics", this.scrollPhysics, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Clip>("clipBehavior", this.clipBehavior, defaultValue: Clip.hardEdge));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("scribbleEnabled", this.scribbleEnabled, defaultValue: true));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("stylusHandwritingEnabled", DartRuntimePrimitives.RequireValue(this.stylusHandwritingEnabled), defaultValue: global::Doroti.Generated.Framework.Widgets.EditableText.defaultStylusHandwritingEnabled));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("enableIMEPersonalizedLearning", this.enableIMEPersonalizedLearning, defaultValue: true));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool?>("enableInlinePrediction", this.enableInlinePrediction, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.SpellCheckConfiguration>("spellCheckConfiguration", this.spellCheckConfiguration, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<List<string>>("contentCommitMimeTypes", (this.contentInsertionConfiguration?.allowedMimeTypes ?? new List<string>()), defaultValue: ((this.contentInsertionConfiguration is null) ? new List<string>() : global::Doroti.Generated.Framework.Widgets.Editable_textLibrary.kDefaultContentInsertionMimeTypes)));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<List<global::Doroti.Ui.Locale>?>("hintLocales", this.hintLocales, defaultValue: null));
    }

}

internal class _TextFieldState__text_field : global::Doroti.Generated.Framework.Widgets.State<TextField>, global::Doroti.Generated.Framework.Widgets.RestorationMixin<TextField>, global::Doroti.Generated.Framework.Widgets.TextSelectionGestureDetectorBuilderDelegate, global::Doroti.Generated.Framework.Services.AutofillClient
{
    internal virtual global::Doroti.Generated.Framework.Widgets.RestorableTextEditingController? _controller { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Widgets.FocusNode? _focusNode { get; set; } = default;
    internal virtual bool _isHovering { get; set; } = false;
    internal virtual bool _showSelectionHandles { get; set; } = false;
    internal virtual _TextFieldSelectionGestureDetectorBuilder__text_field _selectionGestureDetectorBuilder { get; set; } = default!;
    public virtual bool forcePressEnabled { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<global::Doroti.Generated.Framework.Widgets.EditableTextState> editableTextKey { get; private set; } = global::Doroti.Generated.Framework.Widgets.GlobalKey<global::Doroti.Generated.Framework.Widgets.EditableTextState>.Create();
    internal virtual global::Doroti.Generated.Framework.Widgets.WidgetStatesController? _internalStatesController { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Services.RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<dynamic, global::System.Action> _properties { get; set; } = new DartMap<dynamic, global::System.Action>();
    public virtual List<dynamic>? _debugPropertiesWaitingForReregistration { get; set; } = default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual global::Doroti.Generated.Framework.Services.RestorationBucket? _currentParent { get; set; } = default;

    internal virtual global::Doroti.Generated.Framework.Widgets.TextEditingController _effectiveController => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.TextEditingController>((((TextField)this.widget).controller ?? this._controller!.value));
    internal virtual global::Doroti.Generated.Framework.Widgets.FocusNode _effectiveFocusNode => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.FocusNode>((((TextField)this.widget).focusNode ?? (_focusNode ??= new global::Doroti.Generated.Framework.Widgets.FocusNode())));
    internal virtual global::Doroti.Generated.Framework.Services.MaxLengthEnforcement _effectiveMaxLengthEnforcement => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Services.MaxLengthEnforcement>(((((TextField)this.widget).maxLengthEnforcement ?? (global::Doroti.Generated.Framework.Services.MaxLengthEnforcement)LengthLimitingTextInputFormatter.getDefaultMaxLengthEnforcement(Theme.of(this.context).platform))));
    public virtual bool needsCounter => DartRuntimePrimitives.ConvertValue<bool>((((((TextField)this.widget).maxLength is not null) && (((TextField)this.widget).decoration is not null)) && (((TextField)this.widget).decoration!.counterText is null)));
    public virtual bool selectionEnabled => DartRuntimePrimitives.ConvertValue<bool>((((TextField)this.widget).selectionEnabled && this._isEnabled));
    internal virtual bool _isEnabled => DartRuntimePrimitives.ConvertValue<bool>(((((TextField)this.widget).enabled ?? ((TextField)this.widget).decoration?.enabled) ?? true));
    internal virtual long _currentLength => this._effectiveController.value.text.characters().Count;
    internal virtual bool _hasIntrinsicError => DartRuntimePrimitives.ConvertValue<bool>((((((TextField)this.widget).maxLength is not null) && (DartRuntimePrimitives.RequireValue(((TextField)this.widget).maxLength) > 0L)) && (((((TextField)this.widget).controller is null) ? (!this.restorePending && (this._effectiveController.value.text.characters().Count > DartRuntimePrimitives.RequireValue(((TextField)this.widget).maxLength))) : (this._effectiveController.value.text.characters().Count > DartRuntimePrimitives.RequireValue(((TextField)this.widget).maxLength))))));
    internal virtual bool _hasError => DartRuntimePrimitives.ConvertValue<bool>((((((TextField)this.widget).decoration?.errorText is not null) || (((TextField)this.widget).decoration?.error is not null)) || this._hasIntrinsicError));
    internal virtual global::Doroti.Ui.Color _errorColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((((TextField)this.widget).cursorErrorColor ?? _getEffectiveDecoration().errorStyle?.color) ?? Theme.of(this.context).colorScheme.error));
    internal virtual InputDecoration _getEffectiveDecoration()
    {
        MaterialLocalizations localizations__46852 = MaterialLocalizations.of(this.context);
        ThemeData themeData__46923 = Theme.of(this.context);
        InputDecorationThemeData decorationTheme__46989 = InputDecorationTheme.of(this.context);
        InputDecoration effectiveDecoration__47067 = ((((TextField)this.widget).decoration ?? new InputDecoration())).applyDefaults(decorationTheme__46989).copyWith(enabled: this._isEnabled, hintMaxLines: ((((TextField)this.widget).decoration?.hintMaxLines ?? decorationTheme__46989.hintMaxLines) ?? ((TextField)this.widget).maxLines));
        if (((effectiveDecoration__47067.counter is not null) || (effectiveDecoration__47067.counterText is not null)))
        {
            return effectiveDecoration__47067;
        }
        global::Doroti.Generated.Framework.Widgets.Widget? counter__47657 = default!;
        long currentLength__47680 = this._currentLength;
        if ((((effectiveDecoration__47067.counter is null) && (effectiveDecoration__47067.counterText is null)) && (((TextField)this.widget).buildCounter is not null)))
        {
            bool isFocused__47866 = ((global::Doroti.Generated.Framework.Widgets.FocusNode)this._effectiveFocusNode).hasFocus;
            global::Doroti.Generated.Framework.Widgets.Widget? builtCounter__47928 = ((TextField)this.widget).buildCounter!(this.context, currentLength: currentLength__47680, maxLength: ((TextField)this.widget).maxLength, isFocused: isFocused__47866);
            if ((builtCounter__47928 is not null))
            {
                counter__47657 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Semantics(container: true, liveRegion: isFocused__47866, child: builtCounter__47928));
            }
            return effectiveDecoration__47067.copyWith(counter: counter__47657);
        }
        if ((((TextField)this.widget).maxLength is null))
        {
            return effectiveDecoration__47067;
        }
        var counterText__48482 = $"{currentLength__47680}";
        var semanticCounterText__48522 = "";
        if ((DartRuntimePrimitives.RequireValue(((TextField)this.widget).maxLength) > 0L))
        {
            counterText__48482 += $"/{((TextField)this.widget).maxLength}";
            long remaining__48735 = ((DartRuntimePrimitives.RequireValue(((TextField)this.widget).maxLength) - currentLength__47680)).clamp(0L, DartRuntimePrimitives.RequireValue(((TextField)this.widget).maxLength));
            semanticCounterText__48522 = localizations__46852.remainingTextFieldCharacterCount(remaining__48735);
        }
        if (this._hasIntrinsicError)
        {
            return effectiveDecoration__47067.copyWith(errorText: (effectiveDecoration__47067.errorText ?? ""), counterStyle: (effectiveDecoration__47067.errorStyle ?? ((themeData__46923.useMaterial3 ? Text_fieldLibrary._m3CounterErrorStyle(this.context) : Text_fieldLibrary._m2CounterErrorStyle(this.context)))), counterText: counterText__48482, semanticCounterText: semanticCounterText__48522);
        }
        return effectiveDecoration__47067.copyWith(counterText: counterText__48482, semanticCounterText: semanticCounterText__48522);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void initState()
    {
        base.initState();
        _selectionGestureDetectorBuilder = new _TextFieldSelectionGestureDetectorBuilder__text_field(state: this);
        if ((((TextField)this.widget).controller is null))
        {
            _createLocalController();
        }
        this._effectiveFocusNode.canRequestFocus = (((TextField)this.widget).canRequestFocus && this._isEnabled);
        this._effectiveFocusNode.addListener(() => this._handleFocusChanged());
        _initStatesController();
    }

    internal virtual bool _canRequestFocus
    {
        get
        {
            global::Doroti.Generated.Framework.Widgets.NavigationMode mode__49923 = (MediaQuery.maybeNavigationModeOf(this.context) ?? global::Doroti.Generated.Framework.Widgets.NavigationMode.traditional);
            return (mode__49923 switch { global::Doroti.Generated.Framework.Widgets.NavigationMode.traditional => (((TextField)this.widget).canRequestFocus && this._isEnabled), global::Doroti.Generated.Framework.Widgets.NavigationMode.directional => true, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            return default!;
        }
    }
    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket__41020 = this._bucket;
        bool needsRestore__41056 = this.restorePending;
        _currentParent = RestorationScope.maybeOf(this.context);
        bool didReplaceBucket__41159 = _updateBucketIfNecessary(parent: this._currentParent, restorePending: needsRestore__41056);
        if (needsRestore__41056)
        {
            _doRestore(oldBucket__41020);
        }
        if (didReplaceBucket__41159)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket__41020, this._bucket)));
            oldBucket__41020?.dispose();
        }
        this._effectiveFocusNode.canRequestFocus = this._canRequestFocus;
    }

    public override void didUpdateWidget(TextField oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        didUpdateRestorationId();
        if (((((TextField)this.widget).controller is null) && (((TextField)oldWidget).controller is not null)))
        {
            _createLocalController(((TextField)oldWidget).controller!.value);
        }
        else
        {
            if (((((TextField)this.widget).controller is not null) && (((TextField)oldWidget).controller is null)))
            {
                unregisterFromRestoration(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.RestorableProperty<object>>(this._controller!));
                this._controller!.dispose();
                _controller = null;
            }
        }
        if ((!object.Equals(((TextField)this.widget).focusNode, ((TextField)oldWidget).focusNode)))
        {
            ((((TextField)oldWidget).focusNode ?? this._focusNode))?.removeListener(() => this._handleFocusChanged());
            ((((TextField)this.widget).focusNode ?? this._focusNode))?.addListener(() => this._handleFocusChanged());
        }
        this._effectiveFocusNode.canRequestFocus = this._canRequestFocus;
        if (((((global::Doroti.Generated.Framework.Widgets.FocusNode)this._effectiveFocusNode).hasFocus && (((TextField)this.widget).readOnly != ((TextField)oldWidget).readOnly)) && this._isEnabled))
        {
            if (((global::Doroti.Generated.Framework.Widgets.TextEditingController)this._effectiveController).selection.isCollapsed)
            {
                _showSelectionHandles = !((TextField)this.widget).readOnly;
            }
        }
        if ((object.Equals(((TextField)this.widget).statesController, ((TextField)oldWidget).statesController)))
        {
            this._statesController.update(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled, !this._isEnabled);
            this._statesController.update(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered, this._isHovering);
            this._statesController.update(global::Doroti.Generated.Framework.Widgets.WidgetState.focused, ((global::Doroti.Generated.Framework.Widgets.FocusNode)this._effectiveFocusNode).hasFocus);
            this._statesController.update(global::Doroti.Generated.Framework.Widgets.WidgetState.error, this._hasError);
        }
        else
        {
            ((TextField)oldWidget).statesController?.removeListener(() => this._handleStatesControllerChange());
            if ((((TextField)this.widget).statesController is not null))
            {
                this._internalStatesController?.dispose();
                _internalStatesController = null;
            }
            _initStatesController();
        }
    }

    public virtual void restoreState(global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket, bool initialRestore)
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
    }

    internal virtual void _createLocalController(global::Doroti.Generated.Framework.Services.TextEditingValue? value = null)
    {
        DartRuntimePrimitives.Assert(() => (this._controller is null));
        _controller = ((value is null) ? global::Doroti.Generated.Framework.Widgets.RestorableTextEditingController.Create() : new global::Doroti.Generated.Framework.Widgets.RestorableTextEditingController(value));
        if (!this.restorePending)
        {
            _registerController();
        }
    }

    public virtual string? restorationId => ((TextField)this.widget).restorationId;
    public override void dispose()
    {
        this._effectiveFocusNode.removeListener(() => this._handleFocusChanged());
        this._focusNode?.dispose();
        this._controller?.dispose();
        this._statesController.removeListener(() => this._handleStatesControllerChange());
        this._internalStatesController?.dispose();
        this._properties.forEach(((global::System.Action<dynamic, global::System.Action>)((property, listener) => {
if (!((dynamic)property)._disposed)
{
    property.removeListener((global::System.Action)(() => listener()));
}
})));
        this._bucket?.dispose();
        _bucket = null;
        base.dispose();
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.EditableTextState? _editableText => ((global::Doroti.Generated.Framework.Widgets.GlobalKey<global::Doroti.Generated.Framework.Widgets.EditableTextState>)this.editableTextKey).currentState;
    internal virtual void _requestKeyboard()
    {
        this._editableText?.requestKeyboard();
    }

    internal virtual bool _shouldShowSelectionHandles(global::Doroti.Generated.Framework.Services.SelectionChangedCause? cause)
    {
        if ((!this._selectionGestureDetectorBuilder.shouldShowSelectionToolbar || !this._selectionGestureDetectorBuilder.shouldShowSelectionHandles))
        {
            return false;
        }
        if ((object.Equals(cause, global::Doroti.Generated.Framework.Services.SelectionChangedCause.keyboard)))
        {
            return false;
        }
        if ((((TextField)this.widget).readOnly && ((global::Doroti.Generated.Framework.Widgets.TextEditingController)this._effectiveController).selection.isCollapsed))
        {
            return false;
        }
        if (!this._isEnabled)
        {
            return false;
        }
        if (((object.Equals(cause, global::Doroti.Generated.Framework.Services.SelectionChangedCause.longPress)) || (object.Equals(cause, global::Doroti.Generated.Framework.Services.SelectionChangedCause.stylusHandwriting))))
        {
            return true;
        }
        if ((((global::Doroti.Generated.Framework.Widgets.TextEditingController)this._effectiveController).text.Length != 0))
        {
            return true;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleFocusChanged()
    {
        setState(((global::System.Action)(() => {
})));
        this._statesController.update(global::Doroti.Generated.Framework.Widgets.WidgetState.focused, ((global::Doroti.Generated.Framework.Widgets.FocusNode)this._effectiveFocusNode).hasFocus);
    }

    internal virtual void _handleSelectionChanged(global::Doroti.Generated.Framework.Services.TextSelection selection, global::Doroti.Generated.Framework.Services.SelectionChangedCause? cause)
    {
        bool willShowSelectionHandles__54085 = _shouldShowSelectionHandles(cause);
        if ((willShowSelectionHandles__54085 != this._showSelectionHandles))
        {
            setState(((global::System.Action)(() => {
_showSelectionHandles = willShowSelectionHandles__54085;
})));
        }
        switch (Theme.of(this.context).platform)
        {
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.android:
                {
                    if ((object.Equals(cause, global::Doroti.Generated.Framework.Services.SelectionChangedCause.longPress)))
                    {
                        this._editableText?.bringIntoView(((global::Doroti.Generated.Framework.Services.TextSelection)selection).extent);
                    }
                    break;
                }
        }
        switch (Theme.of(this.context).platform)
        {
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.android:
                {
                    break;
                }
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
                {
                    if ((object.Equals(cause, global::Doroti.Generated.Framework.Services.SelectionChangedCause.drag)))
                    {
                        this._editableText?.hideToolbar();
                    }
                    break;
                }
        }
    }

    internal virtual void _handleSelectionHandleTapped()
    {
        if (((global::Doroti.Generated.Framework.Widgets.TextEditingController)this._effectiveController).selection.isCollapsed)
        {
            this._editableText!.toggleToolbar();
        }
    }

    internal virtual void _handleHover(bool hovering)
    {
        if ((hovering != this._isHovering))
        {
            setState(((global::System.Action)(() => {
_isHovering = hovering;
})));
            this._statesController.update(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered, this._isHovering);
        }
    }

    internal virtual void _handleStatesControllerChange()
    {
        setState(((global::System.Action)(() => {
})));
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.WidgetStatesController _statesController => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStatesController>((((TextField)this.widget).statesController ?? this._internalStatesController!));
    internal virtual void _initStatesController()
    {
        if ((((TextField)this.widget).statesController is null))
        {
            _internalStatesController = new global::Doroti.Generated.Framework.Widgets.WidgetStatesController();
        }
        this._statesController.update(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled, !this._isEnabled);
        this._statesController.update(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered, this._isHovering);
        this._statesController.update(global::Doroti.Generated.Framework.Widgets.WidgetState.focused, ((global::Doroti.Generated.Framework.Widgets.FocusNode)this._effectiveFocusNode).hasFocus);
        this._statesController.update(global::Doroti.Generated.Framework.Widgets.WidgetState.error, this._hasError);
        this._statesController.addListener(() => this._handleStatesControllerChange());
    }

    public virtual string autofillId => this._editableText!.autofillId;
    public virtual void autofill(global::Doroti.Generated.Framework.Services.TextEditingValue newEditingValue) => this._editableText!.autofill(newEditingValue);
    public virtual global::Doroti.Generated.Framework.Services.TextInputConfiguration textInputConfiguration
    {
        get
        {
            List<string>? autofillHints__56592 = ((TextField)this.widget).autofillHints?.ToList().ToList();
            global::Doroti.Generated.Framework.Services.AutofillConfiguration autofillConfiguration__56687 = ((autofillHints__56592 is not null) ? new global::Doroti.Generated.Framework.Services.AutofillConfiguration(uniqueIdentifier: this.autofillId, autofillHints: autofillHints__56592, currentEditingValue: this._effectiveController.value, hintText: ((((TextField)this.widget).decoration ?? new InputDecoration())).hintText) : global::Doroti.Generated.Framework.Services.AutofillConfiguration.disabled);
            return ((global::Doroti.Generated.Framework.Services.TextInputConfiguration)(object?)this._editableText!.textInputConfiguration.copyWith(autofillConfiguration: autofillConfiguration__56687));
            return default!;
        }
    }
    internal virtual global::Doroti.Generated.Framework.Painting.TextStyle _getInputStyleForState(global::Doroti.Generated.Framework.Painting.TextStyle style)
    {
        ThemeData theme__57282 = Theme.of(this.context);
        global::Doroti.Generated.Framework.Painting.TextStyle stateStyle__57329 = ((global::Doroti.Generated.Framework.Painting.TextStyle)(object?)WidgetStateProperty.resolveAs((theme__57282.useMaterial3 ? Text_fieldLibrary._m3StateInputStyle(this.context)! : Text_fieldLibrary._m2StateInputStyle(this.context)!), this._statesController.value));
        global::Doroti.Generated.Framework.Painting.TextStyle providedStyle__57519 = ((global::Doroti.Generated.Framework.Painting.TextStyle)(object?)WidgetStateProperty.resolveAs(style, this._statesController.value));
        return ((global::Doroti.Generated.Framework.Painting.TextStyle)(object?)providedStyle__57519.merge(stateStyle__57329));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        DartRuntimePrimitives.Assert(() => !((((((TextField)this.widget).style is not null) && !((TextField)this.widget).style!.inherit) && (((((TextField)this.widget).style!.fontSize is null) || (((TextField)this.widget).style!.textBaseline is null))))), () => (object?)"inherit false style must supply fontSize and textBaseline");
        ThemeData theme__58107 = Theme.of(context);
        global::Doroti.Generated.Framework.Widgets.DefaultSelectionStyle selectionStyle__58166 = ((global::Doroti.Generated.Framework.Widgets.DefaultSelectionStyle)(object?)DefaultSelectionStyle.of(context));
        global::Doroti.Generated.Framework.Painting.TextStyle? providedStyle__58239 = ((global::Doroti.Generated.Framework.Painting.TextStyle?)(object?)WidgetStateProperty.resolveAs(((TextField)this.widget).style, this._statesController.value));
        global::Doroti.Generated.Framework.Painting.TextStyle style__58364 = ((global::Doroti.Generated.Framework.Painting.TextStyle)(object?)_getInputStyleForState((theme__58107.useMaterial3 ? Text_fieldLibrary._m3InputStyle(context) : theme__58107.textTheme.titleMedium!)).merge(providedStyle__58239));
        global::Doroti.Ui.Brightness keyboardAppearance__58527 = (((TextField)this.widget).keyboardAppearance ?? theme__58107.brightness);
        global::Doroti.Generated.Framework.Widgets.TextEditingController controller__58627 = this._effectiveController;
        global::Doroti.Generated.Framework.Widgets.FocusNode focusNode__58682 = this._effectiveFocusNode;
        var formatters__58725 = ((Func<List<global::Doroti.Generated.Framework.Services.TextInputFormatter>>)(() => { var __collection58738 = new List<global::Doroti.Generated.Framework.Services.TextInputFormatter>(); var __collectionSpread58766 = ((TextField)this.widget).inputFormatters; if (__collectionSpread58766 is not null) { __collection58738.AddRange(__collectionSpread58766); } if ((((TextField)this.widget).maxLength is not null)) { __collection58738.Add(new global::Doroti.Generated.Framework.Services.LengthLimitingTextInputFormatter(((TextField)this.widget).maxLength, maxLengthEnforcement: this._effectiveMaxLengthEnforcement)); } return __collection58738; }))();
        global::Doroti.Generated.Framework.Widgets.SpellCheckConfiguration spellCheckConfiguration__59242 = default!;
        switch (global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
                {
                    spellCheckConfiguration__59242 = CupertinoTextField.inferIOSSpellCheckConfiguration(((TextField)this.widget).spellCheckConfiguration);
                    break;
                }
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
                {
                    spellCheckConfiguration__59242 = TextField.inferAndroidSpellCheckConfiguration(((TextField)this.widget).spellCheckConfiguration);
                    break;
                }
        }
        global::Doroti.Generated.Framework.Widgets.TextSelectionControls? textSelectionControls__59813 = ((TextField)this.widget).selectionControls;
        bool paintCursorAboveText__59878 = default!;
        bool? cursorOpacityAnimates__59910 = ((TextField)this.widget).cursorOpacityAnimates;
        global::Doroti.Ui.Offset? cursorOffset__59976 = default!;
        global::Doroti.Ui.Color cursorColor__60006 = default!;
        global::Doroti.Ui.Color selectionColor__60035 = default!;
        global::Doroti.Ui.Color? autocorrectionTextRectColor__60062 = default!;
        global::Doroti.Ui.Radius? cursorRadius__60103 = ((global::Doroti.Ui.Radius?)(object?)((TextField)this.widget).cursorRadius);
        global::System.Action? handleDidGainAccessibilityFocus__60157 = default!;
        global::System.Action? handleDidLoseAccessibilityFocus__60208 = default!;
        switch (theme__58107.platform)
        {
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS:
                {
                    CupertinoThemeData cupertinoTheme__60336 = CupertinoTheme.of(context);
                    forcePressEnabled = true;
                    textSelectionControls__59813 ??= Text_selectionLibrary.materialTextSelectionHandleControls;
                    paintCursorAboveText__59878 = true;
                    cursorOpacityAnimates__59910 ??= true;
                    cursorColor__60006 = (this._hasError ? this._errorColor : ((((TextField)this.widget).cursorColor ?? ((global::Doroti.Generated.Framework.Widgets.DefaultSelectionStyle)selectionStyle__58166).cursorColor) ?? cupertinoTheme__60336.primaryColor));
                    selectionColor__60035 = (((global::Doroti.Generated.Framework.Widgets.DefaultSelectionStyle)selectionStyle__58166).selectionColor ?? cupertinoTheme__60336.primaryColor.withOpacity(0.4));
                    cursorRadius__60103 ??= global::Doroti.Ui.Radius.circular(2.0);
                    cursorOffset__59976 = new global::Doroti.Ui.Offset((Selectable_textLibrary.iOSHorizontalOffset / MediaQuery.devicePixelRatioOf(context)), 0);
                    autocorrectionTextRectColor__60062 = selectionColor__60035;
                    break;
                }
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
                {
                    CupertinoThemeData cupertinoTheme__61104 = CupertinoTheme.of(context);
                    forcePressEnabled = false;
                    textSelectionControls__59813 ??= Desktop_text_selectionLibrary.desktopTextSelectionHandleControls;
                    paintCursorAboveText__59878 = true;
                    cursorOpacityAnimates__59910 ??= false;
                    cursorColor__60006 = (this._hasError ? this._errorColor : ((((TextField)this.widget).cursorColor ?? ((global::Doroti.Generated.Framework.Widgets.DefaultSelectionStyle)selectionStyle__58166).cursorColor) ?? cupertinoTheme__61104.primaryColor));
                    selectionColor__60035 = (((global::Doroti.Generated.Framework.Widgets.DefaultSelectionStyle)selectionStyle__58166).selectionColor ?? cupertinoTheme__61104.primaryColor.withOpacity(0.4));
                    cursorRadius__60103 ??= global::Doroti.Ui.Radius.circular(2.0);
                    cursorOffset__59976 = new global::Doroti.Ui.Offset((Selectable_textLibrary.iOSHorizontalOffset / MediaQuery.devicePixelRatioOf(context)), 0);
                    handleDidGainAccessibilityFocus__60157 = (global::System.Action)(() => {
if ((!((global::Doroti.Generated.Framework.Widgets.FocusNode)this._effectiveFocusNode).hasFocus && ((global::Doroti.Generated.Framework.Widgets.FocusNode)this._effectiveFocusNode).canRequestFocus))
{
    this._effectiveFocusNode.requestFocus();
}
});
                    handleDidLoseAccessibilityFocus__60208 = (global::System.Action)(() => {
this._effectiveFocusNode.unfocus();
});
                    break;
                }
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia:
                {
                    forcePressEnabled = false;
                    textSelectionControls__59813 ??= Text_selectionLibrary.materialTextSelectionHandleControls;
                    paintCursorAboveText__59878 = false;
                    cursorOpacityAnimates__59910 ??= false;
                    cursorColor__60006 = (this._hasError ? this._errorColor : ((((TextField)this.widget).cursorColor ?? ((global::Doroti.Generated.Framework.Widgets.DefaultSelectionStyle)selectionStyle__58166).cursorColor) ?? theme__58107.colorScheme.primary));
                    selectionColor__60035 = (((global::Doroti.Generated.Framework.Widgets.DefaultSelectionStyle)selectionStyle__58166).selectionColor ?? theme__58107.colorScheme.primary.withOpacity(0.4));
                    break;
                }
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
                {
                    forcePressEnabled = false;
                    textSelectionControls__59813 ??= Desktop_text_selectionLibrary.desktopTextSelectionHandleControls;
                    paintCursorAboveText__59878 = false;
                    cursorOpacityAnimates__59910 ??= false;
                    cursorColor__60006 = (this._hasError ? this._errorColor : ((((TextField)this.widget).cursorColor ?? ((global::Doroti.Generated.Framework.Widgets.DefaultSelectionStyle)selectionStyle__58166).cursorColor) ?? theme__58107.colorScheme.primary));
                    selectionColor__60035 = (((global::Doroti.Generated.Framework.Widgets.DefaultSelectionStyle)selectionStyle__58166).selectionColor ?? theme__58107.colorScheme.primary.withOpacity(0.4));
                    handleDidGainAccessibilityFocus__60157 = (global::System.Action)(() => {
if ((!((global::Doroti.Generated.Framework.Widgets.FocusNode)this._effectiveFocusNode).hasFocus && ((global::Doroti.Generated.Framework.Widgets.FocusNode)this._effectiveFocusNode).canRequestFocus))
{
    this._effectiveFocusNode.requestFocus();
}
});
                    handleDidLoseAccessibilityFocus__60208 = (global::System.Action)(() => {
this._effectiveFocusNode.unfocus();
});
                    break;
                }
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
                {
                    forcePressEnabled = false;
                    textSelectionControls__59813 ??= Desktop_text_selectionLibrary.desktopTextSelectionHandleControls;
                    paintCursorAboveText__59878 = false;
                    cursorOpacityAnimates__59910 ??= false;
                    cursorColor__60006 = (this._hasError ? this._errorColor : ((((TextField)this.widget).cursorColor ?? ((global::Doroti.Generated.Framework.Widgets.DefaultSelectionStyle)selectionStyle__58166).cursorColor) ?? theme__58107.colorScheme.primary));
                    selectionColor__60035 = (((global::Doroti.Generated.Framework.Widgets.DefaultSelectionStyle)selectionStyle__58166).selectionColor ?? theme__58107.colorScheme.primary.withOpacity(0.4));
                    handleDidGainAccessibilityFocus__60157 = (global::System.Action)(() => {
if ((!((global::Doroti.Generated.Framework.Widgets.FocusNode)this._effectiveFocusNode).hasFocus && ((global::Doroti.Generated.Framework.Widgets.FocusNode)this._effectiveFocusNode).canRequestFocus))
{
    this._effectiveFocusNode.requestFocus();
}
});
                    handleDidLoseAccessibilityFocus__60208 = (global::System.Action)(() => {
this._effectiveFocusNode.unfocus();
});
                    break;
                }
        }
        global::Doroti.Generated.Framework.Widgets.Widget child__64443 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.RepaintBoundary(child: new global::Doroti.Generated.Framework.Widgets.UnmanagedRestorationScope(bucket: this.bucket, child: new global::Doroti.Generated.Framework.Widgets.EditableText(key: this.editableTextKey, readOnly: (((TextField)this.widget).readOnly || !this._isEnabled), toolbarOptions: ((TextField)this.widget).toolbarOptions, showCursor: ((TextField)this.widget).showCursor, showSelectionHandles: this._showSelectionHandles, controller: controller__58627, focusNode: focusNode__58682, undoController: ((TextField)this.widget).undoController, keyboardType: ((TextField)this.widget).keyboardType, textInputAction: ((TextField)this.widget).textInputAction, textCapitalization: ((TextField)this.widget).textCapitalization, style: style__58364, strutStyle: ((TextField)this.widget).strutStyle, textAlign: ((TextField)this.widget).textAlign, textDirection: ((TextField)this.widget).textDirection, autofocus: ((TextField)this.widget).autofocus, obscuringCharacter: ((TextField)this.widget).obscuringCharacter, obscureText: ((TextField)this.widget).obscureText, autocorrect: ((TextField)this.widget).autocorrect, smartDashesType: ((TextField)this.widget).smartDashesType, smartQuotesType: ((TextField)this.widget).smartQuotesType, enableSuggestions: ((TextField)this.widget).enableSuggestions, maxLines: ((TextField)this.widget).maxLines, minLines: ((TextField)this.widget).minLines, expands: ((TextField)this.widget).expands, selectionColor: (((global::Doroti.Generated.Framework.Widgets.FocusNode)focusNode__58682).hasFocus ? selectionColor__60035 : null), selectionControls: (((TextField)this.widget).selectionEnabled ? textSelectionControls__59813 : null), onChanged: (global::System.Action<string>?)((TextField)this.widget).onChanged, onSelectionChanged: (global::System.Action<global::Doroti.Generated.Framework.Services.TextSelection, global::Doroti.Generated.Framework.Services.SelectionChangedCause?>)this._handleSelectionChanged, onEditingComplete: () => ((TextField)this.widget).onEditingComplete(), onSubmitted: (global::System.Action<string>?)((TextField)this.widget).onSubmitted, onAppPrivateCommand: (global::System.Action<string, DartMap<string, object>>?)((TextField)this.widget).onAppPrivateCommand, groupId: ((TextField)this.widget).groupId, onSelectionHandleTapped: () => this._handleSelectionHandleTapped(), onTapOutside: (global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerDownEvent>?)((TextField)this.widget).onTapOutside, onTapUpOutside: (global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerUpEvent>?)((TextField)this.widget).onTapUpOutside, inputFormatters: formatters__58725, rendererIgnoresPointer: true, mouseCursor: global::Doroti.Generated.Framework.Services.MouseCursor.defer, cursorWidth: ((TextField)this.widget).cursorWidth, cursorHeight: ((TextField)this.widget).cursorHeight, cursorRadius: cursorRadius__60103, cursorColor: cursorColor__60006, selectionHeightStyle: ((TextField)this.widget).selectionHeightStyle, selectionWidthStyle: ((TextField)this.widget).selectionWidthStyle, cursorOpacityAnimates: cursorOpacityAnimates__59910 ?? false, cursorOffset: cursorOffset__59976, paintCursorAboveText: paintCursorAboveText__59878, backgroundCursorColor: CupertinoColors.inactiveGray, scrollPadding: ((TextField)this.widget).scrollPadding, keyboardAppearance: keyboardAppearance__58527, enableInteractiveSelection: ((TextField)this.widget).enableInteractiveSelection, selectAllOnFocus: ((TextField)this.widget).selectAllOnFocus ?? false, dragStartBehavior: ((TextField)this.widget).dragStartBehavior, scrollController: ((TextField)this.widget).scrollController, scrollPhysics: ((TextField)this.widget).scrollPhysics, autofillHints: ((TextField)this.widget).autofillHints.Cast<string>(), autofillClient: this, autocorrectionTextRectColor: autocorrectionTextRectColor__60062, clipBehavior: ((TextField)this.widget).clipBehavior, restorationId: "editable", scribbleEnabled: ((TextField)this.widget).scribbleEnabled, stylusHandwritingEnabled: ((TextField)this.widget).stylusHandwritingEnabled, enableIMEPersonalizedLearning: ((TextField)this.widget).enableIMEPersonalizedLearning, enableInlinePrediction: ((TextField)this.widget).enableInlinePrediction, contentInsertionConfiguration: ((TextField)this.widget).contentInsertionConfiguration, contextMenuBuilder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.EditableTextState, global::Doroti.Generated.Framework.Widgets.Widget>?)((TextField)this.widget).contextMenuBuilder, spellCheckConfiguration: spellCheckConfiguration__59242, magnifierConfiguration: (((TextField)this.widget).magnifierConfiguration ?? TextMagnifier.adaptiveMagnifierConfiguration), hintLocales: ((TextField)this.widget).hintLocales))));
        if ((((TextField)this.widget).decoration is not null))
        {
            child__64443 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.AnimatedBuilder(animation: global::Doroti.Generated.Framework.Foundation.Listenable.CreateMerge(new List<global::Doroti.Generated.Framework.Foundation.Listenable> { focusNode__58682, controller__58627 }.Cast<global::Doroti.Generated.Framework.Foundation.Listenable?>()), builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)((context, child) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new InputDecorator(decoration: _getEffectiveDecoration(), baseStyle: ((TextField)this.widget).style, textAlign: ((TextField)this.widget).textAlign, textAlignVertical: ((TextField)this.widget).textAlignVertical, isHovering: this._isHovering, isFocused: ((global::Doroti.Generated.Framework.Widgets.FocusNode)focusNode__58682).hasFocus, isEmpty: (controller__58627.value.text.Length == 0), expands: ((TextField)this.widget).expands, child: child));
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: child__64443));
        }
        global::Doroti.Generated.Framework.Services.MouseCursor effectiveMouseCursor__68947 = ((global::Doroti.Generated.Framework.Services.MouseCursor)(object?)WidgetStateProperty.resolveAs<global::Doroti.Generated.Framework.Services.MouseCursor>((((TextField)this.widget).mouseCursor ?? global::Doroti.Generated.Framework.Widgets.WidgetStateMouseCursor.textable), this._statesController.value));
        long? semanticsMaxValueLength__69129 = default!;
        if ((((!object.Equals(this._effectiveMaxLengthEnforcement, global::Doroti.Generated.Framework.Services.MaxLengthEnforcement.none)) && (((TextField)this.widget).maxLength is not null)) && (DartRuntimePrimitives.RequireValue(((TextField)this.widget).maxLength) > 0L)))
        {
            semanticsMaxValueLength__69129 = ((TextField)this.widget).maxLength;
        }
        else
        {
            semanticsMaxValueLength__69129 = null;
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.MouseRegion(cursor: effectiveMouseCursor__68947, onEnter: ((global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerEnterEvent>)((@event) => { _handleHover(true); })), onExit: ((global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerExitEvent>)((@event) => { _handleHover(false); })), child: new global::Doroti.Generated.Framework.Widgets.TextFieldTapRegion(child: new global::Doroti.Generated.Framework.Widgets.IgnorePointer(ignoring: (((TextField)this.widget).ignorePointers ?? !this._isEnabled), child: new global::Doroti.Generated.Framework.Widgets.AnimatedBuilder(animation: controller__58627, builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)((context, child) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Semantics(enabled: this._isEnabled, maxValueLength: semanticsMaxValueLength__69129, currentValueLength: this._currentLength, onTap: ((global::System.Action)(((TextField)this.widget).readOnly ? null : (() => {
if (!((global::Doroti.Generated.Framework.Widgets.TextEditingController)this._effectiveController).selection.isValid)
{
    this._effectiveController.selection = global::Doroti.Generated.Framework.Services.TextSelection.CreateCollapsed(offset: ((global::Doroti.Generated.Framework.Widgets.TextEditingController)this._effectiveController).text.Length);
}
_requestKeyboard();
}))), onDidGainAccessibilityFocus: () => handleDidGainAccessibilityFocus__60157(), onDidLoseAccessibilityFocus: () => handleDidLoseAccessibilityFocus__60208(), onFocus: ((global::System.Action)(this._isEnabled ? (() => {
DartRuntimePrimitives.Assert(() => ((global::Doroti.Generated.Framework.Widgets.FocusNode)this._effectiveFocusNode).canRequestFocus, () => (object?)"Received SemanticsAction.focus from the engine. However, the FocusNode " + "of this text field cannot gain focus. This likely indicates a bug. " + "If this text field cannot be focused (e.g. because it is not " + "enabled), then its corresponding semantics node must be configured " + "such that the assistive technology cannot request focus on it.");
if ((((global::Doroti.Generated.Framework.Widgets.FocusNode)this._effectiveFocusNode).canRequestFocus && !((global::Doroti.Generated.Framework.Widgets.FocusNode)this._effectiveFocusNode).hasFocus))
{
    this._effectiveFocusNode.requestFocus();
}
else
{
    if (!((TextField)this.widget).readOnly)
    {
        _requestKeyboard();
    }
}
}) : null)), child: child));
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: this._selectionGestureDetectorBuilder.buildGestureDetector(behavior: global::Doroti.Generated.Framework.Rendering.HitTestBehavior.translucent, child: child__64443))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Services.RestorationBucket? bucket => this._bucket;
    public virtual void didToggleBucket(global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => (this._bucket?.isReplacing != true));
    }

    public virtual void registerForRestoration(dynamic property, string restorationId)
    {
        DartRuntimePrimitives.Assert(() => ((((dynamic)property)._restorationId is null) || ((this._debugDoingRestore && (((dynamic)property)._restorationId == restorationId)))), () => (object?)$"Property is already registered under {((dynamic)property)._restorationId}.");
        DartRuntimePrimitives.Assert(() => (this._debugDoingRestore || !this._properties.Keys.map<dynamic, string?>(((r) => ((dynamic)r)._restorationId)).contains(restorationId)), () => (object?)$"\"{restorationId}\" is already registered to another property.");
        bool hasSerializedValue__36723 = (this.bucket?.contains(restorationId) ?? false);
        object? initialValue__36804 = (hasSerializedValue__36723 ? property.fromPrimitives(this.bucket!.read<object>(restorationId)) : property.createDefaultValue());
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
            property.addListener((global::System.Action)(() => listener()));
            this._properties[property] = (global::System.Action)listener;
        }
        DartRuntimePrimitives.Assert(() => (((((dynamic)property)._restorationId == restorationId) && (object.Equals(((dynamic)property)._owner, this))) && this._properties.ContainsKey(property)));
        property.initWithValue((dynamic)initialValue__36804);
        if (((!hasSerializedValue__36723 && ((dynamic)property).enabled) && (this.bucket is not null)))
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
        global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket__39230 = this._bucket;
        DartRuntimePrimitives.Assert(() => !this.restorePending);
        bool didReplaceBucket__39295 = _updateBucketIfNecessary(parent: this._currentParent, restorePending: false);
        if (didReplaceBucket__39295)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket__39230, this._bucket)));
            DartRuntimePrimitives.Assert(() => ((this._bucket is null) || (oldBucket__39230 is null)));
            oldBucket__39230?.dispose();
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
            global::Doroti.Generated.Framework.Services.RestorationBucket? potentialNewParent__40517 = ((global::Doroti.Generated.Framework.Services.RestorationBucket?)(object?)RestorationScope.maybeOf(this.context));
            return ((!object.Equals(potentialNewParent__40517, this._currentParent)) && ((potentialNewParent__40517?.isReplacing ?? false)));
            return default!;
        }
    }
    public virtual bool _debugDoingRestore => DartRuntimePrimitives.ConvertValue<bool>((this._debugPropertiesWaitingForReregistration is not null));
    public virtual void _doRestore(global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket)
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
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(((Func<List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>>)(() => { var __collection41817 = new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>(); __collection41817.Add(new global::Doroti.Generated.Framework.Foundation.ErrorSummary("Previously registered RestorableProperties must be re-registered in \"restoreState\".")); __collection41817.Add(new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"The RestorableProperties with the following IDs were not re-registered to {this} when " + "\"restoreState\" was called:")); __collection41817.AddRange(this._debugPropertiesWaitingForReregistration!.map<dynamic, global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>(((property) => new global::Doroti.Generated.Framework.Foundation.ErrorDescription($" * {((dynamic)property)._restorationId}")))); return __collection41817; }))()));
                }
                this._debugPropertiesWaitingForReregistration = null;
                return true;
            });
    }

    public virtual bool _updateBucketIfNecessary(global::Doroti.Generated.Framework.Services.RestorationBucket? parent, bool restorePending)
    {
        if (((this.restorationId is null) || (parent is null)))
        {
            bool didReplace__42801 = _setNewBucketIfNecessary(newBucket: null, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (this._bucket is null));
            return didReplace__42801;
        }
        DartRuntimePrimitives.Assert(() => (this.restorationId is not null));
        if ((restorePending || (this._bucket is null)))
        {
            global::Doroti.Generated.Framework.Services.RestorationBucket newBucket__43086 = ((global::Doroti.Generated.Framework.Services.RestorationBucket)(object?)parent.claimChild(this.restorationId!, debugOwner: this));
            bool didReplace__43168 = _setNewBucketIfNecessary(newBucket: newBucket__43086, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (object.Equals(this._bucket, newBucket__43086)));
            return didReplace__43168;
        }
        DartRuntimePrimitives.Assert(() => (this._bucket is not null));
        DartRuntimePrimitives.Assert(() => !restorePending);
        this._bucket!.rename(this.restorationId!);
        parent.adoptChild(this._bucket!);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _setNewBucketIfNecessary(global::Doroti.Generated.Framework.Services.RestorationBucket? newBucket, bool restorePending)
    {
        if ((object.Equals(newBucket, this._bucket)))
        {
            return false;
        }
        global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket__43946 = this._bucket;
        this._bucket = newBucket;
        if (!restorePending)
        {
            if ((this._bucket is not null))
            {
                this._properties.Keys.forEach((__arg0) => ((global::System.Action<dynamic>)this._updateProperty)(__arg0));
            }
            didToggleBucket(oldBucket__43946);
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
        global::System.Action listener__44576 = this._properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
        property.removeListener((global::System.Action)(() => listener__44576()));
        property._unregister();
    }

}

public static partial class Text_fieldLibrary
{
    internal static global::Doroti.Generated.Framework.Painting.TextStyle? _m2StateInputStyle(global::Doroti.Generated.Framework.Widgets.BuildContext context) => global::Doroti.Generated.Framework.Widgets.WidgetStateTextStyle.CreateResolveWith(((states) => {
ThemeData theme__72935 = Theme.of(context);
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return new global::Doroti.Generated.Framework.Painting.TextStyle(color: theme__72935.disabledColor);
}
return new global::Doroti.Generated.Framework.Painting.TextStyle(color: theme__72935.textTheme.titleMedium?.color);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
}

public static partial class Text_fieldLibrary
{
    internal static global::Doroti.Generated.Framework.Painting.TextStyle _m2CounterErrorStyle(global::Doroti.Generated.Framework.Widgets.BuildContext context) => Theme.of(context).textTheme.bodySmall!.copyWith(color: Theme.of(context).colorScheme.error);
}

public static partial class Text_fieldLibrary
{
    internal static global::Doroti.Generated.Framework.Painting.TextStyle? _m3StateInputStyle(global::Doroti.Generated.Framework.Widgets.BuildContext context) => global::Doroti.Generated.Framework.Widgets.WidgetStateTextStyle.CreateResolveWith(((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return new global::Doroti.Generated.Framework.Painting.TextStyle(color: Theme.of(context).textTheme.bodyLarge!.color?.withOpacity(0.38));
}
return new global::Doroti.Generated.Framework.Painting.TextStyle(color: Theme.of(context).textTheme.bodyLarge!.color);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
}

public static partial class Text_fieldLibrary
{
    internal static global::Doroti.Generated.Framework.Painting.TextStyle _m3InputStyle(global::Doroti.Generated.Framework.Widgets.BuildContext context) => Theme.of(context).textTheme.bodyLarge!;
}

public static partial class Text_fieldLibrary
{
    internal static global::Doroti.Generated.Framework.Painting.TextStyle _m3CounterErrorStyle(global::Doroti.Generated.Framework.Widgets.BuildContext context) => Theme.of(context).textTheme.bodySmall!.copyWith(color: Theme.of(context).colorScheme.error);
}
