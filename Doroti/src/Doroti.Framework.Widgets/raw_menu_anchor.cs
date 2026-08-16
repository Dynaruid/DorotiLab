// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/raw_menu_anchor.dart
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

public static partial class Raw_menu_anchorLibrary
{
    internal static bool _kDebugMenus = false;
}

public static partial class Raw_menu_anchorLibrary
{
    internal static DartMap<ShortcutActivator, Intent> _kMenuTraversalShortcuts = new DartMap<ShortcutActivator, Intent> { [new SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.gameButtonA)] = ((Intent)(object?)new ActivateIntent()), [new SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.escape)] = ((Intent)(object?)new DismissIntent()), [new SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.arrowDown)] = ((Intent)(object?)new DirectionalFocusIntent(TraversalDirection.down)), [new SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.arrowUp)] = ((Intent)(object?)new DirectionalFocusIntent(TraversalDirection.up)), [new SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.arrowLeft)] = ((Intent)(object?)new DirectionalFocusIntent(TraversalDirection.left)), [new SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.arrowRight)] = ((Intent)(object?)new DirectionalFocusIntent(TraversalDirection.right)) };
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
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((__other is RawMenuOverlayInfo) && (object.Equals(((RawMenuOverlayInfo)((RawMenuOverlayInfo)__other)).anchorRect, this.anchorRect))) && (object.Equals(((RawMenuOverlayInfo)((RawMenuOverlayInfo)__other)).overlaySize, this.overlaySize))) && (object.Equals(((RawMenuOverlayInfo)((RawMenuOverlayInfo)__other)).position, this.position))) && (object.Equals(((RawMenuOverlayInfo)((RawMenuOverlayInfo)__other)).tapRegionGroupId, this.tapRegionGroupId)));
    }

    public override int GetHashCode()
    {
        return FoundationRuntimePorts.ObjectHash(this.anchorRect, this.overlaySize, this.position, this.tapRegionGroupId);
        return default!;
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
        var __oldWidget = (_MenuControllerScope__raw_menu_anchor)(object)oldWidget;
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
        properties.add(global::Doroti.Framework.Foundation.ObjectFlagProperty<FocusNode>.CreateHas("focusNode", this.childFocusNode));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("useRootOverlay", value: this.useRootOverlay, ifFalse: "use nearest overlay", ifTrue: "use root overlay"));
    }

}

internal interface _RawMenuAnchorBaseMixin__raw_menu_anchor<T> where T : StatefulWidget
{
    List<dynamic> _anchorChildren { get; }
    dynamic _parent { get; set; }
    ScrollPosition? _scrollPosition { get; set; }
    Size? _viewSize { get; set; }

    public bool isRoot { get; }
    public MenuController menuController { get; }
    public bool isOpen { get; }
    public _RawMenuAnchorBaseMixin__raw_menu_anchor<T> root { get; }
    public void initState();
    public void didChangeDependencies();
    public void dispose();
    public void _addChild(dynamic child);
    public void _removeChild(dynamic child);
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
    public Widget build(BuildContext context);
    public string ToString(global::Doroti.Framework.Foundation.DiagnosticLevel minLevel = default!);
}

internal class _RawMenuAnchorState__raw_menu_anchor : State<RawMenuAnchor>, _RawMenuAnchorBaseMixin__raw_menu_anchor<RawMenuAnchor>
{
    internal virtual GlobalKey<_RawMenuAnchorState__raw_menu_anchor> _anchorKey { get; private set; } = GlobalKey<_RawMenuAnchorState__raw_menu_anchor>.Create(debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode ? null : "MenuAnchor"));
    internal virtual OverlayPortalController _overlayController { get; private set; } = new OverlayPortalController(debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode ? null : "MenuAnchor controller"));
    internal virtual Offset? _menuPosition { get; set; } = default;
    public virtual List<dynamic> _anchorChildren { get; set; } = new List<_RawMenuAnchorBaseMixin__raw_menu_anchor<StatefulWidget>>().Cast<dynamic>().ToList();
    public virtual dynamic _parent { get; set; } = default!;
    public virtual ScrollPosition? _scrollPosition { get; set; } = default;
    public virtual Size? _viewSize { get; set; } = default;

    internal virtual bool _isRootOverlayAnchor => (this._parent is not _RawMenuAnchorState__raw_menu_anchor);
    public virtual bool useRootOverlay
    {
        get
        {
            if (this._parent is _RawMenuAnchorState__raw_menu_anchor { useRootOverlay: bool useRoot__26382 } __object26335)
            {
                return useRoot__26382;
            }
            DartRuntimePrimitives.Assert(() => this._isRootOverlayAnchor);
            return ((RawMenuAnchor)(object)this.widget).useRootOverlay;
            return default!;
        }
    }
    public virtual bool isOpen => ((OverlayPortalController)this._overlayController).isShowing;
    public virtual MenuController menuController => ((RawMenuAnchor)(object)this.widget).controller;
    public override void didUpdateWidget(RawMenuAnchor oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((RawMenuAnchor)oldWidget).controller, ((RawMenuAnchor)(object)this.widget).controller)))
        {
            ((RawMenuAnchor)oldWidget).controller._detach(this);
            ((RawMenuAnchor)(object)this.widget).controller._attach(this);
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
        ((dynamic)this._parent)?.requestChildrenClose();
        DartRuntimePrimitives.Assert(() => !((OverlayPortalController)this._overlayController).isShowing);
        _menuPosition = position;
        ((dynamic)this._parent)?._childChangedOpenState();
        this._overlayController.show();
        if (this._isRootOverlayAnchor)
        {
            ((RawMenuAnchor)(object)this.widget).childFocusNode?.requestFocus();
        }
        ((RawMenuAnchor)(object)this.widget).onOpen?.Invoke();
        setState(((global::System.Action)(() => {
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
        if ((!object.Equals(global::Doroti.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase, global::Doroti.Framework.Scheduler.SchedulerPhase.persistentCallbacks)))
        {
            this._overlayController.hide();
        }
        else
        {
            if (!inDispose)
            {
                global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) => {
this._overlayController.hide();
})), debugLabel: "MenuAnchor.hide");
            }
        }
        if (!inDispose)
        {
            ((dynamic)this._parent)?._childChangedOpenState();
            ((RawMenuAnchor)(object)this.widget).onClose?.Invoke();
            if ((this.mounted && (!object.Equals(global::Doroti.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase, global::Doroti.Framework.Scheduler.SchedulerPhase.persistentCallbacks))))
            {
                setState(((global::System.Action)(() => {
})));
            }
        }
    }

    public virtual void handleOpenRequest(Offset? position = null)
    {
        this.widget.onOpenRequested(position, (() => {
open(position: position);
}));
    }

    public virtual void handleCloseRequest()
    {
        if ((!object.Equals(global::Doroti.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase, global::Doroti.Framework.Scheduler.SchedulerPhase.persistentCallbacks)))
        {
            this.widget.onCloseRequested(() => this.close(default));
        }
        else
        {
            global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) => {
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
        Matrix4 transform__29728 = ((OverlayChildLayoutInfo)layoutInfo).childPaintTransform;
        global::Doroti.Ui.Size anchorSize__29787 = ((global::Doroti.Ui.Size)(object?)((OverlayChildLayoutInfo)layoutInfo).childSize);
        global::Doroti.Ui.Rect anchorRect__29909 = ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(transform__29728, (Offset.zero & anchorSize__29787)));
        var info__29997 = new RawMenuOverlayInfo(anchorRect: anchorRect__29909, overlaySize: ((OverlayChildLayoutInfo)layoutInfo).overlaySize, position: this._menuPosition, tapRegionGroupId: ((MenuController)((dynamic)this.root).menuController));
        return this.widget.overlayBuilder(context, info__29997);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Widget buildAnchor(BuildContext context)
    {
        Widget child__30309 = ((Widget)(object?)new Shortcuts(includeSemantics: false, shortcuts: Raw_menu_anchorLibrary._kMenuTraversalShortcuts, child: new TapRegion(groupId: ((MenuController)((dynamic)this.root).menuController), consumeOutsideTaps: (((bool)((dynamic)this.root).isOpen) && ((RawMenuAnchor)(object)this.widget).consumeOutsideTaps), onTapOutside: (global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>)this.handleOutsideTap, child: new Builder(key: this._anchorKey, builder: ((global::System.Func<BuildContext, Widget>)((context) => {
return (((((RawMenuAnchor)(object)this.widget).builder is null ? ((RawMenuAnchor)(object)this.widget).child : ((RawMenuAnchor)(object)this.widget).builder.Invoke(context, this.menuController, ((RawMenuAnchor)(object)this.widget).child))) ?? new SizedBox());
throw new InvalidOperationException("Dart closure completed without a value.");
}))))));
        return ((Widget)(object?)OverlayPortal.CreateOverlayChildLayoutBuilder(controller: this._overlayController, overlayChildBuilder: this._buildOverlay, overlayLocation: (this.useRootOverlay ? OverlayChildLocation.rootOverlay : OverlayChildLocation.nearestOverlay), child: child__30309));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string ToString(global::Doroti.Framework.Foundation.DiagnosticLevel minLevel = default!)
    {
        return global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool isRoot => DartRuntimePrimitives.ConvertValue<bool>((this._parent is null));
    public virtual _RawMenuAnchorBaseMixin__raw_menu_anchor<RawMenuAnchor> root
    {
        get
        {
            dynamic anchor__19171 = this;
            while ((((dynamic)((dynamic)anchor__19171)._parent) is not null))
            {
                anchor__19171 = ((dynamic)((dynamic)anchor__19171)._parent)!;
            }
            return anchor__19171;
            return default!;
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
        _RawMenuAnchorBaseMixin__raw_menu_anchor<StatefulWidget>? newParent__19495 = ((_RawMenuAnchorBaseMixin__raw_menu_anchor<StatefulWidget>?)(object?)MenuController.maybeOf(this.context)?._anchor);
        if ((!object.Equals(newParent__19495, this._parent)))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(newParent__19495, this)), () => (object?)"A MenuController should only be attached to one anchor at a time.");
            ((dynamic)this._parent)?._removeChild(this);
            this._parent = newParent__19495;
            ((dynamic)this._parent)?._addChild(this);
        }
        if (this.isRoot)
        {
            this._scrollPosition?.isScrollingNotifier.removeListener(() => this._handleScroll());
            this._scrollPosition = Scrollable.maybeOf(this.context)?.position;
            this._scrollPosition?.isScrollingNotifier.addListener(() => this._handleScroll());
            global::Doroti.Ui.Size newSize__20053 = ((global::Doroti.Ui.Size)(object?)MediaQuery.sizeOf(this.context));
            if ((((this._viewSize is not null) && (!object.Equals(newSize__20053, this._viewSize))) && this.isOpen))
            {
                handleCloseRequest();
            }
            this._viewSize = newSize__20053;
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
            this._scrollPosition?.isScrollingNotifier.removeListener(() => this._handleScroll());
            this._scrollPosition = null;
        }
        ((dynamic)this._parent)?._removeChild(this);
        this._parent = null;
        this._anchorChildren.Clear();
        this.menuController._detach(this);
        base.dispose();
    }

    public virtual void _addChild(dynamic child)
    {
        DartRuntimePrimitives.Assert(() => (this.isRoot || Raw_menu_anchorLibrary._debugMenuInfo($"Added root child: {child}")));
        DartRuntimePrimitives.Assert(() => !this._anchorChildren.Contains(child));
        this._anchorChildren.Add(child);
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Added:\n{((string)((dynamic)((StatefulWidget)((dynamic)child).widget)).toStringDeep())}"));
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Tree:\n{((string)((dynamic)this.widget).toStringDeep())}"));
    }

    public virtual void _removeChild(dynamic child)
    {
        DartRuntimePrimitives.Assert(() => (this.isRoot || Raw_menu_anchorLibrary._debugMenuInfo($"Removed root child: {child}")));
        DartRuntimePrimitives.Assert(() => this._anchorChildren.Contains(child));
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Removing:\n{((string)((dynamic)((StatefulWidget)((dynamic)child).widget)).toStringDeep())}"));
        this._anchorChildren.Remove(child);
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Tree:\n{((string)((dynamic)this.widget).toStringDeep())}"));
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
        ((dynamic)this._parent)?._childChangedOpenState();
        if ((!object.Equals(global::Doroti.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase, global::Doroti.Framework.Scheduler.SchedulerPhase.persistentCallbacks)))
        {
            setState(((global::System.Action)(() => {
})));
        }
        else
        {
            global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timestamp) => {
setState(((global::System.Action)(() => {
})));
})));
        }
    }

    public virtual void closeChildren(bool inDispose = false)
    {
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Closing children of {this}{(inDispose ? " (dispose)" : "")}"));
        var children__23916 = new List<_RawMenuAnchorBaseMixin__raw_menu_anchor<StatefulWidget>>(DartRuntimePrimitives.ConvertEnumerable<_RawMenuAnchorBaseMixin__raw_menu_anchor<StatefulWidget>>(this._anchorChildren));
        foreach (var child__23993 in children__23916)
        {
            ((dynamic)child__23993).close(inDispose: inDispose);
        }
    }

    public virtual void requestChildrenClose()
    {
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Calling handleCloseRequest for children of {this}"));
        var children__24538 = new List<_RawMenuAnchorBaseMixin__raw_menu_anchor<StatefulWidget>>(DartRuntimePrimitives.ConvertEnumerable<_RawMenuAnchorBaseMixin__raw_menu_anchor<StatefulWidget>>(this._anchorChildren));
        foreach (var child__24615 in children__24538)
        {
            ((dynamic)child__24615).handleCloseRequest();
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
        return ((Widget)(object?)new _MenuControllerScope__raw_menu_anchor(isOpen: this.isOpen, controller: this.menuController, child: new Actions(actions: new DartMap<Type, dynamic>(), child: new Builder(builder: (global::System.Func<BuildContext, Widget>)this.buildAnchor))));
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
        properties.add(global::Doroti.Framework.Foundation.ObjectFlagProperty<MenuController>.CreateHas("controller", this.controller));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _RawMenuAnchorGroupState__raw_menu_anchor());
}

internal class _RawMenuAnchorGroupState__raw_menu_anchor : State<RawMenuAnchorGroup>, _RawMenuAnchorBaseMixin__raw_menu_anchor<RawMenuAnchorGroup>
{
    public virtual List<dynamic> _anchorChildren { get; set; } = new List<_RawMenuAnchorBaseMixin__raw_menu_anchor<StatefulWidget>>().Cast<dynamic>().ToList();
    public virtual dynamic _parent { get; set; } = default!;
    public virtual ScrollPosition? _scrollPosition { get; set; } = default;
    public virtual Size? _viewSize { get; set; } = default;

    public virtual bool isOpen => this._anchorChildren.any(((child) => ((bool)((dynamic)child).isOpen)));
    public virtual MenuController menuController => ((RawMenuAnchorGroup)(object)this.widget).controller;
    public override void didUpdateWidget(RawMenuAnchorGroup oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((RawMenuAnchorGroup)oldWidget).controller, ((RawMenuAnchorGroup)(object)this.widget).controller)))
        {
            ((RawMenuAnchorGroup)oldWidget).controller._detach(this);
            ((RawMenuAnchorGroup)(object)this.widget).controller._attach(this);
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
            if ((!object.Equals(global::Doroti.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase, global::Doroti.Framework.Scheduler.SchedulerPhase.persistentCallbacks)))
            {
                setState(((global::System.Action)(() => {
})));
            }
            else
            {
                global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timestamp) => {
if (this.mounted)
{
    setState(((global::System.Action)(() => {
})));
}
})));
            }
        }
    }

    public virtual void open(Offset? position = null)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((MenuController)this.menuController)._anchor, this)));
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
        return ((Widget)(object?)new TapRegion(groupId: ((MenuController)((dynamic)this.root).menuController), onTapOutside: (global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>)this.handleOutsideTap, child: ((RawMenuAnchorGroup)(object)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool isRoot => DartRuntimePrimitives.ConvertValue<bool>((this._parent is null));
    public virtual _RawMenuAnchorBaseMixin__raw_menu_anchor<RawMenuAnchorGroup> root
    {
        get
        {
            dynamic anchor__19171 = this;
            while ((((dynamic)((dynamic)anchor__19171)._parent) is not null))
            {
                anchor__19171 = ((dynamic)((dynamic)anchor__19171)._parent)!;
            }
            return anchor__19171;
            return default!;
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
        _RawMenuAnchorBaseMixin__raw_menu_anchor<StatefulWidget>? newParent__19495 = ((_RawMenuAnchorBaseMixin__raw_menu_anchor<StatefulWidget>?)(object?)MenuController.maybeOf(this.context)?._anchor);
        if ((!object.Equals(newParent__19495, this._parent)))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(newParent__19495, this)), () => (object?)"A MenuController should only be attached to one anchor at a time.");
            ((dynamic)this._parent)?._removeChild(this);
            this._parent = newParent__19495;
            ((dynamic)this._parent)?._addChild(this);
        }
        if (this.isRoot)
        {
            this._scrollPosition?.isScrollingNotifier.removeListener(() => this._handleScroll());
            this._scrollPosition = Scrollable.maybeOf(this.context)?.position;
            this._scrollPosition?.isScrollingNotifier.addListener(() => this._handleScroll());
            global::Doroti.Ui.Size newSize__20053 = ((global::Doroti.Ui.Size)(object?)MediaQuery.sizeOf(this.context));
            if ((((this._viewSize is not null) && (!object.Equals(newSize__20053, this._viewSize))) && this.isOpen))
            {
                handleCloseRequest();
            }
            this._viewSize = newSize__20053;
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
            this._scrollPosition?.isScrollingNotifier.removeListener(() => this._handleScroll());
            this._scrollPosition = null;
        }
        ((dynamic)this._parent)?._removeChild(this);
        this._parent = null;
        this._anchorChildren.Clear();
        this.menuController._detach(this);
        base.dispose();
    }

    public virtual void _addChild(dynamic child)
    {
        DartRuntimePrimitives.Assert(() => (this.isRoot || Raw_menu_anchorLibrary._debugMenuInfo($"Added root child: {child}")));
        DartRuntimePrimitives.Assert(() => !this._anchorChildren.Contains(child));
        this._anchorChildren.Add(child);
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Added:\n{((string)((dynamic)((StatefulWidget)((dynamic)child).widget)).toStringDeep())}"));
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Tree:\n{((string)((dynamic)this.widget).toStringDeep())}"));
    }

    public virtual void _removeChild(dynamic child)
    {
        DartRuntimePrimitives.Assert(() => (this.isRoot || Raw_menu_anchorLibrary._debugMenuInfo($"Removed root child: {child}")));
        DartRuntimePrimitives.Assert(() => this._anchorChildren.Contains(child));
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Removing:\n{((string)((dynamic)((StatefulWidget)((dynamic)child).widget)).toStringDeep())}"));
        this._anchorChildren.Remove(child);
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Tree:\n{((string)((dynamic)this.widget).toStringDeep())}"));
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
        ((dynamic)this._parent)?._childChangedOpenState();
        if ((!object.Equals(global::Doroti.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase, global::Doroti.Framework.Scheduler.SchedulerPhase.persistentCallbacks)))
        {
            setState(((global::System.Action)(() => {
})));
        }
        else
        {
            global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timestamp) => {
setState(((global::System.Action)(() => {
})));
})));
        }
    }

    public virtual void closeChildren(bool inDispose = false)
    {
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Closing children of {this}{(inDispose ? " (dispose)" : "")}"));
        var children__23916 = new List<_RawMenuAnchorBaseMixin__raw_menu_anchor<StatefulWidget>>(DartRuntimePrimitives.ConvertEnumerable<_RawMenuAnchorBaseMixin__raw_menu_anchor<StatefulWidget>>(this._anchorChildren));
        foreach (var child__23993 in children__23916)
        {
            ((dynamic)child__23993).close(inDispose: inDispose);
        }
    }

    public virtual void requestChildrenClose()
    {
        DartRuntimePrimitives.Assert(() => Raw_menu_anchorLibrary._debugMenuInfo($"Calling handleCloseRequest for children of {this}"));
        var children__24538 = new List<_RawMenuAnchorBaseMixin__raw_menu_anchor<StatefulWidget>>(DartRuntimePrimitives.ConvertEnumerable<_RawMenuAnchorBaseMixin__raw_menu_anchor<StatefulWidget>>(this._anchorChildren));
        foreach (var child__24615 in children__24538)
        {
            ((dynamic)child__24615).handleCloseRequest();
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
        return ((Widget)(object?)new _MenuControllerScope__raw_menu_anchor(isOpen: this.isOpen, controller: this.menuController, child: new Actions(actions: new DartMap<Type, dynamic>(), child: new Builder(builder: (global::System.Func<BuildContext, Widget>)this.buildAnchor))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string ToString(global::Doroti.Framework.Foundation.DiagnosticLevel minLevel = default!) => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
}

public class MenuController
{
    internal virtual dynamic _anchor { get; set; } = default!;

    public virtual bool isOpen => DartRuntimePrimitives.ConvertValue<bool>((((bool?)((dynamic)this._anchor)?.isOpen) ?? false));
    public virtual void open(Offset? position = null)
    {
        DartRuntimePrimitives.Assert(() => (this._anchor is not null));
        ((dynamic)this._anchor!).handleOpenRequest(position: position);
    }

    public virtual void close()
    {
        ((dynamic)this._anchor)?.handleCloseRequest();
    }

    public virtual void closeChildren()
    {
        DartRuntimePrimitives.Assert(() => (this._anchor is not null));
        ((dynamic)this._anchor!).requestChildrenClose();
    }

    internal virtual void _attach(dynamic anchor)
    {
        _anchor = anchor;
    }

    internal virtual void _detach(dynamic anchor)
    {
        if ((object.Equals(this._anchor, anchor)))
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

    public override string ToString() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
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
        ((dynamic)((dynamic)((dynamic)((MenuController)this.controller)._anchor!).root)).handleCloseRequest();
        throw new InvalidOperationException("Dart control flow completed without a value.");
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
                if (Raw_menu_anchorLibrary._kDebugMenus)
                {
                    global::Doroti.Framework.Foundation.PrintLibrary.debugPrint($"MENU: {message}");
                    if (((details is not null) && System.Linq.Enumerable.Any(details)))
                    {
                        foreach (string detail__40260 in details)
                        {
                            global::Doroti.Framework.Foundation.PrintLibrary.debugPrint($"    {detail__40260}");
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
