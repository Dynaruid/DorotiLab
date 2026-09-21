#if ANDROID
using Android.Views;
using Microsoft.Maui.Controls;
using Doroti.Ui;
using View = Microsoft.Maui.Controls.View;

namespace Doroti.Host.Maui;

internal sealed class AndroidPointerSubscription : IDisposable
{
    private readonly View _view;
    private readonly Action<MauiSurfacePointerData> _dispatch;
    private readonly AndroidTrackpadGesture _gesture;
    private Android.Views.View? _native;

    internal AndroidPointerSubscription(View view, Action<MauiSurfacePointerData> dispatch)
    {
        _view = view;
        _dispatch = dispatch;
        _gesture = new(dispatch);
        _view.HandlerChanged += Changed;
        _view.Unfocused += Unfocused;
        _view.Unloaded += Unloaded;
        Attach();
    }

    private void Changed(object? sender, EventArgs args) => Attach();

    private void Unfocused(object? sender, FocusEventArgs args) =>
        _gesture.Cancel(DorotiFrameClock.Now);

    private void Unloaded(object? sender, EventArgs args) => _gesture.Cancel(DorotiFrameClock.Now);

    private void Attach()
    {
        Detach();
        if (_view.Handler?.PlatformView is not Android.Views.View native)
        {
            return;
        }

        _native = native;
        native.Touch += Touch;
        native.GenericMotion += Generic;
    }

    private void Touch(object? sender, Android.Views.View.TouchEventArgs args)
    {
        var e = args.Event;
        if (e is null)
        {
            return;
        }

        var change = e.ActionMasked switch
        {
            MotionEventActions.Down or MotionEventActions.PointerDown => PointerChange.down,
            MotionEventActions.Move => PointerChange.move,
            MotionEventActions.Up or MotionEventActions.PointerUp => PointerChange.up,
            MotionEventActions.Cancel => PointerChange.cancel,
            _ => (PointerChange?)null,
        };
        if (change is null)
        {
            return;
        }

        for (var i = 0; i < e.PointerCount; i++)
        {
            if (change is not PointerChange.move and not PointerChange.cancel && i != e.ActionIndex)
            {
                continue;
            }

            var device = AndroidPointerMapping.DeviceIdentifier(e.DeviceId, e.GetPointerId(i));
            var kind = AndroidPointerMapping.Kind((int)e.GetToolType(i));
            if (
                _gesture.Handle(
                    device,
                    change.Value,
                    kind,
                    e.Source == InputSourceType.Mouse,
                    (int)e.ButtonState,
                    e.GetX(i),
                    e.GetY(i),
                    DorotiFrameClock.Now
                )
            )
            {
                continue;
            }

            _dispatch(
                new(
                    DorotiFrameClock.Now,
                    change.Value,
                    kind,
                    device,
                    e.GetX(i),
                    e.GetY(i),
                    AndroidPointerMapping.Buttons(kind, (int)e.ButtonState),
                    0,
                    0,
                    PointerSignalKind.none,
                    e.GetPressure(i),
                    Orientation: e.GetOrientation(i),
                    Tilt: e.GetAxisValue(Axis.Tilt, i)
                )
            );
        }
        args.Handled = true;
    }

    private void Generic(object? sender, Android.Views.View.GenericMotionEventArgs args)
    {
        var e = args.Event;
        if (e is null || _native is null || e.PointerCount == 0)
        {
            return;
        }

        var change = e.ActionMasked switch
        {
            MotionEventActions.HoverEnter => PointerChange.add,
            MotionEventActions.HoverExit => PointerChange.remove,
            MotionEventActions.HoverMove or MotionEventActions.Scroll => PointerChange.hover,
            _ => (PointerChange?)null,
        };
        if (change is null)
        {
            return;
        }

        var scroll = e.ActionMasked == MotionEventActions.Scroll;
        var config = Android.Views.ViewConfiguration.Get(_native.Context!);
        var xFactor = OperatingSystem.IsAndroidVersionAtLeast(26)
            ? config!.ScaledHorizontalScrollFactor
            : 48;
        var yFactor = OperatingSystem.IsAndroidVersionAtLeast(26)
            ? config!.ScaledVerticalScrollFactor
            : 48;
        var kind = AndroidPointerMapping.Kind((int)e.GetToolType(0));
        var device = AndroidPointerMapping.DeviceIdentifier(e.DeviceId, e.GetPointerId(0));
        _dispatch(
            new(
                DorotiFrameClock.Now,
                change.Value,
                kind,
                device,
                e.GetX(),
                e.GetY(),
                AndroidPointerMapping.Buttons(kind, (int)e.ButtonState),
                scroll ? -e.GetAxisValue(Axis.Hscroll) * xFactor : 0,
                scroll ? -e.GetAxisValue(Axis.Vscroll) * yFactor : 0,
                scroll ? PointerSignalKind.scroll : PointerSignalKind.none,
                e.GetPressure(0),
                Orientation: e.GetOrientation(0),
                Tilt: e.GetAxisValue(Axis.Tilt, 0)
            )
        );
        args.Handled = true;
    }

    private void Detach()
    {
        _gesture.Cancel(DorotiFrameClock.Now);
        if (_native is not null)
        {
            _native.Touch -= Touch;
            _native.GenericMotion -= Generic;
        }
        _native = null;
    }

    public void Dispose()
    {
        _view.HandlerChanged -= Changed;
        _view.Unfocused -= Unfocused;
        _view.Unloaded -= Unloaded;
        Detach();
    }
}
#endif
