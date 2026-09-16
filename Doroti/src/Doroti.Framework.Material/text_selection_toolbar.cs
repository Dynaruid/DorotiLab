// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/text_selection_toolbar.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class Text_selection_toolbarLibrary
{
    internal static double _kToolbarHeight = 44.0;
}

public static partial class Text_selection_toolbarLibrary
{
    internal static double _kToolbarContentDistance = 8.0;
}

public class TextSelectionToolbar : StatelessWidget
{
    public virtual Offset anchorAbove { get; private set; } = default!;
    public virtual Offset anchorBelow { get; private set; } = default!;
    public virtual List<Widget> children { get; private set; } = default!;
    public virtual Func<BuildContext, Widget, Widget> toolbarBuilder { get; private set; } = default!;
    public const double kHandleSize = 22.0;
    public static double kToolbarContentDistanceBelow = kHandleSize - 2.0;

    public TextSelectionToolbar(Key? key = null, Offset anchorAbove = default!, Offset anchorBelow = default!, Func<BuildContext, Widget, Widget> toolbarBuilder = default!, List<Widget> children = default!) : base(key: key)
    {
        Func<BuildContext, Widget, Widget> __toolbarBuilder = toolbarBuilder ?? _defaultToolbarBuilder;
        this.anchorAbove = anchorAbove;
        this.anchorBelow = anchorBelow;
        this.toolbarBuilder = __toolbarBuilder;
        this.children = children;
        System.Diagnostics.Debug.Assert(checked(children.Count) > 0L);
    }

    internal static Widget _defaultToolbarBuilder(BuildContext context, Widget child)
    {
        return new _TextSelectionToolbarContainer__text_selection_toolbar(child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        Offset anchorAbovePadded = anchorAbove - new Offset(0.0, Text_selectionLibrary._kToolbarContentDistance);
        Offset anchorBelowPadded = anchorBelow + new Offset(0.0, kToolbarContentDistanceBelow);
        double screenPadding = CupertinoTextSelectionToolbar.kToolbarScreenPadding;
        double paddingAbove = MediaQuery.paddingOf(context).top + screenPadding;
        double availableHeight = anchorAbovePadded.dy - Text_selectionLibrary._kToolbarContentDistance - paddingAbove;
        bool fitsAboveLocal = Text_selection_toolbarLibrary._kToolbarHeight <= availableHeight;
        var localAdjustment = new Offset(screenPadding, paddingAbove);
        return new Padding(padding: new EdgeInsets(screenPadding, paddingAbove, screenPadding, screenPadding), child: new CustomSingleChildLayout(@delegate: new TextSelectionToolbarLayoutDelegate(anchorAbove: anchorAbovePadded - localAdjustment, anchorBelow: anchorBelowPadded - localAdjustment, fitsAbove: fitsAboveLocal), child: new _TextSelectionToolbarOverflowable__text_selection_toolbar(isAbove: fitsAboveLocal, toolbarBuilder: toolbarBuilder, children: children)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _TextSelectionToolbarOverflowable__text_selection_toolbar : StatefulWidget
{
    public virtual List<Widget> children { get; private set; } = default!;
    public virtual bool isAbove { get; private set; } = default!;
    public virtual Func<BuildContext, Widget, Widget> toolbarBuilder { get; private set; } = default!;

    internal _TextSelectionToolbarOverflowable__text_selection_toolbar(bool isAbove, Func<BuildContext, Widget, Widget> toolbarBuilder, List<Widget> children)
    {
        this.isAbove = isAbove;
        this.toolbarBuilder = toolbarBuilder;
        this.children = children;
        System.Diagnostics.Debug.Assert(checked(children.Count) > 0L);
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _TextSelectionToolbarOverflowableState__text_selection_toolbar());
}

public class _TextSelectionToolbarOverflowableState__text_selection_toolbar : State<_TextSelectionToolbarOverflowable__text_selection_toolbar>, TickerProviderStateMixin<_TextSelectionToolbarOverflowable__text_selection_toolbar>
{
    internal virtual bool _overflowOpen { get; set; } = false;
    internal virtual UniqueKey _containerKey { get; set; } = new UniqueKey();
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual void _reset()
    {
        _containerKey = new UniqueKey();
        _overflowOpen = false;
    }

    public override void didUpdateWidget(_TextSelectionToolbarOverflowable__text_selection_toolbar oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!CollectionsLibrary.listEquals(widget.children, oldWidget.children))
        {
            _reset();
        }
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        TextDirection textDirectionLocal = Directionality.of(context);
        return new _TextSelectionToolbarTrailingEdgeAlign__text_selection_toolbar(key: _containerKey, overflowOpen: _overflowOpen, textDirection: textDirectionLocal, child: new AnimatedSize(duration: Duration.Create(milliseconds: 140L), child: widget.toolbarBuilder(context, new _TextSelectionToolbarItemsLayout__text_selection_toolbar(isAbove: widget.isAbove, overflowOpen: _overflowOpen, textDirection: textDirectionLocal, children: ((Func<List<Widget>>)(() =>
        {
            var __collection7958 = new List<Widget>(); __collection7958.Add(DartRuntimePrimitives.ConvertValue<Widget>(new _TextSelectionToolbarOverflowButton__text_selection_toolbar(key: _overflowOpen ? StandardComponentTypeMembers.key(StandardComponentType.backButton) : StandardComponentTypeMembers.key(StandardComponentType.moreButton), icon: new Icon(_overflowOpen ? Icons.arrow_back : Icons.more_vert), onPressed: () =>
            {
                setState(() =>
                {
                    _overflowOpen = !_overflowOpen;
                });
            }, tooltip: _overflowOpen ? localizations.backButtonTooltip : localizations.moreButtonTooltip))); __collection7958.AddRange(widget.children); return __collection7958;
        }))()))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Scheduler.Ticker createTicker(System.Action<Duration> onTick)
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

    public override void dispose()
    {
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

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<HashSet<Scheduler.Ticker>>("tickers", _tickers, description: (_tickers is not null) ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}" : null, defaultValue: default));
    }

}

internal class _TextSelectionToolbarTrailingEdgeAlign__text_selection_toolbar : SingleChildRenderObjectWidget
{
    public virtual bool overflowOpen { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;

    internal _TextSelectionToolbarTrailingEdgeAlign__text_selection_toolbar(Key? key = null, Widget child = default!, bool overflowOpen = default!, TextDirection textDirection = default!) : base(key: key, child: child)
    {
        this.overflowOpen = overflowOpen;
        this.textDirection = textDirection;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _TextSelectionToolbarTrailingEdgeAlignRenderBox__text_selection_toolbar(overflowOpen: overflowOpen, textDirection: textDirection);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_TextSelectionToolbarTrailingEdgeAlignRenderBox__text_selection_toolbar)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_TextSelectionToolbarTrailingEdgeAlignRenderBox__text_selection_toolbar>)(() =>
{
    var __cascade = __renderObject;
    __cascade.overflowOpen = overflowOpen;
    __cascade.textDirection = textDirection;
    return __cascade;
}))());
    }

}

public class _TextSelectionToolbarTrailingEdgeAlignRenderBox__text_selection_toolbar : RenderProxyBox
{
    internal virtual double? _closedWidth { get; set; } = default;
    internal virtual bool _overflowOpen { get; set; } = default!;
    internal virtual TextDirection _textDirection { get; set; } = default!;

    internal _TextSelectionToolbarTrailingEdgeAlignRenderBox__text_selection_toolbar(bool overflowOpen, TextDirection textDirection)
    {
        _textDirection = textDirection;
        _overflowOpen = overflowOpen;
    }

    public virtual bool overflowOpen
    {
        get => _overflowOpen;
        set
        {
            var __value = value;
            if (__value == overflowOpen)
            {
                return;
            }
            _overflowOpen = __value;
            markNeedsLayout();
        }
    }
    public virtual TextDirection textDirection
    {
        get => _textDirection;
        set
        {
            var __value = value;
            if (Equals(__value, textDirection))
            {
                return;
            }
            _textDirection = __value;
            markNeedsLayout();
        }
    }
    public override void performLayout()
    {
        child!.layout(constraints.loosen(), parentUsesSize: true);
        if (!overflowOpen && (_closedWidth is null))
        {
            _closedWidth = child!.size.width;
        }
        size = constraints.constrain(new Size(((_closedWidth is null) || (child!.size.width > DartRuntimePrimitives.RequireValue(_closedWidth))) ? child!.size.width : DartRuntimePrimitives.RequireValue(_closedWidth), child!.size.height));
        var childParentData = ((ToolbarItemsParentData?)child!.parentData!)!;
        childParentData.offset = new Offset(Equals(textDirection, TextDirection.rtl) ? 0.0 : (size.width - child!.size.width), 0.0);
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        var childParentData = ((ToolbarItemsParentData?)child!.parentData!)!;
        context.paintChild(child!, childParentData.offset + offset);
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        var childParentData = ((ToolbarItemsParentData?)child!.parentData!)!;
        return result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: (result, transformed) =>
        {
            DartRuntimePrimitives.Assert(() => Equals(transformed, position - childParentData.offset));
            return child!.hitTest(result, position: transformed);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void setupParentData(RenderObject child)
    {
        var __child = (RenderBox)child;
        if (__child.parentData is not ToolbarItemsParentData)
        {
            __child.parentData = new ToolbarItemsParentData();
        }
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var childParentData = ((ToolbarItemsParentData?)child.parentData!)!;
        transform.translateByDouble(childParentData.offset.dx, childParentData.offset.dy, 0, 1);
        base.applyPaintTransform(child, transform);
    }

}

internal class _TextSelectionToolbarItemsLayout__text_selection_toolbar : MultiChildRenderObjectWidget
{
    public virtual bool isAbove { get; private set; } = default!;
    public virtual bool overflowOpen { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;

    internal _TextSelectionToolbarItemsLayout__text_selection_toolbar(bool isAbove, bool overflowOpen, TextDirection textDirection, List<Widget> children) : base(children: children)
    {
        this.isAbove = isAbove;
        this.overflowOpen = overflowOpen;
        this.textDirection = textDirection;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderTextSelectionToolbarItemsLayout__text_selection_toolbar(isAbove: isAbove, overflowOpen: overflowOpen, textDirection: textDirection);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderTextSelectionToolbarItemsLayout__text_selection_toolbar)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderTextSelectionToolbarItemsLayout__text_selection_toolbar>)(() =>
{
    var __cascade = __renderObject;
    __cascade.isAbove = isAbove;
    __cascade.textDirection = textDirection;
    __cascade.overflowOpen = overflowOpen;
    return __cascade;
}))());
    }

    public override _TextSelectionToolbarItemsLayoutElement__text_selection_toolbar createElement() => new _TextSelectionToolbarItemsLayoutElement__text_selection_toolbar(this);
}

public class _TextSelectionToolbarItemsLayoutElement__text_selection_toolbar : MultiChildRenderObjectElement
{
    internal _TextSelectionToolbarItemsLayoutElement__text_selection_toolbar(MultiChildRenderObjectWidget widget) : base(widget)
    {
    }

    internal static bool _shouldPaint(Element child)
    {
        return ((ToolbarItemsParentData?)child.renderObject!.parentData!)!.shouldPaint;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugVisitOnstageChildren(System.Action<Element> visitor)
    {
        children.where(_shouldPaint).forEach((__arg0) => visitor(__arg0));
    }

}

public class _RenderTextSelectionToolbarItemsLayout__text_selection_toolbar : RenderBox, ContainerRenderObjectMixin<RenderBox, ToolbarItemsParentData>
{
    internal virtual long _lastIndexThatFits { get; set; } = -1L;
    internal virtual bool _isAbove { get; set; } = default!;
    internal virtual bool _overflowOpen { get; set; } = default!;
    internal virtual TextDirection _textDirection { get; set; } = default!;
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;

    internal _RenderTextSelectionToolbarItemsLayout__text_selection_toolbar(bool isAbove, bool overflowOpen, TextDirection textDirection)
    {
        _isAbove = isAbove;
        _overflowOpen = overflowOpen;
        _textDirection = textDirection;
    }

    public virtual bool isAbove
    {
        get => _isAbove;
        set
        {
            var __value = value;
            if (__value == isAbove)
            {
                return;
            }
            _isAbove = __value;
            markNeedsLayout();
        }
    }
    public virtual bool overflowOpen
    {
        get => _overflowOpen;
        set
        {
            var __value = value;
            if (__value == overflowOpen)
            {
                return;
            }
            _overflowOpen = __value;
            markNeedsLayout();
        }
    }
    public virtual TextDirection textDirection
    {
        get => _textDirection;
        set
        {
            var __value = value;
            if (Equals(__value, textDirection))
            {
                return;
            }
            _textDirection = __value;
            markNeedsLayout();
        }
    }
    internal virtual void _layoutChildren()
    {
        BoxConstraints sizedConstraints = _overflowOpen ? constraints : BoxConstraints.CreateLoose(new Size(constraints.maxWidth, Text_selection_toolbarLibrary._kToolbarHeight));
        var i = -1L;
        var widthLocal = 0.0;
        visitChildren((renderObjectChild) =>
        {
            i++;
            if ((_lastIndexThatFits != -1L) && !overflowOpen)
            {
                return;
            }
            var child = ((RenderBox?)renderObjectChild)!;
            child.layout(sizedConstraints.loosen(), parentUsesSize: true);
            widthLocal += child.size.width;
            if ((widthLocal > sizedConstraints.maxWidth) && (_lastIndexThatFits == -1L))
            {
                _lastIndexThatFits = i - 1L;
            }
        });
        RenderBox navButton = firstChild!;
        if ((_lastIndexThatFits != -1L) && (_lastIndexThatFits == (childCount - 2L)) && ((widthLocal - navButton.size.width) <= sizedConstraints.maxWidth))
        {
            _lastIndexThatFits = -1L;
        }
    }

    internal virtual bool _shouldPaintChild(RenderObject renderObjectChild, long index)
    {
        if (Equals(renderObjectChild, firstChild))
        {
            return _lastIndexThatFits != -1L;
        }
        if (_lastIndexThatFits == -1L)
        {
            return true;
        }
        return index > _lastIndexThatFits == overflowOpen;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Size _placeChildrenHorizontally()
    {
        RenderBox navButton = firstChild!;
        var isRtl = Equals(textDirection, TextDirection.rtl);
        var contentItems = new List<RenderBox>();
        var totalWidth = 0.0;
        var maxHeight = 0.0;
        var i = -1L;
        visitChildren((renderObjectChild) =>
        {
            var child = ((RenderBox?)renderObjectChild)!;
            var childParentData = ((ToolbarItemsParentData?)child.parentData!)!;
            i++;
            if (!_shouldPaintChild(child, i))
            {
                childParentData.shouldPaint = false;
            }
            else
            {
                childParentData.shouldPaint = true;
                totalWidth += child.size.width;
                maxHeight = Math.Max(maxHeight, child.size.height);
                if (!Equals(child, navButton))
                {
                    contentItems.Add(child);
                }
            }
        });
        var currentX = 0.0;
        bool showNavButton = _lastIndexThatFits >= 0L;
        if (isRtl)
        {
            if (showNavButton)
            {
                var navParentData = ((ToolbarItemsParentData?)navButton.parentData!)!;
                navParentData.offset = Offset.zero;
                currentX += navButton.size.width;
            }
            var rightEdge = totalWidth;
            foreach (var item in contentItems)
            {
                rightEdge -= item.size.width;
                var itemParentData = ((ToolbarItemsParentData?)item.parentData!)!;
                itemParentData.offset = new Offset(rightEdge, 0.0);
            }
        }
        else
        {
            foreach (var itemLocal in contentItems)
            {
                var itemParentDataLocal = ((ToolbarItemsParentData?)itemLocal.parentData!)!;
                itemParentDataLocal.offset = new Offset(currentX, 0.0);
                currentX += itemLocal.size.width;
            }
            if (showNavButton)
            {
                var navParentDataLocal = ((ToolbarItemsParentData?)navButton.parentData!)!;
                navParentDataLocal.offset = new Offset(currentX, 0.0);
            }
        }
        return new Size(totalWidth, maxHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Size _placeChildrenVertically()
    {
        RenderBox navButton = firstChild!;
        var currentY = 0.0;
        var maxWidth = 0.0;
        var navButtonParentData = ((ToolbarItemsParentData?)navButton.parentData!)!;
        if (_shouldPaintChild(navButton, 0L))
        {
            navButtonParentData.shouldPaint = true;
            if (!isAbove)
            {
                navButtonParentData.offset = Offset.zero;
                currentY += navButton.size.height;
                maxWidth = Math.Max(maxWidth, navButton.size.width);
            }
        }
        else
        {
            navButtonParentData.shouldPaint = false;
        }
        var i = -1L;
        visitChildren((renderObjectChild) =>
        {
            var child = ((RenderBox?)renderObjectChild)!;
            var childParentData = ((ToolbarItemsParentData?)child.parentData!)!;
            i++;
            if (Equals((RenderBox)renderObjectChild, navButton))
            {
                return;
            }
            if (!_shouldPaintChild(child, i))
            {
                childParentData.shouldPaint = false;
                return;
            }
            childParentData.shouldPaint = true;
            childParentData.offset = new Offset(0.0, currentY);
            currentY += child.size.height;
            maxWidth = Math.Max(maxWidth, child.size.width);
        });
        if (isAbove && navButtonParentData.shouldPaint)
        {
            navButtonParentData.offset = new Offset(0.0, currentY);
            currentY += navButton.size.height;
            maxWidth = Math.Max(maxWidth, navButton.size.width);
        }
        return new Size(maxWidth, currentY);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _placeChildren()
    {
        size = overflowOpen ? _placeChildrenVertically() : _placeChildrenHorizontally();
    }

    internal virtual void _resizeChildrenWhenOverflow()
    {
        if (!overflowOpen)
        {
            return;
        }
        RenderBox navButton = firstChild!;
        var i = -1L;
        visitChildren((renderObjectChild) =>
        {
            var child = ((RenderBox?)renderObjectChild)!;
            var childParentData = ((ToolbarItemsParentData?)child.parentData!)!;
            i++;
            if (Equals((RenderBox)renderObjectChild, navButton))
            {
                return;
            }
            if (!_shouldPaintChild((RenderBox)renderObjectChild, i))
            {
                childParentData.shouldPaint = false;
                return;
            }
            child.layout(BoxConstraints.CreateTightFor(width: size.width), parentUsesSize: true);
        });
    }

    public override void performLayout()
    {
        _lastIndexThatFits = -1L;
        if (firstChild is null)
        {
            size = constraints.smallest;
            return;
        }
        _layoutChildren();
        _placeChildren();
        _resizeChildrenWhenOverflow();
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        visitChildren((renderObjectChild) =>
        {
            var child = ((RenderBox?)renderObjectChild)!;
            var childParentData = ((ToolbarItemsParentData?)child.parentData!)!;
            if (!childParentData.shouldPaint)
            {
                return;
            }
            context.paintChild(child, childParentData.offset + offset);
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

    public override void visitChildrenForSemantics(System.Action<RenderObject> visitor)
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
        children?.forEach((__arg0) => ((System.Action<RenderBox>)add)(__arg0));
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
    }

    public override void redepthChildren()
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            redepthChild(child);
            var childParentData = ((ToolbarItemsParentData?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void visitChildren(System.Action<RenderObject> visitor)
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            visitor(child);
            var childParentData = ((ToolbarItemsParentData?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
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

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        var children = new List<DiagnosticsNode>();
        if (firstChild is not null)
        {
            RenderBox child = firstChild!;
            var count = 1L;
            while (true)
            {
                children.Add(((Diagnosticable)child).toDiagnosticsNode(name: $"child__183606 {count}"));
                if (Equals(child, lastChild))
                {
                    break;
                }
                count += 1L;
                var childParentData = ((ToolbarItemsParentData?)child.parentData!)!;
                child = childParentData.nextSibling!;
            }
        }
        return children;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TextSelectionToolbarContainer__text_selection_toolbar : StatelessWidget
{
    public virtual Widget child { get; private set; } = default!;
    internal static Color _defaultColorLight = new Color(4294967295L);
    internal static Color _defaultColorDark = new Color(4282532418L);

    internal _TextSelectionToolbarContainer__text_selection_toolbar(Widget child)
    {
        this.child = child;
    }

    internal static Color _getColor(ColorScheme colorScheme)
    {
        bool isDefaultSurface = colorScheme.brightness switch { Brightness.light => DartRuntimePrimitives.Identical(ThemeData.Create().colorScheme.surface, colorScheme.surface), Brightness.dark => DartRuntimePrimitives.Identical(ThemeData.Create().colorScheme.surface, colorScheme.surface), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        if (!isDefaultSurface)
        {
            return colorScheme.surface;
        }
        return colorScheme.brightness switch { Brightness.light => _defaultColorLight, Brightness.dark => _defaultColorDark, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        return new Material(borderRadius: BorderRadius.CreateAll(Radius.circular(Text_selection_toolbarLibrary._kToolbarHeight / 2L)), clipBehavior: Clip.antiAlias, color: _getColor(theme.colorScheme), elevation: 1.0, type: MaterialType.card, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TextSelectionToolbarOverflowButton__text_selection_toolbar : StatelessWidget
{
    public virtual Icon icon { get; private set; } = default!;
    public virtual Action? onPressed { get; private set; }
    public virtual string? tooltip { get; private set; }

    internal _TextSelectionToolbarOverflowButton__text_selection_toolbar(Key? key = null, Icon icon = default!, Action? onPressed = null, string? tooltip = null) : base(key: key)
    {
        this.icon = icon;
        this.onPressed = onPressed;
        this.tooltip = tooltip;
    }

    public override Widget build(BuildContext context)
    {
        return new Material(type: MaterialType.card, color: new Color(0L), child: new IconButton(icon: icon, onPressed: onPressed, tooltip: tooltip));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
