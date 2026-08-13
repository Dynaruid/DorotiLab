// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/custom_paint.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Rendering;

public delegate List<CustomPainterSemantics> SemanticsBuilderCallback(Size size);

public abstract class CustomPainter : Listenable
{
    internal virtual Listenable? _repaint { get; private set; }

    protected CustomPainter(Listenable? repaint = null)
    {
        this._repaint = repaint;
    }

    public virtual void addListener(Action listener) => this._repaint?.addListener(listener);
    public virtual void removeListener(Action listener) => this._repaint?.removeListener(listener);
    public abstract void paint(Canvas canvas, Size size);
    public virtual Func<Size, List<CustomPainterSemantics>>? semanticsBuilder => null;
    public virtual bool shouldRebuildSemantics(CustomPainter oldDelegate) => shouldRepaint(oldDelegate);
    public abstract bool shouldRepaint(CustomPainter oldDelegate);
    public virtual bool? hitTest(Offset position) => null;
    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}({(this._repaint?.ToString() ?? "")})";
}

public class CustomPainterSemantics
{
    public virtual Key? key { get; private set; }
    public virtual Rect rect { get; private set; } = default!;
    public virtual Matrix4? transform { get; private set; }
    public virtual global::Doroti.Generated.Framework.Semantics.SemanticsProperties properties { get; private set; } = default!;
    public virtual HashSet<global::Doroti.Generated.Framework.Semantics.SemanticsTag>? tags { get; private set; }

    public CustomPainterSemantics(Key? key = null, Rect rect = default!, global::Doroti.Generated.Framework.Semantics.SemanticsProperties properties = default!, Matrix4? transform = null, HashSet<global::Doroti.Generated.Framework.Semantics.SemanticsTag>? tags = null)
    {
        this.key = key;
        this.rect = rect;
        this.properties = properties;
        this.transform = transform;
        this.tags = tags;
    }

}

public class RenderCustomPaint : RenderProxyBox
{
    internal virtual CustomPainter? _painter { get; set; } = default;
    internal virtual CustomPainter? _foregroundPainter { get; set; } = default;
    internal virtual Size _preferredSize { get; set; } = default!;
    public virtual bool isComplex { get; set; } = default!;
    public virtual bool willChange { get; set; } = default!;
    internal virtual Func<Size, List<CustomPainterSemantics>>? _backgroundSemanticsBuilder { get; set; } = default;
    internal virtual Func<Size, List<CustomPainterSemantics>>? _foregroundSemanticsBuilder { get; set; } = default;
    internal virtual List<global::Doroti.Generated.Framework.Semantics.SemanticsNode>? _backgroundSemanticsNodes { get; set; } = default;
    internal virtual List<global::Doroti.Generated.Framework.Semantics.SemanticsNode>? _foregroundSemanticsNodes { get; set; } = default;

    public RenderCustomPaint(CustomPainter? painter = null, CustomPainter? foregroundPainter = null, Size preferredSize = default, bool isComplex = false, bool willChange = false, RenderBox? child = null) : base(child)
    {
        this.isComplex = isComplex;
        this.willChange = willChange;
        this._painter = painter;
        this._foregroundPainter = foregroundPainter;
        this._preferredSize = preferredSize;
    }

    public virtual CustomPainter? painter
    {
        get => this._painter;
        set
        {
            var __value = value;
            if ((object.Equals(this._painter, __value)))
            {
                return;
            }
            CustomPainter? oldPainter__17619 = this._painter;
            _painter = __value;
            _didUpdatePainter(this._painter, oldPainter__17619);
        }
    }
    public virtual CustomPainter? foregroundPainter
    {
        get => this._foregroundPainter;
        set
        {
            var __value = value;
            if ((object.Equals(this._foregroundPainter, __value)))
            {
                return;
            }
            CustomPainter? oldPainter__18614 = this._foregroundPainter;
            _foregroundPainter = __value;
            _didUpdatePainter(this._foregroundPainter, oldPainter__18614);
        }
    }
    internal virtual void _didUpdatePainter(CustomPainter? newPainter, CustomPainter? oldPainter)
    {
        if ((newPainter is null))
        {
            DartRuntimePrimitives.Assert(() => (oldPainter is not null));
            markNeedsPaint();
        }
        else
        {
            if ((((oldPainter is null) || (!object.Equals(DartRuntimePrimitives.RuntimeType(newPainter), DartRuntimePrimitives.RuntimeType(oldPainter)))) || newPainter.shouldRepaint(oldPainter)))
            {
                markNeedsPaint();
            }
        }
        if (attached)
        {
            oldPainter?.removeListener(markNeedsPaint);
            newPainter?.addListener(markNeedsPaint);
        }
        if ((newPainter is null))
        {
            DartRuntimePrimitives.Assert(() => (oldPainter is not null));
            if (attached)
            {
                markNeedsSemanticsUpdate();
            }
        }
        else
        {
            if ((((oldPainter is null) || (!object.Equals(DartRuntimePrimitives.RuntimeType(newPainter), DartRuntimePrimitives.RuntimeType(oldPainter)))) || newPainter.shouldRebuildSemantics(oldPainter)))
            {
                markNeedsSemanticsUpdate();
            }
        }
    }

    public virtual global::Doroti.Flutter.Ui.Size preferredSize
    {
        get => this._preferredSize;
        set
        {
            var __value = value;
            if ((object.Equals(this.preferredSize, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _preferredSize = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public override double computeMinIntrinsicWidth(double height)
    {
        if ((child is null))
        {
            return (double.IsFinite(this.preferredSize.width) ? this.preferredSize.width : 0);
        }
        return base.computeMinIntrinsicWidth(height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        if ((child is null))
        {
            return (double.IsFinite(this.preferredSize.width) ? this.preferredSize.width : 0);
        }
        return base.computeMaxIntrinsicWidth(height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        if ((child is null))
        {
            return (double.IsFinite(this.preferredSize.height) ? this.preferredSize.height : 0);
        }
        return base.computeMinIntrinsicHeight(width);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        if ((child is null))
        {
            return (double.IsFinite(this.preferredSize.height) ? this.preferredSize.height : 0);
        }
        return base.computeMaxIntrinsicHeight(width);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        this._painter?.addListener(markNeedsPaint);
        this._foregroundPainter?.addListener(markNeedsPaint);
    }

    public override void detach()
    {
        this._painter?.removeListener(markNeedsPaint);
        this._foregroundPainter?.removeListener(markNeedsPaint);
        base.detach();
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        if (((this._foregroundPainter is not null) && ((this._foregroundPainter!.hitTest(position) ?? false))))
        {
            return true;
        }
        return base.hitTestChildren(result, position: position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestSelf(Offset position)
    {
        return ((this._painter is not null) && ((this._painter!.hitTest(position) ?? true)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        base.performLayout();
        markNeedsSemanticsUpdate();
    }

    public override Size computeSizeForNoChild(BoxConstraints constraints)
    {
        return constraints.constrain(this.preferredSize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _paintWithPainter(Canvas canvas, Offset offset, CustomPainter painter)
    {
        long debugPreviousCanvasSaveCount__22969 = default!;
        canvas.save();
        DartRuntimePrimitives.Assert(() =>
            {
                debugPreviousCanvasSaveCount__22969 = canvas.getSaveCount();
                return true;
            });
        if ((!object.Equals(offset, Offset.zero)))
        {
            canvas.translate(offset.dx, offset.dy);
        }
        painter.paint(canvas, size);
        DartRuntimePrimitives.Assert(() =>
            {
                long debugNewCanvasSaveCount__23665 = canvas.getSaveCount();
                if ((debugNewCanvasSaveCount__23665 > debugPreviousCanvasSaveCount__22969))
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"The {painter} custom painter called canvas.save() or canvas.saveLayer() at least " + $"{(debugNewCanvasSaveCount__23665 - debugPreviousCanvasSaveCount__22969)} more " + $"time{(((debugNewCanvasSaveCount__23665 - debugPreviousCanvasSaveCount__22969) == 1L) ? "" : "s")} " + "than it called canvas.restore()."), new ErrorDescription("This leaves the canvas in an inconsistent state and will probably result in a broken display."), new ErrorHint("You must pair each call to save()/saveLayer() with a later matching call to restore().") });
                }
                if ((debugNewCanvasSaveCount__23665 < debugPreviousCanvasSaveCount__22969))
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"The {painter} custom painter called canvas.restore() " + $"{(debugPreviousCanvasSaveCount__22969 - debugNewCanvasSaveCount__23665)} more " + $"time{(((debugPreviousCanvasSaveCount__22969 - debugNewCanvasSaveCount__23665) == 1L) ? "" : "s")} " + "than it called canvas.save() or canvas.saveLayer()."), new ErrorDescription("This leaves the canvas in an inconsistent state and will result in a broken display."), new ErrorHint("You should only call restore() if you first called save() or saveLayer().") });
                }
                return (debugNewCanvasSaveCount__23665 == debugPreviousCanvasSaveCount__22969);
            });
        canvas.restore();
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if ((this._painter is not null))
        {
            _paintWithPainter(((PaintingContext)context).canvas, offset, this._painter!);
            _setRasterCacheHints(context);
        }
        base.paint(context, offset);
        if ((this._foregroundPainter is not null))
        {
            _paintWithPainter(((PaintingContext)context).canvas, offset, this._foregroundPainter!);
            _setRasterCacheHints(context);
        }
    }

    internal virtual void _setRasterCacheHints(PaintingContext context)
    {
        if (this.isComplex)
        {
            context.setIsComplexHint();
        }
        if (this.willChange)
        {
            context.setWillChangeHint();
        }
    }

    public override void describeSemanticsConfiguration(global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        _backgroundSemanticsBuilder = this.painter?.semanticsBuilder;
        _foregroundSemanticsBuilder = this.foregroundPainter?.semanticsBuilder;
        config.isSemanticBoundary = ((this._backgroundSemanticsBuilder is not null) || (this._foregroundSemanticsBuilder is not null));
    }

    public override void assembleSemanticsNode(global::Doroti.Generated.Framework.Semantics.SemanticsNode node, global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration config, IEnumerable<global::Doroti.Generated.Framework.Semantics.SemanticsNode> children)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (((child is null) && (children.Count() != 0)))
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{this.GetType()} does not have a child widget but received a non-empty list of child SemanticsNode:\n" + $"{string.Join("\n", children)}") });
                }
                return true;
            });
        List<CustomPainterSemantics> backgroundSemantics__27324 = ((this._backgroundSemanticsBuilder is null ? new List<CustomPainterSemantics>() : this._backgroundSemanticsBuilder.Invoke(size)));
        _backgroundSemanticsNodes = _updateSemanticsChildren(this._backgroundSemanticsNodes, backgroundSemantics__27324);
        List<CustomPainterSemantics> foregroundSemantics__27596 = ((this._foregroundSemanticsBuilder is null ? new List<CustomPainterSemantics>() : this._foregroundSemanticsBuilder.Invoke(size)));
        _foregroundSemanticsNodes = _updateSemanticsChildren(this._foregroundSemanticsNodes, foregroundSemantics__27596);
        bool hasBackgroundSemantics__27844 = ((this._backgroundSemanticsNodes is not null) && (checked((long)(this._backgroundSemanticsNodes!.Count)) != 0));
        bool hasForegroundSemantics__27968 = ((this._foregroundSemanticsNodes is not null) && (checked((long)(this._foregroundSemanticsNodes!.Count)) != 0));
        var finalChildren__28087 = new List<global::Doroti.Generated.Framework.Semantics.SemanticsNode>();
        base.assembleSemanticsNode(node, config, finalChildren__28087);
    }

    public override void clearSemantics()
    {
        base.clearSemantics();
        _backgroundSemanticsNodes = null;
        _foregroundSemanticsNodes = null;
    }

    internal static List<global::Doroti.Generated.Framework.Semantics.SemanticsNode> _updateSemanticsChildren(List<global::Doroti.Generated.Framework.Semantics.SemanticsNode>? oldSemantics, List<CustomPainterSemantics>? newChildSemantics)
    {
        oldSemantics = (oldSemantics ?? new List<global::Doroti.Generated.Framework.Semantics.SemanticsNode>());
        newChildSemantics = (newChildSemantics ?? new List<CustomPainterSemantics>());
        DartRuntimePrimitives.Assert(() =>
            {
                DartMap<Key, long> keys__30127 = new DartMap<Key, long>();
                var information__30167 = new List<DiagnosticsNode>();
                for (var i__30217 = 0L; (i__30217 < checked((long)(newChildSemantics!.Count))); i__30217 += 1L)
                {
                    CustomPainterSemantics child__30302 = newChildSemantics[(int)(i__30217)];
                    if ((((CustomPainterSemantics)child__30302).key is not null))
                    {
                        if (keys__30127.ContainsKey(((CustomPainterSemantics)child__30302).key))
                        {
                            information__30167.Add(new ErrorDescription($"- duplicate key {((CustomPainterSemantics)child__30302).key} found at position {i__30217}"));
                        }
                        keys__30127[((CustomPainterSemantics)child__30302).key!] = i__30217;
                    }
                }
                if ((checked((long)(information__30167.Count)) != 0))
                {
                    information__30167.Insert(checked((int)0L), new ErrorSummary("Failed to update the list of CustomPainterSemantics:"));
                    throw new FlutterError(information__30167);
                }
                return true;
            });
        var newChildrenTop__30808 = 0L;
        var oldChildrenTop__30836 = 0L;
        long newChildrenBottom__30864 = (checked((long)(newChildSemantics.Count)) - 1L);
        long oldChildrenBottom__30922 = (checked((long)(oldSemantics.Count)) - 1L);
        var newChildren__30978 = new List<global::Doroti.Generated.Framework.Semantics.SemanticsNode?>(System.Linq.Enumerable.Repeat<global::Doroti.Generated.Framework.Semantics.SemanticsNode?>(null, checked((int)checked((long)(newChildSemantics.Count)))));
        while ((((oldChildrenTop__30836 <= oldChildrenBottom__30922)) && ((newChildrenTop__30808 <= newChildrenBottom__30864))))
        {
            global::Doroti.Generated.Framework.Semantics.SemanticsNode oldChild__31208 = oldSemantics[(int)(oldChildrenTop__30836)];
            CustomPainterSemantics newSemantics__31284 = newChildSemantics[(int)(newChildrenTop__30808)];
            if (!_canUpdateSemanticsChild(oldChild__31208, newSemantics__31284))
            {
                break;
            }
            global::Doroti.Generated.Framework.Semantics.SemanticsNode newChild__31446 = _updateSemanticsChild(oldChild__31208, newSemantics__31284);
            newChildren__30978[(int)(newChildrenTop__30808)] = newChild__31446;
            newChildrenTop__30808 += 1L;
            oldChildrenTop__30836 += 1L;
        }
        while ((((oldChildrenTop__30836 <= oldChildrenBottom__30922)) && ((newChildrenTop__30808 <= newChildrenBottom__30864))))
        {
            global::Doroti.Generated.Framework.Semantics.SemanticsNode oldChild__31766 = oldSemantics[(int)(oldChildrenBottom__30922)];
            CustomPainterSemantics newChild__31845 = newChildSemantics[(int)(newChildrenBottom__30864)];
            if (!_canUpdateSemanticsChild(oldChild__31766, newChild__31845))
            {
                break;
            }
            oldChildrenBottom__30922 -= 1L;
            newChildrenBottom__30864 -= 1L;
        }
        bool haveOldChildren__32114 = (oldChildrenTop__30836 <= oldChildrenBottom__30922);
        DartMap<Key, global::Doroti.Generated.Framework.Semantics.SemanticsNode> oldKeyedChildren__32208 = default!;
        if (haveOldChildren__32114)
        {
            oldKeyedChildren__32208 = new DartMap<Key, global::Doroti.Generated.Framework.Semantics.SemanticsNode>();
            while ((oldChildrenTop__30836 <= oldChildrenBottom__30922))
            {
                global::Doroti.Generated.Framework.Semantics.SemanticsNode oldChild__32382 = oldSemantics[(int)(oldChildrenTop__30836)];
                if ((((global::Doroti.Generated.Framework.Semantics.SemanticsNode)oldChild__32382).key is not null))
                {
                    oldKeyedChildren__32208[((global::Doroti.Generated.Framework.Semantics.SemanticsNode)oldChild__32382).key!] = oldChild__32382;
                }
                oldChildrenTop__30836 += 1L;
            }
        }
        while ((newChildrenTop__30808 <= newChildrenBottom__30864))
        {
            global::Doroti.Generated.Framework.Semantics.SemanticsNode? oldChild__32676 = default!;
            CustomPainterSemantics newSemantics__32721 = newChildSemantics[(int)(newChildrenTop__30808)];
            if (haveOldChildren__32114)
            {
                Key? key__32819 = ((CustomPainterSemantics)newSemantics__32721).key;
                if ((key__32819 is not null))
                {
                    oldChild__32676 = oldKeyedChildren__32208.GetValueOrDefault(key__32819);
                    if ((oldChild__32676 is not null))
                    {
                        if (_canUpdateSemanticsChild(oldChild__32676, newSemantics__32721))
                        {
                            oldKeyedChildren__32208.remove(key__32819);
                        }
                        else
                        {
                            oldChild__32676 = null;
                        }
                    }
                }
            }
            DartRuntimePrimitives.Assert(() => ((oldChild__32676 is null) || _canUpdateSemanticsChild(oldChild__32676, newSemantics__32721)));
            global::Doroti.Generated.Framework.Semantics.SemanticsNode newChild__33448 = _updateSemanticsChild(oldChild__32676, newSemantics__32721);
            DartRuntimePrimitives.Assert(() => ((object.Equals(oldChild__32676, newChild__33448)) || (oldChild__32676 is null)));
            newChildren__30978[(int)(newChildrenTop__30808)] = newChild__33448;
            newChildrenTop__30808 += 1L;
        }
        DartRuntimePrimitives.Assert(() => (oldChildrenTop__30836 == (oldChildrenBottom__30922 + 1L)));
        DartRuntimePrimitives.Assert(() => (newChildrenTop__30808 == (newChildrenBottom__30864 + 1L)));
        DartRuntimePrimitives.Assert(() => ((checked((long)(newChildSemantics.Count)) - newChildrenTop__30808) == (checked((long)(oldSemantics.Count)) - oldChildrenTop__30836)));
        newChildrenBottom__30864 = (checked((long)(newChildSemantics.Count)) - 1L);
        oldChildrenBottom__30922 = (checked((long)(oldSemantics.Count)) - 1L);
        while ((((oldChildrenTop__30836 <= oldChildrenBottom__30922)) && ((newChildrenTop__30808 <= newChildrenBottom__30864))))
        {
            global::Doroti.Generated.Framework.Semantics.SemanticsNode oldChild__34141 = oldSemantics[(int)(oldChildrenTop__30836)];
            CustomPainterSemantics newSemantics__34217 = newChildSemantics[(int)(newChildrenTop__30808)];
            DartRuntimePrimitives.Assert(() => _canUpdateSemanticsChild(oldChild__34141, newSemantics__34217));
            global::Doroti.Generated.Framework.Semantics.SemanticsNode newChild__34357 = _updateSemanticsChild(oldChild__34141, newSemantics__34217);
            DartRuntimePrimitives.Assert(() => (object.Equals(oldChild__34141, newChild__34357)));
            newChildren__30978[(int)(newChildrenTop__30808)] = newChild__34357;
            newChildrenTop__30808 += 1L;
            oldChildrenTop__30836 += 1L;
        }
        DartRuntimePrimitives.Assert(() =>
            {
                foreach (var node__34591 in newChildren__30978)
                {
                    DartRuntimePrimitives.Assert(() => (node__34591 is not null));
                }
                return true;
            });
        return newChildren__30978.cast<global::Doroti.Generated.Framework.Semantics.SemanticsNode>().ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static bool _canUpdateSemanticsChild(global::Doroti.Generated.Framework.Semantics.SemanticsNode oldChild, CustomPainterSemantics newSemantics)
    {
        return (object.Equals(((global::Doroti.Generated.Framework.Semantics.SemanticsNode)oldChild).key, ((CustomPainterSemantics)newSemantics).key));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Generated.Framework.Semantics.SemanticsNode _updateSemanticsChild(global::Doroti.Generated.Framework.Semantics.SemanticsNode? oldChild, CustomPainterSemantics newSemantics)
    {
        DartRuntimePrimitives.Assert(() => ((oldChild is null) || _canUpdateSemanticsChild(oldChild, newSemantics)));
        global::Doroti.Generated.Framework.Semantics.SemanticsNode newChild__35556 = (oldChild ?? new global::Doroti.Generated.Framework.Semantics.SemanticsNode(key: ((CustomPainterSemantics)newSemantics).key));
        global::Doroti.Generated.Framework.Semantics.SemanticsProperties properties__35648 = ((CustomPainterSemantics)newSemantics).properties;
        var config__35696 = new global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration();
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).role is not null))
        {
            config__35696.role = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).role);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).sortKey is not null))
        {
            config__35696.sortKey = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).sortKey;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).@checked is not null))
        {
            config__35696.isChecked = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).@checked;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).mixed is not null))
        {
            config__35696.isCheckStateMixed = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).mixed;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).selected is not null))
        {
            config__35696.isSelected = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).selected);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).button is not null))
        {
            config__35696.isButton = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).button);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).expanded is not null))
        {
            config__35696.isExpanded = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).expanded;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).link is not null))
        {
            config__35696.isLink = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).link);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).linkUrl is not null))
        {
            config__35696.linkUrl = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).linkUrl;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).textField is not null))
        {
            config__35696.isTextField = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).textField);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).slider is not null))
        {
            config__35696.isSlider = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).slider);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).keyboardKey is not null))
        {
            config__35696.isKeyboardKey = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).keyboardKey);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).readOnly is not null))
        {
            config__35696.isReadOnly = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).readOnly);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).focusable is not null))
        {
            config__35696.isFocusable = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).focusable);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).focused is not null))
        {
            config__35696.isFocused = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).focused;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).accessibilityFocusBlockType is not null))
        {
            config__35696.accessibilityFocusBlockType = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).accessibilityFocusBlockType);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).enabled is not null))
        {
            config__35696.isEnabled = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).enabled;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).inMutuallyExclusiveGroup is not null))
        {
            config__35696.isInMutuallyExclusiveGroup = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).inMutuallyExclusiveGroup);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).obscured is not null))
        {
            config__35696.isObscured = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).obscured);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).multiline is not null))
        {
            config__35696.isMultiline = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).multiline);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).hidden is not null))
        {
            config__35696.isHidden = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).hidden);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).header is not null))
        {
            config__35696.isHeader = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).header);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).headingLevel is not null))
        {
            config__35696.headingLevel = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).headingLevel);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).scopesRoute is not null))
        {
            config__35696.scopesRoute = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).scopesRoute);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).namesRoute is not null))
        {
            config__35696.namesRoute = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).namesRoute);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).liveRegion is not null))
        {
            config__35696.liveRegion = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).liveRegion);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).isRequired is not null))
        {
            config__35696.isRequired = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).isRequired;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).maxValueLength is not null))
        {
            config__35696.maxValueLength = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).maxValueLength;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).currentValueLength is not null))
        {
            config__35696.currentValueLength = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).currentValueLength;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).toggled is not null))
        {
            config__35696.isToggled = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).toggled;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).image is not null))
        {
            config__35696.isImage = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).image);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).label is not null))
        {
            config__35696.label = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).label!;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).value is not null))
        {
            config__35696.value = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).value!;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).increasedValue is not null))
        {
            config__35696.increasedValue = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).increasedValue!;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).decreasedValue is not null))
        {
            config__35696.decreasedValue = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).decreasedValue!;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).hint is not null))
        {
            config__35696.hint = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).hint!;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).identifier is not null))
        {
            config__35696.identifier = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).identifier!;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).traversalParentIdentifier is not null))
        {
            config__35696.traversalParentIdentifier = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).traversalParentIdentifier;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).traversalChildIdentifier is not null))
        {
            config__35696.traversalChildIdentifier = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).traversalChildIdentifier;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).tooltip is not null))
        {
            config__35696.tooltip = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).tooltip!;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).hintOverrides is not null))
        {
            config__35696.hintOverrides = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).hintOverrides;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).tagForChildren is not null))
        {
            config__35696.addTagForChildren(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).tagForChildren!);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).controlsNodes is not null))
        {
            config__35696.controlsNodes = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).controlsNodes;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).hint is not null))
        {
            config__35696.hint = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).hint!;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).textDirection is not null))
        {
            config__35696.textDirection = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).textDirection;
        }
        if ((!object.Equals(((global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration)config__35696).validationResult, ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).validationResult)))
        {
            config__35696.validationResult = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).validationResult;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).hitTestBehavior is not null))
        {
            config__35696.hitTestBehavior = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).hitTestBehavior);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).inputType is not null))
        {
            config__35696.inputType = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).inputType);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).minValue is not null))
        {
            config__35696.minValue = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).minValue;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).maxValue is not null))
        {
            config__35696.maxValue = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).maxValue;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onTap is not null))
        {
            config__35696.onTap = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onTap;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onLongPress is not null))
        {
            config__35696.onLongPress = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onLongPress;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onScrollLeft is not null))
        {
            config__35696.onScrollLeft = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onScrollLeft;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onScrollRight is not null))
        {
            config__35696.onScrollRight = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onScrollRight;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onScrollUp is not null))
        {
            config__35696.onScrollUp = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onScrollUp;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onScrollDown is not null))
        {
            config__35696.onScrollDown = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onScrollDown;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onIncrease is not null))
        {
            config__35696.onIncrease = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onIncrease;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onDecrease is not null))
        {
            config__35696.onDecrease = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onDecrease;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onCopy is not null))
        {
            config__35696.onCopy = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onCopy;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onCut is not null))
        {
            config__35696.onCut = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onCut;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onPaste is not null))
        {
            config__35696.onPaste = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onPaste;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onMoveCursorForwardByCharacter is not null))
        {
            config__35696.onMoveCursorForwardByCharacter = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onMoveCursorForwardByCharacter;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onMoveCursorBackwardByCharacter is not null))
        {
            config__35696.onMoveCursorBackwardByCharacter = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onMoveCursorBackwardByCharacter;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onMoveCursorForwardByWord is not null))
        {
            config__35696.onMoveCursorForwardByWord = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onMoveCursorForwardByWord;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onMoveCursorBackwardByWord is not null))
        {
            config__35696.onMoveCursorBackwardByWord = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onMoveCursorBackwardByWord;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onSetSelection is not null))
        {
            config__35696.onSetSelection = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onSetSelection;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onSetText is not null))
        {
            config__35696.onSetText = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onSetText;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onDidGainAccessibilityFocus is not null))
        {
            config__35696.onDidGainAccessibilityFocus = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onDidGainAccessibilityFocus;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onDidLoseAccessibilityFocus is not null))
        {
            config__35696.onDidLoseAccessibilityFocus = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onDidLoseAccessibilityFocus;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onFocus is not null))
        {
            config__35696.onFocus = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onFocus;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onDismiss is not null))
        {
            config__35696.onDismiss = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onDismiss;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onExpand is not null))
        {
            config__35696.onExpand = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onExpand;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onCollapse is not null))
        {
            config__35696.onCollapse = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)properties__35648).onCollapse;
        }
        newChild__35556.updateWith(config: config__35696, childrenInInversePaintOrder: new List<global::Doroti.Generated.Framework.Semantics.SemanticsNode>());
        ((Func<global::Doroti.Generated.Framework.Semantics.SemanticsNode>)(() =>
{
    var __cascade = newChild__35556;
    __cascade.rect = ((CustomPainterSemantics)newSemantics).rect;
    __cascade.transform = ((CustomPainterSemantics)newSemantics).transform;
    __cascade.tags = ((CustomPainterSemantics)newSemantics).tags;
    return __cascade;
}))();
        return newChild__35556;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new MessageProperty("painter", $"{this.painter}"));
        properties.add(new MessageProperty("foregroundPainter", $"{this.foregroundPainter}", level: ((this.foregroundPainter is not null) ? DiagnosticLevel.info : DiagnosticLevel.fine)));
        properties.add(new DiagnosticsProperty<global::Doroti.Flutter.Ui.Size>("preferredSize", this.preferredSize, defaultValue: Size.zero));
        properties.add(new DiagnosticsProperty<bool>("isComplex", this.isComplex, defaultValue: false));
        properties.add(new DiagnosticsProperty<bool>("willChange", this.willChange, defaultValue: false));
    }

}

