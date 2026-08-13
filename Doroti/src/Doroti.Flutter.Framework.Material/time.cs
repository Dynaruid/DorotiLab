// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/time.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public enum DayPeriod
{
    am,
    pm
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
        return TimeOfDay.CreateFromDateTime(new DateTime());
    }

    public virtual TimeOfDay replacing(long? hour = null, long? minute = null)
    {
        DartRuntimePrimitives.Assert(() => ((hour is null) || (((hour >= 0L) && (DartRuntimePrimitives.RequireValue(hour) < hoursPerDay)))));
        DartRuntimePrimitives.Assert(() => ((minute is null) || (((minute >= 0L) && (DartRuntimePrimitives.RequireValue(minute) < minutesPerHour)))));
        return new TimeOfDay(hour: (hour ?? this.hour), minute: (minute ?? this.minute));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DayPeriod period => ((this.hour < hoursPerPeriod) ? DayPeriod.am : DayPeriod.pm);
    public virtual long hourOfPeriod => (((this.hour == 0L) || (this.hour == 12L)) ? 12L : (this.hour - this.periodOffset));
    public virtual long periodOffset => ((object.Equals(this.period, DayPeriod.am)) ? 0L : hoursPerPeriod);
    public virtual string format(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        MaterialLocalizations localizations__3846 = MaterialLocalizations.of(context);
        return localizations__3846.formatTimeOfDay(this, alwaysUse24HourFormat: MediaQuery.alwaysUse24HourFormatOf(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool isBefore(TimeOfDay other) => DartRuntimePrimitives.ConvertValue<bool>((compareTo(other) < 0L));
    public virtual bool isAfter(TimeOfDay other) => DartRuntimePrimitives.ConvertValue<bool>((compareTo(other) > 0L));
    public virtual bool isAtSameTimeAs(TimeOfDay other) => DartRuntimePrimitives.ConvertValue<bool>((compareTo(other) == 0L));
    public virtual long compareTo(TimeOfDay other)
    {
        long hourComparison__5370 = this.hour.CompareTo(DartRuntimePrimitives.RequireValue(((TimeOfDay)other).hour));
        return ((hourComparison__5370 == 0L) ? this.minute.CompareTo(DartRuntimePrimitives.RequireValue(((TimeOfDay)other).minute)) : hourComparison__5370);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as TimeOfDay;
        if (__other is null) return false;
        return (((__other is TimeOfDay) && (((TimeOfDay)((TimeOfDay)__other)).hour == this.hour)) && (((TimeOfDay)((TimeOfDay)__other)).minute == this.minute));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.hour, this.minute));
    public override string ToString()
    {
        string addLeadingZeroIfNeeded(long value)
        {
            if ((value < 10L))
            {
                return $"0{value}";
            }
            return ((string)(object?)value.ToString());
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        string hourLabel__5889 = addLeadingZeroIfNeeded(DartRuntimePrimitives.RequireValue(this.hour));
        string minuteLabel__5948 = addLeadingZeroIfNeeded(DartRuntimePrimitives.RequireValue(this.minute));
        return $"{typeof(TimeOfDay)}({hourLabel__5889}:{minuteLabel__5948})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public int CompareTo(TimeOfDay? other) => checked((int)compareTo(other!));
}

public class RestorableTimeOfDay : global::Doroti.Generated.Framework.Widgets.RestorableValue<TimeOfDay>
{
    internal virtual TimeOfDay _defaultValue { get; private set; } = default!;

    public RestorableTimeOfDay(TimeOfDay defaultValue)
    {
        this._defaultValue = defaultValue;
    }

    public override TimeOfDay createDefaultValue() => this._defaultValue;
    public override void didUpdateValue(TimeOfDay oldValue)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Services.RestorationLibrary.debugIsSerializableForRestoration(((TimeOfDay)this.value).hour));
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Services.RestorationLibrary.debugIsSerializableForRestoration(((TimeOfDay)this.value).minute));
        notifyListeners();
    }

    public override TimeOfDay fromPrimitives(object? data)
    {
        var timeData__6784 = ((List<object?>?)(object?)data!)!;
        return new TimeOfDay(minute: ((long)timeData__6784[(int)(0L)]!), hour: ((long)timeData__6784[(int)(1L)]!));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override object? toPrimitives() => new List<long> { ((TimeOfDay)this.value).minute, ((TimeOfDay)this.value).hour };
}

public enum TimeOfDayFormat
{
    HH_colon_mm,
    HH_dot_mm,
    frenchCanadian,
    H_colon_mm,
    h_colon_mm_space_a,
    a_space_h_colon_mm
}

public enum HourFormat
{
    HH,
    H,
    h
}

public static partial class TimeLibrary
{
    public static HourFormat hourFormat(TimeOfDayFormat of) => (of switch { TimeOfDayFormat.h_colon_mm_space_a => HourFormat.h, TimeOfDayFormat.a_space_h_colon_mm => HourFormat.h, TimeOfDayFormat.H_colon_mm => HourFormat.H, TimeOfDayFormat.HH_dot_mm or TimeOfDayFormat.HH_colon_mm => HourFormat.HH, TimeOfDayFormat.frenchCanadian => HourFormat.HH, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
}
