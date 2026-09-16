// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/navigation_drawer.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class NavigationDrawer : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? indicatorColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? indicatorShape { get; private set; }
    public virtual List<global::Doroti.Framework.Widgets.Widget> children { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? header { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? footer { get; private set; }
    public virtual long? selectedIndex { get; private set; }
    public virtual global::System.Action<long>? onDestinationSelected { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry tilePadding { get; private set; } = default!;

    public NavigationDrawer(global::Doroti.Framework.Foundation.Key? key = null, List<global::Doroti.Framework.Widgets.Widget> children = default!, global::Doroti.Framework.Widgets.Widget? header = null, global::Doroti.Framework.Widgets.Widget? footer = null, Color? backgroundColor = null, Color? shadowColor = null, Color? surfaceTintColor = null, double? elevation = null, Color? indicatorColor = null, global::Doroti.Framework.Painting.ShapeBorder? indicatorShape = null, global::System.Action<long>? onDestinationSelected = null, long? selectedIndex = 0, global::Doroti.Framework.Painting.EdgeInsetsGeometry tilePadding = default!) : base(key: key)
    {
        global::Doroti.Framework.Painting.EdgeInsetsGeometry __tilePadding = tilePadding ?? EdgeInsets.CreateSymmetric(horizontal: 12.0);
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

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        long totalNumberOfDestinationsLocal = checked(children.OfType<NavigationDrawerDestination>().ToList().Count);
        var destinationIndex = 0L;
        global::Doroti.Framework.Widgets.Widget wrapChild(global::Doroti.Framework.Widgets.Widget child, long index)
        {
            return new _SelectableAnimatedBuilder__navigation_drawer(isSelected: selectedIndex == index, duration: new Duration(500_000L),
                builder: (_, animation) => new _NavigationDrawerDestinationInfo__navigation_drawer(index: index,
                    totalNumberOfDestinations: totalNumberOfDestinationsLocal, selectedAnimation: animation,
                    indicatorColor: indicatorColor, indicatorShape: indicatorShape, tilePadding: tilePadding,
                    onTap: () => onDestinationSelected?.Invoke(index), child: child));
        }
        var wrappedChildren = ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection6931 = new List<global::Doroti.Framework.Widgets.Widget>(); foreach (var childLocal in children) { if (childLocal is not NavigationDrawerDestination) { __collection6931.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(childLocal)); } else { __collection6931.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(wrapChild((NavigationDrawerDestination)childLocal, destinationIndex++))); } } return __collection6931; }))();
        NavigationDrawerThemeData navigationDrawerTheme = NavigationDrawerTheme.of(context);
        return new Drawer(backgroundColor: backgroundColor ?? navigationDrawerTheme.backgroundColor, shadowColor: shadowColor ?? navigationDrawerTheme.shadowColor, surfaceTintColor: surfaceTintColor ?? navigationDrawerTheme.surfaceTintColor, elevation: elevation ?? navigationDrawerTheme.elevation, child: new global::Doroti.Framework.Widgets.SafeArea(bottom: false, child: new global::Doroti.Framework.Widgets.Column(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection7592 = new List<global::Doroti.Framework.Widgets.Widget>(); var __collectionElement7614 = header; if (__collectionElement7614 is { } __nonNullCollectionElement7614) { __collection7592.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement7614)); } __collection7592.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new Material(type: MaterialType.transparency, child: new global::Doroti.Framework.Widgets.ListView(children: wrappedChildren))))); var __collectionElement7829 = footer; if (__collectionElement7829 is { } __nonNullCollectionElement7829) { __collection7592.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement7829)); } return __collection7592; }))())));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class NavigationDrawerDestination : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget icon { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? selectedIcon { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget label { get; private set; } = default!;
    public virtual bool enabled { get; private set; } = default!;

    public NavigationDrawerDestination(global::Doroti.Framework.Foundation.Key? key = null, Color? backgroundColor = null, global::Doroti.Framework.Widgets.Widget icon = default!, global::Doroti.Framework.Widgets.Widget? selectedIcon = null, global::Doroti.Framework.Widgets.Widget label = default!, bool enabled = true) : base(key: key)
    {
        this.backgroundColor = backgroundColor;
        this.icon = icon;
        this.selectedIcon = selectedIcon;
        this.label = label;
        this.enabled = enabled;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        var selectedState = new HashSet<global::Doroti.Framework.Widgets.WidgetState> { WidgetState.selected };
        var unselectedState = new HashSet<global::Doroti.Framework.Widgets.WidgetState>();
        var disabledState = new HashSet<global::Doroti.Framework.Widgets.WidgetState> { WidgetState.disabled };
        NavigationDrawerThemeData navigationDrawerTheme = NavigationDrawerTheme.of(context);
        NavigationDrawerThemeData defaults = new _NavigationDrawerDefaultsM3__navigation_drawer(context);
        global::Doroti.Framework.Animation.Animation<double> animation = _NavigationDrawerDestinationInfo__navigation_drawer.of(context).selectedAnimation;
        return new _NavigationDestinationBuilder__navigation_drawer(buildIcon: (context) =>
        {
            global::Doroti.Framework.Widgets.Widget selectedIconWidget = IconTheme.merge(data: navigationDrawerTheme.iconTheme?.resolve(enabled ? selectedState : disabledState) ?? defaults.iconTheme!.resolve(enabled ? selectedState : disabledState)!, child: selectedIcon ?? icon);
            global::Doroti.Framework.Widgets.Widget unselectedIconWidget = IconTheme.merge(data: navigationDrawerTheme.iconTheme?.resolve(enabled ? unselectedState : disabledState) ?? defaults.iconTheme!.resolve(enabled ? unselectedState : disabledState)!, child: icon);
            return animation.isForwardOrCompleted ? selectedIconWidget : unselectedIconWidget;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, buildLabel: (context) =>
        {
            global::Doroti.Framework.Painting.TextStyle? effectiveSelectedLabelTextStyle = navigationDrawerTheme.labelTextStyle?.resolve(enabled ? selectedState : disabledState) ?? defaults.labelTextStyle!.resolve(enabled ? selectedState : disabledState);
            global::Doroti.Framework.Painting.TextStyle? effectiveUnselectedLabelTextStyle = navigationDrawerTheme.labelTextStyle?.resolve(enabled ? unselectedState : disabledState) ?? defaults.labelTextStyle!.resolve(enabled ? unselectedState : disabledState);
            return new global::Doroti.Framework.Widgets.DefaultTextStyle(style: animation.isForwardOrCompleted ? effectiveSelectedLabelTextStyle! : effectiveUnselectedLabelTextStyle!, child: label);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, enabled: enabled, backgroundColor: backgroundColor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _NavigationDestinationBuilder__navigation_drawer : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> buildIcon { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> buildLabel { get; private set; } = default!;
    public virtual bool enabled { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }

    internal _NavigationDestinationBuilder__navigation_drawer(global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> buildIcon, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> buildLabel, bool enabled = true, Color? backgroundColor = null)
    {
        this.buildIcon = buildIcon;
        this.buildLabel = buildLabel;
        this.enabled = enabled;
        this.backgroundColor = backgroundColor;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        _NavigationDrawerDestinationInfo__navigation_drawer info = _NavigationDrawerDestinationInfo__navigation_drawer.of(context);
        NavigationDrawerThemeData navigationDrawerTheme = NavigationDrawerTheme.of(context);
        NavigationDrawerThemeData defaults = new _NavigationDrawerDefaultsM3__navigation_drawer(context);
        var inkWell = new InkWell(highlightColor: Colors.transparent, onTap: enabled ? info.onTap : null, customBorder: (info.indicatorShape ?? navigationDrawerTheme.indicatorShape) ?? defaults.indicatorShape!, child: new global::Doroti.Framework.Widgets.Stack(alignment: Alignment.center, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new NavigationIndicator(animation: info.selectedAnimation, color: (info.indicatorColor ?? navigationDrawerTheme.indicatorColor) ?? defaults.indicatorColor!, shape: (info.indicatorShape ?? navigationDrawerTheme.indicatorShape) ?? defaults.indicatorShape!, width: (navigationDrawerTheme.indicatorSize ?? DartRuntimePrimitives.RequireValue(defaults.indicatorSize)).width, height: (navigationDrawerTheme.indicatorSize ?? DartRuntimePrimitives.RequireValue(defaults.indicatorSize)).height)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Row(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(width: 16)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(buildIcon(context)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(width: 12)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(buildLabel(context)) })) }));
        global::Doroti.Framework.Widgets.Widget destination = new global::Doroti.Framework.Widgets.Padding(padding: info.tilePadding, child: new _NavigationDestinationSemantics__navigation_drawer(child: new global::Doroti.Framework.Widgets.SizedBox(height: navigationDrawerTheme.tileHeight ?? defaults.tileHeight, child: inkWell)));
        if (backgroundColor is not null)
        {
            return new Ink(color: backgroundColor, child: destination);
        }
        return destination;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _NavigationDestinationSemantics__navigation_drawer : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;

    internal _NavigationDestinationSemantics__navigation_drawer(global::Doroti.Framework.Widgets.Widget child)
    {
        this.child = child;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        _NavigationDrawerDestinationInfo__navigation_drawer destinationInfo = _NavigationDrawerDestinationInfo__navigation_drawer.of(context);
        return new _StatusTransitionWidgetBuilder__navigation_drawer(animation: destinationInfo.selectedAnimation, builder: (context, child) =>
        {
            return new global::Doroti.Framework.Widgets.Semantics(selected: destinationInfo.selectedAnimation.isForwardOrCompleted, container: true, child: child);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, child: new global::Doroti.Framework.Widgets.Stack(alignment: Alignment.center, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(child), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(label: localizations.tabLabel(tabIndex: destinationInfo.index + 1L, tabCount: destinationInfo.totalNumberOfDestinations))) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _StatusTransitionWidgetBuilder__navigation_drawer : global::Doroti.Framework.Widgets.StatusTransitionWidget
{
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget> builder { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }

    internal _StatusTransitionWidgetBuilder__navigation_drawer(global::Doroti.Framework.Animation.Animation<double> animation, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget> builder, global::Doroti.Framework.Widgets.Widget? child = null) : base(animation: animation)
    {
        this.builder = builder;
        this.child = child;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context) => builder(context, child);
}

internal class _NavigationDrawerDestinationInfo__navigation_drawer : global::Doroti.Framework.Widgets.InheritedWidget
{
    public virtual long index { get; private set; } = default!;
    public virtual long totalNumberOfDestinations { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> selectedAnimation { get; private set; } = default!;
    public virtual Color? indicatorColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? indicatorShape { get; private set; }
    public virtual global::System.Action onTap { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry tilePadding { get; private set; } = default!;

    internal _NavigationDrawerDestinationInfo__navigation_drawer(long index, long totalNumberOfDestinations, global::Doroti.Framework.Animation.Animation<double> selectedAnimation, Color? indicatorColor, global::Doroti.Framework.Painting.ShapeBorder? indicatorShape, global::System.Action onTap, global::Doroti.Framework.Widgets.Widget child, global::Doroti.Framework.Painting.EdgeInsetsGeometry tilePadding) : base(child: child)
    {
        this.index = index;
        this.totalNumberOfDestinations = totalNumberOfDestinations;
        this.selectedAnimation = selectedAnimation;
        this.indicatorColor = indicatorColor;
        this.indicatorShape = indicatorShape;
        this.onTap = onTap;
        this.tilePadding = tilePadding;
    }

    public static _NavigationDrawerDestinationInfo__navigation_drawer of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        _NavigationDrawerDestinationInfo__navigation_drawer? result = context.dependOnInheritedWidgetOfExactType<_NavigationDrawerDestinationInfo__navigation_drawer>();
        DartRuntimePrimitives.Assert(() => result is not null, () => (object?)"Navigation destinations need a _NavigationDrawerDestinationInfo parent, " + "which is usually provided by NavigationDrawer.");
        return result!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget)
    {
        var __oldWidget = (_NavigationDrawerDestinationInfo__navigation_drawer)oldWidget;
        return (index != __oldWidget.index) || (totalNumberOfDestinations != __oldWidget.totalNumberOfDestinations) || (!Equals(selectedAnimation, __oldWidget.selectedAnimation)) || (!Equals(onTap, __oldWidget.onTap));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _SelectableAnimatedBuilder__navigation_drawer : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual bool isSelected { get; private set; } = default!;
    public virtual Duration duration { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget> builder { get; private set; } = default!;

    internal _SelectableAnimatedBuilder__navigation_drawer(bool isSelected, Duration? duration = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget> builder = default!)
    {
        Duration __duration = duration ?? Duration.Create(milliseconds: 200);
        this.isSelected = isSelected;
        this.duration = __duration;
        this.builder = builder;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SelectableAnimatedBuilderState__navigation_drawer());
}

public class _SelectableAnimatedBuilderState__navigation_drawer : global::Doroti.Framework.Widgets.State<_SelectableAnimatedBuilder__navigation_drawer>, global::Doroti.Framework.Widgets.SingleTickerProviderStateMixin<_SelectableAnimatedBuilder__navigation_drawer>
{
    internal virtual global::Doroti.Framework.Animation.AnimationController _controller { get; set; } = default!;
    public virtual global::Doroti.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _controller = new global::Doroti.Framework.Animation.AnimationController(vsync: this);
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
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), _ticker!.describeForError("The offending ticker was") }));
            });
        _tickerModeNotifier?.removeListener(_updateTicker);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return widget.builder(context, _controller);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (_ticker is null)
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new global::Doroti.Framework.Foundation.ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new global::Doroti.Framework.Foundation.ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
            });
        _ticker = new global::Doroti.Framework.Scheduler.Ticker(onTick, debugLabel: Foundation.ConstantsLibrary.kDebugMode ? $"created by {DiagnosticsLibrary.describeIdentity(this)}" : null);
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
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTicker);
        newNotifier.addListener(_updateTicker);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription = (_ticker?.isActive, _ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) };
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Scheduler.Ticker>("ticker", _ticker, description: tickerDescription, showSeparator: false, defaultValue: default));
    }

}

internal class _NavigationDrawerDefaultsM3__navigation_drawer : NavigationDrawerThemeData
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

    internal _NavigationDrawerDefaultsM3__navigation_drawer(global::Doroti.Framework.Widgets.BuildContext context) : base(elevation: 1.0, tileHeight: 56.0, indicatorShape: new global::Doroti.Framework.Painting.StadiumBorder(), indicatorSize: new global::Doroti.Ui.Size(336.0, 56.0))
    {
        this.context = context;
    }

    public override global::Doroti.Ui.Color? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.surfaceContainerLow);
    public override global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public override global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public override global::Doroti.Ui.Color? indicatorColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.secondaryContainer);
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Widgets.IconThemeData?>? iconTheme
    {
        get
        {
            return (global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Widgets.IconThemeData?>?)WidgetStateProperty.resolveWith((states) =>
            {
                return new global::Doroti.Framework.Widgets.IconThemeData(size: 24.0, color: states.Contains(WidgetState.disabled) ? _colors.onSurfaceVariant.withOpacity(0.38) : (states.Contains(WidgetState.selected) ? _colors.onSecondaryContainer : _colors.onSurfaceVariant));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? labelTextStyle
    {
        get
        {
            return (global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>?)WidgetStateProperty.resolveWith((states) =>
            {
                global::Doroti.Framework.Painting.TextStyle style = _textTheme.labelLarge!;
                return style.apply(color: states.Contains(WidgetState.disabled) ? _colors.onSurfaceVariant.withOpacity(0.38) : (states.Contains(WidgetState.selected) ? _colors.onSecondaryContainer : _colors.onSurfaceVariant));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
}
