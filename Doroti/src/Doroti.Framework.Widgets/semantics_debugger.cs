// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/semantics_debugger.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class SemanticsDebugger : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual TextStyle labelStyle { get; private set; } = default!;

    public SemanticsDebugger(Key? key = null, Widget child = default!, TextStyle labelStyle = default!) : base(key: key)
    {
        TextStyle __labelStyle = labelStyle ?? new TextStyle(color: new Color(0xFF000000), fontSize: 10.0, height: 0.8);
        this.child = child;
        this.labelStyle = __labelStyle;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SemanticsDebuggerState__semantics_debugger());
}

internal class _SemanticsDebuggerState__semantics_debugger : State<SemanticsDebugger>, WidgetsBindingObserver
{
    internal virtual PipelineOwner? _pipelineOwner { get; set; } = default;
    internal virtual SemanticsHandle? _semanticsHandle { get; set; } = default;
    internal virtual long _generation { get; set; } = 0L;
    internal virtual Offset? _lastPointerDownLocation { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _semanticsHandle = Framework.Semantics.SemanticsBinding.instance.ensureSemantics();
        WidgetsBinding.instance.addObserver(this);
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        PipelineOwner newOwner = View.pipelineOwnerOf(context);
        DartRuntimePrimitives.Assert(() => newOwner.semanticsOwner is not null);
        if (!Equals(newOwner, _pipelineOwner))
        {
            _pipelineOwner?.semanticsOwner?.removeListener(_update);
            newOwner.semanticsOwner!.addListener(_update);
            _pipelineOwner = newOwner;
        }
    }

    public override void dispose()
    {
        _pipelineOwner?.semanticsOwner?.removeListener(_update);
        _semanticsHandle?.dispose();
        WidgetsBinding.instance.removeObserver(this);
        base.dispose();
    }

    public virtual void didChangeMetrics()
    {
        setState(() =>
        {
        });
    }

    internal virtual void _update()
    {
        _generation++;
        Scheduler.SchedulerBinding.instance.addPostFrameCallback((timeStamp) =>
        {
            if (mounted)
            {
                setState(() =>
                {
                });
            }
        }, debugLabel: "SemanticsDebugger.update");
    }

    internal virtual void _handlePointerDown(Gestures.PointerDownEvent @event)
    {
        setState(() =>
        {
            _lastPointerDownLocation = @event.position * View.of(context).devicePixelRatio;
        });
    }

    internal virtual void _handleTap()
    {
        DartRuntimePrimitives.Assert(() => _lastPointerDownLocation is not null);
        _performAction(DartRuntimePrimitives.RequireValue(_lastPointerDownLocation), SemanticsAction.tap);
        setState(() =>
        {
            _lastPointerDownLocation = null;
        });
    }

    internal virtual void _handleLongPress()
    {
        DartRuntimePrimitives.Assert(() => _lastPointerDownLocation is not null);
        _performAction(DartRuntimePrimitives.RequireValue(_lastPointerDownLocation), SemanticsAction.longPress);
        setState(() =>
        {
            _lastPointerDownLocation = null;
        });
    }

    internal virtual void _handlePanEnd(DragEndDetails details)
    {
        double vx = details.velocity.pixelsPerSecond.dx;
        double vy = details.velocity.pixelsPerSecond.dy;
        if (vx.abs() == vy.abs())
        {
            return;
        }
        if (vx.abs() > vy.abs())
        {
            if (Math.Sign(vx) < 0L)
            {
                _performAction(DartRuntimePrimitives.RequireValue(_lastPointerDownLocation), SemanticsAction.decrease);
                _performAction(DartRuntimePrimitives.RequireValue(_lastPointerDownLocation), SemanticsAction.scrollLeft);
            }
            else
            {
                _performAction(DartRuntimePrimitives.RequireValue(_lastPointerDownLocation), SemanticsAction.increase);
                _performAction(DartRuntimePrimitives.RequireValue(_lastPointerDownLocation), SemanticsAction.scrollRight);
            }
        }
        else
        {
            if (Math.Sign(vy) < 0L)
            {
                _performAction(DartRuntimePrimitives.RequireValue(_lastPointerDownLocation), SemanticsAction.scrollUp);
            }
            else
            {
                _performAction(DartRuntimePrimitives.RequireValue(_lastPointerDownLocation), SemanticsAction.scrollDown);
            }
        }
        setState(() =>
        {
            _lastPointerDownLocation = null;
        });
    }

    internal virtual void _performAction(Offset position, SemanticsAction action)
    {
        _pipelineOwner?.semanticsOwner?.performActionAt(position, action);
    }

    public override Widget build(BuildContext context)
    {
        return new CustomPaint(foregroundPainter: new _SemanticsDebuggerPainter__semantics_debugger(_pipelineOwner!, _generation, _lastPointerDownLocation, View.of(context).devicePixelRatio, widget.labelStyle), child: new GestureDetector(behavior: HitTestBehavior.opaque, onTap: () => _handleTap(), onLongPress: () => _handleLongPress(), onPanEnd: _handlePanEnd, excludeFromSemantics: true, child: new Listener(onPointerDown: _handlePointerDown, behavior: HitTestBehavior.opaque, child: new _IgnorePointerWithSemantics__semantics_debugger(child: widget.child))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SemanticsDebuggerPainter__semantics_debugger : CustomPainter
{
    public virtual PipelineOwner owner { get; private set; } = default!;
    public virtual long generation { get; private set; } = default!;
    public virtual Offset? pointerPosition { get; private set; }
    public virtual double devicePixelRatio { get; private set; } = default!;
    public virtual TextStyle labelStyle { get; private set; } = default!;

    internal _SemanticsDebuggerPainter__semantics_debugger(PipelineOwner owner, long generation, Offset? pointerPosition, double devicePixelRatio, TextStyle labelStyle)
    {
        this.owner = owner;
        this.generation = generation;
        this.pointerPosition = pointerPosition;
        this.devicePixelRatio = devicePixelRatio;
        this.labelStyle = labelStyle;
    }

    internal virtual SemanticsNode? _rootSemanticsNode
    {
        get
        {
            return owner.semanticsOwner?.rootSemanticsNode;
        }
    }
    public override void paint(Canvas canvas, Size size)
    {
        SemanticsNode? rootNode = _rootSemanticsNode;
        canvas.save();
        canvas.scale(1.0 / devicePixelRatio, 1.0 / devicePixelRatio);
        if (rootNode is not null)
        {
            _paint(canvas, rootNode, _findDepth(rootNode), 0L, 0L);
        }
        if (pointerPosition is not null)
        {
            Offset pointerPosition__value6557 = DartRuntimePrimitives.RequireValue(pointerPosition);
            var paintLocal = new Paint();
            paintLocal.color = new Color(2130743551L);
            canvas.drawCircle(DartRuntimePrimitives.RequireValue(pointerPosition), 10.0 * devicePixelRatio, paintLocal);
        }
        canvas.restore();
    }

    public override bool shouldRepaint(CustomPainter oldDelegate)
    {
        var __oldDelegate = (_SemanticsDebuggerPainter__semantics_debugger)oldDelegate;
        return (!Equals(owner, __oldDelegate.owner)) || (generation != __oldDelegate.generation) || (!Equals(pointerPosition, __oldDelegate.pointerPosition));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string getMessage(SemanticsNode node)
    {
        SemanticsData data = node.getSemanticsData();
        var annotations = new List<string>();
        var wantsTap = false;
        if (!Equals(data.flagsCollection.isChecked, CheckedState.none))
        {
            annotations.Add(Equals(data.flagsCollection.isChecked, CheckedState.isTrue) ? "checked" : "unchecked");
            wantsTap = true;
        }
        if (data.flagsCollection.isTextField)
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
        bool isScrollable = data.hasAction(SemanticsAction.scrollLeft) || data.hasAction(SemanticsAction.scrollRight) || data.hasAction(SemanticsAction.scrollUp) || data.hasAction(SemanticsAction.scrollDown);
        bool isAdjustable = data.hasAction(SemanticsAction.increase) || data.hasAction(SemanticsAction.decrease);
        if (isScrollable)
        {
            annotations.Add("scrollable");
        }
        if (isAdjustable)
        {
            annotations.Add("adjustable");
        }
        string message = default!;
        bool shouldIgnoreDuplicatedLabel = Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.android) && (data.attributedLabel.@string == data.tooltip);
        string tooltipAndLabel = string.Join("\n", new List<string>());
        if (tooltipAndLabel.Length == 0)
        {
            message = string.Join("; ", annotations);
        }
        else
        {
            string effectiveLabel = default!;
            if (data.textDirection is null)
            {
                effectiveLabel = $"{Unicode.FSI}{tooltipAndLabel}{Unicode.PDI}";
                annotations.Insert(checked((int)0L), "MISSING TEXT DIRECTION");
            }
            else
            {
                effectiveLabel = DartRuntimePrimitives.RequireValue(data.textDirection) switch { TextDirection.rtl => $"{Unicode.RLI}{tooltipAndLabel}{Unicode.PDI}", TextDirection.ltr => tooltipAndLabel, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
            }
            if (!Enumerable.Any(annotations))
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

    internal virtual void _paintMessage(Canvas canvas, SemanticsNode node)
    {
        string message = getMessage(node);
        if (message.Length == 0)
        {
            return;
        }
        Rect rectLocal = node.rect;
        canvas.save();
        canvas.clipRect(rectLocal);
        var textPainter = ((Func<TextPainter>)(() =>
{
    var __cascade = new TextPainter();
    __cascade.text = new TextSpan(style: labelStyle, text: message);
    __cascade.textDirection = TextDirection.ltr;
    __cascade.textAlign = TextAlign.center;
    __cascade.layout(maxWidth: rectLocal.width);
    return __cascade;
}))();
        textPainter.paint(canvas, Alignment.center.inscribe(textPainter.size, rectLocal).topLeft);
        textPainter.dispose();
        canvas.restore();
    }

    internal virtual long _findDepth(SemanticsNode node)
    {
        if (!node.hasChildren || node.mergeAllDescendantsIntoThisNode)
        {
            return 1L;
        }
        var childrenDepth = 0L;
        node.visitChildren((child) =>
        {
            childrenDepth = Math.Max(childrenDepth, _findDepth(child));
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        return childrenDepth + 1L;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _paint(Canvas canvas, SemanticsNode node, long rank, long indexInParent, long level)
    {
        if (node.traversalChildIdentifier is not null)
        {
            return;
        }
        canvas.save();
        if (node.transform is not null)
        {
            canvas.transform(node.transform!.storage);
        }
        Rect rectLocal = node.rect;
        if (!rectLocal.isEmpty)
        {
            Color lineColor = _colorForNode(indexInParent, level);
            Rect innerRect = rectLocal.deflate(rank * 1.0);
            if (innerRect.isEmpty)
            {
                var fillLocal = ((Func<Paint>)(() =>
{
    var __cascade = new Paint();
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
    var __cascade = new Paint();
    __cascade.color = new Color(4294967295L);
    __cascade.style = PaintingStyle.fill;
    return __cascade;
}))();
                canvas.drawRect(rectLocal, fillAlternate);
                var line = ((Func<Paint>)(() =>
{
    var __cascade = new Paint();
    __cascade.strokeWidth = rank * 2.0;
    __cascade.color = lineColor;
    __cascade.style = PaintingStyle.stroke;
    return __cascade;
}))();
                canvas.drawRect(innerRect, line);
            }
            _paintMessage(canvas, node);
        }
        if (!node.mergeAllDescendantsIntoThisNode)
        {
            long childRank = rank - 1L;
            long childLevel = level + 1L;
            var childIndex = 0L;
            node.visitChildren((child) =>
            {
                _paint(canvas, child, childRank, childIndex, childLevel);
                childIndex += 1L;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
        canvas.restore();
    }

    internal static Color _colorForNode(long index, long level)
    {
        return new HSLColor(1.0, 360.0 * new DartRandom(_getColorSeed(index, level)).nextDouble(), 1.0, 0.7).toColor();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static long _getColorSeed(long level, long index)
    {
        return (level * 10000L) + index;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _IgnorePointerWithSemantics__semantics_debugger : SingleChildRenderObjectWidget
{
    internal _IgnorePointerWithSemantics__semantics_debugger(Widget? child = null) : base(child: child)
    {
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderIgnorePointerWithSemantics__semantics_debugger();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _RenderIgnorePointerWithSemantics__semantics_debugger : RenderProxyBox
{
    internal _RenderIgnorePointerWithSemantics__semantics_debugger()
    {
    }

    public override bool hitTest(BoxHitTestResult result, Offset position) => false;
}

