// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/navigation_rail.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class Navigation_railLibrary
{
    internal static double _kCircularIndicatorDiameter = 56;
    // Compact custom rails keep the same indicator offset below this width.
    internal const double _compactDestinationWidth = 72.0;
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
        System.Diagnostics.Debug.Assert((selectedIndex is null) || (0L <= DartRuntimePrimitives.RequireValue(selectedIndex)) && (DartRuntimePrimitives.RequireValue(selectedIndex) < checked(destinations.Count)));
        System.Diagnostics.Debug.Assert((elevation is null) || (DartRuntimePrimitives.RequireValue(elevation) > 0L));
        System.Diagnostics.Debug.Assert((minWidth is null) || (DartRuntimePrimitives.RequireValue(minWidth) > 0L));
        System.Diagnostics.Debug.Assert((minExtendedWidth is null) || (DartRuntimePrimitives.RequireValue(minExtendedWidth) > 0L));
        System.Diagnostics.Debug.Assert((minWidth is null) || (minExtendedWidth is null) || (minExtendedWidth >= DartRuntimePrimitives.RequireValue(minWidth)));
        System.Diagnostics.Debug.Assert(!extended || (labelType is null) || Equals(DartRuntimePrimitives.RequireValue(labelType), NavigationRailLabelType.none));
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
                if (_tickers is not null)
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in _tickers!)
                    {
                        if (ticker.isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override void didUpdateWidget(NavigationRail oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (widget.extended != oldWidget.extended)
        {
            if (widget.extended)
            {
                _extendedController.forward();
            }
            else
            {
                _extendedController.reverse();
            }
        }
        if (checked(widget.destinations.Count) != checked((long)oldWidget.destinations.Count))
        {
            _resetState();
            return;
        }
        if (widget.selectedIndex != oldWidget.selectedIndex)
        {
            if (oldWidget.selectedIndex is not null)
            {
                _destinationControllers[(int)DartRuntimePrimitives.RequireValue(oldWidget.selectedIndex)].reverse();
            }
            if (widget.selectedIndex is not null)
            {
                _destinationControllers[(int)DartRuntimePrimitives.RequireValue(widget.selectedIndex)].forward();
            }
            return;
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        NavigationRailThemeData navigationRailTheme = NavigationRailTheme.of(context);
        NavigationRailThemeData defaults = new _NavigationRailDefaultsM3__navigation_rail(context);
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        global::Doroti.Ui.Color backgroundColorLocal = (widget.backgroundColor ?? navigationRailTheme.backgroundColor) ?? defaults.backgroundColor!;
        double elevationLocal = (widget.elevation ?? navigationRailTheme.elevation) ?? DartRuntimePrimitives.RequireValue(defaults.elevation);
        double minWidthLocal = (widget.minWidth ?? navigationRailTheme.minWidth) ?? DartRuntimePrimitives.RequireValue(defaults.minWidth);
        double minExtendedWidthLocal = (widget.minExtendedWidth ?? navigationRailTheme.minExtendedWidth) ?? DartRuntimePrimitives.RequireValue(defaults.minExtendedWidth);
        global::Doroti.Framework.Painting.TextStyle unselectedLabelTextStyleLocal = (widget.unselectedLabelTextStyle ?? navigationRailTheme.unselectedLabelTextStyle) ?? defaults.unselectedLabelTextStyle!;
        global::Doroti.Framework.Painting.TextStyle selectedLabelTextStyleLocal = (widget.selectedLabelTextStyle ?? navigationRailTheme.selectedLabelTextStyle) ?? defaults.selectedLabelTextStyle!;
        global::Doroti.Framework.Widgets.IconThemeData unselectedIconThemeLocal = (widget.unselectedIconTheme ?? navigationRailTheme.unselectedIconTheme) ?? defaults.unselectedIconTheme!;
        global::Doroti.Framework.Widgets.IconThemeData selectedIconThemeLocal = (widget.selectedIconTheme ?? navigationRailTheme.selectedIconTheme) ?? defaults.selectedIconTheme!;
        double groupAlignmentLocal = (widget.groupAlignment ?? navigationRailTheme.groupAlignment) ?? DartRuntimePrimitives.RequireValue(defaults.groupAlignment);
        NavigationRailLabelType labelTypeLocal = (widget.labelType ?? navigationRailTheme.labelType) ?? DartRuntimePrimitives.RequireValue(defaults.labelType);
        bool useIndicatorLocal = (widget.useIndicator ?? navigationRailTheme.useIndicator) ?? DartRuntimePrimitives.RequireValue(defaults.useIndicator);
        global::Doroti.Ui.Color? indicatorColorLocal = (widget.indicatorColor ?? navigationRailTheme.indicatorColor) ?? defaults.indicatorColor;
        global::Doroti.Framework.Painting.ShapeBorder? indicatorShapeLocal = (widget.indicatorShape ?? navigationRailTheme.indicatorShape) ?? defaults.indicatorShape;
        global::Doroti.Framework.Widgets.IconThemeData effectiveUnselectedIconTheme = unselectedIconThemeLocal;
        var isRTLDirection = Equals(Directionality.of(context), TextDirection.rtl);
        global::Doroti.Framework.Widgets.Widget mainGroup = new global::Doroti.Framework.Widgets.Column(mainAxisSize: (widget.mainAxisAlignment is not null) ? MainAxisSize.max : MainAxisSize.min, mainAxisAlignment: widget.mainAxisAlignment ?? MainAxisAlignment.start, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() =>
        {
            var __collection20149 = new List<global::Doroti.Framework.Widgets.Widget>(); if (!widget.leadingAtTop && (widget.leading is not null)) { __collection20149.AddRange(new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(widget.leading!), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(Navigation_railLibrary._verticalSpacer) }); }
            for (long i = 0L; i < checked(widget.destinations.Count); i += 1L)
            {
                var destinationIndex = i;
                __collection20149.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _RailDestination__navigation_rail(minWidth: minWidthLocal, minExtendedWidth: minExtendedWidthLocal, extendedTransitionAnimation: _extendedAnimation, selected: widget.selectedIndex == i, icon: (widget.selectedIndex == i) ? widget.destinations[(int)i].selectedIcon : widget.destinations[(int)i].icon, label: widget.destinations[(int)i].label, destinationAnimation: _destinationAnimations[(int)i], labelType: labelTypeLocal, iconTheme: (widget.selectedIndex == i) ? selectedIconThemeLocal : effectiveUnselectedIconTheme, labelTextStyle: (widget.selectedIndex == i) ? selectedLabelTextStyleLocal : unselectedLabelTextStyleLocal, padding: widget.destinations[(int)i].padding, useIndicator: useIndicatorLocal, indicatorColor: useIndicatorLocal ? indicatorColorLocal : null, indicatorShape: useIndicatorLocal ? indicatorShapeLocal : null, onTap: () =>
                {
                    if (widget.onDestinationSelected is not null)
                    {
                        widget.onDestinationSelected!(destinationIndex);
                    }
                }, indexLabel: localizations.tabLabel(tabIndex: i + 1L, tabCount: checked(widget.destinations.Count)), disabled: widget.destinations[(int)i].disabled)));
            }
            if (!widget.trailingAtBottom && (widget.trailing is not null)) { __collection20149.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(widget.trailing!)); }
            return __collection20149;
        }))());
        if (widget.scrollable)
        {
            mainGroup = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SingleChildScrollView(child: mainGroup));
        }
        return new global::Doroti.Framework.Widgets.Semantics(container: true, child: new _ExtendedNavigationRailAnimation__navigation_rail(animation: _extendedAnimation, child: new global::Doroti.Framework.Widgets.Semantics(explicitChildNodes: true, child: new Material(elevation: elevationLocal, color: backgroundColorLocal, child: new global::Doroti.Framework.Widgets.SafeArea(right: isRTLDirection, left: !isRTLDirection, child: new global::Doroti.Framework.Widgets.Column(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection22353 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection22353.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(Navigation_railLibrary._verticalSpacer)); if (widget.leadingAtTop && (widget.leading is not null)) { __collection22353.AddRange(new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(widget.leading!), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(Navigation_railLibrary._verticalSpacer) }); } __collection22353.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: new global::Doroti.Framework.Widgets.Align(alignment: new global::Doroti.Framework.Painting.Alignment(0, groupAlignmentLocal), child: mainGroup)))); if (widget.trailingAtBottom && (widget.trailing is not null)) { __collection22353.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(widget.trailing!)); } return __collection22353; }))()))))));
    }

    internal virtual void _disposeControllers()
    {
        foreach (global::Doroti.Framework.Animation.AnimationController controller in _destinationControllers)
        {
            controller.dispose();
        }
        _extendedController.dispose();
        _extendedAnimation.dispose();
    }

    internal virtual void _initControllers()
    {
        _destinationControllers = new List<global::Doroti.Framework.Animation.AnimationController>(Enumerable.Select(Enumerable.Range(0, checked((int)checked((long)widget.destinations.Count))), (index) =>
        {
            return ((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
            {
                var __cascade = new global::Doroti.Framework.Animation.AnimationController(duration: ThemeLibrary.kThemeAnimationDuration, vsync: this);
                __cascade.addListener(_rebuild);
                return __cascade;
            }))();
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        _destinationAnimations = _destinationControllers.map<global::Doroti.Framework.Animation.AnimationController, global::Doroti.Framework.Animation.Animation<double>>((controller) => controller.view).ToList();
        if (widget.selectedIndex is not null)
        {
            _destinationControllers[(int)DartRuntimePrimitives.RequireValue(widget.selectedIndex)].value = 1.0;
        }
        _extendedController = new global::Doroti.Framework.Animation.AnimationController(duration: ThemeLibrary.kThemeAnimationDuration, vsync: this, value: widget.extended ? 1.0 : 0.0);
        _extendedAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: _extendedController, curve: Curves.easeInOut);
        _extendedController.addListener(() =>
        {
            _rebuild();
        });
    }

    internal virtual void _resetState()
    {
        _disposeControllers();
        _initControllers();
    }

    internal virtual void _rebuild()
    {
        setState(() =>
        {
        });
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider(onTick, this, debugLabel: Foundation.ConstantsLibrary.kDebugMode ? $"created by {DiagnosticsLibrary.describeIdentity(this)}" : null);
    __cascade.muted = !values.enabled;
    __cascade.forceFrames = values.forceFrames;
    return __cascade;
}))();
        _tickers!.Add(result);
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => _tickers is not null);
        DartRuntimePrimitives.Assert(() => _tickers!.Contains(ticker));
        _tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if (_tickers is not null)
        {
            TickerModeData values = _tickerModeNotifier!.value;
            bool mutedLocal = !values.enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", _tickers, description: (_tickers is not null) ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}" : null, defaultValue: default));
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
        if (!Equals(widget.destinationAnimation, oldWidget.destinationAnimation))
        {
            _positionAnimation.dispose();
            _setPositionAnimation();
        }
    }

    internal virtual void _setPositionAnimation()
    {
        _positionAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: new global::Doroti.Framework.Animation.ReverseAnimation(widget.destinationAnimation), curve: Curves.easeInOut, reverseCurve: Curves.easeInOut.flipped);
    }

    public override void dispose()
    {
        _positionAnimation.dispose();
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => widget.useIndicator || (widget.indicatorColor is null), () => (object?)"[NavigationRail.indicatorColor] does not have an effect when [NavigationRail.useIndicator] is false");
        ThemeData theme = Theme.of(context);
        global::Doroti.Ui.TextDirection textDirectionLocal = Directionality.of(context);
        global::Doroti.Framework.Painting.EdgeInsets destinationPadding = (widget.padding ?? EdgeInsets.zero).resolve(textDirectionLocal);
        global::Doroti.Ui.Offset indicatorOffsetLocal = default!;
        var applyXOffsetLocal = false;
        global::Doroti.Framework.Widgets.Widget themedIcon = new global::Doroti.Framework.Widgets.IconTheme(data: widget.disabled ? widget.iconTheme.copyWith(color: theme.colorScheme.onSurface.withOpacity(0.38)) : widget.iconTheme, child: widget.icon);
        global::Doroti.Framework.Widgets.Widget styledLabel = new global::Doroti.Framework.Widgets.DefaultTextStyle(style: widget.disabled ? widget.labelTextStyle.copyWith(color: theme.colorScheme.onSurface.withOpacity(0.38)) : widget.labelTextStyle, child: widget.label);
        global::Doroti.Framework.Widgets.Widget content = default!;
        bool isLargeIconSize = (widget.iconTheme.size is not null) && (DartRuntimePrimitives.RequireValue(widget.iconTheme.size) > Navigation_barLibrary._kIndicatorHeight);
        double indicatorVerticalOffset = isLargeIconSize ? ((DartRuntimePrimitives.RequireValue(widget.iconTheme.size) - Navigation_barLibrary._kIndicatorHeight) / 2L) : 0;
        switch (widget.labelType)
        {
            case NavigationRailLabelType.none:
                {
                    global::Doroti.Framework.Widgets.Widget? spacing = (global::Doroti.Framework.Widgets.Widget?)new global::Doroti.Framework.Widgets.SizedBox(height: Navigation_railLibrary._verticalDestinationSpacingM3 / 2L);
                    indicatorOffsetLocal = new global::Doroti.Ui.Offset((widget.minWidth / 2L) + destinationPadding.left, (Navigation_railLibrary._verticalDestinationSpacingM3 / 2L) + destinationPadding.top + indicatorVerticalOffset);
                    global::Doroti.Framework.Widgets.Widget iconPart = new global::Doroti.Framework.Widgets.Column(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection28264 = new List<global::Doroti.Framework.Widgets.Widget>(); var __collectionElement28286 = spacing; if (__collectionElement28286 is { } __nonNullCollectionElement28286) { __collection28264.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement28286)); } __collection28264.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(width: widget.minWidth, height: null, child: new global::Doroti.Framework.Widgets.Center(child: new _AddIndicator__navigation_rail(addIndicator: widget.useIndicator, indicatorColor: widget.indicatorColor, indicatorShape: widget.indicatorShape, isCircular: false, indicatorAnimation: widget.destinationAnimation, child: themedIcon))))); var __collectionElement28857 = spacing; if (__collectionElement28857 is { } __nonNullCollectionElement28857) { __collection28264.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement28857)); } return __collection28264; }))());
                    if (widget.extendedTransitionAnimation.value == 0L)
                    {
                        content = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: widget.padding ?? EdgeInsets.zero, child: new global::Doroti.Framework.Widgets.Stack(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(iconPart), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(SizedBox.CreateShrink(child: Visibility.CreateMaintain(visible: false, child: widget.label))) })));
                    }
                    else
                    {
                        global::Doroti.Framework.Animation.Animation<double> labelFadeAnimation = widget.extendedTransitionAnimation.drive(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Interval(0.0, 0.25)));
                        applyXOffsetLocal = true;
                        content = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: widget.padding ?? EdgeInsets.zero, child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(widget.minWidth, widget.minExtendedWidth, widget.extendedTransitionAnimation.value))), child: new global::Doroti.Framework.Widgets.ClipRect(child: new global::Doroti.Framework.Widgets.Row(mainAxisSize: MainAxisSize.min, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(iconPart), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: new global::Doroti.Framework.Widgets.Align(heightFactor: 1.0, widthFactor: widget.extendedTransitionAnimation.value, alignment: AlignmentDirectional.centerStart, child: new global::Doroti.Framework.Widgets.FadeTransition(alwaysIncludeSemantics: true, opacity: labelFadeAnimation, child: styledLabel)))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(width: Navigation_railLibrary._horizontalDestinationPadding * widget.extendedTransitionAnimation.value)) })))));
                    }
                    break;
                }
            case NavigationRailLabelType.selected:
                {
                    double appearingAnimationValue = 1L - _positionAnimation.value;
                    double verticalPadding = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(Navigation_railLibrary._verticalDestinationPaddingNoLabel, Navigation_railLibrary._verticalDestinationPaddingWithLabel, appearingAnimationValue));
                    var interval = widget.selected ? new global::Doroti.Framework.Animation.Interval(0.25, 0.75) : new global::Doroti.Framework.Animation.Interval(0.75, 1.0);
                    global::Doroti.Framework.Animation.Animation<double> labelFadeAnimationLocal = widget.destinationAnimation.drive(new global::Doroti.Framework.Animation.CurveTween(curve: interval));
                    double minHeightLocal = 0;
                    global::Doroti.Framework.Widgets.Widget topSpacing = new global::Doroti.Framework.Widgets.SizedBox(height: 0);
                    global::Doroti.Framework.Widgets.Widget labelSpacing = new global::Doroti.Framework.Widgets.SizedBox(height: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(0L, Navigation_railLibrary._verticalIconLabelSpacingM3, appearingAnimationValue)));
                    global::Doroti.Framework.Widgets.Widget bottomSpacing = new global::Doroti.Framework.Widgets.SizedBox(height: Navigation_railLibrary._verticalDestinationSpacingM3);
                    double indicatorHorizontalPadding = destinationPadding.left / 2L - destinationPadding.right / 2L;
                    double indicatorVerticalPadding = destinationPadding.top;
                    indicatorOffsetLocal = new global::Doroti.Ui.Offset((widget.minWidth / 2L) + indicatorHorizontalPadding, indicatorVerticalPadding + indicatorVerticalOffset);
                    if (widget.minWidth < Navigation_railLibrary._compactDestinationWidth)
                    {
                        indicatorOffsetLocal = new global::Doroti.Ui.Offset((widget.minWidth / 2L) + Navigation_railLibrary._horizontalDestinationSpacingM3, indicatorVerticalPadding + indicatorVerticalOffset);
                    }
                    content = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: widget.minWidth, minHeight: minHeightLocal), child: new global::Doroti.Framework.Widgets.Padding(padding: widget.padding ?? EdgeInsets.CreateSymmetric(horizontal: Navigation_railLibrary._horizontalDestinationPadding), child: new global::Doroti.Framework.Widgets.ClipRect(child: new global::Doroti.Framework.Widgets.Column(mainAxisSize: MainAxisSize.min, mainAxisAlignment: MainAxisAlignment.center, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(topSpacing), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _AddIndicator__navigation_rail(addIndicator: widget.useIndicator, indicatorColor: widget.indicatorColor, indicatorShape: widget.indicatorShape, isCircular: false, indicatorAnimation: widget.destinationAnimation, child: themedIcon)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(labelSpacing), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Align(alignment: Alignment.topCenter, heightFactor: appearingAnimationValue, widthFactor: 1.0, child: new global::Doroti.Framework.Widgets.FadeTransition(alwaysIncludeSemantics: true, opacity: labelFadeAnimationLocal, child: styledLabel))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(bottomSpacing) })))));
                    break;
                }
            case NavigationRailLabelType.all:
                {
                    double minHeightAlternate = 0;
                    global::Doroti.Framework.Widgets.Widget topSpacingLocal = new global::Doroti.Framework.Widgets.SizedBox(height: 0);
                    global::Doroti.Framework.Widgets.Widget labelSpacingLocal = new global::Doroti.Framework.Widgets.SizedBox(height: Navigation_railLibrary._verticalIconLabelSpacingM3);
                    global::Doroti.Framework.Widgets.Widget bottomSpacingLocal = new global::Doroti.Framework.Widgets.SizedBox(height: Navigation_railLibrary._verticalDestinationSpacingM3);
                    double indicatorHorizontalPaddingLocal = destinationPadding.left / 2L - destinationPadding.right / 2L;
                    double indicatorVerticalPaddingLocal = destinationPadding.top;
                    indicatorOffsetLocal = new global::Doroti.Ui.Offset((widget.minWidth / 2L) + indicatorHorizontalPaddingLocal, indicatorVerticalPaddingLocal + indicatorVerticalOffset);
                    if (widget.minWidth < Navigation_railLibrary._compactDestinationWidth)
                    {
                        indicatorOffsetLocal = new global::Doroti.Ui.Offset((widget.minWidth / 2L) + Navigation_railLibrary._horizontalDestinationSpacingM3, indicatorVerticalPaddingLocal + indicatorVerticalOffset);
                    }
                    content = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: widget.minWidth, minHeight: minHeightAlternate), child: new global::Doroti.Framework.Widgets.Padding(padding: widget.padding ?? EdgeInsets.CreateSymmetric(horizontal: Navigation_railLibrary._horizontalDestinationPadding), child: new global::Doroti.Framework.Widgets.Column(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(topSpacingLocal), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _AddIndicator__navigation_rail(addIndicator: widget.useIndicator, indicatorColor: widget.indicatorColor, indicatorShape: widget.indicatorShape, isCircular: false, indicatorAnimation: widget.destinationAnimation, child: themedIcon)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(labelSpacingLocal), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(styledLabel), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(bottomSpacingLocal) }))));
                    break;
                }
        }
        ColorScheme colors = Theme.of(context).colorScheme;
        bool primaryColorAlphaModified = colors.primary.alpha < 255.0;
        global::Doroti.Ui.Color effectiveSplashColor = primaryColorAlphaModified ? colors.primary : colors.primary.withOpacity(0.12);
        global::Doroti.Ui.Color effectiveHoverColor = primaryColorAlphaModified ? colors.primary : colors.primary.withOpacity(0.04);
        return new global::Doroti.Framework.Widgets.Semantics(container: true, selected: widget.selected, child: new global::Doroti.Framework.Widgets.Stack(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new Material(type: MaterialType.transparency, child: new _IndicatorInkWell__navigation_rail(onTap: widget.disabled ? null : widget.onTap, borderRadius: BorderRadius.CreateAll(Radius.circular(widget.minWidth / 2.0)), customBorder: widget.indicatorShape, splashColor: effectiveSplashColor, hoverColor: effectiveHoverColor, indicatorOffset: indicatorOffsetLocal, applyXOffset: applyXOffsetLocal, textDirection: textDirectionLocal, child: content))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(label: widget.indexLabel)) }));
    }

}

internal class _IndicatorInkWell__navigation_rail : InkResponse
{
    public virtual Offset indicatorOffset { get; private set; } = default!;
    public virtual bool applyXOffset { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;

    internal _IndicatorInkWell__navigation_rail(global::Doroti.Framework.Widgets.Widget? child = null, global::System.Action? onTap = null, global::Doroti.Framework.Painting.ShapeBorder? customBorder = null, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, Color? splashColor = null, Color? hoverColor = null, Offset indicatorOffset = default!, bool applyXOffset = default!, TextDirection textDirection = default!) : base(child: child, onTap: onTap, splashColor: splashColor, hoverColor: hoverColor, containedInkWell: true, highlightShape: BoxShape.rectangle, borderRadius: null, customBorder: customBorder)
    {
        this.indicatorOffset = indicatorOffset;
        this.applyXOffset = applyXOffset;
        this.textDirection = textDirection;
    }

    public override global::System.Func<Rect>? getRectCallback(global::Doroti.Framework.Rendering.RenderBox referenceBox)
    {
        {
            double boxWidth = referenceBox.size.width;
            double indicatorHorizontalCenter = applyXOffset ? indicatorOffset.dx : (boxWidth / 2L);
            if (Equals(textDirection, TextDirection.rtl))
            {
                indicatorHorizontalCenter = boxWidth - indicatorHorizontalCenter;
            }
            return (global::System.Func<Rect>?)(object?)(() =>
            {
                return Rect.fromLTWH(indicatorHorizontalCenter - Navigation_railLibrary._kCircularIndicatorDiameter / 2L, indicatorOffset.dy, Navigation_railLibrary._kCircularIndicatorDiameter, Navigation_barLibrary._kIndicatorHeight);
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
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
        if (!addIndicator)
        {
            return child;
        }
        global::Doroti.Framework.Widgets.Widget indicator = default!;
        if (isCircular)
        {
            indicator = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new NavigationIndicator(animation: indicatorAnimation, height: Navigation_railLibrary._kCircularIndicatorDiameter, width: Navigation_railLibrary._kCircularIndicatorDiameter, borderRadius: BorderRadius.CreateAll(Radius.circular(Navigation_railLibrary._kCircularIndicatorDiameter / 2L)), color: indicatorColor));
        }
        else
        {
            indicator = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new NavigationIndicator(animation: indicatorAnimation, width: Navigation_railLibrary._kCircularIndicatorDiameter, shape: indicatorShape, color: indicatorColor));
        }
        return new global::Doroti.Framework.Widgets.Stack(alignment: Alignment.center, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(indicator), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(child) });
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
        this.selectedIcon = selectedIcon ?? icon;
    }

}

internal class _ExtendedNavigationRailAnimation__navigation_rail : global::Doroti.Framework.Widgets.InheritedWidget
{
    public virtual global::Doroti.Framework.Animation.Animation<double> animation { get; private set; } = default!;

    internal _ExtendedNavigationRailAnimation__navigation_rail(global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Widgets.Widget child) : base(child: child)
    {
        this.animation = animation;
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => !Equals(animation, ((_ExtendedNavigationRailAnimation__navigation_rail)oldWidget).animation);
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
    internal static global::Doroti.Framework.Widgets.Widget _verticalSpacer = new global::Doroti.Framework.Widgets.SizedBox(height: 8.0);
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
                __late__colors = Theme.of(context).colorScheme;
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
                __late__textTheme = Theme.of(context).textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }

    internal _NavigationRailDefaultsM3__navigation_rail(global::Doroti.Framework.Widgets.BuildContext context) : base(elevation: 0.0, groupAlignment: -1, labelType: NavigationRailLabelType.none, useIndicator: true, minWidth: 80.0, minExtendedWidth: 256)
    {
        this.context = context;
    }

    public override global::Doroti.Ui.Color? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.surface);
    public override global::Doroti.Framework.Painting.TextStyle? unselectedLabelTextStyle
    {
        get
        {
            return (global::Doroti.Framework.Painting.TextStyle?)_textTheme.labelMedium!.copyWith(color: _colors.onSurface);
        }
    }
    public override global::Doroti.Framework.Painting.TextStyle? selectedLabelTextStyle
    {
        get
        {
            return (global::Doroti.Framework.Painting.TextStyle?)_textTheme.labelMedium!.copyWith(color: _colors.onSurface);
        }
    }
    public override global::Doroti.Framework.Widgets.IconThemeData? unselectedIconTheme
    {
        get
        {
            return new global::Doroti.Framework.Widgets.IconThemeData(size: 24.0, color: _colors.onSurfaceVariant);
        }
    }
    public override global::Doroti.Framework.Widgets.IconThemeData? selectedIconTheme
    {
        get
        {
            return new global::Doroti.Framework.Widgets.IconThemeData(size: 24.0, color: _colors.onSecondaryContainer);
        }
    }
    public override global::Doroti.Ui.Color? indicatorColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.secondaryContainer);
    public override global::Doroti.Framework.Painting.ShapeBorder? indicatorShape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.ShapeBorder>(new global::Doroti.Framework.Painting.StadiumBorder());
}
