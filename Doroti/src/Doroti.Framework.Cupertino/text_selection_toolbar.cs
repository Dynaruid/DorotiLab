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
    internal static Size _kToolbarArrowSize = new Size(14.0, 7.0);
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
    internal static CupertinoDynamicColor _kToolbarBackgroundColor = new CupertinoDynamicColor(color: new Color(4294375158L), darkColor: new Color(4280427042L));
}

public static partial class Text_selection_toolbarLibrary
{
    internal static CupertinoDynamicColor _kToolbarDividerColor = new CupertinoDynamicColor(color: new Color(4292269782L), darkColor: new Color(4282532418L));
}

public static partial class Text_selection_toolbarLibrary
{
    internal static CupertinoDynamicColor _kToolbarTextColor = new CupertinoDynamicColor(color: CupertinoColors.black, darkColor: CupertinoColors.white);
}

public static partial class Text_selection_toolbarLibrary
{
    internal static Duration _kToolbarTransitionDuration = Duration.Create(milliseconds: 125L);
}

public delegate Widget CupertinoToolbarBuilder(BuildContext context, Offset anchorAbove, Offset anchorBelow, Widget child);

public class CupertinoTextSelectionToolbar : StatelessWidget
{
    public virtual Offset anchorAbove { get; private set; } = default!;
    public virtual Offset anchorBelow { get; private set; } = default!;
    public virtual List<Widget> children { get; private set; } = default!;
    public virtual Func<BuildContext, Offset, Offset, Widget, Widget> toolbarBuilder { get; private set; } = default!;
    public const double kToolbarScreenPadding = 8.0;

    public CupertinoTextSelectionToolbar(Key? key = null, Offset anchorAbove = default!, Offset anchorBelow = default!, List<Widget> children = default!, Func<BuildContext, Offset, Offset, Widget, Widget> toolbarBuilder = default!) : base(key: key)
    {
        Func<BuildContext, Offset, Offset, Widget, Widget> __toolbarBuilder = toolbarBuilder ?? _defaultToolbarBuilder;
        this.anchorAbove = anchorAbove;
        this.anchorBelow = anchorBelow;
        this.children = children;
        this.toolbarBuilder = __toolbarBuilder;
        System.Diagnostics.Debug.Assert(checked(children.Count) > 0L);
    }

    internal static Widget _defaultToolbarBuilder(BuildContext context, Offset anchorAbove, Offset anchorBelow, Widget child)
    {
        return new _CupertinoTextSelectionToolbarShape__text_selection_toolbar(anchorAbove: anchorAbove, anchorBelow: anchorBelow, shadowColor: Equals(CupertinoTheme.brightnessOf(context), Brightness.light) ? CupertinoColors.black.withOpacity(0.2) : null, child: new ColoredBox(color: Text_selection_toolbarLibrary._kToolbarBackgroundColor.resolveFrom(context), child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        EdgeInsets mediaQueryPadding = MediaQuery.paddingOf(context);
        double paddingAbove = mediaQueryPadding.top + kToolbarScreenPadding;
        double leftMargin = Text_selection_toolbarLibrary._kArrowScreenPadding + mediaQueryPadding.left;
        double rightMargin = MediaQuery.widthOf(context) - mediaQueryPadding.right - Text_selection_toolbarLibrary._kArrowScreenPadding;
        var anchorAboveAdjusted = new Offset(Dart_uiLibrary.clampDouble(anchorAbove.dx, leftMargin, rightMargin), anchorAbove.dy - Text_selection_toolbarLibrary._kToolbarContentDistance - paddingAbove);
        var anchorBelowAdjusted = new Offset(Dart_uiLibrary.clampDouble(anchorBelow.dx, leftMargin, rightMargin), anchorBelow.dy + Text_selection_toolbarLibrary._kToolbarContentDistance - paddingAbove);
        return new Padding(padding: new EdgeInsets(kToolbarScreenPadding, paddingAbove, kToolbarScreenPadding, kToolbarScreenPadding), child: new CustomSingleChildLayout(@delegate: new TextSelectionToolbarLayoutDelegate(anchorAbove: anchorAboveAdjusted, anchorBelow: anchorBelowAdjusted), child: new _CupertinoTextSelectionToolbarContent__text_selection_toolbar(anchorAbove: anchorAboveAdjusted, anchorBelow: anchorBelowAdjusted, toolbarBuilder: toolbarBuilder, children: children)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CupertinoTextSelectionToolbarShape__text_selection_toolbar : SingleChildRenderObjectWidget
{
    internal virtual Offset _anchorAbove { get; private set; } = default!;
    internal virtual Offset _anchorBelow { get; private set; } = default!;
    internal virtual Color? _shadowColor { get; private set; }

    internal _CupertinoTextSelectionToolbarShape__text_selection_toolbar(Offset anchorAbove, Offset anchorBelow, Color? shadowColor = null, Widget? child = null) : base(child: child)
    {
        _anchorAbove = anchorAbove;
        _anchorBelow = anchorBelow;
        _shadowColor = shadowColor;
    }

    public override RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<RenderObject>(new _RenderCupertinoTextSelectionToolbarShape__text_selection_toolbar(_anchorAbove, _anchorBelow, _shadowColor, null));
    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
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

public class _RenderCupertinoTextSelectionToolbarShape__text_selection_toolbar : RenderShiftedBox
{
    internal virtual Offset _anchorAbove { get; set; } = default!;
    internal virtual Offset _anchorBelow { get; set; } = default!;
    internal virtual Color? _shadowColor { get; set; } = default;
    internal virtual LayerHandle<ClipPathLayer> _clipPathLayer { get; private set; } = new LayerHandle<ClipPathLayer>();
    internal virtual Paint? _debugPaint { get; set; } = default;

    internal _RenderCupertinoTextSelectionToolbarShape__text_selection_toolbar(Offset _anchorAbove, Offset _anchorBelow, Color? _shadowColor, RenderBox? child) : base(child)
    {
        this._anchorAbove = _anchorAbove;
        this._anchorBelow = _anchorBelow;
        this._shadowColor = _shadowColor;
    }

    public override bool isRepaintBoundary => true;
    public virtual Offset anchorAbove
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
    public virtual Offset anchorBelow
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
    public virtual Color? shadowColor
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
    internal virtual BoxConstraints _constraintsForChild(BoxConstraints constraints)
    {
        return new BoxConstraints(minWidth: Text_selection_toolbarLibrary._kToolbarArrowSize.width + (Text_selection_toolbarLibrary._kToolbarBorderRadius.x * 2L)).enforce(constraints.loosen());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Offset _computeChildOffset(Size childSize)
    {
        return new Offset(0.0, _isAbove(childSize.height) ? -Text_selection_toolbarLibrary._kToolbarArrowSize.height : 0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return null;
        }
        BoxConstraints enforcedConstraint = _constraintsForChild(constraints);
        double? result = childLocal.getDryBaseline(enforcedConstraint, baseline);
        return (result is null) ? null : (DartRuntimePrimitives.RequireValue(result) + _computeChildOffset(childLocal.getDryLayout(enforcedConstraint)).dy);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return;
        }
        childLocal.layout(_constraintsForChild(constraints), parentUsesSize: true);
        var childParentData = ((BoxParentData?)childLocal.parentData!)!;
        childParentData.offset = _computeChildOffset(childLocal.size);
        size = new Size(childLocal.size.width, childLocal.size.height - Text_selection_toolbarLibrary._kToolbarArrowSize.height);
    }

    internal virtual RRect _shapeRRect(RenderBox child)
    {
        Rect rect = new Offset(0.0, Text_selection_toolbarLibrary._kToolbarArrowSize.height) & new Size(child.size.width, child.size.height - (Text_selection_toolbarLibrary._kToolbarArrowSize.height * 2L));
        return RRect.fromRectAndRadius(rect, Text_selection_toolbarLibrary._kToolbarBorderRadius).scaleRadii();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static Path _addRRectToPath(Path path, RRect rrect, double startAngle)
    {
        double halfPI = Dart_mathLibrary.pi / 2L;
        DartRuntimePrimitives.Assert(() => (startAngle % halfPI) == 0.0);
        Rect rect = rrect.outerRect;
        var rrectCorners = new List<(Offset, Radius)> { (rect.bottomRight, -rrect.brRadius), (rect.bottomLeft, Radius.elliptical(rrect.blRadiusX, -rrect.blRadiusY)), (rect.topLeft, rrect.tlRadius), (rect.topRight, Radius.elliptical(-rrect.trRadiusX, rrect.trRadiusY)) };
        long startQuadrantIndex = checked((long)(startAngle / halfPI));
        for (var i = startQuadrantIndex; i < (checked(rrectCorners.Count) + startQuadrantIndex); i += 1L)
        {
            // Dart modulo stays non-negative. A toolbar below the selection
            // starts at quadrant -1, whose corner is the top-right corner.
            var cornerIndex = (int)((i % rrectCorners.Count + rrectCorners.Count) % rrectCorners.Count);
            var (vertex, rectCenterOffset) = rrectCorners[cornerIndex];
            var otherVertex = new Offset(vertex.dx + (2L * rectCenterOffset.x), vertex.dy + (2L * rectCenterOffset.y));
            var rectLocal = Rect.fromPoints(vertex, otherVertex);
            path.arcTo(rectLocal, halfPI * i, halfPI, false);
        }
        return path;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Path _clipPath(RenderBox child, RRect rrect)
    {
        var path = new Path();
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
        Offset localAnchor = globalToLocal(isAbove ? _anchorAbove : _anchorBelow);
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

    public override void paint(PaintingContext context, Offset offset)
    {
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return;
        }
        var childParentData = ((BoxParentData?)childLocal.parentData!)!;
        RRect rrect = _shapeRRect(childLocal);
        Path clipPath = _clipPath(childLocal, rrect);
        if (_shadowColor is not null)
        {
            var boxShadow = new BoxShadow(color: _shadowColor!, blurRadius: 15.0);
            RRect shadowRRect = RRect.fromLTRBR(rrect.left, rrect.top, rrect.right, rrect.bottom + Text_selection_toolbarLibrary._kToolbarArrowSize.height, Text_selection_toolbarLibrary._kToolbarBorderRadius).shift(offset + childParentData.offset + boxShadow.offset);
            context.canvas.drawRRect(shadowRRect, boxShadow.toPaint());
        }
        _clipPathLayer.layer = context.pushClipPath(needsCompositing, offset + childParentData.offset, Offset.zero & childLocal.size, clipPath, (innerContext, innerOffset) => { innerContext.paintChild(childLocal, innerOffset); }, oldLayer: _clipPathLayer.layer);
    }

    public override void dispose()
    {
        _clipPathLayer.layer = null;
        base.dispose();
    }

    public override void debugPaintSize(PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                RenderBox? childLocal = child;
                if (childLocal is null)
                {
                    return true;
                }
                Paint debugPaint = _debugPaint ??= ((Func<Paint>)(() =>
{
    var __cascade = new Paint();
    __cascade.shader = Ui.Gradient.linear(Offset.zero, new Offset(10.0, 10.0), new List<Color> { CupertinoColors.transparent, new Color(4294902015L), new Color(4294902015L), CupertinoColors.transparent }, new List<double> { 0.25, 0.25, 0.75, 0.75 }, TileMode.repeated);
    __cascade.strokeWidth = 2.0;
    __cascade.style = PaintingStyle.stroke;
    return __cascade;
}))();
                var childParentData = ((BoxParentData?)childLocal.parentData!)!;
                Path clipPath = _clipPath(childLocal, _shapeRRect(childLocal));
                context.canvas.drawPath(clipPath.shift(offset + childParentData.offset), debugPaint);
                return true;
            });
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return false;
        }
        var childParentData = ((BoxParentData?)childLocal.parentData!)!;
        var hitBox = Rect.fromLTWH(childParentData.offset.dx, childParentData.offset.dy + Text_selection_toolbarLibrary._kToolbarArrowSize.height, childLocal.size.width, childLocal.size.height - (Text_selection_toolbarLibrary._kToolbarArrowSize.height * 2L));
        if (!hitBox.contains(position))
        {
            return false;
        }
        return base.hitTestChildren(result, position: position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _CupertinoTextSelectionToolbarContent__text_selection_toolbar : StatefulWidget
{
    public virtual Offset anchorAbove { get; private set; } = default!;
    public virtual Offset anchorBelow { get; private set; } = default!;
    public virtual List<Widget> children { get; private set; } = default!;
    public virtual Func<BuildContext, Offset, Offset, Widget, Widget> toolbarBuilder { get; private set; } = default!;

    internal _CupertinoTextSelectionToolbarContent__text_selection_toolbar(Offset anchorAbove, Offset anchorBelow, Func<BuildContext, Offset, Offset, Widget, Widget> toolbarBuilder, List<Widget> children)
    {
        this.anchorAbove = anchorAbove;
        this.anchorBelow = anchorBelow;
        this.toolbarBuilder = toolbarBuilder;
        this.children = children;
        System.Diagnostics.Debug.Assert(checked(children.Count) > 0L);
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoTextSelectionToolbarContentState__text_selection_toolbar());
}

public class _CupertinoTextSelectionToolbarContentState__text_selection_toolbar : State<_CupertinoTextSelectionToolbarContent__text_selection_toolbar>, TickerProviderStateMixin<_CupertinoTextSelectionToolbarContent__text_selection_toolbar>
{
    internal virtual AnimationController _controller { get; set; } = default!;
    internal virtual long? _nextPage { get; set; } = default;
    internal virtual long _page { get; set; } = 0L;
    internal virtual GlobalKey<IState> _toolbarItemsKey { get; private set; } = GlobalKey<IState>.Create();
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual void _onHorizontalDragEnd(Gestures.DragEndDetails details)
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
        var renderToolbar = ((RenderBox?)_toolbarItemsKey.currentContext?.findRenderObject())!;
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
        var renderToolbar = ((RenderBox?)_toolbarItemsKey.currentContext?.findRenderObject())!;
        if ((renderToolbar is _RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar) && ((_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar)renderToolbar).hasPreviousPage)
        {
            _RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar renderToolbar__19891__as19983 = (_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar)renderToolbar;
            _controller.reverse();
            _controller.addStatusListener(_statusListener);
            _nextPage = _page - 1L;
        }
    }

    internal virtual void _statusListener(AnimationStatus status)
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
        _controller = new AnimationController(value: 1.0, vsync: this, duration: Text_selection_toolbarLibrary._kToolbarTransitionDuration);
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
                    foreach (Scheduler.Ticker ticker in _tickers!)
                    {
                        if (ticker.isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{this} was disposed with an active Ticker."), new ErrorDescription($"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        Color chevronColor = Text_selection_toolbarLibrary._kToolbarTextColor.resolveFrom(context);
        Widget backButtonLocal = new Center(widthFactor: 1.0, heightFactor: 1.0, child: new CupertinoTextSelectionToolbarButton(onPressed: () => _handlePreviousPage(), child: new IgnorePointer(child: new CustomPaint(painter: new _LeftCupertinoChevronPainter__text_selection_toolbar(color: chevronColor), size: new Size(Text_selection_toolbarLibrary._kToolbarChevronSize)))));
        Widget nextButtonLocal = new Center(widthFactor: 1.0, heightFactor: 1.0, child: new CupertinoTextSelectionToolbarButton(onPressed: () => _handleNextPage(), child: new IgnorePointer(child: new CustomPaint(painter: new _RightCupertinoChevronPainter__text_selection_toolbar(color: chevronColor), size: new Size(Text_selection_toolbarLibrary._kToolbarChevronSize)))));
        List<Widget> childrenLocal = widget.children.map((child) =>
        {
            return new Center(widthFactor: 1.0, heightFactor: 1.0, child: child);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }).ToList().Cast<Widget>().ToList();
        return widget.toolbarBuilder(context, widget.anchorAbove, widget.anchorBelow, new FadeTransition(opacity: _controller, child: new AnimatedSize(duration: Text_selection_toolbarLibrary._kToolbarTransitionDuration, curve: Curves.decelerate, child: new GestureDetector(onHorizontalDragEnd: _onHorizontalDragEnd, child: new _CupertinoTextSelectionToolbarItems__text_selection_toolbar(key: _toolbarItemsKey, page: _page, backButton: backButtonLocal, dividerColor: Text_selection_toolbarLibrary._kToolbarDividerColor.resolveFrom(context), dividerWidth: 1.0 / MediaQuery.devicePixelRatioOf(context), nextButton: nextButtonLocal, children: childrenLocal)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Scheduler.Ticker createTicker(Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = ((Func<_WidgetTicker__ticker_provider>)(() =>
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

    public virtual void _removeTicker(_WidgetTicker__ticker_provider ticker)
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
            foreach (Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<HashSet<Scheduler.Ticker>>("tickers", _tickers, description: (_tickers is not null) ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}" : null, defaultValue: default));
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

internal abstract class _CupertinoChevronPainter__text_selection_toolbar : CustomPainter
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
        var centerOffset = new Offset(iconSize / 4L * (isLeft ? 1L : -1L), 0);
        Offset firstPoint = new Offset(iconSize / 2L, 0) + centerOffset;
        Offset middlePoint = new Offset(isLeft ? 0 : iconSize, iconSize / 2L) + centerOffset;
        Offset lowerPoint = new Offset(iconSize / 2L, iconSize) + centerOffset;
        var paintLocal = ((Func<Paint>)(() =>
{
    var __cascade = new Paint();
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

    public override bool shouldRepaint(CustomPainter oldDelegate) => DartRuntimePrimitives.ConvertValue<bool>((!Equals(((_CupertinoChevronPainter__text_selection_toolbar)oldDelegate).color, color)) || (((_CupertinoChevronPainter__text_selection_toolbar)oldDelegate).isLeft != isLeft));
}

public class _CupertinoTextSelectionToolbarItems__text_selection_toolbar : RenderObjectWidget
{
    public virtual Widget backButton { get; private set; } = default!;
    public virtual List<Widget> children { get; private set; } = default!;
    public virtual Color dividerColor { get; private set; } = default!;
    public virtual double dividerWidth { get; private set; } = default!;
    public virtual Widget nextButton { get; private set; } = default!;
    public virtual long page { get; private set; } = default!;

    internal _CupertinoTextSelectionToolbarItems__text_selection_toolbar(Key? key = null, long page = default!, List<Widget> children = default!, Widget backButton = default!, Color dividerColor = default!, double dividerWidth = default!, Widget nextButton = default!) : base(key: key)
    {
        this.page = page;
        this.children = children;
        this.backButton = backButton;
        this.dividerColor = dividerColor;
        this.dividerWidth = dividerWidth;
        this.nextButton = nextButton;
        System.Diagnostics.Debug.Assert(Enumerable.Any(children));
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar(dividerColor: dividerColor, dividerWidth: dividerWidth, page: page);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
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

public class _CupertinoTextSelectionToolbarItemsElement__text_selection_toolbar : RenderObjectElement
{
    internal virtual List<Element> _children { get; set; } = default!;
    public virtual DartMap<_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar, Element> slotToChild { get; private set; } = new DartMap<_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar, Element>();
    internal virtual HashSet<Element> _forgottenChildren { get; private set; } = new HashSet<Element>();

    internal _CupertinoTextSelectionToolbarItemsElement__text_selection_toolbar(_CupertinoTextSelectionToolbarItems__text_selection_toolbar widget) : base(widget)
    {
    }

    public override _RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar renderObject => DartRuntimePrimitives.ConvertValue<_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar>(((_RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar?)base.renderObject)!);
    internal virtual void _updateRenderObject(RenderBox? child, _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar slot)
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

    public override void insertRenderObjectChild(RenderObject child, object? slot)
    {
        if (slot is _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar)
        {
            _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar slot__as28001 = (_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar)slot;
            DartRuntimePrimitives.Assert(() => child is RenderBox);
            _updateRenderObject(((RenderBox?)child)!, DartRuntimePrimitives.RequireValue(slot__as28001));
            DartRuntimePrimitives.Assert(() => renderObject.slottedChildren.ContainsKey(DartRuntimePrimitives.RequireValue(slot__as28001)));
            return;
        }
        if (slot is IndexedSlot<Element?>)
        {
            IndexedSlot<Element?> slot__as28229 = (IndexedSlot<Element?>)slot;
            DartRuntimePrimitives.Assert(() => renderObject.debugValidateChild(child));
            renderObject.insert(((RenderBox?)child)!, after: ((RenderBox?)slot__as28229.value?.renderObject)!);
            return;
        }
        DartRuntimePrimitives.Assert(() => false, () => (object?)"slot must be _CupertinoTextSelectionToolbarItemsSlot or IndexedSlot");
    }

    public override void moveRenderObjectChild(RenderObject child, object? oldSlot, object? newSlot)
    {
        var targetSlot = newSlot as IndexedSlot<Element?>
            ?? throw new ArgumentException("Toolbar children require an indexed slot.", nameof(newSlot));
        renderObject.move((RenderBox)child,
            after: (RenderBox?)targetSlot.value?.renderObject);
    }

    internal static bool _shouldPaint(Element child)
    {
        return ((ToolbarItemsParentData?)child.renderObject!.parentData!)!.shouldPaint;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void removeRenderObjectChild(RenderObject child, object? slot)
    {
        if (slot is _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar)
        {
            _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar slot__as29126 = (_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar)slot;
            DartRuntimePrimitives.Assert(() => child is RenderBox);
            DartRuntimePrimitives.Assert(() => renderObject.slottedChildren.ContainsKey(DartRuntimePrimitives.RequireValue(slot__as29126)));
            _updateRenderObject(null, DartRuntimePrimitives.RequireValue(slot__as29126));
            DartRuntimePrimitives.Assert(() => !renderObject.slottedChildren.ContainsKey(DartRuntimePrimitives.RequireValue(slot__as29126)));
            return;
        }
        DartRuntimePrimitives.Assert(() => slot is IndexedSlot<Element?>);
        DartRuntimePrimitives.Assert(() => Equals(child.parent, renderObject));
        renderObject.remove(((RenderBox?)child)!);
    }

    public override void visitChildren(Action<Element> visitor)
    {
        slotToChild.Values.forEach((__arg0) => visitor(__arg0));
        foreach (Element child in _children)
        {
            if (!_forgottenChildren.Contains(child))
            {
                visitor(child);
            }
        }
    }

    public override void forgetChild(Element child)
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

    internal virtual void _mountChild(Widget widget, _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar slot)
    {
        Element? oldChild = slotToChild.GetValueOrDefault(DartRuntimePrimitives.RequireValue(slot));
        Element? newChild = updateChild(oldChild, widget, DartRuntimePrimitives.RequireValue(slot));
        if (oldChild is not null)
        {
            slotToChild.remove(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(slot)));
        }
        if (newChild is not null)
        {
            slotToChild[slot] = newChild;
        }
    }

    public override void mount(Element? parent, object? newSlot)
    {
        base.mount(parent, newSlot);
        var toolbarItems = ((_CupertinoTextSelectionToolbarItems__text_selection_toolbar?)widget)!;
        _mountChild(toolbarItems.backButton, _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar.backButton);
        _mountChild(toolbarItems.nextButton, _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar.nextButton);
        Element? previousChild = default!;
        _children = new List<Element>(Enumerable.Select(Enumerable.Range(0, checked((int)checked((long)toolbarItems.children.Count))), (i) =>
        {
            Element result = inflateWidget(toolbarItems.children[i], new IndexedSlot<Element?>(i, previousChild));
            previousChild = result;
            return result;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
    }

    public override void debugVisitOnstageChildren(Action<Element> visitor)
    {
        foreach (Element childLocal in slotToChild.Values)
        {
            if (_shouldPaint(childLocal) && !_forgottenChildren.Contains(childLocal))
            {
                visitor(childLocal);
            }
        }
        _children.where((child) => !_forgottenChildren.Contains(child) && _shouldPaint(child)).forEach((__arg0) => visitor(__arg0));
    }

    public override void update(Widget newWidget)
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

public class _RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar : RenderBox, ContainerRenderObjectMixin<RenderBox, ToolbarItemsParentData>, RenderBoxContainerDefaultsMixin<RenderBox, ToolbarItemsParentData>
{
    public virtual DartMap<_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar, RenderBox> slottedChildren { get; private set; } = new DartMap<_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar, RenderBox>();
    public virtual bool hasNextPage { get; set; } = default!;
    public virtual bool hasPreviousPage { get; set; } = default!;
    internal virtual long _page { get; set; } = default!;
    internal virtual Color _dividerColor { get; set; } = default!;
    internal virtual double _dividerWidth { get; set; } = default!;
    internal virtual RenderBox? _backButton { get; set; } = default;
    internal virtual RenderBox? _nextButton { get; set; } = default;
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;

    internal _RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar(Color dividerColor, double dividerWidth, long page)
    {
        _dividerColor = dividerColor;
        _dividerWidth = dividerWidth;
        _page = page;
    }

    internal virtual RenderBox? _updateChild(RenderBox? oldChild, RenderBox? newChild, _CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar slot)
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
    public virtual Color dividerColor
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
    public virtual RenderBox? backButton
    {
        get => _backButton;
        set
        {
            var __value = value;
            _backButton = _updateChild(_backButton, __value, DartRuntimePrimitives.RequireValue(_CupertinoTextSelectionToolbarItemsSlot__text_selection_toolbar.backButton));
        }
    }
    public virtual RenderBox? nextButton
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
            var child = ((RenderBox?)renderObjectChild)!;
            double childHeight = child.getMaxIntrinsicHeight(constraints.maxWidth);
            if (childHeight > greatestHeight)
            {
                greatestHeight = childHeight;
            }
        });
        var slottedConstraints = new BoxConstraints(maxWidth: constraints.maxWidth, minHeight: greatestHeight, maxHeight: greatestHeight);
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
            var childLocal = ((RenderBox?)renderObjectChild)!;
            var childParentData = ((ToolbarItemsParentData?)childLocal.parentData!)!;
            childParentData.shouldPaint = false;
            if (Equals(childLocal, _backButton) || Equals(childLocal, _nextButton) || (currentPage > _page))
            {
                return;
            }
            double paginationButtonsWidth = (currentPage == 0L) ? ((i == (childCount + 1L)) ? 0.0 : _nextButton!.size.width) : subsequentPageButtonsWidth;
            childLocal.layout(new BoxConstraints(maxWidth: constraints.maxWidth - paginationButtonsWidth, minHeight: greatestHeight, maxHeight: greatestHeight), parentUsesSize: true);
            double currentWidth = currentButtonPosition + paginationButtonsWidth + childLocal.size.width;
            if (currentWidth > constraints.maxWidth)
            {
                currentPage++;
                currentButtonPosition = _backButton!.size.width + dividerWidth;
                paginationButtonsWidth = _backButton!.size.width + _nextButton!.size.width;
                childLocal.layout(new BoxConstraints(maxWidth: constraints.maxWidth - paginationButtonsWidth, minHeight: greatestHeight, maxHeight: greatestHeight), parentUsesSize: true);
            }
            childParentData.offset = new Offset(currentButtonPosition, 0.0);
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
            var nextButtonParentData = ((ToolbarItemsParentData?)_nextButton!.parentData!)!;
            var backButtonParentData = ((ToolbarItemsParentData?)_backButton!.parentData!)!;
            if (page != currentPage)
            {
                nextButtonParentData.offset = new Offset(toolbarWidth, 0.0);
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
        size = constraints.constrain(new Size(toolbarWidth, greatestHeight));
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        visitChildren((renderObjectChild) =>
        {
            var child = ((RenderBox?)renderObjectChild)!;
            var childParentData = ((ToolbarItemsParentData?)child.parentData!)!;
            if (childParentData.shouldPaint)
            {
                Offset childOffset = childParentData.offset + offset;
                context.paintChild(child, childOffset);
                if ((childParentData.nextSibling is not null) || Equals(child, backButton))
                {
                    context.canvas.drawLine(new Offset(child.size.width, 0) + childOffset, new Offset(child.size.width, child.size.height) + childOffset, ((Func<Paint>)(() =>
            {
                var __cascade = new Paint();
                __cascade.color = dividerColor;
                return __cascade;
            }))());
                }
            }
        });
    }

    public override void setupParentData(RenderObject child)
    {
        var __child = (RenderBox)child;
        if (__child.parentData is not ToolbarItemsParentData)
        {
            __child.parentData = new ToolbarItemsParentData();
        }
    }

    public static bool hitTestChild(RenderBox? child, BoxHitTestResult result, Offset position)
    {
        if (child is null)
        {
            return false;
        }
        var childParentData = ((ToolbarItemsParentData?)child.parentData!)!;
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

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child = lastChild;
        while (child is not null)
        {
            var childParentData = ((ToolbarItemsParentData?)child.parentData!)!;
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

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            child.attach(owner);
            var childParentData = ((ToolbarItemsParentData?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
        foreach (RenderBox childLocal in slottedChildren.Values)
        {
            childLocal.attach(owner);
        }
    }

    public override void detach()
    {
        base.detach();
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            child.detach();
            var childParentData = ((ToolbarItemsParentData?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
        foreach (RenderBox childLocal in slottedChildren.Values)
        {
            childLocal.detach();
        }
    }

    public override void redepthChildren()
    {
        visitChildren((renderObjectChild) =>
        {
            var child = ((RenderBox?)renderObjectChild)!;
            redepthChild(child);
        });
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        if (_backButton is not null)
        {
            visitor(_backButton!);
        }
        if (_nextButton is not null)
        {
            visitor(_nextButton!);
        }
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            visitor(child);
            var childParentData = ((ToolbarItemsParentData?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        visitChildren((renderObjectChild) =>
        {
            var child = ((RenderBox?)renderObjectChild)!;
            var childParentData = ((ToolbarItemsParentData?)child.parentData!)!;
            if (childParentData.shouldPaint)
            {
                visitor((RenderBox)renderObjectChild);
            }
        });
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        var value = new List<DiagnosticsNode>();
        visitChildren((renderObjectChild) =>
        {
            var child = ((RenderBox?)renderObjectChild)!;
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
        var childParentData = ((ToolbarItemsParentData?)child.parentData!)!;
        while (childParentData.previousSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.previousSibling, child));
            child = childParentData.previousSibling!;
            childParentData = ((ToolbarItemsParentData?)child.parentData!)!;
        }
        return Equals(child, equals);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((ToolbarItemsParentData?)child.parentData!)!;
        while (childParentData.nextSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.nextSibling, child));
            child = childParentData.nextSibling!;
            childParentData = ((ToolbarItemsParentData?)child.parentData!)!;
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
                    throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"A {GetType()} expected a child of type {typeof(RenderBox)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {GetType()} that expected a {typeof(RenderBox)} child was created by", debugCreator, style: DiagnosticsTreeStyle.errorProperty), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", child.debugCreator, style: DiagnosticsTreeStyle.errorProperty) }));
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _insertIntoChildList(RenderBox child, RenderBox? after = null)
    {
        var childParentData = ((ToolbarItemsParentData?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => childParentData.nextSibling is null);
        DartRuntimePrimitives.Assert(() => childParentData.previousSibling is null);
        _childCount += 1L;
        DartRuntimePrimitives.Assert(() => _childCount > 0L);
        if (after is null)
        {
            childParentData.nextSibling = _firstChild;
            if (_firstChild is not null)
            {
                var firstChildParentData = ((ToolbarItemsParentData?)_firstChild!.parentData!)!;
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
            var afterParentData = ((ToolbarItemsParentData?)after.parentData!)!;
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
                var childPreviousSiblingParentData = ((ToolbarItemsParentData?)childParentData.previousSibling!.parentData!)!;
                var childNextSiblingParentData = ((ToolbarItemsParentData?)childParentData.nextSibling!.parentData!)!;
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
        DartRuntimePrimitives.Assert(() => child.parentData is ToolbarItemsParentData, () => (object?)$"A child of {GetType()} has parentData of type {DartRuntimePrimitives.RuntimeType(child.parentData)}, " + $"which does not conform to {(typeof(ToolbarItemsParentData))}. Class using ContainerRenderObjectMixin " + $"should override setupParentData() to set parentData to type {(typeof(ToolbarItemsParentData))}.");
        _insertIntoChildList(child, after: after);
    }

    public virtual void add(RenderBox child)
    {
        insert(child, after: _lastChild);
    }

    public virtual void addAll(List<RenderBox>? children)
    {
        children?.forEach((__arg0) => ((Action<RenderBox>)add)(__arg0));
    }

    public virtual void _removeFromChildList(RenderBox child)
    {
        var childParentData = ((ToolbarItemsParentData?)child.parentData!)!;
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
            var childPreviousSiblingParentData = ((ToolbarItemsParentData?)childParentData.previousSibling!.parentData!)!;
            childPreviousSiblingParentData.nextSibling = childParentData.nextSibling;
        }
        if (childParentData.nextSibling is null)
        {
            DartRuntimePrimitives.Assert(() => Equals(_lastChild, child));
            _lastChild = childParentData.previousSibling;
        }
        else
        {
            var childNextSiblingParentData = ((ToolbarItemsParentData?)childParentData.nextSibling!.parentData!)!;
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
            var childParentData = ((ToolbarItemsParentData?)child.parentData!)!;
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
        var childParentData = ((ToolbarItemsParentData?)child.parentData!)!;
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
        var childParentData = ((ToolbarItemsParentData?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((ToolbarItemsParentData?)child.parentData!)!;
        return childParentData.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToFirstActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((ToolbarItemsParentData?)child.parentData!)!;
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
            var childParentData = ((ToolbarItemsParentData?)child.parentData!)!;
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
            var childParentData = ((ToolbarItemsParentData?)child.parentData!)!;
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
            var childParentData = ((ToolbarItemsParentData?)child.parentData!)!;
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
            var childParentData = ((ToolbarItemsParentData?)child.parentData!)!;
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
