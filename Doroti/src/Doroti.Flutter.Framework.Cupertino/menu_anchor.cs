// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/cupertino/menu_anchor.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Cupertino;

public static partial class Menu_anchorLibrary
{
    internal static DartMap<global::Doroti.Generated.Framework.Widgets.ShortcutActivator, global::Doroti.Generated.Framework.Widgets.Intent> _kMenuTraversalShortcuts = new DartMap<global::Doroti.Generated.Framework.Widgets.ShortcutActivator, global::Doroti.Generated.Framework.Widgets.Intent> { [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowUp)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new _FocusUpIntent__menu_anchor()), [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowDown)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new _FocusDownIntent__menu_anchor()), [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.home)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new _FocusFirstIntent__menu_anchor()), [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.end)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new _FocusLastIntent__menu_anchor()) };
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
    internal static string _kBodyFont = "CupertinoSystemText";
}

public static partial class Menu_anchorLibrary
{
    internal static string _kDisplayFont = "CupertinoSystemDisplay";
}

public static partial class Menu_anchorLibrary
{
    internal static double _kCupertinoMobileBaseFontSize = 17.0;
}

public static partial class Menu_anchorLibrary
{
    internal static double _normalizeTextScale(global::Doroti.Generated.Framework.Painting.TextScaler textScaler)
    {
        if ((object.Equals(textScaler, global::Doroti.Generated.Framework.Painting.TextScaler.noScaling)))
        {
            return 0;
        }
        return (textScaler.scale(Menu_anchorLibrary._kCupertinoMobileBaseFontSize) - Menu_anchorLibrary._kCupertinoMobileBaseFontSize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Menu_anchorLibrary
{
    internal static double _kMinimumNormalizedLargeTextScale = 11;
}

public static partial class Menu_anchorLibrary
{
    internal static double _kMinimumTextScaleFactor = (1L - (3L / Menu_anchorLibrary._kCupertinoMobileBaseFontSize));
}

public static partial class Menu_anchorLibrary
{
    internal static double _kMaximumTextScaleFactor = (1L + (36L / Menu_anchorLibrary._kCupertinoMobileBaseFontSize));
}

public static partial class Menu_anchorLibrary
{
    internal static bool _largeTextModeEnabled(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Generated.Framework.Painting.TextScaler? textScaler__3923 = ((global::Doroti.Generated.Framework.Painting.TextScaler?)(object?)MediaQuery.maybeTextScalerOf(context));
        if ((textScaler__3923 is null))
        {
            return false;
        }
        return (Menu_anchorLibrary._normalizeTextScale(textScaler__3923) >= Menu_anchorLibrary._kMinimumNormalizedLargeTextScale);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal enum _CupertinoMenuWidth__menu_anchor
{
    iPadOS,
    iPadOSAccessible,
    iOS,
    iOSAccessible
}

internal static class _CupertinoMenuWidth__menu_anchorMembers
{
    internal static double points(this _CupertinoMenuWidth__menu_anchor value) => value switch
    {
        _CupertinoMenuWidth__menu_anchor.iPadOS => 262.0,
        _CupertinoMenuWidth__menu_anchor.iPadOSAccessible => 343.0,
        _CupertinoMenuWidth__menu_anchor.iOS => 250.0,
        _ => 370.0,
    };

    internal static _CupertinoMenuWidth__menu_anchor CreateFromScreenWidth(bool isLargeTextModeEnabled, double screenWidth) =>
        screenWidth >= 768.0
            ? (isLargeTextModeEnabled ? _CupertinoMenuWidth__menu_anchor.iPadOSAccessible : _CupertinoMenuWidth__menu_anchor.iPadOS)
            : (isLargeTextModeEnabled ? _CupertinoMenuWidth__menu_anchor.iOSAccessible : _CupertinoMenuWidth__menu_anchor.iOS);
}
internal enum _DynamicTypeStyle__menu_anchor
{
    body,
    subhead
}

internal static class _DynamicTypeStyle__menu_anchorMembers
{
    private const long _kScaleCount = 12;
    private static readonly List<long> _normalizedBodyScales = new() { -3, -2, -1, 0, 2, 4, 6, 11, 16, 23, 30, 36 };
    private static readonly double[] _bodySizes = { 14, 15, 16, 17, 19, 21, 23, 28, 33, 40, 47, 53 };
    private static readonly double[] _subheadSizes = { 12, 13, 14, 15, 17, 19, 21, 26, 31, 38, 45, 51 };
    private static List<global::Doroti.Generated.Framework.Painting.TextStyle> styles(this _DynamicTypeStyle__menu_anchor value) =>
        (value == _DynamicTypeStyle__menu_anchor.body ? _bodySizes : _subheadSizes)
            .Select(size => new global::Doroti.Generated.Framework.Painting.TextStyle(fontSize: size)).ToList();
    private static double _interpolateUnits(double value, double min, double max) => (value - min) / (max - min);
    public static global::Doroti.Generated.Framework.Painting.TextStyle resolveTextStyle(this _DynamicTypeStyle__menu_anchor value, global::Doroti.Generated.Framework.Painting.TextScaler textScaler)
    {
        DartRuntimePrimitives.Assert(() => (checked((long)(value.styles().Count)) == _kScaleCount));
        double units__8465 = Menu_anchorLibrary._normalizeTextScale(textScaler);
        for (var i__8519 = 0L; (i__8519 < checked((long)(value.styles().Count))); i__8519++)
        {
            long bodyUnits__8568 = _normalizedBodyScales[(int)(i__8519)];
            if ((units__8465 > bodyUnits__8568))
            {
                continue;
            }
            if ((units__8465 == bodyUnits__8568))
            {
                return value.styles()[(int)(i__8519)];
            }
            if ((i__8519 == 0L))
            {
                return value.styles().First();
            }
            return TextStyle.lerp(value.styles()[(int)((i__8519 - 1L))], value.styles()[(int)(i__8519)], _interpolateUnits(units__8465, _normalizedBodyScales[(int)((i__8519 - 1L))], bodyUnits__8568))!;
        }
        return value.styles().Last();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Menu_anchorLibrary
{
    internal static double _computeSquaredDistanceToRect(Offset point, Rect rect)
    {
        double dx__9497 = (point.dx - Dart_uiLibrary.clampDouble(point.dx, rect.left, rect.right));
        double dy__9577 = (point.dy - Dart_uiLibrary.clampDouble(point.dy, rect.top, rect.bottom));
        return ((dx__9497 * dx__9497) + (dy__9577 * dy__9577));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Menu_anchorLibrary
{
    internal static double _roundToDivisible(double value, double to)
    {
        if ((to == 0L))
        {
            return value;
        }
        return (((value / to)).round() * to);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public interface CupertinoMenuEntry
{
    public bool hasLeading(global::Doroti.Generated.Framework.Widgets.BuildContext context);
    public bool isDivider { get; }
}

internal class _AnchorScope__menu_anchor : global::Doroti.Generated.Framework.Widgets.InheritedWidget
{
    public virtual bool hasLeading { get; private set; } = default!;

    internal _AnchorScope__menu_anchor(bool hasLeading, global::Doroti.Generated.Framework.Widgets.Widget child) : base(child: child)
    {
        this.hasLeading = hasLeading;
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget)
    {
        var __oldWidget = (_AnchorScope__menu_anchor)(object)oldWidget;
        return (this.hasLeading != ((_AnchorScope__menu_anchor)__oldWidget).hasLeading);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public delegate void CupertinoMenuAnimationStatusChangedCallback(global::Doroti.Generated.Framework.Animation.AnimationStatus status);

public class CupertinoMenuAnchor : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.MenuController? controller { get; private set; }
    public virtual global::System.Action? onOpen { get; private set; }
    public virtual global::System.Action? onClose { get; private set; }
    public virtual AnimationStatusListener? onAnimationStatusChanged { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints { get; private set; }
    public virtual bool constrainCrossAxis { get; private set; } = default!;
    public virtual bool consumeOutsideTaps { get; private set; } = default!;
    public virtual bool enableSwipe { get; private set; } = default!;
    public virtual bool enableLongPressToOpen { get; private set; } = default!;
    public virtual bool useRootOverlay { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry overlayPadding { get; private set; } = default!;
    public virtual List<global::Doroti.Generated.Framework.Widgets.Widget> menuChildren { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.MenuController, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>? builder { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? child { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.FocusNode? childFocusNode { get; private set; }

    public CupertinoMenuAnchor(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.MenuController? controller = null, global::System.Action? onOpen = null, global::System.Action? onClose = null, AnimationStatusListener? onAnimationStatusChanged = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, bool constrainCrossAxis = false, bool consumeOutsideTaps = false, bool enableSwipe = true, bool enableLongPressToOpen = false, bool useRootOverlay = false, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry overlayPadding = default!, List<global::Doroti.Generated.Framework.Widgets.Widget> menuChildren = default!, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.MenuController, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>? builder = null, global::Doroti.Generated.Framework.Widgets.Widget? child = null, global::Doroti.Generated.Framework.Widgets.FocusNode? childFocusNode = null) : base(key: key)
    {
        global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry __overlayPadding = overlayPadding ?? global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateAll(8);
        this.controller = controller;
        this.onOpen = onOpen;
        this.onClose = onClose;
        this.onAnimationStatusChanged = onAnimationStatusChanged;
        this.constraints = constraints;
        this.constrainCrossAxis = constrainCrossAxis;
        this.consumeOutsideTaps = consumeOutsideTaps;
        this.enableSwipe = enableSwipe;
        this.enableLongPressToOpen = enableLongPressToOpen;
        this.useRootOverlay = useRootOverlay;
        this.overlayPadding = __overlayPadding;
        this.menuChildren = menuChildren;
        this.builder = builder;
        this.child = child;
        this.childFocusNode = childFocusNode;
        System.Diagnostics.Debug.Assert((enableSwipe || !enableLongPressToOpen));
    }

    public static bool? maybeHasLeadingOf(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return context.dependOnInheritedWidgetOfExactType<_AnchorScope__menu_anchor>()?.hasLeading;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoMenuAnchorState__menu_anchor());
    public virtual List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        return this.menuChildren.map<global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>(((child) => ((Diagnosticable)child).toDiagnosticsNode())).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.FocusNode?>("childFocusNode", this.childFocusNode));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Rendering.BoxConstraints?>("constraints", this.constraints));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("constrainCrossAxis", value: this.constrainCrossAxis, ifTrue: "constrains cross axis"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("enableSwipe", value: this.enableSwipe, ifTrue: "swipe enabled", ifFalse: "swipe disabled"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("consumeOutsideTaps", value: this.consumeOutsideTaps, ifTrue: "consumes outside taps"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("useRootOverlay", value: this.useRootOverlay, ifTrue: "uses root overlay"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>("overlayPadding", this.overlayPadding));
    }

}

internal class _CupertinoMenuAnchorState__menu_anchor : global::Doroti.Generated.Framework.Widgets.State<CupertinoMenuAnchor>, global::Doroti.Generated.Framework.Widgets.TickerProviderStateMixin<CupertinoMenuAnchor>
{
    internal static Duration _kLongPressToOpenDuration = Duration.Create(milliseconds: 400L);
    internal static global::Doroti.Generated.Framework.Physics.Tolerance _kSpringTolerance = new global::Doroti.Generated.Framework.Physics.Tolerance(velocity: 0.1);
    public static global::Doroti.Generated.Framework.Physics.SpringDescription forwardSpring = global::Doroti.Generated.Framework.Physics.SpringDescription.CreateWithDurationAndBounce(duration: Duration.Create(milliseconds: 337L), bounce: 0.2);
    public static global::Doroti.Generated.Framework.Physics.SpringDescription reverseSpring = global::Doroti.Generated.Framework.Physics.SpringDescription.CreateWithDurationAndBounce(duration: Duration.Create(milliseconds: 409L));
    internal virtual global::Doroti.Generated.Framework.Animation.AnimationController _animationController { get; private set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Widgets.FocusScopeNode _menuScopeNode { get; private set; } = new global::Doroti.Generated.Framework.Widgets.FocusScopeNode(debugLabel: "Menu Scope");
    internal virtual global::Doroti.Generated.Framework.Foundation.ValueNotifier<double> _swipeDistanceNotifier { get; private set; } = new global::Doroti.Generated.Framework.Foundation.ValueNotifier<double>(0);
    internal virtual bool? _hasLeadingWidget { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Widgets.MenuController? _internalMenuController { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.AnimationStatus _animationStatus { get; set; } = global::Doroti.Generated.Framework.Animation.AnimationStatus.dismissed;
    public virtual HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual global::Doroti.Generated.Framework.Widgets.MenuController _menuController => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.MenuController>((((CupertinoMenuAnchor)this.widget).controller ?? this._internalMenuController!));
    public virtual bool isOpenOrOpening => global::Doroti.Generated.Framework.Animation.AnimationStatusMembers.isForwardOrCompleted(this._animationStatus);
    public virtual bool enableSwipe => DartRuntimePrimitives.ConvertValue<bool>((((CupertinoMenuAnchor)this.widget).enableSwipe && (this._animationStatus switch { global::Doroti.Generated.Framework.Animation.AnimationStatus.forward or global::Doroti.Generated.Framework.Animation.AnimationStatus.completed => true, global::Doroti.Generated.Framework.Animation.AnimationStatus.dismissed => true, global::Doroti.Generated.Framework.Animation.AnimationStatus.reverse => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })));
    public override void initState()
    {
        base.initState();
        if ((((CupertinoMenuAnchor)this.widget).controller is null))
        {
            _internalMenuController = new global::Doroti.Generated.Framework.Widgets.MenuController();
        }
        _animationController = global::Doroti.Generated.Framework.Animation.AnimationController.CreateUnbounded(vsync: this);
        this._animationController.addStatusListener((AnimationStatusListener)this._handleAnimationStatusChange);
    }

    public override void didUpdateWidget(CupertinoMenuAnchor oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((CupertinoMenuAnchor)oldWidget).controller, ((CupertinoMenuAnchor)this.widget).controller)))
        {
            if ((((CupertinoMenuAnchor)this.widget).controller is not null))
            {
                _internalMenuController = null;
            }
            else
            {
                DartRuntimePrimitives.Assert(() => (this._internalMenuController is null));
                _internalMenuController = new global::Doroti.Generated.Framework.Widgets.MenuController();
            }
        }
        if ((!object.Equals(((CupertinoMenuAnchor)oldWidget).menuChildren, ((CupertinoMenuAnchor)this.widget).menuChildren)))
        {
            _hasLeadingWidget = _resolveHasLeading();
        }
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _hasLeadingWidget ??= _resolveHasLeading();
    }

    public override void dispose()
    {
        this._menuScopeNode.dispose();
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Animation.AnimationController>)(() =>
{            var __cascade = this._animationController;
            __cascade.stop();
            __cascade.dispose();
            return __cascade;        }))());
        _internalMenuController = null;
        this._swipeDistanceNotifier.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Generated.Framework.Scheduler.Ticker ticker__18989 in this._tickers!)
                    {
                        if (((global::Doroti.Generated.Framework.Scheduler.Ticker)ticker__18989).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker__18989.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual bool _resolveHasLeading()
    {
        return ((CupertinoMenuAnchor)this.widget).menuChildren.any(((element) => {
return (element switch { CupertinoMenuEntry entry__23260 => entry__23260.hasLeading(this.context), _ => false });
throw new InvalidOperationException("Dart closure completed without a value.");
}));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleAnimationStatusChange(global::Doroti.Generated.Framework.Animation.AnimationStatus status)
    {
        setState(((global::System.Action)(() => {
_animationStatus = status;
})));
        ((CupertinoMenuAnchor)this.widget).onAnimationStatusChanged?.Invoke(status);
    }

    internal virtual void _handleSwipeDistanceChange(double distance)
    {
        if (!((global::Doroti.Generated.Framework.Widgets.MenuController)this._menuController).isOpen)
        {
            return;
        }
        this._swipeDistanceNotifier.value = distance;
    }

    internal virtual void _handleAnchorSwipeStart()
    {
        if ((this.isOpenOrOpening || !((CupertinoMenuAnchor)this.widget).enableLongPressToOpen))
        {
            return;
        }
        this._menuController.open();
    }

    internal virtual void _handleCloseRequested(global::System.Action hideMenu)
    {
        if (this._animationStatus is global::Doroti.Generated.Framework.Animation.AnimationStatus.reverse or global::Doroti.Generated.Framework.Animation.AnimationStatus.dismissed)
        {
            return;
        }
        DartRuntimePrimitives.Ignore(this._animationController.animateBackWith(new global::Doroti.Generated.Framework.Physics.ClampedSimulation(new global::Doroti.Generated.Framework.Physics.SpringSimulation(reverseSpring, ((global::Doroti.Generated.Framework.Animation.AnimationController)this._animationController).value, 0.0, 0.0, tolerance: _kSpringTolerance), xMin: 0.0, xMax: 1.0)).whenComplete(() => { ((Action)hideMenu)(); return default!; }));
    }

    internal virtual void _handleOpenRequested(Offset? position, global::System.Action showOverlay)
    {
        showOverlay();
        if (this._animationStatus is global::Doroti.Generated.Framework.Animation.AnimationStatus.completed or global::Doroti.Generated.Framework.Animation.AnimationStatus.forward)
        {
            return;
        }
        this._animationController.animateWith(new global::Doroti.Generated.Framework.Physics.SpringSimulation(forwardSpring, ((global::Doroti.Generated.Framework.Animation.AnimationController)this._animationController).value, 1, 0.5));
        FocusScope.of(this.context).setFirstFocus(this._menuScopeNode);
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildMenuOverlay(global::Doroti.Generated.Framework.Widgets.BuildContext childContext, global::Doroti.Generated.Framework.Widgets.RawMenuOverlayInfo info)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.ExcludeSemantics(excluding: !this.isOpenOrOpening, child: new global::Doroti.Generated.Framework.Widgets.IgnorePointer(ignoring: !this.isOpenOrOpening, child: new global::Doroti.Generated.Framework.Widgets.ExcludeFocus(excluding: !this.isOpenOrOpening, child: new _MenuOverlay__menu_anchor(constrainCrossAxis: ((CupertinoMenuAnchor)this.widget).constrainCrossAxis, visibilityAnimation: ((global::Doroti.Generated.Framework.Animation.AnimationController)this._animationController).view, swipeDistanceListenable: this._swipeDistanceNotifier, constraints: ((CupertinoMenuAnchor)this.widget).constraints, consumeOutsideTaps: ((CupertinoMenuAnchor)this.widget).consumeOutsideTaps, overlaySize: ((global::Doroti.Generated.Framework.Widgets.RawMenuOverlayInfo)info).overlaySize, anchorRect: ((global::Doroti.Generated.Framework.Widgets.RawMenuOverlayInfo)info).anchorRect, anchorPosition: ((global::Doroti.Generated.Framework.Widgets.RawMenuOverlayInfo)info).position, tapRegionGroupId: ((global::Doroti.Generated.Framework.Widgets.RawMenuOverlayInfo)info).tapRegionGroupId, focusScopeNode: this._menuScopeNode, overlayPadding: ((CupertinoMenuAnchor)this.widget).overlayPadding, children: ((CupertinoMenuAnchor)this.widget).menuChildren)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildChild(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.MenuController controller, global::Doroti.Generated.Framework.Widgets.Widget? child)
    {
        global::Doroti.Generated.Framework.Widgets.Widget anchor__25995 = (((((CupertinoMenuAnchor)this.widget).builder is null ? ((CupertinoMenuAnchor)this.widget).child : ((CupertinoMenuAnchor)this.widget).builder.Invoke(context, this._menuController, ((CupertinoMenuAnchor)this.widget).child))) ?? global::Doroti.Generated.Framework.Widgets.SizedBox.CreateShrink());
        if ((!((CupertinoMenuAnchor)this.widget).enableLongPressToOpen || !this.enableSwipe))
        {
            return anchor__25995;
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _SwipeSurface__menu_anchor(onStart: () => this._handleAnchorSwipeStart(), delay: _kLongPressToOpenDuration, child: anchor__25995));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _SwipeRegion__menu_anchor(onDistanceChanged: (global::System.Action<double>)this._handleSwipeDistanceChange, enabled: this.enableSwipe, child: new _AnchorScope__menu_anchor(hasLeading: DartRuntimePrimitives.RequireValue(this._hasLeadingWidget), child: new global::Doroti.Generated.Framework.Widgets.RawMenuAnchor(useRootOverlay: ((CupertinoMenuAnchor)this.widget).useRootOverlay, onCloseRequested: (global::System.Action<global::System.Action>)this._handleCloseRequested, onOpenRequested: (global::System.Action<Offset?, global::System.Action>)this._handleOpenRequested, overlayBuilder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.RawMenuOverlayInfo, global::Doroti.Generated.Framework.Widgets.Widget>)this._buildMenuOverlay, builder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.MenuController, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)this._buildChild, controller: this._menuController, childFocusNode: ((CupertinoMenuAnchor)this.widget).childFocusNode, consumeOutsideTaps: ((CupertinoMenuAnchor)this.widget).consumeOutsideTaps, onClose: () => ((CupertinoMenuAnchor)this.widget).onClose(), onOpen: () => ((CupertinoMenuAnchor)this.widget).onOpen()))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>();
        TickerModeData values__17506 = this._tickerModeNotifier!.value;
        var result__17553 = ((Func<global::Doroti.Generated.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{            var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
            __cascade.muted = !((TickerModeData)values__17506).enabled;
            __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
            return __cascade;        }))();
        this._tickers!.Add(result__17553);
        return ((global::Doroti.Generated.Framework.Scheduler.Ticker)(object?)result__17553);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Generated.Framework.Widgets._WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => (this._tickers is not null));
        DartRuntimePrimitives.Assert(() => this._tickers!.Contains(ticker));
        this._tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if ((this._tickers is not null))
        {
            TickerModeData values__18318 = this._tickerModeNotifier!.value;
            bool muted__18372 = !((TickerModeData)values__18318).enabled;
            foreach (global::Doroti.Generated.Framework.Scheduler.Ticker ticker__18421 in this._tickers!)
            {
                ticker__18421.muted = muted__18372;
                ticker__18421.forceFrames = ((TickerModeData)values__18318).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__18621 = ((global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__18621, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier__18621.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier__18621;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

public class _MenuOverlay__menu_anchor : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual List<global::Doroti.Generated.Framework.Widgets.Widget> children { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.FocusScopeNode focusScopeNode { get; private set; } = default!;
    public virtual bool consumeOutsideTaps { get; private set; } = default!;
    public virtual bool constrainCrossAxis { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints { get; private set; }
    public virtual Size overlaySize { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry overlayPadding { get; private set; } = default!;
    public virtual Rect anchorRect { get; private set; } = default!;
    public virtual Offset? anchorPosition { get; private set; }
    public virtual object tapRegionGroupId { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> visibilityAnimation { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<double> swipeDistanceListenable { get; private set; } = default!;

    internal _MenuOverlay__menu_anchor(List<global::Doroti.Generated.Framework.Widgets.Widget> children, global::Doroti.Generated.Framework.Widgets.FocusScopeNode focusScopeNode, bool consumeOutsideTaps, bool constrainCrossAxis, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints, Size overlaySize, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry overlayPadding, Rect anchorRect, Offset? anchorPosition, object tapRegionGroupId, global::Doroti.Generated.Framework.Animation.Animation<double> visibilityAnimation, global::Doroti.Generated.Framework.Foundation.ValueListenable<double> swipeDistanceListenable)
    {
        this.children = children;
        this.focusScopeNode = focusScopeNode;
        this.consumeOutsideTaps = consumeOutsideTaps;
        this.constrainCrossAxis = constrainCrossAxis;
        this.constraints = constraints;
        this.overlaySize = overlaySize;
        this.overlayPadding = overlayPadding;
        this.anchorRect = anchorRect;
        this.anchorPosition = anchorPosition;
        this.tapRegionGroupId = tapRegionGroupId;
        this.visibilityAnimation = visibilityAnimation;
        this.swipeDistanceListenable = swipeDistanceListenable;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _MenuOverlayState__menu_anchor());
}

internal class _MenuOverlayState__menu_anchor : global::Doroti.Generated.Framework.Widgets.State<_MenuOverlay__menu_anchor>, global::Doroti.Generated.Framework.Widgets.TickerProviderStateMixin<_MenuOverlay__menu_anchor>, global::Doroti.Generated.Framework.Widgets.WidgetsBindingObserver
{
    internal static Offset _kAttachmentOffset = new global::Doroti.Flutter.Ui.Offset(0, 8);
    internal static DartMap<Type, dynamic> _kActions = new DartMap<Type, dynamic> { [typeof(_FocusDownIntent__menu_anchor)] = new _FocusDownAction__menu_anchor(), [typeof(_FocusUpIntent__menu_anchor)] = new _FocusUpAction__menu_anchor(), [typeof(_FocusFirstIntent__menu_anchor)] = new _FocusFirstAction__menu_anchor(), [typeof(_FocusLastIntent__menu_anchor)] = new _FocusLastAction__menu_anchor() };
    internal virtual global::Doroti.Generated.Framework.Animation.AnimationController _swipeAnimationController { get; private set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Widgets.ScrollController _scrollController { get; private set; } = new global::Doroti.Generated.Framework.Widgets.ScrollController();
    internal virtual global::Doroti.Generated.Framework.Animation.ProxyAnimation _scaleAnimation { get; private set; } = new global::Doroti.Generated.Framework.Animation.ProxyAnimation();
    internal virtual global::Doroti.Generated.Framework.Animation.ProxyAnimation _fadeAnimation { get; private set; } = new global::Doroti.Generated.Framework.Animation.ProxyAnimation();
    internal virtual global::Doroti.Generated.Framework.Animation.ProxyAnimation _sizeAnimation { get; private set; } = new global::Doroti.Generated.Framework.Animation.ProxyAnimation();
    internal virtual global::Doroti.Generated.Framework.Painting.Alignment _attachmentPointAlignment { get; set; } = default!;
    internal virtual Offset _attachmentPoint { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Painting.Alignment _menuAlignment { get; set; } = default!;
    internal virtual List<global::Doroti.Generated.Framework.Widgets.Widget> _children { get; set; } = new List<global::Doroti.Generated.Framework.Widgets.Widget>();
    internal virtual TextDirection? _textDirection { get; set; } = default;
    internal virtual double _swipeTargetDistance { get; set; } = 0;
    internal virtual double _swipeCurrentDistance { get; set; } = 0;
    internal virtual double _swipeVelocity { get; set; } = 0;
    internal virtual global::Doroti.Generated.Framework.Scheduler.Ticker? _swipeTicker { get; set; } = default;
    public virtual HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        global::Doroti.Generated.Framework.Widgets.WidgetsBinding.instance.addObserver(this);
        _swipeAnimationController = global::Doroti.Generated.Framework.Animation.AnimationController.CreateUnbounded(value: 1, vsync: this);
        ((_MenuOverlay__menu_anchor)this.widget).swipeDistanceListenable.addListener(() => this._handleSwipeDistanceChanged());
        _resolveChildren();
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Flutter.Ui.TextDirection newTextDirection__29868 = Directionality.of(this.context);
        if ((!object.Equals(this._textDirection, newTextDirection__29868)))
        {
            _textDirection = newTextDirection__29868;
            _resolvePosition();
        }
        _resolveMotion();
    }

    public override void didUpdateWidget(_MenuOverlay__menu_anchor oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((_MenuOverlay__menu_anchor)oldWidget).swipeDistanceListenable, ((_MenuOverlay__menu_anchor)this.widget).swipeDistanceListenable)))
        {
            ((_MenuOverlay__menu_anchor)oldWidget).swipeDistanceListenable.removeListener(() => this._handleSwipeDistanceChanged());
            ((_MenuOverlay__menu_anchor)this.widget).swipeDistanceListenable.addListener(() => this._handleSwipeDistanceChanged());
        }
        if ((!object.Equals(((_MenuOverlay__menu_anchor)oldWidget).visibilityAnimation, ((_MenuOverlay__menu_anchor)this.widget).visibilityAnimation)))
        {
            _resolveMotion();
        }
        if ((((!object.Equals(((_MenuOverlay__menu_anchor)oldWidget).anchorRect, ((_MenuOverlay__menu_anchor)this.widget).anchorRect)) || (!object.Equals(((_MenuOverlay__menu_anchor)oldWidget).anchorPosition, ((_MenuOverlay__menu_anchor)this.widget).anchorPosition))) || (!object.Equals(((_MenuOverlay__menu_anchor)oldWidget).overlaySize, ((_MenuOverlay__menu_anchor)this.widget).overlaySize))))
        {
            _resolvePosition();
        }
        if ((!object.Equals(((_MenuOverlay__menu_anchor)oldWidget).children, ((_MenuOverlay__menu_anchor)this.widget).children)))
        {
            _resolveChildren();
        }
    }

    public virtual void didChangeAccessibilityFeatures()
    {
        base.didChangeAccessibilityFeatures();
        _resolveMotion();
    }

    public override void dispose()
    {
        this._scrollController.dispose();
        ((_MenuOverlay__menu_anchor)this.widget).swipeDistanceListenable.removeListener(() => this._handleSwipeDistanceChanged());
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Scheduler.Ticker?>)(() =>
{            var __cascade = this._swipeTicker;
            __cascade.stop();
            __cascade.dispose();
            return __cascade;        }))());
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Animation.AnimationController>)(() =>
{            var __cascade = this._swipeAnimationController;
            __cascade.stop();
            __cascade.dispose();
            return __cascade;        }))());
        this._scaleAnimation.parent = null;
        this._fadeAnimation.parent = null;
        this._sizeAnimation.parent = null;
        global::Doroti.Generated.Framework.Widgets.WidgetsBinding.instance.removeObserver(this);
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Generated.Framework.Scheduler.Ticker ticker__18989 in this._tickers!)
                    {
                        if (((global::Doroti.Generated.Framework.Scheduler.Ticker)ticker__18989).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker__18989.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual void _resolveChildren()
    {
        if (!System.Linq.Enumerable.Any(((_MenuOverlay__menu_anchor)this.widget).children))
        {
            _children = new List<global::Doroti.Generated.Framework.Widgets.Widget>();
            return;
        }
        var children__31489 = new List<global::Doroti.Generated.Framework.Widgets.Widget>();
        global::Doroti.Generated.Framework.Widgets.Widget child__31523 = ((_MenuOverlay__menu_anchor)this.widget).children.First();
        for (var i__31567 = 0L; (i__31567 < checked((long)(((_MenuOverlay__menu_anchor)this.widget).children.Count))); i__31567++)
        {
            children__31489.Add(child__31523);
            if ((object.Equals(child__31523, ((_MenuOverlay__menu_anchor)this.widget).children.Last())))
            {
                break;
            }
            if (child__31523 is CupertinoMenuEntry { isDivider: true } __object31724)
            {
                child__31523 = ((_MenuOverlay__menu_anchor)this.widget).children[(int)((i__31567 + 1L))];
                continue;
            }
            child__31523 = ((_MenuOverlay__menu_anchor)this.widget).children[(int)((i__31567 + 1L))];
            if (child__31523 is CupertinoMenuEntry { isDivider: true } __object31889)
            {
                continue;
            }
            children__31489.Add(new _CupertinoMenuImplicitDivider__menu_anchor());
        }
        _children = children__31489;
    }

    internal virtual void _resolveMotion()
    {
        global::Doroti.Flutter.Ui.AccessibilityFeatures accessibilityFeatures__32330 = ((global::Doroti.Flutter.Ui.AccessibilityFeatures)(object?)View.of(this.context).platformDispatcher.accessibilityFeatures);
        switch (accessibilityFeatures__32330)
        {
            case global::Doroti.Flutter.Ui.AccessibilityFeatures { disableAnimations: true } __object32475:
                {
                    this._scaleAnimation.parent = global::Doroti.Generated.Framework.Animation.AnimationsLibrary.kAlwaysCompleteAnimation;
                    this._fadeAnimation.parent = global::Doroti.Generated.Framework.Animation.AnimationsLibrary.kAlwaysCompleteAnimation;
                    this._sizeAnimation.parent = global::Doroti.Generated.Framework.Animation.AnimationsLibrary.kAlwaysCompleteAnimation;
                    break;
                }
            case global::Doroti.Flutter.Ui.AccessibilityFeatures { reduceMotion: true } __object32712:
                {
                    this._scaleAnimation.parent = ((global::Doroti.Generated.Framework.Animation.AnimationController)this._swipeAnimationController).view.drive(new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: 0.8, end: 1));
                    this._sizeAnimation.parent = global::Doroti.Generated.Framework.Animation.AnimationsLibrary.kAlwaysCompleteAnimation;
                    this._fadeAnimation.parent = ((_MenuOverlay__menu_anchor)this.widget).visibilityAnimation.drive(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: global::Doroti.Generated.Framework.Animation.Curves.easeIn).chain(new _ClampTween__menu_anchor(begin: 0, end: 1)));
                    break;
                }
            default:
                {
                    this._scaleAnimation.parent = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Animation.Animation<double>>(new _AnimationProduct__menu_anchor(first: ((_MenuOverlay__menu_anchor)this.widget).visibilityAnimation, next: ((global::Doroti.Generated.Framework.Animation.AnimationController)this._swipeAnimationController).view.drive(new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: 0.8, end: 1))));
                    this._sizeAnimation.parent = ((_MenuOverlay__menu_anchor)this.widget).visibilityAnimation.drive(new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: 0.8, end: 1));
                    this._fadeAnimation.parent = ((_MenuOverlay__menu_anchor)this.widget).visibilityAnimation.drive(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: global::Doroti.Generated.Framework.Animation.Curves.easeIn).chain(new _ClampTween__menu_anchor(begin: 0, end: 1)));
                    break;
                }
        }
    }

    internal virtual void _resolvePosition()
    {
        global::Doroti.Flutter.Ui.Offset anchorMidpoint__33923 = default!;
        if ((((_MenuOverlay__menu_anchor)this.widget).anchorPosition is not null))
        {
            anchorMidpoint__33923 = (((_MenuOverlay__menu_anchor)this.widget).anchorRect.topLeft + DartRuntimePrimitives.RequireValue(((_MenuOverlay__menu_anchor)this.widget).anchorPosition));
        }
        else
        {
            anchorMidpoint__33923 = ((Offset)((dynamic)((_MenuOverlay__menu_anchor)this.widget).anchorRect).center);
        }
        double xMidpointRatio__34141 = (anchorMidpoint__33923.dx / ((_MenuOverlay__menu_anchor)this.widget).overlaySize.width);
        double yMidpointRatio__34221 = (anchorMidpoint__33923.dy / ((_MenuOverlay__menu_anchor)this.widget).overlaySize.height);
        double dy__34417 = ((yMidpointRatio__34221 < 0.55) ? 1 : -1);
        double dx__34471 = (xMidpointRatio__34141 switch { < 0.4 => -1.0, > 0.6 => 1.0, _ => 0.0 });
        _menuAlignment = new global::Doroti.Generated.Framework.Painting.Alignment(dx__34471, -dy__34417);
        global::Doroti.Flutter.Ui.Offset transformOrigin__34652 = default!;
        if ((((_MenuOverlay__menu_anchor)this.widget).anchorPosition is not null))
        {
            _attachmentPoint = (((_MenuOverlay__menu_anchor)this.widget).anchorRect.topLeft + DartRuntimePrimitives.RequireValue(((_MenuOverlay__menu_anchor)this.widget).anchorPosition));
            transformOrigin__34652 = this._attachmentPoint;
        }
        else
        {
            global::Doroti.Flutter.Ui.Offset offset__34864 = ((global::Doroti.Flutter.Ui.Offset)(object?)(_kAttachmentOffset * dy__34417));
            _attachmentPoint = (new global::Doroti.Generated.Framework.Painting.Alignment(dx__34471, dy__34417).withinRect(((_MenuOverlay__menu_anchor)this.widget).anchorRect) + offset__34864);
            transformOrigin__34652 = (new global::Doroti.Generated.Framework.Painting.Alignment(0, dy__34417).withinRect(((_MenuOverlay__menu_anchor)this.widget).anchorRect) + offset__34864);
        }
        double xOriginRatio__35086 = (transformOrigin__34652.dx / ((_MenuOverlay__menu_anchor)this.widget).overlaySize.width);
        double yOriginRatio__35165 = (transformOrigin__34652.dy / ((_MenuOverlay__menu_anchor)this.widget).overlaySize.height);
        _attachmentPointAlignment = new global::Doroti.Generated.Framework.Painting.Alignment(((xOriginRatio__35086 * 2L) - 1L), ((yOriginRatio__35165 * 2L) - 1L));
    }

    internal virtual void _handleOutsideTap(global::Doroti.Generated.Framework.Gestures.PointerDownEvent @event)
    {
        MenuController.maybeOf(this.context)!.close();
    }

    internal virtual void _handleSwipeDistanceChanged()
    {
        _swipeTargetDistance = Dart_uiLibrary.clampDouble(((_MenuOverlay__menu_anchor)this.widget).swipeDistanceListenable.value, 0, 150);
        if ((this._swipeCurrentDistance == this._swipeTargetDistance))
        {
            return;
        }
        _swipeTicker ??= createTicker((global::System.Action<Duration>)this._updateSwipeScale);
        if (!this._swipeTicker!.isActive)
        {
            this._swipeTicker!.start();
        }
    }

    internal virtual void _updateSwipeScale(Duration elapsed)
    {
        var maxVelocity__36030 = 20.0;
        var minVelocity__36060 = 8.0;
        var maxSwipeDistance__36089 = 150.0;
        var accelerationRate__36125 = 0.12;
        var decelerationDistanceThreshold__36507 = 80.0;
        var remainingDistanceSnapThreshold__36662 = 1.0;
        var terminationDistanceThreshold__36828 = 5.0;
        double distance__36882 = (this._swipeTargetDistance - this._swipeCurrentDistance);
        double absoluteDistance__36956 = distance__36882.abs();
        double proximityFactor__37386 = Math.Min((absoluteDistance__36956 / decelerationDistanceThreshold__36507), 1.0);
        _swipeVelocity += (accelerationRate__36125 * proximityFactor__37386);
        _swipeVelocity = Dart_uiLibrary.clampDouble(this._swipeVelocity, minVelocity__36060, maxVelocity__36030);
        double finalVelocity__37625 = (this._swipeVelocity * proximityFactor__37386);
        double distanceReduction__37692 = (Math.Sign(distance__36882) * finalVelocity__37625);
        _swipeCurrentDistance += distanceReduction__37692;
        if ((absoluteDistance__36956 < remainingDistanceSnapThreshold__36662))
        {
            _swipeCurrentDistance = this._swipeTargetDistance;
            _swipeVelocity = 0;
            if ((this._swipeTargetDistance < terminationDistanceThreshold__36828))
            {
                this._swipeTicker!.stop();
            }
        }
        this._swipeAnimationController.value = (1L - (this._swipeCurrentDistance / maxSwipeDistance__36089));
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildAlign(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget? child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Align(heightFactor: ((global::Doroti.Generated.Framework.Animation.ProxyAnimation)this._sizeAnimation).value, widthFactor: 1.0, alignment: global::Doroti.Generated.Framework.Painting.Alignment.topCenter, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints__38420 = default!;
        if ((((_MenuOverlay__menu_anchor)this.widget).constraints is not null))
        {
            constraints__38420 = ((_MenuOverlay__menu_anchor)this.widget).constraints!;
        }
        else
        {
            bool isLargeTextModeEnabled__38542 = Menu_anchorLibrary._largeTextModeEnabled(context);
            double screenWidth__38618 = MediaQuery.widthOf(context);
            var menuWidth__38673 = _CupertinoMenuWidth__menu_anchorMembers.CreateFromScreenWidth(isLargeTextModeEnabled: isLargeTextModeEnabled__38542, screenWidth: screenWidth__38618);
            constraints__38420 = global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateTightFor(width: ((_CupertinoMenuWidth__menu_anchor)menuWidth__38673).points());
        }
        global::Doroti.Generated.Framework.Widgets.Widget child__38909 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _SwipeSurface__menu_anchor(child: new global::Doroti.Generated.Framework.Widgets.TapRegion(groupId: ((_MenuOverlay__menu_anchor)this.widget).tapRegionGroupId, consumeOutsideTaps: ((_MenuOverlay__menu_anchor)this.widget).consumeOutsideTaps, onTapOutside: (global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerDownEvent>)this._handleOutsideTap, child: new global::Doroti.Generated.Framework.Widgets.Actions(actions: _kActions, child: new global::Doroti.Generated.Framework.Widgets.Shortcuts(shortcuts: Menu_anchorLibrary._kMenuTraversalShortcuts, child: new global::Doroti.Generated.Framework.Widgets.FocusScope(node: ((_MenuOverlay__menu_anchor)this.widget).focusScopeNode, descendantsAreFocusable: true, descendantsAreTraversable: true, canRequestFocus: true, child: new global::Doroti.Generated.Framework.Widgets.CustomPaint(painter: new _ShadowPainter__menu_anchor(brightness: (CupertinoTheme.maybeBrightnessOf(context) ?? Brightness.light), repaint: this._fadeAnimation), child: new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: this._fadeAnimation, alwaysIncludeSemantics: true, child: new CupertinoPopupSurface(child: new global::Doroti.Generated.Framework.Widgets.AnimatedBuilder(animation: this._sizeAnimation, builder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)this._buildAlign, child: new global::Doroti.Generated.Framework.Widgets.Semantics(explicitChildNodes: true, scopesRoute: true, child: new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: constraints__38420, child: new global::Doroti.Generated.Framework.Widgets.SingleChildScrollView(clipBehavior: Clip.none, child: new global::Doroti.Generated.Framework.Widgets.Column(mainAxisSize: global::Doroti.Generated.Framework.Rendering.MainAxisSize.min, children: this._children))))))))))))));
        if (!((_MenuOverlay__menu_anchor)this.widget).constrainCrossAxis)
        {
            child__38909 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.UnconstrainedBox(clipBehavior: Clip.hardEdge, alignment: global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerStart, constrainedAxis: global::Doroti.Generated.Framework.Painting.Axis.vertical, child: child__38909));
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateLoose(((_MenuOverlay__menu_anchor)this.widget).overlaySize), child: new global::Doroti.Generated.Framework.Widgets.ScaleTransition(scale: this._scaleAnimation, alignment: this._attachmentPointAlignment, child: new global::Doroti.Generated.Framework.Widgets.ValueListenableBuilder<double>(valueListenable: this._sizeAnimation, child: child__38909, builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, double, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)((context, value, child) => {
global::Doroti.Flutter.Ui.Rect effectiveAnchorRect__41900 = ((global::Doroti.Flutter.Ui.Rect)(object?)((((_MenuOverlay__menu_anchor)this.widget).anchorPosition is not null) ? (this._attachmentPoint & Size.zero) : ((_MenuOverlay__menu_anchor)this.widget).anchorRect));
List<global::Doroti.Flutter.Ui.DisplayFeature>? displayFeatures__42079 = ((List<global::Doroti.Flutter.Ui.DisplayFeature>?)(object?)MediaQuery.maybeDisplayFeaturesOf(context));
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.CustomSingleChildLayout(@delegate: new _MenuLayoutDelegate__menu_anchor(anchorRect: effectiveAnchorRect__41900, attachmentPoint: this._attachmentPoint, avoidBounds: ((displayFeatures__42079 is not null) ? _MenuOverlayState__menu_anchor.avoidBounds(displayFeatures__42079) : new HashSet<Rect>()), heightFactor: value, menuAlignment: this._menuAlignment, overlayPadding: ((_MenuOverlay__menu_anchor)this.widget).overlayPadding.resolve(this._textDirection)), child: child));
throw new InvalidOperationException("Dart closure completed without a value.");
}))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static HashSet<global::Doroti.Flutter.Ui.Rect> avoidBounds(List<global::Doroti.Flutter.Ui.DisplayFeature> displayFeatures)
    {
        var bounds__42810 = new HashSet<Rect>();
        foreach (var feature__42847 in displayFeatures)
        {
            if (((feature__42847.bounds.shortestSide > 0L) || (object.Equals(feature__42847.state, DisplayFeatureState.postureHalfOpened))))
            {
                bounds__42810.Add(feature__42847.bounds);
            }
        }
        return ((HashSet<global::Doroti.Flutter.Ui.Rect>)(object?)bounds__42810);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>();
        TickerModeData values__17506 = this._tickerModeNotifier!.value;
        var result__17553 = ((Func<global::Doroti.Generated.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{            var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
            __cascade.muted = !((TickerModeData)values__17506).enabled;
            __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
            return __cascade;        }))();
        this._tickers!.Add(result__17553);
        return ((global::Doroti.Generated.Framework.Scheduler.Ticker)(object?)result__17553);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Generated.Framework.Widgets._WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => (this._tickers is not null));
        DartRuntimePrimitives.Assert(() => this._tickers!.Contains(ticker));
        this._tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if ((this._tickers is not null))
        {
            TickerModeData values__18318 = this._tickerModeNotifier!.value;
            bool muted__18372 = !((TickerModeData)values__18318).enabled;
            foreach (global::Doroti.Generated.Framework.Scheduler.Ticker ticker__18421 in this._tickers!)
            {
                ticker__18421.muted = muted__18372;
                ticker__18421.forceFrames = ((TickerModeData)values__18318).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__18621 = ((global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__18621, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier__18621.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier__18621;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

internal class _ShadowPainter__menu_anchor : global::Doroti.Generated.Framework.Rendering.CustomPainter
{
    internal static Radius _kRadius = global::Doroti.Flutter.Ui.Radius.circular(13);
    internal const double _kShadowOpacity = 0.12;
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> repaint { get; private set; } = default!;
    public virtual Brightness brightness { get; private set; } = default!;

    internal _ShadowPainter__menu_anchor(Brightness brightness, global::Doroti.Generated.Framework.Animation.Animation<double> repaint) : base(repaint: repaint)
    {
        this.brightness = brightness;
        this.repaint = repaint;
    }

    public virtual double shadowAnimation => Dart_uiLibrary.clampDouble(((global::Doroti.Generated.Framework.Animation.Animation<double>)this.repaint).value, 0, 1);
    public override void paint(Canvas canvas, Size size)
    {
        DartRuntimePrimitives.Assert(() => ((this.shadowAnimation >= 0L) && (this.shadowAnimation <= 1L)));
        var center__43575 = new global::Doroti.Flutter.Ui.Offset((size.width / 2L), (size.height / 2L));
        var rect__43635 = global::Doroti.Flutter.Ui.Rect.fromCenter(center: center__43575, width: size.width, height: size.height);
        var roundedRect__43725 = global::Doroti.Flutter.Ui.RSuperellipse.fromRectAndRadius(rect__43635, _kRadius);
        double blurSigma__43806 = (this.shadowAnimation * 50L);
        var shadowPaint__43850 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Flutter.Ui.Paint();
            __cascade.maskFilter = global::Doroti.Flutter.Ui.MaskFilter.blur(BlurStyle.normal, blurSigma__43806);
            __cascade.color = global::Doroti.Flutter.Ui.Color.fromRGBO(0L, 0L, 10L, ((this.shadowAnimation * this.shadowAnimation) * _kShadowOpacity));
            return __cascade;        }))();
        var maskPath__44047 = ((Func<Path>)(() =>
{            var __cascade = new global::Doroti.Flutter.Ui.Path();
            __cascade.fillType = PathFillType.evenOdd;
            __cascade.addRect(rect__43635.inflate(200));
            __cascade.addRRect(global::Doroti.Flutter.Ui.RRect.fromRectAndRadius(rect__43635, _kRadius));
            return __cascade;        }))();
        DartRuntimePrimitives.Ignore(((Func<Canvas>)(() =>
{            var __cascade = canvas;
            __cascade.save();
            __cascade.clipPath(maskPath__44047);
            __cascade.drawRSuperellipse(roundedRect__43725.inflate(50), shadowPaint__43850);
            __cascade.restore();
            return __cascade;        }))());
    }

    public override bool shouldRepaint(global::Doroti.Generated.Framework.Rendering.CustomPainter oldDelegate)
    {
        var __oldDelegate = (_ShadowPainter__menu_anchor)(object)oldDelegate;
        return ((!object.Equals(((_ShadowPainter__menu_anchor)__oldDelegate).brightness, this.brightness)) || (!object.Equals(((_ShadowPainter__menu_anchor)__oldDelegate).repaint, this.repaint)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRebuildSemantics(global::Doroti.Generated.Framework.Rendering.CustomPainter oldDelegate) => false;
}

internal class _MenuLayoutDelegate__menu_anchor : global::Doroti.Generated.Framework.Rendering.SingleChildLayoutDelegate
{
    public virtual Rect anchorRect { get; private set; } = default!;
    public virtual Offset attachmentPoint { get; private set; } = default!;
    public virtual HashSet<Rect> avoidBounds { get; private set; } = default!;
    public virtual double heightFactor { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.Alignment menuAlignment { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsets overlayPadding { get; private set; } = default!;

    internal _MenuLayoutDelegate__menu_anchor(Rect anchorRect, Offset attachmentPoint, HashSet<Rect> avoidBounds, double heightFactor, global::Doroti.Generated.Framework.Painting.Alignment menuAlignment, global::Doroti.Generated.Framework.Painting.EdgeInsets overlayPadding)
    {
        this.anchorRect = anchorRect;
        this.attachmentPoint = attachmentPoint;
        this.avoidBounds = avoidBounds;
        this.heightFactor = heightFactor;
        this.menuAlignment = menuAlignment;
        this.overlayPadding = overlayPadding;
    }

    public override global::Doroti.Generated.Framework.Rendering.BoxConstraints getConstraintsForChild(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints)
    {
        return ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)(object?)global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateLoose(((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).biggest).deflate(this.overlayPadding));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Offset getPositionForChild(Size size, Size childSize)
    {
        double inverseHeightFactor__46023 = ((this.heightFactor > 0.01) ? (1L / this.heightFactor) : 0);
        double finalHeight__46249 = Math.Min((childSize.height * inverseHeightFactor__46023), size.height);
        var finalSize__46336 = new global::Doroti.Flutter.Ui.Size(childSize.width, finalHeight__46249);
        global::Doroti.Flutter.Ui.Offset desiredPosition__46404 = ((global::Doroti.Flutter.Ui.Offset)(object?)(this.attachmentPoint - this.menuAlignment.alongSize(finalSize__46336)));
        global::Doroti.Flutter.Ui.Rect screen__46494 = ((global::Doroti.Flutter.Ui.Rect)(object?)_findClosestScreen(size, ((Offset)((dynamic)this.anchorRect).center), this.avoidBounds));
        global::Doroti.Flutter.Ui.Offset finalPosition__46581 = ((global::Doroti.Flutter.Ui.Offset)(object?)_positionChild(screen__46494, finalSize__46336, desiredPosition__46404, this.anchorRect));
        bool growsUp__46833 = ((finalPosition__46581.dy + finalSize__46336.height) <= ((Offset)((dynamic)this.anchorRect).center).dy);
        if (growsUp__46833)
        {
            double dy__46942 = (finalHeight__46249 - childSize.height);
            return new global::Doroti.Flutter.Ui.Offset(finalPosition__46581.dx, (finalPosition__46581.dy + dy__46942));
        }
        var initialPosition__47058 = new global::Doroti.Flutter.Ui.Offset(finalPosition__46581.dx, this.anchorRect.bottom);
        return DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Offset.lerp(initialPosition__47058, finalPosition__46581, this.heightFactor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Flutter.Ui.Offset _positionChild(Rect screen, Size childSize, Offset position, Rect anchor)
    {
        double x__47296 = position.dx;
        double y__47324 = position.dy;
        bool overLeftEdge(double x)
        {
            return (x < (screen.left + ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.overlayPadding).left));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        bool overRightEdge(double x)
        {
            return (x > ((screen.right - childSize.width) - ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.overlayPadding).right));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        bool overTopEdge(double y)
        {
            return (y < (screen.top + ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.overlayPadding).top));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        bool overBottomEdge(double y)
        {
            return (y > ((screen.bottom - childSize.height) - ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.overlayPadding).bottom));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        bool hasHorizontalAnchorOverlap__47820 = (childSize.width >= screen.width);
        if (hasHorizontalAnchorOverlap__47820)
        {
            x__47296 = (screen.left + ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.overlayPadding).left);
        }
        else
        {
            if (overLeftEdge(x__47296))
            {
                double flipX__48163 = (((((Offset)((dynamic)anchor).center).dx * 2L) - position.dx) - childSize.width);
                hasHorizontalAnchorOverlap__47820 = overRightEdge(flipX__48163);
                if ((hasHorizontalAnchorOverlap__47820 || overLeftEdge(flipX__48163)))
                {
                    x__47296 = (screen.left + ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.overlayPadding).left);
                }
                else
                {
                    x__47296 = flipX__48163;
                }
            }
            else
            {
                if (overRightEdge(x__47296))
                {
                    double flipX__48638 = (((((Offset)((dynamic)anchor).center).dx * 2L) - position.dx) - childSize.width);
                    hasHorizontalAnchorOverlap__47820 = overLeftEdge(flipX__48638);
                    if ((hasHorizontalAnchorOverlap__47820 || overRightEdge(flipX__48638)))
                    {
                        x__47296 = ((screen.right - childSize.width) - ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.overlayPadding).right);
                    }
                    else
                    {
                        x__47296 = flipX__48638;
                    }
                }
            }
        }
        if ((childSize.height >= screen.height))
        {
            return new global::Doroti.Flutter.Ui.Offset(x__47296, (screen.top + ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.overlayPadding).top));
        }
        if ((hasHorizontalAnchorOverlap__47820 && !anchor.isEmpty))
        {
            double below__49701 = (anchor.bottom - y__47324);
            double above__49747 = ((y__47324 + childSize.height) - anchor.top);
            if (((below__49701 > 0L) && (above__49747 > 0L)))
            {
                if ((below__49701 > above__49747))
                {
                    y__47324 = (anchor.top - childSize.height);
                }
                else
                {
                    y__47324 = anchor.bottom;
                }
            }
        }
        if (overTopEdge(y__47324))
        {
            double flipY__50135 = (((((Offset)((dynamic)anchor).center).dy * 2L) - position.dy) - childSize.height);
            if ((overTopEdge(flipY__50135) || overBottomEdge(flipY__50135)))
            {
                y__47324 = (screen.top + ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.overlayPadding).top);
            }
            else
            {
                y__47324 = flipY__50135;
            }
        }
        else
        {
            if (overBottomEdge(y__47324))
            {
                double flipY__50516 = (((((Offset)((dynamic)anchor).center).dy * 2L) - position.dy) - childSize.height);
                if ((overTopEdge(flipY__50516) || overBottomEdge(flipY__50516)))
                {
                    y__47324 = ((screen.bottom - childSize.height) - ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.overlayPadding).bottom);
                }
                else
                {
                    y__47324 = flipY__50516;
                }
            }
        }
        return new global::Doroti.Flutter.Ui.Offset(x__47296, y__47324);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Flutter.Ui.Rect _findClosestScreen(Size parentSize, Offset point, HashSet<Rect> avoidBounds)
    {
        IEnumerable<global::Doroti.Flutter.Ui.Rect> screens__51135 = ((IEnumerable<global::Doroti.Flutter.Ui.Rect>)(object?)DisplayFeatureSubScreen.subScreensInBounds((Offset.zero & parentSize), avoidBounds));
        global::Doroti.Flutter.Ui.Rect? closest__51258 = default!;
        double closestSquaredDistance__51278 = 0;
        foreach (var screen__51321 in screens__51135)
        {
            if (screen__51321.contains(point))
            {
                return screen__51321;
            }
            if ((closest__51258 is null))
            {
                closest__51258 = screen__51321;
                closestSquaredDistance__51278 = Menu_anchorLibrary._computeSquaredDistanceToRect(point, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(closest__51258)));
                continue;
            }
            double squaredDistance__51591 = Menu_anchorLibrary._computeSquaredDistanceToRect(point, screen__51321);
            if ((squaredDistance__51591 < closestSquaredDistance__51278))
            {
                closest__51258 = screen__51321;
                closestSquaredDistance__51278 = squaredDistance__51591;
            }
        }
        return DartRuntimePrimitives.RequireValue(closest__51258);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRelayout(global::Doroti.Generated.Framework.Rendering.SingleChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (_MenuLayoutDelegate__menu_anchor)(object)oldDelegate;
        return ((((((!object.Equals(this.anchorRect, ((_MenuLayoutDelegate__menu_anchor)__oldDelegate).anchorRect)) || (!object.Equals(this.attachmentPoint, ((_MenuLayoutDelegate__menu_anchor)__oldDelegate).attachmentPoint))) || !global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.setEquals(this.avoidBounds, ((_MenuLayoutDelegate__menu_anchor)__oldDelegate).avoidBounds)) || (this.heightFactor != ((_MenuLayoutDelegate__menu_anchor)__oldDelegate).heightFactor)) || (!object.Equals(this.menuAlignment, ((_MenuLayoutDelegate__menu_anchor)__oldDelegate).menuAlignment))) || (!object.Equals(this.overlayPadding, ((_MenuLayoutDelegate__menu_anchor)__oldDelegate).overlayPadding)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _FocusUpIntent__menu_anchor : global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent
{
    internal _FocusUpIntent__menu_anchor() : base(global::Doroti.Generated.Framework.Widgets.TraversalDirection.up)
    {
    }

}

internal class _FocusDownIntent__menu_anchor : global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent
{
    internal _FocusDownIntent__menu_anchor() : base(global::Doroti.Generated.Framework.Widgets.TraversalDirection.down)
    {
    }

}

internal class _FocusUpAction__menu_anchor : global::Doroti.Generated.Framework.Widgets.ContextAction<global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent>
{
    internal _FocusUpAction__menu_anchor()
    {
    }

    public override object? invoke(global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent intent, global::Doroti.Generated.Framework.Widgets.BuildContext? context = null)
    {
        global::Doroti.Generated.Framework.Widgets.FocusTraversalPolicy policy__52670 = (FocusTraversalGroup.maybeOf(context!) ?? new global::Doroti.Generated.Framework.Widgets.ReadingOrderTraversalPolicy());
        if ((Menu_anchorLibrary._isCupertino && !global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb))
        {
            policy__52670.inDirection(global::Doroti.Generated.Framework.Widgets.Focus_managerLibrary.primaryFocus!, ((global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent)intent).direction);
            return default!;
        }
        global::Doroti.Generated.Framework.Widgets.FocusNode? firstFocus__52932 = ((global::Doroti.Generated.Framework.Widgets.FocusNode?)(object?)policy__52670.findFirstFocus(global::Doroti.Generated.Framework.Widgets.Focus_managerLibrary.primaryFocus!, ignoreCurrentFocus: true));
        global::Doroti.Generated.Framework.Widgets.FocusNode lastFocus__53029 = ((global::Doroti.Generated.Framework.Widgets.FocusNode)(object?)policy__52670.findLastFocus(global::Doroti.Generated.Framework.Widgets.Focus_managerLibrary.primaryFocus!, ignoreCurrentFocus: true));
        if ((((global::Doroti.Generated.Framework.Widgets.FocusNode)lastFocus__53029).context is not null))
        {
            if (((object.Equals(global::Doroti.Generated.Framework.Widgets.Focus_managerLibrary.primaryFocus, ((global::Doroti.Generated.Framework.Widgets.FocusNode)lastFocus__53029).enclosingScope)) || (object.Equals(global::Doroti.Generated.Framework.Widgets.Focus_managerLibrary.primaryFocus, firstFocus__52932))))
            {
                policy__52670.requestFocusCallback(lastFocus__53029);
                return default!;
            }
        }
        policy__52670.inDirection(global::Doroti.Generated.Framework.Widgets.Focus_managerLibrary.primaryFocus!, ((global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent)intent).direction);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _FocusDownAction__menu_anchor : global::Doroti.Generated.Framework.Widgets.ContextAction<global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent>
{
    internal _FocusDownAction__menu_anchor()
    {
    }

    public override object? invoke(global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent intent, global::Doroti.Generated.Framework.Widgets.BuildContext? context = null)
    {
        global::Doroti.Generated.Framework.Widgets.FocusTraversalPolicy policy__53577 = (FocusTraversalGroup.maybeOf(context!) ?? new global::Doroti.Generated.Framework.Widgets.ReadingOrderTraversalPolicy());
        if ((Menu_anchorLibrary._isCupertino && !global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb))
        {
            policy__53577.inDirection(global::Doroti.Generated.Framework.Widgets.Focus_managerLibrary.primaryFocus!, ((global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent)intent).direction);
            return default!;
        }
        global::Doroti.Generated.Framework.Widgets.FocusNode? firstFocus__53839 = ((global::Doroti.Generated.Framework.Widgets.FocusNode?)(object?)policy__53577.findFirstFocus(global::Doroti.Generated.Framework.Widgets.Focus_managerLibrary.primaryFocus!, ignoreCurrentFocus: true));
        global::Doroti.Generated.Framework.Widgets.FocusNode lastFocus__53936 = ((global::Doroti.Generated.Framework.Widgets.FocusNode)(object?)policy__53577.findLastFocus(global::Doroti.Generated.Framework.Widgets.Focus_managerLibrary.primaryFocus!, ignoreCurrentFocus: true));
        if ((firstFocus__53839?.context is not null))
        {
            if (((object.Equals(global::Doroti.Generated.Framework.Widgets.Focus_managerLibrary.primaryFocus, firstFocus__53839!.enclosingScope)) || (object.Equals(global::Doroti.Generated.Framework.Widgets.Focus_managerLibrary.primaryFocus, lastFocus__53936))))
            {
                policy__53577.requestFocusCallback(firstFocus__53839);
                return default!;
            }
        }
        policy__53577.inDirection(global::Doroti.Generated.Framework.Widgets.Focus_managerLibrary.primaryFocus!, ((global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent)intent).direction);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _FocusFirstIntent__menu_anchor : global::Doroti.Generated.Framework.Widgets.Intent
{
    internal _FocusFirstIntent__menu_anchor()
    {
    }

}

internal class _FocusFirstAction__menu_anchor : global::Doroti.Generated.Framework.Widgets.ContextAction<_FocusFirstIntent__menu_anchor>
{
    internal _FocusFirstAction__menu_anchor()
    {
    }

    public override object? invoke(_FocusFirstIntent__menu_anchor intent, global::Doroti.Generated.Framework.Widgets.BuildContext? context = null)
    {
        global::Doroti.Generated.Framework.Widgets.FocusTraversalPolicy policy__54553 = (FocusTraversalGroup.maybeOf(context!) ?? new global::Doroti.Generated.Framework.Widgets.ReadingOrderTraversalPolicy());
        global::Doroti.Generated.Framework.Widgets.FocusNode? firstFocus__54663 = ((global::Doroti.Generated.Framework.Widgets.FocusNode?)(object?)policy__54553.findFirstFocus(global::Doroti.Generated.Framework.Widgets.Focus_managerLibrary.primaryFocus!, ignoreCurrentFocus: true));
        if (((firstFocus__54663 is null) || (((global::Doroti.Generated.Framework.Widgets.FocusNode)firstFocus__54663).context is null)))
        {
            return default!;
        }
        policy__54553.requestFocusCallback(firstFocus__54663);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _FocusLastIntent__menu_anchor : global::Doroti.Generated.Framework.Widgets.Intent
{
    internal _FocusLastIntent__menu_anchor()
    {
    }

}

internal class _FocusLastAction__menu_anchor : global::Doroti.Generated.Framework.Widgets.ContextAction<_FocusLastIntent__menu_anchor>
{
    internal _FocusLastAction__menu_anchor()
    {
    }

    public override object? invoke(_FocusLastIntent__menu_anchor intent, global::Doroti.Generated.Framework.Widgets.BuildContext? context = null)
    {
        global::Doroti.Generated.Framework.Widgets.FocusTraversalPolicy policy__55140 = (FocusTraversalGroup.maybeOf(context!) ?? new global::Doroti.Generated.Framework.Widgets.ReadingOrderTraversalPolicy());
        global::Doroti.Generated.Framework.Widgets.FocusNode lastFocus__55249 = ((global::Doroti.Generated.Framework.Widgets.FocusNode)(object?)policy__55140.findLastFocus(global::Doroti.Generated.Framework.Widgets.Focus_managerLibrary.primaryFocus!, ignoreCurrentFocus: true));
        if ((((global::Doroti.Generated.Framework.Widgets.FocusNode)lastFocus__55249).context is null))
        {
            return default!;
        }
        policy__55140.requestFocusCallback(lastFocus__55249);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CupertinoMenuImplicitDivider__menu_anchor : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public static CupertinoDynamicColor kOverlayColor = new CupertinoDynamicColor(color: global::Doroti.Flutter.Ui.Color.fromRGBO(140L, 140L, 140L, 0.3), darkColor: global::Doroti.Flutter.Ui.Color.fromRGBO(255L, 255L, 255L, 0.25));
    public static CupertinoDynamicColor kDividerColor = new CupertinoDynamicColor(color: global::Doroti.Flutter.Ui.Color.fromRGBO(0L, 0L, 0L, 0.25), darkColor: global::Doroti.Flutter.Ui.Color.fromRGBO(255L, 255L, 255L, 0.25));

    internal _CupertinoMenuImplicitDivider__menu_anchor()
    {
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        double pixelRatio__57313 = (MediaQuery.maybeDevicePixelRatioOf(context) ?? 1.0);
        double displacement__57395 = (1L / pixelRatio__57313);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.CustomPaint(size: new global::Doroti.Flutter.Ui.Size(double.PositiveInfinity, displacement__57395), painter: new _CupertinoDividerPainter__menu_anchor(color: CupertinoDynamicColor.resolve(kDividerColor, context), overlayColor: CupertinoDynamicColor.resolve(kOverlayColor, context), antiAlias: (pixelRatio__57313 < 1.0))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CupertinoMenuDivider : global::Doroti.Generated.Framework.Widgets.StatelessWidget, CupertinoMenuEntry
{
    public virtual Color color { get; private set; } = default!;
    public static CupertinoDynamicColor kDefaultColor = new CupertinoDynamicColor(color: global::Doroti.Flutter.Ui.Color.fromRGBO(0L, 0L, 0L, 0.08), darkColor: global::Doroti.Flutter.Ui.Color.fromRGBO(0L, 0L, 0L, 0.16));
    internal const double _kDividerHeight = 8.0;

    public CupertinoMenuDivider(global::Doroti.Generated.Framework.Foundation.Key? key = null, Color color = default!) : base(key: key)
    {
        Color __color = color ?? kDefaultColor;
        this.color = __color;
    }

    public virtual bool isDivider => true;
    public virtual bool hasLeading(global::Doroti.Generated.Framework.Widgets.BuildContext context) => false;
    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.ColoredBox(color: CupertinoDynamicColor.resolve(this.color, context), child: new global::Doroti.Generated.Framework.Widgets.SizedBox(height: _kDividerHeight, width: double.PositiveInfinity)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CupertinoDividerPainter__menu_anchor : global::Doroti.Generated.Framework.Rendering.CustomPainter
{
    public virtual Color color { get; private set; } = default!;
    public virtual Color overlayColor { get; private set; } = default!;
    public virtual bool antiAlias { get; private set; } = default!;

    internal _CupertinoDividerPainter__menu_anchor(Color color, Color overlayColor, bool antiAlias = false)
    {
        this.color = color;
        this.overlayColor = overlayColor;
        this.antiAlias = antiAlias;
    }

    public override void paint(Canvas canvas, Size size)
    {
        global::Doroti.Flutter.Ui.Offset p1__59816 = ((global::Doroti.Flutter.Ui.Offset)(object?)size.centerLeft(Offset.zero));
        global::Doroti.Flutter.Ui.Offset p2__59868 = ((global::Doroti.Flutter.Ui.Offset)(object?)size.centerRight(Offset.zero));
        if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb)
        {
            var overlayPainter__59990 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Flutter.Ui.Paint();
            __cascade.style = PaintingStyle.stroke;
            __cascade.color = this.overlayColor;
            __cascade.isAntiAlias = this.antiAlias;
            __cascade.blendMode = BlendMode.overlay;
            return __cascade;        }))();
            canvas.drawLine(p1__59816, p2__59868, overlayPainter__59990);
        }
        var colorPainter__60224 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Flutter.Ui.Paint();
            __cascade.style = PaintingStyle.stroke;
            __cascade.color = this.color;
            __cascade.isAntiAlias = this.antiAlias;
            return __cascade;        }))();
        canvas.drawLine(p1__59816, p2__59868, colorPainter__60224);
    }

    public override bool shouldRepaint(global::Doroti.Generated.Framework.Rendering.CustomPainter oldDelegate)
    {
        var __oldDelegate = (_CupertinoDividerPainter__menu_anchor)(object)oldDelegate;
        return (((!object.Equals(this.color, ((_CupertinoDividerPainter__menu_anchor)__oldDelegate).color)) || (!object.Equals(this.overlayColor, ((_CupertinoDividerPainter__menu_anchor)__oldDelegate).overlayColor))) || (this.antiAlias != ((_CupertinoDividerPainter__menu_anchor)__oldDelegate).antiAlias));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CupertinoMenuItem : global::Doroti.Generated.Framework.Widgets.StatelessWidget, CupertinoMenuEntry
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? leading { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? trailing { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? subtitle { get; private set; }
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual global::System.Action<bool>? onHover { get; private set; }
    public virtual global::System.Action<bool>? onFocusChange { get; private set; }
    public virtual bool requestFocusOnHover { get; private set; } = default!;
    public virtual bool autofocus { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BoxDecoration>? decoration { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor>? mouseCursor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.HitTestBehavior behavior { get; private set; } = default!;
    public virtual bool requestCloseOnActivate { get; private set; } = default!;
    public virtual bool isDestructiveAction { get; private set; } = default!;
    public virtual double? leadingWidth { get; private set; }
    public virtual double? trailingWidth { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry? leadingMidpointAlignment { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry? trailingMidpointAlignment { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints { get; private set; }
    public static global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BoxDecoration> kDefaultDecoration = global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BoxDecoration>.CreateFromMap(new DartMap<global::Doroti.Generated.Framework.Widgets.WidgetStatesConstraint, global::Doroti.Generated.Framework.Painting.BoxDecoration> { [global::Doroti.Generated.Framework.Widgets.WidgetState.dragged.asConstraint()] = new global::Doroti.Generated.Framework.Painting.BoxDecoration(color: new CupertinoDynamicColor(color: global::Doroti.Flutter.Ui.Color.fromRGBO(50L, 50L, 50L, 0.1), darkColor: global::Doroti.Flutter.Ui.Color.fromRGBO(255L, 255L, 255L, 0.1))), [global::Doroti.Generated.Framework.Widgets.WidgetState.pressed.asConstraint()] = new global::Doroti.Generated.Framework.Painting.BoxDecoration(color: new CupertinoDynamicColor(color: global::Doroti.Flutter.Ui.Color.fromRGBO(50L, 50L, 50L, 0.1), darkColor: global::Doroti.Flutter.Ui.Color.fromRGBO(255L, 255L, 255L, 0.1))), [global::Doroti.Generated.Framework.Widgets.WidgetState.focused.asConstraint()] = new global::Doroti.Generated.Framework.Painting.BoxDecoration(color: new CupertinoDynamicColor(color: global::Doroti.Flutter.Ui.Color.fromRGBO(50L, 50L, 50L, 0.075), darkColor: global::Doroti.Flutter.Ui.Color.fromRGBO(255L, 255L, 255L, 0.075))), [global::Doroti.Generated.Framework.Widgets.WidgetState.hovered.asConstraint()] = new global::Doroti.Generated.Framework.Painting.BoxDecoration(color: new CupertinoDynamicColor(color: global::Doroti.Flutter.Ui.Color.fromRGBO(50L, 50L, 50L, 0.05), darkColor: global::Doroti.Flutter.Ui.Color.fromRGBO(255L, 255L, 255L, 0.05))), [global::Doroti.Generated.Framework.Widgets.WidgetStateMembers.any] = new global::Doroti.Generated.Framework.Painting.BoxDecoration() });
    internal static global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor> _kDefaultCursor = WidgetStateProperty.resolveWith<global::Doroti.Generated.Framework.Services.MouseCursor>(((global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, global::Doroti.Generated.Framework.Services.MouseCursor>)((states) => {
return ((global::Doroti.Generated.Framework.Services.MouseCursor)(object?)((!states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled) && global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb) ? global::Doroti.Generated.Framework.Services.SystemMouseCursors.click : global::Doroti.Generated.Framework.Services.MouseCursor.defer));
throw new InvalidOperationException("Dart closure completed without a value.");
})));
    internal static Color _kDefaultTextColor = ((Color)(object?)new CupertinoDynamicColor(color: global::Doroti.Flutter.Ui.Color.from(alpha: 0.96, red: 0, green: 0, blue: 0), darkColor: global::Doroti.Flutter.Ui.Color.from(alpha: 0.96, red: 1, green: 1, blue: 1)));
    internal static Color _kDefaultSubtitleTextColor = ((Color)(object?)new CupertinoDynamicColor(color: global::Doroti.Flutter.Ui.Color.from(alpha: 0.55, red: 0, green: 0, blue: 0), darkColor: global::Doroti.Flutter.Ui.Color.from(alpha: 0.4, red: 1, green: 1, blue: 1)));
    internal const long _kDefaultMaxLines = 2L;
    internal const long _kDefaultLargeTextModeMaxLines = 100L;
    internal static global::Doroti.Generated.Framework.Painting.TextStyle _kLeadingDefaultTextStyle = new global::Doroti.Generated.Framework.Painting.TextStyle(fontSize: 15, fontWeight: FontWeight.w600);
    internal static global::Doroti.Generated.Framework.Widgets.IconThemeData _kLeadingDefaultIconTheme = new global::Doroti.Generated.Framework.Widgets.IconThemeData(size: 15, weight: 600, applyTextScaling: true);
    internal static global::Doroti.Generated.Framework.Painting.TextStyle _kTrailingDefaultTextStyle = new global::Doroti.Generated.Framework.Painting.TextStyle(fontSize: 21);
    internal static global::Doroti.Generated.Framework.Widgets.IconThemeData _kTrailingDefaultIconTheme = new global::Doroti.Generated.Framework.Widgets.IconThemeData(size: 21, applyTextScaling: true);

    public CupertinoMenuItem(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget child = default!, global::Doroti.Generated.Framework.Widgets.Widget? subtitle = null, global::Doroti.Generated.Framework.Widgets.Widget? leading = null, double? leadingWidth = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? leadingMidpointAlignment = null, global::Doroti.Generated.Framework.Widgets.Widget? trailing = null, double? trailingWidth = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? trailingMidpointAlignment = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, bool autofocus = false, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, global::System.Action<bool>? onFocusChange = null, global::System.Action<bool>? onHover = null, global::System.Action? onPressed = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BoxDecoration>? decoration = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor>? mouseCursor = null, global::Doroti.Generated.Framework.Rendering.HitTestBehavior behavior = global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque, bool requestCloseOnActivate = true, bool requestFocusOnHover = true, bool isDestructiveAction = false) : base(key: key)
    {
        this.child = child;
        this.subtitle = subtitle;
        this.leading = leading;
        this.leadingWidth = leadingWidth;
        this.leadingMidpointAlignment = leadingMidpointAlignment;
        this.trailing = trailing;
        this.trailingWidth = trailingWidth;
        this.trailingMidpointAlignment = trailingMidpointAlignment;
        this.padding = padding;
        this.constraints = constraints;
        this.autofocus = autofocus;
        this.focusNode = focusNode;
        this.onFocusChange = onFocusChange;
        this.onHover = onHover;
        this.onPressed = onPressed;
        this.decoration = decoration;
        this.mouseCursor = mouseCursor;
        this.behavior = behavior;
        this.requestCloseOnActivate = requestCloseOnActivate;
        this.requestFocusOnHover = requestFocusOnHover;
        this.isDestructiveAction = isDestructiveAction;
    }

    public virtual bool hasLeading(global::Doroti.Generated.Framework.Widgets.BuildContext context) => DartRuntimePrimitives.ConvertValue<bool>((this.leading is not null));
    public virtual bool isDivider => false;
    internal virtual global::Doroti.Generated.Framework.Painting.TextStyle _resolveDefaultTextStyle(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Painting.TextScaler textScaler)
    {
        global::Doroti.Flutter.Ui.Color color__71906 = default!;
        if ((this.onPressed is null))
        {
            color__71906 = DartRuntimePrimitives.ConvertValue<Color>(CupertinoColors.systemGrey);
        }
        else
        {
            if (this.isDestructiveAction)
            {
                color__71906 = DartRuntimePrimitives.ConvertValue<Color>(CupertinoColors.systemRed);
            }
            else
            {
                color__71906 = _kDefaultTextColor;
            }
        }
        return ((global::Doroti.Generated.Framework.Painting.TextStyle)(object?)_DynamicTypeStyle__menu_anchor.body.resolveTextStyle(textScaler).copyWith(fontSize: 17, color: CupertinoDynamicColor.resolve(color__71906, context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Painting.TextStyle _resolveDefaultSubtitleStyle(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Painting.TextScaler textScaler)
    {
        var isDark__72463 = (object.Equals(CupertinoTheme.maybeBrightnessOf(context), Brightness.dark));
        return ((global::Doroti.Generated.Framework.Painting.TextStyle)(object?)_DynamicTypeStyle__menu_anchor.subhead.resolveTextStyle(textScaler).copyWith(fontSize: 15, textBaseline: TextBaseline.alphabetic, foreground: ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Flutter.Ui.Paint();
            __cascade.blendMode = (isDark__72463 ? BlendMode.plus : BlendMode.hardLight);
            __cascade.color = CupertinoDynamicColor.resolve(_kDefaultSubtitleTextColor, context);
            return __cascade;        }))()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleSelect(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        if (this.requestCloseOnActivate)
        {
            MenuController.maybeOf(context)?.close();
        }
        this.onPressed?.Invoke();
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Generated.Framework.Painting.TextScaler textScaler__73512 = (MediaQuery.maybeTextScalerOf(context) ?? global::Doroti.Generated.Framework.Painting.TextScaler.CreateLinear((MediaQuery.maybeTextScaleFactorOf(context) ?? 1)));
        global::Doroti.Generated.Framework.Painting.TextStyle defaultTextStyle__73670 = ((global::Doroti.Generated.Framework.Painting.TextStyle)(object?)_resolveDefaultTextStyle(context, textScaler__73512));
        bool isLargeTextModeEnabled__73751 = Menu_anchorLibrary._largeTextModeEnabled(context);
        global::Doroti.Generated.Framework.Widgets.Widget? leadingWidget__73820 = default!;
        global::Doroti.Generated.Framework.Widgets.Widget? trailingWidget__73847 = default!;
        if ((this.leading is not null))
        {
            leadingWidget__73820 = DefaultTextStyle.merge(style: _kLeadingDefaultTextStyle, child: IconTheme.merge(data: _kLeadingDefaultIconTheme, child: this.leading!));
        }
        if (((this.trailing is not null) && !isLargeTextModeEnabled__73751))
        {
            trailingWidget__73847 = DefaultTextStyle.merge(style: _kTrailingDefaultTextStyle, child: IconTheme.merge(data: _kTrailingDefaultIconTheme, child: this.trailing!));
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)MediaQuery.withClampedTextScaling(minScaleFactor: Menu_anchorLibrary._kMinimumTextScaleFactor, maxScaleFactor: Menu_anchorLibrary._kMaximumTextScaleFactor, child: new _CupertinoMenuItemInteractionHandler__menu_anchor(mouseCursor: (this.mouseCursor ?? _kDefaultCursor), requestFocusOnHover: this.requestFocusOnHover, onPressed: ((global::System.Action)((this.onPressed is not null) ? (() => { _handleSelect(context); }) : null)), onHover: (global::System.Action<bool>?)this.onHover, onFocusChange: (global::System.Action<bool>?)this.onFocusChange, autofocus: this.autofocus, focusNode: this.focusNode, decoration: (this.decoration ?? kDefaultDecoration), behavior: this.behavior, child: DefaultTextStyle.merge(maxLines: (isLargeTextModeEnabled__73751 ? _kDefaultLargeTextModeMaxLines : _kDefaultMaxLines), overflow: global::Doroti.Generated.Framework.Painting.TextOverflow.ellipsis, softWrap: true, style: new global::Doroti.Generated.Framework.Painting.TextStyle(color: ((global::Doroti.Generated.Framework.Painting.TextStyle)defaultTextStyle__73670).color), child: IconTheme.merge(data: new global::Doroti.Generated.Framework.Widgets.IconThemeData(color: ((global::Doroti.Generated.Framework.Painting.TextStyle)defaultTextStyle__73670).color), child: new _CupertinoMenuItemLabel__menu_anchor(padding: this.padding, constraints: this.constraints, trailing: trailingWidget__73847, leading: leadingWidget__73820, leadingMidpointAlignment: this.leadingMidpointAlignment, trailingMidpointAlignment: this.trailingMidpointAlignment, leadingWidth: this.leadingWidth, trailingWidth: this.trailingWidth, subtitle: ((this.subtitle is not null) ? DefaultTextStyle.merge(style: _resolveDefaultSubtitleStyle(context, textScaler__73512), child: this.subtitle!) : null), child: DefaultTextStyle.merge(style: defaultTextStyle__73670, child: this.child)))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.Widget?>("child", this.child));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("requestCloseOnActivate", value: this.requestCloseOnActivate, ifTrue: "closes on press", ifFalse: "does not close on press", defaultValue: true));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("requestFocusOnHover", value: this.requestFocusOnHover, ifFalse: "does not request focus on hover", ifTrue: "requests focus on hover", defaultValue: true));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Rendering.HitTestBehavior>("hitTestBehavior", this.behavior));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.FocusNode?>("focusNode", this.focusNode, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("enabled", value: (this.onPressed is not null), ifFalse: "DISABLED"));
        if ((this.subtitle is not null))
        {
            properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.Widget?>("subtitle", this.subtitle));
        }
        if ((this.leading is not null))
        {
            properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.Widget?>("leading", this.leading));
        }
        if ((this.trailing is not null))
        {
            properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.Widget?>("trailing", this.trailing));
        }
    }

}

internal class _CupertinoMenuItemLabel__menu_anchor : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    internal const double _kDefaultHorizontalWidth = 16;
    internal static double _kLeadingWidthSlope = (-311L / 1000L);
    internal const double _kLeadingWidthYIntercept = 10;
    internal static double _kLeadingMidpointSlope = (118L / 1000000L);
    internal static double _kLeadingMidpointYIntercept = (73L / 125L);
    internal static double _kTrailingWidthSlope = (1L / 10L);
    internal const double _kTrailingWidthYIntercept = 22;
    internal static double _kFirstBaselineToTopSlope = (14L / 11L);
    internal static double _kLastBaselineToBottomSlope = (71L / 100L);
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? leading { get; private set; }
    public virtual double? leadingWidth { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry? _leadingAlignment { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? trailing { get; private set; }
    public virtual double? trailingWidth { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry? _trailingAlignment { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? subtitle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Rendering.BoxConstraints? _constraints { get; private set; }

    internal _CupertinoMenuItemLabel__menu_anchor(global::Doroti.Generated.Framework.Widgets.Widget child, global::Doroti.Generated.Framework.Widgets.Widget? subtitle = null, global::Doroti.Generated.Framework.Widgets.Widget? leading = null, double? leadingWidth = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? leadingMidpointAlignment = null, global::Doroti.Generated.Framework.Widgets.Widget? trailing = null, double? trailingWidth = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? trailingMidpointAlignment = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null)
    {
        this.child = child;
        this.subtitle = subtitle;
        this.leading = leading;
        this.leadingWidth = leadingWidth;
        this.trailing = trailing;
        this.trailingWidth = trailingWidth;
        this.padding = padding;
        this._leadingAlignment = leadingMidpointAlignment;
        this._trailingAlignment = trailingMidpointAlignment;
        this._constraints = constraints;
    }

    internal virtual double _resolveLeadingWidth(global::Doroti.Generated.Framework.Painting.TextScaler textScaler, double pixelRatio, double lineHeight)
    {
        double units__79191 = Menu_anchorLibrary._normalizeTextScale(textScaler);
        double value__79249 = ((_kLeadingWidthSlope * units__79191) + _kLeadingWidthYIntercept);
        return Menu_anchorLibrary._roundToDivisible((value__79249 + lineHeight), to: (1L / pixelRatio));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _resolveTrailingWidth(global::Doroti.Generated.Framework.Painting.TextScaler textScaler, double pixelRatio, double lineHeight)
    {
        double units__79499 = Menu_anchorLibrary._normalizeTextScale(textScaler);
        double value__79557 = ((_kTrailingWidthSlope * units__79499) + _kTrailingWidthYIntercept);
        return Menu_anchorLibrary._roundToDivisible((value__79557 + lineHeight), to: (1L / pixelRatio));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry _resolveTrailingAlignment(double trailingWidth)
    {
        double horizontalOffset__79785 = ((DartRuntimePrimitives.RequireValue(trailingWidth) / 2L) + 6L);
        double horizontalRatio__79844 = (((DartRuntimePrimitives.RequireValue(trailingWidth) - horizontalOffset__79785)) / DartRuntimePrimitives.RequireValue(trailingWidth));
        double horizontalAlignment__79931 = (((horizontalRatio__79844 * 2L)) - 1L);
        return ((global::Doroti.Generated.Framework.Painting.AlignmentGeometry)(object?)new global::Doroti.Generated.Framework.Painting.AlignmentDirectional(horizontalAlignment__79931, 0.0));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry _resolveLeadingAlignment(double leadingWidth, global::Doroti.Generated.Framework.Painting.TextScaler textScaler)
    {
        double units__80152 = Menu_anchorLibrary._normalizeTextScale(textScaler);
        double horizontalRatio__80210 = ((_kLeadingMidpointSlope * units__80152) + _kLeadingMidpointYIntercept);
        double horizontalAlignment__80307 = (((horizontalRatio__80210 * 2L)) - 1L);
        return ((global::Doroti.Generated.Framework.Painting.AlignmentGeometry)(object?)new global::Doroti.Generated.Framework.Painting.AlignmentDirectional(horizontalAlignment__80307, 0.0));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _resolveFirstBaselineToTop(double lineHeight, double pixelRatio)
    {
        return Menu_anchorLibrary._roundToDivisible((lineHeight * _kFirstBaselineToTopSlope), to: (1L / pixelRatio));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _resolveLastBaselineToBottom(double lineHeight, double pixelRatio)
    {
        return Menu_anchorLibrary._roundToDivisible((lineHeight * _kLastBaselineToBottomSlope), to: (1L / pixelRatio));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Painting.EdgeInsets _resolvePadding(double minimumHeight, double lineHeight)
    {
        double padding__80855 = Math.Max(0, (minimumHeight - lineHeight));
        return global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(vertical: (DartRuntimePrimitives.RequireValue(padding__80855) / 2L));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Flutter.Ui.TextDirection textDirection__81042 = (Directionality.maybeOf(context) ?? TextDirection.ltr);
        global::Doroti.Generated.Framework.Painting.TextScaler textScaler__81133 = (MediaQuery.maybeTextScalerOf(context) ?? global::Doroti.Generated.Framework.Painting.TextScaler.noScaling);
        double pixelRatio__81226 = (MediaQuery.maybeDevicePixelRatioOf(context) ?? 1.0);
        global::Doroti.Generated.Framework.Painting.TextStyle dynamicBodyText__81311 = ((global::Doroti.Generated.Framework.Painting.TextStyle)(object?)_DynamicTypeStyle__menu_anchor.body.resolveTextStyle(textScaler__81133));
        DartRuntimePrimitives.Assert(() => ((((global::Doroti.Generated.Framework.Painting.TextStyle)dynamicBodyText__81311).fontSize is not null) && (((global::Doroti.Generated.Framework.Painting.TextStyle)dynamicBodyText__81311).height is not null)));
        double lineHeight__81479 = (DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Painting.TextStyle)dynamicBodyText__81311).fontSize) * DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Painting.TextStyle)dynamicBodyText__81311).height));
        bool showLeadingWidget__81560 = ((this.leading is not null) || ((CupertinoMenuAnchor.maybeHasLeadingOf(context) ?? false)));
        double minimumHeight__82324 = (_resolveFirstBaselineToTop(lineHeight__81479, pixelRatio__81226) + _resolveLastBaselineToBottom(lineHeight__81479, pixelRatio__81226));
        global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints__82488 = (this._constraints ?? new global::Doroti.Generated.Framework.Rendering.BoxConstraints(minHeight: minimumHeight__82324));
        global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry resolvedPadding__82590 = ((this.padding ?? (global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry)_resolvePadding(minimumHeight__82324, lineHeight__81479)));
        double resolvedLeadingWidth__82689 = (this.leadingWidth ?? ((showLeadingWidget__81560 ? _resolveLeadingWidth(textScaler__81133, pixelRatio__81226, lineHeight__81479) : _kDefaultHorizontalWidth)));
        double resolvedTrailingWidth__82893 = (this.trailingWidth ?? (((this.trailing is not null) ? _resolveTrailingWidth(textScaler__81133, pixelRatio__81226, lineHeight__81479) : _kDefaultHorizontalWidth)));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: constraints__82488, child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: resolvedPadding__82590, child: new global::Doroti.Generated.Framework.Widgets.Stack(children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection83239 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); if (showLeadingWidget__81560) { __collection83239.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(global::Doroti.Generated.Framework.Widgets.Positioned.CreateDirectional(textDirection: textDirection__81042, start: 0, top: 0, bottom: 0, width: resolvedLeadingWidth__82689, child: new _AlignMidpoint__menu_anchor(alignment: ((this._leadingAlignment ?? (global::Doroti.Generated.Framework.Painting.AlignmentGeometry)_resolveLeadingAlignment(resolvedLeadingWidth__82689, textScaler__81133))), child: this.leading)))); } __collection83239.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: resolvedLeadingWidth__82689, end: resolvedTrailingWidth__82893), child: ((this.subtitle is null) ? new global::Doroti.Generated.Framework.Widgets.Align(alignment: global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerStart, child: this.child) : new global::Doroti.Generated.Framework.Widgets.Column(mainAxisSize: global::Doroti.Generated.Framework.Rendering.MainAxisSize.min, crossAxisAlignment: global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.stretch, mainAxisAlignment: global::Doroti.Generated.Framework.Rendering.MainAxisAlignment.center, children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(this.child), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(height: 1)), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(this.subtitle!) }))))); if ((this.trailing is not null)) { __collection83239.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(global::Doroti.Generated.Framework.Widgets.Positioned.CreateDirectional(textDirection: textDirection__81042, end: 0, top: 0, bottom: 0, width: resolvedTrailingWidth__82893, child: new _AlignMidpoint__menu_anchor(alignment: ((this._trailingAlignment ?? (global::Doroti.Generated.Framework.Painting.AlignmentGeometry)_resolveTrailingAlignment(resolvedTrailingWidth__82893))), child: this.trailing)))); } return __collection83239; }))()))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AlignMidpoint__menu_anchor : global::Doroti.Generated.Framework.Widgets.SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;

    internal _AlignMidpoint__menu_anchor(global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment, global::Doroti.Generated.Framework.Widgets.Widget? child) : base(child: child)
    {
        this.alignment = alignment;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new _RenderAlignMidpoint__menu_anchor(alignment: this.alignment, textDirection: Directionality.maybeOf(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderAlignMidpoint__menu_anchor)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderAlignMidpoint__menu_anchor>)(() =>
{            var __cascade = __renderObject;
            __cascade.alignment = this.alignment;
            __cascade.textDirection = Directionality.maybeOf(context);
            return __cascade;        }))());
    }

}

public class _RenderAlignMidpoint__menu_anchor : global::Doroti.Generated.Framework.Rendering.RenderPositionedBox
{
    internal _RenderAlignMidpoint__menu_anchor(global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, TextDirection? textDirection = null) : base(alignment: alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center, textDirection: textDirection)
    {
    }

    public override void alignChild()
    {
        DartRuntimePrimitives.Assert(() => (this.child is not null));
        DartRuntimePrimitives.Assert(() => !this.child!.debugNeedsLayout);
        DartRuntimePrimitives.Assert(() => this.child!.hasSize);
        DartRuntimePrimitives.Assert(() => this.hasSize);
        var childParentData__86596 = ((global::Doroti.Generated.Framework.Rendering.BoxParentData?)(object?)this.child!.parentData!)!;
        global::Doroti.Flutter.Ui.Offset offset__86671 = ((global::Doroti.Flutter.Ui.Offset)(object?)(this.resolvedAlignment.alongSize(this.size) - this.child!.size.center(Offset.zero)));
        double dx__86766 = Dart_uiLibrary.clampDouble(offset__86671.dx, 0.0, (this.size.width - this.child!.size.width));
        double dy__86852 = Dart_uiLibrary.clampDouble(offset__86671.dy, 0.0, (this.size.height - this.child!.size.height));
        childParentData__86596.offset = new global::Doroti.Flutter.Ui.Offset(dx__86766, dy__86852);
    }

}

public class _CupertinoMenuItemInteractionHandler__menu_anchor : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual global::System.Action<bool>? onHover { get; private set; }
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual global::System.Action<bool>? onFocusChange { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual bool requestFocusOnHover { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.HitTestBehavior behavior { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor> mouseCursor { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BoxDecoration> decoration { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;

    internal _CupertinoMenuItemInteractionHandler__menu_anchor(global::System.Action<bool>? onHover, global::System.Action? onPressed, global::System.Action<bool>? onFocusChange, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode, bool autofocus, bool requestFocusOnHover, global::Doroti.Generated.Framework.Rendering.HitTestBehavior behavior, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor> mouseCursor, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BoxDecoration> decoration, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        this.onHover = onHover;
        this.onPressed = onPressed;
        this.onFocusChange = onFocusChange;
        this.focusNode = focusNode;
        this.autofocus = autofocus;
        this.requestFocusOnHover = requestFocusOnHover;
        this.behavior = behavior;
        this.mouseCursor = mouseCursor;
        this.decoration = decoration;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoMenuItemInteractionHandlerState__menu_anchor());
}

internal class _CupertinoMenuItemInteractionHandlerState__menu_anchor : global::Doroti.Generated.Framework.Widgets.State<_CupertinoMenuItemInteractionHandler__menu_anchor>
{
    private bool __late__actions_initialized;
    private DartMap<Type, dynamic> __late__actions = default!;
    internal virtual DartMap<Type, dynamic> _actions
    {
        get
        {
            if (!__late__actions_initialized)
            {
                __late__actions = new DartMap<Type, dynamic> { [typeof(global::Doroti.Generated.Framework.Widgets.ActivateIntent)] = new global::Doroti.Generated.Framework.Widgets.CallbackAction<global::Doroti.Generated.Framework.Widgets.ActivateIntent>(onInvoke: (global::System.Action<global::Doroti.Generated.Framework.Widgets.Intent?>)this._handleActivation), [typeof(global::Doroti.Generated.Framework.Widgets.ButtonActivateIntent)] = new global::Doroti.Generated.Framework.Widgets.CallbackAction<global::Doroti.Generated.Framework.Widgets.ButtonActivateIntent>(onInvoke: (global::System.Action<global::Doroti.Generated.Framework.Widgets.Intent?>)this._handleActivation) };
                __late__actions_initialized = true;
            }
            return __late__actions;
        }
    }
    internal virtual DartMap<Type, dynamic>? _gestures { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Gestures.DeviceGestureSettings? _gestureSettings { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Widgets.FocusNode? _internalFocusNode { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Widgets.WidgetStatesController _statesController { get; private set; } = new global::Doroti.Generated.Framework.Widgets.WidgetStatesController();

    internal virtual global::Doroti.Generated.Framework.Widgets.FocusNode _focusNode => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.FocusNode>((((_CupertinoMenuItemInteractionHandler__menu_anchor)this.widget).focusNode ?? this._internalFocusNode!));
    public virtual bool isHovered
    {
        get => this._statesController.value.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered);
        set
        {
            var __value = value;
            this._statesController.update(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered, __value);
        }
    }
    public virtual bool isPressed
    {
        get => this._statesController.value.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed);
        set
        {
            var __value = value;
            this._statesController.update(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed, __value);
        }
    }
    public virtual bool isSwiped
    {
        get => this._statesController.value.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.dragged);
        set
        {
            var __value = value;
            this._statesController.update(global::Doroti.Generated.Framework.Widgets.WidgetState.dragged, __value);
        }
    }
    public virtual bool isFocused
    {
        get => this._statesController.value.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused);
        set
        {
            var __value = value;
            this._statesController.update(DartRuntimePrimitives.RequireValue(global::Doroti.Generated.Framework.Widgets.WidgetState.focused), __value);
        }
    }
    public virtual bool isEnabled
    {
        get => !this._statesController.value.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled);
        set
        {
            var __value = value;
            this._statesController.update(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled, !__value);
        }
    }
    public override void initState()
    {
        base.initState();
        if ((((_CupertinoMenuItemInteractionHandler__menu_anchor)this.widget).focusNode is null))
        {
            _internalFocusNode = new global::Doroti.Generated.Framework.Widgets.FocusNode();
        }
        isEnabled = (((_CupertinoMenuItemInteractionHandler__menu_anchor)this.widget).onPressed is not null);
        isFocused = ((global::Doroti.Generated.Framework.Widgets.FocusNode)this._focusNode).hasPrimaryFocus;
    }

    public override void didUpdateWidget(_CupertinoMenuItemInteractionHandler__menu_anchor oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((_CupertinoMenuItemInteractionHandler__menu_anchor)this.widget).focusNode, ((_CupertinoMenuItemInteractionHandler__menu_anchor)oldWidget).focusNode)))
        {
            if ((((_CupertinoMenuItemInteractionHandler__menu_anchor)this.widget).focusNode is not null))
            {
                this._internalFocusNode?.dispose();
                _internalFocusNode = null;
            }
            else
            {
                DartRuntimePrimitives.Assert(() => (this._internalFocusNode is null));
                _internalFocusNode = new global::Doroti.Generated.Framework.Widgets.FocusNode();
            }
            isFocused = ((global::Doroti.Generated.Framework.Widgets.FocusNode)this._focusNode).hasPrimaryFocus;
        }
        if ((!object.Equals((global::System.Action?)((_CupertinoMenuItemInteractionHandler__menu_anchor)this.widget).onPressed, (global::System.Action?)((_CupertinoMenuItemInteractionHandler__menu_anchor)oldWidget).onPressed)))
        {
            if ((((_CupertinoMenuItemInteractionHandler__menu_anchor)this.widget).onPressed is null))
            {
                isEnabled = isHovered = isPressed = isSwiped = isFocused = false;
            }
            else
            {
                isEnabled = true;
            }
        }
    }

    public override void dispose()
    {
        this._statesController.dispose();
        this._internalFocusNode?.dispose();
        _internalFocusNode = null;
        base.dispose();
    }

    internal virtual void _handleFocusChange(bool? focused = null)
    {
        isFocused = ((global::Doroti.Generated.Framework.Widgets.FocusNode)this._focusNode).hasPrimaryFocus;
        ((_CupertinoMenuItemInteractionHandler__menu_anchor)this.widget).onFocusChange?.Invoke(this.isFocused);
    }

    internal virtual void _handleActivation(global::Doroti.Generated.Framework.Widgets.Intent? intent = null)
    {
        isSwiped = isPressed = false;
        ((_CupertinoMenuItemInteractionHandler__menu_anchor)this.widget).onPressed?.Invoke();
    }

    internal virtual void _handleTapDown(global::Doroti.Generated.Framework.Gestures.TapDownDetails details)
    {
        isPressed = true;
    }

    internal virtual void _handleTapUp(global::Doroti.Generated.Framework.Gestures.TapUpDetails? details)
    {
        isPressed = false;
        ((_CupertinoMenuItemInteractionHandler__menu_anchor)this.widget).onPressed?.Invoke();
    }

    internal virtual void _handleTapCancel()
    {
        isPressed = false;
    }

    internal virtual void _handlePointerExit(global::Doroti.Generated.Framework.Gestures.PointerExitEvent @event)
    {
        if (this.isHovered)
        {
            isHovered = isFocused = false;
            ((_CupertinoMenuItemInteractionHandler__menu_anchor)this.widget).onHover?.Invoke(false);
        }
    }

    internal virtual void _handlePointerHover(global::Doroti.Generated.Framework.Gestures.PointerHoverEvent @event)
    {
        if (!this.isHovered)
        {
            isHovered = true;
            ((_CupertinoMenuItemInteractionHandler__menu_anchor)this.widget).onHover?.Invoke(true);
            if (((_CupertinoMenuItemInteractionHandler__menu_anchor)this.widget).requestFocusOnHover)
            {
                this._focusNode.requestFocus();
                FocusTraversalGroup.of(this.context).invalidateScopeData(FocusScope.of(this.context));
            }
        }
    }

    internal virtual void _handleDismissMenu()
    {
        Actions.invoke(this.context, new global::Doroti.Generated.Framework.Widgets.DismissIntent());
    }

    internal virtual void _handleSwipeEnter()
    {
        if (!this.isEnabled)
        {
            return;
        }
        switch (global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.android:
                {
                    DartRuntimePrimitives.Ignore(HapticFeedback.selectionClick());
                    break;
                }
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
                {
                    break;
                }
        }
        isSwiped = true;
    }

    internal virtual void _handleSwipeExit()
    {
        if (this.mounted)
        {
            isSwiped = false;
        }
    }

    internal virtual void _handleSwipeCompleted()
    {
        if ((this.mounted && this.isEnabled))
        {
            _handleActivation();
        }
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildStatefulAppearance(global::Doroti.Generated.Framework.Widgets.BuildContext context, HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> value, global::Doroti.Generated.Framework.Widgets.Widget? child)
    {
        global::Doroti.Generated.Framework.Services.MouseCursor cursor__92502 = ((global::Doroti.Generated.Framework.Services.MouseCursor)(object?)((_CupertinoMenuItemInteractionHandler__menu_anchor)this.widget).mouseCursor.resolve(value));
        global::Doroti.Generated.Framework.Painting.BoxDecoration decoration__92570 = ((global::Doroti.Generated.Framework.Painting.BoxDecoration)(object?)((_CupertinoMenuItemInteractionHandler__menu_anchor)this.widget).decoration.resolve(value));
        bool hasBackground__92632 = ((((global::Doroti.Generated.Framework.Painting.BoxDecoration)decoration__92570).color is not null) || (((global::Doroti.Generated.Framework.Painting.BoxDecoration)decoration__92570).gradient is not null));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.MouseRegion(onHover: ((global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerHoverEvent>)(this.isEnabled ? this._handlePointerHover : null)), onExit: ((global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerExitEvent>)(this.isEnabled ? this._handlePointerExit : null)), hitTestBehavior: global::Doroti.Generated.Framework.Rendering.HitTestBehavior.deferToChild, cursor: cursor__92502, child: new global::Doroti.Generated.Framework.Widgets.DecoratedBox(decoration: decoration__92570.copyWith(color: CupertinoDynamicColor.maybeResolve(((global::Doroti.Generated.Framework.Painting.BoxDecoration)decoration__92570).color, context), backgroundBlendMode: (((global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb || !hasBackground__92632) || (((global::Doroti.Generated.Framework.Painting.BoxDecoration)decoration__92570).backgroundBlendMode is not null)) ? ((global::Doroti.Generated.Framework.Painting.BoxDecoration)decoration__92570).backgroundBlendMode : ((object.Equals(CupertinoTheme.maybeBrightnessOf(context), Brightness.light)) ? BlendMode.multiply : BlendMode.plus))), child: child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Generated.Framework.Gestures.DeviceGestureSettings? newGestureSettings__93488 = ((global::Doroti.Generated.Framework.Gestures.DeviceGestureSettings?)(object?)MediaQuery.maybeGestureSettingsOf(context));
        if ((!object.Equals(this._gestureSettings, newGestureSettings__93488)))
        {
            _gestureSettings = newGestureSettings__93488;
            _gestures = null;
        }
        _gestures ??= new DartMap<Type, dynamic> { [typeof(global::Doroti.Generated.Framework.Gestures.TapGestureRecognizer)] = new global::Doroti.Generated.Framework.Widgets.GestureRecognizerFactoryWithHandlers<global::Doroti.Generated.Framework.Gestures.TapGestureRecognizer>(((global::System.Func<global::Doroti.Generated.Framework.Gestures.TapGestureRecognizer>)(() => new global::Doroti.Generated.Framework.Gestures.TapGestureRecognizer(debugOwner: this))), ((global::System.Action<global::Doroti.Generated.Framework.Gestures.TapGestureRecognizer>)((instance) => {
DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Gestures.TapGestureRecognizer>)(() =>
{            var __cascade = instance;
            __cascade.onTapDown = this._handleTapDown;
            __cascade.onTapUp = this._handleTapUp;
            __cascade.onTapCancel = this._handleTapCancel;
            __cascade.gestureSettings = this._gestureSettings;
            return __cascade;        }))());
}))) };
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.MergeSemantics(child: global::Doroti.Generated.Framework.Widgets.Semantics.CreateFromProperties(properties: new global::Doroti.Generated.Framework.Semantics.SemanticsProperties(enabled: this.isEnabled, onDismiss: ((global::System.Action)(this.isEnabled ? this._handleDismissMenu : null))), child: new global::Doroti.Generated.Framework.Widgets.Actions(actions: (this.isEnabled ? this._actions : new DartMap<Type, dynamic>()), child: new global::Doroti.Generated.Framework.Widgets.Focus(autofocus: (this.isEnabled && ((_CupertinoMenuItemInteractionHandler__menu_anchor)this.widget).autofocus), focusNode: this._focusNode, canRequestFocus: this.isEnabled, skipTraversal: !this.isEnabled, onFocusChange: (global::System.Action<bool>)(value => this._handleFocusChange(value)), child: new _SwipeTarget__menu_anchor(onEnter: () => this._handleSwipeEnter(), onExit: () => this._handleSwipeExit(), onCompletion: () => this._handleSwipeCompleted(), child: new global::Doroti.Generated.Framework.Widgets.ValueListenableBuilder<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>>(valueListenable: this._statesController, builder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)this._buildStatefulAppearance, child: new global::Doroti.Generated.Framework.Widgets.RawGestureDetector(behavior: ((_CupertinoMenuItemInteractionHandler__menu_anchor)this.widget).behavior, gestures: (this.isEnabled ? this._gestures! : new DartMap<Type, dynamic>()), child: ((_CupertinoMenuItemInteractionHandler__menu_anchor)this.widget).child))))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SwipeTarget__menu_anchor : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::System.Action? onEnter { get; private set; }
    public virtual global::System.Action? onExit { get; private set; }
    public virtual global::System.Action? onCompletion { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;

    internal _SwipeTarget__menu_anchor(global::System.Action? onEnter, global::System.Action? onExit, global::System.Action? onCompletion, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        this.onEnter = onEnter;
        this.onExit = onExit;
        this.onCompletion = onCompletion;
        this.child = child;
    }

    public virtual bool isOpaque => true;
    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.MetaData(metaData: this, child: this.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SwipeScope__menu_anchor : global::Doroti.Generated.Framework.Widgets.InheritedWidget
{
    public virtual _SwipeRegionState__menu_anchor state { get; private set; } = default!;

    internal _SwipeScope__menu_anchor(global::Doroti.Generated.Framework.Widgets.Widget child, _SwipeRegionState__menu_anchor state) : base(child: child)
    {
        this.state = state;
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget)
    {
        var __oldWidget = (_SwipeScope__menu_anchor)(object)oldWidget;
        return (!object.Equals(this.state, ((_SwipeScope__menu_anchor)__oldWidget).state));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _SwipeRegion__menu_anchor : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual bool enabled { get; private set; } = default!;
    public virtual global::System.Action<double> onDistanceChanged { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;

    internal _SwipeRegion__menu_anchor(bool enabled = true, global::System.Action<double> onDistanceChanged = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!)
    {
        this.enabled = enabled;
        this.onDistanceChanged = onDistanceChanged;
        this.child = child;
    }

    public static _SwipeRegionState__menu_anchor? of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        _SwipeScope__menu_anchor? scope__97674 = ((_SwipeScope__menu_anchor?)(object?)context.dependOnInheritedWidgetOfExactType<_SwipeScope__menu_anchor>());
        return scope__97674?.state;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SwipeRegionState__menu_anchor());
}

public class _SwipeRegionState__menu_anchor : global::Doroti.Generated.Framework.Widgets.State<_SwipeRegion__menu_anchor>
{
    internal virtual HashSet<_RenderSwipeSurface__menu_anchor> _surfaces { get; private set; } = new HashSet<_RenderSwipeSurface__menu_anchor>();
    internal virtual global::Doroti.Generated.Framework.Gestures.MultiDragGestureRecognizer? _recognizer { get; set; } = default;
    internal virtual Offset? _position { get; set; } = default;

    public virtual bool isSwiping => DartRuntimePrimitives.ConvertValue<bool>((this._position is not null));
    public override void didUpdateWidget(_SwipeRegion__menu_anchor oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((((_SwipeRegion__menu_anchor)this.widget).enabled != ((_SwipeRegion__menu_anchor)oldWidget).enabled))
        {
            if (!((_SwipeRegion__menu_anchor)this.widget).enabled)
            {
                this._recognizer?.dispose();
                _recognizer = null;
                _position = null;
                this.widget.onDistanceChanged(0);
            }
        }
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        this._recognizer?.gestureSettings = MediaQuery.maybeGestureSettingsOf(this.context);
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() => !System.Linq.Enumerable.Any(this._surfaces));
        this._recognizer?.dispose();
        _recognizer = null;
        base.dispose();
    }

    public virtual void attachSurface(_RenderSwipeSurface__menu_anchor surface)
    {
        this._surfaces.Add(surface);
    }

    public virtual void detachSurface(_RenderSwipeSurface__menu_anchor surface)
    {
        this._surfaces.Remove(surface);
    }

    public virtual void beginSwipe(global::Doroti.Generated.Framework.Gestures.PointerDownEvent @event, Duration delay = default, global::System.Action? onStart = null)
    {
        if ((this.isSwiping || !((_SwipeRegion__menu_anchor)this.widget).enabled))
        {
            return;
        }
        this._recognizer?.dispose();
        _recognizer = null;
        global::Doroti.Generated.Framework.Gestures.Drag handleStart(Offset position)
        {
            onStart?.Invoke();
            return ((global::Doroti.Generated.Framework.Gestures.Drag)(object?)_createSwipeHandle(position));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        if ((object.Equals(delay, Duration.zero)))
        {
            _recognizer = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Gestures.MultiDragGestureRecognizer>(((Func<global::Doroti.Generated.Framework.Gestures.ImmediateMultiDragGestureRecognizer>)(() =>
{            var __cascade = new global::Doroti.Generated.Framework.Gestures.ImmediateMultiDragGestureRecognizer(allowedButtonsFilter: ((global::System.Func<long, bool>?)((button) => (button == global::Doroti.Generated.Framework.Gestures.EventsLibrary.kPrimaryButton))));
            __cascade.onStart = handleStart;
            return __cascade;        }))());
        }
        else
        {
            _recognizer = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Gestures.MultiDragGestureRecognizer>(((Func<global::Doroti.Generated.Framework.Gestures.DelayedMultiDragGestureRecognizer>)(() =>
{            var __cascade = new global::Doroti.Generated.Framework.Gestures.DelayedMultiDragGestureRecognizer(delay: delay, allowedButtonsFilter: ((global::System.Func<long, bool>?)((button) => (button == global::Doroti.Generated.Framework.Gestures.EventsLibrary.kPrimaryButton))));
            __cascade.onStart = handleStart;
            return __cascade;        }))());
        }
        this._recognizer!.gestureSettings = MediaQuery.maybeGestureSettingsOf(this.context);
        this._recognizer!.addPointer((global::Doroti.Generated.Framework.Gestures.PointerDownEvent)(object)@event);
    }

    internal virtual global::Doroti.Generated.Framework.Gestures.Drag _createSwipeHandle(Offset position)
    {
        DartRuntimePrimitives.Assert(() => !this.isSwiping, () => (object?)"A new swipe should not begin while a swipe is active.");
        _position = position;
        return ((global::Doroti.Generated.Framework.Gestures.Drag)(object?)new _SwipeHandle__menu_anchor(viewId: checked((long)View.of(this.context).viewId), initialPosition: position, onSwipeUpdate: (global::System.Action<global::Doroti.Generated.Framework.Gestures.DragUpdateDetails>)this._handleSwipeUpdate, onSwipeEnd: (global::System.Action<global::Doroti.Generated.Framework.Gestures.DragEndDetails>)this._handleSwipeEnd, onSwipeCanceled: () => this._handleSwipeCancel()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleSwipeUpdate(global::Doroti.Generated.Framework.Gestures.DragUpdateDetails updateDetails)
    {
        _position = (DartRuntimePrimitives.RequireValue(this._position) + ((global::Doroti.Generated.Framework.Gestures.DragUpdateDetails)updateDetails).delta);
        double minimumSquaredDistance__100459 = double.MaxValue;
        foreach (_RenderSwipeSurface__menu_anchor surface__100537 in this._surfaces)
        {
            double squaredDistance__100580 = Menu_anchorLibrary._computeSquaredDistanceToRect(DartRuntimePrimitives.RequireValue(this._position), surface__100537.computeRect());
            if ((squaredDistance__100580.floor() == 0L))
            {
                this.widget.onDistanceChanged(0);
                return;
            }
            minimumSquaredDistance__100459 = Math.Min(squaredDistance__100580, minimumSquaredDistance__100459);
        }
        double distance__100900 = ((minimumSquaredDistance__100459 == 0L) ? 0 : global::Doroti.Flutter.Runtime.Dart_mathLibrary.sqrt(minimumSquaredDistance__100459));
        this.widget.onDistanceChanged(distance__100900);
    }

    internal virtual void _handleSwipeEnd(global::Doroti.Generated.Framework.Gestures.DragEndDetails updateDetails)
    {
        _completeSwipe();
    }

    internal virtual void _handleSwipeCancel()
    {
        _completeSwipe();
    }

    internal virtual void _completeSwipe()
    {
        this._recognizer?.dispose();
        _recognizer = null;
        _position = null;
        if (this.mounted)
        {
            this.widget.onDistanceChanged(0);
        }
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _SwipeScope__menu_anchor(state: this, child: ((_SwipeRegion__menu_anchor)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SwipeSurface__menu_anchor : global::Doroti.Generated.Framework.Widgets.SingleChildRenderObjectWidget
{
    public virtual Duration delay { get; private set; } = default!;
    public virtual global::System.Action? onStart { get; private set; }

    internal _SwipeSurface__menu_anchor(global::Doroti.Generated.Framework.Widgets.Widget? child, Duration delay = default, global::System.Action? onStart = null) : base(child: child)
    {
        this.delay = delay;
        this.onStart = onStart;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new _RenderSwipeSurface__menu_anchor(region: _SwipeRegion__menu_anchor.of(context)!, delay: this.delay, onStart: () => this.onStart()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderSwipeSurface__menu_anchor)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderSwipeSurface__menu_anchor>)(() =>
{            var __cascade = __renderObject;
            __cascade.region = _SwipeRegion__menu_anchor.of(context)!;
            __cascade.delay = this.delay;
            __cascade.onStart = this.onStart;
            return __cascade;        }))());
    }

}

public class _RenderSwipeSurface__menu_anchor : global::Doroti.Generated.Framework.Rendering.RenderProxyBoxWithHitTestBehavior
{
    internal virtual _SwipeRegionState__menu_anchor _region { get; set; } = default!;
    public virtual Duration delay { get; set; } = default!;
    public virtual global::System.Action? onStart { get; set; } = default;

    internal _RenderSwipeSurface__menu_anchor(_SwipeRegionState__menu_anchor region, Duration delay, global::System.Action? onStart) : base(behavior: global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque)
    {
        this.delay = delay;
        this.onStart = onStart;
        this._region = region;
        this._region.attachSurface(this);
    }

    public virtual _SwipeRegionState__menu_anchor region
    {
        get => this._region;
        set
        {
            var __value = value;
            if ((!object.Equals(this._region, __value)))
            {
                this._region.detachSurface(this);
                _region = __value;
                this._region.attachSurface(this);
            }
        }
    }
    public virtual global::Doroti.Flutter.Ui.Rect computeRect() => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Rect>((localToGlobal(Offset.zero) & this.size));
    public virtual void detach()
    {
        this._region.detachSurface(this);
        base.detach();
    }

    public override void dispose()
    {
        this._region.detachSurface(this);
        base.dispose();
    }

    public override void handleEvent(global::Doroti.Generated.Framework.Gestures.PointerEvent @event, global::Doroti.Generated.Framework.Gestures.HitTestEntry<global::Doroti.Generated.Framework.Gestures.HitTestTarget> entry)
    {
        DartRuntimePrimitives.Assert(() => debugHandleEvent(@event, entry));
        if ((@event is global::Doroti.Generated.Framework.Gestures.PointerDownEvent))
        {
            global::Doroti.Generated.Framework.Gestures.PointerDownEvent @event__as103538 = (global::Doroti.Generated.Framework.Gestures.PointerDownEvent)@event;
            this._region.beginSwipe(((global::Doroti.Generated.Framework.Gestures.PointerDownEvent)@event__as103538), delay: this.delay, onStart: () => this.onStart());
        }
    }

}

internal class _SwipeHandle__menu_anchor : global::Doroti.Generated.Framework.Gestures.Drag
{
    public virtual long viewId { get; private set; } = default!;
    internal virtual List<_SwipeTarget__menu_anchor> _enteredTargets { get; private set; } = new List<_SwipeTarget__menu_anchor>();
    public virtual global::System.Action<global::Doroti.Generated.Framework.Gestures.DragUpdateDetails> onSwipeUpdate { get; private set; } = default!;
    public virtual global::System.Action<global::Doroti.Generated.Framework.Gestures.DragEndDetails> onSwipeEnd { get; private set; } = default!;
    public virtual global::System.Action onSwipeCanceled { get; private set; } = default!;
    internal virtual Offset _position { get; set; } = default!;

    internal _SwipeHandle__menu_anchor(Offset initialPosition, long viewId, global::System.Action<global::Doroti.Generated.Framework.Gestures.DragEndDetails> onSwipeEnd, global::System.Action<global::Doroti.Generated.Framework.Gestures.DragUpdateDetails> onSwipeUpdate, global::System.Action onSwipeCanceled)
    {
        this.viewId = viewId;
        this.onSwipeEnd = onSwipeEnd;
        this.onSwipeUpdate = onSwipeUpdate;
        this.onSwipeCanceled = onSwipeCanceled;
        this._position = initialPosition;
        _updateSwipe();
    }

    public override void update(global::Doroti.Generated.Framework.Gestures.DragUpdateDetails details)
    {
        global::Doroti.Flutter.Ui.Offset oldPosition__104368 = ((global::Doroti.Flutter.Ui.Offset)(object?)this._position);
        _position += ((global::Doroti.Generated.Framework.Gestures.DragUpdateDetails)details).delta;
        if ((!object.Equals(this._position, oldPosition__104368)))
        {
            _updateSwipe();
            this.onSwipeUpdate?.Invoke(details);
        }
    }

    public override void end(global::Doroti.Generated.Framework.Gestures.DragEndDetails details)
    {
        _leaveAllEntered(pointerUp: true);
        this.onSwipeEnd?.Invoke(details);
    }

    public override void cancel()
    {
        _leaveAllEntered();
        this.onSwipeCanceled();
    }

    internal virtual void _updateSwipe()
    {
        var result__104768 = new global::Doroti.Generated.Framework.Gestures.HitTestResult();
        global::Doroti.Generated.Framework.Widgets.WidgetsBinding.instance.hitTestInView(result__104768, this._position, this.viewId);
        var targets__104941 = new List<_SwipeTarget__menu_anchor>();
        foreach (global::Doroti.Generated.Framework.Gestures.HitTestEntry<global::Doroti.Generated.Framework.Gestures.HitTestTarget> entry__104997 in ((global::Doroti.Generated.Framework.Gestures.HitTestResult)result__104768).path)
        {
            if (((global::Doroti.Generated.Framework.Gestures.HitTestEntry<global::Doroti.Generated.Framework.Gestures.HitTestTarget>)entry__104997).target is global::Doroti.Generated.Framework.Rendering.RenderMetaData { metaData: _SwipeTarget__menu_anchor metaData__105084 } __object105049)
            {
                targets__104941.Add(metaData__105084);
            }
        }
        this._enteredTargets.removeWhere(((target) => {
if (!targets__104941.Contains(target))
{
    ((_SwipeTarget__menu_anchor)target).onExit?.Invoke();
    return true;
}
return false;
throw new InvalidOperationException("Dart closure completed without a value.");
}));
        var hitTargets__105324 = new HashSet<_SwipeTarget__menu_anchor>();
        var newlyEnteredTargets__105365 = new List<_SwipeTarget__menu_anchor>();
        var hitExistingTarget__105413 = false;
        foreach (var target__105455 in targets__104941)
        {
            if (this._enteredTargets.Contains(target__105455))
            {
                hitTargets__105324.Add(target__105455);
                hitExistingTarget__105413 = true;
                continue;
            }
            if (!hitExistingTarget__105413)
            {
                hitTargets__105324.Add(target__105455);
                newlyEnteredTargets__105365.Add(target__105455);
            }
            if (((_SwipeTarget__menu_anchor)target__105455).isOpaque)
            {
                break;
            }
        }
        foreach (_SwipeTarget__menu_anchor target__106210 in System.Linq.Enumerable.Reverse(this._enteredTargets))
        {
            if (!hitTargets__105324.Contains(target__106210))
            {
                ((_SwipeTarget__menu_anchor)target__106210).onExit?.Invoke();
            }
        }
        foreach (_SwipeTarget__menu_anchor target__106364 in System.Linq.Enumerable.Reverse(newlyEnteredTargets__105365))
        {
            ((_SwipeTarget__menu_anchor)target__106364).onEnter?.Invoke();
        }
        DartRuntimePrimitives.Ignore(((Func<List<_SwipeTarget__menu_anchor>>)(() =>
{            var __cascade = this._enteredTargets;
            __cascade.Clear();
            __cascade.AddRange(hitTargets__105324);
            return __cascade;        }))());
    }

    internal virtual void _leaveAllEntered(bool pointerUp = false)
    {
        for (var i__106577 = 0L; (i__106577 < checked((long)(this._enteredTargets.Count))); i__106577 += 1L)
        {
            _SwipeTarget__menu_anchor target__106647 = this._enteredTargets[(int)(i__106577)];
            ((_SwipeTarget__menu_anchor)target__106647).onExit?.Invoke();
            if (pointerUp)
            {
                ((_SwipeTarget__menu_anchor)target__106647).onCompletion?.Invoke();
            }
        }
        this._enteredTargets.Clear();
    }

}

internal class _AnimationProduct__menu_anchor : global::Doroti.Generated.Framework.Animation.CompoundAnimation<double>
{
    internal _AnimationProduct__menu_anchor(global::Doroti.Generated.Framework.Animation.Animation<double> first, global::Doroti.Generated.Framework.Animation.Animation<double> next) : base(first: first, next: next)
    {
    }

    public override double value => DartRuntimePrimitives.ConvertValue<double>((base.first.value * base.next.value));
}

internal class _ClampTween__menu_anchor : global::Doroti.Generated.Framework.Animation.Animatable<double>
{
    public virtual double begin { get; private set; } = default!;
    public virtual double end { get; private set; } = default!;

    internal _ClampTween__menu_anchor(double begin, double end)
    {
        this.begin = begin;
        this.end = end;
    }

    public override double transform(double t)
    {
        if ((t < this.begin))
        {
            return this.begin;
        }
        if ((t > this.end))
        {
            return this.end;
        }
        return t;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
