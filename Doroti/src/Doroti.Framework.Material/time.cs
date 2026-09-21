// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/time.dart

using Doroti.Runtime;

namespace Doroti.Framework.Material;

public enum DayPeriod
{
    am,
    pm,
}

public class TimeOfDay : IComparable<TimeOfDay>
{
    public const long hoursPerDay = 24L;
    public const long hoursPerPeriod = 12L;
    public const long minutesPerHour = 60L;
    public virtual long hour { get; private set; } = default!;
    public virtual long minute { get; private set; } = default!;

    public TimeOfDay(long hour, long minute)
    {
        this.hour = hour;
        this.minute = minute;
    }

    public static TimeOfDay CreateFromDateTime(DateTime time)
    {
        var __instance = new TimeOfDay(hour: default!, minute: default!);
        __instance.hour = time.Hour;
        __instance.minute = time.Minute;
        return __instance;
    }

    public static TimeOfDay CreateNow()
    {
        return CreateFromDateTime(DateTime.Now);
    }

    public virtual TimeOfDay replacing(long? hour = null, long? minute = null)
    {
        DartRuntimePrimitives.Assert(() =>
            (hour is null)
            || (
                (hour >= 0L)
                && (
                    (
                        hour
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ) < hoursPerDay
                )
            )
        );
        DartRuntimePrimitives.Assert(() =>
            (minute is null)
            || (
                (minute >= 0L)
                && (
                    (
                        minute
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ) < minutesPerHour
                )
            )
        );
        return new TimeOfDay(hour: hour ?? this.hour, minute: minute ?? this.minute);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DayPeriod period => (hour < hoursPerPeriod) ? DayPeriod.am : DayPeriod.pm;
    public virtual long hourOfPeriod =>
        ((hour == 0L) || (hour == 12L)) ? 12L : (hour - periodOffset);
    public virtual long periodOffset => Equals(period, DayPeriod.am) ? 0L : hoursPerPeriod;

    public virtual string format(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        DartRuntimePrimitives.Assert(() =>
            DebugLibrary.debugCheckHasMaterialLocalizations(context)
        );
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        return localizations.formatTimeOfDay(
            this,
            alwaysUse24HourFormat: MediaQuery.alwaysUse24HourFormatOf(context)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool isBefore(TimeOfDay other) =>
        DartRuntimePrimitives.ConvertValue<bool>(compareTo(other) < 0L);

    public virtual bool isAfter(TimeOfDay other) =>
        DartRuntimePrimitives.ConvertValue<bool>(compareTo(other) > 0L);

    public virtual bool isAtSameTimeAs(TimeOfDay other) =>
        DartRuntimePrimitives.ConvertValue<bool>(compareTo(other) == 0L);

    public virtual long compareTo(TimeOfDay other)
    {
        long hourComparison = hour.CompareTo((other.hour));
        return (hourComparison == 0L) ? minute.CompareTo((other.minute)) : hourComparison;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as TimeOfDay;
        if (__other is null)
        {
            return false;
        }

        return (__other is TimeOfDay) && (__other.hour == hour) && (__other.minute == minute);
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(hour, minute));

    public override string ToString()
    {
        string addLeadingZeroIfNeeded(long value)
        {
            if (value < 10L)
            {
                return $"0{value}";
            }
            return value.ToString();
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        string hourLabel = addLeadingZeroIfNeeded((hour));
        string minuteLabel = addLeadingZeroIfNeeded((minute));
        return $"{typeof(TimeOfDay)}({hourLabel}:{minuteLabel})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public int CompareTo(TimeOfDay? other) => checked((int)compareTo(other!));
}

public class RestorableTimeOfDay : RestorableValue<TimeOfDay>
{
    internal virtual TimeOfDay _defaultValue { get; private set; } = default!;

    public RestorableTimeOfDay(TimeOfDay defaultValue)
    {
        _defaultValue = defaultValue;
    }

    public override TimeOfDay createDefaultValue() => _defaultValue;

    public override void didUpdateValue(TimeOfDay? oldValue)
    {
        DartRuntimePrimitives.Assert(() =>
            RestorationLibrary.debugIsSerializableForRestoration(value.hour)
        );
        DartRuntimePrimitives.Assert(() =>
            RestorationLibrary.debugIsSerializableForRestoration(value.minute)
        );
        notifyListeners();
    }

    public override TimeOfDay fromPrimitives(object? data)
    {
        var timeData = ((List<object?>?)data!)!;
        return new TimeOfDay(minute: (long)timeData[(int)0L]!, hour: (long)timeData[(int)1L]!);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override object? toPrimitives() => new List<long> { value.minute, value.hour };
}

public enum TimeOfDayFormat
{
    HH_colon_mm,
    HH_dot_mm,
    frenchCanadian,
    H_colon_mm,
    h_colon_mm_space_a,
    a_space_h_colon_mm,
}

public enum HourFormat
{
    HH,
    H,
    h,
}

public static partial class TimeLibrary
{
    public static HourFormat hourFormat(TimeOfDayFormat of) =>
        of switch
        {
            TimeOfDayFormat.h_colon_mm_space_a => HourFormat.h,
            TimeOfDayFormat.a_space_h_colon_mm => HourFormat.h,
            TimeOfDayFormat.H_colon_mm => HourFormat.H,
            TimeOfDayFormat.HH_dot_mm or TimeOfDayFormat.HH_colon_mm => HourFormat.HH,
            TimeOfDayFormat.frenchCanadian => HourFormat.HH,
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
}
