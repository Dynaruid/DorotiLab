// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/cupertino/text_form_field_row.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Cupertino;

public class CupertinoTextFormFieldRow : global::Doroti.Generated.Framework.Widgets.FormField<string>
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? prefix { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.TextEditingController? controller { get; private set; }
    public virtual global::System.Action<string>? onChanged { get; private set; }

    public CupertinoTextFormFieldRow(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget? prefix = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Generated.Framework.Widgets.TextEditingController? controller = null, string? initialValue = null, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, global::Doroti.Generated.Framework.Painting.BoxDecoration? decoration = null, global::Doroti.Generated.Framework.Services.TextInputType? keyboardType = null, global::Doroti.Generated.Framework.Services.TextCapitalization textCapitalization = global::Doroti.Generated.Framework.Services.TextCapitalization.none, global::Doroti.Generated.Framework.Services.TextInputAction? textInputAction = null, global::Doroti.Generated.Framework.Painting.TextStyle? style = null, global::Doroti.Generated.Framework.Painting.StrutStyle? strutStyle = null, TextDirection? textDirection = null, TextAlign textAlign = TextAlign.start, global::Doroti.Generated.Framework.Painting.TextAlignVertical? textAlignVertical = null, bool autofocus = false, bool readOnly = false, global::Doroti.Generated.Framework.Widgets.ToolbarOptions? toolbarOptions = null, bool? showCursor = null, string obscuringCharacter = "•", bool obscureText = false, bool autocorrect = true, global::Doroti.Generated.Framework.Services.SmartDashesType? smartDashesType = null, global::Doroti.Generated.Framework.Services.SmartQuotesType? smartQuotesType = null, bool enableSuggestions = true, long? maxLines = 1, long? minLines = null, bool expands = false, long? maxLength = null, global::System.Action<string>? onChanged = null, global::System.Action? onTap = null, global::System.Action? onEditingComplete = null, global::System.Action<string>? onFieldSubmitted = null, global::System.Action<string?>? onSaved = null, global::System.Func<string?, string?>? validator = null, List<global::Doroti.Generated.Framework.Services.TextInputFormatter>? inputFormatters = null, bool? enabled = null, double cursorWidth = 2.0, double? cursorHeight = null, Color? cursorColor = null, Brightness? keyboardAppearance = null, global::Doroti.Generated.Framework.Painting.EdgeInsets scrollPadding = default!, bool enableInteractiveSelection = true, global::Doroti.Generated.Framework.Widgets.TextSelectionControls? selectionControls = null, global::Doroti.Generated.Framework.Widgets.ScrollPhysics? scrollPhysics = null, IEnumerable<string>? autofillHints = null, global::Doroti.Generated.Framework.Widgets.AutovalidateMode autovalidateMode = global::Doroti.Generated.Framework.Widgets.AutovalidateMode.disabled, string? placeholder = null, global::Doroti.Generated.Framework.Painting.TextStyle? placeholderStyle = default!, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.EditableTextState, global::Doroti.Generated.Framework.Widgets.Widget>? contextMenuBuilder = default!, global::Doroti.Generated.Framework.Widgets.SpellCheckConfiguration? spellCheckConfiguration = null, BoxHeightStyle? selectionHeightStyle = null, BoxWidthStyle? selectionWidthStyle = null, string? restorationId = null) : base(key: key, onSaved: onSaved, validator: validator, autovalidateMode: autovalidateMode, restorationId: restorationId, initialValue: ((controller?.text ?? initialValue) ?? ""), builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.FormFieldState<string>, global::Doroti.Generated.Framework.Widgets.Widget>)((field) => {
var state__7266 = ((_CupertinoTextFormFieldRowState__text_form_field_row?)(object?)field)!;
void onChangedHandler(string value)
{
    ((_CupertinoTextFormFieldRowState__text_form_field_row)field).didChange(value);
    onChanged?.Invoke(value);
}
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new CupertinoFormRow(prefix: prefix, padding: padding, error: (((((_CupertinoTextFormFieldRowState__text_form_field_row)field).errorText is null)) ? null : new global::Doroti.Generated.Framework.Widgets.Text(((_CupertinoTextFormFieldRowState__text_form_field_row)field).errorText!)), child: new global::Doroti.Generated.Framework.Widgets.UnmanagedRestorationScope(bucket: ((_CupertinoTextFormFieldRowState__text_form_field_row)field).bucket, child: CupertinoTextField.CreateBorderless(restorationId: restorationId, controller: ((_CupertinoTextFormFieldRowState__text_form_field_row)state__7266)._effectiveController, focusNode: focusNode, keyboardType: keyboardType, decoration: decoration, textInputAction: textInputAction, style: style, strutStyle: strutStyle, textAlign: textAlign, textAlignVertical: textAlignVertical, textCapitalization: textCapitalization, textDirection: textDirection, autofocus: autofocus, toolbarOptions: toolbarOptions, readOnly: readOnly, showCursor: showCursor, obscuringCharacter: obscuringCharacter, obscureText: obscureText, autocorrect: autocorrect, smartDashesType: smartDashesType, smartQuotesType: smartQuotesType, enableSuggestions: enableSuggestions, maxLines: maxLines, minLines: minLines, expands: expands, maxLength: maxLength, onChanged: onChangedHandler, onTap: onTap, onEditingComplete: onEditingComplete, onSubmitted: onFieldSubmitted, inputFormatters: inputFormatters, enabled: (enabled ?? true), cursorWidth: cursorWidth, cursorHeight: cursorHeight, cursorColor: cursorColor, scrollPadding: scrollPadding ?? global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateAll(20.0), scrollPhysics: scrollPhysics, keyboardAppearance: keyboardAppearance, enableInteractiveSelection: enableInteractiveSelection, selectionControls: selectionControls, autofillHints: autofillHints, placeholder: placeholder, placeholderStyle: placeholderStyle ?? new global::Doroti.Generated.Framework.Painting.TextStyle(fontWeight: FontWeight.w400, color: CupertinoColors.placeholderText), contextMenuBuilder: contextMenuBuilder ?? _defaultContextMenuBuilder, spellCheckConfiguration: spellCheckConfiguration, selectionHeightStyle: ((selectionHeightStyle ?? (BoxHeightStyle)global::Doroti.Generated.Framework.Widgets.EditableText.defaultSelectionHeightStyle)), selectionWidthStyle: ((selectionWidthStyle ?? (BoxWidthStyle)global::Doroti.Generated.Framework.Widgets.EditableText.defaultSelectionWidthStyle))))));
throw new InvalidOperationException("Dart closure completed without a value.");
})))
    {
        global::Doroti.Generated.Framework.Painting.EdgeInsets __scrollPadding = scrollPadding ?? global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateAll(20.0);
        global::Doroti.Generated.Framework.Painting.TextStyle? __placeholderStyle = placeholderStyle ?? new global::Doroti.Generated.Framework.Painting.TextStyle(fontWeight: FontWeight.w400, color: CupertinoColors.placeholderText);
        global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.EditableTextState, global::Doroti.Generated.Framework.Widgets.Widget>? __contextMenuBuilder = contextMenuBuilder ?? _defaultContextMenuBuilder;
        this.prefix = prefix;
        this.padding = padding;
        this.controller = controller;
        this.onChanged = onChanged;
        System.Diagnostics.Debug.Assert(((initialValue is null) || (controller is null)));
        System.Diagnostics.Debug.Assert((obscuringCharacter.Length == 1L));
        System.Diagnostics.Debug.Assert(((maxLines is null) || (DartRuntimePrimitives.RequireValue(maxLines) > 0L)));
        System.Diagnostics.Debug.Assert(((minLines is null) || (DartRuntimePrimitives.RequireValue(minLines) > 0L)));
        System.Diagnostics.Debug.Assert(((((maxLines is null)) || ((minLines is null))) || ((maxLines >= DartRuntimePrimitives.RequireValue(minLines)))));
        System.Diagnostics.Debug.Assert((!expands || (((maxLines is null) && (minLines is null)))));
        System.Diagnostics.Debug.Assert((!obscureText || (maxLines == 1L)));
        System.Diagnostics.Debug.Assert(((maxLength is null) || (DartRuntimePrimitives.RequireValue(maxLength) > 0L)));
    }

    internal static global::Doroti.Generated.Framework.Widgets.Widget _defaultContextMenuBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.EditableTextState editableTextState)
    {
        if (SystemContextMenu.isSupportedByField(editableTextState))
        {
            return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)global::Doroti.Generated.Framework.Widgets.SystemContextMenu.CreateEditableText(editableTextState: editableTextState));
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)CupertinoAdaptiveTextSelectionToolbar.CreateEditableText(editableTextState: editableTextState));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.FormFieldState<string> createState() => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.FormFieldState<string>>(new _CupertinoTextFormFieldRowState__text_form_field_row());
}

internal class _CupertinoTextFormFieldRowState__text_form_field_row : global::Doroti.Generated.Framework.Widgets.FormFieldState<string>
{
    internal virtual global::Doroti.Generated.Framework.Widgets.RestorableTextEditingController? _controller { get; set; } = default;

    internal virtual global::Doroti.Generated.Framework.Widgets.TextEditingController _effectiveController => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.TextEditingController>((((CupertinoTextFormFieldRow)this._cupertinoTextFormFieldRow).controller ?? this._controller!.value));
    internal virtual CupertinoTextFormFieldRow _cupertinoTextFormFieldRow => ((CupertinoTextFormFieldRow?)(object?)base.widget)!;
    public override void restoreState(global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket, bool initialRestore)
    {
        base.restoreState(oldBucket, initialRestore);
        if ((this._controller is not null))
        {
            _registerController();
        }
        setValue(((global::Doroti.Generated.Framework.Widgets.TextEditingController)this._effectiveController).text);
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

    public override void initState()
    {
        base.initState();
        if ((((CupertinoTextFormFieldRow)this._cupertinoTextFormFieldRow).controller is null))
        {
            _createLocalController(((this.widget.initialValue is not null) ? new global::Doroti.Generated.Framework.Services.TextEditingValue(text: this.widget.initialValue!) : null));
        }
        else
        {
            ((CupertinoTextFormFieldRow)this._cupertinoTextFormFieldRow).controller!.addListener(() => this._handleControllerChanged());
        }
    }

    public override void didUpdateWidget(global::Doroti.Generated.Framework.Widgets.FormField<string> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((CupertinoTextFormFieldRow)this._cupertinoTextFormFieldRow).controller, ((CupertinoTextFormFieldRow)oldWidget).controller)))
        {
            ((CupertinoTextFormFieldRow)oldWidget).controller?.removeListener(() => this._handleControllerChanged());
            ((CupertinoTextFormFieldRow)this._cupertinoTextFormFieldRow).controller?.addListener(() => this._handleControllerChanged());
            if (((((CupertinoTextFormFieldRow)oldWidget).controller is not null) && (((CupertinoTextFormFieldRow)this._cupertinoTextFormFieldRow).controller is null)))
            {
                _createLocalController(((CupertinoTextFormFieldRow)oldWidget).controller!.value);
            }
            if ((((CupertinoTextFormFieldRow)this._cupertinoTextFormFieldRow).controller is not null))
            {
                setValue(((CupertinoTextFormFieldRow)this._cupertinoTextFormFieldRow).controller!.text);
                if ((((CupertinoTextFormFieldRow)oldWidget).controller is null))
                {
                    unregisterFromRestoration(this._controller!);
                    this._controller!.dispose();
                    _controller = null;
                }
            }
        }
    }

    public override void dispose()
    {
        ((CupertinoTextFormFieldRow)this._cupertinoTextFormFieldRow).controller?.removeListener(() => this._handleControllerChanged());
        this._controller?.dispose();
        base.dispose();
    }

    public override void didChange(string? value)
    {
        base.didChange(value);
        if (((value is not null) && (((global::Doroti.Generated.Framework.Widgets.TextEditingController)this._effectiveController).text != value)))
        {
            this._effectiveController.value = new global::Doroti.Generated.Framework.Services.TextEditingValue(text: value);
        }
    }

    public override void reset()
    {
        this._effectiveController.value = new global::Doroti.Generated.Framework.Services.TextEditingValue(text: (this.widget.initialValue ?? ""));
        base.reset();
        ((CupertinoTextFormFieldRow)this._cupertinoTextFormFieldRow).onChanged?.Invoke(((global::Doroti.Generated.Framework.Widgets.TextEditingController)this._effectiveController).text);
    }

    internal virtual void _handleControllerChanged()
    {
        if ((((global::Doroti.Generated.Framework.Widgets.TextEditingController)this._effectiveController).text != this.value))
        {
            didChange(((global::Doroti.Generated.Framework.Widgets.TextEditingController)this._effectiveController).text);
        }
    }

}
