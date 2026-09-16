// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/menu_anchor.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class Menu_anchorLibrary
{
    internal static bool _kDebugMenus = false;
}

public static partial class Menu_anchorLibrary
{
    internal static double _kDefaultSubmenuIconSize = 24;
}

public static partial class Menu_anchorLibrary
{
    internal static double _kLabelItemDefaultSpacing = 12;
}

public static partial class Menu_anchorLibrary
{
    internal static double _kLabelItemMinSpacing = 4;
}

public static partial class Menu_anchorLibrary
{
    internal static DartMap<global::Doroti.Framework.Widgets.ShortcutActivator, global::Doroti.Framework.Widgets.Intent> _kMenuTraversalShortcuts = new DartMap<global::Doroti.Framework.Widgets.ShortcutActivator, global::Doroti.Framework.Widgets.Intent> { [new global::Doroti.Framework.Widgets.SingleActivator(LogicalKeyboardKey.gameButtonA)] = ((global::Doroti.Framework.Widgets.Intent)new global::Doroti.Framework.Widgets.ActivateIntent()), [new global::Doroti.Framework.Widgets.SingleActivator(LogicalKeyboardKey.escape)] = ((global::Doroti.Framework.Widgets.Intent)new global::Doroti.Framework.Widgets.DismissIntent()), [new global::Doroti.Framework.Widgets.SingleActivator(LogicalKeyboardKey.tab)] = ((global::Doroti.Framework.Widgets.Intent)new global::Doroti.Framework.Widgets.NextFocusIntent()), [new global::Doroti.Framework.Widgets.SingleActivator(LogicalKeyboardKey.tab, shift: true)] = ((global::Doroti.Framework.Widgets.Intent)new global::Doroti.Framework.Widgets.PreviousFocusIntent()), [new global::Doroti.Framework.Widgets.SingleActivator(LogicalKeyboardKey.arrowDown)] = ((global::Doroti.Framework.Widgets.Intent)new global::Doroti.Framework.Widgets.DirectionalFocusIntent(TraversalDirection.down)), [new global::Doroti.Framework.Widgets.SingleActivator(LogicalKeyboardKey.arrowUp)] = ((global::Doroti.Framework.Widgets.Intent)new global::Doroti.Framework.Widgets.DirectionalFocusIntent(TraversalDirection.up)), [new global::Doroti.Framework.Widgets.SingleActivator(LogicalKeyboardKey.arrowLeft)] = ((global::Doroti.Framework.Widgets.Intent)new global::Doroti.Framework.Widgets.DirectionalFocusIntent(TraversalDirection.left)), [new global::Doroti.Framework.Widgets.SingleActivator(LogicalKeyboardKey.arrowRight)] = ((global::Doroti.Framework.Widgets.Intent)new global::Doroti.Framework.Widgets.DirectionalFocusIntent(TraversalDirection.right)) };
}

public static partial class Menu_anchorLibrary
{
    internal static double _kMenuVerticalMinPadding = 8;
}

public static partial class Menu_anchorLibrary
{
    internal static double _kMenuViewPadding = 8;
}

public static partial class Menu_anchorLibrary
{
    internal static double _kTopLevelMenuHorizontalMinPadding = 4;
}

public static partial class Menu_anchorLibrary
{
    internal static Duration _kMenuOpeningDuration = Duration.Create(milliseconds: 500L);
}

public static partial class Menu_anchorLibrary
{
    internal static Duration _kMenuClosingDuration = Duration.Create(milliseconds: 150L);
}

public static partial class Menu_anchorLibrary
{
    internal static global::Doroti.Framework.Animation.Curve _kMenuPanelHeightForwardCurve = ((global::Doroti.Framework.Animation.Curve)new global::Doroti.Framework.Animation.Cubic(0.3, 0, 0, 1));
}

public static partial class Menu_anchorLibrary
{
    internal static global::Doroti.Framework.Animation.Curve _kMenuPanelHeightReverseCurve = ((global::Doroti.Framework.Animation.Curve)new _TweenCurve__menu_anchor(0.35, 1, curve: new global::Doroti.Framework.Animation.FlippedCurve(Easing.emphasizedAccelerate)));
}

public static partial class Menu_anchorLibrary
{
    internal static global::Doroti.Framework.Animation.Curve _kMenuPanelOpacityForwardCurve = ((global::Doroti.Framework.Animation.Curve)new global::Doroti.Framework.Animation.Interval(0, (50L / 500L)));
}

public static partial class Menu_anchorLibrary
{
    internal static global::Doroti.Framework.Animation.Curve _kMenuPanelOpacityReverseCurve = ((global::Doroti.Framework.Animation.Curve)new global::Doroti.Framework.Animation.FlippedCurve(new global::Doroti.Framework.Animation.Interval((100L / 150L), (150L / 150L))));
}

public static partial class Menu_anchorLibrary
{
    internal static double _kMenuItemRelativeFadeInDuration = (1L / 2L);
}

public static partial class Menu_anchorLibrary
{
    internal static double _kMenuItemRelativeFadeOutDuration = (1L / 3L);
}

public static partial class Menu_anchorLibrary
{
    internal static double _kMenuItemRelativeFadeOutDelay = (1L / 3L);
}

public delegate global::Doroti.Framework.Widgets.Widget MenuAnchorChildBuilder(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.MenuController controller, global::Doroti.Framework.Widgets.Widget? child);

internal class _MenuAnchorScope__menu_anchor : global::Doroti.Framework.Widgets.InheritedWidget
{
    public virtual _MenuAnchorState__menu_anchor state { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.AnimationStatus animationStatus { get; private set; } = default!;

    internal _MenuAnchorScope__menu_anchor(_MenuAnchorState__menu_anchor state, global::Doroti.Framework.Animation.AnimationStatus animationStatus, global::Doroti.Framework.Widgets.Widget child) : base(child: child)
    {
        this.state = state;
        this.animationStatus = animationStatus;
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget)
    {
        var __oldWidget = (_MenuAnchorScope__menu_anchor)oldWidget;
        DartRuntimePrimitives.Assert(() => (Equals(((_MenuAnchorScope__menu_anchor)__oldWidget).state, this.state)), () => (object?)"The state of a MenuAnchor should not change.");
        return (!Equals(((_MenuAnchorScope__menu_anchor)__oldWidget).animationStatus, this.animationStatus));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TweenCurve__menu_anchor : global::Doroti.Framework.Animation.Curve
{
    public virtual double begin { get; private set; } = default!;
    public virtual double end { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Curve curve { get; private set; } = default!;

    internal _TweenCurve__menu_anchor(double begin, double end, global::Doroti.Framework.Animation.Curve curve)
    {
        this.begin = begin;
        this.end = end;
        this.curve = curve;
        System.Diagnostics.Debug.Assert((begin >= 0.0));
        System.Diagnostics.Debug.Assert((begin <= 1.0));
        System.Diagnostics.Debug.Assert((end >= 0.0));
        System.Diagnostics.Debug.Assert((end <= 1.0));
        System.Diagnostics.Debug.Assert((end >= begin));
    }

    public override double transformInternal(double t)
    {
        t = this.curve.transform(t);
        return DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(this.begin, this.end, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => $"_TweenCurve({this.begin}, {this.end}, {this.curve})";
}

public class MenuAnchor : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Widgets.MenuController? controller { get; private set; }
    public virtual global::Doroti.Framework.Widgets.FocusNode? childFocusNode { get; private set; }
    public virtual MenuStyle? style { get; private set; }
    public virtual Offset? alignmentOffset { get; private set; }
    public virtual global::Doroti.Framework.Rendering.LayerLink? layerLink { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual bool anchorTapClosesMenu { get; private set; } = default!;
    public virtual bool consumeOutsideTap { get; private set; } = default!;
    public virtual global::System.Action? onOpen { get; private set; }
    public virtual global::System.Action? onClose { get; private set; }
    public virtual bool crossAxisUnconstrained { get; private set; } = default!;
    public virtual bool useRootOverlay { get; private set; } = default!;
    public virtual bool animated { get; private set; } = default!;
    public virtual AnimationStatusListener? onAnimationStatusChanged { get; private set; }
    public virtual List<global::Doroti.Framework.Widgets.Widget> menuChildren { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.MenuController, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>? builder { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? reservedPadding { get; private set; }

    public MenuAnchor(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.MenuController? controller = null, global::Doroti.Framework.Widgets.FocusNode? childFocusNode = null, MenuStyle? style = null, Offset? alignmentOffset = default, global::Doroti.Framework.Painting.EdgeInsetsGeometry? reservedPadding = null, global::Doroti.Framework.Rendering.LayerLink? layerLink = null, Clip clipBehavior = Clip.hardEdge, bool anchorTapClosesMenu = false, bool consumeOutsideTap = false, global::System.Action? onOpen = null, global::System.Action? onClose = null, bool crossAxisUnconstrained = true, bool useRootOverlay = false, bool animated = false, AnimationStatusListener? onAnimationStatusChanged = null, List<global::Doroti.Framework.Widgets.Widget> menuChildren = default!, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.MenuController, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>? builder = null, global::Doroti.Framework.Widgets.Widget? child = null) : base(key: key)
    {
        this.controller = controller;
        this.childFocusNode = childFocusNode;
        this.style = style;
        this.alignmentOffset = alignmentOffset;
        this.reservedPadding = reservedPadding;
        this.layerLink = layerLink;
        this.clipBehavior = clipBehavior;
        this.anchorTapClosesMenu = anchorTapClosesMenu;
        this.consumeOutsideTap = consumeOutsideTap;
        this.onOpen = onOpen;
        this.onClose = onClose;
        this.crossAxisUnconstrained = crossAxisUnconstrained;
        this.useRootOverlay = useRootOverlay;
        this.animated = animated;
        this.onAnimationStatusChanged = onAnimationStatusChanged;
        this.menuChildren = menuChildren;
        this.builder = builder;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _MenuAnchorState__menu_anchor());
    public override List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        return this.menuChildren.map<global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Foundation.DiagnosticsNode>(((child) => ((Diagnosticable)child).toDiagnosticsNode())).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("anchorTapClosesMenu", value: this.anchorTapClosesMenu, ifTrue: "AUTO-CLOSE"));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.FocusNode?>("focusNode", this.childFocusNode));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<MenuStyle?>("style", this.style));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Ui.Clip>("clipBehavior", this.clipBehavior));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Offset?>("alignmentOffset", this.alignmentOffset));
    }

}

internal class _MenuAnchorState__menu_anchor : global::Doroti.Framework.Widgets.State<MenuAnchor>, global::Doroti.Framework.Widgets.SingleTickerProviderStateMixin<MenuAnchor>
{
    internal virtual global::Doroti.Framework.Widgets.MenuController? _internalMenuController { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.FocusScopeNode _menuScopeNode { get; private set; } = new global::Doroti.Framework.Widgets.FocusScopeNode();
    private bool __late__animationController_initialized;
    private global::Doroti.Framework.Animation.AnimationController __late__animationController = default!;
    internal virtual global::Doroti.Framework.Animation.AnimationController _animationController
    {
        get
        {
            if (!__late__animationController_initialized)
            {
                __late__animationController = new global::Doroti.Framework.Animation.AnimationController(vsync: this);
                __late__animationController_initialized = true;
            }
            return __late__animationController;
        }
    }
    private bool __late_heightAnimation_initialized;
    private global::Doroti.Framework.Animation.CurvedAnimation __late_heightAnimation = default!;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation heightAnimation
    {
        get
        {
            if (!__late_heightAnimation_initialized)
            {
                __late_heightAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this._animationController, curve: Menu_anchorLibrary._kMenuPanelHeightForwardCurve, reverseCurve: Menu_anchorLibrary._kMenuPanelHeightReverseCurve);
                __late_heightAnimation_initialized = true;
            }
            return __late_heightAnimation;
        }
    }
    private bool __late_opacityAnimation_initialized;
    private global::Doroti.Framework.Animation.CurvedAnimation __late_opacityAnimation = default!;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation opacityAnimation
    {
        get
        {
            if (!__late_opacityAnimation_initialized)
            {
                __late_opacityAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this._animationController, curve: Menu_anchorLibrary._kMenuPanelOpacityForwardCurve, reverseCurve: Menu_anchorLibrary._kMenuPanelOpacityReverseCurve);
                __late_opacityAnimation_initialized = true;
            }
            return __late_opacityAnimation;
        }
    }
    internal virtual List<global::Doroti.Framework.Widgets.Widget> _menuChildren { get; set; } = new List<global::Doroti.Framework.Widgets.Widget>();
    internal virtual List<global::Doroti.Framework.Animation.CurvedAnimation> _cachedAnimations { get; set; } = new List<global::Doroti.Framework.Animation.CurvedAnimation>();
    public virtual global::Doroti.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual global::Doroti.Framework.Painting.Axis _orientation => Axis.vertical;
    internal virtual global::Doroti.Framework.Widgets.MenuController _menuController => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.MenuController>((((MenuAnchor)this.widget).controller ?? this._internalMenuController!));
    internal virtual _MenuAnchorState__menu_anchor? _parent => _maybeOf(this.context);
    public virtual bool isSubmenu => DartRuntimePrimitives.ConvertValue<bool>((MenuController.maybeOf(this.context) is not null));
    public virtual bool isClosingOrClosed => (((global::Doroti.Framework.Animation.AnimationController)this._animationController).status switch { AnimationStatus.dismissed => true, AnimationStatus.reverse => true, AnimationStatus.forward => false, AnimationStatus.completed => false, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    public virtual bool isClosing => (((global::Doroti.Framework.Animation.AnimationController)this._animationController).status switch { AnimationStatus.reverse => true, AnimationStatus.dismissed or AnimationStatus.forward => false, AnimationStatus.completed => false, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    public override void initState()
    {
        base.initState();
        _resolveAnimationController();
        _resolveMenuItems();
        this._animationController.addStatusListener((AnimationStatusListener)this._handleAnimationStatusChanged);
        if ((((MenuAnchor)this.widget).controller is null))
        {
            _internalMenuController = new global::Doroti.Framework.Widgets.MenuController();
        }
    }

    public override void didUpdateWidget(MenuAnchor oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!Equals(((MenuAnchor)oldWidget).controller, ((MenuAnchor)this.widget).controller)))
        {
            if ((((MenuAnchor)this.widget).controller is null))
            {
                _internalMenuController = new global::Doroti.Framework.Widgets.MenuController();
            }
            else
            {
                _internalMenuController = null;
            }
        }
        if (((((MenuAnchor)oldWidget).animated != ((MenuAnchor)this.widget).animated) || (!Equals(((MenuAnchor)this.widget).menuChildren, ((MenuAnchor)oldWidget).menuChildren))))
        {
            _resolveMenuItems();
        }
        if ((((MenuAnchor)oldWidget).animated != ((MenuAnchor)this.widget).animated))
        {
            _resolveAnimationController();
        }
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() => Menu_anchorLibrary._debugMenuInfo($"Disposing of {this}"));
        this._menuChildren.Clear();
        foreach (global::Doroti.Framework.Animation.CurvedAnimation animation in this._cachedAnimations)
        {
            animation.dispose();
        }
        _internalMenuController = null;
        this._menuScopeNode.dispose();
        this.heightAnimation.dispose();
        this.opacityAnimation.dispose();
        this._animationController.stop();
        this._animationController.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if (((this._ticker is null) || !this._ticker!.isActive))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), this._ticker!.describeForError("The offending ticker was") }));
            });
        this._tickerModeNotifier?.removeListener(this._updateTicker);
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual void _resolveAnimationController()
    {
        if (((MenuAnchor)this.widget).animated)
        {
            this._animationController.duration = Menu_anchorLibrary._kMenuOpeningDuration;
            this._animationController.reverseDuration = Menu_anchorLibrary._kMenuClosingDuration;
        }
        else
        {
            this._animationController.duration = Duration.zero;
            this._animationController.reverseDuration = Duration.zero;
        }
    }

    internal virtual void _resolveMenuItems()
    {
        _menuChildren = new List<global::Doroti.Framework.Widgets.Widget>();
        foreach (global::Doroti.Framework.Animation.CurvedAnimation animation in this._cachedAnimations)
        {
            animation.dispose();
        }
        _cachedAnimations = new List<global::Doroti.Framework.Animation.CurvedAnimation>();
        long itemCount = checked((long)(((MenuAnchor)this.widget).menuChildren.Count));
        if ((itemCount == 0L))
        {
            return;
        }
        if (!((MenuAnchor)this.widget).animated)
        {
            this._menuChildren.AddRange(((MenuAnchor)this.widget).menuChildren.Cast<global::Doroti.Framework.Widgets.Widget>());
            return;
        }
        double forwardFinalItemOffset = (1L - Menu_anchorLibrary._kMenuItemRelativeFadeInDuration);
        double reverseFinalItemOffset = ((1L - Menu_anchorLibrary._kMenuItemRelativeFadeOutDuration) - Menu_anchorLibrary._kMenuItemRelativeFadeOutDelay);
        double forwardProgress = 0;
        double reverseProgress = 0;
        double itemFadeInGap = 0;
        double itemFadeOutGap = 0;
        if ((itemCount > 1L))
        {
            itemFadeInGap = (forwardFinalItemOffset / ((itemCount - 1L)));
            itemFadeOutGap = (reverseFinalItemOffset / ((itemCount - 1L)));
        }
        foreach (global::Doroti.Framework.Widgets.Widget childLocal in ((MenuAnchor)this.widget).menuChildren)
        {
            var forwardCurve = new global::Doroti.Framework.Animation.Interval(forwardProgress, (forwardProgress + Menu_anchorLibrary._kMenuItemRelativeFadeInDuration));
            var reverseCurveLocal = new global::Doroti.Framework.Animation.Interval(reverseProgress, (reverseProgress + Menu_anchorLibrary._kMenuItemRelativeFadeOutDuration));
            var animationLocal = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this._animationController, curve: forwardCurve, reverseCurve: reverseCurveLocal);
            this._cachedAnimations.Add(animationLocal);
            this._menuChildren.Add(new global::Doroti.Framework.Widgets.FadeTransition(opacity: animationLocal, alwaysIncludeSemantics: true, child: childLocal));
            forwardProgress += itemFadeInGap;
            reverseProgress += itemFadeOutGap;
        }
    }

    internal virtual void _handleAnimationStatusChanged(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        setState(((global::System.Action)(() =>
        {
        })));
        ((MenuAnchor)this.widget).onAnimationStatusChanged?.Invoke(status);
    }

    internal virtual void _handleMenuOpenRequest(Offset? position, global::System.Action showOverlay)
    {
        if ((this._parent?.isClosing ?? false))
        {
            return;
        }
        showOverlay();
        if (this._animationController.isForwardOrCompleted)
        {
            return;
        }
        this._animationController.forward();
    }

    internal virtual void _handleMenuCloseRequest(global::System.Action hideOverlay)
    {
        if (!this._animationController.isForwardOrCompleted)
        {
            return;
        }
        DartRuntimePrimitives.Ignore(this._animationController.reverse().whenComplete(() => { ((Action)hideOverlay)(); return default!; }));
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Widgets.Widget childLocal = ((global::Doroti.Framework.Widgets.Widget)new _MenuAnchorScope__menu_anchor(state: this, animationStatus: ((global::Doroti.Framework.Animation.AnimationController)this._animationController).status, child: new global::Doroti.Framework.Widgets.RawMenuAnchor(onOpenRequested: (global::System.Action<Offset?, global::System.Action>)this._handleMenuOpenRequest, onCloseRequested: (global::System.Action<global::System.Action>)this._handleMenuCloseRequest, useRootOverlay: ((MenuAnchor)this.widget).useRootOverlay, onOpen: ((MenuAnchor)this.widget).onOpen, onClose: ((MenuAnchor)this.widget).onClose, consumeOutsideTaps: ((MenuAnchor)this.widget).consumeOutsideTap, controller: this._menuController, childFocusNode: ((MenuAnchor)this.widget).childFocusNode, overlayBuilder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.RawMenuOverlayInfo, global::Doroti.Framework.Widgets.Widget>)this._buildOverlay, builder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.MenuController, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>?)((MenuAnchor)this.widget).builder, child: ((MenuAnchor)this.widget).child)));
        if ((((MenuAnchor)this.widget).layerLink is null))
        {
            return childLocal;
        }
        return ((global::Doroti.Framework.Widgets.Widget)new global::Doroti.Framework.Widgets.CompositedTransformTarget(link: ((MenuAnchor)this.widget).layerLink!, child: childLocal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildOverlay(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.RawMenuOverlayInfo position)
    {
        return ((global::Doroti.Framework.Widgets.Widget)new global::Doroti.Framework.Widgets.ExcludeSemantics(excluding: this.isClosingOrClosed, child: new global::Doroti.Framework.Widgets.IgnorePointer(ignoring: this.isClosingOrClosed, child: new global::Doroti.Framework.Widgets.ExcludeFocus(excluding: this.isClosingOrClosed, child: new _Submenu__menu_anchor(fadeAnimation: this.opacityAnimation, heightAnimation: this.heightAnimation, layerLink: ((MenuAnchor)this.widget).layerLink, consumeOutsideTaps: ((MenuAnchor)this.widget).consumeOutsideTap, menuScopeNode: this._menuScopeNode, menuStyle: ((MenuAnchor)this.widget).style, clipBehavior: ((MenuAnchor)this.widget).clipBehavior, menuChildren: this._menuChildren, crossAxisUnconstrained: ((MenuAnchor)this.widget).crossAxisUnconstrained, menuPosition: position, anchor: this, alignmentOffset: (((MenuAnchor)this.widget).alignmentOffset ?? Offset.zero), reservedPadding: (((MenuAnchor)this.widget).reservedPadding ?? EdgeInsets.CreateAll(Menu_anchorLibrary._kMenuViewPadding)))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual _MenuAnchorState__menu_anchor _root
    {
        get
        {
            var anchor = this;
            while ((((_MenuAnchorState__menu_anchor)anchor)._parent is not null))
            {
                anchor = ((_MenuAnchorState__menu_anchor)anchor)._parent!;
            }
            return anchor;
        }
    }
    internal virtual void _focusButton()
    {
        if ((((MenuAnchor)this.widget).childFocusNode is null))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => Menu_anchorLibrary._debugMenuInfo($"Requesting focus for {((MenuAnchor)this.widget).childFocusNode}"));
        ((MenuAnchor)this.widget).childFocusNode!.requestFocus();
    }

    internal virtual void _focusFirstMenuItem()
    {
        if ((this._menuScopeNode.context?.mounted != true))
        {
            return;
        }
        global::Doroti.Framework.Widgets.FocusTraversalPolicy policy = (FocusTraversalGroup.maybeOf(this._menuScopeNode.context!) ?? new global::Doroti.Framework.Widgets.ReadingOrderTraversalPolicy());
        global::Doroti.Framework.Widgets.FocusNode? firstFocus = ((global::Doroti.Framework.Widgets.FocusNode?)policy.findFirstFocus(this._menuScopeNode, ignoreCurrentFocus: true));
        if ((firstFocus is not null))
        {
            firstFocus.requestFocus();
        }
    }

    internal virtual void _focusLastMenuItem()
    {
        if ((this._menuScopeNode.context?.mounted != true))
        {
            return;
        }
        global::Doroti.Framework.Widgets.FocusTraversalPolicy policy = (FocusTraversalGroup.maybeOf(this._menuScopeNode.context!) ?? new global::Doroti.Framework.Widgets.ReadingOrderTraversalPolicy());
        global::Doroti.Framework.Widgets.FocusNode lastFocus = ((global::Doroti.Framework.Widgets.FocusNode)policy.findLastFocus(this._menuScopeNode, ignoreCurrentFocus: true));
        lastFocus.requestFocus();
    }

    internal static _MenuAnchorState__menu_anchor? _maybeOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return context.getInheritedWidgetOfExactType<_MenuAnchorScope__menu_anchor>()?.state;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Framework.Animation.AnimationStatus? _maybeAnimationStatusOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return context.dependOnInheritedWidgetOfExactType<_MenuAnchorScope__menu_anchor>()?.animationStatus;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString(global::Doroti.Framework.Foundation.DiagnosticLevel minLevel = DiagnosticLevel.debug)
    {
        return DiagnosticsLibrary.describeIdentity(this);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._ticker is null))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this.GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new global::Doroti.Framework.Foundation.ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new global::Doroti.Framework.Foundation.ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
            });
        this._ticker = new global::Doroti.Framework.Scheduler.Ticker((global::System.Action<Duration>)onTick, debugLabel: (Foundation.ConstantsLibrary.kDebugMode ? $"created by {(DiagnosticsLibrary.describeIdentity(this))}" : null));
        _updateTickerModeNotifier();
        _updateTicker();
        return this._ticker!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTicker();
    }

    public virtual void _updateTicker()
    {
        TickerModeData values = this._tickerModeNotifier!.value;
        if ((this._ticker is not null))
        {
            this._ticker!.muted = !((TickerModeData)values).enabled;
            this._ticker!.forceFrames = ((TickerModeData)values).forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)TickerMode.getValuesNotifier(this.context));
        if ((Equals(newNotifier, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(this._updateTicker);
        newNotifier.addListener(this._updateTicker);
        this._tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription = ((this._ticker?.isActive, this._ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) });
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Scheduler.Ticker>("ticker", this._ticker, description: tickerDescription, showSeparator: false, defaultValue: default));
    }

}

public class MenuBar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual MenuStyle? style { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.MenuController? controller { get; private set; }
    public virtual List<global::Doroti.Framework.Widgets.Widget> children { get; private set; } = default!;

    public MenuBar(global::Doroti.Framework.Foundation.Key? key = null, MenuStyle? style = null, Clip clipBehavior = Clip.none, global::Doroti.Framework.Widgets.MenuController? controller = null, List<global::Doroti.Framework.Widgets.Widget> children = default!) : base(key: key)
    {
        this.style = style;
        this.clipBehavior = clipBehavior;
        this.controller = controller;
        this.children = children;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasOverlay(context));
        return ((global::Doroti.Framework.Widgets.Widget)new _MenuBarAnchor__menu_anchor(controller: this.controller, clipBehavior: this.clipBehavior, style: this.style, menuChildren: this.children));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        return ((Func<List<global::Doroti.Framework.Foundation.DiagnosticsNode>>)(() => { var __collection33328 = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>(); __collection33328.AddRange(this.children.map<global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Foundation.DiagnosticsNode>(((item) => ((Diagnosticable)item).toDiagnosticsNode()))); return __collection33328; }))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<MenuStyle?>("style", this.style, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Clip>("clipBehavior", this.clipBehavior, defaultValue: null));
    }

}

public class MenuItemButton : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual global::System.Action<bool>? onHover { get; private set; }
    public virtual bool requestFocusOnHover { get; private set; } = default!;
    public virtual global::System.Action<bool>? onFocusChange { get; private set; }
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.MenuSerializableShortcut? shortcut { get; private set; }
    public virtual string? semanticsLabel { get; private set; }
    public virtual ButtonStyle? style { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStatesController? statesController { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? leadingIcon { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? trailingIcon { get; private set; }
    public virtual bool closeOnActivate { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.Axis overflowAxis { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }

    public MenuItemButton(global::Doroti.Framework.Foundation.Key? key = null, global::System.Action? onPressed = null, global::System.Action<bool>? onHover = null, bool requestFocusOnHover = true, global::System.Action<bool>? onFocusChange = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, global::Doroti.Framework.Widgets.MenuSerializableShortcut? shortcut = null, string? semanticsLabel = null, ButtonStyle? style = null, global::Doroti.Framework.Widgets.WidgetStatesController? statesController = null, Clip clipBehavior = Clip.none, global::Doroti.Framework.Widgets.Widget? leadingIcon = null, global::Doroti.Framework.Widgets.Widget? trailingIcon = null, bool closeOnActivate = true, global::Doroti.Framework.Painting.Axis overflowAxis = Axis.horizontal, global::Doroti.Framework.Widgets.Widget? child = null) : base(key: key)
    {
        this.onPressed = onPressed;
        this.onHover = onHover;
        this.requestFocusOnHover = requestFocusOnHover;
        this.onFocusChange = onFocusChange;
        this.focusNode = focusNode;
        this.autofocus = autofocus;
        this.shortcut = shortcut;
        this.semanticsLabel = semanticsLabel;
        this.style = style;
        this.statesController = statesController;
        this.clipBehavior = clipBehavior;
        this.leadingIcon = leadingIcon;
        this.trailingIcon = trailingIcon;
        this.closeOnActivate = closeOnActivate;
        this.overflowAxis = overflowAxis;
        this.child = child;
    }

    public virtual bool enabled => DartRuntimePrimitives.ConvertValue<bool>((this.onPressed is not null));
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _MenuItemButtonState__menu_anchor());
    public virtual ButtonStyle defaultStyleOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((ButtonStyle)new _MenuButtonDefaultsM3__menu_anchor(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ButtonStyle? themeStyleOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return MenuButtonTheme.of(context).style;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ButtonStyle styleFrom(Color? foregroundColor = null, Color? backgroundColor = null, Color? disabledForegroundColor = null, Color? disabledBackgroundColor = null, Color? shadowColor = null, Color? surfaceTintColor = null, Color? iconColor = null, double? iconSize = null, Color? disabledIconColor = null, global::Doroti.Framework.Painting.TextStyle? textStyle = null, Color? overlayColor = null, double? elevation = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, Size? minimumSize = null, Size? fixedSize = null, Size? maximumSize = null, global::Doroti.Framework.Services.MouseCursor? enabledMouseCursor = null, global::Doroti.Framework.Services.MouseCursor? disabledMouseCursor = null, global::Doroti.Framework.Painting.BorderSide? side = null, global::Doroti.Framework.Painting.OutlinedBorder? shape = null, VisualDensity? visualDensity = null, MaterialTapTargetSize? tapTargetSize = null, Duration? animationDuration = null, bool? enableFeedback = null, global::Doroti.Framework.Painting.AlignmentGeometry? alignment = null, InteractiveInkFeatureFactory? splashFactory = null)
    {
        return ((ButtonStyle)TextButton.styleFrom(foregroundColor: foregroundColor, backgroundColor: backgroundColor, disabledBackgroundColor: disabledBackgroundColor, disabledForegroundColor: disabledForegroundColor, shadowColor: shadowColor, surfaceTintColor: surfaceTintColor, iconColor: iconColor, iconSize: iconSize, disabledIconColor: disabledIconColor, textStyle: textStyle, overlayColor: overlayColor, elevation: elevation, padding: padding, minimumSize: minimumSize, fixedSize: fixedSize, maximumSize: maximumSize, enabledMouseCursor: enabledMouseCursor, disabledMouseCursor: disabledMouseCursor, side: side, shape: shape, visualDensity: visualDensity, tapTargetSize: tapTargetSize, animationDuration: animationDuration, enableFeedback: enableFeedback, alignment: alignment, splashFactory: splashFactory));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("enabled", value: (this.onPressed is not null), ifFalse: "DISABLED"));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ButtonStyle?>("style", this.style, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.MenuSerializableShortcut?>("shortcut", this.shortcut, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.FocusNode?>("focusNode", this.focusNode, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Ui.Clip>("clipBehavior", this.clipBehavior, defaultValue: Clip.none));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStatesController?>("statesController", this.statesController, defaultValue: null));
    }

}

internal class _MenuItemButtonState__menu_anchor : global::Doroti.Framework.Widgets.State<MenuItemButton>
{
    internal virtual global::Doroti.Framework.Widgets.FocusNode? _internalFocusNode { get; set; } = default;
    internal virtual bool _isHovered { get; set; } = false;

    internal virtual global::Doroti.Framework.Widgets.FocusNode _focusNode => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.FocusNode>((((MenuItemButton)this.widget).focusNode ?? this._internalFocusNode!));
    internal virtual _MenuAnchorState__menu_anchor? _anchor => _MenuAnchorState__menu_anchor._maybeOf(this.context);
    public override void initState()
    {
        base.initState();
        _createInternalFocusNodeIfNeeded();
        this._focusNode.addListener(this._handleFocusChange);
    }

    public override void dispose()
    {
        this._focusNode.removeListener(this._handleFocusChange);
        this._internalFocusNode?.dispose();
        _internalFocusNode = null;
        base.dispose();
    }

    public override void didUpdateWidget(MenuItemButton oldWidget)
    {
        if ((!Equals(((MenuItemButton)this.widget).focusNode, ((MenuItemButton)oldWidget).focusNode)))
        {
            ((((MenuItemButton)oldWidget).focusNode ?? this._internalFocusNode))?.removeListener(this._handleFocusChange);
            if ((((MenuItemButton)this.widget).focusNode is not null))
            {
                this._internalFocusNode?.dispose();
                _internalFocusNode = null;
            }
            _createInternalFocusNodeIfNeeded();
            this._focusNode.addListener(this._handleFocusChange);
        }
        base.didUpdateWidget(oldWidget);
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ButtonStyle mergedStyle = ((this.widget.themeStyleOf(context)?.merge(this.widget.defaultStyleOf(context)) ?? (ButtonStyle)this.widget.defaultStyleOf(context)));
        if ((((MenuItemButton)this.widget).style is not null))
        {
            mergedStyle = ((MenuItemButton)this.widget).style!.merge(mergedStyle);
        }
        global::Doroti.Framework.Widgets.Widget childLocal = ((global::Doroti.Framework.Widgets.Widget)new TextButton(onPressed: ((global::System.Action?)(((MenuItemButton)this.widget).enabled ? this._handleSelect : null)), onFocusChange: ((global::System.Action<bool>?)(((MenuItemButton)this.widget).enabled ? ((MenuItemButton)this.widget).onFocusChange : null)), focusNode: this._focusNode, style: mergedStyle, autofocus: (((MenuItemButton)this.widget).enabled && ((MenuItemButton)this.widget).autofocus), statesController: ((MenuItemButton)this.widget).statesController, clipBehavior: ((MenuItemButton)this.widget).clipBehavior, isSemanticButton: (Foundation.ConstantsLibrary.kIsWeb ? true : null), child: new _MenuItemLabel__menu_anchor(leadingIcon: ((MenuItemButton)this.widget).leadingIcon, shortcut: ((MenuItemButton)this.widget).shortcut, semanticsLabel: ((MenuItemButton)this.widget).semanticsLabel, trailingIcon: ((MenuItemButton)this.widget).trailingIcon, hasSubmenu: false, overflowAxis: (this._anchor?._orientation ?? ((MenuItemButton)this.widget).overflowAxis), child: ((MenuItemButton)this.widget).child)));
        if ((Menu_anchorLibrary._platformSupportsAccelerators && ((MenuItemButton)this.widget).enabled))
        {
            childLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new MenuAcceleratorCallbackBinding(onInvoke: () => this._handleSelect(), child: childLocal));
        }
        if (((((MenuItemButton)this.widget).onHover is not null) || ((MenuItemButton)this.widget).requestFocusOnHover))
        {
            childLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.MouseRegion(onHover: (global::System.Action<global::Doroti.Framework.Gestures.PointerHoverEvent>)this._handlePointerHover, onExit: (global::System.Action<global::Doroti.Framework.Gestures.PointerExitEvent>)this._handlePointerExit, child: childLocal));
        }
        return ((global::Doroti.Framework.Widgets.Widget)new global::Doroti.Framework.Widgets.MergeSemantics(child: childLocal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleFocusChange()
    {
        if (!((global::Doroti.Framework.Widgets.FocusNode)this._focusNode).hasPrimaryFocus)
        {
            MenuController.maybeOf(this.context)?.closeChildren();
        }
    }

    internal virtual void _handlePointerExit(global::Doroti.Framework.Gestures.PointerExitEvent @event)
    {
        if (this._isHovered)
        {
            ((MenuItemButton)this.widget).onHover?.Invoke(false);
            _isHovered = false;
        }
    }

    internal virtual void _handlePointerHover(global::Doroti.Framework.Gestures.PointerHoverEvent @event)
    {
        if (!this._isHovered)
        {
            _isHovered = true;
            ((MenuItemButton)this.widget).onHover?.Invoke(true);
            if (((MenuItemButton)this.widget).requestFocusOnHover)
            {
                DartRuntimePrimitives.Assert(() => Menu_anchorLibrary._debugMenuInfo($"Requesting focus for {this._focusNode} from hover"));
                this._focusNode.requestFocus();
                FocusTraversalGroup.of(this.context).invalidateScopeData(FocusScope.of(this.context));
            }
        }
    }

    internal virtual void _handleSelect()
    {
        DartRuntimePrimitives.Assert(() => Menu_anchorLibrary._debugMenuInfo($"Selected {((MenuItemButton)this.widget).child} menu"));
        if (((MenuItemButton)this.widget).closeOnActivate)
        {
            this._anchor?._root._menuController.close();
        }
        Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) =>
        {
            FocusManager.instance.applyFocusChangesIfNeeded();
            ((MenuItemButton)this.widget).onPressed?.Invoke();
        })), debugLabel: "MenuAnchor.onPressed");
    }

    internal virtual void _createInternalFocusNodeIfNeeded()
    {
        if ((((MenuItemButton)this.widget).focusNode is null))
        {
            _internalFocusNode = new global::Doroti.Framework.Widgets.FocusNode();
            DartRuntimePrimitives.Assert(() =>
                {
                    this._internalFocusNode?.debugLabel = $"{typeof(MenuItemButton)}({((MenuItemButton)this.widget).child})";
                    return true;
                });
        }
    }

}

public class CheckboxMenuButton : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual bool? value { get; private set; }
    public virtual bool tristate { get; private set; } = default!;
    public virtual bool isError { get; private set; } = default!;
    public virtual global::System.Action<bool?>? onChanged { get; private set; }
    public virtual global::System.Action<bool>? onHover { get; private set; }
    public virtual global::System.Action<bool>? onFocusChange { get; private set; }
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual global::Doroti.Framework.Widgets.MenuSerializableShortcut? shortcut { get; private set; }
    public virtual ButtonStyle? style { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStatesController? statesController { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? trailingIcon { get; private set; }
    public virtual bool closeOnActivate { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }

    public CheckboxMenuButton(global::Doroti.Framework.Foundation.Key? key = null, bool? value = default!, bool tristate = false, bool isError = false, global::System.Action<bool?>? onChanged = default!, global::System.Action<bool>? onHover = null, global::System.Action<bool>? onFocusChange = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::Doroti.Framework.Widgets.MenuSerializableShortcut? shortcut = null, ButtonStyle? style = null, global::Doroti.Framework.Widgets.WidgetStatesController? statesController = null, Clip clipBehavior = Clip.none, global::Doroti.Framework.Widgets.Widget? trailingIcon = null, bool closeOnActivate = true, global::Doroti.Framework.Widgets.Widget? child = default!) : base(key: key)
    {
        this.value = value;
        this.tristate = tristate;
        this.isError = isError;
        this.onChanged = onChanged;
        this.onHover = onHover;
        this.onFocusChange = onFocusChange;
        this.focusNode = focusNode;
        this.shortcut = shortcut;
        this.style = style;
        this.statesController = statesController;
        this.clipBehavior = clipBehavior;
        this.trailingIcon = trailingIcon;
        this.closeOnActivate = closeOnActivate;
        this.child = child;
    }

    public virtual bool enabled => DartRuntimePrimitives.ConvertValue<bool>((this.onChanged is not null));
    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)new MenuItemButton(key: this.key, onPressed: ((global::System.Action?)((this.onChanged is null) ? null : (() =>
        {
            switch (this.value)
            {
                case false:
                    {
                        this.onChanged!(true);
                        break;
                    }
                case true:
                    {
                        this.onChanged!((this.tristate ? null : false));
                        break;
                    }
                case null:
                    {
                        this.onChanged!(false);
                        break;
                    }
            }
        }))), onHover: (global::System.Action<bool>?)this.onHover, onFocusChange: (global::System.Action<bool>?)this.onFocusChange, focusNode: this.focusNode, style: this.style, shortcut: this.shortcut, statesController: this.statesController, leadingIcon: new global::Doroti.Framework.Widgets.ExcludeFocus(child: new global::Doroti.Framework.Widgets.IgnorePointer(child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(maxHeight: Checkbox.width, maxWidth: Checkbox.width), child: new Checkbox(tristate: this.tristate, value: this.value, onChanged: this.onChanged, isError: this.isError)))), clipBehavior: this.clipBehavior, trailingIcon: this.trailingIcon, closeOnActivate: this.closeOnActivate, child: this.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class RadioMenuButton<T> : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual T value { get; private set; } = default!;
    public virtual T? groupValue { get; private set; }
    public virtual bool toggleable { get; private set; } = default!;
    public virtual global::System.Action<T?>? onChanged { get; private set; }
    public virtual global::System.Action<bool>? onHover { get; private set; }
    public virtual global::System.Action<bool>? onFocusChange { get; private set; }
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual global::Doroti.Framework.Widgets.MenuSerializableShortcut? shortcut { get; private set; }
    public virtual ButtonStyle? style { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStatesController? statesController { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? trailingIcon { get; private set; }
    public virtual bool closeOnActivate { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }

    public RadioMenuButton(global::Doroti.Framework.Foundation.Key? key = null, T value = default!, T? groupValue = default!, global::System.Action<T?>? onChanged = default!, bool toggleable = false, global::System.Action<bool>? onHover = null, global::System.Action<bool>? onFocusChange = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::Doroti.Framework.Widgets.MenuSerializableShortcut? shortcut = null, ButtonStyle? style = null, global::Doroti.Framework.Widgets.WidgetStatesController? statesController = null, Clip clipBehavior = Clip.none, global::Doroti.Framework.Widgets.Widget? trailingIcon = null, bool closeOnActivate = true, global::Doroti.Framework.Widgets.Widget? child = default!) : base(key: key)
    {
        this.value = value;
        this.groupValue = groupValue;
        this.onChanged = onChanged;
        this.toggleable = toggleable;
        this.onHover = onHover;
        this.onFocusChange = onFocusChange;
        this.focusNode = focusNode;
        this.shortcut = shortcut;
        this.style = style;
        this.statesController = statesController;
        this.clipBehavior = clipBehavior;
        this.trailingIcon = trailingIcon;
        this.closeOnActivate = closeOnActivate;
        this.child = child;
    }

    public virtual bool enabled => DartRuntimePrimitives.ConvertValue<bool>((this.onChanged is not null));
    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)new MenuItemButton(key: this.key, onPressed: ((global::System.Action?)((this.onChanged is null) ? null : (() =>
        {
            if ((this.toggleable && EqualityComparer<T>.Default.Equals(this.groupValue, this.value)))
            {
                this.onChanged!(default);
                return;
            }
            this.onChanged!(this.value);
        }))), onHover: (global::System.Action<bool>?)this.onHover, onFocusChange: (global::System.Action<bool>?)this.onFocusChange, focusNode: this.focusNode, style: this.style, shortcut: this.shortcut, statesController: this.statesController, leadingIcon: new global::Doroti.Framework.Widgets.ExcludeFocus(child: new global::Doroti.Framework.Widgets.IgnorePointer(child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(maxHeight: Checkbox.width, maxWidth: Checkbox.width), child: new Radio<T>(value: this.value, groupValue: this.groupValue, onChanged: this.onChanged, toggleable: this.toggleable)))), clipBehavior: this.clipBehavior, trailingIcon: this.trailingIcon, closeOnActivate: this.closeOnActivate, child: this.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SubmenuButton : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::System.Action<bool>? onHover { get; private set; }
    public virtual global::System.Action<bool>? onFocusChange { get; private set; }
    public virtual global::System.Action? onOpen { get; private set; }
    public virtual global::System.Action? onClose { get; private set; }
    public virtual global::Doroti.Framework.Widgets.MenuController? controller { get; private set; }
    public virtual ButtonStyle? style { get; private set; }
    public virtual MenuStyle? menuStyle { get; private set; }
    public virtual Offset? alignmentOffset { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStatesController? statesController { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? leadingIcon { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Widgets.Widget?>? submenuIcon { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? trailingIcon { get; private set; }
    public virtual bool useRootOverlay { get; private set; } = default!;
    public virtual AnimationStatusListener? onAnimationStatusChanged { get; private set; }
    public virtual List<global::Doroti.Framework.Widgets.Widget> menuChildren { get; private set; } = default!;
    public virtual Duration hoverOpenDelay { get; private set; } = default!;
    public virtual bool animated { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }

    public SubmenuButton(global::Doroti.Framework.Foundation.Key? key = null, global::System.Action<bool>? onHover = null, global::System.Action<bool>? onFocusChange = null, global::System.Action? onOpen = null, global::System.Action? onClose = null, global::Doroti.Framework.Widgets.MenuController? controller = null, ButtonStyle? style = null, MenuStyle? menuStyle = null, Offset? alignmentOffset = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::Doroti.Framework.Widgets.WidgetStatesController? statesController = null, global::Doroti.Framework.Widgets.Widget? leadingIcon = null, global::Doroti.Framework.Widgets.Widget? trailingIcon = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Widgets.Widget?>? submenuIcon = null, bool useRootOverlay = false, Duration hoverOpenDelay = default, bool animated = false, AnimationStatusListener? onAnimationStatusChanged = null, List<global::Doroti.Framework.Widgets.Widget> menuChildren = default!, global::Doroti.Framework.Widgets.Widget? child = default!) : base(key: key)
    {
        this.onHover = onHover;
        this.onFocusChange = onFocusChange;
        this.onOpen = onOpen;
        this.onClose = onClose;
        this.controller = controller;
        this.style = style;
        this.menuStyle = menuStyle;
        this.alignmentOffset = alignmentOffset;
        this.clipBehavior = clipBehavior;
        this.focusNode = focusNode;
        this.statesController = statesController;
        this.leadingIcon = leadingIcon;
        this.trailingIcon = trailingIcon;
        this.submenuIcon = submenuIcon;
        this.useRootOverlay = useRootOverlay;
        this.hoverOpenDelay = hoverOpenDelay;
        this.animated = animated;
        this.onAnimationStatusChanged = onAnimationStatusChanged;
        this.menuChildren = menuChildren;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SubmenuButtonState__menu_anchor());
    public virtual ButtonStyle defaultStyleOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((ButtonStyle)new _MenuButtonDefaultsM3__menu_anchor(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ButtonStyle? themeStyleOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return MenuButtonTheme.of(context).style;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ButtonStyle styleFrom(Color? foregroundColor = null, Color? backgroundColor = null, Color? disabledForegroundColor = null, Color? disabledBackgroundColor = null, Color? shadowColor = null, Color? surfaceTintColor = null, Color? iconColor = null, double? iconSize = null, Color? disabledIconColor = null, global::Doroti.Framework.Painting.TextStyle? textStyle = null, Color? overlayColor = null, double? elevation = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, Size? minimumSize = null, Size? fixedSize = null, Size? maximumSize = null, global::Doroti.Framework.Services.MouseCursor? enabledMouseCursor = null, global::Doroti.Framework.Services.MouseCursor? disabledMouseCursor = null, global::Doroti.Framework.Painting.BorderSide? side = null, global::Doroti.Framework.Painting.OutlinedBorder? shape = null, VisualDensity? visualDensity = null, MaterialTapTargetSize? tapTargetSize = null, Duration? animationDuration = null, bool? enableFeedback = null, global::Doroti.Framework.Painting.AlignmentGeometry? alignment = null, InteractiveInkFeatureFactory? splashFactory = null)
    {
        return ((ButtonStyle)TextButton.styleFrom(foregroundColor: foregroundColor, backgroundColor: backgroundColor, disabledBackgroundColor: disabledBackgroundColor, disabledForegroundColor: disabledForegroundColor, shadowColor: shadowColor, surfaceTintColor: surfaceTintColor, iconColor: iconColor, disabledIconColor: disabledIconColor, iconSize: iconSize, textStyle: textStyle, overlayColor: overlayColor, elevation: elevation, padding: padding, minimumSize: minimumSize, fixedSize: fixedSize, maximumSize: maximumSize, enabledMouseCursor: enabledMouseCursor, disabledMouseCursor: disabledMouseCursor, side: side, shape: shape, visualDensity: visualDensity, tapTargetSize: tapTargetSize, animationDuration: animationDuration, enableFeedback: enableFeedback, alignment: alignment, splashFactory: splashFactory));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        return ((Func<List<global::Doroti.Framework.Foundation.DiagnosticsNode>>)(() =>
        {
            var __collection71845 = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>(); __collection71845.AddRange(this.menuChildren.map<global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Foundation.DiagnosticsNode>(((child) =>
            {
                return ((global::Doroti.Framework.Foundation.DiagnosticsNode)((Diagnosticable)child).toDiagnosticsNode());
                throw new InvalidOperationException("Dart closure completed without a value.");
            }))); return __collection71845;
        }))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.FocusNode?>("focusNode", this.focusNode));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<MenuStyle>("menuStyle", this.menuStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Offset>("alignmentOffset", this.alignmentOffset));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Ui.Clip>("clipBehavior", this.clipBehavior));
    }

}

internal class _SubmenuButtonState__menu_anchor : global::Doroti.Framework.Widgets.State<SubmenuButton>
{
    private bool __late_actions_initialized;
    private DartMap<Type, dynamic> __late_actions = default!;
    public virtual DartMap<Type, dynamic> actions
    {
        get
        {
            if (!__late_actions_initialized)
            {
                __late_actions = new DartMap<Type, dynamic> { [typeof(global::Doroti.Framework.Widgets.DirectionalFocusIntent)] = new _SubmenuDirectionalFocusAction__menu_anchor(submenu: this) };
                __late_actions_initialized = true;
            }
            return __late_actions;
        }
    }
    internal virtual bool _waitingToFocusMenu { get; set; } = false;
    internal virtual bool _isOpenOnFocusEnabled { get; set; } = true;
    internal virtual global::Doroti.Framework.Widgets.MenuController? _internalMenuController { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.FocusNode? _internalFocusNode { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<_MenuAnchorState__menu_anchor> _anchorKey { get; private set; } = GlobalKey<_MenuAnchorState__menu_anchor>.Create();
    internal virtual bool _isHovered { get; set; } = false;
    internal virtual global::Doroti.Framework.Animation.AnimationStatus _animationStatus { get; set; } = AnimationStatus.dismissed;
    internal virtual Timer? _hoverOpenTimer { get; set; } = default;

    internal virtual global::Doroti.Framework.Widgets.MenuController _menuController => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.MenuController>((((SubmenuButton)this.widget).controller ?? this._internalMenuController!));
    internal virtual _MenuAnchorState__menu_anchor? _parent => _MenuAnchorState__menu_anchor._maybeOf(this.context);
    internal virtual _MenuAnchorState__menu_anchor? _anchorState => ((global::Doroti.Framework.Widgets.GlobalKey<_MenuAnchorState__menu_anchor>)this._anchorKey).currentState;
    internal virtual global::Doroti.Framework.Widgets.FocusNode _buttonFocusNode => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.FocusNode>((((SubmenuButton)this.widget).focusNode ?? this._internalFocusNode!));
    internal virtual bool _enabled => Enumerable.Any(((SubmenuButton)this.widget).menuChildren);
    public override void initState()
    {
        base.initState();
        if ((((SubmenuButton)this.widget).focusNode is null))
        {
            _internalFocusNode = new global::Doroti.Framework.Widgets.FocusNode();
            DartRuntimePrimitives.Assert(() =>
                {
                    this._internalFocusNode?.debugLabel = $"{typeof(SubmenuButton)}({((SubmenuButton)this.widget).child})";
                    return true;
                });
        }
        if ((((SubmenuButton)this.widget).controller is null))
        {
            _internalMenuController = new global::Doroti.Framework.Widgets.MenuController();
        }
        this._buttonFocusNode.addListener(this._handleFocusChange);
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        DartRuntimePrimitives.Assert(() => _debugValidateHoverOpenDelay());
    }

    public override void dispose()
    {
        _clearHoverOpenTimer();
        this._buttonFocusNode.removeListener(this._handleFocusChange);
        this._internalFocusNode?.dispose();
        _internalFocusNode = null;
        base.dispose();
    }

    public override void didUpdateWidget(SubmenuButton oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        DartRuntimePrimitives.Assert(() => _debugValidateHoverOpenDelay());
        if ((!Equals(((SubmenuButton)this.widget).focusNode, ((SubmenuButton)oldWidget).focusNode)))
        {
            if ((((SubmenuButton)oldWidget).focusNode is null))
            {
                this._internalFocusNode?.removeListener(this._handleFocusChange);
                this._internalFocusNode?.dispose();
                _internalFocusNode = null;
            }
            else
            {
                ((SubmenuButton)oldWidget).focusNode!.removeListener(this._handleFocusChange);
            }
            if ((((SubmenuButton)this.widget).focusNode is null))
            {
                _internalFocusNode ??= new global::Doroti.Framework.Widgets.FocusNode();
                DartRuntimePrimitives.Assert(() =>
                    {
                        this._internalFocusNode?.debugLabel = $"{typeof(SubmenuButton)}({((SubmenuButton)this.widget).child})";
                        return true;
                    });
            }
            this._buttonFocusNode.addListener(this._handleFocusChange);
        }
        if ((!Equals(((SubmenuButton)this.widget).controller, ((SubmenuButton)oldWidget).controller)))
        {
            _internalMenuController = (((((SubmenuButton)oldWidget).controller is null)) ? null : new global::Doroti.Framework.Widgets.MenuController());
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Offset menuPaddingOffset = ((global::Doroti.Ui.Offset)(((SubmenuButton)this.widget).alignmentOffset ?? Offset.zero));
        global::Doroti.Framework.Painting.EdgeInsets menuPadding = ((global::Doroti.Framework.Painting.EdgeInsets)_computeMenuPadding(context));
        global::Doroti.Framework.Painting.Axis orientation = (this._parent?._orientation ?? Axis.vertical);
        menuPaddingOffset += ((orientation, Directionality.of(context)) switch { (Axis.horizontal, TextDirection.rtl) => new global::Doroti.Ui.Offset(((global::Doroti.Framework.Painting.EdgeInsets)menuPadding).right, 0), (Axis.horizontal, TextDirection.ltr) => new global::Doroti.Ui.Offset(-((global::Doroti.Framework.Painting.EdgeInsets)menuPadding).left, 0), (Axis.vertical, TextDirection.rtl) => new global::Doroti.Ui.Offset(0, -((global::Doroti.Framework.Painting.EdgeInsets)menuPadding).top), (Axis.vertical, TextDirection.ltr) => new global::Doroti.Ui.Offset(0, -((global::Doroti.Framework.Painting.EdgeInsets)menuPadding).top), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        var states = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection75817 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (!this._enabled) { __collection75817.Add(WidgetState.disabled); } if (this._isHovered) { __collection75817.Add(WidgetState.hovered); } if (((global::Doroti.Framework.Widgets.FocusNode)this._buttonFocusNode).hasFocus) { __collection75817.Add(WidgetState.focused); } return __collection75817; }))();
        global::Doroti.Framework.Widgets.Widget submenuIconLocal = (((((SubmenuButton)this.widget).submenuIcon?.resolve(states) ?? (global::Doroti.Framework.Widgets.Widget?)MenuTheme.of(context).submenuIcon?.resolve(states))) ?? new global::Doroti.Framework.Widgets.Icon(Icons.arrow_right, size: Menu_anchorLibrary._kDefaultSubmenuIconSize));
        return ((global::Doroti.Framework.Widgets.Widget)new global::Doroti.Framework.Widgets.Actions(actions: this.actions, child: new MenuAnchor(key: this._anchorKey, onAnimationStatusChanged: (AnimationStatusListener)this._handleAnimationStatusChanged, controller: this._menuController, childFocusNode: this._buttonFocusNode, alignmentOffset: menuPaddingOffset, clipBehavior: ((SubmenuButton)this.widget).clipBehavior, onClose: () => this._handleClose(), onOpen: () => this._handleOpen(), style: ((SubmenuButton)this.widget).menuStyle, useRootOverlay: ((SubmenuButton)this.widget).useRootOverlay, animated: ((SubmenuButton)this.widget).animated, builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.MenuController, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>?)((context, controller, child) =>
        {
            ButtonStyle mergedStyle = ((this.widget.themeStyleOf(context)?.merge(this.widget.defaultStyleOf(context)) ?? (ButtonStyle)this.widget.defaultStyleOf(context)));
            mergedStyle = (((SubmenuButton)this.widget).style?.merge(mergedStyle) ?? mergedStyle);
            void toggleShowMenu()
            {
                if (!this.mounted)
                {
                    return;
                }
                if (AnimationStatusMembers.isForwardOrCompleted(this._animationStatus))
                {
                    controller.close();
                }
                else
                {
                    controller.open();
                }
            }
            void handlePointerExit(global::Doroti.Framework.Gestures.PointerExitEvent @event)
            {
                if (this._isHovered)
                {
                    ((SubmenuButton)this.widget).onHover?.Invoke(false);
                    _isHovered = false;
                    _clearHoverOpenTimer();
                }
            }
            void handlePointerHover(global::Doroti.Framework.Gestures.PointerHoverEvent @event)
            {
                if (!this._isHovered)
                {
                    _isHovered = true;
                    ((SubmenuButton)this.widget).onHover?.Invoke(true);
                    _MenuAnchorState__menu_anchor root = _MenuAnchorState__menu_anchor._maybeOf(context)!._root;
                    if (((Equals(this._parent?._orientation, Axis.horizontal)) && !((_MenuAnchorState__menu_anchor)root)._menuController.isOpen))
                    {
                        return;
                    }
                    if (((global::Doroti.Framework.Widgets.FocusNode)this._buttonFocusNode).hasPrimaryFocus)
                    {
                        _clearHoverOpenTimer();
                        _maybeOpenMenuOnHoverOrFocus();
                    }
                    else
                    {
                        this._buttonFocusNode.requestFocus();
                    }
                }
            }
            child = new global::Doroti.Framework.Widgets.MergeSemantics(child: new global::Doroti.Framework.Widgets.Semantics(expanded: (this._enabled && AnimationStatusMembers.isForwardOrCompleted(this._animationStatus)), child: new TextButton(style: mergedStyle, focusNode: this._buttonFocusNode, onFocusChange: ((global::System.Action<bool>?)(this._enabled ? ((SubmenuButton)this.widget).onFocusChange : null)), onPressed: ((global::System.Action?)(this._enabled ? toggleShowMenu : null)), isSemanticButton: (Foundation.ConstantsLibrary.kIsWeb ? true : null), child: new _MenuItemLabel__menu_anchor(leadingIcon: ((SubmenuButton)this.widget).leadingIcon, trailingIcon: ((SubmenuButton)this.widget).trailingIcon, hasSubmenu: true, showDecoration: (Equals(((this._parent?._orientation ?? Axis.horizontal)), Axis.vertical)), submenuIcon: submenuIconLocal, child: child))));
            if (!this._enabled)
            {
                return child;
            }
            child = new global::Doroti.Framework.Widgets.MouseRegion(onHover: (global::System.Action<global::Doroti.Framework.Gestures.PointerHoverEvent>)handlePointerHover, onExit: (global::System.Action<global::Doroti.Framework.Gestures.PointerExitEvent>)handlePointerExit, child: child);
            if (Menu_anchorLibrary._platformSupportsAccelerators)
            {
                return ((global::Doroti.Framework.Widgets.Widget)new MenuAcceleratorCallbackBinding(onInvoke: () => toggleShowMenu(), hasSubmenu: true, child: child));
            }
            return child;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), menuChildren: ((SubmenuButton)this.widget).menuChildren, child: ((SubmenuButton)this.widget).child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleAnimationStatusChanged(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        setState(((global::System.Action)(() =>
        {
            _animationStatus = status;
        })));
        ((SubmenuButton)this.widget).onAnimationStatusChanged?.Invoke(status);
    }

    internal virtual void _handleClose()
    {
        if (!((global::Doroti.Framework.Widgets.FocusNode)this._buttonFocusNode).hasFocus)
        {
            _isOpenOnFocusEnabled = false;
            Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timestamp) =>
            {
                FocusManager.instance.applyFocusChangesIfNeeded();
                _isOpenOnFocusEnabled = true;
            })), debugLabel: "MenuAnchor.preventOpenOnFocus");
        }
        ((SubmenuButton)this.widget).onClose?.Invoke();
    }

    internal virtual void _handleOpen()
    {
        if (!this._waitingToFocusMenu)
        {
            Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) =>
            {
                if (this.mounted)
                {
                    this._buttonFocusNode.requestFocus();
                    _waitingToFocusMenu = false;
                }
            })), debugLabel: "MenuAnchor.focus");
            _waitingToFocusMenu = true;
        }
        setState(((global::System.Action)(() =>
        {
        })));
        ((SubmenuButton)this.widget).onOpen?.Invoke();
    }

    internal virtual global::Doroti.Framework.Painting.EdgeInsets _computeMenuPadding(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry?> insets = ((((SubmenuButton)this.widget).menuStyle?.padding ?? MenuTheme.of(context).style?.padding) ?? new _MenuDefaultsM3__menu_anchor(context).padding!);
        return ((global::Doroti.Framework.Painting.EdgeInsets)insets.resolve((((SubmenuButton)this.widget).statesController?.value ?? new HashSet<global::Doroti.Framework.Widgets.WidgetState>()))!.resolve(Directionality.of(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleFocusChange()
    {
        _clearHoverOpenTimer();
        if (!((global::Doroti.Framework.Widgets.FocusNode)this._buttonFocusNode).hasPrimaryFocus)
        {
            if ((!this._anchorState!._menuScopeNode.hasFocus && AnimationStatusMembers.isForwardOrCompleted(this._animationStatus)))
            {
                this._menuController.close();
            }
            return;
        }
        _maybeOpenMenuOnHoverOrFocus();
    }

    internal virtual void _maybeOpenMenuOnHoverOrFocus()
    {
        if (!this._isOpenOnFocusEnabled)
        {
            return;
        }
        if (((global::Doroti.Framework.Widgets.MenuController)this._menuController).isOpen)
        {
            if ((!Equals(this._animationStatus, AnimationStatus.reverse)))
            {
                return;
            }
            if (this._isHovered)
            {
                return;
            }
            if ((Equals(this._parent?._orientation, Axis.horizontal)))
            {
                return;
            }
        }
        if ((Equals(((SubmenuButton)this.widget).hoverOpenDelay, Duration.zero)))
        {
            this._menuController.open();
            return;
        }
        _hoverOpenTimer = new Timer(((SubmenuButton)this.widget).hoverOpenDelay, (() =>
        {
            this._menuController.open();
        }));
    }

    internal virtual void _clearHoverOpenTimer()
    {
        this._hoverOpenTimer?.cancel();
        _hoverOpenTimer = null;
    }

    internal virtual bool _debugValidateHoverOpenDelay()
    {
        if (((Equals(this._parent?._orientation, Axis.horizontal)) && (((SubmenuButton)this.widget).hoverOpenDelay > Duration.zero)))
        {
            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("A non-zero hoverOpenDelay was used in a top-level SubmenuButton situated in a MenuBar."), new global::Doroti.Framework.Foundation.ErrorDescription("MenuBar children can only be opened by hover if a sibling SubmenuButton is already open. When the hoverOpenDelay for a SubmenuButton is longer than the closing animation of a sibling SubmenuButton, that sibling will close before this SubmenuButton begins opening, leading to this SubmenuButton never opening."), this.context.describeElement("The affected SubmenuButton is") }));
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SubmenuDirectionalFocusAction__menu_anchor : global::Doroti.Framework.Widgets.DirectionalFocusAction
{
    public virtual _SubmenuButtonState__menu_anchor submenu { get; private set; } = default!;

    internal _SubmenuDirectionalFocusAction__menu_anchor(_SubmenuButtonState__menu_anchor submenu)
    {
        this.submenu = submenu;
    }

    internal virtual _MenuAnchorState__menu_anchor? _parent => ((_SubmenuButtonState__menu_anchor)this.submenu)._parent;
    internal virtual _MenuAnchorState__menu_anchor? _anchorState => ((_SubmenuButtonState__menu_anchor)this.submenu)._anchorState;
    internal virtual global::Doroti.Framework.Widgets.MenuController _controller => ((_SubmenuButtonState__menu_anchor)this.submenu)._menuController;
    internal virtual global::Doroti.Framework.Painting.Axis? _orientation => this._parent?._orientation;
    public virtual bool isSubmenu => ((_SubmenuButtonState__menu_anchor)this.submenu)._buttonFocusNode.hasPrimaryFocus;
    internal virtual global::Doroti.Framework.Widgets.FocusNode _button => ((_SubmenuButtonState__menu_anchor)this.submenu)._buttonFocusNode;
    public override object? invoke(global::Doroti.Framework.Widgets.DirectionalFocusIntent intent, global::Doroti.Framework.Widgets.BuildContext? context = null)
    {
        DartRuntimePrimitives.Assert(() => Menu_anchorLibrary._debugMenuInfo($"{(((global::Doroti.Framework.Widgets.DirectionalFocusIntent)intent).direction)}: Invoking directional focus intent."));
        global::Doroti.Ui.TextDirection directionality = Directionality.of(this.submenu.context);
        switch ((this._orientation, directionality, ((global::Doroti.Framework.Widgets.DirectionalFocusIntent)intent).direction))
        {
            case (Axis.horizontal, TextDirection.ltr, TraversalDirection.left):
            case (Axis.horizontal, TextDirection.rtl, TraversalDirection.right):
                {
                    DartRuntimePrimitives.Assert(() => Menu_anchorLibrary._debugMenuInfo($"Moving to previous {typeof(MenuBar)} item"));
                    DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Widgets.FocusNode>)(() =>
{
    var __cascade = this._button;
    __cascade.requestFocus();
    __cascade.previousFocus();
    return __cascade;
}))());
                    return null;
                }
            case (Axis.horizontal, TextDirection.ltr, TraversalDirection.right):
            case (Axis.horizontal, TextDirection.rtl, TraversalDirection.left):
                {
                    DartRuntimePrimitives.Assert(() => Menu_anchorLibrary._debugMenuInfo($"Moving to next {typeof(MenuBar)} item"));
                    DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Widgets.FocusNode>)(() =>
{
    var __cascade = this._button;
    __cascade.requestFocus();
    __cascade.nextFocus();
    return __cascade;
}))());
                    return null;
                }
            case (Axis.horizontal, _, TraversalDirection.down):
                {
                    if (this.isSubmenu)
                    {
                        this._anchorState?._focusFirstMenuItem();
                        return null;
                    }
                    break;
                }
            case (Axis.horizontal, _, TraversalDirection.up):
                {
                    if (this.isSubmenu)
                    {
                        this._anchorState?._focusLastMenuItem();
                        return null;
                    }
                    break;
                }
            case (Axis.vertical, TextDirection.ltr, TraversalDirection.left):
            case (Axis.vertical, TextDirection.rtl, TraversalDirection.right):
                {
                    if ((Equals(((_MenuAnchorState__menu_anchor?)this._parent?._parent)?._orientation, Axis.horizontal)))
                    {
                        if (this.isSubmenu)
                        {
                            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Widgets.FocusNode?>)(() =>
{
    var __cascade = this._parent!.widget.childFocusNode;
    __cascade?.requestFocus();
    __cascade?.previousFocus();
    return __cascade;
}))());
                        }
                        else
                        {
                            DartRuntimePrimitives.Assert(() => Menu_anchorLibrary._debugMenuInfo("Exiting submenu"));
                            this._anchorState?._focusButton();
                        }
                    }
                    else
                    {
                        if (this.isSubmenu)
                        {
                            if ((((_MenuAnchorState__menu_anchor?)this._parent?._parent) is null))
                            {
                                return null;
                            }
                            this._parent?._focusButton();
                            this._parent?._menuController.close();
                        }
                        else
                        {
                            this._controller.close();
                        }
                        DartRuntimePrimitives.Assert(() => Menu_anchorLibrary._debugMenuInfo("Exiting submenu"));
                    }
                    return null;
                }
            case (Axis.vertical, TextDirection.ltr, TraversalDirection.right) when (this.isSubmenu):
            case (Axis.vertical, TextDirection.rtl, TraversalDirection.left) when (this.isSubmenu):
                {
                    DartRuntimePrimitives.Assert(() => Menu_anchorLibrary._debugMenuInfo("Entering submenu"));
                    if (((global::Doroti.Framework.Widgets.MenuController)this._controller).isOpen)
                    {
                        this._anchorState?._focusFirstMenuItem();
                    }
                    else
                    {
                        this._controller.open();
                        Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timestamp) =>
                        {
                            if (((global::Doroti.Framework.Widgets.MenuController)this._controller).isOpen)
                            {
                                this._anchorState?._focusFirstMenuItem();
                            }
                        })));
                    }
                    return null;
                }
            default:
                {
                    break;
                }
        }
        return Actions.maybeInvoke(this.submenu.context, intent);
    }

}

internal class _LocalizedShortcutLabeler__menu_anchor
{
    internal static _LocalizedShortcutLabeler__menu_anchor? _instance = default;
    internal static DartMap<global::Doroti.Framework.Services.LogicalKeyboardKey, string> _shortcutGraphicEquivalents = new DartMap<global::Doroti.Framework.Services.LogicalKeyboardKey, string> { [LogicalKeyboardKey.arrowLeft] = "←", [LogicalKeyboardKey.arrowRight] = "→", [LogicalKeyboardKey.arrowUp] = "↑", [LogicalKeyboardKey.arrowDown] = "↓", [LogicalKeyboardKey.enter] = "↵" };
    internal static HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> _modifiers = new HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> { LogicalKeyboardKey.alt, LogicalKeyboardKey.control, LogicalKeyboardKey.meta, LogicalKeyboardKey.shift, LogicalKeyboardKey.altLeft, LogicalKeyboardKey.controlLeft, LogicalKeyboardKey.metaLeft, LogicalKeyboardKey.shiftLeft, LogicalKeyboardKey.altRight, LogicalKeyboardKey.controlRight, LogicalKeyboardKey.metaRight, LogicalKeyboardKey.shiftRight };
    internal virtual DartMap<MaterialLocalizations, DartMap<global::Doroti.Framework.Services.LogicalKeyboardKey, string>> _cachedShortcutKeys { get; private set; } = new DartMap<MaterialLocalizations, DartMap<global::Doroti.Framework.Services.LogicalKeyboardKey, string>>();

    internal _LocalizedShortcutLabeler__menu_anchor()
    {
    }

    public static _LocalizedShortcutLabeler__menu_anchor instance
    {
        get
        {
            return _instance ??= new _LocalizedShortcutLabeler__menu_anchor();
        }
    }
    public virtual string getShortcutLabel(global::Doroti.Framework.Widgets.MenuSerializableShortcut shortcut, MaterialLocalizations localizations)
    {
        global::Doroti.Framework.Widgets.ShortcutSerialization serialized = ((global::Doroti.Framework.Widgets.ShortcutSerialization)shortcut.serializeForMenu());
        string keySeparator = default!;
        if (Menu_anchorLibrary._usesSymbolicModifiers)
        {
            keySeparator = " ";
        }
        else
        {
            keySeparator = "+";
        }
        if ((((global::Doroti.Framework.Widgets.ShortcutSerialization)serialized).trigger is not null))
        {
            global::Doroti.Framework.Services.LogicalKeyboardKey triggerLocal = ((global::Doroti.Framework.Widgets.ShortcutSerialization)serialized).trigger!;
            var modifiers = ((Func<List<string>>)(() => { var __collection91801 = new List<string>(); if (Menu_anchorLibrary._usesSymbolicModifiers) { __collection91801.AddRange(((Func<List<string>>)(() => { var __collection91850 = new List<string>(); if (DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Widgets.ShortcutSerialization)serialized).control)) { __collection91850.Add(_getModifierLabel(LogicalKeyboardKey.control, localizations)); } if (DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Widgets.ShortcutSerialization)serialized).alt)) { __collection91850.Add(_getModifierLabel(LogicalKeyboardKey.alt, localizations)); } if (DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Widgets.ShortcutSerialization)serialized).shift)) { __collection91850.Add(_getModifierLabel(LogicalKeyboardKey.shift, localizations)); } if (DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Widgets.ShortcutSerialization)serialized).meta)) { __collection91850.Add(_getModifierLabel(LogicalKeyboardKey.meta, localizations)); } return __collection91850; }))()); } else { __collection91801.AddRange(((Func<List<string>>)(() => { var __collection92331 = new List<string>(); if (DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Widgets.ShortcutSerialization)serialized).alt)) { __collection92331.Add(_getModifierLabel(LogicalKeyboardKey.alt, localizations)); } if (DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Widgets.ShortcutSerialization)serialized).control)) { __collection92331.Add(_getModifierLabel(LogicalKeyboardKey.control, localizations)); } if (DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Widgets.ShortcutSerialization)serialized).meta)) { __collection92331.Add(_getModifierLabel(LogicalKeyboardKey.meta, localizations)); } if (DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Widgets.ShortcutSerialization)serialized).shift)) { __collection92331.Add(_getModifierLabel(LogicalKeyboardKey.shift, localizations)); } return __collection92331; }))()); } return __collection91801; }))();
            string? shortcutTrigger = default!;
            long logicalKeyId = ((global::Doroti.Framework.Services.LogicalKeyboardKey)triggerLocal).keyId;
            if (_shortcutGraphicEquivalents.ContainsKey(triggerLocal))
            {
                shortcutTrigger = _shortcutGraphicEquivalents.GetValueOrDefault(triggerLocal);
            }
            else
            {
                shortcutTrigger = _getLocalizedName(triggerLocal, localizations);
                if (((shortcutTrigger is null) && ((logicalKeyId & LogicalKeyboardKey.planeMask) == 0L)))
                {
                    shortcutTrigger = char.ConvertFromUtf32(checked((int)(logicalKeyId & LogicalKeyboardKey.valueMask))).toUpperCase();
                }
                shortcutTrigger ??= ((global::Doroti.Framework.Services.LogicalKeyboardKey)triggerLocal).keyLabel;
            }
            return string.Join(keySeparator, ((Func<List<string>>)(() => { var __collection93661 = new List<string>(); __collection93661.AddRange(modifiers); if (((shortcutTrigger is not null) && (shortcutTrigger.Length != 0))) { __collection93661.Add(shortcutTrigger); } return __collection93661; }))());
        }
        else
        {
            if ((((global::Doroti.Framework.Widgets.ShortcutSerialization)serialized).character is not null))
            {
                var modifiersLocal = ((Func<List<string>>)(() => { var __collection93876 = new List<string>(); if (Menu_anchorLibrary._usesSymbolicModifiers) { __collection93876.AddRange(((Func<List<string>>)(() => { var __collection93989 = new List<string>(); if (DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Widgets.ShortcutSerialization)serialized).control)) { __collection93989.Add(_getModifierLabel(LogicalKeyboardKey.control, localizations)); } if (DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Widgets.ShortcutSerialization)serialized).alt)) { __collection93989.Add(_getModifierLabel(LogicalKeyboardKey.alt, localizations)); } if (DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Widgets.ShortcutSerialization)serialized).meta)) { __collection93989.Add(_getModifierLabel(LogicalKeyboardKey.meta, localizations)); } return __collection93989; }))()); } else { __collection93876.AddRange(((Func<List<string>>)(() => { var __collection94377 = new List<string>(); if (DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Widgets.ShortcutSerialization)serialized).alt)) { __collection94377.Add(_getModifierLabel(LogicalKeyboardKey.alt, localizations)); } if (DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Widgets.ShortcutSerialization)serialized).control)) { __collection94377.Add(_getModifierLabel(LogicalKeyboardKey.control, localizations)); } if (DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Widgets.ShortcutSerialization)serialized).meta)) { __collection94377.Add(_getModifierLabel(LogicalKeyboardKey.meta, localizations)); } return __collection94377; }))()); } return __collection93876; }))();
                return string.Join(keySeparator, ((Func<List<string>>)(() => { var __collection94756 = new List<string>(); __collection94756.AddRange(modifiersLocal); __collection94756.Add(((global::Doroti.Framework.Widgets.ShortcutSerialization)serialized).character!); return __collection94756; }))());
            }
        }
        throw new NotImplementedException("Shortcut labels for ShortcutActivators that do not implement " + "MenuSerializableShortcut (e.g. ShortcutActivators other than SingleActivator or " + "CharacterActivator) are not supported.");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual string? _getLocalizedName(global::Doroti.Framework.Services.LogicalKeyboardKey key, MaterialLocalizations localizations)
    {
        this._cachedShortcutKeys.putIfAbsent(localizations, () => new DartMap<global::Doroti.Framework.Services.LogicalKeyboardKey, string> { [LogicalKeyboardKey.altGraph] = ((MaterialLocalizations)localizations).keyboardKeyAltGraph, [LogicalKeyboardKey.backspace] = ((MaterialLocalizations)localizations).keyboardKeyBackspace, [LogicalKeyboardKey.capsLock] = ((MaterialLocalizations)localizations).keyboardKeyCapsLock, [LogicalKeyboardKey.channelDown] = ((MaterialLocalizations)localizations).keyboardKeyChannelDown, [LogicalKeyboardKey.channelUp] = ((MaterialLocalizations)localizations).keyboardKeyChannelUp, [LogicalKeyboardKey.delete] = ((MaterialLocalizations)localizations).keyboardKeyDelete, [LogicalKeyboardKey.eject] = ((MaterialLocalizations)localizations).keyboardKeyEject, [LogicalKeyboardKey.end] = ((MaterialLocalizations)localizations).keyboardKeyEnd, [LogicalKeyboardKey.escape] = ((MaterialLocalizations)localizations).keyboardKeyEscape, [LogicalKeyboardKey.fn] = ((MaterialLocalizations)localizations).keyboardKeyFn, [LogicalKeyboardKey.home] = ((MaterialLocalizations)localizations).keyboardKeyHome, [LogicalKeyboardKey.insert] = ((MaterialLocalizations)localizations).keyboardKeyInsert, [LogicalKeyboardKey.numLock] = ((MaterialLocalizations)localizations).keyboardKeyNumLock, [LogicalKeyboardKey.numpad1] = ((MaterialLocalizations)localizations).keyboardKeyNumpad1, [LogicalKeyboardKey.numpad2] = ((MaterialLocalizations)localizations).keyboardKeyNumpad2, [LogicalKeyboardKey.numpad3] = ((MaterialLocalizations)localizations).keyboardKeyNumpad3, [LogicalKeyboardKey.numpad4] = ((MaterialLocalizations)localizations).keyboardKeyNumpad4, [LogicalKeyboardKey.numpad5] = ((MaterialLocalizations)localizations).keyboardKeyNumpad5, [LogicalKeyboardKey.numpad6] = ((MaterialLocalizations)localizations).keyboardKeyNumpad6, [LogicalKeyboardKey.numpad7] = ((MaterialLocalizations)localizations).keyboardKeyNumpad7, [LogicalKeyboardKey.numpad8] = ((MaterialLocalizations)localizations).keyboardKeyNumpad8, [LogicalKeyboardKey.numpad9] = ((MaterialLocalizations)localizations).keyboardKeyNumpad9, [LogicalKeyboardKey.numpad0] = ((MaterialLocalizations)localizations).keyboardKeyNumpad0, [LogicalKeyboardKey.numpadAdd] = ((MaterialLocalizations)localizations).keyboardKeyNumpadAdd, [LogicalKeyboardKey.numpadComma] = ((MaterialLocalizations)localizations).keyboardKeyNumpadComma, [LogicalKeyboardKey.numpadDecimal] = ((MaterialLocalizations)localizations).keyboardKeyNumpadDecimal, [LogicalKeyboardKey.numpadDivide] = ((MaterialLocalizations)localizations).keyboardKeyNumpadDivide, [LogicalKeyboardKey.numpadEnter] = ((MaterialLocalizations)localizations).keyboardKeyNumpadEnter, [LogicalKeyboardKey.numpadEqual] = ((MaterialLocalizations)localizations).keyboardKeyNumpadEqual, [LogicalKeyboardKey.numpadMultiply] = ((MaterialLocalizations)localizations).keyboardKeyNumpadMultiply, [LogicalKeyboardKey.numpadParenLeft] = ((MaterialLocalizations)localizations).keyboardKeyNumpadParenLeft, [LogicalKeyboardKey.numpadParenRight] = ((MaterialLocalizations)localizations).keyboardKeyNumpadParenRight, [LogicalKeyboardKey.numpadSubtract] = ((MaterialLocalizations)localizations).keyboardKeyNumpadSubtract, [LogicalKeyboardKey.pageDown] = ((MaterialLocalizations)localizations).keyboardKeyPageDown, [LogicalKeyboardKey.pageUp] = ((MaterialLocalizations)localizations).keyboardKeyPageUp, [LogicalKeyboardKey.power] = ((MaterialLocalizations)localizations).keyboardKeyPower, [LogicalKeyboardKey.powerOff] = ((MaterialLocalizations)localizations).keyboardKeyPowerOff, [LogicalKeyboardKey.printScreen] = ((MaterialLocalizations)localizations).keyboardKeyPrintScreen, [LogicalKeyboardKey.scrollLock] = ((MaterialLocalizations)localizations).keyboardKeyScrollLock, [LogicalKeyboardKey.select] = ((MaterialLocalizations)localizations).keyboardKeySelect, [LogicalKeyboardKey.space] = ((MaterialLocalizations)localizations).keyboardKeySpace });
        return this._cachedShortcutKeys.GetValueOrDefault(localizations)!.GetValueOrDefault(key);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual string _getModifierLabel(global::Doroti.Framework.Services.LogicalKeyboardKey modifier, MaterialLocalizations localizations)
    {
        DartRuntimePrimitives.Assert(() => _modifiers.Contains(modifier), () => (object?)$"{(((global::Doroti.Framework.Services.LogicalKeyboardKey)modifier).keyLabel)} is not a modifier key");
        if ((((Equals(modifier, LogicalKeyboardKey.meta)) || (Equals(modifier, LogicalKeyboardKey.metaLeft))) || (Equals(modifier, LogicalKeyboardKey.metaRight))))
        {
            switch (PlatformLibrary.defaultTargetPlatform)
            {
                case TargetPlatform.android:
                case TargetPlatform.fuchsia:
                case TargetPlatform.linux:
                    {
                        return ((MaterialLocalizations)localizations).keyboardKeyMeta;
                    }
                case TargetPlatform.windows:
                    {
                        return ((MaterialLocalizations)localizations).keyboardKeyMetaWindows;
                    }
                case TargetPlatform.iOS:
                case TargetPlatform.macOS:
                    {
                        return "⌘";
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
        }
        if ((((Equals(modifier, LogicalKeyboardKey.alt)) || (Equals(modifier, LogicalKeyboardKey.altLeft))) || (Equals(modifier, LogicalKeyboardKey.altRight))))
        {
            switch (PlatformLibrary.defaultTargetPlatform)
            {
                case TargetPlatform.android:
                case TargetPlatform.fuchsia:
                case TargetPlatform.linux:
                case TargetPlatform.windows:
                    {
                        return ((MaterialLocalizations)localizations).keyboardKeyAlt;
                    }
                case TargetPlatform.iOS:
                case TargetPlatform.macOS:
                    {
                        return "⌥";
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
        }
        if ((((Equals(modifier, LogicalKeyboardKey.control)) || (Equals(modifier, LogicalKeyboardKey.controlLeft))) || (Equals(modifier, LogicalKeyboardKey.controlRight))))
        {
            switch (PlatformLibrary.defaultTargetPlatform)
            {
                case TargetPlatform.android:
                case TargetPlatform.fuchsia:
                case TargetPlatform.linux:
                case TargetPlatform.windows:
                    {
                        return ((MaterialLocalizations)localizations).keyboardKeyControl;
                    }
                case TargetPlatform.iOS:
                case TargetPlatform.macOS:
                    {
                        return "⌃";
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
        }
        if ((((Equals(modifier, LogicalKeyboardKey.shift)) || (Equals(modifier, LogicalKeyboardKey.shiftLeft))) || (Equals(modifier, LogicalKeyboardKey.shiftRight))))
        {
            switch (PlatformLibrary.defaultTargetPlatform)
            {
                case TargetPlatform.android:
                case TargetPlatform.fuchsia:
                case TargetPlatform.linux:
                case TargetPlatform.windows:
                    {
                        return ((MaterialLocalizations)localizations).keyboardKeyShift;
                    }
                case TargetPlatform.iOS:
                case TargetPlatform.macOS:
                    {
                        return "⇧";
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
        }
        throw DartRuntimePrimitives.AsException(new DartArgumentError($"Keyboard key {(((global::Doroti.Framework.Services.LogicalKeyboardKey)modifier).keyLabel)} is not a modifier."));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MenuBarAnchor__menu_anchor : MenuAnchor
{
    internal _MenuBarAnchor__menu_anchor(List<global::Doroti.Framework.Widgets.Widget> menuChildren, global::Doroti.Framework.Widgets.MenuController? controller = null, Clip clipBehavior = Clip.hardEdge, MenuStyle? style = null) : base(menuChildren: menuChildren, controller: controller, clipBehavior: clipBehavior, style: style)
    {
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _MenuBarAnchorState__menu_anchor());
}

internal class _MenuBarAnchorState__menu_anchor : _MenuAnchorState__menu_anchor
{
    private bool __late_actions_initialized;
    private DartMap<Type, dynamic> __late_actions = default!;
    public virtual DartMap<Type, dynamic> actions
    {
        get
        {
            if (!__late_actions_initialized)
            {
                __late_actions = new DartMap<Type, dynamic> { [typeof(global::Doroti.Framework.Widgets.DismissIntent)] = new global::Doroti.Framework.Widgets.DismissMenuAction(controller: this._menuController) };
                __late_actions_initialized = true;
            }
            return __late_actions;
        }
    }

    internal override global::Doroti.Framework.Painting.Axis _orientation => Axis.horizontal;
    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        var childLocal = new global::Doroti.Framework.Widgets.Actions(actions: this.actions, child: new global::Doroti.Framework.Widgets.Shortcuts(shortcuts: Menu_anchorLibrary._kMenuTraversalShortcuts, child: new _MenuPanel__menu_anchor(menuStyle: ((MenuAnchor)this.widget).style, clipBehavior: ((MenuAnchor)this.widget).clipBehavior, orientation: this._orientation, children: ((MenuAnchor)this.widget).menuChildren)));
        return ((global::Doroti.Framework.Widgets.Widget)new _MenuAnchorScope__menu_anchor(state: this, animationStatus: ((global::Doroti.Framework.Animation.AnimationController)this._animationController).status, child: new global::Doroti.Framework.Widgets.RawMenuAnchorGroup(controller: this._menuController, child: new global::Doroti.Framework.Widgets.Builder(builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)((context) =>
        {
            bool isOpen = (MenuController.maybeIsOpenOf(context) ?? false);
            return ((global::Doroti.Framework.Widgets.Widget)new global::Doroti.Framework.Widgets.FocusScope(node: this._menuScopeNode, skipTraversal: !isOpen, canRequestFocus: isOpen, descendantsAreFocusable: true, child: new global::Doroti.Framework.Widgets.ExcludeFocus(excluding: !isOpen, child: childLocal)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class MenuAcceleratorCallbackBinding : global::Doroti.Framework.Widgets.InheritedWidget
{
    public virtual global::System.Action? onInvoke { get; private set; }
    public virtual bool hasSubmenu { get; private set; } = default!;

    public MenuAcceleratorCallbackBinding(global::Doroti.Framework.Foundation.Key? key = null, global::System.Action? onInvoke = null, bool hasSubmenu = false, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.onInvoke = onInvoke;
        this.hasSubmenu = hasSubmenu;
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget)
    {
        var __oldWidget = (MenuAcceleratorCallbackBinding)oldWidget;
        return ((!Equals((global::System.Action?)this.onInvoke, (global::System.Action?)((MenuAcceleratorCallbackBinding)__oldWidget).onInvoke)) || (this.hasSubmenu != ((MenuAcceleratorCallbackBinding)__oldWidget).hasSubmenu));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static MenuAcceleratorCallbackBinding? maybeOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((MenuAcceleratorCallbackBinding?)context.dependOnInheritedWidgetOfExactType<MenuAcceleratorCallbackBinding>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static MenuAcceleratorCallbackBinding of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        MenuAcceleratorCallbackBinding? result = ((MenuAcceleratorCallbackBinding?)maybeOf(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((result is null))
                {
                    throw DartRuntimePrimitives.AsException(FlutterError.Create("MenuAcceleratorWrapper.of() was called with a context that does not " + "contain a MenuAcceleratorWrapper in the given context.\n" + "No MenuAcceleratorWrapper ancestor could be found in the context that " + "was passed to MenuAcceleratorWrapper.of(). This can happen because " + "you are using a widget that looks for a MenuAcceleratorWrapper " + "ancestor, and do not have a MenuAcceleratorWrapper widget ancestor.\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
            });
        return result!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public delegate global::Doroti.Framework.Widgets.Widget MenuAcceleratorChildBuilder(global::Doroti.Framework.Widgets.BuildContext context, string label, long index);

public class MenuAcceleratorLabel : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual string label { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, string, long, global::Doroti.Framework.Widgets.Widget> builder { get; private set; } = default!;

    public MenuAcceleratorLabel(string label, global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, string, long, global::Doroti.Framework.Widgets.Widget> builder = default!) : base(key: key)
    {
        global::System.Func<global::Doroti.Framework.Widgets.BuildContext, string, long, global::Doroti.Framework.Widgets.Widget> __builder = builder ?? defaultLabelBuilder;
        this.label = label;
        this.builder = __builder;
    }

    public virtual string displayLabel => stripAcceleratorMarkers(this.label);
    public virtual bool hasAccelerator => new RegExp("&(?!([&\\s]|$))").hasMatch(this.label);
    public static global::Doroti.Framework.Widgets.Widget defaultLabelBuilder(global::Doroti.Framework.Widgets.BuildContext context, string label, long index)
    {
        if ((index < 0L))
        {
            return ((global::Doroti.Framework.Widgets.Widget)new global::Doroti.Framework.Widgets.Text(label));
        }
        global::Doroti.Framework.Painting.TextStyle defaultStyle = DefaultTextStyle.of(context).style;
        Characters charactersLocal = label.characters();
        return ((global::Doroti.Framework.Widgets.Widget)new global::Doroti.Framework.Widgets.RichText(text: new global::Doroti.Framework.Painting.TextSpan(children: ((Func<List<global::Doroti.Framework.Painting.TextSpan>>)(() => { var __collection113074 = new List<global::Doroti.Framework.Painting.TextSpan>(); if ((index > 0L)) { __collection113074.Add(new global::Doroti.Framework.Painting.TextSpan(text: charactersLocal.GetRange(0L, index).ToString(), style: defaultStyle)); } __collection113074.Add(new global::Doroti.Framework.Painting.TextSpan(text: charactersLocal.skip(index).take(1L).ToString(), style: defaultStyle.copyWith(decoration: TextDecoration.underline))); if ((index < (charactersLocal.Count - 1L))) { __collection113074.Add(new global::Doroti.Framework.Painting.TextSpan(text: charactersLocal.GetRange((index + 1L)).ToString(), style: defaultStyle)); } return __collection113074; }))().Cast<global::Doroti.Framework.Painting.InlineSpan>().ToList())));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static string stripAcceleratorMarkers(string label, global::System.Action<long>? setIndex = null)
    {
        var quotedAmpersands = 0L;
        var displayLabel = new StringBuffer();
        var acceleratorIndex = -1L;
        Characters labelChars = label.characters();
        Characters ampersand = "&".characters();
        var lastWasAmpersand = false;
        for (var i = 0L; (i < labelChars.Count); i += 1L)
        {
            Characters character = labelChars.characterAt(i);
            if (lastWasAmpersand)
            {
                lastWasAmpersand = false;
                displayLabel.write(character);
                continue;
            }
            if ((!Equals(character, ampersand)))
            {
                displayLabel.write(character);
                continue;
            }
            if ((i == (labelChars.Count - 1L)))
            {
                break;
            }
            lastWasAmpersand = true;
            Characters acceleratorCharacter = labelChars.characterAt((i + 1L));
            if ((((acceleratorIndex == -1L) && (!Equals(acceleratorCharacter, ampersand))) && (acceleratorCharacter.ToString()?.Trim().Length != 0)))
            {
                acceleratorIndex = (i - quotedAmpersands);
            }
            quotedAmpersands += 1L;
        }
        setIndex?.Invoke(acceleratorIndex);
        return ((string)displayLabel.ToString());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _MenuAcceleratorLabelState__menu_anchor());
    public override string ToString(global::Doroti.Framework.Foundation.DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        return $"{typeof(MenuAcceleratorLabel)}(\"{this.label}\")";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.StringProperty("label", this.label));
    }

}

internal class _MenuAcceleratorLabelState__menu_anchor : global::Doroti.Framework.Widgets.State<MenuAcceleratorLabel>
{
    internal virtual string _displayLabel { get; set; } = default!;
    internal virtual long _acceleratorIndex { get; set; } = -1L;
    internal virtual MenuAcceleratorCallbackBinding? _binding { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.MenuController? _menuController { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.ShortcutRegistry? _shortcutRegistry { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.ShortcutRegistryEntry? _shortcutRegistryEntry { get; set; } = default;
    internal virtual bool _showAccelerators { get; set; } = false;

    public override void initState()
    {
        base.initState();
        if (Menu_anchorLibrary._platformSupportsAccelerators)
        {
            _showAccelerators = _altIsPressed();
            HardwareKeyboard.instance.addHandler((global::System.Func<global::Doroti.Framework.Services.KeyEvent, bool>)this._listenToKeyEvent);
        }
        _updateDisplayLabel();
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() => (Menu_anchorLibrary._platformSupportsAccelerators || (this._shortcutRegistryEntry is null)));
        _displayLabel = "";
        if (Menu_anchorLibrary._platformSupportsAccelerators)
        {
            this._shortcutRegistryEntry?.dispose();
            _shortcutRegistryEntry = null;
            _shortcutRegistry = null;
            _menuController = null;
            HardwareKeyboard.instance.removeHandler((global::System.Func<global::Doroti.Framework.Services.KeyEvent, bool>)this._listenToKeyEvent);
        }
        base.dispose();
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        if (!Menu_anchorLibrary._platformSupportsAccelerators)
        {
            return;
        }
        _binding = MenuAcceleratorCallbackBinding.maybeOf(this.context);
        _menuController = MenuController.maybeOf(this.context);
        _shortcutRegistry = ShortcutRegistry.maybeOf(this.context);
        _updateAcceleratorShortcut();
    }

    public override void didUpdateWidget(MenuAcceleratorLabel oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((((MenuAcceleratorLabel)this.widget).label != ((MenuAcceleratorLabel)oldWidget).label))
        {
            _updateDisplayLabel();
        }
    }

    internal static bool _altIsPressed()
    {
        return Enumerable.Any(HardwareKeyboard.instance.logicalKeysPressed.intersection(new HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> { LogicalKeyboardKey.altLeft, LogicalKeyboardKey.altRight, LogicalKeyboardKey.alt }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _listenToKeyEvent(global::Doroti.Framework.Services.KeyEvent @event)
    {
        DartRuntimePrimitives.Assert(() => Menu_anchorLibrary._platformSupportsAccelerators);
        setState(((global::System.Action)(() =>
        {
            _showAccelerators = _altIsPressed();
            _updateAcceleratorShortcut();
        })));
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _updateAcceleratorShortcut()
    {
        DartRuntimePrimitives.Assert(() => Menu_anchorLibrary._platformSupportsAccelerators);
        this._shortcutRegistryEntry?.dispose();
        _shortcutRegistryEntry = null;
        if ((((this._showAccelerators && (this._acceleratorIndex != -1L)) && (this._binding?.onInvoke is not null)) && ((!this._binding!.hasSubmenu || !((this._menuController?.isOpen ?? false))))))
        {
            string acceleratorCharacter = this._displayLabel[(int)(this._acceleratorIndex)].ToString().toLowerCase();
            _shortcutRegistryEntry = this._shortcutRegistry?.addAll(new DartMap<global::Doroti.Framework.Widgets.ShortcutActivator, global::Doroti.Framework.Widgets.Intent> { [new global::Doroti.Framework.Widgets.CharacterActivator(acceleratorCharacter, alt: true)] = ((global::Doroti.Framework.Widgets.Intent)new global::Doroti.Framework.Widgets.VoidCallbackIntent(this._binding!.onInvoke!)) });
        }
    }

    internal virtual void _updateDisplayLabel()
    {
        _displayLabel = MenuAcceleratorLabel.stripAcceleratorMarkers(((MenuAcceleratorLabel)this.widget).label, setIndex: ((global::System.Action<long>)((index) =>
        {
            _acceleratorIndex = index;
        })));
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        long index = (this._showAccelerators ? this._acceleratorIndex : -1L);
        return this.widget.builder(context, this._displayLabel, index);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MenuItemLabel__menu_anchor : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual bool hasSubmenu { get; private set; } = default!;
    public virtual bool showDecoration { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? leadingIcon { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? trailingIcon { get; private set; }
    public virtual global::Doroti.Framework.Widgets.MenuSerializableShortcut? shortcut { get; private set; }
    public virtual string? semanticsLabel { get; private set; }
    public virtual global::Doroti.Framework.Painting.Axis overflowAxis { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? submenuIcon { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }

    internal _MenuItemLabel__menu_anchor(bool hasSubmenu, bool showDecoration = true, global::Doroti.Framework.Widgets.Widget? leadingIcon = null, global::Doroti.Framework.Widgets.Widget? trailingIcon = null, global::Doroti.Framework.Widgets.MenuSerializableShortcut? shortcut = null, string? semanticsLabel = null, global::Doroti.Framework.Painting.Axis overflowAxis = Axis.vertical, global::Doroti.Framework.Widgets.Widget? submenuIcon = null, global::Doroti.Framework.Widgets.Widget? child = null)
    {
        this.hasSubmenu = hasSubmenu;
        this.showDecoration = showDecoration;
        this.leadingIcon = leadingIcon;
        this.trailingIcon = trailingIcon;
        this.shortcut = shortcut;
        this.semanticsLabel = semanticsLabel;
        this.overflowAxis = overflowAxis;
        this.submenuIcon = submenuIcon;
        this.child = child;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        VisualDensity density = Theme.of(context).visualDensity;
        double horizontalPadding = Math.Max(Menu_anchorLibrary._kLabelItemMinSpacing, (Menu_anchorLibrary._kLabelItemDefaultSpacing + (density.horizontal * 2L)));
        global::Doroti.Framework.Widgets.Widget leadings = default!;
        if ((Equals(this.overflowAxis, Axis.vertical)))
        {
            leadings = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.ClipRect(child: new global::Doroti.Framework.Widgets.Row(mainAxisSize: MainAxisSize.min, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection121981 = new List<global::Doroti.Framework.Widgets.Widget>(); var __collectionElement122005 = this.leadingIcon; if (__collectionElement122005 is { } __nonNullCollectionElement122005) { __collection121981.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement122005)); } if ((this.child is not null)) { __collection121981.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.ClipRect(child: new global::Doroti.Framework.Widgets.Padding(padding: ((this.leadingIcon is not null) ? global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: horizontalPadding) : global::Doroti.Framework.Painting.EdgeInsets.zero), child: this.child))))); } return __collection121981; }))()))));
        }
        else
        {
            leadings = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Row(mainAxisSize: MainAxisSize.min, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection122566 = new List<global::Doroti.Framework.Widgets.Widget>(); var __collectionElement122586 = this.leadingIcon; if (__collectionElement122586 is { } __nonNullCollectionElement122586) { __collection122566.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement122586)); } if ((this.child is not null)) { __collection122566.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: ((this.leadingIcon is not null) ? global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: horizontalPadding) : global::Doroti.Framework.Painting.EdgeInsets.zero), child: this.child))); } return __collection122566; }))()));
        }
        global::Doroti.Framework.Widgets.Widget menuItemLabel = ((global::Doroti.Framework.Widgets.Widget)new global::Doroti.Framework.Widgets.Row(mainAxisAlignment: MainAxisAlignment.spaceBetween, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection122978 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection122978.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(leadings)); if ((this.trailingIcon is not null)) { __collection122978.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsetsDirectional.CreateOnly(start: horizontalPadding), child: this.trailingIcon))); } if ((this.showDecoration && (this.shortcut is not null))) { __collection122978.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsetsDirectional.CreateOnly(start: horizontalPadding), child: new global::Doroti.Framework.Widgets.Text(_LocalizedShortcutLabeler__menu_anchor.instance.getShortcutLabel(this.shortcut!, MaterialLocalizations.of(context)))))); } if ((this.showDecoration && this.hasSubmenu)) { __collection122978.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsetsDirectional.CreateOnly(start: horizontalPadding), child: this.submenuIcon))); } return __collection122978; }))()));
        if ((this.semanticsLabel is not null))
        {
            menuItemLabel = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(label: this.semanticsLabel, excludeSemantics: true, child: menuItemLabel));
        }
        return menuItemLabel;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.MenuSerializableShortcut>("shortcut", this.shortcut, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("hasSubmenu", this.hasSubmenu));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("showDecoration", this.showDecoration));
    }

}

internal class _MenuLayout__menu_anchor : global::Doroti.Framework.Rendering.SingleChildLayoutDelegate
{
    public virtual Rect anchorRect { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;
    public virtual Offset alignmentOffset { get; private set; } = default!;
    public virtual Offset? menuPosition { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry menuPadding { get; private set; } = default!;
    public virtual HashSet<Rect> avoidBounds { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.Axis orientation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.Axis parentOrientation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry reservedPadding { get; private set; } = default!;
    public virtual double heightFactor { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.MediaQueryData mediaQueryData { get; private set; } = default!;

    internal _MenuLayout__menu_anchor(Rect anchorRect, TextDirection textDirection, global::Doroti.Framework.Painting.AlignmentGeometry alignment, Offset alignmentOffset, Offset? menuPosition, global::Doroti.Framework.Painting.EdgeInsetsGeometry menuPadding, HashSet<Rect> avoidBounds, global::Doroti.Framework.Painting.Axis orientation, global::Doroti.Framework.Painting.Axis parentOrientation, global::Doroti.Framework.Painting.EdgeInsetsGeometry reservedPadding, double heightFactor, global::Doroti.Framework.Widgets.MediaQueryData mediaQueryData)
    {
        this.anchorRect = anchorRect;
        this.textDirection = textDirection;
        this.alignment = alignment;
        this.alignmentOffset = alignmentOffset;
        this.menuPosition = menuPosition;
        this.menuPadding = menuPadding;
        this.avoidBounds = avoidBounds;
        this.orientation = orientation;
        this.parentOrientation = parentOrientation;
        this.reservedPadding = reservedPadding;
        this.heightFactor = heightFactor;
        this.mediaQueryData = mediaQueryData;
    }

    public override global::Doroti.Framework.Rendering.BoxConstraints getConstraintsForChild(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return ((global::Doroti.Framework.Rendering.BoxConstraints)BoxConstraints.CreateLoose(((global::Doroti.Framework.Rendering.BoxConstraints)constraints).biggest).deflate(this.reservedPadding));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Offset getPositionForChild(Size size, Size childSize)
    {
        global::Doroti.Ui.Rect overlayRect = ((global::Doroti.Ui.Rect)((global::Doroti.Framework.Widgets.MediaQueryData)this.mediaQueryData).padding.deflateRect(((global::Doroti.Framework.Widgets.MediaQueryData)this.mediaQueryData).viewInsets.deflateRect((Offset.zero & size))));
        double unconstrainedHeight = ((this.heightFactor > 0.01) ? (childSize.height / this.heightFactor) : 0);
        double childHeightEstimate = Math.Min(unconstrainedHeight, size.height);
        var childSizeEstimate = new global::Doroti.Ui.Size(childSize.width, childHeightEstimate);
        global::Doroti.Ui.Offset finalPosition = ((global::Doroti.Ui.Offset)_positionChild(childSizeEstimate, overlayRect));
        if ((this.menuPosition is not null))
        {
            Offset menuPosition__value127155 = DartRuntimePrimitives.RequireValue(menuPosition);
            return finalPosition;
        }
        bool growsUp = ((finalPosition.dy + childSizeEstimate.height) <= ((Offset)(this.anchorRect).center).dy);
        if (growsUp)
        {
            double dyLocal = (childHeightEstimate - childSize.height);
            return new global::Doroti.Ui.Offset(finalPosition.dx, (finalPosition.dy + dyLocal));
        }
        var initialPosition = new global::Doroti.Ui.Offset(finalPosition.dx, this.anchorRect.bottom);
        return DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Offset.lerp(initialPosition, finalPosition, this.heightFactor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Offset _positionChild(Size childSize, Rect overlayRect)
    {
        double xLocal = default!;
        double yLocal = default!;
        if ((this.menuPosition is null))
        {
            global::Doroti.Ui.Offset desiredPosition = ((global::Doroti.Ui.Offset)this.alignment.resolve(this.textDirection).withinRect(this.anchorRect));
            global::Doroti.Ui.Offset directionalOffset = default!;
            if ((this.alignment is global::Doroti.Framework.Painting.AlignmentDirectional))
            {
                global::Doroti.Framework.Painting.AlignmentDirectional alignment__as128030 = (global::Doroti.Framework.Painting.AlignmentDirectional)alignment;
                directionalOffset = (this.textDirection switch { TextDirection.rtl => new global::Doroti.Ui.Offset(-this.alignmentOffset.dx, this.alignmentOffset.dy), TextDirection.ltr => this.alignmentOffset, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            }
            else
            {
                directionalOffset = this.alignmentOffset;
            }
            desiredPosition += directionalOffset;
            xLocal = desiredPosition.dx;
            yLocal = desiredPosition.dy;
            switch (this.textDirection)
            {
                case TextDirection.rtl:
                    {
                        xLocal -= childSize.width;
                        break;
                    }
                case TextDirection.ltr:
                    {
                        break;
                    }
            }
        }
        else
        {
            global::Doroti.Ui.Offset adjustedPosition = ((global::Doroti.Ui.Offset)(DartRuntimePrimitives.RequireValue(this.menuPosition) + this.anchorRect.topLeft));
            xLocal = adjustedPosition.dx;
            yLocal = adjustedPosition.dy;
        }
        IEnumerable<global::Doroti.Ui.Rect> subScreens = ((IEnumerable<global::Doroti.Ui.Rect>)DisplayFeatureSubScreen.subScreensInBounds(overlayRect, this.avoidBounds));
        global::Doroti.Ui.Rect allowedRect = ((global::Doroti.Ui.Rect)_closestScreen(subScreens.Cast<Rect>(), ((Offset)(this.anchorRect).center)));
        bool offLeftSide(double x)
        {
            return (x < allowedRect.left);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        bool offRightSide(double x)
        {
            return ((x + childSize.width) > allowedRect.right);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        bool offTop(double y)
        {
            return (y < allowedRect.top);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        bool offBottom(double y)
        {
            return ((y + childSize.height) > allowedRect.bottom);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        if ((childSize.width >= allowedRect.width))
        {
            xLocal = allowedRect.left;
        }
        else
        {
            if (offLeftSide(xLocal))
            {
                if ((!Equals(this.parentOrientation, this.orientation)))
                {
                    xLocal = allowedRect.left;
                }
                else
                {
                    double newX = (this.anchorRect.right + this.alignmentOffset.dx);
                    if (!offRightSide(newX))
                    {
                        xLocal = newX;
                    }
                    else
                    {
                        xLocal = allowedRect.left;
                    }
                }
            }
            else
            {
                if (offRightSide(xLocal))
                {
                    if ((!Equals(this.parentOrientation, this.orientation)))
                    {
                        xLocal = (allowedRect.right - childSize.width);
                    }
                    else
                    {
                        double newXLocal = ((this.anchorRect.left - childSize.width) - this.alignmentOffset.dx);
                        if (!offLeftSide(newXLocal))
                        {
                            xLocal = newXLocal;
                        }
                        else
                        {
                            xLocal = (allowedRect.right - childSize.width);
                        }
                    }
                }
            }
        }
        if ((childSize.height >= allowedRect.height))
        {
            yLocal = allowedRect.top;
        }
        else
        {
            if (offTop(yLocal))
            {
                double newY = this.anchorRect.bottom;
                if (!offBottom(newY))
                {
                    yLocal = newY;
                }
                else
                {
                    yLocal = allowedRect.top;
                }
            }
            else
            {
                if (offBottom(yLocal))
                {
                    double newYLocal = (this.anchorRect.top - childSize.height);
                    if (!offTop(newYLocal))
                    {
                        if ((Equals(this.parentOrientation, Axis.horizontal)))
                        {
                            yLocal = (newYLocal - this.alignmentOffset.dy);
                        }
                        else
                        {
                            yLocal = newYLocal;
                        }
                    }
                    else
                    {
                        yLocal = (allowedRect.bottom - childSize.height);
                    }
                }
            }
        }
        return new global::Doroti.Ui.Offset(xLocal, yLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRelayout(global::Doroti.Framework.Rendering.SingleChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (_MenuLayout__menu_anchor)oldDelegate;
        return (((((((((((!Equals(this.anchorRect, ((_MenuLayout__menu_anchor)__oldDelegate).anchorRect)) || (!Equals(this.textDirection, ((_MenuLayout__menu_anchor)__oldDelegate).textDirection))) || (!Equals(this.alignment, ((_MenuLayout__menu_anchor)__oldDelegate).alignment))) || (!Equals(this.alignmentOffset, ((_MenuLayout__menu_anchor)__oldDelegate).alignmentOffset))) || (!Equals(this.menuPosition, ((_MenuLayout__menu_anchor)__oldDelegate).menuPosition))) || (!Equals(this.menuPadding, ((_MenuLayout__menu_anchor)__oldDelegate).menuPadding))) || (!Equals(this.orientation, ((_MenuLayout__menu_anchor)__oldDelegate).orientation))) || (!Equals(this.parentOrientation, ((_MenuLayout__menu_anchor)__oldDelegate).parentOrientation))) || (!Equals(this.reservedPadding, ((_MenuLayout__menu_anchor)__oldDelegate).reservedPadding))) || (this.heightFactor != ((_MenuLayout__menu_anchor)__oldDelegate).heightFactor)) || !CollectionsLibrary.setEquals(this.avoidBounds, ((_MenuLayout__menu_anchor)__oldDelegate).avoidBounds));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Rect _closestScreen(IEnumerable<Rect> screens, Offset point)
    {
        global::Doroti.Ui.Rect closest = ((global::Doroti.Ui.Rect)screens.First());
        foreach (var screen in screens)
        {
            if ((((((Offset)(screen).center) - point)).distance < ((((Offset)(closest).center) - point)).distance))
            {
                closest = screen;
            }
        }
        return closest;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MenuPanel__menu_anchor : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual MenuStyle? menuStyle { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual bool crossAxisUnconstrained { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.Axis orientation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double>? heightAnimation { get; private set; }
    public virtual List<global::Doroti.Framework.Widgets.Widget> children { get; private set; } = default!;

    internal _MenuPanel__menu_anchor(MenuStyle? menuStyle, Clip clipBehavior = Clip.none, global::Doroti.Framework.Painting.Axis orientation = default!, bool crossAxisUnconstrained = true, global::Doroti.Framework.Animation.Animation<double>? heightAnimation = null, List<global::Doroti.Framework.Widgets.Widget> children = default!)
    {
        this.menuStyle = menuStyle;
        this.clipBehavior = clipBehavior;
        this.orientation = orientation;
        this.crossAxisUnconstrained = crossAxisUnconstrained;
        this.heightAnimation = heightAnimation;
        this.children = children;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _MenuPanelState__menu_anchor());
}

internal class _MenuPanelState__menu_anchor : global::Doroti.Framework.Widgets.State<_MenuPanel__menu_anchor>
{
    public virtual global::Doroti.Framework.Widgets.ScrollController scrollController { get; set; } = new global::Doroti.Framework.Widgets.ScrollController();

    public override void dispose()
    {
        this.scrollController.dispose();
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        (MenuStyle? themeStyle, MenuStyle defaultStyle) = this.widget.orientation switch
        {
            Axis.horizontal => (MenuBarTheme.of(context).style, (MenuStyle)new _MenuBarDefaultsM3__menu_anchor(context)),
            Axis.vertical => (MenuTheme.of(context).style, (MenuStyle)new _MenuDefaultsM3__menu_anchor(context)),
            _ => throw new InvalidOperationException("Unexpected menu orientation.")
        };
        MenuStyle? widgetStyle = ((_MenuPanel__menu_anchor)this.widget).menuStyle;
        P? effectiveValue<P>(global::System.Func<MenuStyle?, P?> getProperty)
        {
            return ((getProperty(widgetStyle) ?? getProperty(themeStyle)) ?? getProperty(defaultStyle));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        P? resolve<P>(global::System.Func<MenuStyle?, global::Doroti.Framework.Widgets.WidgetStateProperty<P>?> getProperty)
        {
            return effectiveValue(((style) =>
            {
                return getProperty(style) is { } property ? property.resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState>()) : default;
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        global::Doroti.Ui.Color? backgroundColorLocal = ((global::Doroti.Ui.Color?)resolve<global::Doroti.Ui.Color?>(((style) => style?.backgroundColor)));
        global::Doroti.Ui.Color? shadowColorLocal = ((global::Doroti.Ui.Color?)resolve<global::Doroti.Ui.Color?>(((style) => style?.shadowColor)));
        global::Doroti.Ui.Color? surfaceTintColorLocal = ((global::Doroti.Ui.Color?)resolve<global::Doroti.Ui.Color?>(((style) => style?.surfaceTintColor)));
        double elevationLocal = (resolve<double?>(((style) => style?.elevation)) ?? 0);
        global::Doroti.Ui.Size? minimumSizeLocal = ((global::Doroti.Ui.Size?)resolve<global::Doroti.Ui.Size?>(((style) => style?.minimumSize)));
        global::Doroti.Ui.Size? fixedSizeLocal = ((global::Doroti.Ui.Size?)resolve<global::Doroti.Ui.Size?>(((style) => style?.fixedSize)));
        global::Doroti.Ui.Size? maximumSizeLocal = ((global::Doroti.Ui.Size?)resolve<global::Doroti.Ui.Size?>(((style) => style?.maximumSize)));
        global::Doroti.Framework.Painting.BorderSide? sideLocal = resolve<global::Doroti.Framework.Painting.BorderSide?>(((style) => style?.side));
        global::Doroti.Framework.Painting.OutlinedBorder shapeLocal = ((global::Doroti.Framework.Painting.OutlinedBorder)resolve<global::Doroti.Framework.Painting.OutlinedBorder?>(((style) => style?.shape))!.copyWith(side: sideLocal));
        VisualDensity visualDensityLocal = (effectiveValue(((style) => style?.visualDensity)) ?? VisualDensity.standard);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry paddingLocal = (resolve<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>(((style) => style?.padding)) ?? EdgeInsets.zero);
        global::Doroti.Ui.Offset densityAdjustment = ((global::Doroti.Ui.Offset)visualDensityLocal.baseSizeAdjustment);
        double dxLocal = Math.Max(0, densityAdjustment.dx);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry resolvedPadding = ((global::Doroti.Framework.Painting.EdgeInsetsGeometry)paddingLocal.add(EdgeInsets.CreateSymmetric(horizontal: dxLocal)).clamp(EdgeInsets.zero, EdgeInsetsGeometry.infinity));
        global::Doroti.Framework.Rendering.BoxConstraints effectiveConstraintsLocal = visualDensityLocal.effectiveConstraints(new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: (minimumSizeLocal?.width ?? 0), minHeight: (minimumSizeLocal?.height ?? 0), maxWidth: (maximumSizeLocal?.width ?? double.PositiveInfinity), maxHeight: (maximumSizeLocal?.height ?? double.PositiveInfinity)));
        if ((fixedSizeLocal is not null))
        {
            Size fixedSize__135131__value136698 = DartRuntimePrimitives.RequireValue(fixedSizeLocal);
            global::Doroti.Ui.Size size = ((global::Doroti.Ui.Size)effectiveConstraintsLocal.constrain(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(fixedSize__135131__value136698))));
            if (double.IsFinite(size.width))
            {
                effectiveConstraintsLocal = effectiveConstraintsLocal.copyWith(minWidth: size.width, maxWidth: size.width);
            }
            if (double.IsFinite(size.height))
            {
                effectiveConstraintsLocal = effectiveConstraintsLocal.copyWith(minHeight: size.height, maxHeight: size.height);
            }
        }
        List<global::Doroti.Framework.Widgets.Widget> childrenLocal = ((_MenuPanel__menu_anchor)this.widget).children.ToList();
        if ((Equals(((_MenuPanel__menu_anchor)this.widget).orientation, Axis.horizontal)))
        {
            childrenLocal = childrenLocal.map<global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Widgets.Widget>(((child) =>
            {
                return new global::Doroti.Framework.Widgets.IntrinsicWidth(child: child);
                throw new InvalidOperationException("Dart closure completed without a value.");
            })).ToList();
        }
        bool displayScrollbar = (_MenuAnchorState__menu_anchor._maybeAnimationStatusOf(context) switch { AnimationStatus.completed => true, AnimationStatus.forward or AnimationStatus.reverse or AnimationStatus.dismissed => false, null => false, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Framework.Widgets.Widget menuPanel = ((global::Doroti.Framework.Widgets.Widget)new global::Doroti.Framework.Widgets.Padding(padding: resolvedPadding, child: new global::Doroti.Framework.Widgets.ScrollConfiguration(behavior: ScrollConfiguration.of(context).copyWith(scrollbars: false, overscroll: false, physics: new global::Doroti.Framework.Widgets.ClampingScrollPhysics()), child: new global::Doroti.Framework.Widgets.PrimaryScrollController(controller: this.scrollController, child: new Scrollbar(thumbVisibility: displayScrollbar, child: new global::Doroti.Framework.Widgets.SingleChildScrollView(controller: this.scrollController, scrollDirection: ((_MenuPanel__menu_anchor)this.widget).orientation, child: new global::Doroti.Framework.Widgets.Flex(crossAxisAlignment: CrossAxisAlignment.start, textDirection: Directionality.of(context), direction: ((_MenuPanel__menu_anchor)this.widget).orientation, mainAxisSize: MainAxisSize.min, children: childrenLocal)))))));
        if ((((_MenuPanel__menu_anchor)this.widget).heightAnimation is not null))
        {
            menuPanel = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: ((_MenuPanel__menu_anchor)this.widget).heightAnimation!, builder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)this._buildAnimatedHeight, child: menuPanel));
        }
        menuPanel = _intrinsicCrossSize(child: new Material(elevation: elevationLocal, shape: shapeLocal, color: backgroundColorLocal, shadowColor: shadowColorLocal, surfaceTintColor: surfaceTintColorLocal, type: ((backgroundColorLocal is null) ? MaterialType.transparency : MaterialType.canvas), clipBehavior: ((_MenuPanel__menu_anchor)this.widget).clipBehavior, child: menuPanel));
        if (((_MenuPanel__menu_anchor)this.widget).crossAxisUnconstrained)
        {
            menuPanel = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.UnconstrainedBox(constrainedAxis: ((_MenuPanel__menu_anchor)this.widget).orientation, clipBehavior: Clip.hardEdge, alignment: AlignmentDirectional.centerStart, child: menuPanel));
        }
        return ((global::Doroti.Framework.Widgets.Widget)new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: effectiveConstraintsLocal, child: menuPanel));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _intrinsicCrossSize(global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(((_MenuPanel__menu_anchor)this.widget).orientation switch { Axis.horizontal => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.SingleChildRenderObjectWidget>(new global::Doroti.Framework.Widgets.IntrinsicHeight(child: child)), Axis.vertical => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.SingleChildRenderObjectWidget>(new global::Doroti.Framework.Widgets.IntrinsicWidth(child: child)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildAnimatedHeight(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget? child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)new global::Doroti.Framework.Widgets.Align(alignment: AlignmentDirectional.topStart, heightFactor: ((_MenuPanel__menu_anchor)this.widget).heightAnimation!.value, widthFactor: 1, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _Submenu__menu_anchor : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Widgets.FocusScopeNode menuScopeNode { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.RawMenuOverlayInfo menuPosition { get; private set; } = default!;
    public virtual _MenuAnchorState__menu_anchor anchor { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.LayerLink? layerLink { get; private set; }
    public virtual MenuStyle? menuStyle { get; private set; }
    public virtual bool consumeOutsideTaps { get; private set; } = default!;
    public virtual Offset alignmentOffset { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual bool crossAxisUnconstrained { get; private set; } = default!;
    public virtual List<global::Doroti.Framework.Widgets.Widget> menuChildren { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> fadeAnimation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> heightAnimation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry reservedPadding { get; private set; } = default!;

    internal _Submenu__menu_anchor(_MenuAnchorState__menu_anchor anchor, global::Doroti.Framework.Rendering.LayerLink? layerLink, MenuStyle? menuStyle, global::Doroti.Framework.Widgets.RawMenuOverlayInfo menuPosition, Offset alignmentOffset, bool consumeOutsideTaps, Clip clipBehavior, bool crossAxisUnconstrained = true, List<global::Doroti.Framework.Widgets.Widget> menuChildren = default!, global::Doroti.Framework.Widgets.FocusScopeNode menuScopeNode = default!, global::Doroti.Framework.Animation.Animation<double> fadeAnimation = default!, global::Doroti.Framework.Animation.Animation<double> heightAnimation = default!, global::Doroti.Framework.Painting.EdgeInsetsGeometry reservedPadding = default!)
    {
        this.anchor = anchor;
        this.layerLink = layerLink;
        this.menuStyle = menuStyle;
        this.menuPosition = menuPosition;
        this.alignmentOffset = alignmentOffset;
        this.consumeOutsideTaps = consumeOutsideTaps;
        this.clipBehavior = clipBehavior;
        this.crossAxisUnconstrained = crossAxisUnconstrained;
        this.menuChildren = menuChildren;
        this.menuScopeNode = menuScopeNode;
        this.fadeAnimation = fadeAnimation;
        this.heightAnimation = heightAnimation;
        this.reservedPadding = reservedPadding;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.TextDirection textDirectionLocal = Directionality.of(context);
        (MenuStyle? themeStyle, MenuStyle defaultStyle) = this.anchor._parent?._orientation switch
        {
            Axis.horizontal or null => (MenuBarTheme.of(context).style, (MenuStyle)new _MenuBarDefaultsM3__menu_anchor(context)),
            Axis.vertical => (MenuTheme.of(context).style, (MenuStyle)new _MenuDefaultsM3__menu_anchor(context)),
            _ => throw new InvalidOperationException("Unexpected menu orientation.")
        };
        T? effectiveValue<T>(global::System.Func<MenuStyle?, T?> getProperty)
        {
            return ((getProperty(this.menuStyle) ?? getProperty(themeStyle)) ?? getProperty(defaultStyle));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        T? resolve<T>(global::System.Func<MenuStyle?, global::Doroti.Framework.Widgets.WidgetStateProperty<T>?> getProperty)
        {
            return effectiveValue(((style) =>
            {
                return ((T?)DartRuntimePrimitives.NullAware(getProperty(style), __target => __target.resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState>())));
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        global::Doroti.Framework.Widgets.WidgetStateMouseCursor mouseCursorLocal = ((global::Doroti.Framework.Widgets.WidgetStateMouseCursor)new _MouseCursor__menu_anchor(((global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Services.MouseCursor?>)((states) => effectiveValue(((style) => style?.mouseCursor?.resolve(states)))))));
        VisualDensity visualDensityLocal = (effectiveValue(((style) => style?.visualDensity)) ?? Theme.of(context).visualDensity);
        global::Doroti.Framework.Painting.AlignmentGeometry alignmentLocal = effectiveValue(((style) => style?.alignment))!;
        global::Doroti.Framework.Painting.EdgeInsetsGeometry paddingLocal = (resolve<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>(((style) => style?.padding)) ?? EdgeInsets.zero);
        global::Doroti.Ui.Offset densityAdjustment = ((global::Doroti.Ui.Offset)visualDensityLocal.baseSizeAdjustment);
        double dxLocal = Math.Max(0, densityAdjustment.dx);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry resolvedPadding = ((global::Doroti.Framework.Painting.EdgeInsetsGeometry)paddingLocal.add(new global::Doroti.Framework.Painting.EdgeInsets(dxLocal, 0, dxLocal, 0)).clamp(EdgeInsets.zero, EdgeInsetsGeometry.infinity));
        global::Doroti.Ui.Rect anchorRectLocal = ((global::Doroti.Ui.Rect)((this.layerLink is null) ? Rect.fromLTRB((((global::Doroti.Framework.Widgets.RawMenuOverlayInfo)this.menuPosition).anchorRect.left + dxLocal), ((global::Doroti.Framework.Widgets.RawMenuOverlayInfo)this.menuPosition).anchorRect.top, ((global::Doroti.Framework.Widgets.RawMenuOverlayInfo)this.menuPosition).anchorRect.right, ((global::Doroti.Framework.Widgets.RawMenuOverlayInfo)this.menuPosition).anchorRect.bottom) : Rect.zero));
        global::Doroti.Framework.Widgets.Widget menuPanel = ((global::Doroti.Framework.Widgets.Widget)new global::Doroti.Framework.Widgets.TapRegion(groupId: ((global::Doroti.Framework.Widgets.RawMenuOverlayInfo)this.menuPosition).tapRegionGroupId, consumeOutsideTaps: (((_MenuAnchorState__menu_anchor)this.anchor)._root._menuController.isOpen && this.anchor.widget.consumeOutsideTap), onTapOutside: ((global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>)((@event) =>
        {
            ((_MenuAnchorState__menu_anchor)this.anchor)._menuController.close();
        })), child: new global::Doroti.Framework.Widgets.MouseRegion(cursor: mouseCursorLocal, hitTestBehavior: HitTestBehavior.deferToChild, child: new global::Doroti.Framework.Widgets.FocusScope(node: ((_MenuAnchorState__menu_anchor)this.anchor)._menuScopeNode, skipTraversal: true, child: new global::Doroti.Framework.Widgets.Actions(actions: new DartMap<Type, dynamic> { [typeof(global::Doroti.Framework.Widgets.DismissIntent)] = new global::Doroti.Framework.Widgets.DismissMenuAction(controller: ((_MenuAnchorState__menu_anchor)this.anchor)._menuController) }, child: new global::Doroti.Framework.Widgets.Shortcuts(shortcuts: Menu_anchorLibrary._kMenuTraversalShortcuts, child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: this.fadeAnimation, alwaysIncludeSemantics: true, child: new _MenuPanel__menu_anchor(menuStyle: this.menuStyle, clipBehavior: this.clipBehavior, orientation: ((_MenuAnchorState__menu_anchor)this.anchor)._orientation, crossAxisUnconstrained: this.crossAxisUnconstrained, heightAnimation: this.heightAnimation, children: this.menuChildren))))))));
        global::Doroti.Framework.Widgets.Widget layout = ((global::Doroti.Framework.Widgets.Widget)new Theme(data: Theme.of(context).copyWith(visualDensity: visualDensityLocal), child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: BoxConstraints.CreateLoose(((global::Doroti.Framework.Widgets.RawMenuOverlayInfo)this.menuPosition).overlaySize), child: new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: this.heightAnimation, builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)((context, child) =>
        {
            global::Doroti.Framework.Widgets.MediaQueryData mediaQuery = ((global::Doroti.Framework.Widgets.MediaQueryData)MediaQuery.of(context));
            return ((global::Doroti.Framework.Widgets.Widget)new global::Doroti.Framework.Widgets.CustomSingleChildLayout(@delegate: new _MenuLayout__menu_anchor(anchorRect: anchorRectLocal, textDirection: textDirectionLocal, avoidBounds: DisplayFeatureSubScreen.avoidBounds(mediaQuery).toSet(), menuPadding: resolvedPadding, alignment: alignmentLocal, alignmentOffset: this.alignmentOffset, menuPosition: ((global::Doroti.Framework.Widgets.RawMenuOverlayInfo)this.menuPosition).position, orientation: ((_MenuAnchorState__menu_anchor)this.anchor)._orientation, parentOrientation: (((_MenuAnchorState__menu_anchor)this.anchor)._parent?._orientation ?? Axis.horizontal), reservedPadding: this.reservedPadding, heightFactor: ((global::Doroti.Framework.Animation.Animation<double>)this.heightAnimation).value, mediaQueryData: mediaQuery), child: menuPanel));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))))));
        if ((this.layerLink is null))
        {
            return layout;
        }
        return ((global::Doroti.Framework.Widgets.Widget)new global::Doroti.Framework.Widgets.CompositedTransformFollower(link: this.layerLink!, targetAnchor: Alignment.bottomLeft, child: layout));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MouseCursor__menu_anchor : global::Doroti.Framework.Widgets.WidgetStateMouseCursor
{
    public virtual global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Services.MouseCursor?> resolveCallback { get; private set; } = default!;

    internal _MouseCursor__menu_anchor(global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Services.MouseCursor?> resolveCallback)
    {
        this.resolveCallback = resolveCallback;
    }

    public override global::Doroti.Framework.Services.MouseCursor resolve(HashSet<global::Doroti.Framework.Widgets.WidgetState> states) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Services.MouseCursor>((this.resolveCallback(states) ?? uncontrolled));
    public override string debugDescription => "Menu_MouseCursor";
}

public static partial class Menu_anchorLibrary
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
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Menu_anchorLibrary
{
    internal static bool _isCupertino
    {
        get
        {
            switch (PlatformLibrary.defaultTargetPlatform)
            {
                case TargetPlatform.iOS:
                case TargetPlatform.macOS:
                    {
                        return true;
                    }
                case TargetPlatform.android:
                case TargetPlatform.fuchsia:
                case TargetPlatform.linux:
                case TargetPlatform.windows:
                    {
                        return false;
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
        }
    }
}

public static partial class Menu_anchorLibrary
{
    internal static bool _usesSymbolicModifiers
    {
        get
        {
            return _isCupertino;
        }
    }
}

public static partial class Menu_anchorLibrary
{
    internal static bool _platformSupportsAccelerators
    {
        get
        {
            return !_isCupertino;
        }
    }
}

internal class _MenuBarDefaultsM3__menu_anchor : MenuStyle
{
    internal static global::Doroti.Framework.Painting.RoundedRectangleBorder _defaultMenuBorder = new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: BorderRadius.CreateAll(Radius.circular(4.0)));
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = Theme.of(this.context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _MenuBarDefaultsM3__menu_anchor(global::Doroti.Framework.Widgets.BuildContext context) : base(elevation: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>(3.0), shape: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.OutlinedBorder>(_defaultMenuBorder), alignment: AlignmentDirectional.bottomStart)
    {
        this.context = context;
    }

    public override global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? backgroundColor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>)new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Color>(this._colors.surfaceContainer));
        }
    }
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? shadowColor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>?)new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Color>(this._colors.shadow));
        }
    }
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? surfaceTintColor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>?)new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Color>(Colors.transparent));
        }
    }
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>? padding
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>?)new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(EdgeInsetsDirectional.CreateSymmetric(horizontal: Menu_anchorLibrary._kTopLevelMenuHorizontalMinPadding)));
        }
    }
    public override VisualDensity? visualDensity => Theme.of(this.context).visualDensity;
}

internal class _MenuButtonDefaultsM3__menu_anchor : ButtonStyle
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = Theme.of(this.context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }
    private bool __late__textTheme_initialized;
    private TextTheme __late__textTheme = default!;
    internal virtual TextTheme _textTheme
    {
        get
        {
            if (!__late__textTheme_initialized)
            {
                __late__textTheme = Theme.of(this.context).textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }

    internal _MenuButtonDefaultsM3__menu_anchor(global::Doroti.Framework.Widgets.BuildContext context) : base(animationDuration: ConstantsLibrary.kThemeChangeDuration, enableFeedback: true, alignment: AlignmentDirectional.centerStart)
    {
        this.context = context;
    }

    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? backgroundColor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>?)ButtonStyleButton.allOrNull<global::Doroti.Ui.Color>(Colors.transparent));
        }
    }
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? elevation
    {
        get
        {
            return ButtonStyleButton.allOrNull<double?>(0.0);
        }
    }
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? foregroundColor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>?)WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(WidgetState.disabled))
                {
                    return (this._colors.onSurface.withOpacity(0.38));
                }
                if (states.Contains(WidgetState.pressed))
                {
                    return (this._colors.onSurface);
                }
                if (states.Contains(WidgetState.hovered))
                {
                    return (this._colors.onSurface);
                }
                if (states.Contains(WidgetState.focused))
                {
                    return (this._colors.onSurface);
                }
                return (this._colors.onSurface);
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
        }
    }
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? iconColor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>?)WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(WidgetState.disabled))
                {
                    return (this._colors.onSurface.withOpacity(0.38));
                }
                if (states.Contains(WidgetState.pressed))
                {
                    return (this._colors.onSurfaceVariant);
                }
                if (states.Contains(WidgetState.hovered))
                {
                    return (this._colors.onSurfaceVariant);
                }
                if (states.Contains(WidgetState.focused))
                {
                    return (this._colors.onSurfaceVariant);
                }
                return (this._colors.onSurfaceVariant);
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
        }
    }
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? iconSize
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<double?>?)new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>(24.0));
        }
    }
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>? maximumSize
    {
        get
        {
            return ButtonStyleButton.allOrNull<global::Doroti.Ui.Size>(Size.infinite);
        }
    }
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>? minimumSize
    {
        get
        {
            return ButtonStyleButton.allOrNull<global::Doroti.Ui.Size>(new global::Doroti.Ui.Size(64.0, 48.0));
        }
    }
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>>(WidgetStateMouseCursor.adaptiveClickable);
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? overlayColor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>?)WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(WidgetState.pressed))
                {
                    return (this._colors.onSurface.withOpacity(0.1));
                }
                if (states.Contains(WidgetState.hovered))
                {
                    return (this._colors.onSurface.withOpacity(0.08));
                }
                if (states.Contains(WidgetState.focused))
                {
                    return (this._colors.onSurface.withOpacity(0.1));
                }
                return (Colors.transparent);
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
        }
    }
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>? padding
    {
        get
        {
            return ButtonStyleButton.allOrNull<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(_scaledPadding(this.context));
        }
    }
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder>? shape
    {
        get
        {
            return ButtonStyleButton.allOrNull<global::Doroti.Framework.Painting.OutlinedBorder>(new global::Doroti.Framework.Painting.RoundedRectangleBorder());
        }
    }
    public override InteractiveInkFeatureFactory? splashFactory => Theme.of(this.context).splashFactory;
    public override MaterialTapTargetSize? tapTargetSize => Theme.of(this.context).materialTapTargetSize;
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?> textStyle
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>)new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.TextStyle?>(this._textTheme.labelLarge));
        }
    }
    public override VisualDensity? visualDensity => Theme.of(this.context).visualDensity;
    internal virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry _scaledPadding(global::Doroti.Framework.Widgets.BuildContext context)
    {
        VisualDensity visualDensityLocal = Theme.of(context).visualDensity;
        if ((visualDensityLocal.horizontal > 0L))
        {
            visualDensityLocal = new VisualDensity(vertical: visualDensityLocal.vertical);
        }
        double fontSizeLocal = (Theme.of(context).textTheme.labelLarge?.fontSize ?? 14.0);
        double fontSizeRatio = (MediaQuery.textScalerOf(context).scale(fontSizeLocal) / 14.0);
        return ButtonStyleButton.scaledPadding(EdgeInsets.CreateSymmetric(horizontal: Math.Max(Menu_anchorLibrary._kMenuViewPadding, (Menu_anchorLibrary._kLabelItemDefaultSpacing + visualDensityLocal.baseSizeAdjustment.dx))), EdgeInsets.CreateSymmetric(horizontal: Math.Max(Menu_anchorLibrary._kMenuViewPadding, (8L + visualDensityLocal.baseSizeAdjustment.dx))), EdgeInsets.CreateSymmetric(horizontal: Menu_anchorLibrary._kMenuViewPadding), fontSizeRatio);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MenuDefaultsM3__menu_anchor : MenuStyle
{
    internal static global::Doroti.Framework.Painting.RoundedRectangleBorder _defaultMenuBorder = new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: BorderRadius.CreateAll(Radius.circular(4.0)));
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = Theme.of(this.context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _MenuDefaultsM3__menu_anchor(global::Doroti.Framework.Widgets.BuildContext context) : base(elevation: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>(3.0), shape: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.OutlinedBorder>(_defaultMenuBorder), alignment: AlignmentDirectional.topEnd)
    {
        this.context = context;
    }

    public override global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? backgroundColor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>)new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Color>(this._colors.surfaceContainer));
        }
    }
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? surfaceTintColor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>?)new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Color>(Colors.transparent));
        }
    }
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? shadowColor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>?)new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Color>(this._colors.shadow));
        }
    }
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>? padding
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>?)new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(EdgeInsetsDirectional.CreateSymmetric(vertical: Menu_anchorLibrary._kMenuVerticalMinPadding)));
        }
    }
    public override VisualDensity? visualDensity => Theme.of(this.context).visualDensity;
}
