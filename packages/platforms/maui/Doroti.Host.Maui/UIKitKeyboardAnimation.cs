#if IOS || MACCATALYST
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Doroti.Host.Maui;

/// <summary>Track UIKit's keyboard curve, including its spring curve (7).</summary>
internal sealed class UIKitKeyboardAnimation(UIView view, Action<CGRect> changed) : IDisposable
{
    private readonly UIView _probe = new() { Hidden = true, UserInteractionEnabled = false };
    private CADisplayLink? _displayLink;
    private CABasicAnimation? _animation;
    private CGRect _initial;
    private CGRect _target;
    private double _start;
    private double _duration;

    internal bool IsAnimating => _displayLink is not null;

    internal void ChangeFrame(CGRect initial, CGRect target, double duration, int curve)
    {
        // WillChangeFrame and WillHide can describe the same transition.
        // Restarting from our last sample delays the second half of dismissal.
        if (duration > 0 && IsAnimating && _target.Equals(target)) return;
        Stop();
        if (duration <= 0 || target.IsEmpty || initial.Equals(target))
        {
            changed(target);
            return;
        }

        _initial = initial;
        _target = target;
        _duration = duration;
        view.AddSubview(_probe);
        UIView.PerformWithoutAnimation(() => _probe.Frame = CGRect.Empty);
        _start = CAAnimation.CurrentMediaTime();
        // Let UIKit construct the native timing function/spring. Approximating
        // curve 7 with smoothstep visibly trails the keyboard as it closes.
        UIView.Animate(duration, 0, (UIViewAnimationOptions)((ulong)curve << 16),
            () => _probe.Frame = new CGRect(0, 1, 0, 0), () => { });
        _animation = _probe.Layer.AnimationForKey("position") as CABasicAnimation;
        _displayLink = CADisplayLink.Create(Tick);
        _displayLink.AddToRunLoop(NSRunLoop.Main, NSRunLoopMode.Common);
    }

    private void Tick()
    {
        if (view.Window is null)
        {
            Stop();
            return;
        }
        // Sample where the keyboard will be when our next frame is displayed,
        // rather than where it was when this callback began.
        var displayTime = _displayLink!.TargetTimestamp;
        var elapsed = Math.Max(0, displayTime - _start);
        var duration = _animation is { Duration: > 0 } animation ? animation.Duration : _duration;
        if (_animation is { BeginTime: > 0 } timed)
            elapsed = Math.Max(0,
                (_probe.Layer.ConvertTimeFromLayer(displayTime, null) - timed.BeginTime)
                    * timed.Speed + timed.TimeOffset);
        if (elapsed >= duration)
        {
            var target = _target;
            Stop();
            changed(target);
            return;
        }

        double progress;
        if (_animation is CASpringAnimation spring)
        {
            progress = UIKitAnimationTiming.Spring(elapsed,
                spring.Mass, spring.Stiffness, spring.Damping, spring.InitialVelocity);
        }
        else if (_animation?.TimingFunction is { } timing)
        {
            var first = timing.GetControlPoint(1);
            var second = timing.GetControlPoint(2);
            progress = UIKitAnimationTiming.Cubic(elapsed / duration,
                first.X, first.Y, second.X, second.Y);
        }
        else
        {
            // Preserve native presentation timing for unsupported animations.
            progress = _probe.Layer.PresentationLayer?.Frame.Y ?? 0;
        }
        changed(new CGRect(
            _initial.X + (_target.X - _initial.X) * progress,
            _initial.Y + (_target.Y - _initial.Y) * progress,
            _initial.Width + (_target.Width - _initial.Width) * progress,
            _initial.Height + (_target.Height - _initial.Height) * progress));
    }

    internal void Stop()
    {
        _displayLink?.Invalidate();
        _displayLink?.Dispose();
        _displayLink = null;
        _animation?.Dispose();
        _animation = null;
        _probe.Layer.RemoveAllAnimations();
        _probe.RemoveFromSuperview();
    }

    public void Dispose()
    {
        Stop();
        _probe.Dispose();
    }
}
#endif
