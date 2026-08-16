// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/foundation/print.dart
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Doroti.Framework.Foundation;

public delegate void DebugPrintCallback(string? message, int? wrapWidth = null);

public static partial class PrintLibrary
{
    internal const int _kDebugPrintCapacity = 12 * 1024;
    internal static readonly TimeSpan _kDebugPrintPauseTime = TimeSpan.FromSeconds(1);
    internal static readonly Queue<string> _debugPrintBuffer = new();
    internal static readonly Stopwatch _debugPrintStopwatch = Stopwatch.StartNew();
    internal static readonly Regex _indentPattern = new(@"^\s*", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    internal static int _debugPrintedCharacters;
    internal static bool _debugPrintScheduled;
    internal static Task _debugPrintTask = Task.CompletedTask;
    internal static TaskCompletionSource? _debugPrintCompleter;

    public static DebugPrintCallback debugPrint { get; set; } = debugPrintThrottled;

    public static Task debugPrintDone => _debugPrintCompleter?.Task ?? Task.CompletedTask;

    public static void debugPrintSynchronously(string? message, int? wrapWidth = null)
    {
        foreach (var line in debugWordWrap(message ?? "null", wrapWidth))
        {
            Console.WriteLine(line);
        }
    }

    public static void debugPrintThrottled(string? message, int? wrapWidth = null)
    {
        lock (_debugPrintBuffer)
        {
            foreach (var line in debugWordWrap(message ?? "null", wrapWidth))
            {
                _debugPrintBuffer.Enqueue(line);
            }
            if (_debugPrintScheduled)
            {
                return;
            }
            _debugPrintScheduled = true;
            _debugPrintCompleter = new(TaskCreationOptions.RunContinuationsAsynchronously);
            _debugPrintTask = DrainAsync();
        }
    }

    public static IReadOnlyList<string> debugWordWrap(string message, int? width = null)
    {
        if (width is null || width <= 0)
        {
            return message.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n');
        }
        var result = new List<string>();
        foreach (var input in message.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n'))
        {
            if (input.Length <= width)
            {
                result.Add(input);
                continue;
            }
            var indent = _indentPattern.Match(input).Value;
            var remaining = input;
            while (remaining.Length > width)
            {
                var breakAt = remaining.LastIndexOf(' ', width.Value);
                if (breakAt <= indent.Length)
                {
                    breakAt = width.Value;
                }
                result.Add(remaining[..breakAt].TrimEnd());
                remaining = indent + remaining[breakAt..].TrimStart();
            }
            result.Add(remaining);
        }
        return result;
    }

    private static async Task DrainAsync()
    {
        while (true)
        {
            string? line;
            lock (_debugPrintBuffer)
            {
                if (_debugPrintBuffer.Count == 0)
                {
                    _debugPrintScheduled = false;
                    _debugPrintedCharacters = 0;
                    _debugPrintCompleter?.TrySetResult();
                    _debugPrintCompleter = null;
                    return;
                }
                line = _debugPrintBuffer.Dequeue();
            }
            Console.WriteLine(line);
            _debugPrintedCharacters += line.Length;
            if (_debugPrintedCharacters >= _kDebugPrintCapacity)
            {
                await Task.Delay(_kDebugPrintPauseTime).ConfigureAwait(false);
                _debugPrintedCharacters = 0;
                _debugPrintStopwatch.Restart();
            }
        }
    }
}

internal enum _WordWrapParseMode
{
    inSpace,
    inWord,
    atBreak,
}
