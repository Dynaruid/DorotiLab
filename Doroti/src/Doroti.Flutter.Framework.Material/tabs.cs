// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/tabs.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

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
    label
}

public enum TabAlignment
{
    start,
    startOffset,
    fill,
    center
}

public enum TabIndicatorAnimation
{
    linear,
    elastic
}

public class Tab : global::Doroti.Generated.Framework.Widgets.StatelessWidget, global::Doroti.Generated.Framework.Widgets.PreferredSizeWidget
{
    public virtual string? text { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? child { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? icon { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? iconMargin { get; private set; }
    public virtual double? height { get; private set; }

    public Tab(global::Doroti.Generated.Framework.Foundation.Key? key = null, string? text = null, global::Doroti.Generated.Framework.Widgets.Widget? icon = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? iconMargin = null, double? height = null, global::Doroti.Generated.Framework.Widgets.Widget? child = null) : base(key: key)
    {
        this.text = text;
        this.icon = icon;
        this.iconMargin = iconMargin;
        this.height = height;
        this.child = child;
        System.Diagnostics.Debug.Assert((((text is not null) || (child is not null)) || (icon is not null)));
        System.Diagnostics.Debug.Assert(((text is null) || (child is null)));
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildLabelText()
    {
        return (this.child ?? new global::Doroti.Generated.Framework.Widgets.Text(this.text!, softWrap: false, overflow: global::Doroti.Generated.Framework.Painting.TextOverflow.fade));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        double calculatedHeight__6389 = default!;
        global::Doroti.Generated.Framework.Widgets.Widget label__6424 = default!;
        if ((this.icon is null))
        {
            calculatedHeight__6389 = TabsLibrary._kTabHeight;
            label__6424 = _buildLabelText();
        }
        else
        {
            if (((this.text is null) && (this.child is null)))
            {
                calculatedHeight__6389 = TabsLibrary._kTabHeight;
                label__6424 = this.icon!;
            }
            else
            {
                calculatedHeight__6389 = TabsLibrary._kTextAndIconTabHeight;
                global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry effectiveIconMargin__6726 = (this.iconMargin ?? ((Theme.of(context).useMaterial3 ? _TabsPrimaryDefaultsM3__tabs.iconMargin : _TabsDefaultsM2__tabs.iconMargin)));
                label__6424 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Column(mainAxisAlignment: global::Doroti.Generated.Framework.Rendering.MainAxisAlignment.center, children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Padding(padding: effectiveIconMargin__6726, child: this.icon)), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(_buildLabelText()) }));
            }
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.SizedBox(height: (this.height ?? calculatedHeight__6389), child: new global::Doroti.Generated.Framework.Widgets.Center(widthFactor: 1.0, child: label__6424)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.StringProperty("text", this.text, defaultValue: null));
    }

    public virtual Size preferredSize
    {
        get
        {
            if ((this.height is not null))
            {
                double height__value7504 = DartRuntimePrimitives.RequireValue(height);
                return new global::Doroti.Flutter.Ui.Size(DartRuntimePrimitives.RequireValue(this.height));
            }
            else
            {
                if (((((this.text is not null) || (this.child is not null))) && (this.icon is not null)))
                {
                    return new global::Doroti.Flutter.Ui.Size(TabsLibrary._kTextAndIconTabHeight);
                }
                else
                {
                    return new global::Doroti.Flutter.Ui.Size(TabsLibrary._kTabHeight);
                }
            }
            return default!;
        }
    }
}

internal class _TabStyle__tabs : global::Doroti.Generated.Framework.Widgets.AnimatedWidget
{
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? labelStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? unselectedLabelStyle { get; private set; }
    public virtual bool isSelected { get; private set; } = default!;
    public virtual bool isPrimary { get; private set; } = default!;
    public virtual Color? labelColor { get; private set; }
    public virtual Color? unselectedLabelColor { get; private set; }
    public virtual TabBarThemeData defaults { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;

    internal _TabStyle__tabs(global::Doroti.Generated.Framework.Animation.Animation<double> animation, bool isSelected, bool isPrimary, Color? labelColor, Color? unselectedLabelColor, global::Doroti.Generated.Framework.Painting.TextStyle? labelStyle, global::Doroti.Generated.Framework.Painting.TextStyle? unselectedLabelStyle, TabBarThemeData defaults, global::Doroti.Generated.Framework.Widgets.Widget child) : base(listenable: animation)
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

    internal virtual global::Doroti.Generated.Framework.Widgets.WidgetStateColor _resolveWithLabelColor(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.IconThemeData? iconTheme = null)
    {
        ThemeData themeData__8511 = Theme.of(context);
        TabBarThemeData tabBarTheme__8568 = TabBarTheme.of(context);
        var animation__8617 = ((global::Doroti.Generated.Framework.Animation.Animation<double>?)(object?)this.listenable)!;
        global::Doroti.Flutter.Ui.Color selectedColor__8913 = ((global::Doroti.Flutter.Ui.Color)(object?)((((this.labelColor ?? tabBarTheme__8568.labelColor) ?? this.labelStyle?.color) ?? tabBarTheme__8568.labelStyle?.color) ?? this.defaults.labelColor!));
        global::Doroti.Flutter.Ui.Color unselectedColor__9102 = default!;
        if ((selectedColor__8913 is global::Doroti.Generated.Framework.Widgets.WidgetStateColor))
        {
            global::Doroti.Generated.Framework.Widgets.WidgetStateColor selectedColor__8913__as9128 = (global::Doroti.Generated.Framework.Widgets.WidgetStateColor)selectedColor__8913;
            unselectedColor__9102 = ((global::Doroti.Generated.Framework.Widgets.WidgetStateColor)selectedColor__8913__as9128).resolve(new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>());
            selectedColor__8913 = ((global::Doroti.Generated.Framework.Widgets.WidgetStateColor)selectedColor__8913__as9128).resolve(new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> { global::Doroti.Generated.Framework.Widgets.WidgetState.selected });
        }
        else
        {
            unselectedColor__9102 = (((((this.unselectedLabelColor ?? tabBarTheme__8568.unselectedLabelColor) ?? this.unselectedLabelStyle?.color) ?? tabBarTheme__8568.unselectedLabelStyle?.color) ?? iconTheme?.color) ?? ((themeData__8511.useMaterial3 ? this.defaults.unselectedLabelColor! : selectedColor__8913.withAlpha(178L))));
        }
        return global::Doroti.Generated.Framework.Widgets.WidgetStateColor.CreateResolveWith(((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    return Dart_uiLibrary.Color.lerp(selectedColor__8913, unselectedColor__9102, ((global::Doroti.Generated.Framework.Animation.Animation<double>)animation__8617).value)!;
}
return Dart_uiLibrary.Color.lerp(unselectedColor__9102, selectedColor__8913, ((global::Doroti.Generated.Framework.Animation.Animation<double>)animation__8617).value)!;
throw new InvalidOperationException("Dart closure completed without a value.");
}));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ThemeData theme__10203 = Theme.of(context);
        TabBarThemeData tabBarTheme__10256 = TabBarTheme.of(context);
        var animation__10305 = ((global::Doroti.Generated.Framework.Animation.Animation<double>?)(object?)this.listenable)!;
        var states__10361 = (this.isSelected ? new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> { global::Doroti.Generated.Framework.Widgets.WidgetState.selected } : new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>());
        global::Doroti.Generated.Framework.Painting.TextStyle selectedStyle__10619 = ((global::Doroti.Generated.Framework.Painting.TextStyle)(object?)this.defaults.labelStyle!.merge((this.labelStyle ?? tabBarTheme__10256.labelStyle)).copyWith(inherit: true));
        global::Doroti.Generated.Framework.Painting.TextStyle unselectedStyle__10763 = ((global::Doroti.Generated.Framework.Painting.TextStyle)(object?)this.defaults.unselectedLabelStyle!.merge(((this.unselectedLabelStyle ?? tabBarTheme__10256.unselectedLabelStyle) ?? this.labelStyle)).copyWith(inherit: true));
        global::Doroti.Generated.Framework.Painting.TextStyle textStyle__10953 = (this.isSelected ? TextStyle.lerp(selectedStyle__10619, unselectedStyle__10763, ((global::Doroti.Generated.Framework.Animation.Animation<double>)animation__10305).value)! : TextStyle.lerp(unselectedStyle__10763, selectedStyle__10619, ((global::Doroti.Generated.Framework.Animation.Animation<double>)animation__10305).value)!);
        global::Doroti.Flutter.Ui.Color defaultIconColor__11143 = ((global::Doroti.Flutter.Ui.Color)(object?)(theme__10203.colorScheme.brightness switch { Brightness.light => ConstantsLibrary.kDefaultIconDarkColor, Brightness.dark => ConstantsLibrary.kDefaultIconLightColor, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        global::Doroti.Generated.Framework.Widgets.IconThemeData? customIconTheme__11332 = (IconTheme.of(context) switch { global::Doroti.Generated.Framework.Widgets.IconThemeData iconTheme__11409 when ((!object.Equals(((global::Doroti.Generated.Framework.Widgets.IconThemeData)iconTheme__11409).color, defaultIconColor__11143))) => iconTheme__11409, _ => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.IconThemeData>(null) });
        global::Doroti.Flutter.Ui.Color iconColor__11514 = ((global::Doroti.Flutter.Ui.Color)(object?)_resolveWithLabelColor(context, iconTheme: customIconTheme__11332).resolve(states__10361));
        global::Doroti.Flutter.Ui.Color labelColor__11638 = ((global::Doroti.Flutter.Ui.Color)(object?)_resolveWithLabelColor(context).resolve(states__10361));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: textStyle__10953.copyWith(color: labelColor__11638), child: IconTheme.merge(data: new global::Doroti.Generated.Framework.Widgets.IconThemeData(size: (customIconTheme__11332?.size ?? 24.0), color: iconColor__11514), child: this.child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal delegate void _LayoutCallback__tabs(List<double> xOffsets, TextDirection textDirection, double width);

public class _TabLabelBarRenderer__tabs : global::Doroti.Generated.Framework.Rendering.RenderFlex
{
    public virtual global::System.Action<List<double>, TextDirection, double> onPerformLayout { get; set; } = default!;

    internal _TabLabelBarRenderer__tabs(global::Doroti.Generated.Framework.Painting.Axis direction, global::Doroti.Generated.Framework.Rendering.MainAxisSize mainAxisSize, global::Doroti.Generated.Framework.Rendering.MainAxisAlignment mainAxisAlignment, global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment crossAxisAlignment, TextDirection textDirection, global::Doroti.Generated.Framework.Painting.VerticalDirection verticalDirection, global::System.Action<List<double>, TextDirection, double> onPerformLayout) : base(direction: direction, mainAxisSize: mainAxisSize, mainAxisAlignment: mainAxisAlignment, crossAxisAlignment: crossAxisAlignment, textDirection: textDirection, verticalDirection: verticalDirection)
    {
        this.onPerformLayout = onPerformLayout;
    }

    public override void performLayout()
    {
        base.performLayout();
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__12789 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)this.firstChild);
        var xOffsets__12819 = new List<double>();
        while ((child__12789 is not null))
        {
            var childParentData__12882 = ((global::Doroti.Generated.Framework.Rendering.FlexParentData?)(object?)child__12789.parentData!)!;
            xOffsets__12819.Add(childParentData__12882.offset.dx);
            DartRuntimePrimitives.Assert(() => (object.Equals(child__12789.parentData, childParentData__12882)));
            child__12789 = childParentData__12882.nextSibling;
        }
        DartRuntimePrimitives.Assert(() => (this.textDirection is not null));
        switch (DartRuntimePrimitives.RequireValue(this.textDirection))
        {
            case TextDirection.rtl:
                {
                    xOffsets__12819.Insert(checked((int)0L), this.size.width);
                    break;
                }
            case TextDirection.ltr:
                {
                    xOffsets__12819.Add(this.size.width);
                    break;
                }
        }
        this.onPerformLayout(xOffsets__12819, DartRuntimePrimitives.RequireValue(this.textDirection), this.size.width);
    }

}

internal class _TabLabelBar__tabs : global::Doroti.Generated.Framework.Widgets.Flex
{
    public virtual global::System.Action<List<double>, TextDirection, double> onPerformLayout { get; private set; } = default!;

    internal _TabLabelBar__tabs(List<global::Doroti.Generated.Framework.Widgets.Widget> children = default!, global::System.Action<List<double>, TextDirection, double> onPerformLayout = default!, global::Doroti.Generated.Framework.Rendering.MainAxisSize mainAxisSize = default!) : base(children: children ?? new List<global::Doroti.Generated.Framework.Widgets.Widget>(), mainAxisSize: mainAxisSize, direction: global::Doroti.Generated.Framework.Painting.Axis.horizontal, mainAxisAlignment: global::Doroti.Generated.Framework.Rendering.MainAxisAlignment.start, crossAxisAlignment: global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.center, verticalDirection: global::Doroti.Generated.Framework.Painting.VerticalDirection.down)
    {
        this.onPerformLayout = onPerformLayout;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new _TabLabelBarRenderer__tabs(direction: this.direction, mainAxisAlignment: this.mainAxisAlignment, mainAxisSize: this.mainAxisSize, crossAxisAlignment: this.crossAxisAlignment, textDirection: DartRuntimePrimitives.RequireValue(getEffectiveTextDirection(context)), verticalDirection: this.verticalDirection, onPerformLayout: (global::System.Action<List<double>, TextDirection, double>)this.onPerformLayout));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_TabLabelBarRenderer__tabs)(object)renderObject;
        base.updateRenderObject(context, __renderObject);
        __renderObject.onPerformLayout = (global::System.Action<List<double>, TextDirection, double>)this.onPerformLayout;
    }

}

public static partial class TabsLibrary
{
    internal static double _indexChangeProgress(TabController controller)
    {
        double controllerValue__14626 = ((TabController)controller).animation!.value;
        double previousIndex__14688 = ((TabController)controller).previousIndex.toDouble();
        double currentIndex__14756 = ((TabController)controller).index.toDouble();
        if (!((TabController)controller).indexIsChanging)
        {
            return Dart_uiLibrary.clampDouble(((currentIndex__14756 - controllerValue__14626)).abs(), 0.0, 1.0);
        }
        return (((controllerValue__14626 - currentIndex__14756)).abs() / ((currentIndex__14756 - previousIndex__14688)).abs());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _DividerPainter__tabs : global::Doroti.Generated.Framework.Rendering.CustomPainter
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
        if ((this.dividerHeight <= 0.0))
        {
            return;
        }
        var paint__15520 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Flutter.Ui.Paint();
            __cascade.color = this.dividerColor;
            __cascade.strokeWidth = this.dividerHeight;
            return __cascade;        }))();
        canvas.drawLine(new global::Doroti.Flutter.Ui.Offset(0, (size.height - ((paint__15520.strokeWidth / 2L)))), new global::Doroti.Flutter.Ui.Offset(size.width, (size.height - ((paint__15520.strokeWidth / 2L)))), paint__15520);
    }

    public override bool shouldRepaint(global::Doroti.Generated.Framework.Rendering.CustomPainter oldDelegate)
    {
        var __oldDelegate = (_DividerPainter__tabs)(object)oldDelegate;
        return ((!object.Equals(((_DividerPainter__tabs)__oldDelegate).dividerColor, this.dividerColor)) || (((_DividerPainter__tabs)__oldDelegate).dividerHeight != this.dividerHeight));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _IndicatorPainterNotifier__tabs : global::Doroti.Generated.Framework.Foundation.ChangeNotifier
{
    public virtual void notify()
    {
        notifyListeners();
    }

    public override string ToString() => global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
}

internal class _IndicatorPainter__tabs : global::Doroti.Generated.Framework.Rendering.CustomPainter
{
    public virtual TabController controller { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.Decoration indicator { get; private set; } = default!;
    public virtual TabBarIndicatorSize indicatorSize { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry indicatorPadding { get; private set; } = default!;
    public virtual List<global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>> tabKeys { get; private set; } = default!;
    public virtual List<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry> labelPaddings { get; private set; } = default!;
    public virtual Color? dividerColor { get; private set; }
    public virtual double? dividerHeight { get; private set; }
    public virtual bool showDivider { get; private set; } = default!;
    public virtual double? devicePixelRatio { get; private set; }
    public virtual TabIndicatorAnimation indicatorAnimation { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;
    internal virtual _IndicatorPainterNotifier__tabs _repaint { get; private set; } = default!;
    internal virtual List<double>? _currentTabOffsets { get; set; } = default;
    internal virtual TextDirection? _currentTextDirection { get; set; } = default;
    internal virtual Rect? _currentRect { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Painting.BoxPainter? _painter { get; set; } = default;
    internal virtual bool _needsPaint { get; set; } = false;

    internal static _IndicatorPainter__tabs Create(TabController controller, global::Doroti.Generated.Framework.Painting.Decoration indicator, TabBarIndicatorSize indicatorSize, List<global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>> tabKeys, _IndicatorPainter__tabs? old, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry indicatorPadding, List<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry> labelPaddings, Color? dividerColor = null, double? dividerHeight = null, bool showDivider = default!, double? devicePixelRatio = null, TabIndicatorAnimation indicatorAnimation = default!, TextDirection textDirection = default!)
    {
        return new _IndicatorPainter__tabs(controller: controller, indicator: indicator, indicatorSize: indicatorSize, tabKeys: tabKeys, old: old, indicatorPadding: indicatorPadding, labelPaddings: labelPaddings, dividerColor: dividerColor, dividerHeight: dividerHeight, showDivider: showDivider, devicePixelRatio: devicePixelRatio, indicatorAnimation: indicatorAnimation, textDirection: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(textDirection)), repaint: new _IndicatorPainterNotifier__tabs());
    }

    internal _IndicatorPainter__tabs(TabController controller, global::Doroti.Generated.Framework.Painting.Decoration indicator, TabBarIndicatorSize indicatorSize, List<global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>> tabKeys, _IndicatorPainter__tabs? old, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry indicatorPadding, List<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry> labelPaddings, Color? dividerColor = null, double? dividerHeight = null, bool showDivider = default!, double? devicePixelRatio = null, TabIndicatorAnimation indicatorAnimation = default!, TextDirection textDirection = default!, _IndicatorPainterNotifier__tabs repaint = default!) : base(repaint: global::Doroti.Generated.Framework.Foundation.Listenable.CreateMerge(new List<global::Doroti.Generated.Framework.Foundation.Listenable?> { ((TabController)controller).animation, repaint }.Cast<global::Doroti.Generated.Framework.Foundation.Listenable?>()))
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
        this._repaint = repaint;
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugMaybeDispatchCreated("material", "_IndicatorPainter", this));
        if ((old is not null))
        {
            saveTabOffsets(((_IndicatorPainter__tabs)old)._currentTabOffsets, ((_IndicatorPainter__tabs)old)._currentTextDirection);
        }
    }

    public virtual void markNeedsPaint()
    {
        _needsPaint = true;
        this._repaint.notify();
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        this._painter?.dispose();
        this._repaint.dispose();
    }

    public virtual void saveTabOffsets(List<double>? tabOffsets, TextDirection? textDirection)
    {
        _currentTabOffsets = tabOffsets;
        _currentTextDirection = textDirection;
    }

    public virtual long maxTabIndex => DartRuntimePrimitives.ConvertValue<long>((checked((long)(this._currentTabOffsets!.Count)) - 2L));
    public virtual double centerOf(long tabIndex)
    {
        DartRuntimePrimitives.Assert(() => (this._currentTabOffsets is not null));
        DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(this._currentTabOffsets!));
        DartRuntimePrimitives.Assert(() => (tabIndex >= 0L));
        DartRuntimePrimitives.Assert(() => (tabIndex <= this.maxTabIndex));
        return (((this._currentTabOffsets![(int)(tabIndex)] + this._currentTabOffsets![(int)((tabIndex + 1L))])) / 2.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Flutter.Ui.Rect indicatorRect(Size tabBarSize, long tabIndex)
    {
        DartRuntimePrimitives.Assert(() => (this._currentTabOffsets is not null));
        DartRuntimePrimitives.Assert(() => (this._currentTextDirection is not null));
        DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(this._currentTabOffsets!));
        DartRuntimePrimitives.Assert(() => (tabIndex >= 0L));
        DartRuntimePrimitives.Assert(() => (tabIndex <= this.maxTabIndex));
        double tabLeft__20395 = default!;
        double tabRight__20404 = default!;
        DartRuntimePrimitives.Ignore((tabLeft__20395, tabRight__20404) = (DartRuntimePrimitives.RequireValue(this._currentTextDirection) switch { TextDirection.rtl => (((double, double))((this._currentTabOffsets![(int)((tabIndex + 1L))], this._currentTabOffsets![(int)(tabIndex)]))), TextDirection.ltr => (((double, double))((this._currentTabOffsets![(int)(tabIndex)], this._currentTabOffsets![(int)((tabIndex + 1L))]))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        if ((object.Equals(this.indicatorSize, TabBarIndicatorSize.label)))
        {
            double tabWidth__20745 = DartRuntimePrimitives.RequireValue(this.tabKeys[(int)(tabIndex)].currentContext!.size).width;
            global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry labelPadding__20834 = this.labelPaddings[(int)(tabIndex)];
            global::Doroti.Generated.Framework.Painting.EdgeInsets insets__20897 = ((global::Doroti.Generated.Framework.Painting.EdgeInsets)(object?)labelPadding__20834.resolve(this._currentTextDirection));
            double delta__20970 = (((((tabRight__20404 - tabLeft__20395)) - ((tabWidth__20745 + insets__20897.horizontal)))) / 2.0);
            tabLeft__20395 += (delta__20970 + ((global::Doroti.Generated.Framework.Painting.EdgeInsets)insets__20897).left);
            tabRight__20404 = (tabLeft__20395 + tabWidth__20745);
        }
        global::Doroti.Generated.Framework.Painting.EdgeInsets insets__21144 = ((global::Doroti.Generated.Framework.Painting.EdgeInsets)(object?)this.indicatorPadding.resolve(this._currentTextDirection));
        var rect__21212 = global::Doroti.Flutter.Ui.Rect.fromLTWH(tabLeft__20395, 0.0, (tabRight__20404 - tabLeft__20395), tabBarSize.height);
        if (!((rect__21212.size >= insets__21144.collapsedSize)))
        {
            throw DartRuntimePrimitives.AsException(global::Doroti.Generated.Framework.Foundation.FlutterError.Create("indicatorPadding insets should be less than Tab Size\n" + $"Rect Size : {rect__21212.size}, Insets: {insets__21144}"));
        }
        return ((global::Doroti.Flutter.Ui.Rect)(object?)insets__21144.deflateRect(rect__21212));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(Canvas canvas, Size size)
    {
        _needsPaint = false;
        _painter ??= this.indicator.createBoxPainter(() => this.markNeedsPaint());
        double value__21694 = ((TabController)this.controller).animation!.value;
        _currentRect = (this.indicatorAnimation switch { TabIndicatorAnimation.linear => _applyLinearEffect(size: size, value: value__21694), TabIndicatorAnimation.elastic => _applyElasticEffect(size: size, value: value__21694), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        DartRuntimePrimitives.Assert(() => (this._currentRect is not null));
        var configuration__22004 = new global::Doroti.Generated.Framework.Painting.ImageConfiguration(size: DartRuntimePrimitives.RequireValue(this._currentRect).size, textDirection: this._currentTextDirection, devicePixelRatio: this.devicePixelRatio);
        if ((this.showDivider && (DartRuntimePrimitives.RequireValue(this.dividerHeight) > 0L)))
        {
            var dividerPaint__22222 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Flutter.Ui.Paint();
            __cascade.color = this.dividerColor!;
            __cascade.strokeWidth = DartRuntimePrimitives.RequireValue(this.dividerHeight);
            return __cascade;        }))();
            var dividerP1__22329 = new global::Doroti.Flutter.Ui.Offset(0, (size.height - ((dividerPaint__22222.strokeWidth / 2L))));
            var dividerP2__22410 = new global::Doroti.Flutter.Ui.Offset(size.width, (size.height - ((dividerPaint__22222.strokeWidth / 2L))));
            canvas.drawLine(dividerP1__22329, dividerP2__22410, dividerPaint__22222);
        }
        this._painter!.paint(canvas, DartRuntimePrimitives.RequireValue(this._currentRect).topLeft, configuration__22004);
    }

    internal virtual global::Doroti.Flutter.Ui.Rect? _applyLinearEffect(Size size, double value)
    {
        double index__22766 = ((TabController)this.controller).index.toDouble();
        bool ltr__22818 = (index__22766 > value);
        long from__22853 = ((ltr__22818 ? value.floor() : value.ceil())).clamp(0L, this.maxTabIndex);
        long to__22934 = ((ltr__22818 ? (from__22853 + 1L) : (from__22853 - 1L))).clamp(0L, this.maxTabIndex);
        global::Doroti.Flutter.Ui.Rect fromRect__23005 = ((global::Doroti.Flutter.Ui.Rect)(object?)indicatorRect(size, from__22853));
        global::Doroti.Flutter.Ui.Rect toRect__23058 = ((global::Doroti.Flutter.Ui.Rect)(object?)indicatorRect(size, to__22934));
        return Dart_uiLibrary.Rect.lerp(fromRect__23005, toRect__23058, ((value - from__22853)).abs());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double decelerateInterpolation(double fraction)
    {
        return global::Doroti.Flutter.Runtime.Dart_mathLibrary.sin((((fraction * Dart_mathLibrary.pi)) / 2.0));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double accelerateInterpolation(double fraction)
    {
        return (1.0 - global::Doroti.Flutter.Runtime.Dart_mathLibrary.cos((((fraction * Dart_mathLibrary.pi)) / 2.0)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Flutter.Ui.Rect? _applyElasticEffect(Size size, double value)
    {
        double index__23589 = ((TabController)this.controller).index.toDouble();
        double progressLeft__23637 = ((index__23589 - value)).abs();
        long to__23690 = (((progressLeft__23637 == 0.0) || !((TabController)this.controller).indexIsChanging) ? (this.textDirection switch { TextDirection.ltr => value.ceil(), TextDirection.rtl => value.floor(), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }).clamp(0L, this.maxTabIndex) : ((TabController)this.controller).index);
        long from__23952 = (((progressLeft__23637 == 0.0) || !((TabController)this.controller).indexIsChanging) ? (this.textDirection switch { TextDirection.ltr => ((to__23690 - 1L)), TextDirection.rtl => ((to__23690 + 1L)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }).clamp(0L, this.maxTabIndex) : ((TabController)this.controller).previousIndex);
        global::Doroti.Flutter.Ui.Rect toRect__24216 = ((global::Doroti.Flutter.Ui.Rect)(object?)indicatorRect(size, to__23690));
        global::Doroti.Flutter.Ui.Rect fromRect__24265 = ((global::Doroti.Flutter.Ui.Rect)(object?)indicatorRect(size, from__23952));
        global::Doroti.Flutter.Ui.Rect rect__24318 = ((global::Doroti.Flutter.Ui.Rect)(object?)DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Rect.lerp(fromRect__24265, toRect__24216, ((value - from__23952)).abs())));
        if (((TabController)this.controller).animation!.isCompleted)
        {
            return rect__24318;
        }
        double tabChangeProgress__24712 = default!;
        if (((TabController)this.controller).indexIsChanging)
        {
            long tabsDelta__24786 = ((((TabController)this.controller).index - ((TabController)this.controller).previousIndex)).abs();
            if ((tabsDelta__24786 != 0L))
            {
                progressLeft__23637 /= tabsDelta__24786;
            }
            tabChangeProgress__24712 = (1L - Dart_uiLibrary.clampDouble(progressLeft__23637, 0.0, 1.0));
        }
        else
        {
            tabChangeProgress__24712 = ((index__23589 - value)).abs();
        }
        if ((tabChangeProgress__24712 == 1.0))
        {
            return rect__24318;
        }
        double leftFraction__25221 = default!;
        double rightFraction__25252 = default!;
        bool isMovingRight__25282 = (this.textDirection switch { TextDirection.ltr => (((TabController)this.controller).indexIsChanging ? (index__23589 > value) : (value > index__23589)), TextDirection.rtl => (((TabController)this.controller).indexIsChanging ? (value > index__23589) : (index__23589 > value)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        if (isMovingRight__25282)
        {
            leftFraction__25221 = accelerateInterpolation(tabChangeProgress__24712);
            rightFraction__25252 = decelerateInterpolation(tabChangeProgress__24712);
        }
        else
        {
            leftFraction__25221 = decelerateInterpolation(tabChangeProgress__24712);
            rightFraction__25252 = accelerateInterpolation(tabChangeProgress__24712);
        }
        double lerpRectLeft__25828 = default!;
        double lerpRectRight__25859 = default!;
        if (((TabController)this.controller).indexIsChanging)
        {
            lerpRectLeft__25828 = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(fromRect__24265.left, toRect__24216.left, leftFraction__25221));
            lerpRectRight__25859 = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(fromRect__24265.right, toRect__24216.right, rightFraction__25252));
        }
        else
        {
            lerpRectLeft__25828 = (((object)isMovingRight__25282) switch { true => DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(fromRect__24265.left, toRect__24216.left, leftFraction__25221)), false => DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(toRect__24216.left, fromRect__24265.left, leftFraction__25221)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            lerpRectRight__25859 = (((object)isMovingRight__25282) switch { true => DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(fromRect__24265.right, toRect__24216.right, rightFraction__25252)), false => DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(toRect__24216.right, fromRect__24265.right, rightFraction__25252)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        }
        return global::Doroti.Flutter.Ui.Rect.fromLTRB(lerpRectLeft__25828, rect__24318.top, lerpRectRight__25859, rect__24318.bottom);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRepaint(global::Doroti.Generated.Framework.Rendering.CustomPainter oldDelegate)
    {
        var __old = (_IndicatorPainter__tabs)(object)oldDelegate;
        return (((((this._needsPaint || (!object.Equals(this.controller, ((_IndicatorPainter__tabs)__old).controller))) || (!object.Equals(this.indicator, ((_IndicatorPainter__tabs)__old).indicator))) || (checked((long)(this.tabKeys.Count)) != checked((long)(((_IndicatorPainter__tabs)__old).tabKeys.Count)))) || (!global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.listEquals(this._currentTabOffsets, ((_IndicatorPainter__tabs)__old)._currentTabOffsets))) || (!object.Equals(this._currentTextDirection, ((_IndicatorPainter__tabs)__old)._currentTextDirection)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ChangeAnimation__tabs : global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.AnimationWithParentMixin<double>
{
    public virtual TabController controller { get; private set; } = default!;

    internal _ChangeAnimation__tabs(TabController controller)
    {
        this.controller = controller;
    }

    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> parent => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Animation.Animation<double>>(((TabController)this.controller).animation!);
    public override void removeStatusListener(AnimationStatusListener listener)
    {
        if ((((TabController)this.controller).animation is not null))
        {
            DartRuntimePrimitives.Noop();
        }
    }

    public override void removeListener(global::System.Action listener)
    {
        if ((((TabController)this.controller).animation is not null))
        {
            DartRuntimePrimitives.Noop();
        }
    }

    public override double value => TabsLibrary._indexChangeProgress(this.controller);
    public override void addListener(global::System.Action listener) => this.parent.addListener(() => listener());
    public override void addStatusListener(AnimationStatusListener listener) => this.parent.addStatusListener((AnimationStatusListener)listener);
    public override AnimationStatus status => ((Animation<double>)this.parent).status;
}

internal class _DragAnimation__tabs : global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.AnimationWithParentMixin<double>
{
    public virtual TabController controller { get; private set; } = default!;
    public virtual long index { get; private set; } = default!;

    internal _DragAnimation__tabs(TabController controller, long index)
    {
        this.controller = controller;
        this.index = index;
    }

    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> parent => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Animation.Animation<double>>(((TabController)this.controller).animation!);
    public override void removeStatusListener(AnimationStatusListener listener)
    {
        if ((((TabController)this.controller).animation is not null))
        {
            DartRuntimePrimitives.Noop();
        }
    }

    public override void removeListener(global::System.Action listener)
    {
        if ((((TabController)this.controller).animation is not null))
        {
            DartRuntimePrimitives.Noop();
        }
    }

    public override double value
    {
        get
        {
            DartRuntimePrimitives.Assert(() => !((TabController)this.controller).indexIsChanging);
            double controllerMaxValue__28466 = ((((TabController)this.controller).length - 1L)).toDouble();
            double controllerValue__28540 = Dart_uiLibrary.clampDouble(((TabController)this.controller).animation!.value, 0.0, controllerMaxValue__28466);
            return Dart_uiLibrary.clampDouble(((controllerValue__28540 - this.index.toDouble())).abs(), 0.0, 1.0);
            return default!;
        }
    }
    public override void addListener(global::System.Action listener) => this.parent.addListener(() => listener());
    public override void addStatusListener(AnimationStatusListener listener) => this.parent.addStatusListener((AnimationStatusListener)listener);
    public override AnimationStatus status => ((Animation<double>)this.parent).status;
}

internal class _TabBarScrollPosition__tabs : global::Doroti.Generated.Framework.Widgets.ScrollPositionWithSingleContext
{
    public virtual _TabBarState__tabs tabBar { get; private set; } = default!;
    internal virtual bool _viewportDimensionWasNonZero { get; set; } = false;
    internal virtual bool _needsPixelsCorrection { get; set; } = true;

    internal _TabBarScrollPosition__tabs(global::Doroti.Generated.Framework.Widgets.ScrollPhysics physics, global::Doroti.Generated.Framework.Widgets.ScrollContext context, global::Doroti.Generated.Framework.Widgets.ScrollPosition? oldPosition, _TabBarState__tabs tabBar) : base(physics: physics, context: context, oldPosition: oldPosition, initialPixels: null)
    {
        this.tabBar = tabBar;
    }

    public override bool applyContentDimensions(double minScrollExtent, double maxScrollExtent)
    {
        var result__29559 = true;
        if (!this._viewportDimensionWasNonZero)
        {
            _viewportDimensionWasNonZero = (this.viewportDimension != 0.0);
        }
        if ((!this._viewportDimensionWasNonZero || this._needsPixelsCorrection))
        {
            _needsPixelsCorrection = false;
            correctPixels(this.tabBar._initialScrollOffset(this.viewportDimension, minScrollExtent, maxScrollExtent));
            result__29559 = false;
        }
        return (base.applyContentDimensions(minScrollExtent, maxScrollExtent) && result__29559);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void markNeedsPixelsCorrection()
    {
        _needsPixelsCorrection = true;
    }

}

public class TabBarScrollController : global::Doroti.Generated.Framework.Widgets.ScrollController
{
    internal virtual _TabBarState__tabs? _tabBarState { get; set; } = default;

    public virtual bool debugCheckHasTabBarState()
    {
        DartRuntimePrimitives.Assert(() => (this._tabBarState is not null), () => (object?)"This TabBarScrollController is not attached to any TabBar.");
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.ScrollPosition createScrollPosition(global::Doroti.Generated.Framework.Widgets.ScrollPhysics physics, global::Doroti.Generated.Framework.Widgets.ScrollContext context, global::Doroti.Generated.Framework.Widgets.ScrollPosition? oldPosition)
    {
        DartRuntimePrimitives.Assert(() => debugCheckHasTabBarState());
        return ((global::Doroti.Generated.Framework.Widgets.ScrollPosition)(object?)new _TabBarScrollPosition__tabs(physics: physics, context: context, oldPosition: oldPosition, tabBar: this._tabBarState!));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        _tabBarState = null;
        base.dispose();
    }

}

public delegate void TabValueChanged<T>(T value, long index);

public class TabBar : global::Doroti.Generated.Framework.Widgets.StatefulWidget, global::Doroti.Generated.Framework.Widgets.PreferredSizeWidget
{
    public virtual List<global::Doroti.Generated.Framework.Widgets.Widget> tabs { get; private set; } = default!;
    public virtual TabController? controller { get; private set; }
    public virtual TabBarScrollController? scrollController { get; private set; }
    public virtual bool isScrollable { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual Color? indicatorColor { get; private set; }
    public virtual double indicatorWeight { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry indicatorPadding { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.Decoration? indicator { get; private set; }
    public virtual bool automaticIndicatorColorAdjustment { get; private set; } = default!;
    public virtual TabBarIndicatorSize? indicatorSize { get; private set; }
    public virtual Color? dividerColor { get; private set; }
    public virtual double? dividerHeight { get; private set; }
    public virtual Color? labelColor { get; private set; }
    public virtual Color? unselectedLabelColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? labelStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? unselectedLabelStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? labelPadding { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual bool? enableFeedback { get; private set; }
    public virtual global::System.Action<long>? onTap { get; private set; }
    public virtual global::System.Action<bool, long>? onHover { get; private set; }
    public virtual global::System.Action<bool, long>? onFocusChange { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.ScrollPhysics? physics { get; private set; }
    public virtual InteractiveInkFeatureFactory? splashFactory { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.BorderRadius? splashBorderRadius { get; private set; }
    public virtual TabAlignment? tabAlignment { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextScaler? textScaler { get; private set; }
    public virtual TabIndicatorAnimation? indicatorAnimation { get; private set; }
    internal virtual bool _isPrimary { get; private set; } = default!;

    public TabBar(global::Doroti.Generated.Framework.Foundation.Key? key = null, List<global::Doroti.Generated.Framework.Widgets.Widget> tabs = default!, TabController? controller = null, TabBarScrollController? scrollController = null, bool isScrollable = false, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, Color? indicatorColor = null, bool automaticIndicatorColorAdjustment = true, double indicatorWeight = 2.0, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry indicatorPadding = default!, global::Doroti.Generated.Framework.Painting.Decoration? indicator = null, TabBarIndicatorSize? indicatorSize = null, Color? dividerColor = null, double? dividerHeight = null, Color? labelColor = null, global::Doroti.Generated.Framework.Painting.TextStyle? labelStyle = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? labelPadding = null, Color? unselectedLabelColor = null, global::Doroti.Generated.Framework.Painting.TextStyle? unselectedLabelStyle = null, global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Generated.Framework.Gestures.DragStartBehavior.start, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null, bool? enableFeedback = null, global::System.Action<long>? onTap = null, global::System.Action<bool, long>? onHover = null, global::System.Action<bool, long>? onFocusChange = null, global::Doroti.Generated.Framework.Widgets.ScrollPhysics? physics = null, InteractiveInkFeatureFactory? splashFactory = null, global::Doroti.Generated.Framework.Painting.BorderRadius? splashBorderRadius = null, TabAlignment? tabAlignment = null, global::Doroti.Generated.Framework.Painting.TextScaler? textScaler = null, TabIndicatorAnimation? indicatorAnimation = null) : base(key: key)
    {
        global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry __indicatorPadding = indicatorPadding ?? global::Doroti.Generated.Framework.Painting.EdgeInsets.zero;
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
        this._isPrimary = true;
        System.Diagnostics.Debug.Assert(((indicator is not null) || ((indicatorWeight > 0.0))));
    }

    public static TabBar CreateSecondary(global::Doroti.Generated.Framework.Foundation.Key? key = null, List<global::Doroti.Generated.Framework.Widgets.Widget> tabs = default!, TabController? controller = null, TabBarScrollController? scrollController = null, bool isScrollable = false, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, Color? indicatorColor = null, bool automaticIndicatorColorAdjustment = true, double indicatorWeight = 2.0, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry indicatorPadding = default!, global::Doroti.Generated.Framework.Painting.Decoration? indicator = null, TabBarIndicatorSize? indicatorSize = null, Color? dividerColor = null, double? dividerHeight = null, Color? labelColor = null, global::Doroti.Generated.Framework.Painting.TextStyle? labelStyle = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? labelPadding = null, Color? unselectedLabelColor = null, global::Doroti.Generated.Framework.Painting.TextStyle? unselectedLabelStyle = null, global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Generated.Framework.Gestures.DragStartBehavior.start, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null, bool? enableFeedback = null, global::System.Action<long>? onTap = null, global::System.Action<bool, long>? onHover = null, global::System.Action<bool, long>? onFocusChange = null, global::Doroti.Generated.Framework.Widgets.ScrollPhysics? physics = null, InteractiveInkFeatureFactory? splashFactory = null, global::Doroti.Generated.Framework.Painting.BorderRadius? splashBorderRadius = null, TabAlignment? tabAlignment = null, global::Doroti.Generated.Framework.Painting.TextScaler? textScaler = null, TabIndicatorAnimation? indicatorAnimation = null)
    {
        var __instance = new TabBar(key: key, tabs: tabs, controller: controller, scrollController: scrollController, isScrollable: isScrollable, padding: padding, indicatorColor: indicatorColor, automaticIndicatorColorAdjustment: automaticIndicatorColorAdjustment, indicatorWeight: indicatorWeight, indicatorPadding: indicatorPadding, indicator: indicator, indicatorSize: indicatorSize, dividerColor: dividerColor, dividerHeight: dividerHeight, labelColor: labelColor, labelStyle: labelStyle, labelPadding: labelPadding, unselectedLabelColor: unselectedLabelColor, unselectedLabelStyle: unselectedLabelStyle, dragStartBehavior: dragStartBehavior, overlayColor: overlayColor, mouseCursor: mouseCursor, enableFeedback: enableFeedback, onTap: onTap, onHover: onHover, onFocusChange: onFocusChange, physics: physics, splashFactory: splashFactory, splashBorderRadius: splashBorderRadius, tabAlignment: tabAlignment, textScaler: textScaler, indicatorAnimation: indicatorAnimation);
        global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry __indicatorPadding = indicatorPadding ?? global::Doroti.Generated.Framework.Painting.EdgeInsets.zero;
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
            double maxHeight__55406 = TabsLibrary._kTabHeight;
            foreach (global::Doroti.Generated.Framework.Widgets.Widget item__55453 in this.tabs)
            {
                if ((item__55453 is global::Doroti.Generated.Framework.Widgets.PreferredSizeWidget))
                {
                    global::Doroti.Generated.Framework.Widgets.PreferredSizeWidget item__55453__as55479 = (global::Doroti.Generated.Framework.Widgets.PreferredSizeWidget)item__55453;
                    double itemHeight__55531 = ((global::Doroti.Generated.Framework.Widgets.PreferredSizeWidget)((global::Doroti.Generated.Framework.Widgets.PreferredSizeWidget)item__55453__as55479)).preferredSize.height;
                    maxHeight__55406 = Math.Max(itemHeight__55531, maxHeight__55406);
                }
            }
            return new global::Doroti.Flutter.Ui.Size((maxHeight__55406 + this.indicatorWeight));
            return default!;
        }
    }
    public virtual bool tabHasTextAndIcon
    {
        get
        {
            foreach (global::Doroti.Generated.Framework.Widgets.Widget item__56008 in this.tabs)
            {
                if ((item__56008 is global::Doroti.Generated.Framework.Widgets.PreferredSizeWidget))
                {
                    global::Doroti.Generated.Framework.Widgets.PreferredSizeWidget item__56008__as56034 = (global::Doroti.Generated.Framework.Widgets.PreferredSizeWidget)item__56008;
                    if ((((global::Doroti.Generated.Framework.Widgets.PreferredSizeWidget)((global::Doroti.Generated.Framework.Widgets.PreferredSizeWidget)item__56008__as56034)).preferredSize.height == TabsLibrary._kTextAndIconTabHeight))
                    {
                        return true;
                    }
                }
            }
            return false;
            return default!;
        }
    }
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _TabBarState__tabs());
}

internal class _TabBarState__tabs : global::Doroti.Generated.Framework.Widgets.State<TabBar>
{
    internal virtual TabBarScrollController? _internalScrollController { get; set; } = default;
    internal virtual TabController? _controller { get; set; } = default;
    internal virtual _IndicatorPainter__tabs? _indicatorPainter { get; set; } = default;
    internal virtual long? _currentIndex { get; set; } = default;
    internal virtual double _tabStripWidth { get; set; } = default!;
    internal virtual List<global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>> _tabKeys { get; set; } = default!;
    internal virtual List<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry> _labelPaddings { get; set; } = default!;
    internal virtual bool _debugHasScheduledValidTabsCountCheck { get; set; } = false;

    public override void initState()
    {
        base.initState();
        _tabKeys = ((TabBar)(object)this.widget).tabs.map<global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>>(((tab) => global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create())).ToList();
        _labelPaddings = new List<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(System.Linq.Enumerable.Repeat<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Generated.Framework.Painting.EdgeInsets.zero, checked((int)checked((long)(((TabBar)(object)this.widget).tabs.Count)))));
    }

    internal virtual TabBarThemeData _defaults
    {
        get
        {
            if (Theme.of(this.context).useMaterial3)
            {
                return (((TabBar)(object)this.widget)._isPrimary ? new _TabsPrimaryDefaultsM3__tabs(this.context, ((TabBar)(object)this.widget).isScrollable) : new _TabsSecondaryDefaultsM3__tabs(this.context, ((TabBar)(object)this.widget).isScrollable));
            }
            else
            {
                return ((TabBarThemeData)(object?)new _TabsDefaultsM2__tabs(this.context, ((TabBar)(object)this.widget).isScrollable));
            }
            return default!;
        }
    }
    internal virtual TabBarScrollController _effectiveScrollController
    {
        get
        {
            if ((((TabBar)(object)this.widget).scrollController is not null))
            {
                this._internalScrollController?.dispose();
                _internalScrollController = null;
                return ((TabBar)(object)this.widget).scrollController!;
            }
            return _internalScrollController ??= new TabBarScrollController();
            return default!;
        }
    }
    internal virtual global::Doroti.Generated.Framework.Painting.Decoration _getIndicator(TabBarIndicatorSize indicatorSize)
    {
        ThemeData theme__57872 = Theme.of(this.context);
        TabBarThemeData tabBarTheme__57925 = TabBarTheme.of(this.context);
        if ((((TabBar)(object)this.widget).indicator is not null))
        {
            return ((TabBar)(object)this.widget).indicator!;
        }
        if ((tabBarTheme__57925.indicator is not null))
        {
            return tabBarTheme__57925.indicator!;
        }
        global::Doroti.Flutter.Ui.Color color__58134 = ((global::Doroti.Flutter.Ui.Color)(object?)((((TabBar)(object)this.widget).indicatorColor ?? tabBarTheme__57925.indicatorColor) ?? this._defaults.indicatorColor!));
        if ((((TabBar)(object)this.widget).automaticIndicatorColorAdjustment && (color__58134.value == Material.maybeOf(this.context)?.color?.value)))
        {
            color__58134 = Colors.white;
        }
        double effectiveIndicatorWeight__59270 = (theme__57872.useMaterial3 ? Math.Max(((TabBar)(object)this.widget).indicatorWeight, (((object)((TabBar)(object)this.widget)._isPrimary) switch { true => _TabsPrimaryDefaultsM3__tabs.indicatorWeight(indicatorSize), false => _TabsSecondaryDefaultsM3__tabs.indicatorWeight, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })) : ((TabBar)(object)this.widget).indicatorWeight);
        bool primaryWithLabelIndicator__59670 = (indicatorSize switch { TabBarIndicatorSize.label => ((TabBar)(object)this.widget)._isPrimary, TabBarIndicatorSize.tab => false, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Generated.Framework.Painting.BorderRadius? effectiveBorderRadius__59848 = ((theme__57872.useMaterial3 && primaryWithLabelIndicator__59670) ? new global::Doroti.Generated.Framework.Painting.BorderRadius(topLeft: global::Doroti.Flutter.Ui.Radius.circular(effectiveIndicatorWeight__59270), topRight: global::Doroti.Flutter.Ui.Radius.circular(effectiveIndicatorWeight__59270)) : null);
        return ((global::Doroti.Generated.Framework.Painting.Decoration)(object?)new UnderlineTabIndicator(borderRadius: effectiveBorderRadius__59848, borderSide: new global::Doroti.Generated.Framework.Painting.BorderSide(width: effectiveIndicatorWeight__59270, color: color__58134)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _controllerIsValid => DartRuntimePrimitives.ConvertValue<bool>((this._controller?.animation is not null));
    internal virtual void _updateTabController()
    {
        TabController? newController__60931 = ((((TabBar)(object)this.widget).controller ?? (TabController)DefaultTabController.maybeOf(this.context)));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((newController__60931 is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Generated.Framework.Foundation.FlutterError.Create($"No TabController for {DartRuntimePrimitives.RuntimeType(this.widget)}.\n" + $"When creating a {DartRuntimePrimitives.RuntimeType(this.widget)}, you must either provide an explicit " + "TabController using the \"controller\" property, or you must ensure that there " + $"is a DefaultTabController above the {DartRuntimePrimitives.RuntimeType(this.widget)}.\n" + "In this case, there was neither an explicit controller nor a default controller."));
                }
                return true;
            });
        if ((object.Equals(newController__60931, this._controller)))
        {
            return;
        }
        if (this._controllerIsValid)
        {
            this._controller!.animation!.removeListener(() => this._handleTabControllerAnimationTick());
            this._controller!.removeListener(() => this._handleTabControllerTick());
        }
        _controller = newController__60931;
        if ((this._controller is not null))
        {
            this._controller!.animation!.addListener(() => this._handleTabControllerAnimationTick());
            this._controller!.addListener(() => this._handleTabControllerTick());
            _currentIndex = this._controller!.index;
        }
    }

    internal virtual void _updateScrollController(TabBarScrollController? oldScrollController = null)
    {
        if ((!object.Equals(oldScrollController, ((TabBar)(object)this.widget).scrollController)))
        {
            oldScrollController?._tabBarState = null;
        }
        if ((((TabBar)(object)this.widget).scrollController is not null))
        {
            this._internalScrollController?._tabBarState = null;
            ((TabBar)(object)this.widget).scrollController?._tabBarState = this;
        }
        else
        {
            _internalScrollController ??= new TabBarScrollController();
            this._internalScrollController?._tabBarState = this;
        }
    }

    internal virtual void _initIndicatorPainter()
    {
        ThemeData theme__62564 = Theme.of(this.context);
        TabBarThemeData tabBarTheme__62617 = TabBarTheme.of(this.context);
        TabBarIndicatorSize indicatorSize__62686 = ((((TabBar)(object)this.widget).indicatorSize ?? tabBarTheme__62617.indicatorSize) ?? DartRuntimePrimitives.RequireValue(this._defaults.indicatorSize));
        _IndicatorPainter__tabs? oldPainter__62819 = this._indicatorPainter;
        TabIndicatorAnimation defaultTabIndicatorAnimation__62884 = (indicatorSize__62686 switch { TabBarIndicatorSize.label => TabIndicatorAnimation.elastic, TabBarIndicatorSize.tab => TabIndicatorAnimation.linear, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        _indicatorPainter = (!this._controllerIsValid ? null : _IndicatorPainter__tabs.Create(controller: this._controller!, indicator: _getIndicator(indicatorSize__62686), indicatorSize: indicatorSize__62686, indicatorPadding: ((TabBar)(object)this.widget).indicatorPadding, tabKeys: this._tabKeys, old: oldPainter__62819, labelPaddings: this._labelPaddings, dividerColor: ((((TabBar)(object)this.widget).dividerColor ?? tabBarTheme__62617.dividerColor) ?? this._defaults.dividerColor), dividerHeight: ((((TabBar)(object)this.widget).dividerHeight ?? tabBarTheme__62617.dividerHeight) ?? this._defaults.dividerHeight), showDivider: (theme__62564.useMaterial3 && !((TabBar)(object)this.widget).isScrollable), devicePixelRatio: MediaQuery.devicePixelRatioOf(this.context), indicatorAnimation: ((((TabBar)(object)this.widget).indicatorAnimation ?? tabBarTheme__62617.indicatorAnimation) ?? defaultTabIndicatorAnimation__62884), textDirection: Directionality.of(this.context)));
        oldPainter__62819?.dispose();
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
        if (((!object.Equals(((TabBar)(object)this.widget).controller, ((TabBar)oldWidget).controller)) || (!object.Equals(((TabBar)(object)this.widget).scrollController, ((TabBar)oldWidget).scrollController))))
        {
            _updateScrollController(oldScrollController: ((TabBar)oldWidget).scrollController);
            _updateTabController();
            _initIndicatorPainter();
            if (this._effectiveScrollController.hasClients)
            {
                global::Doroti.Generated.Framework.Widgets.ScrollPosition position__64819 = this._effectiveScrollController.position;
                if ((position__64819 is _TabBarScrollPosition__tabs))
                {
                    _TabBarScrollPosition__tabs position__64819__as64879 = (_TabBarScrollPosition__tabs)position__64819;
                    ((_TabBarScrollPosition__tabs)position__64819__as64879).markNeedsPixelsCorrection();
                }
            }
        }
        else
        {
            if (((((((((!object.Equals(((TabBar)(object)this.widget).indicatorColor, ((TabBar)oldWidget).indicatorColor)) || (((TabBar)(object)this.widget).indicatorWeight != ((TabBar)oldWidget).indicatorWeight)) || (!object.Equals(((TabBar)(object)this.widget).indicatorSize, ((TabBar)oldWidget).indicatorSize))) || (!object.Equals(((TabBar)(object)this.widget).indicatorPadding, ((TabBar)oldWidget).indicatorPadding))) || (!object.Equals(((TabBar)(object)this.widget).indicator, ((TabBar)oldWidget).indicator))) || (!object.Equals(((TabBar)(object)this.widget).dividerColor, ((TabBar)oldWidget).dividerColor))) || (((TabBar)(object)this.widget).dividerHeight != ((TabBar)oldWidget).dividerHeight)) || (!object.Equals(((TabBar)(object)this.widget).indicatorAnimation, ((TabBar)oldWidget).indicatorAnimation))))
            {
                _initIndicatorPainter();
            }
        }
        if ((checked((long)(((TabBar)(object)this.widget).tabs.Count)) > checked((long)(this._tabKeys.Count))))
        {
            long delta__65575 = (checked((long)(((TabBar)(object)this.widget).tabs.Count)) - checked((long)(this._tabKeys.Count)));
            this._tabKeys.AddRange(DartRuntimePrimitives.CreateList<global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>>(delta__65575, ((n) => global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create())).Cast<global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>>());
            this._labelPaddings.AddRange(new List<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(System.Linq.Enumerable.Repeat<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Generated.Framework.Painting.EdgeInsets.zero, checked((int)delta__65575))).Cast<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>());
        }
        else
        {
            if ((checked((long)(((TabBar)(object)this.widget).tabs.Count)) < checked((long)(this._tabKeys.Count))))
            {
                this._tabKeys.RemoveRange(checked((int)checked((long)(((TabBar)(object)this.widget).tabs.Count))), checked((int)checked((long)(this._tabKeys.Count))));
                this._labelPaddings.RemoveRange(checked((int)checked((long)(((TabBar)(object)this.widget).tabs.Count))), checked((int)checked((long)(this._tabKeys.Count))));
            }
        }
    }

    public override void dispose()
    {
        this._indicatorPainter!.dispose();
        if (this._controllerIsValid)
        {
            this._controller!.animation!.removeListener(() => this._handleTabControllerAnimationTick());
            this._controller!.removeListener(() => this._handleTabControllerTick());
        }
        _controller = null;
        this._internalScrollController?.dispose();
        ((TabBar)(object)this.widget).scrollController?._tabBarState = null;
        base.dispose();
    }

    public virtual long maxTabIndex => this._indicatorPainter!.maxTabIndex;
    internal virtual double _tabScrollOffset(long index, double viewportWidth, double minExtent, double maxExtent)
    {
        if (!((TabBar)(object)this.widget).isScrollable)
        {
            return 0.0;
        }
        double tabCenter__66670 = this._indicatorPainter!.centerOf(index);
        double paddingStart__66729 = default!;
        switch (Directionality.of(this.context))
        {
            case TextDirection.rtl:
                {
                    paddingStart__66729 = (((TabBar)(object)this.widget).padding?.resolve(TextDirection.rtl).right ?? 0);
                    tabCenter__66670 = (this._tabStripWidth - tabCenter__66670);
                    break;
                }
            case TextDirection.ltr:
                {
                    paddingStart__66729 = (((TabBar)(object)this.widget).padding?.resolve(TextDirection.ltr).left ?? 0);
                    break;
                }
        }
        return Dart_uiLibrary.clampDouble(((tabCenter__66670 + paddingStart__66729) - (viewportWidth / 2.0)), minExtent, maxExtent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _tabCenteredScrollOffset(long index)
    {
        global::Doroti.Generated.Framework.Widgets.ScrollPosition position__67226 = this._effectiveScrollController.position;
        return _tabScrollOffset(index, ((global::Doroti.Generated.Framework.Widgets.ScrollPosition)position__67226).viewportDimension, ((global::Doroti.Generated.Framework.Widgets.ScrollPosition)position__67226).minScrollExtent, ((global::Doroti.Generated.Framework.Widgets.ScrollPosition)position__67226).maxScrollExtent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _initialScrollOffset(double viewportWidth, double minExtent, double maxExtent)
    {
        return _tabScrollOffset(DartRuntimePrimitives.RequireValue(this._currentIndex), viewportWidth, minExtent, maxExtent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _scrollToCurrentIndex()
    {
        double offset__67654 = _tabCenteredScrollOffset(DartRuntimePrimitives.RequireValue(this._currentIndex));
        DartRuntimePrimitives.Ignore(this._effectiveScrollController.animateTo(offset__67654, duration: ConstantsLibrary.kTabScrollDuration, curve: global::Doroti.Generated.Framework.Animation.Curves.ease));
    }

    internal virtual void _scrollToControllerValue()
    {
        double? leadingPosition__67865 = ((DartRuntimePrimitives.RequireValue(this._currentIndex) > 0L) ? _tabCenteredScrollOffset((DartRuntimePrimitives.RequireValue(this._currentIndex) - 1L)) : null);
        double middlePosition__67990 = _tabCenteredScrollOffset(DartRuntimePrimitives.RequireValue(this._currentIndex));
        double? trailingPosition__68067 = ((DartRuntimePrimitives.RequireValue(this._currentIndex) < this.maxTabIndex) ? _tabCenteredScrollOffset((DartRuntimePrimitives.RequireValue(this._currentIndex) + 1L)) : null);
        double index__68204 = this._controller!.index.toDouble();
        double value__68260 = this._controller!.animation!.value;
        double offset__68316 = ((value__68260 - index__68204) switch { -1.0 => (leadingPosition__67865 ?? middlePosition__67990), 1.0 => (trailingPosition__68067 ?? middlePosition__67990), 0 => middlePosition__67990, < 0L => ((leadingPosition__67865 is null) ? middlePosition__67990 : DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(middlePosition__67990, DartRuntimePrimitives.RequireValue(leadingPosition__67865), (index__68204 - value__68260)))), _ => ((trailingPosition__68067 is null) ? middlePosition__67990 : DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(middlePosition__67990, DartRuntimePrimitives.RequireValue(trailingPosition__68067), (value__68260 - index__68204)))) });
        this._effectiveScrollController.jumpTo(offset__68316);
    }

    internal virtual void _handleTabControllerAnimationTick()
    {
        DartRuntimePrimitives.Assert(() => this.mounted);
        if ((!this._controller!.indexIsChanging && ((TabBar)(object)this.widget).isScrollable))
        {
            _currentIndex = this._controller!.index;
            _scrollToControllerValue();
        }
    }

    internal virtual void _handleTabControllerTick()
    {
        if ((this._controller!.index != this._currentIndex))
        {
            _currentIndex = this._controller!.index;
            if (((TabBar)(object)this.widget).isScrollable)
            {
                _scrollToCurrentIndex();
            }
        }
        setState(((global::System.Action)(() => {
})));
    }

    internal virtual void _saveTabOffsets(List<double> tabOffsets, TextDirection textDirection, double width)
    {
        _tabStripWidth = width;
        this._indicatorPainter?.saveTabOffsets(tabOffsets, textDirection);
    }

    internal virtual void _handleTap(long index)
    {
        DartRuntimePrimitives.Assert(() => ((index >= 0L) && (index < checked((long)(((TabBar)(object)this.widget).tabs.Count)))));
        this._controller!.animateTo(index);
        ((TabBar)(object)this.widget).onTap?.Invoke(index);
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildStyledTab(global::Doroti.Generated.Framework.Widgets.Widget child, bool isSelected, global::Doroti.Generated.Framework.Animation.Animation<double> animation, TabBarThemeData defaults)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _TabStyle__tabs(animation: animation, isSelected: isSelected, isPrimary: ((TabBar)(object)this.widget)._isPrimary, labelColor: ((TabBar)(object)this.widget).labelColor, unselectedLabelColor: ((TabBar)(object)this.widget).unselectedLabelColor, labelStyle: ((TabBar)(object)this.widget).labelStyle, unselectedLabelStyle: ((TabBar)(object)this.widget).unselectedLabelStyle, defaults: defaults, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _debugScheduleCheckHasValidTabsCount()
    {
        if (this._debugHasScheduledValidTabsCountCheck)
        {
            return true;
        }
        global::Doroti.Generated.Framework.Widgets.WidgetsBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((duration) => {
_debugHasScheduledValidTabsCountCheck = false;
if (!this.mounted)
{
    return;
}
DartRuntimePrimitives.Assert(() =>
    {
        if ((this._controller!.length != checked((long)(((TabBar)(object)this.widget).tabs.Count))))
        {
            throw DartRuntimePrimitives.AsException(global::Doroti.Generated.Framework.Foundation.FlutterError.Create($"Controller's length property ({this._controller!.length}) does not match the " + $"number of tabs ({checked((long)(((TabBar)(object)this.widget).tabs.Count))}) present in TabBar's tabs property."));
        }
        return true;
    });
})), debugLabel: "TabBar.tabsCountCheck");
        _debugHasScheduledValidTabsCountCheck = true;
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _debugTabAlignmentIsValid(TabAlignment tabAlignment)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((((TabBar)(object)this.widget).isScrollable && (object.Equals(tabAlignment, TabAlignment.fill))))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Generated.Framework.Foundation.FlutterError.Create($"{tabAlignment} is only valid for non-scrollable tab bars."));
                }
                if ((!((TabBar)(object)this.widget).isScrollable && (((object.Equals(tabAlignment, TabAlignment.start)) || (object.Equals(tabAlignment, TabAlignment.startOffset))))))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Generated.Framework.Foundation.FlutterError.Create($"{tabAlignment} is only valid for scrollable tab bars."));
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        DartRuntimePrimitives.Assert(() => _debugScheduleCheckHasValidTabsCount());
        ThemeData theme__71781 = Theme.of(context);
        TabBarThemeData tabBarTheme__71834 = TabBarTheme.of(context);
        TabAlignment effectiveTabAlignment__71896 = ((((TabBar)(object)this.widget).tabAlignment ?? tabBarTheme__71834.tabAlignment) ?? DartRuntimePrimitives.RequireValue(this._defaults.tabAlignment));
        DartRuntimePrimitives.Assert(() => _debugTabAlignmentIsValid(effectiveTabAlignment__71896));
        MaterialLocalizations localizations__72099 = MaterialLocalizations.of(context);
        if ((this._controller!.length == 0L))
        {
            return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.LimitedBox(maxWidth: 0.0, child: new global::Doroti.Generated.Framework.Widgets.SizedBox(width: double.PositiveInfinity, height: (TabsLibrary._kTabHeight + ((TabBar)(object)this.widget).indicatorWeight))));
        }
        var wrappedTabs__72355 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)checked((long)(((TabBar)(object)this.widget).tabs.Count)))), ((index) => {
global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry padding__72450 = ((((TabBar)(object)this.widget).labelPadding ?? tabBarTheme__71834.labelPadding) ?? ConstantsLibrary.kTabLabelPadding);
double verticalAdjustment__72558 = (((TabsLibrary._kTextAndIconTabHeight - TabsLibrary._kTabHeight)) / 2.0);
global::Doroti.Generated.Framework.Widgets.Widget tab__72645 = ((TabBar)(object)this.widget).tabs[(int)(index)];
if ((((tab__72645 is global::Doroti.Generated.Framework.Widgets.PreferredSizeWidget) && (((global::Doroti.Generated.Framework.Widgets.PreferredSizeWidget)((global::Doroti.Generated.Framework.Widgets.PreferredSizeWidget)tab__72645)).preferredSize.height == TabsLibrary._kTabHeight)) && ((TabBar)(object)this.widget).tabHasTextAndIcon))
{
    global::Doroti.Generated.Framework.Widgets.PreferredSizeWidget tab__72645__as72681 = (global::Doroti.Generated.Framework.Widgets.PreferredSizeWidget)tab__72645;
    padding__72450 = padding__72450.add(global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(vertical: verticalAdjustment__72558));
}
this._labelPaddings[(int)(index)] = padding__72450;
return new global::Doroti.Generated.Framework.Widgets.Center(heightFactor: 1.0, child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: this._labelPaddings[(int)(index)], child: new global::Doroti.Generated.Framework.Widgets.KeyedSubtree(key: this._tabKeys[(int)(index)], child: ((TabBar)(object)this.widget).tabs[(int)(index)])));
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        if ((this._controller is not null))
        {
            long previousIndex__73459 = this._controller!.previousIndex;
            if (this._controller!.indexIsChanging)
            {
                DartRuntimePrimitives.Assert(() => (this._currentIndex != previousIndex__73459));
                global::Doroti.Generated.Framework.Animation.Animation<double> animation__73706 = ((global::Doroti.Generated.Framework.Animation.Animation<double>)(object?)new _ChangeAnimation__tabs(this._controller!));
                wrappedTabs__72355[(int)(DartRuntimePrimitives.RequireValue(this._currentIndex))] = _buildStyledTab(wrappedTabs__72355[(int)(DartRuntimePrimitives.RequireValue(this._currentIndex))], true, animation__73706, this._defaults);
                wrappedTabs__72355[(int)(previousIndex__73459)] = _buildStyledTab(wrappedTabs__72355[(int)(previousIndex__73459)], false, animation__73706, this._defaults);
            }
            else
            {
                long tabIndex__74181 = DartRuntimePrimitives.RequireValue(this._currentIndex);
                global::Doroti.Generated.Framework.Animation.Animation<double> centerAnimation__74240 = ((global::Doroti.Generated.Framework.Animation.Animation<double>)(object?)new _DragAnimation__tabs(this._controller!, tabIndex__74181));
                wrappedTabs__72355[(int)(tabIndex__74181)] = _buildStyledTab(wrappedTabs__72355[(int)(tabIndex__74181)], true, centerAnimation__74240, this._defaults);
                if ((DartRuntimePrimitives.RequireValue(this._currentIndex) > 0L))
                {
                    long tabIndex__74509 = (DartRuntimePrimitives.RequireValue(this._currentIndex) - 1L);
                    global::Doroti.Generated.Framework.Animation.Animation<double> previousAnimation__74574 = ((global::Doroti.Generated.Framework.Animation.Animation<double>)(object?)new global::Doroti.Generated.Framework.Animation.ReverseAnimation(new _DragAnimation__tabs(this._controller!, tabIndex__74509)));
                    wrappedTabs__72355[(int)(tabIndex__74509)] = _buildStyledTab(wrappedTabs__72355[(int)(tabIndex__74509)], false, previousAnimation__74574, this._defaults);
                }
                if ((DartRuntimePrimitives.RequireValue(this._currentIndex) < (checked((long)(((TabBar)(object)this.widget).tabs.Count)) - 1L)))
                {
                    long tabIndex__74934 = (DartRuntimePrimitives.RequireValue(this._currentIndex) + 1L);
                    global::Doroti.Generated.Framework.Animation.Animation<double> nextAnimation__74999 = ((global::Doroti.Generated.Framework.Animation.Animation<double>)(object?)new global::Doroti.Generated.Framework.Animation.ReverseAnimation(new _DragAnimation__tabs(this._controller!, tabIndex__74934)));
                    wrappedTabs__72355[(int)(tabIndex__74934)] = _buildStyledTab(wrappedTabs__72355[(int)(tabIndex__74934)], false, nextAnimation__74999, this._defaults);
                }
            }
        }
        long tabCount__75509 = checked((long)(((TabBar)(object)this.widget).tabs.Count));
        for (var index__75553 = 0L; (index__75553 < tabCount__75509); index__75553 += 1L)
        {
            var selectedState__75608 = ((Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>>)(() => { var __collection75624 = new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>(); if ((index__75553 == this._currentIndex)) { __collection75624.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.selected); } return __collection75624; }))();
            global::Doroti.Generated.Framework.Services.MouseCursor effectiveMouseCursor__75714 = ((((WidgetStateProperty.resolveAs<global::Doroti.Generated.Framework.Services.MouseCursor?>(((TabBar)(object)this.widget).mouseCursor, selectedState__75608) ?? (global::Doroti.Generated.Framework.Services.MouseCursor)tabBarTheme__71834.mouseCursor?.resolve(selectedState__75608))) ?? (global::Doroti.Generated.Framework.Services.MouseCursor)global::Doroti.Generated.Framework.Widgets.WidgetStateMouseCursor.clickable.resolve(selectedState__75608)));
            global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?> defaultOverlay__75998 = ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>)(object?)WidgetStateProperty.resolveWith<global::Doroti.Flutter.Ui.Color?>((states) => {
HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> effectiveStates__76131 = ((Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>>)(() =>
{            var __cascade = selectedState__75608.toSet();
            __cascade.UnionWith(states);
            return __cascade;        }))();
return (this._defaults.overlayColor?.resolve(effectiveStates__76131));
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            wrappedTabs__72355[(int)(index__75553)] = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new InkWell(mouseCursor: effectiveMouseCursor__75714, onTap: (() => {
_handleTap(index__75553);
}), onHover: ((value) => {
((TabBar)(object)this.widget).onHover?.Invoke(value, index__75553);
}), onFocusChange: ((value) => {
((TabBar)(object)this.widget).onFocusChange?.Invoke(value, index__75553);
}), enableFeedback: (((TabBar)(object)this.widget).enableFeedback ?? true), overlayColor: ((((TabBar)(object)this.widget).overlayColor ?? tabBarTheme__71834.overlayColor) ?? defaultOverlay__75998), splashFactory: ((((TabBar)(object)this.widget).splashFactory ?? tabBarTheme__71834.splashFactory) ?? this._defaults.splashFactory), borderRadius: ((((TabBar)(object)this.widget).splashBorderRadius ?? tabBarTheme__71834.splashBorderRadius) ?? this._defaults.splashBorderRadius), child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(bottom: ((TabBar)(object)this.widget).indicatorWeight), child: new global::Doroti.Generated.Framework.Widgets.Semantics(role: SemanticsRole.tab, child: new global::Doroti.Generated.Framework.Widgets.Stack(children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(wrappedTabs__72355[(int)(index__75553)]), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Semantics(selected: (index__75553 == this._currentIndex), label: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb ? null : localizations__72099.tabLabel(tabIndex: (index__75553 + 1L), tabCount: tabCount__75509)))) })))));
            wrappedTabs__72355[(int)(index__75553)] = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.MergeSemantics(child: wrappedTabs__72355[(int)(index__75553)]));
            if ((!((TabBar)(object)this.widget).isScrollable && (object.Equals(effectiveTabAlignment__71896, TabAlignment.fill))))
            {
                wrappedTabs__72355[(int)(index__75553)] = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Expanded(child: wrappedTabs__72355[(int)(index__75553)]));
            }
        }
        global::Doroti.Generated.Framework.Widgets.Widget tabBar__77889 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Semantics(role: SemanticsRole.tabBar, container: true, explicitChildNodes: true, child: new global::Doroti.Generated.Framework.Widgets.CustomPaint(painter: this._indicatorPainter, child: new _TabStyle__tabs(animation: global::Doroti.Generated.Framework.Animation.AnimationsLibrary.kAlwaysDismissedAnimation, isSelected: false, isPrimary: ((TabBar)(object)this.widget)._isPrimary, labelColor: ((TabBar)(object)this.widget).labelColor, unselectedLabelColor: ((TabBar)(object)this.widget).unselectedLabelColor, labelStyle: ((TabBar)(object)this.widget).labelStyle, unselectedLabelStyle: ((TabBar)(object)this.widget).unselectedLabelStyle, defaults: this._defaults, child: new _TabLabelBar__tabs(onPerformLayout: (global::System.Action<List<double>, TextDirection, double>)this._saveTabOffsets, mainAxisSize: ((object.Equals(effectiveTabAlignment__71896, TabAlignment.fill)) ? global::Doroti.Generated.Framework.Rendering.MainAxisSize.max : global::Doroti.Generated.Framework.Rendering.MainAxisSize.min), children: wrappedTabs__72355)))));
        if (((TabBar)(object)this.widget).isScrollable)
        {
            global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? effectivePadding__78794 = ((object.Equals(effectiveTabAlignment__71896, TabAlignment.startOffset)) ? global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: TabsLibrary._kStartOffset).add((((TabBar)(object)this.widget).padding ?? global::Doroti.Generated.Framework.Painting.EdgeInsets.zero)) : ((TabBar)(object)this.widget).padding);
            tabBar__77889 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.ScrollConfiguration(behavior: ScrollConfiguration.of(context).copyWith(overscroll: false), child: new global::Doroti.Generated.Framework.Widgets.SingleChildScrollView(dragStartBehavior: ((TabBar)(object)this.widget).dragStartBehavior, scrollDirection: global::Doroti.Generated.Framework.Painting.Axis.horizontal, controller: this._effectiveScrollController, padding: effectivePadding__78794, physics: ((TabBar)(object)this.widget).physics, child: tabBar__77889)));
            if (theme__71781.useMaterial3)
            {
                global::Doroti.Generated.Framework.Painting.AlignmentGeometry effectiveAlignment__79581 = (effectiveTabAlignment__71896 switch { TabAlignment.center => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.AlignmentGeometry>(global::Doroti.Generated.Framework.Painting.Alignment.center), TabAlignment.start or TabAlignment.startOffset => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.AlignmentGeometry>(global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerStart), TabAlignment.fill => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.AlignmentGeometry>(global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerStart), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
                global::Doroti.Flutter.Ui.Color dividerColor__79853 = ((global::Doroti.Flutter.Ui.Color)(object?)((((TabBar)(object)this.widget).dividerColor ?? tabBarTheme__71834.dividerColor) ?? this._defaults.dividerColor!));
                double dividerHeight__79977 = ((((TabBar)(object)this.widget).dividerHeight ?? tabBarTheme__71834.dividerHeight) ?? DartRuntimePrimitives.RequireValue(this._defaults.dividerHeight));
                tabBar__77889 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Align(heightFactor: 1.0, widthFactor: ((dividerHeight__79977 > 0L) ? null : 1.0), alignment: effectiveAlignment__79581, child: tabBar__77889));
                if (((!object.Equals(dividerColor__79853, Colors.transparent)) && (dividerHeight__79977 > 0L)))
                {
                    tabBar__77889 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.CustomPaint(painter: new _DividerPainter__tabs(dividerColor: dividerColor__79853, dividerHeight: dividerHeight__79977), child: tabBar__77889));
                }
            }
        }
        else
        {
            if ((((TabBar)(object)this.widget).padding is not null))
            {
                tabBar__77889 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Padding(padding: ((TabBar)(object)this.widget).padding!, child: tabBar__77889));
            }
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new Material(type: MaterialType.transparency, child: new global::Doroti.Generated.Framework.Widgets.MediaQuery(data: MediaQuery.of(context).copyWith(textScaler: (((TabBar)(object)this.widget).textScaler ?? tabBarTheme__71834.textScaler)), child: tabBar__77889)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class TabBarView : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual TabController? controller { get; private set; }
    public virtual List<global::Doroti.Generated.Framework.Widgets.Widget> children { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.ScrollPhysics? physics { get; private set; }
    public virtual global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual double viewportFraction { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;

    public TabBarView(global::Doroti.Generated.Framework.Foundation.Key? key = null, List<global::Doroti.Generated.Framework.Widgets.Widget> children = default!, TabController? controller = null, global::Doroti.Generated.Framework.Widgets.ScrollPhysics? physics = null, global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Generated.Framework.Gestures.DragStartBehavior.start, double viewportFraction = 1.0, Clip clipBehavior = Clip.hardEdge) : base(key: key)
    {
        this.children = children;
        this.controller = controller;
        this.physics = physics;
        this.dragStartBehavior = dragStartBehavior;
        this.viewportFraction = viewportFraction;
        this.clipBehavior = clipBehavior;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _TabBarViewState__tabs());
}

internal class _TabBarViewState__tabs : global::Doroti.Generated.Framework.Widgets.State<TabBarView>
{
    internal virtual TabController? _controller { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Widgets.PageController? _pageController { get; set; } = default;
    internal virtual List<global::Doroti.Generated.Framework.Widgets.Widget> _childrenWithKey { get; set; } = default!;
    internal virtual long? _currentIndex { get; set; } = default;
    internal virtual long _warpUnderwayCount { get; set; } = 0L;
    internal virtual long _scrollUnderwayCount { get; set; } = 0L;
    internal virtual bool _debugHasScheduledValidChildrenCountCheck { get; set; } = false;

    internal virtual bool _controllerIsValid => DartRuntimePrimitives.ConvertValue<bool>((this._controller?.animation is not null));
    internal virtual void _updateTabController()
    {
        TabController? newController__83653 = ((((TabBarView)(object)this.widget).controller ?? (TabController)DefaultTabController.maybeOf(this.context)));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((newController__83653 is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Generated.Framework.Foundation.FlutterError.Create($"No TabController for {DartRuntimePrimitives.RuntimeType(this.widget)}.\n" + $"When creating a {DartRuntimePrimitives.RuntimeType(this.widget)}, you must either provide an explicit " + "TabController using the \"controller\" property, or you must ensure that there " + $"is a DefaultTabController above the {DartRuntimePrimitives.RuntimeType(this.widget)}.\n" + "In this case, there was neither an explicit controller nor a default controller."));
                }
                return true;
            });
        if ((object.Equals(newController__83653, this._controller)))
        {
            return;
        }
        if (this._controllerIsValid)
        {
            this._controller!.animation!.removeListener(() => this._handleTabControllerAnimationTick());
        }
        _controller = newController__83653;
        if ((this._controller is not null))
        {
            this._controller!.animation!.addListener(() => this._handleTabControllerAnimationTick());
        }
    }

    internal virtual void _jumpToPage(long page)
    {
        _warpUnderwayCount += 1L;
        this._pageController!.jumpToPage(page);
        _warpUnderwayCount -= 1L;
    }

    internal async virtual Future _animateToPage(long page, Duration duration, global::Doroti.Generated.Framework.Animation.Curve curve)
    {
        _warpUnderwayCount += 1L;
        await this._pageController!.animateToPage(page, duration: duration, curve: curve);
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
        _currentIndex = this._controller!.index;
        if ((this._pageController is null))
        {
            _pageController = new global::Doroti.Generated.Framework.Widgets.PageController(initialPage: DartRuntimePrimitives.RequireValue(this._currentIndex), viewportFraction: ((TabBarView)(object)this.widget).viewportFraction);
        }
        else
        {
            this._pageController!.jumpToPage(DartRuntimePrimitives.RequireValue(this._currentIndex));
        }
    }

    public override void didUpdateWidget(TabBarView oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((TabBarView)(object)this.widget).controller, ((TabBarView)oldWidget).controller)))
        {
            _updateTabController();
            _currentIndex = this._controller!.index;
            _jumpToPage(DartRuntimePrimitives.RequireValue(this._currentIndex));
        }
        if ((((TabBarView)(object)this.widget).viewportFraction != ((TabBarView)oldWidget).viewportFraction))
        {
            this._pageController?.dispose();
            _pageController = new global::Doroti.Generated.Framework.Widgets.PageController(initialPage: DartRuntimePrimitives.RequireValue(this._currentIndex), viewportFraction: ((TabBarView)(object)this.widget).viewportFraction);
        }
        if (((!object.Equals(((TabBarView)(object)this.widget).children, ((TabBarView)oldWidget).children)) && (this._warpUnderwayCount == 0L)))
        {
            _updateChildren();
        }
    }

    public override void dispose()
    {
        if (this._controllerIsValid)
        {
            this._controller!.animation!.removeListener(() => this._handleTabControllerAnimationTick());
        }
        _controller = null;
        this._pageController?.dispose();
        base.dispose();
    }

    internal virtual void _updateChildren()
    {
        _childrenWithKey = KeyedSubtree.ensureUniqueKeysForList(((TabBarView)(object)this.widget).children.map<global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Widgets.Widget>(((child) => {
return new global::Doroti.Generated.Framework.Widgets.Semantics(role: SemanticsRole.tabPanel, child: child);
throw new InvalidOperationException("Dart closure completed without a value.");
})).ToList());
    }

    internal virtual void _handleTabControllerAnimationTick()
    {
        if (((this._scrollUnderwayCount > 0L) || !this._controller!.indexIsChanging))
        {
            return;
        }
        if ((this._controller!.index != this._currentIndex))
        {
            _currentIndex = this._controller!.index;
            _warpToCurrentIndex();
        }
    }

    internal virtual void _warpToCurrentIndex()
    {
        if ((!this.mounted || (this._pageController!.page == DartRuntimePrimitives.RequireValue(this._currentIndex).toDouble())))
        {
            return;
        }
        var adjacentDestination__87212 = (((DartRuntimePrimitives.RequireValue(this._currentIndex) - this._controller!.previousIndex)).abs() == 1L);
        if (adjacentDestination__87212)
        {
            DartRuntimePrimitives.Ignore(_warpToAdjacentTab(this._controller!.animationDuration));
        }
        else
        {
            DartRuntimePrimitives.Ignore(_warpToNonAdjacentTab(this._controller!.animationDuration));
        }
    }

    internal async virtual Future _warpToAdjacentTab(Duration duration)
    {
        if ((object.Equals(duration, Duration.zero)))
        {
            _jumpToPage(DartRuntimePrimitives.RequireValue(this._currentIndex));
        }
        else
        {
            await _animateToPage(DartRuntimePrimitives.RequireValue(this._currentIndex), duration: duration, curve: global::Doroti.Generated.Framework.Animation.Curves.ease);
        }
        if (this.mounted)
        {
            setState(((global::System.Action)(() => {
_updateChildren();
})));
        }
        await Future.value();
        return;
    }

    internal async virtual Future _warpToNonAdjacentTab(Duration duration)
    {
        long previousIndex__87900 = this._controller!.previousIndex;
        DartRuntimePrimitives.Assert(() => (((DartRuntimePrimitives.RequireValue(this._currentIndex) - previousIndex__87900)).abs() > 1L));
        long initialPage__88145 = ((DartRuntimePrimitives.RequireValue(this._currentIndex) > previousIndex__87900) ? (DartRuntimePrimitives.RequireValue(this._currentIndex) - 1L) : (DartRuntimePrimitives.RequireValue(this._currentIndex) + 1L));
        setState(((global::System.Action)(() => {
_childrenWithKey = new List<global::Doroti.Generated.Framework.Widgets.Widget>(this._childrenWithKey);
global::Doroti.Generated.Framework.Widgets.Widget temp__88604 = this._childrenWithKey[(int)(initialPage__88145)];
this._childrenWithKey[(int)(initialPage__88145)] = this._childrenWithKey[(int)(previousIndex__87900)];
this._childrenWithKey[(int)(previousIndex__87900)] = temp__88604;
})));
        _jumpToPage(initialPage__88145);
        if ((object.Equals(duration, Duration.zero)))
        {
            _jumpToPage(DartRuntimePrimitives.RequireValue(this._currentIndex));
        }
        else
        {
            await _animateToPage(DartRuntimePrimitives.RequireValue(this._currentIndex), duration: duration, curve: global::Doroti.Generated.Framework.Animation.Curves.ease);
        }
        if (this.mounted)
        {
            setState(((global::System.Action)(() => {
_updateChildren();
})));
        }
    }

    internal virtual void _syncControllerOffset()
    {
        this._controller!.offset = Dart_uiLibrary.clampDouble((DartRuntimePrimitives.RequireValue(this._pageController!.page) - this._controller!.index), -1.0, 1.0);
    }

    internal virtual bool _handleScrollNotification(global::Doroti.Generated.Framework.Widgets.ScrollNotification notification)
    {
        if (((this._warpUnderwayCount > 0L) || (this._scrollUnderwayCount > 0L)))
        {
            return false;
        }
        if ((notification.depth != 0L))
        {
            return false;
        }
        if (!this._controllerIsValid)
        {
            return false;
        }
        _scrollUnderwayCount += 1L;
        double page__89653 = DartRuntimePrimitives.RequireValue(this._pageController!.page);
        if (((notification is global::Doroti.Generated.Framework.Widgets.ScrollUpdateNotification) && !this._controller!.indexIsChanging))
        {
            global::Doroti.Generated.Framework.Widgets.ScrollUpdateNotification notification__as89692 = (global::Doroti.Generated.Framework.Widgets.ScrollUpdateNotification)notification;
            bool pageChanged__89786 = (((page__89653 - this._controller!.index)).abs() > 1.0);
            if (pageChanged__89786)
            {
                this._controller!.index = page__89653.round();
                _currentIndex = this._controller!.index;
            }
            _syncControllerOffset();
        }
        else
        {
            if ((notification is global::Doroti.Generated.Framework.Widgets.ScrollEndNotification))
            {
                global::Doroti.Generated.Framework.Widgets.ScrollEndNotification notification__as90007 = (global::Doroti.Generated.Framework.Widgets.ScrollEndNotification)notification;
                this._controller!.index = page__89653.round();
                _currentIndex = this._controller!.index;
                if (!this._controller!.indexIsChanging)
                {
                    _syncControllerOffset();
                }
            }
        }
        _scrollUnderwayCount -= 1L;
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _debugScheduleCheckHasValidChildrenCount()
    {
        if (this._debugHasScheduledValidChildrenCountCheck)
        {
            return true;
        }
        global::Doroti.Generated.Framework.Widgets.WidgetsBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((duration) => {
_debugHasScheduledValidChildrenCountCheck = false;
if (!this.mounted)
{
    return;
}
DartRuntimePrimitives.Assert(() =>
    {
        if ((this._controller!.length != checked((long)(((TabBarView)(object)this.widget).children.Count))))
        {
            throw DartRuntimePrimitives.AsException(global::Doroti.Generated.Framework.Foundation.FlutterError.Create($"Controller's length property ({this._controller!.length}) does not match the " + $"number of children ({checked((long)(((TabBarView)(object)this.widget).children.Count))}) present in TabBarView's children property."));
        }
        return true;
    });
})), debugLabel: "TabBarView.validChildrenCountCheck");
        _debugHasScheduledValidChildrenCountCheck = true;
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => _debugScheduleCheckHasValidChildrenCount());
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.NotificationListener<global::Doroti.Generated.Framework.Widgets.ScrollNotification>(onNotification: (global::System.Func<global::Doroti.Generated.Framework.Widgets.ScrollNotification, bool>)this._handleScrollNotification, child: new global::Doroti.Generated.Framework.Widgets.PageView(dragStartBehavior: ((TabBarView)(object)this.widget).dragStartBehavior, clipBehavior: ((TabBarView)(object)this.widget).clipBehavior, controller: this._pageController, physics: ((((TabBarView)(object)this.widget).physics is null) ? new global::Doroti.Generated.Framework.Widgets.PageScrollPhysics().applyTo(new global::Doroti.Generated.Framework.Widgets.ClampingScrollPhysics()) : new global::Doroti.Generated.Framework.Widgets.PageScrollPhysics().applyTo(((TabBarView)(object)this.widget).physics)), children: this._childrenWithKey)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class TabPageSelectorIndicator : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual Color backgroundColor { get; private set; } = default!;
    public virtual Color borderColor { get; private set; } = default!;
    public virtual double size { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.BorderStyle borderStyle { get; private set; } = default!;

    public TabPageSelectorIndicator(global::Doroti.Generated.Framework.Foundation.Key? key = null, Color backgroundColor = default!, Color borderColor = default!, double size = default!, global::Doroti.Generated.Framework.Painting.BorderStyle borderStyle = global::Doroti.Generated.Framework.Painting.BorderStyle.solid) : base(key: key)
    {
        this.backgroundColor = backgroundColor;
        this.borderColor = borderColor;
        this.size = size;
        this.borderStyle = borderStyle;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Container(width: this.size, height: this.size, margin: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateAll(4.0), decoration: new global::Doroti.Generated.Framework.Painting.BoxDecoration(color: this.backgroundColor, border: global::Doroti.Generated.Framework.Painting.Border.CreateAll(color: this.borderColor, style: this.borderStyle), shape: global::Doroti.Generated.Framework.Painting.BoxShape.circle)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class TabPageSelector : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual TabController? controller { get; private set; }
    public virtual double indicatorSize { get; private set; } = default!;
    public virtual Color? color { get; private set; }
    public virtual Color? selectedColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.BorderStyle? borderStyle { get; private set; }

    public TabPageSelector(global::Doroti.Generated.Framework.Foundation.Key? key = null, TabController? controller = null, double indicatorSize = 12.0, Color? color = null, Color? selectedColor = null, global::Doroti.Generated.Framework.Painting.BorderStyle? borderStyle = null) : base(key: key)
    {
        this.controller = controller;
        this.indicatorSize = indicatorSize;
        this.color = color;
        this.selectedColor = selectedColor;
        this.borderStyle = borderStyle;
        System.Diagnostics.Debug.Assert((indicatorSize > 0.0));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _TabPageSelectorState__tabs());
}

internal class _TabPageSelectorState__tabs : global::Doroti.Generated.Framework.Widgets.State<TabPageSelector>
{
    internal virtual TabController? _previousTabController { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation? _animation { get; set; } = default;

    internal virtual TabController _tabController
    {
        get
        {
            TabController? tabController__94675 = ((((TabPageSelector)(object)this.widget).controller ?? (TabController)DefaultTabController.maybeOf(this.context)));
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((tabController__94675 is null))
                    {
                        throw DartRuntimePrimitives.AsException(global::Doroti.Generated.Framework.Foundation.FlutterError.Create($"No TabController for {this.GetType()}.\n" + $"When creating a {this.GetType()}, you must either provide an explicit TabController " + "using the \"controller\" property, or you must ensure that there is a " + $"DefaultTabController above the {this.GetType()}.\n" + "In this case, there was neither an explicit controller nor a default controller."));
                    }
                    return true;
                });
            return tabController__94675!;
            return default!;
        }
    }
    public override void didUpdateWidget(TabPageSelector oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(this._previousTabController?.animation, ((TabController)this._tabController).animation)))
        {
            _setAnimation();
        }
        if ((!object.Equals(this._previousTabController, this._tabController)))
        {
            _previousTabController = this._tabController;
        }
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        if (((this._animation is null) || (!object.Equals(this._previousTabController?.animation, ((TabController)this._tabController).animation))))
        {
            _setAnimation();
        }
        if ((!object.Equals(this._previousTabController, this._tabController)))
        {
            _previousTabController = this._tabController;
        }
    }

    internal virtual void _setAnimation()
    {
        this._animation?.dispose();
        _animation = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: ((TabController)this._tabController).animation!, curve: global::Doroti.Generated.Framework.Animation.Curves.fastOutSlowIn);
    }

    public override void dispose()
    {
        this._animation?.dispose();
        base.dispose();
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildTabIndicator(long tabIndex, TabController tabController, global::Doroti.Generated.Framework.Animation.ColorTween selectedColorTween, global::Doroti.Generated.Framework.Animation.ColorTween previousColorTween)
    {
        global::Doroti.Flutter.Ui.Color background__96357 = default!;
        if (((TabController)tabController).indexIsChanging)
        {
            double t__96506 = (1.0 - TabsLibrary._indexChangeProgress(tabController));
            if ((((TabController)tabController).index == tabIndex))
            {
                background__96357 = selectedColorTween.lerp(t__96506)!;
            }
            else
            {
                if ((((TabController)tabController).previousIndex == tabIndex))
                {
                    background__96357 = previousColorTween.lerp(t__96506)!;
                }
                else
                {
                    background__96357 = selectedColorTween.begin!;
                }
            }
        }
        else
        {
            double offset__97019 = ((TabController)tabController).offset;
            if ((((TabController)tabController).index == tabIndex))
            {
                background__96357 = selectedColorTween.lerp((1.0 - offset__97019.abs()))!;
            }
            else
            {
                if (((((TabController)tabController).index == (tabIndex - 1L)) && (offset__97019 > 0.0)))
                {
                    background__96357 = selectedColorTween.lerp(offset__97019)!;
                }
                else
                {
                    if (((((TabController)tabController).index == (tabIndex + 1L)) && (offset__97019 < 0.0)))
                    {
                        background__96357 = selectedColorTween.lerp(-offset__97019)!;
                    }
                    else
                    {
                        background__96357 = selectedColorTween.begin!;
                    }
                }
            }
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new TabPageSelectorIndicator(backgroundColor: background__96357, borderColor: selectedColorTween.end!, size: ((TabPageSelector)(object)this.widget).indicatorSize, borderStyle: (((TabPageSelector)(object)this.widget).borderStyle ?? global::Doroti.Generated.Framework.Painting.BorderStyle.solid)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Flutter.Ui.Color fixColor__97783 = ((global::Doroti.Flutter.Ui.Color)(object?)(((TabPageSelector)(object)this.widget).color ?? Colors.transparent));
        global::Doroti.Flutter.Ui.Color fixSelectedColor__97846 = ((global::Doroti.Flutter.Ui.Color)(object?)(((TabPageSelector)(object)this.widget).selectedColor ?? Theme.of(context).colorScheme.secondary));
        var selectedColorTween__97940 = new global::Doroti.Generated.Framework.Animation.ColorTween(begin: fixColor__97783, end: fixSelectedColor__97846);
        var previousColorTween__98023 = new global::Doroti.Generated.Framework.Animation.ColorTween(begin: fixSelectedColor__97846, end: fixColor__97783);
        MaterialLocalizations localizations__98128 = MaterialLocalizations.of(context);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.AnimatedBuilder(animation: this._animation!, builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)((context, child) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Semantics(label: localizations__98128.tabLabel(tabIndex: (((TabController)this._tabController).index + 1L), tabCount: ((TabController)this._tabController).length), child: new global::Doroti.Generated.Framework.Widgets.Row(mainAxisSize: global::Doroti.Generated.Framework.Rendering.MainAxisSize.min, children: new List<global::Doroti.Generated.Framework.Widgets.Widget>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)((TabController)this._tabController).length)), ((tabIndex) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)_buildTabIndicator(tabIndex, this._tabController, selectedColorTween__97940, previousColorTween__98023));
throw new InvalidOperationException("Dart closure completed without a value.");
}))).ToList())));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TabsDefaultsM2__tabs : TabBarThemeData
{
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;
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
    private bool __late_isDark_initialized;
    private bool __late_isDark = default!;
    public virtual bool isDark
    {
        get
        {
            if (!__late_isDark_initialized)
            {
                __late_isDark = (object.Equals(Theme.brightnessOf(this.context), Brightness.dark));
                __late_isDark_initialized = true;
            }
            return __late_isDark;
        }
    }
    private bool __late_primaryColor_initialized;
    private Color __late_primaryColor = default!;
    public virtual Color primaryColor
    {
        get
        {
            if (!__late_primaryColor_initialized)
            {
                __late_primaryColor = (this.isDark ? Colors.grey[900L]! : Colors.blue);
                __late_primaryColor_initialized = true;
            }
            return __late_primaryColor;
        }
    }
    public virtual bool isScrollable { get; private set; } = default!;
    public static global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry iconMargin = ((global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry)(object?)global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(bottom: 10));

    internal _TabsDefaultsM2__tabs(global::Doroti.Generated.Framework.Widgets.BuildContext context, bool isScrollable) : base(indicatorSize: TabBarIndicatorSize.tab)
    {
        this.context = context;
        this.isScrollable = isScrollable;
    }

    public virtual global::Doroti.Flutter.Ui.Color? indicatorColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Color>(((object.Equals(this._colors.secondary, this.primaryColor)) ? Colors.white : this._colors.secondary));
    public virtual global::Doroti.Flutter.Ui.Color? labelColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Color>(Theme.of(this.context).primaryTextTheme.bodyLarge!.color!);
    public override global::Doroti.Generated.Framework.Painting.TextStyle? labelStyle => Theme.of(this.context).primaryTextTheme.bodyLarge;
    public override global::Doroti.Generated.Framework.Painting.TextStyle? unselectedLabelStyle => Theme.of(this.context).primaryTextTheme.bodyLarge;
    public override InteractiveInkFeatureFactory? splashFactory => Theme.of(this.context).splashFactory;
    public override TabAlignment? tabAlignment => (this.isScrollable ? TabAlignment.start : TabAlignment.fill);
}

internal class _TabsPrimaryDefaultsM3__tabs : TabBarThemeData
{
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;
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
    public virtual bool isScrollable { get; private set; } = default!;
    public static global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry iconMargin = ((global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry)(object?)global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(bottom: 2));

    internal _TabsPrimaryDefaultsM3__tabs(global::Doroti.Generated.Framework.Widgets.BuildContext context, bool isScrollable) : base(indicatorSize: TabBarIndicatorSize.label)
    {
        this.context = context;
        this.isScrollable = isScrollable;
    }

    public virtual global::Doroti.Flutter.Ui.Color? dividerColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Color>(this._colors.outlineVariant);
    public override double? dividerHeight => 1.0;
    public virtual global::Doroti.Flutter.Ui.Color? indicatorColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Color>(this._colors.primary);
    public virtual global::Doroti.Flutter.Ui.Color? labelColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Color>(this._colors.primary);
    public override global::Doroti.Generated.Framework.Painting.TextStyle? labelStyle => this._textTheme.titleSmall;
    public virtual global::Doroti.Flutter.Ui.Color? unselectedLabelColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Color>(this._colors.onSurfaceVariant);
    public override global::Doroti.Generated.Framework.Painting.TextStyle? unselectedLabelStyle => this._textTheme.titleSmall;
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?> overlayColor
    {
        get
        {
            return ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>)(object?)WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
    {
        return (this._colors.primary.withOpacity(0.1));
    }
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
    {
        return (this._colors.primary.withOpacity(0.08));
    }
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
    {
        return (this._colors.primary.withOpacity(0.1));
    }
    return null;
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
{
    return (this._colors.primary.withOpacity(0.1));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
{
    return (this._colors.onSurface.withOpacity(0.08));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
{
    return (this._colors.onSurface.withOpacity(0.1));
}
return null;
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            return default!;
        }
    }
    public override InteractiveInkFeatureFactory? splashFactory => Theme.of(this.context).splashFactory;
    public override TabAlignment? tabAlignment => (this.isScrollable ? TabAlignment.startOffset : TabAlignment.fill);
    public static double indicatorWeight(TabBarIndicatorSize indicatorSize)
    {
        return (indicatorSize switch { TabBarIndicatorSize.label => 3.0, TabBarIndicatorSize.tab => 2.0, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TabsSecondaryDefaultsM3__tabs : TabBarThemeData
{
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;
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
    public virtual bool isScrollable { get; private set; } = default!;
    public static double indicatorWeight = 2.0;

    internal _TabsSecondaryDefaultsM3__tabs(global::Doroti.Generated.Framework.Widgets.BuildContext context, bool isScrollable) : base(indicatorSize: TabBarIndicatorSize.tab)
    {
        this.context = context;
        this.isScrollable = isScrollable;
    }

    public virtual global::Doroti.Flutter.Ui.Color? dividerColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Color>(this._colors.outlineVariant);
    public override double? dividerHeight => 1.0;
    public virtual global::Doroti.Flutter.Ui.Color? indicatorColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Color>(this._colors.primary);
    public virtual global::Doroti.Flutter.Ui.Color? labelColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Color>(this._colors.onSurface);
    public override global::Doroti.Generated.Framework.Painting.TextStyle? labelStyle => this._textTheme.titleSmall;
    public virtual global::Doroti.Flutter.Ui.Color? unselectedLabelColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Color>(this._colors.onSurfaceVariant);
    public override global::Doroti.Generated.Framework.Painting.TextStyle? unselectedLabelStyle => this._textTheme.titleSmall;
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?> overlayColor
    {
        get
        {
            return ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>)(object?)WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
    {
        return (this._colors.onSurface.withOpacity(0.1));
    }
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
    {
        return (this._colors.onSurface.withOpacity(0.08));
    }
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
    {
        return (this._colors.onSurface.withOpacity(0.1));
    }
    return null;
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
{
    return (this._colors.onSurface.withOpacity(0.1));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
{
    return (this._colors.onSurface.withOpacity(0.08));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
{
    return (this._colors.onSurface.withOpacity(0.1));
}
return null;
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            return default!;
        }
    }
    public override InteractiveInkFeatureFactory? splashFactory => Theme.of(this.context).splashFactory;
    public override TabAlignment? tabAlignment => (this.isScrollable ? TabAlignment.startOffset : TabAlignment.fill);
}
