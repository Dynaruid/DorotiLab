#if IOS && !MACCATALYST
using CoreGraphics;
using Doroti.Hosting;
using Doroti.Ui;
using Foundation;
using UIKit;
using WebKit;
using Rect = Doroti.Ui.Rect;

namespace Doroti.Host.Maui;

public sealed class UIKitPlatformViewDispatcher : IPlatformViewDispatcher
{
    public ValueTask InvokeAsync(Func<ValueTask> action)
    {
        if (NSThread.IsMain) return action();
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        UIApplication.SharedApplication.BeginInvokeOnMainThread(async () =>
        {
            try { await action(); completion.SetResult(); }
            catch (Exception error) { completion.SetException(error); }
        });
        return new(completion.Task);
    }
    internal static void VerifyThread()
    {
        if (!NSThread.IsMain) throw new InvalidOperationException("UIKit PlatformView requires the main thread.");
    }
}

/// <summary>Owner-local native controls and WKWebView. Navigation/JS APIs belong to the WebView package.</summary>
internal sealed class UIKitPlatformViewFactory(Func<UIView> parent, string viewType, Action beforeFocus) : IPlatformViewFactory
{
    public string ViewType => viewType;
    public PlatformViewSupport QuerySupport(PlatformViewRequest request)
    {
        var supported = request.ViewType == ViewType &&
            request.Composition is PlatformViewComposition.NativeOverlay or PlatformViewComposition.InterleavedComposition &&
            (request.Effects & ~PlatformViewEffects.RectClip) == 0;
        return new("UIKit-Metal", Environment.OSVersion.VersionString, ViewType, supported,
            request.Composition, PlatformViewEffects.RectClip,
            Capabilities: new(PlatformViewRepresentation.NativeHierarchy, PlatformViewTransport.GpuShared,
                PlatformViewInputPolicy.DirectNative, UIKitPlatformViewHost.MaterialEffects),
            Reason: supported ? null : "UIKit supports translation, rectangular clip and direct native input on Graphite Metal.");
    }
    public ValueTask<IPlatformViewInstance> CreateAsync(PlatformViewHandle handle, ReadOnlyMemory<byte> parameters,
        Action<PlatformViewHandle> onFocused, CancellationToken cancellationToken)
    {
        UIKitPlatformViewDispatcher.VerifyThread();
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult<IPlatformViewInstance>(new Instance(parent, ViewType, beforeFocus, handle,
            parameters.IsEmpty ? "Native control" : System.Text.Encoding.UTF8.GetString(parameters.Span), onFocused));
    }
    private sealed class NativeEditor(Action beforeFocus, Action focused) : UITextField
    {
        public override bool BecomeFirstResponder()
        {
            beforeFocus();
            var result = base.BecomeFirstResponder();
            if (result) focused();
            return result;
        }
    }
    private sealed class NativeWebView : WKWebView
    {
        private readonly Action _beforeFocus;
        private readonly Action _focused;
        internal NativeWebView(Action beforeFocus, Action focused, WKWebViewConfiguration configuration)
            : base(CGRect.Empty, configuration) { _beforeFocus = beforeFocus; _focused = focused; }
        public override bool BecomeFirstResponder()
        {
            _beforeFocus();
            var result = base.BecomeFirstResponder();
            if (result) _focused();
            return result;
        }
    }
    private sealed class Instance : IPlatformViewInstance
    {
        private readonly Func<UIView> _parent;
        private readonly Action _beforeFocus;
        private readonly Action<PlatformViewHandle> _focused;
        private readonly PlatformViewHandle _handle;
        private readonly UIView _clip = new() { ClipsToBounds = true, Hidden = true, BackgroundColor = UIColor.Clear };
        private readonly UIView _control;
        private bool _disabled, _disposed;
        private int _clicks;
        internal Instance(Func<UIView> parent, string type, Action beforeFocus, PlatformViewHandle handle,
            string text, Action<PlatformViewHandle> focused)
        {
            _parent = parent; _beforeFocus = beforeFocus; _handle = handle; _focused = focused;
            if (type == "doroti/native-editor")
                _control = new NativeEditor(BeforeFocus, Focused) { Text = text, BorderStyle = UITextBorderStyle.RoundedRect,
                    BackgroundColor = UIColor.SecondarySystemBackground, AccessibilityLabel = "Native editor" };
            else if (type == "doroti/webview")
            {
                using var configuration = new WKWebViewConfiguration();
                var web = new NativeWebView(BeforeFocus, Focused, configuration);
                web.LoadHtmlString(text, null!);
                _control = web;
            }
            else
            {
                var button = new UIButton(UIButtonType.System) { BackgroundColor = UIColor.SecondarySystemBackground };
                button.SetTitle(text, UIControlState.Normal);
                button.TouchUpInside += Activated;
                _control = button;
            }
            _control.AccessibilityIdentifier = $"doroti-platform-view-{handle.OwnerViewId}-{handle.InstanceId}-{handle.InstanceGeneration}";
            _clip.AddSubview(_control);
        }
        private void Activated(object? sender, EventArgs args)
        {
            if (!_disabled && !_disposed && !_clip.Hidden && _control is UIButton button)
                button.SetTitle($"Native clicks: {++_clicks}", UIControlState.Normal);
        }
        private void BeforeFocus() { if (!_disabled && !_disposed) _beforeFocus(); }
        private void Focused()
        {
            if (_disabled || _disposed || _clip.Hidden) return;
            try { _focused(_handle); }
            catch (Exception error) { System.Diagnostics.Trace.TraceError(error.ToString()); }
        }
        public ValueTask ApplyAsync(PlatformViewPlacement placement)
        {
            UIKitPlatformViewDispatcher.VerifyThread();
            ObjectDisposedException.ThrowIf(_disposed, this);
            placement.Validate();
            if (_disabled || placement.Handle != _handle || !placement.Transform.IsAxisAligned ||
                placement.Transform.M11 != 1 || placement.Transform.M22 != 1)
                throw new InvalidOperationException("UIKit requires live owner-local identity, translation and rectangular clipping.");
            var parent = _parent();
            if (_clip.Superview is { } previous && previous != parent)
                throw new InvalidOperationException("A live UIKit PlatformView cannot move between owners.");
            var origin = placement.Transform.Map(placement.Bounds.topLeft);
            var bounds = Rect.fromLTWH(origin.dx, origin.dy, placement.Bounds.width, placement.Bounds.height);
            var visible = (placement.Clip is { } clip ? bounds.intersect(clip) : bounds)
                .intersect(Rect.fromLTWH(0, 0, parent.Bounds.Width, parent.Bounds.Height));
            if (!placement.Visible || visible.isEmpty || bounds.isEmpty)
            { _clip.EndEditing(true); _clip.Hidden = true; return ValueTask.CompletedTask; }
            _clip.Frame = new CGRect(visible.left, visible.top, visible.width, visible.height);
            _clip.Layer.ZPosition = placement.PaintOrder;
            _control.Frame = new CGRect(bounds.left - visible.left, bounds.top - visible.top, bounds.width, bounds.height);
            if (_clip.Superview is null) parent.AddSubview(_clip);
            _clip.Hidden = false;
            return ValueTask.CompletedTask;
        }
        public ValueTask SetFocusAsync(bool focused)
        {
            UIKitPlatformViewDispatcher.VerifyThread();
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (focused)
            {
                if (_disabled || _clip.Hidden || _clip.Window is null) throw new InvalidOperationException("Native focus requires a visible attachment.");
                if (!_control.BecomeFirstResponder()) throw new InvalidOperationException("UIKit rejected native focus.");
            }
            else _clip.EndEditing(true);
            return ValueTask.CompletedTask;
        }
        public ValueTask DetachAsync()
        {
            UIKitPlatformViewDispatcher.VerifyThread();
            if (_disposed) return ValueTask.CompletedTask;
            _clip.EndEditing(true); _clip.Hidden = true; _clip.RemoveFromSuperview();
            return ValueTask.CompletedTask;
        }
        public ValueTask DisableInputAsync()
        {
            UIKitPlatformViewDispatcher.VerifyThread();
            if (_disposed) return ValueTask.CompletedTask;
            _disabled = true; _clip.UserInteractionEnabled = false;
            return DetachAsync();
        }
        public ValueTask DisposeAsync()
        {
            UIKitPlatformViewDispatcher.VerifyThread();
            if (_disposed) return ValueTask.CompletedTask;
            DisableInputAsync(); _disposed = true;
            if (_control is UIButton button) button.TouchUpInside -= Activated;
            if (_control is WKWebView web) web.StopLoading();
            _control.RemoveFromSuperview(); _control.Dispose(); _clip.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
#endif
