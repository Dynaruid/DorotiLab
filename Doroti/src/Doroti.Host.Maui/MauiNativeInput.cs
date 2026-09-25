#if IOS && !MACCATALYST
using SKGLView = Doroti.Host.Maui.DorotiSkiaView;
#endif
#if !MACOS
using Doroti.Ui;
using SkiaSharp.Views.Maui.Controls;
#if IOS || MACCATALYST
using Foundation;
using UIKit;
#endif

namespace Doroti.Host.Maui;

internal static class MauiNativeInput
{
    internal static IDisposable Attach(
        SKGLView view,
        MauiTextInputBridge textInput,
        ulong viewId,
        Action<KeyData> dispatch
    ) => new NativeKeyboardSubscription(view, textInput, viewId, dispatch);

    internal static void SetCursor(SKGLView view, DorotiMouseCursorKind cursor) =>
        NativeCursor.Set(view, cursor);

#if WINDOWS
    private sealed class NativeKeyboardSubscription : IDisposable
    {
        private readonly SKGLView _view;
        private readonly ulong _viewId;
        private readonly Action<KeyData> _dispatch;
        private readonly MauiKeyboardState _keyboard = new();
        private readonly MauiWindowsKeyboardFocus _windowFocus;
        private readonly MauiTextInputBridge _textInput;
        private readonly List<Microsoft.UI.Xaml.UIElement> _native = [];

        internal NativeKeyboardSubscription(
            SKGLView view,
            MauiTextInputBridge textInput,
            ulong viewId,
            Action<KeyData> dispatch
        )
        {
            _view = view;
            _textInput = textInput;
            _viewId = viewId;
            _dispatch = dispatch;
            _windowFocus = new(view, ReleasePressed);
            _view.HandlerChanged += HandleHandlerChanged;
            _view.Unfocused += HandleUnfocused;
            foreach (var input in _textInput.Inputs)
            {
                input.HandlerChanged += HandleHandlerChanged;
            }

            AttachCurrent();
        }

        private void HandleHandlerChanged(object? sender, EventArgs args) => AttachCurrent();

        private void AttachCurrent()
        {
            DetachCurrent();
            foreach (
                var element in new object?[] { _view.Handler?.PlatformView }
                    .Concat(_textInput.Inputs.Select(input => input.Handler?.PlatformView))
                    .OfType<Microsoft.UI.Xaml.UIElement>()
                    .Distinct()
            )
            {
                element.KeyDown += HandleKeyDown;
                element.KeyUp += HandleKeyUp;
                element.LostFocus += HandleNativeLostFocus;
                _native.Add(element);
            }
        }

        private void HandleKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs args)
        {
            if (NativeTextInputOwnsKey(sender, args))
            {
                return;
            }

            Dispatch(
                args,
                args.KeyStatus.WasKeyDown ? KeyEventType.repeat : KeyEventType.down
            );
        }

        private void HandleKeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs args)
        {
            if (NativeTextInputOwnsKey(sender, args))
            {
                return;
            }

            Dispatch(args, KeyEventType.up);
        }

        private bool NativeTextInputOwnsKey(
            object sender,
            Microsoft.UI.Xaml.Input.KeyRoutedEventArgs args
        )
        {
            // Entry and Editor are native TextBox subclasses on MAUI Windows.
            // When one is the active text endpoint, WinUI must apply hardware
            // edits exactly once and publish the result through TextChanged.
            if (!_textInput.HasClient || sender is not Microsoft.UI.Xaml.Controls.TextBox)
            {
                return false;
            }

            args.Handled = false;
            return true;
        }

        private void Dispatch(
            Microsoft.UI.Xaml.Input.KeyRoutedEventArgs args,
            KeyEventType type
        )
        {
            if (_keyboard.Apply(MauiWindowsKeyboard.Translate(_viewId, args, type)) is { } key)
            {
                _dispatch(key);
            }
            args.Handled = true;
        }

        private void HandleNativeLostFocus(object sender, Microsoft.UI.Xaml.RoutedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.UIElement owner && MauiWindowsKeyboard.OwnsFocus(owner)) return;
            ReleasePressed();
        }

        private void HandleUnfocused(object? sender, FocusEventArgs args) => ReleasePressed();

        private void ReleasePressed()
        {
            foreach (var key in _keyboard.ReleaseAll(_viewId, DorotiFrameClock.Now))
            {
                _dispatch(key);
            }
        }

        private void DetachCurrent()
        {
            ReleasePressed();
            foreach (var element in _native)
            {
                element.KeyDown -= HandleKeyDown;
                element.KeyUp -= HandleKeyUp;
                element.LostFocus -= HandleNativeLostFocus;
            }
            _native.Clear();
        }

        public void Dispose()
        {
            _windowFocus.Dispose();
            _view.HandlerChanged -= HandleHandlerChanged;
            _view.Unfocused -= HandleUnfocused;
            foreach (var input in _textInput.Inputs)
            {
                input.HandlerChanged -= HandleHandlerChanged;
            }

            ReleasePressed();
            DetachCurrent();
        }
    }

    private static class NativeCursor
    {
        internal static void Set(SKGLView view, DorotiMouseCursorKind cursor)
        {
            if (view.Handler?.PlatformView is not Microsoft.UI.Xaml.UIElement element)
            {
                return;
            }

            var cursorType = cursor switch
            {
                DorotiMouseCursorKind.click => Windows.UI.Core.CoreCursorType.Hand,
                DorotiMouseCursorKind.text => Windows.UI.Core.CoreCursorType.IBeam,
                DorotiMouseCursorKind.precise => Windows.UI.Core.CoreCursorType.Cross,
                DorotiMouseCursorKind.resizeLeftRight => Windows
                    .UI
                    .Core
                    .CoreCursorType
                    .SizeWestEast,
                DorotiMouseCursorKind.resizeUpDown => Windows.UI.Core.CoreCursorType.SizeNorthSouth,
                DorotiMouseCursorKind.none => Windows.UI.Core.CoreCursorType.Custom,
                _ => Windows.UI.Core.CoreCursorType.Arrow,
            };
            var property = typeof(Microsoft.UI.Xaml.UIElement).GetProperty(
                "ProtectedCursor",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic
            );
            property?.SetValue(
                element,
                Microsoft.UI.Input.InputCursor.CreateFromCoreCursor(
                    new Windows.UI.Core.CoreCursor(cursorType, 0)
                )
            );
        }
    }
#elif IOS || MACCATALYST
    private sealed class NativeKeyboardSubscription : IDisposable
    {
        private readonly SKGLView _view;
        private readonly MauiTextInputBridge _textInput;
        private readonly ulong _viewId;
        private readonly Action<KeyData> _dispatch;
        private DorotiKeyboardView? _keyboardView;

        internal NativeKeyboardSubscription(
            SKGLView view,
            MauiTextInputBridge textInput,
            ulong viewId,
            Action<KeyData> dispatch
        )
        {
            _view = view;
            _textInput = textInput;
            _viewId = viewId;
            _dispatch = dispatch;
            _view.HandlerChanged += HandleHandlerChanged;
            _view.Focused += HandleFocused;
            AttachCurrent();
        }

        private void HandleHandlerChanged(object? sender, EventArgs args) => AttachCurrent();

        private void HandleFocused(object? sender, FocusEventArgs args) =>
            FocusHardwareKeyboardViewIfAvailable();

        private void FocusHardwareKeyboardViewIfAvailable()
        {
            // DorotiKeyboardView only receives physical-key presses. It must never
            // take first responder from the Entry/Editor that owns the software IME.
            if (!_textInput.HasClient)
            {
                _keyboardView?.BecomeFirstResponder();
            }
        }

        private void AttachCurrent()
        {
            _keyboardView?.RemoveFromSuperview();
            _keyboardView?.Dispose();
            _keyboardView = null;
            if (_view.Handler?.PlatformView is not UIView native)
            {
                return;
            }

            _keyboardView = new DorotiKeyboardView(_viewId, _dispatch)
            {
                Frame = native.Bounds,
                BackgroundColor = UIKit.UIColor.Clear,
                UserInteractionEnabled = false,
                AutoresizingMask = UIKit.UIViewAutoresizing.FlexibleDimensions,
            };
            native.AddSubview(_keyboardView);
            FocusHardwareKeyboardViewIfAvailable();
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

    private sealed class DorotiKeyboardView(ulong viewId, Action<KeyData> dispatch) : UIView
    {
        public override bool CanBecomeFirstResponder => true;

        public override void PressesBegan(NSSet<UIPress> presses, UIPressesEvent evt)
        {
            Dispatch(presses, KeyEventType.down);
            base.PressesBegan(presses, evt);
        }

        public override void PressesEnded(NSSet<UIPress> presses, UIPressesEvent evt)
        {
            Dispatch(presses, KeyEventType.up);
            base.PressesEnded(presses, evt);
        }

        private void Dispatch(NSSet<UIPress> presses, KeyEventType type)
        {
            foreach (var press in presses)
            {
                if (press.Key is not { } key)
                {
                    continue;
                }

                var physical = 0x70000 | (long)key.KeyCode;
                var characters = key.CharactersIgnoringModifiers ?? string.Empty;
                var name = characters.Length == 0 ? key.KeyCode.ToString() : characters;
                dispatch(
                    new(
                        viewId,
                        TimeSpan.FromTicks(DateTime.UtcNow.Ticks),
                        type,
                        physical,
                        MauiKeyMap.Logical(name, physical),
                        false,
                        type != KeyEventType.up
                        && characters.Length == 1
                        && !char.IsControl(characters[0])
                            ? characters
                            : null
                    )
                );
            }
        }
    }

    private static class NativeCursor
    {
        private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<
            UIView,
            CursorState
        > States = new();

        internal static void Set(SKGLView view, DorotiMouseCursorKind cursor)
        {
            if (view.Handler?.PlatformView is not UIView native)
            {
                return;
            }

            var state = States.GetValue(native, static value => new CursorState(value));
            state.Cursor = cursor;
            state.Interaction.Invalidate();
        }

        private sealed class CursorState : UIPointerInteractionDelegate
        {
            internal CursorState(UIView view)
            {
                View = view;
                Interaction = new UIPointerInteraction(this);
                view.AddInteraction(Interaction);
            }

            internal UIView View { get; }
            internal UIPointerInteraction Interaction { get; }
            internal DorotiMouseCursorKind Cursor { get; set; }

            public override UIPointerStyle? GetStyleForRegion(
                UIPointerInteraction interaction,
                UIPointerRegion region
            )
            {
                _ = interaction;
                _ = region;
                if (Cursor == DorotiMouseCursorKind.none)
                {
                    return UIKit.UIPointerStyle.CreateHiddenPointerStyle();
                }

                UIAxis? axis = Cursor switch
                {
                    DorotiMouseCursorKind.text or DorotiMouseCursorKind.resizeUpDown => UIKit
                        .UIAxis
                        .Vertical,
                    DorotiMouseCursorKind.verticalText or DorotiMouseCursorKind.resizeLeftRight =>
                        UIKit.UIAxis.Horizontal,
                    _ => null,
                };
                if (axis is null)
                {
                    return UIKit.UIPointerStyle.CreateSystemPointerStyle();
                }

                var actualAxis = axis.Value;
                var length = (nfloat)
                    Math.Max(
                        12,
                        actualAxis == UIKit.UIAxis.Vertical ? View.Bounds.Height : View.Bounds.Width
                    );
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
        private readonly List<Android.Views.ViewTreeObserver> _observers = [];
        private readonly MauiKeyboardState _keyboard = new();
        private readonly WindowFocusListener _windowFocusListener;

        private sealed class WindowFocusListener(Action release) : Java.Lang.Object,
            Android.Views.ViewTreeObserver.IOnWindowFocusChangeListener
        {
            public void OnWindowFocusChanged(bool hasFocus)
            {
                if (!hasFocus) release();
            }
        }

        internal NativeKeyboardSubscription(
            SKGLView view,
            MauiTextInputBridge textInput,
            ulong viewId,
            Action<KeyData> dispatch
        )
        {
            _view = view;
            _textInput = textInput;
            _viewId = viewId;
            _dispatch = dispatch;
            _windowFocusListener = new(ReleasePressed);
            _view.HandlerChanged += HandleHandlerChanged;
            foreach (var input in _textInput.Inputs)
            {
                input.HandlerChanged += HandleHandlerChanged;
            }

            AttachCurrent();
        }

        private void HandleHandlerChanged(object? sender, EventArgs args) => AttachCurrent();

        private void AttachCurrent()
        {
            DetachCurrent();
            foreach (
                var native in new object?[]
                {
                    _view.Handler?.PlatformView,
                    (_view.Handler?.PlatformView as DorotiAndroidViewContainer)?.Surface,
                }
                    .Concat(_textInput.Inputs.Select(input => input.Handler?.PlatformView))
                    .OfType<Android.Views.View>()
                    .Distinct()
            )
            {
                native.Focusable = true;
                native.FocusableInTouchMode = true;
                native.KeyPress += HandleKeyPress;
                native.FocusChange += HandleFocusChange;
                if (native.ViewTreeObserver is { IsAlive: true } observer && !_observers.Contains(observer))
                {
                    observer.AddOnWindowFocusChangeListener(_windowFocusListener);
                    _observers.Add(observer);
                }
                _native.Add(native);
            }
        }

        private void HandleKeyPress(object? sender, Android.Views.View.KeyEventArgs args)
        {
            var nativeEvent = args.Event;
            if (nativeEvent is null)
            {
                return;
            }
            // The hidden EditText is the Android InputConnection endpoint for
            // an active framework text client. Let it apply IME and hardware
            // edits so TextChanged can publish the resulting text, selection,
            // and composing range. Consuming these events here prevents
            // Samsung Keyboard's Backspace (and physical typing) outright.
            if (_textInput.HasClient && sender is Android.Widget.EditText)
            {
                args.Handled = false;
                return;
            }
            if (nativeEvent.Action is not (Android.Views.KeyEventActions.Down or Android.Views.KeyEventActions.Up))
            {
                args.Handled = false;
                return;
            }
            var type = nativeEvent.Action switch
            {
                Android.Views.KeyEventActions.Up => KeyEventType.up,
                _ when nativeEvent.RepeatCount > 0 => KeyEventType.repeat,
                _ => KeyEventType.down,
            };
            var physical = MauiKeyMap.AndroidPhysical((int)args.KeyCode, nativeEvent.ScanCode);
            var character = MauiKeyMap.AndroidCharacter(nativeEvent.GetUnicodeChar(nativeEvent.MetaState));
            var name = character ?? KeyName(args.KeyCode);
            // Logical fallback follows the Android keycode, not the physical position.
            var logical = MauiKeyMap.Logical(name, MauiKeyMap.AndroidPhysical((int)args.KeyCode));
            if (_keyboard.Apply(new(_viewId, DorotiFrameClock.Now, type, physical,
                logical, false, type == KeyEventType.up ? null : character)) is { } key)
            {
                _dispatch(key);
            }
            args.Handled = true;
        }

        private static string KeyName(Android.Views.Keycode key) =>
            key switch
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

        private void HandleFocusChange(object? sender, Android.Views.View.FocusChangeEventArgs args)
        {
            if (!args.HasFocus) ReleasePressed();
        }

        private void ReleasePressed()
        {
            foreach (var key in _keyboard.ReleaseAll(_viewId, DorotiFrameClock.Now)) _dispatch(key);
        }

        private void DetachCurrent()
        {
            ReleasePressed();
            // Before attachment Android may merge a floating ViewTreeObserver
            // into the root. Remove the same listener from the current root too.
            foreach (var observer in _observers.Concat(_native.Select(view => view.ViewTreeObserver)
                .OfType<Android.Views.ViewTreeObserver>()).Distinct())
            {
                if (observer.IsAlive) observer.RemoveOnWindowFocusChangeListener(_windowFocusListener);
            }
            _observers.Clear();
            foreach (var native in _native)
            {
                native.KeyPress -= HandleKeyPress;
                native.FocusChange -= HandleFocusChange;
            }

            _native.Clear();
        }

        public void Dispose()
        {
            _view.HandlerChanged -= HandleHandlerChanged;
            foreach (var input in _textInput.Inputs)
            {
                input.HandlerChanged -= HandleHandlerChanged;
            }

            DetachCurrent();
            _windowFocusListener.Dispose();
        }
    }

    private static class NativeCursor
    {
        internal static void Set(SKGLView view, DorotiMouseCursorKind cursor)
        {
            if (
                !OperatingSystem.IsAndroidVersionAtLeast(24)
                || view.Handler?.PlatformView is not Android.Views.View native
                || native.Context is not { } context
            )
            {
                return;
            }

            var kind = cursor switch
            {
                DorotiMouseCursorKind.click => Android.Views.PointerIconType.Hand,
                DorotiMouseCursorKind.text => Android.Views.PointerIconType.Text,
                DorotiMouseCursorKind.verticalText => Android.Views.PointerIconType.VerticalText,
                DorotiMouseCursorKind.precise => Android.Views.PointerIconType.Crosshair,
                DorotiMouseCursorKind.resizeLeftRight => Android
                    .Views
                    .PointerIconType
                    .HorizontalDoubleArrow,
                DorotiMouseCursorKind.resizeUpDown => Android
                    .Views
                    .PointerIconType
                    .VerticalDoubleArrow,
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
