using Doroti.FlutterCompat;

namespace Doroti.Generated.SharedPreferences;

public sealed class InMemorySharedPreferencesPlatformPort : ISharedPreferencesPlatformPort
{
    private readonly Dictionary<string, object?> _values = new(StringComparer.Ordinal);

    public Future<object?> InvokeAsync(string operation, object? arguments = null)
    {
        return operation switch
        {
            "set" when arguments is KeyValuePair<string, object?> pair => Store(pair),
            "get" when arguments is string key => Future<object?>.value(_values.GetValueOrDefault(key)),
            "remove" when arguments is string key => Future<object?>.value(_values.Remove(key)),
            _ => Future<object?>.error(new NotSupportedException($"Unsupported shared-preferences operation: {operation}")),
        };
    }

    private Future<object?> Store(KeyValuePair<string, object?> pair)
    {
        _values[pair.Key] = pair.Value;
        return Future<object?>.value(true);
    }
}
