#if !MACOS
using Doroti.Ui;
using SkiaSharp.Views.Maui.Controls;

namespace Doroti.Host.Maui;

internal static class MauiNativeInput
{
    internal static IDisposable Attach(SKGLView view, MauiTextInputBridge textInput, ulong viewId, Action<KeyData> dispatch) =>
        new NativeKeyboardSubscription(view, textInput, viewId, dispatch);

    internal static void SetCursor(SKGLView view, DorotiMouseCursorKind cursor) =>
        NativeCursor.Set(view, cursor);

    private static long Logical(string key, long physical)
    {
        if (key.Length == 1 && !char.IsControl(key[0])) return char.ToLowerInvariant(key[0]);
        if (key.Length is 2 or 3 && key[0] == 'F' && int.TryParse(key.AsSpan(1), out var function) && function is >= 1 and <= 24)
            return 0x100000801 + function - 1;
        return key switch
        {
            "Backspace" => 0x100000008,
            "Tab" => 0x100000009,
            "Enter" => 0x10000000d,
            "Escape" => 0x10000001b,
            "Delete" => 0x10000007f,
            "ArrowDown" => 0x100000301,
            "ArrowLeft" => 0x100000302,
            "ArrowRight" => 0x100000303,
            "ArrowUp" => 0x100000304,
            "End" => 0x100000305,
            "Home" => 0x100000306,
            "PageDown" => 0x100000307,
            "PageUp" => 0x100000308,
            "ShiftLeft" => 0x200000100,
            "ShiftRight" => 0x200000101,
            "ControlLeft" => 0x200000102,
            "ControlRight" => 0x200000103,
            "AltLeft" => 0x200000104,
            "AltRight" => 0x200000105,
            "MetaLeft" => 0x200000106,
            "MetaRight" => 0x200000107,
            _ => physical switch
            {
                0x70028 => 0x10000000d,
                0x70029 => 0x10000001b,
                0x7002a => 0x100000008,
                0x7002b => 0x100000009,
                0x7004c => 0x10000007f,
                0x7004f => 0x100000303,
                0x70050 => 0x100000302,
                0x70051 => 0x100000301,
                0x70052 => 0x100000304,
                0x700e0 => 0x200000102,
                0x700e1 => 0x200000100,
                0x700e2 => 0x200000104,
                0x700e3 => 0x200000106,
                0x700e4 => 0x200000103,
                0x700e5 => 0x200000101,
                0x700e6 => 0x200000105,
                0x700e7 => 0x200000107,
                _ => physical == 0 ? 0 : 0x100000000 | physical,
            },
        };
    }

#if WINDOWS
    private sealed class NativeKeyboardSubscription : IDisposable
    {
        private readonly SKGLView _view;
        private readonly ulong _viewId;
        private readonly Action<KeyData> _dispatch;
        private readonly Dictionary<long, (long Logical, string? Character)> _pressed = [];
        private readonly MauiTextInputBridge _textInput;
        private readonly List<Microsoft.UI.Xaml.UIElement> _native = [];

        internal NativeKeyboardSubscription(SKGLView view, MauiTextInputBridge textInput, ulong viewId, Action<KeyData> dispatch)
        {
            _view = view;
            _textInput = textInput;
            _viewId = viewId;
            _dispatch = dispatch;
            _view.HandlerChanged += HandleHandlerChanged;
            _view.Unfocused += HandleUnfocused;
            foreach (var input in _textInput.Inputs) input.HandlerChanged += HandleHandlerChanged;
            AttachCurrent();
        }

        private void HandleHandlerChanged(object? sender, EventArgs args) => AttachCurrent();

        private void AttachCurrent()
        {
            DetachCurrent();
            foreach (var element in new object?[] { _view.Handler?.PlatformView }
                .Concat(_textInput.Inputs.Select(input => input.Handler?.PlatformView))
                .OfType<Microsoft.UI.Xaml.UIElement>().Distinct())
            {
                element.KeyDown += HandleKeyDown;
                element.KeyUp += HandleKeyUp;
                _native.Add(element);
            }
        }

        private void HandleKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs args) =>
            Dispatch(args, args.KeyStatus.RepeatCount > 1 ? KeyEventType.repeat : KeyEventType.down, false);

        private void HandleKeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs args) =>
            Dispatch(args, KeyEventType.up, false);

        private void Dispatch(Microsoft.UI.Xaml.Input.KeyRoutedEventArgs args, KeyEventType type, bool synthesized)
        {
            var key = KeyName(args.Key);
            var physical = Physical(args.Key);
            var logical = Logical(key, physical);
            var character = key.Length == 1 ? key : null;
            if (type is KeyEventType.down or KeyEventType.repeat) _pressed[physical] = (logical, character);
            else _pressed.Remove(physical);
            _dispatch(new(_viewId, TimeSpan.FromTicks(DateTime.UtcNow.Ticks), type,
                physical, logical, synthesized, character));
            args.Handled = true;
        }

        private void HandleUnfocused(object? sender, Microsoft.Maui.Controls.FocusEventArgs args)
            => ReleasePressed();

        private void ReleasePressed()
        {
            var timestamp = TimeSpan.FromTicks(DateTime.UtcNow.Ticks);
            foreach (var (physical, value) in _pressed)
                _dispatch(new(_viewId, timestamp, KeyEventType.up, physical, value.Logical, true, value.Character));
            _pressed.Clear();
        }

        private static long Physical(Windows.System.VirtualKey key)
        {
            var value = (int)key;
            if (value is >= 65 and <= 90) return 0x70004 + value - 65;
            if (value is >= 49 and <= 57) return 0x7001e + value - 49;
            if (value == 48) return 0x70027;
            if (value is >= 112 and <= 123) return 0x7003a + value - 112;
            if (value is >= 124 and <= 135) return 0x70068 + value - 124;
            return key switch
            {
                Windows.System.VirtualKey.Enter => 0x70028,
                Windows.System.VirtualKey.Escape => 0x70029,
                Windows.System.VirtualKey.Back => 0x7002a,
                Windows.System.VirtualKey.Tab => 0x7002b,
                Windows.System.VirtualKey.Space => 0x7002c,
                Windows.System.VirtualKey.Home => 0x7004a,
                Windows.System.VirtualKey.PageUp => 0x7004b,
                Windows.System.VirtualKey.Delete => 0x7004c,
                Windows.System.VirtualKey.End => 0x7004d,
                Windows.System.VirtualKey.PageDown => 0x7004e,
                Windows.System.VirtualKey.Right => 0x7004f,
                Windows.System.VirtualKey.Left => 0x70050,
                Windows.System.VirtualKey.Down => 0x70051,
                Windows.System.VirtualKey.Up => 0x70052,
                Windows.System.VirtualKey.Control => 0x700e0,
                Windows.System.VirtualKey.Shift => 0x700e1,
                Windows.System.VirtualKey.Menu => 0x700e2,
                Windows.System.VirtualKey.LeftWindows => 0x700e3,
                Windows.System.VirtualKey.RightWindows => 0x700e7,
                _ => 0x100000000 | (uint)value,
            };
        }

        private static string KeyName(Windows.System.VirtualKey key)
        {
            var value = (int)key;
            if (value is >= 65 and <= 90) return ((char)value).ToString();
            if (value is >= 48 and <= 57) return ((char)value).ToString();
            return key switch
            {
                Windows.System.VirtualKey.Enter => "Enter",
                Windows.System.VirtualKey.Escape => "Escape",
                Windows.System.VirtualKey.Back => "Backspace",
                Windows.System.VirtualKey.Tab => "Tab",
                Windows.System.VirtualKey.Space => " ",
                Windows.System.VirtualKey.Delete => "Delete",
                Windows.System.VirtualKey.Home => "Home",
                Windows.System.VirtualKey.End => "End",
                Windows.System.VirtualKey.PageUp => "PageUp",
                Windows.System.VirtualKey.PageDown => "PageDown",
                Windows.System.VirtualKey.Left => "ArrowLeft",
                Windows.System.VirtualKey.Right => "ArrowRight",
                Windows.System.VirtualKey.Up => "ArrowUp",
                Windows.System.VirtualKey.Down => "ArrowDown",
                Windows.System.VirtualKey.Control => "ControlLeft",
                Windows.System.VirtualKey.Shift => "ShiftLeft",
                Windows.System.VirtualKey.Menu => "AltLeft",
                Windows.System.VirtualKey.LeftWindows => "MetaLeft",
                Windows.System.VirtualKey.RightWindows => "MetaRight",
                _ => key.ToString(),
            };
        }

        private void DetachCurrent()
        {
            foreach (var element in _native)
            {
                element.KeyDown -= HandleKeyDown;
                element.KeyUp -= HandleKeyUp;
            }
            _native.Clear();
        }

        public void Dispose()
        {
            _view.HandlerChanged -= HandleHandlerChanged;
            _view.Unfocused -= HandleUnfocused;
            foreach (var input in _textInput.Inputs) input.HandlerChanged -= HandleHandlerChanged;
            ReleasePressed();
            DetachCurrent();
        }
    }

    private static class NativeCursor
    {
        internal static void Set(SKGLView view, DorotiMouseCursorKind cursor)
        {
            if (view.Handler?.PlatformView is not Microsoft.UI.Xaml.UIElement element) return;
            var cursorType = cursor switch
            {
                DorotiMouseCursorKind.click => Windows.UI.Core.CoreCursorType.Hand,
                DorotiMouseCursorKind.text => Windows.UI.Core.CoreCursorType.IBeam,
                DorotiMouseCursorKind.precise => Windows.UI.Core.CoreCursorType.Cross,
                DorotiMouseCursorKind.resizeLeftRight => Windows.UI.Core.CoreCursorType.SizeWestEast,
                DorotiMouseCursorKind.resizeUpDown => Windows.UI.Core.CoreCursorType.SizeNorthSouth,
                DorotiMouseCursorKind.none => Windows.UI.Core.CoreCursorType.Custom,
                _ => Windows.UI.Core.CoreCursorType.Arrow,
            };
            var property = typeof(Microsoft.UI.Xaml.UIElement).GetProperty(
                "ProtectedCursor", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            property?.SetValue(element, Microsoft.UI.Input.InputCursor.CreateFromCoreCursor(
                new Windows.UI.Core.CoreCursor(cursorType, 0)));
        }
    }
#elif IOS || MACCATALYST
    private sealed class NativeKeyboardSubscription : IDisposable
    {
        private readonly SKGLView _view;
        private readonly ulong _viewId;
        private readonly Action<KeyData> _dispatch;
        private DorotiKeyboardView? _keyboardView;

        internal NativeKeyboardSubscription(SKGLView view, MauiTextInputBridge textInput, ulong viewId, Action<KeyData> dispatch)
        {
            _view = view;
            _ = textInput;
            _viewId = viewId;
            _dispatch = dispatch;
            _view.HandlerChanged += HandleHandlerChanged;
            _view.Focused += HandleFocused;
            AttachCurrent();
        }

        private void HandleHandlerChanged(object? sender, EventArgs args) => AttachCurrent();
        private void HandleFocused(object? sender, Microsoft.Maui.Controls.FocusEventArgs args) =>
            _keyboardView?.BecomeFirstResponder();

        private void AttachCurrent()
        {
            _keyboardView?.RemoveFromSuperview();
            _keyboardView?.Dispose();
            _keyboardView = null;
            if (_view.Handler?.PlatformView is not UIKit.UIView native) return;
            _keyboardView = new DorotiKeyboardView(_viewId, _dispatch)
            {
                Frame = native.Bounds,
                BackgroundColor = UIKit.UIColor.Clear,
                UserInteractionEnabled = false,
                AutoresizingMask = UIKit.UIViewAutoresizing.FlexibleDimensions,
            };
            native.AddSubview(_keyboardView);
            _keyboardView.BecomeFirstResponder();
        }

        public void Dispose()
        {
            _view.HandlerChanged -= HandleHandlerChanged;
            _view.Focused -= HandleFocused;
            _keyboardView?.RemoveFromSuperview();
            _keyboardView?.Dispose();
            _keyboardView = null;
        }
    }

    private sealed class DorotiKeyboardView(ulong viewId, Action<KeyData> dispatch) : UIKit.UIView
    {
        public override bool CanBecomeFirstResponder => true;

        public override void PressesBegan(Foundation.NSSet<UIKit.UIPress> presses, UIKit.UIPressesEvent evt)
        {
            Dispatch(presses, KeyEventType.down);
            base.PressesBegan(presses, evt);
        }

        public override void PressesEnded(Foundation.NSSet<UIKit.UIPress> presses, UIKit.UIPressesEvent evt)
        {
            Dispatch(presses, KeyEventType.up);
            base.PressesEnded(presses, evt);
        }

        private void Dispatch(Foundation.NSSet<UIKit.UIPress> presses, KeyEventType type)
        {
            foreach (var press in presses)
            {
                if (press.Key is not { } key) continue;
                var physical = 0x70000 | (long)key.KeyCode;
                var characters = key.CharactersIgnoringModifiers ?? string.Empty;
                var name = characters.Length == 0 ? key.KeyCode.ToString() : characters;
                dispatch(new(viewId, TimeSpan.FromTicks(DateTime.UtcNow.Ticks), type,
                    physical, Logical(name, physical), false, characters.Length == 1 ? characters : null));
            }
        }
    }

    private static class NativeCursor
    {
        private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<UIKit.UIView, CursorState> States = new();

        internal static void Set(SKGLView view, DorotiMouseCursorKind cursor)
        {
            if (view.Handler?.PlatformView is not UIKit.UIView native) return;
            var state = States.GetValue(native, static value => new CursorState(value));
            state.Cursor = cursor;
            state.Interaction.Invalidate();
        }

        private sealed class CursorState : UIKit.UIPointerInteractionDelegate
        {
            internal CursorState(UIKit.UIView view)
            {
                View = view;
                Interaction = new UIKit.UIPointerInteraction(this);
                view.AddInteraction(Interaction);
            }

            internal UIKit.UIView View { get; }
            internal UIKit.UIPointerInteraction Interaction { get; }
            internal DorotiMouseCursorKind Cursor { get; set; }

            public override UIKit.UIPointerStyle? GetStyleForRegion(
                UIKit.UIPointerInteraction interaction, UIKit.UIPointerRegion region)
            {
                _ = interaction;
                _ = region;
                if (Cursor == DorotiMouseCursorKind.none) return UIKit.UIPointerStyle.CreateHiddenPointerStyle();
                UIKit.UIAxis? axis = Cursor switch
                {
                    DorotiMouseCursorKind.text or DorotiMouseCursorKind.resizeUpDown => UIKit.UIAxis.Vertical,
                    DorotiMouseCursorKind.verticalText or DorotiMouseCursorKind.resizeLeftRight => UIKit.UIAxis.Horizontal,
                    _ => null,
                };
                if (axis is null) return UIKit.UIPointerStyle.CreateSystemPointerStyle();
                var actualAxis = axis.Value;
                var length = (System.Runtime.InteropServices.NFloat)Math.Max(12, actualAxis == UIKit.UIAxis.Vertical
                    ? View.Bounds.Height : View.Bounds.Width);
                var shape = UIKit.UIPointerShape.CreateBeam(length, actualAxis);
                return UIKit.UIPointerStyle.Create(shape, actualAxis);
            }
        }
    }
#elif ANDROID
    private sealed class NativeKeyboardSubscription : IDisposable
    {
        private readonly SKGLView _view;
        private readonly MauiTextInputBridge _textInput;
        private readonly ulong _viewId;
        private readonly Action<KeyData> _dispatch;
        private readonly List<Android.Views.View> _native = [];

        internal NativeKeyboardSubscription(SKGLView view, MauiTextInputBridge textInput, ulong viewId, Action<KeyData> dispatch)
        {
            _view = view;
            _textInput = textInput;
            _viewId = viewId;
            _dispatch = dispatch;
            _view.HandlerChanged += HandleHandlerChanged;
            foreach (var input in _textInput.Inputs) input.HandlerChanged += HandleHandlerChanged;
            AttachCurrent();
        }

        private void HandleHandlerChanged(object? sender, EventArgs args) => AttachCurrent();

        private void AttachCurrent()
        {
            DetachCurrent();
            foreach (var native in new object?[] { _view.Handler?.PlatformView }
                .Concat(_textInput.Inputs.Select(input => input.Handler?.PlatformView))
                .OfType<Android.Views.View>().Distinct())
            {
                native.Focusable = true;
                native.FocusableInTouchMode = true;
                native.KeyPress += HandleKeyPress;
                _native.Add(native);
            }
        }

        private void HandleKeyPress(object? sender, Android.Views.View.KeyEventArgs args)
        {
            var nativeEvent = args.Event;
            if (nativeEvent is null) return;
            var type = nativeEvent.Action switch
            {
                Android.Views.KeyEventActions.Up => KeyEventType.up,
                _ when nativeEvent.RepeatCount > 0 => KeyEventType.repeat,
                _ => KeyEventType.down,
            };
            var physical = Physical(args.KeyCode);
            var unicode = nativeEvent.GetUnicodeChar(nativeEvent.MetaState);
            var character = unicode > 0 && !char.IsControl((char)unicode) ? char.ConvertFromUtf32(unicode) : null;
            var name = character ?? KeyName(args.KeyCode);
            _dispatch(new(_viewId, TimeSpan.FromTicks(DateTime.UtcNow.Ticks), type,
                physical, Logical(name, physical), false, character));
            args.Handled = true;
        }

        private static long Physical(Android.Views.Keycode key) => key switch
        {
            >= Android.Views.Keycode.A and <= Android.Views.Keycode.Z => 0x70004 + (int)key - (int)Android.Views.Keycode.A,
            >= Android.Views.Keycode.Num1 and <= Android.Views.Keycode.Num9 => 0x7001e + (int)key - (int)Android.Views.Keycode.Num1,
            Android.Views.Keycode.Num0 => 0x70027,
            Android.Views.Keycode.Enter => 0x70028,
            Android.Views.Keycode.Escape or Android.Views.Keycode.Back => 0x70029,
            Android.Views.Keycode.Del => 0x7002a,
            Android.Views.Keycode.Tab => 0x7002b,
            Android.Views.Keycode.Space => 0x7002c,
            Android.Views.Keycode.ForwardDel => 0x7004c,
            Android.Views.Keycode.DpadRight => 0x7004f,
            Android.Views.Keycode.DpadLeft => 0x70050,
            Android.Views.Keycode.DpadDown => 0x70051,
            Android.Views.Keycode.DpadUp => 0x70052,
            _ => 0x100000000 | (uint)key,
        };

        private static string KeyName(Android.Views.Keycode key) => key switch
        {
            Android.Views.Keycode.Enter => "Enter",
            Android.Views.Keycode.Escape or Android.Views.Keycode.Back => "Escape",
            Android.Views.Keycode.Del => "Backspace",
            Android.Views.Keycode.Tab => "Tab",
            Android.Views.Keycode.Space => " ",
            Android.Views.Keycode.ForwardDel => "Delete",
            Android.Views.Keycode.MoveHome => "Home",
            Android.Views.Keycode.MoveEnd => "End",
            Android.Views.Keycode.PageUp => "PageUp",
            Android.Views.Keycode.PageDown => "PageDown",
            Android.Views.Keycode.DpadLeft => "ArrowLeft",
            Android.Views.Keycode.DpadRight => "ArrowRight",
            Android.Views.Keycode.DpadUp => "ArrowUp",
            Android.Views.Keycode.DpadDown => "ArrowDown",
            _ => key.ToString(),
        };

        private void DetachCurrent()
        {
            foreach (var native in _native) native.KeyPress -= HandleKeyPress;
            _native.Clear();
        }

        public void Dispose()
        {
            _view.HandlerChanged -= HandleHandlerChanged;
            foreach (var input in _textInput.Inputs) input.HandlerChanged -= HandleHandlerChanged;
            DetachCurrent();
        }
    }

    private static class NativeCursor
    {
        internal static void Set(SKGLView view, DorotiMouseCursorKind cursor)
        {
            if (!OperatingSystem.IsAndroidVersionAtLeast(24) ||
                view.Handler?.PlatformView is not Android.Views.View native || native.Context is not { } context) return;
            var kind = cursor switch
            {
                DorotiMouseCursorKind.click => Android.Views.PointerIconType.Hand,
                DorotiMouseCursorKind.text => Android.Views.PointerIconType.Text,
                DorotiMouseCursorKind.verticalText => Android.Views.PointerIconType.VerticalText,
                DorotiMouseCursorKind.precise => Android.Views.PointerIconType.Crosshair,
                DorotiMouseCursorKind.resizeLeftRight => Android.Views.PointerIconType.HorizontalDoubleArrow,
                DorotiMouseCursorKind.resizeUpDown => Android.Views.PointerIconType.VerticalDoubleArrow,
                DorotiMouseCursorKind.none => Android.Views.PointerIconType.Null,
                _ => Android.Views.PointerIconType.Arrow,
            };
            native.PointerIcon = Android.Views.PointerIcon.GetSystemIcon(context, kind);
        }
    }
#else
#error Doroti.Host.Maui requires an explicit native input implementation.
#endif
}
#endif
