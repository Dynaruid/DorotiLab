using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace Doroti.Host.Web;

/// <summary>JS event-loop context for a standalone, single-threaded WASM worker.</summary>
[SupportedOSPlatform("browser")]
public sealed partial class BrowserOwnerSynchronizationContext : SynchronizationContext
{
    private static readonly Dictionary<int, (BrowserOwnerSynchronizationContext Owner, SendOrPostCallback Callback, object? State)> Pending = [];
    private static int _nextId;
    private readonly int _threadId = Environment.CurrentManagedThreadId;

    public static void EnsureInstalled()
    {
        if (Current is not null) return;
        if (Environment.GetEnvironmentVariable("DOROTI_STANDALONE_WORKER") != "1")
            throw new InvalidOperationException("Browser rendering requires its JS owner synchronization context.");
        SetSynchronizationContext(new BrowserOwnerSynchronizationContext());
    }

    public override void Post(SendOrPostCallback callback, object? state)
    {
        ArgumentNullException.ThrowIfNull(callback);
        if (Environment.CurrentManagedThreadId != _threadId)
            throw new InvalidOperationException("Standalone browser callbacks cannot leave their JS owner.");
        if (Pending.Count >= 4096) throw new InvalidOperationException("Browser owner callback queue is full.");
        var id = checked(++_nextId);
        Pending.Add(id, (this, callback, state));
        try { Schedule(id); }
        catch { Pending.Remove(id); throw; }
    }

    public override void Send(SendOrPostCallback callback, object? state)
    {
        if (Environment.CurrentManagedThreadId != _threadId)
            throw new NotSupportedException("Cross-thread synchronous browser dispatch is unsupported.");
        callback(state);
    }

    [JSImport("postOwnerCallback", "doroti.web")]
    private static partial void Schedule(int id);

    [JSExport]
    public static void Dispatch(int id)
    {
        if (!Pending.Remove(id, out var work)) return;
        if (Environment.CurrentManagedThreadId != work.Owner._threadId)
            throw new InvalidOperationException("Browser owner callback arrived on a foreign thread.");
        var previous = Current;
        SetSynchronizationContext(work.Owner);
        try { work.Callback(work.State); }
        finally { SetSynchronizationContext(previous); }
    }
}
