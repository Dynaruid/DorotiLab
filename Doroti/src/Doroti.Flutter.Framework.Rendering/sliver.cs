// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/sliver.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Rendering;

public delegate double? ItemExtentBuilder(long index, SliverLayoutDimensions dimensions);

public class SliverLayoutDimensions
{
    public virtual double scrollOffset { get; private set; } = default!;
    public virtual double precedingScrollExtent { get; private set; } = default!;
    public virtual double viewportMainAxisExtent { get; private set; } = default!;
    public virtual double crossAxisExtent { get; private set; } = default!;

    public SliverLayoutDimensions(double scrollOffset, double precedingScrollExtent, double viewportMainAxisExtent, double crossAxisExtent)
    {
        this.scrollOffset = scrollOffset;
        this.precedingScrollExtent = precedingScrollExtent;
        this.viewportMainAxisExtent = viewportMainAxisExtent;
        this.crossAxisExtent = crossAxisExtent;
    }

    public override bool Equals(object? other)
    {
        var __other = other as SliverLayoutDimensions;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((__other is not SliverLayoutDimensions))
        {
            return false;
        }
        return ((((((SliverLayoutDimensions)((SliverLayoutDimensions)__other)).scrollOffset == this.scrollOffset) && (((SliverLayoutDimensions)((SliverLayoutDimensions)__other)).precedingScrollExtent == this.precedingScrollExtent)) && (((SliverLayoutDimensions)((SliverLayoutDimensions)__other)).viewportMainAxisExtent == this.viewportMainAxisExtent)) && (((SliverLayoutDimensions)((SliverLayoutDimensions)__other)).crossAxisExtent == this.crossAxisExtent));
    }

    public override string ToString()
    {
        return $"scrollOffset: {this.scrollOffset}" + $" precedingScrollExtent: {this.precedingScrollExtent}" + $" viewportMainAxisExtent: {this.viewportMainAxisExtent}" + $" crossAxisExtent: {this.crossAxisExtent}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.scrollOffset, this.precedingScrollExtent, this.viewportMainAxisExtent, this.crossAxisExtent);
}

public enum GrowthDirection
{
    forward,
    reverse
}

public static partial class SliverLibrary
{
    public static global::Doroti.Generated.Framework.Painting.AxisDirection applyGrowthDirectionToAxisDirection(global::Doroti.Generated.Framework.Painting.AxisDirection axisDirection, GrowthDirection growthDirection)
    {
        return (growthDirection switch { GrowthDirection.forward => axisDirection, GrowthDirection.reverse => global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.flipAxisDirection(axisDirection), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class SliverLibrary
{
    public static ScrollDirection applyGrowthDirectionToScrollDirection(ScrollDirection scrollDirection, GrowthDirection growthDirection)
    {
        return (growthDirection switch { GrowthDirection.forward => scrollDirection, GrowthDirection.reverse => global::Doroti.Generated.Framework.Rendering.Viewport_offsetLibrary.flipScrollDirection(scrollDirection), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class SliverConstraints : Constraints
{
    public virtual global::Doroti.Generated.Framework.Painting.AxisDirection axisDirection { get; private set; } = default!;
    public virtual GrowthDirection growthDirection { get; private set; } = default!;
    public virtual ScrollDirection userScrollDirection { get; private set; } = default!;
    public virtual double scrollOffset { get; private set; } = default!;
    public virtual double precedingScrollExtent { get; private set; } = default!;
    public virtual double overlap { get; private set; } = default!;
    public virtual double remainingPaintExtent { get; private set; } = default!;
    public virtual double crossAxisExtent { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.AxisDirection crossAxisDirection { get; private set; } = default!;
    public virtual double viewportMainAxisExtent { get; private set; } = default!;
    public virtual double cacheOrigin { get; private set; } = default!;
    public virtual double remainingCacheExtent { get; private set; } = default!;

    public SliverConstraints(global::Doroti.Generated.Framework.Painting.AxisDirection axisDirection, GrowthDirection growthDirection, ScrollDirection userScrollDirection, double scrollOffset, double precedingScrollExtent, double overlap, double remainingPaintExtent, double crossAxisExtent, global::Doroti.Generated.Framework.Painting.AxisDirection crossAxisDirection, double viewportMainAxisExtent, double remainingCacheExtent, double cacheOrigin)
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

    public virtual SliverConstraints copyWith(global::Doroti.Generated.Framework.Painting.AxisDirection? axisDirection = null, GrowthDirection? growthDirection = null, ScrollDirection? userScrollDirection = null, double? scrollOffset = null, double? precedingScrollExtent = null, double? overlap = null, double? remainingPaintExtent = null, double? crossAxisExtent = null, global::Doroti.Generated.Framework.Painting.AxisDirection? crossAxisDirection = null, double? viewportMainAxisExtent = null, double? remainingCacheExtent = null, double? cacheOrigin = null)
    {
        return new SliverConstraints(axisDirection: (axisDirection ?? this.axisDirection), growthDirection: (growthDirection ?? this.growthDirection), userScrollDirection: (userScrollDirection ?? this.userScrollDirection), scrollOffset: (scrollOffset ?? this.scrollOffset), precedingScrollExtent: (precedingScrollExtent ?? this.precedingScrollExtent), overlap: (overlap ?? this.overlap), remainingPaintExtent: (remainingPaintExtent ?? this.remainingPaintExtent), crossAxisExtent: (crossAxisExtent ?? this.crossAxisExtent), crossAxisDirection: (crossAxisDirection ?? this.crossAxisDirection), viewportMainAxisExtent: (viewportMainAxisExtent ?? this.viewportMainAxisExtent), remainingCacheExtent: (remainingCacheExtent ?? this.remainingCacheExtent), cacheOrigin: (cacheOrigin ?? this.cacheOrigin));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Painting.Axis axis => global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(DartRuntimePrimitives.RequireValue(this.axisDirection));
    public virtual GrowthDirection normalizedGrowthDirection
    {
        get
        {
            if (global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.axisDirectionIsReversed(DartRuntimePrimitives.RequireValue(this.axisDirection)))
            {
                return (this.growthDirection switch { GrowthDirection.forward => GrowthDirection.reverse, GrowthDirection.reverse => GrowthDirection.forward, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            }
            return this.growthDirection;
            return default!;
        }
    }
    public override bool isTight => false;
    public override bool isNormalized
    {
        get
        {
            return (((((this.scrollOffset >= 0.0) && (this.crossAxisExtent >= 0.0)) && (!object.Equals(global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(DartRuntimePrimitives.RequireValue(this.axisDirection)), global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(DartRuntimePrimitives.RequireValue(this.crossAxisDirection))))) && (this.viewportMainAxisExtent >= 0.0)) && (this.remainingPaintExtent >= 0.0));
            return default!;
        }
    }
    public virtual BoxConstraints asBoxConstraints(double minExtent = 0.0, double maxExtent = double.PositiveInfinity, double? crossAxisExtent = null)
    {
        crossAxisExtent ??= this.crossAxisExtent;
        switch (this.axis)
        {
            case global::Doroti.Generated.Framework.Painting.Axis.horizontal:
                {
                    return new BoxConstraints(minHeight: DartRuntimePrimitives.RequireValue(crossAxisExtent), maxHeight: DartRuntimePrimitives.RequireValue(crossAxisExtent), minWidth: minExtent, maxWidth: maxExtent);
                }
            case global::Doroti.Generated.Framework.Painting.Axis.vertical:
                {
                    return new BoxConstraints(minWidth: DartRuntimePrimitives.RequireValue(crossAxisExtent), maxWidth: DartRuntimePrimitives.RequireValue(crossAxisExtent), minHeight: minExtent, maxHeight: maxExtent);
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool debugAssertIsValid(bool isAppliedConstraint = false, InformationCollector? informationCollector = null)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                var hasErrors__21234 = false;
                var errorMessage__21265 = new StringBuffer("\n");
                void verify(bool check, string message)
                {
                    if (check)
                    {
                        return;
                    }
                    hasErrors__21234 = true;
                    errorMessage__21265.writeln($"  {message}");
                }
                void verifyDouble(double property, string name, bool mustBePositive = false, bool mustBeNegative = false)
                {
                    if (double.IsNaN(property))
                    {
                        var additional__21678 = ".";
                        if (mustBePositive)
                        {
                            additional__21678 = ", expected greater than or equal to zero.";
                        }
                        else
                        {
                            if (mustBeNegative)
                            {
                                additional__21678 = ", expected less than or equal to zero.";
                            }
                        }
                        verify(false, $"The \"{name}\" is NaN{additional__21678}");
                    }
                    else
                    {
                        if (mustBePositive)
                        {
                            verify((property >= 0.0), $"The \"{name}\" is negative.");
                        }
                        else
                        {
                            if (mustBeNegative)
                            {
                                verify((property <= 0.0), $"The \"{name}\" is positive.");
                            }
                        }
                    }
                }
                verifyDouble(DartRuntimePrimitives.RequireValue(this.scrollOffset), "scrollOffset");
                verifyDouble(DartRuntimePrimitives.RequireValue(this.overlap), "overlap");
                verifyDouble(DartRuntimePrimitives.RequireValue(this.crossAxisExtent), "crossAxisExtent");
                verifyDouble(DartRuntimePrimitives.RequireValue(this.scrollOffset), "scrollOffset", mustBePositive: true);
                verify((!object.Equals(global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(DartRuntimePrimitives.RequireValue(this.axisDirection)), global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(DartRuntimePrimitives.RequireValue(this.crossAxisDirection)))), "The \"axisDirection\" and the \"crossAxisDirection\" are along the same axis.");
                verifyDouble(DartRuntimePrimitives.RequireValue(this.viewportMainAxisExtent), "viewportMainAxisExtent", mustBePositive: true);
                verifyDouble(DartRuntimePrimitives.RequireValue(this.remainingPaintExtent), "remainingPaintExtent", mustBePositive: true);
                verifyDouble(DartRuntimePrimitives.RequireValue(this.remainingCacheExtent), "remainingCacheExtent", mustBePositive: true);
                verifyDouble(DartRuntimePrimitives.RequireValue(this.cacheOrigin), "cacheOrigin", mustBeNegative: true);
                verifyDouble(DartRuntimePrimitives.RequireValue(this.precedingScrollExtent), "precedingScrollExtent", mustBePositive: true);
                verify(this.isNormalized, "The constraints are not normalized.");
                if (hasErrors__21234)
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{this.GetType()} is not valid: {errorMessage__21265}"), new DiagnosticsProperty<SliverConstraints>("The offending constraints were", this, style: DiagnosticsTreeStyle.errorProperty) });
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as SliverConstraints;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((__other is not SliverConstraints))
        {
            return false;
        }
        DartRuntimePrimitives.Assert(() => ((SliverConstraints)__other).debugAssertIsValid());
        return ((((((((((((object.Equals(((SliverConstraints)((SliverConstraints)__other)).axisDirection, this.axisDirection)) && (object.Equals(((SliverConstraints)((SliverConstraints)__other)).growthDirection, this.growthDirection))) && (object.Equals(((SliverConstraints)((SliverConstraints)__other)).userScrollDirection, this.userScrollDirection))) && (((SliverConstraints)((SliverConstraints)__other)).scrollOffset == this.scrollOffset)) && (((SliverConstraints)((SliverConstraints)__other)).precedingScrollExtent == this.precedingScrollExtent)) && (((SliverConstraints)((SliverConstraints)__other)).overlap == this.overlap)) && (((SliverConstraints)((SliverConstraints)__other)).remainingPaintExtent == this.remainingPaintExtent)) && (((SliverConstraints)((SliverConstraints)__other)).crossAxisExtent == this.crossAxisExtent)) && (object.Equals(((SliverConstraints)((SliverConstraints)__other)).crossAxisDirection, this.crossAxisDirection))) && (((SliverConstraints)((SliverConstraints)__other)).viewportMainAxisExtent == this.viewportMainAxisExtent)) && (((SliverConstraints)((SliverConstraints)__other)).remainingCacheExtent == this.remainingCacheExtent)) && (((SliverConstraints)((SliverConstraints)__other)).cacheOrigin == this.cacheOrigin));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.axisDirection, this.growthDirection, this.userScrollDirection, this.scrollOffset, this.precedingScrollExtent, this.overlap, this.remainingPaintExtent, this.crossAxisExtent, this.crossAxisDirection, this.viewportMainAxisExtent, this.remainingCacheExtent, this.cacheOrigin);
    public override string ToString()
    {
        var properties__24869 = new List<string> { $"{this.axisDirection}", $"{this.growthDirection}", $"{this.userScrollDirection}", $"scrollOffset: {this.scrollOffset.toStringAsFixed(1L)}", $"precedingScrollExtent: {this.precedingScrollExtent.toStringAsFixed(1L)}", $"remainingPaintExtent: {this.remainingPaintExtent.toStringAsFixed(1L)}", $"crossAxisExtent: {this.crossAxisExtent.toStringAsFixed(1L)}", $"crossAxisDirection: {this.crossAxisDirection}", $"viewportMainAxisExtent: {this.viewportMainAxisExtent.toStringAsFixed(1L)}", $"remainingCacheExtent: {this.remainingCacheExtent.toStringAsFixed(1L)}", $"cacheOrigin: {this.cacheOrigin.toStringAsFixed(1L)}" };
        return $"SliverConstraints({string.Join(", ", properties__24869)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
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

    public SliverGeometry(double scrollExtent = 0.0, double paintExtent = 0.0, double paintOrigin = 0.0, double? layoutExtent = null, double maxPaintExtent = 0.0, double maxScrollObstructionExtent = 0.0, double? crossAxisExtent = null, double? hitTestExtent = null, bool? visible = null, bool hasVisualOverflow = false, double? scrollOffsetCorrection = null, double? cacheExtent = null)
    {
        this.scrollExtent = scrollExtent;
        this.paintExtent = paintExtent;
        this.paintOrigin = paintOrigin;
        this.maxPaintExtent = maxPaintExtent;
        this.maxScrollObstructionExtent = maxScrollObstructionExtent;
        this.crossAxisExtent = crossAxisExtent;
        this.hasVisualOverflow = hasVisualOverflow;
        this.scrollOffsetCorrection = scrollOffsetCorrection;
        this.layoutExtent = (layoutExtent ?? DartRuntimePrimitives.RequireValue(paintExtent));
        this.hitTestExtent = (hitTestExtent ?? DartRuntimePrimitives.RequireValue(paintExtent));
        this.cacheExtent = ((cacheExtent ?? layoutExtent) ?? DartRuntimePrimitives.RequireValue(paintExtent));
        this.visible = (visible ?? (DartRuntimePrimitives.RequireValue(paintExtent) > 0.0));
        System.Diagnostics.Debug.Assert((scrollOffsetCorrection != 0.0));
    }

    public virtual SliverGeometry copyWith(double? scrollExtent = null, double? paintExtent = null, double? paintOrigin = null, double? layoutExtent = null, double? maxPaintExtent = null, double? maxScrollObstructionExtent = null, double? crossAxisExtent = null, double? hitTestExtent = null, bool? visible = null, bool? hasVisualOverflow = null, double? cacheExtent = null)
    {
        return new SliverGeometry(scrollExtent: (scrollExtent ?? this.scrollExtent), paintExtent: (paintExtent ?? this.paintExtent), paintOrigin: (paintOrigin ?? this.paintOrigin), layoutExtent: (layoutExtent ?? this.layoutExtent), maxPaintExtent: (maxPaintExtent ?? this.maxPaintExtent), maxScrollObstructionExtent: (maxScrollObstructionExtent ?? this.maxScrollObstructionExtent), crossAxisExtent: (crossAxisExtent ?? this.crossAxisExtent), hitTestExtent: (hitTestExtent ?? this.hitTestExtent), visible: (visible ?? this.visible), hasVisualOverflow: (hasVisualOverflow ?? this.hasVisualOverflow), cacheExtent: (cacheExtent ?? this.cacheExtent));
        throw new InvalidOperationException("Dart control flow completed without a value.");
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
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "SliverGeometry"))} is not valid: {summary}") });
                }
                verify((this.scrollExtent >= 0.0), "The \"scrollExtent\" is negative.");
                verify((this.paintExtent >= 0.0), "The \"paintExtent\" is negative.");
                verify((this.layoutExtent >= 0.0), "The \"layoutExtent\" is negative.");
                verify((this.cacheExtent >= 0.0), "The \"cacheExtent\" is negative.");
                if ((this.layoutExtent > this.paintExtent))
                {
                    verify(false, "The \"layoutExtent\" exceeds the \"paintExtent\".", details: SliverLibrary._debugCompareFloats("paintExtent", DartRuntimePrimitives.RequireValue(this.paintExtent), "layoutExtent", DartRuntimePrimitives.RequireValue(this.layoutExtent)));
                }
                if (((this.paintExtent - this.maxPaintExtent) > global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))
                {
                    verify(false, "The \"maxPaintExtent\" is less than the \"paintExtent\".", details: ((Func<List<DiagnosticsNode>>)(() =>
{
    var __cascade = SliverLibrary._debugCompareFloats("maxPaintExtent", DartRuntimePrimitives.RequireValue(this.maxPaintExtent), "paintExtent", DartRuntimePrimitives.RequireValue(this.paintExtent));
    __cascade.Add(new ErrorDescription("By definition, a sliver can't paint more than the maximum that it can paint!"));
    return __cascade;
}))());
                }
                verify((this.hitTestExtent >= 0.0), "The \"hitTestExtent\" is negative.");
                verify((this.scrollOffsetCorrection != 0.0), "The \"scrollOffsetCorrection\" is zero.");
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string toStringShort() => global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "SliverGeometry");
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("scrollExtent", this.scrollExtent));
        if ((this.paintExtent > 0.0))
        {
            properties.add(new DoubleProperty("paintExtent", this.paintExtent, unit: (this.visible ? null : " but not painting")));
        }
        else
        {
            if ((this.paintExtent == 0.0))
            {
                if (this.visible)
                {
                    properties.add(new DoubleProperty("paintExtent", this.paintExtent, unit: (this.visible ? null : " but visible")));
                }
                properties.add(new FlagProperty("visible", value: this.visible, ifFalse: "hidden"));
            }
            else
            {
                properties.add(new DoubleProperty("paintExtent", this.paintExtent, tooltip: "!"));
            }
        }
        properties.add(new DoubleProperty("paintOrigin", this.paintOrigin, defaultValue: 0.0));
        properties.add(new DoubleProperty("layoutExtent", this.layoutExtent, defaultValue: this.paintExtent));
        properties.add(new DoubleProperty("maxPaintExtent", this.maxPaintExtent));
        properties.add(new DoubleProperty("hitTestExtent", this.hitTestExtent, defaultValue: this.paintExtent));
        properties.add(new DiagnosticsProperty<bool>("hasVisualOverflow", this.hasVisualOverflow, defaultValue: false));
        properties.add(new DoubleProperty("scrollOffsetCorrection", this.scrollOffsetCorrection, defaultValue: null));
        properties.add(new DoubleProperty("cacheExtent", this.cacheExtent, defaultValue: 0.0));
    }

}

public delegate bool SliverHitTest(SliverHitTestResult result, double crossAxisPosition, double mainAxisPosition);

public class SliverHitTestResult : HitTestResult
{
    public SliverHitTestResult()
    {
    }

    private SliverHitTestResult(HitTestResult result) : base(result)
    {
    }

    public static SliverHitTestResult CreateWrap(HitTestResult result)
    {
        return new SliverHitTestResult(result);
    }

    public virtual bool addWithAxisOffset(Offset? paintOffset, double mainAxisOffset, double crossAxisOffset, double mainAxisPosition, double crossAxisPosition, Func<SliverHitTestResult, double, double, bool> hitTest)
    {
        if ((paintOffset is not null))
        {
            Offset paintOffset__value42308 = DartRuntimePrimitives.RequireValue(paintOffset);
            pushOffset(-DartRuntimePrimitives.RequireValue(paintOffset__value42308));
        }
        bool isHit__42384 = hitTest(this, (mainAxisPosition - mainAxisOffset), (crossAxisPosition - crossAxisOffset));
        if ((paintOffset is not null))
        {
            Offset paintOffset__value42549 = DartRuntimePrimitives.RequireValue(paintOffset);
            popTransform();
        }
        return isHit__42384;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SliverHitTestEntry : HitTestEntry<RenderSliver>
{
    public virtual double mainAxisPosition { get; private set; } = default!;
    public virtual double crossAxisPosition { get; private set; } = default!;

    public SliverHitTestEntry(RenderSliver target, double mainAxisPosition, double crossAxisPosition) : base(target)
    {
        this.mainAxisPosition = mainAxisPosition;
        this.crossAxisPosition = crossAxisPosition;
    }

    public override string ToString() => $"{DartRuntimePrimitives.RuntimeType(target)}@(mainAxis: {this.mainAxisPosition}, crossAxis: {this.crossAxisPosition})";
}

public class SliverLogicalParentData : ParentData
{
    public virtual double? layoutOffset { get; set; } = default;

    public override string ToString() => $"layoutOffset={((this.layoutOffset is null) ? "None" : DartRuntimePrimitives.RequireValue(this.layoutOffset).toStringAsFixed(1L))}";
}

public class SliverLogicalContainerParentData : SliverLogicalParentData, ContainerParentDataMixin<RenderSliver>
{
    public virtual RenderSliver? previousSibling { get; set; } = default;
    public virtual RenderSliver? nextSibling { get; set; } = default;

    public override void detach()
    {
        DartRuntimePrimitives.Assert(() => (this.previousSibling is null));
        DartRuntimePrimitives.Assert(() => (this.nextSibling is null));
        base.detach();
    }

}

public class SliverPhysicalParentData : ParentData
{
    public virtual Offset paintOffset { get; set; } = Offset.zero;
    public virtual long? crossAxisFlex { get; set; } = default;

    public virtual void applyPaintTransform(Matrix4 transform)
    {
        transform.translateByDouble(this.paintOffset.dx, this.paintOffset.dy, 0, 1);
    }

    public override string ToString() => $"paintOffset={this.paintOffset}";
}

public class SliverPhysicalContainerParentData : SliverPhysicalParentData, ContainerParentDataMixin<RenderSliver>
{
    public virtual RenderSliver? previousSibling { get; set; } = default;
    public virtual RenderSliver? nextSibling { get; set; } = default;

    public override void detach()
    {
        DartRuntimePrimitives.Assert(() => (this.previousSibling is null));
        DartRuntimePrimitives.Assert(() => (this.nextSibling is null));
        base.detach();
    }

}

public static partial class SliverLibrary
{
    internal static List<DiagnosticsNode> _debugCompareFloats(string labelA, double valueA, string labelB, double valueB)
    {
        return new List<DiagnosticsNode>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public abstract class RenderSliver : RenderObject
{
    internal virtual SliverGeometry? _geometry { get; set; } = default;
    public RenderSliver() { }


    public virtual bool ensureSemantics => false;
    public override SliverConstraints constraints => ((SliverConstraints?)(object?)base.constraints)!;
    public virtual SliverGeometry? geometry
    {
        get => this._geometry;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => !((debugDoingThisResize && debugDoingThisLayout)));
            DartRuntimePrimitives.Assert(() => (sizedByParent || !debugDoingThisResize));
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((((sizedByParent && debugDoingThisResize)) || ((!sizedByParent && debugDoingThisLayout))))
                    {
                        return true;
                    }
                    DartRuntimePrimitives.Assert(() => !debugDoingThisResize);
                    DiagnosticsNode? contract__57635 = default!;
                    DiagnosticsNode? violation__57645 = default!;
                    DiagnosticsNode? hint__57656 = default!;
                    if (debugDoingThisLayout)
                    {
                        DartRuntimePrimitives.Assert(() => sizedByParent);
                        violation__57645 = new ErrorDescription("It appears that the geometry setter was called from performLayout().");
                    }
                    else
                    {
                        violation__57645 = new ErrorDescription("The geometry setter was called from outside layout (neither performResize() nor performLayout() were being run for this object).");
                        if (((owner is not null) && owner!.debugDoingLayout))
                        {
                            hint__57656 = new ErrorDescription("Only the object itself can set its geometry. It is a contract violation for other objects to set it.");
                        }
                    }
                    if (sizedByParent)
                    {
                        contract__57635 = new ErrorDescription("Because this RenderSliver has sizedByParent set to true, it must set its geometry in performResize().");
                    }
                    else
                    {
                        contract__57635 = new ErrorDescription("Because this RenderSliver has sizedByParent set to false, it must set its geometry in performLayout().");
                    }
                    var information__58692 = new List<DiagnosticsNode> { new ErrorSummary("RenderSliver geometry setter called incorrectly."), violation__57645, contract__57635, describeForError("The RenderSliver in question is") };
                    throw new FlutterError(information__58692);
                });
            _geometry = __value;
        }
    }
    public override Rect semanticBounds => this.paintBounds;
    public override Rect paintBounds
    {
        get
        {
            switch (((SliverConstraints)this.constraints).axis)
            {
                case global::Doroti.Generated.Framework.Painting.Axis.horizontal:
                    {
                        return global::Doroti.Flutter.Ui.Rect.fromLTWH(0.0, 0.0, this.geometry!.paintExtent, ((SliverConstraints)this.constraints).crossAxisExtent);
                    }
                case global::Doroti.Generated.Framework.Painting.Axis.vertical:
                    {
                        return global::Doroti.Flutter.Ui.Rect.fromLTWH(0.0, 0.0, ((SliverConstraints)this.constraints).crossAxisExtent, this.geometry!.paintExtent);
                    }
            }
            return default!;
        }
    }
    public override void debugResetSize()
    {
    }

    public override void debugAssertDoesMeetConstraints()
    {
        DartRuntimePrimitives.Assert(() => this.geometry!.debugAssertIsValid(informationCollector: ((InformationCollector)(() => new List<DiagnosticsNode> { describeForError("The RenderSliver that returned the offending geometry was") }))));
        DartRuntimePrimitives.Assert(() =>
            {
                if (((this.geometry!.paintOrigin + this.geometry!.paintExtent) > ((SliverConstraints)this.constraints).remainingPaintExtent))
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("SliverGeometry has a paintOffset that exceeds the remainingPaintExtent from the constraints."), describeForError("The render object whose geometry violates the constraints is the following"), new ErrorDescription("The paintOrigin and paintExtent must cause the child sliver to paint " + "within the viewport, and so cannot exceed the remainingPaintExtent.") });
                }
                return true;
            });
    }

    public override void performResize()
    {
        DartRuntimePrimitives.Assert(() => false);
    }

    public virtual double centerOffsetAdjustment => 0.0;
    public virtual bool hitTest(SliverHitTestResult result, double mainAxisPosition, double crossAxisPosition)
    {
        if (((((mainAxisPosition >= 0.0) && (mainAxisPosition < this.geometry!.hitTestExtent)) && (crossAxisPosition >= 0.0)) && (crossAxisPosition < ((SliverConstraints)this.constraints).crossAxisExtent)))
        {
            if ((hitTestChildren(result, mainAxisPosition: mainAxisPosition, crossAxisPosition: crossAxisPosition) || hitTestSelf(mainAxisPosition: mainAxisPosition, crossAxisPosition: crossAxisPosition)))
            {
                result.add(new SliverHitTestEntry(this, mainAxisPosition: mainAxisPosition, crossAxisPosition: crossAxisPosition));
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool hitTestSelf(double mainAxisPosition, double crossAxisPosition) => false;
    public virtual bool hitTestChildren(SliverHitTestResult result, double mainAxisPosition, double crossAxisPosition) => false;
    public virtual double calculatePaintOffset(SliverConstraints constraints, double from, double to)
    {
        DartRuntimePrimitives.Assert(() => (from <= to));
        double a__66523 = ((SliverConstraints)constraints).scrollOffset;
        double b__66570 = (((SliverConstraints)constraints).scrollOffset + ((SliverConstraints)constraints).remainingPaintExtent);
        return Dart_uiLibrary.clampDouble((Dart_uiLibrary.clampDouble(to, a__66523, b__66570) - Dart_uiLibrary.clampDouble(from, a__66523, b__66570)), 0.0, ((SliverConstraints)constraints).remainingPaintExtent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double calculateCacheOffset(SliverConstraints constraints, double from, double to)
    {
        DartRuntimePrimitives.Assert(() => (from <= to));
        double a__67506 = (((SliverConstraints)constraints).scrollOffset + ((SliverConstraints)constraints).cacheOrigin);
        double b__67579 = (((SliverConstraints)constraints).scrollOffset + ((SliverConstraints)constraints).remainingCacheExtent);
        return Dart_uiLibrary.clampDouble((Dart_uiLibrary.clampDouble(to, a__67506, b__67579) - Dart_uiLibrary.clampDouble(from, a__67506, b__67579)), 0.0, ((SliverConstraints)constraints).remainingCacheExtent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double childMainAxisPosition(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                throw new FlutterError($"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "RenderSliver"))} does not implement childPosition.");
            });
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double childCrossAxisPosition(RenderObject child) => 0.0;
    public virtual double? childScrollOffset(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((RenderObject)child).parent, this)));
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                throw new FlutterError($"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "RenderSliver"))} does not implement applyPaintTransform.");
            });
    }

    public virtual global::Doroti.Flutter.Ui.Size getAbsoluteSizeRelativeToOrigin()
    {
        DartRuntimePrimitives.Assert(() => (this.geometry is not null));
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        return (SliverLibrary.applyGrowthDirectionToAxisDirection(((SliverConstraints)this.constraints).axisDirection, ((SliverConstraints)this.constraints).growthDirection) switch { global::Doroti.Generated.Framework.Painting.AxisDirection.up => new global::Doroti.Flutter.Ui.Size(((SliverConstraints)this.constraints).crossAxisExtent, -this.geometry!.paintExtent), global::Doroti.Generated.Framework.Painting.AxisDirection.down => new global::Doroti.Flutter.Ui.Size(((SliverConstraints)this.constraints).crossAxisExtent, this.geometry!.paintExtent), global::Doroti.Generated.Framework.Painting.AxisDirection.left => new global::Doroti.Flutter.Ui.Size(-this.geometry!.paintExtent, ((SliverConstraints)this.constraints).crossAxisExtent), global::Doroti.Generated.Framework.Painting.AxisDirection.right => new global::Doroti.Flutter.Ui.Size(this.geometry!.paintExtent, ((SliverConstraints)this.constraints).crossAxisExtent), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Flutter.Ui.Size getAbsoluteSize()
    {
        DartRuntimePrimitives.Assert(() => (this.geometry is not null));
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        switch (((SliverConstraints)this.constraints).axisDirection)
        {
            case global::Doroti.Generated.Framework.Painting.AxisDirection.up:
            case global::Doroti.Generated.Framework.Painting.AxisDirection.down:
                {
                    return new global::Doroti.Flutter.Ui.Size(((SliverConstraints)this.constraints).crossAxisExtent, this.geometry!.paintExtent);
                }
            case global::Doroti.Generated.Framework.Painting.AxisDirection.right:
            case global::Doroti.Generated.Framework.Painting.AxisDirection.left:
                {
                    return new global::Doroti.Flutter.Ui.Size(this.geometry!.paintExtent, ((SliverConstraints)this.constraints).crossAxisExtent);
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Flutter.Ui.Rect getMaxPaintRect()
    {
        SliverGeometry? sliverGeometry__73541 = this.geometry;
        if (((sliverGeometry__73541 is null) || (object.Equals(sliverGeometry__73541, SliverGeometry.zero))))
        {
            return Rect.zero;
        }
        double maxPaintExtent__73685 = ((SliverGeometry)sliverGeometry__73541).maxPaintExtent;
        if (double.IsInfinity(maxPaintExtent__73685))
        {
            maxPaintExtent__73685 = ((((SliverConstraints)this.constraints).scrollOffset + ((SliverGeometry)sliverGeometry__73541).cacheExtent) + ((SliverConstraints)this.constraints).cacheOrigin);
        }
        double paintExtent__73907 = ((SliverGeometry)sliverGeometry__73541).paintExtent;
        double leadingOffset__74129 = Dart_uiLibrary.clampDouble(((SliverConstraints)this.constraints).scrollOffset, 0.0, (((SliverGeometry)sliverGeometry__73541).scrollExtent - ((SliverGeometry)sliverGeometry__73541).maxScrollObstructionExtent));
        double crossAxisExtent__74304 = (((SliverGeometry)sliverGeometry__73541).crossAxisExtent ?? ((SliverConstraints)this.constraints).crossAxisExtent);
        global::Doroti.Flutter.Ui.Rect rect__74401 = (((SliverConstraints)this.constraints).axis switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => global::Doroti.Flutter.Ui.Rect.fromLTWH(-leadingOffset__74129, 0.0, maxPaintExtent__73685, crossAxisExtent__74304), global::Doroti.Generated.Framework.Painting.Axis.vertical => global::Doroti.Flutter.Ui.Rect.fromLTWH(0.0, -leadingOffset__74129, crossAxisExtent__74304, maxPaintExtent__73685), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return (SliverLibrary.applyGrowthDirectionToAxisDirection(((SliverConstraints)this.constraints).axisDirection, ((SliverConstraints)this.constraints).growthDirection) switch { global::Doroti.Generated.Framework.Painting.AxisDirection.right => rect__74401, global::Doroti.Generated.Framework.Painting.AxisDirection.down => rect__74401, global::Doroti.Generated.Framework.Painting.AxisDirection.left => global::Doroti.Flutter.Ui.Rect.fromLTRB((paintExtent__73907 - rect__74401.right), rect__74401.top, (paintExtent__73907 - rect__74401.left), rect__74401.bottom), global::Doroti.Generated.Framework.Painting.AxisDirection.up => global::Doroti.Flutter.Ui.Rect.fromLTRB(rect__74401.left, (paintExtent__73907 - rect__74401.bottom), rect__74401.right, (paintExtent__73907 - rect__74401.top)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _debugDrawArrow(Canvas canvas, Paint paint, Offset p0, Offset p1, GrowthDirection direction)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((object.Equals(p0, p1)))
                {
                    return true;
                }
                DartRuntimePrimitives.Assert(() => ((p0.dx == p1.dx) || (p0.dy == p1.dy)));
                double d__75431 = (((p1 - p0)).distance * 0.2);
                global::Doroti.Flutter.Ui.Offset temp__75480 = default!;
                double dx1__75499 = default!;
                double dx2__75504 = default!;
                double dy1__75509 = default!;
                double dy2__75514 = default!;
                switch (direction)
                {
                    case GrowthDirection.forward:
                        {
                            dx1__75499 = dx2__75504 = dy1__75509 = dy2__75514 = d__75431;
                            break;
                        }
                    case GrowthDirection.reverse:
                        {
                            temp__75480 = p0;
                            p0 = p1;
                            p1 = temp__75480;
                            dx1__75499 = dx2__75504 = dy1__75509 = dy2__75514 = -d__75431;
                            break;
                        }
                }
                if ((p0.dx == p1.dx))
                {
                    dx2__75504 = -dx2__75504;
                }
                else
                {
                    dy2__75514 = -dy2__75514;
                }
                canvas.drawPath(((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Flutter.Ui.Path();
    __cascade.moveTo(p0.dx, p0.dy);
    __cascade.lineTo(p1.dx, p1.dy);
    __cascade.moveTo((p1.dx - dx1__75499), (p1.dy - dy1__75509));
    __cascade.lineTo(p1.dx, p1.dy);
    __cascade.lineTo((p1.dx - dx2__75504), (p1.dy - dy2__75514));
    return __cascade;
}))(), paint);
                return true;
            });
    }

    public override void debugPaint(PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugPaintSizeEnabled)
                {
                    double strokeWidth__76287 = Math.Min(4.0, (this.geometry!.paintExtent / 30.0));
                    var paint__76360 = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Flutter.Ui.Paint();
    __cascade.color = new global::Doroti.Flutter.Ui.Color(4281584691L);
    __cascade.strokeWidth = strokeWidth__76287;
    __cascade.style = PaintingStyle.stroke;
    __cascade.maskFilter = global::Doroti.Flutter.Ui.MaskFilter.blur(BlurStyle.solid, strokeWidth__76287);
    return __cascade;
}))();
                    double arrowExtent__76592 = this.geometry!.paintExtent;
                    double padding__76650 = Math.Max(2.0, strokeWidth__76287);
                    global::Doroti.Flutter.Ui.Canvas canvas__76709 = ((PaintingContext)context).canvas;
                    canvas__76709.drawCircle(offset.translate(padding__76650, padding__76650), (padding__76650 * 0.5), paint__76360);
                    switch (((SliverConstraints)this.constraints).axis)
                    {
                        case global::Doroti.Generated.Framework.Painting.Axis.vertical:
                            {
                                canvas__76709.drawLine(offset, offset.translate(((SliverConstraints)this.constraints).crossAxisExtent, 0.0), paint__76360);
                                _debugDrawArrow(canvas__76709, paint__76360, offset.translate(((((SliverConstraints)this.constraints).crossAxisExtent * 1.0) / 4.0), padding__76650), offset.translate(((((SliverConstraints)this.constraints).crossAxisExtent * 1.0) / 4.0), (arrowExtent__76592 - padding__76650)), ((SliverConstraints)this.constraints).normalizedGrowthDirection);
                                _debugDrawArrow(canvas__76709, paint__76360, offset.translate(((((SliverConstraints)this.constraints).crossAxisExtent * 3.0) / 4.0), padding__76650), offset.translate(((((SliverConstraints)this.constraints).crossAxisExtent * 3.0) / 4.0), (arrowExtent__76592 - padding__76650)), ((SliverConstraints)this.constraints).normalizedGrowthDirection);
                                break;
                            }
                        case global::Doroti.Generated.Framework.Painting.Axis.horizontal:
                            {
                                canvas__76709.drawLine(offset, offset.translate(0.0, ((SliverConstraints)this.constraints).crossAxisExtent), paint__76360);
                                _debugDrawArrow(canvas__76709, paint__76360, offset.translate(padding__76650, ((((SliverConstraints)this.constraints).crossAxisExtent * 1.0) / 4.0)), offset.translate((arrowExtent__76592 - padding__76650), ((((SliverConstraints)this.constraints).crossAxisExtent * 1.0) / 4.0)), ((SliverConstraints)this.constraints).normalizedGrowthDirection);
                                _debugDrawArrow(canvas__76709, paint__76360, offset.translate(padding__76650, ((((SliverConstraints)this.constraints).crossAxisExtent * 3.0) / 4.0)), offset.translate((arrowExtent__76592 - padding__76650), ((((SliverConstraints)this.constraints).crossAxisExtent * 3.0) / 4.0)), ((SliverConstraints)this.constraints).normalizedGrowthDirection);
                                break;
                            }
                    }
                }
                return true;
            });
    }

    public override void handleEvent(global::Doroti.Generated.Framework.Gestures.PointerEvent @event, HitTestEntry<HitTestTarget> entry)
    {
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<SliverGeometry>("geometry", this.geometry));
    }

}

public interface RenderSliverHelpers
{
    public bool _getRightWayUp(SliverConstraints constraints);
    public bool hitTestBoxChild(BoxHitTestResult result, RenderBox child, double mainAxisPosition, double crossAxisPosition);
    public void applyPaintTransformForBoxChild(RenderBox child, Matrix4 transform);
}

public abstract class RenderSliverSingleBoxAdapter : RenderSliver, RenderObjectWithChildMixin<RenderBox>, RenderSliverHelpers
{
    public virtual RenderBox? _child { get; set; } = default;

    protected RenderSliverSingleBoxAdapter(RenderBox? child = null)
    {
    }

    public override void setupParentData(RenderObject child)
    {
        if ((((RenderObject)child).parentData is not SliverPhysicalParentData))
        {
            child.parentData = new SliverPhysicalParentData();
        }
    }

    public virtual void setChildParentData(RenderObject child, SliverConstraints constraints, SliverGeometry geometry)
    {
        var childParentData__83575 = ((SliverPhysicalParentData?)(object?)((RenderObject)child).parentData!)!;
        childParentData__83575.paintOffset = (SliverLibrary.applyGrowthDirectionToAxisDirection(((SliverConstraints)constraints).axisDirection, ((SliverConstraints)constraints).growthDirection) switch { global::Doroti.Generated.Framework.Painting.AxisDirection.up => new global::Doroti.Flutter.Ui.Offset(0.0, ((((SliverGeometry)geometry).paintExtent + ((SliverConstraints)constraints).scrollOffset) - ((SliverGeometry)geometry).scrollExtent)), global::Doroti.Generated.Framework.Painting.AxisDirection.left => new global::Doroti.Flutter.Ui.Offset(((((SliverGeometry)geometry).paintExtent + ((SliverConstraints)constraints).scrollOffset) - ((SliverGeometry)geometry).scrollExtent), 0.0), global::Doroti.Generated.Framework.Painting.AxisDirection.right => new global::Doroti.Flutter.Ui.Offset(-((SliverConstraints)constraints).scrollOffset, 0.0), global::Doroti.Generated.Framework.Painting.AxisDirection.down => new global::Doroti.Flutter.Ui.Offset(0.0, -((SliverConstraints)constraints).scrollOffset), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    }

    public override bool hitTestChildren(SliverHitTestResult result, double mainAxisPosition, double crossAxisPosition)
    {
        DartRuntimePrimitives.Assert(() => (geometry!.hitTestExtent > 0.0));
        if ((child is not null))
        {
            return hitTestBoxChild(BoxHitTestResult.CreateWrap(result), child!, mainAxisPosition: mainAxisPosition, crossAxisPosition: crossAxisPosition);
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double childMainAxisPosition(RenderObject child)
    {
        var __child = (RenderBox)(object)child;
        return -((SliverConstraints)constraints).scrollOffset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child, this.child)));
        var childParentData__84884 = ((SliverPhysicalParentData?)(object?)((RenderObject)child).parentData!)!;
        childParentData__84884.applyPaintTransform(transform);
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (((child is not null) && geometry!.visible))
        {
            var childParentData__85131 = ((SliverPhysicalParentData?)(object?)child!.parentData!)!;
            context.paintChild(child!, (offset + ((SliverPhysicalParentData)childParentData__85131).paintOffset));
        }
    }

    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((child is not RenderBox))
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"A {this.GetType()} expected a child of type {typeof(RenderBox)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {this.GetType()} that expected a {typeof(RenderBox)} child was created by", debugCreator, style: DiagnosticsTreeStyle.errorProperty), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", ((RenderObject)child).debugCreator, style: DiagnosticsTreeStyle.errorProperty) });
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? child
    {
        get => this._child;
        set
        {
            var __value = value;
            if ((this._child is not null))
            {
                dropChild(this._child!);
            }
            this._child = __value;
            if ((this._child is not null))
            {
                adoptChild(this._child!);
            }
        }
    }
    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        this._child?.attach(owner);
    }

    public override void detach()
    {
        base.detach();
        this._child?.detach();
    }

    public override void redepthChildren()
    {
        if ((this._child is not null))
        {
            redepthChild(this._child!);
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        if ((this._child is not null))
        {
            visitor(this._child!);
        }
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        return ((this.child is not null) ? new List<DiagnosticsNode> { ((Diagnosticable)this.child!).toDiagnosticsNode(name: "child") } : new List<DiagnosticsNode>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _getRightWayUp(SliverConstraints constraints)
    {
        bool reversed__78998 = global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.axisDirectionIsReversed(((SliverConstraints)constraints).axisDirection);
        return (((SliverConstraints)constraints).growthDirection switch { GrowthDirection.forward => !reversed__78998, GrowthDirection.reverse => reversed__78998, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool hitTestBoxChild(BoxHitTestResult result, RenderBox child, double mainAxisPosition, double crossAxisPosition)
    {
        bool rightWayUp__79845 = _getRightWayUp(constraints);
        double delta__79898 = childMainAxisPosition(child);
        double crossAxisDelta__79953 = childCrossAxisPosition(child);
        double absolutePosition__80012 = (mainAxisPosition - delta__79898);
        double absoluteCrossAxisPosition__80074 = (crossAxisPosition - crossAxisDelta__79953);
        global::Doroti.Flutter.Ui.Offset paintOffset__80149 = default!;
        global::Doroti.Flutter.Ui.Offset transformedPosition__80162 = default!;
        switch (((SliverConstraints)constraints).axis)
        {
            case global::Doroti.Generated.Framework.Painting.Axis.horizontal:
                {
                    if (!rightWayUp__79845)
                    {
                        absolutePosition__80012 = (((RenderBox)child).size.width - absolutePosition__80012);
                        delta__79898 = ((geometry!.paintExtent - ((RenderBox)child).size.width) - delta__79898);
                    }
                    paintOffset__80149 = new global::Doroti.Flutter.Ui.Offset(delta__79898, crossAxisDelta__79953);
                    transformedPosition__80162 = new global::Doroti.Flutter.Ui.Offset(absolutePosition__80012, absoluteCrossAxisPosition__80074);
                    break;
                }
            case global::Doroti.Generated.Framework.Painting.Axis.vertical:
                {
                    if (!rightWayUp__79845)
                    {
                        absolutePosition__80012 = (((RenderBox)child).size.height - absolutePosition__80012);
                        delta__79898 = ((geometry!.paintExtent - ((RenderBox)child).size.height) - delta__79898);
                    }
                    paintOffset__80149 = new global::Doroti.Flutter.Ui.Offset(crossAxisDelta__79953, delta__79898);
                    transformedPosition__80162 = new global::Doroti.Flutter.Ui.Offset(absoluteCrossAxisPosition__80074, absolutePosition__80012);
                    break;
                }
        }
        return result.addWithOutOfBandPosition(paintOffset: paintOffset__80149, hitTest: ((Func<BoxHitTestResult, bool>)((result) =>
        {
            return child.hitTest(result, position: transformedPosition__80162);
            return default;
        })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void applyPaintTransformForBoxChild(RenderBox child, Matrix4 transform)
    {
        bool rightWayUp__81586 = _getRightWayUp(constraints);
        double delta__81639 = childMainAxisPosition(child);
        double crossAxisDelta__81694 = childCrossAxisPosition(child);
        switch (((SliverConstraints)constraints).axis)
        {
            case global::Doroti.Generated.Framework.Painting.Axis.horizontal:
                {
                    if (!rightWayUp__81586)
                    {
                        delta__81639 = ((geometry!.paintExtent - ((RenderBox)child).size.width) - delta__81639);
                    }
                    transform.translateByDouble(delta__81639, crossAxisDelta__81694, 0, 1);
                    break;
                }
            case global::Doroti.Generated.Framework.Painting.Axis.vertical:
                {
                    if (!rightWayUp__81586)
                    {
                        delta__81639 = ((geometry!.paintExtent - ((RenderBox)child).size.height) - delta__81639);
                    }
                    transform.translateByDouble(crossAxisDelta__81694, delta__81639, 0, 1);
                    break;
                }
        }
    }

}

public class RenderSliverToBoxAdapter : RenderSliverSingleBoxAdapter
{
    public RenderSliverToBoxAdapter(RenderBox? child = null) : base(child: child)
    {
    }

    public override void performLayout()
    {
        if ((child is null))
        {
            geometry = SliverGeometry.zero;
            return;
        }
        SliverConstraints constraints__86174 = this.constraints;
        child!.layout(constraints__86174.asBoxConstraints(), parentUsesSize: true);
        double childExtent__86296 = (((SliverConstraints)constraints__86174).axis switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => child!.size.width, global::Doroti.Generated.Framework.Painting.Axis.vertical => child!.size.height, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        double paintedChildSize__86449 = calculatePaintOffset(constraints__86174, from: 0.0, to: childExtent__86296);
        double cacheExtent__86548 = calculateCacheOffset(constraints__86174, from: 0.0, to: childExtent__86296);
        DartRuntimePrimitives.Assert(() => double.IsFinite(paintedChildSize__86449));
        DartRuntimePrimitives.Assert(() => (paintedChildSize__86449 >= 0.0));
        geometry = new SliverGeometry(scrollExtent: childExtent__86296, paintExtent: paintedChildSize__86449, cacheExtent: cacheExtent__86548, maxPaintExtent: childExtent__86296, hitTestExtent: paintedChildSize__86449, hasVisualOverflow: ((childExtent__86296 > ((SliverConstraints)constraints__86174).remainingPaintExtent) || (((SliverConstraints)constraints__86174).scrollOffset > 0.0)));
        setChildParentData(child!, constraints__86174, geometry!);
    }

}
