using System.Text.Json;
using Doroti.Runtime;

var payload = new DartMap<string, object?>
{
    ["method"] = "TextInput.setClient",
    ["args"] = new List<object?>
    {
        42L,
        new DartMap<string, object?>
        {
            ["text"] = "한글 \"quoted\"\nline",
            ["enabled"] = true,
            ["nullable"] = null,
            ["selection"] = new long[] { -1, long.MaxValue },
            ["scale"] = 1.25,
        },
    },
};
var encoded = Dart_convertLibrary.json.encode(payload);
using (var document = JsonDocument.Parse(encoded))
{
    var root = document.RootElement;
    var arguments = root.GetProperty("args");
    var client = arguments[1];
    Require(root.GetProperty("method").GetString() == "TextInput.setClient" && arguments[0].GetInt64() == 42,
        "platform method and heterogeneous argument list");
    Require(client.GetProperty("text").GetString() == "한글 \"quoted\"\nline", "Unicode and escaping");
    Require(client.GetProperty("enabled").GetBoolean() && client.GetProperty("nullable").ValueKind == JsonValueKind.Null,
        "boolean and null");
    Require(client.GetProperty("selection")[1].GetInt64() == long.MaxValue && client.GetProperty("scale").GetDouble() == 1.25,
        "integer precision and floating point");
}
Require(Dart_convertLibrary.json.encode(Dart_convertLibrary.json.decode(encoded)) == encoded, "nested map/list round-trip");
Require(Dart_convertLibrary.jsonEncode(new object(), _ => new Dictionary<string, string> { ["value"] = "converted" })
    == "{\"value\":\"converted\"}", "explicit toEncodable conversion");
AssertThrows<JsonException>(() => Dart_convertLibrary.json.encode(new DartMap<long, string> { [1] = "invalid key" }));
AssertThrows<JsonException>(() => Dart_convertLibrary.json.encode(new object()));
AssertThrows<ArgumentException>(() => Dart_convertLibrary.json.encode(double.NaN));
var cycle = new List<object?>();
cycle.Add(cycle);
AssertThrows<InvalidOperationException>(() => Dart_convertLibrary.json.encode(cycle));
Console.WriteLine("Dart JSON codec PASS: channel payloads, Unicode, numeric precision, round-trip and invalid inputs");

static void Require(bool condition, string name)
{
    if (!condition) throw new InvalidOperationException(name);
}

static void AssertThrows<T>(Action action) where T : Exception
{
    try { action(); }
    catch (T) { return; }
    throw new InvalidOperationException($"Expected {typeof(T).Name}.");
}
