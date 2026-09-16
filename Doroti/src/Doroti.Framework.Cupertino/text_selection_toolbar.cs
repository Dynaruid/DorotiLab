// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/text_selection_toolbar.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class Text_selection_toolbarLibrary
{
    internal static Radius _kToolbarBorderRadius = Radius.circular(8.0);
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
        System.Diagnostics.Debug.Assert(checked(children.Count) > 0L);
    }

    internal static global::Doroti.Framework.Widgets.Widget _defaultToolbarBuilder(global::Doroti.Framework.Widgets.BuildContext context, Offset anchorAbove, Offset anchorBelow, global::Doroti.Framework.Widgets.Widget child)
    {
        return new _CupertinoTextSelectionToolbarShape__text_selection_toolbar(anchorAbove: anchorAbove, anchorBelow: anchorBelow, shadowColor: Equals(CupertinoTheme.brightnessOf(context), Brightness.light) ? CupertinoColors.black.withOpacity(0.2) : null, child: new global::Doroti.Framework.Widgets.ColoredBox(color: Text_selection_toolbarLibrary._kToolbarBackgroundColor.resolveFrom(context), child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        global::Doroti.Framework.Painting.EdgeInsets mediaQueryPadding = MediaQuery.paddingOf(context);
        double paddingAbove = mediaQueryPadding.top + kToolbarScreenPadding;
        double leftMargin = Text_selection_toolbarLibrary._kArrowScreenPadding + mediaQueryPadding.left;
        double rightMargin = MediaQuery.widthOf(context) - mediaQueryPadding.right - Text_selection_toolbarLibrary._kArrowScreenPadding;
        var anchorAboveAdjusted = new global::Doroti.Ui.Offset(Dart_uiLibrary.clampDouble(anchorAbove.dx, leftMargin, rightMargin), anchorAbove.dy - Text_selection_toolbarLibrary._kToolbarContentDistance - paddingAbove);
        var anchorBelowAdjusted = new global::Doroti.Ui.Offset(Dart_uiLibrary.clampDouble(anchorBelow.dx, leftMargin, rightMargin), anchorBelow.dy + Text_selection_toolbarLibrary._kToolbarContentDistance - paddingAbove);
        return new global::Doroti.Framework.Widgets.Padding(padding: new global::Doroti.Framework.Painting.EdgeInsets(kToolbarScreenPadding, paddingAbove, kToolbarScreenPadding, kToolbarScreenPadding), child: new global::Doroti.Framework.Widgets.CustomSingleChildLayout(@delegate: new global::Doroti.Framework.Widgets.TextSelectionToolbarLayoutDelegate(anchorAbove: anchorAboveAdjusted, anchorBelow: anchorBelowAdjusted), child: new _CupertinoTextSelectionToolbarContent__text_selection_toolbar(anchorAbove: anchorAboveAdjusted, anchorBelow: anchorBelowAdjusted, toolbarBuilder: toolbarBuilder, children: children)));
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
        _anchorAbove = anchorAbove;
        _anchorBelow = anchorBelow;
        _shadowColor = shadowColor;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(new _RenderCupertinoTextSelectionToolbarShape__text_selection_toolbar(_anchorAbove, _anchorBelow, _shadowColor, null));
    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderCupertinoTextSelectionToolbarShape__text_selection_toolbar)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderCupertinoTextSelectionToolbarShape__text_selection_toolbar>)(() =>
{
    var __cascade = __renderObject;
    __cascade.anchorAbove = _anchorAbove;
    __cascade.anchorBelow = _anchorBelow;
    __cascade.shadowColor = _shadowColor;
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
        get => _anchorAbove;
        set
        {
            var __value = value;
            if (Equals(DartRuntimePrimitives.RequireValue(__value), _anchorAbove))
            {
                return;
            }
            _anchorAbove = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.Offset anchorBelow
    {
        get => _anchorBelow;
        set
        {
            var __value = value;
            if (Equals(DartRuntimePrimitives.RequireValue(__value), _anchorBelow))
            {
                return;
            }
            _anchorBelow = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.Color? shadowColor
    {
        get => _shadowColor;
        set
        {
            var __value = value is null ? null : value;
            if (Equals(__value, _shadowColor))
            {
                return;
            }
            _shadowColor = __value;
            markNeedsPaint();
        }
    }
    internal virtual bool _isAbove(double childHeight) => DartRuntimePrimitives.ConvertValue<bool>(anchorAbove.dy >= (childHeight - Text_selection_toolbarLibrary._kToolbarArrowSize.height));
    internal virtual global::Doroti.Framework.Rendering.BoxConstraints _constraintsForChild(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: Text_selection_toolbarLibrary._kToolbarArrowSize.width + (Text_selection_toolbarLibrary._kToolbarBorderRadius.x * 2L)).enforce(constraints.loosen());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Offset _computeChildOffset(Size childSize)
    {
        return new global::Doroti.Ui.Offset(0.0, _isAbove(childSize.height) ? -Text_selection_toolbarLibrary._kToolbarArrowSize.height : 0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return null;
        }
        global::Doroti.Framework.Rendering.BoxConstraints enforcedConstraint = _constraintsForChild(constraints);
        double? result = childLocal.getDryBaseline(enforcedConstraint, baseline);
        return (result is null) ? null : (DartRuntimePrimitives.RequireValue(result) + _computeChildOffset(childLocal.getDryLayout(enforcedConstraint)).dy);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return;
        }
        childLocal.layout(_constraintsForChild(constraints), parentUsesSize: true);
        var childParentData = ((global::Doroti.Framework.Rendering.BoxParentData?)childLocal.parentData!)!;
        childParentData.offset = _computeChildOffset(childLocal.size);
        size = new global::Doroti.Ui.Size(childLocal.size.width, childLocal.size.height - Text_selection_toolbarLibrary._kToolbarArrowSize.height);
    }

    internal virtual global::Doroti.Ui.RRect _shapeRRect(global::Doroti.Framework.Rendering.RenderBox child)
    {
        global::Doroti.Ui.Rect rect = new global::Doroti.Ui.Offset(0.0, Text_selection_toolbarLibrary._kToolbarArrowSize.height) & new global::Doroti.Ui.Size(child.size.width, child.size.height - (Text_selection_toolbarLibrary._kToolbarArrowSize.height * 2L));
        return RRect.fromRectAndRadius(rect, Text_selection_toolbarLibrary._kToolbarBorderRadius).scaleRadii();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Ui.Path _addRRectToPath(Path path, RRect rrect, double startAngle)
    {
        double halfPI = Dart_mathLibrary.pi / 2L;
        DartRuntimePrimitives.Assert(() => (startAngle % halfPI) == 0.0);
        global::Doroti.Ui.Rect rect = rrect.outerRect;
        var rrectCorners = new List<(global::Doroti.Ui.Offset, global::Doroti.Ui.Radius)> { (rect.bottomRight, -rrect.brRadius), (rect.bottomLeft, Radius.elliptical(rrect.blRadiusX, -rrect.blRadiusY)), (rect.topLeft, rrect.tlRadius), (rect.topRight, Radius.elliptical(-rrect.trRadiusX, rrect.trRadiusY)) };
        long startQuadrantIndex = checked((long)(startAngle / halfPI));
        for (var i = startQuadrantIndex; i < (checked(rrectCorners.Count) + startQuadrantIndex); i += 1L)
        {
            // Dart modulo stays non-negative. A toolbar below the selection
            // starts at quadrant -1, whose corner is the top-right corner.
            var cornerIndex = (int)((i % rrectCorners.Count + rrectCorners.Count) % rrectCorners.Count);
            var (vertex, rectCenterOffset) = rrectCorners[cornerIndex];
            var otherVertex = new global::Doroti.Ui.Offset(vertex.dx + (2L * rectCenterOffset.x), vertex.dy + (2L * rectCenterOffset.y));
            var rectLocal = Rect.fromPoints(vertex, otherVertex);
            path.arcTo(rectLocal, halfPI * i, halfPI, false);
        }
        return path;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Path _clipPath(global::Doroti.Framework.Rendering.RenderBox child, RRect rrect)
    {
        var path = new global::Doroti.Ui.Path();
        if (((Text_selection_toolbarLibrary._kToolbarBorderRadius.x * 2L) + Text_selection_toolbarLibrary._kToolbarArrowSize.width) > size.width)
        {
            return ((Func<Path>)(() =>
{
    var __cascade = path;
    __cascade.addRRect(rrect);
    return __cascade;
}))();
        }
        bool isAbove = _isAbove(child.size.height);
        global::Doroti.Ui.Offset localAnchor = globalToLocal(isAbove ? _anchorAbove : _anchorBelow);
        double arrowTipX = Dart_uiLibrary.clampDouble(localAnchor.dx, Text_selection_toolbarLibrary._kToolbarBorderRadius.x + (Text_selection_toolbarLibrary._kToolbarArrowSize.width / 2L), size.width - (Text_selection_toolbarLibrary._kToolbarArrowSize.width / 2L) - Text_selection_toolbarLibrary._kToolbarBorderRadius.x);
        if (isAbove)
        {
            double arrowBaseY = child.size.height - Text_selection_toolbarLibrary._kToolbarArrowSize.height;
            double arrowTipY = child.size.height;
            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = path;
    __cascade.moveTo(arrowTipX + (Text_selection_toolbarLibrary._kToolbarArrowSize.width / 2L), arrowBaseY);
    __cascade.lineTo(arrowTipX, arrowTipY);
    __cascade.lineTo(arrowTipX - (Text_selection_toolbarLibrary._kToolbarArrowSize.width / 2L), arrowBaseY);
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
    __cascade.moveTo(arrowTipX - (Text_selection_toolbarLibrary._kToolbarArrowSize.width / 2L), arrowBaseYLocal);
    __cascade.lineTo(arrowTipX, arrowTipYLocal);
    __cascade.lineTo(arrowTipX + (Text_selection_toolbarLibrary._kToolbarArrowSize.width / 2L), arrowBaseYLocal);
    return __cascade;
}))());
        }
        double startAngleLocal = isAbove ? (Dart_mathLibrary.pi / 2L) : (-Dart_mathLibrary.pi / 2L);
        return ((Func<Path>)(() =>
{
    var __cascade = _addRRectToPath(path, rrect, startAngle: startAngleLocal);
    __cascade.close();
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return;
        }
        var childParentData = ((global::Doroti.Framework.Rendering.BoxParentData?)childLocal.parentData!)!;
        global::Doroti.Ui.RRect rrect = _shapeRRect(childLocal);
        global::Doroti.Ui.Path clipPath = _clipPath(childLocal, rrect);
        if (_shadowColor is not null)
        {
            var boxShadow = new global::Doroti.Framework.Painting.BoxShadow(color: _shadowColor!, blurRadius: 15.0);
            global::Doroti.Ui.RRect shadowRRect = RRect.fromLTRBR(rrect.left, rrect.top, rrect.right, rrect.bottom + Text_selection_toolbarLibrary._kToolbarArrowSize.height, Text_selection_toolbarLibrary._kToolbarBorderRadius).shift(offset + childParentData.offset + boxShadow.offset);
            context.canvas.drawRRect(shadowRRect, boxShadow.toPaint());
        }
        _clipPathLayer.layer = context.pushClipPath(needsCompositing, offset + childParentData.offset, Offset.zero & childLocal.size, clipPath, (innerContext, innerOffset) => { innerContext.paintChild(childLocal, innerOffset); }, oldLayer: _clipPathLayer.layer);
    }

    public override void dispose()
    {
        _clipPathLayer.layer = null;
        base.dispose();
    }

    public override void debugPaintSize(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                global::Doroti.Framework.Rendering.RenderBox? childLocal = child;
                if (childLocal is null)
                {
                    return true;
                }
                global::Doroti.Ui.Paint debugPaint = _debugPaint ??= ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.shader = Ui.Gradient.linear(Offset.zero, new global::Doroti.Ui.Offset(10.0, 10.0), new List<global::Doroti.Ui.Color> { CupertinoColors.transparent, new global::Doroti.Ui.Color(4294902015L), new global::Doroti.Ui.Color(4294902015L), CupertinoColors.transparent }, new List<double> { 0.25, 0.25, 0.75, 0.75 }, TileMode.repeated);
    __cascade.strokeWidth = 2.0;
    __cascade.style = PaintingStyle.stroke;
    return __cascade;
}))();
                var childParentData = ((global::Doroti.Framework.Rendering.BoxParentData?)childLocal.parentData!)!;
                global::Doroti.Ui.Path clipPath = _clipPath(childLocal, _shapeRRect(childLocal));
                context.canvas.drawPath(clipPath.shift(offset + childParentData.offset), debugPaint);
                return true;
            });
    }

    public override bool hitTestChildren(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return false;
        }
        var childParentData = ((global::Doroti.Framework.Rendering.BoxParentData?)childLocal.parentData!)!;
        var hitBox = Rect.fromLTWH(childParentData.offset.dx, childParentData.offset.dy + Text_selection_toolbarLibrary._kToolbarArrowSize.height, childLocal.size.width, childLocal.size.height - (Text_selection_toolbarLibrary._kToolbarArrowSize.height * 2L));
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
        System.Diagnostics.Debug.Assert(checked(children.Count) > 0L);
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoTextSelectionToolbarContentState__text_selection_toolbar());
}

public class _CupertinoTextSelectionToolbarContentState__text_selection_toolbar : global::Doroti.Framework.Widgets.State<_CupertinoTextSelectionToolbarContent__text_selection_toolbar>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<_CupertinoTextSelectionToolbarContent__text_selection_toolbar>
{
    internal virtual global::Doroti.Framework.Animation.AnimationController _controller { get; set; } = default!;
    internal virtual long? _nextPage { get; set; } = default;
    internal virtual long _page { get; set; } = 0L;
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _toolbarItemsKey { get; private set; } = GlobalKey<IState>.Create();
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual void _onHorizontalDragEnd(global::Doroti.Framework.Gestures.DragEndDetails details)
    {
        double? velocity = details.primaryVelocity;
        if ((velocity is not null) && (DartRuntimePrimitives.RequireValue(velocity) != 0L))
        {
            double velocity__19307__value19352 = DartRuntimePrimitives.RequireValue(velocity);
            if (DartRuntimePrimitives.RequireValue(velocity__19307__value19352) > 0L)
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
        var renderToolbar = ((global::Doroti.Framework.Rendering.RenderBox?)_toolbarItemsKey.currentContext?.findRenderObject())!;
        if ((renderToolbar is _RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar) && ((_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar)renderToolbar).hasNextPage)
        {
            _RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar renderToolbar__19544__as19636 = (_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar)renderToolbar;
            _controller.reverse();
            _controller.addStatusListener(_statusListener);
            _nextPage = _page + 1L;
        }
    }

    internal virtual void _handlePreviousPage()
    {
        var renderToolbar = ((global::Doroti.Framework.Rendering.RenderBox?)_toolbarItemsKey.currentContext?.findRenderObject())!;
        if ((renderToolbar is _RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar) && ((_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar)renderToolbar).hasPreviousPage)
        {
            _RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar renderToolbar__19891__as19983 = (_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar)renderToolbar;
            _controller.reverse();
            _controller.addStatusListener(_statusListener);
            _nextPage = _page - 1L;
        }
    }

    internal virtual void _statusListener(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        if (!AnimationStatusMembers.isDismissed(status))
        {
            return;
        }
        setState(() =>
        {
            _page = DartRuntimePrimitives.RequireValue(_nextPage);
            _nextPage = null;
        });
        _controller.forward();
        _controller.removeStatusListener(_statusListener);
    }

    public override void initState()
    {
        base.initState();
        _controller = new global::Doroti.Framework.Animation.AnimationController(value: 1.0, vsync: this, duration: Text_selection_toolbarLibrary._kToolbarTransitionDuration);
    }

    public override void didUpdateWidget(_CupertinoTextSelectionToolbarContent__text_selection_toolbar oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(widget.children, oldWidget.children))
        {
            _page = 0L;
            _nextPage = null;
            _controller.forward();
            _controller.removeStatusListener(_statusListener);
        }
    }

    public override void dispose()
    {
        _controller.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if (_tickers is not null)
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in _tickers!)
                    {
                        if (ticker.isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color chevronColor = Text_selection_toolbarLibrary._kToolbarTextColor.resolveFrom(context);
        global::Doroti.Framework.Widgets.Widget backButtonLocal = new global::Doroti.Framework.Widgets.Center(widthFactor: 1.0, heightFactor: 1.0, child: new CupertinoTextSelectionToolbarButton(onPressed: () => _handlePreviousPage(), child: new global::Doroti.Framework.Widgets.IgnorePointer(child: new global::Doroti.Framework.Widgets.CustomPaint(painter: new _LeftCupertinoChevronPainter__text_selection_toolbar(color: chevronColor), size: new global::Doroti.Ui.Size(Text_selection_toolbarLibrary._kToolbarChevronSize)))));
        global::Doroti.Framework.Widgets.Widget nextButtonLocal = new global::Doroti.Framework.Widgets.Center(widthFactor: 1.0, heightFactor: 1.0, child: new CupertinoTextSelectionToolbarButton(onPressed: () => _handleNextPage(), child: new global::Doroti.Framework.Widgets.IgnorePointer(child: new global::Doroti.Framework.Widgets.CustomPaint(painter: new _RightCupertinoChevronPainter__text_selection_toolbar(color: chevronColor), size: new global::Doroti.Ui.Size(Text_selection_toolbarLibrary._kToolbarChevronSize)))));
        List<global::Doroti.Framework.Widgets.Widget> childrenLocal = widget.children.map<global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Widgets.Center>((child) =>
        {
            return new global::Doroti.Framework.Widgets.Center(widthFactor: 1.0, heightFactor: 1.0, child: child);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }).ToList().Cast<global::Doroti.Framework.Widgets.Widget>().ToList();
        return widget.toolbarBuilder(context, widget.anchorAbove, widget.anchorBelow, new global::Doroti.Framework.Widgets.FadeTransition(opacity: _controller, child: new global::Doroti.Framework.Widgets.AnimatedSize(duration: Text_selection_toolbarLibrary._kToolbarTransitionDuration, curve: Curves.decelerate, child: new global::Doroti.Framework.Widgets.GestureDetector(onHorizontalDragEnd: _onHorizontalDragEnd, child: new _CupertinoTextSelectionToolbarItems__text_selection_toolbar(key: _toolbarItemsKey, page: _page, backButton: backButtonLocal, dividerColor: Text_selection_toolbarLibrary._kToolbarDividerColor.resolveFrom(context), dividerWidth: 1.0 / MediaQuery.devicePixelRatioOf(context), nextButton: nextButtonLocal, children: childrenLocal)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider(onTick, this, debugLabel: Foundation.ConstantsLibrary.kDebugMode ? $"created by {DiagnosticsLibrary.describeIdentity(this)}" : null);
    __cascade.muted = !values.enabled;
    __cascade.forceFrames = values.forceFrames;
    return __cascade;
}))();
        _tickers!.Add(result);
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => _tickers is not null);
        DartRuntimePrimitives.Assert(() => _tickers!.Contains(ticker));
        _tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if (_tickers is not null)
        {
            TickerModeData values = _tickerModeNotifier!.value;
            bool mutedLocal = !values.enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", _tickers, description: (_tickers is not null) ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}" : null, defaultValue: default));
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
        DartRuntimePrimitives.Assert(() => size.height == size.width, () => (object?)$"size must have the same height and width: {size}");
        double iconSize = size.height;
        var centerOffset = new global::Doroti.Ui.Offset(iconSize / 4L * (isLeft ? 1L : -1L), 0);
        global::Doroti.Ui.Offset firstPoint = new global::Doroti.Ui.Offset(iconSize / 2L, 0) + centerOffset;
        global::Doroti.Ui.Offset middlePoint = new global::Doroti.Ui.Offset(isLeft ? 0 : iconSize, iconSize / 2L) + centerOffset;
        global::Doroti.Ui.Offset lowerPoint = new global::Doroti.Ui.Offset(iconSize / 2L, iconSize) + centerOffset;
        var paintLocal = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = color;
    __cascade.style = PaintingStyle.stroke;
    __cascade.strokeWidth = Text_selection_toolbarLibrary._kToolbarChevronThickness;
    __cascade.strokeCap = StrokeCap.round;
    __cascade.strokeJoin = StrokeJoin.round;
    return __cascade;
}))();
        canvas.drawLine(firstPoint, middlePoint, paintLocal);
        canvas.drawLine(middlePoint, lowerPoint, paintLocal);
    }

    public override bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate) => DartRuntimePrimitives.ConvertValue<bool>((!Equals(((_CupertinoChevronPainter__text_selection_toolbar)oldDelegate).color, color)) || (((_CupertinoChevronPainter__text_selection_toolbar)oldDelegate).isLeft != isLeft));
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
        System.Diagnostics.Debug.Assert(Enumerable.Any(children));
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new _RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar(dividerColor: dividerColor, dividerWidth: dividerWidth, page: page);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar>)(() =>
{
    var __cascade = __renderObject;
    __cascade.page = page;
    __cascade.dividerColor = dividerColor;
    __cascade.dividerWidth = dividerWidth;
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

    public override _RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar renderObject => DartRuntimePrimitives.ConvertValue<_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar>(((_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar?)base.renderObject)!);
    internal virtual void _updateRenderObject(global::Doroti.Framework.Rendering.RenderBox? child, _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar slot)
    {
        switch (DartRuntimePrimitives.RequireValue(slot))
        {
            case _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar.backButton:
                {
                    renderObject.backButton = child;
                    break;
                }
            case _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar.nextButton:
                {
                    renderObject.nextButton = child;
                    break;
                }
        }
    }

    public override void insertRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        if (slot is _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar)
        {
            _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar slot__as28001 = (_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar)slot;
            DartRuntimePrimitives.Assert(() => child is global::Doroti.Framework.Rendering.RenderBox);
            _updateRenderObject(((global::Doroti.Framework.Rendering.RenderBox?)child)!, DartRuntimePrimitives.RequireValue(slot__as28001));
            DartRuntimePrimitives.Assert(() => renderObject.slottedChildren.ContainsKey(DartRuntimePrimitives.RequireValue(slot__as28001)));
            return;
        }
        if (slot is global::Doroti.Framework.Widgets.IndexedSlot<global::Doroti.Framework.Widgets.Element?>)
        {
            global::Doroti.Framework.Widgets.IndexedSlot<global::Doroti.Framework.Widgets.Element?> slot__as28229 = (global::Doroti.Framework.Widgets.IndexedSlot<global::Doroti.Framework.Widgets.Element?>)slot;
            DartRuntimePrimitives.Assert(() => renderObject.debugValidateChild(child));
            renderObject.insert(((global::Doroti.Framework.Rendering.RenderBox?)child)!, after: ((global::Doroti.Framework.Rendering.RenderBox?)slot__as28229.value?.renderObject)!);
            return;
        }
        DartRuntimePrimitives.Assert(() => false, () => (object?)"slot must be _CupertinoTextSelectionToolbarItemsSlot or IndexedSlot");
    }

    public override void moveRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? oldSlot, object? newSlot)
    {
        var targetSlot = newSlot as global::Doroti.Framework.Widgets.IndexedSlot<global::Doroti.Framework.Widgets.Element?>
            ?? throw new ArgumentException("Toolbar children require an indexed slot.", nameof(newSlot));
        renderObject.move((global::Doroti.Framework.Rendering.RenderBox)child,
            after: (global::Doroti.Framework.Rendering.RenderBox?)targetSlot.value?.renderObject);
    }

    internal static bool _shouldPaint(global::Doroti.Framework.Widgets.Element child)
    {
        return ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)child.renderObject!.parentData!)!.shouldPaint;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void removeRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        if (slot is _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar)
        {
            _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar slot__as29126 = (_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar)slot;
            DartRuntimePrimitives.Assert(() => child is global::Doroti.Framework.Rendering.RenderBox);
            DartRuntimePrimitives.Assert(() => renderObject.slottedChildren.ContainsKey(DartRuntimePrimitives.RequireValue(slot__as29126)));
            _updateRenderObject(null, DartRuntimePrimitives.RequireValue(slot__as29126));
            DartRuntimePrimitives.Assert(() => !renderObject.slottedChildren.ContainsKey(DartRuntimePrimitives.RequireValue(slot__as29126)));
            return;
        }
        DartRuntimePrimitives.Assert(() => slot is global::Doroti.Framework.Widgets.IndexedSlot<global::Doroti.Framework.Widgets.Element?>);
        DartRuntimePrimitives.Assert(() => Equals(child.parent, renderObject));
        renderObject.remove(((global::Doroti.Framework.Rendering.RenderBox?)child)!);
    }

    public override void visitChildren(global::System.Action<global::Doroti.Framework.Widgets.Element> visitor)
    {
        slotToChild.Values.forEach((__arg0) => visitor(__arg0));
        foreach (global::Doroti.Framework.Widgets.Element child in _children)
        {
            if (!_forgottenChildren.Contains(child))
            {
                visitor(child);
            }
        }
    }

    public override void forgetChild(global::Doroti.Framework.Widgets.Element child)
    {
        DartRuntimePrimitives.Assert(() => slotToChild.containsValue(child) || _children.Contains(child));
        DartRuntimePrimitives.Assert(() => !_forgottenChildren.Contains(child));
        if (child.slot is _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar slotLocal && slotToChild.ContainsKey(slotLocal))
        {
            slotToChild.remove(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(slotLocal)));
        }
        else
        {
            _forgottenChildren.Add(child);
        }
        base.forgetChild(child);
    }

    internal virtual void _mountChild(global::Doroti.Framework.Widgets.Widget widget, _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar slot)
    {
        global::Doroti.Framework.Widgets.Element? oldChild = slotToChild.GetValueOrDefault(DartRuntimePrimitives.RequireValue(slot));
        global::Doroti.Framework.Widgets.Element? newChild = updateChild(oldChild, widget, DartRuntimePrimitives.RequireValue(slot));
        if (oldChild is not null)
        {
            slotToChild.remove(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(slot)));
        }
        if (newChild is not null)
        {
            slotToChild[slot] = newChild;
        }
    }

    public override void mount(global::Doroti.Framework.Widgets.Element? parent, object? newSlot)
    {
        base.mount(parent, newSlot);
        var toolbarItems = ((_CupertinoTextSelectionToolbarItems__text_selection_toolbar?)widget)!;
        _mountChild(toolbarItems.backButton, _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar.backButton);
        _mountChild(toolbarItems.nextButton, _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar.nextButton);
        global::Doroti.Framework.Widgets.Element? previousChild = default!;
        _children = new List<global::Doroti.Framework.Widgets.Element>(Enumerable.Select(Enumerable.Range(0, checked((int)checked((long)toolbarItems.children.Count))), (i) =>
        {
            global::Doroti.Framework.Widgets.Element result = inflateWidget(toolbarItems.children[i], new global::Doroti.Framework.Widgets.IndexedSlot<global::Doroti.Framework.Widgets.Element?>(i, previousChild));
            previousChild = result;
            return result;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
    }

    public override void debugVisitOnstageChildren(global::System.Action<global::Doroti.Framework.Widgets.Element> visitor)
    {
        foreach (global::Doroti.Framework.Widgets.Element childLocal in slotToChild.Values)
        {
            if (_shouldPaint(childLocal) && !_forgottenChildren.Contains(childLocal))
            {
                visitor(childLocal);
            }
        }
        _children.where((child) => !_forgottenChildren.Contains(child) && _shouldPaint(child)).forEach((__arg0) => visitor(__arg0));
    }

    public override void update(global::Doroti.Framework.Widgets.Widget newWidget)
    {
        var __newWidget = (_CupertinoTextSelectionToolbarItems__text_selection_toolbar)newWidget;
        base.update(__newWidget);
        DartRuntimePrimitives.Assert(() => Equals(widget, __newWidget));
        var toolbarItems = ((_CupertinoTextSelectionToolbarItems__text_selection_toolbar?)widget)!;
        _mountChild(toolbarItems.backButton, _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar.backButton);
        _mountChild(toolbarItems.nextButton, _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar.nextButton);
        _children = updateChildren(_children, toolbarItems.children, forgottenChildren: _forgottenChildren);
        _forgottenChildren.Clear();
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
        _dividerColor = dividerColor;
        _dividerWidth = dividerWidth;
        _page = page;
    }

    internal virtual global::Doroti.Framework.Rendering.RenderBox? _updateChild(global::Doroti.Framework.Rendering.RenderBox? oldChild, global::Doroti.Framework.Rendering.RenderBox? newChild, _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar slot)
    {
        if (oldChild is not null)
        {
            dropChild(oldChild);
            slottedChildren.remove(slot);
        }
        if (newChild is not null)
        {
            slottedChildren[slot] = newChild;
            adoptChild(newChild);
        }
        return newChild;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long page
    {
        get => _page;
        set
        {
            var __value = value;
            if (DartRuntimePrimitives.RequireValue(__value) == _page)
            {
                return;
            }
            _page = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.Color dividerColor
    {
        get => _dividerColor;
        set
        {
            var __value = value;
            if (Equals(__value, _dividerColor))
            {
                return;
            }
            _dividerColor = __value;
            markNeedsLayout();
        }
    }
    public virtual double dividerWidth
    {
        get => _dividerWidth;
        set
        {
            var __value = value;
            if (DartRuntimePrimitives.RequireValue(__value) == _dividerWidth)
            {
                return;
            }
            _dividerWidth = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Rendering.RenderBox? backButton
    {
        get => _backButton;
        set
        {
            var __value = value;
            _backButton = _updateChild(_backButton, __value, DartRuntimePrimitives.RequireValue(_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar.backButton));
        }
    }
    public virtual global::Doroti.Framework.Rendering.RenderBox? nextButton
    {
        get => _nextButton;
        set
        {
            var __value = value;
            _nextButton = _updateChild(_nextButton, __value, DartRuntimePrimitives.RequireValue(_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar.nextButton));
        }
    }
    public override void performLayout()
    {
        if (firstChild is null)
        {
            size = constraints.smallest;
            return;
        }
        var greatestHeight = 0.0;
        visitChildren((renderObjectChild) =>
        {
            var child = ((global::Doroti.Framework.Rendering.RenderBox?)renderObjectChild)!;
            double childHeight = child.getMaxIntrinsicHeight(constraints.maxWidth);
            if (childHeight > greatestHeight)
            {
                greatestHeight = childHeight;
            }
        });
        var slottedConstraints = new global::Doroti.Framework.Rendering.BoxConstraints(maxWidth: constraints.maxWidth, minHeight: greatestHeight, maxHeight: greatestHeight);
        _backButton!.layout(slottedConstraints, parentUsesSize: true);
        _nextButton!.layout(slottedConstraints, parentUsesSize: true);
        double subsequentPageButtonsWidth = _backButton!.size.width + _nextButton!.size.width;
        var currentButtonPosition = 0.0;
        double toolbarWidth = default!;
        var currentPage = 0L;
        var i = -1L;
        visitChildren((renderObjectChild) =>
        {
            i++;
            var childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)renderObjectChild)!;
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)childLocal.parentData!)!;
            childParentData.shouldPaint = false;
            if (Equals(childLocal, _backButton) || Equals(childLocal, _nextButton) || (currentPage > _page))
            {
                return;
            }
            double paginationButtonsWidth = (currentPage == 0L) ? ((i == (childCount + 1L)) ? 0.0 : _nextButton!.size.width) : subsequentPageButtonsWidth;
            childLocal.layout(new global::Doroti.Framework.Rendering.BoxConstraints(maxWidth: constraints.maxWidth - paginationButtonsWidth, minHeight: greatestHeight, maxHeight: greatestHeight), parentUsesSize: true);
            double currentWidth = currentButtonPosition + paginationButtonsWidth + childLocal.size.width;
            if (currentWidth > constraints.maxWidth)
            {
                currentPage++;
                currentButtonPosition = _backButton!.size.width + dividerWidth;
                paginationButtonsWidth = _backButton!.size.width + _nextButton!.size.width;
                childLocal.layout(new global::Doroti.Framework.Rendering.BoxConstraints(maxWidth: constraints.maxWidth - paginationButtonsWidth, minHeight: greatestHeight, maxHeight: greatestHeight), parentUsesSize: true);
            }
            childParentData.offset = new global::Doroti.Ui.Offset(currentButtonPosition, 0.0);
            currentButtonPosition += childLocal.size.width + dividerWidth;
            childParentData.shouldPaint = currentPage == page;
            if (currentPage == page)
            {
                toolbarWidth = currentButtonPosition;
            }
        });
        DartRuntimePrimitives.Assert(() => page <= currentPage);
        if (currentPage > 0L)
        {
            var nextButtonParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)_nextButton!.parentData!)!;
            var backButtonParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)_backButton!.parentData!)!;
            if (page != currentPage)
            {
                nextButtonParentData.offset = new global::Doroti.Ui.Offset(toolbarWidth, 0.0);
                nextButtonParentData.shouldPaint = true;
                toolbarWidth += nextButton!.size.width;
            }
            if (page > 0L)
            {
                backButtonParentData.offset = Offset.zero;
                backButtonParentData.shouldPaint = true;
            }
        }
        else
        {
            toolbarWidth -= dividerWidth;
        }
        hasNextPage = page != currentPage;
        hasPreviousPage = page > 0L;
        size = constraints.constrain(new global::Doroti.Ui.Size(toolbarWidth, greatestHeight));
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        visitChildren((renderObjectChild) =>
        {
            var child = ((global::Doroti.Framework.Rendering.RenderBox?)renderObjectChild)!;
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)child.parentData!)!;
            if (childParentData.shouldPaint)
            {
                global::Doroti.Ui.Offset childOffset = childParentData.offset + offset;
                context.paintChild(child, childOffset);
                if ((childParentData.nextSibling is not null) || Equals(child, backButton))
                {
                    context.canvas.drawLine(new global::Doroti.Ui.Offset(child.size.width, 0) + childOffset, new global::Doroti.Ui.Offset(child.size.width, child.size.height) + childOffset, ((Func<Paint>)(() =>
            {
                var __cascade = new global::Doroti.Ui.Paint();
                __cascade.color = dividerColor;
                return __cascade;
            }))());
                }
            }
        });
    }

    public override void setupParentData(global::Doroti.Framework.Rendering.RenderObject child)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)child;
        if (__child.parentData is not ToolbarItemsParentData)
        {
            __child.parentData = new global::Doroti.Framework.Widgets.ToolbarItemsParentData();
        }
    }

    public static bool hitTestChild(global::Doroti.Framework.Rendering.RenderBox? child, global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        if (child is null)
        {
            return false;
        }
        var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)child.parentData!)!;
        if (!childParentData.shouldPaint)
        {
            return false;
        }
        return result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: (result, transformed) =>
        {
            DartRuntimePrimitives.Assert(() => Equals(transformed, position - childParentData.offset));
            return child.hitTest(result, position: transformed);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestChildren(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = lastChild;
        while (child is not null)
        {
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)child.parentData!)!;
            if (!childParentData.shouldPaint)
            {
                child = childParentData.previousSibling;
                continue;
            }
            if (hitTestChild(child, result, position: position))
            {
                return true;
            }
            child = childParentData.previousSibling;
        }
        if (hitTestChild(backButton, result, position: position))
        {
            return true;
        }
        if (hitTestChild(nextButton, result, position: position))
        {
            return true;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void attach(global::Doroti.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        global::Doroti.Framework.Rendering.RenderBox? child = _firstChild;
        while (child is not null)
        {
            child.attach(owner);
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
        foreach (global::Doroti.Framework.Rendering.RenderBox childLocal in slottedChildren.Values)
        {
            childLocal.attach(owner);
        }
    }

    public override void detach()
    {
        base.detach();
        global::Doroti.Framework.Rendering.RenderBox? child = _firstChild;
        while (child is not null)
        {
            child.detach();
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
        foreach (global::Doroti.Framework.Rendering.RenderBox childLocal in slottedChildren.Values)
        {
            childLocal.detach();
        }
    }

    public override void redepthChildren()
    {
        visitChildren((renderObjectChild) =>
        {
            var child = ((global::Doroti.Framework.Rendering.RenderBox?)renderObjectChild)!;
            redepthChild(child);
        });
    }

    public override void visitChildren(global::System.Action<global::Doroti.Framework.Rendering.RenderObject> visitor)
    {
        if (_backButton is not null)
        {
            visitor(_backButton!);
        }
        if (_nextButton is not null)
        {
            visitor(_nextButton!);
        }
        global::Doroti.Framework.Rendering.RenderBox? child = _firstChild;
        while (child is not null)
        {
            visitor(child);
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void visitChildrenForSemantics(global::System.Action<global::Doroti.Framework.Rendering.RenderObject> visitor)
    {
        visitChildren((renderObjectChild) =>
        {
            var child = ((global::Doroti.Framework.Rendering.RenderBox?)renderObjectChild)!;
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)child.parentData!)!;
            if (childParentData.shouldPaint)
            {
                visitor((global::Doroti.Framework.Rendering.RenderBox)renderObjectChild);
            }
        });
    }

    public override List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        var value = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        visitChildren((renderObjectChild) =>
        {
            var child = ((global::Doroti.Framework.Rendering.RenderBox?)renderObjectChild)!;
            if (Equals(child, backButton))
            {
                value.Add(((Diagnosticable)child).toDiagnosticsNode(name: "back button"));
            }
            else
            {
                if (Equals(child, nextButton))
                {
                    value.Add(((Diagnosticable)child).toDiagnosticsNode(name: "next button"));
                }
                else
                {
                    value.Add(((Diagnosticable)child).toDiagnosticsNode(name: "menu item"));
                }
            }
        });
        return value;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)child.parentData!)!;
        while (childParentData.previousSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.previousSibling, child));
            child = childParentData.previousSibling!;
            childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)child.parentData!)!;
        }
        return Equals(child, equals);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)child.parentData!)!;
        while (childParentData.nextSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.nextSibling, child));
            child = childParentData.nextSibling!;
            childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)child.parentData!)!;
        }
        return Equals(child, equals);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long childCount => _childCount;
    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (child is not RenderBox)
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"A {GetType()} expected a child of type {typeof(RenderBox)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new global::Doroti.Framework.Foundation.ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new global::Doroti.Framework.Foundation.ErrorSpacer(), new global::Doroti.Framework.Foundation.DiagnosticsProperty<object?>($"The {GetType()} that expected a {typeof(RenderBox)} child was created by", debugCreator, style: DiagnosticsTreeStyle.errorProperty), new global::Doroti.Framework.Foundation.ErrorSpacer(), new global::Doroti.Framework.Foundation.DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", child.debugCreator, style: DiagnosticsTreeStyle.errorProperty) }));
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _insertIntoChildList(RenderBox child, RenderBox? after = null)
    {
        var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => childParentData.nextSibling is null);
        DartRuntimePrimitives.Assert(() => childParentData.previousSibling is null);
        _childCount += 1L;
        DartRuntimePrimitives.Assert(() => _childCount > 0L);
        if (after is null)
        {
            childParentData.nextSibling = _firstChild;
            if (_firstChild is not null)
            {
                var firstChildParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)_firstChild!.parentData!)!;
                firstChildParentData.previousSibling = child;
            }
            _firstChild = child;
            _lastChild ??= child;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => _firstChild is not null);
            DartRuntimePrimitives.Assert(() => _lastChild is not null);
            DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(after, equals: _firstChild));
            DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(after, equals: _lastChild));
            var afterParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)after.parentData!)!;
            if (afterParentData.nextSibling is null)
            {
                DartRuntimePrimitives.Assert(() => Equals(after, _lastChild));
                childParentData.previousSibling = after;
                afterParentData.nextSibling = child;
                _lastChild = child;
            }
            else
            {
                childParentData.nextSibling = afterParentData.nextSibling;
                childParentData.previousSibling = after;
                var childPreviousSiblingParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)childParentData.previousSibling!.parentData!)!;
                var childNextSiblingParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)childParentData.nextSibling!.parentData!)!;
                childPreviousSiblingParentData.nextSibling = child;
                childNextSiblingParentData.previousSibling = child;
                DartRuntimePrimitives.Assert(() => Equals(afterParentData.nextSibling, child));
            }
        }
    }

    public virtual void insert(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => !Equals(child, this), () => (object?)"A RenderObject cannot be inserted into itself.");
        DartRuntimePrimitives.Assert(() => !Equals(after, this), () => (object?)"A RenderObject cannot simultaneously be both the parent and the sibling of another RenderObject.");
        DartRuntimePrimitives.Assert(() => !Equals(child, after), () => (object?)"A RenderObject cannot be inserted after itself.");
        DartRuntimePrimitives.Assert(() => !Equals(child, _firstChild));
        DartRuntimePrimitives.Assert(() => !Equals(child, _lastChild));
        adoptChild(child);
        DartRuntimePrimitives.Assert(() => child.parentData is global::Doroti.Framework.Widgets.ToolbarItemsParentData, () => (object?)$"A child of {GetType()} has parentData of type {DartRuntimePrimitives.RuntimeType(child.parentData)}, " + $"which does not conform to {(typeof(global::Doroti.Framework.Widgets.ToolbarItemsParentData))}. Class using ContainerRenderObjectMixin " + $"should override setupParentData() to set parentData to type {(typeof(global::Doroti.Framework.Widgets.ToolbarItemsParentData))}.");
        _insertIntoChildList(child, after: after);
    }

    public virtual void add(RenderBox child)
    {
        insert(child, after: _lastChild);
    }

    public virtual void addAll(List<RenderBox>? children)
    {
        children?.forEach((__arg0) => ((global::System.Action<RenderBox>)add)(__arg0));
    }

    public virtual void _removeFromChildList(RenderBox child)
    {
        var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(child, equals: _firstChild));
        DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(child, equals: _lastChild));
        DartRuntimePrimitives.Assert(() => _childCount >= 0L);
        if (childParentData.previousSibling is null)
        {
            DartRuntimePrimitives.Assert(() => Equals(_firstChild, child));
            _firstChild = childParentData.nextSibling;
        }
        else
        {
            var childPreviousSiblingParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)childParentData.previousSibling!.parentData!)!;
            childPreviousSiblingParentData.nextSibling = childParentData.nextSibling;
        }
        if (childParentData.nextSibling is null)
        {
            DartRuntimePrimitives.Assert(() => Equals(_lastChild, child));
            _lastChild = childParentData.previousSibling;
        }
        else
        {
            var childNextSiblingParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)childParentData.nextSibling!.parentData!)!;
            childNextSiblingParentData.previousSibling = childParentData.previousSibling;
        }
        childParentData.previousSibling = null;
        childParentData.nextSibling = null;
        _childCount -= 1L;
    }

    public virtual void remove(RenderBox child)
    {
        _removeFromChildList(child);
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)child.parentData!)!;
            RenderBox? next = childParentData.nextSibling;
            childParentData.previousSibling = null;
            childParentData.nextSibling = null;
            dropChild(child);
            child = next;
        }
        _firstChild = null;
        _lastChild = null;
        _childCount = 0L;
    }

    public virtual void move(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => !Equals(child, this));
        DartRuntimePrimitives.Assert(() => !Equals(after, this));
        DartRuntimePrimitives.Assert(() => !Equals(child, after));
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)child.parentData!)!;
        if (Equals(childParentData.previousSibling, after))
        {
            return;
        }
        _removeFromChildList(child);
        _insertIntoChildList(child, after: after);
        markNeedsLayout();
    }

    public virtual RenderBox? firstChild => _firstChild;
    public virtual RenderBox? lastChild => _lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)child.parentData!)!;
        return childParentData.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToFirstActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)child.parentData!)!;
            double? result = child.getDistanceToActualBaseline(baseline);
            if (result is not null)
            {
                double result__138852__value138916 = DartRuntimePrimitives.RequireValue(result);
                return DartRuntimePrimitives.RequireValue(result__138852__value138916) + childParentData.offset.dy;
            }
            child = childParentData.nextSibling;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToHighestActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        BaselineOffset minBaseline = BaselineOffset.noBaseline;
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)child.parentData!)!;
            BaselineOffset candidate = new BaselineOffset(child.getDistanceToActualBaseline(baseline)).op_Add(childParentData.offset.dy);
            minBaseline = minBaseline.minOf(candidate);
            child = childParentData.nextSibling;
        }
        return minBaseline.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool defaultHitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child = lastChild;
        while (child is not null)
        {
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)child.parentData!)!;
            bool isHit = result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: (result, transformed) =>
            {
                DartRuntimePrimitives.Assert(() => Equals(transformed, position - childParentData.offset));
                return child!.hitTest(result, position: transformed);
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
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
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)child.parentData!)!;
            context.paintChild(child, childParentData.offset + offset);
            child = childParentData.nextSibling;
        }
    }

    public virtual List<RenderBox> getChildrenAsList()
    {
        var result = new List<RenderBox>();
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)child.parentData!)!;
            result.Add(child!);
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
