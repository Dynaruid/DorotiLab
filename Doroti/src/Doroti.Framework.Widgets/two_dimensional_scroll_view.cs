// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/two_dimensional_scroll_view.dart
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

public abstract class TwoDimensionalScrollView : StatelessWidget
{
    public virtual TwoDimensionalChildDelegate @delegate { get; private set; } = default!;
    public virtual double? cacheExtent { get; private set; }
    public virtual global::Doroti.Framework.Rendering.CacheExtentStyle? cacheExtentStyle { get; private set; }
    public virtual global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent { get; private set; }
    public virtual DiagonalDragBehavior diagonalDragBehavior { get; private set; } = default!;
    public virtual bool? primary { get; private set; }
    public virtual global::Doroti.Framework.Painting.Axis mainAxis { get; private set; } = default!;
    public virtual ScrollableDetails verticalDetails { get; private set; } = default!;
    public virtual ScrollableDetails horizontalDetails { get; private set; } = default!;
    public virtual global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior { get; private set; }
    public virtual global::Doroti.Framework.Rendering.HitTestBehavior hitTestBehavior { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;

    protected TwoDimensionalScrollView(global::Doroti.Framework.Foundation.Key? key = null, bool? primary = null, global::Doroti.Framework.Painting.Axis mainAxis = global::Doroti.Framework.Painting.Axis.vertical, ScrollableDetails verticalDetails = default!, ScrollableDetails horizontalDetails = default!, TwoDimensionalChildDelegate @delegate = default!, double? cacheExtent = null, global::Doroti.Framework.Rendering.CacheExtentStyle? cacheExtentStyle = null, global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, DiagonalDragBehavior diagonalDragBehavior = DiagonalDragBehavior.none, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.start, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Framework.Rendering.HitTestBehavior.opaque) : base(key: key)
    {
        ScrollableDetails __verticalDetails = verticalDetails ?? ScrollableDetails.CreateVertical();
        ScrollableDetails __horizontalDetails = horizontalDetails ?? ScrollableDetails.CreateHorizontal();
        this.primary = primary;
        this.mainAxis = mainAxis;
        this.verticalDetails = __verticalDetails;
        this.horizontalDetails = __horizontalDetails;
        this.@delegate = @delegate;
        this.cacheExtent = cacheExtent;
        this.cacheExtentStyle = cacheExtentStyle;
        this.scrollCacheExtent = scrollCacheExtent;
        this.diagonalDragBehavior = diagonalDragBehavior;
        this.dragStartBehavior = dragStartBehavior;
        this.keyboardDismissBehavior = keyboardDismissBehavior;
        this.clipBehavior = clipBehavior;
        this.hitTestBehavior = hitTestBehavior;
    }

    public abstract Widget buildViewport(BuildContext context, global::Doroti.Framework.Rendering.ViewportOffset verticalOffset, global::Doroti.Framework.Rendering.ViewportOffset horizontalOffset);
    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(((ScrollableDetails)this.verticalDetails).direction), global::Doroti.Framework.Painting.Axis.vertical)), () => (object?)"TwoDimensionalScrollView.verticalDetails are not Axis.vertical.");
        DartRuntimePrimitives.Assert(() => (object.Equals(global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(((ScrollableDetails)this.horizontalDetails).direction), global::Doroti.Framework.Painting.Axis.horizontal)), () => (object?)"TwoDimensionalScrollView.horizontalDetails are not Axis.horizontal.");
        ScrollableDetails mainAxisDetails = (this.mainAxis switch { global::Doroti.Framework.Painting.Axis.vertical => this.verticalDetails, global::Doroti.Framework.Painting.Axis.horizontal => this.horizontalDetails, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        bool effectivePrimary = (this.primary ?? ((((ScrollableDetails)mainAxisDetails).controller is null) && PrimaryScrollController.shouldInherit(context, this.mainAxis)));
        if (effectivePrimary)
        {
            DartRuntimePrimitives.Assert(() => (((ScrollableDetails)mainAxisDetails).controller is null), () => (object?)"TwoDimensionalScrollView.primary was explicitly set to true, but a " + "ScrollController was provided in the ScrollableDetails of the " + "TwoDimensionalScrollView.mainAxis.");
            mainAxisDetails = mainAxisDetails.copyWith(controller: PrimaryScrollController.of(context));
        }
        var scrollable = new TwoDimensionalScrollable(horizontalDetails: (this.mainAxis switch { global::Doroti.Framework.Painting.Axis.horizontal => mainAxisDetails, global::Doroti.Framework.Painting.Axis.vertical => this.horizontalDetails, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }), verticalDetails: (this.mainAxis switch { global::Doroti.Framework.Painting.Axis.vertical => mainAxisDetails, global::Doroti.Framework.Painting.Axis.horizontal => this.verticalDetails, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }), diagonalDragBehavior: this.diagonalDragBehavior, viewportBuilder: (global::System.Func<BuildContext, global::Doroti.Framework.Rendering.ViewportOffset, global::Doroti.Framework.Rendering.ViewportOffset, Widget>)this.buildViewport, dragStartBehavior: this.dragStartBehavior, hitTestBehavior: this.hitTestBehavior);
        Widget scrollableResult = (effectivePrimary ? PrimaryScrollController.CreateNone(child: scrollable) : scrollable);
        ScrollViewKeyboardDismissBehavior effectiveKeyboardDismissBehavior = ((this.keyboardDismissBehavior ?? (ScrollViewKeyboardDismissBehavior)ScrollConfiguration.of(context).getKeyboardDismissBehavior(context)));
        if ((object.Equals(effectiveKeyboardDismissBehavior, ScrollViewKeyboardDismissBehavior.onDrag)))
        {
            return ((Widget)(object?)new NotificationListener<ScrollUpdateNotification>(child: scrollableResult, onNotification: ((global::System.Func<ScrollUpdateNotification, bool>?)((notification) =>
            {
                FocusScopeNode currentScope = ((FocusScopeNode)(object?)FocusScope.of(context));
                if ((((((ScrollUpdateNotification)notification).dragDetails is not null) && !currentScope.hasPrimaryFocus) && currentScope.hasFocus))
                {
                    FocusManager.instance.primaryFocus?.unfocus();
                }
                return false;
                throw new InvalidOperationException("Dart closure completed without a value.");
            }))));
        }
        return scrollableResult;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Painting.Axis>("mainAxis", this.mainAxis));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<DiagonalDragBehavior>("diagonalDragBehavior", this.diagonalDragBehavior));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("primary", value: this.primary, ifTrue: "using primary controller", showName: true));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ScrollableDetails>("verticalDetails", this.verticalDetails, showName: false));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ScrollableDetails>("horizontalDetails", this.horizontalDetails, showName: false));
    }

}

