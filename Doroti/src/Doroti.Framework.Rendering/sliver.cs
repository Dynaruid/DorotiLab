// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/sliver.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

public delegate double? ItemExtentBuilder(long index, SliverLayoutDimensions dimensions);

public class SliverLayoutDimensions
{
    public virtual double scrollOffset { get; private set; } = default!;
    public virtual double precedingScrollExtent { get; private set; } = default!;
    public virtual double viewportMainAxisExtent { get; private set; } = default!;
    public virtual double crossAxisExtent { get; private set; } = default!;

    public SliverLayoutDimensions(
        double scrollOffset,
        double precedingScrollExtent,
        double viewportMainAxisExtent,
        double crossAxisExtent
    )
    {
        this.scrollOffset = scrollOffset;
        this.precedingScrollExtent = precedingScrollExtent;
        this.viewportMainAxisExtent = viewportMainAxisExtent;
        this.crossAxisExtent = crossAxisExtent;
    }

    public override bool Equals(object? other)
    {
        var __other = other as SliverLayoutDimensions;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (__other is not SliverLayoutDimensions)
        {
            return false;
        }
        return (__other.scrollOffset == scrollOffset)
            && (__other.precedingScrollExtent == precedingScrollExtent)
            && (__other.viewportMainAxisExtent == viewportMainAxisExtent)
            && (__other.crossAxisExtent == crossAxisExtent);
    }

    public override string ToString()
    {
        return $"scrollOffset: {scrollOffset}"
            + $" precedingScrollExtent: {precedingScrollExtent}"
            + $" viewportMainAxisExtent: {viewportMainAxisExtent}"
            + $" crossAxisExtent: {crossAxisExtent}";
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(
            scrollOffset,
            precedingScrollExtent,
            viewportMainAxisExtent,
            crossAxisExtent
        );
}

public enum GrowthDirection
{
    forward,
    reverse,
}

public static partial class SliverLibrary
{
    public static AxisDirection applyGrowthDirectionToAxisDirection(
        AxisDirection axisDirection,
        GrowthDirection growthDirection
    )
    {
        return growthDirection switch
        {
            GrowthDirection.forward => axisDirection,
            GrowthDirection.reverse => Basic_typesLibrary.flipAxisDirection(axisDirection),
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public static partial class SliverLibrary
{
    public static ScrollDirection applyGrowthDirectionToScrollDirection(
        ScrollDirection scrollDirection,
        GrowthDirection growthDirection
    )
    {
        return growthDirection switch
        {
            GrowthDirection.forward => scrollDirection,
            GrowthDirection.reverse => Viewport_offsetLibrary.flipScrollDirection(scrollDirection),
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class SliverConstraints : Constraints
{
    public virtual AxisDirection axisDirection { get; private set; } = default!;
    public virtual GrowthDirection growthDirection { get; private set; } = default!;
    public virtual ScrollDirection userScrollDirection { get; private set; } = default!;
    public virtual double scrollOffset { get; private set; } = default!;
    public virtual double precedingScrollExtent { get; private set; } = default!;
    public virtual double overlap { get; private set; } = default!;
    public virtual double remainingPaintExtent { get; private set; } = default!;
    public virtual double crossAxisExtent { get; private set; } = default!;
    public virtual AxisDirection crossAxisDirection { get; private set; } = default!;
    public virtual double viewportMainAxisExtent { get; private set; } = default!;
    public virtual double cacheOrigin { get; private set; } = default!;
    public virtual double remainingCacheExtent { get; private set; } = default!;

    public SliverConstraints(
        AxisDirection axisDirection,
        GrowthDirection growthDirection,
        ScrollDirection userScrollDirection,
        double scrollOffset,
        double precedingScrollExtent,
        double overlap,
        double remainingPaintExtent,
        double crossAxisExtent,
        AxisDirection crossAxisDirection,
        double viewportMainAxisExtent,
        double remainingCacheExtent,
        double cacheOrigin
    )
    {
        this.axisDirection = axisDirection;
        this.growthDirection = growthDirection;
        this.userScrollDirection = userScrollDirection;
        this.scrollOffset = scrollOffset;
        this.precedingScrollExtent = precedingScrollExtent;
        this.overlap = overlap;
        this.remainingPaintExtent = remainingPaintExtent;
        this.crossAxisExtent = crossAxisExtent;
        this.crossAxisDirection = crossAxisDirection;
        this.viewportMainAxisExtent = viewportMainAxisExtent;
        this.remainingCacheExtent = remainingCacheExtent;
        this.cacheOrigin = cacheOrigin;
    }

    public virtual SliverConstraints copyWith(
        AxisDirection? axisDirection = null,
        GrowthDirection? growthDirection = null,
        ScrollDirection? userScrollDirection = null,
        double? scrollOffset = null,
        double? precedingScrollExtent = null,
        double? overlap = null,
        double? remainingPaintExtent = null,
        double? crossAxisExtent = null,
        AxisDirection? crossAxisDirection = null,
        double? viewportMainAxisExtent = null,
        double? remainingCacheExtent = null,
        double? cacheOrigin = null
    )
    {
        return new SliverConstraints(
            axisDirection: axisDirection ?? this.axisDirection,
            growthDirection: growthDirection ?? this.growthDirection,
            userScrollDirection: userScrollDirection ?? this.userScrollDirection,
            scrollOffset: scrollOffset ?? this.scrollOffset,
            precedingScrollExtent: precedingScrollExtent ?? this.precedingScrollExtent,
            overlap: overlap ?? this.overlap,
            remainingPaintExtent: remainingPaintExtent ?? this.remainingPaintExtent,
            crossAxisExtent: crossAxisExtent ?? this.crossAxisExtent,
            crossAxisDirection: crossAxisDirection ?? this.crossAxisDirection,
            viewportMainAxisExtent: viewportMainAxisExtent ?? this.viewportMainAxisExtent,
            remainingCacheExtent: remainingCacheExtent ?? this.remainingCacheExtent,
            cacheOrigin: cacheOrigin ?? this.cacheOrigin
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Axis axis => Basic_typesLibrary.axisDirectionToAxis((axisDirection));
    public virtual GrowthDirection normalizedGrowthDirection
    {
        get
        {
            if (Basic_typesLibrary.axisDirectionIsReversed((axisDirection)))
            {
                return growthDirection switch
                {
                    GrowthDirection.forward => GrowthDirection.reverse,
                    GrowthDirection.reverse => GrowthDirection.forward,
                    _ => throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    ),
                };
            }
            return growthDirection;
        }
    }
    public override bool isTight => false;
    public override bool isNormalized
    {
        get
        {
            return (scrollOffset >= 0.0)
                && (crossAxisExtent >= 0.0)
                && (
                    !Equals(
                        Basic_typesLibrary.axisDirectionToAxis((axisDirection)),
                        Basic_typesLibrary.axisDirectionToAxis((crossAxisDirection))
                    )
                )
                && (viewportMainAxisExtent >= 0.0)
                && (remainingPaintExtent >= 0.0);
        }
    }

    public virtual BoxConstraints asBoxConstraints(
        double minExtent = 0.0,
        double maxExtent = double.PositiveInfinity,
        double? crossAxisExtent = null
    )
    {
        crossAxisExtent ??= this.crossAxisExtent;
        switch (axis)
        {
            case Axis.horizontal:
            {
                return new BoxConstraints(
                    minHeight: (
                        crossAxisExtent
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ),
                    maxHeight: (
                        crossAxisExtent
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ),
                    minWidth: minExtent,
                    maxWidth: maxExtent
                );
            }
            case Axis.vertical:
            {
                return new BoxConstraints(
                    minWidth: (
                        crossAxisExtent
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ),
                    maxWidth: (
                        crossAxisExtent
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ),
                    minHeight: minExtent,
                    maxHeight: maxExtent
                );
            }
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool debugAssertIsValid(
        bool isAppliedConstraint = false,
        InformationCollector? informationCollector = null
    )
    {
        DartRuntimePrimitives.Assert(() =>
        {
            var hasErrors = false;
            var errorMessage = new StringBuffer("\n");
            void verify(bool check, string message)
            {
                if (check)
                {
                    return;
                }
                hasErrors = true;
                errorMessage.writeln($"  {message}");
            }
            void verifyDouble(
                double property,
                string name,
                bool mustBePositive = false,
                bool mustBeNegative = false
            )
            {
                if (double.IsNaN(property))
                {
                    var additional = ".";
                    if (mustBePositive)
                    {
                        additional = ", expected greater than or equal to zero.";
                    }
                    else
                    {
                        if (mustBeNegative)
                        {
                            additional = ", expected less than or equal to zero.";
                        }
                    }
                    verify(false, $"The \"{name}\" is NaN{additional}");
                }
                else
                {
                    if (mustBePositive)
                    {
                        verify(property >= 0.0, $"The \"{name}\" is negative.");
                    }
                    else
                    {
                        if (mustBeNegative)
                        {
                            verify(property <= 0.0, $"The \"{name}\" is positive.");
                        }
                    }
                }
            }
            verifyDouble((scrollOffset), "scrollOffset");
            verifyDouble((overlap), "overlap");
            verifyDouble((crossAxisExtent), "crossAxisExtent");
            verifyDouble((scrollOffset), "scrollOffset", mustBePositive: true);
            verify(
                !Equals(
                    Basic_typesLibrary.axisDirectionToAxis((axisDirection)),
                    Basic_typesLibrary.axisDirectionToAxis((crossAxisDirection))
                ),
                "The \"axisDirection\" and the \"crossAxisDirection\" are along the same axis."
            );
            verifyDouble((viewportMainAxisExtent), "viewportMainAxisExtent", mustBePositive: true);
            verifyDouble((remainingPaintExtent), "remainingPaintExtent", mustBePositive: true);
            verifyDouble((remainingCacheExtent), "remainingCacheExtent", mustBePositive: true);
            verifyDouble((cacheOrigin), "cacheOrigin", mustBeNegative: true);
            verifyDouble((precedingScrollExtent), "precedingScrollExtent", mustBePositive: true);
            verify(isNormalized, "The constraints are not normalized.");
            if (hasErrors)
            {
                throw new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary($"{GetType()} is not valid: {errorMessage}"),
                        new DiagnosticsProperty<SliverConstraints>(
                            "The offending constraints were",
                            this,
                            style: DiagnosticsTreeStyle.errorProperty
                        ),
                    }
                );
            }
            return true;
        });
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as SliverConstraints;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (__other is not SliverConstraints)
        {
            return false;
        }
        DartRuntimePrimitives.Assert(() => __other.debugAssertIsValid());
        return Equals(__other.axisDirection, axisDirection)
            && Equals(__other.growthDirection, growthDirection)
            && Equals(__other.userScrollDirection, userScrollDirection)
            && (__other.scrollOffset == scrollOffset)
            && (__other.precedingScrollExtent == precedingScrollExtent)
            && (__other.overlap == overlap)
            && (__other.remainingPaintExtent == remainingPaintExtent)
            && (__other.crossAxisExtent == crossAxisExtent)
            && Equals(__other.crossAxisDirection, crossAxisDirection)
            && (__other.viewportMainAxisExtent == viewportMainAxisExtent)
            && (__other.remainingCacheExtent == remainingCacheExtent)
            && (__other.cacheOrigin == cacheOrigin);
    }

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(
            axisDirection,
            growthDirection,
            userScrollDirection,
            scrollOffset,
            precedingScrollExtent,
            overlap,
            remainingPaintExtent,
            crossAxisExtent,
            crossAxisDirection,
            viewportMainAxisExtent,
            remainingCacheExtent,
            cacheOrigin
        );

    public override string ToString()
    {
        var properties = new List<string>
        {
            $"{axisDirection}",
            $"{growthDirection}",
            $"{userScrollDirection}",
            $"scrollOffset: {scrollOffset.toStringAsFixed(1L)}",
            $"precedingScrollExtent: {precedingScrollExtent.toStringAsFixed(1L)}",
            $"remainingPaintExtent: {remainingPaintExtent.toStringAsFixed(1L)}",
            $"crossAxisExtent: {crossAxisExtent.toStringAsFixed(1L)}",
            $"crossAxisDirection: {crossAxisDirection}",
            $"viewportMainAxisExtent: {viewportMainAxisExtent.toStringAsFixed(1L)}",
            $"remainingCacheExtent: {remainingCacheExtent.toStringAsFixed(1L)}",
            $"cacheOrigin: {cacheOrigin.toStringAsFixed(1L)}",
        };
        return $"SliverConstraints({string.Join(", ", properties)})";
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class SliverGeometry : Diagnosticable
{
    public static SliverGeometry zero = new SliverGeometry();
    public virtual double scrollExtent { get; private set; } = default!;
    public virtual double paintOrigin { get; private set; } = default!;
    public virtual double paintExtent { get; private set; } = default!;
    public virtual double layoutExtent { get; private set; } = default!;
    public virtual double maxPaintExtent { get; private set; } = default!;
    public virtual double maxScrollObstructionExtent { get; private set; } = default!;
    public virtual double hitTestExtent { get; private set; } = default!;
    public virtual bool visible { get; private set; } = default!;
    public virtual bool hasVisualOverflow { get; private set; } = default!;
    public virtual double? scrollOffsetCorrection { get; private set; }
    public virtual double cacheExtent { get; private set; } = default!;
    public virtual double? crossAxisExtent { get; private set; }

    public SliverGeometry(
        double scrollExtent = 0.0,
        double paintExtent = 0.0,
        double paintOrigin = 0.0,
        double? layoutExtent = null,
        double maxPaintExtent = 0.0,
        double maxScrollObstructionExtent = 0.0,
        double? crossAxisExtent = null,
        double? hitTestExtent = null,
        bool? visible = null,
        bool hasVisualOverflow = false,
        double? scrollOffsetCorrection = null,
        double? cacheExtent = null
    )
    {
        this.scrollExtent = scrollExtent;
        this.paintExtent = paintExtent;
        this.paintOrigin = paintOrigin;
        this.maxPaintExtent = maxPaintExtent;
        this.maxScrollObstructionExtent = maxScrollObstructionExtent;
        this.crossAxisExtent = crossAxisExtent;
        this.hasVisualOverflow = hasVisualOverflow;
        this.scrollOffsetCorrection = scrollOffsetCorrection;
        this.layoutExtent = layoutExtent ?? (paintExtent);
        this.hitTestExtent = hitTestExtent ?? (paintExtent);
        this.cacheExtent = (cacheExtent ?? layoutExtent) ?? (paintExtent);
        this.visible = visible ?? ((paintExtent) > 0.0);
        System.Diagnostics.Debug.Assert(scrollOffsetCorrection != 0.0);
    }

    public virtual SliverGeometry copyWith(
        double? scrollExtent = null,
        double? paintExtent = null,
        double? paintOrigin = null,
        double? layoutExtent = null,
        double? maxPaintExtent = null,
        double? maxScrollObstructionExtent = null,
        double? crossAxisExtent = null,
        double? hitTestExtent = null,
        bool? visible = null,
        bool? hasVisualOverflow = null,
        double? cacheExtent = null
    )
    {
        return new SliverGeometry(
            scrollExtent: scrollExtent ?? this.scrollExtent,
            paintExtent: paintExtent ?? this.paintExtent,
            paintOrigin: paintOrigin ?? this.paintOrigin,
            layoutExtent: layoutExtent ?? this.layoutExtent,
            maxPaintExtent: maxPaintExtent ?? this.maxPaintExtent,
            maxScrollObstructionExtent: maxScrollObstructionExtent
                ?? this.maxScrollObstructionExtent,
            crossAxisExtent: crossAxisExtent ?? this.crossAxisExtent,
            hitTestExtent: hitTestExtent ?? this.hitTestExtent,
            visible: visible ?? this.visible,
            hasVisualOverflow: hasVisualOverflow ?? this.hasVisualOverflow,
            cacheExtent: cacheExtent ?? this.cacheExtent
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool debugAssertIsValid(InformationCollector? informationCollector = null)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            void verify(bool check, string summary, List<DiagnosticsNode>? details = null)
            {
                if (check)
                {
                    return;
                }
                throw new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary(
                            $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "SliverGeometry")} is not valid: {summary}"
                        ),
                    }
                );
            }
            verify(scrollExtent >= 0.0, "The \"scrollExtent\" is negative.");
            verify(paintExtent >= 0.0, "The \"paintExtent\" is negative.");
            verify(layoutExtent >= 0.0, "The \"layoutExtent\" is negative.");
            verify(cacheExtent >= 0.0, "The \"cacheExtent\" is negative.");
            if (layoutExtent > paintExtent)
            {
                verify(
                    false,
                    "The \"layoutExtent\" exceeds the \"paintExtent\".",
                    details: SliverLibrary._debugCompareFloats(
                        "paintExtent",
                        (paintExtent),
                        "layoutExtent",
                        (layoutExtent)
                    )
                );
            }
            if (
                (paintExtent - maxPaintExtent) > Foundation.ConstantsLibrary.precisionErrorTolerance
            )
            {
                verify(
                    false,
                    "The \"maxPaintExtent\" is less than the \"paintExtent\".",
                    details: (
                        (Func<List<DiagnosticsNode>>)(
                            () =>
                            {
                                var __cascade = SliverLibrary._debugCompareFloats(
                                    "maxPaintExtent",
                                    (maxPaintExtent),
                                    "paintExtent",
                                    (paintExtent)
                                );
                                __cascade.Add(
                                    new ErrorDescription(
                                        "By definition, a sliver can't paint more than the maximum that it can paint!"
                                    )
                                );
                                return __cascade;
                            }
                        )
                    )()
                );
            }
            verify(hitTestExtent >= 0.0, "The \"hitTestExtent\" is negative.");
            verify(scrollOffsetCorrection != 0.0, "The \"scrollOffsetCorrection\" is zero.");
            return true;
        });
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual string toStringShort() =>
        objectRuntimeTypeFunctions.objectRuntimeType(this, "SliverGeometry");

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("scrollExtent", scrollExtent));
        if (paintExtent > 0.0)
        {
            properties.add(
                new DoubleProperty(
                    "paintExtent",
                    paintExtent,
                    unit: visible ? null : " but not painting"
                )
            );
        }
        else
        {
            if (paintExtent == 0.0)
            {
                if (visible)
                {
                    properties.add(
                        new DoubleProperty(
                            "paintExtent",
                            paintExtent,
                            unit: visible ? null : " but visible"
                        )
                    );
                }
                properties.add(new FlagProperty("visible", value: visible, ifFalse: "hidden"));
            }
            else
            {
                properties.add(new DoubleProperty("paintExtent", paintExtent, tooltip: "!"));
            }
        }
        properties.add(new DoubleProperty("paintOrigin", paintOrigin, defaultValue: 0.0));
        properties.add(new DoubleProperty("layoutExtent", layoutExtent, defaultValue: paintExtent));
        properties.add(new DoubleProperty("maxPaintExtent", maxPaintExtent));
        properties.add(
            new DoubleProperty("hitTestExtent", hitTestExtent, defaultValue: paintExtent)
        );
        properties.add(
            new DiagnosticsProperty<bool>(
                "hasVisualOverflow",
                hasVisualOverflow,
                defaultValue: false
            )
        );
        properties.add(
            new DoubleProperty("scrollOffsetCorrection", scrollOffsetCorrection, defaultValue: null)
        );
        properties.add(new DoubleProperty("cacheExtent", cacheExtent, defaultValue: 0.0));
    }
}

public delegate bool SliverHitTest(
    SliverHitTestResult result,
    double crossAxisPosition,
    double mainAxisPosition
);

public class SliverHitTestResult : HitTestResult
{
    public SliverHitTestResult() { }

    private SliverHitTestResult(HitTestResult result)
        : base(result) { }

    public static new SliverHitTestResult CreateWrap(HitTestResult result)
    {
        return new SliverHitTestResult(result);
    }

    public virtual bool addWithAxisOffset(
        Offset? paintOffset,
        double mainAxisOffset,
        double crossAxisOffset,
        double mainAxisPosition,
        double crossAxisPosition,
        Func<SliverHitTestResult, double, double, bool> hitTest
    )
    {
        if (paintOffset is not null)
        {
            Offset paintOffset__value42308 = (
                paintOffset
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            pushOffset(-(paintOffset__value42308));
        }
        bool isHit = hitTest(
            this,
            mainAxisPosition - mainAxisOffset,
            crossAxisPosition - crossAxisOffset
        );
        if (paintOffset is not null)
        {
            Offset paintOffset__value42549 = (
                paintOffset
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            popTransform();
        }
        return isHit;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class SliverHitTestEntry : HitTestEntry<RenderSliver>
{
    public virtual double mainAxisPosition { get; private set; } = default!;
    public virtual double crossAxisPosition { get; private set; } = default!;

    public SliverHitTestEntry(
        RenderSliver target,
        double mainAxisPosition,
        double crossAxisPosition
    )
        : base(target)
    {
        this.mainAxisPosition = mainAxisPosition;
        this.crossAxisPosition = crossAxisPosition;
    }

    public override string ToString() =>
        $"{DartRuntimePrimitives.RuntimeType(target)}@(mainAxis: {mainAxisPosition}, crossAxis: {crossAxisPosition})";
}

public class SliverLogicalParentData : ParentData
{
    public virtual double? layoutOffset { get; set; } = default;

    public override string ToString() =>
        $"layoutOffset={((layoutOffset is null) ? "None" : (layoutOffset ?? throw new global::System.NullReferenceException("A required value was null.")).toStringAsFixed(1L))}";
}

public class SliverLogicalContainerParentData
    : SliverLogicalParentData,
        ContainerParentDataMixin<RenderSliver>
{
    public virtual RenderSliver? previousSibling { get; set; } = default;
    public virtual RenderSliver? nextSibling { get; set; } = default;

    public override void detach()
    {
        DartRuntimePrimitives.Assert(() => previousSibling is null);
        DartRuntimePrimitives.Assert(() => nextSibling is null);
        base.detach();
    }
}

public class SliverPhysicalParentData : ParentData
{
    public virtual Offset paintOffset { get; set; } = Offset.zero;
    public virtual long? crossAxisFlex { get; set; } = default;

    public virtual void applyPaintTransform(Matrix4 transform)
    {
        transform.translateByDouble(paintOffset.dx, paintOffset.dy, 0, 1);
    }

    public override string ToString() => $"paintOffset={paintOffset}";
}

public class SliverPhysicalContainerParentData
    : SliverPhysicalParentData,
        ContainerParentDataMixin<RenderSliver>
{
    public virtual RenderSliver? previousSibling { get; set; } = default;
    public virtual RenderSliver? nextSibling { get; set; } = default;

    public override void detach()
    {
        DartRuntimePrimitives.Assert(() => previousSibling is null);
        DartRuntimePrimitives.Assert(() => nextSibling is null);
        base.detach();
    }
}

public static partial class SliverLibrary
{
    internal static List<DiagnosticsNode> _debugCompareFloats(
        string labelA,
        double valueA,
        string labelB,
        double valueB
    )
    {
        return new List<DiagnosticsNode>();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public abstract class RenderSliver : RenderObject
{
    internal virtual SliverGeometry? _geometry { get; set; } = default;

    public RenderSliver() { }

    public virtual bool ensureSemantics => false;
    public override SliverConstraints constraints =>
        ((SliverConstraints?)(object?)base.constraints)!;
    public virtual SliverGeometry? geometry
    {
        get => _geometry;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => !(debugDoingThisResize && debugDoingThisLayout));
            DartRuntimePrimitives.Assert(() => sizedByParent || !debugDoingThisResize);
            DartRuntimePrimitives.Assert(() =>
            {
                if (
                    (sizedByParent && debugDoingThisResize)
                    || (!sizedByParent && debugDoingThisLayout)
                )
                {
                    return true;
                }
                DartRuntimePrimitives.Assert(() => !debugDoingThisResize);
                DiagnosticsNode? contract = default!;
                DiagnosticsNode? violation = default!;
                DiagnosticsNode? hint = default!;
                if (debugDoingThisLayout)
                {
                    DartRuntimePrimitives.Assert(() => sizedByParent);
                    violation = new ErrorDescription(
                        "It appears that the geometry setter was called from performLayout()."
                    );
                }
                else
                {
                    violation = new ErrorDescription(
                        "The geometry setter was called from outside layout (neither performResize() nor performLayout() were being run for this object)."
                    );
                    if ((owner is not null) && owner!.debugDoingLayout)
                    {
                        hint = new ErrorDescription(
                            "Only the object itself can set its geometry. It is a contract violation for other objects to set it."
                        );
                    }
                }
                if (sizedByParent)
                {
                    contract = new ErrorDescription(
                        "Because this RenderSliver has sizedByParent set to true, it must set its geometry in performResize()."
                    );
                }
                else
                {
                    contract = new ErrorDescription(
                        "Because this RenderSliver has sizedByParent set to false, it must set its geometry in performLayout()."
                    );
                }
                var information = new List<DiagnosticsNode>
                {
                    new ErrorSummary("RenderSliver geometry setter called incorrectly."),
                    violation,
                    contract,
                    describeForError("The RenderSliver in question is"),
                };
                throw new FlutterError(information);
            });
            _geometry = __value;
        }
    }
    public override Rect semanticBounds => paintBounds;
    public override Rect paintBounds
    {
        get
        {
            switch (constraints.axis)
            {
                case Axis.horizontal:
                {
                    return Rect.fromLTWH(
                        0.0,
                        0.0,
                        geometry!.paintExtent,
                        constraints.crossAxisExtent
                    );
                }
                case Axis.vertical:
                {
                    return Rect.fromLTWH(
                        0.0,
                        0.0,
                        constraints.crossAxisExtent,
                        geometry!.paintExtent
                    );
                }
            }
            return default!;
        }
    }

    public override void debugResetSize() { }

    public override void debugAssertDoesMeetConstraints()
    {
        DartRuntimePrimitives.Assert(() =>
            geometry!.debugAssertIsValid(informationCollector: () =>
                new List<DiagnosticsNode>
                {
                    describeForError("The RenderSliver that returned the offending geometry was"),
                }
            )
        );
        DartRuntimePrimitives.Assert(() =>
        {
            if ((geometry!.paintOrigin + geometry!.paintExtent) > constraints.remainingPaintExtent)
            {
                throw new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary(
                            "SliverGeometry has a paintOffset that exceeds the remainingPaintExtent from the constraints."
                        ),
                        describeForError(
                            "The render object whose geometry violates the constraints is the following"
                        ),
                        new ErrorDescription(
                            "The paintOrigin and paintExtent must cause the child sliver to paint "
                                + "within the viewport, and so cannot exceed the remainingPaintExtent."
                        ),
                    }
                );
            }
            return true;
        });
    }

    public override void performResize()
    {
        DartRuntimePrimitives.Assert(() => false);
    }

    public virtual double centerOffsetAdjustment => 0.0;

    public virtual bool hitTest(
        SliverHitTestResult result,
        double mainAxisPosition,
        double crossAxisPosition
    )
    {
        if (
            (mainAxisPosition >= 0.0)
            && (mainAxisPosition < geometry!.hitTestExtent)
            && (crossAxisPosition >= 0.0)
            && (crossAxisPosition < constraints.crossAxisExtent)
        )
        {
            if (
                hitTestChildren(
                    result,
                    mainAxisPosition: mainAxisPosition,
                    crossAxisPosition: crossAxisPosition
                )
                || hitTestSelf(
                    mainAxisPosition: mainAxisPosition,
                    crossAxisPosition: crossAxisPosition
                )
            )
            {
                result.add(
                    new SliverHitTestEntry(
                        this,
                        mainAxisPosition: mainAxisPosition,
                        crossAxisPosition: crossAxisPosition
                    )
                );
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool hitTestSelf(double mainAxisPosition, double crossAxisPosition) => false;

    public virtual bool hitTestChildren(
        SliverHitTestResult result,
        double mainAxisPosition,
        double crossAxisPosition
    ) => false;

    public virtual double calculatePaintOffset(
        SliverConstraints constraints,
        double from,
        double to
    )
    {
        DartRuntimePrimitives.Assert(() => from <= to);
        double a = constraints.scrollOffset;
        double b = constraints.scrollOffset + constraints.remainingPaintExtent;
        return DorotiUiLibrary.clampDouble(
            DorotiUiLibrary.clampDouble(to, a, b) - DorotiUiLibrary.clampDouble(from, a, b),
            0.0,
            constraints.remainingPaintExtent
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual double calculateCacheOffset(
        SliverConstraints constraints,
        double from,
        double to
    )
    {
        DartRuntimePrimitives.Assert(() => from <= to);
        double a = constraints.scrollOffset + constraints.cacheOrigin;
        double b = constraints.scrollOffset + constraints.remainingCacheExtent;
        return DorotiUiLibrary.clampDouble(
            DorotiUiLibrary.clampDouble(to, a, b) - DorotiUiLibrary.clampDouble(from, a, b),
            0.0,
            constraints.remainingCacheExtent
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual double childMainAxisPosition(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            throw new FlutterError(
                $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "RenderSliver")} does not implement childPosition."
            );
        });
        return 0.0;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual double childCrossAxisPosition(RenderObject child) => 0.0;

    public virtual double? childScrollOffset(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        return 0.0;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            throw new FlutterError(
                $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "RenderSliver")} does not implement applyPaintTransform."
            );
        });
    }

    public virtual Size getAbsoluteSizeRelativeToOrigin()
    {
        DartRuntimePrimitives.Assert(() => geometry is not null);
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        return SliverLibrary.applyGrowthDirectionToAxisDirection(
            constraints.axisDirection,
            constraints.growthDirection
        ) switch
        {
            AxisDirection.up => new Size(constraints.crossAxisExtent, -geometry!.paintExtent),
            AxisDirection.down => new Size(constraints.crossAxisExtent, geometry!.paintExtent),
            AxisDirection.left => new Size(-geometry!.paintExtent, constraints.crossAxisExtent),
            AxisDirection.right => new Size(geometry!.paintExtent, constraints.crossAxisExtent),
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Size getAbsoluteSize()
    {
        DartRuntimePrimitives.Assert(() => geometry is not null);
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        switch (constraints.axisDirection)
        {
            case AxisDirection.up:
            case AxisDirection.down:
            {
                return new Size(constraints.crossAxisExtent, geometry!.paintExtent);
            }
            case AxisDirection.right:
            case AxisDirection.left:
            {
                return new Size(geometry!.paintExtent, constraints.crossAxisExtent);
            }
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Rect getMaxPaintRect()
    {
        SliverGeometry? sliverGeometry = geometry;
        if ((sliverGeometry is null) || Equals(sliverGeometry, SliverGeometry.zero))
        {
            return Rect.zero;
        }
        double maxPaintExtentLocal = sliverGeometry.maxPaintExtent;
        if (double.IsInfinity(maxPaintExtentLocal))
        {
            maxPaintExtentLocal =
                constraints.scrollOffset + sliverGeometry.cacheExtent + constraints.cacheOrigin;
        }
        double paintExtentLocal = sliverGeometry.paintExtent;
        double leadingOffset = DorotiUiLibrary.clampDouble(
            constraints.scrollOffset,
            0.0,
            sliverGeometry.scrollExtent - sliverGeometry.maxScrollObstructionExtent
        );
        double crossAxisExtentLocal = sliverGeometry.crossAxisExtent ?? constraints.crossAxisExtent;
        Rect rect = constraints.axis switch
        {
            Axis.horizontal => Rect.fromLTWH(
                -leadingOffset,
                0.0,
                maxPaintExtentLocal,
                crossAxisExtentLocal
            ),
            Axis.vertical => Rect.fromLTWH(
                0.0,
                -leadingOffset,
                crossAxisExtentLocal,
                maxPaintExtentLocal
            ),
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        return SliverLibrary.applyGrowthDirectionToAxisDirection(
            constraints.axisDirection,
            constraints.growthDirection
        ) switch
        {
            AxisDirection.right => rect,
            AxisDirection.down => rect,
            AxisDirection.left => Rect.fromLTRB(
                paintExtentLocal - rect.right,
                rect.top,
                paintExtentLocal - rect.left,
                rect.bottom
            ),
            AxisDirection.up => Rect.fromLTRB(
                rect.left,
                paintExtentLocal - rect.bottom,
                rect.right,
                paintExtentLocal - rect.top
            ),
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _debugDrawArrow(
        Canvas canvas,
        Paint paint,
        Offset p0,
        Offset p1,
        GrowthDirection direction
    )
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (Equals(p0, p1))
            {
                return true;
            }
            DartRuntimePrimitives.Assert(() => (p0.dx == p1.dx) || (p0.dy == p1.dy));
            double d = (p1 - p0).distance * 0.2;
            Offset temp = default!;
            double dx1 = default!;
            double dx2 = default!;
            double dy1 = default!;
            double dy2 = default!;
            switch (direction)
            {
                case GrowthDirection.forward:
                {
                    dx1 = dx2 = dy1 = dy2 = d;
                    break;
                }
                case GrowthDirection.reverse:
                {
                    temp = p0;
                    p0 = p1;
                    p1 = temp;
                    dx1 = dx2 = dy1 = dy2 = -d;
                    break;
                }
            }
            if (p0.dx == p1.dx)
            {
                dx2 = -dx2;
            }
            else
            {
                dy2 = -dy2;
            }
            canvas.drawPath(
                (
                    (Func<Path>)(
                        () =>
                        {
                            var __cascade = new Path();
                            __cascade.moveTo(p0.dx, p0.dy);
                            __cascade.lineTo(p1.dx, p1.dy);
                            __cascade.moveTo(p1.dx - dx1, p1.dy - dy1);
                            __cascade.lineTo(p1.dx, p1.dy);
                            __cascade.lineTo(p1.dx - dx2, p1.dy - dy2);
                            return __cascade;
                        }
                    )
                )(),
                paint
            );
            return true;
        });
    }

    public override void debugPaint(PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (DebugLibrary.debugPaintSizeEnabled)
            {
                double strokeWidthLocal = Math.Min(4.0, geometry!.paintExtent / 30.0);
                var paint = (
                    (Func<Paint>)(
                        () =>
                        {
                            var __cascade = new Paint();
                            __cascade.color = new Color(4281584691L);
                            __cascade.strokeWidth = strokeWidthLocal;
                            __cascade.style = PaintingStyle.stroke;
                            __cascade.maskFilter = MaskFilter.blur(
                                BlurStyle.solid,
                                strokeWidthLocal
                            );
                            return __cascade;
                        }
                    )
                )();
                double arrowExtent = geometry!.paintExtent;
                double padding = Math.Max(2.0, strokeWidthLocal);
                Canvas canvasLocal = context.canvas;
                canvasLocal.drawCircle(offset.translate(padding, padding), padding * 0.5, paint);
                switch (constraints.axis)
                {
                    case Axis.vertical:
                    {
                        canvasLocal.drawLine(
                            offset,
                            offset.translate(constraints.crossAxisExtent, 0.0),
                            paint
                        );
                        _debugDrawArrow(
                            canvasLocal,
                            paint,
                            offset.translate(constraints.crossAxisExtent * 1.0 / 4.0, padding),
                            offset.translate(
                                constraints.crossAxisExtent * 1.0 / 4.0,
                                arrowExtent - padding
                            ),
                            constraints.normalizedGrowthDirection
                        );
                        _debugDrawArrow(
                            canvasLocal,
                            paint,
                            offset.translate(constraints.crossAxisExtent * 3.0 / 4.0, padding),
                            offset.translate(
                                constraints.crossAxisExtent * 3.0 / 4.0,
                                arrowExtent - padding
                            ),
                            constraints.normalizedGrowthDirection
                        );
                        break;
                    }
                    case Axis.horizontal:
                    {
                        canvasLocal.drawLine(
                            offset,
                            offset.translate(0.0, constraints.crossAxisExtent),
                            paint
                        );
                        _debugDrawArrow(
                            canvasLocal,
                            paint,
                            offset.translate(padding, constraints.crossAxisExtent * 1.0 / 4.0),
                            offset.translate(
                                arrowExtent - padding,
                                constraints.crossAxisExtent * 1.0 / 4.0
                            ),
                            constraints.normalizedGrowthDirection
                        );
                        _debugDrawArrow(
                            canvasLocal,
                            paint,
                            offset.translate(padding, constraints.crossAxisExtent * 3.0 / 4.0),
                            offset.translate(
                                arrowExtent - padding,
                                constraints.crossAxisExtent * 3.0 / 4.0
                            ),
                            constraints.normalizedGrowthDirection
                        );
                        break;
                    }
                }
            }
            return true;
        });
    }

    public override void handleEvent(PointerEvent @event, HitTestEntry<HitTestTarget> entry) { }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<SliverGeometry>("geometry", geometry));
    }
}

public interface RenderSliverHelpers
{
    public bool _getRightWayUp(SliverConstraints constraints);
    public bool hitTestBoxChild(
        BoxHitTestResult result,
        RenderBox child,
        double mainAxisPosition,
        double crossAxisPosition
    );
    public void applyPaintTransformForBoxChild(RenderBox child, Matrix4 transform);
}

public abstract class RenderSliverSingleBoxAdapter
    : RenderSliver,
        RenderObjectWithChildMixin<RenderBox>,
        RenderSliverHelpers
{
    public virtual RenderBox? _child { get; set; } = default;

    protected RenderSliverSingleBoxAdapter(RenderBox? child = null) { }

    public override void setupParentData(RenderObject child)
    {
        if (child.parentData is not SliverPhysicalParentData)
        {
            child.parentData = new SliverPhysicalParentData();
        }
    }

    public virtual void setChildParentData(
        RenderObject child,
        SliverConstraints constraints,
        SliverGeometry geometry
    )
    {
        var childParentData = ((SliverPhysicalParentData?)(object?)child.parentData!)!;
        childParentData.paintOffset = SliverLibrary.applyGrowthDirectionToAxisDirection(
            constraints.axisDirection,
            constraints.growthDirection
        ) switch
        {
            AxisDirection.up => new Offset(
                0.0,
                geometry.paintExtent + constraints.scrollOffset - geometry.scrollExtent
            ),
            AxisDirection.left => new Offset(
                geometry.paintExtent + constraints.scrollOffset - geometry.scrollExtent,
                0.0
            ),
            AxisDirection.right => new Offset(-constraints.scrollOffset, 0.0),
            AxisDirection.down => new Offset(0.0, -constraints.scrollOffset),
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
    }

    public override bool hitTestChildren(
        SliverHitTestResult result,
        double mainAxisPosition,
        double crossAxisPosition
    )
    {
        DartRuntimePrimitives.Assert(() => geometry!.hitTestExtent > 0.0);
        if (child is not null)
        {
            return hitTestBoxChild(
                BoxHitTestResult.CreateWrap(result),
                child!,
                mainAxisPosition: mainAxisPosition,
                crossAxisPosition: crossAxisPosition
            );
        }
        return false;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double childMainAxisPosition(RenderObject child)
    {
        var __child = (RenderBox)(object)child;
        return -constraints.scrollOffset;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        DartRuntimePrimitives.Assert(() => Equals(child, this.child));
        var childParentData = ((SliverPhysicalParentData?)(object?)child.parentData!)!;
        childParentData.applyPaintTransform(transform);
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if ((child is not null) && geometry!.visible)
        {
            var childParentData = ((SliverPhysicalParentData?)(object?)child!.parentData!)!;
            context.paintChild(child!, offset + childParentData.paintOffset);
        }
    }

    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (child is not RenderBox)
            {
                throw new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary(
                            $"A {GetType()} expected a child of type {typeof(RenderBox)} but received a "
                                + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."
                        ),
                        new ErrorDescription(
                            "RenderObjects expect specific types of children because they "
                                + "coordinate with their children during layout and paint. For "
                                + "example, a RenderSliver cannot be the child of a RenderBox because "
                                + "a RenderSliver does not understand the RenderBox layout protocol."
                        ),
                        new ErrorSpacer(),
                        new DiagnosticsProperty<object?>(
                            $"The {GetType()} that expected a {typeof(RenderBox)} child was created by",
                            debugCreator,
                            style: DiagnosticsTreeStyle.errorProperty
                        ),
                        new ErrorSpacer(),
                        new DiagnosticsProperty<object?>(
                            $"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type "
                                + "was created by",
                            child.debugCreator,
                            style: DiagnosticsTreeStyle.errorProperty
                        ),
                    }
                );
            }
            return true;
        });
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual RenderBox? child
    {
        get => _child;
        set
        {
            var __value = value;
            if (_child is not null)
            {
                dropChild(_child!);
            }
            _child = __value;
            if (_child is not null)
            {
                adoptChild(_child!);
            }
        }
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        _child?.attach(owner);
    }

    public override void detach()
    {
        base.detach();
        _child?.detach();
    }

    public override void redepthChildren()
    {
        if (_child is not null)
        {
            redepthChild(_child!);
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        if (_child is not null)
        {
            visitor(_child!);
        }
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        return (child is not null)
            ? new List<DiagnosticsNode>
            {
                ((Diagnosticable)child!).toDiagnosticsNode(name: "child"),
            }
            : new List<DiagnosticsNode>();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool _getRightWayUp(SliverConstraints constraints)
    {
        bool reversed = Basic_typesLibrary.axisDirectionIsReversed(constraints.axisDirection);
        return constraints.growthDirection switch
        {
            GrowthDirection.forward => !reversed,
            GrowthDirection.reverse => reversed,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool hitTestBoxChild(
        BoxHitTestResult result,
        RenderBox child,
        double mainAxisPosition,
        double crossAxisPosition
    )
    {
        bool rightWayUp = _getRightWayUp(constraints);
        double delta = childMainAxisPosition(child);
        double crossAxisDelta = childCrossAxisPosition(child);
        double absolutePosition = mainAxisPosition - delta;
        double absoluteCrossAxisPosition = crossAxisPosition - crossAxisDelta;
        Offset paintOffsetLocal = default!;
        Offset transformedPosition = default!;
        switch (constraints.axis)
        {
            case Axis.horizontal:
            {
                if (!rightWayUp)
                {
                    absolutePosition = child.size.width - absolutePosition;
                    delta = geometry!.paintExtent - child.size.width - delta;
                }
                paintOffsetLocal = new Offset(delta, crossAxisDelta);
                transformedPosition = new Offset(absolutePosition, absoluteCrossAxisPosition);
                break;
            }
            case Axis.vertical:
            {
                if (!rightWayUp)
                {
                    absolutePosition = child.size.height - absolutePosition;
                    delta = geometry!.paintExtent - child.size.height - delta;
                }
                paintOffsetLocal = new Offset(crossAxisDelta, delta);
                transformedPosition = new Offset(absoluteCrossAxisPosition, absolutePosition);
                break;
            }
        }
        return result.addWithOutOfBandPosition(
            paintOffset: paintOffsetLocal,
            hitTest: (result) =>
            {
                return child.hitTest(result, position: transformedPosition);
            }
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void applyPaintTransformForBoxChild(RenderBox child, Matrix4 transform)
    {
        bool rightWayUp = _getRightWayUp(constraints);
        double delta = childMainAxisPosition(child);
        double crossAxisDelta = childCrossAxisPosition(child);
        switch (constraints.axis)
        {
            case Axis.horizontal:
            {
                if (!rightWayUp)
                {
                    delta = geometry!.paintExtent - child.size.width - delta;
                }
                transform.translateByDouble(delta, crossAxisDelta, 0, 1);
                break;
            }
            case Axis.vertical:
            {
                if (!rightWayUp)
                {
                    delta = geometry!.paintExtent - child.size.height - delta;
                }
                transform.translateByDouble(crossAxisDelta, delta, 0, 1);
                break;
            }
        }
    }
}

public class RenderSliverToBoxAdapter : RenderSliverSingleBoxAdapter
{
    public RenderSliverToBoxAdapter(RenderBox? child = null)
        : base(child: child) { }

    public override void performLayout()
    {
        if (child is null)
        {
            geometry = SliverGeometry.zero;
            return;
        }
        SliverConstraints constraintsLocal = constraints;
        child!.layout(constraintsLocal.asBoxConstraints(), parentUsesSize: true);
        double childExtent = constraintsLocal.axis switch
        {
            Axis.horizontal => child!.size.width,
            Axis.vertical => child!.size.height,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        double paintedChildSize = calculatePaintOffset(
            constraintsLocal,
            from: 0.0,
            to: childExtent
        );
        double cacheExtentLocal = calculateCacheOffset(
            constraintsLocal,
            from: 0.0,
            to: childExtent
        );
        DartRuntimePrimitives.Assert(() => double.IsFinite(paintedChildSize));
        DartRuntimePrimitives.Assert(() => paintedChildSize >= 0.0);
        geometry = new SliverGeometry(
            scrollExtent: childExtent,
            paintExtent: paintedChildSize,
            cacheExtent: cacheExtentLocal,
            maxPaintExtent: childExtent,
            hitTestExtent: paintedChildSize,
            hasVisualOverflow: (childExtent > constraintsLocal.remainingPaintExtent)
                || (constraintsLocal.scrollOffset > 0.0)
        );
        setChildParentData(child!, constraintsLocal, geometry!);
    }
}
