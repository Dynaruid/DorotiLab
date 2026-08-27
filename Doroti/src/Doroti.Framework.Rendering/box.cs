// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/box.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

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
        __instance.minWidth = (width ?? 0.0);
        __instance.maxWidth = (width ?? double.PositiveInfinity);
        __instance.minHeight = (height ?? 0.0);
        __instance.maxHeight = (height ?? double.PositiveInfinity);
        return __instance;
    }

    public static BoxConstraints CreateTightForFinite(double width = double.PositiveInfinity, double height = double.PositiveInfinity)
    {
        var __instance = new BoxConstraints(default!, default!, default!, default!);
        __instance.minWidth = ((DartRuntimePrimitives.RequireValue(width) != double.PositiveInfinity) ? DartRuntimePrimitives.RequireValue(width) : 0.0);
        __instance.maxWidth = ((DartRuntimePrimitives.RequireValue(width) != double.PositiveInfinity) ? DartRuntimePrimitives.RequireValue(width) : double.PositiveInfinity);
        __instance.minHeight = ((DartRuntimePrimitives.RequireValue(height) != double.PositiveInfinity) ? DartRuntimePrimitives.RequireValue(height) : 0.0);
        __instance.maxHeight = ((DartRuntimePrimitives.RequireValue(height) != double.PositiveInfinity) ? DartRuntimePrimitives.RequireValue(height) : double.PositiveInfinity);
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
        __instance.minWidth = (width ?? double.PositiveInfinity);
        __instance.maxWidth = (width ?? double.PositiveInfinity);
        __instance.minHeight = (height ?? double.PositiveInfinity);
        __instance.maxHeight = (height ?? double.PositiveInfinity);
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
        return new BoxConstraints(minWidth: (minWidth ?? this.minWidth), maxWidth: (maxWidth ?? this.maxWidth), minHeight: (minHeight ?? this.minHeight), maxHeight: (maxHeight ?? this.maxHeight));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BoxConstraints deflate(global::Doroti.Framework.Painting.EdgeInsetsGeometry edges)
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        double horizontalLocal = ((global::Doroti.Framework.Painting.EdgeInsetsGeometry)edges).horizontal;
        double verticalLocal = ((global::Doroti.Framework.Painting.EdgeInsetsGeometry)edges).vertical;
        double deflatedMinWidth = Math.Max(0.0, (this.minWidth - horizontalLocal));
        double deflatedMinHeight = Math.Max(0.0, (this.minHeight - verticalLocal));
        return new BoxConstraints(minWidth: deflatedMinWidth, maxWidth: Math.Max(deflatedMinWidth, (this.maxWidth - horizontalLocal)), minHeight: deflatedMinHeight, maxHeight: Math.Max(deflatedMinHeight, (this.maxHeight - verticalLocal)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BoxConstraints loosen()
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        return new BoxConstraints(maxWidth: this.maxWidth, maxHeight: this.maxHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BoxConstraints enforce(BoxConstraints constraints)
    {
        return new BoxConstraints(minWidth: Dart_uiLibrary.clampDouble(this.minWidth, ((BoxConstraints)constraints).minWidth, ((BoxConstraints)constraints).maxWidth), maxWidth: Dart_uiLibrary.clampDouble(this.maxWidth, ((BoxConstraints)constraints).minWidth, ((BoxConstraints)constraints).maxWidth), minHeight: Dart_uiLibrary.clampDouble(this.minHeight, ((BoxConstraints)constraints).minHeight, ((BoxConstraints)constraints).maxHeight), maxHeight: Dart_uiLibrary.clampDouble(this.maxHeight, ((BoxConstraints)constraints).minHeight, ((BoxConstraints)constraints).maxHeight));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BoxConstraints tighten(double? width = null, double? height = null)
    {
        return new BoxConstraints(minWidth: ((width is null) ? this.minWidth : Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(width), this.minWidth, this.maxWidth)), maxWidth: ((width is null) ? this.maxWidth : Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(width), this.minWidth, this.maxWidth)), minHeight: ((height is null) ? this.minHeight : Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(height), this.minHeight, this.maxHeight)), maxHeight: ((height is null) ? this.maxHeight : Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(height), this.minHeight, this.maxHeight)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BoxConstraints flipped
    {
        get
        {
            return new BoxConstraints(minWidth: this.minHeight, maxWidth: this.maxHeight, minHeight: this.minWidth, maxHeight: this.maxWidth);
            return default!;
        }
    }
    public virtual BoxConstraints widthConstraints() => new BoxConstraints(minWidth: this.minWidth, maxWidth: this.maxWidth);
    public virtual BoxConstraints heightConstraints() => new BoxConstraints(minHeight: this.minHeight, maxHeight: this.maxHeight);
    public virtual double constrainWidth(double width = double.PositiveInfinity)
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        return Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(width), this.minWidth, this.maxWidth);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double constrainHeight(double height = double.PositiveInfinity)
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        return Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(height), this.minHeight, this.maxHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Size _debugPropagateDebugSize(Size size, Size result)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((size is _DebugSize__box))
                {
                    _DebugSize__box size__as10760 = (_DebugSize__box)size;
                    result = new _DebugSize__box(result, ((_DebugSize__box)size__as10760)._owner, ((_DebugSize__box)size__as10760)._canBeUsedByParent);
                }
                return true;
            });
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Size constrain(Size size)
    {
        var result = new global::Doroti.Ui.Size(constrainWidth(DartRuntimePrimitives.RequireValue(size.width)), constrainHeight(DartRuntimePrimitives.RequireValue(size.height)));
        DartRuntimePrimitives.Assert(() =>
            {
                result = _debugPropagateDebugSize(size, result);
                return true;
            });
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Size constrainDimensions(double width, double height)
    {
        return new global::Doroti.Ui.Size(constrainWidth(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(width))), constrainHeight(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(height))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Size constrainSizeAndAttemptToPreserveAspectRatio(Size size)
    {
        if (this.isTight)
        {
            global::Doroti.Ui.Size result = this.smallest;
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
        double aspectRatio = (DartRuntimePrimitives.RequireValue(widthLocal) / DartRuntimePrimitives.RequireValue(heightLocal));
        if ((DartRuntimePrimitives.RequireValue(widthLocal) > this.maxWidth))
        {
            widthLocal = this.maxWidth;
            heightLocal = (DartRuntimePrimitives.RequireValue(widthLocal) / aspectRatio);
        }
        if ((DartRuntimePrimitives.RequireValue(heightLocal) > this.maxHeight))
        {
            heightLocal = this.maxHeight;
            widthLocal = (DartRuntimePrimitives.RequireValue(heightLocal) * aspectRatio);
        }
        if ((DartRuntimePrimitives.RequireValue(widthLocal) < this.minWidth))
        {
            widthLocal = this.minWidth;
            heightLocal = (DartRuntimePrimitives.RequireValue(widthLocal) / aspectRatio);
        }
        if ((DartRuntimePrimitives.RequireValue(heightLocal) < this.minHeight))
        {
            heightLocal = this.minHeight;
            widthLocal = (DartRuntimePrimitives.RequireValue(heightLocal) * aspectRatio);
        }
        var resultLocal = new global::Doroti.Ui.Size(constrainWidth(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(widthLocal))), constrainHeight(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(heightLocal))));
        DartRuntimePrimitives.Assert(() =>
            {
                resultLocal = _debugPropagateDebugSize(size, resultLocal);
                return true;
            });
        return resultLocal;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Size biggest => new global::Doroti.Ui.Size(constrainWidth(), constrainHeight());
    public virtual global::Doroti.Ui.Size smallest => new global::Doroti.Ui.Size(constrainWidth(0.0), constrainHeight(0.0));
    public virtual bool hasTightWidth => (this.minWidth >= this.maxWidth);
    public virtual bool hasTightHeight => (this.minHeight >= this.maxHeight);
    public override bool isTight => (this.hasTightWidth && this.hasTightHeight);
    public virtual bool hasBoundedWidth => (this.maxWidth < double.PositiveInfinity);
    public virtual bool hasBoundedHeight => (this.maxHeight < double.PositiveInfinity);
    public virtual bool hasInfiniteWidth => (this.minWidth >= double.PositiveInfinity);
    public virtual bool hasInfiniteHeight => (this.minHeight >= double.PositiveInfinity);
    public virtual bool isSatisfiedBy(Size size)
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        return (((((this.minWidth <= size.width)) && ((size.width <= this.maxWidth))) && ((this.minHeight <= size.height))) && ((size.height <= this.maxHeight)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BoxConstraints op_Multiply(double factor)
    {
        return new BoxConstraints(minWidth: (this.minWidth * factor), maxWidth: (this.maxWidth * factor), minHeight: (this.minHeight * factor), maxHeight: (this.maxHeight * factor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BoxConstraints op_Divide(double factor)
    {
        return new BoxConstraints(minWidth: (this.minWidth / factor), maxWidth: (this.maxWidth / factor), minHeight: (this.minHeight / factor), maxHeight: (this.maxHeight / factor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BoxConstraints ___(double factor)
    {
        return new BoxConstraints(minWidth: ((checked((long)(this.minWidth / factor)))).toDouble(), maxWidth: ((checked((long)(this.maxWidth / factor)))).toDouble(), minHeight: ((checked((long)(this.minHeight / factor)))).toDouble(), maxHeight: ((checked((long)(this.maxHeight / factor)))).toDouble());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BoxConstraints __(double value)
    {
        return new BoxConstraints(minWidth: (this.minWidth % value), maxWidth: (this.maxWidth % value), minHeight: (this.minHeight % value), maxHeight: (this.maxHeight % value));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static BoxConstraints? lerp(BoxConstraints? a, BoxConstraints? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if ((a is null))
        {
            return (b!.op_Multiply(t));
        }
        if ((b is null))
        {
            return (a.op_Multiply(((1.0 - t))));
        }
        DartRuntimePrimitives.Assert(() => a.debugAssertIsValid());
        DartRuntimePrimitives.Assert(() => b.debugAssertIsValid());
        DartRuntimePrimitives.Assert(() => (((double.IsFinite(((BoxConstraints)a).minWidth) && double.IsFinite(((BoxConstraints)b).minWidth))) || (((((BoxConstraints)a).minWidth == double.PositiveInfinity) && (((BoxConstraints)b).minWidth == double.PositiveInfinity)))));
        DartRuntimePrimitives.Assert(() => (((double.IsFinite(((BoxConstraints)a).maxWidth) && double.IsFinite(((BoxConstraints)b).maxWidth))) || (((((BoxConstraints)a).maxWidth == double.PositiveInfinity) && (((BoxConstraints)b).maxWidth == double.PositiveInfinity)))));
        DartRuntimePrimitives.Assert(() => (((double.IsFinite(((BoxConstraints)a).minHeight) && double.IsFinite(((BoxConstraints)b).minHeight))) || (((((BoxConstraints)a).minHeight == double.PositiveInfinity) && (((BoxConstraints)b).minHeight == double.PositiveInfinity)))));
        DartRuntimePrimitives.Assert(() => (((double.IsFinite(((BoxConstraints)a).maxHeight) && double.IsFinite(((BoxConstraints)b).maxHeight))) || (((((BoxConstraints)a).maxHeight == double.PositiveInfinity) && (((BoxConstraints)b).maxHeight == double.PositiveInfinity)))));
        return new BoxConstraints(minWidth: (double.IsFinite(((BoxConstraints)a).minWidth) ? DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((BoxConstraints)a).minWidth, ((BoxConstraints)b).minWidth, t)) : double.PositiveInfinity), maxWidth: (double.IsFinite(((BoxConstraints)a).maxWidth) ? DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((BoxConstraints)a).maxWidth, ((BoxConstraints)b).maxWidth, t)) : double.PositiveInfinity), minHeight: (double.IsFinite(((BoxConstraints)a).minHeight) ? DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((BoxConstraints)a).minHeight, ((BoxConstraints)b).minHeight, t)) : double.PositiveInfinity), maxHeight: (double.IsFinite(((BoxConstraints)a).maxHeight) ? DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((BoxConstraints)a).maxHeight, ((BoxConstraints)b).maxHeight, t)) : double.PositiveInfinity));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isNormalized
    {
        get
        {
            return ((((this.minWidth >= 0.0) && (this.minWidth <= this.maxWidth)) && (this.minHeight >= 0.0)) && (this.minHeight <= this.maxHeight));
            return default!;
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
                if ((((double.IsNaN(this.minWidth) || double.IsNaN(this.maxWidth)) || double.IsNaN(this.minHeight)) || double.IsNaN(this.maxHeight)))
                {
                    var affectedFieldsList = new List<string>();
                    DartRuntimePrimitives.Assert(() => (checked((long)(affectedFieldsList.Count)) != 0));
                    if ((checked((long)(affectedFieldsList.Count)) > 1L))
                    {
                        affectedFieldsList.Add($"and {affectedFieldsList.removeLast()}");
                    }
                    string whichFields = (checked((long)(affectedFieldsList.Count)) switch { 1L => affectedFieldsList.Single(), 2L => string.Join(" ", affectedFieldsList), _ => string.Join(", ", affectedFieldsList) });
                    throwError(new ErrorSummary($"BoxConstraints has {((checked((long)(affectedFieldsList.Count)) == 1L) ? "a NaN value" : "NaN values")} in {whichFields}."));
                }
                if (((this.minWidth < 0.0) && (this.minHeight < 0.0)))
                {
                    throwError(new ErrorSummary("BoxConstraints has both a negative minimum width and a negative minimum height."));
                }
                if ((this.minWidth < 0.0))
                {
                    throwError(new ErrorSummary("BoxConstraints has a negative minimum width."));
                }
                if ((this.minHeight < 0.0))
                {
                    throwError(new ErrorSummary("BoxConstraints has a negative minimum height."));
                }
                if (((this.maxWidth < this.minWidth) && (this.maxHeight < this.minHeight)))
                {
                    throwError(new ErrorSummary("BoxConstraints has both width and height constraints non-normalized."));
                }
                if ((this.maxWidth < this.minWidth))
                {
                    throwError(new ErrorSummary("BoxConstraints has non-normalized width constraints."));
                }
                if ((this.maxHeight < this.minHeight))
                {
                    throwError(new ErrorSummary("BoxConstraints has non-normalized height constraints."));
                }
                if (isAppliedConstraint)
                {
                    if ((double.IsInfinity(this.minWidth) && double.IsInfinity(this.minHeight)))
                    {
                        throwError(new ErrorSummary("BoxConstraints forces an infinite width and infinite height."));
                    }
                    if (double.IsInfinity(this.minWidth))
                    {
                        throwError(new ErrorSummary("BoxConstraints forces an infinite width."));
                    }
                    if (double.IsInfinity(this.minHeight))
                    {
                        throwError(new ErrorSummary("BoxConstraints forces an infinite height."));
                    }
                }
                DartRuntimePrimitives.Assert(() => this.isNormalized);
                return true;
            });
        return this.isNormalized;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BoxConstraints normalize()
    {
        if (this.isNormalized)
        {
            return this;
        }
        double minWidthLocal = ((this.minWidth >= 0.0) ? this.minWidth : 0.0);
        double minHeightLocal = ((this.minHeight >= 0.0) ? this.minHeight : 0.0);
        return new BoxConstraints(minWidth: DartRuntimePrimitives.RequireValue(minWidthLocal), maxWidth: ((DartRuntimePrimitives.RequireValue(minWidthLocal) > this.maxWidth) ? DartRuntimePrimitives.RequireValue(minWidthLocal) : this.maxWidth), minHeight: DartRuntimePrimitives.RequireValue(minHeightLocal), maxHeight: ((DartRuntimePrimitives.RequireValue(minHeightLocal) > this.maxHeight) ? DartRuntimePrimitives.RequireValue(minHeightLocal) : this.maxHeight));
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
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        DartRuntimePrimitives.Assert(() => ((__other is BoxConstraints) && ((BoxConstraints)__other).debugAssertIsValid()));
        return (((((__other is BoxConstraints) && (((BoxConstraints)((BoxConstraints)__other)).minWidth == this.minWidth)) && (((BoxConstraints)((BoxConstraints)__other)).maxWidth == this.maxWidth)) && (((BoxConstraints)((BoxConstraints)__other)).minHeight == this.minHeight)) && (((BoxConstraints)((BoxConstraints)__other)).maxHeight == this.maxHeight));
    }

    public override int GetHashCode()
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        return FoundationRuntimePorts.ObjectHash(this.minWidth, this.maxWidth, this.minHeight, this.maxHeight);
        return default!;
    }
    public override string ToString()
    {
        var annotation = (this.isNormalized ? "" : "; NOT NORMALIZED");
        if (((this.minWidth == double.PositiveInfinity) && (this.minHeight == double.PositiveInfinity)))
        {
            return $"BoxConstraints(biggest{annotation})";
        }
        if (((((this.minWidth == 0L) && (this.maxWidth == double.PositiveInfinity)) && (this.minHeight == 0L)) && (this.maxHeight == double.PositiveInfinity)))
        {
            return $"BoxConstraints(unconstrained{annotation})";
        }
        string describe(double min, double max, string dim)
        {
            if ((min == max))
            {
                return $"{dim}={min.toStringAsFixed(1L)}";
            }
            return $"{min.toStringAsFixed(1L)}<={dim}<={max.toStringAsFixed(1L)}";
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        string width = describe(DartRuntimePrimitives.RequireValue(this.minWidth), DartRuntimePrimitives.RequireValue(this.maxWidth), "w");
        string height = describe(DartRuntimePrimitives.RequireValue(this.minHeight), DartRuntimePrimitives.RequireValue(this.maxHeight), "h");
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

    public static BoxHitTestResult CreateWrap(HitTestResult result)
    {
        return new BoxHitTestResult(result);
    }

    public virtual bool addWithPaintTransform(Matrix4? transform, Offset position, Func<BoxHitTestResult, Offset, bool> hitTest)
    {
        if ((transform is not null))
        {
            transform = Matrix4.tryInvert(PointerEvent.removePerspectiveTransform(transform));
            if ((transform is null))
            {
                return false;
            }
        }
        return addWithRawTransform(transform: transform, position: position, hitTest: (Func<BoxHitTestResult, Offset, bool>)hitTest);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool addWithPaintOffset(Offset? offset, Offset position, Func<BoxHitTestResult, Offset, bool> hitTest)
    {
        global::Doroti.Ui.Offset transformedPosition = ((offset is null) ? position : (position - DartRuntimePrimitives.RequireValue(offset)));
        if ((offset is not null))
        {
            Offset offset__value30995 = DartRuntimePrimitives.RequireValue(offset);
            pushOffset(-DartRuntimePrimitives.RequireValue(offset__value30995));
        }
        bool isHit = hitTest(this, transformedPosition);
        if ((offset is not null))
        {
            Offset offset__value31113 = DartRuntimePrimitives.RequireValue(offset);
            popTransform();
        }
        return isHit;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool addWithRawTransform(Matrix4? transform, Offset position, Func<BoxHitTestResult, Offset, bool> hitTest)
    {
        global::Doroti.Ui.Offset transformedPosition = ((transform is null) ? position : MatrixUtils.transformPoint(transform, position));
        if ((transform is not null))
        {
            pushTransform(transform);
        }
        bool isHit = hitTest(this, transformedPosition);
        if ((transform is not null))
        {
            popTransform();
        }
        return isHit;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool addWithOutOfBandPosition(Offset? paintOffset = null, Matrix4? paintTransform = null, Matrix4? rawTransform = null, Func<BoxHitTestResult, bool> hitTest = default!)
    {
        DartRuntimePrimitives.Assert(() => ((((((paintOffset is null) && (paintTransform is null)) && (rawTransform is not null))) || ((((paintOffset is null) && (paintTransform is not null)) && (rawTransform is null)))) || ((((paintOffset is not null) && (paintTransform is null)) && (rawTransform is null)))));
        if ((paintOffset is not null))
        {
            Offset paintOffset__value34120 = DartRuntimePrimitives.RequireValue(paintOffset);
            pushOffset(-DartRuntimePrimitives.RequireValue(paintOffset__value34120));
        }
        else
        {
            if ((rawTransform is not null))
            {
                pushTransform(rawTransform);
            }
            else
            {
                DartRuntimePrimitives.Assert(() => (paintTransform is not null));
                paintTransform = Matrix4.tryInvert(PointerEvent.removePerspectiveTransform(paintTransform!));
                DartRuntimePrimitives.Assert(() => (paintTransform is not null));
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

    public override string ToString() => $"{(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(target))}@{this.localPosition}";
}

public class BoxParentData : ParentData
{
    public virtual Offset offset { get; set; } = Offset.zero;

    public override string ToString() => $"offset={this.offset}";
}

public abstract class ContainerBoxParentData<ChildType> : BoxParentData, ContainerParentDataMixin<ChildType> where ChildType : RenderObject
{
    public virtual ChildType? previousSibling { get; set; } = default;
    public virtual ChildType? nextSibling { get; set; } = default;

    public override void detach()
    {
        DartRuntimePrimitives.Assert(() => (this.previousSibling is null));
        DartRuntimePrimitives.Assert(() => (this.nextSibling is null));
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
        return new BaselineOffset(((value is null) ? null : (DartRuntimePrimitives.RequireValue(value) + offset)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BaselineOffset minOf(BaselineOffset other)
    {
        return (((this).offset, (other).offset) switch { (double lhs, double rhs) => ((lhs >= rhs) ? other : this), (double lhsLocal, null) => new BaselineOffset(lhsLocal), (null, var rhsLocal) => rhsLocal });
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

    public virtual global::Doroti.Ui.Size memoize(_LayoutCacheStorage__box cacheStorage, BoxConstraints input, Func<BoxConstraints, Size> computer)
    {
        return (cacheStorage._cachedDryLayoutSizes ??= new DartMap<BoxConstraints, Size>()).putIfAbsent(input, (() => computer(input)));
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
        DartMap<BoxConstraints, BaselineOffset> cache = (input.Item2 switch { TextBaseline.alphabetic => cacheStorage._cachedAlphabeticBaseline ??= new DartMap<BoxConstraints, BaselineOffset>(), TextBaseline.ideographic => cacheStorage._cachedIdeoBaseline ??= new DartMap<BoxConstraints, BaselineOffset>(), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
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
        return (cacheStorage._cachedIntrinsicDimensions ??= new DartMap<(_IntrinsicDimension__box, double), double>()).putIfAbsent((value, input), (() => computer(input)));
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
        bool hasCache = ((((((((long?)(this._cachedDryLayoutSizes?.Count)) is { } __count42271 ? __count42271 != 0 : (bool?)null) ?? false)) || (((((long?)(this._cachedIntrinsicDimensions?.Count)) is { } __count42327 ? __count42327 != 0 : (bool?)null) ?? false))) || (((((long?)(this._cachedAlphabeticBaseline?.Count)) is { } __count42388 ? __count42388 != 0 : (bool?)null) ?? false))) || (((((long?)(this._cachedIdeoBaseline?.Count)) is { } __count42448 ? __count42448 != 0 : (bool?)null) ?? false)));
        if (hasCache)
        {
            this._cachedDryLayoutSizes?.Clear();
            this._cachedIntrinsicDimensions?.Clear();
            this._cachedAlphabeticBaseline?.Clear();
            this._cachedIdeoBaseline?.Clear();
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
        if ((((RenderObject)child).parentData is not BoxParentData))
        {
            child.parentData = new BoxParentData();
        }
    }

    internal virtual Output _computeIntrinsics<Input, Output>(_CachedLayoutCalculation__box<Input, Output> type, Input input, Func<Input, Output> computer)
    {
        DartRuntimePrimitives.Assert(() => (RenderObject.debugCheckingIntrinsics || !debugDoingThisResize));
        var shouldCache = true;
        DartRuntimePrimitives.Assert(() =>
            {
                shouldCache = !RenderObject.debugCheckingIntrinsics;
                return true;
            });
        return (shouldCache ? _computeWithTimeline(type, input, (Func<Input, Output>)computer) : computer(input));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Output _computeWithTimeline<Input, Output>(_CachedLayoutCalculation__box<Input, Output> type, Input input, Func<Input, Output> computer)
    {
        DartMap<string, string>? debugTimelineArguments = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                DartMap<string, string> argumentsLocal = (global::Doroti.Framework.Rendering.DebugLibrary.debugEnhanceLayoutTimelineArguments ? toDiagnosticsNode().toTimelineArguments()! : new DartMap<string, string>());
                debugTimelineArguments = type.debugFillTimelineArguments(argumentsLocal, input);
                return true;
            });
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            if ((global::Doroti.Framework.Rendering.DebugLibrary.debugProfileLayoutsEnabled || (_debugIntrinsicsDepth == 0L)))
            {
                FlutterTimeline.startSync(type.eventLabel(this), arguments: debugTimelineArguments);
            }
            _debugIntrinsicsDepth += 1L;
        }
        Output result = type.memoize(this._layoutCacheStorage, input, (Func<Input, Output>)computer);
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            _debugIntrinsicsDepth -= 1L;
            if ((global::Doroti.Framework.Rendering.DebugLibrary.debugProfileLayoutsEnabled || (_debugIntrinsicsDepth == 0L)))
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
                if ((height < 0.0))
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("The height argument to getMinIntrinsicWidth was negative."), new ErrorDescription("The argument to getMinIntrinsicWidth must not be negative or null."), new ErrorHint("If you perform computations on another height before passing it to " + "getMinIntrinsicWidth, consider using math.max() or double.clamp() " + "to force the value into the valid range.") });
                }
                return true;
            });
        return _computeIntrinsics(new _IntrinsicDimension__boxInterfaceAdapter(_IntrinsicDimension__box.minWidth), height, this.computeMinIntrinsicWidth);
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
                if ((height < 0.0))
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("The height argument to getMaxIntrinsicWidth was negative."), new ErrorDescription("The argument to getMaxIntrinsicWidth must not be negative or null."), new ErrorHint("If you perform computations on another height before passing it to " + "getMaxIntrinsicWidth, consider using math.max() or double.clamp() " + "to force the value into the valid range.") });
                }
                return true;
            });
        return _computeIntrinsics(new _IntrinsicDimension__boxInterfaceAdapter(_IntrinsicDimension__box.maxWidth), height, this.computeMaxIntrinsicWidth);
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
                if ((width < 0.0))
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("The width argument to getMinIntrinsicHeight was negative."), new ErrorDescription("The argument to getMinIntrinsicHeight must not be negative or null."), new ErrorHint("If you perform computations on another width before passing it to " + "getMinIntrinsicHeight, consider using math.max() or double.clamp() " + "to force the value into the valid range.") });
                }
                return true;
            });
        return _computeIntrinsics(new _IntrinsicDimension__boxInterfaceAdapter(_IntrinsicDimension__box.minHeight), width, this.computeMinIntrinsicHeight);
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
                if ((width < 0.0))
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("The width argument to getMaxIntrinsicHeight was negative."), new ErrorDescription("The argument to getMaxIntrinsicHeight must not be negative or null."), new ErrorHint("If you perform computations on another width before passing it to " + "getMaxIntrinsicHeight, consider using math.max() or double.clamp() " + "to force the value into the valid range.") });
                }
                return true;
            });
        return _computeIntrinsics(new _IntrinsicDimension__boxInterfaceAdapter(_IntrinsicDimension__box.maxHeight), width, this.computeMaxIntrinsicHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double computeMaxIntrinsicHeight(double width)
    {
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Size getDryLayout(BoxConstraints constraints)
    {
        return _computeIntrinsics(_CachedLayoutCalculation__box<object, object>.dryLayout, constraints, (BoxConstraints __constraints) => this._computeDryLayout(__constraints));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Size _computeDryLayout(BoxConstraints constraints, Func<RenderBox, BoxConstraints, Size> layoutChild = default!)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                DartRuntimePrimitives.Assert(() => !this._computingThisDryLayout);
                _computingThisDryLayout = true;
                return true;
            });
        global::Doroti.Ui.Size result = computeDryLayout(constraints);
        DartRuntimePrimitives.Assert(() =>
            {
                DartRuntimePrimitives.Assert(() => this._computingThisDryLayout);
                _computingThisDryLayout = false;
                return true;
            });
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Size computeDryLayout(BoxConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() => debugCannotComputeDryLayout(error: new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"The {(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "RenderBox"))} class does not implement \"computeDryLayout\"."), new ErrorHint("If you are not writing your own RenderBox subclass, then this is not\n" + "your fault. Contact support: https://github.com/flutter/flutter/issues/new?template=02_bug.yml") })));
        return Size.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? getDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        double? baselineOffset = _computeIntrinsics(_CachedLayoutCalculation__box<object, object>.baseline, (constraints, baseline), (Func<(BoxConstraints, TextBaseline), BaselineOffset>)this._computeDryBaseline).offset;
        DartRuntimePrimitives.Assert(() => (RenderObject.debugCheckingIntrinsics || (baselineOffset == computeDryBaseline(constraints, baseline))));
        return baselineOffset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual BaselineOffset _computeDryBaseline((BoxConstraints, TextBaseline) pair)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                DartRuntimePrimitives.Assert(() => !this._computingThisDryBaseline);
                _computingThisDryBaseline = true;
                return true;
            });
        var result = new BaselineOffset(computeDryBaseline(pair.Item1, pair.Item2));
        DartRuntimePrimitives.Assert(() =>
            {
                DartRuntimePrimitives.Assert(() => this._computingThisDryBaseline);
                _computingThisDryBaseline = false;
                return true;
            });
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => debugCannotComputeDryLayout(error: new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"The {(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "RenderBox"))} class does not implement \"computeDryBaseline\"."), new ErrorHint("If you are not writing your own RenderBox subclass, then this is not\n" + "your fault. Contact support: https://github.com/flutter/flutter/issues/new?template=02_bug.yml") })));
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool debugCannotComputeDryLayout(string? reason = null, FlutterError? error = null)
    {
        DartRuntimePrimitives.Assert(() => (((reason is null)) != ((error is null))));
        DartRuntimePrimitives.Assert(() =>
            {
                if (!RenderObject.debugCheckingIntrinsics)
                {
                    if ((reason is not null))
                    {
                        DartRuntimePrimitives.Assert(() => (error is null));
                        throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"The {(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "RenderBox"))} class does not support dry layout.") });
                    }
                    DartRuntimePrimitives.Assert(() => (error is not null));
                    throw error!;
                }
                _debugDryLayoutCalculationValid = false;
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool hasSize => (this._size is not null);
    public virtual global::Doroti.Ui.Size size
    {
        get
        {
            DartRuntimePrimitives.Assert(() => this.hasSize);
            DartRuntimePrimitives.Assert(() =>
                {
                    global::Doroti.Ui.Size? size = this._size;
                    if ((size is _DebugSize__box))
                    {
                        _DebugSize__box size__93552__as93576 = (_DebugSize__box)size;
                        DartRuntimePrimitives.Assert(() => (object.Equals(((_DebugSize__box)size__93552__as93576)._owner, this)));
                        RenderObject? parentLocal = this.parent;
                        bool doingRegularLayout = !((RenderObject.debugActiveLayout?.debugDoingThisLayoutWithCallback ?? true));
                        bool sizeAccessAllowed = ((((!doingRegularLayout || debugDoingThisResize) || debugDoingThisLayout) || _debugDoingBaseline) || ((object.Equals(RenderObject.debugActiveLayout, parentLocal)) && ((_DebugSize__box)size__93552__as93576)._canBeUsedByParent));
                        DartRuntimePrimitives.Assert(() => sizeAccessAllowed);
                        RenderBox? renderBoxDoingDryLayout = (this._computingThisDryLayout ? this : ((((parentLocal is RenderBox) && ((RenderBox)((RenderBox)parentLocal))._computingThisDryLayout) ? ((RenderBox)parentLocal) : null)));
                        DartRuntimePrimitives.Assert(() => (renderBoxDoingDryLayout is null));
                        RenderBox? renderBoxDoingDryBaseline = (this._computingThisDryBaseline ? this : ((((parentLocal is RenderBox) && ((RenderBox)((RenderBox)parentLocal))._computingThisDryBaseline) ? ((RenderBox)parentLocal) : null)));
                        DartRuntimePrimitives.Assert(() => (renderBoxDoingDryBaseline is null));
                        DartRuntimePrimitives.Assert(() => (object.Equals(size__93552__as93576, this._size)));
                    }
                    return true;
                });
            return (this._size ?? throw new InvalidOperationException($"RenderBox was not laid out: {this.GetType()}#{(global::Doroti.Framework.Foundation.DiagnosticsLibrary.shortHash(this))}"));
            return default!;
        }
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
                    var information = new List<DiagnosticsNode> { new ErrorSummary("RenderBox size setter called incorrectly.") };
                    if (debugDoingThisLayout)
                    {
                        DartRuntimePrimitives.Assert(() => sizedByParent);
                        information.Add(new ErrorDescription("It appears that the size setter was called from performLayout()."));
                    }
                    else
                    {
                        information.Add(new ErrorDescription("The size setter was called from outside layout (neither performResize() nor performLayout() were being run for this object)."));
                        if (((owner is not null) && owner!.debugDoingLayout))
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
    public virtual global::Doroti.Ui.Size debugAdoptSize(Size value)
    {
        var result = value;
        DartRuntimePrimitives.Assert(() =>
            {
                if ((value is _DebugSize__box))
                {
                    _DebugSize__box value__as99138 = (_DebugSize__box)value;
                    if ((!object.Equals(((_DebugSize__box)value__as99138)._owner, this)))
                    {
                        if ((!object.Equals(((_DebugSize__box)value__as99138)._owner.parent, this)))
                        {
                            throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("The size property was assigned a size inappropriately."), describeForError("The following render object"), ((_DebugSize__box)value__as99138)._owner.describeForError("...was assigned a size obtained from"), new ErrorDescription("However, this second render object is not, or is no longer, a " + "child of the first, and it is therefore a violation of the " + "RenderBox layout protocol to use that size in the layout of the " + "first render object."), new ErrorHint("If the size was obtained at a time where it was valid to read " + "the size (because the second render object above was a child " + "of the first at the time), then it should be adopted using " + "debugAdoptSize at that time."), new ErrorHint("If the size comes from a grandchild or a render object from an " + "entirely different part of the render tree, then there is no " + "way to be notified when the size changes and therefore attempts " + "to read that size are almost certainly a source of bugs. A different " + "approach should be used.") });
                        }
                        if (!((_DebugSize__box)value__as99138)._canBeUsedByParent)
                        {
                            throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("A child's size was used without setting parentUsesSize."), describeForError("The following render object"), ((_DebugSize__box)value__as99138)._owner.describeForError("...was assigned a size obtained from its child"), new ErrorDescription("However, when the child was laid out, the parentUsesSize argument " + "was not set or set to false. Subsequently this transpired to be " + "inaccurate: the size was nonetheless used by the parent.\n" + "It is important to tell the framework if the size will be used or not " + "as several important performance optimizations can be made if the " + "size will not be used by the parent.") });
                        }
                    }
                }
                result = new _DebugSize__box(value, this, debugCanParentUseSize);
                return true;
            });
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Rect semanticBounds => (Offset.zero & this.size);
    public override void debugResetSize()
    {
        size = this.size;
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
        DartRuntimePrimitives.Assert(() => (!debugNeedsLayout || RenderObject.debugCheckingIntrinsics));
        DartRuntimePrimitives.Assert(() => (RenderObject.debugCheckingIntrinsics || (owner! switch { PipelineOwner { debugDoingLayout: true } __object103560 => ((object.Equals(RenderObject.debugActiveLayout, parent)) && parent!.debugDoingThisLayout), PipelineOwner { debugDoingPaint: true } __object103701 => (((object.Equals(RenderObject.debugActivePaint, parent)) && parent!.debugDoingThisPaint) || (((object.Equals(RenderObject.debugActivePaint, this)) && debugDoingThisPaint))), PipelineOwner __object103923 => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })));
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
        if (((result is null) && !onlyReal))
        {
            return this.size.height;
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? getDistanceToActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => _debugDoingBaseline);
        return _computeIntrinsics(_CachedLayoutCalculation__box<object, object>.baseline, (this.constraints, baseline), ((Func<(BoxConstraints, TextBaseline), BaselineOffset>)((pair) => new BaselineOffset(computeDistanceToActualBaseline(pair.Item2))))).offset;
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
                if (!this.hasSize)
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
                if (!DartRuntimePrimitives.RequireValue(this._size).isFinite)
                {
                    var information = new List<DiagnosticsNode> { new ErrorSummary($"{this.GetType()} object was given an infinite size during layout."), new ErrorDescription("This probably means that it is a render object that tries to be " + "as big as possible, but it was put inside another render object " + "that allows its children to pick their own size.") };
                    if (!((BoxConstraints)this.constraints).hasBoundedWidth)
                    {
                        var node = this;
                        while ((!((RenderBox)node).constraints.hasBoundedWidth && (node.parent is RenderBox)))
                        {
                            node = ((RenderBox?)(object?)node.parent!)!;
                        }
                        information.Add(node.describeForError("The nearest ancestor providing an unbounded width constraint is"));
                    }
                    if (!((BoxConstraints)this.constraints).hasBoundedHeight)
                    {
                        var nodeLocal = this;
                        while ((!((RenderBox)nodeLocal).constraints.hasBoundedHeight && (nodeLocal.parent is RenderBox)))
                        {
                            nodeLocal = ((RenderBox?)(object?)nodeLocal.parent!)!;
                        }
                        information.Add(nodeLocal.describeForError("The nearest ancestor providing an unbounded height constraint is"));
                    }
                    throw new FlutterError(new List<DiagnosticsNode> { new DiagnosticsProperty<BoxConstraints>($"The constraints that applied to the {this.GetType()} were", this.constraints, style: DiagnosticsTreeStyle.errorProperty), new DiagnosticsProperty<global::Doroti.Ui.Size>("The exact size it was given was", this._size, style: DiagnosticsTreeStyle.errorProperty), new ErrorHint("See https://flutter.dev/to/unbounded-constraints for more information.") });
                }
                if (!this.constraints.isSatisfiedBy(DartRuntimePrimitives.RequireValue(this._size)))
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{this.GetType()} does not meet its constraints."), new DiagnosticsProperty<BoxConstraints>("Constraints", this.constraints, style: DiagnosticsTreeStyle.errorProperty), new DiagnosticsProperty<global::Doroti.Ui.Size>("Size", this._size, style: DiagnosticsTreeStyle.errorProperty), new ErrorHint("If you are not writing your own RenderBox subclass, then this is not " + "your fault. Contact support: https://github.com/flutter/flutter/issues/new?template=02_bug.yml") });
                }
                if (global::Doroti.Framework.Rendering.DebugLibrary.debugCheckIntrinsicSizes)
                {
                    DartRuntimePrimitives.Assert(() => !RenderObject.debugCheckingIntrinsics);
                    RenderObject.debugCheckingIntrinsics = true;
                    var failures = new List<DiagnosticsNode>();
                    double testIntrinsic(Func<double, double> function, string name, double constraint)
                    {
                        double result = function(constraint);
                        if ((result < 0L))
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
                        if ((min > max))
                        {
                            failures.Add(new ErrorDescription($" * getMinIntrinsic{name}({constraint}) returned a larger value ({min}) than getMaxIntrinsic{name}({constraint}) ({max})"));
                        }
                    }
                    try
                    {
                        testIntrinsicsForValues(this.getMinIntrinsicWidth, this.getMaxIntrinsicWidth, "Width", double.PositiveInfinity);
                        testIntrinsicsForValues(this.getMinIntrinsicHeight, this.getMaxIntrinsicHeight, "Height", double.PositiveInfinity);
                        if (((BoxConstraints)this.constraints).hasBoundedWidth)
                        {
                            testIntrinsicsForValues(this.getMinIntrinsicWidth, this.getMaxIntrinsicWidth, "Width", ((BoxConstraints)this.constraints).maxHeight);
                        }
                        if (((BoxConstraints)this.constraints).hasBoundedHeight)
                        {
                            testIntrinsicsForValues(this.getMinIntrinsicHeight, this.getMaxIntrinsicHeight, "Height", ((BoxConstraints)this.constraints).maxWidth);
                        }
                    }
                    finally
                    {
                        RenderObject.debugCheckingIntrinsics = false;
                    }
                    if ((checked((long)(failures.Count)) != 0))
                    {
                        throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"The intrinsic dimension methods of the {this.GetType()} class returned values that violate the intrinsic protocol contract."), new ErrorDescription($"The following {((checked((long)(failures.Count)) > 1L) ? "failures" : "failure")} was detected:"), new ErrorHint("If you are not writing your own RenderBox subclass, then this is not\n" + "your fault. Contact support: https://github.com/flutter/flutter/issues/new?template=02_bug.yml") });
                    }
                    _debugDryLayoutCalculationValid = true;
                    RenderObject.debugCheckingIntrinsics = true;
                    global::Doroti.Ui.Size dryLayoutSize = default!;
                    try
                    {
                        dryLayoutSize = getDryLayout(this.constraints);
                    }
                    finally
                    {
                        RenderObject.debugCheckingIntrinsics = false;
                    }
                    if ((_debugDryLayoutCalculationValid && (!object.Equals(dryLayoutSize, this._size))))
                    {
                        throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"The size given to the {(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "RenderBox"))} class differs from the size computed by computeDryLayout."), new ErrorDescription($"The size computed in {(sizedByParent ? "performResize" : "performLayout")} " + $"is {this.size}, which is different from {dryLayoutSize}, which was computed by computeDryLayout."), new ErrorDescription($"The constraints used were {this.constraints}."), new ErrorHint("If you are not writing your own RenderBox subclass, then this is not\n" + "your fault. Contact support: https://github.com/flutter/flutter/issues/new?template=02_bug.yml") });
                    }
                }
                return true;
            });
    }

    internal virtual void _debugVerifyDryBaselines()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                var messages = new List<DiagnosticsNode> { new ErrorDescription($"The constraints used were {this.constraints}."), new ErrorHint("If you are not writing your own RenderBox subclass, then this is not\n" + "your fault. Contact support: https://github.com/flutter/flutter/issues/new?template=02_bug.yml") };
                foreach (global::Doroti.Ui.TextBaseline baseline in System.Enum.GetValues<TextBaseline>().ToList())
                {
                    DartRuntimePrimitives.Assert(() => !RenderObject.debugCheckingIntrinsics);
                    RenderObject.debugCheckingIntrinsics = true;
                    _debugDryLayoutCalculationValid = true;
                    double? dryBaseline = default!;
                    double? realBaseline = default!;
                    try
                    {
                        dryBaseline = getDryBaseline(this.constraints, baseline);
                        realBaseline = getDistanceToBaseline(baseline, onlyReal: true);
                    }
                    finally
                    {
                        RenderObject.debugCheckingIntrinsics = false;
                    }
                    DartRuntimePrimitives.Assert(() => !RenderObject.debugCheckingIntrinsics);
                    if ((!_debugDryLayoutCalculationValid || (dryBaseline == realBaseline)))
                    {
                        continue;
                    }
                    if ((((dryBaseline is null)) != ((realBaseline is null))))
                    {
                        var (methodReturnedNull, methodReturnedNonNull) = ((dryBaseline is null) ? (((string, string))("computeDryBaseline", "computeDistanceToActualBaseline")) : (((string, string))("computeDistanceToActualBaseline", "computeDryBaseline")));
                        throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"The {baseline} location returned by {(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "RenderBox"))}.computeDistanceToActualBaseline " + "differs from the baseline location computed by computeDryBaseline."), new ErrorDescription($"The {methodReturnedNull} method returned null while the {methodReturnedNonNull} returned a non-null {baseline} of {(dryBaseline ?? realBaseline)}. " + $"Did you forget to implement {methodReturnedNull} for {(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "RenderBox"))}?") });
                    }
                    else
                    {
                        throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"The {baseline} location returned by {(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "RenderBox"))}.computeDistanceToActualBaseline " + "differs from the baseline location computed by computeDryBaseline."), new DiagnosticsProperty<RenderObject>("The RenderBox was", this), new ErrorDescription($"The computeDryBaseline method returned {dryBaseline},\n" + $"while the computeDistanceToActualBaseline method returned {realBaseline}.\n" + $"Consider checking the implementations of the following methods on the {(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "RenderBox"))} class and make sure they are consistent:\n" + " * computeDistanceToActualBaseline\n" + " * computeDryBaseline\n" + " * performLayout\n") });
                    }
                }
                return true;
            });
    }

    public override void markNeedsLayout()
    {
        if ((this._layoutCacheStorage.clear() && (parent is not null)))
        {
            markParentNeedsLayout();
            return;
        }
        base.markNeedsLayout();
    }

    public override void performResize()
    {
        size = computeDryLayout(this.constraints);
        DartRuntimePrimitives.Assert(() => this.size.isFinite);
    }

    public override void performLayout()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (!sizedByParent)
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{this.GetType()} did not implement performLayout()."), new ErrorHint("RenderBox subclasses need to either override performLayout() to " + "set a size and lay out any children, or, set sizedByParent to true " + "so that performResize() sizes the render object.") });
                }
                return true;
            });
    }

    public virtual bool hitTest(BoxHitTestResult result, Offset position)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (!this.hasSize)
                {
                    if (debugNeedsLayout)
                    {
                        throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Cannot hit test a render box that has never been laid out."), describeForError("The hitTest() method was called on this RenderBox"), new ErrorDescription("Unfortunately, this object's geometry is not known at this time, " + "probably because it has never been laid out. " + "This means it cannot be accurately hit-tested."), new ErrorHint("If you are trying " + "to perform a hit test during the layout phase itself, make sure " + "you only hit test nodes that have completed layout (e.g. the node's " + "children, after their layout() method has been called).") });
                    }
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Cannot hit test a render box with no size."), describeForError("The hitTest() method was called on this RenderBox"), new ErrorDescription("Although this node is not marked as needing layout, " + "its size is not set."), new ErrorHint("A RenderBox object must have an " + "explicit size before it can be hit-tested. Make sure " + "that the RenderBox in question sets its size during layout.") });
                }
                return true;
            });
        if (DartRuntimePrimitives.RequireValue(this._size).contains(position))
        {
            if ((hitTestChildren(result, position: position) || hitTestSelf(position)))
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
        DartRuntimePrimitives.Assert(() => (object.Equals(((RenderObject)child).parent, this)));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((((RenderObject)child).parentData is not BoxParentData))
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{this.GetType()} does not implement applyPaintTransform."), describeForError($"The following {this.GetType()} object"), child.describeForError("...did not use a BoxParentData class for the parentData field of the following child"), new ErrorDescription($"The {this.GetType()} class inherits from RenderBox."), new ErrorHint("The default applyPaintTransform implementation provided by RenderBox assumes that the " + "children all use BoxParentData objects for their parentData field. " + $"Since {this.GetType()} does not in fact use that ParentData class for its children, it must " + "provide an implementation of applyPaintTransform that supports the specific ParentData " + $"subclass used by its children (which apparently is {DartRuntimePrimitives.RuntimeType(((RenderObject)child).parentData)}).") });
                }
                return true;
            });
        var childParentData = ((BoxParentData?)(object?)((RenderObject)child).parentData!)!;
        global::Doroti.Ui.Offset offsetLocal = ((BoxParentData)childParentData).offset;
        transform.translateByDouble(offsetLocal.dx, offsetLocal.dy, 0, 1);
    }

    public virtual global::Doroti.Ui.Offset globalToLocal(Offset point, RenderObject? ancestor = null)
    {
        Matrix4 transform = getTransformTo(ancestor);
        double det = transform.invert();
        if ((det == 0.0))
        {
            return Offset.zero;
        }
        Vector3 localScreenOrigin = transform.perspectiveTransform(new Vector3(0.0, 0.0, 0.0));
        Vector3 localViewDirection = (transform.perspectiveTransform(new Vector3(0.0, 0.0, 1.0)) - localScreenOrigin);
        if ((localViewDirection.z == 0.0))
        {
            return Offset.zero;
        }
        Vector3 localScreenPoint = transform.perspectiveTransform(new Vector3(point.dx, point.dy, 0.0));
        Vector3 localPoint = (localScreenPoint - (localViewDirection * ((localScreenPoint.z / localViewDirection.z))));
        return new global::Doroti.Ui.Offset(localPoint.x, localPoint.y);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Offset localToGlobal(Offset point, RenderObject? ancestor = null)
    {
        return MatrixUtils.transformPoint(getTransformTo(ancestor), point);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Rect paintBounds => (Offset.zero & this.size);
    public override void handleEvent(global::Doroti.Framework.Gestures.PointerEvent @event, HitTestEntry<HitTestTarget> entry)
    {
        base.handleEvent(@event, entry);
    }

    public virtual bool debugHandleEvent(global::Doroti.Framework.Gestures.PointerEvent @event, HitTestEntry<HitTestTarget> entry)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (global::Doroti.Framework.Rendering.DebugLibrary.debugPaintPointersEnabled)
                {
                    if ((@event is global::Doroti.Framework.Gestures.PointerDownEvent))
                    {
                        global::Doroti.Framework.Gestures.PointerDownEvent @event__as133711 = (global::Doroti.Framework.Gestures.PointerDownEvent)@event;
                        _debugActivePointers += 1L;
                    }
                    else
                    {
                        if (((@event is global::Doroti.Framework.Gestures.PointerUpEvent) || (@event is global::Doroti.Framework.Gestures.PointerCancelEvent)))
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
                if (global::Doroti.Framework.Rendering.DebugLibrary.debugCheckIntrinsicSizes)
                {
                    _debugVerifyDryBaselines();
                }
                if (global::Doroti.Framework.Rendering.DebugLibrary.debugPaintSizeEnabled)
                {
                    debugPaintSize(context, offset);
                }
                if (global::Doroti.Framework.Rendering.DebugLibrary.debugPaintBaselinesEnabled)
                {
                    debugPaintBaselines(context, offset);
                }
                if (global::Doroti.Framework.Rendering.DebugLibrary.debugPaintPointersEnabled)
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
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.style = PaintingStyle.stroke;
    __cascade.strokeWidth = 1.0;
    __cascade.color = new global::Doroti.Ui.Color(4278255615L);
    return __cascade;
}))();
                ((PaintingContext)context).canvas.drawRect(((offset & this.size)).deflate(0.5), paint);
                return true;
            });
    }

    public virtual void debugPaintBaselines(PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                var paint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.style = PaintingStyle.stroke;
    __cascade.strokeWidth = 0.25;
    return __cascade;
}))();
                global::Doroti.Ui.Path path = default!;
                double? baselineI = getDistanceToBaseline(TextBaseline.ideographic, onlyReal: true);
                if ((baselineI is not null))
                {
                    double baselineI__136228__value136315 = DartRuntimePrimitives.RequireValue(baselineI);
                    paint.color = new global::Doroti.Ui.Color(4294955008L);
                    path = new global::Doroti.Ui.Path();
                    path.moveTo(offset.dx, (offset.dy + DartRuntimePrimitives.RequireValue(baselineI__136228__value136315)));
                    path.lineTo((offset.dx + this.size.width), (offset.dy + DartRuntimePrimitives.RequireValue(baselineI__136228__value136315)));
                    ((PaintingContext)context).canvas.drawPath(path, paint);
                }
                double? baselineA = getDistanceToBaseline(TextBaseline.alphabetic, onlyReal: true);
                if ((baselineA is not null))
                {
                    double baselineA__136632__value136718 = DartRuntimePrimitives.RequireValue(baselineA);
                    paint.color = new global::Doroti.Ui.Color(4278255360L);
                    path = new global::Doroti.Ui.Path();
                    path.moveTo(offset.dx, (offset.dy + DartRuntimePrimitives.RequireValue(baselineA__136632__value136718)));
                    path.lineTo((offset.dx + this.size.width), (offset.dy + DartRuntimePrimitives.RequireValue(baselineA__136632__value136718)));
                    ((PaintingContext)context).canvas.drawPath(path, paint);
                }
                return true;
            });
    }

    public virtual void debugPaintPointers(PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._debugActivePointers > 0L))
                {
                    var paint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = new global::Doroti.Ui.Color((48059L | ((((67108864L * depth)) & 4278190080L))));
    return __cascade;
}))();
                    ((PaintingContext)context).canvas.drawRect((offset & this.size), paint);
                }
                return true;
            });
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Size>("size", this._size, missingIfNull: true));
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
