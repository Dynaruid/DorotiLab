namespace Doroti.Host.Maui;

/// <summary>Managed work owns completion; a native notification carries only its token.</summary>
internal static class MauiManagedCallbackQueue
{
    private static readonly object Gate = new();
    private static readonly Dictionary<long, Action> Pending = [];
    private static long _nextToken;

    internal static int PendingCount { get { lock (Gate) return Pending.Count; } }

    internal static void Post(Action<Action> post, Action callback)
    {
        ArgumentNullException.ThrowIfNull(post);
        ArgumentNullException.ThrowIfNull(callback);
        long token;
        lock (Gate)
        {
            token = checked(++_nextToken);
            Pending.Add(token, callback);
        }
        try
        {
            // Do not capture callback or its ExecutionContext in the Java peer.
            // The dispatcher bounds pending work; retain it on the managed side
            // until delivery, including cancellation and teardown completions.
            post(() => Invoke(token));
        }
        catch
        {
            lock (Gate) Pending.Remove(token);
            throw;
        }
    }

    private static void Invoke(long token)
    {
        Action? callback;
        lock (Gate)
        {
            if (!Pending.Remove(token, out callback)) return;
        }
        // Remove before invoking: reentrancy, exceptions and duplicate native
        // delivery cannot retain or execute the completed managed work twice.
        callback();
    }
}
