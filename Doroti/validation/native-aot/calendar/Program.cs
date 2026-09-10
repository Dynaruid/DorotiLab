using Doroti.Framework.Material;

if (args.Contains("--native") && System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported)
    throw new InvalidOperationException("Expected NativeAOT.");
CalendarDelegate<DateTime> gregorian = new GregorianCalendarDelegate();
var leap = new DateTime(2024, 2, 29, 12, 30, 0);
Require(gregorian.isSameDay(leap, leap.Date) && gregorian.isSameMonth(leap, new DateTime(2024, 2, 1)), "Gregorian date components ignore time");
Require(!gregorian.isSameDay(leap, new DateTime(2024, 3, 1)) && !gregorian.isSameMonth(leap, new DateTime(2025, 2, 28)), "different day/year differ");
Require(!gregorian.isSameDay(null, null) && !gregorian.isSameMonth(null, null) && !gregorian.isSameDay(leap, null), "preserve current Doroti null-date behavior");
Require(gregorian.addDaysToDate(leap, 1) == new DateTime(2024, 3, 1), "leap-day arithmetic");
CalendarDelegate<ExternalDate> custom = new ExternalCalendar();
var day = new ExternalDate(1445, 9, 17, "morning");
Require(custom.isSameDay(day, day with { Annotation = "evening" }), "external date adapter controls components, not raw struct equality");
Require(custom.isSameMonth(day, day with { Date = 18 }) && !custom.isSameDay(day, day with { Date = 18 }), "custom month/day comparison");
Require(!custom.isSameMonth(day, day with { EraYear = 1446 }) && !custom.isSameDay(null, null), "custom year and null contract");
var range = custom.datesOnly(new DateTimeRange<ExternalDate>(day, day with { Date = 18 }));
Require(range.start.Annotation == "" && range.end.Annotation == "", "range uses external normalization");
Require(range.duration.inDays == 1, "external range arithmetic");
Require(new DateTimeRange<DateTime>(leap, leap.AddHours(36)).duration.inHours == 36, "Gregorian range keeps time component");
Console.WriteLine("NativeAOT calendar contract: Gregorian leap/time/null behavior and external date representation/range normalization PASS");

static void Require(bool value, string message) { if (!value) throw new Exception(message); }
readonly record struct ExternalDate(long EraYear, long Period, long Date, string Annotation) : ICalendarDate<ExternalDate>, IComparable<ExternalDate>
{
    long ICalendarDate<ExternalDate>.Year => EraYear;
    long ICalendarDate<ExternalDate>.Month => Period;
    long ICalendarDate<ExternalDate>.Day => Date;
    public Doroti.Runtime.Duration difference(ExternalDate earlier) => Doroti.Runtime.Duration.Create(days: (EraYear-earlier.EraYear)*360+(Period-earlier.Period)*30+Date-earlier.Date);
    public int CompareTo(ExternalDate other) => (EraYear, Period, Date).CompareTo((other.EraYear, other.Period, other.Date));
}
sealed class ExternalCalendar : CalendarDelegate<ExternalDate>
{
    public override ExternalDate now() => new(1445, 9, 17, "now");
    public override ExternalDate dateOnly(ExternalDate date) => date with { Annotation = "" };
    public override long monthDelta(ExternalDate a, ExternalDate b) => (b.EraYear-a.EraYear)*12+b.Period-a.Period;
    public override ExternalDate addMonthsToMonthDate(ExternalDate date, long months) => date with { Period = date.Period + months, Date = 1 };
    public override ExternalDate addDaysToDate(ExternalDate date, long days) => date with { Date = date.Date + days };
    public override long firstDayOffset(long year, long month, MaterialLocalizations localizations) => 0;
    public override long getDaysInMonth(long year, long month) => 30;
    public override ExternalDate getMonth(long year, long month) => new(year, month, 1, "");
    public override ExternalDate getDay(long year, long month, long day) => new(year, month, day, "");
    public override string formatMonthYear(ExternalDate date, MaterialLocalizations localizations) => $"{date.EraYear}/{date.Period}";
    public override string formatMediumDate(ExternalDate date, MaterialLocalizations localizations) => formatShortDate(date, localizations);
    public override string formatShortMonthDay(ExternalDate date, MaterialLocalizations localizations) => $"{date.Period}/{date.Date}";
    public override string formatShortDate(ExternalDate date, MaterialLocalizations localizations) => $"{date.EraYear}/{date.Period}/{date.Date}";
    public override string formatFullDate(ExternalDate date, MaterialLocalizations localizations) => formatShortDate(date, localizations);
    public override string formatCompactDate(ExternalDate date, MaterialLocalizations localizations) => formatShortDate(date, localizations);
    public override ExternalDate? parseCompactDate(string? input, MaterialLocalizations localizations) => null;
    public override string dateHelpText(MaterialLocalizations localizations) => "year/month/day";
}
