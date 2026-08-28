// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/navigation_rail.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Material;

public static partial class Navigation_railLibrary
{
    internal static double _kCircularIndicatorDiameter = 56;
}

public static partial class Navigation_railLibrary
{
    internal static double _kIndicatorHeight = 32;
}

public class NavigationRail : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual bool extended { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? leading { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? trailing { get; private set; }
    public virtual List<NavigationRailDestination> destinations { get; private set; } = default!;
    public virtual long? selectedIndex { get; private set; }
    public virtual global::System.Action<long>? onDestinationSelected { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual double? groupAlignment { get; private set; }
    public virtual NavigationRailLabelType? labelType { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? unselectedLabelTextStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? selectedLabelTextStyle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.IconThemeData? unselectedIconTheme { get; private set; }
    public virtual global::Doroti.Framework.Widgets.IconThemeData? selectedIconTheme { get; private set; }
    public virtual double? minWidth { get; private set; }
    public virtual double? minExtendedWidth { get; private set; }
    public virtual bool? useIndicator { get; private set; }
    public virtual Color? indicatorColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? indicatorShape { get; private set; }
    public virtual bool leadingAtTop { get; private set; } = default!;
    public virtual bool trailingAtBottom { get; private set; } = default!;
    public virtual bool scrollable { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.MainAxisAlignment? mainAxisAlignment { get; private set; }

    public NavigationRail(global::Doroti.Framework.Foundation.Key? key = null, Color? backgroundColor = null, bool extended = false, global::Doroti.Framework.Widgets.Widget? leading = null, global::Doroti.Framework.Widgets.Widget? trailing = null, List<NavigationRailDestination> destinations = default!, long? selectedIndex = default!, global::System.Action<long>? onDestinationSelected = null, double? elevation = null, double? groupAlignment = null, NavigationRailLabelType? labelType = null, global::Doroti.Framework.Painting.TextStyle? unselectedLabelTextStyle = null, global::Doroti.Framework.Painting.TextStyle? selectedLabelTextStyle = null, global::Doroti.Framework.Widgets.IconThemeData? unselectedIconTheme = null, global::Doroti.Framework.Widgets.IconThemeData? selectedIconTheme = null, double? minWidth = null, double? minExtendedWidth = null, bool? useIndicator = null, Color? indicatorColor = null, global::Doroti.Framework.Painting.ShapeBorder? indicatorShape = null, bool leadingAtTop = true, bool trailingAtBottom = false, bool scrollable = false, global::Doroti.Framework.Rendering.MainAxisAlignment? mainAxisAlignment = null) : base(key: key)
    {
        this.backgroundColor = backgroundColor;
        this.extended = extended;
        this.leading = leading;
        this.trailing = trailing;
        this.destinations = destinations;
        this.selectedIndex = selectedIndex;
        this.onDestinationSelected = onDestinationSelected;
        this.elevation = elevation;
        this.groupAlignment = groupAlignment;
        this.labelType = labelType;
        this.unselectedLabelTextStyle = unselectedLabelTextStyle;
        this.selectedLabelTextStyle = selectedLabelTextStyle;
        this.unselectedIconTheme = unselectedIconTheme;
        this.selectedIconTheme = selectedIconTheme;
        this.minWidth = minWidth;
        this.minExtendedWidth = minExtendedWidth;
        this.useIndicator = useIndicator;
        this.indicatorColor = indicatorColor;
        this.indicatorShape = indicatorShape;
        this.leadingAtTop = leadingAtTop;
        this.trailingAtBottom = trailingAtBottom;
        this.scrollable = scrollable;
        this.mainAxisAlignment = mainAxisAlignment;
        System.Diagnostics.Debug.Assert(((selectedIndex is null) || (((0L <= DartRuntimePrimitives.RequireValue(selectedIndex)) && (DartRuntimePrimitives.RequireValue(selectedIndex) < checked((long)(destinations.Count)))))));
        System.Diagnostics.Debug.Assert(((elevation is null) || (DartRuntimePrimitives.RequireValue(elevation) > 0L)));
        System.Diagnostics.Debug.Assert(((minWidth is null) || (DartRuntimePrimitives.RequireValue(minWidth) > 0L)));
        System.Diagnostics.Debug.Assert(((minExtendedWidth is null) || (DartRuntimePrimitives.RequireValue(minExtendedWidth) > 0L)));
        System.Diagnostics.Debug.Assert(((((minWidth is null) || (minExtendedWidth is null))) || (minExtendedWidth >= DartRuntimePrimitives.RequireValue(minWidth))));
        System.Diagnostics.Debug.Assert((!extended || (((labelType is null) || (object.Equals(DartRuntimePrimitives.RequireValue(labelType), NavigationRailLabelType.none))))));
    }

    public static global::Doroti.Framework.Animation.Animation<double> extendedAnimation(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return context.dependOnInheritedWidgetOfExactType<_ExtendedNavigationRailAnimation__navigation_rail>()!.animation;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _NavigationRailState__navigation_rail());
}

internal class _NavigationRailState__navigation_rail : global::Doroti.Framework.Widgets.State<NavigationRail>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<NavigationRail>
{
    internal virtual List<global::Doroti.Framework.Animation.AnimationController> _destinationControllers { get; set; } = default!;
    internal virtual List<global::Doroti.Framework.Animation.Animation<double>> _destinationAnimations { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.AnimationController _extendedController { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation _extendedAnimation { get; set; } = default!;
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _initControllers();
    }

    public override void dispose()
    {
        _disposeControllers();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        this._tickerModeNotifier?.removeListener(this._updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override void didUpdateWidget(NavigationRail oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((((NavigationRail)this.widget).extended != ((NavigationRail)oldWidget).extended))
        {
            if (((NavigationRail)this.widget).extended)
            {
                this._extendedController.forward();
            }
            else
            {
                this._extendedController.reverse();
            }
        }
        if ((checked((long)(((NavigationRail)this.widget).destinations.Count)) != checked((long)(((NavigationRail)oldWidget).destinations.Count))))
        {
            _resetState();
            return;
        }
        if ((((NavigationRail)this.widget).selectedIndex != ((NavigationRail)oldWidget).selectedIndex))
        {
            if ((((NavigationRail)oldWidget).selectedIndex is not null))
            {
                this._destinationControllers[(int)(DartRuntimePrimitives.RequireValue(((NavigationRail)oldWidget).selectedIndex))].reverse();
            }
            if ((((NavigationRail)this.widget).selectedIndex is not null))
            {
                this._destinationControllers[(int)(DartRuntimePrimitives.RequireValue(((NavigationRail)this.widget).selectedIndex))].forward();
            }
            return;
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        NavigationRailThemeData navigationRailTheme = NavigationRailTheme.of(context);
        NavigationRailThemeData defaults = (Theme.of(context).useMaterial3 ? new _NavigationRailDefaultsM3__navigation_rail(context) : new _NavigationRailDefaultsM2__navigation_rail(context));
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        global::Doroti.Ui.Color backgroundColorLocal = ((global::Doroti.Ui.Color)(object?)((((NavigationRail)this.widget).backgroundColor ?? navigationRailTheme.backgroundColor) ?? defaults.backgroundColor!));
        double elevationLocal = ((((NavigationRail)this.widget).elevation ?? navigationRailTheme.elevation) ?? DartRuntimePrimitives.RequireValue(defaults.elevation));
        double minWidthLocal = ((((NavigationRail)this.widget).minWidth ?? navigationRailTheme.minWidth) ?? DartRuntimePrimitives.RequireValue(defaults.minWidth));
        double minExtendedWidthLocal = ((((NavigationRail)this.widget).minExtendedWidth ?? navigationRailTheme.minExtendedWidth) ?? DartRuntimePrimitives.RequireValue(defaults.minExtendedWidth));
        global::Doroti.Framework.Painting.TextStyle unselectedLabelTextStyleLocal = ((((NavigationRail)this.widget).unselectedLabelTextStyle ?? navigationRailTheme.unselectedLabelTextStyle) ?? defaults.unselectedLabelTextStyle!);
        global::Doroti.Framework.Painting.TextStyle selectedLabelTextStyleLocal = ((((NavigationRail)this.widget).selectedLabelTextStyle ?? navigationRailTheme.selectedLabelTextStyle) ?? defaults.selectedLabelTextStyle!);
        global::Doroti.Framework.Widgets.IconThemeData unselectedIconThemeLocal = ((((NavigationRail)this.widget).unselectedIconTheme ?? navigationRailTheme.unselectedIconTheme) ?? defaults.unselectedIconTheme!);
        global::Doroti.Framework.Widgets.IconThemeData selectedIconThemeLocal = ((((NavigationRail)this.widget).selectedIconTheme ?? navigationRailTheme.selectedIconTheme) ?? defaults.selectedIconTheme!);
        double groupAlignmentLocal = ((((NavigationRail)this.widget).groupAlignment ?? navigationRailTheme.groupAlignment) ?? DartRuntimePrimitives.RequireValue(defaults.groupAlignment));
        NavigationRailLabelType labelTypeLocal = ((((NavigationRail)this.widget).labelType ?? navigationRailTheme.labelType) ?? DartRuntimePrimitives.RequireValue(defaults.labelType));
        bool useIndicatorLocal = ((((NavigationRail)this.widget).useIndicator ?? navigationRailTheme.useIndicator) ?? DartRuntimePrimitives.RequireValue(defaults.useIndicator));
        global::Doroti.Ui.Color? indicatorColorLocal = ((global::Doroti.Ui.Color?)(object?)((((NavigationRail)this.widget).indicatorColor ?? navigationRailTheme.indicatorColor) ?? defaults.indicatorColor));
        global::Doroti.Framework.Painting.ShapeBorder? indicatorShapeLocal = ((((NavigationRail)this.widget).indicatorShape ?? navigationRailTheme.indicatorShape) ?? defaults.indicatorShape);
        global::Doroti.Framework.Widgets.IconThemeData effectiveUnselectedIconTheme = (Theme.of(context).useMaterial3 ? unselectedIconThemeLocal : unselectedIconThemeLocal.copyWith(opacity: (((global::Doroti.Framework.Widgets.IconThemeData)unselectedIconThemeLocal).opacity ?? defaults.unselectedIconTheme!.opacity)));
        var isRTLDirection = (object.Equals(Directionality.of(context), TextDirection.rtl));
        global::Doroti.Framework.Widgets.Widget mainGroup = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Column(mainAxisSize: ((((NavigationRail)this.widget).mainAxisAlignment is not null) ? global::Doroti.Framework.Rendering.MainAxisSize.max : global::Doroti.Framework.Rendering.MainAxisSize.min), mainAxisAlignment: (((NavigationRail)this.widget).mainAxisAlignment ?? global::Doroti.Framework.Rendering.MainAxisAlignment.start), children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() =>
        {
            var __collection20149 = new List<global::Doroti.Framework.Widgets.Widget>(); if ((!((NavigationRail)this.widget).leadingAtTop && (((NavigationRail)this.widget).leading is not null))) { __collection20149.AddRange(new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(((NavigationRail)this.widget).leading!), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(Navigation_railLibrary._verticalSpacer) }); }
            for (long i = 0L; (i < checked((long)(((NavigationRail)this.widget).destinations.Count))); i += 1L)
            {
                __collection20149.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _RailDestination__navigation_rail(minWidth: minWidthLocal, minExtendedWidth: minExtendedWidthLocal, extendedTransitionAnimation: this._extendedAnimation, selected: (((NavigationRail)this.widget).selectedIndex == i), icon: ((((NavigationRail)this.widget).selectedIndex == i) ? ((NavigationRail)this.widget).destinations[(int)(i)].selectedIcon : ((NavigationRail)this.widget).destinations[(int)(i)].icon), label: ((NavigationRail)this.widget).destinations[(int)(i)].label, destinationAnimation: this._destinationAnimations[(int)(i)], labelType: labelTypeLocal, iconTheme: ((((NavigationRail)this.widget).selectedIndex == i) ? selectedIconThemeLocal : effectiveUnselectedIconTheme), labelTextStyle: ((((NavigationRail)this.widget).selectedIndex == i) ? selectedLabelTextStyleLocal : unselectedLabelTextStyleLocal), padding: ((NavigationRail)this.widget).destinations[(int)(i)].padding, useIndicator: useIndicatorLocal, indicatorColor: (useIndicatorLocal ? indicatorColorLocal : null), indicatorShape: (useIndicatorLocal ? indicatorShapeLocal : null), onTap: ((global::System.Action)(() =>
                {
                    if ((((NavigationRail)this.widget).onDestinationSelected is not null))
                    {
                        ((NavigationRail)this.widget).onDestinationSelected!(i);
                    }
                })), indexLabel: localizations.tabLabel(tabIndex: (i + 1L), tabCount: checked((long)(((NavigationRail)this.widget).destinations.Count))), disabled: ((NavigationRail)this.widget).destinations[(int)(i)].disabled)));
            }
            if ((!((NavigationRail)this.widget).trailingAtBottom && (((NavigationRail)this.widget).trailing is not null))) { __collection20149.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(((NavigationRail)this.widget).trailing!)); }
            return __collection20149;
        }))()));
        if (((NavigationRail)this.widget).scrollable)
        {
            mainGroup = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SingleChildScrollView(child: mainGroup));
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(container: true, child: new _ExtendedNavigationRailAnimation__navigation_rail(animation: this._extendedAnimation, child: new global::Doroti.Framework.Widgets.Semantics(explicitChildNodes: true, child: new Material(elevation: elevationLocal, color: backgroundColorLocal, child: new global::Doroti.Framework.Widgets.SafeArea(right: isRTLDirection, left: !isRTLDirection, child: new global::Doroti.Framework.Widgets.Column(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection22353 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection22353.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(Navigation_railLibrary._verticalSpacer)); if ((((NavigationRail)this.widget).leadingAtTop && (((NavigationRail)this.widget).leading is not null))) { __collection22353.AddRange(new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(((NavigationRail)this.widget).leading!), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(Navigation_railLibrary._verticalSpacer) }); } __collection22353.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: new global::Doroti.Framework.Widgets.Align(alignment: new global::Doroti.Framework.Painting.Alignment(0, groupAlignmentLocal), child: mainGroup)))); if ((((NavigationRail)this.widget).trailingAtBottom && (((NavigationRail)this.widget).trailing is not null))) { __collection22353.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(((NavigationRail)this.widget).trailing!)); } return __collection22353; }))())))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _disposeControllers()
    {
        foreach (global::Doroti.Framework.Animation.AnimationController controller in this._destinationControllers)
        {
            controller.dispose();
        }
        this._extendedController.dispose();
        this._extendedAnimation.dispose();
    }

    internal virtual void _initControllers()
    {
        _destinationControllers = new List<global::Doroti.Framework.Animation.AnimationController>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)checked((long)(((NavigationRail)this.widget).destinations.Count)))), ((index) =>
        {
            return ((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
            {
                var __cascade = new global::Doroti.Framework.Animation.AnimationController(duration: ThemeLibrary.kThemeAnimationDuration, vsync: this);
                __cascade.addListener(this._rebuild);
                return __cascade;
            }))();
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        _destinationAnimations = this._destinationControllers.map<global::Doroti.Framework.Animation.AnimationController, global::Doroti.Framework.Animation.Animation<double>>(((controller) => ((global::Doroti.Framework.Animation.AnimationController)controller).view)).ToList();
        if ((((NavigationRail)this.widget).selectedIndex is not null))
        {
            this._destinationControllers[(int)(DartRuntimePrimitives.RequireValue(((NavigationRail)this.widget).selectedIndex))].value = 1.0;
        }
        _extendedController = new global::Doroti.Framework.Animation.AnimationController(duration: ThemeLibrary.kThemeAnimationDuration, vsync: this, value: (((NavigationRail)this.widget).extended ? 1.0 : 0.0));
        _extendedAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this._extendedController, curve: global::Doroti.Framework.Animation.Curves.easeInOut);
        this._extendedController.addListener(((global::System.Action)(() =>
        {
            _rebuild();
        })));
    }

    internal virtual void _resetState()
    {
        _disposeControllers();
        _initControllers();
    }

    internal virtual void _rebuild()
    {
        setState(((global::System.Action)(() =>
        {
        })));
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = this._tickerModeNotifier!.value;
        var result = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
    __cascade.muted = !((TickerModeData)values).enabled;
    __cascade.forceFrames = ((TickerModeData)values).forceFrames;
    return __cascade;
}))();
        this._tickers!.Add(result);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider ticker)
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
            TickerModeData values = this._tickerModeNotifier!.value;
            bool mutedLocal = !((TickerModeData)values).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = ((TickerModeData)values).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(this._updateTickers);
        newNotifier.addListener(this._updateTickers);
        this._tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

public class _RailDestination__navigation_rail : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual double minWidth { get; private set; } = default!;
    public virtual double minExtendedWidth { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget icon { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget label { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> destinationAnimation { get; private set; } = default!;
    public virtual NavigationRailLabelType labelType { get; private set; } = default!;
    public virtual bool selected { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> extendedTransitionAnimation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.IconThemeData iconTheme { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle labelTextStyle { get; private set; } = default!;
    public virtual global::System.Action onTap { get; private set; } = default!;
    public virtual string indexLabel { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual bool useIndicator { get; private set; } = default!;
    public virtual Color? indicatorColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? indicatorShape { get; private set; }
    public virtual bool disabled { get; private set; } = default!;

    internal _RailDestination__navigation_rail(double minWidth, double minExtendedWidth, global::Doroti.Framework.Widgets.Widget icon, global::Doroti.Framework.Widgets.Widget label, global::Doroti.Framework.Animation.Animation<double> destinationAnimation, global::Doroti.Framework.Animation.Animation<double> extendedTransitionAnimation, NavigationRailLabelType labelType, bool selected, global::Doroti.Framework.Widgets.IconThemeData iconTheme, global::Doroti.Framework.Painting.TextStyle labelTextStyle, global::System.Action onTap, string indexLabel, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, bool useIndicator = default!, Color? indicatorColor = null, global::Doroti.Framework.Painting.ShapeBorder? indicatorShape = null, bool disabled = false)
    {
        this.minWidth = minWidth;
        this.minExtendedWidth = minExtendedWidth;
        this.icon = icon;
        this.label = label;
        this.destinationAnimation = destinationAnimation;
        this.extendedTransitionAnimation = extendedTransitionAnimation;
        this.labelType = labelType;
        this.selected = selected;
        this.iconTheme = iconTheme;
        this.labelTextStyle = labelTextStyle;
        this.onTap = onTap;
        this.indexLabel = indexLabel;
        this.padding = padding;
        this.useIndicator = useIndicator;
        this.indicatorColor = indicatorColor;
        this.indicatorShape = indicatorShape;
        this.disabled = disabled;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _RailDestinationState__navigation_rail());
}

internal class _RailDestinationState__navigation_rail : global::Doroti.Framework.Widgets.State<_RailDestination__navigation_rail>
{
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation _positionAnimation { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        _setPositionAnimation();
    }

    public override void didUpdateWidget(_RailDestination__navigation_rail oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((_RailDestination__navigation_rail)this.widget).destinationAnimation, ((_RailDestination__navigation_rail)oldWidget).destinationAnimation)))
        {
            this._positionAnimation.dispose();
            _setPositionAnimation();
        }
    }

    internal virtual void _setPositionAnimation()
    {
        _positionAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: new global::Doroti.Framework.Animation.ReverseAnimation(((_RailDestination__navigation_rail)this.widget).destinationAnimation), curve: global::Doroti.Framework.Animation.Curves.easeInOut, reverseCurve: global::Doroti.Framework.Animation.Curves.easeInOut.flipped);
    }

    public override void dispose()
    {
        this._positionAnimation.dispose();
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => (((_RailDestination__navigation_rail)this.widget).useIndicator || (((_RailDestination__navigation_rail)this.widget).indicatorColor is null)), () => (object?)"[NavigationRail.indicatorColor] does not have an effect when [NavigationRail.useIndicator] is false");
        ThemeData theme = Theme.of(context);
        global::Doroti.Ui.TextDirection textDirectionLocal = Directionality.of(context);
        bool material3 = theme.useMaterial3;
        global::Doroti.Framework.Painting.EdgeInsets destinationPadding = ((global::Doroti.Framework.Painting.EdgeInsets)(object?)((((_RailDestination__navigation_rail)this.widget).padding ?? global::Doroti.Framework.Painting.EdgeInsets.zero)).resolve(textDirectionLocal));
        global::Doroti.Ui.Offset indicatorOffsetLocal = default!;
        var applyXOffsetLocal = false;
        global::Doroti.Framework.Widgets.Widget themedIcon = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.IconTheme(data: (((_RailDestination__navigation_rail)this.widget).disabled ? ((_RailDestination__navigation_rail)this.widget).iconTheme.copyWith(color: theme.colorScheme.onSurface.withOpacity(0.38)) : ((_RailDestination__navigation_rail)this.widget).iconTheme), child: ((_RailDestination__navigation_rail)this.widget).icon));
        global::Doroti.Framework.Widgets.Widget styledLabel = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.DefaultTextStyle(style: (((_RailDestination__navigation_rail)this.widget).disabled ? ((_RailDestination__navigation_rail)this.widget).labelTextStyle.copyWith(color: theme.colorScheme.onSurface.withOpacity(0.38)) : ((_RailDestination__navigation_rail)this.widget).labelTextStyle), child: ((_RailDestination__navigation_rail)this.widget).label));
        global::Doroti.Framework.Widgets.Widget content = default!;
        bool isLargeIconSize = ((((_RailDestination__navigation_rail)this.widget).iconTheme.size is not null) && (DartRuntimePrimitives.RequireValue(((_RailDestination__navigation_rail)this.widget).iconTheme.size) > Navigation_barLibrary._kIndicatorHeight));
        double indicatorVerticalOffset = (isLargeIconSize ? (((DartRuntimePrimitives.RequireValue(((_RailDestination__navigation_rail)this.widget).iconTheme.size) - Navigation_barLibrary._kIndicatorHeight)) / 2L) : 0);
        switch (((_RailDestination__navigation_rail)this.widget).labelType)
        {
            case NavigationRailLabelType.none:
                {
                    global::Doroti.Framework.Widgets.Widget? spacing = ((global::Doroti.Framework.Widgets.Widget?)(object?)(material3 ? new global::Doroti.Framework.Widgets.SizedBox(height: (Navigation_railLibrary._verticalDestinationSpacingM3 / 2L)) : null));
                    indicatorOffsetLocal = new global::Doroti.Ui.Offset(((((_RailDestination__navigation_rail)this.widget).minWidth / 2L) + ((global::Doroti.Framework.Painting.EdgeInsets)destinationPadding).left), (((Navigation_railLibrary._verticalDestinationSpacingM3 / 2L) + ((global::Doroti.Framework.Painting.EdgeInsets)destinationPadding).top) + indicatorVerticalOffset));
                    global::Doroti.Framework.Widgets.Widget iconPart = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Column(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection28264 = new List<global::Doroti.Framework.Widgets.Widget>(); var __collectionElement28286 = spacing; if (__collectionElement28286 is { } __nonNullCollectionElement28286) { __collection28264.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement28286)); } __collection28264.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(width: ((_RailDestination__navigation_rail)this.widget).minWidth, height: (material3 ? null : ((_RailDestination__navigation_rail)this.widget).minWidth), child: new global::Doroti.Framework.Widgets.Center(child: new _AddIndicator__navigation_rail(addIndicator: ((_RailDestination__navigation_rail)this.widget).useIndicator, indicatorColor: ((_RailDestination__navigation_rail)this.widget).indicatorColor, indicatorShape: ((_RailDestination__navigation_rail)this.widget).indicatorShape, isCircular: !material3, indicatorAnimation: ((_RailDestination__navigation_rail)this.widget).destinationAnimation, child: themedIcon))))); var __collectionElement28857 = spacing; if (__collectionElement28857 is { } __nonNullCollectionElement28857) { __collection28264.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement28857)); } return __collection28264; }))()));
                    if ((((_RailDestination__navigation_rail)this.widget).extendedTransitionAnimation.value == 0L))
                    {
                        content = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: (((_RailDestination__navigation_rail)this.widget).padding ?? global::Doroti.Framework.Painting.EdgeInsets.zero), child: new global::Doroti.Framework.Widgets.Stack(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(iconPart), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(global::Doroti.Framework.Widgets.SizedBox.CreateShrink(child: global::Doroti.Framework.Widgets.Visibility.CreateMaintain(visible: false, child: ((_RailDestination__navigation_rail)this.widget).label))) })));
                    }
                    else
                    {
                        global::Doroti.Framework.Animation.Animation<double> labelFadeAnimation = ((global::Doroti.Framework.Animation.Animation<double>)(object?)((_RailDestination__navigation_rail)this.widget).extendedTransitionAnimation.drive(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Interval(0.0, 0.25))));
                        applyXOffsetLocal = true;
                        content = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: (((_RailDestination__navigation_rail)this.widget).padding ?? global::Doroti.Framework.Painting.EdgeInsets.zero), child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((_RailDestination__navigation_rail)this.widget).minWidth, ((_RailDestination__navigation_rail)this.widget).minExtendedWidth, ((_RailDestination__navigation_rail)this.widget).extendedTransitionAnimation.value))), child: new global::Doroti.Framework.Widgets.ClipRect(child: new global::Doroti.Framework.Widgets.Row(mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(iconPart), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: new global::Doroti.Framework.Widgets.Align(heightFactor: 1.0, widthFactor: ((_RailDestination__navigation_rail)this.widget).extendedTransitionAnimation.value, alignment: global::Doroti.Framework.Painting.AlignmentDirectional.centerStart, child: new global::Doroti.Framework.Widgets.FadeTransition(alwaysIncludeSemantics: true, opacity: labelFadeAnimation, child: styledLabel)))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(width: (Navigation_railLibrary._horizontalDestinationPadding * ((_RailDestination__navigation_rail)this.widget).extendedTransitionAnimation.value))) })))));
                    }
                    break;
                }
            case NavigationRailLabelType.selected:
                {
                    double appearingAnimationValue = (1L - ((global::Doroti.Framework.Animation.CurvedAnimation)this._positionAnimation).value);
                    double verticalPadding = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(Navigation_railLibrary._verticalDestinationPaddingNoLabel, Navigation_railLibrary._verticalDestinationPaddingWithLabel, appearingAnimationValue));
                    var interval = (((_RailDestination__navigation_rail)this.widget).selected ? new global::Doroti.Framework.Animation.Interval(0.25, 0.75) : new global::Doroti.Framework.Animation.Interval(0.75, 1.0));
                    global::Doroti.Framework.Animation.Animation<double> labelFadeAnimationLocal = ((global::Doroti.Framework.Animation.Animation<double>)(object?)((_RailDestination__navigation_rail)this.widget).destinationAnimation.drive(new global::Doroti.Framework.Animation.CurveTween(curve: interval)));
                    double minHeightLocal = (material3 ? 0 : ((_RailDestination__navigation_rail)this.widget).minWidth);
                    global::Doroti.Framework.Widgets.Widget topSpacing = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.SizedBox(height: (material3 ? 0 : verticalPadding)));
                    global::Doroti.Framework.Widgets.Widget labelSpacing = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.SizedBox(height: (material3 ? DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(0L, Navigation_railLibrary._verticalIconLabelSpacingM3, appearingAnimationValue)) : 0)));
                    global::Doroti.Framework.Widgets.Widget bottomSpacing = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.SizedBox(height: (material3 ? Navigation_railLibrary._verticalDestinationSpacingM3 : verticalPadding)));
                    double indicatorHorizontalPadding = (((((global::Doroti.Framework.Painting.EdgeInsets)destinationPadding).left / 2L)) - ((((global::Doroti.Framework.Painting.EdgeInsets)destinationPadding).right / 2L)));
                    double indicatorVerticalPadding = ((global::Doroti.Framework.Painting.EdgeInsets)destinationPadding).top;
                    indicatorOffsetLocal = new global::Doroti.Ui.Offset(((((_RailDestination__navigation_rail)this.widget).minWidth / 2L) + indicatorHorizontalPadding), (indicatorVerticalPadding + indicatorVerticalOffset));
                    if ((((_RailDestination__navigation_rail)this.widget).minWidth < DartRuntimePrimitives.RequireValue(new _NavigationRailDefaultsM2__navigation_rail(context).minWidth)))
                    {
                        indicatorOffsetLocal = new global::Doroti.Ui.Offset(((((_RailDestination__navigation_rail)this.widget).minWidth / 2L) + Navigation_railLibrary._horizontalDestinationSpacingM3), (indicatorVerticalPadding + indicatorVerticalOffset));
                    }
                    content = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: ((_RailDestination__navigation_rail)this.widget).minWidth, minHeight: minHeightLocal), child: new global::Doroti.Framework.Widgets.Padding(padding: (((_RailDestination__navigation_rail)this.widget).padding ?? global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: Navigation_railLibrary._horizontalDestinationPadding)), child: new global::Doroti.Framework.Widgets.ClipRect(child: new global::Doroti.Framework.Widgets.Column(mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, mainAxisAlignment: global::Doroti.Framework.Rendering.MainAxisAlignment.center, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(topSpacing), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _AddIndicator__navigation_rail(addIndicator: ((_RailDestination__navigation_rail)this.widget).useIndicator, indicatorColor: ((_RailDestination__navigation_rail)this.widget).indicatorColor, indicatorShape: ((_RailDestination__navigation_rail)this.widget).indicatorShape, isCircular: false, indicatorAnimation: ((_RailDestination__navigation_rail)this.widget).destinationAnimation, child: themedIcon)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(labelSpacing), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.Alignment.topCenter, heightFactor: appearingAnimationValue, widthFactor: 1.0, child: new global::Doroti.Framework.Widgets.FadeTransition(alwaysIncludeSemantics: true, opacity: labelFadeAnimationLocal, child: styledLabel))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(bottomSpacing) })))));
                    break;
                }
            case NavigationRailLabelType.all:
                {
                    double minHeightAlternate = (material3 ? 0 : ((_RailDestination__navigation_rail)this.widget).minWidth);
                    global::Doroti.Framework.Widgets.Widget topSpacingLocal = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.SizedBox(height: (material3 ? 0 : Navigation_railLibrary._verticalDestinationPaddingWithLabel)));
                    global::Doroti.Framework.Widgets.Widget labelSpacingLocal = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.SizedBox(height: (material3 ? Navigation_railLibrary._verticalIconLabelSpacingM3 : 0)));
                    global::Doroti.Framework.Widgets.Widget bottomSpacingLocal = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.SizedBox(height: (material3 ? Navigation_railLibrary._verticalDestinationSpacingM3 : Navigation_railLibrary._verticalDestinationPaddingWithLabel)));
                    double indicatorHorizontalPaddingLocal = (((((global::Doroti.Framework.Painting.EdgeInsets)destinationPadding).left / 2L)) - ((((global::Doroti.Framework.Painting.EdgeInsets)destinationPadding).right / 2L)));
                    double indicatorVerticalPaddingLocal = ((global::Doroti.Framework.Painting.EdgeInsets)destinationPadding).top;
                    indicatorOffsetLocal = new global::Doroti.Ui.Offset(((((_RailDestination__navigation_rail)this.widget).minWidth / 2L) + indicatorHorizontalPaddingLocal), (indicatorVerticalPaddingLocal + indicatorVerticalOffset));
                    if ((((_RailDestination__navigation_rail)this.widget).minWidth < DartRuntimePrimitives.RequireValue(new _NavigationRailDefaultsM2__navigation_rail(context).minWidth)))
                    {
                        indicatorOffsetLocal = new global::Doroti.Ui.Offset(((((_RailDestination__navigation_rail)this.widget).minWidth / 2L) + Navigation_railLibrary._horizontalDestinationSpacingM3), (indicatorVerticalPaddingLocal + indicatorVerticalOffset));
                    }
                    content = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: ((_RailDestination__navigation_rail)this.widget).minWidth, minHeight: minHeightAlternate), child: new global::Doroti.Framework.Widgets.Padding(padding: (((_RailDestination__navigation_rail)this.widget).padding ?? global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: Navigation_railLibrary._horizontalDestinationPadding)), child: new global::Doroti.Framework.Widgets.Column(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(topSpacingLocal), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _AddIndicator__navigation_rail(addIndicator: ((_RailDestination__navigation_rail)this.widget).useIndicator, indicatorColor: ((_RailDestination__navigation_rail)this.widget).indicatorColor, indicatorShape: ((_RailDestination__navigation_rail)this.widget).indicatorShape, isCircular: false, indicatorAnimation: ((_RailDestination__navigation_rail)this.widget).destinationAnimation, child: themedIcon)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(labelSpacingLocal), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(styledLabel), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(bottomSpacingLocal) }))));
                    break;
                }
        }
        ColorScheme colors = Theme.of(context).colorScheme;
        bool primaryColorAlphaModified = (colors.primary.alpha < 255.0);
        global::Doroti.Ui.Color effectiveSplashColor = ((global::Doroti.Ui.Color)(object?)(primaryColorAlphaModified ? colors.primary : colors.primary.withOpacity(0.12)));
        global::Doroti.Ui.Color effectiveHoverColor = ((global::Doroti.Ui.Color)(object?)(primaryColorAlphaModified ? colors.primary : colors.primary.withOpacity(0.04)));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(container: true, selected: ((_RailDestination__navigation_rail)this.widget).selected, child: new global::Doroti.Framework.Widgets.Stack(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new Material(type: MaterialType.transparency, child: new _IndicatorInkWell__navigation_rail(onTap: ((global::System.Action)(((_RailDestination__navigation_rail)this.widget).disabled ? null : ((_RailDestination__navigation_rail)this.widget).onTap)), borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular((((_RailDestination__navigation_rail)this.widget).minWidth / 2.0))), customBorder: ((_RailDestination__navigation_rail)this.widget).indicatorShape, splashColor: effectiveSplashColor, hoverColor: effectiveHoverColor, useMaterial3: material3, indicatorOffset: indicatorOffsetLocal, applyXOffset: applyXOffsetLocal, textDirection: textDirectionLocal, child: content))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(label: ((_RailDestination__navigation_rail)this.widget).indexLabel)) })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _IndicatorInkWell__navigation_rail : InkResponse
{
    public virtual bool useMaterial3 { get; private set; } = default!;
    public virtual Offset indicatorOffset { get; private set; } = default!;
    public virtual bool applyXOffset { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;

    internal _IndicatorInkWell__navigation_rail(global::Doroti.Framework.Widgets.Widget? child = null, global::System.Action? onTap = null, global::Doroti.Framework.Painting.ShapeBorder? customBorder = null, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, Color? splashColor = null, Color? hoverColor = null, bool useMaterial3 = default!, Offset indicatorOffset = default!, bool applyXOffset = default!, TextDirection textDirection = default!) : base(child: child, onTap: onTap, splashColor: splashColor, hoverColor: hoverColor, containedInkWell: true, highlightShape: global::Doroti.Framework.Painting.BoxShape.rectangle, borderRadius: (useMaterial3 ? null : borderRadius), customBorder: (useMaterial3 ? customBorder : null))
    {
        this.useMaterial3 = useMaterial3;
        this.indicatorOffset = indicatorOffset;
        this.applyXOffset = applyXOffset;
        this.textDirection = textDirection;
    }

    public virtual RectCallback? getRectCallback(global::Doroti.Framework.Rendering.RenderBox referenceBox)
    {
        if (this.useMaterial3)
        {
            double boxWidth = ((global::Doroti.Framework.Rendering.RenderBox)referenceBox).size.width;
            double indicatorHorizontalCenter = (this.applyXOffset ? this.indicatorOffset.dx : (boxWidth / 2L));
            if ((object.Equals(this.textDirection, TextDirection.rtl)))
            {
                indicatorHorizontalCenter = (boxWidth - indicatorHorizontalCenter);
            }
            return ((RectCallback?)(object?)(() =>
            {
                return global::Doroti.Ui.Rect.fromLTWH((indicatorHorizontalCenter - ((Navigation_railLibrary._kCircularIndicatorDiameter / 2L))), this.indicatorOffset.dy, Navigation_railLibrary._kCircularIndicatorDiameter, Navigation_barLibrary._kIndicatorHeight);
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AddIndicator__navigation_rail : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual bool addIndicator { get; private set; } = default!;
    public virtual bool isCircular { get; private set; } = default!;
    public virtual Color? indicatorColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? indicatorShape { get; private set; }
    public virtual global::Doroti.Framework.Animation.Animation<double> indicatorAnimation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;

    internal _AddIndicator__navigation_rail(bool addIndicator, bool isCircular, Color? indicatorColor, global::Doroti.Framework.Painting.ShapeBorder? indicatorShape, global::Doroti.Framework.Animation.Animation<double> indicatorAnimation, global::Doroti.Framework.Widgets.Widget child)
    {
        this.addIndicator = addIndicator;
        this.isCircular = isCircular;
        this.indicatorColor = indicatorColor;
        this.indicatorShape = indicatorShape;
        this.indicatorAnimation = indicatorAnimation;
        this.child = child;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if (!this.addIndicator)
        {
            return this.child;
        }
        global::Doroti.Framework.Widgets.Widget indicator = default!;
        if (this.isCircular)
        {
            indicator = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new NavigationIndicator(animation: this.indicatorAnimation, height: Navigation_railLibrary._kCircularIndicatorDiameter, width: Navigation_railLibrary._kCircularIndicatorDiameter, borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular((Navigation_railLibrary._kCircularIndicatorDiameter / 2L))), color: this.indicatorColor));
        }
        else
        {
            indicator = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new NavigationIndicator(animation: this.indicatorAnimation, width: Navigation_railLibrary._kCircularIndicatorDiameter, shape: this.indicatorShape, color: this.indicatorColor));
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Stack(alignment: global::Doroti.Framework.Painting.Alignment.center, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(indicator), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(this.child) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public enum NavigationRailLabelType
{
    none,
    selected,
    all
}

public class NavigationRailDestination
{
    public virtual global::Doroti.Framework.Widgets.Widget icon { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget selectedIcon { get; private set; } = default!;
    public virtual Color? indicatorColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? indicatorShape { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget label { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual bool disabled { get; private set; } = default!;

    public NavigationRailDestination(global::Doroti.Framework.Widgets.Widget icon, global::Doroti.Framework.Widgets.Widget? selectedIcon = null, Color? indicatorColor = null, global::Doroti.Framework.Painting.ShapeBorder? indicatorShape = null, global::Doroti.Framework.Widgets.Widget label = default!, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, bool disabled = false)
    {
        this.icon = icon;
        this.indicatorColor = indicatorColor;
        this.indicatorShape = indicatorShape;
        this.label = label;
        this.padding = padding;
        this.disabled = disabled;
        this.selectedIcon = (selectedIcon ?? icon);
    }

}

internal class _ExtendedNavigationRailAnimation__navigation_rail : global::Doroti.Framework.Widgets.InheritedWidget
{
    public virtual global::Doroti.Framework.Animation.Animation<double> animation { get; private set; } = default!;

    internal _ExtendedNavigationRailAnimation__navigation_rail(global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Widgets.Widget child) : base(child: child)
    {
        this.animation = animation;
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => (!object.Equals(this.animation, ((_ExtendedNavigationRailAnimation__navigation_rail)oldWidget).animation));
}

public static partial class Navigation_railLibrary
{
    internal static double _horizontalDestinationPadding = 8.0;
}

public static partial class Navigation_railLibrary
{
    internal static double _verticalDestinationPaddingNoLabel = 24.0;
}

public static partial class Navigation_railLibrary
{
    internal static double _verticalDestinationPaddingWithLabel = 16.0;
}

public static partial class Navigation_railLibrary
{
    internal static global::Doroti.Framework.Widgets.Widget _verticalSpacer = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.SizedBox(height: 8.0));
}

public static partial class Navigation_railLibrary
{
    internal static double _verticalIconLabelSpacingM3 = 4.0;
}

public static partial class Navigation_railLibrary
{
    internal static double _verticalDestinationSpacingM3 = 12.0;
}

public static partial class Navigation_railLibrary
{
    internal static double _horizontalDestinationSpacingM3 = 12.0;
}

internal class _NavigationRailDefaultsM2__navigation_rail : NavigationRailThemeData
{
    internal virtual ThemeData _theme { get; private set; } = default!;
    internal virtual ColorScheme _colors { get; private set; } = default!;

    internal _NavigationRailDefaultsM2__navigation_rail(global::Doroti.Framework.Widgets.BuildContext context) : base(elevation: 0, groupAlignment: -1, labelType: NavigationRailLabelType.none, useIndicator: false, minWidth: 72.0, minExtendedWidth: 256)
    {
        this._theme = Theme.of(context);
        this._colors = Theme.of(context).colorScheme;
    }

    public virtual global::Doroti.Ui.Color? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.surface);
    public virtual global::Doroti.Framework.Painting.TextStyle? unselectedLabelTextStyle
    {
        get
        {
            return ((global::Doroti.Framework.Painting.TextStyle?)(object?)this._theme.textTheme.bodyLarge!.copyWith(color: this._colors.onSurface.withOpacity(0.64)));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Painting.TextStyle? selectedLabelTextStyle
    {
        get
        {
            return ((global::Doroti.Framework.Painting.TextStyle?)(object?)this._theme.textTheme.bodyLarge!.copyWith(color: this._colors.primary));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.IconThemeData? unselectedIconTheme
    {
        get
        {
            return new global::Doroti.Framework.Widgets.IconThemeData(size: 24.0, color: this._colors.onSurface, opacity: 0.64);
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.IconThemeData? selectedIconTheme
    {
        get
        {
            return new global::Doroti.Framework.Widgets.IconThemeData(size: 24.0, color: this._colors.primary, opacity: 1.0);
            return default!;
        }
    }
}

internal class _NavigationRailDefaultsM3__navigation_rail : NavigationRailThemeData
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

    internal _NavigationRailDefaultsM3__navigation_rail(global::Doroti.Framework.Widgets.BuildContext context) : base(elevation: 0.0, groupAlignment: -1, labelType: NavigationRailLabelType.none, useIndicator: true, minWidth: 80.0, minExtendedWidth: 256)
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.surface);
    public virtual global::Doroti.Framework.Painting.TextStyle? unselectedLabelTextStyle
    {
        get
        {
            return ((global::Doroti.Framework.Painting.TextStyle?)(object?)this._textTheme.labelMedium!.copyWith(color: this._colors.onSurface));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Painting.TextStyle? selectedLabelTextStyle
    {
        get
        {
            return ((global::Doroti.Framework.Painting.TextStyle?)(object?)this._textTheme.labelMedium!.copyWith(color: this._colors.onSurface));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.IconThemeData? unselectedIconTheme
    {
        get
        {
            return new global::Doroti.Framework.Widgets.IconThemeData(size: 24.0, color: this._colors.onSurfaceVariant);
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.IconThemeData? selectedIconTheme
    {
        get
        {
            return new global::Doroti.Framework.Widgets.IconThemeData(size: 24.0, color: this._colors.onSecondaryContainer);
            return default!;
        }
    }
    public virtual global::Doroti.Ui.Color? indicatorColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.secondaryContainer);
    public override global::Doroti.Framework.Painting.ShapeBorder? indicatorShape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.ShapeBorder>(new global::Doroti.Framework.Painting.StadiumBorder());
}
