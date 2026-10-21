#if ANDROID
using Android.Content;
using AndroidX.Activity;

namespace Doroti.Host.Maui;

/// <summary>Lifecycle-bound AndroidX callback; disabled at the framework's root route.</summary>
internal sealed class MauiAndroidBackDispatcher : IDisposable
{
    private readonly Microsoft.Maui.Controls.View _element;
    private readonly AndroidBackNavigation _navigation;
    private ComponentActivity? _activity;
    private BackCallback? _callback;

    internal MauiAndroidBackDispatcher(Microsoft.Maui.Controls.View element, AndroidBackNavigation navigation)
    {
        _element = element;
        _navigation = navigation;
        _element.HandlerChanged += Rebind;
        _element.Loaded += Rebind;
        _element.Unloaded += Detach;
        _navigation.HandlesBackChanged += UpdateEnabled;
        _navigation.SystemPopRequested += PopSystem;
        Rebind(null, EventArgs.Empty);
    }

    private void Rebind(object? sender, EventArgs args)
    {
        // The render view can survive Activity recreation. Prefer its current
        // MAUI Window over the Context captured by an older native view.
        Context? context = _element.Window?.Handler?.PlatformView as Android.App.Activity
            ?? (_element.Handler?.PlatformView as Android.Views.View)?.Context;
        while (context is ContextWrapper wrapper && context is not Android.App.Activity)
            context = wrapper.BaseContext;
        var activity = context as ComponentActivity;
        if (ReferenceEquals(activity, _activity)) return;
        Detach(null, EventArgs.Empty);
        if (activity is null || activity.IsDestroyed) return;
        _activity = activity;
        _callback = new BackCallback(_navigation);
        activity.OnBackPressedDispatcher.AddCallback(activity, _callback);
        UpdateEnabled(_navigation.FrameworkHandlesBack);
    }

    private void UpdateEnabled(bool enabled)
    {
        if (_callback is { } callback) callback.Enabled = enabled;
    }

    private void PopSystem()
    {
        if (_activity is not { IsDestroyed: false } activity || _callback is not { } callback) return;
        // The framework has exhausted its routes. Step aside for the native
        // Activity/task behavior, without redispatching popRoute recursively.
        callback.Enabled = false;
        try { activity.OnBackPressedDispatcher.OnBackPressed(); }
        finally
        {
            if (ReferenceEquals(_callback, callback)) callback.Enabled = _navigation.FrameworkHandlesBack;
        }
    }

    private void Detach(object? sender, EventArgs args)
    {
        _callback?.Remove();
        _callback?.Dispose();
        _callback = null;
        _activity = null;
    }

    public void Dispose()
    {
        _element.HandlerChanged -= Rebind;
        _element.Loaded -= Rebind;
        _element.Unloaded -= Detach;
        _navigation.HandlesBackChanged -= UpdateEnabled;
        _navigation.SystemPopRequested -= PopSystem;
        Detach(null, EventArgs.Empty);
    }

    private sealed class BackCallback(AndroidBackNavigation navigation) : OnBackPressedCallback(false)
    {
        public override void HandleOnBackPressed() => navigation.RequestBack();
    }
}
#endif
