// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/text_form_field_row.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public class CupertinoTextFormFieldRow : global::Doroti.Framework.Widgets.FormField<string>
{
    public virtual global::Doroti.Framework.Widgets.Widget? prefix { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual global::Doroti.Framework.Widgets.TextEditingController? controller { get; private set; }
    public virtual global::System.Action<string>? onChanged { get; private set; }

    public CupertinoTextFormFieldRow(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget? prefix = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Widgets.TextEditingController? controller = null, string? initialValue = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::Doroti.Framework.Painting.BoxDecoration? decoration = null, global::Doroti.Framework.Services.TextInputType? keyboardType = null, global::Doroti.Framework.Services.TextCapitalization textCapitalization = TextCapitalization.none, global::Doroti.Framework.Services.TextInputAction? textInputAction = null, global::Doroti.Framework.Painting.TextStyle? style = null, global::Doroti.Framework.Painting.StrutStyle? strutStyle = null, TextDirection? textDirection = null, TextAlign textAlign = TextAlign.start, global::Doroti.Framework.Painting.TextAlignVertical? textAlignVertical = null, bool autofocus = false, bool readOnly = false, global::Doroti.Framework.Widgets.ToolbarOptions? toolbarOptions = null, bool? showCursor = null, string obscuringCharacter = "•", bool obscureText = false, bool autocorrect = true, global::Doroti.Framework.Services.SmartDashesType? smartDashesType = null, global::Doroti.Framework.Services.SmartQuotesType? smartQuotesType = null, bool enableSuggestions = true, long? maxLines = 1, long? minLines = null, bool expands = false, long? maxLength = null, global::System.Action<string>? onChanged = null, global::System.Action? onTap = null, global::System.Action? onEditingComplete = null, global::System.Action<string>? onFieldSubmitted = null, global::System.Action<string?>? onSaved = null, global::System.Func<string?, string?>? validator = null, List<global::Doroti.Framework.Services.TextInputFormatter>? inputFormatters = null, bool? enabled = null, double cursorWidth = 2.0, double? cursorHeight = null, Color? cursorColor = null, Brightness? keyboardAppearance = null, global::Doroti.Framework.Painting.EdgeInsets scrollPadding = default!, bool enableInteractiveSelection = true, global::Doroti.Framework.Widgets.TextSelectionControls? selectionControls = null, global::Doroti.Framework.Widgets.ScrollPhysics? scrollPhysics = null, IEnumerable<string>? autofillHints = null, global::Doroti.Framework.Widgets.AutovalidateMode autovalidateMode = AutovalidateMode.disabled, string? placeholder = null, global::Doroti.Framework.Painting.TextStyle? placeholderStyle = default!, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget>? contextMenuBuilder = default!, global::Doroti.Framework.Widgets.SpellCheckConfiguration? spellCheckConfiguration = null, BoxHeightStyle? selectionHeightStyle = null, BoxWidthStyle? selectionWidthStyle = null, string? restorationId = null) : base(key: key, onSaved: onSaved, validator: validator, autovalidateMode: autovalidateMode, restorationId: restorationId, initialValue: (controller?.text ?? initialValue) ?? "", builder: (field) =>
    {
        var state = ((_CupertinoTextFormFieldRowState__text_form_field_row?)field)!;
        void onChangedHandler(string value)
        {
            ((_CupertinoTextFormFieldRowState__text_form_field_row)field).didChange(value);
            onChanged?.Invoke(value);
        }
        return new CupertinoFormRow(prefix: prefix, padding: padding, error: (((_CupertinoTextFormFieldRowState__text_form_field_row)field).errorText is null) ? null : new global::Doroti.Framework.Widgets.Text(((_CupertinoTextFormFieldRowState__text_form_field_row)field).errorText!), child: new global::Doroti.Framework.Widgets.UnmanagedRestorationScope(bucket: ((_CupertinoTextFormFieldRowState__text_form_field_row)field).bucket, child: CupertinoTextField.CreateBorderless(restorationId: restorationId, controller: state._effectiveController, focusNode: focusNode, keyboardType: keyboardType, decoration: decoration, textInputAction: textInputAction, style: style, strutStyle: strutStyle, textAlign: textAlign, textAlignVertical: textAlignVertical, textCapitalization: textCapitalization, textDirection: textDirection, autofocus: autofocus, toolbarOptions: toolbarOptions, readOnly: readOnly, showCursor: showCursor, obscuringCharacter: obscuringCharacter, obscureText: obscureText, autocorrect: autocorrect, smartDashesType: smartDashesType, smartQuotesType: smartQuotesType, enableSuggestions: enableSuggestions, maxLines: maxLines, minLines: minLines, expands: expands, maxLength: maxLength, onChanged: onChangedHandler, onTap: onTap, onEditingComplete: onEditingComplete, onSubmitted: onFieldSubmitted, inputFormatters: inputFormatters, enabled: enabled ?? true, cursorWidth: cursorWidth, cursorHeight: cursorHeight, cursorColor: cursorColor, scrollPadding: scrollPadding ?? EdgeInsets.CreateAll(20.0), scrollPhysics: scrollPhysics, keyboardAppearance: keyboardAppearance, enableInteractiveSelection: enableInteractiveSelection, selectionControls: selectionControls, autofillHints: autofillHints, placeholder: placeholder, placeholderStyle: placeholderStyle ?? new global::Doroti.Framework.Painting.TextStyle(fontWeight: FontWeight.w400, color: CupertinoColors.placeholderText), contextMenuBuilder: contextMenuBuilder ?? _defaultContextMenuBuilder, spellCheckConfiguration: spellCheckConfiguration, selectionHeightStyle: selectionHeightStyle ?? EditableText.defaultSelectionHeightStyle, selectionWidthStyle: selectionWidthStyle ?? EditableText.defaultSelectionWidthStyle)));
        throw new InvalidOperationException("Dart closure completed without a value.");
    })
    {
        global::Doroti.Framework.Painting.EdgeInsets __scrollPadding = scrollPadding ?? EdgeInsets.CreateAll(20.0);
        global::Doroti.Framework.Painting.TextStyle? __placeholderStyle = placeholderStyle ?? new global::Doroti.Framework.Painting.TextStyle(fontWeight: FontWeight.w400, color: CupertinoColors.placeholderText);
        global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.EditableTextState, global::Doroti.Framework.Widgets.Widget>? __contextMenuBuilder = contextMenuBuilder ?? _defaultContextMenuBuilder;
        this.prefix = prefix;
        this.padding = padding;
        this.controller = controller;
        this.onChanged = onChanged;
        System.Diagnostics.Debug.Assert((initialValue is null) || (controller is null));
        System.Diagnostics.Debug.Assert(obscuringCharacter.Length == 1L);
        System.Diagnostics.Debug.Assert((maxLines is null) || (DartRuntimePrimitives.RequireValue(maxLines) > 0L));
        System.Diagnostics.Debug.Assert((minLines is null) || (DartRuntimePrimitives.RequireValue(minLines) > 0L));
        System.Diagnostics.Debug.Assert(maxLines is null || minLines is null || maxLines >= DartRuntimePrimitives.RequireValue(minLines));
        System.Diagnostics.Debug.Assert(!expands || (maxLines is null) && (minLines is null));
        System.Diagnostics.Debug.Assert(!obscureText || (maxLines == 1L));
        System.Diagnostics.Debug.Assert((maxLength is null) || (DartRuntimePrimitives.RequireValue(maxLength) > 0L));
    }

    internal static global::Doroti.Framework.Widgets.Widget _defaultContextMenuBuilder(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.EditableTextState editableTextState)
    {
        if (SystemContextMenu.isSupportedByField(editableTextState))
        {
            return SystemContextMenu.CreateEditableText(editableTextState: editableTextState);
        }
        return CupertinoAdaptiveTextSelectionToolbar.CreateEditableText(editableTextState: editableTextState);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.FormFieldState<string> createState() => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.FormFieldState<string>>(new _CupertinoTextFormFieldRowState__text_form_field_row());
}

internal class _CupertinoTextFormFieldRowState__text_form_field_row : global::Doroti.Framework.Widgets.FormFieldState<string>
{
    internal virtual global::Doroti.Framework.Widgets.RestorableTextEditingController? _controller { get; set; } = default;

    internal virtual global::Doroti.Framework.Widgets.TextEditingController _effectiveController => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.TextEditingController>(_cupertinoTextFormFieldRow.controller ?? _controller!.value);
    internal virtual CupertinoTextFormFieldRow _cupertinoTextFormFieldRow => ((CupertinoTextFormFieldRow?)base.widget)!;
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
        if (_cupertinoTextFormFieldRow.controller is null)
        {
            _createLocalController((widget.initialValue is not null) ? new global::Doroti.Framework.Services.TextEditingValue(text: widget.initialValue!) : null);
        }
        else
        {
            _cupertinoTextFormFieldRow.controller!.addListener(_handleControllerChanged);
        }
    }

    public override void didUpdateWidget(global::Doroti.Framework.Widgets.FormField<string> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(_cupertinoTextFormFieldRow.controller, ((CupertinoTextFormFieldRow)oldWidget).controller))
        {
            ((CupertinoTextFormFieldRow)oldWidget).controller?.removeListener(_handleControllerChanged);
            _cupertinoTextFormFieldRow.controller?.addListener(_handleControllerChanged);
            if ((((CupertinoTextFormFieldRow)oldWidget).controller is not null) && (_cupertinoTextFormFieldRow.controller is null))
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
            _effectiveController.value = new global::Doroti.Framework.Services.TextEditingValue(text: value);
        }
    }

    public override void reset()
    {
        _effectiveController.value = new global::Doroti.Framework.Services.TextEditingValue(text: widget.initialValue ?? "");
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
