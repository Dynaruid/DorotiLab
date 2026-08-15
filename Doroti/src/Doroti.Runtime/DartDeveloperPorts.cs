using System.Collections.ObjectModel;

namespace Doroti.Runtime;

/// <summary>A Dart developer timeline flow identifier.</summary>
public sealed record Flow(long id)
{
    public static Flow begin() => DartDeveloperTimeline.beginFlow();
    public static Flow step(long id) => DartDeveloperTimeline.step(id);
    public static Flow end(long id) => new(id);
}

/// <summary>Host-neutral representation of dart:developer CreationLocation.</summary>
public sealed record CreationLocation(string file, long line, long column, string? name = null)
{
    public static CreationLocation? of(object? value) => value as CreationLocation;
    public override string ToString() => name is null
        ? $"{file}:{line}:{column}"
        : $"{name} ({file}:{line}:{column})";
}

/// <summary>
/// Observable runtime ownership for dart:developer timeline operations. The port keeps
/// Dart tracing semantics out of generated framework source while allowing a host or a
/// validation listener to consume every emitted event.
/// </summary>
public static class DartDeveloperTimeline
{
    private static readonly object Gate = new();
    private static readonly List<DartTimelineEvent> Events = [];
    private static long _nextFlowId;

    public static event Action<DartTimelineEvent>? eventEmitted;

    public static IReadOnlyList<DartTimelineEvent> snapshot()
    {
        lock (Gate)
        {
            return new ReadOnlyCollection<DartTimelineEvent>(Events.ToArray());
        }
    }

    public static void clear()
    {
        lock (Gate)
        {
            Events.Clear();
        }
    }

    internal static Flow step(long id) => new(id);

    internal static Flow beginFlow() => new(Interlocked.Increment(ref _nextFlowId));

    internal static void emit(string name, DartTimelineEventKind kind, Flow? flow = null)
    {
        var value = new DartTimelineEvent(name, kind, flow?.id, DateTimeOffset.UtcNow);
        lock (Gate)
        {
            Events.Add(value);
        }
        eventEmitted?.Invoke(value);
    }
}

public enum DartTimelineEventKind
{
    begin,
    end,
    instant
}

public readonly record struct DartTimelineEvent(
    string name,
    DartTimelineEventKind kind,
    long? flowId,
    DateTimeOffset timestamp);

/// <summary>A dart:developer TimelineTask with balanced start/finish validation.</summary>
public sealed class TimelineTask
{
    private readonly Stack<string> _activeNames = new();

    public void start(string name, object? arguments = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _activeNames.Push(name);
        DartDeveloperTimeline.emit(name, DartTimelineEventKind.begin);
    }

    public void finish(object? arguments = null)
    {
        if (_activeNames.Count == 0)
        {
            throw new InvalidOperationException("A TimelineTask cannot finish before it starts.");
        }
        var name = _activeNames.Pop();
        DartDeveloperTimeline.emit(name, DartTimelineEventKind.end);
    }
}

/// <summary>Library-shaped adapters used by semantic lowering for dart:developer.</summary>
public static class Dart_developerLibrary
{
    public static void postEvent(string eventKind, object? eventData, string? stream = null) { _ = eventKind; _ = eventData; _ = stream; }
    public static T inspect<T>(T value) => value;
    public static class Flow
    {
        public static global::Doroti.Runtime.Flow step(long id) =>
            DartDeveloperTimeline.step(id);
    }

    public static class Timeline
    {
        private static readonly AsyncLocal<Stack<string>?> ActiveSlices = new();

        public static void startSync(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            var slices = ActiveSlices.Value ??= new Stack<string>();
            slices.Push(name);
            DartDeveloperTimeline.emit(name, DartTimelineEventKind.begin);
        }

        public static void finishSync()
        {
            var slices = ActiveSlices.Value;
            if (slices is null || slices.Count == 0)
            {
                throw new InvalidOperationException("Timeline.finishSync requires a matching startSync.");
            }
            DartDeveloperTimeline.emit(slices.Pop(), DartTimelineEventKind.end);
        }

        public static T timeSync<T>(string name, Func<T> callback, global::Doroti.Runtime.Flow? flow = null)
        {
            ArgumentNullException.ThrowIfNull(callback);
            DartDeveloperTimeline.emit(name, DartTimelineEventKind.begin, flow);
            try
            {
                return callback();
            }
            finally
            {
                DartDeveloperTimeline.emit(name, DartTimelineEventKind.end, flow);
            }
        }

        public static void timeSync(string name, Action callback, global::Doroti.Runtime.Flow? flow = null)
        {
            ArgumentNullException.ThrowIfNull(callback);
            timeSync<object?>(name, () =>
            {
                callback();
                return null;
            }, flow);
        }
    }
}

/// <summary>Unprefixed dart:developer Timeline API emitted from show-imports.</summary>
public static class Timeline
{
    public static void startSync(string name) => Dart_developerLibrary.Timeline.startSync(name);

    public static void finishSync() => Dart_developerLibrary.Timeline.finishSync();

    public static void instantSync(string name, object? arguments = null) =>
        DartDeveloperTimeline.emit(name, DartTimelineEventKind.instant);

    public static T timeSync<T>(string name, Func<T> callback, Flow? flow = null) =>
        Dart_developerLibrary.Timeline.timeSync(name, callback, flow);

    public static void timeSync(string name, Action callback, Flow? flow = null) =>
        Dart_developerLibrary.Timeline.timeSync(name, callback, flow);
}
