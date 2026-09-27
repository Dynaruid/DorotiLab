namespace Doroti.Runtime;

/// <summary>Dart numeric operations whose semantics differ from CLR operators.</summary>
public static class DartNumeric
{
    /// <summary>Euclidean modulo, including negative divisors and Int64.MinValue.</summary>
    public static long Modulo(long value, long divisor)
    {
        // CLR remainder overflows for MinValue / -1; Dart's modulo is zero.
        if (divisor == -1)
            return 0;
        var remainder = value % divisor;
        return remainder < 0
            ? (divisor < 0 ? remainder - divisor : remainder + divisor)
            : remainder;
    }

    public static int Modulo(int value, int divisor) => (int)Modulo((long)value, divisor);

    /// <summary>Matches Dart double %, including NaN, infinities and positive zero.</summary>
    public static double Modulo(double value, double divisor)
    {
        var remainder = value % divisor;
        if (remainder == 0)
            return 0.0;
        return remainder < 0 ? remainder + Math.Abs(divisor) : remainder;
    }

    /// <summary>Preserves the operand's numeric kind for dynamically typed Dart numbers.</summary>
    public static dynamic Modulo(object value, object divisor) =>
        (value, divisor) switch
        {
            (long a, long b) => (object)Modulo(a, b),
            (int a, int b) => (object)Modulo(a, b),
            (long a, int b) => (object)Modulo(a, b),
            (int a, long b) => (object)Modulo(a, b),
            (double a, double b) => Modulo(a, b),
            (double a, long b) => Modulo(a, b),
            (double a, int b) => Modulo(a, b),
            (long a, double b) => Modulo(a, b),
            (int a, double b) => Modulo(a, b),
            _ => throw new ArgumentException("Modulo requires Dart int or double operands."),
        };
}
