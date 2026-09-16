// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/text_form_field.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class TextFormField : global::Doroti.Framework.Widgets.FormField<string>
{
    public virtual global::Doroti.Framework.Widgets.TextEditingController? controller { get; private set; }
    public virtual object groupId { get; private set; } = default!;
    public virtual global::System.Action<string>? onChanged { get; private set; }

    public TextFormField(global::Doroti.Framework.Foundation.Key? key = null, object groupId = default!, global::Doroti.Framework.Widgets.TextEditingController? controller = null, string? initialValue = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, string? forceErrorText = null, InputDecoration? decoration = default!, global::Doroti.Framework.Services.TextInputType? keyboardType = null, global::Doroti.Framework.Services.TextCapitalization textCapitalization = TextCapitalization.none, global::Doroti.Framework.Services.TextInputAction? textInputAction = null, global::Doroti.Framework.Painting.TextStyle? style = null, global::Doroti.Framework.Painting.StrutStyle? strutStyle = null, TextDirection? textDirection = null, TextAlign textAlign = TextAlign.start, global::Doroti.Framework.Painting.TextAlignVertical? textAlignVertical = null, bool autofocus = false, bool readOnly = false, global::Doroti.Framework.Widgets.ToolbarOptions? toolbarOptions = null, bool? showCursor = null, string obscuringCharacter = "•", bool obscureText = false, bool autocorrect = true, global::Doroti.Framework.Services.SmartDashesType? smartDashesType = null, global::Doroti.Framework.Services.SmartQuotesType? smartQuotesType = null, bool enableSuggestions = true, global::Doroti.Framework.Services.MaxLengthEnforcement? maxLengthEnforcement = null, long? maxLines = 1, long? minLines = null, bool expands = false, long? maxLength = null, global::System.Action<string>? onChanged = null, global::System.Action? onTap = null, bool onTapAlwaysCalled = false, global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>? onTapOutside = null, global::System.Action<global::Doroti.Framework.Gestures.PointerUpEvent>? onTapUpOutside = null, global::System.Action? onEditingComplete = null, global::System.Action<string>? onFieldSubmitted = null, global::System.Action<string?>? onSaved = null, global::System.Func<string?, string?>? validator = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, string, global::Doroti.Framework.Widgets.Widget>? errorBuilder = null, List<global::Doroti.Framework.Services.TextInputFormatter>? inputFormatters = null, bool? enabled = null, bool? ignorePointers = null, double cursorWidth = 2.0, double? cursorHeight = null, Radius? cursorRadius = null, Color? cursorColor = null, Color? cursorErrorColor = null, Brightness? keyboardAppearance = null, global::Doroti.Framework.Painting.EdgeInsets scrollPadding = default!, bool? enableInteractiveSelection = null, bool? selectAllOnFocus = null, global::Doroti.Framework.Widgets.TextSelectionControls? selectionControls = null, InputCounterWidgetBuilder? buildCounter = null, global::Doroti.Framework.Widgets.ScrollPhysics? scrollPhysics = null, IEnumerable<string>? autofillHints = null, global::Doroti.Framework.Widgets.AutovalidateMode? autovalidateMode = null, global::Doroti.Framework.Widgets.ScrollController? scrollController = null, string? restorationId = null, bool enableIMEPersonalizedLearning = true, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget>? contextMenuBuilder = default!, global::Doroti.Framework.Widgets.SpellCheckConfiguration? spellCheckConfiguration = null, global::Doroti.Framework.Widgets.TextMagnifierConfiguration? magnifierConfiguration = null, global::Doroti.Framework.Widgets.UndoHistoryController? undoController = null, global::System.Action<string, DartMap<string, object?>>? onAppPrivateCommand = null, bool? cursorOpacityAnimates = null, BoxHeightStyle? selectionHeightStyle = null, BoxWidthStyle? selectionWidthStyle = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = Gestures.DragStartBehavior.start, global::Doroti.Framework.Widgets.ContentInsertionConfiguration? contentInsertionConfiguration = null, global::Doroti.Framework.Widgets.WidgetStatesController? statesController = null, Clip clipBehavior = Clip.hardEdge, bool scribbleEnabled = true, bool? stylusHandwritingEnabled = null, bool canRequestFocus = true, List<Locale>? hintLocales = null) : base(key: key, forceErrorText: forceErrorText, onSaved: onSaved, validator: validator, errorBuilder: errorBuilder, restorationId: restorationId, initialValue: (controller is not null) ? controller.text : (initialValue ?? ""), enabled: (enabled ?? decoration?.enabled) ?? true, autovalidateMode: autovalidateMode ?? AutovalidateMode.disabled, builder: (field) =>
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
        return new global::Doroti.Framework.Widgets.UnmanagedRestorationScope(bucket: ((_TextFormFieldState__text_form_field)field).bucket, child: new TextField(groupId: groupId ?? typeof(global::Doroti.Framework.Widgets.EditableText), restorationId: restorationId, controller: state._effectiveController, focusNode: focusNode, decoration: effectiveDecoration, keyboardType: keyboardType, textInputAction: textInputAction, style: style, strutStyle: strutStyle, textAlign: textAlign, textAlignVertical: textAlignVertical, textDirection: textDirection, textCapitalization: textCapitalization, autofocus: autofocus, statesController: statesController, toolbarOptions: toolbarOptions, readOnly: readOnly, showCursor: showCursor, obscuringCharacter: obscuringCharacter, obscureText: obscureText, autocorrect: autocorrect, smartDashesType: smartDashesType ?? (obscureText ? SmartDashesType.disabled : SmartDashesType.enabled), smartQuotesType: smartQuotesType ?? (obscureText ? SmartQuotesType.disabled : SmartQuotesType.enabled), enableSuggestions: enableSuggestions, maxLengthEnforcement: maxLengthEnforcement, maxLines: maxLines, minLines: minLines, expands: expands, maxLength: maxLength, onChanged: onChangedHandler, onTap: onTap, onTapAlwaysCalled: onTapAlwaysCalled, onTapOutside: onTapOutside, onTapUpOutside: onTapUpOutside, onEditingComplete: onEditingComplete, onSubmitted: onFieldSubmitted, inputFormatters: inputFormatters, enabled: (enabled ?? decoration?.enabled) ?? true, ignorePointers: ignorePointers, cursorWidth: cursorWidth, cursorHeight: cursorHeight, cursorRadius: cursorRadius, cursorColor: cursorColor, cursorErrorColor: cursorErrorColor, scrollPadding: scrollPadding ?? EdgeInsets.CreateAll(20.0), scrollPhysics: scrollPhysics, keyboardAppearance: keyboardAppearance, enableInteractiveSelection: enableInteractiveSelection ?? !obscureText || !readOnly, selectAllOnFocus: selectAllOnFocus, selectionControls: selectionControls, buildCounter: buildCounter, autofillHints: autofillHints, scrollController: scrollController, enableIMEPersonalizedLearning: enableIMEPersonalizedLearning, mouseCursor: mouseCursor, contextMenuBuilder: contextMenuBuilder, spellCheckConfiguration: spellCheckConfiguration, magnifierConfiguration: magnifierConfiguration, undoController: undoController, onAppPrivateCommand: onAppPrivateCommand, cursorOpacityAnimates: cursorOpacityAnimates, selectionHeightStyle: selectionHeightStyle ?? EditableText.defaultSelectionHeightStyle, selectionWidthStyle: selectionWidthStyle ?? EditableText.defaultSelectionWidthStyle, dragStartBehavior: dragStartBehavior, contentInsertionConfiguration: contentInsertionConfiguration, clipBehavior: clipBehavior, scribbleEnabled: scribbleEnabled, stylusHandwritingEnabled: DartRuntimePrimitives.RequireValue(stylusHandwritingEnabled), canRequestFocus: canRequestFocus, hintLocales: hintLocales));
        throw new InvalidOperationException("Dart closure completed without a value.");
    })
    {
        object __groupId = groupId ?? typeof(global::Doroti.Framework.Widgets.EditableText);
        InputDecoration? __decoration = decoration ?? new InputDecoration();
        global::Doroti.Framework.Painting.EdgeInsets __scrollPadding = scrollPadding ?? EdgeInsets.CreateAll(20.0);
        global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget>? __contextMenuBuilder = contextMenuBuilder ?? _defaultContextMenuBuilder;
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

    internal static global::Doroti.Framework.Widgets.Widget _defaultContextMenuBuilder(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.EditableTextState editableTextState)
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

internal class _TextFormFieldState__text_form_field : global::Doroti.Framework.Widgets.FormFieldState<string>
{
    internal virtual global::Doroti.Framework.Widgets.RestorableTextEditingController? _controller { get; set; } = default;
    internal virtual string? _initialValue { get; private set; }

    internal virtual global::Doroti.Framework.Widgets.TextEditingController _effectiveController => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.TextEditingController>(_textFormField.controller ?? _controller!.value);
    internal virtual TextFormField _textFormField => ((TextFormField?)base.widget)!;
    public override void restoreState(global::Doroti.Framework.Services.RestorationBucket? oldBucket, bool initialRestore)
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

    internal virtual void _createLocalController(global::Doroti.Framework.Services.TextEditingValue? value = null)
    {
        DartRuntimePrimitives.Assert(() => _controller is null);
        _controller = (value is null) ? RestorableTextEditingController.Create() : new global::Doroti.Framework.Widgets.RestorableTextEditingController(value);
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
            _createLocalController((widget.initialValue is not null) ? new global::Doroti.Framework.Services.TextEditingValue(text: widget.initialValue!) : null);
        }
        else
        {
            _textFormField.controller!.addListener(_handleControllerChanged);
        }
        _initialValue = _textFormField.initialValue ?? _textFormField.controller?.text;
    }

    public override void didUpdateWidget(global::Doroti.Framework.Widgets.FormField<string> oldWidget)
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
            _effectiveController.value = new global::Doroti.Framework.Services.TextEditingValue(text: value ?? "");
        }
    }

    public override void reset()
    {
        _effectiveController.value = new global::Doroti.Framework.Services.TextEditingValue(text: _initialValue ?? "");
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
