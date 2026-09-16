// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/badge.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class Badge : StatelessWidget
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? textColor { get; private set; }
    public virtual double? smallSize { get; private set; }
    public virtual double? largeSize { get; private set; }
    public virtual TextStyle? textStyle { get; private set; }
    public virtual EdgeInsetsGeometry? padding { get; private set; }
    public virtual AlignmentGeometry? alignment { get; private set; }
    public virtual Offset? offset { get; private set; }
    public virtual Widget? label { get; private set; }
    public virtual bool isLabelVisible { get; private set; } = default!;
    public virtual Widget? child { get; private set; }

    public Badge(Key? key = null, Color? backgroundColor = null, Color? textColor = null, double? smallSize = null, double? largeSize = null, TextStyle? textStyle = null, EdgeInsetsGeometry? padding = null, AlignmentGeometry? alignment = null, Offset? offset = null, Widget? label = null, bool isLabelVisible = true, Widget? child = null) : base(key: key)
    {
        this.backgroundColor = backgroundColor;
        this.textColor = textColor;
        this.smallSize = smallSize;
        this.largeSize = largeSize;
        this.textStyle = textStyle;
        this.padding = padding;
        this.alignment = alignment;
        this.offset = offset;
        this.label = label;
        this.isLabelVisible = isLabelVisible;
        this.child = child;
    }

    public static Badge CreateCount(Key? key = null, Color? backgroundColor = null, Color? textColor = null, double? smallSize = null, double? largeSize = null, TextStyle? textStyle = null, EdgeInsetsGeometry? padding = null, AlignmentGeometry? alignment = null, Offset? offset = null, long count = default!, long maxCount = 999, bool isLabelVisible = true, Widget? child = null)
    {
        var __instance = new Badge(key: key, backgroundColor: backgroundColor, textColor: textColor, smallSize: smallSize, largeSize: largeSize, textStyle: textStyle, padding: padding, alignment: alignment, offset: offset, isLabelVisible: isLabelVisible, child: child);
        __instance.backgroundColor = backgroundColor;
        __instance.textColor = textColor;
        __instance.smallSize = smallSize;
        __instance.largeSize = largeSize;
        __instance.textStyle = textStyle;
        __instance.padding = padding;
        __instance.alignment = alignment;
        __instance.offset = offset;
        __instance.isLabelVisible = isLabelVisible;
        __instance.child = child;
        __instance.label = new Text((count > maxCount) ? $"{maxCount}+" : $"{count}");
        return __instance;
    }

    public override Widget build(BuildContext context)
    {
        if (!isLabelVisible)
        {
            return child ?? new SizedBox();
        }
        BadgeThemeData badgeTheme = BadgeTheme.of(context);
        BadgeThemeData defaults = new _BadgeDefaultsM3__badge(context);
        Decoration effectiveDecoration = new ShapeDecoration(color: (backgroundColor ?? badgeTheme.backgroundColor) ?? defaults.backgroundColor!, shape: new StadiumBorder());
        double effectiveWidthOffset = default!;
        Widget badge = default!;
        var hasLabelLocal = label is not null;
        if (hasLabelLocal)
        {
            double minSizeLocal = effectiveWidthOffset = (largeSize ?? badgeTheme.largeSize) ?? DartRuntimePrimitives.RequireValue(defaults.largeSize);
            badge = DartRuntimePrimitives.ConvertValue<Widget>(new DefaultTextStyle(style: ((textStyle ?? badgeTheme.textStyle) ?? defaults.textStyle!).copyWith(color: (textColor ?? badgeTheme.textColor) ?? defaults.textColor!), child: new _IntrinsicHorizontalStadium__badge(minSize: minSizeLocal, child: new Container(clipBehavior: Clip.antiAlias, decoration: effectiveDecoration, padding: (padding ?? badgeTheme.padding) ?? defaults.padding!, alignment: Alignment.center, child: label))));
        }
        else
        {
            double effectiveSmallSize = effectiveWidthOffset = (smallSize ?? badgeTheme.smallSize) ?? DartRuntimePrimitives.RequireValue(defaults.smallSize);
            badge = DartRuntimePrimitives.ConvertValue<Widget>(new Container(width: effectiveSmallSize, height: effectiveSmallSize, clipBehavior: Clip.antiAlias, decoration: effectiveDecoration));
        }
        if (child is null)
        {
            return badge;
        }
        AlignmentGeometry effectiveAlignment = (alignment ?? badgeTheme.alignment) ?? defaults.alignment!;
        TextDirection textDirectionLocal = Directionality.of(context);
        var defaultOffset = Equals(textDirectionLocal, TextDirection.ltr) ? new Offset(4, -4) : new Offset(-4, -4);
        Offset effectiveOffset = ((offset ?? badgeTheme.offset) ?? defaultOffset) + new Offset(0, 8);
        return new Stack(clipBehavior: Clip.none, children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(child!), DartRuntimePrimitives.ConvertValue<Widget>(Positioned.CreateFill(child: new _Badge__badge(alignment: effectiveAlignment, offset: hasLabelLocal ? effectiveOffset : Offset.zero, hasLabel: hasLabelLocal, widthOffset: effectiveWidthOffset, textDirection: textDirectionLocal, child: badge))) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _Badge__badge : SingleChildRenderObjectWidget
{
    public virtual AlignmentGeometry alignment { get; private set; } = default!;
    public virtual Offset offset { get; private set; } = default!;
    public virtual double widthOffset { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;
    public virtual bool hasLabel { get; private set; } = default!;

    internal _Badge__badge(AlignmentGeometry alignment, Offset offset, double widthOffset, TextDirection textDirection, bool hasLabel, Widget? child = null) : base(child: child)
    {
        this.alignment = alignment;
        this.offset = offset;
        this.widthOffset = widthOffset;
        this.textDirection = textDirection;
        this.hasLabel = hasLabel;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderBadge__badge(alignment: alignment, widthOffset: widthOffset, hasLabel: hasLabel, offset: offset, textDirection: Directionality.maybeOf(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderBadge__badge)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderBadge__badge>)(() =>
{
    var __cascade = __renderObject;
    __cascade.alignment = alignment;
    __cascade.offset = offset;
    __cascade.widthOffset = widthOffset;
    __cascade.hasLabel = hasLabel;
    __cascade.textDirection = Directionality.maybeOf(context);
    return __cascade;
}))());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<AlignmentGeometry>("alignment", alignment));
        properties.add(new DiagnosticsProperty<Offset>("offset", offset));
    }

}

public class _RenderBadge__badge : RenderAligningShiftedBox
{
    internal virtual Offset _offset { get; set; } = default!;
    internal virtual bool _hasLabel { get; set; } = default!;
    internal virtual double _widthOffset { get; set; } = default!;

    internal _RenderBadge__badge(TextDirection? textDirection = null, AlignmentGeometry alignment = default!, Offset offset = default!, bool hasLabel = default!, double widthOffset = default!) : base(textDirection: textDirection, alignment: alignment ?? Alignment.center)
    {
        _offset = offset;
        _hasLabel = hasLabel;
        _widthOffset = widthOffset;
    }

    public virtual Offset offset
    {
        get => _offset;
        set
        {
            var __value = value;
            if (Equals(_offset, __value))
            {
                return;
            }
            _offset = __value;
            markNeedsLayout();
        }
    }
    public virtual bool hasLabel
    {
        get => _hasLabel;
        set
        {
            var __value = value;
            if (_hasLabel == __value)
            {
                return;
            }
            _hasLabel = __value;
            markNeedsLayout();
        }
    }
    public virtual double widthOffset
    {
        get => _widthOffset;
        set
        {
            var __value = value;
            if (_widthOffset == __value)
            {
                return;
            }
            _widthOffset = __value;
            markNeedsLayout();
        }
    }
    public override void performLayout()
    {
        BoxConstraints constraintsLocal = constraints;
        DartRuntimePrimitives.Assert(() => constraintsLocal.hasBoundedWidth);
        DartRuntimePrimitives.Assert(() => constraintsLocal.hasBoundedHeight);
        size = constraintsLocal.biggest;
        child!.layout(new BoxConstraints(), parentUsesSize: true);
        double badgeSize = child!.size.height;
        Alignment resolvedAlignment = alignment.resolve(textDirection);
        var childParentData = ((BoxParentData?)child!.parentData!)!;
        Offset badgeLocation = offset + resolvedAlignment.alongOffset(new Offset(size.width - widthOffset, size.height));
        if (hasLabel)
        {
            badgeLocation = badgeLocation - new Offset(0, badgeSize / 2L);
        }
        childParentData.offset = badgeLocation;
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return constraints.biggest;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return null;
        }
        var childConstraints = new BoxConstraints();
        double? childBaseline = childLocal.getDryBaseline(childConstraints, baseline);
        if (childBaseline is null)
        {
            return null;
        }
        Size mySize = getDryLayout(constraints);
        Alignment resolvedAlignment = alignment.resolve(textDirection);
        Size childSize = childLocal.getDryLayout(childConstraints);
        Offset badgeLocation = offset + resolvedAlignment.alongOffset(new Offset(mySize.width - widthOffset, mySize.height));
        if (hasLabel)
        {
            badgeLocation -= new Offset(0, childSize.height / 2L);
        }
        return DartRuntimePrimitives.RequireValue(childBaseline) + badgeLocation.dy;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _IntrinsicHorizontalStadium__badge : SingleChildRenderObjectWidget
{
    public virtual double minSize { get; private set; } = default!;

    internal _IntrinsicHorizontalStadium__badge(Widget? child = null, double minSize = default!) : base(child: child)
    {
        this.minSize = minSize;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderIntrinsicHorizontalStadium__badge(minSize: minSize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _RenderIntrinsicHorizontalStadium__badge : RenderProxyBox
{
    internal virtual double _minSize { get; set; } = default!;

    internal _RenderIntrinsicHorizontalStadium__badge(RenderBox? child = null, double minSize = default!) : base(child)
    {
        _minSize = minSize;
    }

    public virtual double minSize
    {
        get => _minSize;
        set
        {
            var __value = value;
            if (_minSize == __value)
            {
                return;
            }
            _minSize = __value;
            markNeedsLayout();
        }
    }
    public override double computeMinIntrinsicWidth(double height)
    {
        return getMaxIntrinsicWidth(height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        return Math.Max(getMaxIntrinsicHeight(double.PositiveInfinity), base.computeMaxIntrinsicWidth(height));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return getMaxIntrinsicHeight(width);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return Math.Max(minSize, base.computeMaxIntrinsicHeight(width));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual BoxConstraints _childConstraints(RenderBox child, BoxConstraints constraints)
    {
        double childHeight = Math.Max(minSize, child.getMaxIntrinsicHeight(constraints.maxWidth));
        double childWidth = child.getMaxIntrinsicWidth(constraints.maxHeight);
        return constraints.tighten(width: Math.Max(childWidth, childHeight), height: childHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Size _computeSize(Func<RenderBox, BoxConstraints, Size> layoutChild, BoxConstraints constraints)
    {
        RenderBox childLocal = child!;
        Size childSize = layoutChild(childLocal, _childConstraints(childLocal, constraints));
        if (childSize.height > childSize.width)
        {
            return new Size(childSize.height, childSize.height);
        }
        return childSize;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return _computeSize(layoutChild: ChildLayoutHelper.dryLayoutChild, constraints: constraints);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox childLocal = child!;
        return childLocal.getDryBaseline(_childConstraints(childLocal, constraints), baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        size = _computeSize(layoutChild: ChildLayoutHelper.layoutChild, constraints: constraints);
    }

}

internal class _BadgeDefaultsM3__badge : BadgeThemeData
{
    public virtual BuildContext context { get; private set; } = default!;
    private bool __late__theme_initialized;
    private ThemeData __late__theme = default!;
    internal virtual ThemeData _theme
    {
        get
        {
            if (!__late__theme_initialized)
            {
                __late__theme = Theme.of(context);
                __late__theme_initialized = true;
            }
            return __late__theme;
        }
    }
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = _theme.colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _BadgeDefaultsM3__badge(BuildContext context) : base(smallSize: 6.0, largeSize: 16.0, padding: EdgeInsets.CreateSymmetric(horizontal: 4), alignment: AlignmentDirectional.topEnd)
    {
        this.context = context;
    }

    public override Color? backgroundColor => DartRuntimePrimitives.ConvertValue<Color>(_colors.error);
    public override Color? textColor => DartRuntimePrimitives.ConvertValue<Color>(_colors.onError);
    public override TextStyle? textStyle => Theme.of(context).textTheme.labelSmall;
}
