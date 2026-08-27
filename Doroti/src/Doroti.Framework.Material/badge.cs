// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/badge.dart
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

public class Badge : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? textColor { get; private set; }
    public virtual double? smallSize { get; private set; }
    public virtual double? largeSize { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? textStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual global::Doroti.Framework.Painting.AlignmentGeometry? alignment { get; private set; }
    public virtual Offset? offset { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? label { get; private set; }
    public virtual bool isLabelVisible { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }

    public Badge(global::Doroti.Framework.Foundation.Key? key = null, Color? backgroundColor = null, Color? textColor = null, double? smallSize = null, double? largeSize = null, global::Doroti.Framework.Painting.TextStyle? textStyle = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Painting.AlignmentGeometry? alignment = null, Offset? offset = null, global::Doroti.Framework.Widgets.Widget? label = null, bool isLabelVisible = true, global::Doroti.Framework.Widgets.Widget? child = null) : base(key: key)
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

    public static Badge CreateCount(global::Doroti.Framework.Foundation.Key? key = null, Color? backgroundColor = null, Color? textColor = null, double? smallSize = null, double? largeSize = null, global::Doroti.Framework.Painting.TextStyle? textStyle = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Painting.AlignmentGeometry? alignment = null, Offset? offset = null, long count = default!, long maxCount = 999, bool isLabelVisible = true, global::Doroti.Framework.Widgets.Widget? child = null)
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
        __instance.label = new global::Doroti.Framework.Widgets.Text(((count > maxCount) ? $"{maxCount}+" : $"{count}"));
        return __instance;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if (!this.isLabelVisible)
        {
            return (this.child ?? new global::Doroti.Framework.Widgets.SizedBox());
        }
        BadgeThemeData badgeTheme = BadgeTheme.of(context);
        BadgeThemeData defaults = ((BadgeThemeData)(object?)new _BadgeDefaultsM3__badge(context));
        global::Doroti.Framework.Painting.Decoration effectiveDecoration = ((global::Doroti.Framework.Painting.Decoration)(object?)new global::Doroti.Framework.Painting.ShapeDecoration(color: ((this.backgroundColor ?? badgeTheme.backgroundColor) ?? defaults.backgroundColor!), shape: new global::Doroti.Framework.Painting.StadiumBorder()));
        double effectiveWidthOffset = default!;
        global::Doroti.Framework.Widgets.Widget badge = default!;
        var hasLabelLocal = (this.label is not null);
        if (hasLabelLocal)
        {
            double minSizeLocal = effectiveWidthOffset = ((this.largeSize ?? badgeTheme.largeSize) ?? DartRuntimePrimitives.RequireValue(defaults.largeSize));
            badge = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.DefaultTextStyle(style: (((this.textStyle ?? badgeTheme.textStyle) ?? defaults.textStyle!)).copyWith(color: ((this.textColor ?? badgeTheme.textColor) ?? defaults.textColor!)), child: new _IntrinsicHorizontalStadium__badge(minSize: minSizeLocal, child: new global::Doroti.Framework.Widgets.Container(clipBehavior: Clip.antiAlias, decoration: effectiveDecoration, padding: ((this.padding ?? badgeTheme.padding) ?? defaults.padding!), alignment: global::Doroti.Framework.Painting.Alignment.center, child: this.label))));
        }
        else
        {
            double effectiveSmallSize = effectiveWidthOffset = ((this.smallSize ?? badgeTheme.smallSize) ?? DartRuntimePrimitives.RequireValue(defaults.smallSize));
            badge = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Container(width: effectiveSmallSize, height: effectiveSmallSize, clipBehavior: Clip.antiAlias, decoration: effectiveDecoration));
        }
        if ((this.child is null))
        {
            return badge;
        }
        global::Doroti.Framework.Painting.AlignmentGeometry effectiveAlignment = ((this.alignment ?? badgeTheme.alignment) ?? defaults.alignment!);
        global::Doroti.Ui.TextDirection textDirectionLocal = Directionality.of(context);
        var defaultOffset = ((object.Equals(textDirectionLocal, TextDirection.ltr)) ? new global::Doroti.Ui.Offset(4, -4) : new global::Doroti.Ui.Offset(-4, -4));
        global::Doroti.Ui.Offset effectiveOffset = ((global::Doroti.Ui.Offset)(object?)((((this.offset ?? badgeTheme.offset) ?? defaultOffset)) + new global::Doroti.Ui.Offset(0, 8)));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Stack(clipBehavior: Clip.none, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(this.child!), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(global::Doroti.Framework.Widgets.Positioned.CreateFill(child: new _Badge__badge(alignment: effectiveAlignment, offset: (hasLabelLocal ? effectiveOffset : Offset.zero), hasLabel: hasLabelLocal, widthOffset: effectiveWidthOffset, textDirection: textDirectionLocal, child: badge))) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _Badge__badge : global::Doroti.Framework.Widgets.SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;
    public virtual Offset offset { get; private set; } = default!;
    public virtual double widthOffset { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;
    public virtual bool hasLabel { get; private set; } = default!;

    internal _Badge__badge(global::Doroti.Framework.Painting.AlignmentGeometry alignment, Offset offset, double widthOffset, TextDirection textDirection, bool hasLabel, global::Doroti.Framework.Widgets.Widget? child = null) : base(child: child)
    {
        this.alignment = alignment;
        this.offset = offset;
        this.widthOffset = widthOffset;
        this.textDirection = textDirection;
        this.hasLabel = hasLabel;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderBadge__badge(alignment: this.alignment, widthOffset: this.widthOffset, hasLabel: this.hasLabel, offset: this.offset, textDirection: Directionality.maybeOf(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderBadge__badge)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderBadge__badge>)(() =>
{
    var __cascade = __renderObject;
    __cascade.alignment = this.alignment;
    __cascade.offset = this.offset;
    __cascade.widthOffset = this.widthOffset;
    __cascade.hasLabel = this.hasLabel;
    __cascade.textDirection = Directionality.maybeOf(context);
    return __cascade;
}))());
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.AlignmentGeometry>("alignment", this.alignment));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Offset>("offset", this.offset));
    }

}

public class _RenderBadge__badge : global::Doroti.Framework.Rendering.RenderAligningShiftedBox
{
    internal virtual Offset _offset { get; set; } = default!;
    internal virtual bool _hasLabel { get; set; } = default!;
    internal virtual double _widthOffset { get; set; } = default!;

    internal _RenderBadge__badge(TextDirection? textDirection = null, global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, Offset offset = default!, bool hasLabel = default!, double widthOffset = default!) : base(textDirection: textDirection, alignment: alignment ?? global::Doroti.Framework.Painting.Alignment.center)
    {
        this._offset = offset;
        this._hasLabel = hasLabel;
        this._widthOffset = widthOffset;
    }

    public virtual global::Doroti.Ui.Offset offset
    {
        get => this._offset;
        set
        {
            var __value = value;
            if ((object.Equals(this._offset, __value)))
            {
                return;
            }
            _offset = __value;
            markNeedsLayout();
        }
    }
    public virtual bool hasLabel
    {
        get => this._hasLabel;
        set
        {
            var __value = value;
            if ((this._hasLabel == __value))
            {
                return;
            }
            _hasLabel = __value;
            markNeedsLayout();
        }
    }
    public virtual double widthOffset
    {
        get => this._widthOffset;
        set
        {
            var __value = value;
            if ((this._widthOffset == __value))
            {
                return;
            }
            _widthOffset = __value;
            markNeedsLayout();
        }
    }
    public override void performLayout()
    {
        global::Doroti.Framework.Rendering.BoxConstraints constraintsLocal = this.constraints;
        DartRuntimePrimitives.Assert(() => ((global::Doroti.Framework.Rendering.BoxConstraints)constraintsLocal).hasBoundedWidth);
        DartRuntimePrimitives.Assert(() => ((global::Doroti.Framework.Rendering.BoxConstraints)constraintsLocal).hasBoundedHeight);
        size = ((global::Doroti.Framework.Rendering.BoxConstraints)constraintsLocal).biggest;
        this.child!.layout(new global::Doroti.Framework.Rendering.BoxConstraints(), parentUsesSize: true);
        double badgeSize = this.child!.size.height;
        global::Doroti.Framework.Painting.Alignment resolvedAlignment = ((global::Doroti.Framework.Painting.Alignment)(object?)this.alignment.resolve(this.textDirection));
        var childParentData = ((global::Doroti.Framework.Rendering.BoxParentData?)(object?)this.child!.parentData!)!;
        global::Doroti.Ui.Offset badgeLocation = ((global::Doroti.Ui.Offset)(object?)(this.offset + resolvedAlignment.alongOffset(new global::Doroti.Ui.Offset((this.size.width - this.widthOffset), this.size.height))));
        if (this.hasLabel)
        {
            badgeLocation = (badgeLocation - new global::Doroti.Ui.Offset(0, (badgeSize / 2L)));
        }
        childParentData.offset = badgeLocation;
    }

    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).biggest;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((childLocal is null))
        {
            return null;
        }
        var childConstraints = new global::Doroti.Framework.Rendering.BoxConstraints();
        double? childBaseline = childLocal.getDryBaseline(childConstraints, baseline);
        if ((childBaseline is null))
        {
            return null;
        }
        global::Doroti.Ui.Size mySize = ((global::Doroti.Ui.Size)(object?)getDryLayout(constraints));
        global::Doroti.Framework.Painting.Alignment resolvedAlignment = ((global::Doroti.Framework.Painting.Alignment)(object?)this.alignment.resolve(this.textDirection));
        global::Doroti.Ui.Size childSize = ((global::Doroti.Ui.Size)(object?)childLocal.getDryLayout(childConstraints));
        global::Doroti.Ui.Offset badgeLocation = ((global::Doroti.Ui.Offset)(object?)(this.offset + resolvedAlignment.alongOffset(new global::Doroti.Ui.Offset((mySize.width - this.widthOffset), mySize.height))));
        if (this.hasLabel)
        {
            badgeLocation -= new global::Doroti.Ui.Offset(0, (childSize.height / 2L));
        }
        return (DartRuntimePrimitives.RequireValue(childBaseline) + badgeLocation.dy);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _IntrinsicHorizontalStadium__badge : global::Doroti.Framework.Widgets.SingleChildRenderObjectWidget
{
    public virtual double minSize { get; private set; } = default!;

    internal _IntrinsicHorizontalStadium__badge(global::Doroti.Framework.Widgets.Widget? child = null, double minSize = default!) : base(child: child)
    {
        this.minSize = minSize;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderIntrinsicHorizontalStadium__badge(minSize: this.minSize));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _RenderIntrinsicHorizontalStadium__badge : global::Doroti.Framework.Rendering.RenderProxyBox
{
    internal virtual double _minSize { get; set; } = default!;

    internal _RenderIntrinsicHorizontalStadium__badge(global::Doroti.Framework.Rendering.RenderBox? child = null, double minSize = default!) : base(child)
    {
        this._minSize = minSize;
    }

    public virtual double minSize
    {
        get => this._minSize;
        set
        {
            var __value = value;
            if ((this._minSize == __value))
            {
                return;
            }
            _minSize = __value;
            markNeedsLayout();
        }
    }
    public virtual double computeMinIntrinsicWidth(double height)
    {
        return getMaxIntrinsicWidth(height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double computeMaxIntrinsicWidth(double height)
    {
        return Math.Max(getMaxIntrinsicHeight(double.PositiveInfinity), base.computeMaxIntrinsicWidth(height));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double computeMinIntrinsicHeight(double width)
    {
        return getMaxIntrinsicHeight(width);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double computeMaxIntrinsicHeight(double width)
    {
        return Math.Max(this.minSize, base.computeMaxIntrinsicHeight(width));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Rendering.BoxConstraints _childConstraints(global::Doroti.Framework.Rendering.RenderBox child, global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        double childHeight = Math.Max(this.minSize, child.getMaxIntrinsicHeight(((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth));
        double childWidth = child.getMaxIntrinsicWidth(((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxHeight);
        return ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)constraints.tighten(width: Math.Max(childWidth, childHeight), height: childHeight));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Size _computeSize(global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size> layoutChild, global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        global::Doroti.Framework.Rendering.RenderBox childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child)!;
        global::Doroti.Ui.Size childSize = ((global::Doroti.Ui.Size)(object?)layoutChild(childLocal, _childConstraints(childLocal, constraints)));
        if ((childSize.height > childSize.width))
        {
            return new global::Doroti.Ui.Size(childSize.height, childSize.height);
        }
        return childSize;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return _computeSize(layoutChild: (global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size>)global::Doroti.Framework.Rendering.ChildLayoutHelper.dryLayoutChild, constraints: constraints);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Framework.Rendering.RenderBox childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child)!;
        return childLocal.getDryBaseline(_childConstraints(childLocal, constraints), baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void performLayout()
    {
        size = _computeSize(layoutChild: (global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size>)global::Doroti.Framework.Rendering.ChildLayoutHelper.layoutChild, constraints: this.constraints);
    }

}

internal class _BadgeDefaultsM3__badge : BadgeThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__theme_initialized;
    private ThemeData __late__theme = default!;
    internal virtual ThemeData _theme
    {
        get
        {
            if (!__late__theme_initialized)
            {
                __late__theme = Theme.of(this.context);
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
                __late__colors = this._theme.colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _BadgeDefaultsM3__badge(global::Doroti.Framework.Widgets.BuildContext context) : base(smallSize: 6.0, largeSize: 16.0, padding: global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 4), alignment: global::Doroti.Framework.Painting.AlignmentDirectional.topEnd)
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.error);
    public virtual global::Doroti.Ui.Color? textColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onError);
    public override global::Doroti.Framework.Painting.TextStyle? textStyle => Theme.of(this.context).textTheme.labelSmall;
}
