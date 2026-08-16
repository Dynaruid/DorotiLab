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
        RenderBox? child__181803 = this._firstChild;
        while ((child__181803 is not null))
        {
            child__181803.attach(owner);
            var childParentData__181891 = ((ListWheelParentData?)(object?)child__181803.parentData!)!;
            child__181803 = childParentData__181891.nextSibling;
        }
        this._offset.addListener(this._hasScrolled);
    }

    public override void detach()
    {
        this._offset.removeListener(this._hasScrolled);
        base.detach();
        RenderBox? child__182065 = this._firstChild;
        while ((child__182065 is not null))
        {
            child__182065.detach();
            var childParentData__182148 = ((ListWheelParentData?)(object?)child__182065.parentData!)!;
            child__182065 = childParentData__182148.nextSibling;
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
        var extent__21784 = 0.0;
        RenderBox? child__21813 = firstChild;
        while ((child__21813 is not null))
        {
            extent__21784 = Math.Max(extent__21784, childSize(child__21813));
            child__21813 = childAfter(child__21813);
        }
        return extent__21784;
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
        var childParentData__23034 = ((ListWheelParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (((ListWheelParentData)childParentData__23034).index is not null));
        return DartRuntimePrimitives.RequireValue(((ListWheelParentData)childParentData__23034).index);
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
        var childParentData__24044 = ((ListWheelParentData?)(object?)child.parentData!)!;
        double crossPosition__24160 = ((size.width / 2.0) - (((RenderBox)child).size.width / 2.0));
        childParentData__24044.offset = new global::Doroti.Ui.Offset(crossPosition__24160, indexToScrollOffset(index));
    }

    public override void performLayout()
    {
        this.offset.applyViewportDimension(this._viewportExtent);
        if ((((ListWheelChildManager)this.childManager).childCount is not null))
        {
            this.offset.applyContentDimensions(this._minEstimatedScrollExtent, this._maxEstimatedScrollExtent);
        }
        double visibleHeight__25494 = (size.height * this._squeeze);
        if (this.renderChildrenOutsideViewport)
        {
            visibleHeight__25494 *= 2L;
        }
        double firstVisibleOffset__25818 = ((((ViewportOffset)this.offset).pixels + (this._itemExtent / 2L)) - (visibleHeight__25494 / 2L));
        double lastVisibleOffset__25909 = (firstVisibleOffset__25818 + visibleHeight__25494);
        long targetFirstIndex__26118 = scrollOffsetToIndex(firstVisibleOffset__25818);
        long targetLastIndex__26186 = scrollOffsetToIndex(lastVisibleOffset__25909);
        if (((targetLastIndex__26186 * this._itemExtent) == lastVisibleOffset__25909))
        {
            targetLastIndex__26186--;
        }
        while ((!this.childManager.childExistsAt(targetFirstIndex__26118) && (targetFirstIndex__26118 <= targetLastIndex__26186)))
        {
            targetFirstIndex__26118++;
        }
        while ((!this.childManager.childExistsAt(targetLastIndex__26186) && (targetFirstIndex__26118 <= targetLastIndex__26186)))
        {
            targetLastIndex__26186--;
        }
        if ((targetFirstIndex__26118 > targetLastIndex__26186))
        {
            while ((firstChild is not null))
            {
                _destroyChild(firstChild!);
            }
            return;
        }
        if (((childCount > 0L) && (((indexOf(firstChild!) > targetLastIndex__26186) || (indexOf(lastChild!) < targetFirstIndex__26118)))))
        {
            while ((firstChild is not null))
            {
                _destroyChild(firstChild!);
            }
        }
        BoxConstraints childConstraints__27719 = constraints.copyWith(minHeight: this._itemExtent, maxHeight: this._itemExtent, minWidth: 0.0);
        if ((childCount == 0L))
        {
            _createChild(targetFirstIndex__26118);
            _layoutChild(firstChild!, childConstraints__27719, targetFirstIndex__26118);
        }
        long currentFirstIndex__28093 = indexOf(firstChild!);
        long currentLastIndex__28143 = indexOf(lastChild!);
        while ((currentFirstIndex__28093 < targetFirstIndex__26118))
        {
            _destroyChild(firstChild!);
            currentFirstIndex__28093++;
        }
        while ((currentLastIndex__28143 > targetLastIndex__26186))
        {
            _destroyChild(lastChild!);
            currentLastIndex__28143--;
        }
        RenderBox? child__28573 = firstChild;
        var index__28601 = currentFirstIndex__28093;
        while ((child__28573 is not null))
        {
            _layoutChild(child__28573, childConstraints__27719, index__28601++);
            child__28573 = childAfter(child__28573);
        }
        while ((currentFirstIndex__28093 > targetFirstIndex__26118))
        {
            _createChild((currentFirstIndex__28093 - 1L));
            _layoutChild(firstChild!, childConstraints__27719, --currentFirstIndex__28093);
        }
        while ((currentLastIndex__28143 < targetLastIndex__26186))
        {
            _createChild((currentLastIndex__28143 + 1L), after: lastChild);
            _layoutChild(lastChild!, childConstraints__27719, ++currentLastIndex__28143);
        }
        double minScrollExtent__29535 = (this.childManager.childExistsAt((targetFirstIndex__26118 - 1L)) ? this._minEstimatedScrollExtent : indexToScrollOffset(targetFirstIndex__26118));
        double maxScrollExtent__29704 = (this.childManager.childExistsAt((targetLastIndex__26186 + 1L)) ? this._maxEstimatedScrollExtent : indexToScrollOffset(targetLastIndex__26186));
        this.offset.applyContentDimensions(minScrollExtent__29535, maxScrollExtent__29704);
    }

    internal virtual bool _shouldClipAtCurrentOffset()
    {
        double highestUntransformedPaintY__29983 = _getUntransformedPaintingCoordinateY(0.0);
        return ((highestUntransformedPaintY__29983 < 0.0) || (size.height < ((highestUntransformedPaintY__29983 + this._maxEstimatedScrollExtent) + this._itemExtent)));
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
        RenderBox? childToPaint__31912 = firstChild;
        while ((childToPaint__31912 is not null))
        {
            var childParentData__31986 = ((ListWheelParentData?)(object?)childToPaint__31912.parentData!)!;
            _paintTransformedChild(childToPaint__31912, context, offset, childParentData__31986.offset, center: center);
            childToPaint__31912 = childAfter(childToPaint__31912);
        }
    }

    internal virtual void _paintTransformedChild(RenderBox child, PaintingContext context, Offset offset, Offset layoutOffset, bool? center)
    {
        global::Doroti.Ui.Offset untransformedPaintingCoordinates__32680 = (offset + new global::Doroti.Ui.Offset(layoutOffset.dx, _getUntransformedPaintingCoordinateY(layoutOffset.dy)));
        double fractionalY__32896 = (((untransformedPaintingCoordinates__32680.dy + (this._itemExtent / 2.0))) / size.height);
        double angle__33008 = (((-((fractionalY__32896 - 0.5)) * 2.0) * this._maxVisibleRadian) / this.squeeze);
        if ((((angle__33008 > (Dart_mathLibrary.pi / 2.0)) || (angle__33008 < (-Dart_mathLibrary.pi / 2.0))) || double.IsNaN(angle__33008)))
        {
            return;
        }
        Matrix4 transform__33408 = MatrixUtils.createCylindricalProjectionTransform(radius: ((size.height * this._diameterRatio) / 2.0), angle: angle__33008, perspective: this._perspective);
        var offsetToCenter__33668 = new global::Doroti.Ui.Offset(untransformedPaintingCoordinates__32680.dx, -this._topScrollMarginExtent);
        bool shouldApplyOffCenterDim__33771 = (this.overAndUnderCenterOpacity < 1L);
        if ((this.useMagnifier || shouldApplyOffCenterDim__33771))
        {
            _paintChildWithMagnifier(context, offset, child, transform__33408, offsetToCenter__33668, untransformedPaintingCoordinates__32680, center: center);
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (center is null));
            _paintChildCylindrically(context, offset, child, transform__33408, offsetToCenter__33668);
        }
    }

    internal virtual void _paintChildWithMagnifier(PaintingContext context, Offset offset, RenderBox child, Matrix4 cylindricalTransform, Offset offsetToCenter, Offset untransformedPaintingCoordinates, bool? center)
    {
        double magnifierTopLinePosition__35216 = ((size.height / 2L) - ((this._itemExtent * this._magnification) / 2L));
        double magnifierBottomLinePosition__35312 = ((size.height / 2L) + ((this._itemExtent * this._magnification) / 2L));
        bool isAfterMagnifierTopLine__35410 = (untransformedPaintingCoordinates.dy >= (magnifierTopLinePosition__35216 - (this._itemExtent * this._magnification)));
        bool isBeforeMagnifierBottomLine__35563 = (untransformedPaintingCoordinates.dy <= magnifierBottomLinePosition__35312);
        var centerRect__35680 = global::Doroti.Ui.Rect.fromLTWH(0.0, magnifierTopLinePosition__35216, size.width, (this._itemExtent * this._magnification));
        var topHalfRect__35822 = global::Doroti.Ui.Rect.fromLTWH(0.0, 0.0, size.width, magnifierTopLinePosition__35216);
        var bottomHalfRect__35909 = global::Doroti.Ui.Rect.fromLTWH(0.0, magnifierBottomLinePosition__35312, size.width, magnifierTopLinePosition__35216);
        bool inCenter__36117 = (isAfterMagnifierTopLine__35410 && isBeforeMagnifierBottomLine__35563);
        if (((((center is null) || DartRuntimePrimitives.RequireValue(center))) && inCenter__36117))
        {
            context.pushClipRect(needsCompositing, offset, centerRect__35680, ((Action<PaintingContext, Offset>)((context, offset) =>
            {
                context.pushTransform(needsCompositing, offset, _magnifyTransform(), ((Action<PaintingContext, Offset>)((context, offset) =>
                {
                    context.paintChild(child, (offset + untransformedPaintingCoordinates));
                })));
            })));
        }
        if (((((center is null) || !DartRuntimePrimitives.RequireValue(center))) && inCenter__36117))
        {
            context.pushClipRect(needsCompositing, offset, ((untransformedPaintingCoordinates.dy <= magnifierTopLinePosition__35216) ? topHalfRect__35822 : bottomHalfRect__35909), ((Action<PaintingContext, Offset>)((context, offset) =>
            {
                _paintChildCylindrically(context, offset, child, cylindricalTransform, offsetToCenter);
            })));
        }
        if (((((center is null) || !DartRuntimePrimitives.RequireValue(center))) && !inCenter__36117))
        {
            _paintChildCylindrically(context, offset, child, cylindricalTransform, offsetToCenter);
        }
    }

    internal virtual void _paintChildCylindrically(PaintingContext context, Offset offset, RenderBox child, Matrix4 cylindricalTransform, Offset offsetToCenter)
    {
        global::Doroti.Ui.Offset paintOriginOffset__37569 = (offset + offsetToCenter);
        void painter(PaintingContext context, Offset offset)
        {
            context.paintChild(child, paintOriginOffset__37569);
        }
        context.pushTransform(needsCompositing, offset, _centerOriginTransform(cylindricalTransform), (Action<PaintingContext, Offset>)painter);
        var childParentData__38097 = ((ListWheelParentData?)(object?)child.parentData!)!;
        Matrix4 transform__38268 = ((Func<Matrix4>)(() =>
{
    var __cascade = _centerOriginTransform(cylindricalTransform);
    __cascade.translateByDouble(paintOriginOffset__37569.dx, paintOriginOffset__37569.dy, 0, 1);
    return __cascade;
}))();
        childParentData__38097.transform = transform__38268;
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
        var result__39096 = Matrix4.identity();
        global::Doroti.Ui.Offset centerOriginTranslation__39142 = global::Doroti.Framework.Painting.Alignment.center.alongSize(size);
        result__39096.translateByDouble((centerOriginTranslation__39142.dx * (((-this._offAxisFraction * 2L) + 1L))), centerOriginTranslation__39142.dy, 0, 1);
        result__39096.multiply(originalMatrix);
        result__39096.translateByDouble((-centerOriginTranslation__39142.dx * (((-this._offAxisFraction * 2L) + 1L))), -centerOriginTranslation__39142.dy, 0, 1);
        return result__39096;
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
        var parentData__39912 = ((ListWheelParentData?)(object?)__child.parentData!)!;
        Matrix4? paintTransform__39986 = ((ListWheelParentData)parentData__39912).transform;
        if ((paintTransform__39986 is not null))
        {
            transform.multiply(paintTransform__39986);
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
        RenderBox? child__40389 = lastChild;
        while ((child__40389 is not null))
        {
            var childParentData__40448 = ((ListWheelParentData?)(object?)child__40389.parentData!)!;
            Matrix4? transform__40529 = ((ListWheelParentData)childParentData__40448).transform;
            if ((transform__40529 is not null))
            {
                bool isHit__40653 = result.addWithPaintTransform(transform: transform__40529, position: position, hitTest: ((Func<BoxHitTestResult, Offset, bool>)((result, transformed) =>
                {
                    DartRuntimePrimitives.Assert(() =>
                        {
                            Matrix4? inverted__40873 = Matrix4.tryInvert(PointerEvent.removePerspectiveTransform(transform__40529));
                            if ((inverted__40873 is null))
                            {
                                return _debugAssertValidHitTestOffsets("Null inverted transform", transformed, position);
                            }
                            return _debugAssertValidHitTestOffsets("MatrixUtils.transformPoint", transformed, MatrixUtils.transformPoint(inverted__40873, position));
                        });
                    return child__40389!.hitTest(result, position: transformed);
                    return default;
                })));
                if (isHit__40653)
                {
                    return true;
                }
            }
            child__40389 = childParentData__40448.previousSibling;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RevealedOffset getOffsetToReveal(RenderObject target, double alignment, Rect? rect = null, global::Doroti.Framework.Painting.Axis? axis = null)
    {
        rect ??= ((RenderObject)target).paintBounds;
        var child__42246 = target;
        while ((!object.Equals(((RenderObject)child__42246).parent, this)))
        {
            child__42246 = ((RenderObject)child__42246).parent!;
        }
        var parentData__42343 = ((ListWheelParentData?)(object?)((RenderObject)child__42246).parentData!)!;
        double targetOffset__42415 = parentData__42343.offset.dy;
        Matrix4 transform__42505 = target.getTransformTo(child__42246);
        global::Doroti.Ui.Rect bounds__42562 = MatrixUtils.transformRect(transform__42505, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(rect)));
        global::Doroti.Ui.Rect targetRect__42630 = bounds__42562.translate(0.0, (((size.height - this.itemExtent)) / 2L));
        return new RevealedOffset(offset: targetOffset__42415, rect: targetRect__42630);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void showOnScreen(RenderObject? descendant = null, Rect? rect = null, Duration duration = default, Curve curve = default!)
    {
        if ((descendant is not null))
        {
            RevealedOffset revealedOffset__43048 = getOffsetToReveal(descendant, 0.5, rect: rect);
            if ((object.Equals(duration, Duration.zero)))
            {
                this.offset.jumpTo(((RevealedOffset)revealedOffset__43048).offset);
            }
            else
            {
                _ = this.offset.animateTo(((RevealedOffset)revealedOffset__43048).offset, duration: duration, curve: curve);
            }
            rect = ((RevealedOffset)revealedOffset__43048).rect;
        }
        base.showOnScreen(rect: rect, duration: duration, curve: curve);
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData__173585 = ((ListWheelParentData?)(object?)child.parentData!)!;
        while ((childParentData__173585.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173585.previousSibling, child)));
            child = childParentData__173585.previousSibling!;
            childParentData__173585 = ((ListWheelParentData?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData__173981 = ((ListWheelParentData?)(object?)child.parentData!)!;
        while ((childParentData__173981.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173981.nextSibling, child)));
            child = childParentData__173981.nextSibling!;
            childParentData__173981 = ((ListWheelParentData?)(object?)child.parentData!)!;
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
        var childParentData__175971 = ((ListWheelParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData__175971.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData__175971.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData__175971.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData__176343 = ((ListWheelParentData?)(object?)this._firstChild!.parentData!)!;
                firstChildParentData__176343.previousSibling = child;
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
            var afterParentData__176766 = ((ListWheelParentData?)(object?)after.parentData!)!;
            if ((afterParentData__176766.nextSibling is null))
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(after, this._lastChild)));
                childParentData__175971.previousSibling = after;
                afterParentData__176766.nextSibling = child;
                this._lastChild = child;
            }
            else
            {
                childParentData__175971.nextSibling = afterParentData__176766.nextSibling;
                childParentData__175971.previousSibling = after;
                var childPreviousSiblingParentData__177424 = ((ListWheelParentData?)(object?)childParentData__175971.previousSibling!.parentData!)!;
                var childNextSiblingParentData__177547 = ((ListWheelParentData?)(object?)childParentData__175971.nextSibling!.parentData!)!;
                childPreviousSiblingParentData__177424.nextSibling = child;
                childNextSiblingParentData__177547.previousSibling = child;
                DartRuntimePrimitives.Assert(() => (object.Equals(afterParentData__176766.nextSibling, child)));
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
        var childParentData__179226 = ((ListWheelParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(child, equals: this._firstChild));
        DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(child, equals: this._lastChild));
        DartRuntimePrimitives.Assert(() => (this._childCount >= 0L));
        if ((childParentData__179226.previousSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._firstChild, child)));
            this._firstChild = childParentData__179226.nextSibling;
        }
        else
        {
            var childPreviousSiblingParentData__179613 = ((ListWheelParentData?)(object?)childParentData__179226.previousSibling!.parentData!)!;
            childPreviousSiblingParentData__179613.nextSibling = childParentData__179226.nextSibling;
        }
        if ((childParentData__179226.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData__179226.previousSibling;
        }
        else
        {
            var childNextSiblingParentData__179965 = ((ListWheelParentData?)(object?)childParentData__179226.nextSibling!.parentData!)!;
            childNextSiblingParentData__179965.previousSibling = childParentData__179226.previousSibling;
        }
        childParentData__179226.previousSibling = null;
        childParentData__179226.nextSibling = null;
        this._childCount -= 1L;
    }

    public virtual void remove(RenderBox child)
    {
        _removeFromChildList(child);
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderBox? child__180623 = this._firstChild;
        while ((child__180623 is not null))
        {
            var childParentData__180684 = ((ListWheelParentData?)(object?)child__180623.parentData!)!;
            RenderBox? next__180762 = childParentData__180684.nextSibling;
            childParentData__180684.previousSibling = null;
            childParentData__180684.nextSibling = null;
            dropChild(child__180623);
            child__180623 = next__180762;
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
        var childParentData__181479 = ((ListWheelParentData?)(object?)child.parentData!)!;
        if ((object.Equals(childParentData__181479.previousSibling, after)))
        {
            return;
        }
        _removeFromChildList(child);
        _insertIntoChildList(child, after: after);
        markNeedsLayout();
    }

    public override void redepthChildren()
    {
        RenderBox? child__182311 = this._firstChild;
        while ((child__182311 is not null))
        {
            redepthChild(child__182311);
            var childParentData__182399 = ((ListWheelParentData?)(object?)child__182311.parentData!)!;
            child__182311 = childParentData__182399.nextSibling;
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderBox? child__182587 = this._firstChild;
        while ((child__182587 is not null))
        {
            visitor(child__182587);
            var childParentData__182670 = ((ListWheelParentData?)(object?)child__182587.parentData!)!;
            child__182587 = childParentData__182670.nextSibling;
        }
    }

    public virtual RenderBox? firstChild => this._firstChild;
    public virtual RenderBox? lastChild => this._lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183103 = ((ListWheelParentData?)(object?)child.parentData!)!;
        return childParentData__183103.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183356 = ((ListWheelParentData?)(object?)child.parentData!)!;
        return childParentData__183356.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        var children__183528 = new List<DiagnosticsNode>();
        if ((this.firstChild is not null))
        {
            RenderBox child__183606 = this.firstChild!;
            var count__183637 = 1L;
            while (true)
            {
                children__183528.Add(((Diagnosticable)child__183606).toDiagnosticsNode(name: $"child__183606 {count__183637}"));
                if ((object.Equals(child__183606, this.lastChild)))
                {
                    break;
                }
                count__183637 += 1L;
                var childParentData__183833 = ((ListWheelParentData?)(object?)child__183606.parentData!)!;
                child__183606 = childParentData__183833.nextSibling!;
            }
        }
        return children__183528;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

