#if IOS && !MACCATALYST
using CoreGraphics;
using Doroti.Ui;
using Foundation;
using UIKit;

namespace Doroti.Host.Maui;

/// <summary>Live UIKit material blur, interpolated with public APIs.
/// MatchCommon sigma is a strength convention, not a native Gaussian radius.</summary>
internal sealed class UIKitPlatformBlurView : UIVisualEffectView
{
    internal static PlatformEffectSupport Support { get; } =
        new(
            true,
            1,
            16,
            false,
            "UIKit public material interpolation: one isotropic MatchCommon blur; ExactSigma/saturation are not supported."
        );

    private readonly NSObject _resumeObserver;
    private readonly NSObject _sceneResumeObserver;
    private readonly UIBlurEffect _blur;
    private UIViewPropertyAnimator? _animator;
    private CGRect _appliedBounds;
    private double _intensity;
    private bool _applying,
        _disposed;

    internal UIKitPlatformBlurView()
    {
        // Keep the existing fixed Light appearance. Its material tint is part of
        // the public effect; the authored tint/child are composited separately.
        _blur = UIBlurEffect.FromStyle(UIBlurEffectStyle.Light);
        Effect = null;
        Opaque = false;
        UserInteractionEnabled = false;
        ClipsToBounds = true;
        OverrideUserInterfaceStyle = UIUserInterfaceStyle.Light;
        _resumeObserver = NSNotificationCenter.DefaultCenter.AddObserver(
            UIApplication.DidBecomeActiveNotification,
            _ => Reapply()
        );
        _sceneResumeObserver = NSNotificationCenter.DefaultCenter.AddObserver(
            UIScene.DidActivateNotification,
            notification =>
            {
                if (Window?.WindowScene == notification.Object)
                {
                    Reapply();
                }
            }
        );
    }

    // Expose the actual animator position for the opt-in UIKit runtime probe.
    internal double AppliedIntensity =>
        _animator is { } animator ? (double)animator.FractionComplete : 0;

    internal void SetSigma(double sigma)
    {
        UIKitPlatformViewDispatcher.VerifyThread();
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!double.IsFinite(sigma) || sigma is < 0 or > 16)
        {
            throw new ArgumentOutOfRangeException(nameof(sigma));
        }

        var intensity = sigma / 16;
        if (_intensity == intensity && (_animator is not null || intensity == 0))
        {
            return;
        }

        _intensity = intensity;
        if (_animator is not null && intensity > 0)
        {
            _animator.FractionComplete = (nfloat)intensity;
        }
        else
        {
            Reapply();
        }
    }

    private void Reapply()
    {
        if (_disposed || _applying)
        {
            return;
        }

        _applying = true;
        try
        {
            StopAnimator();
            Effect = null;
            _appliedBounds = Bounds;
            if (_intensity == 0)
            {
                return;
            }

            _animator = new UIViewPropertyAnimator(
                1,
                UIViewAnimationCurve.Linear,
                () => Effect = _blur
            )
            {
                ScrubsLinearly = true,
                PausesOnCompletion = true,
            };
            // Retain the paused animator for the lifetime of the effect. Finishing
            // it would lose the interpolated state and restore the full material.
            _animator.StartAnimation();
            _animator.PauseAnimation();
            _animator.FractionComplete = (nfloat)_intensity;
        }
        finally
        {
            _applying = false;
        }
    }

    private void StopAnimator()
    {
        if (_animator is not { } animator)
        {
            return;
        }

        _animator = null;
        animator.StopAnimation(true);
        animator.Dispose();
    }

    public override void LayoutSubviews()
    {
        base.LayoutSubviews();
        // Ordinary layout/render passes must not continually reset interpolation.
        if (Bounds != _appliedBounds)
        {
            Reapply();
        }
    }

    public override void MovedToWindow()
    {
        base.MovedToWindow();
        Reapply();
    }
    // Required for our iOS 15 minimum; UIKit still delivers this compatibility callback.
#pragma warning disable CA1422
    public override void TraitCollectionDidChange(UITraitCollection? previousTraitCollection)
    {
        base.TraitCollectionDidChange(previousTraitCollection);
        Reapply();
    }
#pragma warning restore CA1422
    public override string ToString() =>
        FormattableString.Invariant($"{base.ToString()} BlurIntensity={AppliedIntensity}");

    protected override void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            _disposed = true;
            StopAnimator();
            Effect = null;
            _blur.Dispose();
            NSNotificationCenter.DefaultCenter.RemoveObserver(_resumeObserver);
            _resumeObserver.Dispose();
            NSNotificationCenter.DefaultCenter.RemoveObserver(_sceneResumeObserver);
            _sceneResumeObserver.Dispose();
        }
        base.Dispose(disposing);
    }
}
#endif
