#if ANDROID
using Android.Views;
using Doroti.Ui;
using Rect = Doroti.Ui.Rect;

namespace Doroti.Host.Maui;

public sealed partial class MauiTextInputBridge
{
    private AndroidTextMagnifierSession? _androidMagnifier;

    internal ITextMagnifierSession? CreateMagnifierSession()
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(28) || _visualHost is null || _disposed) return null;
        var session = new AndroidTextMagnifierSession(this);
        DispatchInputMutation(() =>
        {
            ResetAndroidMagnifier();
            if (_disposed || _suspended) { session.DisposeCore(); return; }
            _androidMagnifier = session;
        });
        return session;
    }

    private void ResetAndroidMagnifier()
    {
        _androidMagnifier?.DisposeCore();
        _androidMagnifier = null;
    }

    private sealed class AndroidTextMagnifierSession(MauiTextInputBridge owner) : ITextMagnifierSession
    {
        private Android.Widget.Magnifier? _magnifier;
        private DorotiAndroidVulkanView? _view;
        private DetachListener? _detach;
        private readonly int[] _hostLocation = new int[2], _surfaceLocation = new int[2];
        private bool _disposed;

        public void Update(Offset position, Rect caretRect) => owner.DispatchInputMutation(() =>
        {
            if (!OperatingSystem.IsAndroidVersionAtLeast(28) || _disposed || owner._disposed
                || owner._suspended || !ReferenceEquals(owner._androidMagnifier, this)) return;
            // The transparent IME EditText is not the content being magnified.
            // Android copies this SurfaceView asynchronously and owns the popup,
            // avoiding a matrix backdrop across independent native raster slices.
            DorotiAndroidVulkanView? surface = null;
            if (owner._visualHost is { } host)
                foreach (var child in host.Children)
                    if (child.Handler?.PlatformView is DorotiAndroidViewContainer container)
                    { surface = container.Surface; break; }
            if (surface is null || !surface.IsAttachedToWindow || surface.Holder?.Surface?.IsValid != true
                || owner._visualHost?.Handler?.PlatformView is not Android.Views.View hostView)
            { HideCore(); return; }
            if (!ReferenceEquals(surface, _view))
            {
                HideCore();
                _view = surface;
                _detach = new DetachListener(HideCore);
                surface.AddOnAttachStateChangeListener(_detach);
                if (OperatingSystem.IsAndroidVersionAtLeast(29))
                {
                    using var builder = new Android.Widget.Magnifier.Builder(surface);
                    builder.SetInitialZoom(1.25f);
                    _magnifier = builder.Build();
                }
                else _magnifier = new Android.Widget.Magnifier(surface);
                AndroidScrollProfile.RecordMagnifierCreated();
            }
            var scale = surface.Resources?.DisplayMetrics?.Density ?? 1;
            hostView.GetLocationInWindow(_hostLocation);
            surface.GetLocationInWindow(_surfaceLocation);
            var x = position.dx * scale + _hostLocation[0] - _surfaceLocation[0];
            var y = caretRect.center.dy * scale + _hostLocation[1] - _surfaceLocation[1];
            _magnifier!.Show((float)Math.Clamp(x, 0, Math.Max(0, surface.Width - 1)),
                (float)Math.Clamp(y, 0, Math.Max(0, surface.Height - 1)));
            AndroidScrollProfile.RecordMagnifierUpdate();
        });

        public void Hide() => owner.DispatchInputMutation(HideCore);

        private void HideCore()
        {
            if (_magnifier is not null)
            {
                if (OperatingSystem.IsAndroidVersionAtLeast(28)) _magnifier.Dismiss();
                _magnifier.Dispose();
                _magnifier = null;
                AndroidScrollProfile.RecordMagnifierDismissed();
            }
            if (_view is not null && _detach is not null) _view.RemoveOnAttachStateChangeListener(_detach);
            _detach?.Dispose();
            _detach = null;
            _view = null;
        }

        public void Dispose() => owner.DispatchInputMutation(() =>
        {
            DisposeCore();
            if (ReferenceEquals(owner._androidMagnifier, this)) owner._androidMagnifier = null;
        });

        internal void DisposeCore()
        {
            _disposed = true;
            HideCore();
        }

        private sealed class DetachListener(Action detached) : Java.Lang.Object, Android.Views.View.IOnAttachStateChangeListener
        {
            public void OnViewAttachedToWindow(Android.Views.View? view) { }
            public void OnViewDetachedFromWindow(Android.Views.View? view) => detached();
        }
    }
}
#endif
