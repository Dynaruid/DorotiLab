// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/raw_menu_anchor.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public static partial class Raw_menu_anchorLibrary
{
    internal static bool _kDebugMenus = false;
}

public static partial class Raw_menu_anchorLibrary
{
    internal static DartMap<ShortcutActivator, Intent> _kMenuTraversalShortcuts = new DartMap<
        ShortcutActivator,
        Intent
    >
    {
        [new SingleActivator(LogicalKeyboardKey.gameButtonA)] = new ActivateIntent(),
        [new SingleActivator(LogicalKeyboardKey.escape)] = new DismissIntent(),
        [new SingleActivator(LogicalKeyboardKey.arrowDown)] = new DirectionalFocusIntent(
            TraversalDirection.down
        ),
        [new SingleActivator(LogicalKeyboardKey.arrowUp)] = new DirectionalFocusIntent(
            TraversalDirection.up
        ),
        [new SingleActivator(LogicalKeyboardKey.arrowLeft)] = new DirectionalFocusIntent(
            TraversalDirection.left
        ),
        [new SingleActivator(LogicalKeyboardKey.arrowRight)] = new DirectionalFocusIntent(
            TraversalDirection.right
        ),
    };
}

public class RawMenuOverlayInfo
{
    public virtual Rect anchorRect { get; private set; } = default!;
    public virtual Size overlaySize { get; private set; } = default!;
    public virtual Offset? position { get; private set; }
    public virtual object tapRegionGroupId { get; private set; } = default!;

    public RawMenuOverlayInfo(
        Rect anchorRect,
        Size overlaySize,
        object tapRegionGroupId,
        Offset? position = null
    )
    {
        this.anchorRect = anchorRect;
        this.overlaySize = overlaySize;
        this.tapRegionGroupId = tapRegionGroupId;
        this.position = position;
    }

    public override bool Equals(object? other)
    {
        var __other = other as RawMenuOverlayInfo;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is RawMenuOverlayInfo)
            && Equals(__other.anchorRect, anchorRect)
            && Equals(__other.overlaySize, overlaySize)
            && Equals(__other.position, position)
            && Equals(__other.tapRegionGroupId, tapRegionGroupId);
    }

    public override int GetHashCode()
    {
        return FoundationRuntimePorts.ObjectHash(
            anchorRect,
            overlaySize,
            position,
            tapRegionGroupId
        );
    }
}

public delegate Widget RawMenuAnchorOverlayBuilder(BuildContext context, RawMenuOverlayInfo info);

public delegate Widget RawMenuAnchorChildBuilder(
    BuildContext context,
    MenuController controller,
    Widget? child
);

public delegate void RawMenuAnchorOpenRequestedCallback(Offset? position, Action showOverlay);

public delegate void RawMenuAnchorCloseRequestedCallback(Action hideOverlay);

internal class _MenuControllerScope__raw_menu_anchor : InheritedWidget
{
    public virtual bool isOpen { get; private set; } = default!;
    public virtual MenuController controller { get; private set; } = default!;

    internal _MenuControllerScope__raw_menu_anchor(
        bool isOpen,
        MenuController controller,
        Widget child
    )
        : base(child: child)
    {
        this.isOpen = isOpen;
        this.controller = controller;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (_MenuControllerScope__raw_menu_anchor)oldWidget;
        return isOpen != __oldWidget.isOpen;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class RawMenuAnchor : StatefulWidget
{
    public virtual Action? onOpen { get; private set; }
    public virtual Action? onClose { get; private set; }
    public virtual Action<Offset?, Action> onOpenRequested { get; private set; } = default!;
    public virtual Action<Action> onCloseRequested { get; private set; } = default!;
    public virtual Func<BuildContext, MenuController, Widget?, Widget>? builder
    {
        get;
        private set;
    }
    public virtual Widget? child { get; private set; }
    public virtual Func<BuildContext, RawMenuOverlayInfo, Widget> overlayBuilder
    {
        get;
        private set;
    } = default!;
    public virtual bool useRootOverlay { get; private set; } = default!;
    public virtual FocusNode? childFocusNode { get; private set; }
    public virtual bool consumeOutsideTaps { get; private set; } = default!;
    public virtual MenuController controller { get; private set; } = default!;

    public RawMenuAnchor(
        Key? key = null,
        FocusNode? childFocusNode = null,
        bool consumeOutsideTaps = false,
        Action? onOpen = null,
        Action? onClose = null,
        Action<Offset?, Action> onOpenRequested = default!,
        Action<Action> onCloseRequested = default!,
        bool useRootOverlay = false,
        Func<BuildContext, MenuController, Widget?, Widget>? builder = null,
        MenuController controller = default!,
        Func<BuildContext, RawMenuOverlayInfo, Widget> overlayBuilder = default!,
        Widget? child = null
    )
        : base(key: key)
    {
        Action<Offset?, Action> __onOpenRequested = onOpenRequested ?? _defaultOnOpenRequested;
        Action<Action> __onCloseRequested = onCloseRequested ?? _defaultOnCloseRequested;
        this.childFocusNode = childFocusNode;
        this.consumeOutsideTaps = consumeOutsideTaps;
        this.onOpen = onOpen;
        this.onClose = onClose;
        this.onOpenRequested = __onOpenRequested;
        this.onCloseRequested = __onCloseRequested;
        this.useRootOverlay = useRootOverlay;
        this.builder = builder;
        this.controller = controller;
        this.overlayBuilder = overlayBuilder;
        this.child = child;
    }

    internal static void _defaultOnOpenRequested(Offset? position, Action showOverlay)
    {
        showOverlay();
    }

    internal static void _defaultOnCloseRequested(Action hideOverlay)
    {
        hideOverlay();
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _RawMenuAnchorState__raw_menu_anchor());

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(ObjectFlagProperty<FocusNode>.CreateHas("focusNode", childFocusNode));
        properties.add(
            new FlagProperty(
                "useRootOverlay",
                value: useRootOverlay,
                ifFalse: "use nearest overlay",
                ifTrue: "use root overlay"
            )
        );
    }
}

internal interface _RawMenuAnchorBaseMixin__raw_menu_anchor : IState
{
    List<_RawMenuAnchorBaseMixin__raw_menu_anchor> _anchorChildren { get; }
    _RawMenuAnchorBaseMixin__raw_menu_anchor? _parent { get; set; }
    ScrollPosition? _scrollPosition { get; set; }
    Size? _viewSize { get; set; }

    public bool isRoot { get; }
    public MenuController menuController { get; }
    public bool isOpen { get; }
    public _RawMenuAnchorBaseMixin__raw_menu_anchor root { get; }
    public new void initState();
    public new void didChangeDependencies();
    public new void dispose();
    public void _addChild(_RawMenuAnchorBaseMixin__raw_menu_anchor child);
    public void _removeChild(_RawMenuAnchorBaseMixin__raw_menu_anchor child);
    public void _handleScroll();
    public void _childChangedOpenState();
    public void open(Offset? position = null);
    public void close(bool inDispose = false);
    public void handleOpenRequest(Offset? position = null);
    public void handleCloseRequest();
    public void closeChildren(bool inDispose = false);
    public void requestChildrenClose();
    public void handleOutsideTap(Gestures.PointerDownEvent pointerDownEvent);
    public Widget buildAnchor(BuildContext context);
    public new Widget build(BuildContext context);
    public string ToString(DiagnosticLevel minLevel = default!);
}

internal class _RawMenuAnchorState__raw_menu_anchor
    : State<RawMenuAnchor>,
        _RawMenuAnchorBaseMixin__raw_menu_anchor
{
    internal virtual GlobalKey<_RawMenuAnchorState__raw_menu_anchor> _anchorKey
    {
        get;
        private set;
    } =
        GlobalKey<_RawMenuAnchorState__raw_menu_anchor>.Create(
            debugLabel: Foundation.ConstantsLibrary.kReleaseMode ? null : "MenuAnchor"
        );
    internal virtual OverlayPortalController _overlayController { get; private set; } =
        new OverlayPortalController(
            debugLabel: Foundation.ConstantsLibrary.kReleaseMode ? null : "MenuAnchor controller"
        );
    internal virtual Offset? _menuPosition { get; set; } = default;
    public virtual List<_RawMenuAnchorBaseMixin__raw_menu_anchor> _anchorChildren { get; set; } =
        new List<_RawMenuAnchorBaseMixin__raw_menu_anchor>();
    public virtual _RawMenuAnchorBaseMixin__raw_menu_anchor? _parent { get; set; } = default!;
    public virtual ScrollPosition? _scrollPosition { get; set; } = default;
    public virtual Size? _viewSize { get; set; } = default;

    internal virtual bool _isRootOverlayAnchor =>
        _parent is not _RawMenuAnchorState__raw_menu_anchor;
    public virtual bool useRootOverlay
    {
        get
        {
            if (
                _parent is _RawMenuAnchorState__raw_menu_anchor
                {
                    useRootOverlay: bool useRoot
                } __object26335
            )
            {
                return useRoot;
            }
            DartRuntimePrimitives.Assert(() => _isRootOverlayAnchor);
            return widget.useRootOverlay;
        }
    }
    public virtual bool isOpen => _overlayController.isShowing;
    public virtual MenuController menuController => widget.controller;

    public override void didUpdateWidget(RawMenuAnchor oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(oldWidget.controller, widget.controller))
        {
            oldWidget.controller._detach(this);
            widget.controller._attach(this);
        }
    }

    public virtual void open(Offset? position = null)
    {
        if (!mounted)
        {
            return;
        }
        if (isOpen)
        {
            close();
        }
        DartRuntimePrimitives.Assert(() =>
            Raw_menu_anchorLibrary._debugMenuInfo($"Opening {this} at {position ?? Offset.zero}")
        );
        _parent?.requestChildrenClose();
        DartRuntimePrimitives.Assert(() => !_overlayController.isShowing);
        _menuPosition = position;
        _parent?._childChangedOpenState();
        _overlayController.show();
        if (_isRootOverlayAnchor)
        {
            widget.childFocusNode?.requestFocus();
        }
        widget.onOpen?.Invoke();
        setState(() => { });
    }

    public virtual void close(bool inDispose = false)
    {
        DartRuntimePrimitives.Assert(() =>
            Raw_menu_anchorLibrary._debugMenuInfo($"Closing {this}")
        );
        if (!isOpen)
        {
            return;
        }
        closeChildren(inDispose: inDispose);
        if (
            !Equals(
                Scheduler.SchedulerBinding.instance.schedulerPhase,
                Scheduler.SchedulerPhase.persistentCallbacks
            )
        )
        {
            _overlayController.hide();
        }
        else
        {
            if (!inDispose)
            {
                Scheduler.SchedulerBinding.instance.addPostFrameCallback(
                    (_) =>
                    {
                        _overlayController.hide();
                    },
                    debugLabel: "MenuAnchor.hide"
                );
            }
        }
        if (!inDispose)
        {
            _parent?._childChangedOpenState();
            widget.onClose?.Invoke();
            if (
                mounted
                && (
                    !Equals(
                        Scheduler.SchedulerBinding.instance.schedulerPhase,
                        Scheduler.SchedulerPhase.persistentCallbacks
                    )
                )
            )
            {
                setState(() => { });
            }
        }
    }

    public virtual void handleOpenRequest(Offset? position = null)
    {
        widget.onOpenRequested(
            position,
            () =>
            {
                open(position: position);
            }
        );
    }

    public virtual void handleCloseRequest()
    {
        if (
            !Equals(
                Scheduler.SchedulerBinding.instance.schedulerPhase,
                Scheduler.SchedulerPhase.persistentCallbacks
            )
        )
        {
            widget.onCloseRequested(() => close(default));
        }
        else
        {
            Scheduler.SchedulerBinding.instance.addPostFrameCallback(
                (_) =>
                {
                    if (mounted)
                    {
                        widget.onCloseRequested(() => close(default));
                    }
                },
                debugLabel: "RawMenuAnchor.handleCloseRequest"
            );
        }
        requestChildrenClose();
    }

    internal virtual Widget _buildOverlay(BuildContext context, OverlayChildLayoutInfo layoutInfo)
    {
        Matrix4 transform = layoutInfo.childPaintTransform;
        Size anchorSize = layoutInfo.childSize;
        Rect anchorRectLocal = MatrixUtils.transformRect(transform, Offset.zero & anchorSize);
        var info = new RawMenuOverlayInfo(
            anchorRect: anchorRectLocal,
            overlaySize: layoutInfo.overlaySize,
            position: _menuPosition,
            tapRegionGroupId: root.menuController
        );
        return widget.overlayBuilder(context, info);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Widget buildAnchor(BuildContext context)
    {
        Widget childLocal = new Shortcuts(
            includeSemantics: false,
            shortcuts: Raw_menu_anchorLibrary._kMenuTraversalShortcuts,
            child: new TapRegion(
                groupId: root.menuController,
                consumeOutsideTaps: root.isOpen && widget.consumeOutsideTaps,
                onTapOutside: handleOutsideTap,
                child: new Builder(
                    key: _anchorKey,
                    builder: (context) =>
                    {
                        return (
                                widget.builder is null
                                    ? widget.child
                                    : widget.builder.Invoke(context, menuController, widget.child)
                            ) ?? new SizedBox();
                        throw new InvalidOperationException(
                            "Dart closure completed without a value."
                        );
                    }
                )
            )
        );
        return OverlayPortal.CreateOverlayChildLayoutBuilder(
            controller: _overlayController,
            overlayChildBuilder: _buildOverlay,
            overlayLocation: useRootOverlay
                ? OverlayChildLocation.rootOverlay
                : OverlayChildLocation.nearestOverlay,
            child: childLocal
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString(DiagnosticLevel minLevel = default!)
    {
        return DiagnosticsLibrary.describeIdentity(this);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool isRoot => DartRuntimePrimitives.ConvertValue<bool>(_parent is null);
    public virtual _RawMenuAnchorBaseMixin__raw_menu_anchor root
    {
        get
        {
            _RawMenuAnchorBaseMixin__raw_menu_anchor anchor = this;
            while (anchor._parent is not null)
            {
                anchor = anchor._parent!;
            }
            return anchor;
        }
    }

    public override void initState()
    {
        base.initState();
        menuController._attach(this);
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _RawMenuAnchorBaseMixin__raw_menu_anchor? newParent = MenuController
            .maybeOf(context)
            ?._anchor;
        if (!Equals(newParent, _parent))
        {
            DartRuntimePrimitives.Assert(
                () => !Equals(newParent, this),
                () => (object?)"A MenuController should only be attached to one anchor at a time."
            );
            _parent?._removeChild(this);
            _parent = newParent;
            _parent?._addChild(this);
        }
        if (isRoot)
        {
            _scrollPosition?.isScrollingNotifier.removeListener(_handleScroll);
            _scrollPosition = Scrollable.maybeOf(context)?.position;
            _scrollPosition?.isScrollingNotifier.addListener(_handleScroll);
            Size newSize = MediaQuery.sizeOf(context);
            if ((_viewSize is not null) && (!Equals(newSize, _viewSize)) && isOpen)
            {
                handleCloseRequest();
            }
            _viewSize = newSize;
        }
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
            Raw_menu_anchorLibrary._debugMenuInfo($"Disposing of {this}")
        );
        if (isOpen)
        {
            close(inDispose: true);
        }
        if (isRoot)
        {
            _scrollPosition?.isScrollingNotifier.removeListener(_handleScroll);
            _scrollPosition = null;
        }
        _parent?._removeChild(this);
        _parent = null;
        _anchorChildren.Clear();
        menuController._detach(this);
        base.dispose();
    }

    public virtual void _addChild(_RawMenuAnchorBaseMixin__raw_menu_anchor child)
    {
        DartRuntimePrimitives.Assert(() =>
            isRoot || Raw_menu_anchorLibrary._debugMenuInfo($"Added root child: {child}")
        );
        DartRuntimePrimitives.Assert(() => !_anchorChildren.Contains(child));
        _anchorChildren.Add(child);
        DartRuntimePrimitives.Assert(() =>
            Raw_menu_anchorLibrary._debugMenuInfo($"Added:\n{child.widget.toStringDeep()}")
        );
        DartRuntimePrimitives.Assert(() =>
            Raw_menu_anchorLibrary._debugMenuInfo($"Tree:\n{widget.toStringDeep()}")
        );
    }

    public virtual void _removeChild(_RawMenuAnchorBaseMixin__raw_menu_anchor child)
    {
        DartRuntimePrimitives.Assert(() =>
            isRoot || Raw_menu_anchorLibrary._debugMenuInfo($"Removed root child: {child}")
        );
        DartRuntimePrimitives.Assert(() => _anchorChildren.Contains(child));
        DartRuntimePrimitives.Assert(() =>
            Raw_menu_anchorLibrary._debugMenuInfo($"Removing:\n{child.widget.toStringDeep()}")
        );
        _anchorChildren.Remove(child);
        DartRuntimePrimitives.Assert(() =>
            Raw_menu_anchorLibrary._debugMenuInfo($"Tree:\n{widget.toStringDeep()}")
        );
    }

    public virtual void _handleScroll()
    {
        if (isOpen)
        {
            handleCloseRequest();
        }
    }

    public virtual void _childChangedOpenState()
    {
        _parent?._childChangedOpenState();
        if (
            !Equals(
                Scheduler.SchedulerBinding.instance.schedulerPhase,
                Scheduler.SchedulerPhase.persistentCallbacks
            )
        )
        {
            setState(() => { });
        }
        else
        {
            Scheduler.SchedulerBinding.instance.addPostFrameCallback(
                (timestamp) =>
                {
                    setState(() => { });
                }
            );
        }
    }

    public virtual void closeChildren(bool inDispose = false)
    {
        DartRuntimePrimitives.Assert(() =>
            Raw_menu_anchorLibrary._debugMenuInfo(
                $"Closing children of {this}{(inDispose ? " (dispose)" : "")}"
            )
        );
        var children = new List<_RawMenuAnchorBaseMixin__raw_menu_anchor>(
            DartRuntimePrimitives.ConvertEnumerable<_RawMenuAnchorBaseMixin__raw_menu_anchor>(
                _anchorChildren
            )
        );
        foreach (var child in children)
        {
            child.close(inDispose: inDispose);
        }
    }

    public virtual void requestChildrenClose()
    {
        DartRuntimePrimitives.Assert(() =>
            Raw_menu_anchorLibrary._debugMenuInfo(
                $"Calling handleCloseRequest for children of {this}"
            )
        );
        var children = new List<_RawMenuAnchorBaseMixin__raw_menu_anchor>(
            DartRuntimePrimitives.ConvertEnumerable<_RawMenuAnchorBaseMixin__raw_menu_anchor>(
                _anchorChildren
            )
        );
        foreach (var child in children)
        {
            child.handleCloseRequest();
        }
    }

    public virtual void handleOutsideTap(Gestures.PointerDownEvent pointerDownEvent)
    {
        DartRuntimePrimitives.Assert(() =>
            Raw_menu_anchorLibrary._debugMenuInfo($"Tapped Outside {menuController}")
        );
        if (isOpen)
        {
            requestChildrenClose();
        }
    }

    public override Widget build(BuildContext context)
    {
        return new _MenuControllerScope__raw_menu_anchor(
            isOpen: isOpen,
            controller: menuController,
            child: new Actions(
                actions: isOpen
                    ? new DartMap<Type, dynamic>
                    {
                        [typeof(DismissIntent)] = new DismissMenuAction(menuController),
                    }
                    : new DartMap<Type, dynamic>(),
                child: new Builder(builder: buildAnchor)
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class RawMenuAnchorGroup : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual MenuController controller { get; private set; } = default!;

    public RawMenuAnchorGroup(
        Key? key = null,
        Widget child = default!,
        MenuController controller = default!
    )
        : base(key: key)
    {
        this.child = child;
        this.controller = controller;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(ObjectFlagProperty<MenuController>.CreateHas("controller", controller));
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _RawMenuAnchorGroupState__raw_menu_anchor());
}

internal class _RawMenuAnchorGroupState__raw_menu_anchor
    : State<RawMenuAnchorGroup>,
        _RawMenuAnchorBaseMixin__raw_menu_anchor
{
    public virtual List<_RawMenuAnchorBaseMixin__raw_menu_anchor> _anchorChildren { get; set; } =
        new List<_RawMenuAnchorBaseMixin__raw_menu_anchor>();
    public virtual _RawMenuAnchorBaseMixin__raw_menu_anchor? _parent { get; set; } = default!;
    public virtual ScrollPosition? _scrollPosition { get; set; } = default;
    public virtual Size? _viewSize { get; set; } = default;

    public virtual bool isOpen => _anchorChildren.any((child) => child.isOpen);
    public virtual MenuController menuController => widget.controller;

    public override void didUpdateWidget(RawMenuAnchorGroup oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(oldWidget.controller, widget.controller))
        {
            oldWidget.controller._detach(this);
            widget.controller._attach(this);
        }
    }

    public virtual void close(bool inDispose = false)
    {
        if (!isOpen)
        {
            return;
        }
        closeChildren(inDispose: inDispose);
        if (!inDispose)
        {
            if (
                !Equals(
                    Scheduler.SchedulerBinding.instance.schedulerPhase,
                    Scheduler.SchedulerPhase.persistentCallbacks
                )
            )
            {
                setState(() => { });
            }
            else
            {
                Scheduler.SchedulerBinding.instance.addPostFrameCallback(
                    (timestamp) =>
                    {
                        if (mounted)
                        {
                            setState(() => { });
                        }
                    }
                );
            }
        }
    }

    public virtual void open(Offset? position = null)
    {
        DartRuntimePrimitives.Assert(() => Equals(menuController._anchor, this));
        return;
    }

    public virtual void handleCloseRequest()
    {
        DartRuntimePrimitives.Assert(() =>
            Raw_menu_anchorLibrary._debugMenuInfo($"Requesting close {this}")
        );
        requestChildrenClose();
    }

    public virtual void handleOpenRequest(Offset? position = null)
    {
        DartRuntimePrimitives.Assert(() =>
            Raw_menu_anchorLibrary._debugMenuInfo($"Requesting open {this}")
        );
        open(position: position);
    }

    public virtual Widget buildAnchor(BuildContext context)
    {
        return new TapRegion(
            groupId: root.menuController,
            onTapOutside: handleOutsideTap,
            child: widget.child
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool isRoot => DartRuntimePrimitives.ConvertValue<bool>(_parent is null);
    public virtual _RawMenuAnchorBaseMixin__raw_menu_anchor root
    {
        get
        {
            _RawMenuAnchorBaseMixin__raw_menu_anchor anchor = this;
            while (anchor._parent is not null)
            {
                anchor = anchor._parent!;
            }
            return anchor;
        }
    }

    public override void initState()
    {
        base.initState();
        menuController._attach(this);
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _RawMenuAnchorBaseMixin__raw_menu_anchor? newParent = MenuController
            .maybeOf(context)
            ?._anchor;
        if (!Equals(newParent, _parent))
        {
            DartRuntimePrimitives.Assert(
                () => !Equals(newParent, this),
                () => (object?)"A MenuController should only be attached to one anchor at a time."
            );
            _parent?._removeChild(this);
            _parent = newParent;
            _parent?._addChild(this);
        }
        if (isRoot)
        {
            _scrollPosition?.isScrollingNotifier.removeListener(_handleScroll);
            _scrollPosition = Scrollable.maybeOf(context)?.position;
            _scrollPosition?.isScrollingNotifier.addListener(_handleScroll);
            Size newSize = MediaQuery.sizeOf(context);
            if ((_viewSize is not null) && (!Equals(newSize, _viewSize)) && isOpen)
            {
                handleCloseRequest();
            }
            _viewSize = newSize;
        }
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
            Raw_menu_anchorLibrary._debugMenuInfo($"Disposing of {this}")
        );
        if (isOpen)
        {
            close(inDispose: true);
        }
        if (isRoot)
        {
            _scrollPosition?.isScrollingNotifier.removeListener(_handleScroll);
            _scrollPosition = null;
        }
        _parent?._removeChild(this);
        _parent = null;
        _anchorChildren.Clear();
        menuController._detach(this);
        base.dispose();
    }

    public virtual void _addChild(_RawMenuAnchorBaseMixin__raw_menu_anchor child)
    {
        DartRuntimePrimitives.Assert(() =>
            isRoot || Raw_menu_anchorLibrary._debugMenuInfo($"Added root child: {child}")
        );
        DartRuntimePrimitives.Assert(() => !_anchorChildren.Contains(child));
        _anchorChildren.Add(child);
        DartRuntimePrimitives.Assert(() =>
            Raw_menu_anchorLibrary._debugMenuInfo($"Added:\n{child.widget.toStringDeep()}")
        );
        DartRuntimePrimitives.Assert(() =>
            Raw_menu_anchorLibrary._debugMenuInfo($"Tree:\n{widget.toStringDeep()}")
        );
    }

    public virtual void _removeChild(_RawMenuAnchorBaseMixin__raw_menu_anchor child)
    {
        DartRuntimePrimitives.Assert(() =>
            isRoot || Raw_menu_anchorLibrary._debugMenuInfo($"Removed root child: {child}")
        );
        DartRuntimePrimitives.Assert(() => _anchorChildren.Contains(child));
        DartRuntimePrimitives.Assert(() =>
            Raw_menu_anchorLibrary._debugMenuInfo($"Removing:\n{child.widget.toStringDeep()}")
        );
        _anchorChildren.Remove(child);
        DartRuntimePrimitives.Assert(() =>
            Raw_menu_anchorLibrary._debugMenuInfo($"Tree:\n{widget.toStringDeep()}")
        );
    }

    public virtual void _handleScroll()
    {
        if (isOpen)
        {
            handleCloseRequest();
        }
    }

    public virtual void _childChangedOpenState()
    {
        _parent?._childChangedOpenState();
        if (
            !Equals(
                Scheduler.SchedulerBinding.instance.schedulerPhase,
                Scheduler.SchedulerPhase.persistentCallbacks
            )
        )
        {
            setState(() => { });
        }
        else
        {
            Scheduler.SchedulerBinding.instance.addPostFrameCallback(
                (timestamp) =>
                {
                    setState(() => { });
                }
            );
        }
    }

    public virtual void closeChildren(bool inDispose = false)
    {
        DartRuntimePrimitives.Assert(() =>
            Raw_menu_anchorLibrary._debugMenuInfo(
                $"Closing children of {this}{(inDispose ? " (dispose)" : "")}"
            )
        );
        var children = new List<_RawMenuAnchorBaseMixin__raw_menu_anchor>(
            DartRuntimePrimitives.ConvertEnumerable<_RawMenuAnchorBaseMixin__raw_menu_anchor>(
                _anchorChildren
            )
        );
        foreach (var child in children)
        {
            child.close(inDispose: inDispose);
        }
    }

    public virtual void requestChildrenClose()
    {
        DartRuntimePrimitives.Assert(() =>
            Raw_menu_anchorLibrary._debugMenuInfo(
                $"Calling handleCloseRequest for children of {this}"
            )
        );
        var children = new List<_RawMenuAnchorBaseMixin__raw_menu_anchor>(
            DartRuntimePrimitives.ConvertEnumerable<_RawMenuAnchorBaseMixin__raw_menu_anchor>(
                _anchorChildren
            )
        );
        foreach (var child in children)
        {
            child.handleCloseRequest();
        }
    }

    public virtual void handleOutsideTap(Gestures.PointerDownEvent pointerDownEvent)
    {
        DartRuntimePrimitives.Assert(() =>
            Raw_menu_anchorLibrary._debugMenuInfo($"Tapped Outside {menuController}")
        );
        if (isOpen)
        {
            requestChildrenClose();
        }
    }

    public override Widget build(BuildContext context)
    {
        return new _MenuControllerScope__raw_menu_anchor(
            isOpen: isOpen,
            controller: menuController,
            child: new Actions(
                actions: isOpen
                    ? new DartMap<Type, dynamic>
                    {
                        [typeof(DismissIntent)] = new DismissMenuAction(menuController),
                    }
                    : new DartMap<Type, dynamic>(),
                child: new Builder(builder: buildAnchor)
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString(DiagnosticLevel minLevel = default!) =>
        DiagnosticsLibrary.describeIdentity(this);
}

public class MenuController
{
    internal virtual _RawMenuAnchorBaseMixin__raw_menu_anchor? _anchor { get; set; } = default!;

    public virtual bool isOpen =>
        DartRuntimePrimitives.ConvertValue<bool>((_anchor?.isOpen) ?? false);

    public virtual void open(Offset? position = null)
    {
        DartRuntimePrimitives.Assert(() => _anchor is not null);
        _anchor!.handleOpenRequest(position: position);
    }

    public virtual void close()
    {
        _anchor?.handleCloseRequest();
    }

    public virtual void closeChildren()
    {
        DartRuntimePrimitives.Assert(() => _anchor is not null);
        _anchor!.requestChildrenClose();
    }

    internal virtual void _attach(_RawMenuAnchorBaseMixin__raw_menu_anchor anchor)
    {
        _anchor = anchor;
    }

    internal virtual void _detach(_RawMenuAnchorBaseMixin__raw_menu_anchor anchor)
    {
        if (Equals(_anchor, anchor))
        {
            _anchor = null;
        }
    }

    public static MenuController? maybeOf(BuildContext context)
    {
        return context
            .getInheritedWidgetOfExactType<_MenuControllerScope__raw_menu_anchor>()
            ?.controller;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool? maybeIsOpenOf(BuildContext context)
    {
        return context
            .dependOnInheritedWidgetOfExactType<_MenuControllerScope__raw_menu_anchor>()
            ?.isOpen;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => DiagnosticsLibrary.describeIdentity(this);
}

public class DismissMenuAction : DismissAction
{
    public virtual MenuController controller { get; private set; } = default!;

    public DismissMenuAction(MenuController controller)
    {
        this.controller = controller;
    }

    public override object? invoke(DismissIntent intent, BuildContext? context = null)
    {
        controller._anchor!.root.handleCloseRequest();
        return null;
    }

    public override bool isEnabled(DismissIntent intent, BuildContext? context = null)
    {
        return controller._anchor is not null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Raw_menu_anchorLibrary
{
    internal static bool _debugMenuInfo(string message, IEnumerable<string>? details = null)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (_kDebugMenus)
            {
                PrintLibrary.debugPrint($"MENU: {message}");
                if ((details is not null) && Enumerable.Any(details))
                {
                    foreach (string detail in details)
                    {
                        PrintLibrary.debugPrint($"    {detail}");
                    }
                }
            }
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
