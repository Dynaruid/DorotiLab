// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/box.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

internal class _DebugSize__box : Size
{
    internal virtual RenderBox _owner { get; private set; } = default!;
    internal virtual bool _canBeUsedByParent { get; private set; } = default!;

    internal _DebugSize__box(Size source, RenderBox _owner, bool _canBeUsedByParent) : base(source)
    {
        this._owner = _owner;
        this._canBeUsedByParent = _canBeUsedByParent;
    }

}

public class BoxConstraints : Constraints
{
    public virtual double minWidth { get; private set; } = default!;
    public virtual double maxWidth { get; private set; } = default!;
    public virtual double minHeight { get; private set; } = default!;
    public virtual double maxHeight { get; private set; } = default!;

    public BoxConstraints(double minWidth = 0.0, double maxWidth = double.PositiveInfinity, double minHeight = 0.0, double maxHeight = double.PositiveInfinity)
    {
        this.minWidth = minWidth;
        this.maxWidth = maxWidth;
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;
    }

    public static BoxConstraints CreateTight(Size size)
    {
        var __instance = new BoxConstraints(default!, default!, default!, default!);
        __instance.minWidth = size.width;
        __instance.maxWidth = size.width;
        __instance.minHeight = size.height;
        __instance.maxHeight = size.height;
        return __instance;
    }

    public static BoxConstraints CreateTightFor(double? width = null, double? height = null)
    {
        var __instance = new BoxConstraints(default!, default!, default!, default!);
        __instance.minWidth = width ?? 0.0;
        __instance.maxWidth = width ?? double.PositiveInfinity;
        __instance.minHeight = height ?? 0.0;
        __instance.maxHeight = height ?? double.PositiveInfinity;
        return __instance;
    }

    public static BoxConstraints CreateTightForFinite(double width = double.PositiveInfinity, double height = double.PositiveInfinity)
    {
        var __instance = new BoxConstraints(default!, default!, default!, default!);
        __instance.minWidth = (DartRuntimePrimitives.RequireValue(width) != double.PositiveInfinity) ? DartRuntimePrimitives.RequireValue(width) : 0.0;
        __instance.maxWidth = (DartRuntimePrimitives.RequireValue(width) != double.PositiveInfinity) ? DartRuntimePrimitives.RequireValue(width) : double.PositiveInfinity;
        __instance.minHeight = (DartRuntimePrimitives.RequireValue(height) != double.PositiveInfinity) ? DartRuntimePrimitives.RequireValue(height) : 0.0;
        __instance.maxHeight = (DartRuntimePrimitives.RequireValue(height) != double.PositiveInfinity) ? DartRuntimePrimitives.RequireValue(height) : double.PositiveInfinity;
        return __instance;
    }

    public static BoxConstraints CreateLoose(Size size)
    {
        var __instance = new BoxConstraints(default!, default!, default!, default!);
        __instance.minWidth = 0.0;
        __instance.maxWidth = size.width;
        __instance.minHeight = 0.0;
        __instance.maxHeight = size.height;
        return __instance;
    }

    public static BoxConstraints CreateExpand(double? width = null, double? height = null)
    {
        var __instance = new BoxConstraints(default!, default!, default!, default!);
        __instance.minWidth = width ?? double.PositiveInfinity;
        __instance.maxWidth = width ?? double.PositiveInfinity;
        __instance.minHeight = height ?? double.PositiveInfinity;
        __instance.maxHeight = height ?? double.PositiveInfinity;
        return __instance;
    }

    public static BoxConstraints CreateFromViewConstraints(ViewConstraints constraints)
    {
        var __instance = new BoxConstraints(default!, default!, default!, default!);
        __instance.minWidth = constraints.minWidth;
        __instance.maxWidth = constraints.maxWidth;
        __instance.minHeight = constraints.minHeight;
        __instance.maxHeight = constraints.maxHeight;
        return __instance;
    }

    public virtual BoxConstraints copyWith(double? minWidth = null, double? maxWidth = null, double? minHeight = null, double? maxHeight = null)
    {
        return new BoxConstraints(minWidth: minWidth ?? this.minWidth, maxWidth: maxWidth ?? this.maxWidth, minHeight: minHeight ?? this.minHeight, maxHeight: maxHeight ?? this.maxHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BoxConstraints deflate(EdgeInsetsGeometry edges)
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        double horizontalLocal = edges.horizontal;
        double verticalLocal = edges.vertical;
        double deflatedMinWidth = Math.Max(0.0, minWidth - horizontalLocal);
        double deflatedMinHeight = Math.Max(0.0, minHeight - verticalLocal);
        return new BoxConstraints(minWidth: deflatedMinWidth, maxWidth: Math.Max(deflatedMinWidth, maxWidth - horizontalLocal), minHeight: deflatedMinHeight, maxHeight: Math.Max(deflatedMinHeight, maxHeight - verticalLocal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BoxConstraints loosen()
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        return new BoxConstraints(maxWidth: maxWidth, maxHeight: maxHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BoxConstraints enforce(BoxConstraints constraints)
    {
        return new BoxConstraints(minWidth: Dart_uiLibrary.clampDouble(minWidth, constraints.minWidth, constraints.maxWidth), maxWidth: Dart_uiLibrary.clampDouble(maxWidth, constraints.minWidth, constraints.maxWidth), minHeight: Dart_uiLibrary.clampDouble(minHeight, constraints.minHeight, constraints.maxHeight), maxHeight: Dart_uiLibrary.clampDouble(maxHeight, constraints.minHeight, constraints.maxHeight));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BoxConstraints tighten(double? width = null, double? height = null)
    {
        return new BoxConstraints(minWidth: (width is null) ? minWidth : Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(width), minWidth, maxWidth), maxWidth: (width is null) ? maxWidth : Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(width), minWidth, maxWidth), minHeight: (height is null) ? minHeight : Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(height), minHeight, maxHeight), maxHeight: (height is null) ? maxHeight : Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(height), minHeight, maxHeight));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BoxConstraints flipped
    {
        get
        {
            return new BoxConstraints(minWidth: minHeight, maxWidth: maxHeight, minHeight: minWidth, maxHeight: maxWidth);
        }
    }
    public virtual BoxConstraints widthConstraints() => new BoxConstraints(minWidth: minWidth, maxWidth: maxWidth);
    public virtual BoxConstraints heightConstraints() => new BoxConstraints(minHeight: minHeight, maxHeight: maxHeight);
    public virtual double constrainWidth(double width = double.PositiveInfinity)
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        return Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(width), minWidth, maxWidth);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double constrainHeight(double height = double.PositiveInfinity)
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        return Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(height), minHeight, maxHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Size _debugPropagateDebugSize(Size size, Size result)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (size is _DebugSize__box)
                {
                    _DebugSize__box size__as10760 = (_DebugSize__box)size;
                    result = new _DebugSize__box(result, size__as10760._owner, size__as10760._canBeUsedByParent);
                }
                return true;
            });
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Size constrain(Size size)
    {
        var result = new Size(constrainWidth(DartRuntimePrimitives.RequireValue(size.width)), constrainHeight(DartRuntimePrimitives.RequireValue(size.height)));
        DartRuntimePrimitives.Assert(() =>
            {
                result = _debugPropagateDebugSize(size, result);
                return true;
            });
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Size constrainDimensions(double width, double height)
    {
        return new Size(constrainWidth(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(width))), constrainHeight(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(height))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Size constrainSizeAndAttemptToPreserveAspectRatio(Size size)
    {
        if (isTight)
        {
            Size result = smallest;
            DartRuntimePrimitives.Assert(() =>
                {
                    result = _debugPropagateDebugSize(size, result);
                    return true;
                });
            return result;
        }
        if (size.isEmpty)
        {
            return constrain(size);
        }
        double widthLocal = size.width;
        double heightLocal = size.height;
        double aspectRatio = DartRuntimePrimitives.RequireValue(widthLocal) / DartRuntimePrimitives.RequireValue(heightLocal);
        if (DartRuntimePrimitives.RequireValue(widthLocal) > maxWidth)
        {
            widthLocal = maxWidth;
            heightLocal = DartRuntimePrimitives.RequireValue(widthLocal) / aspectRatio;
        }
        if (DartRuntimePrimitives.RequireValue(heightLocal) > maxHeight)
        {
            heightLocal = maxHeight;
            widthLocal = DartRuntimePrimitives.RequireValue(heightLocal) * aspectRatio;
        }
        if (DartRuntimePrimitives.RequireValue(widthLocal) < minWidth)
        {
            widthLocal = minWidth;
            heightLocal = DartRuntimePrimitives.RequireValue(widthLocal) / aspectRatio;
        }
        if (DartRuntimePrimitives.RequireValue(heightLocal) < minHeight)
        {
            heightLocal = minHeight;
            widthLocal = DartRuntimePrimitives.RequireValue(heightLocal) * aspectRatio;
        }
        var resultLocal = new Size(constrainWidth(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(widthLocal))), constrainHeight(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(heightLocal))));
        DartRuntimePrimitives.Assert(() =>
            {
                resultLocal = _debugPropagateDebugSize(size, resultLocal);
                return true;
            });
        return resultLocal;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Size biggest => new Size(constrainWidth(), constrainHeight());
    public virtual Size smallest => new Size(constrainWidth(0.0), constrainHeight(0.0));
    public virtual bool hasTightWidth => minWidth >= maxWidth;
    public virtual bool hasTightHeight => minHeight >= maxHeight;
    public override bool isTight => hasTightWidth && hasTightHeight;
    public virtual bool hasBoundedWidth => maxWidth < double.PositiveInfinity;
    public virtual bool hasBoundedHeight => maxHeight < double.PositiveInfinity;
    public virtual bool hasInfiniteWidth => minWidth >= double.PositiveInfinity;
    public virtual bool hasInfiniteHeight => minHeight >= double.PositiveInfinity;
    public virtual bool isSatisfiedBy(Size size)
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        return minWidth <= size.width && size.width <= maxWidth && minHeight <= size.height && size.height <= maxHeight;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BoxConstraints op_Multiply(double factor)
    {
        return new BoxConstraints(minWidth: minWidth * factor, maxWidth: maxWidth * factor, minHeight: minHeight * factor, maxHeight: maxHeight * factor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BoxConstraints op_Divide(double factor)
    {
        return new BoxConstraints(minWidth: minWidth / factor, maxWidth: maxWidth / factor, minHeight: minHeight / factor, maxHeight: maxHeight / factor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BoxConstraints ___(double factor)
    {
        return new BoxConstraints(minWidth: checked((long)(minWidth / factor)).toDouble(), maxWidth: checked((long)(maxWidth / factor)).toDouble(), minHeight: checked((long)(minHeight / factor)).toDouble(), maxHeight: checked((long)(maxHeight / factor)).toDouble());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BoxConstraints __(double value)
    {
        return new BoxConstraints(minWidth: minWidth % value, maxWidth: maxWidth % value, minHeight: minHeight % value, maxHeight: maxHeight % value);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static BoxConstraints? lerp(BoxConstraints? a, BoxConstraints? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if (a is null)
        {
            return b!.op_Multiply(t);
        }
        if (b is null)
        {
            return a.op_Multiply(1.0 - t);
        }
        DartRuntimePrimitives.Assert(() => a.debugAssertIsValid());
        DartRuntimePrimitives.Assert(() => b.debugAssertIsValid());
        DartRuntimePrimitives.Assert(() => double.IsFinite(a.minWidth) && double.IsFinite(b.minWidth) || (a.minWidth == double.PositiveInfinity) && (b.minWidth == double.PositiveInfinity));
        DartRuntimePrimitives.Assert(() => double.IsFinite(a.maxWidth) && double.IsFinite(b.maxWidth) || (a.maxWidth == double.PositiveInfinity) && (b.maxWidth == double.PositiveInfinity));
        DartRuntimePrimitives.Assert(() => double.IsFinite(a.minHeight) && double.IsFinite(b.minHeight) || (a.minHeight == double.PositiveInfinity) && (b.minHeight == double.PositiveInfinity));
        DartRuntimePrimitives.Assert(() => double.IsFinite(a.maxHeight) && double.IsFinite(b.maxHeight) || (a.maxHeight == double.PositiveInfinity) && (b.maxHeight == double.PositiveInfinity));
        return new BoxConstraints(minWidth: double.IsFinite(a.minWidth) ? DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(a.minWidth, b.minWidth, t)) : double.PositiveInfinity, maxWidth: double.IsFinite(a.maxWidth) ? DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(a.maxWidth, b.maxWidth, t)) : double.PositiveInfinity, minHeight: double.IsFinite(a.minHeight) ? DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(a.minHeight, b.minHeight, t)) : double.PositiveInfinity, maxHeight: double.IsFinite(a.maxHeight) ? DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(a.maxHeight, b.maxHeight, t)) : double.PositiveInfinity);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isNormalized
    {
        get
        {
            return (minWidth >= 0.0) && (minWidth <= maxWidth) && (minHeight >= 0.0) && (minHeight <= maxHeight);
        }
    }
    public override bool debugAssertIsValid(bool isAppliedConstraint = false, InformationCollector? informationCollector = null)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                void throwError(DiagnosticsNode message)
                {
                    throw new FlutterError(new List<DiagnosticsNode> { message, new DiagnosticsProperty<BoxConstraints>("The offending constraints were", this, style: DiagnosticsTreeStyle.errorProperty) });
                }
                if (double.IsNaN(minWidth) || double.IsNaN(maxWidth) || double.IsNaN(minHeight) || double.IsNaN(maxHeight))
                {
                    var affectedFieldsList = new List<string>();
                    DartRuntimePrimitives.Assert(() => checked((long)affectedFieldsList.Count) != 0);
                    if (checked(affectedFieldsList.Count) > 1L)
                    {
                        affectedFieldsList.Add($"and {affectedFieldsList.removeLast()}");
                    }
                    string whichFields = checked((long)affectedFieldsList.Count) switch { 1L => affectedFieldsList.Single(), 2L => string.Join(" ", affectedFieldsList), _ => string.Join(", ", affectedFieldsList) };
                    throwError(new ErrorSummary($"BoxConstraints has {((checked(affectedFieldsList.Count) == 1L) ? "a NaN value" : "NaN values")} in {whichFields}."));
                }
                if ((minWidth < 0.0) && (minHeight < 0.0))
                {
                    throwError(new ErrorSummary("BoxConstraints has both a negative minimum width and a negative minimum height."));
                }
                if (minWidth < 0.0)
                {
                    throwError(new ErrorSummary("BoxConstraints has a negative minimum width."));
                }
                if (minHeight < 0.0)
                {
                    throwError(new ErrorSummary("BoxConstraints has a negative minimum height."));
                }
                if ((maxWidth < minWidth) && (maxHeight < minHeight))
                {
                    throwError(new ErrorSummary("BoxConstraints has both width and height constraints non-normalized."));
                }
                if (maxWidth < minWidth)
                {
                    throwError(new ErrorSummary("BoxConstraints has non-normalized width constraints."));
                }
                if (maxHeight < minHeight)
                {
                    throwError(new ErrorSummary("BoxConstraints has non-normalized height constraints."));
                }
                if (isAppliedConstraint)
                {
                    if (double.IsInfinity(minWidth) && double.IsInfinity(minHeight))
                    {
                        throwError(new ErrorSummary("BoxConstraints forces an infinite width and infinite height."));
                    }
                    if (double.IsInfinity(minWidth))
                    {
                        throwError(new ErrorSummary("BoxConstraints forces an infinite width."));
                    }
                    if (double.IsInfinity(minHeight))
                    {
                        throwError(new ErrorSummary("BoxConstraints forces an infinite height."));
                    }
                }
                DartRuntimePrimitives.Assert(() => isNormalized);
                return true;
            });
        return isNormalized;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BoxConstraints normalize()
    {
        if (isNormalized)
        {
            return this;
        }
        double minWidthLocal = (minWidth >= 0.0) ? minWidth : 0.0;
        double minHeightLocal = (minHeight >= 0.0) ? minHeight : 0.0;
        return new BoxConstraints(minWidth: DartRuntimePrimitives.RequireValue(minWidthLocal), maxWidth: (DartRuntimePrimitives.RequireValue(minWidthLocal) > maxWidth) ? DartRuntimePrimitives.RequireValue(minWidthLocal) : maxWidth, minHeight: DartRuntimePrimitives.RequireValue(minHeightLocal), maxHeight: (DartRuntimePrimitives.RequireValue(minHeightLocal) > maxHeight) ? DartRuntimePrimitives.RequireValue(minHeightLocal) : maxHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as BoxConstraints;
        if (__other is null) return false;
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        DartRuntimePrimitives.Assert(() => (__other is BoxConstraints) && __other.debugAssertIsValid());
        return (__other is BoxConstraints) && (__other.minWidth == minWidth) && (__other.maxWidth == maxWidth) && (__other.minHeight == minHeight) && (__other.maxHeight == maxHeight);
    }

    public override int GetHashCode()
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        return FoundationRuntimePorts.ObjectHash(minWidth, maxWidth, minHeight, maxHeight);
    }
    public override string ToString()
    {
        var annotation = isNormalized ? "" : "; NOT NORMALIZED";
        if ((minWidth == double.PositiveInfinity) && (minHeight == double.PositiveInfinity))
        {
            return $"BoxConstraints(biggest{annotation})";
        }
        if ((minWidth == 0L) && (maxWidth == double.PositiveInfinity) && (minHeight == 0L) && (maxHeight == double.PositiveInfinity))
        {
            return $"BoxConstraints(unconstrained{annotation})";
        }
        string describe(double min, double max, string dim)
        {
            if (min == max)
            {
                return $"{dim}={min.toStringAsFixed(1L)}";
            }
            return $"{min.toStringAsFixed(1L)}<={dim}<={max.toStringAsFixed(1L)}";
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        string width = describe(DartRuntimePrimitives.RequireValue(minWidth), DartRuntimePrimitives.RequireValue(maxWidth), "w");
        string height = describe(DartRuntimePrimitives.RequireValue(minHeight), DartRuntimePrimitives.RequireValue(maxHeight), "h");
        return $"BoxConstraints({width}, {height}{annotation})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public delegate bool BoxHitTest(BoxHitTestResult result, Offset position);

public delegate bool BoxHitTestWithOutOfBandPosition(BoxHitTestResult result);

public class BoxHitTestResult : HitTestResult
{
    public BoxHitTestResult()
    {
    }

    private BoxHitTestResult(HitTestResult result) : base(result)
    {
    }

    public new static BoxHitTestResult CreateWrap(HitTestResult result)
    {
        return new BoxHitTestResult(result);
    }

    public virtual bool addWithPaintTransform(Matrix4? transform, Offset position, Func<BoxHitTestResult, Offset, bool> hitTest)
    {
        if (transform is not null)
        {
            transform = Matrix4.tryInvert(PointerEvent.removePerspectiveTransform(transform));
            if (transform is null)
            {
                return false;
            }
        }
        return addWithRawTransform(transform: transform, position: position, hitTest: hitTest);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool addWithPaintOffset(Offset? offset, Offset position, Func<BoxHitTestResult, Offset, bool> hitTest)
    {
        Offset transformedPosition = (offset is null) ? position : (position - DartRuntimePrimitives.RequireValue(offset));
        if (offset is not null)
        {
            Offset offset__value30995 = DartRuntimePrimitives.RequireValue(offset);
            pushOffset(-DartRuntimePrimitives.RequireValue(offset__value30995));
        }
        bool isHit = hitTest(this, transformedPosition);
        if (offset is not null)
        {
            Offset offset__value31113 = DartRuntimePrimitives.RequireValue(offset);
            popTransform();
        }
        return isHit;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool addWithRawTransform(Matrix4? transform, Offset position, Func<BoxHitTestResult, Offset, bool> hitTest)
    {
        Offset transformedPosition = (transform is null) ? position : MatrixUtils.transformPoint(transform, position);
        if (transform is not null)
        {
            pushTransform(transform);
        }
        bool isHit = hitTest(this, transformedPosition);
        if (transform is not null)
        {
            popTransform();
        }
        return isHit;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool addWithOutOfBandPosition(Offset? paintOffset = null, Matrix4? paintTransform = null, Matrix4? rawTransform = null, Func<BoxHitTestResult, bool> hitTest = default!)
    {
        DartRuntimePrimitives.Assert(() => (paintOffset is null) && (paintTransform is null) && (rawTransform is not null) || (paintOffset is null) && (paintTransform is not null) && (rawTransform is null) || (paintOffset is not null) && (paintTransform is null) && (rawTransform is null));
        if (paintOffset is not null)
        {
            Offset paintOffset__value34120 = DartRuntimePrimitives.RequireValue(paintOffset);
            pushOffset(-DartRuntimePrimitives.RequireValue(paintOffset__value34120));
        }
        else
        {
            if (rawTransform is not null)
            {
                pushTransform(rawTransform);
            }
            else
            {
                DartRuntimePrimitives.Assert(() => paintTransform is not null);
                paintTransform = Matrix4.tryInvert(PointerEvent.removePerspectiveTransform(paintTransform!));
                DartRuntimePrimitives.Assert(() => paintTransform is not null);
                pushTransform(paintTransform!);
            }
        }
        bool isHit = hitTest(this);
        popTransform();
        return isHit;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class BoxHitTestEntry : HitTestEntry<RenderBox>
{
    public virtual Offset localPosition { get; private set; } = default!;

    public BoxHitTestEntry(RenderBox target, Offset localPosition) : base(target)
    {
        this.localPosition = localPosition;
    }

    public override string ToString() => $"{DiagnosticsLibrary.describeIdentity(target)}@{localPosition}";
}

public class BoxParentData : ParentData
{
    public virtual Offset offset { get; set; } = Offset.zero;

    public override string ToString() => $"offset={offset}";
}

public abstract class ContainerBoxParentData<ChildType> : BoxParentData, ContainerParentDataMixin<ChildType> where ChildType : RenderObject
{
    public virtual ChildType? previousSibling { get; set; } = default;
    public virtual ChildType? nextSibling { get; set; } = default;

    public override void detach()
    {
        DartRuntimePrimitives.Assert(() => previousSibling is null);
        DartRuntimePrimitives.Assert(() => nextSibling is null);
        base.detach();
    }

}

public class BaselineOffset
{
    public double? offset { get; }

    public BaselineOffset(double? offset)
    {
        this.offset = offset;
    }

    public static implicit operator double?(BaselineOffset value) => value.offset;
    public static implicit operator BaselineOffset(double? value) => new BaselineOffset(value);

    public static BaselineOffset noBaseline = new BaselineOffset(null);

    public virtual BaselineOffset op_Add(double offset)
    {
        double? value = this.offset;
        return new BaselineOffset((value is null) ? null : (DartRuntimePrimitives.RequireValue(value) + offset));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BaselineOffset minOf(BaselineOffset other)
    {
        return (offset, other.offset) switch { (double lhs, double rhs) => (lhs >= rhs) ? other : this, (double lhsLocal, null) => new BaselineOffset(lhsLocal), (null, var rhsLocal) => rhsLocal };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal interface _CachedLayoutCalculation__box<Input, Output>
{
    public static _DryLayout__box dryLayout = new _DryLayout__box();
    public static _Baseline__box baseline = new _Baseline__box();

    public Output memoize(_LayoutCacheStorage__box cacheStorage, Input input, Func<Input, Output> computer);
    public DartMap<string, string> debugFillTimelineArguments(DartMap<string, string> timelineArguments, Input input);
    public string eventLabel(RenderBox renderBox);
}

public class _DryLayout__box : _CachedLayoutCalculation__box<BoxConstraints, Size>
{
    internal _DryLayout__box()
    {
    }

    public virtual Size memoize(_LayoutCacheStorage__box cacheStorage, BoxConstraints input, Func<BoxConstraints, Size> computer)
    {
        using var profile = FrameworkComponentProfile.Begin(FrameworkComponentProfile.Kind.DryCache);
        return (cacheStorage._cachedDryLayoutSizes ??= new DartMap<BoxConstraints, Size>()).putIfAbsent(input, () => computer(input));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DartMap<string, string> debugFillTimelineArguments(DartMap<string, string> timelineArguments, BoxConstraints input)
    {
        return ((Func<DartMap<string, string>>)(() =>
{
    var __cascade = timelineArguments;
    __cascade["getDryLayout constraints"] = $"{input}";
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string eventLabel(RenderBox renderBox) => $"{DartRuntimePrimitives.RuntimeType(renderBox)}.getDryLayout";
}

public class _Baseline__box : _CachedLayoutCalculation__box<(BoxConstraints, TextBaseline), BaselineOffset>
{
    internal _Baseline__box()
    {
    }

    public virtual BaselineOffset memoize(_LayoutCacheStorage__box cacheStorage, (BoxConstraints, TextBaseline) input, Func<(BoxConstraints, TextBaseline), BaselineOffset> computer)
    {
        using var profile = FrameworkComponentProfile.Begin(FrameworkComponentProfile.Kind.BaselineCache);
        DartMap<BoxConstraints, BaselineOffset> cache = input.Item2 switch { TextBaseline.alphabetic => cacheStorage._cachedAlphabeticBaseline ??= new DartMap<BoxConstraints, BaselineOffset>(), TextBaseline.ideographic => cacheStorage._cachedIdeoBaseline ??= new DartMap<BoxConstraints, BaselineOffset>(), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        BaselineOffset ifAbsent()
        {
            return computer(input);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        return cache.putIfAbsent(input.Item1, ifAbsent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DartMap<string, string> debugFillTimelineArguments(DartMap<string, string> timelineArguments, (BoxConstraints, TextBaseline) input)
    {
        return ((Func<DartMap<string, string>>)(() =>
{
    var __cascade = timelineArguments;
    __cascade["baseline type"] = $"{input.Item2}";
    __cascade["constraints"] = $"{input.Item1}";
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string eventLabel(RenderBox renderBox) => $"{DartRuntimePrimitives.RuntimeType(renderBox)}.getDryBaseline";
}

internal enum _IntrinsicDimension__box
{
    minWidth,
    maxWidth,
    minHeight,
    maxHeight
}

internal static class _IntrinsicDimension__boxMembers
{
    public static double memoize(this _IntrinsicDimension__box value, _LayoutCacheStorage__box cacheStorage, double input, Func<double, double> computer)
    {
        using var profile = FrameworkComponentProfile.Begin(FrameworkComponentProfile.Kind.IntrinsicCache);
        return (cacheStorage._cachedIntrinsicDimensions ??= new DartMap<(_IntrinsicDimension__box, double), double>()).putIfAbsent((value, input), () => computer(input));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
    public static DartMap<string, string> debugFillTimelineArguments(this _IntrinsicDimension__box value, DartMap<string, string> timelineArguments, double input)
    {
        return ((Func<DartMap<string, string>>)(() =>
{
    var __cascade = timelineArguments;
    __cascade["intrinsics dimension"] = value.ToString();
    __cascade["intrinsics argument"] = $"{input}";
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
    public static string eventLabel(this _IntrinsicDimension__box value, RenderBox renderBox) => $"{DartRuntimePrimitives.RuntimeType(renderBox)} intrinsics";
}

internal sealed class _IntrinsicDimension__boxInterfaceAdapter : _CachedLayoutCalculation__box<double, double>
{
    private readonly _IntrinsicDimension__box _value;
    public _IntrinsicDimension__boxInterfaceAdapter(_IntrinsicDimension__box value) => _value = value;
    public double memoize(_LayoutCacheStorage__box cacheStorage, double input, Func<double, double> computer) => _value.memoize(cacheStorage, input, computer);
    public DartMap<string, string> debugFillTimelineArguments(DartMap<string, string> timelineArguments, double input) => _value.debugFillTimelineArguments(timelineArguments, input);
    public string eventLabel(RenderBox renderBox) => _value.eventLabel(renderBox);
}

public class _LayoutCacheStorage__box
{
    internal virtual DartMap<(_IntrinsicDimension__box, double), double>? _cachedIntrinsicDimensions { get; set; } = default;
    internal virtual DartMap<BoxConstraints, Size>? _cachedDryLayoutSizes { get; set; } = default;
    internal virtual DartMap<BoxConstraints, BaselineOffset>? _cachedAlphabeticBaseline { get; set; } = default;
    internal virtual DartMap<BoxConstraints, BaselineOffset>? _cachedIdeoBaseline { get; set; } = default;

    public virtual bool clear()
    {
        bool hasCache = ((((long?)(_cachedDryLayoutSizes?.Count)) is { } __count42271 ? __count42271 != 0 : (bool?)null) ?? false) || ((((long?)(_cachedIntrinsicDimensions?.Count)) is { } __count42327 ? __count42327 != 0 : (bool?)null) ?? false) || ((((long?)(_cachedAlphabeticBaseline?.Count)) is { } __count42388 ? __count42388 != 0 : (bool?)null) ?? false) || ((((long?)(_cachedIdeoBaseline?.Count)) is { } __count42448 ? __count42448 != 0 : (bool?)null) ?? false);
        if (hasCache)
        {
            _cachedDryLayoutSizes?.Clear();
            _cachedIntrinsicDimensions?.Clear();
            _cachedAlphabeticBaseline?.Clear();
            _cachedIdeoBaseline?.Clear();
        }
        return hasCache;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class RenderBox : RenderObject
{
    internal virtual _LayoutCacheStorage__box _layoutCacheStorage { get; private set; } = new _LayoutCacheStorage__box();
    internal static long _debugIntrinsicsDepth = 0L;
    internal virtual bool _computingThisDryLayout { get; set; } = false;
    internal virtual bool _computingThisDryBaseline { get; set; } = false;
    internal static bool _debugDryLayoutCalculationValid = true;
    internal virtual Size? _size { get; set; } = default;
    internal static bool _debugDoingBaseline = false;
    internal virtual long _debugActivePointers { get; set; } = 0L;

    public override void setupParentData(RenderObject child)
    {
        if (child.parentData is not BoxParentData)
        {
            child.parentData = new BoxParentData();
        }
    }

    internal virtual Output _computeIntrinsics<Input, Output>(_CachedLayoutCalculation__box<Input, Output> type, Input input, Func<Input, Output> computer)
    {
        DartRuntimePrimitives.Assert(() => debugCheckingIntrinsics || !debugDoingThisResize);
        var shouldCache = true;
        DartRuntimePrimitives.Assert(() =>
            {
                shouldCache = !debugCheckingIntrinsics;
                return true;
            });
        return shouldCache ? _computeWithTimeline(type, input, computer) : computer(input);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Output _computeWithTimeline<Input, Output>(_CachedLayoutCalculation__box<Input, Output> type, Input input, Func<Input, Output> computer)
    {
        DartMap<string, string>? debugTimelineArguments = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                DartMap<string, string> argumentsLocal = DebugLibrary.debugEnhanceLayoutTimelineArguments ? toDiagnosticsNode().toTimelineArguments()! : new DartMap<string, string>();
                debugTimelineArguments = type.debugFillTimelineArguments(argumentsLocal, input);
                return true;
            });
        if (!Foundation.ConstantsLibrary.kReleaseMode)
        {
            if (DebugLibrary.debugProfileLayoutsEnabled || (_debugIntrinsicsDepth == 0L))
            {
                FlutterTimeline.startSync(type.eventLabel(this), arguments: debugTimelineArguments);
            }
            _debugIntrinsicsDepth += 1L;
        }
        Output result = type.memoize(_layoutCacheStorage, input, computer);
        if (!Foundation.ConstantsLibrary.kReleaseMode)
        {
            _debugIntrinsicsDepth -= 1L;
            if (DebugLibrary.debugProfileLayoutsEnabled || (_debugIntrinsicsDepth == 0L))
            {
                FlutterTimeline.finishSync();
            }
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double getMinIntrinsicWidth(double height)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (height < 0.0)
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("The height argument to getMinIntrinsicWidth was negative."), new ErrorDescription("The argument to getMinIntrinsicWidth must not be negative or null."), new ErrorHint("If you perform computations on another height before passing it to " + "getMinIntrinsicWidth, consider using math.max() or double.clamp() " + "to force the value into the valid range.") });
                }
                return true;
            });
        return _computeIntrinsics(new _IntrinsicDimension__boxInterfaceAdapter(_IntrinsicDimension__box.minWidth), height, computeMinIntrinsicWidth);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double computeMinIntrinsicWidth(double height)
    {
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double getMaxIntrinsicWidth(double height)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (height < 0.0)
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("The height argument to getMaxIntrinsicWidth was negative."), new ErrorDescription("The argument to getMaxIntrinsicWidth must not be negative or null."), new ErrorHint("If you perform computations on another height before passing it to " + "getMaxIntrinsicWidth, consider using math.max() or double.clamp() " + "to force the value into the valid range.") });
                }
                return true;
            });
        return _computeIntrinsics(new _IntrinsicDimension__boxInterfaceAdapter(_IntrinsicDimension__box.maxWidth), height, computeMaxIntrinsicWidth);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double computeMaxIntrinsicWidth(double height)
    {
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double getMinIntrinsicHeight(double width)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (width < 0.0)
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("The width argument to getMinIntrinsicHeight was negative."), new ErrorDescription("The argument to getMinIntrinsicHeight must not be negative or null."), new ErrorHint("If you perform computations on another width before passing it to " + "getMinIntrinsicHeight, consider using math.max() or double.clamp() " + "to force the value into the valid range.") });
                }
                return true;
            });
        return _computeIntrinsics(new _IntrinsicDimension__boxInterfaceAdapter(_IntrinsicDimension__box.minHeight), width, computeMinIntrinsicHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double computeMinIntrinsicHeight(double width)
    {
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double getMaxIntrinsicHeight(double width)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (width < 0.0)
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("The width argument to getMaxIntrinsicHeight was negative."), new ErrorDescription("The argument to getMaxIntrinsicHeight must not be negative or null."), new ErrorHint("If you perform computations on another width before passing it to " + "getMaxIntrinsicHeight, consider using math.max() or double.clamp() " + "to force the value into the valid range.") });
                }
                return true;
            });
        return _computeIntrinsics(new _IntrinsicDimension__boxInterfaceAdapter(_IntrinsicDimension__box.maxHeight), width, computeMaxIntrinsicHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double computeMaxIntrinsicHeight(double width)
    {
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Size getDryLayout(BoxConstraints constraints)
    {
        return _computeIntrinsics(_CachedLayoutCalculation__box<object, object>.dryLayout, constraints, (BoxConstraints __constraints) => _computeDryLayout(__constraints));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Size _computeDryLayout(BoxConstraints constraints, Func<RenderBox, BoxConstraints, Size> layoutChild = default!)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                DartRuntimePrimitives.Assert(() => !_computingThisDryLayout);
                _computingThisDryLayout = true;
                return true;
            });
        Size result = computeDryLayout(constraints);
        DartRuntimePrimitives.Assert(() =>
            {
                DartRuntimePrimitives.Assert(() => _computingThisDryLayout);
                _computingThisDryLayout = false;
                return true;
            });
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Size computeDryLayout(BoxConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() => debugCannotComputeDryLayout(error: new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"The {objectRuntimeTypeFunctions.objectRuntimeType(this, "RenderBox")} class does not implement \"computeDryLayout\"."), new ErrorHint("If you are not writing your own RenderBox subclass, then this is not\n" + "your fault. Contact support: https://github.com/flutter/flutter/issues/new?template=02_bug.yml") })));
        return Size.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? getDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        double? baselineOffset = _computeIntrinsics(_CachedLayoutCalculation__box<object, object>.baseline, (constraints, baseline), _computeDryBaseline).offset;
        DartRuntimePrimitives.Assert(() => debugCheckingIntrinsics || (baselineOffset == computeDryBaseline(constraints, baseline)));
        return baselineOffset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual BaselineOffset _computeDryBaseline((BoxConstraints, TextBaseline) pair)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                DartRuntimePrimitives.Assert(() => !_computingThisDryBaseline);
                _computingThisDryBaseline = true;
                return true;
            });
        var result = new BaselineOffset(computeDryBaseline(pair.Item1, pair.Item2));
        DartRuntimePrimitives.Assert(() =>
            {
                DartRuntimePrimitives.Assert(() => _computingThisDryBaseline);
                _computingThisDryBaseline = false;
                return true;
            });
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => debugCannotComputeDryLayout(error: new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"The {objectRuntimeTypeFunctions.objectRuntimeType(this, "RenderBox")} class does not implement \"computeDryBaseline\"."), new ErrorHint("If you are not writing your own RenderBox subclass, then this is not\n" + "your fault. Contact support: https://github.com/flutter/flutter/issues/new?template=02_bug.yml") })));
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool debugCannotComputeDryLayout(string? reason = null, FlutterError? error = null)
    {
        DartRuntimePrimitives.Assert(() => reason is null != error is null);
        DartRuntimePrimitives.Assert(() =>
            {
                if (!debugCheckingIntrinsics)
                {
                    if (reason is not null)
                    {
                        DartRuntimePrimitives.Assert(() => error is null);
                        throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"The {objectRuntimeTypeFunctions.objectRuntimeType(this, "RenderBox")} class does not support dry layout.") });
                    }
                    DartRuntimePrimitives.Assert(() => error is not null);
                    throw error!;
                }
                _debugDryLayoutCalculationValid = false;
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool hasSize => _size is not null;
    public virtual Size size
    {
        get
        {
            DartRuntimePrimitives.Assert(() => hasSize);
            DartRuntimePrimitives.Assert(() =>
                {
                    Size? size = _size;
                    if (size is _DebugSize__box)
                    {
                        _DebugSize__box size__93552__as93576 = (_DebugSize__box)size;
                        DartRuntimePrimitives.Assert(() => Equals(size__93552__as93576._owner, this));
                        RenderObject? parentLocal = parent;
                        bool doingRegularLayout = !(debugActiveLayout?.debugDoingThisLayoutWithCallback ?? true);
                        bool sizeAccessAllowed = !doingRegularLayout || debugDoingThisResize || debugDoingThisLayout || _debugDoingBaseline || (Equals(debugActiveLayout, parentLocal) && size__93552__as93576._canBeUsedByParent);
                        DartRuntimePrimitives.Assert(() => sizeAccessAllowed);
                        RenderBox? renderBoxDoingDryLayout = _computingThisDryLayout ? this : (((parentLocal is RenderBox) && ((RenderBox)parentLocal)._computingThisDryLayout) ? ((RenderBox)parentLocal) : null);
                        DartRuntimePrimitives.Assert(() => renderBoxDoingDryLayout is null);
                        RenderBox? renderBoxDoingDryBaseline = _computingThisDryBaseline ? this : (((parentLocal is RenderBox) && ((RenderBox)parentLocal)._computingThisDryBaseline) ? ((RenderBox)parentLocal) : null);
                        DartRuntimePrimitives.Assert(() => renderBoxDoingDryBaseline is null);
                        DartRuntimePrimitives.Assert(() => Equals(size__93552__as93576, _size));
                    }
                    return true;
                });
            return _size ?? throw new InvalidOperationException($"RenderBox was not laid out: {GetType()}#{DiagnosticsLibrary.shortHash(this)}");
        }
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => !(debugDoingThisResize && debugDoingThisLayout));
            DartRuntimePrimitives.Assert(() => sizedByParent || !debugDoingThisResize);
            DartRuntimePrimitives.Assert(() =>
                {
                    if (sizedByParent && debugDoingThisResize || !sizedByParent && debugDoingThisLayout)
                    {
                        return true;
                    }
                    DartRuntimePrimitives.Assert(() => !debugDoingThisResize);
                    var information = new List<DiagnosticsNode> { new ErrorSummary("RenderBox size setter called incorrectly.") };
                    if (debugDoingThisLayout)
                    {
                        DartRuntimePrimitives.Assert(() => sizedByParent);
                        information.Add(new ErrorDescription("It appears that the size setter was called from performLayout()."));
                    }
                    else
                    {
                        information.Add(new ErrorDescription("The size setter was called from outside layout (neither performResize() nor performLayout() were being run for this object)."));
                        if ((owner is not null) && owner!.debugDoingLayout)
                        {
                            information.Add(new ErrorDescription("Only the object itself can set its size. It is a contract violation for other objects to set it."));
                        }
                    }
                    if (sizedByParent)
                    {
                        information.Add(new ErrorDescription("Because this RenderBox has sizedByParent set to true, it must set its size in performResize()."));
                    }
                    else
                    {
                        information.Add(new ErrorDescription("Because this RenderBox has sizedByParent set to false, it must set its size in performLayout()."));
                    }
                    throw new FlutterError(information);
                });
            DartRuntimePrimitives.Assert(() =>
                {
                    __value = debugAdoptSize(__value);
                    return true;
                });
            _size = __value;
            DartRuntimePrimitives.Assert(() =>
                {
                    debugAssertDoesMeetConstraints();
                    return true;
                });
        }
    }
    public virtual Size debugAdoptSize(Size value)
    {
        var result = value;
        DartRuntimePrimitives.Assert(() =>
            {
                if (value is _DebugSize__box)
                {
                    _DebugSize__box value__as99138 = (_DebugSize__box)value;
                    if (!Equals(value__as99138._owner, this))
                    {
                        if (!Equals(value__as99138._owner.parent, this))
                        {
                            throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("The size property was assigned a size inappropriately."), describeForError("The following render object"), value__as99138._owner.describeForError("...was assigned a size obtained from"), new ErrorDescription("However, this second render object is not, or is no longer, a " + "child of the first, and it is therefore a violation of the " + "RenderBox layout protocol to use that size in the layout of the " + "first render object."), new ErrorHint("If the size was obtained at a time where it was valid to read " + "the size (because the second render object above was a child " + "of the first at the time), then it should be adopted using " + "debugAdoptSize at that time."), new ErrorHint("If the size comes from a grandchild or a render object from an " + "entirely different part of the render tree, then there is no " + "way to be notified when the size changes and therefore attempts " + "to read that size are almost certainly a source of bugs. A different " + "approach should be used.") });
                        }
                        if (!value__as99138._canBeUsedByParent)
                        {
                            throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("A child's size was used without setting parentUsesSize."), describeForError("The following render object"), value__as99138._owner.describeForError("...was assigned a size obtained from its child"), new ErrorDescription("However, when the child was laid out, the parentUsesSize argument " + "was not set or set to false. Subsequently this transpired to be " + "inaccurate: the size was nonetheless used by the parent.\n" + "It is important to tell the framework if the size will be used or not " + "as several important performance optimizations can be made if the " + "size will not be used by the parent.") });
                        }
                    }
                }
                result = new _DebugSize__box(value, this, debugCanParentUseSize);
                return true;
            });
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Rect semanticBounds => Offset.zero & size;
    public override void debugResetSize()
    {
        size = size;
    }

    internal static bool _debugSetDoingBaseline(bool value)
    {
        _debugDoingBaseline = value;
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? getDistanceToBaseline(TextBaseline baseline, bool onlyReal = false)
    {
        DartRuntimePrimitives.Assert(() => !_debugDoingBaseline);
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout || debugCheckingIntrinsics);
        DartRuntimePrimitives.Assert(() => debugCheckingIntrinsics || (owner! switch { PipelineOwner { debugDoingLayout: true } __object103560 => Equals(debugActiveLayout, parent) && parent!.debugDoingThisLayout, PipelineOwner { debugDoingPaint: true } __object103701 => (Equals(debugActivePaint, parent) && parent!.debugDoingThisPaint) || Equals(debugActivePaint, this) && debugDoingThisPaint, PipelineOwner __object103923 => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        DartRuntimePrimitives.Assert(() => _debugSetDoingBaseline(true));
        double? result = default!;
        try
        {
            result = getDistanceToActualBaseline(baseline);
        }
        finally
        {
            DartRuntimePrimitives.Assert(() => _debugSetDoingBaseline(false));
        }
        if ((result is null) && !onlyReal)
        {
            return size.height;
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? getDistanceToActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => _debugDoingBaseline);
        return _computeIntrinsics(_CachedLayoutCalculation__box<object, object>.baseline, (constraints, baseline), (pair) => new BaselineOffset(computeDistanceToActualBaseline(pair.Item2))).offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => _debugDoingBaseline);
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BoxConstraints constraints => ((BoxConstraints?)(object?)base.constraints)!;
    public override void debugAssertDoesMeetConstraints()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (!hasSize)
                {
                    DiagnosticsNode contract = default!;
                    if (sizedByParent)
                    {
                        contract = new ErrorDescription("Because this RenderBox has sizedByParent set to true, it must set its size in performResize().");
                    }
                    else
                    {
                        contract = new ErrorDescription("Because this RenderBox has sizedByParent set to false, it must set its size in performLayout().");
                    }
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("RenderBox did not set its size during layout."), contract, new ErrorDescription("It appears that this did not happen; layout completed, but the size property is still null."), new DiagnosticsProperty<RenderBox>("The RenderBox in question is", this, style: DiagnosticsTreeStyle.errorProperty) });
                }
                if (!DartRuntimePrimitives.RequireValue(_size).isFinite)
                {
                    var information = new List<DiagnosticsNode> { new ErrorSummary($"{GetType()} object was given an infinite size during layout."), new ErrorDescription("This probably means that it is a render object that tries to be " + "as big as possible, but it was put inside another render object " + "that allows its children to pick their own size.") };
                    if (!constraints.hasBoundedWidth)
                    {
                        var node = this;
                        while (!node.constraints.hasBoundedWidth && (node.parent is RenderBox))
                        {
                            node = ((RenderBox?)(object?)node.parent!)!;
                        }
                        information.Add(node.describeForError("The nearest ancestor providing an unbounded width constraint is"));
                    }
                    if (!constraints.hasBoundedHeight)
                    {
                        var nodeLocal = this;
                        while (!nodeLocal.constraints.hasBoundedHeight && (nodeLocal.parent is RenderBox))
                        {
                            nodeLocal = ((RenderBox?)(object?)nodeLocal.parent!)!;
                        }
                        information.Add(nodeLocal.describeForError("The nearest ancestor providing an unbounded height constraint is"));
                    }
                    throw new FlutterError(new List<DiagnosticsNode> { new DiagnosticsProperty<BoxConstraints>($"The constraints that applied to the {GetType()} were", constraints, style: DiagnosticsTreeStyle.errorProperty), new DiagnosticsProperty<Size>("The exact size it was given was", _size, style: DiagnosticsTreeStyle.errorProperty), new ErrorHint("See https://flutter.dev/to/unbounded-constraints for more information.") });
                }
                if (!constraints.isSatisfiedBy(DartRuntimePrimitives.RequireValue(_size)))
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{GetType()} does not meet its constraints."), new DiagnosticsProperty<BoxConstraints>("Constraints", constraints, style: DiagnosticsTreeStyle.errorProperty), new DiagnosticsProperty<Size>("Size", _size, style: DiagnosticsTreeStyle.errorProperty), new ErrorHint("If you are not writing your own RenderBox subclass, then this is not " + "your fault. Contact support: https://github.com/flutter/flutter/issues/new?template=02_bug.yml") });
                }
                if (DebugLibrary.debugCheckIntrinsicSizes)
                {
                    DartRuntimePrimitives.Assert(() => !debugCheckingIntrinsics);
                    debugCheckingIntrinsics = true;
                    var failures = new List<DiagnosticsNode>();
                    double testIntrinsic(Func<double, double> function, string name, double constraint)
                    {
                        double result = function(constraint);
                        if (result < 0L)
                        {
                            failures.Add(new ErrorDescription($" * {name}({constraint}) returned a negative value: {result}"));
                        }
                        if (!double.IsFinite(result))
                        {
                            failures.Add(new ErrorDescription($" * {name}({constraint}) returned a non-finite value: {result}"));
                        }
                        return result;
                        throw new InvalidOperationException("Dart control flow completed without a value.");
                    }
                    void testIntrinsicsForValues(Func<double, double> getMin, Func<double, double> getMax, string name, double constraint)
                    {
                        double min = testIntrinsic(getMin, $"getMinIntrinsic{name}", constraint);
                        double max = testIntrinsic(getMax, $"getMaxIntrinsic{name}", constraint);
                        if (min > max)
                        {
                            failures.Add(new ErrorDescription($" * getMinIntrinsic{name}({constraint}) returned a larger value ({min}) than getMaxIntrinsic{name}({constraint}) ({max})"));
                        }
                    }
                    try
                    {
                        testIntrinsicsForValues(getMinIntrinsicWidth, getMaxIntrinsicWidth, "Width", double.PositiveInfinity);
                        testIntrinsicsForValues(getMinIntrinsicHeight, getMaxIntrinsicHeight, "Height", double.PositiveInfinity);
                        if (constraints.hasBoundedWidth)
                        {
                            testIntrinsicsForValues(getMinIntrinsicWidth, getMaxIntrinsicWidth, "Width", constraints.maxHeight);
                        }
                        if (constraints.hasBoundedHeight)
                        {
                            testIntrinsicsForValues(getMinIntrinsicHeight, getMaxIntrinsicHeight, "Height", constraints.maxWidth);
                        }
                    }
                    finally
                    {
                        debugCheckingIntrinsics = false;
                    }
                    if (checked((long)failures.Count) != 0)
                    {
                        throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"The intrinsic dimension methods of the {GetType()} class returned values that violate the intrinsic protocol contract."), new ErrorDescription($"The following {((checked(failures.Count) > 1L) ? "failures" : "failure")} was detected:"), new ErrorHint("If you are not writing your own RenderBox subclass, then this is not\n" + "your fault. Contact support: https://github.com/flutter/flutter/issues/new?template=02_bug.yml") });
                    }
                    _debugDryLayoutCalculationValid = true;
                    debugCheckingIntrinsics = true;
                    Size dryLayoutSize = default!;
                    try
                    {
                        dryLayoutSize = getDryLayout(constraints);
                    }
                    finally
                    {
                        debugCheckingIntrinsics = false;
                    }
                    if (_debugDryLayoutCalculationValid && (!Equals(dryLayoutSize, _size)))
                    {
                        throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"The size given to the {objectRuntimeTypeFunctions.objectRuntimeType(this, "RenderBox")} class differs from the size computed by computeDryLayout."), new ErrorDescription($"The size computed in {(sizedByParent ? "performResize" : "performLayout")} " + $"is {size}, which is different from {dryLayoutSize}, which was computed by computeDryLayout."), new ErrorDescription($"The constraints used were {constraints}."), new ErrorHint("If you are not writing your own RenderBox subclass, then this is not\n" + "your fault. Contact support: https://github.com/flutter/flutter/issues/new?template=02_bug.yml") });
                    }
                }
                return true;
            });
    }

    internal virtual void _debugVerifyDryBaselines()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                var messages = new List<DiagnosticsNode> { new ErrorDescription($"The constraints used were {constraints}."), new ErrorHint("If you are not writing your own RenderBox subclass, then this is not\n" + "your fault. Contact support: https://github.com/flutter/flutter/issues/new?template=02_bug.yml") };
                foreach (TextBaseline baseline in Enum.GetValues<TextBaseline>().ToList())
                {
                    DartRuntimePrimitives.Assert(() => !debugCheckingIntrinsics);
                    debugCheckingIntrinsics = true;
                    _debugDryLayoutCalculationValid = true;
                    double? dryBaseline = default!;
                    double? realBaseline = default!;
                    try
                    {
                        dryBaseline = getDryBaseline(constraints, baseline);
                        realBaseline = getDistanceToBaseline(baseline, onlyReal: true);
                    }
                    finally
                    {
                        debugCheckingIntrinsics = false;
                    }
                    DartRuntimePrimitives.Assert(() => !debugCheckingIntrinsics);
                    if (!_debugDryLayoutCalculationValid || (dryBaseline == realBaseline))
                    {
                        continue;
                    }
                    if (dryBaseline is null != realBaseline is null)
                    {
                        var (methodReturnedNull, methodReturnedNonNull) = (dryBaseline is null) ? (((string, string))("computeDryBaseline", "computeDistanceToActualBaseline")) : (((string, string))("computeDistanceToActualBaseline", "computeDryBaseline"));
                        throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"The {baseline} location returned by {objectRuntimeTypeFunctions.objectRuntimeType(this, "RenderBox")}.computeDistanceToActualBaseline " + "differs from the baseline location computed by computeDryBaseline."), new ErrorDescription($"The {methodReturnedNull} method returned null while the {methodReturnedNonNull} returned a non-null {baseline} of {dryBaseline ?? realBaseline}. " + $"Did you forget to implement {methodReturnedNull} for {objectRuntimeTypeFunctions.objectRuntimeType(this, "RenderBox")}?") });
                    }
                    else
                    {
                        throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"The {baseline} location returned by {objectRuntimeTypeFunctions.objectRuntimeType(this, "RenderBox")}.computeDistanceToActualBaseline " + "differs from the baseline location computed by computeDryBaseline."), new DiagnosticsProperty<RenderObject>("The RenderBox was", this), new ErrorDescription($"The computeDryBaseline method returned {dryBaseline},\n" + $"while the computeDistanceToActualBaseline method returned {realBaseline}.\n" + $"Consider checking the implementations of the following methods on the {objectRuntimeTypeFunctions.objectRuntimeType(this, "RenderBox")} class and make sure they are consistent:\n" + " * computeDistanceToActualBaseline\n" + " * computeDryBaseline\n" + " * performLayout\n") });
                    }
                }
                return true;
            });
    }

    public override void markNeedsLayout()
    {
        if (_layoutCacheStorage.clear() && (parent is not null))
        {
            markParentNeedsLayout();
            return;
        }
        base.markNeedsLayout();
    }

    public override void performResize()
    {
        size = computeDryLayout(constraints);
        DartRuntimePrimitives.Assert(() => size.isFinite);
    }

    public override void performLayout()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (!sizedByParent)
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{GetType()} did not implement performLayout()."), new ErrorHint("RenderBox subclasses need to either override performLayout() to " + "set a size and lay out any children, or, set sizedByParent to true " + "so that performResize() sizes the render object.") });
                }
                return true;
            });
    }

    public virtual bool hitTest(BoxHitTestResult result, Offset position)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (!hasSize)
                {
                    if (debugNeedsLayout)
                    {
                        throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Cannot hit test a render box that has never been laid out."), describeForError("The hitTest() method was called on this RenderBox"), new ErrorDescription("Unfortunately, this object's geometry is not known at this time, " + "probably because it has never been laid out. " + "This means it cannot be accurately hit-tested."), new ErrorHint("If you are trying " + "to perform a hit test during the layout phase itself, make sure " + "you only hit test nodes that have completed layout (e.g. the node's " + "children, after their layout() method has been called).") });
                    }
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Cannot hit test a render box with no size."), describeForError("The hitTest() method was called on this RenderBox"), new ErrorDescription("Although this node is not marked as needing layout, " + "its size is not set."), new ErrorHint("A RenderBox object must have an " + "explicit size before it can be hit-tested. Make sure " + "that the RenderBox in question sets its size during layout.") });
                }
                return true;
            });
        if (DartRuntimePrimitives.RequireValue(_size).contains(position))
        {
            if (hitTestChildren(result, position: position) || hitTestSelf(position))
            {
                result.add(new BoxHitTestEntry(this, position));
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool hitTestSelf(Offset position) => false;
    public virtual bool hitTestChildren(BoxHitTestResult result, Offset position) => false;
    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        DartRuntimePrimitives.Assert(() =>
            {
                if (child.parentData is not BoxParentData)
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{GetType()} does not implement applyPaintTransform."), describeForError($"The following {GetType()} object"), child.describeForError("...did not use a BoxParentData class for the parentData field of the following child"), new ErrorDescription($"The {GetType()} class inherits from RenderBox."), new ErrorHint("The default applyPaintTransform implementation provided by RenderBox assumes that the " + "children all use BoxParentData objects for their parentData field. " + $"Since {GetType()} does not in fact use that ParentData class for its children, it must " + "provide an implementation of applyPaintTransform that supports the specific ParentData " + $"subclass used by its children (which apparently is {DartRuntimePrimitives.RuntimeType(child.parentData)}).") });
                }
                return true;
            });
        var childParentData = ((BoxParentData?)(object?)child.parentData!)!;
        Offset offsetLocal = childParentData.offset;
        transform.translateByDouble(offsetLocal.dx, offsetLocal.dy, 0, 1);
    }

    public virtual Offset globalToLocal(Offset point, RenderObject? ancestor = null)
    {
        Matrix4 transform = getTransformTo(ancestor);
        double det = transform.invert();
        if (det == 0.0)
        {
            return Offset.zero;
        }
        Vector3 localScreenOrigin = transform.perspectiveTransform(new Vector3(0.0, 0.0, 0.0));
        Vector3 localViewDirection = transform.perspectiveTransform(new Vector3(0.0, 0.0, 1.0)) - localScreenOrigin;
        if (localViewDirection.z == 0.0)
        {
            return Offset.zero;
        }
        Vector3 localScreenPoint = transform.perspectiveTransform(new Vector3(point.dx, point.dy, 0.0));
        Vector3 localPoint = localScreenPoint - (localViewDirection * (localScreenPoint.z / localViewDirection.z));
        return new Offset(localPoint.x, localPoint.y);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Offset localToGlobal(Offset point, RenderObject? ancestor = null)
    {
        return MatrixUtils.transformPoint(getTransformTo(ancestor), point);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Rect paintBounds => Offset.zero & size;
    public override void handleEvent(PointerEvent @event, HitTestEntry<HitTestTarget> entry)
    {
        base.handleEvent(@event, entry);
    }

    public virtual bool debugHandleEvent(PointerEvent @event, HitTestEntry<HitTestTarget> entry)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (DebugLibrary.debugPaintPointersEnabled)
                {
                    if (@event is Gestures.PointerDownEvent)
                    {
                        Gestures.PointerDownEvent @event__as133711 = (Gestures.PointerDownEvent)@event;
                        _debugActivePointers += 1L;
                    }
                    else
                    {
                        if ((@event is Gestures.PointerUpEvent) || (@event is Gestures.PointerCancelEvent))
                        {
                            _debugActivePointers -= 1L;
                        }
                    }
                    markNeedsPaint();
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugPaint(PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (DebugLibrary.debugCheckIntrinsicSizes)
                {
                    _debugVerifyDryBaselines();
                }
                if (DebugLibrary.debugPaintSizeEnabled)
                {
                    debugPaintSize(context, offset);
                }
                if (DebugLibrary.debugPaintBaselinesEnabled)
                {
                    debugPaintBaselines(context, offset);
                }
                if (DebugLibrary.debugPaintPointersEnabled)
                {
                    debugPaintPointers(context, offset);
                }
                return true;
            });
    }

    public virtual void debugPaintSize(PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                var paint = ((Func<Paint>)(() =>
{
    var __cascade = new Paint();
    __cascade.style = PaintingStyle.stroke;
    __cascade.strokeWidth = 1.0;
    __cascade.color = new Color(4278255615L);
    return __cascade;
}))();
                context.canvas.drawRect((offset & size).deflate(0.5), paint);
                return true;
            });
    }

    public virtual void debugPaintBaselines(PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                var paint = ((Func<Paint>)(() =>
{
    var __cascade = new Paint();
    __cascade.style = PaintingStyle.stroke;
    __cascade.strokeWidth = 0.25;
    return __cascade;
}))();
                Path path = default!;
                double? baselineI = getDistanceToBaseline(TextBaseline.ideographic, onlyReal: true);
                if (baselineI is not null)
                {
                    double baselineI__136228__value136315 = DartRuntimePrimitives.RequireValue(baselineI);
                    paint.color = new Color(4294955008L);
                    path = new Path();
                    path.moveTo(offset.dx, offset.dy + DartRuntimePrimitives.RequireValue(baselineI__136228__value136315));
                    path.lineTo(offset.dx + size.width, offset.dy + DartRuntimePrimitives.RequireValue(baselineI__136228__value136315));
                    context.canvas.drawPath(path, paint);
                }
                double? baselineA = getDistanceToBaseline(TextBaseline.alphabetic, onlyReal: true);
                if (baselineA is not null)
                {
                    double baselineA__136632__value136718 = DartRuntimePrimitives.RequireValue(baselineA);
                    paint.color = new Color(4278255360L);
                    path = new Path();
                    path.moveTo(offset.dx, offset.dy + DartRuntimePrimitives.RequireValue(baselineA__136632__value136718));
                    path.lineTo(offset.dx + size.width, offset.dy + DartRuntimePrimitives.RequireValue(baselineA__136632__value136718));
                    context.canvas.drawPath(path, paint);
                }
                return true;
            });
    }

    public virtual void debugPaintPointers(PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (_debugActivePointers > 0L)
                {
                    var paint = ((Func<Paint>)(() =>
{
    var __cascade = new Paint();
    __cascade.color = new Color(48059L | 67108864L * depth & 4278190080L);
    return __cascade;
}))();
                    context.canvas.drawRect(offset & size, paint);
                }
                return true;
            });
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Size>("size", _size, missingIfNull: true));
    }

}

public interface RenderBoxContainerDefaultsMixin<ChildType, ParentDataType> : ContainerRenderObjectMixin<ChildType, ParentDataType> where ChildType : RenderBox where ParentDataType : ContainerBoxParentData<ChildType>
{
    public double? defaultComputeDistanceToFirstActualBaseline(TextBaseline baseline);
    public double? defaultComputeDistanceToHighestActualBaseline(TextBaseline baseline);
    public bool defaultHitTestChildren(BoxHitTestResult result, Offset position);
    public void defaultPaint(PaintingContext context, Offset offset);
    public List<ChildType> getChildrenAsList();
}
