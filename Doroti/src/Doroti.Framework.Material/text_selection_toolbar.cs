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

namespace Doroti.Generated.Framework.Material;

public static partial class Text_selection_toolbarLibrary
{
    internal static double _kToolbarHeight = 44.0;
}

public static partial class Text_selection_toolbarLibrary
{
    internal static double _kToolbarContentDistance = 8.0;
}

public class TextSelectionToolbar : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual Offset anchorAbove { get; private set; } = default!;
    public virtual Offset anchorBelow { get; private set; } = default!;
    public virtual List<global::Doroti.Generated.Framework.Widgets.Widget> children { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Widgets.Widget> toolbarBuilder { get; private set; } = default!;
    public const double kHandleSize = 22.0;
    public static double kToolbarContentDistanceBelow = (kHandleSize - 2.0);

    public TextSelectionToolbar(global::Doroti.Generated.Framework.Foundation.Key? key = null, Offset anchorAbove = default!, Offset anchorBelow = default!, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Widgets.Widget> toolbarBuilder = default!, List<global::Doroti.Generated.Framework.Widgets.Widget> children = default!) : base(key: key)
    {
        global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Widgets.Widget> __toolbarBuilder = toolbarBuilder ?? _defaultToolbarBuilder;
        this.anchorAbove = anchorAbove;
        this.anchorBelow = anchorBelow;
        this.toolbarBuilder = __toolbarBuilder;
        this.children = children;
        System.Diagnostics.Debug.Assert((checked((long)(children.Count)) > 0L));
    }

    internal static global::Doroti.Generated.Framework.Widgets.Widget _defaultToolbarBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _TextSelectionToolbarContainer__text_selection_toolbar(child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Offset anchorAbovePadded__3639 = ((global::Doroti.Ui.Offset)(object?)(this.anchorAbove - new global::Doroti.Ui.Offset(0.0, Text_selectionLibrary._kToolbarContentDistance)));
        global::Doroti.Ui.Offset anchorBelowPadded__3735 = ((global::Doroti.Ui.Offset)(object?)(this.anchorBelow + new global::Doroti.Ui.Offset(0.0, kToolbarContentDistanceBelow)));
        double screenPadding__3836 = CupertinoTextSelectionToolbar.kToolbarScreenPadding;
        double paddingAbove__3922 = (MediaQuery.paddingOf(context).top + screenPadding__3836);
        double availableHeight__4005 = ((anchorAbovePadded__3639.dy - Text_selectionLibrary._kToolbarContentDistance) - paddingAbove__3922);
        bool fitsAbove__4102 = (Text_selection_toolbarLibrary._kToolbarHeight <= availableHeight__4005);
        var localAdjustment__4209 = new global::Doroti.Ui.Offset(screenPadding__3836, paddingAbove__3922);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Padding(padding: new global::Doroti.Generated.Framework.Painting.EdgeInsets(screenPadding__3836, paddingAbove__3922, screenPadding__3836, screenPadding__3836), child: new global::Doroti.Generated.Framework.Widgets.CustomSingleChildLayout(@delegate: new global::Doroti.Generated.Framework.Widgets.TextSelectionToolbarLayoutDelegate(anchorAbove: (anchorAbovePadded__3639 - localAdjustment__4209), anchorBelow: (anchorBelowPadded__3735 - localAdjustment__4209), fitsAbove: fitsAbove__4102), child: new _TextSelectionToolbarOverflowable__text_selection_toolbar(isAbove: fitsAbove__4102, toolbarBuilder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Widgets.Widget>)this.toolbarBuilder, children: this.children))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _TextSelectionToolbarOverflowable__text_selection_toolbar : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual List<global::Doroti.Generated.Framework.Widgets.Widget> children { get; private set; } = default!;
    public virtual bool isAbove { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Widgets.Widget> toolbarBuilder { get; private set; } = default!;

    internal _TextSelectionToolbarOverflowable__text_selection_toolbar(bool isAbove, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Widgets.Widget> toolbarBuilder, List<global::Doroti.Generated.Framework.Widgets.Widget> children)
    {
        this.isAbove = isAbove;
        this.toolbarBuilder = toolbarBuilder;
        this.children = children;
        System.Diagnostics.Debug.Assert((checked((long)(children.Count)) > 0L));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _TextSelectionToolbarOverflowableState__text_selection_toolbar());
}

public class _TextSelectionToolbarOverflowableState__text_selection_toolbar : global::Doroti.Generated.Framework.Widgets.State<_TextSelectionToolbarOverflowable__text_selection_toolbar>, global::Doroti.Generated.Framework.Widgets.TickerProviderStateMixin<_TextSelectionToolbarOverflowable__text_selection_toolbar>
{
    internal virtual bool _overflowOpen { get; set; } = false;
    internal virtual global::Doroti.Generated.Framework.Foundation.UniqueKey _containerKey { get; set; } = new global::Doroti.Generated.Framework.Foundation.UniqueKey();
    public virtual HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual void _reset()
    {
        _containerKey = new global::Doroti.Generated.Framework.Foundation.UniqueKey();
        _overflowOpen = false;
    }

    public override void didUpdateWidget(_TextSelectionToolbarOverflowable__text_selection_toolbar oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.listEquals(((_TextSelectionToolbarOverflowable__text_selection_toolbar)this.widget).children.Cast<_TextSelectionToolbarOverflowable__text_selection_toolbar>().ToList(), ((_TextSelectionToolbarOverflowable__text_selection_toolbar)oldWidget).children.Cast<_TextSelectionToolbarOverflowable__text_selection_toolbar>().ToList()))
        {
            _reset();
        }
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        MaterialLocalizations localizations__7271 = MaterialLocalizations.of(context);
        global::Doroti.Ui.TextDirection textDirection__7346 = Directionality.of(context);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _TextSelectionToolbarTrailingEdgeAlign__text_selection_toolbar(key: this._containerKey, overflowOpen: this._overflowOpen, textDirection: textDirection__7346, child: new global::Doroti.Generated.Framework.Widgets.AnimatedSize(duration: Duration.Create(milliseconds: 140L), child: this.widget.toolbarBuilder(context, new _TextSelectionToolbarItemsLayout__text_selection_toolbar(isAbove: ((_TextSelectionToolbarOverflowable__text_selection_toolbar)this.widget).isAbove, overflowOpen: this._overflowOpen, textDirection: textDirection__7346, children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection7958 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); __collection7958.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new _TextSelectionToolbarOverflowButton__text_selection_toolbar(key: (this._overflowOpen ? StandardComponentTypeMembers.key(global::Doroti.Generated.Framework.Widgets.StandardComponentType.backButton) : StandardComponentTypeMembers.key(global::Doroti.Generated.Framework.Widgets.StandardComponentType.moreButton)), icon: new global::Doroti.Generated.Framework.Widgets.Icon((this._overflowOpen ? Icons.arrow_back : Icons.more_vert)), onPressed: ((global::System.Action)(() => {
setState(((global::System.Action)(() => {
_overflowOpen = !this._overflowOpen;
})));
})), tooltip: (this._overflowOpen ? localizations__7271.backButtonTooltip : localizations__7271.moreButtonTooltip)))); __collection7958.AddRange(((_TextSelectionToolbarOverflowable__text_selection_toolbar)this.widget).children); return __collection7958; }))())))));
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

    public override void dispose()
    {
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
        this._tickerModeNotifier = null;
        base.dispose();
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

internal class _TextSelectionToolbarTrailingEdgeAlign__text_selection_toolbar : global::Doroti.Generated.Framework.Widgets.SingleChildRenderObjectWidget
{
    public virtual bool overflowOpen { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;

    internal _TextSelectionToolbarTrailingEdgeAlign__text_selection_toolbar(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget child = default!, bool overflowOpen = default!, TextDirection textDirection = default!) : base(key: key, child: child)
    {
        this.overflowOpen = overflowOpen;
        this.textDirection = textDirection;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new _TextSelectionToolbarTrailingEdgeAlignRenderBox__text_selection_toolbar(overflowOpen: this.overflowOpen, textDirection: this.textDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_TextSelectionToolbarTrailingEdgeAlignRenderBox__text_selection_toolbar)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_TextSelectionToolbarTrailingEdgeAlignRenderBox__text_selection_toolbar>)(() =>
{            var __cascade = __renderObject;
            __cascade.overflowOpen = this.overflowOpen;
            __cascade.textDirection = this.textDirection;
            return __cascade;        }))());
    }

}

public class _TextSelectionToolbarTrailingEdgeAlignRenderBox__text_selection_toolbar : global::Doroti.Generated.Framework.Rendering.RenderProxyBox
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
        var childParentData__12190 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)this.child!.parentData!)!;
        childParentData__12190.offset = new global::Doroti.Ui.Offset(((object.Equals(this.textDirection, TextDirection.rtl)) ? 0.0 : (this.size.width - this.child!.size.width)), 0.0);
    }

    public virtual void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)
    {
        var childParentData__12521 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)this.child!.parentData!)!;
        context.paintChild(this.child!, (childParentData__12521.offset + offset));
    }

    public virtual bool hitTestChildren(global::Doroti.Generated.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        var childParentData__12886 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)this.child!.parentData!)!;
        return result.addWithPaintOffset(offset: childParentData__12886.offset, position: position, hitTest: ((global::System.Func<global::Doroti.Generated.Framework.Rendering.BoxHitTestResult, Offset, bool>)((result, transformed) => {
DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - childParentData__12886.offset))));
return this.child!.hitTest(result, position: transformed);
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void setupParentData(global::Doroti.Generated.Framework.Rendering.RenderObject child)
    {
        var __child = (global::Doroti.Generated.Framework.Rendering.RenderBox)(object)child;
        if ((__child.parentData is not global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData))
        {
            __child.parentData = new global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData();
        }
    }

    public virtual void applyPaintTransform(global::Doroti.Generated.Framework.Rendering.RenderObject child, Matrix4 transform)
    {
        var childParentData__13525 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)((global::Doroti.Generated.Framework.Rendering.RenderObject)child).parentData!)!;
        transform.translateByDouble(childParentData__13525.offset.dx, childParentData__13525.offset.dy, 0, 1);
        base.applyPaintTransform(child, transform);
    }

}

internal class _TextSelectionToolbarItemsLayout__text_selection_toolbar : global::Doroti.Generated.Framework.Widgets.MultiChildRenderObjectWidget
{
    public virtual bool isAbove { get; private set; } = default!;
    public virtual bool overflowOpen { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;

    internal _TextSelectionToolbarItemsLayout__text_selection_toolbar(bool isAbove, bool overflowOpen, TextDirection textDirection, List<global::Doroti.Generated.Framework.Widgets.Widget> children) : base(children: children)
    {
        this.isAbove = isAbove;
        this.overflowOpen = overflowOpen;
        this.textDirection = textDirection;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new _RenderTextSelectionToolbarItemsLayout__text_selection_toolbar(isAbove: this.isAbove, overflowOpen: this.overflowOpen, textDirection: this.textDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderTextSelectionToolbarItemsLayout__text_selection_toolbar)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderTextSelectionToolbarItemsLayout__text_selection_toolbar>)(() =>
{            var __cascade = __renderObject;
            __cascade.isAbove = this.isAbove;
            __cascade.textDirection = this.textDirection;
            __cascade.overflowOpen = this.overflowOpen;
            return __cascade;        }))());
    }

    public override _TextSelectionToolbarItemsLayoutElement__text_selection_toolbar createElement() => new _TextSelectionToolbarItemsLayoutElement__text_selection_toolbar(this);
}

public class _TextSelectionToolbarItemsLayoutElement__text_selection_toolbar : global::Doroti.Generated.Framework.Widgets.MultiChildRenderObjectElement
{
    internal _TextSelectionToolbarItemsLayoutElement__text_selection_toolbar(global::Doroti.Generated.Framework.Widgets.MultiChildRenderObjectWidget widget) : base(widget)
    {
    }

    internal static bool _shouldPaint(global::Doroti.Generated.Framework.Widgets.Element child)
    {
        return (((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)((global::Doroti.Generated.Framework.Rendering.ParentData?)((dynamic)((global::Doroti.Generated.Framework.Widgets.Element)child).renderObject!).parentData)!)!).shouldPaint;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugVisitOnstageChildren(global::System.Action<global::Doroti.Generated.Framework.Widgets.Element> visitor)
    {
        this.children.where(_shouldPaint).forEach((__arg0) => ((global::System.Action<global::Doroti.Generated.Framework.Widgets.Element>)visitor)(__arg0));
    }

}

public class _RenderTextSelectionToolbarItemsLayout__text_selection_toolbar : global::Doroti.Generated.Framework.Rendering.RenderBox, global::Doroti.Generated.Framework.Rendering.ContainerRenderObjectMixin<global::Doroti.Generated.Framework.Rendering.RenderBox, global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData>
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
        global::Doroti.Generated.Framework.Rendering.BoxConstraints sizedConstraints__16591 = (this._overflowOpen ? this.constraints : global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateLoose(new global::Doroti.Ui.Size(((global::Doroti.Generated.Framework.Rendering.BoxConstraints)this.constraints).maxWidth, Text_selection_toolbarLibrary._kToolbarHeight)));
        var i__16732 = -1L;
        var width__16748 = 0.0;
        visitChildren(((global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject>)((renderObjectChild) => {
i__16732++;
if (((this._lastIndexThatFits != -1L) && !this.overflowOpen))
{
    return;
}
var child__17182 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)renderObjectChild)!;
child__17182.layout(sizedConstraints__16591.loosen(), parentUsesSize: true);
width__16748 += ((global::Doroti.Generated.Framework.Rendering.RenderBox)child__17182).size.width;
if (((width__16748 > ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)sizedConstraints__16591).maxWidth) && (this._lastIndexThatFits == -1L)))
{
    _lastIndexThatFits = (i__16732 - 1L);
}
})));
        global::Doroti.Generated.Framework.Rendering.RenderBox navButton__17618 = this.firstChild!;
        if ((((this._lastIndexThatFits != -1L) && (this._lastIndexThatFits == (this.childCount - 2L))) && ((width__16748 - ((global::Doroti.Generated.Framework.Rendering.RenderBox)navButton__17618).size.width) <= ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)sizedConstraints__16591).maxWidth)))
        {
            _lastIndexThatFits = -1L;
        }
    }

    internal virtual bool _shouldPaintChild(global::Doroti.Generated.Framework.Rendering.RenderObject renderObjectChild, long index)
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
        global::Doroti.Generated.Framework.Rendering.RenderBox navButton__18663 = this.firstChild!;
        var isRtl__18698 = (object.Equals(this.textDirection, TextDirection.rtl));
        var contentItems__18753 = new List<global::Doroti.Generated.Framework.Rendering.RenderBox>();
        var totalWidth__18792 = 0.0;
        var maxHeight__18818 = 0.0;
        var i__18903 = -1L;
        visitChildren(((global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject>)((renderObjectChild) => {
var child__18976 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)renderObjectChild)!;
var childParentData__19028 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child__18976.parentData!)!;
i__18903++;
if (!_shouldPaintChild(child__18976, i__18903))
{
    childParentData__19028.shouldPaint = false;
}
else
{
    childParentData__19028.shouldPaint = true;
    totalWidth__18792 += ((global::Doroti.Generated.Framework.Rendering.RenderBox)child__18976).size.width;
    maxHeight__18818 = Math.Max(maxHeight__18818, ((global::Doroti.Generated.Framework.Rendering.RenderBox)child__18976).size.height);
    if ((!object.Equals(child__18976, navButton__18663)))
    {
        contentItems__18753.Add(child__18976);
    }
}
})));
        var currentX__19572 = 0.0;
        bool showNavButton__19603 = (this._lastIndexThatFits >= 0L);
        if (isRtl__18698)
        {
            if (showNavButton__19603)
            {
                var navParentData__19780 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)navButton__18663.parentData!)!;
                navParentData__19780.offset = Offset.zero;
                currentX__19572 += ((global::Doroti.Generated.Framework.Rendering.RenderBox)navButton__18663).size.width;
            }
            var rightEdge__20002 = totalWidth__18792;
            foreach (var item__20043 in contentItems__18753)
            {
                rightEdge__20002 -= ((global::Doroti.Generated.Framework.Rendering.RenderBox)item__20043).size.width;
                var itemParentData__20119 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)item__20043.parentData!)!;
                itemParentData__20119.offset = new global::Doroti.Ui.Offset(rightEdge__20002, 0.0);
            }
        }
        else
        {
            foreach (var item__20394 in contentItems__18753)
            {
                var itemParentData__20432 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)item__20394.parentData!)!;
                itemParentData__20432.offset = new global::Doroti.Ui.Offset(currentX__19572, 0.0);
                currentX__19572 += ((global::Doroti.Generated.Framework.Rendering.RenderBox)item__20394).size.width;
            }
            if (showNavButton__19603)
            {
                var navParentData__20682 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)navButton__18663.parentData!)!;
                navParentData__20682.offset = new global::Doroti.Ui.Offset(currentX__19572, 0.0);
            }
        }
        return new global::Doroti.Ui.Size(totalWidth__18792, maxHeight__18818);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Size _placeChildrenVertically()
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox navButton__20956 = this.firstChild!;
        var currentY__20990 = 0.0;
        var maxWidth__21014 = 0.0;
        var navButtonParentData__21041 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)navButton__20956.parentData!)!;
        if (_shouldPaintChild(navButton__20956, 0L))
        {
            navButtonParentData__21041.shouldPaint = true;
            if (!this.isAbove)
            {
                navButtonParentData__21041.offset = Offset.zero;
                currentY__20990 += ((global::Doroti.Generated.Framework.Rendering.RenderBox)navButton__20956).size.height;
                maxWidth__21014 = Math.Max(maxWidth__21014, ((global::Doroti.Generated.Framework.Rendering.RenderBox)navButton__20956).size.width);
            }
        }
        else
        {
            navButtonParentData__21041.shouldPaint = false;
        }
        var i__21461 = -1L;
        visitChildren(((global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject>)((renderObjectChild) => {
var child__21534 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)renderObjectChild)!;
var childParentData__21586 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child__21534.parentData!)!;
i__21461++;
if ((object.Equals(((global::Doroti.Generated.Framework.Rendering.RenderBox)renderObjectChild), navButton__20956)))
{
    return;
}
if (!_shouldPaintChild(child__21534, i__21461))
{
    childParentData__21586.shouldPaint = false;
    return;
}
childParentData__21586.shouldPaint = true;
childParentData__21586.offset = new global::Doroti.Ui.Offset(0.0, currentY__20990);
currentY__20990 += ((global::Doroti.Generated.Framework.Rendering.RenderBox)child__21534).size.height;
maxWidth__21014 = Math.Max(maxWidth__21014, ((global::Doroti.Generated.Framework.Rendering.RenderBox)child__21534).size.width);
})));
        if ((this.isAbove && ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData)navButtonParentData__21041).shouldPaint))
        {
            navButtonParentData__21041.offset = new global::Doroti.Ui.Offset(0.0, currentY__20990);
            currentY__20990 += ((global::Doroti.Generated.Framework.Rendering.RenderBox)navButton__20956).size.height;
            maxWidth__21014 = Math.Max(maxWidth__21014, ((global::Doroti.Generated.Framework.Rendering.RenderBox)navButton__20956).size.width);
        }
        return new global::Doroti.Ui.Size(maxWidth__21014, currentY__20990);
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
        global::Doroti.Generated.Framework.Rendering.RenderBox navButton__22887 = this.firstChild!;
        var i__22920 = -1L;
        visitChildren(((global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject>)((renderObjectChild) => {
var child__22994 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)renderObjectChild)!;
var childParentData__23046 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child__22994.parentData!)!;
i__22920++;
if ((object.Equals(((global::Doroti.Generated.Framework.Rendering.RenderBox)renderObjectChild), navButton__22887)))
{
    return;
}
if (!_shouldPaintChild(((global::Doroti.Generated.Framework.Rendering.RenderBox)renderObjectChild), i__22920))
{
    childParentData__23046.shouldPaint = false;
    return;
}
child__22994.layout(global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateTightFor(width: this.size.width), parentUsesSize: true);
})));
    }

    public override void performLayout()
    {
        _lastIndexThatFits = -1L;
        if ((this.firstChild is null))
        {
            size = ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)this.constraints).smallest;
            return;
        }
        _layoutChildren();
        _placeChildren();
        _resizeChildrenWhenOverflow();
    }

    public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)
    {
        visitChildren(((global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject>)((renderObjectChild) => {
var child__23890 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)renderObjectChild)!;
var childParentData__23942 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child__23890.parentData!)!;
if (!((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData)childParentData__23942).shouldPaint)
{
    return;
}
context.paintChild(child__23890, (childParentData__23942.offset + offset));
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

    public override bool hitTestChildren(global::Doroti.Generated.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__24427 = this.lastChild;
        while ((child__24427 is not null))
        {
            var childParentData__24566 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child__24427.parentData!)!;
            if (!((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData)childParentData__24566).shouldPaint)
            {
                child__24427 = childParentData__24566.previousSibling;
                continue;
            }
            bool isHit__24812 = result.addWithPaintOffset(offset: childParentData__24566.offset, position: position, hitTest: ((global::System.Func<global::Doroti.Generated.Framework.Rendering.BoxHitTestResult, Offset, bool>)((result, transformed) => {
DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - childParentData__24566.offset))));
return child__24427!.hitTest(result, position: transformed);
throw new InvalidOperationException("Dart closure completed without a value.");
})));
            if (isHit__24812)
            {
                return true;
            }
            child__24427 = childParentData__24566.previousSibling;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void visitChildrenForSemantics(global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject> visitor)
    {
        visitChildren(((global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject>)((renderObjectChild) => {
var child__25450 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)renderObjectChild)!;
var childParentData__25502 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child__25450.parentData!)!;
if (((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData)childParentData__25502).shouldPaint)
{
    visitor(((global::Doroti.Generated.Framework.Rendering.RenderBox)renderObjectChild));
}
})));
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

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        RenderBox? child__181803 = this._firstChild;
        while ((child__181803 is not null))
        {
            ((dynamic)child__181803).attach(owner);
            var childParentData__181891 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child__181803.parentData!)!;
            child__181803 = childParentData__181891.nextSibling;
        }
    }

    public override void detach()
    {
        base.detach();
        RenderBox? child__182065 = this._firstChild;
        while ((child__182065 is not null))
        {
            ((dynamic)child__182065).detach();
            var childParentData__182148 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child__182065.parentData!)!;
            child__182065 = childParentData__182148.nextSibling;
        }
    }

    public override void redepthChildren()
    {
        RenderBox? child__182311 = this._firstChild;
        while ((child__182311 is not null))
        {
            redepthChild(child__182311);
            var childParentData__182399 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child__182311.parentData!)!;
            child__182311 = childParentData__182399.nextSibling;
        }
    }

    public override void visitChildren(global::System.Action<RenderObject> visitor)
    {
        RenderBox? child__182587 = this._firstChild;
        while ((child__182587 is not null))
        {
            visitor(child__182587);
            var childParentData__182670 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child__182587.parentData!)!;
            child__182587 = childParentData__182670.nextSibling;
        }
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

    public override List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        var children__183528 = new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>();
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
                var childParentData__183833 = ((global::Doroti.Generated.Framework.Widgets.ToolbarItemsParentData?)(object?)child__183606.parentData!)!;
                child__183606 = childParentData__183833.nextSibling!;
            }
        }
        return children__183528;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TextSelectionToolbarContainer__text_selection_toolbar : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;
    internal static Color _defaultColorLight = new global::Doroti.Ui.Color(4294967295L);
    internal static Color _defaultColorDark = new global::Doroti.Ui.Color(4282532418L);

    internal _TextSelectionToolbarContainer__text_selection_toolbar(global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        this.child = child;
    }

    internal static global::Doroti.Ui.Color _getColor(ColorScheme colorScheme)
    {
        bool isDefaultSurface__26207 = (colorScheme.brightness switch { Brightness.light => DartRuntimePrimitives.Identical(ThemeData.Create().colorScheme.surface, colorScheme.surface), Brightness.dark => DartRuntimePrimitives.Identical(ThemeData.Create().colorScheme.surface, colorScheme.surface), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        if (!isDefaultSurface__26207)
        {
            return ((global::Doroti.Ui.Color)(object?)colorScheme.surface);
        }
        return ((global::Doroti.Ui.Color)(object?)(colorScheme.brightness switch { Brightness.light => _defaultColorLight, Brightness.dark => _defaultColorDark, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ThemeData theme__26740 = Theme.of(context);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new Material(borderRadius: global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular((Text_selection_toolbarLibrary._kToolbarHeight / 2L))), clipBehavior: Clip.antiAlias, color: _TextSelectionToolbarContainer__text_selection_toolbar._getColor(theme__26740.colorScheme), elevation: 1.0, type: MaterialType.card, child: this.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TextSelectionToolbarOverflowButton__text_selection_toolbar : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Icon icon { get; private set; } = default!;
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual string? tooltip { get; private set; }

    internal _TextSelectionToolbarOverflowButton__text_selection_toolbar(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Icon icon = default!, global::System.Action? onPressed = null, string? tooltip = null) : base(key: key)
    {
        this.icon = icon;
        this.onPressed = onPressed;
        this.tooltip = tooltip;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new Material(type: MaterialType.card, color: new global::Doroti.Ui.Color(0L), child: new IconButton(icon: this.icon, onPressed: this.onPressed, tooltip: this.tooltip)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
