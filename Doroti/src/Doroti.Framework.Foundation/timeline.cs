// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/foundation/timeline.dart
using System.Diagnostics;

namespace Doroti.Framework.Foundation;

public sealed class TimedBlock : IDisposable
{
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private readonly Action<TimeSpan>? _onFinish;
    private bool _disposed;

    internal TimedBlock(string name, Action<TimeSpan>? onFinish = null)
    {
        this.name = name;
        _onFinish = onFinish;
    }

    public string name { get; }
    public TimeSpan elapsed => _stopwatch.Elapsed;

    public void finish() => Dispose();

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        _stopwatch.Stop();
        _onFinish?.Invoke(_stopwatch.Elapsed);
    }
}

public sealed record AggregatedTimedBlock(string name, int count, TimeSpan total)
{
    public TimeSpan average => count == 0 ? TimeSpan.Zero : TimeSpan.FromTicks(total.Ticks / count);
}

public sealed class AggregatedTimings
{
    private readonly Dictionary<string, (int Count, TimeSpan Total)> _values = new(StringComparer.Ordinal);

    public IReadOnlyList<AggregatedTimedBlock> blocks => _values.OrderBy(item => item.Key, StringComparer.Ordinal)
        .Select(item => new AggregatedTimedBlock(item.Key, item.Value.Count, item.Value.Total)).ToArray();

    public TimedBlock start(string name) => new(name, elapsed => Add(name, elapsed));

    public void reset() => _values.Clear();

    private void Add(string name, TimeSpan elapsed)
    {
        var previous = _values.GetValueOrDefault(name);
        _values[name] = (previous.Count + 1, previous.Total + elapsed);
    }
}

public static class FlutterTimeline
{
    public static TimedBlock startSync(string name, IReadOnlyDictionary<string, object?>? arguments = null)
    {
        _ = arguments;
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new TimedBlock(name);
    }

    public static TimedBlock startSync(string name, IReadOnlyDictionary<string, string>? arguments) =>
        startSync(name, arguments?.ToDictionary(item => item.Key, item => (object?)item.Value));

    public static T timeSync<T>(string name, Func<T> function, IReadOnlyDictionary<string, object?>? arguments = null)
    {
        using var block = startSync(name, arguments);
        return function();
    }

    public static Task<T> timeSync<T>(string name, Func<Task<T>> function, IReadOnlyDictionary<string, object?>? arguments = null) =>
        TimeAsync(name, function, arguments);

    public static void finishSync()
    {
    }

    private static async Task<T> TimeAsync<T>(string name, Func<Task<T>> function, IReadOnlyDictionary<string, object?>? arguments)
    {
        using var block = startSync(name, arguments);
        return await function().ConfigureAwait(false);
    }
}

internal sealed class _BlockBuffer : Queue<double>;
internal sealed class _Float64ListChain : List<double>;
internal sealed class _StringListChain : List<string>;
internal static class TimelineLibrary { internal const int _kSliceSize = 500; }
