#if ANDROID
using Android.Views;
using Android.Util;
using AndroidX.Window.Layout;
using AndroidX.Window.Java.Layout;
using Doroti.Ui;
using Rect = Doroti.Ui.Rect;

namespace Doroti.Host.Maui;

internal sealed partial class MauiViewEnvironment
{
    [System.Runtime.Versioning.SupportedOSPlatform("android34.0")]
    private sealed class ContrastListener(Action changed) : Java.Lang.Object, Android.App.UiModeManager.IContrastChangeListener
    {
        public void OnContrastChanged(float contrast) => changed();
    }
    private WindowInsets? _currentInsets;
    private WindowLayoutInfo? _windowLayout;
    private static Android.App.Activity? FindActivity(Android.Content.Context? context)
    {
        while (context is Android.Content.ContextWrapper wrapper && context is not Android.App.Activity) context = wrapper.BaseContext;
        return context as Android.App.Activity;
    }
    private sealed class LayoutConsumer(Action<WindowLayoutInfo> changed) : Java.Lang.Object, AndroidX.Core.Util.IConsumer
    {
        public void Accept(Java.Lang.Object? value) { if (value is WindowLayoutInfo info) changed(info); }
    }
    private sealed class InsetsListener(Action<WindowInsets> changed) : Java.Lang.Object, Android.Views.View.IOnApplyWindowInsetsListener
    {
        public WindowInsets OnApplyWindowInsets(Android.Views.View? view, WindowInsets? insets)
        {
            if (insets is null) throw new ArgumentNullException(nameof(insets));
            changed(insets);
            return insets;
        }
    }
    [System.Runtime.Versioning.SupportedOSPlatform("android30.0")]
    private sealed class InsetsAnimation(Action<WindowInsets> changed) : WindowInsetsAnimation.Callback((int)WindowInsetsAnimationDispatchMode.ContinueOnSubtree)
    {
        public override WindowInsets OnProgress(WindowInsets insets, IList<WindowInsetsAnimation> runningAnimations)
        { changed(insets); return insets; }
    }
    partial void AttachNative()
    {
        if (_element.Handler?.PlatformView is not Android.Views.View native) return;
        var observer = native.ViewTreeObserver;
        if (observer is not null)
        {
            observer.GlobalLayout += Refresh;
            _detach.Add(() => { if (observer.IsAlive) observer.GlobalLayout -= Refresh; });
        }
        var listener = new InsetsListener(value => { _currentInsets = value; Refresh(); });
        native.SetOnApplyWindowInsetsListener(listener);
        _detach.Add(() => { native.SetOnApplyWindowInsetsListener(null); _currentInsets = null; listener.Dispose(); });
        if (OperatingSystem.IsAndroidVersionAtLeast(30))
        {
            var animation = new InsetsAnimation(value => { _currentInsets = value; Refresh(); });
            native.SetWindowInsetsAnimationCallback(animation);
            _detach.Add(() => { if (OperatingSystem.IsAndroidVersionAtLeast(30)) native.SetWindowInsetsAnimationCallback(null); animation.Dispose(); });
        }
        if (native.Context?.ContentResolver is { } resolver)
        {
            var settings = new MauiSettingsObserver(() => Refresh());
            resolver.RegisterContentObserver(Android.Provider.Settings.System.ContentUri!, true, settings);
            resolver.RegisterContentObserver(Android.Provider.Settings.Secure.ContentUri!, true, settings);
            resolver.RegisterContentObserver(Android.Provider.Settings.Global.ContentUri!, true, settings);
            _detach.Add(() => { resolver.UnregisterContentObserver(settings); settings.Dispose(); });
        }
        if (FindActivity(native.Context) is { } activity)
        {
            if (Application.Current is DorotiMauiApplication && activity.Window is { } window)
            {
                AndroidX.Core.View.WindowCompat.SetDecorFitsSystemWindows(window, false);
                window.SetSoftInputMode(SoftInput.AdjustResize);
            }
            if (OperatingSystem.IsAndroidVersionAtLeast(34) && activity.GetSystemService(Android.Content.Context.UiModeService) is Android.App.UiModeManager modes)
            {
                var contrast = new ContrastListener(() => Refresh());
                modes.AddContrastChangeListener(AndroidX.Core.Content.ContextCompat.GetMainExecutor(activity)!, contrast);
                _detach.Add(() => { if (OperatingSystem.IsAndroidVersionAtLeast(34)) modes.RemoveContrastChangeListener(contrast); contrast.Dispose(); });
            }
            var tracker = new WindowInfoTrackerCallbackAdapter(WindowInfoTracker.Companion.GetOrCreate(activity));
            var consumer = new LayoutConsumer(info => { _windowLayout = info; Refresh(); });
            tracker.AddWindowLayoutInfoListener(activity, AndroidX.Core.Content.ContextCompat.GetMainExecutor(activity)!, consumer);
            _detach.Add(() => { tracker.RemoveWindowLayoutInfoListener(consumer); consumer.Dispose(); tracker.Dispose(); _windowLayout = null; });
        }
        native.RequestApplyInsets();
    }
    partial void CaptureNative()
    {
        if (_element.Handler?.PlatformView is not Android.Views.View native || (_currentInsets ?? native.RootWindowInsets) is not { } insets) return;
        NativePhysicalSize = new(native.Width, native.Height);
        var root = native.RootView!;
        int[] origin = new int[2], rootOrigin = new int[2];
        native.GetLocationOnScreen(origin); root.GetLocationOnScreen(rootOrigin);
        var view = Rect.fromLTWH(origin[0], origin[1], native.Width, native.Height);
        var bounds = Rect.fromLTWH(rootOrigin[0], rootOrigin[1], root.Width, root.Height);
        if (OperatingSystem.IsAndroidVersionAtLeast(30) && FindActivity(native.Context)?.WindowManager?.CurrentWindowMetrics is { } windowMetrics)
        {
            var window = windowMetrics.Bounds;
            bounds = Rect.fromLTWH(window.Left, window.Top, window.Width(), window.Height());
        }
        ViewPadding bars, ime, gestures = ViewPadding.zero;
        if (OperatingSystem.IsAndroidVersionAtLeast(30))
        {
            static ViewPadding Convert(Android.Graphics.Insets value) => new(value.Left, value.Top, value.Right, value.Bottom);
            bars = Convert(insets.GetInsets(WindowInsets.Type.SystemBars() | WindowInsets.Type.DisplayCutout()));
            ime = Convert(insets.GetInsets(WindowInsets.Type.Ime()));
            gestures = Convert(insets.GetInsets(WindowInsets.Type.SystemGestures()));
        }
        else
        {
#pragma warning disable CS0618, CA1422
            using var visible = new Android.Graphics.Rect();
            root.GetWindowVisibleDisplayFrame(visible);
            var keyboard = Math.Max(0, bounds.bottom - visible.Bottom);
            var hasKeyboard = keyboard > bounds.height * .18;
            bars = new(insets.SystemWindowInsetLeft, insets.SystemWindowInsetTop, insets.SystemWindowInsetRight,
                hasKeyboard ? insets.StableInsetBottom : insets.SystemWindowInsetBottom);
            ime = new(0, 0, 0, hasKeyboard ? keyboard : 0);
#pragma warning restore CS0618, CA1422
            if (OperatingSystem.IsAndroidVersionAtLeast(29))
            {
#pragma warning disable CS0618, CA1422
                var g = insets.SystemGestureInsets;
#pragma warning restore CS0618, CA1422
                gestures = new(g.Left, g.Top, g.Right, g.Bottom);
            }
        }
        Padding = ViewOcclusion.SafeEdges(view, bounds, bars);
        Insets = ViewOcclusion.SafeEdges(view, bounds, ime);
        Gestures = ViewOcclusion.SafeEdges(view, bounds, gestures);
        var resources = native.Resources!;
        var configuration = resources.Configuration!;
        Locales = configuration.Locales?.ToLanguageTags()?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(ParseLocale).ToArray();
        if (FontScaler is null || TextScale != configuration.FontScale)
        {
            TextScale = configuration.FontScale;
            // Copy DisplayMetrics: an existing scaler must not change when Resources changes.
            var metrics = new DisplayMetrics(); metrics.SetTo(resources.DisplayMetrics);
            var scale = TextScale;
            FontScaler = OperatingSystem.IsAndroidVersionAtLeast(34)
                ? size => TypedValue.ApplyDimension(ComplexUnitType.Sp, (float)size, metrics) / metrics.Density
                : size => size * scale;
        }
        GestureSettings = new(Android.Views.ViewConfiguration.Get(native.Context!)?.ScaledTouchSlop);
        Use24Hour = Android.Text.Format.DateFormat.Is24HourFormat(native.Context);
        var manager = native.Context?.GetSystemService(Android.Content.Context.AccessibilityService) as Android.Views.Accessibility.AccessibilityManager;
        var animations = Android.Provider.Settings.Global.GetFloat(native.Context!.ContentResolver,
            Android.Provider.Settings.Global.TransitionAnimationScale, 1) == 0;
        var bold = OperatingSystem.IsAndroidVersionAtLeast(31) && configuration.FontWeightAdjustment != int.MaxValue && configuration.FontWeightAdjustment >= 300;
        var contrast = OperatingSystem.IsAndroidVersionAtLeast(34) &&
            (native.Context.GetSystemService(Android.Content.Context.UiModeService) as Android.App.UiModeManager)?.Contrast > 0;
        Accessibility = new(manager?.IsTouchExplorationEnabled == true, false, animations, bold, contrast, false, false);
        var features = new List<DisplayFeature>();
        var density = resources.DisplayMetrics!.Density;
        int[] windowOrigin = new int[2]; native.GetLocationInWindow(windowOrigin);
        if (_windowLayout is not null)
            foreach (var feature in _windowLayout.DisplayFeatures)
            {
                var b = feature.Bounds;
                var local = new Rect((b.Left - windowOrigin[0]) / density, (b.Top - windowOrigin[1]) / density,
                    (b.Right - windowOrigin[0]) / density, (b.Bottom - windowOrigin[1]) / density);
                if (local.right < 0 || local.bottom < 0 || local.left > native.Width / density || local.top > native.Height / density) continue;
                var folding = feature as IFoldingFeature;
                features.Add(new(local, folding is null ? DisplayFeatureType.unknown :
                    (b.Width() > 0 && b.Height() > 0 ? DisplayFeatureType.hinge : DisplayFeatureType.fold),
                    folding?.State.ToString() == "HALF_OPENED" ? DisplayFeatureState.postureHalfOpened :
                    folding is not null ? DisplayFeatureState.postureFlat : DisplayFeatureState.unknown));
            }
        if (OperatingSystem.IsAndroidVersionAtLeast(28) && insets.DisplayCutout is { } cutout)
            foreach (var b in cutout.BoundingRects)
            {
                var local = new Rect((b.Left - windowOrigin[0]) / density, (b.Top - windowOrigin[1]) / density,
                    (b.Right - windowOrigin[0]) / density, (b.Bottom - windowOrigin[1]) / density);
                if (local.right > 0 && local.bottom > 0 && local.left < native.Width / density && local.top < native.Height / density)
                    features.Add(new(local, DisplayFeatureType.cutout, DisplayFeatureState.unknown));
            }
        Features = Array.AsReadOnly(features.ToArray());
        if (OperatingSystem.IsAndroidVersionAtLeast(31))
            Corners = new(insets.GetRoundedCorner((int)RoundedCornerPosition.TopLeft)?.Radius ?? 0,
                insets.GetRoundedCorner((int)RoundedCornerPosition.TopRight)?.Radius ?? 0,
                insets.GetRoundedCorner((int)RoundedCornerPosition.BottomRight)?.Radius ?? 0,
                insets.GetRoundedCorner((int)RoundedCornerPosition.BottomLeft)?.Radius ?? 0);
    }
}
#endif
