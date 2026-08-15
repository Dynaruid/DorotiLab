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

namespace Doroti.Generated.Framework.Material;

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
    public static async Future<DateTime?> showDatePicker(global::Doroti.Generated.Framework.Widgets.BuildContext context, DateTime? initialDate = null, DateTime firstDate = default!, DateTime lastDate = default!, DateTime? currentDate = null, DatePickerEntryMode initialEntryMode = DatePickerEntryMode.calendar, global::System.Func<DateTime, bool>? selectableDayPredicate = null, string? helpText = null, string? cancelText = null, string? confirmText = null, Locale? locale = null, bool barrierDismissible = true, Color? barrierColor = null, string? barrierLabel = null, bool useRootNavigator = true, global::Doroti.Generated.Framework.Widgets.RouteSettings? routeSettings = null, TextDirection? textDirection = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>? builder = null, DatePickerMode initialDatePickerMode = DatePickerMode.day, string? errorFormatText = null, string? errorInvalidText = null, string? fieldHintText = null, string? fieldLabelText = null, global::Doroti.Generated.Framework.Services.TextInputType? keyboardType = null, Offset? anchorPoint = null, global::System.Action<DatePickerEntryMode>? onDatePickerModeChange = null, global::Doroti.Generated.Framework.Widgets.Icon? switchToInputEntryModeIcon = null, global::Doroti.Generated.Framework.Widgets.Icon? switchToCalendarEntryModeIcon = null, CalendarDelegate<DateTime> calendarDelegate = default!)
    {
        initialDate = ((initialDate is null) ? null : calendarDelegate.dateOnly(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(initialDate))));
        firstDate = calendarDelegate.dateOnly(firstDate);
        lastDate = calendarDelegate.dateOnly(lastDate);
        DartRuntimePrimitives.Assert(() => !lastDate.isBefore(firstDate), () => (object?)$"lastDate {lastDate} must be on or after firstDate {firstDate}.");
        DartRuntimePrimitives.Assert(() => ((initialDate is null) || !DartRuntimePrimitives.RequireValue(initialDate).isBefore(firstDate)), () => (object?)$"initialDate {DartRuntimePrimitives.RequireValue(initialDate)} must be on or after firstDate {firstDate}.");
        DartRuntimePrimitives.Assert(() => ((initialDate is null) || !DartRuntimePrimitives.RequireValue(initialDate).isAfter(lastDate)), () => (object?)$"initialDate {DartRuntimePrimitives.RequireValue(initialDate)} must be on or before lastDate {lastDate}.");
        DartRuntimePrimitives.Assert(() => (((selectableDayPredicate is null) || (initialDate is null)) || selectableDayPredicate(DartRuntimePrimitives.RequireValue(initialDate))), () => (object?)$"Provided initialDate {DartRuntimePrimitives.RequireValue(initialDate)} must satisfy provided selectableDayPredicate.");
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        global::Doroti.Generated.Framework.Widgets.Widget dialog__10594 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new DatePickerDialog(initialDate: initialDate, firstDate: firstDate, lastDate: lastDate, currentDate: currentDate, initialEntryMode: initialEntryMode, selectableDayPredicate: (global::System.Func<DateTime, bool>?)selectableDayPredicate, helpText: helpText, cancelText: cancelText, confirmText: confirmText, initialCalendarMode: initialDatePickerMode, errorFormatText: errorFormatText, errorInvalidText: errorInvalidText, fieldHintText: fieldHintText, fieldLabelText: fieldLabelText, keyboardType: keyboardType, onDatePickerModeChange: (global::System.Action<DatePickerEntryMode>?)onDatePickerModeChange, switchToInputEntryModeIcon: switchToInputEntryModeIcon, switchToCalendarEntryModeIcon: switchToCalendarEntryModeIcon, calendarDelegate: calendarDelegate));
        if ((textDirection is not null))
        {
            TextDirection textDirection__value11363 = DartRuntimePrimitives.RequireValue(textDirection);
            dialog__10594 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Directionality(textDirection: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(textDirection__value11363)), child: dialog__10594));
        }
        if ((locale is not null))
        {
            Locale locale__value11473 = DartRuntimePrimitives.RequireValue(locale);
            dialog__10594 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(global::Doroti.Generated.Framework.Widgets.Localizations.CreateOverride(context: context, locale: DartRuntimePrimitives.RequireValue(locale__value11473), child: dialog__10594));
        }
        else
        {
            DatePickerThemeData datePickerTheme__11618 = DatePickerTheme.of(context);
            if ((datePickerTheme__11618.locale is not null))
            {
                dialog__10594 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(global::Doroti.Generated.Framework.Widgets.Localizations.CreateOverride(context: context, locale: datePickerTheme__11618.locale, child: dialog__10594));
            }
        }
        return await DialogLibrary.showDialog<DateTime>(context: context, barrierDismissible: barrierDismissible, barrierColor: barrierColor, barrierLabel: barrierLabel, useRootNavigator: useRootNavigator, routeSettings: routeSettings, builder: ((context) => {
return ((builder is null) ? dialog__10594 : builder(context, dialog__10594));
throw new InvalidOperationException("Dart closure completed without a value.");
}), anchorPoint: DartRuntimePrimitives.RequireValue(anchorPoint));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class DatePickerDialog : global::Doroti.Generated.Framework.Widgets.StatefulWidget
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
    public virtual global::Doroti.Generated.Framework.Services.TextInputType? keyboardType { get; private set; }
    public virtual string? restorationId { get; private set; }
    public virtual global::System.Action<DatePickerEntryMode>? onDatePickerModeChange { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Icon? switchToInputEntryModeIcon { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Icon? switchToCalendarEntryModeIcon { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsets insetPadding { get; private set; } = default!;
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    public DatePickerDialog(global::Doroti.Generated.Framework.Foundation.Key? key = null, DateTime? initialDate = null, DateTime firstDate = default!, DateTime lastDate = default!, DateTime? currentDate = null, DatePickerEntryMode initialEntryMode = DatePickerEntryMode.calendar, global::System.Func<DateTime, bool>? selectableDayPredicate = null, string? cancelText = null, string? confirmText = null, string? helpText = null, DatePickerMode initialCalendarMode = DatePickerMode.day, string? errorFormatText = null, string? errorInvalidText = null, string? fieldHintText = null, string? fieldLabelText = null, global::Doroti.Generated.Framework.Services.TextInputType? keyboardType = null, string? restorationId = null, global::System.Action<DatePickerEntryMode>? onDatePickerModeChange = null, global::Doroti.Generated.Framework.Widgets.Icon? switchToInputEntryModeIcon = null, global::Doroti.Generated.Framework.Widgets.Icon? switchToCalendarEntryModeIcon = null, global::Doroti.Generated.Framework.Painting.EdgeInsets insetPadding = default!, CalendarDelegate<DateTime> calendarDelegate = default!) : base(key: key)
    {
        global::Doroti.Generated.Framework.Painting.EdgeInsets __insetPadding = insetPadding ?? global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 16.0, vertical: 24.0);
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

internal class _DatePickerDialogState__date_picker : global::Doroti.Generated.Framework.Widgets.State<DatePickerDialog>, global::Doroti.Generated.Framework.Widgets.RestorationMixin<DatePickerDialog>
{
    private bool __late__selectedDate_initialized;
    private global::Doroti.Generated.Framework.Widgets.RestorableDateTimeN __late__selectedDate = default!;
    internal virtual global::Doroti.Generated.Framework.Widgets.RestorableDateTimeN _selectedDate
    {
        get
        {
            if (!__late__selectedDate_initialized)
            {
                __late__selectedDate = new global::Doroti.Generated.Framework.Widgets.RestorableDateTimeN(((DatePickerDialog)this.widget).initialDate);
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
    internal virtual _RestorableAutovalidateMode__date_picker _autovalidateMode { get; private set; } = new _RestorableAutovalidateMode__date_picker(global::Doroti.Generated.Framework.Widgets.AutovalidateMode.disabled);
    internal virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> _calendarPickerKey { get; private set; } = global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create();
    internal virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<global::Doroti.Generated.Framework.Widgets.FormState> _formKey { get; private set; } = global::Doroti.Generated.Framework.Widgets.GlobalKey<global::Doroti.Generated.Framework.Widgets.FormState>.Create();
    internal static DartMap<global::Doroti.Generated.Framework.Widgets.ShortcutActivator, global::Doroti.Generated.Framework.Widgets.Intent> _formShortcutMap = new DartMap<global::Doroti.Generated.Framework.Widgets.ShortcutActivator, global::Doroti.Generated.Framework.Widgets.Intent> { [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.enter)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new global::Doroti.Generated.Framework.Widgets.NextFocusIntent()) };
    public virtual global::Doroti.Generated.Framework.Services.RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<dynamic, global::System.Action> _properties { get; set; } = new DartMap<dynamic, global::System.Action>();
    public virtual List<dynamic>? _debugPropertiesWaitingForReregistration { get; set; } = default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual global::Doroti.Generated.Framework.Services.RestorationBucket? _currentParent { get; set; } = default;

    public override void dispose()
    {
        this._selectedDate.dispose();
        this._entryMode.dispose();
        this._autovalidateMode.dispose();
        this._properties.forEach(((global::System.Action<dynamic, global::System.Action>)((property, listener) => {
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
    public virtual void restoreState(global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket, bool initialRestore)
    {
        registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this._selectedDate), "selected_date");
        registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this._autovalidateMode), "autovalidateMode");
        registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this._entryMode), "calendar_entry_mode");
    }

    internal virtual void _handleOk()
    {
        if (((object.Equals(this._entryMode.value, DatePickerEntryMode.input)) || (object.Equals(this._entryMode.value, DatePickerEntryMode.inputOnly))))
        {
            global::Doroti.Generated.Framework.Widgets.FormState form__19636 = ((global::Doroti.Generated.Framework.Widgets.GlobalKey<global::Doroti.Generated.Framework.Widgets.FormState>)this._formKey).currentState!;
            if (!form__19636.validate())
            {
                setState(((global::System.Action)(() => { _ = this._autovalidateMode.value = global::Doroti.Generated.Framework.Widgets.AutovalidateMode.always; })));
                return;
            }
            form__19636.save();
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
        setState(((global::System.Action)(() => {
switch (this._entryMode.value)
{
    case DatePickerEntryMode.calendar:
        {
            this._autovalidateMode.value = global::Doroti.Generated.Framework.Widgets.AutovalidateMode.disabled;
            this._entryMode.value = DatePickerEntryMode.input;
            _handleOnDatePickerModeChange();
            break;
        }
    case DatePickerEntryMode.input:
        {
            ((global::Doroti.Generated.Framework.Widgets.GlobalKey<global::Doroti.Generated.Framework.Widgets.FormState>)this._formKey).currentState!.save();
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

    internal virtual global::Doroti.Ui.Size _dialogSize(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        bool useMaterial3__20857 = Theme.of(context).useMaterial3;
        bool isCalendar__20919 = (this._entryMode.value switch { DatePickerEntryMode.calendar => true, DatePickerEntryMode.calendarOnly => true, DatePickerEntryMode.input => false, DatePickerEntryMode.inputOnly => false, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Generated.Framework.Widgets.Orientation orientation__21144 = MediaQuery.orientationOf(context);
        return ((isCalendar__20919, orientation__21144) switch { (true, global::Doroti.Generated.Framework.Widgets.Orientation.portrait) when (useMaterial3__20857) => Date_pickerLibrary._calendarPortraitDialogSizeM3, (false, global::Doroti.Generated.Framework.Widgets.Orientation.portrait) when (useMaterial3__20857) => Date_pickerLibrary._inputPortraitDialogSizeM3, (true, global::Doroti.Generated.Framework.Widgets.Orientation.portrait) => Date_pickerLibrary._calendarPortraitDialogSizeM2, (false, global::Doroti.Generated.Framework.Widgets.Orientation.portrait) => Date_pickerLibrary._inputPortraitDialogSizeM2, (true, global::Doroti.Generated.Framework.Widgets.Orientation.landscape) => Date_pickerLibrary._calendarLandscapeDialogSize, (false, global::Doroti.Generated.Framework.Widgets.Orientation.landscape) => Date_pickerLibrary._inputLandscapeDialogSize });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ThemeData theme__22016 = Theme.of(context);
        bool useMaterial3__22058 = theme__22016.useMaterial3;
        MaterialLocalizations localizations__22125 = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        global::Doroti.Generated.Framework.Widgets.Orientation orientation__22198 = MediaQuery.orientationOf(context);
        var isLandscapeOrientation__22257 = (object.Equals(orientation__22198, global::Doroti.Generated.Framework.Widgets.Orientation.landscape));
        DatePickerThemeData datePickerTheme__22350 = DatePickerTheme.of(context);
        DatePickerThemeData defaults__22427 = DatePickerTheme.defaults(context);
        TextTheme textTheme__22493 = theme__22016.textTheme;
        global::Doroti.Generated.Framework.Painting.TextStyle? headlineStyle__22767 = default!;
        if (useMaterial3__22058)
        {
            headlineStyle__22767 = (datePickerTheme__22350.headerHeadlineStyle ?? defaults__22427.headerHeadlineStyle);
            switch (this._entryMode.value)
            {
                case DatePickerEntryMode.input:
                case DatePickerEntryMode.inputOnly:
                    {
                        if ((object.Equals(orientation__22198, global::Doroti.Generated.Framework.Widgets.Orientation.landscape)))
                        {
                            headlineStyle__22767 = textTheme__22493.headlineSmall;
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
            headlineStyle__22767 = (isLandscapeOrientation__22257 ? textTheme__22493.headlineSmall : textTheme__22493.headlineMedium);
        }
        global::Doroti.Ui.Color? headerForegroundColor__23396 = ((global::Doroti.Ui.Color?)(object?)(datePickerTheme__22350.headerForegroundColor ?? defaults__22427.headerForegroundColor));
        headlineStyle__22767 = headlineStyle__22767?.copyWith(color: headerForegroundColor__23396);
        global::Doroti.Generated.Framework.Widgets.Widget actions__23594 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Generated.Framework.Rendering.BoxConstraints(minHeight: 52.0), child: MediaQuery.withClampedTextScaling(maxScaleFactor: (isLandscapeOrientation__22257 ? 1.6 : Calendar_date_pickerLibrary._kMaxTextScaleFactor), child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 8), child: new global::Doroti.Generated.Framework.Widgets.Align(alignment: global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerEnd, child: new global::Doroti.Generated.Framework.Widgets.OverflowBar(spacing: 8, children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new TextButton(style: (datePickerTheme__22350.cancelButtonStyle ?? defaults__22427.cancelButtonStyle), onPressed: this._handleCancel, child: new global::Doroti.Generated.Framework.Widgets.Text((((DatePickerDialog)this.widget).cancelText ?? ((useMaterial3__22058 ? ((MaterialLocalizations)localizations__22125).cancelButtonLabel : ((MaterialLocalizations)localizations__22125).cancelButtonLabel.toUpperCase())))))), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new TextButton(style: (datePickerTheme__22350.confirmButtonStyle ?? defaults__22427.confirmButtonStyle), onPressed: this._handleOk, child: new global::Doroti.Generated.Framework.Widgets.Text(((((DatePickerDialog)this.widget).confirmText ?? (string)((MaterialLocalizations)localizations__22125).okButtonLabel))))) }))))));
        CalendarDatePicker calendarDatePicker()
        {
            return new CalendarDatePicker(calendarDelegate: ((DatePickerDialog)this.widget).calendarDelegate, key: this._calendarPickerKey, initialDate: this._selectedDate.value, firstDate: ((DatePickerDialog)this.widget).firstDate, lastDate: ((DatePickerDialog)this.widget).lastDate, currentDate: ((DatePickerDialog)this.widget).currentDate, onDateChanged: (global::System.Action<DateTime>)this._handleDateChanged, selectableDayPredicate: (global::System.Func<DateTime, bool>?)((DatePickerDialog)this.widget).selectableDayPredicate, initialCalendarMode: ((DatePickerDialog)this.widget).initialCalendarMode);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        global::Doroti.Generated.Framework.Widgets.Form inputDatePicker()
        {
            return new global::Doroti.Generated.Framework.Widgets.Form(key: this._formKey, autovalidateMode: this._autovalidateMode.value, child: new global::Doroti.Generated.Framework.Widgets.SizedBox(height: ((object.Equals(orientation__22198, global::Doroti.Generated.Framework.Widgets.Orientation.portrait)) ? Date_pickerLibrary._inputFormPortraitHeight : Date_pickerLibrary._inputFormLandscapeHeight), child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 24), child: new global::Doroti.Generated.Framework.Widgets.Shortcuts(shortcuts: _formShortcutMap, child: new global::Doroti.Generated.Framework.Widgets.Column(mainAxisAlignment: global::Doroti.Generated.Framework.Rendering.MainAxisAlignment.center, children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Flexible(child: MediaQuery.withClampedTextScaling(maxScaleFactor: 2.0, child: new InputDatePickerFormField(calendarDelegate: ((DatePickerDialog)this.widget).calendarDelegate, initialDate: this._selectedDate.value, firstDate: ((DatePickerDialog)this.widget).firstDate, lastDate: ((DatePickerDialog)this.widget).lastDate, onDateSubmitted: (global::System.Action<DateTime>)this._handleDateChanged, onDateSaved: (global::System.Action<DateTime>)this._handleDateChanged, selectableDayPredicate: (global::System.Func<DateTime, bool>?)((DatePickerDialog)this.widget).selectableDayPredicate, errorFormatText: ((DatePickerDialog)this.widget).errorFormatText, errorInvalidText: ((DatePickerDialog)this.widget).errorInvalidText, fieldHintText: ((DatePickerDialog)this.widget).fieldHintText, fieldLabelText: ((DatePickerDialog)this.widget).fieldLabelText, keyboardType: ((DatePickerDialog)this.widget).keyboardType, autofocus: true)))) })))));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        global::Doroti.Generated.Framework.Widgets.Widget picker__27065 = default!;
        global::Doroti.Generated.Framework.Widgets.Widget? entryModeButton__27091 = default!;
        switch (this._entryMode.value)
        {
            case DatePickerEntryMode.calendar:
                {
                    picker__27065 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(calendarDatePicker());
                    entryModeButton__27091 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new IconButton(icon: (((DatePickerDialog)this.widget).switchToInputEntryModeIcon ?? new global::Doroti.Generated.Framework.Widgets.Icon((useMaterial3__22058 ? Icons.edit_outlined : Icons.edit))), color: headerForegroundColor__23396, tooltip: ((MaterialLocalizations)localizations__22125).inputDateModeButtonLabel, onPressed: this._handleEntryModeToggle));
                    break;
                }
            case DatePickerEntryMode.calendarOnly:
                {
                    picker__27065 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(calendarDatePicker());
                    entryModeButton__27091 = null;
                    break;
                }
            case DatePickerEntryMode.input:
                {
                    picker__27065 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(inputDatePicker());
                    entryModeButton__27091 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new IconButton(icon: (((DatePickerDialog)this.widget).switchToCalendarEntryModeIcon ?? new global::Doroti.Generated.Framework.Widgets.Icon(Icons.calendar_today)), color: headerForegroundColor__23396, tooltip: ((MaterialLocalizations)localizations__22125).calendarModeButtonLabel, onPressed: this._handleEntryModeToggle));
                    break;
                }
            case DatePickerEntryMode.inputOnly:
                {
                    picker__27065 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(inputDatePicker());
                    entryModeButton__27091 = null;
                    break;
                }
        }
        global::Doroti.Generated.Framework.Widgets.Widget header__28158 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _DatePickerHeader__date_picker(helpText: (((DatePickerDialog)this.widget).helpText ?? ((useMaterial3__22058 ? ((MaterialLocalizations)localizations__22125).datePickerHelpText : ((MaterialLocalizations)localizations__22125).datePickerHelpText.toUpperCase()))), titleText: ((this._selectedDate.value is null) ? "" : ((DatePickerDialog)this.widget).calendarDelegate.formatMediumDate(DartRuntimePrimitives.RequireValue(this._selectedDate.value), localizations__22125)), titleStyle: headlineStyle__22767, orientation: orientation__22198, isShort: (object.Equals(orientation__22198, global::Doroti.Generated.Framework.Widgets.Orientation.landscape)), entryModeButton: entryModeButton__27091));
        double textScaleFactor__28804 = (MediaQuery.textScalerOf(context).clamp(maxScaleFactor: Calendar_date_pickerLibrary._kMaxTextScaleFactor).scale(Calendar_date_pickerLibrary._fontSizeToScale) / Calendar_date_pickerLibrary._fontSizeToScale);
        global::Doroti.Ui.Size dialogSize__28995 = ((global::Doroti.Ui.Size)(object?)(_dialogSize(context) * textScaleFactor__28804));
        DialogThemeData dialogTheme__29074 = theme__22016.dialogTheme;
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new Dialog(backgroundColor: (datePickerTheme__22350.backgroundColor ?? defaults__22427.backgroundColor), elevation: (useMaterial3__22058 ? (datePickerTheme__22350.elevation ?? DartRuntimePrimitives.RequireValue(defaults__22427.elevation)) : ((datePickerTheme__22350.elevation ?? dialogTheme__29074.elevation) ?? 24)), shadowColor: (datePickerTheme__22350.shadowColor ?? defaults__22427.shadowColor), surfaceTintColor: (datePickerTheme__22350.surfaceTintColor ?? defaults__22427.surfaceTintColor), shape: (useMaterial3__22058 ? (datePickerTheme__22350.shape ?? defaults__22427.shape) : ((datePickerTheme__22350.shape ?? dialogTheme__29074.shape) ?? defaults__22427.shape)), insetPadding: ((DatePickerDialog)this.widget).insetPadding, clipBehavior: Clip.antiAlias, child: new global::Doroti.Generated.Framework.Widgets.AnimatedContainer(width: dialogSize__28995.width, height: dialogSize__28995.height, duration: Date_pickerLibrary._dialogSizeAnimationDuration, curve: global::Doroti.Generated.Framework.Animation.Curves.easeIn, child: MediaQuery.withClampedTextScaling(maxScaleFactor: Calendar_date_pickerLibrary._kMaxTextScaleFactor, child: new global::Doroti.Generated.Framework.Widgets.LayoutBuilder(builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Rendering.BoxConstraints, global::Doroti.Generated.Framework.Widgets.Widget>)((context, constraints) => {
global::Doroti.Ui.Size portraitDialogSize__30279 = ((global::Doroti.Ui.Size)(object?)(useMaterial3__22058 ? Date_pickerLibrary._inputPortraitDialogSizeM3 : Date_pickerLibrary._inputPortraitDialogSizeM2));
bool isFullyPortrait__30570 = (((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxHeight >= Math.Min(dialogSize__28995.height, portraitDialogSize__30279.height));
switch (orientation__22198)
{
    case global::Doroti.Generated.Framework.Widgets.Orientation.portrait:
        {
            bool isInputMode__30797 = ((object.Equals(this._entryMode.value, DatePickerEntryMode.inputOnly)) || (object.Equals(this._entryMode.value, DatePickerEntryMode.input)));
            bool showHeader__31175 = (isFullyPortrait__30570 || !isInputMode__30797);
            bool showPicker__31250 = (isFullyPortrait__30570 || isInputMode__30797);
            return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Column(mainAxisSize: global::Doroti.Generated.Framework.Rendering.MainAxisSize.min, crossAxisAlignment: global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.stretch, children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection31479 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); if (showHeader__31175) { __collection31479.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(header__28158)); } if (useMaterial3__22058) { __collection31479.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new Divider(height: 0, color: datePickerTheme__22350.dividerColor))); } if (showPicker__31250) { __collection31479.AddRange(new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Expanded(child: picker__27065)), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(actions__23594) }); } return __collection31479; }))()));
        }
    case global::Doroti.Generated.Framework.Widgets.Orientation.landscape:
        {
            return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Row(mainAxisSize: global::Doroti.Generated.Framework.Rendering.MainAxisSize.min, crossAxisAlignment: global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.stretch, children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection31985 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); __collection31985.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(header__28158)); if (useMaterial3__22058) { __collection31985.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new VerticalDivider(width: 0, color: datePickerTheme__22350.dividerColor))); } __collection31985.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Flexible(child: new global::Doroti.Generated.Framework.Widgets.Column(mainAxisSize: global::Doroti.Generated.Framework.Rendering.MainAxisSize.min, crossAxisAlignment: global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.stretch, children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Expanded(child: picker__27065)), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(actions__23594) })))); return __collection31985; }))()));
        }
    default:
        throw new InvalidOperationException("Non-exhaustive Dart switch value.");
}
throw new InvalidOperationException("Dart closure completed without a value.");
})))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Services.RestorationBucket? bucket => this._bucket;
    public virtual void didToggleBucket(global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => (this._bucket?.isReplacing != true));
    }

    public virtual void registerForRestoration(dynamic property, string restorationId)
    {
        DartRuntimePrimitives.Assert(() => ((((dynamic)property)._restorationId is null) || ((this._debugDoingRestore && (((dynamic)property)._restorationId == restorationId)))), () => (object?)$"Property is already registered under {((dynamic)property)._restorationId}.");
        DartRuntimePrimitives.Assert(() => (this._debugDoingRestore || !this._properties.Keys.map<dynamic, string?>(((r) => ((dynamic)r)._restorationId)).contains(restorationId)), () => (object?)$"\"{restorationId}\" is already registered to another property.");
        bool hasSerializedValue__36723 = (this.bucket?.contains(restorationId) ?? false);
        object? initialValue__36804 = (hasSerializedValue__36723 ? property.fromPrimitives(this.bucket!.read<object>(restorationId)) : property.createDefaultValue());
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
        property.initWithValue((dynamic)initialValue__36804);
        if (((!hasSerializedValue__36723 && ((dynamic)property).enabled) && (this.bucket is not null)))
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
        global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket__39230 = this._bucket;
        DartRuntimePrimitives.Assert(() => !this.restorePending);
        bool didReplaceBucket__39295 = _updateBucketIfNecessary(parent: this._currentParent, restorePending: false);
        if (didReplaceBucket__39295)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket__39230, this._bucket)));
            DartRuntimePrimitives.Assert(() => ((this._bucket is null) || (oldBucket__39230 is null)));
            oldBucket__39230?.dispose();
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
            global::Doroti.Generated.Framework.Services.RestorationBucket? potentialNewParent__40517 = ((global::Doroti.Generated.Framework.Services.RestorationBucket?)(object?)RestorationScope.maybeOf(this.context));
            return ((!object.Equals(potentialNewParent__40517, this._currentParent)) && ((potentialNewParent__40517?.isReplacing ?? false)));
            return default!;
        }
    }
    public virtual bool _debugDoingRestore => DartRuntimePrimitives.ConvertValue<bool>((this._debugPropertiesWaitingForReregistration is not null));
    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket__41020 = this._bucket;
        bool needsRestore__41056 = this.restorePending;
        this._currentParent = RestorationScope.maybeOf(this.context);
        bool didReplaceBucket__41159 = _updateBucketIfNecessary(parent: this._currentParent, restorePending: needsRestore__41056);
        if (needsRestore__41056)
        {
            _doRestore(oldBucket__41020);
        }
        if (didReplaceBucket__41159)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket__41020, this._bucket)));
            oldBucket__41020?.dispose();
        }
    }

    public virtual void _doRestore(global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket)
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
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(((Func<List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>>)(() => { var __collection41817 = new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>(); __collection41817.Add(new global::Doroti.Generated.Framework.Foundation.ErrorSummary("Previously registered RestorableProperties must be re-registered in \"restoreState\".")); __collection41817.Add(new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"The RestorableProperties with the following IDs were not re-registered to {this} when " + "\"restoreState\" was called:")); __collection41817.AddRange(this._debugPropertiesWaitingForReregistration!.map<dynamic, global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>(((property) => new global::Doroti.Generated.Framework.Foundation.ErrorDescription($" * {((dynamic)property)._restorationId}")))); return __collection41817; }))()));
                }
                this._debugPropertiesWaitingForReregistration = null;
                return true;
            });
    }

    public virtual bool _updateBucketIfNecessary(global::Doroti.Generated.Framework.Services.RestorationBucket? parent, bool restorePending)
    {
        if (((this.restorationId is null) || (parent is null)))
        {
            bool didReplace__42801 = _setNewBucketIfNecessary(newBucket: null, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (this._bucket is null));
            return didReplace__42801;
        }
        DartRuntimePrimitives.Assert(() => (this.restorationId is not null));
        if ((restorePending || (this._bucket is null)))
        {
            global::Doroti.Generated.Framework.Services.RestorationBucket newBucket__43086 = ((global::Doroti.Generated.Framework.Services.RestorationBucket)(object?)parent.claimChild(this.restorationId!, debugOwner: this));
            bool didReplace__43168 = _setNewBucketIfNecessary(newBucket: newBucket__43086, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (object.Equals(this._bucket, newBucket__43086)));
            return didReplace__43168;
        }
        DartRuntimePrimitives.Assert(() => (this._bucket is not null));
        DartRuntimePrimitives.Assert(() => !restorePending);
        this._bucket!.rename(this.restorationId!);
        parent.adoptChild(this._bucket!);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _setNewBucketIfNecessary(global::Doroti.Generated.Framework.Services.RestorationBucket? newBucket, bool restorePending)
    {
        if ((object.Equals(newBucket, this._bucket)))
        {
            return false;
        }
        global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket__43946 = this._bucket;
        this._bucket = newBucket;
        if (!restorePending)
        {
            if ((this._bucket is not null))
            {
                this._properties.Keys.forEach((__arg0) => ((global::System.Action<dynamic>)this._updateProperty)(__arg0));
            }
            didToggleBucket(oldBucket__43946);
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
        global::System.Action listener__44576 = this._properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
        property.removeListener((global::System.Action)(() => listener__44576()));
        property._unregister();
    }

}

internal class _RestorableDatePickerEntryMode__date_picker : global::Doroti.Generated.Framework.Widgets.RestorableValue<DatePickerEntryMode>
{
    internal virtual DatePickerEntryMode _defaultValue { get; private set; } = default!;

    internal _RestorableDatePickerEntryMode__date_picker(DatePickerEntryMode defaultValue)
    {
        this._defaultValue = defaultValue;
    }

    public override DatePickerEntryMode createDefaultValue() => this._defaultValue;
    public override void didUpdateValue(DatePickerEntryMode oldValue)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Services.RestorationLibrary.debugIsSerializableForRestoration(FoundationRuntimePorts.EnumIndex(this.value)));
        notifyListeners();
    }

    public override DatePickerEntryMode fromPrimitives(object? data) => System.Enum.GetValues<DatePickerEntryMode>().ToList()[(int)(((long)data!))];
    public override object? toPrimitives() => FoundationRuntimePorts.EnumIndex(this.value);
}

internal class _RestorableAutovalidateMode__date_picker : global::Doroti.Generated.Framework.Widgets.RestorableValue<global::Doroti.Generated.Framework.Widgets.AutovalidateMode>
{
    internal virtual global::Doroti.Generated.Framework.Widgets.AutovalidateMode _defaultValue { get; private set; } = default!;

    internal _RestorableAutovalidateMode__date_picker(global::Doroti.Generated.Framework.Widgets.AutovalidateMode defaultValue)
    {
        this._defaultValue = defaultValue;
    }

    public override global::Doroti.Generated.Framework.Widgets.AutovalidateMode createDefaultValue() => this._defaultValue;
    public override void didUpdateValue(global::Doroti.Generated.Framework.Widgets.AutovalidateMode oldValue)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Services.RestorationLibrary.debugIsSerializableForRestoration(FoundationRuntimePorts.EnumIndex(this.value)));
        notifyListeners();
    }

    public override global::Doroti.Generated.Framework.Widgets.AutovalidateMode fromPrimitives(object? data) => System.Enum.GetValues<global::Doroti.Generated.Framework.Widgets.AutovalidateMode>().ToList()[(int)(((long)data!))];
    public override object? toPrimitives() => FoundationRuntimePorts.EnumIndex(this.value);
}

internal class _DatePickerHeader__date_picker : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    internal const double _datePickerHeaderLandscapeWidth = 152.0;
    internal const double _datePickerHeaderPortraitHeight = 120.0;
    internal const double _headerPaddingLandscape = 16.0;
    public virtual string helpText { get; private set; } = default!;
    public virtual string titleText { get; private set; } = default!;
    public virtual string? titleSemanticsLabel { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? titleStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Orientation orientation { get; private set; } = default!;
    public virtual bool isShort { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? entryModeButton { get; private set; }

    internal _DatePickerHeader__date_picker(string helpText, string titleText, string? titleSemanticsLabel = null, global::Doroti.Generated.Framework.Painting.TextStyle? titleStyle = default!, global::Doroti.Generated.Framework.Widgets.Orientation orientation = default!, bool isShort = false, global::Doroti.Generated.Framework.Widgets.Widget? entryModeButton = null)
    {
        this.helpText = helpText;
        this.titleText = titleText;
        this.titleSemanticsLabel = titleSemanticsLabel;
        this.titleStyle = titleStyle;
        this.orientation = orientation;
        this.isShort = isShort;
        this.entryModeButton = entryModeButton;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ThemeData theme__35985 = Theme.of(context);
        DatePickerThemeData datePickerTheme__36042 = DatePickerTheme.of(context);
        DatePickerThemeData defaults__36119 = DatePickerTheme.defaults(context);
        global::Doroti.Ui.Color? backgroundColor__36182 = ((global::Doroti.Ui.Color?)(object?)(datePickerTheme__36042.headerBackgroundColor ?? defaults__36119.headerBackgroundColor));
        global::Doroti.Ui.Color? foregroundColor__36298 = ((global::Doroti.Ui.Color?)(object?)(datePickerTheme__36042.headerForegroundColor ?? defaults__36119.headerForegroundColor));
        global::Doroti.Generated.Framework.Painting.TextStyle? helpStyle__36418 = ((global::Doroti.Generated.Framework.Painting.TextStyle?)(object?)((datePickerTheme__36042.headerHelpStyle ?? defaults__36119.headerHelpStyle))?.copyWith(color: foregroundColor__36298));
        double currentScale__36553 = (MediaQuery.textScalerOf(context).scale(Calendar_date_pickerLibrary._fontSizeToScale) / Calendar_date_pickerLibrary._fontSizeToScale);
        double maxHeaderTextScaleFactor__36670 = Math.Min(currentScale__36553, ((this.entryModeButton is not null) ? Date_pickerLibrary._kMaxHeaderWithEntryTextScaleFactor : Date_pickerLibrary._kMaxHeaderTextScaleFactor));
        double textScaleFactor__36849 = (MediaQuery.textScalerOf(context).clamp(maxScaleFactor: maxHeaderTextScaleFactor__36670).scale(Calendar_date_pickerLibrary._fontSizeToScale) / Calendar_date_pickerLibrary._fontSizeToScale);
        double scaledFontSize__37046 = MediaQuery.textScalerOf(context).scale((this.titleStyle?.fontSize ?? 32));
        var headerScaleFactor__37154 = ((textScaleFactor__36849 > 1L) ? textScaleFactor__36849 : 1.0);
        var help__37230 = new global::Doroti.Generated.Framework.Widgets.Text(this.helpText, style: helpStyle__36418, maxLines: 1L, overflow: global::Doroti.Generated.Framework.Painting.TextOverflow.ellipsis, textScaler: MediaQuery.textScalerOf(context).clamp(maxScaleFactor: Math.Min(textScaleFactor__36849, ((object.Equals(this.orientation, global::Doroti.Generated.Framework.Widgets.Orientation.portrait)) ? Date_pickerLibrary._kMaxHelpPortraitTextScaleFactor : Date_pickerLibrary._kMaxHelpLandscapeTextScaleFactor))));
        var title__37643 = new global::Doroti.Generated.Framework.Widgets.Text(this.titleText, semanticsLabel: (this.titleSemanticsLabel ?? this.titleText), style: this.titleStyle, maxLines: ((object.Equals(this.orientation, global::Doroti.Generated.Framework.Widgets.Orientation.portrait)) ? (((scaledFontSize__37046 > 70L) ? 2L : 1L)) : ((scaledFontSize__37046 > 40L) ? 3L : 2L)), overflow: global::Doroti.Generated.Framework.Painting.TextOverflow.ellipsis, textScaler: MediaQuery.textScalerOf(context).clamp(maxScaleFactor: textScaleFactor__36849));
        double fontScaleAdjustedHeaderHeight__38065 = ((headerScaleFactor__37154 > 1.3) ? (headerScaleFactor__37154 - 0.2) : 1.0);
        switch (this.orientation)
        {
            case global::Doroti.Generated.Framework.Widgets.Orientation.portrait:
                {
                    return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Semantics(container: true, child: new global::Doroti.Generated.Framework.Widgets.SizedBox(height: (_datePickerHeaderPortraitHeight * fontScaleAdjustedHeaderHeight__38065), child: new Material(color: backgroundColor__36182, child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: 24, end: 12, bottom: 12), child: new global::Doroti.Generated.Framework.Widgets.Column(crossAxisAlignment: global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.start, children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(height: 16)), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(help__37230), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Flexible(child: new global::Doroti.Generated.Framework.Widgets.SizedBox(height: 38))), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Row(children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection38913 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); __collection38913.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Expanded(child: title__37643))); if ((this.entryModeButton is not null)) { __collection38913.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Semantics(container: true, child: this.entryModeButton))); } return __collection38913; }))())) }))))));
                }
            case global::Doroti.Generated.Framework.Widgets.Orientation.landscape:
                {
                    return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Semantics(container: true, child: new global::Doroti.Generated.Framework.Widgets.SizedBox(width: _datePickerHeaderLandscapeWidth, child: new Material(color: backgroundColor__36182, child: new global::Doroti.Generated.Framework.Widgets.Column(crossAxisAlignment: global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.start, children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection39596 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); __collection39596.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(height: 16))); __collection39596.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: _headerPaddingLandscape), child: help__37230))); __collection39596.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(height: (this.isShort ? 16 : 56)))); __collection39596.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Expanded(child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: _headerPaddingLandscape), child: title__37643)))); if ((this.entryModeButton is not null)) { __collection39596.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Padding(padding: (theme__35985.useMaterial3 ? global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: 8.0, end: 4.0, bottom: 6.0) : global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 4)), child: new global::Doroti.Generated.Framework.Widgets.Semantics(container: true, child: this.entryModeButton)))); } return __collection39596; }))())))));
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
    public static async Future<DateTimeRange<DateTime>?> showDateRangePicker(global::Doroti.Generated.Framework.Widgets.BuildContext context, DateTimeRange<DateTime>? initialDateRange = null, DateTime firstDate = default!, DateTime lastDate = default!, DateTime? currentDate = null, DatePickerEntryMode initialEntryMode = DatePickerEntryMode.calendar, string? helpText = null, string? cancelText = null, string? confirmText = null, string? saveText = null, string? errorFormatText = null, string? errorInvalidText = null, string? errorInvalidRangeText = null, string? fieldStartHintText = null, string? fieldEndHintText = null, string? fieldStartLabelText = null, string? fieldEndLabelText = null, Locale? locale = null, bool barrierDismissible = true, Color? barrierColor = null, string? barrierLabel = null, bool useRootNavigator = true, global::Doroti.Generated.Framework.Widgets.RouteSettings? routeSettings = null, TextDirection? textDirection = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>? builder = null, Offset? anchorPoint = null, global::Doroti.Generated.Framework.Services.TextInputType keyboardType = default!, global::Doroti.Generated.Framework.Widgets.Icon? switchToInputEntryModeIcon = null, global::Doroti.Generated.Framework.Widgets.Icon? switchToCalendarEntryModeIcon = null, global::System.Func<DateTime, DateTime?, DateTime?, bool>? selectableDayPredicate = null, CalendarDelegate<DateTime> calendarDelegate = default!)
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
        global::Doroti.Generated.Framework.Widgets.Widget dialog__49082 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new DateRangePickerDialog(initialDateRange: initialDateRange, firstDate: firstDate, lastDate: lastDate, currentDate: DartRuntimePrimitives.RequireValue(currentDate), selectableDayPredicate: (global::System.Func<DateTime, DateTime?, DateTime?, bool>?)selectableDayPredicate, initialEntryMode: initialEntryMode, helpText: helpText, cancelText: cancelText, confirmText: confirmText, saveText: saveText, errorFormatText: errorFormatText, errorInvalidText: errorInvalidText, errorInvalidRangeText: errorInvalidRangeText, fieldStartHintText: fieldStartHintText, fieldEndHintText: fieldEndHintText, fieldStartLabelText: fieldStartLabelText, fieldEndLabelText: fieldEndLabelText, keyboardType: keyboardType, switchToInputEntryModeIcon: switchToInputEntryModeIcon, switchToCalendarEntryModeIcon: switchToCalendarEntryModeIcon, calendarDelegate: calendarDelegate));
        if ((textDirection is not null))
        {
            TextDirection textDirection__value49942 = DartRuntimePrimitives.RequireValue(textDirection);
            dialog__49082 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Directionality(textDirection: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(textDirection__value49942)), child: dialog__49082));
        }
        if ((locale is not null))
        {
            Locale locale__value50052 = DartRuntimePrimitives.RequireValue(locale);
            dialog__49082 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(global::Doroti.Generated.Framework.Widgets.Localizations.CreateOverride(context: context, locale: DartRuntimePrimitives.RequireValue(locale__value50052), child: dialog__49082));
        }
        return await DialogLibrary.showDialog<DateTimeRange<DateTime>>(context: context, barrierDismissible: barrierDismissible, barrierColor: barrierColor, barrierLabel: barrierLabel, useRootNavigator: useRootNavigator, routeSettings: routeSettings, useSafeArea: false, builder: ((context) => {
return ((builder is null) ? dialog__49082 : builder(context, dialog__49082));
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

public class DateRangePickerDialog : global::Doroti.Generated.Framework.Widgets.StatefulWidget
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
    public virtual global::Doroti.Generated.Framework.Services.TextInputType keyboardType { get; private set; } = default!;
    public virtual string? restorationId { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Icon? switchToInputEntryModeIcon { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Icon? switchToCalendarEntryModeIcon { get; private set; }
    public virtual global::System.Func<DateTime, DateTime?, DateTime?, bool>? selectableDayPredicate { get; private set; }
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    public DateRangePickerDialog(global::Doroti.Generated.Framework.Foundation.Key? key = null, DateTimeRange<DateTime>? initialDateRange = null, DateTime firstDate = default!, DateTime lastDate = default!, DateTime? currentDate = null, DatePickerEntryMode initialEntryMode = DatePickerEntryMode.calendar, string? helpText = null, string? cancelText = null, string? confirmText = null, string? saveText = null, string? errorInvalidRangeText = null, string? errorFormatText = null, string? errorInvalidText = null, string? fieldStartHintText = null, string? fieldEndHintText = null, string? fieldStartLabelText = null, string? fieldEndLabelText = null, global::Doroti.Generated.Framework.Services.TextInputType keyboardType = default!, string? restorationId = null, global::Doroti.Generated.Framework.Widgets.Icon? switchToInputEntryModeIcon = null, global::Doroti.Generated.Framework.Widgets.Icon? switchToCalendarEntryModeIcon = null, global::System.Func<DateTime, DateTime?, DateTime?, bool>? selectableDayPredicate = null, CalendarDelegate<DateTime> calendarDelegate = default!) : base(key: key)
    {
        global::Doroti.Generated.Framework.Services.TextInputType __keyboardType = keyboardType ?? global::Doroti.Generated.Framework.Services.TextInputType.datetime;
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

internal class _DateRangePickerDialogState__date_picker : global::Doroti.Generated.Framework.Widgets.State<DateRangePickerDialog>, global::Doroti.Generated.Framework.Widgets.RestorationMixin<DateRangePickerDialog>
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
    private global::Doroti.Generated.Framework.Widgets.RestorableDateTimeN __late__selectedStart = default!;
    internal virtual global::Doroti.Generated.Framework.Widgets.RestorableDateTimeN _selectedStart
    {
        get
        {
            if (!__late__selectedStart_initialized)
            {
                __late__selectedStart = new global::Doroti.Generated.Framework.Widgets.RestorableDateTimeN(((DateRangePickerDialog)this.widget).initialDateRange?.start);
                __late__selectedStart_initialized = true;
            }
            return __late__selectedStart;
        }
    }
    private bool __late__selectedEnd_initialized;
    private global::Doroti.Generated.Framework.Widgets.RestorableDateTimeN __late__selectedEnd = default!;
    internal virtual global::Doroti.Generated.Framework.Widgets.RestorableDateTimeN _selectedEnd
    {
        get
        {
            if (!__late__selectedEnd_initialized)
            {
                __late__selectedEnd = new global::Doroti.Generated.Framework.Widgets.RestorableDateTimeN(((DateRangePickerDialog)this.widget).initialDateRange?.end);
                __late__selectedEnd_initialized = true;
            }
            return __late__selectedEnd;
        }
    }
    internal virtual global::Doroti.Generated.Framework.Widgets.RestorableBool _autoValidate { get; private set; } = new global::Doroti.Generated.Framework.Widgets.RestorableBool(false);
    internal virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> _calendarPickerKey { get; private set; } = global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create();
    internal virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<_InputDateRangePickerState__date_picker> _inputPickerKey { get; private set; } = global::Doroti.Generated.Framework.Widgets.GlobalKey<_InputDateRangePickerState__date_picker>.Create();
    public virtual global::Doroti.Generated.Framework.Services.RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<dynamic, global::System.Action> _properties { get; set; } = new DartMap<dynamic, global::System.Action>();
    public virtual List<dynamic>? _debugPropertiesWaitingForReregistration { get; set; } = default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual global::Doroti.Generated.Framework.Services.RestorationBucket? _currentParent { get; set; } = default;

    public virtual string? restorationId => ((DateRangePickerDialog)this.widget).restorationId;
    public virtual void restoreState(global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket, bool initialRestore)
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
        this._properties.forEach(((global::System.Action<dynamic, global::System.Action>)((property, listener) => {
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
            _InputDateRangePickerState__date_picker picker__59718 = ((global::Doroti.Generated.Framework.Widgets.GlobalKey<_InputDateRangePickerState__date_picker>)this._inputPickerKey).currentState!;
            if (!picker__59718.validate())
            {
                setState(((global::System.Action)(() => {
this._autoValidate.value = true;
})));
                return;
            }
        }
        DateTimeRange<DateTime>? selectedRange__59917 = (this._hasSelectedDateRange ? new DateTimeRange<DateTime>(start: DartRuntimePrimitives.RequireValue(this._selectedStart.value), end: DartRuntimePrimitives.RequireValue(this._selectedEnd.value)) : null);
        Navigator.pop<object>(this.context, selectedRange__59917);
    }

    internal virtual void _handleCancel()
    {
        Navigator.pop<object>(this.context);
    }

    internal virtual void _handleEntryModeToggle()
    {
        setState(((global::System.Action)(() => {
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
    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ThemeData theme__61993 = Theme.of(context);
        bool useMaterial3__62035 = theme__61993.useMaterial3;
        global::Doroti.Generated.Framework.Widgets.Orientation orientation__62092 = MediaQuery.orientationOf(context);
        MaterialLocalizations localizations__62173 = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        DatePickerThemeData datePickerTheme__62254 = DatePickerTheme.of(context);
        DatePickerThemeData defaults__62331 = DatePickerTheme.defaults(context);
        global::Doroti.Generated.Framework.Widgets.Widget contents__62395 = default!;
        global::Doroti.Ui.Size size__62420 = default!;
        double? elevation__62444 = default!;
        global::Doroti.Ui.Color? shadowColor__62472 = default!;
        global::Doroti.Ui.Color? surfaceTintColor__62502 = default!;
        global::Doroti.Generated.Framework.Painting.ShapeBorder? shape__62543 = default!;
        global::Doroti.Generated.Framework.Painting.EdgeInsets insetPadding__62571 = default!;
        bool showEntryModeButton__62600 = ((object.Equals(this._entryMode.value, DatePickerEntryMode.calendar)) || (object.Equals(this._entryMode.value, DatePickerEntryMode.input)));
        switch (this._entryMode.value)
        {
            case DatePickerEntryMode.calendar:
            case DatePickerEntryMode.calendarOnly:
                {
                    contents__62395 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new _CalendarRangePickerDialog__date_picker(key: this._calendarPickerKey, calendarDelegate: ((DateRangePickerDialog)this.widget).calendarDelegate, selectedStartDate: this._selectedStart.value, selectedEndDate: this._selectedEnd.value, firstDate: ((DateRangePickerDialog)this.widget).firstDate, lastDate: ((DateRangePickerDialog)this.widget).lastDate, selectableDayPredicate: (global::System.Func<DateTime, DateTime?, DateTime?, bool>?)((DateRangePickerDialog)this.widget).selectableDayPredicate, currentDate: ((DateRangePickerDialog)this.widget).currentDate, onStartDateChanged: (__arg0) => ((global::System.Action<DateTime?>)this._handleStartDateChanged)(DartRuntimePrimitives.ConvertValue<DateTime>(__arg0)), onEndDateChanged: (global::System.Action<DateTime?>)this._handleEndDateChanged, onConfirm: ((global::System.Action)(this._hasSelectedDateRange ? this._handleOk : null)), onCancel: () => this._handleCancel(), entryModeButton: (showEntryModeButton__62600 ? new IconButton(icon: (((DateRangePickerDialog)this.widget).switchToInputEntryModeIcon ?? new global::Doroti.Generated.Framework.Widgets.Icon((useMaterial3__62035 ? Icons.edit_outlined : Icons.edit))), padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.zero, tooltip: ((MaterialLocalizations)localizations__62173).inputDateModeButtonLabel, onPressed: this._handleEntryModeToggle) : null), confirmText: (((DateRangePickerDialog)this.widget).saveText ?? ((useMaterial3__62035 ? ((MaterialLocalizations)localizations__62173).saveButtonLabel : ((MaterialLocalizations)localizations__62173).saveButtonLabel.toUpperCase()))), helpText: (((DateRangePickerDialog)this.widget).helpText ?? ((useMaterial3__62035 ? ((MaterialLocalizations)localizations__62173).dateRangePickerHelpText : ((MaterialLocalizations)localizations__62173).dateRangePickerHelpText.toUpperCase())))));
                    size__62420 = MediaQuery.sizeOf(context);
                    insetPadding__62571 = global::Doroti.Generated.Framework.Painting.EdgeInsets.zero;
                    elevation__62444 = (datePickerTheme__62254.rangePickerElevation ?? DartRuntimePrimitives.RequireValue(defaults__62331.rangePickerElevation));
                    shadowColor__62472 = (datePickerTheme__62254.rangePickerShadowColor ?? defaults__62331.rangePickerShadowColor!);
                    surfaceTintColor__62502 = (datePickerTheme__62254.rangePickerSurfaceTintColor ?? defaults__62331.rangePickerSurfaceTintColor!);
                    shape__62543 = (datePickerTheme__62254.rangePickerShape ?? defaults__62331.rangePickerShape);
                    break;
                }
            case DatePickerEntryMode.input:
            case DatePickerEntryMode.inputOnly:
                {
                    contents__62395 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new _InputDateRangePickerDialog__date_picker(calendarDelegate: ((DateRangePickerDialog)this.widget).calendarDelegate, selectedStartDate: this._selectedStart.value, selectedEndDate: this._selectedEnd.value, currentDate: ((DateRangePickerDialog)this.widget).currentDate, picker: new global::Doroti.Generated.Framework.Widgets.SizedBox(height: ((object.Equals(orientation__62092, global::Doroti.Generated.Framework.Widgets.Orientation.portrait)) ? Date_pickerLibrary._inputFormPortraitHeight : Date_pickerLibrary._inputFormLandscapeHeight), child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 24), child: new global::Doroti.Generated.Framework.Widgets.Column(children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Spacer()), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new _InputDateRangePicker__date_picker(key: this._inputPickerKey, calendarDelegate: ((DateRangePickerDialog)this.widget).calendarDelegate, initialStartDate: this._selectedStart.value, initialEndDate: this._selectedEnd.value, firstDate: ((DateRangePickerDialog)this.widget).firstDate, lastDate: ((DateRangePickerDialog)this.widget).lastDate, selectableDayPredicate: (global::System.Func<DateTime, DateTime?, DateTime?, bool>?)((DateRangePickerDialog)this.widget).selectableDayPredicate, onStartDateChanged: (global::System.Action<DateTime?>)this._handleStartDateChanged, onEndDateChanged: (global::System.Action<DateTime?>)this._handleEndDateChanged, autofocus: true, autovalidate: DartRuntimePrimitives.RequireValue(this._autoValidate.value), helpText: ((DateRangePickerDialog)this.widget).helpText, errorInvalidRangeText: ((DateRangePickerDialog)this.widget).errorInvalidRangeText, errorFormatText: ((DateRangePickerDialog)this.widget).errorFormatText, errorInvalidText: ((DateRangePickerDialog)this.widget).errorInvalidText, fieldStartHintText: ((DateRangePickerDialog)this.widget).fieldStartHintText, fieldEndHintText: ((DateRangePickerDialog)this.widget).fieldEndHintText, fieldStartLabelText: ((DateRangePickerDialog)this.widget).fieldStartLabelText, fieldEndLabelText: ((DateRangePickerDialog)this.widget).fieldEndLabelText, keyboardType: ((DateRangePickerDialog)this.widget).keyboardType)), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Spacer()) }))), onConfirm: () => this._handleOk(), onCancel: () => this._handleCancel(), entryModeButton: (showEntryModeButton__62600 ? new IconButton(icon: (((DateRangePickerDialog)this.widget).switchToCalendarEntryModeIcon ?? new global::Doroti.Generated.Framework.Widgets.Icon(Icons.calendar_today)), padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.zero, tooltip: ((MaterialLocalizations)localizations__62173).calendarModeButtonLabel, onPressed: this._handleEntryModeToggle) : null), confirmText: ((((DateRangePickerDialog)this.widget).confirmText ?? (string)((MaterialLocalizations)localizations__62173).okButtonLabel)), cancelText: (((DateRangePickerDialog)this.widget).cancelText ?? ((useMaterial3__62035 ? ((MaterialLocalizations)localizations__62173).cancelButtonLabel : ((MaterialLocalizations)localizations__62173).cancelButtonLabel.toUpperCase()))), helpText: (((DateRangePickerDialog)this.widget).helpText ?? ((useMaterial3__62035 ? ((MaterialLocalizations)localizations__62173).dateRangePickerHelpText : ((MaterialLocalizations)localizations__62173).dateRangePickerHelpText.toUpperCase())))));
                    DialogThemeData dialogTheme__67814 = theme__61993.dialogTheme;
                    size__62420 = ((object.Equals(orientation__62092, global::Doroti.Generated.Framework.Widgets.Orientation.portrait)) ? ((useMaterial3__62035 ? Date_pickerLibrary._inputPortraitDialogSizeM3 : Date_pickerLibrary._inputPortraitDialogSizeM2)) : Date_pickerLibrary._inputRangeLandscapeDialogSize);
                    elevation__62444 = (useMaterial3__62035 ? (datePickerTheme__62254.elevation ?? DartRuntimePrimitives.RequireValue(defaults__62331.elevation)) : ((datePickerTheme__62254.elevation ?? dialogTheme__67814.elevation) ?? 24));
                    shadowColor__62472 = (datePickerTheme__62254.shadowColor ?? defaults__62331.shadowColor);
                    surfaceTintColor__62502 = (datePickerTheme__62254.surfaceTintColor ?? defaults__62331.surfaceTintColor);
                    shape__62543 = (useMaterial3__62035 ? (datePickerTheme__62254.shape ?? defaults__62331.shape) : ((datePickerTheme__62254.shape ?? dialogTheme__67814.shape) ?? defaults__62331.shape));
                    insetPadding__62571 = global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 16.0, vertical: 24.0);
                    break;
                }
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new Dialog(insetPadding: insetPadding__62571, backgroundColor: (datePickerTheme__62254.backgroundColor ?? defaults__62331.backgroundColor), elevation: elevation__62444, shadowColor: shadowColor__62472, surfaceTintColor: surfaceTintColor__62502, shape: shape__62543, clipBehavior: Clip.antiAlias, child: new global::Doroti.Generated.Framework.Widgets.AnimatedContainer(width: size__62420.width, height: size__62420.height, duration: Date_pickerLibrary._dialogSizeAnimationDuration, curve: global::Doroti.Generated.Framework.Animation.Curves.easeIn, child: MediaQuery.withClampedTextScaling(maxScaleFactor: Date_pickerLibrary._kMaxRangeTextScaleFactor, child: new global::Doroti.Generated.Framework.Widgets.Builder(builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>)((context) => {
return contents__62395;
throw new InvalidOperationException("Dart closure completed without a value.");
})))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Services.RestorationBucket? bucket => this._bucket;
    public virtual void didToggleBucket(global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => (this._bucket?.isReplacing != true));
    }

    public virtual void registerForRestoration(dynamic property, string restorationId)
    {
        DartRuntimePrimitives.Assert(() => ((((dynamic)property)._restorationId is null) || ((this._debugDoingRestore && (((dynamic)property)._restorationId == restorationId)))), () => (object?)$"Property is already registered under {((dynamic)property)._restorationId}.");
        DartRuntimePrimitives.Assert(() => (this._debugDoingRestore || !this._properties.Keys.map<dynamic, string?>(((r) => ((dynamic)r)._restorationId)).contains(restorationId)), () => (object?)$"\"{restorationId}\" is already registered to another property.");
        bool hasSerializedValue__36723 = (this.bucket?.contains(restorationId) ?? false);
        object? initialValue__36804 = (hasSerializedValue__36723 ? property.fromPrimitives(this.bucket!.read<object>(restorationId)) : property.createDefaultValue());
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
        property.initWithValue((dynamic)initialValue__36804);
        if (((!hasSerializedValue__36723 && ((dynamic)property).enabled) && (this.bucket is not null)))
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
        global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket__39230 = this._bucket;
        DartRuntimePrimitives.Assert(() => !this.restorePending);
        bool didReplaceBucket__39295 = _updateBucketIfNecessary(parent: this._currentParent, restorePending: false);
        if (didReplaceBucket__39295)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket__39230, this._bucket)));
            DartRuntimePrimitives.Assert(() => ((this._bucket is null) || (oldBucket__39230 is null)));
            oldBucket__39230?.dispose();
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
            global::Doroti.Generated.Framework.Services.RestorationBucket? potentialNewParent__40517 = ((global::Doroti.Generated.Framework.Services.RestorationBucket?)(object?)RestorationScope.maybeOf(this.context));
            return ((!object.Equals(potentialNewParent__40517, this._currentParent)) && ((potentialNewParent__40517?.isReplacing ?? false)));
            return default!;
        }
    }
    public virtual bool _debugDoingRestore => DartRuntimePrimitives.ConvertValue<bool>((this._debugPropertiesWaitingForReregistration is not null));
    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket__41020 = this._bucket;
        bool needsRestore__41056 = this.restorePending;
        this._currentParent = RestorationScope.maybeOf(this.context);
        bool didReplaceBucket__41159 = _updateBucketIfNecessary(parent: this._currentParent, restorePending: needsRestore__41056);
        if (needsRestore__41056)
        {
            _doRestore(oldBucket__41020);
        }
        if (didReplaceBucket__41159)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket__41020, this._bucket)));
            oldBucket__41020?.dispose();
        }
    }

    public virtual void _doRestore(global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket)
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
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(((Func<List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>>)(() => { var __collection41817 = new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>(); __collection41817.Add(new global::Doroti.Generated.Framework.Foundation.ErrorSummary("Previously registered RestorableProperties must be re-registered in \"restoreState\".")); __collection41817.Add(new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"The RestorableProperties with the following IDs were not re-registered to {this} when " + "\"restoreState\" was called:")); __collection41817.AddRange(this._debugPropertiesWaitingForReregistration!.map<dynamic, global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>(((property) => new global::Doroti.Generated.Framework.Foundation.ErrorDescription($" * {((dynamic)property)._restorationId}")))); return __collection41817; }))()));
                }
                this._debugPropertiesWaitingForReregistration = null;
                return true;
            });
    }

    public virtual bool _updateBucketIfNecessary(global::Doroti.Generated.Framework.Services.RestorationBucket? parent, bool restorePending)
    {
        if (((this.restorationId is null) || (parent is null)))
        {
            bool didReplace__42801 = _setNewBucketIfNecessary(newBucket: null, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (this._bucket is null));
            return didReplace__42801;
        }
        DartRuntimePrimitives.Assert(() => (this.restorationId is not null));
        if ((restorePending || (this._bucket is null)))
        {
            global::Doroti.Generated.Framework.Services.RestorationBucket newBucket__43086 = ((global::Doroti.Generated.Framework.Services.RestorationBucket)(object?)parent.claimChild(this.restorationId!, debugOwner: this));
            bool didReplace__43168 = _setNewBucketIfNecessary(newBucket: newBucket__43086, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (object.Equals(this._bucket, newBucket__43086)));
            return didReplace__43168;
        }
        DartRuntimePrimitives.Assert(() => (this._bucket is not null));
        DartRuntimePrimitives.Assert(() => !restorePending);
        this._bucket!.rename(this.restorationId!);
        parent.adoptChild(this._bucket!);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _setNewBucketIfNecessary(global::Doroti.Generated.Framework.Services.RestorationBucket? newBucket, bool restorePending)
    {
        if ((object.Equals(newBucket, this._bucket)))
        {
            return false;
        }
        global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket__43946 = this._bucket;
        this._bucket = newBucket;
        if (!restorePending)
        {
            if ((this._bucket is not null))
            {
                this._properties.Keys.forEach((__arg0) => ((global::System.Action<dynamic>)this._updateProperty)(__arg0));
            }
            didToggleBucket(oldBucket__43946);
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
        global::System.Action listener__44576 = this._properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
        property.removeListener((global::System.Action)(() => listener__44576()));
        property._unregister();
    }

}

internal class _CalendarRangePickerDialog__date_picker : global::Doroti.Generated.Framework.Widgets.StatelessWidget
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
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? entryModeButton { get; private set; }

    internal _CalendarRangePickerDialog__date_picker(global::Doroti.Generated.Framework.Foundation.Key? key = null, DateTime? selectedStartDate = default!, DateTime? selectedEndDate = default!, DateTime firstDate = default!, DateTime lastDate = default!, DateTime? currentDate = default!, global::System.Action<DateTime> onStartDateChanged = default!, global::System.Action<DateTime?> onEndDateChanged = default!, global::System.Action? onConfirm = default!, global::System.Action? onCancel = default!, string confirmText = default!, string helpText = default!, global::System.Func<DateTime, DateTime?, DateTime?, bool>? selectableDayPredicate = default!, CalendarDelegate<DateTime> calendarDelegate = default!, global::Doroti.Generated.Framework.Widgets.Widget? entryModeButton = null) : base(key: key)
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

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ThemeData theme__70516 = Theme.of(context);
        bool useMaterial3__70558 = theme__70516.useMaterial3;
        MaterialLocalizations localizations__70625 = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        global::Doroti.Generated.Framework.Widgets.Orientation orientation__70698 = MediaQuery.orientationOf(context);
        DatePickerThemeData themeData__70777 = DatePickerTheme.of(context);
        DatePickerThemeData defaults__70848 = DatePickerTheme.defaults(context);
        global::Doroti.Ui.Color? dialogBackground__70911 = ((global::Doroti.Ui.Color?)(object?)(themeData__70777.rangePickerBackgroundColor ?? defaults__70848.rangePickerBackgroundColor));
        global::Doroti.Ui.Color? headerBackground__71032 = ((global::Doroti.Ui.Color?)(object?)(themeData__70777.rangePickerHeaderBackgroundColor ?? defaults__70848.rangePickerHeaderBackgroundColor));
        global::Doroti.Ui.Color? headerForeground__71165 = ((global::Doroti.Ui.Color?)(object?)(themeData__70777.rangePickerHeaderForegroundColor ?? defaults__70848.rangePickerHeaderForegroundColor));
        global::Doroti.Ui.Color? headerDisabledForeground__71298 = ((global::Doroti.Ui.Color?)(object?)headerForeground__71165?.withOpacity(0.38));
        global::Doroti.Generated.Framework.Painting.TextStyle? headlineStyle__71383 = (themeData__70777.rangePickerHeaderHeadlineStyle ?? defaults__70848.rangePickerHeaderHeadlineStyle);
        global::Doroti.Generated.Framework.Painting.TextStyle? headlineHelpStyle__71513 = ((global::Doroti.Generated.Framework.Painting.TextStyle?)(object?)((themeData__70777.rangePickerHeaderHelpStyle ?? defaults__70848.rangePickerHeaderHelpStyle))?.apply(color: headerForeground__71165));
        string startDateText__71690 = Date_pickerLibrary._formatRangeStartDate(localizations__70625, this.calendarDelegate, this.selectedStartDate, this.selectedEndDate);
        string endDateText__71846 = Date_pickerLibrary._formatRangeEndDate(localizations__70625, this.calendarDelegate, this.selectedStartDate, this.selectedEndDate, this.calendarDelegate.now());
        global::Doroti.Generated.Framework.Painting.TextStyle? startDateStyle__72032 = ((global::Doroti.Generated.Framework.Painting.TextStyle?)(object?)headlineStyle__71383?.apply(color: ((this.selectedStartDate is not null) ? headerForeground__71165 : headerDisabledForeground__71298)));
        global::Doroti.Generated.Framework.Painting.TextStyle? endDateStyle__72185 = ((global::Doroti.Generated.Framework.Painting.TextStyle?)(object?)headlineStyle__71383?.apply(color: ((this.selectedEndDate is not null) ? headerForeground__71165 : headerDisabledForeground__71298)));
        ButtonStyle buttonStyle__72335 = TextButton.styleFrom(foregroundColor: headerForeground__71165, disabledForegroundColor: headerDisabledForeground__71298);
        var iconTheme__72486 = new global::Doroti.Generated.Framework.Widgets.IconThemeData(color: headerForeground__71165);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.SafeArea(top: false, left: false, right: false, child: new Scaffold(appBar: new AppBar(iconTheme: iconTheme__72486, actionsIconTheme: iconTheme__72486, elevation: (useMaterial3__70558 ? 0 : null), scrolledUnderElevation: (useMaterial3__70558 ? 0 : null), backgroundColor: headerBackground__71032, leading: new CloseButton(onPressed: () => this.onCancel()), actions: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection72957 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); if (((object.Equals(orientation__70698, global::Doroti.Generated.Framework.Widgets.Orientation.landscape)) && (this.entryModeButton is not null))) { __collection72957.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(this.entryModeButton!)); } __collection72957.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new TextButton(style: buttonStyle__72335, onPressed: this.onConfirm, child: new global::Doroti.Generated.Framework.Widgets.Text(this.confirmText)))); __collection72957.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(width: 8))); return __collection72957; }))(), bottom: new global::Doroti.Generated.Framework.Widgets.PreferredSize(preferredSize: new global::Doroti.Ui.Size(double.PositiveInfinity, 64), child: new global::Doroti.Generated.Framework.Widgets.Row(children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection73350 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); __collection73350.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(width: ((MediaQuery.widthOf(context) < 360L) ? 42 : 72)))); __collection73350.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Expanded(child: new global::Doroti.Generated.Framework.Widgets.Semantics(label: $"{this.helpText} {startDateText__71690} to {endDateText__71846}", excludeSemantics: true, child: new global::Doroti.Generated.Framework.Widgets.Column(crossAxisAlignment: global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.start, children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Text(this.helpText, style: headlineHelpStyle__71513, maxLines: 1L, overflow: global::Doroti.Generated.Framework.Painting.TextOverflow.ellipsis)), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(height: 8)), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Row(children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Text(startDateText__71690, style: startDateStyle__72032, maxLines: 1L, overflow: global::Doroti.Generated.Framework.Painting.TextOverflow.ellipsis)), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Text(" – ", style: startDateStyle__72032)), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Flexible(child: new global::Doroti.Generated.Framework.Widgets.Text(endDateText__71846, style: endDateStyle__72185, maxLines: 1L, overflow: global::Doroti.Generated.Framework.Painting.TextOverflow.ellipsis))) })), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(height: 16)) }))))); if (((object.Equals(orientation__70698, global::Doroti.Generated.Framework.Widgets.Orientation.portrait)) && (this.entryModeButton is not null))) { __collection73350.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 8.0), child: new global::Doroti.Generated.Framework.Widgets.IconTheme(data: iconTheme__72486, child: this.entryModeButton!)))); } return __collection73350; }))()))), backgroundColor: dialogBackground__70911, body: new _CalendarDateRangePicker__date_picker(initialStartDate: this.selectedStartDate, initialEndDate: this.selectedEndDate, firstDate: this.firstDate, lastDate: this.lastDate, currentDate: this.currentDate, onStartDateChanged: (global::System.Action<DateTime>)this.onStartDateChanged, onEndDateChanged: (global::System.Action<DateTime?>)this.onEndDateChanged, selectableDayPredicate: (global::System.Func<DateTime, DateTime?, DateTime?, bool>?)this.selectableDayPredicate, calendarDelegate: this.calendarDelegate))));
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

internal class _CalendarDateRangePicker__date_picker : global::Doroti.Generated.Framework.Widgets.StatefulWidget
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

internal class _CalendarDateRangePickerState__date_picker : global::Doroti.Generated.Framework.Widgets.State<_CalendarDateRangePicker__date_picker>
{
    internal virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> _scrollViewKey { get; private set; } = global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create();
    internal virtual global::Doroti.Generated.Framework.Foundation.Key _sliverAfterKey { get; private set; } = ((global::Doroti.Generated.Framework.Foundation.Key)(object?)new global::Doroti.Generated.Framework.Foundation.UniqueKey());
    internal virtual DateTime? _startDate { get; set; } = default;
    internal virtual DateTime? _endDate { get; set; } = default;
    internal virtual long _initialMonthIndex { get; set; } = 0L;
    internal virtual global::Doroti.Generated.Framework.Widgets.ScrollController _controller { get; set; } = default!;
    internal virtual bool _showWeekBottomDivider { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        _controller = new global::Doroti.Generated.Framework.Widgets.ScrollController();
        this._controller.addListener(() => this._scrollListener());
        _startDate = ((_CalendarDateRangePicker__date_picker)this.widget).initialStartDate;
        _endDate = ((_CalendarDateRangePicker__date_picker)this.widget).initialEndDate;
        DateTime initialDate__79386 = (((_CalendarDateRangePicker__date_picker)this.widget).initialStartDate ?? ((_CalendarDateRangePicker__date_picker)this.widget).currentDate);
        if ((!initialDate__79386.isBefore(((_CalendarDateRangePicker__date_picker)this.widget).firstDate) && !initialDate__79386.isAfter(((_CalendarDateRangePicker__date_picker)this.widget).lastDate)))
        {
            _initialMonthIndex = ((_CalendarDateRangePicker__date_picker)this.widget).calendarDelegate.monthDelta(((_CalendarDateRangePicker__date_picker)this.widget).firstDate, initialDate__79386);
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
        if ((((global::Doroti.Generated.Framework.Widgets.ScrollController)this._controller).offset <= ((global::Doroti.Generated.Framework.Widgets.ScrollController)this._controller).position.minScrollExtent))
        {
            setState(((global::System.Action)(() => {
_showWeekBottomDivider = false;
})));
        }
        else
        {
            if (!this._showWeekBottomDivider)
            {
                setState(((global::System.Action)(() => {
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
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia:
                {
                    DartRuntimePrimitives.Ignore(HapticFeedback.vibrate());
                    break;
                }
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
                {
                    break;
                }
        }
    }

    internal virtual void _updateSelection(DateTime date)
    {
        _vibrate();
        setState(((global::System.Action)(() => {
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

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildMonthItem(global::Doroti.Generated.Framework.Widgets.BuildContext context, long index, bool beforeInitialMonth)
    {
        long monthIndex__81594 = (beforeInitialMonth ? ((this._initialMonthIndex - index) - 1L) : (this._initialMonthIndex + index));
        DateTime month__81724 = ((_CalendarDateRangePicker__date_picker)this.widget).calendarDelegate.addMonthsToMonthDate(((_CalendarDateRangePicker__date_picker)this.widget).firstDate, monthIndex__81594);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _MonthItem__date_picker(calendarDelegate: ((_CalendarDateRangePicker__date_picker)this.widget).calendarDelegate, selectedDateStart: this._startDate, selectedDateEnd: this._endDate, currentDate: ((_CalendarDateRangePicker__date_picker)this.widget).currentDate, firstDate: ((_CalendarDateRangePicker__date_picker)this.widget).firstDate, lastDate: ((_CalendarDateRangePicker__date_picker)this.widget).lastDate, displayedMonth: month__81724, onChanged: (global::System.Action<DateTime>)this._updateSelection, selectableDayPredicate: (global::System.Func<DateTime, DateTime?, DateTime?, bool>?)((_CalendarDateRangePicker__date_picker)this.widget).selectableDayPredicate));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Column(children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection82299 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); __collection82299.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new _DayHeaders__date_picker())); if (this._showWeekBottomDivider) { __collection82299.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new Divider(height: 0))); } __collection82299.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Expanded(child: new _CalendarKeyboardNavigator__date_picker(calendarDelegate: ((_CalendarDateRangePicker__date_picker)this.widget).calendarDelegate, firstDate: ((_CalendarDateRangePicker__date_picker)this.widget).firstDate, lastDate: ((_CalendarDateRangePicker__date_picker)this.widget).lastDate, initialFocusedDay: ((this._startDate ?? ((_CalendarDateRangePicker__date_picker)this.widget).initialStartDate) ?? ((_CalendarDateRangePicker__date_picker)this.widget).currentDate), child: new global::Doroti.Generated.Framework.Widgets.CustomScrollView(key: this._scrollViewKey, controller: this._controller, center: this._sliverAfterKey, slivers: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(global::Doroti.Generated.Framework.Widgets.SliverList.CreateBuilder(itemCount: this._initialMonthIndex, itemBuilder: ((context, index) => _buildMonthItem(context, index, true)))), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(global::Doroti.Generated.Framework.Widgets.SliverList.CreateBuilder(key: this._sliverAfterKey, itemCount: (this._numberOfMonths - this._initialMonthIndex), itemBuilder: ((context, index) => _buildMonthItem(context, index, false)))) }))))); return __collection82299; }))()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CalendarKeyboardNavigator__date_picker : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual DateTime firstDate { get; private set; } = default!;
    public virtual DateTime lastDate { get; private set; } = default!;
    public virtual DateTime initialFocusedDay { get; private set; } = default!;
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    internal _CalendarKeyboardNavigator__date_picker(global::Doroti.Generated.Framework.Widgets.Widget child, DateTime firstDate, DateTime lastDate, DateTime initialFocusedDay, CalendarDelegate<DateTime> calendarDelegate)
    {
        this.child = child;
        this.firstDate = firstDate;
        this.lastDate = lastDate;
        this.initialFocusedDay = initialFocusedDay;
        this.calendarDelegate = calendarDelegate;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CalendarKeyboardNavigatorState__date_picker());
}

internal class _CalendarKeyboardNavigatorState__date_picker : global::Doroti.Generated.Framework.Widgets.State<_CalendarKeyboardNavigator__date_picker>
{
    internal virtual DartMap<global::Doroti.Generated.Framework.Widgets.ShortcutActivator, global::Doroti.Generated.Framework.Widgets.Intent> _shortcutMap { get; private set; } = new DartMap<global::Doroti.Generated.Framework.Widgets.ShortcutActivator, global::Doroti.Generated.Framework.Widgets.Intent> { [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowLeft)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent(global::Doroti.Generated.Framework.Widgets.TraversalDirection.left)), [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowRight)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent(global::Doroti.Generated.Framework.Widgets.TraversalDirection.right)), [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowDown)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent(global::Doroti.Generated.Framework.Widgets.TraversalDirection.down)), [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowUp)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent(global::Doroti.Generated.Framework.Widgets.TraversalDirection.up)) };
    internal virtual DartMap<Type, dynamic> _actionMap { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Widgets.FocusNode _dayGridFocus { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Widgets.TraversalDirection? _dayTraversalDirection { get; set; } = default;
    internal virtual DateTime? _focusedDay { get; set; } = default;
    internal static DartMap<global::Doroti.Generated.Framework.Widgets.TraversalDirection, long> _directionOffset = new DartMap<global::Doroti.Generated.Framework.Widgets.TraversalDirection, long> { [global::Doroti.Generated.Framework.Widgets.TraversalDirection.up] = -7L, [global::Doroti.Generated.Framework.Widgets.TraversalDirection.right] = 1L, [global::Doroti.Generated.Framework.Widgets.TraversalDirection.down] = 7L, [global::Doroti.Generated.Framework.Widgets.TraversalDirection.left] = -1L };

    public override void initState()
    {
        base.initState();
        _actionMap = new DartMap<Type, dynamic> { [typeof(global::Doroti.Generated.Framework.Widgets.NextFocusIntent)] = new global::Doroti.Generated.Framework.Widgets.CallbackAction<global::Doroti.Generated.Framework.Widgets.NextFocusIntent>(onInvoke: (global::System.Action<global::Doroti.Generated.Framework.Widgets.NextFocusIntent>)this._handleGridNextFocus), [typeof(global::Doroti.Generated.Framework.Widgets.PreviousFocusIntent)] = new global::Doroti.Generated.Framework.Widgets.CallbackAction<global::Doroti.Generated.Framework.Widgets.PreviousFocusIntent>(onInvoke: (global::System.Action<global::Doroti.Generated.Framework.Widgets.PreviousFocusIntent>)this._handleGridPreviousFocus), [typeof(global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent)] = new global::Doroti.Generated.Framework.Widgets.CallbackAction<global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent>(onInvoke: (global::System.Action<global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent>)this._handleDirectionFocus) };
        _dayGridFocus = new global::Doroti.Generated.Framework.Widgets.FocusNode(debugLabel: "Day Grid");
    }

    public override void dispose()
    {
        this._dayGridFocus.dispose();
        base.dispose();
    }

    internal virtual void _handleGridFocusChange(bool focused)
    {
        setState(((global::System.Action)(() => {
if (focused)
{
    _focusedDay ??= ((_CalendarKeyboardNavigator__date_picker)this.widget).initialFocusedDay;
}
})));
    }

    internal virtual void _handleGridNextFocus(global::Doroti.Generated.Framework.Widgets.NextFocusIntent intent)
    {
        this._dayGridFocus.requestFocus();
        this._dayGridFocus.nextFocus();
    }

    internal virtual void _handleGridPreviousFocus(global::Doroti.Generated.Framework.Widgets.PreviousFocusIntent intent)
    {
        this._dayGridFocus.requestFocus();
        this._dayGridFocus.previousFocus();
    }

    internal virtual void _handleDirectionFocus(global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent intent)
    {
        DartRuntimePrimitives.Assert(() => (this._focusedDay is not null));
        setState(((global::System.Action)(() => {
DateTime? nextDate__86735 = _nextDateInDirection(DartRuntimePrimitives.RequireValue(this._focusedDay), ((global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent)intent).direction);
if ((nextDate__86735 is not null))
{
    DateTime nextDate__86735__value86810 = DartRuntimePrimitives.RequireValue(nextDate__86735);
    _focusedDay = DartRuntimePrimitives.RequireValue(nextDate__86735__value86810);
    _dayTraversalDirection = ((global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent)intent).direction;
}
})));
    }

    internal virtual long _dayDirectionOffset(global::Doroti.Generated.Framework.Widgets.TraversalDirection traversalDirection, TextDirection textDirection)
    {
        if ((object.Equals(textDirection, TextDirection.rtl)))
        {
            if ((object.Equals(traversalDirection, global::Doroti.Generated.Framework.Widgets.TraversalDirection.left)))
            {
                traversalDirection = global::Doroti.Generated.Framework.Widgets.TraversalDirection.right;
            }
            else
            {
                if ((object.Equals(traversalDirection, global::Doroti.Generated.Framework.Widgets.TraversalDirection.right)))
                {
                    traversalDirection = global::Doroti.Generated.Framework.Widgets.TraversalDirection.left;
                }
            }
        }
        return DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<long>(_directionOffset, traversalDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual DateTime? _nextDateInDirection(DateTime date, global::Doroti.Generated.Framework.Widgets.TraversalDirection direction)
    {
        global::Doroti.Ui.TextDirection textDirection__87803 = Directionality.of(this.context);
        DateTime nextDate__87866 = ((_CalendarKeyboardNavigator__date_picker)this.widget).calendarDelegate.addDaysToDate(date, _dayDirectionOffset(direction, textDirection__87803));
        if ((!nextDate__87866.isBefore(((_CalendarKeyboardNavigator__date_picker)this.widget).firstDate) && !nextDate__87866.isAfter(((_CalendarKeyboardNavigator__date_picker)this.widget).lastDate)))
        {
            return nextDate__87866;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.FocusableActionDetector(shortcuts: this._shortcutMap, actions: this._actionMap, focusNode: this._dayGridFocus, onFocusChange: (global::System.Action<bool>)this._handleGridFocusChange, child: new _FocusedDate__date_picker(calendarDelegate: ((_CalendarKeyboardNavigator__date_picker)this.widget).calendarDelegate, date: (((global::Doroti.Generated.Framework.Widgets.FocusNode)this._dayGridFocus).hasFocus ? this._focusedDay : null), scrollDirection: (((global::Doroti.Generated.Framework.Widgets.FocusNode)this._dayGridFocus).hasFocus ? this._dayTraversalDirection : null), child: ((_CalendarKeyboardNavigator__date_picker)this.widget).child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _FocusedDate__date_picker : global::Doroti.Generated.Framework.Widgets.InheritedWidget
{
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;
    public virtual DateTime? date { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.TraversalDirection? scrollDirection { get; private set; }

    internal _FocusedDate__date_picker(global::Doroti.Generated.Framework.Widgets.Widget child, CalendarDelegate<DateTime> calendarDelegate, DateTime? date = null, global::Doroti.Generated.Framework.Widgets.TraversalDirection? scrollDirection = null) : base(child: child)
    {
        this.calendarDelegate = calendarDelegate;
        this.date = date;
        this.scrollDirection = scrollDirection;
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget)
    {
        var __oldWidget = (_FocusedDate__date_picker)(object)oldWidget;
        return (!this.calendarDelegate.isSameDay(this.date, ((_FocusedDate__date_picker)__oldWidget).date) || (!object.Equals(this.scrollDirection, ((_FocusedDate__date_picker)__oldWidget).scrollDirection)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static _FocusedDate__date_picker? maybeOf(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((_FocusedDate__date_picker?)(object?)context.dependOnInheritedWidgetOfExactType<_FocusedDate__date_picker>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DayHeaders__date_picker : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    internal _DayHeaders__date_picker()
    {
    }

    internal virtual List<global::Doroti.Generated.Framework.Widgets.Widget> _getDayHeaders(global::Doroti.Generated.Framework.Painting.TextStyle headerStyle, MaterialLocalizations localizations)
    {
        var result__90110 = new List<global::Doroti.Generated.Framework.Widgets.Widget>();
        for (long i__90151 = ((MaterialLocalizations)localizations).firstDayOfWeekIndex; (checked((long)(result__90110.Count)) < 7L); i__90151 = (((i__90151 + 1L)) % 7L))
        {
            string weekday__90302 = ((MaterialLocalizations)localizations).narrowWeekdays[(int)(i__90151)];
            result__90110.Add(new global::Doroti.Generated.Framework.Widgets.ExcludeSemantics(child: new global::Doroti.Generated.Framework.Widgets.Center(child: new global::Doroti.Generated.Framework.Widgets.Text(weekday__90302, style: headerStyle))));
        }
        return result__90110;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ThemeData themeData__90577 = Theme.of(context);
        ColorScheme colorScheme__90630 = themeData__90577.colorScheme;
        global::Doroti.Generated.Framework.Painting.TextStyle textStyle__90687 = ((global::Doroti.Generated.Framework.Painting.TextStyle)(object?)themeData__90577.textTheme.titleSmall!.apply(color: colorScheme__90630.onSurface));
        MaterialLocalizations localizations__90800 = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        List<global::Doroti.Generated.Framework.Widgets.Widget> labels__90874 = ((List<global::Doroti.Generated.Framework.Widgets.Widget>)(object?)_getDayHeaders(textStyle__90687, localizations__90800));
        labels__90874.Insert(checked((int)0L), global::Doroti.Generated.Framework.Widgets.SizedBox.CreateShrink());
        labels__90874.Add(global::Doroti.Generated.Framework.Widgets.SizedBox.CreateShrink());
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Generated.Framework.Rendering.BoxConstraints(maxWidth: ((object.Equals(MediaQuery.orientationOf(context), global::Doroti.Generated.Framework.Widgets.Orientation.landscape)) ? Date_pickerLibrary._maxCalendarWidthLandscape : Date_pickerLibrary._maxCalendarWidthPortrait), maxHeight: Date_pickerLibrary._monthItemRowHeight), child: global::Doroti.Generated.Framework.Widgets.GridView.CreateCustom(shrinkWrap: true, gridDelegate: Date_pickerLibrary._monthItemGridDelegate, childrenDelegate: new global::Doroti.Generated.Framework.Widgets.SliverChildListDelegate(labels__90874, addRepaintBoundaries: false))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MonthItemGridDelegate__date_picker : global::Doroti.Generated.Framework.Rendering.SliverGridDelegate
{
    internal _MonthItemGridDelegate__date_picker()
    {
    }

    public virtual global::Doroti.Generated.Framework.Rendering.SliverGridLayout getLayout(global::Doroti.Generated.Framework.Rendering.SliverConstraints constraints)
    {
        double tileWidth__91757 = Math.Max((((((global::Doroti.Generated.Framework.Rendering.SliverConstraints)constraints).crossAxisExtent - (2L * Date_pickerLibrary._horizontalPadding))) / 7L), 0.0);
        return ((global::Doroti.Generated.Framework.Rendering.SliverGridLayout)(object?)new _MonthSliverGridLayout__date_picker(crossAxisCount: (7L + 2L), dayChildWidth: tileWidth__91757, edgeChildWidth: Date_pickerLibrary._horizontalPadding, reverseCrossAxis: global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.axisDirectionIsReversed(((global::Doroti.Generated.Framework.Rendering.SliverConstraints)constraints).crossAxisDirection)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool shouldRelayout(global::Doroti.Generated.Framework.Rendering.SliverGridDelegate oldDelegate) => false;
}

public static partial class Date_pickerLibrary
{
    internal static _MonthItemGridDelegate__date_picker _monthItemGridDelegate = new _MonthItemGridDelegate__date_picker();
}

internal class _MonthSliverGridLayout__date_picker : global::Doroti.Generated.Framework.Rendering.SliverGridLayout
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
        long mainAxisCount__94200 = ((scrollOffset / this._rowHeight)).ceil();
        return Math.Max(0L, ((this.crossAxisCount * mainAxisCount__94200) - 1L));
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

    public virtual global::Doroti.Generated.Framework.Rendering.SliverGridGeometry getGeometryForChildIndex(long index)
    {
        long adjustedIndex__94696 = (index % this.crossAxisCount);
        bool isEdge__94751 = ((adjustedIndex__94696 == 0L) || (adjustedIndex__94696 == (this.crossAxisCount - 1L)));
        double crossAxisStart__94836 = Math.Max(0, ((((adjustedIndex__94696 - 1L)) * this.dayChildWidth) + this.edgeChildWidth));
        return new global::Doroti.Generated.Framework.Rendering.SliverGridGeometry(scrollOffset: (((checked((long)(index / this.crossAxisCount)))) * this._rowHeight), crossAxisOffset: _getCrossAxisOffset(crossAxisStart__94836, isEdge__94751), mainAxisExtent: this._childHeight, crossAxisExtent: (isEdge__94751 ? this.edgeChildWidth : this.dayChildWidth));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double computeMaxScrollOffset(long childCount)
    {
        DartRuntimePrimitives.Assert(() => (childCount >= 0L));
        long mainAxisCount__95297 = (((checked((long)(((childCount - 1L)) / this.crossAxisCount)))) + 1L);
        double mainAxisSpacing__95372 = (this._rowHeight - this._childHeight);
        return ((this._rowHeight * mainAxisCount__95297) - mainAxisSpacing__95372);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MonthItem__date_picker : global::Doroti.Generated.Framework.Widgets.StatefulWidget
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

internal class _MonthItemState__date_picker : global::Doroti.Generated.Framework.Widgets.State<_MonthItem__date_picker>
{
    internal virtual List<global::Doroti.Generated.Framework.Widgets.FocusNode> _dayFocusNodes { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        long daysInMonth__97734 = ((_MonthItem__date_picker)this.widget).calendarDelegate.getDaysInMonth(((_MonthItem__date_picker)this.widget).displayedMonth.Year, ((_MonthItem__date_picker)this.widget).displayedMonth.Month);
        _dayFocusNodes = new List<global::Doroti.Generated.Framework.Widgets.FocusNode>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)daysInMonth__97734)), ((index) => new global::Doroti.Generated.Framework.Widgets.FocusNode(skipTraversal: true, debugLabel: $"Day {(index + 1L)}"))));
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        DateTime? focusedDate__98201 = _FocusedDate__date_picker.maybeOf(this.context)?.date;
        if (((focusedDate__98201 is not null) && ((_MonthItem__date_picker)this.widget).calendarDelegate.isSameMonth(((_MonthItem__date_picker)this.widget).displayedMonth, DartRuntimePrimitives.RequireValue(focusedDate__98201))))
        {
            DateTime focusedDate__98201__value98260 = DartRuntimePrimitives.RequireValue(focusedDate__98201);
            this._dayFocusNodes[(int)((DartRuntimePrimitives.RequireValue(focusedDate__98201__value98260).Day - 1L))].requestFocus();
        }
    }

    public override void dispose()
    {
        foreach (global::Doroti.Generated.Framework.Widgets.FocusNode node__98491 in this._dayFocusNodes)
        {
            node__98491.dispose();
        }
        base.dispose();
    }

    internal virtual global::Doroti.Ui.Color _highlightColor(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Ui.Color)(object?)(DatePickerTheme.of(context).rangeSelectionBackgroundColor ?? DatePickerTheme.defaults(context).rangeSelectionBackgroundColor!));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _dayFocusChanged(bool focused)
    {
        if (focused)
        {
            global::Doroti.Generated.Framework.Widgets.TraversalDirection? focusDirection__98861 = _FocusedDate__date_picker.maybeOf(this.context)?.scrollDirection;
            if ((focusDirection__98861 is not null))
            {
                global::Doroti.Generated.Framework.Widgets.TraversalDirection focusDirection__98861__value98936 = DartRuntimePrimitives.RequireValue(focusDirection__98861);
                global::Doroti.Generated.Framework.Widgets.ScrollPositionAlignmentPolicy policy__99000 = global::Doroti.Generated.Framework.Widgets.ScrollPositionAlignmentPolicy.@explicit;
                switch (DartRuntimePrimitives.RequireValue(focusDirection__98861__value98936))
                {
                    case global::Doroti.Generated.Framework.Widgets.TraversalDirection.up:
                    case global::Doroti.Generated.Framework.Widgets.TraversalDirection.left:
                        {
                            policy__99000 = global::Doroti.Generated.Framework.Widgets.ScrollPositionAlignmentPolicy.keepVisibleAtStart;
                            break;
                        }
                    case global::Doroti.Generated.Framework.Widgets.TraversalDirection.right:
                    case global::Doroti.Generated.Framework.Widgets.TraversalDirection.down:
                        {
                            policy__99000 = global::Doroti.Generated.Framework.Widgets.ScrollPositionAlignmentPolicy.keepVisibleAtEnd;
                            break;
                        }
                }
                DartRuntimePrimitives.Ignore(Scrollable.ensureVisible(global::Doroti.Generated.Framework.Widgets.Focus_managerLibrary.primaryFocus!.context!, duration: Calendar_date_pickerLibrary._monthScrollDuration, alignmentPolicy: policy__99000));
            }
        }
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildDayItem(global::Doroti.Generated.Framework.Widgets.BuildContext context, DateTime dayToBuild, long firstDayOffset, long daysInMonth)
    {
        long day__99707 = dayToBuild.Day;
        bool isDisabled__99745 = ((dayToBuild.isAfter(((_MonthItem__date_picker)this.widget).lastDate) || dayToBuild.isBefore(((_MonthItem__date_picker)this.widget).firstDate)) || ((((_MonthItem__date_picker)this.widget).selectableDayPredicate is not null) && !((_MonthItem__date_picker)this.widget).selectableDayPredicate!(dayToBuild, ((_MonthItem__date_picker)this.widget).selectedDateStart, ((_MonthItem__date_picker)this.widget).selectedDateEnd)));
        bool isRangeSelected__100082 = ((((_MonthItem__date_picker)this.widget).selectedDateStart is not null) && (((_MonthItem__date_picker)this.widget).selectedDateEnd is not null));
        bool isSelectedDayStart__100183 = ((((_MonthItem__date_picker)this.widget).selectedDateStart is not null) && dayToBuild.isAtSameMomentAs(DartRuntimePrimitives.RequireValue(((_MonthItem__date_picker)this.widget).selectedDateStart)));
        bool isSelectedDayEnd__100319 = ((((_MonthItem__date_picker)this.widget).selectedDateEnd is not null) && dayToBuild.isAtSameMomentAs(DartRuntimePrimitives.RequireValue(((_MonthItem__date_picker)this.widget).selectedDateEnd)));
        bool isInRange__100449 = ((isRangeSelected__100082 && dayToBuild.isAfter(DartRuntimePrimitives.RequireValue(((_MonthItem__date_picker)this.widget).selectedDateStart))) && dayToBuild.isBefore(DartRuntimePrimitives.RequireValue(((_MonthItem__date_picker)this.widget).selectedDateEnd)));
        bool isOneDayRange__100614 = (isRangeSelected__100082 && (object.Equals(((_MonthItem__date_picker)this.widget).selectedDateStart, ((_MonthItem__date_picker)this.widget).selectedDateEnd)));
        bool isToday__100724 = ((_MonthItem__date_picker)this.widget).calendarDelegate.isSameDay(((_MonthItem__date_picker)this.widget).currentDate, dayToBuild);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _DayItem__date_picker(calendarDelegate: ((_MonthItem__date_picker)this.widget).calendarDelegate, day: dayToBuild, focusNode: this._dayFocusNodes[(int)((day__99707 - 1L))], onChanged: (global::System.Action<DateTime>)((_MonthItem__date_picker)this.widget).onChanged, onFocusChange: (global::System.Action<bool>)this._dayFocusChanged, highlightColor: _highlightColor(context), isDisabled: isDisabled__99745, isRangeSelected: isRangeSelected__100082, isSelectedDayStart: isSelectedDayStart__100183, isSelectedDayEnd: isSelectedDayEnd__100319, isInRange: isInRange__100449, isOneDayRange: isOneDayRange__100614, isToday: isToday__100724));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildEdgeBox(global::Doroti.Generated.Framework.Widgets.BuildContext context, bool isHighlighted)
    {
        global::Doroti.Generated.Framework.Widgets.Widget empty__101401 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.LimitedBox(maxWidth: 0.0, maxHeight: 0.0, child: global::Doroti.Generated.Framework.Widgets.SizedBox.CreateExpand()));
        return (isHighlighted ? new global::Doroti.Generated.Framework.Widgets.ColoredBox(color: _highlightColor(context), child: empty__101401) : empty__101401);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ThemeData themeData__101648 = Theme.of(context);
        TextTheme textTheme__101699 = themeData__101648.textTheme;
        MaterialLocalizations localizations__101764 = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        long year__101829 = ((_MonthItem__date_picker)this.widget).displayedMonth.Year;
        long month__101878 = ((_MonthItem__date_picker)this.widget).displayedMonth.Month;
        long daysInMonth__101929 = ((_MonthItem__date_picker)this.widget).calendarDelegate.getDaysInMonth(year__101829, month__101878);
        long dayOffset__102010 = ((_MonthItem__date_picker)this.widget).calendarDelegate.firstDayOffset(year__101829, month__101878, localizations__101764);
        long weeks__102104 = ((((daysInMonth__101929 + dayOffset__102010)) / 7L)).ceil();
        double gridHeight__102188 = ((weeks__102104 * Date_pickerLibrary._monthItemRowHeight) + (((weeks__102104 - 1L)) * Date_pickerLibrary._monthItemSpaceBetweenRows));
        var dayItems__102291 = new List<global::Doroti.Generated.Framework.Widgets.Widget>();
        for (long day__102425 = ((0L - dayOffset__102010) + 1L); (day__102425 <= daysInMonth__101929); day__102425 += 1L)
        {
            if ((day__102425 < 1L))
            {
                dayItems__102291.Add(new global::Doroti.Generated.Framework.Widgets.LimitedBox(maxWidth: 0.0, maxHeight: 0.0, child: global::Doroti.Generated.Framework.Widgets.SizedBox.CreateExpand()));
            }
            else
            {
                DateTime dayToBuild__102638 = ((_MonthItem__date_picker)this.widget).calendarDelegate.getDay(year__101829, month__101878, day__102425);
                global::Doroti.Generated.Framework.Widgets.Widget dayItem__102722 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)_buildDayItem(context, dayToBuild__102638, dayOffset__102010, daysInMonth__101929));
                dayItems__102291.Add(dayItem__102722);
            }
        }
        var paddedDayItems__102966 = new List<global::Doroti.Generated.Framework.Widgets.Widget>();
        for (var i__103008 = 0L; (i__103008 < weeks__102104); i__103008++)
        {
            long start__103049 = (i__103008 * 7L);
            long end__103099 = Math.Min((start__103049 + 7L), checked((long)(dayItems__102291.Count)));
            List<global::Doroti.Generated.Framework.Widgets.Widget> weekList__103187 = dayItems__102291.GetRange(start__103049, end__103099).ToList();
            DateTime dateAfterLeadingPadding__103250 = ((_MonthItem__date_picker)this.widget).calendarDelegate.getDay(year__101829, month__101878, ((start__103049 - dayOffset__102010) + 1L));
            bool isLeadingInRange__103500 = ((((!(((dayOffset__102010 > 0L) && (i__103008 == 0L))) && (((_MonthItem__date_picker)this.widget).selectedDateStart is not null)) && (((_MonthItem__date_picker)this.widget).selectedDateEnd is not null)) && dateAfterLeadingPadding__103250.isAfter(DartRuntimePrimitives.RequireValue(((_MonthItem__date_picker)this.widget).selectedDateStart))) && !dateAfterLeadingPadding__103250.isAfter(DartRuntimePrimitives.RequireValue(((_MonthItem__date_picker)this.widget).selectedDateEnd)));
            weekList__103187.Insert(checked((int)0L), _buildEdgeBox(context, isLeadingInRange__103500));
            if (((end__103099 < checked((long)(dayItems__102291.Count))) || (((end__103099 == checked((long)(dayItems__102291.Count))) && ((checked((long)(dayItems__102291.Count)) % 7L) == 0L)))))
            {
                DateTime dateBeforeTrailingPadding__104104 = ((_MonthItem__date_picker)this.widget).calendarDelegate.getDay(year__101829, month__101878, (end__103099 - dayOffset__102010));
                bool isTrailingInRange__104364 = ((((((_MonthItem__date_picker)this.widget).selectedDateStart is not null) && (((_MonthItem__date_picker)this.widget).selectedDateEnd is not null)) && !dateBeforeTrailingPadding__104104.isBefore(DartRuntimePrimitives.RequireValue(((_MonthItem__date_picker)this.widget).selectedDateStart))) && dateBeforeTrailingPadding__104104.isBefore(DartRuntimePrimitives.RequireValue(((_MonthItem__date_picker)this.widget).selectedDateEnd)));
                weekList__103187.Add(_buildEdgeBox(context, isTrailingInRange__104364));
            }
            paddedDayItems__102966.AddRange(weekList__103187.Cast<global::Doroti.Generated.Framework.Widgets.Widget>());
        }
        double maxWidth__104766 = ((object.Equals(MediaQuery.orientationOf(context), global::Doroti.Generated.Framework.Widgets.Orientation.landscape)) ? Date_pickerLibrary._maxCalendarWidthLandscape : Date_pickerLibrary._maxCalendarWidthPortrait);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Column(children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Generated.Framework.Rendering.BoxConstraints(maxWidth: maxWidth__104766).tighten(height: Date_pickerLibrary._monthItemHeaderHeight), child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 16), child: new global::Doroti.Generated.Framework.Widgets.Align(alignment: global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerStart, child: new global::Doroti.Generated.Framework.Widgets.ExcludeSemantics(child: new global::Doroti.Generated.Framework.Widgets.Text(((_MonthItem__date_picker)this.widget).calendarDelegate.formatMonthYear(((_MonthItem__date_picker)this.widget).displayedMonth, localizations__101764), style: textTheme__101699.bodyMedium!.apply(color: themeData__101648.colorScheme.onSurface))))))), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Generated.Framework.Rendering.BoxConstraints(maxWidth: maxWidth__104766, maxHeight: gridHeight__102188), child: global::Doroti.Generated.Framework.Widgets.GridView.CreateCustom(physics: new global::Doroti.Generated.Framework.Widgets.NeverScrollableScrollPhysics(), gridDelegate: Date_pickerLibrary._monthItemGridDelegate, childrenDelegate: new global::Doroti.Generated.Framework.Widgets.SliverChildListDelegate(paddedDayItems__102966, addRepaintBoundaries: false)))), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(height: Date_pickerLibrary._monthItemFooterHeight)) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DayItem__date_picker : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual DateTime day { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.FocusNode focusNode { get; private set; } = default!;
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

    internal _DayItem__date_picker(DateTime day, global::Doroti.Generated.Framework.Widgets.FocusNode focusNode, global::System.Action<DateTime> onChanged, global::System.Action<bool> onFocusChange, Color highlightColor, bool isDisabled, bool isRangeSelected, bool isSelectedDayStart, bool isSelectedDayEnd, bool isInRange, bool isOneDayRange, bool isToday, CalendarDelegate<DateTime> calendarDelegate)
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

internal class _DayItemState__date_picker : global::Doroti.Generated.Framework.Widgets.State<_DayItem__date_picker>
{
    internal virtual global::Doroti.Generated.Framework.Widgets.WidgetStatesController _statesController { get; private set; } = new global::Doroti.Generated.Framework.Widgets.WidgetStatesController();

    public override void dispose()
    {
        this._statesController.dispose();
        base.dispose();
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ThemeData theme__107294 = Theme.of(context);
        ColorScheme colorScheme__107343 = theme__107294.colorScheme;
        TextTheme textTheme__107396 = theme__107294.textTheme;
        MaterialLocalizations localizations__107457 = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        DatePickerThemeData datePickerTheme__107538 = DatePickerTheme.of(context);
        DatePickerThemeData defaults__107615 = DatePickerTheme.defaults(context);
        global::Doroti.Ui.TextDirection textDirection__107685 = Directionality.of(context);
        global::Doroti.Ui.Color highlightColor__107745 = ((global::Doroti.Ui.Color)(object?)((_DayItem__date_picker)this.widget).highlightColor);
        global::Doroti.Generated.Framework.Painting.ShapeDecoration? decoration__107807 = default!;
        global::Doroti.Generated.Framework.Painting.TextStyle? itemStyle__107834 = textTheme__107396.bodyMedium;
        P? effectiveValue<P>(global::System.Func<DatePickerThemeData?, P?> getProperty)
        {
            return (getProperty(datePickerTheme__107538) ?? getProperty(defaults__107615));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        P? resolve<P>(global::System.Func<DatePickerThemeData?, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<P>?> getProperty, HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> states)
        {
            return effectiveValue(((theme) => {
return getProperty(theme) is { } property ? property.resolve(states) : default;
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        var states__108300 = ((Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>>)(() => { var __collection108309 = new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>(); if (((_DayItem__date_picker)this.widget).isDisabled) { __collection108309.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled); } if ((((_DayItem__date_picker)this.widget).isSelectedDayStart || ((_DayItem__date_picker)this.widget).isSelectedDayEnd)) { __collection108309.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.selected); } return __collection108309; }))();
        this._statesController.value = states__108300;
        global::Doroti.Ui.Color? dayForegroundColor__108525 = ((global::Doroti.Ui.Color?)(object?)resolve<global::Doroti.Ui.Color?>(((theme) => theme?.dayForegroundColor), states__108300));
        global::Doroti.Ui.Color? dayBackgroundColor__108666 = ((global::Doroti.Ui.Color?)(object?)resolve<global::Doroti.Ui.Color?>(((theme) => theme?.dayBackgroundColor), states__108300));
        global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> dayOverlayColor__108828 = ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>)(object?)WidgetStateProperty.resolveWith<global::Doroti.Ui.Color?>(((global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, global::Doroti.Ui.Color?>)((states) => effectiveValue(((theme) => (((_DayItem__date_picker)this.widget).isInRange ? theme?.rangeSelectionOverlayColor?.resolve(states) : theme?.dayOverlayColor?.resolve(states))))))));
        global::Doroti.Generated.Framework.Painting.OutlinedBorder dayShape__109157 = (resolve<global::Doroti.Generated.Framework.Painting.OutlinedBorder?>(((theme) => theme?.dayShape), states__108300) ?? new global::Doroti.Generated.Framework.Painting.CircleBorder());
        _HighlightPainter__date_picker? highlightPainter__109315 = default!;
        if ((((_DayItem__date_picker)this.widget).isSelectedDayStart || ((_DayItem__date_picker)this.widget).isSelectedDayEnd))
        {
            itemStyle__107834 = itemStyle__107834?.apply(color: dayForegroundColor__108525);
            decoration__107807 = new global::Doroti.Generated.Framework.Painting.ShapeDecoration(color: dayBackgroundColor__108666, shape: dayShape__109157);
            if ((((_DayItem__date_picker)this.widget).isRangeSelected && !((_DayItem__date_picker)this.widget).isOneDayRange))
            {
                _HighlightPainterStyle__date_picker style__109763 = (((_DayItem__date_picker)this.widget).isSelectedDayStart ? _HighlightPainterStyle__date_picker.highlightTrailing : _HighlightPainterStyle__date_picker.highlightLeading);
                highlightPainter__109315 = new _HighlightPainter__date_picker(color: highlightColor__107745, style: style__109763, textDirection: textDirection__107685);
            }
        }
        else
        {
            if (((_DayItem__date_picker)this.widget).isInRange)
            {
                highlightPainter__109315 = new _HighlightPainter__date_picker(color: highlightColor__107745, style: _HighlightPainterStyle__date_picker.highlightAll, textDirection: textDirection__107685);
                if (((_DayItem__date_picker)this.widget).isDisabled)
                {
                    itemStyle__107834 = itemStyle__107834?.apply(color: colorScheme__107343.onSurface.withOpacity(0.38));
                }
            }
            else
            {
                if (((_DayItem__date_picker)this.widget).isDisabled)
                {
                    itemStyle__107834 = itemStyle__107834?.apply(color: colorScheme__107343.onSurface.withOpacity(0.38));
                }
                else
                {
                    if (((_DayItem__date_picker)this.widget).isToday)
                    {
                        itemStyle__107834 = itemStyle__107834?.apply(color: colorScheme__107343.primary);
                        global::Doroti.Generated.Framework.Painting.BorderSide todaySide__110792 = ((global::Doroti.Generated.Framework.Painting.BorderSide)(object?)((datePickerTheme__107538.todayBorder ?? defaults__107615.todayBorder!)).copyWith(color: colorScheme__107343.primary));
                        decoration__107807 = new global::Doroti.Generated.Framework.Painting.ShapeDecoration(shape: dayShape__109157.copyWith(side: todaySide__110792));
                    }
                }
            }
        }
        string dayText__111018 = ((string)(object?)localizations__107457.formatDecimal(((_DayItem__date_picker)this.widget).day.Day));
        var semanticLabelSuffix__111460 = (((_DayItem__date_picker)this.widget).isToday ? $", {((MaterialLocalizations)localizations__107457).currentDateLabel}" : "");
        var semanticLabel__111551 = $"{dayText__111018}, {((_DayItem__date_picker)this.widget).calendarDelegate.formatFullDate(((_DayItem__date_picker)this.widget).day, localizations__107457)}{semanticLabelSuffix__111460}";
        if (((_DayItem__date_picker)this.widget).isSelectedDayStart)
        {
            semanticLabel__111551 = localizations__107457.dateRangeStartDateSemanticLabel(semanticLabel__111551);
        }
        else
        {
            if (((_DayItem__date_picker)this.widget).isSelectedDayEnd)
            {
                semanticLabel__111551 = localizations__107457.dateRangeEndDateSemanticLabel(semanticLabel__111551);
            }
        }
        global::Doroti.Generated.Framework.Widgets.Widget dayWidget__111940 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Container(decoration: decoration__107807, alignment: global::Doroti.Generated.Framework.Painting.Alignment.center, child: new global::Doroti.Generated.Framework.Widgets.Semantics(label: semanticLabel__111551, selected: (((_DayItem__date_picker)this.widget).isSelectedDayStart || ((_DayItem__date_picker)this.widget).isSelectedDayEnd), child: new global::Doroti.Generated.Framework.Widgets.ExcludeSemantics(child: new global::Doroti.Generated.Framework.Widgets.Text(dayText__111018, style: itemStyle__107834)))));
        if ((highlightPainter__109315 is not null))
        {
            dayWidget__111940 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.CustomPaint(painter: highlightPainter__109315, child: dayWidget__111940));
        }
        if (!((_DayItem__date_picker)this.widget).isDisabled)
        {
            dayWidget__111940 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new InkResponse(focusNode: ((_DayItem__date_picker)this.widget).focusNode, onTap: (() => { this.widget.onChanged(((_DayItem__date_picker)this.widget).day); }), customBorder: dayShape__109157, containedInkWell: true, statesController: this._statesController, overlayColor: dayOverlayColor__108828, onFocusChange: ((_DayItem__date_picker)this.widget).onFocusChange, child: dayWidget__111940));
        }
        return dayWidget__111940;
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

internal class _HighlightPainter__date_picker : global::Doroti.Generated.Framework.Rendering.CustomPainter
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
        var paint__113911 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = this.color;
            __cascade.style = PaintingStyle.fill;
            return __cascade;        }))();
        bool rtl__114001 = (this.textDirection switch { TextDirection.rtl => true, null => true, TextDirection.ltr => false, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        switch (this.style)
        {
            case _HighlightPainterStyle__date_picker.highlightLeading when (rtl__114001):
            case _HighlightPainterStyle__date_picker.highlightTrailing when (!rtl__114001):
                {
                    canvas.drawRect(global::Doroti.Ui.Rect.fromLTWH((size.width / 2L), 0, (size.width / 2L), size.height), paint__113911);
                    break;
                }
            case _HighlightPainterStyle__date_picker.highlightLeading:
            case _HighlightPainterStyle__date_picker.highlightTrailing:
                {
                    canvas.drawRect(global::Doroti.Ui.Rect.fromLTWH(0, 0, (size.width / 2L), size.height), paint__113911);
                    break;
                }
            case _HighlightPainterStyle__date_picker.highlightAll:
                {
                    canvas.drawRect(global::Doroti.Ui.Rect.fromLTWH(0, 0, size.width, size.height), paint__113911);
                    break;
                }
            case _HighlightPainterStyle__date_picker.none:
                {
                    break;
                }
        }
    }

    public override bool shouldRepaint(global::Doroti.Generated.Framework.Rendering.CustomPainter oldDelegate) => false;
}

internal class _InputDateRangePickerDialog__date_picker : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual DateTime? selectedStartDate { get; private set; }
    public virtual DateTime? selectedEndDate { get; private set; }
    public virtual DateTime? currentDate { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget picker { get; private set; } = default!;
    public virtual global::System.Action onConfirm { get; private set; } = default!;
    public virtual global::System.Action onCancel { get; private set; } = default!;
    public virtual string? confirmText { get; private set; }
    public virtual string? cancelText { get; private set; }
    public virtual string? helpText { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? entryModeButton { get; private set; }
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    internal _InputDateRangePickerDialog__date_picker(DateTime? selectedStartDate, DateTime? selectedEndDate, DateTime? currentDate, global::Doroti.Generated.Framework.Widgets.Widget picker, global::System.Action onConfirm, global::System.Action onCancel, string? confirmText, string? cancelText, string? helpText, global::Doroti.Generated.Framework.Widgets.Widget? entryModeButton, CalendarDelegate<DateTime> calendarDelegate)
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

    internal virtual string _formatDateRange(global::Doroti.Generated.Framework.Widgets.BuildContext context, DateTime? start, DateTime? end, DateTime now)
    {
        MaterialLocalizations localizations__115745 = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        string startText__115813 = Date_pickerLibrary._formatRangeStartDate(localizations__115745, this.calendarDelegate, start, end);
        string endText__115910 = Date_pickerLibrary._formatRangeEndDate(localizations__115745, this.calendarDelegate, start, end, now);
        if (((start is null) || (end is null)))
        {
            return ((MaterialLocalizations)localizations__115745).unspecifiedDateRange;
        }
        return (Directionality.of(context) switch { TextDirection.rtl => $"{endText__115910} – {startText__115813}", TextDirection.ltr => $"{startText__115813} – {endText__115910}", _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        bool useMaterial3__116317 = Theme.of(context).useMaterial3;
        MaterialLocalizations localizations__116396 = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        global::Doroti.Generated.Framework.Widgets.Orientation orientation__116469 = MediaQuery.orientationOf(context);
        DatePickerThemeData datePickerTheme__116548 = DatePickerTheme.of(context);
        DatePickerThemeData defaults__116625 = DatePickerTheme.defaults(context);
        global::Doroti.Generated.Framework.Painting.TextStyle? headlineStyle__116934 = (((object.Equals(orientation__116469, global::Doroti.Generated.Framework.Widgets.Orientation.portrait))) ? (datePickerTheme__116548.headerHeadlineStyle ?? defaults__116625.headerHeadlineStyle) : Theme.of(context).textTheme.headlineSmall);
        global::Doroti.Ui.Color? headerForegroundColor__117137 = ((global::Doroti.Ui.Color?)(object?)(datePickerTheme__116548.headerForegroundColor ?? defaults__116625.headerForegroundColor));
        headlineStyle__116934 = headlineStyle__116934?.copyWith(color: headerForegroundColor__117137);
        string dateText__117335 = ((string)(object?)_formatDateRange(context, this.selectedStartDate, this.selectedEndDate, DartRuntimePrimitives.RequireValue(this.currentDate)));
        var semanticDateText__117464 = (((this.selectedStartDate is not null) && (this.selectedEndDate is not null)) ? $"{this.calendarDelegate.formatMediumDate(DartRuntimePrimitives.RequireValue(this.selectedStartDate), localizations__116396)} – {this.calendarDelegate.formatMediumDate(DartRuntimePrimitives.RequireValue(this.selectedEndDate), localizations__116396)}" : "");
        global::Doroti.Generated.Framework.Widgets.Widget header__117724 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _DatePickerHeader__date_picker(helpText: (this.helpText ?? ((useMaterial3__116317 ? ((MaterialLocalizations)localizations__116396).dateRangePickerHelpText : ((MaterialLocalizations)localizations__116396).dateRangePickerHelpText.toUpperCase()))), titleText: dateText__117335, titleSemanticsLabel: semanticDateText__117464, titleStyle: headlineStyle__116934, orientation: orientation__116469, isShort: (object.Equals(orientation__116469, global::Doroti.Generated.Framework.Widgets.Orientation.landscape)), entryModeButton: this.entryModeButton));
        global::Doroti.Generated.Framework.Widgets.Widget actions__118193 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Generated.Framework.Rendering.BoxConstraints(minHeight: 52.0), child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 8), child: new global::Doroti.Generated.Framework.Widgets.Align(alignment: global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerEnd, child: new global::Doroti.Generated.Framework.Widgets.OverflowBar(spacing: 8, children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new TextButton(onPressed: this.onCancel, child: new global::Doroti.Generated.Framework.Widgets.Text((this.cancelText ?? ((useMaterial3__116317 ? ((MaterialLocalizations)localizations__116396).cancelButtonLabel : ((MaterialLocalizations)localizations__116396).cancelButtonLabel.toUpperCase())))))), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new TextButton(onPressed: this.onConfirm, child: new global::Doroti.Generated.Framework.Widgets.Text(((this.confirmText ?? (string)((MaterialLocalizations)localizations__116396).okButtonLabel))))) })))));
        double textScaleFactor__119079 = (MediaQuery.textScalerOf(context).clamp(maxScaleFactor: Date_pickerLibrary._kMaxRangeTextScaleFactor).scale(Calendar_date_pickerLibrary._fontSizeToScale) / Calendar_date_pickerLibrary._fontSizeToScale);
        global::Doroti.Ui.Size dialogSize__119275 = ((global::Doroti.Ui.Size)(object?)(((useMaterial3__116317 ? Date_pickerLibrary._inputPortraitDialogSizeM3 : Date_pickerLibrary._inputPortraitDialogSizeM2)) * textScaleFactor__119079));
        switch (orientation__116469)
        {
            case global::Doroti.Generated.Framework.Widgets.Orientation.portrait:
                {
                    return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.LayoutBuilder(builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Rendering.BoxConstraints, global::Doroti.Generated.Framework.Widgets.Widget>)((context, constraints) => {
global::Doroti.Ui.Size portraitDialogSize__119573 = ((global::Doroti.Ui.Size)(object?)(useMaterial3__116317 ? Date_pickerLibrary._inputPortraitDialogSizeM3 : Date_pickerLibrary._inputPortraitDialogSizeM2));
bool isFullyPortrait__119854 = (((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxHeight >= Math.Min(dialogSize__119275.height, portraitDialogSize__119573.height));
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Column(mainAxisSize: global::Doroti.Generated.Framework.Rendering.MainAxisSize.min, crossAxisAlignment: global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.stretch, children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection120213 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); if (isFullyPortrait__119854) { __collection120213.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(header__117724)); } __collection120213.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Expanded(child: this.picker))); __collection120213.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(actions__118193)); return __collection120213; }))()));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
                }
            case global::Doroti.Generated.Framework.Widgets.Orientation.landscape:
                {
                    return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Row(mainAxisSize: global::Doroti.Generated.Framework.Rendering.MainAxisSize.min, crossAxisAlignment: global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.stretch, children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(header__117724), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Flexible(child: new global::Doroti.Generated.Framework.Widgets.Column(mainAxisSize: global::Doroti.Generated.Framework.Rendering.MainAxisSize.min, crossAxisAlignment: global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.stretch, children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Expanded(child: this.picker)), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(actions__118193) }))) }));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _InputDateRangePicker__date_picker : global::Doroti.Generated.Framework.Widgets.StatefulWidget
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
    public virtual global::Doroti.Generated.Framework.Services.TextInputType keyboardType { get; private set; } = default!;
    public virtual global::System.Func<DateTime, DateTime?, DateTime?, bool>? selectableDayPredicate { get; private set; }
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    internal _InputDateRangePicker__date_picker(global::Doroti.Generated.Framework.Foundation.Key? key = null, DateTime? initialStartDate = null, DateTime? initialEndDate = null, DateTime firstDate = default!, DateTime lastDate = default!, global::System.Action<DateTime?>? onStartDateChanged = default!, global::System.Action<DateTime?>? onEndDateChanged = default!, global::System.Func<DateTime, DateTime?, DateTime?, bool>? selectableDayPredicate = default!, CalendarDelegate<DateTime> calendarDelegate = default!, string? helpText = null, string? errorFormatText = null, string? errorInvalidText = null, string? errorInvalidRangeText = null, string? fieldStartHintText = null, string? fieldEndHintText = null, string? fieldStartLabelText = null, string? fieldEndLabelText = null, bool autofocus = false, bool autovalidate = false, global::Doroti.Generated.Framework.Services.TextInputType keyboardType = default!) : base(key: key)
    {
        global::Doroti.Generated.Framework.Services.TextInputType __keyboardType = keyboardType ?? global::Doroti.Generated.Framework.Services.TextInputType.datetime;
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

internal class _InputDateRangePickerState__date_picker : global::Doroti.Generated.Framework.Widgets.State<_InputDateRangePicker__date_picker>
{
    internal virtual string _startInputText { get; set; } = default!;
    internal virtual string _endInputText { get; set; } = default!;
    internal virtual DateTime? _startDate { get; set; } = default;
    internal virtual DateTime? _endDate { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Widgets.TextEditingController _startController { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Widgets.TextEditingController _endController { get; set; } = default!;
    internal virtual string? _startErrorText { get; set; } = default;
    internal virtual string? _endErrorText { get; set; } = default;
    internal virtual bool _autoSelected { get; set; } = false;

    public override void initState()
    {
        base.initState();
        _startDate = ((_InputDateRangePicker__date_picker)this.widget).initialStartDate;
        _startController = new global::Doroti.Generated.Framework.Widgets.TextEditingController();
        _endDate = ((_InputDateRangePicker__date_picker)this.widget).initialEndDate;
        _endController = new global::Doroti.Generated.Framework.Widgets.TextEditingController();
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
        MaterialLocalizations localizations__125435 = ((MaterialLocalizations)(object?)MaterialLocalizations.of(this.context));
        if ((this._startDate is not null))
        {
            _startInputText = ((_InputDateRangePicker__date_picker)this.widget).calendarDelegate.formatCompactDate(DartRuntimePrimitives.RequireValue(this._startDate), localizations__125435);
            bool selectText__125628 = (((_InputDateRangePicker__date_picker)this.widget).autofocus && !this._autoSelected);
            _updateController(this._startController, this._startInputText, selectText__125628);
            _autoSelected = selectText__125628;
        }
        if ((this._endDate is not null))
        {
            _endInputText = ((_InputDateRangePicker__date_picker)this.widget).calendarDelegate.formatCompactDate(DartRuntimePrimitives.RequireValue(this._endDate), localizations__125435);
            _updateController(this._endController, this._endInputText, false);
        }
    }

    public virtual bool validate()
    {
        string? startError__126273 = ((string?)(object?)_validateDate(this._startDate));
        string? endError__126331 = ((string?)(object?)_validateDate(this._endDate));
        if (((startError__126273 is null) && (endError__126331 is null)))
        {
            if (DartRuntimePrimitives.RequireValue(this._startDate).isAfter(DartRuntimePrimitives.RequireValue(this._endDate)))
            {
                startError__126273 = (((_InputDateRangePicker__date_picker)this.widget).errorInvalidRangeText ?? MaterialLocalizations.of(this.context).invalidDateRangeLabel);
            }
        }
        setState(((global::System.Action)(() => {
_startErrorText = startError__126273;
_endErrorText = endError__126331;
})));
        return ((startError__126273 is null) && (endError__126331 is null));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual DateTime? _parseDate(string? text)
    {
        MaterialLocalizations localizations__126818 = ((MaterialLocalizations)(object?)MaterialLocalizations.of(this.context));
        return ((_InputDateRangePicker__date_picker)this.widget).calendarDelegate.parseCompactDate(text, localizations__126818);
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

    internal virtual void _updateController(global::Doroti.Generated.Framework.Widgets.TextEditingController controller, string text, bool selectText)
    {
        global::Doroti.Generated.Framework.Services.TextEditingValue textEditingValue__127677 = ((global::Doroti.Generated.Framework.Services.TextEditingValue)(object?)controller.value.copyWith(text: text));
        if (selectText)
        {
            textEditingValue__127677 = textEditingValue__127677.copyWith(selection: new global::Doroti.Generated.Framework.Services.TextSelection(baseOffset: 0L, extentOffset: text.Length));
        }
        controller.value = textEditingValue__127677;
    }

    internal virtual void _handleStartChanged(string text)
    {
        setState(((global::System.Action)(() => {
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
        setState(((global::System.Action)(() => {
_endInputText = text;
_endDate = _parseDate(text);
((_InputDateRangePicker__date_picker)this.widget).onEndDateChanged?.Invoke(this._endDate);
})));
        if (((_InputDateRangePicker__date_picker)this.widget).autovalidate)
        {
            validate();
        }
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ThemeData theme__128499 = Theme.of(context);
        bool useMaterial3__128541 = theme__128499.useMaterial3;
        MaterialLocalizations localizations__128608 = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        InputDecorationThemeData inputTheme__128694 = ((InputDecorationThemeData)(object?)InputDecorationTheme.of(context));
        InputBorder inputBorder__128763 = (((InputDecorationThemeData)inputTheme__128694).border ?? ((useMaterial3__128541 ? new OutlineInputBorder() : new UnderlineInputBorder())));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Row(crossAxisAlignment: global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.start, children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Expanded(child: new TextField(controller: this._startController, decoration: new InputDecoration(border: inputBorder__128763, filled: ((InputDecorationThemeData)inputTheme__128694).filled, hintText: ((((_InputDateRangePicker__date_picker)this.widget).fieldStartHintText ?? (string)((_InputDateRangePicker__date_picker)this.widget).calendarDelegate.dateHelpText(localizations__128608))), labelText: ((((_InputDateRangePicker__date_picker)this.widget).fieldStartLabelText ?? (string)((MaterialLocalizations)localizations__128608).dateRangeStartLabel)), errorText: this._startErrorText), keyboardType: ((_InputDateRangePicker__date_picker)this.widget).keyboardType, onChanged: this._handleStartChanged, autofocus: ((_InputDateRangePicker__date_picker)this.widget).autofocus))), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(width: 8)), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Expanded(child: new TextField(controller: this._endController, decoration: new InputDecoration(border: inputBorder__128763, filled: ((InputDecorationThemeData)inputTheme__128694).filled, hintText: ((((_InputDateRangePicker__date_picker)this.widget).fieldEndHintText ?? (string)((_InputDateRangePicker__date_picker)this.widget).calendarDelegate.dateHelpText(localizations__128608))), labelText: ((((_InputDateRangePicker__date_picker)this.widget).fieldEndLabelText ?? (string)((MaterialLocalizations)localizations__128608).dateRangeEndLabel)), errorText: this._endErrorText), keyboardType: ((_InputDateRangePicker__date_picker)this.widget).keyboardType, onChanged: this._handleEndChanged))) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
