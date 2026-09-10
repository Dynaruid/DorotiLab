#if ANDROID
using System.Diagnostics.CodeAnalysis;
using Android.Database;
using Android.OS;
using Android.Runtime;

namespace Doroti.Host.Maui;

internal sealed class MauiSettingsObserver : ContentObserver
{
    private Handler? _handler;
    private Action? _changed;

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(MauiSettingsObserver))]
    internal MauiSettingsObserver(Action changed) : this(new Handler(Looper.MainLooper!), changed) { }

    private MauiSettingsObserver(Handler handler, Action changed) : base(handler)
    {
        _handler = handler;
        _changed = changed;
    }

    // Unregistering does not retract notifications already dispatched by Android.
    // A late Java callback may recreate the managed peer after Dispose; it must
    // be inert and must not retain or refresh the detached view environment.
    private MauiSettingsObserver(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership) { }

    public override void OnChange(bool selfChange) => _changed?.Invoke();

    protected override void Dispose(bool disposing)
    {
        _changed = null;
        if (disposing && _handler is { } handler)
        {
            _handler = null;
            // This handler belongs only to this observer, never to the view or
            // another listener. Cancel its queued work before releasing the peer.
            handler.RemoveCallbacksAndMessages(null);
            handler.Dispose();
        }
        base.Dispose(disposing);
    }
}
#endif
