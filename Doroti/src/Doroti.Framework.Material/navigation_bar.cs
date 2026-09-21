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

public class NavigationBar : StatelessWidget
{
    public virtual Duration? animationDuration { get; private set; }
    public virtual long selectedIndex { get; private set; } = default!;
    public virtual List<Widget> destinations { get; private set; } = default!;
    public virtual Action<long>? onDestinationSelected { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual Color? indicatorColor { get; private set; }
    public virtual ShapeBorder? indicatorShape { get; private set; }
    public virtual double? height { get; private set; }
    public virtual NavigationDestinationLabelBehavior? labelBehavior { get; private set; }
    public virtual WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual WidgetStateProperty<TextStyle?>? labelTextStyle { get; private set; }
    public virtual EdgeInsetsGeometry? labelPadding { get; private set; }
    public virtual bool maintainBottomViewPadding { get; private set; } = default!;

    public NavigationBar(
        Key? key = null,
        Duration? animationDuration = null,
        long selectedIndex = 0,
        List<Widget> destinations = default!,
        Action<long>? onDestinationSelected = null,
        Color? backgroundColor = null,
        double? elevation = null,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        Color? indicatorColor = null,
        ShapeBorder? indicatorShape = null,
        double? height = null,
        NavigationDestinationLabelBehavior? labelBehavior = null,
        WidgetStateProperty<Color?>? overlayColor = null,
        WidgetStateProperty<TextStyle?>? labelTextStyle = null,
        EdgeInsetsGeometry? labelPadding = null,
        bool maintainBottomViewPadding = false
    )
        : base(key: key)
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
        System.Diagnostics.Debug.Assert(
            (0L <= selectedIndex) && (selectedIndex < checked(destinations.Count))
        );
    }

    internal virtual Action _handleTap(long index)
    {
        return (onDestinationSelected is not null)
            ? (
                () =>
                {
                    onDestinationSelected!(index);
                }
            )
            : (() => { });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        NavigationBarThemeData defaults = Navigation_barLibrary._defaultsFor(context);
        NavigationBarThemeData navigationBarTheme = NavigationBarTheme.of(context);
        double effectiveHeight =
            (height ?? navigationBarTheme.height)
            ?? DartRuntimePrimitives.RequireValue(defaults.height);
        NavigationDestinationLabelBehavior effectiveLabelBehavior =
            (labelBehavior ?? navigationBarTheme.labelBehavior)
            ?? DartRuntimePrimitives.RequireValue(defaults.labelBehavior);
        return new Material(
            color: (backgroundColor ?? navigationBarTheme.backgroundColor)
                ?? defaults.backgroundColor!,
            elevation: (elevation ?? navigationBarTheme.elevation)
                ?? DartRuntimePrimitives.RequireValue(defaults.elevation),
            shadowColor: (shadowColor ?? navigationBarTheme.shadowColor) ?? defaults.shadowColor,
            surfaceTintColor: (surfaceTintColor ?? navigationBarTheme.surfaceTintColor)
                ?? defaults.surfaceTintColor,
            child: new SafeArea(
                maintainBottomViewPadding: maintainBottomViewPadding,
                child: new Widgets.Semantics(
                    role: SemanticsRole.tabBar,
                    explicitChildNodes: true,
                    container: true,
                    child: new SizedBox(
                        height: effectiveHeight,
                        child: new Row(
                            children: (
                                (Func<List<Widget>>)(
                                    () =>
                                    {
                                        var __collection12378 = new List<Widget>();
                                        for (long i = 0L; i < checked(destinations.Count); i++)
                                        {
                                            var destinationIndex__g65 = i;
                                            __collection12378.Add(
                                                DartRuntimePrimitives.ConvertValue<Widget>(
                                                    new Expanded(
                                                        child: new MergeSemantics(
                                                            child: new Widgets.Semantics(
                                                                role: SemanticsRole.tab,
                                                                selected: destinationIndex__g65
                                                                    == selectedIndex,
                                                                child: new _SelectableAnimatedBuilder__navigation_bar(
                                                                    duration: animationDuration
                                                                        ?? Duration.Create(
                                                                            milliseconds: 500L
                                                                        ),
                                                                    isSelected: destinationIndex__g65
                                                                        == selectedIndex,
                                                                    builder: (context, animation) =>
                                                                    {
                                                                        return new _NavigationDestinationInfo__navigation_bar(
                                                                            index: destinationIndex__g65,
                                                                            selectedIndex: selectedIndex,
                                                                            totalNumberOfDestinations: checked(
                                                                                destinations.Count
                                                                            ),
                                                                            selectedAnimation: animation,
                                                                            labelBehavior: effectiveLabelBehavior,
                                                                            indicatorColor: indicatorColor,
                                                                            indicatorShape: indicatorShape,
                                                                            overlayColor: overlayColor,
                                                                            onTap: _handleTap(
                                                                                destinationIndex__g65
                                                                            ),
                                                                            labelTextStyle: labelTextStyle,
                                                                            labelPadding: labelPadding,
                                                                            child: destinations[
                                                                                (int)destinationIndex__g65
                                                                            ]
                                                                        );
                                                                        throw new InvalidOperationException(
                                                                            "Dart closure completed without a value."
                                                                        );
                                                                    }
                                                                )
                                                            )
                                                        )
                                                    )
                                                )
                                            );
                                        }
                                        return __collection12378;
                                    }
                                )
                            )()
                        )
                    )
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public enum NavigationDestinationLabelBehavior
{
    alwaysShow,
    alwaysHide,
    onlyShowSelected,
}

public class NavigationDestination : StatelessWidget
{
    public virtual Widget icon { get; private set; } = default!;
    public virtual Widget? selectedIcon { get; private set; }
    public virtual string label { get; private set; } = default!;
    public virtual string? tooltip { get; private set; }
    public virtual bool enabled { get; private set; } = default!;

    public NavigationDestination(
        Key? key = null,
        Widget icon = default!,
        Widget? selectedIcon = null,
        string label = default!,
        string? tooltip = null,
        bool enabled = true
    )
        : base(key: key)
    {
        this.icon = icon;
        this.selectedIcon = selectedIcon;
        this.label = label;
        this.tooltip = tooltip;
        this.enabled = enabled;
    }

    public override Widget build(BuildContext context)
    {
        _NavigationDestinationInfo__navigation_bar info =
            _NavigationDestinationInfo__navigation_bar.of(context);
        var selectedState = new HashSet<WidgetState> { WidgetState.selected };
        var unselectedState = new HashSet<WidgetState>();
        var disabledState = new HashSet<WidgetState> { WidgetState.disabled };
        NavigationBarThemeData navigationBarTheme = NavigationBarTheme.of(context);
        NavigationBarThemeData defaults = Navigation_barLibrary._defaultsFor(context);
        Animation<double> animationLocal = info.selectedAnimation;
        return new _NavigationDestinationBuilder__navigation_bar(
            label: label,
            tooltip: tooltip,
            enabled: enabled,
            buildIcon: (context) =>
            {
                IconThemeData selectedIconTheme =
                    navigationBarTheme.iconTheme?.resolve(selectedState)
                    ?? defaults.iconTheme!.resolve(selectedState)!;
                IconThemeData unselectedIconTheme =
                    navigationBarTheme.iconTheme?.resolve(unselectedState)
                    ?? defaults.iconTheme!.resolve(unselectedState)!;
                IconThemeData disabledIconTheme =
                    navigationBarTheme.iconTheme?.resolve(disabledState)
                    ?? defaults.iconTheme!.resolve(disabledState)!;
                Widget selectedIconWidget = IconTheme.merge(
                    data: enabled ? selectedIconTheme : disabledIconTheme,
                    child: selectedIcon ?? icon
                );
                Widget unselectedIconWidget = IconTheme.merge(
                    data: enabled ? unselectedIconTheme : disabledIconTheme,
                    child: icon
                );
                return new Stack(
                    alignment: Alignment.center,
                    children: new List<Widget>
                    {
                        DartRuntimePrimitives.ConvertValue<Widget>(
                            new NavigationIndicator(
                                animation: animationLocal,
                                color: (info.indicatorColor ?? navigationBarTheme.indicatorColor)
                                    ?? defaults.indicatorColor!,
                                shape: (info.indicatorShape ?? navigationBarTheme.indicatorShape)
                                    ?? defaults.indicatorShape!
                            )
                        ),
                        DartRuntimePrimitives.ConvertValue<Widget>(
                            new _StatusTransitionWidgetBuilder__navigation_bar(
                                animation: animationLocal,
                                builder: (context, child) =>
                                {
                                    return animationLocal.isForwardOrCompleted
                                        ? selectedIconWidget
                                        : unselectedIconWidget;
                                    throw new InvalidOperationException(
                                        "Dart closure completed without a value."
                                    );
                                }
                            )
                        ),
                    }
                );
                throw new InvalidOperationException("Dart closure completed without a value.");
            },
            buildLabel: (context) =>
            {
                TextStyle? effectiveSelectedLabelTextStyle =
                    (
                        info.labelTextStyle?.resolve(selectedState)
                        ?? (navigationBarTheme.labelTextStyle?.resolve(selectedState))
                    ) ?? defaults.labelTextStyle!.resolve(selectedState);
                TextStyle? effectiveUnselectedLabelTextStyle =
                    (
                        info.labelTextStyle?.resolve(unselectedState)
                        ?? (navigationBarTheme.labelTextStyle?.resolve(unselectedState))
                    ) ?? defaults.labelTextStyle!.resolve(unselectedState);
                TextStyle? effectiveDisabledLabelTextStyle =
                    (
                        info.labelTextStyle?.resolve(disabledState)
                        ?? (navigationBarTheme.labelTextStyle?.resolve(disabledState))
                    ) ?? defaults.labelTextStyle!.resolve(disabledState);
                EdgeInsetsGeometry labelPaddingLocal =
                    (info.labelPadding ?? navigationBarTheme.labelPadding)
                    ?? defaults.labelPadding!;
                var textStyle = enabled
                    ? (
                        animationLocal.isForwardOrCompleted
                            ? effectiveSelectedLabelTextStyle
                            : effectiveUnselectedLabelTextStyle
                    )
                    : effectiveDisabledLabelTextStyle;
                return new Padding(
                    padding: labelPaddingLocal,
                    child: MediaQuery.withClampedTextScaling(
                        maxScaleFactor: Navigation_barLibrary._kMaxLabelTextScaleFactor,
                        child: new Text(label, style: textStyle)
                    )
                );
                throw new InvalidOperationException("Dart closure completed without a value.");
            }
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _NavigationDestinationBuilder__navigation_bar : StatefulWidget
{
    public virtual Func<BuildContext, Widget> buildIcon { get; private set; } = default!;
    public virtual Func<BuildContext, Widget> buildLabel { get; private set; } = default!;
    public virtual string label { get; private set; } = default!;
    public virtual string? tooltip { get; private set; }
    public virtual bool enabled { get; private set; } = default!;

    internal _NavigationDestinationBuilder__navigation_bar(
        Func<BuildContext, Widget> buildIcon,
        Func<BuildContext, Widget> buildLabel,
        string label,
        string? tooltip = null,
        bool enabled = true
    )
    {
        this.buildIcon = buildIcon;
        this.buildLabel = buildLabel;
        this.label = label;
        this.tooltip = tooltip;
        this.enabled = enabled;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _NavigationDestinationBuilderState__navigation_bar()
        );
}

internal class _NavigationDestinationBuilderState__navigation_bar
    : State<_NavigationDestinationBuilder__navigation_bar>
{
    public virtual GlobalKey<IState> iconKey { get; private set; } = GlobalKey<IState>.Create();

    public override Widget build(BuildContext context)
    {
        _NavigationDestinationInfo__navigation_bar info =
            _NavigationDestinationInfo__navigation_bar.of(context);
        NavigationBarThemeData navigationBarTheme = NavigationBarTheme.of(context);
        NavigationBarThemeData defaults = Navigation_barLibrary._defaultsFor(context);
        return new _NavigationBarDestinationSemantics__navigation_bar(
            enabled: widget.enabled,
            child: new _NavigationBarDestinationTooltip__navigation_bar(
                message: widget.tooltip ?? widget.label,
                child: new _IndicatorInkWell__navigation_bar(
                    iconKey: iconKey,
                    labelBehavior: info.labelBehavior,
                    customBorder: (info.indicatorShape ?? navigationBarTheme.indicatorShape)
                        ?? defaults.indicatorShape,
                    overlayColor: info.overlayColor ?? navigationBarTheme.overlayColor,
                    onTap: widget.enabled ? info.onTap : null,
                    child: new Row(
                        children: new List<Widget>
                        {
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                new Expanded(
                                    child: new _NavigationBarDestinationLayout__navigation_bar(
                                        icon: widget.buildIcon(context),
                                        iconKey: iconKey,
                                        label: widget.buildLabel(context)
                                    )
                                )
                            ),
                        }
                    )
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _IndicatorInkWell__navigation_bar : InkResponse
{
    public virtual GlobalKey<IState> iconKey { get; private set; } = default!;
    public virtual NavigationDestinationLabelBehavior labelBehavior { get; private set; } =
        default!;

    internal _IndicatorInkWell__navigation_bar(
        GlobalKey<IState> iconKey,
        NavigationDestinationLabelBehavior labelBehavior,
        WidgetStateProperty<Color?>? overlayColor = null,
        ShapeBorder? customBorder = null,
        Action? onTap = null,
        Widget? child = null
    )
        : base(
            overlayColor: overlayColor,
            customBorder: customBorder,
            onTap: onTap,
            child: child,
            containedInkWell: true,
            highlightColor: Colors.transparent
        )
    {
        this.iconKey = iconKey;
        this.labelBehavior = labelBehavior;
    }

    public override Func<Rect>? getRectCallback(RenderBox referenceBox)
    {
        return (Func<Rect>?)
            (object?)(
                () =>
                {
                    var iconBox = ((RenderBox?)iconKey.currentContext!.findRenderObject()!)!;
                    Rect iconRect = iconBox.localToGlobal(Offset.zero) & iconBox.size;
                    return referenceBox.globalToLocal(iconRect.topLeft) & iconBox.size;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }
            );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _NavigationDestinationInfo__navigation_bar : InheritedWidget
{
    public virtual long index { get; private set; } = default!;
    public virtual long selectedIndex { get; private set; } = default!;
    public virtual long totalNumberOfDestinations { get; private set; } = default!;
    public virtual Animation<double> selectedAnimation { get; private set; } = default!;
    public virtual NavigationDestinationLabelBehavior labelBehavior { get; private set; } =
        default!;
    public virtual Color? indicatorColor { get; private set; }
    public virtual ShapeBorder? indicatorShape { get; private set; }
    public virtual WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual Action onTap { get; private set; } = default!;
    public virtual WidgetStateProperty<TextStyle?>? labelTextStyle { get; private set; }
    public virtual EdgeInsetsGeometry? labelPadding { get; private set; }

    internal _NavigationDestinationInfo__navigation_bar(
        long index,
        long selectedIndex,
        long totalNumberOfDestinations,
        Animation<double> selectedAnimation,
        NavigationDestinationLabelBehavior labelBehavior,
        Color? indicatorColor,
        ShapeBorder? indicatorShape,
        WidgetStateProperty<Color?>? overlayColor,
        Action onTap,
        WidgetStateProperty<TextStyle?>? labelTextStyle = null,
        EdgeInsetsGeometry? labelPadding = null,
        Widget child = default!
    )
        : base(child: child)
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

    public static _NavigationDestinationInfo__navigation_bar of(BuildContext context)
    {
        _NavigationDestinationInfo__navigation_bar? result =
            context.dependOnInheritedWidgetOfExactType<_NavigationDestinationInfo__navigation_bar>();
        DartRuntimePrimitives.Assert(
            () => result is not null,
            () =>
                (object?)"Navigation destinations need a _NavigationDestinationInfo parent, "
                + "which is usually provided by NavigationBar."
        );
        return result!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (_NavigationDestinationInfo__navigation_bar)oldWidget;
        return (index != __oldWidget.index)
            || (totalNumberOfDestinations != __oldWidget.totalNumberOfDestinations)
            || (!Equals(selectedAnimation, __oldWidget.selectedAnimation))
            || (!Equals(labelBehavior, __oldWidget.labelBehavior))
            || (!Equals(onTap, __oldWidget.onTap));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class NavigationIndicator : StatelessWidget
{
    public virtual Animation<double> animation { get; private set; } = default!;
    public virtual Color? color { get; private set; }
    public virtual double width { get; private set; } = default!;
    public virtual double height { get; private set; } = default!;
    public virtual BorderRadius borderRadius { get; private set; } = default!;
    public virtual ShapeBorder? shape { get; private set; }

    public NavigationIndicator(
        Key? key = null,
        Animation<double> animation = default!,
        Color? color = null,
        double? width = null,
        double? height = null,
        BorderRadius borderRadius = default!,
        ShapeBorder? shape = null
    )
        : base(key: key)
    {
        double __width = width ?? Navigation_barLibrary._kIndicatorWidth;
        double __height = height ?? Navigation_barLibrary._kIndicatorHeight;
        BorderRadius __borderRadius = borderRadius ?? BorderRadius.CreateAll(Radius.circular(16));
        this.animation = animation;
        this.color = color;
        this.width = __width;
        this.height = __height;
        this.borderRadius = __borderRadius;
        this.shape = shape;
    }

    public override Widget build(BuildContext context)
    {
        return new AnimatedBuilder(
            animation: animation,
            builder: (context, child) =>
            {
                double scale = animation.isDismissed
                    ? 0.0
                    : new Tween<double>(begin: 0.4, end: 1.0).transform(
                        new CurveTween(curve: Curves.easeInOutCubicEmphasized).transform(
                            animation.value
                        )
                    );
                return new Transform(
                    alignment: Alignment.center,
                    transform: Matrix4.diagonal3Values(scale, 1.0, 1.0),
                    child: child
                );
                throw new InvalidOperationException("Dart closure completed without a value.");
            },
            child: new _StatusTransitionWidgetBuilder__navigation_bar(
                animation: animation,
                builder: (context, child) =>
                {
                    return new _SelectableAnimatedBuilder__navigation_bar(
                        isSelected: animation.isForwardOrCompleted,
                        duration: Duration.Create(milliseconds: 100L),
                        alwaysDoFullAnimation: true,
                        builder: (context, fadeAnimation) =>
                        {
                            return new FadeTransition(
                                opacity: fadeAnimation,
                                child: new Ink(
                                    width: DartRuntimePrimitives.RequireValue(width),
                                    height: DartRuntimePrimitives.RequireValue(height),
                                    decoration: new ShapeDecoration(
                                        shape: shape
                                            ?? new RoundedRectangleBorder(
                                                borderRadius: borderRadius
                                            ),
                                        color: color ?? Theme.of(context).colorScheme.secondary
                                    )
                                )
                            );
                            throw new InvalidOperationException(
                                "Dart closure completed without a value."
                            );
                        }
                    );
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _NavigationBarDestinationLayout__navigation_bar : StatelessWidget
{
    public virtual Widget icon { get; private set; } = default!;
    public virtual GlobalKey<IState> iconKey { get; private set; } = default!;
    public virtual Widget label { get; private set; } = default!;

    internal _NavigationBarDestinationLayout__navigation_bar(
        Widget icon,
        GlobalKey<IState> iconKey,
        Widget label
    )
    {
        this.icon = icon;
        this.iconKey = iconKey;
        this.label = label;
    }

    public override Widget build(BuildContext context)
    {
        return new _DestinationLayoutAnimationBuilder__navigation_bar(
            builder: (context, animation) =>
            {
                return new CustomMultiChildLayout(
                    @delegate: new _NavigationDestinationLayoutDelegate__navigation_bar(
                        animation: animation
                    ),
                    children: new List<Widget>
                    {
                        DartRuntimePrimitives.ConvertValue<Widget>(
                            new LayoutId(
                                id: _NavigationDestinationLayoutDelegate__navigation_bar.iconId,
                                child: new KeyedSubtree(key: iconKey, child: icon)
                            )
                        ),
                        DartRuntimePrimitives.ConvertValue<Widget>(
                            new LayoutId(
                                id: _NavigationDestinationLayoutDelegate__navigation_bar.labelId,
                                child: new FadeTransition(
                                    alwaysIncludeSemantics: true,
                                    opacity: animation,
                                    child: label
                                )
                            )
                        ),
                    }
                );
                throw new InvalidOperationException("Dart closure completed without a value.");
            }
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _DestinationLayoutAnimationBuilder__navigation_bar : StatelessWidget
{
    public virtual Func<BuildContext, Animation<double>, Widget> builder { get; private set; } =
        default!;

    internal _DestinationLayoutAnimationBuilder__navigation_bar(
        Func<BuildContext, Animation<double>, Widget> builder
    )
    {
        this.builder = builder;
    }

    public override Widget build(BuildContext context)
    {
        _NavigationDestinationInfo__navigation_bar info =
            _NavigationDestinationInfo__navigation_bar.of(context);
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
                return new _CurvedAnimationBuilder__navigation_bar(
                    animation: info.selectedAnimation,
                    curve: Curves.easeInOutCubicEmphasized,
                    reverseCurve: Curves.easeInOutCubicEmphasized.flipped,
                    builder: builder
                );
            }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _NavigationBarDestinationSemantics__navigation_bar : StatelessWidget
{
    public virtual bool enabled { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    internal _NavigationBarDestinationSemantics__navigation_bar(bool enabled, Widget child)
    {
        this.enabled = enabled;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        _NavigationDestinationInfo__navigation_bar destinationInfo =
            _NavigationDestinationInfo__navigation_bar.of(context);
        return new _StatusTransitionWidgetBuilder__navigation_bar(
            animation: destinationInfo.selectedAnimation,
            builder: (context, child) =>
            {
                return new Widgets.Semantics(enabled: enabled, button: true, child: child);
                throw new InvalidOperationException("Dart closure completed without a value.");
            },
            child: Foundation.ConstantsLibrary.kIsWeb
                ? child
                : new Stack(
                    alignment: Alignment.center,
                    children: new List<Widget>
                    {
                        DartRuntimePrimitives.ConvertValue<Widget>(child),
                        DartRuntimePrimitives.ConvertValue<Widget>(
                            new Widgets.Semantics(
                                label: localizations.tabLabel(
                                    tabIndex: destinationInfo.index + 1L,
                                    tabCount: destinationInfo.totalNumberOfDestinations
                                )
                            )
                        ),
                    }
                )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _NavigationBarDestinationTooltip__navigation_bar : StatelessWidget
{
    public virtual string message { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    internal _NavigationBarDestinationTooltip__navigation_bar(string message, Widget child)
    {
        this.message = message;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        return new Tooltip(
            message: message,
            verticalOffset: 42,
            excludeFromSemantics: true,
            preferBelow: false,
            child: child
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _NavigationDestinationLayoutDelegate__navigation_bar : MultiChildLayoutDelegate
{
    public virtual Animation<double> animation { get; private set; } = default!;
    public const long iconId = 1L;
    public const long labelId = 2L;

    internal _NavigationDestinationLayoutDelegate__navigation_bar(Animation<double> animation)
        : base(relayout: animation)
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
        Size iconSize = layoutChild(iconId, BoxConstraints.CreateLoose(size));
        Size labelSize = layoutChild(labelId, BoxConstraints.CreateLoose(size));
        double yPositionOffset = new Tween<double>(
            begin: halfHeight(iconSize),
            end: halfHeight(iconSize) + halfHeight(labelSize)
        ).transform(animation.value);
        double iconYPosition = halfHeight(size) - yPositionOffset;
        positionChild(iconId, new Offset(halfWidth(size) - halfWidth(iconSize), iconYPosition));
        positionChild(
            labelId,
            new Offset(halfWidth(size) - halfWidth(labelSize), iconYPosition + iconSize.height)
        );
    }

    public override bool shouldRelayout(MultiChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (_NavigationDestinationLayoutDelegate__navigation_bar)oldDelegate;
        return !Equals(__oldDelegate.animation, animation);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _StatusTransitionWidgetBuilder__navigation_bar : StatusTransitionWidget
{
    public virtual Func<BuildContext, Widget?, Widget> builder { get; private set; } = default!;
    public virtual Widget? child { get; private set; }

    internal _StatusTransitionWidgetBuilder__navigation_bar(
        Animation<double> animation,
        Func<BuildContext, Widget?, Widget> builder,
        Widget? child = null
    )
        : base(animation: animation)
    {
        this.builder = builder;
        this.child = child;
    }

    public override Widget build(BuildContext context) => builder(context, child);
}

public class _SelectableAnimatedBuilder__navigation_bar : StatefulWidget
{
    public virtual bool isSelected { get; private set; } = default!;
    public virtual Duration duration { get; private set; } = default!;
    public virtual bool alwaysDoFullAnimation { get; private set; } = default!;
    public virtual Func<BuildContext, Animation<double>, Widget> builder { get; private set; } =
        default!;

    internal _SelectableAnimatedBuilder__navigation_bar(
        bool isSelected,
        Duration? duration = null,
        bool alwaysDoFullAnimation = false,
        Func<BuildContext, Animation<double>, Widget> builder = default!
    )
    {
        Duration __duration = duration ?? Duration.Create(milliseconds: 200);
        this.isSelected = isSelected;
        this.duration = __duration;
        this.alwaysDoFullAnimation = alwaysDoFullAnimation;
        this.builder = builder;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _SelectableAnimatedBuilderState__navigation_bar()
        );
}

public class _SelectableAnimatedBuilderState__navigation_bar
    : State<_SelectableAnimatedBuilder__navigation_bar>,
        SingleTickerProviderStateMixin<_SelectableAnimatedBuilder__navigation_bar>
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
            throw DartRuntimePrimitives.AsException(
                new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary($"{this} was disposed with an active Ticker."),
                        new ErrorDescription(
                            $"{GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time "
                                + "dispose() was called on the mixin, that Ticker was still active. The Ticker must "
                                + "be disposed before calling super.dispose()."
                        ),
                        new ErrorHint(
                            "Tickers used by AnimationControllers "
                                + "should be disposed by calling dispose() on the AnimationController itself. "
                                + "Otherwise, the ticker will leak."
                        ),
                        _ticker!.describeForError("The offending ticker was"),
                    }
                )
            );
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
            throw DartRuntimePrimitives.AsException(
                new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary(
                            $"{GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."
                        ),
                        new ErrorDescription(
                            "A SingleTickerProviderStateMixin can only be used as a TickerProvider once."
                        ),
                        new ErrorHint(
                            "If a State is used for multiple AnimationController objects, or if it is passed to other "
                                + "objects and those objects might use it more than one time in total, then instead of "
                                + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin."
                        ),
                    }
                )
            );
        });
        _ticker = new Scheduler.Ticker(
            onTick,
            debugLabel: Foundation.ConstantsLibrary.kDebugMode
                ? $"created by {DiagnosticsLibrary.describeIdentity(this)}"
                : null
        );
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
        string? tickerDescription = (_ticker?.isActive, _ticker?.muted) switch
        {
            (true, true) => "active but muted",
            (true, _) => "active",
            (false, true) => "inactive and muted",
            (false, _) => "inactive",
            (null, _) => DartRuntimePrimitives.ConvertValue<string>(null),
        };
        properties.add(
            new DiagnosticsProperty<Scheduler.Ticker>(
                "ticker",
                _ticker,
                description: tickerDescription,
                showSeparator: false,
                defaultValue: default
            )
        );
    }
}

internal class _CurvedAnimationBuilder__navigation_bar : StatefulWidget
{
    public virtual Animation<double> animation { get; private set; } = default!;
    public virtual Curve curve { get; private set; } = default!;
    public virtual Curve reverseCurve { get; private set; } = default!;
    public virtual Func<BuildContext, Animation<double>, Widget> builder { get; private set; } =
        default!;

    internal _CurvedAnimationBuilder__navigation_bar(
        Animation<double> animation,
        Curve curve,
        Curve reverseCurve,
        Func<BuildContext, Animation<double>, Widget> builder
    )
    {
        this.animation = animation;
        this.curve = curve;
        this.reverseCurve = reverseCurve;
        this.builder = builder;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _CurvedAnimationBuilderState__navigation_bar()
        );
}

internal class _CurvedAnimationBuilderState__navigation_bar
    : State<_CurvedAnimationBuilder__navigation_bar>
{
    internal virtual AnimationStatus _animationDirection { get; set; } = default!;
    internal virtual AnimationStatus? _preservedDirection { get; set; } = default;

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

    internal virtual void _updateStatus(AnimationStatus status)
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
            case AnimationStatus.forward
            or AnimationStatus.reverse when _preservedDirection is not null:
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

    public override Widget build(BuildContext context)
    {
        var shouldUseForwardCurve = !Equals(
            _preservedDirection ?? _animationDirection,
            AnimationStatus.reverse
        );
        Animation<double> curvedAnimation = new CurveTween(
            curve: shouldUseForwardCurve ? widget.curve : widget.reverseCurve
        ).animate(widget.animation);
        return widget.builder(context, curvedAnimation);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Navigation_barLibrary
{
    internal static NavigationBarThemeData _defaultsFor(BuildContext context)
    {
        return new _NavigationBarDefaultsM3__navigation_bar(context);
    }
}

internal class _NavigationBarDefaultsM3__navigation_bar : NavigationBarThemeData
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

    internal _NavigationBarDefaultsM3__navigation_bar(BuildContext context)
        : base(
            height: 80.0,
            elevation: 3.0,
            labelBehavior: NavigationDestinationLabelBehavior.alwaysShow
        )
    {
        this.context = context;
    }

    public override Color? backgroundColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.surfaceContainer);
    public override Color? shadowColor =>
        DartRuntimePrimitives.ConvertValue<Color>(Colors.transparent);
    public override Color? surfaceTintColor =>
        DartRuntimePrimitives.ConvertValue<Color>(Colors.transparent);
    public override WidgetStateProperty<IconThemeData?>? iconTheme
    {
        get
        {
            return (WidgetStateProperty<IconThemeData?>?)
                WidgetStateProperty.resolveWith(
                    (states) =>
                    {
                        return new IconThemeData(
                            size: 24.0,
                            color: states.Contains(WidgetState.disabled)
                                ? _colors.onSurfaceVariant.withOpacity(0.38)
                                : (
                                    states.Contains(WidgetState.selected)
                                        ? _colors.onSecondaryContainer
                                        : _colors.onSurfaceVariant
                                )
                        );
                        throw new InvalidOperationException(
                            "Dart closure completed without a value."
                        );
                    }
                );
        }
    }
    public override Color? indicatorColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.secondaryContainer);
    public override ShapeBorder? indicatorShape =>
        DartRuntimePrimitives.ConvertValue<ShapeBorder>(new StadiumBorder());
    public override WidgetStateProperty<TextStyle?>? labelTextStyle
    {
        get
        {
            return (WidgetStateProperty<TextStyle?>?)
                WidgetStateProperty.resolveWith(
                    (states) =>
                    {
                        TextStyle style = _textTheme.labelMedium!;
                        return style.apply(
                            color: states.Contains(WidgetState.disabled)
                                ? _colors.onSurfaceVariant.withOpacity(0.38)
                                : (
                                    states.Contains(WidgetState.selected)
                                        ? _colors.onSurface
                                        : _colors.onSurfaceVariant
                                )
                        );
                        throw new InvalidOperationException(
                            "Dart closure completed without a value."
                        );
                    }
                );
        }
    }
    public override EdgeInsetsGeometry? labelPadding =>
        DartRuntimePrimitives.ConvertValue<EdgeInsetsGeometry>(EdgeInsets.CreateOnly(top: 4));
}
