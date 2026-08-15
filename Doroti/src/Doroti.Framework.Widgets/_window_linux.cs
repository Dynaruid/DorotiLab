// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/_window_linux.dart
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

namespace Doroti.Generated.Framework.Widgets;

public static partial class _window_linuxLibrary
{
    internal static long _kMaxWindowDimensions = 2147483647L;
}

public static partial class _window_linuxLibrary
{
    internal static string _kWindowingDisabledErrorMessage = "Windowing APIs are not enabled.\n\nWindowing APIs are currently experimental. Do not use windowing APIs in\nproduction applications or plugins published to pub.dev.\n\nTo try experimental windowing APIs:\n1. Switch to Flutter's main release channel.\n2. Turn on the windowing feature flag.\n\nSee: https://github.com/flutter/flutter/issues/30701.\n";
}

public class WindowingOwnerLinuxIo : WindowingOwnerIo
{
    internal virtual LinuxWindowRegistrarIo _registrar { get; private set; } = new LinuxWindowRegistrarIo();

    public WindowingOwnerLinuxIo()
    {
    }

    public virtual LinuxWindowRegistrarIo registrar => this._registrar;
    public virtual WindowControllerIo createWindowController(WindowControllerDelegateIo @delegate = default!, Size? size = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, bool resizable = default!, string? title = null)
    {
        bool __resizable = DartRuntimePrimitives.ConvertValue<bool>(constraints);
        var __constraints = size is null ? null : (global::Doroti.Generated.Framework.Rendering.BoxConstraints)(object)size;
        var __title = resizable is null ? null : (string)(object)resizable;
        var __delegate = (WindowControllerDelegateIo)(object)title;
        var controller__3396 = new WindowControllerLinuxIo(owner: this, @delegate: __delegate, size: title, constraints: __constraints, title: __title);
        this._registrar.register(viewId: checked((long)controller__3396.rootView.viewId), windowHandle: ((WindowControllerLinuxIo)controller__3396)._window.instance.cast<Void>(), viewHandle: ((WindowControllerLinuxIo)controller__3396)._view.instance.cast<Void>());
        return ((WindowControllerIo)(object?)controller__3396);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DialogWindowControllerIo createDialogWindowController(DialogWindowControllerDelegateIo @delegate, Size? size = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, bool resizable = default!, BaseWindowControllerIo? parent = null, string? title = null)
    {
        var controller__4046 = new DialogWindowControllerLinuxIo(owner: this, @delegate: @delegate, size: size, constraints: constraints, parent: parent, title: title);
        this._registrar.register(viewId: checked((long)controller__4046.rootView.viewId), windowHandle: ((DialogWindowControllerLinuxIo)controller__4046)._window.instance.cast<Void>(), viewHandle: ((DialogWindowControllerLinuxIo)controller__4046)._view.instance.cast<Void>());
        return ((DialogWindowControllerIo)(object?)controller__4046);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TooltipWindowControllerIo createTooltipWindowController(TooltipWindowControllerDelegateIo @delegate, global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints, Rect anchorRect, WindowPositionerIo positioner, BaseWindowControllerIo parent)
    {
        var controller__4751 = new TooltipWindowControllerLinuxIo(owner: this, @delegate: @delegate, constraints: constraints, anchorRect: anchorRect, positioner: positioner, parent: parent);
        this._registrar.register(viewId: checked((long)controller__4751.rootView.viewId), windowHandle: ((TooltipWindowControllerLinuxIo)controller__4751)._window.instance.cast<Void>(), viewHandle: ((TooltipWindowControllerLinuxIo)controller__4751)._view.instance.cast<Void>());
        return ((TooltipWindowControllerIo)(object?)controller__4751);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual PopupWindowControllerIo createPopupWindowController(PopupWindowControllerDelegateIo @delegate, global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints, Rect anchorRect, WindowPositionerIo positioner, BaseWindowControllerIo parent)
    {
        var controller__5473 = new PopupWindowControllerLinuxIo(owner: this, @delegate: @delegate, constraints: constraints, anchorRect: anchorRect, positioner: positioner, parent: parent);
        this._registrar.register(viewId: checked((long)controller__5473.rootView.viewId), windowHandle: ((PopupWindowControllerLinuxIo)controller__5473)._window.instance.cast<Void>(), viewHandle: ((PopupWindowControllerLinuxIo)controller__5473)._view.instance.cast<Void>());
        return ((PopupWindowControllerIo)(object?)controller__5473);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SatelliteWindowControllerIo createSatelliteWindowController(SatelliteWindowControllerDelegateIo @delegate, BaseWindowControllerIo parent, WindowPositionerIo initialPositioner, Rect? initialAnchorRect = null, Size? size = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, bool resizable = false, string? title = null)
    {
        throw new NotImplementedException("Satellite windows are not yet implemented on Linux.");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class LinuxWindowRegistrarIo
{
    internal virtual DartMap<long, _GtkWindow___window_linux> _windows { get; private set; } = new DartMap<long, _GtkWindow___window_linux>();
    internal virtual DartMap<long, _FlView___window_linux> _views { get; private set; } = new DartMap<long, _FlView___window_linux>();

    public virtual void register(long viewId, Pointer<Void> windowHandle, Pointer<Void> viewHandle)
    {
        this._windows[viewId] = _GtkWindow___window_linux.CreateFromHandle(windowHandle);
        this._views[viewId] = _FlView___window_linux.CreateFromHandle(viewHandle);
    }

    public virtual void unregister(long viewId)
    {
        this._windows.remove(viewId);
        this._views.remove(viewId);
    }

    internal virtual _GtkWindow___window_linux? _windowForViewId(long viewId) => this._windows.GetValueOrDefault(viewId);
    internal virtual _FlView___window_linux? _viewForViewId(long viewId) => this._views.GetValueOrDefault(viewId);
}

internal interface BaseWindowControllerLinuxIo
{
    public Pointer<Void> windowHandle { get; }
    public Pointer<Void> flutterViewHandle { get; }
}

public class WindowControllerLinuxIo : WindowControllerIo, BaseWindowControllerLinuxIo
{
    internal virtual WindowingOwnerLinuxIo _owner { get; private set; } = default!;
    internal virtual WindowControllerDelegateIo _delegate { get; private set; } = default!;
    internal virtual _GtkWindow___window_linux _window { get; private set; } = default!;
    internal virtual _FlView___window_linux _view { get; private set; } = default!;
    internal virtual _FlViewMonitor___window_linux _viewMonitor { get; private set; } = default!;
    internal virtual _FlWindowMonitor___window_linux _windowMonitor { get; private set; } = default!;
    internal virtual bool _destroyed { get; set; } = false;

    public WindowControllerLinuxIo(WindowingOwnerLinuxIo owner, WindowControllerDelegateIo @delegate, Size? size = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, string? title = null, bool decorated = true)
    {
        this._owner = owner;
        this._delegate = @delegate;
        this._window = new _GtkWindow___window_linux(_GtkWindowType___window_linux.toplevel);
    }

    public override bool isDestroyed => this._destroyed;
    public override Size contentSize => this._window.getSize();
    public override void destroy()
    {
        if (this._destroyed)
        {
            return;
        }
        this._viewMonitor.close();
        this._viewMonitor.unref();
        this._window.destroy();
        this._windowMonitor.close();
        this._windowMonitor.unref();
        _destroyed = true;
        ((WindowingOwnerLinuxIo)this._owner).registrar.unregister(checked((long)this.rootView.viewId));
        notifyListeners();
    }

    public override string title => this._window.getTitle();
    public override bool isActivated => this._window.isActive();
    public override bool isMaximized => this._window.getWindow().getState().Contains(_GdkWindowState___window_linux.maximized);
    public override bool isMinimized => this._window.getWindow().getState().Contains(_GdkWindowState___window_linux.iconified);
    public override bool isFullscreen => this._window.getWindow().getState().Contains(_GdkWindowState___window_linux.fullscreen);
    public override void setSize(Size size)
    {
        this._window.resize(DartRuntimePrimitives.RequireValue(size).width.toInt(), DartRuntimePrimitives.RequireValue(size).height.toInt());
    }

    public override void setConstraints(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints)
    {
        this._window.setGeometryHints(minWidth: ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).minWidth.toInt(), minHeight: ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).minHeight.toInt(), maxWidth: (double.IsInfinity(((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxWidth) ? _window_linuxLibrary._kMaxWindowDimensions : ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxWidth.toInt()), maxHeight: (double.IsInfinity(((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxHeight) ? _window_linuxLibrary._kMaxWindowDimensions : ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxHeight.toInt()));
    }

    public override void setTitle(string title)
    {
        this._window.setTitle(title);
    }

    public override void activate()
    {
        this._window.present();
    }

    public override void setMaximized(bool maximized)
    {
        if (maximized)
        {
            this._window.maximize();
        }
        else
        {
            this._window.unmaximize();
        }
    }

    public override void setMinimized(bool minimized)
    {
        if (minimized)
        {
            this._window.iconify();
        }
        else
        {
            this._window.deiconify();
        }
    }

    public override void setFullscreen(bool fullscreen, Display? display = null)
    {
        if (fullscreen)
        {
            this._window.fullscreen();
        }
        else
        {
            this._window.unfullscreen();
        }
    }

    public virtual Pointer<Void> windowHandle
    {
        get
        {
            if (this._destroyed)
            {
                throw new InvalidOperationException("Window has been destroyed.");
            }
            return this._window.instance.cast<Void>();
            return default!;
        }
    }
    public virtual Pointer<Void> flutterViewHandle
    {
        get
        {
            if (this._destroyed)
            {
                throw new InvalidOperationException("Window has been destroyed.");
            }
            return this._view.instance.cast<Void>();
            return default!;
        }
    }
}

public class DialogWindowControllerLinuxIo : DialogWindowControllerIo, BaseWindowControllerLinuxIo
{
    internal virtual WindowingOwnerLinuxIo _owner { get; private set; } = default!;
    internal virtual DialogWindowControllerDelegateIo _delegate { get; private set; } = default!;
    internal virtual _GtkWindow___window_linux _window { get; private set; } = default!;
    internal virtual BaseWindowControllerIo? _parent { get; private set; }
    internal virtual _FlView___window_linux _view { get; private set; } = default!;
    internal virtual _FlViewMonitor___window_linux _viewMonitor { get; private set; } = default!;
    internal virtual _FlWindowMonitor___window_linux _windowMonitor { get; private set; } = default!;
    internal virtual bool _destroyed { get; set; } = false;

    public DialogWindowControllerLinuxIo(WindowingOwnerLinuxIo owner, DialogWindowControllerDelegateIo @delegate, Size? size = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, BaseWindowControllerIo? parent = null, string? title = null, bool decorated = true)
    {
        this._owner = owner;
        this._delegate = @delegate;
        this._parent = parent;
        this._window = new _GtkWindow___window_linux(_GtkWindowType___window_linux.toplevel);
    }

    public override bool isDestroyed => this._destroyed;
    public override Size contentSize => this._window.getSize();
    public override void destroy()
    {
        if (this._destroyed)
        {
            return;
        }
        this._viewMonitor.close();
        this._viewMonitor.unref();
        this._window.destroy();
        this._windowMonitor.close();
        this._windowMonitor.unref();
        _destroyed = true;
        ((WindowingOwnerLinuxIo)this._owner).registrar.unregister(checked((long)this.rootView.viewId));
        notifyListeners();
    }

    public override BaseWindowControllerIo? parent => this._parent;
    public override string title => this._window.getTitle();
    public override bool isActivated => this._window.isActive();
    public override bool isMinimized => this._window.getWindow().getState().Contains(_GdkWindowState___window_linux.iconified);
    public override void setSize(Size size)
    {
        this._window.resize(DartRuntimePrimitives.RequireValue(size).width.toInt(), DartRuntimePrimitives.RequireValue(size).height.toInt());
    }

    public override void setConstraints(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints)
    {
        this._window.setGeometryHints(minWidth: ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).minWidth.toInt(), minHeight: ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).minHeight.toInt(), maxWidth: (double.IsInfinity(((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxWidth) ? 2147483647L : ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxWidth.toInt()), maxHeight: (double.IsInfinity(((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxHeight) ? 2147483647L : ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxHeight.toInt()));
    }

    public override void setTitle(string title)
    {
        this._window.setTitle(title);
    }

    public override void activate()
    {
        this._window.present();
    }

    public override void setMinimized(bool minimized)
    {
        if (minimized)
        {
            this._window.iconify();
        }
        else
        {
            this._window.deiconify();
        }
    }

    public virtual Pointer<Void> windowHandle
    {
        get
        {
            if (this._destroyed)
            {
                throw new InvalidOperationException("Window has been destroyed.");
            }
            return this._window.instance.cast<Void>();
            return default!;
        }
    }
    public virtual Pointer<Void> flutterViewHandle
    {
        get
        {
            if (this._destroyed)
            {
                throw new InvalidOperationException("Window has been destroyed.");
            }
            return this._view.instance.cast<Void>();
            return default!;
        }
    }
}

public class TooltipWindowControllerLinuxIo : TooltipWindowControllerIo, BaseWindowControllerLinuxIo
{
    internal virtual WindowingOwnerLinuxIo _owner { get; private set; } = default!;
    internal virtual TooltipWindowControllerDelegateIo _delegate { get; private set; } = default!;
    internal virtual _GtkWindow___window_linux _window { get; private set; } = default!;
    internal virtual Rect _anchorRect { get; set; } = default!;
    internal virtual WindowPositionerIo _positioner { get; set; } = default!;
    internal virtual BaseWindowControllerIo _parent { get; private set; } = default!;
    internal virtual _FlView___window_linux _view { get; private set; } = default!;
    internal virtual _FlViewMonitor___window_linux _viewMonitor { get; private set; } = default!;
    internal virtual _FlWindowMonitor___window_linux _windowMonitor { get; private set; } = default!;
    internal virtual bool _destroyed { get; set; } = false;

    public TooltipWindowControllerLinuxIo(WindowingOwnerLinuxIo owner, TooltipWindowControllerDelegateIo @delegate, global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints, Rect anchorRect, WindowPositionerIo positioner, BaseWindowControllerIo parent)
    {
        this._owner = owner;
        this._delegate = @delegate;
        this._parent = parent;
        this._window = new _GtkWindow___window_linux(_GtkWindowType___window_linux.popup);
    }

    public override bool isDestroyed => this._destroyed;
    public override Size contentSize => this._window.getSize();
    public override void destroy()
    {
        if (this._destroyed)
        {
            return;
        }
        this._viewMonitor.close();
        this._viewMonitor.unref();
        this._window.destroy();
        this._windowMonitor.close();
        this._windowMonitor.unref();
        _destroyed = true;
        ((WindowingOwnerLinuxIo)this._owner).registrar.unregister(checked((long)this.rootView.viewId));
        notifyListeners();
    }

    public override void updatePosition(Rect? anchorRect = null, WindowPositionerIo? positioner = null)
    {
        if ((anchorRect is not null))
        {
            Rect anchorRect__value23034 = DartRuntimePrimitives.RequireValue(anchorRect);
            _anchorRect = DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(anchorRect__value23034));
        }
        if ((positioner is not null))
        {
            _positioner = positioner;
        }
        _GtkWindow___window_linux? parentWindow__23185 = ((_GtkWindow___window_linux?)(object?)((WindowingOwnerLinuxIo)this._owner).registrar._windowForViewId(checked((long)((BaseWindowControllerIo)this._parent).rootView.viewId)));
        _FlView___window_linux? view__23279 = ((_FlView___window_linux?)(object?)((WindowingOwnerLinuxIo)this._owner).registrar._viewForViewId(checked((long)((BaseWindowControllerIo)this._parent).rootView.viewId)));
        var offset__23352 = (0L, 0L);
        if (((parentWindow__23185 is not null) && (view__23279 is not null)))
        {
            offset__23352 = (view__23279.translateCoordinates(parentWindow__23185, (0L, 0L)) ?? (0L, 0L));
        }
        this._window.getWindow().moveToRect(x: (this._anchorRect.left.toInt() + offset__23352.Item1), y: (this._anchorRect.top.toInt() + offset__23352.Item2), width: ((this._anchorRect.right - this._anchorRect.left)).toInt(), height: ((this._anchorRect.bottom - this._anchorRect.top)).toInt(), rectAnchor: _anchorToGravity(((WindowPositionerIo)this._positioner).parentAnchor), windowAnchor: _anchorToGravity(((WindowPositionerIo)this._positioner).childAnchor), anchorHints: _constraintAdjustmentToHints(((WindowPositionerIo)this._positioner).constraintAdjustment), rectAnchorDx: ((WindowPositionerIo)this._positioner).offset.dx.toInt(), rectAnchorDy: ((WindowPositionerIo)this._positioner).offset.dy.toInt());
    }

    internal virtual _GdkGravity___window_linux _anchorToGravity(WindowPositionerAnchorIo anchor)
    {
        return (anchor switch { WindowPositionerAnchorIo.center => _GdkGravity___window_linux.center, WindowPositionerAnchorIo.top => _GdkGravity___window_linux.north, WindowPositionerAnchorIo.bottom => _GdkGravity___window_linux.south, WindowPositionerAnchorIo.left => _GdkGravity___window_linux.west, WindowPositionerAnchorIo.right => _GdkGravity___window_linux.east, WindowPositionerAnchorIo.topLeft => _GdkGravity___window_linux.northWest, WindowPositionerAnchorIo.bottomLeft => _GdkGravity___window_linux.southWest, WindowPositionerAnchorIo.topRight => _GdkGravity___window_linux.northEast, WindowPositionerAnchorIo.bottomRight => _GdkGravity___window_linux.southEast, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual HashSet<_GdkAnchorHint___window_linux> _constraintAdjustmentToHints(WindowPositionerConstraintAdjustmentIo adjustment)
    {
        return new HashSet<_GdkAnchorHint___window_linux>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BaseWindowControllerIo parent => this._parent;
    public override void setConstraints(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints)
    {
        this._window.setGeometryHints(minWidth: ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).minWidth.toInt(), minHeight: ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).minHeight.toInt(), maxWidth: (double.IsInfinity(((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxWidth) ? 2147483647L : ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxWidth.toInt()), maxHeight: (double.IsInfinity(((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxHeight) ? 2147483647L : ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxHeight.toInt()));
    }

    public virtual Pointer<Void> windowHandle
    {
        get
        {
            if (this._destroyed)
            {
                throw new InvalidOperationException("Window has been destroyed.");
            }
            return this._window.instance.cast<Void>();
            return default!;
        }
    }
    public virtual Pointer<Void> flutterViewHandle
    {
        get
        {
            if (this._destroyed)
            {
                throw new InvalidOperationException("Window has been destroyed.");
            }
            return this._view.instance.cast<Void>();
            return default!;
        }
    }
}

public class PopupWindowControllerLinuxIo : PopupWindowControllerIo, BaseWindowControllerLinuxIo
{
    internal virtual WindowingOwnerLinuxIo _owner { get; private set; } = default!;
    internal virtual PopupWindowControllerDelegateIo _delegate { get; private set; } = default!;
    internal virtual _GtkWindow___window_linux _window { get; private set; } = default!;
    internal virtual Rect _anchorRect { get; set; } = default!;
    internal virtual WindowPositionerIo _positioner { get; set; } = default!;
    internal virtual BaseWindowControllerIo _parent { get; private set; } = default!;
    internal virtual _FlView___window_linux _view { get; private set; } = default!;
    internal virtual _FlViewMonitor___window_linux _viewMonitor { get; private set; } = default!;
    internal virtual _FlWindowMonitor___window_linux _windowMonitor { get; private set; } = default!;
    internal virtual Offset? _offsetFromParent { get; set; } = default;
    internal virtual bool _destroyed { get; set; } = false;

    public PopupWindowControllerLinuxIo(WindowingOwnerLinuxIo owner, PopupWindowControllerDelegateIo @delegate, global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints, Rect anchorRect, WindowPositionerIo positioner, BaseWindowControllerIo parent)
    {
        this._owner = owner;
        this._delegate = @delegate;
        this._parent = parent;
        this._window = new _GtkWindow___window_linux(_GtkWindowType___window_linux.popup);
    }

    public override bool isDestroyed => this._destroyed;
    public override Size contentSize => this._window.getSize();
    public override void destroy()
    {
        if (this._destroyed)
        {
            return;
        }
        this._viewMonitor.close();
        this._viewMonitor.unref();
        this._window.destroy();
        this._windowMonitor.close();
        this._windowMonitor.unref();
        _destroyed = true;
        ((WindowingOwnerLinuxIo)this._owner).registrar.unregister(checked((long)this.rootView.viewId));
        notifyListeners();
    }

    public override void updatePosition(Rect? anchorRect = null, WindowPositionerIo? positioner = null)
    {
        if ((anchorRect is not null))
        {
            Rect anchorRect__value29691 = DartRuntimePrimitives.RequireValue(anchorRect);
            _anchorRect = DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(anchorRect__value29691));
        }
        if ((positioner is not null))
        {
            _positioner = positioner;
        }
        _GtkWindow___window_linux? parentWindow__29842 = ((_GtkWindow___window_linux?)(object?)((WindowingOwnerLinuxIo)this._owner).registrar._windowForViewId(checked((long)((BaseWindowControllerIo)this._parent).rootView.viewId)));
        _FlView___window_linux? view__29936 = ((_FlView___window_linux?)(object?)((WindowingOwnerLinuxIo)this._owner).registrar._viewForViewId(checked((long)((BaseWindowControllerIo)this._parent).rootView.viewId)));
        var offset__30009 = (0L, 0L);
        if (((parentWindow__29842 is not null) && (view__29936 is not null)))
        {
            offset__30009 = (view__29936.translateCoordinates(parentWindow__29842, (0L, 0L)) ?? (0L, 0L));
        }
        this._window.getWindow().moveToRect(x: (this._anchorRect.left.toInt() + offset__30009.Item1), y: (this._anchorRect.top.toInt() + offset__30009.Item2), width: ((this._anchorRect.right - this._anchorRect.left)).toInt(), height: ((this._anchorRect.bottom - this._anchorRect.top)).toInt(), rectAnchor: _anchorToGravity(((WindowPositionerIo)this._positioner).parentAnchor), windowAnchor: _anchorToGravity(((WindowPositionerIo)this._positioner).childAnchor), anchorHints: _constraintAdjustmentToHints(((WindowPositionerIo)this._positioner).constraintAdjustment), rectAnchorDx: ((WindowPositionerIo)this._positioner).offset.dx.toInt(), rectAnchorDy: ((WindowPositionerIo)this._positioner).offset.dy.toInt());
    }

    public override Offset offsetFromParent
    {
        get
        {
            return (this._offsetFromParent ?? Offset.zero);
            return default!;
        }
    }
    internal virtual _GdkGravity___window_linux _anchorToGravity(WindowPositionerAnchorIo anchor)
    {
        return (anchor switch { WindowPositionerAnchorIo.center => _GdkGravity___window_linux.center, WindowPositionerAnchorIo.top => _GdkGravity___window_linux.north, WindowPositionerAnchorIo.bottom => _GdkGravity___window_linux.south, WindowPositionerAnchorIo.left => _GdkGravity___window_linux.west, WindowPositionerAnchorIo.right => _GdkGravity___window_linux.east, WindowPositionerAnchorIo.topLeft => _GdkGravity___window_linux.northWest, WindowPositionerAnchorIo.bottomLeft => _GdkGravity___window_linux.southWest, WindowPositionerAnchorIo.topRight => _GdkGravity___window_linux.northEast, WindowPositionerAnchorIo.bottomRight => _GdkGravity___window_linux.southEast, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual HashSet<_GdkAnchorHint___window_linux> _constraintAdjustmentToHints(WindowPositionerConstraintAdjustmentIo adjustment)
    {
        return new HashSet<_GdkAnchorHint___window_linux>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BaseWindowControllerIo parent => this._parent;
    public override void setConstraints(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints)
    {
        this._window.setGeometryHints(minWidth: ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).minWidth.toInt(), minHeight: ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).minHeight.toInt(), maxWidth: (double.IsInfinity(((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxWidth) ? 2147483647L : ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxWidth.toInt()), maxHeight: (double.IsInfinity(((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxHeight) ? 2147483647L : ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxHeight.toInt()));
    }

    public virtual Pointer<Void> windowHandle
    {
        get
        {
            if (this._destroyed)
            {
                throw new InvalidOperationException("Window has been destroyed.");
            }
            return this._window.instance.cast<Void>();
            return default!;
        }
    }
    public virtual Pointer<Void> flutterViewHandle
    {
        get
        {
            if (this._destroyed)
            {
                throw new InvalidOperationException("Window has been destroyed.");
            }
            return this._view.instance.cast<Void>();
            return default!;
        }
    }
}

public enum _GtkWindowType___window_linux
{
    toplevel,
    popup
}

public enum _GdkWindowState___window_linux
{
    withdrawn,
    iconified,
    maximized,
    sticky,
    fullscreen,
    above,
    below,
    focused,
    tiled,
    topTiled,
    topResizable,
    rightTiled,
    rightResizable,
    bottomTiled,
    bottomResizable,
    leftTiled,
    leftResizable
}

public enum _GdkWindowTypeHint___window_linux
{
    normal,
    dialog,
    menu,
    toolbar,
    splashscreen,
    utility,
    dock,
    desktop,
    dropdown_menu,
    popup_menu,
    tooltip,
    notification,
    combo,
    dnd
}

public enum _GdkGravity___window_linux
{
    none,
    northWest,
    north,
    northEast,
    west,
    center,
    east,
    southWest,
    south,
    southEast,
    static_
}

public enum _GdkAnchorHint___window_linux
{
    flipX,
    flipY,
    slideX,
    slideY,
    resizeX,
    resizeY
}

public static partial class _window_linuxLibrary
{
    internal static Pointer<Uint8> _stringToNative(string value)
    {
        Uint8List units__35618 = global::Doroti.Runtime.Dart_convertLibrary.utf8.encode(value);
        Pointer<Uint8> buffer__35677 = _gMalloc0((units__35618.Count + 1L)).cast<Uint8>();
        Uint8List nativeString__35751 = buffer__35677.asTypedList((units__35618.Count + 1L));
        nativeString__35751.setAll(0L, units__35618);
        nativeString__35751[units__35618.Count] = 0L;
        return buffer__35677;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class _window_linuxLibrary
{
    internal static string _nativeToString(Pointer<Uint8> value)
    {
        var length__35952 = 0L;
        while ((value[length__35952] != 0L))
        {
            length__35952++;
        }
        return global::Doroti.Runtime.Dart_convertLibrary.utf8.decode(value.asTypedList(length__35952));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _GObject___window_linux
{
    public virtual Pointer<NativeType> instance { get; private set; } = default!;

    internal _GObject___window_linux(Pointer<NativeType> instance)
    {
        this.instance = instance;
    }

    public virtual void unref()
    {
        _GObject___window_linux._unref(this.instance);
    }

    internal abstract static void _unref(Pointer<NativeType> widget);
}

internal class _GtkContainer___window_linux : _GtkWidget___window_linux
{
    internal _GtkContainer___window_linux(Pointer<NativeType> instance) : base(instance)
    {
    }

    public virtual void add(_GtkWidget___window_linux child)
    {
        _GtkContainer___window_linux._gtkContainerAdd(this.instance, child.instance);
    }

    internal abstract static void _gtkContainerAdd(Pointer<NativeType> container, Pointer<NativeType> child);
}

public class _GtkWidget___window_linux : _GObject___window_linux
{
    internal _GtkWidget___window_linux(Pointer<NativeType> instance) : base(instance)
    {
    }

    public virtual void realize()
    {
        _GtkWidget___window_linux._gtkWidgetRealize(this.instance);
    }

    public virtual void show()
    {
        _GtkWidget___window_linux._gtkWidgetShow(this.instance);
    }

    public virtual _GdkWindow___window_linux getWindow()
    {
        return new _GdkWindow___window_linux(_GtkWidget___window_linux._gtkWidgetGetWindow(this.instance));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long getScaleFactor()
    {
        return _GtkWidget___window_linux._gtkWidgetGetScaleFactor(this.instance);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual (long, long)? translateCoordinates(_GtkWidget___window_linux destWidget, (long, long) src)
    {
        Pointer<Int> dest__37987 = _gMalloc0((Dart_ffiLibrary.sizeOf<Int>() * 2L)).cast<Int>();
        bool translated__38063 = _GtkWidget___window_linux._gtkWidgetTranslateCoordinates(this.instance, destWidget.instance, src.Item1, src.Item2, dest__37987.elementAt(0L), dest__37987.elementAt(1L));
        (long, long)? result__38258 = (translated__38063 ? (dest__37987[0L], dest__37987[1L]) : null);
        _gFree(dest__37987);
        return result__38258;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void destroy()
    {
        _GtkWidget___window_linux._gtkWindowDestroy(this.instance);
    }

    internal abstract static void _gtkWidgetRealize(Pointer<NativeType> widget);
    internal abstract static void _gtkWidgetShow(Pointer<NativeType> widget);
    internal abstract static Pointer<NativeType> _gtkWidgetGetWindow(Pointer<NativeType> widget);
    internal abstract static void _gtkWindowDestroy(Pointer<NativeType> widget);
    internal abstract static long _gtkWidgetGetScaleFactor(Pointer<NativeType> widget);
    internal abstract static bool _gtkWidgetTranslateCoordinates(Pointer<NativeType> widget, Pointer<NativeType> destWidget, long srcX, long srcY, Pointer<Int> destX, Pointer<Int> destY);
}

public class _GdkWindow___window_linux : _GObject___window_linux
{
    internal _GdkWindow___window_linux(Pointer<NativeType> instance) : base(instance)
    {
    }

    public virtual HashSet<_GdkWindowState___window_linux> getState()
    {
        long stateBits__40086 = _GdkWindow___window_linux._gdkWindowGetState(this.instance);
        var states__40138 = new HashSet<_GdkWindowState___window_linux>();
        foreach (_GdkWindowState___window_linux state__40199 in System.Enum.GetValues<_GdkWindowState___window_linux>().ToList())
        {
            if ((((stateBits__40086 & ((1L << (int)(FoundationRuntimePorts.EnumIndex(state__40199)))))) != 0L))
            {
                states__40138.Add(state__40199);
            }
        }
        return states__40138;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void moveToRect(long x, long y, long width, long height, _GdkGravity___window_linux rectAnchor, _GdkGravity___window_linux windowAnchor, HashSet<_GdkAnchorHint___window_linux> anchorHints, long rectAnchorDx = 0, long rectAnchorDy = 0)
    {
        Pointer<_GdkRectangle___window_linux> rect__40780 = _gMalloc0(Dart_ffiLibrary.sizeOf<_GdkRectangle___window_linux>()).cast<_GdkRectangle___window_linux>();
        _GdkRectangle___window_linux r__40886 = rect__40780.@ref;
        r__40886.x = x;
        r__40886.y = y;
        r__40886.width = width;
        r__40886.height = height;
        var anchorHintsBits__40978 = 0L;
        foreach (var anchor__41014 in anchorHints)
        {
            anchorHintsBits__40978 |= (1L << (int)(FoundationRuntimePorts.EnumIndex(anchor__41014)));
        }
        _GdkWindow___window_linux._gdkWindowMoveToRect(this.instance, rect__40780, FoundationRuntimePorts.EnumIndex(rectAnchor), FoundationRuntimePorts.EnumIndex(windowAnchor), anchorHintsBits__40978, rectAnchorDx, rectAnchorDy);
        _gFree(rect__40780);
    }

    internal abstract static long _gdkWindowGetState(Pointer<NativeType> window);
    internal abstract static void _gdkWindowMoveToRect(Pointer<NativeType> window, Pointer<NativeType> rect, long rectAnchor, long windowAnchor, long anchorHints, long rectAnchorDx, long rectAnchorDy);
}

internal class _GdkRectangle___window_linux : Struct
{
    public virtual long x { get; set; } = default!;
    public virtual long y { get; set; } = default!;
    public virtual long width { get; set; } = default!;
    public virtual long height { get; set; } = default!;

}

internal class _GdkGeometry___window_linux : Struct
{
    public virtual long minWidth { get; set; } = default!;
    public virtual long minHeight { get; set; } = default!;
    public virtual long maxWidth { get; set; } = default!;
    public virtual long maxHeight { get; set; } = default!;
    public virtual long baseWidth { get; set; } = default!;
    public virtual long baseHeight { get; set; } = default!;
    public virtual long widthInc { get; set; } = default!;
    public virtual long heightInc { get; set; } = default!;
    public virtual double minAspect { get; set; } = default!;
    public virtual double maxAspect { get; set; } = default!;
    public virtual long winGravity { get; set; } = default!;

    internal static _GdkGeometry___window_linux Create()
    {
        return Dart_ffiLibrary.create();
    }

}

public class _GtkWindow___window_linux : _GtkContainer___window_linux
{
    internal _GtkWindow___window_linux(_GtkWindowType___window_linux type) : base(_GtkWindow___window_linux._gtkWindowNew(FoundationRuntimePorts.EnumIndex(type)))
    {
    }

    internal static _GtkWindow___window_linux CreateFromHandle(Pointer<Void> handle)
    {
        var __instance = new _GtkWindow___window_linux(default!);
        return __instance;
    }

    public virtual void present()
    {
        _GtkWindow___window_linux._gtkWindowPresent(this.instance);
    }

    public virtual void setTransientFor(_GtkWindow___window_linux parent)
    {
        _GtkWindow___window_linux._gtkWindowSetTransientFor(this.instance, parent.instance);
    }

    public virtual void setModal(bool modal)
    {
        _GtkWindow___window_linux._gtkWindowSetModal(this.instance, modal);
    }

    public virtual void setTypeHint(_GdkWindowTypeHint___window_linux hint)
    {
        _GtkWindow___window_linux._gtkWindowSetTypeHint(this.instance, FoundationRuntimePorts.EnumIndex(hint));
    }

    public virtual void setDecorated(bool decorated)
    {
        _GtkWindow___window_linux._gtkWindowSetDecorated(this.instance, decorated);
    }

    public virtual void setTitle(string title)
    {
        Pointer<Uint8> titleBuffer__43779 = _window_linuxLibrary._stringToNative(title);
        _GtkWindow___window_linux._gtkWindowSetTitle(this.instance, titleBuffer__43779);
        _gFree(titleBuffer__43779);
    }

    public virtual string getTitle()
    {
        return _window_linuxLibrary._nativeToString(_GtkWindow___window_linux._gtkWindowGetTitle(this.instance));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void setDefaultSize(long width, long height)
    {
        _GtkWindow___window_linux._gtkWindowSetDefaultSize(this.instance, width, height);
    }

    public virtual void setGeometryHints(long? minWidth = null, long? minHeight = null, long? maxWidth = null, long? maxHeight = null)
    {
        Pointer<_GdkGeometry___window_linux> geometry__44348 = _gMalloc0(Dart_ffiLibrary.sizeOf<_GdkGeometry___window_linux>()).cast<_GdkGeometry___window_linux>();
        _GdkGeometry___window_linux g__44455 = geometry__44348.@ref;
        var geometryMask__44481 = 0L;
        if (((minWidth is not null) || (minHeight is not null)))
        {
            g__44455.minWidth = (minWidth ?? 0L);
            g__44455.minHeight = (minHeight ?? 0L);
            geometryMask__44481 |= 2L;
        }
        if (((maxWidth is not null) || (maxHeight is not null)))
        {
            g__44455.maxWidth = (maxWidth ?? _window_linuxLibrary._kMaxWindowDimensions);
            g__44455.maxHeight = (maxHeight ?? _window_linuxLibrary._kMaxWindowDimensions);
            geometryMask__44481 |= 4L;
        }
        _GtkWindow___window_linux._gtkWindowSetGeometryHints(this.instance, Dart_ffiLibrary.nullptr, geometry__44348, geometryMask__44481);
        _gFree(geometry__44348);
    }

    public virtual void resize(long width, long height)
    {
        _GtkWindow___window_linux._gtkWindowResize(this.instance, width, height);
    }

    public virtual void maximize()
    {
        _GtkWindow___window_linux._gtkWindowMaximize(this.instance);
    }

    public virtual void unmaximize()
    {
        _GtkWindow___window_linux._gtkWindowUnmaximize(this.instance);
    }

    public virtual void iconify()
    {
        _GtkWindow___window_linux._gtkWindowIconify(this.instance);
    }

    public virtual void deiconify()
    {
        _GtkWindow___window_linux._gtkWindowDeiconify(this.instance);
    }

    public virtual void fullscreen()
    {
        _GtkWindow___window_linux._gtkWindowFullscreen(this.instance);
    }

    public virtual void unfullscreen()
    {
        _GtkWindow___window_linux._gtkWindowUnfullscreen(this.instance);
    }

    public virtual global::Doroti.Ui.Size getSize()
    {
        Pointer<Int> size__45744 = _gMalloc0((Dart_ffiLibrary.sizeOf<Int>() * 2L)).cast<Int>();
        _GtkWindow___window_linux._gtkWindowGetSize(this.instance, size__45744.elementAt(0L), size__45744.elementAt(1L));
        var result__45886 = new global::Doroti.Ui.Size(size__45744[0L].toDouble(), size__45744[1L].toDouble());
        _gFree(size__45744);
        return result__45886;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool isActive()
    {
        return _GtkWindow___window_linux._gtkWindowIsActive(this.instance);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal abstract static Pointer<NativeType> _gtkWindowNew(long type);
    internal abstract static void _gtkWindowPresent(Pointer<NativeType> window);
    internal abstract static void _gtkWindowSetModal(Pointer<NativeType> window, bool modal);
    internal abstract static void _gtkWindowSetTypeHint(Pointer<NativeType> window, long hint);
    internal abstract static void _gtkWindowSetTransientFor(Pointer<NativeType> window, Pointer<NativeType> parent);
    internal abstract static void _gtkWindowSetTitle(Pointer<NativeType> window, Pointer<Uint8> title);
    internal abstract static void _gtkWindowSetDecorated(Pointer<NativeType> window, bool decorated);
    internal abstract static Pointer<Uint8> _gtkWindowGetTitle(Pointer<NativeType> window);
    internal abstract static void _gtkWindowSetDefaultSize(Pointer<NativeType> window, long width, long height);
    internal abstract static void _gtkWindowSetGeometryHints(Pointer<NativeType> window, Pointer<NativeType> geometryWidget, Pointer<_GdkGeometry___window_linux> geometry, long geometryMask);
    internal abstract static void _gtkWindowResize(Pointer<NativeType> window, long width, long height);
    internal abstract static void _gtkWindowMaximize(Pointer<NativeType> window);
    internal abstract static void _gtkWindowUnmaximize(Pointer<NativeType> window);
    internal abstract static void _gtkWindowIconify(Pointer<NativeType> window);
    internal abstract static void _gtkWindowDeiconify(Pointer<NativeType> window);
    internal abstract static void _gtkWindowFullscreen(Pointer<NativeType> window);
    internal abstract static void _gtkWindowUnfullscreen(Pointer<NativeType> window);
    internal abstract static void _gtkWindowGetSize(Pointer<NativeType> window, Pointer<Int> width, Pointer<Int> height);
    internal abstract static bool _gtkWindowIsActive(Pointer<NativeType> widget);
}

public class _FlEngine___window_linux : _GObject___window_linux
{
    internal _FlEngine___window_linux(long engineId) : base(new Pointer<NativeType>(engineId))
    {
    }

    internal static _FlEngine___window_linux CreateCurrent() => new _FlEngine___window_linux(DartRuntimePrimitives.RequireValue(WidgetsBinding.instance.platformDispatcher.engineId));

}

public class _FlView___window_linux : _GtkWidget___window_linux
{
    internal _FlView___window_linux(_FlEngine___window_linux engine, bool isSizedToContent = false) : base((isSizedToContent ? _FlView___window_linux._flViewNewSizedToContent(engine.instance) : _FlView___window_linux._flViewNewForEngine(engine.instance)))
    {
    }

    internal static _FlView___window_linux CreateFromHandle(Pointer<Void> handle)
    {
        var __instance = new _FlView___window_linux(default!, default!);
        return __instance;
    }

    public virtual long getId()
    {
        return _FlView___window_linux._flViewGetId(this.instance);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal abstract static Pointer<NativeType> _flViewNewForEngine(Pointer<NativeType> engine);
    internal abstract static Pointer<NativeType> _flViewNewSizedToContent(Pointer<NativeType> engine);
    internal abstract static long _flViewGetId(Pointer<NativeType> view);
}

internal class _FlViewMonitor___window_linux : _GObject___window_linux
{
    internal virtual NativeCallable<global::System.Func<Void>> _onFirstFrameFunction { get; private set; } = default!;

    internal static _FlViewMonitor___window_linux Create(_FlView___window_linux view, global::System.Action? onFirstFrame = null)
    {
        void noop()
        {
        }
        return new _FlViewMonitor___window_linux(view.instance, new NativeCallable<global::System.Func<Void>>(((onFirstFrame ?? (global::System.Action)noop))));
    }

    internal _FlViewMonitor___window_linux(Pointer<NativeType> view, NativeCallable<global::System.Func<Void>> _onFirstFrameFunction) : base(_FlViewMonitor___window_linux._flViewMonitorNew(view, (Pointer<NativeFunction<global::System.Func<Void>>>)_onFirstFrameFunction.nativeFunction))
    {
        this._onFirstFrameFunction = _onFirstFrameFunction;
    }

    public virtual void close()
    {
        this._onFirstFrameFunction.close();
    }

    internal abstract static Pointer<NativeType> _flViewMonitorNew(Pointer<NativeType> view, Pointer<NativeFunction<global::System.Func<Void>>> onFirstFrame);
}

internal class _FlWindowMonitor___window_linux : _GObject___window_linux
{
    internal virtual NativeCallable<global::System.Func<Void>> _onConfigureFunction { get; private set; } = default!;
    internal virtual NativeCallable<global::System.Func<Void>> _onStateChangedFunction { get; private set; } = default!;
    internal virtual NativeCallable<global::System.Func<Void>> _onIsActiveNotifyFunction { get; private set; } = default!;
    internal virtual NativeCallable<global::System.Func<Void>> _onTitleNotifyFunction { get; private set; } = default!;
    internal virtual NativeCallable<global::System.Func<Int, Int, Int, Int, Void>> _onMovedToRectFunction { get; private set; } = default!;
    internal virtual NativeCallable<global::System.Func<Void>> _onCloseFunction { get; private set; } = default!;
    internal virtual NativeCallable<global::System.Func<Void>> _onDestroyFunction { get; private set; } = default!;

    internal static _FlWindowMonitor___window_linux Create(_GtkWindow___window_linux window, global::System.Action? onConfigure = null, global::System.Action? onStateChanged = null, global::System.Action? onIsActiveNotify = null, global::System.Action? onTitleNotify = null, global::System.Action<long, long, long, long>? onMovedToRect = null, global::System.Action? onClose = null, global::System.Action? onDestroy = null)
    {
        void noop()
        {
        }
        void noopMovedToRect(long x, long y, long width, long height)
        {
        }
        return new _FlWindowMonitor___window_linux(window.instance, new NativeCallable<global::System.Func<Void>>(((onConfigure ?? (global::System.Action)noop))), new NativeCallable<global::System.Func<Void>>(((onStateChanged ?? (global::System.Action)noop))), new NativeCallable<global::System.Func<Void>>(((onIsActiveNotify ?? (global::System.Action)noop))), new NativeCallable<global::System.Func<Void>>(((onTitleNotify ?? (global::System.Action)noop))), new NativeCallable<global::System.Func<Int, Int, Int, Int, Void>>(((onMovedToRect ?? (global::System.Action<long, long, long, long>)noopMovedToRect))), new NativeCallable<global::System.Func<Void>>(((onClose ?? (global::System.Action)noop))), new NativeCallable<global::System.Func<Void>>(((onDestroy ?? (global::System.Action)noop))));
    }

    internal _FlWindowMonitor___window_linux(Pointer<NativeType> window, NativeCallable<global::System.Func<Void>> _onConfigureFunction, NativeCallable<global::System.Func<Void>> _onStateChangedFunction, NativeCallable<global::System.Func<Void>> _onIsActiveNotifyFunction, NativeCallable<global::System.Func<Void>> _onTitleNotifyFunction, NativeCallable<global::System.Func<Int, Int, Int, Int, Void>> _onMovedToRectFunction, NativeCallable<global::System.Func<Void>> _onCloseFunction, NativeCallable<global::System.Func<Void>> _onDestroyFunction) : base(_FlWindowMonitor___window_linux._flWindowMonitorNew(window, (Pointer<NativeFunction<global::System.Func<Void>>>)_onConfigureFunction.nativeFunction, (Pointer<NativeFunction<global::System.Func<Void>>>)_onStateChangedFunction.nativeFunction, (Pointer<NativeFunction<global::System.Func<Void>>>)_onIsActiveNotifyFunction.nativeFunction, (Pointer<NativeFunction<global::System.Func<Void>>>)_onTitleNotifyFunction.nativeFunction, (Pointer<NativeFunction<global::System.Func<Int, Int, Int, Int, Void>>>)_onMovedToRectFunction.nativeFunction, (Pointer<NativeFunction<global::System.Func<Void>>>)_onCloseFunction.nativeFunction, (Pointer<NativeFunction<global::System.Func<Void>>>)_onDestroyFunction.nativeFunction))
    {
        this._onConfigureFunction = _onConfigureFunction;
        this._onStateChangedFunction = _onStateChangedFunction;
        this._onIsActiveNotifyFunction = _onIsActiveNotifyFunction;
        this._onTitleNotifyFunction = _onTitleNotifyFunction;
        this._onMovedToRectFunction = _onMovedToRectFunction;
        this._onCloseFunction = _onCloseFunction;
        this._onDestroyFunction = _onDestroyFunction;
    }

    public virtual void close()
    {
        this._onConfigureFunction.close();
        this._onStateChangedFunction.close();
        this._onIsActiveNotifyFunction.close();
        this._onTitleNotifyFunction.close();
        this._onMovedToRectFunction.close();
        this._onCloseFunction.close();
        this._onDestroyFunction.close();
    }

    internal abstract static Pointer<NativeType> _flWindowMonitorNew(Pointer<NativeType> window, Pointer<NativeFunction<global::System.Func<Void>>> onConfigure, Pointer<NativeFunction<global::System.Func<Void>>> onStateChanged, Pointer<NativeFunction<global::System.Func<Void>>> onIsActiveNotify, Pointer<NativeFunction<global::System.Func<Void>>> onTitleNotify, Pointer<NativeFunction<global::System.Func<Int, Int, Int, Int, Void>>> onMovedToRect, Pointer<NativeFunction<global::System.Func<Void>>> onClose, Pointer<NativeFunction<global::System.Func<Void>>> onDestroy);
}

