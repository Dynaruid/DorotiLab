// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/text_selection_toolbar.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Cupertino;

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

public delegate global::Doroti.Framework.Widgets.Widget CupertinoToolbarBuilder(global::Doroti.Framework.Widgets.BuildContext context, Offset anchorAbove, Offset anchorBelow, global::Doroti.Framework.Widgets.Widget child);

public class CupertinoTextSelectionToolbar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual Offset anchorAbove { get; private set; } = default!;
    public virtual Offset anchorBelow { get; private set; } = default!;
    public virtual List<global::Doroti.Framework.Widgets.Widget> children { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, Offset, Offset, global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Widgets.Widget> toolbarBuilder { get; private set; } = default!;
    public const double kToolbarScreenPadding = 8.0;

    public CupertinoTextSelectionToolbar(global::Doroti.Framework.Foundation.Key? key = null, Offset anchorAbove = default!, Offset anchorBelow = default!, List<global::Doroti.Framework.Widgets.Widget> children = default!, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, Offset, Offset, global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Widgets.Widget> toolbarBuilder = default!) : base(key: key)
    {
        global::System.Func<global::Doroti.Framework.Widgets.BuildContext, Offset, Offset, global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Widgets.Widget> __toolbarBuilder = toolbarBuilder ?? _defaultToolbarBuilder;
        this.anchorAbove = anchorAbove;
        this.anchorBelow = anchorBelow;
        this.children = children;
        this.toolbarBuilder = __toolbarBuilder;
        System.Diagnostics.Debug.Assert((checked((long)(children.Count)) > 0L));
    }

    internal static global::Doroti.Framework.Widgets.Widget _defaultToolbarBuilder(global::Doroti.Framework.Widgets.BuildContext context, Offset anchorAbove, Offset anchorBelow, global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _CupertinoTextSelectionToolbarShape__text_selection_toolbar(anchorAbove: anchorAbove, anchorBelow: anchorBelow, shadowColor: ((object.Equals(CupertinoTheme.brightnessOf(context), Brightness.light)) ? CupertinoColors.black.withOpacity(0.2) : null), child: new global::Doroti.Framework.Widgets.ColoredBox(color: Desktop_text_selection_toolbarLibrary._kToolbarBackgroundColor.resolveFrom(context), child: child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        global::Doroti.Framework.Painting.EdgeInsets mediaQueryPadding = ((global::Doroti.Framework.Painting.EdgeInsets)(object?)MediaQuery.paddingOf(context));
        double paddingAbove = (((global::Doroti.Framework.Painting.EdgeInsets)mediaQueryPadding).top + kToolbarScreenPadding);
        double leftMargin = (Text_selectionLibrary._kArrowScreenPadding + ((global::Doroti.Framework.Painting.EdgeInsets)mediaQueryPadding).left);
        double rightMargin = ((MediaQuery.widthOf(context) - ((global::Doroti.Framework.Painting.EdgeInsets)mediaQueryPadding).right) - Text_selectionLibrary._kArrowScreenPadding);
        var anchorAboveAdjusted = new global::Doroti.Ui.Offset(Dart_uiLibrary.clampDouble(this.anchorAbove.dx, leftMargin, rightMargin), ((this.anchorAbove.dy - Text_selection_toolbarLibrary._kToolbarContentDistance) - paddingAbove));
        var anchorBelowAdjusted = new global::Doroti.Ui.Offset(Dart_uiLibrary.clampDouble(this.anchorBelow.dx, leftMargin, rightMargin), ((this.anchorBelow.dy + Text_selection_toolbarLibrary._kToolbarContentDistance) - paddingAbove));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Padding(padding: new global::Doroti.Framework.Painting.EdgeInsets(kToolbarScreenPadding, paddingAbove, kToolbarScreenPadding, kToolbarScreenPadding), child: new global::Doroti.Framework.Widgets.CustomSingleChildLayout(@delegate: new global::Doroti.Framework.Widgets.TextSelectionToolbarLayoutDelegate(anchorAbove: anchorAboveAdjusted, anchorBelow: anchorBelowAdjusted), child: new _CupertinoTextSelectionToolbarContent__text_selection_toolbar(anchorAbove: anchorAboveAdjusted, anchorBelow: anchorBelowAdjusted, toolbarBuilder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, Offset, Offset, global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Widgets.Widget>)this.toolbarBuilder, children: this.children))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CupertinoTextSelectionToolbarShape__text_selection_toolbar : global::Doroti.Framework.Widgets.SingleChildRenderObjectWidget
{
    internal virtual Offset _anchorAbove { get; private set; } = default!;
    internal virtual Offset _anchorBelow { get; private set; } = default!;
    internal virtual Color? _shadowColor { get; private set; }

    internal _CupertinoTextSelectionToolbarShape__text_selection_toolbar(Offset anchorAbove, Offset anchorBelow, Color? shadowColor = null, global::Doroti.Framework.Widgets.Widget? child = null) : base(child: child)
    {
        this._anchorAbove = anchorAbove;
        this._anchorBelow = anchorBelow;
        this._shadowColor = shadowColor;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(new _RenderCupertinoTextSelectionToolbarShape__text_selection_toolbar(this._anchorAbove, this._anchorBelow, this._shadowColor, null));
    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderCupertinoTextSelectionToolbarShape__text_selection_toolbar)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderCupertinoTextSelectionToolbarShape__text_selection_toolbar>)(() =>
{
    var __cascade = __renderObject;
    __cascade.anchorAbove = this._anchorAbove;
    __cascade.anchorBelow = this._anchorBelow;
    __cascade.shadowColor = this._shadowColor;
    return __cascade;
}))());
    }

}

public class _RenderCupertinoTextSelectionToolbarShape__text_selection_toolbar : global::Doroti.Framework.Rendering.RenderShiftedBox
{
    internal virtual Offset _anchorAbove { get; set; } = default!;
    internal virtual Offset _anchorBelow { get; set; } = default!;
    internal virtual Color? _shadowColor { get; set; } = default;
    internal virtual global::Doroti.Framework.Rendering.LayerHandle<global::Doroti.Framework.Rendering.ClipPathLayer> _clipPathLayer { get; private set; } = new global::Doroti.Framework.Rendering.LayerHandle<global::Doroti.Framework.Rendering.ClipPathLayer>();
    internal virtual Paint? _debugPaint { get; set; } = default;

    internal _RenderCupertinoTextSelectionToolbarShape__text_selection_toolbar(Offset _anchorAbove, Offset _anchorBelow, Color? _shadowColor, global::Doroti.Framework.Rendering.RenderBox? child) : base(child)
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
    internal virtual global::Doroti.Framework.Rendering.BoxConstraints _constraintsForChild(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: (Text_selection_toolbarLibrary._kToolbarArrowSize.width + (Desktop_text_selection_toolbarLibrary._kToolbarBorderRadius.x * 2L))).enforce(constraints.loosen()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Offset _computeChildOffset(Size childSize)
    {
        return new global::Doroti.Ui.Offset(0.0, (_isAbove(childSize.height) ? -Text_selection_toolbarLibrary._kToolbarArrowSize.height : 0.0));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((childLocal is null))
        {
            return null;
        }
        global::Doroti.Framework.Rendering.BoxConstraints enforcedConstraint = ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)_constraintsForChild(constraints));
        double? result = childLocal.getDryBaseline(enforcedConstraint, baseline);
        return ((result is null) ? null : (DartRuntimePrimitives.RequireValue(result) + _computeChildOffset(childLocal.getDryLayout(enforcedConstraint)).dy));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((childLocal is null))
        {
            return;
        }
        childLocal.layout(_constraintsForChild(this.constraints), parentUsesSize: true);
        var childParentData = ((global::Doroti.Framework.Rendering.BoxParentData?)(object?)childLocal.parentData!)!;
        childParentData.offset = _computeChildOffset(((global::Doroti.Framework.Rendering.RenderBox)childLocal).size);
        size = new global::Doroti.Ui.Size(((global::Doroti.Framework.Rendering.RenderBox)childLocal).size.width, (((global::Doroti.Framework.Rendering.RenderBox)childLocal).size.height - Text_selection_toolbarLibrary._kToolbarArrowSize.height));
    }

    internal virtual global::Doroti.Ui.RRect _shapeRRect(global::Doroti.Framework.Rendering.RenderBox child)
    {
        global::Doroti.Ui.Rect rect = ((global::Doroti.Ui.Rect)(object?)(new global::Doroti.Ui.Offset(0.0, Text_selection_toolbarLibrary._kToolbarArrowSize.height) & new global::Doroti.Ui.Size(((global::Doroti.Framework.Rendering.RenderBox)child).size.width, (((global::Doroti.Framework.Rendering.RenderBox)child).size.height - (Text_selection_toolbarLibrary._kToolbarArrowSize.height * 2L)))));
        return ((global::Doroti.Ui.RRect)(object?)global::Doroti.Ui.RRect.fromRectAndRadius(rect, Desktop_text_selection_toolbarLibrary._kToolbarBorderRadius).scaleRadii());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Ui.Path _addRRectToPath(Path path, RRect rrect, double startAngle)
    {
        double halfPI = (Dart_mathLibrary.pi / 2L);
        DartRuntimePrimitives.Assert(() => ((startAngle % halfPI) == 0.0));
        global::Doroti.Ui.Rect rect = ((global::Doroti.Ui.Rect)(object?)rrect.outerRect);
        var rrectCorners = new List<(global::Doroti.Ui.Offset, global::Doroti.Ui.Radius)> { (rect.bottomRight, -rrect.brRadius), (rect.bottomLeft, global::Doroti.Ui.Radius.elliptical(rrect.blRadiusX, -rrect.blRadiusY)), (rect.topLeft, rrect.tlRadius), (rect.topRight, global::Doroti.Ui.Radius.elliptical(-rrect.trRadiusX, rrect.trRadiusY)) };
        long startQuadrantIndex = (checked((long)(startAngle / halfPI)));
        for (var i = startQuadrantIndex; (i < (checked((long)(rrectCorners.Count)) + startQuadrantIndex)); i += 1L)
        {
            var (vertex, rectCenterOffset) = rrectCorners[(int)((i % checked((long)(rrectCorners.Count))))];
            var otherVertex = new global::Doroti.Ui.Offset((vertex.dx + (2L * rectCenterOffset.x)), (vertex.dy + (2L * rectCenterOffset.y)));
            var rectLocal = global::Doroti.Ui.Rect.fromPoints(vertex, otherVertex);
            path.arcTo(rectLocal, (halfPI * i), halfPI, false);
        }
        return ((global::Doroti.Ui.Path)(object?)path);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Path _clipPath(global::Doroti.Framework.Rendering.RenderBox child, RRect rrect)
    {
        var path = new global::Doroti.Ui.Path();
        if ((((Desktop_text_selection_toolbarLibrary._kToolbarBorderRadius.x * 2L) + Text_selection_toolbarLibrary._kToolbarArrowSize.width) > this.size.width))
        {
            return ((global::Doroti.Ui.Path)(object?)((Func<Path>)(() =>
{
    var __cascade = path;
    __cascade.addRRect(rrect);
    return __cascade;
}))());
        }
        bool isAbove = _isAbove(((global::Doroti.Framework.Rendering.RenderBox)child).size.height);
        global::Doroti.Ui.Offset localAnchor = ((global::Doroti.Ui.Offset)(object?)globalToLocal((isAbove ? this._anchorAbove : this._anchorBelow)));
        double arrowTipX = Dart_uiLibrary.clampDouble(localAnchor.dx, (Desktop_text_selection_toolbarLibrary._kToolbarBorderRadius.x + (Text_selection_toolbarLibrary._kToolbarArrowSize.width / 2L)), ((this.size.width - (Text_selection_toolbarLibrary._kToolbarArrowSize.width / 2L)) - Desktop_text_selection_toolbarLibrary._kToolbarBorderRadius.x));
        if (isAbove)
        {
            double arrowBaseY = (((global::Doroti.Framework.Rendering.RenderBox)child).size.height - Text_selection_toolbarLibrary._kToolbarArrowSize.height);
            double arrowTipY = ((global::Doroti.Framework.Rendering.RenderBox)child).size.height;
            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = path;
    __cascade.moveTo((arrowTipX + (Text_selection_toolbarLibrary._kToolbarArrowSize.width / 2L)), arrowBaseY);
    __cascade.lineTo(arrowTipX, arrowTipY);
    __cascade.lineTo((arrowTipX - (Text_selection_toolbarLibrary._kToolbarArrowSize.width / 2L)), arrowBaseY);
    return __cascade;
}))());
        }
        else
        {
            double arrowBaseYLocal = Text_selection_toolbarLibrary._kToolbarArrowSize.height;
            var arrowTipYLocal = 0.0;
            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = path;
    __cascade.moveTo((arrowTipX - (Text_selection_toolbarLibrary._kToolbarArrowSize.width / 2L)), arrowBaseYLocal);
    __cascade.lineTo(arrowTipX, arrowTipYLocal);
    __cascade.lineTo((arrowTipX + (Text_selection_toolbarLibrary._kToolbarArrowSize.width / 2L)), arrowBaseYLocal);
    return __cascade;
}))());
        }
        double startAngleLocal = (isAbove ? (Dart_mathLibrary.pi / 2L) : (-Dart_mathLibrary.pi / 2L));
        return ((global::Doroti.Ui.Path)(object?)((Func<Path>)(() =>
{
    var __cascade = _RenderCupertinoTextSelectionToolbarShape__text_selection_toolbar._addRRectToPath(path, rrect, startAngle: startAngleLocal);
    __cascade.close();
    return __cascade;
}))());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((childLocal is null))
        {
            return;
        }
        var childParentData = ((global::Doroti.Framework.Rendering.BoxParentData?)(object?)childLocal.parentData!)!;
        global::Doroti.Ui.RRect rrect = ((global::Doroti.Ui.RRect)(object?)_shapeRRect(childLocal));
        global::Doroti.Ui.Path clipPath = ((global::Doroti.Ui.Path)(object?)_clipPath(childLocal, rrect));
        if ((this._shadowColor is not null))
        {
            var boxShadow = new global::Doroti.Framework.Painting.BoxShadow(color: this._shadowColor!, blurRadius: 15.0);
            global::Doroti.Ui.RRect shadowRRect = ((global::Doroti.Ui.RRect)(object?)global::Doroti.Ui.RRect.fromLTRBR(rrect.left, rrect.top, rrect.right, (rrect.bottom + Text_selection_toolbarLibrary._kToolbarArrowSize.height), Desktop_text_selection_toolbarLibrary._kToolbarBorderRadius).shift(((offset + ((global::Doroti.Framework.Rendering.BoxParentData)childParentData).offset) + boxShadow.offset)));
            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRRect(shadowRRect, boxShadow.toPaint());
        }
        this._clipPathLayer.layer = context.pushClipPath(this.needsCompositing, (offset + ((global::Doroti.Framework.Rendering.BoxParentData)childParentData).offset), (Offset.zero & ((global::Doroti.Framework.Rendering.RenderBox)childLocal).size), clipPath, ((global::System.Action<global::Doroti.Framework.Rendering.PaintingContext, Offset>)((innerContext, innerOffset) => { innerContext.paintChild(childLocal, innerOffset); })), oldLayer: ((global::Doroti.Framework.Rendering.LayerHandle<global::Doroti.Framework.Rendering.ClipPathLayer>)this._clipPathLayer).layer);
    }

    public override void dispose()
    {
        this._clipPathLayer.layer = null;
        base.dispose();
    }

    public override void debugPaintSize(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                global::Doroti.Framework.Rendering.RenderBox? childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
                if ((childLocal is null))
                {
                    return true;
                }
                global::Doroti.Ui.Paint debugPaint = (_debugPaint ??= ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.shader = global::Doroti.Ui.Gradient.linear(Offset.zero, new global::Doroti.Ui.Offset(10.0, 10.0), new List<global::Doroti.Ui.Color> { CupertinoColors.transparent, new global::Doroti.Ui.Color(4294902015L), new global::Doroti.Ui.Color(4294902015L), CupertinoColors.transparent }, new List<double> { 0.25, 0.25, 0.75, 0.75 }, TileMode.repeated);
    __cascade.strokeWidth = 2.0;
    __cascade.style = PaintingStyle.stroke;
    return __cascade;
}))());
                var childParentData = ((global::Doroti.Framework.Rendering.BoxParentData?)(object?)childLocal.parentData!)!;
                global::Doroti.Ui.Path clipPath = ((global::Doroti.Ui.Path)(object?)_clipPath(childLocal, _shapeRRect(childLocal)));
                ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawPath(clipPath.shift((offset + ((global::Doroti.Framework.Rendering.BoxParentData)childParentData).offset)), debugPaint);
                return true;
            });
    }

    public override bool hitTestChildren(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((childLocal is null))
        {
            return false;
        }
        var childParentData = ((global::Doroti.Framework.Rendering.BoxParentData?)(object?)childLocal.parentData!)!;
        var hitBox = global::Doroti.Ui.Rect.fromLTWH(((global::Doroti.Framework.Rendering.BoxParentData)childParentData).offset.dx, (((global::Doroti.Framework.Rendering.BoxParentData)childParentData).offset.dy + Text_selection_toolbarLibrary._kToolbarArrowSize.height), ((global::Doroti.Framework.Rendering.RenderBox)childLocal).size.width, (((global::Doroti.Framework.Rendering.RenderBox)childLocal).size.height - (Text_selection_toolbarLibrary._kToolbarArrowSize.height * 2L)));
        if (!hitBox.contains(position))
        {
            return false;
        }
        return base.hitTestChildren(result, position: position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _CupertinoTextSelectionToolbarContent__text_selection_toolbar : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual Offset anchorAbove { get; private set; } = default!;
    public virtual Offset anchorBelow { get; private set; } = default!;
    public virtual List<global::Doroti.Framework.Widgets.Widget> children { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, Offset, Offset, global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Widgets.Widget> toolbarBuilder { get; private set; } = default!;

    internal _CupertinoTextSelectionToolbarContent__text_selection_toolbar(Offset anchorAbove, Offset anchorBelow, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, Offset, Offset, global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Widgets.Widget> toolbarBuilder, List<global::Doroti.Framework.Widgets.Widget> children)
    {
        this.anchorAbove = anchorAbove;
        this.anchorBelow = anchorBelow;
        this.toolbarBuilder = toolbarBuilder;
        this.children = children;
        System.Diagnostics.Debug.Assert((checked((long)(children.Count)) > 0L));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoTextSelectionToolbarContentState__text_selection_toolbar());
}

public class _CupertinoTextSelectionToolbarContentState__text_selection_toolbar : global::Doroti.Framework.Widgets.State<_CupertinoTextSelectionToolbarContent__text_selection_toolbar>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<_CupertinoTextSelectionToolbarContent__text_selection_toolbar>
{
    internal virtual global::Doroti.Framework.Animation.AnimationController _controller { get; set; } = default!;
    internal virtual long? _nextPage { get; set; } = default;
    internal virtual long _page { get; set; } = 0L;
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _toolbarItemsKey { get; private set; } = global::Doroti.Framework.Widgets.GlobalKey<IState>.Create();
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual void _onHorizontalDragEnd(global::Doroti.Framework.Gestures.DragEndDetails details)
    {
        double? velocity = ((global::Doroti.Framework.Gestures.DragEndDetails)details).primaryVelocity;
        if (((velocity is not null) && (DartRuntimePrimitives.RequireValue(velocity) != 0L)))
        {
            double velocity__19307__value19352 = DartRuntimePrimitives.RequireValue(velocity);
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
        var renderToolbar = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((global::Doroti.Framework.Widgets.GlobalKey<IState>)this._toolbarItemsKey).currentContext?.findRenderObject())!;
        if (((renderToolbar is _RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar) && ((_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar)((_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar)renderToolbar)).hasNextPage))
        {
            _RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar renderToolbar__19544__as19636 = (_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar)renderToolbar;
            this._controller.reverse();
            this._controller.addStatusListener((AnimationStatusListener)this._statusListener);
            _nextPage = (this._page + 1L);
        }
    }

    internal virtual void _handlePreviousPage()
    {
        var renderToolbar = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((global::Doroti.Framework.Widgets.GlobalKey<IState>)this._toolbarItemsKey).currentContext?.findRenderObject())!;
        if (((renderToolbar is _RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar) && ((_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar)((_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar)renderToolbar)).hasPreviousPage))
        {
            _RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar renderToolbar__19891__as19983 = (_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar)renderToolbar;
            this._controller.reverse();
            this._controller.addStatusListener((AnimationStatusListener)this._statusListener);
            _nextPage = (this._page - 1L);
        }
    }

    internal virtual void _statusListener(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        if (!global::Doroti.Framework.Animation.AnimationStatusMembers.isDismissed(status))
        {
            return;
        }
        setState(((global::System.Action)(() =>
        {
            _page = DartRuntimePrimitives.RequireValue(this._nextPage);
            _nextPage = null;
        })));
        this._controller.forward();
        this._controller.removeStatusListener((AnimationStatusListener)this._statusListener);
    }

    public override void initState()
    {
        base.initState();
        _controller = new global::Doroti.Framework.Animation.AnimationController(value: 1.0, vsync: this, duration: Text_selection_toolbarLibrary._kToolbarTransitionDuration);
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
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        this._tickerModeNotifier?.removeListener(this._updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color chevronColor = ((global::Doroti.Ui.Color)(object?)Text_selection_toolbarLibrary._kToolbarTextColor.resolveFrom(context));
        global::Doroti.Framework.Widgets.Widget backButtonLocal = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Center(widthFactor: 1.0, heightFactor: 1.0, child: new CupertinoTextSelectionToolbarButton(onPressed: () => this._handlePreviousPage(), child: new global::Doroti.Framework.Widgets.IgnorePointer(child: new global::Doroti.Framework.Widgets.CustomPaint(painter: new _LeftCupertinoChevronPainter__text_selection_toolbar(color: chevronColor), size: new global::Doroti.Ui.Size(Text_selection_toolbarLibrary._kToolbarChevronSize))))));
        global::Doroti.Framework.Widgets.Widget nextButtonLocal = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Center(widthFactor: 1.0, heightFactor: 1.0, child: new CupertinoTextSelectionToolbarButton(onPressed: () => this._handleNextPage(), child: new global::Doroti.Framework.Widgets.IgnorePointer(child: new global::Doroti.Framework.Widgets.CustomPaint(painter: new _RightCupertinoChevronPainter__text_selection_toolbar(color: chevronColor), size: new global::Doroti.Ui.Size(Text_selection_toolbarLibrary._kToolbarChevronSize))))));
        List<global::Doroti.Framework.Widgets.Widget> childrenLocal = ((_CupertinoTextSelectionToolbarContent__text_selection_toolbar)this.widget).children.map<global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Widgets.Center>(((child) =>
        {
            return new global::Doroti.Framework.Widgets.Center(widthFactor: 1.0, heightFactor: 1.0, child: child);
            throw new InvalidOperationException("Dart closure completed without a value.");
        })).ToList().Cast<global::Doroti.Framework.Widgets.Widget>().ToList();
        return this.widget.toolbarBuilder(context, ((_CupertinoTextSelectionToolbarContent__text_selection_toolbar)this.widget).anchorAbove, ((_CupertinoTextSelectionToolbarContent__text_selection_toolbar)this.widget).anchorBelow, new global::Doroti.Framework.Widgets.FadeTransition(opacity: this._controller, child: new global::Doroti.Framework.Widgets.AnimatedSize(duration: Text_selection_toolbarLibrary._kToolbarTransitionDuration, curve: global::Doroti.Framework.Animation.Curves.decelerate, child: new global::Doroti.Framework.Widgets.GestureDetector(onHorizontalDragEnd: (global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>)this._onHorizontalDragEnd, child: new _CupertinoTextSelectionToolbarItems__text_selection_toolbar(key: this._toolbarItemsKey, page: this._page, backButton: backButtonLocal, dividerColor: Text_selection_toolbarLibrary._kToolbarDividerColor.resolveFrom(context), dividerWidth: (1.0 / MediaQuery.devicePixelRatioOf(context)), nextButton: nextButtonLocal, children: childrenLocal)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = this._tickerModeNotifier!.value;
        var result = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
    __cascade.muted = !((TickerModeData)values).enabled;
    __cascade.forceFrames = ((TickerModeData)values).forceFrames;
    return __cascade;
}))();
        this._tickers!.Add(result);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider ticker)
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
            TickerModeData values = this._tickerModeNotifier!.value;
            bool mutedLocal = !((TickerModeData)values).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = ((TickerModeData)values).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(this._updateTickers);
        newNotifier.addListener(this._updateTickers);
        this._tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
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

internal abstract class _CupertinoChevronPainter__text_selection_toolbar : global::Doroti.Framework.Rendering.CustomPainter
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
        double iconSize = size.height;
        var centerOffset = new global::Doroti.Ui.Offset(((iconSize / 4L) * ((this.isLeft ? 1L : -1L))), 0);
        global::Doroti.Ui.Offset firstPoint = ((global::Doroti.Ui.Offset)(object?)(new global::Doroti.Ui.Offset((iconSize / 2L), 0) + centerOffset));
        global::Doroti.Ui.Offset middlePoint = ((global::Doroti.Ui.Offset)(object?)(new global::Doroti.Ui.Offset((this.isLeft ? 0 : iconSize), (iconSize / 2L)) + centerOffset));
        global::Doroti.Ui.Offset lowerPoint = ((global::Doroti.Ui.Offset)(object?)(new global::Doroti.Ui.Offset((iconSize / 2L), iconSize) + centerOffset));
        var paintLocal = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = this.color;
    __cascade.style = PaintingStyle.stroke;
    __cascade.strokeWidth = Text_selection_toolbarLibrary._kToolbarChevronThickness;
    __cascade.strokeCap = StrokeCap.round;
    __cascade.strokeJoin = StrokeJoin.round;
    return __cascade;
}))();
        canvas.drawLine(firstPoint, middlePoint, paintLocal);
        canvas.drawLine(middlePoint, lowerPoint, paintLocal);
    }

    public override bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate) => DartRuntimePrimitives.ConvertValue<bool>(((!object.Equals(((_CupertinoChevronPainter__text_selection_toolbar)oldDelegate).color, this.color)) || (((_CupertinoChevronPainter__text_selection_toolbar)oldDelegate).isLeft != this.isLeft)));
}

public class _CupertinoTextSelectionToolbarItems__text_selection_toolbar : global::Doroti.Framework.Widgets.RenderObjectWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget backButton { get; private set; } = default!;
    public virtual List<global::Doroti.Framework.Widgets.Widget> children { get; private set; } = default!;
    public virtual Color dividerColor { get; private set; } = default!;
    public virtual double dividerWidth { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget nextButton { get; private set; } = default!;
    public virtual long page { get; private set; } = default!;

    internal _CupertinoTextSelectionToolbarItems__text_selection_toolbar(global::Doroti.Framework.Foundation.Key? key = null, long page = default!, List<global::Doroti.Framework.Widgets.Widget> children = default!, global::Doroti.Framework.Widgets.Widget backButton = default!, Color dividerColor = default!, double dividerWidth = default!, global::Doroti.Framework.Widgets.Widget nextButton = default!) : base(key: key)
    {
        this.page = page;
        this.children = children;
        this.backButton = backButton;
        this.dividerColor = dividerColor;
        this.dividerWidth = dividerWidth;
        this.nextButton = nextButton;
        System.Diagnostics.Debug.Assert(System.Linq.Enumerable.Any(children));
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar(dividerColor: this.dividerColor, dividerWidth: this.dividerWidth, page: this.page));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar>)(() =>
{
    var __cascade = __renderObject;
    __cascade.page = this.page;
    __cascade.dividerColor = this.dividerColor;
    __cascade.dividerWidth = this.dividerWidth;
    return __cascade;
}))());
    }

    public override _CupertinoTextSelectionToolbarItemsElement__text_selection_toolbar createElement() => new _CupertinoTextSelectionToolbarItemsElement__text_selection_toolbar(this);
}

public class _CupertinoTextSelectionToolbarItemsElement__text_selection_toolbar : global::Doroti.Framework.Widgets.RenderObjectElement
{
    internal virtual List<global::Doroti.Framework.Widgets.Element> _children { get; set; } = default!;
    public virtual DartMap<_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar, global::Doroti.Framework.Widgets.Element> slotToChild { get; private set; } = new DartMap<_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar, global::Doroti.Framework.Widgets.Element>();
    internal virtual HashSet<global::Doroti.Framework.Widgets.Element> _forgottenChildren { get; private set; } = new HashSet<global::Doroti.Framework.Widgets.Element>();

    internal _CupertinoTextSelectionToolbarItemsElement__text_selection_toolbar(_CupertinoTextSelectionToolbarItems__text_selection_toolbar widget) : base(widget)
    {
    }

    public override _RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar renderObject => DartRuntimePrimitives.ConvertValue<_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar>(((_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar?)(object?)base.renderObject)!);
    internal virtual void _updateRenderObject(global::Doroti.Framework.Rendering.RenderBox? child, _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar slot)
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

    public override void insertRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        if ((slot is _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar))
        {
            _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar slot__as28001 = (_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar)slot;
            DartRuntimePrimitives.Assert(() => (child is global::Doroti.Framework.Rendering.RenderBox));
            _updateRenderObject(((global::Doroti.Framework.Rendering.RenderBox?)(object?)child)!, DartRuntimePrimitives.RequireValue(((_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar)slot__as28001)));
            DartRuntimePrimitives.Assert(() => ((_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar)this.renderObject).slottedChildren.ContainsKey(DartRuntimePrimitives.RequireValue(((_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar)slot__as28001))));
            return;
        }
        if ((slot is global::Doroti.Framework.Widgets.IndexedSlot<global::Doroti.Framework.Widgets.Element?>))
        {
            global::Doroti.Framework.Widgets.IndexedSlot<global::Doroti.Framework.Widgets.Element?> slot__as28229 = (global::Doroti.Framework.Widgets.IndexedSlot<global::Doroti.Framework.Widgets.Element?>)slot;
            DartRuntimePrimitives.Assert(() => ((bool)((dynamic)this.renderObject).debugValidateChild(child)));
            this.renderObject.insert(((global::Doroti.Framework.Rendering.RenderBox?)(object?)child)!, after: ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((global::Doroti.Framework.Widgets.IndexedSlot<global::Doroti.Framework.Widgets.Element?>)((global::Doroti.Framework.Widgets.IndexedSlot<global::Doroti.Framework.Widgets.Element?>)slot__as28229)).value?.renderObject)!);
            return;
        }
        DartRuntimePrimitives.Assert(() => false, () => (object?)"slot must be _CupertinoTextSelectionToolbarItemsSlot or IndexedSlot");
    }

    public override void moveRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? oldSlot, object? newSlot)
    {
        var __oldSlot = (global::Doroti.Framework.Widgets.IndexedSlot<global::Doroti.Framework.Widgets.Element>)(object)oldSlot;
        var __newSlot = (global::Doroti.Framework.Widgets.IndexedSlot<global::Doroti.Framework.Widgets.Element>)(object)newSlot;
        DartRuntimePrimitives.Assert(() => (object.Equals(((global::Doroti.Framework.Rendering.RenderObject)child).parent, this.renderObject)));
        ((dynamic)this.renderObject).move(((global::Doroti.Framework.Rendering.RenderBox?)(object?)child)!, after: ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((global::Doroti.Framework.Widgets.IndexedSlot<global::Doroti.Framework.Widgets.Element>)__newSlot).value.renderObject)!);
    }

    internal static bool _shouldPaint(global::Doroti.Framework.Widgets.Element child)
    {
        return (((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)((global::Doroti.Framework.Rendering.ParentData?)((dynamic)((global::Doroti.Framework.Widgets.Element)child).renderObject!).parentData)!)!).shouldPaint;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void removeRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        if ((slot is _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar))
        {
            _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar slot__as29126 = (_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar)slot;
            DartRuntimePrimitives.Assert(() => (child is global::Doroti.Framework.Rendering.RenderBox));
            DartRuntimePrimitives.Assert(() => ((_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar)this.renderObject).slottedChildren.ContainsKey(DartRuntimePrimitives.RequireValue(((_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar)slot__as29126))));
            _updateRenderObject(null, DartRuntimePrimitives.RequireValue(((_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar)slot__as29126)));
            DartRuntimePrimitives.Assert(() => !((_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar)this.renderObject).slottedChildren.ContainsKey(DartRuntimePrimitives.RequireValue(((_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar)slot__as29126))));
            return;
        }
        DartRuntimePrimitives.Assert(() => (slot is global::Doroti.Framework.Widgets.IndexedSlot<global::Doroti.Framework.Widgets.Element?>));
        DartRuntimePrimitives.Assert(() => (object.Equals(((global::Doroti.Framework.Rendering.RenderObject)child).parent, this.renderObject)));
        ((dynamic)this.renderObject).remove(((global::Doroti.Framework.Rendering.RenderBox?)(object?)child)!);
    }

    public override void visitChildren(global::System.Action<global::Doroti.Framework.Widgets.Element> visitor)
    {
        this.slotToChild.Values.forEach((__arg0) => ((global::System.Action<global::Doroti.Framework.Widgets.Element>)visitor)(__arg0));
        foreach (global::Doroti.Framework.Widgets.Element child in this._children)
        {
            if (!this._forgottenChildren.Contains(child))
            {
                visitor(child);
            }
        }
    }

    public override void forgetChild(global::Doroti.Framework.Widgets.Element child)
    {
        DartRuntimePrimitives.Assert(() => (this.slotToChild.containsValue(child) || this._children.Contains(child)));
        DartRuntimePrimitives.Assert(() => !this._forgottenChildren.Contains(child));
        if (this.slotToChild.ContainsKey(((_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar)(object)((global::Doroti.Framework.Widgets.Element)child).slot)))
        {
            var slotLocal = ((_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar)((global::Doroti.Framework.Widgets.Element)child).slot!);
            this.slotToChild.remove(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(slotLocal)));
        }
        else
        {
            this._forgottenChildren.Add(child);
        }
        base.forgetChild(child);
    }

    internal virtual void _mountChild(global::Doroti.Framework.Widgets.Widget widget, _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar slot)
    {
        global::Doroti.Framework.Widgets.Element? oldChild = this.slotToChild.GetValueOrDefault(DartRuntimePrimitives.RequireValue(slot));
        global::Doroti.Framework.Widgets.Element? newChild = ((global::Doroti.Framework.Widgets.Element?)(object?)updateChild(oldChild, widget, DartRuntimePrimitives.RequireValue(slot)));
        if ((oldChild is not null))
        {
            this.slotToChild.remove(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(slot)));
        }
        if ((newChild is not null))
        {
            this.slotToChild[slot] = newChild;
        }
    }

    public override void mount(global::Doroti.Framework.Widgets.Element? parent, object? newSlot)
    {
        base.mount(parent, newSlot);
        var toolbarItems = ((_CupertinoTextSelectionToolbarItems__text_selection_toolbar?)(object?)this.widget)!;
        _mountChild(((_CupertinoTextSelectionToolbarItems__text_selection_toolbar)toolbarItems).backButton, _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar.backButton);
        _mountChild(((_CupertinoTextSelectionToolbarItems__text_selection_toolbar)toolbarItems).nextButton, _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar.nextButton);
        global::Doroti.Framework.Widgets.Element? previousChild = default!;
        _children = new List<global::Doroti.Framework.Widgets.Element>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)checked((long)(((_CupertinoTextSelectionToolbarItems__text_selection_toolbar)toolbarItems).children.Count)))), ((i) =>
        {
            global::Doroti.Framework.Widgets.Element result = ((global::Doroti.Framework.Widgets.Element)(object?)inflateWidget(((_CupertinoTextSelectionToolbarItems__text_selection_toolbar)toolbarItems).children[(int)(i)], new global::Doroti.Framework.Widgets.IndexedSlot<global::Doroti.Framework.Widgets.Element?>(i, previousChild)));
            previousChild = result;
            return result;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
    }

    public override void debugVisitOnstageChildren(global::System.Action<global::Doroti.Framework.Widgets.Element> visitor)
    {
        foreach (global::Doroti.Framework.Widgets.Element childLocal in this.slotToChild.Values)
        {
            if ((_CupertinoTextSelectionToolbarItemsElement__text_selection_toolbar._shouldPaint(childLocal) && !this._forgottenChildren.Contains(childLocal)))
            {
                visitor(childLocal);
            }
        }
        this._children.where(((child) => (!this._forgottenChildren.Contains(child) && _CupertinoTextSelectionToolbarItemsElement__text_selection_toolbar._shouldPaint(child)))).forEach((__arg0) => ((global::System.Action<global::Doroti.Framework.Widgets.Element>)visitor)(__arg0));
    }

    public override void update(global::Doroti.Framework.Widgets.Widget newWidget)
    {
        var __newWidget = (_CupertinoTextSelectionToolbarItems__text_selection_toolbar)(object)newWidget;
        base.update(__newWidget);
        DartRuntimePrimitives.Assert(() => (object.Equals(this.widget, __newWidget)));
        var toolbarItems = ((_CupertinoTextSelectionToolbarItems__text_selection_toolbar?)(object?)this.widget)!;
        _mountChild(((_CupertinoTextSelectionToolbarItems__text_selection_toolbar)toolbarItems).backButton, _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar.backButton);
        _mountChild(((_CupertinoTextSelectionToolbarItems__text_selection_toolbar)toolbarItems).nextButton, _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar.nextButton);
        _children = updateChildren(this._children, ((_CupertinoTextSelectionToolbarItems__text_selection_toolbar)toolbarItems).children, forgottenChildren: this._forgottenChildren);
        this._forgottenChildren.Clear();
    }

}

public class _RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar : global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.ContainerRenderObjectMixin<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Widgets.ToolbarItemsParentData>, global::Doroti.Framework.Rendering.RenderBoxContainerDefaultsMixin<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Widgets.ToolbarItemsParentData>
{
    public virtual DartMap<_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar, global::Doroti.Framework.Rendering.RenderBox> slottedChildren { get; private set; } = new DartMap<_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar, global::Doroti.Framework.Rendering.RenderBox>();
    public virtual bool hasNextPage { get; set; } = default!;
    public virtual bool hasPreviousPage { get; set; } = default!;
    internal virtual long _page { get; set; } = default!;
    internal virtual Color _dividerColor { get; set; } = default!;
    internal virtual double _dividerWidth { get; set; } = default!;
    internal virtual global::Doroti.Framework.Rendering.RenderBox? _backButton { get; set; } = default;
    internal virtual global::Doroti.Framework.Rendering.RenderBox? _nextButton { get; set; } = default;
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;

    internal _RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar(Color dividerColor, double dividerWidth, long page)
    {
        this._dividerColor = dividerColor;
        this._dividerWidth = dividerWidth;
        this._page = page;
    }

    internal virtual global::Doroti.Framework.Rendering.RenderBox? _updateChild(global::Doroti.Framework.Rendering.RenderBox? oldChild, global::Doroti.Framework.Rendering.RenderBox? newChild, _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar slot)
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
    public virtual global::Doroti.Framework.Rendering.RenderBox? backButton
    {
        get => this._backButton;
        set
        {
            var __value = value;
            _backButton = _updateChild(this._backButton, __value, DartRuntimePrimitives.RequireValue(_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar.backButton));
        }
    }
    public virtual global::Doroti.Framework.Rendering.RenderBox? nextButton
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
            size = ((global::Doroti.Framework.Rendering.BoxConstraints)this.constraints).smallest;
            return;
        }
        var greatestHeight = 0.0;
        visitChildren(((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)((renderObjectChild) =>
        {
            var child = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)renderObjectChild)!;
            double childHeight = child.getMaxIntrinsicHeight(((global::Doroti.Framework.Rendering.BoxConstraints)this.constraints).maxWidth);
            if ((childHeight > greatestHeight))
            {
                greatestHeight = childHeight;
            }
        })));
        var slottedConstraints = new global::Doroti.Framework.Rendering.BoxConstraints(maxWidth: ((global::Doroti.Framework.Rendering.BoxConstraints)this.constraints).maxWidth, minHeight: greatestHeight, maxHeight: greatestHeight);
        this._backButton!.layout(slottedConstraints, parentUsesSize: true);
        this._nextButton!.layout(slottedConstraints, parentUsesSize: true);
        double subsequentPageButtonsWidth = (this._backButton!.size.width + this._nextButton!.size.width);
        var currentButtonPosition = 0.0;
        double toolbarWidth = default!;
        var currentPage = 0L;
        var i = -1L;
        visitChildren(((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)((renderObjectChild) =>
        {
            i++;
            var childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)renderObjectChild)!;
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)childLocal.parentData!)!;
            childParentData.shouldPaint = false;
            if ((((object.Equals(childLocal, this._backButton)) || (object.Equals(childLocal, this._nextButton))) || (currentPage > this._page)))
            {
                return;
            }
            double paginationButtonsWidth = ((currentPage == 0L) ? ((i == (this.childCount + 1L)) ? 0.0 : this._nextButton!.size.width) : subsequentPageButtonsWidth);
            childLocal.layout(new global::Doroti.Framework.Rendering.BoxConstraints(maxWidth: (((global::Doroti.Framework.Rendering.BoxConstraints)this.constraints).maxWidth - paginationButtonsWidth), minHeight: greatestHeight, maxHeight: greatestHeight), parentUsesSize: true);
            double currentWidth = ((currentButtonPosition + paginationButtonsWidth) + ((global::Doroti.Framework.Rendering.RenderBox)childLocal).size.width);
            if ((currentWidth > ((global::Doroti.Framework.Rendering.BoxConstraints)this.constraints).maxWidth))
            {
                currentPage++;
                currentButtonPosition = (this._backButton!.size.width + this.dividerWidth);
                paginationButtonsWidth = (this._backButton!.size.width + this._nextButton!.size.width);
                childLocal.layout(new global::Doroti.Framework.Rendering.BoxConstraints(maxWidth: (((global::Doroti.Framework.Rendering.BoxConstraints)this.constraints).maxWidth - paginationButtonsWidth), minHeight: greatestHeight, maxHeight: greatestHeight), parentUsesSize: true);
            }
            childParentData.offset = new global::Doroti.Ui.Offset(currentButtonPosition, 0.0);
            currentButtonPosition += (((global::Doroti.Framework.Rendering.RenderBox)childLocal).size.width + this.dividerWidth);
            childParentData.shouldPaint = (currentPage == this.page);
            if ((currentPage == this.page))
            {
                toolbarWidth = currentButtonPosition;
            }
        })));
        DartRuntimePrimitives.Assert(() => (this.page <= currentPage));
        if ((currentPage > 0L))
        {
            var nextButtonParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)this._nextButton!.parentData!)!;
            var backButtonParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)this._backButton!.parentData!)!;
            if ((this.page != currentPage))
            {
                nextButtonParentData.offset = new global::Doroti.Ui.Offset(toolbarWidth, 0.0);
                nextButtonParentData.shouldPaint = true;
                toolbarWidth += this.nextButton!.size.width;
            }
            if ((this.page > 0L))
            {
                backButtonParentData.offset = Offset.zero;
                backButtonParentData.shouldPaint = true;
            }
        }
        else
        {
            toolbarWidth -= this.dividerWidth;
        }
        hasNextPage = (this.page != currentPage);
        hasPreviousPage = (this.page > 0L);
        size = this.constraints.constrain(new global::Doroti.Ui.Size(toolbarWidth, greatestHeight));
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        visitChildren(((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)((renderObjectChild) =>
        {
            var child = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)renderObjectChild)!;
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
            if (((global::Doroti.Framework.Widgets.ToolbarItemsParentData)childParentData).shouldPaint)
            {
                global::Doroti.Ui.Offset childOffset = ((global::Doroti.Ui.Offset)(object?)(childParentData.offset + offset));
                context.paintChild(child, childOffset);
                if (((childParentData.nextSibling is not null) || (object.Equals(child, this.backButton))))
                {
                    ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawLine((new global::Doroti.Ui.Offset(((global::Doroti.Framework.Rendering.RenderBox)child).size.width, 0) + childOffset), (new global::Doroti.Ui.Offset(((global::Doroti.Framework.Rendering.RenderBox)child).size.width, ((global::Doroti.Framework.Rendering.RenderBox)child).size.height) + childOffset), ((Func<Paint>)(() =>
            {
                var __cascade = new global::Doroti.Ui.Paint();
                __cascade.color = this.dividerColor;
                return __cascade;
            }))());
                }
            }
        })));
    }

    public override void setupParentData(global::Doroti.Framework.Rendering.RenderObject child)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)(object)child;
        if ((__child.parentData is not global::Doroti.Framework.Widgets.ToolbarItemsParentData))
        {
            __child.parentData = new global::Doroti.Framework.Widgets.ToolbarItemsParentData();
        }
    }

    public static bool hitTestChild(global::Doroti.Framework.Rendering.RenderBox? child, global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        if ((child is null))
        {
            return false;
        }
        var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
        if (!((global::Doroti.Framework.Widgets.ToolbarItemsParentData)childParentData).shouldPaint)
        {
            return false;
        }
        return result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: ((global::System.Func<global::Doroti.Framework.Rendering.BoxHitTestResult, Offset, bool>)((result, transformed) =>
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - childParentData.offset))));
            return child.hitTest(result, position: transformed);
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestChildren(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = this.lastChild;
        while ((child is not null))
        {
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
            if (!((global::Doroti.Framework.Widgets.ToolbarItemsParentData)childParentData).shouldPaint)
            {
                child = childParentData.previousSibling;
                continue;
            }
            if (_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar.hitTestChild(child, result, position: position))
            {
                return true;
            }
            child = childParentData.previousSibling;
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

    public override void attach(global::Doroti.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        global::Doroti.Framework.Rendering.RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            child.attach(owner);
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
        foreach (global::Doroti.Framework.Rendering.RenderBox childLocal in this.slottedChildren.Values)
        {
            childLocal.attach(owner);
        }
    }

    public override void detach()
    {
        base.detach();
        global::Doroti.Framework.Rendering.RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            child.detach();
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
        foreach (global::Doroti.Framework.Rendering.RenderBox childLocal in this.slottedChildren.Values)
        {
            childLocal.detach();
        }
    }

    public override void redepthChildren()
    {
        visitChildren(((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)((renderObjectChild) =>
        {
            var child = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)renderObjectChild)!;
            redepthChild(child);
        })));
    }

    public override void visitChildren(global::System.Action<global::Doroti.Framework.Rendering.RenderObject> visitor)
    {
        if ((this._backButton is not null))
        {
            visitor(this._backButton!);
        }
        if ((this._nextButton is not null))
        {
            visitor(this._nextButton!);
        }
        global::Doroti.Framework.Rendering.RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            visitor(child);
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void visitChildrenForSemantics(global::System.Action<global::Doroti.Framework.Rendering.RenderObject> visitor)
    {
        visitChildren(((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)((renderObjectChild) =>
        {
            var child = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)renderObjectChild)!;
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
            if (((global::Doroti.Framework.Widgets.ToolbarItemsParentData)childParentData).shouldPaint)
            {
                visitor(((global::Doroti.Framework.Rendering.RenderBox)renderObjectChild));
            }
        })));
    }

    public override List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        var value = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        visitChildren(((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)((renderObjectChild) =>
        {
            var child = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)renderObjectChild)!;
            if ((object.Equals(child, this.backButton)))
            {
                value.Add(((Diagnosticable)child).toDiagnosticsNode(name: "back button"));
            }
            else
            {
                if ((object.Equals(child, this.nextButton)))
                {
                    value.Add(((Diagnosticable)child).toDiagnosticsNode(name: "next button"));
                }
                else
                {
                    value.Add(((Diagnosticable)child).toDiagnosticsNode(name: "menu item"));
                }
            }
        })));
        return value;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
        while ((childParentData.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData.previousSibling, child)));
            child = childParentData.previousSibling!;
            childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
        while ((childParentData.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData.nextSibling, child)));
            child = childParentData.nextSibling!;
            childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
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
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"A {this.GetType()} expected a child of type {typeof(RenderBox)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new global::Doroti.Framework.Foundation.ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new global::Doroti.Framework.Foundation.ErrorSpacer(), new global::Doroti.Framework.Foundation.DiagnosticsProperty<object?>($"The {this.GetType()} that expected a {typeof(RenderBox)} child was created by", this.debugCreator, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty), new global::Doroti.Framework.Foundation.ErrorSpacer(), new global::Doroti.Framework.Foundation.DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", ((RenderObject)child).debugCreator, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty) }));
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _insertIntoChildList(RenderBox child, RenderBox? after = null)
    {
        var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)this._firstChild!.parentData!)!;
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
            var afterParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)after.parentData!)!;
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
                var childPreviousSiblingParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)childParentData.previousSibling!.parentData!)!;
                var childNextSiblingParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)childParentData.nextSibling!.parentData!)!;
                childPreviousSiblingParentData.nextSibling = child;
                childNextSiblingParentData.previousSibling = child;
                DartRuntimePrimitives.Assert(() => (object.Equals(afterParentData.nextSibling, child)));
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
        DartRuntimePrimitives.Assert(() => (child.parentData is global::Doroti.Framework.Widgets.ToolbarItemsParentData), () => (object?)$"A child of {this.GetType()} has parentData of type {DartRuntimePrimitives.RuntimeType(child.parentData)}, " + $"which does not conform to {(typeof(global::Doroti.Framework.Widgets.ToolbarItemsParentData))}. Class using ContainerRenderObjectMixin " + $"should override setupParentData() to set parentData to type {(typeof(global::Doroti.Framework.Widgets.ToolbarItemsParentData))}.");
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
        var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
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
            var childPreviousSiblingParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)childParentData.previousSibling!.parentData!)!;
            childPreviousSiblingParentData.nextSibling = childParentData.nextSibling;
        }
        if ((childParentData.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData.previousSibling;
        }
        else
        {
            var childNextSiblingParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)childParentData.nextSibling!.parentData!)!;
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
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
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
        var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
        if ((object.Equals(childParentData.previousSibling, after)))
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
        var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
        return childParentData.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToFirstActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !this.debugNeedsLayout);
        RenderBox? child = this.firstChild;
        while ((child is not null))
        {
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
            double? result = child.getDistanceToActualBaseline(baseline);
            if ((result is not null))
            {
                double result__138852__value138916 = DartRuntimePrimitives.RequireValue(result);
                return (DartRuntimePrimitives.RequireValue(result__138852__value138916) + childParentData.offset.dy);
            }
            child = childParentData.nextSibling;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToHighestActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !this.debugNeedsLayout);
        BaselineOffset minBaseline = BaselineOffset.noBaseline;
        RenderBox? child = this.firstChild;
        while ((child is not null))
        {
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
            BaselineOffset candidate = (new BaselineOffset(child.getDistanceToActualBaseline(baseline)).op_Add(childParentData.offset.dy));
            minBaseline = minBaseline.minOf(candidate);
            child = childParentData.nextSibling;
        }
        return minBaseline.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool defaultHitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child = this.lastChild;
        while ((child is not null))
        {
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
            bool isHit = result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: ((global::System.Func<BoxHitTestResult, Offset, bool>)((result, transformed) =>
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - childParentData.offset))));
                return child!.hitTest(result, position: transformed);
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
            if (isHit)
            {
                return true;
            }
            child = childParentData.previousSibling;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void defaultPaint(PaintingContext context, Offset offset)
    {
        RenderBox? child = this.firstChild;
        while ((child is not null))
        {
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
            context.paintChild(child, (childParentData.offset + offset));
            child = childParentData.nextSibling;
        }
    }

    public virtual List<RenderBox> getChildrenAsList()
    {
        var result = new List<RenderBox>();
        RenderBox? child = this.firstChild;
        while ((child is not null))
        {
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
            result.Add(((RenderBox?)(object?)child)!);
            child = childParentData.nextSibling;
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public enum _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar
{
    backButton,
    nextButton
}
