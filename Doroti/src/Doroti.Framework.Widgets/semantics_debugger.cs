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

namespace Doroti.Framework.Widgets;

public class SemanticsDebugger : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle labelStyle { get; private set; } = default!;

    public SemanticsDebugger(global::Doroti.Framework.Foundation.Key? key = null, Widget child = default!, global::Doroti.Framework.Painting.TextStyle labelStyle = default!) : base(key: key)
    {
        global::Doroti.Framework.Painting.TextStyle __labelStyle = labelStyle ?? new global::Doroti.Framework.Painting.TextStyle(color: new global::Doroti.Ui.Color(0xFF000000), fontSize: 10.0, height: 0.8);
        this.child = child;
        this.labelStyle = __labelStyle;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SemanticsDebuggerState__semantics_debugger());
}

internal class _SemanticsDebuggerState__semantics_debugger : State<SemanticsDebugger>, WidgetsBindingObserver
{
    internal virtual global::Doroti.Framework.Rendering.PipelineOwner? _pipelineOwner { get; set; } = default;
    internal virtual global::Doroti.Framework.Semantics.SemanticsHandle? _semanticsHandle { get; set; } = default;
    internal virtual long _generation { get; set; } = 0L;
    internal virtual Offset? _lastPointerDownLocation { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _semanticsHandle = global::Doroti.Framework.Semantics.SemanticsBinding.instance.ensureSemantics();
        WidgetsBinding.instance.addObserver(this);
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Framework.Rendering.PipelineOwner newOwner = ((global::Doroti.Framework.Rendering.PipelineOwner)(object?)View.pipelineOwnerOf(this.context));
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Rendering.PipelineOwner)newOwner).semanticsOwner is not null));
        if ((!object.Equals(newOwner, this._pipelineOwner)))
        {
            this._pipelineOwner?.semanticsOwner?.removeListener(this._update);
            ((global::Doroti.Framework.Rendering.PipelineOwner)newOwner).semanticsOwner!.addListener(this._update);
            _pipelineOwner = newOwner;
        }
    }

    public override void dispose()
    {
        this._pipelineOwner?.semanticsOwner?.removeListener(this._update);
        this._semanticsHandle?.dispose();
        WidgetsBinding.instance.removeObserver(this);
        base.dispose();
    }

    public virtual void didChangeMetrics()
    {
        setState(((global::System.Action)(() =>
        {
        })));
    }

    internal virtual void _update()
    {
        _generation++;
        global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timeStamp) =>
        {
            if (this.mounted)
            {
                setState(((global::System.Action)(() =>
                {
                })));
            }
        })), debugLabel: "SemanticsDebugger.update");
    }

    internal virtual void _handlePointerDown(global::Doroti.Framework.Gestures.PointerDownEvent @event)
    {
        setState(((global::System.Action)(() =>
        {
            _lastPointerDownLocation = (@event.position * View.of(this.context).devicePixelRatio);
        })));
    }

    internal virtual void _handleTap()
    {
        DartRuntimePrimitives.Assert(() => (this._lastPointerDownLocation is not null));
        _performAction(DartRuntimePrimitives.RequireValue(this._lastPointerDownLocation), SemanticsAction.tap);
        setState(((global::System.Action)(() =>
        {
            _lastPointerDownLocation = null;
        })));
    }

    internal virtual void _handleLongPress()
    {
        DartRuntimePrimitives.Assert(() => (this._lastPointerDownLocation is not null));
        _performAction(DartRuntimePrimitives.RequireValue(this._lastPointerDownLocation), SemanticsAction.longPress);
        setState(((global::System.Action)(() =>
        {
            _lastPointerDownLocation = null;
        })));
    }

    internal virtual void _handlePanEnd(global::Doroti.Framework.Gestures.DragEndDetails details)
    {
        double vx = ((global::Doroti.Framework.Gestures.DragEndDetails)details).velocity.pixelsPerSecond.dx;
        double vy = ((global::Doroti.Framework.Gestures.DragEndDetails)details).velocity.pixelsPerSecond.dy;
        if ((vx.abs() == vy.abs()))
        {
            return;
        }
        if ((vx.abs() > vy.abs()))
        {
            if ((Math.Sign(vx) < 0L))
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
            if ((Math.Sign(vy) < 0L))
            {
                _performAction(DartRuntimePrimitives.RequireValue(this._lastPointerDownLocation), SemanticsAction.scrollUp);
            }
            else
            {
                _performAction(DartRuntimePrimitives.RequireValue(this._lastPointerDownLocation), SemanticsAction.scrollDown);
            }
        }
        setState(((global::System.Action)(() =>
        {
            _lastPointerDownLocation = null;
        })));
    }

    internal virtual void _performAction(Offset position, SemanticsAction action)
    {
        this._pipelineOwner?.semanticsOwner?.performActionAt(position, action);
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new CustomPaint(foregroundPainter: new _SemanticsDebuggerPainter__semantics_debugger(this._pipelineOwner!, this._generation, this._lastPointerDownLocation, View.of(context).devicePixelRatio, ((SemanticsDebugger)this.widget).labelStyle), child: new GestureDetector(behavior: global::Doroti.Framework.Rendering.HitTestBehavior.opaque, onTap: () => this._handleTap(), onLongPress: () => this._handleLongPress(), onPanEnd: (global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>)this._handlePanEnd, excludeFromSemantics: true, child: new Listener(onPointerDown: (global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>)this._handlePointerDown, behavior: global::Doroti.Framework.Rendering.HitTestBehavior.opaque, child: new _IgnorePointerWithSemantics__semantics_debugger(child: ((SemanticsDebugger)this.widget).child)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SemanticsDebuggerPainter__semantics_debugger : global::Doroti.Framework.Rendering.CustomPainter
{
    public virtual global::Doroti.Framework.Rendering.PipelineOwner owner { get; private set; } = default!;
    public virtual long generation { get; private set; } = default!;
    public virtual Offset? pointerPosition { get; private set; }
    public virtual double devicePixelRatio { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle labelStyle { get; private set; } = default!;

    internal _SemanticsDebuggerPainter__semantics_debugger(global::Doroti.Framework.Rendering.PipelineOwner owner, long generation, Offset? pointerPosition, double devicePixelRatio, global::Doroti.Framework.Painting.TextStyle labelStyle)
    {
        this.owner = owner;
        this.generation = generation;
        this.pointerPosition = pointerPosition;
        this.devicePixelRatio = devicePixelRatio;
        this.labelStyle = labelStyle;
    }

    internal virtual global::Doroti.Framework.Semantics.SemanticsNode? _rootSemanticsNode
    {
        get
        {
            return ((global::Doroti.Framework.Rendering.PipelineOwner)this.owner).semanticsOwner?.rootSemanticsNode;
            return default!;
        }
    }
    public override void paint(Canvas canvas, Size size)
    {
        global::Doroti.Framework.Semantics.SemanticsNode? rootNode = this._rootSemanticsNode;
        canvas.save();
        canvas.scale((1.0 / this.devicePixelRatio), (1.0 / this.devicePixelRatio));
        if ((rootNode is not null))
        {
            _paint(canvas, rootNode, _findDepth(rootNode), 0L, 0L);
        }
        if ((this.pointerPosition is not null))
        {
            Offset pointerPosition__value6557 = DartRuntimePrimitives.RequireValue(pointerPosition);
            var paintLocal = new global::Doroti.Ui.Paint();
            paintLocal.color = new global::Doroti.Ui.Color(2130743551L);
            canvas.drawCircle(DartRuntimePrimitives.RequireValue(this.pointerPosition), (10.0 * this.devicePixelRatio), paintLocal);
        }
        canvas.restore();
    }

    public override bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate)
    {
        var __oldDelegate = (_SemanticsDebuggerPainter__semantics_debugger)(object)oldDelegate;
        return (((!object.Equals(this.owner, ((_SemanticsDebuggerPainter__semantics_debugger)__oldDelegate).owner)) || (this.generation != ((_SemanticsDebuggerPainter__semantics_debugger)__oldDelegate).generation)) || (!object.Equals(this.pointerPosition, ((_SemanticsDebuggerPainter__semantics_debugger)__oldDelegate).pointerPosition)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string getMessage(global::Doroti.Framework.Semantics.SemanticsNode node)
    {
        global::Doroti.Framework.Semantics.SemanticsData data = ((global::Doroti.Framework.Semantics.SemanticsData)(object?)node.getSemanticsData());
        var annotations = new List<string>();
        var wantsTap = false;
        if ((!object.Equals(((global::Doroti.Framework.Semantics.SemanticsData)data).flagsCollection.isChecked, CheckedState.none)))
        {
            annotations.Add(((object.Equals(((global::Doroti.Framework.Semantics.SemanticsData)data).flagsCollection.isChecked, CheckedState.isTrue)) ? "checked" : "unchecked"));
            wantsTap = true;
        }
        if (((global::Doroti.Framework.Semantics.SemanticsData)data).flagsCollection.isTextField)
        {
            annotations.Add("textfield");
            wantsTap = true;
        }
        if (data.hasAction(SemanticsAction.tap))
        {
            if (!wantsTap)
            {
                annotations.Add("button");
            }
        }
        else
        {
            if (wantsTap)
            {
                annotations.Add("disabled");
            }
        }
        if (data.hasAction(SemanticsAction.longPress))
        {
            annotations.Add("long-pressable");
        }
        bool isScrollable = (((data.hasAction(SemanticsAction.scrollLeft) || data.hasAction(SemanticsAction.scrollRight)) || data.hasAction(SemanticsAction.scrollUp)) || data.hasAction(SemanticsAction.scrollDown));
        bool isAdjustable = (data.hasAction(SemanticsAction.increase) || data.hasAction(SemanticsAction.decrease));
        if (isScrollable)
        {
            annotations.Add("scrollable");
        }
        if (isAdjustable)
        {
            annotations.Add("adjustable");
        }
        string message = default!;
        bool shouldIgnoreDuplicatedLabel = ((object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.android)) && (((global::Doroti.Framework.Semantics.SemanticsData)data).attributedLabel.@string == ((global::Doroti.Framework.Semantics.SemanticsData)data).tooltip));
        string tooltipAndLabel = string.Join("\n", new List<string>());
        if ((tooltipAndLabel.Length == 0))
        {
            message = string.Join("; ", annotations);
        }
        else
        {
            string effectiveLabel = default!;
            if ((((global::Doroti.Framework.Semantics.SemanticsData)data).textDirection is null))
            {
                effectiveLabel = $"{(global::Doroti.Framework.Foundation.Unicode.FSI)}{tooltipAndLabel}{(global::Doroti.Framework.Foundation.Unicode.PDI)}";
                annotations.Insert(checked((int)0L), "MISSING TEXT DIRECTION");
            }
            else
            {
                effectiveLabel = (DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsData)data).textDirection) switch { TextDirection.rtl => $"{(global::Doroti.Framework.Foundation.Unicode.RLI)}{tooltipAndLabel}{(global::Doroti.Framework.Foundation.Unicode.PDI)}", TextDirection.ltr => tooltipAndLabel, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            }
            if (!System.Linq.Enumerable.Any(annotations))
            {
                message = effectiveLabel;
            }
            else
            {
                message = $"{effectiveLabel} ({string.Join("; ", annotations)})";
            }
        }
        return message.Trim();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _paintMessage(Canvas canvas, global::Doroti.Framework.Semantics.SemanticsNode node)
    {
        string message = ((string)(object?)getMessage(node));
        if ((message.Length == 0))
        {
            return;
        }
        global::Doroti.Ui.Rect rectLocal = ((global::Doroti.Ui.Rect)(object?)((global::Doroti.Framework.Semantics.SemanticsNode)node).rect);
        canvas.save();
        canvas.clipRect(rectLocal);
        var textPainter = ((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = new global::Doroti.Framework.Painting.TextPainter();
    __cascade.text = new global::Doroti.Framework.Painting.TextSpan(style: this.labelStyle, text: message);
    __cascade.textDirection = TextDirection.ltr;
    __cascade.textAlign = global::Doroti.Ui.TextAlign.center;
    __cascade.layout(maxWidth: rectLocal.width);
    return __cascade;
}))();
        textPainter.paint(canvas, global::Doroti.Framework.Painting.Alignment.center.inscribe(((global::Doroti.Framework.Painting.TextPainter)textPainter).size, rectLocal).topLeft);
        textPainter.dispose();
        canvas.restore();
    }

    internal virtual long _findDepth(global::Doroti.Framework.Semantics.SemanticsNode node)
    {
        if ((!((global::Doroti.Framework.Semantics.SemanticsNode)node).hasChildren || ((global::Doroti.Framework.Semantics.SemanticsNode)node).mergeAllDescendantsIntoThisNode))
        {
            return 1L;
        }
        var childrenDepth = 0L;
        node.visitChildren(((global::System.Func<global::Doroti.Framework.Semantics.SemanticsNode, bool>)((child) =>
        {
            childrenDepth = Math.Max(childrenDepth, _findDepth(child));
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        return (childrenDepth + 1L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _paint(Canvas canvas, global::Doroti.Framework.Semantics.SemanticsNode node, long rank, long indexInParent, long level)
    {
        if ((((global::Doroti.Framework.Semantics.SemanticsNode)node).traversalChildIdentifier is not null))
        {
            return;
        }
        canvas.save();
        if ((((global::Doroti.Framework.Semantics.SemanticsNode)node).transform is not null))
        {
            canvas.transform(((global::Doroti.Framework.Semantics.SemanticsNode)node).transform!.storage);
        }
        global::Doroti.Ui.Rect rectLocal = ((global::Doroti.Ui.Rect)(object?)((global::Doroti.Framework.Semantics.SemanticsNode)node).rect);
        if (!rectLocal.isEmpty)
        {
            global::Doroti.Ui.Color lineColor = ((global::Doroti.Ui.Color)(object?)_SemanticsDebuggerPainter__semantics_debugger._colorForNode(indexInParent, level));
            global::Doroti.Ui.Rect innerRect = ((global::Doroti.Ui.Rect)(object?)rectLocal.deflate((rank * 1.0)));
            if (innerRect.isEmpty)
            {
                var fillLocal = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = lineColor;
    __cascade.style = PaintingStyle.fill;
    return __cascade;
}))();
                canvas.drawRect(rectLocal, fillLocal);
            }
            else
            {
                var fillAlternate = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = new global::Doroti.Ui.Color(4294967295L);
    __cascade.style = PaintingStyle.fill;
    return __cascade;
}))();
                canvas.drawRect(rectLocal, fillAlternate);
                var line = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.strokeWidth = (rank * 2.0);
    __cascade.color = lineColor;
    __cascade.style = PaintingStyle.stroke;
    return __cascade;
}))();
                canvas.drawRect(innerRect, line);
            }
            _paintMessage(canvas, node);
        }
        if (!((global::Doroti.Framework.Semantics.SemanticsNode)node).mergeAllDescendantsIntoThisNode)
        {
            long childRank = (rank - 1L);
            long childLevel = (level + 1L);
            var childIndex = 0L;
            node.visitChildren(((global::System.Func<global::Doroti.Framework.Semantics.SemanticsNode, bool>)((child) =>
            {
                _paint(canvas, child, childRank, childIndex, childLevel);
                childIndex += 1L;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
        }
        canvas.restore();
    }

    internal static global::Doroti.Ui.Color _colorForNode(long index, long level)
    {
        return ((global::Doroti.Ui.Color)(object?)new global::Doroti.Framework.Painting.HSLColor(1.0, (360.0 * new DartRandom(_SemanticsDebuggerPainter__semantics_debugger._getColorSeed(index, level)).nextDouble()), 1.0, 0.7).toColor());
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

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderIgnorePointerWithSemantics__semantics_debugger());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _RenderIgnorePointerWithSemantics__semantics_debugger : global::Doroti.Framework.Rendering.RenderProxyBox
{
    internal _RenderIgnorePointerWithSemantics__semantics_debugger()
    {
    }

    public override bool hitTest(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position) => false;
}

