// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/list_wheel_viewport.dart
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

internal delegate double _ChildSizingFunction__list_wheel_viewport(RenderBox child);

public interface ListWheelChildManager
{
    public long? childCount { get; }
    public bool childExistsAt(long index);
    public void createChild(long index, RenderBox? after);
    public void removeChild(RenderBox child);
}

public class ListWheelParentData : ContainerBoxParentData<RenderBox>
{
    public virtual long? index { get; set; } = default;
    public virtual Matrix4? transform { get; set; } = default;

}

public class RenderListWheelViewport : RenderBox, ContainerRenderObjectMixin<RenderBox, ListWheelParentData>
{
    public const double defaultDiameterRatio = 2.0;
    public const double defaultPerspective = 0.003;
    public static string diameterRatioZeroMessage = "You can't set a diameterRatio " + "of 0 or of a negative number. It would imply a cylinder of 0 in diameter " + "in which case nothing will be drawn.";
    public static string perspectiveTooHighMessage = "A perspective too high will " + "be clipped in the z-axis and therefore not renderable. Value must be " + "between 0 and 0.01.";
    public static string clipBehaviorAndRenderChildrenOutsideViewportConflict = "Cannot renderChildrenOutsideViewport and clip since children " + "rendered outside will be clipped anyway.";
    public virtual ListWheelChildManager childManager { get; private set; } = default!;
    internal virtual ViewportOffset _offset { get; set; } = default!;
    internal virtual double _diameterRatio { get; set; } = default!;
    internal virtual double _perspective { get; set; } = default!;
    internal virtual double _offAxisFraction { get; set; } = 0.0;
    internal virtual bool _useMagnifier { get; set; } = false;
    internal virtual double _magnification { get; set; } = 1.0;
    internal virtual double _overAndUnderCenterOpacity { get; set; } = 1.0;
    internal virtual double _itemExtent { get; set; } = default!;
    internal virtual double _squeeze { get; set; } = default!;
    internal virtual bool _renderChildrenOutsideViewport { get; set; } = default!;
    internal virtual Clip _clipBehavior { get; set; } = Clip.hardEdge;
    internal virtual LayerHandle<ClipRectLayer> _clipRectLayer { get; private set; } = new LayerHandle<ClipRectLayer>();
    internal virtual LayerHandle<OpacityLayer> _childOpacityLayerHandler { get; private set; } = new LayerHandle<OpacityLayer>();
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;

    public RenderListWheelViewport(ListWheelChildManager childManager, ViewportOffset offset, double? diameterRatio = null, double? perspective = null, double offAxisFraction = 0, bool useMagnifier = false, double magnification = 1, double overAndUnderCenterOpacity = 1, double itemExtent = default!, double squeeze = 1, bool renderChildrenOutsideViewport = false, Clip clipBehavior = Clip.none, List<RenderBox>? children = null)
    {
        double __diameterRatio = diameterRatio ?? defaultDiameterRatio;
        double __perspective = perspective ?? defaultPerspective;
        this.childManager = childManager;
        this._offset = offset;
        this._diameterRatio = __diameterRatio;
        this._perspective = __perspective;
        this._offAxisFraction = offAxisFraction;
        this._useMagnifier = useMagnifier;
        this._magnification = magnification;
        this._overAndUnderCenterOpacity = overAndUnderCenterOpacity;
        this._itemExtent = itemExtent;
        this._squeeze = squeeze;
        this._renderChildrenOutsideViewport = renderChildrenOutsideViewport;
        this._clipBehavior = clipBehavior;
        System.Diagnostics.Debug.Assert((__diameterRatio > 0L));
        System.Diagnostics.Debug.Assert((__perspective > 0L));
        System.Diagnostics.Debug.Assert((__perspective <= 0.01));
        System.Diagnostics.Debug.Assert((magnification > 0L));
        System.Diagnostics.Debug.Assert(((overAndUnderCenterOpacity >= 0L) && (overAndUnderCenterOpacity <= 1L)));
        System.Diagnostics.Debug.Assert((squeeze > 0L));
        System.Diagnostics.Debug.Assert((itemExtent > 0L));
        System.Diagnostics.Debug.Assert((!renderChildrenOutsideViewport || (object.Equals(clipBehavior, Clip.none))));
    }

    public virtual ViewportOffset offset
    {
        get => this._offset;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._offset)))
            {
                return;
            }
            if (attached)
            {
                this._offset.removeListener(this._hasScrolled);
            }
            _offset = __value;
            if (attached)
            {
                this._offset.addListener(this._hasScrolled);
            }
            markNeedsLayout();
        }
    }
    public virtual double diameterRatio
    {
        get => this._diameterRatio;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value > 0L));
            if ((__value == this._diameterRatio))
            {
                return;
            }
            _diameterRatio = __value;
            markNeedsPaint();
            markNeedsSemanticsUpdate();
        }
    }
    public virtual double perspective
    {
        get => this._perspective;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value > 0L));
            DartRuntimePrimitives.Assert(() => (__value <= 0.01));
            if ((__value == this._perspective))
            {
                return;
            }
            _perspective = __value;
            markNeedsPaint();
            markNeedsSemanticsUpdate();
        }
    }
    public virtual double offAxisFraction
    {
        get => this._offAxisFraction;
        set
        {
            var __value = value;
            if ((__value == this._offAxisFraction))
            {
                return;
            }
            _offAxisFraction = __value;
            markNeedsPaint();
        }
    }
    public virtual bool useMagnifier
    {
        get => this._useMagnifier;
        set
        {
            var __value = value;
            if ((__value == this._useMagnifier))
            {
                return;
            }
            _useMagnifier = __value;
            markNeedsPaint();
        }
    }
    public virtual double magnification
    {
        get => this._magnification;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value > 0L));
            if ((__value == this._magnification))
            {
                return;
            }
            _magnification = __value;
            markNeedsPaint();
        }
    }
    public virtual double overAndUnderCenterOpacity
    {
        get => this._overAndUnderCenterOpacity;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => ((__value >= 0L) && (__value <= 1L)));
            if ((__value == this._overAndUnderCenterOpacity))
            {
                return;
            }
            _overAndUnderCenterOpacity = __value;
            markNeedsPaint();
        }
    }
    public virtual double itemExtent
    {
        get => this._itemExtent;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value > 0L));
            if ((__value == this._itemExtent))
            {
                return;
            }
            _itemExtent = __value;
            markNeedsLayout();
        }
    }
    public virtual double squeeze
    {
        get => this._squeeze;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value > 0L));
            if ((__value == this._squeeze))
            {
                return;
            }
            _squeeze = __value;
            markNeedsLayout();
            markNeedsSemanticsUpdate();
        }
    }
    public virtual bool renderChildrenOutsideViewport
    {
        get => this._renderChildrenOutsideViewport;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (!this.renderChildrenOutsideViewport || (object.Equals(this.clipBehavior, Clip.none))));
            if ((__value == this._renderChildrenOutsideViewport))
            {
                return;
            }
            _renderChildrenOutsideViewport = __value;
            markNeedsLayout();
            markNeedsSemanticsUpdate();
        }
    }
    public virtual global::Doroti.Ui.Clip clipBehavior
    {
        get => this._clipBehavior;
        set
        {
            var __value = value;
            if ((!object.Equals(__value, this._clipBehavior)))
            {
                _clipBehavior = __value;
                markNeedsPaint();
                markNeedsSemanticsUpdate();
            }
        }
    }
    internal virtual void _hasScrolled()
    {
        markNeedsLayout();
        markNeedsSemanticsUpdate();
    }

    public override void setupParentData(RenderObject child)
    {
        if ((((RenderObject)child).parentData is not ListWheelParentData))
        {
            child.parentData = new ListWheelParentData();
        }
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            child.attach(owner);
            var childParentData = ((ListWheelParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
        this._offset.addListener(this._hasScrolled);
    }

    public override void detach()
    {
        this._offset.removeListener(this._hasScrolled);
        base.detach();
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            child.detach();
            var childParentData = ((ListWheelParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override bool isRepaintBoundary => true;
    internal virtual double _viewportExtent
    {
        get
        {
            DartRuntimePrimitives.Assert(() => hasSize);
            return size.height;
            return default!;
        }
    }
    internal virtual double _minEstimatedScrollExtent
    {
        get
        {
            DartRuntimePrimitives.Assert(() => hasSize);
            if ((((ListWheelChildManager)this.childManager).childCount is null))
            {
                return double.NegativeInfinity;
            }
            return 0.0;
            return default!;
        }
    }
    internal virtual double _maxEstimatedScrollExtent
    {
        get
        {
            DartRuntimePrimitives.Assert(() => hasSize);
            if ((((ListWheelChildManager)this.childManager).childCount is null))
            {
                return double.PositiveInfinity;
            }
            return Math.Max(0.0, (((DartRuntimePrimitives.RequireValue(((ListWheelChildManager)this.childManager).childCount) - 1L)) * this._itemExtent));
            return default!;
        }
    }
    internal virtual double _topScrollMarginExtent
    {
        get
        {
            DartRuntimePrimitives.Assert(() => hasSize);
            return ((-size.height / 2.0) + (this._itemExtent / 2.0));
            return default!;
        }
    }
    internal virtual double _getUntransformedPaintingCoordinateY(double layoutCoordinateY)
    {
        return ((layoutCoordinateY - this._topScrollMarginExtent) - ((ViewportOffset)this.offset).pixels);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _maxVisibleRadian
    {
        get
        {
            if ((this._diameterRatio < 1.0))
            {
                return (Dart_mathLibrary.pi / 2.0);
            }
            return global::Doroti.Runtime.Dart_mathLibrary.asin((1.0 / this._diameterRatio));
            return default!;
        }
    }
    internal virtual double _getIntrinsicCrossAxis(Func<RenderBox, double> childSize)
    {
        var extent = 0.0;
        RenderBox? child = firstChild;
        while ((child is not null))
        {
            extent = Math.Max(extent, childSize(child));
            child = childAfter(child);
        }
        return extent;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        return _getIntrinsicCrossAxis(((Func<RenderBox, double>)((child) => child.getMinIntrinsicWidth(height))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        return _getIntrinsicCrossAxis(((Func<RenderBox, double>)((child) => child.getMaxIntrinsicWidth(height))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        if ((((ListWheelChildManager)this.childManager).childCount is null))
        {
            return 0.0;
        }
        return (DartRuntimePrimitives.RequireValue(((ListWheelChildManager)this.childManager).childCount) * this._itemExtent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        if ((((ListWheelChildManager)this.childManager).childCount is null))
        {
            return 0.0;
        }
        return (DartRuntimePrimitives.RequireValue(((ListWheelChildManager)this.childManager).childCount) * this._itemExtent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool sizedByParent => true;
    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return ((BoxConstraints)constraints).biggest;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long indexOf(RenderBox child)
    {
        var childParentData = ((ListWheelParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (((ListWheelParentData)childParentData).index is not null));
        return DartRuntimePrimitives.RequireValue(((ListWheelParentData)childParentData).index);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long scrollOffsetToIndex(double scrollOffset) => ((scrollOffset / this.itemExtent)).floor();
    public virtual double indexToScrollOffset(long index) => (index * this.itemExtent);
    internal virtual void _createChild(long index, RenderBox? after = null)
    {
        invokeLayoutCallback<BoxConstraints>(((Action<BoxConstraints>)((constraints) =>
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(constraints, this.constraints)));
            this.childManager.createChild(index, after: after);
        })));
    }

    internal virtual void _destroyChild(RenderBox child)
    {
        invokeLayoutCallback<BoxConstraints>(((Action<BoxConstraints>)((constraints) =>
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(constraints, this.constraints)));
            this.childManager.removeChild(child);
        })));
    }

    internal virtual void _layoutChild(RenderBox child, BoxConstraints constraints, long index)
    {
        child.layout(constraints, parentUsesSize: true);
        var childParentData = ((ListWheelParentData?)(object?)child.parentData!)!;
        double crossPosition = ((size.width / 2.0) - (((RenderBox)child).size.width / 2.0));
        childParentData.offset = new global::Doroti.Ui.Offset(crossPosition, indexToScrollOffset(index));
    }

    public override void performLayout()
    {
        this.offset.applyViewportDimension(this._viewportExtent);
        if ((((ListWheelChildManager)this.childManager).childCount is not null))
        {
            this.offset.applyContentDimensions(this._minEstimatedScrollExtent, this._maxEstimatedScrollExtent);
        }
        double visibleHeight = (size.height * this._squeeze);
        if (this.renderChildrenOutsideViewport)
        {
            visibleHeight *= 2L;
        }
        double firstVisibleOffset = ((((ViewportOffset)this.offset).pixels + (this._itemExtent / 2L)) - (visibleHeight / 2L));
        double lastVisibleOffset = (firstVisibleOffset + visibleHeight);
        long targetFirstIndex = scrollOffsetToIndex(firstVisibleOffset);
        long targetLastIndex = scrollOffsetToIndex(lastVisibleOffset);
        if (((targetLastIndex * this._itemExtent) == lastVisibleOffset))
        {
            targetLastIndex--;
        }
        while ((!this.childManager.childExistsAt(targetFirstIndex) && (targetFirstIndex <= targetLastIndex)))
        {
            targetFirstIndex++;
        }
        while ((!this.childManager.childExistsAt(targetLastIndex) && (targetFirstIndex <= targetLastIndex)))
        {
            targetLastIndex--;
        }
        if ((targetFirstIndex > targetLastIndex))
        {
            while ((firstChild is not null))
            {
                _destroyChild(firstChild!);
            }
            return;
        }
        if (((childCount > 0L) && (((indexOf(firstChild!) > targetLastIndex) || (indexOf(lastChild!) < targetFirstIndex)))))
        {
            while ((firstChild is not null))
            {
                _destroyChild(firstChild!);
            }
        }
        BoxConstraints childConstraints = constraints.copyWith(minHeight: this._itemExtent, maxHeight: this._itemExtent, minWidth: 0.0);
        if ((childCount == 0L))
        {
            _createChild(targetFirstIndex);
            _layoutChild(firstChild!, childConstraints, targetFirstIndex);
        }
        long currentFirstIndex = indexOf(firstChild!);
        long currentLastIndex = indexOf(lastChild!);
        while ((currentFirstIndex < targetFirstIndex))
        {
            _destroyChild(firstChild!);
            currentFirstIndex++;
        }
        while ((currentLastIndex > targetLastIndex))
        {
            _destroyChild(lastChild!);
            currentLastIndex--;
        }
        RenderBox? child = firstChild;
        var index = currentFirstIndex;
        while ((child is not null))
        {
            _layoutChild(child, childConstraints, index++);
            child = childAfter(child);
        }
        while ((currentFirstIndex > targetFirstIndex))
        {
            _createChild((currentFirstIndex - 1L));
            _layoutChild(firstChild!, childConstraints, --currentFirstIndex);
        }
        while ((currentLastIndex < targetLastIndex))
        {
            _createChild((currentLastIndex + 1L), after: lastChild);
            _layoutChild(lastChild!, childConstraints, ++currentLastIndex);
        }
        double minScrollExtent = (this.childManager.childExistsAt((targetFirstIndex - 1L)) ? this._minEstimatedScrollExtent : indexToScrollOffset(targetFirstIndex));
        double maxScrollExtent = (this.childManager.childExistsAt((targetLastIndex + 1L)) ? this._maxEstimatedScrollExtent : indexToScrollOffset(targetLastIndex));
        this.offset.applyContentDimensions(minScrollExtent, maxScrollExtent);
    }

    internal virtual bool _shouldClipAtCurrentOffset()
    {
        double highestUntransformedPaintY = _getUntransformedPaintingCoordinateY(0.0);
        return ((highestUntransformedPaintY < 0.0) || (size.height < ((highestUntransformedPaintY + this._maxEstimatedScrollExtent) + this._itemExtent)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if ((childCount > 0L))
        {
            if ((_shouldClipAtCurrentOffset() && (!object.Equals(this.clipBehavior, Clip.none))))
            {
                this._clipRectLayer.layer = context.pushClipRect(needsCompositing, offset, (Offset.zero & size), (Action<PaintingContext, Offset>)this._paintVisibleChildren, clipBehavior: this.clipBehavior, oldLayer: ((LayerHandle<ClipRectLayer>)this._clipRectLayer).layer);
            }
            else
            {
                this._clipRectLayer.layer = null;
                _paintVisibleChildren(context, offset);
            }
        }
    }

    public override void dispose()
    {
        this._clipRectLayer.layer = null;
        this._childOpacityLayerHandler.layer = null;
        base.dispose();
    }

    internal virtual void _paintVisibleChildren(PaintingContext context, Offset offset)
    {
        if ((this.overAndUnderCenterOpacity >= 1L))
        {
            _paintAllChildren(context, offset);
            return;
        }
        this._childOpacityLayerHandler.layer = context.pushOpacity(offset, ((this.overAndUnderCenterOpacity * 255L)).round(), ((Action<PaintingContext, Offset>)((context, offset) =>
        {
            _paintAllChildren(context, offset, center: false);
        })));
        _paintAllChildren(context, offset, center: true);
    }

    internal virtual void _paintAllChildren(PaintingContext context, Offset offset, bool? center = null)
    {
        RenderBox? childToPaint = firstChild;
        while ((childToPaint is not null))
        {
            var childParentData = ((ListWheelParentData?)(object?)childToPaint.parentData!)!;
            _paintTransformedChild(childToPaint, context, offset, childParentData.offset, center: center);
            childToPaint = childAfter(childToPaint);
        }
    }

    internal virtual void _paintTransformedChild(RenderBox child, PaintingContext context, Offset offset, Offset layoutOffset, bool? center)
    {
        global::Doroti.Ui.Offset untransformedPaintingCoordinates = (offset + new global::Doroti.Ui.Offset(layoutOffset.dx, _getUntransformedPaintingCoordinateY(layoutOffset.dy)));
        double fractionalY = (((untransformedPaintingCoordinates.dy + (this._itemExtent / 2.0))) / size.height);
        double angleLocal = (((-((fractionalY - 0.5)) * 2.0) * this._maxVisibleRadian) / this.squeeze);
        if ((((angleLocal > (Dart_mathLibrary.pi / 2.0)) || (angleLocal < (-Dart_mathLibrary.pi / 2.0))) || double.IsNaN(angleLocal)))
        {
            return;
        }
        Matrix4 transform = MatrixUtils.createCylindricalProjectionTransform(radius: ((size.height * this._diameterRatio) / 2.0), angle: angleLocal, perspective: this._perspective);
        var offsetToCenter = new global::Doroti.Ui.Offset(untransformedPaintingCoordinates.dx, -this._topScrollMarginExtent);
        bool shouldApplyOffCenterDim = (this.overAndUnderCenterOpacity < 1L);
        if ((this.useMagnifier || shouldApplyOffCenterDim))
        {
            _paintChildWithMagnifier(context, offset, child, transform, offsetToCenter, untransformedPaintingCoordinates, center: center);
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (center is null));
            _paintChildCylindrically(context, offset, child, transform, offsetToCenter);
        }
    }

    internal virtual void _paintChildWithMagnifier(PaintingContext context, Offset offset, RenderBox child, Matrix4 cylindricalTransform, Offset offsetToCenter, Offset untransformedPaintingCoordinates, bool? center)
    {
        double magnifierTopLinePosition = ((size.height / 2L) - ((this._itemExtent * this._magnification) / 2L));
        double magnifierBottomLinePosition = ((size.height / 2L) + ((this._itemExtent * this._magnification) / 2L));
        bool isAfterMagnifierTopLine = (untransformedPaintingCoordinates.dy >= (magnifierTopLinePosition - (this._itemExtent * this._magnification)));
        bool isBeforeMagnifierBottomLine = (untransformedPaintingCoordinates.dy <= magnifierBottomLinePosition);
        var centerRect = global::Doroti.Ui.Rect.fromLTWH(0.0, magnifierTopLinePosition, size.width, (this._itemExtent * this._magnification));
        var topHalfRect = global::Doroti.Ui.Rect.fromLTWH(0.0, 0.0, size.width, magnifierTopLinePosition);
        var bottomHalfRect = global::Doroti.Ui.Rect.fromLTWH(0.0, magnifierBottomLinePosition, size.width, magnifierTopLinePosition);
        bool inCenter = (isAfterMagnifierTopLine && isBeforeMagnifierBottomLine);
        if (((((center is null) || DartRuntimePrimitives.RequireValue(center))) && inCenter))
        {
            context.pushClipRect(needsCompositing, offset, centerRect, ((Action<PaintingContext, Offset>)((context, offset) =>
            {
                context.pushTransform(needsCompositing, offset, _magnifyTransform(), ((Action<PaintingContext, Offset>)((context, offset) =>
                {
                    context.paintChild(child, (offset + untransformedPaintingCoordinates));
                })));
            })));
        }
        if (((((center is null) || !DartRuntimePrimitives.RequireValue(center))) && inCenter))
        {
            context.pushClipRect(needsCompositing, offset, ((untransformedPaintingCoordinates.dy <= magnifierTopLinePosition) ? topHalfRect : bottomHalfRect), ((Action<PaintingContext, Offset>)((context, offset) =>
            {
                _paintChildCylindrically(context, offset, child, cylindricalTransform, offsetToCenter);
            })));
        }
        if (((((center is null) || !DartRuntimePrimitives.RequireValue(center))) && !inCenter))
        {
            _paintChildCylindrically(context, offset, child, cylindricalTransform, offsetToCenter);
        }
    }

    internal virtual void _paintChildCylindrically(PaintingContext context, Offset offset, RenderBox child, Matrix4 cylindricalTransform, Offset offsetToCenter)
    {
        global::Doroti.Ui.Offset paintOriginOffset = (offset + offsetToCenter);
        void painter(PaintingContext context, Offset offset)
        {
            context.paintChild(child, paintOriginOffset);
        }
        context.pushTransform(needsCompositing, offset, _centerOriginTransform(cylindricalTransform), (Action<PaintingContext, Offset>)painter);
        var childParentData = ((ListWheelParentData?)(object?)child.parentData!)!;
        Matrix4 transformLocal = ((Func<Matrix4>)(() =>
{
    var __cascade = _centerOriginTransform(cylindricalTransform);
    __cascade.translateByDouble(paintOriginOffset.dx, paintOriginOffset.dy, 0, 1);
    return __cascade;
}))();
        childParentData.transform = transformLocal;
    }

    internal virtual Matrix4 _magnifyTransform()
    {
        return ((Func<Matrix4>)(() =>
{
    var __cascade = Matrix4.identity();
    __cascade.translateByDouble((size.width * ((-this._offAxisFraction + 0.5))), (size.height / 2L), 0, 1);
    __cascade.scaleByDouble(this._magnification, this._magnification, this._magnification, 1.0);
    __cascade.translateByDouble((-size.width * ((-this._offAxisFraction + 0.5))), (-size.height / 2L), 0, 1);
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Matrix4 _centerOriginTransform(Matrix4 originalMatrix)
    {
        var result = Matrix4.identity();
        global::Doroti.Ui.Offset centerOriginTranslation = global::Doroti.Framework.Painting.Alignment.center.alongSize(size);
        result.translateByDouble((centerOriginTranslation.dx * (((-this._offAxisFraction * 2L) + 1L))), centerOriginTranslation.dy, 0, 1);
        result.multiply(originalMatrix);
        result.translateByDouble((-centerOriginTranslation.dx * (((-this._offAxisFraction * 2L) + 1L))), -centerOriginTranslation.dy, 0, 1);
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static bool _debugAssertValidHitTestOffsets(string context, Offset offset1, Offset offset2)
    {
        if ((!object.Equals(offset1, offset2)))
        {
            throw new FlutterError($"{context} - hit test expected values didn't match: {offset1} != {offset2}");
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var __child = (RenderBox)(object)child;
        var parentDataLocal = ((ListWheelParentData?)(object?)__child.parentData!)!;
        Matrix4? paintTransform = ((ListWheelParentData)parentDataLocal).transform;
        if ((paintTransform is not null))
        {
            transform.multiply(paintTransform);
        }
    }

    public override Rect? describeApproximatePaintClip(RenderObject child)
    {
        if (_shouldClipAtCurrentOffset())
        {
            return (Offset.zero & size);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child = lastChild;
        while ((child is not null))
        {
            var childParentData = ((ListWheelParentData?)(object?)child.parentData!)!;
            Matrix4? transformLocal = ((ListWheelParentData)childParentData).transform;
            if ((transformLocal is not null))
            {
                bool isHit = result.addWithPaintTransform(transform: transformLocal, position: position, hitTest: ((Func<BoxHitTestResult, Offset, bool>)((result, transformed) =>
                {
                    DartRuntimePrimitives.Assert(() =>
                        {
                            Matrix4? inverted = Matrix4.tryInvert(PointerEvent.removePerspectiveTransform(transformLocal));
                            if ((inverted is null))
                            {
                                return _debugAssertValidHitTestOffsets("Null inverted transform", transformed, position);
                            }
                            return _debugAssertValidHitTestOffsets("MatrixUtils.transformPoint", transformed, MatrixUtils.transformPoint(inverted, position));
                        });
                    return child!.hitTest(result, position: transformed);
                    return default;
                })));
                if (isHit)
                {
                    return true;
                }
            }
            child = childParentData.previousSibling;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RevealedOffset getOffsetToReveal(RenderObject target, double alignment, Rect? rect = null, global::Doroti.Framework.Painting.Axis? axis = null)
    {
        rect ??= ((RenderObject)target).paintBounds;
        var child = target;
        while ((!object.Equals(((RenderObject)child).parent, this)))
        {
            child = ((RenderObject)child).parent!;
        }
        var parentDataLocal = ((ListWheelParentData?)(object?)((RenderObject)child).parentData!)!;
        double targetOffset = parentDataLocal.offset.dy;
        Matrix4 transform = target.getTransformTo(child);
        global::Doroti.Ui.Rect bounds = MatrixUtils.transformRect(transform, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(rect)));
        global::Doroti.Ui.Rect targetRect = bounds.translate(0.0, (((size.height - this.itemExtent)) / 2L));
        return new RevealedOffset(offset: targetOffset, rect: targetRect);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void showOnScreen(RenderObject? descendant = null, Rect? rect = null, Duration duration = default, Curve curve = default!)
    {
        if ((descendant is not null))
        {
            RevealedOffset revealedOffset = getOffsetToReveal(descendant, 0.5, rect: rect);
            if ((object.Equals(duration, Duration.zero)))
            {
                this.offset.jumpTo(((RevealedOffset)revealedOffset).offset);
            }
            else
            {
                _ = this.offset.animateTo(((RevealedOffset)revealedOffset).offset, duration: duration, curve: curve);
            }
            rect = ((RevealedOffset)revealedOffset).rect;
        }
        base.showOnScreen(rect: rect, duration: duration, curve: curve);
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((ListWheelParentData?)(object?)child.parentData!)!;
        while ((childParentData.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData.previousSibling, child)));
            child = childParentData.previousSibling!;
            childParentData = ((ListWheelParentData?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((ListWheelParentData?)(object?)child.parentData!)!;
        while ((childParentData.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData.nextSibling, child)));
            child = childParentData.nextSibling!;
            childParentData = ((ListWheelParentData?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long childCount => this._childCount;
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

    public virtual void _insertIntoChildList(RenderBox child, RenderBox? after = null)
    {
        var childParentData = ((ListWheelParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData = ((ListWheelParentData?)(object?)this._firstChild!.parentData!)!;
                firstChildParentData.previousSibling = child;
            }
            this._firstChild = child;
            this._lastChild ??= child;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (this._firstChild is not null));
            DartRuntimePrimitives.Assert(() => (this._lastChild is not null));
            DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(after, equals: this._firstChild));
            DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(after, equals: this._lastChild));
            var afterParentData = ((ListWheelParentData?)(object?)after.parentData!)!;
            if ((afterParentData.nextSibling is null))
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(after, this._lastChild)));
                childParentData.previousSibling = after;
                afterParentData.nextSibling = child;
                this._lastChild = child;
            }
            else
            {
                childParentData.nextSibling = afterParentData.nextSibling;
                childParentData.previousSibling = after;
                var childPreviousSiblingParentData = ((ListWheelParentData?)(object?)childParentData.previousSibling!.parentData!)!;
                var childNextSiblingParentData = ((ListWheelParentData?)(object?)childParentData.nextSibling!.parentData!)!;
                childPreviousSiblingParentData.nextSibling = child;
                childNextSiblingParentData.previousSibling = child;
                DartRuntimePrimitives.Assert(() => (object.Equals(afterParentData.nextSibling, child)));
            }
        }
    }

    public virtual void insert(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(after, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, after)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this._firstChild)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this._lastChild)));
        adoptChild(child);
        DartRuntimePrimitives.Assert(() => (child.parentData is ListWheelParentData));
        _insertIntoChildList(child, after: after);
    }

    public virtual void add(RenderBox child)
    {
        insert(child, after: this._lastChild);
    }

    public virtual void addAll(List<RenderBox>? children)
    {
        children?.forEach(this.add);
    }

    public virtual void _removeFromChildList(RenderBox child)
    {
        var childParentData = ((ListWheelParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(child, equals: this._firstChild));
        DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(child, equals: this._lastChild));
        DartRuntimePrimitives.Assert(() => (this._childCount >= 0L));
        if ((childParentData.previousSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._firstChild, child)));
            this._firstChild = childParentData.nextSibling;
        }
        else
        {
            var childPreviousSiblingParentData = ((ListWheelParentData?)(object?)childParentData.previousSibling!.parentData!)!;
            childPreviousSiblingParentData.nextSibling = childParentData.nextSibling;
        }
        if ((childParentData.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData.previousSibling;
        }
        else
        {
            var childNextSiblingParentData = ((ListWheelParentData?)(object?)childParentData.nextSibling!.parentData!)!;
            childNextSiblingParentData.previousSibling = childParentData.previousSibling;
        }
        childParentData.previousSibling = null;
        childParentData.nextSibling = null;
        this._childCount -= 1L;
    }

    public virtual void remove(RenderBox child)
    {
        _removeFromChildList(child);
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            var childParentData = ((ListWheelParentData?)(object?)child.parentData!)!;
            RenderBox? next = childParentData.nextSibling;
            childParentData.previousSibling = null;
            childParentData.nextSibling = null;
            dropChild(child);
            child = next;
        }
        this._firstChild = null;
        this._lastChild = null;
        this._childCount = 0L;
    }

    public virtual void move(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(after, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, after)));
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((ListWheelParentData?)(object?)child.parentData!)!;
        if ((object.Equals(childParentData.previousSibling, after)))
        {
            return;
        }
        _removeFromChildList(child);
        _insertIntoChildList(child, after: after);
        markNeedsLayout();
    }

    public override void redepthChildren()
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            redepthChild(child);
            var childParentData = ((ListWheelParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            visitor(child);
            var childParentData = ((ListWheelParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public virtual RenderBox? firstChild => this._firstChild;
    public virtual RenderBox? lastChild => this._lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((ListWheelParentData?)(object?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((ListWheelParentData?)(object?)child.parentData!)!;
        return childParentData.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        var children = new List<DiagnosticsNode>();
        if ((this.firstChild is not null))
        {
            RenderBox child = this.firstChild!;
            var count = 1L;
            while (true)
            {
                children.Add(((Diagnosticable)child).toDiagnosticsNode(name: $"child__183606 {count}"));
                if ((object.Equals(child, this.lastChild)))
                {
                    break;
                }
                count += 1L;
                var childParentData = ((ListWheelParentData?)(object?)child.parentData!)!;
                child = childParentData.nextSibling!;
            }
        }
        return children;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

