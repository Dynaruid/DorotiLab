// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/cupertino/text_selection_toolbar.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Cupertino;

public static partial class Text_selection_toolbarLibrary
{
    internal static Radius _kToolbarBorderRadius = global::Doroti.Ui.Radius.circular(8.0);
}

public static partial class Text_selection_toolbarLibrary
{
    internal static double _kToolbarContentDistance = 8.0;
}

public static partial class Text_selection_toolbarLibrary
{
    internal static Size _kToolbarArrowSize = new global::Doroti.Ui.Size(14.0, 7.0);
}

public static partial class Text_selection_toolbarLibrary
{
    internal static double _kArrowScreenPadding = 26.0;
}

public static partial class Text_selection_toolbarLibrary
{
    internal static double _kToolbarChevronSize = 10.0;
}

public static partial class Text_selection_toolbarLibrary
{
    internal static double _kToolbarChevronThickness = 2.0;
}

public static partial class Text_selection_toolbarLibrary
{
    internal static CupertinoDynamicColor _kToolbarBackgroundColor = new CupertinoDynamicColor(color: new global::Doroti.Ui.Color(4294375158L), darkColor: new global::Doroti.Ui.Color(4280427042L));
}

public static partial class Text_selection_toolbarLibrary
{
    internal static CupertinoDynamicColor _kToolbarDividerColor = new CupertinoDynamicColor(color: new global::Doroti.Ui.Color(4292269782L), darkColor: new global::Doroti.Ui.Color(4282532418L));
}

public static partial class Text_selection_toolbarLibrary
{
    internal static CupertinoDynamicColor _kToolbarTextColor = new CupertinoDynamicColor(color: CupertinoColors.black, darkColor: CupertinoColors.white);
}

public static partial class Text_selection_toolbarLibrary
{
    internal static Duration _kToolbarTransitionDuration = Duration.Create(milliseconds: 125L);
}

public delegate global::Doroti.Generated.Framework.Widgets.Widget CupertinoToolbarBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, Offset anchorAbove, Offset anchorBelow, global::Doroti.Generated.Framework.Widgets.Widget child);

public class CupertinoTextSelectionToolbar : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual Offset anchorAbove { get; private set; } = default!;
    public virtual Offset anchorBelow { get; private set; } = default!;
    public virtual List<global::Doroti.Generated.Framework.Widgets.Widget> children { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, Offset, Offset, global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Widgets.Widget> toolbarBuilder { get; private set; } = default!;
    public const double kToolbarScreenPadding = 8.0;

    public CupertinoTextSelectionToolbar(global::Doroti.Generated.Framework.Foundation.Key? key = null, Offset anchorAbove = default!, Offset anchorBelow = default!, List<global::Doroti.Generated.Framework.Widgets.Widget> children = default!, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, Offset, Offset, global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Widgets.Widget> toolbarBuilder = default!) : base(key: key)
    {
        global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, Offset, Offset, global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Widgets.Widget> __toolbarBuilder = toolbarBuilder ?? _defaultToolbarBuilder;
        this.anchorAbove = anchorAbove;
        this.anchorBelow = anchorBelow;
        this.children = children;
        this.toolbarBuilder = __toolbarBuilder;
        System.Diagnostics.Debug.Assert((checked((long)(children.Count)) > 0L));
    }

    internal static global::Doroti.Generated.Framework.Widgets.Widget _defaultToolbarBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, Offset anchorAbove, Offset anchorBelow, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _CupertinoTextSelectionToolbarShape__text_selection_toolbar(anchorAbove: anchorAbove, anchorBelow: anchorBelow, shadowColor: ((object.Equals(CupertinoTheme.brightnessOf(context), Brightness.light)) ? CupertinoColors.black.withOpacity(0.2) : null), child: new global::Doroti.Generated.Framework.Widgets.ColoredBox(color: Desktop_text_selection_toolbarLibrary._kToolbarBackgroundColor.resolveFrom(context), child: child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        global::Doroti.Generated.Framework.Painting.EdgeInsets mediaQueryPadding__5307 = ((global::Doroti.Generated.Framework.Painting.EdgeInsets)(object?)MediaQuery.paddingOf(context));
        double paddingAbove__5376 = (((global::Doroti.Generated.Framework.Painting.EdgeInsets)mediaQueryPadding__5307).top + kToolbarScreenPadding);
        double leftMargin__5590 = (Text_selectionLibrary._kArrowScreenPadding + ((global::Doroti.Generated.Framework.Painting.EdgeInsets)mediaQueryPadding__5307).left);
        double rightMargin__5667 = ((MediaQuery.widthOf(context) - ((global::Doroti.Generated.Framework.Painting.EdgeInsets)mediaQueryPadding__5307).right) - Text_selectionLibrary._kArrowScreenPadding);
        var anchorAboveAdjusted__5778 = new global::Doroti.Ui.Offset(Dart_uiLibrary.clampDouble(this.anchorAbove.dx, leftMargin__5590, rightMargin__5667), ((this.anchorAbove.dy - Text_selection_toolbarLibrary._kToolbarContentDistance) - paddingAbove__5376));
        var anchorBelowAdjusted__5949 = new global::Doroti.Ui.Offset(Dart_uiLibrary.clampDouble(this.anchorBelow.dx, leftMargin__5590, rightMargin__5667), ((this.anchorBelow.dy + Text_selection_toolbarLibrary._kToolbarContentDistance) - paddingAbove__5376));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Padding(padding: new global::Doroti.Generated.Framework.Painting.EdgeInsets(kToolbarScreenPadding, paddingAbove__5376, kToolbarScreenPadding, kToolbarScreenPadding), child: new global::Doroti.Generated.Framework.Widgets.CustomSingleChildLayout(@delegate: new global::Doroti.Generated.Framework.Widgets.TextSelectionToolbarLayoutDelegate(anchorAbove: anchorAboveAdjusted__5778, anchorBelow: anchorBelowAdjusted__5949), child: new _CupertinoTextSelectionToolbarContent__text_selection_toolbar(anchorAbove: anchorAboveAdjusted__5778, anchorBelow: anchorBelowAdjusted__5949, toolbarBuilder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, Offset, Offset, global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Widgets.Widget>)this.toolbarBuilder, children: this.children))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CupertinoTextSelectionToolbarShape__text_selection_toolbar : global::Doroti.Generated.Framework.Widgets.SingleChildRenderObjectWidget
{
    internal virtual Offset _anchorAbove { get; private set; } = default!;
    internal virtual Offset _anchorBelow { get; private set; } = default!;
    internal virtual Color? _shadowColor { get; private set; }

    internal _CupertinoTextSelectionToolbarShape__text_selection_toolbar(Offset anchorAbove, Offset anchorBelow, Color? shadowColor = null, global::Doroti.Generated.Framework.Widgets.Widget? child = null) : base(child: child)
    {
        this._anchorAbove = anchorAbove;
        this._anchorBelow = anchorBelow;
        this._shadowColor = shadowColor;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(new _RenderCupertinoTextSelectionToolbarShape__text_selection_toolbar(this._anchorAbove, this._anchorBelow, this._shadowColor, null));
    public override void updateRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderCupertinoTextSelectionToolbarShape__text_selection_toolbar)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderCupertinoTextSelectionToolbarShape__text_selection_toolbar>)(() =>
{            var __cascade = __renderObject;
            __cascade.anchorAbove = this._anchorAbove;
            __cascade.anchorBelow = this._anchorBelow;
            __cascade.shadowColor = this._shadowColor;
            return __cascade;        }))());
    }

}

public class _RenderCupertinoTextSelectionToolbarShape__text_selection_toolbar : global::Doroti.Generated.Framework.Rendering.RenderShiftedBox
{
    internal virtual Offset _anchorAbove { get; set; } = default!;
    internal virtual Offset _anchorBelow { get; set; } = default!;
    internal virtual Color? _shadowColor { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Rendering.LayerHandle<global::Doroti.Generated.Framework.Rendering.ClipPathLayer> _clipPathLayer { get; private set; } = new global::Doroti.Generated.Framework.Rendering.LayerHandle<global::Doroti.Generated.Framework.Rendering.ClipPathLayer>();
    internal virtual Paint? _debugPaint { get; set; } = default;

    internal _RenderCupertinoTextSelectionToolbarShape__text_selection_toolbar(Offset _anchorAbove, Offset _anchorBelow, Color? _shadowColor, global::Doroti.Generated.Framework.Rendering.RenderBox? child) : base(child)
    {
        this._anchorAbove = _anchorAbove;
        this._anchorBelow = _anchorBelow;
        this._shadowColor = _shadowColor;
    }

    public override bool isRepaintBoundary => true;
    public virtual global::Doroti.Ui.Offset anchorAbove
    {
        get => this._anchorAbove;
        set
        {
            var __value = value;
            if ((object.Equals(DartRuntimePrimitives.RequireValue(__value), this._anchorAbove)))
            {
                return;
            }
            _anchorAbove = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.Offset anchorBelow
    {
        get => this._anchorBelow;
        set
        {
            var __value = value;
            if ((object.Equals(DartRuntimePrimitives.RequireValue(__value), this._anchorBelow)))
            {
                return;
            }
            _anchorBelow = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.Color? shadowColor
    {
        get => this._shadowColor;
        set
        {
            var __value = value is null ? null : (Color)(object)value;
            if ((object.Equals(__value, this._shadowColor)))
            {
                return;
            }
            _shadowColor = __value;
            markNeedsPaint();
        }
    }
    internal virtual bool _isAbove(double childHeight) => DartRuntimePrimitives.ConvertValue<bool>((this.anchorAbove.dy >= (childHeight - Text_selection_toolbarLibrary._kToolbarArrowSize.height)));
    internal virtual global::Doroti.Generated.Framework.Rendering.BoxConstraints _constraintsForChild(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints)
    {
        return ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)(object?)new global::Doroti.Generated.Framework.Rendering.BoxConstraints(minWidth: (Text_selection_toolbarLibrary._kToolbarArrowSize.width + (Desktop_text_selection_toolbarLibrary._kToolbarBorderRadius.x * 2L))).enforce(constraints.loosen()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Offset _computeChildOffset(Size childSize)
    {
        return new global::Doroti.Ui.Offset(0.0, (_isAbove(childSize.height) ? -Text_selection_toolbarLibrary._kToolbarArrowSize.height : 0.0));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__9665 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((child__9665 is null))
        {
            return null;
        }
        global::Doroti.Generated.Framework.Rendering.BoxConstraints enforcedConstraint__9760 = ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)(object?)_constraintsForChild(constraints));
        double? result__9834 = child__9665.getDryBaseline(enforcedConstraint__9760, baseline);
        return ((result__9834 is null) ? null : (DartRuntimePrimitives.RequireValue(result__9834) + _computeChildOffset(child__9665.getDryLayout(enforcedConstraint__9760)).dy));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__10082 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((child__10082 is null))
        {
            return;
        }
        child__10082.layout(_constraintsForChild(this.constraints), parentUsesSize: true);
        var childParentData__10606 = ((global::Doroti.Generated.Framework.Rendering.BoxParentData?)(object?)child__10082.parentData!)!;
        childParentData__10606.offset = _computeChildOffset(((global::Doroti.Generated.Framework.Rendering.RenderBox)child__10082).size);
        size = new global::Doroti.Ui.Size(((global::Doroti.Generated.Framework.Rendering.RenderBox)child__10082).size.width, (((global::Doroti.Generated.Framework.Rendering.RenderBox)child__10082).size.height - Text_selection_toolbarLibrary._kToolbarArrowSize.height));
    }

    internal virtual global::Doroti.Ui.RRect _shapeRRect(global::Doroti.Generated.Framework.Rendering.RenderBox child)
    {
        global::Doroti.Ui.Rect rect__10921 = ((global::Doroti.Ui.Rect)(object?)(new global::Doroti.Ui.Offset(0.0, Text_selection_toolbarLibrary._kToolbarArrowSize.height) & new global::Doroti.Ui.Size(((global::Doroti.Generated.Framework.Rendering.RenderBox)child).size.width, (((global::Doroti.Generated.Framework.Rendering.RenderBox)child).size.height - (Text_selection_toolbarLibrary._kToolbarArrowSize.height * 2L)))));
        return ((global::Doroti.Ui.RRect)(object?)global::Doroti.Ui.RRect.fromRectAndRadius(rect__10921, Desktop_text_selection_toolbarLibrary._kToolbarBorderRadius).scaleRadii());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Ui.Path _addRRectToPath(Path path, RRect rrect, double startAngle)
    {
        double halfPI__11970 = (Dart_mathLibrary.pi / 2L);
        DartRuntimePrimitives.Assert(() => ((startAngle % halfPI__11970) == 0.0));
        global::Doroti.Ui.Rect rect__12047 = ((global::Doroti.Ui.Rect)(object?)rrect.outerRect);
        var rrectCorners__12082 = new List<(global::Doroti.Ui.Offset, global::Doroti.Ui.Radius)> { (rect__12047.bottomRight, -rrect.brRadius), (rect__12047.bottomLeft, global::Doroti.Ui.Radius.elliptical(rrect.blRadiusX, -rrect.blRadiusY)), (rect__12047.topLeft, rrect.tlRadius), (rect__12047.topRight, global::Doroti.Ui.Radius.elliptical(-rrect.trRadiusX, rrect.trRadiusY)) };
        long startQuadrantIndex__12554 = (checked((long)(startAngle / halfPI__11970)));
        for (var i__12610 = startQuadrantIndex__12554; (i__12610 < (checked((long)(rrectCorners__12082.Count)) + startQuadrantIndex__12554)); i__12610 += 1L)
        {
            var (vertex__12710, rectCenterOffset__12725) = rrectCorners__12082[(int)((i__12610 % checked((long)(rrectCorners__12082.Count))))];
            var otherVertex__12796 = new global::Doroti.Ui.Offset((vertex__12710.dx + (2L * rectCenterOffset__12725.x)), (vertex__12710.dy + (2L * rectCenterOffset__12725.y)));
            var rect__12927 = global::Doroti.Ui.Rect.fromPoints(vertex__12710, otherVertex__12796);
            path.arcTo(rect__12927, (halfPI__11970 * i__12610), halfPI__11970, false);
        }
        return ((global::Doroti.Ui.Path)(object?)path);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Path _clipPath(global::Doroti.Generated.Framework.Rendering.RenderBox child, RRect rrect)
    {
        var path__13179 = new global::Doroti.Ui.Path();
        if ((((Desktop_text_selection_toolbarLibrary._kToolbarBorderRadius.x * 2L) + Text_selection_toolbarLibrary._kToolbarArrowSize.width) > this.size.width))
        {
            return ((global::Doroti.Ui.Path)(object?)((Func<Path>)(() =>
{            var __cascade = path__13179;
            __cascade.addRRect(rrect);
            return __cascade;        }))());
        }
        bool isAbove__13634 = _isAbove(((global::Doroti.Generated.Framework.Rendering.RenderBox)child).size.height);
        global::Doroti.Ui.Offset localAnchor__13690 = ((global::Doroti.Ui.Offset)(object?)globalToLocal((isAbove__13634 ? this._anchorAbove : this._anchorBelow)));
        double arrowTipX__13775 = Dart_uiLibrary.clampDouble(localAnchor__13690.dx, (Desktop_text_selection_toolbarLibrary._kToolbarBorderRadius.x + (Text_selection_toolbarLibrary._kToolbarArrowSize.width / 2L)), ((this.size.width - (Text_selection_toolbarLibrary._kToolbarArrowSize.width / 2L)) - Desktop_text_selection_toolbarLibrary._kToolbarBorderRadius.x));
        if (isAbove__13634)
        {
            double arrowBaseY__14084 = (((global::Doroti.Generated.Framework.Rendering.RenderBox)child).size.height - Text_selection_toolbarLibrary._kToolbarArrowSize.height);
            double arrowTipY__14163 = ((global::Doroti.Generated.Framework.Rendering.RenderBox)child).size.height;
            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{            var __cascade = path__13179;
            __cascade.moveTo((arrowTipX__13775 + (Text_selection_toolbarLibrary._kToolbarArrowSize.width / 2L)), arrowBaseY__14084);
            __cascade.lineTo(arrowTipX__13775, arrowTipY__14163);
            __cascade.lineTo((arrowTipX__13775 - (Text_selection_toolbarLibrary._kToolbarArrowSize.width / 2L)), arrowBaseY__14084);
            return __cascade;        }))());
        }
        else
        {
            double arrowBaseY__14576 = Text_selection_toolbarLibrary._kToolbarArrowSize.height;
            var arrowTipY__14628 = 0.0;
            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{            var __cascade = path__13179;
            __cascade.moveTo((arrowTipX__13775 - (Text_selection_toolbarLibrary._kToolbarArrowSize.width / 2L)), arrowBaseY__14576);
            __cascade.lineTo(arrowTipX__13775, arrowTipY__14628);
            __cascade.lineTo((arrowTipX__13775 + (Text_selection_toolbarLibrary._kToolbarArrowSize.width / 2L)), arrowBaseY__14576);
            return __cascade;        }))());
        }
        double startAngle__15018 = (isAbove__13634 ? (Dart_mathLibrary.pi / 2L) : (-Dart_mathLibrary.pi / 2L));
        return ((global::Doroti.Ui.Path)(object?)((Func<Path>)(() =>
{            var __cascade = _RenderCupertinoTextSelectionToolbarShape__text_selection_toolbar._addRRectToPath(path__13179, rrect, startAngle: startAngle__15018);
            __cascade.close();
            return __cascade;        }))());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__15236 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((child__15236 is null))
        {
            return;
        }
        var childParentData__15312 = ((global::Doroti.Generated.Framework.Rendering.BoxParentData?)(object?)child__15236.parentData!)!;
        global::Doroti.Ui.RRect rrect__15383 = ((global::Doroti.Ui.RRect)(object?)_shapeRRect(child__15236));
        global::Doroti.Ui.Path clipPath__15426 = ((global::Doroti.Ui.Path)(object?)_clipPath(child__15236, rrect__15383));
        if ((this._shadowColor is not null))
        {
            var boxShadow__15565 = new global::Doroti.Generated.Framework.Painting.BoxShadow(color: this._shadowColor!, blurRadius: 15.0);
            global::Doroti.Ui.RRect shadowRRect__15646 = ((global::Doroti.Ui.RRect)(object?)global::Doroti.Ui.RRect.fromLTRBR(rrect__15383.left, rrect__15383.top, rrect__15383.right, (rrect__15383.bottom + Text_selection_toolbarLibrary._kToolbarArrowSize.height), Desktop_text_selection_toolbarLibrary._kToolbarBorderRadius).shift(((offset + ((global::Doroti.Generated.Framework.Rendering.BoxParentData)childParentData__15312).offset) + boxShadow__15565.offset)));
            ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawRRect(shadowRRect__15646, boxShadow__15565.toPaint());
        }
        this._clipPathLayer.layer = context.pushClipPath(this.needsCompositing, (offset + ((global::Doroti.Generated.Framework.Rendering.BoxParentData)childParentData__15312).offset), (Offset.zero & ((global::Doroti.Generated.Framework.Rendering.RenderBox)child__15236).size), clipPath__15426, ((global::System.Action<global::Doroti.Generated.Framework.Rendering.PaintingContext, Offset>)((innerContext, innerOffset) => { innerContext.paintChild(child__15236, innerOffset); })), oldLayer: ((global::Doroti.Generated.Framework.Rendering.LayerHandle<global::Doroti.Generated.Framework.Rendering.ClipPathLayer>)this._clipPathLayer).layer);
    }

    public override void dispose()
    {
        this._clipPathLayer.layer = null;
        base.dispose();
    }

    public override void debugPaintSize(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                global::Doroti.Generated.Framework.Rendering.RenderBox? child__16593 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)((dynamic)this).child);
                if ((child__16593 is null))
                {
                    return true;
                }
                global::Doroti.Ui.Paint debugPaint__16691 = (_debugPaint ??= ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.shader = global::Doroti.Ui.Gradient.linear(Offset.zero, new global::Doroti.Ui.Offset(10.0, 10.0), new List<global::Doroti.Ui.Color> { CupertinoColors.transparent, new global::Doroti.Ui.Color(4294902015L), new global::Doroti.Ui.Color(4294902015L), CupertinoColors.transparent }, new List<double> { 0.25, 0.25, 0.75, 0.75 }, TileMode.repeated);
            __cascade.strokeWidth = 2.0;
            __cascade.style = PaintingStyle.stroke;
            return __cascade;        }))());
                var childParentData__17178 = ((global::Doroti.Generated.Framework.Rendering.BoxParentData?)(object?)child__16593.parentData!)!;
                global::Doroti.Ui.Path clipPath__17249 = ((global::Doroti.Ui.Path)(object?)_clipPath(child__16593, _shapeRRect(child__16593)));
                ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawPath(clipPath__17249.shift((offset + ((global::Doroti.Generated.Framework.Rendering.BoxParentData)childParentData__17178).offset)), debugPaint__16691);
                return true;
            });
    }

    public override bool hitTestChildren(global::Doroti.Generated.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__17535 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((child__17535 is null))
        {
            return false;
        }
        var childParentData__17707 = ((global::Doroti.Generated.Framework.Rendering.BoxParentData?)(object?)child__17535.parentData!)!;
        var hitBox__17771 = global::Doroti.Ui.Rect.fromLTWH(((global::Doroti.Generated.Framework.Rendering.BoxParentData)childParentData__17707).offset.dx, (((global::Doroti.Generated.Framework.Rendering.BoxParentData)childParentData__17707).offset.dy + Text_selection_toolbarLibrary._kToolbarArrowSize.height), ((global::Doroti.Generated.Framework.Rendering.RenderBox)child__17535).size.width, (((global::Doroti.Generated.Framework.Rendering.RenderBox)child__17535).size.height - (Text_selection_toolbarLibrary._kToolbarArrowSize.height * 2L)));
        if (!hitBox__17771.contains(position))
        {
            return false;
        }
        return base.hitTestChildren(result, position: position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _CupertinoTextSelectionToolbarContent__text_selection_toolbar : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual Offset anchorAbove { get; private set; } = default!;
    public virtual Offset anchorBelow { get; private set; } = default!;
    public virtual List<global::Doroti.Generated.Framework.Widgets.Widget> children { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, Offset, Offset, global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Widgets.Widget> toolbarBuilder { get; private set; } = default!;

    internal _CupertinoTextSelectionToolbarContent__text_selection_toolbar(Offset anchorAbove, Offset anchorBelow, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, Offset, Offset, global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Widgets.Widget> toolbarBuilder, List<global::Doroti.Generated.Framework.Widgets.Widget> children)
    {
        this.anchorAbove = anchorAbove;
        this.anchorBelow = anchorBelow;
        this.toolbarBuilder = toolbarBuilder;
        this.children = children;
        System.Diagnostics.Debug.Assert((checked((long)(children.Count)) > 0L));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoTextSelectionToolbarContentState__text_selection_toolbar());
}

public class _CupertinoTextSelectionToolbarContentState__text_selection_toolbar : global::Doroti.Generated.Framework.Widgets.State<_CupertinoTextSelectionToolbarContent__text_selection_toolbar>, global::Doroti.Generated.Framework.Widgets.TickerProviderStateMixin<_CupertinoTextSelectionToolbarContent__text_selection_toolbar>
{
    internal virtual global::Doroti.Generated.Framework.Animation.AnimationController _controller { get; set; } = default!;
    internal virtual long? _nextPage { get; set; } = default;
    internal virtual long _page { get; set; } = 0L;
    internal virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> _toolbarItemsKey { get; private set; } = global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create();
    public virtual HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual void _onHorizontalDragEnd(global::Doroti.Generated.Framework.Gestures.DragEndDetails details)
    {
        double? velocity__19307 = ((global::Doroti.Generated.Framework.Gestures.DragEndDetails)details).primaryVelocity;
        if (((velocity__19307 is not null) && (DartRuntimePrimitives.RequireValue(velocity__19307) != 0L)))
        {
            double velocity__19307__value19352 = DartRuntimePrimitives.RequireValue(velocity__19307);
            if ((DartRuntimePrimitives.RequireValue(velocity__19307__value19352) > 0L))
            {
                _handlePreviousPage();
            }
            else
            {
                _handleNextPage();
            }
        }
    }

    internal virtual void _handleNextPage()
    {
        var renderToolbar__19544 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)((global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>)this._toolbarItemsKey).currentContext?.findRenderObject())!;
        if (((renderToolbar__19544 is _RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar) && ((_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar)((_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar)renderToolbar__19544)).hasNextPage))
        {
            _RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar renderToolbar__19544__as19636 = (_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar)renderToolbar__19544;
            this._controller.reverse();
            this._controller.addStatusListener((AnimationStatusListener)this._statusListener);
            _nextPage = (this._page + 1L);
        }
    }

    internal virtual void _handlePreviousPage()
    {
        var renderToolbar__19891 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)((global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>)this._toolbarItemsKey).currentContext?.findRenderObject())!;
        if (((renderToolbar__19891 is _RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar) && ((_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar)((_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar)renderToolbar__19891)).hasPreviousPage))
        {
            _RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar renderToolbar__19891__as19983 = (_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar)renderToolbar__19891;
            this._controller.reverse();
            this._controller.addStatusListener((AnimationStatusListener)this._statusListener);
            _nextPage = (this._page - 1L);
        }
    }

    internal virtual void _statusListener(global::Doroti.Generated.Framework.Animation.AnimationStatus status)
    {
        if (!global::Doroti.Generated.Framework.Animation.AnimationStatusMembers.isDismissed(status))
        {
            return;
        }
        setState(((global::System.Action)(() => {
_page = DartRuntimePrimitives.RequireValue(this._nextPage);
_nextPage = null;
})));
        this._controller.forward();
        this._controller.removeStatusListener((AnimationStatusListener)this._statusListener);
    }

    public override void initState()
    {
        base.initState();
        _controller = new global::Doroti.Generated.Framework.Animation.AnimationController(value: 1.0, vsync: this, duration: Text_selection_toolbarLibrary._kToolbarTransitionDuration);
    }

    public override void didUpdateWidget(_CupertinoTextSelectionToolbarContent__text_selection_toolbar oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((_CupertinoTextSelectionToolbarContent__text_selection_toolbar)this.widget).children, ((_CupertinoTextSelectionToolbarContent__text_selection_toolbar)oldWidget).children)))
        {
            _page = 0L;
            _nextPage = null;
            this._controller.forward();
            this._controller.removeStatusListener((AnimationStatusListener)this._statusListener);
        }
    }

    public override void dispose()
    {
        this._controller.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Generated.Framework.Scheduler.Ticker ticker__18989 in this._tickers!)
                    {
                        if (((global::Doroti.Generated.Framework.Scheduler.Ticker)ticker__18989).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker__18989.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color chevronColor__21264 = ((global::Doroti.Ui.Color)(object?)Text_selection_toolbarLibrary._kToolbarTextColor.resolveFrom(context));
        global::Doroti.Generated.Framework.Widgets.Widget backButton__21570 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Center(widthFactor: 1.0, heightFactor: 1.0, child: new CupertinoTextSelectionToolbarButton(onPressed: () => this._handlePreviousPage(), child: new global::Doroti.Generated.Framework.Widgets.IgnorePointer(child: new global::Doroti.Generated.Framework.Widgets.CustomPaint(painter: new _LeftCupertinoChevronPainter__text_selection_toolbar(color: chevronColor__21264), size: new global::Doroti.Ui.Size(Text_selection_toolbarLibrary._kToolbarChevronSize))))));
        global::Doroti.Generated.Framework.Widgets.Widget nextButton__21978 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Center(widthFactor: 1.0, heightFactor: 1.0, child: new CupertinoTextSelectionToolbarButton(onPressed: () => this._handleNextPage(), child: new global::Doroti.Generated.Framework.Widgets.IgnorePointer(child: new global::Doroti.Generated.Framework.Widgets.CustomPaint(painter: new _RightCupertinoChevronPainter__text_selection_toolbar(color: chevronColor__21264), size: new global::Doroti.Ui.Size(Text_selection_toolbarLibrary._kToolbarChevronSize))))));
        List<global::Doroti.Generated.Framework.Widgets.Widget> children__22389 = ((_CupertinoTextSelectionToolbarContent__text_selection_toolbar)this.widget).children.map<global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Widgets.Center>(((child) => {
return new global::Doroti.Generated.Framework.Widgets.Center(widthFactor: 1.0, heightFactor: 1.0, child: child);
throw new InvalidOperationException("Dart closure completed without a value.");
})).ToList().Cast<global::Doroti.Generated.Framework.Widgets.Widget>().ToList();
        return this.widget.toolbarBuilder(context, ((_CupertinoTextSelectionToolbarContent__text_selection_toolbar)this.widget).anchorAbove, ((_CupertinoTextSelectionToolbarContent__text_selection_toolbar)this.widget).anchorBelow, new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: this._controller, child: new global::Doroti.Generated.Framework.Widgets.AnimatedSize(duration: Text_selection_toolbarLibrary._kToolbarTransitionDuration, curve: global::Doroti.Generated.Framework.Animation.Curves.decelerate, child: new global::Doroti.Generated.Framework.Widgets.GestureDetector(onHorizontalDragEnd: (global::System.Action<global::Doroti.Generated.Framework.Gestures.DragEndDetails>)this._onHorizontalDragEnd, child: new _CupertinoTextSelectionToolbarItems__text_selection_toolbar(key: this._toolbarItemsKey, page: this._page, backButton: backButton__21570, dividerColor: Text_selection_toolbarLibrary._kToolbarDividerColor.resolveFrom(context), dividerWidth: (1.0 / MediaQuery.devicePixelRatioOf(context)), nextButton: nextButton__21978, children: children__22389)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>();
        TickerModeData values__17506 = this._tickerModeNotifier!.value;
        var result__17553 = ((Func<global::Doroti.Generated.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{            var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
            __cascade.muted = !((TickerModeData)values__17506).enabled;
            __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
            return __cascade;        }))();
        this._tickers!.Add(result__17553);
        return ((global::Doroti.Generated.Framework.Scheduler.Ticker)(object?)result__17553);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Generated.Framework.Widgets._WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => (this._tickers is not null));
        DartRuntimePrimitives.Assert(() => this._tickers!.Contains(ticker));
        this._tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if ((this._tickers is not null))
        {
            TickerModeData values__18318 = this._tickerModeNotifier!.value;
            bool muted__18372 = !((TickerModeData)values__18318).enabled;
            foreach (global::Doroti.Generated.Framework.Scheduler.Ticker ticker__18421 in this._tickers!)
            {
                ticker__18421.muted = muted__18372;
                ticker__18421.forceFrames = ((TickerModeData)values__18318).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__18621 = ((global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__18621, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier__18621.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier__18621;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

internal class _LeftCupertinoChevronPainter__text_selection_toolbar : _CupertinoChevronPainter__text_selection_toolbar
{
    internal _LeftCupertinoChevronPainter__text_selection_toolbar(Color color) : base(color: color, isLeft: true)
    {
    }

}

internal class _RightCupertinoChevronPainter__text_selection_toolbar : _CupertinoChevronPainter__text_selection_toolbar
{
    internal _RightCupertinoChevronPainter__text_selection_toolbar(Color color) : base(color: color, isLeft: false)
    {
    }

}

internal abstract class _CupertinoChevronPainter__text_selection_toolbar : global::Doroti.Generated.Framework.Rendering.CustomPainter
{
    public virtual Color color { get; private set; } = default!;
    public virtual bool isLeft { get; private set; } = default!;

    internal _CupertinoChevronPainter__text_selection_toolbar(Color color, bool isLeft)
    {
        this.color = color;
        this.isLeft = isLeft;
    }

    public override void paint(Canvas canvas, Size size)
    {
        DartRuntimePrimitives.Assert(() => (size.height == size.width), () => (object?)$"size must have the same height and width: {size}");
        double iconSize__24246 = size.height;
        var centerOffset__24618 = new global::Doroti.Ui.Offset(((iconSize__24246 / 4L) * ((this.isLeft ? 1L : -1L))), 0);
        global::Doroti.Ui.Offset firstPoint__24696 = ((global::Doroti.Ui.Offset)(object?)(new global::Doroti.Ui.Offset((iconSize__24246 / 2L), 0) + centerOffset__24618));
        global::Doroti.Ui.Offset middlePoint__24766 = ((global::Doroti.Ui.Offset)(object?)(new global::Doroti.Ui.Offset((this.isLeft ? 0 : iconSize__24246), (iconSize__24246 / 2L)) + centerOffset__24618));
        global::Doroti.Ui.Offset lowerPoint__24857 = ((global::Doroti.Ui.Offset)(object?)(new global::Doroti.Ui.Offset((iconSize__24246 / 2L), iconSize__24246) + centerOffset__24618));
        var paint__24928 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = this.color;
            __cascade.style = PaintingStyle.stroke;
            __cascade.strokeWidth = Text_selection_toolbarLibrary._kToolbarChevronThickness;
            __cascade.strokeCap = StrokeCap.round;
            __cascade.strokeJoin = StrokeJoin.round;
            return __cascade;        }))();
        canvas.drawLine(firstPoint__24696, middlePoint__24766, paint__24928);
        canvas.drawLine(middlePoint__24766, lowerPoint__24857, paint__24928);
    }

    public override bool shouldRepaint(global::Doroti.Generated.Framework.Rendering.CustomPainter oldDelegate) => DartRuntimePrimitives.ConvertValue<bool>(((!object.Equals(((_CupertinoChevronPainter__text_selection_toolbar)oldDelegate).color, this.color)) || (((_CupertinoChevronPainter__text_selection_toolbar)oldDelegate).isLeft != this.isLeft)));
}

public class _CupertinoTextSelectionToolbarItems__text_selection_toolbar : global::Doroti.Generated.Framework.Widgets.RenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget backButton { get; private set; } = default!;
    public virtual List<global::Doroti.Generated.Framework.Widgets.Widget> children { get; private set; } = default!;
    public virtual Color dividerColor { get; private set; } = default!;
    public virtual double dividerWidth { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget nextButton { get; private set; } = default!;
    public virtual long page { get; private set; } = default!;

    internal _CupertinoTextSelectionToolbarItems__text_selection_toolbar(global::Doroti.Generated.Framework.Foundation.Key? key = null, long page = default!, List<global::Doroti.Generated.Framework.Widgets.Widget> children = default!, global::Doroti.Generated.Framework.Widgets.Widget backButton = default!, Color dividerColor = default!, double dividerWidth = default!, global::Doroti.Generated.Framework.Widgets.Widget nextButton = default!) : base(key: key)
    {
        this.page = page;
        this.children = children;
        this.backButton = backButton;
        this.dividerColor = dividerColor;
        this.dividerWidth = dividerWidth;
        this.nextButton = nextButton;
        System.Diagnostics.Debug.Assert(System.Linq.Enumerable.Any(children));
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new _RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar(dividerColor: this.dividerColor, dividerWidth: this.dividerWidth, page: this.page));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar>)(() =>
{            var __cascade = __renderObject;
            __cascade.page = this.page;
            __cascade.dividerColor = this.dividerColor;
            __cascade.dividerWidth = this.dividerWidth;
            return __cascade;        }))());
    }

    public override _CupertinoTextSelectionToolbarItemsElement__text_selection_toolbar createElement() => new _CupertinoTextSelectionToolbarItemsElement__text_selection_toolbar(this);
}

public class _CupertinoTextSelectionToolbarItemsElement__text_selection_toolbar : global::Doroti.Generated.Framework.Widgets.RenderObjectElement
{
    internal virtual List<global::Doroti.Generated.Framework.Widgets.Element> _children { get; set; } = default!;
    public virtual DartMap<_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar, global::Doroti.Generated.Framework.Widgets.Element> slotToChild { get; private set; } = new DartMap<_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar, global::Doroti.Generated.Framework.Widgets.Element>();
    internal virtual HashSet<global::Doroti.Generated.Framework.Widgets.Element> _forgottenChildren { get; private set; } = new HashSet<global::Doroti.Generated.Framework.Widgets.Element>();

    internal _CupertinoTextSelectionToolbarItemsElement__text_selection_toolbar(_CupertinoTextSelectionToolbarItems__text_selection_toolbar widget) : base(widget)
    {
    }

    public override _RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar renderObject => DartRuntimePrimitives.ConvertValue<_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar>(((_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar?)(object?)base.renderObject)!);
    internal virtual void _updateRenderObject(global::Doroti.Generated.Framework.Rendering.RenderBox? child, _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar slot)
    {
        switch (DartRuntimePrimitives.RequireValue(slot))
        {
            case _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar.backButton:
                {
                    this.renderObject.backButton = child;
                    break;
                }
            case _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar.nextButton:
                {
                    this.renderObject.nextButton = child;
                    break;
                }
        }
    }

    public override void insertRenderObjectChild(global::Doroti.Generated.Framework.Rendering.RenderObject child, object? slot)
    {
        if ((slot is _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar))
        {
            _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar slot__as28001 = (_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar)slot;
            DartRuntimePrimitives.Assert(() => (child is global::Doroti.Generated.Framework.Rendering.RenderBox));
            _updateRenderObject(((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)child)!, DartRuntimePrimitives.RequireValue(((_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar)slot__as28001)));
            DartRuntimePrimitives.Assert(() => ((_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar)this.renderObject).slottedChildren.ContainsKey(DartRuntimePrimitives.RequireValue(((_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar)slot__as28001))));
            return;
        }
        if ((slot is global::Doroti.Generated.Framework.Widgets.IndexedSlot<global::Doroti.Generated.Framework.Widgets.Element?>))
        {
            global::Doroti.Generated.Framework.Widgets.IndexedSlot<global::Doroti.Generated.Framework.Widgets.Element?> slot__as28229 = (global::Doroti.Generated.Framework.Widgets.IndexedSlot<global::Doroti.Generated.Framework.Widgets.Element?>)slot;
            DartRuntimePrimitives.Assert(() => ((bool)((dynamic)this.renderObject).debugValidateChild(child)));
            this.renderObject.insert(((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)child)!, after: ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)((global::Doroti.Generated.Framework.Widgets.IndexedSlot<global::Doroti.Generated.Framework.Widgets.Element?>)((global::Doroti.Generated.Framework.Widgets.IndexedSlot<global::Doroti.Generated.Framework.Widgets.Element?>)slot__as28229)).value?.renderObject)!);
            return;
        }
        DartRuntimePrimitives.Assert(() => false, () => (object?)"slot must be _CupertinoTextSelectionToolbarItemsSlot or IndexedSlot");
    }

    public override void moveRenderObjectChild(global::Doroti.Generated.Framework.Rendering.RenderObject child, object? oldSlot, object? newSlot)
    {
        var __oldSlot = (global::Doroti.Generated.Framework.Widgets.IndexedSlot<global::Doroti.Generated.Framework.Widgets.Element>)(object)oldSlot;
        var __newSlot = (global::Doroti.Generated.Framework.Widgets.IndexedSlot<global::Doroti.Generated.Framework.Widgets.Element>)(object)newSlot;
        DartRuntimePrimitives.Assert(() => (object.Equals(((global::Doroti.Generated.Framework.Rendering.RenderObject)child).parent, this.renderObject)));
        ((dynamic)this.renderObject).move(((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)child)!, after: ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)((global::Doroti.Generated.Framework.Widgets.IndexedSlot<global::Doroti.Generated.Framework.Widgets.Element>)__newSlot).value.renderObject)!);
    }

    internal static bool _shouldPaint(global::Doroti.Generated.Framework.Widgets.Element child)
    {
        return (((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)((global::Doroti.Generated.Framework.Rendering.ParentData?)((dynamic)((global::Doroti.Generated.Framework.Widgets.Element)child).renderObject!).parentData)!)!).shouldPaint;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void removeRenderObjectChild(global::Doroti.Generated.Framework.Rendering.RenderObject child, object? slot)
    {
        if ((slot is _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar))
        {
            _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar slot__as29126 = (_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar)slot;
            DartRuntimePrimitives.Assert(() => (child is global::Doroti.Generated.Framework.Rendering.RenderBox));
            DartRuntimePrimitives.Assert(() => ((_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar)this.renderObject).slottedChildren.ContainsKey(DartRuntimePrimitives.RequireValue(((_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar)slot__as29126))));
            _updateRenderObject(null, DartRuntimePrimitives.RequireValue(((_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar)slot__as29126)));
            DartRuntimePrimitives.Assert(() => !((_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar)this.renderObject).slottedChildren.ContainsKey(DartRuntimePrimitives.RequireValue(((_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar)slot__as29126))));
            return;
        }
        DartRuntimePrimitives.Assert(() => (slot is global::Doroti.Generated.Framework.Widgets.IndexedSlot<global::Doroti.Generated.Framework.Widgets.Element?>));
        DartRuntimePrimitives.Assert(() => (object.Equals(((global::Doroti.Generated.Framework.Rendering.RenderObject)child).parent, this.renderObject)));
        ((dynamic)this.renderObject).remove(((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)child)!);
    }

    public override void visitChildren(global::System.Action<global::Doroti.Generated.Framework.Widgets.Element> visitor)
    {
        this.slotToChild.Values.forEach((__arg0) => ((global::System.Action<global::Doroti.Generated.Framework.Widgets.Element>)visitor)(__arg0));
        foreach (global::Doroti.Generated.Framework.Widgets.Element child__29698 in this._children)
        {
            if (!this._forgottenChildren.Contains(child__29698))
            {
                visitor(child__29698);
            }
        }
    }

    public override void forgetChild(global::Doroti.Generated.Framework.Widgets.Element child)
    {
        DartRuntimePrimitives.Assert(() => (this.slotToChild.containsValue(child) || this._children.Contains(child)));
        DartRuntimePrimitives.Assert(() => !this._forgottenChildren.Contains(child));
        if (this.slotToChild.ContainsKey(((_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar)(object)((global::Doroti.Generated.Framework.Widgets.Element)child).slot)))
        {
            var slot__30102 = ((_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar)((global::Doroti.Generated.Framework.Widgets.Element)child).slot!);
            this.slotToChild.remove(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(slot__30102)));
        }
        else
        {
            this._forgottenChildren.Add(child);
        }
        base.forgetChild(child);
    }

    internal virtual void _mountChild(global::Doroti.Generated.Framework.Widgets.Widget widget, _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar slot)
    {
        global::Doroti.Generated.Framework.Widgets.Element? oldChild__30425 = this.slotToChild.GetValueOrDefault(DartRuntimePrimitives.RequireValue(slot));
        global::Doroti.Generated.Framework.Widgets.Element? newChild__30474 = ((global::Doroti.Generated.Framework.Widgets.Element?)(object?)updateChild(oldChild__30425, widget, DartRuntimePrimitives.RequireValue(slot)));
        if ((oldChild__30425 is not null))
        {
            this.slotToChild.remove(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(slot)));
        }
        if ((newChild__30474 is not null))
        {
            this.slotToChild[slot] = newChild__30474;
        }
    }

    public override void mount(global::Doroti.Generated.Framework.Widgets.Element? parent, object? newSlot)
    {
        base.mount(parent, newSlot);
        var toolbarItems__30799 = ((_CupertinoTextSelectionToolbarItems__text_selection_toolbar?)(object?)this.widget)!;
        _mountChild(((_CupertinoTextSelectionToolbarItems__text_selection_toolbar)toolbarItems__30799).backButton, _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar.backButton);
        _mountChild(((_CupertinoTextSelectionToolbarItems__text_selection_toolbar)toolbarItems__30799).nextButton, _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar.nextButton);
        global::Doroti.Generated.Framework.Widgets.Element? previousChild__31091 = default!;
        _children = new List<global::Doroti.Generated.Framework.Widgets.Element>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)checked((long)(((_CupertinoTextSelectionToolbarItems__text_selection_toolbar)toolbarItems__30799).children.Count)))), ((i) => {
global::Doroti.Generated.Framework.Widgets.Element result__31205 = ((global::Doroti.Generated.Framework.Widgets.Element)(object?)inflateWidget(((_CupertinoTextSelectionToolbarItems__text_selection_toolbar)toolbarItems__30799).children[(int)(i)], new global::Doroti.Generated.Framework.Widgets.IndexedSlot<global::Doroti.Generated.Framework.Widgets.Element?>(i, previousChild__31091)));
previousChild__31091 = result__31205;
return result__31205;
throw new InvalidOperationException("Dart closure completed without a value.");
})));
    }

    public override void debugVisitOnstageChildren(global::System.Action<global::Doroti.Generated.Framework.Widgets.Element> visitor)
    {
        foreach (global::Doroti.Generated.Framework.Widgets.Element child__31524 in this.slotToChild.Values)
        {
            if ((_CupertinoTextSelectionToolbarItemsElement__text_selection_toolbar._shouldPaint(child__31524) && !this._forgottenChildren.Contains(child__31524)))
            {
                visitor(child__31524);
            }
        }
        this._children.where(((child) => (!this._forgottenChildren.Contains(child) && _CupertinoTextSelectionToolbarItemsElement__text_selection_toolbar._shouldPaint(child)))).forEach((__arg0) => ((global::System.Action<global::Doroti.Generated.Framework.Widgets.Element>)visitor)(__arg0));
    }

    public override void update(global::Doroti.Generated.Framework.Widgets.Widget newWidget)
    {
        var __newWidget = (_CupertinoTextSelectionToolbarItems__text_selection_toolbar)(object)newWidget;
        base.update(__newWidget);
        DartRuntimePrimitives.Assert(() => (object.Equals(this.widget, __newWidget)));
        var toolbarItems__32013 = ((_CupertinoTextSelectionToolbarItems__text_selection_toolbar?)(object?)this.widget)!;
        _mountChild(((_CupertinoTextSelectionToolbarItems__text_selection_toolbar)toolbarItems__32013).backButton, _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar.backButton);
        _mountChild(((_CupertinoTextSelectionToolbarItems__text_selection_toolbar)toolbarItems__32013).nextButton, _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar.nextButton);
        _children = updateChildren(this._children, ((_CupertinoTextSelectionToolbarItems__text_selection_toolbar)toolbarItems__32013).children, forgottenChildren: this._forgottenChildren);
        this._forgottenChildren.Clear();
    }

}

public class _RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar : global::Doroti.Generated.Framework.Rendering.RenderBox, global::Doroti.Generated.Framework.Rendering.ContainerRenderObjectMixin<global::Doroti.Generated.Framework.Rendering.RenderBox, global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData>, global::Doroti.Generated.Framework.Rendering.RenderBoxContainerDefaultsMixin<global::Doroti.Generated.Framework.Rendering.RenderBox, global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData>
{
    public virtual DartMap<_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar, global::Doroti.Generated.Framework.Rendering.RenderBox> slottedChildren { get; private set; } = new DartMap<_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar, global::Doroti.Generated.Framework.Rendering.RenderBox>();
    public virtual bool hasNextPage { get; set; } = default!;
    public virtual bool hasPreviousPage { get; set; } = default!;
    internal virtual long _page { get; set; } = default!;
    internal virtual Color _dividerColor { get; set; } = default!;
    internal virtual double _dividerWidth { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Rendering.RenderBox? _backButton { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Rendering.RenderBox? _nextButton { get; set; } = default;
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;

    internal _RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar(Color dividerColor, double dividerWidth, long page)
    {
        this._dividerColor = dividerColor;
        this._dividerWidth = dividerWidth;
        this._page = page;
    }

    internal virtual global::Doroti.Generated.Framework.Rendering.RenderBox? _updateChild(global::Doroti.Generated.Framework.Rendering.RenderBox? oldChild, global::Doroti.Generated.Framework.Rendering.RenderBox? newChild, _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar slot)
    {
        if ((oldChild is not null))
        {
            dropChild(oldChild);
            this.slottedChildren.remove(slot);
        }
        if ((newChild is not null))
        {
            this.slottedChildren[slot] = newChild;
            adoptChild(newChild);
        }
        return newChild;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long page
    {
        get => this._page;
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) == this._page))
            {
                return;
            }
            _page = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.Color dividerColor
    {
        get => this._dividerColor;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(__value, this._dividerColor)))
            {
                return;
            }
            _dividerColor = __value;
            markNeedsLayout();
        }
    }
    public virtual double dividerWidth
    {
        get => this._dividerWidth;
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) == this._dividerWidth))
            {
                return;
            }
            _dividerWidth = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Generated.Framework.Rendering.RenderBox? backButton
    {
        get => this._backButton;
        set
        {
            var __value = value;
            _backButton = _updateChild(this._backButton, __value, DartRuntimePrimitives.RequireValue(_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar.backButton));
        }
    }
    public virtual global::Doroti.Generated.Framework.Rendering.RenderBox? nextButton
    {
        get => this._nextButton;
        set
        {
            var __value = value;
            _nextButton = _updateChild(this._nextButton, __value, DartRuntimePrimitives.RequireValue(_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar.nextButton));
        }
    }
    public override void performLayout()
    {
        if ((this.firstChild is null))
        {
            size = ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)this.constraints).smallest;
            return;
        }
        var greatestHeight__34802 = 0.0;
        visitChildren(((global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject>)((renderObjectChild) => {
var child__34889 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)renderObjectChild)!;
double childHeight__34948 = child__34889.getMaxIntrinsicHeight(((global::Doroti.Generated.Framework.Rendering.BoxConstraints)this.constraints).maxWidth);
if ((childHeight__34948 > greatestHeight__34802))
{
    greatestHeight__34802 = childHeight__34948;
}
})));
        var slottedConstraints__35152 = new global::Doroti.Generated.Framework.Rendering.BoxConstraints(maxWidth: ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)this.constraints).maxWidth, minHeight: greatestHeight__34802, maxHeight: greatestHeight__34802);
        this._backButton!.layout(slottedConstraints__35152, parentUsesSize: true);
        this._nextButton!.layout(slottedConstraints__35152, parentUsesSize: true);
        double subsequentPageButtonsWidth__35452 = (this._backButton!.size.width + this._nextButton!.size.width);
        var currentButtonPosition__35540 = 0.0;
        double toolbarWidth__35585 = default!;
        var currentPage__35641 = 0L;
        var i__35666 = -1L;
        visitChildren(((global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject>)((renderObjectChild) => {
i__35666++;
var child__35750 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)renderObjectChild)!;
var childParentData__35802 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child__35750.parentData!)!;
childParentData__35802.shouldPaint = false;
if ((((object.Equals(child__35750, this._backButton)) || (object.Equals(child__35750, this._nextButton))) || (currentPage__35641 > this._page)))
{
    return;
}
double paginationButtonsWidth__36291 = ((currentPage__35641 == 0L) ? ((i__35666 == (this.childCount + 1L)) ? 0.0 : this._nextButton!.size.width) : subsequentPageButtonsWidth__35452);
child__35750.layout(new global::Doroti.Generated.Framework.Rendering.BoxConstraints(maxWidth: (((global::Doroti.Generated.Framework.Rendering.BoxConstraints)this.constraints).maxWidth - paginationButtonsWidth__36291), minHeight: greatestHeight__34802, maxHeight: greatestHeight__34802), parentUsesSize: true);
double currentWidth__36897 = ((currentButtonPosition__35540 + paginationButtonsWidth__36291) + ((global::Doroti.Generated.Framework.Rendering.RenderBox)child__35750).size.width);
if ((currentWidth__36897 > ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)this.constraints).maxWidth))
{
    currentPage__35641++;
    currentButtonPosition__35540 = (this._backButton!.size.width + this.dividerWidth);
    paginationButtonsWidth__36291 = (this._backButton!.size.width + this._nextButton!.size.width);
    child__35750.layout(new global::Doroti.Generated.Framework.Rendering.BoxConstraints(maxWidth: (((global::Doroti.Generated.Framework.Rendering.BoxConstraints)this.constraints).maxWidth - paginationButtonsWidth__36291), minHeight: greatestHeight__34802, maxHeight: greatestHeight__34802), parentUsesSize: true);
}
childParentData__35802.offset = new global::Doroti.Ui.Offset(currentButtonPosition__35540, 0.0);
currentButtonPosition__35540 += (((global::Doroti.Generated.Framework.Rendering.RenderBox)child__35750).size.width + this.dividerWidth);
childParentData__35802.shouldPaint = (currentPage__35641 == this.page);
if ((currentPage__35641 == this.page))
{
    toolbarWidth__35585 = currentButtonPosition__35540;
}
})));
        DartRuntimePrimitives.Assert(() => (this.page <= currentPage__35641));
        if ((currentPage__35641 > 0L))
        {
            var nextButtonParentData__37924 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)this._nextButton!.parentData!)!;
            var backButtonParentData__38011 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)this._backButton!.parentData!)!;
            if ((this.page != currentPage__35641))
            {
                nextButtonParentData__37924.offset = new global::Doroti.Ui.Offset(toolbarWidth__35585, 0.0);
                nextButtonParentData__37924.shouldPaint = true;
                toolbarWidth__35585 += this.nextButton!.size.width;
            }
            if ((this.page > 0L))
            {
                backButtonParentData__38011.offset = Offset.zero;
                backButtonParentData__38011.shouldPaint = true;
            }
        }
        else
        {
            toolbarWidth__35585 -= this.dividerWidth;
        }
        hasNextPage = (this.page != currentPage__35641);
        hasPreviousPage = (this.page > 0L);
        size = this.constraints.constrain(new global::Doroti.Ui.Size(toolbarWidth__35585, greatestHeight__34802));
    }

    public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)
    {
        visitChildren(((global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject>)((renderObjectChild) => {
var child__39227 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)renderObjectChild)!;
var childParentData__39279 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child__39227.parentData!)!;
if (((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData)childParentData__39279).shouldPaint)
{
    global::Doroti.Ui.Offset childOffset__39405 = ((global::Doroti.Ui.Offset)(object?)(childParentData__39279.offset + offset));
    context.paintChild(child__39227, childOffset__39405);
    if (((childParentData__39279.nextSibling is not null) || (object.Equals(child__39227, this.backButton))))
    {
        ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawLine((new global::Doroti.Ui.Offset(((global::Doroti.Generated.Framework.Rendering.RenderBox)child__39227).size.width, 0) + childOffset__39405), (new global::Doroti.Ui.Offset(((global::Doroti.Generated.Framework.Rendering.RenderBox)child__39227).size.width, ((global::Doroti.Generated.Framework.Rendering.RenderBox)child__39227).size.height) + childOffset__39405), ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = this.dividerColor;
            return __cascade;        }))());
    }
}
})));
    }

    public override void setupParentData(global::Doroti.Generated.Framework.Rendering.RenderObject child)
    {
        var __child = (global::Doroti.Generated.Framework.Rendering.RenderBox)(object)child;
        if ((__child.parentData is not global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData))
        {
            __child.parentData = new global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData();
        }
    }

    public static bool hitTestChild(global::Doroti.Generated.Framework.Rendering.RenderBox? child, global::Doroti.Generated.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        if ((child is null))
        {
            return false;
        }
        var childParentData__40481 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
        if (!((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData)childParentData__40481).shouldPaint)
        {
            return false;
        }
        return result.addWithPaintOffset(offset: childParentData__40481.offset, position: position, hitTest: ((global::System.Func<global::Doroti.Generated.Framework.Rendering.BoxHitTestResult, Offset, bool>)((result, transformed) => {
DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - childParentData__40481.offset))));
return child.hitTest(result, position: transformed);
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestChildren(global::Doroti.Generated.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__41059 = this.lastChild;
        while ((child__41059 is not null))
        {
            var childParentData__41118 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child__41059.parentData!)!;
            if (!((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData)childParentData__41118).shouldPaint)
            {
                child__41059 = childParentData__41118.previousSibling;
                continue;
            }
            if (_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar.hitTestChild(child__41059, result, position: position))
            {
                return true;
            }
            child__41059 = childParentData__41118.previousSibling;
        }
        if (_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar.hitTestChild(this.backButton, result, position: position))
        {
            return true;
        }
        if (_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar.hitTestChild(this.nextButton, result, position: position))
        {
            return true;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void attach(global::Doroti.Generated.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__181803 = this._firstChild;
        while ((child__181803 is not null))
        {
            child__181803.attach(owner);
            var childParentData__181891 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child__181803.parentData!)!;
            child__181803 = childParentData__181891.nextSibling;
        }
        foreach (global::Doroti.Generated.Framework.Rendering.RenderBox child__41887 in this.slottedChildren.Values)
        {
            child__41887.attach(owner);
        }
    }

    public override void detach()
    {
        base.detach();
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__182065 = this._firstChild;
        while ((child__182065 is not null))
        {
            child__182065.detach();
            var childParentData__182148 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child__182065.parentData!)!;
            child__182065 = childParentData__182148.nextSibling;
        }
        foreach (global::Doroti.Generated.Framework.Rendering.RenderBox child__42094 in this.slottedChildren.Values)
        {
            child__42094.detach();
        }
    }

    public override void redepthChildren()
    {
        visitChildren(((global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject>)((renderObjectChild) => {
var child__42266 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)renderObjectChild)!;
redepthChild(child__42266);
})));
    }

    public override void visitChildren(global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject> visitor)
    {
        if ((this._backButton is not null))
        {
            visitor(this._backButton!);
        }
        if ((this._nextButton is not null))
        {
            visitor(this._nextButton!);
        }
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__182587 = this._firstChild;
        while ((child__182587 is not null))
        {
            visitor(child__182587);
            var childParentData__182670 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child__182587.parentData!)!;
            child__182587 = childParentData__182670.nextSibling;
        }
    }

    public override void visitChildrenForSemantics(global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject> visitor)
    {
        visitChildren(((global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject>)((renderObjectChild) => {
var child__42842 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)renderObjectChild)!;
var childParentData__42894 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child__42842.parentData!)!;
if (((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData)childParentData__42894).shouldPaint)
{
    visitor(((global::Doroti.Generated.Framework.Rendering.RenderBox)renderObjectChild));
}
})));
    }

    public override List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        var value__43127 = new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>();
        visitChildren(((global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject>)((renderObjectChild) => {
var child__43221 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)renderObjectChild)!;
if ((object.Equals(child__43221, this.backButton)))
{
    value__43127.Add(((Diagnosticable)child__43221).toDiagnosticsNode(name: "back button"));
}
else
{
    if ((object.Equals(child__43221, this.nextButton)))
    {
        value__43127.Add(((Diagnosticable)child__43221).toDiagnosticsNode(name: "next button"));
    }
    else
    {
        value__43127.Add(((Diagnosticable)child__43221).toDiagnosticsNode(name: "menu item"));
    }
}
})));
        return value__43127;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData__173585 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
        while ((childParentData__173585.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173585.previousSibling, child)));
            child = childParentData__173585.previousSibling!;
            childParentData__173585 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData__173981 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
        while ((childParentData__173981.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173981.nextSibling, child)));
            child = childParentData__173981.nextSibling!;
            childParentData__173981 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
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
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"A {this.GetType()} expected a child of type {typeof(RenderBox)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new global::Doroti.Generated.Framework.Foundation.ErrorSpacer(), new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<object?>($"The {this.GetType()} that expected a {typeof(RenderBox)} child was created by", this.debugCreator, style: global::Doroti.Generated.Framework.Foundation.DiagnosticsTreeStyle.errorProperty), new global::Doroti.Generated.Framework.Foundation.ErrorSpacer(), new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", ((RenderObject)child).debugCreator, style: global::Doroti.Generated.Framework.Foundation.DiagnosticsTreeStyle.errorProperty) }));
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _insertIntoChildList(RenderBox child, RenderBox? after = null)
    {
        var childParentData__175971 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData__175971.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData__175971.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData__175971.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData__176343 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)this._firstChild!.parentData!)!;
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
            var afterParentData__176766 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)after.parentData!)!;
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
                var childPreviousSiblingParentData__177424 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)childParentData__175971.previousSibling!.parentData!)!;
                var childNextSiblingParentData__177547 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)childParentData__175971.nextSibling!.parentData!)!;
                childPreviousSiblingParentData__177424.nextSibling = child;
                childNextSiblingParentData__177547.previousSibling = child;
                DartRuntimePrimitives.Assert(() => (object.Equals(afterParentData__176766.nextSibling, child)));
            }
        }
    }

    public virtual void insert(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this)), () => (object?)"A RenderObject cannot be inserted into itself.");
        DartRuntimePrimitives.Assert(() => (!object.Equals(after, this)), () => (object?)"A RenderObject cannot simultaneously be both the parent and the sibling of another RenderObject.");
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, after)), () => (object?)"A RenderObject cannot be inserted after itself.");
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this._firstChild)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this._lastChild)));
        adoptChild(child);
        DartRuntimePrimitives.Assert(() => (child.parentData is global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData), () => (object?)$"A child of {this.GetType()} has parentData of type {DartRuntimePrimitives.RuntimeType(child.parentData)}, " + $"which does not conform to {(typeof(global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData))}. Class using ContainerRenderObjectMixin " + $"should override setupParentData() to set parentData to type {(typeof(global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData))}.");
        _insertIntoChildList(child, after: after);
    }

    public virtual void add(RenderBox child)
    {
        insert(child, after: this._lastChild);
    }

    public virtual void addAll(List<RenderBox>? children)
    {
        children?.forEach((__arg0) => ((global::System.Action<RenderBox>)this.add)(__arg0));
    }

    public virtual void _removeFromChildList(RenderBox child)
    {
        var childParentData__179226 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
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
            var childPreviousSiblingParentData__179613 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)childParentData__179226.previousSibling!.parentData!)!;
            childPreviousSiblingParentData__179613.nextSibling = childParentData__179226.nextSibling;
        }
        if ((childParentData__179226.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData__179226.previousSibling;
        }
        else
        {
            var childNextSiblingParentData__179965 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)childParentData__179226.nextSibling!.parentData!)!;
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
            var childParentData__180684 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child__180623.parentData!)!;
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
        var childParentData__181479 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
        if ((object.Equals(childParentData__181479.previousSibling, after)))
        {
            return;
        }
        _removeFromChildList(child);
        _insertIntoChildList(child, after: after);
        markNeedsLayout();
    }

    public virtual RenderBox? firstChild => this._firstChild;
    public virtual RenderBox? lastChild => this._lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183103 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
        return childParentData__183103.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183356 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
        return childParentData__183356.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToFirstActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !this.debugNeedsLayout);
        RenderBox? child__138717 = this.firstChild;
        while ((child__138717 is not null))
        {
            var childParentData__138777 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child__138717.parentData!)!;
            double? result__138852 = child__138717.getDistanceToActualBaseline(baseline);
            if ((result__138852 is not null))
            {
                double result__138852__value138916 = DartRuntimePrimitives.RequireValue(result__138852);
                return (DartRuntimePrimitives.RequireValue(result__138852__value138916) + childParentData__138777.offset.dy);
            }
            child__138717 = childParentData__138777.nextSibling;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToHighestActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !this.debugNeedsLayout);
        BaselineOffset minBaseline__139372 = BaselineOffset.noBaseline;
        RenderBox? child__139428 = this.firstChild;
        while ((child__139428 is not null))
        {
            var childParentData__139488 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child__139428.parentData!)!;
            BaselineOffset candidate__139570 = (new BaselineOffset(child__139428.getDistanceToActualBaseline(baseline)).op_Add(childParentData__139488.offset.dy));
            minBaseline__139372 = minBaseline__139372.minOf(candidate__139570);
            child__139428 = childParentData__139488.nextSibling;
        }
        return minBaseline__139372.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool defaultHitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child__140279 = this.lastChild;
        while ((child__140279 is not null))
        {
            var childParentData__140418 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child__140279.parentData!)!;
            bool isHit__140490 = result.addWithPaintOffset(offset: childParentData__140418.offset, position: position, hitTest: ((global::System.Func<BoxHitTestResult, Offset, bool>)((result, transformed) => {
DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - childParentData__140418.offset))));
return child__140279!.hitTest(result, position: transformed);
throw new InvalidOperationException("Dart closure completed without a value.");
})));
            if (isHit__140490)
            {
                return true;
            }
            child__140279 = childParentData__140418.previousSibling;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void defaultPaint(PaintingContext context, Offset offset)
    {
        RenderBox? child__141240 = this.firstChild;
        while ((child__141240 is not null))
        {
            var childParentData__141300 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child__141240.parentData!)!;
            context.paintChild(child__141240, (childParentData__141300.offset + offset));
            child__141240 = childParentData__141300.nextSibling;
        }
    }

    public virtual List<RenderBox> getChildrenAsList()
    {
        var result__141793 = new List<RenderBox>();
        RenderBox? child__141832 = this.firstChild;
        while ((child__141832 is not null))
        {
            var childParentData__141892 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child__141832.parentData!)!;
            result__141793.Add(((RenderBox?)(object?)child__141832)!);
            child__141832 = childParentData__141892.nextSibling;
        }
        return result__141793;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public enum _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar
{
    backButton,
    nextButton
}
