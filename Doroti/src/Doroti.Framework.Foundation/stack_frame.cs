// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/foundation/stack_frame.dart
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Doroti.Framework.Foundation;

public sealed record StackFrame
{
    private const string PackageStackTraceAsyncGap = "===== asynchronous gap ===========================";
    private static readonly Regex VmPattern = new(
        @"^#(?<number>\d+) +(?<member>.+) \((?<uri>.*?)(?::(?<line>\d+))?(?::(?<column>\d+))?\)$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex WebPattern = new(
        @"^\s*at (?<member>[^\s]+).*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public StackFrame(
        int number,
        int column,
        int line,
        string packageScheme,
        string package,
        string packagePath,
        string className = "",
        string method = "",
        bool isConstructor = false,
        string source = "")
    {
        this.number = number;
        this.column = column;
        this.line = line;
        this.packageScheme = packageScheme;
        this.package = package;
        this.packagePath = packagePath;
        this.className = className;
        this.method = method;
        this.isConstructor = isConstructor;
        this.source = source;
    }

    public static StackFrame asynchronousSuspension { get; } = new(-1, -1, -1, "", "", "", method: "asynchronous suspension", source: "<asynchronous suspension>");
    public static StackFrame stackOverFlowElision { get; } = new(-1, -1, -1, "", "", "", method: "...", source: "...");

    public string source { get; }
    public int number { get; }
    public string packageScheme { get; }
    public string package { get; }
    public string packagePath { get; }
    public int line { get; }
    public int column { get; }
    public string className { get; }
    public string method { get; }
    public bool isConstructor { get; }

    public static IReadOnlyList<StackFrame> fromStackTrace(StackTrace stack) => fromStackString(stack.ToString());

    public static IReadOnlyList<StackFrame> fromStackString(string stack) =>
        stack.Trim().Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n')
            .Where(value => value.Length != 0).Select(fromStackTraceLine).Where(frame => frame is not null).Cast<StackFrame>().ToArray();

    public static StackFrame? fromStackTraceLine(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value is "<asynchronous suspension>" or PackageStackTraceAsyncGap)
        {
            return asynchronousSuspension;
        }
        if (value == "...")
        {
            return stackOverFlowElision;
        }
        if (!value.StartsWith('#'))
        {
            var web = WebPattern.Match(value);
            if (!web.Success)
            {
                return null;
            }
            var parts = web.Groups["member"].Value.Split('.');
            return new StackFrame(-1, -1, -1, "<unknown>", "<unknown>", "<unknown>",
                parts.Length > 1 ? parts[0] : "<unknown>",
                parts.Length > 1 ? string.Join('.', parts.Skip(1)) : parts[0],
                source: value);
        }

        var match = VmPattern.Match(value);
        if (!match.Success)
        {
            return null;
        }
        var member = match.Groups["member"].Value.Replace(".<anonymous closure>", string.Empty, StringComparison.Ordinal);
        var constructor = member.StartsWith("new", StringComparison.Ordinal);
        var className = string.Empty;
        var method = member;
        if (constructor)
        {
            var constructorName = member.Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).FirstOrDefault() ?? "<unknown>";
            var parts = constructorName.Split('.', 2);
            className = parts[0];
            method = parts.Length > 1 ? parts[1] : string.Empty;
        }
        else if (member.Contains('.', StringComparison.Ordinal))
        {
            var parts = member.Split('.', 2);
            className = parts[0];
            method = parts[1];
        }

        ParseUri(match.Groups["uri"].Value, out var scheme, out var package, out var packagePath);
        return new StackFrame(
            int.Parse(match.Groups["number"].Value, CultureInfo.InvariantCulture),
            ParseCoordinate(match.Groups["column"]),
            ParseCoordinate(match.Groups["line"]),
            scheme,
            package,
            packagePath,
            className,
            method,
            constructor,
            value);
    }

    private static int ParseCoordinate(Group group) => group.Success ? int.Parse(group.Value, CultureInfo.InvariantCulture) : -1;

    private static void ParseUri(string raw, out string scheme, out string package, out string path)
    {
        var separator = raw.IndexOf(':');
        scheme = separator > 0 ? raw[..separator] : "file";
        path = separator > 0 ? raw[(separator + 1)..].TrimStart('/') : raw;
        package = "<unknown>";
        if (scheme is "dart" or "package")
        {
            var slash = path.IndexOf('/');
            package = slash >= 0 ? path[..slash] : path;
            path = slash >= 0 ? path[(slash + 1)..] : string.Empty;
        }
    }

    public override string ToString() =>
        $"StackFrame(#{number}, {packageScheme}:{package}/{packagePath}:{line}:{column}, className: {className}, method: {method})";
}
