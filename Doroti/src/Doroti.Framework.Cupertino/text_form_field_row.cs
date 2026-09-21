// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/text_form_field_row.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public class CupertinoTextFormFieldRow : FormField<string>
{
    public virtual Widget? prefix { get; private set; }
    public virtual EdgeInsetsGeometry? padding { get; private set; }
    public virtual TextEditingController? controller { get; private set; }
    public virtual Action<string>? onChanged { get; private set; }

    public CupertinoTextFormFieldRow(
        Key? key = null,
        Widget? prefix = null,
        EdgeInsetsGeometry? padding = null,
        TextEditingController? controller = null,
        string? initialValue = null,
        FocusNode? focusNode = null,
        BoxDecoration? decoration = null,
        TextInputType? keyboardType = null,
        TextCapitalization textCapitalization = TextCapitalization.none,
        TextInputAction? textInputAction = null,
        TextStyle? style = null,
        Painting.StrutStyle? strutStyle = null,
        TextDirection? textDirection = null,
        TextAlign textAlign = TextAlign.start,
        TextAlignVertical? textAlignVertical = null,
        bool autofocus = false,
        bool readOnly = false,
        ToolbarOptions? toolbarOptions = null,
        bool? showCursor = null,
        string obscuringCharacter = "•",
        bool obscureText = false,
        bool autocorrect = true,
        SmartDashesType? smartDashesType = null,
        SmartQuotesType? smartQuotesType = null,
        bool enableSuggestions = true,
        long? maxLines = 1,
        long? minLines = null,
        bool expands = false,
        long? maxLength = null,
        Action<string>? onChanged = null,
        Action? onTap = null,
        Action? onEditingComplete = null,
        Action<string>? onFieldSubmitted = null,
        Action<string?>? onSaved = null,
        Func<string?, string?>? validator = null,
        List<TextInputFormatter>? inputFormatters = null,
        bool? enabled = null,
        double cursorWidth = 2.0,
        double? cursorHeight = null,
        Color? cursorColor = null,
        Brightness? keyboardAppearance = null,
        EdgeInsets scrollPadding = default!,
        bool enableInteractiveSelection = true,
        TextSelectionControls? selectionControls = null,
        ScrollPhysics? scrollPhysics = null,
        IEnumerable<string>? autofillHints = null,
        AutovalidateMode autovalidateMode = AutovalidateMode.disabled,
        string? placeholder = null,
        TextStyle? placeholderStyle = default!,
        Func<BuildContext, EditableTextState, Widget>? contextMenuBuilder = default!,
        SpellCheckConfiguration? spellCheckConfiguration = null,
        BoxHeightStyle? selectionHeightStyle = null,
        BoxWidthStyle? selectionWidthStyle = null,
        string? restorationId = null
    )
        : base(
            key: key,
            onSaved: onSaved,
            validator: validator,
            autovalidateMode: autovalidateMode,
            restorationId: restorationId,
            initialValue: (controller?.text ?? initialValue) ?? "",
            builder: (field) =>
            {
                var state = ((_CupertinoTextFormFieldRowState__text_form_field_row?)field)!;
                void onChangedHandler(string value)
                {
                    ((_CupertinoTextFormFieldRowState__text_form_field_row)field).didChange(value);
                    onChanged?.Invoke(value);
                }
                return new CupertinoFormRow(
                    prefix: prefix,
                    padding: padding,
                    error: (
                        ((_CupertinoTextFormFieldRowState__text_form_field_row)field).errorText
                        is null
                    )
                        ? null
                        : new Text(
                            ((_CupertinoTextFormFieldRowState__text_form_field_row)field).errorText!
                        ),
                    child: new UnmanagedRestorationScope(
                        bucket: (
                            (_CupertinoTextFormFieldRowState__text_form_field_row)field
                        ).bucket,
                        child: CupertinoTextField.CreateBorderless(
                            restorationId: restorationId,
                            controller: state._effectiveController,
                            focusNode: focusNode,
                            keyboardType: keyboardType,
                            decoration: decoration,
                            textInputAction: textInputAction,
                            style: style,
                            strutStyle: strutStyle,
                            textAlign: textAlign,
                            textAlignVertical: textAlignVertical,
                            textCapitalization: textCapitalization,
                            textDirection: textDirection,
                            autofocus: autofocus,
                            toolbarOptions: toolbarOptions,
                            readOnly: readOnly,
                            showCursor: showCursor,
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
                            onChanged: onChangedHandler,
                            onTap: onTap,
                            onEditingComplete: onEditingComplete,
                            onSubmitted: onFieldSubmitted,
                            inputFormatters: inputFormatters,
                            enabled: enabled ?? true,
                            cursorWidth: cursorWidth,
                            cursorHeight: cursorHeight,
                            cursorColor: cursorColor,
                            scrollPadding: scrollPadding ?? EdgeInsets.CreateAll(20.0),
                            scrollPhysics: scrollPhysics,
                            keyboardAppearance: keyboardAppearance,
                            enableInteractiveSelection: enableInteractiveSelection,
                            selectionControls: selectionControls,
                            autofillHints: autofillHints,
                            placeholder: placeholder,
                            placeholderStyle: placeholderStyle
                                ?? new TextStyle(
                                    fontWeight: FontWeight.w400,
                                    color: CupertinoColors.placeholderText
                                ),
                            contextMenuBuilder: contextMenuBuilder ?? _defaultContextMenuBuilder,
                            spellCheckConfiguration: spellCheckConfiguration,
                            selectionHeightStyle: selectionHeightStyle
                                ?? EditableText.defaultSelectionHeightStyle,
                            selectionWidthStyle: selectionWidthStyle
                                ?? EditableText.defaultSelectionWidthStyle
                        )
                    )
                );
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        )
    {
        EdgeInsets __scrollPadding = scrollPadding ?? EdgeInsets.CreateAll(20.0);
        TextStyle? __placeholderStyle =
            placeholderStyle
            ?? new TextStyle(fontWeight: FontWeight.w400, color: CupertinoColors.placeholderText);
        Func<BuildContext, EditableTextState, Widget>? __contextMenuBuilder =
            contextMenuBuilder ?? _defaultContextMenuBuilder;
        this.prefix = prefix;
        this.padding = padding;
        this.controller = controller;
        this.onChanged = onChanged;
        System.Diagnostics.Debug.Assert((initialValue is null) || (controller is null));
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
                    ) > 0L
                )
        );
    }

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
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override FormFieldState<string> createState() =>
        DartRuntimePrimitives.ConvertValue<FormFieldState<string>>(
            new _CupertinoTextFormFieldRowState__text_form_field_row()
        );
}

internal class _CupertinoTextFormFieldRowState__text_form_field_row : FormFieldState<string>
{
    internal virtual RestorableTextEditingController? _controller { get; set; } = default;

    internal virtual TextEditingController _effectiveController =>
        DartRuntimePrimitives.ConvertValue<TextEditingController>(
            _cupertinoTextFormFieldRow.controller ?? _controller!.value
        );
    internal virtual CupertinoTextFormFieldRow _cupertinoTextFormFieldRow =>
        ((CupertinoTextFormFieldRow?)base.widget)!;

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
        _controller =
            (value is null)
                ? RestorableTextEditingController.Create()
                : new RestorableTextEditingController(value);
        if (!restorePending)
        {
            _registerController();
        }
    }

    public override void initState()
    {
        base.initState();
        if (_cupertinoTextFormFieldRow.controller is null)
        {
            _createLocalController(
                (widget.initialValue is not null)
                    ? new TextEditingValue(text: widget.initialValue!)
                    : null
            );
        }
        else
        {
            _cupertinoTextFormFieldRow.controller!.addListener(_handleControllerChanged);
        }
    }

    public override void didUpdateWidget(FormField<string> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (
            !Equals(
                _cupertinoTextFormFieldRow.controller,
                ((CupertinoTextFormFieldRow)oldWidget).controller
            )
        )
        {
            ((CupertinoTextFormFieldRow)oldWidget).controller?.removeListener(
                _handleControllerChanged
            );
            _cupertinoTextFormFieldRow.controller?.addListener(_handleControllerChanged);
            if (
                (((CupertinoTextFormFieldRow)oldWidget).controller is not null)
                && (_cupertinoTextFormFieldRow.controller is null)
            )
            {
                _createLocalController(((CupertinoTextFormFieldRow)oldWidget).controller!.value);
            }
            if (_cupertinoTextFormFieldRow.controller is not null)
            {
                setValue(_cupertinoTextFormFieldRow.controller!.text);
                if (((CupertinoTextFormFieldRow)oldWidget).controller is null)
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
        _cupertinoTextFormFieldRow.controller?.removeListener(_handleControllerChanged);
        _controller?.dispose();
        base.dispose();
    }

    public override void didChange(string? value)
    {
        base.didChange(value);
        if ((value is not null) && (_effectiveController.text != value))
        {
            _effectiveController.value = new TextEditingValue(text: value);
        }
    }

    public override void reset()
    {
        _effectiveController.value = new TextEditingValue(text: widget.initialValue ?? "");
        base.reset();
        _cupertinoTextFormFieldRow.onChanged?.Invoke(_effectiveController.text);
    }

    internal virtual void _handleControllerChanged()
    {
        if (_effectiveController.text != value)
        {
            didChange(_effectiveController.text);
        }
    }
}
