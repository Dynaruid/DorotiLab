// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/input_date_picker_form_field.dart
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

public class InputDatePickerFormField : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual DateTime? initialDate { get; private set; }
    public virtual DateTime firstDate { get; private set; } = default!;
    public virtual DateTime lastDate { get; private set; } = default!;
    public virtual global::System.Action<DateTime>? onDateSubmitted { get; private set; }
    public virtual global::System.Action<DateTime>? onDateSaved { get; private set; }
    public virtual global::System.Func<DateTime, bool>? selectableDayPredicate { get; private set; }
    public virtual string? errorFormatText { get; private set; }
    public virtual string? errorInvalidText { get; private set; }
    public virtual string? fieldHintText { get; private set; }
    public virtual string? fieldLabelText { get; private set; }
    public virtual global::Doroti.Framework.Services.TextInputType? keyboardType { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual bool acceptEmptyDate { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    public InputDatePickerFormField(global::Doroti.Framework.Foundation.Key? key = null, DateTime? initialDate = null, DateTime firstDate = default!, DateTime lastDate = default!, global::System.Action<DateTime>? onDateSubmitted = null, global::System.Action<DateTime>? onDateSaved = null, global::System.Func<DateTime, bool>? selectableDayPredicate = null, string? errorFormatText = null, string? errorInvalidText = null, string? fieldHintText = null, string? fieldLabelText = null, global::Doroti.Framework.Services.TextInputType? keyboardType = null, bool autofocus = false, bool acceptEmptyDate = false, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, CalendarDelegate<DateTime> calendarDelegate = default!) : base(key: key)
    {
        CalendarDelegate<DateTime> __calendarDelegate = calendarDelegate ?? new GregorianCalendarDelegate();
        this.onDateSubmitted = onDateSubmitted;
        this.onDateSaved = onDateSaved;
        this.selectableDayPredicate = selectableDayPredicate;
        this.errorFormatText = errorFormatText;
        this.errorInvalidText = errorInvalidText;
        this.fieldHintText = fieldHintText;
        this.fieldLabelText = fieldLabelText;
        this.keyboardType = keyboardType;
        this.autofocus = autofocus;
        this.acceptEmptyDate = acceptEmptyDate;
        this.focusNode = focusNode;
        this.calendarDelegate = __calendarDelegate;
        this.initialDate = ((initialDate is not null) ? calendarDelegate.dateOnly(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(initialDate))) : null);
        this.firstDate = calendarDelegate.dateOnly(firstDate);
        this.lastDate = calendarDelegate.dateOnly(lastDate);
        DartRuntimePrimitives.Assert(() => !this.lastDate.isBefore(this.firstDate), () => (object?)$"lastDate {this.lastDate} must be on or after firstDate {this.firstDate}.");
        DartRuntimePrimitives.Assert(() => ((initialDate is null) || !DartRuntimePrimitives.RequireValue(this.initialDate).isBefore(this.firstDate)), () => (object?)$"initialDate {this.initialDate} must be on or after firstDate {this.firstDate}.");
        DartRuntimePrimitives.Assert(() => ((initialDate is null) || !DartRuntimePrimitives.RequireValue(this.initialDate).isAfter(this.lastDate)), () => (object?)$"initialDate {this.initialDate} must be on or before lastDate {this.lastDate}.");
        DartRuntimePrimitives.Assert(() => (((this.selectableDayPredicate is null) || (initialDate is null)) || this.selectableDayPredicate!(DartRuntimePrimitives.RequireValue(this.initialDate))), () => (object?)$"Provided initialDate {this.initialDate} must satisfy provided selectableDayPredicate.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _InputDatePickerFormFieldState__input_date_picker_form_field());
}

internal class _InputDatePickerFormFieldState__input_date_picker_form_field : global::Doroti.Framework.Widgets.State<InputDatePickerFormField>
{
    internal virtual global::Doroti.Framework.Widgets.TextEditingController _controller { get; private set; } = new global::Doroti.Framework.Widgets.TextEditingController();
    internal virtual DateTime? _selectedDate { get; set; } = default;
    internal virtual string? _inputText { get; set; } = default;
    internal virtual bool _autoSelected { get; set; } = false;

    public override void initState()
    {
        base.initState();
        _selectedDate = ((InputDatePickerFormField)this.widget).initialDate;
    }

    public override void dispose()
    {
        this._controller.dispose();
        base.dispose();
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _updateValueForSelectedDate();
    }

    public override void didUpdateWidget(InputDatePickerFormField oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((InputDatePickerFormField)this.widget).initialDate, ((InputDatePickerFormField)oldWidget).initialDate)))
        {
            global::Doroti.Framework.Widgets.WidgetsBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timeStamp) => {
setState(((global::System.Action)(() => {
_selectedDate = ((InputDatePickerFormField)this.widget).initialDate;
_updateValueForSelectedDate();
})));
})), debugLabel: "InputDatePickerFormField.update");
        }
    }

    internal virtual void _updateValueForSelectedDate()
    {
        if ((this._selectedDate is not null))
        {
            MaterialLocalizations localizations__6930 = ((MaterialLocalizations)(object?)MaterialLocalizations.of(this.context));
            _inputText = ((InputDatePickerFormField)this.widget).calendarDelegate.formatCompactDate(DartRuntimePrimitives.RequireValue(this._selectedDate), localizations__6930);
            var textEditingValue__7084 = new global::Doroti.Framework.Services.TextEditingValue(text: this._inputText!);
            if ((((InputDatePickerFormField)this.widget).autofocus && !this._autoSelected))
            {
                textEditingValue__7084 = textEditingValue__7084.copyWith(selection: new global::Doroti.Framework.Services.TextSelection(baseOffset: 0L, extentOffset: this._inputText!.Length));
                _autoSelected = true;
            }
            this._controller.value = textEditingValue__7084;
        }
        else
        {
            _inputText = "";
            this._controller.value = new global::Doroti.Framework.Services.TextEditingValue(text: this._inputText!);
        }
    }

    internal virtual DateTime? _parseDate(string? text)
    {
        MaterialLocalizations localizations__7691 = ((MaterialLocalizations)(object?)MaterialLocalizations.of(this.context));
        return ((InputDatePickerFormField)this.widget).calendarDelegate.parseCompactDate(text, localizations__7691);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isValidAcceptableDate(DateTime? date)
    {
        return ((((date is not null) && !DartRuntimePrimitives.RequireValue(date).isBefore(((InputDatePickerFormField)this.widget).firstDate)) && !DartRuntimePrimitives.RequireValue(date).isAfter(((InputDatePickerFormField)this.widget).lastDate)) && (((((InputDatePickerFormField)this.widget).selectableDayPredicate is null) || ((InputDatePickerFormField)this.widget).selectableDayPredicate!(DartRuntimePrimitives.RequireValue(date)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual string? _validateDate(string? text)
    {
        if (((((text is null) || (text.Length == 0))) && ((InputDatePickerFormField)this.widget).acceptEmptyDate))
        {
            return null;
        }
        DateTime? date__8229 = _parseDate(text);
        if ((date__8229 is null))
        {
            return (((InputDatePickerFormField)this.widget).errorFormatText ?? MaterialLocalizations.of(this.context).invalidDateFormatLabel);
        }
        else
        {
            if (!_isValidAcceptableDate(DartRuntimePrimitives.RequireValue(date__8229)))
            {
                return (((InputDatePickerFormField)this.widget).errorInvalidText ?? MaterialLocalizations.of(this.context).dateOutOfRangeLabel);
            }
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _updateDate(string? text, global::System.Action<DateTime>? callback)
    {
        DateTime? date__8635 = _parseDate(text);
        if (_isValidAcceptableDate(date__8635))
        {
            _selectedDate = date__8635;
            _inputText = text;
            callback?.Invoke(DartRuntimePrimitives.RequireValue(this._selectedDate));
        }
    }

    internal virtual void _handleSaved(string? text)
    {
        _updateDate(text, (global::System.Action<DateTime>?)((InputDatePickerFormField)this.widget).onDateSaved);
    }

    internal virtual void _handleSubmitted(string text)
    {
        _updateDate(text, (global::System.Action<DateTime>?)((InputDatePickerFormField)this.widget).onDateSubmitted);
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData theme__9048 = Theme.of(context);
        bool useMaterial3__9090 = theme__9048.useMaterial3;
        MaterialLocalizations localizations__9157 = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        DatePickerThemeData datePickerTheme__9238 = theme__9048.datePickerTheme;
        InputDecorationThemeData inputTheme__9314 = ((InputDecorationThemeData)(object?)InputDecorationTheme.of(context));
        InputBorder effectiveInputBorder__9383 = ((datePickerTheme__9238.inputDecorationTheme?.border ?? ((InputDecorationThemeData)inputTheme__9314).border) ?? ((useMaterial3__9090 ? new OutlineInputBorder() : new UnderlineInputBorder())));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(container: true, child: new TextFormField(decoration: new InputDecoration(hintText: ((((InputDatePickerFormField)this.widget).fieldHintText ?? (string)((InputDatePickerFormField)this.widget).calendarDelegate.dateHelpText(localizations__9157))), labelText: ((((InputDatePickerFormField)this.widget).fieldLabelText ?? (string)((MaterialLocalizations)localizations__9157).dateInputLabel))).applyDefaults(inputTheme__9314.merge(datePickerTheme__9238.inputDecorationTheme).copyWith(border: effectiveInputBorder__9383)), validator: this._validateDate, keyboardType: (((InputDatePickerFormField)this.widget).keyboardType ?? global::Doroti.Framework.Services.TextInputType.datetime), onSaved: this._handleSaved, onFieldSubmitted: this._handleSubmitted, autofocus: ((InputDatePickerFormField)this.widget).autofocus, controller: this._controller, focusNode: ((InputDatePickerFormField)this.widget).focusNode)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
