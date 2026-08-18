// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/navigation_drawer.dart
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
        global::Doroti.Framework.Painting.EdgeInsetsGeometry __tilePadding = tilePadding ?? global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 12.0);
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
        long totalNumberOfDestinations__6103 = checked((long)(this.children.OfType<NavigationDrawerDestination>().ToList().Count));
        var destinationIndex__6234 = 0L;
        global::Doroti.Framework.Widgets.Widget wrapChild(global::Doroti.Framework.Widgets.Widget child, long index)
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new _NavigationDrawerDestinationInfo__navigation_drawer(index: index, totalNumberOfDestinations: totalNumberOfDestinations__6103, selectedAnimation: new global::Doroti.Framework.Animation.AlwaysStoppedAnimation<double>((this.selectedIndex == index) ? 1.0 : 0.0), indicatorColor: this.indicatorColor, indicatorShape: this.indicatorShape, tilePadding: this.tilePadding, onTap: ((global::System.Action)(() => { this.onDestinationSelected?.Invoke(index); })), child: child));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        var wrappedChildren__6913 = ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection6931 = new List<global::Doroti.Framework.Widgets.Widget>(); foreach (var child__6965 in this.children) { if ((child__6965 is not NavigationDrawerDestination)) { __collection6931.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(child__6965)); } else { __collection6931.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(wrapChild(((NavigationDrawerDestination)child__6965), destinationIndex__6234++))); } } return __collection6931; }))();
        NavigationDrawerThemeData navigationDrawerTheme__7127 = NavigationDrawerTheme.of(context);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new Drawer(backgroundColor: (this.backgroundColor ?? navigationDrawerTheme__7127.backgroundColor), shadowColor: (this.shadowColor ?? navigationDrawerTheme__7127.shadowColor), surfaceTintColor: (this.surfaceTintColor ?? navigationDrawerTheme__7127.surfaceTintColor), elevation: (this.elevation ?? navigationDrawerTheme__7127.elevation), child: new global::Doroti.Framework.Widgets.SafeArea(bottom: false, child: new global::Doroti.Framework.Widgets.Column(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection7592 = new List<global::Doroti.Framework.Widgets.Widget>(); var __collectionElement7614 = this.header; if (__collectionElement7614 is { } __nonNullCollectionElement7614) { __collection7592.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement7614)); } __collection7592.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new Material(type: MaterialType.transparency, child: new global::Doroti.Framework.Widgets.ListView(children: wrappedChildren__6913))))); var __collectionElement7829 = this.footer; if (__collectionElement7829 is { } __nonNullCollectionElement7829) { __collection7592.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement7829)); } return __collection7592; }))()))));
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
        var selectedState__9998 = new HashSet<global::Doroti.Framework.Widgets.WidgetState> { global::Doroti.Framework.Widgets.WidgetState.selected };
        var unselectedState__10061 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>();
        var disabledState__10106 = new HashSet<global::Doroti.Framework.Widgets.WidgetState> { global::Doroti.Framework.Widgets.WidgetState.disabled };
        NavigationDrawerThemeData navigationDrawerTheme__10196 = NavigationDrawerTheme.of(context);
        NavigationDrawerThemeData defaults__10291 = ((NavigationDrawerThemeData)(object?)new _NavigationDrawerDefaultsM3__navigation_drawer(context));
        global::Doroti.Framework.Animation.Animation<double> animation__10369 = _NavigationDrawerDestinationInfo__navigation_drawer.of(context).selectedAnimation;
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _NavigationDestinationBuilder__navigation_drawer(buildIcon: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)((context) =>
        {
            global::Doroti.Framework.Widgets.Widget selectedIconWidget__10564 = ((global::Doroti.Framework.Widgets.Widget)(object?)IconTheme.merge(data: (navigationDrawerTheme__10196.iconTheme?.resolve((this.enabled ? selectedState__9998 : disabledState__10106)) ?? defaults__10291.iconTheme!.resolve((this.enabled ? selectedState__9998 : disabledState__10106))!), child: (this.selectedIcon ?? this.icon)));
            global::Doroti.Framework.Widgets.Widget unselectedIconWidget__10873 = ((global::Doroti.Framework.Widgets.Widget)(object?)IconTheme.merge(data: (navigationDrawerTheme__10196.iconTheme?.resolve((this.enabled ? unselectedState__10061 : disabledState__10106)) ?? defaults__10291.iconTheme!.resolve((this.enabled ? unselectedState__10061 : disabledState__10106))!), child: this.icon));
            return (((global::Doroti.Framework.Animation.Animation<double>)animation__10369).isForwardOrCompleted ? selectedIconWidget__10564 : unselectedIconWidget__10873);
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), buildLabel: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)((context) =>
        {
            global::Doroti.Framework.Painting.TextStyle? effectiveSelectedLabelTextStyle__11320 = ((navigationDrawerTheme__10196.labelTextStyle?.resolve((this.enabled ? selectedState__9998 : disabledState__10106)) ?? (global::Doroti.Framework.Painting.TextStyle)defaults__10291.labelTextStyle!.resolve((this.enabled ? selectedState__9998 : disabledState__10106))));
            global::Doroti.Framework.Painting.TextStyle? effectiveUnselectedLabelTextStyle__11597 = ((navigationDrawerTheme__10196.labelTextStyle?.resolve((this.enabled ? unselectedState__10061 : disabledState__10106)) ?? (global::Doroti.Framework.Painting.TextStyle)defaults__10291.labelTextStyle!.resolve((this.enabled ? unselectedState__10061 : disabledState__10106))));
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.DefaultTextStyle(style: (((global::Doroti.Framework.Animation.Animation<double>)animation__10369).isForwardOrCompleted ? effectiveSelectedLabelTextStyle__11320! : effectiveUnselectedLabelTextStyle__11597!), child: this.label));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), enabled: this.enabled, backgroundColor: this.backgroundColor));
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
        _NavigationDrawerDestinationInfo__navigation_drawer info__14411 = ((_NavigationDrawerDestinationInfo__navigation_drawer)(object?)_NavigationDrawerDestinationInfo__navigation_drawer.of(context));
        NavigationDrawerThemeData navigationDrawerTheme__14500 = NavigationDrawerTheme.of(context);
        NavigationDrawerThemeData defaults__14595 = ((NavigationDrawerThemeData)(object?)new _NavigationDrawerDefaultsM3__navigation_drawer(context));
        var inkWell__14655 = new InkWell(highlightColor: Colors.transparent, onTap: ((global::System.Action)(this.enabled ? ((_NavigationDrawerDestinationInfo__navigation_drawer)info__14411).onTap : null)), customBorder: ((((_NavigationDrawerDestinationInfo__navigation_drawer)info__14411).indicatorShape ?? navigationDrawerTheme__14500.indicatorShape) ?? defaults__14595.indicatorShape!), child: new global::Doroti.Framework.Widgets.Stack(alignment: global::Doroti.Framework.Painting.Alignment.center, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new NavigationIndicator(animation: ((_NavigationDrawerDestinationInfo__navigation_drawer)info__14411).selectedAnimation, color: ((((_NavigationDrawerDestinationInfo__navigation_drawer)info__14411).indicatorColor ?? navigationDrawerTheme__14500.indicatorColor) ?? defaults__14595.indicatorColor!), shape: ((((_NavigationDrawerDestinationInfo__navigation_drawer)info__14411).indicatorShape ?? navigationDrawerTheme__14500.indicatorShape) ?? defaults__14595.indicatorShape!), width: ((navigationDrawerTheme__14500.indicatorSize ?? DartRuntimePrimitives.RequireValue(defaults__14595.indicatorSize))).width, height: ((navigationDrawerTheme__14500.indicatorSize ?? DartRuntimePrimitives.RequireValue(defaults__14595.indicatorSize))).height)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Row(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(width: 16)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(this.buildIcon(context)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(width: 12)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(this.buildLabel(context)) })) }));
        global::Doroti.Framework.Widgets.Widget destination__15820 = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Padding(padding: ((_NavigationDrawerDestinationInfo__navigation_drawer)info__14411).tilePadding, child: new _NavigationDestinationSemantics__navigation_drawer(child: new global::Doroti.Framework.Widgets.SizedBox(height: (navigationDrawerTheme__14500.tileHeight ?? defaults__14595.tileHeight), child: inkWell__14655))));
        if ((this.backgroundColor is not null))
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new Ink(color: this.backgroundColor, child: destination__15820));
        }
        return destination__15820;
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
        MaterialLocalizations localizations__16946 = MaterialLocalizations.of(context);
        _NavigationDrawerDestinationInfo__navigation_drawer destinationInfo__17040 = ((_NavigationDrawerDestinationInfo__navigation_drawer)(object?)_NavigationDrawerDestinationInfo__navigation_drawer.of(context));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _StatusTransitionWidgetBuilder__navigation_drawer(animation: ((_NavigationDrawerDestinationInfo__navigation_drawer)destinationInfo__17040).selectedAnimation, builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)((context, child) =>
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(selected: ((_NavigationDrawerDestinationInfo__navigation_drawer)destinationInfo__17040).selectedAnimation.isForwardOrCompleted, container: true, child: child));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), child: new global::Doroti.Framework.Widgets.Stack(alignment: global::Doroti.Framework.Painting.Alignment.center, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(this.child), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(label: localizations__16946.tabLabel(tabIndex: (((_NavigationDrawerDestinationInfo__navigation_drawer)destinationInfo__17040).index + 1L), tabCount: ((_NavigationDrawerDestinationInfo__navigation_drawer)destinationInfo__17040).totalNumberOfDestinations))) })));
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

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context) => this.builder(context, this.child);
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
        _NavigationDrawerDestinationInfo__navigation_drawer? result__22294 = ((_NavigationDrawerDestinationInfo__navigation_drawer?)(object?)context.dependOnInheritedWidgetOfExactType<_NavigationDrawerDestinationInfo__navigation_drawer>());
        DartRuntimePrimitives.Assert(() => (result__22294 is not null), () => (object?)"Navigation destinations need a _NavigationDrawerDestinationInfo parent, " + "which is usually provided by NavigationDrawer.");
        return result__22294!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget)
    {
        var __oldWidget = (_NavigationDrawerDestinationInfo__navigation_drawer)(object)oldWidget;
        return ((((this.index != ((_NavigationDrawerDestinationInfo__navigation_drawer)__oldWidget).index) || (this.totalNumberOfDestinations != ((_NavigationDrawerDestinationInfo__navigation_drawer)__oldWidget).totalNumberOfDestinations)) || (!object.Equals(this.selectedAnimation, ((_NavigationDrawerDestinationInfo__navigation_drawer)__oldWidget).selectedAnimation))) || (!object.Equals((global::System.Action)this.onTap, (global::System.Action)((_NavigationDrawerDestinationInfo__navigation_drawer)__oldWidget).onTap)));
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
        this._controller.duration = ((_SelectableAnimatedBuilder__navigation_drawer)this.widget).duration;
        this._controller.value = (((_SelectableAnimatedBuilder__navigation_drawer)this.widget).isSelected ? 1.0 : 0.0);
    }

    public override void didUpdateWidget(_SelectableAnimatedBuilder__navigation_drawer oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((_SelectableAnimatedBuilder__navigation_drawer)oldWidget).duration, ((_SelectableAnimatedBuilder__navigation_drawer)this.widget).duration)))
        {
            this._controller.duration = ((_SelectableAnimatedBuilder__navigation_drawer)this.widget).duration;
        }
        if ((((_SelectableAnimatedBuilder__navigation_drawer)oldWidget).isSelected != ((_SelectableAnimatedBuilder__navigation_drawer)this.widget).isSelected))
        {
            if (((_SelectableAnimatedBuilder__navigation_drawer)this.widget).isSelected)
            {
                this._controller.forward();
            }
            else
            {
                this._controller.reverse();
            }
        }
    }

    public override void dispose()
    {
        this._controller.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if (((this._ticker is null) || !this._ticker!.isActive))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), this._ticker!.describeForError("The offending ticker was") }));
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return this.widget.builder(context, this._controller);
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
        this._ticker = new global::Doroti.Framework.Scheduler.Ticker((global::System.Action<Duration>)onTick, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
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
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__15400 = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__15400, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        newNotifier__15400.addListener(() => this._updateTicker());
        this._tickerModeNotifier = newNotifier__15400;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription__15805 = ((this._ticker?.isActive, this._ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) });
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Scheduler.Ticker>("ticker", this._ticker, description: tickerDescription__15805, showSeparator: false, defaultValue: default));
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

    internal _NavigationDrawerDefaultsM3__navigation_drawer(global::Doroti.Framework.Widgets.BuildContext context) : base(elevation: 1.0, tileHeight: 56.0, indicatorShape: new global::Doroti.Framework.Painting.StadiumBorder(), indicatorSize: new global::Doroti.Ui.Size(336.0, 56.0))
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.surfaceContainerLow);
    public virtual global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public virtual global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public virtual global::Doroti.Ui.Color? indicatorColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.secondaryContainer);
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Widgets.IconThemeData?>? iconTheme
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Widgets.IconThemeData?>?)(object?)WidgetStateProperty.resolveWith((states) =>
            {
                return (new global::Doroti.Framework.Widgets.IconThemeData(size: 24.0, color: (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled) ? this._colors.onSurfaceVariant.withOpacity(0.38) : (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected) ? this._colors.onSecondaryContainer : this._colors.onSurfaceVariant))));
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? labelTextStyle
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>?)(object?)WidgetStateProperty.resolveWith((states) =>
            {
                global::Doroti.Framework.Painting.TextStyle style__27751 = this._textTheme.labelLarge!;
                return (style__27751.apply(color: (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled) ? this._colors.onSurfaceVariant.withOpacity(0.38) : (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected) ? this._colors.onSecondaryContainer : this._colors.onSurfaceVariant))));
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
            return default!;
        }
    }
}
