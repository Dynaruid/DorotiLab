// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/date.dart
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

/// <summary>Static date components and arithmetic for application-defined calendar values.</summary>
public interface ICalendarDate<T> where T : struct
{
    long Year { get; }
    long Month { get; }
    long Day { get; }
    Duration difference(T earlier);
}

public abstract class CalendarDelegate<T> where T : struct
{
    protected CalendarDelegate()
    {
    }

    /// <summary>Override for a custom date representation. Built-in DateTime delegates need no adapter.</summary>
    protected virtual (long Year, long Month, long Day) getDateParts(T date)
    {
        if (date is DateTime value)
            return (value.Year, value.Month, value.Day);
        if (date is ICalendarDate<T> custom)
            return (custom.Year, custom.Month, custom.Day);
        throw new NotSupportedException($"CalendarDelegate<{typeof(T).Name}> must override getDateParts for its date representation.");
    }

    public abstract T now();
    public abstract T dateOnly(T date);
    public virtual DateTimeRange<T> datesOnly(DateTimeRange<T> range)
    {
        return new DateTimeRange<T>(start: dateOnly(((DateTimeRange<T>)range).start), end: dateOnly(((DateTimeRange<T>)range).end));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool isSameDay(T? dateA, T? dateB)
    {
        if (!dateA.HasValue || !dateB.HasValue) return false;
        return getDateParts(dateA.Value) == getDateParts(dateB.Value);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool isSameMonth(T? dateA, T? dateB)
    {
        if (!dateA.HasValue || !dateB.HasValue) return false;
        var a = getDateParts(dateA.Value);
        var b = getDateParts(dateB.Value);
        return a.Year == b.Year && a.Month == b.Month;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract long monthDelta(T startDate, T endDate);
    public abstract T addMonthsToMonthDate(T monthDate, long monthsToAdd);
    public abstract T addDaysToDate(T date, long days);
    public abstract long firstDayOffset(long year, long month, MaterialLocalizations localizations);
    public abstract long getDaysInMonth(long year, long month);
    public abstract T getMonth(long year, long month);
    public abstract T getDay(long year, long month, long day);
    public abstract string formatMonthYear(T date, MaterialLocalizations localizations);
    public virtual string formatYear(long year, MaterialLocalizations localizations)
    {
        return ((string)(object?)localizations.formatYear(DartRuntimePrimitives.CreateDateTime(year)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract string formatMediumDate(T date, MaterialLocalizations localizations);
    public abstract string formatShortMonthDay(T date, MaterialLocalizations localizations);
    public abstract string formatShortDate(T date, MaterialLocalizations localizations);
    public abstract string formatFullDate(T date, MaterialLocalizations localizations);
    public abstract string formatCompactDate(T date, MaterialLocalizations localizations);
    public abstract T? parseCompactDate(string? inputString, MaterialLocalizations localizations);
    public abstract string dateHelpText(MaterialLocalizations localizations);
}

public class GregorianCalendarDelegate : CalendarDelegate<DateTime>
{
    public GregorianCalendarDelegate()
    {
    }

    public override DateTime now() => DateTime.Now;
    public override DateTime dateOnly(DateTime date) => DateUtils.dateOnly(date);
    public override long monthDelta(DateTime startDate, DateTime endDate) => DateUtils.monthDelta(startDate, endDate);
    public override DateTime addMonthsToMonthDate(DateTime monthDate, long monthsToAdd)
    {
        return DateUtils.addMonthsToMonthDate(monthDate, monthsToAdd);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override DateTime addDaysToDate(DateTime date, long days) => DateUtils.addDaysToDate(date, days);
    public override long firstDayOffset(long year, long month, MaterialLocalizations localizations)
    {
        return DateUtils.firstDayOffset(year, month, localizations);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long getDaysInMonth(long year, long month) => DateUtils.getDaysInMonth(year, month);
    public override DateTime getMonth(long year, long month) => DartRuntimePrimitives.CreateDateTime(year, month);
    public override DateTime getDay(long year, long month, long day) => DartRuntimePrimitives.CreateDateTime(year, month, day);
    public override string formatMonthYear(DateTime date, MaterialLocalizations localizations)
    {
        return ((string)(object?)localizations.formatMonthYear(date));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string formatMediumDate(DateTime date, MaterialLocalizations localizations)
    {
        return ((string)(object?)localizations.formatMediumDate(date));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string formatShortMonthDay(DateTime date, MaterialLocalizations localizations)
    {
        return ((string)(object?)localizations.formatShortMonthDay(date));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string formatShortDate(DateTime date, MaterialLocalizations localizations)
    {
        return ((string)(object?)localizations.formatShortDate(date));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string formatFullDate(DateTime date, MaterialLocalizations localizations)
    {
        return ((string)(object?)localizations.formatFullDate(date));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string formatCompactDate(DateTime date, MaterialLocalizations localizations)
    {
        return ((string)(object?)localizations.formatCompactDate(date));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override DateTime? parseCompactDate(string? inputString, MaterialLocalizations localizations)
    {
        return localizations.parseCompactDate(inputString);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string dateHelpText(MaterialLocalizations localizations)
    {
        return ((MaterialLocalizations)localizations).dateHelpText;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class DateUtils
{
    public static DateTime dateOnly(DateTime date)
    {
        return DartRuntimePrimitives.CreateDateTime(date.Year, date.Month, date.Day);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DateTimeRange<DateTime> datesOnly(DateTimeRange<DateTime> range)
    {
        return new DateTimeRange<DateTime>(start: DateUtils.dateOnly(((DateTimeRange<DateTime>)range).start), end: DateUtils.dateOnly(((DateTimeRange<DateTime>)range).end));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool isSameDay(DateTime? dateA, DateTime? dateB)
    {
        return (((dateA?.Year == dateB?.Year) && (dateA?.Month == dateB?.Month)) && (dateA?.Day == dateB?.Day));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool isSameMonth(DateTime? dateA, DateTime? dateB)
    {
        return ((dateA?.Year == dateB?.Year) && (dateA?.Month == dateB?.Month));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static long monthDelta(DateTime startDate, DateTime endDate)
    {
        return (((((endDate.Year - startDate.Year)) * 12L) + endDate.Month) - startDate.Month);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DateTime addMonthsToMonthDate(DateTime monthDate, long monthsToAdd)
    {
        return DartRuntimePrimitives.CreateDateTime(monthDate.Year, (monthDate.Month + monthsToAdd));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DateTime addDaysToDate(DateTime date, long days)
    {
        return DartRuntimePrimitives.CreateDateTime(date.Year, date.Month, (date.Day + days));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static long firstDayOffset(long year, long month, MaterialLocalizations localizations)
    {
        long weekdayFromMonday = (DartRuntimePrimitives.CreateDateTime(year, month).DayOfWeek.ToDartWeekday() - 1L);
        long firstDayOfWeekIndexLocal = ((MaterialLocalizations)localizations).firstDayOfWeekIndex;
        firstDayOfWeekIndexLocal = (firstDayOfWeekIndexLocal + 6) % 7;
        return (weekdayFromMonday - firstDayOfWeekIndexLocal + 7) % 7;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static long getDaysInMonth(long year, long month)
    {
        if ((month == 2L))
        {
            bool isLeapYear = (((((year % 4L) == 0L)) && (((year % 100L) != 0L))) || (((year % 400L) == 0L)));
            return (isLeapYear ? 29L : 28L);
        }
        var daysInMonth = new List<long> { 31L, -1L, 31L, 30L, 31L, 30L, 31L, 31L, 30L, 31L, 30L, 31L };
        return daysInMonth[(int)((month - 1L))];
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public enum DatePickerEntryMode
{
    calendar,
    input,
    calendarOnly,
    inputOnly
}

public enum DatePickerMode
{
    day,
    year
}

public class DateTimeRange<T> where T : struct
{
    public virtual T start { get; private set; } = default!;
    public virtual T end { get; private set; } = default!;

    public DateTimeRange(T start, T end)
    {
        this.start = start;
        this.end = end;
        System.Diagnostics.Debug.Assert(Comparer<T>.Default.Compare(start, end) <= 0);
    }

    public virtual Duration duration => this.end is DateTime endDate && this.start is DateTime startDate
        ? (Duration)(endDate - startDate)
        : this.end is ICalendarDate<T> custom
            ? custom.difference(this.start)
            : throw new NotSupportedException($"DateTimeRange<{typeof(T).Name}> requires ICalendarDate<{typeof(T).Name}> or an overridden duration.");
    public override bool Equals(object? other)
    {
        var __other = other as DateTimeRange<T>;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((__other is DateTimeRange<T>) && object.Equals(((DateTimeRange<T>)__other).start, this.start)) && object.Equals(((DateTimeRange<T>)__other).end, this.end));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.start, this.end));
    public override string ToString() => $"{this.start} - {this.end}";
}
