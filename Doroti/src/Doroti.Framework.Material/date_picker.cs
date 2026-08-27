// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/date_picker.dart
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

public static partial class Date_pickerLibrary
{
    internal static Size _calendarPortraitDialogSizeM2 = new global::Doroti.Ui.Size(330.0, 518.0);
}

public static partial class Date_pickerLibrary
{
    internal static Size _calendarPortraitDialogSizeM3 = new global::Doroti.Ui.Size(360.0, 568.0);
}

public static partial class Date_pickerLibrary
{
    internal static Size _calendarLandscapeDialogSize = new global::Doroti.Ui.Size(496.0, 346.0);
}

public static partial class Date_pickerLibrary
{
    internal static Size _inputPortraitDialogSizeM2 = new global::Doroti.Ui.Size(330.0, 270.0);
}

public static partial class Date_pickerLibrary
{
    internal static Size _inputPortraitDialogSizeM3 = new global::Doroti.Ui.Size(328.0, 270.0);
}

public static partial class Date_pickerLibrary
{
    internal static Size _inputLandscapeDialogSize = new global::Doroti.Ui.Size(496, 160.0);
}

public static partial class Date_pickerLibrary
{
    internal static Size _inputRangeLandscapeDialogSize = new global::Doroti.Ui.Size(496, 164.0);
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
    public static async Future<DateTime?> showDatePicker(global::Doroti.Framework.Widgets.BuildContext context, DateTime? initialDate = null, DateTime firstDate = default!, DateTime lastDate = default!, DateTime? currentDate = null, DatePickerEntryMode initialEntryMode = DatePickerEntryMode.calendar, global::System.Func<DateTime, bool>? selectableDayPredicate = null, string? helpText = null, string? cancelText = null, string? confirmText = null, Locale? locale = null, bool barrierDismissible = true, Color? barrierColor = null, string? barrierLabel = null, bool useRootNavigator = true, global::Doroti.Framework.Widgets.RouteSettings? routeSettings = null, TextDirection? textDirection = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>? builder = null, DatePickerMode initialDatePickerMode = DatePickerMode.day, string? errorFormatText = null, string? errorInvalidText = null, string? fieldHintText = null, string? fieldLabelText = null, global::Doroti.Framework.Services.TextInputType? keyboardType = null, Offset? anchorPoint = null, global::System.Action<DatePickerEntryMode>? onDatePickerModeChange = null, global::Doroti.Framework.Widgets.Icon? switchToInputEntryModeIcon = null, global::Doroti.Framework.Widgets.Icon? switchToCalendarEntryModeIcon = null, CalendarDelegate<DateTime> calendarDelegate = default!)
    {
        initialDate = ((initialDate is null) ? null : calendarDelegate.dateOnly(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(initialDate))));
        firstDate = calendarDelegate.dateOnly(firstDate);
        lastDate = calendarDelegate.dateOnly(lastDate);
        DartRuntimePrimitives.Assert(() => !lastDate.isBefore(firstDate), () => (object?)$"lastDate {lastDate} must be on or after firstDate {firstDate}.");
        DartRuntimePrimitives.Assert(() => ((initialDate is null) || !DartRuntimePrimitives.RequireValue(initialDate).isBefore(firstDate)), () => (object?)$"initialDate {DartRuntimePrimitives.RequireValue(initialDate)} must be on or after firstDate {firstDate}.");
        DartRuntimePrimitives.Assert(() => ((initialDate is null) || !DartRuntimePrimitives.RequireValue(initialDate).isAfter(lastDate)), () => (object?)$"initialDate {DartRuntimePrimitives.RequireValue(initialDate)} must be on or before lastDate {lastDate}.");
        DartRuntimePrimitives.Assert(() => (((selectableDayPredicate is null) || (initialDate is null)) || selectableDayPredicate(DartRuntimePrimitives.RequireValue(initialDate))), () => (object?)$"Provided initialDate {DartRuntimePrimitives.RequireValue(initialDate)} must satisfy provided selectableDayPredicate.");
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        global::Doroti.Framework.Widgets.Widget dialog = ((global::Doroti.Framework.Widgets.Widget)(object?)new DatePickerDialog(initialDate: initialDate, firstDate: firstDate, lastDate: lastDate, currentDate: currentDate, initialEntryMode: initialEntryMode, selectableDayPredicate: (global::System.Func<DateTime, bool>?)selectableDayPredicate, helpText: helpText, cancelText: cancelText, confirmText: confirmText, initialCalendarMode: initialDatePickerMode, errorFormatText: errorFormatText, errorInvalidText: errorInvalidText, fieldHintText: fieldHintText, fieldLabelText: fieldLabelText, keyboardType: keyboardType, onDatePickerModeChange: (global::System.Action<DatePickerEntryMode>?)onDatePickerModeChange, switchToInputEntryModeIcon: switchToInputEntryModeIcon, switchToCalendarEntryModeIcon: switchToCalendarEntryModeIcon, calendarDelegate: calendarDelegate));
        if ((textDirection is not null))
        {
            TextDirection textDirection__value11363 = DartRuntimePrimitives.RequireValue(textDirection);
            dialog = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Directionality(textDirection: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(textDirection__value11363)), child: dialog));
        }
        if ((locale is not null))
        {
            Locale locale__value11473 = DartRuntimePrimitives.RequireValue(locale);
            dialog = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(global::Doroti.Framework.Widgets.Localizations.CreateOverride(context: context, locale: DartRuntimePrimitives.RequireValue(locale__value11473), child: dialog));
        }
        else
        {
            DatePickerThemeData datePickerTheme = DatePickerTheme.of(context);
            if ((datePickerTheme.locale is not null))
            {
                dialog = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(global::Doroti.Framework.Widgets.Localizations.CreateOverride(context: context, locale: datePickerTheme.locale, child: dialog));
            }
        }
        return await DialogLibrary.showDialog<DateTime>(context: context, barrierDismissible: barrierDismissible, barrierColor: barrierColor, barrierLabel: barrierLabel, useRootNavigator: useRootNavigator, routeSettings: routeSettings, builder: ((context) =>
        {
            return ((builder is null) ? dialog : builder(context, dialog));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }), anchorPoint: DartRuntimePrimitives.RequireValue(anchorPoint));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class DatePickerDialog : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual DateTime? initialDate { get; private set; }
    public virtual DateTime firstDate { get; private set; } = default!;
    public virtual DateTime lastDate { get; private set; } = default!;
    public virtual DateTime currentDate { get; private set; } = default!;
    public virtual DatePickerEntryMode initialEntryMode { get; private set; } = default!;
    public virtual global::System.Func<DateTime, bool>? selectableDayPredicate { get; private set; }
    public virtual string? cancelText { get; private set; }
    public virtual string? confirmText { get; private set; }
    public virtual string? helpText { get; private set; }
    public virtual DatePickerMode initialCalendarMode { get; private set; } = default!;
    public virtual string? errorFormatText { get; private set; }
    public virtual string? errorInvalidText { get; private set; }
    public virtual string? fieldHintText { get; private set; }
    public virtual string? fieldLabelText { get; private set; }
    public virtual global::Doroti.Framework.Services.TextInputType? keyboardType { get; private set; }
    public virtual string? restorationId { get; private set; }
    public virtual global::System.Action<DatePickerEntryMode>? onDatePickerModeChange { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Icon? switchToInputEntryModeIcon { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Icon? switchToCalendarEntryModeIcon { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsets insetPadding { get; private set; } = default!;
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    public DatePickerDialog(global::Doroti.Framework.Foundation.Key? key = null, DateTime? initialDate = null, DateTime firstDate = default!, DateTime lastDate = default!, DateTime? currentDate = null, DatePickerEntryMode initialEntryMode = DatePickerEntryMode.calendar, global::System.Func<DateTime, bool>? selectableDayPredicate = null, string? cancelText = null, string? confirmText = null, string? helpText = null, DatePickerMode initialCalendarMode = DatePickerMode.day, string? errorFormatText = null, string? errorInvalidText = null, string? fieldHintText = null, string? fieldLabelText = null, global::Doroti.Framework.Services.TextInputType? keyboardType = null, string? restorationId = null, global::System.Action<DatePickerEntryMode>? onDatePickerModeChange = null, global::Doroti.Framework.Widgets.Icon? switchToInputEntryModeIcon = null, global::Doroti.Framework.Widgets.Icon? switchToCalendarEntryModeIcon = null, global::Doroti.Framework.Painting.EdgeInsets insetPadding = default!, CalendarDelegate<DateTime> calendarDelegate = default!) : base(key: key)
    {
        global::Doroti.Framework.Painting.EdgeInsets __insetPadding = insetPadding ?? global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 16.0, vertical: 24.0);
        CalendarDelegate<DateTime> __calendarDelegate = calendarDelegate ?? new GregorianCalendarDelegate();
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
        this.initialDate = ((initialDate is null) ? null : calendarDelegate.dateOnly(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(initialDate))));
        this.firstDate = calendarDelegate.dateOnly(firstDate);
        this.lastDate = calendarDelegate.dateOnly(lastDate);
        this.currentDate = calendarDelegate.dateOnly(((currentDate ?? (DateTime)calendarDelegate.now())));
        DartRuntimePrimitives.Assert(() => !this.lastDate.isBefore(this.firstDate), () => (object?)$"lastDate {this.lastDate} must be on or after firstDate {this.firstDate}.");
        DartRuntimePrimitives.Assert(() => ((initialDate is null) || !DartRuntimePrimitives.RequireValue(this.initialDate).isBefore(this.firstDate)), () => (object?)$"initialDate {this.initialDate} must be on or after firstDate {this.firstDate}.");
        DartRuntimePrimitives.Assert(() => ((initialDate is null) || !DartRuntimePrimitives.RequireValue(this.initialDate).isAfter(this.lastDate)), () => (object?)$"initialDate {this.initialDate} must be on or before lastDate {this.lastDate}.");
        DartRuntimePrimitives.Assert(() => (((this.selectableDayPredicate is null) || (initialDate is null)) || this.selectableDayPredicate!(DartRuntimePrimitives.RequireValue(this.initialDate))), () => (object?)$"Provided initialDate {this.initialDate} must satisfy provided selectableDayPredicate");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DatePickerDialogState__date_picker());
}

internal class _DatePickerDialogState__date_picker : global::Doroti.Framework.Widgets.State<DatePickerDialog>, global::Doroti.Framework.Widgets.RestorationMixin<DatePickerDialog>
{
    private bool __late__selectedDate_initialized;
    private global::Doroti.Framework.Widgets.RestorableDateTimeN __late__selectedDate = default!;
    internal virtual global::Doroti.Framework.Widgets.RestorableDateTimeN _selectedDate
    {
        get
        {
            if (!__late__selectedDate_initialized)
            {
                __late__selectedDate = new global::Doroti.Framework.Widgets.RestorableDateTimeN(((DatePickerDialog)this.widget).initialDate);
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
                __late__entryMode = new _RestorableDatePickerEntryMode__date_picker(((DatePickerDialog)this.widget).initialEntryMode);
                __late__entryMode_initialized = true;
            }
            return __late__entryMode;
        }
    }
    internal virtual _RestorableAutovalidateMode__date_picker _autovalidateMode { get; private set; } = new _RestorableAutovalidateMode__date_picker(global::Doroti.Framework.Widgets.AutovalidateMode.disabled);
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _calendarPickerKey { get; private set; } = global::Doroti.Framework.Widgets.GlobalKey<IState>.Create();
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.FormState> _formKey { get; private set; } = global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.FormState>.Create();
    internal static DartMap<global::Doroti.Framework.Widgets.ShortcutActivator, global::Doroti.Framework.Widgets.Intent> _formShortcutMap = new DartMap<global::Doroti.Framework.Widgets.ShortcutActivator, global::Doroti.Framework.Widgets.Intent> { [new global::Doroti.Framework.Widgets.SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.enter)] = ((global::Doroti.Framework.Widgets.Intent)(object?)new global::Doroti.Framework.Widgets.NextFocusIntent()) };
    public virtual global::Doroti.Framework.Services.RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<dynamic, global::System.Action> _properties { get; set; } = new DartMap<dynamic, global::System.Action>();
    public virtual List<dynamic>? _debugPropertiesWaitingForReregistration { get; set; } = default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual global::Doroti.Framework.Services.RestorationBucket? _currentParent { get; set; } = default;

    public override void dispose()
    {
        this._selectedDate.dispose();
        this._entryMode.dispose();
        this._autovalidateMode.dispose();
        this._properties.forEach(((global::System.Action<dynamic, global::System.Action>)((property, listener) =>
        {
            if (!((dynamic)property)._disposed)
            {
                property.removeListener((global::System.Action)(() => listener()));
            }
        })));
        this._bucket?.dispose();
        _bucket = null;
        base.dispose();
    }

    public virtual string? restorationId => ((DatePickerDialog)this.widget).restorationId;
    public virtual void restoreState(global::Doroti.Framework.Services.RestorationBucket? oldBucket, bool initialRestore)
    {
        registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this._selectedDate), "selected_date");
        registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this._autovalidateMode), "autovalidateMode");
        registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this._entryMode), "calendar_entry_mode");
    }

    internal virtual void _handleOk()
    {
        if (((object.Equals(this._entryMode.value, DatePickerEntryMode.input)) || (object.Equals(this._entryMode.value, DatePickerEntryMode.inputOnly))))
        {
            global::Doroti.Framework.Widgets.FormState form = ((global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.FormState>)this._formKey).currentState!;
            if (!form.validate())
            {
                setState(((global::System.Action)(() => { _ = this._autovalidateMode.value = global::Doroti.Framework.Widgets.AutovalidateMode.always; })));
                return;
            }
            form.save();
        }
        Navigator.pop<object>(this.context, this._selectedDate.value);
    }

    internal virtual void _handleCancel()
    {
        Navigator.pop<object>(this.context);
    }

    internal virtual void _handleOnDatePickerModeChange()
    {
        ((DatePickerDialog)this.widget).onDatePickerModeChange?.Invoke(this._entryMode.value);
    }

    internal virtual void _handleEntryModeToggle()
    {
        setState(((global::System.Action)(() =>
        {
            switch (this._entryMode.value)
            {
                case DatePickerEntryMode.calendar:
                    {
                        this._autovalidateMode.value = global::Doroti.Framework.Widgets.AutovalidateMode.disabled;
                        this._entryMode.value = DatePickerEntryMode.input;
                        _handleOnDatePickerModeChange();
                        break;
                    }
                case DatePickerEntryMode.input:
                    {
                        ((global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.FormState>)this._formKey).currentState!.save();
                        this._entryMode.value = DatePickerEntryMode.calendar;
                        _handleOnDatePickerModeChange();
                        break;
                    }
                case DatePickerEntryMode.calendarOnly:
                case DatePickerEntryMode.inputOnly:
                    {
                        DartRuntimePrimitives.Assert(() => false, () => (object?)$"Can not change entry mode from {this._entryMode.value}");
                        break;
                    }
            }
        })));
    }

    internal virtual void _handleDateChanged(DateTime date)
    {
        setState(((global::System.Action)(() => { _ = this._selectedDate.value = date; })));
    }

    internal virtual global::Doroti.Ui.Size _dialogSize(global::Doroti.Framework.Widgets.BuildContext context)
    {
        bool useMaterial3Local = Theme.of(context).useMaterial3;
        bool isCalendar = (this._entryMode.value switch { DatePickerEntryMode.calendar => true, DatePickerEntryMode.calendarOnly => true, DatePickerEntryMode.input => false, DatePickerEntryMode.inputOnly => false, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Framework.Widgets.Orientation orientation = MediaQuery.orientationOf(context);
        return ((isCalendar, orientation) switch { (true, global::Doroti.Framework.Widgets.Orientation.portrait) when (useMaterial3Local) => Date_pickerLibrary._calendarPortraitDialogSizeM3, (false, global::Doroti.Framework.Widgets.Orientation.portrait) when (useMaterial3Local) => Date_pickerLibrary._inputPortraitDialogSizeM3, (true, global::Doroti.Framework.Widgets.Orientation.portrait) => Date_pickerLibrary._calendarPortraitDialogSizeM2, (false, global::Doroti.Framework.Widgets.Orientation.portrait) => Date_pickerLibrary._inputPortraitDialogSizeM2, (true, global::Doroti.Framework.Widgets.Orientation.landscape) => Date_pickerLibrary._calendarLandscapeDialogSize, (false, global::Doroti.Framework.Widgets.Orientation.landscape) => Date_pickerLibrary._inputLandscapeDialogSize });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        bool useMaterial3Local = theme.useMaterial3;
        MaterialLocalizations localizations = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        global::Doroti.Framework.Widgets.Orientation orientationLocal = MediaQuery.orientationOf(context);
        var isLandscapeOrientation = (object.Equals(orientationLocal, global::Doroti.Framework.Widgets.Orientation.landscape));
        DatePickerThemeData datePickerTheme = DatePickerTheme.of(context);
        DatePickerThemeData defaultsLocal = DatePickerTheme.defaults(context);
        TextTheme textThemeLocal = theme.textTheme;
        global::Doroti.Framework.Painting.TextStyle? headlineStyle = default!;
        if (useMaterial3Local)
        {
            headlineStyle = (datePickerTheme.headerHeadlineStyle ?? defaultsLocal.headerHeadlineStyle);
            switch (this._entryMode.value)
            {
                case DatePickerEntryMode.input:
                case DatePickerEntryMode.inputOnly:
                    {
                        if ((object.Equals(orientationLocal, global::Doroti.Framework.Widgets.Orientation.landscape)))
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
        else
        {
            headlineStyle = (isLandscapeOrientation ? textThemeLocal.headlineSmall : textThemeLocal.headlineMedium);
        }
        global::Doroti.Ui.Color? headerForegroundColorLocal = ((global::Doroti.Ui.Color?)(object?)(datePickerTheme.headerForegroundColor ?? defaultsLocal.headerForegroundColor));
        headlineStyle = headlineStyle?.copyWith(color: headerForegroundColorLocal);
        global::Doroti.Framework.Widgets.Widget actions = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(minHeight: 52.0), child: MediaQuery.withClampedTextScaling(maxScaleFactor: (isLandscapeOrientation ? 1.6 : Calendar_date_pickerLibrary._kMaxTextScaleFactor), child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 8), child: new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.AlignmentDirectional.centerEnd, child: new global::Doroti.Framework.Widgets.OverflowBar(spacing: 8, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new TextButton(style: (datePickerTheme.cancelButtonStyle ?? defaultsLocal.cancelButtonStyle), onPressed: this._handleCancel, child: new global::Doroti.Framework.Widgets.Text((((DatePickerDialog)this.widget).cancelText ?? ((useMaterial3Local ? ((MaterialLocalizations)localizations).cancelButtonLabel : ((MaterialLocalizations)localizations).cancelButtonLabel.toUpperCase())))))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new TextButton(style: (datePickerTheme.confirmButtonStyle ?? defaultsLocal.confirmButtonStyle), onPressed: this._handleOk, child: new global::Doroti.Framework.Widgets.Text(((((DatePickerDialog)this.widget).confirmText ?? (string)((MaterialLocalizations)localizations).okButtonLabel))))) }))))));
        CalendarDatePicker calendarDatePicker()
        {
            return new CalendarDatePicker(calendarDelegate: ((DatePickerDialog)this.widget).calendarDelegate, key: this._calendarPickerKey, initialDate: this._selectedDate.value, firstDate: ((DatePickerDialog)this.widget).firstDate, lastDate: ((DatePickerDialog)this.widget).lastDate, currentDate: ((DatePickerDialog)this.widget).currentDate, onDateChanged: (global::System.Action<DateTime>)this._handleDateChanged, selectableDayPredicate: (global::System.Func<DateTime, bool>?)((DatePickerDialog)this.widget).selectableDayPredicate, initialCalendarMode: ((DatePickerDialog)this.widget).initialCalendarMode);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        global::Doroti.Framework.Widgets.Form inputDatePicker()
        {
            return new global::Doroti.Framework.Widgets.Form(key: this._formKey, autovalidateMode: this._autovalidateMode.value, child: new global::Doroti.Framework.Widgets.SizedBox(height: ((object.Equals(orientationLocal, global::Doroti.Framework.Widgets.Orientation.portrait)) ? Date_pickerLibrary._inputFormPortraitHeight : Date_pickerLibrary._inputFormLandscapeHeight), child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 24), child: new global::Doroti.Framework.Widgets.Shortcuts(shortcuts: _formShortcutMap, child: new global::Doroti.Framework.Widgets.Column(mainAxisAlignment: global::Doroti.Framework.Rendering.MainAxisAlignment.center, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: MediaQuery.withClampedTextScaling(maxScaleFactor: 2.0, child: new InputDatePickerFormField(calendarDelegate: ((DatePickerDialog)this.widget).calendarDelegate, initialDate: this._selectedDate.value, firstDate: ((DatePickerDialog)this.widget).firstDate, lastDate: ((DatePickerDialog)this.widget).lastDate, onDateSubmitted: (global::System.Action<DateTime>)this._handleDateChanged, onDateSaved: (global::System.Action<DateTime>)this._handleDateChanged, selectableDayPredicate: (global::System.Func<DateTime, bool>?)((DatePickerDialog)this.widget).selectableDayPredicate, errorFormatText: ((DatePickerDialog)this.widget).errorFormatText, errorInvalidText: ((DatePickerDialog)this.widget).errorInvalidText, fieldHintText: ((DatePickerDialog)this.widget).fieldHintText, fieldLabelText: ((DatePickerDialog)this.widget).fieldLabelText, keyboardType: ((DatePickerDialog)this.widget).keyboardType, autofocus: true)))) })))));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        global::Doroti.Framework.Widgets.Widget picker = default!;
        global::Doroti.Framework.Widgets.Widget? entryModeButtonLocal = default!;
        switch (this._entryMode.value)
        {
            case DatePickerEntryMode.calendar:
                {
                    picker = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(calendarDatePicker());
                    entryModeButtonLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new IconButton(icon: (((DatePickerDialog)this.widget).switchToInputEntryModeIcon ?? new global::Doroti.Framework.Widgets.Icon((useMaterial3Local ? Icons.edit_outlined : Icons.edit))), color: headerForegroundColorLocal, tooltip: ((MaterialLocalizations)localizations).inputDateModeButtonLabel, onPressed: this._handleEntryModeToggle));
                    break;
                }
            case DatePickerEntryMode.calendarOnly:
                {
                    picker = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(calendarDatePicker());
                    entryModeButtonLocal = null;
                    break;
                }
            case DatePickerEntryMode.input:
                {
                    picker = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(inputDatePicker());
                    entryModeButtonLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new IconButton(icon: (((DatePickerDialog)this.widget).switchToCalendarEntryModeIcon ?? new global::Doroti.Framework.Widgets.Icon(Icons.calendar_today)), color: headerForegroundColorLocal, tooltip: ((MaterialLocalizations)localizations).calendarModeButtonLabel, onPressed: this._handleEntryModeToggle));
                    break;
                }
            case DatePickerEntryMode.inputOnly:
                {
                    picker = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(inputDatePicker());
                    entryModeButtonLocal = null;
                    break;
                }
        }
        global::Doroti.Framework.Widgets.Widget header = ((global::Doroti.Framework.Widgets.Widget)(object?)new _DatePickerHeader__date_picker(helpText: (((DatePickerDialog)this.widget).helpText ?? ((useMaterial3Local ? ((MaterialLocalizations)localizations).datePickerHelpText : ((MaterialLocalizations)localizations).datePickerHelpText.toUpperCase()))), titleText: ((this._selectedDate.value is null) ? "" : ((DatePickerDialog)this.widget).calendarDelegate.formatMediumDate(DartRuntimePrimitives.RequireValue(this._selectedDate.value), localizations)), titleStyle: headlineStyle, orientation: orientationLocal, isShort: (object.Equals(orientationLocal, global::Doroti.Framework.Widgets.Orientation.landscape)), entryModeButton: entryModeButtonLocal));
        double textScaleFactor = (MediaQuery.textScalerOf(context).clamp(maxScaleFactor: Calendar_date_pickerLibrary._kMaxTextScaleFactor).scale(Calendar_date_pickerLibrary._fontSizeToScale) / Calendar_date_pickerLibrary._fontSizeToScale);
        global::Doroti.Ui.Size dialogSize = ((global::Doroti.Ui.Size)(object?)(_dialogSize(context) * textScaleFactor));
        DialogThemeData dialogThemeLocal = theme.dialogTheme;
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new Dialog(backgroundColor: (datePickerTheme.backgroundColor ?? defaultsLocal.backgroundColor), elevation: (useMaterial3Local ? (datePickerTheme.elevation ?? DartRuntimePrimitives.RequireValue(defaultsLocal.elevation)) : ((datePickerTheme.elevation ?? dialogThemeLocal.elevation) ?? 24)), shadowColor: (datePickerTheme.shadowColor ?? defaultsLocal.shadowColor), surfaceTintColor: (datePickerTheme.surfaceTintColor ?? defaultsLocal.surfaceTintColor), shape: (useMaterial3Local ? (datePickerTheme.shape ?? defaultsLocal.shape) : ((datePickerTheme.shape ?? dialogThemeLocal.shape) ?? defaultsLocal.shape)), insetPadding: ((DatePickerDialog)this.widget).insetPadding, clipBehavior: Clip.antiAlias, child: new global::Doroti.Framework.Widgets.AnimatedContainer(width: dialogSize.width, height: dialogSize.height, duration: Date_pickerLibrary._dialogSizeAnimationDuration, curve: global::Doroti.Framework.Animation.Curves.easeIn, child: MediaQuery.withClampedTextScaling(maxScaleFactor: Calendar_date_pickerLibrary._kMaxTextScaleFactor, child: new global::Doroti.Framework.Widgets.LayoutBuilder(builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Rendering.BoxConstraints, global::Doroti.Framework.Widgets.Widget>)((context, constraints) =>
        {
            global::Doroti.Ui.Size portraitDialogSize = ((global::Doroti.Ui.Size)(object?)(useMaterial3Local ? Date_pickerLibrary._inputPortraitDialogSizeM3 : Date_pickerLibrary._inputPortraitDialogSizeM2));
            bool isFullyPortrait = (((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxHeight >= Math.Min(dialogSize.height, portraitDialogSize.height));
            switch (orientationLocal)
            {
                case global::Doroti.Framework.Widgets.Orientation.portrait:
                    {
                        bool isInputMode = ((object.Equals(this._entryMode.value, DatePickerEntryMode.inputOnly)) || (object.Equals(this._entryMode.value, DatePickerEntryMode.input)));
                        bool showHeader = (isFullyPortrait || !isInputMode);
                        bool showPicker = (isFullyPortrait || isInputMode);
                        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Column(mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.stretch, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection31479 = new List<global::Doroti.Framework.Widgets.Widget>(); if (showHeader) { __collection31479.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(header)); } if (useMaterial3Local) { __collection31479.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new Divider(height: 0, color: datePickerTheme.dividerColor))); } if (showPicker) { __collection31479.AddRange(new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: picker)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(actions) }); } return __collection31479; }))()));
                    }
                case global::Doroti.Framework.Widgets.Orientation.landscape:
                    {
                        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Row(mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.stretch, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection31985 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection31985.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(header)); if (useMaterial3Local) { __collection31985.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new VerticalDivider(width: 0, color: datePickerTheme.dividerColor))); } __collection31985.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: new global::Doroti.Framework.Widgets.Column(mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.stretch, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: picker)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(actions) })))); return __collection31985; }))()));
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
            throw new InvalidOperationException("Dart closure completed without a value.");
        })))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Services.RestorationBucket? bucket => this._bucket;
    public virtual void didToggleBucket(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => (this._bucket?.isReplacing != true));
    }

    public virtual void registerForRestoration(dynamic property, string restorationId)
    {
        DartRuntimePrimitives.Assert(() => ((((dynamic)property)._restorationId is null) || ((this._debugDoingRestore && (((dynamic)property)._restorationId == restorationId)))), () => (object?)$"Property is already registered under {((dynamic)property)._restorationId}.");
        DartRuntimePrimitives.Assert(() => (this._debugDoingRestore || !this._properties.Keys.map<dynamic, string?>(((r) => ((dynamic)r)._restorationId)).contains(restorationId)), () => (object?)$"\"{restorationId}\" is already registered to another property.");
        bool hasSerializedValue = (this.bucket?.contains(restorationId) ?? false);
        object? initialValue = (hasSerializedValue ? property.fromPrimitives(this.bucket!.read<object>(restorationId)) : property.createDefaultValue());
        if (!((dynamic)property).isRegistered)
        {
            property._register(restorationId, this);
            void listener()
            {
                if ((this.bucket is null))
                {
                    return;
                }
                _updateProperty(property);
            }
            property.addListener((global::System.Action)(() => listener()));
            this._properties[property] = (global::System.Action)listener;
        }
        DartRuntimePrimitives.Assert(() => (((((dynamic)property)._restorationId == restorationId) && (object.Equals(((dynamic)property)._owner, this))) && this._properties.ContainsKey(property)));
        property.initWithValue((dynamic)initialValue);
        if (((!hasSerializedValue && ((dynamic)property).enabled) && (this.bucket is not null)))
        {
            _updateProperty(property);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
    }

    public virtual void unregisterFromRestoration(dynamic property)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((dynamic)property)._owner, this)));
        this._bucket?.remove<object?>(((dynamic)property)._restorationId!);
        _unregister(property);
    }

    public virtual void didUpdateRestorationId()
    {
        if ((((this._currentParent is null) || (this._bucket?.restorationId == this.restorationId)) || this.restorePending))
        {
            return;
        }
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        DartRuntimePrimitives.Assert(() => !this.restorePending);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: this._currentParent, restorePending: false);
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket, this._bucket)));
            DartRuntimePrimitives.Assert(() => ((this._bucket is null) || (oldBucket is null)));
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
            if (this._firstRestorePending)
            {
                return true;
            }
            if ((this.restorationId is null))
            {
                return false;
            }
            global::Doroti.Framework.Services.RestorationBucket? potentialNewParent = ((global::Doroti.Framework.Services.RestorationBucket?)(object?)RestorationScope.maybeOf(this.context));
            return ((!object.Equals(potentialNewParent, this._currentParent)) && ((potentialNewParent?.isReplacing ?? false)));
            return default!;
        }
    }
    public virtual bool _debugDoingRestore => DartRuntimePrimitives.ConvertValue<bool>((this._debugPropertiesWaitingForReregistration is not null));
    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        bool needsRestore = this.restorePending;
        this._currentParent = RestorationScope.maybeOf(this.context);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: this._currentParent, restorePending: needsRestore);
        if (needsRestore)
        {
            _doRestore(oldBucket);
        }
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket, this._bucket)));
            oldBucket?.dispose();
        }
    }

    public virtual void _doRestore(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration = this._properties.Keys.ToList();
                return true;
            });
        restoreState(oldBucket, this._firstRestorePending);
        this._firstRestorePending = false;
        DartRuntimePrimitives.Assert(() =>
            {
                if (System.Linq.Enumerable.Any(this._debugPropertiesWaitingForReregistration!))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(((Func<List<global::Doroti.Framework.Foundation.DiagnosticsNode>>)(() => { var __collection41817 = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>(); __collection41817.Add(new global::Doroti.Framework.Foundation.ErrorSummary("Previously registered RestorableProperties must be re-registered in \"restoreState\".")); __collection41817.Add(new global::Doroti.Framework.Foundation.ErrorDescription($"The RestorableProperties with the following IDs were not re-registered to {this} when " + "\"restoreState\" was called:")); __collection41817.AddRange(this._debugPropertiesWaitingForReregistration!.map<dynamic, global::Doroti.Framework.Foundation.DiagnosticsNode>(((property) => new global::Doroti.Framework.Foundation.ErrorDescription($" * {((dynamic)property)._restorationId}")))); return __collection41817; }))()));
                }
                this._debugPropertiesWaitingForReregistration = null;
                return true;
            });
    }

    public virtual bool _updateBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? parent, bool restorePending)
    {
        if (((this.restorationId is null) || (parent is null)))
        {
            bool didReplace = _setNewBucketIfNecessary(newBucket: null, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (this._bucket is null));
            return didReplace;
        }
        DartRuntimePrimitives.Assert(() => (this.restorationId is not null));
        if ((restorePending || (this._bucket is null)))
        {
            global::Doroti.Framework.Services.RestorationBucket newBucketLocal = ((global::Doroti.Framework.Services.RestorationBucket)(object?)parent.claimChild(this.restorationId!, debugOwner: this));
            bool didReplaceLocal = _setNewBucketIfNecessary(newBucket: newBucketLocal, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (object.Equals(this._bucket, newBucketLocal)));
            return didReplaceLocal;
        }
        DartRuntimePrimitives.Assert(() => (this._bucket is not null));
        DartRuntimePrimitives.Assert(() => !restorePending);
        this._bucket!.rename(this.restorationId!);
        parent.adoptChild(this._bucket!);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _setNewBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? newBucket, bool restorePending)
    {
        if ((object.Equals(newBucket, this._bucket)))
        {
            return false;
        }
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        this._bucket = newBucket;
        if (!restorePending)
        {
            if ((this._bucket is not null))
            {
                this._properties.Keys.forEach((__arg0) => ((global::System.Action<dynamic>)this._updateProperty)(__arg0));
            }
            didToggleBucket(oldBucket);
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _updateProperty(dynamic property)
    {
        if (((dynamic)property).enabled)
        {
            this._bucket?.write(((dynamic)property)._restorationId!, property.toPrimitives());
        }
        else
        {
            this._bucket?.remove<object>(((dynamic)property)._restorationId!);
        }
    }

    public virtual void _unregister(dynamic property)
    {
        global::System.Action listener = this._properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
        property.removeListener((global::System.Action)(() => listener()));
        property._unregister();
    }

}

internal class _RestorableDatePickerEntryMode__date_picker : global::Doroti.Framework.Widgets.RestorableValue<DatePickerEntryMode>
{
    internal virtual DatePickerEntryMode _defaultValue { get; private set; } = default!;

    internal _RestorableDatePickerEntryMode__date_picker(DatePickerEntryMode defaultValue)
    {
        this._defaultValue = defaultValue;
    }

    public override DatePickerEntryMode createDefaultValue() => this._defaultValue;
    public override void didUpdateValue(DatePickerEntryMode oldValue)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Services.RestorationLibrary.debugIsSerializableForRestoration(FoundationRuntimePorts.EnumIndex(this.value)));
        notifyListeners();
    }

    public override DatePickerEntryMode fromPrimitives(object? data) => System.Enum.GetValues<DatePickerEntryMode>().ToList()[(int)(((long)data!))];
    public override object? toPrimitives() => FoundationRuntimePorts.EnumIndex(this.value);
}

internal class _RestorableAutovalidateMode__date_picker : global::Doroti.Framework.Widgets.RestorableValue<global::Doroti.Framework.Widgets.AutovalidateMode>
{
    internal virtual global::Doroti.Framework.Widgets.AutovalidateMode _defaultValue { get; private set; } = default!;

    internal _RestorableAutovalidateMode__date_picker(global::Doroti.Framework.Widgets.AutovalidateMode defaultValue)
    {
        this._defaultValue = defaultValue;
    }

    public override global::Doroti.Framework.Widgets.AutovalidateMode createDefaultValue() => this._defaultValue;
    public override void didUpdateValue(global::Doroti.Framework.Widgets.AutovalidateMode oldValue)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Services.RestorationLibrary.debugIsSerializableForRestoration(FoundationRuntimePorts.EnumIndex(this.value)));
        notifyListeners();
    }

    public override global::Doroti.Framework.Widgets.AutovalidateMode fromPrimitives(object? data) => System.Enum.GetValues<global::Doroti.Framework.Widgets.AutovalidateMode>().ToList()[(int)(((long)data!))];
    public override object? toPrimitives() => FoundationRuntimePorts.EnumIndex(this.value);
}

internal class _DatePickerHeader__date_picker : global::Doroti.Framework.Widgets.StatelessWidget
{
    internal const double _datePickerHeaderLandscapeWidth = 152.0;
    internal const double _datePickerHeaderPortraitHeight = 120.0;
    internal const double _headerPaddingLandscape = 16.0;
    public virtual string helpText { get; private set; } = default!;
    public virtual string titleText { get; private set; } = default!;
    public virtual string? titleSemanticsLabel { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? titleStyle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Orientation orientation { get; private set; } = default!;
    public virtual bool isShort { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? entryModeButton { get; private set; }

    internal _DatePickerHeader__date_picker(string helpText, string titleText, string? titleSemanticsLabel = null, global::Doroti.Framework.Painting.TextStyle? titleStyle = default!, global::Doroti.Framework.Widgets.Orientation orientation = default!, bool isShort = false, global::Doroti.Framework.Widgets.Widget? entryModeButton = null)
    {
        this.helpText = helpText;
        this.titleText = titleText;
        this.titleSemanticsLabel = titleSemanticsLabel;
        this.titleStyle = titleStyle;
        this.orientation = orientation;
        this.isShort = isShort;
        this.entryModeButton = entryModeButton;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        DatePickerThemeData datePickerTheme = DatePickerTheme.of(context);
        DatePickerThemeData defaultsLocal = DatePickerTheme.defaults(context);
        global::Doroti.Ui.Color? backgroundColor = ((global::Doroti.Ui.Color?)(object?)(datePickerTheme.headerBackgroundColor ?? defaultsLocal.headerBackgroundColor));
        global::Doroti.Ui.Color? foregroundColor = ((global::Doroti.Ui.Color?)(object?)(datePickerTheme.headerForegroundColor ?? defaultsLocal.headerForegroundColor));
        global::Doroti.Framework.Painting.TextStyle? helpStyle = ((global::Doroti.Framework.Painting.TextStyle?)(object?)((datePickerTheme.headerHelpStyle ?? defaultsLocal.headerHelpStyle))?.copyWith(color: foregroundColor));
        double currentScale = (MediaQuery.textScalerOf(context).scale(Calendar_date_pickerLibrary._fontSizeToScale) / Calendar_date_pickerLibrary._fontSizeToScale);
        double maxHeaderTextScaleFactor = Math.Min(currentScale, ((this.entryModeButton is not null) ? Date_pickerLibrary._kMaxHeaderWithEntryTextScaleFactor : Date_pickerLibrary._kMaxHeaderTextScaleFactor));
        double textScaleFactor = (MediaQuery.textScalerOf(context).clamp(maxScaleFactor: maxHeaderTextScaleFactor).scale(Calendar_date_pickerLibrary._fontSizeToScale) / Calendar_date_pickerLibrary._fontSizeToScale);
        double scaledFontSize = MediaQuery.textScalerOf(context).scale((this.titleStyle?.fontSize ?? 32));
        var headerScaleFactor = ((textScaleFactor > 1L) ? textScaleFactor : 1.0);
        var help = new global::Doroti.Framework.Widgets.Text(this.helpText, style: helpStyle, maxLines: 1L, overflow: global::Doroti.Framework.Painting.TextOverflow.ellipsis, textScaler: MediaQuery.textScalerOf(context).clamp(maxScaleFactor: Math.Min(textScaleFactor, ((object.Equals(this.orientation, global::Doroti.Framework.Widgets.Orientation.portrait)) ? Date_pickerLibrary._kMaxHelpPortraitTextScaleFactor : Date_pickerLibrary._kMaxHelpLandscapeTextScaleFactor))));
        var title = new global::Doroti.Framework.Widgets.Text(this.titleText, semanticsLabel: (this.titleSemanticsLabel ?? this.titleText), style: this.titleStyle, maxLines: ((object.Equals(this.orientation, global::Doroti.Framework.Widgets.Orientation.portrait)) ? (((scaledFontSize > 70L) ? 2L : 1L)) : ((scaledFontSize > 40L) ? 3L : 2L)), overflow: global::Doroti.Framework.Painting.TextOverflow.ellipsis, textScaler: MediaQuery.textScalerOf(context).clamp(maxScaleFactor: textScaleFactor));
        double fontScaleAdjustedHeaderHeight = ((headerScaleFactor > 1.3) ? (headerScaleFactor - 0.2) : 1.0);
        switch (this.orientation)
        {
            case global::Doroti.Framework.Widgets.Orientation.portrait:
                {
                    return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(container: true, child: new global::Doroti.Framework.Widgets.SizedBox(height: (_datePickerHeaderPortraitHeight * fontScaleAdjustedHeaderHeight), child: new Material(color: backgroundColor, child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: 24, end: 12, bottom: 12), child: new global::Doroti.Framework.Widgets.Column(crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.start, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(height: 16)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(help), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: new global::Doroti.Framework.Widgets.SizedBox(height: 38))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Row(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection38913 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection38913.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: title))); if ((this.entryModeButton is not null)) { __collection38913.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(container: true, child: this.entryModeButton))); } return __collection38913; }))())) }))))));
                }
            case global::Doroti.Framework.Widgets.Orientation.landscape:
                {
                    return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(container: true, child: new global::Doroti.Framework.Widgets.SizedBox(width: _datePickerHeaderLandscapeWidth, child: new Material(color: backgroundColor, child: new global::Doroti.Framework.Widgets.Column(crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.start, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection39596 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection39596.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(height: 16))); __collection39596.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: _headerPaddingLandscape), child: help))); __collection39596.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(height: (this.isShort ? 16 : 56)))); __collection39596.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: _headerPaddingLandscape), child: title)))); if ((this.entryModeButton is not null)) { __collection39596.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: (theme.useMaterial3 ? global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: 8.0, end: 4.0, bottom: 6.0) : global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 4)), child: new global::Doroti.Framework.Widgets.Semantics(container: true, child: this.entryModeButton)))); } return __collection39596; }))())))));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public delegate bool SelectableDayForRangePredicate(DateTime day, DateTime? selectedStartDay, DateTime? selectedEndDay);

public static partial class Date_pickerLibrary
{
    public static async Future<DateTimeRange<DateTime>?> showDateRangePicker(global::Doroti.Framework.Widgets.BuildContext context, DateTimeRange<DateTime>? initialDateRange = null, DateTime firstDate = default!, DateTime lastDate = default!, DateTime? currentDate = null, DatePickerEntryMode initialEntryMode = DatePickerEntryMode.calendar, string? helpText = null, string? cancelText = null, string? confirmText = null, string? saveText = null, string? errorFormatText = null, string? errorInvalidText = null, string? errorInvalidRangeText = null, string? fieldStartHintText = null, string? fieldEndHintText = null, string? fieldStartLabelText = null, string? fieldEndLabelText = null, Locale? locale = null, bool barrierDismissible = true, Color? barrierColor = null, string? barrierLabel = null, bool useRootNavigator = true, global::Doroti.Framework.Widgets.RouteSettings? routeSettings = null, TextDirection? textDirection = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>? builder = null, Offset? anchorPoint = null, global::Doroti.Framework.Services.TextInputType keyboardType = default!, global::Doroti.Framework.Widgets.Icon? switchToInputEntryModeIcon = null, global::Doroti.Framework.Widgets.Icon? switchToCalendarEntryModeIcon = null, global::System.Func<DateTime, DateTime?, DateTime?, bool>? selectableDayPredicate = null, CalendarDelegate<DateTime> calendarDelegate = default!)
    {
        initialDateRange = ((initialDateRange is null) ? null : calendarDelegate.datesOnly(initialDateRange));
        firstDate = calendarDelegate.dateOnly(firstDate);
        lastDate = calendarDelegate.dateOnly(lastDate);
        DartRuntimePrimitives.Assert(() => !lastDate.isBefore(firstDate), () => (object?)$"lastDate {lastDate} must be on or after firstDate {firstDate}.");
        DartRuntimePrimitives.Assert(() => ((initialDateRange is null) || !((DateTimeRange<DateTime>)initialDateRange).start.isBefore(firstDate)), () => (object?)$"initialDateRange's start date must be on or after firstDate {firstDate}.");
        DartRuntimePrimitives.Assert(() => ((initialDateRange is null) || !((DateTimeRange<DateTime>)initialDateRange).end.isBefore(firstDate)), () => (object?)$"initialDateRange's end date must be on or after firstDate {firstDate}.");
        DartRuntimePrimitives.Assert(() => ((initialDateRange is null) || !((DateTimeRange<DateTime>)initialDateRange).start.isAfter(lastDate)), () => (object?)$"initialDateRange's start date must be on or before lastDate {lastDate}.");
        DartRuntimePrimitives.Assert(() => ((initialDateRange is null) || !((DateTimeRange<DateTime>)initialDateRange).end.isAfter(lastDate)), () => (object?)$"initialDateRange's end date must be on or before lastDate {lastDate}.");
        DartRuntimePrimitives.Assert(() => (((initialDateRange is null) || (selectableDayPredicate is null)) || selectableDayPredicate(((DateTimeRange<DateTime>)initialDateRange).start, ((DateTimeRange<DateTime>)initialDateRange).start, ((DateTimeRange<DateTime>)initialDateRange).end)), () => (object?)"initialDateRange's start date must be selectable.");
        DartRuntimePrimitives.Assert(() => (((initialDateRange is null) || (selectableDayPredicate is null)) || selectableDayPredicate(((DateTimeRange<DateTime>)initialDateRange).end, ((DateTimeRange<DateTime>)initialDateRange).start, ((DateTimeRange<DateTime>)initialDateRange).end)), () => (object?)"initialDateRange's end date must be selectable.");
        currentDate = calendarDelegate.dateOnly(((currentDate ?? (DateTime)calendarDelegate.now())));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        global::Doroti.Framework.Widgets.Widget dialog = ((global::Doroti.Framework.Widgets.Widget)(object?)new DateRangePickerDialog(initialDateRange: initialDateRange, firstDate: firstDate, lastDate: lastDate, currentDate: DartRuntimePrimitives.RequireValue(currentDate), selectableDayPredicate: (global::System.Func<DateTime, DateTime?, DateTime?, bool>?)selectableDayPredicate, initialEntryMode: initialEntryMode, helpText: helpText, cancelText: cancelText, confirmText: confirmText, saveText: saveText, errorFormatText: errorFormatText, errorInvalidText: errorInvalidText, errorInvalidRangeText: errorInvalidRangeText, fieldStartHintText: fieldStartHintText, fieldEndHintText: fieldEndHintText, fieldStartLabelText: fieldStartLabelText, fieldEndLabelText: fieldEndLabelText, keyboardType: keyboardType, switchToInputEntryModeIcon: switchToInputEntryModeIcon, switchToCalendarEntryModeIcon: switchToCalendarEntryModeIcon, calendarDelegate: calendarDelegate));
        if ((textDirection is not null))
        {
            TextDirection textDirection__value49942 = DartRuntimePrimitives.RequireValue(textDirection);
            dialog = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Directionality(textDirection: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(textDirection__value49942)), child: dialog));
        }
        if ((locale is not null))
        {
            Locale locale__value50052 = DartRuntimePrimitives.RequireValue(locale);
            dialog = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(global::Doroti.Framework.Widgets.Localizations.CreateOverride(context: context, locale: DartRuntimePrimitives.RequireValue(locale__value50052), child: dialog));
        }
        return await DialogLibrary.showDialog<DateTimeRange<DateTime>>(context: context, barrierDismissible: barrierDismissible, barrierColor: barrierColor, barrierLabel: barrierLabel, useRootNavigator: useRootNavigator, routeSettings: routeSettings, useSafeArea: false, builder: ((context) =>
        {
            return ((builder is null) ? dialog : builder(context, dialog));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }), anchorPoint: anchorPoint);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Date_pickerLibrary
{
    internal static string _formatRangeStartDate(MaterialLocalizations localizations, CalendarDelegate<DateTime> calendarDelegate, DateTime? startDate, DateTime? endDate)
    {
        return ((startDate is null) ? ((MaterialLocalizations)localizations).dateRangeStartLabel : ((((endDate is null) || (DartRuntimePrimitives.RequireValue(startDate).Year == DartRuntimePrimitives.RequireValue(endDate).Year))) ? calendarDelegate.formatShortMonthDay(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(startDate)), localizations) : calendarDelegate.formatShortDate(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(startDate)), localizations)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Date_pickerLibrary
{
    internal static string _formatRangeEndDate(MaterialLocalizations localizations, CalendarDelegate<DateTime> calendarDelegate, DateTime? startDate, DateTime? endDate, DateTime currentDate)
    {
        return ((endDate is null) ? ((MaterialLocalizations)localizations).dateRangeEndLabel : (((((startDate is not null) && (DartRuntimePrimitives.RequireValue(startDate).Year == DartRuntimePrimitives.RequireValue(endDate).Year)) && (DartRuntimePrimitives.RequireValue(startDate).Year == currentDate.Year))) ? calendarDelegate.formatShortMonthDay(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(endDate)), localizations) : calendarDelegate.formatShortDate(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(endDate)), localizations)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class DateRangePickerDialog : global::Doroti.Framework.Widgets.StatefulWidget
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
    public virtual global::Doroti.Framework.Services.TextInputType keyboardType { get; private set; } = default!;
    public virtual string? restorationId { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Icon? switchToInputEntryModeIcon { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Icon? switchToCalendarEntryModeIcon { get; private set; }
    public virtual global::System.Func<DateTime, DateTime?, DateTime?, bool>? selectableDayPredicate { get; private set; }
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    public DateRangePickerDialog(global::Doroti.Framework.Foundation.Key? key = null, DateTimeRange<DateTime>? initialDateRange = null, DateTime firstDate = default!, DateTime lastDate = default!, DateTime? currentDate = null, DatePickerEntryMode initialEntryMode = DatePickerEntryMode.calendar, string? helpText = null, string? cancelText = null, string? confirmText = null, string? saveText = null, string? errorInvalidRangeText = null, string? errorFormatText = null, string? errorInvalidText = null, string? fieldStartHintText = null, string? fieldEndHintText = null, string? fieldStartLabelText = null, string? fieldEndLabelText = null, global::Doroti.Framework.Services.TextInputType keyboardType = default!, string? restorationId = null, global::Doroti.Framework.Widgets.Icon? switchToInputEntryModeIcon = null, global::Doroti.Framework.Widgets.Icon? switchToCalendarEntryModeIcon = null, global::System.Func<DateTime, DateTime?, DateTime?, bool>? selectableDayPredicate = null, CalendarDelegate<DateTime> calendarDelegate = default!) : base(key: key)
    {
        global::Doroti.Framework.Services.TextInputType __keyboardType = keyboardType ?? global::Doroti.Framework.Services.TextInputType.datetime;
        CalendarDelegate<DateTime> __calendarDelegate = calendarDelegate ?? new GregorianCalendarDelegate();
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
        this._currentDate = currentDate;
    }

    public virtual DateTime currentDate
    {
        get
        {
            return this.calendarDelegate.dateOnly(((this._currentDate ?? (DateTime)this.calendarDelegate.now())));
            return default!;
        }
    }
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DateRangePickerDialogState__date_picker());
}

internal class _DateRangePickerDialogState__date_picker : global::Doroti.Framework.Widgets.State<DateRangePickerDialog>, global::Doroti.Framework.Widgets.RestorationMixin<DateRangePickerDialog>
{
    private bool __late__entryMode_initialized;
    private _RestorableDatePickerEntryMode__date_picker __late__entryMode = default!;
    internal virtual _RestorableDatePickerEntryMode__date_picker _entryMode
    {
        get
        {
            if (!__late__entryMode_initialized)
            {
                __late__entryMode = new _RestorableDatePickerEntryMode__date_picker(((DateRangePickerDialog)this.widget).initialEntryMode);
                __late__entryMode_initialized = true;
            }
            return __late__entryMode;
        }
    }
    private bool __late__selectedStart_initialized;
    private global::Doroti.Framework.Widgets.RestorableDateTimeN __late__selectedStart = default!;
    internal virtual global::Doroti.Framework.Widgets.RestorableDateTimeN _selectedStart
    {
        get
        {
            if (!__late__selectedStart_initialized)
            {
                __late__selectedStart = new global::Doroti.Framework.Widgets.RestorableDateTimeN(((DateRangePickerDialog)this.widget).initialDateRange?.start);
                __late__selectedStart_initialized = true;
            }
            return __late__selectedStart;
        }
    }
    private bool __late__selectedEnd_initialized;
    private global::Doroti.Framework.Widgets.RestorableDateTimeN __late__selectedEnd = default!;
    internal virtual global::Doroti.Framework.Widgets.RestorableDateTimeN _selectedEnd
    {
        get
        {
            if (!__late__selectedEnd_initialized)
            {
                __late__selectedEnd = new global::Doroti.Framework.Widgets.RestorableDateTimeN(((DateRangePickerDialog)this.widget).initialDateRange?.end);
                __late__selectedEnd_initialized = true;
            }
            return __late__selectedEnd;
        }
    }
    internal virtual global::Doroti.Framework.Widgets.RestorableBool _autoValidate { get; private set; } = new global::Doroti.Framework.Widgets.RestorableBool(false);
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _calendarPickerKey { get; private set; } = global::Doroti.Framework.Widgets.GlobalKey<IState>.Create();
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<_InputDateRangePickerState__date_picker> _inputPickerKey { get; private set; } = global::Doroti.Framework.Widgets.GlobalKey<_InputDateRangePickerState__date_picker>.Create();
    public virtual global::Doroti.Framework.Services.RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<dynamic, global::System.Action> _properties { get; set; } = new DartMap<dynamic, global::System.Action>();
    public virtual List<dynamic>? _debugPropertiesWaitingForReregistration { get; set; } = default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual global::Doroti.Framework.Services.RestorationBucket? _currentParent { get; set; } = default;

    public virtual string? restorationId => ((DateRangePickerDialog)this.widget).restorationId;
    public virtual void restoreState(global::Doroti.Framework.Services.RestorationBucket? oldBucket, bool initialRestore)
    {
        registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this._entryMode), "entry_mode");
        registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this._selectedStart), "selected_start");
        registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this._selectedEnd), "selected_end");
        registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this._autoValidate), "autovalidate");
    }

    public override void dispose()
    {
        this._entryMode.dispose();
        this._selectedStart.dispose();
        this._selectedEnd.dispose();
        this._autoValidate.dispose();
        this._properties.forEach(((global::System.Action<dynamic, global::System.Action>)((property, listener) =>
        {
            if (!((dynamic)property)._disposed)
            {
                property.removeListener((global::System.Action)(() => listener()));
            }
        })));
        this._bucket?.dispose();
        _bucket = null;
        base.dispose();
    }

    internal virtual void _handleOk()
    {
        if (((object.Equals(this._entryMode.value, DatePickerEntryMode.input)) || (object.Equals(this._entryMode.value, DatePickerEntryMode.inputOnly))))
        {
            _InputDateRangePickerState__date_picker picker = ((global::Doroti.Framework.Widgets.GlobalKey<_InputDateRangePickerState__date_picker>)this._inputPickerKey).currentState!;
            if (!picker.validate())
            {
                setState(((global::System.Action)(() =>
                {
                    this._autoValidate.value = true;
                })));
                return;
            }
        }
        DateTimeRange<DateTime>? selectedRange = (this._hasSelectedDateRange ? new DateTimeRange<DateTime>(start: DartRuntimePrimitives.RequireValue(this._selectedStart.value), end: DartRuntimePrimitives.RequireValue(this._selectedEnd.value)) : null);
        Navigator.pop<object>(this.context, selectedRange);
    }

    internal virtual void _handleCancel()
    {
        Navigator.pop<object>(this.context);
    }

    internal virtual void _handleEntryModeToggle()
    {
        setState(((global::System.Action)(() =>
        {
            switch (this._entryMode.value)
            {
                case DatePickerEntryMode.calendar:
                    {
                        this._autoValidate.value = false;
                        this._entryMode.value = DatePickerEntryMode.input;
                        break;
                    }
                case DatePickerEntryMode.input:
                    {
                        if ((((this._selectedStart.value is not null) && (this._selectedEnd.value is not null)) && DartRuntimePrimitives.RequireValue(this._selectedStart.value).isAfter(DartRuntimePrimitives.RequireValue(this._selectedEnd.value))))
                        {
                            this._selectedEnd.value = null;
                        }
                        if (((this._selectedStart.value is not null) && !_isDaySelectable(DartRuntimePrimitives.RequireValue(this._selectedStart.value))))
                        {
                            this._selectedStart.value = null;
                            this._selectedEnd.value = null;
                        }
                        else
                        {
                            if (((this._selectedEnd.value is not null) && !_isDaySelectable(DartRuntimePrimitives.RequireValue(this._selectedEnd.value))))
                            {
                                this._selectedEnd.value = null;
                            }
                        }
                        this._entryMode.value = DatePickerEntryMode.calendar;
                        break;
                    }
                case DatePickerEntryMode.calendarOnly:
                case DatePickerEntryMode.inputOnly:
                    {
                        DartRuntimePrimitives.Assert(() => false, () => (object?)$"Can not change entry mode from {this._entryMode}");
                        break;
                    }
            }
        })));
    }

    internal virtual bool _isDaySelectable(DateTime day)
    {
        if ((day.isBefore(((DateRangePickerDialog)this.widget).firstDate) || day.isAfter(((DateRangePickerDialog)this.widget).lastDate)))
        {
            return false;
        }
        if ((((DateRangePickerDialog)this.widget).selectableDayPredicate is null))
        {
            return true;
        }
        return ((DateRangePickerDialog)this.widget).selectableDayPredicate!(day, this._selectedStart.value, this._selectedEnd.value);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleStartDateChanged(DateTime? date)
    {
        setState(((global::System.Action)(() => { _ = this._selectedStart.value = date; })));
    }

    internal virtual void _handleEndDateChanged(DateTime? date)
    {
        setState(((global::System.Action)(() => { _ = this._selectedEnd.value = date; })));
    }

    internal virtual bool _hasSelectedDateRange => DartRuntimePrimitives.ConvertValue<bool>(((this._selectedStart.value is not null) && (this._selectedEnd.value is not null)));
    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        bool useMaterial3Local = theme.useMaterial3;
        global::Doroti.Framework.Widgets.Orientation orientation = MediaQuery.orientationOf(context);
        MaterialLocalizations localizations = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        DatePickerThemeData datePickerTheme = DatePickerTheme.of(context);
        DatePickerThemeData defaultsLocal = DatePickerTheme.defaults(context);
        global::Doroti.Framework.Widgets.Widget contents = default!;
        global::Doroti.Ui.Size size = default!;
        double? elevationLocal = default!;
        global::Doroti.Ui.Color? shadowColorLocal = default!;
        global::Doroti.Ui.Color? surfaceTintColorLocal = default!;
        global::Doroti.Framework.Painting.ShapeBorder? shapeLocal = default!;
        global::Doroti.Framework.Painting.EdgeInsets insetPaddingLocal = default!;
        bool showEntryModeButton = ((object.Equals(this._entryMode.value, DatePickerEntryMode.calendar)) || (object.Equals(this._entryMode.value, DatePickerEntryMode.input)));
        switch (this._entryMode.value)
        {
            case DatePickerEntryMode.calendar:
            case DatePickerEntryMode.calendarOnly:
                {
                    contents = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _CalendarRangePickerDialog__date_picker(key: this._calendarPickerKey, calendarDelegate: ((DateRangePickerDialog)this.widget).calendarDelegate, selectedStartDate: this._selectedStart.value, selectedEndDate: this._selectedEnd.value, firstDate: ((DateRangePickerDialog)this.widget).firstDate, lastDate: ((DateRangePickerDialog)this.widget).lastDate, selectableDayPredicate: (global::System.Func<DateTime, DateTime?, DateTime?, bool>?)((DateRangePickerDialog)this.widget).selectableDayPredicate, currentDate: ((DateRangePickerDialog)this.widget).currentDate, onStartDateChanged: (__arg0) => ((global::System.Action<DateTime?>)this._handleStartDateChanged)(DartRuntimePrimitives.ConvertValue<DateTime>(__arg0)), onEndDateChanged: (global::System.Action<DateTime?>)this._handleEndDateChanged, onConfirm: ((global::System.Action)(this._hasSelectedDateRange ? this._handleOk : null)), onCancel: () => this._handleCancel(), entryModeButton: (showEntryModeButton ? new IconButton(icon: (((DateRangePickerDialog)this.widget).switchToInputEntryModeIcon ?? new global::Doroti.Framework.Widgets.Icon((useMaterial3Local ? Icons.edit_outlined : Icons.edit))), padding: global::Doroti.Framework.Painting.EdgeInsets.zero, tooltip: ((MaterialLocalizations)localizations).inputDateModeButtonLabel, onPressed: this._handleEntryModeToggle) : null), confirmText: (((DateRangePickerDialog)this.widget).saveText ?? ((useMaterial3Local ? ((MaterialLocalizations)localizations).saveButtonLabel : ((MaterialLocalizations)localizations).saveButtonLabel.toUpperCase()))), helpText: (((DateRangePickerDialog)this.widget).helpText ?? ((useMaterial3Local ? ((MaterialLocalizations)localizations).dateRangePickerHelpText : ((MaterialLocalizations)localizations).dateRangePickerHelpText.toUpperCase())))));
                    size = MediaQuery.sizeOf(context);
                    insetPaddingLocal = global::Doroti.Framework.Painting.EdgeInsets.zero;
                    elevationLocal = (datePickerTheme.rangePickerElevation ?? DartRuntimePrimitives.RequireValue(defaultsLocal.rangePickerElevation));
                    shadowColorLocal = (datePickerTheme.rangePickerShadowColor ?? defaultsLocal.rangePickerShadowColor!);
                    surfaceTintColorLocal = (datePickerTheme.rangePickerSurfaceTintColor ?? defaultsLocal.rangePickerSurfaceTintColor!);
                    shapeLocal = (datePickerTheme.rangePickerShape ?? defaultsLocal.rangePickerShape);
                    break;
                }
            case DatePickerEntryMode.input:
            case DatePickerEntryMode.inputOnly:
                {
                    contents = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _InputDateRangePickerDialog__date_picker(calendarDelegate: ((DateRangePickerDialog)this.widget).calendarDelegate, selectedStartDate: this._selectedStart.value, selectedEndDate: this._selectedEnd.value, currentDate: ((DateRangePickerDialog)this.widget).currentDate, picker: new global::Doroti.Framework.Widgets.SizedBox(height: ((object.Equals(orientation, global::Doroti.Framework.Widgets.Orientation.portrait)) ? Date_pickerLibrary._inputFormPortraitHeight : Date_pickerLibrary._inputFormLandscapeHeight), child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 24), child: new global::Doroti.Framework.Widgets.Column(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Spacer()), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _InputDateRangePicker__date_picker(key: this._inputPickerKey, calendarDelegate: ((DateRangePickerDialog)this.widget).calendarDelegate, initialStartDate: this._selectedStart.value, initialEndDate: this._selectedEnd.value, firstDate: ((DateRangePickerDialog)this.widget).firstDate, lastDate: ((DateRangePickerDialog)this.widget).lastDate, selectableDayPredicate: (global::System.Func<DateTime, DateTime?, DateTime?, bool>?)((DateRangePickerDialog)this.widget).selectableDayPredicate, onStartDateChanged: (global::System.Action<DateTime?>)this._handleStartDateChanged, onEndDateChanged: (global::System.Action<DateTime?>)this._handleEndDateChanged, autofocus: true, autovalidate: DartRuntimePrimitives.RequireValue(this._autoValidate.value), helpText: ((DateRangePickerDialog)this.widget).helpText, errorInvalidRangeText: ((DateRangePickerDialog)this.widget).errorInvalidRangeText, errorFormatText: ((DateRangePickerDialog)this.widget).errorFormatText, errorInvalidText: ((DateRangePickerDialog)this.widget).errorInvalidText, fieldStartHintText: ((DateRangePickerDialog)this.widget).fieldStartHintText, fieldEndHintText: ((DateRangePickerDialog)this.widget).fieldEndHintText, fieldStartLabelText: ((DateRangePickerDialog)this.widget).fieldStartLabelText, fieldEndLabelText: ((DateRangePickerDialog)this.widget).fieldEndLabelText, keyboardType: ((DateRangePickerDialog)this.widget).keyboardType)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Spacer()) }))), onConfirm: () => this._handleOk(), onCancel: () => this._handleCancel(), entryModeButton: (showEntryModeButton ? new IconButton(icon: (((DateRangePickerDialog)this.widget).switchToCalendarEntryModeIcon ?? new global::Doroti.Framework.Widgets.Icon(Icons.calendar_today)), padding: global::Doroti.Framework.Painting.EdgeInsets.zero, tooltip: ((MaterialLocalizations)localizations).calendarModeButtonLabel, onPressed: this._handleEntryModeToggle) : null), confirmText: ((((DateRangePickerDialog)this.widget).confirmText ?? (string)((MaterialLocalizations)localizations).okButtonLabel)), cancelText: (((DateRangePickerDialog)this.widget).cancelText ?? ((useMaterial3Local ? ((MaterialLocalizations)localizations).cancelButtonLabel : ((MaterialLocalizations)localizations).cancelButtonLabel.toUpperCase()))), helpText: (((DateRangePickerDialog)this.widget).helpText ?? ((useMaterial3Local ? ((MaterialLocalizations)localizations).dateRangePickerHelpText : ((MaterialLocalizations)localizations).dateRangePickerHelpText.toUpperCase())))));
                    DialogThemeData dialogThemeLocal = theme.dialogTheme;
                    size = ((object.Equals(orientation, global::Doroti.Framework.Widgets.Orientation.portrait)) ? ((useMaterial3Local ? Date_pickerLibrary._inputPortraitDialogSizeM3 : Date_pickerLibrary._inputPortraitDialogSizeM2)) : Date_pickerLibrary._inputRangeLandscapeDialogSize);
                    elevationLocal = (useMaterial3Local ? (datePickerTheme.elevation ?? DartRuntimePrimitives.RequireValue(defaultsLocal.elevation)) : ((datePickerTheme.elevation ?? dialogThemeLocal.elevation) ?? 24));
                    shadowColorLocal = (datePickerTheme.shadowColor ?? defaultsLocal.shadowColor);
                    surfaceTintColorLocal = (datePickerTheme.surfaceTintColor ?? defaultsLocal.surfaceTintColor);
                    shapeLocal = (useMaterial3Local ? (datePickerTheme.shape ?? defaultsLocal.shape) : ((datePickerTheme.shape ?? dialogThemeLocal.shape) ?? defaultsLocal.shape));
                    insetPaddingLocal = global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 16.0, vertical: 24.0);
                    break;
                }
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new Dialog(insetPadding: insetPaddingLocal, backgroundColor: (datePickerTheme.backgroundColor ?? defaultsLocal.backgroundColor), elevation: elevationLocal, shadowColor: shadowColorLocal, surfaceTintColor: surfaceTintColorLocal, shape: shapeLocal, clipBehavior: Clip.antiAlias, child: new global::Doroti.Framework.Widgets.AnimatedContainer(width: size.width, height: size.height, duration: Date_pickerLibrary._dialogSizeAnimationDuration, curve: global::Doroti.Framework.Animation.Curves.easeIn, child: MediaQuery.withClampedTextScaling(maxScaleFactor: Date_pickerLibrary._kMaxRangeTextScaleFactor, child: new global::Doroti.Framework.Widgets.Builder(builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)((context) =>
        {
            return contents;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Services.RestorationBucket? bucket => this._bucket;
    public virtual void didToggleBucket(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => (this._bucket?.isReplacing != true));
    }

    public virtual void registerForRestoration(dynamic property, string restorationId)
    {
        DartRuntimePrimitives.Assert(() => ((((dynamic)property)._restorationId is null) || ((this._debugDoingRestore && (((dynamic)property)._restorationId == restorationId)))), () => (object?)$"Property is already registered under {((dynamic)property)._restorationId}.");
        DartRuntimePrimitives.Assert(() => (this._debugDoingRestore || !this._properties.Keys.map<dynamic, string?>(((r) => ((dynamic)r)._restorationId)).contains(restorationId)), () => (object?)$"\"{restorationId}\" is already registered to another property.");
        bool hasSerializedValue = (this.bucket?.contains(restorationId) ?? false);
        object? initialValue = (hasSerializedValue ? property.fromPrimitives(this.bucket!.read<object>(restorationId)) : property.createDefaultValue());
        if (!((dynamic)property).isRegistered)
        {
            property._register(restorationId, this);
            void listener()
            {
                if ((this.bucket is null))
                {
                    return;
                }
                _updateProperty(property);
            }
            property.addListener((global::System.Action)(() => listener()));
            this._properties[property] = (global::System.Action)listener;
        }
        DartRuntimePrimitives.Assert(() => (((((dynamic)property)._restorationId == restorationId) && (object.Equals(((dynamic)property)._owner, this))) && this._properties.ContainsKey(property)));
        property.initWithValue((dynamic)initialValue);
        if (((!hasSerializedValue && ((dynamic)property).enabled) && (this.bucket is not null)))
        {
            _updateProperty(property);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
    }

    public virtual void unregisterFromRestoration(dynamic property)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((dynamic)property)._owner, this)));
        this._bucket?.remove<object?>(((dynamic)property)._restorationId!);
        _unregister(property);
    }

    public virtual void didUpdateRestorationId()
    {
        if ((((this._currentParent is null) || (this._bucket?.restorationId == this.restorationId)) || this.restorePending))
        {
            return;
        }
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        DartRuntimePrimitives.Assert(() => !this.restorePending);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: this._currentParent, restorePending: false);
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket, this._bucket)));
            DartRuntimePrimitives.Assert(() => ((this._bucket is null) || (oldBucket is null)));
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
            if (this._firstRestorePending)
            {
                return true;
            }
            if ((this.restorationId is null))
            {
                return false;
            }
            global::Doroti.Framework.Services.RestorationBucket? potentialNewParent = ((global::Doroti.Framework.Services.RestorationBucket?)(object?)RestorationScope.maybeOf(this.context));
            return ((!object.Equals(potentialNewParent, this._currentParent)) && ((potentialNewParent?.isReplacing ?? false)));
            return default!;
        }
    }
    public virtual bool _debugDoingRestore => DartRuntimePrimitives.ConvertValue<bool>((this._debugPropertiesWaitingForReregistration is not null));
    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        bool needsRestore = this.restorePending;
        this._currentParent = RestorationScope.maybeOf(this.context);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: this._currentParent, restorePending: needsRestore);
        if (needsRestore)
        {
            _doRestore(oldBucket);
        }
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket, this._bucket)));
            oldBucket?.dispose();
        }
    }

    public virtual void _doRestore(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration = this._properties.Keys.ToList();
                return true;
            });
        restoreState(oldBucket, this._firstRestorePending);
        this._firstRestorePending = false;
        DartRuntimePrimitives.Assert(() =>
            {
                if (System.Linq.Enumerable.Any(this._debugPropertiesWaitingForReregistration!))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(((Func<List<global::Doroti.Framework.Foundation.DiagnosticsNode>>)(() => { var __collection41817 = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>(); __collection41817.Add(new global::Doroti.Framework.Foundation.ErrorSummary("Previously registered RestorableProperties must be re-registered in \"restoreState\".")); __collection41817.Add(new global::Doroti.Framework.Foundation.ErrorDescription($"The RestorableProperties with the following IDs were not re-registered to {this} when " + "\"restoreState\" was called:")); __collection41817.AddRange(this._debugPropertiesWaitingForReregistration!.map<dynamic, global::Doroti.Framework.Foundation.DiagnosticsNode>(((property) => new global::Doroti.Framework.Foundation.ErrorDescription($" * {((dynamic)property)._restorationId}")))); return __collection41817; }))()));
                }
                this._debugPropertiesWaitingForReregistration = null;
                return true;
            });
    }

    public virtual bool _updateBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? parent, bool restorePending)
    {
        if (((this.restorationId is null) || (parent is null)))
        {
            bool didReplace = _setNewBucketIfNecessary(newBucket: null, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (this._bucket is null));
            return didReplace;
        }
        DartRuntimePrimitives.Assert(() => (this.restorationId is not null));
        if ((restorePending || (this._bucket is null)))
        {
            global::Doroti.Framework.Services.RestorationBucket newBucketLocal = ((global::Doroti.Framework.Services.RestorationBucket)(object?)parent.claimChild(this.restorationId!, debugOwner: this));
            bool didReplaceLocal = _setNewBucketIfNecessary(newBucket: newBucketLocal, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (object.Equals(this._bucket, newBucketLocal)));
            return didReplaceLocal;
        }
        DartRuntimePrimitives.Assert(() => (this._bucket is not null));
        DartRuntimePrimitives.Assert(() => !restorePending);
        this._bucket!.rename(this.restorationId!);
        parent.adoptChild(this._bucket!);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _setNewBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? newBucket, bool restorePending)
    {
        if ((object.Equals(newBucket, this._bucket)))
        {
            return false;
        }
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        this._bucket = newBucket;
        if (!restorePending)
        {
            if ((this._bucket is not null))
            {
                this._properties.Keys.forEach((__arg0) => ((global::System.Action<dynamic>)this._updateProperty)(__arg0));
            }
            didToggleBucket(oldBucket);
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _updateProperty(dynamic property)
    {
        if (((dynamic)property).enabled)
        {
            this._bucket?.write(((dynamic)property)._restorationId!, property.toPrimitives());
        }
        else
        {
            this._bucket?.remove<object>(((dynamic)property)._restorationId!);
        }
    }

    public virtual void _unregister(dynamic property)
    {
        global::System.Action listener = this._properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
        property.removeListener((global::System.Action)(() => listener()));
        property._unregister();
    }

}

internal class _CalendarRangePickerDialog__date_picker : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual DateTime? selectedStartDate { get; private set; }
    public virtual DateTime? selectedEndDate { get; private set; }
    public virtual DateTime firstDate { get; private set; } = default!;
    public virtual DateTime lastDate { get; private set; } = default!;
    public virtual global::System.Func<DateTime, DateTime?, DateTime?, bool>? selectableDayPredicate { get; private set; }
    public virtual DateTime? currentDate { get; private set; }
    public virtual global::System.Action<DateTime> onStartDateChanged { get; private set; } = default!;
    public virtual global::System.Action<DateTime?> onEndDateChanged { get; private set; } = default!;
    public virtual global::System.Action? onConfirm { get; private set; }
    public virtual global::System.Action? onCancel { get; private set; }
    public virtual string confirmText { get; private set; } = default!;
    public virtual string helpText { get; private set; } = default!;
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? entryModeButton { get; private set; }

    internal _CalendarRangePickerDialog__date_picker(global::Doroti.Framework.Foundation.Key? key = null, DateTime? selectedStartDate = default!, DateTime? selectedEndDate = default!, DateTime firstDate = default!, DateTime lastDate = default!, DateTime? currentDate = default!, global::System.Action<DateTime> onStartDateChanged = default!, global::System.Action<DateTime?> onEndDateChanged = default!, global::System.Action? onConfirm = default!, global::System.Action? onCancel = default!, string confirmText = default!, string helpText = default!, global::System.Func<DateTime, DateTime?, DateTime?, bool>? selectableDayPredicate = default!, CalendarDelegate<DateTime> calendarDelegate = default!, global::Doroti.Framework.Widgets.Widget? entryModeButton = null) : base(key: key)
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

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        bool useMaterial3Local = theme.useMaterial3;
        MaterialLocalizations localizations = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        global::Doroti.Framework.Widgets.Orientation orientation = MediaQuery.orientationOf(context);
        DatePickerThemeData themeData = DatePickerTheme.of(context);
        DatePickerThemeData defaultsLocal = DatePickerTheme.defaults(context);
        global::Doroti.Ui.Color? dialogBackground = ((global::Doroti.Ui.Color?)(object?)(themeData.rangePickerBackgroundColor ?? defaultsLocal.rangePickerBackgroundColor));
        global::Doroti.Ui.Color? headerBackground = ((global::Doroti.Ui.Color?)(object?)(themeData.rangePickerHeaderBackgroundColor ?? defaultsLocal.rangePickerHeaderBackgroundColor));
        global::Doroti.Ui.Color? headerForeground = ((global::Doroti.Ui.Color?)(object?)(themeData.rangePickerHeaderForegroundColor ?? defaultsLocal.rangePickerHeaderForegroundColor));
        global::Doroti.Ui.Color? headerDisabledForeground = ((global::Doroti.Ui.Color?)(object?)headerForeground?.withOpacity(0.38));
        global::Doroti.Framework.Painting.TextStyle? headlineStyle = (themeData.rangePickerHeaderHeadlineStyle ?? defaultsLocal.rangePickerHeaderHeadlineStyle);
        global::Doroti.Framework.Painting.TextStyle? headlineHelpStyle = ((global::Doroti.Framework.Painting.TextStyle?)(object?)((themeData.rangePickerHeaderHelpStyle ?? defaultsLocal.rangePickerHeaderHelpStyle))?.apply(color: headerForeground));
        string startDateText = Date_pickerLibrary._formatRangeStartDate(localizations, this.calendarDelegate, this.selectedStartDate, this.selectedEndDate);
        string endDateText = Date_pickerLibrary._formatRangeEndDate(localizations, this.calendarDelegate, this.selectedStartDate, this.selectedEndDate, this.calendarDelegate.now());
        global::Doroti.Framework.Painting.TextStyle? startDateStyle = ((global::Doroti.Framework.Painting.TextStyle?)(object?)headlineStyle?.apply(color: ((this.selectedStartDate is not null) ? headerForeground : headerDisabledForeground)));
        global::Doroti.Framework.Painting.TextStyle? endDateStyle = ((global::Doroti.Framework.Painting.TextStyle?)(object?)headlineStyle?.apply(color: ((this.selectedEndDate is not null) ? headerForeground : headerDisabledForeground)));
        ButtonStyle buttonStyle = TextButton.styleFrom(foregroundColor: headerForeground, disabledForegroundColor: headerDisabledForeground);
        var iconThemeLocal = new global::Doroti.Framework.Widgets.IconThemeData(color: headerForeground);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.SafeArea(top: false, left: false, right: false, child: new Scaffold(appBar: new AppBar(iconTheme: iconThemeLocal, actionsIconTheme: iconThemeLocal, elevation: (useMaterial3Local ? 0 : null), scrolledUnderElevation: (useMaterial3Local ? 0 : null), backgroundColor: headerBackground, leading: new CloseButton(onPressed: () => this.onCancel()), actions: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection72957 = new List<global::Doroti.Framework.Widgets.Widget>(); if (((object.Equals(orientation, global::Doroti.Framework.Widgets.Orientation.landscape)) && (this.entryModeButton is not null))) { __collection72957.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(this.entryModeButton!)); } __collection72957.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new TextButton(style: buttonStyle, onPressed: this.onConfirm, child: new global::Doroti.Framework.Widgets.Text(this.confirmText)))); __collection72957.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(width: 8))); return __collection72957; }))(), bottom: new global::Doroti.Framework.Widgets.PreferredSize(preferredSize: new global::Doroti.Ui.Size(double.PositiveInfinity, 64), child: new global::Doroti.Framework.Widgets.Row(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection73350 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection73350.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(width: ((MediaQuery.widthOf(context) < 360L) ? 42 : 72)))); __collection73350.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.Semantics(label: $"{this.helpText} {startDateText} to {endDateText}", excludeSemantics: true, child: new global::Doroti.Framework.Widgets.Column(crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.start, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Text(this.helpText, style: headlineHelpStyle, maxLines: 1L, overflow: global::Doroti.Framework.Painting.TextOverflow.ellipsis)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(height: 8)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Row(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Text(startDateText, style: startDateStyle, maxLines: 1L, overflow: global::Doroti.Framework.Painting.TextOverflow.ellipsis)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Text(" – ", style: startDateStyle)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: new global::Doroti.Framework.Widgets.Text(endDateText, style: endDateStyle, maxLines: 1L, overflow: global::Doroti.Framework.Painting.TextOverflow.ellipsis))) })), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(height: 16)) }))))); if (((object.Equals(orientation, global::Doroti.Framework.Widgets.Orientation.portrait)) && (this.entryModeButton is not null))) { __collection73350.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 8.0), child: new global::Doroti.Framework.Widgets.IconTheme(data: iconThemeLocal, child: this.entryModeButton!)))); } return __collection73350; }))()))), backgroundColor: dialogBackground, body: new _CalendarDateRangePicker__date_picker(initialStartDate: this.selectedStartDate, initialEndDate: this.selectedEndDate, firstDate: this.firstDate, lastDate: this.lastDate, currentDate: this.currentDate, onStartDateChanged: (global::System.Action<DateTime>)this.onStartDateChanged, onEndDateChanged: (global::System.Action<DateTime?>)this.onEndDateChanged, selectableDayPredicate: (global::System.Func<DateTime, DateTime?, DateTime?, bool>?)this.selectableDayPredicate, calendarDelegate: this.calendarDelegate))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
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

internal class _CalendarDateRangePicker__date_picker : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual DateTime? initialStartDate { get; private set; }
    public virtual DateTime? initialEndDate { get; private set; }
    public virtual DateTime firstDate { get; private set; } = default!;
    public virtual DateTime lastDate { get; private set; } = default!;
    public virtual global::System.Func<DateTime, DateTime?, DateTime?, bool>? selectableDayPredicate { get; private set; }
    public virtual DateTime currentDate { get; private set; } = default!;
    public virtual global::System.Action<DateTime>? onStartDateChanged { get; private set; }
    public virtual global::System.Action<DateTime?>? onEndDateChanged { get; private set; }
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    internal _CalendarDateRangePicker__date_picker(DateTime? initialStartDate = null, DateTime? initialEndDate = null, DateTime firstDate = default!, DateTime lastDate = default!, global::System.Func<DateTime, DateTime?, DateTime?, bool>? selectableDayPredicate = default!, DateTime? currentDate = null, global::System.Action<DateTime>? onStartDateChanged = default!, global::System.Action<DateTime?>? onEndDateChanged = default!, CalendarDelegate<DateTime> calendarDelegate = default!)
    {
        this.selectableDayPredicate = selectableDayPredicate;
        this.onStartDateChanged = onStartDateChanged;
        this.onEndDateChanged = onEndDateChanged;
        this.calendarDelegate = calendarDelegate;
        this.initialStartDate = ((initialStartDate is not null) ? calendarDelegate.dateOnly(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(initialStartDate))) : null);
        this.initialEndDate = ((initialEndDate is not null) ? calendarDelegate.dateOnly(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(initialEndDate))) : null);
        this.firstDate = calendarDelegate.dateOnly(firstDate);
        this.lastDate = calendarDelegate.dateOnly(lastDate);
        this.currentDate = calendarDelegate.dateOnly(((currentDate ?? (DateTime)calendarDelegate.now())));
        DartRuntimePrimitives.Assert(() => (((this.initialStartDate is null) || (this.initialEndDate is null)) || !DartRuntimePrimitives.RequireValue(this.initialStartDate).isAfter(DartRuntimePrimitives.RequireValue(initialEndDate))), () => (object?)"initialStartDate must be on or before initialEndDate.");
        DartRuntimePrimitives.Assert(() => !this.lastDate.isBefore(this.firstDate), () => (object?)"firstDate must be on or before lastDate.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CalendarDateRangePickerState__date_picker());
}

internal class _CalendarDateRangePickerState__date_picker : global::Doroti.Framework.Widgets.State<_CalendarDateRangePicker__date_picker>
{
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _scrollViewKey { get; private set; } = global::Doroti.Framework.Widgets.GlobalKey<IState>.Create();
    internal virtual global::Doroti.Framework.Foundation.Key _sliverAfterKey { get; private set; } = ((global::Doroti.Framework.Foundation.Key)(object?)new global::Doroti.Framework.Foundation.UniqueKey());
    internal virtual DateTime? _startDate { get; set; } = default;
    internal virtual DateTime? _endDate { get; set; } = default;
    internal virtual long _initialMonthIndex { get; set; } = 0L;
    internal virtual global::Doroti.Framework.Widgets.ScrollController _controller { get; set; } = default!;
    internal virtual bool _showWeekBottomDivider { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        _controller = new global::Doroti.Framework.Widgets.ScrollController();
        this._controller.addListener(() => this._scrollListener());
        _startDate = ((_CalendarDateRangePicker__date_picker)this.widget).initialStartDate;
        _endDate = ((_CalendarDateRangePicker__date_picker)this.widget).initialEndDate;
        DateTime initialDate = (((_CalendarDateRangePicker__date_picker)this.widget).initialStartDate ?? ((_CalendarDateRangePicker__date_picker)this.widget).currentDate);
        if ((!initialDate.isBefore(((_CalendarDateRangePicker__date_picker)this.widget).firstDate) && !initialDate.isAfter(((_CalendarDateRangePicker__date_picker)this.widget).lastDate)))
        {
            _initialMonthIndex = ((_CalendarDateRangePicker__date_picker)this.widget).calendarDelegate.monthDelta(((_CalendarDateRangePicker__date_picker)this.widget).firstDate, initialDate);
        }
        _showWeekBottomDivider = (this._initialMonthIndex != 0L);
    }

    public override void dispose()
    {
        this._controller.dispose();
        base.dispose();
    }

    internal virtual void _scrollListener()
    {
        if ((((global::Doroti.Framework.Widgets.ScrollController)this._controller).offset <= ((global::Doroti.Framework.Widgets.ScrollController)this._controller).position.minScrollExtent))
        {
            setState(((global::System.Action)(() =>
            {
                _showWeekBottomDivider = false;
            })));
        }
        else
        {
            if (!this._showWeekBottomDivider)
            {
                setState(((global::System.Action)(() =>
                {
                    _showWeekBottomDivider = true;
                })));
            }
        }
    }

    internal virtual long _numberOfMonths => DartRuntimePrimitives.ConvertValue<long>((((_CalendarDateRangePicker__date_picker)this.widget).calendarDelegate.monthDelta(((_CalendarDateRangePicker__date_picker)this.widget).firstDate, ((_CalendarDateRangePicker__date_picker)this.widget).lastDate) + 1L));
    internal virtual void _vibrate()
    {
        switch (Theme.of(this.context).platform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                {
                    DartRuntimePrimitives.Ignore(HapticFeedback.vibrate());
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    break;
                }
        }
    }

    internal virtual void _updateSelection(DateTime date)
    {
        _vibrate();
        setState(((global::System.Action)(() =>
        {
            if ((((this._startDate is not null) && (this._endDate is null)) && !date.isBefore(DartRuntimePrimitives.RequireValue(this._startDate))))
            {
                _endDate = date;
                ((_CalendarDateRangePicker__date_picker)this.widget).onEndDateChanged?.Invoke(this._endDate);
            }
            else
            {
                _startDate = date;
                ((_CalendarDateRangePicker__date_picker)this.widget).onStartDateChanged?.Invoke(DartRuntimePrimitives.RequireValue(this._startDate));
                if ((this._endDate is not null))
                {
                    _endDate = null;
                    ((_CalendarDateRangePicker__date_picker)this.widget).onEndDateChanged?.Invoke(this._endDate);
                }
            }
        })));
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildMonthItem(global::Doroti.Framework.Widgets.BuildContext context, long index, bool beforeInitialMonth)
    {
        long monthIndex = (beforeInitialMonth ? ((this._initialMonthIndex - index) - 1L) : (this._initialMonthIndex + index));
        DateTime month = ((_CalendarDateRangePicker__date_picker)this.widget).calendarDelegate.addMonthsToMonthDate(((_CalendarDateRangePicker__date_picker)this.widget).firstDate, monthIndex);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _MonthItem__date_picker(calendarDelegate: ((_CalendarDateRangePicker__date_picker)this.widget).calendarDelegate, selectedDateStart: this._startDate, selectedDateEnd: this._endDate, currentDate: ((_CalendarDateRangePicker__date_picker)this.widget).currentDate, firstDate: ((_CalendarDateRangePicker__date_picker)this.widget).firstDate, lastDate: ((_CalendarDateRangePicker__date_picker)this.widget).lastDate, displayedMonth: month, onChanged: (global::System.Action<DateTime>)this._updateSelection, selectableDayPredicate: (global::System.Func<DateTime, DateTime?, DateTime?, bool>?)((_CalendarDateRangePicker__date_picker)this.widget).selectableDayPredicate));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Column(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection82299 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection82299.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _DayHeaders__date_picker())); if (this._showWeekBottomDivider) { __collection82299.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new Divider(height: 0))); } __collection82299.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new _CalendarKeyboardNavigator__date_picker(calendarDelegate: ((_CalendarDateRangePicker__date_picker)this.widget).calendarDelegate, firstDate: ((_CalendarDateRangePicker__date_picker)this.widget).firstDate, lastDate: ((_CalendarDateRangePicker__date_picker)this.widget).lastDate, initialFocusedDay: ((this._startDate ?? ((_CalendarDateRangePicker__date_picker)this.widget).initialStartDate) ?? ((_CalendarDateRangePicker__date_picker)this.widget).currentDate), child: new global::Doroti.Framework.Widgets.CustomScrollView(key: this._scrollViewKey, controller: this._controller, center: this._sliverAfterKey, slivers: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(global::Doroti.Framework.Widgets.SliverList.CreateBuilder(itemCount: this._initialMonthIndex, itemBuilder: ((context, index) => _buildMonthItem(context, index, true)))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(global::Doroti.Framework.Widgets.SliverList.CreateBuilder(key: this._sliverAfterKey, itemCount: (this._numberOfMonths - this._initialMonthIndex), itemBuilder: ((context, index) => _buildMonthItem(context, index, false)))) }))))); return __collection82299; }))()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CalendarKeyboardNavigator__date_picker : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual DateTime firstDate { get; private set; } = default!;
    public virtual DateTime lastDate { get; private set; } = default!;
    public virtual DateTime initialFocusedDay { get; private set; } = default!;
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    internal _CalendarKeyboardNavigator__date_picker(global::Doroti.Framework.Widgets.Widget child, DateTime firstDate, DateTime lastDate, DateTime initialFocusedDay, CalendarDelegate<DateTime> calendarDelegate)
    {
        this.child = child;
        this.firstDate = firstDate;
        this.lastDate = lastDate;
        this.initialFocusedDay = initialFocusedDay;
        this.calendarDelegate = calendarDelegate;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CalendarKeyboardNavigatorState__date_picker());
}

internal class _CalendarKeyboardNavigatorState__date_picker : global::Doroti.Framework.Widgets.State<_CalendarKeyboardNavigator__date_picker>
{
    internal virtual DartMap<global::Doroti.Framework.Widgets.ShortcutActivator, global::Doroti.Framework.Widgets.Intent> _shortcutMap { get; private set; } = new DartMap<global::Doroti.Framework.Widgets.ShortcutActivator, global::Doroti.Framework.Widgets.Intent> { [new global::Doroti.Framework.Widgets.SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.arrowLeft)] = ((global::Doroti.Framework.Widgets.Intent)(object?)new global::Doroti.Framework.Widgets.DirectionalFocusIntent(global::Doroti.Framework.Widgets.TraversalDirection.left)), [new global::Doroti.Framework.Widgets.SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.arrowRight)] = ((global::Doroti.Framework.Widgets.Intent)(object?)new global::Doroti.Framework.Widgets.DirectionalFocusIntent(global::Doroti.Framework.Widgets.TraversalDirection.right)), [new global::Doroti.Framework.Widgets.SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.arrowDown)] = ((global::Doroti.Framework.Widgets.Intent)(object?)new global::Doroti.Framework.Widgets.DirectionalFocusIntent(global::Doroti.Framework.Widgets.TraversalDirection.down)), [new global::Doroti.Framework.Widgets.SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.arrowUp)] = ((global::Doroti.Framework.Widgets.Intent)(object?)new global::Doroti.Framework.Widgets.DirectionalFocusIntent(global::Doroti.Framework.Widgets.TraversalDirection.up)) };
    internal virtual DartMap<Type, dynamic> _actionMap { get; set; } = default!;
    internal virtual global::Doroti.Framework.Widgets.FocusNode _dayGridFocus { get; set; } = default!;
    internal virtual global::Doroti.Framework.Widgets.TraversalDirection? _dayTraversalDirection { get; set; } = default;
    internal virtual DateTime? _focusedDay { get; set; } = default;
    internal static DartMap<global::Doroti.Framework.Widgets.TraversalDirection, long> _directionOffset = new DartMap<global::Doroti.Framework.Widgets.TraversalDirection, long> { [global::Doroti.Framework.Widgets.TraversalDirection.up] = -7L, [global::Doroti.Framework.Widgets.TraversalDirection.right] = 1L, [global::Doroti.Framework.Widgets.TraversalDirection.down] = 7L, [global::Doroti.Framework.Widgets.TraversalDirection.left] = -1L };

    public override void initState()
    {
        base.initState();
        _actionMap = new DartMap<Type, dynamic> { [typeof(global::Doroti.Framework.Widgets.NextFocusIntent)] = new global::Doroti.Framework.Widgets.CallbackAction<global::Doroti.Framework.Widgets.NextFocusIntent>(onInvoke: (global::System.Action<global::Doroti.Framework.Widgets.NextFocusIntent>)this._handleGridNextFocus), [typeof(global::Doroti.Framework.Widgets.PreviousFocusIntent)] = new global::Doroti.Framework.Widgets.CallbackAction<global::Doroti.Framework.Widgets.PreviousFocusIntent>(onInvoke: (global::System.Action<global::Doroti.Framework.Widgets.PreviousFocusIntent>)this._handleGridPreviousFocus), [typeof(global::Doroti.Framework.Widgets.DirectionalFocusIntent)] = new global::Doroti.Framework.Widgets.CallbackAction<global::Doroti.Framework.Widgets.DirectionalFocusIntent>(onInvoke: (global::System.Action<global::Doroti.Framework.Widgets.DirectionalFocusIntent>)this._handleDirectionFocus) };
        _dayGridFocus = new global::Doroti.Framework.Widgets.FocusNode(debugLabel: "Day Grid");
    }

    public override void dispose()
    {
        this._dayGridFocus.dispose();
        base.dispose();
    }

    internal virtual void _handleGridFocusChange(bool focused)
    {
        setState(((global::System.Action)(() =>
        {
            if (focused)
            {
                _focusedDay ??= ((_CalendarKeyboardNavigator__date_picker)this.widget).initialFocusedDay;
            }
        })));
    }

    internal virtual void _handleGridNextFocus(global::Doroti.Framework.Widgets.NextFocusIntent intent)
    {
        this._dayGridFocus.requestFocus();
        this._dayGridFocus.nextFocus();
    }

    internal virtual void _handleGridPreviousFocus(global::Doroti.Framework.Widgets.PreviousFocusIntent intent)
    {
        this._dayGridFocus.requestFocus();
        this._dayGridFocus.previousFocus();
    }

    internal virtual void _handleDirectionFocus(global::Doroti.Framework.Widgets.DirectionalFocusIntent intent)
    {
        DartRuntimePrimitives.Assert(() => (this._focusedDay is not null));
        setState(((global::System.Action)(() =>
        {
            DateTime? nextDate = _nextDateInDirection(DartRuntimePrimitives.RequireValue(this._focusedDay), ((global::Doroti.Framework.Widgets.DirectionalFocusIntent)intent).direction);
            if ((nextDate is not null))
            {
                DateTime nextDate__86735__value86810 = DartRuntimePrimitives.RequireValue(nextDate);
                _focusedDay = DartRuntimePrimitives.RequireValue(nextDate__86735__value86810);
                _dayTraversalDirection = ((global::Doroti.Framework.Widgets.DirectionalFocusIntent)intent).direction;
            }
        })));
    }

    internal virtual long _dayDirectionOffset(global::Doroti.Framework.Widgets.TraversalDirection traversalDirection, TextDirection textDirection)
    {
        if ((object.Equals(textDirection, TextDirection.rtl)))
        {
            if ((object.Equals(traversalDirection, global::Doroti.Framework.Widgets.TraversalDirection.left)))
            {
                traversalDirection = global::Doroti.Framework.Widgets.TraversalDirection.right;
            }
            else
            {
                if ((object.Equals(traversalDirection, global::Doroti.Framework.Widgets.TraversalDirection.right)))
                {
                    traversalDirection = global::Doroti.Framework.Widgets.TraversalDirection.left;
                }
            }
        }
        return DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<long>(_directionOffset, traversalDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual DateTime? _nextDateInDirection(DateTime date, global::Doroti.Framework.Widgets.TraversalDirection direction)
    {
        global::Doroti.Ui.TextDirection textDirection = Directionality.of(this.context);
        DateTime nextDate = ((_CalendarKeyboardNavigator__date_picker)this.widget).calendarDelegate.addDaysToDate(date, _dayDirectionOffset(direction, textDirection));
        if ((!nextDate.isBefore(((_CalendarKeyboardNavigator__date_picker)this.widget).firstDate) && !nextDate.isAfter(((_CalendarKeyboardNavigator__date_picker)this.widget).lastDate)))
        {
            return nextDate;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.FocusableActionDetector(shortcuts: this._shortcutMap, actions: this._actionMap, focusNode: this._dayGridFocus, onFocusChange: (global::System.Action<bool>)this._handleGridFocusChange, child: new _FocusedDate__date_picker(calendarDelegate: ((_CalendarKeyboardNavigator__date_picker)this.widget).calendarDelegate, date: (((global::Doroti.Framework.Widgets.FocusNode)this._dayGridFocus).hasFocus ? this._focusedDay : null), scrollDirection: (((global::Doroti.Framework.Widgets.FocusNode)this._dayGridFocus).hasFocus ? this._dayTraversalDirection : null), child: ((_CalendarKeyboardNavigator__date_picker)this.widget).child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _FocusedDate__date_picker : global::Doroti.Framework.Widgets.InheritedWidget
{
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;
    public virtual DateTime? date { get; private set; }
    public virtual global::Doroti.Framework.Widgets.TraversalDirection? scrollDirection { get; private set; }

    internal _FocusedDate__date_picker(global::Doroti.Framework.Widgets.Widget child, CalendarDelegate<DateTime> calendarDelegate, DateTime? date = null, global::Doroti.Framework.Widgets.TraversalDirection? scrollDirection = null) : base(child: child)
    {
        this.calendarDelegate = calendarDelegate;
        this.date = date;
        this.scrollDirection = scrollDirection;
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget)
    {
        var __oldWidget = (_FocusedDate__date_picker)(object)oldWidget;
        return (!this.calendarDelegate.isSameDay(this.date, ((_FocusedDate__date_picker)__oldWidget).date) || (!object.Equals(this.scrollDirection, ((_FocusedDate__date_picker)__oldWidget).scrollDirection)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static _FocusedDate__date_picker? maybeOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((_FocusedDate__date_picker?)(object?)context.dependOnInheritedWidgetOfExactType<_FocusedDate__date_picker>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DayHeaders__date_picker : global::Doroti.Framework.Widgets.StatelessWidget
{
    internal _DayHeaders__date_picker()
    {
    }

    internal virtual List<global::Doroti.Framework.Widgets.Widget> _getDayHeaders(global::Doroti.Framework.Painting.TextStyle headerStyle, MaterialLocalizations localizations)
    {
        var result = new List<global::Doroti.Framework.Widgets.Widget>();
        for (long i = ((MaterialLocalizations)localizations).firstDayOfWeekIndex; (checked((long)(result.Count)) < 7L); i = (((i + 1L)) % 7L))
        {
            string weekday = ((MaterialLocalizations)localizations).narrowWeekdays[(int)(i)];
            result.Add(new global::Doroti.Framework.Widgets.ExcludeSemantics(child: new global::Doroti.Framework.Widgets.Center(child: new global::Doroti.Framework.Widgets.Text(weekday, style: headerStyle))));
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData themeData = Theme.of(context);
        ColorScheme colorSchemeLocal = themeData.colorScheme;
        global::Doroti.Framework.Painting.TextStyle textStyle = ((global::Doroti.Framework.Painting.TextStyle)(object?)themeData.textTheme.titleSmall!.apply(color: colorSchemeLocal.onSurface));
        MaterialLocalizations localizations = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        List<global::Doroti.Framework.Widgets.Widget> labels = ((List<global::Doroti.Framework.Widgets.Widget>)(object?)_getDayHeaders(textStyle, localizations));
        labels.Insert(checked((int)0L), global::Doroti.Framework.Widgets.SizedBox.CreateShrink());
        labels.Add(global::Doroti.Framework.Widgets.SizedBox.CreateShrink());
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(maxWidth: ((object.Equals(MediaQuery.orientationOf(context), global::Doroti.Framework.Widgets.Orientation.landscape)) ? Date_pickerLibrary._maxCalendarWidthLandscape : Date_pickerLibrary._maxCalendarWidthPortrait), maxHeight: Date_pickerLibrary._monthItemRowHeight), child: global::Doroti.Framework.Widgets.GridView.CreateCustom(shrinkWrap: true, gridDelegate: Date_pickerLibrary._monthItemGridDelegate, childrenDelegate: new global::Doroti.Framework.Widgets.SliverChildListDelegate(labels, addRepaintBoundaries: false))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MonthItemGridDelegate__date_picker : global::Doroti.Framework.Rendering.SliverGridDelegate
{
    internal _MonthItemGridDelegate__date_picker()
    {
    }

    public virtual global::Doroti.Framework.Rendering.SliverGridLayout getLayout(global::Doroti.Framework.Rendering.SliverConstraints constraints)
    {
        double tileWidth = Math.Max((((((global::Doroti.Framework.Rendering.SliverConstraints)constraints).crossAxisExtent - (2L * Date_pickerLibrary._horizontalPadding))) / 7L), 0.0);
        return ((global::Doroti.Framework.Rendering.SliverGridLayout)(object?)new _MonthSliverGridLayout__date_picker(crossAxisCount: (7L + 2L), dayChildWidth: tileWidth, edgeChildWidth: Date_pickerLibrary._horizontalPadding, reverseCrossAxis: global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionIsReversed(((global::Doroti.Framework.Rendering.SliverConstraints)constraints).crossAxisDirection)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool shouldRelayout(global::Doroti.Framework.Rendering.SliverGridDelegate oldDelegate) => false;
}

public static partial class Date_pickerLibrary
{
    internal static _MonthItemGridDelegate__date_picker _monthItemGridDelegate = new _MonthItemGridDelegate__date_picker();
}

internal class _MonthSliverGridLayout__date_picker : global::Doroti.Framework.Rendering.SliverGridLayout
{
    public virtual long crossAxisCount { get; private set; } = default!;
    public virtual double dayChildWidth { get; private set; } = default!;
    public virtual double edgeChildWidth { get; private set; } = default!;
    public virtual bool reverseCrossAxis { get; private set; } = default!;

    internal _MonthSliverGridLayout__date_picker(long crossAxisCount, double dayChildWidth, double edgeChildWidth, bool reverseCrossAxis)
    {
        this.crossAxisCount = crossAxisCount;
        this.dayChildWidth = dayChildWidth;
        this.edgeChildWidth = edgeChildWidth;
        this.reverseCrossAxis = reverseCrossAxis;
        System.Diagnostics.Debug.Assert((crossAxisCount > 0L));
        System.Diagnostics.Debug.Assert((dayChildWidth >= 0L));
        System.Diagnostics.Debug.Assert((edgeChildWidth >= 0L));
    }

    internal virtual double _rowHeight
    {
        get
        {
            return (Date_pickerLibrary._monthItemRowHeight + Date_pickerLibrary._monthItemSpaceBetweenRows);
            return default!;
        }
    }
    internal virtual double _childHeight
    {
        get
        {
            return Date_pickerLibrary._monthItemRowHeight;
            return default!;
        }
    }
    public virtual long getMinChildIndexForScrollOffset(double scrollOffset)
    {
        return (this.crossAxisCount * ((checked((long)(scrollOffset / this._rowHeight)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long getMaxChildIndexForScrollOffset(double scrollOffset)
    {
        long mainAxisCount = ((scrollOffset / this._rowHeight)).ceil();
        return Math.Max(0L, ((this.crossAxisCount * mainAxisCount) - 1L));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getCrossAxisOffset(double crossAxisStart, bool isPadding)
    {
        if (this.reverseCrossAxis)
        {
            return (((((((this.crossAxisCount - 2L)) * this.dayChildWidth) + (2L * this.edgeChildWidth))) - crossAxisStart) - ((isPadding ? this.edgeChildWidth : this.dayChildWidth)));
        }
        return crossAxisStart;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Rendering.SliverGridGeometry getGeometryForChildIndex(long index)
    {
        long adjustedIndex = (index % this.crossAxisCount);
        bool isEdge = ((adjustedIndex == 0L) || (adjustedIndex == (this.crossAxisCount - 1L)));
        double crossAxisStart = Math.Max(0, ((((adjustedIndex - 1L)) * this.dayChildWidth) + this.edgeChildWidth));
        return new global::Doroti.Framework.Rendering.SliverGridGeometry(scrollOffset: (((checked((long)(index / this.crossAxisCount)))) * this._rowHeight), crossAxisOffset: _getCrossAxisOffset(crossAxisStart, isEdge), mainAxisExtent: this._childHeight, crossAxisExtent: (isEdge ? this.edgeChildWidth : this.dayChildWidth));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double computeMaxScrollOffset(long childCount)
    {
        DartRuntimePrimitives.Assert(() => (childCount >= 0L));
        long mainAxisCount = (((checked((long)(((childCount - 1L)) / this.crossAxisCount)))) + 1L);
        double mainAxisSpacing = (this._rowHeight - this._childHeight);
        return ((this._rowHeight * mainAxisCount) - mainAxisSpacing);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MonthItem__date_picker : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual DateTime? selectedDateStart { get; private set; }
    public virtual DateTime? selectedDateEnd { get; private set; }
    public virtual DateTime currentDate { get; private set; } = default!;
    public virtual global::System.Action<DateTime> onChanged { get; private set; } = default!;
    public virtual DateTime firstDate { get; private set; } = default!;
    public virtual DateTime lastDate { get; private set; } = default!;
    public virtual DateTime displayedMonth { get; private set; } = default!;
    public virtual global::System.Func<DateTime, DateTime?, DateTime?, bool>? selectableDayPredicate { get; private set; }
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    internal _MonthItem__date_picker(DateTime? selectedDateStart, DateTime? selectedDateEnd, DateTime currentDate, global::System.Action<DateTime> onChanged, DateTime firstDate, DateTime lastDate, DateTime displayedMonth, global::System.Func<DateTime, DateTime?, DateTime?, bool>? selectableDayPredicate, CalendarDelegate<DateTime> calendarDelegate)
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
        System.Diagnostics.Debug.Assert(((selectedDateStart is null) || !DartRuntimePrimitives.RequireValue(selectedDateStart).isBefore(firstDate)));
        System.Diagnostics.Debug.Assert(((selectedDateEnd is null) || !DartRuntimePrimitives.RequireValue(selectedDateEnd).isBefore(firstDate)));
        System.Diagnostics.Debug.Assert(((selectedDateStart is null) || !DartRuntimePrimitives.RequireValue(selectedDateStart).isAfter(lastDate)));
        System.Diagnostics.Debug.Assert(((selectedDateEnd is null) || !DartRuntimePrimitives.RequireValue(selectedDateEnd).isAfter(lastDate)));
        System.Diagnostics.Debug.Assert((((selectedDateStart is null) || (selectedDateEnd is null)) || !DartRuntimePrimitives.RequireValue(selectedDateStart).isAfter(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(selectedDateEnd)))));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _MonthItemState__date_picker());
}

internal class _MonthItemState__date_picker : global::Doroti.Framework.Widgets.State<_MonthItem__date_picker>
{
    internal virtual List<global::Doroti.Framework.Widgets.FocusNode> _dayFocusNodes { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        long daysInMonth = ((_MonthItem__date_picker)this.widget).calendarDelegate.getDaysInMonth(((_MonthItem__date_picker)this.widget).displayedMonth.Year, ((_MonthItem__date_picker)this.widget).displayedMonth.Month);
        _dayFocusNodes = new List<global::Doroti.Framework.Widgets.FocusNode>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)daysInMonth)), ((index) => new global::Doroti.Framework.Widgets.FocusNode(skipTraversal: true, debugLabel: $"Day {(index + 1L)}"))));
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        DateTime? focusedDate = _FocusedDate__date_picker.maybeOf(this.context)?.date;
        if (((focusedDate is not null) && ((_MonthItem__date_picker)this.widget).calendarDelegate.isSameMonth(((_MonthItem__date_picker)this.widget).displayedMonth, DartRuntimePrimitives.RequireValue(focusedDate))))
        {
            DateTime focusedDate__98201__value98260 = DartRuntimePrimitives.RequireValue(focusedDate);
            this._dayFocusNodes[(int)((DartRuntimePrimitives.RequireValue(focusedDate__98201__value98260).Day - 1L))].requestFocus();
        }
    }

    public override void dispose()
    {
        foreach (global::Doroti.Framework.Widgets.FocusNode node in this._dayFocusNodes)
        {
            node.dispose();
        }
        base.dispose();
    }

    internal virtual global::Doroti.Ui.Color _highlightColor(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Ui.Color)(object?)(DatePickerTheme.of(context).rangeSelectionBackgroundColor ?? DatePickerTheme.defaults(context).rangeSelectionBackgroundColor!));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _dayFocusChanged(bool focused)
    {
        if (focused)
        {
            global::Doroti.Framework.Widgets.TraversalDirection? focusDirection = _FocusedDate__date_picker.maybeOf(this.context)?.scrollDirection;
            if ((focusDirection is not null))
            {
                global::Doroti.Framework.Widgets.TraversalDirection focusDirection__98861__value98936 = DartRuntimePrimitives.RequireValue(focusDirection);
                global::Doroti.Framework.Widgets.ScrollPositionAlignmentPolicy policy = global::Doroti.Framework.Widgets.ScrollPositionAlignmentPolicy.@explicit;
                switch (DartRuntimePrimitives.RequireValue(focusDirection__98861__value98936))
                {
                    case global::Doroti.Framework.Widgets.TraversalDirection.up:
                    case global::Doroti.Framework.Widgets.TraversalDirection.left:
                        {
                            policy = global::Doroti.Framework.Widgets.ScrollPositionAlignmentPolicy.keepVisibleAtStart;
                            break;
                        }
                    case global::Doroti.Framework.Widgets.TraversalDirection.right:
                    case global::Doroti.Framework.Widgets.TraversalDirection.down:
                        {
                            policy = global::Doroti.Framework.Widgets.ScrollPositionAlignmentPolicy.keepVisibleAtEnd;
                            break;
                        }
                }
                DartRuntimePrimitives.Ignore(Scrollable.ensureVisible(global::Doroti.Framework.Widgets.Focus_managerLibrary.primaryFocus!.context!, duration: Calendar_date_pickerLibrary._monthScrollDuration, alignmentPolicy: policy));
            }
        }
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildDayItem(global::Doroti.Framework.Widgets.BuildContext context, DateTime dayToBuild, long firstDayOffset, long daysInMonth)
    {
        long dayLocal = dayToBuild.Day;
        bool isDisabledLocal = ((dayToBuild.isAfter(((_MonthItem__date_picker)this.widget).lastDate) || dayToBuild.isBefore(((_MonthItem__date_picker)this.widget).firstDate)) || ((((_MonthItem__date_picker)this.widget).selectableDayPredicate is not null) && !((_MonthItem__date_picker)this.widget).selectableDayPredicate!(dayToBuild, ((_MonthItem__date_picker)this.widget).selectedDateStart, ((_MonthItem__date_picker)this.widget).selectedDateEnd)));
        bool isRangeSelectedLocal = ((((_MonthItem__date_picker)this.widget).selectedDateStart is not null) && (((_MonthItem__date_picker)this.widget).selectedDateEnd is not null));
        bool isSelectedDayStartLocal = ((((_MonthItem__date_picker)this.widget).selectedDateStart is not null) && dayToBuild.isAtSameMomentAs(DartRuntimePrimitives.RequireValue(((_MonthItem__date_picker)this.widget).selectedDateStart)));
        bool isSelectedDayEndLocal = ((((_MonthItem__date_picker)this.widget).selectedDateEnd is not null) && dayToBuild.isAtSameMomentAs(DartRuntimePrimitives.RequireValue(((_MonthItem__date_picker)this.widget).selectedDateEnd)));
        bool isInRangeLocal = ((isRangeSelectedLocal && dayToBuild.isAfter(DartRuntimePrimitives.RequireValue(((_MonthItem__date_picker)this.widget).selectedDateStart))) && dayToBuild.isBefore(DartRuntimePrimitives.RequireValue(((_MonthItem__date_picker)this.widget).selectedDateEnd)));
        bool isOneDayRangeLocal = (isRangeSelectedLocal && (object.Equals(((_MonthItem__date_picker)this.widget).selectedDateStart, ((_MonthItem__date_picker)this.widget).selectedDateEnd)));
        bool isTodayLocal = ((_MonthItem__date_picker)this.widget).calendarDelegate.isSameDay(((_MonthItem__date_picker)this.widget).currentDate, dayToBuild);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _DayItem__date_picker(calendarDelegate: ((_MonthItem__date_picker)this.widget).calendarDelegate, day: dayToBuild, focusNode: this._dayFocusNodes[(int)((dayLocal - 1L))], onChanged: (global::System.Action<DateTime>)((_MonthItem__date_picker)this.widget).onChanged, onFocusChange: (global::System.Action<bool>)this._dayFocusChanged, highlightColor: _highlightColor(context), isDisabled: isDisabledLocal, isRangeSelected: isRangeSelectedLocal, isSelectedDayStart: isSelectedDayStartLocal, isSelectedDayEnd: isSelectedDayEndLocal, isInRange: isInRangeLocal, isOneDayRange: isOneDayRangeLocal, isToday: isTodayLocal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildEdgeBox(global::Doroti.Framework.Widgets.BuildContext context, bool isHighlighted)
    {
        global::Doroti.Framework.Widgets.Widget empty = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.LimitedBox(maxWidth: 0.0, maxHeight: 0.0, child: global::Doroti.Framework.Widgets.SizedBox.CreateExpand()));
        return (isHighlighted ? new global::Doroti.Framework.Widgets.ColoredBox(color: _highlightColor(context), child: empty) : empty);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData themeData = Theme.of(context);
        TextTheme textThemeLocal = themeData.textTheme;
        MaterialLocalizations localizations = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        long year = ((_MonthItem__date_picker)this.widget).displayedMonth.Year;
        long month = ((_MonthItem__date_picker)this.widget).displayedMonth.Month;
        long daysInMonth = ((_MonthItem__date_picker)this.widget).calendarDelegate.getDaysInMonth(year, month);
        long dayOffset = ((_MonthItem__date_picker)this.widget).calendarDelegate.firstDayOffset(year, month, localizations);
        long weeks = ((((daysInMonth + dayOffset)) / 7L)).ceil();
        double gridHeight = ((weeks * Date_pickerLibrary._monthItemRowHeight) + (((weeks - 1L)) * Date_pickerLibrary._monthItemSpaceBetweenRows));
        var dayItems = new List<global::Doroti.Framework.Widgets.Widget>();
        for (long day = ((0L - dayOffset) + 1L); (day <= daysInMonth); day += 1L)
        {
            if ((day < 1L))
            {
                dayItems.Add(new global::Doroti.Framework.Widgets.LimitedBox(maxWidth: 0.0, maxHeight: 0.0, child: global::Doroti.Framework.Widgets.SizedBox.CreateExpand()));
            }
            else
            {
                DateTime dayToBuild = ((_MonthItem__date_picker)this.widget).calendarDelegate.getDay(year, month, day);
                global::Doroti.Framework.Widgets.Widget dayItem = ((global::Doroti.Framework.Widgets.Widget)(object?)_buildDayItem(context, dayToBuild, dayOffset, daysInMonth));
                dayItems.Add(dayItem);
            }
        }
        var paddedDayItems = new List<global::Doroti.Framework.Widgets.Widget>();
        for (var i = 0L; (i < weeks); i++)
        {
            long start = (i * 7L);
            long end = Math.Min((start + 7L), checked((long)(dayItems.Count)));
            List<global::Doroti.Framework.Widgets.Widget> weekList = dayItems.GetRange(start, end).ToList();
            DateTime dateAfterLeadingPadding = ((_MonthItem__date_picker)this.widget).calendarDelegate.getDay(year, month, ((start - dayOffset) + 1L));
            bool isLeadingInRange = ((((!(((dayOffset > 0L) && (i == 0L))) && (((_MonthItem__date_picker)this.widget).selectedDateStart is not null)) && (((_MonthItem__date_picker)this.widget).selectedDateEnd is not null)) && dateAfterLeadingPadding.isAfter(DartRuntimePrimitives.RequireValue(((_MonthItem__date_picker)this.widget).selectedDateStart))) && !dateAfterLeadingPadding.isAfter(DartRuntimePrimitives.RequireValue(((_MonthItem__date_picker)this.widget).selectedDateEnd)));
            weekList.Insert(checked((int)0L), _buildEdgeBox(context, isLeadingInRange));
            if (((end < checked((long)(dayItems.Count))) || (((end == checked((long)(dayItems.Count))) && ((checked((long)(dayItems.Count)) % 7L) == 0L)))))
            {
                DateTime dateBeforeTrailingPadding = ((_MonthItem__date_picker)this.widget).calendarDelegate.getDay(year, month, (end - dayOffset));
                bool isTrailingInRange = ((((((_MonthItem__date_picker)this.widget).selectedDateStart is not null) && (((_MonthItem__date_picker)this.widget).selectedDateEnd is not null)) && !dateBeforeTrailingPadding.isBefore(DartRuntimePrimitives.RequireValue(((_MonthItem__date_picker)this.widget).selectedDateStart))) && dateBeforeTrailingPadding.isBefore(DartRuntimePrimitives.RequireValue(((_MonthItem__date_picker)this.widget).selectedDateEnd)));
                weekList.Add(_buildEdgeBox(context, isTrailingInRange));
            }
            paddedDayItems.AddRange(weekList.Cast<global::Doroti.Framework.Widgets.Widget>());
        }
        double maxWidthLocal = ((object.Equals(MediaQuery.orientationOf(context), global::Doroti.Framework.Widgets.Orientation.landscape)) ? Date_pickerLibrary._maxCalendarWidthLandscape : Date_pickerLibrary._maxCalendarWidthPortrait);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Column(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(maxWidth: maxWidthLocal).tighten(height: Date_pickerLibrary._monthItemHeaderHeight), child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 16), child: new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.AlignmentDirectional.centerStart, child: new global::Doroti.Framework.Widgets.ExcludeSemantics(child: new global::Doroti.Framework.Widgets.Text(((_MonthItem__date_picker)this.widget).calendarDelegate.formatMonthYear(((_MonthItem__date_picker)this.widget).displayedMonth, localizations), style: textThemeLocal.bodyMedium!.apply(color: themeData.colorScheme.onSurface))))))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(maxWidth: maxWidthLocal, maxHeight: gridHeight), child: global::Doroti.Framework.Widgets.GridView.CreateCustom(physics: new global::Doroti.Framework.Widgets.NeverScrollableScrollPhysics(), gridDelegate: Date_pickerLibrary._monthItemGridDelegate, childrenDelegate: new global::Doroti.Framework.Widgets.SliverChildListDelegate(paddedDayItems, addRepaintBoundaries: false)))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(height: Date_pickerLibrary._monthItemFooterHeight)) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DayItem__date_picker : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual DateTime day { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.FocusNode focusNode { get; private set; } = default!;
    public virtual global::System.Action<DateTime> onChanged { get; private set; } = default!;
    public virtual global::System.Action<bool> onFocusChange { get; private set; } = default!;
    public virtual Color highlightColor { get; private set; } = default!;
    public virtual bool isDisabled { get; private set; } = default!;
    public virtual bool isRangeSelected { get; private set; } = default!;
    public virtual bool isSelectedDayStart { get; private set; } = default!;
    public virtual bool isSelectedDayEnd { get; private set; } = default!;
    public virtual bool isInRange { get; private set; } = default!;
    public virtual bool isOneDayRange { get; private set; } = default!;
    public virtual bool isToday { get; private set; } = default!;
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    internal _DayItem__date_picker(DateTime day, global::Doroti.Framework.Widgets.FocusNode focusNode, global::System.Action<DateTime> onChanged, global::System.Action<bool> onFocusChange, Color highlightColor, bool isDisabled, bool isRangeSelected, bool isSelectedDayStart, bool isSelectedDayEnd, bool isInRange, bool isOneDayRange, bool isToday, CalendarDelegate<DateTime> calendarDelegate)
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

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DayItemState__date_picker());
}

internal class _DayItemState__date_picker : global::Doroti.Framework.Widgets.State<_DayItem__date_picker>
{
    internal virtual global::Doroti.Framework.Widgets.WidgetStatesController _statesController { get; private set; } = new global::Doroti.Framework.Widgets.WidgetStatesController();

    public override void dispose()
    {
        this._statesController.dispose();
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData themeLocal = Theme.of(context);
        ColorScheme colorSchemeLocal = themeLocal.colorScheme;
        TextTheme textThemeLocal = themeLocal.textTheme;
        MaterialLocalizations localizations = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        DatePickerThemeData datePickerTheme = DatePickerTheme.of(context);
        DatePickerThemeData defaultsLocal = DatePickerTheme.defaults(context);
        global::Doroti.Ui.TextDirection textDirectionLocal = Directionality.of(context);
        global::Doroti.Ui.Color highlightColorLocal = ((global::Doroti.Ui.Color)(object?)((_DayItem__date_picker)this.widget).highlightColor);
        global::Doroti.Framework.Painting.ShapeDecoration? decorationLocal = default!;
        global::Doroti.Framework.Painting.TextStyle? itemStyle = textThemeLocal.bodyMedium;
        P? effectiveValue<P>(global::System.Func<DatePickerThemeData?, P?> getProperty)
        {
            return (getProperty(datePickerTheme) ?? getProperty(defaultsLocal));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        P? resolve<P>(global::System.Func<DatePickerThemeData?, global::Doroti.Framework.Widgets.WidgetStateProperty<P>?> getProperty, HashSet<global::Doroti.Framework.Widgets.WidgetState> states)
        {
            return effectiveValue(((theme) =>
            {
                return getProperty(theme) is { } property ? property.resolve(states) : default;
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        var statesLocal = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection108309 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (((_DayItem__date_picker)this.widget).isDisabled) { __collection108309.Add(global::Doroti.Framework.Widgets.WidgetState.disabled); } if ((((_DayItem__date_picker)this.widget).isSelectedDayStart || ((_DayItem__date_picker)this.widget).isSelectedDayEnd)) { __collection108309.Add(global::Doroti.Framework.Widgets.WidgetState.selected); } return __collection108309; }))();
        this._statesController.value = statesLocal;
        global::Doroti.Ui.Color? dayForegroundColorLocal = ((global::Doroti.Ui.Color?)(object?)resolve<global::Doroti.Ui.Color?>(((theme) => theme?.dayForegroundColor), statesLocal));
        global::Doroti.Ui.Color? dayBackgroundColorLocal = ((global::Doroti.Ui.Color?)(object?)resolve<global::Doroti.Ui.Color?>(((theme) => theme?.dayBackgroundColor), statesLocal));
        global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> dayOverlayColorLocal = ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>)(object?)WidgetStateProperty.resolveWith<global::Doroti.Ui.Color?>(((global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Ui.Color?>)((states) => effectiveValue(((theme) => (((_DayItem__date_picker)this.widget).isInRange ? theme?.rangeSelectionOverlayColor?.resolve(states) : theme?.dayOverlayColor?.resolve(states))))))));
        global::Doroti.Framework.Painting.OutlinedBorder dayShapeLocal = (resolve<global::Doroti.Framework.Painting.OutlinedBorder?>(((theme) => theme?.dayShape), statesLocal) ?? new global::Doroti.Framework.Painting.CircleBorder());
        _HighlightPainter__date_picker? highlightPainter = default!;
        if ((((_DayItem__date_picker)this.widget).isSelectedDayStart || ((_DayItem__date_picker)this.widget).isSelectedDayEnd))
        {
            itemStyle = itemStyle?.apply(color: dayForegroundColorLocal);
            decorationLocal = new global::Doroti.Framework.Painting.ShapeDecoration(color: dayBackgroundColorLocal, shape: dayShapeLocal);
            if ((((_DayItem__date_picker)this.widget).isRangeSelected && !((_DayItem__date_picker)this.widget).isOneDayRange))
            {
                _HighlightPainterStyle__date_picker styleLocal = (((_DayItem__date_picker)this.widget).isSelectedDayStart ? _HighlightPainterStyle__date_picker.highlightTrailing : _HighlightPainterStyle__date_picker.highlightLeading);
                highlightPainter = new _HighlightPainter__date_picker(color: highlightColorLocal, style: styleLocal, textDirection: textDirectionLocal);
            }
        }
        else
        {
            if (((_DayItem__date_picker)this.widget).isInRange)
            {
                highlightPainter = new _HighlightPainter__date_picker(color: highlightColorLocal, style: _HighlightPainterStyle__date_picker.highlightAll, textDirection: textDirectionLocal);
                if (((_DayItem__date_picker)this.widget).isDisabled)
                {
                    itemStyle = itemStyle?.apply(color: colorSchemeLocal.onSurface.withOpacity(0.38));
                }
            }
            else
            {
                if (((_DayItem__date_picker)this.widget).isDisabled)
                {
                    itemStyle = itemStyle?.apply(color: colorSchemeLocal.onSurface.withOpacity(0.38));
                }
                else
                {
                    if (((_DayItem__date_picker)this.widget).isToday)
                    {
                        itemStyle = itemStyle?.apply(color: colorSchemeLocal.primary);
                        global::Doroti.Framework.Painting.BorderSide todaySide = ((global::Doroti.Framework.Painting.BorderSide)(object?)((datePickerTheme.todayBorder ?? defaultsLocal.todayBorder!)).copyWith(color: colorSchemeLocal.primary));
                        decorationLocal = new global::Doroti.Framework.Painting.ShapeDecoration(shape: dayShapeLocal.copyWith(side: todaySide));
                    }
                }
            }
        }
        string dayText = ((string)(object?)localizations.formatDecimal(((_DayItem__date_picker)this.widget).day.Day));
        var semanticLabelSuffix = (((_DayItem__date_picker)this.widget).isToday ? $", {((MaterialLocalizations)localizations).currentDateLabel}" : "");
        var semanticLabel = $"{dayText}, {((_DayItem__date_picker)this.widget).calendarDelegate.formatFullDate(((_DayItem__date_picker)this.widget).day, localizations)}{semanticLabelSuffix}";
        if (((_DayItem__date_picker)this.widget).isSelectedDayStart)
        {
            semanticLabel = localizations.dateRangeStartDateSemanticLabel(semanticLabel);
        }
        else
        {
            if (((_DayItem__date_picker)this.widget).isSelectedDayEnd)
            {
                semanticLabel = localizations.dateRangeEndDateSemanticLabel(semanticLabel);
            }
        }
        global::Doroti.Framework.Widgets.Widget dayWidget = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Container(decoration: decorationLocal, alignment: global::Doroti.Framework.Painting.Alignment.center, child: new global::Doroti.Framework.Widgets.Semantics(label: semanticLabel, selected: (((_DayItem__date_picker)this.widget).isSelectedDayStart || ((_DayItem__date_picker)this.widget).isSelectedDayEnd), child: new global::Doroti.Framework.Widgets.ExcludeSemantics(child: new global::Doroti.Framework.Widgets.Text(dayText, style: itemStyle)))));
        if ((highlightPainter is not null))
        {
            dayWidget = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.CustomPaint(painter: highlightPainter, child: dayWidget));
        }
        if (!((_DayItem__date_picker)this.widget).isDisabled)
        {
            dayWidget = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new InkResponse(focusNode: ((_DayItem__date_picker)this.widget).focusNode, onTap: (() => { this.widget.onChanged(((_DayItem__date_picker)this.widget).day); }), customBorder: dayShapeLocal, containedInkWell: true, statesController: this._statesController, overlayColor: dayOverlayColorLocal, onFocusChange: ((_DayItem__date_picker)this.widget).onFocusChange, child: dayWidget));
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
    highlightAll
}

internal class _HighlightPainter__date_picker : global::Doroti.Framework.Rendering.CustomPainter
{
    public virtual Color color { get; private set; } = default!;
    public virtual _HighlightPainterStyle__date_picker style { get; private set; } = default!;
    public virtual TextDirection? textDirection { get; private set; }

    internal _HighlightPainter__date_picker(Color color, _HighlightPainterStyle__date_picker style = _HighlightPainterStyle__date_picker.none, TextDirection? textDirection = null)
    {
        this.color = color;
        this.style = style;
        this.textDirection = textDirection;
    }

    public override void paint(Canvas canvas, Size size)
    {
        if ((object.Equals(this.style, _HighlightPainterStyle__date_picker.none)))
        {
            return;
        }
        var paintLocal = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = this.color;
    __cascade.style = PaintingStyle.fill;
    return __cascade;
}))();
        bool rtlLocal = (this.textDirection switch { TextDirection.rtl => true, null => true, TextDirection.ltr => false, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        switch (this.style)
        {
            case _HighlightPainterStyle__date_picker.highlightLeading when (rtlLocal):
            case _HighlightPainterStyle__date_picker.highlightTrailing when (!rtlLocal):
                {
                    canvas.drawRect(global::Doroti.Ui.Rect.fromLTWH((size.width / 2L), 0, (size.width / 2L), size.height), paintLocal);
                    break;
                }
            case _HighlightPainterStyle__date_picker.highlightLeading:
            case _HighlightPainterStyle__date_picker.highlightTrailing:
                {
                    canvas.drawRect(global::Doroti.Ui.Rect.fromLTWH(0, 0, (size.width / 2L), size.height), paintLocal);
                    break;
                }
            case _HighlightPainterStyle__date_picker.highlightAll:
                {
                    canvas.drawRect(global::Doroti.Ui.Rect.fromLTWH(0, 0, size.width, size.height), paintLocal);
                    break;
                }
            case _HighlightPainterStyle__date_picker.none:
                {
                    break;
                }
        }
    }

    public override bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate) => false;
}

internal class _InputDateRangePickerDialog__date_picker : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual DateTime? selectedStartDate { get; private set; }
    public virtual DateTime? selectedEndDate { get; private set; }
    public virtual DateTime? currentDate { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget picker { get; private set; } = default!;
    public virtual global::System.Action onConfirm { get; private set; } = default!;
    public virtual global::System.Action onCancel { get; private set; } = default!;
    public virtual string? confirmText { get; private set; }
    public virtual string? cancelText { get; private set; }
    public virtual string? helpText { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? entryModeButton { get; private set; }
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    internal _InputDateRangePickerDialog__date_picker(DateTime? selectedStartDate, DateTime? selectedEndDate, DateTime? currentDate, global::Doroti.Framework.Widgets.Widget picker, global::System.Action onConfirm, global::System.Action onCancel, string? confirmText, string? cancelText, string? helpText, global::Doroti.Framework.Widgets.Widget? entryModeButton, CalendarDelegate<DateTime> calendarDelegate)
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

    internal virtual string _formatDateRange(global::Doroti.Framework.Widgets.BuildContext context, DateTime? start, DateTime? end, DateTime now)
    {
        MaterialLocalizations localizations = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        string startText = Date_pickerLibrary._formatRangeStartDate(localizations, this.calendarDelegate, start, end);
        string endText = Date_pickerLibrary._formatRangeEndDate(localizations, this.calendarDelegate, start, end, now);
        if (((start is null) || (end is null)))
        {
            return ((MaterialLocalizations)localizations).unspecifiedDateRange;
        }
        return (Directionality.of(context) switch { TextDirection.rtl => $"{endText} – {startText}", TextDirection.ltr => $"{startText} – {endText}", _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        bool useMaterial3Local = Theme.of(context).useMaterial3;
        MaterialLocalizations localizations = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        global::Doroti.Framework.Widgets.Orientation orientationLocal = MediaQuery.orientationOf(context);
        DatePickerThemeData datePickerTheme = DatePickerTheme.of(context);
        DatePickerThemeData defaultsLocal = DatePickerTheme.defaults(context);
        global::Doroti.Framework.Painting.TextStyle? headlineStyle = (((object.Equals(orientationLocal, global::Doroti.Framework.Widgets.Orientation.portrait))) ? (datePickerTheme.headerHeadlineStyle ?? defaultsLocal.headerHeadlineStyle) : Theme.of(context).textTheme.headlineSmall);
        global::Doroti.Ui.Color? headerForegroundColorLocal = ((global::Doroti.Ui.Color?)(object?)(datePickerTheme.headerForegroundColor ?? defaultsLocal.headerForegroundColor));
        headlineStyle = headlineStyle?.copyWith(color: headerForegroundColorLocal);
        string dateText = ((string)(object?)_formatDateRange(context, this.selectedStartDate, this.selectedEndDate, DartRuntimePrimitives.RequireValue(this.currentDate)));
        var semanticDateText = (((this.selectedStartDate is not null) && (this.selectedEndDate is not null)) ? $"{this.calendarDelegate.formatMediumDate(DartRuntimePrimitives.RequireValue(this.selectedStartDate), localizations)} – {this.calendarDelegate.formatMediumDate(DartRuntimePrimitives.RequireValue(this.selectedEndDate), localizations)}" : "");
        global::Doroti.Framework.Widgets.Widget header = ((global::Doroti.Framework.Widgets.Widget)(object?)new _DatePickerHeader__date_picker(helpText: (this.helpText ?? ((useMaterial3Local ? ((MaterialLocalizations)localizations).dateRangePickerHelpText : ((MaterialLocalizations)localizations).dateRangePickerHelpText.toUpperCase()))), titleText: dateText, titleSemanticsLabel: semanticDateText, titleStyle: headlineStyle, orientation: orientationLocal, isShort: (object.Equals(orientationLocal, global::Doroti.Framework.Widgets.Orientation.landscape)), entryModeButton: this.entryModeButton));
        global::Doroti.Framework.Widgets.Widget actions = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(minHeight: 52.0), child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 8), child: new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.AlignmentDirectional.centerEnd, child: new global::Doroti.Framework.Widgets.OverflowBar(spacing: 8, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new TextButton(onPressed: this.onCancel, child: new global::Doroti.Framework.Widgets.Text((this.cancelText ?? ((useMaterial3Local ? ((MaterialLocalizations)localizations).cancelButtonLabel : ((MaterialLocalizations)localizations).cancelButtonLabel.toUpperCase())))))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new TextButton(onPressed: this.onConfirm, child: new global::Doroti.Framework.Widgets.Text(((this.confirmText ?? (string)((MaterialLocalizations)localizations).okButtonLabel))))) })))));
        double textScaleFactor = (MediaQuery.textScalerOf(context).clamp(maxScaleFactor: Date_pickerLibrary._kMaxRangeTextScaleFactor).scale(Calendar_date_pickerLibrary._fontSizeToScale) / Calendar_date_pickerLibrary._fontSizeToScale);
        global::Doroti.Ui.Size dialogSize = ((global::Doroti.Ui.Size)(object?)(((useMaterial3Local ? Date_pickerLibrary._inputPortraitDialogSizeM3 : Date_pickerLibrary._inputPortraitDialogSizeM2)) * textScaleFactor));
        switch (orientationLocal)
        {
            case global::Doroti.Framework.Widgets.Orientation.portrait:
                {
                    return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.LayoutBuilder(builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Rendering.BoxConstraints, global::Doroti.Framework.Widgets.Widget>)((context, constraints) =>
                    {
                        global::Doroti.Ui.Size portraitDialogSize = ((global::Doroti.Ui.Size)(object?)(useMaterial3Local ? Date_pickerLibrary._inputPortraitDialogSizeM3 : Date_pickerLibrary._inputPortraitDialogSizeM2));
                        bool isFullyPortrait = (((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxHeight >= Math.Min(dialogSize.height, portraitDialogSize.height));
                        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Column(mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.stretch, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection120213 = new List<global::Doroti.Framework.Widgets.Widget>(); if (isFullyPortrait) { __collection120213.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(header)); } __collection120213.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: this.picker))); __collection120213.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(actions)); return __collection120213; }))()));
                        throw new InvalidOperationException("Dart closure completed without a value.");
                    }))));
                }
            case global::Doroti.Framework.Widgets.Orientation.landscape:
                {
                    return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Row(mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.stretch, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(header), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: new global::Doroti.Framework.Widgets.Column(mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.stretch, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: this.picker)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(actions) }))) }));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _InputDateRangePicker__date_picker : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual DateTime? initialStartDate { get; private set; }
    public virtual DateTime? initialEndDate { get; private set; }
    public virtual DateTime firstDate { get; private set; } = default!;
    public virtual DateTime lastDate { get; private set; } = default!;
    public virtual global::System.Action<DateTime?>? onStartDateChanged { get; private set; }
    public virtual global::System.Action<DateTime?>? onEndDateChanged { get; private set; }
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
    public virtual global::Doroti.Framework.Services.TextInputType keyboardType { get; private set; } = default!;
    public virtual global::System.Func<DateTime, DateTime?, DateTime?, bool>? selectableDayPredicate { get; private set; }
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    internal _InputDateRangePicker__date_picker(global::Doroti.Framework.Foundation.Key? key = null, DateTime? initialStartDate = null, DateTime? initialEndDate = null, DateTime firstDate = default!, DateTime lastDate = default!, global::System.Action<DateTime?>? onStartDateChanged = default!, global::System.Action<DateTime?>? onEndDateChanged = default!, global::System.Func<DateTime, DateTime?, DateTime?, bool>? selectableDayPredicate = default!, CalendarDelegate<DateTime> calendarDelegate = default!, string? helpText = null, string? errorFormatText = null, string? errorInvalidText = null, string? errorInvalidRangeText = null, string? fieldStartHintText = null, string? fieldEndHintText = null, string? fieldStartLabelText = null, string? fieldEndLabelText = null, bool autofocus = false, bool autovalidate = false, global::Doroti.Framework.Services.TextInputType keyboardType = default!) : base(key: key)
    {
        global::Doroti.Framework.Services.TextInputType __keyboardType = keyboardType ?? global::Doroti.Framework.Services.TextInputType.datetime;
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
        this.initialStartDate = ((initialStartDate is null) ? null : calendarDelegate.dateOnly(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(initialStartDate))));
        this.initialEndDate = ((initialEndDate is null) ? null : calendarDelegate.dateOnly(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(initialEndDate))));
        this.firstDate = calendarDelegate.dateOnly(firstDate);
        this.lastDate = calendarDelegate.dateOnly(lastDate);
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _InputDateRangePickerState__date_picker());
}

internal class _InputDateRangePickerState__date_picker : global::Doroti.Framework.Widgets.State<_InputDateRangePicker__date_picker>
{
    internal virtual string _startInputText { get; set; } = default!;
    internal virtual string _endInputText { get; set; } = default!;
    internal virtual DateTime? _startDate { get; set; } = default;
    internal virtual DateTime? _endDate { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.TextEditingController _startController { get; set; } = default!;
    internal virtual global::Doroti.Framework.Widgets.TextEditingController _endController { get; set; } = default!;
    internal virtual string? _startErrorText { get; set; } = default;
    internal virtual string? _endErrorText { get; set; } = default;
    internal virtual bool _autoSelected { get; set; } = false;

    public override void initState()
    {
        base.initState();
        _startDate = ((_InputDateRangePicker__date_picker)this.widget).initialStartDate;
        _startController = new global::Doroti.Framework.Widgets.TextEditingController();
        _endDate = ((_InputDateRangePicker__date_picker)this.widget).initialEndDate;
        _endController = new global::Doroti.Framework.Widgets.TextEditingController();
    }

    public override void dispose()
    {
        this._startController.dispose();
        this._endController.dispose();
        base.dispose();
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        MaterialLocalizations localizations = ((MaterialLocalizations)(object?)MaterialLocalizations.of(this.context));
        if ((this._startDate is not null))
        {
            _startInputText = ((_InputDateRangePicker__date_picker)this.widget).calendarDelegate.formatCompactDate(DartRuntimePrimitives.RequireValue(this._startDate), localizations);
            bool selectText = (((_InputDateRangePicker__date_picker)this.widget).autofocus && !this._autoSelected);
            _updateController(this._startController, this._startInputText, selectText);
            _autoSelected = selectText;
        }
        if ((this._endDate is not null))
        {
            _endInputText = ((_InputDateRangePicker__date_picker)this.widget).calendarDelegate.formatCompactDate(DartRuntimePrimitives.RequireValue(this._endDate), localizations);
            _updateController(this._endController, this._endInputText, false);
        }
    }

    public virtual bool validate()
    {
        string? startError = ((string?)(object?)_validateDate(this._startDate));
        string? endError = ((string?)(object?)_validateDate(this._endDate));
        if (((startError is null) && (endError is null)))
        {
            if (DartRuntimePrimitives.RequireValue(this._startDate).isAfter(DartRuntimePrimitives.RequireValue(this._endDate)))
            {
                startError = (((_InputDateRangePicker__date_picker)this.widget).errorInvalidRangeText ?? MaterialLocalizations.of(this.context).invalidDateRangeLabel);
            }
        }
        setState(((global::System.Action)(() =>
        {
            _startErrorText = startError;
            _endErrorText = endError;
        })));
        return ((startError is null) && (endError is null));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual DateTime? _parseDate(string? text)
    {
        MaterialLocalizations localizations = ((MaterialLocalizations)(object?)MaterialLocalizations.of(this.context));
        return ((_InputDateRangePicker__date_picker)this.widget).calendarDelegate.parseCompactDate(text, localizations);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual string? _validateDate(DateTime? date)
    {
        if ((date is null))
        {
            return (((_InputDateRangePicker__date_picker)this.widget).errorFormatText ?? MaterialLocalizations.of(this.context).invalidDateFormatLabel);
        }
        else
        {
            if (!_isDaySelectable(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(date))))
            {
                return (((_InputDateRangePicker__date_picker)this.widget).errorInvalidText ?? MaterialLocalizations.of(this.context).dateOutOfRangeLabel);
            }
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isDaySelectable(DateTime day)
    {
        if ((day.isBefore(((_InputDateRangePicker__date_picker)this.widget).firstDate) || day.isAfter(((_InputDateRangePicker__date_picker)this.widget).lastDate)))
        {
            return false;
        }
        if ((((_InputDateRangePicker__date_picker)this.widget).selectableDayPredicate is null))
        {
            return true;
        }
        return ((_InputDateRangePicker__date_picker)this.widget).selectableDayPredicate!(day, this._startDate, this._endDate);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _updateController(global::Doroti.Framework.Widgets.TextEditingController controller, string text, bool selectText)
    {
        global::Doroti.Framework.Services.TextEditingValue textEditingValue = ((global::Doroti.Framework.Services.TextEditingValue)(object?)controller.value.copyWith(text: text));
        if (selectText)
        {
            textEditingValue = textEditingValue.copyWith(selection: new global::Doroti.Framework.Services.TextSelection(baseOffset: 0L, extentOffset: text.Length));
        }
        controller.value = textEditingValue;
    }

    internal virtual void _handleStartChanged(string text)
    {
        setState(((global::System.Action)(() =>
        {
            _startInputText = text;
            _startDate = _parseDate(text);
            ((_InputDateRangePicker__date_picker)this.widget).onStartDateChanged?.Invoke(this._startDate);
        })));
        if (((_InputDateRangePicker__date_picker)this.widget).autovalidate)
        {
            validate();
        }
    }

    internal virtual void _handleEndChanged(string text)
    {
        setState(((global::System.Action)(() =>
        {
            _endInputText = text;
            _endDate = _parseDate(text);
            ((_InputDateRangePicker__date_picker)this.widget).onEndDateChanged?.Invoke(this._endDate);
        })));
        if (((_InputDateRangePicker__date_picker)this.widget).autovalidate)
        {
            validate();
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        bool useMaterial3Local = theme.useMaterial3;
        MaterialLocalizations localizations = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        InputDecorationThemeData inputTheme = ((InputDecorationThemeData)(object?)InputDecorationTheme.of(context));
        InputBorder inputBorder = (((InputDecorationThemeData)inputTheme).border ?? ((useMaterial3Local ? new OutlineInputBorder() : new UnderlineInputBorder())));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Row(crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.start, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new TextField(controller: this._startController, decoration: new InputDecoration(border: inputBorder, filled: ((InputDecorationThemeData)inputTheme).filled, hintText: ((((_InputDateRangePicker__date_picker)this.widget).fieldStartHintText ?? (string)((_InputDateRangePicker__date_picker)this.widget).calendarDelegate.dateHelpText(localizations))), labelText: ((((_InputDateRangePicker__date_picker)this.widget).fieldStartLabelText ?? (string)((MaterialLocalizations)localizations).dateRangeStartLabel)), errorText: this._startErrorText), keyboardType: ((_InputDateRangePicker__date_picker)this.widget).keyboardType, onChanged: this._handleStartChanged, autofocus: ((_InputDateRangePicker__date_picker)this.widget).autofocus))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(width: 8)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new TextField(controller: this._endController, decoration: new InputDecoration(border: inputBorder, filled: ((InputDecorationThemeData)inputTheme).filled, hintText: ((((_InputDateRangePicker__date_picker)this.widget).fieldEndHintText ?? (string)((_InputDateRangePicker__date_picker)this.widget).calendarDelegate.dateHelpText(localizations))), labelText: ((((_InputDateRangePicker__date_picker)this.widget).fieldEndLabelText ?? (string)((MaterialLocalizations)localizations).dateRangeEndLabel)), errorText: this._endErrorText), keyboardType: ((_InputDateRangePicker__date_picker)this.widget).keyboardType, onChanged: this._handleEndChanged))) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
