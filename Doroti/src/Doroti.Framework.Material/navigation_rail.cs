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

public class NavigationRail : StatefulWidget
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual bool extended { get; private set; } = default!;
    public virtual Widget? leading { get; private set; }
    public virtual Widget? trailing { get; private set; }
    public virtual List<NavigationRailDestination> destinations { get; private set; } = default!;
    public virtual long? selectedIndex { get; private set; }
    public virtual Action<long>? onDestinationSelected { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual double? groupAlignment { get; private set; }
    public virtual NavigationRailLabelType? labelType { get; private set; }
    public virtual TextStyle? unselectedLabelTextStyle { get; private set; }
    public virtual TextStyle? selectedLabelTextStyle { get; private set; }
    public virtual IconThemeData? unselectedIconTheme { get; private set; }
    public virtual IconThemeData? selectedIconTheme { get; private set; }
    public virtual double? minWidth { get; private set; }
    public virtual double? minExtendedWidth { get; private set; }
    public virtual bool? useIndicator { get; private set; }
    public virtual Color? indicatorColor { get; private set; }
    public virtual ShapeBorder? indicatorShape { get; private set; }
    public virtual bool leadingAtTop { get; private set; } = default!;
    public virtual bool trailingAtBottom { get; private set; } = default!;
    public virtual bool scrollable { get; private set; } = default!;
    public virtual MainAxisAlignment? mainAxisAlignment { get; private set; }

    public NavigationRail(
        Key? key = null,
        Color? backgroundColor = null,
        bool extended = false,
        Widget? leading = null,
        Widget? trailing = null,
        List<NavigationRailDestination> destinations = default!,
        long? selectedIndex = default!,
        Action<long>? onDestinationSelected = null,
        double? elevation = null,
        double? groupAlignment = null,
        NavigationRailLabelType? labelType = null,
        TextStyle? unselectedLabelTextStyle = null,
        TextStyle? selectedLabelTextStyle = null,
        IconThemeData? unselectedIconTheme = null,
        IconThemeData? selectedIconTheme = null,
        double? minWidth = null,
        double? minExtendedWidth = null,
        bool? useIndicator = null,
        Color? indicatorColor = null,
        ShapeBorder? indicatorShape = null,
        bool leadingAtTop = true,
        bool trailingAtBottom = false,
        bool scrollable = false,
        MainAxisAlignment? mainAxisAlignment = null
    )
        : base(key: key)
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
        System.Diagnostics.Debug.Assert(
            (selectedIndex is null)
                || (
                    (
                        0L
                        <= (
                            selectedIndex
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        )
                    )
                    && (
                        (
                            selectedIndex
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        ) < checked(destinations.Count)
                    )
                )
        );
        System.Diagnostics.Debug.Assert(
            (elevation is null)
                || (
                    (
                        elevation
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ) > 0L
                )
        );
        System.Diagnostics.Debug.Assert(
            (minWidth is null)
                || (
                    (
                        minWidth
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ) > 0L
                )
        );
        System.Diagnostics.Debug.Assert(
            (minExtendedWidth is null)
                || (
                    (
                        minExtendedWidth
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ) > 0L
                )
        );
        System.Diagnostics.Debug.Assert(
            (minWidth is null)
                || (minExtendedWidth is null)
                || (
                    minExtendedWidth
                    >= (
                        minWidth
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                )
        );
        System.Diagnostics.Debug.Assert(
            !extended
                || (labelType is null)
                || Equals(
                    (
                        labelType
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ),
                    NavigationRailLabelType.none
                )
        );
    }

    public static Animation<double> extendedAnimation(BuildContext context)
    {
        return context
            .dependOnInheritedWidgetOfExactType<_ExtendedNavigationRailAnimation__navigation_rail>()!
            .animation;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _NavigationRailState__navigation_rail());
}

internal class _NavigationRailState__navigation_rail
    : State<NavigationRail>,
        TickerProviderStateMixin<NavigationRail>
{
    internal virtual List<AnimationController> _destinationControllers { get; set; } = default!;
    internal virtual List<Animation<double>> _destinationAnimations { get; set; } = default!;
    internal virtual AnimationController _extendedController { get; set; } = default!;
    internal virtual CurvedAnimation _extendedAnimation { get; set; } = default!;
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

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
                foreach (Scheduler.Ticker ticker in _tickers!)
                {
                    if (ticker.isActive)
                    {
                        throw DartRuntimePrimitives.AsException(
                            new FlutterError(
                                new List<DiagnosticsNode>
                                {
                                    new ErrorSummary($"{this} was disposed with an active Ticker."),
                                    new ErrorDescription(
                                        $"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time "
                                            + "dispose() was called on the mixin, that Ticker was still active. All Tickers must "
                                            + "be disposed before calling super.dispose()."
                                    ),
                                    new ErrorHint(
                                        "Tickers used by AnimationControllers "
                                            + "should be disposed by calling dispose() on the AnimationController itself. "
                                            + "Otherwise, the ticker will leak."
                                    ),
                                    ticker.describeForError("The offending ticker was"),
                                }
                            )
                        );
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
                _destinationControllers[
                    (int)(
                        oldWidget.selectedIndex
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                ]
                    .reverse();
            }
            if (widget.selectedIndex is not null)
            {
                _destinationControllers[
                    (int)(
                        widget.selectedIndex
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                ]
                    .forward();
            }
            return;
        }
    }

    public override Widget build(BuildContext context)
    {
        NavigationRailThemeData navigationRailTheme = NavigationRailTheme.of(context);
        NavigationRailThemeData defaults = new _NavigationRailDefaultsM3__navigation_rail(context);
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        Color backgroundColorLocal =
            (widget.backgroundColor ?? navigationRailTheme.backgroundColor)
            ?? defaults.backgroundColor!;
        double elevationLocal =
            (widget.elevation ?? navigationRailTheme.elevation)
            ?? (
                defaults.elevation
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        double minWidthLocal =
            (widget.minWidth ?? navigationRailTheme.minWidth)
            ?? (
                defaults.minWidth
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        double minExtendedWidthLocal =
            (widget.minExtendedWidth ?? navigationRailTheme.minExtendedWidth)
            ?? (
                defaults.minExtendedWidth
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        TextStyle unselectedLabelTextStyleLocal =
            (widget.unselectedLabelTextStyle ?? navigationRailTheme.unselectedLabelTextStyle)
            ?? defaults.unselectedLabelTextStyle!;
        TextStyle selectedLabelTextStyleLocal =
            (widget.selectedLabelTextStyle ?? navigationRailTheme.selectedLabelTextStyle)
            ?? defaults.selectedLabelTextStyle!;
        IconThemeData unselectedIconThemeLocal =
            (widget.unselectedIconTheme ?? navigationRailTheme.unselectedIconTheme)
            ?? defaults.unselectedIconTheme!;
        IconThemeData selectedIconThemeLocal =
            (widget.selectedIconTheme ?? navigationRailTheme.selectedIconTheme)
            ?? defaults.selectedIconTheme!;
        double groupAlignmentLocal =
            (widget.groupAlignment ?? navigationRailTheme.groupAlignment)
            ?? (
                defaults.groupAlignment
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        NavigationRailLabelType labelTypeLocal =
            (widget.labelType ?? navigationRailTheme.labelType)
            ?? (
                defaults.labelType
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        bool useIndicatorLocal =
            (widget.useIndicator ?? navigationRailTheme.useIndicator)
            ?? (
                defaults.useIndicator
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        Color? indicatorColorLocal =
            (widget.indicatorColor ?? navigationRailTheme.indicatorColor)
            ?? defaults.indicatorColor;
        ShapeBorder? indicatorShapeLocal =
            (widget.indicatorShape ?? navigationRailTheme.indicatorShape)
            ?? defaults.indicatorShape;
        IconThemeData effectiveUnselectedIconTheme = unselectedIconThemeLocal;
        var isRTLDirection = Equals(Directionality.of(context), TextDirection.rtl);
        Widget mainGroup = new Column(
            mainAxisSize: (widget.mainAxisAlignment is not null)
                ? MainAxisSize.max
                : MainAxisSize.min,
            mainAxisAlignment: widget.mainAxisAlignment ?? MainAxisAlignment.start,
            children: (
                (Func<List<Widget>>)(
                    () =>
                    {
                        var __collection20149 = new List<Widget>();
                        if (!widget.leadingAtTop && (widget.leading is not null))
                        {
                            __collection20149.AddRange(
                                new List<Widget>
                                {
                                    DartRuntimePrimitives.ConvertValue<Widget>(widget.leading!),
                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                        Navigation_railLibrary._verticalSpacer
                                    ),
                                }
                            );
                        }
                        for (long i = 0L; i < checked(widget.destinations.Count); i += 1L)
                        {
                            var destinationIndex = i;
                            __collection20149.Add(
                                DartRuntimePrimitives.ConvertValue<Widget>(
                                    new _RailDestination__navigation_rail(
                                        minWidth: minWidthLocal,
                                        minExtendedWidth: minExtendedWidthLocal,
                                        extendedTransitionAnimation: _extendedAnimation,
                                        selected: widget.selectedIndex == i,
                                        icon: (widget.selectedIndex == i)
                                            ? widget.destinations[(int)i].selectedIcon
                                            : widget.destinations[(int)i].icon,
                                        label: widget.destinations[(int)i].label,
                                        destinationAnimation: _destinationAnimations[(int)i],
                                        labelType: labelTypeLocal,
                                        iconTheme: (widget.selectedIndex == i)
                                            ? selectedIconThemeLocal
                                            : effectiveUnselectedIconTheme,
                                        labelTextStyle: (widget.selectedIndex == i)
                                            ? selectedLabelTextStyleLocal
                                            : unselectedLabelTextStyleLocal,
                                        padding: widget.destinations[(int)i].padding,
                                        useIndicator: useIndicatorLocal,
                                        indicatorColor: useIndicatorLocal
                                            ? indicatorColorLocal
                                            : null,
                                        indicatorShape: useIndicatorLocal
                                            ? indicatorShapeLocal
                                            : null,
                                        onTap: () =>
                                        {
                                            if (widget.onDestinationSelected is not null)
                                            {
                                                widget.onDestinationSelected!(destinationIndex);
                                            }
                                        },
                                        indexLabel: localizations.tabLabel(
                                            tabIndex: i + 1L,
                                            tabCount: checked(widget.destinations.Count)
                                        ),
                                        disabled: widget.destinations[(int)i].disabled
                                    )
                                )
                            );
                        }
                        if (!widget.trailingAtBottom && (widget.trailing is not null))
                        {
                            __collection20149.Add(
                                DartRuntimePrimitives.ConvertValue<Widget>(widget.trailing!)
                            );
                        }
                        return __collection20149;
                    }
                )
            )()
        );
        if (widget.scrollable)
        {
            mainGroup = DartRuntimePrimitives.ConvertValue<Widget>(
                new SingleChildScrollView(child: mainGroup)
            );
        }
        return new Widgets.Semantics(
            container: true,
            child: new _ExtendedNavigationRailAnimation__navigation_rail(
                animation: _extendedAnimation,
                child: new Widgets.Semantics(
                    explicitChildNodes: true,
                    child: new Material(
                        elevation: elevationLocal,
                        color: backgroundColorLocal,
                        child: new SafeArea(
                            right: isRTLDirection,
                            left: !isRTLDirection,
                            child: new Column(
                                children: (
                                    (Func<List<Widget>>)(
                                        () =>
                                        {
                                            var __collection22353 = new List<Widget>();
                                            __collection22353.Add(
                                                DartRuntimePrimitives.ConvertValue<Widget>(
                                                    Navigation_railLibrary._verticalSpacer
                                                )
                                            );
                                            if (widget.leadingAtTop && (widget.leading is not null))
                                            {
                                                __collection22353.AddRange(
                                                    new List<Widget>
                                                    {
                                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                                            widget.leading!
                                                        ),
                                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                                            Navigation_railLibrary._verticalSpacer
                                                        ),
                                                    }
                                                );
                                            }
                                            __collection22353.Add(
                                                DartRuntimePrimitives.ConvertValue<Widget>(
                                                    new Flexible(
                                                        child: new Align(
                                                            alignment: new Alignment(
                                                                0,
                                                                groupAlignmentLocal
                                                            ),
                                                            child: mainGroup
                                                        )
                                                    )
                                                )
                                            );
                                            if (
                                                widget.trailingAtBottom
                                                && (widget.trailing is not null)
                                            )
                                            {
                                                __collection22353.Add(
                                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                                        widget.trailing!
                                                    )
                                                );
                                            }
                                            return __collection22353;
                                        }
                                    )
                                )()
                            )
                        )
                    )
                )
            )
        );
    }

    internal virtual void _disposeControllers()
    {
        foreach (AnimationController controller in _destinationControllers)
        {
            controller.dispose();
        }
        _extendedController.dispose();
        _extendedAnimation.dispose();
    }

    internal virtual void _initControllers()
    {
        _destinationControllers = new List<AnimationController>(
            Enumerable.Select(
                Enumerable.Range(0, checked((int)checked((long)widget.destinations.Count))),
                (index) =>
                {
                    return (
                        (Func<AnimationController>)(
                            () =>
                            {
                                var __cascade = new AnimationController(
                                    duration: ThemeLibrary.kThemeAnimationDuration,
                                    vsync: this
                                );
                                __cascade.addListener(_rebuild);
                                return __cascade;
                            }
                        )
                    )();
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                }
            )
        );
        _destinationAnimations = _destinationControllers
            .map((controller) => controller.view)
            .ToList();
        if (widget.selectedIndex is not null)
        {
            _destinationControllers[
                (int)(
                    widget.selectedIndex
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
            ].value = 1.0;
        }
        _extendedController = new AnimationController(
            duration: ThemeLibrary.kThemeAnimationDuration,
            vsync: this,
            value: widget.extended ? 1.0 : 0.0
        );
        _extendedAnimation = new CurvedAnimation(
            parent: _extendedController,
            curve: Curves.easeInOut
        );
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
        setState(() => { });
    }

    public virtual Scheduler.Ticker createTicker(Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = (
            (Func<_WidgetTicker__ticker_provider>)(
                () =>
                {
                    var __cascade = new _WidgetTicker__ticker_provider(
                        onTick,
                        this,
                        debugLabel: Foundation.ConstantsLibrary.kDebugMode
                            ? $"created by {DiagnosticsLibrary.describeIdentity(this)}"
                            : null
                    );
                    __cascade.muted = !values.enabled;
                    __cascade.forceFrames = values.forceFrames;
                    return __cascade;
                }
            )
        )();
        _tickers!.Add(result);
        return result;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void _removeTicker(_WidgetTicker__ticker_provider ticker)
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
            foreach (Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new DiagnosticsProperty<HashSet<Scheduler.Ticker>>(
                "tickers",
                _tickers,
                description: (_tickers is not null)
                    ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}"
                    : null,
                defaultValue: default
            )
        );
    }
}

public class _RailDestination__navigation_rail : StatefulWidget
{
    public virtual double minWidth { get; private set; } = default!;
    public virtual double minExtendedWidth { get; private set; } = default!;
    public virtual Widget icon { get; private set; } = default!;
    public virtual Widget label { get; private set; } = default!;
    public virtual Animation<double> destinationAnimation { get; private set; } = default!;
    public virtual NavigationRailLabelType labelType { get; private set; } = default!;
    public virtual bool selected { get; private set; } = default!;
    public virtual Animation<double> extendedTransitionAnimation { get; private set; } = default!;
    public virtual IconThemeData iconTheme { get; private set; } = default!;
    public virtual TextStyle labelTextStyle { get; private set; } = default!;
    public virtual Action onTap { get; private set; } = default!;
    public virtual string indexLabel { get; private set; } = default!;
    public virtual EdgeInsetsGeometry? padding { get; private set; }
    public virtual bool useIndicator { get; private set; } = default!;
    public virtual Color? indicatorColor { get; private set; }
    public virtual ShapeBorder? indicatorShape { get; private set; }
    public virtual bool disabled { get; private set; } = default!;

    internal _RailDestination__navigation_rail(
        double minWidth,
        double minExtendedWidth,
        Widget icon,
        Widget label,
        Animation<double> destinationAnimation,
        Animation<double> extendedTransitionAnimation,
        NavigationRailLabelType labelType,
        bool selected,
        IconThemeData iconTheme,
        TextStyle labelTextStyle,
        Action onTap,
        string indexLabel,
        EdgeInsetsGeometry? padding = null,
        bool useIndicator = default!,
        Color? indicatorColor = null,
        ShapeBorder? indicatorShape = null,
        bool disabled = false
    )
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

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _RailDestinationState__navigation_rail());
}

internal class _RailDestinationState__navigation_rail : State<_RailDestination__navigation_rail>
{
    internal virtual CurvedAnimation _positionAnimation { get; set; } = default!;

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
        _positionAnimation = new CurvedAnimation(
            parent: new ReverseAnimation(widget.destinationAnimation),
            curve: Curves.easeInOut,
            reverseCurve: Curves.easeInOut.flipped
        );
    }

    public override void dispose()
    {
        _positionAnimation.dispose();
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(
            () => widget.useIndicator || (widget.indicatorColor is null),
            () =>
                (object?)
                    "[NavigationRail.indicatorColor] does not have an effect when [NavigationRail.useIndicator] is false"
        );
        ThemeData theme = Theme.of(context);
        TextDirection textDirectionLocal = Directionality.of(context);
        EdgeInsets destinationPadding = (widget.padding ?? EdgeInsets.zero).resolve(
            textDirectionLocal
        );
        Offset indicatorOffsetLocal = default!;
        var applyXOffsetLocal = false;
        Widget themedIcon = new IconTheme(
            data: widget.disabled
                ? widget.iconTheme.copyWith(color: theme.colorScheme.onSurface.withOpacity(0.38))
                : widget.iconTheme,
            child: widget.icon
        );
        Widget styledLabel = new DefaultTextStyle(
            style: widget.disabled
                ? widget.labelTextStyle.copyWith(
                    color: theme.colorScheme.onSurface.withOpacity(0.38)
                )
                : widget.labelTextStyle,
            child: widget.label
        );
        Widget content = default!;
        bool isLargeIconSize =
            (widget.iconTheme.size is not null)
            && (
                (
                    widget.iconTheme.size
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ) > Navigation_barLibrary._kIndicatorHeight
            );
        double indicatorVerticalOffset = isLargeIconSize
            ? (
                (
                    (
                        widget.iconTheme.size
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ) - Navigation_barLibrary._kIndicatorHeight
                ) / 2L
            )
            : 0;
        switch (widget.labelType)
        {
            case NavigationRailLabelType.none:
            {
                Widget? spacing = (Widget?)
                    new SizedBox(height: Navigation_railLibrary._verticalDestinationSpacingM3 / 2L);
                indicatorOffsetLocal = new Offset(
                    (widget.minWidth / 2L) + destinationPadding.left,
                    (Navigation_railLibrary._verticalDestinationSpacingM3 / 2L)
                        + destinationPadding.top
                        + indicatorVerticalOffset
                );
                Widget iconPart = new Column(
                    children: (
                        (Func<List<Widget>>)(
                            () =>
                            {
                                var __collection28264 = new List<Widget>();
                                var __collectionElement28286 = spacing;
                                if (__collectionElement28286 is { } __nonNullCollectionElement28286)
                                {
                                    __collection28264.Add(
                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                            __nonNullCollectionElement28286
                                        )
                                    );
                                }
                                __collection28264.Add(
                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                        new SizedBox(
                                            width: widget.minWidth,
                                            height: null,
                                            child: new Center(
                                                child: new _AddIndicator__navigation_rail(
                                                    addIndicator: widget.useIndicator,
                                                    indicatorColor: widget.indicatorColor,
                                                    indicatorShape: widget.indicatorShape,
                                                    isCircular: false,
                                                    indicatorAnimation: widget.destinationAnimation,
                                                    child: themedIcon
                                                )
                                            )
                                        )
                                    )
                                );
                                var __collectionElement28857 = spacing;
                                if (__collectionElement28857 is { } __nonNullCollectionElement28857)
                                {
                                    __collection28264.Add(
                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                            __nonNullCollectionElement28857
                                        )
                                    );
                                }
                                return __collection28264;
                            }
                        )
                    )()
                );
                if (widget.extendedTransitionAnimation.value == 0L)
                {
                    content = DartRuntimePrimitives.ConvertValue<Widget>(
                        new Padding(
                            padding: widget.padding ?? EdgeInsets.zero,
                            child: new Stack(
                                children: new List<Widget>
                                {
                                    DartRuntimePrimitives.ConvertValue<Widget>(iconPart),
                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                        SizedBox.CreateShrink(
                                            child: Visibility.CreateMaintain(
                                                visible: false,
                                                child: widget.label
                                            )
                                        )
                                    ),
                                }
                            )
                        )
                    );
                }
                else
                {
                    Animation<double> labelFadeAnimation = widget.extendedTransitionAnimation.drive(
                        new CurveTween(curve: new Interval(0.0, 0.25))
                    );
                    applyXOffsetLocal = true;
                    content = DartRuntimePrimitives.ConvertValue<Widget>(
                        new Padding(
                            padding: widget.padding ?? EdgeInsets.zero,
                            child: new ConstrainedBox(
                                constraints: new BoxConstraints(
                                    minWidth: (
                                        DorotiUiLibrary.lerpDouble(
                                            widget.minWidth,
                                            widget.minExtendedWidth,
                                            widget.extendedTransitionAnimation.value
                                        )
                                        ?? throw new global::System.NullReferenceException(
                                            "A required value was null."
                                        )
                                    )
                                ),
                                child: new ClipRect(
                                    child: new Row(
                                        mainAxisSize: MainAxisSize.min,
                                        children: new List<Widget>
                                        {
                                            DartRuntimePrimitives.ConvertValue<Widget>(iconPart),
                                            DartRuntimePrimitives.ConvertValue<Widget>(
                                                new Flexible(
                                                    child: new Align(
                                                        heightFactor: 1.0,
                                                        widthFactor: widget
                                                            .extendedTransitionAnimation
                                                            .value,
                                                        alignment: AlignmentDirectional.centerStart,
                                                        child: new FadeTransition(
                                                            alwaysIncludeSemantics: true,
                                                            opacity: labelFadeAnimation,
                                                            child: styledLabel
                                                        )
                                                    )
                                                )
                                            ),
                                            DartRuntimePrimitives.ConvertValue<Widget>(
                                                new SizedBox(
                                                    width: Navigation_railLibrary._horizontalDestinationPadding
                                                        * widget.extendedTransitionAnimation.value
                                                )
                                            ),
                                        }
                                    )
                                )
                            )
                        )
                    );
                }
                break;
            }
            case NavigationRailLabelType.selected:
            {
                double appearingAnimationValue = 1L - _positionAnimation.value;
                double verticalPadding = (
                    DorotiUiLibrary.lerpDouble(
                        Navigation_railLibrary._verticalDestinationPaddingNoLabel,
                        Navigation_railLibrary._verticalDestinationPaddingWithLabel,
                        appearingAnimationValue
                    )
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
                var interval = widget.selected ? new Interval(0.25, 0.75) : new Interval(0.75, 1.0);
                Animation<double> labelFadeAnimationLocal = widget.destinationAnimation.drive(
                    new CurveTween(curve: interval)
                );
                double minHeightLocal = 0;
                Widget topSpacing = new SizedBox(height: 0);
                Widget labelSpacing = new SizedBox(
                    height: (
                        DorotiUiLibrary.lerpDouble(
                            0L,
                            Navigation_railLibrary._verticalIconLabelSpacingM3,
                            appearingAnimationValue
                        )
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                );
                Widget bottomSpacing = new SizedBox(
                    height: Navigation_railLibrary._verticalDestinationSpacingM3
                );
                double indicatorHorizontalPadding =
                    (destinationPadding.left / 2L) - (destinationPadding.right / 2L);
                double indicatorVerticalPadding = destinationPadding.top;
                indicatorOffsetLocal = new Offset(
                    (widget.minWidth / 2L) + indicatorHorizontalPadding,
                    indicatorVerticalPadding + indicatorVerticalOffset
                );
                if (widget.minWidth < Navigation_railLibrary._compactDestinationWidth)
                {
                    indicatorOffsetLocal = new Offset(
                        (widget.minWidth / 2L)
                            + Navigation_railLibrary._horizontalDestinationSpacingM3,
                        indicatorVerticalPadding + indicatorVerticalOffset
                    );
                }
                content = DartRuntimePrimitives.ConvertValue<Widget>(
                    new ConstrainedBox(
                        constraints: new BoxConstraints(
                            minWidth: widget.minWidth,
                            minHeight: minHeightLocal
                        ),
                        child: new Padding(
                            padding: widget.padding
                                ?? EdgeInsets.CreateSymmetric(
                                    horizontal: Navigation_railLibrary._horizontalDestinationPadding
                                ),
                            child: new ClipRect(
                                child: new Column(
                                    mainAxisSize: MainAxisSize.min,
                                    mainAxisAlignment: MainAxisAlignment.center,
                                    children: new List<Widget>
                                    {
                                        DartRuntimePrimitives.ConvertValue<Widget>(topSpacing),
                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                            new _AddIndicator__navigation_rail(
                                                addIndicator: widget.useIndicator,
                                                indicatorColor: widget.indicatorColor,
                                                indicatorShape: widget.indicatorShape,
                                                isCircular: false,
                                                indicatorAnimation: widget.destinationAnimation,
                                                child: themedIcon
                                            )
                                        ),
                                        DartRuntimePrimitives.ConvertValue<Widget>(labelSpacing),
                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                            new Align(
                                                alignment: Alignment.topCenter,
                                                heightFactor: appearingAnimationValue,
                                                widthFactor: 1.0,
                                                child: new FadeTransition(
                                                    alwaysIncludeSemantics: true,
                                                    opacity: labelFadeAnimationLocal,
                                                    child: styledLabel
                                                )
                                            )
                                        ),
                                        DartRuntimePrimitives.ConvertValue<Widget>(bottomSpacing),
                                    }
                                )
                            )
                        )
                    )
                );
                break;
            }
            case NavigationRailLabelType.all:
            {
                double minHeightAlternate = 0;
                Widget topSpacingLocal = new SizedBox(height: 0);
                Widget labelSpacingLocal = new SizedBox(
                    height: Navigation_railLibrary._verticalIconLabelSpacingM3
                );
                Widget bottomSpacingLocal = new SizedBox(
                    height: Navigation_railLibrary._verticalDestinationSpacingM3
                );
                double indicatorHorizontalPaddingLocal =
                    (destinationPadding.left / 2L) - (destinationPadding.right / 2L);
                double indicatorVerticalPaddingLocal = destinationPadding.top;
                indicatorOffsetLocal = new Offset(
                    (widget.minWidth / 2L) + indicatorHorizontalPaddingLocal,
                    indicatorVerticalPaddingLocal + indicatorVerticalOffset
                );
                if (widget.minWidth < Navigation_railLibrary._compactDestinationWidth)
                {
                    indicatorOffsetLocal = new Offset(
                        (widget.minWidth / 2L)
                            + Navigation_railLibrary._horizontalDestinationSpacingM3,
                        indicatorVerticalPaddingLocal + indicatorVerticalOffset
                    );
                }
                content = DartRuntimePrimitives.ConvertValue<Widget>(
                    new ConstrainedBox(
                        constraints: new BoxConstraints(
                            minWidth: widget.minWidth,
                            minHeight: minHeightAlternate
                        ),
                        child: new Padding(
                            padding: widget.padding
                                ?? EdgeInsets.CreateSymmetric(
                                    horizontal: Navigation_railLibrary._horizontalDestinationPadding
                                ),
                            child: new Column(
                                children: new List<Widget>
                                {
                                    DartRuntimePrimitives.ConvertValue<Widget>(topSpacingLocal),
                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                        new _AddIndicator__navigation_rail(
                                            addIndicator: widget.useIndicator,
                                            indicatorColor: widget.indicatorColor,
                                            indicatorShape: widget.indicatorShape,
                                            isCircular: false,
                                            indicatorAnimation: widget.destinationAnimation,
                                            child: themedIcon
                                        )
                                    ),
                                    DartRuntimePrimitives.ConvertValue<Widget>(labelSpacingLocal),
                                    DartRuntimePrimitives.ConvertValue<Widget>(styledLabel),
                                    DartRuntimePrimitives.ConvertValue<Widget>(bottomSpacingLocal),
                                }
                            )
                        )
                    )
                );
                break;
            }
        }
        ColorScheme colors = Theme.of(context).colorScheme;
        bool primaryColorAlphaModified = colors.primary.alpha < 255.0;
        Color effectiveSplashColor = primaryColorAlphaModified
            ? colors.primary
            : colors.primary.withOpacity(0.12);
        Color effectiveHoverColor = primaryColorAlphaModified
            ? colors.primary
            : colors.primary.withOpacity(0.04);
        return new Widgets.Semantics(
            container: true,
            selected: widget.selected,
            child: new Stack(
                children: new List<Widget>
                {
                    DartRuntimePrimitives.ConvertValue<Widget>(
                        new Material(
                            type: MaterialType.transparency,
                            child: new _IndicatorInkWell__navigation_rail(
                                onTap: widget.disabled ? null : widget.onTap,
                                borderRadius: BorderRadius.CreateAll(
                                    Radius.circular(widget.minWidth / 2.0)
                                ),
                                customBorder: widget.indicatorShape,
                                splashColor: effectiveSplashColor,
                                hoverColor: effectiveHoverColor,
                                indicatorOffset: indicatorOffsetLocal,
                                applyXOffset: applyXOffsetLocal,
                                textDirection: textDirectionLocal,
                                child: content
                            )
                        )
                    ),
                    DartRuntimePrimitives.ConvertValue<Widget>(
                        new Widgets.Semantics(label: widget.indexLabel)
                    ),
                }
            )
        );
    }
}

internal class _IndicatorInkWell__navigation_rail : InkResponse
{
    public virtual Offset indicatorOffset { get; private set; } = default!;
    public virtual bool applyXOffset { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;

    internal _IndicatorInkWell__navigation_rail(
        Widget? child = null,
        Action? onTap = null,
        ShapeBorder? customBorder = null,
        BorderRadius? borderRadius = null,
        Color? splashColor = null,
        Color? hoverColor = null,
        Offset indicatorOffset = default!,
        bool applyXOffset = default!,
        TextDirection textDirection = default!
    )
        : base(
            child: child,
            onTap: onTap,
            splashColor: splashColor,
            hoverColor: hoverColor,
            containedInkWell: true,
            highlightShape: BoxShape.rectangle,
            borderRadius: null,
            customBorder: customBorder
        )
    {
        this.indicatorOffset = indicatorOffset;
        this.applyXOffset = applyXOffset;
        this.textDirection = textDirection;
    }

    public override Func<Rect>? getRectCallback(RenderBox referenceBox)
    {
        {
            double boxWidth = referenceBox.size.width;
            double indicatorHorizontalCenter = applyXOffset ? indicatorOffset.dx : (boxWidth / 2L);
            if (Equals(textDirection, TextDirection.rtl))
            {
                indicatorHorizontalCenter = boxWidth - indicatorHorizontalCenter;
            }
            return (Func<Rect>?)
                (object?)(
                    () =>
                    {
                        return Rect.fromLTWH(
                            indicatorHorizontalCenter
                                - (Navigation_railLibrary._kCircularIndicatorDiameter / 2L),
                            indicatorOffset.dy,
                            Navigation_railLibrary._kCircularIndicatorDiameter,
                            Navigation_barLibrary._kIndicatorHeight
                        );
                        throw new InvalidOperationException(
                            "Callback completed without returning a value."
                        );
                    }
                );
        }
    }
}

internal class _AddIndicator__navigation_rail : StatelessWidget
{
    public virtual bool addIndicator { get; private set; } = default!;
    public virtual bool isCircular { get; private set; } = default!;
    public virtual Color? indicatorColor { get; private set; }
    public virtual ShapeBorder? indicatorShape { get; private set; }
    public virtual Animation<double> indicatorAnimation { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    internal _AddIndicator__navigation_rail(
        bool addIndicator,
        bool isCircular,
        Color? indicatorColor,
        ShapeBorder? indicatorShape,
        Animation<double> indicatorAnimation,
        Widget child
    )
    {
        this.addIndicator = addIndicator;
        this.isCircular = isCircular;
        this.indicatorColor = indicatorColor;
        this.indicatorShape = indicatorShape;
        this.indicatorAnimation = indicatorAnimation;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        if (!addIndicator)
        {
            return child;
        }
        Widget indicator = default!;
        if (isCircular)
        {
            indicator = DartRuntimePrimitives.ConvertValue<Widget>(
                new NavigationIndicator(
                    animation: indicatorAnimation,
                    height: Navigation_railLibrary._kCircularIndicatorDiameter,
                    width: Navigation_railLibrary._kCircularIndicatorDiameter,
                    borderRadius: BorderRadius.CreateAll(
                        Radius.circular(Navigation_railLibrary._kCircularIndicatorDiameter / 2L)
                    ),
                    color: indicatorColor
                )
            );
        }
        else
        {
            indicator = DartRuntimePrimitives.ConvertValue<Widget>(
                new NavigationIndicator(
                    animation: indicatorAnimation,
                    width: Navigation_railLibrary._kCircularIndicatorDiameter,
                    shape: indicatorShape,
                    color: indicatorColor
                )
            );
        }
        return new Stack(
            alignment: Alignment.center,
            children: new List<Widget>
            {
                DartRuntimePrimitives.ConvertValue<Widget>(indicator),
                DartRuntimePrimitives.ConvertValue<Widget>(child),
            }
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public enum NavigationRailLabelType
{
    none,
    selected,
    all,
}

public class NavigationRailDestination
{
    public virtual Widget icon { get; private set; } = default!;
    public virtual Widget selectedIcon { get; private set; } = default!;
    public virtual Color? indicatorColor { get; private set; }
    public virtual ShapeBorder? indicatorShape { get; private set; }
    public virtual Widget label { get; private set; } = default!;
    public virtual EdgeInsetsGeometry? padding { get; private set; }
    public virtual bool disabled { get; private set; } = default!;

    public NavigationRailDestination(
        Widget icon,
        Widget? selectedIcon = null,
        Color? indicatorColor = null,
        ShapeBorder? indicatorShape = null,
        Widget label = default!,
        EdgeInsetsGeometry? padding = null,
        bool disabled = false
    )
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

internal class _ExtendedNavigationRailAnimation__navigation_rail : InheritedWidget
{
    public virtual Animation<double> animation { get; private set; } = default!;

    internal _ExtendedNavigationRailAnimation__navigation_rail(
        Animation<double> animation,
        Widget child
    )
        : base(child: child)
    {
        this.animation = animation;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        !Equals(
            animation,
            ((_ExtendedNavigationRailAnimation__navigation_rail)oldWidget).animation
        );
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
    internal static Widget _verticalSpacer = new SizedBox(height: 8.0);
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
    public virtual BuildContext context { get; private set; } = default!;
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

    internal _NavigationRailDefaultsM3__navigation_rail(BuildContext context)
        : base(
            elevation: 0.0,
            groupAlignment: -1,
            labelType: NavigationRailLabelType.none,
            useIndicator: true,
            minWidth: 80.0,
            minExtendedWidth: 256
        )
    {
        this.context = context;
    }

    public override Color? backgroundColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.surface);
    public override TextStyle? unselectedLabelTextStyle
    {
        get { return (TextStyle?)_textTheme.labelMedium!.copyWith(color: _colors.onSurface); }
    }
    public override TextStyle? selectedLabelTextStyle
    {
        get { return (TextStyle?)_textTheme.labelMedium!.copyWith(color: _colors.onSurface); }
    }
    public override IconThemeData? unselectedIconTheme
    {
        get { return new IconThemeData(size: 24.0, color: _colors.onSurfaceVariant); }
    }
    public override IconThemeData? selectedIconTheme
    {
        get { return new IconThemeData(size: 24.0, color: _colors.onSecondaryContainer); }
    }
    public override Color? indicatorColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.secondaryContainer);
    public override ShapeBorder? indicatorShape =>
        DartRuntimePrimitives.ConvertValue<ShapeBorder>(new StadiumBorder());
}
