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
        global::Doroti.Ui.TextDirection textDirectionLocal = Directionality.of(context);
        var children = new List<Widget>();
        if (this.leading is not null)
            children.Add(new LayoutId(id: _ToolbarSlot__navigation_toolbar.leading, child: this.leading));
        if (this.middle is not null)
            children.Add(new LayoutId(id: _ToolbarSlot__navigation_toolbar.middle, child: this.middle));
        if (this.trailing is not null)
            children.Add(new LayoutId(id: _ToolbarSlot__navigation_toolbar.trailing, child: this.trailing));
        return ((Widget)(object?)new CustomMultiChildLayout(@delegate: new _ToolbarLayout__navigation_toolbar(centerMiddle: this.centerMiddle, middleSpacing: DartRuntimePrimitives.RequireValue(this.middleSpacing), textDirection: textDirectionLocal), children: children));
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
        var leadingWidth = 0.0;
        var trailingWidth = 0.0;
        if (hasChild(_ToolbarSlot__navigation_toolbar.leading))
        {
            var constraints = new global::Doroti.Framework.Rendering.BoxConstraints(maxWidth: size.width, minHeight: size.height, maxHeight: size.height);
            leadingWidth = layoutChild(_ToolbarSlot__navigation_toolbar.leading, constraints).width;
            double leadingX = (this.textDirection switch { TextDirection.rtl => (size.width - leadingWidth), TextDirection.ltr => 0.0, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            positionChild(_ToolbarSlot__navigation_toolbar.leading, new global::Doroti.Ui.Offset(leadingX, 0.0));
        }
        if (hasChild(_ToolbarSlot__navigation_toolbar.trailing))
        {
            var constraintsLocal = global::Doroti.Framework.Rendering.BoxConstraints.CreateLoose(size);
            global::Doroti.Ui.Size trailingSize = ((global::Doroti.Ui.Size)(object?)layoutChild(_ToolbarSlot__navigation_toolbar.trailing, constraintsLocal));
            double trailingX = (this.textDirection switch { TextDirection.rtl => 0.0, TextDirection.ltr => (size.width - trailingSize.width), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            double trailingY = (((size.height - trailingSize.height)) / 2.0);
            trailingWidth = trailingSize.width;
            positionChild(_ToolbarSlot__navigation_toolbar.trailing, new global::Doroti.Ui.Offset(trailingX, trailingY));
        }
        if (hasChild(_ToolbarSlot__navigation_toolbar.middle))
        {
            double maxWidthLocal = Math.Max((((size.width - leadingWidth) - trailingWidth) - (this.middleSpacing * 2.0)), 0.0);
            global::Doroti.Framework.Rendering.BoxConstraints constraintsAlternate = ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)global::Doroti.Framework.Rendering.BoxConstraints.CreateLoose(size).copyWith(maxWidth: maxWidthLocal));
            global::Doroti.Ui.Size middleSize = ((global::Doroti.Ui.Size)(object?)layoutChild(_ToolbarSlot__navigation_toolbar.middle, constraintsAlternate));
            double middleStartMargin = (leadingWidth + this.middleSpacing);
            var middleStart = middleStartMargin;
            double middleY = (((size.height - middleSize.height)) / 2.0);
            if (this.centerMiddle)
            {
                middleStart = (((size.width - middleSize.width)) / 2.0);
                if (((middleStart + middleSize.width) > (size.width - trailingWidth)))
                {
                    middleStart = (((size.width - trailingWidth) - middleSize.width) - this.middleSpacing);
                }
                else
                {
                    if ((middleStart < middleStartMargin))
                    {
                        middleStart = middleStartMargin;
                    }
                }
            }
            double middleX = (this.textDirection switch { TextDirection.rtl => ((size.width - middleSize.width) - middleStart), TextDirection.ltr => middleStart, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            positionChild(_ToolbarSlot__navigation_toolbar.middle, new global::Doroti.Ui.Offset(middleX, middleY));
        }
    }

    public override bool shouldRelayout(global::Doroti.Framework.Rendering.MultiChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (_ToolbarLayout__navigation_toolbar)(object)oldDelegate;
        return (((((_ToolbarLayout__navigation_toolbar)__oldDelegate).centerMiddle != this.centerMiddle) || (((_ToolbarLayout__navigation_toolbar)__oldDelegate).middleSpacing != this.middleSpacing)) || (!object.Equals(((_ToolbarLayout__navigation_toolbar)__oldDelegate).textDirection, this.textDirection)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
