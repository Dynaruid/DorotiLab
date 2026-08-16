// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/button_bar.dart
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
        System.Diagnostics.Debug.Assert(((buttonMinWidth is null) || (buttonMinWidth >= 0.0)));
        System.Diagnostics.Debug.Assert(((buttonHeight is null) || (buttonHeight >= 0.0)));
        System.Diagnostics.Debug.Assert(((overflowButtonSpacing is null) || (overflowButtonSpacing >= 0.0)));
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ButtonThemeData parentButtonTheme__8680 = ButtonTheme.of(context);
        ButtonBarThemeData barTheme__8754 = ButtonBarTheme.of(context);
        ButtonThemeData buttonTheme__8820 = parentButtonTheme__8680.copyWith(textTheme: ((this.buttonTextTheme ?? barTheme__8754.buttonTextTheme) ?? ButtonTextTheme.primary), minWidth: ((this.buttonMinWidth ?? barTheme__8754.buttonMinWidth) ?? 64.0), height: ((this.buttonHeight ?? barTheme__8754.buttonHeight) ?? 36.0), padding: ((this.buttonPadding ?? barTheme__8754.buttonPadding) ?? global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 8.0)), alignedDropdown: ((this.buttonAlignedDropdown ?? barTheme__8754.buttonAlignedDropdown) ?? false), layoutBehavior: ((this.layoutBehavior ?? barTheme__8754.layoutBehavior) ?? ButtonBarLayoutBehavior.padded));
        double paddingUnit__9496 = (buttonTheme__8820.padding.horizontal / 4.0);
        global::Doroti.Framework.Widgets.Widget child__9565 = ((global::Doroti.Framework.Widgets.Widget)(object?)ButtonTheme.CreateFromButtonThemeData(data: buttonTheme__8820, child: new _ButtonBarRow__button_bar(mainAxisAlignment: ((this.alignment ?? barTheme__8754.alignment) ?? global::Doroti.Framework.Rendering.MainAxisAlignment.end), mainAxisSize: ((this.mainAxisSize ?? barTheme__8754.mainAxisSize) ?? global::Doroti.Framework.Rendering.MainAxisSize.max), overflowDirection: ((this.overflowDirection ?? barTheme__8754.overflowDirection) ?? global::Doroti.Framework.Painting.VerticalDirection.down), overflowButtonSpacing: this.overflowButtonSpacing, children: this.children.map<global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Widgets.Widget>(((child) => {
return new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: paddingUnit__9496), child: child);
throw new InvalidOperationException("Dart closure completed without a value.");
})).ToList())));
        switch (buttonTheme__8820.layoutBehavior)
        {
            case var __constant10272 when (object.Equals(__constant10272, ButtonBarLayoutBehavior.padded)):
                {
                    return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(vertical: (2.0 * paddingUnit__9496), horizontal: paddingUnit__9496), child: child__9565));
                }
            case var __constant10469 when (object.Equals(__constant10469, ButtonBarLayoutBehavior.constrained)):
                {
                    return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(minHeight: 52.0), child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: paddingUnit__9496), child: new global::Doroti.Framework.Widgets.Center(child: child__9565))));
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

    internal _ButtonBarRow__button_bar(List<global::Doroti.Framework.Widgets.Widget> children, global::Doroti.Framework.Rendering.MainAxisSize mainAxisSize = global::Doroti.Framework.Rendering.MainAxisSize.max, global::Doroti.Framework.Rendering.MainAxisAlignment mainAxisAlignment = global::Doroti.Framework.Rendering.MainAxisAlignment.start, global::Doroti.Framework.Painting.VerticalDirection overflowDirection = global::Doroti.Framework.Painting.VerticalDirection.down, double? overflowButtonSpacing = null) : base(children: children, mainAxisSize: mainAxisSize, mainAxisAlignment: mainAxisAlignment, direction: global::Doroti.Framework.Painting.Axis.horizontal, verticalDirection: overflowDirection)
    {
        this.overflowButtonSpacing = overflowButtonSpacing;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderButtonBarRow__button_bar(direction: this.direction, mainAxisAlignment: this.mainAxisAlignment, mainAxisSize: this.mainAxisSize, crossAxisAlignment: this.crossAxisAlignment, textDirection: DartRuntimePrimitives.RequireValue(getEffectiveTextDirection(context)), verticalDirection: this.verticalDirection, textBaseline: this.textBaseline, overflowButtonSpacing: this.overflowButtonSpacing));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderButtonBarRow__button_bar)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderButtonBarRow__button_bar>)(() =>
{            var __cascade = __renderObject;
            __cascade.direction = this.direction;
            __cascade.mainAxisAlignment = this.mainAxisAlignment;
            __cascade.mainAxisSize = this.mainAxisSize;
            __cascade.crossAxisAlignment = this.crossAxisAlignment;
            __cascade.textDirection = getEffectiveTextDirection(context);
            __cascade.verticalDirection = this.verticalDirection;
            __cascade.textBaseline = this.textBaseline;
            __cascade.overflowButtonSpacing = this.overflowButtonSpacing;
            return __cascade;        }))());
    }

}

public class _RenderButtonBarRow__button_bar : global::Doroti.Framework.Rendering.RenderFlex
{
    internal virtual bool _hasCheckedLayoutWidth { get; set; } = false;
    public virtual double? overflowButtonSpacing { get; set; } = default;

    internal _RenderButtonBarRow__button_bar(global::Doroti.Framework.Painting.Axis direction = global::Doroti.Framework.Painting.Axis.horizontal, global::Doroti.Framework.Rendering.MainAxisSize mainAxisSize = global::Doroti.Framework.Rendering.MainAxisSize.max, global::Doroti.Framework.Rendering.MainAxisAlignment mainAxisAlignment = global::Doroti.Framework.Rendering.MainAxisAlignment.start, global::Doroti.Framework.Rendering.CrossAxisAlignment crossAxisAlignment = global::Doroti.Framework.Rendering.CrossAxisAlignment.center, TextDirection textDirection = default!, global::Doroti.Framework.Painting.VerticalDirection verticalDirection = global::Doroti.Framework.Painting.VerticalDirection.down, TextBaseline? textBaseline = null, double? overflowButtonSpacing = null) : base(direction: direction, mainAxisSize: mainAxisSize, mainAxisAlignment: mainAxisAlignment, crossAxisAlignment: crossAxisAlignment, textDirection: textDirection, verticalDirection: verticalDirection, textBaseline: textBaseline)
    {
        this.overflowButtonSpacing = overflowButtonSpacing;
        System.Diagnostics.Debug.Assert(((overflowButtonSpacing is null) || (overflowButtonSpacing >= 0L)));
    }

    public override global::Doroti.Framework.Rendering.BoxConstraints constraints
    {
        get
        {
            if (this._hasCheckedLayoutWidth)
            {
                return base.constraints;
            }
            return ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)base.constraints.copyWith(maxWidth: double.PositiveInfinity));
            return default!;
        }
    }
    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        global::Doroti.Ui.Size size__14749 = ((global::Doroti.Ui.Size)(object?)base.computeDryLayout(constraints.copyWith(maxWidth: double.PositiveInfinity)));
        if ((size__14749.width <= ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth))
        {
            return base.computeDryLayout(constraints);
        }
        var currentHeight__14939 = 0.0;
        global::Doroti.Framework.Rendering.RenderBox? child__14975 = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)this.firstChild);
        while ((child__14975 is not null))
        {
            global::Doroti.Framework.Rendering.BoxConstraints childConstraints__15050 = ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)constraints.copyWith(minWidth: 0.0));
            global::Doroti.Ui.Size childSize__15123 = ((global::Doroti.Ui.Size)(object?)child__14975.getDryLayout(childConstraints__15050));
            currentHeight__14939 += childSize__15123.height;
            child__14975 = childAfter(child__14975);
            if (((this.overflowButtonSpacing is not null) && (child__14975 is not null)))
            {
                double overflowButtonSpacing__value15257 = DartRuntimePrimitives.RequireValue(overflowButtonSpacing);
                currentHeight__14939 += DartRuntimePrimitives.RequireValue(this.overflowButtonSpacing);
            }
        }
        return constraints.constrain(new global::Doroti.Ui.Size(((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth, currentHeight__14939));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        _hasCheckedLayoutWidth = false;
        base.performLayout();
        _hasCheckedLayoutWidth = true;
        if ((this.size.width <= ((global::Doroti.Framework.Rendering.BoxConstraints)this.constraints).maxWidth))
        {
            base.performLayout();
        }
        else
        {
            global::Doroti.Framework.Rendering.BoxConstraints childConstraints__16242 = ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)this.constraints.copyWith(minWidth: 0.0));
            var currentHeight__16308 = 0.0;
            global::Doroti.Framework.Rendering.RenderBox? child__16346 = (this.verticalDirection switch { global::Doroti.Framework.Painting.VerticalDirection.down => this.firstChild, global::Doroti.Framework.Painting.VerticalDirection.up => this.lastChild, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            while ((child__16346 is not null))
            {
                var childParentData__16526 = ((global::Doroti.Framework.Rendering.FlexParentData?)(object?)child__16346.parentData!)!;
                child__16346.layout(childConstraints__16242, parentUsesSize: true);
                switch (DartRuntimePrimitives.RequireValue(this.textDirection))
                {
                    case TextDirection.ltr:
                        {
                            switch (this.mainAxisAlignment)
                            {
                                case global::Doroti.Framework.Rendering.MainAxisAlignment.center:
                                    {
                                        double midpoint__17224 = (((((global::Doroti.Framework.Rendering.BoxConstraints)this.constraints).maxWidth - ((global::Doroti.Framework.Rendering.RenderBox)child__16346).size.width)) / 2.0);
                                        childParentData__16526.offset = new global::Doroti.Ui.Offset(midpoint__17224, currentHeight__16308);
                                        break;
                                    }
                                case global::Doroti.Framework.Rendering.MainAxisAlignment.end:
                                    {
                                        childParentData__16526.offset = new global::Doroti.Ui.Offset((((global::Doroti.Framework.Rendering.BoxConstraints)this.constraints).maxWidth - ((global::Doroti.Framework.Rendering.RenderBox)child__16346).size.width), currentHeight__16308);
                                        break;
                                    }
                                case global::Doroti.Framework.Rendering.MainAxisAlignment.spaceAround:
                                case global::Doroti.Framework.Rendering.MainAxisAlignment.spaceBetween:
                                case global::Doroti.Framework.Rendering.MainAxisAlignment.spaceEvenly:
                                case global::Doroti.Framework.Rendering.MainAxisAlignment.start:
                                    {
                                        childParentData__16526.offset = new global::Doroti.Ui.Offset(0, currentHeight__16308);
                                        break;
                                    }
                            }
                            break;
                        }
                    case TextDirection.rtl:
                        {
                            switch (this.mainAxisAlignment)
                            {
                                case global::Doroti.Framework.Rendering.MainAxisAlignment.center:
                                    {
                                        double midpoint__17985 = ((((global::Doroti.Framework.Rendering.BoxConstraints)this.constraints).maxWidth / 2.0) - (((global::Doroti.Framework.Rendering.RenderBox)child__16346).size.width / 2.0));
                                        childParentData__16526.offset = new global::Doroti.Ui.Offset(midpoint__17985, currentHeight__16308);
                                        break;
                                    }
                                case global::Doroti.Framework.Rendering.MainAxisAlignment.end:
                                    {
                                        childParentData__16526.offset = new global::Doroti.Ui.Offset(0, currentHeight__16308);
                                        break;
                                    }
                                case global::Doroti.Framework.Rendering.MainAxisAlignment.spaceAround:
                                case global::Doroti.Framework.Rendering.MainAxisAlignment.spaceBetween:
                                case global::Doroti.Framework.Rendering.MainAxisAlignment.spaceEvenly:
                                case global::Doroti.Framework.Rendering.MainAxisAlignment.start:
                                    {
                                        childParentData__16526.offset = new global::Doroti.Ui.Offset((((global::Doroti.Framework.Rendering.BoxConstraints)this.constraints).maxWidth - ((global::Doroti.Framework.Rendering.RenderBox)child__16346).size.width), currentHeight__16308);
                                        break;
                                    }
                            }
                            break;
                        }
                }
                currentHeight__16308 += ((global::Doroti.Framework.Rendering.RenderBox)child__16346).size.height;
                child__16346 = (this.verticalDirection switch { global::Doroti.Framework.Painting.VerticalDirection.down => childParentData__16526.nextSibling, global::Doroti.Framework.Painting.VerticalDirection.up => childParentData__16526.previousSibling, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
                if (((this.overflowButtonSpacing is not null) && (child__16346 is not null)))
                {
                    double overflowButtonSpacing__value18856 = DartRuntimePrimitives.RequireValue(overflowButtonSpacing);
                    currentHeight__16308 += DartRuntimePrimitives.RequireValue(this.overflowButtonSpacing);
                }
            }
            this.size = this.constraints.constrain(new global::Doroti.Ui.Size(((global::Doroti.Framework.Rendering.BoxConstraints)this.constraints).maxWidth, currentHeight__16308));
        }
    }

}
