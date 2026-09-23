// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/tabs.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class TabsLibrary
{
    internal static double _kTabHeight = 46.0;
}

public static partial class TabsLibrary
{
    internal static double _kTextAndIconTabHeight = 72.0;
}

public static partial class TabsLibrary
{
    internal static double _kStartOffset = 52.0;
}

public enum TabBarIndicatorSize
{
    tab,
    label,
}

public enum TabAlignment
{
    start,
    startOffset,
    fill,
    center,
}

public enum TabIndicatorAnimation
{
    linear,
    elastic,
}

public class Tab : StatelessWidget, PreferredSizeWidget
{
    public virtual string? text { get; private set; }
    public virtual Widget? child { get; private set; }
    public virtual Widget? icon { get; private set; }
    public virtual EdgeInsetsGeometry? iconMargin { get; private set; }
    public virtual double? height { get; private set; }

    public Tab(
        Key? key = null,
        string? text = null,
        Widget? icon = null,
        EdgeInsetsGeometry? iconMargin = null,
        double? height = null,
        Widget? child = null
    )
        : base(key: key)
    {
        this.text = text;
        this.icon = icon;
        this.iconMargin = iconMargin;
        this.height = height;
        this.child = child;
        System.Diagnostics.Debug.Assert(
            (text is not null) || (child is not null) || (icon is not null)
        );
        System.Diagnostics.Debug.Assert((text is null) || (child is null));
    }

    internal virtual Widget _buildLabelText()
    {
        return child ?? new Text(text!, softWrap: false, overflow: TextOverflow.fade);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        double calculatedHeight = default!;
        Widget label = default!;
        if (icon is null)
        {
            calculatedHeight = TabsLibrary._kTabHeight;
            label = _buildLabelText();
        }
        else
        {
            if ((text is null) && (child is null))
            {
                calculatedHeight = TabsLibrary._kTabHeight;
                label = icon!;
            }
            else
            {
                calculatedHeight = TabsLibrary._kTextAndIconTabHeight;
                EdgeInsetsGeometry effectiveIconMargin =
                    iconMargin ?? _TabsPrimaryDefaultsM3__tabs.iconMargin;
                label = DartRuntimePrimitives.ConvertValue<Widget>(
                    new Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: new List<Widget>
                        {
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                new Padding(padding: effectiveIconMargin, child: icon)
                            ),
                            DartRuntimePrimitives.ConvertValue<Widget>(_buildLabelText()),
                        }
                    )
                );
            }
        }
        return new SizedBox(
            height: height ?? calculatedHeight,
            child: new Center(widthFactor: 1.0, child: label)
        );
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new StringProperty("text", text, defaultValue: null));
    }

    public virtual Size preferredSize
    {
        get
        {
            if (height is not null)
            {
                double height__value7504 = (
                    height
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
                return new Size(
                    (
                        height
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                );
            }
            else
            {
                if (((text is not null) || (child is not null)) && (icon is not null))
                {
                    return new Size(TabsLibrary._kTextAndIconTabHeight);
                }
                else
                {
                    return new Size(TabsLibrary._kTabHeight);
                }
            }
        }
    }
}

internal class _TabStyle__tabs : AnimatedWidget
{
    public virtual TextStyle? labelStyle { get; private set; }
    public virtual TextStyle? unselectedLabelStyle { get; private set; }
    public virtual bool isSelected { get; private set; } = default!;
    public virtual bool isPrimary { get; private set; } = default!;
    public virtual Color? labelColor { get; private set; }
    public virtual Color? unselectedLabelColor { get; private set; }
    public virtual TabBarThemeData defaults { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    internal _TabStyle__tabs(
        Animation<double> animation,
        bool isSelected,
        bool isPrimary,
        Color? labelColor,
        Color? unselectedLabelColor,
        TextStyle? labelStyle,
        TextStyle? unselectedLabelStyle,
        TabBarThemeData defaults,
        Widget child
    )
        : base(listenable: animation)
    {
        this.isSelected = isSelected;
        this.isPrimary = isPrimary;
        this.labelColor = labelColor;
        this.unselectedLabelColor = unselectedLabelColor;
        this.labelStyle = labelStyle;
        this.unselectedLabelStyle = unselectedLabelStyle;
        this.defaults = defaults;
        this.child = child;
    }

    internal virtual WidgetStateColor _resolveWithLabelColor(
        BuildContext context,
        IconThemeData? iconTheme = null
    )
    {
        ThemeData themeData = Theme.of(context);
        TabBarThemeData tabBarTheme = TabBarTheme.of(context);
        var animation = ((Animation<double>?)listenable)!;
        Color selectedColor =
            (
                ((labelColor ?? tabBarTheme.labelColor) ?? labelStyle?.color)
                ?? tabBarTheme.labelStyle?.color
            ) ?? defaults.labelColor!;
        Color unselectedColor = default!;
        if (selectedColor is WidgetStateColor)
        {
            WidgetStateColor selectedColor__8913__as9128 = (WidgetStateColor)selectedColor;
            unselectedColor = selectedColor__8913__as9128.resolve(new HashSet<WidgetState>());
            selectedColor = selectedColor__8913__as9128.resolve(
                new HashSet<WidgetState> { WidgetState.selected }
            );
        }
        else
        {
            unselectedColor =
                (
                    (
                        (
                            (unselectedLabelColor ?? tabBarTheme.unselectedLabelColor)
                            ?? unselectedLabelStyle?.color
                        ) ?? tabBarTheme.unselectedLabelStyle?.color
                    ) ?? iconTheme?.color
                ) ?? defaults.unselectedLabelColor!;
        }
        return WidgetStateColor.CreateResolveWith(
            (states) =>
            {
                if (states.Contains(WidgetState.selected))
                {
                    return DorotiUiLibrary.Color.lerp(
                        selectedColor,
                        unselectedColor,
                        animation.value
                    )!;
                }
                return DorotiUiLibrary.Color.lerp(unselectedColor, selectedColor, animation.value)!;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
    }

    public override Widget build(BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        TabBarThemeData tabBarTheme = TabBarTheme.of(context);
        var animation = ((Animation<double>?)listenable)!;
        var states = isSelected
            ? new HashSet<WidgetState> { WidgetState.selected }
            : new HashSet<WidgetState>();
        TextStyle selectedStyle = defaults
            .labelStyle!.merge(labelStyle ?? tabBarTheme.labelStyle)
            .copyWith(inherit: true);
        TextStyle unselectedStyle = defaults
            .unselectedLabelStyle!.merge(
                (unselectedLabelStyle ?? tabBarTheme.unselectedLabelStyle) ?? labelStyle
            )
            .copyWith(inherit: true);
        TextStyle textStyle = isSelected
            ? TextStyle.lerp(selectedStyle, unselectedStyle, animation.value)!
            : TextStyle.lerp(unselectedStyle, selectedStyle, animation.value)!;
        Color defaultIconColor = theme.colorScheme.brightness switch
        {
            Brightness.light => ConstantsLibrary.kDefaultIconDarkColor,
            Brightness.dark => ConstantsLibrary.kDefaultIconLightColor,
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        IconThemeData? customIconTheme = IconTheme.of(context) switch
        {
            IconThemeData iconThemeLocal when !Equals(iconThemeLocal.color, defaultIconColor) =>
                iconThemeLocal,
            _ => DartRuntimePrimitives.ConvertValue<IconThemeData>(null),
        };
        Color iconColor = _resolveWithLabelColor(context, iconTheme: customIconTheme)
            .resolve(states);
        Color labelColor = _resolveWithLabelColor(context).resolve(states);
        return new DefaultTextStyle(
            style: textStyle.copyWith(color: labelColor),
            child: IconTheme.merge(
                data: new IconThemeData(size: customIconTheme?.size ?? 24.0, color: iconColor),
                child: child
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal delegate void _LayoutCallback__tabs(
    List<double> xOffsets,
    TextDirection textDirection,
    double width
);

public class _TabLabelBarRenderer__tabs : RenderFlex
{
    public virtual Action<List<double>, TextDirection, double> onPerformLayout { get; set; } =
        default!;

    internal _TabLabelBarRenderer__tabs(
        Axis direction,
        MainAxisSize mainAxisSize,
        MainAxisAlignment mainAxisAlignment,
        CrossAxisAlignment crossAxisAlignment,
        TextDirection textDirection,
        VerticalDirection verticalDirection,
        Action<List<double>, TextDirection, double> onPerformLayout
    )
        : base(
            direction: direction,
            mainAxisSize: mainAxisSize,
            mainAxisAlignment: mainAxisAlignment,
            crossAxisAlignment: crossAxisAlignment,
            textDirection: textDirection,
            verticalDirection: verticalDirection
        )
    {
        this.onPerformLayout = onPerformLayout;
    }

    public override void performLayout()
    {
        base.performLayout();
        RenderBox? child = firstChild;
        var xOffsets = new List<double>();
        while (child is not null)
        {
            var childParentData = ((FlexParentData?)child.parentData!)!;
            xOffsets.Add(childParentData.offset.dx);
            DartRuntimePrimitives.Assert(() => Equals(child.parentData, childParentData));
            child = childParentData.nextSibling;
        }
        DartRuntimePrimitives.Assert(() => textDirection is not null);
        switch (
            (
                textDirection
                ?? throw new global::System.NullReferenceException("A required value was null.")
            )
        )
        {
            case TextDirection.rtl:
            {
                xOffsets.Insert(checked((int)0L), size.width);
                break;
            }
            case TextDirection.ltr:
            {
                xOffsets.Add(size.width);
                break;
            }
        }
        onPerformLayout(
            xOffsets,
            (
                textDirection
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ),
            size.width
        );
    }
}

internal class _TabLabelBar__tabs : Flex
{
    public virtual Action<List<double>, TextDirection, double> onPerformLayout
    {
        get;
        private set;
    } = default!;

    internal _TabLabelBar__tabs(
        List<Widget> children = default!,
        Action<List<double>, TextDirection, double> onPerformLayout = default!,
        MainAxisSize mainAxisSize = default!
    )
        : base(
            children: children ?? new List<Widget>(),
            mainAxisSize: mainAxisSize,
            direction: Axis.horizontal,
            mainAxisAlignment: MainAxisAlignment.start,
            crossAxisAlignment: CrossAxisAlignment.center,
            verticalDirection: VerticalDirection.down
        )
    {
        this.onPerformLayout = onPerformLayout;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _TabLabelBarRenderer__tabs(
            direction: direction,
            mainAxisAlignment: mainAxisAlignment,
            mainAxisSize: mainAxisSize,
            crossAxisAlignment: crossAxisAlignment,
            textDirection: (
                getEffectiveTextDirection(context)
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ),
            verticalDirection: verticalDirection,
            onPerformLayout: onPerformLayout
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_TabLabelBarRenderer__tabs)renderObject;
        base.updateRenderObject(context, __renderObject);
        __renderObject.onPerformLayout = onPerformLayout;
    }
}

public static partial class TabsLibrary
{
    internal static double _indexChangeProgress(TabController controller)
    {
        double controllerValue = controller.animation!.value;
        double previousIndexLocal = controller.previousIndex.toDouble();
        double currentIndex = controller.index.toDouble();
        if (!controller.indexIsChanging)
        {
            return DorotiUiLibrary.clampDouble((currentIndex - controllerValue).abs(), 0.0, 1.0);
        }
        return (controllerValue - currentIndex).abs() / (currentIndex - previousIndexLocal).abs();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _DividerPainter__tabs : CustomPainter
{
    public virtual Color dividerColor { get; private set; } = default!;
    public virtual double dividerHeight { get; private set; } = default!;

    internal _DividerPainter__tabs(Color dividerColor, double dividerHeight)
    {
        this.dividerColor = dividerColor;
        this.dividerHeight = dividerHeight;
    }

    public override void paint(Canvas canvas, Size size)
    {
        if (dividerHeight <= 0.0)
        {
            return;
        }
        var paintLocal = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.color = dividerColor;
                    __cascade.strokeWidth = dividerHeight;
                    return __cascade;
                }
            )
        )();
        canvas.drawLine(
            new Offset(0, size.height - (paintLocal.strokeWidth / 2L)),
            new Offset(size.width, size.height - (paintLocal.strokeWidth / 2L)),
            paintLocal
        );
    }

    public override bool shouldRepaint(CustomPainter oldDelegate)
    {
        var __oldDelegate = (_DividerPainter__tabs)oldDelegate;
        return (!Equals(__oldDelegate.dividerColor, dividerColor))
            || (__oldDelegate.dividerHeight != dividerHeight);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _IndicatorPainterNotifier__tabs : ChangeNotifier
{
    public virtual void notify()
    {
        notifyListeners();
    }

    public override string ToString() => DiagnosticsLibrary.describeIdentity(this);
}

internal class _IndicatorPainter__tabs : CustomPainter
{
    public virtual TabController controller { get; private set; } = default!;
    public virtual Decoration indicator { get; private set; } = default!;
    public virtual TabBarIndicatorSize indicatorSize { get; private set; } = default!;
    public virtual EdgeInsetsGeometry indicatorPadding { get; private set; } = default!;
    public virtual List<GlobalKey<IState>> tabKeys { get; private set; } = default!;
    public virtual List<EdgeInsetsGeometry> labelPaddings { get; private set; } = default!;
    public virtual Color? dividerColor { get; private set; }
    public virtual double? dividerHeight { get; private set; }
    public virtual bool showDivider { get; private set; } = default!;
    public virtual double? devicePixelRatio { get; private set; }
    public virtual TabIndicatorAnimation indicatorAnimation { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;

    // Dart library-private member: distinct from the same name in the base library.
    internal virtual _IndicatorPainterNotifier__tabs _repaint { get; private set; } = default!;
    internal virtual List<double>? _currentTabOffsets { get; set; } = default;
    internal virtual TextDirection? _currentTextDirection { get; set; } = default;
    internal virtual Rect? _currentRect { get; set; } = default;
    internal virtual BoxPainter? _painter { get; set; } = default;
    internal virtual bool _needsPaint { get; set; } = false;

    internal static _IndicatorPainter__tabs Create(
        TabController controller,
        Decoration indicator,
        TabBarIndicatorSize indicatorSize,
        List<GlobalKey<IState>> tabKeys,
        _IndicatorPainter__tabs? old,
        EdgeInsetsGeometry indicatorPadding,
        List<EdgeInsetsGeometry> labelPaddings,
        Color? dividerColor = null,
        double? dividerHeight = null,
        bool showDivider = default!,
        double? devicePixelRatio = null,
        TabIndicatorAnimation indicatorAnimation = default!,
        TextDirection textDirection = default!
    )
    {
        return new _IndicatorPainter__tabs(
            controller: controller,
            indicator: indicator,
            indicatorSize: indicatorSize,
            tabKeys: tabKeys,
            old: old,
            indicatorPadding: indicatorPadding,
            labelPaddings: labelPaddings,
            dividerColor: dividerColor,
            dividerHeight: dividerHeight,
            showDivider: showDivider,
            devicePixelRatio: devicePixelRatio,
            indicatorAnimation: indicatorAnimation,
            textDirection: ((textDirection)),
            repaint: new _IndicatorPainterNotifier__tabs()
        );
    }

    internal _IndicatorPainter__tabs(
        TabController controller,
        Decoration indicator,
        TabBarIndicatorSize indicatorSize,
        List<GlobalKey<IState>> tabKeys,
        _IndicatorPainter__tabs? old,
        EdgeInsetsGeometry indicatorPadding,
        List<EdgeInsetsGeometry> labelPaddings,
        Color? dividerColor = null,
        double? dividerHeight = null,
        bool showDivider = default!,
        double? devicePixelRatio = null,
        TabIndicatorAnimation indicatorAnimation = default!,
        TextDirection textDirection = default!,
        _IndicatorPainterNotifier__tabs repaint = default!
    )
        : base(
            repaint: Listenable.CreateMerge(
                new List<Listenable?> { controller.animation, repaint }.Cast<Listenable?>()
            )
        )
    {
        this.controller = controller;
        this.indicator = indicator;
        this.indicatorSize = indicatorSize;
        this.tabKeys = tabKeys;
        this.indicatorPadding = indicatorPadding;
        this.labelPaddings = labelPaddings;
        this.dividerColor = dividerColor;
        this.dividerHeight = dividerHeight;
        this.showDivider = showDivider;
        this.devicePixelRatio = devicePixelRatio;
        this.indicatorAnimation = indicatorAnimation;
        this.textDirection = textDirection;
        _repaint = repaint;
        DartRuntimePrimitives.Assert(() =>
            Foundation.DebugLibrary.debugMaybeDispatchCreated("material", "_IndicatorPainter", this)
        );
        if (old is not null)
        {
            saveTabOffsets(old._currentTabOffsets, old._currentTextDirection);
        }
    }

    public virtual void markNeedsPaint()
    {
        _needsPaint = true;
        _repaint.notify();
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
            Foundation.DebugLibrary.debugMaybeDispatchDisposed(this)
        );
        _painter?.dispose();
        _repaint.dispose();
    }

    public virtual void saveTabOffsets(List<double>? tabOffsets, TextDirection? textDirection)
    {
        _currentTabOffsets = tabOffsets;
        _currentTextDirection = textDirection;
    }

    public virtual long maxTabIndex =>
        DartRuntimePrimitives.ConvertValue<long>(checked(_currentTabOffsets!.Count) - 2L);

    public virtual double centerOf(long tabIndex)
    {
        DartRuntimePrimitives.Assert(() => _currentTabOffsets is not null);
        DartRuntimePrimitives.Assert(() => Enumerable.Any(_currentTabOffsets!));
        DartRuntimePrimitives.Assert(() => tabIndex >= 0L);
        DartRuntimePrimitives.Assert(() => tabIndex <= maxTabIndex);
        return (_currentTabOffsets![(int)tabIndex] + _currentTabOffsets![(int)(tabIndex + 1L)])
            / 2.0;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Rect indicatorRect(Size tabBarSize, long tabIndex)
    {
        DartRuntimePrimitives.Assert(() => _currentTabOffsets is not null);
        DartRuntimePrimitives.Assert(() => _currentTextDirection is not null);
        DartRuntimePrimitives.Assert(() => Enumerable.Any(_currentTabOffsets!));
        DartRuntimePrimitives.Assert(() => tabIndex >= 0L);
        DartRuntimePrimitives.Assert(() => tabIndex <= maxTabIndex);
        double tabLeft = default!;
        double tabRight = default!;
        DartRuntimePrimitives.Ignore(
            (tabLeft, tabRight) = (
                _currentTextDirection
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ) switch
            {
                TextDirection.rtl => (
                    _currentTabOffsets![(int)(tabIndex + 1L)],
                    _currentTabOffsets![(int)tabIndex]
                ),
                TextDirection.ltr => (
                    _currentTabOffsets![(int)tabIndex],
                    _currentTabOffsets![(int)(tabIndex + 1L)]
                ),
                _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                    throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    ),
            }
        );
        if (Equals(indicatorSize, TabBarIndicatorSize.label))
        {
            double tabWidth = (
                tabKeys[(int)tabIndex].currentContext!.size
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ).width;
            EdgeInsetsGeometry labelPadding = labelPaddings[(int)tabIndex];
            EdgeInsets insets = labelPadding.resolve(_currentTextDirection);
            double delta = (tabRight - tabLeft - (tabWidth + insets.horizontal)) / 2.0;
            tabLeft += delta + insets.left;
            tabRight = tabLeft + tabWidth;
        }
        EdgeInsets insetsLocal = indicatorPadding.resolve(_currentTextDirection);
        var rect = Rect.fromLTWH(tabLeft, 0.0, tabRight - tabLeft, tabBarSize.height);
        if (!(rect.size >= insetsLocal.collapsedSize))
        {
            throw DartRuntimePrimitives.AsException(
                FlutterError.Create(
                    "indicatorPadding insets should be less than Tab Size\n"
                        + $"Rect Size : {rect.size}, Insets: {insetsLocal}"
                )
            );
        }
        return insetsLocal.deflateRect(rect);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void paint(Canvas canvas, Size size)
    {
        _needsPaint = false;
        _painter ??= indicator.createBoxPainter(() => markNeedsPaint());
        double valueLocal = controller.animation!.value;
        _currentRect = indicatorAnimation switch
        {
            TabIndicatorAnimation.linear => _applyLinearEffect(size: size, value: valueLocal),
            TabIndicatorAnimation.elastic => _applyElasticEffect(size: size, value: valueLocal),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        DartRuntimePrimitives.Assert(() => _currentRect is not null);
        var configuration = new ImageConfiguration(
            size: (
                _currentRect
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ).size,
            textDirection: _currentTextDirection,
            devicePixelRatio: devicePixelRatio
        );
        if (
            showDivider
            && (
                (
                    dividerHeight
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ) > 0L
            )
        )
        {
            var dividerPaint = (
                (Func<Paint>)(
                    () =>
                    {
                        var __cascade = new Paint();
                        __cascade.color = dividerColor!;
                        __cascade.strokeWidth = (
                            dividerHeight
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        );
                        return __cascade;
                    }
                )
            )();
            var dividerP1 = new Offset(0, size.height - (dividerPaint.strokeWidth / 2L));
            var dividerP2 = new Offset(size.width, size.height - (dividerPaint.strokeWidth / 2L));
            canvas.drawLine(dividerP1, dividerP2, dividerPaint);
        }
        _painter!.paint(
            canvas,
            (
                _currentRect
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ).topLeft,
            configuration
        );
    }

    internal virtual Rect? _applyLinearEffect(Size size, double value)
    {
        double indexLocal = controller.index.toDouble();
        bool ltr = indexLocal > value;
        long @from = (ltr ? value.floor() : value.ceil()).clamp(0L, maxTabIndex);
        long to = (ltr ? (@from + 1L) : (@from - 1L)).clamp(0L, maxTabIndex);
        Rect fromRect = indicatorRect(size, @from);
        Rect toRect = indicatorRect(size, to);
        return DorotiUiLibrary.Rect.lerp(fromRect, toRect, (value - @from).abs());
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual double decelerateInterpolation(double fraction)
    {
        return Math.Sin(fraction * Math.PI / 2.0);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual double accelerateInterpolation(double fraction)
    {
        return 1.0 - Math.Cos(fraction * Math.PI / 2.0);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Rect? _applyElasticEffect(Size size, double value)
    {
        double indexLocal = controller.index.toDouble();
        double progressLeft = (indexLocal - value).abs();
        long to =
            ((progressLeft == 0.0) || !controller.indexIsChanging)
                ? (
                    textDirection switch
                    {
                        TextDirection.ltr => value.ceil(),
                        TextDirection.rtl => value.floor(),
                        _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                            throw new InvalidOperationException(
                                "Switch expression did not handle the supplied value."
                            ),
                    }
                ).clamp(0L, maxTabIndex)
                : controller.index;
        long @from =
            ((progressLeft == 0.0) || !controller.indexIsChanging)
                ? (
                    textDirection switch
                    {
                        TextDirection.ltr => to - 1L,
                        TextDirection.rtl => to + 1L,
                        _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                            throw new InvalidOperationException(
                                "Switch expression did not handle the supplied value."
                            ),
                    }
                ).clamp(0L, maxTabIndex)
                : controller.previousIndex;
        Rect toRect = indicatorRect(size, to);
        Rect fromRect = indicatorRect(size, @from);
        Rect rect = (
            DorotiUiLibrary.Rect.lerp(fromRect, toRect, (value - @from).abs())
            ?? throw new global::System.NullReferenceException("A required value was null.")
        );
        if (controller.animation!.isCompleted)
        {
            return rect;
        }
        double tabChangeProgress = default!;
        if (controller.indexIsChanging)
        {
            long tabsDelta = (controller.index - controller.previousIndex).abs();
            if (tabsDelta != 0L)
            {
                progressLeft /= tabsDelta;
            }
            tabChangeProgress = 1L - DorotiUiLibrary.clampDouble(progressLeft, 0.0, 1.0);
        }
        else
        {
            tabChangeProgress = (indexLocal - value).abs();
        }
        if (tabChangeProgress == 1.0)
        {
            return rect;
        }
        double leftFraction = default!;
        double rightFraction = default!;
        bool isMovingRight = textDirection switch
        {
            TextDirection.ltr => controller.indexIsChanging
                ? (indexLocal > value)
                : (value > indexLocal),
            TextDirection.rtl => controller.indexIsChanging
                ? (value > indexLocal)
                : (indexLocal > value),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        if (isMovingRight)
        {
            leftFraction = accelerateInterpolation(tabChangeProgress);
            rightFraction = decelerateInterpolation(tabChangeProgress);
        }
        else
        {
            leftFraction = decelerateInterpolation(tabChangeProgress);
            rightFraction = accelerateInterpolation(tabChangeProgress);
        }
        double lerpRectLeft = default!;
        double lerpRectRight = default!;
        if (controller.indexIsChanging)
        {
            lerpRectLeft = (
                DorotiUiLibrary.lerpDouble(fromRect.left, toRect.left, leftFraction)
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            lerpRectRight = (
                DorotiUiLibrary.lerpDouble(fromRect.right, toRect.right, rightFraction)
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        }
        else
        {
            lerpRectLeft = (object)isMovingRight switch
            {
                true => (
                    DorotiUiLibrary.lerpDouble(fromRect.left, toRect.left, leftFraction)
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                false => (
                    DorotiUiLibrary.lerpDouble(toRect.left, fromRect.left, leftFraction)
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                    throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    ),
            };
            lerpRectRight = (object)isMovingRight switch
            {
                true => (
                    DorotiUiLibrary.lerpDouble(fromRect.right, toRect.right, rightFraction)
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                false => (
                    DorotiUiLibrary.lerpDouble(toRect.right, fromRect.right, rightFraction)
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                    throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    ),
            };
        }
        return Rect.fromLTRB(lerpRectLeft, rect.top, lerpRectRight, rect.bottom);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool shouldRepaint(CustomPainter oldDelegate)
    {
        var __old = (_IndicatorPainter__tabs)oldDelegate;
        return _needsPaint
            || (!Equals(controller, __old.controller))
            || (!Equals(indicator, __old.indicator))
            || (checked(tabKeys.Count) != checked((long)__old.tabKeys.Count))
            || (!CollectionsLibrary.listEquals(_currentTabOffsets, __old._currentTabOffsets))
            || (!Equals(_currentTextDirection, __old._currentTextDirection));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _ChangeAnimation__tabs : Animation<double>, AnimationWithParentMixin<double>
{
    public virtual TabController controller { get; private set; } = default!;

    internal _ChangeAnimation__tabs(TabController controller)
    {
        this.controller = controller;
    }

    public virtual Animation<double> parent =>
        DartRuntimePrimitives.ConvertValue<Animation<double>>(controller.animation!);

    public override void removeStatusListener(AnimationStatusListener listener)
    {
        if (controller.animation is not null)
        {
            DartRuntimePrimitives.Noop();
        }
    }

    public override void removeListener(Action listener)
    {
        if (controller.animation is not null)
        {
            DartRuntimePrimitives.Noop();
        }
    }

    public override double value => TabsLibrary._indexChangeProgress(controller);

    public override void addListener(Action listener) => parent.addListener(listener);

    public override void addStatusListener(AnimationStatusListener listener) =>
        parent.addStatusListener(listener);

    public override AnimationStatus status => parent.status;
}

internal class _DragAnimation__tabs : Animation<double>, AnimationWithParentMixin<double>
{
    public virtual TabController controller { get; private set; } = default!;
    public virtual long index { get; private set; } = default!;

    internal _DragAnimation__tabs(TabController controller, long index)
    {
        this.controller = controller;
        this.index = index;
    }

    public virtual Animation<double> parent =>
        DartRuntimePrimitives.ConvertValue<Animation<double>>(controller.animation!);

    public override void removeStatusListener(AnimationStatusListener listener)
    {
        if (controller.animation is not null)
        {
            DartRuntimePrimitives.Noop();
        }
    }

    public override void removeListener(Action listener)
    {
        if (controller.animation is not null)
        {
            DartRuntimePrimitives.Noop();
        }
    }

    public override double value
    {
        get
        {
            DartRuntimePrimitives.Assert(() => !controller.indexIsChanging);
            double controllerMaxValue = (controller.length - 1L).toDouble();
            double controllerValue = DorotiUiLibrary.clampDouble(
                controller.animation!.value,
                0.0,
                controllerMaxValue
            );
            return DorotiUiLibrary.clampDouble((controllerValue - index.toDouble()).abs(), 0.0, 1.0);
        }
    }

    public override void addListener(Action listener) => parent.addListener(listener);

    public override void addStatusListener(AnimationStatusListener listener) =>
        parent.addStatusListener(listener);

    public override AnimationStatus status => parent.status;
}

internal class _TabBarScrollPosition__tabs : ScrollPositionWithSingleContext
{
    public virtual _TabBarState__tabs tabBar { get; private set; } = default!;
    internal virtual bool _viewportDimensionWasNonZero { get; set; } = false;
    internal virtual bool _needsPixelsCorrection { get; set; } = true;

    internal _TabBarScrollPosition__tabs(
        ScrollPhysics physics,
        ScrollContext context,
        ScrollPosition? oldPosition,
        _TabBarState__tabs tabBar
    )
        : base(physics: physics, context: context, oldPosition: oldPosition, initialPixels: null)
    {
        this.tabBar = tabBar;
    }

    public override bool applyContentDimensions(double minScrollExtent, double maxScrollExtent)
    {
        var result = true;
        if (!_viewportDimensionWasNonZero)
        {
            _viewportDimensionWasNonZero = viewportDimension != 0.0;
        }
        if (!_viewportDimensionWasNonZero || _needsPixelsCorrection)
        {
            _needsPixelsCorrection = false;
            correctPixels(
                tabBar._initialScrollOffset(viewportDimension, minScrollExtent, maxScrollExtent)
            );
            result = false;
        }
        return base.applyContentDimensions(minScrollExtent, maxScrollExtent) && result;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void markNeedsPixelsCorrection()
    {
        _needsPixelsCorrection = true;
    }
}

public class TabBarScrollController : ScrollController
{
    internal virtual _TabBarState__tabs? _tabBarState { get; set; } = default;

    public virtual bool debugCheckHasTabBarState()
    {
        DartRuntimePrimitives.Assert(
            () => _tabBarState is not null,
            () => (object?)"This TabBarScrollController is not attached to any TabBar."
        );
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override ScrollPosition createScrollPosition(
        ScrollPhysics physics,
        ScrollContext context,
        ScrollPosition? oldPosition
    )
    {
        DartRuntimePrimitives.Assert(() => debugCheckHasTabBarState());
        return new _TabBarScrollPosition__tabs(
            physics: physics,
            context: context,
            oldPosition: oldPosition,
            tabBar: _tabBarState!
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void dispose()
    {
        _tabBarState = null;
        base.dispose();
    }
}

public delegate void TabValueChanged<T>(T value, long index);

public class TabBar : StatefulWidget, PreferredSizeWidget
{
    public virtual List<Widget> tabs { get; private set; } = default!;
    public virtual TabController? controller { get; private set; }
    public virtual TabBarScrollController? scrollController { get; private set; }
    public virtual bool isScrollable { get; private set; } = default!;
    public virtual EdgeInsetsGeometry? padding { get; private set; }
    public virtual Color? indicatorColor { get; private set; }
    public virtual double indicatorWeight { get; private set; } = default!;
    public virtual EdgeInsetsGeometry indicatorPadding { get; private set; } = default!;
    public virtual Decoration? indicator { get; private set; }
    public virtual bool automaticIndicatorColorAdjustment { get; private set; } = default!;
    public virtual TabBarIndicatorSize? indicatorSize { get; private set; }
    public virtual Color? dividerColor { get; private set; }
    public virtual double? dividerHeight { get; private set; }
    public virtual Color? labelColor { get; private set; }
    public virtual Color? unselectedLabelColor { get; private set; }
    public virtual TextStyle? labelStyle { get; private set; }
    public virtual TextStyle? unselectedLabelStyle { get; private set; }
    public virtual EdgeInsetsGeometry? labelPadding { get; private set; }
    public virtual WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual MouseCursor? mouseCursor { get; private set; }
    public virtual bool? enableFeedback { get; private set; }
    public virtual Action<long>? onTap { get; private set; }
    public virtual Action<bool, long>? onHover { get; private set; }
    public virtual Action<bool, long>? onFocusChange { get; private set; }
    public virtual ScrollPhysics? physics { get; private set; }
    public virtual InteractiveInkFeatureFactory? splashFactory { get; private set; }
    public virtual BorderRadius? splashBorderRadius { get; private set; }
    public virtual TabAlignment? tabAlignment { get; private set; }
    public virtual TextScaler? textScaler { get; private set; }
    public virtual TabIndicatorAnimation? indicatorAnimation { get; private set; }
    internal virtual bool _isPrimary { get; private set; } = default!;

    public TabBar(
        Key? key = null,
        List<Widget> tabs = default!,
        TabController? controller = null,
        TabBarScrollController? scrollController = null,
        bool isScrollable = false,
        EdgeInsetsGeometry? padding = null,
        Color? indicatorColor = null,
        bool automaticIndicatorColorAdjustment = true,
        double indicatorWeight = 2.0,
        EdgeInsetsGeometry indicatorPadding = default!,
        Decoration? indicator = null,
        TabBarIndicatorSize? indicatorSize = null,
        Color? dividerColor = null,
        double? dividerHeight = null,
        Color? labelColor = null,
        TextStyle? labelStyle = null,
        EdgeInsetsGeometry? labelPadding = null,
        Color? unselectedLabelColor = null,
        TextStyle? unselectedLabelStyle = null,
        Gestures.DragStartBehavior dragStartBehavior = Gestures.DragStartBehavior.start,
        WidgetStateProperty<Color?>? overlayColor = null,
        MouseCursor? mouseCursor = null,
        bool? enableFeedback = null,
        Action<long>? onTap = null,
        Action<bool, long>? onHover = null,
        Action<bool, long>? onFocusChange = null,
        ScrollPhysics? physics = null,
        InteractiveInkFeatureFactory? splashFactory = null,
        BorderRadius? splashBorderRadius = null,
        TabAlignment? tabAlignment = null,
        TextScaler? textScaler = null,
        TabIndicatorAnimation? indicatorAnimation = null
    )
        : base(key: key)
    {
        EdgeInsetsGeometry __indicatorPadding = indicatorPadding ?? EdgeInsets.zero;
        this.tabs = tabs;
        this.controller = controller;
        this.scrollController = scrollController;
        this.isScrollable = isScrollable;
        this.padding = padding;
        this.indicatorColor = indicatorColor;
        this.automaticIndicatorColorAdjustment = automaticIndicatorColorAdjustment;
        this.indicatorWeight = indicatorWeight;
        this.indicatorPadding = __indicatorPadding;
        this.indicator = indicator;
        this.indicatorSize = indicatorSize;
        this.dividerColor = dividerColor;
        this.dividerHeight = dividerHeight;
        this.labelColor = labelColor;
        this.labelStyle = labelStyle;
        this.labelPadding = labelPadding;
        this.unselectedLabelColor = unselectedLabelColor;
        this.unselectedLabelStyle = unselectedLabelStyle;
        this.dragStartBehavior = dragStartBehavior;
        this.overlayColor = overlayColor;
        this.mouseCursor = mouseCursor;
        this.enableFeedback = enableFeedback;
        this.onTap = onTap;
        this.onHover = onHover;
        this.onFocusChange = onFocusChange;
        this.physics = physics;
        this.splashFactory = splashFactory;
        this.splashBorderRadius = splashBorderRadius;
        this.tabAlignment = tabAlignment;
        this.textScaler = textScaler;
        this.indicatorAnimation = indicatorAnimation;
        _isPrimary = true;
        System.Diagnostics.Debug.Assert((indicator is not null) || indicatorWeight > 0.0);
    }

    public static TabBar CreateSecondary(
        Key? key = null,
        List<Widget> tabs = default!,
        TabController? controller = null,
        TabBarScrollController? scrollController = null,
        bool isScrollable = false,
        EdgeInsetsGeometry? padding = null,
        Color? indicatorColor = null,
        bool automaticIndicatorColorAdjustment = true,
        double indicatorWeight = 2.0,
        EdgeInsetsGeometry indicatorPadding = default!,
        Decoration? indicator = null,
        TabBarIndicatorSize? indicatorSize = null,
        Color? dividerColor = null,
        double? dividerHeight = null,
        Color? labelColor = null,
        TextStyle? labelStyle = null,
        EdgeInsetsGeometry? labelPadding = null,
        Color? unselectedLabelColor = null,
        TextStyle? unselectedLabelStyle = null,
        Gestures.DragStartBehavior dragStartBehavior = Gestures.DragStartBehavior.start,
        WidgetStateProperty<Color?>? overlayColor = null,
        MouseCursor? mouseCursor = null,
        bool? enableFeedback = null,
        Action<long>? onTap = null,
        Action<bool, long>? onHover = null,
        Action<bool, long>? onFocusChange = null,
        ScrollPhysics? physics = null,
        InteractiveInkFeatureFactory? splashFactory = null,
        BorderRadius? splashBorderRadius = null,
        TabAlignment? tabAlignment = null,
        TextScaler? textScaler = null,
        TabIndicatorAnimation? indicatorAnimation = null
    )
    {
        var __instance = new TabBar(
            key: key,
            tabs: tabs,
            controller: controller,
            scrollController: scrollController,
            isScrollable: isScrollable,
            padding: padding,
            indicatorColor: indicatorColor,
            automaticIndicatorColorAdjustment: automaticIndicatorColorAdjustment,
            indicatorWeight: indicatorWeight,
            indicatorPadding: indicatorPadding,
            indicator: indicator,
            indicatorSize: indicatorSize,
            dividerColor: dividerColor,
            dividerHeight: dividerHeight,
            labelColor: labelColor,
            labelStyle: labelStyle,
            labelPadding: labelPadding,
            unselectedLabelColor: unselectedLabelColor,
            unselectedLabelStyle: unselectedLabelStyle,
            dragStartBehavior: dragStartBehavior,
            overlayColor: overlayColor,
            mouseCursor: mouseCursor,
            enableFeedback: enableFeedback,
            onTap: onTap,
            onHover: onHover,
            onFocusChange: onFocusChange,
            physics: physics,
            splashFactory: splashFactory,
            splashBorderRadius: splashBorderRadius,
            tabAlignment: tabAlignment,
            textScaler: textScaler,
            indicatorAnimation: indicatorAnimation
        );
        EdgeInsetsGeometry __indicatorPadding = indicatorPadding ?? EdgeInsets.zero;
        __instance.tabs = tabs;
        __instance.controller = controller;
        __instance.scrollController = scrollController;
        __instance.isScrollable = isScrollable;
        __instance.padding = padding;
        __instance.indicatorColor = indicatorColor;
        __instance.automaticIndicatorColorAdjustment = automaticIndicatorColorAdjustment;
        __instance.indicatorWeight = indicatorWeight;
        __instance.indicatorPadding = __indicatorPadding;
        __instance.indicator = indicator;
        __instance.indicatorSize = indicatorSize;
        __instance.dividerColor = dividerColor;
        __instance.dividerHeight = dividerHeight;
        __instance.labelColor = labelColor;
        __instance.labelStyle = labelStyle;
        __instance.labelPadding = labelPadding;
        __instance.unselectedLabelColor = unselectedLabelColor;
        __instance.unselectedLabelStyle = unselectedLabelStyle;
        __instance.dragStartBehavior = dragStartBehavior;
        __instance.overlayColor = overlayColor;
        __instance.mouseCursor = mouseCursor;
        __instance.enableFeedback = enableFeedback;
        __instance.onTap = onTap;
        __instance.onHover = onHover;
        __instance.onFocusChange = onFocusChange;
        __instance.physics = physics;
        __instance.splashFactory = splashFactory;
        __instance.splashBorderRadius = splashBorderRadius;
        __instance.tabAlignment = tabAlignment;
        __instance.textScaler = textScaler;
        __instance.indicatorAnimation = indicatorAnimation;
        __instance._isPrimary = false;
        return __instance;
    }

    public virtual Size preferredSize
    {
        get
        {
            double maxHeight = TabsLibrary._kTabHeight;
            foreach (Widget item in tabs)
            {
                if (item is PreferredSizeWidget)
                {
                    PreferredSizeWidget item__55453__as55479 = (PreferredSizeWidget)item;
                    double itemHeight = item__55453__as55479.preferredSize.height;
                    maxHeight = Math.Max(itemHeight, maxHeight);
                }
            }
            return new Size(maxHeight + indicatorWeight);
        }
    }
    public virtual bool tabHasTextAndIcon
    {
        get
        {
            foreach (Widget item in tabs)
            {
                if (item is PreferredSizeWidget)
                {
                    PreferredSizeWidget item__56008__as56034 = (PreferredSizeWidget)item;
                    if (
                        item__56008__as56034.preferredSize.height
                        == TabsLibrary._kTextAndIconTabHeight
                    )
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _TabBarState__tabs());
}

internal class _TabBarState__tabs : State<TabBar>
{
    internal virtual TabBarScrollController? _internalScrollController { get; set; } = default;
    internal virtual TabController? _controller { get; set; } = default;
    internal virtual _IndicatorPainter__tabs? _indicatorPainter { get; set; } = default;
    internal virtual long? _currentIndex { get; set; } = default;
    internal virtual double _tabStripWidth { get; set; } = default!;
    internal virtual List<GlobalKey<IState>> _tabKeys { get; set; } = default!;
    internal virtual List<EdgeInsetsGeometry> _labelPaddings { get; set; } = default!;
    internal virtual bool _debugHasScheduledValidTabsCountCheck { get; set; } = false;

    public override void initState()
    {
        base.initState();
        _tabKeys = widget.tabs.map((tab) => GlobalKey<IState>.Create()).ToList();
        _labelPaddings = new List<EdgeInsetsGeometry>(
            Enumerable.Repeat<EdgeInsetsGeometry>(
                EdgeInsets.zero,
                checked((int)checked((long)widget.tabs.Count))
            )
        );
    }

    internal virtual TabBarThemeData _defaults
    {
        get
        {
            {
                return widget._isPrimary
                    ? new _TabsPrimaryDefaultsM3__tabs(context, widget.isScrollable)
                    : new _TabsSecondaryDefaultsM3__tabs(context, widget.isScrollable);
            }
        }
    }
    internal virtual TabBarScrollController _effectiveScrollController
    {
        get
        {
            if (widget.scrollController is not null)
            {
                _internalScrollController?.dispose();
                _internalScrollController = null;
                return widget.scrollController!;
            }
            return _internalScrollController ??= new TabBarScrollController();
        }
    }

    internal virtual Decoration _getIndicator(TabBarIndicatorSize indicatorSize)
    {
        ThemeData theme = Theme.of(context);
        TabBarThemeData tabBarTheme = TabBarTheme.of(context);
        if (widget.indicator is not null)
        {
            return widget.indicator!;
        }
        if (tabBarTheme.indicator is not null)
        {
            return tabBarTheme.indicator!;
        }
        Color colorLocal =
            (widget.indicatorColor ?? tabBarTheme.indicatorColor) ?? _defaults.indicatorColor!;
        if (
            widget.automaticIndicatorColorAdjustment
            && (colorLocal.value == Material.maybeOf(context)?.color?.value)
        )
        {
            colorLocal = Colors.white;
        }
        double effectiveIndicatorWeight = Math.Max(
            widget.indicatorWeight,
            (object)widget._isPrimary switch
            {
                true => _TabsPrimaryDefaultsM3__tabs.indicatorWeight(indicatorSize),
                false => _TabsSecondaryDefaultsM3__tabs.indicatorWeight,
                _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                    throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    ),
            }
        );
        bool primaryWithLabelIndicator = indicatorSize switch
        {
            TabBarIndicatorSize.label => widget._isPrimary,
            TabBarIndicatorSize.tab => false,
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        BorderRadius? effectiveBorderRadius = primaryWithLabelIndicator
            ? new BorderRadius(
                topLeft: Radius.circular(effectiveIndicatorWeight),
                topRight: Radius.circular(effectiveIndicatorWeight)
            )
            : null;
        return new UnderlineTabIndicator(
            borderRadius: effectiveBorderRadius,
            borderSide: new BorderSide(width: effectiveIndicatorWeight, color: colorLocal)
        );
    }

    internal virtual bool _controllerIsValid =>
        DartRuntimePrimitives.ConvertValue<bool>(_controller?.animation is not null);

    internal virtual void _updateTabController()
    {
        TabController? newController = widget.controller ?? DefaultTabController.maybeOf(context);
        DartRuntimePrimitives.Assert(() =>
        {
            if (newController is null)
            {
                throw DartRuntimePrimitives.AsException(
                    FlutterError.Create(
                        $"No TabController for {DartRuntimePrimitives.RuntimeType(widget)}.\n"
                            + $"When creating a {DartRuntimePrimitives.RuntimeType(widget)}, you must either provide an explicit "
                            + "TabController using the \"controller\" property, or you must ensure that there "
                            + $"is a DefaultTabController above the {DartRuntimePrimitives.RuntimeType(widget)}.\n"
                            + "In this case, there was neither an explicit controller nor a default controller."
                    )
                );
            }
            return true;
        });
        if (Equals(newController, _controller))
        {
            return;
        }
        if (_controllerIsValid)
        {
            _controller!.animation!.removeListener(_handleTabControllerAnimationTick);
            _controller!.removeListener(_handleTabControllerTick);
        }
        _controller = newController;
        if (_controller is not null)
        {
            _controller!.animation!.addListener(_handleTabControllerAnimationTick);
            _controller!.addListener(_handleTabControllerTick);
            _currentIndex = _controller!.index;
        }
    }

    internal virtual void _updateScrollController(
        TabBarScrollController? oldScrollController = null
    )
    {
        if (!Equals(oldScrollController, widget.scrollController))
        {
            oldScrollController?._tabBarState = null;
        }
        if (widget.scrollController is not null)
        {
            _internalScrollController?._tabBarState = null;
            widget.scrollController?._tabBarState = this;
        }
        else
        {
            _internalScrollController ??= new TabBarScrollController();
            _internalScrollController?._tabBarState = this;
        }
    }

    internal virtual void _initIndicatorPainter()
    {
        ThemeData theme = Theme.of(context);
        TabBarThemeData tabBarTheme = TabBarTheme.of(context);
        TabBarIndicatorSize indicatorSizeLocal =
            (widget.indicatorSize ?? tabBarTheme.indicatorSize)
            ?? (
                _defaults.indicatorSize
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        _IndicatorPainter__tabs? oldPainter = _indicatorPainter;
        TabIndicatorAnimation defaultTabIndicatorAnimation = indicatorSizeLocal switch
        {
            TabBarIndicatorSize.label => TabIndicatorAnimation.elastic,
            TabBarIndicatorSize.tab => TabIndicatorAnimation.linear,
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        _indicatorPainter = !_controllerIsValid
            ? null
            : _IndicatorPainter__tabs.Create(
                controller: _controller!,
                indicator: _getIndicator(indicatorSizeLocal),
                indicatorSize: indicatorSizeLocal,
                indicatorPadding: widget.indicatorPadding,
                tabKeys: _tabKeys,
                old: oldPainter,
                labelPaddings: _labelPaddings,
                dividerColor: (widget.dividerColor ?? tabBarTheme.dividerColor)
                    ?? _defaults.dividerColor,
                dividerHeight: (widget.dividerHeight ?? tabBarTheme.dividerHeight)
                    ?? _defaults.dividerHeight,
                showDivider: !widget.isScrollable,
                devicePixelRatio: MediaQuery.devicePixelRatioOf(context),
                indicatorAnimation: (widget.indicatorAnimation ?? tabBarTheme.indicatorAnimation)
                    ?? defaultTabIndicatorAnimation,
                textDirection: Directionality.of(context)
            );
        oldPainter?.dispose();
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _updateScrollController();
        _updateTabController();
        _initIndicatorPainter();
    }

    public override void didUpdateWidget(TabBar oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (
            (!Equals(widget.controller, oldWidget.controller))
            || (!Equals(widget.scrollController, oldWidget.scrollController))
        )
        {
            _updateScrollController(oldScrollController: oldWidget.scrollController);
            _updateTabController();
            _initIndicatorPainter();
            if (_effectiveScrollController.hasClients)
            {
                ScrollPosition positionLocal = _effectiveScrollController.position;
                if (positionLocal is _TabBarScrollPosition__tabs)
                {
                    _TabBarScrollPosition__tabs position__64819__as64879 =
                        (_TabBarScrollPosition__tabs)positionLocal;
                    position__64819__as64879.markNeedsPixelsCorrection();
                }
            }
        }
        else
        {
            if (
                (!Equals(widget.indicatorColor, oldWidget.indicatorColor))
                || (widget.indicatorWeight != oldWidget.indicatorWeight)
                || (!Equals(widget.indicatorSize, oldWidget.indicatorSize))
                || (!Equals(widget.indicatorPadding, oldWidget.indicatorPadding))
                || (!Equals(widget.indicator, oldWidget.indicator))
                || (!Equals(widget.dividerColor, oldWidget.dividerColor))
                || (widget.dividerHeight != oldWidget.dividerHeight)
                || (!Equals(widget.indicatorAnimation, oldWidget.indicatorAnimation))
            )
            {
                _initIndicatorPainter();
            }
        }
        if (checked(widget.tabs.Count) > checked((long)_tabKeys.Count))
        {
            long delta = checked(widget.tabs.Count) - checked((long)_tabKeys.Count);
            _tabKeys.AddRange(
                DartRuntimePrimitives
                    .CreateList(delta, (n) => GlobalKey<IState>.Create())
                    .Cast<GlobalKey<IState>>()
            );
            _labelPaddings.AddRange(
                new List<EdgeInsetsGeometry>(
                    Enumerable.Repeat<EdgeInsetsGeometry>(EdgeInsets.zero, checked((int)delta))
                ).Cast<EdgeInsetsGeometry>()
            );
        }
        else
        {
            if (checked(widget.tabs.Count) < checked((long)_tabKeys.Count))
            {
                _tabKeys.RemoveRange(
                    checked((int)checked((long)widget.tabs.Count)),
                    checked((int)checked((long)_tabKeys.Count))
                );
                _labelPaddings.RemoveRange(
                    checked((int)checked((long)widget.tabs.Count)),
                    checked((int)checked((long)_tabKeys.Count))
                );
            }
        }
    }

    public override void dispose()
    {
        _indicatorPainter!.dispose();
        if (_controllerIsValid)
        {
            _controller!.animation!.removeListener(_handleTabControllerAnimationTick);
            _controller!.removeListener(_handleTabControllerTick);
        }
        _controller = null;
        _internalScrollController?.dispose();
        widget.scrollController?._tabBarState = null;
        base.dispose();
    }

    public virtual long maxTabIndex => _indicatorPainter!.maxTabIndex;

    internal virtual double _tabScrollOffset(
        long index,
        double viewportWidth,
        double minExtent,
        double maxExtent
    )
    {
        if (!widget.isScrollable)
        {
            return 0.0;
        }
        double tabCenter = _indicatorPainter!.centerOf(index);
        double paddingStart = default!;
        switch (Directionality.of(context))
        {
            case TextDirection.rtl:
            {
                paddingStart = widget.padding?.resolve(TextDirection.rtl).right ?? 0;
                tabCenter = _tabStripWidth - tabCenter;
                break;
            }
            case TextDirection.ltr:
            {
                paddingStart = widget.padding?.resolve(TextDirection.ltr).left ?? 0;
                break;
            }
        }
        return DorotiUiLibrary.clampDouble(
            tabCenter + paddingStart - (viewportWidth / 2.0),
            minExtent,
            maxExtent
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual double _tabCenteredScrollOffset(long index)
    {
        ScrollPosition positionLocal = _effectiveScrollController.position;
        return _tabScrollOffset(
            index,
            positionLocal.viewportDimension,
            positionLocal.minScrollExtent,
            positionLocal.maxScrollExtent
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual double _initialScrollOffset(
        double viewportWidth,
        double minExtent,
        double maxExtent
    )
    {
        return _tabScrollOffset(
            (
                _currentIndex
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ),
            viewportWidth,
            minExtent,
            maxExtent
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _scrollToCurrentIndex()
    {
        double offset = _tabCenteredScrollOffset(
            (
                _currentIndex
                ?? throw new global::System.NullReferenceException("A required value was null.")
            )
        );
        DartRuntimePrimitives.Ignore(
            _effectiveScrollController.animateTo(
                offset,
                duration: ConstantsLibrary.kTabScrollDuration,
                curve: Curves.ease
            )
        );
    }

    internal virtual void _scrollToControllerValue()
    {
        double? leadingPosition =
            (
                (
                    _currentIndex
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ) > 0L
            )
                ? _tabCenteredScrollOffset(
                    (
                        _currentIndex
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ) - 1L
                )
                : null;
        double middlePosition = _tabCenteredScrollOffset(
            (
                _currentIndex
                ?? throw new global::System.NullReferenceException("A required value was null.")
            )
        );
        double? trailingPosition =
            (
                (
                    _currentIndex
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ) < maxTabIndex
            )
                ? _tabCenteredScrollOffset(
                    (
                        _currentIndex
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ) + 1L
                )
                : null;
        double indexLocal = _controller!.index.toDouble();
        double valueLocal = _controller!.animation!.value;
        double offset = (valueLocal - indexLocal) switch
        {
            -1.0 => leadingPosition ?? middlePosition,
            1.0 => trailingPosition ?? middlePosition,
            0 => middlePosition,
            < 0L => (leadingPosition is null)
                ? middlePosition
                : (
                    DorotiUiLibrary.lerpDouble(
                        middlePosition,
                        (
                            leadingPosition
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        ),
                        indexLocal - valueLocal
                    )
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
            _ => (trailingPosition is null)
                ? middlePosition
                : (
                    DorotiUiLibrary.lerpDouble(
                        middlePosition,
                        (
                            trailingPosition
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        ),
                        valueLocal - indexLocal
                    )
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
        };
        _effectiveScrollController.jumpTo(offset);
    }

    internal virtual void _handleTabControllerAnimationTick()
    {
        DartRuntimePrimitives.Assert(() => mounted);
        if (!_controller!.indexIsChanging && widget.isScrollable)
        {
            _currentIndex = _controller!.index;
            _scrollToControllerValue();
        }
    }

    internal virtual void _handleTabControllerTick()
    {
        if (_controller!.index != _currentIndex)
        {
            _currentIndex = _controller!.index;
            if (widget.isScrollable)
            {
                _scrollToCurrentIndex();
            }
        }
        setState(() => { });
    }

    internal virtual void _saveTabOffsets(
        List<double> tabOffsets,
        TextDirection textDirection,
        double width
    )
    {
        _tabStripWidth = width;
        _indicatorPainter?.saveTabOffsets(tabOffsets, textDirection);
    }

    internal virtual void _handleTap(long index)
    {
        DartRuntimePrimitives.Assert(() => (index >= 0L) && (index < checked(widget.tabs.Count)));
        _controller!.animateTo(index);
        widget.onTap?.Invoke(index);
    }

    internal virtual Widget _buildStyledTab(
        Widget child,
        bool isSelected,
        Animation<double> animation,
        TabBarThemeData defaults
    )
    {
        return new _TabStyle__tabs(
            animation: animation,
            isSelected: isSelected,
            isPrimary: widget._isPrimary,
            labelColor: widget.labelColor,
            unselectedLabelColor: widget.unselectedLabelColor,
            labelStyle: widget.labelStyle,
            unselectedLabelStyle: widget.unselectedLabelStyle,
            defaults: defaults,
            child: child
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual bool _debugScheduleCheckHasValidTabsCount()
    {
        if (_debugHasScheduledValidTabsCountCheck)
        {
            return true;
        }
        WidgetsBinding.instance.addPostFrameCallback(
            (duration) =>
            {
                _debugHasScheduledValidTabsCountCheck = false;
                if (!mounted)
                {
                    return;
                }
                DartRuntimePrimitives.Assert(() =>
                {
                    if (_controller!.length != checked(widget.tabs.Count))
                    {
                        throw DartRuntimePrimitives.AsException(
                            FlutterError.Create(
                                $"Controller's length property ({_controller!.length}) does not match the "
                                    + $"number of tabs ({checked((long)widget.tabs.Count)}) present in TabBar's tabs property."
                            )
                        );
                    }
                    return true;
                });
            },
            debugLabel: "TabBar.tabsCountCheck"
        );
        _debugHasScheduledValidTabsCountCheck = true;
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual bool _debugTabAlignmentIsValid(TabAlignment tabAlignment)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (widget.isScrollable && Equals(tabAlignment, TabAlignment.fill))
            {
                throw DartRuntimePrimitives.AsException(
                    FlutterError.Create(
                        $"{tabAlignment} is only valid for non-scrollable tab bars."
                    )
                );
            }
            if (
                !widget.isScrollable
                && (
                    Equals(tabAlignment, TabAlignment.start)
                    || Equals(tabAlignment, TabAlignment.startOffset)
                )
            )
            {
                throw DartRuntimePrimitives.AsException(
                    FlutterError.Create($"{tabAlignment} is only valid for scrollable tab bars.")
                );
            }
            return true;
        });
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            DebugLibrary.debugCheckHasMaterialLocalizations(context)
        );
        DartRuntimePrimitives.Assert(() => _debugScheduleCheckHasValidTabsCount());
        ThemeData theme = Theme.of(context);
        TabBarThemeData tabBarTheme = TabBarTheme.of(context);
        TabAlignment effectiveTabAlignment =
            (widget.tabAlignment ?? tabBarTheme.tabAlignment)
            ?? (
                _defaults.tabAlignment
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        DartRuntimePrimitives.Assert(() => _debugTabAlignmentIsValid(effectiveTabAlignment));
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        if (_controller!.length == 0L)
        {
            return new LimitedBox(
                maxWidth: 0.0,
                child: new SizedBox(
                    width: double.PositiveInfinity,
                    height: TabsLibrary._kTabHeight + widget.indicatorWeight
                )
            );
        }
        var wrappedTabs = new List<Widget>(
            Enumerable.Select(
                Enumerable.Range(0, checked((int)checked((long)widget.tabs.Count))),
                (index) =>
                {
                    EdgeInsetsGeometry paddingLocal =
                        (widget.labelPadding ?? tabBarTheme.labelPadding)
                        ?? ConstantsLibrary.kTabLabelPadding;
                    double verticalAdjustment =
                        (TabsLibrary._kTextAndIconTabHeight - TabsLibrary._kTabHeight) / 2.0;
                    Widget tabLocal = widget.tabs[index];
                    if (
                        (tabLocal is PreferredSizeWidget)
                        && (
                            ((PreferredSizeWidget)tabLocal).preferredSize.height
                            == TabsLibrary._kTabHeight
                        )
                        && widget.tabHasTextAndIcon
                    )
                    {
                        PreferredSizeWidget tab__72645__as72681 = (PreferredSizeWidget)tabLocal;
                        paddingLocal = paddingLocal.add(
                            EdgeInsets.CreateSymmetric(vertical: verticalAdjustment)
                        );
                    }
                    _labelPaddings[index] = paddingLocal;
                    return new Center(
                        heightFactor: 1.0,
                        child: new Padding(
                            padding: _labelPaddings[index],
                            child: new KeyedSubtree(key: _tabKeys[index], child: widget.tabs[index])
                        )
                    );
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                }
            )
        );
        if (_controller is not null)
        {
            long previousIndexLocal = _controller!.previousIndex;
            if (_controller!.indexIsChanging)
            {
                DartRuntimePrimitives.Assert(() => _currentIndex != previousIndexLocal);
                Animation<double> animationLocal = new _ChangeAnimation__tabs(_controller!);
                wrappedTabs[
                    (int)(
                        _currentIndex
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                ] = _buildStyledTab(
                    wrappedTabs[
                        (int)(
                            _currentIndex
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        )
                    ],
                    true,
                    animationLocal,
                    _defaults
                );
                wrappedTabs[(int)previousIndexLocal] = _buildStyledTab(
                    wrappedTabs[(int)previousIndexLocal],
                    false,
                    animationLocal,
                    _defaults
                );
            }
            else
            {
                long tabIndexLocal = (
                    _currentIndex
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
                Animation<double> centerAnimation = new _DragAnimation__tabs(
                    _controller!,
                    tabIndexLocal
                );
                wrappedTabs[(int)tabIndexLocal] = _buildStyledTab(
                    wrappedTabs[(int)tabIndexLocal],
                    true,
                    centerAnimation,
                    _defaults
                );
                if (
                    (
                        _currentIndex
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ) > 0L
                )
                {
                    long tabIndexAlternate =
                        (
                            _currentIndex
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        ) - 1L;
                    Animation<double> previousAnimation = new ReverseAnimation(
                        new _DragAnimation__tabs(_controller!, tabIndexAlternate)
                    );
                    wrappedTabs[(int)tabIndexAlternate] = _buildStyledTab(
                        wrappedTabs[(int)tabIndexAlternate],
                        false,
                        previousAnimation,
                        _defaults
                    );
                }
                if (
                    (
                        _currentIndex
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ) < (checked(widget.tabs.Count) - 1L)
                )
                {
                    long tabIndexNested =
                        (
                            _currentIndex
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        ) + 1L;
                    Animation<double> nextAnimation = new ReverseAnimation(
                        new _DragAnimation__tabs(_controller!, tabIndexNested)
                    );
                    wrappedTabs[(int)tabIndexNested] = _buildStyledTab(
                        wrappedTabs[(int)tabIndexNested],
                        false,
                        nextAnimation,
                        _defaults
                    );
                }
            }
        }
        long tabCountLocal = checked(widget.tabs.Count);
        for (var indexLocal = 0L; indexLocal < tabCountLocal; indexLocal += 1L)
        {
            // Dart captures a separate loop variable per iteration; C# for loops do not.
            var tabIndex = indexLocal;
            var selectedState = (
                (Func<HashSet<WidgetState>>)(
                    () =>
                    {
                        var __collection75624 = new HashSet<WidgetState>();
                        if (indexLocal == _currentIndex)
                        {
                            __collection75624.Add(WidgetState.selected);
                        }
                        return __collection75624;
                    }
                )
            )();
            MouseCursor effectiveMouseCursor =
                (
                    WidgetStateProperty.resolveAs(widget.mouseCursor, selectedState)
                    ?? (tabBarTheme.mouseCursor?.resolve(selectedState))
                ) ?? WidgetStateMouseCursor.clickable.resolve(selectedState);
            WidgetStateProperty<Color?> defaultOverlay = WidgetStateProperty.resolveWith(
                (states) =>
                {
                    HashSet<WidgetState> effectiveStates = (
                        (Func<HashSet<WidgetState>>)(
                            () =>
                            {
                                var __cascade = selectedState.toSet();
                                __cascade.UnionWith(states);
                                return __cascade;
                            }
                        )
                    )();
                    return _defaults.overlayColor?.resolve(effectiveStates);
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                }
            );
            wrappedTabs[(int)indexLocal] = DartRuntimePrimitives.ConvertValue<Widget>(
                new InkWell(
                    mouseCursor: effectiveMouseCursor,
                    onTap: () =>
                    {
                        _handleTap(tabIndex);
                    },
                    onHover: (value) =>
                    {
                        widget.onHover?.Invoke(value, tabIndex);
                    },
                    onFocusChange: (value) =>
                    {
                        widget.onFocusChange?.Invoke(value, tabIndex);
                    },
                    enableFeedback: widget.enableFeedback ?? true,
                    overlayColor: (widget.overlayColor ?? tabBarTheme.overlayColor)
                        ?? defaultOverlay,
                    splashFactory: (widget.splashFactory ?? tabBarTheme.splashFactory)
                        ?? _defaults.splashFactory,
                    borderRadius: (widget.splashBorderRadius ?? tabBarTheme.splashBorderRadius)
                        ?? _defaults.splashBorderRadius,
                    child: new Padding(
                        padding: EdgeInsets.CreateOnly(bottom: widget.indicatorWeight),
                        child: new Widgets.Semantics(
                            role: SemanticsRole.tab,
                            child: new Stack(
                                children: new List<Widget>
                                {
                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                        wrappedTabs[(int)indexLocal]
                                    ),
                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                        new Widgets.Semantics(
                                            selected: indexLocal == _currentIndex,
                                            label: Foundation.ConstantsLibrary.kIsWeb
                                                ? null
                                                : localizations.tabLabel(
                                                    tabIndex: indexLocal + 1L,
                                                    tabCount: tabCountLocal
                                                )
                                        )
                                    ),
                                }
                            )
                        )
                    )
                )
            );
            wrappedTabs[(int)indexLocal] = DartRuntimePrimitives.ConvertValue<Widget>(
                new MergeSemantics(child: wrappedTabs[(int)indexLocal])
            );
            if (!widget.isScrollable && Equals(effectiveTabAlignment, TabAlignment.fill))
            {
                wrappedTabs[(int)indexLocal] = DartRuntimePrimitives.ConvertValue<Widget>(
                    new Expanded(child: wrappedTabs[(int)indexLocal])
                );
            }
        }
        Widget tabBarLocal = new Widgets.Semantics(
            role: SemanticsRole.tabBar,
            container: true,
            explicitChildNodes: true,
            child: new CustomPaint(
                painter: _indicatorPainter,
                child: new _TabStyle__tabs(
                    animation: AnimationsLibrary.kAlwaysDismissedAnimation,
                    isSelected: false,
                    isPrimary: widget._isPrimary,
                    labelColor: widget.labelColor,
                    unselectedLabelColor: widget.unselectedLabelColor,
                    labelStyle: widget.labelStyle,
                    unselectedLabelStyle: widget.unselectedLabelStyle,
                    defaults: _defaults,
                    child: new _TabLabelBar__tabs(
                        onPerformLayout: _saveTabOffsets,
                        mainAxisSize: Equals(effectiveTabAlignment, TabAlignment.fill)
                            ? MainAxisSize.max
                            : MainAxisSize.min,
                        children: wrappedTabs
                    )
                )
            )
        );
        if (widget.isScrollable)
        {
            EdgeInsetsGeometry? effectivePadding = Equals(
                effectiveTabAlignment,
                TabAlignment.startOffset
            )
                ? EdgeInsetsDirectional
                    .CreateOnly(start: TabsLibrary._kStartOffset)
                    .add(widget.padding ?? EdgeInsets.zero)
                : widget.padding;
            tabBarLocal = DartRuntimePrimitives.ConvertValue<Widget>(
                new ScrollConfiguration(
                    behavior: ScrollConfiguration.of(context).copyWith(overscroll: false),
                    child: new SingleChildScrollView(
                        dragStartBehavior: widget.dragStartBehavior,
                        scrollDirection: Axis.horizontal,
                        controller: _effectiveScrollController,
                        padding: effectivePadding,
                        physics: widget.physics,
                        child: tabBarLocal
                    )
                )
            );
            {
                AlignmentGeometry effectiveAlignment = effectiveTabAlignment switch
                {
                    TabAlignment.center => DartRuntimePrimitives.ConvertValue<AlignmentGeometry>(
                        Alignment.center
                    ),
                    TabAlignment.start or TabAlignment.startOffset =>
                        DartRuntimePrimitives.ConvertValue<AlignmentGeometry>(
                            AlignmentDirectional.centerStart
                        ),
                    TabAlignment.fill => DartRuntimePrimitives.ConvertValue<AlignmentGeometry>(
                        AlignmentDirectional.centerStart
                    ),
                    _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                        throw new InvalidOperationException(
                            "Switch expression did not handle the supplied value."
                        ),
                };
                Color dividerColorLocal =
                    (widget.dividerColor ?? tabBarTheme.dividerColor) ?? _defaults.dividerColor!;
                double dividerHeightLocal =
                    (widget.dividerHeight ?? tabBarTheme.dividerHeight)
                    ?? (
                        _defaults.dividerHeight
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    );
                tabBarLocal = DartRuntimePrimitives.ConvertValue<Widget>(
                    new Align(
                        heightFactor: 1.0,
                        widthFactor: (dividerHeightLocal > 0L) ? null : 1.0,
                        alignment: effectiveAlignment,
                        child: tabBarLocal
                    )
                );
                if ((!Equals(dividerColorLocal, Colors.transparent)) && (dividerHeightLocal > 0L))
                {
                    tabBarLocal = DartRuntimePrimitives.ConvertValue<Widget>(
                        new CustomPaint(
                            painter: new _DividerPainter__tabs(
                                dividerColor: dividerColorLocal,
                                dividerHeight: dividerHeightLocal
                            ),
                            child: tabBarLocal
                        )
                    );
                }
            }
        }
        else
        {
            if (widget.padding is not null)
            {
                tabBarLocal = DartRuntimePrimitives.ConvertValue<Widget>(
                    new Padding(padding: widget.padding!, child: tabBarLocal)
                );
            }
        }
        return new Material(
            type: MaterialType.transparency,
            child: new MediaQuery(
                data: MediaQuery
                    .of(context)
                    .copyWith(textScaler: widget.textScaler ?? tabBarTheme.textScaler),
                child: tabBarLocal
            )
        );
    }
}

public class TabBarView : StatefulWidget
{
    public virtual TabController? controller { get; private set; }
    public virtual List<Widget> children { get; private set; } = default!;
    public virtual ScrollPhysics? physics { get; private set; }
    public virtual Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual double viewportFraction { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;

    public TabBarView(
        Key? key = null,
        List<Widget> children = default!,
        TabController? controller = null,
        ScrollPhysics? physics = null,
        Gestures.DragStartBehavior dragStartBehavior = Gestures.DragStartBehavior.start,
        double viewportFraction = 1.0,
        Clip clipBehavior = Clip.hardEdge
    )
        : base(key: key)
    {
        this.children = children;
        this.controller = controller;
        this.physics = physics;
        this.dragStartBehavior = dragStartBehavior;
        this.viewportFraction = viewportFraction;
        this.clipBehavior = clipBehavior;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _TabBarViewState__tabs());
}

internal class _TabBarViewState__tabs : State<TabBarView>
{
    internal virtual TabController? _controller { get; set; } = default;
    internal virtual PageController? _pageController { get; set; } = default;
    internal virtual List<Widget> _childrenWithKey { get; set; } = default!;
    internal virtual long? _currentIndex { get; set; } = default;
    internal virtual long _warpUnderwayCount { get; set; } = 0L;
    internal virtual long _scrollUnderwayCount { get; set; } = 0L;
    internal virtual bool _debugHasScheduledValidChildrenCountCheck { get; set; } = false;

    internal virtual bool _controllerIsValid =>
        DartRuntimePrimitives.ConvertValue<bool>(_controller?.animation is not null);

    internal virtual void _updateTabController()
    {
        TabController? newController = widget.controller ?? DefaultTabController.maybeOf(context);
        DartRuntimePrimitives.Assert(() =>
        {
            if (newController is null)
            {
                throw DartRuntimePrimitives.AsException(
                    FlutterError.Create(
                        $"No TabController for {DartRuntimePrimitives.RuntimeType(widget)}.\n"
                            + $"When creating a {DartRuntimePrimitives.RuntimeType(widget)}, you must either provide an explicit "
                            + "TabController using the \"controller\" property, or you must ensure that there "
                            + $"is a DefaultTabController above the {DartRuntimePrimitives.RuntimeType(widget)}.\n"
                            + "In this case, there was neither an explicit controller nor a default controller."
                    )
                );
            }
            return true;
        });
        if (Equals(newController, _controller))
        {
            return;
        }
        if (_controllerIsValid)
        {
            _controller!.animation!.removeListener(_handleTabControllerAnimationTick);
        }
        _controller = newController;
        if (_controller is not null)
        {
            _controller!.animation!.addListener(_handleTabControllerAnimationTick);
        }
    }

    internal virtual void _jumpToPage(long page)
    {
        _warpUnderwayCount += 1L;
        _pageController!.jumpToPage(page);
        _warpUnderwayCount -= 1L;
    }

    internal virtual async Future _animateToPage(long page, Duration duration, Curve curve)
    {
        _warpUnderwayCount += 1L;
        await _pageController!.animateToPage(page, duration: duration, curve: curve);
        _warpUnderwayCount -= 1L;
    }

    public override void initState()
    {
        base.initState();
        _updateChildren();
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _updateTabController();
        _currentIndex = _controller!.index;
        if (_pageController is null)
        {
            _pageController = new PageController(
                initialPage: (
                    _currentIndex
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                viewportFraction: widget.viewportFraction
            );
        }
        else
        {
            _pageController!.jumpToPage(
                (
                    _currentIndex
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
            );
        }
    }

    public override void didUpdateWidget(TabBarView oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(widget.controller, oldWidget.controller))
        {
            _updateTabController();
            _currentIndex = _controller!.index;
            _jumpToPage(
                (
                    _currentIndex
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
            );
        }
        if (widget.viewportFraction != oldWidget.viewportFraction)
        {
            _pageController?.dispose();
            _pageController = new PageController(
                initialPage: (
                    _currentIndex
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                viewportFraction: widget.viewportFraction
            );
        }
        if ((!Equals(widget.children, oldWidget.children)) && (_warpUnderwayCount == 0L))
        {
            _updateChildren();
        }
    }

    public override void dispose()
    {
        if (_controllerIsValid)
        {
            _controller!.animation!.removeListener(_handleTabControllerAnimationTick);
        }
        _controller = null;
        _pageController?.dispose();
        base.dispose();
    }

    internal virtual void _updateChildren()
    {
        _childrenWithKey = KeyedSubtree.ensureUniqueKeysForList(
            widget
                .children.map<Widget, Widget>(
                    (child) =>
                    {
                        return new Widgets.Semantics(role: SemanticsRole.tabPanel, child: child);
                        throw new InvalidOperationException(
                            "Callback completed without returning a value."
                        );
                    }
                )
                .ToList()
        );
    }

    internal virtual void _handleTabControllerAnimationTick()
    {
        if ((_scrollUnderwayCount > 0L) || !_controller!.indexIsChanging)
        {
            return;
        }
        if (_controller!.index != _currentIndex)
        {
            _currentIndex = _controller!.index;
            _warpToCurrentIndex();
        }
    }

    internal virtual void _warpToCurrentIndex()
    {
        if (
            !mounted
            || (
                _pageController!.page
                == (
                    _currentIndex
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ).toDouble()
            )
        )
        {
            return;
        }
        var adjacentDestination =
            (
                (
                    _currentIndex
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ) - _controller!.previousIndex
            ).abs() == 1L;
        if (adjacentDestination)
        {
            DartRuntimePrimitives.Ignore(_warpToAdjacentTab(_controller!.animationDuration));
        }
        else
        {
            DartRuntimePrimitives.Ignore(_warpToNonAdjacentTab(_controller!.animationDuration));
        }
    }

    internal virtual async Future _warpToAdjacentTab(Duration duration)
    {
        if (Equals(duration, Duration.zero))
        {
            _jumpToPage(
                (
                    _currentIndex
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
            );
        }
        else
        {
            await _animateToPage(
                (
                    _currentIndex
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                duration: duration,
                curve: Curves.ease
            );
        }
        if (mounted)
        {
            setState(() =>
            {
                _updateChildren();
            });
        }
        await Future.value();
        return;
    }

    internal virtual async Future _warpToNonAdjacentTab(Duration duration)
    {
        long previousIndexLocal = _controller!.previousIndex;
        DartRuntimePrimitives.Assert(() =>
            (
                (
                    _currentIndex
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ) - previousIndexLocal
            ).abs() > 1L
        );
        long initialPage =
            (
                (
                    _currentIndex
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ) > previousIndexLocal
            )
                ? (
                    (
                        _currentIndex
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ) - 1L
                )
                : (
                    (
                        _currentIndex
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ) + 1L
                );
        setState(() =>
        {
            _childrenWithKey = new List<Widget>(_childrenWithKey);
            Widget temp = _childrenWithKey[(int)initialPage];
            _childrenWithKey[(int)initialPage] = _childrenWithKey[(int)previousIndexLocal];
            _childrenWithKey[(int)previousIndexLocal] = temp;
        });
        _jumpToPage(initialPage);
        if (Equals(duration, Duration.zero))
        {
            _jumpToPage(
                (
                    _currentIndex
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
            );
        }
        else
        {
            await _animateToPage(
                (
                    _currentIndex
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                duration: duration,
                curve: Curves.ease
            );
        }
        if (mounted)
        {
            setState(() =>
            {
                _updateChildren();
            });
        }
    }

    internal virtual void _syncControllerOffset()
    {
        _controller!.offset = DorotiUiLibrary.clampDouble(
            (
                _pageController!.page
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ) - _controller!.index,
            -1.0,
            1.0
        );
    }

    internal virtual bool _handleScrollNotification(ScrollNotification notification)
    {
        if ((_warpUnderwayCount > 0L) || (_scrollUnderwayCount > 0L))
        {
            return false;
        }
        if (notification.depth != 0L)
        {
            return false;
        }
        if (!_controllerIsValid)
        {
            return false;
        }
        _scrollUnderwayCount += 1L;
        double pageLocal = (
            _pageController!.page
            ?? throw new global::System.NullReferenceException("A required value was null.")
        );
        if ((notification is ScrollUpdateNotification) && !_controller!.indexIsChanging)
        {
            ScrollUpdateNotification notification__as89692 = (ScrollUpdateNotification)notification;
            bool pageChanged = (pageLocal - _controller!.index).abs() > 1.0;
            if (pageChanged)
            {
                _controller!.index = pageLocal.round();
                _currentIndex = _controller!.index;
            }
            _syncControllerOffset();
        }
        else
        {
            if (notification is ScrollEndNotification)
            {
                ScrollEndNotification notification__as90007 = (ScrollEndNotification)notification;
                _controller!.index = pageLocal.round();
                _currentIndex = _controller!.index;
                if (!_controller!.indexIsChanging)
                {
                    _syncControllerOffset();
                }
            }
        }
        _scrollUnderwayCount -= 1L;
        return false;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual bool _debugScheduleCheckHasValidChildrenCount()
    {
        if (_debugHasScheduledValidChildrenCountCheck)
        {
            return true;
        }
        WidgetsBinding.instance.addPostFrameCallback(
            (duration) =>
            {
                _debugHasScheduledValidChildrenCountCheck = false;
                if (!mounted)
                {
                    return;
                }
                DartRuntimePrimitives.Assert(() =>
                {
                    if (_controller!.length != checked(widget.children.Count))
                    {
                        throw DartRuntimePrimitives.AsException(
                            FlutterError.Create(
                                $"Controller's length property ({_controller!.length}) does not match the "
                                    + $"number of children ({checked((long)widget.children.Count)}) present in TabBarView's children property."
                            )
                        );
                    }
                    return true;
                });
            },
            debugLabel: "TabBarView.validChildrenCountCheck"
        );
        _debugHasScheduledValidChildrenCountCheck = true;
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => _debugScheduleCheckHasValidChildrenCount());
        return new NotificationListener<ScrollNotification>(
            onNotification: _handleScrollNotification,
            child: new PageView(
                dragStartBehavior: widget.dragStartBehavior,
                clipBehavior: widget.clipBehavior,
                controller: _pageController,
                physics: (widget.physics is null)
                    ? new PageScrollPhysics().applyTo(new ClampingScrollPhysics())
                    : new PageScrollPhysics().applyTo(widget.physics),
                children: _childrenWithKey
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class TabPageSelectorIndicator : StatelessWidget
{
    public virtual Color backgroundColor { get; private set; } = default!;
    public virtual Color borderColor { get; private set; } = default!;
    public virtual double size { get; private set; } = default!;
    public virtual BorderStyle borderStyle { get; private set; } = default!;

    public TabPageSelectorIndicator(
        Key? key = null,
        Color backgroundColor = default!,
        Color borderColor = default!,
        double size = default!,
        BorderStyle borderStyle = BorderStyle.solid
    )
        : base(key: key)
    {
        this.backgroundColor = backgroundColor;
        this.borderColor = borderColor;
        this.size = size;
        this.borderStyle = borderStyle;
    }

    public override Widget build(BuildContext context)
    {
        return new Container(
            width: size,
            height: size,
            margin: EdgeInsets.CreateAll(4.0),
            decoration: new BoxDecoration(
                color: backgroundColor,
                border: Border.CreateAll(color: borderColor, style: borderStyle),
                shape: BoxShape.circle
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class TabPageSelector : StatefulWidget
{
    public virtual TabController? controller { get; private set; }
    public virtual double indicatorSize { get; private set; } = default!;
    public virtual Color? color { get; private set; }
    public virtual Color? selectedColor { get; private set; }
    public virtual BorderStyle? borderStyle { get; private set; }

    public TabPageSelector(
        Key? key = null,
        TabController? controller = null,
        double indicatorSize = 12.0,
        Color? color = null,
        Color? selectedColor = null,
        BorderStyle? borderStyle = null
    )
        : base(key: key)
    {
        this.controller = controller;
        this.indicatorSize = indicatorSize;
        this.color = color;
        this.selectedColor = selectedColor;
        this.borderStyle = borderStyle;
        System.Diagnostics.Debug.Assert(indicatorSize > 0.0);
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _TabPageSelectorState__tabs());
}

internal class _TabPageSelectorState__tabs : State<TabPageSelector>
{
    internal virtual TabController? _previousTabController { get; set; } = default;
    internal virtual CurvedAnimation? _animation { get; set; } = default;

    internal virtual TabController _tabController
    {
        get
        {
            TabController? tabController =
                widget.controller ?? DefaultTabController.maybeOf(context);
            DartRuntimePrimitives.Assert(() =>
            {
                if (tabController is null)
                {
                    throw DartRuntimePrimitives.AsException(
                        FlutterError.Create(
                            $"No TabController for {GetType()}.\n"
                                + $"When creating a {GetType()}, you must either provide an explicit TabController "
                                + "using the \"controller\" property, or you must ensure that there is a "
                                + $"DefaultTabController above the {GetType()}.\n"
                                + "In this case, there was neither an explicit controller nor a default controller."
                        )
                    );
                }
                return true;
            });
            return tabController!;
        }
    }

    public override void didUpdateWidget(TabPageSelector oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(_previousTabController?.animation, _tabController.animation))
        {
            _setAnimation();
        }
        if (!Equals(_previousTabController, _tabController))
        {
            _previousTabController = _tabController;
        }
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        if (
            (_animation is null)
            || (!Equals(_previousTabController?.animation, _tabController.animation))
        )
        {
            _setAnimation();
        }
        if (!Equals(_previousTabController, _tabController))
        {
            _previousTabController = _tabController;
        }
    }

    internal virtual void _setAnimation()
    {
        _animation?.dispose();
        _animation = new CurvedAnimation(
            parent: _tabController.animation!,
            curve: Curves.fastOutSlowIn
        );
    }

    public override void dispose()
    {
        _animation?.dispose();
        base.dispose();
    }

    internal virtual Widget _buildTabIndicator(
        long tabIndex,
        TabController tabController,
        ColorTween selectedColorTween,
        ColorTween previousColorTween
    )
    {
        Color background = default!;
        if (tabController.indexIsChanging)
        {
            double t = 1.0 - TabsLibrary._indexChangeProgress(tabController);
            if (tabController.index == tabIndex)
            {
                background = selectedColorTween.lerp(t)!;
            }
            else
            {
                if (tabController.previousIndex == tabIndex)
                {
                    background = previousColorTween.lerp(t)!;
                }
                else
                {
                    background = selectedColorTween.begin!;
                }
            }
        }
        else
        {
            double offsetLocal = tabController.offset;
            if (tabController.index == tabIndex)
            {
                background = selectedColorTween.lerp(1.0 - offsetLocal.abs())!;
            }
            else
            {
                if ((tabController.index == (tabIndex - 1L)) && (offsetLocal > 0.0))
                {
                    background = selectedColorTween.lerp(offsetLocal)!;
                }
                else
                {
                    if ((tabController.index == (tabIndex + 1L)) && (offsetLocal < 0.0))
                    {
                        background = selectedColorTween.lerp(-offsetLocal)!;
                    }
                    else
                    {
                        background = selectedColorTween.begin!;
                    }
                }
            }
        }
        return new TabPageSelectorIndicator(
            backgroundColor: background,
            borderColor: selectedColorTween.end!,
            size: widget.indicatorSize,
            borderStyle: widget.borderStyle ?? BorderStyle.solid
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget build(BuildContext context)
    {
        Color fixColor = widget.color ?? Colors.transparent;
        Color fixSelectedColor = widget.selectedColor ?? Theme.of(context).colorScheme.secondary;
        var selectedColorTween = new ColorTween(begin: fixColor, end: fixSelectedColor);
        var previousColorTween = new ColorTween(begin: fixSelectedColor, end: fixColor);
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        return new AnimatedBuilder(
            animation: _animation!,
            builder: (context, child) =>
            {
                return new Widgets.Semantics(
                    label: localizations.tabLabel(
                        tabIndex: _tabController.index + 1L,
                        tabCount: _tabController.length
                    ),
                    child: new Row(
                        mainAxisSize: MainAxisSize.min,
                        children: new List<Widget>(
                            Enumerable.Select(
                                Enumerable.Range(0, checked((int)_tabController.length)),
                                (tabIndex) =>
                                {
                                    return _buildTabIndicator(
                                        tabIndex,
                                        _tabController,
                                        selectedColorTween,
                                        previousColorTween
                                    );
                                    throw new InvalidOperationException(
                                        "Callback completed without returning a value."
                                    );
                                }
                            )
                        ).ToList()
                    )
                );
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _TabsPrimaryDefaultsM3__tabs : TabBarThemeData
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
    public virtual bool isScrollable { get; private set; } = default!;
    public static EdgeInsetsGeometry iconMargin = EdgeInsets.CreateOnly(bottom: 2);

    internal _TabsPrimaryDefaultsM3__tabs(BuildContext context, bool isScrollable)
        : base(indicatorSize: TabBarIndicatorSize.label)
    {
        this.context = context;
        this.isScrollable = isScrollable;
    }

    public override Color? dividerColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.outlineVariant);
    public override double? dividerHeight => 1.0;
    public override Color? indicatorColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.primary);
    public override Color? labelColor => DartRuntimePrimitives.ConvertValue<Color>(_colors.primary);
    public override TextStyle? labelStyle => _textTheme.titleSmall;
    public override Color? unselectedLabelColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.onSurfaceVariant);
    public override TextStyle? unselectedLabelStyle => _textTheme.titleSmall;
    public override WidgetStateProperty<Color?> overlayColor
    {
        get
        {
            return WidgetStateProperty.resolveWith(
                (states) =>
                {
                    if (states.Contains(WidgetState.selected))
                    {
                        if (states.Contains(WidgetState.pressed))
                        {
                            return _colors.primary.withOpacity(0.1);
                        }
                        if (states.Contains(WidgetState.hovered))
                        {
                            return _colors.primary.withOpacity(0.08);
                        }
                        if (states.Contains(WidgetState.focused))
                        {
                            return _colors.primary.withOpacity(0.1);
                        }
                        return null;
                    }
                    if (states.Contains(WidgetState.pressed))
                    {
                        return _colors.primary.withOpacity(0.1);
                    }
                    if (states.Contains(WidgetState.hovered))
                    {
                        return _colors.onSurface.withOpacity(0.08);
                    }
                    if (states.Contains(WidgetState.focused))
                    {
                        return _colors.onSurface.withOpacity(0.1);
                    }
                    return null;
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                }
            );
        }
    }
    public override InteractiveInkFeatureFactory? splashFactory => Theme.of(context).splashFactory;
    public override TabAlignment? tabAlignment =>
        isScrollable ? TabAlignment.startOffset : TabAlignment.fill;

    public static double indicatorWeight(TabBarIndicatorSize indicatorSize)
    {
        return indicatorSize switch
        {
            TabBarIndicatorSize.label => 3.0,
            TabBarIndicatorSize.tab => 2.0,
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _TabsSecondaryDefaultsM3__tabs : TabBarThemeData
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
    public virtual bool isScrollable { get; private set; } = default!;
    public static double indicatorWeight = 2.0;

    internal _TabsSecondaryDefaultsM3__tabs(BuildContext context, bool isScrollable)
        : base(indicatorSize: TabBarIndicatorSize.tab)
    {
        this.context = context;
        this.isScrollable = isScrollable;
    }

    public override Color? dividerColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.outlineVariant);
    public override double? dividerHeight => 1.0;
    public override Color? indicatorColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.primary);
    public override Color? labelColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.onSurface);
    public override TextStyle? labelStyle => _textTheme.titleSmall;
    public override Color? unselectedLabelColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.onSurfaceVariant);
    public override TextStyle? unselectedLabelStyle => _textTheme.titleSmall;
    public override WidgetStateProperty<Color?> overlayColor
    {
        get
        {
            return WidgetStateProperty.resolveWith(
                (states) =>
                {
                    if (states.Contains(WidgetState.selected))
                    {
                        if (states.Contains(WidgetState.pressed))
                        {
                            return _colors.onSurface.withOpacity(0.1);
                        }
                        if (states.Contains(WidgetState.hovered))
                        {
                            return _colors.onSurface.withOpacity(0.08);
                        }
                        if (states.Contains(WidgetState.focused))
                        {
                            return _colors.onSurface.withOpacity(0.1);
                        }
                        return null;
                    }
                    if (states.Contains(WidgetState.pressed))
                    {
                        return _colors.onSurface.withOpacity(0.1);
                    }
                    if (states.Contains(WidgetState.hovered))
                    {
                        return _colors.onSurface.withOpacity(0.08);
                    }
                    if (states.Contains(WidgetState.focused))
                    {
                        return _colors.onSurface.withOpacity(0.1);
                    }
                    return null;
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                }
            );
        }
    }
    public override InteractiveInkFeatureFactory? splashFactory => Theme.of(context).splashFactory;
    public override TabAlignment? tabAlignment =>
        isScrollable ? TabAlignment.startOffset : TabAlignment.fill;
}
