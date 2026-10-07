using System.Text.Json;
using System.Text.RegularExpressions;

namespace Doroti.Web.DevServer;

public sealed record ProxyRule(Uri Target, string? Prefix, Regex? Pattern, string? Replace)
{
    public Uri? Destination(string path, string query)
    {
        string rewritten;
        if (Prefix is not null)
        {
            if (!path.StartsWith(Prefix, StringComparison.Ordinal)) return null;
            rewritten = Replace is null ? path : Replace + path[Prefix.Length..];
        }
        else
        {
            if (!Pattern!.IsMatch(path)) return null;
            rewritten = Replace is null ? path : Pattern.Replace(path, Replace, 1);
        }
        // Concatenation keeps the configured origin authoritative even for // paths.
        var destination = Target.AbsoluteUri.TrimEnd('/') + "/" + rewritten.TrimStart('/');
        if (query.Length > 0) destination += destination.Contains('?') ? "&" + query.TrimStart('?') : query;
        return new Uri(destination, UriKind.Absolute);
    }
}

public sealed record WebDevConfig(string? Host, int? Port, string? Certificate, string? CertificateKey,
    IReadOnlyDictionary<string, string> Headers, IReadOnlyList<ProxyRule> Proxy)
{
    public static WebDevConfig Load(string file)
    {
        try
        {
            using var reader = File.OpenText(file);
            return Parse(reader, Path.GetDirectoryName(Path.GetFullPath(file))!);
        }
        catch (Exception error) when (error is JsonException or ArgumentException or InvalidDataException or IOException)
        {
            throw new InvalidDataException($"Invalid Web development configuration '{file}': {error.Message}", error);
        }
    }

    public static WebDevConfig Parse(TextReader reader, string directory)
    {
        using var document = JsonDocument.Parse(reader.ReadToEnd(), new JsonDocumentOptions
        {
            CommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        });
        var root = Mapping(document.RootElement, "root", "server");
        var server = Mapping(Required(root, "server"), "server", "host", "port", "https", "headers", "proxy");
        var host = Optional(server, "host");
        if (host is not null && (host.Length == 0 || Uri.CheckHostName(host.Trim('[', ']')) == UriHostNameType.Unknown))
            throw new InvalidDataException("server.host must be a hostname or IP address.");
        int? port = null;
        if (server.TryGetValue("port", out var portNode))
        {
            if (portNode.ValueKind != JsonValueKind.Number || !portNode.TryGetInt32(out var value) || value is < 1 or > 65535)
                throw new InvalidDataException("server.port must be between 1 and 65535.");
            port = value;
        }
        string? certificate = null, key = null;
        if (server.TryGetValue("https", out var httpsNode))
        {
            var https = Mapping(httpsNode, "server.https", "certPath", "certKeyPath");
            certificate = Path.GetFullPath(Scalar(Required(https, "certPath")), directory);
            key = Path.GetFullPath(Scalar(Required(https, "certKeyPath")), directory);
        }
        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (server.TryGetValue("headers", out var headersNode))
            foreach (var node in Sequence(headersNode, "server.headers"))
            {
                var entry = Mapping(node, "header", "name", "value");
                var name = Scalar(Required(entry, "name"));
                var value = Scalar(Required(entry, "value"));
                if (name.Length == 0 || name.Any(c => !char.IsAsciiLetterOrDigit(c) && !"!#$%&'*+-.^_`|~".Contains(c)) ||
                    value.Any(c => c is '\r' or '\n' or '\0') || !headers.TryAdd(name, value))
                    throw new InvalidDataException("Invalid or duplicate response header.");
                if (name.Equals("Content-Length", StringComparison.OrdinalIgnoreCase) || name.Equals("Transfer-Encoding", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidDataException("Custom headers cannot change response framing.");
            }
        var proxy = new List<ProxyRule>();
        if (server.TryGetValue("proxy", out var proxyNode))
            foreach (var node in Sequence(proxyNode, "server.proxy"))
            {
                var entry = Mapping(node, "proxy rule", "target", "prefix", "regex", "replace");
                if (!Uri.TryCreate(Scalar(Required(entry, "target")), UriKind.Absolute, out var target) ||
                    target.Scheme is not ("http" or "https") || target.UserInfo.Length != 0 || target.Query.Length != 0 || target.Fragment.Length != 0)
                    throw new InvalidDataException("Proxy target must be an HTTP(S) base URL without credentials, query or fragment.");
                var prefix = Optional(entry, "prefix");
                var regex = Optional(entry, "regex");
                if ((prefix is null) == (regex is null)) throw new InvalidDataException("Each proxy rule needs exactly one of prefix or regex.");
                if (prefix is not null && !prefix.StartsWith('/')) throw new InvalidDataException("Proxy prefix must start with /.");
                if (regex is { Length: 0 }) throw new InvalidDataException("Proxy regex cannot be empty.");
                var replace = Optional(entry, "replace");
                if (replace?.Contains('#') == true) throw new InvalidDataException("Proxy replacement cannot contain a fragment.");
                proxy.Add(new(target, prefix, regex is null ? null : new Regex(regex, RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100)), replace));
            }
        return new(host, port, certificate, key, headers, proxy);
    }

    private static Dictionary<string, JsonElement> Mapping(JsonElement node, string location, params string[] allowed)
    {
        if (node.ValueKind != JsonValueKind.Object) throw new InvalidDataException($"{location} must be an object.");
        var result = new Dictionary<string, JsonElement>(StringComparer.Ordinal);
        foreach (var pair in node.EnumerateObject())
        {
            var key = pair.Name;
            if (!allowed.Contains(key) || !result.TryAdd(key, pair.Value)) throw new InvalidDataException($"Unknown or duplicate {location} setting: {key}.");
        }
        return result;
    }
    private static IEnumerable<JsonElement> Sequence(JsonElement node, string location) =>
        node.ValueKind == JsonValueKind.Array ? node.EnumerateArray() : throw new InvalidDataException($"{location} must be an array.");
    private static JsonElement Required(Dictionary<string, JsonElement> map, string key) =>
        map.TryGetValue(key, out var value) ? value : throw new InvalidDataException($"Missing setting: {key}.");
    private static string? Optional(Dictionary<string, JsonElement> map, string key) => map.TryGetValue(key, out var node) ? Scalar(node) : null;
    private static string Scalar(JsonElement node) => node.ValueKind == JsonValueKind.String ? node.GetString()! : throw new InvalidDataException("Expected a string value.");
}
