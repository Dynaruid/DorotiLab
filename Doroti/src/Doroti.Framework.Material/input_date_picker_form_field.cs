// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/input_date_picker_form_field.dart

using Doroti.Runtime;

namespace Doroti.Framework.Material;

public class InputDatePickerFormField : StatefulWidget
{
    public virtual DateTime? initialDate { get; private set; }
    public virtual DateTime firstDate { get; private set; } = default!;
    public virtual DateTime lastDate { get; private set; } = default!;
    public virtual Action<DateTime>? onDateSubmitted { get; private set; }
    public virtual Action<DateTime>? onDateSaved { get; private set; }
    public virtual Func<DateTime, bool>? selectableDayPredicate { get; private set; }
    public virtual string? errorFormatText { get; private set; }
    public virtual string? errorInvalidText { get; private set; }
    public virtual string? fieldHintText { get; private set; }
    public virtual string? fieldLabelText { get; private set; }
    public virtual TextInputType? keyboardType { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual bool acceptEmptyDate { get; private set; } = default!;
    public virtual FocusNode? focusNode { get; private set; }
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    public InputDatePickerFormField(
        Key? key = null,
        DateTime? initialDate = null,
        DateTime firstDate = default!,
        DateTime lastDate = default!,
        Action<DateTime>? onDateSubmitted = null,
        Action<DateTime>? onDateSaved = null,
        Func<DateTime, bool>? selectableDayPredicate = null,
        string? errorFormatText = null,
        string? errorInvalidText = null,
        string? fieldHintText = null,
        string? fieldLabelText = null,
        TextInputType? keyboardType = null,
        bool autofocus = false,
        bool acceptEmptyDate = false,
        FocusNode? focusNode = null,
        CalendarDelegate<DateTime> calendarDelegate = default!
    )
        : base(key: key)
    {
        CalendarDelegate<DateTime> __calendarDelegate =
            calendarDelegate ?? new GregorianCalendarDelegate();
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
        this.initialDate =
            (initialDate is not null)
                ? this.calendarDelegate.dateOnly(
                    DartRuntimePrimitives.RequireValue(
                        DartRuntimePrimitives.RequireValue(initialDate)
                    )
                )
                : null;
        this.firstDate = this.calendarDelegate.dateOnly(firstDate);
        this.lastDate = this.calendarDelegate.dateOnly(lastDate);
        DartRuntimePrimitives.Assert(
            () => !this.lastDate.isBefore(this.firstDate),
            () =>
                (object?)$"lastDate {this.lastDate} must be on or after firstDate {this.firstDate}."
        );
        DartRuntimePrimitives.Assert(
            () =>
                (initialDate is null)
                || !DartRuntimePrimitives.RequireValue(this.initialDate).isBefore(this.firstDate),
            () =>
                (object?)
                    $"initialDate {this.initialDate} must be on or after firstDate {this.firstDate}."
        );
        DartRuntimePrimitives.Assert(
            () =>
                (initialDate is null)
                || !DartRuntimePrimitives.RequireValue(this.initialDate).isAfter(this.lastDate),
            () =>
                (object?)
                    $"initialDate {this.initialDate} must be on or before lastDate {this.lastDate}."
        );
        DartRuntimePrimitives.Assert(
            () =>
                (this.selectableDayPredicate is null)
                || (initialDate is null)
                || this.selectableDayPredicate!(
                    DartRuntimePrimitives.RequireValue(this.initialDate)
                ),
            () =>
                (object?)
                    $"Provided initialDate {this.initialDate} must satisfy provided selectableDayPredicate."
        );
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _InputDatePickerFormFieldState__input_date_picker_form_field()
        );
}

internal class _InputDatePickerFormFieldState__input_date_picker_form_field
    : State<InputDatePickerFormField>
{
    internal virtual TextEditingController _controller { get; private set; } =
        new TextEditingController();
    internal virtual DateTime? _selectedDate { get; set; } = default;
    internal virtual string? _inputText { get; set; } = default;
    internal virtual bool _autoSelected { get; set; } = false;

    public override void initState()
    {
        base.initState();
        _selectedDate = widget.initialDate;
    }

    public override void dispose()
    {
        _controller.dispose();
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
        if (!Equals(widget.initialDate, oldWidget.initialDate))
        {
            WidgetsBinding.instance.addPostFrameCallback(
                (timeStamp) =>
                {
                    setState(() =>
                    {
                        _selectedDate = widget.initialDate;
                        _updateValueForSelectedDate();
                    });
                },
                debugLabel: "InputDatePickerFormField.update"
            );
        }
    }

    internal virtual void _updateValueForSelectedDate()
    {
        if (_selectedDate is not null)
        {
            MaterialLocalizations localizations = MaterialLocalizations.of(context);
            _inputText = widget.calendarDelegate.formatCompactDate(
                DartRuntimePrimitives.RequireValue(_selectedDate),
                localizations
            );
            var textEditingValue = new TextEditingValue(text: _inputText!);
            if (widget.autofocus && !_autoSelected)
            {
                textEditingValue = textEditingValue.copyWith(
                    selection: new TextSelection(baseOffset: 0L, extentOffset: _inputText!.Length)
                );
                _autoSelected = true;
            }
            _controller.value = textEditingValue;
        }
        else
        {
            _inputText = "";
            _controller.value = new TextEditingValue(text: _inputText!);
        }
    }

    internal virtual DateTime? _parseDate(string? text)
    {
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        return widget.calendarDelegate.parseCompactDate(text, localizations);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isValidAcceptableDate(DateTime? date)
    {
        return (date is not null)
            && !DartRuntimePrimitives.RequireValue(date).isBefore(widget.firstDate)
            && !DartRuntimePrimitives.RequireValue(date).isAfter(widget.lastDate)
            && (
                (widget.selectableDayPredicate is null)
                || widget.selectableDayPredicate!(DartRuntimePrimitives.RequireValue(date))
            );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual string? _validateDate(string? text)
    {
        if (((text is null) || (text.Length == 0)) && widget.acceptEmptyDate)
        {
            return null;
        }
        DateTime? date = _parseDate(text);
        if (date is null)
        {
            return widget.errorFormatText
                ?? MaterialLocalizations.of(context).invalidDateFormatLabel;
        }
        else
        {
            if (!_isValidAcceptableDate(DartRuntimePrimitives.RequireValue(date)))
            {
                return widget.errorInvalidText
                    ?? MaterialLocalizations.of(context).dateOutOfRangeLabel;
            }
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _updateDate(string? text, Action<DateTime>? callback)
    {
        DateTime? date = _parseDate(text);
        if (_isValidAcceptableDate(date))
        {
            _selectedDate = date;
            _inputText = text;
            callback?.Invoke(DartRuntimePrimitives.RequireValue(_selectedDate));
        }
    }

    internal virtual void _handleSaved(string? text)
    {
        _updateDate(text, widget.onDateSaved);
    }

    internal virtual void _handleSubmitted(string text)
    {
        _updateDate(text, widget.onDateSubmitted);
    }

    public override Widget build(BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        DatePickerThemeData datePickerThemeLocal = theme.datePickerTheme;
        InputDecorationThemeData inputTheme = InputDecorationTheme.of(context);
        InputBorder effectiveInputBorder =
            (datePickerThemeLocal.inputDecorationTheme?.border ?? inputTheme.border)
            ?? new OutlineInputBorder();
        return new Widgets.Semantics(
            container: true,
            child: new TextFormField(
                decoration: new InputDecoration(
                    hintText: widget.fieldHintText
                        ?? widget.calendarDelegate.dateHelpText(localizations),
                    labelText: widget.fieldLabelText ?? localizations.dateInputLabel
                ).applyDefaults(
                    inputTheme
                        .merge(datePickerThemeLocal.inputDecorationTheme)
                        .copyWith(border: effectiveInputBorder)
                ),
                validator: _validateDate,
                keyboardType: widget.keyboardType ?? TextInputType.datetime,
                onSaved: _handleSaved,
                onFieldSubmitted: _handleSubmitted,
                autofocus: widget.autofocus,
                controller: _controller,
                focusNode: widget.focusNode
            )
        );
    }
}
