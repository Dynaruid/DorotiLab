using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Doroti.Host.Maui;

internal static class ManagedNativeCallbackRegression
{
    private sealed class Owner { internal int Calls; }

    internal static void Run()
    {
        var pending = CreatePending();
        Collect();
        Check(pending.Owner.IsAlive && MauiManagedCallbackQueue.PendingCount == 1,
            "managed work owns its callback until native delivery");
        pending.Native();
        Check(MauiManagedCallbackQueue.PendingCount == 0, "delivery releases managed registration");
        pending.Native();
        Collect();
        Check(!pending.Owner.IsAlive, "completed native notification carries no managed owner or execution context");

        var failed = FailPost();
        failed.Native();
        Collect();
        Check(!failed.Owner.IsAlive && MauiManagedCallbackQueue.PendingCount == 0,
            "failed posting releases work and retracts a late native delivery");

        Action? throwing = null;
        MauiManagedCallbackQueue.Post(action => throwing = action,
            () => throw new InvalidOperationException("expected callback failure"));
        try { throwing!(); }
        catch (InvalidOperationException error) when (error.Message == "expected callback failure") { }
        Check(MauiManagedCallbackQueue.PendingCount == 0, "throwing work releases registration before user code");
        throwing!();

        var nativeQueue = new ConcurrentQueue<Action>();
        var delivered = 0;
        Parallel.For(0, 20, _ => MauiManagedCallbackQueue.Post(nativeQueue.Enqueue, () => delivered++));
        Check(MauiManagedCallbackQueue.PendingCount == 20, "concurrent producers preserve all pending work");
        while (nativeQueue.TryDequeue(out var callback)) { callback(); callback(); }
        Check(delivered == 20 && MauiManagedCallbackQueue.PendingCount == 0, "each native token delivers once");

        MauiManagedCallbackQueue.Post(nativeQueue.Enqueue, () =>
            MauiManagedCallbackQueue.Post(nativeQueue.Enqueue, () => delivered++));
        while (nativeQueue.TryDequeue(out var callback)) callback();
        Check(delivered == 21 && MauiManagedCallbackQueue.PendingCount == 0, "reentrant posting cannot lose work");
        GC.KeepAlive(pending.Native);
        Console.WriteLine("PASS: managed callback ownership, late/duplicate delivery, failure cleanup and concurrent/reentrant dispatch");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static (WeakReference Owner, Action Native) CreatePending()
    {
        var owner = new Owner();
        Action? native = null;
        MauiManagedCallbackQueue.Post(action => native = action, () => owner.Calls++);
        return (new(owner), native!);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static (WeakReference Owner, Action Native) FailPost()
    {
        var owner = new Owner();
        Action? native = null;
        try
        {
            MauiManagedCallbackQueue.Post(action =>
            {
                native = action;
                throw new InvalidOperationException("expected posting failure");
            }, () => owner.Calls++);
        }
        catch (InvalidOperationException error) when (error.Message == "expected posting failure") { }
        return (new(owner), native!);
    }

    private static void Collect() { GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect(); }
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
