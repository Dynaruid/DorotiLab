// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/foundation/assertions.dart
using System.Diagnostics;
using Doroti.Flutter.Runtime;

namespace Doroti.Generated.Framework.Foundation;

public delegate IEnumerable<DiagnosticsNode> DiagnosticPropertiesTransformer(IEnumerable<DiagnosticsNode> properties);
public delegate void FlutterExceptionHandler(FlutterErrorDetails details);
public delegate IEnumerable<DiagnosticsNode> InformationCollector();
public delegate StackTrace StackTraceDemangler(StackTrace stack);

public class ErrorDescription(string message) : DiagnosticsNode(null, message, DiagnosticsTreeStyle.errorProperty, DiagnosticLevel.info, showName: false);
public sealed class ErrorSummary(string message) : DiagnosticsNode(null, message, DiagnosticsTreeStyle.flat, DiagnosticLevel.summary, showName: false);
public sealed class ErrorHint(string message) : DiagnosticsNode(null, message, DiagnosticsTreeStyle.flat, DiagnosticLevel.hint, showName: false);
public sealed class ErrorSpacer() : DiagnosticsNode(null, string.Empty, DiagnosticsTreeStyle.whitespace, showName: false);

public sealed record PartialStackFrame(object package, string className, string method)
{
    public static PartialStackFrame asynchronousSuspension { get; } = new(string.Empty, string.Empty, "asynchronous suspension");

    public bool matches(StackFrame stackFrame)
    {
        ArgumentNullException.ThrowIfNull(stackFrame);
        var stackPackage = $"{stackFrame.packageScheme}:{stackFrame.package}/{stackFrame.packagePath}";
        var packageMatches = package switch
        {
            string text => stackPackage.Contains(text, StringComparison.Ordinal),
            RegExp expression => expression.hasMatch(stackPackage),
            _ => string.Equals(package.ToString(), stackPackage, StringComparison.Ordinal),
        };
        return packageMatches && stackFrame.className == className && stackFrame.method == method;
    }
}

public abstract class StackFilter
{
    public abstract IEnumerable<string> filter(IEnumerable<string> frames);
}

public sealed class RepetitiveStackFrameFilter : StackFilter
{
    private readonly string? _prefix;
    private readonly string? _suffix;
    private readonly int _replacementCount;
    private readonly IReadOnlyList<PartialStackFrame>? _frames;
    private readonly string? _replacement;

    public RepetitiveStackFrameFilter(string prefix, string suffix, int replacementCount = 1)
    {
        _prefix = prefix;
        _suffix = suffix;
        _replacementCount = replacementCount;
    }

    public RepetitiveStackFrameFilter(List<PartialStackFrame> frames, string replacement)
    {
        _frames = frames ?? throw new ArgumentNullException(nameof(frames));
        _replacement = replacement ?? throw new ArgumentNullException(nameof(replacement));
        _replacementCount = 1;
    }

    public override IEnumerable<string> filter(IEnumerable<string> frames)
    {
        ArgumentNullException.ThrowIfNull(frames);
        var buffered = frames.ToArray();
        if (_frames is { Count: > 0 } patterns)
        {
            for (var frameIndex = 0; frameIndex < buffered.Length;)
            {
                var parsed = frameIndex + patterns.Count <= buffered.Length
                    ? buffered.Skip(frameIndex).Take(patterns.Count).Select(StackFrame.fromStackTraceLine).ToArray()
                    : [];
                if (parsed.Length == patterns.Count && parsed.All(frame => frame is not null) &&
                    patterns.Select((pattern, offset) => pattern.matches(parsed[offset]!)).All(matches => matches))
                {
                    yield return _replacement!;
                    frameIndex += patterns.Count;
                    continue;
                }
                yield return buffered[frameIndex++];
            }
            yield break;
        }
        var index = 0;
        while (index < buffered.Length)
        {
            var start = index;
            while (index < buffered.Length && buffered[index].StartsWith(_prefix!, StringComparison.Ordinal) && buffered[index].EndsWith(_suffix!, StringComparison.Ordinal))
            {
                index++;
            }
            var count = index - start;
            if (count > _replacementCount)
            {
                yield return $"... {count} frames elided ...";
            }
            else if (count > 0)
            {
                for (var cursor = start; cursor < index; cursor++)
                {
                    yield return buffered[cursor];
                }
            }
            else
            {
                yield return buffered[index++];
            }
        }
    }
}

public sealed class DiagnosticsStackTrace : DiagnosticsBlock
{
    public DiagnosticsStackTrace(
        string name,
        StackTrace? stack,
        StackFilter? stackFilter = null,
        bool showSeparator = true)
        : base(
            name,
            stack?.ToString() ?? string.Empty,
            (stack is null
                ? []
                : stackFilter?.filter(stack.ToString().Split('\n')) ?? stack.ToString().Split('\n'))
            .Select(line => new DiagnosticsNode(null, line.TrimEnd(), showName: false)),
            showSeparator: showSeparator)
    {
    }
}

public class FlutterErrorDetails
{
    public FlutterErrorDetails(
        object exception,
        StackTrace? stack = null,
        string? library = null,
        DiagnosticsNode? context = null,
        InformationCollector? informationCollector = null,
        bool silent = false)
    {
        exceptionThrown = exception as Exception ?? new Exception(exception?.ToString() ?? "null");
        this.stack = stack;
        this.library = library ?? "Flutter framework";
        this.context = context;
        this.informationCollector = informationCollector;
        this.silent = silent;
    }

    public FlutterErrorDetails(
        Exception exception,
        StackTrace? stack,
        DiagnosticsNode context,
        InformationCollector informationCollector)
        : this(exception, stack, null, context, informationCollector, false)
    {
    }

    public FlutterErrorDetails(object exception, string library, Func<List<DiagnosticsNode>> informationCollector)
        : this(
            exception as Exception ?? new Exception(exception?.ToString() ?? "null"),
            null,
            library,
            null,
            () => informationCollector(),
            false)
    {
    }

    public Exception exceptionThrown { get; }
    public object exception => exceptionThrown;
    public StackTrace? stack { get; }
    public string library { get; }
    public DiagnosticsNode? context { get; }
    public InformationCollector? informationCollector { get; }
    public bool silent { get; }
    public string exceptionAsString() => exceptionThrown.Message;
    public DiagnosticsNode summary => exceptionThrown is FlutterError flutterError
        ? flutterError.diagnostics
        : new ErrorSummary(exceptionAsString().Split('\n')[0].TrimStart());

    public DiagnosticsNode toDiagnosticsNode()
    {
        var children = new List<DiagnosticsNode>
        {
            exceptionThrown is FlutterError flutterError ? flutterError.diagnostics : new ErrorSummary(exceptionAsString()),
        };
        if (context is not null)
        {
            children.Add(new ErrorDescription($"The following exception was thrown {context.toDescription()}"));
        }
        if (informationCollector is not null)
        {
            children.AddRange(informationCollector());
        }
        if (stack is not null)
        {
            children.Add(new DiagnosticsStackTrace("When the exception was thrown, this was the stack", stack));
        }
        return new DiagnosticsBlock("FlutterErrorDetails", library, children, DiagnosticsTreeStyle.error);
    }

    public override string ToString() => toDiagnosticsNode().toStringDeep();
}

public class FlutterError : Exception
{
    public const long wrapWidth = 100;

    public FlutterError(string message) : this(new ErrorSummary(message)) { }

    public FlutterError(DiagnosticsNode diagnostics) : base(diagnostics.toDescription()) => this.diagnostics = diagnostics;

    public FlutterError(IEnumerable<DiagnosticsNode> diagnostics)
        : this(new DiagnosticsBlock("FlutterError", "", diagnostics, DiagnosticsTreeStyle.error))
    {
    }

    public static FlutterError Create(string message) => new(message);

    public static FlutterError Create(IEnumerable<DiagnosticsNode> diagnostics) => new(diagnostics);

    public static void addDefaultStackFilter(Func<IEnumerable<string>, IEnumerable<string>> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
    }

    public static void addDefaultStackFilter(StackFilter filter) =>
        ArgumentNullException.ThrowIfNull(filter);

    public static FlutterExceptionHandler presentError { get; set; } = reportError;

    public DiagnosticsNode diagnostics { get; }
    public StackTrace stackTrace => new(this, true);

    public static FlutterExceptionHandler? onError { get; set; }

    public static StackTrace demangleStackTrace(StackTrace stack) => stack;

    public static IEnumerable<string> defaultStackFilter(IEnumerable<string> frames) => frames;

    public static void reportError(FlutterErrorDetails details)
    {
        ArgumentNullException.ThrowIfNull(details);
        if (onError is { } handler)
        {
            handler(details);
            return;
        }
        dumpErrorToConsole(details);
    }

    public static void dumpErrorToConsole(FlutterErrorDetails details, bool forceReport = false)
    {
        if (!details.silent || forceReport)
        {
            PrintLibrary.debugPrint(details.ToString());
        }
    }

    public static void resetErrorCount()
    {
    }
}

internal sealed class _ErrorDiagnostic(string message) : ErrorDescription(message);
internal sealed class _FlutterErrorDetailsNode(FlutterErrorDetails details) : DiagnosticsNode("FlutterErrorDetails", details, DiagnosticsTreeStyle.error);

public static class AssertionsLibrary
{
    public static void debugPrintStack(string? label = null, StackTrace? stackTrace = null, int? maxFrames = null)
    {
        var lines = (stackTrace ?? new StackTrace(1, true)).ToString().Split('\n');
        if (maxFrames is { } limit)
        {
            lines = lines.Take(limit).ToArray();
        }
        PrintLibrary.debugPrint(string.Join(Environment.NewLine, string.IsNullOrEmpty(label) ? lines : new[] { label }.Concat(lines)));
    }
}
