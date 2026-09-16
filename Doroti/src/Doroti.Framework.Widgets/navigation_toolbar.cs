// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/navigation_toolbar.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class NavigationToolbar : StatelessWidget
{
    public const double kMiddleSpacing = 16.0;
    public virtual Widget? leading { get; private set; }
    public virtual Widget? middle { get; private set; }
    public virtual Widget? trailing { get; private set; }
    public virtual bool centerMiddle { get; private set; } = default!;
    public virtual double middleSpacing { get; private set; } = default!;

    public NavigationToolbar(Key? key = null, Widget? leading = null, Widget? middle = null, Widget? trailing = null, bool centerMiddle = true, double? middleSpacing = null) : base(key: key)
    {
        double __middleSpacing = middleSpacing ?? kMiddleSpacing;
        this.leading = leading;
        this.middle = middle;
        this.trailing = trailing;
        this.centerMiddle = centerMiddle;
        this.middleSpacing = __middleSpacing;
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasDirectionality(context));
        TextDirection textDirectionLocal = Directionality.of(context);
        var children = new List<Widget>();
        if (leading is not null)
            children.Add(new LayoutId(id: _ToolbarSlot__navigation_toolbar.leading, child: leading));
        if (middle is not null)
            children.Add(new LayoutId(id: _ToolbarSlot__navigation_toolbar.middle, child: middle));
        if (trailing is not null)
            children.Add(new LayoutId(id: _ToolbarSlot__navigation_toolbar.trailing, child: trailing));
        return new CustomMultiChildLayout(@delegate: new _ToolbarLayout__navigation_toolbar(centerMiddle: centerMiddle, middleSpacing: DartRuntimePrimitives.RequireValue(middleSpacing), textDirection: textDirectionLocal), children: children);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal enum _ToolbarSlot__navigation_toolbar
{
    leading,
    middle,
    trailing
}

internal class _ToolbarLayout__navigation_toolbar : MultiChildLayoutDelegate
{
    public virtual bool centerMiddle { get; private set; } = default!;
    public virtual double middleSpacing { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;

    internal _ToolbarLayout__navigation_toolbar(bool centerMiddle, double middleSpacing, TextDirection textDirection)
    {
        this.centerMiddle = centerMiddle;
        this.middleSpacing = middleSpacing;
        this.textDirection = textDirection;
    }

    public override void performLayout(Size size)
    {
        var leadingWidth = 0.0;
        var trailingWidth = 0.0;
        if (hasChild(_ToolbarSlot__navigation_toolbar.leading))
        {
            var constraints = new BoxConstraints(maxWidth: size.width, minHeight: size.height, maxHeight: size.height);
            leadingWidth = layoutChild(_ToolbarSlot__navigation_toolbar.leading, constraints).width;
            double leadingX = textDirection switch { TextDirection.rtl => size.width - leadingWidth, TextDirection.ltr => 0.0, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
            positionChild(_ToolbarSlot__navigation_toolbar.leading, new Offset(leadingX, 0.0));
        }
        if (hasChild(_ToolbarSlot__navigation_toolbar.trailing))
        {
            var constraintsLocal = BoxConstraints.CreateLoose(size);
            Size trailingSize = layoutChild(_ToolbarSlot__navigation_toolbar.trailing, constraintsLocal);
            double trailingX = textDirection switch { TextDirection.rtl => 0.0, TextDirection.ltr => size.width - trailingSize.width, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
            double trailingY = (size.height - trailingSize.height) / 2.0;
            trailingWidth = trailingSize.width;
            positionChild(_ToolbarSlot__navigation_toolbar.trailing, new Offset(trailingX, trailingY));
        }
        if (hasChild(_ToolbarSlot__navigation_toolbar.middle))
        {
            double maxWidthLocal = Math.Max(size.width - leadingWidth - trailingWidth - (middleSpacing * 2.0), 0.0);
            BoxConstraints constraintsAlternate = BoxConstraints.CreateLoose(size).copyWith(maxWidth: maxWidthLocal);
            Size middleSize = layoutChild(_ToolbarSlot__navigation_toolbar.middle, constraintsAlternate);
            double middleStartMargin = leadingWidth + middleSpacing;
            var middleStart = middleStartMargin;
            double middleY = (size.height - middleSize.height) / 2.0;
            if (centerMiddle)
            {
                middleStart = (size.width - middleSize.width) / 2.0;
                if ((middleStart + middleSize.width) > (size.width - trailingWidth))
                {
                    middleStart = size.width - trailingWidth - middleSize.width - middleSpacing;
                }
                else
                {
                    if (middleStart < middleStartMargin)
                    {
                        middleStart = middleStartMargin;
                    }
                }
            }
            double middleX = textDirection switch { TextDirection.rtl => size.width - middleSize.width - middleStart, TextDirection.ltr => middleStart, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
            positionChild(_ToolbarSlot__navigation_toolbar.middle, new Offset(middleX, middleY));
        }
    }

    public override bool shouldRelayout(MultiChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (_ToolbarLayout__navigation_toolbar)oldDelegate;
        return (__oldDelegate.centerMiddle != centerMiddle) || (__oldDelegate.middleSpacing != middleSpacing) || (!Equals(__oldDelegate.textDirection, textDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
