// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/text_form_field.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class TextFormField : FormField<string>
{
    public virtual TextEditingController? controller { get; private set; }
    public virtual object groupId { get; private set; } = default!;
    public virtual Action<string>? onChanged { get; private set; }

    public TextFormField(Key? key = null, object groupId = default!, TextEditingController? controller = null, string? initialValue = null, FocusNode? focusNode = null, string? forceErrorText = null, InputDecoration? decoration = default!, TextInputType? keyboardType = null, TextCapitalization textCapitalization = TextCapitalization.none, TextInputAction? textInputAction = null, TextStyle? style = null, Painting.StrutStyle? strutStyle = null, TextDirection? textDirection = null, TextAlign textAlign = TextAlign.start, TextAlignVertical? textAlignVertical = null, bool autofocus = false, bool readOnly = false, ToolbarOptions? toolbarOptions = null, bool? showCursor = null, string obscuringCharacter = "•", bool obscureText = false, bool autocorrect = true, SmartDashesType? smartDashesType = null, SmartQuotesType? smartQuotesType = null, bool enableSuggestions = true, MaxLengthEnforcement? maxLengthEnforcement = null, long? maxLines = 1, long? minLines = null, bool expands = false, long? maxLength = null, Action<string>? onChanged = null, Action? onTap = null, bool onTapAlwaysCalled = false, Action<Gestures.PointerDownEvent>? onTapOutside = null, Action<Gestures.PointerUpEvent>? onTapUpOutside = null, Action? onEditingComplete = null, Action<string>? onFieldSubmitted = null, Action<string?>? onSaved = null, Func<string?, string?>? validator = null, Func<BuildContext, string, Widget>? errorBuilder = null, List<TextInputFormatter>? inputFormatters = null, bool? enabled = null, bool? ignorePointers = null, double cursorWidth = 2.0, double? cursorHeight = null, Radius? cursorRadius = null, Color? cursorColor = null, Color? cursorErrorColor = null, Brightness? keyboardAppearance = null, EdgeInsets scrollPadding = default!, bool? enableInteractiveSelection = null, bool? selectAllOnFocus = null, TextSelectionControls? selectionControls = null, InputCounterWidgetBuilder? buildCounter = null, ScrollPhysics? scrollPhysics = null, IEnumerable<string>? autofillHints = null, AutovalidateMode? autovalidateMode = null, ScrollController? scrollController = null, string? restorationId = null, bool enableIMEPersonalizedLearning = true, MouseCursor? mouseCursor = null, Func<BuildContext, EditableTextState, Widget>? contextMenuBuilder = default!, SpellCheckConfiguration? spellCheckConfiguration = null, TextMagnifierConfiguration? magnifierConfiguration = null, UndoHistoryController? undoController = null, Action<string, DartMap<string, object?>>? onAppPrivateCommand = null, bool? cursorOpacityAnimates = null, BoxHeightStyle? selectionHeightStyle = null, BoxWidthStyle? selectionWidthStyle = null, Gestures.DragStartBehavior dragStartBehavior = Gestures.DragStartBehavior.start, ContentInsertionConfiguration? contentInsertionConfiguration = null, WidgetStatesController? statesController = null, Clip clipBehavior = Clip.hardEdge, bool scribbleEnabled = true, bool? stylusHandwritingEnabled = null, bool canRequestFocus = true, List<Locale>? hintLocales = null) : base(key: key, forceErrorText: forceErrorText, onSaved: onSaved, validator: validator, errorBuilder: errorBuilder, restorationId: restorationId, initialValue: (controller is not null) ? controller.text : (initialValue ?? ""), enabled: (enabled ?? decoration?.enabled) ?? true, autovalidateMode: autovalidateMode ?? AutovalidateMode.disabled, builder: (field) =>
    {
        var state = ((_TextFormFieldState__text_form_field?)field)!;
        InputDecoration effectiveDecoration = (decoration ?? new InputDecoration()).applyDefaults(InputDecorationTheme.of(((_TextFormFieldState__text_form_field)field).context));
        string? errorTextLocal = ((_TextFormFieldState__text_form_field)field).errorText;
        if (errorTextLocal is not null)
        {
            effectiveDecoration = (errorBuilder is not null) ? effectiveDecoration.copyWith(error: errorBuilder(state.context, errorTextLocal)) : effectiveDecoration.copyWith(errorText: errorTextLocal);
        }
        void onChangedHandler(string value)
        {
            ((_TextFormFieldState__text_form_field)field).didChange(value);
            onChanged?.Invoke(value);
        }
        return new UnmanagedRestorationScope(bucket: ((_TextFormFieldState__text_form_field)field).bucket, child: new TextField(groupId: groupId ?? typeof(EditableText), restorationId: restorationId, controller: state._effectiveController, focusNode: focusNode, decoration: effectiveDecoration, keyboardType: keyboardType, textInputAction: textInputAction, style: style, strutStyle: strutStyle, textAlign: textAlign, textAlignVertical: textAlignVertical, textDirection: textDirection, textCapitalization: textCapitalization, autofocus: autofocus, statesController: statesController, toolbarOptions: toolbarOptions, readOnly: readOnly, showCursor: showCursor, obscuringCharacter: obscuringCharacter, obscureText: obscureText, autocorrect: autocorrect, smartDashesType: smartDashesType ?? (obscureText ? SmartDashesType.disabled : SmartDashesType.enabled), smartQuotesType: smartQuotesType ?? (obscureText ? SmartQuotesType.disabled : SmartQuotesType.enabled), enableSuggestions: enableSuggestions, maxLengthEnforcement: maxLengthEnforcement, maxLines: maxLines, minLines: minLines, expands: expands, maxLength: maxLength, onChanged: onChangedHandler, onTap: onTap, onTapAlwaysCalled: onTapAlwaysCalled, onTapOutside: onTapOutside, onTapUpOutside: onTapUpOutside, onEditingComplete: onEditingComplete, onSubmitted: onFieldSubmitted, inputFormatters: inputFormatters, enabled: (enabled ?? decoration?.enabled) ?? true, ignorePointers: ignorePointers, cursorWidth: cursorWidth, cursorHeight: cursorHeight, cursorRadius: cursorRadius, cursorColor: cursorColor, cursorErrorColor: cursorErrorColor, scrollPadding: scrollPadding ?? EdgeInsets.CreateAll(20.0), scrollPhysics: scrollPhysics, keyboardAppearance: keyboardAppearance, enableInteractiveSelection: enableInteractiveSelection ?? !obscureText || !readOnly, selectAllOnFocus: selectAllOnFocus, selectionControls: selectionControls, buildCounter: buildCounter, autofillHints: autofillHints, scrollController: scrollController, enableIMEPersonalizedLearning: enableIMEPersonalizedLearning, mouseCursor: mouseCursor, contextMenuBuilder: contextMenuBuilder, spellCheckConfiguration: spellCheckConfiguration, magnifierConfiguration: magnifierConfiguration, undoController: undoController, onAppPrivateCommand: onAppPrivateCommand, cursorOpacityAnimates: cursorOpacityAnimates, selectionHeightStyle: selectionHeightStyle ?? EditableText.defaultSelectionHeightStyle, selectionWidthStyle: selectionWidthStyle ?? EditableText.defaultSelectionWidthStyle, dragStartBehavior: dragStartBehavior, contentInsertionConfiguration: contentInsertionConfiguration, clipBehavior: clipBehavior, scribbleEnabled: scribbleEnabled, stylusHandwritingEnabled: DartRuntimePrimitives.RequireValue(stylusHandwritingEnabled), canRequestFocus: canRequestFocus, hintLocales: hintLocales));
        throw new InvalidOperationException("Dart closure completed without a value.");
    })
    {
        object __groupId = groupId ?? typeof(EditableText);
        InputDecoration? __decoration = decoration ?? new InputDecoration();
        EdgeInsets __scrollPadding = scrollPadding ?? EdgeInsets.CreateAll(20.0);
        Func<BuildContext, EditableTextState, Widget>? __contextMenuBuilder = contextMenuBuilder ?? _defaultContextMenuBuilder;
        bool __stylusHandwritingEnabled = stylusHandwritingEnabled ?? EditableText.defaultStylusHandwritingEnabled;
        this.groupId = __groupId;
        this.controller = controller;
        this.onChanged = onChanged;
        System.Diagnostics.Debug.Assert((initialValue is null) || (controller is null));
        System.Diagnostics.Debug.Assert(obscuringCharacter.Length == 1L);
        System.Diagnostics.Debug.Assert((maxLines is null) || (DartRuntimePrimitives.RequireValue(maxLines) > 0L));
        System.Diagnostics.Debug.Assert((minLines is null) || (DartRuntimePrimitives.RequireValue(minLines) > 0L));
        System.Diagnostics.Debug.Assert(maxLines is null || minLines is null || maxLines >= DartRuntimePrimitives.RequireValue(minLines));
        System.Diagnostics.Debug.Assert(!expands || (maxLines is null) && (minLines is null));
        System.Diagnostics.Debug.Assert(!obscureText || (maxLines == 1L));
        System.Diagnostics.Debug.Assert((maxLength is null) || (DartRuntimePrimitives.RequireValue(maxLength) == TextField.noMaxLength) || (DartRuntimePrimitives.RequireValue(maxLength) > 0L));
        System.Diagnostics.Debug.Assert((errorBuilder is null) || (__decoration?.errorText is null));
    }

    internal static Widget _defaultContextMenuBuilder(BuildContext context, EditableTextState editableTextState)
    {
        if (SystemContextMenu.isSupportedByField(editableTextState))
        {
            return SystemContextMenu.CreateEditableText(editableTextState: editableTextState);
        }
        return AdaptiveTextSelectionToolbar.CreateEditableText(editableTextState: editableTextState);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _TextFormFieldState__text_form_field());
}

internal class _TextFormFieldState__text_form_field : FormFieldState<string>
{
    internal virtual RestorableTextEditingController? _controller { get; set; } = default;
    internal virtual string? _initialValue { get; private set; }

    internal virtual TextEditingController _effectiveController => DartRuntimePrimitives.ConvertValue<TextEditingController>(_textFormField.controller ?? _controller!.value);
    internal virtual TextFormField _textFormField => ((TextFormField?)base.widget)!;
    public override void restoreState(RestorationBucket? oldBucket, bool initialRestore)
    {
        base.restoreState(oldBucket, initialRestore);
        if (_controller is not null)
        {
            _registerController();
        }
        setValue(_effectiveController.text);
    }

    internal virtual void _registerController()
    {
        DartRuntimePrimitives.Assert(() => _controller is not null);
        registerForRestoration(_controller!, "controller");
    }

    internal virtual void _createLocalController(TextEditingValue? value = null)
    {
        DartRuntimePrimitives.Assert(() => _controller is null);
        _controller = (value is null) ? RestorableTextEditingController.Create() : new RestorableTextEditingController(value);
        if (!restorePending)
        {
            _registerController();
        }
    }

    public override void initState()
    {
        base.initState();
        if (_textFormField.controller is null)
        {
            _createLocalController((widget.initialValue is not null) ? new TextEditingValue(text: widget.initialValue!) : null);
        }
        else
        {
            _textFormField.controller!.addListener(_handleControllerChanged);
        }
        _initialValue = _textFormField.initialValue ?? _textFormField.controller?.text;
    }

    public override void didUpdateWidget(FormField<string> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(_textFormField.controller, ((TextFormField)oldWidget).controller))
        {
            ((TextFormField)oldWidget).controller?.removeListener(_handleControllerChanged);
            _textFormField.controller?.addListener(_handleControllerChanged);
            if ((((TextFormField)oldWidget).controller is not null) && (_textFormField.controller is null))
            {
                _createLocalController(((TextFormField)oldWidget).controller!.value);
            }
            if (_textFormField.controller is not null)
            {
                setValue(_textFormField.controller!.text);
                if (((TextFormField)oldWidget).controller is null)
                {
                    unregisterFromRestoration(_controller!);
                    _controller!.dispose();
                    _controller = null;
                }
            }
        }
    }

    public override void dispose()
    {
        _textFormField.controller?.removeListener(_handleControllerChanged);
        _controller?.dispose();
        base.dispose();
    }

    public override void didChange(string? value)
    {
        base.didChange(value);
        if (_effectiveController.text != value)
        {
            _effectiveController.value = new TextEditingValue(text: value ?? "");
        }
    }

    public override void reset()
    {
        _effectiveController.value = new TextEditingValue(text: _initialValue ?? "");
        base.reset();
        _textFormField.onChanged?.Invoke(_effectiveController.text);
    }

    internal virtual void _handleControllerChanged()
    {
        if (_effectiveController.text != value)
        {
            didChange(_effectiveController.text);
        }
    }

}
