#if IOS && !MACCATALYST
using Doroti.Ui;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Doroti.Host.Maui;

/// <summary>Live UIKit Gaussian extraction. Depends on private UIKit structure;
/// .NET iOS exception marshaling and structural probes make failures explicit.</summary>
internal sealed class UIKitPlatformBlurView : UIVisualEffectView
{
    private static PlatformEffectSupport? _support;
    private readonly NSObject _resumeObserver;
    private readonly NSObject _sceneResumeObserver;
    private double _sigma;
    private bool _applying, _disposed;
    private string? _failure;

    internal static PlatformEffectSupport Support
    {
        get
        {
            if (_support is not null) return _support;
            UIKitPlatformViewDispatcher.VerifyThread();
            using var blur = UIBlurEffect.FromStyle(UIBlurEffectStyle.Light);
            using var probe = new UIVisualEffectView(blur);
            var reason = UIKitGaussianFilter.Apply(probe, 6);
            return _support = reason is null
                ? new(true, 1, 16, false, "UIKit internal Gaussian: one isotropic MatchCommon blur; ExactSigma/saturation not qualified.")
                : new(Reason: reason);
        }
    }

    internal UIKitPlatformBlurView()
    {
        using var blur = UIBlurEffect.FromStyle(UIBlurEffectStyle.Light);
        Effect = blur;
        Opaque = false;
        UserInteractionEnabled = false;
        ClipsToBounds = true;
        OverrideUserInterfaceStyle = UIUserInterfaceStyle.Light;
        _resumeObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.DidBecomeActiveNotification, _ => Reapply());
        _sceneResumeObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIScene.DidActivateNotification, notification =>
        { if (Window?.WindowScene == notification.Object) Reapply(); });
    }

    internal void SetSigma(double sigma)
    {
        UIKitPlatformViewDispatcher.VerifyThread();
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!double.IsFinite(sigma) || sigma is <= 0 or > 16) throw new ArgumentOutOfRangeException(nameof(sigma));
        _sigma = sigma;
        Reapply();
        if (_failure is not null) throw new NotSupportedException(_failure);
    }

    private void Reapply()
    {
        if (_disposed || _applying || _sigma <= 0) return;
        _applying = true;
        try
        {
            _failure = UIKitGaussianFilter.Apply(this, _sigma);
            if (_failure is not null)
            {
                _support = new(Reason: _failure);
                Hidden = true; // Never display the unmodified material as a successful Gaussian.
            }
        }
        finally { _applying = false; }
    }

    public override void LayoutSubviews() { base.LayoutSubviews(); Reapply(); }
    public override void MovedToWindow() { base.MovedToWindow(); Reapply(); }
    // Required for our iOS 15 minimum; UIKit still delivers this compatibility callback.
#pragma warning disable CA1422
    public override void TraitCollectionDidChange(UITraitCollection? previousTraitCollection)
    { base.TraitCollectionDidChange(previousTraitCollection); Reapply(); }
#pragma warning restore CA1422
    public override string ToString() => FormattableString.Invariant($"{base.ToString()} GaussianSigma={_sigma} failure={_failure ?? "none"}");

    protected override void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            _disposed = true;
            NSNotificationCenter.DefaultCenter.RemoveObserver(_resumeObserver);
            _resumeObserver.Dispose();
            NSNotificationCenter.DefaultCenter.RemoveObserver(_sceneResumeObserver);
            _sceneResumeObserver.Dispose();
        }
        base.Dispose(disposing);
    }
}

// Only bound Foundation calls are used: the .NET iOS runtime intercepts their
// Objective-C exceptions and raises ObjCException at the managed call boundary.
// Raw objc_msgSend P/Invokes would require separate exception handling.
internal static class UIKitGaussianFilter
{
    private static readonly NSString FiltersKey = new("filters");
    private static readonly NSString NameKey = new("name");
    private static readonly NSString RadiusKey = new("inputRadius");
    private static readonly Selector CopySelector = new("copyWithZone:");

    internal static string? Apply(UIVisualEffectView effect, double sigma)
    {
        UIKitPlatformViewDispatcher.VerifyThread();
        if (!double.IsFinite(sigma) || sigma is <= 0 or > 16) return "Unsupported UIKit Gaussian radius";
        UIView? backdrop = null, tint = null;
        NSArray? oldFilters = null;
        CoreGraphics.CGColor? oldColor = null;
        var changed = false;
        try
        {
            foreach (var child in effect.Subviews)
            {
                var name = child.Class.Name ?? string.Empty;
                if (name.EndsWith("BackdropView", StringComparison.Ordinal)) backdrop = child;
                else if (name.EndsWith("VisualEffectSubview", StringComparison.Ordinal)) tint = child;
            }
            if (backdrop is null || tint is null) return "UIKit backdrop/effect structure unavailable";
            oldFilters = backdrop.Layer.ValueForKey(FiltersKey) as NSArray;
            if (oldFilters is null) return "UIKit Gaussian filter unavailable";
            for (nuint i = 0; i < oldFilters.Count; i++)
            {
                var filter = oldFilters.GetItem<NSObject>(i);
                if (filter is null || filter.ValueForKey(NameKey)?.ToString() != "gaussianBlur" ||
                    filter.ValueForKey(RadiusKey) is not NSNumber || !filter.RespondsToSelector(CopySelector)) continue;
                // Unknown UIKit classes are wrapped as NSObject, whose Copy()
                // checks the managed interface. Use the bound native protocol.
                using var copying = ObjCRuntime.Runtime.GetINativeObject<INSCopying>(filter.Handle, false);
                if (copying is null) return "UIKit Gaussian copying protocol unavailable";
                using var copy = copying.Copy(null);
                using var radius = NSNumber.FromDouble(sigma);
                copy.SetValueForKey(radius, RadiusKey);
                if (copy.ValueForKey(RadiusKey) is not NSNumber actual || Math.Abs(actual.DoubleValue - sigma) > .001)
                    return "UIKit rejected Gaussian radius";
                using var onlyBlur = NSArray.FromNSObjects(copy);
                oldColor = tint.Layer.BackgroundColor;
                changed = true;
                backdrop.Layer.SetValueForKey(onlyBlur, FiltersKey);
                using var clear = UIColor.Clear.CGColor;
                tint.Layer.BackgroundColor = clear;
                return null;
            }
            return "UIKit Gaussian filter unavailable";
        }
        catch (ObjCException error)
        {
            if (changed)
            {
                try
                {
                    backdrop!.Layer.SetValueForKey(oldFilters!, FiltersKey);
                    tint!.Layer.BackgroundColor = oldColor;
                }
                catch (ObjCException) { effect.Hidden = true; }
            }
            return "UIKit Gaussian unsupported: " + error.Message;
        }
        finally { oldColor?.Dispose(); }
    }
}
#endif
