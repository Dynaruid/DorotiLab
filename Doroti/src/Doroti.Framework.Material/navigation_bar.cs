// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/navigation_bar.dart

using Doroti.Runtime;
using Doroti.Ui;

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
        System.Diagnostics.Debug.Assert(checked(destinations.Count) >= 2L);
        System.Diagnostics.Debug.Assert((0L <= selectedIndex) && (selectedIndex < checked(destinations.Count)));
    }

    internal virtual global::System.Action _handleTap(long index)
    {
        return (onDestinationSelected is not null) ? (() => { onDestinationSelected!(index); }) : (() =>
        {
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        NavigationBarThemeData defaults = Navigation_barLibrary._defaultsFor(context);
        NavigationBarThemeData navigationBarTheme = NavigationBarTheme.of(context);
        double effectiveHeight = (height ?? navigationBarTheme.height) ?? DartRuntimePrimitives.RequireValue(defaults.height);
        NavigationDestinationLabelBehavior effectiveLabelBehavior = (labelBehavior ?? navigationBarTheme.labelBehavior) ?? DartRuntimePrimitives.RequireValue(defaults.labelBehavior);
        return new Material(color: (backgroundColor ?? navigationBarTheme.backgroundColor) ?? defaults.backgroundColor!, elevation: (elevation ?? navigationBarTheme.elevation) ?? DartRuntimePrimitives.RequireValue(defaults.elevation), shadowColor: (shadowColor ?? navigationBarTheme.shadowColor) ?? defaults.shadowColor, surfaceTintColor: (surfaceTintColor ?? navigationBarTheme.surfaceTintColor) ?? defaults.surfaceTintColor, child: new global::Doroti.Framework.Widgets.SafeArea(maintainBottomViewPadding: maintainBottomViewPadding, child: new global::Doroti.Framework.Widgets.Semantics(role: SemanticsRole.tabBar, explicitChildNodes: true, container: true, child: new global::Doroti.Framework.Widgets.SizedBox(height: effectiveHeight, child: new global::Doroti.Framework.Widgets.Row(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() =>
        {
            var __collection12378 = new List<global::Doroti.Framework.Widgets.Widget>(); for (long i = 0L; i < checked(destinations.Count); i++)
            {
                var destinationIndex__g65 = i; __collection12378.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.MergeSemantics(child: new global::Doroti.Framework.Widgets.Semantics(role: SemanticsRole.tab, selected: destinationIndex__g65 == selectedIndex, child: new _SelectableAnimatedBuilder__navigation_bar(duration: animationDuration ?? Duration.Create(milliseconds: 500L), isSelected: destinationIndex__g65 == selectedIndex, builder: (context, animation) =>
                {
                    return new _NavigationDestinationInfo__navigation_bar(index: destinationIndex__g65, selectedIndex: selectedIndex, totalNumberOfDestinations: checked(destinations.Count), selectedAnimation: animation, labelBehavior: effectiveLabelBehavior, indicatorColor: indicatorColor, indicatorShape: indicatorShape, overlayColor: overlayColor, onTap: _handleTap(destinationIndex__g65), labelTextStyle: labelTextStyle, labelPadding: labelPadding, child: destinations[(int)destinationIndex__g65]);
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }))))));
            }
            return __collection12378;
        }))())))));
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
        _NavigationDestinationInfo__navigation_bar info = _NavigationDestinationInfo__navigation_bar.of(context);
        var selectedState = new HashSet<global::Doroti.Framework.Widgets.WidgetState> { WidgetState.selected };
        var unselectedState = new HashSet<global::Doroti.Framework.Widgets.WidgetState>();
        var disabledState = new HashSet<global::Doroti.Framework.Widgets.WidgetState> { WidgetState.disabled };
        NavigationBarThemeData navigationBarTheme = NavigationBarTheme.of(context);
        NavigationBarThemeData defaults = Navigation_barLibrary._defaultsFor(context);
        global::Doroti.Framework.Animation.Animation<double> animationLocal = info.selectedAnimation;
        return new _NavigationDestinationBuilder__navigation_bar(label: label, tooltip: tooltip, enabled: enabled, buildIcon: (context) =>
        {
            global::Doroti.Framework.Widgets.IconThemeData selectedIconTheme = navigationBarTheme.iconTheme?.resolve(selectedState) ?? defaults.iconTheme!.resolve(selectedState)!;
            global::Doroti.Framework.Widgets.IconThemeData unselectedIconTheme = navigationBarTheme.iconTheme?.resolve(unselectedState) ?? defaults.iconTheme!.resolve(unselectedState)!;
            global::Doroti.Framework.Widgets.IconThemeData disabledIconTheme = navigationBarTheme.iconTheme?.resolve(disabledState) ?? defaults.iconTheme!.resolve(disabledState)!;
            global::Doroti.Framework.Widgets.Widget selectedIconWidget = IconTheme.merge(data: enabled ? selectedIconTheme : disabledIconTheme, child: selectedIcon ?? icon);
            global::Doroti.Framework.Widgets.Widget unselectedIconWidget = IconTheme.merge(data: enabled ? unselectedIconTheme : disabledIconTheme, child: icon);
            return new global::Doroti.Framework.Widgets.Stack(alignment: Alignment.center, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new NavigationIndicator(animation: animationLocal, color: (info.indicatorColor ?? navigationBarTheme.indicatorColor) ?? defaults.indicatorColor!, shape: (info.indicatorShape ?? navigationBarTheme.indicatorShape) ?? defaults.indicatorShape!)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _StatusTransitionWidgetBuilder__navigation_bar(animation: animationLocal, builder: (context, child) => {
return animationLocal.isForwardOrCompleted ? selectedIconWidget : unselectedIconWidget;
throw new InvalidOperationException("Dart closure completed without a value.");
})) });
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, buildLabel: (context) =>
        {
            global::Doroti.Framework.Painting.TextStyle? effectiveSelectedLabelTextStyle = (info.labelTextStyle?.resolve(selectedState) ?? (navigationBarTheme.labelTextStyle?.resolve(selectedState))) ?? defaults.labelTextStyle!.resolve(selectedState);
            global::Doroti.Framework.Painting.TextStyle? effectiveUnselectedLabelTextStyle = (info.labelTextStyle?.resolve(unselectedState) ?? (navigationBarTheme.labelTextStyle?.resolve(unselectedState))) ?? defaults.labelTextStyle!.resolve(unselectedState);
            global::Doroti.Framework.Painting.TextStyle? effectiveDisabledLabelTextStyle = (info.labelTextStyle?.resolve(disabledState) ?? (navigationBarTheme.labelTextStyle?.resolve(disabledState))) ?? defaults.labelTextStyle!.resolve(disabledState);
            global::Doroti.Framework.Painting.EdgeInsetsGeometry labelPaddingLocal = (info.labelPadding ?? navigationBarTheme.labelPadding) ?? defaults.labelPadding!;
            var textStyle = enabled ? (animationLocal.isForwardOrCompleted ? effectiveSelectedLabelTextStyle : effectiveUnselectedLabelTextStyle) : effectiveDisabledLabelTextStyle;
            return new global::Doroti.Framework.Widgets.Padding(padding: labelPaddingLocal, child: MediaQuery.withClampedTextScaling(maxScaleFactor: Navigation_barLibrary._kMaxLabelTextScaleFactor, child: new global::Doroti.Framework.Widgets.Text(label, style: textStyle)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
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
    public virtual global::Doroti.Framework.Widgets.GlobalKey<IState> iconKey { get; private set; } = GlobalKey<IState>.Create();

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        _NavigationDestinationInfo__navigation_bar info = _NavigationDestinationInfo__navigation_bar.of(context);
        NavigationBarThemeData navigationBarTheme = NavigationBarTheme.of(context);
        NavigationBarThemeData defaults = Navigation_barLibrary._defaultsFor(context);
        return new _NavigationBarDestinationSemantics__navigation_bar(enabled: widget.enabled, child: new _NavigationBarDestinationTooltip__navigation_bar(message: widget.tooltip ?? widget.label, child: new _IndicatorInkWell__navigation_bar(iconKey: iconKey, labelBehavior: info.labelBehavior, customBorder: (info.indicatorShape ?? navigationBarTheme.indicatorShape) ?? defaults.indicatorShape, overlayColor: info.overlayColor ?? navigationBarTheme.overlayColor, onTap: widget.enabled ? info.onTap : null, child: new global::Doroti.Framework.Widgets.Row(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new _NavigationBarDestinationLayout__navigation_bar(icon: widget.buildIcon(context), iconKey: iconKey, label: widget.buildLabel(context)))) }))));
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

    public override global::System.Func<Rect>? getRectCallback(global::Doroti.Framework.Rendering.RenderBox referenceBox)
    {
        return (global::System.Func<Rect>?)(object?)(() =>
        {
            var iconBox = ((global::Doroti.Framework.Rendering.RenderBox?)iconKey.currentContext!.findRenderObject()!)!;
            global::Doroti.Ui.Rect iconRect = iconBox.localToGlobal(Offset.zero) & iconBox.size;
            return referenceBox.globalToLocal(iconRect.topLeft) & iconBox.size;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
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
        _NavigationDestinationInfo__navigation_bar? result = context.dependOnInheritedWidgetOfExactType<_NavigationDestinationInfo__navigation_bar>();
        DartRuntimePrimitives.Assert(() => result is not null, () => (object?)"Navigation destinations need a _NavigationDestinationInfo parent, " + "which is usually provided by NavigationBar.");
        return result!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget)
    {
        var __oldWidget = (_NavigationDestinationInfo__navigation_bar)oldWidget;
        return (index != __oldWidget.index) || (totalNumberOfDestinations != __oldWidget.totalNumberOfDestinations) || (!Equals(selectedAnimation, __oldWidget.selectedAnimation)) || (!Equals(labelBehavior, __oldWidget.labelBehavior)) || (!Equals(onTap, __oldWidget.onTap));
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
        global::Doroti.Framework.Painting.BorderRadius __borderRadius = borderRadius ?? BorderRadius.CreateAll(Radius.circular(16));
        this.animation = animation;
        this.color = color;
        this.width = __width;
        this.height = __height;
        this.borderRadius = __borderRadius;
        this.shape = shape;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: animation, builder: (context, child) =>
        {
            double scale = animation.isDismissed ? 0.0 : new global::Doroti.Framework.Animation.Tween<double>(begin: 0.4, end: 1.0).transform(new global::Doroti.Framework.Animation.CurveTween(curve: Curves.easeInOutCubicEmphasized).transform(animation.value));
            return new global::Doroti.Framework.Widgets.Transform(alignment: Alignment.center, transform: Matrix4.diagonal3Values(scale, 1.0, 1.0), child: child);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, child: new _StatusTransitionWidgetBuilder__navigation_bar(animation: animation, builder: (context, child) =>
        {
            return new _SelectableAnimatedBuilder__navigation_bar(isSelected: animation.isForwardOrCompleted, duration: Duration.Create(milliseconds: 100L), alwaysDoFullAnimation: true, builder: (context, fadeAnimation) =>
            {
                return new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeAnimation, child: new Ink(width: DartRuntimePrimitives.RequireValue(width), height: DartRuntimePrimitives.RequireValue(height), decoration: new global::Doroti.Framework.Painting.ShapeDecoration(shape: shape ?? new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: borderRadius), color: color ?? Theme.of(context).colorScheme.secondary)));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
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
        return new _DestinationLayoutAnimationBuilder__navigation_bar(builder: (context, animation) =>
        {
            return new global::Doroti.Framework.Widgets.CustomMultiChildLayout(@delegate: new _NavigationDestinationLayoutDelegate__navigation_bar(animation: animation), children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.LayoutId(id: _NavigationDestinationLayoutDelegate__navigation_bar.iconId, child: new global::Doroti.Framework.Widgets.KeyedSubtree(key: iconKey, child: icon))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.LayoutId(id: _NavigationDestinationLayoutDelegate__navigation_bar.labelId, child: new global::Doroti.Framework.Widgets.FadeTransition(alwaysIncludeSemantics: true, opacity: animation, child: label))) });
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
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
        _NavigationDestinationInfo__navigation_bar info = _NavigationDestinationInfo__navigation_bar.of(context);
        switch (info.labelBehavior)
        {
            case NavigationDestinationLabelBehavior.alwaysShow:
                {
                    return builder(context, AnimationsLibrary.kAlwaysCompleteAnimation);
                }
            case NavigationDestinationLabelBehavior.alwaysHide:
                {
                    return builder(context, AnimationsLibrary.kAlwaysDismissedAnimation);
                }
            case NavigationDestinationLabelBehavior.onlyShowSelected:
                {
                    return new _CurvedAnimationBuilder__navigation_bar(animation: info.selectedAnimation, curve: Curves.easeInOutCubicEmphasized, reverseCurve: Curves.easeInOutCubicEmphasized.flipped, builder: builder);
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
        _NavigationDestinationInfo__navigation_bar destinationInfo = _NavigationDestinationInfo__navigation_bar.of(context);
        return new _StatusTransitionWidgetBuilder__navigation_bar(animation: destinationInfo.selectedAnimation, builder: (context, child) =>
        {
            return new global::Doroti.Framework.Widgets.Semantics(enabled: enabled, button: true, child: child);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, child: Foundation.ConstantsLibrary.kIsWeb ? child : new global::Doroti.Framework.Widgets.Stack(alignment: Alignment.center, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(child), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(label: localizations.tabLabel(tabIndex: destinationInfo.index + 1L, tabCount: destinationInfo.totalNumberOfDestinations))) }));
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
        return new Tooltip(message: message, verticalOffset: 42, excludeFromSemantics: true, preferBelow: false, child: child);
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
            return size.width / 2L;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        double halfHeight(Size size)
        {
            return size.height / 2L;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        global::Doroti.Ui.Size iconSize = layoutChild(iconId, BoxConstraints.CreateLoose(size));
        global::Doroti.Ui.Size labelSize = layoutChild(labelId, BoxConstraints.CreateLoose(size));
        double yPositionOffset = new global::Doroti.Framework.Animation.Tween<double>(begin: halfHeight(iconSize), end: halfHeight(iconSize) + halfHeight(labelSize)).transform(animation.value);
        double iconYPosition = halfHeight(size) - yPositionOffset;
        positionChild(iconId, new global::Doroti.Ui.Offset(halfWidth(size) - halfWidth(iconSize), iconYPosition));
        positionChild(labelId, new global::Doroti.Ui.Offset(halfWidth(size) - halfWidth(labelSize), iconYPosition + iconSize.height));
    }

    public override bool shouldRelayout(global::Doroti.Framework.Rendering.MultiChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (_NavigationDestinationLayoutDelegate__navigation_bar)oldDelegate;
        return !Equals(__oldDelegate.animation, animation);
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

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context) => builder(context, child);
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
        _controller.duration = widget.duration;
        _controller.value = widget.isSelected ? 1.0 : 0.0;
    }

    public override void didUpdateWidget(_SelectableAnimatedBuilder__navigation_bar oldWidget)
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
                _controller.forward(from: widget.alwaysDoFullAnimation ? 0 : null);
            }
            else
            {
                _controller.reverse(from: widget.alwaysDoFullAnimation ? 1 : null);
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
        _animationDirection = widget.animation.status;
        _updateStatus(widget.animation.status);
        widget.animation.addStatusListener(_updateStatus);
    }

    public override void dispose()
    {
        widget.animation.removeStatusListener(_updateStatus);
        base.dispose();
    }

    internal virtual void _updateStatus(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        if (!Equals(_animationDirection, status))
        {
            setState(() =>
            {
                _animationDirection = status;
            });
        }
        switch (status)
        {
            case AnimationStatus.forward or AnimationStatus.reverse when _preservedDirection is not null:
                {
                    break;
                }
            case AnimationStatus.forward or AnimationStatus.reverse:
                {
                    setState(() =>
                    {
                        _preservedDirection = status;
                    });
                    break;
                }
            case AnimationStatus.completed or AnimationStatus.dismissed:
                {
                    setState(() =>
                    {
                        _preservedDirection = null;
                    });
                    break;
                }
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        var shouldUseForwardCurve = !Equals(_preservedDirection ?? _animationDirection, AnimationStatus.reverse);
        global::Doroti.Framework.Animation.Animation<double> curvedAnimation = new global::Doroti.Framework.Animation.CurveTween(curve: shouldUseForwardCurve ? widget.curve : widget.reverseCurve).animate(widget.animation);
        return widget.builder(context, curvedAnimation);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Navigation_barLibrary
{
    internal static NavigationBarThemeData _defaultsFor(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new _NavigationBarDefaultsM3__navigation_bar(context);
    }
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

    internal _NavigationBarDefaultsM3__navigation_bar(global::Doroti.Framework.Widgets.BuildContext context) : base(height: 80.0, elevation: 3.0, labelBehavior: NavigationDestinationLabelBehavior.alwaysShow)
    {
        this.context = context;
    }

    public override global::Doroti.Ui.Color? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.surfaceContainer);
    public override global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public override global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
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
    public override global::Doroti.Ui.Color? indicatorColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.secondaryContainer);
    public override global::Doroti.Framework.Painting.ShapeBorder? indicatorShape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.ShapeBorder>(new global::Doroti.Framework.Painting.StadiumBorder());
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? labelTextStyle
    {
        get
        {
            return (global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>?)WidgetStateProperty.resolveWith((states) =>
            {
                global::Doroti.Framework.Painting.TextStyle style = _textTheme.labelMedium!;
                return style.apply(color: states.Contains(WidgetState.disabled) ? _colors.onSurfaceVariant.withOpacity(0.38) : (states.Contains(WidgetState.selected) ? _colors.onSurface : _colors.onSurfaceVariant));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    public override global::Doroti.Framework.Painting.EdgeInsetsGeometry? labelPadding => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(EdgeInsets.CreateOnly(top: 4));
}
