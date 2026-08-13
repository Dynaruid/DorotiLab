using System.Collections;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Doroti.Flutter.Runtime;

public static class Dart_ioLibrary
{
    public static GZipCodec gzip { get; } = new();
}

public sealed class GZipCodec
{
    public List<long> decode(Uint8List bytes) => decode(bytes.ToList());

    public List<long> decode(List<long> bytes)
    {
        using var input = new MemoryStream(bytes.Select(value => checked((byte)value)).ToArray());
        using var gzip = new System.IO.Compression.GZipStream(input, System.IO.Compression.CompressionMode.Decompress);
        using var output = new MemoryStream();
        gzip.CopyTo(output);
        return output.ToArray().Select(value => (long)value).ToList();
    }
}

public interface IDartMap
{
    bool TryGetValueObject(object? key, out object? value);
}

public static class DartPatternRuntime
{
    public static bool IsMap(object? value) => value is IDartMap or IDictionary;

    public static bool TryGetMapValue(object? map, object? key, out object? value)
    {
        if (map is IDartMap dartMap) return dartMap.TryGetValueObject(key, out value);
        if (map is IDictionary dictionary && dictionary.Contains(key!))
        {
            value = dictionary[key!];
            return true;
        }
        value = null;
        return false;
    }
}

public static class DartCollectionRuntime
{
    public static TValue? NullableMapValue<TValue>(object map, object? key)
        where TValue : struct
    {
        return DartPatternRuntime.TryGetMapValue(map, key, out var value) && value is TValue typed
            ? typed
            : null;
    }

    public static TKey? FirstKeyOrNull<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> values)
        where TKey : struct
    {
        ArgumentNullException.ThrowIfNull(values);
        return values.Count == 0 ? null : values.Keys.First();
    }

    public static TKey? LastKeyOrNull<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> values)
        where TKey : struct
    {
        ArgumentNullException.ThrowIfNull(values);
        return values.Count == 0 ? null : values.Keys.Last();
    }
}

/// <summary>A Dart Map that permits null keys while retaining insertion order.</summary>
public sealed class DartMap<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, IDictionary, IDartMap
{
    private readonly List<KeyValuePair<TKey, TValue>> _items = [];
    public DartMap() { }
    public DartMap(IEnumerable<KeyValuePair<TKey, TValue>> values) { foreach (var pair in values) this[pair.Key] = pair.Value; }
    public DartMap(IEnumerable<TKey> keys, IEnumerable<TValue> values)
    {
        ArgumentNullException.ThrowIfNull(keys);
        ArgumentNullException.ThrowIfNull(values);
        using var keyEnumerator = keys.GetEnumerator();
        using var valueEnumerator = values.GetEnumerator();
        while (true)
        {
            var hasKey = keyEnumerator.MoveNext();
            var hasValue = valueEnumerator.MoveNext();
            if (hasKey != hasValue)
            {
                throw new ArgumentException("Dart Map.fromIterables requires equal key and value counts.");
            }
            if (!hasKey) break;
            this[keyEnumerator.Current] = valueEnumerator.Current;
        }
    }
    public TValue this[TKey key]
    {
        get { var index = Find(key); return index >= 0 ? _items[index].Value : throw new KeyNotFoundException(); }
        set { var index = Find(key); if (index >= 0) _items[index] = new(key, value); else _items.Add(new(key, value)); }
    }
    public ICollection<TKey> Keys => _items.Select(pair => pair.Key).ToArray();
    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;
    public ICollection<TValue> Values => _items.Select(pair => pair.Value).ToArray();
    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;
    public int Count => _items.Count;
    public IEnumerable<MapEntry<TKey, TValue>> entries => _items.Select(pair => new MapEntry<TKey, TValue>(pair.Key, pair.Value));
    public bool IsReadOnly => false;
    public void Add(TKey key, TValue value) { if (ContainsKey(key)) throw new ArgumentException("The key already exists."); _items.Add(new(key, value)); }
    public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);
    public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> values)
    {
        foreach (var pair in values) this[pair.Key] = pair.Value;
    }
    public void addEntries(IEnumerable<MapEntry<TKey, TValue>> values)
    {
        foreach (var entry in values) this[entry.key] = entry.value;
    }
    public bool ContainsKey(TKey key) => Find(key) >= 0;
    public bool containsValue(TValue value) => _items.Any(pair => EqualityComparer<TValue>.Default.Equals(pair.Value, value));
    public bool Remove(TKey key) { var index = Find(key); if (index < 0) return false; _items.RemoveAt(index); return true; }
    public TValue? remove(TKey key)
    {
        var index = Find(key);
        if (index < 0) return default;
        var value = _items[index].Value;
        _items.RemoveAt(index);
        return value;
    }
    public bool TryGetValue(TKey key, out TValue value) { var index = Find(key); if (index >= 0) { value = _items[index].Value; return true; } value = default!; return false; }
    public TValue? GetValueOrDefault(object? key)
    {
        if (key is TKey typedKey) return TryGetValue(typedKey, out var value) ? value : default;
        return default;
    }
    bool IDartMap.TryGetValueObject(object? key, out object? value)
    {
        if ((key is TKey || key is null && default(TKey) is null) && TryGetValue((TKey)key!, out var typedValue))
        {
            value = typedValue;
            return true;
        }
        value = null;
        return false;
    }
    public void Clear() => _items.Clear();
    public bool Contains(KeyValuePair<TKey, TValue> item) => Find(item.Key) is var index && index >= 0 && EqualityComparer<TValue>.Default.Equals(_items[index].Value, item.Value);
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);
    public bool Remove(KeyValuePair<TKey, TValue> item) { if (!Contains(item)) return false; return Remove(item.Key); }
    public TValue putIfAbsent(TKey key, Func<TValue> ifAbsent)
    {
        ArgumentNullException.ThrowIfNull(ifAbsent);
        if (TryGetValue(key, out var value)) return value;
        value = ifAbsent();
        this[key] = value;
        return value;
    }

    public TValue update(TKey key, Func<TValue, TValue> update, Func<TValue>? ifAbsent = null)
    {
        ArgumentNullException.ThrowIfNull(update);
        if (TryGetValue(key, out var value))
        {
            value = update(value);
        }
        else
        {
            value = ifAbsent is null ? throw new KeyNotFoundException() : ifAbsent();
        }
        this[key] = value;
        return value;
    }

    public void removeWhere(Func<TKey, TValue, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        foreach (var key in _items.Where(pair => predicate(pair.Key, pair.Value)).Select(pair => pair.Key).ToArray())
        {
            Remove(key);
        }
    }
    public DartMap<TNewKey, TNewValue> cast<TNewKey, TNewValue>()
    {
        var result = new DartMap<TNewKey, TNewValue>();
        foreach (var pair in _items) result.Add((TNewKey)(object?)pair.Key!, (TNewValue)(object?)pair.Value!);
        return result;
    }
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    object? IDictionary.this[object key]
    {
        get => key is TKey typedKey && TryGetValue(typedKey, out var value) ? value : null;
        set => this[(TKey)key] = (TValue)value!;
    }
    ICollection IDictionary.Keys => _items.Select(pair => (object?)pair.Key).ToArray();
    ICollection IDictionary.Values => _items.Select(pair => (object?)pair.Value).ToArray();
    bool IDictionary.IsFixedSize => false;
    bool IDictionary.IsReadOnly => false;
    void IDictionary.Add(object key, object? value) => Add((TKey)key, (TValue)value!);
    bool IDictionary.Contains(object key) => key is TKey typedKey && ContainsKey(typedKey);
    IDictionaryEnumerator IDictionary.GetEnumerator() => new NonGenericEnumerator(_items);
    void IDictionary.Remove(object key) { if (key is TKey typedKey) Remove(typedKey); }
    bool ICollection.IsSynchronized => false;
    object ICollection.SyncRoot => this;
    void ICollection.CopyTo(Array array, int index)
    {
        foreach (var pair in _items)
        {
            array.SetValue(new DictionaryEntry(pair.Key!, pair.Value), index++);
        }
    }
    private int Find(TKey key) => _items.FindIndex(pair => EqualityComparer<TKey>.Default.Equals(pair.Key, key));

    private sealed class NonGenericEnumerator(List<KeyValuePair<TKey, TValue>> items) : IDictionaryEnumerator
    {
        private int _index = -1;
        public DictionaryEntry Entry => new(Key, Value);
        public object Key => items[_index].Key!;
        public object? Value => items[_index].Value;
        public object Current => Entry;
        public bool MoveNext() => ++_index < items.Count;
        public void Reset() => _index = -1;
    }
}

public readonly record struct MapEntry<TKey, TValue>(TKey key, TValue value);

public sealed class MapEquality<TKey, TValue> where TKey : notnull
{
    public bool equals(IReadOnlyDictionary<TKey, TValue>? left, IReadOnlyDictionary<TKey, TValue>? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null || left.Count != right.Count) return false;
        return left.All(pair => right.TryGetValue(pair.Key, out var value) && EqualityComparer<TValue>.Default.Equals(pair.Value, value));
    }

    public int hash(IReadOnlyDictionary<TKey, TValue>? values)
    {
        if (values is null) return 0;
        var hash = new HashCode();
        foreach (var pair in values.OrderBy(pair => pair.Key, Comparer<TKey>.Default))
        {
            hash.Add(pair.Key);
            hash.Add(pair.Value);
        }
        return hash.ToHashCode();
    }
}

public sealed class DartUri
{
    private readonly Uri _value;
    public DartUri(
        string? path = null,
        IReadOnlyDictionary<string, List<string>>? queryParameters = null,
        string? fragment = null)
    {
        var value = path ?? string.Empty;
        if (queryParameters is { Count: > 0 })
        {
            value += "?" + string.Join("&", queryParameters.SelectMany(pair => pair.Value.Select(item =>
                $"{Uri.EscapeDataString(pair.Key)}={Uri.EscapeDataString(item)}")));
        }
        if (!string.IsNullOrEmpty(fragment)) value += "#" + Uri.EscapeDataString(fragment);
        _value = new Uri(value, UriKind.RelativeOrAbsolute);
    }
    private DartUri(Uri value) => _value = value;
    public static DartUri parse(string value) => new(new Uri(value, UriKind.RelativeOrAbsolute));
    public DartUri replace(IReadOnlyDictionary<string, string>? queryParameters = null, string? fragment = null)
    {
        var basePath = _value.IsAbsoluteUri ? _value.GetLeftPart(UriPartial.Path) : path;
        var query = queryParameters?.ToDictionary(
            pair => pair.Key,
            pair => new List<string> { pair.Value },
            StringComparer.Ordinal);
        return new DartUri(basePath, query, fragment ?? this.fragment);
    }
    public static DartUri @base { get; } = new(new Uri(AppContext.BaseDirectory, UriKind.Absolute));
    public static string encodeFull(string value) => Uri.EscapeDataString(value).Replace("%2F", "/", StringComparison.OrdinalIgnoreCase);
    public string path
    {
        get
        {
            var value = _value.IsAbsoluteUri ? _value.AbsolutePath : _value.OriginalString;
            var end = value.IndexOfAny(['?', '#']);
            return end < 0 ? value : value[..end];
        }
    }
    public string fragment
    {
        get
        {
            if (_value.IsAbsoluteUri) return Uri.UnescapeDataString(_value.Fragment.TrimStart('#'));
            var marker = _value.OriginalString.IndexOf('#');
            return marker < 0 ? string.Empty : Uri.UnescapeDataString(_value.OriginalString[(marker + 1)..]);
        }
    }
    public DartMap<string, List<string>> queryParametersAll
    {
        get
        {
            var raw = _value.IsAbsoluteUri ? _value.Query.TrimStart('?') : RelativeQuery(_value.OriginalString);
            var result = new DartMap<string, List<string>>();
            if (raw.Length == 0) return result;
            foreach (var component in raw.Split('&', StringSplitOptions.RemoveEmptyEntries))
            {
                var separator = component.IndexOf('=');
                var key = Uri.UnescapeDataString(separator < 0 ? component : component[..separator]);
                var value = Uri.UnescapeDataString(separator < 0 ? string.Empty : component[(separator + 1)..]);
                if (!result.TryGetValue(key, out var values)) result[key] = values = [];
                values.Add(value);
            }
            return result;
        }
    }
    public DartUri resolve(string reference) => new(new Uri(_value, reference));
    public override string ToString() => _value.ToString();

    private static string RelativeQuery(string value)
    {
        var query = value.IndexOf('?');
        if (query < 0) return string.Empty;
        var fragment = value.IndexOf('#', query + 1);
        return fragment < 0 ? value[(query + 1)..] : value[(query + 1)..fragment];
    }
}

public static class HttpStatus { public const long ok = 200; }

public sealed class HttpClient
{
    private readonly System.Net.Http.HttpClient _client = new();
    public bool autoUncompress { get; set; }
    public Future<HttpClientRequest> getUrl(DartUri uri) => Future<HttpClientRequest>.value(new(_client, uri));
}

public sealed class HttpClientRequest(System.Net.Http.HttpClient client, DartUri uri)
{
    public DartHttpHeaders headers { get; } = new();
    public Future<HttpClientResponse> close() => Future<HttpClientResponse>.fromTask(SendAsync());
    private async Task<HttpClientResponse> SendAsync() => new(await client.GetAsync(uri.ToString(), System.Net.Http.HttpCompletionOption.ResponseHeadersRead));
}

public sealed class DartHttpHeaders
{
    private readonly Dictionary<string, List<string>> _values = new(StringComparer.OrdinalIgnoreCase);
    public void add(string name, object value) => (_values.TryGetValue(name, out var values) ? values : _values[name] = []).Add(value.ToString() ?? string.Empty);
}

public sealed class HttpClientResponse : IAsyncEnumerable<ReadOnlyMemory<byte>>
{
    private readonly System.Net.Http.HttpResponseMessage _response;
    public HttpClientResponse(System.Net.Http.HttpResponseMessage response) => _response = response;
    public long statusCode => (long)_response.StatusCode;
    public Future<T> drain<T>(T futureValue) => Future<T>.fromTask(DrainAsync(futureValue));
    private async Task<T> DrainAsync<T>(T value) { await _response.Content.LoadIntoBufferAsync(); return value; }
    public async IAsyncEnumerator<ReadOnlyMemory<byte>> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        await using var stream = await _response.Content.ReadAsStreamAsync(cancellationToken);
        var buffer = new byte[8192];
        while (await stream.ReadAsync(buffer, cancellationToken) is var count && count > 0)
            yield return buffer.AsMemory(0, count).ToArray();
    }
}

public class DartMatch(long start, long end, System.Text.RegularExpressions.Match? match = null)
{
    public long start { get; } = start;
    public long end { get; } = end;
    public long groupCount => Math.Max(0, (match?.Groups.Count ?? 1) - 1);
    public string? group(long group) => match is null
        ? group == 0 ? null : null
        : group >= 0 && group < match.Groups.Count && match.Groups[(int)group].Success
            ? match.Groups[(int)group].Value
            : null;
}

public abstract class Pattern
{
    public abstract IEnumerable<DartMatch> allMatches(string input);
    public virtual DartMatch? matchAsPrefix(string input, long start = 0) =>
        allMatches(input).FirstOrDefault(match => match.start == start);
    public static implicit operator Pattern(string value) => new LiteralPattern(value);
    private sealed class LiteralPattern(string value) : Pattern
    {
        public override IEnumerable<DartMatch> allMatches(string input)
        {
            ArgumentNullException.ThrowIfNull(input);
            if (value.Length == 0) return Enumerable.Range(0, input.Length + 1).Select(index => new DartMatch(index, index));
            var matches = new List<DartMatch>();
            for (var start = 0; (start = input.IndexOf(value, start, StringComparison.Ordinal)) >= 0; start += value.Length)
                matches.Add(new DartMatch(start, start + value.Length));
            return matches;
        }
    }
}

public sealed class RegExp : Pattern
{
    private readonly Regex _regex;
    public RegExp(string pattern, bool multiLine = false, bool caseSensitive = true, bool unicode = false, bool dotAll = false)
    {
        _ = unicode;
        pattern = pattern
            .Replace(@"\p{Space_Separator}", @"\p{Zs}", StringComparison.Ordinal)
            .Replace(@"\p{Punctuation}", @"\p{P}", StringComparison.Ordinal);
        var options = RegexOptions.CultureInvariant;
        if (multiLine) options |= RegexOptions.Multiline;
        if (!caseSensitive) options |= RegexOptions.IgnoreCase;
        if (dotAll) options |= RegexOptions.Singleline;
        _regex = new Regex(pattern, options);
    }
    public override IEnumerable<DartMatch> allMatches(string input) =>
        _regex.Matches(input).Select(value => new DartMatch(value.Index, value.Index + value.Length, value));
    public bool hasMatch(string input) => _regex.IsMatch(input);
}

public static class Dart_convertLibrary
{
    public static class utf8
    {
        public static Utf8Decoder decoder { get; } = new();
        public static Uint8List encode(string value) => new(Encoding.UTF8.GetBytes(value ?? throw new ArgumentNullException(nameof(value))));
        public static string decode(Uint8List value) => Encoding.UTF8.GetString(value.Select(item => checked((byte)item)).ToArray());
        public static string decode(List<long> value) => Encoding.UTF8.GetString(value.Select(item => checked((byte)item)).ToArray());
    }
    public sealed class Utf8Decoder
    {
        public string convert(ReadOnlyMemory<byte> value) => Encoding.UTF8.GetString(value.Span);
        public string convert(Uint8List value) => utf8.decode(value);
    }
    public static class base64
    {
        public static Base64Encoder encoder { get; } = new();
        public static Uint8List decode(string value) => new(Convert.FromBase64String(value));
        public static string encode(Uint8List value) => Convert.ToBase64String(value.Select(item => checked((byte)item)).ToArray());
    }
    public sealed class Base64Encoder
    {
        public string convert(Uint8List value) => base64.encode(value);
    }
    public static JsonCodec json { get; } = new();
    public static string jsonEncode(object? value, Func<object?, object?>? toEncodable = null) =>
        json.encode(toEncodable is null ? value : toEncodable(value));

    public sealed class JsonCodec
    {
        public string encode(object? value) => JsonSerializer.Serialize(value);
        public object? decode(string value)
        {
            using var document = JsonDocument.Parse(value);
            return ConvertElement(document.RootElement);
        }

        private static object? ConvertElement(JsonElement element) => element.ValueKind switch
        {
            JsonValueKind.Object => new DartMap<string, object?>(element.EnumerateObject()
                .ToDictionary(property => property.Name, property => ConvertElement(property.Value), StringComparer.Ordinal)),
            JsonValueKind.Array => element.EnumerateArray().Select(ConvertElement).ToList(),
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number when element.TryGetInt64(out var integer) => integer,
            JsonValueKind.Number => element.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null or JsonValueKind.Undefined => null,
            _ => throw new FormatException($"Unsupported JSON value kind: {element.ValueKind}."),
        };
    }
}
