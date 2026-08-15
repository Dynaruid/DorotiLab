// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/menu_anchor.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

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
    internal static DartMap<global::Doroti.Generated.Framework.Widgets.ShortcutActivator, global::Doroti.Generated.Framework.Widgets.Intent> _kMenuTraversalShortcuts = new DartMap<global::Doroti.Generated.Framework.Widgets.ShortcutActivator, global::Doroti.Generated.Framework.Widgets.Intent> { [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.gameButtonA)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new global::Doroti.Generated.Framework.Widgets.ActivateIntent()), [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.escape)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new global::Doroti.Generated.Framework.Widgets.DismissIntent()), [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.tab)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new global::Doroti.Generated.Framework.Widgets.NextFocusIntent()), [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.tab, shift: true)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new global::Doroti.Generated.Framework.Widgets.PreviousFocusIntent()), [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowDown)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent(global::Doroti.Generated.Framework.Widgets.TraversalDirection.down)), [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowUp)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent(global::Doroti.Generated.Framework.Widgets.TraversalDirection.up)), [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowLeft)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent(global::Doroti.Generated.Framework.Widgets.TraversalDirection.left)), [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowRight)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent(global::Doroti.Generated.Framework.Widgets.TraversalDirection.right)) };
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
    internal static global::Doroti.Generated.Framework.Animation.Curve _kMenuPanelHeightForwardCurve = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)new global::Doroti.Generated.Framework.Animation.Cubic(0.3, 0, 0, 1));
}

public static partial class Menu_anchorLibrary
{
    internal static global::Doroti.Generated.Framework.Animation.Curve _kMenuPanelHeightReverseCurve = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)new _TweenCurve__menu_anchor(0.35, 1, curve: new global::Doroti.Generated.Framework.Animation.FlippedCurve(Easing.emphasizedAccelerate)));
}

public static partial class Menu_anchorLibrary
{
    internal static global::Doroti.Generated.Framework.Animation.Curve _kMenuPanelOpacityForwardCurve = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)new global::Doroti.Generated.Framework.Animation.Interval(0, (50L / 500L)));
}

public static partial class Menu_anchorLibrary
{
    internal static global::Doroti.Generated.Framework.Animation.Curve _kMenuPanelOpacityReverseCurve = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)new global::Doroti.Generated.Framework.Animation.FlippedCurve(new global::Doroti.Generated.Framework.Animation.Interval((100L / 150L), (150L / 150L))));
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

public delegate global::Doroti.Generated.Framework.Widgets.Widget MenuAnchorChildBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.MenuController controller, global::Doroti.Generated.Framework.Widgets.Widget? child);

internal class _MenuAnchorScope__menu_anchor : global::Doroti.Generated.Framework.Widgets.InheritedWidget
{
    public virtual _MenuAnchorState__menu_anchor state { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.AnimationStatus animationStatus { get; private set; } = default!;

    internal _MenuAnchorScope__menu_anchor(_MenuAnchorState__menu_anchor state, global::Doroti.Generated.Framework.Animation.AnimationStatus animationStatus, global::Doroti.Generated.Framework.Widgets.Widget child) : base(child: child)
    {
        this.state = state;
        this.animationStatus = animationStatus;
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget)
    {
        var __oldWidget = (_MenuAnchorScope__menu_anchor)(object)oldWidget;
        DartRuntimePrimitives.Assert(() => (object.Equals(((_MenuAnchorScope__menu_anchor)__oldWidget).state, this.state)), () => (object?)"The state of a MenuAnchor should not change.");
        return (!object.Equals(((_MenuAnchorScope__menu_anchor)__oldWidget).animationStatus, this.animationStatus));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TweenCurve__menu_anchor : global::Doroti.Generated.Framework.Animation.Curve
{
    public virtual double begin { get; private set; } = default!;
    public virtual double end { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Curve curve { get; private set; } = default!;

    internal _TweenCurve__menu_anchor(double begin, double end, global::Doroti.Generated.Framework.Animation.Curve curve)
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

public class MenuAnchor : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.MenuController? controller { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.FocusNode? childFocusNode { get; private set; }
    public virtual MenuStyle? style { get; private set; }
    public virtual Offset? alignmentOffset { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.LayerLink? layerLink { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual bool anchorTapClosesMenu { get; private set; } = default!;
    public virtual bool consumeOutsideTap { get; private set; } = default!;
    public virtual global::System.Action? onOpen { get; private set; }
    public virtual global::System.Action? onClose { get; private set; }
    public virtual bool crossAxisUnconstrained { get; private set; } = default!;
    public virtual bool useRootOverlay { get; private set; } = default!;
    public virtual bool animated { get; private set; } = default!;
    public virtual AnimationStatusListener? onAnimationStatusChanged { get; private set; }
    public virtual List<global::Doroti.Generated.Framework.Widgets.Widget> menuChildren { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.MenuController, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>? builder { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? child { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? reservedPadding { get; private set; }

    public MenuAnchor(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.MenuController? controller = null, global::Doroti.Generated.Framework.Widgets.FocusNode? childFocusNode = null, MenuStyle? style = null, Offset? alignmentOffset = default, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? reservedPadding = null, global::Doroti.Generated.Framework.Rendering.LayerLink? layerLink = null, Clip clipBehavior = Clip.hardEdge, bool anchorTapClosesMenu = false, bool consumeOutsideTap = false, global::System.Action? onOpen = null, global::System.Action? onClose = null, bool crossAxisUnconstrained = true, bool useRootOverlay = false, bool animated = false, AnimationStatusListener? onAnimationStatusChanged = null, List<global::Doroti.Generated.Framework.Widgets.Widget> menuChildren = default!, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.MenuController, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>? builder = null, global::Doroti.Generated.Framework.Widgets.Widget? child = null) : base(key: key)
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
    public virtual List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        return this.menuChildren.map<global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>(((child) => ((Diagnosticable)child).toDiagnosticsNode())).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("anchorTapClosesMenu", value: this.anchorTapClosesMenu, ifTrue: "AUTO-CLOSE"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.FocusNode?>("focusNode", this.childFocusNode));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<MenuStyle?>("style", this.style));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Ui.Clip>("clipBehavior", this.clipBehavior));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Offset?>("alignmentOffset", this.alignmentOffset));
    }

}

internal class _MenuAnchorState__menu_anchor : global::Doroti.Generated.Framework.Widgets.State<MenuAnchor>, global::Doroti.Generated.Framework.Widgets.SingleTickerProviderStateMixin<MenuAnchor>
{
    internal virtual global::Doroti.Generated.Framework.Widgets.MenuController? _internalMenuController { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Widgets.FocusScopeNode _menuScopeNode { get; private set; } = new global::Doroti.Generated.Framework.Widgets.FocusScopeNode();
    private bool __late__animationController_initialized;
    private global::Doroti.Generated.Framework.Animation.AnimationController __late__animationController = default!;
    internal virtual global::Doroti.Generated.Framework.Animation.AnimationController _animationController
    {
        get
        {
            if (!__late__animationController_initialized)
            {
                __late__animationController = new global::Doroti.Generated.Framework.Animation.AnimationController(vsync: this);
                __late__animationController_initialized = true;
            }
            return __late__animationController;
        }
    }
    private bool __late_heightAnimation_initialized;
    private global::Doroti.Generated.Framework.Animation.CurvedAnimation __late_heightAnimation = default!;
    public virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation heightAnimation
    {
        get
        {
            if (!__late_heightAnimation_initialized)
            {
                __late_heightAnimation = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: this._animationController, curve: Menu_anchorLibrary._kMenuPanelHeightForwardCurve, reverseCurve: Menu_anchorLibrary._kMenuPanelHeightReverseCurve);
                __late_heightAnimation_initialized = true;
            }
            return __late_heightAnimation;
        }
    }
    private bool __late_opacityAnimation_initialized;
    private global::Doroti.Generated.Framework.Animation.CurvedAnimation __late_opacityAnimation = default!;
    public virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation opacityAnimation
    {
        get
        {
            if (!__late_opacityAnimation_initialized)
            {
                __late_opacityAnimation = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: this._animationController, curve: Menu_anchorLibrary._kMenuPanelOpacityForwardCurve, reverseCurve: Menu_anchorLibrary._kMenuPanelOpacityReverseCurve);
                __late_opacityAnimation_initialized = true;
            }
            return __late_opacityAnimation;
        }
    }
    internal virtual List<global::Doroti.Generated.Framework.Widgets.Widget> _menuChildren { get; set; } = new List<global::Doroti.Generated.Framework.Widgets.Widget>();
    internal virtual List<global::Doroti.Generated.Framework.Animation.CurvedAnimation> _cachedAnimations { get; set; } = new List<global::Doroti.Generated.Framework.Animation.CurvedAnimation>();
    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual global::Doroti.Generated.Framework.Painting.Axis _orientation => global::Doroti.Generated.Framework.Painting.Axis.vertical;
    internal virtual global::Doroti.Generated.Framework.Widgets.MenuController _menuController => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.MenuController>((((MenuAnchor)(object)this.widget).controller ?? this._internalMenuController!));
    internal virtual _MenuAnchorState__menu_anchor? _parent => _MenuAnchorState__menu_anchor._maybeOf(this.context);
    public virtual bool isSubmenu => DartRuntimePrimitives.ConvertValue<bool>((MenuController.maybeOf(this.context) is not null));
    public virtual bool isClosingOrClosed => (((global::Doroti.Generated.Framework.Animation.AnimationController)this._animationController).status switch { global::Doroti.Generated.Framework.Animation.AnimationStatus.dismissed => true, global::Doroti.Generated.Framework.Animation.AnimationStatus.reverse => true, global::Doroti.Generated.Framework.Animation.AnimationStatus.forward => false, global::Doroti.Generated.Framework.Animation.AnimationStatus.completed => false, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    public virtual bool isClosing => (((global::Doroti.Generated.Framework.Animation.AnimationController)this._animationController).status switch { global::Doroti.Generated.Framework.Animation.AnimationStatus.reverse => true, global::Doroti.Generated.Framework.Animation.AnimationStatus.dismissed or global::Doroti.Generated.Framework.Animation.AnimationStatus.forward => false, global::Doroti.Generated.Framework.Animation.AnimationStatus.completed => false, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    public override void initState()
    {
        base.initState();
        _resolveAnimationController();
        _resolveMenuItems();
        this._animationController.addStatusListener((AnimationStatusListener)this._handleAnimationStatusChanged);
        if ((((MenuAnchor)(object)this.widget).controller is null))
        {
            _internalMenuController = new global::Doroti.Generated.Framework.Widgets.MenuController();
        }
    }

    public override void didUpdateWidget(MenuAnchor oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((MenuAnchor)oldWidget).controller, ((MenuAnchor)(object)this.widget).controller)))
        {
            if ((((MenuAnchor)(object)this.widget).controller is null))
            {
                _internalMenuController = new global::Doroti.Generated.Framework.Widgets.MenuController();
            }
            else
            {
                _internalMenuController = null;
            }
        }
        if (((((MenuAnchor)oldWidget).animated != ((MenuAnchor)(object)this.widget).animated) || (!object.Equals(((MenuAnchor)(object)this.widget).menuChildren, ((MenuAnchor)oldWidget).menuChildren))))
        {
            _resolveMenuItems();
        }
        if ((((MenuAnchor)oldWidget).animated != ((MenuAnchor)(object)this.widget).animated))
        {
            _resolveAnimationController();
        }
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() => Menu_anchorLibrary._debugMenuInfo($"Disposing of {this}"));
        this._menuChildren.Clear();
        foreach (global::Doroti.Generated.Framework.Animation.CurvedAnimation animation__21418 in this._cachedAnimations)
        {
            animation__21418.dispose();
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
                throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), this._ticker!.describeForError("The offending ticker was") }));
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual void _resolveAnimationController()
    {
        if (((MenuAnchor)(object)this.widget).animated)
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
        _menuChildren = new List<global::Doroti.Generated.Framework.Widgets.Widget>();
        foreach (global::Doroti.Generated.Framework.Animation.CurvedAnimation animation__22133 in this._cachedAnimations)
        {
            animation__22133.dispose();
        }
        _cachedAnimations = new List<global::Doroti.Generated.Framework.Animation.CurvedAnimation>();
        long itemCount__22260 = checked((long)(((MenuAnchor)(object)this.widget).menuChildren.Count));
        if ((itemCount__22260 == 0L))
        {
            return;
        }
        if (!((MenuAnchor)(object)this.widget).animated)
        {
            this._menuChildren.AddRange(((MenuAnchor)(object)this.widget).menuChildren.Cast<global::Doroti.Generated.Framework.Widgets.Widget>());
            return;
        }
        double forwardFinalItemOffset__22462 = (1L - Menu_anchorLibrary._kMenuItemRelativeFadeInDuration);
        double reverseFinalItemOffset__22542 = ((1L - Menu_anchorLibrary._kMenuItemRelativeFadeOutDuration) - Menu_anchorLibrary._kMenuItemRelativeFadeOutDelay);
        double forwardProgress__22659 = 0;
        double reverseProgress__22691 = 0;
        double itemFadeInGap__22723 = 0;
        double itemFadeOutGap__22753 = 0;
        if ((itemCount__22260 > 1L))
        {
            itemFadeInGap__22723 = (forwardFinalItemOffset__22462 / ((itemCount__22260 - 1L)));
            itemFadeOutGap__22753 = (reverseFinalItemOffset__22542 / ((itemCount__22260 - 1L)));
        }
        foreach (global::Doroti.Generated.Framework.Widgets.Widget child__23073 in ((MenuAnchor)(object)this.widget).menuChildren)
        {
            var forwardCurve__23117 = new global::Doroti.Generated.Framework.Animation.Interval(forwardProgress__22659, (forwardProgress__22659 + Menu_anchorLibrary._kMenuItemRelativeFadeInDuration));
            var reverseCurve__23249 = new global::Doroti.Generated.Framework.Animation.Interval(reverseProgress__22691, (reverseProgress__22691 + Menu_anchorLibrary._kMenuItemRelativeFadeOutDuration));
            var animation__23382 = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: this._animationController, curve: forwardCurve__23117, reverseCurve: reverseCurve__23249);
            this._cachedAnimations.Add(animation__23382);
            this._menuChildren.Add(new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: animation__23382, alwaysIncludeSemantics: true, child: child__23073));
            forwardProgress__22659 += itemFadeInGap__22723;
            reverseProgress__22691 += itemFadeOutGap__22753;
        }
    }

    internal virtual void _handleAnimationStatusChanged(global::Doroti.Generated.Framework.Animation.AnimationStatus status)
    {
        setState(((global::System.Action)(() => {
})));
        ((MenuAnchor)(object)this.widget).onAnimationStatusChanged?.Invoke(status);
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

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Generated.Framework.Widgets.Widget child__24734 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _MenuAnchorScope__menu_anchor(state: this, animationStatus: ((global::Doroti.Generated.Framework.Animation.AnimationController)this._animationController).status, child: new global::Doroti.Generated.Framework.Widgets.RawMenuAnchor(onOpenRequested: (global::System.Action<Offset?, global::System.Action>)this._handleMenuOpenRequest, onCloseRequested: (global::System.Action<global::System.Action>)this._handleMenuCloseRequest, useRootOverlay: ((MenuAnchor)(object)this.widget).useRootOverlay, onOpen: () => ((MenuAnchor)(object)this.widget).onOpen(), onClose: () => ((MenuAnchor)(object)this.widget).onClose(), consumeOutsideTaps: ((MenuAnchor)(object)this.widget).consumeOutsideTap, controller: this._menuController, childFocusNode: ((MenuAnchor)(object)this.widget).childFocusNode, overlayBuilder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.RawMenuOverlayInfo, global::Doroti.Generated.Framework.Widgets.Widget>)this._buildOverlay, builder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.MenuController, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>?)((MenuAnchor)(object)this.widget).builder, child: ((MenuAnchor)(object)this.widget).child)));
        if ((((MenuAnchor)(object)this.widget).layerLink is null))
        {
            return child__24734;
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.CompositedTransformTarget(link: ((MenuAnchor)(object)this.widget).layerLink!, child: child__24734));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildOverlay(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.RawMenuOverlayInfo position)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.ExcludeSemantics(excluding: this.isClosingOrClosed, child: new global::Doroti.Generated.Framework.Widgets.IgnorePointer(ignoring: this.isClosingOrClosed, child: new global::Doroti.Generated.Framework.Widgets.ExcludeFocus(excluding: this.isClosingOrClosed, child: new _Submenu__menu_anchor(fadeAnimation: this.opacityAnimation, heightAnimation: this.heightAnimation, layerLink: ((MenuAnchor)(object)this.widget).layerLink, consumeOutsideTaps: ((MenuAnchor)(object)this.widget).consumeOutsideTap, menuScopeNode: this._menuScopeNode, menuStyle: ((MenuAnchor)(object)this.widget).style, clipBehavior: ((MenuAnchor)(object)this.widget).clipBehavior, menuChildren: this._menuChildren, crossAxisUnconstrained: ((MenuAnchor)(object)this.widget).crossAxisUnconstrained, menuPosition: position, anchor: this, alignmentOffset: (((MenuAnchor)(object)this.widget).alignmentOffset ?? Offset.zero), reservedPadding: (((MenuAnchor)(object)this.widget).reservedPadding ?? global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateAll(Menu_anchorLibrary._kMenuViewPadding)))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual _MenuAnchorState__menu_anchor _root
    {
        get
        {
            var anchor__26891 = this;
            while ((((_MenuAnchorState__menu_anchor)anchor__26891)._parent is not null))
            {
                anchor__26891 = ((_MenuAnchorState__menu_anchor)anchor__26891)._parent!;
            }
            return anchor__26891;
            return default!;
        }
    }
    internal virtual void _focusButton()
    {
        if ((((MenuAnchor)(object)this.widget).childFocusNode is null))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => Menu_anchorLibrary._debugMenuInfo($"Requesting focus for {((MenuAnchor)(object)this.widget).childFocusNode}"));
        ((MenuAnchor)(object)this.widget).childFocusNode!.requestFocus();
    }

    internal virtual void _focusFirstMenuItem()
    {
        if ((this._menuScopeNode.context?.mounted != true))
        {
            return;
        }
        global::Doroti.Generated.Framework.Widgets.FocusTraversalPolicy policy__27348 = (FocusTraversalGroup.maybeOf(this._menuScopeNode.context!) ?? new global::Doroti.Generated.Framework.Widgets.ReadingOrderTraversalPolicy());
        global::Doroti.Generated.Framework.Widgets.FocusNode? firstFocus__27473 = ((global::Doroti.Generated.Framework.Widgets.FocusNode?)(object?)policy__27348.findFirstFocus(this._menuScopeNode, ignoreCurrentFocus: true));
        if ((firstFocus__27473 is not null))
        {
            firstFocus__27473.requestFocus();
        }
    }

    internal virtual void _focusLastMenuItem()
    {
        if ((this._menuScopeNode.context?.mounted != true))
        {
            return;
        }
        global::Doroti.Generated.Framework.Widgets.FocusTraversalPolicy policy__27757 = (FocusTraversalGroup.maybeOf(this._menuScopeNode.context!) ?? new global::Doroti.Generated.Framework.Widgets.ReadingOrderTraversalPolicy());
        global::Doroti.Generated.Framework.Widgets.FocusNode lastFocus__27881 = ((global::Doroti.Generated.Framework.Widgets.FocusNode)(object?)policy__27757.findLastFocus(this._menuScopeNode, ignoreCurrentFocus: true));
        lastFocus__27881.requestFocus();
    }

    internal static _MenuAnchorState__menu_anchor? _maybeOf(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return context.getInheritedWidgetOfExactType<_MenuAnchorScope__menu_anchor>()?.state;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Generated.Framework.Animation.AnimationStatus? _maybeAnimationStatusOf(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return context.dependOnInheritedWidgetOfExactType<_MenuAnchorScope__menu_anchor>()?.animationStatus;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string ToString(global::Doroti.Generated.Framework.Foundation.DiagnosticLevel minLevel = global::Doroti.Generated.Framework.Foundation.DiagnosticLevel.debug)
    {
        return global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._ticker is null))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this.GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
            });
        this._ticker = new global::Doroti.Generated.Framework.Scheduler.Ticker((global::System.Action<Duration>)onTick, debugLabel: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
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
        TickerModeData values__15157 = this._tickerModeNotifier!.value;
        if ((this._ticker is not null))
        {
            this._ticker!.muted = !((TickerModeData)values__15157).enabled;
            this._ticker!.forceFrames = ((TickerModeData)values__15157).forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__15400 = ((global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__15400, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        newNotifier__15400.addListener(() => this._updateTicker());
        this._tickerModeNotifier = newNotifier__15400;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription__15805 = ((this._ticker?.isActive, this._ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) });
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Scheduler.Ticker>("ticker", this._ticker, description: tickerDescription__15805, showSeparator: false, defaultValue: default));
    }

}

public class MenuBar : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual MenuStyle? style { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.MenuController? controller { get; private set; }
    public virtual List<global::Doroti.Generated.Framework.Widgets.Widget> children { get; private set; } = default!;

    public MenuBar(global::Doroti.Generated.Framework.Foundation.Key? key = null, MenuStyle? style = null, Clip clipBehavior = Clip.none, global::Doroti.Generated.Framework.Widgets.MenuController? controller = null, List<global::Doroti.Generated.Framework.Widgets.Widget> children = default!) : base(key: key)
    {
        this.style = style;
        this.clipBehavior = clipBehavior;
        this.controller = controller;
        this.children = children;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasOverlay(context));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _MenuBarAnchor__menu_anchor(controller: this.controller, clipBehavior: this.clipBehavior, style: this.style, menuChildren: this.children));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        return ((Func<List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>>)(() => { var __collection33328 = new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>(); __collection33328.AddRange(this.children.map<global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>(((item) => ((Diagnosticable)item).toDiagnosticsNode()))); return __collection33328; }))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<MenuStyle?>("style", this.style, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Clip>("clipBehavior", this.clipBehavior, defaultValue: null));
    }

}

public class MenuItemButton : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual global::System.Action<bool>? onHover { get; private set; }
    public virtual bool requestFocusOnHover { get; private set; } = default!;
    public virtual global::System.Action<bool>? onFocusChange { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.MenuSerializableShortcut? shortcut { get; private set; }
    public virtual string? semanticsLabel { get; private set; }
    public virtual ButtonStyle? style { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? leadingIcon { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? trailingIcon { get; private set; }
    public virtual bool closeOnActivate { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.Axis overflowAxis { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? child { get; private set; }

    public MenuItemButton(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Action? onPressed = null, global::System.Action<bool>? onHover = null, bool requestFocusOnHover = true, global::System.Action<bool>? onFocusChange = null, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, global::Doroti.Generated.Framework.Widgets.MenuSerializableShortcut? shortcut = null, string? semanticsLabel = null, ButtonStyle? style = null, global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController = null, Clip clipBehavior = Clip.none, global::Doroti.Generated.Framework.Widgets.Widget? leadingIcon = null, global::Doroti.Generated.Framework.Widgets.Widget? trailingIcon = null, bool closeOnActivate = true, global::Doroti.Generated.Framework.Painting.Axis overflowAxis = global::Doroti.Generated.Framework.Painting.Axis.horizontal, global::Doroti.Generated.Framework.Widgets.Widget? child = null) : base(key: key)
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
    public virtual ButtonStyle defaultStyleOf(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((ButtonStyle)(object?)new _MenuButtonDefaultsM3__menu_anchor(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ButtonStyle? themeStyleOf(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return MenuButtonTheme.of(context).style;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ButtonStyle styleFrom(Color? foregroundColor = null, Color? backgroundColor = null, Color? disabledForegroundColor = null, Color? disabledBackgroundColor = null, Color? shadowColor = null, Color? surfaceTintColor = null, Color? iconColor = null, double? iconSize = null, Color? disabledIconColor = null, global::Doroti.Generated.Framework.Painting.TextStyle? textStyle = null, Color? overlayColor = null, double? elevation = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, Size? minimumSize = null, Size? fixedSize = null, Size? maximumSize = null, global::Doroti.Generated.Framework.Services.MouseCursor? enabledMouseCursor = null, global::Doroti.Generated.Framework.Services.MouseCursor? disabledMouseCursor = null, global::Doroti.Generated.Framework.Painting.BorderSide? side = null, global::Doroti.Generated.Framework.Painting.OutlinedBorder? shape = null, VisualDensity? visualDensity = null, MaterialTapTargetSize? tapTargetSize = null, Duration? animationDuration = null, bool? enableFeedback = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment = null, InteractiveInkFeatureFactory? splashFactory = null)
    {
        return ((ButtonStyle)(object?)TextButton.styleFrom(foregroundColor: foregroundColor, backgroundColor: backgroundColor, disabledBackgroundColor: disabledBackgroundColor, disabledForegroundColor: disabledForegroundColor, shadowColor: shadowColor, surfaceTintColor: surfaceTintColor, iconColor: iconColor, iconSize: iconSize, disabledIconColor: disabledIconColor, textStyle: textStyle, overlayColor: overlayColor, elevation: elevation, padding: padding, minimumSize: minimumSize, fixedSize: fixedSize, maximumSize: maximumSize, enabledMouseCursor: enabledMouseCursor, disabledMouseCursor: disabledMouseCursor, side: side, shape: shape, visualDensity: visualDensity, tapTargetSize: tapTargetSize, animationDuration: animationDuration, enableFeedback: enableFeedback, alignment: alignment, splashFactory: splashFactory));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("enabled", value: (this.onPressed is not null), ifFalse: "DISABLED"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<ButtonStyle?>("style", this.style, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.MenuSerializableShortcut?>("shortcut", this.shortcut, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.FocusNode?>("focusNode", this.focusNode, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Ui.Clip>("clipBehavior", this.clipBehavior, defaultValue: Clip.none));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStatesController?>("statesController", this.statesController, defaultValue: null));
    }

}

internal class _MenuItemButtonState__menu_anchor : global::Doroti.Generated.Framework.Widgets.State<MenuItemButton>
{
    internal virtual global::Doroti.Generated.Framework.Widgets.FocusNode? _internalFocusNode { get; set; } = default;
    internal virtual bool _isHovered { get; set; } = false;

    internal virtual global::Doroti.Generated.Framework.Widgets.FocusNode _focusNode => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.FocusNode>((((MenuItemButton)(object)this.widget).focusNode ?? this._internalFocusNode!));
    internal virtual _MenuAnchorState__menu_anchor? _anchor => _MenuAnchorState__menu_anchor._maybeOf(this.context);
    public override void initState()
    {
        base.initState();
        _createInternalFocusNodeIfNeeded();
        this._focusNode.addListener(() => this._handleFocusChange());
    }

    public override void dispose()
    {
        this._focusNode.removeListener(() => this._handleFocusChange());
        this._internalFocusNode?.dispose();
        _internalFocusNode = null;
        base.dispose();
    }

    public override void didUpdateWidget(MenuItemButton oldWidget)
    {
        if ((!object.Equals(((MenuItemButton)(object)this.widget).focusNode, ((MenuItemButton)oldWidget).focusNode)))
        {
            ((((MenuItemButton)oldWidget).focusNode ?? this._internalFocusNode))?.removeListener(() => this._handleFocusChange());
            if ((((MenuItemButton)(object)this.widget).focusNode is not null))
            {
                this._internalFocusNode?.dispose();
                _internalFocusNode = null;
            }
            _createInternalFocusNodeIfNeeded();
            this._focusNode.addListener(() => this._handleFocusChange());
        }
        base.didUpdateWidget(oldWidget);
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ButtonStyle mergedStyle__45303 = ((this.widget.themeStyleOf(context)?.merge(this.widget.defaultStyleOf(context)) ?? (ButtonStyle)this.widget.defaultStyleOf(context)));
        if ((((MenuItemButton)(object)this.widget).style is not null))
        {
            mergedStyle__45303 = ((MenuItemButton)(object)this.widget).style!.merge(mergedStyle__45303);
        }
        global::Doroti.Generated.Framework.Widgets.Widget child__45540 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new TextButton(onPressed: ((global::System.Action)(((MenuItemButton)(object)this.widget).enabled ? this._handleSelect : null)), onFocusChange: ((global::System.Action<bool>)(((MenuItemButton)(object)this.widget).enabled ? ((MenuItemButton)(object)this.widget).onFocusChange : null)), focusNode: this._focusNode, style: mergedStyle__45303, autofocus: (((MenuItemButton)(object)this.widget).enabled && ((MenuItemButton)(object)this.widget).autofocus), statesController: ((MenuItemButton)(object)this.widget).statesController, clipBehavior: ((MenuItemButton)(object)this.widget).clipBehavior, isSemanticButton: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb ? true : null), child: new _MenuItemLabel__menu_anchor(leadingIcon: ((MenuItemButton)(object)this.widget).leadingIcon, shortcut: ((MenuItemButton)(object)this.widget).shortcut, semanticsLabel: ((MenuItemButton)(object)this.widget).semanticsLabel, trailingIcon: ((MenuItemButton)(object)this.widget).trailingIcon, hasSubmenu: false, overflowAxis: (this._anchor?._orientation ?? ((MenuItemButton)(object)this.widget).overflowAxis), child: ((MenuItemButton)(object)this.widget).child)));
        if ((Menu_anchorLibrary._platformSupportsAccelerators && ((MenuItemButton)(object)this.widget).enabled))
        {
            child__45540 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new MenuAcceleratorCallbackBinding(onInvoke: () => this._handleSelect(), child: child__45540));
        }
        if (((((MenuItemButton)(object)this.widget).onHover is not null) || ((MenuItemButton)(object)this.widget).requestFocusOnHover))
        {
            child__45540 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.MouseRegion(onHover: (global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerHoverEvent>)this._handlePointerHover, onExit: (global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerExitEvent>)this._handlePointerExit, child: child__45540));
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.MergeSemantics(child: child__45540));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleFocusChange()
    {
        if (!((global::Doroti.Generated.Framework.Widgets.FocusNode)this._focusNode).hasPrimaryFocus)
        {
            MenuController.maybeOf(this.context)?.closeChildren();
        }
    }

    internal virtual void _handlePointerExit(global::Doroti.Generated.Framework.Gestures.PointerExitEvent @event)
    {
        if (this._isHovered)
        {
            ((MenuItemButton)(object)this.widget).onHover?.Invoke(false);
            _isHovered = false;
        }
    }

    internal virtual void _handlePointerHover(global::Doroti.Generated.Framework.Gestures.PointerHoverEvent @event)
    {
        if (!this._isHovered)
        {
            _isHovered = true;
            ((MenuItemButton)(object)this.widget).onHover?.Invoke(true);
            if (((MenuItemButton)(object)this.widget).requestFocusOnHover)
            {
                DartRuntimePrimitives.Assert(() => Menu_anchorLibrary._debugMenuInfo($"Requesting focus for {this._focusNode} from hover"));
                this._focusNode.requestFocus();
                FocusTraversalGroup.of(this.context).invalidateScopeData(FocusScope.of(this.context));
            }
        }
    }

    internal virtual void _handleSelect()
    {
        DartRuntimePrimitives.Assert(() => Menu_anchorLibrary._debugMenuInfo($"Selected {((MenuItemButton)(object)this.widget).child} menu"));
        if (((MenuItemButton)(object)this.widget).closeOnActivate)
        {
            this._anchor?._root._menuController.close();
        }
        global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) => {
global::Doroti.Generated.Framework.Widgets.FocusManager.instance.applyFocusChangesIfNeeded();
((MenuItemButton)(object)this.widget).onPressed?.Invoke();
})), debugLabel: "MenuAnchor.onPressed");
    }

    internal virtual void _createInternalFocusNodeIfNeeded()
    {
        if ((((MenuItemButton)(object)this.widget).focusNode is null))
        {
            _internalFocusNode = new global::Doroti.Generated.Framework.Widgets.FocusNode();
            DartRuntimePrimitives.Assert(() =>
                {
                    this._internalFocusNode?.debugLabel = $"{typeof(MenuItemButton)}({((MenuItemButton)(object)this.widget).child})";
                    return true;
                });
        }
    }

}

public class CheckboxMenuButton : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual bool? value { get; private set; }
    public virtual bool tristate { get; private set; } = default!;
    public virtual bool isError { get; private set; } = default!;
    public virtual global::System.Action<bool?>? onChanged { get; private set; }
    public virtual global::System.Action<bool>? onHover { get; private set; }
    public virtual global::System.Action<bool>? onFocusChange { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.MenuSerializableShortcut? shortcut { get; private set; }
    public virtual ButtonStyle? style { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? trailingIcon { get; private set; }
    public virtual bool closeOnActivate { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? child { get; private set; }

    public CheckboxMenuButton(global::Doroti.Generated.Framework.Foundation.Key? key = null, bool? value = default!, bool tristate = false, bool isError = false, global::System.Action<bool?>? onChanged = default!, global::System.Action<bool>? onHover = null, global::System.Action<bool>? onFocusChange = null, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, global::Doroti.Generated.Framework.Widgets.MenuSerializableShortcut? shortcut = null, ButtonStyle? style = null, global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController = null, Clip clipBehavior = Clip.none, global::Doroti.Generated.Framework.Widgets.Widget? trailingIcon = null, bool closeOnActivate = true, global::Doroti.Generated.Framework.Widgets.Widget? child = default!) : base(key: key)
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
    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new MenuItemButton(key: this.key, onPressed: ((global::System.Action)((this.onChanged is null) ? null : (() => {
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
}))), onHover: (global::System.Action<bool>?)this.onHover, onFocusChange: (global::System.Action<bool>?)this.onFocusChange, focusNode: this.focusNode, style: this.style, shortcut: this.shortcut, statesController: this.statesController, leadingIcon: new global::Doroti.Generated.Framework.Widgets.ExcludeFocus(child: new global::Doroti.Generated.Framework.Widgets.IgnorePointer(child: new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Generated.Framework.Rendering.BoxConstraints(maxHeight: Checkbox.width, maxWidth: Checkbox.width), child: new Checkbox(tristate: this.tristate, value: this.value, onChanged: this.onChanged, isError: this.isError)))), clipBehavior: this.clipBehavior, trailingIcon: this.trailingIcon, closeOnActivate: this.closeOnActivate, child: this.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class RadioMenuButton<T> : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual T value { get; private set; } = default!;
    public virtual T? groupValue { get; private set; }
    public virtual bool toggleable { get; private set; } = default!;
    public virtual global::System.Action<T?>? onChanged { get; private set; }
    public virtual global::System.Action<bool>? onHover { get; private set; }
    public virtual global::System.Action<bool>? onFocusChange { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.MenuSerializableShortcut? shortcut { get; private set; }
    public virtual ButtonStyle? style { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? trailingIcon { get; private set; }
    public virtual bool closeOnActivate { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? child { get; private set; }

    public RadioMenuButton(global::Doroti.Generated.Framework.Foundation.Key? key = null, T value = default!, T? groupValue = default!, global::System.Action<T?>? onChanged = default!, bool toggleable = false, global::System.Action<bool>? onHover = null, global::System.Action<bool>? onFocusChange = null, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, global::Doroti.Generated.Framework.Widgets.MenuSerializableShortcut? shortcut = null, ButtonStyle? style = null, global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController = null, Clip clipBehavior = Clip.none, global::Doroti.Generated.Framework.Widgets.Widget? trailingIcon = null, bool closeOnActivate = true, global::Doroti.Generated.Framework.Widgets.Widget? child = default!) : base(key: key)
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
    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new MenuItemButton(key: this.key, onPressed: ((global::System.Action)((this.onChanged is null) ? null : (() => {
if ((this.toggleable && EqualityComparer<T>.Default.Equals(this.groupValue, this.value)))
{
    this.onChanged!(default);
    return;
}
this.onChanged!(this.value);
}))), onHover: (global::System.Action<bool>?)this.onHover, onFocusChange: (global::System.Action<bool>?)this.onFocusChange, focusNode: this.focusNode, style: this.style, shortcut: this.shortcut, statesController: this.statesController, leadingIcon: new global::Doroti.Generated.Framework.Widgets.ExcludeFocus(child: new global::Doroti.Generated.Framework.Widgets.IgnorePointer(child: new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Generated.Framework.Rendering.BoxConstraints(maxHeight: Checkbox.width, maxWidth: Checkbox.width), child: new Radio<T>(value: this.value, groupValue: this.groupValue, onChanged: this.onChanged, toggleable: this.toggleable)))), clipBehavior: this.clipBehavior, trailingIcon: this.trailingIcon, closeOnActivate: this.closeOnActivate, child: this.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SubmenuButton : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual global::System.Action<bool>? onHover { get; private set; }
    public virtual global::System.Action<bool>? onFocusChange { get; private set; }
    public virtual global::System.Action? onOpen { get; private set; }
    public virtual global::System.Action? onClose { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.MenuController? controller { get; private set; }
    public virtual ButtonStyle? style { get; private set; }
    public virtual MenuStyle? menuStyle { get; private set; }
    public virtual Offset? alignmentOffset { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? leadingIcon { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Widgets.Widget?>? submenuIcon { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? trailingIcon { get; private set; }
    public virtual bool useRootOverlay { get; private set; } = default!;
    public virtual AnimationStatusListener? onAnimationStatusChanged { get; private set; }
    public virtual List<global::Doroti.Generated.Framework.Widgets.Widget> menuChildren { get; private set; } = default!;
    public virtual Duration hoverOpenDelay { get; private set; } = default!;
    public virtual bool animated { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? child { get; private set; }

    public SubmenuButton(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Action<bool>? onHover = null, global::System.Action<bool>? onFocusChange = null, global::System.Action? onOpen = null, global::System.Action? onClose = null, global::Doroti.Generated.Framework.Widgets.MenuController? controller = null, ButtonStyle? style = null, MenuStyle? menuStyle = null, Offset? alignmentOffset = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController = null, global::Doroti.Generated.Framework.Widgets.Widget? leadingIcon = null, global::Doroti.Generated.Framework.Widgets.Widget? trailingIcon = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Widgets.Widget?>? submenuIcon = null, bool useRootOverlay = false, Duration hoverOpenDelay = default, bool animated = false, AnimationStatusListener? onAnimationStatusChanged = null, List<global::Doroti.Generated.Framework.Widgets.Widget> menuChildren = default!, global::Doroti.Generated.Framework.Widgets.Widget? child = default!) : base(key: key)
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
    public virtual ButtonStyle defaultStyleOf(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((ButtonStyle)(object?)new _MenuButtonDefaultsM3__menu_anchor(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ButtonStyle? themeStyleOf(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return MenuButtonTheme.of(context).style;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ButtonStyle styleFrom(Color? foregroundColor = null, Color? backgroundColor = null, Color? disabledForegroundColor = null, Color? disabledBackgroundColor = null, Color? shadowColor = null, Color? surfaceTintColor = null, Color? iconColor = null, double? iconSize = null, Color? disabledIconColor = null, global::Doroti.Generated.Framework.Painting.TextStyle? textStyle = null, Color? overlayColor = null, double? elevation = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, Size? minimumSize = null, Size? fixedSize = null, Size? maximumSize = null, global::Doroti.Generated.Framework.Services.MouseCursor? enabledMouseCursor = null, global::Doroti.Generated.Framework.Services.MouseCursor? disabledMouseCursor = null, global::Doroti.Generated.Framework.Painting.BorderSide? side = null, global::Doroti.Generated.Framework.Painting.OutlinedBorder? shape = null, VisualDensity? visualDensity = null, MaterialTapTargetSize? tapTargetSize = null, Duration? animationDuration = null, bool? enableFeedback = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment = null, InteractiveInkFeatureFactory? splashFactory = null)
    {
        return ((ButtonStyle)(object?)TextButton.styleFrom(foregroundColor: foregroundColor, backgroundColor: backgroundColor, disabledBackgroundColor: disabledBackgroundColor, disabledForegroundColor: disabledForegroundColor, shadowColor: shadowColor, surfaceTintColor: surfaceTintColor, iconColor: iconColor, disabledIconColor: disabledIconColor, iconSize: iconSize, textStyle: textStyle, overlayColor: overlayColor, elevation: elevation, padding: padding, minimumSize: minimumSize, fixedSize: fixedSize, maximumSize: maximumSize, enabledMouseCursor: enabledMouseCursor, disabledMouseCursor: disabledMouseCursor, side: side, shape: shape, visualDensity: visualDensity, tapTargetSize: tapTargetSize, animationDuration: animationDuration, enableFeedback: enableFeedback, alignment: alignment, splashFactory: splashFactory));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        return ((Func<List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>>)(() => { var __collection71845 = new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>(); __collection71845.AddRange(this.menuChildren.map<global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>(((child) => {
return ((global::Doroti.Generated.Framework.Foundation.DiagnosticsNode)(object?)((Diagnosticable)child).toDiagnosticsNode());
throw new InvalidOperationException("Dart closure completed without a value.");
}))); return __collection71845; }))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.FocusNode?>("focusNode", this.focusNode));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<MenuStyle>("menuStyle", this.menuStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Offset>("alignmentOffset", this.alignmentOffset));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Ui.Clip>("clipBehavior", this.clipBehavior));
    }

}

internal class _SubmenuButtonState__menu_anchor : global::Doroti.Generated.Framework.Widgets.State<SubmenuButton>
{
    private bool __late_actions_initialized;
    private DartMap<Type, dynamic> __late_actions = default!;
    public virtual DartMap<Type, dynamic> actions
    {
        get
        {
            if (!__late_actions_initialized)
            {
                __late_actions = new DartMap<Type, dynamic> { [typeof(global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent)] = new _SubmenuDirectionalFocusAction__menu_anchor(submenu: this) };
                __late_actions_initialized = true;
            }
            return __late_actions;
        }
    }
    internal virtual bool _waitingToFocusMenu { get; set; } = false;
    internal virtual bool _isOpenOnFocusEnabled { get; set; } = true;
    internal virtual global::Doroti.Generated.Framework.Widgets.MenuController? _internalMenuController { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Widgets.FocusNode? _internalFocusNode { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<_MenuAnchorState__menu_anchor> _anchorKey { get; private set; } = global::Doroti.Generated.Framework.Widgets.GlobalKey<_MenuAnchorState__menu_anchor>.Create();
    internal virtual bool _isHovered { get; set; } = false;
    internal virtual global::Doroti.Generated.Framework.Animation.AnimationStatus _animationStatus { get; set; } = global::Doroti.Generated.Framework.Animation.AnimationStatus.dismissed;
    internal virtual Timer? _hoverOpenTimer { get; set; } = default;

    internal virtual global::Doroti.Generated.Framework.Widgets.MenuController _menuController => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.MenuController>((((SubmenuButton)(object)this.widget).controller ?? this._internalMenuController!));
    internal virtual _MenuAnchorState__menu_anchor? _parent => _MenuAnchorState__menu_anchor._maybeOf(this.context);
    internal virtual _MenuAnchorState__menu_anchor? _anchorState => ((global::Doroti.Generated.Framework.Widgets.GlobalKey<_MenuAnchorState__menu_anchor>)this._anchorKey).currentState;
    internal virtual global::Doroti.Generated.Framework.Widgets.FocusNode _buttonFocusNode => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.FocusNode>((((SubmenuButton)(object)this.widget).focusNode ?? this._internalFocusNode!));
    internal virtual bool _enabled => System.Linq.Enumerable.Any(((SubmenuButton)(object)this.widget).menuChildren);
    public override void initState()
    {
        base.initState();
        if ((((SubmenuButton)(object)this.widget).focusNode is null))
        {
            _internalFocusNode = new global::Doroti.Generated.Framework.Widgets.FocusNode();
            DartRuntimePrimitives.Assert(() =>
                {
                    this._internalFocusNode?.debugLabel = $"{typeof(SubmenuButton)}({((SubmenuButton)(object)this.widget).child})";
                    return true;
                });
        }
        if ((((SubmenuButton)(object)this.widget).controller is null))
        {
            _internalMenuController = new global::Doroti.Generated.Framework.Widgets.MenuController();
        }
        this._buttonFocusNode.addListener(() => this._handleFocusChange());
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        DartRuntimePrimitives.Assert(() => _debugValidateHoverOpenDelay());
    }

    public override void dispose()
    {
        _clearHoverOpenTimer();
        this._buttonFocusNode.removeListener(() => this._handleFocusChange());
        this._internalFocusNode?.dispose();
        _internalFocusNode = null;
        base.dispose();
    }

    public override void didUpdateWidget(SubmenuButton oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        DartRuntimePrimitives.Assert(() => _debugValidateHoverOpenDelay());
        if ((!object.Equals(((SubmenuButton)(object)this.widget).focusNode, ((SubmenuButton)oldWidget).focusNode)))
        {
            if ((((SubmenuButton)oldWidget).focusNode is null))
            {
                this._internalFocusNode?.removeListener(() => this._handleFocusChange());
                this._internalFocusNode?.dispose();
                _internalFocusNode = null;
            }
            else
            {
                ((SubmenuButton)oldWidget).focusNode!.removeListener(() => this._handleFocusChange());
            }
            if ((((SubmenuButton)(object)this.widget).focusNode is null))
            {
                _internalFocusNode ??= new global::Doroti.Generated.Framework.Widgets.FocusNode();
                DartRuntimePrimitives.Assert(() =>
                    {
                        this._internalFocusNode?.debugLabel = $"{typeof(SubmenuButton)}({((SubmenuButton)(object)this.widget).child})";
                        return true;
                    });
            }
            this._buttonFocusNode.addListener(() => this._handleFocusChange());
        }
        if ((!object.Equals(((SubmenuButton)(object)this.widget).controller, ((SubmenuButton)oldWidget).controller)))
        {
            _internalMenuController = (((((SubmenuButton)oldWidget).controller is null)) ? null : new global::Doroti.Generated.Framework.Widgets.MenuController());
        }
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Offset menuPaddingOffset__75078 = ((global::Doroti.Ui.Offset)(object?)(((SubmenuButton)(object)this.widget).alignmentOffset ?? Offset.zero));
        global::Doroti.Generated.Framework.Painting.EdgeInsets menuPadding__75158 = ((global::Doroti.Generated.Framework.Painting.EdgeInsets)(object?)_computeMenuPadding(context));
        global::Doroti.Generated.Framework.Painting.Axis orientation__75217 = (this._parent?._orientation ?? global::Doroti.Generated.Framework.Painting.Axis.vertical);
        menuPaddingOffset__75078 += ((orientation__75217, Directionality.of(context)) switch { (global::Doroti.Generated.Framework.Painting.Axis.horizontal, TextDirection.rtl) => new global::Doroti.Ui.Offset(((global::Doroti.Generated.Framework.Painting.EdgeInsets)menuPadding__75158).right, 0), (global::Doroti.Generated.Framework.Painting.Axis.horizontal, TextDirection.ltr) => new global::Doroti.Ui.Offset(-((global::Doroti.Generated.Framework.Painting.EdgeInsets)menuPadding__75158).left, 0), (global::Doroti.Generated.Framework.Painting.Axis.vertical, TextDirection.rtl) => new global::Doroti.Ui.Offset(0, -((global::Doroti.Generated.Framework.Painting.EdgeInsets)menuPadding__75158).top), (global::Doroti.Generated.Framework.Painting.Axis.vertical, TextDirection.ltr) => new global::Doroti.Ui.Offset(0, -((global::Doroti.Generated.Framework.Painting.EdgeInsets)menuPadding__75158).top), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        var states__75808 = ((Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>>)(() => { var __collection75817 = new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>(); if (!this._enabled) { __collection75817.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled); } if (this._isHovered) { __collection75817.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered); } if (((global::Doroti.Generated.Framework.Widgets.FocusNode)this._buttonFocusNode).hasFocus) { __collection75817.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.focused); } return __collection75817; }))();
        global::Doroti.Generated.Framework.Widgets.Widget submenuIcon__76000 = (((((SubmenuButton)(object)this.widget).submenuIcon?.resolve(states__75808) ?? (global::Doroti.Generated.Framework.Widgets.Widget)MenuTheme.of(context).submenuIcon?.resolve(states__75808))) ?? new global::Doroti.Generated.Framework.Widgets.Icon(Icons.arrow_right, size: Menu_anchorLibrary._kDefaultSubmenuIconSize));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Actions(actions: this.actions, child: new MenuAnchor(key: this._anchorKey, onAnimationStatusChanged: (AnimationStatusListener)this._handleAnimationStatusChanged, controller: this._menuController, childFocusNode: this._buttonFocusNode, alignmentOffset: menuPaddingOffset__75078, clipBehavior: ((SubmenuButton)(object)this.widget).clipBehavior, onClose: () => this._handleClose(), onOpen: () => this._handleOpen(), style: ((SubmenuButton)(object)this.widget).menuStyle, useRootOverlay: ((SubmenuButton)(object)this.widget).useRootOverlay, animated: ((SubmenuButton)(object)this.widget).animated, builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.MenuController, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>?)((context, controller, child) => {
ButtonStyle mergedStyle__77143 = ((this.widget.themeStyleOf(context)?.merge(this.widget.defaultStyleOf(context)) ?? (ButtonStyle)this.widget.defaultStyleOf(context)));
mergedStyle__77143 = (((SubmenuButton)(object)this.widget).style?.merge(mergedStyle__77143) ?? mergedStyle__77143);
void toggleShowMenu()
{
    if (!this.mounted)
    {
        return;
    }
    if (global::Doroti.Generated.Framework.Animation.AnimationStatusMembers.isForwardOrCompleted(this._animationStatus))
    {
        controller.close();
    }
    else
    {
        controller.open();
    }
}
void handlePointerExit(global::Doroti.Generated.Framework.Gestures.PointerExitEvent @event)
{
    if (this._isHovered)
    {
        ((SubmenuButton)(object)this.widget).onHover?.Invoke(false);
        _isHovered = false;
        _clearHoverOpenTimer();
    }
}
void handlePointerHover(global::Doroti.Generated.Framework.Gestures.PointerHoverEvent @event)
{
    if (!this._isHovered)
    {
        _isHovered = true;
        ((SubmenuButton)(object)this.widget).onHover?.Invoke(true);
        _MenuAnchorState__menu_anchor root__78310 = _MenuAnchorState__menu_anchor._maybeOf(context)!._root;
        if (((object.Equals(this._parent?._orientation, global::Doroti.Generated.Framework.Painting.Axis.horizontal)) && !((_MenuAnchorState__menu_anchor)root__78310)._menuController.isOpen))
        {
            return;
        }
        if (((global::Doroti.Generated.Framework.Widgets.FocusNode)this._buttonFocusNode).hasPrimaryFocus)
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
child = new global::Doroti.Generated.Framework.Widgets.MergeSemantics(child: new global::Doroti.Generated.Framework.Widgets.Semantics(expanded: (this._enabled && global::Doroti.Generated.Framework.Animation.AnimationStatusMembers.isForwardOrCompleted(this._animationStatus)), child: new TextButton(style: mergedStyle__77143, focusNode: this._buttonFocusNode, onFocusChange: ((global::System.Action<bool>)(this._enabled ? ((SubmenuButton)(object)this.widget).onFocusChange : null)), onPressed: ((global::System.Action)(this._enabled ? toggleShowMenu : null)), isSemanticButton: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb ? true : null), child: new _MenuItemLabel__menu_anchor(leadingIcon: ((SubmenuButton)(object)this.widget).leadingIcon, trailingIcon: ((SubmenuButton)(object)this.widget).trailingIcon, hasSubmenu: true, showDecoration: (object.Equals(((this._parent?._orientation ?? global::Doroti.Generated.Framework.Painting.Axis.horizontal)), global::Doroti.Generated.Framework.Painting.Axis.vertical)), submenuIcon: submenuIcon__76000, child: child))));
if (!this._enabled)
{
    return child;
}
child = new global::Doroti.Generated.Framework.Widgets.MouseRegion(onHover: (global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerHoverEvent>)handlePointerHover, onExit: (global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerExitEvent>)handlePointerExit, child: child);
if (Menu_anchorLibrary._platformSupportsAccelerators)
{
    return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new MenuAcceleratorCallbackBinding(onInvoke: () => toggleShowMenu(), hasSubmenu: true, child: child));
}
return child;
throw new InvalidOperationException("Dart closure completed without a value.");
})), menuChildren: ((SubmenuButton)(object)this.widget).menuChildren, child: ((SubmenuButton)(object)this.widget).child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleAnimationStatusChanged(global::Doroti.Generated.Framework.Animation.AnimationStatus status)
    {
        setState(((global::System.Action)(() => {
_animationStatus = status;
})));
        ((SubmenuButton)(object)this.widget).onAnimationStatusChanged?.Invoke(status);
    }

    internal virtual void _handleClose()
    {
        if (!((global::Doroti.Generated.Framework.Widgets.FocusNode)this._buttonFocusNode).hasFocus)
        {
            _isOpenOnFocusEnabled = false;
            global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timestamp) => {
global::Doroti.Generated.Framework.Widgets.FocusManager.instance.applyFocusChangesIfNeeded();
_isOpenOnFocusEnabled = true;
})), debugLabel: "MenuAnchor.preventOpenOnFocus");
        }
        ((SubmenuButton)(object)this.widget).onClose?.Invoke();
    }

    internal virtual void _handleOpen()
    {
        if (!this._waitingToFocusMenu)
        {
            global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) => {
if (this.mounted)
{
    this._buttonFocusNode.requestFocus();
    _waitingToFocusMenu = false;
}
})), debugLabel: "MenuAnchor.focus");
            _waitingToFocusMenu = true;
        }
        setState(((global::System.Action)(() => {
})));
        ((SubmenuButton)(object)this.widget).onOpen?.Invoke();
    }

    internal virtual global::Doroti.Generated.Framework.Painting.EdgeInsets _computeMenuPadding(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?> insets__81836 = ((((SubmenuButton)(object)this.widget).menuStyle?.padding ?? MenuTheme.of(context).style?.padding) ?? new _MenuDefaultsM3__menu_anchor(context).padding!);
        return ((global::Doroti.Generated.Framework.Painting.EdgeInsets)(object?)insets__81836.resolve((((SubmenuButton)(object)this.widget).statesController?.value ?? new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>()))!.resolve(Directionality.of(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleFocusChange()
    {
        _clearHoverOpenTimer();
        if (!((global::Doroti.Generated.Framework.Widgets.FocusNode)this._buttonFocusNode).hasPrimaryFocus)
        {
            if ((!this._anchorState!._menuScopeNode.hasFocus && global::Doroti.Generated.Framework.Animation.AnimationStatusMembers.isForwardOrCompleted(this._animationStatus)))
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
        if (((global::Doroti.Generated.Framework.Widgets.MenuController)this._menuController).isOpen)
        {
            if ((!object.Equals(this._animationStatus, global::Doroti.Generated.Framework.Animation.AnimationStatus.reverse)))
            {
                return;
            }
            if (this._isHovered)
            {
                return;
            }
            if ((object.Equals(this._parent?._orientation, global::Doroti.Generated.Framework.Painting.Axis.horizontal)))
            {
                return;
            }
        }
        if ((object.Equals(((SubmenuButton)(object)this.widget).hoverOpenDelay, Duration.zero)))
        {
            this._menuController.open();
            return;
        }
        _hoverOpenTimer = new Timer(((SubmenuButton)(object)this.widget).hoverOpenDelay, (() => {
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
        if (((object.Equals(this._parent?._orientation, global::Doroti.Generated.Framework.Painting.Axis.horizontal)) && (((SubmenuButton)(object)this.widget).hoverOpenDelay > Duration.zero)))
        {
            throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary("A non-zero hoverOpenDelay was used in a top-level SubmenuButton situated in a MenuBar."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("MenuBar children can only be opened by hover if a sibling SubmenuButton is already open. When the hoverOpenDelay for a SubmenuButton is longer than the closing animation of a sibling SubmenuButton, that sibling will close before this SubmenuButton begins opening, leading to this SubmenuButton never opening."), this.context.describeElement("The affected SubmenuButton is") }));
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SubmenuDirectionalFocusAction__menu_anchor : global::Doroti.Generated.Framework.Widgets.DirectionalFocusAction
{
    public virtual _SubmenuButtonState__menu_anchor submenu { get; private set; } = default!;

    internal _SubmenuDirectionalFocusAction__menu_anchor(_SubmenuButtonState__menu_anchor submenu)
    {
        this.submenu = submenu;
    }

    internal virtual _MenuAnchorState__menu_anchor? _parent => ((_SubmenuButtonState__menu_anchor)this.submenu)._parent;
    internal virtual _MenuAnchorState__menu_anchor? _anchorState => ((_SubmenuButtonState__menu_anchor)this.submenu)._anchorState;
    internal virtual global::Doroti.Generated.Framework.Widgets.MenuController _controller => ((_SubmenuButtonState__menu_anchor)this.submenu)._menuController;
    internal virtual global::Doroti.Generated.Framework.Painting.Axis? _orientation => this._parent?._orientation;
    public virtual bool isSubmenu => ((_SubmenuButtonState__menu_anchor)this.submenu)._buttonFocusNode.hasPrimaryFocus;
    internal virtual global::Doroti.Generated.Framework.Widgets.FocusNode _button => ((_SubmenuButtonState__menu_anchor)this.submenu)._buttonFocusNode;
    public override object? invoke(global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent intent, global::Doroti.Generated.Framework.Widgets.BuildContext? context = null)
    {
        DartRuntimePrimitives.Assert(() => Menu_anchorLibrary._debugMenuInfo($"{(((global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent)intent).direction)}: Invoking directional focus intent."));
        global::Doroti.Ui.TextDirection directionality__85063 = Directionality.of(this.submenu.context);
        switch ((this._orientation, directionality__85063, ((global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent)intent).direction))
        {
            case (global::Doroti.Generated.Framework.Painting.Axis.horizontal, TextDirection.ltr, global::Doroti.Generated.Framework.Widgets.TraversalDirection.left):
            case (global::Doroti.Generated.Framework.Painting.Axis.horizontal, TextDirection.rtl, global::Doroti.Generated.Framework.Widgets.TraversalDirection.right):
                {
                    DartRuntimePrimitives.Assert(() => Menu_anchorLibrary._debugMenuInfo($"Moving to previous {typeof(MenuBar)} item"));
                    DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Widgets.FocusNode>)(() =>
{            var __cascade = this._button;
            __cascade.requestFocus();
            __cascade.previousFocus();
            return __cascade;        }))());
                    return null;
                }
            case (global::Doroti.Generated.Framework.Painting.Axis.horizontal, TextDirection.ltr, global::Doroti.Generated.Framework.Widgets.TraversalDirection.right):
            case (global::Doroti.Generated.Framework.Painting.Axis.horizontal, TextDirection.rtl, global::Doroti.Generated.Framework.Widgets.TraversalDirection.left):
                {
                    DartRuntimePrimitives.Assert(() => Menu_anchorLibrary._debugMenuInfo($"Moving to next {typeof(MenuBar)} item"));
                    DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Widgets.FocusNode>)(() =>
{            var __cascade = this._button;
            __cascade.requestFocus();
            __cascade.nextFocus();
            return __cascade;        }))());
                    return null;
                }
            case (global::Doroti.Generated.Framework.Painting.Axis.horizontal, _, global::Doroti.Generated.Framework.Widgets.TraversalDirection.down):
                {
                    if (this.isSubmenu)
                    {
                        this._anchorState?._focusFirstMenuItem();
                        return null;
                    }
                    break;
                }
            case (global::Doroti.Generated.Framework.Painting.Axis.horizontal, _, global::Doroti.Generated.Framework.Widgets.TraversalDirection.up):
                {
                    if (this.isSubmenu)
                    {
                        this._anchorState?._focusLastMenuItem();
                        return null;
                    }
                    break;
                }
            case (global::Doroti.Generated.Framework.Painting.Axis.vertical, TextDirection.ltr, global::Doroti.Generated.Framework.Widgets.TraversalDirection.left):
            case (global::Doroti.Generated.Framework.Painting.Axis.vertical, TextDirection.rtl, global::Doroti.Generated.Framework.Widgets.TraversalDirection.right):
                {
                    if ((object.Equals(((_MenuAnchorState__menu_anchor?)((dynamic)this._parent)?._parent)?._orientation, global::Doroti.Generated.Framework.Painting.Axis.horizontal)))
                    {
                        if (this.isSubmenu)
                        {
                            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Widgets.FocusNode?>)(() =>
{            var __cascade = this._parent!.widget.childFocusNode;
            __cascade.requestFocus();
            __cascade.previousFocus();
            return __cascade;        }))());
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
                            if ((((_MenuAnchorState__menu_anchor?)((dynamic)this._parent)?._parent) is null))
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
            case (global::Doroti.Generated.Framework.Painting.Axis.vertical, TextDirection.ltr, global::Doroti.Generated.Framework.Widgets.TraversalDirection.right) when (this.isSubmenu):
            case (global::Doroti.Generated.Framework.Painting.Axis.vertical, TextDirection.rtl, global::Doroti.Generated.Framework.Widgets.TraversalDirection.left) when (this.isSubmenu):
                {
                    DartRuntimePrimitives.Assert(() => Menu_anchorLibrary._debugMenuInfo("Entering submenu"));
                    if (((global::Doroti.Generated.Framework.Widgets.MenuController)this._controller).isOpen)
                    {
                        this._anchorState?._focusFirstMenuItem();
                    }
                    else
                    {
                        this._controller.open();
                        global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timestamp) => {
if (((global::Doroti.Generated.Framework.Widgets.MenuController)this._controller).isOpen)
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
    internal static DartMap<global::Doroti.Generated.Framework.Services.LogicalKeyboardKey, string> _shortcutGraphicEquivalents = new DartMap<global::Doroti.Generated.Framework.Services.LogicalKeyboardKey, string> { [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowLeft] = "←", [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowRight] = "→", [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowUp] = "↑", [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowDown] = "↓", [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.enter] = "↵" };
    internal static HashSet<global::Doroti.Generated.Framework.Services.LogicalKeyboardKey> _modifiers = new HashSet<global::Doroti.Generated.Framework.Services.LogicalKeyboardKey> { global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.alt, global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.control, global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.meta, global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.shift, global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.altLeft, global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.controlLeft, global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.metaLeft, global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.shiftLeft, global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.altRight, global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.controlRight, global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.metaRight, global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.shiftRight };
    internal virtual DartMap<MaterialLocalizations, DartMap<global::Doroti.Generated.Framework.Services.LogicalKeyboardKey, string>> _cachedShortcutKeys { get; private set; } = new DartMap<MaterialLocalizations, DartMap<global::Doroti.Generated.Framework.Services.LogicalKeyboardKey, string>>();

    internal _LocalizedShortcutLabeler__menu_anchor()
    {
    }

    public static _LocalizedShortcutLabeler__menu_anchor instance
    {
        get
        {
            return _instance ??= new _LocalizedShortcutLabeler__menu_anchor();
            return default!;
        }
    }
    public virtual string getShortcutLabel(global::Doroti.Generated.Framework.Widgets.MenuSerializableShortcut shortcut, MaterialLocalizations localizations)
    {
        global::Doroti.Generated.Framework.Widgets.ShortcutSerialization serialized__91419 = ((global::Doroti.Generated.Framework.Widgets.ShortcutSerialization)(object?)shortcut.serializeForMenu());
        string keySeparator__91478 = default!;
        if (Menu_anchorLibrary._usesSymbolicModifiers)
        {
            keySeparator__91478 = " ";
        }
        else
        {
            keySeparator__91478 = "+";
        }
        if ((((global::Doroti.Generated.Framework.Widgets.ShortcutSerialization)serialized__91419).trigger is not null))
        {
            global::Doroti.Generated.Framework.Services.LogicalKeyboardKey trigger__91746 = ((global::Doroti.Generated.Framework.Widgets.ShortcutSerialization)serialized__91419).trigger!;
            var modifiers__91789 = ((Func<List<string>>)(() => { var __collection91801 = new List<string>(); if (Menu_anchorLibrary._usesSymbolicModifiers) { __collection91801.AddRange(((Func<List<string>>)(() => { var __collection91850 = new List<string>(); if (DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Widgets.ShortcutSerialization)serialized__91419).control)) { __collection91850.Add(_getModifierLabel(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.control, localizations)); } if (DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Widgets.ShortcutSerialization)serialized__91419).alt)) { __collection91850.Add(_getModifierLabel(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.alt, localizations)); } if (DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Widgets.ShortcutSerialization)serialized__91419).shift)) { __collection91850.Add(_getModifierLabel(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.shift, localizations)); } if (DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Widgets.ShortcutSerialization)serialized__91419).meta)) { __collection91850.Add(_getModifierLabel(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.meta, localizations)); } return __collection91850; }))()); } else { __collection91801.AddRange(((Func<List<string>>)(() => { var __collection92331 = new List<string>(); if (DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Widgets.ShortcutSerialization)serialized__91419).alt)) { __collection92331.Add(_getModifierLabel(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.alt, localizations)); } if (DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Widgets.ShortcutSerialization)serialized__91419).control)) { __collection92331.Add(_getModifierLabel(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.control, localizations)); } if (DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Widgets.ShortcutSerialization)serialized__91419).meta)) { __collection92331.Add(_getModifierLabel(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.meta, localizations)); } if (DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Widgets.ShortcutSerialization)serialized__91419).shift)) { __collection92331.Add(_getModifierLabel(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.shift, localizations)); } return __collection92331; }))()); } return __collection91801; }))();
            string? shortcutTrigger__92804 = default!;
            long logicalKeyId__92837 = ((global::Doroti.Generated.Framework.Services.LogicalKeyboardKey)trigger__91746).keyId;
            if (_shortcutGraphicEquivalents.ContainsKey(trigger__91746))
            {
                shortcutTrigger__92804 = _shortcutGraphicEquivalents.GetValueOrDefault(trigger__91746);
            }
            else
            {
                shortcutTrigger__92804 = _getLocalizedName(trigger__91746, localizations);
                if (((shortcutTrigger__92804 is null) && ((logicalKeyId__92837 & global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.planeMask) == 0L)))
                {
                    shortcutTrigger__92804 = char.ConvertFromUtf32(checked((int)(logicalKeyId__92837 & global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.valueMask))).toUpperCase();
                }
                shortcutTrigger__92804 ??= ((global::Doroti.Generated.Framework.Services.LogicalKeyboardKey)trigger__91746).keyLabel;
            }
            return string.Join(keySeparator__91478, ((Func<List<string>>)(() => { var __collection93661 = new List<string>(); __collection93661.AddRange(modifiers__91789); if (((shortcutTrigger__92804 is not null) && (shortcutTrigger__92804.Length != 0))) { __collection93661.Add(shortcutTrigger__92804); } return __collection93661; }))());
        }
        else
        {
            if ((((global::Doroti.Generated.Framework.Widgets.ShortcutSerialization)serialized__91419).character is not null))
            {
                var modifiers__93864 = ((Func<List<string>>)(() => { var __collection93876 = new List<string>(); if (Menu_anchorLibrary._usesSymbolicModifiers) { __collection93876.AddRange(((Func<List<string>>)(() => { var __collection93989 = new List<string>(); if (DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Widgets.ShortcutSerialization)serialized__91419).control)) { __collection93989.Add(_getModifierLabel(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.control, localizations)); } if (DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Widgets.ShortcutSerialization)serialized__91419).alt)) { __collection93989.Add(_getModifierLabel(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.alt, localizations)); } if (DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Widgets.ShortcutSerialization)serialized__91419).meta)) { __collection93989.Add(_getModifierLabel(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.meta, localizations)); } return __collection93989; }))()); } else { __collection93876.AddRange(((Func<List<string>>)(() => { var __collection94377 = new List<string>(); if (DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Widgets.ShortcutSerialization)serialized__91419).alt)) { __collection94377.Add(_getModifierLabel(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.alt, localizations)); } if (DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Widgets.ShortcutSerialization)serialized__91419).control)) { __collection94377.Add(_getModifierLabel(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.control, localizations)); } if (DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Widgets.ShortcutSerialization)serialized__91419).meta)) { __collection94377.Add(_getModifierLabel(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.meta, localizations)); } return __collection94377; }))()); } return __collection93876; }))();
                return string.Join(keySeparator__91478, ((Func<List<string>>)(() => { var __collection94756 = new List<string>(); __collection94756.AddRange(modifiers__93864); __collection94756.Add(((global::Doroti.Generated.Framework.Widgets.ShortcutSerialization)serialized__91419).character!); return __collection94756; }))());
            }
        }
        throw new NotImplementedException("Shortcut labels for ShortcutActivators that do not implement " + "MenuSerializableShortcut (e.g. ShortcutActivators other than SingleActivator or " + "CharacterActivator) are not supported.");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual string? _getLocalizedName(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey key, MaterialLocalizations localizations)
    {
        this._cachedShortcutKeys.putIfAbsent(localizations, () => new DartMap<global::Doroti.Generated.Framework.Services.LogicalKeyboardKey, string> { [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.altGraph] = ((MaterialLocalizations)localizations).keyboardKeyAltGraph, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.backspace] = ((MaterialLocalizations)localizations).keyboardKeyBackspace, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.capsLock] = ((MaterialLocalizations)localizations).keyboardKeyCapsLock, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.channelDown] = ((MaterialLocalizations)localizations).keyboardKeyChannelDown, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.channelUp] = ((MaterialLocalizations)localizations).keyboardKeyChannelUp, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.delete] = ((MaterialLocalizations)localizations).keyboardKeyDelete, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.eject] = ((MaterialLocalizations)localizations).keyboardKeyEject, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.end] = ((MaterialLocalizations)localizations).keyboardKeyEnd, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.escape] = ((MaterialLocalizations)localizations).keyboardKeyEscape, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.fn] = ((MaterialLocalizations)localizations).keyboardKeyFn, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.home] = ((MaterialLocalizations)localizations).keyboardKeyHome, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.insert] = ((MaterialLocalizations)localizations).keyboardKeyInsert, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.numLock] = ((MaterialLocalizations)localizations).keyboardKeyNumLock, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.numpad1] = ((MaterialLocalizations)localizations).keyboardKeyNumpad1, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.numpad2] = ((MaterialLocalizations)localizations).keyboardKeyNumpad2, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.numpad3] = ((MaterialLocalizations)localizations).keyboardKeyNumpad3, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.numpad4] = ((MaterialLocalizations)localizations).keyboardKeyNumpad4, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.numpad5] = ((MaterialLocalizations)localizations).keyboardKeyNumpad5, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.numpad6] = ((MaterialLocalizations)localizations).keyboardKeyNumpad6, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.numpad7] = ((MaterialLocalizations)localizations).keyboardKeyNumpad7, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.numpad8] = ((MaterialLocalizations)localizations).keyboardKeyNumpad8, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.numpad9] = ((MaterialLocalizations)localizations).keyboardKeyNumpad9, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.numpad0] = ((MaterialLocalizations)localizations).keyboardKeyNumpad0, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.numpadAdd] = ((MaterialLocalizations)localizations).keyboardKeyNumpadAdd, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.numpadComma] = ((MaterialLocalizations)localizations).keyboardKeyNumpadComma, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.numpadDecimal] = ((MaterialLocalizations)localizations).keyboardKeyNumpadDecimal, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.numpadDivide] = ((MaterialLocalizations)localizations).keyboardKeyNumpadDivide, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.numpadEnter] = ((MaterialLocalizations)localizations).keyboardKeyNumpadEnter, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.numpadEqual] = ((MaterialLocalizations)localizations).keyboardKeyNumpadEqual, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.numpadMultiply] = ((MaterialLocalizations)localizations).keyboardKeyNumpadMultiply, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.numpadParenLeft] = ((MaterialLocalizations)localizations).keyboardKeyNumpadParenLeft, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.numpadParenRight] = ((MaterialLocalizations)localizations).keyboardKeyNumpadParenRight, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.numpadSubtract] = ((MaterialLocalizations)localizations).keyboardKeyNumpadSubtract, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.pageDown] = ((MaterialLocalizations)localizations).keyboardKeyPageDown, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.pageUp] = ((MaterialLocalizations)localizations).keyboardKeyPageUp, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.power] = ((MaterialLocalizations)localizations).keyboardKeyPower, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.powerOff] = ((MaterialLocalizations)localizations).keyboardKeyPowerOff, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.printScreen] = ((MaterialLocalizations)localizations).keyboardKeyPrintScreen, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.scrollLock] = ((MaterialLocalizations)localizations).keyboardKeyScrollLock, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.select] = ((MaterialLocalizations)localizations).keyboardKeySelect, [global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.space] = ((MaterialLocalizations)localizations).keyboardKeySpace });
        return this._cachedShortcutKeys.GetValueOrDefault(localizations)!.GetValueOrDefault(key);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual string _getModifierLabel(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey modifier, MaterialLocalizations localizations)
    {
        DartRuntimePrimitives.Assert(() => _modifiers.Contains(modifier), () => (object?)$"{(((global::Doroti.Generated.Framework.Services.LogicalKeyboardKey)modifier).keyLabel)} is not a modifier key");
        if ((((object.Equals(modifier, global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.meta)) || (object.Equals(modifier, global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.metaLeft))) || (object.Equals(modifier, global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.metaRight))))
        {
            switch (global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
            {
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.android:
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia:
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
                    {
                        return ((MaterialLocalizations)localizations).keyboardKeyMeta;
                    }
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
                    {
                        return ((MaterialLocalizations)localizations).keyboardKeyMetaWindows;
                    }
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS:
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
                    {
                        return "⌘";
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
        }
        if ((((object.Equals(modifier, global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.alt)) || (object.Equals(modifier, global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.altLeft))) || (object.Equals(modifier, global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.altRight))))
        {
            switch (global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
            {
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.android:
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia:
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
                    {
                        return ((MaterialLocalizations)localizations).keyboardKeyAlt;
                    }
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS:
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
                    {
                        return "⌥";
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
        }
        if ((((object.Equals(modifier, global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.control)) || (object.Equals(modifier, global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.controlLeft))) || (object.Equals(modifier, global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.controlRight))))
        {
            switch (global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
            {
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.android:
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia:
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
                    {
                        return ((MaterialLocalizations)localizations).keyboardKeyControl;
                    }
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS:
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
                    {
                        return "⌃";
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
        }
        if ((((object.Equals(modifier, global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.shift)) || (object.Equals(modifier, global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.shiftLeft))) || (object.Equals(modifier, global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.shiftRight))))
        {
            switch (global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
            {
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.android:
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia:
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
                    {
                        return ((MaterialLocalizations)localizations).keyboardKeyShift;
                    }
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS:
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
                    {
                        return "⇧";
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
        }
        throw DartRuntimePrimitives.AsException(new DartArgumentError($"Keyboard key {(((global::Doroti.Generated.Framework.Services.LogicalKeyboardKey)modifier).keyLabel)} is not a modifier."));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MenuBarAnchor__menu_anchor : MenuAnchor
{
    internal _MenuBarAnchor__menu_anchor(List<global::Doroti.Generated.Framework.Widgets.Widget> menuChildren, global::Doroti.Generated.Framework.Widgets.MenuController? controller = null, Clip clipBehavior = Clip.hardEdge, MenuStyle? style = null) : base(menuChildren: menuChildren, controller: controller, clipBehavior: clipBehavior, style: style)
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
                __late_actions = new DartMap<Type, dynamic> { [typeof(global::Doroti.Generated.Framework.Widgets.DismissIntent)] = new global::Doroti.Generated.Framework.Widgets.DismissMenuAction(controller: this._menuController) };
                __late_actions_initialized = true;
            }
            return __late_actions;
        }
    }

    internal override global::Doroti.Generated.Framework.Painting.Axis _orientation => global::Doroti.Generated.Framework.Painting.Axis.horizontal;
    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        var child__101952 = new global::Doroti.Generated.Framework.Widgets.Actions(actions: this.actions, child: new global::Doroti.Generated.Framework.Widgets.Shortcuts(shortcuts: Menu_anchorLibrary._kMenuTraversalShortcuts, child: new _MenuPanel__menu_anchor(menuStyle: ((MenuAnchor)(object)this.widget).style, clipBehavior: ((MenuAnchor)(object)this.widget).clipBehavior, orientation: this._orientation, children: ((MenuAnchor)(object)this.widget).menuChildren)));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _MenuAnchorScope__menu_anchor(state: this, animationStatus: ((global::Doroti.Generated.Framework.Animation.AnimationController)this._animationController).status, child: new global::Doroti.Generated.Framework.Widgets.RawMenuAnchorGroup(controller: this._menuController, child: new global::Doroti.Generated.Framework.Widgets.Builder(builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>)((context) => {
bool isOpen__102535 = (MenuController.maybeIsOpenOf(context) ?? false);
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.FocusScope(node: this._menuScopeNode, skipTraversal: !isOpen__102535, canRequestFocus: isOpen__102535, descendantsAreFocusable: true, child: new global::Doroti.Generated.Framework.Widgets.ExcludeFocus(excluding: !isOpen__102535, child: child__101952)));
throw new InvalidOperationException("Dart closure completed without a value.");
}))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class MenuAcceleratorCallbackBinding : global::Doroti.Generated.Framework.Widgets.InheritedWidget
{
    public virtual global::System.Action? onInvoke { get; private set; }
    public virtual bool hasSubmenu { get; private set; } = default!;

    public MenuAcceleratorCallbackBinding(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Action? onInvoke = null, bool hasSubmenu = false, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.onInvoke = onInvoke;
        this.hasSubmenu = hasSubmenu;
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget)
    {
        var __oldWidget = (MenuAcceleratorCallbackBinding)(object)oldWidget;
        return ((!object.Equals((global::System.Action?)this.onInvoke, (global::System.Action?)((MenuAcceleratorCallbackBinding)__oldWidget).onInvoke)) || (this.hasSubmenu != ((MenuAcceleratorCallbackBinding)__oldWidget).hasSubmenu));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static MenuAcceleratorCallbackBinding? maybeOf(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((MenuAcceleratorCallbackBinding?)(object?)context.dependOnInheritedWidgetOfExactType<MenuAcceleratorCallbackBinding>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static MenuAcceleratorCallbackBinding of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        MenuAcceleratorCallbackBinding? result__105368 = ((MenuAcceleratorCallbackBinding?)(object?)MenuAcceleratorCallbackBinding.maybeOf(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((result__105368 is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Generated.Framework.Foundation.FlutterError.Create("MenuAcceleratorWrapper.of() was called with a context that does not " + "contain a MenuAcceleratorWrapper in the given context.\n" + "No MenuAcceleratorWrapper ancestor could be found in the context that " + "was passed to MenuAcceleratorWrapper.of(). This can happen because " + "you are using a widget that looks for a MenuAcceleratorWrapper " + "ancestor, and do not have a MenuAcceleratorWrapper widget ancestor.\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
            });
        return result__105368!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public delegate global::Doroti.Generated.Framework.Widgets.Widget MenuAcceleratorChildBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, string label, long index);

public class MenuAcceleratorLabel : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual string label { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, string, long, global::Doroti.Generated.Framework.Widgets.Widget> builder { get; private set; } = default!;

    public MenuAcceleratorLabel(string label, global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, string, long, global::Doroti.Generated.Framework.Widgets.Widget> builder = default!) : base(key: key)
    {
        global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, string, long, global::Doroti.Generated.Framework.Widgets.Widget> __builder = builder ?? defaultLabelBuilder;
        this.label = label;
        this.builder = __builder;
    }

    public virtual string displayLabel => MenuAcceleratorLabel.stripAcceleratorMarkers(this.label);
    public virtual bool hasAccelerator => new RegExp("&(?!([&\\s]|$))").hasMatch(this.label);
    public static global::Doroti.Generated.Framework.Widgets.Widget defaultLabelBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, string label, long index)
    {
        if ((index < 0L))
        {
            return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Text(label));
        }
        global::Doroti.Generated.Framework.Painting.TextStyle defaultStyle__112910 = DefaultTextStyle.of(context).style;
        Characters characters__112982 = label.characters();
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.RichText(text: new global::Doroti.Generated.Framework.Painting.TextSpan(children: ((Func<List<global::Doroti.Generated.Framework.Painting.TextSpan>>)(() => { var __collection113074 = new List<global::Doroti.Generated.Framework.Painting.TextSpan>(); if ((index > 0L)) { __collection113074.Add(new global::Doroti.Generated.Framework.Painting.TextSpan(text: characters__112982.GetRange(0L, index).ToString(), style: defaultStyle__112910)); } __collection113074.Add(new global::Doroti.Generated.Framework.Painting.TextSpan(text: characters__112982.skip(index).take(1L).ToString(), style: defaultStyle__112910.copyWith(decoration: TextDecoration.underline))); if ((index < (characters__112982.Count - 1L))) { __collection113074.Add(new global::Doroti.Generated.Framework.Painting.TextSpan(text: characters__112982.GetRange((index + 1L)).ToString(), style: defaultStyle__112910)); } return __collection113074; }))().Cast<global::Doroti.Generated.Framework.Painting.InlineSpan>().ToList())));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static string stripAcceleratorMarkers(string label, global::System.Action<long>? setIndex = null)
    {
        var quotedAmpersands__114002 = 0L;
        var displayLabel__114034 = new StringBuffer();
        var acceleratorIndex__114073 = -1L;
        Characters labelChars__114220 = label.characters();
        Characters ampersand__114272 = "&".characters();
        var lastWasAmpersand__114308 = false;
        for (var i__114347 = 0L; (i__114347 < labelChars__114220.Count); i__114347 += 1L)
        {
            Characters character__114536 = labelChars__114220.characterAt(i__114347);
            if (lastWasAmpersand__114308)
            {
                lastWasAmpersand__114308 = false;
                displayLabel__114034.write(character__114536);
                continue;
            }
            if ((!object.Equals(character__114536, ampersand__114272)))
            {
                displayLabel__114034.write(character__114536);
                continue;
            }
            if ((i__114347 == (labelChars__114220.Count - 1L)))
            {
                break;
            }
            lastWasAmpersand__114308 = true;
            Characters acceleratorCharacter__114979 = labelChars__114220.characterAt((i__114347 + 1L));
            if ((((acceleratorIndex__114073 == -1L) && (!object.Equals(acceleratorCharacter__114979, ampersand__114272))) && (acceleratorCharacter__114979.ToString().Trim().Length != 0)))
            {
                acceleratorIndex__114073 = (i__114347 - quotedAmpersands__114002);
            }
            quotedAmpersands__114002 += 1L;
        }
        setIndex?.Invoke(acceleratorIndex__114073);
        return ((string)(object?)displayLabel__114034.ToString());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _MenuAcceleratorLabelState__menu_anchor());
    public virtual string ToString(global::Doroti.Generated.Framework.Foundation.DiagnosticLevel minLevel = global::Doroti.Generated.Framework.Foundation.DiagnosticLevel.info)
    {
        return $"{typeof(MenuAcceleratorLabel)}(\"{this.label}\")";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.StringProperty("label", this.label));
    }

}

internal class _MenuAcceleratorLabelState__menu_anchor : global::Doroti.Generated.Framework.Widgets.State<MenuAcceleratorLabel>
{
    internal virtual string _displayLabel { get; set; } = default!;
    internal virtual long _acceleratorIndex { get; set; } = -1L;
    internal virtual MenuAcceleratorCallbackBinding? _binding { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Widgets.MenuController? _menuController { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Widgets.ShortcutRegistry? _shortcutRegistry { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Widgets.ShortcutRegistryEntry? _shortcutRegistryEntry { get; set; } = default;
    internal virtual bool _showAccelerators { get; set; } = false;

    public override void initState()
    {
        base.initState();
        if (Menu_anchorLibrary._platformSupportsAccelerators)
        {
            _showAccelerators = _MenuAcceleratorLabelState__menu_anchor._altIsPressed();
            global::Doroti.Generated.Framework.Services.HardwareKeyboard.instance.addHandler((global::System.Func<global::Doroti.Generated.Framework.Services.KeyEvent, bool>)this._listenToKeyEvent);
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
            global::Doroti.Generated.Framework.Services.HardwareKeyboard.instance.removeHandler((global::System.Func<global::Doroti.Generated.Framework.Services.KeyEvent, bool>)this._listenToKeyEvent);
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
        if ((((MenuAcceleratorLabel)(object)this.widget).label != ((MenuAcceleratorLabel)oldWidget).label))
        {
            _updateDisplayLabel();
        }
    }

    internal static bool _altIsPressed()
    {
        return System.Linq.Enumerable.Any(global::Doroti.Generated.Framework.Services.HardwareKeyboard.instance.logicalKeysPressed.intersection(new HashSet<global::Doroti.Generated.Framework.Services.LogicalKeyboardKey> { global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.altLeft, global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.altRight, global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.alt }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _listenToKeyEvent(global::Doroti.Generated.Framework.Services.KeyEvent @event)
    {
        DartRuntimePrimitives.Assert(() => Menu_anchorLibrary._platformSupportsAccelerators);
        setState(((global::System.Action)(() => {
_showAccelerators = _MenuAcceleratorLabelState__menu_anchor._altIsPressed();
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
            string acceleratorCharacter__118944 = this._displayLabel[(int)(this._acceleratorIndex)].ToString().toLowerCase();
            _shortcutRegistryEntry = this._shortcutRegistry?.addAll(new DartMap<global::Doroti.Generated.Framework.Widgets.ShortcutActivator, global::Doroti.Generated.Framework.Widgets.Intent> { [new global::Doroti.Generated.Framework.Widgets.CharacterActivator(acceleratorCharacter__118944, alt: true)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new global::Doroti.Generated.Framework.Widgets.VoidCallbackIntent(this._binding!.onInvoke!)) });
        }
    }

    internal virtual void _updateDisplayLabel()
    {
        _displayLabel = MenuAcceleratorLabel.stripAcceleratorMarkers(((MenuAcceleratorLabel)(object)this.widget).label, setIndex: ((global::System.Action<long>)((index) => {
_acceleratorIndex = index;
})));
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        long index__119513 = (this._showAccelerators ? this._acceleratorIndex : -1L);
        return this.widget.builder(context, this._displayLabel, index__119513);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MenuItemLabel__menu_anchor : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual bool hasSubmenu { get; private set; } = default!;
    public virtual bool showDecoration { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? leadingIcon { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? trailingIcon { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.MenuSerializableShortcut? shortcut { get; private set; }
    public virtual string? semanticsLabel { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.Axis overflowAxis { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? submenuIcon { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? child { get; private set; }

    internal _MenuItemLabel__menu_anchor(bool hasSubmenu, bool showDecoration = true, global::Doroti.Generated.Framework.Widgets.Widget? leadingIcon = null, global::Doroti.Generated.Framework.Widgets.Widget? trailingIcon = null, global::Doroti.Generated.Framework.Widgets.MenuSerializableShortcut? shortcut = null, string? semanticsLabel = null, global::Doroti.Generated.Framework.Painting.Axis overflowAxis = global::Doroti.Generated.Framework.Painting.Axis.vertical, global::Doroti.Generated.Framework.Widgets.Widget? submenuIcon = null, global::Doroti.Generated.Framework.Widgets.Widget? child = null)
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

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        VisualDensity density__121595 = Theme.of(context).visualDensity;
        double horizontalPadding__121655 = Math.Max(Menu_anchorLibrary._kLabelItemMinSpacing, (Menu_anchorLibrary._kLabelItemDefaultSpacing + (density__121595.horizontal * 2L)));
        global::Doroti.Generated.Framework.Widgets.Widget leadings__121790 = default!;
        if ((object.Equals(this.overflowAxis, global::Doroti.Generated.Framework.Painting.Axis.vertical)))
        {
            leadings__121790 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Expanded(child: new global::Doroti.Generated.Framework.Widgets.ClipRect(child: new global::Doroti.Generated.Framework.Widgets.Row(mainAxisSize: global::Doroti.Generated.Framework.Rendering.MainAxisSize.min, children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection121981 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); var __collectionElement122005 = this.leadingIcon; if (__collectionElement122005 is { } __nonNullCollectionElement122005) { __collection121981.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(__nonNullCollectionElement122005)); } if ((this.child is not null)) { __collection121981.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Expanded(child: new global::Doroti.Generated.Framework.Widgets.ClipRect(child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: ((this.leadingIcon is not null) ? global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: horizontalPadding__121655) : global::Doroti.Generated.Framework.Painting.EdgeInsets.zero), child: this.child))))); } return __collection121981; }))()))));
        }
        else
        {
            leadings__121790 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Row(mainAxisSize: global::Doroti.Generated.Framework.Rendering.MainAxisSize.min, children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection122566 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); var __collectionElement122586 = this.leadingIcon; if (__collectionElement122586 is { } __nonNullCollectionElement122586) { __collection122566.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(__nonNullCollectionElement122586)); } if ((this.child is not null)) { __collection122566.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Padding(padding: ((this.leadingIcon is not null) ? global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: horizontalPadding__121655) : global::Doroti.Generated.Framework.Painting.EdgeInsets.zero), child: this.child))); } return __collection122566; }))()));
        }
        global::Doroti.Generated.Framework.Widgets.Widget menuItemLabel__122884 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Row(mainAxisAlignment: global::Doroti.Generated.Framework.Rendering.MainAxisAlignment.spaceBetween, children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection122978 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); __collection122978.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(leadings__121790)); if ((this.trailingIcon is not null)) { __collection122978.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: horizontalPadding__121655), child: this.trailingIcon))); } if ((this.showDecoration && (this.shortcut is not null))) { __collection122978.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: horizontalPadding__121655), child: new global::Doroti.Generated.Framework.Widgets.Text(_LocalizedShortcutLabeler__menu_anchor.instance.getShortcutLabel(this.shortcut!, MaterialLocalizations.of(context)))))); } if ((this.showDecoration && this.hasSubmenu)) { __collection122978.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: horizontalPadding__121655), child: this.submenuIcon))); } return __collection122978; }))()));
        if ((this.semanticsLabel is not null))
        {
            menuItemLabel__122884 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Semantics(label: this.semanticsLabel, excludeSemantics: true, child: menuItemLabel__122884));
        }
        return menuItemLabel__122884;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.MenuSerializableShortcut>("shortcut", this.shortcut, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("hasSubmenu", this.hasSubmenu));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("showDecoration", this.showDecoration));
    }

}

internal class _MenuLayout__menu_anchor : global::Doroti.Generated.Framework.Rendering.SingleChildLayoutDelegate
{
    public virtual Rect anchorRect { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;
    public virtual Offset alignmentOffset { get; private set; } = default!;
    public virtual Offset? menuPosition { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry menuPadding { get; private set; } = default!;
    public virtual HashSet<Rect> avoidBounds { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.Axis orientation { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.Axis parentOrientation { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry reservedPadding { get; private set; } = default!;
    public virtual double heightFactor { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.MediaQueryData mediaQueryData { get; private set; } = default!;

    internal _MenuLayout__menu_anchor(Rect anchorRect, TextDirection textDirection, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment, Offset alignmentOffset, Offset? menuPosition, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry menuPadding, HashSet<Rect> avoidBounds, global::Doroti.Generated.Framework.Painting.Axis orientation, global::Doroti.Generated.Framework.Painting.Axis parentOrientation, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry reservedPadding, double heightFactor, global::Doroti.Generated.Framework.Widgets.MediaQueryData mediaQueryData)
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

    public override global::Doroti.Generated.Framework.Rendering.BoxConstraints getConstraintsForChild(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints)
    {
        return ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)(object?)global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateLoose(((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).biggest).deflate(this.reservedPadding));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Offset getPositionForChild(Size size, Size childSize)
    {
        global::Doroti.Ui.Rect overlayRect__126685 = ((global::Doroti.Ui.Rect)(object?)((global::Doroti.Generated.Framework.Widgets.MediaQueryData)this.mediaQueryData).padding.deflateRect(((global::Doroti.Generated.Framework.Widgets.MediaQueryData)this.mediaQueryData).viewInsets.deflateRect((Offset.zero & size))));
        double unconstrainedHeight__126824 = ((this.heightFactor > 0.01) ? (childSize.height / this.heightFactor) : 0);
        double childHeightEstimate__126922 = Math.Min(unconstrainedHeight__126824, size.height);
        var childSizeEstimate__126998 = new global::Doroti.Ui.Size(childSize.width, childHeightEstimate__126922);
        global::Doroti.Ui.Offset finalPosition__127082 = ((global::Doroti.Ui.Offset)(object?)_positionChild(childSizeEstimate__126998, overlayRect__126685));
        if ((this.menuPosition is not null))
        {
            Offset menuPosition__value127155 = DartRuntimePrimitives.RequireValue(menuPosition);
            return finalPosition__127082;
        }
        bool growsUp__127385 = ((finalPosition__127082.dy + childSizeEstimate__126998.height) <= ((Offset)((dynamic)this.anchorRect).center).dy);
        if (growsUp__127385)
        {
            double dy__127502 = (childHeightEstimate__126922 - childSize.height);
            return new global::Doroti.Ui.Offset(finalPosition__127082.dx, (finalPosition__127082.dy + dy__127502));
        }
        var initialPosition__127626 = new global::Doroti.Ui.Offset(finalPosition__127082.dx, this.anchorRect.bottom);
        return DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Offset.lerp(initialPosition__127626, finalPosition__127082, this.heightFactor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Offset _positionChild(Size childSize, Rect overlayRect)
    {
        double x__127845 = default!;
        double y__127859 = default!;
        if ((this.menuPosition is null))
        {
            global::Doroti.Ui.Offset desiredPosition__127907 = ((global::Doroti.Ui.Offset)(object?)this.alignment.resolve(this.textDirection).withinRect(this.anchorRect));
            global::Doroti.Ui.Offset directionalOffset__128001 = default!;
            if ((this.alignment is global::Doroti.Generated.Framework.Painting.AlignmentDirectional))
            {
                global::Doroti.Generated.Framework.Painting.AlignmentDirectional alignment__as128030 = (global::Doroti.Generated.Framework.Painting.AlignmentDirectional)alignment;
                directionalOffset__128001 = (this.textDirection switch { TextDirection.rtl => new global::Doroti.Ui.Offset(-this.alignmentOffset.dx, this.alignmentOffset.dy), TextDirection.ltr => this.alignmentOffset, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            }
            else
            {
                directionalOffset__128001 = this.alignmentOffset;
            }
            desiredPosition__127907 += directionalOffset__128001;
            x__127845 = desiredPosition__127907.dx;
            y__127859 = desiredPosition__127907.dy;
            switch (this.textDirection)
            {
                case TextDirection.rtl:
                    {
                        x__127845 -= childSize.width;
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
            global::Doroti.Ui.Offset adjustedPosition__128615 = ((global::Doroti.Ui.Offset)(object?)(DartRuntimePrimitives.RequireValue(this.menuPosition) + this.anchorRect.topLeft));
            x__127845 = adjustedPosition__128615.dx;
            y__127859 = adjustedPosition__128615.dy;
        }
        IEnumerable<global::Doroti.Ui.Rect> subScreens__128764 = ((IEnumerable<global::Doroti.Ui.Rect>)(object?)DisplayFeatureSubScreen.subScreensInBounds(overlayRect, this.avoidBounds));
        global::Doroti.Ui.Rect allowedRect__128881 = ((global::Doroti.Ui.Rect)(object?)_closestScreen(subScreens__128764.Cast<Rect>(), ((Offset)((dynamic)this.anchorRect).center)));
        bool offLeftSide(double x)
        {
            return (x < allowedRect__128881.left);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        bool offRightSide(double x)
        {
            return ((x + childSize.width) > allowedRect__128881.right);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        bool offTop(double y)
        {
            return (y < allowedRect__128881.top);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        bool offBottom(double y)
        {
            return ((y + childSize.height) > allowedRect__128881.bottom);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        if ((childSize.width >= allowedRect__128881.width))
        {
            x__127845 = allowedRect__128881.left;
        }
        else
        {
            if (offLeftSide(x__127845))
            {
                if ((!object.Equals(this.parentOrientation, this.orientation)))
                {
                    x__127845 = allowedRect__128881.left;
                }
                else
                {
                    double newX__129974 = (this.anchorRect.right + this.alignmentOffset.dx);
                    if (!offRightSide(newX__129974))
                    {
                        x__127845 = newX__129974;
                    }
                    else
                    {
                        x__127845 = allowedRect__128881.left;
                    }
                }
            }
            else
            {
                if (offRightSide(x__127845))
                {
                    if ((!object.Equals(this.parentOrientation, this.orientation)))
                    {
                        x__127845 = (allowedRect__128881.right - childSize.width);
                    }
                    else
                    {
                        double newX__130329 = ((this.anchorRect.left - childSize.width) - this.alignmentOffset.dx);
                        if (!offLeftSide(newX__130329))
                        {
                            x__127845 = newX__130329;
                        }
                        else
                        {
                            x__127845 = (allowedRect__128881.right - childSize.width);
                        }
                    }
                }
            }
        }
        if ((childSize.height >= allowedRect__128881.height))
        {
            y__127859 = allowedRect__128881.top;
        }
        else
        {
            if (offTop(y__127859))
            {
                double newY__130746 = this.anchorRect.bottom;
                if (!offBottom(newY__130746))
                {
                    y__127859 = newY__130746;
                }
                else
                {
                    y__127859 = allowedRect__128881.top;
                }
            }
            else
            {
                if (offBottom(y__127859))
                {
                    double newY__130936 = (this.anchorRect.top - childSize.height);
                    if (!offTop(newY__130936))
                    {
                        if ((object.Equals(this.parentOrientation, global::Doroti.Generated.Framework.Painting.Axis.horizontal)))
                        {
                            y__127859 = (newY__130936 - this.alignmentOffset.dy);
                        }
                        else
                        {
                            y__127859 = newY__130936;
                        }
                    }
                    else
                    {
                        y__127859 = (allowedRect__128881.bottom - childSize.height);
                    }
                }
            }
        }
        return new global::Doroti.Ui.Offset(x__127845, y__127859);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRelayout(global::Doroti.Generated.Framework.Rendering.SingleChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (_MenuLayout__menu_anchor)(object)oldDelegate;
        return (((((((((((!object.Equals(this.anchorRect, ((_MenuLayout__menu_anchor)__oldDelegate).anchorRect)) || (!object.Equals(this.textDirection, ((_MenuLayout__menu_anchor)__oldDelegate).textDirection))) || (!object.Equals(this.alignment, ((_MenuLayout__menu_anchor)__oldDelegate).alignment))) || (!object.Equals(this.alignmentOffset, ((_MenuLayout__menu_anchor)__oldDelegate).alignmentOffset))) || (!object.Equals(this.menuPosition, ((_MenuLayout__menu_anchor)__oldDelegate).menuPosition))) || (!object.Equals(this.menuPadding, ((_MenuLayout__menu_anchor)__oldDelegate).menuPadding))) || (!object.Equals(this.orientation, ((_MenuLayout__menu_anchor)__oldDelegate).orientation))) || (!object.Equals(this.parentOrientation, ((_MenuLayout__menu_anchor)__oldDelegate).parentOrientation))) || (!object.Equals(this.reservedPadding, ((_MenuLayout__menu_anchor)__oldDelegate).reservedPadding))) || (this.heightFactor != ((_MenuLayout__menu_anchor)__oldDelegate).heightFactor)) || !global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.setEquals(this.avoidBounds, ((_MenuLayout__menu_anchor)__oldDelegate).avoidBounds));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Rect _closestScreen(IEnumerable<Rect> screens, Offset point)
    {
        global::Doroti.Ui.Rect closest__132094 = ((global::Doroti.Ui.Rect)(object?)screens.First());
        foreach (var screen__132134 in screens)
        {
            if ((((((Offset)((dynamic)screen__132134).center) - point)).distance < ((((Offset)((dynamic)closest__132094).center) - point)).distance))
            {
                closest__132094 = screen__132134;
            }
        }
        return closest__132094;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MenuPanel__menu_anchor : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual MenuStyle? menuStyle { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual bool crossAxisUnconstrained { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.Axis orientation { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double>? heightAnimation { get; private set; }
    public virtual List<global::Doroti.Generated.Framework.Widgets.Widget> children { get; private set; } = default!;

    internal _MenuPanel__menu_anchor(MenuStyle? menuStyle, Clip clipBehavior = Clip.none, global::Doroti.Generated.Framework.Painting.Axis orientation = default!, bool crossAxisUnconstrained = true, global::Doroti.Generated.Framework.Animation.Animation<double>? heightAnimation = null, List<global::Doroti.Generated.Framework.Widgets.Widget> children = default!)
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

internal class _MenuPanelState__menu_anchor : global::Doroti.Generated.Framework.Widgets.State<_MenuPanel__menu_anchor>
{
    public virtual global::Doroti.Generated.Framework.Widgets.ScrollController scrollController { get; set; } = new global::Doroti.Generated.Framework.Widgets.ScrollController();

    public override void dispose()
    {
        this.scrollController.dispose();
        base.dispose();
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        var (themeStyle__133967, defaultStyle__133989) = (((_MenuPanel__menu_anchor)(object)this.widget).orientation switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => (((MenuStyle?, MenuStyle))(DartRuntimePrimitives.ConvertValue<(MenuStyle?, MenuStyle)>((MenuBarTheme.of(context).style, new _MenuBarDefaultsM3__menu_anchor(context))))), global::Doroti.Generated.Framework.Painting.Axis.vertical => (((MenuStyle?, MenuStyle))(DartRuntimePrimitives.ConvertValue<(MenuStyle?, MenuStyle)>((MenuTheme.of(context).style, new _MenuDefaultsM3__menu_anchor(context))))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        MenuStyle? widgetStyle__134231 = ((_MenuPanel__menu_anchor)(object)this.widget).menuStyle;
        P? effectiveValue<P>(global::System.Func<MenuStyle?, P?> getProperty)
        {
            return ((getProperty(widgetStyle__134231) ?? getProperty(themeStyle__133967)) ?? getProperty(defaultStyle__133989));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        P? resolve<P>(global::System.Func<MenuStyle?, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<P>?> getProperty)
        {
            return effectiveValue(((style) => {
return getProperty(style) is { } property ? property.resolve(new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>()) : default;
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        global::Doroti.Ui.Color? backgroundColor__134664 = ((global::Doroti.Ui.Color?)(object?)resolve<global::Doroti.Ui.Color?>(((style) => style?.backgroundColor)));
        global::Doroti.Ui.Color? shadowColor__134762 = ((global::Doroti.Ui.Color?)(object?)resolve<global::Doroti.Ui.Color?>(((style) => style?.shadowColor)));
        global::Doroti.Ui.Color? surfaceTintColor__134852 = ((global::Doroti.Ui.Color?)(object?)resolve<global::Doroti.Ui.Color?>(((style) => style?.surfaceTintColor)));
        double elevation__134952 = (resolve<double?>(((style) => style?.elevation)) ?? 0);
        global::Doroti.Ui.Size? minimumSize__135043 = ((global::Doroti.Ui.Size?)(object?)resolve<global::Doroti.Ui.Size?>(((style) => style?.minimumSize)));
        global::Doroti.Ui.Size? fixedSize__135131 = ((global::Doroti.Ui.Size?)(object?)resolve<global::Doroti.Ui.Size?>(((style) => style?.fixedSize)));
        global::Doroti.Ui.Size? maximumSize__135215 = ((global::Doroti.Ui.Size?)(object?)resolve<global::Doroti.Ui.Size?>(((style) => style?.maximumSize)));
        global::Doroti.Generated.Framework.Painting.BorderSide? side__135309 = resolve<global::Doroti.Generated.Framework.Painting.BorderSide?>(((style) => style?.side));
        global::Doroti.Generated.Framework.Painting.OutlinedBorder shape__135398 = ((global::Doroti.Generated.Framework.Painting.OutlinedBorder)(object?)resolve<global::Doroti.Generated.Framework.Painting.OutlinedBorder?>(((style) => style?.shape))!.copyWith(side: side__135309));
        VisualDensity visualDensity__135527 = (effectiveValue(((style) => style?.visualDensity)) ?? VisualDensity.standard);
        global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry padding__135666 = (resolve<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?>(((style) => style?.padding)) ?? global::Doroti.Generated.Framework.Painting.EdgeInsets.zero);
        global::Doroti.Ui.Offset densityAdjustment__135788 = ((global::Doroti.Ui.Offset)(object?)visualDensity__135527.baseSizeAdjustment);
        double dx__136161 = Math.Max(0, densityAdjustment__135788.dx);
        global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry resolvedPadding__136230 = ((global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry)(object?)padding__135666.add(global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: dx__136161)).clamp(global::Doroti.Generated.Framework.Painting.EdgeInsets.zero, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry.infinity));
        global::Doroti.Generated.Framework.Rendering.BoxConstraints effectiveConstraints__136389 = visualDensity__135527.effectiveConstraints(new global::Doroti.Generated.Framework.Rendering.BoxConstraints(minWidth: (minimumSize__135043?.width ?? 0), minHeight: (minimumSize__135043?.height ?? 0), maxWidth: (maximumSize__135215?.width ?? double.PositiveInfinity), maxHeight: (maximumSize__135215?.height ?? double.PositiveInfinity)));
        if ((fixedSize__135131 is not null))
        {
            Size fixedSize__135131__value136698 = DartRuntimePrimitives.RequireValue(fixedSize__135131);
            global::Doroti.Ui.Size size__136736 = ((global::Doroti.Ui.Size)(object?)effectiveConstraints__136389.constrain(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(fixedSize__135131__value136698))));
            if (double.IsFinite(size__136736.width))
            {
                effectiveConstraints__136389 = effectiveConstraints__136389.copyWith(minWidth: size__136736.width, maxWidth: size__136736.width);
            }
            if (double.IsFinite(size__136736.height))
            {
                effectiveConstraints__136389 = effectiveConstraints__136389.copyWith(minHeight: size__136736.height, maxHeight: size__136736.height);
            }
        }
        List<global::Doroti.Generated.Framework.Widgets.Widget> children__137350 = ((_MenuPanel__menu_anchor)(object)this.widget).children.ToList();
        if ((object.Equals(((_MenuPanel__menu_anchor)(object)this.widget).orientation, global::Doroti.Generated.Framework.Painting.Axis.horizontal)))
        {
            children__137350 = children__137350.map<global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Widgets.Widget>(((child) => {
return new global::Doroti.Generated.Framework.Widgets.IntrinsicWidth(child: child);
throw new InvalidOperationException("Dart closure completed without a value.");
})).ToList();
        }
        bool displayScrollbar__137568 = (_MenuAnchorState__menu_anchor._maybeAnimationStatusOf(context) switch { global::Doroti.Generated.Framework.Animation.AnimationStatus.completed => true, global::Doroti.Generated.Framework.Animation.AnimationStatus.forward or global::Doroti.Generated.Framework.Animation.AnimationStatus.reverse or global::Doroti.Generated.Framework.Animation.AnimationStatus.dismissed => false, null => false, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Generated.Framework.Widgets.Widget menuPanel__137830 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Padding(padding: resolvedPadding__136230, child: new global::Doroti.Generated.Framework.Widgets.ScrollConfiguration(behavior: ScrollConfiguration.of(context).copyWith(scrollbars: false, overscroll: false, physics: new global::Doroti.Generated.Framework.Widgets.ClampingScrollPhysics()), child: new global::Doroti.Generated.Framework.Widgets.PrimaryScrollController(controller: this.scrollController, child: new Scrollbar(thumbVisibility: displayScrollbar__137568, child: new global::Doroti.Generated.Framework.Widgets.SingleChildScrollView(controller: this.scrollController, scrollDirection: ((_MenuPanel__menu_anchor)(object)this.widget).orientation, child: new global::Doroti.Generated.Framework.Widgets.Flex(crossAxisAlignment: global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.start, textDirection: Directionality.of(context), direction: ((_MenuPanel__menu_anchor)(object)this.widget).orientation, mainAxisSize: global::Doroti.Generated.Framework.Rendering.MainAxisSize.min, children: children__137350)))))));
        if ((((_MenuPanel__menu_anchor)(object)this.widget).heightAnimation is not null))
        {
            menuPanel__137830 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.AnimatedBuilder(animation: ((_MenuPanel__menu_anchor)(object)this.widget).heightAnimation!, builder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)this._buildAnimatedHeight, child: menuPanel__137830));
        }
        menuPanel__137830 = _intrinsicCrossSize(child: new Material(elevation: elevation__134952, shape: shape__135398, color: backgroundColor__134664, shadowColor: shadowColor__134762, surfaceTintColor: surfaceTintColor__134852, type: ((backgroundColor__134664 is null) ? MaterialType.transparency : MaterialType.canvas), clipBehavior: ((_MenuPanel__menu_anchor)(object)this.widget).clipBehavior, child: menuPanel__137830));
        if (((_MenuPanel__menu_anchor)(object)this.widget).crossAxisUnconstrained)
        {
            menuPanel__137830 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.UnconstrainedBox(constrainedAxis: ((_MenuPanel__menu_anchor)(object)this.widget).orientation, clipBehavior: Clip.hardEdge, alignment: global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerStart, child: menuPanel__137830));
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: effectiveConstraints__136389, child: menuPanel__137830));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _intrinsicCrossSize(global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)(((_MenuPanel__menu_anchor)(object)this.widget).orientation switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.SingleChildRenderObjectWidget>(new global::Doroti.Generated.Framework.Widgets.IntrinsicHeight(child: child)), global::Doroti.Generated.Framework.Painting.Axis.vertical => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.SingleChildRenderObjectWidget>(new global::Doroti.Generated.Framework.Widgets.IntrinsicWidth(child: child)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildAnimatedHeight(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget? child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Align(alignment: global::Doroti.Generated.Framework.Painting.AlignmentDirectional.topStart, heightFactor: ((_MenuPanel__menu_anchor)(object)this.widget).heightAnimation!.value, widthFactor: 1, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _Submenu__menu_anchor : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.FocusScopeNode menuScopeNode { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.RawMenuOverlayInfo menuPosition { get; private set; } = default!;
    public virtual _MenuAnchorState__menu_anchor anchor { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.LayerLink? layerLink { get; private set; }
    public virtual MenuStyle? menuStyle { get; private set; }
    public virtual bool consumeOutsideTaps { get; private set; } = default!;
    public virtual Offset alignmentOffset { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual bool crossAxisUnconstrained { get; private set; } = default!;
    public virtual List<global::Doroti.Generated.Framework.Widgets.Widget> menuChildren { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> fadeAnimation { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> heightAnimation { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry reservedPadding { get; private set; } = default!;

    internal _Submenu__menu_anchor(_MenuAnchorState__menu_anchor anchor, global::Doroti.Generated.Framework.Rendering.LayerLink? layerLink, MenuStyle? menuStyle, global::Doroti.Generated.Framework.Widgets.RawMenuOverlayInfo menuPosition, Offset alignmentOffset, bool consumeOutsideTaps, Clip clipBehavior, bool crossAxisUnconstrained = true, List<global::Doroti.Generated.Framework.Widgets.Widget> menuChildren = default!, global::Doroti.Generated.Framework.Widgets.FocusScopeNode menuScopeNode = default!, global::Doroti.Generated.Framework.Animation.Animation<double> fadeAnimation = default!, global::Doroti.Generated.Framework.Animation.Animation<double> heightAnimation = default!, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry reservedPadding = default!)
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

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.TextDirection textDirection__141416 = Directionality.of(context);
        var (themeStyle__141482, defaultStyle__141504) = (((_MenuAnchorState__menu_anchor)this.anchor)._parent?._orientation switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => (((MenuStyle?, MenuStyle))(DartRuntimePrimitives.ConvertValue<(MenuStyle?, MenuStyle)>((MenuBarTheme.of(context).style, new _MenuBarDefaultsM3__menu_anchor(context))))), null => (((MenuStyle?, MenuStyle))(DartRuntimePrimitives.ConvertValue<(MenuStyle?, MenuStyle)>((MenuBarTheme.of(context).style, new _MenuBarDefaultsM3__menu_anchor(context))))), global::Doroti.Generated.Framework.Painting.Axis.vertical => (((MenuStyle?, MenuStyle))(DartRuntimePrimitives.ConvertValue<(MenuStyle?, MenuStyle)>((MenuTheme.of(context).style, new _MenuDefaultsM3__menu_anchor(context))))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        T? effectiveValue<T>(global::System.Func<MenuStyle?, T?> getProperty)
        {
            return ((getProperty(this.menuStyle) ?? getProperty(themeStyle__141482)) ?? getProperty(defaultStyle__141504));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        T? resolve<T>(global::System.Func<MenuStyle?, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<T>?> getProperty)
        {
            return effectiveValue(((style) => {
return ((T?)(object?)DartRuntimePrimitives.NullAware(getProperty(style), __target => __target.resolve(new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>())));
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        global::Doroti.Generated.Framework.Widgets.WidgetStateMouseCursor mouseCursor__142157 = ((global::Doroti.Generated.Framework.Widgets.WidgetStateMouseCursor)(object?)new _MouseCursor__menu_anchor(((global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, global::Doroti.Generated.Framework.Services.MouseCursor?>)((states) => effectiveValue(((style) => style?.mouseCursor?.resolve(states)))))));
        VisualDensity visualDensity__142337 = (effectiveValue(((style) => style?.visualDensity)) ?? Theme.of(context).visualDensity);
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment__142492 = effectiveValue(((style) => style?.alignment))!;
        global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry padding__142590 = (resolve<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?>(((style) => style?.padding)) ?? global::Doroti.Generated.Framework.Painting.EdgeInsets.zero);
        global::Doroti.Ui.Offset densityAdjustment__142712 = ((global::Doroti.Ui.Offset)(object?)visualDensity__142337.baseSizeAdjustment);
        double dx__143037 = Math.Max(0, densityAdjustment__142712.dx);
        global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry resolvedPadding__143106 = ((global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry)(object?)padding__142590.add(new global::Doroti.Generated.Framework.Painting.EdgeInsets(dx__143037, 0, dx__143037, 0)).clamp(global::Doroti.Generated.Framework.Painting.EdgeInsets.zero, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry.infinity));
        global::Doroti.Ui.Rect anchorRect__143258 = ((global::Doroti.Ui.Rect)(object?)((this.layerLink is null) ? global::Doroti.Ui.Rect.fromLTRB((((global::Doroti.Generated.Framework.Widgets.RawMenuOverlayInfo)this.menuPosition).anchorRect.left + dx__143037), ((global::Doroti.Generated.Framework.Widgets.RawMenuOverlayInfo)this.menuPosition).anchorRect.top, ((global::Doroti.Generated.Framework.Widgets.RawMenuOverlayInfo)this.menuPosition).anchorRect.right, ((global::Doroti.Generated.Framework.Widgets.RawMenuOverlayInfo)this.menuPosition).anchorRect.bottom) : Rect.zero));
        global::Doroti.Generated.Framework.Widgets.Widget menuPanel__143540 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.TapRegion(groupId: ((global::Doroti.Generated.Framework.Widgets.RawMenuOverlayInfo)this.menuPosition).tapRegionGroupId, consumeOutsideTaps: (((_MenuAnchorState__menu_anchor)this.anchor)._root._menuController.isOpen && this.anchor.widget.consumeOutsideTap), onTapOutside: ((global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerDownEvent>)((@event) => {
((_MenuAnchorState__menu_anchor)this.anchor)._menuController.close();
})), child: new global::Doroti.Generated.Framework.Widgets.MouseRegion(cursor: mouseCursor__142157, hitTestBehavior: global::Doroti.Generated.Framework.Rendering.HitTestBehavior.deferToChild, child: new global::Doroti.Generated.Framework.Widgets.FocusScope(node: ((_MenuAnchorState__menu_anchor)this.anchor)._menuScopeNode, skipTraversal: true, child: new global::Doroti.Generated.Framework.Widgets.Actions(actions: new DartMap<Type, dynamic> { [typeof(global::Doroti.Generated.Framework.Widgets.DismissIntent)] = new global::Doroti.Generated.Framework.Widgets.DismissMenuAction(controller: ((_MenuAnchorState__menu_anchor)this.anchor)._menuController) }, child: new global::Doroti.Generated.Framework.Widgets.Shortcuts(shortcuts: Menu_anchorLibrary._kMenuTraversalShortcuts, child: new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: this.fadeAnimation, alwaysIncludeSemantics: true, child: new _MenuPanel__menu_anchor(menuStyle: this.menuStyle, clipBehavior: this.clipBehavior, orientation: ((_MenuAnchorState__menu_anchor)this.anchor)._orientation, crossAxisUnconstrained: this.crossAxisUnconstrained, heightAnimation: this.heightAnimation, children: this.menuChildren))))))));
        global::Doroti.Generated.Framework.Widgets.Widget layout__144826 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new Theme(data: Theme.of(context).copyWith(visualDensity: visualDensity__142337), child: new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateLoose(((global::Doroti.Generated.Framework.Widgets.RawMenuOverlayInfo)this.menuPosition).overlaySize), child: new global::Doroti.Generated.Framework.Widgets.AnimatedBuilder(animation: this.heightAnimation, builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)((context, child) => {
global::Doroti.Generated.Framework.Widgets.MediaQueryData mediaQuery__145172 = ((global::Doroti.Generated.Framework.Widgets.MediaQueryData)(object?)MediaQuery.of(context));
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.CustomSingleChildLayout(@delegate: new _MenuLayout__menu_anchor(anchorRect: anchorRect__143258, textDirection: textDirection__141416, avoidBounds: DisplayFeatureSubScreen.avoidBounds(mediaQuery__145172).toSet(), menuPadding: resolvedPadding__143106, alignment: alignment__142492, alignmentOffset: this.alignmentOffset, menuPosition: ((global::Doroti.Generated.Framework.Widgets.RawMenuOverlayInfo)this.menuPosition).position, orientation: ((_MenuAnchorState__menu_anchor)this.anchor)._orientation, parentOrientation: (((_MenuAnchorState__menu_anchor)this.anchor)._parent?._orientation ?? global::Doroti.Generated.Framework.Painting.Axis.horizontal), reservedPadding: this.reservedPadding, heightFactor: ((global::Doroti.Generated.Framework.Animation.Animation<double>)this.heightAnimation).value, mediaQueryData: mediaQuery__145172), child: menuPanel__143540));
throw new InvalidOperationException("Dart closure completed without a value.");
}))))));
        if ((this.layerLink is null))
        {
            return layout__144826;
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.CompositedTransformFollower(link: this.layerLink!, targetAnchor: global::Doroti.Generated.Framework.Painting.Alignment.bottomLeft, child: layout__144826));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MouseCursor__menu_anchor : global::Doroti.Generated.Framework.Widgets.WidgetStateMouseCursor
{
    public virtual global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, global::Doroti.Generated.Framework.Services.MouseCursor?> resolveCallback { get; private set; } = default!;

    internal _MouseCursor__menu_anchor(global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, global::Doroti.Generated.Framework.Services.MouseCursor?> resolveCallback)
    {
        this.resolveCallback = resolveCallback;
    }

    public override global::Doroti.Generated.Framework.Services.MouseCursor resolve(HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> states) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Services.MouseCursor>((this.resolveCallback(states) ?? global::Doroti.Generated.Framework.Services.MouseCursor.uncontrolled));
    public override string debugDescription => "Menu_MouseCursor";
}

public static partial class Menu_anchorLibrary
{
    internal static bool _debugMenuInfo(string message, IEnumerable<string>? details = null)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (Menu_anchorLibrary._kDebugMenus)
                {
                    global::Doroti.Generated.Framework.Foundation.PrintLibrary.debugPrint($"MENU: {message}");
                    if (((details is not null) && System.Linq.Enumerable.Any(details)))
                    {
                        foreach (string detail__147207 in details)
                        {
                            global::Doroti.Generated.Framework.Foundation.PrintLibrary.debugPrint($"    {detail__147207}");
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
            switch (global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
            {
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS:
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
                    {
                        return true;
                    }
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.android:
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia:
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
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
            return Menu_anchorLibrary._isCupertino;
        }
    }
}

public static partial class Menu_anchorLibrary
{
    internal static bool _platformSupportsAccelerators
    {
        get
        {
            return !Menu_anchorLibrary._isCupertino;
        }
    }
}

internal class _MenuBarDefaultsM3__menu_anchor : MenuStyle
{
    internal static global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder _defaultMenuBorder = new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(4.0)));
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;
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

    internal _MenuBarDefaultsM3__menu_anchor(global::Doroti.Generated.Framework.Widgets.BuildContext context) : base(elevation: new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<double?>(3.0), shape: new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.OutlinedBorder>(_defaultMenuBorder), alignment: global::Doroti.Generated.Framework.Painting.AlignmentDirectional.bottomStart)
    {
        this.context = context;
    }

    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? backgroundColor
    {
        get
        {
            return ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>)(object?)new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Color>(this._colors.surfaceContainer));
            return default!;
        }
    }
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? shadowColor
    {
        get
        {
            return ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>?)(object?)new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Color>(this._colors.shadow));
            return default!;
        }
    }
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? surfaceTintColor
    {
        get
        {
            return ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>?)(object?)new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Color>(Colors.transparent));
            return default!;
        }
    }
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?>? padding
    {
        get
        {
            return ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?>?)(object?)new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateSymmetric(horizontal: Menu_anchorLibrary._kTopLevelMenuHorizontalMinPadding)));
            return default!;
        }
    }
    public override VisualDensity? visualDensity => Theme.of(this.context).visualDensity;
}

internal class _MenuButtonDefaultsM3__menu_anchor : ButtonStyle
{
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;
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

    internal _MenuButtonDefaultsM3__menu_anchor(global::Doroti.Generated.Framework.Widgets.BuildContext context) : base(animationDuration: ConstantsLibrary.kThemeChangeDuration, enableFeedback: true, alignment: global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerStart)
    {
        this.context = context;
    }

    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? backgroundColor
    {
        get
        {
            return ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>?)(object?)ButtonStyleButton.allOrNull<global::Doroti.Ui.Color>(Colors.transparent));
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>? elevation
    {
        get
        {
            return ButtonStyleButton.allOrNull<double>(0.0);
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? foregroundColor
    {
        get
        {
            return ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>?)(object?)WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return (this._colors.onSurface.withOpacity(0.38));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
{
    return (this._colors.onSurface);
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
{
    return (this._colors.onSurface);
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
{
    return (this._colors.onSurface);
}
return (this._colors.onSurface);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? iconColor
    {
        get
        {
            return ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>?)(object?)WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return (this._colors.onSurface.withOpacity(0.38));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
{
    return (this._colors.onSurfaceVariant);
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
{
    return (this._colors.onSurfaceVariant);
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
{
    return (this._colors.onSurfaceVariant);
}
return (this._colors.onSurfaceVariant);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>? iconSize
    {
        get
        {
            return ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>?)(object?)new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<double?>(24.0));
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>? maximumSize
    {
        get
        {
            return ButtonStyleButton.allOrNull<global::Doroti.Ui.Size>(Size.infinite);
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>? minimumSize
    {
        get
        {
            return ButtonStyleButton.allOrNull<global::Doroti.Ui.Size>(new global::Doroti.Ui.Size(64.0, 48.0));
            return default!;
        }
    }
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>>(global::Doroti.Generated.Framework.Widgets.WidgetStateMouseCursor.adaptiveClickable);
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? overlayColor
    {
        get
        {
            return ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>?)(object?)WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
{
    return (this._colors.onSurface.withOpacity(0.1));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
{
    return (this._colors.onSurface.withOpacity(0.08));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
{
    return (this._colors.onSurface.withOpacity(0.1));
}
return (Colors.transparent);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>? padding
    {
        get
        {
            return ButtonStyleButton.allOrNull<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(_scaledPadding(this.context));
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder>? shape
    {
        get
        {
            return ButtonStyleButton.allOrNull<global::Doroti.Generated.Framework.Painting.OutlinedBorder>(new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder());
            return default!;
        }
    }
    public override InteractiveInkFeatureFactory? splashFactory => Theme.of(this.context).splashFactory;
    public override MaterialTapTargetSize? tapTargetSize => Theme.of(this.context).materialTapTargetSize;
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?> textStyle
    {
        get
        {
            return ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?>)(object?)new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.TextStyle>(this._textTheme.labelLarge));
            return default!;
        }
    }
    public override VisualDensity? visualDensity => Theme.of(this.context).visualDensity;
    internal virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry _scaledPadding(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        VisualDensity visualDensity__153973 = Theme.of(context).visualDensity;
        if ((visualDensity__153973.horizontal > 0L))
        {
            visualDensity__153973 = new VisualDensity(vertical: visualDensity__153973.vertical);
        }
        double fontSize__154734 = (Theme.of(context).textTheme.labelLarge?.fontSize ?? 14.0);
        double fontSizeRatio__154820 = (MediaQuery.textScalerOf(context).scale(fontSize__154734) / 14.0);
        return ButtonStyleButton.scaledPadding(global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: Math.Max(Menu_anchorLibrary._kMenuViewPadding, (Menu_anchorLibrary._kLabelItemDefaultSpacing + visualDensity__153973.baseSizeAdjustment.dx))), global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: Math.Max(Menu_anchorLibrary._kMenuViewPadding, (8L + visualDensity__153973.baseSizeAdjustment.dx))), global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: Menu_anchorLibrary._kMenuViewPadding), fontSizeRatio__154820);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MenuDefaultsM3__menu_anchor : MenuStyle
{
    internal static global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder _defaultMenuBorder = new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(4.0)));
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;
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

    internal _MenuDefaultsM3__menu_anchor(global::Doroti.Generated.Framework.Widgets.BuildContext context) : base(elevation: new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<double?>(3.0), shape: new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.OutlinedBorder>(_defaultMenuBorder), alignment: global::Doroti.Generated.Framework.Painting.AlignmentDirectional.topEnd)
    {
        this.context = context;
    }

    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? backgroundColor
    {
        get
        {
            return ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>)(object?)new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Color>(this._colors.surfaceContainer));
            return default!;
        }
    }
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? surfaceTintColor
    {
        get
        {
            return ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>?)(object?)new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Color>(Colors.transparent));
            return default!;
        }
    }
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? shadowColor
    {
        get
        {
            return ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>?)(object?)new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Color>(this._colors.shadow));
            return default!;
        }
    }
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?>? padding
    {
        get
        {
            return ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?>?)(object?)new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateSymmetric(vertical: Menu_anchorLibrary._kMenuVerticalMinPadding)));
            return default!;
        }
    }
    public override VisualDensity? visualDensity => Theme.of(this.context).visualDensity;
}
