#if WINDOWS
using Doroti.Ui;
using Microsoft.UI.Xaml;
using Windows.UI.ViewManagement;
using System.Runtime.InteropServices;
using Rect = Doroti.Ui.Rect;

namespace Doroti.Host.Maui;

internal sealed partial class MauiViewEnvironment
{
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate nint WindowObserver(nint window, uint message, nuint wparam, nint lparam, nuint id, nuint data);
    [LibraryImport("comctl32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetWindowSubclass(nint window, nint callback, nuint id, nuint data);
    [LibraryImport("comctl32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool RemoveWindowSubclass(nint window, nint callback, nuint id);
    [LibraryImport("comctl32.dll")]
    private static partial nint DefSubclassProc(nint window, uint message, nuint wparam, nint lparam);
    [StructLayout(LayoutKind.Sequential)]
    private struct NativeHighContrast { internal uint Size; internal uint Flags; internal nint DefaultScheme; }
    [LibraryImport("user32.dll", EntryPoint = "SystemParametersInfoW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool QueryHighContrast(uint action, uint size, ref NativeHighContrast value, uint flags);
    private InputPane? _inputPane;
    private readonly UISettings _uiSettings = new();
    private bool _highContrast;
    private Windows.Foundation.Rect _keyboardOcclusion;
    private bool _animationsEnabled = true;
    private bool _nativeSettingsRefreshPosted;
    partial void AttachNative()
    {
        if (_element.Handler?.PlatformView is not FrameworkElement native) return;
        void Schedule() => _element.Dispatcher.Dispatch(() => Refresh());
        void TextChanged(UISettings sender, object args) => Schedule();
        _uiSettings.TextScaleFactorChanged += TextChanged;
        _uiSettings.AnimationsEnabledChanged += TextChanged;
        _detach.Add(() => { _uiSettings.TextScaleFactorChanged -= TextChanged;
            _uiSettings.AnimationsEnabledChanged -= TextChanged; });
        if (_element.Window?.Handler?.PlatformView is Microsoft.UI.Xaml.Window window)
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WindowObserver observer = (handle, message, wparam, lparam, id, data) =>
            {
                if (message is 0x1A or 0x1E) { System.Globalization.CultureInfo.CurrentCulture.ClearCachedData(); Schedule(); }
                return DefSubclassProc(handle, message, wparam, lparam);
            };
            var callback = Marshal.GetFunctionPointerForDelegate(observer);
            if (!SetWindowSubclass(hwnd, callback, (nuint)callback, 0))
                throw new InvalidOperationException("Could not observe native Windows environment changes.");
            _detach.Add(() => { RemoveWindowSubclass(hwnd, callback, (nuint)callback); GC.KeepAlive(observer); });
            _inputPane = InputPaneInterop.GetForWindow(hwnd);
            void PaneChanged(InputPane pane, InputPaneVisibilityEventArgs args) { args.EnsuredFocusedElementInView = true; Schedule(); }
            _inputPane.Showing += PaneChanged; _inputPane.Hiding += PaneChanged;
            var pane = _inputPane;
            _detach.Add(() => { pane.Showing -= PaneChanged; pane.Hiding -= PaneChanged; _inputPane = null; });
        }
        if (native.XamlRoot is { } root)
        {
            void RootChanged(XamlRoot sender, XamlRootChangedEventArgs args) => Refresh();
            root.Changed += RootChanged; _detach.Add(() => root.Changed -= RootChanged);
        }
    }
    partial void CaptureNative()
    {
        if (_element.Handler?.PlatformView is not FrameworkElement native || native.XamlRoot is not { } root) return;
        NativePhysicalSize = new(Math.Round(native.ActualWidth * root.RasterizationScale), Math.Round(native.ActualHeight * root.RasterizationScale));
        Padding = ViewPadding.zero;
        var position = native.TransformToVisual(null).TransformPoint(new Windows.Foundation.Point());
        var bounds = Rect.fromLTWH(position.X, position.Y, native.ActualWidth, native.ActualHeight);
        try
        {
            // InputPane can make an outgoing COM call. Cross-process synchronous
            // WM_SIZE delivery forbids that call (RPC_E_CANTCALLOUT_ININPUTSYNCCALL).
            // Keep the last settings snapshot and refresh after the window message.
            var keyboardSnapshot = _inputPane?.OccludedRect ?? default;
            var textScaleSnapshot = _uiSettings.TextScaleFactor;
            var animationsSnapshot = _uiSettings.AnimationsEnabled;
            var localesSnapshot = Windows.System.UserProfile.GlobalizationPreferences.Languages.Select(ParseLocale).ToArray();
            _keyboardOcclusion = keyboardSnapshot;
            TextScale = textScaleSnapshot;
            _animationsEnabled = animationsSnapshot;
            Locales = localesSnapshot;
        }
        catch (COMException exception) when (exception.HResult == unchecked((int)0x8001010D))
        {
            if (!_nativeSettingsRefreshPosted)
            {
                _nativeSettingsRefreshPosted = true;
                if (!native.DispatcherQueue.TryEnqueue(() => { _nativeSettingsRefreshPosted = false; Refresh(); }))
                    _nativeSettingsRefreshPosted = false;
            }
        }
        var keyboard = _keyboardOcclusion;
        Insets = ViewOcclusion.Scale(ViewOcclusion.EdgeInsets(bounds,
            Rect.fromLTWH(keyboard.X, keyboard.Y, keyboard.Width, keyboard.Height)), root.RasterizationScale);
        var contrast = new NativeHighContrast { Size = (uint)Marshal.SizeOf<NativeHighContrast>() };
        if (QueryHighContrast(0x42, contrast.Size, ref contrast, 0)) _highContrast = (contrast.Flags & 1) != 0;
        Accessibility = new(false, false, !_animationsEnabled, false, _highContrast, false, false);
        Use24Hour = !System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.Contains('t');
    }
}
#endif
