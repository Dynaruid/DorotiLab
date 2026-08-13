// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/_window_win32.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public delegate void HWNDIo();

public static partial class _window_win32Library
{
    internal static long _WM_DESTROY = 2L;
}

public static partial class _window_win32Library
{
    internal static long _WM_SIZE = 5L;
}

public static partial class _window_win32Library
{
    internal static long _WM_ACTIVATE = 6L;
}

public static partial class _window_win32Library
{
    internal static long _WM_CLOSE = 16L;
}

public static partial class _window_win32Library
{
    internal static long _WA_INACTIVE = 0L;
}

public static partial class _window_win32Library
{
    internal static long _SW_RESTORE = 9L;
}

public static partial class _window_win32Library
{
    internal static long _SW_MAXIMIZE = 3L;
}

public static partial class _window_win32Library
{
    internal static long _SW_MINIMIZE = 6L;
}

public static partial class _window_win32Library
{
    internal static string _kWindowingDisabledErrorMessage = "Windowing APIs are not enabled.\n\nWindowing APIs are currently experimental. Do not use windowing APIs in\nproduction applications or plugins published to pub.dev.\n\nTo try experimental windowing APIs:\n1. Switch to Flutter's main release channel.\n2. Turn on the windowing feature flag.\n\nSee: https://github.com/flutter/flutter/issues/30701.\n";
}

internal interface _WindowsMessageHandler___window_win32
{
    public long? handleWindowsMessage(FlutterView view, Pointer<Void> windowHandle, long message, long wParam, long lParam);
}

public class WindowingOwnerWin32Io : WindowingOwnerIo
{
    internal virtual List<_WindowsMessageHandler___window_win32> _messageHandlers { get; private set; } = new List<_WindowsMessageHandler___window_win32>();
    public virtual Allocator allocator { get; private set; } = default!;

    public WindowingOwnerWin32Io()
    {
        this.allocator = new _CallocAllocator___window_win32();
    }

    public virtual WindowControllerIo createWindowController(WindowControllerDelegateIo @delegate = default!, Size? size = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, bool resizable = default!, string? title = null)
    {
        bool __resizable = DartRuntimePrimitives.ConvertValue<bool>(constraints);
        var __constraints = size is null ? null : (global::Doroti.Generated.Framework.Rendering.BoxConstraints)(object)size;
        var __title = resizable is null ? null : (string)(object)resizable;
        var __delegate = (WindowControllerDelegateIo)(object)title;
        return ((WindowControllerIo)(object?)new WindowControllerWin32Io(owner: this, @delegate: __delegate, size: title, constraints: __constraints, title: __title, resizable: __resizable));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DialogWindowControllerIo createDialogWindowController(DialogWindowControllerDelegateIo @delegate, Size? size = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, bool resizable = default!, BaseWindowControllerIo? parent = null, string? title = null)
    {
        return ((DialogWindowControllerIo)(object?)new DialogWindowControllerWin32Io(owner: this, @delegate: @delegate, size: size, constraints: constraints, title: title, parent: parent, resizable: resizable));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TooltipWindowControllerIo createTooltipWindowController(TooltipWindowControllerDelegateIo @delegate, global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints, Rect anchorRect, WindowPositionerIo positioner, BaseWindowControllerIo parent)
    {
        return ((TooltipWindowControllerIo)(object?)new TooltipWindowControllerWin32Io(owner: this, @delegate: @delegate, contentSizeConstraints: constraints, anchorRect: anchorRect, positioner: positioner, parent: parent));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual PopupWindowControllerIo createPopupWindowController(PopupWindowControllerDelegateIo @delegate, global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints, Rect anchorRect, WindowPositionerIo positioner, BaseWindowControllerIo parent)
    {
        return ((PopupWindowControllerIo)(object?)new PopupWindowControllerWin32Io(owner: this, @delegate: @delegate, contentSizeConstraints: constraints, anchorRect: anchorRect, positioner: positioner, parent: parent));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SatelliteWindowControllerIo createSatelliteWindowController(SatelliteWindowControllerDelegateIo @delegate, BaseWindowControllerIo parent, WindowPositionerIo initialPositioner, Rect? initialAnchorRect = null, Size? size = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, bool resizable = default!, string? title = null)
    {
        throw new NotImplementedException("Satellite windows are not yet implemented on Windows.");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _addMessageHandler(_WindowsMessageHandler___window_win32 handler)
    {
        if (this._messageHandlers.Contains(handler))
        {
            return;
        }
        this._messageHandlers.Add(handler);
    }

    internal virtual void _removeMessageHandler(_WindowsMessageHandler___window_win32 handler)
    {
        this._messageHandlers.Remove(handler);
    }

    internal virtual void _onMessage(Pointer<_WindowsMessage___window_win32> message)
    {
        global::Doroti.Flutter.Ui.FlutterView flutterView__7409 = ((global::Doroti.Flutter.Ui.FlutterView)(object?)WidgetsBinding.instance.platformDispatcher.views.firstWhere(((view) => (checked((long)view.viewId) == checked((long)message.@ref.viewId)))));
        long handlesLength__7569 = checked((long)(this._messageHandlers.Count));
        foreach (_WindowsMessageHandler___window_win32 handler__7648 in this._messageHandlers)
        {
            DartRuntimePrimitives.Assert(() => (checked((long)(this._messageHandlers.Count)) == handlesLength__7569), () => (object?)$"Message handler list changed while processing message: {message}");
            long? result__7844 = handler__7648.handleWindowsMessage(flutterView__7409, message.@ref.windowHandle, message.@ref.message, message.@ref.wParam, message.@ref.lParam);
            if ((result__7844 is not null))
            {
                long result__7844__value8042 = DartRuntimePrimitives.RequireValue(result__7844);
                message.@ref.handled = true;
                message.@ref.lResult = DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(result__7844__value8042));
                return;
            }
        }
    }

}

internal class _WindowMessageHandler___window_win32 : _WindowsMessageHandler___window_win32
{
    public virtual WindowControllerWin32Io controller { get; private set; } = default!;

    internal _WindowMessageHandler___window_win32(WindowControllerWin32Io controller)
    {
        this.controller = controller;
    }

    public virtual long? handleWindowsMessage(FlutterView view, Pointer<Void> windowHandle, long message, long wParam, long lParam)
    {
        return this.controller._handleWindowsMessage(view, windowHandle, message, wParam, lParam);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public interface BaseWindowControllerWin32Io
{
    public Pointer<Void> windowHandle { get; }
}

public class WindowControllerWin32Io : WindowControllerIo, BaseWindowControllerWin32Io
{
    internal virtual WindowingOwnerWin32Io _owner { get; private set; } = default!;
    internal virtual WindowControllerDelegateIo _delegate { get; private set; } = default!;
    internal virtual _WindowMessageHandler___window_win32 _handler { get; private set; } = default!;
    internal virtual bool _destroyed { get; set; } = false;

    public WindowControllerWin32Io(WindowingOwnerWin32Io owner, WindowControllerDelegateIo @delegate, Size? size = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, string? title = null, bool resizable = default!)
    {
        this._owner = owner;
        this._delegate = @delegate;
    }

    public override bool isDestroyed => this._destroyed;
    public override Size contentSize
    {
        get
        {
            _ensureNotDestroyed();
            _ActualContentSize___window_win32 size__11194 = ((_ActualContentSize___window_win32)(object?)_Win32PlatformInterface___window_win32.getWindowContentSize(this.windowHandle));
            var result__11271 = new global::Doroti.Flutter.Ui.Size(((_ActualContentSize___window_win32)size__11194).width, ((_ActualContentSize___window_win32)size__11194).height);
            return result__11271;
            return default!;
        }
    }
    public override string title
    {
        get
        {
            _ensureNotDestroyed();
            return ((string)(object?)_Win32PlatformInterface___window_win32.getWindowTitle(((WindowingOwnerWin32Io)this._owner).allocator, this.windowHandle));
            return default!;
        }
    }
    public override bool isActivated
    {
        get
        {
            _ensureNotDestroyed();
            return (object.Equals(_Win32PlatformInterface___window_win32.getForegroundWindow(), this.windowHandle));
            return default!;
        }
    }
    public override bool isMaximized
    {
        get
        {
            _ensureNotDestroyed();
            return (_Win32PlatformInterface___window_win32.isZoomed(this.windowHandle) != 0L);
            return default!;
        }
    }
    public override bool isMinimized
    {
        get
        {
            _ensureNotDestroyed();
            return (_Win32PlatformInterface___window_win32.isIconic(this.windowHandle) != 0L);
            return default!;
        }
    }
    public override bool isFullscreen
    {
        get
        {
            _ensureNotDestroyed();
            return _Win32PlatformInterface___window_win32.getFullscreen(this.windowHandle);
            return default!;
        }
    }
    public override void setSize(Size size)
    {
        _ensureNotDestroyed();
        _Win32PlatformInterface___window_win32.setWindowContentSize(((WindowingOwnerWin32Io)this._owner).allocator, this.windowHandle, size);
    }

    public override void setConstraints(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints)
    {
        _ensureNotDestroyed();
        _Win32PlatformInterface___window_win32.setWindowConstraints(((WindowingOwnerWin32Io)this._owner).allocator, this.windowHandle, constraints);
        notifyListeners();
    }

    public override void setTitle(string title)
    {
        _ensureNotDestroyed();
        _Win32PlatformInterface___window_win32.setWindowTitle(((WindowingOwnerWin32Io)this._owner).allocator, this.windowHandle, title);
        notifyListeners();
    }

    public override void activate()
    {
        _ensureNotDestroyed();
        _Win32PlatformInterface___window_win32.showWindow(this.windowHandle, _window_win32Library._SW_RESTORE);
    }

    public override void setMaximized(bool maximized)
    {
        _ensureNotDestroyed();
        if (maximized)
        {
            _Win32PlatformInterface___window_win32.showWindow(this.windowHandle, _window_win32Library._SW_MAXIMIZE);
        }
        else
        {
            _Win32PlatformInterface___window_win32.showWindow(this.windowHandle, _window_win32Library._SW_RESTORE);
        }
    }

    public override void setMinimized(bool minimized)
    {
        _ensureNotDestroyed();
        if (minimized)
        {
            _Win32PlatformInterface___window_win32.showWindow(this.windowHandle, _window_win32Library._SW_MINIMIZE);
        }
        else
        {
            _Win32PlatformInterface___window_win32.showWindow(this.windowHandle, _window_win32Library._SW_RESTORE);
        }
    }

    public override void setFullscreen(bool fullscreen, Display? display = null)
    {
        _Win32PlatformInterface___window_win32.setFullscreen(((WindowingOwnerWin32Io)this._owner).allocator, this.windowHandle, fullscreen, display: display);
    }

    public virtual Pointer<Void> windowHandle
    {
        get
        {
            _ensureNotDestroyed();
            return ((Pointer<Void>)(object?)_Win32PlatformInterface___window_win32.getWindowHandle(DartRuntimePrimitives.RequireValue(WidgetsBinding.instance.platformDispatcher.engineId), checked((long)this.rootView.viewId)));
            return default!;
        }
    }
    internal virtual void _ensureNotDestroyed()
    {
        if (this._destroyed)
        {
            throw new InvalidOperationException("Window has been destroyed.");
        }
    }

    public override void destroy()
    {
        if (this._destroyed)
        {
            return;
        }
        _Win32PlatformInterface___window_win32.destroyWindow(this.windowHandle);
        if (!this._destroyed)
        {
            _destroyed = true;
            notifyListeners();
        }
    }

    internal virtual long? _handleWindowsMessage(FlutterView view, Pointer<Void> windowHandle, long message, long wParam, long lParam)
    {
        if ((checked((long)view.viewId) != checked((long)this.rootView.viewId)))
        {
            return null;
        }
        if ((message == _window_win32Library._WM_CLOSE))
        {
            this._delegate.onWindowCloseRequested(this);
            return 0L;
        }
        else
        {
            if ((message == _window_win32Library._WM_DESTROY))
            {
                bool wasAlreadyDestroyed__14619 = this._destroyed;
                _destroyed = true;
                if (!wasAlreadyDestroyed__14619)
                {
                    notifyListeners();
                }
                this._owner._removeMessageHandler(this._handler);
                this._delegate.onWindowDestroyed();
                return 0L;
            }
            else
            {
                if (((message == _window_win32Library._WM_SIZE) || (message == _window_win32Library._WM_ACTIVATE)))
                {
                    notifyListeners();
                }
            }
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DialogWindowMesageHandler___window_win32 : _WindowsMessageHandler___window_win32
{
    public virtual DialogWindowControllerWin32Io controller { get; private set; } = default!;

    internal _DialogWindowMesageHandler___window_win32(DialogWindowControllerWin32Io controller)
    {
        this.controller = controller;
    }

    public virtual long? handleWindowsMessage(FlutterView view, Pointer<Void> windowHandle, long message, long wParam, long lParam)
    {
        return this.controller._handleWindowsMessage(view, windowHandle, message, wParam, lParam);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DialogWindowControllerWin32Io : DialogWindowControllerIo, BaseWindowControllerWin32Io
{
    internal virtual WindowingOwnerWin32Io _owner { get; private set; } = default!;
    internal virtual DialogWindowControllerDelegateIo _delegate { get; private set; } = default!;
    internal virtual BaseWindowControllerIo? _parent { get; private set; }
    internal virtual _DialogWindowMesageHandler___window_win32 _handler { get; private set; } = default!;
    internal virtual bool _destroyed { get; set; } = false;

    public DialogWindowControllerWin32Io(WindowingOwnerWin32Io owner, DialogWindowControllerDelegateIo @delegate, Size? size = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, string? title = null, BaseWindowControllerIo? parent = null, bool resizable = default!)
    {
        this._owner = owner;
        this._delegate = @delegate;
        this._parent = parent;
    }

    public override bool isDestroyed => this._destroyed;
    public override Size contentSize
    {
        get
        {
            _ensureNotDestroyed();
            _ActualContentSize___window_win32 size__17765 = ((_ActualContentSize___window_win32)(object?)_Win32PlatformInterface___window_win32.getWindowContentSize(this.windowHandle));
            var result__17842 = new global::Doroti.Flutter.Ui.Size(((_ActualContentSize___window_win32)size__17765).width, ((_ActualContentSize___window_win32)size__17765).height);
            return result__17842;
            return default!;
        }
    }
    public override string title
    {
        get
        {
            _ensureNotDestroyed();
            return ((string)(object?)_Win32PlatformInterface___window_win32.getWindowTitle(((WindowingOwnerWin32Io)this._owner).allocator, this.windowHandle));
            return default!;
        }
    }
    public override bool isActivated
    {
        get
        {
            _ensureNotDestroyed();
            return (object.Equals(_Win32PlatformInterface___window_win32.getForegroundWindow(), this.windowHandle));
            return default!;
        }
    }
    public override bool isMinimized
    {
        get
        {
            _ensureNotDestroyed();
            return (_Win32PlatformInterface___window_win32.isIconic(this.windowHandle) != 0L);
            return default!;
        }
    }
    public override void setSize(Size size)
    {
        _ensureNotDestroyed();
        _Win32PlatformInterface___window_win32.setWindowContentSize(((WindowingOwnerWin32Io)this._owner).allocator, this.windowHandle, size);
    }

    public override void setConstraints(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints)
    {
        _ensureNotDestroyed();
        _Win32PlatformInterface___window_win32.setWindowConstraints(((WindowingOwnerWin32Io)this._owner).allocator, this.windowHandle, constraints);
        notifyListeners();
    }

    public override void setTitle(string title)
    {
        _ensureNotDestroyed();
        _Win32PlatformInterface___window_win32.setWindowTitle(((WindowingOwnerWin32Io)this._owner).allocator, this.windowHandle, title);
        notifyListeners();
    }

    public override void activate()
    {
        _ensureNotDestroyed();
        _Win32PlatformInterface___window_win32.showWindow(this.windowHandle, _window_win32Library._SW_RESTORE);
    }

    public override void setMinimized(bool minimized)
    {
        if ((this.parent is not null))
        {
            return;
        }
        _ensureNotDestroyed();
        if (minimized)
        {
            _Win32PlatformInterface___window_win32.showWindow(this.windowHandle, _window_win32Library._SW_MINIMIZE);
        }
        else
        {
            _Win32PlatformInterface___window_win32.showWindow(this.windowHandle, _window_win32Library._SW_RESTORE);
        }
    }

    public override BaseWindowControllerIo? parent => this._parent;
    public virtual Pointer<Void> windowHandle
    {
        get
        {
            _ensureNotDestroyed();
            return ((Pointer<Void>)(object?)_Win32PlatformInterface___window_win32.getWindowHandle(DartRuntimePrimitives.RequireValue(WidgetsBinding.instance.platformDispatcher.engineId), checked((long)this.rootView.viewId)));
            return default!;
        }
    }
    internal virtual void _ensureNotDestroyed()
    {
        if (this._destroyed)
        {
            throw new InvalidOperationException("Window has been destroyed.");
        }
    }

    public override void destroy()
    {
        if (this._destroyed)
        {
            return;
        }
        _Win32PlatformInterface___window_win32.destroyWindow(this.windowHandle);
    }

    internal virtual long? _handleWindowsMessage(FlutterView view, Pointer<Void> windowHandle, long message, long wParam, long lParam)
    {
        if ((checked((long)view.viewId) != checked((long)this.rootView.viewId)))
        {
            return null;
        }
        if ((message == _window_win32Library._WM_CLOSE))
        {
            this._delegate.onWindowCloseRequested(this);
            return 0L;
        }
        else
        {
            if ((message == _window_win32Library._WM_DESTROY))
            {
                bool wasAlreadyDestroyed__20608 = this._destroyed;
                _destroyed = true;
                if (!wasAlreadyDestroyed__20608)
                {
                    notifyListeners();
                }
                this._owner._removeMessageHandler(this._handler);
                this._delegate.onWindowDestroyed();
                return 0L;
            }
            else
            {
                if (((message == _window_win32Library._WM_SIZE) || (message == _window_win32Library._WM_ACTIVATE)))
                {
                    notifyListeners();
                }
            }
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal delegate Pointer<_Rect___window_win32> _GetWindowPositionNative___window_win32(Pointer<_Size___window_win32> childSize, Pointer<_Rect___window_win32> parentRect, Pointer<_Rect___window_win32> outputRect);

public class TooltipWindowControllerWin32Io : TooltipWindowControllerIo, BaseWindowControllerWin32Io, _WindowsMessageHandler___window_win32
{
    internal virtual WindowingOwnerWin32Io _owner { get; private set; } = default!;
    internal virtual TooltipWindowControllerDelegateIo _delegate { get; private set; } = default!;
    internal virtual BaseWindowControllerIo _parent { get; private set; } = default!;
    internal virtual WindowPositionerIo _positioner { get; set; } = default!;
    internal virtual Rect _anchorRect { get; set; } = default!;
    internal virtual bool _destroyed { get; set; } = false;
    internal virtual NativeCallable<global::System.Func<Pointer<_Size___window_win32>, Pointer<_Rect___window_win32>, Pointer<_Rect___window_win32>, Pointer<_Rect___window_win32>>> _onGetWindowPosition { get; private set; } = default!;

    public TooltipWindowControllerWin32Io(WindowingOwnerWin32Io owner, TooltipWindowControllerDelegateIo @delegate, global::Doroti.Generated.Framework.Rendering.BoxConstraints contentSizeConstraints, BaseWindowControllerIo parent, Rect anchorRect, WindowPositionerIo positioner)
    {
        this._delegate = @delegate;
        this._owner = owner;
        this._parent = parent;
        this._anchorRect = DartRuntimePrimitives.RequireValue(anchorRect);
        this._positioner = positioner;
    }

    public override bool isDestroyed => this._destroyed;
    internal virtual Pointer<_Rect___window_win32> _handleGetWindowPosition(Pointer<_Size___window_win32> childSize, Pointer<_Rect___window_win32> parentRect, Pointer<_Rect___window_win32> outputRect)
    {
        Pointer<_Rect___window_win32> result__23568 = this._owner.allocator();
        double scale__23621 = PlatformDispatcher.instance.views.firstWhere(((view) => (checked((long)view.viewId) == checked((long)this.rootView.viewId)))).devicePixelRatio;
        var scaledAnchorRect__23774 = global::Doroti.Flutter.Ui.Rect.fromLTWH((this._anchorRect.left * scale__23621), (this._anchorRect.top * scale__23621), (this._anchorRect.width * scale__23621), (this._anchorRect.height * scale__23621));
        global::Doroti.Flutter.Ui.Offset scaledOffset__23962 = ((global::Doroti.Flutter.Ui.Offset)(object?)(((WindowPositionerIo)this._positioner).offset * scale__23621));
        WindowPositionerIo scaledPositioner__24032 = ((WindowPositionerIo)(object?)this._positioner.copyWith(offset: scaledOffset__23962));
        global::Doroti.Flutter.Ui.Rect targetRect__24110 = ((global::Doroti.Flutter.Ui.Rect)(object?)scaledPositioner__24032.placeWindow(childSize: childSize.@ref.toSize(), anchorRect: scaledAnchorRect__23774.translate(parentRect.@ref.left.toDouble(), parentRect.@ref.top.toDouble()), parentRect: parentRect.@ref.toRect(), displayRect: outputRect.@ref.toRect()));
        result__23568.@ref.left = targetRect__24110.left.toInt();
        result__23568.@ref.top = targetRect__24110.top.toInt();
        result__23568.@ref.width = targetRect__24110.width.toInt();
        result__23568.@ref.height = targetRect__24110.height.toInt();
        return result__23568;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Pointer<Void> windowHandle
    {
        get
        {
            _ensureNotDestroyed();
            return ((Pointer<Void>)(object?)_Win32PlatformInterface___window_win32.getWindowHandle(DartRuntimePrimitives.RequireValue(PlatformDispatcher.instance.engineId), checked((long)this.rootView.viewId)));
            return default!;
        }
    }
    public override Size contentSize
    {
        get
        {
            _ensureNotDestroyed();
            _ActualContentSize___window_win32 size__24980 = ((_ActualContentSize___window_win32)(object?)_Win32PlatformInterface___window_win32.getWindowContentSize(this.windowHandle));
            return new global::Doroti.Flutter.Ui.Size(((_ActualContentSize___window_win32)size__24980).width, ((_ActualContentSize___window_win32)size__24980).height);
            return default!;
        }
    }
    internal virtual void _ensureNotDestroyed()
    {
        if (this._destroyed)
        {
            throw new InvalidOperationException("Window has been destroyed.");
        }
    }

    public override void destroy()
    {
        if (this._destroyed)
        {
            return;
        }
        _Win32PlatformInterface___window_win32.destroyWindow(this.windowHandle);
        if (!this._destroyed)
        {
            _destroyed = true;
            notifyListeners();
        }
    }

    public override void updatePosition(Rect? anchorRect = null, WindowPositionerIo? positioner = null)
    {
        if ((anchorRect is not null))
        {
            Rect anchorRect__value25520 = DartRuntimePrimitives.RequireValue(anchorRect);
            _anchorRect = DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(anchorRect__value25520));
        }
        if ((positioner is not null))
        {
            _positioner = positioner;
        }
        _Win32PlatformInterface___window_win32.updateTooltipWindowPosition(this.windowHandle);
    }

    public virtual long? handleWindowsMessage(FlutterView view, Pointer<Void> windowHandle, long message, long wParam, long lParam)
    {
        if ((checked((long)view.viewId) == checked((long)((BaseWindowControllerIo)this.parent).rootView.viewId)))
        {
            if ((message == _window_win32Library._WM_SIZE))
            {
                DartAsyncRuntime.scheduleMicrotask(this.destroy);
            }
            else
            {
                if (((message == _window_win32Library._WM_ACTIVATE) && (wParam == _window_win32Library._WA_INACTIVE)))
                {
                    DartAsyncRuntime.scheduleMicrotask(this.destroy);
                }
            }
            return null;
        }
        if ((checked((long)view.viewId) != checked((long)this.rootView.viewId)))
        {
            return null;
        }
        if (((message == _window_win32Library._WM_SIZE) || (message == _window_win32Library._WM_ACTIVATE)))
        {
            notifyListeners();
        }
        else
        {
            if ((message == _window_win32Library._WM_DESTROY))
            {
                bool wasAlreadyDestroyed__26789 = this._destroyed;
                _destroyed = true;
                if (!wasAlreadyDestroyed__26789)
                {
                    notifyListeners();
                }
                this._onGetWindowPosition.close();
                this._owner._removeMessageHandler(this);
                this._delegate.onWindowDestroyed();
                return 0L;
            }
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BaseWindowControllerIo parent => this._parent;
    public override void setConstraints(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints)
    {
    }

}

public class PopupWindowControllerWin32Io : PopupWindowControllerIo, _WindowsMessageHandler___window_win32
{
    internal virtual WindowingOwnerWin32Io _owner { get; private set; } = default!;
    internal virtual PopupWindowControllerDelegateIo _delegate { get; private set; } = default!;
    internal virtual BaseWindowControllerIo _parent { get; private set; } = default!;
    internal virtual WindowPositionerIo _positioner { get; set; } = default!;
    internal virtual Rect _anchorRect { get; set; } = default!;
    internal virtual bool _destroyed { get; set; } = false;
    internal virtual NativeCallable<global::System.Func<Pointer<_Size___window_win32>, Pointer<_Rect___window_win32>, Pointer<_Rect___window_win32>, Pointer<_Rect___window_win32>>> _onGetWindowPosition { get; private set; } = default!;

    public PopupWindowControllerWin32Io(WindowingOwnerWin32Io owner, PopupWindowControllerDelegateIo @delegate, global::Doroti.Generated.Framework.Rendering.BoxConstraints contentSizeConstraints, BaseWindowControllerIo parent, Rect anchorRect, WindowPositionerIo positioner)
    {
        this._delegate = @delegate;
        this._owner = owner;
        this._parent = parent;
        this._anchorRect = DartRuntimePrimitives.RequireValue(anchorRect);
        this._positioner = positioner;
    }

    public override bool isDestroyed => this._destroyed;
    internal virtual Pointer<_Rect___window_win32> _handleGetWindowPosition(Pointer<_Size___window_win32> childSize, Pointer<_Rect___window_win32> parentRect, Pointer<_Rect___window_win32> outputRect)
    {
        double scale__29553 = PlatformDispatcher.instance.views.firstWhere(((view) => (checked((long)view.viewId) == checked((long)this.rootView.viewId)))).devicePixelRatio;
        var scaledAnchorRect__29706 = global::Doroti.Flutter.Ui.Rect.fromLTWH((this._anchorRect.left * scale__29553), (this._anchorRect.top * scale__29553), (this._anchorRect.width * scale__29553), (this._anchorRect.height * scale__29553));
        global::Doroti.Flutter.Ui.Offset scaledOffset__29894 = ((global::Doroti.Flutter.Ui.Offset)(object?)(((WindowPositionerIo)this._positioner).offset * scale__29553));
        WindowPositionerIo scaledPositioner__29964 = ((WindowPositionerIo)(object?)this._positioner.copyWith(offset: scaledOffset__29894));
        global::Doroti.Flutter.Ui.Rect targetRect__30042 = ((global::Doroti.Flutter.Ui.Rect)(object?)scaledPositioner__29964.placeWindow(childSize: childSize.@ref.toSize(), anchorRect: scaledAnchorRect__29706.translate(parentRect.@ref.left.toDouble(), parentRect.@ref.top.toDouble()), parentRect: parentRect.@ref.toRect(), displayRect: outputRect.@ref.toRect()));
        Pointer<_Rect___window_win32> result__30383 = this._owner.allocator();
        result__30383.@ref.left = targetRect__30042.left.toInt();
        result__30383.@ref.top = targetRect__30042.top.toInt();
        result__30383.@ref.width = targetRect__30042.width.toInt();
        result__30383.@ref.height = targetRect__30042.height.toInt();
        return result__30383;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Pointer<Void> getWindowHandle()
    {
        _ensureNotDestroyed();
        return ((Pointer<Void>)(object?)_Win32PlatformInterface___window_win32.getWindowHandle(DartRuntimePrimitives.RequireValue(PlatformDispatcher.instance.engineId), checked((long)this.rootView.viewId)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size contentSize
    {
        get
        {
            _ensureNotDestroyed();
            _ActualContentSize___window_win32 size__30978 = ((_ActualContentSize___window_win32)(object?)_Win32PlatformInterface___window_win32.getWindowContentSize(getWindowHandle()));
            return new global::Doroti.Flutter.Ui.Size(((_ActualContentSize___window_win32)size__30978).width, ((_ActualContentSize___window_win32)size__30978).height);
            return default!;
        }
    }
    internal virtual void _ensureNotDestroyed()
    {
        if (this._destroyed)
        {
            throw new InvalidOperationException("Window has been destroyed.");
        }
    }

    public override void destroy()
    {
        if (this._destroyed)
        {
            return;
        }
        _Win32PlatformInterface___window_win32.destroyWindow(getWindowHandle());
        if (!this._destroyed)
        {
            _destroyed = true;
            notifyListeners();
        }
    }

    public override void updatePosition(Rect? anchorRect = null, WindowPositionerIo? positioner = null)
    {
        _ensureNotDestroyed();
        if ((anchorRect is not null))
        {
            Rect anchorRect__value31555 = DartRuntimePrimitives.RequireValue(anchorRect);
            _anchorRect = DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(anchorRect__value31555));
        }
        if ((positioner is not null))
        {
            _positioner = positioner;
        }
        _Win32PlatformInterface___window_win32.updatePopupWindowPosition(getWindowHandle());
    }

    public override Offset offsetFromParent
    {
        get
        {
            _ensureNotDestroyed();
            Pointer<Void> popupHandle__31848 = ((Pointer<Void>)(object?)getWindowHandle());
            Pointer<Void> parentHandle__31896 = ((Pointer<Void>)(object?)_Win32PlatformInterface___window_win32.getWindowHandle(DartRuntimePrimitives.RequireValue(PlatformDispatcher.instance.engineId), checked((long)((BaseWindowControllerIo)this.parent).rootView.viewId)));
            global::Doroti.Flutter.Ui.Offset physicalOffset__32052 = ((global::Doroti.Flutter.Ui.Offset)(object?)_Win32PlatformInterface___window_win32.getWindowOffsetFromParent(((WindowingOwnerWin32Io)this._owner).allocator, popupHandle__31848, parentHandle__31896));
            double scale__32263 = PlatformDispatcher.instance.views.firstWhere(((view) => (checked((long)view.viewId) == checked((long)this.rootView.viewId)))).devicePixelRatio;
            return (physicalOffset__32052 / scale__32263);
            return default!;
        }
    }
    public virtual long? handleWindowsMessage(FlutterView view, Pointer<Void> windowHandle, long message, long wParam, long lParam)
    {
        if ((message == _window_win32Library._WM_DESTROY))
        {
            bool wasAlreadyDestroyed__32866 = this._destroyed;
            _destroyed = true;
            if (!wasAlreadyDestroyed__32866)
            {
                notifyListeners();
            }
            this._onGetWindowPosition.close();
            this._owner._removeMessageHandler(this);
            this._delegate.onWindowDestroyed();
            return 0L;
        }
        if (this._destroyed)
        {
            return null;
        }
        if ((checked((long)view.viewId) == checked((long)((BaseWindowControllerIo)this.parent).rootView.viewId)))
        {
            if ((message == _window_win32Library._WM_SIZE))
            {
                DartAsyncRuntime.scheduleMicrotask(this.destroy);
                return null;
            }
        }
        if ((message == _window_win32Library._WM_ACTIVATE))
        {
            Pointer<Void> parentHwnd__33898 = ((Pointer<Void>)(object?)_Win32PlatformInterface___window_win32.getWindowHandle(DartRuntimePrimitives.RequireValue(PlatformDispatcher.instance.engineId), checked((long)((BaseWindowControllerIo)this.parent).rootView.viewId)));
            Pointer<Void> hFocused__34057 = ((Pointer<Void>)(object?)_Win32PlatformInterface___window_win32.getForegroundWindow());
            if ((((!object.Equals(hFocused__34057, parentHwnd__33898)) && (!object.Equals(hFocused__34057, getWindowHandle()))) && !_Win32PlatformInterface___window_win32.isChild(getWindowHandle(), hFocused__34057)))
            {
                DartAsyncRuntime.scheduleMicrotask(this.destroy);
            }
            return null;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BaseWindowControllerIo parent => this._parent;
    public override void setConstraints(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints)
    {
    }

    public override void activate()
    {
        _ensureNotDestroyed();
        _Win32PlatformInterface___window_win32.showWindow(getWindowHandle(), _window_win32Library._SW_RESTORE);
    }

    public override bool isActivated
    {
        get
        {
            _ensureNotDestroyed();
            return (object.Equals(_Win32PlatformInterface___window_win32.getForegroundWindow(), getWindowHandle()));
            return default!;
        }
    }
}

internal class _Size___window_win32 : Struct
{
    public virtual long width { get; set; } = default!;
    public virtual long height { get; set; } = default!;

    public override string ToString()
    {
        return $"Size(width: {this.width}, height: {this.height})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Flutter.Ui.Size toSize()
    {
        return new global::Doroti.Flutter.Ui.Size(this.width.toDouble(), this.height.toDouble());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _Win32Rect___window_win32 : Struct
{
    public virtual long left { get; set; } = default!;
    public virtual long top { get; set; } = default!;
    public virtual long right { get; set; } = default!;
    public virtual long bottom { get; set; } = default!;

}

internal class _Win32Point___window_win32 : Struct
{
    public virtual long x { get; set; } = default!;
    public virtual long y { get; set; } = default!;

}

internal class _Rect___window_win32 : Struct
{
    public virtual long left { get; set; } = default!;
    public virtual long top { get; set; } = default!;
    public virtual long width { get; set; } = default!;
    public virtual long height { get; set; } = default!;

    public virtual global::Doroti.Flutter.Ui.Rect toRect()
    {
        return global::Doroti.Flutter.Ui.Rect.fromLTWH(this.left.toDouble(), this.top.toDouble(), this.width.toDouble(), this.height.toDouble());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"Rect(left: {this.left}, top: {this.top}, width: {this.width}, height: {this.height})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _Win32PlatformInterface___window_win32
{
    public static void initializeWindowing(Allocator allocator, long engineId, global::System.Action<Pointer<_WindowsMessage___window_win32>> onMessage)
    {
        Pointer<_WindowingInitRequest___window_win32> request__36244 = allocator();
        try
        {
            request__36244.@ref.onMessage = new NativeCallable<global::System.Func<Pointer<_WindowsMessage___window_win32>, Void>>(onMessage).nativeFunction;
            _Win32PlatformInterface___window_win32._initializeWindowing(engineId, request__36244);
        }
        finally
        {
            allocator.free(request__36244);
        }
    }

    internal abstract static void _initializeWindowing(long engineId, Pointer<_WindowingInitRequest___window_win32> request);
    public static long createWindow(Allocator allocator, long engineId, Size? size, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints, string? title, bool shrinkWrap, bool resizable)
    {
        Pointer<_WindowCreationRequest___window_win32> request__37078 = allocator();
        try
        {
            request__37078.@ref.size.from(size);
            request__37078.@ref.constraints.from(constraints);
            request__37078.@ref.title = ((title ?? "Window")).toNativeUtf16(allocator: allocator);
            request__37078.@ref.shrinkWrap = shrinkWrap;
            request__37078.@ref.resizable = resizable;
            return _Win32PlatformInterface___window_win32._createWindow(engineId, request__37078);
        }
        finally
        {
            allocator.free(request__37078);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal abstract static long _createWindow(long engineId, Pointer<_WindowCreationRequest___window_win32> request);
    public static long createDialogWindow(Allocator allocator, long engineId, Size? size, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints, string? title, Pointer<Void>? parent, bool shrinkWrap, bool resizable)
    {
        Pointer<_DialogWindowCreationRequest___window_win32> request__38033 = allocator();
        try
        {
            request__38033.@ref.size.from(size);
            request__38033.@ref.constraints.from(constraints);
            request__38033.@ref.title = ((title ?? "Dialog window")).toNativeUtf16(allocator: allocator);
            request__38033.@ref.parentOrNull = (parent ?? new Pointer<Void>(0L));
            request__38033.@ref.shrinkWrap = shrinkWrap;
            request__38033.@ref.resizable = resizable;
            return _Win32PlatformInterface___window_win32._createDialogWindow(engineId, request__38033);
        }
        finally
        {
            allocator.free(request__38033);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal abstract static long _createDialogWindow(long engineId, Pointer<_DialogWindowCreationRequest___window_win32> request);
    public static long createTooltipWindow(Allocator allocator, long engineId, global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints, Pointer<Void> parent, Pointer<NativeFunction<global::System.Func<Pointer<_Size___window_win32>, Pointer<_Rect___window_win32>, Pointer<_Rect___window_win32>, Pointer<_Rect___window_win32>>>> onGetWindowPosition)
    {
        Pointer<_TooltipWindowCreationRequest___window_win32> request__39280 = allocator();
        try
        {
            request__39280.@ref.constraints.from(constraints);
            request__39280.@ref.parent = parent;
            request__39280.@ref.onGetWindowPosition = onGetWindowPosition;
            return _Win32PlatformInterface___window_win32._createTooltipWindow(engineId, request__39280);
        }
        finally
        {
            allocator.free(request__39280);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal abstract static long _createTooltipWindow(long engineId, Pointer<_TooltipWindowCreationRequest___window_win32> request);
    public static long createPopupWindow(Allocator allocator, long engineId, global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints, Pointer<Void> parent, Pointer<NativeFunction<global::System.Func<Pointer<_Size___window_win32>, Pointer<_Rect___window_win32>, Pointer<_Rect___window_win32>, Pointer<_Rect___window_win32>>>> onGetWindowPosition)
    {
        Pointer<_PopupWindowCreationRequest___window_win32> request__40335 = allocator();
        try
        {
            request__40335.@ref.constraints.from(constraints);
            request__40335.@ref.parent = parent;
            request__40335.@ref.onGetWindowPosition = onGetWindowPosition;
            return _Win32PlatformInterface___window_win32._createPopupWindow(engineId, request__40335);
        }
        finally
        {
            allocator.free(request__40335);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal abstract static long _createPopupWindow(long engineId, Pointer<_PopupWindowCreationRequest___window_win32> request);
    public abstract static Pointer<Void> getWindowHandle(long engineId, long viewId);
    public abstract static void destroyWindow(Pointer<Void> windowHandle);
    public abstract static _ActualContentSize___window_win32 getWindowContentSize(Pointer<Void> windowHandle);
    public static void setWindowTitle(Allocator allocator, Pointer<Void> windowHandle, string title)
    {
        Pointer<_Utf16___window_win32> titlePointer__41636 = title.toNativeUtf16(allocator: allocator);
        try
        {
            _Win32PlatformInterface___window_win32._setWindowTitle(windowHandle, titlePointer__41636);
        }
        finally
        {
            allocator.free(titlePointer__41636);
        }
    }

    internal abstract static void _setWindowTitle(Pointer<Void> windowHandle, Pointer<_Utf16___window_win32> title);
    public static void setWindowContentSize(Allocator allocator, Pointer<Void> windowHandle, Size? size)
    {
        Pointer<_WindowSizeRequest___window_win32> request__42126 = allocator();
        try
        {
            request__42126.@ref.from(size);
            _Win32PlatformInterface___window_win32._setWindowContentSize(windowHandle, request__42126);
        }
        finally
        {
            allocator.free(request__42126);
        }
    }

    internal abstract static void _setWindowContentSize(Pointer<Void> windowHandle, Pointer<_WindowSizeRequest___window_win32> size);
    public static void setWindowConstraints(Allocator allocator, Pointer<Void> windowHandle, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints)
    {
        Pointer<_WindowConstraintsRequest___window_win32> request__42755 = allocator();
        try
        {
            request__42755.@ref.from(constraints);
            _Win32PlatformInterface___window_win32._setWindowConstraints(windowHandle, request__42755);
        }
        finally
        {
            allocator.free(request__42755);
        }
    }

    internal abstract static void _setWindowConstraints(Pointer<Void> windowHandle, Pointer<_WindowConstraintsRequest___window_win32> constraints);
    public abstract static void showWindow(Pointer<Void> windowHandle, long command);
    public abstract static long isIconic(Pointer<Void> windowHandle);
    public abstract static long isZoomed(Pointer<Void> windowHandle);
    public static void setFullscreen(Allocator allocator, Pointer<Void> windowHandle, bool fullscreen, Display? display = null)
    {
        Pointer<_WindowFullscreenRequest___window_win32> request__43795 = allocator();
        try
        {
            request__43795.@ref.fullscreen = fullscreen;
            request__43795.@ref.hasDisplayId = (display is not null);
            request__43795.@ref.displayId = (display?.id ?? 0L);
            _Win32PlatformInterface___window_win32._setFullscreen(windowHandle, request__43795);
        }
        finally
        {
            allocator.free(request__43795);
        }
    }

    internal abstract static void _setFullscreen(Pointer<Void> windowHandle, Pointer<_WindowFullscreenRequest___window_win32> request);
    public abstract static bool getFullscreen(Pointer<Void> windowHandle);
    internal abstract static long _getWindowTextLength(Pointer<Void> windowHandle);
    internal abstract static long _getWindowText(Pointer<Void> windowHandle, Pointer<_Utf16___window_win32> lpString, long maxLength);
    public static string getWindowTitle(Allocator allocator, Pointer<Void> windowHandle)
    {
        long length__44970 = _Win32PlatformInterface___window_win32._getWindowTextLength(windowHandle);
        if ((length__44970 == 0L))
        {
            return "";
        }
        Pointer<Uint16> data__45096 = allocator((length__44970 + 1L));
        try
        {
            Pointer<_Utf16___window_win32> buffer__45180 = data__45096.cast<_Utf16___window_win32>();
            _Win32PlatformInterface___window_win32._getWindowText(windowHandle, buffer__45180, (length__44970 + 1L));
            return buffer__45180.toDartString();
        }
        finally
        {
            allocator.free(data__45096);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract static Pointer<Void> getForegroundWindow();
    public abstract static void updateTooltipWindowPosition(Pointer<Void> windowHandle);
    public abstract static void updatePopupWindowPosition(Pointer<Void> windowHandle);
    public abstract static bool isChild(Pointer<Void> parent, Pointer<Void> child);
    public abstract static bool getWindowRect(Pointer<Void> windowHandle, Pointer<_Win32Rect___window_win32> rect);
    public abstract static bool clientToScreen(Pointer<Void> windowHandle, Pointer<_Win32Point___window_win32> point);
    public static global::Doroti.Flutter.Ui.Offset getWindowOffsetFromParent(Allocator allocator, Pointer<Void> windowHandle, Pointer<Void> parentHandle)
    {
        Pointer<_Win32Rect___window_win32> windowRect__46590 = allocator();
        Pointer<_Win32Point___window_win32> parentOrigin__46663 = allocator();
        try
        {
            _Win32PlatformInterface___window_win32.getWindowRect(windowHandle, windowRect__46590);
            parentOrigin__46663.@ref.x = 0L;
            parentOrigin__46663.@ref.y = 0L;
            _Win32PlatformInterface___window_win32.clientToScreen(parentHandle, parentOrigin__46663);
            return new global::Doroti.Flutter.Ui.Offset(((windowRect__46590.@ref.left - parentOrigin__46663.@ref.x)).toDouble(), ((windowRect__46590.@ref.top - parentOrigin__46663.@ref.y)).toDouble());
        }
        finally
        {
            allocator.free(windowRect__46590);
            allocator.free(parentOrigin__46663);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _WindowCreationRequest___window_win32 : Struct
{
    public virtual _WindowSizeRequest___window_win32 size { get; set; } = default!;
    public virtual _WindowConstraintsRequest___window_win32 constraints { get; set; } = default!;
    public virtual Pointer<_Utf16___window_win32> title { get; set; } = default!;
    public virtual bool shrinkWrap { get; set; } = default!;
    public virtual bool resizable { get; set; } = default!;

}

internal class _DialogWindowCreationRequest___window_win32 : Struct
{
    public virtual _WindowSizeRequest___window_win32 size { get; set; } = default!;
    public virtual _WindowConstraintsRequest___window_win32 constraints { get; set; } = default!;
    public virtual Pointer<_Utf16___window_win32> title { get; set; } = default!;
    public virtual Pointer<Void> parentOrNull { get; set; } = default!;
    public virtual bool shrinkWrap { get; set; } = default!;
    public virtual bool resizable { get; set; } = default!;

}

internal class _TooltipWindowCreationRequest___window_win32 : Struct
{
    public virtual _WindowConstraintsRequest___window_win32 constraints { get; set; } = default!;
    public virtual Pointer<Void> parent { get; set; } = default!;
    public virtual Pointer<NativeFunction<global::System.Func<Pointer<_Size___window_win32>, Pointer<_Rect___window_win32>, Pointer<_Rect___window_win32>, Pointer<_Rect___window_win32>>>> onGetWindowPosition { get; set; } = default!;

}

internal class _PopupWindowCreationRequest___window_win32 : Struct
{
    public virtual _WindowConstraintsRequest___window_win32 constraints { get; set; } = default!;
    public virtual Pointer<Void> parent { get; set; } = default!;
    public virtual Pointer<NativeFunction<global::System.Func<Pointer<_Size___window_win32>, Pointer<_Rect___window_win32>, Pointer<_Rect___window_win32>, Pointer<_Rect___window_win32>>>> onGetWindowPosition { get; set; } = default!;

}

internal class _WindowingInitRequest___window_win32 : Struct
{
    public virtual Pointer<NativeFunction<global::System.Func<Pointer<_WindowsMessage___window_win32>, Void>>> onMessage { get; set; } = default!;

}

internal class _WindowSizeRequest___window_win32 : Struct
{
    public virtual bool hasSize { get; set; } = default!;
    public virtual double width { get; set; } = default!;
    public virtual double height { get; set; } = default!;

    public virtual void from(Size? size)
    {
        hasSize = (size is not null);
        width = (size?.width ?? 0);
        height = (size?.height ?? 0);
    }

}

internal class _WindowConstraintsRequest___window_win32 : Struct
{
    public virtual bool hasConstraints { get; set; } = default!;
    public virtual double minWidth { get; set; } = default!;
    public virtual double minHeight { get; set; } = default!;
    public virtual double maxWidth { get; set; } = default!;
    public virtual double maxHeight { get; set; } = default!;

    public virtual void from(global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints)
    {
        hasConstraints = (constraints is not null);
        minWidth = (constraints?.minWidth ?? 0);
        minHeight = (constraints?.minHeight ?? 0);
        maxWidth = (constraints?.maxWidth ?? double.MaxValue);
        maxHeight = (constraints?.maxHeight ?? double.MaxValue);
    }

}

internal class _WindowsMessage___window_win32 : Struct
{
    public virtual long viewId { get; set; } = default!;
    public virtual Pointer<Void> windowHandle { get; set; } = default!;
    public virtual long message { get; set; } = default!;
    public virtual long wParam { get; set; } = default!;
    public virtual long lParam { get; set; } = default!;
    public virtual long lResult { get; set; } = default!;
    public virtual bool handled { get; set; } = default!;

}

internal class _ActualContentSize___window_win32 : Struct
{
    public virtual double width { get; set; } = default!;
    public virtual double height { get; set; } = default!;

}

internal class _WindowFullscreenRequest___window_win32 : Struct
{
    public virtual bool fullscreen { get; set; } = default!;
    public virtual bool hasDisplayId { get; set; } = default!;
    public virtual long displayId { get; set; } = default!;

}

internal class _Utf16___window_win32 : Opaque
{
}

internal delegate Pointer<NativeType> _WinCoTaskMemAllocNative___window_win32(Size __unused0);

internal delegate Pointer<NativeType> _WinCoTaskMemAlloc___window_win32(long __unused0);

internal delegate Void _WinCoTaskMemFreeNative___window_win32(Pointer<NativeType> __unused0);

internal delegate void _WinCoTaskMemFree___window_win32(Pointer<NativeType> __unused0);

internal class _CallocAllocator___window_win32 : Allocator
{
    internal virtual DynamicLibrary _ole32lib { get; private set; } = default!;
    internal virtual global::System.Func<long, Pointer<NativeType>> _winCoTaskMemAlloc { get; private set; } = default!;
    internal virtual Pointer<NativeFunction<global::System.Func<Pointer<NativeType>, Void>>> _winCoTaskMemFreePointer { get; private set; } = default!;
    internal virtual global::System.Action<Pointer<NativeType>> _winCoTaskMemFree { get; private set; } = default!;

    internal _CallocAllocator___window_win32()
    {
    }

    internal virtual void _fillMemory(Pointer<NativeType> destination, long length, long fill)
    {
        Pointer<Uint8> ptr__54924 = destination.cast<Uint8>();
        for (var i__54974 = 0L; (i__54974 < length); i__54974++)
        {
            ptr__54924[i__54974] = fill;
        }
    }

    internal virtual void _zeroMemory(Pointer<NativeType> destination, long length) => _fillMemory(destination, length, 0L);
    public virtual Pointer<T> allocate<T>(long byteCount, long? alignment = null) where T : NativeType
    {
        Pointer<T> result__55413 = default!;
        result__55413 = this._winCoTaskMemAlloc(byteCount).cast<T>();
        if ((result__55413.address == 0L))
        {
            throw DartRuntimePrimitives.AsException(new DartArgumentError($"Could not allocate {byteCount} bytes."));
        }
        if (Platform.isWindows)
        {
            _zeroMemory(result__55413, byteCount);
        }
        return result__55413;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void free(Pointer<NativeType> pointer)
    {
        this._winCoTaskMemFree(pointer);
    }

    public virtual Pointer<NativeFinalizerFunction> nativeFree => DartRuntimePrimitives.ConvertValue<Pointer<NativeFinalizerFunction>>(this._winCoTaskMemFreePointer);
}

