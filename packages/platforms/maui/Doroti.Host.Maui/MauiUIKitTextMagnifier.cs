#if IOS
using CoreGraphics;
using Doroti.Ui;
using UIKit;
using Rect = Doroti.Ui.Rect;

namespace Doroti.Host.Maui;

public sealed partial class MauiTextInputBridge
{
    private UIKitTextMagnifierSession? _systemMagnifier;

    internal ITextMagnifierSession? CreateMagnifierSession()
    {
        if (!OperatingSystem.IsIOSVersionAtLeast(17) || _visualHost is null || _disposed)
        {
            return null;
        }

        var session = new UIKitTextMagnifierSession(this);
        DispatchInputMutation(() =>
        {
            ResetSystemMagnifier();
            if (_disposed || _suspended)
            {
                session.DisposeCore();
                return;
            }
            _systemMagnifier = session;
        });
        return session;
    }

    private void ResetSystemMagnifier()
    {
        _systemMagnifier?.DisposeCore();
        _systemMagnifier = null;
    }

    private sealed class UIKitTextMagnifierSession(MauiTextInputBridge owner) : ITextMagnifierSession
    {
        private UITextLoupeSession? _loupe;
        private UIView? _view;
        private bool _disposed;

        public void Update(Offset position, Rect caretRect) => owner.DispatchInputMutation(() =>
        {
            if (!OperatingSystem.IsIOSVersionAtLeast(17) || _disposed || owner._disposed
                || owner._suspended || !ReferenceEquals(owner._systemMagnifier, this))
            {
                return;
            }
            if (owner._visualHost?.Handler?.PlatformView is not UIView view || view.Window is null)
            {
                HideCore();
                return;
            }

            if (!ReferenceEquals(_view, view))
            {
                HideCore();
                _view = view;
            }

            // The text endpoint is transparent and positioned for IME use. Anchor
            // the loupe in the visible Doroti surface's coordinate space instead.
            var point = new CGPoint(position.dx, position.dy);
            // Doroti paints the caret; there is no UIKit selection widget to animate from.
            _loupe ??= UITextLoupeSession.BeginLoupeSession(point, null, view);
            _loupe?.MoveToPoint(point,
                new CGRect(caretRect.left, caretRect.top, caretRect.width, caretRect.height), true);
        });

        public void Hide() => owner.DispatchInputMutation(HideCore);

        private void HideCore()
        {
            if (OperatingSystem.IsIOSVersionAtLeast(17))
            {
                _loupe?.Invalidate();
                _loupe?.Dispose();
                _loupe = null;
            }
            _view = null;
        }

        public void Dispose() => owner.DispatchInputMutation(() =>
        {
            DisposeCore();
            if (ReferenceEquals(owner._systemMagnifier, this))
            {
                owner._systemMagnifier = null;
            }
        });

        internal void DisposeCore()
        {
            _disposed = true;
            HideCore();
        }
    }
}
#endif
