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
    internal static DartMap<ShortcutActivator, Intent> _kMenuTraversalShortcuts = new DartMap<ShortcutActivator, Intent> { [new SingleActivator(LogicalKeyboardKey.gameButtonA)] = ((Intent)new ActivateIntent()), [new SingleActivator(LogicalKeyboardKey.escape)] = ((Intent)new DismissIntent()), [new SingleActivator(LogicalKeyboardKey.arrowDown)] = ((Intent)new DirectionalFocusIntent(TraversalDirection.down)), [new SingleActivator(LogicalKeyboardKey.arrowUp)] = ((Intent)new DirectionalFocusIntent(TraversalDirection.up)), [new SingleActivator(LogicalKeyboardKey.arrowLeft)] = ((Intent)new DirectionalFocusIntent(TraversalDirection.left)), [new SingleActivator(LogicalKeyboardKey.arrowRight)] = ((Intent)new DirectionalFocusIntent(TraversalDirection.right)) };
}

public class RawMenuOverlayInfo
{
    public virtual Rect anchorRect { get; private set; } = default!;
    public virtual Size overlaySize { get; private set; } = default!;
    public virtual Offset? position { get; private set; }
    public virtual object tapRegionGroupId { get; private set; } = default!;

    public RawMenuOverlayInfo(Rect anchorRect, Size overlaySize, object tapRegionGroupId, Offset? position = null)
    {
        this.anchorRect = anchorRect;
        this.overlaySize = overlaySize;
        this.tapRegionGroupId = tapRegionGroupId;
        this.position = position;
    }

    public override bool Equals(object? other)
    {
        var __other = other as RawMenuOverlayInfo;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((__other is RawMenuOverlayInfo) && (Equals(((RawMenuOverlayInfo)((RawMenuOverlayInfo)__other)).anchorRect, this.anchorRect))) && (Equals(((RawMenuOverlayInfo)((RawMenuOverlayInfo)__other)).overlaySize, this.overlaySize))) && (Equals(((RawMenuOverlayInfo)((RawMenuOverlayInfo)__other)).position, this.position))) && (Equals(((RawMenuOverlayInfo)((RawMenuOverlayInfo)__other)).tapRegionGroupId, this.tapRegionGroupId)));
    }

    public override int GetHashCode()
    {
        return FoundationRuntimePorts.ObjectHash(this.anchorRect, this.overlaySize, this.position, this.tapRegionGroupId);
    }
}

public delegate Widget RawMenuAnchorOverlayBuilder(BuildContext context, RawMenuOverlayInfo info);

public delegate Widget RawMenuAnchorChildBuilder(BuildContext context, MenuController controller, Widget? child);

public delegate void RawMenuAnchorOpenRequestedCallback(Offset? position, global::System.Action showOverlay);

public delegate void RawMenuAnchorCloseRequestedCallback(global::System.Action hideOverlay);

internal class _MenuControllerScope__raw_menu_anchor : InheritedWidget
{
    public virtual bool isOpen { get; private set; } = default!;
    public virtual MenuController controller { get; private set; } = default!;

    internal _MenuControllerScope__raw_menu_anchor(bool isOpen, MenuController controller, Widget child) : base(child: child)
    {
        this.isOpen = isOpen;
        this.controller = controller;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (_MenuControllerScope__raw_menu_anchor)oldWidget;
        return (this.isOpen != ((_MenuControllerScope__raw_menu_anchor)__oldWidget).isOpen);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class RawMenuAnchor : StatefulWidget
{
    public virtual global::System.Action? onOpen { get; private set; }
    public virtual global::System.Action? onClose { get; private set; }
    public virtual global::System.Action<Offset?, global::System.Action> onOpenRequested { get; private set; } = default!;
    public virtual global::System.Action<global::System.Action> onCloseRequested { get; private set; } = default!;
    public virtual global::System.Func<BuildContext, MenuController, Widget?, Widget>? builder { get; private set; }
    public virtual Widget? child { get; private set; }
    public virtual global::System.Func<BuildContext, RawMenuOverlayInfo, Widget> overlayBuilder { get; private set; } = default!;
    public virtual bool useRootOverlay { get; private set; } = default!;
    public virtual FocusNode? childFocusNode { get; private set; }
    public virtual bool consumeOutsideTaps { get; private set; } = default!;
    public virtual MenuController controller { get; private set; } = default!;

    public RawMenuAnchor(global::Doroti.Framework.Foundation.Key? key = null, FocusNode? childFocusNode = null, bool consumeOutsideTaps = false, global::System.Action? onOpen = null, global::System.Action? onClose = null, global::System.Action<Offset?, global::System.Action> onOpenRequested = default!, global::System.Action<global::System.Action> onCloseRequested = default!, bool useRootOverlay = false, global::System.Func<BuildContext, MenuController, Widget?, Widget>? builder = null, MenuController controller = default!, global::System.Func<BuildContext, RawMenuOverlayInfo, Widget> overlayBuilder = default!, Widget? child = null) : base(key: key)
    {
        global::System.Action<Offset?, global::System.Action> __onOpenRequested = onOpenRequested ?? _defaultOnOpenRequested;
        global::System.Action<global::System.Action> __onCloseRequested = onCloseRequested ?? _defaultOnCloseRequested;
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

    internal static void _defaultOnOpenRequested(Offset? position, global::System.Action showOverlay)
    {
        showOverlay();
    }

    internal static void _defaultOnCloseRequested(global::System.Action hideOverlay)
    {
        hideOverlay();
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _RawMenuAnchorState__raw_menu_anchor());
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(ObjectFlagProperty<FocusNode>.CreateHas("focusNode", this.childFocusNode));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("useRootOverlay", value: this.useRootOverlay, ifFalse: "use nearest overlay", ifTrue: "use root overlay"));
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
    public void handleOutsideTap(global::Doroti.Framework.Gestures.PointerDownEvent pointerDownEvent);
    public Widget buildAnchor(BuildContext context);
    public new Widget build(BuildContext context);
    public string ToString(global::Doroti.Framework.Foundation.DiagnosticLevel minLevel = default!);
}

internal class _RawMenuAnchorState__raw_menu_anchor : State<RawMenuAnchor>, _RawMenuAnchorBaseMixin__raw_menu_anchor
{
    internal virtual GlobalKey<_RawMenuAnchorState__raw_menu_anchor> _anchorKey { get; private set; } = GlobalKey<_RawMenuAnchorState__raw_menu_anchor>.Create(debugLabel: (Foundation.ConstantsLibrary.kReleaseMode ? null : "MenuAnchor"));
    internal virtual OverlayPortalController _overlayController { get; private set; } = new OverlayPortalController(debugLabel: (Foundation.ConstantsLibrary.kReleaseMode ? null : "MenuAnchor controller"));
    internal virtual Offset? _menuPosition { get; set; } = default;
    public virtual List<_RawMenuAnchorBaseMixin__raw_menu_anchor> _anchorChildren { get; set; } = new List<_RawMenuAnchorBaseMixin__raw_menu_anchor>();
    public virtual _RawMenuAnchorBaseMixin__raw_menu_anchor? _parent { get; set; } = default!;
    public virtual ScrollPosition? _scrollPosition { get; set; } = default;
    public virtual Size? _viewSize { get; set; } = default;

    internal virtual bool _isRootOverlayAnchor => (this._parent is not _RawMenuAnchorState__raw_menu_anchor);
    public virtual bool useRootOverlay
    {
        get
        {
            if (this._parent is _RawMenuAnchorState__raw_menu_anchor { useRootOverlay: bool useRoot } __object26335)
            {
                return useRoot;
            }
            DartRuntimePrimitives.Assert(() => this._isRootOverlayAnchor);
            return ((RawMenuAnchor)this.widget).useRootOverlay;
        }
    }
    public virtual bool isOpen => ((OverlayPortalController)this._overlayController).isShowing;
    public virtual MenuController menuController => ((RawMenuAnchor)this.widget).controller;
    public override void didUpdateWidget(RawMenuAnchor oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!Equals(((RawMenuAnchor)oldWidget).controller, ((RawMenuAnchor)this.widget).controller)))
        {
            ((RawMenuAnchor)oldWidget).controller._detach(this);
            ((RawMenuAnchor)this.widget).controller._attach(this);
        }
    }

    public virtual void open(Offset? position = null)
    {
        if (!this.mounted)
        {
            return;
        }
        if (this.isOpen)
        {
            close();
        }
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Opening {this} at {(position ?? Offset.zero)}"));
        (this._parent)?.requestChildrenClose();
        DartRuntimePrimitives.Assert(() => !((OverlayPortalController)this._overlayController).isShowing);
        _menuPosition = position;
        (this._parent)?._childChangedOpenState();
        this._overlayController.show();
        if (this._isRootOverlayAnchor)
        {
            ((RawMenuAnchor)this.widget).childFocusNode?.requestFocus();
        }
        ((RawMenuAnchor)this.widget).onOpen?.Invoke();
        setState(((global::System.Action)(() =>
        {
        })));
    }

    public virtual void close(bool inDispose = false)
    {
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Closing {this}"));
        if (!this.isOpen)
        {
            return;
        }
        closeChildren(inDispose: inDispose);
        if ((!Equals(Scheduler.SchedulerBinding.instance.schedulerPhase, Scheduler.SchedulerPhase.persistentCallbacks)))
        {
            this._overlayController.hide();
        }
        else
        {
            if (!inDispose)
            {
                Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) =>
                {
                    this._overlayController.hide();
                })), debugLabel: "MenuAnchor.hide");
            }
        }
        if (!inDispose)
        {
            (this._parent)?._childChangedOpenState();
            ((RawMenuAnchor)this.widget).onClose?.Invoke();
            if ((this.mounted && (!Equals(Scheduler.SchedulerBinding.instance.schedulerPhase, Scheduler.SchedulerPhase.persistentCallbacks))))
            {
                setState(((global::System.Action)(() =>
                {
                })));
            }
        }
    }

    public virtual void handleOpenRequest(Offset? position = null)
    {
        this.widget.onOpenRequested(position, (() =>
        {
            open(position: position);
        }));
    }

    public virtual void handleCloseRequest()
    {
        if ((!Equals(Scheduler.SchedulerBinding.instance.schedulerPhase, Scheduler.SchedulerPhase.persistentCallbacks)))
        {
            this.widget.onCloseRequested(() => this.close(default));
        }
        else
        {
            Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) =>
            {
                if (this.mounted)
                {
                    this.widget.onCloseRequested(() => this.close(default));
                }
            })), debugLabel: "RawMenuAnchor.handleCloseRequest");
        }
        requestChildrenClose();
    }

    internal virtual Widget _buildOverlay(BuildContext context, OverlayChildLayoutInfo layoutInfo)
    {
        Matrix4 transform = ((OverlayChildLayoutInfo)layoutInfo).childPaintTransform;
        global::Doroti.Ui.Size anchorSize = ((global::Doroti.Ui.Size)((OverlayChildLayoutInfo)layoutInfo).childSize);
        global::Doroti.Ui.Rect anchorRectLocal = ((global::Doroti.Ui.Rect)MatrixUtils.transformRect(transform, (Offset.zero & anchorSize)));
        var info = new RawMenuOverlayInfo(anchorRect: anchorRectLocal, overlaySize: ((OverlayChildLayoutInfo)layoutInfo).overlaySize, position: this._menuPosition, tapRegionGroupId: ((MenuController)(this.root).menuController));
        return this.widget.overlayBuilder(context, info);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Widget buildAnchor(BuildContext context)
    {
        Widget childLocal = ((Widget)new Shortcuts(includeSemantics: false, shortcuts: Raw_menu_anchorLibrary._kMenuTraversalShortcuts, child: new TapRegion(groupId: ((MenuController)(this.root).menuController), consumeOutsideTaps: (((bool)(this.root).isOpen) && ((RawMenuAnchor)this.widget).consumeOutsideTaps), onTapOutside: (global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>)this.handleOutsideTap, child: new Builder(key: this._anchorKey, builder: ((global::System.Func<BuildContext, Widget>)((context) =>
        {
            return (((((RawMenuAnchor)this.widget).builder is null ? ((RawMenuAnchor)this.widget).child : ((RawMenuAnchor)this.widget).builder.Invoke(context, this.menuController, ((RawMenuAnchor)this.widget).child))) ?? new SizedBox());
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))))));
        return ((Widget)OverlayPortal.CreateOverlayChildLayoutBuilder(controller: this._overlayController, overlayChildBuilder: this._buildOverlay, overlayLocation: (this.useRootOverlay ? OverlayChildLocation.rootOverlay : OverlayChildLocation.nearestOverlay), child: childLocal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString(global::Doroti.Framework.Foundation.DiagnosticLevel minLevel = default!)
    {
        return DiagnosticsLibrary.describeIdentity(this);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool isRoot => DartRuntimePrimitives.ConvertValue<bool>((this._parent is null));
    public virtual _RawMenuAnchorBaseMixin__raw_menu_anchor root
    {
        get
        {
            _RawMenuAnchorBaseMixin__raw_menu_anchor anchor = this;
            while ((anchor._parent is not null))
            {
                anchor = anchor._parent!;
            }
            return anchor;
        }
    }
    public override void initState()
    {
        base.initState();
        this.menuController._attach(this);
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _RawMenuAnchorBaseMixin__raw_menu_anchor? newParent = ((_RawMenuAnchorBaseMixin__raw_menu_anchor?)MenuController.maybeOf(this.context)?._anchor);
        if ((!Equals(newParent, this._parent)))
        {
            DartRuntimePrimitives.Assert(() => (!Equals(newParent, this)), () => (object?)"A MenuController should only be attached to one anchor at a time.");
            (this._parent)?._removeChild(this);
            this._parent = newParent;
            (this._parent)?._addChild(this);
        }
        if (this.isRoot)
        {
            this._scrollPosition?.isScrollingNotifier.removeListener(this._handleScroll);
            this._scrollPosition = Scrollable.maybeOf(this.context)?.position;
            this._scrollPosition?.isScrollingNotifier.addListener(this._handleScroll);
            global::Doroti.Ui.Size newSize = ((global::Doroti.Ui.Size)MediaQuery.sizeOf(this.context));
            if ((((this._viewSize is not null) && (!Equals(newSize, this._viewSize))) && this.isOpen))
            {
                handleCloseRequest();
            }
            this._viewSize = newSize;
        }
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Disposing of {this}"));
        if (this.isOpen)
        {
            close(inDispose: true);
        }
        if (this.isRoot)
        {
            this._scrollPosition?.isScrollingNotifier.removeListener(this._handleScroll);
            this._scrollPosition = null;
        }
        (this._parent)?._removeChild(this);
        this._parent = null;
        this._anchorChildren.Clear();
        this.menuController._detach(this);
        base.dispose();
    }

    public virtual void _addChild(_RawMenuAnchorBaseMixin__raw_menu_anchor child)
    {
        DartRuntimePrimitives.Assert(() => (this.isRoot || Raw_menu_anchorLibrary._debugMenuInfo($"Added root child: {child}")));
        DartRuntimePrimitives.Assert(() => !this._anchorChildren.Contains(child));
        this._anchorChildren.Add(child);
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Added:\n{((string)(((StatefulWidget)(child).widget)).toStringDeep())}"));
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Tree:\n{((string)(this.widget).toStringDeep())}"));
    }

    public virtual void _removeChild(_RawMenuAnchorBaseMixin__raw_menu_anchor child)
    {
        DartRuntimePrimitives.Assert(() => (this.isRoot || Raw_menu_anchorLibrary._debugMenuInfo($"Removed root child: {child}")));
        DartRuntimePrimitives.Assert(() => this._anchorChildren.Contains(child));
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Removing:\n{((string)(((StatefulWidget)(child).widget)).toStringDeep())}"));
        this._anchorChildren.Remove(child);
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Tree:\n{((string)(this.widget).toStringDeep())}"));
    }

    public virtual void _handleScroll()
    {
        if (this.isOpen)
        {
            handleCloseRequest();
        }
    }

    public virtual void _childChangedOpenState()
    {
        (this._parent)?._childChangedOpenState();
        if ((!Equals(Scheduler.SchedulerBinding.instance.schedulerPhase, Scheduler.SchedulerPhase.persistentCallbacks)))
        {
            setState(((global::System.Action)(() =>
            {
            })));
        }
        else
        {
            Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timestamp) =>
            {
                setState(((global::System.Action)(() =>
                {
                })));
            })));
        }
    }

    public virtual void closeChildren(bool inDispose = false)
    {
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Closing children of {this}{(inDispose ? " (dispose)" : "")}"));
        var children = new List<_RawMenuAnchorBaseMixin__raw_menu_anchor>(DartRuntimePrimitives.ConvertEnumerable<_RawMenuAnchorBaseMixin__raw_menu_anchor>(this._anchorChildren));
        foreach (var child in children)
        {
            (child).close(inDispose: inDispose);
        }
    }

    public virtual void requestChildrenClose()
    {
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Calling handleCloseRequest for children of {this}"));
        var children = new List<_RawMenuAnchorBaseMixin__raw_menu_anchor>(DartRuntimePrimitives.ConvertEnumerable<_RawMenuAnchorBaseMixin__raw_menu_anchor>(this._anchorChildren));
        foreach (var child in children)
        {
            (child).handleCloseRequest();
        }
    }

    public virtual void handleOutsideTap(global::Doroti.Framework.Gestures.PointerDownEvent pointerDownEvent)
    {
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Tapped Outside {this.menuController}"));
        if (this.isOpen)
        {
            requestChildrenClose();
        }
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)new _MenuControllerScope__raw_menu_anchor(isOpen: this.isOpen, controller: this.menuController, child: new Actions(actions: this.isOpen ? new DartMap<Type, dynamic> { [typeof(DismissIntent)] = new DismissMenuAction(this.menuController) } : new DartMap<Type, dynamic>(), child: new Builder(builder: (global::System.Func<BuildContext, Widget>)this.buildAnchor))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class RawMenuAnchorGroup : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual MenuController controller { get; private set; } = default!;

    public RawMenuAnchorGroup(global::Doroti.Framework.Foundation.Key? key = null, Widget child = default!, MenuController controller = default!) : base(key: key)
    {
        this.child = child;
        this.controller = controller;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(ObjectFlagProperty<MenuController>.CreateHas("controller", this.controller));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _RawMenuAnchorGroupState__raw_menu_anchor());
}

internal class _RawMenuAnchorGroupState__raw_menu_anchor : State<RawMenuAnchorGroup>, _RawMenuAnchorBaseMixin__raw_menu_anchor
{
    public virtual List<_RawMenuAnchorBaseMixin__raw_menu_anchor> _anchorChildren { get; set; } = new List<_RawMenuAnchorBaseMixin__raw_menu_anchor>();
    public virtual _RawMenuAnchorBaseMixin__raw_menu_anchor? _parent { get; set; } = default!;
    public virtual ScrollPosition? _scrollPosition { get; set; } = default;
    public virtual Size? _viewSize { get; set; } = default;

    public virtual bool isOpen => this._anchorChildren.any(((child) => ((bool)(child).isOpen)));
    public virtual MenuController menuController => ((RawMenuAnchorGroup)this.widget).controller;
    public override void didUpdateWidget(RawMenuAnchorGroup oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!Equals(((RawMenuAnchorGroup)oldWidget).controller, ((RawMenuAnchorGroup)this.widget).controller)))
        {
            ((RawMenuAnchorGroup)oldWidget).controller._detach(this);
            ((RawMenuAnchorGroup)this.widget).controller._attach(this);
        }
    }

    public virtual void close(bool inDispose = false)
    {
        if (!this.isOpen)
        {
            return;
        }
        closeChildren(inDispose: inDispose);
        if (!inDispose)
        {
            if ((!Equals(Scheduler.SchedulerBinding.instance.schedulerPhase, Scheduler.SchedulerPhase.persistentCallbacks)))
            {
                setState(((global::System.Action)(() =>
                {
                })));
            }
            else
            {
                Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timestamp) =>
                {
                    if (this.mounted)
                    {
                        setState(((global::System.Action)(() =>
                        {
                        })));
                    }
                })));
            }
        }
    }

    public virtual void open(Offset? position = null)
    {
        DartRuntimePrimitives.Assert(() => (Equals(((MenuController)this.menuController)._anchor, this)));
        return;
    }

    public virtual void handleCloseRequest()
    {
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Requesting close {this}"));
        requestChildrenClose();
    }

    public virtual void handleOpenRequest(Offset? position = null)
    {
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Requesting open {this}"));
        open(position: position);
    }

    public virtual Widget buildAnchor(BuildContext context)
    {
        return ((Widget)new TapRegion(groupId: ((MenuController)(this.root).menuController), onTapOutside: (global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>)this.handleOutsideTap, child: ((RawMenuAnchorGroup)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool isRoot => DartRuntimePrimitives.ConvertValue<bool>((this._parent is null));
    public virtual _RawMenuAnchorBaseMixin__raw_menu_anchor root
    {
        get
        {
            _RawMenuAnchorBaseMixin__raw_menu_anchor anchor = this;
            while ((anchor._parent is not null))
            {
                anchor = anchor._parent!;
            }
            return anchor;
        }
    }
    public override void initState()
    {
        base.initState();
        this.menuController._attach(this);
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _RawMenuAnchorBaseMixin__raw_menu_anchor? newParent = ((_RawMenuAnchorBaseMixin__raw_menu_anchor?)MenuController.maybeOf(this.context)?._anchor);
        if ((!Equals(newParent, this._parent)))
        {
            DartRuntimePrimitives.Assert(() => (!Equals(newParent, this)), () => (object?)"A MenuController should only be attached to one anchor at a time.");
            (this._parent)?._removeChild(this);
            this._parent = newParent;
            (this._parent)?._addChild(this);
        }
        if (this.isRoot)
        {
            this._scrollPosition?.isScrollingNotifier.removeListener(this._handleScroll);
            this._scrollPosition = Scrollable.maybeOf(this.context)?.position;
            this._scrollPosition?.isScrollingNotifier.addListener(this._handleScroll);
            global::Doroti.Ui.Size newSize = ((global::Doroti.Ui.Size)MediaQuery.sizeOf(this.context));
            if ((((this._viewSize is not null) && (!Equals(newSize, this._viewSize))) && this.isOpen))
            {
                handleCloseRequest();
            }
            this._viewSize = newSize;
        }
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Disposing of {this}"));
        if (this.isOpen)
        {
            close(inDispose: true);
        }
        if (this.isRoot)
        {
            this._scrollPosition?.isScrollingNotifier.removeListener(this._handleScroll);
            this._scrollPosition = null;
        }
        (this._parent)?._removeChild(this);
        this._parent = null;
        this._anchorChildren.Clear();
        this.menuController._detach(this);
        base.dispose();
    }

    public virtual void _addChild(_RawMenuAnchorBaseMixin__raw_menu_anchor child)
    {
        DartRuntimePrimitives.Assert(() => (this.isRoot || Raw_menu_anchorLibrary._debugMenuInfo($"Added root child: {child}")));
        DartRuntimePrimitives.Assert(() => !this._anchorChildren.Contains(child));
        this._anchorChildren.Add(child);
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Added:\n{((string)(((StatefulWidget)(child).widget)).toStringDeep())}"));
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Tree:\n{((string)(this.widget).toStringDeep())}"));
    }

    public virtual void _removeChild(_RawMenuAnchorBaseMixin__raw_menu_anchor child)
    {
        DartRuntimePrimitives.Assert(() => (this.isRoot || Raw_menu_anchorLibrary._debugMenuInfo($"Removed root child: {child}")));
        DartRuntimePrimitives.Assert(() => this._anchorChildren.Contains(child));
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Removing:\n{((string)(((StatefulWidget)(child).widget)).toStringDeep())}"));
        this._anchorChildren.Remove(child);
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Tree:\n{((string)(this.widget).toStringDeep())}"));
    }

    public virtual void _handleScroll()
    {
        if (this.isOpen)
        {
            handleCloseRequest();
        }
    }

    public virtual void _childChangedOpenState()
    {
        (this._parent)?._childChangedOpenState();
        if ((!Equals(Scheduler.SchedulerBinding.instance.schedulerPhase, Scheduler.SchedulerPhase.persistentCallbacks)))
        {
            setState(((global::System.Action)(() =>
            {
            })));
        }
        else
        {
            Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timestamp) =>
            {
                setState(((global::System.Action)(() =>
                {
                })));
            })));
        }
    }

    public virtual void closeChildren(bool inDispose = false)
    {
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Closing children of {this}{(inDispose ? " (dispose)" : "")}"));
        var children = new List<_RawMenuAnchorBaseMixin__raw_menu_anchor>(DartRuntimePrimitives.ConvertEnumerable<_RawMenuAnchorBaseMixin__raw_menu_anchor>(this._anchorChildren));
        foreach (var child in children)
        {
            (child).close(inDispose: inDispose);
        }
    }

    public virtual void requestChildrenClose()
    {
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Calling handleCloseRequest for children of {this}"));
        var children = new List<_RawMenuAnchorBaseMixin__raw_menu_anchor>(DartRuntimePrimitives.ConvertEnumerable<_RawMenuAnchorBaseMixin__raw_menu_anchor>(this._anchorChildren));
        foreach (var child in children)
        {
            (child).handleCloseRequest();
        }
    }

    public virtual void handleOutsideTap(global::Doroti.Framework.Gestures.PointerDownEvent pointerDownEvent)
    {
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Tapped Outside {this.menuController}"));
        if (this.isOpen)
        {
            requestChildrenClose();
        }
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)new _MenuControllerScope__raw_menu_anchor(isOpen: this.isOpen, controller: this.menuController, child: new Actions(actions: this.isOpen ? new DartMap<Type, dynamic> { [typeof(DismissIntent)] = new DismissMenuAction(this.menuController) } : new DartMap<Type, dynamic>(), child: new Builder(builder: (global::System.Func<BuildContext, Widget>)this.buildAnchor))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString(global::Doroti.Framework.Foundation.DiagnosticLevel minLevel = default!) => DiagnosticsLibrary.describeIdentity(this);
}

public class MenuController
{
    internal virtual _RawMenuAnchorBaseMixin__raw_menu_anchor? _anchor { get; set; } = default!;

    public virtual bool isOpen => DartRuntimePrimitives.ConvertValue<bool>((((bool?)(this._anchor)?.isOpen) ?? false));
    public virtual void open(Offset? position = null)
    {
        DartRuntimePrimitives.Assert(() => (this._anchor is not null));
        (this._anchor!).handleOpenRequest(position: position);
    }

    public virtual void close()
    {
        (this._anchor)?.handleCloseRequest();
    }

    public virtual void closeChildren()
    {
        DartRuntimePrimitives.Assert(() => (this._anchor is not null));
        (this._anchor!).requestChildrenClose();
    }

    internal virtual void _attach(_RawMenuAnchorBaseMixin__raw_menu_anchor anchor)
    {
        _anchor = anchor;
    }

    internal virtual void _detach(_RawMenuAnchorBaseMixin__raw_menu_anchor anchor)
    {
        if ((Equals(this._anchor, anchor)))
        {
            _anchor = null;
        }
    }

    public static MenuController? maybeOf(BuildContext context)
    {
        return context.getInheritedWidgetOfExactType<_MenuControllerScope__raw_menu_anchor>()?.controller;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool? maybeIsOpenOf(BuildContext context)
    {
        return context.dependOnInheritedWidgetOfExactType<_MenuControllerScope__raw_menu_anchor>()?.isOpen;
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
        (((MenuController)this.controller)._anchor!.root).handleCloseRequest();
        return null;
    }

    public override bool isEnabled(DismissIntent intent, BuildContext? context = null)
    {
        return (((MenuController)this.controller)._anchor is not null);
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
                    if (((details is not null) && Enumerable.Any(details)))
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
