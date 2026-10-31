using System.Runtime.CompilerServices;
using Doroti.Host.Maui;

internal static class WeakNotificationRegression
{
    private sealed class Owner { internal int Calls; }
    internal static void Run()
    {
        var live = new Owner();
        var notify = MauiWeakCallback.Create(live, static owner => owner.Calls++);
        notify();
        Collect();
        notify();
        if (live.Calls != 2) throw new InvalidOperationException("A live input owner must receive coalesced notifications.");
        GC.KeepAlive(live);

        var pending = AbandonOwner();
        Collect();
        if (pending.Owner.IsAlive) throw new InvalidOperationException("A pending native notification must not retain its owner or widget tree.");
        pending.Callback(); // A late native delivery after teardown is harmless.
        GC.KeepAlive(pending.Callback);
        Console.WriteLine("PASS: queued weak notifications preserve live delivery and release abandoned owners before native callback retirement");
    }
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static (WeakReference Owner, Action Callback) AbandonOwner()
    {
        var owner = new Owner();
        return (new(owner), MauiWeakCallback.Create(owner, static target => target.Calls++));
    }
    private static void Collect()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }
}
