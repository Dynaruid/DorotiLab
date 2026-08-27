// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/_window_macos.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Widgets;

public static partial class _window_macosLibrary
{
    internal static string _kWindowingDisabledErrorMessage = "Windowing APIs are not enabled.\n\nWindowing APIs are currently experimental. Do not use windowing APIs in\nproduction applications or plugins published to pub.dev.\n\nTo try experimental windowing APIs:\n1. Switch to Flutter's main release channel.\n2. Turn on the windowing feature flag.\n\nSee: https://github.com/flutter/flutter/issues/30701.\n";
}

public class WindowingOwnerMacOSIo : WindowingOwnerIo
{
    internal virtual List<_WindowControllerMixin___window_macos> _activeControllers { get; private set; } = new List<_WindowControllerMixin___window_macos>();

    public WindowingOwnerMacOSIo()
    {
    }

    public virtual WindowControllerIo createWindowController(WindowControllerDelegateIo @delegate, Size? size = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, bool resizable = default!, string? title = null)
    {
        var controller = new WindowControllerMacOSIo(owner: this, @delegate: @delegate, size: size, title: title);
        this._activeControllers.Add(controller);
        return ((WindowControllerIo)(object?)controller);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DialogWindowControllerIo createDialogWindowController(DialogWindowControllerDelegateIo @delegate, Size? size = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, bool resizable = default!, BaseWindowControllerIo? parent = null, string? title = null)
    {
        var controller = new DialogWindowControllerMacOSIo(owner: this, @delegate: @delegate, size: size, parent: parent, title: title);
        this._activeControllers.Add(controller);
        return ((DialogWindowControllerIo)(object?)controller);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TooltipWindowControllerIo createTooltipWindowController(TooltipWindowControllerDelegateIo @delegate, global::Doroti.Framework.Rendering.BoxConstraints constraints, Rect anchorRect, WindowPositionerIo positioner, BaseWindowControllerIo parent)
    {
        var controller = new TooltipWindowControllerMacOSIo(owner: this, @delegate: @delegate, contentSizeConstraints: constraints, anchorRect: anchorRect, positioner: positioner, parent: parent);
        this._activeControllers.Add(controller);
        return ((TooltipWindowControllerIo)(object?)controller);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual PopupWindowControllerIo createPopupWindowController(PopupWindowControllerDelegateIo @delegate, global::Doroti.Framework.Rendering.BoxConstraints constraints, Rect anchorRect, WindowPositionerIo positioner, BaseWindowControllerIo parent)
    {
        var controller = new PopupWindowControllerMacOSIo(owner: this, @delegate: @delegate, contentSizeConstraints: constraints, parent: parent, anchorRect: anchorRect, positioner: positioner);
        this._activeControllers.Add(controller);
        return ((PopupWindowControllerIo)(object?)controller);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Pointer<Void> getWindowHandle(DorotiView view)
    {
        return ((Pointer<Void>)(object?)_MacOSPlatformInterface___window_macos.getWindowHandle(DartRuntimePrimitives.RequireValue(PlatformDispatcher.instance.engineId), checked((long)view.viewId)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SatelliteWindowControllerIo createSatelliteWindowController(SatelliteWindowControllerDelegateIo @delegate, BaseWindowControllerIo parent, WindowPositionerIo initialPositioner, Rect? initialAnchorRect = null, Size? size = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, bool resizable = false, string? title = null)
    {
        throw new NotImplementedException("Satellite windows are not yet implemented on macOS.");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal interface BaseWindowControllerMacOSIo
{
    public Pointer<Void> windowHandle { get; }
    public bool isDestroyed { get; }
}

public interface _WindowControllerMixin___window_macos : BaseWindowControllerMacOSIo
{
    bool _destroyed { get; set; }
    NativeCallable<global::System.Func<Void>> _onShouldClose { get; }
    NativeCallable<global::System.Func<Void>> _onWillClose { get; }
    NativeCallable<global::System.Func<Void>> _onResize { get; }
    NativeCallable<global::System.Func<Pointer<_Size___window_macos>, Pointer<_Rect___window_macos>, Pointer<_Rect___window_macos>, Pointer<_Rect___window_macos>>> _onGetWindowPosition { get; }
    WindowingOwnerMacOSIo _owner { get; }

    public void _initController(WindowingOwnerMacOSIo owner);
    public void _handleOnShouldClose();
    public void _handleOnResize();
    public void _handleOnWillClose();
    public Pointer<_Rect___window_macos> _handleOnGetWindowPosition(Pointer<_Size___window_macos> childSize, Pointer<_Rect___window_macos> parentRect, Pointer<_Rect___window_macos> outputRect);
    public void _ensureNotDestroyed();
    public global::Doroti.Ui.DorotiView rootView { get; }
    public Pointer<Void> windowHandle { get; }
    public global::Doroti.Ui.Size contentSize { get; }
    public void destroy();
    public bool isDestroyed { get; }
}

public class TooltipWindowControllerMacOSIo : TooltipWindowControllerIo, _WindowControllerMixin___window_macos
{
    internal virtual TooltipWindowControllerDelegateIo _delegate { get; private set; } = default!;
    internal virtual BaseWindowControllerIo _parent { get; private set; } = default!;
    internal virtual WindowPositionerIo _positioner { get; set; } = default!;
    internal virtual Rect _anchorRect { get; set; } = default!;
    public virtual bool _destroyed { get; set; } = false;
    public virtual NativeCallable<global::System.Func<Void>> _onShouldClose { get; set; } = default!;
    public virtual NativeCallable<global::System.Func<Void>> _onWillClose { get; set; } = default!;
    public virtual NativeCallable<global::System.Func<Void>> _onResize { get; set; } = default!;
    public virtual NativeCallable<global::System.Func<Pointer<_Size___window_macos>, Pointer<_Rect___window_macos>, Pointer<_Rect___window_macos>, Pointer<_Rect___window_macos>>> _onGetWindowPosition { get; set; } = default!;
    public virtual WindowingOwnerMacOSIo _owner { get; set; } = default!;

    public TooltipWindowControllerMacOSIo(WindowingOwnerMacOSIo owner, TooltipWindowControllerDelegateIo @delegate, global::Doroti.Framework.Rendering.BoxConstraints contentSizeConstraints, BaseWindowControllerIo parent, Rect anchorRect, WindowPositionerIo positioner)
    {
        this._anchorRect = DartRuntimePrimitives.RequireValue(anchorRect);
        this._positioner = positioner;
        this._delegate = @delegate;
        this._parent = parent;
    }

    public override void updatePosition(Rect? anchorRect = null, WindowPositionerIo? positioner = null)
    {
        if ((anchorRect is not null))
        {
            Rect anchorRect__value9934 = DartRuntimePrimitives.RequireValue(anchorRect);
            _anchorRect = DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(anchorRect__value9934));
        }
        if ((positioner is not null))
        {
            _positioner = positioner;
        }
        _MacOSPlatformInterface___window_macos.updateWindowPosition(this.windowHandle);
    }

    public virtual void _handleOnShouldClose()
    {
        destroy();
    }

    public virtual void _handleOnWillClose()
    {
        this._onWillClose.close();
        this._onShouldClose.close();
        this._onResize.close();
        this._onGetWindowPosition.close();
        _destroyed = true;
        ((WindowingOwnerMacOSIo)this._owner)._activeControllers.Remove(this);
        notifyListeners();
        this._delegate.onWindowDestroyed();
    }

    public virtual void _handleOnResize()
    {
        notifyListeners();
    }

    public virtual Pointer<_Rect___window_macos> _handleOnGetWindowPosition(Pointer<_Size___window_macos> childSize, Pointer<_Rect___window_macos> parentRect, Pointer<_Rect___window_macos> outputRect)
    {
        Pointer<_Rect___window_macos> result = _window_macosLibrary._allocator();
        global::Doroti.Ui.Rect targetRect = ((global::Doroti.Ui.Rect)(object?)this._positioner.placeWindow(childSize: childSize.@ref.toSize(), anchorRect: this._anchorRect.translate(parentRect.@ref.left, parentRect.@ref.top), parentRect: parentRect.@ref.toRect(), displayRect: outputRect.@ref.toRect()));
        result.@ref.left = targetRect.left;
        result.@ref.top = targetRect.top;
        result.@ref.width = childSize.@ref.width;
        result.@ref.height = childSize.@ref.height;
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BaseWindowControllerIo parent => this._parent;
    public override void setConstraints(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        _ensureNotDestroyed();
        _MacOSPlatformInterface___window_macos.setWindowConstraints(this.windowHandle, constraints);
    }

    public virtual void _initController(WindowingOwnerMacOSIo owner)
    {
        if (!global::Doroti.Framework.Foundation._featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_window_macosLibrary._kWindowingDisabledErrorMessage);
        }
        this._onShouldClose = new NativeCallable<global::System.Func<Void>>(this._handleOnShouldClose);
        this._onWillClose = new NativeCallable<global::System.Func<Void>>(this._handleOnWillClose);
        this._onResize = new NativeCallable<global::System.Func<Void>>(this._handleOnResize);
        this._onGetWindowPosition = new NativeCallable<global::System.Func<Pointer<_Size___window_macos>, Pointer<_Rect___window_macos>, Pointer<_Rect___window_macos>, Pointer<_Rect___window_macos>>>(this._handleOnGetWindowPosition);
        this._owner = owner;
        ((WindowingOwnerMacOSIo)this._owner)._activeControllers.Add(this);
    }

    public virtual void _ensureNotDestroyed()
    {
        if (this._destroyed)
        {
            throw new InvalidOperationException("Window has been destroyed.");
        }
    }

    public virtual Pointer<Void> windowHandle
    {
        get
        {
            _ensureNotDestroyed();
            return ((Pointer<Void>)(object?)WindowingOwnerMacOSIo.getWindowHandle(this.rootView));
            return default!;
        }
    }
    public override Size contentSize
    {
        get
        {
            _ensureNotDestroyed();
            return _MacOSPlatformInterface___window_macos.getWindowContentSize(this.windowHandle);
            return default!;
        }
    }
    public override void destroy()
    {
        if (this._destroyed)
        {
            return;
        }
        Pointer<Void> handle = this.windowHandle;
        _MacOSPlatformInterface___window_macos.destroyWindow(handle);
    }

    public override bool isDestroyed => this._destroyed;
}

public class PopupWindowControllerMacOSIo : PopupWindowControllerIo, _WindowControllerMixin___window_macos
{
    internal virtual PopupWindowControllerDelegateIo _delegate { get; private set; } = default!;
    internal virtual BaseWindowControllerIo _parent { get; private set; } = default!;
    internal virtual WindowPositionerIo _positioner { get; set; } = default!;
    internal virtual Rect _anchorRect { get; set; } = default!;
    public virtual bool _destroyed { get; set; } = false;
    public virtual NativeCallable<global::System.Func<Void>> _onShouldClose { get; set; } = default!;
    public virtual NativeCallable<global::System.Func<Void>> _onWillClose { get; set; } = default!;
    public virtual NativeCallable<global::System.Func<Void>> _onResize { get; set; } = default!;
    public virtual NativeCallable<global::System.Func<Pointer<_Size___window_macos>, Pointer<_Rect___window_macos>, Pointer<_Rect___window_macos>, Pointer<_Rect___window_macos>>> _onGetWindowPosition { get; set; } = default!;
    public virtual WindowingOwnerMacOSIo _owner { get; set; } = default!;

    public PopupWindowControllerMacOSIo(WindowingOwnerMacOSIo owner, PopupWindowControllerDelegateIo @delegate, global::Doroti.Framework.Rendering.BoxConstraints contentSizeConstraints, BaseWindowControllerIo parent, Rect anchorRect, WindowPositionerIo positioner)
    {
        this._anchorRect = DartRuntimePrimitives.RequireValue(anchorRect);
        this._positioner = positioner;
        this._delegate = @delegate;
        this._parent = parent;
    }

    public override void updatePosition(Rect? anchorRect = null, WindowPositionerIo? positioner = null)
    {
        if ((anchorRect is not null))
        {
            Rect anchorRect__value12906 = DartRuntimePrimitives.RequireValue(anchorRect);
            _anchorRect = DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(anchorRect__value12906));
        }
        if ((positioner is not null))
        {
            _positioner = positioner;
        }
        _MacOSPlatformInterface___window_macos.updateWindowPosition(this.windowHandle);
    }

    public override Offset offsetFromParent
    {
        get
        {
            return _MacOSPlatformInterface___window_macos.getOffsetInParent(this.windowHandle).toOffset();
            return default!;
        }
    }
    public virtual void _handleOnShouldClose()
    {
        destroy();
    }

    public virtual void _handleOnWillClose()
    {
        this._onWillClose.close();
        this._onShouldClose.close();
        this._onResize.close();
        this._onGetWindowPosition.close();
        _destroyed = true;
        ((WindowingOwnerMacOSIo)this._owner)._activeControllers.Remove(this);
        notifyListeners();
        this._delegate.onWindowDestroyed();
    }

    public virtual void _handleOnResize()
    {
        notifyListeners();
    }

    public virtual Pointer<_Rect___window_macos> _handleOnGetWindowPosition(Pointer<_Size___window_macos> childSize, Pointer<_Rect___window_macos> parentRect, Pointer<_Rect___window_macos> outputRect)
    {
        Pointer<_Rect___window_macos> result = _window_macosLibrary._allocator();
        global::Doroti.Ui.Rect targetRect = ((global::Doroti.Ui.Rect)(object?)this._positioner.placeWindow(childSize: childSize.@ref.toSize(), anchorRect: this._anchorRect.translate(parentRect.@ref.left, parentRect.@ref.top), parentRect: parentRect.@ref.toRect(), displayRect: outputRect.@ref.toRect()));
        result.@ref.left = targetRect.left;
        result.@ref.top = targetRect.top;
        result.@ref.width = childSize.@ref.width;
        result.@ref.height = childSize.@ref.height;
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BaseWindowControllerIo parent => this._parent;
    public override void setConstraints(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        _ensureNotDestroyed();
        _MacOSPlatformInterface___window_macos.setWindowConstraints(this.windowHandle, constraints);
    }

    public virtual void _initController(WindowingOwnerMacOSIo owner)
    {
        if (!global::Doroti.Framework.Foundation._featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_window_macosLibrary._kWindowingDisabledErrorMessage);
        }
        this._onShouldClose = new NativeCallable<global::System.Func<Void>>(this._handleOnShouldClose);
        this._onWillClose = new NativeCallable<global::System.Func<Void>>(this._handleOnWillClose);
        this._onResize = new NativeCallable<global::System.Func<Void>>(this._handleOnResize);
        this._onGetWindowPosition = new NativeCallable<global::System.Func<Pointer<_Size___window_macos>, Pointer<_Rect___window_macos>, Pointer<_Rect___window_macos>, Pointer<_Rect___window_macos>>>(this._handleOnGetWindowPosition);
        this._owner = owner;
        ((WindowingOwnerMacOSIo)this._owner)._activeControllers.Add(this);
    }

    public virtual void _ensureNotDestroyed()
    {
        if (this._destroyed)
        {
            throw new InvalidOperationException("Window has been destroyed.");
        }
    }

    public virtual Pointer<Void> windowHandle
    {
        get
        {
            _ensureNotDestroyed();
            return ((Pointer<Void>)(object?)WindowingOwnerMacOSIo.getWindowHandle(this.rootView));
            return default!;
        }
    }
    public override Size contentSize
    {
        get
        {
            _ensureNotDestroyed();
            return _MacOSPlatformInterface___window_macos.getWindowContentSize(this.windowHandle);
            return default!;
        }
    }
    public override void destroy()
    {
        if (this._destroyed)
        {
            return;
        }
        Pointer<Void> handle = this.windowHandle;
        _MacOSPlatformInterface___window_macos.destroyWindow(handle);
    }

    public override bool isDestroyed => this._destroyed;
}

public class WindowControllerMacOSIo : WindowControllerIo, _WindowControllerMixin___window_macos
{
    internal virtual WindowControllerDelegateIo _delegate { get; private set; } = default!;
    public virtual bool _destroyed { get; set; } = false;
    public virtual NativeCallable<global::System.Func<Void>> _onShouldClose { get; set; } = default!;
    public virtual NativeCallable<global::System.Func<Void>> _onWillClose { get; set; } = default!;
    public virtual NativeCallable<global::System.Func<Void>> _onResize { get; set; } = default!;
    public virtual NativeCallable<global::System.Func<Pointer<_Size___window_macos>, Pointer<_Rect___window_macos>, Pointer<_Rect___window_macos>, Pointer<_Rect___window_macos>>> _onGetWindowPosition { get; set; } = default!;
    public virtual WindowingOwnerMacOSIo _owner { get; set; } = default!;

    public WindowControllerMacOSIo(WindowingOwnerMacOSIo owner, WindowControllerDelegateIo @delegate, Size? size, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, string? title = null)
    {
        this._delegate = @delegate;
    }

    public virtual void _handleOnShouldClose()
    {
        this._delegate.onWindowCloseRequested(this);
    }

    public virtual void _handleOnWillClose()
    {
        this._onWillClose.close();
        this._onShouldClose.close();
        this._onResize.close();
        this._onGetWindowPosition.close();
        _destroyed = true;
        ((WindowingOwnerMacOSIo)this._owner)._activeControllers.Remove(this);
        notifyListeners();
        this._delegate.onWindowDestroyed();
    }

    public virtual void _handleOnResize()
    {
        notifyListeners();
    }

    public override void setSize(Size size)
    {
        _ensureNotDestroyed();
        _MacOSPlatformInterface___window_macos.setWindowContentSize(this.windowHandle, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(size)));
    }

    public override void setConstraints(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        _ensureNotDestroyed();
        _MacOSPlatformInterface___window_macos.setWindowConstraints(this.windowHandle, constraints);
    }

    public override void setTitle(string title)
    {
        _ensureNotDestroyed();
        _MacOSPlatformInterface___window_macos.setWindowTitle(this.windowHandle, title);
        notifyListeners();
    }

    public override Size contentSize
    {
        get
        {
            _ensureNotDestroyed();
            return _MacOSPlatformInterface___window_macos.getWindowContentSize(this.windowHandle);
            return default!;
        }
    }
    public override void activate()
    {
        _ensureNotDestroyed();
        _MacOSPlatformInterface___window_macos.activate(this.windowHandle);
    }

    public override void setMaximized(bool maximized)
    {
        _ensureNotDestroyed();
        _MacOSPlatformInterface___window_macos.setMaximized(this.windowHandle, maximized);
    }

    public override bool isMaximized
    {
        get
        {
            _ensureNotDestroyed();
            return _MacOSPlatformInterface___window_macos.isMaximized(this.windowHandle);
            return default!;
        }
    }
    public override void setMinimized(bool minimized)
    {
        _ensureNotDestroyed();
        if (minimized)
        {
            _MacOSPlatformInterface___window_macos.minimize(this.windowHandle);
        }
        else
        {
            _MacOSPlatformInterface___window_macos.unminimize(this.windowHandle);
        }
    }

    public override bool isMinimized
    {
        get
        {
            _ensureNotDestroyed();
            return _MacOSPlatformInterface___window_macos.isMinimized(this.windowHandle);
            return default!;
        }
    }
    public override void setFullscreen(bool fullscreen, Display? display = null)
    {
        _ensureNotDestroyed();
        _MacOSPlatformInterface___window_macos.setFullscreen(this.windowHandle, fullscreen);
    }

    public override bool isFullscreen
    {
        get
        {
            _ensureNotDestroyed();
            return _MacOSPlatformInterface___window_macos.isFullscreen(this.windowHandle);
            return default!;
        }
    }
    public override bool isActivated => _MacOSPlatformInterface___window_macos.isActivated(this.windowHandle);
    public override string title => _MacOSPlatformInterface___window_macos.getTitle(this.windowHandle);
    public virtual void _initController(WindowingOwnerMacOSIo owner)
    {
        if (!global::Doroti.Framework.Foundation._featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_window_macosLibrary._kWindowingDisabledErrorMessage);
        }
        this._onShouldClose = new NativeCallable<global::System.Func<Void>>(this._handleOnShouldClose);
        this._onWillClose = new NativeCallable<global::System.Func<Void>>(this._handleOnWillClose);
        this._onResize = new NativeCallable<global::System.Func<Void>>(this._handleOnResize);
        this._onGetWindowPosition = new NativeCallable<global::System.Func<Pointer<_Size___window_macos>, Pointer<_Rect___window_macos>, Pointer<_Rect___window_macos>, Pointer<_Rect___window_macos>>>(this._handleOnGetWindowPosition);
        this._owner = owner;
        ((WindowingOwnerMacOSIo)this._owner)._activeControllers.Add(this);
    }

    public virtual Pointer<_Rect___window_macos> _handleOnGetWindowPosition(Pointer<_Size___window_macos> childSize, Pointer<_Rect___window_macos> parentRect, Pointer<_Rect___window_macos> outputRect)
    {
        return new Pointer<_Rect___window_macos>(0L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _ensureNotDestroyed()
    {
        if (this._destroyed)
        {
            throw new InvalidOperationException("Window has been destroyed.");
        }
    }

    public virtual Pointer<Void> windowHandle
    {
        get
        {
            _ensureNotDestroyed();
            return ((Pointer<Void>)(object?)WindowingOwnerMacOSIo.getWindowHandle(this.rootView));
            return default!;
        }
    }
    public override void destroy()
    {
        if (this._destroyed)
        {
            return;
        }
        Pointer<Void> handle = this.windowHandle;
        _MacOSPlatformInterface___window_macos.destroyWindow(handle);
    }

    public override bool isDestroyed => this._destroyed;
}

public class DialogWindowControllerMacOSIo : DialogWindowControllerIo, _WindowControllerMixin___window_macos
{
    internal virtual DialogWindowControllerDelegateIo _delegate { get; private set; } = default!;
    private BaseWindowControllerIo? __field_parent = default!;
    public override BaseWindowControllerIo? parent { get => __field_parent; }
    public virtual bool _destroyed { get; set; } = false;
    public virtual NativeCallable<global::System.Func<Void>> _onShouldClose { get; set; } = default!;
    public virtual NativeCallable<global::System.Func<Void>> _onWillClose { get; set; } = default!;
    public virtual NativeCallable<global::System.Func<Void>> _onResize { get; set; } = default!;
    public virtual NativeCallable<global::System.Func<Pointer<_Size___window_macos>, Pointer<_Rect___window_macos>, Pointer<_Rect___window_macos>, Pointer<_Rect___window_macos>>> _onGetWindowPosition { get; set; } = default!;
    public virtual WindowingOwnerMacOSIo _owner { get; set; } = default!;

    public DialogWindowControllerMacOSIo(WindowingOwnerMacOSIo owner, DialogWindowControllerDelegateIo @delegate, Size? size, BaseWindowControllerIo? parent = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, string? title = null)
    {
        this.__field_parent = parent;
        this._delegate = @delegate;
    }

    public virtual void _handleOnShouldClose()
    {
        this._delegate.onWindowCloseRequested(this);
    }

    public virtual void _handleOnWillClose()
    {
        this._onWillClose.close();
        this._onShouldClose.close();
        this._onResize.close();
        this._onGetWindowPosition.close();
        _destroyed = true;
        ((WindowingOwnerMacOSIo)this._owner)._activeControllers.Remove(this);
        notifyListeners();
        this._delegate.onWindowDestroyed();
    }

    public virtual void _handleOnResize()
    {
        notifyListeners();
    }

    public override void setSize(Size size)
    {
        _ensureNotDestroyed();
        _MacOSPlatformInterface___window_macos.setWindowContentSize(this.windowHandle, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(size)));
    }

    public override void setConstraints(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        _ensureNotDestroyed();
        _MacOSPlatformInterface___window_macos.setWindowConstraints(this.windowHandle, constraints);
    }

    public override void setTitle(string title)
    {
        _ensureNotDestroyed();
        _MacOSPlatformInterface___window_macos.setWindowTitle(this.windowHandle, title);
        notifyListeners();
    }

    public override Size contentSize
    {
        get
        {
            _ensureNotDestroyed();
            return _MacOSPlatformInterface___window_macos.getWindowContentSize(this.windowHandle);
            return default!;
        }
    }
    public override void activate()
    {
        _ensureNotDestroyed();
        _MacOSPlatformInterface___window_macos.activate(this.windowHandle);
    }

    public override void setMinimized(bool minimized)
    {
        _ensureNotDestroyed();
        if (minimized)
        {
            _MacOSPlatformInterface___window_macos.minimize(this.windowHandle);
        }
        else
        {
            _MacOSPlatformInterface___window_macos.unminimize(this.windowHandle);
        }
    }

    public override bool isMinimized
    {
        get
        {
            _ensureNotDestroyed();
            return _MacOSPlatformInterface___window_macos.isMinimized(this.windowHandle);
            return default!;
        }
    }
    public override bool isActivated => _MacOSPlatformInterface___window_macos.isActivated(this.windowHandle);
    public override string title => _MacOSPlatformInterface___window_macos.getTitle(this.windowHandle);
    public virtual void _initController(WindowingOwnerMacOSIo owner)
    {
        if (!global::Doroti.Framework.Foundation._featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_window_macosLibrary._kWindowingDisabledErrorMessage);
        }
        this._onShouldClose = new NativeCallable<global::System.Func<Void>>(this._handleOnShouldClose);
        this._onWillClose = new NativeCallable<global::System.Func<Void>>(this._handleOnWillClose);
        this._onResize = new NativeCallable<global::System.Func<Void>>(this._handleOnResize);
        this._onGetWindowPosition = new NativeCallable<global::System.Func<Pointer<_Size___window_macos>, Pointer<_Rect___window_macos>, Pointer<_Rect___window_macos>, Pointer<_Rect___window_macos>>>(this._handleOnGetWindowPosition);
        this._owner = owner;
        ((WindowingOwnerMacOSIo)this._owner)._activeControllers.Add(this);
    }

    public virtual Pointer<_Rect___window_macos> _handleOnGetWindowPosition(Pointer<_Size___window_macos> childSize, Pointer<_Rect___window_macos> parentRect, Pointer<_Rect___window_macos> outputRect)
    {
        return new Pointer<_Rect___window_macos>(0L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _ensureNotDestroyed()
    {
        if (this._destroyed)
        {
            throw new InvalidOperationException("Window has been destroyed.");
        }
    }

    public virtual Pointer<Void> windowHandle
    {
        get
        {
            _ensureNotDestroyed();
            return ((Pointer<Void>)(object?)WindowingOwnerMacOSIo.getWindowHandle(this.rootView));
            return default!;
        }
    }
    public override void destroy()
    {
        if (this._destroyed)
        {
            return;
        }
        Pointer<Void> handle = this.windowHandle;
        _MacOSPlatformInterface___window_macos.destroyWindow(handle);
    }

    public override bool isDestroyed => this._destroyed;
}

internal class _WindowCreationRequest___window_macos : Struct
{
    public virtual bool hasSize { get; set; } = default!;
    public virtual _Size___window_macos contentSize { get; set; } = default!;
    public virtual bool hasConstraints { get; set; } = default!;
    public virtual _Constraints___window_macos constraints { get; set; } = default!;
    public virtual long parentViewId { get; set; } = default!;
    public virtual Pointer<NativeFunction<global::System.Func<Void>>> onShouldClose { get; set; } = default!;
    public virtual Pointer<NativeFunction<global::System.Func<Void>>> onWillClose { get; set; } = default!;
    public virtual Pointer<NativeFunction<global::System.Func<Void>>> onNotifyListeners { get; set; } = default!;
    public virtual Pointer<NativeFunction<global::System.Func<Pointer<_Size___window_macos>, Pointer<_Rect___window_macos>, Pointer<_Rect___window_macos>, Pointer<_Rect___window_macos>>>> onGetWindowPosition { get; set; } = default!;

}

internal class _Size___window_macos : Struct
{
    public virtual double width { get; set; } = default!;
    public virtual double height { get; set; } = default!;

    public override string ToString() => $"Size(width: {this.width}, height: {this.height})";
    public virtual global::Doroti.Ui.Size toSize() => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Size>(new global::Doroti.Ui.Size(this.width, this.height));
}

internal class _Offset___window_macos : Struct
{
    public virtual double x { get; set; } = default!;
    public virtual double y { get; set; } = default!;

    public override string ToString() => $"Offset(x: {this.x}, y: {this.y})";
    public virtual global::Doroti.Ui.Offset toOffset() => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Offset>(new global::Doroti.Ui.Offset(this.x, this.y));
}

internal class _Rect___window_macos : Struct
{
    public virtual double left { get; set; } = default!;
    public virtual double top { get; set; } = default!;
    public virtual double width { get; set; } = default!;
    public virtual double height { get; set; } = default!;

    public virtual global::Doroti.Ui.Rect toRect()
    {
        return global::Doroti.Ui.Rect.fromLTWH(this.left, this.top, this.width, this.height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"Rect(left: {this.left}, top: {this.top}, width: {this.width}, height: {this.height})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _Constraints___window_macos : Struct
{
    public virtual double minWidth { get; set; } = default!;
    public virtual double minHeight { get; set; } = default!;
    public virtual double maxWidth { get; set; } = default!;
    public virtual double maxHeight { get; set; } = default!;

}

internal class _MacOSPlatformInterface___window_macos
{
    public abstract static Pointer<Void> getWindowHandle(long engineId, long viewId);
    internal abstract static void _setWindowContentSize(Pointer<Void> windowHandle, Pointer<_Size___window_macos> size);
    public static void setWindowContentSize(Pointer<Void> windowHandle, Size size)
    {
        Pointer<_Size___window_macos> ffiSize = _window_macosLibrary._allocator();
        DartRuntimePrimitives.Ignore(((Func<_Size___window_macos>)(() =>
{            var __cascade = ffiSize.@ref;
            __cascade.width = DartRuntimePrimitives.RequireValue(size).width;
            __cascade.height = DartRuntimePrimitives.RequireValue(size).height;
            return __cascade;        }))());
        _MacOSPlatformInterface___window_macos._setWindowContentSize(windowHandle, ffiSize);
        _window_macosLibrary._allocator.free(ffiSize);
    }

    internal abstract static void _setWindowConstraints(Pointer<Void> windowHandle, Pointer<_Constraints___window_macos> size);
    public static void setWindowConstraints(Pointer<Void> windowHandle, global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        Pointer<_Constraints___window_macos> ffiConstraints = _window_macosLibrary._allocator();
        DartRuntimePrimitives.Ignore(((Func<_Constraints___window_macos>)(() =>
{            var __cascade = ffiConstraints.@ref;
            __cascade.minWidth = ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).minWidth;
            __cascade.minHeight = ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).minHeight;
            __cascade.maxWidth = ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth;
            __cascade.maxHeight = ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxHeight;
            return __cascade;        }))());
        _MacOSPlatformInterface___window_macos._setWindowConstraints(windowHandle, ffiConstraints);
        _window_macosLibrary._allocator.free(ffiConstraints);
    }

    internal abstract static long _createWindow(long engineId, Pointer<_WindowCreationRequest___window_macos> request);
    public static long createWindow(Size? size, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, Pointer<NativeFunction<global::System.Func<Void>>> onShouldClose = default!, Pointer<NativeFunction<global::System.Func<Void>>> onWillClose = default!, Pointer<NativeFunction<global::System.Func<Void>>> onNotifyListeners = default!)
    {
        Pointer<_WindowCreationRequest___window_macos> request = ((Func<Pointer<_WindowCreationRequest___window_macos>>)(() =>
{            var __cascade = _window_macosLibrary._allocator();
            __cascade.@ref.onShouldClose = onShouldClose;
            __cascade.@ref.onWillClose = onWillClose;
            __cascade.@ref.onNotifyListeners = onNotifyListeners;
            return __cascade;        }))();
        if ((size is not null))
        {
            Size size__value25007 = DartRuntimePrimitives.RequireValue(size);
            DartRuntimePrimitives.Ignore(((Func<_WindowCreationRequest___window_macos>)(() =>
{            var __cascade = request.@ref;
            __cascade.hasSize = true;
            __cascade.contentSize.width = DartRuntimePrimitives.RequireValue(size__value25007).width;
            __cascade.contentSize.height = DartRuntimePrimitives.RequireValue(size__value25007).height;
            return __cascade;        }))());
        }
        if ((constraints is not null))
        {
            DartRuntimePrimitives.Ignore(((Func<_WindowCreationRequest___window_macos>)(() =>
{            var __cascade = request.@ref;
            __cascade.hasConstraints = true;
            __cascade.constraints.minWidth = ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).minWidth;
            __cascade.constraints.minHeight = ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).minHeight;
            __cascade.constraints.maxWidth = ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth;
            __cascade.constraints.maxHeight = ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxHeight;
            return __cascade;        }))());
        }
        long viewId = _MacOSPlatformInterface___window_macos._createWindow(DartRuntimePrimitives.RequireValue(WidgetsBinding.instance.platformDispatcher.engineId), request);
        _window_macosLibrary._allocator.free(request);
        return viewId;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal abstract static long _createDialogWindow(long engineId, Pointer<_WindowCreationRequest___window_macos> request);
    public static long createDialogWindow(Size? size, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, long? parentViewId = null, Pointer<NativeFunction<global::System.Func<Void>>> onShouldClose = default!, Pointer<NativeFunction<global::System.Func<Void>>> onWillClose = default!, Pointer<NativeFunction<global::System.Func<Void>>> onNotifyListeners = default!)
    {
        Pointer<_WindowCreationRequest___window_macos> request = ((Func<Pointer<_WindowCreationRequest___window_macos>>)(() =>
{            var __cascade = _window_macosLibrary._allocator();
            __cascade.@ref.onShouldClose = onShouldClose;
            __cascade.@ref.onWillClose = onWillClose;
            __cascade.@ref.onNotifyListeners = onNotifyListeners;
            __cascade.@ref.parentViewId = (parentViewId ?? 0L);
            return __cascade;        }))();
        if ((size is not null))
        {
            Size size__value26560 = DartRuntimePrimitives.RequireValue(size);
            DartRuntimePrimitives.Ignore(((Func<_WindowCreationRequest___window_macos>)(() =>
{            var __cascade = request.@ref;
            __cascade.hasSize = true;
            __cascade.contentSize.width = DartRuntimePrimitives.RequireValue(size__value26560).width;
            __cascade.contentSize.height = DartRuntimePrimitives.RequireValue(size__value26560).height;
            return __cascade;        }))());
        }
        if ((constraints is not null))
        {
            DartRuntimePrimitives.Ignore(((Func<_WindowCreationRequest___window_macos>)(() =>
{            var __cascade = request.@ref;
            __cascade.hasConstraints = true;
            __cascade.constraints.minWidth = ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).minWidth;
            __cascade.constraints.minHeight = ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).minHeight;
            __cascade.constraints.maxWidth = ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth;
            __cascade.constraints.maxHeight = ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxHeight;
            return __cascade;        }))());
        }
        try
        {
            long viewId = _MacOSPlatformInterface___window_macos._createDialogWindow(DartRuntimePrimitives.RequireValue(WidgetsBinding.instance.platformDispatcher.engineId), request);
            return viewId;
        }
        finally
        {
            _window_macosLibrary._allocator.free(request);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal abstract static long _createTooltipWindow(long engineId, Pointer<_WindowCreationRequest___window_macos> request);
    public static long createTooltipWindow(global::Doroti.Framework.Rendering.BoxConstraints constraints, long parentViewId, Pointer<NativeFunction<global::System.Func<Void>>> onShouldClose, Pointer<NativeFunction<global::System.Func<Void>>> onWillClose, Pointer<NativeFunction<global::System.Func<Void>>> onNotifyListeners, Pointer<NativeFunction<global::System.Func<Pointer<_Size___window_macos>, Pointer<_Rect___window_macos>, Pointer<_Rect___window_macos>, Pointer<_Rect___window_macos>>>> onGetWindowPosition)
    {
        Pointer<_WindowCreationRequest___window_macos> request = ((Func<Pointer<_WindowCreationRequest___window_macos>>)(() =>
{            var __cascade = _window_macosLibrary._allocator();
            __cascade.@ref.onShouldClose = onShouldClose;
            __cascade.@ref.onWillClose = onWillClose;
            __cascade.@ref.onNotifyListeners = onNotifyListeners;
            __cascade.@ref.onGetWindowPosition = onGetWindowPosition;
            __cascade.@ref.parentViewId = DartRuntimePrimitives.RequireValue(parentViewId);
            return __cascade;        }))();
        DartRuntimePrimitives.Ignore(((Func<_WindowCreationRequest___window_macos>)(() =>
{            var __cascade = request.@ref;
            __cascade.hasConstraints = true;
            __cascade.constraints.minWidth = ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).minWidth;
            __cascade.constraints.minHeight = ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).minHeight;
            __cascade.constraints.maxWidth = ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth;
            __cascade.constraints.maxHeight = ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxHeight;
            return __cascade;        }))());
        long viewId = _MacOSPlatformInterface___window_macos._createTooltipWindow(DartRuntimePrimitives.RequireValue(PlatformDispatcher.instance.engineId), request);
        _window_macosLibrary._allocator.free(request);
        return viewId;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal abstract static long _createPopupWindow(long engineId, Pointer<_WindowCreationRequest___window_macos> request);
    public static long createPopupWindow(global::Doroti.Framework.Rendering.BoxConstraints constraints, long parentViewId, Pointer<NativeFunction<global::System.Func<Void>>> onShouldClose, Pointer<NativeFunction<global::System.Func<Void>>> onWillClose, Pointer<NativeFunction<global::System.Func<Void>>> onNotifyListeners, Pointer<NativeFunction<global::System.Func<Pointer<_Size___window_macos>, Pointer<_Rect___window_macos>, Pointer<_Rect___window_macos>, Pointer<_Rect___window_macos>>>> onGetWindowPosition)
    {
        Pointer<_WindowCreationRequest___window_macos> request = ((Func<Pointer<_WindowCreationRequest___window_macos>>)(() =>
{            var __cascade = _window_macosLibrary._allocator();
            __cascade.@ref.onShouldClose = onShouldClose;
            __cascade.@ref.onWillClose = onWillClose;
            __cascade.@ref.onNotifyListeners = onNotifyListeners;
            __cascade.@ref.onGetWindowPosition = onGetWindowPosition;
            __cascade.@ref.parentViewId = DartRuntimePrimitives.RequireValue(parentViewId);
            return __cascade;        }))();
        DartRuntimePrimitives.Ignore(((Func<_WindowCreationRequest___window_macos>)(() =>
{            var __cascade = request.@ref;
            __cascade.hasConstraints = true;
            __cascade.constraints.minWidth = ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).minWidth;
            __cascade.constraints.minHeight = ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).minHeight;
            __cascade.constraints.maxWidth = ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth;
            __cascade.constraints.maxHeight = ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxHeight;
            return __cascade;        }))());
        long viewId = _MacOSPlatformInterface___window_macos._createPopupWindow(DartRuntimePrimitives.RequireValue(PlatformDispatcher.instance.engineId), request);
        _window_macosLibrary._allocator.free(request);
        return viewId;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal abstract static void _destroyWindow(long engineId, Pointer<Void> handle);
    public static void destroyWindow(Pointer<Void> windowHandle)
    {
        _MacOSPlatformInterface___window_macos._destroyWindow(DartRuntimePrimitives.RequireValue(WidgetsBinding.instance.platformDispatcher.engineId), windowHandle);
    }

    internal abstract static _Size___window_macos _getWindowContentSize(Pointer<Void> windowHandle);
    public static global::Doroti.Ui.Size getWindowContentSize(Pointer<Void> windowHandle)
    {
        _Size___window_macos size = ((_Size___window_macos)(object?)_MacOSPlatformInterface___window_macos._getWindowContentSize(windowHandle));
        return new global::Doroti.Ui.Size(((_Size___window_macos)size).width, ((_Size___window_macos)size).height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal abstract static void _setWindowTitle(Pointer<Void> windowHandle, Pointer<_Utf8___window_macos> title);
    public static void setWindowTitle(Pointer<Void> windowHandle, string title)
    {
        Pointer<_Utf8___window_macos> titlePointer = title.toNativeUtf8();
        _MacOSPlatformInterface___window_macos._setWindowTitle(windowHandle, titlePointer);
        _window_macosLibrary._allocator.free(titlePointer);
    }

    public abstract static void setMaximized(Pointer<Void> windowHandle, bool maximized);
    public abstract static bool isMaximized(Pointer<Void> windowHandle);
    public abstract static void minimize(Pointer<Void> windowHandle);
    public abstract static void unminimize(Pointer<Void> windowHandle);
    public abstract static bool isMinimized(Pointer<Void> windowHandle);
    public abstract static void setFullscreen(Pointer<Void> windowHandle, bool fullscreen);
    public abstract static bool isFullscreen(Pointer<Void> windowHandle);
    public abstract static void activate(Pointer<Void> windowHandle);
    internal abstract static Pointer<_Utf8___window_macos> _getTitle(Pointer<Void> windowHandle);
    public static string getTitle(Pointer<Void> windowHandle)
    {
        Pointer<_Utf8___window_macos> title = ((Pointer<_Utf8___window_macos>)(object?)_MacOSPlatformInterface___window_macos._getTitle(windowHandle));
        string result = title.toDartString();
        _window_macosLibrary._allocator.free(title);
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract static bool isActivated(Pointer<Void> windowHandle);
    public abstract static void updateWindowPosition(Pointer<Void> windowHandle);
    public abstract static _Offset___window_macos getOffsetInParent(Pointer<Void> windowHandle);
}

internal delegate Pointer<Void> _PosixCallocNative___window_macos(IntPtr num, IntPtr size);

internal delegate Void _PosixFreeNative___window_macos(Pointer<NativeType> __unused0);

public static partial class _window_macosLibrary
{
    internal static Pointer<NativeFunction<global::System.Func<Pointer<NativeType>, Void>>> _posixFreePointer = Dart_ffiLibrary.addressOf<NativeFunction<global::System.Func<Pointer<NativeType>, Void>>>(_window_macosLibrary._posixFree);
}

public static partial class _window_macosLibrary
{
    internal static _CallocAllocator___window_macos _allocator = new _CallocAllocator___window_macos();
}

internal class _CallocAllocator___window_macos : Allocator
{
    internal _CallocAllocator___window_macos()
    {
    }

    public virtual Pointer<T> allocate<T>(long byteCount, long? alignment = null) where T : NativeType
    {
        Pointer<T> result = _posixCalloc(byteCount, 1L).cast<T>();
        if ((result.address == 0L))
        {
            throw DartRuntimePrimitives.AsException(new DartArgumentError($"Could not allocate {byteCount} bytes."));
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void free(Pointer<NativeType> pointer)
    {
        _posixFree(pointer);
    }

    public virtual Pointer<NativeFinalizerFunction> nativeFree => DartRuntimePrimitives.ConvertValue<Pointer<NativeFinalizerFunction>>(_window_macosLibrary._posixFreePointer);
}

internal class _Utf8___window_macos : Opaque
{
}

