// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/_window.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public static partial class _windowLibrary
{
    internal static string _kWindowingDisabledErrorMessage =
        "Windowing APIs are not enabled.\n\nWindowing APIs are currently experimental. Do not use windowing APIs in\nproduction applications or plugins published to pub.dev.\n\nTo try experimental windowing APIs:\n1. Switch to Flutter's main release channel.\n2. Turn on the windowing feature flag.\n\nSee: https://github.com/flutter/flutter/issues/30701.\n";
}

public abstract class BaseWindowControllerIo : ChangeNotifier
{
    internal virtual DorotiView _view { get; private set; } = default!;

    public abstract Size contentSize { get; }
    public abstract void destroy();
    public virtual DorotiView rootView
    {
        get => _view;
        set
        {
            var view = value;
            _view = view;
        }
    }
    public abstract bool isDestroyed { get; }
}

public class WindowControllerDelegateIo
{
    public virtual void onWindowCloseRequested(WindowControllerIo controller)
    {
        if (!_featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
        controller.destroy();
    }

    public virtual void onWindowDestroyed()
    {
        if (!_featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
    }
}

public abstract class WindowControllerIo : BaseWindowControllerIo
{
    public static WindowControllerIo Create(
        Size size,
        BoxConstraints? constraints = null,
        string? title = null,
        WindowControllerDelegateIo? @delegate = null
    )
    {
        if (!_featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
        if (constraints is not null)
        {
            DartRuntimePrimitives.Assert(() => constraints.isSatisfiedBy(size));
        }
        WindowingOwnerIo owner = WidgetsBinding.instance.windowingOwner;
        return owner.createWindowController(
            @delegate: @delegate ?? new WindowControllerDelegateIo(),
            size: size,
            constraints: constraints,
            title: title,
            resizable: true
        );
    }

    public static WindowControllerIo CreateShrinkWrap(
        bool resizable = false,
        BoxConstraints? constraints = null,
        string? title = null,
        WindowControllerDelegateIo? @delegate = null
    )
    {
        if (!_featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
        WidgetsFlutterBinding.ensureInitialized();
        WindowingOwnerIo owner = WidgetsBinding.instance.windowingOwner;
        return owner.createWindowController(
            @delegate: @delegate ?? new WindowControllerDelegateIo(),
            constraints: constraints,
            resizable: resizable,
            title: title
        );
    }

    protected WindowControllerIo() { }

    public abstract string title { get; }
    public abstract bool isActivated { get; }
    public abstract bool isMaximized { get; }
    public abstract bool isMinimized { get; }
    public abstract bool isFullscreen { get; }
    public abstract void setSize(Size size);
    public abstract void setConstraints(BoxConstraints constraints);
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
        if (!_featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
        controller.destroy();
    }

    public virtual void onWindowDestroyed()
    {
        if (!_featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
    }
}

public abstract class DialogWindowControllerIo : BaseWindowControllerIo
{
    public static DialogWindowControllerIo Create(
        Size size,
        BoxConstraints? constraints = null,
        BaseWindowControllerIo? parent = null,
        string? title = null,
        DialogWindowControllerDelegateIo? @delegate = null
    )
    {
        if (!_featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
        WidgetsFlutterBinding.ensureInitialized();
        if (constraints is not null)
        {
            DartRuntimePrimitives.Assert(() => constraints.isSatisfiedBy(size));
        }
        WindowingOwnerIo owner = WidgetsBinding.instance.windowingOwner;
        return owner.createDialogWindowController(
            @delegate: @delegate ?? new DialogWindowControllerDelegateIo(),
            size: size,
            constraints: constraints,
            title: title,
            parent: parent,
            resizable: true
        );
    }

    public static DialogWindowControllerIo CreateShrinkWrap(
        bool resizable = false,
        BoxConstraints? constraints = null,
        BaseWindowControllerIo? parent = null,
        string? title = null,
        DialogWindowControllerDelegateIo? @delegate = null
    )
    {
        if (!_featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
        WidgetsFlutterBinding.ensureInitialized();
        WindowingOwnerIo owner = WidgetsBinding.instance.windowingOwner;
        return owner.createDialogWindowController(
            @delegate: @delegate ?? new DialogWindowControllerDelegateIo(),
            constraints: constraints,
            resizable: resizable,
            title: title,
            parent: parent
        );
    }

    protected DialogWindowControllerIo() { }

    public abstract BaseWindowControllerIo? parent { get; }
    public abstract string title { get; }
    public abstract bool isActivated { get; }
    public abstract bool isMinimized { get; }
    public abstract void setSize(Size size);
    public abstract void setConstraints(BoxConstraints constraints);
    public abstract void setTitle(string title);
    public abstract void activate();
    public abstract void setMinimized(bool minimized);
}

public class TooltipWindowControllerDelegateIo
{
    public virtual void onWindowDestroyed()
    {
        if (!_featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
    }
}

public abstract class TooltipWindowControllerIo : BaseWindowControllerIo
{
    public static TooltipWindowControllerIo Create(
        BaseWindowControllerIo parent,
        Rect anchorRect,
        WindowPositionerIo positioner,
        BoxConstraints constraints = default!,
        TooltipWindowControllerDelegateIo? @delegate = null
    )
    {
        BoxConstraints __constraints = constraints ?? new BoxConstraints();
        if (!_featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
        WidgetsFlutterBinding.ensureInitialized();
        WindowingOwnerIo owner = WidgetsBinding.instance.windowingOwner;
        TooltipWindowControllerIo controller = owner.createTooltipWindowController(
            parent: parent,
            constraints: __constraints,
            @delegate: @delegate ?? new TooltipWindowControllerDelegateIo(),
            anchorRect: DartRuntimePrimitives.RequireValue(
                DartRuntimePrimitives.RequireValue(anchorRect)
            ),
            positioner: positioner
        );
        return controller;
    }

    protected TooltipWindowControllerIo() { }

    public abstract BaseWindowControllerIo parent { get; }
    public abstract void setConstraints(BoxConstraints constraints);
    public abstract void updatePosition(
        Rect? anchorRect = null,
        WindowPositionerIo? positioner = null
    );
}

public class PopupWindowControllerDelegateIo
{
    public virtual void onWindowDestroyed()
    {
        if (!_featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
    }
}

public abstract class PopupWindowControllerIo : BaseWindowControllerIo
{
    public static PopupWindowControllerIo Create(
        BaseWindowControllerIo parent,
        Rect anchorRect,
        WindowPositionerIo positioner,
        BoxConstraints? constraints = null,
        PopupWindowControllerDelegateIo? @delegate = null
    )
    {
        if (!_featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
        WidgetsFlutterBinding.ensureInitialized();
        WindowingOwnerIo owner = WidgetsBinding.instance.windowingOwner;
        return owner.createPopupWindowController(
            parent: parent,
            constraints: constraints ?? new BoxConstraints(),
            @delegate: @delegate ?? new PopupWindowControllerDelegateIo(),
            anchorRect: DartRuntimePrimitives.RequireValue(
                DartRuntimePrimitives.RequireValue(anchorRect)
            ),
            positioner: positioner
        );
    }

    protected PopupWindowControllerIo() { }

    public abstract BaseWindowControllerIo parent { get; }
    public abstract void setConstraints(BoxConstraints constraints);
    public abstract void updatePosition(
        Rect? anchorRect = null,
        WindowPositionerIo? positioner = null
    );
    public abstract Offset offsetFromParent { get; }

    public virtual void activate()
    {
        BaseWindowControllerIo parentLocal = parent;
        while (true)
        {
            if (parentLocal is WindowControllerIo)
            {
                WindowControllerIo parent__35385__as35436 = (WindowControllerIo)parentLocal;
                parent__35385__as35436.activate();
                break;
            }
            else
            {
                if (parentLocal is DialogWindowControllerIo)
                {
                    DialogWindowControllerIo parent__35385__as35525 =
                        (DialogWindowControllerIo)parentLocal;
                    parent__35385__as35525.activate();
                    break;
                }
                else
                {
                    if (parentLocal is PopupWindowControllerIo)
                    {
                        PopupWindowControllerIo parent__35385__as35620 =
                            (PopupWindowControllerIo)parentLocal;
                        parentLocal = parent__35385__as35620.parent;
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            $"Unexpected controller in hierarchy {parentLocal}"
                        );
                    }
                }
            }
        }
    }

    public virtual bool isActivated
    {
        get
        {
            BaseWindowControllerIo parentLocal = parent;
            while (true)
            {
                if (parentLocal is WindowControllerIo)
                {
                    WindowControllerIo parent__35986__as36037 = (WindowControllerIo)parentLocal;
                    return parent__35986__as36037.isActivated;
                }
                else
                {
                    if (parentLocal is DialogWindowControllerIo)
                    {
                        DialogWindowControllerIo parent__35986__as36119 =
                            (DialogWindowControllerIo)parentLocal;
                        return parent__35986__as36119.isActivated;
                    }
                    else
                    {
                        if (parentLocal is PopupWindowControllerIo)
                        {
                            PopupWindowControllerIo parent__35986__as36207 =
                                (PopupWindowControllerIo)parentLocal;
                            parentLocal = parent__35986__as36207.parent;
                        }
                        else
                        {
                            throw new InvalidOperationException(
                                $"Unexpected controller in hierarchy {parentLocal}"
                            );
                        }
                    }
                }
            }
        }
    }
}

public class SatelliteWindowControllerDelegateIo
{
    public virtual void onWindowCloseRequested(SatelliteWindowControllerIo controller)
    {
        if (!_featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
        controller.destroy();
    }

    public virtual void onWindowDestroyed()
    {
        if (!_featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
    }
}

public abstract class SatelliteWindowControllerIo : BaseWindowControllerIo
{
    public static SatelliteWindowControllerIo Create(
        BaseWindowControllerIo parent,
        WindowPositionerIo initialPositioner,
        Rect? initialAnchorRect = null,
        Size? size = null,
        BoxConstraints? constraints = null,
        string? title = null,
        SatelliteWindowControllerDelegateIo? @delegate = null
    )
    {
        if (!_featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
        if ((size is not null) && (constraints is not null))
        {
            Size size__value41904 = DartRuntimePrimitives.RequireValue(size);
            DartRuntimePrimitives.Assert(() =>
                constraints.isSatisfiedBy(
                    DartRuntimePrimitives.RequireValue(
                        DartRuntimePrimitives.RequireValue(size__value41904)
                    )
                )
            );
        }
        WindowingOwnerIo owner = WidgetsBinding.instance.windowingOwner;
        return owner.createSatelliteWindowController(
            @delegate: @delegate ?? new SatelliteWindowControllerDelegateIo(),
            parent: parent,
            initialAnchorRect: initialAnchorRect,
            initialPositioner: initialPositioner,
            size: size,
            constraints: constraints,
            title: title,
            resizable: true
        );
    }

    public static SatelliteWindowControllerIo CreateShrinkWrap(
        BaseWindowControllerIo parent,
        WindowPositionerIo initialPositioner,
        Rect? initialAnchorRect = null,
        bool resizable = false,
        BoxConstraints? constraints = null,
        string? title = null,
        SatelliteWindowControllerDelegateIo? @delegate = null
    )
    {
        if (!_featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
        WidgetsFlutterBinding.ensureInitialized();
        WindowingOwnerIo owner = WidgetsBinding.instance.windowingOwner;
        return owner.createSatelliteWindowController(
            @delegate: @delegate ?? new SatelliteWindowControllerDelegateIo(),
            parent: parent,
            initialAnchorRect: initialAnchorRect,
            initialPositioner: initialPositioner,
            constraints: constraints,
            resizable: resizable,
            title: title
        );
    }

    protected SatelliteWindowControllerIo() { }

    public abstract BaseWindowControllerIo parent { get; }
    public abstract string title { get; }
    public abstract bool isActivated { get; }
    public abstract void setParent(BaseWindowControllerIo parent);
    public abstract void setSize(Size size);
    public abstract void setConstraints(BoxConstraints constraints);
    public abstract void setTitle(string title);
    public abstract void activate();
}

public interface WindowingOwnerIo
{
    public WindowControllerIo createWindowController(
        WindowControllerDelegateIo @delegate,
        Size? size = null,
        BoxConstraints? constraints = null,
        bool resizable = default!,
        string? title = null
    );
    public DialogWindowControllerIo createDialogWindowController(
        DialogWindowControllerDelegateIo @delegate,
        Size? size = null,
        BoxConstraints? constraints = null,
        bool resizable = default!,
        BaseWindowControllerIo? parent = null,
        string? title = null
    );
    public TooltipWindowControllerIo createTooltipWindowController(
        TooltipWindowControllerDelegateIo @delegate,
        BoxConstraints constraints,
        Rect anchorRect,
        WindowPositionerIo positioner,
        BaseWindowControllerIo parent
    );
    public PopupWindowControllerIo createPopupWindowController(
        PopupWindowControllerDelegateIo @delegate,
        BoxConstraints constraints,
        Rect anchorRect,
        WindowPositionerIo positioner,
        BaseWindowControllerIo parent
    );
    public SatelliteWindowControllerIo createSatelliteWindowController(
        SatelliteWindowControllerDelegateIo @delegate,
        BaseWindowControllerIo parent,
        WindowPositionerIo initialPositioner,
        Rect? initialAnchorRect = null,
        Size? size = null,
        BoxConstraints? constraints = null,
        bool resizable = default!,
        string? title = null
    );
}

public static partial class _windowLibrary
{
    public static WindowingOwnerIo createDefaultWindowingOwner()
    {
        if (!_featuresLibrary.isWindowingEnabled)
        {
            return new _WindowingOwnerUnsupported___window(
                errorMessage: _kWindowingDisabledErrorMessage
            );
        }
        WindowingOwnerIo? owner = _window_ioLibrary.createDefaultOwner();
        if (owner is not null)
        {
            return owner;
        }
        return new _WindowingOwnerUnsupported___window(
            errorMessage: "Windowing is unsupported on this platform."
        );
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

    public virtual WindowControllerIo createWindowController(
        WindowControllerDelegateIo @delegate,
        Size? size = null,
        BoxConstraints? constraints = null,
        bool resizable = true,
        string? title = null
    )
    {
        throw new NotSupportedException(errorMessage);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DialogWindowControllerIo createDialogWindowController(
        DialogWindowControllerDelegateIo @delegate,
        Size? size = null,
        BoxConstraints? constraints = null,
        bool resizable = true,
        BaseWindowControllerIo? parent = null,
        string? title = null
    )
    {
        throw new NotSupportedException(errorMessage);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TooltipWindowControllerIo createTooltipWindowController(
        TooltipWindowControllerDelegateIo @delegate,
        BoxConstraints constraints,
        Rect anchorRect,
        WindowPositionerIo positioner,
        BaseWindowControllerIo parent
    )
    {
        throw new NotImplementedException(errorMessage);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual PopupWindowControllerIo createPopupWindowController(
        PopupWindowControllerDelegateIo @delegate,
        BoxConstraints constraints,
        Rect anchorRect,
        WindowPositionerIo positioner,
        BaseWindowControllerIo parent
    )
    {
        throw new NotImplementedException(errorMessage);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SatelliteWindowControllerIo createSatelliteWindowController(
        SatelliteWindowControllerDelegateIo @delegate,
        BaseWindowControllerIo parent,
        WindowPositionerIo initialPositioner,
        Rect? initialAnchorRect = null,
        Size? size = null,
        BoxConstraints? constraints = null,
        bool resizable = true,
        string? title = null
    )
    {
        throw new NotImplementedException(errorMessage);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class WindowIo : StatelessWidget
{
    public virtual WindowControllerIo controller { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public WindowIo(
        Key? key = null,
        WindowControllerIo controller = default!,
        Widget child = default!
    )
        : base(key: key)
    {
        this.controller = controller;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        return new ListenableBuilder(
            listenable: controller,
            builder: (context, widget) =>
                new WindowScopeIo(
                    controller: controller,
                    child: new View(view: controller.rootView, child: child)
                )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class DialogWindowIo : StatelessWidget
{
    public virtual DialogWindowControllerIo controller { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public DialogWindowIo(
        Key? key = null,
        DialogWindowControllerIo controller = default!,
        Widget child = default!
    )
        : base(key: key)
    {
        this.controller = controller;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        return new ListenableBuilder(
            listenable: controller,
            builder: (context, widget) =>
                new WindowScopeIo(
                    controller: controller,
                    child: new View(view: controller.rootView, child: child)
                )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class TooltipWindowIo : StatelessWidget
{
    public virtual TooltipWindowControllerIo controller { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public TooltipWindowIo(
        Key? key = null,
        TooltipWindowControllerIo controller = default!,
        Widget child = default!
    )
        : base(key: key)
    {
        this.controller = controller;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        return new ListenableBuilder(
            listenable: controller,
            builder: (context, widget) =>
                new WindowScopeIo(
                    controller: controller,
                    child: new View(view: controller.rootView, child: child)
                )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class PopupWindowIo : StatelessWidget
{
    public virtual PopupWindowControllerIo controller { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public PopupWindowIo(
        Key? key = null,
        PopupWindowControllerIo controller = default!,
        Widget child = default!
    )
        : base(key: key)
    {
        this.controller = controller;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        return new ListenableBuilder(
            listenable: controller,
            builder: (context, widget) =>
                new WindowScopeIo(
                    controller: controller,
                    child: new View(view: controller.rootView, child: child)
                )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class SatelliteWindowIo : StatelessWidget
{
    public virtual SatelliteWindowControllerIo controller { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public SatelliteWindowIo(
        Key? key = null,
        SatelliteWindowControllerIo controller = default!,
        Widget child = default!
    )
        : base(key: key)
    {
        this.controller = controller;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        return new ListenableBuilder(
            listenable: controller,
            builder: (context, widget) =>
                new WindowScopeIo(
                    controller: controller,
                    child: new View(view: controller.rootView, child: child)
                )
        );
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
    destroyed,
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

    public WindowScopeIo(
        Key? key = null,
        BaseWindowControllerIo controller = default!,
        Widget child = default!
    )
        : base(key: key, child: child)
    {
        this.controller = controller;
        _isDestroyed = controller.isDestroyed;
        _contentSize = controller.isDestroyed ? Size.zero : controller.contentSize;
        _title = controller.isDestroyed ? "" : _titleValue(controller);
        _isActivated = !controller.isDestroyed && _isActivatedValue(controller);
        _isMaximized = !controller.isDestroyed && _isMaximizedValue(controller);
        _isMinimized = !controller.isDestroyed && _isMinimizedValue(controller);
        _isFullscreen = !controller.isDestroyed && _isFullscreenValue(controller);
    }

    public static BaseWindowControllerIo of(BuildContext context)
    {
        return _of(context);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static BaseWindowControllerIo? maybeOf(BuildContext context)
    {
        return _maybeOf(context);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Size contentSizeOf(BuildContext context) =>
        DartRuntimePrimitives.ConvertValue<Size>(
            _of(context, _WindowControllerAspect___window.contentSize).contentSize
        );

    public static Size? maybeContentSizeOf(BuildContext context) =>
        DartRuntimePrimitives.ConvertValue<Size>(
            _maybeOf(context, _WindowControllerAspect___window.contentSize)?.contentSize
        );

    public static string titleOf(BuildContext context)
    {
        return _titleValue(_of(context, _WindowControllerAspect___window.title));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static string? maybeTitleOf(BuildContext context)
    {
        BaseWindowControllerIo? controller = _maybeOf(
            context,
            _WindowControllerAspect___window.title
        );
        if (controller is null)
        {
            return null;
        }
        return (string?)_titleValue(controller);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool isActivatedOf(BuildContext context)
    {
        return _isActivatedValue(_of(context, _WindowControllerAspect___window.activated));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool? maybeIsActivatedOf(BuildContext context)
    {
        BaseWindowControllerIo? controller = _maybeOf(
            context,
            _WindowControllerAspect___window.activated
        );
        if (controller is null)
        {
            return null;
        }
        return _isActivatedValue(controller);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool isMinimizedOf(BuildContext context)
    {
        return _isMinimizedValue(_of(context, _WindowControllerAspect___window.minimized));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool? maybeIsMinimizedOf(BuildContext context)
    {
        BaseWindowControllerIo? controller = _maybeOf(
            context,
            _WindowControllerAspect___window.minimized
        );
        if (controller is null)
        {
            return null;
        }
        return _isMinimizedValue(controller);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool isMaximizedOf(BuildContext context)
    {
        return _isMaximizedValue(_of(context, _WindowControllerAspect___window.maximized));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool? maybeIsMaximizedOf(BuildContext context)
    {
        BaseWindowControllerIo? controller = _maybeOf(
            context,
            _WindowControllerAspect___window.maximized
        );
        if (controller is null)
        {
            return null;
        }
        return _isMaximizedValue(controller);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool isFullscreenOf(BuildContext context)
    {
        return _isFullscreenValue(_of(context, _WindowControllerAspect___window.fullscreen));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool? maybeIsFullscreenOf(BuildContext context)
    {
        BaseWindowControllerIo? controller = _maybeOf(
            context,
            _WindowControllerAspect___window.fullscreen
        );
        if (controller is null)
        {
            return null;
        }
        return _isFullscreenValue(controller);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool isDestroyedOf(BuildContext context)
    {
        return _of(context, _WindowControllerAspect___window.destroyed).isDestroyed;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool? maybeIsDestroyedOf(BuildContext context)
    {
        return _maybeOf(context, _WindowControllerAspect___window.destroyed)?.isDestroyed;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static string _titleValue(BaseWindowControllerIo controller) =>
        controller switch
        {
            WindowControllerIo __object77339 => __object77339.title,
            DialogWindowControllerIo __object77383 => __object77383.title,
            TooltipWindowControllerIo __object77433 => "",
            PopupWindowControllerIo __object77470 => "",
            SatelliteWindowControllerIo __object77505 => __object77505.title,
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };

    internal static bool _isActivatedValue(BaseWindowControllerIo controller) =>
        controller switch
        {
            WindowControllerIo __object77818 => __object77818.isActivated,
            DialogWindowControllerIo __object77868 => __object77868.isActivated,
            TooltipWindowControllerIo __object77924 => false,
            PopupWindowControllerIo __object77964 => __object77964.isActivated,
            SatelliteWindowControllerIo __object78019 => __object78019.isActivated,
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };

    internal static bool _isMaximizedValue(BaseWindowControllerIo controller) =>
        controller switch
        {
            WindowControllerIo __object78348 => __object78348.isMaximized,
            DialogWindowControllerIo __object78398 => false,
            TooltipWindowControllerIo __object78437 => false,
            PopupWindowControllerIo __object78477 => false,
            SatelliteWindowControllerIo __object78515 => false,
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };

    internal static bool _isMinimizedValue(BaseWindowControllerIo controller) =>
        controller switch
        {
            WindowControllerIo __object78827 => __object78827.isMinimized,
            DialogWindowControllerIo __object78877 => __object78877.isMinimized,
            TooltipWindowControllerIo __object78933 => false,
            PopupWindowControllerIo __object78973 => false,
            SatelliteWindowControllerIo __object79011 => false,
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };

    internal static bool _isFullscreenValue(BaseWindowControllerIo controller) =>
        controller switch
        {
            WindowControllerIo __object79323 => __object79323.isFullscreen,
            DialogWindowControllerIo __object79374 => false,
            TooltipWindowControllerIo __object79413 => false,
            PopupWindowControllerIo __object79453 => false,
            SatelliteWindowControllerIo __object79491 => false,
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };

    internal static BaseWindowControllerIo _of(
        BuildContext context,
        _WindowControllerAspect___window? aspect = null
    )
    {
        if (!_featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
        DartRuntimePrimitives.Assert(() => _debugCheckHasWindowController(context));
        return InheritedModel<object>
            .inheritFrom<WindowScopeIo>(context, aspect: aspect)!
            .controller;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static BaseWindowControllerIo? _maybeOf(
        BuildContext context,
        _WindowControllerAspect___window? aspect = null
    )
    {
        if (!_featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
        return InheritedModel<object>
            .inheritFrom<WindowScopeIo>(context, aspect: aspect)
            ?.controller;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static bool _debugCheckHasWindowController(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (context.dependOnInheritedWidgetOfExactType<WindowScopeIo>() is null)
            {
                throw DartRuntimePrimitives.AsException(
                    new FlutterError(
                        new List<DiagnosticsNode>
                        {
                            new ErrorSummary("No WindowScope found in context."),
                            new ErrorDescription(
                                $"{DartRuntimePrimitives.RuntimeType(context.widget)} widgets require a WindowScope widget ancestor."
                            ),
                            context.describeWidget(
                                "The specific widget that could not find a WindowScope ancestor was"
                            ),
                            context.describeOwnershipChain(
                                "The ownership chain for the affected widget is"
                            ),
                            new ErrorHint(
                                "No WindowScope ancestor could be found starting from the context "
                                    + "that was passed to WindowScope.of(). This can happen because the "
                                    + "context used is not a descendant of a Window widget, which introduces "
                                    + "a WindowScope."
                            ),
                        }
                    )
                );
            }
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (WindowScopeIo)oldWidget;
        return (!Equals(controller, __oldWidget.controller))
            || (!Equals(_contentSize, __oldWidget._contentSize))
            || (_title != __oldWidget._title)
            || (_isActivated != __oldWidget._isActivated)
            || (_isMaximized != __oldWidget._isMaximized)
            || (_isMinimized != __oldWidget._isMinimized)
            || (_isFullscreen != __oldWidget._isFullscreen)
            || (_isDestroyed != __oldWidget._isDestroyed);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotifyDependent(
        InheritedModel<_WindowControllerAspect___window> oldWidget,
        HashSet<_WindowControllerAspect___window> dependencies
    )
    {
        var __oldWidget = (WindowScopeIo)oldWidget;
        var __dependencies = new HashSet<object>(dependencies.Cast<object>());
        return __dependencies.any(
            (dependency) =>
                (dependency is _WindowControllerAspect___window)
                && (
                    (_WindowControllerAspect___window)dependency switch
                    {
                        _WindowControllerAspect___window.contentSize => !Equals(
                            _contentSize,
                            __oldWidget._contentSize
                        ),
                        _WindowControllerAspect___window.title => _title != __oldWidget._title,
                        _WindowControllerAspect___window.activated => _isActivated
                            != __oldWidget._isActivated,
                        _WindowControllerAspect___window.maximized => _isMaximized
                            != __oldWidget._isMaximized,
                        _WindowControllerAspect___window.minimized => _isMinimized
                            != __oldWidget._isMinimized,
                        _WindowControllerAspect___window.fullscreen => _isFullscreen
                            != __oldWidget._isFullscreen,
                        _WindowControllerAspect___window.destroyed => _isDestroyed
                            != __oldWidget._isDestroyed,
                        _ => throw new InvalidOperationException(
                            "Non-exhaustive Dart switch value."
                        ),
                    }
                )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class WindowRegistryIo : ChangeNotifier
{
    internal virtual List<WindowEntryIo> _windows { get; private set; } = new List<WindowEntryIo>();

    public WindowRegistryIo() { }

    public virtual List<WindowEntryIo> windows =>
        new List<WindowEntryIo>(DartRuntimePrimitives.ConvertEnumerable<WindowEntryIo>(_windows));

    public virtual void register(WindowEntryIo entry)
    {
        if (!_featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
        _windows.Add(entry);
        notifyListeners();
    }

    public virtual void unregister(WindowEntryIo entry)
    {
        if (!_featuresLibrary.isWindowingEnabled)
        {
            throw new NotSupportedException(_windowLibrary._kWindowingDisabledErrorMessage);
        }
        _windows.Remove(entry);
        notifyListeners();
    }

    public static WindowRegistryIo? maybeOf(BuildContext context)
    {
        return context
            .dependOnInheritedWidgetOfExactType<_WindowRegistryScope___window>()
            ?._registry;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static WindowRegistryIo of(BuildContext context)
    {
        WindowRegistryIo? registry = maybeOf(context);
        DartRuntimePrimitives.Assert(() =>
        {
            if (registry is null)
            {
                throw DartRuntimePrimitives.AsException(
                    new FlutterError(
                        new List<DiagnosticsNode>
                        {
                            new ErrorSummary("No WindowRegistry found in context."),
                            new ErrorDescription(
                                $"{DartRuntimePrimitives.RuntimeType(context.widget)} widgets require a WindowRegistry widget ancestor."
                            ),
                            context.describeWidget(
                                "The specific widget that could not find a WindowRegistry ancestor was"
                            ),
                            context.describeOwnershipChain(
                                "The ownership chain for the affected widget is"
                            ),
                            new ErrorHint(
                                "No WindowRegistry ancestor could be found starting from the context "
                                    + "that was passed to WindowRegistry.of(). This can happen because the "
                                    + "context used is not a descendant of a WindowManager widget, which introduces "
                                    + "a WindowRegistry."
                            ),
                        }
                    )
                );
            }
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        return registry!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _WindowRegistryScope___window : InheritedWidget
{
    internal virtual WindowRegistryIo _registry { get; private set; } = default!;

    internal _WindowRegistryScope___window(WindowRegistryIo registry, Widget child)
        : base(child: child)
    {
        _registry = registry;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (_WindowRegistryScope___window)oldWidget;
        return !Equals(_registry, __oldWidget._registry);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class WindowEntryIo
{
    public virtual BaseWindowControllerIo controller { get; private set; } = default!;
    public virtual Func<BuildContext, Widget> builder { get; private set; } = default!;

    public WindowEntryIo(BaseWindowControllerIo controller, Func<BuildContext, Widget> builder)
    {
        this.controller = controller;
        this.builder = builder;
    }
}

public class WindowManagerIo : StatefulWidget
{
    public virtual List<WindowEntryIo> initialWindows { get; private set; } = default!;

    public WindowManagerIo(Key? key = null, List<WindowEntryIo> initialWindows = default!)
        : base(key: key)
    {
        this.initialWindows = initialWindows;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _WindowManagerState___window());
}

internal class _WindowManagerState___window : State<WindowManagerIo>
{
    internal virtual WindowRegistryIo _registry { get; private set; } = new WindowRegistryIo();

    public override void initState()
    {
        base.initState();
        widget.initialWindows.forEach(
            (__arg0) => ((Action<WindowEntryIo>)_registry.register)(__arg0)
        );
    }

    public override Widget build(BuildContext context)
    {
        return new _WindowRegistryScope___window(
            registry: _registry,
            child: new ListenableBuilder(
                listenable: _registry,
                builder: (context, child) =>
                {
                    List<Widget> subViews = _registry
                        .windows.map(
                            (entry) =>
                            {
                                return entry.controller switch
                                {
                                    DialogWindowControllerIo dialog =>
                                        DartRuntimePrimitives.ConvertValue<StatelessWidget>(
                                            new DialogWindowIo(
                                                controller: dialog,
                                                child: entry.builder(context)
                                            )
                                        ),
                                    WindowControllerIo regular =>
                                        DartRuntimePrimitives.ConvertValue<StatelessWidget>(
                                            new WindowIo(
                                                controller: regular,
                                                child: entry.builder(context)
                                            )
                                        ),
                                    TooltipWindowControllerIo tooltip =>
                                        DartRuntimePrimitives.ConvertValue<StatelessWidget>(
                                            new TooltipWindowIo(
                                                controller: tooltip,
                                                child: entry.builder(context)
                                            )
                                        ),
                                    PopupWindowControllerIo popup =>
                                        DartRuntimePrimitives.ConvertValue<StatelessWidget>(
                                            new PopupWindowIo(
                                                controller: popup,
                                                child: entry.builder(context)
                                            )
                                        ),
                                    SatelliteWindowControllerIo satellite =>
                                        DartRuntimePrimitives.ConvertValue<StatelessWidget>(
                                            new SatelliteWindowIo(
                                                controller: satellite,
                                                child: entry.builder(context)
                                            )
                                        ),
                                    _ => throw new InvalidOperationException(
                                        "Non-exhaustive Dart switch value."
                                    ),
                                };
                                throw new InvalidOperationException(
                                    "Dart closure completed without a value."
                                );
                            }
                        )
                        .ToList()
                        .Cast<Widget>()
                        .ToList();
                    return new ViewCollection(views: subViews);
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
