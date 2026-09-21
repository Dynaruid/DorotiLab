// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/two_dimensional_scroll_view.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public abstract class TwoDimensionalScrollView : StatelessWidget
{
    public virtual TwoDimensionalChildDelegate @delegate { get; private set; } = default!;
    public virtual double? cacheExtent { get; private set; }
    public virtual CacheExtentStyle? cacheExtentStyle { get; private set; }
    public virtual ScrollCacheExtent? scrollCacheExtent { get; private set; }
    public virtual DiagonalDragBehavior diagonalDragBehavior { get; private set; } = default!;
    public virtual bool? primary { get; private set; }
    public virtual Axis mainAxis { get; private set; } = default!;
    public virtual ScrollableDetails verticalDetails { get; private set; } = default!;
    public virtual ScrollableDetails horizontalDetails { get; private set; } = default!;
    public virtual DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior { get; private set; }
    public virtual HitTestBehavior hitTestBehavior { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;

    protected TwoDimensionalScrollView(
        Key? key = null,
        bool? primary = null,
        Axis mainAxis = Axis.vertical,
        ScrollableDetails verticalDetails = default!,
        ScrollableDetails horizontalDetails = default!,
        TwoDimensionalChildDelegate @delegate = default!,
        double? cacheExtent = null,
        CacheExtentStyle? cacheExtentStyle = null,
        ScrollCacheExtent? scrollCacheExtent = null,
        DiagonalDragBehavior diagonalDragBehavior = DiagonalDragBehavior.none,
        DragStartBehavior dragStartBehavior = DragStartBehavior.start,
        ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null,
        Clip clipBehavior = Clip.hardEdge,
        HitTestBehavior hitTestBehavior = HitTestBehavior.opaque
    )
        : base(key: key)
    {
        ScrollableDetails __verticalDetails = verticalDetails ?? ScrollableDetails.CreateVertical();
        ScrollableDetails __horizontalDetails =
            horizontalDetails ?? ScrollableDetails.CreateHorizontal();
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

    public abstract Widget buildViewport(
        BuildContext context,
        ViewportOffset verticalOffset,
        ViewportOffset horizontalOffset
    );

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(
            () =>
                Equals(
                    Basic_typesLibrary.axisDirectionToAxis(verticalDetails.direction),
                    Axis.vertical
                ),
            () => (object?)"TwoDimensionalScrollView.verticalDetails are not Axis.vertical."
        );
        DartRuntimePrimitives.Assert(
            () =>
                Equals(
                    Basic_typesLibrary.axisDirectionToAxis(horizontalDetails.direction),
                    Axis.horizontal
                ),
            () => (object?)"TwoDimensionalScrollView.horizontalDetails are not Axis.horizontal."
        );
        ScrollableDetails mainAxisDetails = mainAxis switch
        {
            Axis.vertical => verticalDetails,
            Axis.horizontal => horizontalDetails,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        bool effectivePrimary =
            primary
            ?? (
                (mainAxisDetails.controller is null)
                && PrimaryScrollController.shouldInherit(context, mainAxis)
            );
        if (effectivePrimary)
        {
            DartRuntimePrimitives.Assert(
                () => mainAxisDetails.controller is null,
                () =>
                    (object?)"TwoDimensionalScrollView.primary was explicitly set to true, but a "
                    + "ScrollController was provided in the ScrollableDetails of the "
                    + "TwoDimensionalScrollView.mainAxis."
            );
            mainAxisDetails = mainAxisDetails.copyWith(
                controller: PrimaryScrollController.of(context)
            );
        }
        var scrollable = new TwoDimensionalScrollable(
            horizontalDetails: mainAxis switch
            {
                Axis.horizontal => mainAxisDetails,
                Axis.vertical => horizontalDetails,
                _ => throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
            },
            verticalDetails: mainAxis switch
            {
                Axis.vertical => mainAxisDetails,
                Axis.horizontal => verticalDetails,
                _ => throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
            },
            diagonalDragBehavior: diagonalDragBehavior,
            viewportBuilder: buildViewport,
            dragStartBehavior: dragStartBehavior,
            hitTestBehavior: hitTestBehavior
        );
        Widget scrollableResult = effectivePrimary
            ? PrimaryScrollController.CreateNone(child: scrollable)
            : scrollable;
        ScrollViewKeyboardDismissBehavior effectiveKeyboardDismissBehavior =
            keyboardDismissBehavior
            ?? ScrollConfiguration.of(context).getKeyboardDismissBehavior(context);
        if (Equals(effectiveKeyboardDismissBehavior, ScrollViewKeyboardDismissBehavior.onDrag))
        {
            return new NotificationListener<ScrollUpdateNotification>(
                child: scrollableResult,
                onNotification: (notification) =>
                {
                    FocusScopeNode currentScope = FocusScope.of(context);
                    if (
                        (notification.dragDetails is not null)
                        && !currentScope.hasPrimaryFocus
                        && currentScope.hasFocus
                    )
                    {
                        FocusManager.instance.primaryFocus?.unfocus();
                    }
                    return false;
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                }
            );
        }
        return scrollableResult;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<Axis>("mainAxis", mainAxis));
        properties.add(
            new EnumProperty<DiagonalDragBehavior>("diagonalDragBehavior", diagonalDragBehavior)
        );
        properties.add(
            new FlagProperty(
                "primary",
                value: primary,
                ifTrue: "using primary controller",
                showName: true
            )
        );
        properties.add(
            new DiagnosticsProperty<ScrollableDetails>(
                "verticalDetails",
                verticalDetails,
                showName: false
            )
        );
        properties.add(
            new DiagnosticsProperty<ScrollableDetails>(
                "horizontalDetails",
                horizontalDetails,
                showName: false
            )
        );
    }
}
