// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/dropdown_menu_form_field.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class DropdownMenuFormField<T> : FormField<T>
{
    public virtual Action<T?>? onSelected { get; private set; }
    public virtual TextEditingController? controller { get; private set; }
    public virtual List<DropdownMenuEntry<T>> dropdownMenuEntries { get; private set; } = default!;

    public DropdownMenuFormField(Key? key = null, bool enabled = true, double? width = null, double? menuHeight = null, Widget? leadingIcon = null, Widget? trailingIcon = null, bool showTrailingIcon = true, FocusNode? trailingIconFocusNode = null, Widget? label = null, string? hintText = null, string? helperText = null, Widget? selectedTrailingIcon = null, bool enableFilter = false, bool enableSearch = true, TextInputType? keyboardType = null, TextStyle? textStyle = null, TextAlign textAlign = TextAlign.start, object? inputDecorationTheme = null, Func<BuildContext, MenuController, InputDecoration>? decorationBuilder = null, MenuStyle? menuStyle = null, TextEditingController? controller = null, T? initialSelection = default, Action<T?>? onSelected = null, FocusNode? focusNode = null, bool? requestFocusOnTap = null, bool selectOnly = false, EdgeInsetsGeometry? expandedInsets = null, Offset? alignmentOffset = null, Func<List<DropdownMenuEntry<T>>, string, List<DropdownMenuEntry<T>>>? filterCallback = null, Func<List<DropdownMenuEntry<T>>, string, long?>? searchCallback = null, List<DropdownMenuEntry<T>> dropdownMenuEntries = default!, List<TextInputFormatter>? inputFormatters = null, DropdownMenuCloseBehavior closeBehavior = DropdownMenuCloseBehavior.all, long maxLines = 1, TextInputAction? textInputAction = null, double? cursorHeight = null, MenuController? menuController = null, string? restorationId = null, Action<T?>? onSaved = null, AutovalidateMode autovalidateMode = AutovalidateMode.disabled, Func<T?, string?>? validator = null, string? forceErrorText = null, Func<BuildContext, string, Widget>? errorBuilder = null) : base(key: key, restorationId: restorationId, onSaved: onSaved, validator: validator, forceErrorText: forceErrorText, errorBuilder: errorBuilder, initialValue: initialSelection, autovalidateMode: autovalidateMode, builder: (field) =>
    {
        var state = ((_DropdownMenuFormFieldState__dropdown_menu_form_field<T>?)field)!;
        InputDecoration effectiveDecorationBuilder(BuildContext context, MenuController menuController)
        {
            InputDecoration decoration = decorationBuilder is null ? new InputDecoration() : decorationBuilder.Invoke(context, menuController);
            InputDecoration decorationWithLabels = decoration.copyWith(label: label, hintText: hintText, helperText: helperText);
            string? errorTextLocal = state.errorText;
            if (errorTextLocal is null)
            {
                return decorationWithLabels;
            }
            return (errorBuilder is not null) ? decorationWithLabels.copyWith(error: errorBuilder(state.context, errorTextLocal)) : decorationWithLabels.copyWith(errorText: errorTextLocal);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        return new UnmanagedRestorationScope(bucket: ((_DropdownMenuFormFieldState__dropdown_menu_form_field<T>)field).bucket, child: new DropdownMenu<T>(restorationId: restorationId, enabled: enabled, width: width, menuHeight: menuHeight, leadingIcon: leadingIcon, trailingIcon: trailingIcon, showTrailingIcon: showTrailingIcon, trailingIconFocusNode: trailingIconFocusNode, selectedTrailingIcon: selectedTrailingIcon, enableFilter: enableFilter, enableSearch: enableSearch, keyboardType: keyboardType, textStyle: textStyle, textAlign: textAlign, inputDecorationTheme: inputDecorationTheme, decorationBuilder: effectiveDecorationBuilder, menuStyle: menuStyle, controller: state.textFieldController, initialSelection: state.value, onSelected: ((_DropdownMenuFormFieldState__dropdown_menu_form_field<T>)field).didChange, focusNode: focusNode, requestFocusOnTap: requestFocusOnTap, selectOnly: selectOnly, expandedInsets: expandedInsets, alignmentOffset: alignmentOffset, filterCallback: filterCallback, searchCallback: searchCallback, inputFormatters: inputFormatters, closeBehavior: closeBehavior, dropdownMenuEntries: dropdownMenuEntries, maxLines: maxLines, textInputAction: textInputAction, cursorHeight: cursorHeight, menuController: menuController));
        throw new InvalidOperationException("Dart closure completed without a value.");
    })
    {
        this.controller = controller;
        this.onSelected = onSelected;
        this.dropdownMenuEntries = dropdownMenuEntries;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DropdownMenuFormFieldState__dropdown_menu_form_field<T>());
}

internal class _DropdownMenuFormFieldState__dropdown_menu_form_field<T> : FormFieldState<T>
{
    internal virtual RestorableTextEditingController? _restorableController { get; set; } = default;
    internal virtual TextEditingController? _localTextFieldController { get; set; } = default;

    internal virtual DropdownMenuFormField<T> _dropdownMenuFormField => ((DropdownMenuFormField<T>?)widget)!;
    public virtual TextEditingController textFieldController => DartRuntimePrimitives.ConvertValue<TextEditingController>(_dropdownMenuFormField.controller ?? (_localTextFieldController ??= new TextEditingController()));
    public override void initState()
    {
        base.initState();
        _createRestorableController(widget.initialValue);
    }

    internal virtual void _createRestorableController(T? initialValue)
    {
        DartRuntimePrimitives.Assert(() => _restorableController is null);
        _restorableController = new RestorableTextEditingController(new TextEditingValue(text: _findLabelByValue(initialValue)));
        if (!restorePending)
        {
            _registerRestorableController();
        }
    }

    public override void didUpdateWidget(FormField<T> oldWidget)
    {
        var __oldWidget = (DropdownMenuFormField<T>)oldWidget;
        base.didUpdateWidget(__oldWidget);
        if (!EqualityComparer<T>.Default.Equals(__oldWidget.initialValue, widget.initialValue) && !hasInteractedByUser)
        {
            setValue(widget.initialValue);
        }
        if (!Equals(__oldWidget.controller, _dropdownMenuFormField.controller))
        {
            _localTextFieldController?.dispose();
            _localTextFieldController = null;
        }
    }

    public override void dispose()
    {
        _restorableController?.dispose();
        _localTextFieldController?.dispose();
        base.dispose();
    }

    public override void didChange(T? value)
    {
        base.didChange(value);
        _dropdownMenuFormField.onSelected?.Invoke(value);
        _updateRestorableController(value);
    }

    public override void reset()
    {
        base.reset();
        _dropdownMenuFormField.onSelected?.Invoke(value);
        _updateRestorableController(widget.initialValue);
        if (widget.initialValue is null)
        {
            textFieldController.clear();
        }
    }

    internal virtual void _updateRestorableController(T? value)
    {
        if (_restorableController is not null)
        {
            _restorableController!.value.value = new TextEditingValue(text: _findLabelByValue(value));
        }
    }

    public override void restoreState(RestorationBucket? oldBucket, bool initialRestore)
    {
        base.restoreState(oldBucket, initialRestore);
        if (_restorableController is not null)
        {
            _registerRestorableController();
            T? matchingValue = _findValueByLabel(_restorableController!.value.text);
            if (matchingValue is not null)
            {
                setValue(matchingValue);
            }
        }
    }

    internal virtual void _registerRestorableController()
    {
        DartRuntimePrimitives.Assert(() => _restorableController is not null);
        registerForRestoration(_restorableController!, "controller");
    }

    internal virtual T? _findValueByLabel(string label)
    {
        foreach (DropdownMenuEntry<T> entry in _dropdownMenuFormField.dropdownMenuEntries)
        {
            if (entry.label == label)
            {
                return entry.value;
            }
        }
        return default;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual string _findLabelByValue(T? value)
    {
        foreach (DropdownMenuEntry<T> entry in _dropdownMenuFormField.dropdownMenuEntries)
        {
            if (EqualityComparer<T>.Default.Equals(entry.value, value))
            {
                return entry.label;
            }
        }
        return "";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
