using Doroti.Hosting;
using Doroti.Ui;

namespace Doroti.Host.Maui;

/// <summary>Native app delegates deliver links here; binding and state remain owned by one view.</summary>
public static class MauiApplicationActivation
{
    private static readonly object Gate = new();
    private static readonly Queue<ApplicationActivation> Pending = new();
    private static Action<ApplicationActivation>? _receiver;
    public static void Deliver(string location, ApplicationActivationSource source, bool coldStart)
    {
        ApplicationNavigationHost.ValidateLocation(location);
        var activation = new ApplicationActivation(Guid.NewGuid().ToString("N"), location, source, coldStart);
        lock (Gate)
        {
            if (_receiver is { } receiver) receiver(activation);
            else if (Pending.Count < 32) Pending.Enqueue(activation);
            else throw new InvalidOperationException("Application activation queue is full.");
        }
    }
    internal static ApplicationActivation? TakeCold()
    {
        lock (Gate) return Pending.TryPeek(out var activation) && activation.ColdStart ? Pending.Dequeue() : null;
    }
    internal static IDisposable Attach(Action<ApplicationActivation> receiver)
    {
        lock (Gate)
        {
            if (_receiver is not null) throw new NotSupportedException("MAUI activation currently has one application view owner.");
            _receiver = receiver;
            while (Pending.TryDequeue(out var activation)) receiver(activation);
        }
        return new Subscription(() => { lock (Gate) { if (_receiver == receiver) _receiver = null; } });
    }
    private sealed class Subscription(Action dispose) : IDisposable { public void Dispose() => dispose(); }
}

#if ANDROID
/// <summary>Use as the runner activity base; declare app-specific IntentFilter schemes in the runner.</summary>
public abstract class DorotiMauiActivity : Microsoft.Maui.MauiAppCompatActivity
{
    protected override void OnCreate(Android.OS.Bundle? savedInstanceState)
    {
        if (savedInstanceState is null && Intent?.Action == Android.Content.Intent.ActionView && Intent.DataString is { } location)
            TryDeliver(location, true);
        base.OnCreate(savedInstanceState);
    }
    protected override void OnNewIntent(Android.Content.Intent? intent)
    {
        base.OnNewIntent(intent);
        if (intent?.Action == Android.Content.Intent.ActionView && intent.DataString is { } location)
            TryDeliver(location, false);
    }
    private static void TryDeliver(string location, bool cold)
    {
        try { MauiApplicationActivation.Deliver(location, ApplicationActivationSource.AndroidIntent, cold); }
        catch (ArgumentException) { /* Invalid external links leave the current route unchanged. */ }
    }
}
#endif
