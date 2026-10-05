#if IOS && !MACCATALYST
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Doroti.Host.Maui;

internal interface IUIKitAnimatedViewport
{
    UIKitAnimatedViewport AnimatedViewport { get; }
}

/// <summary>
/// UIKit commits the destination bounds before its rotation animation starts.
/// Render at the presentation bounds until that animation finishes, so the
/// framework lays out each intermediate size instead of jumping to the destination.
/// </summary>
internal sealed class UIKitAnimatedViewport(
    UIView view,
    Action<CGSize> render,
    Func<bool>? canRender = null,
    Func<bool>? canFinish = null
) : IDisposable
{
    private CADisplayLink? _displayLink;
    private CGSize _initialSize;
    private UIEdgeInsets _initialInsets;
    private nfloat _scale;
    private double _deadline;
    private long _generation;
    private bool _disposed;
    private bool _renderedWhileAnimating;

    internal CGSize Size { get; private set; }
    internal CGSize AnimationExtent =>
        new(
            Math.Max(_initialSize.Width, view.Bounds.Width),
            Math.Max(_initialSize.Height, view.Bounds.Height)
        );
    internal nfloat Scale => view.Window?.Screen.Scale ?? view.ContentScaleFactor;
    internal UIEdgeInsets SafeAreaInsets { get; private set; }
    internal bool IsAnimating => _displayLink is not null;
    internal double FrameTargetTimestamp { get; private set; }
    internal double FrameInterval { get; private set; }
    internal float PreferredFrameRate => _displayLink?.PreferredFrameRateRange.Preferred ?? 0;
    internal event Action<bool>? AnimationChanged;
    internal event Action? Changed;

    internal void LayoutChanged()
    {
        if (_disposed || view.Window is null || !IsValid(view.Bounds.Size))
        {
            Stop();
            return;
        }
        if (_displayLink is not null)
        {
            return;
        }

        var coordinator = FindTransition();
        var duration = coordinator?.TransitionDuration ?? UIView.InheritedAnimationDuration;
        // MAUI can arrange the Metal child after UIKit's animation block has
        // returned. Its presentation layer still carries the in-flight bounds
        // animation even though the inherited duration/coordinator is absent.
        var presentationSize = view.Layer.PresentationLayer?.Bounds.Size;
        var presentationIsResizing =
            presentationSize is { } presented
            && IsValid(presented)
            && !presented.Equals(view.Bounds.Size);
        if (
            !IsValid(Size)
            || (Size.Equals(view.Bounds.Size) && SafeAreaInsets.Equals(view.SafeAreaInsets))
            || (duration <= 0 && !presentationIsResizing)
            || UIAccessibility.IsReduceMotionEnabled
        )
        {
            Render(view.Bounds.Size, view.SafeAreaInsets);
            return;
        }

        _initialSize = Size;
        _initialInsets = SafeAreaInsets;
        // The fallback is a cleanup deadline, not an invented animation curve.
        // Presentation bounds remain the clock and stop the loop when settled.
        _deadline = CAAnimation.CurrentMediaTime() + Math.Max(1, duration + 0.5);
        var generation = ++_generation;
        _displayLink = CADisplayLink.Create(Tick);
        // Let UIKit choose the cadence for the short rotation transition from
        // the display and system policy, without imposing an application cap.
        _displayLink.AddToRunLoop(NSRunLoop.Main, NSRunLoopMode.Common);
        AnimationChanged?.Invoke(true);
        coordinator?.AnimateAlongsideTransition(
            _ => { },
            _ =>
            {
                if (!_disposed && generation == _generation)
                {
                    Finish();
                }
            }
        );
    }

    private IUIViewControllerTransitionCoordinator? FindTransition()
    {
        for (
            UIResponder? responder = view;
            responder is not null;
            responder = responder.NextResponder
        )
        {
            if (
                responder is UIViewController controller
                && controller.GetTransitionCoordinator() is { IsAnimated: true } coordinator
                && !coordinator.TargetTransform().IsIdentity
            )
            {
                return coordinator;
            }
        }
        return null;
    }

    private void Tick()
    {
        if (_disposed || view.Window is null)
        {
            Stop();
            return;
        }
        if (CAAnimation.CurrentMediaTime() >= _deadline || UIAccessibility.IsReduceMotionEnabled)
        {
            Finish(force: true);
            return;
        }

        if (canRender?.Invoke() == false)
        {
            return;
        }

        // Presentation bounds include UIKit's timing curve and interruptions.
        // Never animate the view's model bounds or feed them back into MAUI layout.
        FrameTargetTimestamp = _displayLink!.TargetTimestamp;
        FrameInterval = FrameTargetTimestamp - _displayLink.Timestamp;
        var size =
            UIKitBoundsAnimation.Sample(view.Layer, FrameTargetTimestamp)
            ?? view.Layer.PresentationLayer?.Bounds.Size
            ?? Size;
        if (!IsValid(size))
        {
            return;
        }
        var target = view.Bounds.Size;
        if (size.Equals(target))
        {
            Finish();
            return;
        }
        var dx = (double)(target.Width - _initialSize.Width);
        var dy = (double)(target.Height - _initialSize.Height);
        var distance = dx * dx + dy * dy;
        var progress =
            distance > 0
                ? Math.Clamp(
                    (
                        ((double)size.Width - _initialSize.Width) * dx
                        + ((double)size.Height - _initialSize.Height) * dy
                    ) / distance,
                    0,
                    1
                )
                : 1;
        var insets = view.SafeAreaInsets;
        Render(
            size,
            new UIEdgeInsets(
                Lerp(_initialInsets.Top, insets.Top, progress),
                Lerp(_initialInsets.Left, insets.Left, progress),
                Lerp(_initialInsets.Bottom, insets.Bottom, progress),
                Lerp(_initialInsets.Right, insets.Right, progress)
            )
        );
    }

    private void Render(CGSize size, UIEdgeInsets insets)
    {
        if (!IsValid(size))
        {
            return;
        }
        if (
            Size.Equals(size)
            && SafeAreaInsets.Equals(insets)
            && _scale == Scale
            && _renderedWhileAnimating == IsAnimating
        )
        {
            return;
        }
        Size = size;
        SafeAreaInsets = insets;
        _scale = Scale;
        _renderedWhileAnimating = IsAnimating;
        Changed?.Invoke();
        render(size);
    }

    private void Finish(bool force = false)
    {
        if (!force && !_disposed && view.Window is not null && (canFinish ?? canRender)?.Invoke() == false)
        {
            // Keep the display pulse alive until the last GPU lease retires.
            // Advancing geometry first would invalidate that in-flight scene.
            return;
        }
        Stop();
        if (!_disposed && view.Window is not null)
        {
            Render(view.Bounds.Size, view.SafeAreaInsets);
            // Retry the final size if the renderer was under GPU backpressure.
            view.SetNeedsDisplay();
        }
    }

    internal void Stop()
    {
        var wasAnimating = _displayLink is not null;
        ++_generation;
        _displayLink?.Invalidate();
        _displayLink?.Dispose();
        _displayLink = null;
        if (wasAnimating)
        {
            AnimationChanged?.Invoke(false);
        }
    }

    public void Dispose()
    {
        _disposed = true;
        Stop();
    }

    private static bool IsValid(CGSize size) =>
        double.IsFinite(size.Width)
        && double.IsFinite(size.Height)
        && size.Width > 0
        && size.Height > 0;

    private static nfloat Lerp(nfloat from, nfloat to, double progress) =>
        (nfloat)(from + (to - from) * progress);
}
#endif
