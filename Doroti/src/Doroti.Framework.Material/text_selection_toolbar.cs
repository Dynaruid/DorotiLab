// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/text_selection_toolbar.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Material;

public static partial class Text_selection_toolbarLibrary
{
    internal static double _kToolbarHeight = 44.0;
}

public static partial class Text_selection_toolbarLibrary
{
    internal static double _kToolbarContentDistance = 8.0;
}

public class TextSelectionToolbar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual Offset anchorAbove { get; private set; } = default!;
    public virtual Offset anchorBelow { get; private set; } = default!;
    public virtual List<global::Doroti.Framework.Widgets.Widget> children { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Widgets.Widget> toolbarBuilder { get; private set; } = default!;
    public const double kHandleSize = 22.0;
    public static double kToolbarContentDistanceBelow = (kHandleSize - 2.0);

    public TextSelectionToolbar(global::Doroti.Framework.Foundation.Key? key = null, Offset anchorAbove = default!, Offset anchorBelow = default!, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Widgets.Widget> toolbarBuilder = default!, List<global::Doroti.Framework.Widgets.Widget> children = default!) : base(key: key)
    {
        global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Widgets.Widget> __toolbarBuilder = toolbarBuilder ?? _defaultToolbarBuilder;
        this.anchorAbove = anchorAbove;
        this.anchorBelow = anchorBelow;
        this.toolbarBuilder = __toolbarBuilder;
        this.children = children;
        System.Diagnostics.Debug.Assert((checked((long)(children.Count)) > 0L));
    }

    internal static global::Doroti.Framework.Widgets.Widget _defaultToolbarBuilder(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _TextSelectionToolbarContainer__text_selection_toolbar(child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Offset anchorAbovePadded = ((global::Doroti.Ui.Offset)(object?)(this.anchorAbove - new global::Doroti.Ui.Offset(0.0, Text_selectionLibrary._kToolbarContentDistance)));
        global::Doroti.Ui.Offset anchorBelowPadded = ((global::Doroti.Ui.Offset)(object?)(this.anchorBelow + new global::Doroti.Ui.Offset(0.0, kToolbarContentDistanceBelow)));
        double screenPadding = CupertinoTextSelectionToolbar.kToolbarScreenPadding;
        double paddingAbove = (MediaQuery.paddingOf(context).top + screenPadding);
        double availableHeight = ((anchorAbovePadded.dy - Text_selectionLibrary._kToolbarContentDistance) - paddingAbove);
        bool fitsAboveLocal = (Text_selection_toolbarLibrary._kToolbarHeight <= availableHeight);
        var localAdjustment = new global::Doroti.Ui.Offset(screenPadding, paddingAbove);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Padding(padding: new global::Doroti.Framework.Painting.EdgeInsets(screenPadding, paddingAbove, screenPadding, screenPadding), child: new global::Doroti.Framework.Widgets.CustomSingleChildLayout(@delegate: new global::Doroti.Framework.Widgets.TextSelectionToolbarLayoutDelegate(anchorAbove: (anchorAbovePadded - localAdjustment), anchorBelow: (anchorBelowPadded - localAdjustment), fitsAbove: fitsAboveLocal), child: new _TextSelectionToolbarOverflowable__text_selection_toolbar(isAbove: fitsAboveLocal, toolbarBuilder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Widgets.Widget>)this.toolbarBuilder, children: this.children))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _TextSelectionToolbarOverflowable__text_selection_toolbar : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual List<global::Doroti.Framework.Widgets.Widget> children { get; private set; } = default!;
    public virtual bool isAbove { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Widgets.Widget> toolbarBuilder { get; private set; } = default!;

    internal _TextSelectionToolbarOverflowable__text_selection_toolbar(bool isAbove, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Widgets.Widget> toolbarBuilder, List<global::Doroti.Framework.Widgets.Widget> children)
    {
        this.isAbove = isAbove;
        this.toolbarBuilder = toolbarBuilder;
        this.children = children;
        System.Diagnostics.Debug.Assert((checked((long)(children.Count)) > 0L));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _TextSelectionToolbarOverflowableState__text_selection_toolbar());
}

public class _TextSelectionToolbarOverflowableState__text_selection_toolbar : global::Doroti.Framework.Widgets.State<_TextSelectionToolbarOverflowable__text_selection_toolbar>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<_TextSelectionToolbarOverflowable__text_selection_toolbar>
{
    internal virtual bool _overflowOpen { get; set; } = false;
    internal virtual global::Doroti.Framework.Foundation.UniqueKey _containerKey { get; set; } = new global::Doroti.Framework.Foundation.UniqueKey();
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual void _reset()
    {
        _containerKey = new global::Doroti.Framework.Foundation.UniqueKey();
        _overflowOpen = false;
    }

    public override void didUpdateWidget(_TextSelectionToolbarOverflowable__text_selection_toolbar oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!global::Doroti.Framework.Foundation.CollectionsLibrary.listEquals(((_TextSelectionToolbarOverflowable__text_selection_toolbar)this.widget).children.Cast<_TextSelectionToolbarOverflowable__text_selection_toolbar>().ToList(), ((_TextSelectionToolbarOverflowable__text_selection_toolbar)oldWidget).children.Cast<_TextSelectionToolbarOverflowable__text_selection_toolbar>().ToList()))
        {
            _reset();
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        global::Doroti.Ui.TextDirection textDirectionLocal = Directionality.of(context);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _TextSelectionToolbarTrailingEdgeAlign__text_selection_toolbar(key: this._containerKey, overflowOpen: this._overflowOpen, textDirection: textDirectionLocal, child: new global::Doroti.Framework.Widgets.AnimatedSize(duration: Duration.Create(milliseconds: 140L), child: this.widget.toolbarBuilder(context, new _TextSelectionToolbarItemsLayout__text_selection_toolbar(isAbove: ((_TextSelectionToolbarOverflowable__text_selection_toolbar)this.widget).isAbove, overflowOpen: this._overflowOpen, textDirection: textDirectionLocal, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() =>
        {
            var __collection7958 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection7958.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _TextSelectionToolbarOverflowButton__text_selection_toolbar(key: (this._overflowOpen ? StandardComponentTypeMembers.key(global::Doroti.Framework.Widgets.StandardComponentType.backButton) : StandardComponentTypeMembers.key(global::Doroti.Framework.Widgets.StandardComponentType.moreButton)), icon: new global::Doroti.Framework.Widgets.Icon((this._overflowOpen ? Icons.arrow_back : Icons.more_vert)), onPressed: ((global::System.Action)(() =>
            {
                setState(((global::System.Action)(() =>
                {
                    _overflowOpen = !this._overflowOpen;
                })));
            })), tooltip: (this._overflowOpen ? localizations.backButtonTooltip : localizations.moreButtonTooltip)))); __collection7958.AddRange(((_TextSelectionToolbarOverflowable__text_selection_toolbar)this.widget).children); return __collection7958;
        }))())))));
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
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier;
    }

    public override void dispose()
    {
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
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        this._tickerModeNotifier = null;
        base.dispose();
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

internal class _TextSelectionToolbarTrailingEdgeAlign__text_selection_toolbar : global::Doroti.Framework.Widgets.SingleChildRenderObjectWidget
{
    public virtual bool overflowOpen { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;

    internal _TextSelectionToolbarTrailingEdgeAlign__text_selection_toolbar(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget child = default!, bool overflowOpen = default!, TextDirection textDirection = default!) : base(key: key, child: child)
    {
        this.overflowOpen = overflowOpen;
        this.textDirection = textDirection;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _TextSelectionToolbarTrailingEdgeAlignRenderBox__text_selection_toolbar(overflowOpen: this.overflowOpen, textDirection: this.textDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_TextSelectionToolbarTrailingEdgeAlignRenderBox__text_selection_toolbar)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_TextSelectionToolbarTrailingEdgeAlignRenderBox__text_selection_toolbar>)(() =>
{
    var __cascade = __renderObject;
    __cascade.overflowOpen = this.overflowOpen;
    __cascade.textDirection = this.textDirection;
    return __cascade;
}))());
    }

}

public class _TextSelectionToolbarTrailingEdgeAlignRenderBox__text_selection_toolbar : global::Doroti.Framework.Rendering.RenderProxyBox
{
    internal virtual double? _closedWidth { get; set; } = default;
    internal virtual bool _overflowOpen { get; set; } = default!;
    internal virtual TextDirection _textDirection { get; set; } = default!;

    internal _TextSelectionToolbarTrailingEdgeAlignRenderBox__text_selection_toolbar(bool overflowOpen, TextDirection textDirection)
    {
        this._textDirection = textDirection;
        this._overflowOpen = overflowOpen;
    }

    public virtual bool overflowOpen
    {
        get => this._overflowOpen;
        set
        {
            var __value = value;
            if ((__value == this.overflowOpen))
            {
                return;
            }
            _overflowOpen = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.TextDirection textDirection
    {
        get => this._textDirection;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this.textDirection)))
            {
                return;
            }
            _textDirection = __value;
            markNeedsLayout();
        }
    }
    public virtual void performLayout()
    {
        this.child!.layout(this.constraints.loosen(), parentUsesSize: true);
        if ((!this.overflowOpen && (this._closedWidth is null)))
        {
            _closedWidth = this.child!.size.width;
        }
        size = this.constraints.constrain(new global::Doroti.Ui.Size((((this._closedWidth is null) || (this.child!.size.width > DartRuntimePrimitives.RequireValue(this._closedWidth))) ? this.child!.size.width : DartRuntimePrimitives.RequireValue(this._closedWidth)), this.child!.size.height));
        var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)this.child!.parentData!)!;
        childParentData.offset = new global::Doroti.Ui.Offset(((object.Equals(this.textDirection, TextDirection.rtl)) ? 0.0 : (this.size.width - this.child!.size.width)), 0.0);
    }

    public virtual void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)this.child!.parentData!)!;
        context.paintChild(this.child!, (childParentData.offset + offset));
    }

    public virtual bool hitTestChildren(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)this.child!.parentData!)!;
        return result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: ((global::System.Func<global::Doroti.Framework.Rendering.BoxHitTestResult, Offset, bool>)((result, transformed) =>
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - childParentData.offset))));
            return this.child!.hitTest(result, position: transformed);
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void setupParentData(global::Doroti.Framework.Rendering.RenderObject child)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)(object)child;
        if ((__child.parentData is not global::Doroti.Framework.Widgets.ToolbarItemsParentData))
        {
            __child.parentData = new global::Doroti.Framework.Widgets.ToolbarItemsParentData();
        }
    }

    public virtual void applyPaintTransform(global::Doroti.Framework.Rendering.RenderObject child, Matrix4 transform)
    {
        var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)((global::Doroti.Framework.Rendering.RenderObject)child).parentData!)!;
        transform.translateByDouble(childParentData.offset.dx, childParentData.offset.dy, 0, 1);
        base.applyPaintTransform(child, transform);
    }

}

internal class _TextSelectionToolbarItemsLayout__text_selection_toolbar : global::Doroti.Framework.Widgets.MultiChildRenderObjectWidget
{
    public virtual bool isAbove { get; private set; } = default!;
    public virtual bool overflowOpen { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;

    internal _TextSelectionToolbarItemsLayout__text_selection_toolbar(bool isAbove, bool overflowOpen, TextDirection textDirection, List<global::Doroti.Framework.Widgets.Widget> children) : base(children: children)
    {
        this.isAbove = isAbove;
        this.overflowOpen = overflowOpen;
        this.textDirection = textDirection;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderTextSelectionToolbarItemsLayout__text_selection_toolbar(isAbove: this.isAbove, overflowOpen: this.overflowOpen, textDirection: this.textDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderTextSelectionToolbarItemsLayout__text_selection_toolbar)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderTextSelectionToolbarItemsLayout__text_selection_toolbar>)(() =>
{
    var __cascade = __renderObject;
    __cascade.isAbove = this.isAbove;
    __cascade.textDirection = this.textDirection;
    __cascade.overflowOpen = this.overflowOpen;
    return __cascade;
}))());
    }

    public override _TextSelectionToolbarItemsLayoutElement__text_selection_toolbar createElement() => new _TextSelectionToolbarItemsLayoutElement__text_selection_toolbar(this);
}

public class _TextSelectionToolbarItemsLayoutElement__text_selection_toolbar : global::Doroti.Framework.Widgets.MultiChildRenderObjectElement
{
    internal _TextSelectionToolbarItemsLayoutElement__text_selection_toolbar(global::Doroti.Framework.Widgets.MultiChildRenderObjectWidget widget) : base(widget)
    {
    }

    internal static bool _shouldPaint(global::Doroti.Framework.Widgets.Element child)
    {
        return (((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)((global::Doroti.Framework.Rendering.ParentData?)((dynamic)((global::Doroti.Framework.Widgets.Element)child).renderObject!).parentData)!)!).shouldPaint;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugVisitOnstageChildren(global::System.Action<global::Doroti.Framework.Widgets.Element> visitor)
    {
        this.children.where(_shouldPaint).forEach((__arg0) => ((global::System.Action<global::Doroti.Framework.Widgets.Element>)visitor)(__arg0));
    }

}

public class _RenderTextSelectionToolbarItemsLayout__text_selection_toolbar : global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.ContainerRenderObjectMixin<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Widgets.ToolbarItemsParentData>
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
        this._isAbove = isAbove;
        this._overflowOpen = overflowOpen;
        this._textDirection = textDirection;
    }

    public virtual bool isAbove
    {
        get => this._isAbove;
        set
        {
            var __value = value;
            if ((__value == this.isAbove))
            {
                return;
            }
            _isAbove = __value;
            markNeedsLayout();
        }
    }
    public virtual bool overflowOpen
    {
        get => this._overflowOpen;
        set
        {
            var __value = value;
            if ((__value == this.overflowOpen))
            {
                return;
            }
            _overflowOpen = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.TextDirection textDirection
    {
        get => this._textDirection;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this.textDirection)))
            {
                return;
            }
            _textDirection = __value;
            markNeedsLayout();
        }
    }
    internal virtual void _layoutChildren()
    {
        global::Doroti.Framework.Rendering.BoxConstraints sizedConstraints = (this._overflowOpen ? this.constraints : global::Doroti.Framework.Rendering.BoxConstraints.CreateLoose(new global::Doroti.Ui.Size(((global::Doroti.Framework.Rendering.BoxConstraints)this.constraints).maxWidth, Text_selection_toolbarLibrary._kToolbarHeight)));
        var i = -1L;
        var widthLocal = 0.0;
        visitChildren(((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)((renderObjectChild) =>
        {
            i++;
            if (((this._lastIndexThatFits != -1L) && !this.overflowOpen))
            {
                return;
            }
            var child = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)renderObjectChild)!;
            child.layout(sizedConstraints.loosen(), parentUsesSize: true);
            widthLocal += ((global::Doroti.Framework.Rendering.RenderBox)child).size.width;
            if (((widthLocal > ((global::Doroti.Framework.Rendering.BoxConstraints)sizedConstraints).maxWidth) && (this._lastIndexThatFits == -1L)))
            {
                _lastIndexThatFits = (i - 1L);
            }
        })));
        global::Doroti.Framework.Rendering.RenderBox navButton = this.firstChild!;
        if ((((this._lastIndexThatFits != -1L) && (this._lastIndexThatFits == (this.childCount - 2L))) && ((widthLocal - ((global::Doroti.Framework.Rendering.RenderBox)navButton).size.width) <= ((global::Doroti.Framework.Rendering.BoxConstraints)sizedConstraints).maxWidth)))
        {
            _lastIndexThatFits = -1L;
        }
    }

    internal virtual bool _shouldPaintChild(global::Doroti.Framework.Rendering.RenderObject renderObjectChild, long index)
    {
        if ((object.Equals(renderObjectChild, this.firstChild)))
        {
            return (this._lastIndexThatFits != -1L);
        }
        if ((this._lastIndexThatFits == -1L))
        {
            return true;
        }
        return (((index > this._lastIndexThatFits)) == this.overflowOpen);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Size _placeChildrenHorizontally()
    {
        global::Doroti.Framework.Rendering.RenderBox navButton = this.firstChild!;
        var isRtl = (object.Equals(this.textDirection, TextDirection.rtl));
        var contentItems = new List<global::Doroti.Framework.Rendering.RenderBox>();
        var totalWidth = 0.0;
        var maxHeight = 0.0;
        var i = -1L;
        visitChildren(((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)((renderObjectChild) =>
        {
            var child = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)renderObjectChild)!;
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
            i++;
            if (!_shouldPaintChild(child, i))
            {
                childParentData.shouldPaint = false;
            }
            else
            {
                childParentData.shouldPaint = true;
                totalWidth += ((global::Doroti.Framework.Rendering.RenderBox)child).size.width;
                maxHeight = Math.Max(maxHeight, ((global::Doroti.Framework.Rendering.RenderBox)child).size.height);
                if ((!object.Equals(child, navButton)))
                {
                    contentItems.Add(child);
                }
            }
        })));
        var currentX = 0.0;
        bool showNavButton = (this._lastIndexThatFits >= 0L);
        if (isRtl)
        {
            if (showNavButton)
            {
                var navParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)navButton.parentData!)!;
                navParentData.offset = Offset.zero;
                currentX += ((global::Doroti.Framework.Rendering.RenderBox)navButton).size.width;
            }
            var rightEdge = totalWidth;
            foreach (var item in contentItems)
            {
                rightEdge -= ((global::Doroti.Framework.Rendering.RenderBox)item).size.width;
                var itemParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)item.parentData!)!;
                itemParentData.offset = new global::Doroti.Ui.Offset(rightEdge, 0.0);
            }
        }
        else
        {
            foreach (var itemLocal in contentItems)
            {
                var itemParentDataLocal = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)itemLocal.parentData!)!;
                itemParentDataLocal.offset = new global::Doroti.Ui.Offset(currentX, 0.0);
                currentX += ((global::Doroti.Framework.Rendering.RenderBox)itemLocal).size.width;
            }
            if (showNavButton)
            {
                var navParentDataLocal = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)navButton.parentData!)!;
                navParentDataLocal.offset = new global::Doroti.Ui.Offset(currentX, 0.0);
            }
        }
        return new global::Doroti.Ui.Size(totalWidth, maxHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Size _placeChildrenVertically()
    {
        global::Doroti.Framework.Rendering.RenderBox navButton = this.firstChild!;
        var currentY = 0.0;
        var maxWidth = 0.0;
        var navButtonParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)navButton.parentData!)!;
        if (_shouldPaintChild(navButton, 0L))
        {
            navButtonParentData.shouldPaint = true;
            if (!this.isAbove)
            {
                navButtonParentData.offset = Offset.zero;
                currentY += ((global::Doroti.Framework.Rendering.RenderBox)navButton).size.height;
                maxWidth = Math.Max(maxWidth, ((global::Doroti.Framework.Rendering.RenderBox)navButton).size.width);
            }
        }
        else
        {
            navButtonParentData.shouldPaint = false;
        }
        var i = -1L;
        visitChildren(((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)((renderObjectChild) =>
        {
            var child = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)renderObjectChild)!;
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
            i++;
            if ((object.Equals(((global::Doroti.Framework.Rendering.RenderBox)renderObjectChild), navButton)))
            {
                return;
            }
            if (!_shouldPaintChild(child, i))
            {
                childParentData.shouldPaint = false;
                return;
            }
            childParentData.shouldPaint = true;
            childParentData.offset = new global::Doroti.Ui.Offset(0.0, currentY);
            currentY += ((global::Doroti.Framework.Rendering.RenderBox)child).size.height;
            maxWidth = Math.Max(maxWidth, ((global::Doroti.Framework.Rendering.RenderBox)child).size.width);
        })));
        if ((this.isAbove && ((global::Doroti.Framework.Widgets.ToolbarItemsParentData)navButtonParentData).shouldPaint))
        {
            navButtonParentData.offset = new global::Doroti.Ui.Offset(0.0, currentY);
            currentY += ((global::Doroti.Framework.Rendering.RenderBox)navButton).size.height;
            maxWidth = Math.Max(maxWidth, ((global::Doroti.Framework.Rendering.RenderBox)navButton).size.width);
        }
        return new global::Doroti.Ui.Size(maxWidth, currentY);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _placeChildren()
    {
        size = (this.overflowOpen ? _placeChildrenVertically() : _placeChildrenHorizontally());
    }

    internal virtual void _resizeChildrenWhenOverflow()
    {
        if (!this.overflowOpen)
        {
            return;
        }
        global::Doroti.Framework.Rendering.RenderBox navButton = this.firstChild!;
        var i = -1L;
        visitChildren(((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)((renderObjectChild) =>
        {
            var child = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)renderObjectChild)!;
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
            i++;
            if ((object.Equals(((global::Doroti.Framework.Rendering.RenderBox)renderObjectChild), navButton)))
            {
                return;
            }
            if (!_shouldPaintChild(((global::Doroti.Framework.Rendering.RenderBox)renderObjectChild), i))
            {
                childParentData.shouldPaint = false;
                return;
            }
            child.layout(global::Doroti.Framework.Rendering.BoxConstraints.CreateTightFor(width: this.size.width), parentUsesSize: true);
        })));
    }

    public override void performLayout()
    {
        _lastIndexThatFits = -1L;
        if ((this.firstChild is null))
        {
            size = ((global::Doroti.Framework.Rendering.BoxConstraints)this.constraints).smallest;
            return;
        }
        _layoutChildren();
        _placeChildren();
        _resizeChildrenWhenOverflow();
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        visitChildren(((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)((renderObjectChild) =>
        {
            var child = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)renderObjectChild)!;
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
            if (!((global::Doroti.Framework.Widgets.ToolbarItemsParentData)childParentData).shouldPaint)
            {
                return;
            }
            context.paintChild(child, (childParentData.offset + offset));
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
            bool isHit = result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: ((global::System.Func<global::Doroti.Framework.Rendering.BoxHitTestResult, Offset, bool>)((result, transformed) =>
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

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            ((dynamic)child).attach(owner);
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void detach()
    {
        base.detach();
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            ((dynamic)child).detach();
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void redepthChildren()
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            redepthChild(child);
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void visitChildren(global::System.Action<RenderObject> visitor)
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            visitor(child);
            var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
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

    public override List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        var children = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
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
                var childParentData = ((global::Doroti.Framework.Widgets.ToolbarItemsParentData?)(object?)child.parentData!)!;
                child = childParentData.nextSibling!;
            }
        }
        return children;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TextSelectionToolbarContainer__text_selection_toolbar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;
    internal static Color _defaultColorLight = new global::Doroti.Ui.Color(4294967295L);
    internal static Color _defaultColorDark = new global::Doroti.Ui.Color(4282532418L);

    internal _TextSelectionToolbarContainer__text_selection_toolbar(global::Doroti.Framework.Widgets.Widget child)
    {
        this.child = child;
    }

    internal static global::Doroti.Ui.Color _getColor(ColorScheme colorScheme)
    {
        bool isDefaultSurface = (colorScheme.brightness switch { Brightness.light => DartRuntimePrimitives.Identical(ThemeData.Create().colorScheme.surface, colorScheme.surface), Brightness.dark => DartRuntimePrimitives.Identical(ThemeData.Create().colorScheme.surface, colorScheme.surface), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        if (!isDefaultSurface)
        {
            return ((global::Doroti.Ui.Color)(object?)colorScheme.surface);
        }
        return ((global::Doroti.Ui.Color)(object?)(colorScheme.brightness switch { Brightness.light => _defaultColorLight, Brightness.dark => _defaultColorDark, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new Material(borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular((Text_selection_toolbarLibrary._kToolbarHeight / 2L))), clipBehavior: Clip.antiAlias, color: _TextSelectionToolbarContainer__text_selection_toolbar._getColor(theme.colorScheme), elevation: 1.0, type: MaterialType.card, child: this.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TextSelectionToolbarOverflowButton__text_selection_toolbar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Widgets.Icon icon { get; private set; } = default!;
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual string? tooltip { get; private set; }

    internal _TextSelectionToolbarOverflowButton__text_selection_toolbar(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Icon icon = default!, global::System.Action? onPressed = null, string? tooltip = null) : base(key: key)
    {
        this.icon = icon;
        this.onPressed = onPressed;
        this.tooltip = tooltip;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new Material(type: MaterialType.card, color: new global::Doroti.Ui.Color(0L), child: new IconButton(icon: this.icon, onPressed: this.onPressed, tooltip: this.tooltip)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
