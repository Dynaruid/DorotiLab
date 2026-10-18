#if IOS || MACCATALYST
using CoreGraphics;
using Doroti.Ui;
using Foundation;
using UIKit;
using Rect = Doroti.Ui.Rect;

namespace Doroti.Host.Maui;

internal sealed partial class MauiViewEnvironment
{
    private CGRect _keyboardScreenFrame;
    private UIView? _keyboardWindow;
    private UIKitKeyboardAnimation? _keyboardAnimation;
    private bool _configurationDirty = true;

    private void StopKeyboardAnimation()
    {
        _keyboardAnimation?.Stop();
    }

    private void ChangeKeyboardFrame(CGRect target, double duration, int curve)
    {
        var initial =
            _keyboardScreenFrame.IsEmpty && _keyboardWindow is UIWindow window
                ? new CGRect(target.X, window.Screen.Bounds.Bottom, target.Width, target.Height)
                : _keyboardScreenFrame;
        _keyboardAnimation?.ChangeFrame(initial, target, duration, curve);
    }

    partial void AttachNative()
    {
        if (_element.Handler?.PlatformView is not UIView native)
        {
            return;
        }
        _configurationDirty = true;
        _keyboardAnimation = new UIKitKeyboardAnimation(native, frame =>
        {
            _keyboardScreenFrame = frame;
            Refresh();
        });

#if IOS && !MACCATALYST
        if (native is IUIKitAnimatedViewport animated)
        {
            void RefreshViewport() => Refresh();
            animated.AnimatedViewport.Changed += RefreshViewport;
            _detach.Add(() => animated.AnimatedViewport.Changed -= RefreshViewport);
        }
#endif

        var sentinel = new EnvironmentView(() => Refresh())
        {
            Frame = native.Bounds,
            AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
            UserInteractionEnabled = false,
            BackgroundColor = UIColor.Clear,
        };
        native.AddSubview(sentinel);
        _detach.Add(() =>
        {
            _keyboardAnimation?.Dispose();
            _keyboardAnimation = null;
            sentinel.RemoveFromSuperview();
            sentinel.Dispose();
            _keyboardScreenFrame = CGRect.Empty;
            _keyboardWindow = null;
        });
        foreach (
            var name in new[]
            {
                "UIKeyboardWillChangeFrameNotification",
                "UIKeyboardWillHideNotification",
                "UIKeyboardDidChangeFrameNotification",
                "UIKeyboardDidHideNotification",
                "UIContentSizeCategoryDidChangeNotification",
                "UIAccessibilityVoiceOverStatusDidChangeNotification",
                "UIAccessibilitySwitchControlStatusDidChangeNotification",
                "UIAccessibilityInvertColorsStatusDidChangeNotification",
                "UIAccessibilityReduceMotionStatusDidChangeNotification",
                "UIAccessibilityBoldTextStatusDidChangeNotification",
                "UIAccessibilityDarkerSystemColorsStatusDidChangeNotification",
                "UIAccessibilityOnOffSwitchLabelsDidChangeNotification",
                "UIApplicationDidBecomeActiveNotification",
                "UIWindowDidResignKeyNotification",
                "NSCurrentLocaleDidChangeNotification",
                "UIApplicationSignificantTimeChangeNotification",
                "NSSystemTimeZoneDidChangeNotification",
            }
        )
        {
            var token = NSNotificationCenter.DefaultCenter.AddObserver(
                new NSString(name),
                notification =>
                {
                    // Rotation pulses change geometry, not locale, font scaling
                    // or accessibility policy. Refresh those on their native
                    // notifications instead of querying Foundation every pulse.
                    if (!name.Contains("Keyboard", StringComparison.Ordinal))
                        _configurationDirty = true;
                    if (name.Contains("Keyboard", StringComparison.Ordinal))
                    {
                        // Notifications are process-wide. Only the window owning the editing view participates.
                        if (native.Window is not { IsKeyWindow: true })
                        {
                            return;
                        }

                        _keyboardWindow = native.Window;
                        var target =
                            name == "UIKeyboardDidHideNotification"
                                ? CGRect.Empty
                                : (
                                    notification.UserInfo?[UIKeyboard.FrameEndUserInfoKey]
                                    as NSValue
                                )?.CGRectValue
                                    ?? CGRect.Empty;
                        var duration = name.Contains("Will", StringComparison.Ordinal)
                            ? (
                                notification.UserInfo?[UIKeyboard.AnimationDurationUserInfoKey]
                                as NSNumber
                            )?.DoubleValue
                                ?? 0
                            : 0;
                        var curve =
                            (
                                notification.UserInfo?[UIKeyboard.AnimationCurveUserInfoKey]
                                as NSNumber
                            )?.Int32Value
                            ?? 0;
                        ChangeKeyboardFrame(target, duration, curve);
                    }
                    if (
                        name == "UIWindowDidResignKeyNotification"
                        && native.Window?.IsKeyWindow != true
                    )
                    {
                        StopKeyboardAnimation();
                        _keyboardScreenFrame = CGRect.Empty;
                    }
                    Refresh();
                }
            );
            _detach.Add(() =>
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(token);
                token.Dispose();
            });
        }
    }

    partial void CaptureNative()
    {
        if (_element.Handler?.PlatformView is not UIView native || native.Window is not { } window)
        {
            return;
        }

        var scale = (double)window.Screen.Scale;
        var viewportSize = native.Bounds.Size;
        var safe = native.SafeAreaInsets;
#if IOS && !MACCATALYST
        if (native is IUIKitAnimatedViewport animated && animated.AnimatedViewport.Size.Width > 0)
        {
            viewportSize = animated.AnimatedViewport.Size;
            safe = animated.AnimatedViewport.SafeAreaInsets;
        }
#endif
        NativePhysicalSize = new(
            Math.Round(viewportSize.Width * scale),
            Math.Round(viewportSize.Height * scale)
        );
        Padding = ViewOcclusion.Scale(new(safe.Left, safe.Top, safe.Right, safe.Bottom), scale);
        Insets = ViewPadding.zero;
        if (_keyboardWindow == window && !_keyboardScreenFrame.IsEmpty)
        {
            var inWindow = window.ConvertRectFromCoordinateSpace(
                _keyboardScreenFrame,
                window.Screen.CoordinateSpace
            );
            var keyboard = native.ConvertRectFromView(inWindow, window);
            Insets = ViewOcclusion.Scale(
                ViewOcclusion.EdgeInsets(
                    Rect.fromLTWH(
                        native.Bounds.X,
                        native.Bounds.Y,
                        native.Bounds.Width,
                        native.Bounds.Height
                    ),
                    Rect.fromLTWH(keyboard.X, keyboard.Y, keyboard.Width, keyboard.Height)
                ),
                scale
            );
        }
        if (!_configurationDirty)
            return;
        using var preferredBodyFont =
            UIFont.GetPreferredFontForTextStyle(UIFontTextStyle.Body)
            ?? throw new InvalidOperationException(
                "UIKit did not provide its preferred body font."
            );
        TextScale = preferredBodyFont.PointSize / 17.0;
        Accessibility = new(
            UIAccessibility.IsVoiceOverRunning || UIAccessibility.IsSwitchControlRunning,
            UIAccessibility.IsInvertColorsEnabled,
            false,
            UIAccessibility.IsBoldTextEnabled,
            UIAccessibility.DarkerSystemColorsEnabled,
            UIAccessibility.IsOnOffSwitchLabelsEnabled,
            false,
            UIAccessibility.IsReduceMotionEnabled
        );
        var pattern = NSDateFormatter.GetDateFormatFromTemplate("j", 0, NSLocale.CurrentLocale);
        Locales = NSLocale.PreferredLanguages.Select(ParseLocale).ToArray();
        Use24Hour = pattern?.Contains('a') == false;
        _configurationDirty = false;
    }

    private sealed class EnvironmentView(Action changed) : UIView
    {
        public override void SafeAreaInsetsDidChange()
        {
            base.SafeAreaInsetsDidChange();
            changed();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            changed();
        }
    }
}
#endif
