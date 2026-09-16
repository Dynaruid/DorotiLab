// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/navigation_drawer.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class NavigationDrawer : StatelessWidget
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? indicatorColor { get; private set; }
    public virtual ShapeBorder? indicatorShape { get; private set; }
    public virtual List<Widget> children { get; private set; } = default!;
    public virtual Widget? header { get; private set; }
    public virtual Widget? footer { get; private set; }
    public virtual long? selectedIndex { get; private set; }
    public virtual Action<long>? onDestinationSelected { get; private set; }
    public virtual EdgeInsetsGeometry tilePadding { get; private set; } = default!;

    public NavigationDrawer(Key? key = null, List<Widget> children = default!, Widget? header = null, Widget? footer = null, Color? backgroundColor = null, Color? shadowColor = null, Color? surfaceTintColor = null, double? elevation = null, Color? indicatorColor = null, ShapeBorder? indicatorShape = null, Action<long>? onDestinationSelected = null, long? selectedIndex = 0, EdgeInsetsGeometry tilePadding = default!) : base(key: key)
    {
        EdgeInsetsGeometry __tilePadding = tilePadding ?? EdgeInsets.CreateSymmetric(horizontal: 12.0);
        this.children = children;
        this.header = header;
        this.footer = footer;
        this.backgroundColor = backgroundColor;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.elevation = elevation;
        this.indicatorColor = indicatorColor;
        this.indicatorShape = indicatorShape;
        this.onDestinationSelected = onDestinationSelected;
        this.selectedIndex = selectedIndex;
        this.tilePadding = __tilePadding;
    }

    public override Widget build(BuildContext context)
    {
        long totalNumberOfDestinationsLocal = checked(children.OfType<NavigationDrawerDestination>().ToList().Count);
        var destinationIndex = 0L;
        Widget wrapChild(Widget child, long index)
        {
            return new _SelectableAnimatedBuilder__navigation_drawer(isSelected: selectedIndex == index, duration: new Duration(500_000L),
                builder: (_, animation) => new _NavigationDrawerDestinationInfo__navigation_drawer(index: index,
                    totalNumberOfDestinations: totalNumberOfDestinationsLocal, selectedAnimation: animation,
                    indicatorColor: indicatorColor, indicatorShape: indicatorShape, tilePadding: tilePadding,
                    onTap: () => onDestinationSelected?.Invoke(index), child: child));
        }
        var wrappedChildren = ((Func<List<Widget>>)(() => { var __collection6931 = new List<Widget>(); foreach (var childLocal in children) { if (childLocal is not NavigationDrawerDestination) { __collection6931.Add(DartRuntimePrimitives.ConvertValue<Widget>(childLocal)); } else { __collection6931.Add(DartRuntimePrimitives.ConvertValue<Widget>(wrapChild((NavigationDrawerDestination)childLocal, destinationIndex++))); } } return __collection6931; }))();
        NavigationDrawerThemeData navigationDrawerTheme = NavigationDrawerTheme.of(context);
        return new Drawer(backgroundColor: backgroundColor ?? navigationDrawerTheme.backgroundColor, shadowColor: shadowColor ?? navigationDrawerTheme.shadowColor, surfaceTintColor: surfaceTintColor ?? navigationDrawerTheme.surfaceTintColor, elevation: elevation ?? navigationDrawerTheme.elevation, child: new SafeArea(bottom: false, child: new Column(children: ((Func<List<Widget>>)(() => { var __collection7592 = new List<Widget>(); var __collectionElement7614 = header; if (__collectionElement7614 is { } __nonNullCollectionElement7614) { __collection7592.Add(DartRuntimePrimitives.ConvertValue<Widget>(__nonNullCollectionElement7614)); } __collection7592.Add(DartRuntimePrimitives.ConvertValue<Widget>(new Expanded(child: new Material(type: MaterialType.transparency, child: new ListView(children: wrappedChildren))))); var __collectionElement7829 = footer; if (__collectionElement7829 is { } __nonNullCollectionElement7829) { __collection7592.Add(DartRuntimePrimitives.ConvertValue<Widget>(__nonNullCollectionElement7829)); } return __collection7592; }))())));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class NavigationDrawerDestination : StatelessWidget
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual Widget icon { get; private set; } = default!;
    public virtual Widget? selectedIcon { get; private set; }
    public virtual Widget label { get; private set; } = default!;
    public virtual bool enabled { get; private set; } = default!;

    public NavigationDrawerDestination(Key? key = null, Color? backgroundColor = null, Widget icon = default!, Widget? selectedIcon = null, Widget label = default!, bool enabled = true) : base(key: key)
    {
        this.backgroundColor = backgroundColor;
        this.icon = icon;
        this.selectedIcon = selectedIcon;
        this.label = label;
        this.enabled = enabled;
    }

    public override Widget build(BuildContext context)
    {
        var selectedState = new HashSet<WidgetState> { WidgetState.selected };
        var unselectedState = new HashSet<WidgetState>();
        var disabledState = new HashSet<WidgetState> { WidgetState.disabled };
        NavigationDrawerThemeData navigationDrawerTheme = NavigationDrawerTheme.of(context);
        NavigationDrawerThemeData defaults = new _NavigationDrawerDefaultsM3__navigation_drawer(context);
        Animation<double> animation = _NavigationDrawerDestinationInfo__navigation_drawer.of(context).selectedAnimation;
        return new _NavigationDestinationBuilder__navigation_drawer(buildIcon: (context) =>
        {
            Widget selectedIconWidget = IconTheme.merge(data: navigationDrawerTheme.iconTheme?.resolve(enabled ? selectedState : disabledState) ?? defaults.iconTheme!.resolve(enabled ? selectedState : disabledState)!, child: selectedIcon ?? icon);
            Widget unselectedIconWidget = IconTheme.merge(data: navigationDrawerTheme.iconTheme?.resolve(enabled ? unselectedState : disabledState) ?? defaults.iconTheme!.resolve(enabled ? unselectedState : disabledState)!, child: icon);
            return animation.isForwardOrCompleted ? selectedIconWidget : unselectedIconWidget;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, buildLabel: (context) =>
        {
            TextStyle? effectiveSelectedLabelTextStyle = navigationDrawerTheme.labelTextStyle?.resolve(enabled ? selectedState : disabledState) ?? defaults.labelTextStyle!.resolve(enabled ? selectedState : disabledState);
            TextStyle? effectiveUnselectedLabelTextStyle = navigationDrawerTheme.labelTextStyle?.resolve(enabled ? unselectedState : disabledState) ?? defaults.labelTextStyle!.resolve(enabled ? unselectedState : disabledState);
            return new DefaultTextStyle(style: animation.isForwardOrCompleted ? effectiveSelectedLabelTextStyle! : effectiveUnselectedLabelTextStyle!, child: label);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, enabled: enabled, backgroundColor: backgroundColor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _NavigationDestinationBuilder__navigation_drawer : StatelessWidget
{
    public virtual Func<BuildContext, Widget> buildIcon { get; private set; } = default!;
    public virtual Func<BuildContext, Widget> buildLabel { get; private set; } = default!;
    public virtual bool enabled { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }

    internal _NavigationDestinationBuilder__navigation_drawer(Func<BuildContext, Widget> buildIcon, Func<BuildContext, Widget> buildLabel, bool enabled = true, Color? backgroundColor = null)
    {
        this.buildIcon = buildIcon;
        this.buildLabel = buildLabel;
        this.enabled = enabled;
        this.backgroundColor = backgroundColor;
    }

    public override Widget build(BuildContext context)
    {
        _NavigationDrawerDestinationInfo__navigation_drawer info = _NavigationDrawerDestinationInfo__navigation_drawer.of(context);
        NavigationDrawerThemeData navigationDrawerTheme = NavigationDrawerTheme.of(context);
        NavigationDrawerThemeData defaults = new _NavigationDrawerDefaultsM3__navigation_drawer(context);
        var inkWell = new InkWell(highlightColor: Colors.transparent, onTap: enabled ? info.onTap : null, customBorder: (info.indicatorShape ?? navigationDrawerTheme.indicatorShape) ?? defaults.indicatorShape!, child: new Stack(alignment: Alignment.center, children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new NavigationIndicator(animation: info.selectedAnimation, color: (info.indicatorColor ?? navigationDrawerTheme.indicatorColor) ?? defaults.indicatorColor!, shape: (info.indicatorShape ?? navigationDrawerTheme.indicatorShape) ?? defaults.indicatorShape!, width: (navigationDrawerTheme.indicatorSize ?? DartRuntimePrimitives.RequireValue(defaults.indicatorSize)).width, height: (navigationDrawerTheme.indicatorSize ?? DartRuntimePrimitives.RequireValue(defaults.indicatorSize)).height)), DartRuntimePrimitives.ConvertValue<Widget>(new Row(children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new SizedBox(width: 16)), DartRuntimePrimitives.ConvertValue<Widget>(buildIcon(context)), DartRuntimePrimitives.ConvertValue<Widget>(new SizedBox(width: 12)), DartRuntimePrimitives.ConvertValue<Widget>(buildLabel(context)) })) }));
        Widget destination = new Padding(padding: info.tilePadding, child: new _NavigationDestinationSemantics__navigation_drawer(child: new SizedBox(height: navigationDrawerTheme.tileHeight ?? defaults.tileHeight, child: inkWell)));
        if (backgroundColor is not null)
        {
            return new Ink(color: backgroundColor, child: destination);
        }
        return destination;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _NavigationDestinationSemantics__navigation_drawer : StatelessWidget
{
    public virtual Widget child { get; private set; } = default!;

    internal _NavigationDestinationSemantics__navigation_drawer(Widget child)
    {
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        _NavigationDrawerDestinationInfo__navigation_drawer destinationInfo = _NavigationDrawerDestinationInfo__navigation_drawer.of(context);
        return new _StatusTransitionWidgetBuilder__navigation_drawer(animation: destinationInfo.selectedAnimation, builder: (context, child) =>
        {
            return new Widgets.Semantics(selected: destinationInfo.selectedAnimation.isForwardOrCompleted, container: true, child: child);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, child: new Stack(alignment: Alignment.center, children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(child), DartRuntimePrimitives.ConvertValue<Widget>(new Widgets.Semantics(label: localizations.tabLabel(tabIndex: destinationInfo.index + 1L, tabCount: destinationInfo.totalNumberOfDestinations))) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _StatusTransitionWidgetBuilder__navigation_drawer : StatusTransitionWidget
{
    public virtual Func<BuildContext, Widget?, Widget> builder { get; private set; } = default!;
    public virtual Widget? child { get; private set; }

    internal _StatusTransitionWidgetBuilder__navigation_drawer(Animation<double> animation, Func<BuildContext, Widget?, Widget> builder, Widget? child = null) : base(animation: animation)
    {
        this.builder = builder;
        this.child = child;
    }

    public override Widget build(BuildContext context) => builder(context, child);
}

internal class _NavigationDrawerDestinationInfo__navigation_drawer : InheritedWidget
{
    public virtual long index { get; private set; } = default!;
    public virtual long totalNumberOfDestinations { get; private set; } = default!;
    public virtual Animation<double> selectedAnimation { get; private set; } = default!;
    public virtual Color? indicatorColor { get; private set; }
    public virtual ShapeBorder? indicatorShape { get; private set; }
    public virtual Action onTap { get; private set; } = default!;
    public virtual EdgeInsetsGeometry tilePadding { get; private set; } = default!;

    internal _NavigationDrawerDestinationInfo__navigation_drawer(long index, long totalNumberOfDestinations, Animation<double> selectedAnimation, Color? indicatorColor, ShapeBorder? indicatorShape, Action onTap, Widget child, EdgeInsetsGeometry tilePadding) : base(child: child)
    {
        this.index = index;
        this.totalNumberOfDestinations = totalNumberOfDestinations;
        this.selectedAnimation = selectedAnimation;
        this.indicatorColor = indicatorColor;
        this.indicatorShape = indicatorShape;
        this.onTap = onTap;
        this.tilePadding = tilePadding;
    }

    public static _NavigationDrawerDestinationInfo__navigation_drawer of(BuildContext context)
    {
        _NavigationDrawerDestinationInfo__navigation_drawer? result = context.dependOnInheritedWidgetOfExactType<_NavigationDrawerDestinationInfo__navigation_drawer>();
        DartRuntimePrimitives.Assert(() => result is not null, () => (object?)"Navigation destinations need a _NavigationDrawerDestinationInfo parent, " + "which is usually provided by NavigationDrawer.");
        return result!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (_NavigationDrawerDestinationInfo__navigation_drawer)oldWidget;
        return (index != __oldWidget.index) || (totalNumberOfDestinations != __oldWidget.totalNumberOfDestinations) || (!Equals(selectedAnimation, __oldWidget.selectedAnimation)) || (!Equals(onTap, __oldWidget.onTap));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _SelectableAnimatedBuilder__navigation_drawer : StatefulWidget
{
    public virtual bool isSelected { get; private set; } = default!;
    public virtual Duration duration { get; private set; } = default!;
    public virtual Func<BuildContext, Animation<double>, Widget> builder { get; private set; } = default!;

    internal _SelectableAnimatedBuilder__navigation_drawer(bool isSelected, Duration? duration = null, Func<BuildContext, Animation<double>, Widget> builder = default!)
    {
        Duration __duration = duration ?? Duration.Create(milliseconds: 200);
        this.isSelected = isSelected;
        this.duration = __duration;
        this.builder = builder;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SelectableAnimatedBuilderState__navigation_drawer());
}

public class _SelectableAnimatedBuilderState__navigation_drawer : State<_SelectableAnimatedBuilder__navigation_drawer>, SingleTickerProviderStateMixin<_SelectableAnimatedBuilder__navigation_drawer>
{
    internal virtual AnimationController _controller { get; set; } = default!;
    public virtual Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _controller = new AnimationController(vsync: this);
        _controller.duration = widget.duration;
        _controller.value = widget.isSelected ? 1.0 : 0.0;
    }

    public override void didUpdateWidget(_SelectableAnimatedBuilder__navigation_drawer oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(oldWidget.duration, widget.duration))
        {
            _controller.duration = widget.duration;
        }
        if (oldWidget.isSelected != widget.isSelected)
        {
            if (widget.isSelected)
            {
                _controller.forward();
            }
            else
            {
                _controller.reverse();
            }
        }
    }

    public override void dispose()
    {
        _controller.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((_ticker is null) || !_ticker!.isActive)
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{this} was disposed with an active Ticker."), new ErrorDescription($"{GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), _ticker!.describeForError("The offending ticker was") }));
            });
        _tickerModeNotifier?.removeListener(_updateTicker);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return widget.builder(context, _controller);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Scheduler.Ticker createTicker(Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (_ticker is null)
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
            });
        _ticker = new Scheduler.Ticker(onTick, debugLabel: Foundation.ConstantsLibrary.kDebugMode ? $"created by {DiagnosticsLibrary.describeIdentity(this)}" : null);
        _updateTickerModeNotifier();
        _updateTicker();
        return _ticker!;
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
        TickerModeData values = _tickerModeNotifier!.value;
        if (_ticker is not null)
        {
            _ticker!.muted = !values.enabled;
            _ticker!.forceFrames = values.forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTicker);
        newNotifier.addListener(_updateTicker);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription = (_ticker?.isActive, _ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) };
        properties.add(new DiagnosticsProperty<Scheduler.Ticker>("ticker", _ticker, description: tickerDescription, showSeparator: false, defaultValue: default));
    }

}

internal class _NavigationDrawerDefaultsM3__navigation_drawer : NavigationDrawerThemeData
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

    internal _NavigationDrawerDefaultsM3__navigation_drawer(BuildContext context) : base(elevation: 1.0, tileHeight: 56.0, indicatorShape: new StadiumBorder(), indicatorSize: new Size(336.0, 56.0))
    {
        this.context = context;
    }

    public override Color? backgroundColor => DartRuntimePrimitives.ConvertValue<Color>(_colors.surfaceContainerLow);
    public override Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<Color>(Colors.transparent);
    public override Color? shadowColor => DartRuntimePrimitives.ConvertValue<Color>(Colors.transparent);
    public override Color? indicatorColor => DartRuntimePrimitives.ConvertValue<Color>(_colors.secondaryContainer);
    public override WidgetStateProperty<IconThemeData?>? iconTheme
    {
        get
        {
            return (WidgetStateProperty<IconThemeData?>?)WidgetStateProperty.resolveWith((states) =>
            {
                return new IconThemeData(size: 24.0, color: states.Contains(WidgetState.disabled) ? _colors.onSurfaceVariant.withOpacity(0.38) : (states.Contains(WidgetState.selected) ? _colors.onSecondaryContainer : _colors.onSurfaceVariant));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    public override WidgetStateProperty<TextStyle?>? labelTextStyle
    {
        get
        {
            return (WidgetStateProperty<TextStyle?>?)WidgetStateProperty.resolveWith((states) =>
            {
                TextStyle style = _textTheme.labelLarge!;
                return style.apply(color: states.Contains(WidgetState.disabled) ? _colors.onSurfaceVariant.withOpacity(0.38) : (states.Contains(WidgetState.selected) ? _colors.onSecondaryContainer : _colors.onSurfaceVariant));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
}
