// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/navigation_toolbar.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Widgets;

public class NavigationToolbar : StatelessWidget
{
    public const double kMiddleSpacing = 16.0;
    public virtual Widget? leading { get; private set; }
    public virtual Widget? middle { get; private set; }
    public virtual Widget? trailing { get; private set; }
    public virtual bool centerMiddle { get; private set; } = default!;
    public virtual double middleSpacing { get; private set; } = default!;

    public NavigationToolbar(global::Doroti.Framework.Foundation.Key? key = null, Widget? leading = null, Widget? middle = null, Widget? trailing = null, bool centerMiddle = true, double? middleSpacing = null) : base(key: key)
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
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        global::Doroti.Ui.TextDirection textDirection__2159 = Directionality.of(context);
        return ((Widget)(object?)new CustomMultiChildLayout(@delegate: new _ToolbarLayout__navigation_toolbar(centerMiddle: this.centerMiddle, middleSpacing: DartRuntimePrimitives.RequireValue(this.middleSpacing), textDirection: textDirection__2159), children: new List<Widget>()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal enum _ToolbarSlot__navigation_toolbar
{
    leading,
    middle,
    trailing
}

internal class _ToolbarLayout__navigation_toolbar : global::Doroti.Framework.Rendering.MultiChildLayoutDelegate
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
        var leadingWidth__3390 = 0.0;
        var trailingWidth__3418 = 0.0;
        if (hasChild(_ToolbarSlot__navigation_toolbar.leading))
        {
            var constraints__3494 = new global::Doroti.Framework.Rendering.BoxConstraints(maxWidth: size.width, minHeight: size.height, maxHeight: size.height);
            leadingWidth__3390 = layoutChild(_ToolbarSlot__navigation_toolbar.leading, constraints__3494).width;
            double leadingX__3776 = (this.textDirection switch { TextDirection.rtl => (size.width - leadingWidth__3390), TextDirection.ltr => 0.0, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            positionChild(_ToolbarSlot__navigation_toolbar.leading, new global::Doroti.Ui.Offset(leadingX__3776, 0.0));
        }
        if (hasChild(_ToolbarSlot__navigation_toolbar.trailing))
        {
            var constraints__4039 = global::Doroti.Framework.Rendering.BoxConstraints.CreateLoose(size);
            global::Doroti.Ui.Size trailingSize__4098 = ((global::Doroti.Ui.Size)(object?)layoutChild(_ToolbarSlot__navigation_toolbar.trailing, constraints__4039));
            double trailingX__4181 = (this.textDirection switch { TextDirection.rtl => 0.0, TextDirection.ltr => (size.width - trailingSize__4098.width), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            double trailingY__4342 = (((size.height - trailingSize__4098.height)) / 2.0);
            trailingWidth__3418 = trailingSize__4098.width;
            positionChild(_ToolbarSlot__navigation_toolbar.trailing, new global::Doroti.Ui.Offset(trailingX__4181, trailingY__4342));
        }
        if (hasChild(_ToolbarSlot__navigation_toolbar.middle))
        {
            double maxWidth__4580 = Math.Max((((size.width - leadingWidth__3390) - trailingWidth__3418) - (this.middleSpacing * 2.0)), 0.0);
            global::Doroti.Framework.Rendering.BoxConstraints constraints__4723 = ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)global::Doroti.Framework.Rendering.BoxConstraints.CreateLoose(size).copyWith(maxWidth: maxWidth__4580));
            global::Doroti.Ui.Size middleSize__4811 = ((global::Doroti.Ui.Size)(object?)layoutChild(_ToolbarSlot__navigation_toolbar.middle, constraints__4723));
            double middleStartMargin__4891 = (leadingWidth__3390 + this.middleSpacing);
            var middleStart__4951 = middleStartMargin__4891;
            double middleY__5003 = (((size.height - middleSize__4811.height)) / 2.0);
            if (this.centerMiddle)
            {
                middleStart__4951 = (((size.width - middleSize__4811.width)) / 2.0);
                if (((middleStart__4951 + middleSize__4811.width) > (size.width - trailingWidth__3418)))
                {
                    middleStart__4951 = (((size.width - trailingWidth__3418) - middleSize__4811.width) - this.middleSpacing);
                }
                else
                {
                    if ((middleStart__4951 < middleStartMargin__4891))
                    {
                        middleStart__4951 = middleStartMargin__4891;
                    }
                }
            }
            double middleX__5596 = (this.textDirection switch { TextDirection.rtl => ((size.width - middleSize__4811.width) - middleStart__4951), TextDirection.ltr => middleStart__4951, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            positionChild(_ToolbarSlot__navigation_toolbar.middle, new global::Doroti.Ui.Offset(middleX__5596, middleY__5003));
        }
    }

    public override bool shouldRelayout(global::Doroti.Framework.Rendering.MultiChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (_ToolbarLayout__navigation_toolbar)(object)oldDelegate;
        return (((((_ToolbarLayout__navigation_toolbar)__oldDelegate).centerMiddle != this.centerMiddle) || (((_ToolbarLayout__navigation_toolbar)__oldDelegate).middleSpacing != this.middleSpacing)) || (!object.Equals(((_ToolbarLayout__navigation_toolbar)__oldDelegate).textDirection, this.textDirection)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

