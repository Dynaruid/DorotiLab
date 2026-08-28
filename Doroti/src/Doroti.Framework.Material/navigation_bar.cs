// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/navigation_bar.dart
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

public static partial class Navigation_barLibrary
{
    internal static double _kIndicatorHeight = 32;
}

public static partial class Navigation_barLibrary
{
    internal static double _kIndicatorWidth = 64;
}

public static partial class Navigation_barLibrary
{
    internal static double _kMaxLabelTextScaleFactor = 1.3;
}

public class NavigationBar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual Duration? animationDuration { get; private set; }
    public virtual long selectedIndex { get; private set; } = default!;
    public virtual List<global::Doroti.Framework.Widgets.Widget> destinations { get; private set; } = default!;
    public virtual global::System.Action<long>? onDestinationSelected { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual Color? indicatorColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? indicatorShape { get; private set; }
    public virtual double? height { get; private set; }
    public virtual NavigationDestinationLabelBehavior? labelBehavior { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? labelTextStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? labelPadding { get; private set; }
    public virtual bool maintainBottomViewPadding { get; private set; } = default!;

    public NavigationBar(global::Doroti.Framework.Foundation.Key? key = null, Duration? animationDuration = null, long selectedIndex = 0, List<global::Doroti.Framework.Widgets.Widget> destinations = default!, global::System.Action<long>? onDestinationSelected = null, Color? backgroundColor = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, Color? indicatorColor = null, global::Doroti.Framework.Painting.ShapeBorder? indicatorShape = null, double? height = null, NavigationDestinationLabelBehavior? labelBehavior = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? labelTextStyle = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? labelPadding = null, bool maintainBottomViewPadding = false) : base(key: key)
    {
        this.animationDuration = animationDuration;
        this.selectedIndex = selectedIndex;
        this.destinations = destinations;
        this.onDestinationSelected = onDestinationSelected;
        this.backgroundColor = backgroundColor;
        this.elevation = elevation;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.indicatorColor = indicatorColor;
        this.indicatorShape = indicatorShape;
        this.height = height;
        this.labelBehavior = labelBehavior;
        this.overlayColor = overlayColor;
        this.labelTextStyle = labelTextStyle;
        this.labelPadding = labelPadding;
        this.maintainBottomViewPadding = maintainBottomViewPadding;
        System.Diagnostics.Debug.Assert((checked((long)(destinations.Count)) >= 2L));
        System.Diagnostics.Debug.Assert(((0L <= selectedIndex) && (selectedIndex < checked((long)(destinations.Count)))));
    }

    internal virtual global::System.Action _handleTap(long index)
    {
        return ((global::System.Action)((global::System.Action)((this.onDestinationSelected is not null) ? (() => { this.onDestinationSelected!(index); }) : (() =>
        {
        }))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        NavigationBarThemeData defaults = Navigation_barLibrary._defaultsFor(context);
        NavigationBarThemeData navigationBarTheme = NavigationBarTheme.of(context);
        double effectiveHeight = ((this.height ?? navigationBarTheme.height) ?? DartRuntimePrimitives.RequireValue(defaults.height));
        NavigationDestinationLabelBehavior effectiveLabelBehavior = ((this.labelBehavior ?? navigationBarTheme.labelBehavior) ?? DartRuntimePrimitives.RequireValue(defaults.labelBehavior));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new Material(color: ((this.backgroundColor ?? navigationBarTheme.backgroundColor) ?? defaults.backgroundColor!), elevation: ((this.elevation ?? navigationBarTheme.elevation) ?? DartRuntimePrimitives.RequireValue(defaults.elevation)), shadowColor: ((this.shadowColor ?? navigationBarTheme.shadowColor) ?? defaults.shadowColor), surfaceTintColor: ((this.surfaceTintColor ?? navigationBarTheme.surfaceTintColor) ?? defaults.surfaceTintColor), child: new global::Doroti.Framework.Widgets.SafeArea(maintainBottomViewPadding: this.maintainBottomViewPadding, child: new global::Doroti.Framework.Widgets.Semantics(role: SemanticsRole.tabBar, explicitChildNodes: true, container: true, child: new global::Doroti.Framework.Widgets.SizedBox(height: effectiveHeight, child: new global::Doroti.Framework.Widgets.Row(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() =>
        {
            var __collection12378 = new List<global::Doroti.Framework.Widgets.Widget>(); for (long i = 0L; (i < checked((long)(this.destinations.Count))); i++)
            {
                var destinationIndex__g65 = i; __collection12378.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.MergeSemantics(child: new global::Doroti.Framework.Widgets.Semantics(role: SemanticsRole.tab, selected: (destinationIndex__g65 == this.selectedIndex), child: new _SelectableAnimatedBuilder__navigation_bar(duration: (this.animationDuration ?? Duration.Create(milliseconds: 500L)), isSelected: (destinationIndex__g65 == this.selectedIndex), builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget>)((context, animation) =>
                {
                    return ((global::Doroti.Framework.Widgets.Widget)(object?)new _NavigationDestinationInfo__navigation_bar(index: destinationIndex__g65, selectedIndex: this.selectedIndex, totalNumberOfDestinations: checked((long)(this.destinations.Count)), selectedAnimation: animation, labelBehavior: effectiveLabelBehavior, indicatorColor: this.indicatorColor, indicatorShape: this.indicatorShape, overlayColor: this.overlayColor, onTap: _handleTap(destinationIndex__g65), labelTextStyle: this.labelTextStyle, labelPadding: this.labelPadding, child: this.destinations[(int)(destinationIndex__g65)]));
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }))))))));
            }
            return __collection12378;
        }))()))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public enum NavigationDestinationLabelBehavior
{
    alwaysShow,
    alwaysHide,
    onlyShowSelected
}

public class NavigationDestination : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget icon { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? selectedIcon { get; private set; }
    public virtual string label { get; private set; } = default!;
    public virtual string? tooltip { get; private set; }
    public virtual bool enabled { get; private set; } = default!;

    public NavigationDestination(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget icon = default!, global::Doroti.Framework.Widgets.Widget? selectedIcon = null, string label = default!, string? tooltip = null, bool enabled = true) : base(key: key)
    {
        this.icon = icon;
        this.selectedIcon = selectedIcon;
        this.label = label;
        this.tooltip = tooltip;
        this.enabled = enabled;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        _NavigationDestinationInfo__navigation_bar info = ((_NavigationDestinationInfo__navigation_bar)(object?)_NavigationDestinationInfo__navigation_bar.of(context));
        var selectedState = new HashSet<global::Doroti.Framework.Widgets.WidgetState> { global::Doroti.Framework.Widgets.WidgetState.selected };
        var unselectedState = new HashSet<global::Doroti.Framework.Widgets.WidgetState>();
        var disabledState = new HashSet<global::Doroti.Framework.Widgets.WidgetState> { global::Doroti.Framework.Widgets.WidgetState.disabled };
        NavigationBarThemeData navigationBarTheme = NavigationBarTheme.of(context);
        NavigationBarThemeData defaults = Navigation_barLibrary._defaultsFor(context);
        global::Doroti.Framework.Animation.Animation<double> animationLocal = ((_NavigationDestinationInfo__navigation_bar)info).selectedAnimation;
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _NavigationDestinationBuilder__navigation_bar(label: this.label, tooltip: this.tooltip, enabled: this.enabled, buildIcon: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)((context) =>
        {
            global::Doroti.Framework.Widgets.IconThemeData selectedIconTheme = (navigationBarTheme.iconTheme?.resolve(selectedState) ?? defaults.iconTheme!.resolve(selectedState)!);
            global::Doroti.Framework.Widgets.IconThemeData unselectedIconTheme = (navigationBarTheme.iconTheme?.resolve(unselectedState) ?? defaults.iconTheme!.resolve(unselectedState)!);
            global::Doroti.Framework.Widgets.IconThemeData disabledIconTheme = (navigationBarTheme.iconTheme?.resolve(disabledState) ?? defaults.iconTheme!.resolve(disabledState)!);
            global::Doroti.Framework.Widgets.Widget selectedIconWidget = ((global::Doroti.Framework.Widgets.Widget)(object?)IconTheme.merge(data: (this.enabled ? selectedIconTheme : disabledIconTheme), child: (this.selectedIcon ?? this.icon)));
            global::Doroti.Framework.Widgets.Widget unselectedIconWidget = ((global::Doroti.Framework.Widgets.Widget)(object?)IconTheme.merge(data: (this.enabled ? unselectedIconTheme : disabledIconTheme), child: this.icon));
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Stack(alignment: global::Doroti.Framework.Painting.Alignment.center, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new NavigationIndicator(animation: animationLocal, color: ((((_NavigationDestinationInfo__navigation_bar)info).indicatorColor ?? navigationBarTheme.indicatorColor) ?? defaults.indicatorColor!), shape: ((((_NavigationDestinationInfo__navigation_bar)info).indicatorShape ?? navigationBarTheme.indicatorShape) ?? defaults.indicatorShape!))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _StatusTransitionWidgetBuilder__navigation_bar(animation: animationLocal, builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)((context, child) => {
return (((global::Doroti.Framework.Animation.Animation<double>)animationLocal).isForwardOrCompleted ? selectedIconWidget : unselectedIconWidget);
throw new InvalidOperationException("Dart closure completed without a value.");
})))) }));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), buildLabel: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)((context) =>
        {
            global::Doroti.Framework.Painting.TextStyle? effectiveSelectedLabelTextStyle = ((((((_NavigationDestinationInfo__navigation_bar)info).labelTextStyle?.resolve(selectedState) ?? (global::Doroti.Framework.Painting.TextStyle)navigationBarTheme.labelTextStyle?.resolve(selectedState))) ?? (global::Doroti.Framework.Painting.TextStyle)defaults.labelTextStyle!.resolve(selectedState)));
            global::Doroti.Framework.Painting.TextStyle? effectiveUnselectedLabelTextStyle = ((((((_NavigationDestinationInfo__navigation_bar)info).labelTextStyle?.resolve(unselectedState) ?? (global::Doroti.Framework.Painting.TextStyle)navigationBarTheme.labelTextStyle?.resolve(unselectedState))) ?? (global::Doroti.Framework.Painting.TextStyle)defaults.labelTextStyle!.resolve(unselectedState)));
            global::Doroti.Framework.Painting.TextStyle? effectiveDisabledLabelTextStyle = ((((((_NavigationDestinationInfo__navigation_bar)info).labelTextStyle?.resolve(disabledState) ?? (global::Doroti.Framework.Painting.TextStyle)navigationBarTheme.labelTextStyle?.resolve(disabledState))) ?? (global::Doroti.Framework.Painting.TextStyle)defaults.labelTextStyle!.resolve(disabledState)));
            global::Doroti.Framework.Painting.EdgeInsetsGeometry labelPaddingLocal = ((((_NavigationDestinationInfo__navigation_bar)info).labelPadding ?? navigationBarTheme.labelPadding) ?? defaults.labelPadding!);
            var textStyle = (this.enabled ? (((global::Doroti.Framework.Animation.Animation<double>)animationLocal).isForwardOrCompleted ? effectiveSelectedLabelTextStyle : effectiveUnselectedLabelTextStyle) : effectiveDisabledLabelTextStyle);
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Padding(padding: labelPaddingLocal, child: MediaQuery.withClampedTextScaling(maxScaleFactor: Navigation_barLibrary._kMaxLabelTextScaleFactor, child: new global::Doroti.Framework.Widgets.Text(this.label, style: textStyle))));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _NavigationDestinationBuilder__navigation_bar : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> buildIcon { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> buildLabel { get; private set; } = default!;
    public virtual string label { get; private set; } = default!;
    public virtual string? tooltip { get; private set; }
    public virtual bool enabled { get; private set; } = default!;

    internal _NavigationDestinationBuilder__navigation_bar(global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> buildIcon, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> buildLabel, string label, string? tooltip = null, bool enabled = true)
    {
        this.buildIcon = buildIcon;
        this.buildLabel = buildLabel;
        this.label = label;
        this.tooltip = tooltip;
        this.enabled = enabled;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _NavigationDestinationBuilderState__navigation_bar());
}

internal class _NavigationDestinationBuilderState__navigation_bar : global::Doroti.Framework.Widgets.State<_NavigationDestinationBuilder__navigation_bar>
{
    public virtual global::Doroti.Framework.Widgets.GlobalKey<IState> iconKey { get; private set; } = global::Doroti.Framework.Widgets.GlobalKey<IState>.Create();

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        _NavigationDestinationInfo__navigation_bar info = ((_NavigationDestinationInfo__navigation_bar)(object?)_NavigationDestinationInfo__navigation_bar.of(context));
        NavigationBarThemeData navigationBarTheme = NavigationBarTheme.of(context);
        NavigationBarThemeData defaults = Navigation_barLibrary._defaultsFor(context);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _NavigationBarDestinationSemantics__navigation_bar(enabled: ((_NavigationDestinationBuilder__navigation_bar)this.widget).enabled, child: new _NavigationBarDestinationTooltip__navigation_bar(message: (((_NavigationDestinationBuilder__navigation_bar)this.widget).tooltip ?? ((_NavigationDestinationBuilder__navigation_bar)this.widget).label), child: new _IndicatorInkWell__navigation_bar(iconKey: this.iconKey, labelBehavior: ((_NavigationDestinationInfo__navigation_bar)info).labelBehavior, customBorder: ((((_NavigationDestinationInfo__navigation_bar)info).indicatorShape ?? navigationBarTheme.indicatorShape) ?? defaults.indicatorShape), overlayColor: (((_NavigationDestinationInfo__navigation_bar)info).overlayColor ?? navigationBarTheme.overlayColor), onTap: ((global::System.Action)(((_NavigationDestinationBuilder__navigation_bar)this.widget).enabled ? ((_NavigationDestinationInfo__navigation_bar)info).onTap : null)), child: new global::Doroti.Framework.Widgets.Row(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new _NavigationBarDestinationLayout__navigation_bar(icon: this.widget.buildIcon(context), iconKey: this.iconKey, label: this.widget.buildLabel(context)))) })))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _IndicatorInkWell__navigation_bar : InkResponse
{
    public virtual global::Doroti.Framework.Widgets.GlobalKey<IState> iconKey { get; private set; } = default!;
    public virtual NavigationDestinationLabelBehavior labelBehavior { get; private set; } = default!;

    internal _IndicatorInkWell__navigation_bar(global::Doroti.Framework.Widgets.GlobalKey<IState> iconKey, NavigationDestinationLabelBehavior labelBehavior, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, global::Doroti.Framework.Painting.ShapeBorder? customBorder = null, global::System.Action? onTap = null, global::Doroti.Framework.Widgets.Widget? child = null) : base(overlayColor: overlayColor, customBorder: customBorder, onTap: onTap, child: child, containedInkWell: true, highlightColor: Colors.transparent)
    {
        this.iconKey = iconKey;
        this.labelBehavior = labelBehavior;
    }

    public virtual RectCallback? getRectCallback(global::Doroti.Framework.Rendering.RenderBox referenceBox)
    {
        return ((RectCallback?)(object?)(() =>
        {
            var iconBox = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((global::Doroti.Framework.Widgets.GlobalKey<IState>)this.iconKey).currentContext!.findRenderObject()!)!;
            global::Doroti.Ui.Rect iconRect = ((global::Doroti.Ui.Rect)(object?)(((Offset)((dynamic)iconBox).localToGlobal(Offset.zero)) & ((global::Doroti.Framework.Rendering.RenderBox)iconBox).size));
            return (((Offset)((dynamic)referenceBox).globalToLocal(iconRect.topLeft)) & ((global::Doroti.Framework.Rendering.RenderBox)iconBox).size);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _NavigationDestinationInfo__navigation_bar : global::Doroti.Framework.Widgets.InheritedWidget
{
    public virtual long index { get; private set; } = default!;
    public virtual long selectedIndex { get; private set; } = default!;
    public virtual long totalNumberOfDestinations { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> selectedAnimation { get; private set; } = default!;
    public virtual NavigationDestinationLabelBehavior labelBehavior { get; private set; } = default!;
    public virtual Color? indicatorColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? indicatorShape { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual global::System.Action onTap { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? labelTextStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? labelPadding { get; private set; }

    internal _NavigationDestinationInfo__navigation_bar(long index, long selectedIndex, long totalNumberOfDestinations, global::Doroti.Framework.Animation.Animation<double> selectedAnimation, NavigationDestinationLabelBehavior labelBehavior, Color? indicatorColor, global::Doroti.Framework.Painting.ShapeBorder? indicatorShape, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor, global::System.Action onTap, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? labelTextStyle = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? labelPadding = null, global::Doroti.Framework.Widgets.Widget child = default!) : base(child: child)
    {
        this.index = index;
        this.selectedIndex = selectedIndex;
        this.totalNumberOfDestinations = totalNumberOfDestinations;
        this.selectedAnimation = selectedAnimation;
        this.labelBehavior = labelBehavior;
        this.indicatorColor = indicatorColor;
        this.indicatorShape = indicatorShape;
        this.overlayColor = overlayColor;
        this.onTap = onTap;
        this.labelTextStyle = labelTextStyle;
        this.labelPadding = labelPadding;
    }

    public static _NavigationDestinationInfo__navigation_bar of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        _NavigationDestinationInfo__navigation_bar? result = ((_NavigationDestinationInfo__navigation_bar?)(object?)context.dependOnInheritedWidgetOfExactType<_NavigationDestinationInfo__navigation_bar>());
        DartRuntimePrimitives.Assert(() => (result is not null), () => (object?)"Navigation destinations need a _NavigationDestinationInfo parent, " + "which is usually provided by NavigationBar.");
        return result!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget)
    {
        var __oldWidget = (_NavigationDestinationInfo__navigation_bar)(object)oldWidget;
        return (((((this.index != ((_NavigationDestinationInfo__navigation_bar)__oldWidget).index) || (this.totalNumberOfDestinations != ((_NavigationDestinationInfo__navigation_bar)__oldWidget).totalNumberOfDestinations)) || (!object.Equals(this.selectedAnimation, ((_NavigationDestinationInfo__navigation_bar)__oldWidget).selectedAnimation))) || (!object.Equals(this.labelBehavior, ((_NavigationDestinationInfo__navigation_bar)__oldWidget).labelBehavior))) || (!object.Equals((global::System.Action)this.onTap, (global::System.Action)((_NavigationDestinationInfo__navigation_bar)__oldWidget).onTap)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class NavigationIndicator : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual Color? color { get; private set; }
    public virtual double width { get; private set; } = default!;
    public virtual double height { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.BorderRadius borderRadius { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape { get; private set; }

    public NavigationIndicator(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Animation.Animation<double> animation = default!, Color? color = null, double? width = null, double? height = null, global::Doroti.Framework.Painting.BorderRadius borderRadius = default!, global::Doroti.Framework.Painting.ShapeBorder? shape = null) : base(key: key)
    {
        double __width = width ?? Navigation_barLibrary._kIndicatorWidth;
        double __height = height ?? Navigation_barLibrary._kIndicatorHeight;
        global::Doroti.Framework.Painting.BorderRadius __borderRadius = borderRadius ?? global::Doroti.Framework.Painting.BorderRadius.CreateAll(Radius.circular(16));
        this.animation = animation;
        this.color = color;
        this.width = __width;
        this.height = __height;
        this.borderRadius = __borderRadius;
        this.shape = shape;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: this.animation, builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)((context, child) =>
        {
            double scale = (((global::Doroti.Framework.Animation.Animation<double>)this.animation).isDismissed ? 0.0 : new global::Doroti.Framework.Animation.Tween<double>(begin: 0.4, end: 1.0).transform(new global::Doroti.Framework.Animation.CurveTween(curve: global::Doroti.Framework.Animation.Curves.easeInOutCubicEmphasized).transform(((global::Doroti.Framework.Animation.Animation<double>)this.animation).value)));
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Transform(alignment: global::Doroti.Framework.Painting.Alignment.center, transform: Matrix4.diagonal3Values(scale, 1.0, 1.0), child: child));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), child: new _StatusTransitionWidgetBuilder__navigation_bar(animation: this.animation, builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)((context, child) =>
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new _SelectableAnimatedBuilder__navigation_bar(isSelected: ((global::Doroti.Framework.Animation.Animation<double>)this.animation).isForwardOrCompleted, duration: Duration.Create(milliseconds: 100L), alwaysDoFullAnimation: true, builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget>)((context, fadeAnimation) =>
            {
                return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeAnimation, child: new Ink(width: DartRuntimePrimitives.RequireValue(this.width), height: DartRuntimePrimitives.RequireValue(this.height), decoration: new global::Doroti.Framework.Painting.ShapeDecoration(shape: (this.shape ?? new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: this.borderRadius)), color: (this.color ?? Theme.of(context).colorScheme.secondary)))));
                throw new InvalidOperationException("Dart closure completed without a value.");
            }))));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _NavigationBarDestinationLayout__navigation_bar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget icon { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.GlobalKey<IState> iconKey { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget label { get; private set; } = default!;

    internal _NavigationBarDestinationLayout__navigation_bar(global::Doroti.Framework.Widgets.Widget icon, global::Doroti.Framework.Widgets.GlobalKey<IState> iconKey, global::Doroti.Framework.Widgets.Widget label)
    {
        this.icon = icon;
        this.iconKey = iconKey;
        this.label = label;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _DestinationLayoutAnimationBuilder__navigation_bar(builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget>)((context, animation) =>
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.CustomMultiChildLayout(@delegate: new _NavigationDestinationLayoutDelegate__navigation_bar(animation: animation), children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.LayoutId(id: _NavigationDestinationLayoutDelegate__navigation_bar.iconId, child: new global::Doroti.Framework.Widgets.KeyedSubtree(key: this.iconKey, child: this.icon))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.LayoutId(id: _NavigationDestinationLayoutDelegate__navigation_bar.labelId, child: new global::Doroti.Framework.Widgets.FadeTransition(alwaysIncludeSemantics: true, opacity: animation, child: this.label))) }));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DestinationLayoutAnimationBuilder__navigation_bar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget> builder { get; private set; } = default!;

    internal _DestinationLayoutAnimationBuilder__navigation_bar(global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget> builder)
    {
        this.builder = builder;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        _NavigationDestinationInfo__navigation_bar info = ((_NavigationDestinationInfo__navigation_bar)(object?)_NavigationDestinationInfo__navigation_bar.of(context));
        switch (((_NavigationDestinationInfo__navigation_bar)info).labelBehavior)
        {
            case NavigationDestinationLabelBehavior.alwaysShow:
                {
                    return this.builder(context, global::Doroti.Framework.Animation.AnimationsLibrary.kAlwaysCompleteAnimation);
                }
            case NavigationDestinationLabelBehavior.alwaysHide:
                {
                    return this.builder(context, global::Doroti.Framework.Animation.AnimationsLibrary.kAlwaysDismissedAnimation);
                }
            case NavigationDestinationLabelBehavior.onlyShowSelected:
                {
                    return ((global::Doroti.Framework.Widgets.Widget)(object?)new _CurvedAnimationBuilder__navigation_bar(animation: ((_NavigationDestinationInfo__navigation_bar)info).selectedAnimation, curve: global::Doroti.Framework.Animation.Curves.easeInOutCubicEmphasized, reverseCurve: global::Doroti.Framework.Animation.Curves.easeInOutCubicEmphasized.flipped, builder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget>)this.builder));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _NavigationBarDestinationSemantics__navigation_bar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual bool enabled { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;

    internal _NavigationBarDestinationSemantics__navigation_bar(bool enabled, global::Doroti.Framework.Widgets.Widget child)
    {
        this.enabled = enabled;
        this.child = child;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        _NavigationDestinationInfo__navigation_bar destinationInfo = ((_NavigationDestinationInfo__navigation_bar)(object?)_NavigationDestinationInfo__navigation_bar.of(context));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _StatusTransitionWidgetBuilder__navigation_bar(animation: ((_NavigationDestinationInfo__navigation_bar)destinationInfo).selectedAnimation, builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)((context, child) =>
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(enabled: this.enabled, button: true, child: child));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), child: (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb ? this.child : new global::Doroti.Framework.Widgets.Stack(alignment: global::Doroti.Framework.Painting.Alignment.center, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(this.child), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(label: localizations.tabLabel(tabIndex: (((_NavigationDestinationInfo__navigation_bar)destinationInfo).index + 1L), tabCount: ((_NavigationDestinationInfo__navigation_bar)destinationInfo).totalNumberOfDestinations))) }))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _NavigationBarDestinationTooltip__navigation_bar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual string message { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;

    internal _NavigationBarDestinationTooltip__navigation_bar(string message, global::Doroti.Framework.Widgets.Widget child)
    {
        this.message = message;
        this.child = child;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new Tooltip(message: this.message, verticalOffset: 42, excludeFromSemantics: true, preferBelow: false, child: this.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _NavigationDestinationLayoutDelegate__navigation_bar : global::Doroti.Framework.Rendering.MultiChildLayoutDelegate
{
    public virtual global::Doroti.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public const long iconId = 1L;
    public const long labelId = 2L;

    internal _NavigationDestinationLayoutDelegate__navigation_bar(global::Doroti.Framework.Animation.Animation<double> animation) : base(relayout: animation)
    {
        this.animation = animation;
    }

    public override void performLayout(Size size)
    {
        double halfWidth(Size size)
        {
            return (size.width / 2L);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        double halfHeight(Size size)
        {
            return (size.height / 2L);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        global::Doroti.Ui.Size iconSize = ((global::Doroti.Ui.Size)(object?)layoutChild(iconId, global::Doroti.Framework.Rendering.BoxConstraints.CreateLoose(size)));
        global::Doroti.Ui.Size labelSize = ((global::Doroti.Ui.Size)(object?)layoutChild(labelId, global::Doroti.Framework.Rendering.BoxConstraints.CreateLoose(size)));
        double yPositionOffset = new global::Doroti.Framework.Animation.Tween<double>(begin: halfHeight(iconSize), end: (halfHeight(iconSize) + halfHeight(labelSize))).transform(((global::Doroti.Framework.Animation.Animation<double>)this.animation).value);
        double iconYPosition = (halfHeight(size) - yPositionOffset);
        positionChild(iconId, new global::Doroti.Ui.Offset((halfWidth(size) - halfWidth(iconSize)), iconYPosition));
        positionChild(labelId, new global::Doroti.Ui.Offset((halfWidth(size) - halfWidth(labelSize)), (iconYPosition + iconSize.height)));
    }

    public override bool shouldRelayout(global::Doroti.Framework.Rendering.MultiChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (_NavigationDestinationLayoutDelegate__navigation_bar)(object)oldDelegate;
        return (!object.Equals(((_NavigationDestinationLayoutDelegate__navigation_bar)__oldDelegate).animation, this.animation));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _StatusTransitionWidgetBuilder__navigation_bar : global::Doroti.Framework.Widgets.StatusTransitionWidget
{
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget> builder { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }

    internal _StatusTransitionWidgetBuilder__navigation_bar(global::Doroti.Framework.Animation.Animation<double> animation, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget> builder, global::Doroti.Framework.Widgets.Widget? child = null) : base(animation: animation)
    {
        this.builder = builder;
        this.child = child;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context) => this.builder(context, this.child);
}

public class _SelectableAnimatedBuilder__navigation_bar : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual bool isSelected { get; private set; } = default!;
    public virtual Duration duration { get; private set; } = default!;
    public virtual bool alwaysDoFullAnimation { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget> builder { get; private set; } = default!;

    internal _SelectableAnimatedBuilder__navigation_bar(bool isSelected, Duration? duration = null, bool alwaysDoFullAnimation = false, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget> builder = default!)
    {
        Duration __duration = duration ?? Duration.Create(milliseconds: 200);
        this.isSelected = isSelected;
        this.duration = __duration;
        this.alwaysDoFullAnimation = alwaysDoFullAnimation;
        this.builder = builder;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SelectableAnimatedBuilderState__navigation_bar());
}

public class _SelectableAnimatedBuilderState__navigation_bar : global::Doroti.Framework.Widgets.State<_SelectableAnimatedBuilder__navigation_bar>, global::Doroti.Framework.Widgets.SingleTickerProviderStateMixin<_SelectableAnimatedBuilder__navigation_bar>
{
    internal virtual global::Doroti.Framework.Animation.AnimationController _controller { get; set; } = default!;
    public virtual global::Doroti.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _controller = new global::Doroti.Framework.Animation.AnimationController(vsync: this);
        this._controller.duration = ((_SelectableAnimatedBuilder__navigation_bar)this.widget).duration;
        this._controller.value = (((_SelectableAnimatedBuilder__navigation_bar)this.widget).isSelected ? 1.0 : 0.0);
    }

    public override void didUpdateWidget(_SelectableAnimatedBuilder__navigation_bar oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((_SelectableAnimatedBuilder__navigation_bar)oldWidget).duration, ((_SelectableAnimatedBuilder__navigation_bar)this.widget).duration)))
        {
            this._controller.duration = ((_SelectableAnimatedBuilder__navigation_bar)this.widget).duration;
        }
        if ((((_SelectableAnimatedBuilder__navigation_bar)oldWidget).isSelected != ((_SelectableAnimatedBuilder__navigation_bar)this.widget).isSelected))
        {
            if (((_SelectableAnimatedBuilder__navigation_bar)this.widget).isSelected)
            {
                this._controller.forward(from: (((_SelectableAnimatedBuilder__navigation_bar)this.widget).alwaysDoFullAnimation ? 0 : null));
            }
            else
            {
                this._controller.reverse(from: (((_SelectableAnimatedBuilder__navigation_bar)this.widget).alwaysDoFullAnimation ? 1 : null));
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
        this._tickerModeNotifier?.removeListener(this._updateTicker);
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
        TickerModeData values = this._tickerModeNotifier!.value;
        if ((this._ticker is not null))
        {
            this._ticker!.muted = !((TickerModeData)values).enabled;
            this._ticker!.forceFrames = ((TickerModeData)values).forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier, this._tickerModeNotifier)))
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

internal class _CurvedAnimationBuilder__navigation_bar : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Curve curve { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Curve reverseCurve { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget> builder { get; private set; } = default!;

    internal _CurvedAnimationBuilder__navigation_bar(global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Curve curve, global::Doroti.Framework.Animation.Curve reverseCurve, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget> builder)
    {
        this.animation = animation;
        this.curve = curve;
        this.reverseCurve = reverseCurve;
        this.builder = builder;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CurvedAnimationBuilderState__navigation_bar());
}

internal class _CurvedAnimationBuilderState__navigation_bar : global::Doroti.Framework.Widgets.State<_CurvedAnimationBuilder__navigation_bar>
{
    internal virtual global::Doroti.Framework.Animation.AnimationStatus _animationDirection { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.AnimationStatus? _preservedDirection { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _animationDirection = ((_CurvedAnimationBuilder__navigation_bar)this.widget).animation.status;
        _updateStatus(((_CurvedAnimationBuilder__navigation_bar)this.widget).animation.status);
        ((_CurvedAnimationBuilder__navigation_bar)this.widget).animation.addStatusListener((AnimationStatusListener)this._updateStatus);
    }

    public override void dispose()
    {
        ((_CurvedAnimationBuilder__navigation_bar)this.widget).animation.removeStatusListener((AnimationStatusListener)this._updateStatus);
        base.dispose();
    }

    internal virtual void _updateStatus(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        if ((!object.Equals(this._animationDirection, status)))
        {
            setState(((global::System.Action)(() =>
            {
                _animationDirection = status;
            })));
        }
        switch (status)
        {
            case global::Doroti.Framework.Animation.AnimationStatus.forward or global::Doroti.Framework.Animation.AnimationStatus.reverse when ((this._preservedDirection is not null)):
                {
                    break;
                }
            case global::Doroti.Framework.Animation.AnimationStatus.forward or global::Doroti.Framework.Animation.AnimationStatus.reverse:
                {
                    setState(((global::System.Action)(() =>
                    {
                        _preservedDirection = status;
                    })));
                    break;
                }
            case global::Doroti.Framework.Animation.AnimationStatus.completed or global::Doroti.Framework.Animation.AnimationStatus.dismissed:
                {
                    setState(((global::System.Action)(() =>
                    {
                        _preservedDirection = null;
                    })));
                    break;
                }
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        var shouldUseForwardCurve = (!object.Equals(((this._preservedDirection ?? this._animationDirection)), global::Doroti.Framework.Animation.AnimationStatus.reverse));
        global::Doroti.Framework.Animation.Animation<double> curvedAnimation = ((global::Doroti.Framework.Animation.Animation<double>)(object?)new global::Doroti.Framework.Animation.CurveTween(curve: (shouldUseForwardCurve ? ((_CurvedAnimationBuilder__navigation_bar)this.widget).curve : ((_CurvedAnimationBuilder__navigation_bar)this.widget).reverseCurve)).animate(((_CurvedAnimationBuilder__navigation_bar)this.widget).animation));
        return this.widget.builder(context, curvedAnimation);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Navigation_barLibrary
{
    internal static NavigationBarThemeData _defaultsFor(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return (Theme.of(context).useMaterial3 ? new _NavigationBarDefaultsM3__navigation_bar(context) : new _NavigationBarDefaultsM2__navigation_bar(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _NavigationBarDefaultsM2__navigation_bar : NavigationBarThemeData
{
    internal virtual ThemeData _theme { get; private set; } = default!;
    internal virtual ColorScheme _colors { get; private set; } = default!;

    internal _NavigationBarDefaultsM2__navigation_bar(global::Doroti.Framework.Widgets.BuildContext context) : base(height: 80.0, elevation: 0.0, indicatorShape: new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(16))), labelBehavior: NavigationDestinationLabelBehavior.alwaysShow)
    {
        this._theme = Theme.of(context);
        this._colors = Theme.of(context).colorScheme;
    }

    public virtual global::Doroti.Ui.Color? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(ElevationOverlay.colorWithOverlay(this._colors.surface, this._colors.onSurface, 3.0));
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Widgets.IconThemeData?>? iconTheme
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Widgets.IconThemeData?>?)(object?)new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Widgets.IconThemeData>(new global::Doroti.Framework.Widgets.IconThemeData(size: 24, color: this._colors.onSurface)));
            return default!;
        }
    }
    public virtual global::Doroti.Ui.Color? indicatorColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.secondary.withOpacity(0.24));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? labelTextStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.TextStyle?>(this._theme.textTheme.labelSmall!.copyWith(color: this._colors.onSurface)));
    public override global::Doroti.Framework.Painting.EdgeInsetsGeometry? labelPadding => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(top: 4));
}

internal class _NavigationBarDefaultsM3__navigation_bar : NavigationBarThemeData
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

    internal _NavigationBarDefaultsM3__navigation_bar(global::Doroti.Framework.Widgets.BuildContext context) : base(height: 80.0, elevation: 3.0, labelBehavior: NavigationDestinationLabelBehavior.alwaysShow)
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.surfaceContainer);
    public virtual global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public virtual global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
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
    public virtual global::Doroti.Ui.Color? indicatorColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.secondaryContainer);
    public override global::Doroti.Framework.Painting.ShapeBorder? indicatorShape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.ShapeBorder>(new global::Doroti.Framework.Painting.StadiumBorder());
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? labelTextStyle
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>?)(object?)WidgetStateProperty.resolveWith((states) =>
            {
                global::Doroti.Framework.Painting.TextStyle style = this._textTheme.labelMedium!;
                return (style.apply(color: (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled) ? this._colors.onSurfaceVariant.withOpacity(0.38) : (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected) ? this._colors.onSurface : this._colors.onSurfaceVariant))));
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
            return default!;
        }
    }
    public override global::Doroti.Framework.Painting.EdgeInsetsGeometry? labelPadding => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(top: 4));
}
