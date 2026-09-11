#if MACOS
using AppKit;
using CoreGraphics;
using Doroti.Hosting;
using Doroti.Ui;
using Foundation;
using Rect = Doroti.Ui.Rect;

namespace Doroti.Host.Maui;

/// <summary>AppKit UI dispatcher. Native creation and all attachment mutations stay on the main thread.</summary>
public sealed class AppKitPlatformViewDispatcher : IPlatformViewDispatcher
{
    public ValueTask InvokeAsync(Func<ValueTask> action)
    {
        if (NSThread.IsMain) return action();
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        NSApplication.SharedApplication.BeginInvokeOnMainThread(async () =>
        {
            try { await action(); completion.SetResult(); }
            catch (Exception error) { completion.SetException(error); }
        });
        return new(completion.Task);
    }

    internal static void VerifyThread()
    {
        if (!NSThread.IsMain) throw new InvalidOperationException("AppKit PlatformView requires the main thread.");
    }
}

/// <summary>Generic NSButton/NSTextField attachment; native handles never leave the AppKit backend.</summary>
public sealed class AppKitPlatformViewFactory : IPlatformViewFactory
{
    private readonly Func<NSView> _parent;
    private readonly bool _editor;
    private readonly Action? _beforeFocus;
    private readonly Action? _restoreFocus;
    private readonly bool _interleaved;

    public AppKitPlatformViewFactory(Func<NSView> parent, bool editor, Action? beforeFocus = null, Action? restoreFocus = null,
        bool interleaved = false)
    {
        _parent = parent ?? throw new ArgumentNullException(nameof(parent));
        _editor = editor;
        _beforeFocus = beforeFocus;
        _restoreFocus = restoreFocus;
        _interleaved = interleaved;
    }

    public string ViewType => _editor ? "doroti/native-editor" : "doroti/native-button";
    public PlatformViewSupport QuerySupport(PlatformViewRequest request)
    {
        var supported = request.ViewType == ViewType && (request.Composition == PlatformViewComposition.NativeOverlay ||
            _interleaved && request.Composition == PlatformViewComposition.InterleavedComposition) &&
            (request.Effects & ~PlatformViewEffects.RectClip) == 0;
        return new("AppKit-NSView", Environment.OSVersion.VersionString, ViewType, supported,
            request.Composition, PlatformViewEffects.RectClip,
            Reason: supported ? null : "This AppKit attachment requires a matching compositor and supports only translation and rectangular clipping.");
    }

    public ValueTask<IPlatformViewInstance> CreateAsync(PlatformViewHandle handle, ReadOnlyMemory<byte> parameters,
        Action<PlatformViewHandle> onFocused, CancellationToken cancellationToken)
    {
        AppKitPlatformViewDispatcher.VerifyThread();
        cancellationToken.ThrowIfCancellationRequested();
        var text = parameters.IsEmpty ? (_editor ? "Native editor" : "Native button") : System.Text.Encoding.UTF8.GetString(parameters.Span);
        return ValueTask.FromResult<IPlatformViewInstance>(new Instance(this, handle, text, onFocused));
    }

    private sealed class ClipView : NSView
    {
        public ClipView() { WantsLayer = true; Layer!.MasksToBounds = true; Hidden = true; }
        public override bool IsFlipped => true;
        public bool InputEnabled { get; set; } = true;
        public override NSView? HitTest(CGPoint point) => InputEnabled ? base.HitTest(point) : null;
    }

    private sealed class NativeButton(Action beforeFocus, Action focused) : NSButton
    {
        public override bool AcceptsFirstResponder() => true;
        public override bool CanBecomeKeyView => Enabled && !IsHiddenOrHasHiddenAncestor;
        public override bool BecomeFirstResponder()
        {
            beforeFocus();
            var result = base.BecomeFirstResponder();
            if (result) focused();
            return result;
        }
        public override void MouseDown(NSEvent theEvent)
        {
            Window?.MakeFirstResponder(this);
            base.MouseDown(theEvent);
        }
    }

    private sealed class NativeEditor(Action beforeFocus, Action focused) : NSTextField
    {
        public override bool BecomeFirstResponder()
        {
            beforeFocus();
            var result = base.BecomeFirstResponder();
            if (result) focused();
            return result;
        }
        public override void MouseDown(NSEvent theEvent)
        {
            beforeFocus();
            base.MouseDown(theEvent);
        }
    }

    private sealed class Instance : IPlatformViewInstance
    {
        private readonly AppKitPlatformViewFactory _factory;
        private readonly PlatformViewHandle _handle;
        private readonly Action<PlatformViewHandle> _focused;
        private readonly ClipView _clip = new();
        private readonly NSControl _control;
        private readonly NSObject? _editingObserver;
        private bool _inputEnabled = true;
        private bool _disposed;
        private int _clicks;

        public Instance(AppKitPlatformViewFactory factory, PlatformViewHandle handle, string text, Action<PlatformViewHandle> focused)
        {
            _factory = factory; _handle = handle; _focused = focused;
            if (factory._editor)
            {
                var editor = new NativeEditor(BeforeFocus, Focused) { StringValue = text, Editable = true, Selectable = true, Bezeled = true };
                _control = editor;
                _editingObserver = NSNotificationCenter.DefaultCenter.AddObserver(NSControl.TextDidBeginEditingNotification,
                    _ => { BeforeFocus(); Focused(); }, editor);
            }
            else
            {
                // FlexiblePush supports the widget's arbitrary bounded height; fixed-height
                // rounded bezels can omit their background when stretched to a tall slot.
                var button = new NativeButton(BeforeFocus, Focused) { Title = text, BezelStyle = NSBezelStyle.FlexiblePush };
                button.SetButtonType(NSButtonType.MomentaryPushIn);
                button.Bordered = true;
                // Native bezel vibrancy cannot sample the sibling Metal surface. Supply an
                // opaque native backing so its label remains readable on either app background.
                button.WantsLayer = true;
                button.Layer!.BackgroundColor = NSColor.ControlBackground.CGColor;
                button.Activated += Activated;
                _control = button;
            }
            _control.Identifier = $"doroti-platform-view-{handle.OwnerViewId}-{handle.InstanceId}-{handle.InstanceGeneration}";
            _clip.AddSubview(_control);
        }

        private void Activated(object? sender, EventArgs args)
        {
            if (_inputEnabled && !_disposed && _control is NSButton button) button.Title = $"Native clicks: {++_clicks}";
        }
        private void BeforeFocus() { if (_inputEnabled && !_disposed) _factory._beforeFocus?.Invoke(); }
        private void Focused()
        {
            if (!_inputEnabled || _disposed || _clip.Hidden) return;
            // Managed callbacks must not unwind through an Objective-C responder callback.
            try { _focused(_handle); }
            catch (Exception error) { System.Diagnostics.Trace.TraceError(error.ToString()); }
        }

        public ValueTask ApplyAsync(PlatformViewPlacement placement)
        {
            AppKitPlatformViewDispatcher.VerifyThread();
            ObjectDisposedException.ThrowIf(_disposed, this);
            placement.Validate();
            if (placement.Handle != _handle || placement.Transform.M11 != 1 || placement.Transform.M22 != 1 || !placement.Transform.IsAxisAligned)
                throw new InvalidOperationException("AppKit NativeOverlay only supports owner-local translation and rectangular clipping.");
            if (!_inputEnabled) throw new InvalidOperationException("Removed native view cannot be reattached.");
            var parent = _factory._parent();
            if (_clip.Superview is { } current && current != parent)
                throw new InvalidOperationException("Reparenting a live AppKit PlatformView is unsupported.");
            var origin = placement.Transform.Map(placement.Bounds.topLeft);
            var bounds = Rect.fromLTWH(origin.dx, origin.dy, placement.Bounds.width, placement.Bounds.height);
            var visible = placement.Clip is { } clip ? bounds.intersect(clip) : bounds;
            visible = visible.intersect(Rect.fromLTWH(0, 0, parent.Bounds.Width, parent.Bounds.Height));
            if (!placement.Visible || visible.isEmpty || bounds.isEmpty)
            {
                ReleaseFocus();
                _clip.Hidden = true;
                return ValueTask.CompletedTask;
            }
            // NSView frames are in logical points. AppKit owns backing-scale conversion.
            var y = parent.IsFlipped ? visible.top : parent.Bounds.Height - visible.bottom;
            _clip.Frame = new CGRect(parent.Bounds.X + visible.left, parent.Bounds.Y + y, visible.width, visible.height);
            _clip.Layer!.ZPosition = placement.PaintOrder;
            _control.Frame = new CGRect(bounds.left - visible.left, bounds.top - visible.top, bounds.width, bounds.height);
            if (_clip.Superview is null) parent.AddSubview(_clip);
            _clip.Hidden = false;
            return ValueTask.CompletedTask;
        }

        private bool OwnsFocus() => _clip.Window?.FirstResponder is { } responder &&
            (responder == _control || _control is NSTextField field && responder == field.CurrentEditor);
        private void ReleaseFocus()
        {
            if (!OwnsFocus()) return;
            _clip.Window?.MakeFirstResponder(null);
            _factory._restoreFocus?.Invoke();
        }
        public ValueTask SetFocusAsync(bool focused)
        {
            AppKitPlatformViewDispatcher.VerifyThread();
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (focused)
            {
                if (!_inputEnabled || _clip.Hidden || _clip.Window is null)
                    throw new InvalidOperationException("Only an attached, visible AppKit control can receive focus.");
                if (!_clip.Window.MakeFirstResponder(_control)) throw new InvalidOperationException("AppKit rejected native focus.");
            }
            else ReleaseFocus();
            return ValueTask.CompletedTask;
        }
        public ValueTask DetachAsync()
        {
            AppKitPlatformViewDispatcher.VerifyThread();
            if (_disposed) return ValueTask.CompletedTask;
            ReleaseFocus(); _clip.Hidden = true; _clip.RemoveFromSuperview();
            return ValueTask.CompletedTask;
        }
        public ValueTask DisableInputAsync()
        {
            AppKitPlatformViewDispatcher.VerifyThread();
            if (_disposed) return ValueTask.CompletedTask;
            _inputEnabled = false; _clip.InputEnabled = false; _control.Enabled = false;
            ReleaseFocus(); _clip.Hidden = true;
            return ValueTask.CompletedTask;
        }
        public ValueTask DisposeAsync()
        {
            AppKitPlatformViewDispatcher.VerifyThread();
            if (_disposed) return ValueTask.CompletedTask;
            DisableInputAsync(); DetachAsync(); _disposed = true;
            _editingObserver?.Dispose();
            if (_control is NSButton button) button.Activated -= Activated;
            _control.RemoveFromSuperview(); _control.Dispose(); _clip.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
#endif
