#if IOS || MACCATALYST
using Foundation;
using UIKit;
using CoreGraphics;
using CoreAnimation;
using Doroti.Ui;
using Rect = Doroti.Ui.Rect;

namespace Doroti.Host.Maui;

internal sealed partial class MauiViewEnvironment
{
    private CGRect _keyboardScreenFrame;
    private UIView? _keyboardWindow;
    private CADisplayLink? _keyboardAnimation;
    private void StopKeyboardAnimation()
    {
        _keyboardAnimation?.Invalidate(); _keyboardAnimation?.Dispose(); _keyboardAnimation = null;
    }
    private void ChangeKeyboardFrame(CGRect target, double duration, int curve)
    {
        StopKeyboardAnimation();
        if (duration <= 0 || target.IsEmpty)
        { _keyboardScreenFrame = target; Refresh(); return; }
        var initial = _keyboardScreenFrame.IsEmpty && _keyboardWindow is UIWindow window
            ? new CGRect(target.X, window.Screen.Bounds.Bottom, target.Width, target.Height) : _keyboardScreenFrame;
        var start = System.Diagnostics.Stopwatch.GetTimestamp();
        _keyboardAnimation = CADisplayLink.Create(() =>
        {
            var t = Math.Clamp(System.Diagnostics.Stopwatch.GetElapsedTime(start).TotalSeconds / duration, 0, 1);
            var progress = curve switch { 1 => t * t, 2 => 1 - (1 - t) * (1 - t), 3 => t, _ => t * t * (3 - 2 * t) };
            _keyboardScreenFrame = new CGRect(initial.X + (target.X - initial.X) * progress,
                initial.Y + (target.Y - initial.Y) * progress, initial.Width + (target.Width - initial.Width) * progress,
                initial.Height + (target.Height - initial.Height) * progress);
            Refresh();
            if (t >= 1) StopKeyboardAnimation();
        });
        _keyboardAnimation.AddToRunLoop(NSRunLoop.Main, NSRunLoopMode.Common);
    }
    partial void AttachNative()
    {
        if (_element.Handler?.PlatformView is not UIView native) return;
        var sentinel = new EnvironmentView(() => Refresh()) { Frame = native.Bounds,
            AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
            UserInteractionEnabled = false, BackgroundColor = UIColor.Clear };
        native.AddSubview(sentinel);
        _detach.Add(() => { StopKeyboardAnimation(); sentinel.RemoveFromSuperview(); sentinel.Dispose(); _keyboardScreenFrame = CGRect.Empty; });
        foreach (var name in new[] { "UIKeyboardWillChangeFrameNotification", "UIKeyboardWillHideNotification",
            "UIKeyboardDidChangeFrameNotification", "UIKeyboardDidHideNotification", "UIContentSizeCategoryDidChangeNotification",
            "UIAccessibilityVoiceOverStatusDidChangeNotification", "UIAccessibilitySwitchControlStatusDidChangeNotification", "UIAccessibilityInvertColorsStatusDidChangeNotification",
            "UIAccessibilityReduceMotionStatusDidChangeNotification", "UIAccessibilityBoldTextStatusDidChangeNotification",
            "UIAccessibilityDarkerSystemColorsStatusDidChangeNotification", "UIAccessibilityOnOffSwitchLabelsDidChangeNotification",
            "UIApplicationDidBecomeActiveNotification", "UIWindowDidResignKeyNotification", "NSCurrentLocaleDidChangeNotification" })
        {
            var token = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(name), notification =>
            {
                if (name.Contains("Keyboard", StringComparison.Ordinal))
                {
                    // Notifications are process-wide. Only the window owning the editing view participates.
                    if (native.Window is not { IsKeyWindow: true }) return;
                    _keyboardWindow = native.Window;
                    var target = name == "UIKeyboardDidHideNotification" ? CGRect.Empty :
                        (notification.UserInfo?[UIKeyboard.FrameEndUserInfoKey] as NSValue)?.CGRectValue ?? CGRect.Empty;
                    var duration = name.Contains("Will", StringComparison.Ordinal)
                        ? (notification.UserInfo?[UIKeyboard.AnimationDurationUserInfoKey] as NSNumber)?.DoubleValue ?? 0 : 0;
                    var curve = (notification.UserInfo?[UIKeyboard.AnimationCurveUserInfoKey] as NSNumber)?.Int32Value ?? 0;
                    ChangeKeyboardFrame(target, duration, curve);
                }
                if (name == "UIWindowDidResignKeyNotification" && native.Window?.IsKeyWindow != true)
                { StopKeyboardAnimation(); _keyboardScreenFrame = CGRect.Empty; }
                Refresh();
            });
            _detach.Add(() => { NSNotificationCenter.DefaultCenter.RemoveObserver(token); token.Dispose(); });
        }
    }
    partial void CaptureNative()
    {
        if (_element.Handler?.PlatformView is not UIView native || native.Window is not { } window) return;
        var scale = (double)window.Screen.Scale;
        NativePhysicalSize = new(Math.Round(native.Bounds.Width * scale), Math.Round(native.Bounds.Height * scale));
        var safe = native.SafeAreaInsets;
        Padding = ViewOcclusion.Scale(new(safe.Left, safe.Top, safe.Right, safe.Bottom), scale);
        Insets = ViewPadding.zero;
        if (_keyboardWindow == window && !_keyboardScreenFrame.IsEmpty)
        {
            var inWindow = window.ConvertRectFromCoordinateSpace(_keyboardScreenFrame, window.Screen.CoordinateSpace);
            var keyboard = native.ConvertRectFromView(inWindow, window);
            Insets = ViewOcclusion.Scale(ViewOcclusion.EdgeInsets(
                Rect.fromLTWH(native.Bounds.X, native.Bounds.Y, native.Bounds.Width, native.Bounds.Height),
                Rect.fromLTWH(keyboard.X, keyboard.Y, keyboard.Width, keyboard.Height)), scale);
        }
        TextScale = UIFont.GetPreferredFontForTextStyle(UIFontTextStyle.Body).PointSize / 17.0;
        Accessibility = new(UIAccessibility.IsVoiceOverRunning || UIAccessibility.IsSwitchControlRunning, UIAccessibility.IsInvertColorsEnabled,
            false, UIAccessibility.IsBoldTextEnabled,
            UIAccessibility.DarkerSystemColorsEnabled, UIAccessibility.IsOnOffSwitchLabelsEnabled, false,
            UIAccessibility.IsReduceMotionEnabled);
        var pattern = NSDateFormatter.GetDateFormatFromTemplate("j", 0, NSLocale.CurrentLocale);
        Locales = NSLocale.PreferredLanguages.Select(ParseLocale).ToArray();
        Use24Hour = pattern?.Contains('a') == false;
    }
    private sealed class EnvironmentView(Action changed) : UIView
    {
        public override void SafeAreaInsetsDidChange() { base.SafeAreaInsetsDidChange(); changed(); }
        public override void LayoutSubviews() { base.LayoutSubviews(); changed(); }
    }
}
#endif
