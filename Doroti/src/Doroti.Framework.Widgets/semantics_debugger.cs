// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/semantics_debugger.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public class SemanticsDebugger : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle labelStyle { get; private set; } = default!;

    public SemanticsDebugger(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget child = default!, global::Doroti.Generated.Framework.Painting.TextStyle labelStyle = default!) : base(key: key)
    {
        global::Doroti.Generated.Framework.Painting.TextStyle __labelStyle = labelStyle ?? new global::Doroti.Generated.Framework.Painting.TextStyle(color: new global::Doroti.Ui.Color(0xFF000000), fontSize: 10.0, height: 0.8);
        this.child = child;
        this.labelStyle = __labelStyle;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SemanticsDebuggerState__semantics_debugger());
}

internal class _SemanticsDebuggerState__semantics_debugger : State<SemanticsDebugger>, WidgetsBindingObserver
{
    internal virtual global::Doroti.Generated.Framework.Rendering.PipelineOwner? _pipelineOwner { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Semantics.SemanticsHandle? _semanticsHandle { get; set; } = default;
    internal virtual long _generation { get; set; } = 0L;
    internal virtual Offset? _lastPointerDownLocation { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _semanticsHandle = global::Doroti.Generated.Framework.Semantics.SemanticsBinding.instance.ensureSemantics();
        WidgetsBinding.instance.addObserver(this);
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Generated.Framework.Rendering.PipelineOwner newOwner__1779 = ((global::Doroti.Generated.Framework.Rendering.PipelineOwner)(object?)View.pipelineOwnerOf(this.context));
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Generated.Framework.Rendering.PipelineOwner)newOwner__1779).semanticsOwner is not null));
        if ((!object.Equals(newOwner__1779, this._pipelineOwner)))
        {
            this._pipelineOwner?.semanticsOwner?.removeListener(() => this._update());
            ((global::Doroti.Generated.Framework.Rendering.PipelineOwner)newOwner__1779).semanticsOwner!.addListener(() => this._update());
            _pipelineOwner = newOwner__1779;
        }
    }

    public override void dispose()
    {
        this._pipelineOwner?.semanticsOwner?.removeListener(() => this._update());
        this._semanticsHandle?.dispose();
        WidgetsBinding.instance.removeObserver(this);
        base.dispose();
    }

    public virtual void didChangeMetrics()
    {
        setState(((global::System.Action)(() => {
})));
    }

    internal virtual void _update()
    {
        _generation++;
        global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timeStamp) => {
if (this.mounted)
{
    setState(((global::System.Action)(() => {
})));
}
})), debugLabel: "SemanticsDebugger.update");
    }

    internal virtual void _handlePointerDown(global::Doroti.Generated.Framework.Gestures.PointerDownEvent @event)
    {
        setState(((global::System.Action)(() => {
_lastPointerDownLocation = (@event.position * View.of(this.context).devicePixelRatio);
})));
    }

    internal virtual void _handleTap()
    {
        DartRuntimePrimitives.Assert(() => (this._lastPointerDownLocation is not null));
        _performAction(DartRuntimePrimitives.RequireValue(this._lastPointerDownLocation), SemanticsAction.tap);
        setState(((global::System.Action)(() => {
_lastPointerDownLocation = null;
})));
    }

    internal virtual void _handleLongPress()
    {
        DartRuntimePrimitives.Assert(() => (this._lastPointerDownLocation is not null));
        _performAction(DartRuntimePrimitives.RequireValue(this._lastPointerDownLocation), SemanticsAction.longPress);
        setState(((global::System.Action)(() => {
_lastPointerDownLocation = null;
})));
    }

    internal virtual void _handlePanEnd(global::Doroti.Generated.Framework.Gestures.DragEndDetails details)
    {
        double vx__3964 = ((global::Doroti.Generated.Framework.Gestures.DragEndDetails)details).velocity.pixelsPerSecond.dx;
        double vy__4023 = ((global::Doroti.Generated.Framework.Gestures.DragEndDetails)details).velocity.pixelsPerSecond.dy;
        if ((vx__3964.abs() == vy__4023.abs()))
        {
            return;
        }
        if ((vx__3964.abs() > vy__4023.abs()))
        {
            if ((Math.Sign(vx__3964) < 0L))
            {
                _performAction(DartRuntimePrimitives.RequireValue(this._lastPointerDownLocation), SemanticsAction.decrease);
                _performAction(DartRuntimePrimitives.RequireValue(this._lastPointerDownLocation), SemanticsAction.scrollLeft);
            }
            else
            {
                _performAction(DartRuntimePrimitives.RequireValue(this._lastPointerDownLocation), SemanticsAction.increase);
                _performAction(DartRuntimePrimitives.RequireValue(this._lastPointerDownLocation), SemanticsAction.scrollRight);
            }
        }
        else
        {
            if ((Math.Sign(vy__4023) < 0L))
            {
                _performAction(DartRuntimePrimitives.RequireValue(this._lastPointerDownLocation), SemanticsAction.scrollUp);
            }
            else
            {
                _performAction(DartRuntimePrimitives.RequireValue(this._lastPointerDownLocation), SemanticsAction.scrollDown);
            }
        }
        setState(((global::System.Action)(() => {
_lastPointerDownLocation = null;
})));
    }

    internal virtual void _performAction(Offset position, SemanticsAction action)
    {
        this._pipelineOwner?.semanticsOwner?.performActionAt(position, action);
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new CustomPaint(foregroundPainter: new _SemanticsDebuggerPainter__semantics_debugger(this._pipelineOwner!, this._generation, this._lastPointerDownLocation, View.of(context).devicePixelRatio, ((SemanticsDebugger)this.widget).labelStyle), child: new GestureDetector(behavior: global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque, onTap: () => this._handleTap(), onLongPress: () => this._handleLongPress(), onPanEnd: (global::System.Action<global::Doroti.Generated.Framework.Gestures.DragEndDetails>)this._handlePanEnd, excludeFromSemantics: true, child: new Listener(onPointerDown: (global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerDownEvent>)this._handlePointerDown, behavior: global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque, child: new _IgnorePointerWithSemantics__semantics_debugger(child: ((SemanticsDebugger)this.widget).child)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SemanticsDebuggerPainter__semantics_debugger : global::Doroti.Generated.Framework.Rendering.CustomPainter
{
    public virtual global::Doroti.Generated.Framework.Rendering.PipelineOwner owner { get; private set; } = default!;
    public virtual long generation { get; private set; } = default!;
    public virtual Offset? pointerPosition { get; private set; }
    public virtual double devicePixelRatio { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle labelStyle { get; private set; } = default!;

    internal _SemanticsDebuggerPainter__semantics_debugger(global::Doroti.Generated.Framework.Rendering.PipelineOwner owner, long generation, Offset? pointerPosition, double devicePixelRatio, global::Doroti.Generated.Framework.Painting.TextStyle labelStyle)
    {
        this.owner = owner;
        this.generation = generation;
        this.pointerPosition = pointerPosition;
        this.devicePixelRatio = devicePixelRatio;
        this.labelStyle = labelStyle;
    }

    internal virtual global::Doroti.Generated.Framework.Semantics.SemanticsNode? _rootSemanticsNode
    {
        get
        {
            return ((global::Doroti.Generated.Framework.Rendering.PipelineOwner)this.owner).semanticsOwner?.rootSemanticsNode;
            return default!;
        }
    }
    public override void paint(Canvas canvas, Size size)
    {
        global::Doroti.Generated.Framework.Semantics.SemanticsNode? rootNode__6339 = this._rootSemanticsNode;
        canvas.save();
        canvas.scale((1.0 / this.devicePixelRatio), (1.0 / this.devicePixelRatio));
        if ((rootNode__6339 is not null))
        {
            _paint(canvas, rootNode__6339, _findDepth(rootNode__6339), 0L, 0L);
        }
        if ((this.pointerPosition is not null))
        {
            Offset pointerPosition__value6557 = DartRuntimePrimitives.RequireValue(pointerPosition);
            var paint__6596 = new global::Doroti.Ui.Paint();
            paint__6596.color = new global::Doroti.Ui.Color(2130743551L);
            canvas.drawCircle(DartRuntimePrimitives.RequireValue(this.pointerPosition), (10.0 * this.devicePixelRatio), paint__6596);
        }
        canvas.restore();
    }

    public override bool shouldRepaint(global::Doroti.Generated.Framework.Rendering.CustomPainter oldDelegate)
    {
        var __oldDelegate = (_SemanticsDebuggerPainter__semantics_debugger)(object)oldDelegate;
        return (((!object.Equals(this.owner, ((_SemanticsDebuggerPainter__semantics_debugger)__oldDelegate).owner)) || (this.generation != ((_SemanticsDebuggerPainter__semantics_debugger)__oldDelegate).generation)) || (!object.Equals(this.pointerPosition, ((_SemanticsDebuggerPainter__semantics_debugger)__oldDelegate).pointerPosition)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string getMessage(global::Doroti.Generated.Framework.Semantics.SemanticsNode node)
    {
        global::Doroti.Generated.Framework.Semantics.SemanticsData data__7077 = ((global::Doroti.Generated.Framework.Semantics.SemanticsData)(object?)node.getSemanticsData());
        var annotations__7119 = new List<string>();
        var wantsTap__7154 = false;
        if ((!object.Equals(((global::Doroti.Generated.Framework.Semantics.SemanticsData)data__7077).flagsCollection.isChecked, CheckedState.none)))
        {
            annotations__7119.Add(((object.Equals(((global::Doroti.Generated.Framework.Semantics.SemanticsData)data__7077).flagsCollection.isChecked, CheckedState.isTrue)) ? "checked" : "unchecked"));
            wantsTap__7154 = true;
        }
        if (((global::Doroti.Generated.Framework.Semantics.SemanticsData)data__7077).flagsCollection.isTextField)
        {
            annotations__7119.Add("textfield");
            wantsTap__7154 = true;
        }
        if (data__7077.hasAction(SemanticsAction.tap))
        {
            if (!wantsTap__7154)
            {
                annotations__7119.Add("button");
            }
        }
        else
        {
            if (wantsTap__7154)
            {
                annotations__7119.Add("disabled");
            }
        }
        if (data__7077.hasAction(SemanticsAction.longPress))
        {
            annotations__7119.Add("long-pressable");
        }
        bool isScrollable__7811 = (((data__7077.hasAction(SemanticsAction.scrollLeft) || data__7077.hasAction(SemanticsAction.scrollRight)) || data__7077.hasAction(SemanticsAction.scrollUp)) || data__7077.hasAction(SemanticsAction.scrollDown));
        bool isAdjustable__8055 = (data__7077.hasAction(SemanticsAction.increase) || data__7077.hasAction(SemanticsAction.decrease));
        if (isScrollable__7811)
        {
            annotations__7119.Add("scrollable");
        }
        if (isAdjustable__8055)
        {
            annotations__7119.Add("adjustable");
        }
        string message__8318 = default!;
        bool shouldIgnoreDuplicatedLabel__8504 = ((object.Equals(global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Generated.Framework.Foundation.TargetPlatform.android)) && (((global::Doroti.Generated.Framework.Semantics.SemanticsData)data__7077).attributedLabel.@string == ((global::Doroti.Generated.Framework.Semantics.SemanticsData)data__7077).tooltip));
        string tooltipAndLabel__8663 = string.Join("\n", new List<string>());
        if ((tooltipAndLabel__8663.Length == 0))
        {
            message__8318 = string.Join("; ", annotations__7119);
        }
        else
        {
            string effectiveLabel__8984 = default!;
            if ((((global::Doroti.Generated.Framework.Semantics.SemanticsData)data__7077).textDirection is null))
            {
                effectiveLabel__8984 = $"{(global::Doroti.Generated.Framework.Foundation.Unicode.FSI)}{tooltipAndLabel__8663}{(global::Doroti.Generated.Framework.Foundation.Unicode.PDI)}";
                annotations__7119.Insert(checked((int)0L), "MISSING TEXT DIRECTION");
            }
            else
            {
                effectiveLabel__8984 = (DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsData)data__7077).textDirection) switch { TextDirection.rtl => $"{(global::Doroti.Generated.Framework.Foundation.Unicode.RLI)}{tooltipAndLabel__8663}{(global::Doroti.Generated.Framework.Foundation.Unicode.PDI)}", TextDirection.ltr => tooltipAndLabel__8663, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            }
            if (!System.Linq.Enumerable.Any(annotations__7119))
            {
                message__8318 = effectiveLabel__8984;
            }
            else
            {
                message__8318 = $"{effectiveLabel__8984} ({string.Join("; ", annotations__7119)})";
            }
        }
        return message__8318.Trim();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _paintMessage(Canvas canvas, global::Doroti.Generated.Framework.Semantics.SemanticsNode node)
    {
        string message__9656 = ((string)(object?)getMessage(node));
        if ((message__9656.Length == 0))
        {
            return;
        }
        global::Doroti.Ui.Rect rect__9746 = ((global::Doroti.Ui.Rect)(object?)((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).rect);
        canvas.save();
        canvas.clipRect(rect__9746);
        var textPainter__9820 = ((Func<global::Doroti.Generated.Framework.Painting.TextPainter>)(() =>
{            var __cascade = new global::Doroti.Generated.Framework.Painting.TextPainter();
            __cascade.text = new global::Doroti.Generated.Framework.Painting.TextSpan(style: this.labelStyle, text: message__9656);
            __cascade.textDirection = TextDirection.ltr;
            __cascade.textAlign = global::Doroti.Ui.TextAlign.center;
            __cascade.layout(maxWidth: rect__9746.width);
            return __cascade;        }))();
        textPainter__9820.paint(canvas, global::Doroti.Generated.Framework.Painting.Alignment.center.inscribe(((global::Doroti.Generated.Framework.Painting.TextPainter)textPainter__9820).size, rect__9746).topLeft);
        textPainter__9820.dispose();
        canvas.restore();
    }

    internal virtual long _findDepth(global::Doroti.Generated.Framework.Semantics.SemanticsNode node)
    {
        if ((!((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).hasChildren || ((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).mergeAllDescendantsIntoThisNode))
        {
            return 1L;
        }
        var childrenDepth__10383 = 0L;
        node.visitChildren(((global::System.Func<global::Doroti.Generated.Framework.Semantics.SemanticsNode, bool>)((child) => {
childrenDepth__10383 = Math.Max(childrenDepth__10383, _findDepth(child));
return true;
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        return (childrenDepth__10383 + 1L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _paint(Canvas canvas, global::Doroti.Generated.Framework.Semantics.SemanticsNode node, long rank, long indexInParent, long level)
    {
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).traversalChildIdentifier is not null))
        {
            return;
        }
        canvas.save();
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).transform is not null))
        {
            canvas.transform(((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).transform!.storage);
        }
        global::Doroti.Ui.Rect rect__10860 = ((global::Doroti.Ui.Rect)(object?)((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).rect);
        if (!rect__10860.isEmpty)
        {
            global::Doroti.Ui.Color lineColor__10921 = ((global::Doroti.Ui.Color)(object?)_SemanticsDebuggerPainter__semantics_debugger._colorForNode(indexInParent, level));
            global::Doroti.Ui.Rect innerRect__10987 = ((global::Doroti.Ui.Rect)(object?)rect__10860.deflate((rank * 1.0)));
            if (innerRect__10987.isEmpty)
            {
                var fill__11070 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = lineColor__10921;
            __cascade.style = PaintingStyle.fill;
            return __cascade;        }))();
                canvas.drawRect(rect__10860, fill__11070);
            }
            else
            {
                var fill__11221 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = new global::Doroti.Ui.Color(4294967295L);
            __cascade.style = PaintingStyle.fill;
            return __cascade;        }))();
                canvas.drawRect(rect__10860, fill__11221);
                var line__11371 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.strokeWidth = (rank * 2.0);
            __cascade.color = lineColor__10921;
            __cascade.style = PaintingStyle.stroke;
            return __cascade;        }))();
                canvas.drawRect(innerRect__10987, line__11371);
            }
            _paintMessage(canvas, node);
        }
        if (!((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).mergeAllDescendantsIntoThisNode)
        {
            long childRank__11651 = (rank - 1L);
            long childLevel__11689 = (level + 1L);
            var childIndex__11723 = 0L;
            node.visitChildren(((global::System.Func<global::Doroti.Generated.Framework.Semantics.SemanticsNode, bool>)((child) => {
_paint(canvas, child, childRank__11651, childIndex__11723, childLevel__11689);
childIndex__11723 += 1L;
return true;
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        }
        canvas.restore();
    }

    internal static global::Doroti.Ui.Color _colorForNode(long index, long level)
    {
        return ((global::Doroti.Ui.Color)(object?)new global::Doroti.Generated.Framework.Painting.HSLColor(1.0, (360.0 * new DartRandom(_SemanticsDebuggerPainter__semantics_debugger._getColorSeed(index, level)).nextDouble()), 1.0, 0.7).toColor());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static long _getColorSeed(long level, long index)
    {
        return ((level * 10000L) + index);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _IgnorePointerWithSemantics__semantics_debugger : SingleChildRenderObjectWidget
{
    internal _IgnorePointerWithSemantics__semantics_debugger(Widget? child = null) : base(child: child)
    {
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new _RenderIgnorePointerWithSemantics__semantics_debugger());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _RenderIgnorePointerWithSemantics__semantics_debugger : global::Doroti.Generated.Framework.Rendering.RenderProxyBox
{
    internal _RenderIgnorePointerWithSemantics__semantics_debugger()
    {
    }

    public override bool hitTest(global::Doroti.Generated.Framework.Rendering.BoxHitTestResult result, Offset position) => false;
}

