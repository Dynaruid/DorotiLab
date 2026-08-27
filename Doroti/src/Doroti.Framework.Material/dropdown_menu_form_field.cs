// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/dropdown_menu_form_field.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Material;

public class DropdownMenuFormField<T> : global::Doroti.Framework.Widgets.FormField<T>
{
    public virtual global::System.Action<T?>? onSelected { get; private set; }
    public virtual global::Doroti.Framework.Widgets.TextEditingController? controller { get; private set; }
    public virtual List<DropdownMenuEntry<T>> dropdownMenuEntries { get; private set; } = default!;

    public DropdownMenuFormField(global::Doroti.Framework.Foundation.Key? key = null, bool enabled = true, double? width = null, double? menuHeight = null, global::Doroti.Framework.Widgets.Widget? leadingIcon = null, global::Doroti.Framework.Widgets.Widget? trailingIcon = null, bool showTrailingIcon = true, global::Doroti.Framework.Widgets.FocusNode? trailingIconFocusNode = null, global::Doroti.Framework.Widgets.Widget? label = null, string? hintText = null, string? helperText = null, global::Doroti.Framework.Widgets.Widget? selectedTrailingIcon = null, bool enableFilter = false, bool enableSearch = true, global::Doroti.Framework.Services.TextInputType? keyboardType = null, global::Doroti.Framework.Painting.TextStyle? textStyle = null, TextAlign textAlign = TextAlign.start, object? inputDecorationTheme = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.MenuController, InputDecoration>? decorationBuilder = null, MenuStyle? menuStyle = null, global::Doroti.Framework.Widgets.TextEditingController? controller = null, T? initialSelection = default, global::System.Action<T?>? onSelected = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool? requestFocusOnTap = null, bool selectOnly = false, global::Doroti.Framework.Painting.EdgeInsetsGeometry? expandedInsets = null, Offset? alignmentOffset = null, global::System.Func<List<DropdownMenuEntry<T>>, string, List<DropdownMenuEntry<T>>>? filterCallback = null, global::System.Func<List<DropdownMenuEntry<T>>, string, long?>? searchCallback = null, List<DropdownMenuEntry<T>> dropdownMenuEntries = default!, List<global::Doroti.Framework.Services.TextInputFormatter>? inputFormatters = null, DropdownMenuCloseBehavior closeBehavior = DropdownMenuCloseBehavior.all, long maxLines = 1, global::Doroti.Framework.Services.TextInputAction? textInputAction = null, double? cursorHeight = null, global::Doroti.Framework.Widgets.MenuController? menuController = null, string? restorationId = null, global::System.Action<T?>? onSaved = null, global::Doroti.Framework.Widgets.AutovalidateMode autovalidateMode = global::Doroti.Framework.Widgets.AutovalidateMode.disabled, global::System.Func<T?, string?>? validator = null, string? forceErrorText = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, string, global::Doroti.Framework.Widgets.Widget>? errorBuilder = null) : base(key: key, restorationId: restorationId, onSaved: onSaved, validator: validator, forceErrorText: forceErrorText, errorBuilder: errorBuilder, initialValue: initialSelection, autovalidateMode: autovalidateMode, builder: ((global::System.Func<global::Doroti.Framework.Widgets.FormFieldState<T>, global::Doroti.Framework.Widgets.Widget>)((field) =>
    {
        var state = ((_DropdownMenuFormFieldState__dropdown_menu_form_field<T>?)(object?)field)!;
        InputDecoration effectiveDecorationBuilder(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.MenuController menuController)
        {
            InputDecoration decoration = ((decorationBuilder is null ? new InputDecoration() : decorationBuilder.Invoke(context, menuController)));
            InputDecoration decorationWithLabels = ((InputDecoration)(object?)decoration.copyWith(label: label, hintText: hintText, helperText: helperText));
            string? errorTextLocal = state.errorText;
            if ((errorTextLocal is null))
            {
                return decorationWithLabels;
            }
            return ((errorBuilder is not null) ? decorationWithLabels.copyWith(error: errorBuilder(state.context, errorTextLocal)) : decorationWithLabels.copyWith(errorText: errorTextLocal));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.UnmanagedRestorationScope(bucket: ((_DropdownMenuFormFieldState__dropdown_menu_form_field<T>)field).bucket, child: new DropdownMenu<T>(restorationId: restorationId, enabled: enabled, width: width, menuHeight: menuHeight, leadingIcon: leadingIcon, trailingIcon: trailingIcon, showTrailingIcon: showTrailingIcon, trailingIconFocusNode: trailingIconFocusNode, selectedTrailingIcon: selectedTrailingIcon, enableFilter: enableFilter, enableSearch: enableSearch, keyboardType: keyboardType, textStyle: textStyle, textAlign: textAlign, inputDecorationTheme: inputDecorationTheme, decorationBuilder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.MenuController, InputDecoration>)effectiveDecorationBuilder, menuStyle: menuStyle, controller: ((_DropdownMenuFormFieldState__dropdown_menu_form_field<T>)state).textFieldController, initialSelection: state.value, onSelected: (global::System.Action<T?>)((_DropdownMenuFormFieldState__dropdown_menu_form_field<T>)((_DropdownMenuFormFieldState__dropdown_menu_form_field<T>)field)).didChange, focusNode: focusNode, requestFocusOnTap: requestFocusOnTap, selectOnly: selectOnly, expandedInsets: expandedInsets, alignmentOffset: alignmentOffset, filterCallback: (global::System.Func<List<DropdownMenuEntry<T>>, string, List<DropdownMenuEntry<T>>>?)filterCallback, searchCallback: (global::System.Func<List<DropdownMenuEntry<T>>, string, long?>?)searchCallback, inputFormatters: inputFormatters, closeBehavior: closeBehavior, dropdownMenuEntries: dropdownMenuEntries, maxLines: maxLines, textInputAction: textInputAction, cursorHeight: cursorHeight, menuController: menuController)));
        throw new InvalidOperationException("Dart closure completed without a value.");
    })))
    {
        this.controller = controller;
        this.onSelected = onSelected;
        this.dropdownMenuEntries = dropdownMenuEntries;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DropdownMenuFormFieldState__dropdown_menu_form_field<T>());
}

internal class _DropdownMenuFormFieldState__dropdown_menu_form_field<T> : global::Doroti.Framework.Widgets.FormFieldState<T>
{
    internal virtual global::Doroti.Framework.Widgets.RestorableTextEditingController? _restorableController { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.TextEditingController? _localTextFieldController { get; set; } = default;

    internal virtual DropdownMenuFormField<T> _dropdownMenuFormField => ((DropdownMenuFormField<T>?)(object?)this.widget)!;
    public virtual global::Doroti.Framework.Widgets.TextEditingController textFieldController => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.TextEditingController>((((DropdownMenuFormField<T>)this._dropdownMenuFormField).controller ?? (_localTextFieldController ??= new global::Doroti.Framework.Widgets.TextEditingController())));
    public override void initState()
    {
        base.initState();
        _createRestorableController(((FormField<T>)(object)this.widget).initialValue);
    }

    internal virtual void _createRestorableController(T? initialValue)
    {
        DartRuntimePrimitives.Assert(() => (this._restorableController is null));
        _restorableController = new global::Doroti.Framework.Widgets.RestorableTextEditingController(new global::Doroti.Framework.Services.TextEditingValue(text: _findLabelByValue(initialValue)));
        if (!this.restorePending)
        {
            _registerRestorableController();
        }
    }

    public override void didUpdateWidget(global::Doroti.Framework.Widgets.FormField<T> oldWidget)
    {
        var __oldWidget = (DropdownMenuFormField<T>)(object)oldWidget;
        base.didUpdateWidget(__oldWidget);
        if ((!EqualityComparer<T>.Default.Equals(__oldWidget.initialValue, ((FormField<T>)(object)this.widget).initialValue) && !this.hasInteractedByUser))
        {
            setValue(((FormField<T>)(object)this.widget).initialValue);
        }
        if ((!object.Equals(((DropdownMenuFormField<T>)__oldWidget).controller, ((DropdownMenuFormField<T>)this._dropdownMenuFormField).controller)))
        {
            this._localTextFieldController?.dispose();
            _localTextFieldController = null;
        }
    }

    public override void dispose()
    {
        this._restorableController?.dispose();
        this._localTextFieldController?.dispose();
        base.dispose();
    }

    public override void didChange(T? value)
    {
        base.didChange(value);
        ((DropdownMenuFormField<T>)this._dropdownMenuFormField).onSelected?.Invoke(value);
        _updateRestorableController(value);
    }

    public override void reset()
    {
        base.reset();
        ((DropdownMenuFormField<T>)this._dropdownMenuFormField).onSelected?.Invoke(this.value);
        _updateRestorableController(((FormField<T>)(object)this.widget).initialValue);
        if ((((FormField<T>)(object)this.widget).initialValue is null))
        {
            this.textFieldController.clear();
        }
    }

    internal virtual void _updateRestorableController(T? value)
    {
        if ((this._restorableController is not null))
        {
            this._restorableController!.value.value = new global::Doroti.Framework.Services.TextEditingValue(text: _findLabelByValue(value));
        }
    }

    public override void restoreState(global::Doroti.Framework.Services.RestorationBucket? oldBucket, bool initialRestore)
    {
        base.restoreState(oldBucket, initialRestore);
        if ((this._restorableController is not null))
        {
            _registerRestorableController();
            T? matchingValue = ((T?)(object?)_findValueByLabel(this._restorableController!.value.text));
            if ((matchingValue is not null))
            {
                setValue(matchingValue);
            }
        }
    }

    internal virtual void _registerRestorableController()
    {
        DartRuntimePrimitives.Assert(() => (this._restorableController is not null));
        registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this._restorableController!), "controller");
    }

    internal virtual T? _findValueByLabel(string label)
    {
        foreach (DropdownMenuEntry<T> entry in ((DropdownMenuFormField<T>)this._dropdownMenuFormField).dropdownMenuEntries)
        {
            if ((((DropdownMenuEntry<T>)entry).label == label))
            {
                return ((DropdownMenuEntry<T>)entry).value;
            }
        }
        return default;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual string _findLabelByValue(T? value)
    {
        foreach (DropdownMenuEntry<T> entry in ((DropdownMenuFormField<T>)this._dropdownMenuFormField).dropdownMenuEntries)
        {
            if (EqualityComparer<T>.Default.Equals(((DropdownMenuEntry<T>)entry).value, value))
            {
                return ((DropdownMenuEntry<T>)entry).label;
            }
        }
        return "";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
