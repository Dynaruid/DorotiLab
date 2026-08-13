namespace Doroti.DartToCSharp;

internal readonly record struct LibraryId
{
    public LibraryId(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Any(char.IsWhiteSpace))
        {
            throw new ArgumentException("Library identity must be a non-empty URI.", nameof(value));
        }
        Value = value;
    }

    public string Value { get; }
    public override string ToString() => Value;
}

internal readonly record struct SymbolId
{
    private SymbolId(string value) => Value = value;

    public string Value { get; }

    public static SymbolId Parse(string value)
    {
        var separator = value.LastIndexOf('#');
        if (string.IsNullOrWhiteSpace(value) || separator <= 0 || separator == value.Length - 1)
        {
            throw new FormatException($"Canonical symbol identity must contain a library URI and name: {value}");
        }
        _ = new LibraryId(value[..separator]);
        return new(value);
    }

    public static SymbolId TypeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Type name cannot be empty.", nameof(name));
        }
        return new($"type:{name}#{name}");
    }

    public LibraryId Library => new(Value[..Value.LastIndexOf('#')]);
    public string Name => Value[(Value.LastIndexOf('#') + 1)..];
    public override string ToString() => Value;
}

internal enum SymbolKind
{
    Library,
    Class,
    Mixin,
    Enum,
    Extension,
    TypeAlias,
    Constructor,
    Method,
    Getter,
    Setter,
    Operator,
    Field,
    Function,
    Parameter,
    TypeParameter,
    Local,
}

internal enum Nullability
{
    NonNullable,
    Nullable,
    Legacy,
}
