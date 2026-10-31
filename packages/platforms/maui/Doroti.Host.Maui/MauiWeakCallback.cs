namespace Doroti.Host.Maui;

/// <summary>A queued notification borrows its managed owner's lifetime.</summary>
internal static class MauiWeakCallback
{
    internal static Action Create<T>(T owner, Action<T> invoke) where T : class
    {
        ArgumentNullException.ThrowIfNull(owner);
        ArgumentNullException.ThrowIfNull(invoke);
        var weak = new WeakReference<T>(owner);
        return () => { if (weak.TryGetTarget(out var current)) invoke(current); };
    }
}
