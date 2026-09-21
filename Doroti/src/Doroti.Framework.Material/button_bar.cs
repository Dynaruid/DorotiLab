// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/button_bar.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class ButtonBar : StatelessWidget
{
    public virtual MainAxisAlignment? alignment { get; private set; }
    public virtual MainAxisSize? mainAxisSize { get; private set; }
    public virtual ButtonTextTheme? buttonTextTheme { get; private set; }
    public virtual double? buttonMinWidth { get; private set; }
    public virtual double? buttonHeight { get; private set; }
    public virtual EdgeInsetsGeometry? buttonPadding { get; private set; }
    public virtual bool? buttonAlignedDropdown { get; private set; }
    public virtual ButtonBarLayoutBehavior? layoutBehavior { get; private set; }
    public virtual VerticalDirection? overflowDirection { get; private set; }
    public virtual double? overflowButtonSpacing { get; private set; }
    public virtual List<Widget> children { get; private set; } = default!;

    public ButtonBar(
        Key? key = null,
        MainAxisAlignment? alignment = null,
        MainAxisSize? mainAxisSize = null,
        ButtonTextTheme? buttonTextTheme = null,
        double? buttonMinWidth = null,
        double? buttonHeight = null,
        EdgeInsetsGeometry? buttonPadding = null,
        bool? buttonAlignedDropdown = null,
        ButtonBarLayoutBehavior? layoutBehavior = null,
        VerticalDirection? overflowDirection = null,
        double? overflowButtonSpacing = null,
        List<Widget> children = default!
    )
        : base(key: key)
    {
        List<Widget> __children = children ?? new List<Widget>();
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
        System.Diagnostics.Debug.Assert(
            (overflowButtonSpacing is null) || (overflowButtonSpacing >= 0.0)
        );
    }

    public override Widget build(BuildContext context)
    {
        ButtonThemeData parentButtonTheme = ButtonTheme.of(context);
        ButtonBarThemeData barTheme = ButtonBarTheme.of(context);
        ButtonThemeData buttonTheme = parentButtonTheme.copyWith(
            textTheme: (buttonTextTheme ?? barTheme.buttonTextTheme) ?? ButtonTextTheme.primary,
            minWidth: (buttonMinWidth ?? barTheme.buttonMinWidth) ?? 64.0,
            height: (buttonHeight ?? barTheme.buttonHeight) ?? 36.0,
            padding: (buttonPadding ?? barTheme.buttonPadding)
                ?? EdgeInsets.CreateSymmetric(horizontal: 8.0),
            alignedDropdown: (buttonAlignedDropdown ?? barTheme.buttonAlignedDropdown) ?? false,
            layoutBehavior: (layoutBehavior ?? barTheme.layoutBehavior)
                ?? ButtonBarLayoutBehavior.padded
        );
        double paddingUnit = buttonTheme.padding.horizontal / 4.0;
        Widget childLocal = ButtonTheme.CreateFromButtonThemeData(
            data: buttonTheme,
            child: new _ButtonBarRow__button_bar(
                mainAxisAlignment: (alignment ?? barTheme.alignment) ?? MainAxisAlignment.end,
                mainAxisSize: (mainAxisSize ?? barTheme.mainAxisSize) ?? MainAxisSize.max,
                overflowDirection: (overflowDirection ?? barTheme.overflowDirection)
                    ?? VerticalDirection.down,
                overflowButtonSpacing: overflowButtonSpacing,
                children: children
                    .map<Widget, Widget>(
                        (child) =>
                        {
                            return new Padding(
                                padding: EdgeInsets.CreateSymmetric(horizontal: paddingUnit),
                                child: child
                            );
                            throw new InvalidOperationException(
                                "Dart closure completed without a value."
                            );
                        }
                    )
                    .ToList()
            )
        );
        switch (buttonTheme.layoutBehavior)
        {
            case var __constant10272 when Equals(__constant10272, ButtonBarLayoutBehavior.padded):
            {
                return new Padding(
                    padding: EdgeInsets.CreateSymmetric(
                        vertical: 2.0 * paddingUnit,
                        horizontal: paddingUnit
                    ),
                    child: childLocal
                );
            }
            case var __constant10469
                when Equals(__constant10469, ButtonBarLayoutBehavior.constrained):
            {
                return new ConstrainedBox(
                    constraints: new BoxConstraints(minHeight: 52.0),
                    child: new Padding(
                        padding: EdgeInsets.CreateSymmetric(horizontal: paddingUnit),
                        child: new Center(child: childLocal)
                    )
                );
            }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _ButtonBarRow__button_bar : Flex
{
    public virtual double? overflowButtonSpacing { get; private set; }

    internal _ButtonBarRow__button_bar(
        List<Widget> children,
        MainAxisSize mainAxisSize = MainAxisSize.max,
        MainAxisAlignment mainAxisAlignment = MainAxisAlignment.start,
        VerticalDirection overflowDirection = VerticalDirection.down,
        double? overflowButtonSpacing = null
    )
        : base(
            children: children,
            mainAxisSize: mainAxisSize,
            mainAxisAlignment: mainAxisAlignment,
            direction: Axis.horizontal,
            verticalDirection: overflowDirection
        )
    {
        this.overflowButtonSpacing = overflowButtonSpacing;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderButtonBarRow__button_bar(
            direction: direction,
            mainAxisAlignment: mainAxisAlignment,
            mainAxisSize: mainAxisSize,
            crossAxisAlignment: crossAxisAlignment,
            textDirection: (
                getEffectiveTextDirection(context)
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ),
            verticalDirection: verticalDirection,
            textBaseline: textBaseline,
            overflowButtonSpacing: overflowButtonSpacing
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderButtonBarRow__button_bar)renderObject;
        DartRuntimePrimitives.Ignore(
            (
                (Func<_RenderButtonBarRow__button_bar>)(
                    () =>
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
                    }
                )
            )()
        );
    }
}

public class _RenderButtonBarRow__button_bar : RenderFlex
{
    internal virtual bool _hasCheckedLayoutWidth { get; set; } = false;
    public virtual double? overflowButtonSpacing { get; set; } = default;

    internal _RenderButtonBarRow__button_bar(
        Axis direction = Axis.horizontal,
        MainAxisSize mainAxisSize = MainAxisSize.max,
        MainAxisAlignment mainAxisAlignment = MainAxisAlignment.start,
        CrossAxisAlignment crossAxisAlignment = CrossAxisAlignment.center,
        TextDirection textDirection = default!,
        VerticalDirection verticalDirection = VerticalDirection.down,
        TextBaseline? textBaseline = null,
        double? overflowButtonSpacing = null
    )
        : base(
            direction: direction,
            mainAxisSize: mainAxisSize,
            mainAxisAlignment: mainAxisAlignment,
            crossAxisAlignment: crossAxisAlignment,
            textDirection: textDirection,
            verticalDirection: verticalDirection,
            textBaseline: textBaseline
        )
    {
        this.overflowButtonSpacing = overflowButtonSpacing;
        System.Diagnostics.Debug.Assert(
            (overflowButtonSpacing is null) || (overflowButtonSpacing >= 0L)
        );
    }

    public override BoxConstraints constraints
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

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        Size size = base.computeDryLayout(constraints.copyWith(maxWidth: double.PositiveInfinity));
        if (size.width <= constraints.maxWidth)
        {
            return base.computeDryLayout(constraints);
        }
        var currentHeight = 0.0;
        RenderBox? child = firstChild;
        while (child is not null)
        {
            BoxConstraints childConstraints = constraints.copyWith(minWidth: 0.0);
            Size childSize = child.getDryLayout(childConstraints);
            currentHeight += childSize.height;
            child = childAfter(child);
            if ((overflowButtonSpacing is not null) && (child is not null))
            {
                double overflowButtonSpacing__value15257 = (
                    overflowButtonSpacing
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                );
                currentHeight += (
                    overflowButtonSpacing
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                );
            }
        }
        return constraints.constrain(new Size(constraints.maxWidth, currentHeight));
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
            BoxConstraints childConstraints = constraints.copyWith(minWidth: 0.0);
            var currentHeight = 0.0;
            RenderBox? child = verticalDirection switch
            {
                VerticalDirection.down => firstChild,
                VerticalDirection.up => lastChild,
                _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                    throw new InvalidOperationException("Non-exhaustive Dart switch value."),
            };
            while (child is not null)
            {
                var childParentData = ((FlexParentData?)child.parentData!)!;
                child.layout(childConstraints, parentUsesSize: true);
                switch (
                    (
                        textDirection
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    )
                )
                {
                    case TextDirection.ltr:
                    {
                        switch (mainAxisAlignment)
                        {
                            case MainAxisAlignment.center:
                            {
                                double midpoint = (constraints.maxWidth - child.size.width) / 2.0;
                                childParentData.offset = new Offset(midpoint, currentHeight);
                                break;
                            }
                            case MainAxisAlignment.end:
                            {
                                childParentData.offset = new Offset(
                                    constraints.maxWidth - child.size.width,
                                    currentHeight
                                );
                                break;
                            }
                            case MainAxisAlignment.spaceAround:
                            case MainAxisAlignment.spaceBetween:
                            case MainAxisAlignment.spaceEvenly:
                            case MainAxisAlignment.start:
                            {
                                childParentData.offset = new Offset(0, currentHeight);
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
                                double midpointLocal =
                                    (constraints.maxWidth / 2.0) - (child.size.width / 2.0);
                                childParentData.offset = new Offset(midpointLocal, currentHeight);
                                break;
                            }
                            case MainAxisAlignment.end:
                            {
                                childParentData.offset = new Offset(0, currentHeight);
                                break;
                            }
                            case MainAxisAlignment.spaceAround:
                            case MainAxisAlignment.spaceBetween:
                            case MainAxisAlignment.spaceEvenly:
                            case MainAxisAlignment.start:
                            {
                                childParentData.offset = new Offset(
                                    constraints.maxWidth - child.size.width,
                                    currentHeight
                                );
                                break;
                            }
                        }
                        break;
                    }
                }
                currentHeight += child.size.height;
                child = verticalDirection switch
                {
                    VerticalDirection.down => childParentData.nextSibling,
                    VerticalDirection.up => childParentData.previousSibling,
                    _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                        throw new InvalidOperationException("Non-exhaustive Dart switch value."),
                };
                if ((overflowButtonSpacing is not null) && (child is not null))
                {
                    double overflowButtonSpacing__value18856 = (
                        overflowButtonSpacing
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    );
                    currentHeight += (
                        overflowButtonSpacing
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    );
                }
            }
            size = constraints.constrain(new Size(constraints.maxWidth, currentHeight));
        }
    }
}
