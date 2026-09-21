// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/date_picker.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class Date_pickerLibrary { }

public static partial class Date_pickerLibrary
{
    internal static Size _calendarPortraitDialogSizeM3 = new Size(360.0, 568.0);
}

public static partial class Date_pickerLibrary
{
    internal static Size _calendarLandscapeDialogSize = new Size(496.0, 346.0);
}

public static partial class Date_pickerLibrary { }

public static partial class Date_pickerLibrary
{
    internal static Size _inputPortraitDialogSizeM3 = new Size(328.0, 270.0);
}

public static partial class Date_pickerLibrary
{
    internal static Size _inputLandscapeDialogSize = new Size(496, 160.0);
}

public static partial class Date_pickerLibrary
{
    internal static Size _inputRangeLandscapeDialogSize = new Size(496, 164.0);
}

public static partial class Date_pickerLibrary
{
    internal static Duration _dialogSizeAnimationDuration = Duration.Create(milliseconds: 200L);
}

public static partial class Date_pickerLibrary
{
    internal static double _inputFormPortraitHeight = 98.0;
}

public static partial class Date_pickerLibrary
{
    internal static double _inputFormLandscapeHeight = 108.0;
}

public static partial class Date_pickerLibrary
{
    internal static double _kMaxTextScaleFactor = 3.0;
}

public static partial class Date_pickerLibrary
{
    internal static double _kMaxRangeTextScaleFactor = 1.3;
}

public static partial class Date_pickerLibrary
{
    internal static double _kMaxHeaderTextScaleFactor = 1.6;
}

public static partial class Date_pickerLibrary
{
    internal static double _kMaxHeaderWithEntryTextScaleFactor = 1.4;
}

public static partial class Date_pickerLibrary
{
    internal static double _kMaxHelpPortraitTextScaleFactor = 1.6;
}

public static partial class Date_pickerLibrary
{
    internal static double _kMaxHelpLandscapeTextScaleFactor = 1.4;
}

public static partial class Date_pickerLibrary
{
    internal static double _fontSizeToScale = 14.0;
}

public static partial class Date_pickerLibrary
{
    public static async Future<DateTime?> showDatePicker(
        BuildContext context,
        DateTime? initialDate = null,
        DateTime firstDate = default!,
        DateTime lastDate = default!,
        DateTime? currentDate = null,
        DatePickerEntryMode initialEntryMode = DatePickerEntryMode.calendar,
        Func<DateTime, bool>? selectableDayPredicate = null,
        string? helpText = null,
        string? cancelText = null,
        string? confirmText = null,
        Locale? locale = null,
        bool barrierDismissible = true,
        Color? barrierColor = null,
        string? barrierLabel = null,
        bool useRootNavigator = true,
        RouteSettings? routeSettings = null,
        TextDirection? textDirection = null,
        Func<BuildContext, Widget?, Widget>? builder = null,
        DatePickerMode initialDatePickerMode = DatePickerMode.day,
        string? errorFormatText = null,
        string? errorInvalidText = null,
        string? fieldHintText = null,
        string? fieldLabelText = null,
        TextInputType? keyboardType = null,
        Offset? anchorPoint = null,
        Action<DatePickerEntryMode>? onDatePickerModeChange = null,
        Icon? switchToInputEntryModeIcon = null,
        Icon? switchToCalendarEntryModeIcon = null,
        CalendarDelegate<DateTime> calendarDelegate = default!
    )
    {
        calendarDelegate ??= new GregorianCalendarDelegate();
        initialDate =
            (initialDate is null)
                ? null
                : calendarDelegate.dateOnly(
                    DartRuntimePrimitives.RequireValue(
                        DartRuntimePrimitives.RequireValue(initialDate)
                    )
                );
        firstDate = calendarDelegate.dateOnly(firstDate);
        lastDate = calendarDelegate.dateOnly(lastDate);
        DartRuntimePrimitives.Assert(
            () => !lastDate.isBefore(firstDate),
            () => (object?)$"lastDate {lastDate} must be on or after firstDate {firstDate}."
        );
        DartRuntimePrimitives.Assert(
            () =>
                (initialDate is null)
                || !DartRuntimePrimitives.RequireValue(initialDate).isBefore(firstDate),
            () =>
                (object?)
                    $"initialDate {DartRuntimePrimitives.RequireValue(initialDate)} must be on or after firstDate {firstDate}."
        );
        DartRuntimePrimitives.Assert(
            () =>
                (initialDate is null)
                || !DartRuntimePrimitives.RequireValue(initialDate).isAfter(lastDate),
            () =>
                (object?)
                    $"initialDate {DartRuntimePrimitives.RequireValue(initialDate)} must be on or before lastDate {lastDate}."
        );
        DartRuntimePrimitives.Assert(
            () =>
                (selectableDayPredicate is null)
                || (initialDate is null)
                || selectableDayPredicate(DartRuntimePrimitives.RequireValue(initialDate)),
            () =>
                (object?)
                    $"Provided initialDate {DartRuntimePrimitives.RequireValue(initialDate)} must satisfy provided selectableDayPredicate."
        );
        DartRuntimePrimitives.Assert(() =>
            DebugLibrary.debugCheckHasMaterialLocalizations(context)
        );
        Widget dialog = new DatePickerDialog(
            initialDate: initialDate,
            firstDate: firstDate,
            lastDate: lastDate,
            currentDate: currentDate,
            initialEntryMode: initialEntryMode,
            selectableDayPredicate: selectableDayPredicate,
            helpText: helpText,
            cancelText: cancelText,
            confirmText: confirmText,
            initialCalendarMode: initialDatePickerMode,
            errorFormatText: errorFormatText,
            errorInvalidText: errorInvalidText,
            fieldHintText: fieldHintText,
            fieldLabelText: fieldLabelText,
            keyboardType: keyboardType,
            onDatePickerModeChange: onDatePickerModeChange,
            switchToInputEntryModeIcon: switchToInputEntryModeIcon,
            switchToCalendarEntryModeIcon: switchToCalendarEntryModeIcon,
            calendarDelegate: calendarDelegate
        );
        if (textDirection is not null)
        {
            TextDirection textDirection__value11363 = DartRuntimePrimitives.RequireValue(
                textDirection
            );
            dialog = DartRuntimePrimitives.ConvertValue<Widget>(
                new Directionality(
                    textDirection: DartRuntimePrimitives.RequireValue(
                        DartRuntimePrimitives.RequireValue(textDirection__value11363)
                    ),
                    child: dialog
                )
            );
        }
        if (locale is not null)
        {
            Locale locale__value11473 = DartRuntimePrimitives.RequireValue(locale);
            dialog = DartRuntimePrimitives.ConvertValue<Widget>(
                Localizations.CreateOverride(
                    context: context,
                    locale: DartRuntimePrimitives.RequireValue(locale__value11473),
                    child: dialog
                )
            );
        }
        else
        {
            DatePickerThemeData datePickerTheme = DatePickerTheme.of(context);
            if (datePickerTheme.locale is not null)
            {
                dialog = DartRuntimePrimitives.ConvertValue<Widget>(
                    Localizations.CreateOverride(
                        context: context,
                        locale: datePickerTheme.locale,
                        child: dialog
                    )
                );
            }
        }
        return await DialogLibrary.showDialog<DateTime?>(
            context: context,
            barrierDismissible: barrierDismissible,
            barrierColor: barrierColor,
            barrierLabel: barrierLabel,
            useRootNavigator: useRootNavigator,
            routeSettings: routeSettings,
            builder: (context) =>
            {
                return (builder is null) ? dialog : builder(context, dialog);
                throw new InvalidOperationException("Dart closure completed without a value.");
            },
            anchorPoint: anchorPoint
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class DatePickerDialog : StatefulWidget
{
    public virtual DateTime? initialDate { get; private set; }
    public virtual DateTime firstDate { get; private set; } = default!;
    public virtual DateTime lastDate { get; private set; } = default!;
    public virtual DateTime currentDate { get; private set; } = default!;
    public virtual DatePickerEntryMode initialEntryMode { get; private set; } = default!;
    public virtual Func<DateTime, bool>? selectableDayPredicate { get; private set; }
    public virtual string? cancelText { get; private set; }
    public virtual string? confirmText { get; private set; }
    public virtual string? helpText { get; private set; }
    public virtual DatePickerMode initialCalendarMode { get; private set; } = default!;
    public virtual string? errorFormatText { get; private set; }
    public virtual string? errorInvalidText { get; private set; }
    public virtual string? fieldHintText { get; private set; }
    public virtual string? fieldLabelText { get; private set; }
    public virtual TextInputType? keyboardType { get; private set; }
    public virtual string? restorationId { get; private set; }
    public virtual Action<DatePickerEntryMode>? onDatePickerModeChange { get; private set; }
    public virtual Icon? switchToInputEntryModeIcon { get; private set; }
    public virtual Icon? switchToCalendarEntryModeIcon { get; private set; }
    public virtual EdgeInsets insetPadding { get; private set; } = default!;
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    public DatePickerDialog(
        Key? key = null,
        DateTime? initialDate = null,
        DateTime firstDate = default!,
        DateTime lastDate = default!,
        DateTime? currentDate = null,
        DatePickerEntryMode initialEntryMode = DatePickerEntryMode.calendar,
        Func<DateTime, bool>? selectableDayPredicate = null,
        string? cancelText = null,
        string? confirmText = null,
        string? helpText = null,
        DatePickerMode initialCalendarMode = DatePickerMode.day,
        string? errorFormatText = null,
        string? errorInvalidText = null,
        string? fieldHintText = null,
        string? fieldLabelText = null,
        TextInputType? keyboardType = null,
        string? restorationId = null,
        Action<DatePickerEntryMode>? onDatePickerModeChange = null,
        Icon? switchToInputEntryModeIcon = null,
        Icon? switchToCalendarEntryModeIcon = null,
        EdgeInsets insetPadding = default!,
        CalendarDelegate<DateTime> calendarDelegate = default!
    )
        : base(key: key)
    {
        EdgeInsets __insetPadding =
            insetPadding ?? EdgeInsets.CreateSymmetric(horizontal: 16.0, vertical: 24.0);
        CalendarDelegate<DateTime> __calendarDelegate =
            calendarDelegate ?? new GregorianCalendarDelegate();
        this.initialEntryMode = initialEntryMode;
        this.selectableDayPredicate = selectableDayPredicate;
        this.cancelText = cancelText;
        this.confirmText = confirmText;
        this.helpText = helpText;
        this.initialCalendarMode = initialCalendarMode;
        this.errorFormatText = errorFormatText;
        this.errorInvalidText = errorInvalidText;
        this.fieldHintText = fieldHintText;
        this.fieldLabelText = fieldLabelText;
        this.keyboardType = keyboardType;
        this.restorationId = restorationId;
        this.onDatePickerModeChange = onDatePickerModeChange;
        this.switchToInputEntryModeIcon = switchToInputEntryModeIcon;
        this.switchToCalendarEntryModeIcon = switchToCalendarEntryModeIcon;
        this.insetPadding = __insetPadding;
        this.calendarDelegate = __calendarDelegate;
        this.initialDate =
            (initialDate is null)
                ? null
                : this.calendarDelegate.dateOnly(
                    DartRuntimePrimitives.RequireValue(
                        DartRuntimePrimitives.RequireValue(initialDate)
                    )
                );
        this.firstDate = this.calendarDelegate.dateOnly(firstDate);
        this.lastDate = this.calendarDelegate.dateOnly(lastDate);
        this.currentDate = this.calendarDelegate.dateOnly(
            currentDate ?? this.calendarDelegate.now()
        );
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
                    $"Provided initialDate {this.initialDate} must satisfy provided selectableDayPredicate"
        );
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _DatePickerDialogState__date_picker());
}

internal class _DatePickerDialogState__date_picker
    : State<DatePickerDialog>,
        RestorationMixin<DatePickerDialog>
{
    private bool __late__selectedDate_initialized;
    private RestorableDateTimeN __late__selectedDate = default!;
    internal virtual RestorableDateTimeN _selectedDate
    {
        get
        {
            if (!__late__selectedDate_initialized)
            {
                __late__selectedDate = new RestorableDateTimeN(widget.initialDate);
                __late__selectedDate_initialized = true;
            }
            return __late__selectedDate;
        }
    }
    private bool __late__entryMode_initialized;
    private _RestorableDatePickerEntryMode__date_picker __late__entryMode = default!;
    internal virtual _RestorableDatePickerEntryMode__date_picker _entryMode
    {
        get
        {
            if (!__late__entryMode_initialized)
            {
                __late__entryMode = new _RestorableDatePickerEntryMode__date_picker(
                    widget.initialEntryMode
                );
                __late__entryMode_initialized = true;
            }
            return __late__entryMode;
        }
    }
    internal virtual _RestorableAutovalidateMode__date_picker _autovalidateMode
    {
        get;
        private set;
    } = new _RestorableAutovalidateMode__date_picker(AutovalidateMode.disabled);
    internal virtual GlobalKey<IState> _calendarPickerKey { get; private set; } =
        GlobalKey<IState>.Create();
    internal virtual GlobalKey<FormState> _formKey { get; private set; } =
        GlobalKey<FormState>.Create();
    internal static DartMap<ShortcutActivator, Intent> _formShortcutMap = new DartMap<
        ShortcutActivator,
        Intent
    >
    {
        [new SingleActivator(LogicalKeyboardKey.enter)] = new NextFocusIntent(),
    };
    public virtual RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<IRestorableProperty, Action> _properties { get; set; } =
        new DartMap<IRestorableProperty, Action>();
    public virtual List<IRestorableProperty>? _debugPropertiesWaitingForReregistration { get; set; } =
        default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual RestorationBucket? _currentParent { get; set; } = default;

    public override void dispose()
    {
        _selectedDate.dispose();
        _entryMode.dispose();
        _autovalidateMode.dispose();
        _properties.forEach(
            (property, listener) =>
            {
                if (!property._disposed)
                {
                    property.removeListener(listener);
                }
            }
        );
        _bucket?.dispose();
        _bucket = null;
        base.dispose();
    }

    public virtual string? restorationId => widget.restorationId;

    public virtual void restoreState(RestorationBucket? oldBucket, bool initialRestore)
    {
        registerForRestoration(_selectedDate, "selected_date");
        registerForRestoration(_autovalidateMode, "autovalidateMode");
        registerForRestoration(_entryMode, "calendar_entry_mode");
    }

    internal virtual void _handleOk()
    {
        if (
            Equals(_entryMode.value, DatePickerEntryMode.input)
            || Equals(_entryMode.value, DatePickerEntryMode.inputOnly)
        )
        {
            FormState form = _formKey.currentState!;
            if (!form.validate())
            {
                setState(() =>
                {
                    _ = _autovalidateMode.value = AutovalidateMode.always;
                });
                return;
            }
            form.save();
        }
        Navigator.pop<object>(context, _selectedDate.value);
    }

    internal virtual void _handleCancel()
    {
        Navigator.pop<object>(context);
    }

    internal virtual void _handleOnDatePickerModeChange()
    {
        widget.onDatePickerModeChange?.Invoke(_entryMode.value);
    }

    internal virtual void _handleEntryModeToggle()
    {
        setState(() =>
        {
            switch (_entryMode.value)
            {
                case DatePickerEntryMode.calendar:
                {
                    _autovalidateMode.value = AutovalidateMode.disabled;
                    _entryMode.value = DatePickerEntryMode.input;
                    _handleOnDatePickerModeChange();
                    break;
                }
                case DatePickerEntryMode.input:
                {
                    _formKey.currentState!.save();
                    _entryMode.value = DatePickerEntryMode.calendar;
                    _handleOnDatePickerModeChange();
                    break;
                }
                case DatePickerEntryMode.calendarOnly:
                case DatePickerEntryMode.inputOnly:
                {
                    DartRuntimePrimitives.Assert(
                        () => false,
                        () => (object?)$"Can not change entry mode from {_entryMode.value}"
                    );
                    break;
                }
            }
        });
    }

    internal virtual void _handleDateChanged(DateTime date)
    {
        setState(() =>
        {
            _ = _selectedDate.value = date;
        });
    }

    internal virtual Size _dialogSize(BuildContext context)
    {
        bool isCalendar = _entryMode.value switch
        {
            DatePickerEntryMode.calendar => true,
            DatePickerEntryMode.calendarOnly => true,
            DatePickerEntryMode.input => false,
            DatePickerEntryMode.inputOnly => false,
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        Orientation orientation = MediaQuery.orientationOf(context);
        return (isCalendar, orientation) switch
        {
            (true, Orientation.portrait) => Date_pickerLibrary._calendarPortraitDialogSizeM3,
            (false, Orientation.portrait) => Date_pickerLibrary._inputPortraitDialogSizeM3,
            (true, Orientation.landscape) => Date_pickerLibrary._calendarLandscapeDialogSize,
            (false, Orientation.landscape) => Date_pickerLibrary._inputLandscapeDialogSize,
        };
    }

    public override Widget build(BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        Orientation orientationLocal = MediaQuery.orientationOf(context);
        var isLandscapeOrientation = Equals(orientationLocal, Orientation.landscape);
        DatePickerThemeData datePickerTheme = DatePickerTheme.of(context);
        DatePickerThemeData defaultsLocal = DatePickerTheme.defaults(context);
        TextTheme textThemeLocal = theme.textTheme;
        TextStyle? headlineStyle = default!;
        {
            headlineStyle =
                datePickerTheme.headerHeadlineStyle ?? defaultsLocal.headerHeadlineStyle;
            switch (_entryMode.value)
            {
                case DatePickerEntryMode.input:
                case DatePickerEntryMode.inputOnly:
                {
                    if (Equals(orientationLocal, Orientation.landscape))
                    {
                        headlineStyle = textThemeLocal.headlineSmall;
                    }
                    break;
                }
                case DatePickerEntryMode.calendar:
                case DatePickerEntryMode.calendarOnly:
                    break;
            }
        }
        Color? headerForegroundColorLocal =
            datePickerTheme.headerForegroundColor ?? defaultsLocal.headerForegroundColor;
        headlineStyle = headlineStyle?.copyWith(color: headerForegroundColorLocal);
        Widget actions = new ConstrainedBox(
            constraints: new BoxConstraints(minHeight: 52.0),
            child: MediaQuery.withClampedTextScaling(
                maxScaleFactor: isLandscapeOrientation
                    ? 1.6
                    : Calendar_date_pickerLibrary._kMaxTextScaleFactor,
                child: new Padding(
                    padding: EdgeInsets.CreateSymmetric(horizontal: 8),
                    child: new Align(
                        alignment: AlignmentDirectional.centerEnd,
                        child: new OverflowBar(
                            spacing: 8,
                            children: new List<Widget>
                            {
                                DartRuntimePrimitives.ConvertValue<Widget>(
                                    new TextButton(
                                        style: datePickerTheme.cancelButtonStyle
                                            ?? defaultsLocal.cancelButtonStyle,
                                        onPressed: _handleCancel,
                                        child: new Text(
                                            widget.cancelText ?? localizations.cancelButtonLabel
                                        )
                                    )
                                ),
                                DartRuntimePrimitives.ConvertValue<Widget>(
                                    new TextButton(
                                        style: datePickerTheme.confirmButtonStyle
                                            ?? defaultsLocal.confirmButtonStyle,
                                        onPressed: _handleOk,
                                        child: new Text(
                                            widget.confirmText ?? localizations.okButtonLabel
                                        )
                                    )
                                ),
                            }
                        )
                    )
                )
            )
        );
        CalendarDatePicker calendarDatePicker()
        {
            return new CalendarDatePicker(
                calendarDelegate: widget.calendarDelegate,
                key: _calendarPickerKey,
                initialDate: _selectedDate.value,
                firstDate: widget.firstDate,
                lastDate: widget.lastDate,
                currentDate: widget.currentDate,
                onDateChanged: _handleDateChanged,
                selectableDayPredicate: widget.selectableDayPredicate,
                initialCalendarMode: widget.initialCalendarMode
            );
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        Form inputDatePicker()
        {
            return new Form(
                key: _formKey,
                autovalidateMode: _autovalidateMode.value,
                child: new SizedBox(
                    height: Equals(orientationLocal, Orientation.portrait)
                        ? Date_pickerLibrary._inputFormPortraitHeight
                        : Date_pickerLibrary._inputFormLandscapeHeight,
                    child: new Padding(
                        padding: EdgeInsets.CreateSymmetric(horizontal: 24),
                        child: new Shortcuts(
                            shortcuts: _formShortcutMap,
                            child: new Column(
                                mainAxisAlignment: MainAxisAlignment.center,
                                children: new List<Widget>
                                {
                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                        new Flexible(
                                            child: MediaQuery.withClampedTextScaling(
                                                maxScaleFactor: 2.0,
                                                child: new InputDatePickerFormField(
                                                    calendarDelegate: widget.calendarDelegate,
                                                    initialDate: _selectedDate.value,
                                                    firstDate: widget.firstDate,
                                                    lastDate: widget.lastDate,
                                                    onDateSubmitted: _handleDateChanged,
                                                    onDateSaved: _handleDateChanged,
                                                    selectableDayPredicate: widget.selectableDayPredicate,
                                                    errorFormatText: widget.errorFormatText,
                                                    errorInvalidText: widget.errorInvalidText,
                                                    fieldHintText: widget.fieldHintText,
                                                    fieldLabelText: widget.fieldLabelText,
                                                    keyboardType: widget.keyboardType,
                                                    autofocus: true
                                                )
                                            )
                                        )
                                    ),
                                }
                            )
                        )
                    )
                )
            );
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        Widget picker = default!;
        Widget? entryModeButtonLocal = default!;
        switch (_entryMode.value)
        {
            case DatePickerEntryMode.calendar:
            {
                picker = DartRuntimePrimitives.ConvertValue<Widget>(calendarDatePicker());
                entryModeButtonLocal = DartRuntimePrimitives.ConvertValue<Widget>(
                    new IconButton(
                        icon: widget.switchToInputEntryModeIcon ?? new Icon(Icons.edit_outlined),
                        color: headerForegroundColorLocal,
                        tooltip: localizations.inputDateModeButtonLabel,
                        onPressed: _handleEntryModeToggle
                    )
                );
                break;
            }
            case DatePickerEntryMode.calendarOnly:
            {
                picker = DartRuntimePrimitives.ConvertValue<Widget>(calendarDatePicker());
                entryModeButtonLocal = null;
                break;
            }
            case DatePickerEntryMode.input:
            {
                picker = DartRuntimePrimitives.ConvertValue<Widget>(inputDatePicker());
                entryModeButtonLocal = DartRuntimePrimitives.ConvertValue<Widget>(
                    new IconButton(
                        icon: widget.switchToCalendarEntryModeIcon
                            ?? new Icon(Icons.calendar_today),
                        color: headerForegroundColorLocal,
                        tooltip: localizations.calendarModeButtonLabel,
                        onPressed: _handleEntryModeToggle
                    )
                );
                break;
            }
            case DatePickerEntryMode.inputOnly:
            {
                picker = DartRuntimePrimitives.ConvertValue<Widget>(inputDatePicker());
                entryModeButtonLocal = null;
                break;
            }
        }
        Widget header = new _DatePickerHeader__date_picker(
            helpText: widget.helpText ?? localizations.datePickerHelpText,
            titleText: (_selectedDate.value is null)
                ? ""
                : widget.calendarDelegate.formatMediumDate(
                    DartRuntimePrimitives.RequireValue(_selectedDate.value),
                    localizations
                ),
            titleStyle: headlineStyle,
            orientation: orientationLocal,
            isShort: Equals(orientationLocal, Orientation.landscape),
            entryModeButton: entryModeButtonLocal
        );
        double textScaleFactor =
            MediaQuery
                .textScalerOf(context)
                .clamp(maxScaleFactor: Calendar_date_pickerLibrary._kMaxTextScaleFactor)
                .scale(Calendar_date_pickerLibrary._fontSizeToScale)
            / Calendar_date_pickerLibrary._fontSizeToScale;
        Size dialogSize = _dialogSize(context) * textScaleFactor;
        DialogThemeData dialogThemeLocal = theme.dialogTheme;
        return new Dialog(
            backgroundColor: datePickerTheme.backgroundColor ?? defaultsLocal.backgroundColor,
            elevation: datePickerTheme.elevation
                ?? DartRuntimePrimitives.RequireValue(defaultsLocal.elevation),
            shadowColor: datePickerTheme.shadowColor ?? defaultsLocal.shadowColor,
            surfaceTintColor: datePickerTheme.surfaceTintColor ?? defaultsLocal.surfaceTintColor,
            shape: datePickerTheme.shape ?? defaultsLocal.shape,
            insetPadding: widget.insetPadding,
            clipBehavior: Clip.antiAlias,
            child: new AnimatedContainer(
                width: dialogSize.width,
                height: dialogSize.height,
                duration: Date_pickerLibrary._dialogSizeAnimationDuration,
                curve: Curves.easeIn,
                child: MediaQuery.withClampedTextScaling(
                    maxScaleFactor: Calendar_date_pickerLibrary._kMaxTextScaleFactor,
                    child: new LayoutBuilder(
                        builder: (context, constraints) =>
                        {
                            Size portraitDialogSize = Date_pickerLibrary._inputPortraitDialogSizeM3;
                            bool isFullyPortrait =
                                constraints.maxHeight
                                >= Math.Min(dialogSize.height, portraitDialogSize.height);
                            switch (orientationLocal)
                            {
                                case Orientation.portrait:
                                {
                                    bool isInputMode =
                                        Equals(_entryMode.value, DatePickerEntryMode.inputOnly)
                                        || Equals(_entryMode.value, DatePickerEntryMode.input);
                                    bool showHeader = isFullyPortrait || !isInputMode;
                                    bool showPicker = isFullyPortrait || isInputMode;
                                    return new Column(
                                        mainAxisSize: MainAxisSize.min,
                                        crossAxisAlignment: CrossAxisAlignment.stretch,
                                        children: (
                                            (Func<List<Widget>>)(
                                                () =>
                                                {
                                                    var __collection31479 = new List<Widget>();
                                                    if (showHeader)
                                                    {
                                                        __collection31479.Add(
                                                            DartRuntimePrimitives.ConvertValue<Widget>(
                                                                header
                                                            )
                                                        );
                                                    }
                                                    {
                                                        __collection31479.Add(
                                                            DartRuntimePrimitives.ConvertValue<Widget>(
                                                                new Divider(
                                                                    height: 0,
                                                                    color: datePickerTheme.dividerColor
                                                                )
                                                            )
                                                        );
                                                    }
                                                    if (showPicker)
                                                    {
                                                        __collection31479.AddRange(
                                                            new List<Widget>
                                                            {
                                                                DartRuntimePrimitives.ConvertValue<Widget>(
                                                                    new Expanded(child: picker)
                                                                ),
                                                                DartRuntimePrimitives.ConvertValue<Widget>(
                                                                    actions
                                                                ),
                                                            }
                                                        );
                                                    }
                                                    return __collection31479;
                                                }
                                            )
                                        )()
                                    );
                                }
                                case Orientation.landscape:
                                {
                                    return new Row(
                                        mainAxisSize: MainAxisSize.min,
                                        crossAxisAlignment: CrossAxisAlignment.stretch,
                                        children: (
                                            (Func<List<Widget>>)(
                                                () =>
                                                {
                                                    var __collection31985 = new List<Widget>();
                                                    __collection31985.Add(
                                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                                            header
                                                        )
                                                    );
                                                    {
                                                        __collection31985.Add(
                                                            DartRuntimePrimitives.ConvertValue<Widget>(
                                                                new VerticalDivider(
                                                                    width: 0,
                                                                    color: datePickerTheme.dividerColor
                                                                )
                                                            )
                                                        );
                                                    }
                                                    __collection31985.Add(
                                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                                            new Flexible(
                                                                child: new Column(
                                                                    mainAxisSize: MainAxisSize.min,
                                                                    crossAxisAlignment: CrossAxisAlignment.stretch,
                                                                    children: new List<Widget>
                                                                    {
                                                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                                                            new Expanded(
                                                                                child: picker
                                                                            )
                                                                        ),
                                                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                                                            actions
                                                                        ),
                                                                    }
                                                                )
                                                            )
                                                        )
                                                    );
                                                    return __collection31985;
                                                }
                                            )
                                        )()
                                    );
                                }
                                default:
                                    throw new InvalidOperationException(
                                        "Non-exhaustive Dart switch value."
                                    );
                            }
                            throw new InvalidOperationException(
                                "Dart closure completed without a value."
                            );
                        }
                    )
                )
            )
        );
    }

    public virtual RestorationBucket? bucket => _bucket;

    public virtual void didToggleBucket(RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => _bucket?.isReplacing != true);
    }

    public virtual void registerForRestoration(IRestorableProperty property, string restorationId)
    {
        DartRuntimePrimitives.Assert(
            () =>
                (property._restorationId is null)
                || (_debugDoingRestore && (property._restorationId == restorationId)),
            () => (object?)$"Property is already registered under {property._restorationId}."
        );
        DartRuntimePrimitives.Assert(
            () =>
                _debugDoingRestore
                || !_properties.Keys.map((r) => r._restorationId).contains(restorationId),
            () => (object?)$"\"{restorationId}\" is already registered to another property."
        );
        bool hasSerializedValue = bucket?.contains(restorationId) ?? false;
        object? initialValue = hasSerializedValue
            ? property.fromPrimitivesObject(bucket!.read<object>(restorationId))
            : property.createDefaultValueObject();
        if (!property.isRegistered)
        {
            property._register(restorationId, this);
            void listener()
            {
                if (bucket is null)
                {
                    return;
                }
                _updateProperty(property);
            }
            property.addListener(listener);
            _properties[property] = listener;
        }
        DartRuntimePrimitives.Assert(() =>
            (property._restorationId == restorationId)
            && Equals(property._owner, this)
            && _properties.ContainsKey(property)
        );
        property.initWithValueObject(initialValue);
        if (!hasSerializedValue && property.enabled && (bucket is not null))
        {
            _updateProperty(property);
        }
        DartRuntimePrimitives.Assert(() =>
        {
            _debugPropertiesWaitingForReregistration?.Remove(property);
            return true;
        });
    }

    public virtual void unregisterFromRestoration(IRestorableProperty property)
    {
        DartRuntimePrimitives.Assert(() => Equals(property._owner, this));
        _bucket?.remove<object?>(property._restorationId!);
        _unregister(property);
    }

    public virtual void didUpdateRestorationId()
    {
        if ((_currentParent is null) || (_bucket?.restorationId == restorationId) || restorePending)
        {
            return;
        }
        RestorationBucket? oldBucket = _bucket;
        DartRuntimePrimitives.Assert(() => !restorePending);
        bool didReplaceBucket = _updateBucketIfNecessary(
            parent: _currentParent,
            restorePending: false
        );
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => !Equals(oldBucket, _bucket));
            DartRuntimePrimitives.Assert(() => (_bucket is null) || (oldBucket is null));
            oldBucket?.dispose();
        }
    }

    public override void didUpdateWidget(DatePickerDialog oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        didUpdateRestorationId();
    }

    public virtual bool restorePending
    {
        get
        {
            if (_firstRestorePending)
            {
                return true;
            }
            if (restorationId is null)
            {
                return false;
            }
            RestorationBucket? potentialNewParent = RestorationScope.maybeOf(context);
            return (!Equals(potentialNewParent, _currentParent))
                && (potentialNewParent?.isReplacing ?? false);
        }
    }
    public virtual bool _debugDoingRestore =>
        DartRuntimePrimitives.ConvertValue<bool>(
            _debugPropertiesWaitingForReregistration is not null
        );

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        RestorationBucket? oldBucket = _bucket;
        bool needsRestore = restorePending;
        _currentParent = RestorationScope.maybeOf(context);
        bool didReplaceBucket = _updateBucketIfNecessary(
            parent: _currentParent,
            restorePending: needsRestore
        );
        if (needsRestore)
        {
            _doRestore(oldBucket);
        }
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => !Equals(oldBucket, _bucket));
            oldBucket?.dispose();
        }
    }

    public virtual void _doRestore(RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            _debugPropertiesWaitingForReregistration = _properties.Keys.ToList();
            return true;
        });
        restoreState(oldBucket, _firstRestorePending);
        _firstRestorePending = false;
        DartRuntimePrimitives.Assert(() =>
        {
            if (Enumerable.Any(_debugPropertiesWaitingForReregistration!))
            {
                throw DartRuntimePrimitives.AsException(
                    new FlutterError(
                        (
                            (Func<List<DiagnosticsNode>>)(
                                () =>
                                {
                                    var __collection41817 = new List<DiagnosticsNode>();
                                    __collection41817.Add(
                                        new ErrorSummary(
                                            "Previously registered RestorableProperties must be re-registered in \"restoreState\"."
                                        )
                                    );
                                    __collection41817.Add(
                                        new ErrorDescription(
                                            $"The RestorableProperties with the following IDs were not re-registered to {this} when "
                                                + "\"restoreState\" was called:"
                                        )
                                    );
                                    __collection41817.AddRange(
                                        _debugPropertiesWaitingForReregistration!.map<
                                            IRestorableProperty,
                                            DiagnosticsNode
                                        >(
                                            (property) =>
                                                new ErrorDescription(
                                                    $" * {property._restorationId}"
                                                )
                                        )
                                    );
                                    return __collection41817;
                                }
                            )
                        )()
                    )
                );
            }
            _debugPropertiesWaitingForReregistration = null;
            return true;
        });
    }

    public virtual bool _updateBucketIfNecessary(RestorationBucket? parent, bool restorePending)
    {
        if ((restorationId is null) || (parent is null))
        {
            bool didReplace = _setNewBucketIfNecessary(
                newBucket: null,
                restorePending: restorePending
            );
            DartRuntimePrimitives.Assert(() => _bucket is null);
            return didReplace;
        }
        DartRuntimePrimitives.Assert(() => restorationId is not null);
        if (restorePending || (_bucket is null))
        {
            RestorationBucket newBucketLocal = parent.claimChild(restorationId!, debugOwner: this);
            bool didReplaceLocal = _setNewBucketIfNecessary(
                newBucket: newBucketLocal,
                restorePending: restorePending
            );
            DartRuntimePrimitives.Assert(() => Equals(_bucket, newBucketLocal));
            return didReplaceLocal;
        }
        DartRuntimePrimitives.Assert(() => _bucket is not null);
        DartRuntimePrimitives.Assert(() => !restorePending);
        _bucket!.rename(restorationId!);
        parent.adoptChild(_bucket!);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _setNewBucketIfNecessary(RestorationBucket? newBucket, bool restorePending)
    {
        if (Equals(newBucket, _bucket))
        {
            return false;
        }
        RestorationBucket? oldBucket = _bucket;
        _bucket = newBucket;
        if (!restorePending)
        {
            if (_bucket is not null)
            {
                _properties.Keys.forEach(
                    (__arg0) => ((Action<IRestorableProperty>)_updateProperty)(__arg0)
                );
            }
            didToggleBucket(oldBucket);
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _updateProperty(IRestorableProperty property)
    {
        if (property.enabled)
        {
            _bucket?.write(property._restorationId!, property.toPrimitives());
        }
        else
        {
            _bucket?.remove<object>(property._restorationId!);
        }
    }

    public virtual void _unregister(IRestorableProperty property)
    {
        Action listener = _properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
        {
            _debugPropertiesWaitingForReregistration?.Remove(property);
            return true;
        });
        property.removeListener(listener);
        property._unregister();
    }
}

internal class _RestorableDatePickerEntryMode__date_picker : RestorableValue<DatePickerEntryMode>
{
    internal virtual DatePickerEntryMode _defaultValue { get; private set; } = default!;

    internal _RestorableDatePickerEntryMode__date_picker(DatePickerEntryMode defaultValue)
    {
        _defaultValue = defaultValue;
    }

    public override DatePickerEntryMode createDefaultValue() => _defaultValue;

    public override void didUpdateValue(DatePickerEntryMode oldValue)
    {
        DartRuntimePrimitives.Assert(() =>
            RestorationLibrary.debugIsSerializableForRestoration(
                FoundationRuntimePorts.EnumIndex(value)
            )
        );
        notifyListeners();
    }

    public override DatePickerEntryMode fromPrimitives(object? data) =>
        Enum.GetValues<DatePickerEntryMode>().ToList()[(int)(long)data!];

    public override object? toPrimitives() => FoundationRuntimePorts.EnumIndex(value);
}

internal class _RestorableAutovalidateMode__date_picker : RestorableValue<AutovalidateMode>
{
    internal virtual AutovalidateMode _defaultValue { get; private set; } = default!;

    internal _RestorableAutovalidateMode__date_picker(AutovalidateMode defaultValue)
    {
        _defaultValue = defaultValue;
    }

    public override AutovalidateMode createDefaultValue() => _defaultValue;

    public override void didUpdateValue(AutovalidateMode oldValue)
    {
        DartRuntimePrimitives.Assert(() =>
            RestorationLibrary.debugIsSerializableForRestoration(
                FoundationRuntimePorts.EnumIndex(value)
            )
        );
        notifyListeners();
    }

    public override AutovalidateMode fromPrimitives(object? data) =>
        Enum.GetValues<AutovalidateMode>().ToList()[(int)(long)data!];

    public override object? toPrimitives() => FoundationRuntimePorts.EnumIndex(value);
}

internal class _DatePickerHeader__date_picker : StatelessWidget
{
    internal const double _datePickerHeaderLandscapeWidth = 152.0;
    internal const double _datePickerHeaderPortraitHeight = 120.0;
    internal const double _headerPaddingLandscape = 16.0;
    public virtual string helpText { get; private set; } = default!;
    public virtual string titleText { get; private set; } = default!;
    public virtual string? titleSemanticsLabel { get; private set; }
    public virtual TextStyle? titleStyle { get; private set; }
    public virtual Orientation orientation { get; private set; } = default!;
    public virtual bool isShort { get; private set; } = default!;
    public virtual Widget? entryModeButton { get; private set; }

    internal _DatePickerHeader__date_picker(
        string helpText,
        string titleText,
        string? titleSemanticsLabel = null,
        TextStyle? titleStyle = default!,
        Orientation orientation = default!,
        bool isShort = false,
        Widget? entryModeButton = null
    )
    {
        this.helpText = helpText;
        this.titleText = titleText;
        this.titleSemanticsLabel = titleSemanticsLabel;
        this.titleStyle = titleStyle;
        this.orientation = orientation;
        this.isShort = isShort;
        this.entryModeButton = entryModeButton;
    }

    public override Widget build(BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        DatePickerThemeData datePickerTheme = DatePickerTheme.of(context);
        DatePickerThemeData defaultsLocal = DatePickerTheme.defaults(context);
        Color? backgroundColor =
            datePickerTheme.headerBackgroundColor ?? defaultsLocal.headerBackgroundColor;
        Color? foregroundColor =
            datePickerTheme.headerForegroundColor ?? defaultsLocal.headerForegroundColor;
        TextStyle? helpStyle = (
            datePickerTheme.headerHelpStyle ?? defaultsLocal.headerHelpStyle
        )?.copyWith(color: foregroundColor);
        double currentScale =
            MediaQuery.textScalerOf(context).scale(Calendar_date_pickerLibrary._fontSizeToScale)
            / Calendar_date_pickerLibrary._fontSizeToScale;
        double maxHeaderTextScaleFactor = Math.Min(
            currentScale,
            (entryModeButton is not null)
                ? Date_pickerLibrary._kMaxHeaderWithEntryTextScaleFactor
                : Date_pickerLibrary._kMaxHeaderTextScaleFactor
        );
        double textScaleFactor =
            MediaQuery
                .textScalerOf(context)
                .clamp(maxScaleFactor: maxHeaderTextScaleFactor)
                .scale(Calendar_date_pickerLibrary._fontSizeToScale)
            / Calendar_date_pickerLibrary._fontSizeToScale;
        double scaledFontSize = MediaQuery.textScalerOf(context).scale(titleStyle?.fontSize ?? 32);
        var headerScaleFactor = (textScaleFactor > 1L) ? textScaleFactor : 1.0;
        var help = new Text(
            helpText,
            style: helpStyle,
            maxLines: 1L,
            overflow: TextOverflow.ellipsis,
            textScaler: MediaQuery
                .textScalerOf(context)
                .clamp(
                    maxScaleFactor: Math.Min(
                        textScaleFactor,
                        Equals(orientation, Orientation.portrait)
                            ? Date_pickerLibrary._kMaxHelpPortraitTextScaleFactor
                            : Date_pickerLibrary._kMaxHelpLandscapeTextScaleFactor
                    )
                )
        );
        var title = new Text(
            titleText,
            semanticsLabel: titleSemanticsLabel ?? titleText,
            style: titleStyle,
            maxLines: Equals(orientation, Orientation.portrait)
                ? ((scaledFontSize > 70L) ? 2L : 1L)
                : ((scaledFontSize > 40L) ? 3L : 2L),
            overflow: TextOverflow.ellipsis,
            textScaler: MediaQuery.textScalerOf(context).clamp(maxScaleFactor: textScaleFactor)
        );
        double fontScaleAdjustedHeaderHeight =
            (headerScaleFactor > 1.3) ? (headerScaleFactor - 0.2) : 1.0;
        switch (orientation)
        {
            case Orientation.portrait:
            {
                return new Widgets.Semantics(
                    container: true,
                    child: new SizedBox(
                        height: _datePickerHeaderPortraitHeight * fontScaleAdjustedHeaderHeight,
                        child: new Material(
                            color: backgroundColor,
                            child: new Padding(
                                padding: EdgeInsetsDirectional.CreateOnly(
                                    start: 24,
                                    end: 12,
                                    bottom: 12
                                ),
                                child: new Column(
                                    crossAxisAlignment: CrossAxisAlignment.start,
                                    children: new List<Widget>
                                    {
                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                            new SizedBox(height: 16)
                                        ),
                                        DartRuntimePrimitives.ConvertValue<Widget>(help),
                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                            new Flexible(child: new SizedBox(height: 38))
                                        ),
                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                            new Row(
                                                children: (
                                                    (Func<List<Widget>>)(
                                                        () =>
                                                        {
                                                            var __collection38913 =
                                                                new List<Widget>();
                                                            __collection38913.Add(
                                                                DartRuntimePrimitives.ConvertValue<Widget>(
                                                                    new Expanded(child: title)
                                                                )
                                                            );
                                                            if (entryModeButton is not null)
                                                            {
                                                                __collection38913.Add(
                                                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                                                        new Widgets.Semantics(
                                                                            container: true,
                                                                            child: entryModeButton
                                                                        )
                                                                    )
                                                                );
                                                            }
                                                            return __collection38913;
                                                        }
                                                    )
                                                )()
                                            )
                                        ),
                                    }
                                )
                            )
                        )
                    )
                );
            }
            case Orientation.landscape:
            {
                return new Widgets.Semantics(
                    container: true,
                    child: new SizedBox(
                        width: _datePickerHeaderLandscapeWidth,
                        child: new Material(
                            color: backgroundColor,
                            child: new Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: (
                                    (Func<List<Widget>>)(
                                        () =>
                                        {
                                            var __collection39596 = new List<Widget>();
                                            __collection39596.Add(
                                                DartRuntimePrimitives.ConvertValue<Widget>(
                                                    new SizedBox(height: 16)
                                                )
                                            );
                                            __collection39596.Add(
                                                DartRuntimePrimitives.ConvertValue<Widget>(
                                                    new Padding(
                                                        padding: EdgeInsets.CreateSymmetric(
                                                            horizontal: _headerPaddingLandscape
                                                        ),
                                                        child: help
                                                    )
                                                )
                                            );
                                            __collection39596.Add(
                                                DartRuntimePrimitives.ConvertValue<Widget>(
                                                    new SizedBox(height: isShort ? 16 : 56)
                                                )
                                            );
                                            __collection39596.Add(
                                                DartRuntimePrimitives.ConvertValue<Widget>(
                                                    new Expanded(
                                                        child: new Padding(
                                                            padding: EdgeInsets.CreateSymmetric(
                                                                horizontal: _headerPaddingLandscape
                                                            ),
                                                            child: title
                                                        )
                                                    )
                                                )
                                            );
                                            if (entryModeButton is not null)
                                            {
                                                __collection39596.Add(
                                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                                        new Padding(
                                                            padding: EdgeInsetsDirectional.CreateOnly(
                                                                start: 8.0,
                                                                end: 4.0,
                                                                bottom: 6.0
                                                            ),
                                                            child: new Widgets.Semantics(
                                                                container: true,
                                                                child: entryModeButton
                                                            )
                                                        )
                                                    )
                                                );
                                            }
                                            return __collection39596;
                                        }
                                    )
                                )()
                            )
                        )
                    )
                );
            }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public delegate bool SelectableDayForRangePredicate(
    DateTime day,
    DateTime? selectedStartDay,
    DateTime? selectedEndDay
);

public static partial class Date_pickerLibrary
{
    public static async Future<DateTimeRange<DateTime>?> showDateRangePicker(
        BuildContext context,
        DateTimeRange<DateTime>? initialDateRange = null,
        DateTime firstDate = default!,
        DateTime lastDate = default!,
        DateTime? currentDate = null,
        DatePickerEntryMode initialEntryMode = DatePickerEntryMode.calendar,
        string? helpText = null,
        string? cancelText = null,
        string? confirmText = null,
        string? saveText = null,
        string? errorFormatText = null,
        string? errorInvalidText = null,
        string? errorInvalidRangeText = null,
        string? fieldStartHintText = null,
        string? fieldEndHintText = null,
        string? fieldStartLabelText = null,
        string? fieldEndLabelText = null,
        Locale? locale = null,
        bool barrierDismissible = true,
        Color? barrierColor = null,
        string? barrierLabel = null,
        bool useRootNavigator = true,
        RouteSettings? routeSettings = null,
        TextDirection? textDirection = null,
        Func<BuildContext, Widget?, Widget>? builder = null,
        Offset? anchorPoint = null,
        TextInputType keyboardType = default!,
        Icon? switchToInputEntryModeIcon = null,
        Icon? switchToCalendarEntryModeIcon = null,
        Func<DateTime, DateTime?, DateTime?, bool>? selectableDayPredicate = null,
        CalendarDelegate<DateTime> calendarDelegate = default!
    )
    {
        initialDateRange =
            (initialDateRange is null) ? null : calendarDelegate.datesOnly(initialDateRange);
        firstDate = calendarDelegate.dateOnly(firstDate);
        lastDate = calendarDelegate.dateOnly(lastDate);
        DartRuntimePrimitives.Assert(
            () => !lastDate.isBefore(firstDate),
            () => (object?)$"lastDate {lastDate} must be on or after firstDate {firstDate}."
        );
        DartRuntimePrimitives.Assert(
            () => (initialDateRange is null) || !initialDateRange.start.isBefore(firstDate),
            () =>
                (object?)$"initialDateRange's start date must be on or after firstDate {firstDate}."
        );
        DartRuntimePrimitives.Assert(
            () => (initialDateRange is null) || !initialDateRange.end.isBefore(firstDate),
            () => (object?)$"initialDateRange's end date must be on or after firstDate {firstDate}."
        );
        DartRuntimePrimitives.Assert(
            () => (initialDateRange is null) || !initialDateRange.start.isAfter(lastDate),
            () =>
                (object?)$"initialDateRange's start date must be on or before lastDate {lastDate}."
        );
        DartRuntimePrimitives.Assert(
            () => (initialDateRange is null) || !initialDateRange.end.isAfter(lastDate),
            () => (object?)$"initialDateRange's end date must be on or before lastDate {lastDate}."
        );
        DartRuntimePrimitives.Assert(
            () =>
                (initialDateRange is null)
                || (selectableDayPredicate is null)
                || selectableDayPredicate(
                    initialDateRange.start,
                    initialDateRange.start,
                    initialDateRange.end
                ),
            () => (object?)"initialDateRange's start date must be selectable."
        );
        DartRuntimePrimitives.Assert(
            () =>
                (initialDateRange is null)
                || (selectableDayPredicate is null)
                || selectableDayPredicate(
                    initialDateRange.end,
                    initialDateRange.start,
                    initialDateRange.end
                ),
            () => (object?)"initialDateRange's end date must be selectable."
        );
        currentDate = calendarDelegate.dateOnly(currentDate ?? calendarDelegate.now());
        DartRuntimePrimitives.Assert(() =>
            DebugLibrary.debugCheckHasMaterialLocalizations(context)
        );
        Widget dialog = new DateRangePickerDialog(
            initialDateRange: initialDateRange,
            firstDate: firstDate,
            lastDate: lastDate,
            currentDate: DartRuntimePrimitives.RequireValue(currentDate),
            selectableDayPredicate: selectableDayPredicate,
            initialEntryMode: initialEntryMode,
            helpText: helpText,
            cancelText: cancelText,
            confirmText: confirmText,
            saveText: saveText,
            errorFormatText: errorFormatText,
            errorInvalidText: errorInvalidText,
            errorInvalidRangeText: errorInvalidRangeText,
            fieldStartHintText: fieldStartHintText,
            fieldEndHintText: fieldEndHintText,
            fieldStartLabelText: fieldStartLabelText,
            fieldEndLabelText: fieldEndLabelText,
            keyboardType: keyboardType,
            switchToInputEntryModeIcon: switchToInputEntryModeIcon,
            switchToCalendarEntryModeIcon: switchToCalendarEntryModeIcon,
            calendarDelegate: calendarDelegate
        );
        if (textDirection is not null)
        {
            TextDirection textDirection__value49942 = DartRuntimePrimitives.RequireValue(
                textDirection
            );
            dialog = DartRuntimePrimitives.ConvertValue<Widget>(
                new Directionality(
                    textDirection: DartRuntimePrimitives.RequireValue(
                        DartRuntimePrimitives.RequireValue(textDirection__value49942)
                    ),
                    child: dialog
                )
            );
        }
        if (locale is not null)
        {
            Locale locale__value50052 = DartRuntimePrimitives.RequireValue(locale);
            dialog = DartRuntimePrimitives.ConvertValue<Widget>(
                Localizations.CreateOverride(
                    context: context,
                    locale: DartRuntimePrimitives.RequireValue(locale__value50052),
                    child: dialog
                )
            );
        }
        return await DialogLibrary.showDialog<DateTimeRange<DateTime>>(
            context: context,
            barrierDismissible: barrierDismissible,
            barrierColor: barrierColor,
            barrierLabel: barrierLabel,
            useRootNavigator: useRootNavigator,
            routeSettings: routeSettings,
            useSafeArea: false,
            builder: (context) =>
            {
                return (builder is null) ? dialog : builder(context, dialog);
                throw new InvalidOperationException("Dart closure completed without a value.");
            },
            anchorPoint: anchorPoint
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Date_pickerLibrary
{
    internal static string _formatRangeStartDate(
        MaterialLocalizations localizations,
        CalendarDelegate<DateTime> calendarDelegate,
        DateTime? startDate,
        DateTime? endDate
    )
    {
        return (startDate is null)
            ? localizations.dateRangeStartLabel
            : (
                (
                    (endDate is null)
                    || (
                        DartRuntimePrimitives.RequireValue(startDate).Year
                        == DartRuntimePrimitives.RequireValue(endDate).Year
                    )
                )
                    ? calendarDelegate.formatShortMonthDay(
                        DartRuntimePrimitives.RequireValue(
                            DartRuntimePrimitives.RequireValue(startDate)
                        ),
                        localizations
                    )
                    : calendarDelegate.formatShortDate(
                        DartRuntimePrimitives.RequireValue(
                            DartRuntimePrimitives.RequireValue(startDate)
                        ),
                        localizations
                    )
            );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Date_pickerLibrary
{
    internal static string _formatRangeEndDate(
        MaterialLocalizations localizations,
        CalendarDelegate<DateTime> calendarDelegate,
        DateTime? startDate,
        DateTime? endDate,
        DateTime currentDate
    )
    {
        return (endDate is null)
            ? localizations.dateRangeEndLabel
            : (
                (
                    (startDate is not null)
                    && (
                        DartRuntimePrimitives.RequireValue(startDate).Year
                        == DartRuntimePrimitives.RequireValue(endDate).Year
                    )
                    && (DartRuntimePrimitives.RequireValue(startDate).Year == currentDate.Year)
                )
                    ? calendarDelegate.formatShortMonthDay(
                        DartRuntimePrimitives.RequireValue(
                            DartRuntimePrimitives.RequireValue(endDate)
                        ),
                        localizations
                    )
                    : calendarDelegate.formatShortDate(
                        DartRuntimePrimitives.RequireValue(
                            DartRuntimePrimitives.RequireValue(endDate)
                        ),
                        localizations
                    )
            );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class DateRangePickerDialog : StatefulWidget
{
    public virtual DateTimeRange<DateTime>? initialDateRange { get; private set; }
    public virtual DateTime firstDate { get; private set; } = default!;
    public virtual DateTime lastDate { get; private set; } = default!;
    internal virtual DateTime? _currentDate { get; private set; }
    public virtual DatePickerEntryMode initialEntryMode { get; private set; } = default!;
    public virtual string? cancelText { get; private set; }
    public virtual string? confirmText { get; private set; }
    public virtual string? saveText { get; private set; }
    public virtual string? helpText { get; private set; }
    public virtual string? errorInvalidRangeText { get; private set; }
    public virtual string? errorFormatText { get; private set; }
    public virtual string? errorInvalidText { get; private set; }
    public virtual string? fieldStartHintText { get; private set; }
    public virtual string? fieldEndHintText { get; private set; }
    public virtual string? fieldStartLabelText { get; private set; }
    public virtual string? fieldEndLabelText { get; private set; }
    public virtual TextInputType keyboardType { get; private set; } = default!;
    public virtual string? restorationId { get; private set; }
    public virtual Icon? switchToInputEntryModeIcon { get; private set; }
    public virtual Icon? switchToCalendarEntryModeIcon { get; private set; }
    public virtual Func<DateTime, DateTime?, DateTime?, bool>? selectableDayPredicate
    {
        get;
        private set;
    }
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    public DateRangePickerDialog(
        Key? key = null,
        DateTimeRange<DateTime>? initialDateRange = null,
        DateTime firstDate = default!,
        DateTime lastDate = default!,
        DateTime? currentDate = null,
        DatePickerEntryMode initialEntryMode = DatePickerEntryMode.calendar,
        string? helpText = null,
        string? cancelText = null,
        string? confirmText = null,
        string? saveText = null,
        string? errorInvalidRangeText = null,
        string? errorFormatText = null,
        string? errorInvalidText = null,
        string? fieldStartHintText = null,
        string? fieldEndHintText = null,
        string? fieldStartLabelText = null,
        string? fieldEndLabelText = null,
        TextInputType keyboardType = default!,
        string? restorationId = null,
        Icon? switchToInputEntryModeIcon = null,
        Icon? switchToCalendarEntryModeIcon = null,
        Func<DateTime, DateTime?, DateTime?, bool>? selectableDayPredicate = null,
        CalendarDelegate<DateTime> calendarDelegate = default!
    )
        : base(key: key)
    {
        TextInputType __keyboardType = keyboardType ?? TextInputType.datetime;
        CalendarDelegate<DateTime> __calendarDelegate =
            calendarDelegate ?? new GregorianCalendarDelegate();
        this.initialDateRange = initialDateRange;
        this.firstDate = firstDate;
        this.lastDate = lastDate;
        this.initialEntryMode = initialEntryMode;
        this.helpText = helpText;
        this.cancelText = cancelText;
        this.confirmText = confirmText;
        this.saveText = saveText;
        this.errorInvalidRangeText = errorInvalidRangeText;
        this.errorFormatText = errorFormatText;
        this.errorInvalidText = errorInvalidText;
        this.fieldStartHintText = fieldStartHintText;
        this.fieldEndHintText = fieldEndHintText;
        this.fieldStartLabelText = fieldStartLabelText;
        this.fieldEndLabelText = fieldEndLabelText;
        this.keyboardType = __keyboardType;
        this.restorationId = restorationId;
        this.switchToInputEntryModeIcon = switchToInputEntryModeIcon;
        this.switchToCalendarEntryModeIcon = switchToCalendarEntryModeIcon;
        this.selectableDayPredicate = selectableDayPredicate;
        this.calendarDelegate = __calendarDelegate;
        _currentDate = currentDate;
    }

    public virtual DateTime currentDate
    {
        get { return calendarDelegate.dateOnly(_currentDate ?? calendarDelegate.now()); }
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _DateRangePickerDialogState__date_picker());
}

internal class _DateRangePickerDialogState__date_picker
    : State<DateRangePickerDialog>,
        RestorationMixin<DateRangePickerDialog>
{
    private bool __late__entryMode_initialized;
    private _RestorableDatePickerEntryMode__date_picker __late__entryMode = default!;
    internal virtual _RestorableDatePickerEntryMode__date_picker _entryMode
    {
        get
        {
            if (!__late__entryMode_initialized)
            {
                __late__entryMode = new _RestorableDatePickerEntryMode__date_picker(
                    widget.initialEntryMode
                );
                __late__entryMode_initialized = true;
            }
            return __late__entryMode;
        }
    }
    private bool __late__selectedStart_initialized;
    private RestorableDateTimeN __late__selectedStart = default!;
    internal virtual RestorableDateTimeN _selectedStart
    {
        get
        {
            if (!__late__selectedStart_initialized)
            {
                __late__selectedStart = new RestorableDateTimeN(widget.initialDateRange?.start);
                __late__selectedStart_initialized = true;
            }
            return __late__selectedStart;
        }
    }
    private bool __late__selectedEnd_initialized;
    private RestorableDateTimeN __late__selectedEnd = default!;
    internal virtual RestorableDateTimeN _selectedEnd
    {
        get
        {
            if (!__late__selectedEnd_initialized)
            {
                __late__selectedEnd = new RestorableDateTimeN(widget.initialDateRange?.end);
                __late__selectedEnd_initialized = true;
            }
            return __late__selectedEnd;
        }
    }
    internal virtual RestorableBool _autoValidate { get; private set; } = new RestorableBool(false);
    internal virtual GlobalKey<IState> _calendarPickerKey { get; private set; } =
        GlobalKey<IState>.Create();
    internal virtual GlobalKey<_InputDateRangePickerState__date_picker> _inputPickerKey
    {
        get;
        private set;
    } = GlobalKey<_InputDateRangePickerState__date_picker>.Create();
    public virtual RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<IRestorableProperty, Action> _properties { get; set; } =
        new DartMap<IRestorableProperty, Action>();
    public virtual List<IRestorableProperty>? _debugPropertiesWaitingForReregistration { get; set; } =
        default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual RestorationBucket? _currentParent { get; set; } = default;

    public virtual string? restorationId => widget.restorationId;

    public virtual void restoreState(RestorationBucket? oldBucket, bool initialRestore)
    {
        registerForRestoration(_entryMode, "entry_mode");
        registerForRestoration(_selectedStart, "selected_start");
        registerForRestoration(_selectedEnd, "selected_end");
        registerForRestoration(_autoValidate, "autovalidate");
    }

    public override void dispose()
    {
        _entryMode.dispose();
        _selectedStart.dispose();
        _selectedEnd.dispose();
        _autoValidate.dispose();
        _properties.forEach(
            (property, listener) =>
            {
                if (!property._disposed)
                {
                    property.removeListener(listener);
                }
            }
        );
        _bucket?.dispose();
        _bucket = null;
        base.dispose();
    }

    internal virtual void _handleOk()
    {
        if (
            Equals(_entryMode.value, DatePickerEntryMode.input)
            || Equals(_entryMode.value, DatePickerEntryMode.inputOnly)
        )
        {
            _InputDateRangePickerState__date_picker picker = _inputPickerKey.currentState!;
            if (!picker.validate())
            {
                setState(() =>
                {
                    _autoValidate.value = true;
                });
                return;
            }
        }
        DateTimeRange<DateTime>? selectedRange = _hasSelectedDateRange
            ? new DateTimeRange<DateTime>(
                start: DartRuntimePrimitives.RequireValue(_selectedStart.value),
                end: DartRuntimePrimitives.RequireValue(_selectedEnd.value)
            )
            : null;
        Navigator.pop<object>(context, selectedRange);
    }

    internal virtual void _handleCancel()
    {
        Navigator.pop<object>(context);
    }

    internal virtual void _handleEntryModeToggle()
    {
        setState(() =>
        {
            switch (_entryMode.value)
            {
                case DatePickerEntryMode.calendar:
                {
                    _autoValidate.value = false;
                    _entryMode.value = DatePickerEntryMode.input;
                    break;
                }
                case DatePickerEntryMode.input:
                {
                    if (
                        (_selectedStart.value is not null)
                        && (_selectedEnd.value is not null)
                        && DartRuntimePrimitives
                            .RequireValue(_selectedStart.value)
                            .isAfter(DartRuntimePrimitives.RequireValue(_selectedEnd.value))
                    )
                    {
                        _selectedEnd.value = null;
                    }
                    if (
                        (_selectedStart.value is not null)
                        && !_isDaySelectable(
                            DartRuntimePrimitives.RequireValue(_selectedStart.value)
                        )
                    )
                    {
                        _selectedStart.value = null;
                        _selectedEnd.value = null;
                    }
                    else
                    {
                        if (
                            (_selectedEnd.value is not null)
                            && !_isDaySelectable(
                                DartRuntimePrimitives.RequireValue(_selectedEnd.value)
                            )
                        )
                        {
                            _selectedEnd.value = null;
                        }
                    }
                    _entryMode.value = DatePickerEntryMode.calendar;
                    break;
                }
                case DatePickerEntryMode.calendarOnly:
                case DatePickerEntryMode.inputOnly:
                {
                    DartRuntimePrimitives.Assert(
                        () => false,
                        () => (object?)$"Can not change entry mode from {_entryMode}"
                    );
                    break;
                }
            }
        });
    }

    internal virtual bool _isDaySelectable(DateTime day)
    {
        if (day.isBefore(widget.firstDate) || day.isAfter(widget.lastDate))
        {
            return false;
        }
        if (widget.selectableDayPredicate is null)
        {
            return true;
        }
        return widget.selectableDayPredicate!(day, _selectedStart.value, _selectedEnd.value);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleStartDateChanged(DateTime? date)
    {
        setState(() =>
        {
            _ = _selectedStart.value = date;
        });
    }

    internal virtual void _handleEndDateChanged(DateTime? date)
    {
        setState(() =>
        {
            _ = _selectedEnd.value = date;
        });
    }

    internal virtual bool _hasSelectedDateRange =>
        DartRuntimePrimitives.ConvertValue<bool>(
            (_selectedStart.value is not null) && (_selectedEnd.value is not null)
        );

    public override Widget build(BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        Orientation orientation = MediaQuery.orientationOf(context);
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        DatePickerThemeData datePickerTheme = DatePickerTheme.of(context);
        DatePickerThemeData defaultsLocal = DatePickerTheme.defaults(context);
        Widget contents = default!;
        Size size = default!;
        double? elevationLocal = default!;
        Color? shadowColorLocal = default!;
        Color? surfaceTintColorLocal = default!;
        ShapeBorder? shapeLocal = default!;
        EdgeInsets insetPaddingLocal = default!;
        bool showEntryModeButton =
            Equals(_entryMode.value, DatePickerEntryMode.calendar)
            || Equals(_entryMode.value, DatePickerEntryMode.input);
        switch (_entryMode.value)
        {
            case DatePickerEntryMode.calendar:
            case DatePickerEntryMode.calendarOnly:
            {
                contents = DartRuntimePrimitives.ConvertValue<Widget>(
                    new _CalendarRangePickerDialog__date_picker(
                        key: _calendarPickerKey,
                        calendarDelegate: widget.calendarDelegate,
                        selectedStartDate: _selectedStart.value,
                        selectedEndDate: _selectedEnd.value,
                        firstDate: widget.firstDate,
                        lastDate: widget.lastDate,
                        selectableDayPredicate: widget.selectableDayPredicate,
                        currentDate: widget.currentDate,
                        onStartDateChanged: (__arg0) =>
                            ((Action<DateTime?>)_handleStartDateChanged)(
                                DartRuntimePrimitives.ConvertValue<DateTime>(__arg0)
                            ),
                        onEndDateChanged: _handleEndDateChanged,
                        onConfirm: _hasSelectedDateRange ? _handleOk : null,
                        onCancel: () => _handleCancel(),
                        entryModeButton: showEntryModeButton
                            ? new IconButton(
                                icon: widget.switchToInputEntryModeIcon
                                    ?? new Icon(Icons.edit_outlined),
                                padding: EdgeInsets.zero,
                                tooltip: localizations.inputDateModeButtonLabel,
                                onPressed: _handleEntryModeToggle
                            )
                            : null,
                        confirmText: widget.saveText ?? localizations.saveButtonLabel,
                        helpText: widget.helpText ?? localizations.dateRangePickerHelpText
                    )
                );
                size = MediaQuery.sizeOf(context);
                insetPaddingLocal = EdgeInsets.zero;
                elevationLocal =
                    datePickerTheme.rangePickerElevation
                    ?? DartRuntimePrimitives.RequireValue(defaultsLocal.rangePickerElevation);
                shadowColorLocal =
                    datePickerTheme.rangePickerShadowColor ?? defaultsLocal.rangePickerShadowColor!;
                surfaceTintColorLocal =
                    datePickerTheme.rangePickerSurfaceTintColor
                    ?? defaultsLocal.rangePickerSurfaceTintColor!;
                shapeLocal = datePickerTheme.rangePickerShape ?? defaultsLocal.rangePickerShape;
                break;
            }
            case DatePickerEntryMode.input:
            case DatePickerEntryMode.inputOnly:
            {
                contents = DartRuntimePrimitives.ConvertValue<Widget>(
                    new _InputDateRangePickerDialog__date_picker(
                        calendarDelegate: widget.calendarDelegate,
                        selectedStartDate: _selectedStart.value,
                        selectedEndDate: _selectedEnd.value,
                        currentDate: widget.currentDate,
                        picker: new SizedBox(
                            height: Equals(orientation, Orientation.portrait)
                                ? Date_pickerLibrary._inputFormPortraitHeight
                                : Date_pickerLibrary._inputFormLandscapeHeight,
                            child: new Padding(
                                padding: EdgeInsets.CreateSymmetric(horizontal: 24),
                                child: new Column(
                                    children: new List<Widget>
                                    {
                                        DartRuntimePrimitives.ConvertValue<Widget>(new Spacer()),
                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                            new _InputDateRangePicker__date_picker(
                                                key: _inputPickerKey,
                                                calendarDelegate: widget.calendarDelegate,
                                                initialStartDate: _selectedStart.value,
                                                initialEndDate: _selectedEnd.value,
                                                firstDate: widget.firstDate,
                                                lastDate: widget.lastDate,
                                                selectableDayPredicate: widget.selectableDayPredicate,
                                                onStartDateChanged: _handleStartDateChanged,
                                                onEndDateChanged: _handleEndDateChanged,
                                                autofocus: true,
                                                autovalidate: DartRuntimePrimitives.RequireValue(
                                                    _autoValidate.value
                                                ),
                                                helpText: widget.helpText,
                                                errorInvalidRangeText: widget.errorInvalidRangeText,
                                                errorFormatText: widget.errorFormatText,
                                                errorInvalidText: widget.errorInvalidText,
                                                fieldStartHintText: widget.fieldStartHintText,
                                                fieldEndHintText: widget.fieldEndHintText,
                                                fieldStartLabelText: widget.fieldStartLabelText,
                                                fieldEndLabelText: widget.fieldEndLabelText,
                                                keyboardType: widget.keyboardType
                                            )
                                        ),
                                        DartRuntimePrimitives.ConvertValue<Widget>(new Spacer()),
                                    }
                                )
                            )
                        ),
                        onConfirm: () => _handleOk(),
                        onCancel: () => _handleCancel(),
                        entryModeButton: showEntryModeButton
                            ? new IconButton(
                                icon: widget.switchToCalendarEntryModeIcon
                                    ?? new Icon(Icons.calendar_today),
                                padding: EdgeInsets.zero,
                                tooltip: localizations.calendarModeButtonLabel,
                                onPressed: _handleEntryModeToggle
                            )
                            : null,
                        confirmText: widget.confirmText ?? localizations.okButtonLabel,
                        cancelText: widget.cancelText ?? localizations.cancelButtonLabel,
                        helpText: widget.helpText ?? localizations.dateRangePickerHelpText
                    )
                );
                DialogThemeData dialogThemeLocal = theme.dialogTheme;
                size = Equals(orientation, Orientation.portrait)
                    ? Date_pickerLibrary._inputPortraitDialogSizeM3
                    : Date_pickerLibrary._inputRangeLandscapeDialogSize;
                elevationLocal =
                    datePickerTheme.elevation
                    ?? DartRuntimePrimitives.RequireValue(defaultsLocal.elevation);
                shadowColorLocal = datePickerTheme.shadowColor ?? defaultsLocal.shadowColor;
                surfaceTintColorLocal =
                    datePickerTheme.surfaceTintColor ?? defaultsLocal.surfaceTintColor;
                shapeLocal = datePickerTheme.shape ?? defaultsLocal.shape;
                insetPaddingLocal = EdgeInsets.CreateSymmetric(horizontal: 16.0, vertical: 24.0);
                break;
            }
        }
        return new Dialog(
            insetPadding: insetPaddingLocal,
            backgroundColor: datePickerTheme.backgroundColor ?? defaultsLocal.backgroundColor,
            elevation: elevationLocal,
            shadowColor: shadowColorLocal,
            surfaceTintColor: surfaceTintColorLocal,
            shape: shapeLocal,
            clipBehavior: Clip.antiAlias,
            child: new AnimatedContainer(
                width: size.width,
                height: size.height,
                duration: Date_pickerLibrary._dialogSizeAnimationDuration,
                curve: Curves.easeIn,
                child: MediaQuery.withClampedTextScaling(
                    maxScaleFactor: Date_pickerLibrary._kMaxRangeTextScaleFactor,
                    child: new Builder(
                        builder: (context) =>
                        {
                            return contents;
                            throw new InvalidOperationException(
                                "Dart closure completed without a value."
                            );
                        }
                    )
                )
            )
        );
    }

    public virtual RestorationBucket? bucket => _bucket;

    public virtual void didToggleBucket(RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => _bucket?.isReplacing != true);
    }

    public virtual void registerForRestoration(IRestorableProperty property, string restorationId)
    {
        DartRuntimePrimitives.Assert(
            () =>
                (property._restorationId is null)
                || (_debugDoingRestore && (property._restorationId == restorationId)),
            () => (object?)$"Property is already registered under {property._restorationId}."
        );
        DartRuntimePrimitives.Assert(
            () =>
                _debugDoingRestore
                || !_properties.Keys.map((r) => r._restorationId).contains(restorationId),
            () => (object?)$"\"{restorationId}\" is already registered to another property."
        );
        bool hasSerializedValue = bucket?.contains(restorationId) ?? false;
        object? initialValue = hasSerializedValue
            ? property.fromPrimitivesObject(bucket!.read<object>(restorationId))
            : property.createDefaultValueObject();
        if (!property.isRegistered)
        {
            property._register(restorationId, this);
            void listener()
            {
                if (bucket is null)
                {
                    return;
                }
                _updateProperty(property);
            }
            property.addListener(listener);
            _properties[property] = listener;
        }
        DartRuntimePrimitives.Assert(() =>
            (property._restorationId == restorationId)
            && Equals(property._owner, this)
            && _properties.ContainsKey(property)
        );
        property.initWithValueObject(initialValue);
        if (!hasSerializedValue && property.enabled && (bucket is not null))
        {
            _updateProperty(property);
        }
        DartRuntimePrimitives.Assert(() =>
        {
            _debugPropertiesWaitingForReregistration?.Remove(property);
            return true;
        });
    }

    public virtual void unregisterFromRestoration(IRestorableProperty property)
    {
        DartRuntimePrimitives.Assert(() => Equals(property._owner, this));
        _bucket?.remove<object?>(property._restorationId!);
        _unregister(property);
    }

    public virtual void didUpdateRestorationId()
    {
        if ((_currentParent is null) || (_bucket?.restorationId == restorationId) || restorePending)
        {
            return;
        }
        RestorationBucket? oldBucket = _bucket;
        DartRuntimePrimitives.Assert(() => !restorePending);
        bool didReplaceBucket = _updateBucketIfNecessary(
            parent: _currentParent,
            restorePending: false
        );
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => !Equals(oldBucket, _bucket));
            DartRuntimePrimitives.Assert(() => (_bucket is null) || (oldBucket is null));
            oldBucket?.dispose();
        }
    }

    public override void didUpdateWidget(DateRangePickerDialog oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        didUpdateRestorationId();
    }

    public virtual bool restorePending
    {
        get
        {
            if (_firstRestorePending)
            {
                return true;
            }
            if (restorationId is null)
            {
                return false;
            }
            RestorationBucket? potentialNewParent = RestorationScope.maybeOf(context);
            return (!Equals(potentialNewParent, _currentParent))
                && (potentialNewParent?.isReplacing ?? false);
        }
    }
    public virtual bool _debugDoingRestore =>
        DartRuntimePrimitives.ConvertValue<bool>(
            _debugPropertiesWaitingForReregistration is not null
        );

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        RestorationBucket? oldBucket = _bucket;
        bool needsRestore = restorePending;
        _currentParent = RestorationScope.maybeOf(context);
        bool didReplaceBucket = _updateBucketIfNecessary(
            parent: _currentParent,
            restorePending: needsRestore
        );
        if (needsRestore)
        {
            _doRestore(oldBucket);
        }
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => !Equals(oldBucket, _bucket));
            oldBucket?.dispose();
        }
    }

    public virtual void _doRestore(RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            _debugPropertiesWaitingForReregistration = _properties.Keys.ToList();
            return true;
        });
        restoreState(oldBucket, _firstRestorePending);
        _firstRestorePending = false;
        DartRuntimePrimitives.Assert(() =>
        {
            if (Enumerable.Any(_debugPropertiesWaitingForReregistration!))
            {
                throw DartRuntimePrimitives.AsException(
                    new FlutterError(
                        (
                            (Func<List<DiagnosticsNode>>)(
                                () =>
                                {
                                    var __collection41817 = new List<DiagnosticsNode>();
                                    __collection41817.Add(
                                        new ErrorSummary(
                                            "Previously registered RestorableProperties must be re-registered in \"restoreState\"."
                                        )
                                    );
                                    __collection41817.Add(
                                        new ErrorDescription(
                                            $"The RestorableProperties with the following IDs were not re-registered to {this} when "
                                                + "\"restoreState\" was called:"
                                        )
                                    );
                                    __collection41817.AddRange(
                                        _debugPropertiesWaitingForReregistration!.map<
                                            IRestorableProperty,
                                            DiagnosticsNode
                                        >(
                                            (property) =>
                                                new ErrorDescription(
                                                    $" * {property._restorationId}"
                                                )
                                        )
                                    );
                                    return __collection41817;
                                }
                            )
                        )()
                    )
                );
            }
            _debugPropertiesWaitingForReregistration = null;
            return true;
        });
    }

    public virtual bool _updateBucketIfNecessary(RestorationBucket? parent, bool restorePending)
    {
        if ((restorationId is null) || (parent is null))
        {
            bool didReplace = _setNewBucketIfNecessary(
                newBucket: null,
                restorePending: restorePending
            );
            DartRuntimePrimitives.Assert(() => _bucket is null);
            return didReplace;
        }
        DartRuntimePrimitives.Assert(() => restorationId is not null);
        if (restorePending || (_bucket is null))
        {
            RestorationBucket newBucketLocal = parent.claimChild(restorationId!, debugOwner: this);
            bool didReplaceLocal = _setNewBucketIfNecessary(
                newBucket: newBucketLocal,
                restorePending: restorePending
            );
            DartRuntimePrimitives.Assert(() => Equals(_bucket, newBucketLocal));
            return didReplaceLocal;
        }
        DartRuntimePrimitives.Assert(() => _bucket is not null);
        DartRuntimePrimitives.Assert(() => !restorePending);
        _bucket!.rename(restorationId!);
        parent.adoptChild(_bucket!);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _setNewBucketIfNecessary(RestorationBucket? newBucket, bool restorePending)
    {
        if (Equals(newBucket, _bucket))
        {
            return false;
        }
        RestorationBucket? oldBucket = _bucket;
        _bucket = newBucket;
        if (!restorePending)
        {
            if (_bucket is not null)
            {
                _properties.Keys.forEach(
                    (__arg0) => ((Action<IRestorableProperty>)_updateProperty)(__arg0)
                );
            }
            didToggleBucket(oldBucket);
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _updateProperty(IRestorableProperty property)
    {
        if (property.enabled)
        {
            _bucket?.write(property._restorationId!, property.toPrimitives());
        }
        else
        {
            _bucket?.remove<object>(property._restorationId!);
        }
    }

    public virtual void _unregister(IRestorableProperty property)
    {
        Action listener = _properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
        {
            _debugPropertiesWaitingForReregistration?.Remove(property);
            return true;
        });
        property.removeListener(listener);
        property._unregister();
    }
}

internal class _CalendarRangePickerDialog__date_picker : StatelessWidget
{
    public virtual DateTime? selectedStartDate { get; private set; }
    public virtual DateTime? selectedEndDate { get; private set; }
    public virtual DateTime firstDate { get; private set; } = default!;
    public virtual DateTime lastDate { get; private set; } = default!;
    public virtual Func<DateTime, DateTime?, DateTime?, bool>? selectableDayPredicate
    {
        get;
        private set;
    }
    public virtual DateTime? currentDate { get; private set; }
    public virtual Action<DateTime> onStartDateChanged { get; private set; } = default!;
    public virtual Action<DateTime?> onEndDateChanged { get; private set; } = default!;
    public virtual Action? onConfirm { get; private set; }
    public virtual Action? onCancel { get; private set; }
    public virtual string confirmText { get; private set; } = default!;
    public virtual string helpText { get; private set; } = default!;
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;
    public virtual Widget? entryModeButton { get; private set; }

    internal _CalendarRangePickerDialog__date_picker(
        Key? key = null,
        DateTime? selectedStartDate = default!,
        DateTime? selectedEndDate = default!,
        DateTime firstDate = default!,
        DateTime lastDate = default!,
        DateTime? currentDate = default!,
        Action<DateTime> onStartDateChanged = default!,
        Action<DateTime?> onEndDateChanged = default!,
        Action? onConfirm = default!,
        Action? onCancel = default!,
        string confirmText = default!,
        string helpText = default!,
        Func<DateTime, DateTime?, DateTime?, bool>? selectableDayPredicate = default!,
        CalendarDelegate<DateTime> calendarDelegate = default!,
        Widget? entryModeButton = null
    )
        : base(key: key)
    {
        this.selectedStartDate = selectedStartDate;
        this.selectedEndDate = selectedEndDate;
        this.firstDate = firstDate;
        this.lastDate = lastDate;
        this.currentDate = currentDate;
        this.onStartDateChanged = onStartDateChanged;
        this.onEndDateChanged = onEndDateChanged;
        this.onConfirm = onConfirm;
        this.onCancel = onCancel;
        this.confirmText = confirmText;
        this.helpText = helpText;
        this.selectableDayPredicate = selectableDayPredicate;
        this.calendarDelegate = calendarDelegate;
        this.entryModeButton = entryModeButton;
    }

    public override Widget build(BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        Orientation orientation = MediaQuery.orientationOf(context);
        DatePickerThemeData themeData = DatePickerTheme.of(context);
        DatePickerThemeData defaultsLocal = DatePickerTheme.defaults(context);
        Color? dialogBackground =
            themeData.rangePickerBackgroundColor ?? defaultsLocal.rangePickerBackgroundColor;
        Color? headerBackground =
            themeData.rangePickerHeaderBackgroundColor
            ?? defaultsLocal.rangePickerHeaderBackgroundColor;
        Color? headerForeground =
            themeData.rangePickerHeaderForegroundColor
            ?? defaultsLocal.rangePickerHeaderForegroundColor;
        Color? headerDisabledForeground = headerForeground?.withOpacity(0.38);
        TextStyle? headlineStyle =
            themeData.rangePickerHeaderHeadlineStyle
            ?? defaultsLocal.rangePickerHeaderHeadlineStyle;
        TextStyle? headlineHelpStyle = (
            themeData.rangePickerHeaderHelpStyle ?? defaultsLocal.rangePickerHeaderHelpStyle
        )?.apply(color: headerForeground);
        string startDateText = Date_pickerLibrary._formatRangeStartDate(
            localizations,
            calendarDelegate,
            selectedStartDate,
            selectedEndDate
        );
        string endDateText = Date_pickerLibrary._formatRangeEndDate(
            localizations,
            calendarDelegate,
            selectedStartDate,
            selectedEndDate,
            calendarDelegate.now()
        );
        TextStyle? startDateStyle = headlineStyle?.apply(
            color: (selectedStartDate is not null) ? headerForeground : headerDisabledForeground
        );
        TextStyle? endDateStyle = headlineStyle?.apply(
            color: (selectedEndDate is not null) ? headerForeground : headerDisabledForeground
        );
        ButtonStyle buttonStyle = TextButton.styleFrom(
            foregroundColor: headerForeground,
            disabledForegroundColor: headerDisabledForeground
        );
        var iconThemeLocal = new IconThemeData(color: headerForeground);
        return new SafeArea(
            top: false,
            left: false,
            right: false,
            child: new Scaffold(
                appBar: new AppBar(
                    iconTheme: iconThemeLocal,
                    actionsIconTheme: iconThemeLocal,
                    elevation: 0,
                    scrolledUnderElevation: 0,
                    backgroundColor: headerBackground,
                    leading: new CloseButton(onPressed: onCancel),
                    actions: (
                        (Func<List<Widget>>)(
                            () =>
                            {
                                var __collection72957 = new List<Widget>();
                                if (
                                    Equals(orientation, Orientation.landscape)
                                    && (entryModeButton is not null)
                                )
                                {
                                    __collection72957.Add(
                                        DartRuntimePrimitives.ConvertValue<Widget>(entryModeButton!)
                                    );
                                }
                                __collection72957.Add(
                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                        new TextButton(
                                            style: buttonStyle,
                                            onPressed: onConfirm,
                                            child: new Text(confirmText)
                                        )
                                    )
                                );
                                __collection72957.Add(
                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                        new SizedBox(width: 8)
                                    )
                                );
                                return __collection72957;
                            }
                        )
                    )(),
                    bottom: new PreferredSize(
                        preferredSize: new Size(double.PositiveInfinity, 64),
                        child: new Row(
                            children: (
                                (Func<List<Widget>>)(
                                    () =>
                                    {
                                        var __collection73350 = new List<Widget>();
                                        __collection73350.Add(
                                            DartRuntimePrimitives.ConvertValue<Widget>(
                                                new SizedBox(
                                                    width: (MediaQuery.widthOf(context) < 360L)
                                                        ? 42
                                                        : 72
                                                )
                                            )
                                        );
                                        __collection73350.Add(
                                            DartRuntimePrimitives.ConvertValue<Widget>(
                                                new Expanded(
                                                    child: new Widgets.Semantics(
                                                        label: $"{helpText} {startDateText} to {endDateText}",
                                                        excludeSemantics: true,
                                                        child: new Column(
                                                            crossAxisAlignment: CrossAxisAlignment.start,
                                                            children: new List<Widget>
                                                            {
                                                                DartRuntimePrimitives.ConvertValue<Widget>(
                                                                    new Text(
                                                                        helpText,
                                                                        style: headlineHelpStyle,
                                                                        maxLines: 1L,
                                                                        overflow: TextOverflow.ellipsis
                                                                    )
                                                                ),
                                                                DartRuntimePrimitives.ConvertValue<Widget>(
                                                                    new SizedBox(height: 8)
                                                                ),
                                                                DartRuntimePrimitives.ConvertValue<Widget>(
                                                                    new Row(
                                                                        children: new List<Widget>
                                                                        {
                                                                            DartRuntimePrimitives.ConvertValue<Widget>(
                                                                                new Text(
                                                                                    startDateText,
                                                                                    style: startDateStyle,
                                                                                    maxLines: 1L,
                                                                                    overflow: TextOverflow.ellipsis
                                                                                )
                                                                            ),
                                                                            DartRuntimePrimitives.ConvertValue<Widget>(
                                                                                new Text(
                                                                                    " – ",
                                                                                    style: startDateStyle
                                                                                )
                                                                            ),
                                                                            DartRuntimePrimitives.ConvertValue<Widget>(
                                                                                new Flexible(
                                                                                    child: new Text(
                                                                                        endDateText,
                                                                                        style: endDateStyle,
                                                                                        maxLines: 1L,
                                                                                        overflow: TextOverflow.ellipsis
                                                                                    )
                                                                                )
                                                                            ),
                                                                        }
                                                                    )
                                                                ),
                                                                DartRuntimePrimitives.ConvertValue<Widget>(
                                                                    new SizedBox(height: 16)
                                                                ),
                                                            }
                                                        )
                                                    )
                                                )
                                            )
                                        );
                                        if (
                                            Equals(orientation, Orientation.portrait)
                                            && (entryModeButton is not null)
                                        )
                                        {
                                            __collection73350.Add(
                                                DartRuntimePrimitives.ConvertValue<Widget>(
                                                    new Padding(
                                                        padding: EdgeInsets.CreateSymmetric(
                                                            horizontal: 8.0
                                                        ),
                                                        child: new IconTheme(
                                                            data: iconThemeLocal,
                                                            child: entryModeButton!
                                                        )
                                                    )
                                                )
                                            );
                                        }
                                        return __collection73350;
                                    }
                                )
                            )()
                        )
                    )
                ),
                backgroundColor: dialogBackground,
                body: new _CalendarDateRangePicker__date_picker(
                    initialStartDate: selectedStartDate,
                    initialEndDate: selectedEndDate,
                    firstDate: firstDate,
                    lastDate: lastDate,
                    currentDate: currentDate,
                    onStartDateChanged: onStartDateChanged,
                    onEndDateChanged: onEndDateChanged,
                    selectableDayPredicate: selectableDayPredicate,
                    calendarDelegate: calendarDelegate
                )
            )
        );
    }
}

public static partial class Date_pickerLibrary
{
    internal static Duration _monthScrollDuration = Duration.Create(milliseconds: 200L);
}

public static partial class Date_pickerLibrary
{
    internal static double _monthItemHeaderHeight = 58.0;
}

public static partial class Date_pickerLibrary
{
    internal static double _monthItemFooterHeight = 12.0;
}

public static partial class Date_pickerLibrary
{
    internal static double _monthItemRowHeight = 42.0;
}

public static partial class Date_pickerLibrary
{
    internal static double _monthItemSpaceBetweenRows = 8.0;
}

public static partial class Date_pickerLibrary
{
    internal static double _horizontalPadding = 8.0;
}

public static partial class Date_pickerLibrary
{
    internal static double _maxCalendarWidthLandscape = 384.0;
}

public static partial class Date_pickerLibrary
{
    internal static double _maxCalendarWidthPortrait = 480.0;
}

internal class _CalendarDateRangePicker__date_picker : StatefulWidget
{
    public virtual DateTime? initialStartDate { get; private set; }
    public virtual DateTime? initialEndDate { get; private set; }
    public virtual DateTime firstDate { get; private set; } = default!;
    public virtual DateTime lastDate { get; private set; } = default!;
    public virtual Func<DateTime, DateTime?, DateTime?, bool>? selectableDayPredicate
    {
        get;
        private set;
    }
    public virtual DateTime currentDate { get; private set; } = default!;
    public virtual Action<DateTime>? onStartDateChanged { get; private set; }
    public virtual Action<DateTime?>? onEndDateChanged { get; private set; }
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    internal _CalendarDateRangePicker__date_picker(
        DateTime? initialStartDate = null,
        DateTime? initialEndDate = null,
        DateTime firstDate = default!,
        DateTime lastDate = default!,
        Func<DateTime, DateTime?, DateTime?, bool>? selectableDayPredicate = default!,
        DateTime? currentDate = null,
        Action<DateTime>? onStartDateChanged = default!,
        Action<DateTime?>? onEndDateChanged = default!,
        CalendarDelegate<DateTime> calendarDelegate = default!
    )
    {
        this.selectableDayPredicate = selectableDayPredicate;
        this.onStartDateChanged = onStartDateChanged;
        this.onEndDateChanged = onEndDateChanged;
        this.calendarDelegate = calendarDelegate;
        this.initialStartDate =
            (initialStartDate is not null)
                ? this.calendarDelegate.dateOnly(
                    DartRuntimePrimitives.RequireValue(
                        DartRuntimePrimitives.RequireValue(initialStartDate)
                    )
                )
                : null;
        this.initialEndDate =
            (initialEndDate is not null)
                ? this.calendarDelegate.dateOnly(
                    DartRuntimePrimitives.RequireValue(
                        DartRuntimePrimitives.RequireValue(initialEndDate)
                    )
                )
                : null;
        this.firstDate = this.calendarDelegate.dateOnly(firstDate);
        this.lastDate = this.calendarDelegate.dateOnly(lastDate);
        this.currentDate = this.calendarDelegate.dateOnly(
            currentDate ?? this.calendarDelegate.now()
        );
        DartRuntimePrimitives.Assert(
            () =>
                (this.initialStartDate is null)
                || (this.initialEndDate is null)
                || !DartRuntimePrimitives
                    .RequireValue(this.initialStartDate)
                    .isAfter(DartRuntimePrimitives.RequireValue(initialEndDate)),
            () => (object?)"initialStartDate must be on or before initialEndDate."
        );
        DartRuntimePrimitives.Assert(
            () => !this.lastDate.isBefore(this.firstDate),
            () => (object?)"firstDate must be on or before lastDate."
        );
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _CalendarDateRangePickerState__date_picker()
        );
}

internal class _CalendarDateRangePickerState__date_picker
    : State<_CalendarDateRangePicker__date_picker>
{
    internal virtual GlobalKey<IState> _scrollViewKey { get; private set; } =
        GlobalKey<IState>.Create();
    internal virtual Key _sliverAfterKey { get; private set; } = new UniqueKey();
    internal virtual DateTime? _startDate { get; set; } = default;
    internal virtual DateTime? _endDate { get; set; } = default;
    internal virtual long _initialMonthIndex { get; set; } = 0L;
    internal virtual ScrollController _controller { get; set; } = default!;
    internal virtual bool _showWeekBottomDivider { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        _controller = new ScrollController();
        _controller.addListener(_scrollListener);
        _startDate = widget.initialStartDate;
        _endDate = widget.initialEndDate;
        DateTime initialDate = widget.initialStartDate ?? widget.currentDate;
        if (!initialDate.isBefore(widget.firstDate) && !initialDate.isAfter(widget.lastDate))
        {
            _initialMonthIndex = widget.calendarDelegate.monthDelta(widget.firstDate, initialDate);
        }
        _showWeekBottomDivider = _initialMonthIndex != 0L;
    }

    public override void dispose()
    {
        _controller.dispose();
        base.dispose();
    }

    internal virtual void _scrollListener()
    {
        if (_controller.offset <= _controller.position.minScrollExtent)
        {
            setState(() =>
            {
                _showWeekBottomDivider = false;
            });
        }
        else
        {
            if (!_showWeekBottomDivider)
            {
                setState(() =>
                {
                    _showWeekBottomDivider = true;
                });
            }
        }
    }

    internal virtual long _numberOfMonths =>
        DartRuntimePrimitives.ConvertValue<long>(
            widget.calendarDelegate.monthDelta(widget.firstDate, widget.lastDate) + 1L
        );

    internal virtual void _vibrate()
    {
        switch (Theme.of(context).platform)
        {
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            {
                DartRuntimePrimitives.Ignore(HapticFeedback.vibrate());
                break;
            }
            case TargetPlatform.iOS:
            case TargetPlatform.linux:
            case TargetPlatform.macOS:
            case TargetPlatform.windows:
            {
                break;
            }
        }
    }

    internal virtual void _updateSelection(DateTime date)
    {
        _vibrate();
        setState(() =>
        {
            if (
                (_startDate is not null)
                && (_endDate is null)
                && !date.isBefore(DartRuntimePrimitives.RequireValue(_startDate))
            )
            {
                _endDate = date;
                widget.onEndDateChanged?.Invoke(_endDate);
            }
            else
            {
                _startDate = date;
                widget.onStartDateChanged?.Invoke(DartRuntimePrimitives.RequireValue(_startDate));
                if (_endDate is not null)
                {
                    _endDate = null;
                    widget.onEndDateChanged?.Invoke(_endDate);
                }
            }
        });
    }

    internal virtual Widget _buildMonthItem(
        BuildContext context,
        long index,
        bool beforeInitialMonth
    )
    {
        long monthIndex = beforeInitialMonth
            ? (_initialMonthIndex - index - 1L)
            : (_initialMonthIndex + index);
        DateTime month = widget.calendarDelegate.addMonthsToMonthDate(widget.firstDate, monthIndex);
        return new _MonthItem__date_picker(
            calendarDelegate: widget.calendarDelegate,
            selectedDateStart: _startDate,
            selectedDateEnd: _endDate,
            currentDate: widget.currentDate,
            firstDate: widget.firstDate,
            lastDate: widget.lastDate,
            displayedMonth: month,
            onChanged: _updateSelection,
            selectableDayPredicate: widget.selectableDayPredicate
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        return new Column(
            children: (
                (Func<List<Widget>>)(
                    () =>
                    {
                        var __collection82299 = new List<Widget>();
                        __collection82299.Add(
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                new _DayHeaders__date_picker()
                            )
                        );
                        if (_showWeekBottomDivider)
                        {
                            __collection82299.Add(
                                DartRuntimePrimitives.ConvertValue<Widget>(new Divider(height: 0))
                            );
                        }
                        __collection82299.Add(
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                new Expanded(
                                    child: new _CalendarKeyboardNavigator__date_picker(
                                        calendarDelegate: widget.calendarDelegate,
                                        firstDate: widget.firstDate,
                                        lastDate: widget.lastDate,
                                        initialFocusedDay: (_startDate ?? widget.initialStartDate)
                                            ?? widget.currentDate,
                                        child: new CustomScrollView(
                                            key: _scrollViewKey,
                                            controller: _controller,
                                            center: _sliverAfterKey,
                                            slivers: new List<Widget>
                                            {
                                                DartRuntimePrimitives.ConvertValue<Widget>(
                                                    SliverList.CreateBuilder(
                                                        itemCount: _initialMonthIndex,
                                                        itemBuilder: (context, index) =>
                                                            _buildMonthItem(context, index, true)
                                                    )
                                                ),
                                                DartRuntimePrimitives.ConvertValue<Widget>(
                                                    SliverList.CreateBuilder(
                                                        key: _sliverAfterKey,
                                                        itemCount: _numberOfMonths
                                                            - _initialMonthIndex,
                                                        itemBuilder: (context, index) =>
                                                            _buildMonthItem(context, index, false)
                                                    )
                                                ),
                                            }
                                        )
                                    )
                                )
                            )
                        );
                        return __collection82299;
                    }
                )
            )()
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _CalendarKeyboardNavigator__date_picker : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual DateTime firstDate { get; private set; } = default!;
    public virtual DateTime lastDate { get; private set; } = default!;
    public virtual DateTime initialFocusedDay { get; private set; } = default!;
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    internal _CalendarKeyboardNavigator__date_picker(
        Widget child,
        DateTime firstDate,
        DateTime lastDate,
        DateTime initialFocusedDay,
        CalendarDelegate<DateTime> calendarDelegate
    )
    {
        this.child = child;
        this.firstDate = firstDate;
        this.lastDate = lastDate;
        this.initialFocusedDay = initialFocusedDay;
        this.calendarDelegate = calendarDelegate;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _CalendarKeyboardNavigatorState__date_picker()
        );
}

internal class _CalendarKeyboardNavigatorState__date_picker
    : State<_CalendarKeyboardNavigator__date_picker>
{
    internal virtual DartMap<ShortcutActivator, Intent> _shortcutMap { get; private set; } =
        new DartMap<ShortcutActivator, Intent>
        {
            [new SingleActivator(LogicalKeyboardKey.arrowLeft)] = new DirectionalFocusIntent(
                TraversalDirection.left
            ),
            [new SingleActivator(LogicalKeyboardKey.arrowRight)] = new DirectionalFocusIntent(
                TraversalDirection.right
            ),
            [new SingleActivator(LogicalKeyboardKey.arrowDown)] = new DirectionalFocusIntent(
                TraversalDirection.down
            ),
            [new SingleActivator(LogicalKeyboardKey.arrowUp)] = new DirectionalFocusIntent(
                TraversalDirection.up
            ),
        };
    internal virtual DartMap<Type, dynamic> _actionMap { get; set; } = default!;
    internal virtual FocusNode _dayGridFocus { get; set; } = default!;
    internal virtual TraversalDirection? _dayTraversalDirection { get; set; } = default;
    internal virtual DateTime? _focusedDay { get; set; } = default;
    internal static DartMap<TraversalDirection, long> _directionOffset = new DartMap<
        TraversalDirection,
        long
    >
    {
        [TraversalDirection.up] = -7L,
        [TraversalDirection.right] = 1L,
        [TraversalDirection.down] = 7L,
        [TraversalDirection.left] = -1L,
    };

    public override void initState()
    {
        base.initState();
        _actionMap = new DartMap<Type, dynamic>
        {
            [typeof(NextFocusIntent)] = new CallbackAction<NextFocusIntent>(
                onInvoke: _handleGridNextFocus
            ),
            [typeof(PreviousFocusIntent)] = new CallbackAction<PreviousFocusIntent>(
                onInvoke: _handleGridPreviousFocus
            ),
            [typeof(DirectionalFocusIntent)] = new CallbackAction<DirectionalFocusIntent>(
                onInvoke: _handleDirectionFocus
            ),
        };
        _dayGridFocus = new FocusNode(debugLabel: "Day Grid");
    }

    public override void dispose()
    {
        _dayGridFocus.dispose();
        base.dispose();
    }

    internal virtual void _handleGridFocusChange(bool focused)
    {
        setState(() =>
        {
            if (focused)
            {
                _focusedDay ??= widget.initialFocusedDay;
            }
        });
    }

    internal virtual void _handleGridNextFocus(NextFocusIntent intent)
    {
        _dayGridFocus.requestFocus();
        _dayGridFocus.nextFocus();
    }

    internal virtual void _handleGridPreviousFocus(PreviousFocusIntent intent)
    {
        _dayGridFocus.requestFocus();
        _dayGridFocus.previousFocus();
    }

    internal virtual void _handleDirectionFocus(DirectionalFocusIntent intent)
    {
        DartRuntimePrimitives.Assert(() => _focusedDay is not null);
        setState(() =>
        {
            DateTime? nextDate = _nextDateInDirection(
                DartRuntimePrimitives.RequireValue(_focusedDay),
                intent.direction
            );
            if (nextDate is not null)
            {
                DateTime nextDate__86735__value86810 = DartRuntimePrimitives.RequireValue(nextDate);
                _focusedDay = DartRuntimePrimitives.RequireValue(nextDate__86735__value86810);
                _dayTraversalDirection = intent.direction;
            }
        });
    }

    internal virtual long _dayDirectionOffset(
        TraversalDirection traversalDirection,
        TextDirection textDirection
    )
    {
        if (Equals(textDirection, TextDirection.rtl))
        {
            if (Equals(traversalDirection, TraversalDirection.left))
            {
                traversalDirection = TraversalDirection.right;
            }
            else
            {
                if (Equals(traversalDirection, TraversalDirection.right))
                {
                    traversalDirection = TraversalDirection.left;
                }
            }
        }
        return DartRuntimePrimitives.RequireValue(
            DartCollectionRuntime.NullableMapValue<long>(_directionOffset, traversalDirection)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual DateTime? _nextDateInDirection(DateTime date, TraversalDirection direction)
    {
        TextDirection textDirection = Directionality.of(context);
        DateTime nextDate = widget.calendarDelegate.addDaysToDate(
            date,
            _dayDirectionOffset(direction, textDirection)
        );
        if (!nextDate.isBefore(widget.firstDate) && !nextDate.isAfter(widget.lastDate))
        {
            return nextDate;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        return new FocusableActionDetector(
            shortcuts: _shortcutMap,
            actions: _actionMap,
            focusNode: _dayGridFocus,
            onFocusChange: _handleGridFocusChange,
            child: new _FocusedDate__date_picker(
                calendarDelegate: widget.calendarDelegate,
                date: _dayGridFocus.hasFocus ? _focusedDay : null,
                scrollDirection: _dayGridFocus.hasFocus ? _dayTraversalDirection : null,
                child: widget.child
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _FocusedDate__date_picker : InheritedWidget
{
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;
    public virtual DateTime? date { get; private set; }
    public virtual TraversalDirection? scrollDirection { get; private set; }

    internal _FocusedDate__date_picker(
        Widget child,
        CalendarDelegate<DateTime> calendarDelegate,
        DateTime? date = null,
        TraversalDirection? scrollDirection = null
    )
        : base(child: child)
    {
        this.calendarDelegate = calendarDelegate;
        this.date = date;
        this.scrollDirection = scrollDirection;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (_FocusedDate__date_picker)oldWidget;
        return !calendarDelegate.isSameDay(date, __oldWidget.date)
            || (!Equals(scrollDirection, __oldWidget.scrollDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static _FocusedDate__date_picker? maybeOf(BuildContext context)
    {
        return context.dependOnInheritedWidgetOfExactType<_FocusedDate__date_picker>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _DayHeaders__date_picker : StatelessWidget
{
    internal _DayHeaders__date_picker() { }

    internal virtual List<Widget> _getDayHeaders(
        TextStyle headerStyle,
        MaterialLocalizations localizations
    )
    {
        var result = new List<Widget>();
        for (
            long i = localizations.firstDayOfWeekIndex;
            checked(result.Count) < 7L;
            i = (i + 1L) % 7L
        )
        {
            string weekday = localizations.narrowWeekdays[(int)i];
            result.Add(
                new ExcludeSemantics(
                    child: new Center(child: new Text(weekday, style: headerStyle))
                )
            );
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        ThemeData themeData = Theme.of(context);
        ColorScheme colorSchemeLocal = themeData.colorScheme;
        TextStyle textStyle = themeData.textTheme.titleSmall!.apply(
            color: colorSchemeLocal.onSurface
        );
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        List<Widget> labels = _getDayHeaders(textStyle, localizations);
        labels.Insert(checked((int)0L), SizedBox.CreateShrink());
        labels.Add(SizedBox.CreateShrink());
        return new ConstrainedBox(
            constraints: new BoxConstraints(
                maxWidth: Equals(MediaQuery.orientationOf(context), Orientation.landscape)
                    ? Date_pickerLibrary._maxCalendarWidthLandscape
                    : Date_pickerLibrary._maxCalendarWidthPortrait,
                maxHeight: Date_pickerLibrary._monthItemRowHeight
            ),
            child: GridView.CreateCustom(
                shrinkWrap: true,
                gridDelegate: Date_pickerLibrary._monthItemGridDelegate,
                childrenDelegate: new SliverChildListDelegate(labels, addRepaintBoundaries: false)
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _MonthItemGridDelegate__date_picker : SliverGridDelegate
{
    internal _MonthItemGridDelegate__date_picker() { }

    public virtual SliverGridLayout getLayout(SliverConstraints constraints)
    {
        double tileWidth = Math.Max(
            (constraints.crossAxisExtent - (2L * Date_pickerLibrary._horizontalPadding)) / 7L,
            0.0
        );
        return new _MonthSliverGridLayout__date_picker(
            crossAxisCount: 7L + 2L,
            dayChildWidth: tileWidth,
            edgeChildWidth: Date_pickerLibrary._horizontalPadding,
            reverseCrossAxis: Basic_typesLibrary.axisDirectionIsReversed(
                constraints.crossAxisDirection
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool shouldRelayout(SliverGridDelegate oldDelegate) => false;
}

public static partial class Date_pickerLibrary
{
    internal static _MonthItemGridDelegate__date_picker _monthItemGridDelegate =
        new _MonthItemGridDelegate__date_picker();
}

internal class _MonthSliverGridLayout__date_picker : SliverGridLayout
{
    public virtual long crossAxisCount { get; private set; } = default!;
    public virtual double dayChildWidth { get; private set; } = default!;
    public virtual double edgeChildWidth { get; private set; } = default!;
    public virtual bool reverseCrossAxis { get; private set; } = default!;

    internal _MonthSliverGridLayout__date_picker(
        long crossAxisCount,
        double dayChildWidth,
        double edgeChildWidth,
        bool reverseCrossAxis
    )
    {
        this.crossAxisCount = crossAxisCount;
        this.dayChildWidth = dayChildWidth;
        this.edgeChildWidth = edgeChildWidth;
        this.reverseCrossAxis = reverseCrossAxis;
        System.Diagnostics.Debug.Assert(crossAxisCount > 0L);
        System.Diagnostics.Debug.Assert(dayChildWidth >= 0L);
        System.Diagnostics.Debug.Assert(edgeChildWidth >= 0L);
    }

    internal virtual double _rowHeight
    {
        get
        {
            return Date_pickerLibrary._monthItemRowHeight
                + Date_pickerLibrary._monthItemSpaceBetweenRows;
        }
    }
    internal virtual double _childHeight
    {
        get { return Date_pickerLibrary._monthItemRowHeight; }
    }

    public virtual long getMinChildIndexForScrollOffset(double scrollOffset)
    {
        return crossAxisCount * checked((long)(scrollOffset / _rowHeight));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long getMaxChildIndexForScrollOffset(double scrollOffset)
    {
        long mainAxisCount = (scrollOffset / _rowHeight).ceil();
        return Math.Max(0L, (crossAxisCount * mainAxisCount) - 1L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getCrossAxisOffset(double crossAxisStart, bool isPadding)
    {
        if (reverseCrossAxis)
        {
            return ((crossAxisCount - 2L) * dayChildWidth)
                + (2L * edgeChildWidth)
                - crossAxisStart
                - (isPadding ? edgeChildWidth : dayChildWidth);
        }
        return crossAxisStart;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SliverGridGeometry getGeometryForChildIndex(long index)
    {
        long adjustedIndex = index % crossAxisCount;
        bool isEdge = (adjustedIndex == 0L) || (adjustedIndex == (crossAxisCount - 1L));
        double crossAxisStart = Math.Max(
            0,
            ((adjustedIndex - 1L) * dayChildWidth) + edgeChildWidth
        );
        return new SliverGridGeometry(
            scrollOffset: checked(index / crossAxisCount) * _rowHeight,
            crossAxisOffset: _getCrossAxisOffset(crossAxisStart, isEdge),
            mainAxisExtent: _childHeight,
            crossAxisExtent: isEdge ? edgeChildWidth : dayChildWidth
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double computeMaxScrollOffset(long childCount)
    {
        DartRuntimePrimitives.Assert(() => childCount >= 0L);
        long mainAxisCount = checked((childCount - 1L) / crossAxisCount) + 1L;
        double mainAxisSpacing = _rowHeight - _childHeight;
        return (_rowHeight * mainAxisCount) - mainAxisSpacing;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _MonthItem__date_picker : StatefulWidget
{
    public virtual DateTime? selectedDateStart { get; private set; }
    public virtual DateTime? selectedDateEnd { get; private set; }
    public virtual DateTime currentDate { get; private set; } = default!;
    public virtual Action<DateTime> onChanged { get; private set; } = default!;
    public virtual DateTime firstDate { get; private set; } = default!;
    public virtual DateTime lastDate { get; private set; } = default!;
    public virtual DateTime displayedMonth { get; private set; } = default!;
    public virtual Func<DateTime, DateTime?, DateTime?, bool>? selectableDayPredicate
    {
        get;
        private set;
    }
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    internal _MonthItem__date_picker(
        DateTime? selectedDateStart,
        DateTime? selectedDateEnd,
        DateTime currentDate,
        Action<DateTime> onChanged,
        DateTime firstDate,
        DateTime lastDate,
        DateTime displayedMonth,
        Func<DateTime, DateTime?, DateTime?, bool>? selectableDayPredicate,
        CalendarDelegate<DateTime> calendarDelegate
    )
    {
        this.selectedDateStart = selectedDateStart;
        this.selectedDateEnd = selectedDateEnd;
        this.currentDate = currentDate;
        this.onChanged = onChanged;
        this.firstDate = firstDate;
        this.lastDate = lastDate;
        this.displayedMonth = displayedMonth;
        this.selectableDayPredicate = selectableDayPredicate;
        this.calendarDelegate = calendarDelegate;
        System.Diagnostics.Debug.Assert(!firstDate.isAfter(lastDate));
        System.Diagnostics.Debug.Assert(
            (selectedDateStart is null)
                || !DartRuntimePrimitives.RequireValue(selectedDateStart).isBefore(firstDate)
        );
        System.Diagnostics.Debug.Assert(
            (selectedDateEnd is null)
                || !DartRuntimePrimitives.RequireValue(selectedDateEnd).isBefore(firstDate)
        );
        System.Diagnostics.Debug.Assert(
            (selectedDateStart is null)
                || !DartRuntimePrimitives.RequireValue(selectedDateStart).isAfter(lastDate)
        );
        System.Diagnostics.Debug.Assert(
            (selectedDateEnd is null)
                || !DartRuntimePrimitives.RequireValue(selectedDateEnd).isAfter(lastDate)
        );
        System.Diagnostics.Debug.Assert(
            (selectedDateStart is null)
                || (selectedDateEnd is null)
                || !DartRuntimePrimitives
                    .RequireValue(selectedDateStart)
                    .isAfter(
                        DartRuntimePrimitives.RequireValue(
                            DartRuntimePrimitives.RequireValue(selectedDateEnd)
                        )
                    )
        );
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _MonthItemState__date_picker());
}

internal class _MonthItemState__date_picker : State<_MonthItem__date_picker>
{
    internal virtual List<FocusNode> _dayFocusNodes { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        long daysInMonth = widget.calendarDelegate.getDaysInMonth(
            widget.displayedMonth.Year,
            widget.displayedMonth.Month
        );
        _dayFocusNodes = new List<FocusNode>(
            Enumerable.Select(
                Enumerable.Range(0, checked((int)daysInMonth)),
                (index) => new FocusNode(skipTraversal: true, debugLabel: $"Day {index + 1L}")
            )
        );
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        DateTime? focusedDate = _FocusedDate__date_picker.maybeOf(context)?.date;
        if (
            (focusedDate is not null)
            && widget.calendarDelegate.isSameMonth(
                widget.displayedMonth,
                DartRuntimePrimitives.RequireValue(focusedDate)
            )
        )
        {
            DateTime focusedDate__98201__value98260 = DartRuntimePrimitives.RequireValue(
                focusedDate
            );
            _dayFocusNodes[
                (int)(DartRuntimePrimitives.RequireValue(focusedDate__98201__value98260).Day - 1L)
            ]
                .requestFocus();
        }
    }

    public override void dispose()
    {
        foreach (FocusNode node in _dayFocusNodes)
        {
            node.dispose();
        }
        base.dispose();
    }

    internal virtual Color _highlightColor(BuildContext context)
    {
        return DatePickerTheme.of(context).rangeSelectionBackgroundColor
            ?? DatePickerTheme.defaults(context).rangeSelectionBackgroundColor!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _dayFocusChanged(bool focused)
    {
        if (focused)
        {
            TraversalDirection? focusDirection = _FocusedDate__date_picker
                .maybeOf(context)
                ?.scrollDirection;
            if (focusDirection is not null)
            {
                TraversalDirection focusDirection__98861__value98936 =
                    DartRuntimePrimitives.RequireValue(focusDirection);
                ScrollPositionAlignmentPolicy policy = ScrollPositionAlignmentPolicy.@explicit;
                switch (DartRuntimePrimitives.RequireValue(focusDirection__98861__value98936))
                {
                    case TraversalDirection.up:
                    case TraversalDirection.left:
                    {
                        policy = ScrollPositionAlignmentPolicy.keepVisibleAtStart;
                        break;
                    }
                    case TraversalDirection.right:
                    case TraversalDirection.down:
                    {
                        policy = ScrollPositionAlignmentPolicy.keepVisibleAtEnd;
                        break;
                    }
                }
                DartRuntimePrimitives.Ignore(
                    Scrollable.ensureVisible(
                        Focus_managerLibrary.primaryFocus!.context!,
                        duration: Calendar_date_pickerLibrary._monthScrollDuration,
                        alignmentPolicy: policy
                    )
                );
            }
        }
    }

    internal virtual Widget _buildDayItem(
        BuildContext context,
        DateTime dayToBuild,
        long firstDayOffset,
        long daysInMonth
    )
    {
        long dayLocal = dayToBuild.Day;
        bool isDisabledLocal =
            dayToBuild.isAfter(widget.lastDate)
            || dayToBuild.isBefore(widget.firstDate)
            || (
                (widget.selectableDayPredicate is not null)
                && !widget.selectableDayPredicate!(
                    dayToBuild,
                    widget.selectedDateStart,
                    widget.selectedDateEnd
                )
            );
        bool isRangeSelectedLocal =
            (widget.selectedDateStart is not null) && (widget.selectedDateEnd is not null);
        bool isSelectedDayStartLocal =
            (widget.selectedDateStart is not null)
            && dayToBuild.isAtSameMomentAs(
                DartRuntimePrimitives.RequireValue(widget.selectedDateStart)
            );
        bool isSelectedDayEndLocal =
            (widget.selectedDateEnd is not null)
            && dayToBuild.isAtSameMomentAs(
                DartRuntimePrimitives.RequireValue(widget.selectedDateEnd)
            );
        bool isInRangeLocal =
            isRangeSelectedLocal
            && dayToBuild.isAfter(DartRuntimePrimitives.RequireValue(widget.selectedDateStart))
            && dayToBuild.isBefore(DartRuntimePrimitives.RequireValue(widget.selectedDateEnd));
        bool isOneDayRangeLocal =
            isRangeSelectedLocal && Equals(widget.selectedDateStart, widget.selectedDateEnd);
        bool isTodayLocal = widget.calendarDelegate.isSameDay(widget.currentDate, dayToBuild);
        return new _DayItem__date_picker(
            calendarDelegate: widget.calendarDelegate,
            day: dayToBuild,
            focusNode: _dayFocusNodes[(int)(dayLocal - 1L)],
            onChanged: widget.onChanged,
            onFocusChange: _dayFocusChanged,
            highlightColor: _highlightColor(context),
            isDisabled: isDisabledLocal,
            isRangeSelected: isRangeSelectedLocal,
            isSelectedDayStart: isSelectedDayStartLocal,
            isSelectedDayEnd: isSelectedDayEndLocal,
            isInRange: isInRangeLocal,
            isOneDayRange: isOneDayRangeLocal,
            isToday: isTodayLocal
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildEdgeBox(BuildContext context, bool isHighlighted)
    {
        Widget empty = new LimitedBox(
            maxWidth: 0.0,
            maxHeight: 0.0,
            child: SizedBox.CreateExpand()
        );
        return isHighlighted
            ? new ColoredBox(color: _highlightColor(context), child: empty)
            : empty;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        ThemeData themeData = Theme.of(context);
        TextTheme textThemeLocal = themeData.textTheme;
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        long year = widget.displayedMonth.Year;
        long month = widget.displayedMonth.Month;
        long daysInMonth = widget.calendarDelegate.getDaysInMonth(year, month);
        long dayOffset = widget.calendarDelegate.firstDayOffset(year, month, localizations);
        long weeks = ((daysInMonth + dayOffset) / 7L).ceil();
        double gridHeight =
            (weeks * Date_pickerLibrary._monthItemRowHeight)
            + ((weeks - 1L) * Date_pickerLibrary._monthItemSpaceBetweenRows);
        var dayItems = new List<Widget>();
        for (long day = 0L - dayOffset + 1L; day <= daysInMonth; day += 1L)
        {
            if (day < 1L)
            {
                dayItems.Add(
                    new LimitedBox(maxWidth: 0.0, maxHeight: 0.0, child: SizedBox.CreateExpand())
                );
            }
            else
            {
                DateTime dayToBuild = widget.calendarDelegate.getDay(year, month, day);
                Widget dayItem = _buildDayItem(context, dayToBuild, dayOffset, daysInMonth);
                dayItems.Add(dayItem);
            }
        }
        var paddedDayItems = new List<Widget>();
        for (var i = 0L; i < weeks; i++)
        {
            long start = i * 7L;
            long end = Math.Min(start + 7L, checked(dayItems.Count));
            List<Widget> weekList = dayItems.GetRange(start, end).ToList();
            DateTime dateAfterLeadingPadding = widget.calendarDelegate.getDay(
                year,
                month,
                start - dayOffset + 1L
            );
            bool isLeadingInRange =
                !((dayOffset > 0L) && (i == 0L))
                && (widget.selectedDateStart is not null)
                && (widget.selectedDateEnd is not null)
                && dateAfterLeadingPadding.isAfter(
                    DartRuntimePrimitives.RequireValue(widget.selectedDateStart)
                )
                && !dateAfterLeadingPadding.isAfter(
                    DartRuntimePrimitives.RequireValue(widget.selectedDateEnd)
                );
            weekList.Insert(checked((int)0L), _buildEdgeBox(context, isLeadingInRange));
            if (
                (end < checked(dayItems.Count))
                || ((end == checked(dayItems.Count)) && ((checked(dayItems.Count) % 7L) == 0L))
            )
            {
                DateTime dateBeforeTrailingPadding = widget.calendarDelegate.getDay(
                    year,
                    month,
                    end - dayOffset
                );
                bool isTrailingInRange =
                    (widget.selectedDateStart is not null)
                    && (widget.selectedDateEnd is not null)
                    && !dateBeforeTrailingPadding.isBefore(
                        DartRuntimePrimitives.RequireValue(widget.selectedDateStart)
                    )
                    && dateBeforeTrailingPadding.isBefore(
                        DartRuntimePrimitives.RequireValue(widget.selectedDateEnd)
                    );
                weekList.Add(_buildEdgeBox(context, isTrailingInRange));
            }
            paddedDayItems.AddRange(weekList.Cast<Widget>());
        }
        double maxWidthLocal = Equals(MediaQuery.orientationOf(context), Orientation.landscape)
            ? Date_pickerLibrary._maxCalendarWidthLandscape
            : Date_pickerLibrary._maxCalendarWidthPortrait;
        return new Column(
            children: new List<Widget>
            {
                DartRuntimePrimitives.ConvertValue<Widget>(
                    new ConstrainedBox(
                        constraints: new BoxConstraints(maxWidth: maxWidthLocal).tighten(
                            height: Date_pickerLibrary._monthItemHeaderHeight
                        ),
                        child: new Padding(
                            padding: EdgeInsets.CreateSymmetric(horizontal: 16),
                            child: new Align(
                                alignment: AlignmentDirectional.centerStart,
                                child: new ExcludeSemantics(
                                    child: new Text(
                                        widget.calendarDelegate.formatMonthYear(
                                            widget.displayedMonth,
                                            localizations
                                        ),
                                        style: textThemeLocal.bodyMedium!.apply(
                                            color: themeData.colorScheme.onSurface
                                        )
                                    )
                                )
                            )
                        )
                    )
                ),
                DartRuntimePrimitives.ConvertValue<Widget>(
                    new ConstrainedBox(
                        constraints: new BoxConstraints(
                            maxWidth: maxWidthLocal,
                            maxHeight: gridHeight
                        ),
                        child: GridView.CreateCustom(
                            physics: new NeverScrollableScrollPhysics(),
                            gridDelegate: Date_pickerLibrary._monthItemGridDelegate,
                            childrenDelegate: new SliverChildListDelegate(
                                paddedDayItems,
                                addRepaintBoundaries: false
                            )
                        )
                    )
                ),
                DartRuntimePrimitives.ConvertValue<Widget>(
                    new SizedBox(height: Date_pickerLibrary._monthItemFooterHeight)
                ),
            }
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _DayItem__date_picker : StatefulWidget
{
    public virtual DateTime day { get; private set; } = default!;
    public virtual FocusNode focusNode { get; private set; } = default!;
    public virtual Action<DateTime> onChanged { get; private set; } = default!;
    public virtual Action<bool> onFocusChange { get; private set; } = default!;
    public virtual Color highlightColor { get; private set; } = default!;
    public virtual bool isDisabled { get; private set; } = default!;
    public virtual bool isRangeSelected { get; private set; } = default!;
    public virtual bool isSelectedDayStart { get; private set; } = default!;
    public virtual bool isSelectedDayEnd { get; private set; } = default!;
    public virtual bool isInRange { get; private set; } = default!;
    public virtual bool isOneDayRange { get; private set; } = default!;
    public virtual bool isToday { get; private set; } = default!;
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    internal _DayItem__date_picker(
        DateTime day,
        FocusNode focusNode,
        Action<DateTime> onChanged,
        Action<bool> onFocusChange,
        Color highlightColor,
        bool isDisabled,
        bool isRangeSelected,
        bool isSelectedDayStart,
        bool isSelectedDayEnd,
        bool isInRange,
        bool isOneDayRange,
        bool isToday,
        CalendarDelegate<DateTime> calendarDelegate
    )
    {
        this.day = day;
        this.focusNode = focusNode;
        this.onChanged = onChanged;
        this.onFocusChange = onFocusChange;
        this.highlightColor = highlightColor;
        this.isDisabled = isDisabled;
        this.isRangeSelected = isRangeSelected;
        this.isSelectedDayStart = isSelectedDayStart;
        this.isSelectedDayEnd = isSelectedDayEnd;
        this.isInRange = isInRange;
        this.isOneDayRange = isOneDayRange;
        this.isToday = isToday;
        this.calendarDelegate = calendarDelegate;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _DayItemState__date_picker());
}

internal class _DayItemState__date_picker : State<_DayItem__date_picker>
{
    internal virtual WidgetStatesController _statesController { get; private set; } =
        new WidgetStatesController();

    public override void dispose()
    {
        _statesController.dispose();
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        ThemeData themeLocal = Theme.of(context);
        ColorScheme colorSchemeLocal = themeLocal.colorScheme;
        TextTheme textThemeLocal = themeLocal.textTheme;
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        DatePickerThemeData datePickerTheme = DatePickerTheme.of(context);
        DatePickerThemeData defaultsLocal = DatePickerTheme.defaults(context);
        TextDirection textDirectionLocal = Directionality.of(context);
        Color highlightColorLocal = widget.highlightColor;
        ShapeDecoration? decorationLocal = default!;
        TextStyle? itemStyle = textThemeLocal.bodyMedium;
        P? effectiveValue<P>(Func<DatePickerThemeData?, P?> getProperty)
        {
            return getProperty(datePickerTheme) ?? getProperty(defaultsLocal);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        P? resolve<P>(
            Func<DatePickerThemeData?, WidgetStateProperty<P>?> getProperty,
            HashSet<WidgetState> states
        )
        {
            return effectiveValue(
                (theme) =>
                {
                    return getProperty(theme) is { } property ? property.resolve(states) : default;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }
            );
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        var statesLocal = (
            (Func<HashSet<WidgetState>>)(
                () =>
                {
                    var __collection108309 = new HashSet<WidgetState>();
                    if (widget.isDisabled)
                    {
                        __collection108309.Add(WidgetState.disabled);
                    }
                    if (widget.isSelectedDayStart || widget.isSelectedDayEnd)
                    {
                        __collection108309.Add(WidgetState.selected);
                    }
                    return __collection108309;
                }
            )
        )();
        _statesController.value = statesLocal;
        Color? dayForegroundColorLocal = resolve((theme) => theme?.dayForegroundColor, statesLocal);
        Color? dayBackgroundColorLocal = resolve((theme) => theme?.dayBackgroundColor, statesLocal);
        WidgetStateProperty<Color?> dayOverlayColorLocal = WidgetStateProperty.resolveWith(
            (states) =>
                effectiveValue(
                    (theme) =>
                        widget.isInRange
                            ? theme?.rangeSelectionOverlayColor?.resolve(states)
                            : theme?.dayOverlayColor?.resolve(states)
                )
        );
        OutlinedBorder dayShapeLocal =
            resolve((theme) => theme?.dayShape, statesLocal) ?? new CircleBorder();
        _HighlightPainter__date_picker? highlightPainter = default!;
        if (widget.isSelectedDayStart || widget.isSelectedDayEnd)
        {
            itemStyle = itemStyle?.apply(color: dayForegroundColorLocal);
            decorationLocal = new ShapeDecoration(
                color: dayBackgroundColorLocal,
                shape: dayShapeLocal
            );
            if (widget.isRangeSelected && !widget.isOneDayRange)
            {
                _HighlightPainterStyle__date_picker styleLocal = widget.isSelectedDayStart
                    ? _HighlightPainterStyle__date_picker.highlightTrailing
                    : _HighlightPainterStyle__date_picker.highlightLeading;
                highlightPainter = new _HighlightPainter__date_picker(
                    color: highlightColorLocal,
                    style: styleLocal,
                    textDirection: textDirectionLocal
                );
            }
        }
        else
        {
            if (widget.isInRange)
            {
                highlightPainter = new _HighlightPainter__date_picker(
                    color: highlightColorLocal,
                    style: _HighlightPainterStyle__date_picker.highlightAll,
                    textDirection: textDirectionLocal
                );
                if (widget.isDisabled)
                {
                    itemStyle = itemStyle?.apply(
                        color: colorSchemeLocal.onSurface.withOpacity(0.38)
                    );
                }
            }
            else
            {
                if (widget.isDisabled)
                {
                    itemStyle = itemStyle?.apply(
                        color: colorSchemeLocal.onSurface.withOpacity(0.38)
                    );
                }
                else
                {
                    if (widget.isToday)
                    {
                        itemStyle = itemStyle?.apply(color: colorSchemeLocal.primary);
                        BorderSide todaySide = (
                            datePickerTheme.todayBorder ?? defaultsLocal.todayBorder!
                        ).copyWith(color: colorSchemeLocal.primary);
                        decorationLocal = new ShapeDecoration(
                            shape: dayShapeLocal.copyWith(side: todaySide)
                        );
                    }
                }
            }
        }
        string dayText = localizations.formatDecimal(widget.day.Day);
        var semanticLabelSuffix = widget.isToday ? $", {localizations.currentDateLabel}" : "";
        var semanticLabel =
            $"{dayText}, {widget.calendarDelegate.formatFullDate(widget.day, localizations)}{semanticLabelSuffix}";
        if (widget.isSelectedDayStart)
        {
            semanticLabel = localizations.dateRangeStartDateSemanticLabel(semanticLabel);
        }
        else
        {
            if (widget.isSelectedDayEnd)
            {
                semanticLabel = localizations.dateRangeEndDateSemanticLabel(semanticLabel);
            }
        }
        Widget dayWidget = new Container(
            decoration: decorationLocal,
            alignment: Alignment.center,
            child: new Widgets.Semantics(
                label: semanticLabel,
                selected: widget.isSelectedDayStart || widget.isSelectedDayEnd,
                child: new ExcludeSemantics(child: new Text(dayText, style: itemStyle))
            )
        );
        if (highlightPainter is not null)
        {
            dayWidget = DartRuntimePrimitives.ConvertValue<Widget>(
                new CustomPaint(painter: highlightPainter, child: dayWidget)
            );
        }
        if (!widget.isDisabled)
        {
            dayWidget = DartRuntimePrimitives.ConvertValue<Widget>(
                new InkResponse(
                    focusNode: widget.focusNode,
                    onTap: () =>
                    {
                        widget.onChanged(widget.day);
                    },
                    customBorder: dayShapeLocal,
                    containedInkWell: true,
                    statesController: _statesController,
                    overlayColor: dayOverlayColorLocal,
                    onFocusChange: widget.onFocusChange,
                    child: dayWidget
                )
            );
        }
        return dayWidget;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal enum _HighlightPainterStyle__date_picker
{
    none,
    highlightLeading,
    highlightTrailing,
    highlightAll,
}

internal class _HighlightPainter__date_picker : CustomPainter
{
    public virtual Color color { get; private set; } = default!;
    public virtual _HighlightPainterStyle__date_picker style { get; private set; } = default!;
    public virtual TextDirection? textDirection { get; private set; }

    internal _HighlightPainter__date_picker(
        Color color,
        _HighlightPainterStyle__date_picker style = _HighlightPainterStyle__date_picker.none,
        TextDirection? textDirection = null
    )
    {
        this.color = color;
        this.style = style;
        this.textDirection = textDirection;
    }

    public override void paint(Canvas canvas, Size size)
    {
        if (Equals(style, _HighlightPainterStyle__date_picker.none))
        {
            return;
        }
        var paintLocal = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.color = color;
                    __cascade.style = PaintingStyle.fill;
                    return __cascade;
                }
            )
        )();
        bool rtlLocal = textDirection switch
        {
            TextDirection.rtl => true,
            null => true,
            TextDirection.ltr => false,
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        switch (style)
        {
            case _HighlightPainterStyle__date_picker.highlightLeading when rtlLocal:
            case _HighlightPainterStyle__date_picker.highlightTrailing when !rtlLocal:
            {
                canvas.drawRect(
                    Rect.fromLTWH(size.width / 2L, 0, size.width / 2L, size.height),
                    paintLocal
                );
                break;
            }
            case _HighlightPainterStyle__date_picker.highlightLeading:
            case _HighlightPainterStyle__date_picker.highlightTrailing:
            {
                canvas.drawRect(Rect.fromLTWH(0, 0, size.width / 2L, size.height), paintLocal);
                break;
            }
            case _HighlightPainterStyle__date_picker.highlightAll:
            {
                canvas.drawRect(Rect.fromLTWH(0, 0, size.width, size.height), paintLocal);
                break;
            }
            case _HighlightPainterStyle__date_picker.none:
            {
                break;
            }
        }
    }

    public override bool shouldRepaint(CustomPainter oldDelegate) => false;
}

internal class _InputDateRangePickerDialog__date_picker : StatelessWidget
{
    public virtual DateTime? selectedStartDate { get; private set; }
    public virtual DateTime? selectedEndDate { get; private set; }
    public virtual DateTime? currentDate { get; private set; }
    public virtual Widget picker { get; private set; } = default!;
    public virtual Action onConfirm { get; private set; } = default!;
    public virtual Action onCancel { get; private set; } = default!;
    public virtual string? confirmText { get; private set; }
    public virtual string? cancelText { get; private set; }
    public virtual string? helpText { get; private set; }
    public virtual Widget? entryModeButton { get; private set; }
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    internal _InputDateRangePickerDialog__date_picker(
        DateTime? selectedStartDate,
        DateTime? selectedEndDate,
        DateTime? currentDate,
        Widget picker,
        Action onConfirm,
        Action onCancel,
        string? confirmText,
        string? cancelText,
        string? helpText,
        Widget? entryModeButton,
        CalendarDelegate<DateTime> calendarDelegate
    )
    {
        this.selectedStartDate = selectedStartDate;
        this.selectedEndDate = selectedEndDate;
        this.currentDate = currentDate;
        this.picker = picker;
        this.onConfirm = onConfirm;
        this.onCancel = onCancel;
        this.confirmText = confirmText;
        this.cancelText = cancelText;
        this.helpText = helpText;
        this.entryModeButton = entryModeButton;
        this.calendarDelegate = calendarDelegate;
    }

    internal virtual string _formatDateRange(
        BuildContext context,
        DateTime? start,
        DateTime? end,
        DateTime now
    )
    {
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        string startText = Date_pickerLibrary._formatRangeStartDate(
            localizations,
            calendarDelegate,
            start,
            end
        );
        string endText = Date_pickerLibrary._formatRangeEndDate(
            localizations,
            calendarDelegate,
            start,
            end,
            now
        );
        if ((start is null) || (end is null))
        {
            return localizations.unspecifiedDateRange;
        }
        return Directionality.of(context) switch
        {
            TextDirection.rtl => $"{endText} – {startText}",
            TextDirection.ltr => $"{startText} – {endText}",
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
    }

    public override Widget build(BuildContext context)
    {
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        Orientation orientationLocal = MediaQuery.orientationOf(context);
        DatePickerThemeData datePickerTheme = DatePickerTheme.of(context);
        DatePickerThemeData defaultsLocal = DatePickerTheme.defaults(context);
        TextStyle? headlineStyle = Equals(orientationLocal, Orientation.portrait)
            ? (datePickerTheme.headerHeadlineStyle ?? defaultsLocal.headerHeadlineStyle)
            : Theme.of(context).textTheme.headlineSmall;
        Color? headerForegroundColorLocal =
            datePickerTheme.headerForegroundColor ?? defaultsLocal.headerForegroundColor;
        headlineStyle = headlineStyle?.copyWith(color: headerForegroundColorLocal);
        string dateText = _formatDateRange(
            context,
            selectedStartDate,
            selectedEndDate,
            DartRuntimePrimitives.RequireValue(currentDate)
        );
        var semanticDateText =
            ((selectedStartDate is not null) && (selectedEndDate is not null))
                ? $"{calendarDelegate.formatMediumDate(DartRuntimePrimitives.RequireValue(selectedStartDate), localizations)} – {calendarDelegate.formatMediumDate(DartRuntimePrimitives.RequireValue(selectedEndDate), localizations)}"
                : "";
        Widget header = new _DatePickerHeader__date_picker(
            helpText: helpText ?? localizations.dateRangePickerHelpText,
            titleText: dateText,
            titleSemanticsLabel: semanticDateText,
            titleStyle: headlineStyle,
            orientation: orientationLocal,
            isShort: Equals(orientationLocal, Orientation.landscape),
            entryModeButton: entryModeButton
        );
        Widget actions = new ConstrainedBox(
            constraints: new BoxConstraints(minHeight: 52.0),
            child: new Padding(
                padding: EdgeInsets.CreateSymmetric(horizontal: 8),
                child: new Align(
                    alignment: AlignmentDirectional.centerEnd,
                    child: new OverflowBar(
                        spacing: 8,
                        children: new List<Widget>
                        {
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                new TextButton(
                                    onPressed: onCancel,
                                    child: new Text(cancelText ?? localizations.cancelButtonLabel)
                                )
                            ),
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                new TextButton(
                                    onPressed: onConfirm,
                                    child: new Text(confirmText ?? localizations.okButtonLabel)
                                )
                            ),
                        }
                    )
                )
            )
        );
        double textScaleFactor =
            MediaQuery
                .textScalerOf(context)
                .clamp(maxScaleFactor: Date_pickerLibrary._kMaxRangeTextScaleFactor)
                .scale(Calendar_date_pickerLibrary._fontSizeToScale)
            / Calendar_date_pickerLibrary._fontSizeToScale;
        Size dialogSize = Date_pickerLibrary._inputPortraitDialogSizeM3 * textScaleFactor;
        switch (orientationLocal)
        {
            case Orientation.portrait:
            {
                return new LayoutBuilder(
                    builder: (context, constraints) =>
                    {
                        Size portraitDialogSize = Date_pickerLibrary._inputPortraitDialogSizeM3;
                        bool isFullyPortrait =
                            constraints.maxHeight
                            >= Math.Min(dialogSize.height, portraitDialogSize.height);
                        return new Column(
                            mainAxisSize: MainAxisSize.min,
                            crossAxisAlignment: CrossAxisAlignment.stretch,
                            children: (
                                (Func<List<Widget>>)(
                                    () =>
                                    {
                                        var __collection120213 = new List<Widget>();
                                        if (isFullyPortrait)
                                        {
                                            __collection120213.Add(
                                                DartRuntimePrimitives.ConvertValue<Widget>(header)
                                            );
                                        }
                                        __collection120213.Add(
                                            DartRuntimePrimitives.ConvertValue<Widget>(
                                                new Expanded(child: picker)
                                            )
                                        );
                                        __collection120213.Add(
                                            DartRuntimePrimitives.ConvertValue<Widget>(actions)
                                        );
                                        return __collection120213;
                                    }
                                )
                            )()
                        );
                    }
                );
            }
            case Orientation.landscape:
            {
                return new Row(
                    mainAxisSize: MainAxisSize.min,
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: new List<Widget>
                    {
                        DartRuntimePrimitives.ConvertValue<Widget>(header),
                        DartRuntimePrimitives.ConvertValue<Widget>(
                            new Flexible(
                                child: new Column(
                                    mainAxisSize: MainAxisSize.min,
                                    crossAxisAlignment: CrossAxisAlignment.stretch,
                                    children: new List<Widget>
                                    {
                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                            new Expanded(child: picker)
                                        ),
                                        DartRuntimePrimitives.ConvertValue<Widget>(actions),
                                    }
                                )
                            )
                        ),
                    }
                );
            }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _InputDateRangePicker__date_picker : StatefulWidget
{
    public virtual DateTime? initialStartDate { get; private set; }
    public virtual DateTime? initialEndDate { get; private set; }
    public virtual DateTime firstDate { get; private set; } = default!;
    public virtual DateTime lastDate { get; private set; } = default!;
    public virtual Action<DateTime?>? onStartDateChanged { get; private set; }
    public virtual Action<DateTime?>? onEndDateChanged { get; private set; }
    public virtual string? helpText { get; private set; }
    public virtual string? errorFormatText { get; private set; }
    public virtual string? errorInvalidText { get; private set; }
    public virtual string? errorInvalidRangeText { get; private set; }
    public virtual string? fieldStartHintText { get; private set; }
    public virtual string? fieldEndHintText { get; private set; }
    public virtual string? fieldStartLabelText { get; private set; }
    public virtual string? fieldEndLabelText { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual bool autovalidate { get; private set; } = default!;
    public virtual TextInputType keyboardType { get; private set; } = default!;
    public virtual Func<DateTime, DateTime?, DateTime?, bool>? selectableDayPredicate
    {
        get;
        private set;
    }
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    internal _InputDateRangePicker__date_picker(
        Key? key = null,
        DateTime? initialStartDate = null,
        DateTime? initialEndDate = null,
        DateTime firstDate = default!,
        DateTime lastDate = default!,
        Action<DateTime?>? onStartDateChanged = default!,
        Action<DateTime?>? onEndDateChanged = default!,
        Func<DateTime, DateTime?, DateTime?, bool>? selectableDayPredicate = default!,
        CalendarDelegate<DateTime> calendarDelegate = default!,
        string? helpText = null,
        string? errorFormatText = null,
        string? errorInvalidText = null,
        string? errorInvalidRangeText = null,
        string? fieldStartHintText = null,
        string? fieldEndHintText = null,
        string? fieldStartLabelText = null,
        string? fieldEndLabelText = null,
        bool autofocus = false,
        bool autovalidate = false,
        TextInputType keyboardType = default!
    )
        : base(key: key)
    {
        TextInputType __keyboardType = keyboardType ?? TextInputType.datetime;
        this.onStartDateChanged = onStartDateChanged;
        this.onEndDateChanged = onEndDateChanged;
        this.selectableDayPredicate = selectableDayPredicate;
        this.calendarDelegate = calendarDelegate;
        this.helpText = helpText;
        this.errorFormatText = errorFormatText;
        this.errorInvalidText = errorInvalidText;
        this.errorInvalidRangeText = errorInvalidRangeText;
        this.fieldStartHintText = fieldStartHintText;
        this.fieldEndHintText = fieldEndHintText;
        this.fieldStartLabelText = fieldStartLabelText;
        this.fieldEndLabelText = fieldEndLabelText;
        this.autofocus = autofocus;
        this.autovalidate = autovalidate;
        this.keyboardType = __keyboardType;
        this.initialStartDate =
            (initialStartDate is null)
                ? null
                : this.calendarDelegate.dateOnly(
                    DartRuntimePrimitives.RequireValue(
                        DartRuntimePrimitives.RequireValue(initialStartDate)
                    )
                );
        this.initialEndDate =
            (initialEndDate is null)
                ? null
                : this.calendarDelegate.dateOnly(
                    DartRuntimePrimitives.RequireValue(
                        DartRuntimePrimitives.RequireValue(initialEndDate)
                    )
                );
        this.firstDate = this.calendarDelegate.dateOnly(firstDate);
        this.lastDate = this.calendarDelegate.dateOnly(lastDate);
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _InputDateRangePickerState__date_picker());
}

internal class _InputDateRangePickerState__date_picker : State<_InputDateRangePicker__date_picker>
{
    internal virtual string _startInputText { get; set; } = default!;
    internal virtual string _endInputText { get; set; } = default!;
    internal virtual DateTime? _startDate { get; set; } = default;
    internal virtual DateTime? _endDate { get; set; } = default;
    internal virtual TextEditingController _startController { get; set; } = default!;
    internal virtual TextEditingController _endController { get; set; } = default!;
    internal virtual string? _startErrorText { get; set; } = default;
    internal virtual string? _endErrorText { get; set; } = default;
    internal virtual bool _autoSelected { get; set; } = false;

    public override void initState()
    {
        base.initState();
        _startDate = widget.initialStartDate;
        _startController = new TextEditingController();
        _endDate = widget.initialEndDate;
        _endController = new TextEditingController();
    }

    public override void dispose()
    {
        _startController.dispose();
        _endController.dispose();
        base.dispose();
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        if (_startDate is not null)
        {
            _startInputText = widget.calendarDelegate.formatCompactDate(
                DartRuntimePrimitives.RequireValue(_startDate),
                localizations
            );
            bool selectText = widget.autofocus && !_autoSelected;
            _updateController(_startController, _startInputText, selectText);
            _autoSelected = selectText;
        }
        if (_endDate is not null)
        {
            _endInputText = widget.calendarDelegate.formatCompactDate(
                DartRuntimePrimitives.RequireValue(_endDate),
                localizations
            );
            _updateController(_endController, _endInputText, false);
        }
    }

    public virtual bool validate()
    {
        string? startError = _validateDate(_startDate);
        string? endError = _validateDate(_endDate);
        if ((startError is null) && (endError is null))
        {
            if (
                DartRuntimePrimitives
                    .RequireValue(_startDate)
                    .isAfter(DartRuntimePrimitives.RequireValue(_endDate))
            )
            {
                startError =
                    widget.errorInvalidRangeText
                    ?? MaterialLocalizations.of(context).invalidDateRangeLabel;
            }
        }
        setState(() =>
        {
            _startErrorText = startError;
            _endErrorText = endError;
        });
        return (startError is null) && (endError is null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual DateTime? _parseDate(string? text)
    {
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        return widget.calendarDelegate.parseCompactDate(text, localizations);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual string? _validateDate(DateTime? date)
    {
        if (date is null)
        {
            return widget.errorFormatText
                ?? MaterialLocalizations.of(context).invalidDateFormatLabel;
        }
        else
        {
            if (
                !_isDaySelectable(
                    DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(date))
                )
            )
            {
                return widget.errorInvalidText
                    ?? MaterialLocalizations.of(context).dateOutOfRangeLabel;
            }
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isDaySelectable(DateTime day)
    {
        if (day.isBefore(widget.firstDate) || day.isAfter(widget.lastDate))
        {
            return false;
        }
        if (widget.selectableDayPredicate is null)
        {
            return true;
        }
        return widget.selectableDayPredicate!(day, _startDate, _endDate);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _updateController(
        TextEditingController controller,
        string text,
        bool selectText
    )
    {
        TextEditingValue textEditingValue = controller.value.copyWith(text: text);
        if (selectText)
        {
            textEditingValue = textEditingValue.copyWith(
                selection: new TextSelection(baseOffset: 0L, extentOffset: text.Length)
            );
        }
        controller.value = textEditingValue;
    }

    internal virtual void _handleStartChanged(string text)
    {
        setState(() =>
        {
            _startInputText = text;
            _startDate = _parseDate(text);
            widget.onStartDateChanged?.Invoke(_startDate);
        });
        if (widget.autovalidate)
        {
            validate();
        }
    }

    internal virtual void _handleEndChanged(string text)
    {
        setState(() =>
        {
            _endInputText = text;
            _endDate = _parseDate(text);
            widget.onEndDateChanged?.Invoke(_endDate);
        });
        if (widget.autovalidate)
        {
            validate();
        }
    }

    public override Widget build(BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        InputDecorationThemeData inputTheme = InputDecorationTheme.of(context);
        InputBorder inputBorder = inputTheme.border ?? new OutlineInputBorder();
        return new Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: new List<Widget>
            {
                DartRuntimePrimitives.ConvertValue<Widget>(
                    new Expanded(
                        child: new TextField(
                            controller: _startController,
                            decoration: new InputDecoration(
                                border: inputBorder,
                                filled: inputTheme.filled,
                                hintText: widget.fieldStartHintText
                                    ?? widget.calendarDelegate.dateHelpText(localizations),
                                labelText: widget.fieldStartLabelText
                                    ?? localizations.dateRangeStartLabel,
                                errorText: _startErrorText
                            ),
                            keyboardType: widget.keyboardType,
                            onChanged: _handleStartChanged,
                            autofocus: widget.autofocus
                        )
                    )
                ),
                DartRuntimePrimitives.ConvertValue<Widget>(new SizedBox(width: 8)),
                DartRuntimePrimitives.ConvertValue<Widget>(
                    new Expanded(
                        child: new TextField(
                            controller: _endController,
                            decoration: new InputDecoration(
                                border: inputBorder,
                                filled: inputTheme.filled,
                                hintText: widget.fieldEndHintText
                                    ?? widget.calendarDelegate.dateHelpText(localizations),
                                labelText: widget.fieldEndLabelText
                                    ?? localizations.dateRangeEndLabel,
                                errorText: _endErrorText
                            ),
                            keyboardType: widget.keyboardType,
                            onChanged: _handleEndChanged
                        )
                    )
                ),
            }
        );
    }
}
