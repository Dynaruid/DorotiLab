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

    private sealed class AndroidTextMagnifierSession : ITextMagnifierSession
    {
        private readonly MauiTextInputBridge owner;
        private readonly Java.Lang.Runnable _applyCallback;
        private Android.Widget.Magnifier? _magnifier;
        private DorotiAndroidVulkanView? _view;
        private Android.Views.View? _postedOn;
        private DetachListener? _detach;
        private readonly int[] _hostLocation = new int[2], _surfaceLocation = new int[2];
        private bool _disposed;
        private Offset _position;
        private Rect _caret = Rect.zero;

        internal AndroidTextMagnifierSession(MauiTextInputBridge owner)
        {
            this.owner = owner;
            _applyCallback = new Java.Lang.Runnable(ApplyPending);
        }

        public void Update(Offset position, Rect caretRect) => owner.DispatchInputMutation(() =>
        {
            if (!OperatingSystem.IsAndroidVersionAtLeast(28) || _disposed || owner._disposed
                || owner._suspended || !ReferenceEquals(owner._androidMagnifier, this)) return;
            _position = position;
            _caret = caretRect;
            AndroidScrollProfile.RecordMagnifierRequest(_postedOn is not null);
            if (_postedOn is not null) return;
            if (owner._visualHost?.Handler?.PlatformView is not Android.Views.View host || !host.IsAttachedToWindow) return;
            _postedOn = host;
            host.PostOnAnimation(_applyCallback);
        });

        private void ApplyPending()
        {
            _postedOn = null;
            if (!OperatingSystem.IsAndroidVersionAtLeast(28) || _disposed || owner._disposed
                || owner._suspended || !ReferenceEquals(owner._androidMagnifier, this)) return;
            // The transparent IME EditText is not the content being magnified.
            // Android copies this SurfaceView asynchronously and owns the popup,
            // avoiding a matrix backdrop across independent native raster slices.
            var surface = _view;
            if (surface is null && owner._visualHost is { } visualHost)
                foreach (var child in visualHost.Children)
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
                    var density = surface.Resources?.DisplayMetrics?.Density ?? 1;
                    using var builder = new Android.Widget.Magnifier.Builder(surface);
                    builder.SetInitialZoom(1.25f);
                    // Match Material's 77.37 x 37.9 logical-pixel capsule,
                    // instead of Android's almost rectangular theme default.
                    var height = Math.Max(1, (int)Math.Round(37.9 * density));
                    builder.SetSize(Math.Max(1, (int)Math.Round(77.37 * density)), height);
                    builder.SetCornerRadius(height / 2f);
                    builder.SetElevation(3 * density);
                    builder.SetDefaultSourceToMagnifierOffset(0, -(int)Math.Round((37.9 / 2 + 22) * density));
                    using var tint = new Android.Graphics.Drawables.ColorDrawable(new Android.Graphics.Color(0x089E9E9E));
                    builder.SetOverlay(tint);
                    _magnifier = builder.Build();
                }
                else _magnifier = new Android.Widget.Magnifier(surface);
                AndroidScrollProfile.RecordMagnifierCreated();
            }
            var scale = surface.Resources?.DisplayMetrics?.Density ?? 1;
            hostView.GetLocationInWindow(_hostLocation);
            surface.GetLocationInWindow(_surfaceLocation);
            var x = _position.dx * scale + _hostLocation[0] - _surfaceLocation[0];
            var y = _caret.center.dy * scale + _hostLocation[1] - _surfaceLocation[1];
            _magnifier!.Show((float)Math.Clamp(x, 0, Math.Max(0, surface.Width - 1)),
                (float)Math.Clamp(y, 0, Math.Max(0, surface.Height - 1)));
            AndroidScrollProfile.RecordMagnifierUpdate();
        }

        public void Hide() => owner.DispatchInputMutation(HideCore);

        private void HideCore()
        {
            if (_postedOn is { } posted)
            {
                posted.RemoveCallbacks(_applyCallback);
                _postedOn = null;
                AndroidScrollProfile.RecordMagnifierCancelled();
            }
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
            if (_disposed) return;
            _disposed = true;
            HideCore();
            // Choreographer may already have snapshotted this Runnable before
            // RemoveCallbacks. Keep its JNI peer alive for a harmless late Run;
            // the disposed guard prevents UI work and the JNI GC bridge retires it.
        }

        private sealed class DetachListener(Action detached) : Java.Lang.Object, Android.Views.View.IOnAttachStateChangeListener
        {
            public void OnViewAttachedToWindow(Android.Views.View? view) { }
            public void OnViewDetachedFromWindow(Android.Views.View? view) => detached();
        }
    }
}
#endif
