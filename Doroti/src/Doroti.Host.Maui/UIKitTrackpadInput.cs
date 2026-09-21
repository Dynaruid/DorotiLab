#if IOS || MACCATALYST
using CoreGraphics;
using Doroti.Ui;
using Foundation;
using Microsoft.Maui.Controls;
using UIKit;

namespace Doroti.Host.Maui;

// FlutterViewController.mm: continuousScrollEvent / discreteScrollEvent /
// pinchEvent. Empty AllowedTouchTypes keeps direct touchscreen gestures in
// Doroti's pointer recognizers rather than recognizing them a second time.
internal sealed class UIKitTrackpadInput : IDisposable
{
    private readonly View _view;
    private readonly Action<MauiSurfacePointerData> _dispatch;
    private readonly GestureDelegate _delegate = new();
    private readonly NSObject _deactivation;
    private UIView? _native;
    private UIPanGestureRecognizer? _pan,
        _wheel;
    private UIPinchGestureRecognizer? _pinch;
    private UIRotationGestureRecognizer? _rotation;
    private UIHoverGestureRecognizer? _hover;
    private MauiTrackpadGesture? _panStream,
        _pinchStream;
    private CGPoint _lastWheel,
        _lastHover;
    private TimeSpan _inertiaCancelAfter = TimeSpan.MaxValue;
    private TimeSpan _inertiaDeadline;
    private int _pinchParts;

    internal UIKitTrackpadInput(View view, Action<MauiSurfacePointerData> dispatch)
    {
        _view = view;
        _dispatch = dispatch;
        _view.HandlerChanged += OnHandlerChanged;
        _deactivation = UIApplication.Notifications.ObserveWillResignActive((_, _) => Cancel());
        Attach();
    }

    private void OnHandlerChanged(object? sender, EventArgs args) => Attach();

    private static ulong Device(UIGestureRecognizer recognizer) =>
        unchecked((ulong)(nint)recognizer.Handle);

    private double Ratio => (double)(_native?.ContentScaleFactor ?? 1);

    private void Begin(MauiTrackpadGesture stream, UIGestureRecognizer recognizer)
    {
        var point = recognizer.LocationInView(_native);
        stream.Begin(DorotiFrameClock.Now, point.X * Ratio, point.Y * Ratio);
    }

    private void Attach()
    {
        Detach();
        if (_view.Handler?.PlatformView is not UIView native)
        {
            return;
        }

        _native = native;
        _pan = new UIPanGestureRecognizer(Continuous)
        {
            AllowedScrollTypesMask = UIScrollTypeMask.Continuous,
        };
        _wheel = new UIPanGestureRecognizer(Discrete)
        {
            AllowedScrollTypesMask = UIScrollTypeMask.Discrete,
        };
        _pinch = new UIPinchGestureRecognizer(recognizer => Pinch(recognizer, 1));
        _rotation = new UIRotationGestureRecognizer(recognizer => Pinch(recognizer, 2));
        _panStream = new(Device(_pan), _dispatch);
        _pinchStream = new(Device(_pinch), _dispatch);
        _delegate.OnScroll = () =>
        {
            if (DorotiFrameClock.Now < _inertiaDeadline)
            {
                _panStream.CancelInertia(DorotiFrameClock.Now);
            }

            _inertiaDeadline = TimeSpan.Zero;
        };
        foreach (var recognizer in new UIGestureRecognizer[] { _pan, _wheel, _pinch, _rotation })
        {
            recognizer.AllowedTouchTypes = [];
            recognizer.CancelsTouchesInView = false;
            recognizer.DelaysTouchesBegan = false;
            recognizer.DelaysTouchesEnded = false;
            recognizer.Delegate = _delegate;
            native.AddGestureRecognizer(recognizer);
        }
        _hover = new UIHoverGestureRecognizer(recognizer =>
        {
            var location = recognizer.LocationInView(native);
            if (location == _lastHover && DorotiFrameClock.Now > _inertiaCancelAfter)
            {
                _panStream.CancelInertia(DorotiFrameClock.Now);
                _inertiaCancelAfter = TimeSpan.MaxValue;
            }
            _lastHover = location;
        })
        {
            Delegate = _delegate,
        };
        native.AddGestureRecognizer(_hover);
    }

    private void Continuous(UIPanGestureRecognizer recognizer)
    {
        if (_panStream is null || _native is null)
        {
            return;
        }

        var time = DorotiFrameClock.Now;
        switch (recognizer.State)
        {
            case UIGestureRecognizerState.Began:
                _inertiaCancelAfter = TimeSpan.MaxValue;
                Begin(_panStream, recognizer);
                break;
            case UIGestureRecognizerState.Changed:
                Begin(_panStream, recognizer);
                var translation = recognizer.TranslationInView(_native);
                _panStream.Update(time, translation.X * Ratio, translation.Y * Ratio);
                break;
            case UIGestureRecognizerState.Ended:
            case UIGestureRecognizerState.Cancelled:
            case UIGestureRecognizerState.Failed:
                _panStream.End(time);
                _inertiaCancelAfter = time + TimeSpan.FromMilliseconds(100);
                var velocity = recognizer.VelocityInView(_native);
                _inertiaDeadline =
                    time
                    + TimeSpan.FromSeconds(
                        Math.Max(
                            0,
                            (
                                .1821
                                * Math.Log(
                                    Math.Max(
                                        1,
                                        Math.Max(Math.Abs(velocity.X), Math.Abs(velocity.Y))
                                    )
                                )
                            ) - .4825
                        )
                    );
                break;
        }
    }

    private void Pinch(UIGestureRecognizer recognizer, int part)
    {
        if (_pinchStream is null)
        {
            return;
        }

        if (recognizer.State is UIGestureRecognizerState.Began or UIGestureRecognizerState.Changed)
        {
            _pinchParts |= part;
            Begin(_pinchStream, recognizer);
            if (recognizer.State == UIGestureRecognizerState.Changed)
            {
                _pinchStream.Update(
                    DorotiFrameClock.Now,
                    0,
                    0,
                    (double)(_pinch?.Scale ?? 1),
                    (double)(_rotation?.Rotation ?? 0)
                );
            }
        }
        else if (
            recognizer.State
            is UIGestureRecognizerState.Ended
                or UIGestureRecognizerState.Cancelled
                or UIGestureRecognizerState.Failed
        )
        {
            _pinchParts &= ~part;
            if (_pinchParts == 0)
            {
                _pinchStream.End(DorotiFrameClock.Now);
                if (_pinch is not null)
                {
                    _pinch.Scale = 1;
                }

                if (_rotation is not null)
                {
                    _rotation.Rotation = 0;
                }
            }
        }
    }

    private void Discrete(UIPanGestureRecognizer recognizer)
    {
        if (_native is null)
        {
            return;
        }

        var translation = recognizer.TranslationInView(_native);
        var position = recognizer.LocationInView(_native);
        var x = (translation.X - _lastWheel.X) * Ratio;
        var y = -(translation.Y - _lastWheel.Y) * Ratio;
        if (
            recognizer.State
            is UIGestureRecognizerState.Began
                or UIGestureRecognizerState.Changed
                or UIGestureRecognizerState.Ended
        )
        {
            _dispatch(
                new(
                    DorotiFrameClock.Now,
                    PointerChange.hover,
                    PointerDeviceKind.mouse,
                    Device(recognizer),
                    position.X * Ratio,
                    position.Y * Ratio,
                    0,
                    x,
                    y,
                    PointerSignalKind.scroll,
                    0
                )
            );
        }

        _lastWheel = recognizer.State
            is UIGestureRecognizerState.Ended
                or UIGestureRecognizerState.Cancelled
                or UIGestureRecognizerState.Failed
            ? CGPoint.Empty
            : translation;
    }

    private void Cancel()
    {
        _panStream?.Remove(DorotiFrameClock.Now);
        _pinchStream?.Remove(DorotiFrameClock.Now);
        _pinchParts = 0;
        _lastWheel = CGPoint.Empty;
        _inertiaCancelAfter = TimeSpan.MaxValue;
        _inertiaDeadline = TimeSpan.Zero;
    }

    private void Detach()
    {
        Cancel();
        foreach (
            var recognizer in new UIGestureRecognizer?[] { _pan, _wheel, _pinch, _rotation, _hover }
        )
        {
            if (recognizer is null)
            {
                continue;
            }

            _native?.RemoveGestureRecognizer(recognizer);
            recognizer.Dispose();
        }
        _pan = _wheel = null;
        _pinch = null;
        _rotation = null;
        _hover = null;
        _native = null;
    }

    public void Dispose()
    {
        _view.HandlerChanged -= OnHandlerChanged;
        Detach();
        _deactivation.Dispose();
        _delegate.Dispose();
    }

    private sealed class GestureDelegate : UIGestureRecognizerDelegate
    {
        internal Action? OnScroll;

        public override bool ShouldRecognizeSimultaneously(
            UIGestureRecognizer gestureRecognizer,
            UIGestureRecognizer otherGestureRecognizer
        ) => true;

        public override bool ShouldReceiveEvent(
            UIGestureRecognizer gestureRecognizer,
            UIEvent @event
        )
        {
            if (@event.Type == UIEventType.Scroll)
            {
                OnScroll?.Invoke();
            }

            return true;
        }
    }
}
#endif
