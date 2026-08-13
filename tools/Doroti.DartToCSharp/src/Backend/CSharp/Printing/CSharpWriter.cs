using System.Globalization;
using System.Text;

namespace Doroti.DartToCSharp;

internal sealed class CSharpWriter
{
    private readonly StringBuilder _builder;

    public CSharpWriter(int capacity = 16) => _builder = new StringBuilder(capacity);

    public int Length => _builder.Length;
    public int LineCount { get; private set; }
    public int CurrentLine => LineCount + 1;

    public CSharpWriter Append(char value)
    {
        _builder.Append(value);
        if (value == '\n')
        {
            LineCount++;
        }
        return this;
    }

    public CSharpWriter Append(string? value)
    {
        _builder.Append(value);
        if (value is not null)
        {
            LineCount += value.Count(character => character == '\n');
        }
        return this;
    }

    public CSharpWriter Append(object? value) => Append(
        value is IFormattable formattable
            ? formattable.ToString(null, CultureInfo.InvariantCulture)
            : value?.ToString());

    public CSharpWriter AppendLine()
    {
        _builder.Append('\n');
        LineCount++;
        return this;
    }

    public CSharpWriter AppendLine(string? value)
    {
        Append(value);
        return AppendLine();
    }

    public override string ToString() => _builder.ToString();
}
