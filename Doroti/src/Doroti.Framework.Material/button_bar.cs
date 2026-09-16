// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/button_bar.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class ButtonBar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Rendering.MainAxisAlignment? alignment { get; private set; }
    public virtual global::Doroti.Framework.Rendering.MainAxisSize? mainAxisSize { get; private set; }
    public virtual ButtonTextTheme? buttonTextTheme { get; private set; }
    public virtual double? buttonMinWidth { get; private set; }
    public virtual double? buttonHeight { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? buttonPadding { get; private set; }
    public virtual bool? buttonAlignedDropdown { get; private set; }
    public virtual ButtonBarLayoutBehavior? layoutBehavior { get; private set; }
    public virtual global::Doroti.Framework.Painting.VerticalDirection? overflowDirection { get; private set; }
    public virtual double? overflowButtonSpacing { get; private set; }
    public virtual List<global::Doroti.Framework.Widgets.Widget> children { get; private set; } = default!;

    public ButtonBar(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Rendering.MainAxisAlignment? alignment = null, global::Doroti.Framework.Rendering.MainAxisSize? mainAxisSize = null, ButtonTextTheme? buttonTextTheme = null, double? buttonMinWidth = null, double? buttonHeight = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? buttonPadding = null, bool? buttonAlignedDropdown = null, ButtonBarLayoutBehavior? layoutBehavior = null, global::Doroti.Framework.Painting.VerticalDirection? overflowDirection = null, double? overflowButtonSpacing = null, List<global::Doroti.Framework.Widgets.Widget> children = default!) : base(key: key)
    {
        List<global::Doroti.Framework.Widgets.Widget> __children = children ?? new List<global::Doroti.Framework.Widgets.Widget>();
        this.alignment = alignment;
        this.mainAxisSize = mainAxisSize;
        this.buttonTextTheme = buttonTextTheme;
        this.buttonMinWidth = buttonMinWidth;
        this.buttonHeight = buttonHeight;
        this.buttonPadding = buttonPadding;
        this.buttonAlignedDropdown = buttonAlignedDropdown;
        this.layoutBehavior = layoutBehavior;
        this.overflowDirection = overflowDirection;
        this.overflowButtonSpacing = overflowButtonSpacing;
        this.children = __children;
        System.Diagnostics.Debug.Assert((buttonMinWidth is null) || (buttonMinWidth >= 0.0));
        System.Diagnostics.Debug.Assert((buttonHeight is null) || (buttonHeight >= 0.0));
        System.Diagnostics.Debug.Assert((overflowButtonSpacing is null) || (overflowButtonSpacing >= 0.0));
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ButtonThemeData parentButtonTheme = ButtonTheme.of(context);
        ButtonBarThemeData barTheme = ButtonBarTheme.of(context);
        ButtonThemeData buttonTheme = parentButtonTheme.copyWith(textTheme: (buttonTextTheme ?? barTheme.buttonTextTheme) ?? ButtonTextTheme.primary, minWidth: (buttonMinWidth ?? barTheme.buttonMinWidth) ?? 64.0, height: (buttonHeight ?? barTheme.buttonHeight) ?? 36.0, padding: (buttonPadding ?? barTheme.buttonPadding) ?? EdgeInsets.CreateSymmetric(horizontal: 8.0), alignedDropdown: (buttonAlignedDropdown ?? barTheme.buttonAlignedDropdown) ?? false, layoutBehavior: (layoutBehavior ?? barTheme.layoutBehavior) ?? ButtonBarLayoutBehavior.padded);
        double paddingUnit = buttonTheme.padding.horizontal / 4.0;
        global::Doroti.Framework.Widgets.Widget childLocal = ButtonTheme.CreateFromButtonThemeData(data: buttonTheme, child: new _ButtonBarRow__button_bar(mainAxisAlignment: (alignment ?? barTheme.alignment) ?? MainAxisAlignment.end, mainAxisSize: (mainAxisSize ?? barTheme.mainAxisSize) ?? MainAxisSize.max, overflowDirection: (overflowDirection ?? barTheme.overflowDirection) ?? VerticalDirection.down, overflowButtonSpacing: overflowButtonSpacing, children: children.map<global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Widgets.Widget>((child) =>
        {
            return new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsets.CreateSymmetric(horizontal: paddingUnit), child: child);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }).ToList()));
        switch (buttonTheme.layoutBehavior)
        {
            case var __constant10272 when Equals(__constant10272, ButtonBarLayoutBehavior.padded):
                {
                    return new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsets.CreateSymmetric(vertical: 2.0 * paddingUnit, horizontal: paddingUnit), child: childLocal);
                }
            case var __constant10469 when Equals(__constant10469, ButtonBarLayoutBehavior.constrained):
                {
                    return new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(minHeight: 52.0), child: new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsets.CreateSymmetric(horizontal: paddingUnit), child: new global::Doroti.Framework.Widgets.Center(child: childLocal)));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ButtonBarRow__button_bar : global::Doroti.Framework.Widgets.Flex
{
    public virtual double? overflowButtonSpacing { get; private set; }

    internal _ButtonBarRow__button_bar(List<global::Doroti.Framework.Widgets.Widget> children, global::Doroti.Framework.Rendering.MainAxisSize mainAxisSize = MainAxisSize.max, global::Doroti.Framework.Rendering.MainAxisAlignment mainAxisAlignment = MainAxisAlignment.start, global::Doroti.Framework.Painting.VerticalDirection overflowDirection = VerticalDirection.down, double? overflowButtonSpacing = null) : base(children: children, mainAxisSize: mainAxisSize, mainAxisAlignment: mainAxisAlignment, direction: Axis.horizontal, verticalDirection: overflowDirection)
    {
        this.overflowButtonSpacing = overflowButtonSpacing;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new _RenderButtonBarRow__button_bar(direction: direction, mainAxisAlignment: mainAxisAlignment, mainAxisSize: mainAxisSize, crossAxisAlignment: crossAxisAlignment, textDirection: DartRuntimePrimitives.RequireValue(getEffectiveTextDirection(context)), verticalDirection: verticalDirection, textBaseline: textBaseline, overflowButtonSpacing: overflowButtonSpacing);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderButtonBarRow__button_bar)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderButtonBarRow__button_bar>)(() =>
{
    var __cascade = __renderObject;
    __cascade.direction = direction;
    __cascade.mainAxisAlignment = mainAxisAlignment;
    __cascade.mainAxisSize = mainAxisSize;
    __cascade.crossAxisAlignment = crossAxisAlignment;
    __cascade.textDirection = getEffectiveTextDirection(context);
    __cascade.verticalDirection = verticalDirection;
    __cascade.textBaseline = textBaseline;
    __cascade.overflowButtonSpacing = overflowButtonSpacing;
    return __cascade;
}))());
    }

}

public class _RenderButtonBarRow__button_bar : global::Doroti.Framework.Rendering.RenderFlex
{
    internal virtual bool _hasCheckedLayoutWidth { get; set; } = false;
    public virtual double? overflowButtonSpacing { get; set; } = default;

    internal _RenderButtonBarRow__button_bar(global::Doroti.Framework.Painting.Axis direction = Axis.horizontal, global::Doroti.Framework.Rendering.MainAxisSize mainAxisSize = MainAxisSize.max, global::Doroti.Framework.Rendering.MainAxisAlignment mainAxisAlignment = MainAxisAlignment.start, global::Doroti.Framework.Rendering.CrossAxisAlignment crossAxisAlignment = CrossAxisAlignment.center, TextDirection textDirection = default!, global::Doroti.Framework.Painting.VerticalDirection verticalDirection = VerticalDirection.down, TextBaseline? textBaseline = null, double? overflowButtonSpacing = null) : base(direction: direction, mainAxisSize: mainAxisSize, mainAxisAlignment: mainAxisAlignment, crossAxisAlignment: crossAxisAlignment, textDirection: textDirection, verticalDirection: verticalDirection, textBaseline: textBaseline)
    {
        this.overflowButtonSpacing = overflowButtonSpacing;
        System.Diagnostics.Debug.Assert((overflowButtonSpacing is null) || (overflowButtonSpacing >= 0L));
    }

    public override global::Doroti.Framework.Rendering.BoxConstraints constraints
    {
        get
        {
            if (_hasCheckedLayoutWidth)
            {
                return base.constraints;
            }
            return base.constraints.copyWith(maxWidth: double.PositiveInfinity);
        }
    }
    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        global::Doroti.Ui.Size size = base.computeDryLayout(constraints.copyWith(maxWidth: double.PositiveInfinity));
        if (size.width <= constraints.maxWidth)
        {
            return base.computeDryLayout(constraints);
        }
        var currentHeight = 0.0;
        global::Doroti.Framework.Rendering.RenderBox? child = firstChild;
        while (child is not null)
        {
            global::Doroti.Framework.Rendering.BoxConstraints childConstraints = constraints.copyWith(minWidth: 0.0);
            global::Doroti.Ui.Size childSize = child.getDryLayout(childConstraints);
            currentHeight += childSize.height;
            child = childAfter(child);
            if ((overflowButtonSpacing is not null) && (child is not null))
            {
                double overflowButtonSpacing__value15257 = DartRuntimePrimitives.RequireValue(overflowButtonSpacing);
                currentHeight += DartRuntimePrimitives.RequireValue(overflowButtonSpacing);
            }
        }
        return constraints.constrain(new global::Doroti.Ui.Size(constraints.maxWidth, currentHeight));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        _hasCheckedLayoutWidth = false;
        base.performLayout();
        _hasCheckedLayoutWidth = true;
        if (size.width <= constraints.maxWidth)
        {
            base.performLayout();
        }
        else
        {
            global::Doroti.Framework.Rendering.BoxConstraints childConstraints = constraints.copyWith(minWidth: 0.0);
            var currentHeight = 0.0;
            global::Doroti.Framework.Rendering.RenderBox? child = verticalDirection switch { VerticalDirection.down => firstChild, VerticalDirection.up => lastChild, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
            while (child is not null)
            {
                var childParentData = ((global::Doroti.Framework.Rendering.FlexParentData?)child.parentData!)!;
                child.layout(childConstraints, parentUsesSize: true);
                switch (DartRuntimePrimitives.RequireValue(textDirection))
                {
                    case TextDirection.ltr:
                        {
                            switch (mainAxisAlignment)
                            {
                                case MainAxisAlignment.center:
                                    {
                                        double midpoint = (constraints.maxWidth - child.size.width) / 2.0;
                                        childParentData.offset = new global::Doroti.Ui.Offset(midpoint, currentHeight);
                                        break;
                                    }
                                case MainAxisAlignment.end:
                                    {
                                        childParentData.offset = new global::Doroti.Ui.Offset(constraints.maxWidth - child.size.width, currentHeight);
                                        break;
                                    }
                                case MainAxisAlignment.spaceAround:
                                case MainAxisAlignment.spaceBetween:
                                case MainAxisAlignment.spaceEvenly:
                                case MainAxisAlignment.start:
                                    {
                                        childParentData.offset = new global::Doroti.Ui.Offset(0, currentHeight);
                                        break;
                                    }
                            }
                            break;
                        }
                    case TextDirection.rtl:
                        {
                            switch (mainAxisAlignment)
                            {
                                case MainAxisAlignment.center:
                                    {
                                        double midpointLocal = (constraints.maxWidth / 2.0) - (child.size.width / 2.0);
                                        childParentData.offset = new global::Doroti.Ui.Offset(midpointLocal, currentHeight);
                                        break;
                                    }
                                case MainAxisAlignment.end:
                                    {
                                        childParentData.offset = new global::Doroti.Ui.Offset(0, currentHeight);
                                        break;
                                    }
                                case MainAxisAlignment.spaceAround:
                                case MainAxisAlignment.spaceBetween:
                                case MainAxisAlignment.spaceEvenly:
                                case MainAxisAlignment.start:
                                    {
                                        childParentData.offset = new global::Doroti.Ui.Offset(constraints.maxWidth - child.size.width, currentHeight);
                                        break;
                                    }
                            }
                            break;
                        }
                }
                currentHeight += child.size.height;
                child = verticalDirection switch { VerticalDirection.down => childParentData.nextSibling, VerticalDirection.up => childParentData.previousSibling, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
                if ((overflowButtonSpacing is not null) && (child is not null))
                {
                    double overflowButtonSpacing__value18856 = DartRuntimePrimitives.RequireValue(overflowButtonSpacing);
                    currentHeight += DartRuntimePrimitives.RequireValue(overflowButtonSpacing);
                }
            }
            size = constraints.constrain(new global::Doroti.Ui.Size(constraints.maxWidth, currentHeight));
        }
    }

}
