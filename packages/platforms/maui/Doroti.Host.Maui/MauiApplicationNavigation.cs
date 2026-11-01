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
        // Only diagnostic flags are forwarded; initialize before framework static
        // profile switches are read. Normal launches retain their existing flags.
        foreach (var flag in new[] { "DOROTI_SCROLL_PROFILE", "DOROTI_STAGE_TRACE", "DOROTI_LAYOUT_PROFILE", "DOROTI_ALLOCATION_PROFILE", "DOROTI_FRAME_TRACE_CAPACITY" })
            if (Intent?.GetStringExtra(flag) is { } enabled)
                Environment.SetEnvironmentVariable(flag, enabled);
        if (savedInstanceState is null && Intent?.Action == Android.Content.Intent.ActionView && Intent.DataString is { } location)
            TryDeliver(location, true);
        base.OnCreate(savedInstanceState);
    }
    protected override void OnNewIntent(Android.Content.Intent? intent)
    {
        base.OnNewIntent(intent);
        if (intent?.GetStringExtra("DOROTI_SCROLL_GC") == "1")
            AndroidScrollProfile.CollectGarbageForDiagnostics("intent");
        if (intent?.GetStringExtra("DOROTI_WORK_TRACE_START") == "1" && AndroidScrollProfile.Enabled && !FrameworkWorkTrace.Enabled)
            FrameworkWorkTrace.Start();
        if (intent?.GetStringExtra("DOROTI_SCROLL_EXPORT") == "1")
            AndroidScrollProfile.Export(this);
        if (AndroidScrollProfile.Enabled)
        {
            if (intent?.GetStringExtra("DOROTI_SCROLL_LIFECYCLE") == "recreate") Recreate();
            if (intent?.GetStringExtra("DOROTI_SCROLL_LIFECYCLE") == "finish") Finish();
        }
        if (intent?.Action == Android.Content.Intent.ActionView && intent.DataString is { } location)
            TryDeliver(location, false);
    }
    public override void OnTrimMemory(Android.Content.TrimMemory level)
    {
        base.OnTrimMemory(level);
        if (level >= Android.Content.TrimMemory.Background
            || (level > 0 && level < Android.Content.TrimMemory.UiHidden))
            (Microsoft.Maui.Controls.Application.Current as DorotiMauiApplication)?.TrimAndroidGpuMemory();
        if (AndroidScrollProfile.Enabled && Intent?.GetStringExtra("DOROTI_SCROLL_GC_ON_TRIM") == "1")
            AndroidScrollProfile.CollectGarbageForDiagnostics("trim-" + (int)level);
    }
    private static void TryDeliver(string location, bool cold)
    {
        try { MauiApplicationActivation.Deliver(location, ApplicationActivationSource.AndroidIntent, cold); }
        catch (ArgumentException) { /* Invalid external links leave the current route unchanged. */ }
    }
}
#endif
