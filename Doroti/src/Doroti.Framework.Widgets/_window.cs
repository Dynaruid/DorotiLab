// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/_window.dart
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

public static partial class _windowLibrary
{
    internal static string _kWindowingDisabledErrorMessage = "Windowing APIs are not enabled.\n\nWindowing APIs are currently experimental. Do not use windowing APIs in\nproduction applications or plugins published to pub.dev.\n\nTo try experimental windowing APIs:\n1. Switch to Flutter's main release channel.\n2. Turn on the windowing feature flag.\n\nSee: https://github.com/flutter/flutter/issues/30701.\n";
}

public abstract class BaseWindowControllerIo : global::Doroti.Generated.Framework.Foundation.ChangeNotifier
{
    internal virtual DorotiView _view { get; private set; } = default!;

    public abstract global::Doroti.Ui.Size contentSize { get; }
    public abstract void destroy();
    public virtual global::Doroti.Ui.DorotiView rootView
    {
        get => this._view;
        set
        {
            var view = (DorotiView)(object)value;
            _view = view;
        }
    }
    public abstract bool isDestroyed { get; }
}

public class WindowControllerDelegateIo
{
    public virtual void onWindowCloseRequested(WindowControllerIo controller)
    {
        if (!global::Doroti.Generated.Framework.Foundation._featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
        controller.destroy();
    }

    public virtual void onWindowDestroyed()
    {
        if (!global::Doroti.Generated.Framework.Foundation._featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
    }

}

public abstract class WindowControllerIo : BaseWindowControllerIo
{
    public static WindowControllerIo Create(Size size, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, string? title = null, WindowControllerDelegateIo? @delegate = null)
    {
        if (!global::Doroti.Generated.Framework.Foundation._featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
        if ((constraints is not null))
        {
            DartRuntimePrimitives.Assert(() => constraints.isSatisfiedBy(size));
        }
        WindowingOwnerIo owner__8133 = WidgetsBinding.instance.windowingOwner;
        return ((WindowControllerIo)(object?)owner__8133.createWindowController(@delegate: (@delegate ?? new WindowControllerDelegateIo()), size: size, constraints: constraints, title: title, resizable: true));
    }

    public static WindowControllerIo CreateShrinkWrap(bool resizable = false, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, string? title = null, WindowControllerDelegateIo? @delegate = null)
    {
        if (!global::Doroti.Generated.Framework.Foundation._featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
        WidgetsFlutterBinding.ensureInitialized();
        WindowingOwnerIo owner__10233 = WidgetsBinding.instance.windowingOwner;
        return ((WindowControllerIo)(object?)owner__10233.createWindowController(@delegate: (@delegate ?? new WindowControllerDelegateIo()), constraints: constraints, resizable: resizable, title: title));
    }

    protected WindowControllerIo()
    {
    }

    public abstract string title { get; }
    public abstract bool isActivated { get; }
    public abstract bool isMaximized { get; }
    public abstract bool isMinimized { get; }
    public abstract bool isFullscreen { get; }
    public abstract void setSize(Size size);
    public abstract void setConstraints(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints);
    public abstract void setTitle(string title);
    public abstract void activate();
    public abstract void setMaximized(bool maximized);
    public abstract void setMinimized(bool minimized);
    public abstract void setFullscreen(bool fullscreen, Display? display = null);
}

public class DialogWindowControllerDelegateIo
{
    public virtual void onWindowCloseRequested(DialogWindowControllerIo controller)
    {
        if (!global::Doroti.Generated.Framework.Foundation._featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
        controller.destroy();
    }

    public virtual void onWindowDestroyed()
    {
        if (!global::Doroti.Generated.Framework.Foundation._featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
    }

}

public abstract class DialogWindowControllerIo : BaseWindowControllerIo
{
    public static DialogWindowControllerIo Create(Size size, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, BaseWindowControllerIo? parent = null, string? title = null, DialogWindowControllerDelegateIo? @delegate = null)
    {
        if (!global::Doroti.Generated.Framework.Foundation._featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
        WidgetsFlutterBinding.ensureInitialized();
        if ((constraints is not null))
        {
            DartRuntimePrimitives.Assert(() => constraints.isSatisfiedBy(size));
        }
        WindowingOwnerIo owner__20060 = WidgetsBinding.instance.windowingOwner;
        return ((DialogWindowControllerIo)(object?)owner__20060.createDialogWindowController(@delegate: (@delegate ?? new DialogWindowControllerDelegateIo()), size: size, constraints: constraints, title: title, parent: parent, resizable: true));
    }

    public static DialogWindowControllerIo CreateShrinkWrap(bool resizable = false, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, BaseWindowControllerIo? parent = null, string? title = null, DialogWindowControllerDelegateIo? @delegate = null)
    {
        if (!global::Doroti.Generated.Framework.Foundation._featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
        WidgetsFlutterBinding.ensureInitialized();
        WindowingOwnerIo owner__21173 = WidgetsBinding.instance.windowingOwner;
        return ((DialogWindowControllerIo)(object?)owner__21173.createDialogWindowController(@delegate: (@delegate ?? new DialogWindowControllerDelegateIo()), constraints: constraints, resizable: resizable, title: title, parent: parent));
    }

    protected DialogWindowControllerIo()
    {
    }

    public abstract BaseWindowControllerIo? parent { get; }
    public abstract string title { get; }
    public abstract bool isActivated { get; }
    public abstract bool isMinimized { get; }
    public abstract void setSize(Size size);
    public abstract void setConstraints(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints);
    public abstract void setTitle(string title);
    public abstract void activate();
    public abstract void setMinimized(bool minimized);
}

public class TooltipWindowControllerDelegateIo
{
    public virtual void onWindowDestroyed()
    {
        if (!global::Doroti.Generated.Framework.Foundation._featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
    }

}

public abstract class TooltipWindowControllerIo : BaseWindowControllerIo
{
    public static TooltipWindowControllerIo Create(BaseWindowControllerIo parent, Rect anchorRect, WindowPositionerIo positioner, global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints = default!, TooltipWindowControllerDelegateIo? @delegate = null)
    {
        global::Doroti.Generated.Framework.Rendering.BoxConstraints __constraints = constraints ?? new global::Doroti.Generated.Framework.Rendering.BoxConstraints();
        if (!global::Doroti.Generated.Framework.Foundation._featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
        WidgetsFlutterBinding.ensureInitialized();
        WindowingOwnerIo owner__27992 = WidgetsBinding.instance.windowingOwner;
        TooltipWindowControllerIo controller__28074 = ((TooltipWindowControllerIo)(object?)owner__27992.createTooltipWindowController(parent: parent, constraints: __constraints, @delegate: (@delegate ?? new TooltipWindowControllerDelegateIo()), anchorRect: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(anchorRect)), positioner: positioner));
        return controller__28074;
    }

    protected TooltipWindowControllerIo()
    {
    }

    public abstract BaseWindowControllerIo parent { get; }
    public abstract void setConstraints(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints);
    public abstract void updatePosition(Rect? anchorRect = null, WindowPositionerIo? positioner = null);
}

public class PopupWindowControllerDelegateIo
{
    public virtual void onWindowDestroyed()
    {
        if (!global::Doroti.Generated.Framework.Foundation._featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
    }

}

public abstract class PopupWindowControllerIo : BaseWindowControllerIo
{
    public static PopupWindowControllerIo Create(BaseWindowControllerIo parent, Rect anchorRect, WindowPositionerIo positioner, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, PopupWindowControllerDelegateIo? @delegate = null)
    {
        if (!global::Doroti.Generated.Framework.Foundation._featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
        WidgetsFlutterBinding.ensureInitialized();
        WindowingOwnerIo owner__33068 = WidgetsBinding.instance.windowingOwner;
        return ((PopupWindowControllerIo)(object?)owner__33068.createPopupWindowController(parent: parent, constraints: (constraints ?? new global::Doroti.Generated.Framework.Rendering.BoxConstraints()), @delegate: (@delegate ?? new PopupWindowControllerDelegateIo()), anchorRect: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(anchorRect)), positioner: positioner));
    }

    protected PopupWindowControllerIo()
    {
    }

    public abstract BaseWindowControllerIo parent { get; }
    public abstract void setConstraints(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints);
    public abstract void updatePosition(Rect? anchorRect = null, WindowPositionerIo? positioner = null);
    public abstract global::Doroti.Ui.Offset offsetFromParent { get; }
    public virtual void activate()
    {
        BaseWindowControllerIo parent__35385 = this.parent;
        while (true)
        {
            if ((parent__35385 is WindowControllerIo))
            {
                WindowControllerIo parent__35385__as35436 = (WindowControllerIo)parent__35385;
                ((WindowControllerIo)parent__35385__as35436).activate();
                break;
            }
            else
            {
                if ((parent__35385 is DialogWindowControllerIo))
                {
                    DialogWindowControllerIo parent__35385__as35525 = (DialogWindowControllerIo)parent__35385;
                    ((DialogWindowControllerIo)parent__35385__as35525).activate();
                    break;
                }
                else
                {
                    if ((parent__35385 is PopupWindowControllerIo))
                    {
                        PopupWindowControllerIo parent__35385__as35620 = (PopupWindowControllerIo)parent__35385;
                        parent__35385 = ((PopupWindowControllerIo)((PopupWindowControllerIo)parent__35385__as35620)).parent;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unexpected controller in hierarchy {parent__35385}");
                    }
                }
            }
        }
    }

    public virtual bool isActivated
    {
        get
        {
            BaseWindowControllerIo parent__35986 = this.parent;
            while (true)
            {
                if ((parent__35986 is WindowControllerIo))
                {
                    WindowControllerIo parent__35986__as36037 = (WindowControllerIo)parent__35986;
                    return ((WindowControllerIo)((WindowControllerIo)parent__35986__as36037)).isActivated;
                }
                else
                {
                    if ((parent__35986 is DialogWindowControllerIo))
                    {
                        DialogWindowControllerIo parent__35986__as36119 = (DialogWindowControllerIo)parent__35986;
                        return ((DialogWindowControllerIo)((DialogWindowControllerIo)parent__35986__as36119)).isActivated;
                    }
                    else
                    {
                        if ((parent__35986 is PopupWindowControllerIo))
                        {
                            PopupWindowControllerIo parent__35986__as36207 = (PopupWindowControllerIo)parent__35986;
                            parent__35986 = ((PopupWindowControllerIo)((PopupWindowControllerIo)parent__35986__as36207)).parent;
                        }
                        else
                        {
                            throw new InvalidOperationException($"Unexpected controller in hierarchy {parent__35986}");
                        }
                    }
                }
            }
            return default!;
        }
    }
}

public class SatelliteWindowControllerDelegateIo
{
    public virtual void onWindowCloseRequested(SatelliteWindowControllerIo controller)
    {
        if (!global::Doroti.Generated.Framework.Foundation._featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
        controller.destroy();
    }

    public virtual void onWindowDestroyed()
    {
        if (!global::Doroti.Generated.Framework.Foundation._featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
    }

}

public abstract class SatelliteWindowControllerIo : BaseWindowControllerIo
{
    public static SatelliteWindowControllerIo Create(BaseWindowControllerIo parent, WindowPositionerIo initialPositioner, Rect? initialAnchorRect = null, Size? size = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, string? title = null, SatelliteWindowControllerDelegateIo? @delegate = null)
    {
        if (!global::Doroti.Generated.Framework.Foundation._featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
        if (((size is not null) && (constraints is not null)))
        {
            Size size__value41904 = DartRuntimePrimitives.RequireValue(size);
            DartRuntimePrimitives.Assert(() => constraints.isSatisfiedBy(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(size__value41904))));
        }
        WindowingOwnerIo owner__42022 = WidgetsBinding.instance.windowingOwner;
        return ((SatelliteWindowControllerIo)(object?)owner__42022.createSatelliteWindowController(@delegate: (@delegate ?? new SatelliteWindowControllerDelegateIo()), parent: parent, initialAnchorRect: initialAnchorRect, initialPositioner: initialPositioner, size: size, constraints: constraints, title: title, resizable: true));
    }

    public static SatelliteWindowControllerIo CreateShrinkWrap(BaseWindowControllerIo parent, WindowPositionerIo initialPositioner, Rect? initialAnchorRect = null, bool resizable = false, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, string? title = null, SatelliteWindowControllerDelegateIo? @delegate = null)
    {
        if (!global::Doroti.Generated.Framework.Foundation._featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
        WidgetsFlutterBinding.ensureInitialized();
        WindowingOwnerIo owner__43351 = WidgetsBinding.instance.windowingOwner;
        return ((SatelliteWindowControllerIo)(object?)owner__43351.createSatelliteWindowController(@delegate: (@delegate ?? new SatelliteWindowControllerDelegateIo()), parent: parent, initialAnchorRect: initialAnchorRect, initialPositioner: initialPositioner, constraints: constraints, resizable: resizable, title: title));
    }

    protected SatelliteWindowControllerIo()
    {
    }

    public abstract BaseWindowControllerIo parent { get; }
    public abstract string title { get; }
    public abstract bool isActivated { get; }
    public abstract void setParent(BaseWindowControllerIo parent);
    public abstract void setSize(Size size);
    public abstract void setConstraints(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints);
    public abstract void setTitle(string title);
    public abstract void activate();
}

public interface WindowingOwnerIo
{
    public WindowControllerIo createWindowController(WindowControllerDelegateIo @delegate, Size? size = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, bool resizable = default!, string? title = null);
    public DialogWindowControllerIo createDialogWindowController(DialogWindowControllerDelegateIo @delegate, Size? size = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, bool resizable = default!, BaseWindowControllerIo? parent = null, string? title = null);
    public TooltipWindowControllerIo createTooltipWindowController(TooltipWindowControllerDelegateIo @delegate, global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints, Rect anchorRect, WindowPositionerIo positioner, BaseWindowControllerIo parent);
    public PopupWindowControllerIo createPopupWindowController(PopupWindowControllerDelegateIo @delegate, global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints, Rect anchorRect, WindowPositionerIo positioner, BaseWindowControllerIo parent);
    public SatelliteWindowControllerIo createSatelliteWindowController(SatelliteWindowControllerDelegateIo @delegate, BaseWindowControllerIo parent, WindowPositionerIo initialPositioner, Rect? initialAnchorRect = null, Size? size = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, bool resizable = default!, string? title = null);
}

public static partial class _windowLibrary
{
    public static WindowingOwnerIo createDefaultWindowingOwner()
    {
        if (!global::Doroti.Generated.Framework.Foundation._featuresLibrary.isWindowingEnabled)
        {
            return ((WindowingOwnerIo)(object?)new _WindowingOwnerUnsupported___window(errorMessage: _windowLibrary._kWindowingDisabledErrorMessage));
        }
        WindowingOwnerIo? owner__50301 = _window_ioLibrary.createDefaultOwner();
        if ((owner__50301 is not null))
        {
            return owner__50301;
        }
        return ((WindowingOwnerIo)(object?)new _WindowingOwnerUnsupported___window(errorMessage: "Windowing is unsupported on this platform."));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _WindowingOwnerUnsupported___window : WindowingOwnerIo
{
    public virtual string errorMessage { get; private set; } = default!;

    internal _WindowingOwnerUnsupported___window(string errorMessage)
    {
        this.errorMessage = errorMessage;
    }

    public virtual WindowControllerIo createWindowController(WindowControllerDelegateIo @delegate, Size? size = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, bool resizable = true, string? title = null)
    {
        throw new NotSupportedException(this.errorMessage);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DialogWindowControllerIo createDialogWindowController(DialogWindowControllerDelegateIo @delegate, Size? size = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, bool resizable = true, BaseWindowControllerIo? parent = null, string? title = null)
    {
        throw new NotSupportedException(this.errorMessage);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TooltipWindowControllerIo createTooltipWindowController(TooltipWindowControllerDelegateIo @delegate, global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints, Rect anchorRect, WindowPositionerIo positioner, BaseWindowControllerIo parent)
    {
        throw new NotImplementedException(this.errorMessage);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual PopupWindowControllerIo createPopupWindowController(PopupWindowControllerDelegateIo @delegate, global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints, Rect anchorRect, WindowPositionerIo positioner, BaseWindowControllerIo parent)
    {
        throw new NotImplementedException(this.errorMessage);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SatelliteWindowControllerIo createSatelliteWindowController(SatelliteWindowControllerDelegateIo @delegate, BaseWindowControllerIo parent, WindowPositionerIo initialPositioner, Rect? initialAnchorRect = null, Size? size = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, bool resizable = true, string? title = null)
    {
        throw new NotImplementedException(this.errorMessage);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class WindowIo : StatelessWidget
{
    public virtual WindowControllerIo controller { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public WindowIo(global::Doroti.Generated.Framework.Foundation.Key? key = null, WindowControllerIo controller = default!, Widget child = default!) : base(key: key)
    {
        this.controller = controller;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new ListenableBuilder(listenable: this.controller, builder: ((global::System.Func<BuildContext, Widget?, Widget>)((context, widget) => new WindowScopeIo(controller: this.controller, child: new View(view: this.controller.rootView, child: this.child))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DialogWindowIo : StatelessWidget
{
    public virtual DialogWindowControllerIo controller { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public DialogWindowIo(global::Doroti.Generated.Framework.Foundation.Key? key = null, DialogWindowControllerIo controller = default!, Widget child = default!) : base(key: key)
    {
        this.controller = controller;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new ListenableBuilder(listenable: this.controller, builder: ((global::System.Func<BuildContext, Widget?, Widget>)((context, widget) => new WindowScopeIo(controller: this.controller, child: new View(view: this.controller.rootView, child: this.child))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class TooltipWindowIo : StatelessWidget
{
    public virtual TooltipWindowControllerIo controller { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public TooltipWindowIo(global::Doroti.Generated.Framework.Foundation.Key? key = null, TooltipWindowControllerIo controller = default!, Widget child = default!) : base(key: key)
    {
        this.controller = controller;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new ListenableBuilder(listenable: this.controller, builder: ((global::System.Func<BuildContext, Widget?, Widget>)((context, widget) => new WindowScopeIo(controller: this.controller, child: new View(view: this.controller.rootView, child: this.child))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class PopupWindowIo : StatelessWidget
{
    public virtual PopupWindowControllerIo controller { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public PopupWindowIo(global::Doroti.Generated.Framework.Foundation.Key? key = null, PopupWindowControllerIo controller = default!, Widget child = default!) : base(key: key)
    {
        this.controller = controller;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new ListenableBuilder(listenable: this.controller, builder: ((global::System.Func<BuildContext, Widget?, Widget>)((context, widget) => new WindowScopeIo(controller: this.controller, child: new View(view: this.controller.rootView, child: this.child))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SatelliteWindowIo : StatelessWidget
{
    public virtual SatelliteWindowControllerIo controller { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public SatelliteWindowIo(global::Doroti.Generated.Framework.Foundation.Key? key = null, SatelliteWindowControllerIo controller = default!, Widget child = default!) : base(key: key)
    {
        this.controller = controller;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new ListenableBuilder(listenable: this.controller, builder: ((global::System.Func<BuildContext, Widget?, Widget>)((context, widget) => new WindowScopeIo(controller: this.controller, child: new View(view: this.controller.rootView, child: this.child))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public enum _WindowControllerAspect___window
{
    contentSize,
    title,
    activated,
    maximized,
    minimized,
    fullscreen,
    destroyed
}

public class WindowScopeIo : InheritedModel<_WindowControllerAspect___window>
{
    internal virtual Size _contentSize { get; private set; } = default!;
    internal virtual string _title { get; private set; } = default!;
    internal virtual bool _isActivated { get; private set; } = default!;
    internal virtual bool _isMaximized { get; private set; } = default!;
    internal virtual bool _isMinimized { get; private set; } = default!;
    internal virtual bool _isFullscreen { get; private set; } = default!;
    internal virtual bool _isDestroyed { get; private set; } = default!;
    public virtual BaseWindowControllerIo controller { get; private set; } = default!;

    public WindowScopeIo(global::Doroti.Generated.Framework.Foundation.Key? key = null, BaseWindowControllerIo controller = default!, Widget child = default!) : base(key: key, child: child)
    {
        this.controller = controller;
        this._isDestroyed = ((BaseWindowControllerIo)controller).isDestroyed;
        this._contentSize = (((BaseWindowControllerIo)controller).isDestroyed ? Size.zero : ((BaseWindowControllerIo)controller).contentSize);
        this._title = (((BaseWindowControllerIo)controller).isDestroyed ? "" : WindowScopeIo._titleValue(controller));
        this._isActivated = (!((BaseWindowControllerIo)controller).isDestroyed && WindowScopeIo._isActivatedValue(controller));
        this._isMaximized = (!((BaseWindowControllerIo)controller).isDestroyed && WindowScopeIo._isMaximizedValue(controller));
        this._isMinimized = (!((BaseWindowControllerIo)controller).isDestroyed && WindowScopeIo._isMinimizedValue(controller));
        this._isFullscreen = (!((BaseWindowControllerIo)controller).isDestroyed && WindowScopeIo._isFullscreenValue(controller));
    }

    public static BaseWindowControllerIo of(BuildContext context)
    {
        return ((BaseWindowControllerIo)(object?)WindowScopeIo._of(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static BaseWindowControllerIo? maybeOf(BuildContext context)
    {
        return ((BaseWindowControllerIo?)(object?)WindowScopeIo._maybeOf(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Ui.Size contentSizeOf(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Size>(WindowScopeIo._of(context, _WindowControllerAspect___window.contentSize).contentSize);
    public static global::Doroti.Ui.Size? maybeContentSizeOf(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Size>(WindowScopeIo._maybeOf(context, _WindowControllerAspect___window.contentSize)?.contentSize);
    public static string titleOf(BuildContext context)
    {
        return ((string)(object?)WindowScopeIo._titleValue(WindowScopeIo._of(context, _WindowControllerAspect___window.title)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static string? maybeTitleOf(BuildContext context)
    {
        BaseWindowControllerIo? controller__70382 = ((BaseWindowControllerIo?)(object?)WindowScopeIo._maybeOf(context, _WindowControllerAspect___window.title));
        if ((controller__70382 is null))
        {
            return ((string)(object)null);
        }
        return ((string?)(object?)WindowScopeIo._titleValue(controller__70382));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool isActivatedOf(BuildContext context)
    {
        return WindowScopeIo._isActivatedValue(WindowScopeIo._of(context, _WindowControllerAspect___window.activated));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool? maybeIsActivatedOf(BuildContext context)
    {
        BaseWindowControllerIo? controller__71719 = ((BaseWindowControllerIo?)(object?)WindowScopeIo._maybeOf(context, _WindowControllerAspect___window.activated));
        if ((controller__71719 is null))
        {
            return null;
        }
        return WindowScopeIo._isActivatedValue(controller__71719);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool isMinimizedOf(BuildContext context)
    {
        return WindowScopeIo._isMinimizedValue(WindowScopeIo._of(context, _WindowControllerAspect___window.minimized));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool? maybeIsMinimizedOf(BuildContext context)
    {
        BaseWindowControllerIo? controller__73070 = ((BaseWindowControllerIo?)(object?)WindowScopeIo._maybeOf(context, _WindowControllerAspect___window.minimized));
        if ((controller__73070 is null))
        {
            return null;
        }
        return WindowScopeIo._isMinimizedValue(controller__73070);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool isMaximizedOf(BuildContext context)
    {
        return WindowScopeIo._isMaximizedValue(WindowScopeIo._of(context, _WindowControllerAspect___window.maximized));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool? maybeIsMaximizedOf(BuildContext context)
    {
        BaseWindowControllerIo? controller__74421 = ((BaseWindowControllerIo?)(object?)WindowScopeIo._maybeOf(context, _WindowControllerAspect___window.maximized));
        if ((controller__74421 is null))
        {
            return null;
        }
        return WindowScopeIo._isMaximizedValue(controller__74421);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool isFullscreenOf(BuildContext context)
    {
        return WindowScopeIo._isFullscreenValue(WindowScopeIo._of(context, _WindowControllerAspect___window.fullscreen));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool? maybeIsFullscreenOf(BuildContext context)
    {
        BaseWindowControllerIo? controller__75774 = ((BaseWindowControllerIo?)(object?)WindowScopeIo._maybeOf(context, _WindowControllerAspect___window.fullscreen));
        if ((controller__75774 is null))
        {
            return null;
        }
        return WindowScopeIo._isFullscreenValue(controller__75774);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool isDestroyedOf(BuildContext context)
    {
        return WindowScopeIo._of(context, _WindowControllerAspect___window.destroyed).isDestroyed;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool? maybeIsDestroyedOf(BuildContext context)
    {
        return WindowScopeIo._maybeOf(context, _WindowControllerAspect___window.destroyed)?.isDestroyed;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static string _titleValue(BaseWindowControllerIo controller) => (controller switch { WindowControllerIo __object77339 => ((WindowControllerIo)((WindowControllerIo)__object77339)).title, DialogWindowControllerIo __object77383 => ((DialogWindowControllerIo)((DialogWindowControllerIo)__object77383)).title, TooltipWindowControllerIo __object77433 => "", PopupWindowControllerIo __object77470 => "", SatelliteWindowControllerIo __object77505 => ((SatelliteWindowControllerIo)((SatelliteWindowControllerIo)__object77505)).title, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    internal static bool _isActivatedValue(BaseWindowControllerIo controller) => (controller switch { WindowControllerIo __object77818 => ((WindowControllerIo)((WindowControllerIo)__object77818)).isActivated, DialogWindowControllerIo __object77868 => ((DialogWindowControllerIo)((DialogWindowControllerIo)__object77868)).isActivated, TooltipWindowControllerIo __object77924 => false, PopupWindowControllerIo __object77964 => ((PopupWindowControllerIo)((PopupWindowControllerIo)__object77964)).isActivated, SatelliteWindowControllerIo __object78019 => ((SatelliteWindowControllerIo)((SatelliteWindowControllerIo)__object78019)).isActivated, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    internal static bool _isMaximizedValue(BaseWindowControllerIo controller) => (controller switch { WindowControllerIo __object78348 => ((WindowControllerIo)((WindowControllerIo)__object78348)).isMaximized, DialogWindowControllerIo __object78398 => false, TooltipWindowControllerIo __object78437 => false, PopupWindowControllerIo __object78477 => false, SatelliteWindowControllerIo __object78515 => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    internal static bool _isMinimizedValue(BaseWindowControllerIo controller) => (controller switch { WindowControllerIo __object78827 => ((WindowControllerIo)((WindowControllerIo)__object78827)).isMinimized, DialogWindowControllerIo __object78877 => ((DialogWindowControllerIo)((DialogWindowControllerIo)__object78877)).isMinimized, TooltipWindowControllerIo __object78933 => false, PopupWindowControllerIo __object78973 => false, SatelliteWindowControllerIo __object79011 => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    internal static bool _isFullscreenValue(BaseWindowControllerIo controller) => (controller switch { WindowControllerIo __object79323 => ((WindowControllerIo)((WindowControllerIo)__object79323)).isFullscreen, DialogWindowControllerIo __object79374 => false, TooltipWindowControllerIo __object79413 => false, PopupWindowControllerIo __object79453 => false, SatelliteWindowControllerIo __object79491 => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    internal static BaseWindowControllerIo _of(BuildContext context, _WindowControllerAspect___window? aspect = null)
    {
        if (!global::Doroti.Generated.Framework.Foundation._featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
        DartRuntimePrimitives.Assert(() => WindowScopeIo._debugCheckHasWindowController(context));
        return InheritedModel<object>.inheritFrom<WindowScopeIo>(context, aspect: aspect)!.controller;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static BaseWindowControllerIo? _maybeOf(BuildContext context, _WindowControllerAspect___window? aspect = null)
    {
        if (!global::Doroti.Generated.Framework.Foundation._featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
        return InheritedModel<object>.inheritFrom<WindowScopeIo>(context, aspect: aspect)?.controller;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static bool _debugCheckHasWindowController(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((context.dependOnInheritedWidgetOfExactType<WindowScopeIo>() is null))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary("No WindowScope found in context."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{DartRuntimePrimitives.RuntimeType(((BuildContext)context).widget)} widgets require a WindowScope widget ancestor."), context.describeWidget("The specific widget that could not find a WindowScope ancestor was"), context.describeOwnershipChain("The ownership chain for the affected widget is"), new global::Doroti.Generated.Framework.Foundation.ErrorHint("No WindowScope ancestor could be found starting from the context " + "that was passed to WindowScope.of(). This can happen because the " + "context used is not a descendant of a Window widget, which introduces " + "a WindowScope.") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (WindowScopeIo)(object)oldWidget;
        return ((((((((!object.Equals(this.controller, ((WindowScopeIo)__oldWidget).controller)) || (!object.Equals(this._contentSize, ((WindowScopeIo)__oldWidget)._contentSize))) || (this._title != ((WindowScopeIo)__oldWidget)._title)) || (this._isActivated != ((WindowScopeIo)__oldWidget)._isActivated)) || (this._isMaximized != ((WindowScopeIo)__oldWidget)._isMaximized)) || (this._isMinimized != ((WindowScopeIo)__oldWidget)._isMinimized)) || (this._isFullscreen != ((WindowScopeIo)__oldWidget)._isFullscreen)) || (this._isDestroyed != ((WindowScopeIo)__oldWidget)._isDestroyed));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotifyDependent(InheritedModel<_WindowControllerAspect___window> oldWidget, HashSet<_WindowControllerAspect___window> dependencies)
    {
        var __oldWidget = (WindowScopeIo)(object)oldWidget;
        var __dependencies = (HashSet<object>)(object)dependencies;
        return __dependencies.any(((dependency) => ((dependency is _WindowControllerAspect___window) && (((_WindowControllerAspect___window)dependency) switch { _WindowControllerAspect___window.contentSize => (!object.Equals(this._contentSize, ((WindowScopeIo)__oldWidget)._contentSize)), _WindowControllerAspect___window.title => (this._title != ((WindowScopeIo)__oldWidget)._title), _WindowControllerAspect___window.activated => (this._isActivated != ((WindowScopeIo)__oldWidget)._isActivated), _WindowControllerAspect___window.maximized => (this._isMaximized != ((WindowScopeIo)__oldWidget)._isMaximized), _WindowControllerAspect___window.minimized => (this._isMinimized != ((WindowScopeIo)__oldWidget)._isMinimized), _WindowControllerAspect___window.fullscreen => (this._isFullscreen != ((WindowScopeIo)__oldWidget)._isFullscreen), _WindowControllerAspect___window.destroyed => (this._isDestroyed != ((WindowScopeIo)__oldWidget)._isDestroyed), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class WindowRegistryIo : global::Doroti.Generated.Framework.Foundation.ChangeNotifier
{
    internal virtual List<WindowEntryIo> _windows { get; private set; } = new List<WindowEntryIo>();

    public WindowRegistryIo()
    {
    }

    public virtual List<WindowEntryIo> windows => new List<WindowEntryIo>(DartRuntimePrimitives.ConvertEnumerable<WindowEntryIo>(this._windows));
    public virtual void register(WindowEntryIo entry)
    {
        if (!global::Doroti.Generated.Framework.Foundation._featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
        this._windows.Add(entry);
        notifyListeners();
    }

    public virtual void unregister(WindowEntryIo entry)
    {
        if (!global::Doroti.Generated.Framework.Foundation._featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
        this._windows.Remove(entry);
        notifyListeners();
    }

    public static WindowRegistryIo? maybeOf(BuildContext context)
    {
        return context.dependOnInheritedWidgetOfExactType<_WindowRegistryScope___window>()?._registry;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static WindowRegistryIo of(BuildContext context)
    {
        WindowRegistryIo? registry__85861 = ((WindowRegistryIo?)(object?)WindowRegistryIo.maybeOf(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((registry__85861 is null))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary("No WindowRegistry found in context."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{DartRuntimePrimitives.RuntimeType(((BuildContext)context).widget)} widgets require a WindowRegistry widget ancestor."), context.describeWidget("The specific widget that could not find a WindowRegistry ancestor was"), context.describeOwnershipChain("The ownership chain for the affected widget is"), new global::Doroti.Generated.Framework.Foundation.ErrorHint("No WindowRegistry ancestor could be found starting from the context " + "that was passed to WindowRegistry.of(). This can happen because the " + "context used is not a descendant of a WindowManager widget, which introduces " + "a WindowRegistry.") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return registry__85861!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _WindowRegistryScope___window : InheritedWidget
{
    internal virtual WindowRegistryIo _registry { get; private set; } = default!;

    internal _WindowRegistryScope___window(WindowRegistryIo registry, Widget child) : base(child: child)
    {
        this._registry = registry;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (_WindowRegistryScope___window)(object)oldWidget;
        return (!object.Equals(this._registry, ((_WindowRegistryScope___window)__oldWidget)._registry));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class WindowEntryIo
{
    public virtual BaseWindowControllerIo controller { get; private set; } = default!;
    public virtual global::System.Func<BuildContext, Widget> builder { get; private set; } = default!;

    public WindowEntryIo(BaseWindowControllerIo controller, global::System.Func<BuildContext, Widget> builder)
    {
        this.controller = controller;
        this.builder = builder;
    }

}

public class WindowManagerIo : StatefulWidget
{
    public virtual List<WindowEntryIo> initialWindows { get; private set; } = default!;

    public WindowManagerIo(global::Doroti.Generated.Framework.Foundation.Key? key = null, List<WindowEntryIo> initialWindows = default!) : base(key: key)
    {
        this.initialWindows = initialWindows;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _WindowManagerState___window());
}

internal class _WindowManagerState___window : State<WindowManagerIo>
{
    internal virtual WindowRegistryIo _registry { get; private set; } = new WindowRegistryIo();

    public override void initState()
    {
        base.initState();
        ((WindowManagerIo)this.widget).initialWindows.forEach((__arg0) => ((global::System.Action<WindowEntryIo>)((WindowRegistryIo)this._registry).register)(__arg0));
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new _WindowRegistryScope___window(registry: this._registry, child: new ListenableBuilder(listenable: this._registry, builder: ((global::System.Func<BuildContext, Widget?, Widget>)((context, child) => {
List<Widget> subViews__90295 = ((WindowRegistryIo)this._registry).windows.map<WindowEntryIo, StatelessWidget>(((entry) => {
return (((WindowEntryIo)entry).controller switch { DialogWindowControllerIo dialog__90440 => DartRuntimePrimitives.ConvertValue<StatelessWidget>(new DialogWindowIo(controller: dialog__90440, child: entry.builder(context))), WindowControllerIo regular__90601 => DartRuntimePrimitives.ConvertValue<StatelessWidget>(new WindowIo(controller: regular__90601, child: entry.builder(context))), TooltipWindowControllerIo tooltip__90765 => DartRuntimePrimitives.ConvertValue<StatelessWidget>(new TooltipWindowIo(controller: tooltip__90765, child: entry.builder(context))), PopupWindowControllerIo popup__90934 => DartRuntimePrimitives.ConvertValue<StatelessWidget>(new PopupWindowIo(controller: popup__90934, child: entry.builder(context))), SatelliteWindowControllerIo satellite__91101 => DartRuntimePrimitives.ConvertValue<StatelessWidget>(new SatelliteWindowIo(controller: satellite__91101, child: entry.builder(context))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
throw new InvalidOperationException("Dart closure completed without a value.");
})).ToList().Cast<Widget>().ToList();
return ((Widget)(object?)new ViewCollection(views: subViews__90295));
throw new InvalidOperationException("Dart closure completed without a value.");
})))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

